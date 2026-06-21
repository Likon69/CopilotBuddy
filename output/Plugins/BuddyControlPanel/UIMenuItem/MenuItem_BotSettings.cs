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

using System.Windows;

using BuddyControlPanel.Resources.Localization;
using Styx;
using Styx.CommonBot;

// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    internal class MenuItem_BotSettings : MenuItemBase
    {
        public MenuItem_BotSettings()
            : base(BCPGlobalization.Item_BotConfiguration_Label,
                    BCPGlobalization.Item_BotConfiguration_ToolTip)
        {
            // empty
        }


        protected override void OnClick()
        {
            if (BotManager.Current == null)
                return;

            var startLocation = Utility.GetStartLocation(this);

            // N.B. The ConfigurationForm/ConfigurationWindow should always be launched
            // from the 'main' thread, since they may have already been created and cached.
            Utility.BeginInvokeOnSpecificDispatcher(Application.Current.Dispatcher, () =>
            {
                var configForm = BotManager.Current.ConfigurationForm;
                var configWindow = BotManager.Current.ConfigurationWindow;

                if (configWindow != null)
                    configWindow.ShowAtFront(startLocation);

                else if ((configForm != null) && !configForm.IsDisposed && !configForm.Disposing)
                    configForm.ShowAtFront(startLocation);
            });

            base.OnClick();
        }


        public override void NotifyBotChanged(BotBase newBot)
        {
            // N.B.: Prevent Configuration windows from being created on 'wrong' thread...
            // Accessing some ConfigurationForm/ConfigurationWindow may cause them to be created and cached.
            // We don't want this to happen on the 'wrong' (Overlay) thread, so we structure the logic to prevent this.
            Utility.BeginInvokeOnSpecificDispatcher(Application.Current.Dispatcher, () =>
            {
                var hasConfiguration = false;

                if (newBot != null)
                {
                    var configForm = newBot.ConfigurationForm;
                    var configWindow = newBot.ConfigurationWindow;

                    hasConfiguration = (configForm != null) && !configForm.IsDisposed && !configForm.Disposing;
                    hasConfiguration |= (configWindow != null);
                }

                Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
                {
                    Header = (newBot == null)
                        ? BCPGlobalization.Item_BotConfiguration_Label
                        : string.Format(BCPGlobalization.Item_BotConfigurationSpecific_LabelFormat, newBot.Name);

                    IsEnabled = hasConfiguration;

                    ToolTip = (newBot == null)
                        ? BCPGlobalization.Item_BotConfigurationUnavailable_ToolTip
                        : IsEnabled
                            ? string.Format(BCPGlobalization.Item_BotConfigurationSpecific_ToolTipFormat, newBot.Name)
                            : string.Format(BCPGlobalization.Item_BotConfigurationSpecificUnavailable_ToolTipFormat, newBot.Name);
                });
            });
        }
    }
}