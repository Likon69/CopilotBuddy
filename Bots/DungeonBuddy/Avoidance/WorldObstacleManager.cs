// WorldObstacleManager.cs
// HB 6.2.3 pattern adaptation: geometric avoidance for world GameObjects whose M2
// models have nBoundingTriangles=0 and are therefore absent from navigation vmaps.
//
// Forge, mailbox, and similar objects are not baked into the navmesh —
// the pathfinder generates straight-through paths and the character gets stuck.
// This manager registers them as AvoidInfo objects so Helpers.GetAvoidPath()
// computes tangent-line detours around them via the DungeonBuddy avoidance algorithm.
//
// Call Initialize() from any bot's Start() to enable avoidance for that bot.
// Call Shutdown() from Stop() to release resources and unsubscribe from Navigator.

using System;
using Styx;
using Styx.Helpers;
using TreeSharp;

namespace Bots.DungeonBuddy.Avoidance
{
    public static class WorldObstacleManager
    {
        private static bool _initialized;

        /// <summary>
        /// Registers common world obstacle types and wires Navigator callbacks.
        /// Safe to call multiple times — re-registers cleanly on each bot start.
        /// HB 6.2.3: AvoidanceNavigationProvider is set as NavigationProvider in DungeonBuddy.Start().
        /// Here we use Navigator delegate injection (no NavigationProvider subclass needed).
        /// </summary>
        public static void Initialize()
        {
            // Clear previous registration so AvoidInfos aren't duplicated on bot restart
            Shutdown();

            RegisterObstacles();

            // Wire Navigator: AvoidanceManager.Update() called from WoWPulsator (step 3 of wiring).
            // Wire the waypoint provider: GetAvoidWaypoints() called by Navigator.MoveTo() per tick.
            Styx.Logic.Pathing.Navigator.NavAvoidanceUpdater = AvoidanceManager.Update;
            Styx.Logic.Pathing.Navigator.NavAvoidWaypointProvider = Helpers.GetAvoidWaypoints;

            _initialized = true;
            Logging.Write("[WorldObstacleManager] Geometric avoidance initialized for world navigation.");
        }

        /// <summary>
        /// Clears all registrations and unwires Navigator callbacks.
        /// Call from the bot's Stop() method.
        /// </summary>
        public static void Shutdown()
        {
            if (!_initialized) return;

            // Clear all avoidance state: AvoidInfos, per-tick Avoids, and their clusters.
            // AvoidanceManager.Clear() only clears AvoidInfos — explicitly clear the runtime
            // lists too so stale AvoidObjects don't accumulate across bot restarts.
            AvoidanceManager.Clear();
            AvoidanceManager.Avoids.Clear();
            AvoidanceManager.AvoidClusters.Clear();
            Helpers.ClearAvoidPath();

            Styx.Logic.Pathing.Navigator.NavAvoidanceUpdater = null;
            Styx.Logic.Pathing.Navigator.NavAvoidWaypointProvider = null;

            _initialized = false;
        }

        private static void RegisterObstacles()
        {
            // Always-active condition: world obstacles never turn off
            CanRunDecoratorDelegate always = ctx => true;

            // Forge / Anvil / SpellFocus objects (type 8 in WotLK DBC).
            // These include crafting forges, blacksmith anvils, inscribers' desks, etc.
            // Radius 2.5f covers large forge models in Orgrimmar and Ironforge.
            AvoidanceManager.Add(new AvoidInfo(
                always,
                obj =>
                {
                    var go = obj as Styx.WoWInternals.WoWObjects.WoWGameObject;
                    return go != null && go.IsValid &&
                           go.SubType == Styx.WoWGameObjectType.SpellFocus;
                },
                () => 2.5f));

            // Mailbox (type 19). Every city mailbox blocks the path at close range.
            AvoidanceManager.Add(new AvoidInfo(
                always,
                obj =>
                {
                    var go = obj as Styx.WoWInternals.WoWObjects.WoWGameObject;
                    return go != null && go.IsValid &&
                           go.SubType == Styx.WoWGameObjectType.Mailbox;
                },
                () => 1.5f));

            // Trap (type 6): campfires, player-placed fires, small physical obstacles.
            // DisplayId != 0 filters out invisible spell area triggers that also use type Trap
            // (quest triggers, rune of power, etc.) — those have no collision model.
            // Radius 1.5f matches a campfire footprint; large enough to detour around,
            // small enough not to cause routing issues in open areas.
            AvoidanceManager.Add(new AvoidInfo(
                always,
                obj =>
                {
                    var go = obj as Styx.WoWInternals.WoWObjects.WoWGameObject;
                    return go != null && go.IsValid &&
                           go.SubType == Styx.WoWGameObjectType.Trap &&
                           go.DisplayId != 0;
                },
                () => 1.5f));
        }
    }
}
