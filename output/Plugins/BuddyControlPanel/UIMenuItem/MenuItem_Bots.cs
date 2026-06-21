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

using System.Linq;

using BuddyControlPanel.Resources.Localization;
using Styx;
using Styx.CommonBot;

// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    internal class MenuItem_Bots : MenuItemBase
    {
        #region Creation and Destruction
        /// <summary>
        /// Creates a MenuItem as a list of Bots that are installed in Honorbuddy.
        /// </summary>
        public MenuItem_Bots()
            : base(BCPGlobalization.Item_Bots_Label,
                string.Format(BCPGlobalization.Item_Bots_ToolTipFormat, BCPGlobalization.BuddyBotName))
        {
            Background = Assets.BrushTransparent;

            // Create the sub-menu entries...
            foreach (var botBase in BotManager.Instance.Bots.Values.OrderBy(botBase => botBase.Name))
                Items.Add(new MenuItem_Bot(botBase));
        }
        #endregion


        // Anytime a new bot is selected, we want to deselect the previous bot choice, and
        // 'check' the current bot chocie.  We also need to update tooltips, and menu bots
        // with information caused by a new bot selection.
        public override void NotifyBotChanged(BotBase newBot)
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
            {
                //Header = ((newBot == null) || string.IsNullOrEmpty(newBot.Name))
                //	? "Bots"
                //	: string.Format("Bot ({0})", newBot.Name);

                foreach (var menuItem in Items.OfType<MenuItem_Bot>())
                    menuItem.NotifyBotChanged(newBot);
            });
        }


        public override void NotifyStateChanged(CurrentBotStateNotification currentBotStateNotification)
        {
            HandleNotifyOfStateChange(currentBotStateNotification);
        }
    }
}