// Source: .hb 4.3.4/Honorbuddy/Honorbuddy/ns11/Class50.cs
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
    /// Arathi Basin (map 529) — capture-and-hold base logic.
    /// 5 bases (Stables, Blacksmith, LumberMill, GoldMine, Farm).
    /// Horde starts at Farm, Alliance at Stables. Then pursues the closest
    /// conflicted base, the closest base in battle, the biggest fight, or
    /// the closest base needing defense.
    /// </summary>
    internal sealed class ArathiBasin : Battleground
    {
        private readonly WaitTimer _refreshTimer = new WaitTimer(TimeSpan.FromSeconds(2));

        public override string Name => BGBuddyResources.ArathiBasin;
        public override int MapId => 529;

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
                        ctx => BGBuddySettings.Instance.AbLogicType,
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
                var ab = landmark.ToArathiBasinLandmark();
                var info = new LandmarkInfo(
                    (int)ab.LandmarkType,
                    ab.ControlType,
                    GetLandmarkBox(ab.LandmarkType.ToString()));

                Statuses[(int)ab.LandmarkType] = info;
                info.Process();
            }
        }

        private Composite BuildAttackLogic()
        {
            return new PrioritySelector(
                // Horde start: rush Farm if every base is uncontested or enemy-held.
                new Decorator(ctx => StyxWoW.Me.IsHorde && AllBasesUncontestedByUs,
                    new TreeSharp.Action(ctx => SetHotspot(ArathiBasinLandmarkType.Farm.ToString(), BGBuddyResources.StartOfGame))),
                // Alliance start: rush Stables on the same condition.
                new Decorator(ctx => StyxWoW.Me.IsAlliance && AllBasesUncontestedByUs,
                    new TreeSharp.Action(ctx => SetHotspot(ArathiBasinLandmarkType.Stables.ToString(), BGBuddyResources.StartOfGame))),
                // A base is currently being fought over.
                new Decorator(ctx => ClosestConflicted != null,
                    new TreeSharp.Action(ctx => SetHotspot(((ArathiBasinLandmarkType)ClosestConflicted.Type).ToString(), BGBuddyResources.Conflicted))),
                // A base has at least 2 friendlies (or 2 enemies on our base).
                new Decorator(ctx => ClosestInBattle != null,
                    new TreeSharp.Action(ctx => SetHotspot(((ArathiBasinLandmarkType)ClosestInBattle.Type).ToString(), BGBuddyResources.Battle))),
                // The biggest fight location has enemies being attacked by friendlies.
                new PrioritySelector(ctx => BiggestFight,
                    new Decorator(ctx => ((WoWPoint)ctx) != WoWPoint.Zero,
                        new TreeSharp.Action(ctx => SetHotspot((WoWPoint)ctx)))),
                // Defend the weakest friendly-held base.
                new Decorator(ctx => ClosestToDefend != null,
                    new TreeSharp.Action(ctx => SetHotspot(((ArathiBasinLandmarkType)ClosestToDefend.Type).ToString(), BGBuddyResources.NothingElseToDo))),
                new ActionAlwaysSucceed());
        }

        // "All bases are either not ours, or in conflict" — used to decide whether
        // we should fall back to the start-of-game base rush.
        private bool AllBasesUncontestedByUs
            => Statuses.Values.All(lm => !lm.ControlledByUs && lm.Control != LandmarkControlType.InConflict);
    }
}
