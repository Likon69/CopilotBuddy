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
using System.Windows.Controls.Primitives;

// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    /// <summary>
    /// A Proxy button is one where we just 'press' the equivalent button on the Honorbuddy
    /// MainWindow.  We use Proxy buttons to eliminate duplicating code that we don't have
    /// access (i.e., private or protected).  We also use Proxy buttons where we can add no
    /// additional value by replicating the code.  For instance, the dialog or window that should
    /// become visible doesn't have enough API exposed to allow us to place the window in
    /// convenient spot, or such.
    /// </summary>
    internal class MenuItem_ButtonProxy : MenuItemBase
    {
        public MenuItem_ButtonProxy(string menuItemHeader,
                                    string toolTip,
                                    string linkedButtonName,
                                    bool isDisabledOnBotStopped)
            : base(menuItemHeader, toolTip)
        {
            Contract.Requires(menuItemHeader != null, () => "menuItemHeader may not be null");
            Contract.Requires(linkedButtonName != null, () => "linkedButtonName may not be null");

            _isDisabledOnBotStopped = isDisabledOnBotStopped;

            if (!string.IsNullOrEmpty(linkedButtonName))
            {
                // N.B.: If Honorbuddy main window renames this control or changes its type,
                // this will need adjustment also. This call will throw a Maintenance exception 
                // if the control is not found.
                _linkedButton = Utility.FindUiControlByName<Button>(linkedButtonName);
            }
        }

        private readonly bool _isDisabledOnBotStopped;
        private readonly Button _linkedButton;

        protected override void OnClick()
        {
            if (_linkedButton != null)
            {
                Utility.InvokeOnSpecificDispatcher(_linkedButton.Dispatcher, () =>
                    _linkedButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)));
            }

            base.OnClick();
        }

        public override void NotifyStateChanged(CurrentBotStateNotification currentBotStateNotification)
        {
            if (_isDisabledOnBotStopped)
                HandleNotifyOfStateChange(currentBotStateNotification);
        }
    }
}