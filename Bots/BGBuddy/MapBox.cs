using System;
using System.Globalization;
using System.Xml.Linq;
using Tripper.Tools.Math;

namespace Bots.BGBuddy
{
    /// <summary>
    /// Represents a named rectangular area on a battleground map, defined by TopLeft/BottomRight corners and a Center point.
    /// Used by BgBotProfile for hotspot boxes.
    /// </summary>
    public struct MapBox
    {
        public Vector3 TopLeft;
        public Vector3 BottomRight;

        public string Name { get; set; }

        public Vector3 Center { get; set; }

        /// <summary>
        /// Parses a MapBox from an XML element containing TopLeft, BottomRight, Center sub-elements and a Name attribute.
        /// </summary>
        public static MapBox FromXElement(XElement element)
        {
            var box = new MapBox();

            XElement topLeftElem = element.Element("TopLeft");
            XElement bottomRightElem = element.Element("BottomRight");
            XElement centerElem = element.Element("Center");

            box.TopLeft = new Vector3(
                Convert.ToSingle(topLeftElem.Attribute("X").Value, CultureInfo.InvariantCulture),
                Convert.ToSingle(topLeftElem.Attribute("Y").Value, CultureInfo.InvariantCulture),
                Convert.ToSingle(topLeftElem.Attribute("Z").Value, CultureInfo.InvariantCulture));

            box.BottomRight = new Vector3(
                Convert.ToSingle(bottomRightElem.Attribute("X").Value, CultureInfo.InvariantCulture),
                Convert.ToSingle(bottomRightElem.Attribute("Y").Value, CultureInfo.InvariantCulture),
                Convert.ToSingle(bottomRightElem.Attribute("Z").Value, CultureInfo.InvariantCulture));

            box.Center = new Vector3(
                Convert.ToSingle(centerElem.Attribute("X").Value, CultureInfo.InvariantCulture),
                Convert.ToSingle(centerElem.Attribute("Y").Value, CultureInfo.InvariantCulture),
                Convert.ToSingle(centerElem.Attribute("Z").Value, CultureInfo.InvariantCulture));

            box.Name = element.Attribute("Name").Value;

            return box;
        }
    }
}
