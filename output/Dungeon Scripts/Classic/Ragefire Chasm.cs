using System;
using System.Linq;
using CommonBehaviors.Actions;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using TreeSharp;
using Styx;
using Styx.WoWInternals.WoWObjects;
using System.Collections.Generic;
using Action = TreeSharp.Action;

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
    public class RagefireChasm : Dungeon
    {
        #region Overrides of Dungeon

        /// <summary> The mapid of this dungeon. </summary>
        /// <value>The map identifier.</value>
        public override uint DungeonId
        {
            get { return 4; }
        }
        public override WoWPoint Entrance { get { return new WoWPoint(1820,-4422.798, -18.4704); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(2.77, -16.866, -16.19); } }


        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var t in units)
            {
                var prioObject = t.Object;

                //We kill his adds first
                if (prioObject.Entry == 11319 || prioObject.Entry == 11318)
                {
                    t.Score += 400;
                }
            }
        }

        #endregion

        [EncounterHandler(11517, "Oggleflint")]
        public Composite OggleflintFight()
        {
            return
                new PrioritySelector();
        }

        [EncounterHandler(11520, "Taragaman")]
        public Composite TaragamanFight()
        {
            return
                new PrioritySelector();
        }
		
		[EncounterHandler(11518, "Jergosh the Invoker")]
        public Composite JergoshFight()
        {
            return
                new PrioritySelector();
        }

        [EncounterHandler(11519, "Bazzalan")]
        public Composite BazzalanFight()
        {
            return
                new PrioritySelector();
        }
    }
}
