using System;
using System.Collections.Generic;
using System.IO;
using Styx;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.WoWInternals.WoWObjects;

namespace Bots.Gatherbuddy
{
    /// <summary>
    /// Manages tracking of harvested and blacklisted nodes.
    /// Prevents returning to already-harvested nodes before respawn.
    /// FEAT-40: Added persistent blacklist save/load.
    /// </summary>
    public static class NodeTracker
    {
        // Temporarily blacklisted nodes (guid → expiration)
        private static readonly Dictionary<ulong, DateTime> _blacklistedNodes = new();
        
        // Recently harvested nodes (approximate position → expiration)
        // Note: Using position because GUID changes after respawn
        private static readonly Dictionary<string, DateTime> _harvestedPositions = new();
        
        // FEAT-40: Persistent position blacklist (saved between sessions)
        private static readonly HashSet<string> _persistentBlacklist = new();
        
        // Estimated node respawn time (5-10 minutes in WotLK)
        private static readonly TimeSpan RespawnTime = TimeSpan.FromMinutes(5);

        private static string BlacklistFilePath =>
            Path.Combine(Logging.ApplicationPath, "Settings", $"GatherBlacklist_{StyxWoW.Me?.Name ?? "Unknown"}.txt");

        /// <summary>
        /// Blacklists a node for a given duration (failed harvest, ninja, etc.)
        /// </summary>
        public static void Blacklist(WoWGameObject node, TimeSpan duration)
        {
            if (node == null) return;
            _blacklistedNodes[node.Guid] = DateTime.Now + duration;
        }

        /// <summary>
        /// Blacklists a node using the default settings timer
        /// </summary>
        public static void Blacklist(WoWGameObject node)
        {
            Blacklist(node, TimeSpan.FromSeconds(GatherbuddySettings.Instance.BlacklistTimer));
        }

        /// <summary>
        /// Permanently blacklists a node position (persisted across sessions).
        /// </summary>
        public static void BlacklistPermanent(WoWGameObject node)
        {
            if (node == null) return;
            string posKey = GetPositionKey(node.Location);
            _persistentBlacklist.Add(posKey);
        }

        /// <summary>
        /// Marks a node as harvested (avoid returning before respawn)
        /// </summary>
        public static void MarkHarvested(WoWGameObject node)
        {
            if (node == null) return;
            
            // Key based on rounded position (nodes respawn at same location)
            string posKey = GetPositionKey(node.Location);
            _harvestedPositions[posKey] = DateTime.Now + RespawnTime;
        }

        /// <summary>
        /// Checks if a node is valid for harvesting
        /// </summary>
        public static bool IsNodeValid(WoWGameObject node)
        {
            if (node == null) return false;
            
            // Check blacklist by GUID
            if (_blacklistedNodes.TryGetValue(node.Guid, out var expiry) && DateTime.Now < expiry)
                return false;
            
            // Check if position recently harvested
            string posKey = GetPositionKey(node.Location);
            if (_harvestedPositions.TryGetValue(posKey, out var harvestExpiry) && DateTime.Now < harvestExpiry)
                return false;

            // Check persistent blacklist
            if (_persistentBlacklist.Contains(posKey))
                return false;
            
            return true;
        }

        /// <summary>
        /// Cleans up expired entries (call periodically)
        /// </summary>
        public static void CleanupExpired()
        {
            var now = DateTime.Now;
            
            // Clean blacklist
            var expiredGuids = new List<ulong>();
            foreach (var kvp in _blacklistedNodes)
            {
                if (now >= kvp.Value)
                    expiredGuids.Add(kvp.Key);
            }
            foreach (var guid in expiredGuids)
                _blacklistedNodes.Remove(guid);
            
            // Clean harvested positions
            var expiredPositions = new List<string>();
            foreach (var kvp in _harvestedPositions)
            {
                if (now >= kvp.Value)
                    expiredPositions.Add(kvp.Key);
            }
            foreach (var pos in expiredPositions)
                _harvestedPositions.Remove(pos);
        }

        /// <summary>
        /// FEAT-40: Saves the persistent blacklist to disk.
        /// </summary>
        public static void SaveBlacklist()
        {
            try
            {
                string dir = Path.GetDirectoryName(BlacklistFilePath)!;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllLines(BlacklistFilePath, _persistentBlacklist);
                Logging.WriteDebug($"[GatherBuddy] Saved {_persistentBlacklist.Count} blacklisted positions");
            }
            catch (Exception ex)
            {
                Logging.WriteDebug($"[GatherBuddy] Failed to save blacklist: {ex.Message}");
            }
        }

        /// <summary>
        /// FEAT-40: Loads the persistent blacklist from disk.
        /// </summary>
        public static void LoadBlacklist()
        {
            _persistentBlacklist.Clear();
            try
            {
                if (File.Exists(BlacklistFilePath))
                {
                    foreach (var line in File.ReadAllLines(BlacklistFilePath))
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                            _persistentBlacklist.Add(line.Trim());
                    }
                    Logging.WriteDebug($"[GatherBuddy] Loaded {_persistentBlacklist.Count} blacklisted positions");
                }
            }
            catch (Exception ex)
            {
                Logging.WriteDebug($"[GatherBuddy] Failed to load blacklist: {ex.Message}");
            }
        }

        private static string GetPositionKey(WoWPoint pos)
        {
            // Round to 5 yards to group position variations
            int x = (int)(pos.X / 5) * 5;
            int y = (int)(pos.Y / 5) * 5;
            int z = (int)(pos.Z / 5) * 5;
            return $"{x},{y},{z}";
        }

        public static void Reset()
        {
            _blacklistedNodes.Clear();
            _harvestedPositions.Clear();
            // Note: persistent blacklist is NOT cleared on reset
        }
    }
}
