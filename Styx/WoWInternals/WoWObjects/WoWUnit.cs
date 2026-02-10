using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using GreenMagic;
using Styx.Combat.CombatRoutine;
using Styx.Common;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.WoWInternals.WoWCache;

namespace Styx.WoWInternals.WoWObjects
{
    public class WoWUnit : WoWObject, ILootableObject
    {
        #region Constructors

        public WoWUnit(uint baseAddress) : base(baseAddress)
        {
        }

        #endregion

        #region Static Dictionaries (Reaction Cache)

        private static readonly Dictionary<uint, WoWUnitReaction> HardcodedReactions = new Dictionary<uint, WoWUnitReaction>
        {
            { 6, WoWUnitReaction.Neutral },
            { 257, WoWUnitReaction.Neutral },
            { 80, WoWUnitReaction.Neutral },
            { 1867, WoWUnitReaction.Hostile }
        };

        private static readonly Dictionary<uint, WoWUnitReaction> HordeReactionsByEntry = new Dictionary<uint, WoWUnitReaction>
        {
            { 1867, WoWUnitReaction.Hostile }
        };

        private static readonly Dictionary<uint, WoWUnitReaction> HordeReactionsByFaction = new Dictionary<uint, WoWUnitReaction>
        {
            { 76, WoWUnitReaction.Hostile }
        };

        private static readonly Dictionary<uint, WoWUnitReaction> ReactionCache = new Dictionary<uint, WoWUnitReaction>();

        #endregion

        #region Instance Reaction Caches (per unit)

        /// <summary>
        /// Instance-level cache for reactions by entry ID.
        /// </summary>
        private readonly Dictionary<uint, WoWUnitReaction> _reactionCacheByEntry = new Dictionary<uint, WoWUnitReaction>();

        /// <summary>
        /// Instance-level cache for reactions by GUID (for units without entry).
        /// </summary>
        private readonly Dictionary<ulong, WoWUnitReaction> _reactionCacheByGuid = new Dictionary<ulong, WoWUnitReaction>();

        #endregion

        #region Display Flags & Dynamic Flags

        public uint DisplayFlags
        {
            get
            {
                if (BaseAddress == 0)
                    return 0;

                Memory? wow = ObjectManager.Wow;
                if (wow == null)
                    return 0;

                return wow.Read<uint>(BaseAddress + 2608);
            }
        }

        private BitVector32 NpcFlags => GetDescriptor<BitVector32>(UnitFields.NpcFlags);

        private BitVector32 DynamicFlags => GetDescriptor<BitVector32>(UnitFields.DynamicFlags);

        internal UnitDynamicFlags DynFlags => (UnitDynamicFlags)GetDescriptor<uint>(UnitFields.DynamicFlags);

        #endregion

        #region NPC Type Properties

        public bool CanGossip => HasNpcFlag(UnitNPCFlags.Gossip);
        public bool IsQuestGiver => HasNpcFlag(UnitNPCFlags.Questgiver);
        public bool IsTrainer => HasNpcFlag(UnitNPCFlags.Trainer);
        public bool IsClassTrainer => HasNpcFlag(UnitNPCFlags.ClassTrainer);
        public bool IsProfessionTrainer => HasNpcFlag(UnitNPCFlags.ProfessionTrainer);
        public bool IsAnyTrainer => HasNpcFlag(UnitNPCFlags.AnyTrainer);
        public bool IsAnyVendor => HasNpcFlag(UnitNPCFlags.AnyVendor);
        public bool IsVendor => HasNpcFlag(UnitNPCFlags.Vendor);
        public bool IsAmmoVendor => HasNpcFlag(UnitNPCFlags.AmmoVendor);
        public bool IsFoodVendor => HasNpcFlag(UnitNPCFlags.FoodVendor);
        public bool IsPoisonVendor => HasNpcFlag(UnitNPCFlags.PoisionVendor);
        public bool IsPetitioner => HasNpcFlag(UnitNPCFlags.Petitioner);
        public bool IsReagentVendor => HasNpcFlag(UnitNPCFlags.ReagentVendor);
        public bool IsRepairMerchant => HasNpcFlag(UnitNPCFlags.Repair);
        public bool IsFlightMaster => HasNpcFlag(UnitNPCFlags.Flightmaster);
        public bool IsSpiritHealer => HasNpcFlag(UnitNPCFlags.Spirithealer);
        public bool IsSpiritGuide => HasNpcFlag(UnitNPCFlags.Spiritguide);
        public bool IsInnkeeper => HasNpcFlag(UnitNPCFlags.Innkeeper);
        public bool IsBanker => HasNpcFlag(UnitNPCFlags.Banker);
        public bool IsTabardDesigner => HasNpcFlag(UnitNPCFlags.TarbardDesigner);
        public bool IsBattleMaster => HasNpcFlag(UnitNPCFlags.Battlemaster);
        public bool IsAuctioneer => HasNpcFlag(UnitNPCFlags.Auctioneer);
        public bool IsStableMaster => HasNpcFlag(UnitNPCFlags.Stablemaster);
        public bool IsGuard => HasNpcFlag(UnitNPCFlags.Guard);
        public bool IsGuildBanker => HasNpcFlag(UnitNPCFlags.GuildBanker);

        #endregion

        #region Dynamic Flags Properties

        [Obsolete("Use TaggedByOther instead.")]
        public bool Tagged => TaggedByOther;

        public bool TaggedByOther => HasDynamicFlag(UnitDynamicFlags.TaggedByOther);
        public bool TaggedByMe => HasDynamicFlag(UnitDynamicFlags.TaggedByMe);
        public bool CanLoot => HasDynamicFlag(UnitDynamicFlags.Lootable);
        public bool Tracked => HasDynamicFlag(UnitDynamicFlags.TrackUnit);
        public bool Dead => CurrentHealth == 0;
        
        /// <summary>
        /// Returns true if the unit is dead. From HB 5.4.8+
        /// Checks health, and for non-players also checks the Dead dynamic flag.
        /// </summary>
        public bool IsDead
        {
            get
            {
                if (CurrentHealth == unchecked((int)0xFFFFFFFF))
                    return true;
                
                if (this is WoWPlayer)
                    return CurrentHealth == 0;
                
                // For units: also check Dead dynamic flag (0x20 in 3.3.5a)
                return CurrentHealth == 0 || HasDynamicFlag(UnitDynamicFlags.Dead);
            }
        }
        
        public bool RafLinked => HasDynamicFlag(UnitDynamicFlags.ReferAFriendLinked);
        public bool TappedByAllThreatLists => HasDynamicFlag(UnitDynamicFlags.TappedByAllThreatLists);

        #endregion

        #region Unit Flags

        private BitVector32 Flags => GetDescriptor<BitVector32>(UnitFields.Flags);
        internal UnitFlags UFlags => (UnitFlags)GetDescriptor<uint>(UnitFields.Flags);

