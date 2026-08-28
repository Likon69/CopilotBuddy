// PolyNav.cs — Ported from HB 6.2.3 Styx/Pathing/FlightorNavigation/PolyNav.cs
// 2D polygon-aware pathfinder using a visibility graph + A*.
//
// Usage:
//   var nav = new PolyNav(continentBoundary, aerialBlackspots);
//   Vector2[] waypoints = nav.FindPath(from2D, to2D);

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Styx.Common;
using Tripper.XNAMath;

namespace Styx.Logic.Pathing.FlightorNavigation
{
    /// <summary>
    /// A node in the visibility graph. Holds a world-space 2D position and an
    /// adjacency list of indices into the graph's node collection.
    /// </summary>
    internal sealed class GraphNode
    {
        public readonly int    Index;
        public readonly Vector2 Position;
        public readonly List<int> Neighbors = new List<int>();

        public GraphNode(int index, Vector2 position)
        {
            Index    = index;
            Position = position;
        }
    }

    /// <summary>
    /// Builds and maintains a visibility graph from a set of polygon boundary
    /// vertices and hole vertices.  Temp nodes (start/end of each path query)
    /// are appended, used, then removed after each query.
    /// </summary>
    internal sealed class VisibilityGraph
    {
        private readonly List<GraphNode> _nodes = new List<GraphNode>();

        // ── Node registration ──────────────────────────────────────────────────

        /// <summary>
        /// Registers a new node at <paramref name="position"/> and returns it.
        /// <paramref name="initialNeighbors"/> are added as outgoing edges FROM the new node.
        /// (method_6 in WoD source)
        /// </summary>
        public GraphNode AddNode(Vector2 position, GraphNode[] initialNeighbors)
        {
            var node = new GraphNode(_nodes.Count, position);
            foreach (var neighbor in initialNeighbors)
                node.Neighbors.Add(neighbor.Index);
            _nodes.Add(node);
            return node;
        }

        /// <summary>
        /// Adds a directed edge from → to.
        /// (method_7 in WoD source)
        /// </summary>
        public void AddEdge(GraphNode from, GraphNode to)
        {
            if (!from.Neighbors.Contains(to.Index))
                from.Neighbors.Add(to.Index);
        }

        /// <summary>Get node by index. (method_5 in WoD source)</summary>
        public GraphNode GetNode(int index) => _nodes[index];

        /// <summary>Remove a (temp) node. (method_0 in WoD source)</summary>
        public void RemoveNode(GraphNode node) => _nodes.Remove(node);

        /// <summary>
        /// Compact the node list after removing temp nodes:
        /// purge any neighbor indices that are now out of range.
        /// (method_1 in WoD source)
        /// </summary>
        public void Rebuild()
        {
            int count = _nodes.Count;
            foreach (var node in _nodes)
                node.Neighbors.RemoveAll(idx => idx < 0 || idx >= count);
        }

        /// <summary>Snapshot of all nodes for A*. (method_2 in WoD source)</summary>
        public IList<GraphNode> Snapshot() => _nodes.ToList();

        /// <summary>Live enumeration of all nodes. (ReadOnlyCollection_0 in WoD source)</summary>
        public IReadOnlyList<GraphNode> All => _nodes;
    }

    /// <summary>
    /// A* pathfinder over a visibility graph.
    /// Takes a snapshot of the graph's nodes at construction time so that
    /// transient start/end nodes added during a query don't interfere with
    /// subsequent queries.
    /// </summary>
    internal sealed class AStarSolver
    {
        private readonly IList<GraphNode> _nodes;

        /// <param name="nodes">Snapshot of all graph nodes (including temp start/end nodes).</param>
        /// <param name="bucketSize">Ignored — historical parameter from WoD bucket-queue implementation.</param>
        public AStarSolver(IList<GraphNode> nodes, int bucketSize)
        {
            _nodes = nodes;
        }

        /// <summary>Get node by index. (method_0 in WoD source)</summary>
        public GraphNode GetNode(int index) => _nodes[index];

