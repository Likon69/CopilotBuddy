using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Bots.DungeonBuddy.Helpers;
using Styx;
using Styx.Helpers;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Tripper.Tools.Math;

namespace Bots.DungeonBuddy.Avoidance
{
    public static class Helpers
    {
        // HB 6.2.3 pattern: Helpers delegates to Navigator statics for path access
        // (replaces the MeshNavigator.CurrentMovePath dynamic dispatch from HB 6.2.3)

        // smethod_0: adjusts Z to nearest ground height
        internal static Vector3 AdjustGroundPoint(Vector3 point)
        {
            try
            {
                var heights = Navigator.FindHeights(point.X, point.Y);
                if (heights != null && heights.Count > 0)
                    point.Z = heights.OrderBy(h => Math.Abs(h - point.Z)).First();
            }
            catch { }
            return point;
        }

        // smethod_6: WoWPoint -> Vector3 identity
        private static Vector3 WoWPointToVector3(WoWPoint p) => p;

        // smethod_7: dictionary ContainsKey+ContainsValue check
        private static bool AvoidLocationMatchesCurrent(Avoid a)
            => _avoidLocations.ContainsKey(a) && _avoidLocations[a].Equals(a.Location);

        // smethod_8: leash check
        private static bool IsWithinLeash(AvoidTracelineResult h)
            => h.Avoid.LeashPoint == WoWPoint.Zero || h.Avoid.LeashPoint.DistanceSqr(h.Exit) < h.Avoid.LeashRadiusSqr;

        // smethod_9: Exit selector
        private static Vector3 ExitSelector(AvoidTracelineResult h) => h.Exit;

        // smethod_10: trace.Hits > 0
        private static bool TraceHasHits(Class252<Avoid, AvoidTracelineResult> x)
            => x.trace.Hits > 0;

        // smethod_11: trace selector
        private static AvoidTracelineResult TraceSelector(Class252<Avoid, AvoidTracelineResult> x) => x.trace;

        // smethod_12: nearestHit != null
        private static bool HasNearestHit(Class253<AvoidCluster, AvoidTracelineResult> x)
            => x.nearestHit != null;

        // smethod_13: Class254(cluster, nearestHit.Exit - nearestHit.Enter)
        private static Class254<Class253<AvoidCluster, AvoidTracelineResult>, Vector3> ToClusterDelta(Class253<AvoidCluster, AvoidTracelineResult> x)
            => new Class254<Class253<AvoidCluster, AvoidTracelineResult>, Vector3>(x, x.nearestHit.Exit - x.nearestHit.Enter);

        // smethod_14: ClusterHit from Class254
        private static ClusterHit ToClusterHit(Class254<Class253<AvoidCluster, AvoidTracelineResult>, Vector3> x)
            => new ClusterHit(x.Cluster.cluster, x.Cluster.nearestHit.Enter, (float)Math.Atan2(x.relativePoint.Y, x.relativePoint.X));