        public bool Combat => HasUnitFlag(UnitFlags.InCombat);
        public bool Skinnable => HasUnitFlag(UnitFlags.Skinnable);
        public bool Dazed => HasUnitFlag(UnitFlags.Dazed);
        public bool Disarmed => HasUnitFlag(UnitFlags.Disarmed);
        public bool Attackable => !HasUnitFlag(UnitFlags.NotAttackable);
        public bool PvpFlagged => HasUnitFlag(UnitFlags.PvpEnabling);
        public bool PlayerControlled => HasUnitFlag(UnitFlags.PlayerControlled);
        public bool Fleeing => HasUnitFlag(UnitFlags.Fleeing);
        public bool Pacified => HasUnitFlag(UnitFlags.Pacified);
        public bool Stunned => HasUnitFlag(UnitFlags.Stunned);
        public bool Rooted => HasUnitFlag(UnitFlags.Rooted);
        public bool CanSelect => !HasUnitFlag(UnitFlags.NotSelectable);
        public bool Silenced => HasUnitFlag(UnitFlags.Silenced);
        public bool Possessed => HasUnitFlag(UnitFlags.Possessed);
        public bool Elite => HasUnitFlag(UnitFlags.PlusMob);
        
        /// <summary>
        /// Gets whether this unit is a boss (level cranked or rare elite).
        /// </summary>
        public bool IsBoss => Elite && (Level == -1 || Level > StyxWoW.Me.Level + 5);
        
        public bool Looting => HasUnitFlag(UnitFlags.Looting);
        public bool PetInCombat => HasUnitFlag(UnitFlags.PetInCombat);
        public bool OnTaxi => HasUnitFlag(UnitFlags.OnTaxi);

        public virtual bool Mounted => MountDisplayId > 0;

        #endregion

        #region Unit Flags 2

        private BitVector32 Flags2 => GetDescriptor<BitVector32>(UnitFields.Flags2);

        public bool FeignDeathed => HasUnitFlag2(UnitFlags2.FeignDeath);

        #endregion

        #region Bytes0 (Race, Class, Gender, Power)

        private byte[] Bytes0 => BitConverter.GetBytes(GetDescriptor<uint>(UnitFields.Bytes0));

        public WoWClass Class => (WoWClass)Bytes0[1];
        public WoWRace Race => (WoWRace)Bytes0[0];
        public WoWPowerType PowerType => (WoWPowerType)Bytes0[3];
        public WoWGender Gender => (WoWGender)Bytes0[2];

        #endregion

        #region Bytes1 (Stand State, Pet Talents, etc.)

        internal byte[] Bytes1 => BitConverter.GetBytes(GetDescriptor<uint>(UnitFields.Bytes1));

        public WoWStateFlag StateFlag => (WoWStateFlag)Bytes1[3];

        #endregion

        #region Bytes2 (Sheath, PvP, Shapeshift)

        internal byte[] Bytes2 => BitConverter.GetBytes(GetDescriptor<uint>(UnitFields.Bytes2));

        internal UnitPvPStateFlags PvPState => (UnitPvPStateFlags)Bytes2[1];
        internal SheathType SheathType => (SheathType)Bytes2[0];
        public ShapeshiftForm Shapeshift => (ShapeshiftForm)Bytes2[3];

        #endregion

        #region Flag Helper Methods

        private bool HasNpcFlag(UnitNPCFlags flags)
        {
            BitVector32 npcFlags = NpcFlags;
            return npcFlags[(int)flags];
        }

        private bool HasDynamicFlag(UnitDynamicFlags flags)
        {
            BitVector32 dynFlags = DynamicFlags;
            return dynFlags[(int)flags];
        }

		/// <summary>
		/// Public method to check if unit has a specific dynamic flag.
		/// </summary>
		public bool HasUnitDynamicFlag(UnitDynamicFlags flags)
		{
			return HasDynamicFlag(flags);
		}

        private bool HasUnitFlag(UnitFlags flags)
        {
            BitVector32 unitFlags = Flags;
            return unitFlags[(int)flags];
        }

        private bool HasUnitFlag2(UnitFlags2 flags)
        {
            BitVector32 flags2 = Flags2;
            return flags2[(int)flags];
        }

        private static bool HasFlag(uint flag, uint val)
        {
            return (flag & val) != 0;
        }

        #endregion

        #region Descriptor Helper

        internal T GetDescriptor<T>(UnitFields field) where T : struct
        {
            return GetDescriptorField<T>((int)field * 4);
        }

        #endregion

        #region Position & Movement

        public bool Behind(WoWUnit? obj)
        {
            if (obj == null || obj.Distance >= 10.0)
                return false;

            return WoWMathHelper.IsBehind(Location, obj.Location, obj.Rotation);
        }

        public override float X
        {
            get
            {
                Memory? wow = ObjectManager.Wow;
                if (wow == null)
                    return 0f;
                return wow.Read<float>(BaseAddress + 1944);
            }
        }

        public override float Y
        {
            get
            {
                Memory? wow = ObjectManager.Wow;
                if (wow == null)
                    return 0f;
                return wow.Read<float>(BaseAddress + 1948);
            }
        }

        public override float Z
        {
            get
            {
                Memory? wow = ObjectManager.Wow;
                if (wow == null)
                    return 0f;
                return wow.Read<float>(BaseAddress + 1952);
            }
        }

        /// <summary>
        /// Raw transport-local position (from CMovementData). Use Location for world coords.
        /// </summary>
        public WoWPoint RelativeLocation
        {
            get
            {
                Memory? wow = ObjectManager.Wow;
                if (wow == null)
                    return WoWPoint.Zero;
                return wow.Read<WoWPoint>(BaseAddress + 1944);
            }
        }

        public override WoWPoint Location
        {
            get
            {
                if (IsOnTransport)
                    return GetWorldPosition();
                return RelativeLocation;
            }
        }

        public override float Rotation
        {
            get
            {
                Memory? wow = ObjectManager.Wow;
                if (wow == null)
                    return 0f;
                return wow.Read<float>(BaseAddress + 1960);
            }
        }

        public WoWGameObject? Transport => ObjectManager.GetObjectByGuid<WoWGameObject>(WoWMovementInfo.TransportGuid);

        public bool IsOnTransport => WoWMovementInfo.TransportGuid != 0UL;

        public bool IsPlayerBehind => WoWMathHelper.IsBehind(ObjectManager.Me!.Location, Location, Rotation);

        public bool IsAutoAttacking
        {
            get
            {
                Memory? wow = ObjectManager.Wow;
                if (wow == null)
                    return false;
                return wow.Read<ulong>(BaseAddress + 2592) != 0UL;
            }
        }

        /// <summary>
        /// Whether the unit is currently moving.
        /// Uses MovementInfo.IsMoving which checks movement flags (HB 4.3.4 style).
        /// </summary>
        public bool IsMoving => WoWMovementInfo.IsMoving;

        public bool IsPet => SummonedBy != null;

        /// <summary>
        /// Movement information for this unit.
        /// Alias for WoWMovementInfo for HB 4.3.4 compatibility.
        /// </summary>
        public WoWMovementInfo MovementInfo => WoWMovementInfo;

        public WoWMovementInfo WoWMovementInfo
        {
            get
            {
                Memory? wow = ObjectManager.Wow;
                if (wow == null)
                    return new WoWMovementInfo(0);
                return new WoWMovementInfo(wow.Read<uint>(BaseAddress + 216));
            }
        }

