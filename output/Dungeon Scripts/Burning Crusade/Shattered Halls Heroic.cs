using System.Collections.Generic;
using System.Linq;
using Styx;
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
    public class ShatteredHallsHeroic : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 189; } }
        public override WoWPoint Entrance { get { return new WoWPoint(-310.2105, 3087.014, -3.916756); } }

        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            units.RemoveAll(
                u =>
                    { // remove low level units.
                        var unit = u as WoWUnit;
                        if (unit != null)
                        {
                            if (unit is WoWPlayer)
                                return true;

                            if ((unit.Entry == 17356 || unit.Entry == 17357) && !unit.Combat) // Creeping Ozzling
                                return true;

                            if (StyxWoW.Me.IsTank() && ScriptHelpers.IsBossAlive("Warchief Kargath Bladefist") && (unit.Entry == 17621 || unit.Entry == 17623 || unit.Entry == 17622))
                                // Heathen Guard, Reaver Guard, Sharpshooter Guard
                                return true;
                        }
                        return false;
                    });
        }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var priority in units)
            {
                var unit = priority.Object as WoWUnit;
                if (unit != null)
                {
                    if (StyxWoW.Me.IsDps())
                    {
                        if (unit.Entry == 17621 || unit.Entry == 17623 || unit.Entry == 17622) // Heathen Guard, Reaver Guard, Sharpshooter Guard
                            priority.Score += 400;
                    }
                }
            }
        }

        #endregion

        [EncounterHandler(17356, "Creeping Ozze")]
        [EncounterHandler(17357, "Creeping Ozzling")]
        public Composite CreepingOzzlingEncounter()
        {
            var tankLoc = new WoWPoint(180.5946, 227.8267, -18.55346);
            return new PrioritySelector(
                new Decorator(
                    ctx => StyxWoW.Me.IsTank() && ScriptHelpers.PartyIncludingMe.All(p => p.HealthPercent > 50) && !StyxWoW.Me.IsActuallyInCombat && StyxWoW.Me.Location.DistanceSqr(tankLoc) > 5,
                    new Action(ctx => Navigator.MoveTo(tankLoc))));
        }

        [ObjectHandler(182539, "Grand Warlock Chamber Door")]
        public Composite GrandWarlockChamberDoorHandler()
        {
            var roomBeforeFirstBossLoc = new WoWPoint(123.3501, 264.7724, -13.23036);
            var dropDownLoc = new WoWPoint(121.9095, 236.8019, -46.10165);
            var topOfJumpLoc = new WoWPoint(123.0873, 250.3539, -15.3936);
            return new PrioritySelector(
                new Decorator(
                    ctx => ((WoWGameObject) ctx).State == WoWGameObjectState.Ready && !StyxWoW.Me.Combat && StyxWoW.Me.Location.DistanceSqr(roomBeforeFirstBossLoc) <= 20*20 && StyxWoW.Me.Z > -25,
                    new PrioritySelector(
                        new Decorator(
                            ctx => StyxWoW.Me.IsTank() || (StyxWoW.Me.IsFollower() && ScriptHelpers.Tank != null && ScriptHelpers.Tank.Z < -25),
                            new PrioritySelector(
                                new Decorator(
                                    ctx => StyxWoW.Me.Location.DistanceSqr(topOfJumpLoc) > 4*4,
                                    new Action(ctx => Navigator.MoveTo(topOfJumpLoc))),
                                new Action(ctx => WoWMovement.ClickToMove(dropDownLoc))))))
                );
        }

        WoWUnit _grandWarlockNethekurse;

        [EncounterHandler(16807, "Grand Warlock Nethekurse")]
        public Composite GrandWarlockNethekurseEncounter()
        {
            return new PrioritySelector(
                ctx => _grandWarlockNethekurse = ctx as WoWUnit,
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 10, 17471), // Lesser Shadow Fissure
                ScriptHelpers.CreateRunAwayFromBad(ctx => StyxWoW.Me.IsFollower() && _grandWarlockNethekurse != null && _grandWarlockNethekurse.HasAura("Dark Spin"), 10, 16807),
                ScriptHelpers.CreateTankFaceAwayGroupUnit(10)
                );
        }

        [ObjectHandler(181915, "Blaze")]
        public Composite BlazeHandler() { return ScriptHelpers.CreateRunAwayFromBad(ctx => true, 10, 181915); }

        WoWUnit _warchiefKargathBladefist;

        [EncounterHandler(16808, "Warchief Kargath Bladefist")]
        public Composite WarchiefKargathBladefistEncounter()
        {
            var platformCenterLocation = new WoWPoint(231.25, -83.64489, 4.940088);
            return new PrioritySelector(
                ctx => _warchiefKargathBladefist = ctx as WoWUnit,
                ScriptHelpers.CreateSpreadOutLogic(ctx => Targeting.Instance.FirstUnit == _warchiefKargathBladefist, () => platformCenterLocation, 15, 25),
                ScriptHelpers.CreateTankUnitAtLocation(() => platformCenterLocation, 5)
                );
        }
    }
}