using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Bots.BGBuddy.Helpers;
using Bots.BGBuddy.Resources;
using Styx.Logic.Profiles;
using Tripper.Tools.Math;

namespace Bots.BGBuddy
{
    /// <summary>
    /// Represents a BGBuddy profile loaded from XML.
    /// Contains named hotspot boxes per BattlegroundSide, start locations, and blackspots.
    /// </summary>
    public class BgBotProfile
    {
        [Conditional("DEBUG")]
        public static void Test()
        {
            // Intentionally left empty — debug-only profile load test
        }

        #region Properties

        public string Name { get; set; }

        public List<Blackspot> Blackspots { get; set; }

        public Dictionary<BattlegroundSide, List<MapBox>> Boxes { get; set; }

        public Dictionary<BattlegroundSide, Vector3> StartLocations { get; set; }

        #endregion

        public BgBotProfile()
        {
            Blackspots = new List<Blackspot>();
            Boxes = new Dictionary<BattlegroundSide, List<MapBox>>();
        }

        /// <summary>
        /// Loads a BgBotProfile from an XML file on disk.
        /// </summary>
        public static BgBotProfile Load(string filePath)
        {
            var profile = new BgBotProfile();

            if (!File.Exists(filePath))
                return profile;

            var settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true
            };

            using (var reader = XmlReader.Create(File.OpenRead(filePath), settings))
            {
                var root = XElement.Load(reader);

                profile.Name = root.Element("Name").Value;

                // Parse StartLocations
                profile.StartLocations = root.Elements("StartLocation")
                    .Select(elem => new
                    {
                        Location = new Vector3(
                            float.Parse(elem.Attribute("X").Value, CultureInfo.InvariantCulture),
                            float.Parse(elem.Attribute("Y").Value, CultureInfo.InvariantCulture),
                            float.Parse(elem.Attribute("Z").Value, CultureInfo.InvariantCulture)),
                        Side = (BattlegroundSide)Enum.Parse(typeof(BattlegroundSide), elem.Attribute("Side").Value, true)
                    })
                    .ToDictionary(item => item.Side, item => item.Location);

                // Parse Blackspots
                if (root.Element("Blackspots") != null)
                {
                    profile.Blackspots = root.Element("Blackspots")
                        .Elements()
                        .Select(elem => new Blackspot(elem))
                        .ToList();
                }

                // Parse Boxes
                profile.Boxes = ParseBoxes(root);
            }

            return profile;
        }

        /// <summary>
        /// XML validation error handler for profile loading.
        /// </summary>
        private static void OnValidationError(object sender, ValidationEventArgs e)
        {
            Logger.Write(BGBuddyResources.BgBotProfile_settings_ValidationEventHandler_Error_loading_BGBuddy_profile____0___ + e.Message);
        }

        /// <summary>
        /// Parses all Boxes elements from the profile XML into a dictionary keyed by BattlegroundSide.
        /// </summary>
        private static Dictionary<BattlegroundSide, List<MapBox>> ParseBoxes(XElement root)
        {
            var boxes = new Dictionary<BattlegroundSide, List<MapBox>>();

            foreach (var boxesElem in root.Elements("Boxes"))
            {
                var side = (BattlegroundSide)Enum.Parse(typeof(BattlegroundSide), boxesElem.Attribute("Side").Value, true);

                if (!boxes.ContainsKey(side))
                    boxes.Add(side, new List<MapBox>());

                foreach (var boxElem in boxesElem.Elements("Box"))
                {
                    boxes[side].Add(MapBox.FromXElement(boxElem));
                }
            }

            return boxes;
        }
    }
}
