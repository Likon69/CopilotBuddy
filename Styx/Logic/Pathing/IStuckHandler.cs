using System;

namespace Styx.Logic.Pathing
{
    /// <summary>
    /// Interface for handling stuck detection and recovery.
    /// </summary>
    public interface IStuckHandler
    {
        /// <summary>
        /// Checks if the player is currently stuck.
        /// </summary>
        /// <returns>True if stuck, false otherwise.</returns>
        bool IsStuck();

        /// <summary>
        /// Attempts to unstick the player.
        /// </summary>
        void Unstick();

        /// <summary>
        /// Resets the stuck handler state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Called when this stuck handler becomes the active navigation stuck handler.
        /// </summary>
        void OnSetAsCurrent();

        /// <summary>
        /// Called when this stuck handler is no longer the active navigation stuck handler.
        /// </summary>
        void OnRemoveAsCurrent();
    }
}
