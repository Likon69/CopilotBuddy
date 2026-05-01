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
            _peHeaderParser = new PeHeaderParser(_process.MainModule.BaseAddress, this);
            _aslrEnabled = (_peHeaderParser.NtHeader.OptionalHeader.DllCharacteristics & 64) != 0;

            if (_aslrEnabled)
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

        #endregion

        #region Address Calculation

        public uint GetAbsolute(uint relative)
        {
            return relative + _imageBase;
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
            return ReadStructArray<T>(new IntPtr(address), elements);
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
            IntPtr handle = Imports.CreateRemoteThread(hProcess, IntPtr.Zero, 0U, new IntPtr(startAddress), new IntPtr(parameter), creationFlags, out tid);
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
