#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Styx.Logic.Combat
{
    /// <summary>
    /// Extended spell manager with CanBuff/Buff/CastRandom/BuffRandom helpers.
    /// FEAT-06: All methods have been merged into SpellManager. This class is kept
    /// for backward compatibility only — new code should use SpellManager directly.
    /// </summary>
    [System.Obsolete("Use SpellManager instead. All SpellManagerEx methods are now in SpellManager.")]
    public class SpellManagerEx
    {
        private static readonly Random _random;
        static SpellManagerEx()
        {
            _random = new Random(Environment.TickCount);
        }

        public SpellManagerEx()
        {
        }

        public static bool HasSpell(string name)
        {
            return SpellManager.HasSpell(name);
        }

        public static bool HasSpell(int id)
        {
            return SpellManager.HasSpell(id);
        }

        public static bool HasSpell(WoWSpell spell)
        {
            return SpellManager.KnownSpells.ContainsValue(spell);
        }

        public static bool CanCast(string spellName)
        {
            return CanCast(spellName, false);
        }

        public static bool CanCast(string spellName, WoWUnit target)
        {
            return CanCast(spellName, target, false);
        }

        public static bool CanCast(string spellName, bool checkRange)
        {
            return CanCast(spellName, StyxWoW.Me.CurrentTarget, checkRange);
        }

        public static bool CanCast(string spellName, WoWUnit target, bool checkRange)
        {
            WoWSpell spell;
            if (SpellManager.KnownSpells.TryGetValue(spellName, out spell))
            {
                return CanCast(spell, target, checkRange);
            }
            return false;
        }

        public static bool CanCast(int spellId)
        {
            return CanCast(spellId, false);
        }

        public static bool CanCast(int spellId, WoWUnit target)
        {
            return CanCast(spellId, target, false);
        }

        public static bool CanCast(int spellId, bool checkRange)
        {
            return CanCast(spellId, StyxWoW.Me.CurrentTarget, checkRange);
        }

        public static bool CanCast(int spellId, WoWUnit target, bool checkRange)
        {
            return CanCast(SpellManager.KnownSpells.Values.FirstOrDefault(s => s.Id == spellId), target, checkRange);
        }

        public static bool CanCast(WoWSpell spell)
        {
            return CanCast(spell, false);
        }

        public static bool CanCast(WoWSpell spell, WoWUnit target)
        {
            return CanCast(spell, target, false);
        }

        public static bool CanCast(WoWSpell spell, bool checkRange)
        {
            return CanCast(spell, StyxWoW.Me.CurrentTarget, checkRange);
        }

        public static bool CanCast(WoWSpell spell, WoWUnit target, bool checkRange)
        {
            if (spell == null)
            {
                return false;
            }
            if (!HasSpell(spell))
            {
                return false;
            }
            if (StyxWoW.GlobalCooldown)
            {
                return false;
            }
            if (spell.Cooldown)
            {
                return false;
            }
            if (StyxWoW.Me.IsCasting)
            {
                return false;
            }
            if (!spell.CanCast)
            {
                return false;
            }
            if (spell.CastTime != 0U && StyxWoW.Me.IsMoving)
            {
                return false;
            }
            if (checkRange)
            {
                return target.Distance < (double)spell.MaxRange;
            }
            return true;
        }

        public static bool CanBuff(string spellName)
        {
            return CanBuff(spellName, false);
        }

        public static bool CanBuff(string spellName, WoWUnit target)
        {
            return CanBuff(spellName, target, false);
        }

        public static bool CanBuff(string spellName, bool checkRange)
        {
            return CanBuff(spellName, StyxWoW.Me, checkRange);
        }

        public static bool CanBuff(string spellName, WoWUnit target, bool checkRange)
        {
            WoWSpell spell;
            if (SpellManager.KnownSpells.TryGetValue(spellName, out spell))
            {
                return CanBuff(spell, target, checkRange);
            }
            return false;
        }

        public static bool CanBuff(int spellId)
        {
            return CanBuff(spellId, false);
        }

        public static bool CanBuff(int spellId, WoWUnit target)
        {
            return CanBuff(spellId, target, false);
        }

        public static bool CanBuff(int spellId, bool checkRange)
        {
            return CanBuff(spellId, StyxWoW.Me, checkRange);
        }

        public static bool CanBuff(int spellId, WoWUnit target, bool checkRange)
        {
            return CanBuff(SpellManager.KnownSpells.Values.FirstOrDefault(s => s.Id == spellId), target, checkRange);
        }

        public static bool CanBuff(WoWSpell spell)
        {
            return CanBuff(spell, false);
        }

        public static bool CanBuff(WoWSpell spell, WoWUnit target)
        {
            return CanBuff(spell, target, false);
        }

        public static bool CanBuff(WoWSpell spell, bool checkRange)
        {
            return CanBuff(spell, StyxWoW.Me, checkRange);
        }

        public static bool CanBuff(WoWSpell spell, WoWUnit target, bool checkRange)
        {
            if (CanCast(spell, target, checkRange))
            {
                return !target.HasAura(spell.Name);
            }
            return false;
        }

        public static bool Cast(string spellName)
        {
            return Cast(spellName, StyxWoW.Me.CurrentTarget);
        }

        public static bool Cast(string spellName, WoWUnit target)
        {
            WoWSpell spell;
            if (SpellManager.KnownSpells.TryGetValue(spellName, out spell))
            {
                return Cast(spell, target);
            }
            return false;
        }

        public static bool Cast(int spellId)
        {
            return Cast(spellId, StyxWoW.Me.CurrentTarget);
        }

        public static bool Cast(int spellId, WoWUnit target)
        {
            return Cast(SpellManager.KnownSpells.Values.FirstOrDefault(s => s.Id == spellId), target);
        }

        public static bool Cast(WoWSpell spell)
        {
            return Cast(spell, StyxWoW.Me.CurrentTarget);
        }

        public static bool Cast(WoWSpell spell, WoWUnit target)
        {
            if (spell != null)
            {
                SpellManager.CastSpellById(spell.Id, (target == null) ? 0UL : target.Guid);
                return true;
            }
            return false;
        }

        public static bool Buff(string spellName)
        {
            return Buff(spellName, StyxWoW.Me);
        }

        public static bool Buff(string spellName, WoWUnit target)
        {
            WoWSpell spell;
            if (SpellManager.KnownSpells.TryGetValue(spellName, out spell))
            {
                return Buff(spell, target);
            }
            return false;
        }

        public static bool Buff(int spellId)
        {
            return Buff(spellId, StyxWoW.Me);
        }

        public static bool Buff(int spellId, WoWUnit target)
        {
            return Buff(SpellManager.KnownSpells.Values.FirstOrDefault(s => s.Id == spellId), target);
        }

        public static bool Buff(WoWSpell spell)
        {
            return Buff(spell, StyxWoW.Me);
        }

        public static bool Buff(WoWSpell spell, WoWUnit target)
        {
            return Cast(spell, target);
        }

        public static bool CastRandom(IEnumerable<string> spellNames, WoWUnit target, bool checkRange)
        {
            return CastRandom(spellNames.Select(n => SpellManager.KnownSpells.Values.FirstOrDefault(s => s.Name == n)), target, checkRange);
        }

        public static bool CastRandom(IEnumerable<string> spellNames, WoWUnit target)
        {
            return CastRandom(spellNames, target, false);
        }

        public static bool CastRandom(IEnumerable<string> spellNames, bool checkRange)
        {
            return CastRandom(spellNames, StyxWoW.Me.CurrentTarget, checkRange);
        }

        public static bool CastRandom(IEnumerable<int> spellIds, WoWUnit target, bool checkRange)
        {
            return CastRandom(spellIds.Select(i => SpellManager.KnownSpells.Values.FirstOrDefault(s => s.Id == i)), target, checkRange);
        }

        public static bool CastRandom(IEnumerable<int> spellIds, WoWUnit target)
        {
            return CastRandom(spellIds, target, false);
        }

        public static bool CastRandom(IEnumerable<int> spellIds, bool checkRange)
        {
            return CastRandom(spellIds, StyxWoW.Me.CurrentTarget, checkRange);
        }

        public static bool CastRandom(IEnumerable<WoWSpell> _spellList, WoWUnit target)
        {
            return CastRandom(_spellList, target, false);
        }

        public static bool CastRandom(IEnumerable<WoWSpell> _spellList, bool checkRange)
        {
            return CastRandom(_spellList, StyxWoW.Me.CurrentTarget, checkRange);
        }

        public static bool CastRandom(IEnumerable<WoWSpell> _spellList, WoWUnit target, bool checkRange)
        {
            List<WoWSpell> list = _spellList.ToList();
            while (list.Count != 0)
            {
                int index = _random.Next(0, list.Count);
                if (CanCast(list[index], target, checkRange))
                {
                    Cast(list[index], target);
                    return true;
                }
                list.RemoveAt(index);
            }
            return false;
        }

        public static bool BuffRandom(IEnumerable<string> spellNames, WoWUnit target, bool checkRange)
        {
            return BuffRandom(spellNames.Select(n => SpellManager.KnownSpells.Values.FirstOrDefault(s => s.Name == n)), target, checkRange);
        }

        public static bool BuffRandom(IEnumerable<string> spellNames, WoWUnit target)
        {
            return BuffRandom(spellNames, target, false);
        }

        public static bool BuffRandom(IEnumerable<string> spellNames, bool checkRange)
        {
            return BuffRandom(spellNames, StyxWoW.Me.CurrentTarget, checkRange);
        }

        public static bool BuffRandom(IEnumerable<int> spellIds, WoWUnit target, bool checkRange)
        {
            return BuffRandom(spellIds.Select(i => SpellManager.KnownSpells.Values.FirstOrDefault(s => s.Id == i)), target, checkRange);
        }

        public static bool BuffRandom(IEnumerable<int> spellIds, WoWUnit target)
        {
            return BuffRandom(spellIds, target, false);
        }

        public static bool BuffRandom(IEnumerable<int> spellIds, bool checkRange)
        {
            return BuffRandom(spellIds, StyxWoW.Me.CurrentTarget, checkRange);
        }

        public static bool BuffRandom(IEnumerable<WoWSpell> _spellList, WoWUnit target)
        {
            return BuffRandom(_spellList, target, false);
        }

        public static bool BuffRandom(IEnumerable<WoWSpell> _spellList, bool checkRange)
        {
            return BuffRandom(_spellList, StyxWoW.Me.CurrentTarget, checkRange);
        }

        public static bool BuffRandom(IEnumerable<WoWSpell> _spellList, WoWUnit target, bool checkRange)
        {
            List<WoWSpell> list = _spellList.ToList();
            while (list.Count != 0)
            {
                int index = _random.Next(0, list.Count);
                if (CanBuff(list[index], target, checkRange))
                {
                    Buff(list[index], target);
                    return true;
                }
                list.RemoveAt(index);
            }
            return false;
        }

        public static void StopCasting()
        {
            Lua.DoString("_spellstopCasting()");
        }

        private static void OnBotStart(EventArgs args)
        {
            RefreshSpells();
            Lua.Events.DetachEvent("SPELLS_CHANGED", OnSpellsChanged);
            Lua.Events.DetachEvent("LEARNED_SPELL_IN_TAB", OnSpellsChanged);
            Lua.Events.AttachEvent("SPELLS_CHANGED", OnSpellsChanged);
            Lua.Events.AttachEvent("LEARNED_SPELL_IN_TAB", OnSpellsChanged);
        }

        internal static void Initialize()
        {
            // Delegates to SpellManager.KnownSpells — no separate spell cache needed
            BotEvents.OnBotStart += OnBotStart;
        }

        internal static void Shutdown()
        {
            BotEvents.OnBotStart -= OnBotStart;
        }

        private static void OnSpellsChanged(object sender, LuaEventArgs e)
        {
            // SpellManager handles its own refresh via Refresh()
        }

        private static void RefreshSpells()
        {
            // SpellManager handles its own refresh via Refresh()
        }

        public static Dictionary<string, WoWSpell> Spells
        {
            get
            {
                return new Dictionary<string, WoWSpell>(SpellManager.KnownSpells);
            }
        }
    }
}