        public static AvoidPathResult GetAvoidPath(WoWPoint destination)
        {
            var @class = new Class111();
            @class.woWPoint_1 = destination;
            @class.woWPoint_0 = StyxWoW.Me.Location;

            var clusterOnPath = AvoidanceManager.AvoidClusters.FirstOrDefault(c => c.Any(a => a.IsPointInAvoid(@class.woWPoint_0)));
            if (clusterOnPath != null)
            {
                // HB 6.2.3: exit directed toward current target; fall back to nearest if no target.
                WoWUnit currentTarget0 = StyxWoW.Me.CurrentTarget;
                WoWPoint goal0 = currentTarget0 != null ? currentTarget0.Location : WoWPoint.Zero;
                var safeLocation = GetBestLocationOutsideCluster(@class.woWPoint_0, goal0, 2, clusterOnPath, 1.5f);
                if (!safeLocation.Equals(Vector3.Zero))
                {
                    // HB 6.2.3: reuse cached escape path if endpoint hasn't moved.
                    if (_cachedAvoidPathResult == null || _cachedAvoidPathResult.Path == null ||
                        _cachedAvoidPathResult.Path.Length == 0 ||
                        _cachedAvoidPathResult.Path[_cachedAvoidPathResult.Path.Length - 1].DistanceSqr(safeLocation) > 0.25f)
                    {
                        // HB 6.2.3: GeneratePath(...).Skip(1) — skip the start (player pos) from path array.
                        var rawPath = Styx.Logic.Pathing.Navigator.ComputeRawPath(@class.woWPoint_0, safeLocation);
                        _cachedAvoidPathResult = new AvoidPathResult(PathResult.Changed,
                            rawPath.Skip(1).Select(WoWPointToVector3).ToArray());
                    }
                    _lastDestination = safeLocation;
                    return _cachedAvoidPathResult;
                }
            }

            var avoids = AvoidanceManager.Avoids;
            if (_avoidLocations.Count > 0 && _avoidLocations.Count == avoids.Count && avoids.All(AvoidLocationMatchesCurrent))
            {
                if (avoids.Count == 0)
                    return null;
                if (_lastDestination.DistanceSqr(@class.woWPoint_1) < 0.25f)
                    return _cachedAvoidPathResult;
            }

            if (!Styx.Logic.Pathing.Navigator.HasActivePath)
                return null;

            // HB 6.2.3: save original destination BEFORE any cluster-redirect modification.
            // woWPoint_ = @class.woWPoint_1 is captured before clusterBlocking may change it.
            // _lastDestination is updated to this original value at the end so cache hits work correctly.
            WoWPoint originalDestination = @class.woWPoint_1;

            var clusterBlocking = AvoidanceManager.AvoidClusters.FirstOrDefault(c => c.Any(a => a.IsPointInAvoid(@class.woWPoint_1)));
            bool pathOverriddenByCluster = false;
            if (clusterBlocking != null)
            {
                // HB 6.2.3: goal = last nav-path point that's NOT inside the cluster,
                // so the exit is directed toward where the path was already going.
                WoWPoint lastSafeNavPt = Styx.Logic.Pathing.Navigator.GetRemainingNavPath()
                    .LastOrDefault(p => !clusterBlocking.Any(a => a.IsPointInAvoid(p)));
                @class.woWPoint_1 = GetBestLocationOutsideCluster(@class.woWPoint_1, lastSafeNavPt, 2, clusterBlocking, 1f);
                if (@class.woWPoint_1 == WoWPoint.Zero)
                    return null;
                // HB 6.2.3: ALWAYS regenerate the nav path to the redirected destination.
                // currentMovePath.Path = meshNavigator_.FindPath(from, newDest); currentMovePath.Index = 0;
                Styx.Logic.Pathing.Navigator.OverrideCurrentPath(
                    Styx.Logic.Pathing.Navigator.ComputeRawPath(@class.woWPoint_0, @class.woWPoint_1));
                pathOverriddenByCluster = true;
            }

            // HB 6.2.3: refresh nav path when avoid set has changed (skip if already overridden above).
            if (!pathOverriddenByCluster && avoids.Count != _avoidLocations.Count)
            {
                Styx.Logic.Pathing.Navigator.OverrideCurrentPath(
                    Styx.Logic.Pathing.Navigator.ComputeRawPath(@class.woWPoint_0, @class.woWPoint_1));
            }

            _avoidLocations.Clear();
            foreach (var a in avoids)
                _avoidLocations[a] = a.Location;

            if (avoids.Count == 0)
                return null;

            var pathArray = Styx.Logic.Pathing.Navigator.GetRemainingNavPath().Select(WoWPointToVector3).ToArray();
            AvoidPathResult avoidPathResult;
            try
            {
                // HB 6.2.3 smethod_1(woWPoint_0, woWPoint_1, array2, 0): first arg is CURRENT player position.
                // _lastLocation is only updated AFTER this call (would be Zero on first tick).
                avoidPathResult = ComputeAvoidPathRecursive(@class.woWPoint_0, @class.woWPoint_1, pathArray, 0);
            }
            catch (AvoidPathNotFoundException ex)
            {
                Logging.WriteDebug("Avoid path generation has failed: {0}", ex.Message);
                Logging.WriteDebug("Start:{0}, End:{1}", @class.woWPoint_0, @class.woWPoint_1);
                foreach (var a in avoids)
                    Logging.WriteDebug("\tLocation:{0}, Radius {1} ", a.Location, a.Radius);
                ClearAvoidPath();
                return null;
            }

            // HB 6.2.3: woWPoint_0 = woWPoint_ (original destination, not the redirected one).
            _lastDestination = originalDestination;
            // HB 6.2.3: if destination was redirected (clusterBlocking), downgrade Changed → Partial.
            // Signals caller that this is a partial route to a redirected dest, not the final goal.
            if (avoidPathResult.Result > PathResult.Partial && clusterBlocking != null)
                avoidPathResult = new AvoidPathResult(PathResult.Partial, avoidPathResult.Path);
            if (avoidPathResult.Result != PathResult.Unchanged && avoidPathResult.Result != PathResult.Failed)
            {
                // HB 6.2.3: MeshTraceline(Me.Location, path[1]) == false → LOS clear → skip waypoint 0.
                // The bot is already close enough that it can steer directly to the second waypoint,
                // avoiding the micro-jitter of stopping at path[0] first.
                // CB: Raycast returns false when the ray is unobstructed (hitT >= 1.0f = no navmesh wall).
                if (avoidPathResult.Path != null && avoidPathResult.Path.Length > 1)
                {
                    var path1WP = new WoWPoint(avoidPathResult.Path[1].X, avoidPathResult.Path[1].Y, avoidPathResult.Path[1].Z);
                    if (!Styx.Logic.Pathing.Navigator.Raycast(@class.woWPoint_0, path1WP, out _))
                        avoidPathResult.Index = 1;
                }
                _cachedAvoidPathResult = avoidPathResult;
                return avoidPathResult;
            }
            // HB 6.2.3: Unchanged or Failed → clear cache (avoidPathResult_0 = null) and return null.
            // If cached, next tick would return stale Changed path even when no obstacle is in the way.
            _cachedAvoidPathResult = null;
            return null;
        }

        public static void ClearAvoidPath()
        {
            _avoidLocations.Clear();
            _cachedAvoidPathResult = null;
            _lastDestination = WoWPoint.Zero;
        }

