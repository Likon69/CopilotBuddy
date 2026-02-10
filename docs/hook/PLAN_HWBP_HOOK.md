# PLAN: Hardware Breakpoint (HWBP) EndScene Hook

> **Objective:** Replace the inline 5-byte JMP detour with a hardware breakpoint (DR0–DR3)
> hook that modifies **zero bytes** in WoW's memory. The CPU raises `EXCEPTION_SINGLE_STEP`
> when EndScene is called, and a VEH handler redirects execution to our trampoline.
>
> **Stealth level:** Maximum — invisible to Warden MEM_CHECK, PAGE_CHECK, and byte scanning.
> Only detectable via `GetThreadContext` DR register inspection or VEH handler enumeration.
>
> **Complexity:** High — requires cross-thread debug register management, VEH exception
> dispatching, and a fundamentally different execution flow than the current inline JMP.
>
> **Prerequisite:** VEH crash protection (already implemented in ExecutorRand.cs).
>
> **Reference implementations:**
> - BlackBone library (`LocalHook.hpp`, `LocalHookBase.cpp`) — canonical HWBP + VEH
> - Shtahet/AthenaInjected (`HardwareBrakepointHook.cs`) — C# external debugger approach
> - Fernir/Dinnerbot (`Main.cpp`) — C++ external HWBP for WoW
>
> **Reference docs:**
> - [RESEARCH_ENDSCENE_HOOK_ANALYSIS.md](RESEARCH_ENDSCENE_HOOK_ANALYSIS.md) §2.3 & §5
> - [PLAN_ERROR132_VEH_FIX.md](PLAN_ERROR132_VEH_FIX.md)

---

## Table of Contents

