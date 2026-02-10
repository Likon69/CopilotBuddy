using System;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using GreenMagic.Native;

namespace GreenMagic
{
    /// <summary>
    /// ExecutorRand - Executor with polymorphic ASM code generation.
    /// Adds random NOP/PUSH/POP/XOR instructions between real code to vary signatures.
    /// Same functionality as Executor but with anti-detection randomization.
    /// </summary>
    public sealed class ExecutorRand : IDisposable
    {
        public Memory Memory { get; private set; }
        private readonly Random m_Random;
        private readonly object thisLock = new object();
        public object AssemblyLock = new object();

        // EventWaitHandles for synchronization
        private EventWaitHandle m_InjectionWaitingEvent;
        private EventWaitHandle m_InjectionContinueEvent;
        public EventWaitHandle m_InjectionFinishedEvent;
        private string m_InjectionWaitingEventName;
        private string m_InjectionContinueEventName;
        private string m_InjectionFinishedEventName;

        // Memory addresses in WoW process
        private uint m_OrigEndScene;
        private uint m_EndSceneDetour;
        private uint m_InjectedCode;
        private uint m_DataPtr;
        private uint m_ReturnedDataPtr;
        private uint m_InjectionWaitingHandlePtr;   // data + 0
        private uint m_InjectionContinueHandlePtr;  // data + 4
        private uint m_InjectionFinishedHandlePtr;  // data + 8

        // VEH data region fields (data + 16..36)
        private uint m_FrameCountPtr;               // data + 16
        private uint m_InHookPtr;                   // data + 20
        private uint m_StatusPtr;                   // data + 24
        private uint m_TlsPtr;                      // data + 28 (init: 0xFFFFFFFF)
        private uint m_VehPtr;                      // data + 32 (init: 0)
        private uint m_FrameDropWaitTimePtr;        // data + 36

        // Kernel32 function addresses
        private UIntPtr WaitForSingleObject;
        private UIntPtr CreateEventA;
        private UIntPtr ResetEvent;
        private UIntPtr SetEvent;

        // Kernel32 function addresses for VEH
        private UIntPtr TlsAlloc;
        private UIntPtr TlsFree;
        private UIntPtr TlsGetValue;
        private UIntPtr TlsSetValue;
        private UIntPtr AddVectoredExceptionHandler;
        private UIntPtr RemoveVectoredExceptionHandler;

        // State
        private bool m_ContinuousExecution;
        private bool m_FirstExecution;
        private byte[] m_ClearBytes = new byte[4096];
        private byte[]? m_OriginalEndSceneBytes;
        private bool m_Disposed;

        public bool IsOpen => Memory != null && Memory.IsProcessOpen && Memory.IsThreadOpen;
        public bool IsInitialized { get; private set; }
        public uint DataPointer => m_DataPtr;
        public uint ReturnPointer => m_ReturnedDataPtr;
        public uint InjectCodePointer => m_InjectedCode;

        /// <summary>
        /// Number of EndScene frames that have been processed by the detour.
        /// Read from the data region in the target process.
        /// </summary>
        public uint FrameCount => Memory.Read<uint>(new uint[] { m_FrameCountPtr });

        /// <summary>
        /// Whether the detour is currently executing (inside the hook code).
        /// </summary>
        public bool InHook => Memory.Read<uint>(new uint[] { m_InHookPtr }) != 0;

        /// <summary>
        /// Last VEH status code (0=OK, 1=TlsAlloc fail, 2=AddVEH fail, 3=TlsSetValue fail, 4=exception).
        /// </summary>
        public int LastStatus => Memory.Read<int>(new uint[] { m_StatusPtr });

        public ExecutorRand(Memory memory, uint endSceneAddress)
        {
            if (memory == null || !memory.IsProcessOpen)
                throw new ArgumentNullException(nameof(memory), "Memory is null or process not open");

            if (endSceneAddress == 0)
                throw new ArgumentException("EndScene address is null");

            // Verify EndScene is valid
            uint[] testRead = new uint[] { endSceneAddress };
            if (memory.Read<uint>(testRead) == 0)
                throw new ArgumentException("EndScene address is invalid");

            Memory = memory;
            Memory.Asm.SetMemorySize(65536);
            m_OrigEndScene = endSceneAddress;

            // Initialize random with crypto seed
            byte[] seed = new byte[4];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetNonZeroBytes(seed);
            }
            m_Random = new Random(BitConverter.ToInt32(seed, 0));

