using System.Collections.Generic;
using Styx;
using Styx.Logic;
using Styx.Logic.Pathing;
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
    public class UnderbogHeroic : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 186; } }

        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            units.RemoveAll(
                unit =>
                    {
                        if (unit.Entry == 18105 && unit.Z < 70) // remove Ghaz'an if swiming around.
                            return true;
                        return false;
                    });
        }


        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var targetPriority in units)
            {
                switch (targetPriority.Object.Entry)
                {
                    case 17730: // Murkblood Healer
                        if (StyxWoW.Me.IsDps())
                            targetPriority.Score += 220;
                        break;
                }
            }
        }

        #endregion

        WoWUnit _hungarfen;

        [EncounterHandler(17770, "Hungarfen")]
        public Composite HungarfenEncounter()
        {
            return new PrioritySelector(
                ctx => _hungarfen = ctx as WoWUnit,
                // run from spores
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 15, 17990), // Underbog Mushroom
                ScriptHelpers.CreateSpreadOutLogic(ctx => true, () => _hungarfen.Location, 15, 30)
                );
        }

        private readonly WoWPoint _ghazanTankSpot = new WoWPoint(152.2157, -467.3924, 75.0878);
        private readonly WoWPoint _ghazanFollowerWaitSpot = new WoWPoint(162.3641, -445.7501, 72.43507);

        WoWUnit _ghazan;

        [EncounterHandler(18105, "Ghaz'an", Mode = CallBehaviorMode.Proximity, BossRange = 95)]
        public Composite GhazanEncounter()
        {

            return new PrioritySelector(
                ctx => _ghazan = ctx as WoWUnit,
                // pull boss. check if he's not in the water first.
                new Decorator(
                    ctx => StyxWoW.Me.IsTank() && _ghazan.Z > 50f,
                    new PrioritySelector(
                        new Decorator(
                            ctx => Targeting.Instance.FirstUnit == null,
                            new Action(ctx => Navigator.GetRunStatusFromMoveResult(Navigator.MoveTo(_ghazan.Location)))),
                        new Decorator(
                            ctx => Targeting.Instance.FirstUnit == _ghazan,
                            new PrioritySelector(
                                new Decorator(
                                    ctx => !StyxWoW.Me.Combat && _ghazan.DistanceSqr <= 30*30,
                                    new Sequence(
                                        ScriptHelpers.CreateCastRangedAbility(),
                                        ScriptHelpers.CreateMoveToContinue(() => _ghazanTankSpot))),
                                ScriptHelpers.CreateTankUnitAtLocation(() => _ghazanTankSpot, 5f))))),
                // wait for tank to move in place.
                ScriptHelpers.CreateWaitAtLocationUntilTankPulled(ctx => StyxWoW.Me.Location.DistanceSqr(_ghazanFollowerWaitSpot) < 15*15, () => _ghazanFollowerWaitSpot)
                );
        }


        [EncounterHandler(17826, "Swamplord Musel'ek", Mode = CallBehaviorMode.Proximity, BossRange = 100)]
        public Composite SwamplordMuselekEncounter()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateClearArea(() => new WoWPoint(214.0852, -131.6859, 27.32064), 50f, u => u.Entry == 17734) // kill the Underbog Lord
                );
        }

        [EncounterHandler(17882, "The Black Stalker")]
        public Composite TheBlackStalkerEncounter()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateSpreadOutLogic(ctx => true, () => ScriptHelpers.Tank.Location, 13, 30f) // spread out
                );
        }
    }
}