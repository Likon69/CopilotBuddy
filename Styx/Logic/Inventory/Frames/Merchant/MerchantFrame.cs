#nullable disable
using System;
using System.Collections.Generic;
using System.Text;
using GreenMagic;
using Styx.Logic.Combat;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Styx.Logic.Inventory.Frames.Merchant
{
    /// <summary>
    /// Represents the merchant/vendor frame.
    /// Addresses from HB 3.3.5a.
    /// </summary>
    public class MerchantFrame : Frame
    {
        public MerchantFrame() : base("MerchantFrame")
        {
        }

        static MerchantFrame()
        {
            Instance = new MerchantFrame();
        }

        /// <summary>
        /// The merchant NPC.
        /// Address: 12559336U (0xBF9BE8)
        /// </summary>
        public WoWUnit Merchant
        {
            get
            {
                ulong guid = ObjectManager.Wow.Read<ulong>(12559336U);
                return ObjectManager.GetObjectByGuid<WoWUnit>(guid);
            }
        }

        /// <summary>
        /// Number of buyback items.
        /// Address: 12559348U (0xBF9BF4)
        /// </summary>
        public int NumBuybackItems
        {
            get
            {
                return ObjectManager.Wow.Read<int>(12559348U);
            }
        }

        /// <summary>
        /// Sells all items matching the specified qualities.
        /// </summary>
        public void SellItemQualities(ItemQuality qualities, IEnumerable<string> nameExceptions, IEnumerable<uint> idExceptions)
        {
            if (qualities == ItemQuality.None)
                return;

            List<string> qualityConditions = new List<string>();
            if ((qualities & ItemQuality.Poor) != ItemQuality.None)
                qualityConditions.Add("quality == 0");
            if ((qualities & ItemQuality.Common) != ItemQuality.None)
                qualityConditions.Add("quality == 1");
            if ((qualities & ItemQuality.Uncommon) != ItemQuality.None)
                qualityConditions.Add("quality == 2");
            if ((qualities & ItemQuality.Rare) != ItemQuality.None)
                qualityConditions.Add("quality == 3");
            if ((qualities & ItemQuality.Epic) != ItemQuality.None)
                qualityConditions.Add("quality == 4");

            StringBuilder qualityBuilder = new StringBuilder();
            qualityBuilder.Append("(" + qualityConditions[0]);
            for (int i = 1; i < qualityConditions.Count; i++)
            {
                qualityBuilder.Append(" or " + qualityConditions[i]);
            }
            qualityBuilder.Append(")");

            StringBuilder exceptionsBuilder = new StringBuilder();
            if (nameExceptions != null || idExceptions != null)
            {
                HashSet<string> nameSet = new HashSet<string>();
                HashSet<uint> idSet = new HashSet<uint>();
                exceptionsBuilder.Append("local itemExceptions = {");
                bool hasExceptions = false;

                if (nameExceptions != null)
                {
                    foreach (string name in nameExceptions)
                    {
                        string lowerName = name.ToLower();
                        if (!string.IsNullOrEmpty(lowerName) && !nameSet.Contains(lowerName))
                        {
                            exceptionsBuilder.Append("{n=\"" + Lua.Escape(lowerName) + "\"},");
                            hasExceptions = true;
                            nameSet.Add(lowerName);
                        }
                    }
                }

                if (idExceptions != null)
                {
                    foreach (uint id in idExceptions)
                    {
                        if (!idSet.Contains(id))
                        {
                            exceptionsBuilder.Append("{i=" + id + "},");
                            hasExceptions = true;
                            idSet.Add(id);
                        }
                    }
                }

                if (hasExceptions)
                {
                    exceptionsBuilder.Remove(exceptionsBuilder.Length - 1, 1);
                }
                exceptionsBuilder.Append("}");
            }

            string lua = string.Format(
                "{0}for b=0,4 do for s=1,GetContainerNumSlots(b) do local itemLink = GetContainerItemLink(b, s) if itemLink then local _, _, _, _, id, _, _, _, _, _, _, _, _, name = string.find(itemLink, \"|?c?f?f?(%x*)|?H?([^:]*):?(%d+):?(%d*):?(%d*):?(%d*):?(%d*):?(%d*):?(%-?%d*):?(%-?%d*):?(%d*)|?h?%[?([^%[%]]*)%]?|?h?|?r?\") id = tonumber(id) name = string.lower(name) local _, _, quality = GetItemInfo(itemLink) if {1} then local skip = false if itemExceptions then for i=1, #itemExceptions do if (itemExceptions[i].i and id == itemExceptions[i].i) or (itemExceptions[i].n and name == itemExceptions[i].n) then skip = true break end end end if not skip then UseContainerItem(b, s) end end end end end",
                exceptionsBuilder + " ",
                qualityBuilder);

            Lua.DoString(lua);
        }

        public void Close()
        {
            Hide();
        }

        public new void Hide()
        {
            Lua.DoString("CloseMerchant()");
        }

        /// <summary>
        /// Number of items the merchant sells.
        /// Address: 12559344U (0xBF9BF0)
        /// </summary>
        public int MerchantNumItems
        {
            get
            {
                return ObjectManager.Wow.Read<int>(12559344U);
            }
        }

        private static bool CanAfford(int stack, WoWItem item)
        {
            return item.ItemInfo.BuyPrice * stack <= ObjectManager.Me.Coinage;
        }

        public void BuyItem(WoWItem item, int stackCount)
        {
            if (CanAfford(stackCount, item))
            {
                int? index = GetMerchantIndex(item.Entry);
                if (index.HasValue)
                {
                    Lua.DoString("BuyMerchantItem(" + index.Value + "," + stackCount + ")");
                }
            }
        }

        public void BuyItem(uint itemId, int stackCount)
        {
            int? index = GetMerchantIndex(itemId);
            if (index.HasValue)
            {
                Lua.DoString("BuyMerchantItem(" + index.Value + "," + stackCount + ")");
            }
        }

        /// <summary>
        /// Buys an item by merchant index and amount.
        /// </summary>
        public void BuyItem(int index, int amount)
        {
            if (index > 0 && amount > 0)
            {
                Lua.DoString("BuyMerchantItem(" + index + "," + amount + ")");
            }
        }

        public void SellItem(WoWItem item)
        {
            Lua.DoString(
                "for b=0,4 do if GetBagName(b) then for s=1, GetContainerNumSlots(b) do local itemLink = GetContainerItemLink(b, s) if itemLink then local _, stackCount = GetContainerItemInfo(b, s)\tif string.find(itemLink, \"{0}\") and stackCount == {1} then UseContainerItem(b, s)\tend\tend\tend end end",
                item.Name,
                item.StackCount);
        }

        public void RepairAllItems()
        {
            RepairAllItems(false);
        }

        public void RepairAllItems(bool useGuildBankFunds)
        {
            Lua.DoString("RepairAllItems(" + (useGuildBankFunds ? "1" : "nil") + ")");
        }

        /// <summary>
        /// Gets all items the merchant sells.
        /// </summary>
        public List<MerchantItem> MerchantItems
        {
            get
            {
                List<MerchantItem> list = new List<MerchantItem>();
                if (IsVisible)
                {
                    for (int i = 0; i < MerchantNumItems; i++)
                    {
                        MerchantItem item = GetMerchantItemAtIndex(i);
                        if (!list.Contains(item))
                        {
                            list.Add(item);
                        }
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// Gets all merchant items as an array.
        /// </summary>
        public MerchantItem[] GetAllMerchantItems()
        {
            return MerchantItems.ToArray();
        }

        private int? GetMerchantIndex(uint itemId)
        {
            for (int i = 1; i < MerchantNumItems + 1; i++)
            {
                MerchantItem item = GetMerchantItemAtIndex(i);
                if ((long)item.ItemId == (long)itemId)
                {
                    return item.Index;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets merchant item at specified index.
        /// Base address: 12554536U (0xBF87A8)
        /// Item size: 32 bytes
        /// </summary>
        private static MerchantItem GetMerchantItemAtIndex(int index)
        {
            uint offset = (uint)(32 * index);
            uint itemAddress = 12554536U + offset;
            return new MerchantItem(itemAddress, index);
        }

        /// <summary>
        /// Gets a buyback item by index.
        /// Address: 12554488U (0xBF8778)
        /// </summary>
        public WoWItem GetBuybackItem(int index)
        {
            if (index < 0 || index >= 12)
                throw new ArgumentOutOfRangeException("index");

            WoWBag inventory = StyxWoW.Me.Inventory;
            uint slotId = ObjectManager.Wow.Read<uint>((uint)(12554488 + index * 4));
            return inventory.GetItemBySlot(slotId);
        }

        /// <summary>
        /// Gets the best drink the merchant sells that the player can use.
        /// Returns -1 if none found.
        /// </summary>
        public int GetBestDrinkFromVendor()
        {
            try
            {
                return GetBestConsumableFromVendor("Drink");
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the best food the merchant sells that the player can use.
        /// Returns -1 if none found.
        /// </summary>
        public int GetBestFoodFromVendor()
        {
            try
            {
                return GetBestConsumableFromVendor("Food");
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the best consumable of a specific type from a vendor.
        /// </summary>
        private int GetBestConsumableFromVendor(string consumableType)
        {
            int bestIndex = -1;
            int bestLevel = -1;
            int playerLevel = StyxWoW.Me?.Level ?? 1;

            foreach (var item in GetAllMerchantItems())
            {
                if (item.ItemInfo == null)
                    continue;

                // Check if item has the spell effect (Food/Drink)
                int[] spellIds = item.ItemInfo.SpellId;
                if (spellIds == null || spellIds.Length == 0 || spellIds[0] == 0)
                    continue;

                WoWSpell spell = WoWSpell.FromId(spellIds[0]);
                if (spell == null || spell.Name != consumableType)
                    continue;

                // Check if player can use this item (level check)
                if (item.ItemInfo.RequiredLevel > playerLevel)
                    continue;

                // Find highest level item that player can use
                if (item.ItemInfo.RequiredLevel > bestLevel)
                {
                    bestLevel = item.ItemInfo.RequiredLevel;
                    bestIndex = item.Index;
                }
            }

            return bestIndex;
        }

        /// <summary>
        /// Gets a merchant item by its index.
        /// </summary>
        public MerchantItem GetMerchantItemByIndex(int index)
        {
            if (index < 0 || index >= MerchantNumItems)
                return null;
            return GetMerchantItemAtIndex(index);
        }

        /// <summary>
        /// Returns true if the buy operation was successful.
        /// </summary>
        public bool BuyItem(int index, int amount, out bool success)
        {
            if (index < 0 || amount <= 0)
            {
                success = false;
                return false;
            }
            Lua.DoString("BuyMerchantItem(" + index + "," + amount + ")");
            success = true;
            return true;
        }

        public static readonly MerchantFrame Instance;
    }
}
