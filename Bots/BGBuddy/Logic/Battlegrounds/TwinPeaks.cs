// Source: .hb 4.3.4/Honorbuddy/Honorbuddy/ns29/Class56.cs
// WotLK note: Twin Peaks is a Cataclysm battleground. The map (726) is not
// present in 3.3.5a. The class is a no-op for WotLK — DllLoader<Battleground>
// finds it but BGBuddy never reaches MapId 726.

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
    /// Twin Peaks (map 726) — Cataclysm flag-carrier variant of Warsong Gulch.
    /// Identical logic to WSG: defend our carrier if healer, focus enemy carrier
    /// when 2+ attackers present, else chase the biggest fight / biggest pack.
    /// </summary>
    internal sealed class TwinPeaks : Battleground
    {
        public override string Name => BGBuddyResources.TwinPeaks;
        public override int MapId => 726;

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
                        ctx => BGBuddySettings.Instance.TpLogicType,
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

        // See WarsongGulch.ShouldFocusOnEnemyFlagCarrier — WoWPlayer has no Attackers
        // property in this codebase, so we count friendlies within 10y of the carrier.
        private bool ShouldFocusOnEnemyFlagCarrier
            => EnemyFlagCarrier != null
               && StyxWoW.Me.IsAlive
               && ObjectManager.GetObjectsOfType<WoWPlayer>(false, false)
                    .Count(p => p.IsHorde == StyxWoW.Me.IsHorde
                             && p.IsAlive
                             && p.Location.DistanceSqr(EnemyFlagCarrier.Location) < 100f) >= 2;
    }
}
