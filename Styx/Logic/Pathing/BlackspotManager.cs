using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Styx.Helpers;
using Styx.Logic.Profiles;
using Tripper.Navigation;

namespace Styx.Logic.Pathing
{
    /// <summary>
    /// Manages blackspots - areas to avoid during navigation.
    /// Blackspots can be added from profiles or dynamically at runtime.
    /// Like HB WoD, this marks navmesh polygons with AreaType.Blackspot (17)
    /// and sets a high path cost (60f) to make the pathfinder avoid them.
    /// </summary>
    public static class BlackspotManager
    {
        private static readonly List<Blackspot> _blackspots = new List<Blackspot>();
        private static readonly List<GlobalBlackspot> _globalBlackspots = new List<GlobalBlackspot>();
        private static readonly HashSet<Blackspot> _markedBlackspots = new HashSet<Blackspot>();
        private static readonly HashSet<Blackspot> _failedBlackspotMarks = new HashSet<Blackspot>();
        private static readonly Dictionary<Blackspot, List<PolyKey>> _blackspotPolygons = new Dictionary<Blackspot, List<PolyKey>>();
        private static readonly Dictionary<PolyKey, byte> _originalPolyAreas = new Dictionary<PolyKey, byte>();
        private static readonly Dictionary<PolyKey, ushort> _originalPolyFlags = new Dictionary<PolyKey, ushort>();
        private static readonly object _lock = new object();

        private readonly struct PolyKey : IEquatable<PolyKey>
        {
            public PolyKey(uint mapId, ulong polyRef)
            {
                MapId = mapId;
                PolyRef = polyRef;
            }

            public uint MapId { get; }
            public ulong PolyRef { get; }

            public bool Equals(PolyKey other)
            {
                return MapId == other.MapId && PolyRef == other.PolyRef;
            }

            public override bool Equals(object? obj)
            {
                return obj is PolyKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(MapId, PolyRef);
            }
        }
        
        /// <summary>
        /// Area type used for blackspots (Blackspot = 17, HB WoD).
        /// </summary>
        private const byte BlackspotAreaType = (byte)AreaType.Blackspot;
        
        /// <summary>
        /// High cost assigned to blackspot polygons (same as HB).
        /// </summary>
        private const float BlackspotAreaCost = 60f;

        /// <summary>
        /// Maximum polygons to query for a single blackspot.
        /// </summary>
        private const int MaxPolygonsPerBlackspot = 8192;
        
        /// <summary>
        /// Whether the blackspot area cost has been initialized.
        /// </summary>
        private static bool _areaCostInitialized = false;
        
        /// <summary>
        /// Last map ID where blackspots were marked.
        /// Used to re-mark all blackspots on map change.
        /// </summary>
        private static uint _lastMarkedMapId = 0;
        
        /// <summary>
        /// Static constructor - subscribes to profile and tile events.
        /// </summary>
        static BlackspotManager()
        {
            // Subscribe to profile changes to load blackspots from profile
            BotEvents.Profile.OnNewProfileLoaded += OnNewProfileLoaded;

            // Clear session blackspots on bot stop so stuck-handler marks don't persist across runs.
            BotEvents.OnBotStopped += OnBotStopped;

            // maintain tile subscription when navigation provider switches (HB 6.2.3)
            Navigator.OnNavigationProviderChanged += OnNavigationProviderChanged;

            // also subscribe immediately to Tripper navigator; this covers the common case
            try
            {
                var nav = Navigator.TripperNavigator; // create/get navigator
                if (nav != null)
                {
                    nav.TileLoaded += OnTileLoaded;
                }
            }
            catch (Exception)
            {
                // Ignore if navigator not available yet
            }

            // Load global blackspots on startup
            LoadGlobalBlackspots();

            // If a profile was already loaded before this static ctor ran, apply its blackspots now.
            try
            {
                var profile = ProfileManager.CurrentProfile;
                if (profile?.Blackspots != null && profile.Blackspots.Count > 0)
                {
                    AddBlackspots(profile.Blackspots);
                    EnsureBlackspotsMarked();
                }
            }
            catch (Exception)
            {
                // best-effort only
            }
        }
        
