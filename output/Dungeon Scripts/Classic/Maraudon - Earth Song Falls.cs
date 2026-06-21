using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Styx.Logic;
using Styx.Logic.Pathing;
using TreeSharp;
using Styx.WoWInternals.WoWObjects;

#if USE_DUNGEONBUDDY_DLL
using Bots.DungeonBuddyDll;
using Bots.DungeonBuddyDll.Profiles;
using Bots.DungeonBuddyDll.Attributes;
using Bots.DungeonBuddyDll.Helpers;

namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Classic
#else
using Bots.DungeonBuddy;
    using Bots.DungeonBuddy.Profiles;
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
namespace Bots.DungeonBuddy.Dungeon_Scripts.Classic
#endif
{
    public class MaraudonEarthSongFalls : Dungeon
    {
        #region Overrides of Dungeon
        public override uint DungeonId
        {
            get { return 273; }
        }
        public override WoWPoint Entrance { get { return new WoWPoint(-1381.243, 2918.000, 73.42503); } }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var t in units)
            {
                var prioObject = t.Object;

                //Shardlings
                if (prioObject.Entry == 11783)
                {
                    t.Score += 400;
                }
            }
        }

        #endregion

        [EncounterHandler(12203, "Landslide")]
        public Composite LandslideFight()
        {

            return new PrioritySelector();
        }

        [EncounterHandler(12201, "Princess Theradras")]
        public Composite PrincessTheradrasFight()
        {

            return new PrioritySelector();
        }
    }
}