        /// <summary>
        /// Run A* from <paramref name="startIndex"/> to <paramref name="endIndex"/>.
        /// Fills <paramref name="result"/> with the sequence of node indices
        /// forming the shortest path (start inclusive, end inclusive).
        /// (method_2 in WoD source)
        /// </summary>
        public void FindPath(int startIndex, int endIndex, List<int> result)
        {
            result.Clear(); // WoD Class1069.method_2: always start clean
            if (startIndex == endIndex)
            {
                result.Add(startIndex);
                return;
            }

            int n = _nodes.Count;
            var gCost  = new float[n];
            var parent = new int[n];
            var closed = new bool[n];
            for (int i = 0; i < n; i++) { gCost[i] = float.MaxValue; parent[i] = -1; }
            gCost[startIndex] = 0f;

            // WoD Class1069.method_2: track best-approx node (lowest heuristic to end seen so far)
            int   bestApprox = startIndex;
            float bestH      = Heuristic(startIndex, endIndex);

            // .NET priority queue: (priority=f, nodeIndex)
            var open = new PriorityQueue<int, float>();
            open.Enqueue(startIndex, bestH);

            while (open.Count > 0)
            {
                int cur = open.Dequeue();

                if (cur == endIndex)
                {
                    // Reconstruct path
                    for (int id = endIndex; id != -1; id = parent[id])
                        result.Insert(0, id);
                    return;
                }

                if (closed[cur]) continue;
                closed[cur] = true;

                GraphNode curNode = _nodes[cur];
                foreach (int neighborIdx in curNode.Neighbors)
                {
                    if (closed[neighborIdx]) continue;
                    GraphNode neighborNode = _nodes[neighborIdx];
                    Vector2 curPos      = curNode.Position;
                    Vector2 neighborPos = neighborNode.Position;
                    float edgeCost = Vector2.Distance(ref curPos, ref neighborPos);
                    float newG = gCost[cur] + edgeCost;
                    if (newG < gCost[neighborIdx])
                    {
                        gCost[neighborIdx]  = newG;
                        parent[neighborIdx] = cur;
                        float h = Heuristic(neighborIdx, endIndex);
                        open.Enqueue(neighborIdx, newG + h);
                        // Update best-approx if this neighbor is closer to the end (WoD Class1069.method_2)
                        if (h < bestH) { bestH = h; bestApprox = neighborIdx; }
                    }
                }
            }
            // Endpoint not reached — fall back to best-approx node (WoD Class1069.method_2 fallback)
            for (int id = bestApprox; id != -1; id = parent[id])
                result.Insert(0, id);
        }

        private float Heuristic(int fromIndex, int toIndex)
        {
            Vector2 a = _nodes[fromIndex].Position;
            Vector2 b = _nodes[toIndex].Position;
            return Vector2.Distance(ref a, ref b) * 0.999f; // WoD Class1069.method_2: slight under-estimate for tie-breaking
        }
    }

    /// <summary>
    /// 2D polygon-aware path navigator.
    /// Given a continent boundary polygon and a set of hole polygons (aerial blackspots),
    /// routes a path between two points using a visibility graph and A*.
    /// Ported from HB 6.2.3 PolyNav.
    /// </summary>
    public class PolyNav
    {
        /// <summary>Continent boundary polygon (must be convex or simple).</summary>
        public Vector2[] Points { get; set; }

        /// <summary>Hole polygons (aerial blackspots) that paths must route around.</summary>
        public Vector2[][] Holes { get; set; }

        // Combined boundary: outer polygon + all holes, with Vector2.Zero separators,
        // used by ContainsPoint for a single ray-cast test.
        private readonly Vector2[] _combined;

        // The permanent visibility graph (polygon boundary + hole vertices).
        private readonly VisibilityGraph _graph;

        public PolyNav(Vector2[] points, IEnumerable<Vector2[]> holes)
        {
            Points = points;
            Holes  = holes.ToArray();

            // Build the combined polygon array for the ContainsPoint fast-path:
            // [ Zero, ...outer, outer[0], Zero, ...hole1, hole1[0], Zero, ... ]
            var combined = new List<Vector2>();
            combined.Add(Vector2.Zero);
            combined.AddRange(points);
            combined.Add(points[0]);
            combined.Add(Vector2.Zero);
            foreach (var hole in Holes)
            {
                if (hole.Length > 0)
                {
                    combined.AddRange(hole);
                    combined.Add(hole[0]);
                    combined.Add(Vector2.Zero);
                }
            }
            _combined = combined.ToArray();

            // Build the permanent visibility graph
            _graph = new VisibilityGraph();
            BuildGraph(_graph, Points);
            foreach (var hole in Holes)
                BuildGraph(_graph, hole);
        }

