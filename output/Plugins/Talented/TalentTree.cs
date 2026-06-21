using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;

namespace Talented
{
    public class TalentTree
    {
        public TalentTree(int specialization, List<TalentPlacement> talents, WoWClass @class, string name)
        {
            Specialization = specialization;
            TalentPlacements = talents;
            Class = @class;
            BuildName = name;
        }

        /// <summary> The name of this build. </summary>
        public string BuildName { get; private set; }

        /// <summary> The class this build is intended to be used on. </summary>
        public WoWClass Class { get; private set; }

        /// <summary> The index of the spcialization to choose. </summary>
        public int Specialization { get; private set; }

        /// <summary> List of wanted talents. </summary>
        public List<TalentPlacement> TalentPlacements { get; private set; }

        public static TalentTree FromXml(XElement root)
        {
            if (root == null)
                throw new Exception("Element passed in to TalenTree.FromXml can't be null!");

            XAttribute specializationAttribute = root.Attribute("Specialization");
            if (specializationAttribute == null)
                throw new Exception("TalenTree element needs a Spcialization attribute!: " + root);

            string buildName = "";
            XAttribute nameAttribute = root.Attribute("Name");
            if (nameAttribute != null)
                buildName = nameAttribute.Value;

            WoWClass @wowClass = WoWClass.None;
            XAttribute wowClassAttribute = root.Attribute("Class");
            if (wowClassAttribute != null)
            {
                try
                {
                    @wowClass = (WoWClass)Enum.Parse(typeof(WoWClass), wowClassAttribute.Value);
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                }
            }

            int specializationIndex;
            if (!int.TryParse(specializationAttribute.Value, out specializationIndex))
                throw new Exception("Could not parse integer in 'Specialization' attribute in TalentTree element.");

            var talentPlacements = new List<TalentPlacement>();
            var talents = root.Elements("Talent");

            foreach (XElement element in talents)
            {
                int tab = 0, index = 0, count = 0;
                string name = "";

                foreach (XAttribute attribute in element.Attributes())
                {
                    switch (attribute.Name.ToString().ToLower())
                    {
                        case "tab":
                            if (!int.TryParse(attribute.Value, out tab))
                                throw new Exception("Could not parse integer in 'Tab' attribute in Talent element: " + element);
                            break;

                        case "index":
                            if (!int.TryParse(attribute.Value, out index))
                                throw new Exception("Could not parse integer in 'Index' attribute in Talent element: " + element);
                            break;

                        case "count":
                            if (!int.TryParse(attribute.Value, out count))
                                throw new Exception("Could not parse integer in 'Count' attribute in Talent element: " + element);
                            break;

                        case "name":
                            name = attribute.Value;
                            break;
                    }
                }

                talentPlacements.Add(new TalentPlacement(tab, index, count, name));
            }

            return new TalentTree(specializationIndex, talentPlacements, @wowClass, buildName);
        }

        public override string ToString()
        {
            return BuildName;
        }
    }
}
