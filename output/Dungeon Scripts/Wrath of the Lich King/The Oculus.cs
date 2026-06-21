using Styx.Logic;
using TreeSharp;

#if USE_DUNGEONBUDDY_DLL
using Bots.DungeonBuddyDll;
using Bots.DungeonBuddyDll.Profiles;
using Bots.DungeonBuddyDll.Attributes;
using Bots.DungeonBuddyDll.Helpers;
namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Wrath_of_the_Lich_King
#else
    using Bots.DungeonBuddy;
    using Bots.DungeonBuddy.Profiles;
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
namespace Bots.DungeonBuddy.Dungeon_Scripts.Wrath_of_the_Lich_King
#endif
{
    public class TheOculus : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 206; } }

        #endregion

        [ObjectHandler(189985, "Nexus Portal")]
        public Composite NexusPortalHandler()
        {
            return new PrioritySelector(
                new Decorator(
                    ctx => Targeting.Instance.FirstUnit == null,
                    ScriptHelpers.CreateInteractWithObject(189985)));
        }
    }
}