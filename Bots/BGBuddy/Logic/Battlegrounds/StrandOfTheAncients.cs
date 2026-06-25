// Ported from: .hb 4.3.4/Honorbuddy/Honorbuddy/Bots/BGBuddy/Logic/Battlegrounds/StrandOfTheAncients.cs
// Source: .hb 4.3.4/Honorbuddy/Honorbuddy/ns17/Class55.cs
// Target path: Bots/BGBuddy/Logic/Battlegrounds/StrandOfTheAncients.cs
//
// Navigation adaptation note:
// HB 4.3.4 (Tripper.RecastManaged) iterated every Poly of every loaded MeshTile and
// toggled poly.Flags directly to (de)activate gate / portal polygons.
// CopilotBuddy uses a different navmesh API surface (no public Mesh/Poly access):
//   - Navigator.TripperNavigator.SetPolyFlags(mapId, polyRef, flags)
//   - Navigator.TripperNavigator.GetPolyArea(mapId, polyRef, out area)
//   - Navigator.TripperNavigator.QueryPolygons(mapId, center, extents, maxPolys)
//   - Navigator.TripperNavigator.TileLoaded event
// On TileLoaded we re-query polygons in the tile bounds and SetPolyFlags per
// polyRef. There is no public "UnloadAllTiles" in the streaming model; when gate
// state changes we simply re-flag the affected tiles via RefreshGates().

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bots.BGBuddy.Helpers;
using Bots.BGBuddy.Resources;
using CommonBehaviors.Actions;
using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;
using Navigator = Styx.Logic.Pathing.Navigator;
using Tripper.Navigation;
using Tripper.Tools.Math;

namespace Bots.BGBuddy.Logic.Battlegrounds
{
    /// <summary>
    /// Strand of the Ancients (map 607) — multi-stage assault on Titan relics.
    /// Gates (Yellow Moon, Purple Amethyst, Red Sun, Blue Sapphire, Green Emerald)
    /// block pathing until destroyed. Defender portals (DefendersPortal area type)
    /// are deactivated while we are on offense so we do not path backwards into
    /// the central courtyard through the wrong portal.
    ///
    /// The base BotBase normally calls CreateBeforePrepBehavior() during the 60s
    /// Preparation phase to mount up and run to the boats. We override that to
    /// instead click-to-move toward the boat pier and wait for the teleport buff.
    /// </summary>
    internal sealed class StrandOfTheAncients : Battleground
    {
        private readonly WaitTimer _boatTimer = new WaitTimer(TimeSpan.FromSeconds(5));
        private bool _teleportBuffActive;
        private bool _initialSide;

        // Approximate pier (boat) waypoints used to ClickToMove toward once Preparation ends.
        private static readonly HashSet<WoWPoint> BoatPierPoints = new HashSet<WoWPoint>
        {
            new WoWPoint(1610.332f, 50.70834f, 7.579856f),
            new WoWPoint(1599.641f, -102.9774f, 8.8739f),
        };

        // Navmesh AreaType IDs for the gate polygons (MaNGOS / HB RecastManaged).
        // These come from MoveMapSharedDefines.h::NavTerrain + the SotA-specific
        // MISC1..MISC5 slots used by the SotA navmesh data.
        private const byte NavAreaDefendersPortal = 9;
        private const byte NavAreaSotAGate1 = 20;
        private const byte NavAreaSotAGate2 = 21;
        private const byte NavAreaSotAGate3 = 22;
        private const byte NavAreaSotAGate4 = 23;
        private const byte NavAreaSotAGate5 = 24;

        // Detour poly flags: 1 = walk, 16 = block, 32 = swim (HB convention reused).
        private const ushort PolyFlagWalk = 1;
        private const ushort PolyFlagBlock = 16;
        private const ushort PolyFlagSwim = 32;

        public override string Name => BGBuddyResources.StrandOfTheAncients;
        public override int MapId => 607;

