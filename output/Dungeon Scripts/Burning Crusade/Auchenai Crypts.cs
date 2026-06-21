using System.Collections.Generic;

using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
#if USE_DUNGEONBUDDY_DLL
    using Bots.DungeonBuddyDll;
    using Bots.DungeonBuddyDll.Attributes;
    using Bots.DungeonBuddyDll.Helpers;
    namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Burning_Crusade
#else
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
namespace Bots.DungeonBuddy.Dungeon_Scripts.Burning_Crusade
#endif
{
    public class AuchenaiCrypts : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 149; } }

        public override WoWPoint Entrance { get { return new WoWPoint(-3362.34, 5230.694, -101.0485); } }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var targetPriority in units)
            {
                if (targetPriority.Object.Entry == 18441) // Stolen Soul
                    targetPriority.Score += 200;
            }
        }

        #endregion

        private WoWUnit _shirrak;

        [EncounterHandler(18371, "Shirrak the Dead Watcher")]
        public Composite ShirrakEncounter()
        {
            return new PrioritySelector(
                ctx => _shirrak = ctx as WoWUnit,
                ScriptHelpers.CreateSpreadOutLogic(ctx => true, () => _shirrak.Location, 10, 30), // Focus Fire
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 10, 18374) // Focus Fire
                );
        }

        [EncounterHandler(18373, "Exarch Maladaar", Mode = CallBehaviorMode.CurrentBoss)]
        public Composite ExarchMaladaarEncounter()
        {
            //WoWUnit boss = null;
            return new PrioritySelector(
                //   ctx => boss = ctx as WoWUnit,
                );
        }
    }
}