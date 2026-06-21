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

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Bots.Grind;
using Bots.Quest;
using BuddyControlPanel.Resources.Localization;
using Styx;
using Styx.CommonBot;
using Styx.Logic;

// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    internal class MenuItem_BotActions : MenuItemBase
    {
        #region Creation and Destruction
        /// <summary>
        /// Creates a MenuItem as a list of Bots that are installed in Honorbuddy.
        /// </summary>
        public MenuItem_BotActions()
            : base(BCPGlobalization.GeneralText_BotActions, null)
        {
            Background = Assets.BrushTransparent;

            // Create the sub-menu entries...
            Items.Add(new MenuItem_LevelBotVendorAction(
                BCPGlobalization.Item_VendorAction_MailSellRepair_Label,
                BCPGlobalization.Item_VendorAction_MailSellRepair_ToolTip,
                s_viableBots_ArchGrindQuest,
                new LambdaProperty<bool>("MailSellRepair",
                    () => Vendors.ForceMail && Vendors.ForceRepair && Vendors.ForceSell,
                    (value) =>
                    {
                        Vendors.ForceMail = value;
                        Vendors.ForceRepair = value;
                        Vendors.ForceSell = value;
                    })));

            Items.Add(new MenuItem_LevelBotVendorAction(
                BCPGlobalization.Item_VendorAction_Mail_Label,
                BCPGlobalization.Item_VendorAction_Mail_ToolTip,
                s_viableBots_ArchGrindQuest,
                new LambdaProperty<bool>("Mail",
                    () => Vendors.ForceMail,
                    (value) => Vendors.ForceMail = value)));

            Items.Add(new MenuItem_LevelBotVendorAction(
                BCPGlobalization.Item_VendorAction_Repair_Label,
                BCPGlobalization.Item_VendorAction_Repair_ToolTip,
                s_viableBots_ArchGrindQuest,
                new LambdaProperty<bool>("Repair",
                    () => Vendors.ForceRepair,
                    (value) => Vendors.ForceRepair = value)));

            Items.Add(new MenuItem_LevelBotVendorAction(
                BCPGlobalization.Item_VendorAction_Sell_Label,
                BCPGlobalization.Item_VendorAction_Sell_ToolTip,
                s_viableBots_ArchGrindQuest,
                new LambdaProperty<bool>("Sell",
                    () => Vendors.ForceSell,
                    (value) => Vendors.ForceSell = value)));

            Items.Add(new Assets.ThemedSeparator());

            Items.Add(new MenuItem_LevelBotVendorAction(
                BCPGlobalization.BotAction_MoveToInnkeepAndStopBot_Label,
                null,
                s_viableBots_GrindQuest,
                new LambdaProperty<bool>("ToInnkeepAndStop",
                    () => BotAction_MoveToInnkeeper.Instance.PursueInnkeeperViaAction == BotAction_MoveToInnkeeper.ActionOnArrival.BotStop,
                    (value) =>
                    {
                        BotAction_MoveToInnkeeper.Instance.PursueInnkeeperViaAction =
                            value
                            ? BotAction_MoveToInnkeeper.ActionOnArrival.BotStop
                            : BotAction_MoveToInnkeeper.ActionOnArrival.NoPursue;
                    })));

            Items.Add(new MenuItem_LevelBotVendorAction(
                BCPGlobalization.BotAction_MoveToInnkeepAndLogout_Label,
                null,
                s_viableBots_GrindQuest,
                new LambdaProperty<bool>("ToInnkeepAndLogout",
                    () => BotAction_MoveToInnkeeper.Instance.PursueInnkeeperViaAction == BotAction_MoveToInnkeeper.ActionOnArrival.Logout,
                    (value) =>
                    {
                        BotAction_MoveToInnkeeper.Instance.PursueInnkeeperViaAction =
                            value
                            ? BotAction_MoveToInnkeeper.ActionOnArrival.Logout
                            : BotAction_MoveToInnkeeper.ActionOnArrival.NoPursue;
                    })));

            Items.Add(new MenuItem_LevelBotVendorAction(
                BCPGlobalization.BotAction_MoveToInnkeepAndExitGame_Label,
                null,
                s_viableBots_GrindQuest,
                new LambdaProperty<bool>("ToInnkeepAndExit",
                    () => BotAction_MoveToInnkeeper.Instance.PursueInnkeeperViaAction == BotAction_MoveToInnkeeper.ActionOnArrival.GameExit,
                    (value) =>
                    {
                        BotAction_MoveToInnkeeper.Instance.PursueInnkeeperViaAction =
                            value
                            ? BotAction_MoveToInnkeeper.ActionOnArrival.GameExit
                            : BotAction_MoveToInnkeeper.ActionOnArrival.NoPursue;
                    })));
        }


        private static readonly List<BotBase> s_viableBots_ArchGrindQuest =
            (from botBase in BotManager.Instance.Bots.Values
             where
                 (botBase is LevelBot)
                || (botBase is QuestBot)
             select botBase)
            .ToList();

        private static readonly List<BotBase> s_viableBots_GrindQuest =
            (from botBase in BotManager.Instance.Bots.Values
             where
                 (botBase is LevelBot)
                || (botBase is QuestBot)
             select botBase)
            .ToList();


        public void EvaluateBotActionsAvailable()
        {
            var isEnableBotActionSubmenu = false;
            foreach (var item in Items.OfType<MenuItem_BotAction>())
            {
                item.EvaluatePresentation();
                if (item.Visibility == Visibility.Visible)
                    isEnableBotActionSubmenu = true;
            }

            var noActionsAvailableToolTip =
                ((BotManager.Current == null) || string.IsNullOrEmpty(BotManager.Current.Name))
                ? BCPGlobalization.GeneralText_BotActionsNotAvailable_UnknownBot
                : string.Format(BCPGlobalization.GeneralTextFormat_BotActionsNotAvailable_SpecificBot,
                                BotManager.Current.Name);

            IsEnabled = isEnableBotActionSubmenu;
            ToolTip = isEnableBotActionSubmenu
                ? DefaultToolTip
                : noActionsAvailableToolTip;
        }
        #endregion
    }


    // marker interface
    internal abstract class MenuItem_BotAction : MenuItemBase
    {
        protected MenuItem_BotAction(string actionName, string toolTip, List<BotBase> viableBots)
            : base(actionName, toolTip)
        {
            Contract.Requires(!string.IsNullOrEmpty(actionName), () => "actionName may not be null or empty.");
            Contract.Requires(viableBots != null, () => "viableBots may not be null.");

            _viableBots = viableBots;

            Header = actionName;
        }

        protected readonly List<BotBase> _viableBots;

        public abstract void EvaluatePresentation();
    }


    internal class MenuItem_LevelBotVendorAction : MenuItem_BotAction
    {
        public MenuItem_LevelBotVendorAction(string actionName, string toolTip,
            List<BotBase> viableBots, LambdaProperty<bool> lambdaProperty)
            : base(actionName, toolTip, viableBots)
        {
            IsCheckable = true;
            IsVisibleChanged += (sender, evt) =>
            {
                if (!IsVisible)
                    return;

                IsChecked = lambdaProperty.Getter();
            };

            Click += (sender, evt) =>
            {
                lambdaProperty.Setter(IsChecked);
                var confirmation = IsChecked
                    ? string.Format(BCPGlobalization.GeneralTextFormat_VendorActionsScheduled, actionName)
                    : string.Format(BCPGlobalization.GeneralTextFormat_VendorActionsCancelled, actionName);
                Utility.OverlayNotification(confirmation, Assets.ColorInformation);
            };
        }

        public override void EvaluatePresentation()
        {
            var currentBot = BotManager.Current;
            var actionReason_Disabled = ActionReason_Disabled();
            var isActionAvailable = _viableBots.Any(botBase => botBase == currentBot);

            IsEnabled = actionReason_Disabled == null;
            Visibility = isActionAvailable ? Visibility.Visible : Visibility.Collapsed;
            ToolTip = IsEnabled ? DefaultToolTip : actionReason_Disabled;
        }


        private string ActionReason_Disabled()
        {
            if (!Utility.IsVendorsAccessible())
                return BCPGlobalization.GeneralText_BotActionUnusable_VendorsInaccessible;

            return null;
        }
    }
}