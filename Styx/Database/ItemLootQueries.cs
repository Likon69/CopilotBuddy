#nullable disable
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Styx.Helpers;

namespace Styx.Database
{
    /// <summary>
    /// Provides item→creature and item→gameobject loot lookups from item_loot.db.
    /// Replaces the limited hardcoded DropDatabase for mob/gameobject identification.
    /// </summary>
    public static class ItemLootQueries
    {
        private static SQLiteConnection _connection;
        private static bool _initialized;
        private static bool _isAvailable;

        private static SQLiteCommand _getCreaturesByItemCmd;
        private static SQLiteCommand _unitDropsItemCmd;
        private static SQLiteCommand _getGameObjectsByItemCmd;
        private static SQLiteCommand _gameObjectDropsItemCmd;

        public static bool IsAvailable
        {
            get
            {
                EnsureInitialized();
                return _isAvailable;
            }
        }

        private static void EnsureInitialized()
        {
            if (_initialized) return;
            _initialized = true;

            try
            {
                var dbPath = Path.Combine(Logging.ApplicationPath, "item_loot.db");
                if (!File.Exists(dbPath))
                {
                    Logging.WriteDebug("[ItemLoot] Database not found: {0}", dbPath);
                    return;
                }

                var builder = new SQLiteConnectionStringBuilder
                {
                    DataSource = dbPath,
                    ReadOnly = true
                };

                _connection = new SQLiteConnection(builder.ConnectionString);
                _connection.Open();

                _getCreaturesByItemCmd = new SQLiteCommand(
                    "SELECT creature_entry FROM creature_loot WHERE item_id = @item_id",
                    _connection);
                _getCreaturesByItemCmd.Parameters.Add("@item_id", System.Data.DbType.Int32);

                _unitDropsItemCmd = new SQLiteCommand(
                    "SELECT 1 FROM creature_loot WHERE item_id = @item_id AND creature_entry = @creature_entry LIMIT 1",
                    _connection);
                _unitDropsItemCmd.Parameters.Add("@item_id", System.Data.DbType.Int32);
                _unitDropsItemCmd.Parameters.Add("@creature_entry", System.Data.DbType.Int32);

                _getGameObjectsByItemCmd = new SQLiteCommand(
                    "SELECT gameobject_entry FROM gameobject_loot WHERE item_id = @item_id",
                    _connection);
                _getGameObjectsByItemCmd.Parameters.Add("@item_id", System.Data.DbType.Int32);

                _gameObjectDropsItemCmd = new SQLiteCommand(
                    "SELECT 1 FROM gameobject_loot WHERE item_id = @item_id AND gameobject_entry = @gameobject_entry LIMIT 1",
                    _connection);
                _gameObjectDropsItemCmd.Parameters.Add("@item_id", System.Data.DbType.Int32);
                _gameObjectDropsItemCmd.Parameters.Add("@gameobject_entry", System.Data.DbType.Int32);

                _isAvailable = true;
                Logging.Write("[ItemLoot] Database loaded: item_loot.db");
            }
            catch (Exception ex)
            {
                Logging.WriteDebug("[ItemLoot] Failed to load: {0}", ex.Message);
                _isAvailable = false;
            }
        }

        /// <summary>
        /// Gets all creature entries that drop a given item.
        /// </summary>
        public static List<uint> GetCreaturesForItem(uint itemId)
        {
            EnsureInitialized();
            var result = new List<uint>();
            if (!_isAvailable) return result;

            try
            {
                _getCreaturesByItemCmd.Parameters["@item_id"].Value = (int)itemId;
                using var reader = _getCreaturesByItemCmd.ExecuteReader();
                while (reader.Read())
                    result.Add((uint)reader.GetInt32(0));
            }
            catch (Exception ex)
            {
                Logging.WriteDebug("[ItemLoot] Error getting creatures for item {0}: {1}", itemId, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Checks if a specific creature drops a specific item.
        /// </summary>
        public static bool UnitDropsItem(uint creatureEntry, uint itemId)
        {
            EnsureInitialized();
            if (!_isAvailable) return false;

            try
            {
                _unitDropsItemCmd.Parameters["@item_id"].Value = (int)itemId;
                _unitDropsItemCmd.Parameters["@creature_entry"].Value = (int)creatureEntry;
                using var reader = _unitDropsItemCmd.ExecuteReader();
                return reader.Read();
            }
            catch (Exception ex)
            {
                Logging.WriteDebug("[ItemLoot] Error checking unit drop: {0}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets all gameobject entries that contain a given item.
        /// </summary>
        public static List<uint> GetGameObjectsForItem(uint itemId)
        {
            EnsureInitialized();
            var result = new List<uint>();
            if (!_isAvailable) return result;

            try
            {
                _getGameObjectsByItemCmd.Parameters["@item_id"].Value = (int)itemId;
                using var reader = _getGameObjectsByItemCmd.ExecuteReader();
                while (reader.Read())
                    result.Add((uint)reader.GetInt32(0));
            }
            catch (Exception ex)
            {
                Logging.WriteDebug("[ItemLoot] Error getting gameobjects for item {0}: {1}", itemId, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Checks if a specific gameobject contains a specific item.
        /// </summary>
        public static bool GameObjectDropsItem(uint gameObjectEntry, uint itemId)
        {
            EnsureInitialized();
            if (!_isAvailable) return false;

            try
            {
                _gameObjectDropsItemCmd.Parameters["@item_id"].Value = (int)itemId;
                _gameObjectDropsItemCmd.Parameters["@gameobject_entry"].Value = (int)gameObjectEntry;
                using var reader = _gameObjectDropsItemCmd.ExecuteReader();
                return reader.Read();
            }
            catch (Exception ex)
            {
                Logging.WriteDebug("[ItemLoot] Error checking gameobject drop: {0}", ex.Message);
                return false;
            }
        }
    }
}
