using System;
using System.Numerics;

namespace Tripper.Navigation
{
    /// <summary>
    /// Identifies a MaNGOS ADT navigation tile using the same X/Y coordinates as
    /// the .mmtile file names and Navigation.dll's WorldToTile helper.
    /// V5 .mmtile files may contain a 4x4 grid of Detour sub-tiles inside one ADT.
    /// </summary>
    public struct TileIdentifier : IEquatable<TileIdentifier>
    {
        /// <summary>MaNGOS ADT tile X coordinate (0-63 for standard WoW maps).</summary>
        public int X { get; }

        /// <summary>MaNGOS ADT tile Y coordinate (0-63 for standard WoW maps).</summary>
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
        /// Gets the MaNGOS ADT tile identifier for a given world position.
        /// Matches Navigation::WorldToTile: tileX = (origin - worldX) / tileSize,
        /// tileY = (origin - worldY) / tileSize, with C-style integer truncation.
        /// </summary>
        /// <param name="x">World X position.</param>
        /// <param name="y">World Y position.</param>
        /// <returns>TileIdentifier for the position.</returns>
        public static TileIdentifier GetByPosition(float x, float y)
        {
            const float tileSize = 533.3333f;
            const float gridOrigin = 32.0f * tileSize;
            int tileX = (int)((gridOrigin - x) / tileSize);
            int tileY = (int)((gridOrigin - y) / tileSize);
            return new TileIdentifier(tileX, tileY);
        }

        public static TileIdentifier GetByPosition(Vector3 position)
        {
            return GetByPosition(position.X, position.Y);
        }

        public static TileIdentifier GetByPosition(ref Vector3 position)
        {
            return GetByPosition(position.X, position.Y);
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
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        /// <summary>
        /// Returns a string representation of this tile identifier.
        /// </summary>
        public override string ToString()
        {
            return $"Tile({X}, {Y})";
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
