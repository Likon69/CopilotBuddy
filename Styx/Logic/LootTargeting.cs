#nullable disable
using System;
using System.Collections.Generic;
using GreenMagic;
using Styx.Helpers;
using Styx.Logic.AreaManagement;
using Styx.Patchables;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Styx.Logic
{
    public class LootTargeting : Targeting
    {
        private static LootTargeting _instance;
        public Dictionary<ulong, DateTime> AlreadySkinned = new Dictionary<ulong, DateTime>();
        public Dictionary<ulong, DateTime> AlreadyLooted = new Dictionary<ulong, DateTime>();
        public Dictionary<ulong, DateTime> AlreadyHarvested = new Dictionary<ulong, DateTime>();

        private LootTargeting()
        {
            DisplayTargetingExceptions = true;
            MaxTargets = 2;
        }

        public static bool LootFrameIsOpen
        {
            get
            {
                // WotLK 3.3.5a: Use Lua to check if loot frame is open
                string result = Lua.GetReturnVal<string>("if LootFrame and LootFrame:IsShown() then return '1' else return '0' end", 0);
                return result == "1";
            }
        }

        protected override void DefaultRemoveTargetsFilter(List<WoWObject> objects)
        {
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                // BUG-11: Check blacklist first (HB 4.3.4)
                if (Blacklist.Contains(objects[i].Guid))
                {
                    objects.RemoveAt(i);
                    continue;
                }

                if (objects[i] is WoWUnit unit && unit != null)
                {
                    if (unit.IsDisabled)
                    {
                        objects.RemoveAt(i);
                        continue;
                    }

                    // Keep if killed by me and can loot
                    if (unit.KilledByMe && unit.CanLoot)
                        continue;

                    // Keep if skinnable and we skin mobs
                    if ((unit.CanSkin || unit.Skinnable) && SkinMobs && (NinjaSkin || unit.KilledByMe))
                        continue;

                    // Otherwise remove units
                    objects.RemoveAt(i);
                }
                else if (objects[i] is WoWGameObject && objects[i].IsValid)
                {
                    // Keep valid game objects (chests, herbs, minerals)
                    continue;
                }
                else
                {
                    objects.RemoveAt(i);
                }
            }
        }

        protected override void DefaultIncludeTargetsFilter(List<WoWObject> incomingObjects, HashSet<WoWObject> outgoingObjects)
        {
            // No additional targets to include for looting
        }

        protected override void DefaultTargetWeight(List<TargetPriority> objs)
        {
            for (int i = 0; i < objs.Count; i++)
            {
                // Prioritize closer objects (max 200 points, -2 per yard)
                objs[i].Score += 200f - (float)objs[i].Object.Distance * 2f;
            }
        }

        public new static LootTargeting Instance
        {
            get { return _instance ??= new LootTargeting(); }
        }

        public WoWObject FirstObject
        {
            get
            {
                if (LootingList.Count == 0)
                    return null;
                return LootingList[0];
            }
        }

        public List<WoWObject> LootingList
        {
            get { return ObjectList; }
        }

        public static bool SkinMobs
        {
            get { return LevelbotSettings.Instance.SkinMobs; }
        }

        public static bool LootMobs
        {
            get { return LevelbotSettings.Instance.LootMobs; }
        }

        public static bool NinjaSkin
        {
            get { return LevelbotSettings.Instance.NinjaSkin; }
        }

        public static bool HarvestMinerals
        {
            get { return LevelbotSettings.Instance.HarvestMinerals; }
        }

        public static bool HarvestHerbs
        {
            get { return LevelbotSettings.Instance.HarvestHerbs; }
        }

        public static bool LootChests
        {
            get { return LevelbotSettings.Instance.LootChests; }
        }

        public static double LootRadius
        {
            get
            {
                GrindArea currentGrindArea = StyxWoW.AreaManager.CurrentGrindArea;
                if (currentGrindArea != null)
                {
                    double? lootRadius = currentGrindArea.LootRadius;
                    if (lootRadius != null && lootRadius.Value > 5.0)
                    {
                        return lootRadius.Value;
                    }
                }
                return LevelbotSettings.Instance.LootRadius;
            }
        }

        protected override List<WoWObject> GetInitialObjectList()
        {
            return ObjectManager.ObjectList;
        }
    }
}