        /// <summary>
        /// Called when a new profile is loaded.
        /// Removes old profile blackspots and adds new ones.
        /// </summary>
        private static void OnNewProfileLoaded(BotEvents.Profile.NewProfileLoadedEventArgs args)
        {
            try
            {
                // Remove blackspots from old profile
                if (args.OldProfile?.Blackspots != null && args.OldProfile.Blackspots.Count > 0)
                {
                    RemoveBlackspots(args.OldProfile.Blackspots);
                    Logging.WriteDebug($"[Blackspot] Removed {args.OldProfile.Blackspots.Count} blackspots from old profile");
                }
            }
            catch (Exception) { }

            try
            {
                // Add blackspots from new profile
                if (args.NewProfile?.Blackspots != null && args.NewProfile.Blackspots.Count > 0)
                {
                    AddBlackspots(args.NewProfile.Blackspots);
                    Logging.Write($"[Blackspot] Loaded {args.NewProfile.Blackspots.Count} blackspots from profile");
                    EnsureBlackspotsMarked();
                }
            }
            catch (Exception ex)
            {
                Logging.WriteDebug($"[Blackspot] Error loading profile blackspots: {ex.Message}");
            }
        }

        // HB 6.2.3: rewire tile subscription when provider changes
        private static void OnBotStopped(EventArgs args)
        {
            // Clear session (non-global) blackspots. StuckHandler uses AddBlackspot (non-global),
            // so those should not persist into the next session. Global blackspots (from file) are
            // intentional and stay. Restores navmesh polygon areas back to originals.
            ClearBlackspots();
        }

        // HB 6.2.3: rewire tile subscription when provider changes
        private static void OnNavigationProviderChanged(object sender, NavigationProviderChangedEventArgs<NavigationProvider> e)
        {
            try
            {
                var nav = Navigator.TripperNavigator;
                if (nav != null)
                {
                    // remove first to avoid duplicate handlers
                    nav.TileLoaded -= OnTileLoaded;
                    nav.TileLoaded += OnTileLoaded;
                }
            }
            catch (Exception)
            {
                // ignore; tile subscription is best-effort
            }
        }

