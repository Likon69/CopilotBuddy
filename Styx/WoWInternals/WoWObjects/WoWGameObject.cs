using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Numerics;
using System.Runtime.InteropServices;
using GreenMagic;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.Patchables;
using Styx.WoWInternals.WoWCache;
using Matrix = Tripper.Tools.Math.Matrix;

namespace Styx.WoWInternals.WoWObjects
{
    public class WoWGameObject : WoWObject, IComparable<WoWGameObject>, IComparable<WoWUnit>, IComparer<WoWUnit>, IComparer<WoWGameObject>, ILootableObject
    {
        #region Constants - Offsets 3.3.5a
        
        // Position offsets for GameObjects (different from Unit)
        private const uint GO_POSITION_X_OFFSET = 0xE8;     // 232
        private const uint GO_POSITION_Y_OFFSET = 0xEC;     // 236
        private const uint GO_POSITION_Z_OFFSET = 0xF0;     // 240
        private const uint GO_ROTATION_OFFSET = 0xF8;       // 248
        
        #endregion
        
        #region Descriptor Offsets - GAMEOBJECT_FIELD
        
        // GameObjectFields pour 3.3.5a (Offsets.txt ligne 4989-4997: indices × 4)
        // Source: Offsets.txt WoWGameObjectFields
        // Note: GetDescriptorField() attend des byte offsets, pas des indices
        private const int GAMEOBJECT_CREATED_BY = 0x6 * 4;   // 24 bytes - Owner GUID (8 bytes ulong)
        private const int GAMEOBJECT_DISPLAYID = 0x8 * 4;    // 32 bytes - Display ID
        private const int GAMEOBJECT_FLAGS = 0x9 * 4;        // 36 bytes - Flags
        private const int GAMEOBJECT_PARENTROTATION = 0xA * 4; // 40 bytes - Quaternion (16 bytes, 4 floats)
        private const int GAMEOBJECT_DYNAMIC = 0xE * 4;      // 56 bytes - Dynamic flags (2 bytes)
        private const int GAMEOBJECT_FACTION = 0xF * 4;      // 60 bytes - Faction template
        private const int GAMEOBJECT_LEVEL = 0x10 * 4;       // 64 bytes - Level
        private const int GAMEOBJECT_BYTES_1 = 0x11 * 4;     // 68 bytes - State, Type, ArtKit, Anim
        
        #endregion
        
        #region Slot Mapping Table — WotLK 3.3.5a (HB 3.3.5a port)

        /// <summary>
        /// Memory layout of one GO type slot-mapping entry (HB 3.3.5a Struct64).
        /// 36 of these are read from 0xA38F90; each maps a GO type to its slot ID array.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct SlotMappingEntry
        {
            public uint Unknown;
            public uint SlotCount;
            public uint SlotArrayAddress;
            private uint _padding;
        }

        /// <summary>
        /// Per-GO-type slot mapping: _slotMappingTable[goType][i] == dataSlot
        /// means Properties[i] holds the value for that dataSlot.
        /// Lazy-loaded on first GetDataSlot() call when ObjectManager.Wow is available.
        /// </summary>
        private static uint[][]? _slotMappingTable;

        #endregion

        #region Constructor
        public WoWGameObject(uint baseAddress) : base(baseAddress)
        {
        }
        
        #endregion
        
        #region Position Override
        public override float X
        {
            get
            {
                if (Memory == null) return 0f;
                return Memory.Read<float>(BaseAddress + GO_POSITION_X_OFFSET);
            }
        }
        public override float Y
        {
            get
            {
                if (Memory == null) return 0f;
                return Memory.Read<float>(BaseAddress + GO_POSITION_Y_OFFSET);
            }
        }
        public override float Z
        {
            get
            {
                if (Memory == null) return 0f;
                return Memory.Read<float>(BaseAddress + GO_POSITION_Z_OFFSET);
            }
        }
        public override WoWPoint Location
        {
            get
            {
                return new WoWPoint(X, Y, Z);
            }
        }
        public override float Rotation
        {
            get
            {
                if (Memory == null) return 0f;
                return Memory.Read<float>(BaseAddress + GO_ROTATION_OFFSET);
            }
        }
        
        #endregion
        
