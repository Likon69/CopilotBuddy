using Styx.Helpers;

namespace Styx.CommonBot
{
    /// <summary>
    /// Fires overlay toast notifications on key game events (level up, player death).
    /// HB 6.2.3 pattern: static subscriber, initialized once via EnsureLoaded().
    /// </summary>
    internal static class ToastNotifier
    {
        static ToastNotifier()
        {
            BotEvents.Player.OnLevelUp += OnLevelUp;
            BotEvents.Player.OnPlayerDied += OnPlayerDied;
        }

        /// <summary>
        /// Called from GameStats static ctor to ensure this type is initialized before gameplay starts.
        /// </summary>
        internal static void EnsureLoaded() { }

        private static void OnLevelUp(BotEvents.Player.LevelUpEventArgs args)
        {
            if (!StyxWoW.IsInGame)
                return;

            // Guard: do NOT access StyxWoW.Overlay here — that getter lazily creates
            // and activates the overlay even when BuddyControlPanel is disabled.
            // IsOverlayActive checks _overlayManager != null without triggering creation.
            if (!StyxWoW.IsOverlayActive)
                return;

            StyxWoW.Overlay.AddToast($"Ding! You are now level {args.NewLevel}.", 7000);
            Logging.Write("Toast: level up to {0}.", args.NewLevel);
        }

        private static void OnPlayerDied()
        {
            if (!StyxWoW.IsInGame)
                return;

            if (!StyxWoW.IsOverlayActive)
                return;

            StyxWoW.Overlay.AddToast("You have died.", 5000);
            Logging.Write("Toast: player died.");
        }
    }
}
