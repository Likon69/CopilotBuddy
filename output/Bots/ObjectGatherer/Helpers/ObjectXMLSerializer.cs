using System;
using System.IO;
using System.Xml.Serialization;

namespace ObjectGatherer.Helpers {
    /// <summary>
    /// XML serialization and deserialization of strongly typed objects to/from an XML file.
    /// </summary>
    public static class ObjectXMLSerializer<T> where T : class {
        /// <summary>
        /// Loads an object from an XML file in Document format.
        /// </summary>
        public static T Load(string path) {
            using (var reader = new StreamReader(path)) {
                var xmlSerializer = new XmlSerializer(typeof(T));
                return xmlSerializer.Deserialize(reader) as T;
            }
        }

        /// <summary>
        /// Loads an object from an XML file, supplying extra data types for custom type deserialization.
        /// </summary>
        public static T Load(string path, Type[] extraTypes) {
            using (var reader = new StreamReader(path)) {
                var xmlSerializer = extraTypes != null
                    ? new XmlSerializer(typeof(T), extraTypes)
                    : new XmlSerializer(typeof(T));
                return xmlSerializer.Deserialize(reader) as T;
            }
        }

        /// <summary>
        /// Saves an object to an XML file in Document format.
        /// </summary>
        public static void Save(T serializableObject, string path) {
            using (var writer = new StreamWriter(path)) {
                var xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(writer, serializableObject);
            }
        }

        /// <summary>
        /// Saves an object to an XML file, supplying extra data types for custom type serialization.
        /// </summary>
        public static void Save(T serializableObject, string path, Type[] extraTypes) {
            using (var writer = new StreamWriter(path)) {
                var xmlSerializer = extraTypes != null
                    ? new XmlSerializer(typeof(T), extraTypes)
                    : new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(writer, serializableObject);
            }
        }
    }
}
