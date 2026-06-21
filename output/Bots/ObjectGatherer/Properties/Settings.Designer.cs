using System;
using System.IO;
using System.Xml.Serialization;
using ObjectGatherer.Helpers;

namespace ObjectGatherer.Properties {
    /// <summary>
    /// Simple settings class replacing ApplicationSettingsBase (not available in .NET 10 runtime compilation).
    /// Persists window settings to an XML file.
    /// </summary>
    [Serializable]
    public sealed class Settings {
        private static readonly string SettingsPath = Path.Combine(
            AppContext.BaseDirectory, "Settings", "ObjectGatherer", "WindowSettings.xml");

        private static Settings _defaultInstance;

        public static Settings Default {
            get {
                if (_defaultInstance == null) {
                    Load();
                }
                return _defaultInstance;
            }
        }

        public WindowSettings CustomWindowSettings { get; set; }

        public Settings() { }

        private static void Load() {
            try {
                if (File.Exists(SettingsPath)) {
                    using (var reader = new StreamReader(SettingsPath)) {
                        var serializer = new XmlSerializer(typeof(Settings));
                        _defaultInstance = serializer.Deserialize(reader) as Settings ?? new Settings();
                    }
                } else {
                    _defaultInstance = new Settings();
                }
            } catch {
                _defaultInstance = new Settings();
            }
        }

        public void Save() {
            try {
                var dir = Path.GetDirectoryName(SettingsPath);
                if (dir != null && !Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }
                using (var writer = new StreamWriter(SettingsPath)) {
                    var serializer = new XmlSerializer(typeof(Settings));
                    serializer.Serialize(writer, this);
                }
            } catch {
                // Settings save failure is non-critical
            }
        }
    }
}