        /// <summary>
        /// Wrapper consumed by Navigator.NavAvoidWaypointProvider.
        /// Returns the current avoidance detour as WoWPoint[] (remaining waypoints from Index),
        /// or null when no avoidance is needed.
        /// HB 6.2.3 AvoidanceNavigationProvider.MovePath() pattern.
        /// </summary>
        public static WoWPoint[] GetAvoidWaypoints(WoWPoint destination)
        {
            if (!StyxWoW.Me.IsAlive) return null;
            var result = GetAvoidPath(destination);
            if (result == null || result.Path == null || result.Path.Length == 0)
                return null;
            // PathResult.Unchanged: no obstacles intersect the nav path — normal navigation handles it.
            // HB 6.2.3 AvoidanceNavigationProvider falls through to base.MovePath() for Unchanged.
            if (result.Result == PathResult.Unchanged)
                return null;
            int idx = result.Index >= 0 ? result.Index : 0;
            return result.Path.Skip(idx).Select(v => new WoWPoint(v.X, v.Y, v.Z)).ToArray();
        }

        private static AvoidPathResult ComputeAvoidPathRecursive(Vector3 from, Vector3 to, Vector3[] points, int depth)
        {
            for (int i = 0; i < points.Length; i++)
            {
                var segStart = i == 0 ? from : points[i - 1];
                var segEnd = points[i];
                var hit = AvoidClusterTraceline(AvoidanceManager.AvoidClusters, segStart, segEnd);
                if (hit == null)
                    continue;

                var tangents = hit.Cluster.GetLineTangentPoints(segStart);
                if (tangents == null)
                    continue;

                var side = DetermineAvoidSide(hit.Point, tangents.RightPoint, tangents.LeftPoint, segStart);
                var leftTarget = new AvoidTarget(tangents.LeftAvoid, tangents.LeftPoint, AvoidSide.Left);
                var rightTarget = new AvoidTarget(tangents.RightAvoid, tangents.RightPoint, AvoidSide.Right);
                var preferred = side == AvoidSide.Right ? rightTarget : leftTarget;
                var alternative = side == AvoidSide.Right ? leftTarget : rightTarget;

                var preferredResult = ComputeAvoidForTarget(preferred, segStart, to, depth);
                if (preferredResult.Result > PathResult.Partial)
                    return preferredResult;

                var altResult = ComputeAvoidForTarget(alternative, segStart, to, depth);
                if (altResult.Result > PathResult.Partial)
                    return altResult;

                if (preferredResult.Result == PathResult.Failed && altResult.Result == PathResult.Partial)
                {
                    var partial = points.Take(i).ToList();
                    partial.Add(WoWMathHelper.CalculatePointFrom(from, hit.Point, Styx.Logic.Pathing.Navigator.PathPrecision + 0.25f));
                    return new AvoidPathResult(PathResult.Partial, partial.ToArray());
                }

                return preferredResult;
            }

            return new AvoidPathResult(PathResult.Unchanged, points);
        }

