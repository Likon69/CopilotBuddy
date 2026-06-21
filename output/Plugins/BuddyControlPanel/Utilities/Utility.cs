// Originally contributed by Chinajade
//
// LICENSE:
// This work is licensed under the
//     Creative Commons Attribution-NonCommercial-ShareAlike 4.0 Unported License.
// also known as CC-BY-NC-SA.  To view a copy of this license, visit
//      http://creativecommons.org/licenses/by-nc-sa/4.0/
// or send a letter to
//      Creative Commons // 171 Second Street, Suite 300 // San Francisco, California, 94105, USA.
//

#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;

using BuddyControlPanel.Resources.Localization;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.Localization;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Profiles;
using Styx.Logic.Pathing;
using Styx.Plugins;
using Styx.WoWInternals;
using Styx.WoWInternals.DBC;
using Styx.WoWInternals.World;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    // By its nature, only static elements belong in the Utility class...
    public static class Utility
    {
        /// <summary>
        /// returns IDs of maps that are active on current continent, 
        /// returning current mapId and parent mapId before other phased mapIds
        /// </summary>
        // N.B.: Code taken from DungeonBuddy.  Thanks, Highvoltz!
        public static IEnumerable<uint> ActiveMapIds
        {
            get
            {
                var currentMap = StyxWoW.Me.CurrentMap;
                yield return currentMap.MapId;

                if (currentMap.ParentMapId != -1)
                    yield return (uint)currentMap.ParentMapId;

                foreach (var map in StyxWoW.WorldScene.WorldMap.GetMaps())
                {
                    if (!map.IsActive || map.MapID == currentMap.MapId || map.MapID == currentMap.ParentMapId)
                        continue;
                    yield return (uint)map.MapID;
                }
            }
        }


        public static XAttribute AttributeIgnoreCase(this XElement rootElement, XName name)
        {
            var xAttribute = rootElement.Attribute(name);

            if (xAttribute != null)
                return xAttribute;

            var nameLowerCase = name.ToString().ToLowerInvariant();

            return
                rootElement.Attributes()
                .FirstOrDefault(e => e.Name.LocalName.ToString().ToLowerInvariant() == nameLowerCase);
        }


        /// <summary>
        /// Allows ACTION to be asynchronously executed on the specified DISPATCHER.  This is useful for performing
        /// activities that may be locked to particular dispatchers--such as the thread enforcement
        /// performed by WPF controls.
        /// The method is written to avoid doing thread switches, if possible.
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="action"></param>
        public static void BeginInvokeOnSpecificDispatcher(Dispatcher dispatcher, Action action)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
                return;
            }

            action();
        }


        public static string BotName_QuestBot
        {
            // We pull the bot names out of the 'main' Globalization file, to reduce language maintenance...
            get { return Globalization.QuestBot_Name; }
        }


        public static string BotName_GrindBot
        {
            // We pull the bot names out of the 'main' Globalization file, to reduce language maintenance...
            get { return Globalization.LevelBot_Name; }
        }


        public static void BuddyBotExit()
        {
            TreeRoot.Stop();
            // Post shutdown to the UI dispatcher at Background priority so the dialog
            // and OnClick call stack can unwind fully before Application.Shutdown() fires.
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => Application.Current.Shutdown()));
            // Hard-kill fallback: if graceful shutdown stalls for any reason,
            // terminate the process after 2 seconds regardless.
            ThreadPool.QueueUserWorkItem(o =>
            {
                Thread.Sleep(2000);
                Environment.Exit(0);
            });
        }


        public static void BuddyBotStop()
        {
            TreeRoot.Stop();
        }


        public static XElement ElementIgnoreCase(this XElement rootElement, XName name)
        {
            var xElement = rootElement.Element(name);

            if (xElement != null)
                return xElement;

            var nameLowerCase = name.ToString().ToLowerInvariant();

            return
                rootElement.Elements()
                .FirstOrDefault(e => e.Name.LocalName.ToString().ToLowerInvariant() == nameLowerCase);
        }


        public static BotBase FindBotBase<T>()
        {
            return BotManager.Instance.Bots.Values.FirstOrDefault(botBase => botBase is T);
        }


        /// <summary>
        /// Convenience method to locate a WPF control 'by name'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controlName"></param>
        /// <param name="notFoundIsFatal"></param>
        /// <returns></returns>
        public static T FindUiControlByName<T>(string controlName, bool notFoundIsFatal = true)
        {
            T control = default(T);
            InvokeOnSpecificDispatcher(Application.Current.Dispatcher,
                () =>
                {
                    // NB: If Honorbuddy main window renames the control or changes its type,
                    // this application will need adjustment also...
                    control = (T)Application.Current.MainWindow.FindName(controlName);
                    if (control == null)
                    {
                        var message = String.Format("Unable to locate \"{0}\" control", controlName);
                        PluginLog.MaintenanceError(message);
                        if (notFoundIsFatal)
                            throw new ArgumentException(message);
                    }
                });

            return control;
        }


        public static void GameClientExit()
        {
            // Otherwise, schedule a game-client shutdown as a background thread...
            ThreadPool.QueueUserWorkItem(o =>
            {
                // Give some time for things to settle, before we terminate game client...
                Thread.Sleep(500);
                TreeRoot.Shutdown(HonorbuddyExitCode.Default, true);
            });
            TreeRoot.Stop();
        }


        public static void GameClientLogout()
        {
            // Otherwise, schedule a game-client shutdown as a background thread...
            ThreadPool.QueueUserWorkItem(o =>
            {
                // Give some time for things to settle, before we terminate game client...
                Thread.Sleep(500);
                Lua.DoString("Logout()");
                TreeRoot.Shutdown();
            });
            TreeRoot.Stop();
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


        public static string GetCurrentProfileName()
        {
            return
                ((ProfileManager.CurrentOuterProfile != null)
                 && !string.IsNullOrEmpty(ProfileManager.CurrentOuterProfile.Name))
                    ? ProfileManager.CurrentOuterProfile.Name
                    : "None";
        }

        /// <summary>
        /// Creates a collection of enumerable type T.
        /// This is occasionally useful for use in foreach statements, or LINQ expressions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }


        /// <summary>
        /// Returns the location where a new window or popup should be placed relative to the FRAMEWORKELEMENT.
        /// </summary>
        /// <param name="frameworkElement"></param>
        /// <returns></returns>
        public static Point GetStartLocation(FrameworkElement frameworkElement)
        {
            return frameworkElement.PointToScreen(new Point(0, 0));
        }

        /// <summary>
        /// Allows ACTION to be synchronously executed on the specified DISPATCHER.  This is useful for performing
        /// activities that may be locked to particular dispatchers--such as the thread enforcement
        /// performed by WPF controls.
        /// The method is written to avoid doing thread switches, if possible.
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="action"></param>
        public static void InvokeOnSpecificDispatcher(Dispatcher dispatcher, Action action)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke(DispatcherPriority.Normal, action);
                return;
            }

            action();
        }

        public static bool IsQuestProfile(Profile profile)
        {
            return (profile != null) && (profile.QuestOrder != null) && (profile.QuestOrder.Count > 0);
        }


        public static bool IsVendorsAccessible()
        {
            return !StyxWoW.Me.IsInInstance || StyxWoW.Me.CurrentMap.IsGarrison;
        }


        /// <summary>
        /// Sends the MESSAGE to the Overlay "Toast" facilities in the specified COLOR.
        /// The toast message will remain on the game client screen for TOASTDURATION.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="toastDuration">if omitted, this value defaults to 2000 milliseconds</param>
        public static void OverlayNotification(string message, Color color, TimeSpan? toastDuration = null)
        {
            // Make certain any actions we take happen on the 'main thread' (HB API is not MT-safe)...
            InvokeOnSpecificDispatcher(Application.Current.Dispatcher,
                () => StyxWoW.Overlay.AddToast(() => message,
                                                (toastDuration ?? TimeSpan.FromMilliseconds(2000)),
                                                color,
                                                Colors.Black,
                                                Assets.OverlayFontFamily));
        }

        public static ImageSource ToImageSource(string imageName, int? specificSize = null)
        {
            var resourceSubdirectory =
                (specificSize != null)
                    ? String.Format("Images{0}", specificSize)
                    : "Images";

            Contract.Requires(!String.IsNullOrEmpty(imageName), () => "ImageName may not be null or empty");

            var fullPath = Path.Combine(PluginManager.PluginsDirectory,
                                        Assets.PluginInternalName,
                                        "Resources",
                                        resourceSubdirectory,
                                        imageName);

            try
            {
                var bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(fullPath, UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();

                return bitmapImage;
            }
            catch (Exception ex)
            {
                var message = String.Format(BCPGlobalization.GeneralText_ProblemEncounteredTryingToObtain + " {0}", imageName);
                PluginLog.MaintenanceError(message);
                throw new Exception(message, ex);
            }
        }


        public static ImageSource ToImageSource(this Icon icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}