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
    public class UpperBlackrockSpire : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 330; } }

        public override WoWPoint Entrance { get { return new WoWPoint(-7522.93, -1232.999, 285.74); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(77.55, -223.18, 49.84); } }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var p in units)
            {
                WoWUnit unit = p.Object.ToUnit();
                switch (unit.Entry)
                {
                    case 9818: // Blackhand Summoner
                    case 10442: // Chromatic Whelp
                        if (StyxWoW.Me.IsDps())
                            p.Score += 120;

                        break;
                }
            }
        }

        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            // let the dps kill these.
            units.RemoveAll(unit => unit.Entry == 10442 && StyxWoW.Me.IsTank());
        }

        #endregion

        [ObjectHandler(175244, "Emberseer In", ObjectRange = 120)]
        public Composite ClearHallToEmberSeerHandler()
        {
            // clear hall of argo.
            WoWGameObject door = null;
            return new PrioritySelector(
                ctx => door = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(u => u.IsValid && u.Entry == 175244),
                new Decorator(
                    ctx => door.State == WoWGameObjectState.Ready,
                    ScriptHelpers.CreateClearArea(() =>new WoWPoint(185.2658, -314.7129, 76.92092), 100, u => true)));
        }

        [ObjectHandler(175706, "Blackrock Altar", ObjectRange = 36)]
        public Composite BlackrockAltarHandler()
        {
            WoWGameObject entranceDoor = null;
            WoWUnit emberseer = null;
            return new PrioritySelector(
                ctx =>
                    {
                        entranceDoor = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(u => u.IsValid && u.Entry == 175244);
                        emberseer = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.IsValid && u.Entry == 9816);
                        return ctx;
                    },
                new Decorator(
                    ctx => StyxWoW.Me.Z > 80f && entranceDoor.State == WoWGameObjectState.Active && emberseer != null && emberseer.HasAura(15282),
                    ScriptHelpers.CreateInteractWithObject(175706, 8)),
                // wait for boss to spawn
                new Decorator(
                    ctx => emberseer != null && emberseer.IsAlive && !emberseer.HasAura(15282) && Targeting.Instance.FirstUnit == null,
                    new PrioritySelector(
                        new Decorator(
                            ctx => StyxWoW.Me.IsTank() && emberseer.DistanceSqr > 4*4,
                            new Action(ctx => Navigator.GetRunStatusFromMoveResult(Navigator.MoveTo(emberseer.Location)))),
                        new Decorator(
                            ctx => StyxWoW.Me.IsTank() && Targeting.Instance.FirstUnit == null && emberseer.DistanceSqr <= 4*4,
                            new ActionAlwaysSucceed())))
                );
        }

        private readonly WoWPoint _drakeTankSpot = new WoWPoint(149.3783, -420.6858, 110.4725);

        [ObjectHandler(175186, "Boss Gate", ObjectRange = 120)]
        public Composite DrakeHandler()
        {
            WoWUnit tank = null;
            WoWGameObject exitDoor = null;
            return new PrioritySelector(
                ctx => exitDoor = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(u => u.IsValid && u.Entry == 164726),
                new Decorator(
                    ctx => ((WoWGameObject) ctx).State == WoWGameObjectState.Ready && !ScriptHelpers.IsBossAlive("Pyroguard Emberseer"),
                    new PrioritySelector(
                        new Decorator(
                            ctx => exitDoor.State == WoWGameObjectState.Ready,
                            new PrioritySelector(
                                ctx => tank = ScriptHelpers.Tank,
                                // if we're a follow and stray too far from tank than move towards tank 
                                new Decorator(
                                    ctx => StyxWoW.Me.IsFollower() && tank != null && (tank.DistanceSqr > 15*15 || ObjectManager.GetObjectsOfType<WoWUnit>().Any(u => u.Aggro)),
                                    new Action(ctx => Navigator.GetRunStatusFromMoveResult(Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(StyxWoW.Me.Location, tank.Location, 4f))))),
                                // tank shit at one spot, don't get trapped on other side of the drake gate with no healer. 
                                new Decorator(
                                    ctx => StyxWoW.Me.IsTank(),
                                    new PrioritySelector(
                                        ScriptHelpers.CreateTankUnitAtLocation(() => _drakeTankSpot, 20f),

                                        new Decorator( // don't do anything while waiting for the waves.
                                            ctx => Targeting.Instance.FirstUnit == null,
                                            new ActionAlwaysSucceed())
                                        ))))
                        )));
        }

        [EncounterHandler(10339, "Gyth")]
        public Composite GythEncounter()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateTankFaceAwayGroupUnit(15));
        }

        [EncounterHandler(10429, "Warchief Rend Blackhand")]
        public Composite WarchiefRendBlackhandEncounter()
        {
            WoWUnit warchief = null;
            return new PrioritySelector(
                ctx => warchief = ctx as WoWUnit,
                new Decorator(
                    ctx => StyxWoW.Me.IsFollower(),
                    ScriptHelpers.CreateRunAwayFromBad(ctx =>warchief != null && warchief.HasAura("Whirlwind"),6f,10429)));
        }

        [EncounterHandler(10430, "The Beast")]
        public Composite TheBeastEncounter()
        {
            return new PrioritySelector(
                // tank against wall because of knockback.
                ScriptHelpers.CreateTankUnitAtLocation(() => new WoWPoint(70.44568, -541.3387, 110.9312), 5f));
        }


        [EncounterHandler(10363, "General Drakkisath")]
        public Composite GeneralDrakkisathEncounter()
        {
            return new PrioritySelector(
                // has a cleave
                ScriptHelpers.CreateTankFaceAwayGroupUnit(10));
        }

        //private readonly WoWPoint _drakeTankSpot = new WoWPoint(190.6837, -403.9853, 110.8693);
        //[ObjectHandler(175185, "Drake Gate")]
        //public Composite DrakeHandler()
        //{
        //    return new PrioritySelector(
        //        new Decorator(
        //            ctx => ScriptHelpers.IsBossAlive("Gyth") || ScriptHelpers.IsBossAlive("Warchief Rend Blackhand"),
        //            new PrioritySelector(
        //                new Decorator(
        //                    ctx => StyxWoW.Me.Location.DistanceSqr(_drakeTankSpot) > 4 * 4,
        //                    new Action(ctx => ScriptHelpers.ControlledMoveTo(_drakeTankSpot))),
        //                new Decorator(
        //                    ctx => ScriptHelpers.MovementEnabled && StyxWoW.Me.Location.DistanceSqr(_drakeTankSpot) <= 4 * 4,
        //                    new Action(ctx => ScriptHelpers.DisableMovement(() => (ScriptHelpers.IsBossAlive("Gyth") || ScriptHelpers.IsBossAlive("Warchief Rend Blackhand")) && StyxWoW.Me.IsAlive)))
        //                )));
        //}
    }
}