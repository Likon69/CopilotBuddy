using System;

namespace TreeSharp
{
    public static class CompositeExtensions
    {
        public static bool IsRunning(this Composite? c)
        {
            return c != null && c.LastStatus.HasValue && c.LastStatus.Value == RunStatus.Running;
        }
    }
}
