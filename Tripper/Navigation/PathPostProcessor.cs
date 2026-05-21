using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Tripper.Navigation
{
    /// <summary>
    /// Post-processes paths to move waypoints away from navmesh edges.
    /// Exact port of HB 6.2.3 Class1458 (method_2/3/5/9/10/11).
    /// </summary>
    internal static class PathPostProcessor
    {
        private const float DefaultEdgeDistance = 2.0f;
        private const int MaxRecursionDepth = 5;
        private const int MaxRaycastPolys = 512;

        private enum MoveResult { NoWallNearPoint, Failed, Succeeded }

        private delegate void FixSubPathCallback(
            ref Vector3[] points, ref PolygonReference[] polys, ref StraightPathFlags[] flags);

        // ── Public entry points (match HB method_11 / method_8) ──

        public static void MoveAwayFromEdges(
            uint mapId,
            ref Vector3[] points,
            ref StraightPathFlags[] flags,
            float edgeDistance = DefaultEdgeDistance)
        {
            PolygonReference[] polygons = Array.Empty<PolygonReference>();
            MoveAwayFromEdges(mapId, ref points, ref flags, ref polygons, edgeDistance);
        }

        public static void MoveAwayFromEdges(
            uint mapId,
            ref Vector3[] points,
            ref StraightPathFlags[] flags,
            ref PolygonReference[] polygons,
            float edgeDistance = DefaultEdgeDistance)
        {
            if (points == null || points.Length < 2)
                return;

            // Ensure polygon array exists (HB always has one from FindStraightPath)
            if (polygons == null || polygons.Length == 0)
                polygons = new PolygonReference[points.Length];
            else if (polygons.Length < points.Length)
                Array.Resize(ref polygons, points.Length);

            // Fill zero‐polyRefs by snapping each point to the navmesh
            EnsurePolyRefs(mapId, points, ref polygons);

            MoveAwayFromEdgesRecursive(mapId, ref points, ref polygons, ref flags, edgeDistance, 0);
        }

        public static void Randomize(
            uint mapId,
            ref Vector3[] points,
            ref StraightPathFlags[] flags,
            float minOffset = 2.0f,
            float maxOffset = 6.0f)
        {
            PolygonReference[] polygons = Array.Empty<PolygonReference>();
            Randomize(mapId, ref points, ref flags, ref polygons, minOffset, maxOffset);
        }

        public static void Randomize(
            uint mapId,
            ref Vector3[] points,
            ref StraightPathFlags[] flags,
            ref PolygonReference[] polygons,
            float minOffset = 2.0f,
            float maxOffset = 6.0f)
        {
            if (points == null || points.Length < 2)
                return;

            if (polygons == null || polygons.Length == 0)
                polygons = new PolygonReference[points.Length];
            else if (polygons.Length < points.Length)
                Array.Resize(ref polygons, points.Length);

            EnsurePolyRefs(mapId, points, ref polygons);

            RandomizeRecursive(mapId, ref points, ref polygons, ref flags,
                               minOffset, maxOffset, 4f, new Random(), 0);
        }

        // ── Private: MoveAwayFromEdges recursion (HB method_10) ──

        private static void MoveAwayFromEdgesRecursive(
            uint mapId,
            ref Vector3[] points,
            ref PolygonReference[] polygons,
            ref StraightPathFlags[] flags,
            float edgeDistance,
            int depth)
        {
            if (depth > MaxRecursionDepth)
                return;

            var ptList = points.ToList();
            var polyList = polygons.ToList();
            var flagList = flags.ToList();

            // Pass 1: move waypoints away from edges (HB method_3)
            MoveWaypointsFromEdges(mapId, ptList, polyList, flagList, edgeDistance);

            // Pass 2: fix walkability with recursive callback (HB method_9)
            int capturedDepth = depth;
            FixSubPathCallback callback = (ref Vector3[] sp, ref PolygonReference[] spo, ref StraightPathFlags[] sf) =>
            {
                MoveAwayFromEdgesRecursive(mapId, ref sp, ref spo, ref sf, edgeDistance, capturedDepth + 1);
            };
            FixPathWalkability(mapId, ptList, polyList, flagList, callback);

            points = ptList.ToArray();
            polygons = polyList.ToArray();
            flags = flagList.ToArray();
        }

        // ── Private: Randomize recursion (HB method_7) ──

        private static void RandomizeRecursive(
            uint mapId,
            ref Vector3[] points,
            ref PolygonReference[] polygons,
            ref StraightPathFlags[] flags,
            float minOffset,
            float maxOffset,
            float maxRandom,
            Random random,
            int depth)
        {
            if (depth > MaxRecursionDepth)
                return;

            var ptList = points.ToList();
            var polyList = polygons.ToList();
            var flagList = flags.ToList();

            RandomizeWaypoints(mapId, ptList, polyList, flagList, minOffset, maxOffset, maxRandom, random);

            int capturedDepth = depth;
            FixSubPathCallback callback = (ref Vector3[] sp, ref PolygonReference[] spo, ref StraightPathFlags[] sf) =>
            {
                RandomizeRecursive(mapId, ref sp, ref spo, ref sf,
                                   minOffset, maxOffset, maxRandom, random, capturedDepth + 1);
            };
            FixPathWalkability(mapId, ptList, polyList, flagList, callback);

            points = ptList.ToArray();
            polygons = polyList.ToArray();
            flags = flagList.ToArray();
        }

        // ── MoveWaypointsFromEdges (HB method_3) ──

        private static void MoveWaypointsFromEdges(
            uint mapId,
            List<Vector3> points,
            List<PolygonReference> polygons,
            List<StraightPathFlags> flags,
            float edgeDistance)
        {
            for (int i = 1; i < points.Count - 1; i++)
            {
                if ((flags[i] & StraightPathFlags.OffMeshConnection) != 0)
                    continue;
                if ((flags[i - 1] & StraightPathFlags.OffMeshConnection) != 0)
                    continue;

                Vector3 point = points[i];
                PolygonReference polyRef = polygons[i];

                if (TryMoveAwayFromEdge(mapId, ref point, ref polyRef, edgeDistance) == MoveResult.Succeeded)
                {
                    points[i] = point;
                    polygons[i] = polyRef;
                }
            }
        }

        // ── TryMoveAwayFromEdge (exact port of HB method_2) ──

        private static MoveResult TryMoveAwayFromEdge(
            uint mapId, ref Vector3 point, ref PolygonReference polyRef, float edgeDistance)
        {
            // Step 1: FindDistanceToWall from polygon
            float distance = NativeMethods.FindDistanceToWallFromPoly(
                mapId, polyRef.Id, new NativeMethods.XYZ(point), edgeDistance,
                out NativeMethods.XYZ hitPointXyz, out NativeMethods.XYZ hitNormalXyz);

            if (distance < 0) // dtStatus failure
                return MoveResult.Failed;

            if (distance >= edgeDistance)
                return MoveResult.NoWallNearPoint;

            Vector3 wallHitPoint = hitPointXyz.ToVector3();
            Vector3 wallNormal = hitNormalXyz.ToVector3();

            // Step 2: snap hitPoint to nearest polygon (HB: FindNearestPolygon with extents 0.5/5/0.5 nav-space)
            // WoW-space equivalent: X=0.5, Y=0.5, Z=5 (Z=hauteur en WoW)
            Vector3 snappedHit = wallHitPoint;
            if (!NativeMethods.FindNearestPolyRef(
                    mapId,
                    new NativeMethods.XYZ(wallHitPoint),
                    new NativeMethods.XYZ(0.5f, 0.5f, 5f),
                    out ulong hitPolyRef,
                    out NativeMethods.XYZ snappedHitXyz))
                return MoveResult.Failed;

            if (hitPolyRef == 0)
                return MoveResult.Failed;

            snappedHit = snappedHitXyz.ToVector3();

            // Step 3: raycast from snappedHit toward (hitPoint + normal * edgeDistance * 2)
            Vector3 rayTarget = wallHitPoint + wallNormal * edgeDistance * 2f;
            float t = 0f;
            ulong[] rayPath = new ulong[MaxRaycastPolys];
            int rayPathCount = 0;

            for (int attempt = 0; attempt < 3; attempt++)
            {
                uint status = NativeMethods.Raycast(
                    mapId, hitPolyRef,
                    new NativeMethods.XYZ(snappedHit), new NativeMethods.XYZ(rayTarget),
                    out t, out _, rayPath, out rayPathCount, MaxRaycastPolys);

                if (NativeMethods.NavStatusIsFailure(status))
                    return MoveResult.Failed;

                if (rayPathCount > 0)
                    break; // got polys — proceed

                if (attempt == 2)
                    return MoveResult.Failed; // three strikes

                // Nudge start along ray direction (HB retry logic)
                Vector3 dir = rayTarget - snappedHit;
                float len = dir.Length();
                if (len > 0.001f)
                    dir /= len;

                if (attempt == 0)
                    snappedHit += dir * 0.2f;
                else
                    snappedHit -= dir * 0.4f;
            }

            // Step 4: compute new point
            if (t >= float.MaxValue * 0.5f)
            {
                // Ray reached end — no wall in that direction
                point = wallHitPoint + wallNormal * edgeDistance;
            }
            else
            {
                // Ray hit something — midpoint between wallHitPoint and ray hit
                Vector3 rayHitPos = snappedHit + (rayTarget - snappedHit) * t;
                point = (wallHitPoint + rayHitPos) * 0.5f;
            }

            // Step 5: GetPolyHeight on each visited poly to get correct Y
            for (int i = 0; i < rayPathCount; i++)
            {
                if (NativeMethods.GetPolyHeight(mapId, rayPath[i], new NativeMethods.XYZ(point), out float height))
                {
                    point = new Vector3(point.X, point.Y, height);
                    polyRef = new PolygonReference(rayPath[i]);
                    return MoveResult.Succeeded;
                }
            }

            return MoveResult.Failed;
        }

        // ── RandomizeWaypoints (HB method_4) ──

        private static void RandomizeWaypoints(
            uint mapId,
            List<Vector3> points,
            List<PolygonReference> polygons,
            List<StraightPathFlags> flags,
            float minOffset, float maxOffset, float maxRandom,
            Random random)
        {
            for (int i = 1; i < points.Count - 1; i++)
            {
                if ((flags[i] & StraightPathFlags.OffMeshConnection) != 0)
                    continue;
                if ((flags[i - 1] & StraightPathFlags.OffMeshConnection) != 0)
                    continue;

                float dist = (float)(random.NextDouble() * (maxOffset - minOffset) + minOffset);
                Vector3 pt = points[i];
                PolygonReference pr = polygons[i];
                bool moved = false;

                MoveResult result = TryMoveAwayFromEdge(mapId, ref pt, ref pr, dist);
                if (result == MoveResult.NoWallNearPoint)
                {
                    float r = (float)random.NextDouble() * Math.Min(dist, maxRandom);
                    if (NativeMethods.FindRandomPointAroundCircle(
                            mapId, new NativeMethods.XYZ(pt), r, out NativeMethods.XYZ rndPt))
                    {
                        pt = rndPt.ToVector3();
                        // Re-snap to get poly ref
                        if (NativeMethods.FindNearestPolyRef(mapId,
                                new NativeMethods.XYZ(pt), new NativeMethods.XYZ(0.5f, 0.5f, 5f),
                                out ulong rndRef, out _) && rndRef != 0)
                            pr = new PolygonReference(rndRef);
                        moved = true;
                    }
                }
                else if (result == MoveResult.Succeeded)
                {
                    moved = true;
                }

                if (moved)
                {
                    points[i] = pt;
                    polygons[i] = pr;
                }
            }
        }

        // ── FixPathWalkability (exact port of HB method_5) ──

        private static void FixPathWalkability(
            uint mapId,
            List<Vector3> points,
            List<PolygonReference> polygons,
            List<StraightPathFlags> flags,
            FixSubPathCallback? callback)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                if ((flags[i] & StraightPathFlags.OffMeshConnection) != 0)
                    continue;
                if ((flags[i + 1] & StraightPathFlags.OffMeshConnection) != 0)
                    continue;

                Vector3 segStart = points[i];
                Vector3 segEnd = points[i + 1];

                if (Vector3.DistanceSquared(segStart, segEnd) < 0.01f)
                    continue;

                // Raycast along the segment
                ulong startPolyId = polygons[i].Id;
                if (startPolyId == 0)
                    continue;

                ulong[] rayPath = new ulong[MaxRaycastPolys];
                uint rcStatus = NativeMethods.Raycast(
                    mapId, startPolyId,
                    new NativeMethods.XYZ(segStart), new NativeMethods.XYZ(segEnd),
                    out float t, out _, rayPath, out _, MaxRaycastPolys);

                if (NativeMethods.NavStatusIsFailure(rcStatus))
                    continue;

                // If raycast blocked at the very start (t ≈ 0), nudge start forward 0.1
                if (t <= 1e-7f)
                {
                    Vector3 dir = segEnd - segStart;
                    float len = dir.Length();
                    if (len > 0.001f)
                        dir /= len;

                    Vector3 nudged = segStart + dir * 0.1f;

                    if (NativeMethods.FindNearestPolyRef(mapId,
                            new NativeMethods.XYZ(nudged), new NativeMethods.XYZ(0.5f, 0.5f, 5f),
                            out ulong nudgedRef, out NativeMethods.XYZ nudgedSnapped) && nudgedRef != 0)
                    {
                        nudged = nudgedSnapped.ToVector3();

                        rcStatus = NativeMethods.Raycast(
                            mapId, nudgedRef,
                            new NativeMethods.XYZ(nudged), new NativeMethods.XYZ(segEnd),
                            out t, out _, rayPath, out _, MaxRaycastPolys);

                        if (NativeMethods.NavStatusIsFailure(rcStatus))
                            continue;

                        if (t > 1e-7f)
                        {
                            points[i] = nudged;
                            polygons[i] = new PolygonReference(nudgedRef);
                        }
                    }
                    // If snap failed, fall through — reroute block below fires with original t ≈ 0
                }

                // If still blocked (t < FLT_MAX), reroute the segment
                if (t < float.MaxValue * 0.5f)
                {
                    // Get end polygon
                    ulong endPolyId = (i + 1 < polygons.Count) ? polygons[i + 1].Id : 0;
                    if (endPolyId == 0)
                    {
                        if (!NativeMethods.FindNearestPolyRef(mapId,
                                new NativeMethods.XYZ(segEnd), new NativeMethods.XYZ(0.5f, 0.5f, 5f),
                                out endPolyId, out _) || endPolyId == 0)
                            continue;
                    }

                    // Use CalculatePathEx to find an alternative route for this segment
                    // (replaces HB's FindPath + FindStraightPath pair)
                    if (!CalculateSubPath(mapId, points[i], segEnd,
                            out Vector3[] subPts, out StraightPathFlags[] subFlags, out PolygonReference[] subPolys))
                        continue;

                    if (subPts.Length <= 1)
                        continue;

                    // Recursive callback on the sub-path (HB delegate12_0)
                    if (callback != null)
                        callback(ref subPts, ref subPolys, ref subFlags);

                    // Insert intermediate points (skip first and last — they overlap with existing path)
                    int insertCount = subPts.Length - 2;
                    if (insertCount > 0)
                    {
                        points.InsertRange(i + 1, subPts.Skip(1).Take(insertCount));
                        polygons.InsertRange(i + 1, subPolys.Skip(1).Take(insertCount));
                        flags.InsertRange(i + 1, subFlags.Skip(1).Take(insertCount));
                        i += insertCount;
                    }
                }
            }
        }

        // ── Helpers ──

        /// <summary>
        /// Computes a sub-path between two points using CalculatePathEx.
        /// Replaces HB's FindPath + FindStraightPath pair.
        /// </summary>
        private static bool CalculateSubPath(
            uint mapId, Vector3 start, Vector3 end,
            out Vector3[] points, out StraightPathFlags[] flags, out PolygonReference[] polygons)
        {
            points = Array.Empty<Vector3>();
            flags = Array.Empty<StraightPathFlags>();
            polygons = Array.Empty<PolygonReference>();

            IntPtr resultPtr = NativeMethods.CalculatePathEx(
                mapId, new NativeMethods.XYZ(start), new NativeMethods.XYZ(end), true);

            if (resultPtr == IntPtr.Zero)
                return false;

            try
            {
                var result = Marshal.PtrToStructure<NativeMethods.PathResult>(resultPtr);

                if (NativeMethods.NavStatusIsFailure(result.Status) || result.Length <= 0)
                    return false;

                // Reject partial paths (HB checks HasFlag(64) = DT_PARTIAL_RESULT)
                if ((result.Status & 0x40) != 0)
                    return false;

                int len = result.Length;
                points = new Vector3[len];
                flags = new StraightPathFlags[len];
                polygons = new PolygonReference[len];

                unsafe
                {
                    var pts = (NativeMethods.XYZ*)result.Points.ToPointer();
                    for (int i = 0; i < len; i++)
                        points[i] = pts[i].ToVector3();

                    if (result.StraightPathFlags != IntPtr.Zero)
                    {
                        byte* fl = (byte*)result.StraightPathFlags.ToPointer();
                        for (int i = 0; i < len; i++)
                            flags[i] = (StraightPathFlags)fl[i];
                    }

                    if (result.PolyRefs != IntPtr.Zero)
                    {
                        ulong* pr = (ulong*)result.PolyRefs.ToPointer();
                        for (int i = 0; i < len; i++)
                            polygons[i] = new PolygonReference(pr[i]);
                    }
                }

                return true;
            }
            finally
            {
                NativeMethods.FreePathResult(resultPtr);
            }
        }

        /// <summary>
        /// Ensures every element in the polygons array has a valid poly ref.
        /// Snaps points with zero refs to the navmesh.
        /// </summary>
        private static void EnsurePolyRefs(uint mapId, Vector3[] points, ref PolygonReference[] polygons)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (i >= polygons.Length || polygons[i].Id == 0)
                {
                    if (NativeMethods.FindNearestPolyRef(mapId,
                            new NativeMethods.XYZ(points[i]),
                            new NativeMethods.XYZ(0.5f, 0.5f, 5f),
                            out ulong refId, out _) && refId != 0)
                    {
                        if (i < polygons.Length)
                            polygons[i] = new PolygonReference(refId);
                    }
                }
            }
        }
    }
}
