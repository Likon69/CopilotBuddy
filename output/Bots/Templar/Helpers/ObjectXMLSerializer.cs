// For serialization of an object to an XML Document file.
// For reading/writing data to an XML file.

using System;
using System.IO;
using System.Xml.Serialization;

namespace Templar.Helpers {
    /// <summary>
    /// Serialization format types.
    /// </summary>
    public enum SerializedFormat {
        /// <summary>
        /// Binary serialization format.
        /// </summary>
        Binary,

        /// <summary>
        /// Document serialization format.
        /// </summary>
        Document
    }
     
    /// <summary>
    /// Facade to XML serialization and deserialization of strongly typed objects to/from an XML file.
    /// 
    /// References: XML Serialization at http://samples.gotdotnet.com/:
    /// http://samples.gotdotnet.com/QuickStart/howto/default.aspx?url=/quickstart/howto/doc/xmlserialization/rwobjfromxml.aspx
    /// </summary>
    public static class ObjectXMLSerializer<T> where T : class // Specify that T must be a class.
    {
        #region Load methods

        /// <summary>
        /// Loads an object from an XML file in Document format.
        /// </summary>
        /// <example>
        /// <code>
        /// serializableObject = ObjectXMLSerializer&lt;SerializableObject&gt;.Load(@"C:\XMLObjects.xml");
        /// </code>
        /// </example>
        /// <param name="path">Path of the file to load the object from.</param>
        /// <returns>Object loaded from an XML file in Document format.</returns>
        public static T Load(string path) {
            T serializableObject = LoadFromDocumentFormat(null, path);
            return serializableObject;
        }

        /// <summary>
        /// Loads an object from an XML file using a specified serialized format.
        /// </summary>
        /// <example>
        /// <code>
        /// serializableObject = ObjectXMLSerializer&lt;SerializableObject&gt;.Load(@"C:\XMLObjects.xml", SerializedFormat.Binary);
        /// </code>
        /// </example>        
        /// <param name="path">Path of the file to load the object from.</param>
        /// <param name="serializedFormat">XML serialized format used to load the object.</param>
        /// <returns>Object loaded from an XML file using the specified serialized format.</returns>
        public static T Load(string path, SerializedFormat serializedFormat) {
            // SerializedFormat.Binary is no longer supported — fall through to Document.
            return LoadFromDocumentFormat(null, path);
        }

        /// <summary>
        /// Loads an object from an XML file in Document format, supplying extra data types to enable deserialization of custom types within the object.
        /// </summary>
        /// <example>
        /// <code>
        /// serializableObject = ObjectXMLSerializer&lt;SerializableObject&gt;.Load(@"C:\XMLObjects.xml", new Type[] { typeof(MyCustomType) });
        /// </code>
        /// </example>
        /// <param name="path">Path of the file to load the object from.</param>
        /// <param name="extraTypes">Extra data types to enable deserialization of custom types within the object.</param>
        /// <returns>Object loaded from an XML file in Document format.</returns>
        public static T Load(string path, Type[] extraTypes) {
            T serializableObject = LoadFromDocumentFormat(extraTypes, path);
            return serializableObject;
        }

        #endregion

        #region Save methods

        public static void Save(T serializableObject, string path) {
            SaveToDocumentFormat(serializableObject, null, path);
        }

        public static void Save(T serializableObject, string path, SerializedFormat serializedFormat) {
            SaveToDocumentFormat(serializableObject, null, path);
        }

        public static void Save(T serializableObject, string path, Type[] extraTypes) {
            SaveToDocumentFormat(serializableObject, extraTypes, path);
        }

        #endregion

        #region Private

        private static T LoadFromDocumentFormat(Type[] extraTypes, string path) {
            T serializableObject;

            using(TextReader textReader = new StreamReader(path)) {
                XmlSerializer xmlSerializer = CreateXmlSerializer(extraTypes);
                serializableObject = xmlSerializer.Deserialize(textReader) as T;
            }

            return serializableObject;
        }

        private static XmlSerializer CreateXmlSerializer(Type[] extraTypes) {
            Type objectType = typeof(T);

            XmlSerializer xmlSerializer = extraTypes != null ? new XmlSerializer(objectType, extraTypes) : new XmlSerializer(objectType);

            return xmlSerializer;
        }

        private static void SaveToDocumentFormat(T serializableObject, Type[] extraTypes, string path) {
            using(TextWriter textWriter = new StreamWriter(path)) {
                XmlSerializer xmlSerializer = CreateXmlSerializer(extraTypes);
                xmlSerializer.Serialize(textWriter, serializableObject);
            }
        }

        #endregion
    }
}

