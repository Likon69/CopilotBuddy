using System.Runtime.InteropServices;

namespace Styx.Logic.Questing
{
	/// <summary>
	/// Quest log entry structure (20 bytes).
	/// Layout: Id(4) + State(4) + ObjectiveRequiredCounts(8) + Time(4) = 20 bytes.
	/// Read from address 12728252 (0xC23F3C).
	/// WoW 3.3.5a build 12340.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct QuestLogEntry
	{
		public int Id;
		public StateFlag State;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public short[] ObjectiveRequiredCounts;
		public int Time;

		public override string ToString()
		{
			return $"Id: {Id} State: {State}";
		}

		public enum StateFlag : uint
		{
			None = 0,
			Complete = 1,
			Failed = 2
		}
	}
}
