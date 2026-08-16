#nullable disable
using System;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Pathing;

namespace Styx.WoWInternals.WoWObjects
{
    /// <summary>
    /// Represents a party/raid member.
    /// WotLK 3.3.5a implementation using Lua calls.
    /// </summary>
    public class WoWPartyMember : IEquatable<WoWPartyMember>
    {
        #region Fields

        private readonly ulong _guid;
        private readonly string _unitId;
        private readonly bool _isRaidMember;
        private readonly int _index;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a party member from a unit ID.
        /// </summary>
        public WoWPartyMember(string unitId, int index, bool isRaidMember)
        {
            _unitId = unitId;
            _index = index;
            _isRaidMember = isRaidMember;
            _guid = GetGuidFromUnitId(unitId);
        }

        /// <summary>
        /// Creates a party member from a GUID.
        /// </summary>
        public WoWPartyMember(ulong guid, bool isRaidMember)
        {
            _guid = guid;
            _isRaidMember = isRaidMember;
            _unitId = isRaidMember ? "raid" : "party";
            _index = 0;
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates a WoWPartyMember from a party index (1-4).
        /// </summary>
        public static WoWPartyMember FromPartyIndex(int index)
        {
            if (index < 1 || index > 4)
                return null;

            string unitId = "party" + index;
            var exists = Lua.GetReturnVal<int>($"return UnitExists('{unitId}') and 1 or 0", 0);
            if (exists == 0)
                return null;

            return new WoWPartyMember(unitId, index, false);
        }

        /// <summary>
        /// Creates a WoWPartyMember from a raid index (1-40).
        /// </summary>
        public static WoWPartyMember FromRaidIndex(int index)
        {
            if (index < 1 || index > 40)
                return null;

            string unitId = "raid" + index;
            var exists = Lua.GetReturnVal<int>($"return UnitExists('{unitId}') and 1 or 0", 0);
            if (exists == 0)
                return null;

            return new WoWPartyMember(unitId, index, true);
        }

        #endregion

        #region Core Properties

        /// <summary>
        /// Gets the GUID of this party member.
        /// </summary>
        public ulong Guid => _guid;

        /// <summary>
        /// Gets the name of this party member.
        /// </summary>
        public string Name => Lua.GetReturnVal<string>($"return UnitName('{_unitId}')", 0) ?? "Unknown";

        /// <summary>
        /// Gets the unit ID used for Lua calls.
        /// </summary>
        public string UnitId => _unitId;

        #endregion

        #region Status Flags

        /// <summary>
        /// Returns true if this member is online.
        /// </summary>
        public bool IsOnline
        {
            get
            {
                var result = Lua.GetReturnVal<int>($"return UnitIsConnected('{_unitId}') and 1 or 0", 0);
                return result == 1;
            }
        }

        /// <summary>
        /// Returns true if this member is dead.
        /// </summary>
        public bool Dead
        {
            get
            {
                var result = Lua.GetReturnVal<int>($"return UnitIsDead('{_unitId}') and 1 or 0", 0);
                return result == 1;
            }
        }

        /// <summary>
        /// Returns true if this member is a ghost.
        /// </summary>
        public bool Ghost
        {
            get
            {
                var result = Lua.GetReturnVal<int>($"return UnitIsGhost('{_unitId}') and 1 or 0", 0);
                return result == 1;
            }
        }

        /// <summary>
        /// Returns true if this member is the group leader.
        /// </summary>
        public bool GroupLeader
        {
            get
            {
                var result = Lua.GetReturnVal<int>($"return UnitIsGroupLeader('{_unitId}') and 1 or 0", 0);
                return result == 1;
            }
        }

        /// <summary>
        /// Returns true if this member is flagged for PvP.
        /// </summary>
        public bool PvpFlagged
        {
            get
            {
                var result = Lua.GetReturnVal<int>($"return UnitIsPVP('{_unitId}') and 1 or 0", 0);
                return result == 1;
            }
        }

        /// <summary>
        /// Returns true if this member is AFK.
        /// </summary>
        public bool AFKFlagged
        {
            get
            {
                var result = Lua.GetReturnVal<int>($"return UnitIsAFK('{_unitId}') and 1 or 0", 0);
                return result == 1;
            }
        }

        /// <summary>
        /// Returns true if this member is DND.
        /// </summary>
        public bool DNDFlagged
        {
            get
            {
                var result = Lua.GetReturnVal<int>($"return UnitIsDND('{_unitId}') and 1 or 0", 0);
                return result == 1;
            }
        }

        /// <summary>
        /// Returns true if this member is flagged for Free-For-All PvP.
        /// Ported from HB 4.3.4 WoWPartyMember.FFAPvpFlagged (StatusFlags & 16).
        /// </summary>
        public bool FFAPvpFlagged
        {
            get
            {
                var result = Lua.GetReturnVal<int>($"return UnitIsPVPFreeForAll('{_unitId}') and 1 or 0", 0);
                return result == 1;
            }
        }

        #endregion

        #region Raid Role Flags

        /// <summary>
        /// Returns true if this member is the main tank.
        /// </summary>
        public bool IsMainTank
        {
            get
            {
                if (!_isRaidMember)
                    return false;
                var result = Lua.GetReturnVal<string>($"local name, rank, subgroup, level, class, fileName, zone, online, isDead, role, isML = GetRaidRosterInfo({_index}); return role or ''", 0);
                return result == "MAINTANK";
            }
        }

        /// <summary>
        /// Returns true if this member is the main assist.
        /// </summary>
        public bool IsMainAssist
        {
            get
            {
                if (!_isRaidMember)
                    return false;
                var result = Lua.GetReturnVal<string>($"local name, rank, subgroup, level, class, fileName, zone, online, isDead, role, isML = GetRaidRosterInfo({_index}); return role or ''", 0);
                return result == "MAINASSIST";
            }
        }

        #endregion

        #region Health & Power

        /// <summary>
        /// Gets the current health.
        /// </summary>
        public uint Health => (uint)Lua.GetReturnVal<int>($"return UnitHealth('{_unitId}')", 0);

        /// <summary>
        /// Gets the maximum health.
        /// </summary>
        public uint HealthMax => (uint)Lua.GetReturnVal<int>($"return UnitHealthMax('{_unitId}')", 0);

        /// <summary>
        /// Gets the health percentage.
        /// </summary>
        public double HealthPercent
        {
            get
            {
                var max = HealthMax;
                return max > 0 ? (Health * 100.0 / max) : 0;
            }
        }

        /// <summary>
        /// Gets the current power (mana/rage/energy).
        /// </summary>
        public uint Power => (uint)Lua.GetReturnVal<int>($"return UnitMana('{_unitId}')", 0);

        /// <summary>
        /// Gets the maximum power.
        /// </summary>
        public uint PowerMax => (uint)Lua.GetReturnVal<int>($"return UnitManaMax('{_unitId}')", 0);

        /// <summary>
        /// Gets the power type.
        /// </summary>
        public WoWPowerType PowerType => (WoWPowerType)Lua.GetReturnVal<int>($"return UnitPowerType('{_unitId}')", 0);

        #endregion

        #region Level & Class

        /// <summary>
        /// Gets the level.
        /// </summary>
        public int Level => Lua.GetReturnVal<int>($"return UnitLevel('{_unitId}')", 0);

        /// <summary>
        /// Gets the class.
        /// </summary>
        public WoWClass Class
        {
            get
            {
                var classFile = Lua.GetReturnVal<string>($"local _, classFile = UnitClass('{_unitId}'); return classFile or ''", 0);
                return ParseClass(classFile);
            }
        }

        private WoWClass ParseClass(string classFile)
        {
            switch (classFile?.ToUpperInvariant())
            {
                case "WARRIOR": return WoWClass.Warrior;
                case "PALADIN": return WoWClass.Paladin;
                case "HUNTER": return WoWClass.Hunter;
                case "ROGUE": return WoWClass.Rogue;
                case "PRIEST": return WoWClass.Priest;
                case "DEATHKNIGHT": return WoWClass.DeathKnight;
                case "SHAMAN": return WoWClass.Shaman;
                case "MAGE": return WoWClass.Mage;
                case "WARLOCK": return WoWClass.Warlock;
                case "DRUID": return WoWClass.Druid;
                default: return WoWClass.None;
            }
        }

        #endregion

        #region Location

        /// <summary>
        /// Gets the member's location.
        /// </summary>
        /// <summary>
        /// HB 4.3.4 compat — Location3D returned WoWPoint while Location returned Vector2.
        /// In CopilotBuddy, Location already returns WoWPoint, so this is an alias.
        /// External bots (LazyRaider etc.) reference this.
        /// </summary>
        public WoWPoint Location3D => Location;

        public WoWPoint Location
        {
            get
            {
                // Party/raid members location is limited - we can get it if they're in range
                var player = ToPlayer();
                if (player != null)
                    return player.Location;

                // Otherwise use map position (less accurate)
                var x = Lua.GetReturnVal<float>($"local x,y = GetPlayerMapPosition('{_unitId}'); return x or 0", 0);
                var y = Lua.GetReturnVal<float>($"local x,y = GetPlayerMapPosition('{_unitId}'); return y or 0", 0);
                
                // Map coords are 0-1 range, need to convert to world coords
                // This is approximate and zone-dependent
                return new WoWPoint(x * 100, y * 100, 0);
            }
        }

        /// <summary>
        /// Gets the zone area ID.
        /// </summary>
        public int AreaTableId
        {
            get
            {
                // WotLK doesn't expose this directly for party members
                // Return the player's zone if same zone, otherwise 0
                return 0;
            }
        }

        /// <summary>
        /// Gets the continent ID for this party member.
        /// Ported from HB 4.3.4 WoWPartyMember.ContinentId (memory struct field).
        /// WotLK Lua has no per-unit continent API; returns 0 as a safe stub.
        /// </summary>
        public ushort ContinentId => 0;

        #endregion

        #region Role

        /// <summary>
        /// Gets the assigned role in LFG (WotLK 3.3.5a: UnitGroupRolesAssigned available since patch 3.3.0).
        /// Falls back to manual MT/MA assignment.
        /// </summary>
        public GroupRole Role
        {
            get
            {
                // UnitGroupRolesAssigned returns "TANK", "HEALER", "DAMAGER", or "NONE"
                var lfgRole = Lua.GetReturnVal<string>($"return UnitGroupRolesAssigned('{_unitId}')", 0);
                switch (lfgRole)
                {
                    case "TANK":    return GroupRole.Tank;
                    case "HEALER":  return GroupRole.Healer;
                    case "DAMAGER": return GroupRole.Damage;
                }

                // Fall back to manual raid assignment (MAINTANK via right-click menu)
                if (IsMainTank)
                    return GroupRole.Tank;

                return GroupRole.None;
            }
        }

        /// <summary>
        /// Returns true if the member has the specified role.
        /// </summary>
        public bool HasRole(GroupRole role)
        {
            return (Role & role) != GroupRole.None;
        }

        #endregion

        #region Raid Info

        /// <summary>
        /// Gets the raid subgroup number (1-8).
        /// </summary>
        public uint GroupNumber
        {
            get
            {
                if (!_isRaidMember)
                    return 0;
                return (uint)Lua.GetReturnVal<int>($"local _, _, subgroup = GetRaidRosterInfo({_index}); return subgroup or 0", 0);
            }
        }

        /// <summary>
        /// Gets the raid rank (0=member, 1=assist, 2=leader).
        /// </summary>
        public int RaidRank
        {
            get
            {
                if (!_isRaidMember)
                    return GroupLeader ? 2 : 0;
                return Lua.GetReturnVal<int>($"local _, rank = GetRaidRosterInfo({_index}); return rank or 0", 0);
            }
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Converts this party member to a WoWPlayer if in range.
        /// </summary>
        public WoWPlayer ToPlayer()
        {
            if (_guid == 0)
                return null;
            return ObjectManager.GetObjectByGuid<WoWPlayer>(_guid);
        }

        #endregion

        #region Helper Methods

        private static ulong GetGuidFromUnitId(string unitId)
        {
            // WotLK GUID format from UnitGUID
            var guidStr = Lua.GetReturnVal<string>($"return UnitGUID('{unitId}') or '0x0'", 0);
            if (string.IsNullOrEmpty(guidStr) || guidStr == "0x0")
                return 0;

            // Parse the GUID (format: 0x0600000000123456)
            try
            {
                if (guidStr.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    guidStr = guidStr.Substring(2);
                return ulong.Parse(guidStr, System.Globalization.NumberStyles.HexNumber);
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        #region Equality

        public bool Equals(WoWPartyMember other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return _guid == other._guid;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is WoWPartyMember member && Equals(member);
        }

        public override int GetHashCode() => _guid.GetHashCode();

        public static bool operator ==(WoWPartyMember left, WoWPartyMember right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WoWPartyMember left, WoWPartyMember right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return $"[WoWPartyMember] {Name} (0x{Guid:X}) - HP:{Health}/{HealthMax} Level:{Level} Online:{IsOnline}";
        }

        #endregion

        #region Enums

        /// <summary>
        /// Group role flags.
        /// </summary>
        [Flags]
        public enum GroupRole
        {
            None = 0,
            Leader = 1,
            Tank = 2,
            Healer = 4,
            Damage = 8,
        }

        #endregion
    }
}
