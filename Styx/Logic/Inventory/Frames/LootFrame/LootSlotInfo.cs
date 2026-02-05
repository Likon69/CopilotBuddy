using System;
using System.Collections.Generic;
using Styx.WoWInternals;

#nullable disable
namespace Styx.Logic.Inventory.Frames.LootFrame
{
    /// <summary>
    /// Contains information about a loot slot from the loot window.
    /// </summary>
    public class LootSlotInfo
    {
        private readonly List<string> _slotData;

        /// <summary>
        /// Creates a new LootSlotInfo for the specified loot slot index.
        /// </summary>
        /// <param name="index">The zero-based slot index.</param>
        internal LootSlotInfo(int index)
        {
            // Lua uses 1-based indexing
            int luaIndex = index + 1;
            _slotData = Lua.GetReturnValues($"return GetLootSlotInfo({luaIndex})");
        }

        /// <summary>
        /// Gets the icon texture path for this loot slot.
        /// </summary>
        public string LootIcon
        {
            get
            {
                if (_slotData == null || _slotData.Count < 1)
                    return string.Empty;
                return _slotData[0] ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets the name of the item in this loot slot.
        /// </summary>
        public string LootName
        {
            get
            {
                if (_slotData == null || _slotData.Count < 2)
                    return string.Empty;
                return _slotData[1] ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets the quantity of items in this loot slot.
        /// </summary>
        public int LootQuantity
        {
            get
            {
                if (_slotData == null || _slotData.Count < 3 || string.IsNullOrEmpty(_slotData[2]))
                    return 0;

                if (int.TryParse(_slotData[2], out int quantity))
                    return quantity;
                return 0;
            }
        }

        /// <summary>
        /// Gets the rarity/quality of the item in this loot slot.
        /// </summary>
        public LootRarity LootRarity
        {
            get
            {
                if (_slotData == null || _slotData.Count < 4 || string.IsNullOrEmpty(_slotData[3]))
                    return LootRarity.Unknown;

                if (int.TryParse(_slotData[3], out int rarity) && Enum.IsDefined(typeof(LootRarity), rarity))
                    return (LootRarity)rarity;

                return LootRarity.Unknown;
            }
        }

        /// <summary>
        /// Gets whether this loot slot is currently locked (another player has priority).
        /// </summary>
        public bool Locked
        {
            get
            {
                if (_slotData == null || _slotData.Count < 5)
                    return false;
                return _slotData[4] == "1";
            }
        }

        /// <summary>
        /// Gets whether this slot contains valid loot data.
        /// </summary>
        public bool IsValid => _slotData != null && _slotData.Count >= 4;

        /// <summary>
        /// Returns a string representation of this loot slot.
        /// </summary>
        public override string ToString()
        {
            return $"[LootSlotInfo: {LootName} x{LootQuantity} ({LootRarity})]";
        }
    }
}
