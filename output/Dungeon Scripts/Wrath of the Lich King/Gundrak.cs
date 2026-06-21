using System.Collections.Generic;
using System.Linq;
using CommonBehaviors.Actions;
using Styx;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;

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
    public class Gundrak : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 216; } }

        public override WoWPoint Entrance { get { return new WoWPoint(6971.443, -4399.999, 441.5751); } }

        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            units.RemoveAll(
                ret =>
                    {
                        if (ret.Entry == 29834 && !ret.ToUnit().Combat)
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
                    if (unit.Entry == 29742)
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
                    if (unit.Entry == 29742 && StyxWoW.Me.IsDps())
                        priority.Score += 500; // kill the snake wrap...
                }
            }
        }

        #endregion
        WoWUnit _sladran;

        [EncounterHandler(29304, "Slad'ran")]
        public Composite SladranEncounter()
        {
            return new PrioritySelector(
                ctx => _sladran = ctx as WoWUnit,
                // run from frost nova.
                ScriptHelpers.CreateRunAwayFromBad(ctx => _sladran.CastingSpellId == 59842 || _sladran.CastingSpellId == 55081, () => _sladran.Location, 35, 20, 29304)
                );
        }

        [ObjectHandler(192518, "Altar of Slad'ran", ObjectRange = 2000)]
        public Composite AltarofSladranHandler()
        {
            WoWGameObject altar = null;
            return new PrioritySelector(
                ctx => altar = ctx as WoWGameObject,
                new Decorator(
                    ctx => altar.CanUse() && !ScriptHelpers.IsBossAlive("Slad'ran") && Targeting.Instance.FirstUnit == null,
                    new PrioritySelector(
                        new Decorator(
                            ctx => altar.DistanceSqr > 5 * 5,
                            new TreeSharp.Action(ctx => ScriptHelpers.MoveTankTo(altar.Location))),
                        new Decorator(
                            ctx => altar.DistanceSqr <= 5 * 5,
                            ScriptHelpers.CreateInteractWithObject(() => altar)))));
        }

        [EncounterHandler(29307, "Drakkari Colossus")]
        public Composite DrakkariColossusEncounter()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 5, 55627) // run from mojo puddles.
                );
        }

        [ObjectHandler(192520, "Altar of the Drakkari Colossus", ObjectRange = 2000)]
        public Composite AltaroftheDrakkariColossusHandler()
        {
            WoWGameObject altar = null;
            return new PrioritySelector(
                ctx => altar = ctx as WoWGameObject,
                new Decorator(
                    ctx => altar.CanUse() && !ScriptHelpers.IsBossAlive("Drakkari Colossus") && Targeting.Instance.FirstUnit == null,
                    new PrioritySelector(
                        new Decorator(
                            ctx => altar.DistanceSqr > 5 * 5,
                            new TreeSharp.Action(ctx => ScriptHelpers.MoveTankTo(altar.Location))),
                        new Decorator(
                            ctx => altar.DistanceSqr <= 5 * 5,
                            ScriptHelpers.CreateInteractWithObject(() => altar)))));
        }

        /*

        [ObjectHandler(193188, "Bridge Boss")]
        public Composite GunDrakTrapdoor01Handler() // handles moving across the draw bridge as it's not meshed...
        {
            var moveToLoc1 = new WoWPoint(1742.854, 743.9608, 118.7645);
            var moveToLoc2 = new WoWPoint(1800.74, 743.6006, 119.1867);
            var trashLoc = new WoWPoint(1777.252, 743.6567, 119.4397);
            var trashPullLoc = new WoWPoint(1749.057, 743.1019, 118.879);

            WoWUnit trash = null;

            return new PrioritySelector(
                new Decorator(
                    ctx => StyxWoW.Me.Location.DistanceSqr(moveToLoc1) <= 8*8,
                    new PrioritySelector(
                        ctx =>
                        trash =
                        ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => (u.Entry == 29982 || u.Entry == 29931) && u.IsAlive && u.Location.Distance2DSqr(trashLoc) < 35*35).OrderBy(u => u.DistanceSqr).
                            FirstOrDefault(),
                        new Decorator(
                            ctx => trash != null && StyxWoW.Me.IsTank(),
                            new Sequence(
                                new Action(
                                    ctx =>
                                        {
                                            if (!StyxWoW.Me.IsAlive)
                                                return RunStatus.Failure;
                                            if (StyxWoW.Me.Location.DistanceSqr(trashPullLoc) > 2*2)
                                            {
                                                WoWMovement.ClickToMove(trashPullLoc);
                                                return RunStatus.Running;
                                            }
                                            return RunStatus.Success;
                                        }),
                                new Action(ctx => trash.Target()),
                                new WaitContinue(2, ctx => StyxWoW.Me.Combat || StyxWoW.Me.CurrentTarget == trash, new ActionAlwaysSucceed()),
                                new DecoratorContinue(
                                    ctx => !StyxWoW.Me.Combat,
                                    ScriptHelpers.CreateCastTankRangedAbility())
                                )),
                        new Decorator(
                            ctx => trash == null,
                            new PrioritySelector(
                                new Sequence(
                                    new Action(
                                        ctx =>
                                            {
                                                if (!StyxWoW.Me.IsAlive)
                                                    return RunStatus.Failure;
                                                if (StyxWoW.Me.IsAlive && StyxWoW.Me.Location.DistanceSqr(moveToLoc1) > 2*2)
                                                {
                                                    WoWMovement.ClickToMove(moveToLoc1);
                                                    return RunStatus.Running;
                                                }
                                                return RunStatus.Success;
                                            }),
                                    new Action(
                                        ctx =>
                                            {
                                                if (!StyxWoW.Me.IsAlive)
                                                    return RunStatus.Failure;

                                                if (StyxWoW.Me.IsFollower() && ScriptHelpers.Tank != null && ScriptHelpers.Tank.X < 1810)
                                                {
                                                    WoWMovement.ClickToMove(ScriptHelpers.Tank.Location);
                                                    return RunStatus.Running;
                                                }

                                                if (StyxWoW.Me.IsTank() && StyxWoW.Me.Location.DistanceSqr(moveToLoc2) > 2*2)
                                                {
                                                    WoWMovement.ClickToMove(moveToLoc2);
                                                    return RunStatus.Running;
                                                }
                                                return RunStatus.Success;
                                            })))
                            ))));
        }
        */

        [EncounterHandler(29305, "Moorabi")]
        public Composite MoorabiEncounter()
        {
            return new PrioritySelector(
                );
        }


        [ObjectHandler(192519, "Altar of Moorabi", ObjectRange = 2000)]
        public Composite AltarofMoorabiHandler()
        {
            WoWGameObject altar = null;
            return new PrioritySelector(
                ctx => altar = ctx as WoWGameObject,
                new Decorator(
                    ctx => altar.CanUse() && !ScriptHelpers.IsBossAlive("Moorabi") && Targeting.Instance.FirstUnit == null,
                    new PrioritySelector(
                        new Decorator(
                            ctx => altar.DistanceSqr > 5 * 5,
                            new TreeSharp.Action(ctx => ScriptHelpers.MoveTankTo(altar.Location))),
                        new Decorator(
                            ctx => altar.DistanceSqr <= 5 * 5,
                            ScriptHelpers.CreateInteractWithObject(() => altar)))));
        }

        WoWUnit _galdarah;

        [EncounterHandler(29306, "Gal'darah")]
        public Composite GaldarahEncounter()
        {
            return new PrioritySelector(
                ctx => _galdarah = ctx as WoWUnit, // gains Transform (AuraId: 55297) aura when in Rhino mode.
                ScriptHelpers.CreateRunAwayFromBad(ctx => _galdarah != null && (_galdarah.HasAura("Whirling Slash")) || StyxWoW.Me.IsRange(), 15, 29306)
                );
        }
    }
}