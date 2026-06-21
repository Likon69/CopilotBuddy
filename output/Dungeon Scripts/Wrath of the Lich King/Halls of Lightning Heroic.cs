using System;
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
using Action = TreeSharp.Action;

#if USE_DUNGEONBUDDY_DLL
using Bots.DungeonBuddyDll;
using Bots.DungeonBuddyDll.Profiles;
using Bots.DungeonBuddyDll.Attributes;
using Bots.DungeonBuddyDll.Helpers;
namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Wrath_of_the_Lich_King
#else
    using Bots.DungeonBuddy.Profiles;
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
namespace Bots.DungeonBuddy.Dungeon_Scripts.Wrath_of_the_Lich_King
#endif
{
    public class HallsOfLightningHeroic : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 212; } }
        public override WoWPoint Entrance { get { return new WoWPoint(9185.314, -1386.893, 1110.216); } }

        public override bool IsFlyingCorpseRun { get { return true; } }

        public override void OnEnter() { _volkhanCleartrashPath.CycleTo(_volkhanCleartrashPath.First); }

        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            units.RemoveAll(
                ret =>
                    {
                        if (ret.Entry == 28585 && !((WoWUnit) ret).Combat) // Slag
                            return true;
                        if (ret.Entry == 28926) //  Spark of Ionar
                            return true;
                        return false;
                    });
        }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            foreach (var obj in incomingunits)
            {
                var unit = obj as WoWUnit;
                if (unit != null)
                {
                }
            }
        }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var priority in units)
            {
                var unit = priority.Object as WoWUnit;
                if (unit != null)
                {
                    if (unit.Entry == 29240 && StyxWoW.Me.IsDps()) // Stormforged Lieutenant
                        priority.Score += 500;
                    if (unit.Entry == 28582 && StyxWoW.Me.IsDps()) // Stormforged Mender
                        priority.Score += 500;
                }
            }
        }

        #endregion

        [EncounterHandler(28586, "General Bjarngrim", Mode = CallBehaviorMode.Proximity, BossRange = 300)]
        public Composite GeneralBjarngrimEncounter()
        {
            WoWUnit boss = null;
            var runToPoint = new WoWPoint(1331.974, 101.1538, 40.18048);

            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit,
                // run from boss if tank and I'm fighting stuff or he's near adds that would aggro if boss is pulled.
                new Decorator(
                    ctx => StyxWoW.Me.IsTank() && !boss.Combat && boss.DistanceSqr <= 75*75 &&
                           (StyxWoW.Me.IsActuallyInCombat || ScriptHelpers.GetUnfriendlyNpsAtLocation(() => boss.Location, 50, u => u.Entry != 29240 && u != boss).Any()),
                    new Sequence(
                        ScriptHelpers.CreateMoveToContinue(ctx => boss.DistanceSqr <= 90*90, () => runToPoint, true))),
                new Decorator(
                    ctx => boss.Combat,
                    new PrioritySelector(
                        ScriptHelpers.CreateTankFaceAwayGroupUnit(10),
                        ScriptHelpers.CreateRunAwayFromBad(ctx => boss != null && !StyxWoW.Me.IsTank() && boss.HasAura("Whirlwind"), 12, 28586)))
                );
        }

        private readonly CircularQueue<WoWPoint> _volkhanCleartrashPath = new CircularQueue<WoWPoint>
                                                                              {
                                                                                  new WoWPoint(1361.804, -188.0309, 52.02539),
                                                                                  new WoWPoint(1366.668, -145.0833, 52.0296),
                                                                                  new WoWPoint(1362.529, -184.9581, 52.02528),
                                                                                  new WoWPoint(1303.465, -190.1832, 52.02384),
                                                                                  new WoWPoint(1299.959, -140.8916, 52.00739)
                                                                              };

        WoWUnit _volkhan;

        [EncounterHandler(28587, "Volkhan", Mode = CallBehaviorMode.Proximity, BossRange = 110)]
        public Composite VolkhanEncounter()
        {
            var roomCenterLoc = new WoWPoint(1331.7, -165.3561, 52.02283);

            return new PrioritySelector(
                ctx => _volkhan = ctx as WoWUnit,
                // clear the room while being careful not to aggro boss.
                new Decorator(
                    ctx =>
                    StyxWoW.Me.IsTank() && ScriptHelpers.GetUnfriendlyNpsAtLocation(() => roomCenterLoc, 51, u => u.Z >= 50 && u != _volkhan).Any() &&
                    (Targeting.Instance.FirstUnit == null || Targeting.Instance.FirstUnit.DistanceSqr > 25*25),
                    new PrioritySelector(
                        ctx => _volkhanCleartrashPath.Peek(),
                        new Decorator(
                            ctx => StyxWoW.Me.Location.Distance2DSqr((WoWPoint) ctx) < 5*5,
                            new Action(ctx => _volkhanCleartrashPath.Dequeue())),
                        new Action(ctx => Navigator.GetRunStatusFromMoveResult(Navigator.MoveTo((WoWPoint) ctx)))
                        )),
                ScriptHelpers.CreateRunAwayFromBad(ctx => (_volkhan.CastingSpellId == 59529 || _volkhan.CastingSpellId == 52237), () => _volkhan.Location, 35, 13, 28681) // brittle golem
                );
        }

        WoWUnit _ionar, _sparks;

        [EncounterHandler(28546, "Ionar", Mode = CallBehaviorMode.CurrentBoss)]
        public Composite IonarEncounter()
        {
            var tankLoc = new WoWPoint(1081.995, -261.8092, 61.20797);
            var runtoLoc = new WoWPoint(1177.047, -231.855, 52.36805);
            var disperseTimer = new WaitTimer(TimeSpan.FromSeconds(5));

            return new PrioritySelector(
                ctx =>
                    {
                        _sparks = ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == 28926).OrderBy(u => u.DistanceSqr).FirstOrDefault();
                        return _ionar = ctx as WoWUnit;
                    },
                new Action(
                    ctx =>
                        {
                            if (_ionar != null && _ionar.CastingSpellId == 52770)
                                disperseTimer.Reset();
                            return RunStatus.Failure;
                        }),
                new Decorator(
                    ctx => _ionar != null && !disperseTimer.IsFinished || (_sparks != null && _sparks.DistanceSqr <= 20*20),
                    new Action(ctx => Navigator.GetRunStatusFromMoveResult(Navigator.MoveTo(runtoLoc)))),
                new Decorator(
                    ctx => _sparks != null,
                    new ActionAlwaysSucceed()),
                // run from sparks.. 
                ScriptHelpers.CreateRunAwayFromBad(
                    ctx => StyxWoW.Me.IsFollower() && (StyxWoW.Me.HasAura("Static Overload") || StyxWoW.Me.PartyMembers.Any(p => p.HasAura("Static Overload") && p.DistanceSqr < 15*15)), 15,
                    u => u is WoWPlayer),
                new Decorator(
                    ctx => _ionar != null && _ionar.CurrentTarget == StyxWoW.Me,
                    ScriptHelpers.CreateTankUnitAtLocation(() => tankLoc, 7)),
                // for the statues that come alive..
                new Decorator(
                    ctx => StyxWoW.Me.IsTank() && (_ionar == null || !_ionar.Combat) && StyxWoW.Me.Combat && Targeting.Instance.FirstUnit == null && StyxWoW.Me.IsMoving,
                    new Action(ctx => WoWMovement.MoveStop()))
                );
        }

        WoWUnit _loken;

        [EncounterHandler(28923, "Loken")]
        public Composite LokenEncounter()
        {
            return new PrioritySelector(
                ctx => _loken = ctx as WoWUnit,
                ScriptHelpers.CreateRunAwayFromBad(ctx => _loken.CastingSpellId == 59835 || _loken.CastingSpellId == 52960, () => _loken.Location, 40, 35, 28923),
                new Decorator(
                    ctx => StyxWoW.Me.IsRange() && _loken.DistanceSqr > 13*13,
                    new Action(ctx => Navigator.GetRunStatusFromMoveResult(Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(StyxWoW.Me.Location, _loken.Location, 10)))))
                );
        }
    }
}