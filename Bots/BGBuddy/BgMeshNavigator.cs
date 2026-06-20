// Ported from: .hb 4.3.4/Honorbuddy/Honorbuddy/ns5/Class80.cs
// Target path: Bots/BGBuddy/BgMeshNavigator.cs
// Deobfuscated: Class80         → BgMeshNavigator
//              method_10        → OnTileLoaded (ADT reading, single event per MaNGOS tile)
//              method_11        → OnTileLoaded
//              bool_1           → _pathRandomized
//              dictionary_0     → _savedPolyAreas
//              random_0         → _random
// Adaptation:  base.Nav.Mesh.SetPolyArea/GetPolyArea/GetPolyFlags
//              → Navigator.TripperNavigator.SetPolyArea/GetPolyArea/GetPolyFlags (CopilotBuddy API)
//              Constructor hooks → OnSetAsCurrent/OnRemoveAsCurrent lifecycle
//              Class80.MoveTo (complete override) → TryRandomizePath + base.MoveTo
//              (Preserves CopilotBuddy's full MeshNavigator.MoveTo logic)

using System;
using System.Collections.Generic;
using System.Numerics;
using Bots.BGBuddy;
using Styx;
using Styx.Helpers;
using StyxNav = Styx.Logic.Pathing.Navigator;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Tripper.Navigation;

namespace Bots.BGBuddy
{
    /// <summary>
    /// BGBuddy-specific navigation provider. Extends MeshNavigator with:
    ///   1. Automatic blackspot injection when navmesh tiles load.
    ///   2. Per-path polygon area randomization to vary routes and avoid bot-like repetition.
    /// </summary>
    /// <remarks>
    /// Source: HB 4.3.4 ns5/Class80.cs — deobfuscated and adapted to CopilotBuddy's
    /// Navigator API (no Nav.Mesh shim; uses Navigator.TripperNavigator directly).
    /// </remarks>
    internal class BgMeshNavigator : MeshNavigator
    {
        #region Fields

        // Map of polygon → original area, for restoring polygons we randomized (HB: dictionary_0)
        private readonly Dictionary<PolygonReference, byte> _savedPolyAreas = new Dictionary<PolygonReference, byte>();

        // Per-path randomization seed (HB: random_0)
        private readonly Random _random = new Random(Guid.NewGuid().GetHashCode());

        // True after we've randomized path polygons for the current destination (HB: bool_1)
        private bool _pathRandomized;

        // Destination for which _pathRandomized is valid
        private WoWPoint _lastRandomizedDest = WoWPoint.Zero;

        #endregion

        #region Lifecycle

        public override void OnSetAsCurrent()
        {
            base.OnSetAsCurrent();
            StyxNav.TripperNavigator.TileLoaded += OnTileLoaded;
        }

        public override void OnRemoveAsCurrent()
        {
            StyxNav.TripperNavigator.TileLoaded -= OnTileLoaded;
            base.OnRemoveAsCurrent();
        }

        #endregion

        #region Door handling override (HB 4.3.4 BgMeshNavigator had no door click)

        // HB 4.3.4 BgMeshNavigator (ns5/Class80) inherits from a MeshNavigator that
        // had no door handling at all. HB 6.2.3 MeshNavigator added method_7/8/29
        // (HandleDoors/IsDoorUsable) for open-world doors. CopilotBuddy's base
        // MeshNavigator is a port of HB 6.2.3, so we inherit HandleDoors by default.
        // BG gates (WSG portcullis entry 179919, IoC gates, SotA gates, AV banners)
        // are script-driven — the server lowers them on prep timer expiry, the WoW
        // API keeps reporting IsClosed=true for several seconds, and the open-world
        // HandleDoors then spam-clicks an already-open sliding door. Override the
        // hook to a no-op so the bot just walks through.
        protected override MoveResult? HandleDoors(LocalPlayer me) => null;

        #endregion

