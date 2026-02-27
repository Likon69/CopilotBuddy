#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Styx.WoWInternals.WoWObjects;

namespace Styx.Logic
{
    public static class Blacklist
    {
        private static readonly Dictionary<ulong, DateTime> _blacklistedGuids;
        private static Func<KeyValuePair<ulong, DateTime>, ulong> _keySelector;

        static Blacklist()
        {
            Blacklist._blacklistedGuids = new Dictionary<ulong, DateTime>();
        }

        public static void Flush()
        {
            if (Blacklist._blacklistedGuids.Count != 0)
            {
                DateTime now = DateTime.Now;
                IEnumerable<KeyValuePair<ulong, DateTime>> source = Blacklist._blacklistedGuids.Where(kvp => kvp.Value < now);
                if (Blacklist._keySelector == null)
                {
                    Blacklist._keySelector = new Func<KeyValuePair<ulong, DateTime>, ulong>(GetKey);
                }
                List<ulong> list = source.Select(Blacklist._keySelector).ToList<ulong>();
                foreach (ulong expiredGuid in list)
                {
                    Blacklist._blacklistedGuids.Remove(expiredGuid);
                }
            }
        }

        public static bool Contains(WoWObject o)
        {
            return Blacklist.Contains(o.Guid);
        }

        public static bool Contains(ulong guid)
        {
            return Blacklist.Contains(guid, true);
        }

        public static bool Contains(ulong guid, bool flush)
        {
            if (flush)
            {
                Blacklist.Flush();
            }
            if (Blacklist._blacklistedGuids.ContainsKey(guid))
            {
                return (DateTime.Now <= Blacklist._blacklistedGuids[guid]);
            }
            return false;
        }

        public static void Add(ulong guid, TimeSpan end)
        {
            if (Blacklist._blacklistedGuids.ContainsKey(guid))
            {
                Dictionary<ulong, DateTime> dictionary = Blacklist._blacklistedGuids;
                DateTime now = DateTime.Now;
                dictionary[guid] = now.Add(end);
            }
            else
            {
                Dictionary<ulong, DateTime> dictionary2 = Blacklist._blacklistedGuids;
                DateTime now2 = DateTime.Now;
                dictionary2.Add(guid, now2.Add(end));
            }
        }

        public static void Add(WoWObject o, TimeSpan end)
        {
            Blacklist.Add(o.Guid, end);
        }

        private static ulong GetKey(KeyValuePair<ulong, DateTime> kvp)
        {
            return kvp.Key;
        }
    }
}
