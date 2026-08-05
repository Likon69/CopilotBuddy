#nullable disable
using System;
using System.Collections.Generic;
using GreenMagic;
using Styx.Helpers;
using Styx.Patchables;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

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
        /// Returns true if this is a melee-range spell (SpellRangeId == 2).
        /// Does NOT include self-only spells (RangeId == 1). Matches HB 4.3.4.
        /// </summary>
        public bool IsMeleeSpell => SpellRangeId == 2;

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

        public uint AttributesEx
        {
            get { return _spellEntry.AttributesEx; }
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
            get { return _spellEntry.MaxAffectedTargets; }
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

        public string Description
        {
            get { return ObjectManager.Wow.Read<string>(_spellEntry.Description); }
        }

        // HB 4.3.4 WoWSpell.cs line 426: delegates to CooldownTimeLeft
        public bool Cooldown
        {
            get
            {
                // Primary: ASM call to Spell_C__GetSpellCooldown — HB 3.3.5a method_0 pattern.
                // Language-independent, no Lua involved, no timing race.
                // ecx = SpellCooldownPtr (0xD3F5AC), args: push 0,0,0,0,spellId; call 0x807980
                try
                {
                    ExecutorRand? executor = ObjectManager.Executor;
                    if (executor != null)
                    {
                        lock (executor.AssemblyLock)
                        {
                            executor.Clear();
                            executor.AddLine("push 0");
                            executor.AddLine("push 0");
                            executor.AddLine("push 0");
                            executor.AddLine("push 0");
                            executor.AddLine("push {0}", _id);
                            executor.AddLine("mov ecx, {0}", 0xD3F5ACU); // SpellCooldownPtr
                            executor.AddLine("call {0}", 8419712U);       // Spell_C__GetSpellCooldown = 0x807980
                            executor.AddLine("retn");
                            executor.Execute();
                            int result = executor.Memory.Read<int>(executor.ReturnPointer);
                            return result != 0;
                        }
                    }
                }
                catch
                {
                    // Fall through to Lua-based check
                }
                // Fallback: Lua (may be unreliable on non-English clients or slow machines)
                return CooldownTimeLeft.TotalMilliseconds > 0.0;
            }
        }

        /// <summary>
        /// Gets the remaining cooldown time for this spell.
        /// WotLK 3.3.5a: GetSpellCooldown() requires the spell name in the CLIENT'S LANGUAGE.
        /// Passing the English name fails on non-English clients (e.g. "Blood Fury" on a Russian
        /// client where the spell is "\208\175\209\128\208\190\209\129\209\130\209\140 \208\186\209\128\208\190\208\178\208\184").
        /// Fix: use GetSpellInfo(id) to get the localized name first — language-independent.
        /// </summary>
        public TimeSpan CooldownTimeLeft
        {
            get
            {
                // GetSpellInfo(id) returns the localized spell name regardless of client language.
                // GetSpellCooldown(localizedName) then works on ALL clients.
                double returnVal = Lua.GetReturnVal<double>(
                    string.Format("local n=GetSpellInfo({0}); if not n then return 0 end; local x,y=GetSpellCooldown(n); if not x then return 0 end; return x+y-GetTime()", Id), 0U);
                if (returnVal <= 0.0)
                    return TimeSpan.Zero;
                return TimeSpan.FromSeconds(returnVal);
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
            get
            {
                return Lua.GetReturnVal<bool>("return IsUsableSpell(select(1, GetSpellInfo(" + Id + ")))", 0U);
            }
        }

        public string RangeDescription
        {
            get { return StyxWoW.Db[ClientDb.SpellRange].GetRow(RangeIndex).GetField<string>(6U); }
        }

        public SpellEffect GetSpellEffect(int index)
        {
            // HB 4.3.4 returns null for missing/empty effects.
            // WotLK 3.3.5a DBC has fixed 3-slot arrays; unused slots have Effect == 0.
            if (index < 0 || index > 2)
                return null;

            if (_spellEntry.Effect[index] == 0)
                return null;

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
