using System;
using System.Collections.Generic;
using GreenMagic;
using Styx.Helpers;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.Patchables;
using Styx.WoWInternals.WoWCache;

namespace Styx.WoWInternals.WoWObjects
{
    public class WoWItem : WoWObject
    {
        #region Descriptor Offsets - ITEM_FIELD (3.3.5a)

        // WotLK 3.3.5a Item Descriptor Field INDICES (not byte offsets)
        // Object fields 0-5 (6 fields), then Item fields start at index 6
        // GetDescriptorField expects byte offset, so we multiply by 4
        // Reference: HB 3.3.5a ns4/Enum10.cs
        private const int OBJECT_FIELD_COUNT = 6;              // Object descriptor fields before Item fields
        
        // Item field indices (relative to item start, add OBJECT_FIELD_COUNT for absolute index)
        private const int ITEM_FIELD_OWNER = 0;                // 2 fields (ulong) - absolute index 6
        private const int ITEM_FIELD_CONTAINED = 2;            // 2 fields (ulong) - absolute index 8  
        private const int ITEM_FIELD_CREATOR = 4;              // 2 fields (ulong) - absolute index 10
        private const int ITEM_FIELD_GIFTCREATOR = 6;          // 2 fields (ulong) - absolute index 12
        private const int ITEM_FIELD_STACK_COUNT = 8;          // 1 field - absolute index 14 (0xE)
        private const int ITEM_FIELD_DURATION = 9;             // 1 field
        private const int ITEM_FIELD_SPELL_CHARGES = 10;       // 5 fields
        private const int ITEM_FIELD_FLAGS = 15;               // 1 field (0xF)
        private const int ITEM_FIELD_ENCHANTMENT = 16;         // 36 fields (12 enchants * 3)
        private const int ITEM_FIELD_PROPERTY_SEED = 52;       // 1 field (0x34)
        private const int ITEM_FIELD_RANDOM_PROPERTIES_ID = 53; // 1 field (0x35)
        private const int ITEM_FIELD_DURABILITY = 54;          // 1 field (0x36)  
        private const int ITEM_FIELD_MAXDURABILITY = 55;       // 1 field (0x37)
        private const int ITEM_FIELD_CREATE_PLAYED_TIME = 56;  // 1 field (0x38)

        #endregion

        #region Fields

        private string? _name;
        private string? _link;
        private ItemInfo? _itemInfo;
        private ItemStats? _itemStats;

        #endregion

        #region Item Flags

        [Flags]
        private enum ItemFlagValues : uint
        {
            None = 0x0,
            Soulbound = 0x1,         // flag_1
            Conjured = 0x2,          // flag_2
            Openable = 0x4,          // flag_3
            GiftWrapped = 0x8,       // flag_4
            // 0x10 unused (bit 4 skipped in WoW item flags)
            Totem = 0x20,            // flag_5 — was 0x10 (WRONG)
            TriggersSpell = 0x40,    // flag_6 — was 0x20 (WRONG)
            NoEquipCooldown = 0x80,  // flag_7 — was 0x40 (WRONG)
            IsWand = 0x100,          // flag_8 — was 0x200 (WRONG)
            IsWrappingPaper = 0x200, // flag_9 — was 0x400 (WRONG)
            // flags 10-12 (0x400, 0x800, 0x1000) unused
            IsCharter = 0x2000,      // flag_13
            IsReadable = 0x4000,     // flag_14
            IsPvPItem = 0x8000,      // flag_15
            CanExpire = 0x10000,     // flag_16
            // 0x20000 unused (bit 17 skipped)
            CanProspect = 0x40000,   // flag_17 — was 0x20000 (WRONG)
            IsUniqueEquipped = 0x80000, // flag_18 — was 0x40000 (WRONG)
            // flags 20-22 (0x100000-0x200000) unused
            IsThrownWeapon = 0x400000,  // flag_19 — was 0x80000 (WRONG)
            // flags 24-26 unused
            IsAccountBound = 0x8000000,  // flag_20 — was 0x100000 (WRONG)
            IsEnchantScroll = 0x10000000, // flag_21 — was 0x200000 (WRONG)
            IsMillable = 0x20000000      // flag_22 — was 0x400000 (WRONG)
        }

