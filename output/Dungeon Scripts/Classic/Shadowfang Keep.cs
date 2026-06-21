using System;
using System.Collections.Generic;
using CommonBehaviors.Actions;
using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using System.Linq;
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
    public class ShadowfangKeep : Dungeon
    {
        #region Overrides of Dungeon

        /// <summary> The mapid of this dungeon. </summary>
        /// <value>The map identifier.</value>
        public override uint DungeonId
        {
            get { return 8; }
        }

        public override WoWPoint Entrance { get { return new WoWPoint(-233.2333, 1571, 76.88484); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(-229.73, 2107.91, 76.88484); } }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var t in units)
            {
                var prioObject = t.Object;

                //Trash
                if (prioObject.Entry == 47137)
                {
                    t.Score += 400;
                }

                //Baron Silverlain's Summoned adds need to die first
                if (prioObject.Entry == 50857 || prioObject.Entry == 50869 || prioObject.Entry == 50834 || prioObject.Entry == 50851)
                {
                    t.Score += 1000;
                }

                if (prioObject.Entry == 50561)
                {
                    t.Score += 1000;
                }

                //Godfrey ghouls are a top priorty
                if (prioObject.Entry == 50561)
                {
                    t.Score += 1000;
                }
            }
        }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            base.IncludeTargetsFilter(incomingunits, outgoingunits);

            foreach (var unit in incomingunits.Select(obj => obj.ToUnit()))
            {
                if (!StyxWoW.Me.Combat && ScriptHelpers.Tank == StyxWoW.Me)
                {
                    WoWPoint boss1OutsideStairsDontPullPoint = new WoWPoint(-158.4905, 2234.09, 84.42113);
                    WoWPoint boss2StairsDontPullPoint = new WoWPoint(-285.6845, 2326.824, 95.86655);
                    //Do not pull these mobs as the tank
                    if ((unit.Location.Distance(boss2StairsDontPullPoint) < 15) || (unit.Location.Distance(boss1OutsideStairsDontPullPoint) < 15))
                    {
                        continue;
                    }
                }
            }
        }

        #endregion

        #region Encounter Handlers

        [EncounterHandler(0)]
        public Composite RootLogic()
        {
            WoWPoint firstDoorWaitLocation = new WoWPoint(-241.9713, 2154.46, 90.62418);
            WoWPoint secondDoorWaitLocation = new WoWPoint(-134.5107, 2168.385, 128.9439);
            WoWPoint thirdDoorWaitLocation = new WoWPoint(-124.5004, 2164.421, 155.6786);

            WoWGameObject firstdoor = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(x => x.Entry == 18895);
            WoWGameObject seconddoor = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(x => x.Entry == 18972);
            WoWGameObject thirddoor = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(x => x.Entry == 18971);

            List<WoWUnit> listOfHostileMobsDoor3 = ObjectManager.GetObjectsOfType<WoWUnit>()
                                     .Where(unit => unit.IsAlive && unit.IsHostile && unit.Location.Distance(thirdDoorWaitLocation) < 15)
                                     .OrderBy(u => u.Distance)
                                     .ToList();


            return new PrioritySelector(
                new Decorator(nat => firstdoor != null && firstdoor.State == WoWGameObjectState.Ready && !ScriptHelpers.IsBossAlive("Baron Ashbury") && StyxWoW.Me == ScriptHelpers.Tank && firstdoor.Location.Distance(StyxWoW.Me.Location) < 20,
                    new PrioritySelector(
                        new Decorator(nat => StyxWoW.Me.Location.Distance(firstDoorWaitLocation) > 3,
                                      new Sequence(
                                          new Action(nat => TreeRoot.StatusText = "Moving to door Wait Location"),
                                          new Action(nat => Navigator.MoveTo(firstDoorWaitLocation)))),

                        new Decorator(nat => StyxWoW.Me.Location.Distance(firstDoorWaitLocation) < 3,
                                      new Sequence(
                                          new Action(nat =>TreeRoot.StatusText = "Waiting for First Door to Open"),
                                          new ActionAlwaysSucceed())))),

                new Decorator(nat => seconddoor != null && seconddoor.State == WoWGameObjectState.Ready && !ScriptHelpers.IsBossAlive("Lord Walden") && StyxWoW.Me == ScriptHelpers.Tank && seconddoor.Location.Distance(StyxWoW.Me.Location) < 20,
                    new PrioritySelector(
                        new Decorator(nat => StyxWoW.Me.Location.Distance(secondDoorWaitLocation) > 3,
                                      new Sequence(
                                          new Action(nat => TreeRoot.StatusText = "Moving to door Wait Location"),
                                          new Action(nat => Navigator.MoveTo(secondDoorWaitLocation)))),

                        new Decorator(nat => StyxWoW.Me.Location.Distance(secondDoorWaitLocation) < 3,
                                      new Sequence(
                                          new Action(nat => TreeRoot.StatusText = "Waiting for Second Door to Open"),
                                          new ActionAlwaysSucceed())))),

                new Decorator(nat => thirddoor != null && thirddoor.State == WoWGameObjectState.Ready && !ScriptHelpers.IsBossAlive("Lord Walden") && StyxWoW.Me == ScriptHelpers.Tank && thirddoor.Location.Distance(StyxWoW.Me.Location) < 20 && listOfHostileMobsDoor3.Count == 0,
                    new PrioritySelector(
                        new Decorator(nat => StyxWoW.Me.Location.Distance(thirdDoorWaitLocation) > 3,
                                      new Sequence(
                                          new Action(nat => TreeRoot.StatusText = "Moving to door Wait Location"),
                                          new Action(nat => Navigator.MoveTo(thirdDoorWaitLocation)))),

                        new Decorator(nat => StyxWoW.Me.Location.Distance(thirdDoorWaitLocation) < 3,
                                      new Sequence(
                                          new Action(nat => TreeRoot.StatusText = "Waiting for Third Door to Open"),
                                          new ActionAlwaysSucceed()))))


                );

        }

        [EncounterHandler(47138, Mode = CallBehaviorMode.Proximity)]
        public Composite PustulantMonstrosity()
        {
            return ScriptHelpers.CreateClearArea(() => new WoWPoint(-91.75407,2128.595,144.9211), 50, u => u.Entry == 47138);
        }

        [EncounterHandler(46962, "Baron Ashbury")]
        public Composite BaronAshburyFight()
        {
            //No scripting currently needed for Reg mode, will need it for Heroic
            return
                new PrioritySelector();
        }

        [EncounterHandler(3887, "Baron Silverlaine")]
        public Composite BaronSilverlaineFight()
        {
            //No scripting currently needed for Reg mode, will need it for Heroic. Targeting is done in TargetFilters Above
            return
                new PrioritySelector();
        }

        [EncounterHandler(4278, Mode = CallBehaviorMode.Proximity)]
        public Composite ClearCommanderRoom()
        {
            return ScriptHelpers.CreateClearArea(() => new WoWPoint(-248.4097, 2251.753, 100.8921), 20, u => u.Entry == 3877);
        }

        [EncounterHandler(4278, "Commander Springvale", Mode = CallBehaviorMode.Proximity)]
        public Composite CommanderSpringvaleFight()
        {
            return
                new PrioritySelector(
                        ScriptHelpers.CreateTankFaceAwayGroupUnit(10),
                        ScriptHelpers.CreateRunAwayFromBad(ctx => StyxWoW.Me.IsActuallyInCombat, 10f, u => u.Entry == 50503 && u.Distance <= 10)
                    );
        }

        [EncounterHandler(46963, "Lord Walden")]//3887
        public Composite LordWaldenFight()
        {
            WoWPoint centerOfRoom = new WoWPoint(-146.3959, 2172.847, 127.9531);
           //This function still needs to be scripted but its the only thing needed here so when it is scripted will just automatically work
            return
                new PrioritySelector(
                    new Decorator(nat => StyxWoW.Me.Location.Distance(centerOfRoom) > 15.5f, // If we are not even in the room then get in here!
                        new Action(nat => Navigator.MoveTo(centerOfRoom))),

                ScriptHelpers.CreateTankUnitAtLocation(() => centerOfRoom, 6f),
                ScriptHelpers.CreateSpreadOutLogic(ctx => StyxWoW.Me.IsActuallyInCombat, 5)
                
                );
        }

        [EncounterHandler(46964, "Lord Godfrey", Mode = CallBehaviorMode.CurrentBoss)]
        public Composite LordGodfreyFight()
        {
            List<WoWUnit> listOfHostileMob = ObjectManager.GetObjectsOfType<WoWUnit>()
                                     .Where(ret => ret.IsAlive && ret.Entry == 47138)
                                     .OrderBy(u => u.Distance)
                                     .ToList();
           WoWUnit boss = null;
            
            return
                new PrioritySelector(ctx => boss = ctx as WoWUnit,

                    ScriptHelpers.CreatePullNpcToLocation(nat => !ScriptHelpers.IsBossAlive("Lord Godfrey") && boss!= null && ScriptHelpers.Tank == StyxWoW.Me && StyxWoW.Me.Location.Distance(new WoWPoint(-94.86681, 2151.281, 144.9219)) < 30 && listOfHostileMob.Count == 0, () => boss, () => new WoWPoint(-94.86681, 2151.281, 144.9219), 7),

                    ScriptHelpers.GetBehindUnit(r => boss != null && boss.CastingSpellId == 93520 && !boss.IsPlayerBehind, () => boss),

                    new Decorator(r => boss != null && boss.CastingSpellId != 93520, ScriptHelpers.CreateTankUnitAtLocation(() => new WoWPoint(-98.13088, 2143.893, 144.921), 10f)),

                    ScriptHelpers.CreateTankFaceAwayGroupUnit(7));
        }


        #endregion
    }
}
