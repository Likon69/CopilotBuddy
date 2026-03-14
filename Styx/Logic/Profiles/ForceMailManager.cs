using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Styx.Helpers;

namespace Styx.Logic.Profiles
{
    public class ForceMailManager
    {
        private static readonly DualHashSet<uint, string> _persistentItems = new DualHashSet<uint, string>();
        private static readonly DualHashSet<uint, string> _sessionItems = new DualHashSet<uint, string>();
        private static readonly HashSet<string> _xmlFileNames = new HashSet<string> { "forcemail", "forcedmail", "force mail", "forced mail" };

        static ForceMailManager()
        {
            ReloadProtectedItems();
        }

        public static void ReloadProtectedItems()
        {
            _persistentItems.HashSet1.Clear();
            _persistentItems.HashSet2.Clear();
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (string file in Directory.GetFiles(dir, "*.xml", SearchOption.AllDirectories))
            {
                if (_xmlFileNames.Contains(Path.GetFileNameWithoutExtension(file).ToLowerInvariant()))
                    LoadFile(file);
            }
        }

        private static void LoadFile(string path)
        {
            if (!Path.GetExtension(path).Equals(".xml", StringComparison.OrdinalIgnoreCase) || !File.Exists(path))
                return;

            XElement root;
            try
            {
                root = XElement.Load(path);
            }
            catch (XmlException ex)
            {
                Logging.Write("Error in ForceMail XML file at {0}: {1}", path, ex.Message);
                return;
            }

            foreach (XElement item in root.Elements())
            {
                if (!item.Name.ToString().Equals("item", StringComparison.OrdinalIgnoreCase))
                    continue;

                uint id = 0;
                string name = "";
                foreach (XAttribute attr in item.Attributes())
                {
                    string key = attr.Name.ToString().ToLowerInvariant();
                    if (key == "id" || key == "entry")
                        uint.TryParse(attr.Value, out id);
                    else if (key == "name")
                        name = attr.Value.ToLowerInvariant();
                }
                if (id == 0 && !string.IsNullOrEmpty(item.Value))
                    uint.TryParse(item.Value, out id);

                if (id == 0 && string.IsNullOrEmpty(name))
                    continue;

                if (id != 0 && !_persistentItems.Contains(id))
                    _persistentItems.Add(id);
                if (!string.IsNullOrEmpty(name) && !_persistentItems.Contains(name))
                    _persistentItems.Add(name);
            }
        }

        public static bool Contains(uint item)
        {
            return _persistentItems.Contains(item)
                || _sessionItems.Contains(item)
                || (ProfileManager.CurrentProfile?.ForceMail.Contains(item) ?? false);
        }

        public static bool Contains(string item)
        {
            item = item.ToLowerInvariant();
            return _persistentItems.Contains(item)
                || _sessionItems.Contains(item)
                || (ProfileManager.CurrentProfile?.ForceMail.Contains(item) ?? false);
        }

        public static bool Add(uint item) => _sessionItems.Add(item);
        public static bool Add(string item) => _sessionItems.Add(item.ToLowerInvariant());
        public static bool Remove(uint item) => _sessionItems.Remove(item);
        public static bool Remove(string item) => _sessionItems.Remove(item.ToLowerInvariant());

        public static List<string> GetAllItemNames()
        {
            var list = _persistentItems.HashSet2.ToList();
            list.AddRange(_sessionItems.HashSet2);
            if (ProfileManager.CurrentProfile != null)
                list.AddRange(ProfileManager.CurrentProfile.ForceMail.HashSet2);
            return list;
        }

        public static List<uint> GetAllItemIds()
        {
            var list = _persistentItems.HashSet1.ToList();
            list.AddRange(_sessionItems.HashSet1);
            if (ProfileManager.CurrentProfile != null)
                list.AddRange(ProfileManager.CurrentProfile.ForceMail.HashSet1);
            return list;
        }
    }
}