        public override void Dispose()
        {
            BGBuddy.Instance.WorldStatesUpdated -= RefreshLandmarks;
            Navigator.TripperNavigator.TileLoaded -= OnTileLoaded;
            BotEvents.OnBotStart -= OnBotStarted;
            Statuses.Clear();
        }

        public override void Start()
        {
            LoadProfile();
            StartingLocation = RandomizeLocation(StartingLocation, 10);
            RefreshLandmarks();
            BGBuddy.Instance.WorldStatesUpdated += RefreshLandmarks;
            Navigator.TripperNavigator.TileLoaded += OnTileLoaded;
            BotEvents.OnBotStart += OnBotStarted;
            _initialSide = IsAttacking;
        }

        // Called by BGBuddy's CreateBeforePrepBehavior override during the
        // 60s Preparation phase. Mount up + run to the boat + wait for teleport.
        // HB 4.3.4 Class55.method_19..22 / smethod_58..62.
        //
        // Fix: was using go.DistanceSqr < 400f (any Transport GO within 20 yards) to detect
        // "on boat" — this was always TRUE at the docks since the boat stays moored nearby.
        // Now uses StyxWoW.Me.IsOnTransport (same property as Battleground.cs:422) which is
        // only TRUE when the player is physically riding the transport.
        protected override Composite CreateBeforePrepBehavior()
        {
            return new PrioritySelector(
                // [0] smethod_58: Preparation aura still up — move toward nearest pier and
                // keep timers warm. Returns Failure so the outer loop re-evaluates next tick.
                new Decorator(ctx => StyxWoW.Me.HasAura("Preparation"),
                    new Action(ctx =>
                    {
                        _boatTimer.Reset();
                        var pier = BoatPierPoints.OrderBy(p => p.DistanceSqr(StyxWoW.Me.Location)).First();
                        if (StyxWoW.Me.Location.DistanceSqr(pier) > 100f) // > 10 yards — keep moving
                            WoWMovement.ClickToMove(pier);
                        return RunStatus.Failure;
                    })),
                // [1] smethod_59: past Preparation AND physically on the boat transport —
                // wait the 5s timer then ClickToMove to pier to debark.
                // StyxWoW.Me.IsOnTransport is the correct check (cf. Battleground.cs:422).
                new Decorator(ctx => StyxWoW.Me.IsOnTransport,
                    new PrioritySelector(
                        new Decorator(ctx => !_boatTimer.IsFinished,
                            new Action(ctx => { Logger.Write(BGBuddyResources.WaitingForBoat); return RunStatus.Running; })),
                        new Sequence(
                            new Action(ctx => { Logger.Write(BGBuddyResources.GettingOfTheBoat); return RunStatus.Success; }),
                            new Action(ctx =>
                            {
                                _boatTimer.Reset();
                                WoWMovement.ClickToMove(BoatPierPoints.OrderBy(p => p.DistanceSqr(StyxWoW.Me.Location)).First());
                                return RunStatus.Success;
                            })))),
                // [2] Fallback: side change + teleport buff toggle detection.
                // HB 4.3.4 Class55.method_22 — runs as the last child of CreateBeforePrepBehavior's
                // PrioritySelector, i.e. only when we are not in Preparation and not on the boat.
                new Action(ctx => CheckTeleportBuffChange()));
        }

        public override Composite Logic
        {
            get
            {
                return new Sequence(
                    new Switch<LogicType>(
                        ctx => BGBuddySettings.Instance.SotaLogicType,
                        new SwitchArgument<LogicType>(LogicType.Attack, BuildAttackLogic())),
                    new Decorator(ctx => BotPoi.Current.Type == PoiType.Hotspot,
                        BGBuddy.CreateMoveToLocationBehavior(ctx => Hotspot, true, 5f)));
            }
        }

