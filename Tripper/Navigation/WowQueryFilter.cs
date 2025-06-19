using System;
using System.Collections.Generic;

namespace Tripper.Navigation
{
    // HB 6.2.3 Tripper/Navigation/WowQueryFilter.cs
    // HB wraps Tripper.RecastManaged.Detour.QueryFilter; CB uses managed QueryFilter dictionary.
    public class WowQueryFilter : IDisposable
    {
        public WowQueryFilter()
        {
            AreaCosts = new Dictionary<AreaType, float>();
        }

        public WowQueryFilter(AbilityFlags includeFlags, AbilityFlags excludeFlags)
            : this()
        {
            IncludeFlags = includeFlags;
            ExcludeFlags = excludeFlags;
        }

        public AbilityFlags IncludeFlags { get; set; } = AbilityFlags.All;

        public AbilityFlags ExcludeFlags { get; set; } = AbilityFlags.Unwalkable | AbilityFlags.Transport;

        public Dictionary<AreaType, float> AreaCosts { get; set; }

        public void SetAreaCost(AreaType area, float cost)
        {
            if (cost < 1f)
                throw new ArgumentOutOfRangeException(nameof(cost), "Cost must be above or equal to 1");
            AreaCosts[area] = cost;
        }

        public float GetAreaCost(AreaType area)
        {
            return AreaCosts.TryGetValue(area, out float cost) ? cost : 1f;
        }

        public void Dispose()
        {
        }
    }
}
