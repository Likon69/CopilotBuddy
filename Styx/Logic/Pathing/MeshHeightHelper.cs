// Type: Styx.Logic.Pathing.MeshHeightHelper
// Purpose: Find mesh height at a given X,Y position for navigation

using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable disable
namespace Styx.Logic.Pathing;

/// <summary>
/// Helper class to find the navigation mesh height at a given position.
/// Used by quest objectives to find valid Z coordinates for navigation.
/// </summary>
public static class MeshHeightHelper
{
    /// <summary>
    /// Finds the mesh height at the given position, searching nearby if not found directly.
    /// </summary>
    /// <param name="position">The position to find height for. Z will be modified if found.</param>
    /// <returns>True if a valid height was found, false otherwise.</returns>
    public static bool FindMeshHeight(ref Vector3 position)
    {
        Vector3 playerLocation = ToVector3(ObjectManager.Me.Location);
        List<float> heights = Navigator.FindHeights(position.X, position.Y);
        
        float foundHeight;
        if (!FindClosestHeight(playerLocation, position, heights, out foundHeight))
        {
            // Try searching in a line towards the player
            Vector3 direction = Vector3.Normalize(playerLocation - position);
            for (float distance = 1f; distance <= 10.0f; distance++)
            {
                Vector3 searchPos = position + (direction * distance);
                List<float> searchHeights = Navigator.FindHeights(searchPos.X, searchPos.Y);
                if (FindClosestHeight(playerLocation, searchPos, searchHeights, out foundHeight))
                {
                    position = searchPos;
                    position.Z = foundHeight;
                    return true;
                }
            }
            
            // Try a grid search around the position
            for (float offsetX = -10f; offsetX <= 10.0f; offsetX += 2.5f)
            {
                for (float offsetY = -10f; offsetY <= 10.0f; offsetY += 2.5f)
                {
                    Vector3 searchPos = new Vector3(position.X + offsetX, position.Y + offsetY, 0.0f);
                    List<float> searchHeights = Navigator.FindHeights(searchPos.X, searchPos.Y);
                    if (FindClosestHeight(playerLocation, searchPos, searchHeights, out foundHeight))
                    {
                        position = searchPos;
                        position.Z = foundHeight;
                        return true;
                    }
                }
            }
            return false;
        }
        
        position.Z = foundHeight;
        return true;
    }

    /// <summary>
    /// Finds the closest height to the player's Z position from a list of heights.
    /// </summary>
    private static bool FindClosestHeight(
        Vector3 playerPosition,
        Vector3 targetPosition,
        IList<float> heights,
        out float foundHeight)
    {
        foundHeight = 0.0f;
        if (heights.Count <= 0)
            return false;
            
        float closestDiff = float.MaxValue;
        for (int i = 0; i < heights.Count; i++)
        {
            float diff = Math.Abs(playerPosition.Z - heights[i]);
            if (diff < closestDiff)
            {
                closestDiff = diff;
                foundHeight = heights[i];
            }
        }
        return true;
    }

    /// <summary>
    /// Converts a WoWPoint to a Vector3.
    /// </summary>
    private static Vector3 ToVector3(WoWPoint point)
    {
        return new Vector3(point.X, point.Y, point.Z);
    }

    /// <summary>
    /// Converts a Vector3 to a WoWPoint.
    /// </summary>
    public static WoWPoint ToWoWPoint(Vector3 vector)
    {
        return new WoWPoint(vector.X, vector.Y, vector.Z);
    }
}
