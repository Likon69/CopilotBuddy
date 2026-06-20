using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Tripper.Navigation
{
    /// <summary>
    /// P/Invoke declarations for Navigation.dll C exports.
    /// Provides low-level access to Detour navmesh pathfinding functionality.
    /// EXACT MATCH to DllMain.cpp exports from .Reference\C++\Navigation
    /// </summary>
    internal static class NativeMethods
    {
        private const string DllName = "Navigation.dll";

        #region Structures

        [StructLayout(LayoutKind.Sequential)]
        internal struct XYZ
        {
            public float X;
            public float Y;
            public float Z;

            public XYZ(float x, float y, float z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public XYZ(Vector3 vec)
            {
                X = vec.X;
                Y = vec.Y;
                Z = vec.Z;
            }

            public Vector3 ToVector3() => new Vector3(X, Y, Z);
        }

        // Matches NavStats_C in NavBridge.h — exactly 6 fields (24 bytes x86)
        [StructLayout(LayoutKind.Sequential)]
        internal struct NavStats
        {
            public float PathfindTimeMs;
            public int PolysVisited;
            public float PathLength;
            public int ShortcutsApplied;
            public int StuckRecoveries;
            public int PathRecalculations;
        }

        /// <summary>
        /// Native PathResult structure from Navigation.dll.
        /// Matches C++ PathResult struct in PathResult.h
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct PathResult
        {
            public IntPtr Points;              // XYZ* array
            public IntPtr StraightPathFlags;   // unsigned char* (StraightPathFlags)
            public IntPtr PolyTypes;           // unsigned char* (AreaType)
            public IntPtr AbilityFlags;        // unsigned char* (AbilityFlags)
            public IntPtr PolyRefs;            // uint64_t* array (polygon references for each point)
            public int Length;
            public uint Status;                // NavStatusFlag bits
            public int FailStep;               // NavPathFindStep
        }

        #endregion

        #region Basic Pathfinding - EXACT NAMES from DllMain.cpp

        /// <summary>
        /// Calculates a path from start to end position.
        /// Returns array of XYZ points that must be freed with FreePathArr.
        /// Navigation.dll initializes automatically in DllMain - no LoadMaps needed.
        /// </summary>
        [DllImport(DllName, EntryPoint = "CalculatePath_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CalculatePath(
            uint mapId,
            XYZ start,
            XYZ end,
            [MarshalAs(UnmanagedType.I1)] bool smoothPath,
            out int length);

        /// <summary>
        /// Frees path array allocated by CalculatePath.
        /// </summary>
        [DllImport(DllName, EntryPoint = "FreePathArr_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void FreePathArr(IntPtr pathArr);

        /// <summary>
        /// Calculates extended path result with detailed information (flags, status).
        /// Returns PathResult pointer that must be freed with FreePathResult.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CalculatePathEx(
            uint mapId,
            XYZ start,
            XYZ end,
            [MarshalAs(UnmanagedType.I1)] bool straightPath);

        /// <summary>
        /// Frees PathResult allocated by CalculatePathEx.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void FreePathResult(IntPtr result);

        #endregion

        #region Loader

        [DllImport(DllName, EntryPoint = "Nav_LoadMaps", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool LoadMaps();

        [DllImport(DllName, EntryPoint = "Nav_UnloadMaps", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void UnloadMaps();

        #endregion

        #region Advanced Detour Navigation Functions - Like HB RecastManaged

        /// <summary>
        /// Finds the nearest valid navmesh point to a given position.
        /// </summary>
        // Maps to FindNearestPointEx_C — extents match HB 6.2.3 WowNavigator.Extents=(3,20,3)
        [DllImport(DllName, EntryPoint = "FindNearestPointEx_C", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool FindNearestPoly(
            uint mapId,
            XYZ center,
            float extentX,
            float extentY,
            float extentZ,
            out XYZ nearestPoint);

        [DllImport(DllName, EntryPoint = "FindNearestPoint_C", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool FindNearestPoint(
            uint mapId,
            XYZ position,
            out XYZ nearestPoint);

        /// <summary>
        /// Finds polygons within a circle around a point.
        /// </summary>
        // DllMain.cpp exports FindPolysAroundCircle (sans _C) — old API layer
        [DllImport(DllName, EntryPoint = "FindPolysAroundCircle", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int FindPolysAroundCircle(
            uint mapId,
            XYZ center,
            float radius,
            IntPtr results,
            int maxResults);

        /// <summary>
        /// Finds distance to nearest wall/boundary.
        /// </summary>
        // NavBridge.h exports FindDistanceToWall_C.
        [DllImport(DllName, EntryPoint = "FindDistanceToWall_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float FindDistanceToWall(
            uint mapId,
            XYZ position,
            float maxRadius,
            out XYZ hitPoint);

        /// <summary>
        /// Finds distance to nearest wall/boundary with extended output.
        /// Returns hitNormal in addition to hitPoint (like HB WoD).
        /// </summary>
        [DllImport(DllName, EntryPoint = "FindDistanceToWallEx_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float FindDistanceToWallEx(
            uint mapId,
            XYZ position,
            float maxRadius,
            out XYZ hitPoint,
            out XYZ hitNormal);

        /// <summary>
        /// Finds distance to nearest wall from a specific polygon.
        /// Used when we already know which polygon the point is on (like HB WoD method_2).
        /// </summary>
        [DllImport(DllName, EntryPoint = "FindDistanceToWallFromPoly_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float FindDistanceToWallFromPoly(
            uint mapId,
            ulong polyRef,
            XYZ position,
            float maxRadius,
            out XYZ hitPoint,
            out XYZ hitNormal);

        /// <summary>
        /// Checks if a point is on the navmesh.
        /// </summary>
        [DllImport(DllName, EntryPoint = "IsPointOnNavMesh_C", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool IsPointOnNavMesh(
            uint mapId,
            XYZ point,
            float tolerance);

        /// <summary>
        /// Finds a random navigable point within radius.
        /// </summary>
        [DllImport(DllName, EntryPoint = "FindRandomPoint_C", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool FindRandomPointAroundCircle(
            uint mapId,
            XYZ center,
            float radius,
            out XYZ outResult);

        /// <summary>
        /// Checks if there's line of sight between two points on navmesh.
        /// </summary>
        [DllImport(DllName, EntryPoint = "HasLineOfSight_C", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool HasLineOfSight(
            uint mapId,
            XYZ start,
            XYZ end);

        [DllImport(DllName, EntryPoint = "Raycast_C", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RaycastSimple(
            uint mapId,
            XYZ start,
            XYZ end,
            out XYZ hitPos,
            out float tHit);

        #endregion

        #region Poly Reference Functions
        // Note: dtPolyRef is unsigned long long (64-bit) with DT_POLYREF64 defined
        // This matches HB 4.3.4 Tripper.RecastManaged.Detour.PolygonReference

        /// <summary>
        /// Finds the nearest polygon reference to a position.
        /// </summary>
        [DllImport(DllName, EntryPoint = "FindNearestPolyRef_C", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool FindNearestPolyRef(
            uint mapId,
            XYZ position,
            XYZ extents,
            out ulong outPolyRef,
            out XYZ nearestPoint);

        /// <summary>
        /// Gets the height at a position on a specific polygon.
        /// </summary>
        [DllImport(DllName, EntryPoint = "GetPolyHeight_C", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool GetPolyHeight(
            uint mapId,
            ulong polyRef,
            XYZ position,
            out float outHeight);

        /// <summary>
        /// Finds the closest point on a polygon.
        /// </summary>
        [DllImport(DllName, EntryPoint = "ClosestPointOnPoly_C", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ClosestPointOnPoly(
            uint mapId,
            ulong polyRef,
            XYZ position,
            out XYZ closestPoint);

        /// <summary>
        /// Finds the closest point on polygon boundary.
        /// </summary>
        [DllImport(DllName, EntryPoint = "ClosestPointOnPolyBoundary_C", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ClosestPointOnPolyBoundary(
            uint mapId,
            ulong polyRef,
            XYZ position,
            out XYZ closestPoint);

        #endregion

        #region Polygon Area/Flags Manipulation (for blackspot marking)

        /// <summary>
        /// Sets the user defined area for a polygon.
        /// Used for marking blackspots - like HB Tripper.RecastManaged.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="polyRef">The polygon reference.</param>
        /// <param name="area">The new area id for the polygon. [Limit: &lt; DT_MAX_AREAS]</param>
        /// <returns>dtStatus - DT_SUCCESS or error flags.</returns>
        [DllImport(DllName, EntryPoint = "SetPolyArea_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint SetPolyArea(uint mapId, ulong polyRef, byte area);

        /// <summary>
        /// Gets the user defined area for a polygon.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="polyRef">The polygon reference.</param>
        /// <param name="outArea">The area id for the polygon.</param>
        /// <returns>dtStatus - DT_SUCCESS or error flags.</returns>
        [DllImport(DllName, EntryPoint = "GetPolyArea_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint GetPolyArea(uint mapId, ulong polyRef, out byte outArea);

        /// <summary>
        /// Sets the user defined flags for a polygon.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="polyRef">The polygon reference.</param>
        /// <param name="flags">The new flags for the polygon.</param>
        /// <returns>dtStatus - DT_SUCCESS or error flags.</returns>
        [DllImport(DllName, EntryPoint = "SetPolyFlags_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint SetPolyFlags(uint mapId, ulong polyRef, ushort flags);

        /// <summary>
        /// Gets the user defined flags for a polygon.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="polyRef">The polygon reference.</param>
        /// <param name="outFlags">The polygon flags.</param>
        /// <returns>dtStatus - DT_SUCCESS or error flags.</returns>
        [DllImport(DllName, EntryPoint = "GetPolyFlags_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint GetPolyFlags(uint mapId, ulong polyRef, out ushort outFlags);

        #endregion

        #region Polygon Queries

        /// <summary>
        /// Queries polygons within a bounding box.
        /// </summary>
        [DllImport(DllName, EntryPoint = "QueryPolygons_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int QueryPolygons(
            uint mapId,
            XYZ center,
            XYZ extents,
            IntPtr outPolys,
            int maxPolys);

        /// <summary>
        /// Finds local polygon neighbourhood around a position.
        /// </summary>
        [DllImport(DllName, EntryPoint = "FindLocalNeighbourhood_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int FindLocalNeighbourhood(
            uint mapId,
            ulong startPolyRef,
            XYZ center,
            float radius,
            IntPtr outPolys,
            IntPtr outParents,
            int maxResults);

        /// <summary>
        /// Gets wall segments for a polygon.
        /// </summary>
        [DllImport(DllName, EntryPoint = "GetPolyWallSegments_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetPolyWallSegments(
            uint mapId,
            ulong polyRef,
            IntPtr outSegmentStart,
            IntPtr outSegmentEnd,
            int maxSegments);

        #endregion

        #region Sliced Pathfinding - HB Style

        /// <summary>
        /// Initializes a sliced (async) path search.
        /// </summary>
        [DllImport(DllName, EntryPoint = "InitSlicedFindPath", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool InitSlicedFindPath(
            uint mapId,
            XYZ start,
            XYZ end);

        /// <summary>
        /// Updates sliced pathfinding with iteration limit.
        /// </summary>
        [DllImport(DllName, EntryPoint = "UpdateSlicedFindPath", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool UpdateSlicedFindPath(int maxIterations);

        /// <summary>
        /// Updates sliced pathfinding with time budget (ms).
        /// </summary>
        [DllImport(DllName, EntryPoint = "UpdateSlicedFindPathMs", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool UpdateSlicedFindPathMs(float msBudget);

        /// <summary>
        /// Finalizes sliced path and returns result.
        /// Must free result with FreePathArr.
        /// </summary>
        [DllImport(DllName, EntryPoint = "FinalizeSlicedFindPath", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr FinalizeSlicedFindPath(
            int maxPathSize,
            out int length);

        #endregion

        #region Query Filter

        /// <summary>
        /// Sets polygon flags to include in pathfinding.
        /// </summary>
        [DllImport(DllName, EntryPoint = "SetIncludeFlags_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetIncludeFlags(ushort flags);

        /// <summary>
        /// Sets polygon flags to exclude from pathfinding.
        /// </summary>
        [DllImport(DllName, EntryPoint = "SetExcludeFlags_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetExcludeFlags(ushort flags);

        /// <summary>
        /// Gets current polygon include flags.
        /// </summary>
        [DllImport(DllName, EntryPoint = "GetIncludeFlags_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ushort GetIncludeFlags();

        /// <summary>
        /// Gets current polygon exclude flags.
        /// </summary>
        [DllImport(DllName, EntryPoint = "GetExcludeFlags_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ushort GetExcludeFlags();

        /// <summary>
        /// Sets traversal cost for an area type.
        /// SetAreaCost_C takes (mapId, areaType, cost) but mapId is ignored (singleton filter).
        /// </summary>
        [DllImport(DllName, EntryPoint = "SetAreaCost_C", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SetAreaCost(uint mapId, int areaId, float cost);

        /// <summary>
        /// Gets traversal cost for an area type.
        /// </summary>
        [DllImport(DllName, EntryPoint = "GetAreaCost_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float GetAreaCost(int areaId);

        #endregion

        #region Tile Management

        /// <summary>
        /// Checks if a specific tile is loaded.
        /// </summary>
        [DllImport(DllName, EntryPoint = "IsTileLoaded_C", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool IsTileLoaded(
            uint mapId,
            int x,
            int y);

        /// <summary>
        /// Gets count of loaded tiles for a map.
        /// </summary>
        [DllImport(DllName, EntryPoint = "GetLoadedTilesCount_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetLoadedTilesCount(uint mapId);

        /// <summary>
        /// Counts loaded ADT (MaNGOS) tiles for the map. One ADT = up to 16 Detour
        /// sub-tiles in V5 4x4 format. This differs from <see cref="GetLoadedTilesCount"/>
        /// which counts Detour sub-tiles.
        /// </summary>
        [DllImport(DllName, EntryPoint = "GetLoadedAdtCount_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetLoadedAdtCount(uint mapId);

        /// <summary>
        /// Unloads a specific MaNGOS ADT tile. In V5 .mmtile format this removes
        /// all Detour sub-tiles stored inside that ADT tile.
        /// </summary>
        [DllImport(DllName, EntryPoint = "UnloadTile_C", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool UnloadTile(
            uint mapId,
            int x,
            int y);

        /// <summary>
        /// Ensures tiles around position are loaded (HB-style streaming).
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="position">Center position.</param>
        /// <param name="ring">Tile ring radius (2 = 5x5 tiles).</param>
        [DllImport(DllName, EntryPoint = "EnsureTiles_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void EnsureTiles(
            uint mapId,
            XYZ position,
            int ring);

        /// <summary>
        /// Prefetches tiles in movement direction for smoother streaming.
        /// </summary>
        /// <param name="mapId">Map ID.</param>
        /// <param name="position">Current position.</param>
        /// <param name="velocity">Current velocity/direction.</param>
        /// <param name="ring">Base tile ring radius.</param>
        [DllImport(DllName, EntryPoint = "EnsureTilesDirectional_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void EnsureTilesDirectional(
            uint mapId,
            XYZ position,
            XYZ velocity,
            int ring);

        [DllImport(DllName, EntryPoint = "WorldToTile_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void WorldToTile(
            float worldX,
            float worldZ,
            out int tileX,
            out int tileY);

        /// <summary>
        /// Gets direct pointer to dtNavMeshQuery (for advanced NavBridge use).
        /// </summary>
        [DllImport(DllName, EntryPoint = "GetNavMeshQuery", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetNavMeshQuery(uint mapId);

        /// <summary>
        /// Gets direct pointer to default dtQueryFilter (for advanced use).
        /// </summary>
        [DllImport(DllName, EntryPoint = "GetDefaultFilter", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetDefaultFilter();

        /// <summary>
        /// Callback delegate invoked by Navigation.dll when a navmesh tile is loaded.
        /// Mirrors HB 6.2.3 Tripper.RecastManaged.Detour.LoadTileDelegate pattern.
        /// </summary>
        // StdCall matches MMAP::TileLoadedCallback typedef (void __stdcall*) in MoveMap.h
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void TileLoadedCallbackDelegate(uint mapId, int x, int y);

        /// <summary>
        /// Registers a managed callback to be invoked when a tile is loaded.
        /// Mirrors HB 6.2.3 NavMeshQuery.SetTileLoaderFunction().
        /// </summary>
        [DllImport(DllName, EntryPoint = "SetTileLoadedCallback_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetTileLoadedCallback(TileLoadedCallbackDelegate callback);

        /// <summary>
        /// Log callback delegate — invoked by Navigation.dll for internal events (pathfind results, tile loads).
        /// Mirrors pattern of TileLoadedCallbackDelegate. Fires into C# OnNavigatorLogMessage.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void NavLogCallbackDelegate([MarshalAs(UnmanagedType.LPStr)] string msg);

        /// <summary>
        /// Registers the log callback. Must be called once after the DLL is loaded.
        /// Same lifecycle as SetTileLoadedCallback.
        /// </summary>
        [DllImport(DllName, EntryPoint = "SetNavLogCallback_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetNavLogCallback(NavLogCallbackDelegate callback);

        #endregion

        #region Path Following

        /// <summary>
        /// Updates path following with raycast shortcuts.
        /// Returns the new waypoint index to use.
        /// </summary>
        [DllImport(DllName, EntryPoint = "UpdatePathFollowing_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int UpdatePathFollowing(
            uint mapId,
            XYZ currentPos,
            int pathLength,
            IntPtr pathPoints,
            int currentWaypointIndex,
            int agentId);

        #endregion

        #region Raycast - HB Style

        /// <summary>
        /// HB-style raycast: resolves start polygon then calls dtNavMeshQuery::raycast
        /// returning visited poly corridor (for push-ahead shortcuts).
        /// Exported as Raycast_HB_C in NavBridge.
        /// </summary>
        [DllImport(DllName, EntryPoint = "Raycast_HB_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint Raycast(
            uint mapId,
            ulong startRef,
            XYZ startPos,
            XYZ endPos,
            out float outT,
            out XYZ outHitNormal,
            [In, Out] ulong[] outPath,
            out int outPathCount,
            int maxPath);

        #endregion

        #region Statistics

        /// <summary>
        /// Gets current navigation statistics via out-pointer wrapper.
        /// DLL exports GetNavStats_C(NavStats_C* out) — fills the struct in-place.
        /// </summary>
        [DllImport(DllName, EntryPoint = "GetNavStats_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetNavStats(out NavStats outStats);

        /// <summary>
        /// Resets navigation statistics counters.
        /// </summary>
        [DllImport(DllName, EntryPoint = "ResetNavStats_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ResetNavStats();

        #endregion

        #region NavStatus Helpers

        [DllImport(DllName, EntryPoint = "NavStatus_FailureFlag", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint NavStatusFailureFlag();

        [DllImport(DllName, EntryPoint = "NavStatus_SuccessFlag", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint NavStatusSuccessFlag();

        [DllImport(DllName, EntryPoint = "NavStatus_InProgressFlag", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint NavStatusInProgressFlag();

        [DllImport(DllName, EntryPoint = "NavStatus_PartialResultFlag", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint NavStatusPartialResultFlag();

        [DllImport(DllName, EntryPoint = "NavStatus_IsFailure", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool NavStatusIsFailure(uint status);

        [DllImport(DllName, EntryPoint = "NavStatus_IsSuccess", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool NavStatusIsSuccess(uint status);

        [DllImport(DllName, EntryPoint = "NavStatus_IsInProgress", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool NavStatusIsInProgress(uint status);

        [DllImport(DllName, EntryPoint = "NavStatus_HasFlag", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool NavStatusHasFlag(uint status, uint flag);

        #endregion

        #region OffMesh Connections - HB 4.3.4 Style

        /// <summary>
        /// Checks if a position is near an offmesh connection (portal, elevator, etc.).
        /// Returns the endpoint, type, and interact ID if found.
        /// </summary>
        [DllImport(DllName, EntryPoint = "IsOffMeshConnection_C", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool IsOffMeshConnection(
            uint mapId,
            XYZ position,
            out XYZ outEnd,
            out byte outType,
            out uint outInteractId);

        /// <summary>
        /// Adds a custom offmesh connection at runtime (injected into the navmesh).
        /// </summary>
        [DllImport(DllName, EntryPoint = "AddOffMeshConnection_C", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void AddOffMeshConnection(
            uint mapId,
            XYZ start,
            XYZ end,
            float radius,
            byte flags,
            byte type,
            uint interactId);

        /// <summary>
        /// Loads offmesh connections from binary .offmesh file for a specific tile.
        /// </summary>
        [DllImport(DllName, EntryPoint = "LoadTileOffMesh_C", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool LoadTileOffMesh(uint mapId, int tileX, int tileY);

        #endregion
    }
}
