using System;
using System.Runtime.InteropServices;
using Styx.WoWInternals;

#nullable disable
namespace Styx.Logic.Inventory
{
    /// <summary>
    /// Handles loot roll events for group loot in dungeons/raids.
    /// </summary>
    public static class LootRoll
    {
        /// <summary>
        /// Event fired when a loot roll starts.
        /// </summary>
        public static event LootRollEventDelegate OnLootRoll;

        /// <summary>
        /// Initializes loot roll event listening.
        /// </summary>
        internal static void Initialize()
        {
            Lua.Events.AttachEvent("START_LOOT_ROLL", OnStartLootRoll);
        }

        private static void OnStartLootRoll(object sender, LuaEventArgs e)
        {
            if (e.Args == null || e.Args.Length == 0)
                return;

            int rollId = Convert.ToInt32(e.Args[0]);
            var info = GetRollInfo(rollId);

            if (info.HasValue && OnLootRoll != null)
            {
                OnLootRoll(info.Value);
            }
        }

        /// <summary>
        /// Gets loot roll information from memory.
        /// </summary>
        private static LootRollItemInfo? GetRollInfo(int rollId)
        {
            // For 3.3.5a, we use Lua to get roll info instead of direct memory access
            // as the structure may differ
            try
            {
                var result = Lua.GetReturnValues(
                    $"local texture, name, count, quality, bindOnPickUp, canNeed, canGreed, canDisenchant = GetLootRollItemInfo({rollId}); " +
                    $"return texture or '', name or '', count or 0, quality or 0, bindOnPickUp and 1 or 0, canNeed and 1 or 0, canGreed and 1 or 0, canDisenchant and 1 or 0");

                if (result == null || result.Count < 8)
                    return null;

                return new LootRollItemInfo
                {
                    RollId = (uint)rollId,
                    ItemName = result[1],
                    Count = Convert.ToUInt32(result[2]),
                    Quality = Convert.ToInt32(result[3]),
                    BindOnPickUp = result[4] == "1",
                    CanNeed = result[5] == "1",
                    CanGreed = result[6] == "1",
                    CanDisenchant = result[7] == "1"
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Rolls need on an item.
        /// </summary>
        public static void RollNeed(uint rollId)
        {
            Lua.DoString($"RollOnLoot({rollId}, 1)");
        }

        /// <summary>
        /// Rolls greed on an item.
        /// </summary>
        public static void RollGreed(uint rollId)
        {
            Lua.DoString($"RollOnLoot({rollId}, 2)");
        }

        /// <summary>
        /// Rolls disenchant on an item.
        /// </summary>
        public static void RollDisenchant(uint rollId)
        {
            Lua.DoString($"RollOnLoot({rollId}, 3)");
        }

        /// <summary>
        /// Passes on an item.
        /// </summary>
        public static void RollPass(uint rollId)
        {
            Lua.DoString($"RollOnLoot({rollId}, 0)");
        }

        /// <summary>
        /// Delegate for loot roll events.
        /// </summary>
        public delegate void LootRollEventDelegate(LootRollItemInfo info);

        /// <summary>
        /// Wrapper class for interacting with loot roll info.
        /// </summary>
        public class LootRollItem
        {
            private readonly LootRollItemInfo _info;

            internal LootRollItem(LootRollItemInfo info)
            {
                _info = info;
            }

            public uint RollId => _info.RollId;
            public bool CanNeed => _info.CanNeed;
            public bool CanGreed => _info.CanGreed;
            public bool CanDisenchant => _info.CanDisenchant;
            public string ItemName => _info.ItemName;
            public uint Count => _info.Count;
            public int Quality => _info.Quality;
        }

        /// <summary>
        /// Information about a loot roll item.
        /// </summary>
        public struct LootRollItemInfo
        {
            public uint RollId;
            public string ItemName;
            public uint Count;
            public int Quality;
            public bool BindOnPickUp;
            public bool CanNeed;
            public bool CanGreed;
            public bool CanDisenchant;

            public override string ToString()
            {
                return $"RollId: {RollId}, Name: {ItemName}, Count: {Count}, Quality: {Quality}, " +
                       $"CanNeed: {CanNeed}, CanGreed: {CanGreed}, CanDisenchant: {CanDisenchant}";
            }
        }
    }
}
