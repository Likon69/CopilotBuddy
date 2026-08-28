using System;
using System.Collections.Generic;
using System.Linq;
using Bots.BGBuddy.Helpers;
using Bots.BGBuddy.Logic.Battlegrounds;
using Bots.BGBuddy.Resources;
using CommonBehaviors.Actions;
using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Inventory.Frames.LootFrame;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;
using Tripper.Tools.Math;

namespace Bots.BGBuddy
{
    /// <summary>
    /// Abstract base class for all battleground implementations.
    /// Provides common logic for hotspot management, targeting, looting, combat, and flag interaction.
    /// </summary>
    public abstract class Battleground : IDisposable
    {
        #region Fields

        private WoWPoint _lastTargetLocation;
        private ulong _lastTargetGuid;

        // Timer for RandomizeLocation attempts
        private readonly WaitTimer _randomizeTimer = new WaitTimer(TimeSpan.FromSeconds(1));

        // Blacklisted game object entries (e.g., AV banners that shouldn't be interacted with as flags)
        private static readonly HashSet<uint> _blacklistedFlagEntries = new HashSet<uint> { 34480, 32566 };

        // Shared timers for hotspot management
        internal static readonly WaitTimer HotspotTimer = new WaitTimer(TimeSpan.FromSeconds(10));
        internal static readonly WaitTimer SetHotspotTimer = new WaitTimer(TimeSpan.FromSeconds(10));

        #endregion

        #region Constructor

        protected Battleground()
        {
            Statuses = new Dictionary<int, LandmarkInfo>();
        }

        #endregion

        #region Abstract Members

        public abstract string Name { get; }
        public abstract int MapId { get; }
        public abstract Composite Logic { get; }
        public abstract void Dispose();
        public abstract void Start();

        #endregion

        #region Properties

        internal Dictionary<int, LandmarkInfo> Statuses { get; set; }

        public BgBotProfile Profile { get; set; }

        public WoWPoint Hotspot { get; set; }

        public WoWPoint ActualHotspot { get; set; }

        public WoWPoint StartingLocation { get; set; }

        /// <summary>
        /// Current side in the battleground.
        /// For SotA (map 607), returns Attack/Defend. Otherwise returns Alliance/Horde.
        /// </summary>
        public BattlegroundSide Side
        {
            get
            {
                // SotA uses Attack/Defend sides instead of Horde/Alliance
                if (StyxWoW.Me.CurrentMap.MapId == 607)
                    return IsAttacking ? BattlegroundSide.Attack : BattlegroundSide.Defend;

                return StyxWoW.Me.IsHorde ? BattlegroundSide.Horde : BattlegroundSide.Alliance;
            }
        }

        /// <summary>
        /// Whether we are within 30 yards of our current hotspot.
        /// </summary>
        public bool IsAtHotspot => StyxWoW.Me.Location.DistanceSqr(Hotspot) < 900f;

        /// <summary>
        /// Whether our faction is currently on the attacking side (all nodes are lost).
        /// Horde: no AllianceControlled(512) nodes. Alliance: no HordeControlled(1024) nodes.
        /// </summary>
        protected bool IsAttacking
        {
            get
            {
                if (StyxWoW.Me.IsHorde)
                    return !Statuses.Values.Any(lm => lm.Type == 512);
                else
                    return !Statuses.Values.Any(lm => lm.Type == 1024);
            }
        }

        /// <summary>
        /// Whether our current target has moved too far from where we initially targeted them.
        /// Excludes entry 28781 (NPCs that naturally move) and checks 50yd distance threshold.
        /// </summary>
        protected bool NeedToDropCurrentTarget
        {
            get
            {
                return _lastTargetLocation != WoWPoint.Zero
                    && StyxWoW.Me.CurrentTarget != null
                    && StyxWoW.Me.CurrentTarget.Entry != 28781
                    && StyxWoW.Me.CurrentTargetGuid == _lastTargetGuid
                    && StyxWoW.Me.CurrentTarget.Location.DistanceSqr(_lastTargetLocation) > 2500f;
            }
        }

        /// <summary>
        /// English name for the battleground based on MapId.
        /// </summary>
        public string NonLocalizedName
        {
            get
            {
                return MapId switch
                {
                    30 => "Alterac Valley",
                    489 => "Warsong Gulch",
                    529 => "Arathi Basin",
                    566 => "Eye of the Storm",
                    607 => "Strand of the Ancients",
                    628 => "Isle of Conquest",
                    // Cataclysm BGs — return empty for WotLK
                    726 => "Twin Peaks",
                    761 => "Battle For Gilneas",
                    _ => ""
                };
            }
        }

