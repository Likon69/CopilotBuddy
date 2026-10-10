using System.Runtime.InteropServices;

namespace GreenMagic.Native
{
    /// <summary>
    /// MEMORY_BASIC_INFORMATION for 64-bit target processes.
    ///
    /// On a 64-bit host the kernel always fills the 64-bit layout
    /// (48-byte struct) when VirtualQueryEx is called — regardless of
    /// whether the target is 32- or 64-bit — so the existing 32-bit
    /// <see cref="MEMORY_BASIC_INFORMATION"/> (28-byte layout with int
    /// fields) is only correct when the *caller* is a 32-bit process.
    /// Since CopilotBuddy can now build as x64, any VirtualQueryEx call
    /// made from the x64 build MUST use this struct.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORY_BASIC_INFORMATION64
    {
        public ulong BaseAddress;
        public ulong AllocationBase;
        public uint AllocationProtect;
        public uint __alignment1;
        public ulong RegionSize;
        public uint State;
        public uint Protect;
        public uint Type;
        public uint __alignment2;
    }
}
