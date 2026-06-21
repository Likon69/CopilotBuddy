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
    public class ScarletMonestary : Dungeon
    {
        #region Overrides of Dungeon

        /// <summary> The mapid of this dungeon. </summary>
        /// <value>The map identifier.</value>
        public override uint DungeonId
        {
            get { return 18; }
        }

        public override WoWPoint Entrance { get { return new WoWPoint(2919.255, -802.4, 160.3327); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(1687.29, 1046.65, 18.68); } }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            base.IncludeTargetsFilter(incomingunits, outgoingunits);

            foreach (var unit in incomingunits.Select(obj => obj.ToUnit()))
            {
                if (!StyxWoW.Me.Combat && ScriptHelpers.Tank == StyxWoW.Me)
                {
                    //Force add bugged mobs to pull, such as the goblin and a few defies that return !IsHostile
                    if ((unit.DistanceSqr < 40 * 40 && !unit.IsTargetingMyPartyMember) && (unit.Entry == 4293 || unit.Entry == 4306 || unit.Entry == 4308 || unit.Entry == 6427 || unit.Entry == 6426))
                    {
                        outgoingunits.Add(unit);
                    }
                }

                if (StyxWoW.Me.Combat)
                {
                    //Force it to attack Helix
                   /*if ((unit.DistanceSqr < 40 * 40 && !unit.IsTargetingMyPartyMember) && (unit.Entry == 47296 || unit.Entry == 47739))
                    {
                        outgoingunits.Add(unit);
                    }*/
                }

            }
        }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var t in units)
            {
                var prioObject = t.Object;

                //Target good ones first
                if (prioObject.Entry == 4306 || prioObject.Entry == 4293)
                {
                    t.Score += 400;
                }
            }
        }

        #endregion

        [EncounterHandler(0, "Root")]
        public Composite Root()
        {
            return new PrioritySelector();
        }

        [EncounterHandler(3983, "Interrogator Vishas", Mode = CallBehaviorMode.Proximity)]
        public Composite InterrogatorVishasFight()
        {
            return ScriptHelpers.CreateClearArea(() => new WoWPoint(1786.55, 1124.44, 7.49), 40, u => u.Entry == 4306 || u.Entry == 4293);
        }

        [EncounterHandler(4543, "Bloodmage Thalnos")]
        public Composite BloodmageThalnosFight()
        {
            return
                new PrioritySelector();
        }
    }
}
