#nullable disable

using System;
using System.Collections.Generic;
using System.Text;
using GreenMagic;
using Styx.WoWInternals;

namespace Styx.Logic.Inventory.Frames.LootFrame
{
    /// <summary>
    /// Handles the loot frame UI and looting operations.
    /// </summary>
    public class LootFrame : Frame
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static readonly LootFrame Instance = new LootFrame();

        public LootFrame() : base("LootFrame")
        {
        }

        /// <summary>
        /// Whether loot frame is visible (has a looting target).
        /// </summary>
        public new bool IsVisible
        {
            get { return LootingObjectGuid != 0UL; }
        }

        /// <summary>
        /// GUID of the object being looted.
        /// Address: 12560600 (0xBFA8D8)
        /// </summary>
        public ulong LootingObjectGuid
        {
            get
            {
                Memory wow = ObjectManager.Wow;
                if (wow == null) return 0UL;
                return wow.Read<ulong>(12560600U);
            }
        }

        /// <summary>
        /// Number of items available to loot.
        /// </summary>
        public int LootItems
        {
            get
            {
                try
                {
                    var result = Lua.GetReturnValues("return GetNumLootItems()");
                    if (result != null && result.Count > 0)
                        return int.Parse(result[0]);
                    return 0;
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the item ID at the specified loot slot.
        /// Base address: 12560020 (0xBFA694), 32 bytes per slot
        /// </summary>
        public uint GetItemId(int slot)
        {
            uint address = (uint)(12560020 + 32 * slot);
            Memory wow = ObjectManager.Wow;
            if (wow == null) return 0U;
            return wow.Read<uint>(address);
        }

        /// <summary>
        /// BUG-21 fix: Returns structured loot slot info array instead of just a string.
        /// </summary>
        public LootSlotInfo[] GetLootSlots()
        {
            int count = LootItems;
            var slots = new LootSlotInfo[count];
            for (int i = 0; i < count; i++)
                slots[i] = new LootSlotInfo(i);
            return slots;
        }

        /// <summary>
        /// Gets loot information string.
        /// </summary>
        public string LootInfo(out bool locked)
        {
            StringBuilder sb = new StringBuilder();
            locked = false;

            for (int i = 0; i < LootItems; i++)
            {
                var slotInfo = new LootSlotInfo(i);
                locked = slotInfo.Locked;

                if (slotInfo.Locked)
                    return sb.ToString();

                if (!string.IsNullOrEmpty(slotInfo.LootName))
                {
                    sb.AppendFormat("Trying to loot #{0} of {1} Rarity:{2}{3}",
                        slotInfo.LootQuantity,
                        slotInfo.LootName,
                        slotInfo.LootRarity,
                        (i < LootItems - 1) ? Environment.NewLine : "");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Loots a specific slot.
        /// </summary>
        public void Loot(int slot)
        {
            Lua.DoString("LootSlot({0})", slot + 1);
        }

        /// <summary>
        /// Loots all items and closes the loot frame.
        /// </summary>
        public void LootAll()
        {
            Lua.DoString("for i=1, GetNumLootItems() do LootSlot(i) ConfirmBindOnUse() end CloseLoot()");
        }

        /// <summary>
        /// Closes the loot frame.
        /// </summary>
        public void Close()
        {
            if (IsVisible)
            {
                Lua.DoString("CloseLoot()");
            }
        }

        /// <summary>
        /// Loot slot information (BUG-21: made public for structured access).
        /// </summary>
        public class LootSlotInfo
        {
            private readonly List<string> _data;

            public LootSlotInfo(int index)
            {
                index++;
                _data = Lua.GetReturnValues("return GetLootSlotInfo(" + index + ")");
                if (_data == null)
                    _data = new List<string>();
            }

            public string LootIcon
            {
                get
                {
                    if (_data.Count > 0 && !string.IsNullOrEmpty(_data[0]))
                        return _data[0];
                    return "";
                }
            }

            public string LootName
            {
                get
                {
                    if (_data.Count > 1 && !string.IsNullOrEmpty(_data[1]))
                        return _data[1];
                    return "";
                }
            }

            public int LootQuantity
            {
                get
                {
                    if (_data.Count > 2 && !string.IsNullOrEmpty(_data[2]))
                    {
                        if (int.TryParse(_data[2], out int qty))
                            return qty;
                    }
                    return 0;
                }
            }

            public LootRarity LootRarity
            {
                get
                {
                    if (_data.Count > 3 && !string.IsNullOrEmpty(_data[3]))
                    {
                        if (Enum.TryParse(_data[3], out LootRarity rarity))
                            return rarity;
                    }
                    return LootRarity.Unknown;
                }
            }

            public bool Locked
            {
                get
                {
                    if (_data.Count > 4 && !string.IsNullOrEmpty(_data[4]))
                        return _data[4] == "1";
                    return false;
                }
            }
        }
    }
}
