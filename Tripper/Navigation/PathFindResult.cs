using System;
using System.Numerics;

namespace Tripper.Navigation
{
    /// <summary>
    /// Represents the result of a pathfinding operation.
    /// Contains the calculated path points, polygon information, flags, and status.
    /// </summary>
    public class PathFindResult
    {
        /// <summary>
        /// Mesh manager that produced this result.
        /// HB 6.2.3 exposes this on PathFindResult for manager-level follow-up queries.
        /// </summary>
        public IMeshManager? Manager { get; internal set; }

        /// <summary>
        /// Time elapsed during pathfinding operation.
        /// </summary>
        public TimeSpan Elapsed { get; internal set; }

        /// <summary>
        /// Detour status of the pathfinding operation.
        /// </summary>
        public Status Status { get; internal set; }

        /// <summary>
        /// Array of polygon references for each path segment.
        /// Used for advanced path manipulation and validation.
        /// </summary>
        public PolygonReference[] Polygons { get; internal set; }

        /// <summary>
        /// Array of straight path flags for each path segment.
        /// Indicates segment type (Start, End, OffMeshConnection).
        /// </summary>
        public StraightPathFlags[] Flags { get; internal set; }

        /// <summary>
        /// Array of world positions representing the path from start to end.
        /// Empty if pathfinding failed.
        /// </summary>
        public Vector3[] Points { get; internal set; }

        /// <summary>
        /// Array of ability flags required for each path segment.
        /// Indicates movement capabilities needed (Run, Swim, Jump, etc.).
        /// </summary>
        public AbilityFlags[] AbilityFlags { get; internal set; }

        /// <summary>
        /// Array of area types for each path segment.
        /// Indicates terrain type (Ground, Water, Elevator, etc.).
        /// </summary>
        public AreaType[] PolyTypes { get; internal set; }

        /// <summary>
        /// Starting polygon reference.
        /// </summary>
        public PolygonReference StartPoly { get; internal set; }

        /// <summary>
        /// Ending polygon reference.
        /// </summary>
        public PolygonReference EndPoly { get; internal set; }

        /// <summary>
        /// Starting position (actual start point on navmesh).
        /// </summary>
        public Vector3 Start { get; internal set; }

        /// <summary>
        /// Ending position (actual end point on navmesh).
        /// </summary>
        public Vector3 End { get; internal set; }

        /// <summary>
        /// Indicates if pathfinding was aborted before completion.
        /// </summary>
        public bool Aborted { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the pathfinding operation succeeded.
        /// Based on Status.Succeeded property.
        /// </summary>
        public bool Succeeded => Status.Succeeded;

        /// <summary>
        /// Indicates if the path is incomplete.
        /// True when destination is unreachable but a partial path was found.
        /// </summary>
        public bool IsPartialPath { get; internal set; }

        /// <summary>
        /// If pathfinding failed, indicates the specific failure step.
        /// Only set when Status != Success.
        /// </summary>
        public PathFindStep FailStep { get; internal set; }

        /// <summary>
        /// Gets the number of points in the path.
        /// Returns 0 if pathfinding failed.
        /// </summary>
        public int PathLength => Points?.Length ?? 0;

        /// <summary>
        /// Initializes an empty PathFindResult (used for failed pathfinding).
        /// </summary>
        public PathFindResult()
        {
            Elapsed = TimeSpan.Zero;
            Status = Navigation.Status.Failure;
            Points = Array.Empty<Vector3>();
            Flags = Array.Empty<StraightPathFlags>();
            Polygons = Array.Empty<PolygonReference>();
            AbilityFlags = Array.Empty<AbilityFlags>();
            PolyTypes = Array.Empty<AreaType>();
            StartPoly = PolygonReference.Invalid;
            EndPoly = PolygonReference.Invalid;
            Start = Vector3.Zero;
            End = Vector3.Zero;
            Aborted = false;
            IsPartialPath = false;
            FailStep = PathFindStep.None;
            Manager = null;
        }

        /// <summary>
        /// Creates a failed PathFindResult with the specified error step.
        /// </summary>
        /// <param name="failStep">The specific failure reason.</param>
        /// <returns>A new PathFindResult indicating failure.</returns>
        public static PathFindResult CreateFailed(PathFindStep failStep)
        {
            return new PathFindResult
            {
                Status = Navigation.Status.Failure,
                FailStep = failStep,
                IsPartialPath = false
            };
        }

        /// <summary>
        /// Returns a string representation of this PathFindResult.
        /// </summary>
        public override string ToString()
        {
            return $"PathFindResult: {Status}, {PathLength} points{(IsPartialPath ? " (Partial)" : "")}";
        }
    }
}
