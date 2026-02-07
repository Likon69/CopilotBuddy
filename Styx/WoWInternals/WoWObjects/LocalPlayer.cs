using System;
using System.Collections.Generic;
using System.Linq;
using GreenMagic;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.Logic.Questing;
using Styx.Patchables;
using Styx.WoWInternals.DBC;
using Styx.WoWInternals.WoWCache;

namespace Styx.WoWInternals.WoWObjects
{
    /// <summary>
    /// Enumeration of Death Knight rune types.
    /// </summary>
    public enum RuneType : byte
    {
        Unknown = 0,
        Blood = 1,
        Unholy = 2,
        Frost = 3,
        Death = 4
    }

    public class LocalPlayer : WoWPlayer
    {
        #region Constants - Static Addresses 3.3.5a
        
        // Adresses statiques pour le joueur local
        private const uint AccountNamePtr = 0xB6A400;       // 11971136U
        private const uint CorpsePointPtr = 0xBD1F48;       // 12388952U  
        private const uint ZoneIdPtr = 0xBD1D7C;            // 12388364U
        private const uint MapIdPtr = 0xAB63BC;             // 11232188U - From Offsets.txt: CurrentMapId
        private const uint ContinentNamePtr = 0xCD8F20;     // 13469216U
        private const uint RealmNamePtr = 0xBD1CF4;         // 12388228U (approximate)
        private const uint PlayerNamePtr = 0xBD1E28;        // 12388520U
        private const uint XPPtr = 0xBE5D88;                // 12475784U
        private const uint RestingPtr = 0xAF7154;           // 11489876U
        private const uint LastRedErrorPtr = 0xBCF390;      // 12385168U
        private const uint KnownSpellsPtr = 0xBE5D88;       // 12475784U - Start of spell list
        
        #endregion
        
        #region Constructor
        public LocalPlayer(uint baseAddress) : base(baseAddress)
        {
        }
        
        #endregion
        
