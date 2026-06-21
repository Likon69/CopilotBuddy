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
    public class UtgardePinnacle : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 203; } }

        public override WoWPoint Entrance { get { return new WoWPoint(1234.11, -4860.151, 218.2939); } }

        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            units.RemoveAll(
                ret =>
                    {
                        if (ret.Entry == 26667) // Dragonflayer Spectator
                            return true;
                        if (ret.Entry == 26893) // grauf
                            return true;
                        if (ret.Entry == 26693 && ret.ToUnit().HasAura("Ride Vehicle")) // Skadi
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
                    if (unit.Entry == 27281) // Ritual Channeler
                        priority.Score += 500;
                    if (unit.Entry == 26554 && StyxWoW.Me.IsDps()) // seer
                        priority.Score += 500;
                }
            }
        }

        #endregion

        WoWUnit _sorrowgrave, _svala;

        [EncounterHandler(26668, "Svala Sorrowgrave", Mode = CallBehaviorMode.CurrentBoss)]
        public Composite SvalaSorrowgraveEncounter()
        {

            return new PrioritySelector(
                ctx =>
                {
                    _svala = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Entry == 29281);
                    return _sorrowgrave = ctx as WoWUnit;
                },
                // wait for the bitch to spawn. 
                new Decorator(
                    ctx => ((_svala != null && _svala.DistanceSqr <= 10 * 10) || (_sorrowgrave != null && !_sorrowgrave.Attackable && _sorrowgrave.DistanceSqr <= 10 * 10)) && StyxWoW.Me.IsTank() && !StyxWoW.Me.IsActuallyInCombat,
                    new ActionAlwaysSucceed()),
                // run from ritual if all the channelers are dead..
                new Decorator(
                    ctx => _sorrowgrave != null && _sorrowgrave.Combat,
                    new PrioritySelector(
                        ScriptHelpers.CreateRunAwayFromBad(ctx => !StyxWoW.Me.IsTank() && _sorrowgrave != null && !ScriptHelpers.GetUnfriendlyNpsAtLocation(() => _sorrowgrave.Location, 30, u => u.Entry == 27281).Any(), 13, 27327) // Ritual Target
                        )));
        }

        [ObjectHandler(188593, "Stasis Generator")]
        public Composite StasisGeneratorHandler()
        {
            WoWGameObject generator = null;
            return new PrioritySelector(
                ctx => generator = ctx as WoWGameObject,
                new Decorator(
                    ctx => StyxWoW.Me.IsTank() && Targeting.Instance.FirstUnit == null && generator.CanUse(),
                    ScriptHelpers.CreateInteractWithObject(() => generator, 12, false))
                );
        }

        [EncounterHandler(26685, "Massive Jormungar")]
        public Composite MassiveJormungarEncounter()
        {
            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit,
                ScriptHelpers.CreateTankFaceAwayGroupUnit(13) // frontal spray. 
                );
        }

        [EncounterHandler(26687, "Gortok Palehoof")]
        public Composite GortokPalehoofEncounter()
        {
            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit,
                ScriptHelpers.CreateTankFaceAwayGroupUnit(13) // cleave 
                );
        }

        private WoWUnit _skadi = null;

        [EncounterHandler(26693, "Skadi the Ruthless", Mode = CallBehaviorMode.CurrentBoss)]
        public Composite SkadiTheRuthlessEncounter()
        {
            WoWUnit grauf = null;
            WoWDynamicObject freezingCloud = null;
            WoWGameObject harpoon = null, harpoonLancher = null;
            var throwHarpoonsLoc = new WoWPoint(520.4827, -541.5633, 119.8416);
            var tankSpot = new WoWPoint(486.1297, -515.4338, 104.723);


            return new PrioritySelector(
                ctx => _skadi = ctx as WoWUnit,
                // boss is in the air
                new Decorator(
                // boss is flying around and sometimes drops out of object manager
                    ctx => (_skadi != null && _skadi.HasAura("Ride Vehicle")) || (_skadi == null && StyxWoW.Me.Y < -475),
                    new PrioritySelector(
                        ctx =>
                        {
                            freezingCloud = ObjectManager.GetObjectsOfType<WoWDynamicObject>().FirstOrDefault(o => o.Entry == 47579);
                            harpoon = ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => o.Entry == 192539).OrderBy(o => o.DistanceSqr).FirstOrDefault();
                            harpoonLancher = ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => o.Entry == 192176 && o.CanUse()).OrderBy(o => o.DistanceSqr).FirstOrDefault();
                            grauf = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Entry == 26893 && u.IsAlive);
                            return ctx;
                        },
                // freezing cloud is on west/left side.
                        new Decorator(
                            ctx => freezingCloud != null && freezingCloud.Y >= -512 && StyxWoW.Me.Y > -516 && StyxWoW.Me.Y < -492,
                            new TreeSharp.Action(
                                ctx =>
                                {
                                    var moveToLoc = StyxWoW.Me.Location;
                                    moveToLoc.Y = -516.5f;
                                    WoWMovement.ClickToMove(moveToLoc);
                                })),
                // freezing cloud is on east/right side
                        new Decorator(
                            ctx => freezingCloud != null && freezingCloud.Y < -512 && StyxWoW.Me.Y < -510.5,
                            new TreeSharp.Action(
                                ctx =>
                                {
                                    var moveToLoc = StyxWoW.Me.Location;
                                    moveToLoc.Y = -510f;
                                    WoWMovement.ClickToMove(moveToLoc);
                                })),
                // only have the dps pick up the harpoons and only when there is no freezing cloud on ground.
                        new Decorator(
                            ctx => harpoon != null && freezingCloud == null && StyxWoW.Me.IsDps(),
                            ScriptHelpers.CreateInteractWithObject(() => harpoon, 0, true)),
                // check if shadi is in front of the harpoons and if we have any harpoons..
                        new Decorator(
                            ctx => grauf != null && grauf.Location.DistanceSqr(throwHarpoonsLoc) <= 20 * 20 && harpoonLancher != null && StyxWoW.Me.BagItems.Any(i => i.Entry == 37372),
                            ScriptHelpers.CreateInteractWithObject(() => harpoonLancher)),
                // move towards the end of the gauntlet
                        new Decorator(
                            ctx => StyxWoW.Me.IsTank() && StyxWoW.Me.Location.DistanceSqr(tankSpot) > 15 * 15,
                            new TreeSharp.Action(ctx => ScriptHelpers.MoveTankTo(tankSpot)))
                        )),
                // boss is on the ground. 
                new Decorator(
                    ctx => _skadi != null && !_skadi.HasAura("Ride Vehicle"),
                    new PrioritySelector(
                // run away from skadi during whirlwind if not tank or if ranged stay out .
                        ScriptHelpers.CreateRunAwayFromBad(ctx => _skadi != null && (_skadi.HasAura("Whirlwind") && !StyxWoW.Me.IsTank()) || StyxWoW.Me.IsRange(), 15, 26693),
                        new Decorator(
                            ctx => StyxWoW.Me.IsTank() && _skadi.CurrentTarget == StyxWoW.Me,
                            ScriptHelpers.CreateTankUnitAtLocation(() => tankSpot, 5))))
                );
        }

        [EncounterHandler(26861, "King Ymiron")]
        public Composite KingYmironEncounter()
        {
            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit,
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 15, 27339), // Spirit Fount
                ScriptHelpers.CreateDispellEnemy("Bane",ScriptHelpers.EnemyDispellType.Magic, () => boss));
        }
    }
}