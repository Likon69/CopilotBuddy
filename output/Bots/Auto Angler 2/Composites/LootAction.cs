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
using Styx.Logic.Inventory.Frames.LootFrame;


namespace HighVoltz.Composites
{
    public class LootAction : Action
    {
        static Stopwatch _lootSw = new Stopwatch();
        public static Stopwatch WaitingForLootSW { get { return _lootSw; } }
        protected override RunStatus Run(object context)
        {
            if (GetLoot())
                return RunStatus.Success;
            else
                return RunStatus.Failure;
        }
        /// <summary>
        /// returns true if waiting for loot or if successfully looted.
        /// </summary>
        /// <returns></returns>
        public static bool GetLoot()
        {
            if (_lootSw.IsRunning && _lootSw.ElapsedMilliseconds < 5000)
            {
                // loot everything.
                if (LootFrame.Instance != null && LootFrame.Instance.IsVisible)
                {
                    for (int i = 0; i < LootFrame.Instance.LootItems; i++)
                    {
                        var lootInfo = LootFrame.Instance.LootInfo(i);
                        if (AutoAngler.FishCaught.ContainsKey(lootInfo.LootName))
                            AutoAngler.FishCaught[lootInfo.LootName] += (uint)lootInfo.LootQuantity;
                        else
                            AutoAngler.FishCaught.Add(lootInfo.LootName, (uint)lootInfo.LootQuantity);
                    }
                    LootFrame.Instance.LootAll();
                    _lootSw.Reset();
                }
                return true;
            }
            return false;
        }
    }
}
