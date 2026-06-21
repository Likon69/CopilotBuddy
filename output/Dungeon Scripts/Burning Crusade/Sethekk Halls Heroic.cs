using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if USE_DUNGEONBUDDY_DLL
using Bots.DungeonBuddyDll;
using Bots.DungeonBuddyDll.Actions;
using Bots.DungeonBuddyDll.Attributes;
using Bots.DungeonBuddyDll.Helpers;
namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Burning_Crusade
#else
    using Bots.DungeonBuddy.Actions;
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
    namespace Bots.DungeonBuddy.Dungeon_Scripts.Burning_Crusade
#endif
{
    public class SethekkHallsHeroic : Dungeon
    {
        #region Overrides of Dungeon
        public override uint DungeonId
        {
            get { return 180; }
        }
        #endregion
    }
}
