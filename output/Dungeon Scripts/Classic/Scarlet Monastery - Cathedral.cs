using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Styx.Logic.Pathing;

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
    public class ScarletMonasteryCathedral : Dungeon
    {
        #region Overrides of Dungeon
        public override uint DungeonId
        {
            get { return 164; }
        }

        public override WoWPoint Entrance { get { return new WoWPoint(2919.21, -821.85, 160.3331); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(853.05, 1314.95, 18.68); } }

        #endregion
    }
}
