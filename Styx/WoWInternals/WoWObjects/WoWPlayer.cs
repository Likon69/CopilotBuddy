using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Styx.Combat.CombatRoutine;
using Styx.Offsets;

namespace Styx.WoWInternals.WoWObjects
{
    public class WoWPlayer : WoWUnit
    {
        #region Constants - Player Descriptor Fields
        
        // Player descriptor fields (offset = field * 4 from descriptor base)
        private const uint DescRace = 0x34;              // Race dans le descriptor
        private const uint DescClass = 0x34;             // Classe (partagé avec race, bits différents)
        private const uint DescGender = 0x34;            // Genre (partagé, bits différents)
        private const uint DescPlayerFlags = 0x96;       // PLAYER_FLAGS (absolute descriptor index)
        private const uint DescXP = 0x27A;               // PLAYER_XP
        private const uint DescNextLevelXP = 0x27B;      // PLAYER_NEXT_LEVEL_XP
        private const uint DescCoinage = 0x492;          // PLAYER_FIELD_COINAGE
        private const uint DescHonor = 0x4FD;            // PLAYER_FIELD_HONOR_CURRENCY
        private const uint DescPlayerBytes3 = 0x9B;       // PLAYER_BYTES_3 (Inebriation, BattlefieldArenaFaction)
        private const uint DescGuildTimestamp = 0x9D;     // PLAYER_GUILD_TIMESTAMP
        private const uint DescChosenTitle = 0x141;       // PLAYER_CHOSEN_TITLE
        private const uint DescInebriation = 0x142;       // PLAYER_FIELD_INEBRIATION
        private const uint DescPvpMedalCount = 0x4B0;     // PLAYER_FIELD_PVP_MEDALS
        
        // Race, Class, Gender are inherited from WoWUnit (offset 0x17 - UNIT_FIELD_BYTES_0)
        
        #endregion
        
        #region Constructor
        public WoWPlayer(uint baseAddress) : base(baseAddress)
        {
        }
        
        #endregion
        
        #region Gender Helper Properties
        // Race, Class, Gender are inherited from WoWUnit (offset 0x17 - UNIT_FIELD_BYTES_0)
        // No redefinition needed for 3.3.5a - using correct parent implementation
        
        public bool IsMale => Gender == WoWGender.Male;
        public bool IsFemale => Gender == WoWGender.Female;
        
        #endregion
        
        #region Faction Properties
        public bool IsAlliance
        {
            get
            {
                return Race switch
                {
                    WoWRace.Human => true,
                    WoWRace.Dwarf => true,
                    WoWRace.NightElf => true,
                    WoWRace.Gnome => true,
                    WoWRace.Draenei => true,
                    _ => false
                };
            }
        }
        public bool IsHorde
        {
            get
            {
                return Race switch
                {
                    WoWRace.Orc => true,
                    WoWRace.Undead => true,
                    WoWRace.Tauren => true,
                    WoWRace.Troll => true,
                    WoWRace.BloodElf => true,
                    _ => false
                };
            }
        }
        
        #endregion
        
        #region Class Helpers
        public bool IsTank
        {
            get
            {
                return Class == WoWClass.Warrior || 
                       Class == WoWClass.Paladin || 
                       Class == WoWClass.DeathKnight ||
                       Class == WoWClass.Druid;
                // Note: Should check spec/stance, simplified here
            }
        }
        public bool IsHealer
        {
            get
            {
                return Class == WoWClass.Priest ||
                       Class == WoWClass.Paladin ||
                       Class == WoWClass.Shaman ||
                       Class == WoWClass.Druid;
                // Note: Should check spec, simplified here
            }
        }
        public bool IsCaster
        {
            get
            {
                return Class == WoWClass.Mage ||
                       Class == WoWClass.Warlock ||
                       Class == WoWClass.Priest ||
                       Class == WoWClass.Shaman ||
                       Class == WoWClass.Druid;
            }
        }
        public bool IsMelee
        {
            get
            {
                return Class == WoWClass.Warrior ||
                       Class == WoWClass.Rogue ||
                       Class == WoWClass.DeathKnight ||
                       Class == WoWClass.Paladin;
            }
        }
        
        #endregion
        
        #region Override Properties
        public override WoWObjectType Type => WoWObjectType.Player;
        public override float InteractRange => 3f;
        
        #endregion
        
        #region Group Properties
        
