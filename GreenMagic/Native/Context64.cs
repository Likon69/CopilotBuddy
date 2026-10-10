using System.Runtime.InteropServices;

namespace GreenMagic.Native
{
    /// <summary>
    /// 128-bit XMM/AVT register used by the AMD64 CONTEXT structure.
    /// Must be 16-byte aligned — matches the native M128A layout.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public struct M128A
    {
        public ulong Low;
        public long High;
    }

    /// <summary>
    /// AMD64 (x64) thread CONTEXT structure — exact byte-for-byte layout of
    /// the native Win32 CONTEXT for x64 (winnt.h). sizeof(CONTEXT) = 1232
    /// (0x4D0). Field offsets:
    ///
    ///   0x000  P1Home..P6Home      (parameter home space, 6 x DWORD64)
    ///   0x030  ContextFlags (DWORD), 0x034 MxCsr (DWORD)
    ///   0x038  SegCs/SegDs/SegEs/SegFs/SegGs/SegSs (WORD each)
    ///   0x044  EFlags (DWORD)
    ///   0x048  Dr0..Dr7            (6 x DWORD64)
    ///   0x078  Rax..R15            (16 x DWORD64, Rax..Rdi then R8..R15)
    ///   0x0F8  Rip                 (instruction pointer — the x64 EIP)
    ///   0x100  FltSave             (XMM_SAVE_AREA32, 512 bytes)
    ///   0x300  VectorRegister[26]  (26 x M128A = 416 bytes)
    ///   0x4A0  VectorControl (DWORD64)
    ///   0x4A8  DebugControl, LastBranchToRip, LastBranchFromRip,
    ///          LastExceptionToRip, LastExceptionFromRip (5 x DWORD64)
    ///   0x4D0  end
    ///
    /// The 32-bit <see cref="Context"/> struct in Context.cs mirrors the
    /// i386 CONTEXT (871 bytes). Windows rejects a GetThreadContext /
    /// SetThreadContext call when the caller's struct does not match the
    /// bitness of the target thread, so when attaching to a 64-bit WoW
    /// process this struct MUST be used instead of <see cref="Context"/>.
    ///
    /// The x87/SSE state (FltSave + VectorRegister) is carried as opaque
    /// bytes: the Executor only needs to save the context, redirect Rip
    /// and restore everything untouched, and that keeps the layout robust
    /// against future .NET marshalling changes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public struct Context64
    {
        // Register parameter home addresses (P1Home..P6Home)
        public ulong P1Home;
        public ulong P2Home;
        public ulong P3Home;
        public ulong P4Home;
        public ulong P5Home;
        public ulong P6Home;

        /// <summary>Register mask describing which fields are valid (ContextFlags64).</summary>
        public uint ContextFlags;

        /// <summary>MXCSR control/status register.</summary>
        public uint MxCsr;

        // Segment registers
        public ushort SegCs;
        public ushort SegDs;
        public ushort SegEs;
        public ushort SegFs;
        public ushort SegGs;
        public ushort SegSs;

        /// <summary>CPU flags register.</summary>
        public uint EFlags;

        // Debug registers
        public ulong Dr0;
        public ulong Dr1;
        public ulong Dr2;
        public ulong Dr3;
        public ulong Dr6;
        public ulong Dr7;

        // General purpose registers (native order: Rax..Rdi, then R8..R15)
        public ulong Rax;
        public ulong Rcx;
        public ulong Rdx;
        public ulong Rbx;
        public ulong Rsp;
        public ulong Rbp;
        public ulong Rsi;
        public ulong Rdi;
        public ulong R8;
        public ulong R9;
        public ulong R10;
        public ulong R11;
        public ulong R12;
        public ulong R13;
        public ulong R14;
        public ulong R15;

        /// <summary>Instruction pointer. The x64 equivalent of EIP.</summary>
        public ulong Rip;

        // XMM_SAVE_AREA32 (x87 + SSE state), kept opaque — 512 bytes.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] FltSave;

        // AVT/extended vector registers (26 x M128A).
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
        public M128A[] VectorRegister;

        public ulong VectorControl;

        // Special debug control registers.
        public ulong DebugControl;
        public ulong LastBranchToRip;
        public ulong LastBranchFromRip;
        public ulong LastExceptionToRip;
        public ulong LastExceptionFromRip;
    }

    /// <summary>
    /// CONTEXT flags for the AMD64 architecture (CONTEXT_AMD64 = 0x100000).
    /// The i386 equivalents used for 32-bit targets live in
    /// <see cref="ContextFlags"/> (CONTEXT_i386 = 0x10000).
    /// </summary>
    public static class ContextFlags64
    {
        private const ulong CONTEXT_AMD64 = 0x100000UL;

        public const ulong CONTEXT_CONTROL = CONTEXT_AMD64 | 0x01UL;           // Rip, Rsp, SegSs, EFlags
        public const ulong CONTEXT_INTEGER = CONTEXT_AMD64 | 0x02UL;           // Rax..R15
        public const ulong CONTEXT_SEGMENTS = CONTEXT_AMD64 | 0x04UL;          // SegCs..SegGs
        public const ulong CONTEXT_FLOATING_POINT = CONTEXT_AMD64 | 0x08UL;    // Xmm/FltSave
        public const ulong CONTEXT_DEBUG_REGISTERS = CONTEXT_AMD64 | 0x10UL;   // Dr0..Dr7

        public const ulong CONTEXT_FULL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_FLOATING_POINT;
        public const ulong CONTEXT_ALL = CONTEXT_FULL | CONTEXT_SEGMENTS | CONTEXT_DEBUG_REGISTERS;
    }
}