        #region Tile loaded — blackspot injection (HB Class80.method_10 / method_11)

        /// <summary>
        /// Called when a MaNGOS ADT navmesh tile is loaded.
        /// Resets path randomization state and re-injects BG profile blackspots.
        /// </summary>
        private void OnTileLoaded(object? sender, TileLoadedEventArgs e)
        {
            _pathRandomized = false;
            Battleground? currentBattleground = BGBuddy.CurrentBattleground;
            if (currentBattleground?.Profile != null)
                BlackspotManager.AddBlackspots(currentBattleground.Profile.Blackspots);
        }

        #endregion

        #region MoveTo — path randomization (HB Class80.MoveTo polygon mutation)

        /// <summary>
        /// Overrides MoveTo to inject polygon area randomization before path generation.
        /// 30% of walkable/swim polygons on the new path are tagged as area 27,
        /// causing Detour to prefer alternate routes — varies bot path each run.
        /// </summary>
        public override MoveResult MoveTo(WoWPoint destination)
        {
            // Only randomize once per destination (HB: bool_1 guard)
            if (!_pathRandomized
                && _lastRandomizedDest.DistanceSqr(destination) > PathPrecision * PathPrecision)
            {
                TryRandomizePath(destination);
            }

            return base.MoveTo(destination);
        }

        /// <summary>
        /// Runs a FindPath to obtain polygon references, then marks ~30% of walkable
        /// polygons as area 27 so the actual path generation picks a varied route.
        /// </summary>
        private void TryRandomizePath(WoWPoint destination)
        {
            if (!StyxNav.IsNavigatorLoaded)
                return;

            uint mapId = (uint)(StyxWoW.Me?.MapId ?? 0);
            if (mapId == 0)
                return;

            // Only randomize long paths (HB: path.Start.DistanceSqr(path.End) > 1600f)
            if (StyxWoW.Me.Location.DistanceSqr(destination) <= 1600f)
                return;

            var start = new Vector3(StyxWoW.Me.Location.X, StyxWoW.Me.Location.Y, StyxWoW.Me.Location.Z);
            var end = new Vector3(destination.X, destination.Y, destination.Z);

            PathFindResult? result = StyxNav.TripperNavigator.FindPath(mapId, start, end, true);
            if (result == null || !result.Succeeded || result.IsPartialPath || result.Polygons == null)
                return;

            for (int i = 0; i < result.Polygons.Length; i++)
            {
                PolygonReference polyRef = result.Polygons[i];

                // Restore any polygon we previously randomized (HB: dictionary_0 restore)
                if (_savedPolyAreas.TryGetValue(polyRef, out byte savedArea))
                {
                    StyxNav.TripperNavigator.SetPolyArea(mapId, polyRef, savedArea);
                    _savedPolyAreas.Remove(polyRef);
                }

                // 30% chance to randomize this polygon (HB: random_0.NextDouble() <= 0.3)
                if (_random.NextDouble() > 0.3)
                    continue;

                // Only randomize ground (1) or swim (4) area types (HB: b == 4 || b == 1)
                if (StyxNav.TripperNavigator.GetPolyArea(mapId, polyRef, out byte currentArea) != 0)
                    continue;
                if (currentArea != 1 && currentArea != 4)
                    continue;

                _savedPolyAreas[polyRef] = currentArea;
                StyxNav.TripperNavigator.SetPolyArea(mapId, polyRef, 27);
            }

            Logging.WriteNavigator("BG path randomized");
            _lastRandomizedDest = destination;
            _pathRandomized = true;
        }

        #endregion

        #region Clear (HB Class80.Clear)

        /// <summary>
        /// Resets path randomization state in addition to base path state.
        /// </summary>
        public override bool Clear()
        {
            _pathRandomized = false;
            _lastRandomizedDest = WoWPoint.Zero;
            return base.Clear();
        }

        #endregion
    }
}
