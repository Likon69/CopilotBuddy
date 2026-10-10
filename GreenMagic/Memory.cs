using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using Fasm;
using GreenMagic.Internals;
using GreenMagic.Native;
using Styx;
using Styx.WoWInternals;

namespace GreenMagic
{
    /// <summary>
    /// Memory manager - exact port of BlueMagic.Memory.
    /// Handles process/thread, allocation, read/write operations.
    /// </summary>
    public class Memory : IDisposable
    {
        public const int DefaultMemorySize = 4096;

        private readonly int _processId;
        private readonly IntPtr _hProcess;
        private readonly IntPtr _hThread;
        private readonly IntPtr _hWnd;
        private readonly Process _process;
        private readonly PatchManager _patchManager;
        private readonly PeHeaderParser _peHeaderParser;
        private readonly bool _aslrEnabled;
        private readonly uint _imageBase;
        private readonly ManagedFasm _asm;

        // x64 target support (see "x64 Target Support" region at the bottom).
        private readonly bool _is64BitTarget;
        private readonly ulong _imageBase64;

        // cache control mimics HB GreyMagic.ExternalProcessMemory
        private ThreadLocal<Dictionary<IntPtr, byte[]>> _cache = new ThreadLocal<Dictionary<IntPtr, byte[]>>(() => new Dictionary<IntPtr, byte[]>());
        // HB 6.2.3 StyxWoW: DefaultCacheValue = false. Cache is only enabled
        // inside the tick via TemporaryCacheState(true) under AcquireFrame.
        private ThreadLocal<bool> _cacheEnabled = new ThreadLocal<bool>(() => false);

        public Memory(int processId)
        {
            _imageBase = 0x400000; // Default WoW 3.3.5 base

            if (processId == 0)
                return;

            Process.EnterDebugMode();
            _processId = processId;
            _process = Process.GetProcesses().FirstOrDefault(p => p.Id == processId);

            if (_process == null)
                return;

            if (_process.MainModule == null)
                throw new Exception("Process has no main module");

            // OpenProcess: request only the rights we actually use.
            // VM_READ | VM_WRITE | VM_OPERATION | QUERY_INFORMATION | QUERY_LIMITED_INFORMATION
            // + SYNCHRONIZE (required for WaitForSingleObject on process handle)
            // HB used PROCESS_ALL_ACCESS (0x1F0FFF) — reduces handle detection surface.
            uint flags = 0x0002  // PROCESS_CREATE_THREAD (for InjectAndExecute via CreateRemoteThread)
                       | 0x0008  // PROCESS_VM_OPERATION
                       | 0x0010  // PROCESS_VM_READ
                       | 0x0020  // PROCESS_VM_WRITE
                       | 0x0400  // PROCESS_QUERY_INFORMATION
                       | 0x1000  // PROCESS_QUERY_LIMITED_INFORMATION
                       | 0x100000; // SYNCHRONIZE

            _hProcess = Imports.OpenProcess(flags, false, processId);
            
            if (_hProcess != IntPtr.Zero)
            {
                // OpenThread: minimum rights for SuspendThread, ResumeThread,
                // GetThreadContext, SetThreadContext used by EIP hijack init.
                uint threadFlags = 0x0002   // THREAD_SUSPEND_RESUME
                                 | 0x0008   // THREAD_GET_CONTEXT
                                 | 0x0010   // THREAD_SET_CONTEXT
                                 | 0x0040   // THREAD_QUERY_INFORMATION
                                 | 0x100000; // SYNCHRONIZE
                _hThread = Imports.OpenThread(threadFlags, false, (uint)_process.Threads[0].Id);
                _hWnd = _process.MainWindowHandle;

                if (_asm == null)
                    _asm = new ManagedFasm(_hProcess);
                else
                    _asm.SetProcessHandle(_hProcess);
            }

            _patchManager = new PatchManager(this);

            // PE header parsing. The parser understands both PE32 (32-bit)
            // and PE32+ (64-bit) images. Guarded: a parse failure degrades
            // to defaults instead of aborting the attach.
            PeHeaderParser parser = null;
            bool aslr = false;
            try
            {
                parser = new PeHeaderParser(_process.MainModule.BaseAddress, this);
                aslr = parser.Is64BitImage
                    ? (parser.NtHeader64.OptionalHeader.DllCharacteristics & 64) != 0
                    : (parser.NtHeader.OptionalHeader.DllCharacteristics & 64) != 0;
            }
            catch
            {
                parser = null;
                aslr = false;
            }
            _peHeaderParser = parser;
            _aslrEnabled = aslr;

            // Detect whether the attached process is a native 64-bit
            // executable (e.g. a modern x64 WoW client build). The check
            // reads the PE machine field from the main module; if anything
            // goes wrong we silently fall back to classic 32-bit behavior
            // so the existing 3.3.5a attach path is unaffected.
            _is64BitTarget = DetectTargetIs64Bit(_hProcess, _process.MainModule.BaseAddress);
            _imageBase64 = _is64BitTarget
                ? (ulong)_process.MainModule.BaseAddress.ToInt64()
                : _imageBase;

            if (!_is64BitTarget && _aslrEnabled)
                _imageBase = (uint)_process.MainModule.BaseAddress.ToInt32();
        }

