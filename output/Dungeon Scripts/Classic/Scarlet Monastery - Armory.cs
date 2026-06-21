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
    public class ScarletMonasteryArmory : Dungeon
    {
        #region Overrides of Dungeon
        public override uint DungeonId
        {
            get { return 163; }
        }

        public override WoWPoint Entrance { get { return new WoWPoint(2879.328f, -838.9813f, 160.3264f); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(1608.465, -315.2825, 18.67); } }
        #endregion

        private WoWUnit _herod;

        [EncounterHandler(3975, "Herod")]
        public Composite HerodFight()
        {
            return new PrioritySelector(
                ctx => _herod = ctx as WoWUnit,
                ScriptHelpers.CreateRunAwayFromBad(ctx => _herod != null && _herod.HasAura("Whirlwind") && StyxWoW.Me.IsFollower(), 8f, 3975));
        }
    }
}