        /// <summary>
        /// Closest landmark that is in conflict (InConflict state with ≥2 friendlies).
        /// </summary>
        protected LandmarkInfo ClosestConflicted
        {
            get
            {
                return Statuses.Values
                    .OrderBy(lm => lm.Box.Center.Distance(StyxWoW.Me.Location))
                    .FirstOrDefault(lm => lm.Control == LandmarkControlType.InConflict && lm.FriendlyPlayersAround >= 2);
            }
        }

        /// <summary>
        /// Closest landmark that has a battle happening (not controlled by us with friendlies, or controlled by us with enemies).
        /// </summary>
        protected LandmarkInfo ClosestInBattle
        {
            get
            {
                return Statuses.Values
                    .OrderBy(lm => lm.Box.Center.DistanceSqr(StyxWoW.Me.Location))
                    .FirstOrDefault(lm =>
                        (!lm.ControlledByUs && lm.FriendlyPlayersAround >= 2) ||
                        (lm.ControlledByUs && lm.EnemyPlayersAround >= 2));
            }
        }

        /// <summary>
        /// Closest landmark we own that needs defending (ordered by fewest friendly players).
        /// </summary>
        protected LandmarkInfo ClosestToDefend
        {
            get
            {
                return Statuses.Values
                    .OrderBy(lm => lm.FriendlyPlayersAround)
                    .FirstOrDefault(lm => lm.ControlledByUs);
            }
        }

        /// <summary>
        /// Location with the biggest cluster of enemy players being attacked by our allies.
        /// </summary>
        protected WoWPoint BiggestFight
        {
            get
            {
                bool isHorde = StyxWoW.Me.IsHorde;
                var allPlayers = ObjectManager.GetObjectsOfType<WoWPlayer>();
                var alivePlayers = allPlayers.Where(p => p.IsAlive);
                var friendlyPlayers = alivePlayers.Where(p => p.IsHorde == isHorde);
                var enemyPlayers = alivePlayers.Where(p => p.IsHorde != isHorde);

                WoWPoint result = WoWPoint.Zero;
                int bestCount = 1;
                WoWPlayer bestEnemy = null;

                foreach (var enemy in enemyPlayers)
                {
                    // Count how many friendly players are near this enemy (within 30yd)
                    int friendliesNear = friendlyPlayers.Count(f => f.Location.DistanceSqr(enemy.Location) <= 900f);

                    if (friendliesNear > bestCount && Navigator.CanNavigateFully(StyxWoW.Me.Location, enemy.Location))
                    {
                        bestCount = friendliesNear;
                        bestEnemy = enemy;
                    }
                }

                if (bestEnemy != null)
                    result = bestEnemy.Location;

                return result;
            }
        }