        /// <summary>
        /// Whether this player is in the local player's party (not including self).
        /// </summary>
        public bool IsInMyParty
        {
            get
            {
                LocalPlayer? me = ObjectManager.Me;
                if (me == null || IsMe) return false;
                
                ulong myGuid = Guid;
                for (int i = 0; i < 4; i++)
                {
                    if (me.GetPartyMemberGUID(i) == myGuid)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// HB compatibility alias.
        /// </summary>
        public bool InMyParty => IsInMyParty;
        
        /// <summary>
        /// Whether this player is in the local player's raid (not including self).
        /// </summary>
        public bool IsInMyRaid
        {
            get
            {
                LocalPlayer? me = ObjectManager.Me;
                if (me == null || IsMe) return false;
                
                ulong myGuid = Guid;
                for (int i = 0; i < 40; i++)
                {
                    if (me.GetRaidMemberGUID(i) == myGuid)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// HB compatibility alias.
        /// </summary>
        public bool InMyRaid => IsInMyRaid;

        /// <summary>
        /// Whether this player is in the local player's party or raid.
        /// </summary>
        public bool IsInMyPartyOrRaid => IsInMyParty || IsInMyRaid;

        /// <summary>
        /// HB compatibility alias. 
        /// </summary>
        public bool InMyPartyOrRaid => IsInMyPartyOrRaid;
        
        #endregion
        
        #region Player State Properties
        
        // mirror HB 3.3.5a / 4.3.4 implementation using flags bitvector
        private BitVector32 PlayerFlags
        {
            get => new BitVector32(ReadDescriptor<int>(DescPlayerFlags));
        }

        public virtual bool IsGhost
        {
            get => PlayerFlags[16];
        }

        /// <summary>
        /// HB 4.3.4: override bool IsAlive => !base.Dead && !this.IsGhost
        /// </summary>
        public override bool IsAlive => !Dead && !IsGhost;
        
        #endregion
        
        #region Combat Stats (FEAT-10)

        // Absolute descriptor indices from 335offsetsall.txt
        private const uint DescBlockPercent = 0x400;         // PLAYER_BLOCK_PERCENTAGE
        private const uint DescDodgePercent = 0x401;         // PLAYER_DODGE_PERCENTAGE
        private const uint DescParryPercent = 0x402;         // PLAYER_PARRY_PERCENTAGE
        private const uint DescCritPercent = 0x405;          // PLAYER_CRIT_PERCENTAGE
        private const uint DescRangedCritPercent = 0x406;    // PLAYER_RANGED_CRIT_PERCENTAGE

        /// <summary>Block chance percentage (PLAYER_BLOCK_PERCENTAGE).</summary>
        public float BlockPercent => ReadDescriptor<float>(DescBlockPercent);

        /// <summary>Dodge chance percentage (PLAYER_DODGE_PERCENTAGE).</summary>
        public float DodgePercent => ReadDescriptor<float>(DescDodgePercent);

        /// <summary>Parry chance percentage (PLAYER_PARRY_PERCENTAGE).</summary>
        public float ParryPercent => ReadDescriptor<float>(DescParryPercent);

        /// <summary>Melee crit chance percentage (PLAYER_CRIT_PERCENTAGE).</summary>
        public float CritPercent => ReadDescriptor<float>(DescCritPercent);

        /// <summary>Ranged crit chance percentage (PLAYER_RANGED_CRIT_PERCENTAGE).</summary>
        public float RangedCritPercent => ReadDescriptor<float>(DescRangedCritPercent);

        #endregion
        
        #region Money & Experience Properties
        
        /// <summary>
        /// BUG-16: Changed from int to uint to avoid overflow at high gold amounts.
        /// WotLK gold cap is ~214k gold = ~2.14 billion copper, fits uint but overflows int.
        /// </summary>
        public uint Coinage => ReadDescriptor<uint>(DescCoinage);
        
        public uint Gold => Coinage / 10000;
        /// <summary>
        /// Total money expressed in silver. HB 3.3.5a/4.3.4: Coinage / 100.
        /// </summary>
        public uint Silver => Coinage / 100;
        /// <summary>
        /// Total money expressed in copper (same as Coinage).
        /// HB 3.3.5a: Copper == Coinage. HB 4.3.4: Copper reads the raw descriptor.
        /// </summary>
        public uint Copper => Coinage;
        
        public uint Honor => ReadDescriptor<uint>(DescHonor);
        
        public uint XP => ReadDescriptor<uint>(DescXP);
        public uint NextLevelXP => ReadDescriptor<uint>(DescNextLevelXP);

        public float LevelFraction => NextLevelXP > 0 ? (float)(Level + (double)XP / NextLevelXP) : Level;
        
        // Aliases for HB compatibility
        public uint Experience => XP;
        public uint NextLevelExperience => NextLevelXP;
        
        public double XPPercent => NextLevelXP > 0 ? (double)XP / NextLevelXP * 100 : 0;
        

        #endregion
        
        #region Player Flags
        
        public bool IsGroupLeader => PlayerFlags[1];
        public bool IsAFKFlagged => PlayerFlags[2];
        public bool IsDNDFlagged => PlayerFlags[4];
        public bool IsGM => PlayerFlags[8];
        public bool IsResting => PlayerFlags[32];
        public bool IsFFAPvPFlagged => PlayerFlags[128];
        public bool ContestedPvPFlagged => PlayerFlags[256];
        public bool IsPvPFlagged => PlayerFlags[512];
        public bool IsHidingHelm => PlayerFlags[1024];
        public bool IsHidingCloak => PlayerFlags[2048];
        public bool IsOutOfBounds => PlayerFlags[16384];
        public bool IsInsideSanctuary => PlayerFlags[65536];
        public bool IsPvPTimerActive => PlayerFlags[262144];
        
        #endregion

        #region Duel Properties

        // Absolute descriptor indices from 335offsetsall.txt
        private const uint DescDuelArbiter = 0x94;           // PLAYER_DUEL_ARBITER (size 2, GUID)
        private const uint DescDuelTeam = 0x9C;              // PLAYER_DUEL_TEAM

        /// <summary>
        /// Gets the player's duel team (0 = not dueling, 1 or 2 = team number)
        /// </summary>
        public uint DuelTeam => ReadDescriptor<uint>(DescDuelTeam);

        /// <summary>
        /// Gets the GUID of the duel arbiter (the flag in the ground during a duel)
        /// </summary>
        public ulong DuelArbiterGuid => ReadDescriptor<ulong>(DescDuelArbiter);

        /// <summary>
        /// Returns true if this player is currently in a duel.
        /// </summary>
        public bool IsDueling => DuelTeam != 0;



        #endregion

        #region Extended Combat Stats (FEAT-26)

        // Absolute descriptor indices from 335offsetsall.txt
        private const uint DescExpertise = 0x403;                    // PLAYER_EXPERTISE
        private const uint DescOffHandExpertise = 0x404;             // PLAYER_OFFHAND_EXPERTISE
        private const uint DescOffHandCritPercent = 0x407;           // PLAYER_OFFHAND_CRIT_PERCENTAGE
        private const uint DescSpellCritPercent1 = 0x408;            // PLAYER_SPELL_CRIT_PERCENTAGE1 (7 schools)
        private const uint DescShieldBlock = 0x40F;                  // PLAYER_SHIELD_BLOCK
        private const uint DescShieldBlockCritPercent = 0x410;       // PLAYER_SHIELD_BLOCK_CRIT_PERCENTAGE
        private const uint DescModDamageDonePos = 0x493;             // PLAYER_FIELD_MOD_DAMAGE_DONE_POS (7 schools)
        private const uint DescModDamageDoneNeg = 0x49A;             // PLAYER_FIELD_MOD_DAMAGE_DONE_NEG (7 schools)
        private const uint DescModDamageDonePct = 0x4A1;             // PLAYER_FIELD_MOD_DAMAGE_DONE_PCT (7 schools)
        private const uint DescModHealingDonePos = 0x4A8;            // PLAYER_FIELD_MOD_HEALING_DONE_POS
        private const uint DescModHealingPct = 0x4A9;                // PLAYER_FIELD_MOD_HEALING_PCT
        private const uint DescModHealingDonePct = 0x4AA;            // PLAYER_FIELD_MOD_HEALING_DONE_PCT
        private const uint DescModTargetResistance = 0x4AB;          // PLAYER_FIELD_MOD_TARGET_RESISTANCE
        private const uint DescModTargetPhysicalResistance = 0x4AC;  // PLAYER_FIELD_MOD_TARGET_PHYSICAL_RESISTANCE
        private const uint DescCombatRating1 = 0x4CF;                // PLAYER_FIELD_COMBAT_RATING_1 (25 entries)

        /// <summary>Expertise rating (PLAYER_EXPERTISE).</summary>
        public uint Expertise => ReadDescriptor<uint>(DescExpertise);

        /// <summary>Off-hand expertise rating (PLAYER_OFFHAND_EXPERTISE).</summary>
        public uint OffHandExpertise => ReadDescriptor<uint>(DescOffHandExpertise);

        /// <summary>Off-hand crit percentage (PLAYER_OFFHAND_CRIT_PERCENTAGE).</summary>
        public float OffHandCritPercent => ReadDescriptor<float>(DescOffHandCritPercent);

        /// <summary>Shield block value (PLAYER_SHIELD_BLOCK).</summary>
        public uint ShieldBlock => ReadDescriptor<uint>(DescShieldBlock);

        /// <summary>Shield block crit percentage (PLAYER_SHIELD_BLOCK_CRIT_PERCENTAGE).</summary>
        public float ShieldBlockCritPercent => ReadDescriptor<float>(DescShieldBlockCritPercent);

        // Spell crit by school (7 schools: Physical, Holy, Fire, Nature, Frost, Shadow, Arcane)
        /// <summary>Spell crit percentage for the specified school index (0-6).</summary>
        public float GetSpellCritPercent(int schoolIndex)
        {
            if (schoolIndex < 0 || schoolIndex > 6) return 0f;
            return ReadDescriptor<float>(DescSpellCritPercent1 + (uint)schoolIndex);
        }

        /// <summary>Physical spell crit percentage (school 0).</summary>
        public float PhysicalCritPercent => ReadDescriptor<float>(DescSpellCritPercent1);
        /// <summary>Holy spell crit percentage.</summary>
        public float HolyCritPercent => ReadDescriptor<float>(DescSpellCritPercent1 + 1);
        /// <summary>Fire spell crit percentage.</summary>
        public float FireCritPercent => ReadDescriptor<float>(DescSpellCritPercent1 + 2);
        /// <summary>Nature spell crit percentage.</summary>
        public float NatureCritPercent => ReadDescriptor<float>(DescSpellCritPercent1 + 3);
        /// <summary>Frost spell crit percentage.</summary>
        public float FrostCritPercent => ReadDescriptor<float>(DescSpellCritPercent1 + 4);
        /// <summary>Shadow spell crit percentage.</summary>
        public float ShadowCritPercent => ReadDescriptor<float>(DescSpellCritPercent1 + 5);
        /// <summary>Arcane spell crit percentage.</summary>
        public float ArcaneCritPercent => ReadDescriptor<float>(DescSpellCritPercent1 + 6);

        // Spell bonus damage positive by school (7 schools)
        /// <summary>Gets positive spell bonus damage for the specified school index (0-6).</summary>
        public int GetSpellBonusDamagePositive(int schoolIndex)
        {
            if (schoolIndex < 0 || schoolIndex > 6) return 0;
            return ReadDescriptor<int>(DescModDamageDonePos + (uint)schoolIndex);
        }

        /// <summary>Physical bonus damage (positive, school 0).</summary>
        public int PhysicalBonusPositive => ReadDescriptor<int>(DescModDamageDonePos);
        /// <summary>Holy bonus damage (positive).</summary>
        public int HolyBonusPositive => ReadDescriptor<int>(DescModDamageDonePos + 1);
        /// <summary>Fire bonus damage (positive).</summary>
        public int FireBonusPositive => ReadDescriptor<int>(DescModDamageDonePos + 2);
        /// <summary>Nature bonus damage (positive).</summary>
        public int NatureBonusPositive => ReadDescriptor<int>(DescModDamageDonePos + 3);
        /// <summary>Frost bonus damage (positive).</summary>
        public int FrostBonusPositive => ReadDescriptor<int>(DescModDamageDonePos + 4);
        /// <summary>Shadow bonus damage (positive).</summary>
        public int ShadowBonusPositive => ReadDescriptor<int>(DescModDamageDonePos + 5);
        /// <summary>Arcane bonus damage (positive).</summary>
        public int ArcaneBonusPositive => ReadDescriptor<int>(DescModDamageDonePos + 6);

        // Spell bonus damage negative by school (7 schools)
        /// <summary>Gets negative spell bonus damage for the specified school index (0-6).</summary>
        public int GetSpellBonusDamageNegative(int schoolIndex)
        {
            if (schoolIndex < 0 || schoolIndex > 6) return 0;
            return ReadDescriptor<int>(DescModDamageDoneNeg + (uint)schoolIndex);
        }

        /// <summary>Physical bonus damage (negative, school 0).</summary>
        public int PhysicalBonusNegative => ReadDescriptor<int>(DescModDamageDoneNeg);
        /// <summary>Holy bonus damage (negative).</summary>
        public int HolyBonusNegative => ReadDescriptor<int>(DescModDamageDoneNeg + 1);
        /// <summary>Fire bonus damage (negative).</summary>
        public int FireBonusNegative => ReadDescriptor<int>(DescModDamageDoneNeg + 2);
        /// <summary>Nature bonus damage (negative).</summary>
        public int NatureBonusNegative => ReadDescriptor<int>(DescModDamageDoneNeg + 3);
        /// <summary>Frost bonus damage (negative).</summary>
        public int FrostBonusNegative => ReadDescriptor<int>(DescModDamageDoneNeg + 4);
        /// <summary>Shadow bonus damage (negative).</summary>
        public int ShadowBonusNegative => ReadDescriptor<int>(DescModDamageDoneNeg + 5);
        /// <summary>Arcane bonus damage (negative).</summary>
        public int ArcaneBonusNegative => ReadDescriptor<int>(DescModDamageDoneNeg + 6);

        // Spell bonus damage percent by school (7 schools)
        /// <summary>Gets spell bonus damage percent for the specified school index (0-6).</summary>
        public float GetSpellBonusDamagePercent(int schoolIndex)
        {
            if (schoolIndex < 0 || schoolIndex > 6) return 0f;
            return ReadDescriptor<float>(DescModDamageDonePct + (uint)schoolIndex);
        }

        /// <summary>Holy bonus damage percent.</summary>
        public float HolyBonusPercent => ReadDescriptor<float>(DescModDamageDonePct + 1);
        /// <summary>Fire bonus damage percent.</summary>
        public float FireBonusPercent => ReadDescriptor<float>(DescModDamageDonePct + 2);
        /// <summary>Nature bonus damage percent.</summary>
        public float NatureBonusPercent => ReadDescriptor<float>(DescModDamageDonePct + 3);
        /// <summary>Frost bonus damage percent.</summary>
        public float FrostBonusPercent => ReadDescriptor<float>(DescModDamageDonePct + 4);
        /// <summary>Shadow bonus damage percent.</summary>
        public float ShadowBonusPercent => ReadDescriptor<float>(DescModDamageDonePct + 5);
        /// <summary>Arcane bonus damage percent.</summary>
        public float ArcaneBonusPercent => ReadDescriptor<float>(DescModDamageDonePct + 6);

        // Healing
        /// <summary>Healing bonus (positive).</summary>
        public int HealingBonusPositive => ReadDescriptor<int>(DescModHealingDonePos);
        /// <summary>Healing modifier percent.</summary>
        public float HealingModifierPercent => ReadDescriptor<float>(DescModHealingPct);
        /// <summary>Healing bonus percent.</summary>
        public float HealingBonusPercent => ReadDescriptor<float>(DescModHealingDonePct);

        // Target modifiers
        /// <summary>Spell power modifier vs target (PLAYER_FIELD_MOD_TARGET_RESISTANCE).</summary>
        public int TargetResistanceModifier => ReadDescriptor<int>(DescModTargetResistance);
        /// <summary>Armor modifier vs target (PLAYER_FIELD_MOD_TARGET_PHYSICAL_RESISTANCE).</summary>
        public int TargetArmorModifier => ReadDescriptor<int>(DescModTargetPhysicalResistance);

        // Combat Ratings (25 entries)
        /// <summary>Gets a combat rating value by index (0-24).</summary>
        public int GetCombatRating(int index)
        {
            if (index < 0 || index > 24) return 0;
            return ReadDescriptor<int>(DescCombatRating1 + (uint)index);
        }

        // WotLK Combat Rating indices (CR_* enum from UpdateFields):
        // 0=WeaponSkill, 1=DefenseSkill, 2=Dodge, 3=Parry, 4=Block,
        // 5=HitMelee, 6=HitRanged, 7=HitSpell, 8=CritMelee, 9=CritRanged, 10=CritSpell,
        // 11=HitTakenMelee(unused), 12=HitTakenRanged(unused), 13=HitTakenSpell(unused),
        // 14=CritTakenMelee(Resilience), 15=CritTakenRanged(Resilience), 16=CritTakenSpell(Resilience),
        // 17=HasteMelee, 18=HasteRanged, 19=HasteSpell,
        // 20=WeaponSkillMainhand, 21=WeaponSkillOffhand, 22=WeaponSkillRanged,
        // 23=Expertise, 24=ArmorPenetration
        /// <summary>Resilience rating (index 14 = CR_CRIT_TAKEN_MELEE, base resilience in WotLK).</summary>
        public int Resilience => GetCombatRating(14);
        /// <summary>Armor penetration rating (index 24 = CR_ARMOR_PENETRATION in WotLK).</summary>
        public int ArmorPenetration => GetCombatRating(24);
        /// <summary>Melee haste rating (index 17 = CR_HASTE_MELEE).</summary>
        public int HasteRating => GetCombatRating(17);
        /// <summary>Expertise rating (index 23 = CR_EXPERTISE).</summary>
        public int ExpertiseRating => GetCombatRating(23);

        /// <summary>Spell power modifier percent (Cata+ only, does not exist in WotLK). Always 1.0f.</summary>
        public float SpellPowerModifierPercent => 1.0f;

        #endregion

        #region Player Properties (FEAT-27)

        // Absolute descriptor indices from 335offsetsall.txt
        private const uint DescPlayerBytes = 0x99;                   // PLAYER_BYTES (Skin, Face, HairStyle, HairColor)
        private const uint DescPlayerBytes2 = 0x9A;                  // PLAYER_BYTES_2 (FacialHair, etc)
        private const uint DescGuildRank = 0x98;                     // PLAYER_GUILDRANK
        private const uint DescCharacterPoints1 = 0x3FC;             // PLAYER_CHARACTER_POINTS1
        private const uint DescCharacterPoints2 = 0x3FD;             // PLAYER_CHARACTER_POINTS2
        private const uint DescRestStateExperience = 0x491;          // PLAYER_REST_STATE_EXPERIENCE
        private const uint DescFieldKills = 0x4C9;                   // PLAYER_FIELD_KILLS
        private const uint DescLifetimeHonorableKills = 0x4CC;       // PLAYER_FIELD_LIFETIME_HONORABLE_KILLS
        private const uint DescSelfResSpell = 0x4AF;                 // PLAYER_SELF_RES_SPELL
        private const uint DescWatchedFactionIndex = 0x4CE;          // PLAYER_FIELD_WATCHED_FACTION_INDEX
        private const uint DescArenaCurrency = 0x4FE;                // PLAYER_FIELD_ARENA_CURRENCY
        private const uint DescMaxLevel = 0x4FF;                     // PLAYER_FIELD_MAX_LEVEL
        private const uint DescRuneRegen1 = 0x519;                   // PLAYER_RUNE_REGEN_1 (4 floats)
        private const uint DescGlyphSlots1 = 0x520;                  // PLAYER_FIELD_GLYPH_SLOTS_1 (6 entries)
        private const uint DescGlyphs1 = 0x526;                      // PLAYER_FIELD_GLYPHS_1 (6 entries)
        private const uint DescGlyphsEnabled = 0x52C;                // PLAYER_GLYPHS_ENABLED
        private const uint DescPetSpellPower = 0x52D;                // PLAYER_PET_SPELL_POWER
        private const uint DescFieldBytes = 0x4AD;                   // PLAYER_FIELD_BYTES

        /// <summary>Talent points remaining (PLAYER_CHARACTER_POINTS1).</summary>
        public uint CharacterPoints => ReadDescriptor<uint>(DescCharacterPoints1);

        /// <summary>Profession points remaining (PLAYER_CHARACTER_POINTS2).</summary>
        public uint CharacterPoints2 => ReadDescriptor<uint>(DescCharacterPoints2);

        /// <summary>Rested XP amount (PLAYER_REST_STATE_EXPERIENCE).</summary>
        public uint RestedExperience => ReadDescriptor<uint>(DescRestStateExperience);

        /// <summary>Whether the player has rested XP.</summary>
        public bool HasRestedXp => RestedExperience > 0;

        /// <summary>Guild rank (PLAYER_GUILDRANK).</summary>
        public uint GuildRank => ReadDescriptor<uint>(DescGuildRank);

        /// <summary>Guild timestamp (PLAYER_GUILD_TIMESTAMP).</summary>
        public uint GuildTimestamp => ReadDescriptor<uint>(DescGuildTimestamp);

        /// <summary>Guild level (Cata+ only). Always 0 in WotLK.</summary>
        public uint GuildLevel => 0;

        /// <summary>Guild delete date (Cata+ only). Always 0 in WotLK.</summary>
        public uint GuildDeleteDate => 0;

        /// <summary>Number of purchased bank bag slots (byte 2 of PLAYER_BYTES_2, 0x9A).</summary>
        public byte BankBagSlotCount
        {
            get
            {
                uint bytes2 = ReadDescriptor<uint>(DescPlayerBytes2);
                return (byte)((bytes2 >> 16) & 0xFF);
            }
        }

        /// <summary>Whether the player is tracking stealthed units (bit 1 of PlayerFieldBytes byte 0).</summary>
        public bool IsTrackingStealthed
        {
            get
            {
                uint fieldBytes = ReadDescriptor<uint>(DescFieldBytes);
                return ((byte)(fieldBytes & 0xFF) & 0x02) != 0;
            }
        }

        /// <summary>Whether the corpse release timer is visible (bit 3 of PlayerFieldBytes byte 0).</summary>
        public bool ReleaseTimerIsVisible
        {
            get
            {
                uint fieldBytes = ReadDescriptor<uint>(DescFieldBytes);
                return ((byte)(fieldBytes & 0xFF) & 0x08) != 0;
            }
        }

        /// <summary>Pet spell power scaling for hunter/warlock pets (PLAYER_PET_SPELL_POWER).</summary>
        public uint PetSpellPower => ReadDescriptor<uint>(DescPetSpellPower);

        /// <summary>DK rune regeneration rates (4 floats, PLAYER_RUNE_REGEN_1).</summary>
        public float[] RuneRegen
        {
            get
            {
                return new float[]
                {
                    ReadDescriptor<float>(DescRuneRegen1),
                    ReadDescriptor<float>(DescRuneRegen1 + 1),
                    ReadDescriptor<float>(DescRuneRegen1 + 2),
                    ReadDescriptor<float>(DescRuneRegen1 + 3)
                };
            }
        }

        /// <summary>Glyph enabled bitmask (PLAYER_GLYPHS_ENABLED).</summary>
        public uint GlyphsEnabled => ReadDescriptor<uint>(DescGlyphsEnabled);

        /// <summary>Gets the glyph spell ID at the specified slot (0-5).</summary>
        public uint GetGlyph(int slot)
        {
            if (slot < 0 || slot > 5) return 0;
            return ReadDescriptor<uint>(DescGlyphs1 + (uint)slot);
        }

        /// <summary>Gets the glyph slot type at the specified index (0-5).</summary>
        public uint GetGlyphSlot(int slot)
        {
            if (slot < 0 || slot > 5) return 0;
            return ReadDescriptor<uint>(DescGlyphSlots1 + (uint)slot);
        }

        /// <summary>Returns all 6 glyph slots as a list (WotLK: 3 major + 3 minor).</summary>
        public List<WoWGlyphInfo> Glyphs
        {
            get
            {
                var list = new List<WoWGlyphInfo>(6);
                uint enabledMask = GlyphsEnabled;
                for (int i = 0; i < 6; i++)
                {
                    uint glyphId = GetGlyph(i);
                    uint slotType = GetGlyphSlot(i);
                    bool isEnabled = (enabledMask & (1u << i)) != 0;
                    list.Add(new WoWGlyphInfo(i, slotType, glyphId, isEnabled));
                }
                return list;
            }
        }

        /// <summary>Today's honorable kills (lower 16 bits of PLAYER_FIELD_KILLS).</summary>
        public ushort HonorableKillsToday
        {
            get
            {
                uint kills = ReadDescriptor<uint>(DescFieldKills);
                return (ushort)(kills & 0xFFFF);
            }
        }

        /// <summary>Yesterday's honorable kills (upper 16 bits of PLAYER_FIELD_KILLS).</summary>
        public ushort HonorableKillsYesterday
        {
            get
            {
                uint kills = ReadDescriptor<uint>(DescFieldKills);
                return (ushort)((kills >> 16) & 0xFFFF);
            }
        }

        /// <summary>Lifetime honorable kills (PLAYER_FIELD_LIFETIME_HONORABLE_KILLS).</summary>
        public uint LifetimeHonorableKills => ReadDescriptor<uint>(DescLifetimeHonorableKills);

        /// <summary>Self-resurrection spell ID (e.g., Soulstone, Ankh). 0 = none.</summary>
        public uint SelfResurrectSpellId => ReadDescriptor<uint>(DescSelfResSpell);

        /// <summary>Watched faction index (PLAYER_FIELD_WATCHED_FACTION_INDEX).</summary>
        public uint WatchedFactionIndex => ReadDescriptor<uint>(DescWatchedFactionIndex);

        /// <summary>Arena points (PLAYER_FIELD_ARENA_CURRENCY).</summary>
        public uint ArenaCurrency => ReadDescriptor<uint>(DescArenaCurrency);

        /// <summary>PvP medal count (PLAYER_FIELD_PVP_MEDALS).</summary>
        public uint PvpMedalCount => ReadDescriptor<uint>(DescPvpMedalCount);

        /// <summary>Max level (80 in WotLK).</summary>
        public uint MaxLevel => ReadDescriptor<uint>(DescMaxLevel);

        // Appearance bytes from PLAYER_BYTES (0x99)
        /// <summary>Skin color/type (byte 0 of PLAYER_BYTES).</summary>
        public byte Skin
        {
            get
            {
                uint bytes = ReadDescriptor<uint>(DescPlayerBytes);
                return (byte)(bytes & 0xFF);
            }
        }

        /// <summary>Face type (byte 1 of PLAYER_BYTES).</summary>
        public byte FaceType
        {
            get
            {
                uint bytes = ReadDescriptor<uint>(DescPlayerBytes);
                return (byte)((bytes >> 8) & 0xFF);
            }
        }

        /// <summary>Hair style (byte 2 of PLAYER_BYTES).</summary>
        public byte HairStyle
        {
            get
            {
                uint bytes = ReadDescriptor<uint>(DescPlayerBytes);
                return (byte)((bytes >> 16) & 0xFF);
            }
        }

        /// <summary>Hair color (byte 3 of PLAYER_BYTES).</summary>
        public byte HairColor
        {
            get
            {
                uint bytes = ReadDescriptor<uint>(DescPlayerBytes);
                return (byte)((bytes >> 24) & 0xFF);
            }
        }

        /// <summary>Facial hair style (byte 0 of PLAYER_BYTES_2).</summary>
        public byte FacialHair
        {
            get
            {
                uint bytes = ReadDescriptor<uint>(DescPlayerBytes2);
                return (byte)(bytes & 0xFF);
            }
        }

        /// <summary>Battlefield arena faction (byte 3 of PLAYER_BYTES_3).</summary>
        public byte BattlefieldArenaFaction
        {
            get
            {
                uint bytes = ReadDescriptor<uint>(DescPlayerBytes3);
                return (byte)((bytes >> 24) & 0xFF);
            }
        }

        /// <summary>Currently displayed title (PLAYER_CHOSEN_TITLE).</summary>
        public uint ChosenTitle => ReadDescriptor<uint>(DescChosenTitle);

        /// <summary>Player inebriation level (PLAYER_FIELD_INEBRIATION).</summary>
        public WoWInebriationLevel Inebriation => (WoWInebriationLevel)ReadDescriptor<uint>(DescInebriation);

        #endregion
        
        #region Equipment Properties
        
        public uint MainhandEntryId => ReadDescriptor<uint>(0x11D);  // Item entry ID in mainhand slot
        public uint OffhandEntryId => ReadDescriptor<uint>(0x11F);   // Item entry ID in offhand slot
        
        public WoWItem? Mainhand
        {
            get
            {
                var entryId = MainhandEntryId;
                if (entryId == 0) return null;
                return ObjectManager.CachedItems.FirstOrDefault(item => item.Entry == entryId);
            }
        }
        
        public WoWItem? Offhand
        {
            get
            {
                var entryId = OffhandEntryId;
                if (entryId == 0) return null;
                return ObjectManager.CachedItems.FirstOrDefault(item => item.Entry == entryId);
            }
        }
        
        #endregion
        
        #region Mounted Override
        
        public override bool Mounted
        {
            get
            {
                if (Class == WoWClass.Druid)
                {
                    var shapeshift = Shapeshift;
                    if (shapeshift == ShapeshiftForm.FlightForm || shapeshift == ShapeshiftForm.EpicFlightForm)
                        return true;
                }
                return base.Mounted;
            }
        }
        
        #endregion

        #region Minions Property
        
        /// <summary>
        /// Gets all minions (pets, totems, etc.) controlled by this player.
        /// Excludes non-combat pets.
        /// </summary>
        public List<WoWUnit> Minions
        {
            get
            {
                // filtering on cached units reduces allocation/memory access
                return ObjectManager.CachedUnits
                    .Where(u => !u.IsNonCombatPet && (WoWObject)u.ControllingPlayer == (WoWObject)this)
                    .ToList();
            }
        }

        #endregion

        #region Cata Stubs — CATA-02/03

        /// <summary>Mastery rating (Cataclysm+ only). Always 0 in WotLK.</summary>
        public float Mastery => 0f;

        /// <summary>Mastery percent (Cataclysm+ only). Always 0 in WotLK.</summary>
        public float MasteryPercent => 0f;

        #endregion
    }
}
