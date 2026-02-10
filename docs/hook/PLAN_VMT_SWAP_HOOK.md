# PLAN: VMT Swap EndScene Hook

> **Objective:** Replace the inline 5-byte JMP detour with a VTable (VMT) pointer swap
> that replaces the EndScene entry in IDirect3DDevice9's virtual method table with a
> pointer to our trampoline. **Zero code bytes are modified** — only a single pointer
> in the VTable is changed.
>
> **Stealth level:** High — invisible to Warden MEM_CHECK, PAGE_CHECK, and code byte scanning.
> Detectable only via VTable pointer validation (comparing the VTable pointer or EndScene
> entry against known good values).
>
> **Complexity:** Medium — simpler than HWBP, but requires understanding D3D9 COM object
> layout and handling the VTable pointer carefully.
>
> **Prerequisite:** VEH crash protection (already implemented in ExecutorRand.cs).
>
> **Reference implementations:**
> - EndOfEntropy/D3D9-VMT — basic D3D9 VMT swap in C++
> - Shtahet/AthenaInjected (`D3D9.cs`) — C# with SlimDX for VTable discovery
> - Fernir/Dinnerbot (`Endscene.cpp`) — external VMT swap for WoW
>
> **Reference docs:**
> - [RESEARCH_ENDSCENE_HOOK_ANALYSIS.md](RESEARCH_ENDSCENE_HOOK_ANALYSIS.md) §2.2
> - [PLAN_ERROR132_VEH_FIX.md](PLAN_ERROR132_VEH_FIX.md)

---

## Table of Contents

