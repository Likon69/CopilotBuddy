// Originally contributed by Chinajade.
//
// LICENSE:
// This work is licensed under the
//     Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
// also known as CC-BY-NC-SA.  To view a copy of this license, visit
//      http://creativecommons.org/licenses/by-nc-sa/3.0/
// or send a letter to
//      Creative Commons // 171 Second Street, Suite 300 // San Francisco, California, 94105, USA.

#region Usings

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Profiles;
using TreeSharp;

using Action = TreeSharp.Action;
#endregion


namespace BuddyControlPanel
{
    public class PluginLog
    {
        // 30Apr2013-06:20UTC chinajade
        public static string BuildLogMessage(string messageType, string format, params object[] args)
        {
            return string.Format("[{0}-v{1}({2})] {3}",
                Assets.PluginInternalName,
                Assets.PluginVersion,
                messageType,
                string.Format(format, args));
        }


        /// <summary>
        /// <para>For DEBUG USE ONLY--don't use in production code!</para>
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Debug(string format, params object[] args)
        {
            Logging.Write(Colors.Fuchsia, BuildLogMessage("debug", format, args));
        }


        /// <summary>
        /// <para>For chasing longer-term (i.e., sporadic) issues.  These messages are only emitted to the log--not the scrolly window,
        /// and are acceptable to leave in production code.</para>
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void DeveloperInfo(string format, params object[] args)
        {
            Logging.WriteDiagnostic(Colors.LimeGreen, BuildLogMessage("debug", format, args));
        }


        /// <summary>
        /// <para>Error situations occur when bad data/input is provided, and no corrective actions can be taken.</para>
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Error(string format, params object[] args)
        {
            Logging.Write(Colors.Red, BuildLogMessage("error", format, args));
        }


        /// <summary>
        /// <para>Exception situations occur when bad data/input is provided, and no corrective actions can be taken.</para>
        /// </summary>
        /// <param name="except"></param>
        /// <param name="formatForPrefix"></param>
        /// <param name="argsForPrefix"></param>
        public static void Exception(Exception except, string formatForPrefix = null, params object[] argsForPrefix)
        {
            var messagePrefix =
                (formatForPrefix == null)
                ? "MAINTENANCE PROBLEM"
                : string.Format(formatForPrefix, argsForPrefix);

            Error("[{1}]: {2}{0}EXCEPTION ({3}):{0}{4}{0}FROM HERE:{0}{5}{0}",
                Environment.NewLine,
                messagePrefix,
                except.Message,
                except.GetType().Name,
                except.ToString(),
                except.StackTrace);
        }


        /// <summary>
        /// <para>Error situations occur when bad data/input is provided, and no corrective actions can be taken.</para>
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Fatal(string format, params object[] args)
        {
            Logging.Write(Colors.Red, BuildLogMessage("fatal", format, args));
            TreeRoot.Stop("Fatal error in quest behavior, or profile.");
        }


        /// <summary>
        /// <para>Normal information to keep user informed.</para>
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Info(string format, params object[] args)
        {
            Logging.Write(Colors.CornflowerBlue, BuildLogMessage("info", format, args));
        }


        /// <summary>
        /// MaintenanceErrors occur as a result of incorrect code maintenance.  There is usually no corrective
        /// action a user can perform in the field for these types of errors.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        ///  30Jun2012-15:58UTC chinajade
        public static void MaintenanceError(string format, params object[] args)
        {
            string formattedMessage = string.Format(format, args);
            var trace = new StackTrace(1);

            Error("[MAINTENANCE ERROR] {0}\nFROM HERE:\n{1}", formattedMessage, trace.ToString());
        }


        //  5May2013-09:04UTC chinajade
        public static Composite MarkerPS(Contract.ProvideStringDelegate messageDelegate)
        {
            return new Action(context =>
            {
                Logging.Write(Colors.Fuchsia, BuildLogMessage("marker", messageDelegate()));
                return RunStatus.Failure;
            });
        }


        //  26May2013-09:04UTC chinajade
        public static Composite MarkerSeq(Contract.ProvideStringDelegate messageDelegate)
        {
            return new Action(context =>
            {
                Logging.Write(Colors.Fuchsia, BuildLogMessage("marker", messageDelegate()));
            });
        }


        /// <summary>
        /// <para>Used to notify of problems where corrective (fallback) actions are possible.</para>
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Warning(string format, params object[] args)
        {
            Logging.Write(Colors.DarkOrange, BuildLogMessage("warning", format, args));
        }
    }
}