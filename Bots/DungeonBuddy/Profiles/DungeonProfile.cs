using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;

namespace Bots.DungeonBuddy.Profiles
{
    /// <summary>
    /// Donjon chargé depuis Default Profiles\DungeonBuddy\*.xml
    /// Format XML: <DungeonBuddyProfile><DungeonId>202</DungeonId><HotSpots>...</HotSpots><Blackspots>...</Blackspots></DungeonBuddyProfile>
    /// Hotspot attrs: x, y, z
    /// Blackspot attrs: x, y, z, radius, height
    /// </summary>
    public class DungeonProfile
    {
        public string Name { get; private set; } = "Unknown";
        public uint DungeonId { get; private set; }
        public List<WoWPoint> HotSpots { get; } = new();
        public List<Blackspot> Blackspots { get; } = new();

        private DungeonProfile() { }

        public static DungeonProfile Load(XElement root)
        {
            var profile = new DungeonProfile();
            profile.Name = root.Element("Name")?.Value ?? "Unknown";

            if (uint.TryParse(root.Element("DungeonId")?.Value, out uint id))
                profile.DungeonId = id;

            // <HotSpots><Hotspot x="..." y="..." z="..." /></HotSpots>
            var hotspotsEl = root.Element("HotSpots") ?? root.Element("Hotspots");
            if (hotspotsEl != null)
            {
                foreach (var el in hotspotsEl.Elements("Hotspot"))
                {
                    float x = ParseAttr(el, "x", "X");
                    float y = ParseAttr(el, "y", "Y");
                    float z = ParseAttr(el, "z", "Z");
                    profile.HotSpots.Add(new WoWPoint(x, y, z));
                }
            }

            // <Blackspots><Blackspot x="..." y="..." z="..." radius="..." height="..." /></Blackspots>
            var blackspotsEl = root.Element("Blackspots");
            if (blackspotsEl != null)
            {
                foreach (var el in blackspotsEl.Elements("Blackspot"))
                {
                    float x = ParseAttr(el, "x", "X");
                    float y = ParseAttr(el, "y", "Y");
                    float z = ParseAttr(el, "z", "Z");
                    float radius = ParseAttr(el, "radius", "Radius");
                    float height = ParseAttr(el, "height", "Height");
                    profile.Blackspots.Add(new Blackspot(new WoWPoint(x, y, z), radius, height));
                }
            }

            return profile;
        }

        private static float ParseAttr(XElement el, string lower, string upper)
        {
            var attr = el.Attribute(lower) ?? el.Attribute(upper);
            if (attr == null) return 0f;
            return float.TryParse(attr.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float val) ? val : 0f;
        }
    }
}
