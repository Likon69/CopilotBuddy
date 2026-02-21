using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GreenMagic;
using Styx.Patchables;

namespace Styx.WoWInternals.WoWCache
{
    public class WoWCache
    {
        #region Fields

        private readonly List<Cache> _caches = new List<Cache>();

        #endregion

        #region Constructor

        public WoWCache()
        {
            // Base address for cache structures in 3.3.5a (HB uses 12965520 = 0xC5D790)
            uint baseAddr = 12965520U; // Was 0xC5D780, correct is 0xC5D790
            
            for (int i = 0; i < 15; i++)
            {
                CacheDb cacheType = (CacheDb)i;
                GlobalOffsets functionOffset;
                
                switch (cacheType)
                {
                    case CacheDb.Creature:
                        functionOffset = GlobalOffsets.DbCreatureCache_GetInfoBlockById;
                        break;
                    case CacheDb.GameObject:
                        functionOffset = GlobalOffsets.DbGameObjectCache_GetInfoBlockById;
                        break;
                    case CacheDb.ItemName:
                        functionOffset = GlobalOffsets.DbItemNameCache_GetInfoBlockById;
                        break;
                    case CacheDb.Item:
                        functionOffset = GlobalOffsets.DBItemCache_GetInfoBlockByID;
                        break;
                    case CacheDb.Npc:
                        functionOffset = GlobalOffsets.DbNpcCache_GetInfoBlockById;
                        break;
                    case CacheDb.Name:
                        functionOffset = GlobalOffsets.DbNameCache_GetInfoBlockById;
                        break;
                    case CacheDb.Guild:
                        functionOffset = GlobalOffsets.DbGuildCache_GetInfoBlockById;
                        break;
                    case CacheDb.Quest:
                        functionOffset = GlobalOffsets.DbQuestCache_GetInfoBlockById;
                        break;
                    case CacheDb.PageText:
                        functionOffset = GlobalOffsets.DbPageTextCache_GetInfoBlockById;
                        break;
                    case CacheDb.PetName:
                        functionOffset = GlobalOffsets.DbPetNameCache_GetInfoBlockById;
                        break;
                    case CacheDb.Petition:
                        functionOffset = GlobalOffsets.DbPetitionCache_GetInfoBlockById;
                        break;
                    case CacheDb.ItemText:
                        functionOffset = GlobalOffsets.DbItemTextCache_GetInfoBlockById;
                        break;
                    case CacheDb.WoW:
                        functionOffset = GlobalOffsets.DbWoWCache_GetInfoBlockById;
                        break;
                    case CacheDb.ArenaTeam:
                        functionOffset = GlobalOffsets.DbArenaTeamCache_GetInfoBlockById;
                        break;
                    case CacheDb.Dance:
                        functionOffset = GlobalOffsets.DbDanceCache_GetInfoBlockById;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                _caches.Add(new Cache(baseAddr, functionOffset));
                baseAddr += 136; // Each cache structure is 136 bytes apart
            }
        }

        #endregion

        #region Indexer
        public Cache this[CacheDb index] => _caches[(int)index];

        #endregion

        #region Nested Classes
        public class Cache
        {
            public readonly uint Address;
            internal readonly GlobalOffsets FunctionOffset;
            private readonly uint _entryOffset;

            internal Cache(uint address, GlobalOffsets infoBlockAddr)
            {
                Address = address;
                FunctionOffset = infoBlockAddr;
                // Read the entry offset from the function
                var wow = ObjectManager.Wow;
                _entryOffset = wow?.Read<uint>((uint)infoBlockAddr + 162) ?? 0;
            }
            public InfoBlock? GetInfoBlockById(uint id)
            {
                var wow = ObjectManager.Wow;
                if (wow == null) return null;
                
                uint entryPtr;
                if (!TryFindEntry(id, out entryPtr))
                    return null;

                // Check if entry is valid
                if (wow.Read<byte>(entryPtr + _entryOffset) == 0)
                    return null;

                return new InfoBlock(entryPtr + 24, id);
            }

            private bool TryFindEntry(uint id, out uint result)
            {
                result = 0;
                
                var memory = ObjectManager.Wow;
                if (memory == null) return false;
                
                var cacheHeader = memory.Read<CacheHeader>(Address);
                var hashTable = cacheHeader.HashTable;
                
                if (hashTable.Mask == uint.MaxValue)
                    return false;

                uint hash = id & hashTable.Mask;
                var bucket = memory.Read<HashBucket>(hashTable.TablePtr + hash * 12);
                
                uint ptr = bucket.NextPtr;
                byte[] buffer = new byte[4 + bucket.EntrySize + 4];
                
                while ((ptr & 1) == 0 && ptr != 0)
                {
                    memory.ReadBytes(ptr, buffer);
                    
                    if (BitConverter.ToUInt32(buffer, 0) == id)
                    {
                        result = ptr;
                        return true;
                    }
                    
                    ptr = BitConverter.ToUInt32(buffer, buffer.Length - 4);
                }
                
                return false;
            }

            #region Internal Structs

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct CacheHeader
            {
                private uint _field0;
                private uint _field1;
                public HashTableInfo HashTable;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct HashTableInfo
            {
                private uint _field0;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
                private uint[] _reserved;
                public uint TablePtr;
                private uint _field2;
                public uint Mask;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct HashBucket
            {
                public uint EntrySize;
                private uint _field1;
                public uint NextPtr;
            }

            #endregion
        }
        public class InfoBlock
        {
            public uint Id { get; private set; }
            public uint Address { get; private set; }

            internal InfoBlock(uint address, uint id)
            {
                Address = address;
                Id = id;
            }
            public ItemCacheEntry Item
            {
                get
                {
                    if (Address == 0)
                        return default(ItemCacheEntry);
                    var wow = ObjectManager.Wow;
                    return wow != null ? wow.Read<ItemCacheEntry>(Address) : default;
                }
            }
            public QuestCacheEntry Quest
            {
                get
                {
                    if (Address == 0)
                        return default(QuestCacheEntry);
                    var wow = ObjectManager.Wow;
                    return wow != null ? wow.Read<QuestCacheEntry>(Address) : default;
                }
            }
            public GameObjectCacheEntry GameObject
            {
                get
                {
                    if (Address == 0)
                        return default(GameObjectCacheEntry);
                    var wow = ObjectManager.Wow;
                    return wow != null ? wow.Read<GameObjectCacheEntry>(Address) : default;
                }
            }
            public CreatureCacheEntry Creature
            {
                get
                {
                    if (Address == 0)
                        return default(CreatureCacheEntry);
                    var wow = ObjectManager.Wow;
                    return wow != null ? wow.Read<CreatureCacheEntry>(Address) : default;
                }
            }
        }

        #endregion

        #region Cache Entry Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct ItemCacheEntry
        {
            public int EntryLength;
            public int ClassId;
            public int SubClassId;
            public int Unk0;
            public int DisplayInfoId;
            public int Rarity;
            public int TypeFlags;
            public int BuyPrice;
            public int Faction;
            public int SellPrice;
            public int EquipSlot;
            public int AllowedClasses;
            public int AllowedRaces;
            public int ItemLevel;
            public int RequiredLevel;
            public int RequiredSkill;
            public int RequiredSkillLevel;
            public int RequireSpell;
            public int RequiredHonorRank;
            public int RequiredCityRank;
            public int RequiredReputationFaction;
            public int RequiredReputationRank;
            public int UniqueCount;
            public int MaxStackSize;
            public int BagSlots;
            public int NumberOfStats;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.I4)]
            public int[] StatId;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.I4)]
            public int[] StatValue;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I4)]
            public int[] Unk1;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.R4)]
            public float[] DamageMin;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.R4)]
            public float[] DamageMax;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I4)]
            public int[] DamageType;
            
            public int ResistPhysical;
            public int ResistHoly;
            public int ResistFire;
            public int ResistNature;
            public int ResistFrost;
            public int ResistShadow;
            public int ResistArcane;
            public int WeaponDelay;
            public int AmmoType;
            public float RangeModifier;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = UnmanagedType.I4)]
            public int[] SpellId;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = UnmanagedType.I4)]
            public int[] SpellTriggerId;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = UnmanagedType.I4)]
            public int[] SpellCharges;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = UnmanagedType.I4)]
            public int[] SpellCooldown;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = UnmanagedType.I4)]
            public int[] SpellCategory;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = UnmanagedType.I4)]
            public int[] SpellCategoryCooldown;
            
            public int BondId;
            public uint DescriptionPtr;
            public int BookTextId;
            public int BookPages;
            public int BookStationaryId;
            // HonorBuddy-compatible alias: some plugins expect StartQuestId
            public int StartQuestId { get { return this.BookStationaryId; } }
            // Note: HB decompiled had StartQuestId in ItemSparseEntry; map to BookStationaryId here
            public int BeginQuestId;
            public int LockPickSkillRequired;
            public int MaterialId;
            public int SheathId;
            public int RandomPropertyId;
            public int RandomPropertyId2;
            public int BlockValue;
            public int ItemSetId;
            public int DurabilityValue;
            public int ItemAreaId;
            public int ItemMapId;
            public int BagFamily;
            // HonorBuddy-compatible alias
            public int BagFamilyId { get { return BagFamily; } }
            public int TotemCategory;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I4)]
            public SocketColorFlags[] SocketColor;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I4)]
            public int[] SocketUnk;
            
            public int SocketBonus;
            public int GemProperties;
            public int DisenchantSkillLevel;
            public float ArmorDamageModifier;
            public int ItemExtendedCost;
            public int ItemLimitId;
            public int Unk2;
            public uint NamePtr;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct QuestCacheEntry
        {
            public uint Id;
            public uint Method;
            public uint Level;
            public uint RequiredLevel;
            public uint AreaIdOrSortId;
            [MarshalAs(UnmanagedType.U4)]
            public QuestInfoTagType InfoId;
            public uint SuggestedPlayers;
            public uint FriendlyFactionId;
            public uint FriendlyFactionAmount;
            public uint HostileFactionId;
            public uint HostileFactionAmount;
            public uint NextQuestId;
            public uint XpId;
            public uint RewardMoney;
            public uint RewardMoneyCompensation;
            public uint RewardSpellId;
            public uint SpellCastOnPlayer;
            public uint RewardHonor;
            public float RewardHonorMultiplier;
            public uint RelatedItemId;
            [MarshalAs(UnmanagedType.U4)]
            public QuestFlags Flags;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] RewardItem;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] RewardItemCount;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public int[] RewardChoiceItem;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public int[] RewardChoiceItemCount;
            
            public uint PointMapId;
            public float PointX;
            public float PointY;
            public uint PointOptional;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] Name;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3000)]
            public byte[] ObjectiveText;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3000)]
            public byte[] Description;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] SubDescription;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] ObjectiveId;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] ObjectiveRequiredCount;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public int[] CollectItemId;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public int[] CollectItemCount;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] IntermediateItemId;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] IntermediateItemCount;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public byte[] ObjectiveTexts;
            
            public uint RewardTitleId;
            public uint RequiredPlayersKilled;
            public uint RewardTalentPoints;
            public uint RewardArenaPoints;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)]
            public byte[] CompletionText;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public uint[] RewardReputationFactions;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public uint[] RewardReputationValueId;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public uint[] RewardReputationOverride;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct GameObjectCacheEntry
        {
            public int GameObjectType;
            public int DisplayId;
            public uint CastBarCaptionPtr;
            public uint UnkString0Ptr;
            public uint UnkString1Ptr;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public int[] Properties;
            
            public float Scale;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public int[] QuestItems;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] NamePtrs;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct CreatureCacheEntry
        {
            private uint _reserved0;
            public uint SubNamePtr;
            public uint IconNamePtr;
            [MarshalAs(UnmanagedType.U4)]
            public CreatureCacheTypeFlags TypeFlags;
            public uint TypeID;
            public uint FamilyID;
            public uint Rank;
            public uint GroupID;
            public uint GroupID2;
            public uint SpellDataID;
            public uint ModelID1;
            public uint ModelID2;
            public uint ModelID3;
            public float HealthModifier;
            public float ManaModifier;
            public uint RacialLeader;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public uint[] QuestItems;
            
            public uint MovementID;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] NamePtrs;
        }

        #endregion

        #region Enums
        [Flags]
        public enum SocketColorFlags
        {
            None = 0,
            Meta = 1,
            Red = 2,
            Yellow = 4,
            Blue = 8
        }
        public enum QuestInfoTagType : uint
        {
            None = 0,
            Group = 1,
            Life = 21,
            PvP = 41,
            Raid = 62,
            Dungeon = 81,
            WorldEvent = 82,
            Legendary = 83,
            Escort = 84,
            Heroic = 85,
            Raid10 = 88,
            Raid25 = 89
        }
        [Flags]
        public enum QuestFlags : uint
        {
            None = 0x0,
            StayAlive = 0x1,
            PartyQuest = 0x2,
            Shareable = 0x8,
            Daily = 0x1000,
            FlagsPVP = 0x2000,
            Weekly = 0x8000,
            AutoAccept = 0x80000,
            All = 0xFFFFFFFF
        }
        [Flags]
        public enum CreatureCacheTypeFlags
        {
            Tameable = 1,
            GhostVisible = 2,
            HerbSkin = 256,
            MineSkin = 512,
            Salvageable = 32768,
            Exotic = 65536
        }

        #endregion
    }
}