        public uint MovementFlags
        {
            get
            {
                Memory? wow = ObjectManager.Wow;
                if (wow == null)
                    return 0;
                return wow.Read<uint>(BaseAddress + 216, 68);
            }
        }

        public bool IsFalling
        {
            get
            {
                uint flags = MovementFlags;
                if ((flags & 4096) != 0)
                    return (flags & 2048) == 0;
                return false;
            }
        }

        public bool IsFlying
        {
            get
            {
                bool hasFlag = (MovementFlags & 33554432) != 0;
                if (hasFlag)
                {
                    Memory? wow = ObjectManager.Wow;
                    if (wow == null)
                        return false;

                    uint ptr = wow.Read<uint>(BaseAddress + 3936);
                    if (ptr == 0)
                        return true;

                    if (wow.Read<uint>(ptr + 20) != 3)
                        return true;
                }
                return false;
            }
        }

        #endregion

        #region Power & Health

        public int GetCurrentPower(WoWPowerType power)
        {
            switch (power)
            {
                case WoWPowerType.Health:
                    return GetDescriptor<int>(UnitFields.Health);
                case WoWPowerType.Mana:
                    return GetDescriptor<int>(UnitFields.Mana);
                case WoWPowerType.Rage:
                    return GetDescriptor<int>(UnitFields.Rage) / 10;
                case WoWPowerType.Focus:
                    return GetDescriptor<int>(UnitFields.Focus);
                case WoWPowerType.Energy:
                    return GetDescriptor<int>(UnitFields.Energy);
                case WoWPowerType.Happiness:
                    return GetDescriptor<int>(UnitFields.Happiness);
                case WoWPowerType.Runes:
                    return GetDescriptor<int>(UnitFields.Runes);
                case WoWPowerType.RunicPower:
                    return GetDescriptor<int>(UnitFields.RunicPower) / 10;
                default:
                    throw new ArgumentOutOfRangeException(nameof(power));
            }
        }

        public int GetMaxPower(WoWPowerType power)
        {
            switch (power)
            {
                case WoWPowerType.Health:
                    return GetDescriptor<int>(UnitFields.MaxHealth);
                case WoWPowerType.Mana:
                    return GetDescriptor<int>(UnitFields.MaxMana);
                case WoWPowerType.Rage:
                    return GetDescriptor<int>(UnitFields.MaxRage) / 10;
                case WoWPowerType.Focus:
                    return GetDescriptor<int>(UnitFields.MaxFocus);
                case WoWPowerType.Energy:
                    return GetDescriptor<int>(UnitFields.MaxEnergy);
                case WoWPowerType.Happiness:
                    return GetDescriptor<int>(UnitFields.MaxHappiness);
                case WoWPowerType.Runes:
                    return GetDescriptor<int>(UnitFields.MaxRunes);
                case WoWPowerType.RunicPower:
                    return GetDescriptor<int>(UnitFields.MaxRunicPower) / 10;
                default:
                    throw new ArgumentOutOfRangeException(nameof(power));
            }
        }

        public double GetPowerPercent(WoWPowerType p)
        {
            int current = GetCurrentPower(p);
            int max = GetMaxPower(p);
            if (max == 0)
                return 0;
            return Math.Min((double)(current * 100) / max, 100.0);
        }

        public int CurrentHealth => GetCurrentPower(WoWPowerType.Health);
        public int CurrentMana => GetCurrentPower(WoWPowerType.Mana);
        public int CurrentRage => GetCurrentPower(WoWPowerType.Rage);
        public int CurrentEnergy => GetCurrentPower(WoWPowerType.Energy);
        public int CurrentFocus => GetCurrentPower(WoWPowerType.Focus);
        public uint CurrentHappiness => (uint)Math.Round(GetCurrentPower(WoWPowerType.Happiness) / 10000.0);
        public int CurrentRunicPower => GetCurrentPower(WoWPowerType.RunicPower);
        public int CurrentPower => GetCurrentPower(PowerType);

        /// <summary>
        /// Gets pet happiness percentage (HB 4.3.4 compatibility).
        /// WotLK feature - pets have happiness.
        /// </summary>
        public double HappinessPercent
        {
            get
            {
                if (MaxHappiness <= 0)
                    return 0;
                return (GetCurrentPower(WoWPowerType.Happiness) / 10000.0) / (MaxHappiness / 10000.0) * 100.0;
            }
        }

        public int MaxHealth => GetMaxPower(WoWPowerType.Health);
        public int MaxMana => GetMaxPower(WoWPowerType.Mana);
        public int MaxRage => GetMaxPower(WoWPowerType.Rage);
        public int MaxEnergy => GetMaxPower(WoWPowerType.Energy);
        public int MaxFocus => GetMaxPower(WoWPowerType.Focus);
        public int MaxHappiness => GetMaxPower(WoWPowerType.Happiness);
        public int MaxRunicPower => GetMaxPower(WoWPowerType.RunicPower);
        public int MaxPower => GetMaxPower(PowerType);

        public double HealthPercent => GetPowerPercent(WoWPowerType.Health);

        /// <summary>
        /// Gets the rage percentage (0-100). Used by Warriors.
        /// </summary>
        public double RagePercent => GetPowerPercent(WoWPowerType.Rage);

        /// <summary>
        /// Gets the energy percentage (0-100). Used by Rogues, Feral Druids.
        /// </summary>
        public double EnergyPercent => GetPowerPercent(WoWPowerType.Energy);

        /// <summary>
        /// Gets the runic power percentage (0-100). Used by Death Knights.
        /// </summary>
        public double RunicPowerPercent => GetPowerPercent(WoWPowerType.RunicPower);

        public double ManaPercent
        {
            get
            {
                try
                {
                    return GetPowerPercent(WoWPowerType.Mana);
                }
                catch
                {
                    return 0.0;
                }
            }
        }

        #endregion

        #region Level & Faction

        public int Level => GetDescriptor<int>(UnitFields.Level);
        internal int InternalLevel => GetDescriptor<int>(UnitFields.Level);

        public uint FactionId => GetDescriptor<uint>(UnitFields.FactionTemplate);
        
        #endregion

        #region Target & Relationships

        public ulong CurrentTargetGuid => GetDescriptor<ulong>(UnitFields.Target);
        public WoWUnit? CurrentTarget => ObjectManager.GetObjectByGuid<WoWUnit>(CurrentTargetGuid);
        public bool GotTarget => CurrentTarget != null;

        public ulong CharmedByGuid => GetDescriptor<ulong>(UnitFields.CharmedBy);

        public WoWUnit? CharmedBy => ObjectManager.GetObjectByGuid<WoWUnit>(CharmedByGuid);

        public ulong Summon => GetDescriptor<ulong>(UnitFields.Summon);
        public ulong Charmed => GetDescriptor<ulong>(UnitFields.Charm);
        public ulong Critter => GetDescriptor<ulong>(UnitFields.Critter);

        public WoWUnit? SummonedBy => ObjectManager.GetObjectByGuid<WoWUnit>(SummonedByGuid);
        public ulong SummonedByGuid => GetDescriptor<ulong>(UnitFields.SummonedBy);

