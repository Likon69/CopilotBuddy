#nullable disable
using System;
using System.Xml.Linq;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles.Quest;
using Styx.WoWInternals.WoWObjects;

namespace Styx.Logic.Profiles
{
    /// <summary>
    /// Represents a vendor NPC in a profile.
    /// </summary>
    public class Vendor : IEquatable<Vendor>, IXmlObject
    {
        /// <summary>
        /// Vendor type enumeration.
        /// </summary>
        public enum VendorType
        {
            Unknown,
            Repair,
            Food,
            Sell,
            Restock,
            Train,
            FlightMaster,
            InnKeeper,
            Ammo
        }

        /// <summary>
        /// Creates a vendor from parameters.
        /// </summary>
        public Vendor(int entry, string name, VendorType type, WoWPoint location)
        {
            Name = name;
            Entry = entry;
            Type = type;
            Location = location;
        }

        /// <summary>
        /// Creates a vendor from XML.
        /// </summary>
        public Vendor(XElement xml)
        {
            Element = xml;
            Location = ProfileHelper.ParseLocation(xml);
            Type = VendorType.Unknown;
            Name = string.Empty;
            TrainClass = WoWClass.None;

            foreach (XAttribute attribute in xml.Attributes())
            {
                try
                {
                    switch (attribute.Name.ToString().ToLowerInvariant())
                    {
                        case "name":
                            Name = attribute.Value;
                            continue;
                        case "id":
                        case "entry":
                            if (!int.TryParse(attribute.Value, out int result1))
                                throw new ProfileAttributeExpectedException<int>(attribute, new string[0]);
                            Entry = result1;
                            continue;
                        case "id2":
                        case "entry2":
                            if (!int.TryParse(attribute.Value, out int result2))
                                throw new ProfileAttributeExpectedException<int>(attribute, new string[0]);
                            Entry2 = result2;
                            continue;
                        case "type":
                            try
                            {
                                Type = (VendorType)Enum.Parse(typeof(VendorType), attribute.Value);
                                continue;
                            }
                            catch (ArgumentException)
                            {
                                throw new ProfileAttributeExpectedException<VendorType>(attribute, new string[6]
                                {
                                    "Repair",
                                    "Food",
                                    "Sell",
                                    "Restock",
                                    "Train",
                                    "Ammo"
                                });
                            }
                        case "trainclass":
                            try
                            {
                                TrainClass = (WoWClass)Enum.Parse(typeof(WoWClass), attribute.Value);
                                continue;
                            }
                            catch (ArgumentException)
                            {
                                throw new ProfileAttributeExpectedException<VendorType>(attribute, new string[10]
                                {
                                    "Deathknight",
                                    "Druid",
                                    "Hunter",
                                    "Mage",
                                    "Paladin",
                                    "Priest",
                                    "Rogue",
                                    "Shaman",
                                    "Warlock",
                                    "Warrior"
                                });
                            }
                        case "usablewhen":
                            if (!string.IsNullOrWhiteSpace(attribute.Value))
                                UsableWhen = DelayCompiledExpression.Condition(attribute.Value);
                            continue;
                        case "x":
                        case "y":
                        case "z":
                            continue;
                        default:
                            throw new ProfileUnknownAttributeException(attribute, new string[8]
                            {
                                nameof(Name),
                                nameof(Entry),
                                nameof(Type),
                                "X",
                                "Y",
                                "Z",
                                nameof(TrainClass),
                                nameof(UsableWhen)
                            });
                    }
                }
                catch (ProfileException ex)
                {
                    Logging.WriteException(ex);
                }
            }
        }

        /// <summary>
        /// Creates a vendor from a WoW object.
        /// </summary>
        public Vendor(WoWObject unit, VendorType type)
        {
            Name = unit.Name;
            Entry = (int)unit.Entry;
            Type = type;
            Location = unit.Location;
        }

        /// <summary>
        /// Gets the NPC name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the NPC entry ID.
        /// </summary>
        public int Entry { get; private set; }

        /// <summary>
        /// Gets the secondary NPC entry ID.
        /// </summary>
        public int? Entry2 { get; private set; }

        /// <summary>
        /// Gets the vendor type.
        /// </summary>
        public VendorType Type { get; private set; }

        /// <summary>
        /// Gets or sets the vendor location.
        /// </summary>
        public WoWPoint Location { get; set; }

        /// <summary>
        /// Gets or sets the class this trainer trains (if applicable).
        /// </summary>
        public WoWClass TrainClass { get; set; }

        /// <summary>
        /// The XML element this vendor was parsed from, null when it was built from a WoWObject.
        /// </summary>
        public XElement Element { get; private set; }

        /// <summary>
        /// Optional condition guarding the vendor, compiled with the rest of the profile.
        /// HB 6.2.3 Vendor.UsableWhen; VendorManager skips a vendor whose condition is false.
        /// </summary>
        [CompileExpression]
        public DelayCompiledExpression<Func<bool>> UsableWhen { get; private set; }

        /// <summary>
        /// Determines whether this vendor equals another.
        /// </summary>
        public bool Equals(Vendor other)
        {
            return !ReferenceEquals(other, null) &&
                   Entry == other.Entry &&
                   Location == other.Location &&
                   Name == other.Name &&
                   Type == other.Type;
        }

        public override bool Equals(object obj) => Equals(obj as Vendor);

        public override int GetHashCode()
        {
            return HashCode.Combine(Entry, Location, Name, Type);
        }

        public override string ToString()
        {
            return $"[Vendor Name: {Name}, Entry: {Entry}, Type: {Type}, Location: {Location}, TrainClass: {TrainClass}]";
        }
    }
}
