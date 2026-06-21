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
using Bots.DungeonBuddyDll.Profiles;
using Bots.DungeonBuddyDll.Attributes;
using Bots.DungeonBuddyDll.Helpers;
namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Classic
#else
    using Bots.DungeonBuddy.Profiles;
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
namespace Bots.DungeonBuddy.Dungeon_Scripts.Classic
#endif

{
    public class MaraudonFoulsporeCavern:Dungeon
    {
        #region Overrides of Dungeon
        public override uint DungeonId
        {
            get { return 26; }
        }
        public override WoWPoint Entrance { get { return new WoWPoint(-1381.243, 2918.000, 73.42503); } }
        #endregion

        [EncounterHandler(12258, "Razorlash")]
        public Composite RazorlashFight()
        {
            return
                new PrioritySelector(
                    // Boss has a cleave
                    ScriptHelpers.CreateTankFaceAwayGroupUnit(10));
        }
    }
}