        #region Descriptor Properties
        public ulong CreatedByGuid => GetDescriptorField<ulong>(GAMEOBJECT_CREATED_BY);
        public WoWUnit? CreatedBy => ObjectManager.GetObjectByGuid<WoWUnit>(CreatedByGuid);
        public uint DisplayId => GetDescriptorField<uint>(GAMEOBJECT_DISPLAYID);
        public GameObjectFlags Flags => (GameObjectFlags)GetDescriptorField<uint>(GAMEOBJECT_FLAGS);
        public uint DynamicFlags => GetDescriptorField<uint>(GAMEOBJECT_DYNAMIC);
        public uint Faction => GetDescriptorField<uint>(GAMEOBJECT_FACTION);
        public int Level => GetDescriptorField<int>(GAMEOBJECT_LEVEL);
        public uint Bytes1 => GetDescriptorField<uint>(GAMEOBJECT_BYTES_1);
        public uint FlagsDynamic => DynamicFlags;
        public uint FactionTemplateId => Faction;
        public WoWFactionTemplate? FactionTemplate => WoWFactionTemplate.FromId(FactionTemplateId);
        
        #endregion
        
        #region State Properties (from Bytes1)
        public WoWGameObjectState State
        {
            get
            {
                return (WoWGameObjectState)(Bytes1 & 0xFF);
            }
        }
        public WoWGameObjectType SubType
        {
            get
            {
                return (WoWGameObjectType)((Bytes1 >> 8) & 0xFF);
            }
        }

        public Vector4 ParentRotation
        {
            get
            {
                float x = GetDescriptorField<float>(GAMEOBJECT_PARENTROTATION + 0);
                float y = GetDescriptorField<float>(GAMEOBJECT_PARENTROTATION + 4);
                float z = GetDescriptorField<float>(GAMEOBJECT_PARENTROTATION + 8);
                float w = GetDescriptorField<float>(GAMEOBJECT_PARENTROTATION + 12);
                return new Vector4(x, y, z, w);
            }
        }

        /// <summary>
        /// Sub-object associated with this GameObject (chair, door, bobber, etc.).
        /// HB-compatible API: returns a `WoWSubObject` so callers can cast to specific sub-types
        /// (e.g. `WoWFishingBobber`). When a real sub-object pointer is unavailable we return
        /// an inert `WoWSubObject` with a zero base address to preserve runtime safety.
        /// </summary>
        public WoWSubObject SubObj
        {
            get
            {
                try
                {
                    Memory? wow = ObjectManager.Wow;
                    if (wow == null)
                        return new WoWSubObject(0u);

                    // HB 3.3.5a: sub-object pointer stored at offset +416
                    const uint SUBOBJECT_PTR_OFFSET = 416u;
                    uint ptr = wow.Read<uint>(BaseAddress + SUBOBJECT_PTR_OFFSET);
                    if (ptr == 0u)
                        return new WoWSubObject(0u);

                    switch (SubType)
                    {
                        case WoWGameObjectType.Door:
                            return new WoWDoor(ptr);
                        case WoWGameObjectType.Chair:
                            return new WoWChair(ptr);
                        case WoWGameObjectType.FishingBobber:
                            return new WoWFishingBobber(ptr);
                        // Many GO types are represented by an animated/generic subobject in HB
                        // Use WoWAnimatedSubObject where appropriate if a specialized class is not present
                        case WoWGameObjectType.Button:
                        case WoWGameObjectType.QuestGiver:
                        case WoWGameObjectType.Chest:
                        case WoWGameObjectType.Generic:
                        case WoWGameObjectType.Trap:
                        case WoWGameObjectType.SpellFocus:
                        case WoWGameObjectType.Text:
                        case WoWGameObjectType.Goober:
                        case WoWGameObjectType.AreaDamage:
                        case WoWGameObjectType.DuelFlag:
                        case WoWGameObjectType.Ritual:
                        case WoWGameObjectType.Mailbox:
                        case WoWGameObjectType.SpellCaster:
                        case WoWGameObjectType.MeetingStone:
                        case WoWGameObjectType.FlagStand:
                        case WoWGameObjectType.FishingHole:
                        case WoWGameObjectType.FlagDrop:
                        case WoWGameObjectType.MiniGame:
                        case WoWGameObjectType.CapturePoint:
                        case WoWGameObjectType.GuildBank:
                            return new WoWAnimatedSubObject(ptr);
                        default:
                            return new WoWSubObject(ptr);
                    }
                }
                catch
                {
                    return new WoWSubObject(0u);
                }
            }
        }
        public byte ArtKit
        {
            get
            {
                return (byte)((Bytes1 >> 16) & 0xFF);
            }
        }
        public byte AnimationProgress
        {
            get
            {
                return (byte)((Bytes1 >> 24) & 0xFF);
            }
        }
        
        #endregion
        
