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
    public class TheNexusHeroic : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 226; } }

        public override WoWPoint Entrance { get { return new WoWPoint(3900.531, 6985.386, 69.4887); } }

        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            units.RemoveAll(
                obj =>
                    {
                        if (obj.Entry == 26763 && ((WoWUnit) obj).HasAura("Rift Shield")) // Anomalus
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
                    if (StyxWoW.Me.IsDps() && unit.Entry == 26918) // Chaotic Rift
                        priority.Score += 500;
                }
            }
        }

        #endregion

        [EncounterHandler(26731, "Grand Magus Telestra", Mode = CallBehaviorMode.Proximity, BossRange = 65)]
        public Composite GrandMagusTelestraEncounter()
        {
            var trashTankLoc = new WoWPoint(545.6187, 108.4139, -16.63844);
            var trashLoc = new WoWPoint(518.0992, 90.58775, -16.12559);

            WoWUnit boss = null, trash = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit,
                new Decorator(
                    ctx => !boss.Combat && !StyxWoW.Me.Combat,
                    new PrioritySelector(
                        ctx => trash = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => trashLoc, 10, u => true).FirstOrDefault(),
                        ScriptHelpers.CreatePullNpcToLocation(ctx => trash != null, () => trash, () => trashTankLoc, 10)))
                );
        }

        [EncounterHandler(26763, "Anomalus")]
        public Composite AnomalusEncounter()
        {
            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit
                );
        }

        [EncounterHandler(26794, "Ormorok the Tree-Shaper")]
        public Composite OrmorokTheTreeShaperEncounter()
        {
            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit,
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 10, 27079) // Crystal Spike Trigger
                );
        }

        [EncounterHandler(26723, "Keristrasza", Mode = CallBehaviorMode.Proximity)]
        public Composite KeristraszaEncounter()
        {
            var faceTimer = new Stopwatch();
            WoWGameObject sphere = null;
            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit,
                new Decorator(
                    ctx => !boss.Combat,
                    new PrioritySelector(
                        ctx => sphere =
                               ObjectManager.GetObjectsOfType<WoWGameObject>().Where(
                                   o => (o.Entry == 188526 || o.Entry == 188527 || o.Entry == 188528) && o.CanUse()).OrderBy(
                                       o => o.DistanceSqr).FirstOrDefault(),
                        // clear the room, 
                        ScriptHelpers.CreateClearArea(() => boss.Location, 60, u => u != boss),
                        // interact with the spheres.
                        new Decorator(
                            ctx => StyxWoW.Me.IsTank() && sphere != null && Targeting.Instance.FirstUnit == null,
                            ScriptHelpers.CreateInteractWithObject(() => sphere))
                        )),
                new Decorator(
                    ctx => boss.Combat,
                    new PrioritySelector(
                        ctx => { return ctx; },
                        new Decorator(
                            ctx => StyxWoW.Me.HasAura("Intense Cold") && StyxWoW.Me.Auras["Intense Cold"].StackCount >= 3,
                            new Sequence(
                                new Action(ctx => WoWMovement.Move(WoWMovement.MovementDirection.JumpAscend)),
                                new WaitContinue(1, ctx => false, new ActionAlwaysSucceed()),
                                new Action(ctx => WoWMovement.MoveStop(WoWMovement.MovementDirection.JumpAscend)))),
                        new Action(
                            ctx =>
                                {
                                    var bossToMeFacing = WoWMathHelper.CalculateNeededFacing(boss.Location, StyxWoW.Me.Location);
                                    var bossRotation = WoWMathHelper.NormalizeRadian(boss.Rotation);
                                    var facingDifference = WoWMathHelper.NormalizeRadian(bossToMeFacing - bossRotation);
                                    var moveTo = WoWMathHelper.CalculatePointAtSide(
                                        boss.Location, boss.Rotation, (float) boss.Distance, facingDifference > Math.PI);
                                    // make sure
                                    if (StyxWoW.Me.IsFollower() && !boss.IsCasting && StyxWoW.Me.Location.DistanceSqr(moveTo) > 4*4)
                                    {
                                        if (!faceTimer.IsRunning)
                                            faceTimer.Start();
                                        if (faceTimer.ElapsedMilliseconds > 2000)
                                            return Navigator.GetRunStatusFromMoveResult(Navigator.MoveTo(moveTo));
                                    }
                                    else if (faceTimer.IsRunning)
                                        faceTimer.Reset();
                                    return RunStatus.Failure;
                                })
                        ))
                );
        }
    }
}