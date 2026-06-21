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

using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using BuddyControlPanel.Resources.Localization;
using CopilotBuddy.UI;
using Styx;
using Styx.CommonBot;
using Styx.Logic.Profiles;
using Styx.Logic.Combat;

// ReSharper disable ConvertClosureToMethodGroup
// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    public class ContextMenu_Main : Assets.ThemedContextMenu, IBotChangeListener, IBotStateChangeListener, IProfileChangeListener
    {
        #region Creation & Destruction
        public ContextMenu_Main()
        {
            BorderBrush = Assets.BrushTransparent;

            Items.Add(new MenuItem_Bots());
            Items.Add(new MenuItem_BotSettings());
            _menuItem_BotActions = new MenuItem_BotActions();
            Items.Add(_menuItem_BotActions);

            Items.Add(new Assets.ThemedSeparator());
            Items.Add(new MenuItem_LoadProfile());
            Items.Add(new MenuItem_ProfileSettings());

            Items.Add(new Assets.ThemedSeparator());
            var combatRoutineName = ((RoutineManager.Current != null) && !string.IsNullOrEmpty(RoutineManager.Current.Name))
                ? RoutineManager.Current.Name
                : BCPGlobalization.GeneralText_Unknown;
            Items.Add(new MenuItem_ButtonProxy(
                string.Format(BCPGlobalization.Item_CombatRoutineConfiguration_LabelFormat, combatRoutineName),
                string.Format(BCPGlobalization.Item_CombatRoutineConfiguration_ToolTipFormat, combatRoutineName),
                "btnClassConfig",
                false));

            Items.Add(new MenuItem_SettingsAndTools());
            Items.Add(new MenuItem_Plugins());

            Items.Add(new Assets.ThemedSeparator());
            Items.Add(new MenuItem_DeveloperTools());

            Items.Add(new Assets.ThemedSeparator());
            Items.Add(new MenuItem_Help());
            Items.Add(new MenuItem_Preferences());

            Items.Add(new Assets.ThemedSeparator());
            Items.Add(new MenuItem_Cancel(this));
            Items.Add(BuildExitChoices());
        }
        #endregion

        private readonly MenuItem_BotActions _menuItem_BotActions;

        private MenuItem BuildExitChoices()
        {
            var exitSubMenu = new Assets.ThemedMenuItem()
            {
                Header = BCPGlobalization.Item_Exit_Label,
                ToolTip = null,
            };

            exitSubMenu.Items.Add(new MenuItem_Exit_BuddyBotClose());
            exitSubMenu.Items.Add(new MenuItem_Exit_GameClientLogout());
            exitSubMenu.Items.Add(new MenuItem_Exit_GameClientExit());

            return exitSubMenu;
        }

        public void NotifyBotChanged(BotBase newBot)
        {
            foreach (var menuItem in Items.OfType<MenuItemBase>())
                menuItem.NotifyBotChanged(newBot);
        }


        public void NotifyProfileChanged()
        {
            foreach (var menuItem in Items.OfType<MenuItemBase>())
                menuItem.NotifyProfileChanged();
        }


        public void NotifyStateChanged(CurrentBotStateNotification currentBotStateNotification)
        {
            foreach (var menuItem in Items.OfType<MenuItemBase>())
                menuItem.NotifyStateChanged(currentBotStateNotification);
        }

        protected override void OnOpened(RoutedEventArgs evt)
        {
            _menuItem_BotActions.EvaluateBotActionsAvailable();
        }
    }


    internal class MenuItem_Cancel : MenuItemBase
    {
        public MenuItem_Cancel(ContextMenu contextMenuToClose)
            : base(BCPGlobalization.Item_ContextMenuCancel_Label, BCPGlobalization.Item_ContextMenuCancel_ToolTip)
        {
            _contextMenuToClose = contextMenuToClose;
        }

        private readonly ContextMenu _contextMenuToClose;

        protected override void OnClick()
        {
            _contextMenuToClose.IsOpen = false;
            base.OnClick();
        }
    }


    internal class MenuItem_DeveloperTools : MenuItemBase
    {
        public MenuItem_DeveloperTools()
            : base(BCPGlobalization.Item_DeveloperTools_Label, BCPGlobalization.Item_DeveloperTools_ToolTip)
        {
            // empty
        }

        protected override void OnClick()
        {
            Utility.InvokeOnSpecificDispatcher(Application.Current.Dispatcher, () =>
            {
                var window = new DeveloperToolsWindow() { Owner = Application.Current.MainWindow };
                window.Show();
            });

            base.OnClick();
        }
    }


    internal abstract class MenuItem_ExitBase : MenuItemBase
    {
        public MenuItem_ExitBase(string exitActionLabel,
                            string exitActionToolTip,
                            string exitActionConfirmationQuestion)
            : base(exitActionLabel, exitActionToolTip)
        {
            Contract.Requires(!string.IsNullOrEmpty(exitActionLabel), () => "exitActionLabel may not be null or empty.");
            Contract.Requires(!string.IsNullOrEmpty(exitActionToolTip), () => "exitActionToolTip may not be null or empty.");
            Contract.Requires(!string.IsNullOrEmpty(exitActionConfirmationQuestion),
                                () => "exitActionConfirmationQuestion may not be null or empty.");

            _exitActionConfirmationQuestion = exitActionConfirmationQuestion;
        }

        private readonly string _exitActionConfirmationQuestion;

        protected override void OnClick()
        {
            var mousePosition = Utility.GetStartLocation(this);

            var exitConfirmationDialog = new Dialog_General(SystemIcons.Question,
                BCPGlobalization.Title_ExitConfirmation,
                _exitActionConfirmationQuestion,
                MessageBoxButton.OKCancel)
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = mousePosition.X,
                Top = mousePosition.Y
            };

            var messageBoxResult = exitConfirmationDialog.ShowDialog();
            if (messageBoxResult.HasValue && messageBoxResult.Value)
                DoExitAction();

            base.OnClick();
        }

        protected abstract void DoExitAction();
    }

    internal class MenuItem_Exit_BuddyBotClose : MenuItem_ExitBase
    {
        public MenuItem_Exit_BuddyBotClose()
            : base(string.Format(BCPGlobalization.Item_Exit_BuddyBotClose_LabelFormat, BCPGlobalization.BuddyBotName),
                string.Format(BCPGlobalization.Item_Exit_BuddyBotClose_ToolTipFormat, BCPGlobalization.BuddyBotName),
                string.Format(BCPGlobalization.Item_Exit_BuddyBotClose_ConfirmationQuestion, BCPGlobalization.BuddyBotName))
        {
            // empty
        }

        protected override void DoExitAction()
        {
            Utility.BuddyBotExit();
        }
    }

    internal class MenuItem_Exit_GameClientExit : MenuItem_ExitBase
    {
        public MenuItem_Exit_GameClientExit()
            : base(BCPGlobalization.Item_Exit_GameClientExit_Label,
                string.Format(BCPGlobalization.Item_Exit_GameClientExit_ToolTipFormat, BCPGlobalization.BuddyBotName),
                string.Format(BCPGlobalization.Item_Exit_GameClientExit_ConfirmationQuestion, BCPGlobalization.BuddyBotName))
        {
            // empty
        }

        protected override void DoExitAction()
        {
            Utility.GameClientExit();
        }
    }

    internal class MenuItem_Exit_GameClientLogout : MenuItem_ExitBase
    {
        public MenuItem_Exit_GameClientLogout()
            : base(BCPGlobalization.Item_Exit_GameClientLogout_Label,
                string.Format(BCPGlobalization.Item_Exit_GameClientLogout_ToolTipFormat, BCPGlobalization.BuddyBotName),
                string.Format(BCPGlobalization.Item_Exit_GameClientLogout_ConfirmationQuestion, BCPGlobalization.BuddyBotName))
        {
            // empty
        }

        protected override void DoExitAction()
        {
            Utility.GameClientLogout();
        }
    }


    internal class MenuItem_Preferences : MenuItemBase
    {
        public MenuItem_Preferences()
            : base(BCPGlobalization.Item_BuddyControlPanelPreferences_Label,
            string.Format(BCPGlobalization.Item_BuddyControlPanelPreferences_ToolTipFormat, BCPGlobalization.PluginName))
        {
            // empty
        }

        protected override void OnClick()
        {
            Dialog_Preferences.Instance.ShowAtFront(Utility.GetStartLocation(this));
            base.OnClick();
        }
    }


    internal class MenuItem_ProfileSettings : MenuItemBase
    {
        public MenuItem_ProfileSettings()
            : base(BCPGlobalization.Item_ProfileConfiguration_Label,
                   BCPGlobalization.Item_ProfileConfiguration_ToolTip)
        {
            // empty
        }

        protected override void OnClick()
        {
            Dialog_ProfileSettings.Instance.ReevaluateState();
            Dialog_ProfileSettings.Instance.ShowAtFront(Utility.GetStartLocation(this));
            base.OnClick();
        }

        public override void NotifyBotChanged(BotBase newBot)
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () => HandleStateChanges());
        }

        public override void NotifyProfileChanged()
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () => HandleStateChanges());
        }

        public override void NotifyStateChanged(CurrentBotStateNotification currentBotState)
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
            {
                if (currentBotState.IsLoadProfileInProgress.HasValue)
                {
                    IsEnabled = !currentBotState.IsLoadProfileInProgress.Value;
                    ToolTip = IsEnabled ? DefaultToolTip : BCPGlobalization.GeneralText_DisabledWhileProfileIsLoading;
                    if (!IsEnabled)
                        return;
                }

                HandleStateChanges();
            });
        }

        private void HandleStateChanges()
        {
            IsEnabled = true;
            ToolTip = DefaultToolTip;
        }
    }


    internal class MenuItem_SettingsAndTools : MenuItemBase
    {
        public MenuItem_SettingsAndTools()
            : base(string.Format(BCPGlobalization.Item_BuddyBotConfiguration_LabelFormat, BCPGlobalization.BuddyBotName),
                    string.Format(BCPGlobalization.Item_BuddyBotConfiguration_ToolTipFormat, BCPGlobalization.BuddyBotName))
        {
            // empty
        }

        protected override void OnClick()
        {
            (new SettingsWindow()).ShowAtFront(Utility.GetStartLocation(this));
            base.OnClick();
        }


        public override void NotifyStateChanged(CurrentBotStateNotification currentBotStateNotification)
        {
            HandleNotifyOfStateChange(currentBotStateNotification);
        }
    }
}
