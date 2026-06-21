using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Styx;
using Styx.Database;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Templar.GUI.Tabs;

namespace Templar.Helpers {
    class Variables {

        // ===========================================================
        // Constants
        // ===========================================================

        // Creature ID
        public const int HordeTundraMammothCreature = 32640;
        public const int SandstoneDrakeCreature = 50269;
        public const int DrixBlackwrenchCreature = 32641;
        public const int AllianceTundraMammothCreature = 32633;
        public const int ExpeditionYakCreature = 62809;

        // Spell ID
        public const int HordeTundraMammothSpell = 61447;
        public const int SandstoneDrakeSpell = 93326;
        public const int ResurrectionSickness = 15007;
        public const int AllianceTundraMammothSpell = 61425;
        public const int ExpeditionYakSpell = 122708;

        // ===========================================================
        // Fields
        // ===========================================================

        public static Stopwatch MountUpStopwatch = new Stopwatch();
        public static Stopwatch MountDelay = new Stopwatch();
        public static Stopwatch ClearTargetTimer = new Stopwatch();
        public static Stopwatch CombatBugStopwatch = new Stopwatch();
        public static Stopwatch AFKTimer = new Stopwatch();

        public static WoWPoint StartLocation = WoWPoint.Zero;
        public static string StartLocationZone;
        public static string StartLocationSubzone;
        public static string StartLocationContinent;

        public static List<WoWItem> MailList = new List<WoWItem>();
        public static List<WoWItem> VendorSellList = new List<WoWItem>();
        public static List<WoWItem> DisenchantList = new List<WoWItem>();

        public static List<WoWItem> InventoryList = new List<WoWItem>();

        public static readonly Stopwatch LootStopwatch = new Stopwatch();
        public static readonly Stopwatch SkinStopwatch = new Stopwatch();
        public static readonly Stopwatch DisenchantStopwatch = new Stopwatch();

        public static readonly Stopwatch PullBlacklistStopwatch = new Stopwatch();
        public static readonly Stopwatch LootBlacklistStopwatch = new Stopwatch();
        public static readonly Stopwatch SkinBlacklistStopwatch = new Stopwatch();
        public static readonly Stopwatch DisenchantBlacklistStopwatch = new Stopwatch();

        public static bool AlteredSettings;
        public static WoWUnit NextMob;
        public static WoWUnit LootMob;
        public static WoWUnit SkinMob;

        public static WoWItem DisenchantItem;

        public static ulong NextMobGuid;
        public static ulong LootMobGuid;
        public static ulong SkinMobGuid;
        public static ulong DisenchantItemGuid;

        public static bool SetStartLocation;

        public static uint NextMobHP;

        public static bool NeedToPull;

        // ===========================================================
        // Constructors
        // ===========================================================

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        public static uint MyLatency {
            get {
                return StyxWoW.WoWClient.Latency;
            }
        }

        public static int MobsTargettingMe {
            get {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Count(u => (u.IsTargetingMeOrPet || (u.IsTargetingAnyMinion && u.OwnedByUnit == StyxWoW.Me)) && !u.IsPlayer);
            }
        }

        public static bool NeedToVendor {
            get {
                return NeedToRepair || MinimumBagSlotsMet;
            }
        }

        public static bool NeedToRepair {
            get {
                return StyxWoW.Me.DurabilityPercent <= (double)VendorSettings.Instance.MinimumDurabilityForRepairs / 100;
            }
        }

        public static bool MinimumBagSlotsMet {
            get {
                return StyxWoW.Me.FreeBagSlots <= (uint)GeneralSettings.Instance.MinimumFreeBagSlots;
            }
        }

        public static bool HasEnoughForRepairs {
            get {
                var repaircost = Lua.GetReturnVal<ulong>("return GetRepairAllCost()", 0);
                return StyxWoW.Me.Copper >= repaircost;
            }
        }

        public static bool IsAtStartLocation {
            get { return StyxWoW.Me.Location.Distance(StartLocation) <= 5; }
        }

        public static WoWGameObject CloseMailbox {
            get {
                return ObjectManager.GetObjectsOfType<WoWGameObject>().Where(go => go != null && go.SubType == WoWGameObjectType.Mailbox).OrderBy(go => go.Distance).FirstOrDefault();
            }
        }

        // No mailbox DB query in WotLK — rely on CloseMailbox from ObjectManager instead.
        public static NpcResult FarMailbox {
            get { return null; }
        }

        public static WoWUnit CloseRepairVendor {
            get {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u != null && u.IsRepairMerchant && u.IsAlive && !u.IsHostile).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public static WoWUnit CloseVendor {
            get {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u != null && u.IsVendor && u.IsAlive && !u.IsHostile).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public static NpcResult FarRepairVendor {
            get {
                return NpcQueries.GetNearestNpc(StyxWoW.Me.FactionTemplate.Faction, StyxWoW.Me.MapId, StyxWoW.Me.Location, UnitNPCFlags.Repair);
            }
        }

        public static NpcResult FarVendor {
            get {
                return NpcQueries.GetNearestNpc(StyxWoW.Me.FactionTemplate.Faction, StyxWoW.Me.MapId, StyxWoW.Me.Location, UnitNPCFlags.AnyVendor);
            }
        }

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================
    }
}
