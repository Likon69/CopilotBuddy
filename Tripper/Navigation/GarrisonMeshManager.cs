using System;
using System.Numerics;

namespace Tripper.Navigation
{
    // HB 6.2.3 Tripper/Navigation/GarrisonMeshManager.cs
    // HB version handles WoD Garrison sub-areas (Alliance/Horde partial path locations, etc.)
    // WotLK 3.3.5a has no garrisons. Stub: always unloaded.
    public sealed class GarrisonMeshManager : IMeshManager, IDisposable
    {
        internal GarrisonMeshManager(Navigator navigator)
        {
        }

        public bool IsLoaded => false;

        public IntPtr Mesh => IntPtr.Zero;

        public IntPtr MeshQuery => IntPtr.Zero;

        public PathFindResult FindPath(Vector3 start, Vector3 end)
        {
            return PathFindResult.CreateFailed(PathFindStep.InitPathFind);
        }

        public bool LoadTile(TileIdentifier tid)
        {
            return false;
        }

        public bool UnloadTile(TileIdentifier tid)
        {
            return false;
        }

        public void Dispose()
        {
        }
    }
}
