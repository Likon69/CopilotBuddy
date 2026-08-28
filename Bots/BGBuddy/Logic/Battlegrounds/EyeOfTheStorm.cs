// Source: .hb 4.3.4/Honorbuddy/Honorbuddy/ns27/Class53.cs
using System;
using System.Linq;
using Bots.BGBuddy.Resources;
using CommonBehaviors.Actions;
using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.Logic.Profiles;
using TreeSharp;
using Tripper.Tools.Math;

namespace Bots.BGBuddy.Logic.Battlegrounds
{
    /// <summary>
    /// Eye of the Storm (map 566) — 4 towers (Blood Elf, Fel Reaver Ruins, Mage Tower,
    /// Draenei Ruins) plus the central flag. A blackspot is added over the central
    /// flag pole so the bot does not path through it; the rest of the logic is the
    /// standard 4-tower rotation: rush a base if everything is uncontrolled, else
    /// chase the closest fight / biggest brawl / weakest defended tower.
    /// </summary>
    internal sealed class EyeOfTheStorm : Battleground
    {
        private readonly WaitTimer _refreshTimer = new WaitTimer(TimeSpan.FromSeconds(2));
        private Blackspot _centerBlackspot;

        public override string Name => BGBuddyResources.EyeOfTheStorm;
        public override int MapId => 566;

        public override void Dispose()
        {
            BGBuddy.Instance.WorldStatesUpdated -= RefreshLandmarks;
            Statuses.Clear();
        }

        public override void Start()
        {
            LoadProfile();
            RefreshLandmarks();
            BGBuddy.Instance.WorldStatesUpdated += RefreshLandmarks;

            // Mark the central flag area as a navmesh blackspot so the pathfinder
            // routes around it. Centered 50y below ground (the flag pole is at 0,0,0-ish
            // and the pole geometry extends well below surface).
            var center = Profile.Boxes[Side].First(b => b.Name == "Flag").Center;
            center.Z -= 50f;
            _centerBlackspot = new Blackspot(center, 45f, 100f);
            BlackspotManager.AddBlackspots(new[] { _centerBlackspot });
        }

        public override Composite Logic
        {
            get
            {
                return new Sequence(
                    new Switch<LogicType>(
                        ctx => BGBuddySettings.Instance.EotsLogicType,
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
                var eots = landmark.ToEyeOfTheStormLandmark();
                var info = new LandmarkInfo(
                    (int)eots.LandmarkType,
                    eots.ControlType,
                    GetLandmarkBox(eots.LandmarkType.ToString()));

                Statuses[(int)eots.LandmarkType] = info;
                info.Process();
            }
        }

        private Composite BuildAttackLogic()
        {
            return new PrioritySelector(
                // Horde start: rush Blood Elf Tower if every tower is uncontrolled.
                new Decorator(ctx => StyxWoW.Me.IsHorde && AllBasesUncontrolled,
                    new TreeSharp.Action(ctx => SetHotspot(EyeOfTheStormLandmarkType.BloodElfTower.ToString(), BGBuddyResources.StartOfGame))),
                // Alliance start: rush Mage Tower on the same condition.
                new Decorator(ctx => StyxWoW.Me.IsAlliance && AllBasesUncontrolled,
                    new TreeSharp.Action(ctx => SetHotspot(EyeOfTheStormLandmarkType.MageTower.ToString(), BGBuddyResources.StartOfGame))),
                // A tower has at least 2 friendlies (or 2 enemies on our tower).
                new Decorator(ctx => ClosestInBattle != null,
                    new TreeSharp.Action(ctx => SetHotspot(((EyeOfTheStormLandmarkType)ClosestInBattle.Type).ToString(), BGBuddyResources.Battle))),
                // The biggest fight location has enemies being attacked by friendlies.
                new PrioritySelector(ctx => BiggestFight,
                    new Decorator(ctx => ((WoWPoint)ctx) != WoWPoint.Zero,
                        new TreeSharp.Action(ctx => SetHotspot((WoWPoint)ctx)))),
                // Defend the weakest friendly-held tower.
                new Decorator(ctx => ClosestToDefend != null,
                    new TreeSharp.Action(ctx => SetHotspot(((EyeOfTheStormLandmarkType)ClosestToDefend.Type).ToString(), BGBuddyResources.NothingElseToDo))),
                new ActionAlwaysSucceed());
        }

        // "All towers are uncontrolled or unknown" — the start-of-game condition.
        private bool AllBasesUncontrolled
            => Statuses.Values.All(lm => lm.Control == LandmarkControlType.Uncontrolled
                                      || lm.Control == LandmarkControlType.Unknown);
    }
}

