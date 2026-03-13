#nullable disable
using System;
using Styx.Logic.Pathing;

namespace Styx.Logic
{
    /// <summary>
    /// Event arguments for mount up events (HB 4.3.4 compatibility).
    /// </summary>
    public class MountUpEventArgs : EventArgs
    {
        /// <summary>
        /// Gets whether the mount is a flying mount.
        /// </summary>
        public bool IsFlying { get; set; }

        /// <summary>
        /// Gets the name of the mount.
        /// </summary>
        public string MountName { get; set; }

        /// <summary>
        /// The destination the bot is traveling to.
        /// </summary>
        public WoWPoint Destination { get; set; }

        /// <summary>
        /// Set to true to cancel the mount-up attempt (e.g. during elevator ride).
        /// HB 6.2.3 pattern: MeshNavigator.method_17 cancels mount while riding transport.
        /// </summary>
        public bool Cancel { get; set; }

        public MountUpEventArgs()
        {
            IsFlying = false;
            MountName = string.Empty;
            Destination = WoWPoint.Empty;
        }

        public MountUpEventArgs(bool isFlying, string mountName)
        {
            IsFlying = isFlying;
            MountName = mountName ?? string.Empty;
            Destination = WoWPoint.Empty;
        }
    }
}