        #region Properties

        public ManagedFasm Asm => _asm;
        internal PeHeaderParser PeHeaderParser => _peHeaderParser;
        public PatchManager PatchManager => _patchManager;
        public bool IsThreadOpen => _hThread != IntPtr.Zero;
        public bool IsProcessOpen => _hProcess != IntPtr.Zero;
        public int ProcessId => _processId;
        public IntPtr ProcessHandle => _hProcess;
        public IntPtr ThreadHandle => _hThread;
        public IntPtr WindowHandle => _hWnd;
        public Process Process => _process;
        public uint ImageBase => _imageBase;

        /// <summary>
        /// True when the attached process is a native 64-bit executable
        /// (IMAGE_FILE_MACHINE_AMD64). When true, the legacy uint-addressed
        /// API of this class cannot represent addresses above 4 GB — use
        /// the ulong-addressed methods in the "x64 Target Support" region.
        /// </summary>
        public bool Is64BitTarget => _is64BitTarget;

        /// <summary>
        /// Image base of the target's main module as a full 64-bit value.
        /// For 32-bit targets this equals <see cref="ImageBase"/>.
        /// </summary>
        public ulong ImageBase64 => _imageBase64;

        #endregion

        #region Address Calculation

        public uint GetAbsolute(uint relative)
        {
            return relative + _imageBase;
        }

        /// <summary>
        /// Converts an image-relative offset to an absolute 64-bit address.
        /// The ulong equivalent of <see cref="GetAbsolute"/>.
        /// </summary>
        public ulong GetAbsolute64(ulong relative)
        {
            return relative + _imageBase64;
        }

        #endregion

        #region Read Methods

        private unsafe T ReadInternal<T>(uint address)
        {
            Type type = typeof(T);

            if (type == typeof(string))
                return (T)(object)ReadString(Encoding.UTF8, address);

            int size = FastSize<T>.Size;
            byte[] buffer = ReadBytes(address, size);

            if (buffer == null)
                return default;

            fixed (byte* ptr = buffer)
            {
                return (T)Marshal.PtrToStructure(new IntPtr(ptr), type);
            }
        }

        public T Read<T>(params uint[] addresses)
        {
            if (addresses.Length == 0)
                return default;

            if (addresses.Length == 1)
                return ReadInternal<T>(addresses[0]);

            uint ptr = 0;
            for (int i = 0; i < addresses.Length; i++)
            {
                if (i == addresses.Length - 1)
                    return ReadInternal<T>(addresses[i] + ptr);

                ptr = ReadInternal<uint>(ptr + addresses[i]);
            }

            return default;
        }

        public T ReadRelative<T>(params uint[] addresses)
        {
            if (addresses.Length == 0)
                return default;

            if (addresses.Length == 1)
                return ReadInternal<T>(GetAbsolute(addresses[0]));

            uint ptr = 0;
            for (int i = 0; i < addresses.Length; i++)
            {
                if (i == addresses.Length - 1)
                    return ReadInternal<T>(GetAbsolute(addresses[i]) + ptr);

                ptr = ReadInternal<uint>(ptr + GetAbsolute(addresses[i]));
            }

            return default;
        }

        public unsafe T ReadStruct<T>(uint address) where T : struct
        {
            int size = FastSize<T>.Size;
            byte[] buffer = ReadBytes(address, size);

            if (buffer.Length < size)
                throw new ArgumentException("Buffer too small", "address");

            fixed (byte* ptr = buffer)
            {
                return (T)Marshal.PtrToStructure(new IntPtr(ptr), typeof(T));
            }
        }

        public T ReadStruct<T>(long address) where T : struct
        {
            return ReadStruct<T>(Convert.ToUInt32(address));
        }

        public unsafe T[] ReadStructArray<T>(IntPtr address, int elements) where T : struct
        {
            Type type = typeof(T);
            int size = FastSize<T>.Size;
            byte[] buffer = ReadBytes(address.ToUInt32(), elements * size);
            T[] result = new T[elements];

            // original HB 3.3.5a guarded against a null buffer
            if (buffer != null)
            {
                fixed (byte* ptr = buffer)
                {
                    for (int i = 0; i < elements; i++)
                    {
                        result[i] = (T)Marshal.PtrToStructure(new IntPtr(ptr + i * size), type);
                    }
                }
            }

            return result;
        }

        public T[] ReadStructArray<T>(uint address, int elements) where T : struct
        {
            return ReadStructArray<T>(new IntPtr(unchecked((int)address)), elements);
        }

        public T[] ReadStructArrayRelative<T>(uint address, int elements) where T : struct
        {
            return ReadStructArray<T>(GetAbsolute(address), elements);
        }

