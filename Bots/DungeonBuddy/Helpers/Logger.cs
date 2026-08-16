using System.Drawing;
using Styx.Helpers;

namespace Bots.DungeonBuddy.Helpers
{
    public static class Logger
    {
        public static void Write(Color color, string format, params object[] args)
        {
            Logging.Write(color, "[DungeonBuddy]: " + string.Format(format, args));
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
            Logging.WriteDebug(Color.Orange, "[DungeonBuddy-DEBUG]: " + message);
        }

        public static void WriteDebug(string format, params object[] args)
        {
            WriteDebug(string.Format(format, args));
        }

        public static void WriteError(Error error)
        {
            if (error == null)
                return;

            switch (error.Type)
            {
                case ErrorType.Error:
                    Logging.WriteError("[DungeonBuddy-Error]: " + error);
                    break;
                case ErrorType.Warning:
                    Logging.Write("[DungeonBuddy-Warning]: {0}", error);
                    break;
            }
        }
    }
}
