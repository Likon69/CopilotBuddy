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
using System.Windows;

using BuddyControlPanel.Resources.Localization;
using Styx;
using Styx.CommonBot;
using Styx.Logic.BehaviorTree;
using Styx.Plugins;
using Styx.Plugins.PluginClass;

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantLambdaSignatureParentheses
#endregion


// TODO: Separate out fundamental types (WpfKeyWithModifier)
// TODO: Autoscaling -> Controls and Font gets bigger as window gets smaller?
// TODO: Configuration template?
// TODO: Vector-based artwork to eliminate number of resources needed?

namespace BuddyControlPanel
{
    public class BuddyControlPanelPlugin : HBPlugin, IDisposable
    {
        #region Creation & Destruction
        public BuddyControlPanelPlugin()
        {
            // TODO: Infralution.Localization.Wpf.CultureManager.UICultureChanged += CultureManagerOnUiCultureChanged;

            // empty
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
                // empty, for now
            }
        }
        #endregion

        // These elements are in a 'global' place, because its needed to tag things like log messages
        // and maintenance exceptions.
        public override string Name { get { return BCPGlobalization.PluginName; } }
        public override string Author { get { return Assets.PluginAuthor; } }
        public override Version Version { get { return Assets.PluginVersion; } }

        public override void OnDisable()
        {
            Utility.InvokeOnSpecificDispatcher(Application.Current.Dispatcher, () =>
            {
                KeybindManager.ToJunkyard();
                Overlay_ControlBar.ToJunkyard();
                Overlay_StatusDisplay.ToJunkyard();

                BuddyControlPanelSettings.ToJunkyard();
            });
        }

        public override void OnEnable()
        {
            Utility.InvokeOnSpecificDispatcher(Application.Current.Dispatcher, () =>
            {
                if (!StyxWoW.Overlay.IsActive || !StyxWoW.Overlay.IsDesktopCompositionEnabled)
                {
                    PluginLog.Warning(BCPGlobalization.GeneralTextFormat_OverlayIsNotActive,
                                        Environment.NewLine, BCPGlobalization.PluginName);
                    return;
                }

                // Make certain we've got active instances of key components...
                var buddyControlPanelSettings = BuddyControlPanelSettings.Instance;

                var overlayControlBar = Overlay_ControlBar.Instance;
                var overlayStatusDisplay = Overlay_StatusDisplay.Instance;
                var hotkeysDirector = KeybindManager.Instance;

                // Align controls state with existing Honorbuddy state...
                NotifyBotChange(BotManager.Current);
                NotifyStateChange(new CurrentBotStateNotification() { TreeRootState = TreeRoot.State });
            });
        }


        public override void Pulse()
        {
            // empty
        }

        public static void NotifyBotChange(BotBase newBot)
        {
            Overlay_ControlBar.Instance.NotifyBotChanged(newBot);
        }

        public static void NotifyStateChange(CurrentBotStateNotification currentBotStateNotification)
        {
            Overlay_ControlBar.Instance.NotifyStateChanged(currentBotStateNotification);
        }
    }
}