1. [How VMT Swap Hooks Work](#1-how-vmt-swap-hooks-work)
2. [Architecture Comparison: VMT Swap vs Current JMP](#2-architecture-comparison-vmt-swap-vs-current-jmp)
3. [Detection Surface Analysis](#3-detection-surface-analysis)
4. [D3D9 VTable Layout for WoW 3.3.5a](#4-d3d9-vtable-layout-for-wow-335a)
5. [Implementation Approach for CopilotBuddy](#5-implementation-approach-for-copilotbuddy)
6. [Detailed Implementation Steps](#6-detailed-implementation-steps)
7. [Trampoline Design](#7-trampoline-design)
8. [Compatibility with Overlays](#8-compatibility-with-overlays)
9. [Challenges & Risks](#9-challenges--risks)
10. [Testing Plan](#10-testing-plan)
11. [Performance Considerations](#11-performance-considerations)
12. [Rollback Plan](#12-rollback-plan)
13. [Private Server Warden Analysis](#13-private-server-warden-analysis)
14. [Missing Robustness Features Analysis](#14-missing-robustness-features-analysis)

---

## 1. How VMT Swap Hooks Work

### COM Object Layout (IDirect3DDevice9)

Direct3D 9 uses COM (Component Object Model) interfaces. Every COM object has the same
memory layout:

```
IDirect3DDevice9* pDevice:
  +0x00: VTable pointer → points to array of function pointers
  +0x04: Reference count
  +0x08: Internal data...

VTable (in d3d9.dll .rdata section):
  [0]  QueryInterface
  [1]  AddRef
  [2]  Release
  [3]  TestCooperativeLevel
  ...
  [42] EndScene          ← THIS IS WHAT WE SWAP
  [43] Present
  ...
  [118] Last method
```

### Hook Mechanism

**Method A: In-Place VTable Entry Swap (simpler)**

1. Locate the VTable pointer: `pDevice` → `[pDevice]` = VTable address
2. Make the EndScene entry writable: `VirtualProtect(&vtable[42], 4, PAGE_EXECUTE_READWRITE)`
3. Save original: `originalEndScene = vtable[42]`
4. Write our hook: `vtable[42] = (DWORD)&ourTrampoline`
5. Restore protection: `VirtualProtect(&vtable[42], 4, oldProtect)`

**Method B: Full VTable Copy (stealthier)**

1. Locate the VTable pointer: `pDevice` → `[pDevice]` = VTable address
2. Allocate a new VTable: `newVTable = VirtualAllocEx(119 * 4 bytes)`
3. Copy entire VTable: `memcpy(newVTable, originalVTable, 119 * 4)`
4. Modify EndScene in copy: `newVTable[42] = &ourTrampoline`
5. Swap VTable pointer in device: `[pDevice] = newVTable`

**Method A** modifies the VTable in the d3d9.dll `.rdata` section — this can be detected
by checking if the pointer at VTable[42] points into d3d9.dll's address range.

**Method B** creates a copy on the heap. The device's VTable pointer itself changes from
pointing into d3d9.dll's `.rdata` to pointing at heap memory — detectable by comparing
the VTable pointer against the known good d3d9.dll range.

### Which Method for CopilotBuddy?

**Method A (in-place swap)** is recommended because:
- Simpler to implement out-of-process
- Less memory to manage (no full VTable copy)
- The VTable pointer in the device object remains unchanged (points to d3d9.dll)
- Only the EndScene entry (4 bytes in `.rdata`) is modified
- Detection requires scanning VTable entries, not just the VTable pointer

**The tradeoff:** 4 bytes in d3d9.dll's `.rdata` section are modified (the EndScene pointer),
but NO code bytes (`.text` section) are modified. Warden's PAGE_CHECK scans code pages
(`.text`), not data pages (`.rdata`).

---

## 2. Architecture Comparison: VMT Swap vs Current JMP

### Current Flow (Inline JMP — ExecutorRand.cs)

```
WoW calls EndScene via vtable[42]
  → vtable[42] = original EndScene address
  → CPU enters EndScene function
  → First 5 bytes are JMP m_EndSceneDetour         (MODIFIED code bytes)
  → ...trampoline...
  → replay stolen prologue
  → JMP EndScene+5
```

### Proposed Flow (VMT Swap — Method A)

```
WoW calls EndScene via vtable[42]
  → vtable[42] = our trampoline address             (MODIFIED pointer, NOT code)
  → CPU enters our trampoline directly
  → ...trampoline (full EndScene replacement)...
  → call original EndScene                           (no stolen prologue needed!)
  → EndScene runs COMPLETELY normally
  → return to WoW
```

### Key Advantage: No Prologue Stealing

With VMT swap, the original EndScene function is **completely unmodified**. Our trampoline
calls it directly via the saved original pointer. There's no need to:
- Read/detect prologue patterns
- Replay stolen bytes
- Handle different prologue variants

**This eliminates an entire class of bugs** related to prologue detection.

### Comparison Table

| Aspect | Inline JMP | VMT Swap (Method A) |
|--------|-----------|---------------------|
| Code bytes modified | 5 bytes at EndScene | **0 bytes** |
| Data bytes modified | 0 | 4 bytes in VTable |
| Original function intact? | No (prologue overwritten) | **Yes (completely intact)** |
| Prologue detection needed? | Yes (7+ patterns) | **No** |
| Prologue replay needed? | Yes | **No** |
| Detection by code scan | Trivial (0xE9 at function) | **Impossible** |
| Detection by data scan | N/A | Possible (VTable entry check) |
| Existing hook chaining | Complex (CopiedBytesTransformer) | **Automatic** (call original) |
| Hook installation | WriteProcessMemory on .text | WriteProcessMemory on .rdata |
| Compatibility with overlays | Conflicts (both write at same address) | May conflict (both swap same VTable entry) |

---

## 3. Detection Surface Analysis

### Private Server Warden (AzerothCore/TrinityCore)

| Warden Scan Type | Can Detect VMT Swap? | Details |
|-----------------|----------------------|---------|
| **MEM_CHECK** (0xF3) | **Only if scanning VTable** | Reads N bytes at any address. If a check targets the VTable entry address, it would detect the changed pointer. But standard warden_checks **do not include D3D9 VTable addresses** — they target WoW's own code/data. |
| **PAGE_CHECK_A/B** (0xB2/0xBF) | **No** | Hashes memory regions (SHA1) and compares to expected values. The `0xE9` in AzerothCore source is a protocol status byte (pass/fail), not a JMP opcode search. VMT swap doesn't modify scanned regions → undetectable. |
| **MODULE_CHECK** (0xD9) | **No** | Checks for loaded DLLs. No DLL used. |
| **DRIVER_CHECK** (0x71) | **No** | Checks for loaded drivers. No driver used. |
| **LUA_EVAL_CHECK** (0x8B) | **No** | Evaluates Lua expressions. Unrelated. |
| **TIMING_CHECK** (0x57) | **No** | Checks `GetTickCount` detouring. VMT swap doesn't detour any WinAPI. |

### How a Custom Warden Check Could Detect VMT Swap

A server admin could add a MEM_CHECK that reads 4 bytes at the VTable EndScene entry address.
But this requires knowing:
1. The exact address of the device's VTable in memory (varies per system)
2. The expected value (original EndScene function address, varies per d3d9.dll version)

This is **extremely unlikely** on private servers because:
- D3D9 device address is not static (it's heap-allocated)
- The VTable address depends on the d3d9.dll version and load address (ASLR)
- Adding such a check would break for users with different GPU drivers

### Detection via D3D9 Module Range Check

A more sophisticated approach: check if `vtable[42]` points within d3d9.dll's address range.

```
Original:  vtable[42] = 0x6DEF1234  (inside d3d9.dll: 0x6DE00000–0x6DF00000)
After VMT: vtable[42] = 0x03A50000  (VirtualAllocEx'd memory, outside d3d9.dll)
```

This is theoretically possible but:
- Requires knowing d3d9.dll's base address and size
- No standard Warden check does this
- Would require a custom Warden module

---

## 4. D3D9 VTable Layout for WoW 3.3.5a

### Device Pointer Chain (Verified in IDA + Runtime)

```
[0x00C5DF88] → D3D structure base
    + 0x397C  → IDirect3DDevice9* pDevice
        [pDevice + 0x00] → VTable pointer
            + 0xA8 (42 × 4) → EndScene function pointer
```

### IDirect3DDevice9 VTable (119 entries, index 0–118)

Key entries surrounding EndScene:

| Index | Offset | Method | Notes |
|-------|--------|--------|-------|
| 0 | 0x00 | QueryInterface | IUnknown |
| 1 | 0x04 | AddRef | IUnknown |
| 2 | 0x08 | Release | IUnknown |
| ... | ... | ... | ... |
| 41 | 0xA4 | BeginScene | Called before rendering |
| **42** | **0xA8** | **EndScene** | **← Our hook target** |
| 43 | 0xAC | Present | Called after rendering |
| ... | ... | ... | ... |
| 118 | 0x1D8 | Last method | |

### Discovery Code (Already in CopilotBuddy)

The `ObjectManager.cs` already discovers the EndScene address using this pointer chain:

```csharp
// Read D3D structure base
uint d3dBase = Memory.Read<uint>(new uint[] { 0x00C5DF88 });
// Read device pointer
uint pDevice = Memory.Read<uint>(new uint[] { d3dBase + 0x397C });
// Read VTable pointer
uint vtablePtr = Memory.Read<uint>(new uint[] { pDevice });
// Read EndScene entry
uint endSceneAddr = Memory.Read<uint>(new uint[] { vtablePtr + 0xA8 });
```

For VMT swap, we need the **VTable entry address** (where the EndScene pointer is stored),
not just the EndScene function address:

```csharp
uint vtableEndSceneEntry = vtablePtr + 0xA8;  // Address of the 4-byte pointer
uint originalEndScene = Memory.Read<uint>(new uint[] { vtableEndSceneEntry });
```

---

## 5. Implementation Approach for CopilotBuddy

### Chosen Approach: In-Place VTable Entry Swap (Method A)

**Why Method A over Method B:**
1. CopilotBuddy is out-of-process — using `WriteProcessMemory` to modify 4 bytes at
   the VTable entry is simpler than allocating + copying an entire VTable.
2. The VTable pointer in the device object stays unchanged → fewer things to track.
3. Cleanup is trivial — write back the original 4-byte pointer.
4. No need to handle VTable lifetime or prevent WoW from freeing our copy.

### File Structure

```
GreenMagic/
  ExecutorRand.cs       ← existing (inline JMP + VEH crash protection)
  VmtExecutor.cs        ← NEW (VMT swap + VEH crash protection)
  Memory.cs             ← shared
  InjectionSEHException.cs  ← shared
```

### Trampoline Type: Bare Function (HRESULT __stdcall)

Unlike the inline JMP hook where our trampoline is spliced into the middle of EndScene,
the VMT swap makes our trampoline a **full replacement function** that the D3D9 runtime
calls directly via the VTable. It must:

1. Have the correct calling convention: `__stdcall` (COM convention)
2. Accept the correct parameter: `IDirect3DDevice9* this` (first hidden param in stdcall COM)
3. Call the original EndScene with the same parameter
4. Return `HRESULT` (S_OK = 0)

```asm
; Our hook function: called instead of EndScene
; Stack on entry: [ESP] = return address, [ESP+4] = pDevice (this pointer)
@HookEndScene:
    ; Save pDevice for later
    push dword [esp+4]              ; save pDevice

    ; === Our injection code ===
    pushad
    ; ... CheckInjection loop ...
    ; ... VEH-protected CallGate ...
    popad

    ; Call original EndScene
    ; pop saved pDevice and pass it
    ; Original EndScene is stdcall: push pDevice, call originalEndScene
    call [m_OriginalEndScene]       ; pDevice is still on stack
    
    ; Return whatever EndScene returned (HRESULT in EAX)
    retn 4                          ; stdcall: callee pops 1 param (pDevice)
```

**Wait — this needs careful analysis.** The VTable call flow in x86 COM is:

```
; WoW's code (calling EndScene):
mov eax, [pDevice]                  ; eax = VTable pointer
push pDevice                        ; this pointer
call dword [eax+0xA8]               ; call vtable[42] — our hook
; After return, stack is cleaned (stdcall)
```

So our hook receives: `[ESP] = return address, [ESP+4] = pDevice`

To call the original EndScene:
```asm
; pDevice is at [ESP+4] (our caller put it there)
push dword [esp+4]                  ; push pDevice for original EndScene
call [m_OriginalEndScene]           ; stdcall: callee cleans pDevice
; EAX = HRESULT from original EndScene
retn 4                              ; we clean our pDevice parameter
```

Actually this double-pushes pDevice. Let me think more carefully:

```asm
@HookEndScene:
    ; On entry: [ESP] = return addr, [ESP+4] = pDevice
    
    ; === Our code ===
    pushad                          ; save all registers
    ; ... injection code ...
    popad

    ; Now call original EndScene with the SAME pDevice
    ; We need to pass [ESP+4] to it
    jmp [m_OriginalEndScene]        ; tail call — same stack frame!
```

**A `jmp` tail call is the cleanest approach.** It passes the same stack frame (including
pDevice and return address) to the original EndScene, which handles the `retn 4` cleanup.

---

## 6. Detailed Implementation Steps

### Step 1: Store VTable Entry Address

In the constructor, compute and save the address where the EndScene pointer lives:

```csharp
// Discovery (reusing existing code from ObjectManager)
uint d3dBase = Memory.Read<uint>(new uint[] { 0x00C5DF88 });
uint pDevice = Memory.Read<uint>(new uint[] { d3dBase + 0x397C });
uint vtablePtr = Memory.Read<uint>(new uint[] { pDevice });

m_VtableEndSceneEntry = vtablePtr + 0xA8;  // Address of the 4-byte pointer
m_OrigEndScene = Memory.Read<uint>(new uint[] { m_VtableEndSceneEntry });
```

### Step 2: Allocate Memory Regions

Same as current `ExecutorRand` but with one difference:
- No need to read EndScene prologue bytes (no prologue stealing)
- Still need: detour trampoline, injected code buffer, data region

```csharp
m_EndSceneDetour = Memory.AllocateMemory(4096, 0x1000, 0x40);  // PAGE_EXECUTE_READWRITE
m_InjectedCode = Memory.AllocateMemory(4096, 0x1000, 0x240);   // PAGE_EXECUTE_READWRITE | PAGE_NOCACHE
m_DataPtr = Memory.AllocateMemory(4096, 0x1000, 0x04);          // PAGE_READWRITE
```

Note: The trampoline allocation uses `PAGE_EXECUTE_READWRITE` (0x40) instead of
`PAGE_EXECUTE_READ` (0x20) because we may need to write the original EndScene address
into the trampoline memory at runtime.

### Step 3: Build Trampoline ASM

```csharp
private void InjectDetour()
{
    Clear();

    // ── VEH infrastructure (same as current ExecutorRand) ──
    EmitVehBlock();  // @ExceptionHandler, @SetUpGate, @TearDownGate, @CallGate, @CallGateInterject

    // ── Hook function entry ──
    AddLine("@HookEndScene:");

    // Save all registers (including pDevice on stack)
    AddRandomLine("pushad");

    // Frame counter
    AddRandomLine("inc dword [{0}]", m_FrameCountPtr);

    // ── Injection loop ──
    AddRandomLine("@CheckInjection:");
    AddRandomLine("mov eax, [{0}]", m_InjectionWaitingHandlePtr);
    AddRandomLine("push 0");
    AddRandomLine("push eax");
    AddRandomLine("call {0}", WaitForSingleObject);
    AddRandomLine("test eax, eax");
    AddRandomLine("jnz @NoInjection");

    // Mark as in-hook
    AddRandomLine("mov dword [{0}], 1", m_InHookPtr);

    // Set up VEH if not already done
    AddRandomLine("mov eax, [{0}]", m_VehPtr);
    AddRandomLine("test eax, eax");
    AddRandomLine("jnz @DoCall");
    AddRandomLine("call @SetUpGate");
    AddRandomLine("test eax, eax");
    AddRandomLine("jnz @StoreEaxAsStatus");

    AddLine("@DoCall:");
    AddRandomLine("call @CallGate");

    AddLine("@StoreEaxAsStatus:");
    AddRandomLine("mov [{0}], eax", m_StatusPtr);

    // Clear in-hook flag
    AddRandomLine("mov dword [{0}], 0", m_InHookPtr);

    // Signal finished
    AddRandomLine("push dword [{0}]", m_InjectionFinishedHandlePtr);
    AddRandomLine("call {0}", SetEvent);

    // Wait for continue
    AddRandomLine("push 1000");
    AddRandomLine("push dword [{0}]", m_InjectionContinueHandlePtr);
    AddRandomLine("call {0}", WaitForSingleObject);
    AddRandomLine("test eax, eax");
    AddRandomLine("jz @CheckInjection");

    // Reset waiting event
    AddRandomLine("push dword [{0}]", m_InjectionWaitingHandlePtr);
    AddRandomLine("call {0}", ResetEvent);

    AddLine("@NoInjection:");

    // Tear down VEH
    AddRandomLine("mov eax, [{0}]", m_VehPtr);
    AddRandomLine("test eax, eax");
    AddRandomLine("jz @SkipTearDown");
    AddRandomLine("call @TearDownGate");
    AddLine("@SkipTearDown:");

    // Restore all registers
    AddRandomLine("popad");

    // ══════════════════════════════════════════
    // Tail-call to original EndScene
    // Stack state: [ESP] = return address, [ESP+4] = pDevice
    // Same state as when we entered — just JMP to original
    // ══════════════════════════════════════════
    AddLine("jmp {0}", m_OrigEndScene);

    // Inject trampoline into allocated memory
    if (!Memory.Asm.Inject(m_EndSceneDetour))
        throw new Exception("Failed to inject VMT trampoline");
}
```

### Step 4: Swap the VTable Entry

```csharp
private void InstallHook()
{
    // Save original EndScene pointer (already done in constructor)
    // m_OrigEndScene is the original function address

    // Save the original 4 bytes at the VTable entry for restoration
    m_OriginalVtableBytes = Memory.ReadBytes(m_VtableEndSceneEntry, 4);

    // VTable is in d3d9.dll .rdata (PAGE_READONLY) — must change protection
    uint oldProtect;
    Imports.VirtualProtectEx(Memory.ProcessHandle,
        (IntPtr)m_VtableEndSceneEntry, 4, 0x40, out oldProtect);

    // Write our trampoline address into the VTable
    // NOTE: m_EndSceneDetour must be the address of @HookEndScene within the buffer,
    // not necessarily the buffer base (VEH code may be emitted before it).
    Memory.Write<uint>(m_VtableEndSceneEntry, m_EndSceneDetour);

    // Restore original page protection
    Imports.VirtualProtectEx(Memory.ProcessHandle,
        (IntPtr)m_VtableEndSceneEntry, 4, oldProtect, out _);

    // NO prologue bytes overwritten
    // NO JMP written at EndScene
    // Only the 4-byte pointer in the VTable is changed
}
```

### Step 5: Cleanup / Unhook

```csharp
public void Dispose()
{
    if (m_Disposed) return;
    m_Disposed = true;

    try
    {
        // Signal one more frame for VEH teardown
        Memory.Write<byte>(m_InjectedCode, 0xC3); // retn
        m_InjectionWaitingEvent.Set();
        m_InjectionFinishedEvent.WaitOne(5000, false);
        m_InjectionWaitingEvent.Reset();
        m_InjectionContinueEvent.Set();

        // Suspend render thread briefly to prevent race condition
        // (WoW might be about to call through our trampoline while we free it)
        Imports.SuspendThread(Memory.ThreadHandle);
        try
        {
            // Restore original VTable entry
            if (m_OriginalVtableBytes != null)
            {
                uint oldProtect;
                Imports.VirtualProtectEx(Memory.ProcessHandle,
                    (IntPtr)m_VtableEndSceneEntry, 4, 0x40, out oldProtect);
                Memory.WriteBytes(m_VtableEndSceneEntry, m_OriginalVtableBytes);
                Imports.VirtualProtectEx(Memory.ProcessHandle,
                    (IntPtr)m_VtableEndSceneEntry, 4, oldProtect, out _);
            }
        }
        finally
        {
            Imports.ResumeThread(Memory.ThreadHandle);
        }

        // NO need to restore EndScene prologue bytes (they were never modified!)

        // Free memory
        Memory.FreeMemory(m_EndSceneDetour);
        Memory.FreeMemory(m_InjectedCode);
        Memory.FreeMemory(m_DataPtr);
    }
    catch { }

    m_InjectionWaitingEvent?.Close();
    m_InjectionContinueEvent?.Close();
    m_InjectionFinishedEvent?.Close();
}
```

### Step 6: Handle Device Reset

**Critical concern:** When WoW's D3D9 device is lost (Alt+Tab in fullscreen, resolution change,
minimize), Direct3D may **recreate the device**. This means:
- The old device pointer becomes invalid
- A new device is created with a new VTable
- Our hook is lost

**Detection:** Monitor `IDirect3DDevice9::TestCooperativeLevel()` return value:
- `D3D_OK` (0) = device is fine
- `D3DERR_DEVICELOST` (0x88760868) = device lost, wait
- `D3DERR_DEVICENOTRESET` (0x88760869) = device ready to be reset

**Solution for WoW 3.3.5a:**
WoW 3.3.5a uses **windowed mode** on most private servers (or borderless windowed).
In windowed mode, device loss is much less common. However, we should handle it:

```csharp
// In Execute() or a periodic check:
uint currentEndScenePtr = Memory.Read<uint>(new uint[] { m_VtableEndSceneEntry });
if (currentEndScenePtr != m_EndSceneDetour)
{
    // VTable entry was reset (device recreated or another hook took over)
    // Re-read the original EndScene address
    m_OrigEndScene = currentEndScenePtr;
    // Re-install our hook
    Memory.Write<uint>(m_VtableEndSceneEntry, m_EndSceneDetour);
    // Update the JMP target in our trampoline (the original EndScene address may have changed)
    // ... update m_OrigEndScene in the trampoline ASM ...
}
```

**Alternative approach:** Store `m_OrigEndScene` in the data region (not hardcoded in ASM).
The trampoline reads it from the data region at runtime:

```asm
; Instead of: jmp 0x6DEF1234 (hardcoded)
; Use:        jmp dword [m_OrigEndScenePtr]  (data region read)
mov eax, [{m_OrigEndSceneDataPtr}]
jmp eax
```

This way, we can update the original EndScene address in the data region without
re-injecting the trampoline.

---

## 7. Trampoline Design

### Complete Trampoline ASM

```asm
; ═══════════════════════════════════════════════════════
; @ExceptionHandler — VEH crash protection (same as ExecutorRand)
; ═══════════════════════════════════════════════════════
@ExceptionHandler:
    push ebp
    mov ebp, esp
    push ecx

    ; TLS check for crash protection
    push dword [m_TlsPtr]
    call [TlsGetValue]
    test eax, eax
    jz @EHContinueSearch

    ; Our thread crashed — extract exception code
    mov ecx, eax                       ; ecx = saved ESP from TLS
    mov eax, [ebp+8]                   ; EXCEPTION_POINTERS*
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
    mov ecx, 0xFFFFFFFF                ; EXCEPTION_CONTINUE_EXECUTION
    jmp @EHReturn

@EHContinueSearch:
    xor ecx, ecx                       ; EXCEPTION_CONTINUE_SEARCH

@EHReturn:
    mov eax, ecx
    pop ecx
    pop ebp
    retn 4

; ═══════════════════════════════════════════════════════
; @SetUpGate, @TearDownGate, @CallGate, @CallGateInterject
; (Identical to ExecutorRand — omitted for brevity)
; ═══════════════════════════════════════════════════════
; ... same as current ExecutorRand.cs EmitVehBlock() ...

; ═══════════════════════════════════════════════════════
; @HookEndScene — The VMT swap landing point
; Called by D3D9 runtime as: vtable[42](pDevice)
; Stack: [ESP] = return address, [ESP+4] = pDevice
; ═══════════════════════════════════════════════════════
@HookEndScene:
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

    ; Mark in-hook
    mov dword [m_InHookPtr], 1

    ; VEH setup check
    mov eax, [m_VehPtr]
    test eax, eax
    jnz @DoCall
    call @SetUpGate
    test eax, eax
    jnz @StoreStatus

    @DoCall:
    call @CallGate

    @StoreStatus:
    mov [m_StatusPtr], eax
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

    ; Reset
    push dword [m_InjectionWaitingHandlePtr]
    call [ResetEvent]

    @NoInjection:
    ; VEH teardown
    mov eax, [m_VehPtr]
    test eax, eax
    jz @SkipTearDown
    call @TearDownGate
    @SkipTearDown:

    popad

    ; ═══ Tail-call to original EndScene ═══
    ; The stack is exactly as WoW's caller set it up:
    ; [ESP] = return address, [ESP+4] = pDevice
    ; A JMP preserves this stack frame for the original EndScene.
    jmp dword [m_OrigEndSceneDataPtr]
    ; Original EndScene does `retn 4` which returns to WoW's caller
```

### Why JMP Not CALL

Using `jmp` instead of `call` for the original EndScene is critical:
- `call` would push a return address, corrupting the stack frame
- `jmp` preserves the exact stack layout that WoW's caller expects
- The original EndScene's `retn 4` returns directly to WoW's calling code

### Data Region Access

The `m_OrigEndSceneDataPtr` is a data region field that stores the original EndScene address.
This allows runtime updates if the device is recreated:

```
Data region layout (48 bytes):
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
+40  origEndSceneAddress       (4 bytes) ← NEW: updated on device reset
+44  vtableEntryAddress        (4 bytes) ← NEW: for re-hooking after reset
```

---

## 8. Compatibility with Overlays

### Discord, Steam, and Other Overlays

Many applications hook D3D9 EndScene for rendering overlays:
- **Discord** overlay
- **Steam** overlay  
- **MSI Afterburner** / RivaTuner
- **FRAPS**
- **Reshade / ENB**

These typically use either:
1. **Inline JMP** at EndScene (like our current approach) — conflicts with our JMP
2. **VMT swap** on the same VTable entry — conflicts with our VMT swap

### Conflict Scenarios

**Scenario 1: Overlay installs first (JMP at EndScene), then CopilotBuddy VMT swap**

```
Before CopilotBuddy:
  vtable[42] → EndScene (original)
  EndScene prologue: E9 xx xx xx xx (overlay's JMP)

After CopilotBuddy VMT swap:
  vtable[42] → our trampoline
  EndScene prologue: E9 xx xx xx xx (still overlay's JMP, but nobody calls EndScene directly anymore)
  Our trampoline → jmp original EndScene → overlay JMP → overlay code → original EndScene body
```

**Result: WORKS!** Our trampoline calls what it thinks is the "original" EndScene, which
is actually the overlay-hooked EndScene. The overlay runs, then the real EndScene runs.
The overlay's inline JMP still works because it's in the function body — we're just calling
the function via pointer, not via VTable.

**Scenario 2: CopilotBuddy VMT swap first, then overlay installs JMP**

```
Before overlay:
  vtable[42] → our trampoline
  EndScene prologue: original (unmodified)

After overlay JMP:
  vtable[42] → our trampoline (unchanged)
  EndScene prologue: E9 xx xx xx xx (overlay's JMP)
  Our trampoline → jmp modified EndScene → overlay JMP → overlay code → EndScene body
```

**Result: WORKS!** Same as above — we call EndScene via pointer, overlay's JMP runs
as part of EndScene's normal execution.

**Scenario 3: Both use VMT swap**

```
Before overlay:
  vtable[42] → our trampoline

After overlay VMT swap:
  vtable[42] → overlay trampoline (OVERWRITES our entry!)
  Our trampoline is orphaned — never called
```

**Result: BROKEN.** The last VMT swap wins. Our hook is silently removed.

**Mitigation:** Periodically check if `vtable[42]` still points to our trampoline.
If it doesn't, re-install our hook. This creates a "fight" between hooks — whoever
checks last wins. In practice, overlays are installed once and don't periodically re-check.

**Scenario 3 — Most likely real-world case: RTSS (RivaTuner Statistics Server / MSI Afterburner)**

RTSS hooks D3D9 EndScene via VMT swap — the exact same technique as our approach.
This is the most probable Scenario 3 conflict in practice. CopilotBuddy's `VerifyHook()`
will detect RTSS overwriting our entry and re-chain through RTSS's trampoline. Users may
see brief visual glitches during the initial "fight". Recommend disabling the RTSS overlay
when using CopilotBuddy to avoid this entirely.

### Recommended Handling

```csharp
// In Execute() — verify hook is still installed
private void VerifyHook()
{
    uint currentEntry = Memory.Read<uint>(new uint[] { m_VtableEndSceneEntry });
    if (currentEntry != m_EndSceneDetour)
    {
        // Someone overwrote our VTable entry  
        // Save their address as the new "original" (they might be an overlay)
        m_OrigEndScene = currentEntry;
        Memory.Write<uint>(m_OrigEndSceneDataPtr, currentEntry);

        // Re-install our hook (VTable is in .rdata — need VirtualProtectEx)
        uint oldProtect;
        Imports.VirtualProtectEx(Memory.ProcessHandle,
            (IntPtr)m_VtableEndSceneEntry, 4, 0x40, out oldProtect);
        Memory.Write<uint>(m_VtableEndSceneEntry, m_EndSceneDetour);
        Imports.VirtualProtectEx(Memory.ProcessHandle,
            (IntPtr)m_VtableEndSceneEntry, 4, oldProtect, out _);

        Debug.WriteLine($"[VmtExecutor] Hook was overwritten by 0x{currentEntry:X8}, re-hooked and chained");
    }
}
```

---

## 9. Challenges & Risks

### 9.1 Device Recreation

**Risk:** WoW recreates the D3D9 device on resolution change, Alt+Tab (fullscreen),
or graphics settings change. The new device has a new VTable — our hook is lost.

**Mitigation:** 
- Monitor the device pointer periodically via the pointer chain
- If device pointer changes, re-discover VTable and re-hook
- Store original EndScene in data region (not hardcoded) for runtime updates

### 9.2 VTable Entry Protection

**Risk:** The VTable lives in d3d9.dll's `.rdata` section, which is typically
`PAGE_READONLY`. We need to change protection to write our pointer.

**Mitigation:** Use `VirtualProtectEx` to temporarily make the entry writable.
This is a 4-byte region — no significant security impact.

```csharp
// Change protection
uint oldProtect;
Imports.VirtualProtectEx(Memory.ProcessHandle, (IntPtr)m_VtableEndSceneEntry, 4, 0x40, out oldProtect);
// Write our pointer
Memory.Write<uint>(m_VtableEndSceneEntry, m_EndSceneDetour);
// Restore protection
Imports.VirtualProtectEx(Memory.ProcessHandle, (IntPtr)m_VtableEndSceneEntry, 4, oldProtect, out _);
```

### 9.3 Multiple Devices

**Risk:** WoW might create multiple D3D9 devices (unlikely in 3.3.5a, but possible in
theory for auxiliary rendering).

**Mitigation:** Only hook the device found via the known pointer chain. If there are
multiple devices, only the one at `[0x00C5DF88] + 0x397C` is the main render device.

### 9.4 Thread Safety During Swap

**Risk:** If WoW's render thread reads the VTable entry while we're writing it,
it might get a partial write (torn read). On x86, 4-byte aligned writes are atomic,
so this is not a risk as long as the VTable entry is 4-byte aligned (it always is,
since each entry is a 4-byte pointer).

**Mitigation:** None needed — x86 guarantees atomic 4-byte aligned writes/reads.

### 9.5 VTable Pointer vs VTable Entry

**Important distinction:**

```
pDevice:
  +0: VTable POINTER ← this points to the VTable array (Method B would change this)

VTable array:
  [42]: EndScene ENTRY ← this is the function pointer (Method A changes this)
```

Method A (our approach) modifies the EndScene **entry** within the VTable.
Method B would modify the VTable **pointer** in the device object.
We're using Method A — simpler and less disruptive.

---

## 10. Testing Plan

### Phase 1: VTable Discovery

1. Read pDevice from pointer chain
2. Read VTable pointer from pDevice
3. Read EndScene entry from VTable
4. Verify EndScene address is within d3d9.dll range
5. Log all addresses for verification

### Phase 2: Basic VMT Swap

1. Allocate trampoline (bare `push pDevice; call originalEndScene; retn 4`)
2. Swap VTable entry → our trampoline
3. Verify WoW renders normally
4. Restore VTable entry
5. Verify clean unhook

### Phase 3: Full Injection Integration

1. Add injection infrastructure (CheckInjection loop, VEH crash protection)
2. Test single `Execute()` call
3. Test continuous execution (FrameLock)
4. Test VEH crash protection (inject bad code → should throw `InjectionSEHException`)
5. Test GrabFrame

### Phase 4: Device Reset Handling

1. Change WoW resolution → verify hook survives
2. Alt+Tab from fullscreen → verify hook survives
3. Minimize/restore → verify hook survives
4. Test hook re-installation after device recreation

### Phase 5: Overlay Compatibility

1. Test with Discord overlay active
2. Test with Steam overlay active
3. Test with both overlays active
4. Verify hook priority (CopilotBuddy installs after overlays)

### Phase 6: Stress Testing

1. Run bot for 30+ minutes, monitor FPS
2. Zone transitions (loading screens)
3. Login/logout cycle (device recreation)
4. Multiple attach/detach cycles

---

## 11. Performance Considerations

| Metric | Inline JMP (current) | VMT Swap |
|--------|---------------------|----------|
| Hook entry overhead | ~2 ns (JMP instruction) | ~2 ns (indirect call through VTable) |
| Code modification | 5 bytes at EndScene | **0 bytes** (4 bytes in VTable) |
| Memory footprint | ~12 KB | ~12 KB (same) |
| Prologue detection | ~1 μs (pattern match at init) | **Not needed** |
| Re-entry to original | JMP past stolen prologue | **Direct call** (no replay) |
| Total frame overhead | ~1 μs | ~1 μs |
| FPS impact | None | **None** |

VMT swap has **identical performance** to the inline JMP hook. The VTable call is how
D3D9 normally calls EndScene — our hook doesn't add any extra call indirection beyond
what's already there.

---

## 12. Rollback Plan

If VMT swap hook fails or causes instability:

1. `VmtExecutor` is a separate class — `ExecutorRand` remains untouched.
2. Switch back by changing executor creation in `ObjectManager.cs`.
3. Zero code changes needed in `ExecutorRand.cs`.
4. Both executors share the same injection infrastructure (`Execute()`, `FrameLock`, etc.).

---

## 13. Private Server Warden Analysis

### AzerothCore Warden Source Code Findings

From `src/server/game/Warden/WardenWin.cpp` line 653-675:

```cpp
case PAGE_CHECK_A:
case PAGE_CHECK_B:
case DRIVER_CHECK:
case MODULE_CHECK:
{
    uint8 const byte = 0xE9;
    if (CRYPTO_memcmp(buff.contents() + buff.rpos(), &byte, sizeof(uint8)) != 0)
    {
        // FAIL — response byte was NOT 0xE9 (check detected tampering)
        checkFailed = checkId;
    }
    // else: PASS — response byte IS 0xE9 (check passed, no tampering)
    // NOTE: 0xE9 here is a Warden protocol status byte meaning "passed",
    // NOT the x86 JMP opcode. PAGE_CHECK hashes memory regions via SHA1.
}
```

**Key insight:** The `0xE9` value in the AzerothCore source is a **protocol status byte**
(the Warden client module returns `0xE9` to mean "check passed"), not a JMP opcode search.
PAGE_CHECK actually hashes memory regions (SHA1) and compares against expected server-side
values. MEM_CHECK reads bytes at specific fixed addresses — this is what catches inline
JMP hooks when the bytes at the hooked address no longer match the expected prologue.
VMT swap doesn't modify any code bytes → **completely undetectable by both checks**.

### MEM_CHECK Could Theoretically Detect

```cpp
case MEM_CHECK:
{
    // Reads N bytes at specified address and compares
    // Could detect modified VTable entries IF the address is in warden_checks
}
```

But the `warden_checks` table would need to contain the VTable entry address, which
varies per system. No standard AzerothCore installation includes such checks.

### Warden Check Configuration

```sql
-- Standard warden_checks entries target:
-- WoW's own .text section addresses (static, known)
-- Example: FrameScript::Execute at 0x00819210
-- NOT D3D9 VTable addresses (heap-allocated, variable)
```

**Conclusion:** VMT swap is **safe against standard AzerothCore Warden** and any
private server using the standard check database.

### Could a Custom Warden Module Detect It?

A motivated server admin could write a custom check that:
1. Reads `[0x00C5DF88]` → `+0x397C` → `[result]` → `+0xA8` = EndScene VTable entry
2. Checks if it points within d3d9.dll's address range
3. If not → detection

This requires:
- Deep D3D9 knowledge
- Custom Warden module development
- Would break for users with certain GPU drivers that proxy d3d9.dll

**Probability:** Very low for private WoW 3.3.5a servers.

---

## 14. Missing Robustness Features Analysis

### What CopilotBuddy Should Still Add (Regardless of Hook Type)

| Feature | Status | Priority | Reason |
|---------|--------|----------|--------|
| **VEH crash protection** | ✅ Done | — | Already implemented. Prevents Error 132. |
| **TLS thread identification** | ✅ Done | — | Already implemented with VEH. |
| **6 prologue patterns** | ✅ Done (but not needed for VMT swap!) | — | VMT swap doesn't steal prologues. |
| **Frame counter** | ✅ Done | — | Data region +16. |
| **In-hook flag** | ✅ Done | — | Data region +20. |
| **Status codes (0-4)** | ✅ Done | — | VEH status system. |
| **Injected code wiping** | ⚠️ Missing | **Medium** | Overwrite injected code buffer with random bytes after execution. Prevents memory scanners from reading ASM between frames. Simple addition. |
| **FrameDropWaitTime XOR** | ⚠️ Low | Low | XOR-obfuscate the WaitForSingleObject timeout in data region. Cosmetic anti-detection. |
| **ExecutorPatch** | ❌ Not needed | None | No need to temporarily patch game code on private servers. |
| **AsmRandomizer** | ❌ Nice-to-have | Low | Existing `AddRandomLine()` is sufficient for private servers. |
| **RegisterSet shuffle** | ❌ Nice-to-have | Low | `pushad`/`popad` works fine. No stability benefit. |
| **Monitor FrameLock** | ⚠️ Consider | Medium | Upgrade to `Monitor.Enter`/`Exit` pattern from HB 5.4.8/6.2.3 for thread safety. |

### VMT Swap Advantages for Robustness

1. **No prologue bugs** — eliminates the entire prologue detection/replay system.
   This removes a common source of crashes when drivers update or when other software
   hooks EndScene before us.

2. **Clean overlay coexistence** — if an overlay uses inline JMP at EndScene, our VMT
   swap calls the overlay-hooked EndScene naturally. No conflict.

3. **Simpler cleanup** — restoring a 4-byte pointer is much more reliable than restoring
   5-7 prologue bytes, especially if another hook modified the prologue after us.

4. **No thread suspension** — unlike HWBP (which needs `SuspendThread` + `SetThreadContext`),
   VMT swap uses a simple `WriteProcessMemory` to swap a pointer. No risk of suspending
   the render thread at a bad time.

### Recommendation

For CopilotBuddy on **private WoW 3.3.5a servers**:

1. **VMT Swap is the best next step** — simpler than HWBP, excellent stealth, removes
   prologue-related bugs, good overlay compatibility.

2. **HWBP is the ultimate stealth option** — zero memory modifications of any kind, but
   more complex (VEH exception handling, DR register management, Trap Flag).

3. **Both can coexist as options** — `VmtExecutor.cs` and `HwbpExecutor.cs` alongside
   `ExecutorRand.cs`. The user chooses via settings which hook type to use.

### HB WoD FrameLock Slider — Verified

**HB 6.2.3 does NOT have a FrameLock slider.** It has:
- Checkbox: `StyxSettings.UseFrameLock` (marked `[Obsolete]`, defaults to `true`)
- TPS slider: `CharSettings.TicksPerSecond` (range 4-100)
- `FrameDropWaitTime` is hardcoded to 1000ms in `Executor.cs` — never exposed in UI

CopilotBuddy already has the equivalent settings. Nothing missing.

### HB WoD PullDistance Slider — Verified

**HB 6.2.3 does NOT have a PullDistance slider in the main SettingsWindow.**
Only BGBuddy has a numeric up-down for pull distance (bot-specific, not global).
HB 4.3.4 had a slider, and CopilotBuddy already has one. Nothing missing.