        public virtual WoWUnit? OwnedByUnit => CharmedBy ?? SummonedBy;

        public WoWUnit? OwnedByRoot
        {
            get
            {
                WoWUnit? owner = OwnedByUnit;
                if (owner == null)
                    return null;
                return owner.OwnedByUnit ?? owner;
            }
        }

        /// <summary>
        /// Gets the controlling player for this unit.
        /// Walks the charm/summon chain up to 2 levels to find a player controller.
        /// </summary>
        public WoWPlayer? ControllingPlayer
        {
            get
            {
                WoWUnit? unit = this;
                ulong guid = CharmedByGuid;
                if (guid == 0UL)
                    guid = CreatedByGuid;

                if (guid != 0UL)
                    unit = ObjectManager.GetObjectByGuid<WoWUnit>(guid);

                if (unit == null)
                    return null;

                // If not a player yet, walk one more level
                if (!(unit is WoWPlayer))
                {
                    guid = unit.CharmedByGuid;
                    if (guid == 0UL)
                        guid = unit.CreatedByGuid;
                    unit = ObjectManager.GetObjectByGuid<WoWUnit>(guid);
                }

                if (unit != null && unit is WoWPlayer player)
                    return player;

                return null;
            }
        }

        public virtual WoWUnit? Pet
        {
            get
            {
                ulong guid = Charmed;
                if (guid == 0)
                    guid = Summon;

                return guid == 0 ? null : ObjectManager.GetObjectByGuid<WoWUnit>(guid);
            }
        }

        /// <summary>
        /// Checks if this unit is dueling another unit.
        /// Both units must be player-controlled and have matching duel arbiters but different teams.
        /// </summary>
        public bool IsDueling(WoWUnit other)
        {
            if (!PlayerControlled || !other.PlayerControlled)
                return false;

            WoWPlayer? thisPlayer = this is WoWPlayer p1 ? p1 : ControllingPlayer;
            WoWPlayer? otherPlayer = other is WoWPlayer p2 ? p2 : other.ControllingPlayer;

            if (thisPlayer != null && otherPlayer != null)
            {
                uint duelTeam1 = thisPlayer.DuelTeam;
                if (duelTeam1 != 0)
                {
                    uint duelTeam2 = otherPlayer.DuelTeam;
                    if (duelTeam2 != 0 && 
                        thisPlayer.DuelArbiterGuid == otherPlayer.DuelArbiterGuid && 
                        duelTeam1 != duelTeam2)
                    {
                        return true;
                    }
                }
                return false;
            }

            // Fallback for when ControllingPlayer is null but one unit is Me
            bool isMe = IsMe;
            bool isOtherMe = other.IsMe;
            
            if (!isMe && !isOtherMe)
                return false;

            // Check duel arbiter guid stored in memory
            Memory? wow = ObjectManager.Wow;
            if (wow == null)
                return false;

            ulong duelArbiterGuid = wow.Read<ulong>(12725088);
            WoWUnit targetUnit = isMe ? other : this;
            
            ulong controllerGuid = targetUnit.CharmedByGuid;
            if (controllerGuid == 0)
                controllerGuid = targetUnit.CreatedByGuid;

            if (controllerGuid != 0)
                return controllerGuid == duelArbiterGuid;

            return false;
        }

        #endregion

        #region Alive & Ghost

        public bool IsAlive => !Dead;

        public virtual bool IsGhost => CurrentHealth == 1;

        public bool GotAlivePet
        {
            get
            {
                WoWUnit? pet = Pet;
                return pet?.IsAlive ?? false;
            }
        }

        public bool IsTargetingPet
        {
            get
            {
                WoWUnit? pet = Pet;
                return pet != null && CurrentTargetGuid == pet.Guid;
            }
        }

        public bool KilledByMe => TaggedByMe && Dead;

        #endregion

        #region Casting

        /// <summary>
        /// Gets the spell ID currently being cast (regular cast).
        /// Returns 0 if not casting.
        /// Offset: BaseAddress + 2668 (WoW 3.3.5a build 12340)
        /// </summary>
        public int CastingSpellId
        {
            get
            {
                Memory? wow = ObjectManager.Wow;
                if (wow == null)
                    return 0;
                return wow.Read<int>(BaseAddress + 2668);
            }
        }

        /// <summary>
        /// Gets the spell ID currently being channeled.
        /// Returns 0 if not channeling.
        /// Offset: BaseAddress + 2688 (WoW 3.3.5a build 12340)
        /// </summary>
        public int CastingChanneledId
        {
            get
            {
                Memory? wow = ObjectManager.Wow;
                if (wow == null)
                    return 0;
                return wow.Read<int>(BaseAddress + 2688);
            }
        }

        /// <summary>
        /// Alias for CastingChanneledId for consistency.
        /// </summary>
        public int ChannelSpellId => CastingChanneledId;

        /// <summary>
        /// Gets the spell ID being cast or channeled.
        /// Returns CastingSpellId if casting, otherwise ChannelSpellId.
        /// </summary>
        public int Casting
        {
            get
            {
                int spellId = CastingSpellId;
                return spellId != 0 ? spellId : ChannelSpellId;
            }
        }

        /// <summary>
        /// Gets the spell currently being cast (regular cast only).
        /// </summary>
        public WoWSpell? CastingSpell
        {
            get
            {
                int spellId = CastingSpellId;
                if (spellId == 0)
                    return null;
                return WoWSpell.FromId(spellId);
            }
        }

        /// <summary>
        /// Gets the spell currently being channeled.
        /// </summary>
        public WoWSpell? ChannelSpell
        {
            get
            {
                int spellId = ChannelSpellId;
                if (spellId == 0)
                    return null;
                return WoWSpell.FromId(spellId);
            }
        }

        /// <summary>
        /// Gets the spell being cast or channeled (whichever is active).
        /// </summary>
        public WoWSpell? CurrentCastingSpell => CastingSpell ?? ChannelSpell;

        /// <summary>
        /// Gets the GUID of the object being channeled on.
        /// Used for spells like Mind Control that channel on a target.
        /// </summary>
        public ulong ChannelObjectGuid => GetDescriptor<ulong>(UnitFields.ChannelObject);

        /// <summary>
        /// Gets the WoW object being channeled on.
        /// Returns null if not channeling on an object.
        /// </summary>
        public WoWObject? ChannelObject
        {
            get
            {
                ulong guid = ChannelObjectGuid;
                if (guid == 0)
                    return null;
                return ObjectManager.GetObjectByGuid<WoWObject>(guid);
            }
        }

        public int ChanneledCasting => GetDescriptor<int>(UnitFields.ChannelSpell);

        /// <summary>
        /// Returns true if the unit is currently casting or channeling a spell.
        /// </summary>
        public bool IsCasting => ChannelObject != null || Casting > 0;

        /// <summary>
        /// Returns true if the unit is currently channeling a spell.
        /// </summary>
        public bool IsChanneling => ChannelSpellId != 0;

