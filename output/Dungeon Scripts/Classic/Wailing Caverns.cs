using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

using CommonBehaviors.Actions;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.POI;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using TreeSharp;
using Styx;
using Styx.WoWInternals.WoWObjects;
using System.Collections.Generic;
using Action = TreeSharp.Action;

#if USE_DUNGEONBUDDY_DLL
using Bots.DungeonBuddyDll;
using Bots.DungeonBuddyDll.Attributes;
using Bots.DungeonBuddyDll.Profiles;
using Bots.DungeonBuddyDll.Helpers;
namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Classic
#else
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
    using Bots.DungeonBuddy.Profiles;
namespace Bots.DungeonBuddy.Dungeon_Scripts.Classic
#endif
{
    public class WailingCaverns:Dungeon
    {

        #region Overrides of Dungeon
        public override uint DungeonId
        {
            get { return 1; }
        }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            base.IncludeTargetsFilter(incomingunits, outgoingunits);

            foreach (var unit in incomingunits.Select(obj => obj.ToUnit()))
            {
                if (!StyxWoW.Me.Combat && ScriptHelpers.Tank == StyxWoW.Me)
                {
                    //Force add bugged mobs to pull, such as the goblin and a few defies that return !IsHostile
                    if ((unit.DistanceSqr < 40 * 40 && !unit.IsTargetingMyPartyMember) && (unit.Entry == 5762 || unit.Entry == 5763 || unit.Entry == 3635 || unit.Entry == 3640 || unit.Entry == 3636))
                    {
                        outgoingunits.Add(unit);
                    }
                }

            }
        }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var t in units)
            {
                var prioObject = t.Object;

                //We kill his adds first
                if (prioObject.Entry == 3840 || prioObject.Entry == 5756)
                {
                    t.Score += 400;
                }
            }
        }

        #endregion

        public bool EventStarted;

        

        [EncounterHandler(0)]
        public Composite RootLogic()
        {
            return
                new PrioritySelector(
                        ScriptHelpers.CreateJumpDown(nat => !ScriptHelpers.IsBossAlive("Lord Cobrahn") && StyxWoW.Me.Location.Distance(new WoWPoint(-104.6688, 426.2907, -74.10992)) < 30, (new WoWPoint(-104.6688, 426.2907, -74.10992)), (new WoWPoint(-91.53806, 420.0687, -106.4197))),
                        ScriptHelpers.CreateJumpDown(nat => !ScriptHelpers.IsBossAlive("Skum") && StyxWoW.Me.Location.Distance(new WoWPoint(-291.5874, -5.171218, -58.48951)) < 30, (new WoWPoint(-291.5874, -5.171218, -58.48951)), (new WoWPoint(-284.1962, 3.52924, -63.7272))),
                        ScriptHelpers.CreateJumpDown(nat => !ScriptHelpers.IsBossAlive("Verdan the Everliving") && StyxWoW.Me.Location.Distance(new WoWPoint(-55.53799, 46.36995, -29.13713)) < 50, (new WoWPoint(-55.53799, 46.36995, -29.13713)), (new WoWPoint(-45.38879, 47.57143, -107.9036)))
                    
                    );

            

        }

        [EncounterHandler(3671, "Lady Anacondra")]
        public Composite LadyAnacondraFight()
        {
            return
                new PrioritySelector();
        }

        [EncounterHandler(3653, "Kresh")]
        public Composite KreshFight()
        {
            return
                new PrioritySelector();
        }

        [EncounterHandler(3670, "Lord Pythas")]
        public Composite LordPythasFight()
        {
            return
                new PrioritySelector();
        }

        [EncounterHandler(3669, "Lord Cobrahn")]
        public Composite LordCobrahnFight()
        {
            return
                new PrioritySelector();
        }

        [EncounterHandler(3674, "Skum")]
        public Composite SkumFight()
        {
            return
                new PrioritySelector();
        }

        [EncounterHandler(3673, "Lord Serpentis")]
        public Composite LordSerpentisFight()
        {
            return
                new PrioritySelector();
        }

        [EncounterHandler(5775, "Verdan the Everliving")]
        public Composite VerdantheEverlivingFight()
        {
            return
                new PrioritySelector();
        }

        [EncounterHandler(3654, "Mutanus the Devourer", Mode = CallBehaviorMode.CurrentBoss)]
        public Composite MutanustheDevourerFight()
        {
            //3635 3640 3636 5762 5763
            List<WoWUnit> listOfHostileMob = ObjectManager.GetObjectsOfType<WoWUnit>()
                                     .Where(ret => ret.IsAlive && ret.Location.Distance(StyxWoW.Me.Location) < 10)
                                     .OrderBy(u => u.Distance)
                                     .ToList();

            WoWPoint startEventPoint = new WoWPoint(-134.965, 125.402, -78.17783);
            WoWUnit eventNPC = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(x => x.Entry == 3678);
            WoWUnit realBoss = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(x => x.Entry == 3654);
            return
                new PrioritySelector(

                    

                    new Decorator(nat => EventStarted && StyxWoW.Me == ScriptHelpers.Tank && realBoss != null,
                        new Sequence(
                             new Action(ret => EventStarted = false),
                             new Action(ret => Logging.Write("Boss Spawned! Finishing Event")))),

                    new Decorator(nat => !ScriptHelpers.IsBossAlive("Verdan the Everliving") && !EventStarted && StyxWoW.Me.Location.Distance(startEventPoint) >= 3 && StyxWoW.Me == ScriptHelpers.Tank && !StyxWoW.Me.IsActuallyInCombat && eventNPC == null,
                        new Action(ret => Navigator.MoveTo(startEventPoint))),

                    new Decorator(nat => !ScriptHelpers.IsBossAlive("Verdan the Everliving") && !EventStarted && StyxWoW.Me.Location.Distance(startEventPoint) >= 3 && StyxWoW.Me == ScriptHelpers.Tank && !StyxWoW.Me.IsActuallyInCombat && eventNPC != null,
                        new Action(ret => Navigator.MoveTo(eventNPC.Location))),

                    new Decorator(nat => eventNPC != null && !ScriptHelpers.IsBossAlive("Verdan the Everliving") && !EventStarted && StyxWoW.Me.Location.Distance(startEventPoint) < 3 && StyxWoW.Me == ScriptHelpers.Tank && !StyxWoW.Me.IsActuallyInCombat,
                        new PrioritySelector(
                           new Decorator(nat => eventNPC.Location.Distance(startEventPoint) < 3,
                               new Sequence(
                                   new Action(nat => Logging.Write("Starting Event for Final Boss!")),
                                   new Action(nat => eventNPC.Interact()),
                                   new Action(nat => Lua.DoString("SelectGossipOption(1)")),
                                   new Action(nat => Lua.DoString("SelectGossipOption(1)")))),

                           new Decorator(nat => eventNPC.Location.Distance(startEventPoint) >= 3,
                               new Sequence(
                                   new Action(nat => Logging.Write("Starting True Event")),
                                   new Action(nat => EventStarted = true)))
                                   )),

                     new Decorator(nat => EventStarted && StyxWoW.Me.Location.Distance(eventNPC.Location) > 5 && StyxWoW.Me == ScriptHelpers.Tank && !StyxWoW.Me.IsActuallyInCombat,
                             new Action(ret => Navigator.MoveTo(eventNPC.Location))),

                     new Decorator(nat => EventStarted && StyxWoW.Me == ScriptHelpers.Tank && !StyxWoW.Me.IsActuallyInCombat && eventNPC == null,
                         new Sequence(
                             new Action(ret => EventStarted = false),
                             new Action(ret => Logging.Write("Failed event, Starting over")))),

                     new Decorator(nat => !StyxWoW.Me.IsActuallyInCombat && StyxWoW.Me == ScriptHelpers.Tank,
                        new ActionAlwaysSucceed())
                    
                    );
        }

        /*public override MoveResult MoveTo(WoWPoint location)
        {
            WoWPoint startMovePoint = new WoWPoint(-167.133, -70.24321, -69.87201);
            WoWPoint moveToPoint = new WoWPoint(-201.9103, -112.113, -71.32834);

            WoWPoint LastMovePoint = WoWPoint.Empty;
            var newMeshNavigator = (MeshNavigator)Navigator.NavigationProvider;

            if(newMeshNavigator.CurrentMovePath != null && newMeshNavigator.CurrentMovePath.Path.IsPartialPath)
                LastMovePoint = newMeshNavigator.CurrentMovePath.Path.Points[newMeshNavigator.CurrentMovePath.Path.Points.Length - 1];

            Logging.Write("Can Fully Navigate : " + Navigator.CanNavigateFully(StyxWoW.Me.Location, StyxWoW.Me.CurrentTarget.Location));

            // Override NavigationError

            if (LastMovePoint != WoWPoint.Empty)
                Logging.Write("" + LastMovePoint.X + " , " + LastMovePoint.Y + " , " + LastMovePoint.Z);

            if (WoWMovement.ClickToMoveInfo.IsUsing && WoWMovement.ClickToMoveInfo.ClickPos == moveToPoint)
                return MoveResult.Moved;

            if ((StyxWoW.Me.Location.Distance(startMovePoint) <= 10) && !Navigator.CanNavigateFully(StyxWoW.Me.Location, location))
            {
                WoWMovement.ClickToMove(moveToPoint);
                return MoveResult.Moved;
            }
            if ((StyxWoW.Me.Location.Distance(startMovePoint) > 10) && (StyxWoW.Me.Location.Distance(startMovePoint) < 30) && !Navigator.CanNavigateFully(StyxWoW.Me.Location, location) && !WoWMovement.ClickToMoveInfo.IsUsing)
            {
                return Navigator.MoveTo(startMovePoint);
            }

            return base.MoveTo(location);
        }*/

    }
}