        #endregion

        #region Constructor

        public WoWItem(uint baseAddress) : base(baseAddress)
        {
        }

        #endregion

        #region Descriptor Helper
        
        /// <summary>
        /// Gets an item descriptor field value.
        /// Converts field index to byte offset: (OBJECT_FIELD_COUNT + fieldIndex) * 4
        /// </summary>
        private T GetItemDescriptor<T>(int fieldIndex) where T : struct
        {
            int byteOffset = (OBJECT_FIELD_COUNT + fieldIndex) * 4;
            return GetDescriptorField<T>(byteOffset);
        }

        #endregion

        #region Position Override (Items don't have position)

        // Name is NOT overridden - we use base WoWObject.Name -> GetObjectName() 
        // which works for items via vtable lookup (like .Reference solution does)

        public override WoWPoint Location => WoWPoint.Zero;
        public override float X => 0f;
        public override float Y => 0f;
        public override float Z => 0f;
        public override double Distance => 0;
        public override double DistanceSqr => 0;
        public override double Distance2D => 0;
        public override double Distance2DSqr => 0;

        public override void Interact() => Use();

        #endregion

        #region Name Methods

        private string GetItemName()
        {
            if (string.IsNullOrEmpty(_name))
            {
                uint randomPropId = RandomPropertiesId;
                if (randomPropId != 0)
                {
                    _name = GetItemNameWithSuffix(Entry, randomPropId);
                }
                else
                {
                    // Use ItemInfo.Name from cache instead of base.Name (which uses wrong vtable for items)
                    try
                    {
                        _name = ItemInfo?.Name ?? $"Item_{Entry}";
                    }
                    catch
                    {
                        _name = $"Item_{Entry}";
                    }
                }
            }
            return _name ?? $"Item_{Entry}";
        }

        private static string GetItemNameWithSuffix(uint itemId, uint suffixId)
        {
            var executor = ObjectManager.Executor;
            if (executor == null)
                return string.Empty;

            lock (executor.AssemblyLock)
            {
                uint bufferPtr = executor.Memory.AllocateMemory(1024);
                try
                {
                    executor.Clear();
                    executor.AddLine("push {0}", suffixId);
                    executor.AddLine("push {0}", itemId);
                    executor.AddLine("push 0");
                    executor.AddLine("push {0}", bufferPtr);
                    executor.AddLine("call {0}", (uint)GlobalOffsets.CGItem_C__BuildItemName);
                    executor.AddLine("add esp, 16");
                    executor.AddLine("retn");
                    executor.Execute();
                    string retval;
                    using (StyxWoW.Memory.TemporaryCacheState(false))
                    {
                        retval = executor.Memory.Read<string>(bufferPtr);
                    }
                    return retval;
                }
                finally
                {
                    if (bufferPtr != 0)
                        executor.Memory.FreeMemory(bufferPtr);
                }
            }
        }

        #endregion

        #region Core Properties

        public ulong OwnerGuid => GetItemDescriptor<ulong>(ITEM_FIELD_OWNER);

        public ulong ContainerGuid => GetItemDescriptor<ulong>(ITEM_FIELD_CONTAINED);

        public ulong CreatorGuid => GetItemDescriptor<ulong>(ITEM_FIELD_CREATOR);

        public ulong GiftCreatorGuid => GetItemDescriptor<ulong>(ITEM_FIELD_GIFTCREATOR);

        public uint StackCount => GetItemDescriptor<uint>(ITEM_FIELD_STACK_COUNT);

        public uint Duration => GetItemDescriptor<uint>(ITEM_FIELD_DURATION);

        public uint SpellCharges => GetItemDescriptor<uint>(ITEM_FIELD_SPELL_CHARGES);

        public uint Flags => GetItemDescriptor<uint>(ITEM_FIELD_FLAGS);

        public uint PropertySeed => GetItemDescriptor<uint>(ITEM_FIELD_PROPERTY_SEED);

