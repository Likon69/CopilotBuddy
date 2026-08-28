// Source: .hb 4.3.4/Honorbuddy/Honorbuddy/ns28/Class54.cs
//
// Navigation adaptation note:
// HB 4.3.4 (Tripper.RecastManaged) iterated every Poly of every loaded MeshTile and
// toggled poly.Flags directly. CopilotBuddy exposes the navmesh via:
//   - Navigator.TripperNavigator.SetPolyFlags(mapId, polyRef, flags)
//   - Navigator.TripperNavigator.GetPolyArea(mapId, polyRef, out area)
//   - Navigator.TripperNavigator.QueryPolygons(mapId, center, extents, maxPolys)
//   - Navigator.TripperNavigator.TileLoaded event
// On TileLoaded we re-query polygons in the tile bounds and SetPolyFlags per
// polyRef. The "UnloadAllTiles" call that HB 4.3.4 used to force a fresh mesh
// after a gate state change is replaced by a synchronous re-query + re-flag
// pass (the streaming model has no public unload).

using System;
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
using TreeSharp;
using Action = TreeSharp.Action;
using Navigator = Styx.Logic.Pathing.Navigator;
using Tripper.Navigation;
using Tripper.Tools.Math;

namespace Bots.BGBuddy.Logic.Battlegrounds
{
    /// <summary>
    /// Isle of Conquest (map 628) — 2-keep assault with 6 gates and 2 faction-specific
    /// portals. Gates (Alliance: 7, Horde: 112) block pathing until destroyed.
    /// Portals (Horde=10, Alliance=11) are blocked for the opposing faction.
    /// The front gates (21=Horde front, 23=Alliance front) stay walkable for the
    /// first 90 seconds of the BG so both teams can move out of their keep area.
    /// </summary>
    internal sealed class IsleOfConquest : Battleground
    {
        private readonly WaitTimer _refreshTimer = new WaitTimer(TimeSpan.FromSeconds(2));
        private readonly WaitTimer _startOfGameTimer = new WaitTimer(TimeSpan.FromSeconds(90));
        private bool _teleportBuffActive;
        private bool _startOfGameReloaded;

        // Navmesh AreaType IDs (MaNGOS / HB RecastManaged, see MoveMapSharedDefines.h).
        private const byte NavAreaHordePortal = 10;
        private const byte NavAreaAlliancePortal = 11;
        private const byte NavAreaHordeWestGate = 20;
        private const byte NavAreaHordeFrontGate = 21;
        private const byte NavAreaHordeEastGate = 22;
        private const byte NavAreaAllianceFrontGate = 23;
        private const byte NavAreaAllianceWestGate = 24;
        private const byte NavAreaAllianceEastGate = 25;

        // Detour poly flags: 1 = walk, 16 = block, 32 = portal/swim (HB convention).
        private const ushort PolyFlagWalk = 1;
        private const ushort PolyFlagBlock = 16;
        private const ushort PolyFlagPortal = 32;

        public override string Name => BGBuddyResources.IsleOfConquest;
        public override int MapId => 628;

        public override void Dispose()
        {
            BGBuddy.Instance.WorldStatesUpdated -= RefreshLandmarks;
            Navigator.TripperNavigator.TileLoaded -= OnTileLoaded;
            _startOfGameTimer.Finished -= OnStartOfGameTimer;
            BotEvents.OnBotStart -= OnBotStarted;
            _startOfGameReloaded = false;
            Statuses.Clear();
        }

        public override void Start()
        {
            LoadProfile();
            StartingLocation = RandomizeLocation(StartingLocation, 5);
            _startOfGameTimer.Reset();
            RefreshLandmarks();
            BGBuddy.Instance.WorldStatesUpdated += RefreshLandmarks;
            Navigator.TripperNavigator.TileLoaded += OnTileLoaded;
            _startOfGameTimer.Finished += OnStartOfGameTimer;
            BotEvents.OnBotStart += OnBotStarted;
        }

