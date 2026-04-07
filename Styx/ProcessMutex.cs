using System;
using System.Threading;

namespace Styx
{
    /// <summary>
    /// Creates named mutexes to claim ownership of a WoW process.
    /// Prevents multiple CopilotBuddy instances from attaching to the same WoW.
    /// Deobfuscated from HB 4.3.4 ns18.Class21.
    /// </summary>
    internal static class ProcessMutex
    {
        /// <summary>
        /// Attempts to create and acquire a named mutex for the given WoW process ID.
        /// If <paramref name="createdNew"/> is true, this instance now owns the mutex
        /// (i.e. no other CopilotBuddy is attached to that PID).
        /// If false, another instance already claimed it.
        /// </summary>
        /// <param name="processId">The WoW process ID to claim.</param>
        /// <param name="createdNew">True if this call created (and owns) the mutex; false if already held.</param>
        /// <returns>The Mutex handle. Caller must Close/Dispose when done.</returns>
        public static Mutex Create(int processId, out bool createdNew)
        {
            // HB 4.3.4: "Local\\" + (MachineName.GetHashCode() + pid.GetHashCode() + 25)
            // HB 6.2.3 uses XOR + salt — more collision resistant. We use the 6.2.3 pattern.
            string name = "Local\\" + ((long)(Environment.MachineName.GetHashCode()
                ^ processId.GetHashCode()
                ^ TimeZoneInfo.Local.StandardName.GetHashCode())
                ^ ((long)"CopilotBuddy".GetHashCode() + 2922027367L));

            return new Mutex(true, name, out createdNew);
        }
    }
}
