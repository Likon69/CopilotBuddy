namespace Tripper.Navigation
{
    /// <summary>
    /// Path post-processing mode for pathfinding results.
    /// Matches Honorbuddy's PathPostProcessing enum.
    /// </summary>
    public enum PathPostProcessing
    {
        /// <summary>
        /// No post-processing. Return raw path from Detour.
        /// </summary>
        None = 0,

        /// <summary>
        /// Move waypoints away from navmesh edges/walls.
        /// This prevents the character from walking too close to walls,
        /// stairs edges, or other terrain features.
        /// </summary>
        MoveAwayFromEdges = 1,

        /// <summary>
        /// Add randomization to path points for more human-like movement.
        /// Includes edge avoidance plus random offset.
        /// </summary>
        Randomize = 2
    }
}
