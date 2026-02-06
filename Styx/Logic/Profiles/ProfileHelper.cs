#nullable disable
using System;
using System.Globalization;
using System.Xml.Linq;
using Styx.Logic.Pathing;

namespace Styx.Logic.Profiles
{
    /// <summary>
    /// Helper methods for profile parsing.
    /// </summary>
    internal static class ProfileHelper
    {
        /// <summary>
        /// Parses a float value that may use comma OR dot as decimal separator.
        /// Handles French locale (comma) and international format (dot).
        /// </summary>
        private static float ParseFloatLocaleAgnostic(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0f;

            // First try with InvariantCulture (dot as decimal separator)
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
                return result;

            // If that fails, try replacing comma with dot (French locale)
            string normalized = value.Replace(',', '.');
            if (float.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                return result;

            return 0f;
        }

        /// <summary>
        /// Parses a WoWPoint from an XML element with X, Y, Z attributes.
        /// Handles both dot and comma as decimal separators.
        /// </summary>
        public static WoWPoint ParseLocation(XElement element)
        {
            float x = 0, y = 0, z = 0;

            foreach (var attr in element.Attributes())
            {
                switch (attr.Name.LocalName.ToLowerInvariant())
                {
                    case "x":
                        x = ParseFloatLocaleAgnostic(attr.Value);
                        break;
                    case "y":
                        y = ParseFloatLocaleAgnostic(attr.Value);
                        break;
                    case "z":
                        z = ParseFloatLocaleAgnostic(attr.Value);
                        break;
                }
            }

            return new WoWPoint(x, y, z);
        }

        /// <summary>
        /// Parses a boolean attribute value.
        /// </summary>
        public static bool ParseBool(XAttribute attr, bool defaultValue = false)
        {
            if (attr == null) return defaultValue;
            
            string value = attr.Value.ToLowerInvariant();
            return value == "true" || value == "1" || value == "yes";
        }

        /// <summary>
        /// Parses an integer attribute value.
        /// </summary>
        public static int ParseInt(XAttribute attr, int defaultValue = 0)
        {
            if (attr == null) return defaultValue;
            return int.TryParse(attr.Value, out int result) ? result : defaultValue;
        }

        /// <summary>
        /// Parses a float attribute value.
        /// Handles both dot and comma as decimal separators.
        /// </summary>
        public static float ParseFloat(XAttribute attr, float defaultValue = 0f)
        {
            if (attr == null) return defaultValue;
            return ParseFloatLocaleAgnostic(attr.Value);
        }
    }
}
