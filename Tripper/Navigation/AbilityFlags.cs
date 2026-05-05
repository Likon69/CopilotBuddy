using System;

namespace Tripper.Navigation
{
    /// <summary>
    /// Defines movement abilities and traversal capabilities for pathfinding.
    /// Used as flags in Detour query filters to determine valid navigation paths.
    /// </summary>
    [Flags]
    public enum AbilityFlags : ushort
    {
        /// <summary>No abilities specified.</summary>
        None = 0,

        /// <summary>Can walk/run on ground surfaces.</summary>
        Run = 1,

        /// <summary>Traversal is only valid while the character is alive.</summary>
        OnlyWhileAlive = 2,

        /// <summary>Can swim through water.</summary>
        Swim = 4,

        /// <summary>Can jump over obstacles or gaps.</summary>
        Jump = 8,

        /// <summary>Area is unwalkable - should be avoided.</summary>
        Unwalkable = 16,

        /// <summary>Can use teleport/portal mechanics.</summary>
        Teleport = 32,

        /// <summary>Can use transport vehicles (boats, zeppelins, elevators).</summary>
        Transport = 64,

        /// <summary>Horde faction access - can traverse Horde-only areas.</summary>
        Horde = 4096,

        /// <summary>Alliance faction access - can traverse Alliance-only areas.</summary>
        Alliance = 8192,

        /// <summary>Known building traversal area.</summary>
        KnownBuilding = 16384,

        /// <summary>All abilities combined - full traversal capabilities.</summary>
        All = 65535
    }
}
