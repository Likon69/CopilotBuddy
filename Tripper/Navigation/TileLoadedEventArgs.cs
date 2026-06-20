using System;

namespace Tripper.Navigation
{
    /// <summary>
    /// Event arguments for tile-load notifications.
    /// TileLoaded uses MaNGOS ADT tile coordinates; OnSubTileLoaded uses Detour
    /// sub-tile coordinates produced by MeshMapCalculator.
    /// </summary>
    public class TileLoadedEventArgs : EventArgs
    {
        public TileLoadedEventArgs(uint mapId, int tileX, int tileY)
        {
            MapId = mapId;
            TileX = tileX;
            TileY = tileY;
        }

        public uint MapId { get; }
        public int TileX { get; }
        public int TileY { get; }
    }
}
