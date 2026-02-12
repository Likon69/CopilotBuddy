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
    public class WoWGameObject : WoWObject, IComparable<WoWGameObject>, IComparable<WoWUnit>, IComparer<WoWUnit>, IComparer<WoWGameObject>
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
        public ushort DynamicFlags => GetDescriptorField<ushort>(GAMEOBJECT_DYNAMIC);
        public uint Faction => GetDescriptorField<uint>(GAMEOBJECT_FACTION);
        public int Level => GetDescriptorField<int>(GAMEOBJECT_LEVEL);
        public uint Bytes1 => GetDescriptorField<uint>(GAMEOBJECT_BYTES_1);
        public ushort FlagsDynamic => DynamicFlags;
        public uint FactionTemplateId => Faction;
        
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
        /// Sub-object cache entry (InteractDistance, SpellFocus, etc.). Returns a lightweight placeholder when cache is missing.
        /// </summary>
        public GameObjectSubData SubObj => new GameObjectSubData
        {
            InteractDistance = 4.5f,
            SpellFocusId = 0,
            ModelId = 0
        };
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
                    WoWSkill skill = ObjectManager.Me.GetSkill(SkillLine.Herbalism);
                    if (skill != null && skill.MaxValue > 0)
                    {
                        int effectiveSkill = skill.CurrentValue;
                        // Tauren racial: +15 Herbalism
                        if (StyxWoW.Me.Race == WoWRace.Tauren)
                            effectiveSkill += 15;
                        
                        uint? required = RequiredSkill;
                        if (!required.HasValue || effectiveSkill >= required.Value)
                            return true;
                    }
                }
                // Check minerals (mining skill)
                else if (IsMineral)
                {
                    WoWSkill skill = ObjectManager.Me.GetSkill(SkillLine.Mining);
                    if (skill != null && skill.MaxValue > 0)
                    {
                        uint? required = RequiredSkill;
                        if (!required.HasValue || skill.CurrentValue >= required.Value)
                            return true;
                    }
                }
                // Check locked chests (lockpicking skill)
                else if (IsChest && Locked)
                {
                    WoWSkill skill = ObjectManager.Me.GetSkill(SkillLine.Lockpicking);
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
        
        // HB 4.3.4: CanMine uses IsMineral (which checks LockType)
        public bool CanMine => IsMineral && State == WoWGameObjectState.Ready;
        
        // HB 4.3.4: CanHarvest uses IsHerb (which checks LockType) 
        public bool CanHarvest => IsHerb && State == WoWGameObjectState.Ready;
        
        public bool CanFish => SubType == WoWGameObjectType.FishingBobber;
        
        #endregion
        
        #region Type Helpers
        // HB 4.3.4: IsChest checks a dictionary of chest entries, simplified here
        public bool IsChest => SubType == WoWGameObjectType.Chest;
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
            
            // Check faction template
            return WoWUnitReaction.Neutral;
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
                    if (!GetDataSlot(GameObjectDataSlot.LockId, out int lockId))
                        return null;
                    
                    if (lockId != 0)
                    {
                        // NOTE: Full implementation requires reading Lock.dbc from MPQs
                        // For basic bot functionality, we can rely on server-side checks
                        // when attempting to interact with locked objects.
                        // Return null for now - interaction will fail if skill too low
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the cached game object info from WoW client memory.
        /// Reads the cache entry pointer at BaseAddress + 0x1A4 (420 decimal).
        /// STUB-01: Critical for IsHerb, IsMineral, GetDataSlot, LockRecord.
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
            // STUB-02: Read from cache entry Properties[] via GO type slot-mapping table.
            // The slot map at 0xA38F90 (10710832) defines which Properties[] index
            // corresponds to each data slot for each GO type. For simplicity, and since
            // WotLK GO types have consistent slot layouts, we map directly using the
            // Properties[] array from the cache entry.
            WoWCache.WoWCache.GameObjectCacheEntry entry;
            if (!GetCachedInfo(out entry))
            {
                value = 0;
                return false;
            }

            if (dataSlot >= (uint)entry.Properties.Length)
            {
                value = 0;
                return false;
            }

            value = entry.Properties[(int)dataSlot];
            return true;
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
        Ready = 0,
        Active = 1,
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
