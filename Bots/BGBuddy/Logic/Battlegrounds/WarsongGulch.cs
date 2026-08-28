// Source: .hb 4.3.4/Honorbuddy/Honorbuddy/ns30/Class57.cs
using System.Linq;
using Bots.BGBuddy.Resources;
using CommonBehaviors.Actions;
using Styx;
using Styx.Logic;
using Styx.Logic.POI;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;

namespace Bots.BGBuddy.Logic.Battlegrounds
{
    /// <summary>
    /// Warsong Gulch (map 489) — flag carrier logic.
    /// Defend our carrier if we are a healer; focus the enemy carrier if 2+ friendlies
    /// are on them; otherwise chase the biggest fight / biggest friendly pack.
    /// </summary>
    internal sealed class WarsongGulch : Battleground
    {
        public override string Name => BGBuddyResources.WarsongGulch;
        public override int MapId => 489;

        public override void Dispose() { }

        public override void Start()
        {
            LoadProfile();
            StartingLocation = RandomizeLocation(StartingLocation, 5);
        }

        public override Composite Logic
        {
            get
            {
                return new Sequence(
                    new Switch<LogicType>(
                        ctx => BGBuddySettings.Instance.WsgLogicType,
                        new SwitchArgument<LogicType>(LogicType.Attack, BuildAttackLogic())),
                    new Decorator(ctx => BotPoi.Current.Type == PoiType.Hotspot,
                        BGBuddy.CreateMoveToLocationBehavior(ctx => Hotspot, true, 5f)));
            }
        }

        private Composite BuildAttackLogic()
        {
            return new PrioritySelector(
                new Decorator(ctx => ShouldDefendFriendlyFlagCarrier,
                    new TreeSharp.Action(ctx => SetHotspot(FriendlyFlagCarrier.Location, 10f, BGBuddyResources.DefendFriendlyFlagCarrier))),
                new Decorator(ctx => ShouldFocusOnEnemyFlagCarrier,
                    new TreeSharp.Action(ctx => SetHotspot(EnemyFlagCarrier.Location, 10f, BGBuddyResources.EnemyFlagCarrier))),
                new PrioritySelector(ctx => BiggestFight,
                    new Decorator(ctx => ((WoWPoint)ctx) != WoWPoint.Zero,
                        new TreeSharp.Action(ctx => SetHotspot((WoWPoint)ctx)))),
                new PrioritySelector(ctx => BiggestFriendlyPack,
                    new Decorator(ctx => ((WoWPoint)ctx) != WoWPoint.Zero,
                        new TreeSharp.Action(ctx => SetHotspot((WoWPoint)ctx, 10f, BGBuddyResources.BiggestFriendlyPack)))),
                new ActionAlwaysSucceed());
        }

        private bool ShouldDefendFriendlyFlagCarrier
            => FriendlyFlagCarrier != null && StyxWoW.Me.SpecType == SpecType.Healer;

        // HB 4.3.4 used EnemyFlagCarrier.Attackers to detect 2+ friendlies on the carrier.
        // WoWPlayer in this codebase does not expose "Attackers" — the closest equivalent is
        // counting friendlies within 10y of the enemy carrier. If the carrier is reachable
        // and at least 2 friendlies are close by, we treat the carrier as a valid focus.
        private bool ShouldFocusOnEnemyFlagCarrier
            => EnemyFlagCarrier != null
               && StyxWoW.Me.IsAlive
               && ObjectManager.GetObjectsOfType<WoWPlayer>(false, false)
                    .Count(p => p.IsHorde == StyxWoW.Me.IsHorde
                             && p.IsAlive
                             && p.Location.DistanceSqr(EnemyFlagCarrier.Location) < 100f) >= 2;
    }
}