        private void CheckTeleportBuffChange()
        {
            bool hasTeleport = StyxWoW.Me.HasAura("Teleport");
            if (_teleportBuffActive == hasTeleport) return;

            _teleportBuffActive = hasTeleport;
            // Side change (Attack/Defend flip) means profile and gate priorities need a refresh.
            if (_initialSide != IsAttacking)
            {
                LoadProfile();
                StartingLocation = RandomizeLocation(StartingLocation, 10);
                _initialSide = IsAttacking;
            }
            ReloadGates("Teleport buff toggled");
        }

        private void OnBotStarted(EventArgs e) => ReloadGates("Bot start");

        // BuildAttackLogic — HB 4.3.4 Class55.method_18.
        // 7 children: 4 gate-rush priorities + 2 context-changed priority selectors
        // (BiggestFight, BiggestFriendlyPack) + ActionAlwaysSucceed.
        // Order is the original decompiled order: inner relic gate first, then fall
        // through each outer gate, then opportunistic fight/pack following.
        private Composite BuildAttackLogic()
        {
            return new PrioritySelector(
                // [0] method_24/25: Type 64 (Yellow Moon, inner gate) destroyed → push to relic chamber.
                new Decorator(ctx => AnyStatus(lm => lm.Type == (int)StrandOfTheAncientsLandmarkType.GateOfTheYellowMoon
                                                  && lm.Control == LandmarkControlType.Destroyed),
                    new Action(ctx => SetHotspot(StrandOfTheAncientsLandmarkType.ChamberOfAncientRelics.ToString(), true, Side.ToString()))),
                // [1] method_26/27: Type 32 (Green Emerald) OR Type 16 (Purple Amethyst) destroyed → Yellow Moon.
                new Decorator(ctx => AnyStatus(lm => (lm.Type == (int)StrandOfTheAncientsLandmarkType.GateOfTheGreenEmerald
                                                   || lm.Type == (int)StrandOfTheAncientsLandmarkType.GateOfThePurpleAmethyst)
                                                  && lm.Control == LandmarkControlType.Destroyed),
                    new Action(ctx => SetHotspot(StrandOfTheAncientsLandmarkType.GateOfTheYellowMoon.ToString(), true, Side.ToString()))),
                // [2] method_28/29: Type 8 (Red Sun) destroyed → Purple Amethyst.
                new Decorator(ctx => AnyStatus(lm => lm.Type == (int)StrandOfTheAncientsLandmarkType.GateOfTheRedSun
                                                  && lm.Control == LandmarkControlType.Destroyed),
                    new Action(ctx => SetHotspot(StrandOfTheAncientsLandmarkType.GateOfThePurpleAmethyst.ToString(), true, Side.ToString()))),
                // [3] method_30/31: Type 4 (Blue Sapphire) destroyed → Red Sun.
                new Decorator(ctx => AnyStatus(lm => lm.Type == (int)StrandOfTheAncientsLandmarkType.GateOfTheBlueSapphire
                                                  && lm.Control == LandmarkControlType.Destroyed),
                    new Action(ctx => SetHotspot(StrandOfTheAncientsLandmarkType.GateOfTheRedSun.ToString(), true, Side.ToString()))),
                // [4] method_32/33 (smethod_73): BiggestFight context change → if not Zero, set hotspot.
                new PrioritySelector(ctx => BiggestFight,
                    new Decorator(ctx => ((WoWPoint)ctx) != WoWPoint.Zero,
                        new Action(ctx => SetHotspot((WoWPoint)ctx)))),
                // [5] method_34/35 (smethod_74): BiggestFriendlyPack context change → if not Zero, set hotspot.
                new PrioritySelector(ctx => BiggestFriendlyPack,
                    new Decorator(ctx => ((WoWPoint)ctx) != WoWPoint.Zero,
                        new Action(ctx => SetHotspot((WoWPoint)ctx, 10f, BGBuddyResources.BiggestFriendlyPack)))),
                // [6] Fallback.
                new ActionAlwaysSucceed());
        }

