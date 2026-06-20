using System;
using System.Numerics;

namespace Tripper.Navigation
{
    // HB 6.2.3 Tripper/Navigation/WorldMeshManager.cs
    // HB version manages streaming tiles for WoD sub-areas.
    // CB: backed by consolidated Navigator.
    public sealed class WorldMeshManager : IMeshManager, IDisposable
    {
        private readonly Navigator _navigator;

        private static Vector3 GetTileCenter(TileIdentifier tile)
        {
            float tileCenterX = (32.0f - tile.X - 0.5f) * MapConsts.TileSize;
            float tileCenterY = (32.0f - tile.Y - 0.5f) * MapConsts.TileSize;
            return new Vector3(tileCenterX, tileCenterY, 0f);
        }

        internal WorldMeshManager(Navigator navigator)
        {
            _navigator = navigator;
        }

        public bool IsLoaded => _navigator.IsLoaded;

        public TimeSpan GarbageCollectTime
        {
            get => _navigator.GarbageCollectTime;
            set => _navigator.GarbageCollectTime = value;
        }

        public event EventHandler<TileLoadedEventArgs>
            TileLoaded
        {
            add => _navigator.TileLoaded += value;
            remove => _navigator.TileLoaded -= value;
        }

        public event EventHandler<TileLoadedEventArgs>
            SubTileLoaded
        {
            add => _navigator.OnSubTileLoaded += value;
            remove => _navigator.OnSubTileLoaded -= value;
        }

        public IntPtr Mesh => IntPtr.Zero;

        public IntPtr MeshQuery => _navigator.GetNavMeshQueryPtr(_navigator.CurrentMapId);

        public PathFindResult FindPath(Vector3 start, Vector3 end)
        {
            return _navigator.FindPath(_navigator.CurrentMapId, start, end, true);
        }

        public bool LoadTile(TileIdentifier tid)
        {
            // Do NOT check CurrentMapId == 0 here — Eastern Kingdoms is map 0.
            // Guard on IsLoaded only; CurrentMapId == 0 is valid for EK.
            if (!_navigator.IsLoaded)
            {
                return false;
            }

            Vector3 tileCenter = GetTileCenter(tid);
            _navigator.EnsureTilesAroundPosition(_navigator.CurrentMapId, tileCenter, 0);
            return _navigator.IsTileLoaded(_navigator.CurrentMapId, tid.X, tid.Y);
        }

        public bool UnloadTile(TileIdentifier tid)
        {
            return _navigator.UnloadTile(_navigator.CurrentMapId, tid);
        }

        public void UnloadAllTiles()
        {
            _navigator.UnloadAllTiles();
        }

        public void Dispose()
        {
        }
    }
}
