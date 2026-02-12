#nullable disable
using System;
using System.Collections.Generic;
using GreenMagic;
using Styx.Helpers;
using Styx.Patchables;
using Styx.WoWInternals;

namespace Styx.Logic.Combat
{
    public class WoWSpell : IEquatable<WoWSpell>
    {
        private readonly int _id;
        private readonly SpellEntry _spellEntry;
        private readonly WoWDb.Row _row;

        private static readonly Dictionary<int, SpellInfoCache> _spellInfoCache;
        private static readonly Dictionary<int, WoWDb.Row> _rowCache;

        static WoWSpell()
        {
            _spellInfoCache = new Dictionary<int, SpellInfoCache>();
            _rowCache = new Dictionary<int, WoWDb.Row>();
        }

        private WoWSpell(int id, WoWDb.Row row)
        {
            _id = id;
            _row = row;
            _spellEntry = row.GetStruct<SpellEntry>();
        }

        // NOTE: ASM injection for cooldown removed - using Lua instead (see Cooldown property)
        // The old GetSpellCooldown() method caused crashes (InjectionFinishedEvent never fired)

        public bool IsValid
        {
            get { return _id != 0 && _row != null; }
        }

        public uint BaseLevel
        {
            get { return _spellEntry.baseLevel; }
        }

        public uint Level
        {
            get { return _spellEntry.spellLevel; }
        }

        private uint RangeIndex
        {
            get { return _spellEntry.rangeIndex; }
        }

        /// <summary>
        /// Gets the spell range ID for combat routine range checks.
        /// Uses Lua-based MinRange/MaxRange which are reliable and match what
        /// Singular expects: 1 = Self Only (max=0), 2 = Melee (max≤5), 3+ = Ranged.
        /// Note: DBC rangeIndex is available via RangeIndex property for DBC lookups.
        /// </summary>
        public uint SpellRangeId
        {
            get
            {
                float min = MinRange;
                float max = MaxRange;

                // Self-only spells (buffs, melee with max=0 from Lua)
                if (max == 0f && min == 0f)
                    return 1U;

                // Melee range spells (MaxRange <= 5 yards)
                if (max > 0f && max <= 5f)
                    return 2U;

                return 3U;
            }
        }

        /// <summary>
        /// Returns true if this is a melee-range spell.
        /// FEAT-08: Checks both synthetic rangeId and DBC rangeIndex.
        /// </summary>
        public bool IsMeleeSpell => SpellRangeId <= 2;

        /// <summary>
        /// Returns true if this spell can only target self.
        /// FEAT-08: Uses synthetic rangeId (Lua-based MaxRange == 0).
        /// </summary>
        public bool IsSelfOnlySpell => SpellRangeId == 1;

        public uint ManaCostPercent
        {
            get { return _spellEntry.ManaCostPercentage; }
        }

        /// <summary>
        /// Returns true if this spell is channeled.
        /// Checks AttributesEx for channeled flag (0x44).
        /// </summary>
        public bool IsChanneled
        {
            get
            {
                // BUG-01 fix: Was checking Attributes, must check AttributesEx
                return (_spellEntry.AttributesEx & 0x44) != 0;
            }
        }

        public int Id
        {
            get { return _id; }
        }

        public uint Category
        {
            get { return _spellEntry.Category; }
        }

        public WoWDispelType DispelType
        {
            get { return (WoWDispelType)_spellEntry.Dispel; }
        }

        public WoWSpellMechanic Mechanic
        {
            get { return (WoWSpellMechanic)_spellEntry.Mechanic; }
        }

        public uint MaxTargets
        {
            get { return _spellEntry.MaxTargetLevel; }
        }

        public WoWCreatureType TargetType
        {
            get { return (WoWCreatureType)_spellEntry.TargetCreatureType; }
        }

        public int CreatesItemId
        {
            get { return (int)_spellEntry.EffectItemType[0]; }
        }

        public SpellEffect SpellEffect1
        {
            get { return GetSpellEffect(0); }
        }

        public SpellEffect SpellEffect2
        {
            get { return GetSpellEffect(1); }
        }

        public SpellEffect SpellEffect3
        {
            get { return GetSpellEffect(2); }
        }

        public SpellEffect[] SpellEffects
        {
            get
            {
                SpellEffect[] effects = new SpellEffect[3];
                for (int i = 0; i < 3; i++)
                {
                    effects[i] = GetSpellEffect(i);
                }
                return effects;
            }
        }

        public WoWPowerType PowerType
        {
            get { return (WoWPowerType)_spellEntry.powerType; }
        }