        private static AvoidPathResult ComputeAvoidForTarget(AvoidTarget target, Vector3 from, Vector3 to, int depth)
        {
            if (depth >= 80)
                throw new AvoidPathNotFoundException(string.Format("Recursion limit was reached ({0})", 80));

            var path = new List<Vector3>();
            var overallResult = PathResult.Changed;
            var currentPoint = target.Point;
            var avoid = target.Avoid;
            var side = target.Side;

            var offset = currentPoint - new Vector3(avoid.Location.X, avoid.Location.Y, avoid.Location.Z);
            float angle = (float)Math.Atan2(offset.Y, offset.X);
            var edgePoint = AdjustGroundPoint(avoid.Location.RayCast(angle, avoid.Radius + 0.5f));
            var previousPoint = Vector3.Zero;

            if (edgePoint.DistanceSqr(to) < 0.25f)
                return new AvoidPathResult(PathResult.Changed, new Vector3[] { from, to });

            var directResult = ComputeDirectPath(edgePoint, to, avoid, depth);
            if (directResult.Result <= PathResult.Partial)
                return directResult;

            path.AddRange(directResult.Path.Take(directResult.Path.Length - 1));
            var detourPoints = new List<Vector3>();
            bool toggleSide = false;
            float step = 0.05f;
            int iterations = 0;
            AvoidPathResult lastResult = null;

            do
            {
                if (!previousPoint.Equals(Vector3.Zero) && !Styx.Logic.Pathing.Navigator.CanNavigateFully(previousPoint, edgePoint))
                    break;

                if (iterations++ > 100)
                    break;

                var hits = GetAvoidsHitByTraceline(AvoidanceManager.Avoids, edgePoint, to)
                    .OrderBy(h => h.Enter.DistanceSqr(to) - h.Avoid.RadiusSqr)
                    .ToArray();

                if (hits.Length == 0)
                {
                    var emptyResult = ComputeDirectPath(edgePoint, to, avoid, depth);
                    path.AddRange(detourPoints);
                    path.AddRange(emptyResult.Path);
                    overallResult = emptyResult.Result;
                    break;
                }

                if (hits[0].Avoid != avoid)
                {
                    LineCircleTangentPoints tangents = null;
                    bool blocked = true;
                    for (int idx = 0; idx < detourPoints.Count; idx++)
                    {
                        tangents = GetLineCircleTangentPoints(hits[0].Avoid.Location, hits[0].Avoid.Radius + 0.5f, detourPoints[idx]);
                        if (tangents == null)
                            continue;

                        var tangentPoint = side == AvoidSide.Right ? tangents.RightPoint : tangents.LeftPoint;
                        blocked = GetAvoidsHitByTraceline(AvoidanceManager.Avoids, detourPoints[idx], tangentPoint).Any();
                        if (!blocked)
                        {
                            if (idx + 1 < detourPoints.Count)
                                detourPoints.RemoveRange(idx + 1, detourPoints.Count - (idx + 1));
                            edgePoint = detourPoints[idx];
                            break;
                        }
                    }

                    if (tangents == null || blocked)
                        break;

                    avoid = hits[0].Avoid;
                    var tanPt = side == AvoidSide.Right ? tangents.RightPoint : tangents.LeftPoint;
                    offset = tanPt - new Vector3(avoid.Location.X, avoid.Location.Y, avoid.Location.Z);
                    angle = (float)Math.Atan2(offset.Y, offset.X);
                    var rayEnd = avoid.Location.RayCast(angle, avoid.Radius + 0.5f);
                    lastResult = ComputeDirectPath(edgePoint, rayEnd, avoid, depth);
                    detourPoints.AddRange(lastResult.Path);
                    if (lastResult.Result > PathResult.Partial)
                    {
                        step = 0.05f;
                        edgePoint = rayEnd;
                        continue;
                    }
                    break;
                }

                var otherClusters = AvoidanceManager.AvoidClusters.Where(c => c != null).ToList();
                var clusterHit = AvoidClusterTraceline(otherClusters, edgePoint, to);
                var clusterTangents = clusterHit?.Cluster.GetLineTangentPoints(edgePoint);
                if (clusterTangents != null)
                {
                    var clusterSide = DetermineAvoidSide(clusterHit.Point, clusterTangents.RightPoint, clusterTangents.LeftPoint, from);
                    var clLeft = new AvoidTarget(clusterTangents.LeftAvoid, clusterTangents.LeftPoint, AvoidSide.Left);
                    var clRight = new AvoidTarget(clusterTangents.RightAvoid, clusterTangents.RightPoint, AvoidSide.Right);
                    var primaryTarget = !toggleSide
                        ? (clusterSide == AvoidSide.Right ? clRight : clLeft)
                        : (clusterSide == AvoidSide.Right ? clLeft : clRight);

                    if (AvoidTraceline(avoid, edgePoint, primaryTarget.Point).Hits == 0)
                    {
                        if (toggleSide)
                        {
                            var recurseResult = ComputeAvoidForTarget(primaryTarget, edgePoint, to, depth);
                            path.AddRange(detourPoints);
                            path.AddRange(recurseResult.Path);
                            return new AvoidPathResult(recurseResult.Result > PathResult.Partial ? PathResult.Changed : recurseResult.Result, path.ToArray());
                        }

                        lastResult = ComputeAvoidForTarget(primaryTarget, edgePoint, to, depth);
                        if (lastResult.Result <= PathResult.Partial)
                        {
                            step = 0.05f;
                            toggleSide = true;
                            continue;
                        }

                        path.AddRange(detourPoints);
                        path.AddRange(lastResult.Path);
                        overallResult = lastResult.Result;
                        break;
                    }
                }

                detourPoints.Add(edgePoint);
                float rotate = WoWMathHelper.NormalizeRadian(6.28318548f * step);
                float newAngle = side == AvoidSide.Right ? rotate + angle : -rotate + angle;
                previousPoint = edgePoint;
                edgePoint = AdjustGroundPoint(avoid.Location.RayCast(newAngle, avoid.Radius + 0.5f));
                step += 0.05f;
            }
            while (step < 1f);

            if (iterations > 100 || (!previousPoint.Equals(Vector3.Zero) && !Styx.Logic.Pathing.Navigator.CanNavigateFully(previousPoint, edgePoint)))
            {
                overallResult = PathResult.Failed;
            }
            else if (detourPoints.Count == 0)
            {
                var fallback = ComputeDirectPath(edgePoint, to, avoid, depth);
                path.AddRange(detourPoints);
                path.AddRange(fallback.Path);
                overallResult = fallback.Result;
            }
            else
            {
                path.AddRange(detourPoints);
                overallResult = PathResult.Partial;
            }

            return new AvoidPathResult(overallResult > PathResult.Partial ? PathResult.Changed : overallResult, path.ToArray());
        }

        private static AvoidPathResult ComputeDirectPath(Vector3 from, Vector3 to, Avoid avoid, int depth)
        {
            // HB smethod_3: FindPath from → to, fail if empty, recurse to check for more avoids.
            var rawPts = Styx.Logic.Pathing.Navigator.ComputeRawPath(from, to);
            if (rawPts == null || rawPts.Length == 0)
                return new AvoidPathResult(PathResult.Failed, null);
            var pathPts = rawPts.Select(WoWPointToVector3).ToArray();
            if (avoid != null && pathPts.Any(p => avoid.IsPointInAvoid(p)))
                return new AvoidPathResult(PathResult.Failed, pathPts);
            return ComputeAvoidPathRecursive(from, to, pathPts, depth + 1);
        }

