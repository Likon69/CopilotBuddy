using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Tripper.Navigation
{
    /// <summary>
    /// Main navigation system for WoW pathfinding using Detour navmesh.
    /// Provides mesh loading, path calculation, and navigation queries.
    /// Adapted from Honorbuddy 5.4.8 WowNavigator for Trinity navmesh format.
    /// </summary>
    public sealed class Navigator : IDisposable
    {
        #region Fields

        private readonly object _meshLock = new object();
        private readonly Dictionary<string, QueryFilter> _queryFilters = new Dictionary<string, QueryFilter>();
        private QueryFilter _currentQueryFilter = null!;
        private bool _isDisposed;
        private DateTime _lastGarbageCollect = DateTime.UtcNow;

        // HB 6.2.3 pattern: prevent GC of native callback delegate
        private NativeMethods.TileLoadedCallbackDelegate? _nativeTileLoadedCallback;

        #endregion

        #region Events

        /// <summary>
        /// Raised when a tile is loaded into the navmesh.
        /// </summary>
        public event EventHandler<TileLoadedEventArgs>? TileLoaded;

        /// <summary>
        /// Raised when a map is fully loaded.
        /// </summary>
        public event EventHandler<MapLoadedEventArgs>? MapLoaded;

        /// <summary>
        /// Raised during pathfinding progress.
        /// </summary>
        public event EventHandler<PathProgressEventArgs>? PathProgress;

        /// <summary>
        /// Raised when a navigation log message is generated.
        /// </summary>
        public event Action<string>? LogMessage;

        #endregion

        #region Properties

        /// <summary>
        /// Lock object for thread-safe mesh operations.
        /// </summary>
        public object MeshLock => _meshLock;

        /// <summary>
        /// Time interval for garbage collection of unused tiles.
        /// </summary>
        public TimeSpan GarbageCollectTime { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Search extents for navmesh queries (X, Y, Z).
        /// Default: 3, 20, 3 yards.
        /// </summary>
        public Vector3 Extents { get; set; } = new Vector3(3f, 20f, 3f);

        /// <summary>
        /// Current query filter used for pathfinding operations.
        /// </summary>
        public QueryFilter QueryFilter
        {
            get => _currentQueryFilter;
            set => _currentQueryFilter = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Indicates if the navigation system is initialized and ready.
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Current map ID that the navigator is operating on.
        /// </summary>
        public uint CurrentMapId { get; private set; }

        /// <summary>
        /// Path post-processing mode. Default is MoveAwayFromEdges to avoid stairs/wall issues.
        /// Matches Honorbuddy's PathPostProcessing behavior.
        /// </summary>
        public PathPostProcessing PathPostProcessing { get; set; } = PathPostProcessing.MoveAwayFromEdges;

        /// <summary>
        /// Edge distance threshold for MoveAwayFromEdges post-processing.
        /// Points closer than this to a wall will be moved away.
        /// Default: 2.0 yards (like HB).
        /// </summary>
        public float EdgeDistance { get; set; } = 2.0f;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the Navigator class.
        /// </summary>
        public Navigator()
        {
            InitializeQueryFilters();
            ResetQueryFilter();
        }

        /// <summary>
        /// Initializes default query filters for different movement scenarios.
        /// </summary>
        private void InitializeQueryFilters()
        {
            // HB 6.2.3 GetNewDefaultQueryFilter:
            // Include = All, Exclude = Unwalkable | Transport
            _queryFilters["Default"] = new QueryFilter
            {
                IncludeFlags = AbilityFlags.All,
                ExcludeFlags = AbilityFlags.Unwalkable | AbilityFlags.Transport,
                AreaCosts = new Dictionary<AreaType, float>
                {
                    { AreaType.Ground, 1.66f },
                    { AreaType.Water, 3.33f },
                    { AreaType.Road, 1.0f }
                }
            };

            // Swimming filter - prioritize water movement
            _queryFilters["Swimming"] = new QueryFilter
            {
                IncludeFlags = AbilityFlags.Swim | AbilityFlags.Run,
                ExcludeFlags = AbilityFlags.Unwalkable,
                AreaCosts = new Dictionary<AreaType, float>
                {
                    { AreaType.Water, 1.0f },
                    { AreaType.Ground, 1.5f }
                }
            };

            // Flying filter - allow all movement types
            _queryFilters["Flying"] = new QueryFilter
            {
                IncludeFlags = AbilityFlags.Run | AbilityFlags.Swim | AbilityFlags.Jump | AbilityFlags.Transport,
                ExcludeFlags = AbilityFlags.None,
                AreaCosts = new Dictionary<AreaType, float>
                {
                    { AreaType.Ground, 1.0f },
                    { AreaType.Water, 1.0f },
                    { AreaType.Elevator, 1.0f }
                }
            };

            // Transport filter - include elevators, boats, etc.
            _queryFilters["Transport"] = new QueryFilter
            {
                IncludeFlags = AbilityFlags.Run | AbilityFlags.Swim | AbilityFlags.Transport,
                ExcludeFlags = AbilityFlags.Unwalkable,
                AreaCosts = new Dictionary<AreaType, float>
                {
                    { AreaType.Ground, 1.0f },
                    { AreaType.Elevator, 0.5f },
                    { AreaType.Portal, 0.5f }
                }
            };
        }

        /// <summary>
        /// Resets query filter to default settings.
        /// </summary>
        public void ResetQueryFilter()
        {
            _currentQueryFilter = _queryFilters["Default"].Clone();
            ApplyCurrentQueryFilterToNative();
        }

        /// <summary>
        /// Applies the current managed query filter to the native Navigation.dll filter.
        /// This keeps include/exclude flags and area costs in sync with HB behavior.
        /// </summary>
        private void ApplyCurrentQueryFilterToNative()
        {
            try
            {
                NativeMethods.SetIncludeFlags((ushort)_currentQueryFilter.IncludeFlags);
                NativeMethods.SetExcludeFlags((ushort)_currentQueryFilter.ExcludeFlags);

                // Start from HB default costs, then apply filter-specific overrides.
                SetDefaultAreaCosts();
                foreach (var kvp in _currentQueryFilter.AreaCosts)
                {
                    NativeMethods.SetAreaCost((uint)kvp.Key, kvp.Value);
                }
            }
            catch (Exception ex)
            {
                Log($"Failed to apply query filter: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads navigation meshes from the mmaps directory.
        /// </summary>
        /// <returns>True if meshes were loaded successfully.</returns>
        public bool LoadMeshes()
        {
            lock (_meshLock)
            {
                try
                {
                    // Navigation.dll initializes automatically in DllMain
                    // Tiles are loaded on-demand when CalculatePath is called
                    
                    // HB 6.2.3 pattern: register tile loaded callback (mirrors WorldMeshManager.method_3)
                    _nativeTileLoadedCallback = OnNativeTileLoaded;
                    NativeMethods.SetTileLoadedCallback(_nativeTileLoadedCallback);
                    
                    // Sync managed filter to native filter once mesh layer is ready.
                    ApplyCurrentQueryFilterToNative();
                    
                    IsLoaded = true;
                    Log("Navigation system ready (tiles load on-demand)");
                    return true;
                }
                catch (Exception ex)
                {
                    Log($"Exception initializing navigation: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Sets default area costs in the native DLL filter.
        /// Matches HB 6.2.3 WowNavigator.SetDefaultQueryFilterCosts pattern.
        /// Road=1.0 (preferred), Ground=1.66, Water=3.33, etc.
        /// </summary>
        private void SetDefaultAreaCosts()
        {
            try
            {
                // HB 6.2.3 SetDefaultQueryFilterCosts — exact values from decompile
                // Lower cost = preferred path. Road=1.0 is natural preference without detour-causing suroptimisation.
                NativeMethods.SetAreaCost((uint)AreaType.Ground, 1.66f);
                NativeMethods.SetAreaCost((uint)AreaType.Road, 1.0f);             // HB 6.2.3: 1.0 (was 0.5 — caused road detours)
                NativeMethods.SetAreaCost((uint)AreaType.Water, 3.33f);
                NativeMethods.SetAreaCost((uint)AreaType.Lava, 55.0f);
                NativeMethods.SetAreaCost((uint)AreaType.Fall, 1.7f);
                NativeMethods.SetAreaCost((uint)AreaType.Gate, 1.66f);
                NativeMethods.SetAreaCost((uint)AreaType.Elevator, 3.16f);
                NativeMethods.SetAreaCost((uint)AreaType.Portal, 1.66f);
                NativeMethods.SetAreaCost((uint)AreaType.DefendersPortal, 3.16f);
                NativeMethods.SetAreaCost((uint)AreaType.HordePortal, 1.66f);
                NativeMethods.SetAreaCost((uint)AreaType.AlliancePortal, 1.66f);
                NativeMethods.SetAreaCost((uint)AreaType.Blocked, 100.0f);
                NativeMethods.SetAreaCost((uint)AreaType.InteractUnit, 1.66f);
                NativeMethods.SetAreaCost((uint)AreaType.InteractObject, 1.66f);
                NativeMethods.SetAreaCost((uint)AreaType.Blackspot, 60.0f);       // HB 6.2.3: blackspot penalty
                NativeMethods.SetAreaCost((uint)AreaType.Horde, 1.66f);           // HB 6.2.3: faction area
                NativeMethods.SetAreaCost((uint)AreaType.Alliance, 1.66f);        // HB 6.2.3: faction area
                
                Log("Default area costs set (Road=1.0, Ground=1.66)");
            }
            catch (Exception ex)
            {
                Log($"Failed to set area costs: {ex.Message}");
            }
        }

        /// <summary>
        /// Unloads all navigation meshes and releases resources.
        /// </summary>
        public void UnloadMeshes()
        {
            lock (_meshLock)
            {
                try
                {
                    // Navigation.dll handles cleanup in DllMain DLL_PROCESS_DETACH
                    IsLoaded = false;
                    Log("Navigation system marked as unloaded");
                }
                catch (Exception ex)
                {
                    Log($"Exception during navigation cleanup: {ex.Message}");
                }
            }
        }

        #endregion

        #region Pathfinding

        /// <summary>
        /// Calculates a path from start to end position.
        /// Uses CalculatePathEx for complete path information including flags and area types.
        /// </summary>
        /// <param name="mapId">Map ID to pathfind on.</param>
        /// <param name="start">Starting position.</param>
        /// <param name="end">Destination position.</param>
        /// <param name="straightPath">If true, returns straight path; otherwise returns polygon corridor.</param>
        /// <returns>PathFindResult containing the calculated path.</returns>
        public PathFindResult FindPath(uint mapId, Vector3 start, Vector3 end, bool straightPath = true)
        {
            if (!IsLoaded)
            {
                Log("Cannot pathfind - meshes not loaded");
                return PathFindResult.CreateFailed(PathFindStep.None);
            }

            lock (_meshLock)
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    CurrentMapId = mapId;

                    var startC = new NativeMethods.XYZ(start);
                    var endC = new NativeMethods.XYZ(end);

                    // Use CalculatePathEx for complete path data
                    IntPtr resultPtr = NativeMethods.CalculatePathEx(mapId, startC, endC, straightPath);

                    if (resultPtr == IntPtr.Zero)
                    {
                        return PathFindResult.CreateFailed(PathFindStep.InitPathFind);
                    }

                    try
                    {
                        // Marshal the PathResult structure
                        var nativeResult = Marshal.PtrToStructure<NativeMethods.PathResult>(resultPtr);
                        
                        stopwatch.Stop();

                        // Check status
                        var status = new Status(nativeResult.Status);
                        if (status.Failed || nativeResult.Length == 0)
                        {
                            return new PathFindResult
                            {
                                Elapsed = stopwatch.Elapsed,
                                Status = status,
                                FailStep = (PathFindStep)nativeResult.FailStep,
                                Start = start,
                                End = end
                            };
                        }

                        int pathLength = nativeResult.Length;

                        // Marshal arrays from native memory
                        Vector3[] points = new Vector3[pathLength];
                        StraightPathFlags[] flags = new StraightPathFlags[pathLength];
                        AreaType[] polyTypes = new AreaType[pathLength];
                        AbilityFlags[] abilityFlags = new AbilityFlags[pathLength];
                        PolygonReference[] polygons = new PolygonReference[pathLength];

                        unsafe
                        {
                            // Points
                            NativeMethods.XYZ* pointsPtr = (NativeMethods.XYZ*)nativeResult.Points.ToPointer();
                            for (int i = 0; i < pathLength; i++)
                            {
                                points[i] = pointsPtr[i].ToVector3();
                            }

                            // StraightPathFlags
                            if (nativeResult.StraightPathFlags != IntPtr.Zero)
                            {
                                byte* flagsPtr = (byte*)nativeResult.StraightPathFlags.ToPointer();
                                for (int i = 0; i < pathLength; i++)
                                {
                                    flags[i] = (StraightPathFlags)flagsPtr[i];
                                }
                            }

                            // PolyTypes (AreaType)
                            if (nativeResult.PolyTypes != IntPtr.Zero)
                            {
                                byte* polyTypesPtr = (byte*)nativeResult.PolyTypes.ToPointer();
                                for (int i = 0; i < pathLength; i++)
                                {
                                    polyTypes[i] = (AreaType)polyTypesPtr[i];
                                }
                            }

                            // AbilityFlags
                            if (nativeResult.AbilityFlags != IntPtr.Zero)
                            {
                                byte* abilityPtr = (byte*)nativeResult.AbilityFlags.ToPointer();
                                for (int i = 0; i < pathLength; i++)
                                {
                                    abilityFlags[i] = (AbilityFlags)abilityPtr[i];
                                }
                            }

                            // PolyRefs (polygon references for each point)
                            if (nativeResult.PolyRefs != IntPtr.Zero)
                            {
                                ulong* polyRefsPtr = (ulong*)nativeResult.PolyRefs.ToPointer();
                                for (int i = 0; i < pathLength; i++)
                                {
                                    polygons[i] = new PolygonReference(polyRefsPtr[i]);
                                }
                            }
                        }

                        // Apply path post-processing (like HB's MoveAwayFromEdges)
                        if (PathPostProcessing != PathPostProcessing.None && points.Length > 2)
                        {
                            try
                            {
                                if (PathPostProcessing == PathPostProcessing.MoveAwayFromEdges)
                                {
                                    PathPostProcessor.MoveAwayFromEdges(mapId, ref points, ref flags, ref polygons, EdgeDistance);
                                }
                                else if (PathPostProcessing == PathPostProcessing.Randomize)
                                {
                                    PathPostProcessor.Randomize(mapId, ref points, ref flags);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log($"Path post-processing failed: {ex.Message}");
                                // Continue with unprocessed path
                            }
                        }

                        // Water/Lava Z+2f fix (HB 6.2.3 step 15 in Class1458.method_0)
                        // When path points are in Water or Lava area, elevate Z by 2f to prevent
                        // the character from swimming under the surface.
                        if (polyTypes != null)
                        {
                            for (int i = 1; i < points.Length - 1; i++)
                            {
                                if (i < polyTypes.Length &&
                                    (polyTypes[i] == AreaType.Water || polyTypes[i] == AreaType.Lava))
                                {
                                    points[i] = new System.Numerics.Vector3(points[i].X, points[i].Y, points[i].Z + 2.0f);
                                }
                            }
                        }

                        // Create full result
                        var result = new PathFindResult
                        {
                            Elapsed = stopwatch.Elapsed,
                            Status = status,
                            Points = points,
                            Flags = flags,
                            Polygons = polygons,
                            AbilityFlags = abilityFlags,
                            PolyTypes = polyTypes,
                            StartPoly = PolygonReference.Invalid,
                            EndPoly = PolygonReference.Invalid,
                            Start = points.Length > 0 ? points[0] : start,
                            End = points.Length > 0 ? points[^1] : end,
                            Aborted = false,
                            IsPartialPath = status.IsPartialResult,
                            FailStep = PathFindStep.None
                        };

                        RaisePathProgress(result);
                        return result;
                    }
                    finally
                    {
                        NativeMethods.FreePathResult(resultPtr);
                    }
                }
                catch (Exception ex)
                {
                    Log($"Pathfinding exception: {ex.Message}");
                    return PathFindResult.CreateFailed(PathFindStep.None);
                }
            }
        }

        /// <summary>
        /// Finds the nearest valid navmesh point to a given position.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="position">Position to search from.</param>
        /// <param name="nearestPoint">Output nearest valid point.</param>
        /// <returns>True if a valid point was found.</returns>
        public bool FindNearestPoint(uint mapId, Vector3 position, out Vector3 nearestPoint)
        {
            nearestPoint = Vector3.Zero;

            if (!IsLoaded)
                return false;

            lock (_meshLock)
            {
                try
                {
                    var posC = new NativeMethods.XYZ(position);
                    NativeMethods.XYZ nearestC;

                    bool success = NativeMethods.FindNearestPoly(mapId, posC, 3.0f, out nearestC);
                    if (success)
                    {
                        nearestPoint = nearestC.ToVector3();
                    }
                    return success;
                }
                catch (Exception ex)
                {
                    Log($"FindNearestPoint exception: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Finds a random navigable point within radius of center position.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="center">Center position.</param>
        /// <param name="radius">Search radius in yards.</param>
        /// <param name="randomPoint">Output random valid point.</param>
        /// <returns>True if a random point was found.</returns>
        public bool FindRandomPoint(uint mapId, Vector3 center, float radius, out Vector3 randomPoint)
        {
            randomPoint = Vector3.Zero;

            if (!IsLoaded)
                return false;

            lock (_meshLock)
            {
                try
                {
                    var centerC = new NativeMethods.XYZ(center);
                    NativeMethods.XYZ randomC;

                    bool success = NativeMethods.FindRandomPointAroundCircle(mapId, centerC, radius, out randomC);
                    if (success)
                    {
                        randomPoint = randomC.ToVector3();
                    }
                    return success;
                }
                catch (Exception ex)
                {
                    Log($"FindRandomPoint exception: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Performs a raycast from start to end position.
        /// Returns detailed raycast information including hit position and visited polygons.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="start">Ray start position.</param>
        /// <param name="end">Ray end position.</param>
        /// <param name="hitT">Output normalized hit distance (0-1, 1.0 = no hit).</param>
        /// <param name="hitNormal">Output hit normal vector.</param>
        /// <returns>Detour status code.</returns>
        public Status Raycast(uint mapId, Vector3 start, Vector3 end, out float hitT, out Vector3 hitNormal)
        {
            hitT = 1.0f;
            hitNormal = Vector3.Zero;

            if (!IsLoaded)
                return Status.Failure;

            lock (_meshLock)
            {
                try
                {
                    var startC = new NativeMethods.XYZ(start);
                    var endC = new NativeMethods.XYZ(end);
                    var extents = new NativeMethods.XYZ(Extents);

                    // First find the start polygon
                    if (!NativeMethods.FindNearestPolyRef(mapId, startC, extents, out ulong startRef, out _))
                        return Status.Failure;

                    // Perform raycast
                    ulong[] path = new ulong[256];
                    NativeMethods.XYZ hitNormalC;
                    uint status = NativeMethods.Raycast(mapId, startRef, startC, endC, 
                        out hitT, out hitNormalC, path, out int pathCount, 256);

                    hitNormal = hitNormalC.ToVector3();
                    return new Status(status);
                }
                catch (Exception ex)
                {
                    Log($"Raycast exception: {ex.Message}");
                    return Status.Failure;
                }
            }
        }

        /// <summary>
        /// GAP 3: Raycast with custom extents for FindNearestPoly.
        /// HB 6.2.3 method_26 uses tight extents (0.5, 3, 0.5) for push-ahead
        /// to avoid snapping to the wrong nav-mesh layer in multi-floor areas.
        /// </summary>
        public Status RaycastWithExtents(uint mapId, Vector3 start, Vector3 end, Vector3 customExtents,
            out float hitT, out Vector3 hitNormal, out ulong[] visitedPolys, out int visitedCount)
        {
            hitT = 1.0f;
            hitNormal = Vector3.Zero;
            visitedPolys = new ulong[64];
            visitedCount = 0;

            if (!IsLoaded)
                return Status.Failure;

            lock (_meshLock)
            {
                try
                {
                    var startC = new NativeMethods.XYZ(start);
                    var endC = new NativeMethods.XYZ(end);
                    var extentsC = new NativeMethods.XYZ(customExtents);

                    // FindNearestPoly with custom (tight) extents — HB 6.2.3 method_26
                    if (!NativeMethods.FindNearestPolyRef(mapId, startC, extentsC, out ulong startRef, out _))
                        return Status.Failure;

                    // Perform raycast from resolved poly
                    NativeMethods.XYZ hitNormalC;
                    uint status = NativeMethods.Raycast(mapId, startRef, startC, endC,
                        out hitT, out hitNormalC, visitedPolys, out visitedCount, 64);

                    hitNormal = hitNormalC.ToVector3();
                    return new Status(status);
                }
                catch (Exception ex)
                {
                    Log($"RaycastWithExtents exception: {ex.Message}");
                    return Status.Failure;
                }
            }
        }

        /// <summary>
        /// GAP 7: Raycast with area type validation — matches HB 6.2.3 method_13.
        /// After raycast, iterates visited polygons and checks that ALL have allowed area types
        /// (Ground, Water, Road, and the player's faction area). Returns true if the path is
        /// BLOCKED (hit boundary or bad area type).
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="start">Ray start position.</param>
        /// <param name="end">Ray end position.</param>
        /// <param name="hitT">Output normalized hit distance.</param>
        /// <param name="factionArea">Player faction area type (Horde or Alliance) — HB 6.2.3 areaType_0.</param>
        /// <returns>True if blocked (hit boundary OR traverses disallowed area type), false if clear.</returns>
        public bool RaycastBlocked(uint mapId, Vector3 start, Vector3 end, out float hitT, AreaType factionArea = AreaType.Ground)
        {
            hitT = 1.0f;

            if (!IsLoaded)
                return true; // assume blocked if not loaded

            lock (_meshLock)
            {
                try
                {
                    var startC = new NativeMethods.XYZ(start);
                    var endC = new NativeMethods.XYZ(end);
                    var extentsC = new NativeMethods.XYZ(Extents);

                    if (!NativeMethods.FindNearestPolyRef(mapId, startC, extentsC, out ulong startRef, out _))
                        return true;

                    ulong[] path = new ulong[512];
                    uint status = NativeMethods.Raycast(mapId, startRef, startC, endC,
                        out hitT, out _, path, out int pathCount, 512);

                    if (new Status(status).Failed)
                        return true;

                    // HB 6.2.3 method_13: iterate visited polys and break on non-Ground/Water/Road/faction
                    for (int i = 0; i < pathCount; i++)
                    {
                        if (path[i] == 0) break;
                        uint areaStatus = NativeMethods.GetPolyArea(mapId, path[i], out byte area);
                        if (new Status(areaStatus).Failed)
                            return true; // can't resolve area → assume blocked

                        var areaType = (AreaType)area;
                        if (areaType != AreaType.Ground && areaType != AreaType.Water && areaType != AreaType.Road && areaType != factionArea)
                        {
                            return true; // disallowed area type in ray path
                        }
                    }

                    return hitT < 1.0f; // true = hit navmesh boundary
                }
                catch (Exception ex)
                {
                    Log($"RaycastBlocked exception: {ex.Message}");
                    return true;
                }
            }
        }

        /// <summary>
        /// Checks if there's line of sight between two points on the navmesh.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="start">Start position.</param>
        /// <param name="end">End position.</param>
        /// <returns>True if there's clear line of sight.</returns>
        public bool HasLineOfSight(uint mapId, Vector3 start, Vector3 end)
        {
            if (!IsLoaded)
                return false;

            lock (_meshLock)
            {
                try
                {
                    var startC = new NativeMethods.XYZ(start);
                    var endC = new NativeMethods.XYZ(end);
                    return NativeMethods.HasLineOfSight(mapId, startC, endC);
                }
                catch (Exception ex)
                {
                    Log($"HasLineOfSight exception: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Finds distance to the nearest wall/boundary from a position.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="position">Position to check.</param>
        /// <param name="maxRadius">Maximum search radius.</param>
        /// <param name="hitPoint">Output hit point on wall.</param>
        /// <returns>Distance to wall, or -1 if failed.</returns>
        public float FindDistanceToWall(uint mapId, Vector3 position, float maxRadius, out Vector3 hitPoint)
        {
            hitPoint = Vector3.Zero;

            if (!IsLoaded)
                return -1f;

            lock (_meshLock)
            {
                try
                {
                    var posC = new NativeMethods.XYZ(position);
                    NativeMethods.XYZ hitC;
                    float distance = NativeMethods.FindDistanceToWall(mapId, posC, maxRadius, out hitC);
                    hitPoint = hitC.ToVector3();
                    return distance;
                }
                catch (Exception ex)
                {
                    Log($"FindDistanceToWall exception: {ex.Message}");
                    return -1f;
                }
            }
        }

        /// <summary>
        /// Checks if a point is on the navmesh.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="point">Point to check.</param>
        /// <param name="tolerance">Search tolerance.</param>
        /// <returns>True if the point is on the navmesh.</returns>
        public bool IsPointOnNavMesh(uint mapId, Vector3 point, float tolerance = 3.0f)
        {
            if (!IsLoaded)
                return false;

            lock (_meshLock)
            {
                try
                {
                    var pointC = new NativeMethods.XYZ(point);
                    return NativeMethods.IsPointOnNavMesh(mapId, pointC, tolerance);
                }
                catch (Exception ex)
                {
                    Log($"IsPointOnNavMesh exception: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the height at a position on a specific polygon.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="polyRef">Polygon reference.</param>
        /// <param name="position">Position to check.</param>
        /// <param name="height">Output height.</param>
        /// <returns>True if height was found.</returns>
        public bool GetPolyHeight(uint mapId, PolygonReference polyRef, Vector3 position, out float height)
        {
            height = 0f;

            if (!IsLoaded)
                return false;

            lock (_meshLock)
            {
                try
                {
                    var posC = new NativeMethods.XYZ(position);
                    return NativeMethods.GetPolyHeight(mapId, polyRef.Id, posC, out height);
                }
                catch (Exception ex)
                {
                    Log($"GetPolyHeight exception: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Finds the closest point on a polygon.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="polyRef">Polygon reference.</param>
        /// <param name="position">Position to check.</param>
        /// <param name="closestPoint">Output closest point.</param>
        /// <returns>True if closest point was found.</returns>
        public bool ClosestPointOnPoly(uint mapId, PolygonReference polyRef, Vector3 position, out Vector3 closestPoint)
        {
            closestPoint = Vector3.Zero;

            if (!IsLoaded)
                return false;

            lock (_meshLock)
            {
                try
                {
                    var posC = new NativeMethods.XYZ(position);
                    NativeMethods.XYZ closestC;
                    bool success = NativeMethods.ClosestPointOnPoly(mapId, polyRef.Id, posC, out closestC);
                    if (success)
                        closestPoint = closestC.ToVector3();
                    return success;
                }
                catch (Exception ex)
                {
                    Log($"ClosestPointOnPoly exception: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Finds the closest point on polygon boundary.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="polyRef">Polygon reference.</param>
        /// <param name="position">Position to check.</param>
        /// <param name="closestPoint">Output closest point on boundary.</param>
        /// <returns>True if closest point was found.</returns>
        public bool ClosestPointOnPolyBoundary(uint mapId, PolygonReference polyRef, Vector3 position, out Vector3 closestPoint)
        {
            closestPoint = Vector3.Zero;

            if (!IsLoaded)
                return false;

            lock (_meshLock)
            {
                try
                {
                    var posC = new NativeMethods.XYZ(position);
                    NativeMethods.XYZ closestC;
                    bool success = NativeMethods.ClosestPointOnPolyBoundary(mapId, polyRef.Id, posC, out closestC);
                    if (success)
                        closestPoint = closestC.ToVector3();
                    return success;
                }
                catch (Exception ex)
                {
                    Log($"ClosestPointOnPolyBoundary exception: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Finds the nearest polygon reference to a position.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="position">Position to search from.</param>
        /// <param name="polyRef">Output polygon reference.</param>
        /// <param name="nearestPoint">Output nearest point on polygon.</param>
        /// <returns>True if a polygon was found.</returns>
        public bool FindNearestPolyRef(uint mapId, Vector3 position, out PolygonReference polyRef, out Vector3 nearestPoint)
        {
            polyRef = PolygonReference.Invalid;
            nearestPoint = Vector3.Zero;

            if (!IsLoaded)
                return false;

            lock (_meshLock)
            {
                try
                {
                    var posC = new NativeMethods.XYZ(position);
                    var extentsC = new NativeMethods.XYZ(Extents);
                    NativeMethods.XYZ nearestC;
                    bool success = NativeMethods.FindNearestPolyRef(mapId, posC, extentsC, out ulong refId, out nearestC);
                    if (success)
                    {
                        polyRef = new PolygonReference(refId);
                        nearestPoint = nearestC.ToVector3();
                    }
                    return success;
                }
                catch (Exception ex)
                {
                    Log($"FindNearestPolyRef exception: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Queries polygons within a bounding box centered at position.
        /// Like HB's NavMesh.QueryPolygons.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="center">Center of search area.</param>
        /// <param name="extents">Search extents (half-dimensions).</param>
        /// <param name="maxResults">Maximum polygons to return.</param>
        /// <returns>Array of polygon references found.</returns>
        public PolygonReference[] QueryPolygons(uint mapId, Vector3 center, Vector3 extents, int maxResults = 256)
        {
            if (!IsLoaded || maxResults <= 0)
                return Array.Empty<PolygonReference>();

            lock (_meshLock)
            {
                try
                {
                    var centerC = new NativeMethods.XYZ(center);
                    var extentsC = new NativeMethods.XYZ(extents);

                    // Allocate buffer for results
                    ulong[] polyRefs = new ulong[maxResults];
                    
                    unsafe
                    {
                        fixed (ulong* ptr = polyRefs)
                        {
                            int count = NativeMethods.QueryPolygons(mapId, centerC, extentsC, 
                                (IntPtr)ptr, maxResults);
                            
                            if (count <= 0)
                                return Array.Empty<PolygonReference>();

                            var results = new PolygonReference[count];
                            for (int i = 0; i < count; i++)
                                results[i] = new PolygonReference(polyRefs[i]);
                            
                            return results;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log($"QueryPolygons exception: {ex.Message}");
                    return Array.Empty<PolygonReference>();
                }
            }
        }

        /// <summary>
        /// Finds local polygon neighbourhood around a starting polygon.
        /// Useful for local obstacle detection and area analysis.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="startPoly">Starting polygon reference.</param>
        /// <param name="center">Center position.</param>
        /// <param name="radius">Search radius.</param>
        /// <param name="maxResults">Maximum results.</param>
        /// <returns>Array of polygon references in the neighbourhood.</returns>
        public PolygonReference[] FindLocalNeighbourhood(uint mapId, PolygonReference startPoly, 
            Vector3 center, float radius, int maxResults = 64)
        {
            if (!IsLoaded || !startPoly.IsValid || maxResults <= 0)
                return Array.Empty<PolygonReference>();

            lock (_meshLock)
            {
                try
                {
                    var centerC = new NativeMethods.XYZ(center);

                    ulong[] polys = new ulong[maxResults];
                    ulong[] parents = new ulong[maxResults];

                    unsafe
                    {
                        fixed (ulong* polysPtr = polys)
                        fixed (ulong* parentsPtr = parents)
                        {
                            int count = NativeMethods.FindLocalNeighbourhood(mapId, startPoly.Id, centerC, radius,
                                (IntPtr)polysPtr, (IntPtr)parentsPtr, maxResults);

                            if (count <= 0)
                                return Array.Empty<PolygonReference>();

                            var results = new PolygonReference[count];
                            for (int i = 0; i < count; i++)
                                results[i] = new PolygonReference(polys[i]);

                            return results;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log($"FindLocalNeighbourhood exception: {ex.Message}");
                    return Array.Empty<PolygonReference>();
                }
            }
        }

        /// <summary>
        /// Gets wall segments for a polygon (edges that are boundaries).
        /// Useful for edge avoidance and MoveAwayFromEdges post-processing.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="polyRef">Polygon reference.</param>
        /// <param name="maxSegments">Maximum segments to return.</param>
        /// <returns>List of wall segments (start, end point pairs).</returns>
        public (Vector3 start, Vector3 end)[] GetPolyWallSegments(uint mapId, PolygonReference polyRef, int maxSegments = 32)
        {
            if (!IsLoaded || !polyRef.IsValid || maxSegments <= 0)
                return Array.Empty<(Vector3, Vector3)>();

            lock (_meshLock)
            {
                try
                {
                    NativeMethods.XYZ[] starts = new NativeMethods.XYZ[maxSegments];
                    NativeMethods.XYZ[] ends = new NativeMethods.XYZ[maxSegments];

                    unsafe
                    {
                        fixed (NativeMethods.XYZ* startsPtr = starts)
                        fixed (NativeMethods.XYZ* endsPtr = ends)
                        {
                            int count = NativeMethods.GetPolyWallSegments(mapId, polyRef.Id,
                                (IntPtr)startsPtr, (IntPtr)endsPtr, maxSegments);

                            if (count <= 0)
                                return Array.Empty<(Vector3, Vector3)>();

                            var results = new (Vector3, Vector3)[count];
                            for (int i = 0; i < count; i++)
                            {
                                results[i] = (starts[i].ToVector3(), ends[i].ToVector3());
                            }

                            return results;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log($"GetPolyWallSegments exception: {ex.Message}");
                    return Array.Empty<(Vector3, Vector3)>();
                }
            }
        }

        /// <summary>
        /// Finds polygons within a circle around a point.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="center">Center position.</param>
        /// <param name="radius">Search radius.</param>
        /// <param name="maxResults">Maximum results.</param>
        /// <returns>Array of polygon center positions.</returns>
        public Vector3[] FindPolysAroundCircle(uint mapId, Vector3 center, float radius, int maxResults = 64)
        {
            if (!IsLoaded || maxResults <= 0)
                return Array.Empty<Vector3>();

            lock (_meshLock)
            {
                try
                {
                    var centerC = new NativeMethods.XYZ(center);
                    NativeMethods.XYZ[] results = new NativeMethods.XYZ[maxResults];

                    unsafe
                    {
                        fixed (NativeMethods.XYZ* ptr = results)
                        {
                            int count = NativeMethods.FindPolysAroundCircle(mapId, centerC, radius,
                                (IntPtr)ptr, maxResults);

                            if (count <= 0)
                                return Array.Empty<Vector3>();

                            var positions = new Vector3[count];
                            for (int i = 0; i < count; i++)
                                positions[i] = results[i].ToVector3();

                            return positions;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log($"FindPolysAroundCircle exception: {ex.Message}");
                    return Array.Empty<Vector3>();
                }
            }
        }

        #endregion

        #region Sliced Pathfinding

        /// <summary>
        /// Initializes a sliced (async) path search.
        /// Use UpdateSlicedFindPath() to incrementally compute the path.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="start">Start position.</param>
        /// <param name="end">End position.</param>
        /// <returns>True if initialization succeeded.</returns>
        public bool InitSlicedFindPath(uint mapId, Vector3 start, Vector3 end)
        {
            if (!IsLoaded)
                return false;

            lock (_meshLock)
            {
                try
                {
                    var startC = new NativeMethods.XYZ(start);
                    var endC = new NativeMethods.XYZ(end);
                    return NativeMethods.InitSlicedFindPath(mapId, startC, endC);
                }
                catch (Exception ex)
                {
                    Log($"InitSlicedFindPath exception: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Updates sliced pathfinding with iteration limit.
        /// </summary>
        /// <param name="maxIterations">Maximum iterations to perform.</param>
        /// <returns>True if path search is complete or still in progress.</returns>
        public bool UpdateSlicedFindPath(int maxIterations)
        {
            if (!IsLoaded)
                return false;

            lock (_meshLock)
            {
                try
                {
                    return NativeMethods.UpdateSlicedFindPath(maxIterations);
                }
                catch (Exception ex)
                {
                    Log($"UpdateSlicedFindPath exception: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Updates sliced pathfinding with time budget.
        /// </summary>
        /// <param name="msBudget">Time budget in milliseconds.</param>
        /// <returns>True if path search is complete or still in progress.</returns>
        public bool UpdateSlicedFindPathMs(float msBudget)
        {
            if (!IsLoaded)
                return false;

            lock (_meshLock)
            {
                try
                {
                    return NativeMethods.UpdateSlicedFindPathMs(msBudget);
                }
                catch (Exception ex)
                {
                    Log($"UpdateSlicedFindPathMs exception: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Finalizes sliced path search and returns the path.
        /// </summary>
        /// <param name="maxPathSize">Maximum path size.</param>
        /// <returns>Array of path points, or empty if failed.</returns>
        public Vector3[] FinalizeSlicedFindPath(int maxPathSize = 256)
        {
            if (!IsLoaded)
                return Array.Empty<Vector3>();

            lock (_meshLock)
            {
                try
                {
                    IntPtr pathPtr = NativeMethods.FinalizeSlicedFindPath(maxPathSize, out int length);
                    if (pathPtr == IntPtr.Zero || length <= 0)
                        return Array.Empty<Vector3>();

                    try
                    {
                        var points = new Vector3[length];
                        unsafe
                        {
                            NativeMethods.XYZ* ptr = (NativeMethods.XYZ*)pathPtr.ToPointer();
                            for (int i = 0; i < length; i++)
                            {
                                points[i] = ptr[i].ToVector3();
                            }
                        }
                        return points;
                    }
                    finally
                    {
                        NativeMethods.FreePathArr(pathPtr);
                    }
                }
                catch (Exception ex)
                {
                    Log($"FinalizeSlicedFindPath exception: {ex.Message}");
                    return Array.Empty<Vector3>();
                }
            }
        }

        #endregion

        #region Path Following

        /// <summary>
        /// Updates path following with raycast shortcuts.
        /// Returns the new waypoint index to skip to if a shortcut is found.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="currentPos">Current position.</param>
        /// <param name="path">Path points array.</param>
        /// <param name="currentWaypointIndex">Current waypoint index.</param>
        /// <param name="agentId">Agent ID for multi-agent support.</param>
        /// <returns>New waypoint index (may be ahead if shortcut found).</returns>
        public int UpdatePathFollowing(uint mapId, Vector3 currentPos, Vector3[] path, int currentWaypointIndex, int agentId = 0)
        {
            if (!IsLoaded || path == null || path.Length == 0)
                return currentWaypointIndex;

            lock (_meshLock)
            {
                try
                {
                    var currentC = new NativeMethods.XYZ(currentPos);
                    
                    // Convert path to native array
                    var nativePath = new NativeMethods.XYZ[path.Length];
                    for (int i = 0; i < path.Length; i++)
                        nativePath[i] = new NativeMethods.XYZ(path[i]);

                    unsafe
                    {
                        fixed (NativeMethods.XYZ* pathPtr = nativePath)
                        {
                            return NativeMethods.UpdatePathFollowing(mapId, currentC, path.Length,
                                (IntPtr)pathPtr, currentWaypointIndex, agentId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log($"UpdatePathFollowing exception: {ex.Message}");
                    return currentWaypointIndex;
                }
            }
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Gets current navigation statistics (internal use only).
        /// HB WoD doesn't expose NavStats publicly.
        /// </summary>
        /// <returns>Navigation statistics.</returns>
        internal NativeMethods.NavStats GetNavStats()
        {
            NativeMethods.GetNavStats(out var stats);
            return stats;
        }

        /// <summary>
        /// Resets navigation statistics counters.
        /// </summary>
        internal void ResetNavStats()
        {
            NativeMethods.ResetNavStats();
        }

        #endregion

        #region Query Filter Settings

        /// <summary>
        /// Sets faction-aware query filter based on player faction.
        /// Ported from HB 6.2.3 WowNavigator.SetFactionQueryFilter.
        /// Excludes the opposite faction's ability flag and applies a 50x cost penalty
        /// on the opposite faction's area type (prevents pathing through enemy-only areas).
        /// </summary>
        /// <param name="isHorde">True if the player is Horde, false if Alliance.</param>
        public void SetFactionQueryFilter(bool isHorde)
        {
            try
            {
                // HB pattern: build faction filter from a fresh default filter.
                var baseFilter = _queryFilters["Default"].Clone();
                var exclude = baseFilter.ExcludeFlags;

                if (isHorde)
                {
                    exclude |= AbilityFlags.Alliance;
                    exclude &= ~AbilityFlags.Horde;

                    _currentQueryFilter = baseFilter;
                    _currentQueryFilter.ExcludeFlags = exclude;
                    ApplyCurrentQueryFilterToNative();

                    NativeMethods.SetAreaCost((uint)AreaType.Alliance, 50.0f);    // huge penalty
                    NativeMethods.SetAreaCost((uint)AreaType.Horde, 1.66f);       // normal
                    Log("Faction filter set: Horde (excluding Alliance paths)");
                }
                else
                {
                    exclude |= AbilityFlags.Horde;
                    exclude &= ~AbilityFlags.Alliance;

                    _currentQueryFilter = baseFilter;
                    _currentQueryFilter.ExcludeFlags = exclude;
                    ApplyCurrentQueryFilterToNative();

                    NativeMethods.SetAreaCost((uint)AreaType.Horde, 50.0f);       // huge penalty
                    NativeMethods.SetAreaCost((uint)AreaType.Alliance, 1.66f);     // normal
                    Log("Faction filter set: Alliance (excluding Horde paths)");
                }
            }
            catch (Exception ex)
            {
                Log($"Failed to set faction filter: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets polygon include flags for pathfinding.
        /// </summary>
        /// <param name="flags">Include flags.</param>
        public void SetIncludeFlags(ushort flags)
        {
            NativeMethods.SetIncludeFlags(flags);
        }

        /// <summary>
        /// Sets polygon exclude flags for pathfinding.
        /// </summary>
        /// <param name="flags">Exclude flags.</param>
        public void SetExcludeFlags(ushort flags)
        {
            NativeMethods.SetExcludeFlags(flags);
        }

        /// <summary>
        /// Gets current polygon include flags.
        /// </summary>
        /// <returns>Current include flags.</returns>
        public ushort GetIncludeFlags()
        {
            return NativeMethods.GetIncludeFlags();
        }

        /// <summary>
        /// Gets current polygon exclude flags.
        /// </summary>
        /// <returns>Current exclude flags.</returns>
        public ushort GetExcludeFlags()
        {
            return NativeMethods.GetExcludeFlags();
        }

        /// <summary>
        /// Sets traversal cost for an area type.
        /// Like HB WoD QueryFilter.SetAreaCost.
        /// </summary>
        /// <param name="areaId">Area type ID (0-63).</param>
        /// <param name="cost">Traversal cost multiplier.</param>
        public void SetAreaCost(uint areaId, float cost)
        {
            NativeMethods.SetAreaCost(areaId, cost);
        }

        /// <summary>
        /// Sets traversal cost for an area type using AreaType enum.
        /// Like HB WoD QueryFilter.SetAreaCost.
        /// </summary>
        /// <param name="areaType">Area type.</param>
        /// <param name="cost">Traversal cost multiplier.</param>
        public void SetAreaCost(AreaType areaType, float cost)
        {
            NativeMethods.SetAreaCost((uint)areaType, cost);
        }

        /// <summary>
        /// Gets traversal cost for an area type.
        /// Like HB WoD QueryFilter.GetAreaCost.
        /// </summary>
        /// <param name="areaId">Area type ID (0-63).</param>
        /// <returns>Traversal cost multiplier.</returns>
        public float GetAreaCost(uint areaId)
        {
            return NativeMethods.GetAreaCost(areaId);
        }

        /// <summary>
        /// Gets traversal cost for an area type using AreaType enum.
        /// </summary>
        /// <param name="areaType">Area type.</param>
        /// <returns>Traversal cost multiplier.</returns>
        public float GetAreaCost(AreaType areaType)
        {
            return NativeMethods.GetAreaCost((uint)areaType);
        }

        #endregion

        #region Polygon Area/Flags Manipulation

        /// <summary>
        /// Sets the area type for a polygon.
        /// Like HB WoD NavMesh.SetPolyArea - used for blackspot marking.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="polyRef">Polygon reference.</param>
        /// <param name="area">Area type ID (0-63).</param>
        /// <returns>Detour status (success if high bit set).</returns>
        public uint SetPolyArea(uint mapId, PolygonReference polyRef, byte area)
        {
            return NativeMethods.SetPolyArea(mapId, polyRef.Id, area);
        }

        /// <summary>
        /// Gets the area type for a polygon.
        /// Like HB WoD NavMesh.GetPolyArea.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="polyRef">Polygon reference.</param>
        /// <param name="area">Output area type.</param>
        /// <returns>Detour status (success if high bit set).</returns>
        public uint GetPolyArea(uint mapId, PolygonReference polyRef, out byte area)
        {
            return NativeMethods.GetPolyArea(mapId, polyRef.Id, out area);
        }

        /// <summary>
        /// Sets flags for a polygon.
        /// Like HB WoD NavMesh.SetPolyFlags.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="polyRef">Polygon reference.</param>
        /// <param name="flags">Polygon flags.</param>
        /// <returns>Detour status (success if high bit set).</returns>
        public uint SetPolyFlags(uint mapId, PolygonReference polyRef, ushort flags)
        {
            return NativeMethods.SetPolyFlags(mapId, polyRef.Id, flags);
        }

        /// <summary>
        /// Gets flags for a polygon.
        /// Like HB WoD NavMesh.GetPolyFlags.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="polyRef">Polygon reference.</param>
        /// <param name="flags">Output polygon flags.</param>
        /// <returns>Detour status (success if high bit set).</returns>
        public uint GetPolyFlags(uint mapId, PolygonReference polyRef, out ushort flags)
        {
            return NativeMethods.GetPolyFlags(mapId, polyRef.Id, out flags);
        }

        /// <summary>
        /// Sets path randomization for more natural movement.
        /// </summary>
        /// <param name="enabled">Enable randomization.</param>
        /// <param name="magnitude">Randomization magnitude (0-1).</param>
        public void SetPathRandomization(bool enabled, float magnitude = 0.05f)
        {
            NativeMethods.SetPathRandomization(enabled, magnitude);
        }

        /// <summary>
        /// Ensures tiles around position are loaded (HB-style streaming).
        /// Use this before pathfinding to ensure navmesh coverage.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="position">Center position.</param>
        /// <param name="ring">Tile ring radius (2 = 5x5 tiles).</param>
        public void EnsureTilesAroundPosition(uint mapId, Vector3 position, int ring = 2)
        {
            if (!IsLoaded)
                return;

            lock (_meshLock)
            {
                try
                {
                    var posC = new NativeMethods.XYZ(position);
                    NativeMethods.EnsureTiles(mapId, posC, ring);
                }
                catch (Exception ex)
                {
                    Log($"EnsureTilesAroundPosition exception: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Prefetches tiles in movement direction for smoother streaming.
        /// Call this regularly during movement for best performance.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="position">Current position.</param>
        /// <param name="velocity">Current velocity/movement direction.</param>
        /// <param name="ring">Base tile ring radius.</param>
        public void EnsureTilesDirectional(uint mapId, Vector3 position, Vector3 velocity, int ring = 2)
        {
            if (!IsLoaded)
                return;

            lock (_meshLock)
            {
                try
                {
                    var posC = new NativeMethods.XYZ(position);
                    var velC = new NativeMethods.XYZ(velocity);
                    NativeMethods.EnsureTilesDirectional(mapId, posC, velC, ring);
                }
                catch (Exception ex)
                {
                    Log($"EnsureTilesDirectional exception: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Gets direct pointer to dtNavMeshQuery for advanced NavBridge use.
        /// WARNING: Use with caution - direct Detour access.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <returns>Pointer to dtNavMeshQuery, or IntPtr.Zero if unavailable.</returns>
        public IntPtr GetNavMeshQueryPtr(uint mapId)
        {
            if (!IsLoaded)
                return IntPtr.Zero;

            lock (_meshLock)
            {
                try
                {
                    return NativeMethods.GetNavMeshQuery(mapId);
                }
                catch (Exception ex)
                {
                    Log($"GetNavMeshQueryPtr exception: {ex.Message}");
                    return IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// Gets direct pointer to default dtQueryFilter for advanced use.
        /// WARNING: Use with caution - direct Detour access.
        /// </summary>
        /// <returns>Pointer to dtQueryFilter.</returns>
        public IntPtr GetDefaultFilterPtr()
        {
            if (!IsLoaded)
                return IntPtr.Zero;

            lock (_meshLock)
            {
                try
                {
                    return NativeMethods.GetDefaultFilter();
                }
                catch (Exception ex)
                {
                    Log($"GetDefaultFilterPtr exception: {ex.Message}");
                    return IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// Checks if a specific tile is loaded.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <returns>True if tile is loaded.</returns>
        public bool IsTileLoaded(uint mapId, int tileX, int tileY)
        {
            return NativeMethods.IsTileLoaded(mapId, tileX, tileY);
        }

        /// <summary>
        /// Gets count of loaded tiles for a map.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <returns>Number of loaded tiles.</returns>
        public int GetLoadedTilesCount(uint mapId)
        {
            return NativeMethods.GetLoadedTilesCount(mapId);
        }

        #endregion

        #region OffMesh Connections

        /// <summary>
        /// Adds a custom offmesh connection at runtime.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="start">Connection start position.</param>
        /// <param name="end">Connection end position.</param>
        /// <param name="radius">Connection radius.</param>
        /// <param name="flags">Connection flags.</param>
        /// <param name="type">Connection type (0=normal, 1=elevator, 2=portal).</param>
        /// <param name="interactId">Object ID to interact with (for elevators/portals).</param>
        public void AddOffMeshConnection(uint mapId, Vector3 start, Vector3 end, float radius = 1.0f, 
            byte flags = 1, byte type = 0, uint interactId = 0)
        {
            if (!IsLoaded)
                return;

            lock (_meshLock)
            {
                try
                {
                    // AddOffMeshConnection not currently exported by Navigation.dll
                    // Would need to add export to DllMain.cpp if needed
                    Log($"AddOffMeshConnection not implemented in Navigation.dll");
                }
                catch (Exception ex)
                {
                    Log($"AddOffMeshConnection exception: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Loads offmesh connections for a specific tile from .offmesh file.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <returns>True if offmesh data was loaded successfully.</returns>
        public bool LoadTileOffMesh(uint mapId, int tileX, int tileY)
        {
            if (!IsLoaded)
                return false;

            lock (_meshLock)
            {
                try
                {
                    // LoadTileOffMesh not currently exported by Navigation.dll
                    // Tiles are loaded on-demand automatically
                    Log($"Tile offmesh loading handled automatically by Navigation.dll");
                    return true;
                }
                catch (Exception ex)
                {
                    Log($"LoadTileOffMesh exception: {ex.Message}");
                    return false;
                }
            }
        }

        #endregion

        #region Tile Management

        /// <summary>
        /// Ensures tiles within specified ring distance are loaded.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="position">Center position.</param>
        /// <param name="ring">Ring distance (number of tiles in each direction).</param>
        public void EnsureTiles(uint mapId, Vector3 position, int ring = 2)
        {
            if (!IsLoaded)
                return;

            lock (_meshLock)
            {
                try
                {
                    // EnsureTiles not directly exported - tiles load on-demand
                    // Navigation.dll handles tile streaming internally
                    Log($"Tile streaming handled automatically by Navigation.dll");
                }
                catch (Exception ex)
                {
                    Log($"EnsureTiles exception: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Gets tile coordinates for a world position.
        /// </summary>
        /// <param name="position">World position.</param>
        /// <returns>TileIdentifier for the position.</returns>
        public TileIdentifier GetTileByPosition(Vector3 position)
        {
            return TileIdentifier.GetByPosition(position.X, position.Y);
        }

        #endregion

        #region Event Helpers

        /// <summary>
        /// Callback invoked by Navigation.dll when a navmesh tile is loaded.
        /// Mirrors HB 6.2.3 WorldMeshManager.method_3 → fires TileLoaded event.
        /// </summary>
        private void OnNativeTileLoaded(uint mapId, int x, int y)
        {
            RaiseTileLoaded(mapId, x, y);
        }

        private void RaisePathProgress(PathFindResult result)
        {
            PathProgress?.Invoke(this, new PathProgressEventArgs(result));
        }

        private void RaiseTileLoaded(uint mapId, int tileX, int tileY)
        {
            TileLoaded?.Invoke(this, new TileLoadedEventArgs(mapId, tileX, tileY));
        }

        private void RaiseMapLoaded(uint mapId)
        {
            MapLoaded?.Invoke(this, new MapLoadedEventArgs(mapId));
        }

        private void Log(string message)
        {
            LogMessage?.Invoke(message);
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Releases all resources used by the Navigator.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;

            lock (_meshLock)
            {
                // Unregister native callback before GC can collect the delegate
                if (_nativeTileLoadedCallback != null)
                {
                    NativeMethods.SetTileLoadedCallback(null!);
                    _nativeTileLoadedCallback = null;
                }

                UnloadMeshes();
                _queryFilters.Clear();
                _isDisposed = true;
            }

            GC.SuppressFinalize(this);
        }

        #endregion
    }

    #region Event Args

    /// <summary>
    /// Event arguments for path progress events.
    /// </summary>
    public class PathProgressEventArgs : EventArgs
    {
        public PathFindResult Result { get; }

        public PathProgressEventArgs(PathFindResult result)
        {
            Result = result;
        }
    }

    /// <summary>
    /// Event arguments for tile loaded events.
    /// </summary>
    public class TileLoadedEventArgs : EventArgs
    {
        public uint MapId { get; }
        public int TileX { get; }
        public int TileY { get; }

        public TileLoadedEventArgs(uint mapId, int tileX, int tileY)
        {
            MapId = mapId;
            TileX = tileX;
            TileY = tileY;
        }
    }

    /// <summary>
    /// Event arguments for map loaded events.
    /// </summary>
    public class MapLoadedEventArgs : EventArgs
    {
        public uint MapId { get; }

        public MapLoadedEventArgs(uint mapId)
        {
            MapId = mapId;
        }
    }

    #endregion

    #region Query Filter

    /// <summary>
    /// Defines query filtering rules for pathfinding operations.
    /// Controls which polygon types are traversable and their costs.
    /// </summary>
    public class QueryFilter
    {
        /// <summary>
        /// Flags that must be present on a polygon for it to be traversable.
        /// </summary>
        public AbilityFlags IncludeFlags { get; set; } = AbilityFlags.All;

        /// <summary>
        /// Flags that prevent a polygon from being traversable.
        /// </summary>
        public AbilityFlags ExcludeFlags { get; set; } = AbilityFlags.Unwalkable | AbilityFlags.Transport;

        /// <summary>
        /// Cost multipliers for different area types.
        /// Higher cost = less preferred path.
        /// </summary>
        public Dictionary<AreaType, float> AreaCosts { get; set; } = new Dictionary<AreaType, float>();

        /// <summary>
        /// Creates a copy of this query filter.
        /// </summary>
        public QueryFilter Clone()
        {
            return new QueryFilter
            {
                IncludeFlags = IncludeFlags,
                ExcludeFlags = ExcludeFlags,
                AreaCosts = new Dictionary<AreaType, float>(AreaCosts)
            };
        }
    }

    #endregion
}
