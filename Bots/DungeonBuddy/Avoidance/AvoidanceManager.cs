using System;
using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Bots.DungeonBuddy.Avoidance
{
    public static class AvoidanceManager
    {
        public static readonly List<AvoidInfo> AvoidInfos = new();
        public static readonly List<Avoid> Avoids = new();
        public static readonly List<AvoidCluster> AvoidClusters = new();

        public static void Add(AvoidInfo avoid)
        {
            AvoidInfos.Add(avoid);
        }

        public static void AddRange(IEnumerable<AvoidInfo> avoids)
        {
            AvoidInfos.AddRange(avoids);
        }

        public static void Remove(AvoidInfo avoid)
        {
            AvoidInfos.Remove(avoid);
        }

        public static void RemoveAll(Predicate<AvoidInfo> match)
        {
            AvoidInfos.RemoveAll(match);
        }

        // HB: ClearAvoidPath lives in Helpers.cs, not here.
        // HB AvoidanceManager.Clear() only clears AvoidInfos.
        public static void Clear()
        {
            AvoidInfos.Clear();
        }

        public static void ClearAvoidPath()
        {
            Helpers.ClearAvoidPath();
        }

        // HB smethod_0
        internal static void Update()
        {
            bool changed = Avoids.RemoveAll(a => !a.IsValid) > 0;

            foreach (var obj in ObjectManager.ObjectList)
            {
                foreach (var ai in AvoidInfos.Where(ai => ai.ObjectSelector != null))
                {
                    // HB 6.2.3 Pulse(): uniqueness is (AvoidInfo, WoWObject) pair — allows N AvoidObjects
                    // per AvoidInfo, one per distinct world object (e.g. every forge in range = one entry).
                    if (ai.ObjectSelector(obj) && ai.CanRun(obj) && !Avoids.Any(a => a is AvoidObject ao && ao.AvoidInfo == ai && ao.Object == obj))
                    {
                        changed = true;
                        Avoids.Add(new AvoidObject(ai, obj));
                    }
                }
            }

            foreach (var ai in AvoidInfos.Where(ai => ai.LocationSelector != null))
            {
                if (ai.CanRun(null) && !Avoids.Any(a => a is AvoidLocation al && al.AvoidInfo == ai))
                {
                    changed = true;
                    Avoids.Add(new AvoidLocation(ai));
                }
            }

            foreach (var avoid in Avoids)
            {
                var oldLoc = avoid.Location;
                avoid.vmethod_0();
                if (oldLoc != avoid.Location && !changed)
                    changed = true;
            }

            if (changed)
                Helpers.BuildClusters(Avoids, AvoidClusters);
        }

        // Compatibility wrappers used by current DungeonBuddy/ScriptHelpers call sites.
        public static bool IsInAvoidance(WoWPoint location)
        {
            return Avoids.Any(a => a.IsPointInAvoid(location));
        }

        public static WoWPoint GetSafePoint(WoWPoint from, float minDistance = 10f)
        {
            var nearestAvoid = Avoids
                .Where(a => a.Location.DistanceSqr(from) < (a.Radius + 20f) * (a.Radius + 20f))
                .OrderBy(a => a.Location.DistanceSqr(from))
                .FirstOrDefault();

            if (nearestAvoid == null)
                return from;

            var directionAway = from - nearestAvoid.Location;
            directionAway.Normalize();
            var safePoint = from + directionAway * (nearestAvoid.Radius + minDistance);

            if (Navigator.CanNavigateFully(from, safePoint) && !IsInAvoidance(safePoint))
                return safePoint;

            return from;
        }
    }
}
