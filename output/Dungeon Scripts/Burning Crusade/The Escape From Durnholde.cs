using System.Collections.Generic;
using System.Linq;
using CommonBehaviors.Actions;
using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;

#if USE_DUNGEONBUDDY_DLL
    using Bots.DungeonBuddyDll;
    using Bots.DungeonBuddyDll.Attributes;
    using Bots.DungeonBuddyDll.Helpers;
namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Burning_Crusade
#else
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
namespace Bots.DungeonBuddy.Dungeon_Scripts.Burning_Crusade
#endif
{
    public class TheEscapeFromDurnholde : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 170; } }

        public override WoWPoint Entrance { get { return new WoWPoint(-8332.569, -4057.429, -207.7462); } }

        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            WoWUnit thrall = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Entry == 17876);
            units.RemoveAll(
                u =>
                    { // remove low level units.
                        var unit = u as WoWUnit;
                        if (unit == null)
                            return false;

                        if (StyxWoW.Me.Level - unit.Level > 5 && !unit.Combat)
                            return true;

                        if (thrall != null && unit.DistanceSqr > 25*25 && !unit.Combat)
                            return true;
                        return false;
                    });
        }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            foreach (var u in incomingunits)
            {
                var unit = u as WoWUnit;
                if (unit != null && unit.CurrentTarget != null && unit.CurrentTarget.Entry == 17876) // targeting Thrall
                    outgoingunits.Add(unit);
            }
        }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var priority in units)
            {
                var unit = priority.Object as WoWUnit;
                if (unit != null)
                {
                    if (unit.Entry == 17820 && StyxWoW.Me.IsRange()) // Durnholde Rifleman
                        priority.Score += 200;

                    if (unit.Entry == 17833) // Durnholde Warden
                        priority.Score += 210;

                    if (unit.CurrentTarget != null && unit.CurrentTarget.Entry == 17876) // targeting Thrall.
                        priority.Score += 250;
                }
            }
        }

        public override void OnEnter() { _epochHunterStep1Done = _epochHunterStep2Done = _epochHunterStep3Done = _villageOnFire = false; }

        #endregion

        private readonly CircularQueue<WoWPoint> _barrelLocations = new CircularQueue<WoWPoint>
                                                                        {
                                                                            new WoWPoint(2102.327, 113.4871, 53.25289),
                                                                            new WoWPoint(2166.457, 219.0035, 52.66649),
                                                                        };

        private readonly WoWPoint _villageLocation = new WoWPoint(2127.658, 180.1258, 69.28806);

        [EncounterHandler(18723, "Erozion", Mode = CallBehaviorMode.Proximity)]
        public Composite ErozionEncounter()
        {
            return new PrioritySelector(
                // get some bombs.
                new Decorator(
                    ctx => !StyxWoW.Me.BagItems.Any(i => i.IsValid && i.Entry == 25853),
                    ScriptHelpers.CreateTalkToNpc(18723)),
                // talk to the drake for a ride
                new Decorator(
                    ctx => StyxWoW.Me.Mounted,
                    new Action(ctx => Mount.Dismount())),
                ScriptHelpers.CreateTalkToNpc(18725)
                );
        }

        private bool _villageOnFire;

        private WoWUnit _lieutenantDrake;

        [EncounterHandler(17848, "Lieutenant Drake", Mode = CallBehaviorMode.CurrentBoss)]
        public Composite LieutenantDrakeEncounter()
        {
            WoWGameObject barrel = null;
            return new PrioritySelector(
                ctx =>
                    {
                        if (!_villageOnFire)
                            _villageOnFire = OnFire;
                        return _lieutenantDrake = ctx as WoWUnit;
                    },
                new Decorator(
                    ctx => _lieutenantDrake != null && _lieutenantDrake.Combat,
                    ScriptHelpers.CreateRunAwayFromBad(ctx => StyxWoW.Me.IsFollower() && _lieutenantDrake.HasAura("Whirlwind"), () => _lieutenantDrake.Location, 30, 10, 17848)),
                // go to each barrel
                new Decorator(
                    ctx => Targeting.Instance.FirstUnit == null && !_villageOnFire && StyxWoW.Me.Location.DistanceSqr(_villageLocation) <= 200*200,
                    new PrioritySelector(
                        ctx =>
                            {
                                barrel = ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => o.Entry == 182589 && o.CanUse()).OrderBy(o => o.DistanceSqr).FirstOrDefault();
                                return _barrelLocations.Peek();
                            },
                        new ActionSetActivity("Blowing up barrels"),
                        new Decorator(
                            ctx => barrel != null,
                            new Action(ctx => ScriptHelpers.MoveTankTo(barrel.Location))),
                        new Decorator(
                            ctx => barrel != null && barrel.DistanceSqr < 7*7 && Targeting.Instance.FirstUnit == null,
                            ScriptHelpers.CreateInteractWithObject(182589, 6)),
                        new Decorator(
                            ctx => barrel == null,
                            new PrioritySelector(
                                new Decorator(
                                    ctx => StyxWoW.Me.Location.Distance2DSqr((WoWPoint) ctx) <= 3*3,
                                    new Action(ctx => _barrelLocations.Dequeue())),
                                new Action(ctx => ScriptHelpers.MoveTankTo((WoWPoint) ctx)))))));
        }


        [EncounterHandler(17862, "Captain Skarloc", Mode = CallBehaviorMode.CurrentBoss)]
        public Composite CaptainSkarlocEncounter()
        {
            var thrallLoc = new WoWPoint(2230.14, 117.0324, 82.29137);
            var thrallEscortEndLoc = new WoWPoint(2063.042, 229.1765, 64.49027);

            return new PrioritySelector(
                ScriptHelpers.CreateTankTalkToThenEscortNpc(17876, 0, thrallLoc, thrallEscortEndLoc, 10, () => !ScriptHelpers.IsBossAlive("Captain Skarloc"))
                );
        }

        private bool _epochHunterStep1Done;
        private bool _epochHunterStep2Done;
        private bool _epochHunterStep3Done;
        private WoWUnit _thrall;
        private WoWUnit _epochHunter;
        private WoWUnit _taretha;

        [EncounterHandler(18096, "Epoch Hunter", Mode = CallBehaviorMode.CurrentBoss)]
        public Composite EpochHunterEncounter()
        {
            var thrallStartLoc1 = new WoWPoint(2063.042, 229.1765, 64.49027);
            var thrallStartLoc2 = new WoWPoint(2486.878, 624.3096, 57.90615);
            var thrallStartLoc3 = new WoWPoint(2660.485, 659.4092, 61.9358);
            var thrallEndLoc = new WoWPoint(2635.069, 673.2795, 54.46173);
            return new PrioritySelector(
                ctx =>
                    {
                        _thrall = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Entry == 17876);
                        _taretha = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Entry == 18887);
                        if (!_epochHunterStep1Done && _thrall != null)
                            _epochHunterStep1Done = _thrall.Location.DistanceSqr(thrallStartLoc2) < 5*5;
                        if (!_epochHunterStep2Done && _thrall != null)
                            _epochHunterStep2Done = _thrall.Location.DistanceSqr(thrallStartLoc3) < 5*5 && _taretha != null && _taretha.CanGossip;

                        if (!_epochHunterStep3Done && _thrall != null)
                            _epochHunterStep3Done = _thrall.Location.DistanceSqr(thrallEndLoc) < 5*5;

                        if (_thrall != null && _thrall.Dead)
                            _epochHunterStep2Done = false;

                        return _epochHunter = ctx as WoWUnit;
                    },
                new Decorator(
                    ctx => _epochHunter != null && _epochHunter.Combat,
                    ScriptHelpers.CreateTankFaceAwayGroupUnit(20)),
                new Decorator(
                    ctx => !_epochHunterStep1Done,
                    new PrioritySelector(
                        new Decorator(
                            ctx => StyxWoW.Me.Location.DistanceSqr(thrallStartLoc1) < 5*5 && _thrall == null,
                            new Action(ctx => _epochHunterStep1Done = true)),
                        ScriptHelpers.CreateTankTalkToThenEscortNpc(17876, 0, thrallStartLoc1, thrallStartLoc2, 10, () => true))),
                new Decorator(
                    ctx => !_epochHunterStep2Done && _epochHunterStep1Done,
                    ScriptHelpers.CreateTankTalkToThenEscortNpc(17876, 0, thrallStartLoc2, thrallStartLoc3, 3, () => _taretha != null && _taretha.CanGossip)),
                new Decorator(
                    ctx => !_epochHunterStep3Done && _epochHunterStep2Done && _epochHunterStep1Done,
                    new PrioritySelector(
                        new Decorator(
                            ctx => StyxWoW.Me.Location.DistanceSqr(thrallStartLoc3) < 5*5 && _thrall == null,
                            new Action(ctx => _epochHunterStep2Done = false)),
                        new Decorator(
                            ctx => _taretha != null && _taretha.CanGossip,
                            new Sequence(
                                ScriptHelpers.CreateTalkToNpcContinue(18887),
                                ScriptHelpers.CreateTalkToNpcContinue(18887))),
                        new Decorator(
                            ctx => _taretha != null && !_taretha.CanGossip,
                            ScriptHelpers.CreateTankTalkToThenEscortNpc(17876, 0, thrallStartLoc3, thrallEndLoc, 3, () => !ScriptHelpers.IsBossAlive("Epoch Hunter")))))
                );
        }

        private bool OnFire { get { return ObjectManager.GetObjectsOfType<WoWGameObject>().Any(o => o.IsValid && o.Entry == 182592); } } // Roaring Flame
    }
}