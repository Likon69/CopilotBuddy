using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSharp;
using Action = TreeSharp.Action;
using Styx.Logic.Combat;
using Styx.WoWInternals;
using Styx.Logic.POI;
using Styx.WoWInternals.WoWObjects;

namespace HighVoltz.Composites
{
    public class CombatAction : Action
    {
        protected override RunStatus Run(object context)
        {
            if (BotPoi.Current != null && BotPoi.Current.Type == PoiType.Harvest)
            {
                MoveToPoolAction.MoveToPoolSW.Reset();
                MoveToPoolAction.MoveToPoolSW.Start();
            }
            bool Is2Hand = false;
            // equip right hand weapon
            uint mainHandID = AutoAngler.Instance.MySettings.MainHand;
            WoWItem mainHand = ObjectManager.Me.Inventory.Equipped.MainHand;

            if (mainHand == null || (mainHand.Entry != mainHandID && Util1.IsItemInBag(mainHandID)))
            {
                Is2Hand = Util1.GetIteminBag(AutoAngler.Instance.MySettings.MainHand).ItemInfo.InventoryType == Styx.InventoryType.TwoHandWeapon;
                Util1.EquipItemByID(AutoAngler.Instance.MySettings.MainHand);
            }

            // equip left hand weapon
            uint offhandID = AutoAngler.Instance.MySettings.OffHand;
            WoWItem offhand = ObjectManager.Me.Inventory.Equipped.OffHand;

            if ((!Is2Hand && offhandID > 0 &&
                (offhand == null || (offhand.Entry != offhandID && Util1.IsItemInBag(offhandID)))))
            {
                Util1.EquipItemByID(AutoAngler.Instance.MySettings.OffHand);
            }

            if (RoutineManager.Current.CombatBehavior != null) // this check doesn't have any effect. anymore...
            {
                try
                {
                    if (!RoutineManager.Current.CombatBehavior.IsRunning)
                        RoutineManager.Current.CombatBehavior.Start(null);
                    return RoutineManager.Current.CombatBehavior.Tick(null);
                }
                catch
                {
                    RoutineManager.Current.Combat();
                    return RunStatus.Success;
                }
            }
            else
            {
                RoutineManager.Current.Combat();
                return RunStatus.Success;
            }
        }
    }
}