        /// <summary>
        /// Location of the largest moving group of friendly players.
        /// </summary>
        protected WoWPoint BiggestFriendlyPack
        {
            get
            {
                bool isHorde = StyxWoW.Me.IsHorde;
                WoWPoint result = WoWPoint.Zero;

                var friendlyPlayers = ObjectManager.GetObjectsOfType<WoWPlayer>()
                    .Where(p => p.IsHorde == isHorde && p.IsAlive);

                // Order by cluster size descending, then by distance
                var ordered = friendlyPlayers
                    .OrderByDescending(p => friendlyPlayers.Count(f => f.Location.DistanceSqr(p.Location) <= 3600f))
                    .ThenBy(p => p.DistanceSqr);

                foreach (var player in ordered)
                {
                    if (Navigator.CanNavigateFully(StyxWoW.Me.Location, player.Location) && player.IsMoving)
                    {
                        result = player.Location;
                        break;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Enemy player carrying a flag (has aura containing "flag").
        /// </summary>
        protected WoWPlayer EnemyFlagCarrier
        {
            get
            {
                bool isHorde = StyxWoW.Me.IsHorde;
                return ObjectManager.GetObjectsOfType<WoWPlayer>(false, false)
                    .FirstOrDefault(p => p.IsHorde != isHorde && p.Auras.Keys.Any(k => k.ToLowerInvariant().Contains("flag")));
            }
        }

        /// <summary>
        /// Friendly player carrying a flag (has aura containing "flag").
        /// </summary>
        protected WoWPlayer FriendlyFlagCarrier
        {
            get
            {
                bool isHorde = StyxWoW.Me.IsHorde;
                return ObjectManager.GetObjectsOfType<WoWPlayer>(false, false)
                    .FirstOrDefault(p => p.IsHorde == isHorde && p.Auras.Keys.Any(k => k.ToLowerInvariant().Contains("flag")));
            }
        }

        #endregion

        #region Profile & Location

        /// <summary>
        /// Loads the BGBuddy profile for this battleground from the Default Profiles directory.
        /// </summary>
        public void LoadProfile()
        {
            Profile = BgBotProfile.Load(Logging.ApplicationPath + "\\Default Profiles\\BGBuddy\\" + NonLocalizedName + ".xml");
            StartingLocation = Profile.StartLocations.ContainsKey(Side)
                ? Profile.StartLocations[Side]
                : WoWPoint.Zero;
        }

        /// <summary>
        /// Randomizes a location within a 10-yard radius, finding a navigable Z-height.
        /// </summary>
        public WoWPoint RandomizeLocation(WoWPoint pt)
        {
            return RandomizeLocation(pt, 10);
        }

        /// <summary>
        /// Randomizes a location within the given maxDistance radius.
        /// Tries random ray-cast directions until a navigable point with valid height is found.
        /// Falls back to the original point if the timer expires.
        /// </summary>
        public WoWPoint RandomizeLocation(WoWPoint pt, int maxDistance)
        {
            if (pt == WoWPoint.Zero)
                return pt;

            _randomizeTimer.Reset();
            var random = new Random();

            while (!_randomizeTimer.IsFinished)
            {
                double angle = random.NextDouble() * 360.0;
                int distance = random.Next(0, maxDistance);

                WoWPoint candidate = pt.RayCast(WoWMathHelper.DegreesToRadians((float)angle), (float)distance);
                List<float> heights = Navigator.FindHeights(candidate.X, candidate.Y);

                if (heights.Count != 0)
                {
                    // Pick the closest height to our candidate
                    float bestZ = heights.OrderBy(h => Math.Abs(h - candidate.Z)).First();
                    candidate.Z = bestZ;

                    if (Navigator.CanNavigateFully(StyxWoW.Me.Location, candidate))
                        return candidate;
                }
            }

            return pt;
        }

        #endregion

        #region Targeting

        protected void TargetFirstTarget()
        {
            WoWUnit firstUnit = Targeting.Instance.FirstUnit;
            _lastTargetGuid = firstUnit.Guid;
            _lastTargetLocation = firstUnit.Location;

            if (StyxWoW.Me.CurrentTargetGuid != _lastTargetGuid)
                firstUnit.Target();

            BotPoi.Current = new BotPoi(firstUnit, PoiType.Kill);
        }

        protected void ClearTarget()
        {
            BotPoi.Clear(BGBuddyResources.Battleground_ClearTarget_Target_being_cleared_);
            StyxWoW.Me.ClearTarget();
            _lastTargetGuid = 0;
            _lastTargetLocation = WoWPoint.Zero;
        }

        #endregion

        #region Behavior Tree Construction

        /// <summary>
        /// Hook for subclasses to inject behavior before the Preparation phase logic.
        /// </summary>
        protected virtual Composite CreateBeforePrepBehavior()
        {
            return new PrioritySelector();
        }

        /// <summary>
        /// Main common logic for all battlegrounds: prep phase, rest, buffs, mount, move to start,
        /// then targeting sanity, combat, flag interaction, and landmark processing.
        /// </summary>
        public Composite CreateCommonLogic
        {
            get
            {
                return new PrioritySelector(
                    CreateBeforePrepBehavior(),

                    // Preparation phase — buff up and move to starting location
                    new Decorator(
                        ctx => StyxWoW.Me.HasAura("Preparation"),
                        new Sequence(
                            new PrioritySelector(
                                new Decorator(ctx => StyxWoW.Me.MapId != 607,
                                    BGBuddy.Instance.CreateTakeConsumablesBehavior()),
                                RoutineManager.Current.RestBehavior,
                                RoutineManager.Current.PreCombatBuffBehavior,
                                new Decorator(ctx => !StyxWoW.Me.Mounted,
                                    new Action(ctx => Mount.MountUp(() => true, () => StartingLocation))),
                                new Decorator(ctx => StartingLocation != WoWPoint.Zero && !StyxWoW.Me.IsOnTransport,
                                    BGBuddy.CreateMoveToLocationBehavior(ctx => StartingLocation, true, 2f)),
                                new ActionAlwaysSucceed()
                            ),
                            new Action(ctx => HotspotTimer.Reset())
                        )),

                    CreateTargetingSanityChecks(),
                    CreateCombatBehavior(),
                    new Decorator(ctx => MapId != 489 && MapId != 726,
                        CreateInteractFlagsBehavior()),

                    // Process stale landmarks (every 5 seconds)
                    new Action(ctx =>
                    {
                        foreach (var lm in Statuses.Values.Where(l => DateTime.Now.Subtract(l.LastProcessed) >= TimeSpan.FromSeconds(5)))
                            lm.Process();
                        return RunStatus.Failure;
                    })
                );
            }
        }

        /// <summary>
        /// Loot behavior for player corpses when enabled and no active targets.
        /// </summary>
        protected Composite CreateLootInsigniaBehavior()
        {
            return new PrioritySelector(
                ctx =>
                {
                    // Find nearest lootable hostile player corpse
                    return ObjectManager.GetObjectsOfType<WoWObject>(true)
                        .Select(o => new { Object = o, Player = o.ToPlayer() })
                        .Select(x => new { x.Object, x.Player, Corpse = x.Object as WoWCorpse })
                        .Where(x => (x.Player != null && x.Player.IsHostile && x.Player.CanLoot) ||
                                    (x.Corpse != null && x.Corpse.IsLootable && !x.Corpse.IsOnlyBones))
                        .Where(x => x.Object.DistanceSqr < 400)
                        .OrderBy(x => x.Object.DistanceSqr)
                        .Select(x => x.Object)
                        .FirstOrDefault();
                },
                new Decorator(
                    (ctx) => ctx != null && BGBuddySettings.Instance.LootCorpses && Targeting.Instance.FirstUnit == null,
                    new PrioritySelector(
                        new Decorator(
                            ctx => ((WoWCorpse)ctx).DistanceSqr < 9,
                            new Sequence(
                                new DecoratorContinue(ctx => StyxWoW.Me.IsMoving,
                                    new Action(ctx => Navigator.PlayerMover.MoveStop())),
                                new WaitContinue(1, ctx => !StyxWoW.Me.IsMoving, new ActionAlwaysSucceed()),
                                new Action(ctx => ((WoWCorpse)ctx).Interact()),
                                new WaitContinue(2, ctx => LootFrame.Instance.IsVisible, new Action(ctx => LootFrame.Instance.LootAll())),
                                new Action(ctx => Navigator.Clear())
                            )),
                        new Sequence(
                            new Action(ctx => Logger.Write(BGBuddyResources.Battleground_CreateLootInsigniaBehavior_Moving_to_loot_the_player_corpse)),
                            BGBuddy.CreateMoveToLocationBehavior(ctx => ((WoWCorpse)ctx).Location, true, 2f)
                        )
                    )
                )
            );
        }

        /// <summary>
        /// Behavior for interacting with nearby flag/button game objects in the battleground.
        /// </summary>
        protected Composite CreateInteractFlagsBehavior()
        {
            return new PrioritySelector(
                ctx =>
                {
                    // Find nearest interactable flag/button game object within 25yd
                    return ObjectManager.GetObjectsOfType<WoWGameObject>(false, false)
                        .FirstOrDefault(g =>
                            (g.SubType == WoWGameObjectType.Button || g.SubType == WoWGameObjectType.Goober) &&
                            !_blacklistedFlagEntries.Contains(g.Entry) &&
                            g.CanUse() &&
                            g.DistanceSqr < 625);
                },
                new Decorator(
                    ctx => ctx != null && Targeting.Instance.FirstUnit == null,
                    new PrioritySelector(
                        new Decorator(
                            ctx => ((WoWGameObject)ctx).WithinInteractRange,
                            new Sequence(
                                new DecoratorContinue(ctx => StyxWoW.Me.IsMoving,
                                    new Action(ctx => Navigator.PlayerMover.MoveStop())),
                                new WaitContinue(1, ctx => !StyxWoW.Me.IsMoving, new ActionAlwaysSucceed()),
                                new Action(ctx => ((WoWGameObject)ctx).Interact()),
                                new WaitContinue(1, ctx => StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                                new Action(ctx => Navigator.Clear()),
                                new WaitContinue(10, ctx => !StyxWoW.Me.IsCasting, new ActionAlwaysSucceed())
                            )),
                        new Sequence(
                            new Action(ctx => Logger.Write(BGBuddyResources.Battleground_CreateInteractFlagsBehavior_Moving_to_interact_with_flag)),
                            BGBuddy.CreateMoveToLocationBehavior(ctx => ((WoWGameObject)ctx).Location, true, 3f)
                        )
                    )
                )
            );
        }

        /// <summary>
        /// Sanity checks for targeting: clear dead targets, drop faraway targets, acquire new targets.
        /// </summary>
        protected Composite CreateTargetingSanityChecks()
        {
            return new PrioritySelector(
                // Sanity #1: Target is dead — clear it
                new Decorator(
                    ctx => StyxWoW.Me.CurrentTarget != null && StyxWoW.Me.CurrentTarget.Dead,
                    new Sequence(
                        new Action(ctx => ClearTarget()),
                        new WaitContinue(2, ctx => StyxWoW.Me.CurrentTargetGuid == 0, new ActionAlwaysSucceed())
                    )),

                // Sanity #2: Target moved too far from original position — blacklist and clear
                new Decorator(
                    ctx => NeedToDropCurrentTarget,
                    new Sequence(
                        new Action(ctx =>
                        {
                            Logger.Write(BGBuddyResources.Battleground_CreateTargetingSanityChecks_We_ve_gotten_to_far_away_from_the_original_target_spot__Blacklisting_target_for_5s_and_clearing_);
                            Blacklist.Add(StyxWoW.Me.CurrentTarget, TimeSpan.FromSeconds(5));
                            ClearTarget();
                        }),
                        new WaitContinue(2, ctx => StyxWoW.Me.CurrentTargetGuid == 0, new ActionAlwaysSucceed())
                    )),

                // Sanity #3: Non-healer or in-combat, first unit exists, not already targeted — target it
                new Decorator(
                    ctx => (StyxWoW.Me.SpecType != SpecType.Healer || StyxWoW.Me.Combat)
                        && Targeting.Instance.FirstUnit != null
                        && (_lastTargetGuid != Targeting.Instance.FirstUnit.Guid || BotPoi.Current.Type != PoiType.Kill)
                        && Navigator.CanNavigateFully(StyxWoW.Me.Location, Targeting.Instance.FirstUnit.Location),
                    new Sequence(
                        new Action(ctx => TargetFirstTarget()),
                        new WaitContinue(2, ctx => StyxWoW.Me.CurrentTargetGuid == _lastTargetGuid, new ActionAlwaysSucceed())
                    )),

                // Sanity #4: No first unit but POI is Kill — clear stale POI
                new Decorator(
                    ctx => Targeting.Instance.FirstUnit == null && BotPoi.Current.Type == PoiType.Kill,
                    new Sequence(
                        new Action(ctx => ClearTarget()),
                        new ActionClearPoi("BGBuddy Sanity Check #4 [No Target+Type=Kill]"),
                        new WaitContinue(2, ctx => StyxWoW.Me.CurrentTargetGuid == 0, new ActionAlwaysSucceed())
                    ))
            );
        }

        /// <summary>
        /// Combat behavior: out-of-combat buffs, in-combat heals/buffs, pull/combat routines.
        /// </summary>
        protected Composite CreateCombatBehavior()
        {
            return new PrioritySelector(
                // Out of combat — rest and pre-combat buffs
                new Decorator(
                    ctx => !StyxWoW.Me.Combat,
                    new PrioritySelector(
                        RoutineManager.Current.RestBehavior,
                        RoutineManager.Current.PreCombatBuffBehavior
                    )),

                // In combat — heal, combat buffs
                new Decorator(
                    ctx => StyxWoW.Me.Combat,
                    new PrioritySelector(
                        RoutineManager.Current.HealBehavior,
                        RoutineManager.Current.CombatBuffBehavior
                    )),

                // Kill POI active — pull or move to target
                new Decorator(
                    ctx => BotPoi.Current.Type == PoiType.Kill,
                    new PrioritySelector(
                        // Not in combat + have target — pull
                        new Decorator(
                            ctx => !StyxWoW.Me.Combat && StyxWoW.Me.GotTarget,
                            new PrioritySelector(
                                new Decorator(
                                    ctx => StyxWoW.Me.CurrentTarget.Distance <= BGBuddySettings.Instance.PullDistance,
                                    new PrioritySelector(
                                        RoutineManager.Current.PullBuffBehavior,
                                        RoutineManager.Current.PullBehavior
                                    )),
                                BGBuddy.CreateMoveToLocationBehavior(ctx => StyxWoW.Me.CurrentTarget.Location, true, 2f)
                            )),
                        // In combat + have target — combat routine
                        new Decorator(
                            ctx => StyxWoW.Me.GotTarget && StyxWoW.Me.Combat,
                            new PrioritySelector(RoutineManager.Current.CombatBehavior)),
                        new ActionAlwaysSucceed()
                    ))
            );
        }

        #endregion

        #region Hotspot Management

        /// <summary>
        /// Sets hotspot from a named box in the profile, with randomization. Returns Success if set.
        /// </summary>
        protected RunStatus SetHotspot(string type, string reason)
        {
            return SetHotspot(type, true, reason);
        }

        protected RunStatus SetHotspot(string type, bool randomize, string reason)
        {
            if (BotPoi.Current.Type != PoiType.Hotspot)
                ActualHotspot = WoWPoint.Zero;

            if (!Profile.Boxes.ContainsKey(Side))
                return RunStatus.Failure;

            MapBox box = Profile.Boxes[Side].FirstOrDefault(b => b.Name == type);
            if (box.Center == Vector3.Zero)
                return RunStatus.Failure;

            // Already heading to this hotspot and close enough — keep going
            if (BotPoi.Current.Type == PoiType.Hotspot)
            {
                if (StyxWoW.Me.Location.DistanceSqr(ActualHotspot) > 225f)
                    return RunStatus.Success;
                if (ActualHotspot.Equals(box.Center))
                    return RunStatus.Success;
            }

            SetHotspotTimer.Reset();
            ActualHotspot = box.Center;
            Hotspot = randomize ? RandomizeLocation(box.Center) : box.Center;
            BotPoi.Current = new BotPoi(Hotspot, PoiType.Hotspot);
            Logger.Write(string.Format(BGBuddyResources.Battleground_SetHotspot_Moving_to__0___Reason___1__, box.Name, reason));
            return RunStatus.Success;
        }

        protected RunStatus SetHotspot(WoWPoint location)
        {
            return SetHotspot(location, BGBuddyResources.Battleground_SetHotspot_biggest_fight);
        }

        protected RunStatus SetHotspot(WoWPoint location, string reason)
        {
            return SetHotspot(location, false, reason);
        }

        protected RunStatus SetHotspot(WoWPoint location, float overrideDistance, string reason)
        {
            return SetHotspot(location, overrideDistance, true, reason);
        }

        protected RunStatus SetHotspot(WoWPoint location, bool ignoreTimer, string reason)
        {
            return SetHotspot(location, 30f, ignoreTimer, reason);
        }

        /// <summary>
        /// Sets hotspot to an arbitrary location with optional randomization and distance threshold.
        /// </summary>
        protected RunStatus SetHotspot(WoWPoint location, float overrideDistance, bool ignoreTimer, string reason)
        {
            if (location == WoWPoint.Zero)
                return RunStatus.Failure;

            // Already close to this hotspot — don't change
            if (BotPoi.Current.Type == PoiType.Hotspot)
            {
                if (StyxWoW.Me.Location.Distance2DSqr(ActualHotspot) > 400f)
                    return RunStatus.Success;
                if (ActualHotspot.DistanceSqr(location) < overrideDistance * overrideDistance)
                    return RunStatus.Success;
            }

            SetHotspotTimer.Reset();
            ActualHotspot = location;
            // Randomize only if the hotspot timer has expired (prevents constant re-randomization)
            Hotspot = HotspotTimer.IsFinished ? RandomizeLocation(location) : location;
            BotPoi.Current = new BotPoi(Hotspot, PoiType.Hotspot);
            Logger.Write(string.Format(BGBuddyResources.Battleground_SetHotspot_Moving_to__0_, reason));
            return RunStatus.Success;
        }

        /// <summary>
        /// Gets a named MapBox from the profile for the current side.
        /// </summary>
        protected MapBox GetLandmarkBox(string type)
        {
            if (!Profile.Boxes.ContainsKey(Side))
                return default(MapBox);

            return Profile.Boxes[Side].FirstOrDefault(b => b.Name == type.ToString());
        }

        #endregion
    }
}