        /// <summary>
        /// Alias for ChannelSpellId - HB 4.3.4 compatibility.
        /// </summary>
        public int ChanneledCastingSpellId => ChannelSpellId;

        /// <summary>
        /// Alias for CastingSpellId - HB 4.3.4 compatibility.
        /// </summary>
        public int NonChanneledCastingSpellId => CastingSpellId;

        /// <summary>
        /// Returns true if this unit's current cast can be interrupted.
        /// Checks both IsCasting and Interruptible flag.
        /// </summary>
        public bool CanInterruptCurrentSpellCast
        {
            get
            {
                if (!IsCasting)
                    return false;
                
                // Check if the spell is interruptible via Lua
                // NonInterruptible flag (bit 4 of CastingInfo flags)
                var result = Lua.GetReturnVal<int>(
                    string.Format("local n,_,_,_,_,_,_,notInterruptible = UnitCastingInfo('target'); return notInterruptible and 1 or 0"), 0);
                return result == 0;
            }
        }

        /// <summary>
        /// Gets the time remaining on the current cast.
        /// </summary>
        public TimeSpan CurrentCastTimeLeft
        {
            get
            {
                if (!IsCasting || CastingSpellId == 0)
                    return TimeSpan.Zero;
                
                // Use Lua to get cast end time and remaining time
                var remaining = Lua.GetReturnVal<double>(
                    "local _,_,_,_,endTime = UnitCastingInfo('player'); if endTime then return (endTime/1000) - GetTime() else return 0 end", 0);
                
                return remaining > 0 ? TimeSpan.FromSeconds(remaining) : TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Gets the total cast time of the current spell being cast.
        /// </summary>
        public TimeSpan CurrentCastTime
        {
            get
            {
                if (!IsCasting || CastingSpellId == 0)
                    return TimeSpan.Zero;
                
                var spell = CastingSpell;
                if (spell == null)
                    return TimeSpan.Zero;
                
                return TimeSpan.FromMilliseconds(spell.CastTime);
            }
        }

        #endregion

        #region Type & Classification

        public float BoundingRadius => GetDescriptor<float>(UnitFields.BoundingRadius);
        public float CombatReach => GetDescriptor<float>(UnitFields.CombatReach);

        /// <summary>
        /// Gets the melee reach for this unit.
        /// Special cases for certain NPCs, otherwise CombatReach + 2.
        /// </summary>
        public float MeleeReach
        {
            get
            {
                // Special cases from HB 4.3.4
                if (Entry == 13158U)  // Training Dummy
                    return 5f;
                if (Entry == 49044U)  // Raider's Training Dummy
                    return 6f;
                return CombatReach + 2f;
            }
        }

        /// <summary>
        /// Returns true if this unit is within melee range of the player.
        /// </summary>
        public bool IsWithinMeleeRange => IsWithinMeleeRangeOf(StyxWoW.Me);

        /// <summary>
        /// Returns true if this unit is within melee range of another unit.
        /// </summary>
        public bool IsWithinMeleeRangeOf(WoWUnit other)
        {
            if (other == null)
                return false;
            
            float meleeRange = MeleeReach + other.MeleeReach + 1.33f;
            return Distance2DTo(other) <= meleeRange;
        }

        /// <summary>
        /// Returns the 2D distance to another unit (ignoring Z axis).
        /// </summary>
        public float Distance2DTo(WoWUnit other)
        {
            if (other == null)
                return float.MaxValue;
            
            float dx = Location.X - other.Location.X;
            float dy = Location.Y - other.Location.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        // HB 4.3.4 compatibility. On WotLK (3.3.5a) we don't have the same client offset,
        // so we derive a reasonable height from known unit descriptors.
        public float BoundingHeight
        {
            get
            {
                float height = Math.Max(CombatReach * 2f, BoundingRadius * 2f);
                return MathEx.Clamp(height, 0.5f, 50f);
            }
        }
        public int MountDisplayId => GetDescriptor<int>(UnitFields.MountDisplayId);
        public int DisplayId => GetDescriptor<int>(UnitFields.DisplayId);
        public ulong CreatedByGuid => GetDescriptor<ulong>(UnitFields.CreatedBy);
        public WoWUnit? CreatedBy => ObjectManager.GetObjectByGuid<WoWUnit>(CreatedByGuid);
        public uint CreatedBySpellId => GetDescriptor<uint>(UnitFields.CreatedBySpell);
        public int BaseMana => GetDescriptor<int>(UnitFields.BaseMana);

        public bool IsPlayer => Type == WoWObjectType.Player;
        public bool IsUnit => Type == WoWObjectType.Unit;
        public bool IsTotem => CreatureType == WoWCreatureType.Totem;
        public bool IsNonCombatPet => CreatureType == WoWCreatureType.NonCombatPet;
        public bool BehindTarget => Behind(CurrentTarget);

        // Creature type checks for Singular compatibility
        public bool IsCritter => CreatureType == WoWCreatureType.Critter;
        public bool IsDemon => CreatureType == WoWCreatureType.Demon;
        public bool IsHumanoid => CreatureType == WoWCreatureType.Humanoid;
        public bool IsDragon => CreatureType == WoWCreatureType.Dragon;
        public bool IsGiant => CreatureType == WoWCreatureType.Giant;
        public bool IsUndead => CreatureType == WoWCreatureType.Undead;
        public bool IsBeast => CreatureType == WoWCreatureType.Beast;
        public bool IsElemental => CreatureType == WoWCreatureType.Elemental;
        public bool IsMechanical => CreatureType == WoWCreatureType.Mechanical;

        /// <summary>
        /// Check if the unit is in the player's party or raid.
        /// </summary>
        public bool IsInMyPartyOrRaid
        {
            get
            {
                if (StyxWoW.Me == null) return false;
                if (Guid == StyxWoW.Me.Guid) return true;
                
                // Check party
                foreach (var guid in StyxWoW.Me.PartyMemberGuids)
                {
                    if (guid == Guid) return true;
                }
                
                // Check raid
                foreach (var guid in StyxWoW.Me.RaidMemberGuids)
                {
                    if (guid == Guid) return true;
                }
                
                return false;
            }
        }

        public WoWCreatureType CreatureType
        {
            get
            {
                if (GetCachedInfo(out Styx.WoWInternals.WoWCache.WoWCache.CreatureCacheEntry info))
                    return (WoWCreatureType)info.TypeID;
                return WoWCreatureType.NotSpecified;
            }
        }

        public WoWUnitClassificationType CreatureRank
        {
            get
            {
                if (GetCachedInfo(out Styx.WoWInternals.WoWCache.WoWCache.CreatureCacheEntry info))
                    return (WoWUnitClassificationType)info.Rank;
                return WoWUnitClassificationType.NotSpecified;
            }
        }

        public override float InteractRange => CombatReach + 4f;

        public WoWFactionTemplate? FactionTemplate => WoWFactionTemplate.FromId(FactionId);

        #endregion

        #region Stealth & Detection

        public float GetStealthDetectionRange(WoWUnit to)
        {
            WoWAuraCollection allAuras = GetAllAuras();
            float range = 10.5f - allAuras.GetTotalAuraModifier(WoWApplyAuraType.ModStealth) / 100f;
            range += (to.Level - Level);

            int stealthLevel = allAuras.GetTotalAuraModifier(WoWApplyAuraType.ModStealthLevel);
            if (stealthLevel < 0)
                stealthLevel = 0;

            WoWAuraCollection toAuras = to.GetAllAuras();
            range += (toAuras.GetTotalAuraModifier(WoWApplyAuraType.ModStealthDetect) - stealthLevel) / 5f;

            return Math.Max(0f, Math.Min(range, 45f));
        }

        public float GetAggroRange(WoWUnit to)
        {
            if (to.IsStealthed)
                return GetStealthDetectionRange(to);

            WoWUnitReaction reaction = GetReactionTowards(to);
            if (reaction >= WoWUnitReaction.Neutral || Dead)
                return 0f;

            float range = 20f;
            int levelDiff = Level - to.Level;
            range += levelDiff * 1f;

            if (Elite)
                range += 3f;

            switch (CreatureType)
            {
                case WoWCreatureType.Beast:
                    range -= 3f;
                    break;
                case WoWCreatureType.Mechanical:
                    range += 3f;
                    break;
            }

            range += GetAllAuras().GetTotalAuraModifier(WoWApplyAuraType.ModDetectedRange);
            range += to.GetAllAuras().GetTotalAuraModifier(WoWApplyAuraType.ModDetectRange);

            return Math.Max(5f, Math.Min(range, 45f));
        }

        public bool IsStealthed
        {
            get
            {
                Memory? wow = ObjectManager.Wow;
                if (wow == null)
                    return false;
                return wow.Read<uint>(BaseAddress + 208, 270) == 2;
            }
        }

        #endregion

        #region Reaction & Faction

        public void Face()
        {
            ObjectManager.Me!.SetFacing(this);
        }

        public void Target()
        {
            if (CanSelect && StyxWoW.Me!.CurrentTarget != this)
            {
                StyxWoW.ResetAfk();
                SetTarget(Guid);
            }
        }

        internal void SetTarget(ulong guid)
        {
            if (ObjectManager.Executor == null)
                throw new ArgumentException("Executor is null");

            ExecutorRand executor = ObjectManager.Executor;
            lock (executor.AssemblyLock)
            {
                try
                {
                    uint guidHigh = (uint)((guid >> 32) & 0xFFFFFFFF);
                    uint guidLow = (uint)(guid & 0xFFFFFFFF);

                    executor.Clear();
                    executor.AddLine($"push {guidHigh}");
                    executor.AddLine($"push {guidLow}");
                    executor.AddLine($"call {5393392}");
                    executor.AddLine("add esp, 8");
                    executor.AddLine("retn");
                    executor.Execute();
                }
                catch (Exception ex)
                {
                    Logging.WriteDebug("Failed to set target");
                    Logging.WriteException(ex);
                }
            }
        }

        public WoWUnitReaction GetReactionTowards(WoWUnit otherUnit)
        {
            // Same unit = friendly
            if (this == otherUnit)
                return WoWUnitReaction.Friendly;

            // Check hardcoded reactions first
            uint entry = otherUnit.Entry;
            if (HardcodedReactions.ContainsKey(entry))
                return HardcodedReactions[entry];

            if (StyxWoW.Me!.IsHorde && HordeReactionsByFaction.ContainsKey(otherUnit.FactionId))
                return HordeReactionsByFaction[otherUnit.FactionId];

            if (StyxWoW.Me.IsHorde && HordeReactionsByEntry.ContainsKey(entry))
                return HordeReactionsByEntry[entry];

            // Check instance cache by entry first, then by GUID
            if (entry != 0 && _reactionCacheByEntry.ContainsKey(entry))
                return _reactionCacheByEntry[entry];

            ulong guid = otherUnit.Guid;
            if (_reactionCacheByGuid.ContainsKey(guid))
                return _reactionCacheByGuid[guid];

            // Player-controlled units: special duel/pvp handling
            if (PlayerControlled && otherUnit.PlayerControlled)
            {
                WoWPlayer? controllingPlayer = ControllingPlayer;
                WoWPlayer? otherControllingPlayer = otherUnit.ControllingPlayer;

                if (controllingPlayer != null && otherControllingPlayer != null)
                {
                    // Same controller = friendly
                    if (controllingPlayer == otherControllingPlayer)
                        return WoWUnitReaction.Friendly;

                    // Duel check
                    uint duelTeam = controllingPlayer.DuelTeam;
                    if (duelTeam != 0)
                    {
                        uint otherDuelTeam = otherControllingPlayer.DuelTeam;
                        if (otherDuelTeam != 0)
                        {
                            ulong duelArbiter = controllingPlayer.DuelArbiterGuid;
                            ulong otherDuelArbiter = otherControllingPlayer.DuelArbiterGuid;
                            if (duelArbiter == otherDuelArbiter)
                            {
                                return duelTeam == otherDuelTeam ? WoWUnitReaction.Friendly : WoWUnitReaction.Hostile;
                            }
                        }
                    }

                    // Party/raid check - same party/raid = friendly
                    if ((controllingPlayer.IsMe && otherControllingPlayer.InMyPartyOrRaid) ||
                        (otherControllingPlayer.IsMe && controllingPlayer.InMyPartyOrRaid))
                    {
                        return WoWUnitReaction.Friendly;
                    }
                }

                // Contested PvP flag check - both flagged for PvP = hostile
                // Only check for players (ContestedPvPFlagged is a WoWPlayer property)
                if (IsPlayer && otherUnit.IsPlayer)
                {
                    var me = ToPlayer();
                    var other = otherUnit.ToPlayer();
                    if (me != null && other != null && me.ContestedPvPFlagged && other.ContestedPvPFlagged)
                        return WoWUnitReaction.Hostile;
                }
            }

            // Call the native GetReactionTowards function
            WoWUnitReaction reaction = GetReactionTowardsNative(otherUnit);

            // Cache the result
            if (entry != 0)
                _reactionCacheByEntry[entry] = reaction;
            else
                _reactionCacheByGuid[guid] = reaction;

            return reaction;
        }

        /// <summary>
        /// Calls the native GetReactionTowards function via ASM injection.
        /// </summary>
        private WoWUnitReaction GetReactionTowardsNative(WoWUnit otherUnit)
        {
            ExecutorRand? executor = ObjectManager.Executor;
            if (executor == null)
                return WoWUnitReaction.Neutral;

            try
            {
                lock (executor.AssemblyLock)
                {
                    executor.Clear();
                    executor.AddLine($"push {otherUnit.BaseAddress}");
                    executor.AddLine($"mov ecx, {BaseAddress}");
                    executor.AddLine($"call {7492032}");
                    executor.AddLine("retn");
                    executor.Execute();
                }

                Memory? memory = executor.Memory;
                if (memory == null)
                    return WoWUnitReaction.Neutral;

                return (WoWUnitReaction)memory.Read<uint>(executor.ReturnPointer);
            }
            catch (Exception ex)
            {
                Logging.WriteDebug($"GetReactionTowardsException: {Name} vs {otherUnit.Name}");
                Logging.WriteDebug(ex.ToString());
                return WoWUnitReaction.Neutral;
            }
        }

        // ContestedPvPFlagged removed from WoWUnit - not present in HB 3.3.5a original
        // This property is defined in WoWPlayer using PlayerFlags (correct for players)

        public bool IsFriendly
        {
            get
            {
                if (IsPlayer)
                {
                    WoWPlayer? player = ToPlayer();
                    return player != null && player.IsHorde == ObjectManager.Me!.IsHorde;
                }
                return MyReaction >= WoWUnitReaction.Friendly;
            }
        }

        public bool IsHostile
        {
            get
            {
                if (IsPlayer)
                {
                    WoWPlayer? player = ToPlayer();
                    return player != null && player.IsHorde == ObjectManager.Me!.IsAlliance;
                }
                return MyReaction <= WoWUnitReaction.Unfriendly;
            }
        }

        public bool IsNeutral => MyReaction == WoWUnitReaction.Neutral;

        public WoWUnitReaction MyReaction => ObjectManager.Me!.GetReactionTowards(this);

        public bool IsTargetingMeOrPet
        {
            get
            {
                WoWUnit? pet = ObjectManager.Me!.Pet;
                ulong targetGuid = CurrentTargetGuid;

                if (pet != null && pet.Guid == targetGuid)
                    return true;

                if (ObjectManager.Me.Guid == targetGuid)
                    return true;

                if (OwnedByRoot != null)
                    return Guid == CurrentTargetGuid;

                return false;
            }
        }

        [Obsolete("Use IsTargetingMeOrPet instead. They do the same.")]
        public bool IsTargetingMe => IsTargetingMeOrPet;

        #endregion

        #region Threat

        public UnitThreatInfo GetThreatInfoFor(WoWUnit otherUnit)
        {
            return UnitThreatInfo.GetThreatInfo(this, otherUnit);
        }

        public bool Aggro => ThreatInfo.ThreatStatus >= ThreatStatus.NoobishTank;

        public bool PetAggro
        {
            get
            {
                WoWUnit? pet = ObjectManager.Me!.Pet;
                if (pet != null && Guid == pet.CurrentTargetGuid)
                    return pet.Combat;
                return false;
            }
        }

        public float MyAggroRange => GetAggroRange(StyxWoW.Me!);
        public float MyStealthDetectionRange => StyxWoW.Me!.GetStealthDetectionRange(this);
        public UnitThreatInfo ThreatInfo => GetThreatInfoFor(StyxWoW.Me!);

        #endregion

        #region Stats

        public int Strength => GetDescriptor<int>(UnitFields.Stat0);
        public int Agility => GetDescriptor<int>(UnitFields.Stat1);
        public int Stamina => GetDescriptor<int>(UnitFields.Stat2);
        public int Intellect => GetDescriptor<int>(UnitFields.Stat3);
        public int Spirit => GetDescriptor<int>(UnitFields.Stat4);

        public int StrengthBonus => GetDescriptor<int>(UnitFields.PosStat0);
        public int AgilityBonus => GetDescriptor<int>(UnitFields.PosStat1);
        public int StaminaBonus => GetDescriptor<int>(UnitFields.PosStat2);
        public int IntellectBonus => GetDescriptor<int>(UnitFields.PosStat3);
        public int SpiritBonus => GetDescriptor<int>(UnitFields.PosStat4);

        public int Armor => GetDescriptor<int>(UnitFields.ResistanceArmor);
        public int HolyResist => GetDescriptor<int>(UnitFields.ResistanceHoly);
        public int FireResist => GetDescriptor<int>(UnitFields.ResistanceFire);
        public int NatureResist => GetDescriptor<int>(UnitFields.ResistanceNature);
        public int FrostResist => GetDescriptor<int>(UnitFields.ResistanceFrost);
        public int ShadowResist => GetDescriptor<int>(UnitFields.ResistanceShadow);
        public int ArcaneResist => GetDescriptor<int>(UnitFields.ResistanceArcane);

        #endregion

        #region Skinning & Loot

        public bool IsSwimming
        {
            get
            {
                Memory? wow = ObjectManager.Wow;
                if (wow == null)
                    return false;
                return (wow.Read<uint>(BaseAddress + 2608) & 2097152) != 0;
            }
        }

        public WoWCreatureSkinType SkinType
        {
            get
            {
                if (GetCachedInfo(out Styx.WoWInternals.WoWCache.WoWCache.CreatureCacheEntry info))
                {
                    if ((info.TypeFlags & Styx.WoWInternals.WoWCache.WoWCache.CreatureCacheTypeFlags.HerbSkin) != 0)
                        return WoWCreatureSkinType.Herb;
                    if ((info.TypeFlags & Styx.WoWInternals.WoWCache.WoWCache.CreatureCacheTypeFlags.MineSkin) != 0)
                        return WoWCreatureSkinType.Rock;
                    if ((info.TypeFlags & Styx.WoWInternals.WoWCache.WoWCache.CreatureCacheTypeFlags.Salvageable) != 0)
                        return WoWCreatureSkinType.Bolts;
                    return WoWCreatureSkinType.Leather;
                }
                return WoWCreatureSkinType.None;
            }
        }

        public bool IsTameable
        {
            get
            {
                if (GetCachedInfo(out Styx.WoWInternals.WoWCache.WoWCache.CreatureCacheEntry info))
                    return (info.TypeFlags & Styx.WoWInternals.WoWCache.WoWCache.CreatureCacheTypeFlags.Tameable) != 0;
                return false;
            }
        }

        public bool IsGhostVisible
        {
            get
            {
                if (GetCachedInfo(out Styx.WoWInternals.WoWCache.WoWCache.CreatureCacheEntry info))
                    return (info.TypeFlags & Styx.WoWInternals.WoWCache.WoWCache.CreatureCacheTypeFlags.GhostVisible) != 0;
                return false;
            }
        }

        public bool IsExotic
        {
            get
            {
                if (GetCachedInfo(out Styx.WoWInternals.WoWCache.WoWCache.CreatureCacheEntry info))
                    return (info.TypeFlags & Styx.WoWInternals.WoWCache.WoWCache.CreatureCacheTypeFlags.Exotic) != 0;
                return false;
            }
        }

        public bool CanSkin
        {
            get
            {
                if (Level <= 2 || ObjectManager.Me == null || IsPlayer)
                    return false;

                WoWCreatureType type = CreatureType;
                WoWSkill? skill;

                switch (type)
                {
                    case WoWCreatureType.Beast:
                    case WoWCreatureType.Dragon:
                    case WoWCreatureType.Demon:
                    case WoWCreatureType.Undead:
                    case WoWCreatureType.Humanoid:
                    case WoWCreatureType.Critter:
                        if (!Skinnable || SkinType != WoWCreatureSkinType.Leather)
                            return false;
                        skill = ObjectManager.Me!.GetSkill(SkillLine.Skinning);
                        break;

                    case WoWCreatureType.Elemental:
                    case WoWCreatureType.Mechanical:
                        SkillLine skillLine;
                        switch (SkinType)
                        {
                            case WoWCreatureSkinType.Herb:
                                skillLine = SkillLine.Herbalism;
                                break;
                            case WoWCreatureSkinType.Rock:
                                skillLine = SkillLine.Mining;
                                break;
                            case WoWCreatureSkinType.Bolts:
                                skillLine = SkillLine.Engineering;
                                break;
                            default:
                                return false;
                        }
                        skill = ObjectManager.Me!.GetSkill(skillLine);
                        break;

                    default:
                        return false;
                }

                if (skill == null || skill.MaxValue == 0)
                    return false;

                int current = skill.CurrentValue;
                if (current < 100)
                    return Level <= current / 10 + 10;
                return Level <= current / 5;
            }
        }

        #endregion

        #region Party/Raid Targeting

        public bool IsTargetingMyPartyMember
        {
            get
            {
                ulong targetGuid = CurrentTargetGuid;
                if (targetGuid == 0)
                    return false;

                for (int i = 0; i < 4; i++)
                {
                    if (ObjectManager.Me!.GetPartyMemberGuid(i) == targetGuid)
                        return true;
                }
                return false;
            }
        }

        public bool IsTargetingMyRaidMember
        {
            get
            {
                ulong targetGuid = CurrentTargetGuid;
                if (targetGuid == 0)
                    return false;

                for (int i = 0; i < 40; i++)
                {
                    if (ObjectManager.Me!.GetRaidMemberGuid(i) == targetGuid)
                        return true;
                }
                return false;
            }
        }

        #endregion

        #region Auras

        public Dictionary<string, WoWAura> Auras => GetAurasDictionary();

        [Obsolete("'Buffs' has been renamed to 'Auras'. Use 'Auras' instead.")]
        public Dictionary<string, WoWAura> Buffs => Auras;

        public Dictionary<string, WoWAura> PassiveAuras =>
            Auras.Where(a => a.Value.IsPassive || a.Value.TimeLeft.Milliseconds <= 0)
                .ToDictionary(k => k.Key, v => v.Value);

        public Dictionary<string, WoWAura> ActiveAuras =>
            Auras.Where(a => !a.Value.IsPassive && a.Value.TimeLeft.Milliseconds > 0)
                .ToDictionary(k => k.Key, v => v.Value);

        public Dictionary<string, WoWAura> Debuffs =>
            Auras.Where(a => a.Value.IsHarmful && a.Value.IsActive && !a.Value.IsPassive)
                .ToDictionary(k => k.Key, v => v.Value);

        [Obsolete("Caching of buffs has been removed. Use the 'Auras' property instead.")]
        public Dictionary<string, WoWAura> GetBuffs(bool forceRefresh) => GetAurasDictionary();

        public WoWAura? GetAuraByName(string name) => GetAllAuras().FirstOrDefault(a => a.Name == name);

        /// <summary>
        /// Gets an aura by its spell ID.
        /// </summary>
        public WoWAura? GetAuraById(int id) => GetAllAuras().FirstOrDefault(a => a.SpellId == id);

        /// <summary>
        /// Checks if the unit has an aura with the specified spell ID.
        /// </summary>
        public bool HasAura(int id) => GetAuraById(id) != null;

        public bool HasAura(string name) => GetAuraByName(name) != null;

        private Dictionary<string, WoWAura> GetAurasDictionary()
        {
            Dictionary<string, WoWAura> dict = new Dictionary<string, WoWAura>();
            foreach (WoWAura aura in GetAllAuras())
            {
                dict[aura.Name] = aura;
            }
            return dict;
        }

        public unsafe WoWAuraCollection GetAllAuras()
        {
            Memory? wow = ObjectManager.Wow;
            if (wow == null)
                return new WoWAuraCollection(0);

            uint auraBase = BaseAddress + 3152;
            int auraCount = wow.Read<int>(BaseAddress + 3536);

            // Dynamic auras
            if (auraCount == -1)
            {
                auraBase = wow.Read<uint>(BaseAddress + 3160);
                auraCount = wow.Read<int>(BaseAddress + 3156);
            }

            WoWAura.AuraInfo[] auraInfos = new WoWAura.AuraInfo[auraCount];

            fixed (WoWAura.AuraInfo* ptr = auraInfos)
            {
                wow.ReadBytes(auraBase, (void*)ptr, 24 * auraCount);
            }

            WoWAuraCollection collection = new WoWAuraCollection(auraCount);
            for (uint i = 0; i < auraCount; i++)
            {
                WoWAura aura = new WoWAura(auraInfos[i]);
                if (aura.Spell != null)
                    collection.Add(aura);
            }

            return collection;
        }

        #endregion

        #region Helper Methods

        public WoWPoint GetTraceLinePos() => new WoWPoint(X, Y, Z + 2.132f);

        /// <summary>
        /// Whether this unit is in line of sight from the player.
        /// Ported from HB 4.3.4.
        /// </summary>
        public bool InLineOfSight
        {
            get
            {
                LocalPlayer? me = ObjectManager.Me;
                if (me == null)
                    return false;
                return World.GameWorld.IsInLineOfSight(me.GetTraceLinePos(), GetTraceLinePos());
            }
        }

        /// <summary>
        /// Whether this unit is in line of spell sight from the player.
        /// Ported from HB 4.3.4.
        /// </summary>
        public bool InLineOfSpellSight
        {
            get
            {
                LocalPlayer? me = ObjectManager.Me;
                if (me == null)
                    return false;
                return World.GameWorld.IsInLineOfSpellSight(me.GetTraceLinePos(), GetTraceLinePos());
            }
        }

        /// <summary>
        /// Returns the world-space position of this unit.
        /// When on a transport (elevator, ship), transforms the transport-local
        /// position by the transport's world matrix (read from GO BaseAddress + 0x1A8).
        /// Matches HB 3.3.5a's GetWorldPosition().
        /// </summary>
        public WoWPoint GetWorldPosition()
        {
            WoWPoint local = RelativeLocation;
            if (!IsOnTransport)
                return local;

            WoWGameObject? transport = Transport;
            if (transport == null)
                return local;

            // Read the 4×4 world matrix the WoW client maintains for this GO.
            // Offset 0x1A8 (424) from GO base — continuously updated for moving transports.
            Tripper.Tools.Math.Matrix worldMatrix = transport.GetWorldMatrix();
            Matrix4x4 mat = worldMatrix;

            // Transform local position to world position
            var localVec = new System.Numerics.Vector3(local.X, local.Y, local.Z);
            var worldVec = System.Numerics.Vector3.Transform(localVec, mat);

            return new WoWPoint(worldVec.X, worldVec.Y, worldVec.Z);
        }

        public bool GetCachedInfo(out Styx.WoWInternals.WoWCache.WoWCache.CreatureCacheEntry info)
        {
            Memory? wow = ObjectManager.Wow;
            if (wow == null)
            {
                info = default;
                return false;
            }

            uint ptr = wow.Read<uint>(BaseAddress + 2404);
            if (ptr != 0)
            {
                info = wow.Read<Styx.WoWInternals.WoWCache.WoWCache.CreatureCacheEntry>(ptr);
                return true;
            }

            info = default;
            return false;
        }

        #endregion
    }
}
