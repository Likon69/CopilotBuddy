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

using System.Windows.Controls;

using BuddyControlPanel.Resources.Localization;
using Styx;
using Styx.CommonBot;
using Styx.Logic.BehaviorTree;

// ReSharper disable CheckNamespace
#endregion


namespace BuddyControlPanel
{
    internal abstract class MenuItemBase : Assets.ThemedMenuItem, IBotChangeListener, IBotStateChangeListener, IProfileChangeListener
    {
        protected MenuItemBase(string menuItemHeader, string defaultToolTip = null)
        {
            Header = menuItemHeader;
            ToolTip = defaultToolTip;

            _defaultToolTip = defaultToolTip;
            ToolTipService.SetShowOnDisabled(this, true);
        }

        private readonly string _defaultToolTip;
        public string DefaultToolTip { get { return _defaultToolTip; } }

        public virtual void NotifyBotChanged(BotBase newBot) { /*empty*/ }
        public virtual void NotifyProfileChanged() { /*empty*/ }
        public virtual void NotifyStateChanged(CurrentBotStateNotification currentBotStateNotification) { /*empty*/ }

        // Convenience methods for derived classes...
        protected void HandleNotifyOfStateChange(CurrentBotStateNotification currentBotState)
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
            {
                if (currentBotState.IsLoadProfileInProgress.HasValue)
                {
                    IsEnabled = !currentBotState.IsLoadProfileInProgress.Value;
                    ToolTip = IsEnabled ? _defaultToolTip : BCPGlobalization.GeneralText_DisabledWhileProfileIsLoading;
                    if (!IsEnabled)
                        return;
                }

                if (currentBotState.TreeRootState.HasValue)
                {
                    IsEnabled = (currentBotState.TreeRootState == TreeRootState.Stopped);
                    ToolTip = IsEnabled ? _defaultToolTip : BCPGlobalization.GeneralText_DisabledWhileBotIsRunning;
                }
            });
        }
    }
}
