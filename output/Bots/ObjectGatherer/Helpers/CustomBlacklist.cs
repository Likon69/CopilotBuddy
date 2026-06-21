using System;
using System.Collections.Generic;
using System.Linq;
using Styx.Helpers;
using Styx.Database;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace ObjectGatherer.Helpers {
    public class CustomBlacklist {
        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        private static readonly Dictionary<ulong, DateTime> GuidBlackList = new Dictionary<ulong, DateTime>();
        private static readonly Dictionary<int, DateTime> EntryBlacklist = new Dictionary<int, DateTime>();
        private static readonly Dictionary<string, DateTime> NameBlacklist = new Dictionary<string, DateTime>();

        private static WaitTimer _sweepTimer;

        // ===========================================================
        // Constructors
        // ===========================================================

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        public static void SweepTimer() {
            var maxSweepTime = TimeSpan.FromSeconds(30);

            _sweepTimer = new WaitTimer(maxSweepTime) {
                WaitTime = maxSweepTime
            };
        }

        public static void Add(ulong guid, TimeSpan timeSpan) {
            RemoveExpired();
            GuidBlackList[guid] = DateTime.Now.Add(timeSpan);
        }

        public static void Add(string name, TimeSpan timeSpan) {
            RemoveExpired();
            NameBlacklist[name] = DateTime.Now.Add(timeSpan);
        }

        public static void Add(int entry, TimeSpan timeSpan) {
            RemoveExpired();
            EntryBlacklist[entry] = DateTime.Now.Add(timeSpan);
        }

        public static void Add(WoWObject wowObject, TimeSpan timeSpan) {
            if(wowObject != null) {
                Add(wowObject.Guid, timeSpan);
            }
        }

        public static void Add(NpcResult npcResult, TimeSpan timeSpan) {
            if(npcResult != null) {
                Add(npcResult.Entry, timeSpan);
            }
        }

        public static bool Contains(WoWObject wowObject) {
            return (wowObject != null) && Contains(wowObject.Guid);
        }

        public static bool Contains(NpcResult npcResult) {
            return (npcResult != null) && Contains(npcResult.Entry);
        }

        public static bool Contains(ulong guid) {
            DateTime expiry;
            if(GuidBlackList.TryGetValue(guid, out expiry)) {
                return (expiry > DateTime.Now);
            }
            return false;
        }

        public static bool Contains(string name) {
            DateTime expiry;
            if(NameBlacklist.TryGetValue(name, out expiry)) {
                return (expiry > DateTime.Now);
            }
            return false;
        }

        public static bool Contains(int entry) {
            DateTime expiry;
            if(EntryBlacklist.TryGetValue(entry, out expiry)) {
                return (expiry > DateTime.Now);
            }
            return false;
        }

        public static void RemoveExpired() {
            if(!_sweepTimer.IsFinished) {
                return;
            }

            var now = DateTime.Now;
            var expiredGuids = (from key in GuidBlackList.Keys where (GuidBlackList[key] < now) select key).ToList();
            foreach(var guid in expiredGuids) {
                GuidBlackList.Remove(guid);
            }

            var expiredEntries = (from key in EntryBlacklist.Keys where (EntryBlacklist[key] < now) select key).ToList();
            foreach(var entry in expiredEntries) {
                EntryBlacklist.Remove(entry);
            }

            var expiredNames = (from key in NameBlacklist.Keys where (NameBlacklist[key] < now) select key).ToList();
            foreach(var name in expiredNames) {
                NameBlacklist.Remove(name);
            }

            _sweepTimer.Reset();
        }

        // GUID
        public static void RemoveAllGuid() {
            var everything = (from key in GuidBlackList.Keys select key).ToList();
            foreach(var guid in everything) {
                GuidBlackList.Remove(guid);
            }
        }

        // Entry
        public static void RemoveAllEntries() {
            var everything = (from key in EntryBlacklist.Keys select key).ToList();
            foreach(var entry in everything) {
                EntryBlacklist.Remove(entry);
            }
        }

        // Name
        public static void RemoveAllNames() {
            var everything = (from key in NameBlacklist.Keys select key).ToList();

            foreach(var name in everything) {
                NameBlacklist.Remove(name);
            }
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================
    }
}