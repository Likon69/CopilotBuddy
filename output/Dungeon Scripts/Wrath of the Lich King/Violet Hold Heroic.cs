using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    using Bots.DungeonBuddyDll.Enums;
    using Bots.DungeonBuddyDll.Profiles;
    using Bots.DungeonBuddyDll.Attributes;
    using Bots.DungeonBuddyDll.Helpers;

    namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Wrath_of_the_Lich_King
#else
    using Bots.DungeonBuddy;
    using Bots.DungeonBuddy.Enums;
    using Bots.DungeonBuddy.Profiles;
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
    namespace Bots.DungeonBuddy.Dungeon_Scripts.Wrath_of_the_Lich_King
#endif
{
    public class VioletHoldHeroic : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 221; } }
        public override WoWPoint Entrance { get { return new WoWPoint(5676.366, 481.6799, 652.2564); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(1797.077f, 804.2758f, 44.36467f); } }

        public override void OnExit() { ScriptHelpers.EventInProcess = false; }

        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            units.RemoveAll(
                ret =>
                    {
                        if (ret.Entry == 29271) // Ethereal Sphere
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
                    if (StyxWoW.Me.IsTank() && unit.IsHostile && !unit.IsCritter)
                        outgoingunits.Add(unit);
                    if (unit.Entry == 29364 && unit.CanSelect && unit.Attackable) // Void Sentry
                        outgoingunits.Add(unit);
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
                    if (unit.Entry == 29364) // Void Sentry
                        priority.Score += 500;

                    if (StyxWoW.Me.IsTank() && !unit.TaggedByMe)
                        priority.Score += 100;
                }
            }
        }

        #endregion

        [ObjectHandler(191723, "Prison Seal", ObjectRange = 200)]
        public Composite PrisonSealHandler()
        {
            var roomCenterLoc = new WoWPoint(1883.929, 802.645, 38.41035);
            var sinclariStartingLoc = new WoWPoint(1830.95, 799.463, 44.33469);
            WoWGameObject seal = null;
            WoWUnit portal = null, sinclari = null;
            ;

            return new PrioritySelector(
                ctx =>
                    {
                        sinclari = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Entry == 30658);
                        portal =
                            ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == 30679 || u.Entry == 32174).
                                OrderBy(u => u.DistanceSqr).FirstOrDefault();
                        return seal = ctx as WoWGameObject;
                    },
                new Decorator(
                    ctx => sinclari != null && sinclari.Location.DistanceSqr(sinclariStartingLoc) <= 3*3,
                    //
                    new PrioritySelector(
                        new Decorator(
                            ctx => ScriptHelpers.EventInProcess,
                            new Action(ctx => ScriptHelpers.EventInProcess = false)),
                        new Decorator(
                            ctx => StyxWoW.Me.X < 1822,
                            new Action(
                                ctx => // step inside the room to not get locked out.
                                    {
                                        var loc = StyxWoW.Me.Location;
                                        loc.X = 1830;
                                        WoWMovement.ClickToMove(loc);
                                    })),
                        new Decorator(
                            ctx => StyxWoW.Me.IsTank(),
                            new Sequence(
                                new Decorator(
                                    ctx => ScriptHelpers.PartyIncludingMe.Count() == 5 || DungeonBuddySettings.Instance.QueueType == QueueType.SoloFarm,
                                    new Sequence(
                                        ScriptHelpers.CreateTalkToNpc(30658),
                                        new WaitContinue(2, ctx => false, new ActionAlwaysSucceed()),
                                        ScriptHelpers.CreateTalkToNpc(30658),
                                        new WaitContinue(15, ctx => false, new ActionAlwaysSucceed()))))))),
                new Decorator(
                    ctx =>
                    ScriptHelpers.IsBossAlive("Cyanigosa") &&
                    (seal.State == WoWGameObjectState.Ready ||
                     (sinclari != null && sinclari.Location.DistanceSqr(sinclariStartingLoc) > 3*3)),
                    new PrioritySelector(
                        new Decorator(
                            ctx => !ScriptHelpers.EventInProcess,
                            new Action(ctx => ScriptHelpers.EventInProcess = true)),
                        new Decorator(
                            ctx => ScriptHelpers.GetUnfriendlyNpsAtLocation(() => roomCenterLoc, 200, u => true).Any(),
                            ScriptHelpers.CreateClearArea(() => roomCenterLoc, 200, u => true)),
                        new Decorator(
                            ctx =>
                            Targeting.Instance.FirstUnit == null &&
                            !ScriptHelpers.GetUnfriendlyNpsAtLocation(() => roomCenterLoc, 200, u => true).Any(),
                            new PrioritySelector(
                                new Decorator(
                                    ctx => portal != null && StyxWoW.Me.Location.DistanceSqr(portal.Location) > 10*10,
                                    new Action(ctx => ScriptHelpers.MoveTankTo(portal.Location))),
                                new Decorator(
                                    ctx =>
                                    portal != null && StyxWoW.Me.IsTank() &&
                                    StyxWoW.Me.Location.DistanceSqr(portal.Location) <= 10*10,
                                    new ActionAlwaysSucceed()),
                                new Decorator(
                                    ctx => portal == null && StyxWoW.Me.Location.DistanceSqr(roomCenterLoc) > 15*15,
                                    new Action(ctx => ScriptHelpers.MoveTankTo(roomCenterLoc))),
                                new Decorator(
                                    ctx =>
                                    portal == null && StyxWoW.Me.IsTank() &&
                                    StyxWoW.Me.Location.DistanceSqr(roomCenterLoc) <= 15*15,
                                    new ActionAlwaysSucceed()))))
                    ));
        }

        [EncounterHandler(29315, "Erekem")]
        public Composite ErekemEncounter()
        {
            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit
                );
        }

        [EncounterHandler(29316, "Moragg")]
        public Composite MoraggEncounter()
        {
            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit
                );
        }

        [EncounterHandler(29313, "Ichoron")]
        public Composite IchoronEncounter()
        {
            WoWUnit boss = null;
            WoWGameObject activationCrystal = null;

            return new PrioritySelector(
                ctx =>
                    {
                        activationCrystal =
                            ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => o.Entry == 193611 && o.CanUse()).
                                OrderBy(o => o.DistanceSqr).FirstOrDefault();
                        return boss = ctx as WoWUnit;
                    },
                new Decorator(
                    ctx => boss.HasAura("Drained") && activationCrystal != null,
                    ScriptHelpers.CreateInteractWithObject(() => activationCrystal, 0, true))
                );
        }

        [EncounterHandler(29266, "Xevozz")]
        public Composite XevozzEncounter()
        {
            WoWUnit boss = null;
            var tankLoc = new WoWPoint(1937.878, 793.6298, 52.41381);

            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit,
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 16, 29271),
                // Ethereal Sphere
                new Decorator(
                    ctx => StyxWoW.Me.IsTank() && boss.CurrentTarget == StyxWoW.Me,
                    ScriptHelpers.CreateTankUnitAtLocation(() => tankLoc, 6))
                );
        }

        [EncounterHandler(29312, "Lavanthor")]
        public Composite LavanthorEncounter()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 14, 191457),
                // Lava bomb
                ScriptHelpers.CreateTankFaceAwayGroupUnit(14)
                );
        }

        [EncounterHandler(29314, "Zuramat the Obliterator")]
        public Composite ZuramatTheObliteratorEncounter()
        {
            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit,
                ScriptHelpers.CreateDispellEnemy("Shroud of Darkness", ScriptHelpers.EnemyDispellType.Magic, () => boss)
                );
        }

        [EncounterHandler(31134, "Cyanigosa")]
        public Composite CyanigosaEncounter()
        {
            WoWUnit boss = null;
            var faceTimer = new Stopwatch();

            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit,
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 14, 59369, 59369, 58693),
                new Decorator(
                    ctx => !StyxWoW.Me.IsTank(),
                    new Action(
                        ctx =>
                            {
                                var bossToMeFacing = WoWMathHelper.CalculateNeededFacing(
                                    boss.Location, StyxWoW.Me.Location);
                                var bossRotation = WoWMathHelper.NormalizeRadian(boss.Rotation);
                                var facingDifference = WoWMathHelper.NormalizeRadian(bossToMeFacing - bossRotation);

                                var moveTo = WoWMathHelper.CalculatePointAtSide(
                                    boss.Location,
                                    boss.Rotation,
                                    (float) boss.Distance,
                                    facingDifference > Math.PI);
                                if (!Navigator.CanNavigateFully(StyxWoW.Me.Location, moveTo))
                                {
                                    moveTo = WoWMathHelper.CalculatePointAtSide(
                                        boss.Location,
                                        boss.Rotation,
                                        4f,
                                        facingDifference > Math.PI);
                                }
                                if (StyxWoW.Me.Location.DistanceSqr(moveTo) >
                                    Navigator.PathPrecision*Navigator.PathPrecision)
                                {
                                    return Navigator.GetRunStatusFromMoveResult(Navigator.MoveTo(moveTo));
                                }
                                return RunStatus.Failure;
                            }))
                );
        }
    }
}