        #region Helper Properties
        public bool Locked => (Flags & GameObjectFlags.Locked) != 0;
        public bool InUse => (Flags & GameObjectFlags.InUse) != 0;
        public bool Triggered => (Flags & GameObjectFlags.Triggered) != 0;
        public bool Transport => (Flags & GameObjectFlags.Transport) != 0;
        public bool IsTransport => Transport;
        
        // HB 4.3.4 style: IsHerb and IsMineral use LockType, not SubType
        public bool IsHerb => LockType == WoWLockType.Herbalism;
        public bool IsMineral => Entry != 185877U && LockType == WoWLockType.Mining;
        
        public WoWLockType LockType
        {
            get
            {
                LockEntry? lockRecord = LockRecord;
                return lockRecord.HasValue && lockRecord.Value.Type[0] == 2 
                    ? (WoWLockType)lockRecord.Value.LockProperties[0] 
                    : WoWLockType.None;
            }
        }
        
        internal LockEntry? LockRecord
        {
            get
            {
                if (!GetDataSlot(GameObjectDataSlot.LockId, out int lockId))
                    return null;
                if (lockId == 0)
                    return null;

                var db = StyxWoW.Db[Patchables.ClientDb.Lock];
                if (db == null)
                    return null;

                var row = db.GetRow((uint)lockId);
                if (row == null || !row.IsValid)
                    return null;

                return row.GetStruct<LockEntry>();
            }
        }
        
        public bool CanLoot
        {
            get
            {
                // Check herbs (herbalism skill)
                if (IsHerb)
                {
                    if (State != WoWGameObjectState.Ready) return false; // Already harvested.
                    WoWSkill? skill = ObjectManager.Me.GetSkill(SkillLine.Herbalism);
                    if (skill == null) return false;
                    int effectiveSkill = skill.CurrentValue;
                    // Tauren racial: +15 Herbalism
                    if (StyxWoW.Me.Race == WoWRace.Tauren)
                        effectiveSkill += 15;
                    uint? required = RequiredSkill;
                    return !required.HasValue || required.Value == 0 || effectiveSkill >= required.Value;
                }
                // Check minerals (mining skill)
                else if (IsMineral)
                {
                    if (State != WoWGameObjectState.Ready) return false; // Already mined.
                    WoWSkill? skill = ObjectManager.Me.GetSkill(SkillLine.Mining);
                    if (skill == null) return false;
                    uint? required = RequiredSkill;
                    return !required.HasValue || required.Value == 0 || skill.CurrentValue >= required.Value;
                }
                // Check locked chests (lockpicking skill)
                else if (IsChest && Locked)
                {
                    WoWSkill? skill = ObjectManager.Me.GetSkill(SkillLine.Lockpicking);
                    if (skill != null && skill.MaxValue > 0)
                    {
                        uint? required = RequiredSkill;
                        if (!required.HasValue || required.Value == 0 || skill.CurrentValue >= required.Value)
                            return true;
                    }
                    return false;
                }
                
                // Fallback for quest GameObjects (Goobers, unlocked chests, etc.):
                // Check DynamicFlags: bit 0 = activatable, bit 3 = has loot/sparkle
                // (FlagsDynamic & 9) == 9 means both flags are set
                return (FlagsDynamic & 9) == 9;
            }
        }
        
        // HB 4.3.4: CanMine just checks LockType (no State check in HB)
        public bool CanMine => IsMineral;
        
        // HB 4.3.4: CanHarvest just checks LockType (no State check in HB)
        public bool CanHarvest => IsHerb;
        
        public bool CanFish => SubType == WoWGameObjectType.FishingBobber;
        
        #endregion
        
        #region Type Helpers
        // HB 4.3.4: IsChest checks a dictionary of chest entries, simplified here.
        // Herb nodes and mineral nodes also use SubType=3 (Chest) in WoW's DB, so exclude them.
        public bool IsChest => SubType == WoWGameObjectType.Chest && !IsHerb && !IsMineral;
        public bool IsDoor => SubType == WoWGameObjectType.Door;
        public bool IsButton => SubType == WoWGameObjectType.Button;
        public bool IsQuestGiver => SubType == WoWGameObjectType.QuestGiver;
        public bool IsMailbox => SubType == WoWGameObjectType.Mailbox;
        /// <summary>
        /// Gets the model file path from GameObjectDisplayInfo DBC.
        /// BUG-23 fix: Was returning Name instead of actual model path.
        /// </summary>
        public string Model
        {
            get
            {
                uint displayId = DisplayId;
                if (displayId == 0)
                    return Name;

                try
                {
                    var db = StyxWoW.Db[Patchables.ClientDb.GameObjectDisplayInfo];
                    if (db == null)
                        return Name;

                    var row = db.GetRow(displayId);
                    if (row == null)
                        return Name;

                    uint strPtr = row.GetField<uint>(1U);
                    if (strPtr == 0)
                        return Name;

                    return ObjectManager.Wow.Read<string>(strPtr) ?? Name;
                }
                catch
                {
                    return Name;
                }
            }
        }
        public WoWSpellFocus SpellFocus
        {
            get
            {
                if (GetDataSlot(GameObjectDataSlot.SpellFocusId, out int id))
                    return (WoWSpellFocus)id;
                return WoWSpellFocus.None;
            }
        }
        public override float InteractRange => Math.Max(0f, SubObj.InteractDistance - 0.25f);
        