        private void RefreshLandmarks()
        {
            Styx.Logic.Battlegrounds.LandMarks.Refresh();
            bool anyGateChange = false;
            var sb = new StringBuilder();
            sb.AppendLine();

            foreach (var landmark in Styx.Logic.Battlegrounds.LandMarks.LandmarkList)
            {
                var sota = landmark.ToStrandOfTheAncientsLandmark();
                var isGateType = (sota.LandmarkType & StrandOfTheAncientsLandmarkType.Gates) != StrandOfTheAncientsLandmarkType.Unknown;

                if (!Statuses.TryGetValue((int)sota.LandmarkType, out var existing))
                {
                    Statuses[(int)sota.LandmarkType] = new LandmarkInfo(
                        (int)sota.LandmarkType,
                        sota.ControlType,
                        GetLandmarkBox(sota.LandmarkType.ToString()));
                    if (isGateType)
                    {
                        anyGateChange = true;
                        sb.AppendLine($"{sota.LandmarkType} has been added");
                    }
                }
                else if (existing.Control != sota.ControlType && isGateType)
                {
                    anyGateChange = true;
                    sb.AppendLine($"State of {sota.LandmarkType} changed. Old Value: {existing.Control} New Value: {sota.ControlType}");
                    Statuses[(int)sota.LandmarkType] = new LandmarkInfo(
                        (int)sota.LandmarkType,
                        sota.ControlType,
                        GetLandmarkBox(sota.LandmarkType.ToString()));
                }
                else
                {
                    Statuses[(int)sota.LandmarkType] = new LandmarkInfo(
                        (int)sota.LandmarkType,
                        sota.ControlType,
                        GetLandmarkBox(sota.LandmarkType.ToString()));
                }
                Statuses[(int)sota.LandmarkType].Process();
            }

            // Drop Statuses whose landmark no longer exists.
            var activeTypes = Styx.Logic.Battlegrounds.LandMarks.LandmarkList
                .Select(l => l.ToStrandOfTheAncientsLandmark().LandmarkType)
                .ToHashSet();
            var stale = Statuses.Keys.Where(k => !activeTypes.Contains((StrandOfTheAncientsLandmarkType)k)).ToList();
            foreach (var k in stale) Statuses.Remove(k);

            if (anyGateChange) ReloadGates(sb.ToString());
        }