        /// <summary>
        /// HB 6.2.3 Helpers.GetBestLocationOutsideCluster — goal-directed cluster escape.
        /// Nudges the search origin toward 'goal' by up to 'goalPriority' units before scanning
        /// exit directions, so the bot exits on the side closest to its current target.
        /// CB uses the same Navigation.dll that HB's RecastManaged wraps, so all three
        /// checks are now exact ports:
        ///   - FindNearestPolyRef → candidate is on the navmesh
        ///   - Raycast hitT == FLT_MAX → no wall between searchOrigin and candidate
        ///   - FindDistanceToWall > 1f → candidate is not right next to a wall
        /// AvoidInfo.Priority is now wired in; dungeon scripts can set priority=High on
        /// instant-kill avoids so they dominate the exit-score and are escaped first.
        /// </summary>
        public static Vector3 GetBestLocationOutsideCluster(Vector3 from, Vector3 goal, int goalPriority, AvoidCluster cluster, float extendDistance)
        {
            Vector3 searchOrigin = from;
            goalPriority %= 10;

            // Nudge searchOrigin toward goal by min(dist, goalPriority) units.
            // HB: vector3_0 = vector3_0.smethod_12(heading, min(dist, goalPriority))
            if (!goal.Equals(Vector3.Zero) && !goal.Equals(from))
            {
                float dx = goal.X - from.X;
                float dy = goal.Y - from.Y;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                if (dist > 0f)
                {
                    float nudge = dist < (float)goalPriority ? dist : (float)goalPriority;
                    float heading = (float)Math.Atan2(dy, dx);
                    searchOrigin = new Vector3(
                        from.X + (float)Math.Cos(heading) * nudge,
                        from.Y + (float)Math.Sin(heading) * nudge,
                        from.Z);
                    searchOrigin = AdjustGroundPoint(searchOrigin);
                }
            }

            // Tag each cluster avoid: is searchOrigin inside it?
            // If searchOrigin is outside all avoids, return it directly — already safe.
            var avoidPairs = cluster.Select(a => (avoid: a, inside: a.IsPointInAvoid(searchOrigin))).ToList();
            if (!avoidPairs.Any(p => p.inside))
                return searchOrigin;

            // Scan 20 equidistant angles. For each, find the farthest exit through blocking avoids.
            // Score = sum of (enter→exit distance * Priority) — higher priority avoids dominate.
            // Ascending score = shortest weighted escape path = preferred direction.
            var candidates = new List<(float score, Vector3 point)>();
            for (float t = 0f; t < 1f; t += 0.05f)
            {
                float angle = 6.28318548f * t;
                WoWPoint rayEnd = GetPointAt(searchOrigin, 100f, angle, 0f);

                var rayHits = avoidPairs
                    .Where(p => p.inside)
                    .Select(p => new { pair = p, trace = AvoidTraceline(p.avoid, searchOrigin, rayEnd) })
                    .Where(x => x.trace.Hits > 0)
                    .OrderByDescending(x => x.trace.Exit.DistanceSqr(searchOrigin))
                    .ToList();

                if (!rayHits.Any()) continue;

                var farthest = rayHits.First();
                Vector3 exitPt = farthest.trace.Exit;
                Vector3 enterPt = farthest.pair.inside ? searchOrigin : farthest.trace.Enter;
                exitPt.Z = enterPt.Z;

                // Extend past the exit surface by extendDistance
                Vector3 dir = exitPt - enterPt;
                float len = (float)Math.Sqrt(dir.X * dir.X + dir.Y * dir.Y);
                if (len > 0f) { dir.X /= len; dir.Y /= len; dir.Z = 0f; }
                Vector3 extended = exitPt + dir * extendDistance;

                // HB 6.2.3: score weighted by AvoidInfo.Priority so high-priority avoids
                // (instant-kill fire) push the bot to pick the exit that clears them fastest.
                float score = rayHits.Sum(x =>
                {
                    Vector3 entry = x.pair.inside ? searchOrigin : x.trace.Enter;
                    float ddx = x.trace.Exit.X - entry.X;
                    float ddy = x.trace.Exit.Y - entry.Y;
                    return (float)Math.Sqrt(ddx * ddx + ddy * ddy) * (float)x.pair.avoid.AvoidInfo.Priority;
                });

                if (!extended.Equals(Vector3.Zero))
                    candidates.Add((score, extended));
            }

            // HB 6.2.3 validation (exact port):
            //   1. Raycast(searchOrigin→candidate) must be clear — no navmesh wall between them.
            //      MeshTraceline: hitT == FLT_MAX → clear. CB Raycast returns true if blocked.
            //   2. FindDistanceToWall > 1f — candidate is not right against a wall.
            //      If the nav system isn't loaded, accept any candidate (fail-soft).
            WoWPoint searchOriginWP = new WoWPoint(searchOrigin.X, searchOrigin.Y, searchOrigin.Z);
            foreach (var (_, pt) in candidates.OrderBy(c => c.score))
            {
                Vector3 safe = AdjustGroundPoint(pt);
                if (Styx.Logic.Pathing.Navigator.IsNavigatorLoaded)
                {
                    WoWPoint safeWP = new WoWPoint(safe.X, safe.Y, safe.Z);
                    // HB 6.2.3: MeshTraceline(origin, candidate) must be false (hitT == FLT_MAX = no wall).
                    if (Styx.Logic.Pathing.Navigator.Raycast(searchOriginWP, safeWP, out _))
                        continue; // navmesh wall between origin and candidate — try next direction
                    // HB 6.2.3: DistToWall(candidate, 1.5f) != null && > 1f
                    float wallDist = Styx.Logic.Pathing.Navigator.FindDistanceToWall(safeWP, 5f);
                    if (wallDist <= 1f)
                        continue; // against a wall or off-mesh — try next direction
                }
                return safe;
            }

            return Vector3.Zero;
        }

