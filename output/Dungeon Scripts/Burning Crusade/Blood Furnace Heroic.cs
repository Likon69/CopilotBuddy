using System.Collections.Generic;
using System.Linq;

using Styx;
using Styx.Logic;
using Styx.Logic.POI;
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
    public class BloodFurnaceHeroic : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 187; } }

        /// <summary>
        ///   The entrance of this dungeon as a <see cref="WoWPoint" /> .
        /// </summary>
        /// <value> The entrance. </value>
        public override WoWPoint Entrance { get { return new WoWPoint(-311.6548, 3175.894, 27.29512); } }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var targetPriority in units)
            {
                switch (targetPriority.Object.Entry)
                {
                    case 17395: // Shadowmoon Summoner
                        if (StyxWoW.Me.IsDps())
                            targetPriority.Score += 120;
                        break;
                }
            }
        }

        public override void RemoveTargetsFilter(List<WoWObject> units) { units.RemoveAll(unit => unit is WoWPlayer); } // don't kill MCed party members.

        #endregion

        [ObjectHandler(181877, "Proximity Bomb", ObjectRange = 15)]
        public Composite ProximityBombHandler() { return ScriptHelpers.CreateRunAwayFromBad(ctx => true, 5f, 181877); }

        private readonly WoWPoint _tankPoint = new WoWPoint(455.4293, 95.4829, 9.613952);

        [ObjectHandler(181982, "Cell Door Lever", ObjectRange = 100)]
        public Composite CellDoorLeverHandler()
        {
            WoWGameObject entranceDoor = null, lever = null;
            return new PrioritySelector(
                ctx =>
                    {
                        entranceDoor = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(o => o.Entry == 181822);
                        return lever = ctx as WoWGameObject;
                    },
                new Decorator(
                    ctx => lever.State == WoWGameObjectState.Ready && ScriptHelpers.IsBossAlive("Broggok"),
                    new PrioritySelector(
                        // clear the area before we start the event
                        ScriptHelpers.CreateClearArea(() => _tankPoint, 70, u => u.CanSelect),
                        // nothing left to clear, lets go pull that lever.
                        new Decorator(
                            ctx => StyxWoW.Me.IsTank() && Targeting.Instance.FirstUnit == null && BotPoi.Current.Type == PoiType.None,
                            new PrioritySelector(
                                new Decorator(
                                    ctx => entranceDoor.State == WoWGameObjectState.Active,
                                    ScriptHelpers.CreateInteractWithObject(181982)),
                                new Decorator(
                                    ctx => entranceDoor.State == WoWGameObjectState.Ready,
                                    new Action(ctx => ScriptHelpers.MoveTankTo(_tankPoint))))))));
        }

        [EncounterHandler(17662, "Broggok Poison Cloud", Mode = CallBehaviorMode.Proximity)]
        public Composite BroggokHandler() { return ScriptHelpers.CreateRunAwayFromBad(ctx => true, 8f, 17662); }

        private WoWUnit _kelidan;

        [EncounterHandler(17377, "Keli'dan the Breaker", Mode = CallBehaviorMode.Proximity)]
        public Composite KelidanTheBreakerHandler()
        {
            return new PrioritySelector(
                ctx => _kelidan = ctx as WoWUnit,
                ScriptHelpers.CreateRunAwayFromBad(ctx => ((WoWUnit) ctx).HasAura(30940), () => _kelidan.Location, 30, 17f, 17377));
        }
    }
}