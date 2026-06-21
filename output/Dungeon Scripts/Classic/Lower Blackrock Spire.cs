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
    using Bots.DungeonBuddy;
    using Bots.DungeonBuddy.Profiles;
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
    namespace Bots.DungeonBuddy.Dungeon_Scripts.Classic
#endif
{
    public class LowerBlackrockSpire:Dungeon
    {
        #region Overrides of Dungeon
        public override uint DungeonId
        {
            get { return 32; }
        }

        public override WoWPoint Entrance { get { return new WoWPoint(-7522.93, -1232.999, 285.74); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(77.55, -223.18, 49.84); } }

        #endregion
    }
}