        public static Vector3 GetNearestLocationOutsideCluster(Vector3 location, AvoidCluster cluster, float extendDistance)
        {
            var candidates = new List<Vector3>();
            for (float t = 0f; t < 1f; t += 0.05f)
            {
                float angle = 6.28318548f * t;
                var rayEnd = GetPointAt(location, 100f, angle, 0f);
                var hits = GetAvoidsHitByTraceline(cluster, location, rayEnd)
                    .Where(IsWithinLeash)
                    .OrderByDescending(h => h.Exit.DistanceSqr(location));
                var exit = hits.Select(ExitSelector).FirstOrDefault();
                if (!exit.Equals(Vector3.Zero))
                    candidates.Add(exit);
            }

            foreach (var candidate in candidates.OrderBy(c => location.DistanceSqr(c)))
            {
                var safePoint = AdjustGroundPoint(CalculatePointFrom(location, candidate, -extendDistance));
                if (Styx.Logic.Pathing.Navigator.CanNavigateFully(location, safePoint))
                    return safePoint;
            }

            return Vector3.Zero;
        }

        internal static MoveResult MoveAwayFromCluster(AvoidCluster cluster)
        {
            // HB 6.2.3 smethod_4: directed escape toward current target.
            WoWUnit currentTarget = StyxWoW.Me.CurrentTarget;
            WoWPoint goal = currentTarget != null ? currentTarget.Location : WoWPoint.Zero;
            var safeLocation = GetBestLocationOutsideCluster(StyxWoW.Me.Location, goal, 2, cluster, 1.5f);
            if (safeLocation.Equals(Vector3.Zero))
                return MoveResult.Failed;

            ClearAvoidPath();
            if (cluster[0] is AvoidObject ao)
            {
                string name = ao.Object is WoWDynamicObject ? WoWSpell.FromId((int)ao.Object.Entry).Name : ao.Object.Name;
                Logging.Write("Running from {0}", name);
            }
            else if (cluster[0] is AvoidLocation)
            {
                Logging.Write("Running from location {0}", cluster[0].Location);
            }

            if (StyxWoW.Me.Location.DistanceSqr(safeLocation) <= Styx.Logic.Pathing.Navigator.PathPrecision * Styx.Logic.Pathing.Navigator.PathPrecision)
            {
                WoWMovement.ClickToMove(safeLocation);
                return MoveResult.Moved;
            }
            return Styx.Logic.Pathing.Navigator.MoveTo(safeLocation);
        }

        private static AvoidSide DetermineAvoidSide(Vector3 hitPoint, Vector3 rightPoint, Vector3 leftPoint, Vector3 from)
        {
            float a = (float)Math.Atan2(hitPoint.Y - from.Y, hitPoint.X - from.X);
            float b = (float)Math.Atan2(rightPoint.Y - from.Y, rightPoint.X - from.X);
            float c = (float)Math.Atan2(leftPoint.Y - from.Y, leftPoint.X - from.X);
            return WoWMathHelper.NormalizeRadian(c - a) <= WoWMathHelper.NormalizeRadian(a - b) ? AvoidSide.Left : AvoidSide.Right;
        }

        public static IEnumerable<AvoidTracelineResult> GetAvoidsHitByTraceline(IEnumerable<Avoid> avoids, Vector3 start, Vector3 end)
        {
            var state = new Class115();
            state.vector3_0 = start;
            state.vector3_1 = end;
            return avoids
                .Select(a => new Class252<Avoid, AvoidTracelineResult>(a, AvoidTraceline(a, start, end)))
                .Where(TraceHasHits)
                .OrderByDescending(h => h.trace.Enter.DistanceSqr(end))
                .Select(TraceSelector);
        }

        public static ClusterHit AvoidClusterTraceline(List<AvoidCluster> clusters, Vector3 start, Vector3 end)
        {
            var state = new Class116();
            state.vector3_0 = start;
            state.vector3_1 = end;
            return clusters
                .Select(c => new Class253<AvoidCluster, AvoidTracelineResult>(c, GetAvoidsHitByTraceline(c, start, end).OrderBy(h => h.Enter.DistanceSqr(start)).FirstOrDefault()))
                .Where(HasNearestHit)
                .Select(ToClusterDelta)
                .OrderBy(x => x.Cluster.cluster.Center.DistanceSqr(start))
                .Select(ToClusterHit)
                .FirstOrDefault();
        }

        public static void BuildClusters(List<Avoid> avoids, List<AvoidCluster> clusters)
        {
            clusters.Clear();
            foreach (var avoid in avoids)
            {
                if (!clusters.Any(c => c.Contains(avoid)))
                {
                    var newCluster = new AvoidCluster(avoid);
                    while (true)
                    {
                        var overlapping = avoids.FirstOrDefault(a => !newCluster.Contains(a) && newCluster.Any(c => AreOverlapping(c, a)));
                        if (overlapping == null)
                            break;

                        newCluster.Add(overlapping);
                    }

                    clusters.Add(newCluster);
                }
            }
        }

        private static bool AreOverlapping(Avoid a, Avoid b)
        {
            float combined = a.Radius + b.Radius + 1f;
            return a.Location.DistanceSqr(b.Location) < combined * combined;
        }

        public static AvoidTracelineResult AvoidTraceline(Avoid avoid, Vector3 start, Vector3 end)
        {
            var intersections = GetLineCircleIntersections(avoid.Location, avoid.Radius + 0.25f, start, end, out var enter, out var exit);
            return new AvoidTracelineResult(intersections, enter, exit, avoid);
        }

