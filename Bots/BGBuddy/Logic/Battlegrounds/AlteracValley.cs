// Source: .hb 4.3.4/Honorbuddy/Honorbuddy/ns23/Class51.cs
using System;
using System.Linq;
using Bots.BGBuddy.Helpers;
using Bots.BGBuddy.Resources;
using CommonBehaviors.Actions;
using CommonBehaviors.Resources;
using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;

namespace Bots.BGBuddy.Logic.Battlegrounds
{
    /// <summary>
    /// Alterac Valley (map 30) — 2-faction assault with NPC boss kills.
    /// Horde bosses: Captain Galvandar (11947) at Iceblood Garrison, Tower Point.
    /// Alliance bosses: Balinda Stonehearth (11949) at Stonehearth Outpost, Vanndar Stormpike (11949) at Dun Baldar.
    /// Towers (Alliance: 588 = StormpikeAidStation|StonehearthOutpost|StonehearthBunker) — destroy ≥2 to unlock boss rush.
    /// Towers (Horde: 208896 = WestFrostwolfTower|EastFrostwolfTower|IcebloodTower) — destroy ≥2 to unlock boss rush.
    /// </summary>
    internal sealed class AlteracValley : Battleground
    {
        private readonly WaitTimer _refreshTimer = new WaitTimer(TimeSpan.FromSeconds(2));
        private bool _bossesConsideredDead;

        // HB 4.3.4 WoW NPC entries for the AV boss NPCs.
        private const uint BalindaStonehearthEntry = 11949;
        private const uint CaptainGalvandarEntry = 11947;

        public override string Name => BGBuddyResources.AlteracValley;
        public override int MapId => 30;

        public override void Dispose()
        {
            BGBuddy.Instance.WorldStatesUpdated -= RefreshLandmarks;
            Statuses.Clear();
        }

        public override void Start()
        {
            LoadProfile();
            StartingLocation = RandomizeLocation(StartingLocation, 5);
            RefreshLandmarks();
            BGBuddy.Instance.WorldStatesUpdated += RefreshLandmarks;
            _bossesConsideredDead = true; // Re-evaluated every tick via the boss-rush block
        }

