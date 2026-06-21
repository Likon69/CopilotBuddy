using System.Diagnostics;
using System.Linq;
using ObjectGatherer.GUI.Tabs;
using Styx;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace ObjectGatherer.Helpers {
    public class Variables {
        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        public static readonly Stopwatch BusyStopwatch = new Stopwatch();
        public static readonly Stopwatch LootStopwatch = new Stopwatch();
        public static readonly Stopwatch LootMobBlacklistStopwatch = new Stopwatch();
        public static readonly Stopwatch LootObjectBlacklistStopwatch = new Stopwatch();
        public static readonly Stopwatch WaitBeforeInteractStopwatch = new Stopwatch();

        public static WoWGameObject PreviousLootObject;
        public static WoWGameObject LootObject;
        public static WoWUnit PreviousLootCorpse;
        public static WoWUnit LootMob;

        public static ulong LootMobGuid;

        public static bool NeedToLoot;
        public static bool NeedToDismount = false;

        // ===========================================================
        // Constructors
        // ===========================================================

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        public static bool HasAncientGuoLaiCacheKey {
            get {
                return StyxWoW.Me.BagItems.FirstOrDefault(i => i.Entry == 87779) != null;
            }
        }

        public static int ItemCount(uint item) {
            return Lua.GetReturnVal<int>(string.Format("return GetItemCount({0}, true)", item), 0);
        }

        public static bool CanSafelyLoot {
            get {
                return !StyxWoW.Me.IsOnTransport && !StyxWoW.Me.IsGhost && !StyxWoW.Me.IsDead && !StyxWoW.Me.Combat &&
                       !StyxWoW.Me.IsActuallyInCombat && !(ObjectManager.GetObjectsOfType<WoWUnit>().Any(unit => unit.Aggro || unit.PetAggro));
            }
        }

        public static WoWGameObject GetNextObject {
            get {
                if(GeneralSettings.Instance.GatherHerbs) {
                    return GeneralSettings.Instance.GatherOre ? ObjectManager.GetObjectsOfType<WoWGameObject>().Where(NextListObjectOrNode).OrderBy(o => StyxWoW.Me.Location.Distance(o.Location)).FirstOrDefault() : ObjectManager.GetObjectsOfType<WoWGameObject>().Where(NextListObjectOrHerb).OrderBy(o => StyxWoW.Me.Location.Distance(o.Location)).FirstOrDefault();
                }

                return GeneralSettings.Instance.GatherOre ? ObjectManager.GetObjectsOfType<WoWGameObject>().Where(NextListObjectOrMineral).OrderBy(o => StyxWoW.Me.Location.Distance(o.Location)).FirstOrDefault() : ObjectManager.GetObjectsOfType<WoWGameObject>().Where(NextListObject).OrderBy(o => StyxWoW.Me.Location.Distance(o.Location)).FirstOrDefault();
            }
        }

        public static WoWUnit GetNextCorpse {
            get {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.IsDead && u.Lootable && u.CanLoot && !CustomBlacklist.Contains(u.Guid)).OrderBy(u => StyxWoW.Me.Location.Distance(u.Location)).FirstOrDefault();
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

        private static bool NextObject(WoWGameObject o) {
            return o.CanUse() && !CustomBlacklist.Contains(o.Guid) && o.IsValid;
        }

        private static bool NextListObjectOrHerb(WoWGameObject o) {
            return NextObject(o) && (o.IsHerb || EntryList.IdList.Contains(o.Entry));
        }

        private static bool NextListObjectOrMineral(WoWGameObject o) {
            return NextObject(o) && (o.IsMineral || EntryList.IdList.Contains(o.Entry));
        }

        private static bool NextListObjectOrNode(WoWGameObject o) {
            return NextObject(o) && ((o.IsMineral || o.IsHerb) || EntryList.IdList.Contains(o.Entry));
        }

        private static bool NextListObject(WoWGameObject o) {
            return NextObject(o) && EntryList.IdList.Contains(o.Entry);
        }
    }
}