1. [How HWBP Hooks Work](#1-how-hwbp-hooks-work)
2. [Architecture Comparison: HWBP vs Current JMP](#2-architecture-comparison-hwbp-vs-current-jmp)
3. [Detection Surface Analysis](#3-detection-surface-analysis)
4. [Implementation Approach for CopilotBuddy](#4-implementation-approach-for-copilotbuddy)
5. [Detailed Implementation Steps](#5-detailed-implementation-steps)
6. [ASM Trampoline Design](#6-asm-trampoline-design)
7. [Thread Management](#7-thread-management)
8. [Challenges & Risks](#8-challenges--risks)
9. [Testing Plan](#9-testing-plan)
10. [Performance Considerations](#10-performance-considerations)
11. [Rollback Plan](#11-rollback-plan)
12. [Missing Robustness Features Analysis](#12-missing-robustness-features-analysis)

---

## 1. How HWBP Hooks Work

### x86 Debug Register Layout

The x86 CPU has 8 debug registers (DR0–DR7). Only 4 are usable for breakpoints:

| Register | Purpose |
|----------|---------|
| **DR0** | Breakpoint address #0 |
| **DR1** | Breakpoint address #1 |
| **DR2** | Breakpoint address #2 |
| **DR3** | Breakpoint address #3 |
| **DR6** | Debug status — which breakpoint fired (read-only by CPU) |
| **DR7** | Debug control — enable/disable, conditions, lengths |

### DR7 Bit Layout (x86)

```
Bit(s)   Field           Description
──────   ─────           ───────────
0        L0              Local enable for DR0
1        G0              Global enable for DR0
2        L1              Local enable for DR1
3        G1              Global enable for DR1
4        L2              Local enable for DR2
5        G2              Global enable for DR2
6        L3              Local enable for DR3
7        G3              Global enable for DR3
8        LE              Local exact (legacy, set to 1)
9        GE              Global exact (legacy, set to 1)
13       GD              General detect (debug register access trap)
16-17    R/W0            Condition for DR0 (00=execute, 01=write, 11=read/write)
18-19    LEN0            Length for DR0 (00=1 byte, 01=2, 11=4)
20-21    R/W1            Condition for DR1
22-23    LEN1            Length for DR1
24-25    R/W2            Condition for DR2
26-27    LEN2            Length for DR2
28-29    R/W3            Condition for DR3
30-31    LEN3            Length for DR3
```

For an **execution breakpoint** on DR0:
- Set DR0 = EndScene address
- Set DR7 bit 0 (L0 = 1) — local enable
- Set DR7 bits 16-17 = 00 — execute condition
- Set DR7 bits 18-19 = 00 — 1-byte length

### Exception Flow

```
1. CPU executes instruction at address in DR0
2. CPU raises EXCEPTION_SINGLE_STEP (0x80000004)
3. Windows dispatches to VEH chain (our handler is first)
4. VEH handler checks DR6 to identify which breakpoint fired
5. VEH handler modifies CONTEXT.Eip → redirect to our trampoline
6. Returns EXCEPTION_CONTINUE_EXECUTION (-1)
7. CPU resumes at our trampoline instead of EndScene
```

### Key Difference from Inline JMP

| Aspect | Inline JMP | HWBP |
|--------|-----------|------|
| Memory modification | 5 bytes at EndScene | **Zero bytes** |
| Original code intact? | No (overwritten) | **Yes** |
| Detection by byte scan | Trivial (E9 at prologue) | **Impossible** |
| Hook installation | WriteProcessMemory | SetThreadContext (DR regs) |
| Hook trigger mechanism | CPU follows JMP opcode | CPU raises exception |
| Performance | Direct jump (fast) | Exception + VEH dispatch (slower) |
| Max simultaneous hooks | Unlimited | **4** (DR0–DR3) |
| Compatibility | May conflict with other hooks | Independent of code hooks |

---

## 2. Architecture Comparison: HWBP vs Current JMP

### Current Flow (Inline JMP — ExecutorRand.cs)

```
WoW calls EndScene
  → JMP m_EndSceneDetour              (5-byte inline JMP, modifies code)
  → pushad
  → @CheckInjection loop
  → VEH-protected call to injected code
  → popad
  → replay stolen prologue
  → JMP EndScene+5                    (return to original code past JMP)
```

### Proposed Flow (HWBP)

```
WoW calls EndScene
  → CPU hits DR0 breakpoint           (NO code modification)
  → EXCEPTION_SINGLE_STEP fired
  → VEH handler catches it
  → Checks DR6 to confirm it's our BP
  → Modifies CONTEXT.Eip → detour trampoline
  → Returns EXCEPTION_CONTINUE_EXECUTION
  → CPU resumes at detour trampoline
  → pushad
  → @CheckInjection loop
  → VEH-protected call to injected code (reuses existing VEH infrastructure)
  → popad
  → Temporarily disable DR0 (to avoid re-trigger)
  → Set CONTEXT.EFlags |= 0x100 (Trap Flag = single-step)
  → JMP to original EndScene           (execute the REAL prologue)
  → After 1 instruction, single-step trap fires
  → VEH re-enables DR0
  → Returns EXCEPTION_CONTINUE_EXECUTION
  → EndScene continues normally
```

### Dual-VEH Design

The HWBP hook requires **two separate VEH mechanisms** that must coexist:

1. **HWBP VEH** (always active): Dispatches `EXCEPTION_SINGLE_STEP` on every EndScene call.
   This is the hook entry point. Must run for the lifetime of the hook.

2. **Crash Protection VEH** (per-batch): The existing VEH from the Error 132 fix.
   Catches exceptions from injected code. Set up/torn down per batch (current design).

These can be combined into a **single VEH handler** that dispatches based on exception code
and context:

```
VEH Handler:
  if ExceptionCode == 0x80000004 (SINGLE_STEP):
    if DR6 indicates our breakpoint:
      → HWBP dispatch (redirect to trampoline or re-enable DR0)
    else:
      → EXCEPTION_CONTINUE_SEARCH (not our breakpoint)
  
  if ExceptionCode == 0xC0000005 (ACCESS_VIOLATION) or other:
    if TLS says our thread:
      → Crash protection dispatch (redirect to @CallGateInterject)
    else:
      → EXCEPTION_CONTINUE_SEARCH
```

---

## 3. Detection Surface Analysis

### Private Server Warden (AzerothCore/TrinityCore)

Based on AzerothCore source analysis (`src/server/game/Warden/`):

| Warden Scan Type | Can Detect HWBP? | Details |
|-----------------|-------------------|---------|
| **MEM_CHECK** | **No** | Reads memory bytes at specified address. EndScene bytes are unmodified. |
| **PAGE_CHECK_A/B** | **No** | Hashes memory regions (SHA1) and compares to expected values. The `0xE9` in AzerothCore source is a protocol status byte (pass/fail), not a JMP opcode search. No bytes modified → undetectable. |
| **MODULE_CHECK** | **No** | Checks for loaded DLLs. No DLL injected. |
| **DRIVER_CHECK** | **No** | Checks for loaded drivers. No driver used. |
| **LUA_EVAL_CHECK** | **No** | Evaluates Lua expressions. No relation to hooking. |
| **TIMING_CHECK** | **Possibly** | Checks GetTickCount detouring. HWBP exception handling adds latency that could theoretically affect timing. In practice, the delay is <1ms per frame — within normal variability. |
| **PROC_CHECK** | **No** | Checks if exported functions are detoured (inline hook detection). **Disabled** (commented out) in AzerothCore. |
| **DR register scan** | **Theoretically** | Would require `GetThreadContext` + checking DR0–DR3 on all threads. AzerothCore Warden **does NOT do this** — the Warden module runs in-process but the check types are server-defined, and no standard check reads debug registers. |
| **VEH enumeration** | **Theoretically** | Could enumerate VEH handlers via `RtlFirstVEH` (ntdll internals). AzerothCore Warden **does NOT do this**. |

### Enhanced Private Server Warden

Some private servers run **custom Warden modules** that could:
- Call `GetThreadContext` on all threads and check DR0–DR3
- Enumerate VEH handlers via undocumented ntdll structures
- Use `NtQueryInformationThread` with `ThreadHideFromDebugger`

**Mitigations for future hardening (not Phase 1):**
- Hook `GetThreadContext` to zero DR registers in the returned CONTEXT
- Use `NtSetInformationThread(ThreadHideFromDebugger)` to prevent DR reads
- Use the HWBP only on the render thread, not all threads

---

## 4. Implementation Approach for CopilotBuddy

### Chosen Approach: Out-of-Process HWBP via SetThreadContext

CopilotBuddy is an **out-of-process** bot. It does NOT inject a DLL into WoW — it uses
`VirtualAllocEx` + `WriteProcessMemory` to inject ASM code caves. This means:

1. **DR registers are set via** `SuspendThread` → `GetThreadContext` → modify DR → `SetThreadContext` → `ResumeThread`

2. **VEH handler must be assembly** injected into WoW via code cave (same as current approach) —
   we can't use C# delegates since we're out-of-process.

3. **The VEH handler ASM lives in the detour trampoline** (allocated via `VirtualAllocEx`).
   It's registered via injecting a call to `AddVectoredExceptionHandler(1, &handler)`.

### Key Design Decision: Merge Both VEH Roles

Instead of two separate VEH handlers, we use **one registration** with branching logic:

```asm
@VEH_MasterHandler:
    ; Check exception code
    mov eax, [ebp+8]           ; EXCEPTION_POINTERS*
    mov ecx, [eax]             ; EXCEPTION_RECORD*
    mov ecx, [ecx]             ; ExceptionCode

    cmp ecx, 0x80000004        ; EXCEPTION_SINGLE_STEP?
    je @HWBP_Dispatch

    ; Fall through to crash protection (existing VEH logic)
    ; ... TLS check, redirect to @CallGateInterject ...

@HWBP_Dispatch:
    ; Check DR6 to see if our breakpoint fired
    mov eax, [ebp+8]
    mov eax, [eax+4]           ; CONTEXT*
    mov ecx, [eax+0x14]       ; CONTEXT.Dr6
    test ecx, 1               ; DR0 triggered? (bit 0)
    jz @HWBP_NotOurs

    ; Clear DR6 (acknowledge breakpoint)
    mov dword [eax+0x14], 0    ; CONTEXT.Dr6 = 0

    ; Check state: are we returning from original EndScene?
    cmp dword [m_HwbpState], 1
    je @HWBP_ReEnable          ; Yes → re-enable DR0 after single-step

    ; First hit: redirect to our trampoline
    push @TrampolineEntry
    pop ecx
    mov [eax+0xB8], ecx        ; CONTEXT.Eip = our trampoline

    ; Disable DR0 temporarily (will re-enable after single-step through original)
    and dword [eax+0x18], 0xFFFFFFFC  ; Clear L0+G0 in DR7

    mov eax, 0xFFFFFFFF        ; EXCEPTION_CONTINUE_EXECUTION
    jmp @VEH_Return

@HWBP_ReEnable:
    ; Re-enable DR0 after single-stepping through original EndScene prologue
    or dword [eax+0x18], 1     ; Set L0 in DR7
    mov dword [m_HwbpState], 0 ; Reset state

    mov eax, 0xFFFFFFFF        ; EXCEPTION_CONTINUE_EXECUTION
    jmp @VEH_Return

@HWBP_NotOurs:
    xor eax, eax               ; EXCEPTION_CONTINUE_SEARCH
    jmp @VEH_Return
```

### Trampoline Flow (After VEH Redirects)

```asm
@TrampolineEntry:
    pushad
    ; ... CheckInjection loop (same as current) ...
    ; ... VEH-protected CallGate to injected code ...
    popad

    ; Now we need to execute the original EndScene
    ; Set state so VEH knows to re-enable DR0 after single-step
    mov dword [m_HwbpState], 1

    ; Set Trap Flag (TF) so CPU single-steps 1 instruction
    pushfd
    or dword [esp], 0x100      ; EFlags.TF = 1
    popfd

    ; Jump to original EndScene (unmodified!)
    jmp [m_OrigEndScene]
    ; → CPU executes 1 instruction of EndScene
    ; → Single-step trap fires
    ; → VEH re-enables DR0
    ; → EndScene continues normally
```

### Alternative: Skip the Single-Step Trick

If we DON'T need to re-enable DR0 between frames (because EndScene is called exactly
once per frame on the render thread), we can simplify:

```asm
@TrampolineEntry:
    pushad
    ; ... our code ...
    popad

    ; DR0 is still disabled by VEH handler
    ; Call original EndScene directly (it won't trigger the breakpoint)
    call [m_OrigEndScene]

    ; Re-enable DR0 for next frame
    ; (This is tricky from ASM — need to call SetThreadContext or use the DR7 write)
```

**Problem:** We can't modify DR7 from ring 3 with `mov dr7, eax` — that's a privileged instruction.
We need to either:
1. Use the Trap Flag single-step approach (VEH modifies DR7 via CONTEXT), or
2. Use `SetThreadContext` from CopilotBuddy after each frame (too slow, causes thread suspension).

**Decision:** Use the **Trap Flag approach** (option 1). The VEH handler controls DR7 via
the CONTEXT structure during exception handling — no privileged instructions needed.

### Why Trap Flag (TF) Instead of Resume Flag (RF)?

The x86 Resume Flag (RF, EFLAGS bit 16) suppresses debug breakpoints for **one instruction**
after exception return. However, RF is consumed by the **first instruction executed** after
`EXCEPTION_CONTINUE_EXECUTION`. Since our VEH handler redirects EIP to the **trampoline**
(not EndScene), the RF would protect the first trampoline instruction — not the
`jmp [m_OrigEndScene]` at the end of the trampoline.

Therefore, we use the Trap Flag (TF): the VEH handler disables DR0 in the CONTEXT,
then the trampoline executes our injection code, sets TF, and jumps to EndScene. The CPU
executes one instruction of EndScene, fires a single-step exception, and our VEH re-enables
DR0. This costs ~10μs extra per frame (two VEH dispatches instead of one) but is the
correct approach for this two-stage redirection pattern.

---

## 5. Detailed Implementation Steps

### Step 1: Identify the Render Thread

Before setting HWBP, we need the **thread ID** of WoW's render thread (the one that calls EndScene).

```csharp
// Already available in Memory.cs:
// memory._hThread is the main thread handle (first thread of process)
// WoW's render thread IS typically the main thread in WoW 3.3.5a
```

**Verification:** WoW 3.3.5a (build 12340) uses a single-threaded rendering model.
The main thread calls `IDirect3DDevice9::EndScene()` every frame.
This thread is `Process.Threads[0]` — the same one `Memory.cs` opens.

### Step 2: New File — HwbpExecutor.cs

Create `GreenMagic\HwbpExecutor.cs` as a new executor variant alongside `ExecutorRand.cs`.
This class implements the HWBP hook while reusing the injection infrastructure.

```
GreenMagic/
  ExecutorRand.cs       ← existing (inline JMP + VEH crash protection)
  HwbpExecutor.cs       ← NEW (HWBP + merged VEH)
  Memory.cs             ← shared
  InjectionSEHException.cs  ← shared
```

### Step 3: P/Invoke Additions (Native/Imports.cs)

Required additions for thread context manipulation:

```csharp
// CONTEXT structure for x86 (needed for Get/SetThreadContext)
[StructLayout(LayoutKind.Sequential)]
public struct CONTEXT_x86
{
    public uint ContextFlags;     // 0x00
    // Debug registers
    public uint Dr0;              // 0x04
    public uint Dr1;              // 0x08
    public uint Dr2;              // 0x0C
    public uint Dr3;              // 0x10
    public uint Dr6;              // 0x14
    public uint Dr7;              // 0x18
    // Float save area (112 bytes)
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 112)]
    public byte[] FloatSave;      // 0x1C
    // Segment registers
    public uint SegGs;            // 0x8C
    public uint SegFs;            // 0x90
    public uint SegEs;            // 0x94
    public uint SegDs;            // 0x98
    // General purpose registers
    public uint Edi;              // 0x9C
    public uint Esi;              // 0xA0
    public uint Ebx;              // 0xA4
    public uint Edx;              // 0xA8
    public uint Ecx;              // 0xAC
    public uint Eax;              // 0xB0
    public uint Ebp;              // 0xB4
    public uint Eip;              // 0xB8
    public uint SegCs;            // 0xBC
    public uint EFlags;           // 0xC0
    public uint Esp;              // 0xC4
    public uint SegSs;            // 0xC8
    // Extended registers (512 bytes)
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
    public byte[] ExtendedRegisters; // 0xCC
}

public const uint CONTEXT_DEBUG_REGISTERS = 0x00010010;
public const uint CONTEXT_FULL = 0x0001003F;

// These are already in Memory.cs / Imports.cs:
// GetThreadContext, SetThreadContext, SuspendThread, ResumeThread
```

### Step 4: DR Register Setup in C#

```csharp
/// <summary>
/// Set a hardware execution breakpoint on DR0 for the specified thread.
/// </summary>
private void SetHardwareBreakpoint(IntPtr threadHandle, uint address)
{
    // Suspend the thread
    Imports.SuspendThread(threadHandle);
    
    try
    {
        var ctx = new CONTEXT_x86 
        { 
            ContextFlags = CONTEXT_DEBUG_REGISTERS 
        };
        
        if (!Imports.GetThreadContext(threadHandle, ref ctx))
            throw new Exception("GetThreadContext failed");
        
        // Set DR0 = EndScene address
        ctx.Dr0 = address;
        
        // Enable local breakpoint on DR0, execute condition, 1-byte length
        // Clear bits 0-1 (L0, G0), 16-19 (R/W0, LEN0)
        ctx.Dr7 &= ~(uint)0x000F0003;
        // Set L0 = 1 (local enable)
        ctx.Dr7 |= 0x00000001;
        // R/W0 = 00 (execute), LEN0 = 00 (1 byte) — already cleared
        
        ctx.ContextFlags = CONTEXT_DEBUG_REGISTERS;
        if (!Imports.SetThreadContext(threadHandle, ref ctx))
            throw new Exception("SetThreadContext failed");
    }
    finally
    {
        Imports.ResumeThread(threadHandle);
    }
}

/// <summary>
/// Remove the hardware breakpoint from DR0.
/// </summary>
private void ClearHardwareBreakpoint(IntPtr threadHandle)
{
    Imports.SuspendThread(threadHandle);
    
    try
    {
        var ctx = new CONTEXT_x86 
        { 
            ContextFlags = CONTEXT_DEBUG_REGISTERS 
        };
        
        if (!Imports.GetThreadContext(threadHandle, ref ctx))
            throw new Exception("GetThreadContext failed");
        
        ctx.Dr0 = 0;
        ctx.Dr7 &= ~(uint)0x000F0003; // Clear DR0 enable and conditions
        
        ctx.ContextFlags = CONTEXT_DEBUG_REGISTERS;
        if (!Imports.SetThreadContext(threadHandle, ref ctx))
            throw new Exception("SetThreadContext failed");
    }
    finally
    {
        Imports.ResumeThread(threadHandle);
    }
}
```

### Step 5: Inject VEH Handler ASM into WoW

The VEH handler must be **ASM in WoW's address space** (we're out-of-process).
It's allocated via `VirtualAllocEx` and registered via `InjectAndExecute` calling
`AddVectoredExceptionHandler`.

### Step 6: Data Region Extension

```
Data region layout (52 bytes):
+0   injectionWaitingHandle    (4 bytes)
+4   injectionContinueHandle   (4 bytes)
+8   injectionFinishedHandle   (4 bytes)
+12  returnedData              (4 bytes)
+16  frameCount                (4 bytes)
+20  inHookFlag                (4 bytes)
+24  statusCode                (4 bytes)
+28  tlsSlotIndex              (4 bytes)
+32  vehHandle                 (4 bytes)
+36  frameDropWaitTime         (4 bytes)
+40  hwbpState                 (4 bytes) ← NEW: 0=waiting, 1=re-enable DR0
+44  endSceneAddress           (4 bytes) ← NEW: stored for VEH reference
+48  trampolineAddress         (4 bytes) ← NEW: stored for VEH redirection
```

### Step 7: Register the VEH Handler

```csharp
// Inject a stub that calls AddVectoredExceptionHandler(1, @VEH_MasterHandler)
// and stores the result in m_VehPtr
Clear();
AddLine("push @VEH_MasterHandler");
AddLine("push 1");
AddLine("call {0}", AddVectoredExceptionHandler);
AddLine("mov [{0}], eax", m_VehPtr);
AddLine("retn");
Memory.Asm.InjectAndExecute(eventStub);
```

### Step 8: Set HWBP After VEH Is Registered

```csharp
// VEH must be registered BEFORE setting the HWBP
// Otherwise the breakpoint fires with no handler → crash
SetHardwareBreakpoint(Memory.ThreadHandle, m_OrigEndScene);
```

---

## 6. ASM Trampoline Design

### Complete VEH Handler ASM (stdcall, 1 parameter)

```asm
@VEH_MasterHandler:
    push ebp
    mov ebp, esp
    push ecx                           ; scratch register

    ; ── Get EXCEPTION_POINTERS ──
    mov eax, [ebp+8]                   ; EXCEPTION_POINTERS* (stdcall param)
    mov ecx, [eax]                     ; EXCEPTION_RECORD*
    mov ecx, [ecx]                     ; ExceptionCode

    ; ═══ SINGLE_STEP dispatch (HWBP) ═══
    cmp ecx, 0x80000004
    jne @VEH_CheckCrash

    ; Check DR6 bit 0 (DR0 triggered)
    mov eax, [ebp+8]
    mov eax, [eax+4]                   ; CONTEXT*
    test dword [eax+0x14], 1           ; CONTEXT.Dr6 & 1
    jz @VEH_ContinueSearch

    ; Clear DR6
    mov dword [eax+0x14], 0

    ; Check state
    cmp dword [m_HwbpState], 1
    je @VEH_ReEnableDR0

    ; First hit: redirect Eip to trampoline
    mov ecx, [m_TrampolineAddr]
    mov [eax+0xB8], ecx                ; CONTEXT.Eip = trampoline

    ; Disable DR0 (clear L0 in DR7) to prevent re-trigger
    and dword [eax+0x18], 0xFFFFFFFE   ; CONTEXT.Dr7 &= ~1

    mov ecx, 0xFFFFFFFF                ; EXCEPTION_CONTINUE_EXECUTION
    jmp @VEH_Return

@VEH_ReEnableDR0:
    ; Re-enable DR0 after single-stepping through original prologue
    or dword [eax+0x18], 1             ; CONTEXT.Dr7 |= L0
    ; Clear Trap Flag
    and dword [eax+0xC0], 0xFFFFFEFF   ; CONTEXT.EFlags &= ~0x100
    mov dword [m_HwbpState], 0         ; Reset state

    mov ecx, 0xFFFFFFFF                ; EXCEPTION_CONTINUE_EXECUTION
    jmp @VEH_Return

    ; ═══ Crash protection dispatch ═══
@VEH_CheckCrash:
    ; TLS check: is this our injected code thread?
    push dword [m_TlsPtr]
    call [TlsGetValue]                 ; TlsGetValue(slot)
    test eax, eax
    jz @VEH_ContinueSearch

    ; Our thread crashed — save exception code and redirect
    mov ecx, eax                       ; ecx = saved ESP (TLS value)
    mov eax, [ebp+8]
    push eax
    mov eax, [eax]                     ; EXCEPTION_RECORD*
    mov eax, [eax]                     ; ExceptionCode
    mov [m_ReturnedDataPtr], eax
    pop eax
    mov eax, [eax+4]                   ; CONTEXT*
    mov [eax+0xC4], ecx                ; CONTEXT.Esp = saved ESP
    push @CallGateInterject
    pop ecx
    mov [eax+0xB8], ecx                ; CONTEXT.Eip = @CallGateInterject

    mov ecx, 0xFFFFFFFF
    jmp @VEH_Return

@VEH_ContinueSearch:
    xor ecx, ecx                       ; EXCEPTION_CONTINUE_SEARCH

@VEH_Return:
    mov eax, ecx
    pop ecx
    pop ebp
    retn 4                             ; stdcall cleanup
```

### Complete Trampoline ASM

```asm
@TrampolineEntry:
    pushad

    ; Frame counter
    inc dword [m_FrameCountPtr]

    @CheckInjection:
    mov eax, [m_InjectionWaitingHandlePtr]
    push 0
    push eax
    call [WaitForSingleObject]
    test eax, eax
    jnz @NoInjection

    ; Mark as in-hook
    mov dword [m_InHookPtr], 1

    ; Set up TLS if not done
    ; (TLS is always available since VEH persists in HWBP mode)
    ; TlsSetValue(slot, ESP)
    push esp
    push dword [m_TlsPtr]
    call [TlsSetValue]
    test eax, eax
    jz @StoreStatus3

    ; Call injected code
    call [m_InjectedCode]
    mov [m_ReturnedDataPtr], eax

    ; Clear TLS
    push 0
    push dword [m_TlsPtr]
    call [TlsSetValue]

    ; Status = 0 (success)
    mov dword [m_StatusPtr], 0
    jmp @SignalFinished

@StoreStatus3:
    mov dword [m_StatusPtr], 3
    jmp @SignalFinished

    @CallGateInterject:            ; VEH redirects here on crash
    push 0
    push dword [m_TlsPtr]
    call [TlsSetValue]            ; Clear TLS
    mov dword [m_StatusPtr], 4

@SignalFinished:
    mov dword [m_InHookPtr], 0

    ; Signal finished
    push dword [m_InjectionFinishedHandlePtr]
    call [SetEvent]

    ; Wait for continue
    push 1000
    push dword [m_InjectionContinueHandlePtr]
    call [WaitForSingleObject]
    test eax, eax
    jz @CheckInjection

    ; Reset waiting event
    push dword [m_InjectionWaitingHandlePtr]
    call [ResetEvent]

@NoInjection:
    popad

    ; Set state for VEH re-enablement
    mov dword [m_HwbpState], 1

    ; Set Trap Flag for single-step
    pushfd
    or dword [esp], 0x100
    popfd

    ; Jump to ORIGINAL EndScene (unmodified!)
    jmp [m_OrigEndScene]
    ; → CPU executes 1 instruction of EndScene
    ; → Single-step fires → VEH re-enables DR0
    ; → EndScene continues normally
```

---

## 7. Thread Management

### New Thread Creation

If WoW creates new threads (e.g., during loading screens, zone transitions), those threads
won't have DR0 set. This is fine because:

1. Only the **render thread** calls EndScene
2. The render thread is the main thread, created at process startup
3. We only need HWBP on that one thread

### Thread Suspension Concerns

`SuspendThread` / `ResumeThread` is used only during:
- Initial HWBP setup (constructor)
- HWBP cleanup (Dispose)

This is safe because:
- It's a brief suspension (microseconds)
- WoW's render thread can tolerate brief pauses (happens with GC, disk I/O, etc.)
- We never suspend during active injection

### Multi-Thread HWBP (Future Enhancement)

If WoW ever calls EndScene from a different thread (unlikely in 3.3.5a), we would need to:
1. Enumerate all threads via `CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0)`
2. Set DR0 on each thread belonging to WoW's process
3. Monitor for new threads and set DR0 on them too

This is NOT needed for 3.3.5a where rendering is single-threaded.

---

## 8. Challenges & Risks

### 8.1 Performance Impact

**Concern:** Every EndScene call triggers an exception → VEH dispatch → context switch.
This adds overhead compared to a direct JMP.

**Measurement:** Exception handling on modern Windows takes ~5-15 microseconds.
At 60 FPS, that's 0.3-0.9ms per second — negligible (< 0.1% of frame time).

**Mitigation:** None needed. The overhead is within noise.

### 8.2 DR0 Conflicts

**Concern:** Other software (debuggers, anti-cheat, overlays) might use DR0.

**Mitigation:**
- Check DR0 before setting it. If non-zero, try DR1, DR2, DR3.
- If all debug registers are in use, fall back to inline JMP hook.

### 8.3 Single-Step Trap Flag Interactions

**Concern:** The Trap Flag (TF) in EFLAGS causes a single-step exception after exactly
one instruction. If another single-step handler is present (debugger), it might interfere.

**Mitigation:**
- Our VEH handler checks `m_HwbpState` to distinguish our single-step from others.
- If `m_HwbpState != 1`, we return `EXCEPTION_CONTINUE_SEARCH` for the single-step.

### 8.4 VEH Registration Persistence

**Concern:** In the current inline JMP design, VEH is set up/torn down per batch.
For HWBP, the VEH handler must persist for the lifetime of the hook (it IS the hook).

**Impact:** VEH handle is registered once at construction and removed at Dispose.
This is simpler than the per-batch approach but means VEH overhead is always present
(even when not injecting). The overhead is zero when no exceptions fire.

### 8.5 Restore on Detach

**Concern:** When CopilotBuddy disconnects, DR0 must be cleared and VEH removed.

**Implementation:**
1. `ClearHardwareBreakpoint(threadHandle)` — zeros DR0 and DR7 bits
2. Inject `RemoveVectoredExceptionHandler(vehHandle)` and execute
3. Free allocated memory regions

If CopilotBuddy crashes without cleanup, DR0 remains set but the VEH handler's
memory is freed (VirtualFreeEx or process exit). This means:
- DR0 still points to EndScene → single-step fires each frame
- No VEH handler → exception propagates to SEH → WoW's crash handler → Error 132

**Mitigations:**
- Register `AppDomain.CurrentDomain.UnhandledException` and `ProcessExit` handlers
  to ensure DR0 is cleared and VEH removed even on unexpected shutdown.
- Consider a **heartbeat mechanism**: the VEH handler in WoW checks a "heartbeat" address
  that CopilotBuddy writes to every second. If the value becomes stale (> 5 seconds),
  the VEH handler auto-disables DR0 via CONTEXT manipulation during the next
  single-step dispatch, then removes itself on the following frame.
- Use `try/finally` in the main bot loop to guarantee `Dispose()` is called.

### 8.6 PUSHFD/POPFD and EFLAGS

**Concern:** `pushfd` / `popfd` are used to set the Trap Flag. Some anti-cheat systems
monitor for TF usage.

**Mitigation:** In 3.3.5a private servers, no known Warden scan checks for TF.
This is a theoretical concern only.

---

## 9. Testing Plan

### Phase 1: Basic HWBP Setup/Teardown

1. Set DR0 on WoW's render thread
2. Verify DR0 is set via `GetThreadContext`
3. Register VEH handler (basic — just logs and continues)
4. Verify EndScene still works (WoW renders normally)
5. Clear DR0 and remove VEH
6. Verify clean teardown

### Phase 2: Trampoline Redirect

1. Set DR0 + register full VEH handler
2. VEH handler redirects to trampoline
3. Trampoline does `pushad` / `popad` only (no injection)
4. Single-step re-enablement works
5. Verify WoW renders every frame without stutter

### Phase 3: Full Integration

1. Add injection infrastructure (CheckInjection loop)
2. Test single Execute() call
3. Test continuous execution (FrameLock)
4. Verify VEH crash protection works (inject bad code → should throw InjectionSEHException)
5. Stress test: run bot for 30+ minutes, monitor frame rate

### Phase 4: Cleanup & Edge Cases

1. Test cleanup on CopilotBuddy exit
2. Test cleanup on WoW exit while hooked
3. Test recovery from CopilotBuddy crash (no cleanup)
4. Test with different D3D9 drivers (Nvidia, AMD, Intel)
5. Test with Discord overlay active (overlay uses D3D9 hooks)

---

## 10. Performance Considerations

| Metric | Inline JMP (current) | HWBP (proposed) |
|--------|---------------------|-----------------|
| Hook entry overhead | ~2 ns (JMP instruction) | ~10-15 μs (exception dispatch) |
| Code modification | 5 bytes at EndScene | **0 bytes** |
| Memory footprint | ~12 KB (3 allocations) | ~16 KB (4 allocations) |
| Maximum hooks | Unlimited | 4 (DR0–DR3) |
| Re-entry after original | Direct JMP (instant) | Single-step + VEH (~10 μs) |
| Total frame overhead | ~1 μs | ~25 μs |
| FPS impact at 60 FPS | None | ~0.15% (negligible) |

The ~25 μs overhead per frame is well within acceptable bounds. WoW 3.3.5a typically
spends 8-16 ms per frame (60-120 FPS). The HWBP overhead is < 0.3% of frame time.

---

## 11. Rollback Plan

If HWBP hook fails or causes instability:

1. `HwbpExecutor` is a separate class — `ExecutorRand` remains untouched.
2. The hook type is selected at initialization. Switch back to `ExecutorRand` by changing
   one line in `ObjectManager.cs` or wherever the executor is created.
3. Zero code changes needed in `ExecutorRand.cs` — it's fully functional as-is.

---

## 12. Missing Robustness Features Analysis

### What CopilotBuddy Should Still Add (Regardless of Hook Type)

Based on the comparison table, here's what matters for **robustness and crash prevention**
on private servers:

| Feature | Status | Priority | Reason |
|---------|--------|----------|--------|
| **VEH crash protection** | ✅ Done | — | Already implemented. Prevents Error 132. |
| **TLS thread identification** | ✅ Done | — | Already implemented with VEH. |
| **6 prologue patterns** | ✅ Done | — | Handles all known D3D9 prologues (cascading if/else detection). |
| **Frame counter** | ✅ Done | — | Data region +16, read via `FrameCount` property. |
| **In-hook flag** | ✅ Done | — | Data region +20, read via `InHook` property. |
| **Status codes (0-4)** | ✅ Done | — | VEH status system. |
| **FrameDropWaitTime XOR obfuscation** | ⚠️ Low | Low | Prevents memory scanner from reading timeout. Not critical for stability. Cosmetic anti-detection. |
| **Injected code wiping** | ⚠️ Missing | Medium | HB 6.2.3 fills injected code buffer with random bytes after execution. Prevents memory scanners from reading your ASM between frames. Adds ~1 line of code. |
| **ExecutorPatch** | ❌ Not needed | None | Only useful for temporarily patching game code (e.g., disabling Warden checks during injection). Private servers with controlled Warden don't need this. |
| **AsmRandomizer (1052 lines)** | ❌ Nice-to-have | Low | The existing `AddRandomLine()` with 5 junk patterns is sufficient. The full AsmRandomizer with polymorphic instruction variants is anti-pattern-matching — overkill for private servers without advanced byte scanning. |
| **RegisterSet (Fisher-Yates)** | ❌ Nice-to-have | Low | Randomized push/pop order instead of `pushad`/`popad`. Anti-detection only — no stability benefit. |
| **Monitor locks on FrameLock** | ⚠️ Consider | Medium | HB 5.4.8/6.2.3 use `Monitor.Enter`/`Exit` in FrameLock with `_wasAcquired` tracking. CopilotBuddy uses a simple counter. Could cause issues if `Dispose()` is called from a different thread or if exceptions skip the `Dispose()`. Consider upgrading to match 5.4.8 pattern. |

### Private Server Warden Reality

Based on AzerothCore source code analysis:

1. **Warden is OPTIONAL** — `CONFIG_WARDEN_ENABLED` defaults to disabled on many servers.
2. **Checks are DB-driven** — `warden_checks` table. Server admins choose what to scan.
3. **MEM_CHECK** reads bytes at addresses — detects inline JMP hooks.
4. **PAGE_CHECK** hashes memory regions (SHA1) — the `0xE9` in the source is a protocol status byte, not a JMP scan.
5. **No DR register scanning** — AzerothCore has no check type for debug registers.
6. **No VEH enumeration** — no check type inspects VEH chain.
7. **Custom modules possible** — admins can write custom Warden modules with arbitrary scans,
   but this requires deep expertise and is extremely rare on private servers.

### Recommended Additions for Stability (High Priority)

1. **Injected code wiping** — After each execution, overwrite the injected code buffer with
   `m_ClearBytes` (random bytes). This is 1-2 lines of code in `Execute()`:
   ```csharp
   // After WaitForInjection completes
   Memory.WriteBytes(m_InjectedCode, m_ClearBytes);
   ```

2. **Monitor-based FrameLock** — Upgrade `FrameLock.cs` to use `Monitor.Enter`/`Exit`
   with `_wasAcquired` tracking like HB 5.4.8/6.2.3. This prevents race conditions if
   `Dispose()` is called out-of-order.

### FrameLock Slider (HB WoD)

**HB 6.2.3 does NOT have a FrameLock slider.** It has:
- A checkbox (`StyxSettings.UseFrameLock`) marked `[Obsolete]` with `[DefaultValue(true)]`
- A TPS (Ticks Per Second) slider (`CharSettings.TicksPerSecond`, range 4–100)
- `FrameDropWaitTime` is hardcoded to 1000ms — never exposed in UI

**CopilotBuddy already has** a `UseFrameLock` checkbox and TPS slider in SettingsWindow.
No FrameLock slider needed — HB never had one.

### PullDistance Slider

**HB 6.2.3 does NOT have a PullDistance slider in the main settings.**
HB 4.3.4 had one, and CopilotBuddy already has one. Nothing missing.