        public byte[] ReadBytes(uint address, int count)
        {
            if (ProcessHandle == IntPtr.Zero)
                throw new InvalidOperationException("Process handle is not open");

            if (count == 0)
                return Array.Empty<byte>();

            // HB 5.4.8 ExternalProcessMemory.ReadByteBuffer cache pattern:
            // check cache first, RPM on miss, store result in cache.
            // unchecked: uint addresses > 0x7FFFFFFF overflow IntPtr(long) on x86 — reinterpret bits instead.
            var key = new IntPtr(unchecked((int)address));
            if (_cacheEnabled.Value)
            {
                var dict = _cache.Value;
                if (dict.TryGetValue(key, out byte[]? cached))
                {
                    if (cached.Length == count)
                        return cached;
                    dict.Remove(key); // size mismatch — stale entry
                }
            }

            byte[] buffer = new byte[count];
            int bytesRead;

            if (Imports.ReadProcessMemory(ProcessHandle, address, buffer, count, out bytesRead) && bytesRead == count)
            {
                if (_cacheEnabled.Value)
                    _cache.Value[key] = buffer;
                return buffer;
            }

            return null;
        }

        public byte[] ReadBytes(IntPtr address, int count)
        {
            if (_hProcess == IntPtr.Zero)
                throw new InvalidOperationException("Process handle is not open");

            if (count == 0)
                return Array.Empty<byte>();

            if (_cacheEnabled.Value)
            {
                var dict = _cache.Value;
                if (dict.TryGetValue(address, out byte[]? cached))
                {
                    if (cached.Length == count)
                        return cached;
                    dict.Remove(address);
                }
            }

            byte[] buffer = new byte[count];
            int bytesRead;

            if (Imports.ReadProcessMemory(_hProcess, address.ToUInt32(), buffer, count, out bytesRead) && bytesRead == count)
            {
                if (_cacheEnabled.Value)
                    _cache.Value[address] = buffer;
                return buffer;
            }

            return null;
        }

        public unsafe void ReadBytes(uint address, byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            fixed (byte* ptr = bytes)
            {
                ReadRawMemory(_hProcess, address, new IntPtr(ptr), bytes.Length);
            }
        }

        public unsafe void ReadBytes(uint address, void* buffer, int count)
        {
            ReadRawMemory(_hProcess, address, new IntPtr(buffer), count);
        }

        public uint ReadUInt32(uint address)
        {
            byte[] buffer = ReadBytes(address, 4);
            if (buffer == null)
                return 0;
            return BitConverter.ToUInt32(buffer, 0);
        }

        public int ReadInt32(uint address)
        {
            byte[] buffer = ReadBytes(address, 4);
            if (buffer == null)
                return 0;
            return BitConverter.ToInt32(buffer, 0);
        }

        public int ReadRawMemory(IntPtr hProcess, uint address, IntPtr buffer, int size)
        {
            try
            {
                int bytesRead;
                if (!Imports.ReadProcessMemory(hProcess, address, buffer, size, out bytesRead))
                    throw new Exception("ReadProcessMemory failed");
                return bytesRead;
            }
            catch
            {
                return 0;
            }
        }

        public string ReadString(Encoding encoding, uint address, int length)
        {
            byte[] buffer = new byte[512];
            int bytesRead;
            Imports.ReadProcessMemory(ProcessHandle, address, buffer, buffer.Length, out bytesRead);

            string result = encoding.GetString(buffer);
            int nullIndex = result.IndexOf('\0');
            if (nullIndex != -1)
                result = result.Remove(nullIndex);

            return result;
        }

        public string ReadString(Encoding encoding, uint address)
        {
            return ReadString(encoding, address, 256);
        }

        public string ReadString(uint address, int length)
        {
            return ReadString(Encoding.UTF8, address, length);
        }

        public string ReadString(uint address)
        {
            return ReadString(address, 256);
        }

        #endregion

        #region Write Methods

        public bool Write(uint address, string value)
        {
            return WriteString(address, value);
        }

        public bool Write(uint address, byte[] value)
        {
            return WriteBytes(address, value) == value.Length;
        }

        public bool Write<T>(uint address, T value)
        {
            if (typeof(T) == typeof(string))
            {
                return WriteString(address, Convert.ToString(value));
            }

            try
            {
                object obj = value;

                if (typeof(T) == typeof(byte[]))
                {
                    byte[] arr = (byte[])obj;
                    return WriteBytes(address, arr) == arr.Length;
                }

                TypeCode typeCode = Type.GetTypeCode(typeof(T));
                byte[] bytes;

                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        bytes = BitConverter.GetBytes((bool)obj);
                        break;
                    case TypeCode.Char:
                        bytes = BitConverter.GetBytes((char)obj);
                        break;
                    case TypeCode.Byte:
                        bytes = new byte[] { (byte)obj };
                        break;
                    case TypeCode.Int16:
                        bytes = BitConverter.GetBytes((short)obj);
                        break;
                    case TypeCode.UInt16:
                        bytes = BitConverter.GetBytes((ushort)obj);
                        break;
                    case TypeCode.Int32:
                        bytes = BitConverter.GetBytes((int)obj);
                        break;
                    case TypeCode.UInt32:
                        bytes = BitConverter.GetBytes((uint)obj);
                        break;
                    case TypeCode.Int64:
                        bytes = BitConverter.GetBytes((long)obj);
                        break;
                    case TypeCode.UInt64:
                        bytes = BitConverter.GetBytes((ulong)obj);
                        break;
                    case TypeCode.Single:
                        bytes = BitConverter.GetBytes((float)obj);
                        break;
                    case TypeCode.Double:
                        bytes = BitConverter.GetBytes((double)obj);
                        break;
                    default:
                        return WriteObject(address, value);
                }

                return WriteBytes(address, bytes) == bytes.Length;
            }
            catch
            {
                return false;
            }
        }

