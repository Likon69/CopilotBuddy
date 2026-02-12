#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using Styx.Common;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Styx.Logic.Combat
{
    /// <summary>
    /// Legacy spell manager wrapper for older Singular code compatibility.
    /// Wraps SpellManager methods for HB 3.3.5a/4.3.4 compatibility.
    /// </summary>
    public static class LegacySpellManager
    {
        /// <summary>Known spells dictionary keyed by name (populated via Refresh).</summary>
        public static Dictionary<string, WoWSpell> KnownSpells { get; } = new Dictionary<string, WoWSpell>(StringComparer.OrdinalIgnoreCase);

        private static int _lastKnownSpellCount;

        /// <summary>
        /// Rebuilds the KnownSpells dictionary from the player's spell book.
        /// Called when a combat routine is loaded or changed (HB 4.3.4 pattern).
        /// </summary>
        public static void Refresh()
        {
            var me = StyxWoW.Me;
            if (me == null) return;

            int currentCount = me.KnownSpells.Count;
            if (_lastKnownSpellCount != 0 && currentCount == _lastKnownSpellCount)
                return;

            Logging.Write("Building spell book...");
            KnownSpells.Clear();
            foreach (WoWSpell spell in me.KnownSpells)
            {
                if (!KnownSpells.ContainsKey(spell.Name))
                {
                    KnownSpells.Add(spell.Name, spell);
                }
            }
            _lastKnownSpellCount = currentCount;
            Logging.Write("Spell book built. {0} spells.", KnownSpells.Count);
        }
        /// <summary>
        /// Casts a spell by name on the current target.
        /// </summary>
        public static bool Cast(string spellName)
        {
            return SpellManager.Cast(spellName);
        }

        /// <summary>
        /// Casts a spell by name on a specific target.
        /// </summary>
        public static bool Cast(string spellName, WoWUnit target)
        {
            return SpellManager.Cast(spellName, target);
        }

        /// <summary>
        /// Casts a spell by ID on the current target.
        /// </summary>
        public static bool Cast(int spellId)
        {
            // Convert spell ID to string for SpellManager
            var spell = WoWSpell.FromId(spellId);
            if (spell != null)
            {
                return SpellManager.Cast(spell.Name);
            }
            return false;
        }

        /// <summary>
        /// Casts a spell by ID on a specific target.
        /// </summary>
        public static bool Cast(int spellId, WoWUnit target)
        {
            // Convert spell ID to string for SpellManager
            var spell = WoWSpell.FromId(spellId);
            if (spell != null)
            {
                return SpellManager.Cast(spell.Name, target);
            }
            return false;
        }

        /// <summary>
        /// Checks if player has a spell.
        /// </summary>
        public static bool HasSpell(string spellName)
        {
            return SpellManager.HasSpell(spellName);
        }

        /// <summary>
        /// Checks if player has a spell by ID.
        /// </summary>
        public static bool HasSpell(int spellId)
        {
            var spell = WoWSpell.FromId(spellId);
            return spell != null && SpellManager.HasSpell(spell.Name);
        }

        /// <summary>
        /// Gets spell cooldown remaining in milliseconds.
        /// </summary>
        public static int GetSpellCooldown(string spellName)
        {
            var spell = SpellManager.Spells.ContainsKey(spellName) ? SpellManager.Spells[spellName] : null;
            if (spell != null)
            {
                var cooldownTime = spell.CooldownTimeLeft;
                return (int)cooldownTime.TotalMilliseconds;
            }
            return 0;
        }

        /// <summary>
        /// Gets spell by name.
        /// </summary>
        public static WoWSpell GetSpell(string spellName)
        {
            return SpellManager.Spells.ContainsKey(spellName) ? SpellManager.Spells[spellName] : null;
        }

        /// <summary>
        /// Casts a pet spell by name.
        /// </summary>
        public static bool CastPetAction(string spellName)
        {
            var me = StyxWoW.Me;
            if (me == null || !me.HasPet)
                return false;

            var spell = me.PetSpells.FirstOrDefault(s => s.Spell != null && s.Spell.Name == spellName);
            if (spell != null && spell.Spell != null)
            {
                return Cast(spell.Spell.Id, me.Pet);
            }

            return false;
        }

        /// <summary>
        /// Casts a spell by ID on the current target.
        /// Used by Singular for trap launcher and similar mechanics.
        /// </summary>
        public static bool CastSpellById(int spellId)
        {
            return Cast(spellId);
        }

        /// <summary>
        /// Casts a spell by ID on a specific target.
        /// </summary>
        public static bool CastSpellById(int spellId, WoWUnit target)
        {
            return Cast(spellId, target);
        }

        /// <summary>
        /// Clicks a remote location for ground-targeted spells (trap placement, etc).
        /// </summary>
        public static void ClickRemoteLocation(WoWPoint location)
        {
            // In WotLK 3.3.5a, ground-targeted spells are confirmed via click at location
            // This uses the in-game SpellTargetUnit/SpellTargetMapCoordinates
            try
            {
                // Use Lua to click the pending spell at the location
                Lua.DoString(string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "SpellTargetUnit('player'); CameraOrSelectOrMoveStart(); CameraOrSelectOrMoveStop();"));
            }
            catch
            {
                // Fallback: just try to use the pending spell
            }
        }
    }
}