        public override Composite Logic
        {
            get
            {
                return new Sequence(
                    new Switch<LogicType>(
                        ctx => BGBuddySettings.Instance.AVLogicType,
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
                var av = landmark.ToAlteracValleyLandmark();
                var info = new LandmarkInfo(
                    (int)av.LandmarkType,
                    av.ControlType,
                    GetLandmarkBox(av.LandmarkType.ToString()));

                Statuses[(int)av.LandmarkType] = info;
                info.Process();
            }
        }

        private Composite BuildAttackLogic()
        {
            return new PrioritySelector(
                new TreeSharp.Decorator(ctx => StyxWoW.Me.IsHorde,
                    new TreeSharp.PrioritySelector(
                        new TreeSharp.Decorator(ctx => HasDeadBossNearby(AlteracValleyLandmarkType.StonehearthOutpost, BalindaStonehearthEntry),
                            new TreeSharp.Action(ctx => { Logger.Write(BGBuddyResources.BalindaIsDead); _bossesConsideredDead = false; })),
                        new TreeSharp.Decorator(ctx => HasDeadBossNearby(AlteracValleyLandmarkType.IcebloodGarrison, CaptainGalvandarEntry),
                            new TreeSharp.Action(ctx => { Logger.Write(CommonBehaviorsResources.GalvandarIsDead); _bossesConsideredDead = false; })),
                        new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.StonehearthOutpost.ToString(), false, BGBuddyResources.BalindaStonehearth)))),
                new TreeSharp.Decorator(ctx => StyxWoW.Me.IsHorde,
                    new TreeSharp.PrioritySelector(
                        new TreeSharp.Decorator(ctx => CountDestroyedTowers(AlteracValleyLandmarkType.AllianceTowers) >= 2,
                            new TreeSharp.PrioritySelector(
                                new TreeSharp.Decorator(ctx => IsControlledByEnemy(AlteracValleyLandmarkType.StormpikeAidStation),
                                    new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.StormpikeAidStation.ToString(), false, BGBuddyResources.Assault))),
                                new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.DunBaldar.ToString(), false, BGBuddyResources.VanndarStormpike)))),
                        new TreeSharp.Decorator(ctx => IsControlledByEnemy(AlteracValleyLandmarkType.IcewingBunker),
                            new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.IcewingBunker.ToString(), false, BGBuddyResources.Assault))),
                        new TreeSharp.Decorator(ctx => IsControlledByEnemy(AlteracValleyLandmarkType.StormpikeGraveyard),
                            new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.StormpikeGraveyard.ToString(), false, BGBuddyResources.Assault))),
                        new TreeSharp.Decorator(ctx => IsControlledByEnemy(AlteracValleyLandmarkType.DunBaldarNorthBunker),
                            new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.DunBaldarNorthBunker.ToString(), false, BGBuddyResources.Assault))),
                        new TreeSharp.Decorator(ctx => IsControlledByEnemy(AlteracValleyLandmarkType.DunBaldarSouthBunker),
                            new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.DunBaldarSouthBunker.ToString(), false, BGBuddyResources.Assault))),
                        new TreeSharp.Decorator(ctx => IsControlledByEnemy(AlteracValleyLandmarkType.StormpikeAidStation),
                            new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.StormpikeAidStation.ToString(), false, BGBuddyResources.Assault))),
                        new TreeSharp.PrioritySelector(ctx => BiggestFight,
                            new TreeSharp.Decorator(ctx => ((WoWPoint)ctx) != WoWPoint.Zero,
                                new TreeSharp.Action(ctx => SetHotspot((WoWPoint)ctx, 10f, BGBuddyResources.BiggestFight)))),
                        new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.StormpikeAidStation.ToString(), CommonBehaviorsResources.NothingElseToDoWaitingTower)))),
                new TreeSharp.Decorator(ctx => CountDestroyedTowers(AlteracValleyLandmarkType.HordeTowers) >= 2,
                    new TreeSharp.PrioritySelector(
                        new TreeSharp.Decorator(ctx => IsControlledByEnemy(AlteracValleyLandmarkType.FrostwolfReliefHut),
                            new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.FrostwolfReliefHut.ToString(), false, BGBuddyResources.Assault))),
                        new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.FrostwolfKeep.ToString(), false, BGBuddyResources.GeneralDrekThar)))),
                new TreeSharp.Decorator(ctx => IsControlledByEnemy(AlteracValleyLandmarkType.TowerPoint),
                    new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.TowerPoint.ToString(), false, BGBuddyResources.Assault))),
                new TreeSharp.Decorator(ctx => IsControlledByEnemy(AlteracValleyLandmarkType.FrostwolfGraveyard),
                    new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.FrostwolfGraveyard.ToString(), false, BGBuddyResources.Assault))),
                new TreeSharp.Decorator(ctx => IsControlledByEnemy(AlteracValleyLandmarkType.WestFrostwolfTower),
                    new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.WestFrostwolfTower.ToString(), false, BGBuddyResources.Assault))),
                new TreeSharp.Decorator(ctx => IsControlledByEnemy(AlteracValleyLandmarkType.EastFrostwolfTower),
                    new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.EastFrostwolfTower.ToString(), false, BGBuddyResources.Assault))),
                new TreeSharp.Decorator(ctx => IsControlledByEnemy(AlteracValleyLandmarkType.FrostwolfReliefHut),
                    new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.FrostwolfReliefHut.ToString(), false, BGBuddyResources.Assault))),
                new TreeSharp.Decorator(ctx => BiggestFight != WoWPoint.Zero,
                    new TreeSharp.Action(ctx => SetHotspot(BiggestFight, 10f, BGBuddyResources.BiggestFight))),
                new TreeSharp.Action(ctx => SetHotspot(AlteracValleyLandmarkType.FrostwolfReliefHut.ToString(), CommonBehaviorsResources.NothingElseToDoWaitingTower)));
        }

        // HB 4.3.4 pattern: smethod_25-equivalent. Wraps _bossesConsideredDead in a
        // CanRunDecoratorDelegate so the Decorator 2-arg ctor resolves unambiguously.
        private bool BossesConsideredDead(object ctx) => _bossesConsideredDead;

        // True if the given landmark is in Statuses and is currently held by the enemy.
        private bool IsControlledByEnemy(AlteracValleyLandmarkType type)
            => Statuses.TryGetValue((int)type, out var info) && info.ControlledByEnemy;

        // Counts how many landmarks matching `mask` are in the Destroyed state.
        private int CountDestroyedTowers(AlteracValleyLandmarkType mask)
            => Statuses.Values.Count(lm =>
                (lm.Type & (int)mask) != 0 && lm.Control == LandmarkControlType.Destroyed);

        // True if the landmark exists, we are within 70y of its center, and the named
        // boss NPC is not present (or not alive) in the object manager.
        private bool HasDeadBossNearby(AlteracValleyLandmarkType type, uint npcEntry)
        {
            if (!Statuses.TryGetValue((int)type, out var info)) return false;
            if (info.Box.Center.DistanceSqr(StyxWoW.Me.Location) > 4900f) return false; // 70y²

            return !ObjectManager.GetObjectsOfType<WoWUnit>()
                .Any(u => u.Entry == npcEntry && u.IsAlive);
        }
    }
}