        public uint RandomPropertiesId => GetItemDescriptor<uint>(ITEM_FIELD_RANDOM_PROPERTIES_ID);

        public WoWItemRandomProperties RandomProperties => new WoWItemRandomProperties(RandomPropertiesId);

        public uint Durability => GetItemDescriptor<uint>(ITEM_FIELD_DURABILITY);

        public uint MaxDurability => GetItemDescriptor<uint>(ITEM_FIELD_MAXDURABILITY);

        public double DurabilityPercent
        {
            get
            {
                uint max = MaxDurability;
                return max > 0 ? (Durability * 100.0 / max) : 100;
            }
        }

        public bool IsBroken => MaxDurability > 0 && Durability == 0;

        public uint CreatePlayedTime => GetItemDescriptor<uint>(ITEM_FIELD_CREATE_PLAYED_TIME);

        #endregion

        #region Enchantment Methods

        public WoWItemEnchantment TemporaryEnchantment => GetEnchantment(1);

        public WoWItemEnchantment GetEnchantment(string name)
        {
            for (uint i = 0; i < 12; i++)
            {
                var enchant = GetEnchantment(i);
                if (enchant.IsValid && string.Equals(enchant.Name, name, StringComparison.OrdinalIgnoreCase))
                    return enchant;
            }
            return new WoWItemEnchantment(0, 0, 0);
        }

        public WoWItemEnchantment GetEnchantmentById(uint id)
        {
            for (uint i = 0; i < 12; i++)
            {
                var enchant = GetEnchantment(i);
                if (enchant.IsValid && enchant.Id == id)
                    return enchant;
            }
            return new WoWItemEnchantment(0, 0, 0);
        }

        public WoWItemEnchantment GetEnchantment(uint index)
        {
            int fieldOffset = ITEM_FIELD_ENCHANTMENT + (int)(index * 3);
            uint id = GetItemDescriptor<uint>(fieldOffset);
            int duration = GetItemDescriptor<int>(fieldOffset + 1);
            int charges = GetItemDescriptor<int>(fieldOffset + 2);
            return new WoWItemEnchantment(id, duration, charges);
        }

        #endregion

        #region Stat Methods

        public WoWItemStat GetStat(int index) => new WoWItemStat(GetStatType(index), GetStatValue(index));

        public WoWItemStatType GetStatType(int index) => (WoWItemStatType)ItemInfo.StatId[index];

        public int GetStatValue(int index) => ItemInfo.StatValue[index];

        public float GetMinDamage(int index) => ItemInfo.DamageMin[index];

        public float GetMaxDamage(int index) => ItemInfo.DamageMax[index];

        public int GetDamageType(int index) => ItemInfo.DamageType[index];

        #endregion

        #region Spell Methods

        public List<WoWItemSpell> ItemSpells
        {
            get
            {
                var list = new List<WoWItemSpell>();
                for (int i = 0; i < 5; i++)
                {
                    var spell = GetSpell(i);
                    if (spell == null || !spell.IsValid)
                        break;
                    list.Add(spell);
                }
                return list;
            }
        }