        public bool WriteStruct<T>(uint address, T value) where T : struct
        {
            try
            {
                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(value));
                byte[] buffer = new byte[Marshal.SizeOf(value)];
                Marshal.Copy(ptr, buffer, 0, Marshal.SizeOf(value));
                Marshal.FreeHGlobal(ptr);
                return WriteBytes(address, buffer) > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool WriteObject(uint address, object obj)
        {
            return WriteObject(address, obj, obj.GetType());
        }

        public bool WriteObject(uint address, object obj, Type type)
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(type);
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(obj, ptr, false);
                int written = WriteRawMemory(_hProcess, address, ptr, size);
                if (size != written)
                    throw new Exception("WriteObject size mismatch");
            }
            catch
            {
                return false;
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(ptr, type);
                    Marshal.FreeHGlobal(ptr);
                }
            }
            return true;
        }

        public int WriteBytes(uint address, byte[] data)
        {
            if (ProcessHandle == IntPtr.Zero)
                throw new Exception("Process handle is not open");

            int bytesWritten;
            if (!Imports.WriteProcessMemory(ProcessHandle, address, data, (uint)data.Length, out bytesWritten))
                throw new Exception($"WriteProcessMemory failed at 0x{address:X8}. Error: {Marshal.GetLastWin32Error()}");

            return bytesWritten;
        }

        private static int WriteRawMemory(IntPtr hProcess, uint address, IntPtr buffer, int size)
        {
            IntPtr bytesWritten;
            if (!Imports.WriteProcessMemory(hProcess, address, buffer, size, out bytesWritten))
                return 0;

            return (int)bytesWritten;
        }

        private bool WriteString(uint address, string value)
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                // StringToHGlobalAnsi already adds null terminator
                ptr = Marshal.StringToHGlobalAnsi(value);
                int length = value.Length + 1; // +1 for null terminator
                int written = WriteRawMemory(_hProcess, address, ptr, length);
                if (length != written)
                    throw new Exception();
            }
            catch
            {
                return false;
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }
            return true;
        }

        #endregion

        /// <summary>
        /// Return an <see cref="ExternalReadCache"/> saving the current
        /// enabled state.  Mirrors HB's ExternalProcessMemory.SaveCacheState.
        /// </summary>
        public ExternalReadCache SaveCacheState()
        {
            return new ExternalReadCache(this);
        }

        /// <summary>
        /// Temporarily toggles the cache flag and returns a disposable that
        /// restores the previous state when disposed.  Parameters and behavior
        /// exactly match HB's TemporaryCacheState.
        /// </summary>
        public ExternalReadCache TemporaryCacheState(bool enabledTemporarily)
        {
            return new ExternalReadCache(this, enabledTemporarily);
        }

        public void ClearCache()
        {
            _cache.Value.Clear();
        }

        public void DisableCache()
        {
            _cacheEnabled.Value = false;
        }

        public void EnableCache()
        {
            _cacheEnabled.Value = true;
        }

        public bool CacheEnabled
        {
            get { return _cacheEnabled.Value; }
        }

        #region Frame Support

        /// <summary>
        /// HB 5.4.8/6.2.3 AcquireFrame: FrameLock FIRST (acquires
        /// AssemblyLock + BeginExecute), then GrabFrame INSIDE the lock
        /// scope.  The original CopilotBuddy code had the order reversed
        /// (GrabFrame first, FrameLock second), which left a race window
        /// where the background thread could steal the lock.
        /// </summary>
        public IDisposable AcquireFrame(bool isHardLock = false)
        {
            var frameLock = new Styx.FrameLock();
            try
            {
                if (isHardLock)
                {
                    ObjectManager.Executor?.GrabFrame();
                }
            }
            catch
            {
                frameLock.Dispose();
                throw;
            }
            return frameLock;
        }

        public IDisposable AcquireFrame()
        {
            return AcquireFrame(false);
        }

        /// <summary>
        /// HB 6.2.3 ExternalProcessMemory.ReleaseFrame — returns a
        /// <see cref="FrameLockRelease"/> that ends the current continuous
        /// execution, letting WoW render while we do CPU-only work (e.g.
        /// pathfinding).  On Dispose the frame lock is reacquired.
        /// </summary>
        public FrameLockRelease ReleaseFrame(bool reacquireAsHardLock = false)
        {
            return new FrameLockRelease(ObjectManager.Executor, reacquireAsHardLock);
        }

        public FrameLockRelease ReleaseFrame()
        {
            return new FrameLockRelease(ObjectManager.Executor, false);
        }

        #endregion

        #region Memory Allocation

        public uint AllocateMemory(int size, uint allocationType, uint protect)
        {
            // HB 6.2.3 two-step pattern: allocate as PAGE_READWRITE first,
            // then VirtualProtectEx to the real protection if different.
            // This avoids creating pages with the final protection atomically
            // (VirtualAllocEx with RWX is more suspicious than alloc RW + protect).
            uint addr = Imports.VirtualAllocEx(ProcessHandle, 0U, size, allocationType, 0x04); // PAGE_READWRITE
            if (addr != 0 && protect != 0x04)
            {
                if (!Imports.VirtualProtectEx(ProcessHandle, addr, (uint)size, protect, out _))
                    throw new System.ComponentModel.Win32Exception();
            }
            return addr;
        }

        public uint AllocateMemory(int size)
        {
            return AllocateMemory(size, 0x1000U, 0x04U); // MEM_COMMIT, PAGE_READWRITE
        }

        /// <summary>
        /// Overload for compatibility with ExecutorRand calls
        /// </summary>
        public uint AllocateMemory(int size, int allocationType, uint protect)
        {
            return AllocateMemory(size, (uint)allocationType, protect);
        }

        public uint AllocateMemory()
        {
            return AllocateMemory(DefaultMemorySize);
        }

        public bool FreeMemory(uint address, int size, uint freeType)
        {
            if (freeType == 0x8000U) // MEM_RELEASE
                size = 0;

            return Imports.VirtualFreeEx(ProcessHandle, address, size, freeType);
        }

        public bool FreeMemory(uint address)
        {
            return FreeMemory(address, 0, 0x8000U); // MEM_RELEASE
        }

        #endregion

        // Note: HB BlueMagic does not include explicit memory protection helpers.

        #region Thread Control

        public uint SuspendThread(IntPtr hThread)
        {
            return Imports.SuspendThread(hThread);
        }

        public uint SuspendThread()
        {
            return Imports.SuspendThread(ThreadHandle);
        }

        public uint ResumeThread(IntPtr hThread)
        {
            return Imports.ResumeThread(hThread);
        }

        public uint ResumeThread()
        {
            return Imports.ResumeThread(ThreadHandle);
        }

        public uint TerminateThread(IntPtr hThread, uint exitCode)
        {
            return Imports.TerminateThread(hThread, exitCode);
        }

        public uint TerminateThread(uint exitCode)
        {
            return TerminateThread(ThreadHandle, exitCode);
        }

        public Context GetThreadContext(IntPtr hThread, uint contextFlags)
        {
            Context context = new Context { ContextFlags = contextFlags };

            if (!Imports.GetThreadContext(hThread, ref context))
                context.ContextFlags = 0U;

            return context;
        }

        public Context GetThreadContext(uint contextFlags)
        {
            return GetThreadContext(ThreadHandle, contextFlags);
        }

        public bool SetThreadContext(IntPtr hThread, Context ctx)
        {
            return Imports.SetThreadContext(hThread, ref ctx);
        }

        public bool SetThreadContext(Context ctx)
        {
            return Imports.SetThreadContext(ThreadHandle, ref ctx);
        }

        #endregion

        #region Remote Thread

        public IntPtr CreateRemoteThread(uint startAddress, uint parameter)
        {
            uint threadId;
            return CreateRemoteThread(ProcessHandle, startAddress, parameter, 0U, out threadId);
        }

        public IntPtr CreateRemoteThread(IntPtr hProcess, uint startAddress, uint parameter)
        {
            uint threadId;
            return CreateRemoteThread(hProcess, startAddress, parameter, 0U, out threadId);
        }

        public IntPtr CreateRemoteThread(IntPtr hProcess, uint startAddress, uint parameter, out uint threadId)
        {
            return CreateRemoteThread(hProcess, startAddress, parameter, 0U, out threadId);
        }

        public IntPtr CreateRemoteThread(IntPtr hProcess, uint startAddress, uint parameter, uint creationFlags, out uint threadId)
        {
            IntPtr tid;
            IntPtr handle = Imports.CreateRemoteThread(hProcess, IntPtr.Zero, 0U, new IntPtr(unchecked((int)startAddress)), new IntPtr(unchecked((int)parameter)), creationFlags, out tid);
            threadId = (uint)tid.ToInt32();
            return handle;
        }

        public uint GetExitCodeThread(IntPtr hThread)
        {
            UIntPtr exitCode;
            if (!Imports.GetExitCodeThread(hThread, out exitCode))
                throw new Exception("GetExitCodeThread failed");

            return (uint)exitCode;
        }

        public uint WaitForSingleObject(IntPtr hObject)
        {
            return Imports.WaitForSingleObject(hObject, uint.MaxValue);
        }

        public uint WaitForSingleObject(IntPtr hObject, uint milliseconds)
        {
            return Imports.WaitForSingleObject(hObject, milliseconds);
        }

        #endregion

        #region Module Methods

        public ProcessModule GetModule(string moduleName)
        {
            return _process.Modules.Cast<ProcessModule>()
                .FirstOrDefault(m => m.ModuleName.ToLower().Equals(moduleName.ToLower()));
        }

        #endregion

        #region x64 Target Support

        /// <summary>
        /// Reads the PE machine field of the target's main module to decide
        /// whether it is a native 64-bit process. Returns false on any
        /// failure — the default keeps the legacy 32-bit behavior.
        /// </summary>
        private static bool DetectTargetIs64Bit(IntPtr hProcess, IntPtr moduleBase)
        {
            try
            {
                if (hProcess == IntPtr.Zero || moduleBase == IntPtr.Zero)
                    return false;

                byte[] dosHeader = new byte[64];
                IntPtr bytesRead;
                if (!Imports.ReadProcessMemory64(hProcess, moduleBase, dosHeader, (IntPtr)dosHeader.Length, out bytesRead)
                    || (long)bytesRead != dosHeader.Length)
                    return false;

                if (dosHeader[0] != (byte)'M' || dosHeader[1] != (byte)'Z')
                    return false;

                uint eLfanew = BitConverter.ToUInt32(dosHeader, 0x3C);

                // PE signature (4 bytes) + machine field (2 bytes)
                byte[] ntStart = new byte[6];
                if (!Imports.ReadProcessMemory64(hProcess, new IntPtr(moduleBase.ToInt64() + eLfanew), ntStart, (IntPtr)ntStart.Length, out bytesRead)
                    || (long)bytesRead != ntStart.Length)
                    return false;

                if (ntStart[0] != (byte)'P' || ntStart[1] != (byte)'E' || ntStart[2] != 0 || ntStart[3] != 0)
                    return false;

                ushort machine = BitConverter.ToUInt16(ntStart, 4);
                return machine == 0x8664; // IMAGE_FILE_MACHINE_AMD64
            }
            catch
            {
                return false;
            }
        }

        // ------------------------------------------------------------------
        // 64-bit address read primitives.
        //
        // The uint-addressed methods above work unchanged for 32-bit
        // targets. For 64-bit targets addresses may live above 4 GB, so
        // this region mirrors the same API surface with ulong addresses
        // and 8-byte pointer chains.
        // ------------------------------------------------------------------

        public byte[] ReadBytes64(ulong address, int count)
        {
            if (ProcessHandle == IntPtr.Zero)
                throw new InvalidOperationException("Process handle is not open");

            if (count == 0)
                return Array.Empty<byte>();

            IntPtr key = new IntPtr((long)address);
            if (_cacheEnabled.Value)
            {
                var dict = _cache.Value;
                if (dict.TryGetValue(key, out byte[]? cached))
                {
                    if (cached.Length == count)
                        return cached;
                    dict.Remove(key); // size mismatch — stale entry
                }
            }

            byte[] buffer = new byte[count];
            IntPtr bytesRead;
            if (Imports.ReadProcessMemory64(ProcessHandle, key, buffer, (IntPtr)count, out bytesRead)
                && (long)bytesRead == count)
            {
                if (_cacheEnabled.Value)
                    _cache.Value[key] = buffer;
                return buffer;
            }

            return null;
        }

        public int ReadRawMemory64(IntPtr hProcess, ulong address, IntPtr buffer, int size)
        {
            try
            {
                IntPtr bytesRead;
                if (!Imports.ReadProcessMemory64(hProcess, new IntPtr((long)address), buffer, (IntPtr)size, out bytesRead))
                    throw new Exception("ReadProcessMemory failed");
                return (int)bytesRead.ToInt64();
            }
            catch
            {
                return 0;
            }
        }

        private unsafe T ReadInternal64<T>(ulong address)
        {
            Type type = typeof(T);

            if (type == typeof(string))
                return (T)(object)ReadString64(address);

            int size = FastSize<T>.Size;
            byte[] buffer = ReadBytes64(address, size);

            if (buffer == null)
                return default;

            fixed (byte* ptr = buffer)
            {
                return (T)Marshal.PtrToStructure(new IntPtr(ptr), type);
            }
        }

        /// <summary>Reads a value from a full 64-bit address.</summary>
        public T Read64<T>(ulong address)
        {
            return ReadInternal64<T>(address);
        }

        /// <summary>Reads an 8-byte pointer from a 64-bit address.</summary>
        public ulong ReadPointer64(ulong address)
        {
            byte[] buffer = ReadBytes64(address, 8);
            if (buffer == null)
                return 0UL;
            return BitConverter.ToUInt64(buffer, 0);
        }

        /// <summary>
        /// Follows a pointer chain with 8-byte pointers (the x64 object
        /// model): each intermediate entry is read as a pointer, the final
        /// entry is read as T at (finalPointer + addresses[^1]).
        /// 64-bit analog of <see cref="Read{T}(uint[])"/>.
        /// </summary>
        public T ReadChain64<T>(params ulong[] addresses)
        {
            if (addresses.Length == 0)
                return default;

            if (addresses.Length == 1)
                return ReadInternal64<T>(addresses[0]);

            ulong ptr = 0UL;
            for (int i = 0; i < addresses.Length; i++)
            {
                if (i == addresses.Length - 1)
                    return ReadInternal64<T>(ptr + addresses[i]);

                ptr = ReadPointer64(ptr + addresses[i]);
            }

            return default;
        }

        /// <summary>
        /// Image-relative variant of <see cref="ReadChain64{T}(ulong[])"/> —
        /// the first address is resolved through <see cref="GetAbsolute64"/>.
        /// 64-bit analog of <see cref="ReadRelative{T}(uint[])"/>.
        /// </summary>
        public T ReadChainRelative64<T>(params ulong[] addresses)
        {
            if (addresses.Length == 0)
                return default;

            if (addresses.Length == 1)
                return ReadInternal64<T>(GetAbsolute64(addresses[0]));

            ulong ptr = 0UL;
            for (int i = 0; i < addresses.Length; i++)
            {
                if (i == addresses.Length - 1)
                    return ReadInternal64<T>(GetAbsolute64(addresses[i]) + ptr);

                ptr = ReadPointer64(ptr + GetAbsolute64(addresses[i]));
            }

            return default;
        }

        public unsafe T ReadStruct64<T>(ulong address) where T : struct
        {
            int size = FastSize<T>.Size;
            byte[] buffer = ReadBytes64(address, size);

            if (buffer == null || buffer.Length < size)
                throw new ArgumentException("Buffer too small", "address");

            fixed (byte* ptr = buffer)
            {
                return (T)Marshal.PtrToStructure(new IntPtr(ptr), typeof(T));
            }
        }

        public string ReadString64(ulong address)
        {
            return ReadString64(address, 512);
        }

        public string ReadString64(ulong address, int length)
        {
            return ReadString64(Encoding.UTF8, address, length);
        }

        public string ReadString64(Encoding encoding, ulong address, int length)
        {
            byte[] buffer = ReadBytes64(address, length);
            if (buffer == null)
                return null;

            string result = encoding.GetString(buffer);
            int nullIndex = result.IndexOf('\0');
            if (nullIndex != -1)
                result = result.Remove(nullIndex);

            return result;
        }

        // ------------------------------------------------------------------
        // 64-bit address write primitives.
        // ------------------------------------------------------------------

        /// <summary>Writes raw bytes to a 64-bit address. Returns success.</summary>
        public bool WriteBytes64(ulong address, byte[] data)
        {
            if (ProcessHandle == IntPtr.Zero)
                throw new InvalidOperationException("Process handle is not open");

            IntPtr bytesWritten;
            if (!Imports.WriteProcessMemory64(ProcessHandle, new IntPtr((long)address), data, (IntPtr)data.Length, out bytesWritten))
                throw new Exception($"WriteProcessMemory failed at 0x{address:X16}. Error: {Marshal.GetLastWin32Error()}");

            return (long)bytesWritten == data.Length;
        }

        public int WriteRawMemory64(IntPtr hProcess, ulong address, IntPtr buffer, int size)
        {
            IntPtr bytesWritten;
            if (!Imports.WriteProcessMemory64(hProcess, new IntPtr((long)address), buffer, (IntPtr)size, out bytesWritten))
                return 0;

            return (int)bytesWritten.ToInt64();
        }

        public bool Write64<T>(ulong address, T value)
        {
            try
            {
                if (typeof(T) == typeof(string))
                    return WriteString64(address, Convert.ToString(value));

                if (typeof(T) == typeof(byte[]))
                {
                    byte[] arr = (byte[])(object)value;
                    return WriteBytes64(address, arr);
                }

                object obj = value;
                TypeCode typeCode = Type.GetTypeCode(typeof(T));
                byte[] bytes;

                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        bytes = BitConverter.GetBytes((bool)obj);
                        break;
                    case TypeCode.Char:
                        bytes = BitConverter.GetBytes((char)obj);
                        break;
                    case TypeCode.Byte:
                        bytes = new byte[] { (byte)obj };
                        break;
                    case TypeCode.Int16:
                        bytes = BitConverter.GetBytes((short)obj);
                        break;
                    case TypeCode.UInt16:
                        bytes = BitConverter.GetBytes((ushort)obj);
                        break;
                    case TypeCode.Int32:
                        bytes = BitConverter.GetBytes((int)obj);
                        break;
                    case TypeCode.UInt32:
                        bytes = BitConverter.GetBytes((uint)obj);
                        break;
                    case TypeCode.Int64:
                        bytes = BitConverter.GetBytes((long)obj);
                        break;
                    case TypeCode.UInt64:
                        bytes = BitConverter.GetBytes((ulong)obj);
                        break;
                    case TypeCode.Single:
                        bytes = BitConverter.GetBytes((float)obj);
                        break;
                    case TypeCode.Double:
                        bytes = BitConverter.GetBytes((double)obj);
                        break;
                    default:
                        return WriteObject64(address, obj);
                }

                return WriteBytes64(address, bytes);
            }
            catch
            {
                return false;
            }
        }

        public bool WritePointer64(ulong address, ulong pointer)
        {
            return WriteBytes64(address, BitConverter.GetBytes(pointer));
        }

        public bool WriteObject64(ulong address, object obj)
        {
            return WriteObject64(address, obj, obj.GetType());
        }

        public bool WriteObject64(ulong address, object obj, Type type)
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(type);
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(obj, ptr, false);
                int written = WriteRawMemory64(_hProcess, address, ptr, size);
                if (size != written)
                    throw new Exception("WriteObject size mismatch");
            }
            catch
            {
                return false;
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(ptr, type);
                    Marshal.FreeHGlobal(ptr);
                }
            }
            return true;
        }

        private bool WriteString64(ulong address, string value)
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                // StringToHGlobalAnsi already adds the null terminator
                ptr = Marshal.StringToHGlobalAnsi(value);
                int length = value.Length + 1;
                int written = WriteRawMemory64(_hProcess, address, ptr, length);
                if (length != written)
                    throw new Exception();
            }
            catch
            {
                return false;
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }
            return true;
        }

        // ------------------------------------------------------------------
        // 64-bit allocation, query, thread-context and remote-thread.
        // ------------------------------------------------------------------

        /// <summary>
        /// Allocates memory in the target with a full 64-bit address.
        /// Same two-step RW-then-protect pattern as
        /// <see cref="AllocateMemory(int, uint, uint)"/>.
        /// </summary>
        public ulong AllocateMemory64(int size, uint allocationType, uint protect)
        {
            ulong addr = (ulong)Imports.VirtualAllocEx64(ProcessHandle, IntPtr.Zero, (IntPtr)size, allocationType, 0x04).ToInt64(); // PAGE_READWRITE
            if (addr != 0UL && protect != 0x04U)
            {
                if (!Imports.VirtualProtectEx64(ProcessHandle, new IntPtr((long)addr), (IntPtr)size, protect, out _))
                    throw new System.ComponentModel.Win32Exception();
            }
            return addr;
        }

        public ulong AllocateMemory64(int size)
        {
            return AllocateMemory64(size, 0x1000U, 0x04U); // MEM_COMMIT, PAGE_READWRITE
        }

        public bool FreeMemory64(ulong address)
        {
            return Imports.VirtualFreeEx64(ProcessHandle, new IntPtr((long)address), IntPtr.Zero, 0x8000U); // MEM_RELEASE
        }

        /// <summary>Queries a 64-bit MEMORY_BASIC_INFORMATION for an address.</summary>
        public MEMORY_BASIC_INFORMATION64 QueryMemory64(ulong address)
        {
            MEMORY_BASIC_INFORMATION64 info;
            Imports.VirtualQueryEx64(ProcessHandle, new IntPtr((long)address), out info, (IntPtr)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION64)));
            return info;
        }

        /// <summary>
        /// Retrieves the thread context of a 64-bit thread. The FltSave and
        /// VectorRegister arrays must be pre-allocated for the marshaller,
        /// which this method handles.
        /// </summary>
        public Context64 GetThreadContext64(IntPtr hThread, ulong contextFlags)
        {
            Context64 context = new Context64
            {
                ContextFlags = (uint)contextFlags,
                FltSave = new byte[512],
                VectorRegister = new M128A[26]
            };

            if (!Imports.GetThreadContext64(hThread, ref context))
                context.ContextFlags = 0U;

            return context;
        }

        public Context64 GetThreadContext64(ulong contextFlags)
        {
            return GetThreadContext64(ThreadHandle, contextFlags);
        }

        public bool SetThreadContext64(IntPtr hThread, Context64 ctx)
        {
            return Imports.SetThreadContext64(hThread, ref ctx);
        }

        public bool SetThreadContext64(Context64 ctx)
        {
            return Imports.SetThreadContext64(ThreadHandle, ref ctx);
        }

        /// <summary>
        /// Creates a remote thread in the target starting at a 64-bit
        /// address. Only valid for a 64-bit target process.
        /// </summary>
        public IntPtr CreateRemoteThread64(ulong startAddress, ulong parameter)
        {
            IntPtr threadId;
            return Imports.CreateRemoteThread(ProcessHandle, IntPtr.Zero, 0U,
                new IntPtr((long)startAddress), new IntPtr((long)parameter), 0U, out threadId);
        }

        #endregion

        #region Dispose

        internal void CloseProcessHandle()
        {
            Imports.CloseHandle(ProcessHandle);
            Process.LeaveDebugMode();
        }

        public void Dispose()
        {
            CloseProcessHandle();
        }

        #endregion
    }
}
