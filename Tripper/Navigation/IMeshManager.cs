using System;
using System.Numerics;

namespace Tripper.Navigation
{
    /// <summary>
    /// Compatibility interface matching HB 6.2.3 PathFindResult.Manager contract.
    /// CopilotBuddy uses a consolidated navigator, so these pointers are optional.
    /// </summary>
    public interface IMeshManager
    {
        IntPtr Mesh { get; }

        IntPtr MeshQuery { get; }

        PathFindResult FindPath(Vector3 start, Vector3 end);

        bool LoadTile(TileIdentifier tid);

        bool UnloadTile(TileIdentifier tid);
    }
}
