// Ported from: .hb 4.3.4/Honorbuddy/Honorbuddy/Bots/BGBuddy/Logic/Battlegrounds/BattleForGilneas.cs
// Source: .hb 4.3.4/Honorbuddy/Honorbuddy/ns25/Class52.cs
// Target path: Bots/BGBuddy/Logic/Battlegrounds/BattleForGilneas.cs
// WotLK note: Battle For Gilneas is a Cataclysm battleground. The map (761) and
// landmarks are not present in 3.3.5a. The class is included for completeness
// and will be a no-op for WotLK — DllLoader<Battleground> finds it but the
// BGBuddy's OnMapChangedOrStartStop never reaches MapId 761.

using System;
using System.Linq;
using Bots.BGBuddy.Resources;
using CommonBehaviors.Actions;
using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using TreeSharp;

namespace Bots.BGBuddy.Logic.Battlegrounds
{
    /// <summary>
    /// Battle For Gilneas (map 761) — Cataclysm capture-and-hold, 3 bases
    /// (Mines, Waterworks, Lighthouse). Mirrors ArathiBasin with 3 nodes.
    /// </summary>
    internal sealed class BattleForGilneas : Battleground
    {
        private readonly WaitTimer _refreshTimer = new WaitTimer(TimeSpan.FromSeconds(2));

        public override string Name => BGBuddyResources.BattleForGilneas;
        public override int MapId => 761;

        public override void Dispose()
        {
            BGBuddy.Instance.WorldStatesUpdated -= RefreshLandmarks;
            Statuses.Clear();
        }

        public override void Start()
        {
            LoadProfile();
            StartingLocation = RandomizeLocation(StartingLocation, 3);
            RefreshLandmarks();
            BGBuddy.Instance.WorldStatesUpdated += RefreshLandmarks;
        }

        public override Composite Logic
        {
            get
            {
                return new Sequence(
                    new Switch<LogicType>(
                        ctx => BGBuddySettings.Instance.BfgLogicType,
                        new SwitchArgument<LogicType>(LogicType.Attack, BuildAttackLogic())),
                    new Decorator(ctx => BotPoi.Current.Type == PoiType.Hotspot,
                        BGBuddy.CreateMoveToLocationBehavior(ctx => Hotspot, true, 5f)));
            }
        }

        private void RefreshLandmarks()
        {
            if (!_refreshTimer.IsFinished) return;
            _refreshTimer.Reset();

            Styx.Logic.Battlegrounds.LandMarks.Refresh();
            foreach (var landmark in Styx.Logic.Battlegrounds.LandMarks.LandmarkList)
            {
                var bfg = landmark.ToBattleForGilneasLandmark();
                var info = new LandmarkInfo(
                    (int)bfg.LandmarkType,
                    bfg.ControlType,
                    GetLandmarkBox(bfg.LandmarkType.ToString()));

                Statuses[(int)bfg.LandmarkType] = info;
                info.Process();
            }
        }

        private Composite BuildAttackLogic()
        {
            return new PrioritySelector(
                new Decorator(ctx => StyxWoW.Me.IsHorde && AllBasesUncontestedByUs,
                    new TreeSharp.Action(ctx => SetHotspot(BattleForGilneasLandmarkType.Mines.ToString(), BGBuddyResources.StartOfGame))),
                new Decorator(ctx => StyxWoW.Me.IsAlliance && AllBasesUncontestedByUs,
                    new TreeSharp.Action(ctx => SetHotspot(BattleForGilneasLandmarkType.Lighthouse.ToString(), BGBuddyResources.StartOfGame))),
                new Decorator(ctx => ClosestConflicted != null,
                    new TreeSharp.Action(ctx => SetHotspot(((BattleForGilneasLandmarkType)ClosestConflicted.Type).ToString(), BGBuddyResources.Conflicted))),
                new Decorator(ctx => ClosestInBattle != null,
                    new TreeSharp.Action(ctx => SetHotspot(((BattleForGilneasLandmarkType)ClosestInBattle.Type).ToString(), BGBuddyResources.Battle))),
                new PrioritySelector(ctx => BiggestFight,
                    new Decorator(ctx => ((WoWPoint)ctx) != WoWPoint.Zero,
                        new TreeSharp.Action(ctx => SetHotspot((WoWPoint)ctx)))),
                new Decorator(ctx => ClosestToDefend != null,
                    new TreeSharp.Action(ctx => SetHotspot(((BattleForGilneasLandmarkType)ClosestToDefend.Type).ToString(), BGBuddyResources.NothingElseToDo))),
                new ActionAlwaysSucceed());
        }

        private bool AllBasesUncontestedByUs
            => Statuses.Values.All(lm => !lm.ControlledByUs && lm.Control != LandmarkControlType.InConflict);
    }
}
