using System;
using System.Drawing;
using Bots.BGBuddy.Resources;
using Styx.Helpers;

namespace Bots.BGBuddy.Helpers
{
    /// <summary>
    /// BGBuddy-specific logger that prefixes all messages with [BGBuddy].
    /// </summary>
    public static class Logger
    {
        public static void Write(Color color, string format, params object[] args)
        {
            Logging.Write(color, BGBuddyResources.LogPrefix + string.Format(format, args));
        }

        public static void Write(string format, params object[] args)
        {
            Write(Color.PowderBlue, string.Format(format, args));
        }

        public static void Write(string message)
        {
            Write(Color.PowderBlue, message);
        }

        public static void WriteDebug(string message)
        {
            Logging.WriteDebug(Color.Orange, BGBuddyResources.DebugLogPrefix + message);
        }

        public static void WriteDebug(string format, params object[] args)
        {
            WriteDebug(string.Format(format, args));
        }
    }
}