        public static bool LineAvoidCollision(Avoid avoid, Vector3 start, Vector3 end)
            => GetLineCircleIntersections(avoid.Location, avoid.Radius + 0.25f, start, end, out _, out _) > 0;

        public static int GetLineCircleIntersections(Vector3 center, float radius, Vector3 start, Vector3 end, out Vector3 enterPoint, out Vector3 exitPoint)
        {
            float r2 = radius * radius;
            float dx = start.X - center.X;
            float dy = start.Y - center.Y;
            float lx = end.X - start.X;
            float ly = end.Y - start.Y;
            float l2 = lx * lx + ly * ly;
            float lDotD = 2f * (lx * dx + ly * dy);
            float d2 = dx * dx + dy * dy - r2;
            float disc = lDotD * lDotD - 4f * l2 * d2;

            if (l2 < 1e-7 || disc < 0f)
            {
                enterPoint = new Vector3(float.NaN, float.NaN, 0f);
                exitPoint = new Vector3(float.NaN, float.NaN, 0f);
                return 0;
            }

            if (Math.Abs(disc) < 1.401298E-45f)
            {
                float t = -lDotD / (2f * l2);
                enterPoint = new Vector3(start.X + t * lx, start.Y + t * ly, center.Z);
                exitPoint = new Vector3(float.NaN, float.NaN, 0f);
                return 1;
            }

            float sqrtDisc = (float)Math.Sqrt(disc);
            float t1 = (-lDotD - sqrtDisc) / (2f * l2);
            float t2 = (-lDotD + sqrtDisc) / (2f * l2);
            enterPoint = new Vector3(start.X + t1 * lx, start.Y + t1 * ly, center.Z);
            exitPoint = new Vector3(start.X + t2 * lx, start.Y + t2 * ly, center.Z);

            bool startOutside = start.DistanceSqr(center) > r2;
            bool endOutside = end.DistanceSqr(center) > r2;
            bool enterOutside = enterPoint.DistanceSqr(center) > r2;
            bool exitOutside = exitPoint.DistanceSqr(center) > r2;

            if ((startOutside || enterOutside) && (endOutside || exitOutside))
                return 0;
            return 2;
        }

        public static LineCircleTangentPoints GetLineCircleTangentPoints(Vector3 center, float radius, Vector3 externalPoint)
        {
            float dx = externalPoint.X - center.X;
            float dy = externalPoint.Y - center.Y;
            float d2 = dx * dx + dy * dy;
            float r2 = radius * radius;

            if (d2 < r2)
                return null;

            float invD2 = 1f / d2;
            float dist = (float)Math.Sqrt(Math.Abs(d2 - r2));
            float cos1 = center.X + radius * (radius * dx - dy * dist) * invD2;
            float sin1 = center.Y + radius * (radius * dy + dx * dist) * invD2;
            float cos2 = center.X + radius * (radius * dx + dy * dist) * invD2;
            float sin2 = center.Y + radius * (radius * dy - dx * dist) * invD2;

            return new LineCircleTangentPoints(
                new Vector3(cos1, sin1, center.Z),
                new Vector3(cos2, sin2, center.Z));
        }

        public static WoWPoint GetPointAt(Vector3 from, float distance, float angleRadians, float pitchRadians)
        {
            float cosA = (float)Math.Cos(angleRadians);
            float sinA = (float)Math.Sin(angleRadians);
            float cosP = (float)Math.Cos(pitchRadians);
            float sinP = (float)Math.Sin(pitchRadians);
            return new WoWPoint(
                from.X + distance * cosA * cosP,
                from.Y + distance * sinA * cosP,
                from.Z + distance * sinP);
        }

        public static WoWPoint CalculatePointFrom(Vector3 from, Vector3 to, float offset)
        {
            var dir = to - from;
            dir.Normalize();
            return from + dir * offset;
        }

        private const float TWO_PI = 6.28318548f;
        private const float PI = 3.14159274f;
        internal const float EXTEND_RADIUS = 0.5f;
        internal const float INNER_RADIUS = 0.25f;
        internal const float PATH_RADIUS = 0.25f;
        private const int MAX_RECURSION = 80;

        private static readonly Dictionary<Avoid, WoWPoint> _avoidLocations = new();
        private static AvoidPathResult _cachedAvoidPathResult;
        // HB 6.2.3 Helpers.woWPoint_0: tracks the LAST DESTINATION passed to GetAvoidPath, not player pos.
        // Used to detect destination changes for cache invalidation.
        private static WoWPoint _lastDestination;

        // CompilerGenerated delegate fields
        [CompilerGenerated] private static Func<WoWPoint, Vector3> _func_0;
        [CompilerGenerated] private static Func<Avoid, bool> _func_1;
        [CompilerGenerated] private static Func<AvoidTracelineResult, bool> _func_2;
        [CompilerGenerated] private static Func<AvoidTracelineResult, Vector3> _func_3;
        [CompilerGenerated] private static Func<Class252<Avoid, AvoidTracelineResult>, bool> _func_4;
        [CompilerGenerated] private static Func<Class252<Avoid, AvoidTracelineResult>, AvoidTracelineResult> _func_5;
        [CompilerGenerated] private static Func<Class253<AvoidCluster, AvoidTracelineResult>, bool> _func_6;
        [CompilerGenerated] private static Func<Class253<AvoidCluster, AvoidTracelineResult>, Class254<Class253<AvoidCluster, AvoidTracelineResult>, Vector3>> _func_7;
        [CompilerGenerated] private static Func<Class254<Class253<AvoidCluster, AvoidTracelineResult>, Vector3>, ClusterHit> _func_8;