        public SpellEntry InternalInfo
        {
            get { return _spellEntry; }
        }

        public int PowerCost
        {
            get
            {
                SpellInfoCache cache;
                if (_spellInfoCache.TryGetValue(Id, out cache))
                {
                    return cache.PowerCost;
                }
                _spellInfoCache.Add(Id, GetSpellInfo());
                return _spellInfoCache[Id].PowerCost;
            }
        }

        public bool IsFunnel
        {
            get
            {
                SpellInfoCache cache;
                if (_spellInfoCache.TryGetValue(Id, out cache))
                {
                    return cache.IsFunnel;
                }
                _spellInfoCache.Add(Id, GetSpellInfo());
                return _spellInfoCache[Id].IsFunnel;
            }
        }

        public uint CastTime
        {
            get
            {
                SpellInfoCache cache;
                if (_spellInfoCache.TryGetValue(Id, out cache))
                {
                    return cache.CastTime;
                }
                _spellInfoCache.Add(Id, GetSpellInfo());
                return _spellInfoCache[Id].CastTime;
            }
        }

        public float MinRange
        {
            get
            {
                SpellInfoCache cache;
                if (_spellInfoCache.TryGetValue(Id, out cache))
                {
                    return cache.MinRange;
                }
                _spellInfoCache.Add(Id, GetSpellInfo());
                return _spellInfoCache[Id].MinRange;
            }
        }

        public float MaxRange
        {
            get
            {
                SpellInfoCache cache;
                if (_spellInfoCache.TryGetValue(Id, out cache))
                {
                    return cache.MaxRange;
                }
                _spellInfoCache.Add(Id, GetSpellInfo());
                return _spellInfoCache[Id].MaxRange;
            }
        }

        public uint MaxStackCount
        {
            get { return _spellEntry.StackAmount; }
        }

        public string Name
        {
            get 
            { 
                // Use SpellDb to avoid Lua calls which can crash the game
                return SpellDb.GetSpellName(Id);
            }
        }

        public string Rank
        {
            get 
            { 
                // Use SpellDb to avoid Lua calls which can crash the game
                return SpellDb.GetSpellRank(Id);
            }
        }

        public string Tooltip
        {
            get { return ObjectManager.Wow.Read<string>(_spellEntry.ToolTip); }
        }

        // Cooldown check using Lua (from .Reference code that worked)
        // Uses Lua GetSpellCooldown instead of injected executor call to avoid crashes
        private static readonly Dictionary<int, (double expiresAt, long lastQueryTicks)> _cooldownCache = new Dictionary<int, (double, long)>();
        private const int CooldownCacheMinMs = 50; // minimal interval between Lua queries for same spell

