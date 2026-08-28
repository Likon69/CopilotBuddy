// Rewritten without BoundingBox2 dependency — uses inline range check.

using System;
using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Tripper.Tools.Math;
using Vector2 = System.Numerics.Vector2;

namespace Bots.BGBuddy
{
    /// <summary>
    /// Utility for counting players within MapBox regions and raid management.
    /// </summary>
    public class RaidHelper
    {
        /// <summary>
        /// Promotes a random raid member to leader if we are currently the group leader.
        /// </summary>
        public static bool SetNewLeader()
        {
            if (!StyxWoW.Me.IsGroupLeader)
                return false;

            int numRaidMembers = StyxWoW.Me.NumRaidMembers;
            var random = new Random(Environment.TickCount);
            int index = random.Next(0, numRaidMembers);
            Lua.DoString("PromoteToLeader(\"" + Lua.Escape(StyxWoW.Me.RaidMembers[index].Name) + "\")");
            return true;
        }

        /// <summary>
        /// Converts a world position to minimap coordinates within a given area.
        /// </summary>
        public Vector2 GetMapLocation(Vector2 areaA, Vector2 areaB, Vector2 worldPoint)
        {
            return new Vector2(
                (worldPoint.Y - areaA.Y) / (areaB.Y - areaA.Y),
                (worldPoint.X - areaA.X) / (areaB.X - areaA.X));
        }

        /// <summary>
        /// Counts how many 2D positions fall within the bounding box defined by topLeft and bottomRight.
        /// </summary>
        public static int GetCountWithin(Vector3 topLeft, Vector3 bottomRight, IEnumerable<Vector2> playerPositions)
        {
            // Inline bounding box containment instead of BoundingBox2
            float minX = Math.Min(topLeft.X, bottomRight.X);
            float maxX = Math.Max(topLeft.X, bottomRight.X);
            float minY = Math.Min(topLeft.Y, bottomRight.Y);
            float maxY = Math.Max(topLeft.Y, bottomRight.Y);

            return playerPositions.Count(p => p.X >= minX && p.X <= maxX && p.Y >= minY && p.Y <= maxY);
        }

        /// <summary>
        /// Counts enemy players within the given MapBox area.
        /// </summary>
        public static int CountEnemyPlayersInBox(MapBox box)
        {
            bool isHorde = StyxWoW.Me.IsHorde;

            // Get enemy players that are alive
            var enemyPlayers = ObjectManager.GetObjectsOfType<WoWPlayer>(false, false)
                .Where(p => p.IsHorde != isHorde && p.IsAlive);

            var positions = enemyPlayers.Select(p => new Vector2(p.X, p.Y));
            return GetCountWithin(box.TopLeft, box.BottomRight, positions);
        }

        /// <summary>
        /// Counts friendly (alive, non-ghost) raid members within the given MapBox area.
        /// </summary>
        public static int CountFriendlyPlayersInBox(MapBox box)
        {
            var aliveMembers = StyxWoW.Me.RaidMemberInfos
                .Where(m => !m.Dead && !m.Ghost);

            var positions = aliveMembers
                .Select(m => m.Location)
                .Select(loc => new Vector2(loc.X, loc.Y))
                .ToList();

            return GetCountWithin(box.TopLeft, box.BottomRight, positions);
        }
    }
}
