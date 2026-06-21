using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Styx.Logic.Pathing;

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
    public class DireMaulWarpwoodQuarter : Dungeon
    {
        #region Overrides of Dungeon
        public override uint DungeonId
        {
            get { return 34; }
        }

        public override WoWPoint Entrance { get { return new WoWPoint(-3817.948, 1250.378, 160.2729); } }

        #endregion
    }
}
