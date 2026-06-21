using System.Timers;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Windows.Media;

namespace Tyrael.Shared
{
    public class TyraelUtilities
    {
        #region Click-To-Move (CTM) Functions
        /// <summary>
        /// This runs the RunClickToMove() void after a set time.
        /// </summary>
        /// <param name="tickingtime">Ticking time in MS.</param>
        public static void ClickToMove(int tickingtime)
        {
            _tyraeltimer = new Timer(tickingtime);
            _tyraeltimer.Elapsed += OnTimedEvent;
            _tyraeltimer.AutoReset = false;
            _tyraeltimer.Enabled = true;
        }

        private static Timer _tyraeltimer;

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            RunClickToMove();
        }

        /// <summary>
        /// Enables or Disables CTM when this void is executed.
        /// </summary>
        internal static void RunClickToMove()
        {
            Lua.DoString(!TyraelSettings.Instance.ClickToMove
                ? "SetCVar('autoInteract', '0')"
                : "SetCVar('autoInteract', '1')");
        }

        /// <summary>
        /// Disables CTM when this void is executed.
        /// </summary>
        internal static void DisableClickToMove()
        {
            Lua.DoString("SetCVar('autoInteract', '0')");
        }

        /// <summary>
        /// Enables CTM when this void is executed.
        /// </summary>
        internal static void EnableClickToMove()
        {
            Lua.DoString("SetCVar('autoInteract', '1')");
        }
        #endregion

        #region Hotkey Functions
        /// <summary>
        /// Global bool to see if Tyrael's Paused or not.
        /// </summary>
        public static bool IsTyraelPaused { get; set; }

        /// <summary>
        /// Used for registering hotkeys within Honorbuddy using the HB API.
        /// </summary>
        internal static void RegisterHotkeys()
        {
            HotkeysManager.Register("Tyrael Pause", TyraelSettings.Instance.PauseKeyChoice, TyraelSettings.Instance.ModKeyChoice, hk =>
            {
                IsTyraelPaused = !IsTyraelPaused;
                if (IsTyraelPaused)
                {
                    if (TyraelSettings.Instance.ChatOutput)
                    {
                        Lua.DoString(@"print('[Tyrael] Rotation \124cFFE61515 Paused!')");
                    }

                    if (TyraelSettings.Instance.RaidWarningOutput)
                    {
                        Lua.DoString("RaidNotice_AddMessage(RaidWarningFrame, \"[Tyrael] Rotation Paused!\", ChatTypeInfo[\"RAID_WARNING\"]);");
                    }

                    Logging.Write(Colors.Red, "[Tyrael] Rotation Paused!");
                    TreeRoot.TicksPerSecond = CharacterSettings.Instance.TicksPerSecond; Tyrael.IsPaused = true;
                }
                else
                {
                    if (TyraelSettings.Instance.ChatOutput)
                    {
                        Lua.DoString(@"print('[Tyrael] Rotation \124cFF15E61C Resumed!')");
                    }

                    if (TyraelSettings.Instance.RaidWarningOutput)
                    {
                        Lua.DoString("RaidNotice_AddMessage(RaidWarningFrame, \"[Tyrael] Rotation Resumed!\", ChatTypeInfo[\"GUILD\"]);");
                    }

                    Logging.Write(Colors.LimeGreen, "[Tyrael] Rotation Resumed!");
                    TreeRoot.TicksPerSecond = CharacterSettings.Instance.TicksPerSecond; Tyrael.IsPaused = false;
                }
            });
        }

        /// <summary>
        /// Used for unregistering hotkeys within Honorbuddy using the HB API.
        /// </summary>
        internal static void RemoveHotkeys()
        {
            HotkeysManager.Unregister("Tyrael Pause");
        }

        /// <summary>
        /// Used for reregistering hotkeys within Honorbuddy using the HB API.
        /// </summary>
        internal static void ReRegisterHotkeys()
        {
            RemoveHotkeys();
            RegisterHotkeys();
        }
        #endregion

        #region Logging Functions
        internal static void DiagnosticLog(string message, params object[] args)
        {
            Write(LogLevel.Diagnostic, Colors.MediumPurple, message, args);
        }

        internal static void PrintLog(string message, params object[] args)
        {
            Write(LogLevel.Normal, ColorException(message) ? Colors.White : Colors.DodgerBlue, message, args);
        }

        internal static void WriteToFileLog(string message, params object[] args)
        {
            Logging.WriteToFileSync(LogLevel.Verbose, "[Tyrael] " + message, args);
        }

        /// <summary>
        /// Main void for logging purposes.
        /// </summary>
        /// <param name="level">Define the loglevel (Eg: LogLevel.Diagnostic).</param>
        /// <param name="specificcolor">Define the color (Eg: Colors.OrangeRed).</param>
        /// <param name="message">The actual message to print.</param>
        /// <param name="args">Arguments to fill parameters.</param>
        private static void Write(LogLevel level, Color specificcolor, string message, params object[] args)
        {
            Logging.Write(level, specificcolor, string.Format("[Tyrael] {0}", message), args);
        }

        private static bool ColorException(string message)
        {
            return message.Contains("------------------------------------------");
        }

        /// <summary>
        /// Prints the Settings files to the logfile of HB.
        /// </summary>
        private static void LogSettings(string desc, Settings set)
        {
            if (set == null)
            {
                return;
            }

            WriteToFileLog("====== {0} ======", desc);
            foreach (var kvp in set.GetSettings())
            {
                WriteToFileLog("{0}: {1}", kvp.Key, kvp.Value.ToString());
            }
            WriteToFileLog("");
        }

        /// <summary>
        /// Prints required diagnostical information to the Logfile.
        /// </summary>
        internal static void PrintInformation(bool reinitialized = false)
        {
            WriteToFileLog("");
            WriteToFileLog("------------------------------------------");
            WriteToFileLog("Diagnostic Logging");
            WriteToFileLog("");
            WriteToFileLog("Tyrael Revision: {0}", Tyrael.Revision);
            WriteToFileLog("");
            WriteToFileLog("HardLock Enabled: {0}", StyxSettings.Instance.UseFrameLock);
            WriteToFileLog("Ticks per Second: {0}", CharacterSettings.Instance.TicksPerSecond);
            WriteToFileLog("");
            LogSettings("Settings", TyraelSettings.Instance);
            WriteToFileLog("");
            WriteToFileLog("------------------------------------------");
        }
        #endregion

        #region Others
        /// <summary>
        /// Is the Object viable?
        /// </summary>
        internal static bool IsViable(WoWObject wowObject)
        {
            return (wowObject != null) && wowObject.IsValid;
        }

        #endregion
    }
}