        // ── Public API ─────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the neighbour positions of the graph node closest to <paramref name="point"/>.
        /// Sets <paramref name="closestPoint"/> to that node's exact position.
        /// (method GetConnections in WoD PolyNav source)
        /// </summary>
        public IEnumerable<Vector2> GetConnections(Vector2 point, out Vector2 closestPoint)
        {
            float bestDistSqr = float.MaxValue;
            GraphNode? closest = null;
            foreach (var node in _graph.All)
            {
                Vector2 pos = node.Position;
                float distSqr = Vector2.DistanceSqr(ref point, ref pos);
                if (distSqr < bestDistSqr)
                {
                    bestDistSqr = distSqr;
                    closest = node;
                }
            }
            if (closest == null)
            {
                closestPoint = Vector2.Zero;
                return System.Linq.Enumerable.Empty<Vector2>();
            }
            closestPoint = closest.Position;
            return closest.Neighbors.Select(idx => _graph.GetNode(idx).Position);
        }

        /// <summary>Returns true if the line segment from <paramref name="start"/> to
        /// <paramref name="end"/> lies entirely within the navigable polygon
        /// (inside the outer boundary and not crossing any hole).
        /// </summary>
        public bool ContainsLine(Vector2 start, Vector2 end)
        {
            // Midpoint must be inside
            if (!ContainsPoint((start + end) / 2f))
                return false;
            // Must not cross outer boundary
            if (SegmentCrossesPolygon(start, end, Points))
                return false;
            // Must not cross any hole
            foreach (var hole in Holes)
            {
                if (SegmentCrossesPolygon(start, end, hole))
                    return false;
            }
            return true;
        }

        /// <summary>Returns true if <paramref name="point"/> is inside the navigable area.</summary>
        public bool ContainsPoint(Vector2 point) => PointInPolygon(point, _combined);

        /// <summary>
        /// Find a 2D path from <paramref name="start"/> to <paramref name="end"/>.
        /// Returns an empty array if either point is outside the navigable area.
        /// Returns {start, end} if a straight line is clear.
        /// Otherwise returns visibility-graph waypoints via A*.
        /// </summary>
        public Vector2[] FindPath(Vector2 start, Vector2 end)
        {
            if (!ContainsPoint(start) || !ContainsPoint(end))
                return Array.Empty<Vector2>();

            if (ContainsLine(start, end))
                return new[] { start, end };

            // Add temp start node and connect it to all visible permanent nodes
            var startNode = _graph.AddNode(start, Array.Empty<GraphNode>());
            foreach (var node in _graph.All)
            {
                if (node.Index == startNode.Index) continue;
                if (ContainsLine(startNode.Position, Nudge(startNode.Position, node.Position)))
                    _graph.AddEdge(startNode, node);
            }

            // Add temp end node and connect all visible permanent nodes to it
            // WoD: test visibility FROM endNode TOWARD each other node (nudge from endNode side)
            var endNode = _graph.AddNode(end, Array.Empty<GraphNode>());
            foreach (var node in _graph.All)
            {
                if (node.Index == endNode.Index) continue;
                if (ContainsLine(endNode.Position, Nudge(endNode.Position, node.Position)))
                    _graph.AddEdge(node, endNode);
            }

            // A* on snapshot (includes temp nodes)
            var solver = new AStarSolver(_graph.Snapshot(), 8192);
            var pathIndices = new List<int>();
            solver.FindPath(startNode.Index, endNode.Index, pathIndices);
            Vector2[] result = pathIndices.Select(i => solver.GetNode(i).Position).ToArray();

            // Remove temp nodes and rebuild
            _graph.RemoveNode(startNode);
            _graph.RemoveNode(endNode);
            _graph.Rebuild();

            return result;
        }