        /// <summary>
        /// Handler for navigator tile-loaded events. Re-applies profile and global blackspots
        /// when nav tiles stream in (HB-like behavior).
        /// </summary>
        private static void OnTileLoaded(object? sender, TileLoadedEventArgs e)
        {
            try
            {
                uint currentMap = StyxWoW.Me?.MapId ?? e.MapId;
                if (currentMap != 0 && currentMap != e.MapId)
                    return;

                lock (_lock)
                {
                    foreach (var spot in _blackspots)
                    {
                        if (ShouldMarkOnTileLoaded(spot, e.TileX, e.TileY))
                        {
                            _markedBlackspots.Remove(spot);
                            _failedBlackspotMarks.Remove(spot);
                            MarkBlackspotPolygons(spot, e.MapId, false);
                        }
                    }

                    foreach (var globalSpot in _globalBlackspots)
                    {
                        if (globalSpot.MapId == e.MapId && ShouldMarkOnTileLoaded(globalSpot.Blackspot, e.TileX, e.TileY))
                        {
                            _markedBlackspots.Remove(globalSpot.Blackspot);
                            _failedBlackspotMarks.Remove(globalSpot.Blackspot);
                            MarkBlackspotPolygons(globalSpot.Blackspot, e.MapId, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteDebug($"[Blackspot] OnTileLoaded error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all current blackspots (profile + runtime added).
        /// </summary>
        public static ReadOnlyCollection<Blackspot> Blackspots
        {
            get
            {
                lock (_lock)
                {
                    return _blackspots.AsReadOnly();
                }
            }
        }

        /// <summary>
        /// Gets all global blackspots (persisted across sessions).
        /// </summary>
        public static ReadOnlyCollection<GlobalBlackspot> GlobalBlackspots
        {
            get
            {
                lock (_lock)
                {
                    return _globalBlackspots.AsReadOnly();
                }
            }
        }

        /// <summary>
        /// Checks if a location is within any blackspot.
        /// </summary>
        /// <param name="location">The location to check.</param>
        /// <param name="radius">Additional radius to add to the check.</param>
        /// <returns>True if the location is blackspotted.</returns>
        public static bool IsBlackspotted(WoWPoint location, float radius = 0f)
        {
            lock (_lock)
            {
                // Check profile blackspots
                foreach (var spot in _blackspots)
                {
                    if (IsInBlackspot(location, spot, radius))
                        return true;
                }

                // Check global blackspots for current map
                uint currentMap = StyxWoW.Me?.MapId ?? 0;
                foreach (var globalSpot in _globalBlackspots)
                {
                    if (globalSpot.MapId == currentMap && IsInBlackspot(location, globalSpot.Blackspot, radius))
                        return true;
                }

                return false;
            }
        }

        private static bool IsInBlackspot(WoWPoint location, Blackspot spot, float extraRadius)
        {
            float totalRadius = spot.Radius + extraRadius;
            float dx = location.X - spot.Location.X;
            float dy = location.Y - spot.Location.Y;
            float dz = location.Z - spot.Location.Z;

            // Check horizontal distance
            if (dx * dx + dy * dy > totalRadius * totalRadius)
                return false;

            // Check vertical distance
            return Math.Abs(dz) <= spot.Height;
        }

        /// <summary>
        /// Adds a blackspot at the specified location.
        /// </summary>
        public static void AddBlackspot(WoWPoint location, float radius, float height)
        {
            AddBlackspots(new[] { new Blackspot(location, radius, height) });
        }

        public static void AddBlackspot(WoWPoint location, float radius, float height, string name)
        {
            AddBlackspot(location, radius, height);
        }

        /// <summary>
        /// Adds multiple blackspots and marks the corresponding navmesh polygons.
        /// Like HB WoD: QueryPolygons + SetPolyArea(Blackspot) + SetAreaCost(60f)
        /// </summary>
        public static void AddBlackspots(IEnumerable<Blackspot> blackspots)
        {
            if (blackspots == null)
                return;

            // Ensure area cost is set for blackspot polygons (only once)
            EnsureAreaCostInitialized();

            lock (_lock)
            {
                foreach (var spot in blackspots)
                {
                    if (!_blackspots.Contains(spot))
                    {
                        _blackspots.Add(spot);
                        
                        // Mark navmesh polygons for this blackspot
                        MarkBlackspotPolygons(spot);
                    }
                }
            }
        }
        
        /// <summary>
        /// Ensures the blackspot area cost is set (60f for AreaType.Blackspot).
        /// </summary>
        private static void EnsureAreaCostInitialized()
        {
            if (_areaCostInitialized)
                return;
                
            try
            {
                NativeMethods.SetAreaCost(0u, (int)BlackspotAreaType, BlackspotAreaCost);

                _areaCostInitialized = true;
                Logging.WriteDebug($"[Blackspot] Area cost initialized: area {BlackspotAreaType} = {BlackspotAreaCost}x");
            }
            catch (Exception ex)
            {
                Logging.WriteDebug($"[Blackspot] Failed to set area cost: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Ensures all blackspots are marked on the navmesh.
        /// Call this before pathfinding to ensure tiles are loaded and blackspots applied.
        /// This is the substitute for HB's OnTileLoaded callback.
        /// </summary>
        public static void EnsureBlackspotsMarked()
        {
            var meRef = StyxWoW.Me;
            if (meRef == null)
                return;
            uint currentMapId = meRef.MapId;
                
            // If map changed, clear marked blackspots to force re-marking
            if (_lastMarkedMapId != currentMapId)
            {
                lock (_lock)
                {
                    _markedBlackspots.Clear();
                    _failedBlackspotMarks.Clear();
                    _lastMarkedMapId = currentMapId;
                }
            }
            
            EnsureAreaCostInitialized();
            
            lock (_lock)
            {
                // Re-mark profile blackspots that haven't been successfully marked.
                // Retry failed blackspots on every pathfind: the tile that wasn't loaded at profile-load
                // time may now be loaded. HB 4.3.4 / 5.4.8 re-mark unconditionally (no _failedBlackspotMarks gate).
                foreach (var spot in _blackspots)
                {
                    if (!_markedBlackspots.Contains(spot))
                    {
                        MarkBlackspotPolygons(spot, currentMapId);
                    }
                }

                // Re-mark global blackspots for current map
                foreach (var globalSpot in _globalBlackspots)
                {
                    if (globalSpot.MapId == currentMapId && !_markedBlackspots.Contains(globalSpot.Blackspot))
                    {
                        MarkBlackspotPolygons(globalSpot.Blackspot, currentMapId);
                    }
                }
            }
        }
        
        /// <summary>
        /// Marks navmesh polygons within a blackspot zone with high-cost area type.
        /// This is the core of HB's blackspot system: SetPolyArea(polyRef, AreaType.Blackspot).
        /// </summary>
        private static void MarkBlackspotPolygons(Blackspot spot)
        {
            var me = StyxWoW.Me;
            if (me == null)
            {
                Logging.WriteDebug($"[Blackspot] Cannot mark polygons - no map loaded");
                return;
            }
            MarkBlackspotPolygons(spot, me.MapId);
        }
        
        /// <summary>
        /// Marks navmesh polygons within a blackspot zone with high-cost area type.
        /// </summary>
        private static void MarkBlackspotPolygons(Blackspot spot, uint mapId)
        {
            MarkBlackspotPolygons(spot, mapId, true);
        }

        /// <summary>
        /// Checks whether a blackspot can affect a loaded MaNGOS ADT navigation tile.
        /// </summary>
        private static bool ShouldMarkOnTileLoaded(Blackspot spot, int tileX, int tileY)
        {
            TileIdentifier min = TileIdentifier.GetByPosition(spot.Location.X - spot.Radius, spot.Location.Y - spot.Radius);
            TileIdentifier max = TileIdentifier.GetByPosition(spot.Location.X + spot.Radius, spot.Location.Y + spot.Radius);

            int minX = Math.Min(min.X, max.X);
            int maxX = Math.Max(min.X, max.X);
            int minY = Math.Min(min.Y, max.Y);
            int maxY = Math.Max(min.Y, max.Y);

            return tileX >= minX && tileX <= maxX && tileY >= minY && tileY <= maxY;
        }

        /// <summary>
        /// Marks navmesh polygons within a blackspot zone with high-cost area type.
        /// </summary>
        private static void MarkBlackspotPolygons(Blackspot spot, uint mapId, bool ensureTiles)
        {
            IntPtr polyRefsPtr = IntPtr.Zero;
            try
            {
                if (ensureTiles)
                {
                    // Ensure tiles are loaded at blackspot location
                    var centerXyz = new NativeMethods.XYZ(spot.Location.X, spot.Location.Y, spot.Location.Z);
                    NativeMethods.EnsureTiles(mapId, centerXyz, 1); // Load 3x3 tiles around blackspot
                }

                // Convert WoWPoint to navmesh coordinates
                var center = new NativeMethods.XYZ
                {
                    X = spot.Location.X,
                    Y = spot.Location.Y,
                    Z = spot.Location.Z
                };
                
                // Native QueryPolygons converts WoW extents to Detour extents as (Y, Z, X).
                // Pass WoW-space half extents: horizontal radius on X/Y, vertical height on Z.
                var extents = new NativeMethods.XYZ
                {
                    X = spot.Radius,
                    Y = spot.Radius,
                    Z = spot.Height
                };
                
                // Allocate unmanaged memory for polygon refs (ulong = 8 bytes)
                int bufferSize = MaxPolygonsPerBlackspot * sizeof(ulong);
                polyRefsPtr = Marshal.AllocHGlobal(bufferSize);
                
                // Query all polygons in the blackspot zone
                int polyCount = NativeMethods.QueryPolygons(mapId, center, extents, polyRefsPtr, MaxPolygonsPerBlackspot);
                
                if (polyCount <= 0)
                {
                    // Tiles might not be loaded yet; retry on tile-loaded/map-change, not every pathfind.
                    _failedBlackspotMarks.Add(spot);
                    Logging.Write($"Warning: QueryPolygons failed while applying blackspot {spot.Location} with radius {spot.Radius} - tile not loaded?");
                    return;
                }
                
                if (polyCount >= MaxPolygonsPerBlackspot)
                {
                    Logging.Write($"[Blackspot] Warning: Max polygon count ({MaxPolygonsPerBlackspot}) exceeded at {spot.Location}. " +
                        "Consider using multiple smaller blackspots.");
                }
                
                // Mark each polygon with the blackspot area type
                int markedCount = 0;
                if (!_blackspotPolygons.TryGetValue(spot, out List<PolyKey>? affectedPolys))
                {
                    affectedPolys = new List<PolyKey>(polyCount);
                    _blackspotPolygons[spot] = affectedPolys;
                }

                for (int i = 0; i < polyCount; i++)
                {
                    ulong polyRef = (ulong)Marshal.ReadInt64(polyRefsPtr, i * sizeof(ulong));
                    PolyKey key = new PolyKey(mapId, polyRef);

                    if (!_originalPolyAreas.ContainsKey(key))
                    {
                        uint areaStatus = NativeMethods.GetPolyArea(mapId, polyRef, out byte originalArea);
                        if ((areaStatus & 0x40000000) != 0) // DT_SUCCESS
                            _originalPolyAreas[key] = originalArea;
                    }

                    if (!_originalPolyFlags.ContainsKey(key))
                    {
                        uint flagStatus = NativeMethods.GetPolyFlags(mapId, polyRef, out ushort originalFlags);
                        if ((flagStatus & 0x40000000) != 0) // DT_SUCCESS
                            _originalPolyFlags[key] = originalFlags;
                    }

                    uint status = NativeMethods.SetPolyArea(mapId, polyRef, BlackspotAreaType);
                    if ((status & 0x40000000) != 0) // DT_SUCCESS
                    {
                        if (!affectedPolys.Contains(key))
                            affectedPolys.Add(key);
                        markedCount++;
                    }
                }
                
                // Track successfully marked blackspots so we don't re-mark them
                if (markedCount > 0)
                {
                    _markedBlackspots.Add(spot);
                    _failedBlackspotMarks.Remove(spot);
                    Logging.WriteDebug($"[Blackspot] Marked {markedCount}/{polyCount} polygons at {spot.Location} (radius {spot.Radius})");
                }
                else
                {
                    _failedBlackspotMarks.Add(spot);
                    Logging.WriteDebug($"[Blackspot] Failed to mark any polygons at {spot.Location} (found {polyCount} polys)");
                }
            }
            catch (Exception ex)
            {
                Logging.WriteDebug($"[Blackspot] Error marking polygons: {ex.Message}");
            }
            finally
            {
                if (polyRefsPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(polyRefsPtr);
                }
            }
        }

        /// <summary>
        /// Restores polygons affected by a blackspot to their original area type.
        /// HB's dynamic blackspot manager keeps the original area byte and writes it back on reset/remove.
        /// </summary>
        private static void RestoreBlackspotPolygons(Blackspot spot)
        {
            if (!_blackspotPolygons.TryGetValue(spot, out List<PolyKey>? affectedPolys))
            {
                _markedBlackspots.Remove(spot);
                _failedBlackspotMarks.Remove(spot);
                return;
            }

            _blackspotPolygons.Remove(spot);
            _markedBlackspots.Remove(spot);
            _failedBlackspotMarks.Remove(spot);

            int restoredCount = 0;
            foreach (var key in affectedPolys.Distinct())
            {
                if (IsPolyStillBlackspotted(key))
                    continue;

                if (_originalPolyAreas.TryGetValue(key, out byte originalArea))
                {
                    uint status = NativeMethods.SetPolyArea(key.MapId, key.PolyRef, originalArea);
                    if ((status & 0x40000000) != 0) // DT_SUCCESS
                    {
                        // Restore exact original flags — never mask live value which may have
                        // been corrupted if we set it incorrectly during marking.
                        ushort restoreFlags = _originalPolyFlags.TryGetValue(key, out ushort origFlags)
                            ? origFlags
                            : (ushort)0x0001; // ground walkable fallback
                        NativeMethods.SetPolyFlags(key.MapId, key.PolyRef, restoreFlags);
                        restoredCount++;
                    }

                    _originalPolyAreas.Remove(key);
                    _originalPolyFlags.Remove(key);
                }
            }

            if (restoredCount > 0)
            {
                Logging.WriteDebug($"[Blackspot] Restored {restoredCount} polygons at {spot.Location} (radius {spot.Radius})");
            }
        }

        private static bool IsPolyStillBlackspotted(PolyKey key)
        {
            foreach (var affectedPolys in _blackspotPolygons.Values)
            {
                if (affectedPolys.Contains(key))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a blackspot.
        /// </summary>
        public static void RemoveBlackspot(Blackspot spot)
        {
            lock (_lock)
            {
                _blackspots.Remove(spot);
                RestoreBlackspotPolygons(spot);
            }
        }

        /// <summary>
        /// Removes multiple blackspots.
        /// </summary>
        public static void RemoveBlackspots(IEnumerable<Blackspot> spots)
        {
            if (spots == null)
                return;

            lock (_lock)
            {
                var spotList = spots.ToList();
                foreach (var spot in spotList)
                {
                    RestoreBlackspotPolygons(spot);
                }

                _blackspots.RemoveAll(s => spotList.Contains(s));
            }
        }

        /// <summary>
        /// Clears all non-global blackspots.
        /// </summary>
        public static void ClearBlackspots()
        {
            lock (_lock)
            {
                foreach (var spot in _blackspots.ToList())
                {
                    RestoreBlackspotPolygons(spot);
                }

                _blackspots.Clear();
            }
        }

        /// <summary>
        /// Adds a global blackspot that persists across sessions.
        /// </summary>
        public static void AddGlobalBlackspot(WoWPoint location, float radius, float height)
        {
            uint mapId = StyxWoW.Me?.MapId ?? 0;
            AddGlobalBlackspot(new GlobalBlackspot(location, radius, height, mapId));
        }

        /// <summary>
        /// Adds a global blackspot.
        /// </summary>
        public static void AddGlobalBlackspot(GlobalBlackspot blackspot)
        {
            if (blackspot == null)
                return;

            // Ensure area cost is set
            EnsureAreaCostInitialized();

            lock (_lock)
            {
                if (_globalBlackspots.Contains(blackspot))
                    return;

                if (IsBlackspotted(blackspot.Blackspot.Location, blackspot.Blackspot.Radius))
                    return;

                _globalBlackspots.Add(blackspot);

                // Mark polygon if on current map
                uint currentMap = StyxWoW.Me?.MapId ?? 0;
                if (blackspot.MapId == currentMap)
                {
                    MarkBlackspotPolygons(blackspot.Blackspot);
                }
                
                SaveGlobalBlackspots();
                Logging.Write($"[Blackspot] Added global blackspot at {blackspot.Blackspot.Location} (radius {blackspot.Blackspot.Radius})");
            }
        }

        /// <summary>
        /// Removes a global blackspot.
        /// </summary>
        public static void RemoveGlobalBlackspot(GlobalBlackspot blackspot)
        {
            lock (_lock)
            {
                _globalBlackspots.Remove(blackspot);
                RestoreBlackspotPolygons(blackspot.Blackspot);
                SaveGlobalBlackspots();
            }
        }

        /// <summary>
        /// Loads global blackspots from file.
        /// </summary>
        public static void LoadGlobalBlackspots()
        {
            string path = Path.Combine(Logging.ApplicationPath, "GlobalStuckBlackspots.xml");
            if (!File.Exists(path))
                return;

            try
            {
                var doc = XDocument.Load(path);
                var root = doc.Element("GlobalBlackspots");
                if (root == null)
                    return;

                var loaded = GlobalBlackspot.GetBlackspotsFromXml(root);
                lock (_lock)
                {
                    foreach (var spot in _globalBlackspots)
                    {
                        RestoreBlackspotPolygons(spot.Blackspot);
                    }

                    _globalBlackspots.Clear();
                    _globalBlackspots.AddRange(loaded);
                }

                Logging.Write($"Loaded {loaded.Count} global blackspots");
            }
            catch (Exception ex)
            {
                Logging.Write($"Error loading global blackspots: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves global blackspots to file.
        /// </summary>
        private static void SaveGlobalBlackspots()
        {
            try
            {
                var root = new XElement("GlobalBlackspots");
                foreach (var spot in _globalBlackspots)
                {
                    root.Add(spot.GetXml());
                }

                string path = Path.Combine(Logging.ApplicationPath, "GlobalStuckBlackspots.xml");
                root.Save(path);
            }
            catch (Exception ex)
            {
                Logging.Write($"Error saving global blackspots: {ex.Message}");
            }
        }

        /// <summary>
        /// Represents a global blackspot that is saved across sessions.
        /// </summary>
        public class GlobalBlackspot : IEquatable<GlobalBlackspot>
        {
            public Blackspot Blackspot { get; set; }
            public uint MapId { get; set; }

            public GlobalBlackspot(WoWPoint location, float radius, float height, uint mapId)
            {
                Blackspot = new Blackspot(location, radius, height);
                MapId = mapId;
            }

            public XElement GetXml()
            {
                return new XElement("GlobalBlackspot",
                    new XAttribute("X", Blackspot.Location.X),
                    new XAttribute("Y", Blackspot.Location.Y),
                    new XAttribute("Z", Blackspot.Location.Z),
                    new XAttribute("Radius", Blackspot.Radius),
                    new XAttribute("Height", Blackspot.Height),
                    new XAttribute("MapId", MapId));
            }

            public static List<GlobalBlackspot> GetBlackspotsFromXml(XElement xml)
            {
                var result = new List<GlobalBlackspot>();

                foreach (var element in xml.Elements("GlobalBlackspot"))
                {
                    try
                    {
                        float x = Convert.ToSingle(element.Attribute("X")?.Value, CultureInfo.InvariantCulture);
                        float y = Convert.ToSingle(element.Attribute("Y")?.Value, CultureInfo.InvariantCulture);
                        float z = Convert.ToSingle(element.Attribute("Z")?.Value, CultureInfo.InvariantCulture);
                        float radius = Convert.ToSingle(element.Attribute("Radius")?.Value, CultureInfo.InvariantCulture);
                        float height = Convert.ToSingle(element.Attribute("Height")?.Value, CultureInfo.InvariantCulture);
                        uint mapId = Convert.ToUInt32(element.Attribute("MapId")?.Value, CultureInfo.InvariantCulture);

                        result.Add(new GlobalBlackspot(new WoWPoint(x, y, z), radius, height, mapId));
                    }
                    catch
                    {
                        // Skip invalid entries
                    }
                }

                return result;
            }

            public bool Equals(GlobalBlackspot? other)
            {
                if (other is null)
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return Blackspot.Equals(other.Blackspot) && MapId == other.MapId;
            }

            public override bool Equals(object? obj)
            {
                return obj is GlobalBlackspot other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Blackspot, MapId);
            }

            public static bool operator ==(GlobalBlackspot? left, GlobalBlackspot? right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(GlobalBlackspot? left, GlobalBlackspot? right)
            {
                return !Equals(left, right);
            }
        }
    }
}
