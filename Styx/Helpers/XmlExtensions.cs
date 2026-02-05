using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace Styx.Helpers
{
    /// <summary>
    /// Extension methods for XML processing.
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// Logs an error message with line info for the XML element.
        /// </summary>
        public static void Error(this XObject actualElement, string errorMessage)
        {
            Logging.WriteDebug(actualElement.GetError(errorMessage));
        }

        /// <summary>
        /// Gets an error message string with line info for the XML element.
        /// </summary>
        public static string GetError(this XObject actualElement, string errorMessage)
        {
            IXmlLineInfo lineInfo = actualElement as IXmlLineInfo;
            if (lineInfo != null && lineInfo.HasLineInfo())
            {
                return $"XML Error: {errorMessage} - On line {lineInfo.LineNumber} - [{actualElement}]";
            }
            return $"XML Error: {errorMessage} - [{actualElement}]";
        }

        /// <summary>
        /// Gets an attribute value as a string, or a default value if not found.
        /// </summary>
        public static string GetAttributeValue(this XElement element, string attributeName, string defaultValue = null)
        {
            return element.Attribute(attributeName)?.Value ?? defaultValue;
        }

        /// <summary>
        /// Gets an attribute value as an integer.
        /// </summary>
        public static int GetAttributeInt(this XElement element, string attributeName, int defaultValue = 0)
        {
            var attr = element.Attribute(attributeName);
            if (attr == null)
                return defaultValue;
            if (int.TryParse(attr.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// Gets an attribute value as a float.
        /// </summary>
        public static float GetAttributeFloat(this XElement element, string attributeName, float defaultValue = 0f)
        {
            var attr = element.Attribute(attributeName);
            if (attr == null)
                return defaultValue;
            if (float.TryParse(attr.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// Gets an attribute value as a double.
        /// </summary>
        public static double GetAttributeDouble(this XElement element, string attributeName, double defaultValue = 0.0)
        {
            var attr = element.Attribute(attributeName);
            if (attr == null)
                return defaultValue;
            if (double.TryParse(attr.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// Gets an attribute value as a boolean.
        /// </summary>
        public static bool GetAttributeBool(this XElement element, string attributeName, bool defaultValue = false)
        {
            var attr = element.Attribute(attributeName);
            if (attr == null)
                return defaultValue;

            string value = attr.Value.ToLowerInvariant();
            return value == "true" || value == "1" || value == "yes";
        }

        /// <summary>
        /// Gets an attribute value as a uint.
        /// </summary>
        public static uint GetAttributeUInt(this XElement element, string attributeName, uint defaultValue = 0)
        {
            var attr = element.Attribute(attributeName);
            if (attr == null)
                return defaultValue;
            if (uint.TryParse(attr.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// Gets an attribute value as an enum.
        /// </summary>
        public static T GetAttributeEnum<T>(this XElement element, string attributeName, T defaultValue = default) where T : struct, Enum
        {
            var attr = element.Attribute(attributeName);
            if (attr == null)
                return defaultValue;
            if (Enum.TryParse<T>(attr.Value, true, out T result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// Checks if an element has a specific attribute.
        /// </summary>
        public static bool HasAttribute(this XElement element, string attributeName)
        {
            return element.Attribute(attributeName) != null;
        }

        /// <summary>
        /// Gets a child element value as a string.
        /// </summary>
        public static string GetElementValue(this XElement element, string childName, string defaultValue = null)
        {
            return element.Element(childName)?.Value ?? defaultValue;
        }

        /// <summary>
        /// Gets a child element value as an integer.
        /// </summary>
        public static int GetElementInt(this XElement element, string childName, int defaultValue = 0)
        {
            var child = element.Element(childName);
            if (child == null)
                return defaultValue;
            if (int.TryParse(child.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// Logs a warning for an unexpected attribute.
        /// </summary>
        public static void WarnUnexpectedAttribute(this XElement element, XAttribute attr)
        {
            IXmlLineInfo lineInfo = attr as IXmlLineInfo;
            string location = lineInfo?.HasLineInfo() == true ? $" (line {lineInfo.LineNumber})" : "";
            Logging.WriteDebug($"Unexpected attribute '{attr.Name}' in element '{element.Name}'{location}");
        }

        /// <summary>
        /// Logs a warning for an unexpected element.
        /// </summary>
        public static void WarnUnexpectedElement(this XElement parent, XElement child)
        {
            IXmlLineInfo lineInfo = child as IXmlLineInfo;
            string location = lineInfo?.HasLineInfo() == true ? $" (line {lineInfo.LineNumber})" : "";
            Logging.WriteDebug($"Unexpected element '{child.Name}' in '{parent.Name}'{location}");
        }
    }
}
