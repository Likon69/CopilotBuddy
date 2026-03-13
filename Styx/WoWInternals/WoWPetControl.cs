using System;
using System.Linq;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic.Combat;
using Styx.WoWInternals.WoWObjects;

namespace Styx.WoWInternals
{
    public class WoWPetControl
    {
        public static bool CanCastAction(WoWPetSpell petAction)
        {
            return petAction != null
                && (petAction.SpellType != WoWPetSpell.PetSpellType.Spell || !(petAction.Spell == null))
                && !StyxWoW.Me.Mounted
                && (petAction.SpellType != WoWPetSpell.PetSpellType.Spell || !petAction.Spell.Cooldown);
        }

        public static void CastAction(WoWPetSpell action, WoWUnit wowUnit = null)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (wowUnit != null && !wowUnit.IsValid)
                throw new ArgumentException("The unit passed in is invalid", "wowUnit");

            string actionName = GetActionName(action);
            if (wowUnit == null)
            {
                Logging.WriteDiagnostic("Instructing pet to '{0}'.", actionName);
                Lua.DoString("CastPetAction({0})", action.ActionBarIndex + 1);
            }
            else
            {
                Logging.WriteDiagnostic("Instructing pet to '{0}' on '{1}'.", actionName, wowUnit.Name);
                // WoW 3.3.5a: set focus to current target then cast on focus
                using (StyxWoW.Memory.AcquireFrame(true))
                {
                    Lua.DoString("FocusUnit('target')");
                    Lua.DoString("CastPetAction({0}, 'focus')", action.ActionBarIndex + 1);
                    Lua.DoString("ClearFocus()");
                }
            }
        }

        public static WoWPetSpell FindActionByName(string actionName)
        {
            return StyxWoW.Me.PetSpells.FirstOrDefault(s => GetActionName(s).Equals(actionName, StringComparison.OrdinalIgnoreCase));
        }

        public static string GetActionName(WoWPetSpell action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            switch (action.SpellType)
            {
                case WoWPetSpell.PetSpellType.Spell:
                    return action.Spell.Name;
                case WoWPetSpell.PetSpellType.Action:
                    return action.Action.ToString();
                case WoWPetSpell.PetSpellType.Stance:
                    return action.Stance.ToString();
                default:
                    return "Unknown";
            }
        }

        public static bool IsActionActive(WoWPetSpell petAction)
        {
            return petAction != null && Lua.GetReturnVal<bool>(string.Format("return GetPetActionInfo({0})", petAction.ActionBarIndex + 1), 4U);
        }

        public static bool CanPetBeDismissed
        {
            get { return StyxWoW.Me.GotAlivePet && Lua.GetReturnVal<bool>("return PetCanBeDismissed()", 0U); }
        }

        public static bool DismissPet()
        {
            if (!CanPetBeDismissed)
                return false;

            if (StyxWoW.Me.Class == WoWClass.Hunter)
            {
                if (StyxWoW.Me.IsMoving)
                    WoWMovement.MoveStop();
                SpellManager.Cast("Dismiss Pet");
            }
            else
            {
                Lua.DoString("PetDismiss(); DestroyTotem(1); DestroyTotem(2); DestroyTotem(3); DestroyTotem(4);");
            }
            return true;
        }

        public static void Attack(WoWUnit wowUnit)
        {
            if (wowUnit == null || !wowUnit.IsValid)
                return;
            if (!StyxWoW.Me.GotAlivePet)
                return;
            if (StyxWoW.Me.Pet != null && StyxWoW.Me.Pet.CurrentTarget == wowUnit)
                return;

            var attackAction = StyxWoW.Me.PetSpells.FirstOrDefault(
                s => s.SpellType == WoWPetSpell.PetSpellType.Action && s.Action == WoWPetSpell.PetAction.Attack);
            if (attackAction != null)
                CastAction(attackAction, wowUnit);
        }

        public static void Follow()
        {
            if (!StyxWoW.Me.GotAlivePet)
                return;

            var followAction = StyxWoW.Me.PetSpells.FirstOrDefault(
                s => s.SpellType == WoWPetSpell.PetSpellType.Action && s.Action == WoWPetSpell.PetAction.Follow);
            if (followAction == null)
                return;

            if ((!IsActionActive(followAction) || (StyxWoW.Me.Pet != null && StyxWoW.Me.Pet.GotTarget)) && CanCastAction(followAction))
                CastAction(followAction);
        }

        public static void SetStance(WoWPetSpell.PetStance stance)
        {
            if (!StyxWoW.Me.GotAlivePet)
                return;

            var stanceAction = StyxWoW.Me.PetSpells.FirstOrDefault(
                s => s.SpellType == WoWPetSpell.PetSpellType.Stance && s.Stance == stance);
            if (stanceAction != null && !IsActionActive(stanceAction) && CanCastAction(stanceAction))
                CastAction(stanceAction);
        }
    }
}