        public WoWUnitReaction GetReactionTowards(WoWUnit unit)
        {
            var creator = CreatedBy;
            if (creator != null)
                return creator.GetReactionTowards(unit);
            
            var factionTemplate = FactionTemplate;
            if (factionTemplate == null)
                return WoWUnitReaction.Neutral;
            return factionTemplate.GetReactionTowards(unit);
        }

        /// <summary>
        /// Gets the required skill level to interact with this object (herbs, minerals, chests).
        /// </summary>
        public uint? RequiredSkill
        {
            get
            {
                if (IsHerb || IsMineral || IsChest)
                {
                    LockEntry? lockRecord = LockRecord;
                    if (lockRecord.HasValue)
                        return lockRecord.Value.RequiredSkill[0];
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the cached game object info from WoW client memory.
        /// Reads the cache entry pointer at BaseAddress + 0x1A4 (420 decimal).
        /// Used by IsHerb, IsMineral, GetDataSlot, LockRecord.
        /// </summary>
        public bool GetCachedInfo(out WoWCache.WoWCache.GameObjectCacheEntry info)
        {
            Memory? wow = ObjectManager.Wow;
            if (wow == null)
            {
                info = default;
                return false;
            }

            uint cachePtr = wow.Read<uint>(BaseAddress + 420U); // 0x1A4
            if (cachePtr != 0U)
            {
                info = wow.Read<WoWCache.WoWCache.GameObjectCacheEntry>(cachePtr);
                return true;
            }

            info = default;
            return false;
        }
        
        #endregion
        
        #region IComparable Implementation
        public int CompareTo(WoWGameObject? other)
        {
            if (other == null) return 1;
            return Distance.CompareTo(other.Distance);
        }
        public int CompareTo(WoWUnit? other)
        {
            if (other == null) return 1;
            return Distance.CompareTo(other.Distance);
        }

        public int Compare(WoWUnit? x, WoWUnit? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;
            return x.Distance.CompareTo(y.Distance);
        }

        public int Compare(WoWGameObject? x, WoWGameObject? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;
            return x.Distance.CompareTo(y.Distance);
        }

        public bool CanUse() => !Locked && !InUse;
        public bool CanUseNow() => CanUse();
        public bool CanUseNow(out GameError reason)
        {
            reason = 0; // No error
            if (Locked)
            {
                reason = GameError.ChestInUse;
                return false;
            }
            if (InUse)
            {
                reason = GameError.ChestInUse;
                return false;
            }
            return true;
        }

        public bool GetDataSlot(GameObjectDataSlot slot, out int value)
        {
            return GetDataSlot((uint)slot, out value);
        }

        public bool GetDataSlot(GameObjectDataSlot slot, out bool value)
        {
            int temp;
            bool result = GetDataSlot((uint)slot, out temp);
            value = temp != 0;
            return result;
        }

        public bool GetDataSlot(uint dataSlot, out int value)
        {
            // HB 3.3.5a port: look up which Properties[] index corresponds to the
            // requested dataSlot using the per-GO-type slot mapping table at 0xA38F90.
            WoWCache.WoWCache.GameObjectCacheEntry entry;
            if (!GetCachedInfo(out entry))
            {
                value = 0;
                return false;
            }

            WoWGameObjectType subType = SubType;
            if (subType >= (WoWGameObjectType)36)
            {
                value = 0;
                return false;
            }

            if (_slotMappingTable == null)
                LoadSlotMappingTable();

            // If memory was unavailable, table stays null — bail and retry next pulse.
            if (_slotMappingTable == null)
            {
                value = 0;
                return false;
            }

            uint[] typeSlots = _slotMappingTable[(int)subType];
            for (int i = 0; i < typeSlots.Length; i++)
            {
                if (typeSlots[i] == dataSlot)
                {
                    if (i >= entry.Properties.Length)
                    {
                        value = 0;
                        return false;
                    }
                    value = entry.Properties[i];
                    return true;
                }
            }

            value = 0;
            return false;
        }

        /// <summary>
        /// Reads the 36-entry slot mapping table from WoW memory at 0xA38F90.
        /// Each GO type has its own uint[] of slot IDs that map to Properties[] indices.
        /// Only sets _slotMappingTable on success — stays null on failure so next call retries.
        /// </summary>
        private static void LoadSlotMappingTable()
        {
            const uint SlotMappingTableAddress = 10710832U; // 0xA38F90 — WotLK 3.3.5a
            const int GameObjectTypeCount = 36;
            const int MaxSlotsPerType = 24; // Properties[] SizeConst

            Memory? memory = ObjectManager.Wow;
            if (memory == null)
                return; // Will retry next call

            try
            {
                var table = new uint[GameObjectTypeCount][];
                var entries = memory.ReadStructArray<SlotMappingEntry>(SlotMappingTableAddress, GameObjectTypeCount);

                for (int i = 0; i < GameObjectTypeCount; i++)
                {
                    int slotCount = (int)entries[i].SlotCount;
                    if (slotCount > MaxSlotsPerType)
                        slotCount = MaxSlotsPerType;

                    table[i] = memory.ReadStructArray<uint>(entries[i].SlotArrayAddress, slotCount);
                }

                _slotMappingTable = table; // Atomic assignment — only when fully loaded
            }
            catch (Exception ex)
            {
                Helpers.Logging.Write("[WoWGameObject] Failed to load slot mapping table: {0}", ex.Message);
                // Leave _slotMappingTable null so next call retries
            }
        }

        public Matrix WorldMatrix => GetWorldMatrix();

        /// <summary>
        /// World transform matrix. For transports (elevators, ships), reads the live
        /// matrix from game memory at BaseAddress + 0x1A8 (updated each frame by the client).
        /// For static objects, computes from position + rotation.
        /// Matches HB 3.3.5a's GetWorldMatrix().
        /// </summary>
        public Matrix GetWorldMatrix()
        {
            // Transports (type 15 = TRANSPORT, type 11 = MO_TRANSPORT) have a live matrix
            // maintained by the client at offset 0x1A8 (424 bytes from base).
            if (SubType == WoWGameObjectType.Transport || SubType == WoWGameObjectType.MapObjectTransport)
            {
                Memory? wow = ObjectManager.Wow;
                if (wow != null)
                {
                    return wow.Read<Matrix>(BaseAddress + 0x1A8);
                }
            }
            return BuildWorldMatrix(Location, Rotation);
        }

        public Matrix GetWorldMatrix(bool includeScale)
        {
            return GetWorldMatrix();
        }

        #endregion
        
        #region ToString
        public override string ToString()
        {
            return $"[GameObject: {Name} (Entry: {Entry}, Type: {SubType}, State: {State}, Distance: {Distance:F1})]";
        }

        private static Matrix4x4 BuildWorldMatrix(WoWPoint location, float rotation)
        {
            var rotationZ = Matrix4x4.CreateRotationZ(rotation);
            rotationZ.Translation = new Vector3(location.X, location.Y, location.Z);
            return rotationZ;
        }

        #endregion
    }

    /// <summary>
    /// Minimal cached gameobject sub-data (distance, focus id, model id).
    /// </summary>
    public sealed class GameObjectSubData
    {
        public float InteractDistance { get; set; }
        public int SpellFocusId { get; set; }
        public int ModelId { get; set; }
    }
    
    #region Enums
    [Flags]
    public enum GameObjectFlags : uint
    {
        None = 0x0,
        InUse = 0x1,           // Est utilisé par quelqu'un
        Locked = 0x2,          // Verrouillé (besoin de clé ou lockpicking)
        ConditionInteract = 0x4,
        Transport = 0x8,       // Est un transport (bateau, zeppelin)
        NotSelectable = 0x10,  // Non sélectionnable
        NoDespawn = 0x20,      // Ne disparaît pas
        Triggered = 0x40,      // A été déclenché
        Damaged = 0x200,       // Endommagé
        Destroyed = 0x400      // Détruit
    }
    public enum WoWGameObjectState : byte
    {
        Active = 0,
        Ready = 1,
        ActiveAlternative = 2,
        Destroyed = 3
    }

    /// <summary>
    /// Lock entry from Lock.dbc - used for lockpicking/mining/herbalism requirements.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 56)]
    public struct LockEntry
    {
        public int Id;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public int[] Type;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public uint[] LockProperties;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public uint[] RequiredSkill;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public uint[] Action;
    }
    
    #endregion
}