        #region Account & Character Info
        public string AccountName
        {
            get
            {
                if (Memory == null) return string.Empty;
                try
                {
                    return Memory.ReadString(AccountNamePtr, 32);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public override string Name
        {
            get
            {
                if (Memory == null) return "Unknown";
                try
                {
                    return Memory.ReadString(PlayerNamePtr, 32);
                }
                catch
                {
                    return "Unknown";
                }
            }
        }
        public string RealmName
        {
            get
            {
                if (Memory == null) return string.Empty;
                try
                {
                    return Memory.ReadString(RealmNamePtr, 32);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the current minimap zone text.
        /// Ported from HB 3.3.5a - reads pointer at 12388220U then string at that address.
        /// </summary>
        public string MinimapZoneText
        {
            get
            {
                try
                {
                    Memory? wow = ObjectManager.Wow;
                    if (wow == null) return string.Empty;
                    uint[] ptrArray = new uint[]
                    {
                        12388220U
                    };
                    uint textPtr = wow.Read<uint>(ptrArray);
                    
                    uint[] textArray = new uint[]
                    {
                        textPtr
                    };
                    return wow.Read<string>(textArray);
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return string.Empty;
                }
            }
        }
        
        #endregion
        
        #region Location Info
        public uint ZoneId
        {
            get
            {
                if (Memory == null) return 0U;
                return Memory.Read<uint>(ZoneIdPtr);
            }
        }
        public uint MapId
        {
            get
            {
                if (Memory == null) return 0U;
                return Memory.Read<uint>(MapIdPtr);
            }
        }
        
        private Map? _currentMap;
        private uint _currentMapCachedId;
        
        /// <summary>
        /// Gets the current map information from the Map.dbc.
        /// Based on HB 4.3.4.
        /// </summary>
        public Map CurrentMap
        {
            get
            {
                uint mapId = MapId;
                
                // Cache the Map object and invalidate if mapId changes
                if (_currentMap == null || _currentMapCachedId != mapId)
                {
                    _currentMap = new Map(mapId);
                    _currentMapCachedId = mapId;
                }
                
                return _currentMap;
            }
        }
        
        public string ContinentName
        {
            get
            {
                if (Memory == null) return string.Empty;
                try
                {
                    return Memory.ReadString(ContinentNamePtr, 64);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the real zone text (unmodified by subzones).
        /// Ported from HB 3.3.5a - Address 12388224U
        /// </summary>
        public string RealZoneText
        {
            get
            {
                try
                {
                    Memory? wow = ObjectManager.Wow;
                    if (wow == null) return string.Empty;
                    uint[] ptrArray = new uint[]
                    {
                        12388224U
                    };
                    uint textPtr = wow.Read<uint>(ptrArray);
                    uint[] textArray = new uint[]
                    {
                        textPtr
                    };
                    return wow.Read<string>(textArray);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the zone text (may be modified by subzones).
        /// Ported from HB 3.3.5a - Address 12388232U
        /// </summary>
        public string ZoneText
        {
            get
            {
                try
                {
                    Memory? wow = ObjectManager.Wow;
                    if (wow == null) return string.Empty;
                    uint[] ptrArray = new uint[]
                    {
                        12388232U
                    };
                    uint textPtr = wow.Read<uint>(ptrArray);
                    uint[] textArray = new uint[]
                    {
                        textPtr
                    };
                    return wow.Read<string>(textArray);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the subzone text.
        /// Ported from HB 3.3.5a - Address 12388228U
        /// </summary>
        public string SubZoneText
        {
            get
            {
                try
                {
                    Memory? wow = ObjectManager.Wow;
                    if (wow == null) return string.Empty;
                    uint[] ptrArray = new uint[]
                    {
                        12388228U
                    };
                    uint textPtr = wow.Read<uint>(ptrArray);
                    uint[] textArray = new uint[]
                    {
                        textPtr
                    };
                    return wow.Read<string>(textArray);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the map name.
        /// Ported from HB 3.3.5a - Address 13502160U
        /// </summary>
        public string MapName
        {
            get
            {
                try
                {
                    Memory? wow = ObjectManager.Wow;
                    if (wow == null) return string.Empty;
                    uint[] array = new uint[]
                    {
                        13502160U
                    };
                    return wow.Read<string>(array);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public WoWPoint CorpsePoint
        {
            get
            {
                if (Memory == null) return WoWPoint.Empty;
                return Memory.Read<WoWPoint>(CorpsePointPtr);
            }
        }

        internal static WoWPoint InstanceDeathLocation { get; set; }

        public WoWPoint InstanceCorpseLocation
        {
            get
            {
                return InstanceDeathLocation != WoWPoint.Empty ? InstanceDeathLocation : CorpsePoint;
            }
        }
        
        #endregion
        
        #region Experience & Rest
        public uint CurrentXP
        {
            get
            {
                if (Memory == null) return 0U;
                return Memory.Read<uint>(XPPtr);
            }
        }
        public uint XPToNextLevel
        {
            get
            {
                // Formule WoW 3.3.5a approximative
                int level = Level;
                if (level >= 80) return 0;
                
                // Formule simplifiée
                return (uint)(level * level * 100 + level * 500);
            }
        }
        public new double XPPercent
        {
            get
            {
                uint needed = XPToNextLevel;
                if (needed == 0) return 100;
                return (CurrentXP * 100.0) / needed;
            }
        }
        public new bool IsResting
        {
            get
            {
                if (Memory == null) return false;
                return Memory.Read<byte>(RestingPtr) != 0;
            }
        }
        
        #endregion
        
        #region Combat State
        public bool IsInCombat
        {
            get
            {
                return Combat;
            }
        }

        public bool IsActuallyInCombat
        {
            get
            {
                return (Combat || PetInCombat) && 
                       !ObjectManager.GetObjectsOfType<WoWUnit>().All(u => !u.Aggro) && 
                       Targeting.Instance.FirstUnit != null;
            }
        }

        public override bool IsGhost
        {
            get
            {
                return base.IsDead && CorpsePoint != WoWPoint.Empty;
            }
        }
        
        #endregion
        
        #region Target
        public WoWUnit? LocalTarget
        {
            get
            {
                ulong targetGuid = LocalTargetGuid;
                if (targetGuid == 0) return null;
                return ObjectManager.GetObjectByGuid<WoWUnit>(targetGuid);
            }
        }
        public ulong LocalTargetGuid
        {
            get
            {
                // Lire depuis l'adresse statique de la cible
                if (Memory == null) return 0UL;
                return Memory.Read<ulong>(0xBD07B0); // TARGET_GUID_PTR
            }
        }
        
        #endregion
        
        #region Pet
        public bool HasPet
        {
            get
            {
                return Pet != null;
            }
        }
        public override WoWUnit? Pet
        {
            get
            {
                ulong guid = base.Charmed;
                if (guid == 0) guid = base.Summon;
                if (guid == 0) return null;
                return ObjectManager.GetObjectByGuid<WoWUnit>(guid);
            }
        }
        
        #endregion
        
        #region Movement
        public bool IsMounted
        {
            get
            {
                return Mounted;
            }
        }
        public bool IsTravelForm
        {
            get
            {
                ShapeshiftForm form = Shapeshift;
                return form == ShapeshiftForm.FlightForm ||
                       form == ShapeshiftForm.EpicFlightForm ||
                       form == ShapeshiftForm.Aqua ||
                       form == ShapeshiftForm.Travel;
            }
        }
        
        #endregion
        
        #region Focus Target
        /// <summary>
        /// Gets the GUID of the focused unit.
        /// </summary>
        public ulong FocusedUnitGuid
        {
            get
            {
                if (Memory == null) return 0;
                try
                {
                    // FocusGuid offset from GlobalOffsets
                    return Memory.Read<ulong>(0x00BD07C8);
                }
                catch
                {
                    return 0;
                }
            }
        }
        
        /// <summary>
        /// Gets the focused unit (if any).
        /// </summary>
        public WoWUnit FocusedUnit
        {
            get
            {
                ulong guid = FocusedUnitGuid;
                if (guid == 0) return null;
                return ObjectManager.GetObjectByGuid<WoWUnit>(guid);
            }
        }
        #endregion
        
        #region Override Properties
        public new bool IsMe => true;
        public override double Distance => 0;
        public override double DistanceSqr => 0;
        
        #endregion
        
        #region Error Messages
        public string LastRedErrorMessage
        {
            get
            {
                if (Memory == null) return string.Empty;
                try
                {
                    return Memory.ReadString(LastRedErrorPtr, 256);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Whether there's a loading screen.
        /// Deprecated: Use StyxWoW.IsInGame instead.
        /// Ported from HB 3.3.5a for compatibility.
        /// </summary>
        [Obsolete("Use StyxWoW.IsInGame instead. This property is deprecated, and will be removed in a future release.")]
        public bool LoadingScreen
        {
            get
            {
                try
                {
                    return !StyxWoW.IsInGame;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return false;
                }
            }
        }
        
        #endregion
        
        #region Known Spells
        public List<WoWSpell> KnownSpells
        {
            get
            {
                List<WoWSpell> spells = new List<WoWSpell>();
                if (Memory == null) return spells;
                
                try
                {
                    uint address = KnownSpellsPtr;
                    uint spellId = Memory.Read<uint>(address);
                    
                    while (spellId != 0U)
                    {
                        WoWSpell? spell = WoWSpell.FromId((int)spellId);
                        if (spell != null)
                        {
                            spells.Add(spell);
                        }
                        address += 4U;
                        spellId = Memory.Read<uint>(address);
                    }
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                }
                
                return spells;
            }
        }
        
        #endregion

        #region Skills

        // Adresses statiques pour les skills 3.3.5a
        private const uint SkillsBasePtr = 0xBE55C8;      // Base address for skill data
        private const int MaxSkills = 128;
        private const int SkillEntrySize = 12;            // sizeof(SkillInfo)

        /// <summary>
        /// Gets a skill by skill line.
        /// </summary>
        public WoWSkill? GetSkill(SkillLine skillLine)
        {
            return GetSkill((int)skillLine);
        }

        /// <summary>
        /// Gets a skill by skill line ID.
        /// </summary>
        public WoWSkill? GetSkill(int skillLineId)
        {
            return GetSkill((uint)skillLineId);
        }

        /// <summary>
        /// Gets a skill by skill line ID.
        /// </summary>
        public WoWSkill? GetSkill(uint skillLineId)
        {
            if (Memory == null) return null;

            // Scan through skill slots to find matching skill
            for (int i = 0; i < MaxSkills; i++)
            {
                uint skillPtr = SkillsBasePtr + (uint)(i * SkillEntrySize);
                ushort id = Memory.Read<ushort>(skillPtr);

                if (id == skillLineId)
                {
                    return new WoWSkill(skillPtr);
                }

                // Empty slot, stop scanning
                if (id == 0) break;
            }

            return null;
        }

        /// <summary>
        /// Gets all known skills.
        /// </summary>
        public List<WoWSkill> GetAllSkills()
        {
            List<WoWSkill> skills = new List<WoWSkill>();
            if (Memory == null) return skills;

            for (int i = 0; i < MaxSkills; i++)
            {
                uint skillPtr = SkillsBasePtr + (uint)(i * SkillEntrySize);
                ushort id = Memory.Read<ushort>(skillPtr);

                if (id == 0) break;

                WoWSkill skill = new WoWSkill(skillPtr);
                if (skill.IsValid)
                {
                    skills.Add(skill);
                }
            }

            return skills;
        }

        /// <summary>
        /// Gets the maximum level of creatures this player can skin based on Skinning skill.
        /// </summary>
        public int CanSkinLevel
        {
            get
            {
                WoWSkill? skinning = GetSkill(SkillLine.Skinning);
                if (skinning == null) return 0;
                
                int currentValue = skinning.CurrentValue;
                if (currentValue == 0) return 0;
                
                return currentValue < 100 ? currentValue / 10 + 10 : currentValue / 5;
            }
        }

        #endregion

        #region Party & Raid Members

        // Adresses statiques 3.3.5a pour groupe/raid
        private const uint PartyMemberGuidsPtr = 0xBD1DD8;   // 12392776 - Party member GUIDs (5 slots * 8 bytes)
        private const uint RaidMemberPtrsPtr = 0xBECFC8;     // 12498280 - Raid member pointers (40 slots * 4 bytes)

        /// <summary>
        /// Gets the GUID of a party member by index (0-4).
        /// Renamed method - use GetPartyMemberGuid instead.
        /// Ported from HB 3.3.5a
        /// </summary>
        [Obsolete("This has been renamed. Use GetPartyMemberGuid instead.")]
        public ulong GetPartyMemberGUID(int index)
        {
            return GetPartyMemberGuid(index);
        }

        /// <summary>
        /// Gets the GUID of a party member by index (0-3).
        /// Ported from HB 3.3.5a - Address 12392776 (0xBD1DD8)
        /// </summary>
        public ulong GetPartyMemberGuid(int index)
        {
            if (Memory == null || index < 0 || index > 3)
                return 0UL;

            return Memory.Read<ulong>((uint)(12392776 + index * 8));
        }

        /// <summary>
        /// Gets the GUID of a raid member by index (0-39).
        /// Renamed method - use GetRaidMemberGuid instead.
        /// Ported from HB 3.3.5a
        /// </summary>
        [Obsolete("Use GetRaidMemberGuid. This has been renamed.")]
        public ulong GetRaidMemberGUID(int index)
        {
            return GetRaidMemberGuid(index);
        }

        /// <summary>
        /// Gets the GUID of a raid member by index (0-39).
        /// Ported from HB 3.3.5a - Address 12498280 (0xBECFC8)
        /// </summary>
        public ulong GetRaidMemberGuid(int index)
        {
            if (Memory == null || index < 0 || index > 39)
                return 0UL;

            Memory? wow = ObjectManager.Wow;
            if (wow == null) return 0UL;
            uint[] ptrArray = new uint[]
            {
                (uint)(12498280 + index * 4)
            };
            uint raidMemberPtr = wow.Read<uint>(ptrArray);
            if (raidMemberPtr == 0U)
                return 0UL;

            uint[] guidArray = new uint[]
            {
                raidMemberPtr
            };
            return wow.Read<ulong>(guidArray);
        }

        /// <summary>
        /// Gets a raid member by index.
        /// Ported from HB 3.3.5a
        /// </summary>
        public WoWPlayer? GetRaidMember(int index)
        {
            return ObjectManager.GetObjectByGuid<WoWPlayer>(GetRaidMemberGuid(index));
        }

        /// <summary>
        /// Gets all party member GUIDs.
        /// </summary>
        public ulong[] GetPartyMemberGUIDs()
        {
            ulong[] guids = new ulong[5];
            for (int i = 0; i < 5; i++)
            {
                guids[i] = GetPartyMemberGuid(i);
            }
            return guids;
        }

        /// <summary>
        /// Gets all raid member GUIDs.
        /// </summary>
        public ulong[] GetRaidMemberGUIDs()
        {
            ulong[] guids = new ulong[40];
            for (int i = 0; i < 40; i++)
            {
                guids[i] = GetRaidMemberGuid(i);
            }
            return guids;
        }

        /// <summary>
        /// Gets all party member GUIDs as array (non-zero values only).
        /// </summary>
        public ulong[] PartyMemberGuids
        {
            get
            {
                return GetPartyMemberGUIDs().Where(guid => guid != 0).ToArray();
            }
        }

        /// <summary>
        /// Gets all raid member GUIDs as array (non-zero values only).
        /// </summary>
        public ulong[] RaidMemberGuids
        {
            get
            {
                return GetRaidMemberGUIDs().Where(guid => guid != 0).ToArray();
            }
        }

        /// <summary>
        /// Gets party member infos (WoWPartyMember objects).
        /// </summary>
        public List<WoWPartyMember> PartyMemberInfos
        {
            get
            {
                var list = new List<WoWPartyMember>();
                foreach (ulong guid in PartyMemberGuids)
                {
                    if (guid != Guid && guid != 0)
                        list.Add(new WoWPartyMember(guid, true));
                }
                return list;
            }
        }

        /// <summary>
        /// Gets party members as WoWPlayer objects (only alive/valid players).
        /// </summary>
        public List<WoWPlayer> PartyMembers
        {
            get
            {
                return PartyMemberGuids
                    .Select(guid => ObjectManager.GetObjectByGuid<WoWPlayer>(guid))
                    .Where(player => player != null)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets the list of pet spells (action bar).
        /// </summary>
        public List<WoWPetSpell> PetSpells
        {
            get
            {
                // TODO: Find correct WotLK 3.3.5a offset for pet spells array
                // The offset 5057 from HB 4.3.4 causes crashes - disabled for now
                // Use Lua to get pet spells instead:
                // HasPetSpells() returns numPetSpells, petToken
                List<WoWPetSpell> petSpells = new List<WoWPetSpell>();
                try
                {
                    var hasPet = Lua.GetReturnVal<bool>("return HasPetUI()", 0);
                    if (!hasPet) return petSpells;
                    
                    for (int index = 1; index <= 10; ++index)
                    {
                        var spellId = Lua.GetReturnVal<int>($"local name,_,_,_,_,_,spellId = GetPetActionInfo({index}); return spellId or 0", 0);
                        petSpells.Add(new WoWPetSpell((uint)spellId, index - 1));
                    }
                }
                catch { }
                return petSpells;
            }
        }

        /// <summary>
        /// Sets the focus target by WoWUnit.
        /// </summary>
        public void SetFocus(WoWUnit unit)
        {
            if (unit == null)
            {
                Lua.DoString("ClearFocus()");
                return;
            }
            Lua.DoString($"FocusUnit('target', {unit.Guid})");
        }

        /// <summary>
        /// Sets the focus target by GUID.
        /// </summary>
        public void SetFocus(ulong guid)
        {
            if (guid == 0)
            {
                Lua.DoString("ClearFocus()");
                return;
            }
            Lua.DoString($"FocusUnit('target', {guid})");
        }

        /// <summary>
        /// Gets raid member infos (WoWPartyMember objects).
        /// </summary>
        public List<WoWPartyMember> RaidMemberInfos
        {
            get
            {
                var list = new List<WoWPartyMember>();
                foreach (ulong guid in RaidMemberGuids)
                {
                    if (guid != Guid && guid != 0)
                        list.Add(new WoWPartyMember(guid, false));
                }
                return list;
            }
        }

        /// <summary>
        /// Gets the player's group role (Tank, Healer, Damage).
        /// Uses GetRaidRosterInfo() which exists in WotLK 3.3.5a (patch 3.3.0+).
        /// </summary>
        public WoWPartyMember.GroupRole Role
        {
            get
            {
                try
                {
                    // Only works in raid or LFG dungeon groups
                    if (!IsInRaid && !IsInParty)
                        return WoWPartyMember.GroupRole.None;
                    
                    // In raid, use GetRaidRosterInfo to get role
                    if (IsInRaid)
                    {
                        // Find our index in raid (1-40)
                        for (int i = 1; i <= 40; i++)
                        {
                            // GetRaidRosterInfo returns: name, rank, subgroup, level, class, fileName, 
                            // zone, online, isDead, role, isML, combatRole
                            var name = Lua.GetReturnVal<string>($"local name = GetRaidRosterInfo({i}); return name", 0);
                            
                            if (string.IsNullOrEmpty(name))
                                break; // No more raid members
                            
                            if (name.Equals(Name, StringComparison.OrdinalIgnoreCase))
                            {
                                // Get role (10th return value)
                                var role = Lua.GetReturnVal<string>(
                                    $"local _,_,_,_,_,_,_,_,_,role = GetRaidRosterInfo({i}); return role or 'NONE'", 
                                    0);
                                
                                return ParseLFGRole(role);
                            }
                        }
                    }
                    
                    // In party (5-man), roles are only assigned in LFG dungeons
                    // For normal parties, return None
                    return WoWPartyMember.GroupRole.None;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return WoWPartyMember.GroupRole.None;
                }
            }
        }
        
        /// <summary>
        /// Parse LFG role string from GetRaidRosterInfo.
        /// </summary>
        private WoWPartyMember.GroupRole ParseLFGRole(string role)
        {
            if (string.IsNullOrEmpty(role))
                return WoWPartyMember.GroupRole.None;
            
            return role.ToUpperInvariant() switch
            {
                "TANK" => WoWPartyMember.GroupRole.Tank,
                "HEALER" => WoWPartyMember.GroupRole.Healer,
                "DAMAGER" => WoWPartyMember.GroupRole.Damage,
                _ => WoWPartyMember.GroupRole.None
            };
        }

        #endregion

        #region Facing

        /// <summary>
        /// Sets the player's facing towards a unit.
        /// </summary>
        public void SetFacing(WoWUnit unit)
        {
            if (unit == null) return;
            SetFacing(unit.Location);
        }

        /// <summary>
        /// Sets the player's facing towards a game object.
        /// </summary>
        public void SetFacing(WoWGameObject gameObject)
        {
            if (gameObject == null) return;
            SetFacing(gameObject.Location);
        }

        /// <summary>
        /// Sets the player's facing towards a point.
        /// </summary>
        public void SetFacing(WoWPoint point)
        {
            float facing = WoWMathHelper.CalculateNeededFacing(Location, point);
            SetFacing(facing);
        }

        /// <summary>
        /// Sets the player's facing to a specific angle (in radians).
        /// Uses ClickToMove with Face type like HB 4.3.4 for proper server sync and animation.
        /// Adding epsilon (1E-06f) like HB 4.3.4 to prevent precision issues.
        /// </summary>
        public void SetFacing(float facing)
        {
            // Normalize facing to 0-2π
            while (facing < 0) facing += (float)(2 * Math.PI);
            while (facing >= 2 * Math.PI) facing -= (float)(2 * Math.PI);

            // Use ClickToMove with Face type for proper server-side facing update.
            // Pass WoWPoint.Zero (not WoWPoint.Empty which is NaN) — the position is
            // unused for Face type, only the facing float matters to the game.
            // Adding small epsilon like HB 4.3.4 to prevent floating-point precision issues.
            WoWMovement.ClickToMove(0UL, WoWPoint.Zero, facing + 1E-06f, WoWMovement.ClickToMoveType.Face);
        }

        #endregion

        #region Death Knight Runes

        /// <summary>
        /// Gets the rune count at the specified index (0-5).
        /// </summary>
        public byte GetRuneCount(int index)
        {
            if (Memory == null || index < 0 || index >= 6)
                return 0;

            try
            {
                uint runeMask = Memory.Read<uint>(12731272);
                return (byte)((runeMask >> index) & 1);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the rune type at the specified index (0-5).
        /// </summary>
        public RuneType GetRuneType(int index)
        {
            if (Memory == null || index < 0 || index >= 6)
                return RuneType.Unknown;

            try
            {
                return Memory.Read<RuneType>(12731140 + (uint)(4 * index));
            }
            catch
            {
                return RuneType.Unknown;
            }
        }

        /// <summary>
        /// Gets the total rune count of a specific type.
        /// </summary>
        public byte GetRuneCount(RuneType type)
        {
            byte total = 0;
            for (int i = 0; i < 6; i++)
            {
                if (GetRuneType(i) == type)
                {
                    total += GetRuneCount(i);
                }
            }
            return total;
        }

        /// <summary>
        /// Gets the count of unholy runes.
        /// </summary>
        public int UnholyRuneCount => GetRuneCount(RuneType.Unholy);

        /// <summary>
        /// Gets the count of blood runes.
        /// </summary>
        public int BloodRuneCount => GetRuneCount(RuneType.Blood);

        /// <summary>
        /// Gets the count of frost runes.
        /// </summary>
        public int FrostRuneCount => GetRuneCount(RuneType.Frost);

        /// <summary>
        /// Gets the count of death runes.
        /// </summary>
        public int DeathRuneCount => GetRuneCount(RuneType.Death);

        #endregion

        #region Combo Points & Spells

        /// <summary>
        /// Gets the current combo points on the target.
        /// Ported from HB 3.3.5a - Addresses: 12388520 (target GUID), 12388429 (combo points byte)
        /// </summary>
        public int ComboPoints
        {
            get
            {
                try
                {
                    ulong currentTargetGuid = CurrentTargetGuid;
                    if (currentTargetGuid != 0UL)
                    {
                        Memory? wow = ObjectManager.Wow;
                        if (wow == null) return 0;
                        uint[] array = new uint[]
                        {
                            12388520U
                        };
                        if (currentTargetGuid == wow.Read<ulong>(array))
                        {
                            uint[] array2 = new uint[]
                            {
                                12388429U
                            };
                            return (int)wow.Read<byte>(array2);
                        }
                    }
                    return 0;
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets raw combo points (HB 4.3.4 compatibility - no Anticipation talent in WotLK).
        /// In WotLK, this is identical to ComboPoints.
        /// </summary>
        public int RawComboPoints => ComboPoints;

        /// <summary>
        /// Gets the spell ID currently being cast (HB 4.3.4 compatibility).
        /// </summary>
        public int CurrentCastId => CastingSpellId;

        /// <summary>
        /// Gets the current repeating spell ID (auto-attack spell).
        /// </summary>
        public int AuthRepeatingSpellId
        {
            get
            {
                if (Memory == null)
                    return 0;

                try
                {
                    return Memory.Read<int>(13866956);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Whether the player is auto-repeating a spell.
        /// </summary>
        public bool IsAutoRepeatingSpell => AuthRepeatingSpellId != 0;

        /// <summary>
        /// Alias for AuthRepeatingSpellId for compatibility.
        /// </summary>
        public int AutoRepeatingSpellId => AuthRepeatingSpellId;

        #endregion

        #region Instance & Durability

        /// <summary>
        /// Whether the player is in an instanced zone.
        /// </summary>
        public bool IsInInstance
        {
            get
            {
                if (Memory == null)
                    return false;

                try
                {
                    WoWDb.Row? row = StyxWoW.Db?[ClientDb.Map]?.GetRow(MapId);
                    if (row == null)
                        return false;

                    uint instanceType = row.GetField<uint>(2);
                    return instanceType != 0;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Whether the player is ascending (in elevator/moving up).
        /// </summary>
        public bool IsAscending
        {
            get
            {
                if (Memory == null || BaseAddress == 0)
                    return false;

                try
                {
                    uint movementData = Memory.Read<uint>(BaseAddress + 216, 68);
                    return movementData == 2202009600;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the total durability of all equipped items.
        /// </summary>
        public uint Durability
        {
            get
            {
                uint totalDurability = 0;
                try
                {
                    // Would need Inventory system to calculate properly
                    // Placeholder for now
                }
                catch
                {
                }
                return totalDurability;
            }
        }

        /// <summary>
        /// Gets the maximum durability of all equipped items.
        /// </summary>
        public uint MaxDurability
        {
            get
            {
                uint totalMaxDurability = 0;
                try
                {
                    // Would need Inventory system to calculate properly
                    // Placeholder for now
                }
                catch
                {
                }
                return totalMaxDurability;
            }
        }

        /// <summary>
        /// Gets the durability percentage (0.0 to 1.0).
        /// </summary>
        public double DurabilityPercent
        {
            get
            {
                if (MaxDurability == 0)
                    return 0;
                return Durability / (double)MaxDurability;
            }
        }

        /// <summary>
        /// Gets the lowest durability percentage among equipped items (0.0 to 1.0).
        /// </summary>
        public double LowestDurabilityPercent
        {
            get
            {
                // Would need Inventory system
                return 100.0;
            }
        }

        /// <summary>
        /// Whether all bag slots are full.
        /// Ported from HB 3.3.5a
        /// </summary>
        public bool BagsFull
        {
            get
            {
                try
                {
                    return FreeBagSlots == 0U;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return true;
                }
            }
        }

        /// <summary>
        /// Gets the number of free bag slots (including bank bags when at bank).
        /// Ported from HB 3.3.5a
        /// </summary>
        public uint FreeBagSlots
        {
            get
            {
                try
                {
                    uint totalFreeSlots = Inventory.Backpack.FreeSlots;
                    for (uint i = 0U; i < 4U; i++)
                    {
                        WoWContainer bagAtIndex = GetBagAtIndex(i);
                        if (bagAtIndex != null)
                        {
                            totalFreeSlots += bagAtIndex.FreeSlots;
                        }
                    }
                    return totalFreeSlots;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return 0U;
                }
            }
        }

        /// <summary>
        /// Whether all normal (non-special) bag slots are full.
        /// Ported from HB 3.3.5a
        /// </summary>
        public bool NormalBagsFull
        {
            get
            {
                try
                {
                    return FreeNormalBagSlots == 0U;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return true;
                }
            }
        }

        /// <summary>
        /// Gets the number of free normal bag slots (excludes special bags like herb, mining, etc.).
        /// Ported from HB 3.3.5a
        /// </summary>
        public uint FreeNormalBagSlots
        {
            get
            {
                try
                {
                    uint totalNormalBagFreeSlots = Inventory.Backpack.FreeSlots;
                    for (uint i = 0U; i < 4U; i++)
                    {
                        WoWContainer bagAtIndex = GetBagAtIndex(i);
                        if (bagAtIndex != null)
                        {
                            ItemInfo itemInfo = bagAtIndex.ItemInfo;
                            if (itemInfo != null && itemInfo.BagFamily == 0)
                            {
                                totalNormalBagFreeSlots += bagAtIndex.FreeSlots;
                            }
                        }
                    }
                    return totalNormalBagFreeSlots;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return 0U;
                }
            }
        }

        #endregion

        #region Combat Actions

        /// <summary>
        /// Toggles attack on the current target.
        /// </summary>
        public void ToggleAttack()
        {
            try
            {
                Lua.DoString("AttackTarget()");
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
            }
        }

        /// <summary>
        /// Clears the current target.
        /// </summary>
        public void ClearTarget()
        {
            try
            {
                Lua.DoString("ClearTarget()");
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
            }
        }

        /// <summary>
        /// Targets the last target.
        /// </summary>
        public void TargetLastTarget()
        {
            try
            {
                Lua.DoString("TargetLastTarget()");
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
            }
        }

        #endregion

        #region Faction & Reactions

        // Forced reactions array - address in WoW 3.3.5a
        private const uint ForcedReactionsCountPtr = 0xC20230;  // 12713520U -> 12727440U
        private const uint ForcedReactionsArrayPtr = 0xC20234;  // 12713524U -> 12727444U
        private const uint FactionStandingBasePtr = 0xC202F0;   // 12725104U

        /// <summary>
        /// Struct for forced reaction entries.
        /// </summary>
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct ForcedReactionEntry
        {
            public uint FactionId;
            public uint Reaction;
        }

        /// <summary>
        /// Gets a forced reaction override for a faction ID.
        /// Used to determine forced reactions from reputation.
        /// </summary>
        internal bool GetForceReaction(uint factionId, out WoWUnitReaction reaction)
        {
            reaction = WoWUnitReaction.Neutral;
            if (Memory == null)
                return false;

            try
            {
                uint count = Memory.Read<uint>(ForcedReactionsCountPtr);
                if (count == 0 || count > 1000)
                    return false;

                uint arrayPtr = Memory.Read<uint>(ForcedReactionsArrayPtr);
                if (arrayPtr == 0)
                    return false;

                // Read array of forced reactions
                for (int i = 0; i < count; i++)
                {
                    uint entryPtr = arrayPtr + (uint)(i * 8);
                    uint entryFactionId = Memory.Read<uint>(entryPtr);
                    if (entryFactionId == factionId)
                    {
                        uint reactionValue = Memory.Read<uint>(entryPtr + 4);
                        reaction = (WoWUnitReaction)reactionValue;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
            }

            return false;
        }

        /// <summary>
        /// Gets faction standing for a faction.
        /// </summary>
        public bool GetFactionStanding(WoWFaction faction, out FactionStanding standing)
        {
            standing = default;
            if (faction == null || Memory == null)
                return false;

            try
            {
                int repGainId = faction.Record.RepGainId;
                if (repGainId >= 0 && repGainId <= 127)
                {
                    uint standingPtr = FactionStandingBasePtr + (uint)(repGainId * System.Runtime.InteropServices.Marshal.SizeOf<FactionStanding>());
                    standing = Memory.Read<FactionStanding>(standingPtr);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
            }

            return false;
        }

        /// <summary>
        /// Gets faction standing by faction ID.
        /// </summary>
        public bool GetFactionStanding(uint factionId, out FactionStanding standing)
        {
            WoWFaction? faction = WoWFaction.FromId(factionId);
            if (faction == null)
            {
                standing = default;
                return false;
            }
            return GetFactionStanding(faction, out standing);
        }

        #endregion

        #region Current Cursor Spell

        /// <summary>
        /// Gets the spell currently pending on the cursor (if any).
        /// Address: 11489876 (0xAF5654)
        /// Ported from HB 3.3.5a smethod_11
        /// </summary>
        public WoWSpell? CurrentCursorSpell
        {
            get
            {
                Memory? wow = ObjectManager.Wow;
                if (wow == null) return null;
                uint[] array = new uint[]
                {
                    11489876U
                };
                uint questPointer = wow.Read<uint>(array);
                if ((questPointer & 1U) == 1U || questPointer == 0U)
                {
                    questPointer = 0U;
                }
                while (questPointer != 0U && (questPointer & 1U) == 0U)
                {
                    uint[] array2 = new uint[]
                    {
                        questPointer + 32U
                    };
                    int spellId = wow.Read<int>(array2);
                    if (spellId != 0)
                    {
                        return WoWSpell.FromId(spellId);
                    }
                    uint[] array3 = new uint[]
                    {
                        questPointer + 4U
                    };
                    questPointer = wow.Read<uint>(array3);
                }
                return null;
            }
        }

        #endregion

        #region Party & Raid

        /// <summary>
        /// Gets the GUID of party member 1.
        /// </summary>
        public ulong PartyMember1GUID
        {
            get
            {
                if (Memory == null) return 0;
                try
                {
                    return Memory.Read<ulong>(12392776);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the GUID of party member 2.
        /// </summary>
        public ulong PartyMember2GUID
        {
            get
            {
                if (Memory == null) return 0;
                try
                {
                    return Memory.Read<ulong>(12392784);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the GUID of party member 3.
        /// </summary>
        public ulong PartyMember3GUID
        {
            get
            {
                if (Memory == null) return 0;
                try
                {
                    return Memory.Read<ulong>(12392792);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the GUID of party member 4.
        /// </summary>
        public ulong PartyMember4GUID
        {
            get
            {
                if (Memory == null) return 0;
                try
                {
                    return Memory.Read<ulong>(12392800);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets party member 1.
        /// </summary>
        public WoWPlayer? PartyMember1 => ObjectManager.GetObjectByGuid<WoWPlayer>(PartyMember1GUID);

        /// <summary>
        /// Gets party member 2.
        /// </summary>
        public WoWPlayer? PartyMember2 => ObjectManager.GetObjectByGuid<WoWPlayer>(PartyMember2GUID);

        /// <summary>
        /// Gets party member 3.
        /// </summary>
        public WoWPlayer? PartyMember3 => ObjectManager.GetObjectByGuid<WoWPlayer>(PartyMember3GUID);

        /// <summary>
        /// Gets party member 4.
        /// </summary>
        public WoWPlayer? PartyMember4 => ObjectManager.GetObjectByGuid<WoWPlayer>(PartyMember4GUID);

        /// <summary>
        /// Gets whether the player is in a party.
        /// </summary>
        public bool IsInParty => PartyMember1GUID != 0;

        /// <summary>
        /// Gets whether the player is in a raid.
        /// </summary>
        public bool IsInRaid => NumRaidMembers > 0;

        /// <summary>
        /// Gets the number of raid members.
        /// </summary>
        public int NumRaidMembers
        {
            get
            {
                if (Memory == null)
                    return 0;

                try
                {
                    return Memory.Read<int>(12498440);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets all raid members (HB 4.3.4 compatibility).
        /// Returns party members if not in raid.
        /// </summary>
        public List<WoWPlayer> RaidMembers
        {
            get
            {
                List<WoWPlayer> members = new List<WoWPlayer>();

                if (IsInRaid)
                {
                    // In raid - get via Lua
                    for (int i = 1; i <= NumRaidMembers; i++)
                    {
                        var guid = Lua.GetReturnVal<ulong>($"return UnitGUID('raid{i}')", 0);
                        if (guid != 0)
                        {
                            var player = ObjectManager.GetObjectByGuid<WoWPlayer>(guid);
                            if (player != null)
                                members.Add(player);
                        }
                    }
                }
                else if (IsInParty)
                {
                    // In party - use party members
                    if (PartyMember1 != null) members.Add(PartyMember1);
                    if (PartyMember2 != null) members.Add(PartyMember2);
                    if (PartyMember3 != null) members.Add(PartyMember3);
                    if (PartyMember4 != null) members.Add(PartyMember4);
                }

                return members;
            }
        }

        #endregion

        #region Helper Methods & Actions

        /// <summary>
        /// Checks if the specified unit is behind the player.
        /// </summary>
        public bool IsBehind(WoWUnit unit)
        {
            if (unit == null)
                return false;

            return WoWMathHelper.IsBehind(Location, unit.Location, unit.Rotation);
        }

        #endregion

        #region Inventory & Bags

        /// <summary>
        /// Gets the player's inventory structure.
        /// </summary>
        public WoWPlayerInventory Inventory
        {
            get
            {
                try
                {
                    Memory? wow = ObjectManager.Wow;
                    if (wow == null) return null!;
                    uint[] array = new uint[]
                    {
                        BaseAddress + 6384U
                    };
                    return new WoWPlayerInventory(wow.Read<BagStructure>(array));
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return null!;
                }
            }
        }

        /// <summary>
        /// Gets the GUID of the bag at the specified index (0-10).
        /// </summary>
        public ulong GetBagGuidAtIndex(uint index)
        {
            ulong result;
            try
            {
                if (index > 10U)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                if (index >= 4U)
                {
                    Memory? wow = ObjectManager.Wow;
                    if (wow == null) return 0UL;
                    uint[] array = new uint[]
                    {
                        12499360U
                    };
                    if (wow.Read<ulong>(array) == 0UL)
                    {
                        return 0UL;
                    }
                }
                Memory? wow2 = ObjectManager.Wow;
                if (wow2 == null) return 0UL;
                uint[] array2 = new uint[]
                {
                    12727616U + 8U * index
                };
                ulong raidMemberGuid = wow2.Read<ulong>(array2);
                result = raidMemberGuid;
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Gets the container at the specified bag index.
        /// </summary>
        public WoWContainer GetBagAtIndex(uint index)
        {
            try
            {
                if (index > 10U)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return ObjectManager.GetObjectByGuid<WoWContainer>(GetBagGuidAtIndex(index))!;
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
                return null!;
            }
        }

        /// <summary>
        /// Gets the container at the specified bag slot.
        /// </summary>
        public WoWContainer GetBag(WoWBagSlot slot)
        {
            try
            {
                return GetBagAtIndex((uint)slot);
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
                return null!;
            }
        }

        /// <summary>
        /// Gets the backpack and bag contents (GUIDs only).
        /// </summary>
        public List<ulong> BagItemGuids
        {
            get
            {
                try
                {
                    List<ulong> list = new List<ulong>();
                    
                    // Add backpack items
                    if (Inventory != null && Inventory.Backpack != null)
                    {
                        list.AddRange(Inventory.Backpack.PhysicalItemGuids);
                    }

                    // Add items from 4 bag slots
                    for (uint i = 0U; i < 4U; i++)
                    {
                        WoWContainer bag = GetBagAtIndex(i);
                        if (bag != null)
                        {
                            list.AddRange(bag.PhysicalItemGuids);
                        }
                    }

                    return list;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return new List<ulong>();
                }
            }
        }

        /// <summary>
        /// Gets all carried items (bags + currency + keyring + equipped).
        /// </summary>
        public List<ulong> CarriedItemGuids
        {
            get
            {
                try
                {
                    List<ulong> list = BagItemGuids;

                    if (Inventory != null)
                    {
                        if (Inventory.Currency != null)
                            list.AddRange(Inventory.Currency.PhysicalItemGuids);

                        if (Inventory.Keyring != null)
                            list.AddRange(Inventory.Keyring.PhysicalItemGuids);

                        if (Inventory.Equipped != null)
                            list.AddRange(Inventory.Equipped.PhysicalItemGuids);
                    }

                    return list;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return new List<ulong>();
                }
            }
        }

        /// <summary>
        /// Gets all items in bags and equipped slots.
        /// </summary>
        public List<WoWItem> BagItems
        {
            get
            {
                try
                {
                    List<WoWItem> items = new List<WoWItem>();
                    foreach (ulong guid in BagItemGuids)
                    {
                        if (guid == 0)
                            continue;

                        WoWItem? item = ObjectManager.GetObjectByGuid<WoWItem>(guid);
                        if (item != null && item.IsValid)
                        {
                            items.Add(item);
                        }
                    }
                    return items;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return new List<WoWItem>();
                }
            }
        }

        /// <summary>
        /// Gets all carried items (bags + currency + keyring + equipped).
        /// </summary>
        public List<WoWItem> CarriedItems
        {
            get
            {
                try
                {
                    List<WoWItem> items = new List<WoWItem>();
                    foreach (ulong guid in CarriedItemGuids)
                    {
                        if (guid == 0)
                            continue;

                        WoWItem? item = ObjectManager.GetObjectByGuid<WoWItem>(guid);
                        if (item != null && item.IsValid)
                        {
                            items.Add(item);
                        }
                    }
                    return items;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return new List<WoWItem>();
                }
            }
        }

        #endregion

        #region Equipment & Item Checking

        /// <summary>
        /// Checks if the player can equip the specified item.
        /// </summary>
        public bool CanEquipItem(WoWItem item)
        {
            try
            {
                return CanEquipItem(item.ItemInfo);
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// Checks if the player can equip an item with the specified info.
        /// </summary>
        public bool CanEquipItem(ItemInfo itemInfo)
        {
            try
            {
                GameError reason;
                return CanEquipItem(itemInfo, out reason);
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// Checks if the player can equip an item (with reason output).
        /// Calls native CGPlayer_C::CanUseItem at 0x6DC3F0 (7193584)
        /// </summary>
        public bool CanEquipItem(ItemInfo itemInfo, out GameError reason)
        {
            try
            {
                if (itemInfo.InternalInfo.EquipSlot == 0)
                {
                    reason = GameError.InvFull;
                    return false;
                }
                else
                {
                    return CanUseItemInternal(this, itemInfo.BaseAddress, out reason);
                }
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
                reason = GameError.InvFull;
                throw;
            }
        }

        /// <summary>
        /// Checks if the player can equip the specified item (with reason output).
        /// </summary>
        public bool CanEquipItem(WoWItem item, out GameError reason)
        {
            try
            {
                return CanEquipItem(item.ItemInfo, out reason);
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
                reason = GameError.InvFull;
                throw;
            }
        }

        /// <summary>
        /// Internal native call to CGPlayer_C::CanUseItem
        /// Address: 0x6DC3F0 (7193584)
        /// </summary>
        private static bool CanUseItemInternal(LocalPlayer player, uint itemCacheEntryBaseAddress, out GameError reason)
        {
            try
            {
                if (player == null)
                {
                    throw new ArgumentNullException("player");
                }

                ExecutorRand? executor = ObjectManager.Executor;
                if (executor == null)
                {
                    throw new Exception("Invalid executor used in CGPlayer_C::CanUseItem");
                }

                bool canUseItem;
                lock (executor.AssemblyLock)
                {
                    executor.Clear();
                    uint allocatedMemoryAddress = executor.Memory.AllocateMemory(4);
                    if (allocatedMemoryAddress == 0U)
                    {
                        throw new Exception("Couldn't allocate memory for CGPlayer_C::CanUseItem");
                    }
                    try
                    {
                        executor.AddLine("push {0}", allocatedMemoryAddress);
                        executor.AddLine("push {0}", itemCacheEntryBaseAddress);
                        executor.AddLine("mov ecx, {0}", player.BaseAddress);
                        executor.AddLine("call {0}", 7193584U);  // CGPlayer_C::CanUseItem
                        executor.AddLine("retn");
                        executor.Execute();
                        
                        reason = (GameError)executor.Memory.Read<uint>(new uint[] { allocatedMemoryAddress });
                        canUseItem = Convert.ToBoolean(executor.Memory.Read<uint>(new uint[] { executor.ReturnPointer }));
                    }
                    finally
                    {
                        executor.Memory.FreeMemory(allocatedMemoryAddress);
                    }
                }
                return canUseItem;
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
                reason = GameError.InvFull;
                throw;
            }
        }

        #endregion

        #region Mirror Timers

        /// <summary>
        /// Gets the mirror timer info for the specified type (Breath, Fatigue, FeignDeath).
        /// Address: 12389248 (0xBD0380)
        /// Ported from HB 3.3.5a
        /// </summary>
        public MirrorTimerInfo GetMirrorTimerInfo(MirrorTimerType type)
        {
            Memory? wow = ObjectManager.Wow;
            if (wow == null) return default;
            uint[] array = new uint[]
            {
                (uint)(12389248 + (int)type * FastSize<MirrorTimerInfo>.Size)
            };
            return wow.Read<MirrorTimerInfo>(array);
        }

        #endregion

        #region Skills

        /// <summary>
        /// Gets all skills in a dictionary (ID -> WoWSkill).
        /// </summary>
        public Dictionary<int, WoWSkill> AllSkills
        {
            get
            {
                try
                {
                    Dictionary<int, WoWSkill> dictionary = new Dictionary<int, WoWSkill>();
                    
                    // Iterate through 128 skill slots
                    for (uint i = 0U; i < 128U; i++)
                    {
                        WoWSkill skill = GetSkillByIndex(i);
                        if (skill != null && skill.IsValid && !dictionary.ContainsKey(skill.Id))
                        {
                            dictionary.Add(skill.Id, skill);
                        }
                    }
                    
                    return dictionary;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return new Dictionary<int, WoWSkill>();
                }
            }
        }

        /// <summary>
        /// Helper to get skill at index (internal use).
        /// Ported from HB 3.3.5a method_23
        /// </summary>
        private WoWSkill GetSkillByIndex(uint index)
        {
            try
            {
                // HB 3.3.5a uses offset 2544 (0x9F0) from player descriptor base
                uint skillsOffset = 2544U;
                
                Memory? wow = ObjectManager.Wow;
                if (wow == null) return null!;
                uint[] array = new uint[]
                {
                    BaseAddress + 8U
                };
                uint descriptorBase = wow.Read<uint>(array);
                
                if (descriptorBase != 0U && skillsOffset != 0U)
                {
                    // Each skill entry is 12 bytes
                    return new WoWSkill(descriptorBase + skillsOffset + index * 12U);
                }
                else
                {
                    return null!;
                }
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
                return null!;
            }
        }

        /// <summary>
        /// Gets skill by name.
        /// Ported from HB 3.3.5a
        /// </summary>
        public WoWSkill? GetSkill(string name)
        {
            var keyValuePair = AllSkills.Where(kvp => kvp.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            return keyValuePair.Value;
        }

        #endregion

        #region States & Flags - Additional

        /// <summary>
        /// Gets whether the player is stealthed.
        /// </summary>
        public new bool IsStealthed
        {
            get
            {
                try
                {
                    if (Memory == null)
                        return false;

                    // Stealth state at BaseAddress + 208, offset 270
                    uint stealthByte = Memory.Read<uint>(BaseAddress + 208U, 270U);
                    return stealthByte == 2U;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the player's current shapeshift form (stance).
        /// </summary>
        public ShapeshiftForm Stance
        {
            get
            {
                try
                {
                    var bytes2 = Bytes2;
                    return (ShapeshiftForm)bytes2[3];
                }
                catch
                {
                    return ShapeshiftForm.Normal;
                }
            }
        }

        #endregion

        #region Pending Cursor Spell

        // Offsets for pending cursor spell (3.3.5a)
        // SpellTargetMode - indicates if we're in targeting mode: 0x00CEC1CC
        // SpellTargetSpellId - the spell ID being targeted: 0x00CEC1D0
        private const uint SpellTargetModePtr = 0x00CEC1CC;
        private const uint SpellTargetSpellIdPtr = 0x00CEC1D0;

        /// <summary>
        /// Gets the spell currently awaiting target selection (null if none).
        /// Ported from HB 4.3.4.
        /// </summary>
        public WoWSpell? CurrentPendingCursorSpell
        {
            get
            {
                if (Memory == null)
                    return null;
                if (Memory.Read<uint>(SpellTargetModePtr) == 0U)
                    return null;
                int spellId = Memory.Read<int>(SpellTargetSpellIdPtr);
                if (spellId <= 0)
                    return null;
                return WoWSpell.FromId(spellId);
            }
        }

        /// <summary>
        /// Checks if a pending cursor spell matches the given spell ID.
        /// </summary>
        public bool HasPendingSpell(int spellId)
        {
            WoWSpell? pendingCursorSpell = CurrentPendingCursorSpell;
            return pendingCursorSpell != null && pendingCursorSpell.Id == spellId;
        }

        /// <summary>
        /// Checks if a pending cursor spell matches the given spell name.
        /// </summary>
        public bool HasPendingSpell(string name)
        {
            WoWSpell? pendingCursorSpell = CurrentPendingCursorSpell;
            return pendingCursorSpell != null && pendingCursorSpell.Name == name;
        }

        /// <summary>
        /// Checks if a pending cursor spell matches the given WoWSpell.
        /// </summary>
        public bool HasPendingSpell(WoWSpell? spell)
        {
            WoWSpell? pendingCursorSpell = CurrentPendingCursorSpell;
            return spell != null && pendingCursorSpell != null && pendingCursorSpell.Id == spell.Id;
        }

        #endregion

        #region Group Info

        private WoWGroupInfo _groupInfo;

        /// <summary>
        /// Gets the group information for this player.
        /// </summary>
        public WoWGroupInfo GroupInfo => _groupInfo ?? (_groupInfo = WoWGroupInfo.Instance);

        #endregion

        #region Totems (Shaman)

        private WoWTotemInfo[] _totems;

        /// <summary>
        /// Gets the active totems for this player (Shaman only).
        /// Index 0=Fire, 1=Earth, 2=Water, 3=Air
        /// </summary>
        public WoWTotemInfo[] Totems
        {
            get
            {
                if (_totems == null)
                {
                    _totems = new WoWTotemInfo[4];
                    for (int i = 0; i < 4; i++)
                        _totems[i] = new WoWTotemInfo(i);
                }
                return _totems;
            }
        }

        #endregion

        #region CC State Properties

        /// <summary>
        /// Returns true if the player is stunned.
        /// </summary>
        public bool IsStunned
        {
            get
            {
                // Check for common stun auras or use the stun mechanic
                var result = Lua.GetReturnVal<int>(
                    "for i=1,40 do local _,_,_,_,_,_,_,_,_,_,id = UnitDebuff('player',i); " +
                    "if id then local mechanic = select(2, GetSpellInfo(id)); " +
                    "if mechanic == 'stun' or mechanic == 'STUN' then return 1 end end end return 0", 0);
                return result == 1;
            }
        }

        /// <summary>
        /// Returns true if the player is rooted.
        /// </summary>
        public bool IsRooted
        {
            get
            {
                var result = Lua.GetReturnVal<int>(
                    "for i=1,40 do local _,_,_,_,_,_,_,_,_,_,id = UnitDebuff('player',i); " +
                    "if id then local mechanic = select(2, GetSpellInfo(id)); " +
                    "if mechanic == 'root' or mechanic == 'ROOT' then return 1 end end end return 0", 0);
                return result == 1;
            }
        }

        /// <summary>
        /// Returns true if the player is snared (slowed).
        /// </summary>
        public bool Snared
        {
            get
            {
                // Check for common snare auras
                var result = Lua.GetReturnVal<int>(
                    "for i=1,40 do local _,_,_,_,_,_,_,_,_,_,id = UnitDebuff('player',i); " +
                    "if id then local mechanic = select(2, GetSpellInfo(id)); " +
                    "if mechanic == 'snare' or mechanic == 'SNARE' then return 1 end end end return 0", 0);
                return result == 1;
            }
        }

        #endregion

        #region Mana & Power Properties

        /// <summary>
        /// Gets the mana regeneration rate per 5 seconds.
        /// </summary>
        public float ManaRegenRate
        {
            get
            {
                // GetManaRegen returns base, casting regen
                var regen = Lua.GetReturnVal<float>("local base, casting = GetManaRegen(); return base * 5", 0);
                return regen;
            }
        }

        /// <summary>
        /// Gets the power percentage (mana/rage/energy/etc).
        /// </summary>
        public double PowerPercent
        {
            get
            {
                var max = MaxPower;
                if (max <= 0)
                    return 0;
                return (CurrentPower * 100.0) / max;
            }
        }

        #endregion

        /// <summary>
        /// Gets the player's quest log.
        /// </summary>
        public readonly QuestLog QuestLog = new QuestLog();
    }
}