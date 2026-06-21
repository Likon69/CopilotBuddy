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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Styx;
using Styx.CommonBot;

// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    internal class MenuItem_Bot : MenuItemBase
    {
        public MenuItem_Bot(BotBase botBase)
            : base(botBase.Name, null)
        {
            Contract.Requires(botBase != null, () => "botBase may not be null");

            IsCheckable = true;

            _botBase = botBase;

            if (s_botSelectorComboBox == null)
            {
                // Retrieves the "Bot Selection" ComboBox from the main Honorbuddy interface
                Utility.InvokeOnSpecificDispatcher(Application.Current.Dispatcher, () =>
                {
                    // N.B.: If Honorbuddy main window renames this control or changes its type,
                    // this will need adjustment also. This call will throw a Maintenance exception 
                    // if the control is not found.
                    s_botSelectorComboBox = Utility.FindUiControlByName<ComboBox>("cmbBotSelector");
                });
            }
        }

        private readonly BotBase _botBase;
        private static ComboBox s_botSelectorComboBox;


        protected override void OnClick()
        {
            // This method sets the value of the selected botbase from the control on
            // the main Honorbuddy panel.  If the control's name or type changes, this method
            // will need to be adjusted accordingly.
            // By doing it this way, the Honorbuddy main window stays in sync with any bot changes
            // made through the Buddy Control Panel.
            // N.B.: We need to make this call asynchronously.  Otherwise, it will block the Overlay
            // thread when the OnBotChanged callback is fired.  If synchronous, this will result
            // in deadly embrace between the two threads.
            Utility.BeginInvokeOnSpecificDispatcher(s_botSelectorComboBox.Dispatcher, () =>
            {
                // Don't fire events neeedlessly...
                if ((s_botSelectorComboBox.SelectedItem == null) || (s_botSelectorComboBox.SelectedItem != _botBase))
                    s_botSelectorComboBox.SelectedItem = _botBase;
            });

            base.OnClick();
        }


        public override void NotifyBotChanged(BotBase newBot)
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
            {
                var isSelectedBot = (_botBase == newBot);
                IsChecked = isSelectedBot;
                IsEnabled = !isSelectedBot;
            });
        }
    }
}