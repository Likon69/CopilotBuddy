using System;
using System.Numerics;

namespace Tripper.Navigation
{
    /// <summary>
    /// Identifies a WoW ADT navigation tile using X/Y coordinates.
    ///
    /// One ADT tile = 533.333 yards. The 64×64 ADT grid has its origin at tile [32, 32]
    /// (world position 0, 0).
    ///
    /// With the V5 mmtile format (MMAP_MULTI_TILE_VERSION = 5), each ADT is stored as a
    /// 4×4 grid of 16 Detour sub-tiles (133.333 yards each). Navigation.dll handles the
    /// sub-tile decomposition internally. <see cref="MeshMapCalculator"/> converts between
    /// ADT tile coordinates and their 16 Detour sub-tile coordinates.
    ///
    /// <c>GetByPosition</c> always returns ADT-level coordinates, matching Navigation.dll's
    /// <c>WorldToTile_C</c> (floor((gridOrigin - world) / 533.333)).
    /// </summary>
    public struct TileIdentifier : IEquatable<TileIdentifier>
    {
        /// <summary>Tile X coordinate (0-63 for standard WoW maps).</summary>
        public int X { get; }

        /// <summary>Tile Y coordinate (0-63 for standard WoW maps).</summary>
        public int Y { get; }

        /// <summary>
        /// Initializes a new tile identifier with the specified coordinates.
        /// </summary>
        /// <param name="x">Tile X coordinate.</param>
        /// <param name="y">Tile Y coordinate.</param>
        public TileIdentifier(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets the tile identifier for a given world position.
        /// Matches Navigation.dll's 1:1 raw-tile WorldToTile conversion.
        /// </summary>
        public static TileIdentifier GetByPosition(Vector3 pos)
        {
            return GetByPosition(pos.X, pos.Y);
        }

        public static TileIdentifier GetByPosition(ref Vector3 pos)
        {
            return GetByPosition(pos.X, pos.Y);
        }

        public static TileIdentifier GetByPosition(Vector2 pos)
        {
            return GetByPosition(pos.X, pos.Y);
        }

        /// <summary>
        /// Gets the tile identifier for a given world position.
        /// Matches Navigation.cpp WorldToTile for 1x1 MaNGOS tiles.
        /// </summary>
        /// <param name="x">World X position.</param>
        /// <param name="y">World Y position.</param>
        /// <returns>TileIdentifier for the position.</returns>
        public static TileIdentifier GetByPosition(float x, float y)
        {
            const float gridOrigin = 32.0f * MapConsts.TileSize;
            int tileX = (int)((gridOrigin - x) / MapConsts.TileSize);
            int tileY = (int)((gridOrigin - y) / MapConsts.TileSize);
            return new TileIdentifier(tileX, tileY);
        }

        /// <summary>
        /// Checks equality between two tile identifiers.
        /// </summary>
        public bool Equals(TileIdentifier other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// Checks equality with another object.
        /// </summary>
        public override bool Equals(object? obj)
        {
            return obj is TileIdentifier other && Equals(other);
        }

        /// <summary>
        /// Gets the hash code for this tile identifier.
        /// </summary>
        public override int GetHashCode()
        {
            return Y * 64 + X;
        }

        /// <summary>
        /// Returns a string representation of this tile identifier.
        /// </summary>
        public override string ToString()
        {
            return string.Format("<{0}, {1}>", X, Y);
        }

        public static bool operator ==(TileIdentifier left, TileIdentifier right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TileIdentifier left, TileIdentifier right)
        {
            return !left.Equals(right);
        }
    }
}