        // Runs during the (non-existent) prep phase of IoC. Detects toggle of
        // the "Teleport" buff (the BG-ready aura) and triggers a navmesh re-flag
        // so portal walkability stays in sync with the player's actual side.
        // IoC has no real prep phase, but the buff can still flip if the player
        // dies during the count-in and rezzes back. HB 4.3.4 Class54.method_19.
        protected override Composite CreateBeforePrepBehavior()
        {
            return new PrioritySelector(
                new Action(ctx =>
                {
                    bool hasTeleport = StyxWoW.Me.HasAura("Teleport");
                    if (_teleportBuffActive != hasTeleport)
                    {
                        _teleportBuffActive = hasTeleport;
                        ReloadGates("Teleport Buff");
                    }
                    return RunStatus.Failure;
                }));
        }

        public override Composite Logic
        {
            get
            {
                return new Sequence(
                    new Switch<LogicType>(
                        ctx => BGBuddySettings.Instance.IocLogicType,
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
            bool anyGateChange = false;
            var sb = new StringBuilder();
            sb.AppendLine();

            foreach (var landmark in Styx.Logic.Battlegrounds.LandMarks.LandmarkList)
            {
                var ioc = landmark.ToIsleOfConquestLandmark();
                var isGateType = (ioc.LandmarkType & IsleOfConquestLandmarkType.Gates) != IsleOfConquestLandmarkType.Unknown;

                if (!Statuses.TryGetValue((int)ioc.LandmarkType, out var existing))
                {
                    Statuses[(int)ioc.LandmarkType] = new LandmarkInfo(
                        (int)ioc.LandmarkType,
                        ioc.ControlType,
                        GetLandmarkBox(ioc.LandmarkType.ToString()));
                    if (isGateType)
                    {
                        anyGateChange = true;
                        sb.AppendLine($"New gate added [{ioc.LandmarkType}]");
                    }
                }
                else if (existing.Control != ioc.ControlType && isGateType)
                {
                    anyGateChange = true;
                    sb.AppendLine($"State of {ioc.LandmarkType} changed. Old Value: {existing.Control} New Value: {ioc.ControlType}");
                    Statuses[(int)ioc.LandmarkType] = new LandmarkInfo(
                        (int)ioc.LandmarkType,
                        ioc.ControlType,
                        GetLandmarkBox(ioc.LandmarkType.ToString()));
                }
                else
                {
                    Statuses[(int)ioc.LandmarkType] = new LandmarkInfo(
                        (int)ioc.LandmarkType,
                        ioc.ControlType,
                        GetLandmarkBox(ioc.LandmarkType.ToString()));
                }
                Statuses[(int)ioc.LandmarkType].Process();
            }

            if (anyGateChange) ReloadGates(sb.ToString());
        }

        private Composite BuildAttackLogic()
        {
            return new PrioritySelector(
                // An enemy keep gate is destroyed — push the keep directly.
                new Decorator(ctx => AttackToKeep != IsleOfConquestLandmarkType.Unknown,
                    new Action(ctx => SetHotspot(AttackToKeep.ToString(), BGBuddyResources.EnemyKeep))),
                new PrioritySelector(ctx => BiggestFight,
                    new Decorator(ctx => ((WoWPoint)ctx) != WoWPoint.Zero,
                        new Action(ctx => SetHotspot((WoWPoint)ctx)))),
                new PrioritySelector(ctx => BiggestFriendlyPack,
                    new Decorator(ctx => ((WoWPoint)ctx) != WoWPoint.Zero,
                        new Action(ctx => SetHotspot((WoWPoint)ctx, 10f, BGBuddyResources.BiggestFriendlyPack)))),
                new ActionAlwaysSucceed());
        }

        // Returns the enemy keep landmark if at least one of its gates is destroyed,
        // else Unknown.
        private IsleOfConquestLandmarkType AttackToKeep
        {
            get
            {
                bool isHorde = StyxWoW.Me.IsHorde;
                var destroyedGates = Statuses.Values
                    .Where(lm => lm.Control == LandmarkControlType.Destroyed)
                    .Select(lm => (IsleOfConquestLandmarkType)lm.Type)
                    .ToList();

                foreach (var gate in destroyedGates)
                {
                    if ((gate & IsleOfConquestLandmarkType.AllianceGates) != IsleOfConquestLandmarkType.Unknown && isHorde)
                        return IsleOfConquestLandmarkType.AllianceKeep;
                    if ((gate & IsleOfConquestLandmarkType.HordeGates) != IsleOfConquestLandmarkType.Unknown && !isHorde)
                        return IsleOfConquestLandmarkType.HordeKeep;
                }
                return IsleOfConquestLandmarkType.Unknown;
            }
        }

        private void OnStartOfGameTimer(object sender, WaitTimer.WaitTimerEventArgs e)
        {
            if (_startOfGameReloaded) return;
            ReloadGates("Start of Game Timer Finished");
            _startOfGameReloaded = true;
        }

        private void OnBotStarted(EventArgs e) => ReloadGates("Bot start");

        // TileLoaded callback: re-flag every navmesh polygon in this tile
        // according to the current gate state and the start-of-game grace window.
        private void OnTileLoaded(object sender, Tripper.Navigation.TileLoadedEventArgs e)
        {
            if (e.MapId != (uint)MapId) return;
            var tileCenter = TileWorldCenter(e.TileX, e.TileY);
            var extents = new System.Numerics.Vector3(MapConsts.TileSize / 2f, MapConsts.TileSize / 2f, 200f);
            var polys = Navigator.TripperNavigator.QueryPolygons(e.MapId, tileCenter, extents, 8192);

            bool isHorde = StyxWoW.Me.IsHorde;
            bool hasTeleport = StyxWoW.Me.HasAura("Teleport");
            bool startOfGame = !_startOfGameTimer.IsFinished;
            var destroyedGates = Statuses.Values
                .Where(lm => lm.Control == LandmarkControlType.Destroyed)
                .ToList();

            foreach (var polyRef in polys)
            {
                Navigator.TripperNavigator.GetPolyArea(e.MapId, polyRef, out byte area);
                switch (area)
                {
                    case NavAreaHordePortal:
                        // Block the Horde portal for everyone except Horde once the start-of-game grace ends.
                        Navigator.TripperNavigator.SetPolyFlags(
                            e.MapId, polyRef,
                            (!isHorde || startOfGame || hasTeleport) ? PolyFlagBlock : PolyFlagPortal);
                        break;
                    case NavAreaAlliancePortal:
                        Navigator.TripperNavigator.SetPolyFlags(
                            e.MapId, polyRef,
                            (isHorde || startOfGame || hasTeleport) ? PolyFlagBlock : PolyFlagPortal);
                        break;
                    case NavAreaHordeWestGate:
                        Navigator.TripperNavigator.SetPolyFlags(
                            e.MapId, polyRef,
                            destroyedGates.Any(lm => lm.Type == (int)IsleOfConquestLandmarkType.HordeGateWest)
                                ? PolyFlagWalk : PolyFlagBlock);
                        break;
                    case NavAreaHordeFrontGate:
                        // Walkable if destroyed OR if we are still in the start-of-game grace window.
                        Navigator.TripperNavigator.SetPolyFlags(
                            e.MapId, polyRef,
                            (destroyedGates.Any(lm => lm.Type == (int)IsleOfConquestLandmarkType.HordeGateFront) || startOfGame)
                                ? PolyFlagWalk : PolyFlagBlock);
                        break;
                    case NavAreaHordeEastGate:
                        Navigator.TripperNavigator.SetPolyFlags(
                            e.MapId, polyRef,
                            destroyedGates.Any(lm => lm.Type == (int)IsleOfConquestLandmarkType.HordeGateEast)
                                ? PolyFlagWalk : PolyFlagBlock);
                        break;
                    case NavAreaAllianceFrontGate:
                        Navigator.TripperNavigator.SetPolyFlags(
                            e.MapId, polyRef,
                            (destroyedGates.Any(lm => lm.Type == (int)IsleOfConquestLandmarkType.AllianceGateFront) || startOfGame)
                                ? PolyFlagWalk : PolyFlagBlock);
                        break;
                    case NavAreaAllianceWestGate:
                        Navigator.TripperNavigator.SetPolyFlags(
                            e.MapId, polyRef,
                            destroyedGates.Any(lm => lm.Type == (int)IsleOfConquestLandmarkType.AllianceGateWest)
                                ? PolyFlagWalk : PolyFlagBlock);
                        break;
                    case NavAreaAllianceEastGate:
                        Navigator.TripperNavigator.SetPolyFlags(
                            e.MapId, polyRef,
                            destroyedGates.Any(lm => lm.Type == (int)IsleOfConquestLandmarkType.AllianceGateEast)
                                ? PolyFlagWalk : PolyFlagBlock);
                        break;
                }
            }
        }

        // Re-flag polygons around every active gate center. Replaces the
        // "UnloadAllTiles + reload" pattern from HB 4.3.4 (the streaming navmesh
        // has no public unload).
        private void ReloadGates(string reason)
        {
            Logger.WriteDebug($"Reloading gates. Reason: {reason}");
            bool isHorde = StyxWoW.Me.IsHorde;
            bool hasTeleport = StyxWoW.Me.HasAura("Teleport");
            bool startOfGame = !_startOfGameTimer.IsFinished;
            var destroyedGates = Statuses.Values
                .Where(lm => lm.Control == LandmarkControlType.Destroyed)
                .ToList();

            foreach (var lm in Statuses.Values)
            {
                if (lm.Box.Center == Vector3.Zero) continue;
                var center = new System.Numerics.Vector3(lm.Box.Center.X, lm.Box.Center.Y, lm.Box.Center.Z);
                var polys = Navigator.TripperNavigator.QueryPolygons((uint)MapId, center, new System.Numerics.Vector3(80f, 80f, 50f), 256);
                foreach (var polyRef in polys)
                {
                    Navigator.TripperNavigator.GetPolyArea((uint)MapId, polyRef, out byte area);
                    switch (area)
                    {
                        case NavAreaHordePortal:
                            Navigator.TripperNavigator.SetPolyFlags(
                                (uint)MapId, polyRef,
                                (!isHorde || startOfGame || hasTeleport) ? PolyFlagBlock : PolyFlagPortal);
                            break;
                        case NavAreaAlliancePortal:
                            Navigator.TripperNavigator.SetPolyFlags(
                                (uint)MapId, polyRef,
                                (isHorde || startOfGame || hasTeleport) ? PolyFlagBlock : PolyFlagPortal);
                            break;
                        case NavAreaHordeWestGate:
                            Navigator.TripperNavigator.SetPolyFlags(
                                (uint)MapId, polyRef,
                                destroyedGates.Any(g => g.Type == (int)IsleOfConquestLandmarkType.HordeGateWest)
                                    ? PolyFlagWalk : PolyFlagBlock);
                            break;
                        case NavAreaHordeFrontGate:
                            Navigator.TripperNavigator.SetPolyFlags(
                                (uint)MapId, polyRef,
                                (destroyedGates.Any(g => g.Type == (int)IsleOfConquestLandmarkType.HordeGateFront) || startOfGame)
                                    ? PolyFlagWalk : PolyFlagBlock);
                            break;
                        case NavAreaHordeEastGate:
                            Navigator.TripperNavigator.SetPolyFlags(
                                (uint)MapId, polyRef,
                                destroyedGates.Any(g => g.Type == (int)IsleOfConquestLandmarkType.HordeGateEast)
                                    ? PolyFlagWalk : PolyFlagBlock);
                            break;
                        case NavAreaAllianceFrontGate:
                            Navigator.TripperNavigator.SetPolyFlags(
                                (uint)MapId, polyRef,
                                (destroyedGates.Any(g => g.Type == (int)IsleOfConquestLandmarkType.AllianceGateFront) || startOfGame)
                                    ? PolyFlagWalk : PolyFlagBlock);
                            break;
                        case NavAreaAllianceWestGate:
                            Navigator.TripperNavigator.SetPolyFlags(
                                (uint)MapId, polyRef,
                                destroyedGates.Any(g => g.Type == (int)IsleOfConquestLandmarkType.AllianceGateWest)
                                    ? PolyFlagWalk : PolyFlagBlock);
                            break;
                        case NavAreaAllianceEastGate:
                            Navigator.TripperNavigator.SetPolyFlags(
                                (uint)MapId, polyRef,
                                destroyedGates.Any(g => g.Type == (int)IsleOfConquestLandmarkType.AllianceGateEast)
                                    ? PolyFlagWalk : PolyFlagBlock);
                            break;
                    }
                }
            }
        }

        private static System.Numerics.Vector3 TileWorldCenter(int tileX, int tileY)
            => new System.Numerics.Vector3(
                (32f - tileX - 0.5f) * MapConsts.TileSize,
                (32f - tileY - 0.5f) * MapConsts.TileSize,
                0f);
    }
}