        /// <summary>Compute the bounding box of the continent polygon.</summary>
        public void GetBounds(out Vector2 min, out Vector2 max)
        {
            min = new Vector2(float.MaxValue, float.MaxValue);
            max = new Vector2(float.MinValue, float.MinValue);
            foreach (var v in Points)
            {
                min.X = Math.Min(v.X, min.X);
                min.Y = Math.Min(v.Y, min.Y);
                max.X = Math.Max(v.X, max.X);
                max.Y = Math.Max(v.Y, max.Y);
            }
        }

        // ── Private helpers ────────────────────────────────────────────────────

        /// <summary>
        /// Build the polygon boundary chain in the visibility graph.
        /// Adds each vertex as a node, links consecutive vertices as directed
        /// edges, closes the loop, then builds cross-visibility edges.
        /// (method_2 in WoD PolyNav source)
        /// </summary>
        private void BuildGraph(VisibilityGraph graph, Vector2[] polygon)
        {
            if (polygon.Length == 0) return;

            // Phase 1: add all polygon vertices as a chain
            var first = graph.AddNode(polygon[0], Array.Empty<GraphNode>());
            var prev  = first;
            for (int i = 1; i < polygon.Length; i++)
            {
                var cur = graph.AddNode(polygon[i], new[] { prev }); // backward edge cur→prev
                graph.AddEdge(prev, cur);                             // forward edge prev→cur
                prev = cur;
            }
            // Close the loop
            graph.AddEdge(prev,  first);
            graph.AddEdge(first, prev);

            // Phase 2: add visibility cross-edges between all polygon vertices
            for (int i = 0; i < polygon.Length; i++)
            {
                var a = graph.GetNode(first.Index + i);
                foreach (var b in graph.All)
                {
                    if (b.Index == a.Index) continue;
                    if (a.Neighbors.Contains(b.Index)) continue;
                    if (ContainsLine(Nudge(b.Position, a.Position), Nudge(a.Position, b.Position)))
                    {
                        graph.AddEdge(a, b);
                        graph.AddEdge(b, a);
                    }
                }
            }
        }

        /// <summary>
        /// Nudge a point 1% of the way toward another point.
        /// Prevents ContainsLine from testing the endpoint exactly on the boundary.
        /// (method_0 in WoD PolyNav source: return a + (b - a) * 0.99f)
        /// </summary>
        private static Vector2 Nudge(Vector2 from, Vector2 to)
            => from + (to - from) * 0.99f;

        /// <summary>
        /// Ray-casting point-in-polygon test.
        /// Works on the combined array (outer + holes with Zero separators).
        /// (smethod_0 in WoD PolyNav source)
        /// </summary>
        private static bool PointInPolygon(Vector2 point, Vector2[] polygon)
        {
            bool inside = false;
            int  j      = polygon.Length - 1;
            for (int i = 0; i < polygon.Length; i++)
            {
                double xi = polygon[i].X, yi = polygon[i].Y;
                double xj = polygon[j].X, yj = polygon[j].Y;
                if ((yi > point.Y) != (yj > point.Y) &&
                    point.X < (xj - xi) * ((double)point.Y - yi) / (yj - yi) + xi)
                    inside = !inside;
                j = i;
            }
            return inside;
        }

        /// <summary>
        /// Returns true if segment [a,b] crosses any edge of the polygon.
        /// (smethod_1 in WoD PolyNav source)
        /// </summary>
        private static bool SegmentCrossesPolygon(Vector2 a, Vector2 b, Vector2[] polygon)
        {
            int j = polygon.Length - 1;
            for (int i = 0; i < polygon.Length; i++)
            {
                if (DoLineSegmentsIntersect(a, b, polygon[j], polygon[i]))
                    return true;
                j = i;
            }
            return false;
        }

        /// <summary>
        /// Line-segment intersection test using cross-product (orientation) method.
        /// Wraps MathEx.DoLineSegmentsIntersect.
        /// </summary>
        private static bool DoLineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
            => Styx.Common.MathEx.DoLineSegmentsIntersect(
                p1.X, p1.Y, p2.X, p2.Y,
                p3.X, p3.Y, p4.X, p4.Y);
    }
}
