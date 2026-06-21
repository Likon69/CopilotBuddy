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

using Styx.CommonBot;
using Styx.Logic.BehaviorTree;

// ReSharper disable CheckNamespace
#endregion


namespace BuddyControlPanel
{
    /// <summary>
    /// Defines the current state for the notification operation.
    /// Note that a partial state may be present... where the value of one or more of the members is unknown.
    /// A listener should only act on information that is available, and not make assumptions about missing information.
    /// </summary>
    public class CurrentBotStateNotification
    {
        public TreeRootState? TreeRootState { get; set; }
        public bool? IsLoadProfileInProgress { get; set; }
    }


    public interface IBotStateChangeListener
    {
        void NotifyStateChanged(CurrentBotStateNotification currentBotStateNotification);
    }
}