        // Class110: ground Z adjustment
        private sealed class Class110
        {
            public Vector3 vector3_0;
            public float method_0(float z) => Math.Abs(z - this.vector3_0.Z);
        }

        // Class111: GetAvoidPath state
        private sealed class Class111
        {
            public bool method_0(AvoidCluster cluster) => cluster.Any(a => a.IsPointInAvoid(this.woWPoint_0));
            public bool method_1(AvoidCluster cluster) => cluster.Any(a => a.IsPointInAvoid(this.woWPoint_1));
            public bool method_2(Avoid avoid) => avoid.IsPointInAvoid(this.woWPoint_0);
            public bool method_3(Avoid avoid) => avoid.IsPointInAvoid(this.woWPoint_1);
            public WoWPoint woWPoint_0;
            public WoWPoint woWPoint_1;
        }

        // Class112: trace ordering
        private sealed class Class112
        {
            public Vector3 vector3_0;
            public AvoidCluster avoidCluster_0;
            public float method_0(AvoidTracelineResult h) => h.Enter.DistanceSqr(this.vector3_0) - h.Avoid.RadiusSqr;
            public bool method_1(AvoidCluster c) => c != this.avoidCluster_0;
        }

        // Class113: pathfind result check
        private sealed class Class113
        {
            public Avoid avoid_0;
            public bool method_0(Vector3 p) => this.avoid_0 != null && this.avoid_0.IsPointInAvoid(p);
        }

        // Class114: GetNearestLocationOutsideCluster
        private sealed class Class114
        {
            public Vector3 vector3_0;
            public float method_0(AvoidTracelineResult h) => this.vector3_0.DistanceSqr(h.Exit);
            public float method_1(Vector3 p) => this.vector3_0.DistanceSqr(p);
        }

        // Class115: GetAvoidsHitByTraceline
        private sealed class Class115
        {
            public Vector3 vector3_0;
            public Vector3 vector3_1;
            public Class252<Avoid, AvoidTracelineResult> method_0(Avoid a)
                => new Class252<Avoid, AvoidTracelineResult>(a, Helpers.AvoidTraceline(a, this.vector3_0, this.vector3_1));
            public float method_1(Class252<Avoid, AvoidTracelineResult> x)
                => this.vector3_1.DistanceSqr(x.trace.Enter);
        }

        // Class116: AvoidClusterTraceline
        private sealed class Class116
        {
            public Vector3 vector3_0;
            public Vector3 vector3_1;
            public Class253<AvoidCluster, AvoidTracelineResult> method_0(AvoidCluster c)
                => new Class253<AvoidCluster, AvoidTracelineResult>(c, Helpers.GetAvoidsHitByTraceline(c, this.vector3_0, this.vector3_1).OrderBy(h => h.Enter.DistanceSqr(this.vector3_0)).FirstOrDefault());
            public float method_1(Class254<Class253<AvoidCluster, AvoidTracelineResult>, Vector3> x)
                => this.vector3_0.DistanceSqr(x.Cluster.nearestHit.Enter);
            public float method_2(AvoidTracelineResult h)
                => this.vector3_0.DistanceSqr(h.Enter);
        }

        // Class117: BuildClusters - cluster contains avoid
        private sealed class Class117
        {
            public Avoid avoid_0;
            public bool method_0(AvoidCluster c) => c.Contains(this.avoid_0);
        }

        // Class118: BuildClusters - avoid not in cluster + overlap check
        private sealed class Class118
        {
            public Class117 class117_0;
            public AvoidCluster avoidCluster_0;
            public bool method_0(Avoid a) => !this.avoidCluster_0.Contains(a);
            public bool method_1(Avoid a)
            {
                var state = new Class119();
                state.class118_0 = this;
                state.class117_0 = this.class117_0;
                state.avoid_0 = a;
                return this.avoidCluster_0.Any(b => OverlapCheck(state.avoid_0, b));
            }
            private static bool OverlapCheck(Avoid a, Avoid b)
            {
                float combined = a.Radius + b.Radius + 1f;
                return a.Location.DistanceSqr(b.Location) < combined * combined;
            }
        }

        private sealed class Class119
        {
            public Class118 class118_0;
            public Class117 class117_0;
            public Avoid avoid_0;
        }

        private class AvoidTarget
        {
            public Avoid Avoid { get; }
            public Vector3 Point { get; }
            public AvoidSide Side { get; }
            public AvoidTarget(Avoid avoid, Vector3 point, AvoidSide side)
            { Avoid = avoid; Point = point; Side = side; }
        }
    }

    // Class252/253/254 used by Helpers but not AvoidCluster
    internal class Class252<T1, T2>
    {
        public T1 avoid;
        public T2 trace;
        public Class252(T1 avoid, T2 trace) { this.avoid = avoid; this.trace = trace; }
    }

    internal class Class253<T1, T2>
    {
        public T1 cluster;
        public T2 nearestHit;
        public Class253(T1 cluster, T2 nearestHit) { this.cluster = cluster; this.nearestHit = nearestHit; }
    }

    internal class Class254<T1, T2>
    {
        public T1 Cluster;
        public T2 relativePoint;
        public Class254(T1 cluster, T2 relativePoint) { this.Cluster = cluster; this.relativePoint = relativePoint; }
    }
}