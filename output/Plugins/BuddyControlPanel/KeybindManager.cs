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

using System;
using System.Windows.Forms;
using System.Windows.Input;

using BuddyControlPanel.Resources.Localization;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;

namespace BuddyControlPanel
{
    internal class KeybindManager : IDisposable
    {
        #region Creation & Destruction
        private KeybindManager(BuddyControlPanelSettings.KeybindSettings configuration)
        {
            Configuration_Set(configuration);
        }

        // Basic Dispose pattern (ref: https://msdn.microsoft.com/en-us/library/b1yfkh5e%28v=vs.110%29.aspx)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                HotkeysManager.Unregister(_keyName_StartStop);
                HotkeysManager.Unregister(_keyName_PauseResume);
            }
        }

        private static KeybindManager s_instance;
        public static KeybindManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    var config = BuddyControlPanelSettings.Instance.KeybindConfiguration
                                 ?? Configuration_GetDefaults();

                    s_instance = new KeybindManager(config);
                }
                return s_instance;
            }
        }

        /// <summary>
        /// De-initializes the singleton instance.
        /// </summary>
        public static void ToJunkyard()
        {
            if (s_instance != null)
            {
                s_instance.Dispose();
                s_instance = null;
            }
        }
        #endregion


        private BuddyControlPanelSettings.WpfKeyWithModifiers _key_PauseResume;
        private BuddyControlPanelSettings.WpfKeyWithModifiers _key_StartStop;
        private readonly string _keyName_PauseResume = string.Format("{0}_PauseUnpause", Assets.PluginInternalName);
        private readonly string _keyName_StartStop = string.Format("{0}_StartStop", Assets.PluginInternalName);


        #region Configuration
        public BuddyControlPanelSettings.KeybindSettings Configuration_Get()
        {
            return new BuddyControlPanelSettings.KeybindSettings()
            {
                KeyPauseResume = _key_PauseResume,
                KeyStartStop = _key_StartStop,
            };
        }


        // N.B. Configuration_GetDefaults() must always be declared static.
        // This method is used to initialize instances (or the singleton), and we can't require
        // a valid instance of the object in order to determine the default values.
        public static BuddyControlPanelSettings.KeybindSettings Configuration_GetDefaults()
        {
            return new BuddyControlPanelSettings.KeybindSettings()
            {
                KeyPauseResume = new BuddyControlPanelSettings.WpfKeyWithModifiers() { Key = Key.None },
                KeyStartStop = new BuddyControlPanelSettings.WpfKeyWithModifiers() { Key = Key.None },
            };
        }


        public void Configuration_Set(BuddyControlPanelSettings.KeybindSettings configuration)
        {
            Contract.Requires(configuration != null, () => "configuration may not be null");

            HotkeysManager.Unregister(_keyName_PauseResume);
            if (configuration.KeyPauseResume.Key != Key.None)
            {
                _key_PauseResume = configuration.KeyPauseResume;
                RegisterHotkeyAssignment(_keyName_PauseResume, configuration.KeyPauseResume, PauseUnpause);
            }

            HotkeysManager.Unregister(_keyName_StartStop);
            if (configuration.KeyStartStop.Key != Key.None)
            {
                _key_StartStop = configuration.KeyStartStop;
                RegisterHotkeyAssignment(_keyName_StartStop, configuration.KeyStartStop, StopStart);
            }
        }

        #endregion


        // Taken from Singular. Thanks, Bobby53!
        private static void RegisterHotkeyAssignment(string name, BuddyControlPanelSettings.WpfKeyWithModifiers wpfKey, Action<Hotkey> callback)
        {
            var formsKey = wpfKey.FormsKey;
            var keyCode = formsKey & Keys.KeyCode;
            var mods = Styx.Common.ModifierKeys.NoRepeat;

            if (wpfKey.ModifierAlt)
                mods |= Styx.Common.ModifierKeys.Alt;
            if (wpfKey.ModifierControl)
                mods |= Styx.Common.ModifierKeys.Control;
            if (wpfKey.ModifierShift)
                mods |= Styx.Common.ModifierKeys.Shift;

            HotkeysManager.Register(name, keyCode, mods, callback);
        }


        private void StopStart(Hotkey hotkey)
        {
            if (TreeRoot.IsRunning)
                TreeRoot.Stop();
            else
            {
                // Capture any messages that may result from a failed bot launch...
                var finalPhrase = BCPGlobalization.GeneralText_FinalLogPhrase;
                var logWatcher = new LogWatcher(finalPhrase);

                TreeRoot.Start();

                // If start was successful, we don't want to emit any log entries...
                if (TreeRoot.IsRunning)
                    logWatcher.Dispose();

                // On a failed start, tell the user what went' wrong...
                else
                {
                    BuddyControlPanelPlugin.NotifyStateChange(new CurrentBotStateNotification() { TreeRootState = TreeRootState.Stopped });
                    // Use normal logging here...
                    // These messages will be reflected as 'toasts' on the game-client screen. Our PluginLog facilities
                    // will adorn the messages with the plugin name and version, and we don't want that for toast messages.
                    Logging.Write(Assets.ColorProblem,
                        BCPGlobalization.GeneralText_FailedToStartBot,
                        BCPGlobalization.BuddyBotName,
                        BotManager.Current.Name);
                    Logging.Write(Assets.ColorProblem, finalPhrase);
                }
            }
        }


        private void PauseUnpause(Hotkey hotkey)
        {
            if (TreeRoot.IsRunning)
            {
                if (TreeRoot.IsPaused)
                    TreeRoot.Resume();
                else
                    TreeRoot.Pause();
            }

            else
            {
                Utility.OverlayNotification(string.Format(BCPGlobalization.GeneralText_PauseHotkeyIgnoredBotIsNotRunning,
                                            BCPGlobalization.BuddyBotName),
                    Assets.ColorProblem);
            }
        }
    }
}