            bool flag = false;
            string text = Environment.UserDomainName + "\\" + Environment.UserName;
            EventWaitHandleSecurity eventWaitHandleSecurity = new EventWaitHandleSecurity();
            EventWaitHandleAccessRule eventWaitHandleAccessRule = new EventWaitHandleAccessRule(text, EventWaitHandleRights.FullControl, AccessControlType.Allow);
            // Also add Everyone and SYSTEM for cross-process IPC
            eventWaitHandleSecurity.AddAccessRule(eventWaitHandleAccessRule);
            eventWaitHandleSecurity.AddAccessRule(new EventWaitHandleAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), EventWaitHandleRights.Synchronize | EventWaitHandleRights.Modify, AccessControlType.Allow));
            eventWaitHandleSecurity.AddAccessRule(new EventWaitHandleAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), EventWaitHandleRights.FullControl, AccessControlType.Allow));
            
            m_InjectionWaitingEventName = "Global\\" + GetRandomString(16);
            System.Diagnostics.Debug.WriteLine($"[ExecutorRand] Creating WaitingEvent: {m_InjectionWaitingEventName}");
            m_InjectionWaitingEvent = EventWaitHandleAcl.Create(false, EventResetMode.AutoReset, m_InjectionWaitingEventName, out flag, eventWaitHandleSecurity);
            if (!flag)
            {
                throw new Exception("You should never see this message, but the event was opened instead of created!  That's bad!");
            }
            m_InjectionContinueEventName = "Global\\" + GetRandomString(16);
            System.Diagnostics.Debug.WriteLine($"[ExecutorRand] Creating ContinueEvent: {m_InjectionContinueEventName}");
            m_InjectionContinueEvent = EventWaitHandleAcl.Create(false, EventResetMode.AutoReset, m_InjectionContinueEventName, out flag, eventWaitHandleSecurity);
            if (!flag)
            {
                throw new Exception("You should never see this message, but the event was opened instead of created!  That's bad!");
            }
            m_InjectionFinishedEventName = "Global\\" + GetRandomString(16);
            System.Diagnostics.Debug.WriteLine($"[ExecutorRand] Creating FinishedEvent: {m_InjectionFinishedEventName}");
            m_InjectionFinishedEvent = EventWaitHandleAcl.Create(false, EventResetMode.AutoReset, m_InjectionFinishedEventName, out flag, eventWaitHandleSecurity);
            if (!flag)
            {
                throw new Exception("You should never see this message, but the event was opened instead of created!  That's bad!");
            }

            InitializeDetour();
            Clear();
            IsInitialized = true;
        }

        public ExecutorRand(int processId, uint endSceneAddress)
            : this(new Memory(processId), endSceneAddress)
        {
        }

        #region Execution Control

        public void BeginExecute()
        {
            lock (thisLock)
            {
                if (m_ContinuousExecution)
                    EndExecute();

                m_ContinuousExecution = true;
                m_FirstExecution = true;
            }
        }

        public void EndExecute()
        {
            lock (thisLock)
            {
                m_ContinuousExecution = false;
                m_FirstExecution = false;
                m_InjectionWaitingEvent.Reset();
                m_InjectionContinueEvent.Set();
            }
        }

        public void Execute()
        {
            if (!IsOpen || !IsInitialized)
                throw new Exception("Cannot execute code while process is not opened and/or Executor is not initialized.");
            lock (thisLock)
            {
                Memory.Asm!.Inject(m_InjectedCode);
                if (m_ContinuousExecution)
                {
                    if (m_FirstExecution)
                    {
                        m_InjectionContinueEvent.Reset();
                        m_InjectionWaitingEvent.Set();
                        WaitForInjection(10000);
                        m_FirstExecution = false;
                    }
                    else
                    {
                        m_InjectionWaitingEvent.Set();
                        m_InjectionContinueEvent.Set();
                        WaitForInjection(10000);
                    }
                }
                else
                {
                    m_InjectionWaitingEvent.Set();
                    WaitForInjection(10000);
                    m_InjectionWaitingEvent.Reset();
                    m_InjectionContinueEvent.Set();
                }
            }
        }

        public void GrabFrame()
        {
            Memory.Write<byte>(m_InjectedCode, 195);
            m_InjectionWaitingEvent.Set();
            if (!m_InjectionFinishedEvent.WaitOne(10000, false))
                throw new Exception("Process must have frozen or gotten out of sync; InjectionFinishedEvent was never fired.");
            m_InjectionWaitingEvent.Reset();
            m_InjectionContinueEvent.Set();
        }

        /// <summary>
        /// Wait for the injection to complete and check the VEH status code.
        /// Throws appropriate exceptions based on the status.
        /// </summary>
        /// <param name="timeout">Maximum wait time in milliseconds.</param>
        private void WaitForInjection(int timeout = 10000)
        {
            if (!m_InjectionFinishedEvent.WaitOne(timeout, false))
            {
                throw new InjectionDesyncException(
                    "Process must have frozen or gotten out of sync; " +
                    "InjectionFinishedEvent was never fired within " + timeout + "ms.");
            }

            // Read the status code from the data region
            int status = Memory.Read<int>(new uint[] { m_StatusPtr });

            switch (status)
            {
                case 0:
                    // Success — injected code ran normally
                    break;

                case 1:
                    throw new InjectionException(1,
                        "TlsAlloc returned TLS_OUT_OF_INDEXES (0xFFFFFFFF). " +
                        "No TLS slots available in the target process.");

                case 2:
                    throw new InjectionException(2,
                        "AddVectoredExceptionHandler returned NULL. " +
                        "VEH registration failed.");

                case 3:
                    throw new InjectionException(3,
                        "TlsSetValue returned FALSE. " +
                        "Failed to mark current thread for VEH identification.");

                case 4:
                    // Read the exception code stored by the VEH handler
                    uint exceptionCode = Memory.Read<uint>(new uint[] { m_ReturnedDataPtr });
                    throw new InjectionSEHException(exceptionCode);

                default:
                    throw new InjectionException(status,
                        $"Injection resulted in unknown status code {status}.");
            }
        }

        #endregion

        #region ASM Building

        public void Clear() => Memory.Asm.Clear();
        public void AddLine(string line) => Memory.Asm.AddLine(line);
        public void AddLine(string format, params object[] args) => Memory.Asm.AddLine(format, args);

        /// <summary>
        /// Add line with random padding instructions before and after.
        /// </summary>
        public void AddRandomLine(string line)
        {
            AddRandomLine(line, Array.Empty<object>());
        }

        public void AddRandomLine(int numRandomLines, string line)
        {
            AddRandomLine(numRandomLines, line, Array.Empty<object>());
        }

        public void AddRandomLine(string format, params object[] args)
        {
            AddRandomLine(m_Random.Next(2, 5), format, args);
        }

        public void AddRandomLine(int numRandomLines, string format, params object[] args)
        {
            if (numRandomLines <= 1)
                numRandomLines = 2;

            int before = numRandomLines / 2;
            int after = before + numRandomLines % 2;

            RandomLine(before);
            AddLine(format, args);
            RandomLine(after);
        }

        /// <summary>
        /// Insert random NOP/PUSH/POP/XOR instructions.
        /// </summary>
        private void RandomLine(int numLines)
        {
            while (numLines-- > 0)
            {
                string r32 = GetRandomR32();
                string r16 = GetRandomR16();
                int type = m_Random.Next(1, 6);

                switch (type)
                {
                    case 1:
                        for (int i = m_Random.Next(1, 4); i > 0; i--) AddLine("nop");
                        break;
                    case 2:
                        AddLine("mov {0}, {0}", r32);
                        break;
                    case 3:
                        AddLine("mov {0}, {0}", r16);
                        break;
                    case 4:
                        AddLine("push {0}", r16);
                        AddLine("pop {0}", r16);
                        break;
                    case 5:
                        AddLine("push {0}", r32);
                        AddLine("pop {0}", r32);
                        break;
                }
            }
        }

        private string GetRandomR32()
        {
            return m_Random.Next(1, 7) switch
            {
                1 => "eax",
                2 => "ebx",
                3 => "ecx",
                4 => "edx",
                5 => "edi",
                6 => "esi",
                _ => "eax"
            };
        }

        private string GetRandomR16()
        {
            return m_Random.Next(1, 7) switch
            {
                1 => "ax",
                2 => "bx",
                3 => "cx",
                4 => "dx",
                5 => "di",
                6 => "si",
                _ => "ax"
            };
        }

        #endregion

        #region Detour Setup

        private void InitializeDetour()
        {
            if (!IsOpen) throw new Exception("Process is not open for memory manipulation.");
            int size = m_ClearBytes.Length;
            if (Memory.GetModule("d3d9.dll") == null)
                throw new Exception("Executor can only be used on processes that use DirectX9.");

            IntPtr k32 = Imports.GetModuleHandleW("kernel32.dll");
            if (k32 == IntPtr.Zero) throw new Exception("GetModuleHandleW(kernel32) failed");

            WaitForSingleObject = Imports.GetProcAddress(k32, "WaitForSingleObject");
            CreateEventA        = Imports.GetProcAddress(k32, "CreateEventA");
            ResetEvent          = Imports.GetProcAddress(k32, "ResetEvent");
            SetEvent            = Imports.GetProcAddress(k32, "SetEvent");

            // VEH kernel32 functions
            TlsAlloc                       = Imports.GetProcAddress(k32, "TlsAlloc");
            TlsFree                        = Imports.GetProcAddress(k32, "TlsFree");
            TlsGetValue                    = Imports.GetProcAddress(k32, "TlsGetValue");
            TlsSetValue                    = Imports.GetProcAddress(k32, "TlsSetValue");
            AddVectoredExceptionHandler    = Imports.GetProcAddress(k32, "AddVectoredExceptionHandler");
            RemoveVectoredExceptionHandler = Imports.GetProcAddress(k32, "RemoveVectoredExceptionHandler");

            if (WaitForSingleObject == UIntPtr.Zero ||
                CreateEventA == UIntPtr.Zero ||
                ResetEvent == UIntPtr.Zero ||
                SetEvent == UIntPtr.Zero)
                throw new Exception("Failed to resolve required kernel32 exports");

            if (TlsAlloc == UIntPtr.Zero || TlsFree == UIntPtr.Zero ||
                TlsGetValue == UIntPtr.Zero || TlsSetValue == UIntPtr.Zero ||
                AddVectoredExceptionHandler == UIntPtr.Zero ||
                RemoveVectoredExceptionHandler == UIntPtr.Zero)
                throw new Exception("Failed to resolve required VEH kernel32 exports");

            uint eventStub = Memory.AllocateMemory(size, 0x1000, 0x20); // PAGE_EXECUTE_READ
            if (eventStub == 0) throw new Exception("Allocate event stub failed");
            uint namesMem = Memory.AllocateMemory(size, 0x1000, 0x04); // PAGE_READWRITE
            if (namesMem == 0) throw new Exception("Allocate event names failed");

            uint name2 = namesMem + (uint)m_InjectionWaitingEventName.Length + 4;
            uint name3 = name2 + (uint)m_InjectionContinueEventName.Length + 4;
            if (!Memory.Write(namesMem, m_InjectionWaitingEventName) ||
                !Memory.Write(name2, m_InjectionContinueEventName) ||
                !Memory.Write(name3, m_InjectionFinishedEventName))
            {
                throw new Exception("Could not write event names to memory.");
            }

            m_EndSceneDetour = Memory.AllocateMemory(size, 0x1000, 0x20);
            if (m_EndSceneDetour == 0) throw new Exception("Allocate detour failed");
            m_InjectedCode = Memory.AllocateMemory(size, 0x1000, 0x240); // PAGE_EXECUTE_READWRITE | PAGE_NOCACHE
            if (m_InjectedCode == 0) throw new Exception("Allocate injected code failed");
            m_DataPtr = Memory.AllocateMemory(size, 0x1000, 0x04);
            if (m_DataPtr == 0) throw new Exception("Allocate data failed");

            // Sauvegarder les bytes originaux de EndScene pour le prologue
            // Read 16 bytes to cover all known prologue patterns (up to 7 bytes for Win8 KB3000850)
            m_OriginalEndSceneBytes = Memory.ReadBytes(m_OrigEndScene, 16);
            System.Diagnostics.Debug.WriteLine($"[InitializeDetour] Original EndScene bytes: {(m_OriginalEndSceneBytes != null ? BitConverter.ToString(m_OriginalEndSceneBytes) : "null")}");
            if (m_OriginalEndSceneBytes == null || m_OriginalEndSceneBytes.Length < 5)
                throw new Exception("Failed to read original EndScene bytes.");

            m_InjectionWaitingHandlePtr  = m_DataPtr;
            m_InjectionContinueHandlePtr = m_DataPtr + 4;
            m_InjectionFinishedHandlePtr = m_DataPtr + 8;
            m_ReturnedDataPtr            = m_DataPtr + 12;
            m_FrameCountPtr              = m_DataPtr + 16;
            m_InHookPtr                  = m_DataPtr + 20;
            m_StatusPtr                  = m_DataPtr + 24;
            m_TlsPtr                     = m_DataPtr + 28;
            m_VehPtr                     = m_DataPtr + 32;
            m_FrameDropWaitTimePtr       = m_DataPtr + 36;

            // Initialize VEH-related memory to safe defaults
            Memory.Write<uint>(m_TlsPtr, 0xFFFFFFFF);  // TLS_OUT_OF_INDEXES sentinel
            Memory.Write<uint>(m_VehPtr, 0);            // No VEH registered yet
            Memory.Write<uint>(m_StatusPtr, 0);         // Status OK
            Memory.Write<uint>(m_FrameCountPtr, 0);     // Frame counter
            Memory.Write<uint>(m_InHookPtr, 0);         // Not in hook

            if (!InjectEventStub(eventStub, namesMem, name2, name3))
                throw new Exception("InjectEventStub failed");

            InjectDetour();
            Memory.FreeMemory(eventStub);
            Memory.FreeMemory(namesMem);
        }

        /// <summary>
        /// Emit the VEH infrastructure block into the detour ASM stream.
        /// Must be called at the start of InjectDetour(), before the main loop.
        /// Emits: @ExceptionHandler, @SetUpGate, @TearDownGate, @CallGate, @CallGateInterject
        /// </summary>
        private void EmitVehBlock()
        {
            // Jump over VEH definitions to the main detour code
            AddLine("jmp @Continue");

            // ═══════════════════════════════════════════════════
            // @ExceptionHandler — VEH callback (called by Windows)
            // ═══════════════════════════════════════════════════
            // Windows VEH callback signature: LONG CALLBACK handler(EXCEPTION_POINTERS* pExInfo)
            // Stack on entry: [esp+4] = EXCEPTION_POINTERS*
            //   EXCEPTION_POINTERS { EXCEPTION_RECORD* ExceptionRecord; CONTEXT* ContextRecord; }

            AddLine("@ExceptionHandler:");
            AddLine("push ebp");
            AddLine("mov ebp, esp");
            AddLine("push ecx");                          // save scratch

            // Check TLS to identify if this is our thread
            AddLine("push dword [{0}]", m_TlsPtr);        // push TLS slot index
            AddLine("call {0}", TlsGetValue);              // TlsGetValue(slot) → eax
            AddLine("mov ecx, eax");                       // ecx = TLS value
            AddLine("test ecx, ecx");
            AddLine("jz @EHContinueSearch");               // TLS == 0 → not our thread

            // ── Our thread: exception in injected code ──
            // Read EXCEPTION_POINTERS* from stack
            AddLine("mov eax, [ebp+8]");                   // eax = EXCEPTION_POINTERS*

            // Read ExceptionRecord → ExceptionCode
            AddLine("push eax");                           // save EXCEPTION_POINTERS*
            AddLine("mov eax, [eax]");                     // eax = EXCEPTION_RECORD*
            AddLine("mov eax, [eax]");                     // eax = ExceptionCode (first field)
            AddLine("mov [{0}], eax", m_ReturnedDataPtr);  // store exception code

            // Read ContextRecord and modify it
            AddLine("pop eax");                            // eax = EXCEPTION_POINTERS* again
            AddLine("mov eax, [eax+4]");                   // eax = CONTEXT*

            // Write saved ESP (from TLS) into CONTEXT.Esp (offset 0xC4)
            // This restores the stack pointer when Windows resumes execution
            AddLine("mov [eax+0xC4], ecx");                // CONTEXT.Esp = TLS value (saved ESP)

            // Redirect CONTEXT.Eip to @CallGateInterject (offset 0xB8)
            AddLine("push @CallGateInterject");
            AddLine("pop ecx");                            // ecx = address of @CallGateInterject
            AddLine("mov [eax+0xB8], ecx");                // CONTEXT.Eip = @CallGateInterject

            // Return EXCEPTION_CONTINUE_EXECUTION (-1)
            AddLine("mov ecx, 0xFFFFFFFF");
            AddLine("jmp @EHReturn");

            AddLine("@EHContinueSearch:");
            // Not our thread — let WoW's handler deal with it
            AddLine("xor ecx, ecx");                       // return 0 = EXCEPTION_CONTINUE_SEARCH

            AddLine("@EHReturn:");
            AddLine("mov eax, ecx");
            AddLine("pop ecx");                            // restore scratch
            AddLine("pop ebp");
            AddLine("retn 4");                             // stdcall: callee cleans 1 param (4 bytes)

            // ═══════════════════════════════════════════════════
            // @SetUpGate — Allocate TLS slot + register VEH
            // ═══════════════════════════════════════════════════
            // Returns: 0 = success, 1 = TlsAlloc failed, 2 = AddVEH failed

            AddLine("@SetUpGate:");
            AddLine("call {0}", TlsAlloc);                 // TlsAlloc() → eax
            AddLine("cmp eax, 0xFFFFFFFF");
            AddLine("je @SetUpGateFail1");
            AddLine("mov [{0}], eax", m_TlsPtr);           // store TLS index

            // AddVectoredExceptionHandler(1, @ExceptionHandler)
            AddLine("push @ExceptionHandler");
            AddLine("push 1");                             // first handler (highest priority)
            AddLine("call {0}", AddVectoredExceptionHandler);
            AddLine("test eax, eax");
            AddLine("jz @SetUpGateFail2");
            AddLine("mov [{0}], eax", m_VehPtr);            // store VEH handle

            // Success
            AddLine("xor eax, eax");                       // return 0
            AddLine("jmp @SetUpGateReturn");

            AddLine("@SetUpGateFail1:");
            AddLine("mov eax, 1");                         // return 1 (TlsAlloc failed)
            AddLine("jmp @SetUpGateReturn");

            AddLine("@SetUpGateFail2:");
            AddLine("mov eax, 2");                         // return 2 (AddVEH failed)

            AddLine("@SetUpGateReturn:");
            AddLine("retn");

            // ═══════════════════════════════════════════════════
            // @TearDownGate — Remove VEH + free TLS slot
            // ═══════════════════════════════════════════════════

            AddLine("@TearDownGate:");

            // Remove VEH if registered
            AddLine("mov eax, [{0}]", m_VehPtr);
            AddLine("test eax, eax");
            AddLine("jz @TearDownRemoveTLS");              // vehPtr == 0 → skip
            AddLine("push eax");
            AddLine("call {0}", RemoveVectoredExceptionHandler);
            AddLine("mov dword [{0}], 0", m_VehPtr);       // zero out vehPtr

            AddLine("@TearDownRemoveTLS:");
            // Free TLS slot if allocated
            AddLine("mov eax, [{0}]", m_TlsPtr);
            AddLine("cmp eax, 0xFFFFFFFF");
            AddLine("je @TearDownDone");                   // TLS_OUT_OF_INDEXES → skip
            AddLine("push eax");
            AddLine("call {0}", TlsFree);
            AddLine("mov dword [{0}], 0xFFFFFFFF", m_TlsPtr); // reset sentinel

            AddLine("@TearDownDone:");
            AddLine("retn");

            // ═══════════════════════════════════════════════════
            // @CallGate — Set TLS, call injected code, return status
            // ═══════════════════════════════════════════════════
            // Returns: 0 = success, 3 = TlsSetValue failed, 4 = exception caught

            AddLine("@CallGate:");
            // TlsSetValue(tlsIndex, ESP) to mark current thread
            AddLine("push esp");                           // value = current ESP
            AddLine("push dword [{0}]", m_TlsPtr);        // key = TLS index
            AddLine("call {0}", TlsSetValue);              // TlsSetValue(slot, esp)
            AddLine("test eax, eax");
            AddLine("jz @CallGateFail3");

            // Call injected code
            AddLine("call {0}", m_InjectedCode);
            AddLine("mov [{0}], eax", m_ReturnedDataPtr);  // store return value

            // Clear TLS (unmark thread)
            AddLine("push 0");                             // value = 0
            AddLine("push dword [{0}]", m_TlsPtr);        // key = TLS index
            AddLine("call {0}", TlsSetValue);              // TlsSetValue(slot, 0)

            // Success
            AddLine("xor eax, eax");                       // return 0
            AddLine("jmp @CallGateReturn");

            AddLine("@CallGateFail3:");
            AddLine("mov eax, 3");                         // return 3 (TlsSetValue failed)
            AddLine("jmp @CallGateReturn");

            // ═══════════════════════════════════════════════════
            // @CallGateInterject — VEH redirects EIP here on crash
            // ═══════════════════════════════════════════════════
            // ESP was restored by VEH handler via CONTEXT.Esp (pointing at call @CallGate return addr)
            // The exception code is already stored in m_ReturnedDataPtr by the handler

            AddLine("@CallGateInterject:");

            // Clear TLS (unmark thread)
            AddLine("push 0");
            AddLine("push dword [{0}]", m_TlsPtr);
            AddLine("call {0}", TlsSetValue);              // TlsSetValue(slot, 0) — clear mark

            AddLine("mov eax, 4");                         // return 4 (exception caught)

            AddLine("@CallGateReturn:");
            AddLine("retn");
        }

        private void InjectDetour()
        {
            Clear();

            // ── Emit VEH infrastructure (placed before main code, jumped over) ──
            EmitVehBlock();

            // ── Main detour code starts here ──
            AddLine("@Continue:");
            AddRandomLine("pushad");

            // ── Frame counter ──
            AddRandomLine("mov eax, [{0}]", m_FrameCountPtr);
            AddRandomLine("inc eax");
            AddRandomLine("mov [{0}], eax", m_FrameCountPtr);

            AddRandomLine("@CheckInjection:");
            AddRandomLine("mov eax, [{0}]", m_InjectionWaitingHandlePtr);
            AddRandomLine("push 0");
            AddRandomLine("push eax");
            AddRandomLine("call {0}", WaitForSingleObject);
            AddRandomLine("test eax, eax");
            AddRandomLine("jnz @NoInjection");

            // ── Mark as in-hook ──
            AddRandomLine("mov dword [{0}], 1", m_InHookPtr);

            // ── Set up VEH if not already done ──
            AddRandomLine("mov eax, [{0}]", m_VehPtr);
            AddRandomLine("test eax, eax");
            AddRandomLine("jnz @DoCall");
            AddRandomLine("call @SetUpGate");
            AddRandomLine("test eax, eax");
            AddRandomLine("jnz @StoreEaxAsStatus");

            AddLine("@DoCall:");
            // ── Call injected code through VEH-protected CallGate ──
            AddRandomLine("call @CallGate");

            AddLine("@StoreEaxAsStatus:");
            AddRandomLine("mov [{0}], eax", m_StatusPtr);

            // ── Clear in-hook flag ──
            AddRandomLine("mov dword [{0}], 0", m_InHookPtr);

            // ── Signal injection finished ──
            AddRandomLine("mov eax, [{0}]", m_InjectionFinishedHandlePtr);
            AddRandomLine("push eax");
            AddRandomLine("call {0}", SetEvent);

            // ── Wait for continue signal ──
            AddRandomLine("mov eax, [{0}]", m_InjectionContinueHandlePtr);
            AddRandomLine("push 1000");
            AddRandomLine("push eax");
            AddRandomLine("call {0}", WaitForSingleObject);
            AddRandomLine("test eax, eax");
            AddRandomLine("jz @CheckInjection");

            // ── Continue not signaled → reset waiting event and fall through ──
            AddRandomLine("mov eax, [{0}]", m_InjectionWaitingHandlePtr);
            AddRandomLine("push eax");
            AddRandomLine("call {0}", ResetEvent);

            AddLine("@NoInjection:");

            // ── Tear down VEH if it was set up ──
            AddRandomLine("mov eax, [{0}]", m_VehPtr);
            AddRandomLine("test eax, eax");
            AddRandomLine("jz @SkipTearDown");
            AddRandomLine("call @TearDownGate");
            AddLine("@SkipTearDown:");

            AddRandomLine("popad");
            
            // Restore original EndScene prologue based on captured bytes
            var prolog = m_OriginalEndSceneBytes;
            if (prolog != null && prolog.Length >= 6)
            {
                Debug.WriteLine($"[InjectDetour] Prologue bytes: {BitConverter.ToString(prolog)}");

                if (prolog[0] == 0x55 && prolog[1] == 0x8B && prolog[2] == 0xEC)
                {
                    if (prolog.Length >= 6 && prolog[3] == 0x8B && prolog[4] == 0x45 && prolog[5] == 0x08)
                    {
                        // Nvidia coproc manager: push ebp; mov ebp,esp; mov eax,[ebp+8]
                        Debug.WriteLine("[InjectDetour] Using 6-byte prologue (55 8B EC 8B 45 08)");
                        AddLine("push ebp");
                        AddLine("mov ebp, esp");
                        AddLine("mov eax, [ebp+8]");
                        AddLine("jmp {0}", m_OrigEndScene + 6);
                    }
                    else if (prolog.Length >= 6 && prolog[3] == 0x83 && prolog[4] == 0xEC)
                    {
                        // Neverwinter-style: push ebp; mov ebp,esp; sub esp,XX
                        Debug.WriteLine($"[InjectDetour] Using 6-byte prologue (55 8B EC 83 EC {prolog[5]:X2})");
                        AddLine("push ebp");
                        AddLine("mov ebp, esp");
                        AddLine("sub esp, {0}", prolog[5]);
                        AddLine("jmp {0}", m_OrigEndScene + 6);
                    }
                    else
                    {
                        // Generic push ebp; mov ebp,esp — assume 5 bytes overwritten
                        Debug.WriteLine("[InjectDetour] Using generic 5-byte push ebp prologue");
                        AddLine("push ebp");
                        AddLine("mov ebp, esp");
                        AddLine("jmp {0}", m_OrigEndScene + 5);
                    }
                }
                else if (prolog[0] == 0x8B && prolog[1] == 0xFF)
                {
                    // Standard D3D9 hotpatch: mov edi,edi; push ebp; mov ebp,esp
                    Debug.WriteLine("[InjectDetour] Using 5-byte prologue (8B FF 55 8B EC)");
                    AddLine("mov edi, edi");
                    AddLine("push ebp");
                    AddLine("mov ebp, esp");
                    AddLine("jmp {0}", m_OrigEndScene + 5);
                }
                else if (prolog[0] == 0x6A && prolog.Length >= 7 && prolog[2] == 0xB8)
                {
                    // Win8 KB3000850: push imm8; mov eax,imm32 — 7 bytes
                    Debug.WriteLine("[InjectDetour] Using 7-byte Win8 prologue (6A xx B8 xx xx xx xx)");
                    AddLine("push {0}", prolog[1]);
                    uint imm32 = BitConverter.ToUInt32(prolog, 3);
                    AddLine("mov eax, {0}", imm32);
                    AddLine("jmp {0}", m_OrigEndScene + 7);
                }
                else if (prolog[0] == 0xE9)
                {
                    // Existing hook (JMP rel32) — chain with it
                    Debug.WriteLine("[InjectDetour] Existing hook detected (E9 xx xx xx xx) — chaining");
                    int rel32 = BitConverter.ToInt32(prolog, 1);
                    uint existingTarget = (uint)(m_OrigEndScene + 5 + rel32);
                    AddLine("jmp {0}", existingTarget);
                }
                else
                {
                    // Fallback to standard 5-byte prologue
                    Debug.WriteLine("[InjectDetour] FALLBACK: Using standard 5-byte prologue");
                    AddLine("mov edi, edi");
                    AddLine("push ebp");
                    AddLine("mov ebp, esp");
                    AddLine("jmp {0}", m_OrigEndScene + 5);
                }
            }
            else
            {
                // Fallback
                Debug.WriteLine("[InjectDetour] FALLBACK: Using standard 5-byte prologue");
                AddLine("mov edi, edi");
                AddLine("push ebp");
                AddLine("mov ebp, esp");
                AddLine("jmp {0}", m_OrigEndScene + 5);
            }
            
            if (!Memory.Asm!.Inject(m_EndSceneDetour))
                throw new Exception("Failed to inject detour trampoline");
                
            // Install 5-byte JMP at original EndScene to redirect to our detour
            Clear();
            AddLine("jmp {0}", m_EndSceneDetour);
            Debug.WriteLine($"[InjectDetour] Installing JMP at EndScene 0x{m_OrigEndScene:X8} -> 0x{m_EndSceneDetour:X8}");
            
            if (!Memory.Asm!.Inject(m_OrigEndScene))
                throw new Exception("Failed to inject JMP at EndScene");
                
            Debug.WriteLine("[InjectDetour] VEH-protected detour installed successfully");
        }

        private bool InjectEventStub(uint eventStub, uint waitNamePtr, uint contNamePtr, uint finNamePtr)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[InjectEventStub] eventStub=0x{eventStub:X8}, CreateEventA=0x{CreateEventA:X}");
                System.Diagnostics.Debug.WriteLine($"[InjectEventStub] m_InjectionWaitingHandlePtr=0x{m_InjectionWaitingHandlePtr:X8}");
                
                Clear();
                AddRandomLine("push {0}", waitNamePtr);
                AddRandomLine("push 0");
                AddRandomLine("push 0");
                AddRandomLine("push 0");
                AddRandomLine("call {0}", CreateEventA);
                AddRandomLine("test eax, eax");
                AddRandomLine("jz @ReturnFalse");
                AddRandomLine("mov [{0}], eax", m_InjectionWaitingHandlePtr);
                AddRandomLine("push {0}", contNamePtr);
                AddRandomLine("push 0");
                AddRandomLine("push 0");
                AddRandomLine("push 0");
                AddRandomLine("call {0}", CreateEventA);
                AddRandomLine("test eax, eax");
                AddRandomLine("jz @ReturnFalse");
                AddRandomLine("mov [{0}], eax", m_InjectionContinueHandlePtr);
                AddRandomLine("push {0}", finNamePtr);
                AddRandomLine("push 0");
                AddRandomLine("push 0");
                AddRandomLine("push 0");
                AddRandomLine("call {0}", CreateEventA);
                AddRandomLine("test eax, eax");
                AddRandomLine("jz @ReturnFalse");
                AddRandomLine("mov [{0}], eax", m_InjectionFinishedHandlePtr);
                AddLine("mov eax, 1");
                AddLine("retn");
                AddLine("@ReturnFalse:");
                AddLine("xor eax, eax");
                AddLine("retn");
                
                uint result = Memory.Asm!.InjectAndExecute(eventStub);
                System.Diagnostics.Debug.WriteLine($"[InjectEventStub] InjectAndExecute returned: {result}");
                return result == 1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[InjectEventStub] Exception: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Utilities

        private string GetRandomString(int minLength, int maxLength)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int length = minLength < maxLength ? m_Random.Next(minLength, maxLength) : minLength;

            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                result.Append(chars[(int)(m_Random.NextDouble() * (chars.Length - 1))]);

            return result.ToString();
        }

        private string GetRandomString(int length) => GetRandomString(length, length);
        private string GetRandomString() => GetRandomString(64, 64);

        #endregion

        #region Cleanup

        public void Dispose()
        {
            if (m_Disposed)
                return;

            m_Disposed = true;
            IsInitialized = false;

            try
            {
                if (Memory != null && m_OriginalEndSceneBytes != null)
                {
                    // Signal one more frame to trigger TearDownGate in the target
                    // Write a bare RET to injected code so it does nothing
                    Memory.Write<byte>(m_InjectedCode, 195); // 0xC3 = retn
                    try
                    {
                        m_InjectionWaitingEvent.Set();
                        m_InjectionFinishedEvent.WaitOne(5000, false);
                        m_InjectionWaitingEvent.Reset();
                        m_InjectionContinueEvent.Set();
                    }
                    catch { }

                    // Restore original EndScene bytes
                    Memory.WriteBytes(m_OrigEndScene, m_OriginalEndSceneBytes);
                    Debug.WriteLine($"[ExecutorRand] Restored original EndScene bytes at 0x{m_OrigEndScene:X8}");

                    // Clear injected code
                    Memory.WriteBytes(m_InjectedCode, m_ClearBytes);

                    // Free allocated memory
                    Memory.FreeMemory(m_EndSceneDetour);
                    Memory.FreeMemory(m_InjectedCode);
                    Memory.FreeMemory(m_DataPtr);
                }
            }
            catch
            {
            }

            try { m_InjectionWaitingEvent?.Close(); } catch { }
            try { m_InjectionContinueEvent?.Close(); } catch { }
            try { m_InjectionFinishedEvent?.Close(); } catch { }
        }

        #endregion
    }
}
