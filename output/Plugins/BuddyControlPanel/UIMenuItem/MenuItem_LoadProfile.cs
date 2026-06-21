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
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using Microsoft.Win32;

using BuddyControlPanel.Resources.Localization;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.Logic.Profiles;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.WoWInternals;

using MenuItem = System.Windows.Controls.MenuItem;

// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    internal class MenuItem_LoadProfile : MenuItemBase
    {
        public MenuItem_LoadProfile()
            : base(BCPGlobalization.Item_LoadProfile_Label, null)
        {
            _menuItem_FromBuddyStore = new MenuItem_FromBuddyStore();
            _menuItem_FromFileSystem = new MenuItem_FromFileSystem();
            _menuItem_FromWebSource = new MenuItem_FromWebSource();
            _separator = new Assets.ThemedSeparator();

            RebuildSubMenu();
        }

        private readonly MenuItem _menuItem_FromBuddyStore;
        private readonly MenuItem _menuItem_FromFileSystem;
        private readonly MenuItem _menuItem_FromWebSource;
        private readonly Separator _separator;


        protected override void OnSubmenuOpened(RoutedEventArgs evt)
        {
            RebuildSubMenu();
            base.OnSubmenuOpened(evt);
        }

        public override void NotifyStateChanged(CurrentBotStateNotification currentBotStateNotification)
        {
            HandleNotifyOfStateChange(currentBotStateNotification);
        }

        private void RebuildSubMenu()
        {
            // Rebuid the base menu...
            Items.Clear();
            Items.Add(_menuItem_FromFileSystem);
            Items.Add(_menuItem_FromBuddyStore);
            Items.Add(_menuItem_FromWebSource);

            if (CharacterSettings.Instance.RecentProfiles != null)
            {
                Items.Add(_separator);
                foreach (var recentProfile in CharacterSettings.Instance.RecentProfiles)
                {
                    if (string.IsNullOrEmpty(recentProfile))
                        continue;

                    var split = recentProfile.Split(new[] { "@@!@@" }, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length != 2)
                        continue;

                    Items.Add(new MenuItem_RecentProfile(split[0], split[1]));
                }
            }
        }


        private class MenuItem_FromBuddyStore : MenuItemBase
        {
            public MenuItem_FromBuddyStore()
                : base(BCPGlobalization.Item_LoadProfileFromBuddyStore_Label, null)
            {
                // empty
            }

            protected override void OnClick()
            {
                // BuddyStore not available in WotLK 3.3.5a.
                base.OnClick();
            }
        }


        private class MenuItem_FromFileSystem : MenuItemBase
        {
            public MenuItem_FromFileSystem()
                : base(BCPGlobalization.Item_LoadProfileFromFile_Label, null)
            {
                // empty
            }

            protected override void OnClick()
            {
                string fileName = null;

                // Synchronous operation for modal dialog...
                Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
                {
                    TreeRoot.StatusText = BCPGlobalization.GeneralText_LoadingProfile;
                    var openFileDialog = new OpenFileDialog
                    {
                        DefaultExt = ".xml",
                        Filter = "Profile XML|*.xml",
                        Title = BCPGlobalization.Title_LoadAProfile,
                    };

                    bool? dialogResult = openFileDialog.ShowDialog();
                    if (dialogResult == true)
                        fileName = openFileDialog.FileName;
                });

                // Potentially lengthy 'load profile' operation conducted in background...
                if (!string.IsNullOrEmpty(fileName))
                    LoadProfile(Dispatcher, fileName);

                base.OnClick();
            }
        }


        private class MenuItem_FromWebSource : MenuItemBase
        {
            public MenuItem_FromWebSource()
                : base(BCPGlobalization.Item_LoadProfileFromWebSource_Label, null)
            {
                // empty
            }

            protected override void OnClick()
            {
                var mousePosition = Utility.GetStartLocation(this);

                var dialog = new Dialog_LoadProfileFromWeb()
                {
                    Left = mousePosition.X,
                    Top = mousePosition.Y,
                    WindowStartupLocation = WindowStartupLocation.Manual
                };

                bool? dialogResult = dialog.ShowDialog();
                var selectedProfileName = dialog.Input;
                if ((dialogResult == true) && !string.IsNullOrWhiteSpace(selectedProfileName))
                {
                    try
                    {
                        // Fetch the profile resource...
                        var profileContents = new WebClient().DownloadString(selectedProfileName);

                        // Scribble it locally...
                        var uri = new Uri(selectedProfileName);
                        var fileName = Path.GetFileName(uri.LocalPath);
                        var filePath = Path.Combine(Utilities.AssemblyDirectory, "Cache", fileName);
                        File.WriteAllText(filePath, profileContents);

                        // Load the local resource into the profilemanager...
                        LoadProfile(Dispatcher, filePath);
                    }
                    catch (Exception)
                    {
                        var message = string.Format(BCPGlobalization.GeneralText_ThereWasAnErrorRequestingTheWebProfile + "{0}{1}",
                            Environment.NewLine, selectedProfileName);
                        Utility.OverlayNotification(message, Assets.ColorProblem, TimeSpan.FromMilliseconds(5000));
                        PluginLog.DeveloperInfo(message);
                    }
                }

                base.OnClick();
            }
        }


        private class MenuItem_RecentProfile : MenuItemBase
        {
            public MenuItem_RecentProfile(string profileName, string profileLocation)
                : base(profileName, string.Format(BCPGlobalization.Item_RecentProfile_ToolTipFormat, profileLocation))
            {
                Contract.Requires(!string.IsNullOrEmpty(profileName), () => "name may not be null or empty.");
                Contract.Requires(!string.IsNullOrEmpty(profileLocation), () => "location may not be null or empty.");

                _profileLocation = profileLocation;

                Foreground = Brushes.LightSkyBlue;
            }

            private readonly string _profileLocation;

            protected override void OnClick()
            {
                LoadProfile(Dispatcher, _profileLocation);
                base.OnClick();
            }
        }


        private static void LoadProfile(Dispatcher dispatcher, string path)
        {
            Contract.Requires(dispatcher != null, () => "dispatcher != null");
            Contract.Requires(!string.IsNullOrEmpty(path), () => "path may not be null or empty");

            BuddyControlPanelPlugin.NotifyStateChange(new CurrentBotStateNotification() { IsLoadProfileInProgress = true });
            Utility.OverlayNotification(
                string.Format("{0}: \"{1}\"", BCPGlobalization.GeneralText_LoadingProfile, Path.GetFileName(path)),
                Assets.ColorInformation,
                TimeSpan.FromMilliseconds(2500));

            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    ObjectManager.Update();
                    try
                    {
                        ProfileManager.LoadNew(path);
                        Utility.OverlayNotification(BCPGlobalization.GeneralText_ProfileLoadCompleted,
                                                    Assets.ColorInformation, TimeSpan.FromMilliseconds(2500));
                    }
                    catch (XmlException ex)
                    {
                        var message = string.Format(BCPGlobalization.GeneralText_ProfileFailedToLoad + "{0}\"{1}\"!",
                                                    Environment.NewLine, path);
                        Utility.OverlayNotification(message, Assets.ColorProblem, TimeSpan.FromMilliseconds(5000));
                        PluginLog.Error(message);
                        PluginLog.Error(ex.Message);
                    }
                    catch (ProfileNotFoundException)
                    {
                        var message = string.Format(BCPGlobalization.GeneralText_ProfileCouldNotBeFound + "{0}\"{1}\"",
                                                    Environment.NewLine, path);
                        Utility.OverlayNotification(message, Assets.ColorProblem, TimeSpan.FromMilliseconds(5000));
                        PluginLog.Error(message);
                    }
                }
                finally
                {
                    BuddyControlPanelPlugin.NotifyStateChange(new CurrentBotStateNotification()
                    {
                        TreeRootState = TreeRoot.State,
                        IsLoadProfileInProgress = false,
                    });
                }
            });
        }
    }
}