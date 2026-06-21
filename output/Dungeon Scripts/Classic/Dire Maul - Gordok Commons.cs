using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Styx;
using Styx.Logic.Pathing;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;

#if USE_DUNGEONBUDDY_DLL
using Bots.DungeonBuddyDll;
using Bots.DungeonBuddyDll.Attributes;
using Bots.DungeonBuddyDll.Helpers;
namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Classic
#else
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
    namespace Bots.DungeonBuddy.Dungeon_Scripts.Classic
#endif
{
    public class DireMaulGordokCommons : Dungeon
    {
        #region Overrides of Dungeon
        public override uint DungeonId
        {
            get { return 38; }
        }

        public override WoWPoint Entrance { get { return new WoWPoint(-3817.948, 1250.378, 160.2729); } }
        #endregion

        [EncounterHandler(14325, "Captain Kromcrush")]
        public Composite CaptainKromcrushFight()
        {
            return ScriptHelpers.CreateTankUnitAtLocation(() => new WoWPoint(584.6393, 481.2674, 29.4628), 7);
        }

        public override void WeighTargetsFilter(List<Styx.Logic.Targeting.TargetPriority> units)
        {
            foreach (var p in units)
            {
                var unit = p.Object.ToUnit();
                if (unit.Entry == 11450) // Gordok Reaver
                    p.Score += 100;
            }
        }
    }
}
