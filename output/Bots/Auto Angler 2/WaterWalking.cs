using System.Linq;
using Styx.WoWInternals;
using Styx.Logic.Combat;
using Styx.WoWInternals.WoWObjects;
using System.Diagnostics;
using Styx.Helpers;
using Styx;

namespace HighVoltz
{
    public class WaterWalking
    {
        public static bool CanCast
        {
            get
            {
                return AutoAngler.Instance.MySettings.UseWaterWalking && 
                    (SpellManager.HasSpell(1706) ||// priest levitate
                    SpellManager.HasSpell(546) || // shaman water walking
                    SpellManager.HasSpell(3714) ||// Dk Path of frost
                    Util1.IsItemInBag(8827));//isItemInBag(8827);
            }
        }

        public static bool IsActive
        {
            get
            { // DKs have 2 Path of Frost auras. only one can be stored in WoWAuras at any time. 
                return ObjectManager.Me.Auras.Values.
                    Count(a => (a.SpellId == 11319 || a.SpellId == 1706 || a.SpellId == 546) &&
                    a.TimeLeft >= new System.TimeSpan(0, 0, 20)) > 0 ||
                    ObjectManager.Me.HasAura("Path of Frost");
            }
        }

        static Stopwatch _recastSW = new Stopwatch();
        static public bool Cast()
        {
 
            bool casted = false;
            if (!IsActive)
            {
                if (_recastSW.IsRunning && _recastSW.ElapsedMilliseconds < 5000)
                    return false;
                _recastSW.Reset();
                _recastSW.Start();
                int waterwalkingSpellID = 0;
                switch (ObjectManager.Me.Class)
                {
                    case Styx.Combat.CombatRoutine.WoWClass.Priest:
                        waterwalkingSpellID = 1706;
                        break;
                    case Styx.Combat.CombatRoutine.WoWClass.Shaman:
                        waterwalkingSpellID = 546;
                        break;
                    case Styx.Combat.CombatRoutine.WoWClass.DeathKnight:
                        waterwalkingSpellID = 3714;
                        break;
                }
                if (SpellManager.CanCast(waterwalkingSpellID))
                {
                    SpellManager.Cast(waterwalkingSpellID);
                    casted = true;
                }
                WoWItem waterPot = Util1.GetIteminBag(8827);
                if (waterPot != null && waterPot.Use())
                {
                    casted = true;
                }
            }
            if (ObjectManager.Me.IsSwimming)
            {
                using (new FrameLock())
                {
                    KeyboardManager.AntiAfk();
                    WoWMovement.Move(WoWMovement.MovementDirection.JumpAscend);
                }
            }
            return casted;
        }
    }
}
