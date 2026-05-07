namespace Bots.DungeonBuddy.Avoidance
{
    /// <summary>
    /// HB 6.2.3 AvoidancePriority — weights the urgency of an avoid zone.
    /// Used by GetBestLocationOutsideCluster to bias the escape route scoring:
    /// high-priority avoids (e.g. instant-kill fire) dominate the score so the
    /// bot exits through them first, even if the path is longer.
    /// Values match HB 6.2.3 exactly: Low=1, Medium=10, High=100.
    /// </summary>
    public enum AvoidancePriority
    {
        Low    = 1,
        Medium = 10,
        High   = 100
    }
}