        public WoWItemSpell? GetSpell(int index)
        {
            try
            {
                var info = ItemInfo;
                if (info == null) return null;

                int spellId = info.SpellId[index];
                int triggerId = info.SpellTriggerId[index];
                int charges = info.SpellCharges[index];
                int cooldown = info.SpellCooldown[index];
                int category = info.SpellCategory[index];
                int categoryCooldown = info.SpellCategoryCooldown[index];

                var spell = new WoWItemSpell(spellId, triggerId, charges, cooldown, category, categoryCooldown);
                return spell.IsValid ? spell : null;
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        public WoWSocketColor GetSocketColor(int index) => (WoWSocketColor)ItemInfo.SocketColor[index];

        #endregion

        #region ItemInfo

        public ItemInfo ItemInfo
        {
            get
            {
                if (_itemInfo == null)
                    _itemInfo = ItemInfo.FromId(Entry);
                return _itemInfo!;
            }
        }

        public WoWItemQuality Quality => ItemInfo?.Quality ?? WoWItemQuality.Common;

        public int RequiredLevel => ItemInfo?.RequiredLevel ?? 0;

        public int ItemLevel => ItemInfo?.Level ?? 0;

        public WoWItemClass ItemClass => ItemInfo?.ItemClass ?? WoWItemClass.Consumable;

        public WoWInventorySlot EquipSlot => (WoWInventorySlot)(ItemInfo?.EquipSlot ?? InventoryType.None);

        public int SellPrice => ItemInfo?.SellPrice ?? 0;

        public int BuyPrice => ItemInfo?.BuyPrice ?? 0;

        #endregion

        #region Link & Stats

        public string Link => GetItemLink();

        private string GetItemLink()
        {
            if (_link == null)
                _link = CreateItemLink(Entry, (int)Quality, 0, Array.Empty<int>(), RandomPropertiesId);
            return _link;
        }

        public ItemStats GetItemStats()
        {
            if (_itemStats == null)
                _itemStats = CalculateItemStats(this);
            return _itemStats;
        }

        public ItemStats ItemStats => GetItemStats();

        public int BagIndex
        {
            get
            {
                ulong containerGuid = ContainerGuid;
                for (uint index = 0; index <= 10U; ++index)
                {
                    if ((long)StyxWoW.Me.GetBagGuidAtIndex(index) == (long)containerGuid)
                        return (int)index;
                }
                return -1;
            }
        }

        public int BagSlot
        {
            get
            {
                ulong[] guids = BagIndex == -1
                    ? StyxWoW.Me.Inventory.Backpack.ItemGuids
                    : StyxWoW.Me.GetBagAtIndex((uint)BagIndex).ItemGuids;

                for (int bagSlot = 0; bagSlot < guids.Length; ++bagSlot)
                {
                    if ((long)guids[bagSlot] == (long)Guid)
                        return bagSlot;
                }
                return -1;
            }
        }

        private static string CreateItemLink(uint itemId, int quality, int enchantId, int[]? gemIds, uint suffixId)
        {
            return $"|cff{GetQualityColor(quality)}|Hitem:{itemId}:0:0:0:0:0:{suffixId}:0|h[Item]|h|r";
        }

        private static string GetQualityColor(int quality)
        {
            switch (quality)
            {
                case 0: return "9d9d9d"; // Poor
                case 1: return "ffffff"; // Common
                case 2: return "1eff00"; // Uncommon
                case 3: return "0070dd"; // Rare
                case 4: return "a335ee"; // Epic
                case 5: return "ff8000"; // Legendary
                case 6: return "e6cc80"; // Artifact
                case 7: return "00ccff"; // Heirloom
                default: return "ffffff";
            }
        }

        private static ItemStats CalculateItemStats(WoWItem item)
        {
            var stats = new ItemStats();
            var info = item.ItemInfo;
            if (info == null) return stats;

            if (info.IsWeapon)
                stats.DPS = info.DPS;

            var itemStats = info.GetItemStats();
            foreach (var kvp in itemStats)
                stats.Stats[(StatTypes)(int)kvp.Key] = kvp.Value;

            return stats;
        }

        #endregion

        #region Use Methods

        public bool Use() => Use(0UL, false);

        public bool Use(bool forceUse) => Use(0UL, forceUse);

        public bool Use(ulong targetGuid) => Use(targetGuid, false);

        public bool Use(ulong targetGuid, bool forceUse)
        {
            return UseItem(BaseAddress, targetGuid, forceUse);
        }

        public void UseContainerItem()
        {
            Lua.DoString("UseContainerItem({0}, {1})", (object)(this.BagIndex + 1), (object)(this.BagSlot + 1));
        }

        public void PickUp()
        {
            Lua.DoString("PickupContainerItem({0}, {1})", (object)(this.BagIndex + 1), (object)(this.BagSlot + 1));
        }

        private static bool UseItem(uint itemPtr, ulong targetGuid, bool forceUse)
        {
            var executor = ObjectManager.Executor;
            if (executor == null)
                return false;

            lock (executor.AssemblyLock)
            {
                uint guidPtr = executor.Memory.AllocateMemory(8);
                try
                {
                    executor.Memory.Write(guidPtr, targetGuid);

                    executor.Clear();
                    executor.AddLine("push {0}", forceUse ? 1 : 0);
                    executor.AddLine("push {0}", guidPtr);
                    executor.AddLine("mov ecx, {0}", itemPtr);
                    executor.AddLine("call {0}", (uint)GlobalOffsets.CGItem_C__Use);
                    executor.AddLine("retn");
                    executor.Execute();

                    bool success;
                    using (StyxWoW.Memory.TemporaryCacheState(false))
                    {
                        success = executor.Memory.Read<int>(executor.ReturnPointer) != 0;
                    }
                    return success;
                }
                finally
                {
                    if (guidPtr != 0)
                        executor.Memory.FreeMemory(guidPtr);
                }
            }
        }

        #endregion

        #region Flag Properties

        private bool HasFlag(ItemFlagValues flag) => (Flags & (uint)flag) != 0;
        public bool IsSoulbound => HasFlag(ItemFlagValues.Soulbound);
        public bool IsConjured => HasFlag(ItemFlagValues.Conjured);
        public bool IsOpenable => HasFlag(ItemFlagValues.Openable);
        public bool IsGiftWrapped => HasFlag(ItemFlagValues.GiftWrapped);

        public bool IsTotem => HasFlag(ItemFlagValues.Totem);

        public bool TriggersSpell => HasFlag(ItemFlagValues.TriggersSpell);

        public bool HasEquipCooldown => !HasFlag(ItemFlagValues.NoEquipCooldown);

        public bool IsWand => HasFlag(ItemFlagValues.IsWand);

        public bool IsWrappingPaper => HasFlag(ItemFlagValues.IsWrappingPaper);

        public bool IsCharter => HasFlag(ItemFlagValues.IsCharter);

        public bool IsReadable => HasFlag(ItemFlagValues.IsReadable);

        public bool IsPvPItem => HasFlag(ItemFlagValues.IsPvPItem);

        public bool CanExpire => HasFlag(ItemFlagValues.CanExpire);

        public bool CanProspect => HasFlag(ItemFlagValues.CanProspect);

        public bool IsUniqueEquipped => HasFlag(ItemFlagValues.IsUniqueEquipped);

        public bool IsThrownWeapon => HasFlag(ItemFlagValues.IsThrownWeapon);

        public bool IsAccountBound => HasFlag(ItemFlagValues.IsAccountBound);

        public bool IsEnchantScroll => HasFlag(ItemFlagValues.IsEnchantScroll);

        public bool IsMillable => HasFlag(ItemFlagValues.IsMillable);

        public bool IsGift => HasFlag(ItemFlagValues.GiftWrapped);

        #endregion

        #region Additional Properties

        public float Cooldown
        {
            get
            {
                return Lua.GetReturnVal<float>("return GetItemCooldown(" + Entry + ")", 0);
            }
        }

        /// <summary>
        /// FEAT-43: Gets the remaining cooldown as a TimeSpan.
        /// Uses Lua GetItemCooldown which returns (startTime, duration, isEnabled).
        /// </summary>
        public TimeSpan CooldownTimeLeft
        {
            get
            {
                try
                {
                    var results = Lua.GetReturnValues(
                        $"local s,d,e = GetItemCooldown({Entry}); if s > 0 then return d - (GetTime() - s) else return 0 end");
                    if (results != null && results.Count > 0)
                    {
                        float seconds = Lua.ParseLuaValue<float>(results[0]);
                        if (seconds > 0)
                            return TimeSpan.FromSeconds(seconds);
                    }
                }
                catch { }
                return TimeSpan.Zero;
            }
        }

        public bool Usable
        {
            get
            {
                string lua = $"local u,q = IsUsableItem({Entry}); return u";
                return Lua.GetReturnVal<bool>(lua, 0);
            }
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return $"[Item: {Name} (Entry: {Entry}, Stack: {StackCount}, Quality: {Quality})]";
        }

        #endregion

        #region Nested Classes

        public class WoWItemEnchantment
        {
            public uint Id { get; private set; }
            public int ExpirationTimestamp { get; private set; }
            public int ChargesLeft { get; private set; }

            internal WoWItemEnchantment(uint id)
                : this(id, 0, 0)
            {
            }

            internal WoWItemEnchantment(uint id, int expiration, int chargesLeft)
            {
                Id = id;
                ExpirationTimestamp = expiration;
                ChargesLeft = chargesLeft;
            }

            public bool IsValid => Id != 0;

            public string Name
            {
                get
                {
                    if (!IsValid) return string.Empty;
                    var db = StyxWoW.Db[ClientDb.SpellItemEnchantment];
                    if (db == null) return string.Empty;
                    var row = db.GetRow(Id);
                    if (row == null || !row.IsValid) return string.Empty;
                    return row.GetField<string>(14U) ?? string.Empty;
                }
            }

            /// <summary>
            /// Gets the stat bonus at the specified index (0-2)
            /// </summary>
            public WoWItemStat? GetStat(int index)
            {
                if (index < 0 || index > 2)
                    throw new ArgumentOutOfRangeException(nameof(index), "index can't be greater than 2");
                
                if (!IsValid) return null;
                
                var db = StyxWoW.Db[ClientDb.SpellItemEnchantment];
                if (db == null) return null;
                var row = db.GetRow(Id);
                if (row == null || !row.IsValid) return null;
                
                // SpellItemEnchantment record structure:
                // Fields 3-5: EnchantmentType[3]
                // Fields 6-8: MinAmount[3]
                // Fields 9-11: MaxAmount[3]
                // Fields 12-14: SpellId[3] (used as stat type when EnchantmentType == 5)
                
                int enchantType = row.GetField<int>((uint)(3 + index));
                if (enchantType != 5) // 5 = stat bonus type
                    return null;
                    
                int statType = row.GetField<int>((uint)(12 + index));
                int statValue = row.GetField<int>((uint)(6 + index));
                
                return new WoWItemStat((WoWItemStatType)statType, statValue);
            }

            public override string ToString() => $"[Enchant: {Name} (Id: {Id})]";
        }

        public class WoWItemRandomProperties
        {
            public uint Id { get; private set; }

            internal WoWItemRandomProperties(uint id)
            {
                Id = id;
            }

            public bool IsValid => Id != 0;

            public string Name
            {
                get
                {
                    if (!IsValid) return string.Empty;
                    var db = StyxWoW.Db[ClientDb.ItemRandomSuffix];
                    if (db == null) return string.Empty;
                    var row = db.GetRow(Id);
                    if (row == null || !row.IsValid) return string.Empty;
                    return row.GetField<string>(1U) ?? string.Empty;
                }
            }

            public override string ToString() => $"[RandomProps: {Name} (Id: {Id})]";
        }

        public class WoWItemSpell
        {
            public int Id { get; private set; }
            public int TriggerId { get; private set; }
            public int Charges { get; private set; }
            public int Cooldown { get; private set; }
            public int Category { get; private set; }
            public int CategoryCooldown { get; private set; }
            public WoWSpell? ActualSpell { get; private set; }

            internal WoWItemSpell(int id, int triggerId, int charges, int cooldown, int category, int categoryCooldown)
            {
                Id = id;
                TriggerId = triggerId;
                Charges = charges;
                Cooldown = cooldown;
                Category = category;
                CategoryCooldown = categoryCooldown;
                ActualSpell = WoWSpell.FromId(id);
            }

            public bool IsValid => Id != 0;

            public override string ToString() => $"[ItemSpell: {Id} (Trigger: {TriggerId})]";
        }

        public class WoWItemStat
        {
            public WoWItemStatType Type { get; set; }
            public int Value { get; set; }

            public WoWItemStat(WoWItemStatType type, int value)
            {
                Type = type;
                Value = value;
            }

            public override string ToString() => $"[Stat: {Type} = {Value}]";
        }

        #endregion
    }
}
