using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = TreeSharp.Action;
using Styx.Logic.POI;
using Styx.WoWInternals.WoWObjects;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.World;
using Styx.Logic;
using Styx.Logic.BehaviorTree;
using TreeSharp;
using Styx;
using Styx.Helpers;
using System.Diagnostics;

namespace HighVoltz.Composites
{
    public class ApplyLureAction : Action
    {
        LocalPlayer _me = ObjectManager.Me;
        private static Dictionary<uint, string> Lures = new Dictionary<uint, string>
        { 
            {68049,"Heat-Treated Spinning Lure"},
            {62673,"Feathered Lure"},
            {34861,"Sharpened Fish Hook"},
            {46006,"Glow Worm"},
            {6533,"Aquadynamic Fish Attractor"},
            {7307,"Flesh Eating Worm"},
            {6532,"Bright Baubles"},
            {6530,"Nightcrawlers"},
            {6811,"Aquadynamic Fish Lens"},
            {6529,"Shiny Bauble"},
            {67404,"Glass Fishing Bobber"},
        };

        protected override RunStatus Run(object context)
        {
            if (!_me.IsCasting && !Util1.IsLureOnPole && Applylure())
                return RunStatus.Success;
            return RunStatus.Failure;
        }

        Stopwatch _lureRecastSW = new Stopwatch();
        // does nothing if no lures are in bag
        private bool Applylure()
        {
            if (_lureRecastSW.IsRunning && _lureRecastSW.ElapsedMilliseconds < 10000)
                return false;
            _lureRecastSW.Reset();
            _lureRecastSW.Start();
            if (_me.Inventory.Equipped.MainHand != null &&
                _me.Inventory.Equipped.MainHand.ItemInfo.WeaponClass != WoWItemWeaponClass.FishingPole)
                return false;
            // Weather-Beaten Fishing Hat  
            WoWItem head = _me.Inventory.GetItemBySlot((uint)Styx.WoWEquipSlot.Head);
            if (head != null && head.Entry == 33820)
            {
                AutoAngler.Instance.Log("Appling Weather-Beaten Fishing Hat to fishing pole");
                Util1.UseItemByID(33820);
                return true;
            }
            foreach (var kv in Lures)
            {
                WoWItem _lureInBag = Util1.GetIteminBag(kv.Key);
                if (_lureInBag != null && _lureInBag.Use())
                {
                    AutoAngler.Instance.Log("Appling {0} to fishing pole", kv.Value);
                    return true;
                }
            }
            return false;
        }
    }
}