        // The TileLoaded callback: re-flag every navmesh polygon in this tile
        // according to the current gate state.
        private void OnTileLoaded(object sender, Tripper.Navigation.TileLoadedEventArgs e)
        {
            if (e.MapId != (uint)MapId) return;
            var tileCenter = TileWorldCenter(e.TileX, e.TileY);
            var extents = new System.Numerics.Vector3(MapConsts.TileSize / 2f, MapConsts.TileSize / 2f, 200f);
            var polys = Navigator.TripperNavigator.QueryPolygons(e.MapId, tileCenter, extents, 8192);

            var destroyedGates = Statuses.Values
                .Where(lm => lm.Control == LandmarkControlType.Destroyed)
                .ToList();

            bool hasTeleport = StyxWoW.Me.HasAura("Teleport");

            foreach (var polyRef in polys)
            {
                Navigator.TripperNavigator.GetPolyArea(e.MapId, polyRef, out byte area);
                switch (area)
                {
                    case NavAreaDefendersPortal:
                        // Block our path through defender portals while on offense (or while we have teleport buff).
                        Navigator.TripperNavigator.SetPolyFlags(
                            e.MapId, polyRef, (IsAttacking || hasTeleport) ? PolyFlagBlock : PolyFlagSwim);
                        break;
                    case NavAreaSotAGate1: // Purple Amethyst (Type 8)
                        Navigator.TripperNavigator.SetPolyFlags(
                            e.MapId, polyRef,
                            destroyedGates.Any(lm => lm.Type == (int)StrandOfTheAncientsLandmarkType.GateOfThePurpleAmethyst)
                                ? PolyFlagWalk : PolyFlagBlock);
                        break;
                    case NavAreaSotAGate2: // Red Sun (Type 4)
                        Navigator.TripperNavigator.SetPolyFlags(
                            e.MapId, polyRef,
                            destroyedGates.Any(lm => lm.Type == (int)StrandOfTheAncientsLandmarkType.GateOfTheRedSun)
                                ? PolyFlagWalk : PolyFlagBlock);
                        break;
                    case NavAreaSotAGate3: // Purple Amethyst inner? (Type 32)
                        Navigator.TripperNavigator.SetPolyFlags(
                            e.MapId, polyRef,
                            destroyedGates.Any(lm => lm.Type == (int)StrandOfTheAncientsLandmarkType.GateOfThePurpleAmethyst)
                                ? PolyFlagWalk : PolyFlagBlock);
                        break;
                    case NavAreaSotAGate4: // Green Emerald (Type 16)
                        Navigator.TripperNavigator.SetPolyFlags(
                            e.MapId, polyRef,
                            destroyedGates.Any(lm => lm.Type == (int)StrandOfTheAncientsLandmarkType.GateOfTheGreenEmerald)
                                ? PolyFlagWalk : PolyFlagBlock);
                        break;
                    case NavAreaSotAGate5: // Blue Sapphire (Type 64)
                        Navigator.TripperNavigator.SetPolyFlags(
                            e.MapId, polyRef,
                            destroyedGates.Any(lm => lm.Type == (int)StrandOfTheAncientsLandmarkType.GateOfTheBlueSapphire)
                                ? PolyFlagWalk : PolyFlagBlock);
                        break;
                }
            }
        }

        // The CopilotBuddy navmesh is a streaming model — there is no public
        // "UnloadAllTiles" exposed. We force a re-query of every loaded tile
        // by walking them and re-flagging.
        private void ReloadGates(string reason)
        {
            Logger.WriteDebug($"Reloading gates. Reason: {reason}");
            // Fire a synthetic OnTileLoaded for every (tileX, tileY) currently in the mesh.
            // We re-walk the Statuses' landmark areas since they are spatially distributed
            // across the map, but the cleanest fallback is to QueryPolygons around each
            // active gate's box center.
            foreach (var lm in Statuses.Values)
            {
                if (lm.Box.Center == Vector3.Zero) continue;
                var center = new System.Numerics.Vector3(lm.Box.Center.X, lm.Box.Center.Y, lm.Box.Center.Z);
                var polys = Navigator.TripperNavigator.QueryPolygons((uint)MapId, center, new System.Numerics.Vector3(80f, 80f, 50f), 256);
                foreach (var polyRef in polys)
                {
                    Navigator.TripperNavigator.GetPolyArea((uint)MapId, polyRef, out byte area);
                    // Generic block-by-default for any gate area
                    if (area >= NavAreaSotAGate1 && area <= NavAreaSotAGate5)
                    {
                        bool isDestroyed = lm.Control == LandmarkControlType.Destroyed;
                        Navigator.TripperNavigator.SetPolyFlags((uint)MapId, polyRef, isDestroyed ? PolyFlagWalk : PolyFlagBlock);
                    }
                    else if (area == NavAreaDefendersPortal)
                    {
                        Navigator.TripperNavigator.SetPolyFlags(
                            (uint)MapId, polyRef,
                            (IsAttacking || _teleportBuffActive) ? PolyFlagBlock : PolyFlagSwim);
                    }
                }
            }
        }

        private static System.Numerics.Vector3 TileWorldCenter(int tileX, int tileY)
            => new System.Numerics.Vector3(
                (32f - tileX - 0.5f) * MapConsts.TileSize,
                (32f - tileY - 0.5f) * MapConsts.TileSize,
                0f);

        private bool AnyStatus(Func<LandmarkInfo, bool> predicate)
            => Statuses.Values.Any(predicate);
    }
}