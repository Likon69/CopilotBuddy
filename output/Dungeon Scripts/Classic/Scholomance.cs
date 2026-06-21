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
    public class Scholomance:Dungeon
    {
        #region Overrides of Dungeon
        public override uint DungeonId
        {
            get { return 2; }
        }

        public override WoWPoint Entrance { get { return new WoWPoint(1279.48, -2551.52,87.41); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(188.05, 126.49, 138.82); } }

        #endregion
    }
}
