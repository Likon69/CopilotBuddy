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

using Styx;
using Styx.CommonBot;

// ReSharper disable CheckNamespace
#endregion


namespace BuddyControlPanel
{
    public interface IBotChangeListener
    {
        void NotifyBotChanged(BotBase newBot);
    }
}
