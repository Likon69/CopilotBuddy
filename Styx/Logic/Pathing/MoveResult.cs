using System;

namespace Styx.Logic.Pathing
{
    // Order must match HB 4.3.4 so any numeric comparisons behave identically
    public enum MoveResult
    {
        Failed = 0,
        ReachedDestination = 1,
        PathGenerationFailed = 2,
        PathGenerated = 3,
        UnstuckAttempt = 4,
        Moved = 5
    }
}