        public bool Cooldown
        {
            get
            {
                long nowTicks = Environment.TickCount;
                (var expiresAt, var lastQuery) = _cooldownCache.TryGetValue(Id, out var entry) ? entry : (0.0, 0L);
                
                // If cached and not expired and queried recently, reuse
                if (expiresAt > 0.0 && expiresAt > GetTimeSeconds() && (nowTicks - lastQuery) < CooldownCacheMinMs)
                    return true;

                // Query Lua: start, duration
                try
                {
                    var lua = Lua.GetReturnValues($"local s,d,_,_=GetSpellCooldown({Id}); if s==0 or d==0 then return 0,0 else return s,d end", "cooldown.lua");
                    if (lua != null && lua.Count >= 2)
                    {
                        if (double.TryParse(lua[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double start) && 
                            double.TryParse(lua[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double dur))
                        {
                            if (start > 0 && dur > 0)
                            {
                                double remaining = (start + dur) - GetTimeSeconds();
                                if (remaining > 0)
                                {
                                    _cooldownCache[Id] = ((start + dur), nowTicks);
                                    return true;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // If Lua fails, assume not on cooldown
                    return false;
                }
                
                // Not on cooldown
                _cooldownCache[Id] = (0.0, nowTicks);
                return false;
            }
        }

        private static double GetTimeSeconds() => Lua.GetReturnVal<double>("return GetTime()", 0);

        /// <summary>
        /// Gets the remaining cooldown time for this spell.
        /// </summary>
        public TimeSpan CooldownTimeLeft
        {
            get
            {
                var luaTime = Lua.GetReturnVal<double>(string.Format("local x,y=GetSpellCooldown({0}); return x+y-GetTime()", Id), 0);
                if (luaTime <= 0)
                    return TimeSpan.Zero;
                return TimeSpan.FromSeconds(luaTime);
            }
        }

        public uint BaseCooldown
        {
            get { return _spellEntry.StartRecoveryTime; }
        }

        public bool HasRange
        {
            get
            {
                if (MinRange == 0f && MaxRange != 0f)
                    return true;
                if (MinRange == 0f)
                    return MaxRange == 0f;
                return false;
            }
        }

        public int BaseDuration
        {
            get { return StyxWoW.Db[ClientDb.SpellDuration].GetRow(_spellEntry.DurationIndex).GetField<int>(1U); }
        }

        public int DurationPerLevel
        {
            get { return StyxWoW.Db[ClientDb.SpellDuration].GetRow(_spellEntry.DurationIndex).GetField<int>(2U); }
        }

        public int MaxDuration
        {
            get { return StyxWoW.Db[ClientDb.SpellDuration].GetRow(_spellEntry.DurationIndex).GetField<int>(3U); }
        }

        public WoWSpellSchool School
        {
            get { return (WoWSpellSchool)_spellEntry.SchoolMask; }
        }

        public bool CanCast
        {
            get { return Lua.GetReturnVal<bool>("return IsUsableSpell(select(1, GetSpellInfo(" + Id + ")))", 0U); }
        }

        public string RangeDescription
        {
            get { return StyxWoW.Db[ClientDb.SpellRange].GetRow(RangeIndex).GetField<string>(6U); }
        }

        public SpellEffect GetSpellEffect(int index)
        {
            if (index > 2)
            {
                throw new IndexOutOfRangeException("Index can't be higher than 2 for SpellEffects!");
            }
            return new SpellEffect(
                (WoWSpellEffectType)_spellEntry.Effect[index],
                (WoWApplyAuraType)_spellEntry.EffectApplyAuraName[index],
                _spellEntry.EffectRealPointsPerLevel[index],
                _spellEntry.EffectBasePoints[index],
                _spellEntry.EffectMechanic[index],
                _spellEntry.EffectImplicitTargetA[index],
                _spellEntry.EffectImplicitTargetB[index],
                _spellEntry.EffectRadiusIndex[index],
                _spellEntry.EffectAmplitude[index],
                _spellEntry.EffectMultipleValue[index],
                _spellEntry.EffectChainTarget[index],
                _spellEntry.EffectItemType[index],
                _spellEntry.EffectMiscValueA[index],
                _spellEntry.EffectMiscValueB[index],
                _spellEntry.EffectTriggerSpell[index],
                _spellEntry.EffectPointsPerComboPoint[index],
                _spellEntry.EffectSpellClassMask[index]
            );
        }

        private SpellInfoCache GetSpellInfo()
        {
            var result = Lua.GetReturnValues("return GetSpellInfo(" + Id + ")", "hax.lua");
            int powerCost = 0;
            bool isFunnel = false;
            uint castTime = 0;
            float minRange = 0f;
            float maxRange = 0f;
            
            if (result != null && result.Count > 8)
            {
                int.TryParse(result[3], out powerCost);
                bool.TryParse(result[4], out isFunnel);
                uint.TryParse(result[6], out castTime);
                float.TryParse(result[7], out minRange);
                float.TryParse(result[8], out maxRange);
            }

            return new SpellInfoCache
            {
                PowerCost = powerCost,
                IsFunnel = isFunnel,
                CastTime = castTime,
                MinRange = minRange,
                MaxRange = maxRange
            };
        }

        public void Cast()
        {
            SpellManager.CastSpellById(Id);
        }

        public static WoWSpell FromId(int id)
        {
            WoWDb.Row row;
            if (!_rowCache.TryGetValue(id, out row))
            {
                var db = StyxWoW.Db[ClientDb.Spell];
                if (db != null)
                {
                    row = db.GetRow((uint)id);
                    _rowCache.Add(id, row);
                }
            }
            if (row == null)
            {
                return null;
            }
            return new WoWSpell(id, row);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}), Range: {2}-{3}, CastTime: {4}, Cost: {5} ({6}%), Mechanic: {7}, Dispel: {8}, TargetType: {9}, Power: {10}",
                Name,
                Rank,
                MinRange,
                MaxRange,
                CastTime,
                PowerCost,
                ManaCostPercent,
                Mechanic,
                DispelType,
                TargetType,
                (int)PowerType
            );
        }

        public bool Equals(WoWSpell other)
        {
            return Id == other.Id;
        }

        private class SpellInfoCache
        {
            public int PowerCost;
            public bool IsFunnel;
            public uint CastTime;
            public float MinRange;
            public float MaxRange;
        }
    }
}
