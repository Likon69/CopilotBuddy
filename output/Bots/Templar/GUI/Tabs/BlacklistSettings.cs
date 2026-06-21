using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using Styx;
using Styx.Helpers;
using Templar.Helpers;

namespace Templar.GUI.Tabs {
    public class BlacklistEntry : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute]
        public uint Entry { get; set; }
        [XmlAttribute]
        public string Name { get; set; }

        protected virtual void OnPropertyChanged(string propertyName) {
            var handler = PropertyChanged;
            if(handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class BlacklistSettings {
        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        public static BlacklistSettings Instance = new BlacklistSettings();

        public BindingList<BlacklistEntry> BlacklistedUnits = new BindingList<BlacklistEntry>();

        // ===========================================================
        // Constructors
        // ===========================================================

        static BlacklistSettings() {
            var folderPath = Path.GetDirectoryName(SettingsFilePath);

            if(folderPath != null && !Directory.Exists(folderPath)) {
                Directory.CreateDirectory(folderPath);
            }

            Load();
        }
        // ===========================================================
        // Getter & Setter
        // ===========================================================

        public static string SettingsFilePath {
            get {
                return Path.Combine(AppContext.BaseDirectory, string.Format(@"Settings\{0}\{1}-{2}\{3}.xml", "Templar", StyxWoW.Me.Name, StyxWoW.Me.RealmName, "BlacklistSettings"));
            }
        }

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================
        public static void Load() {
            try {
                Instance = ObjectXMLSerializer<BlacklistSettings>.Load(SettingsFilePath);
            } catch(Exception) {
                Instance = new BlacklistSettings();
            }
        }

        public static void Save() {
            ObjectXMLSerializer<BlacklistSettings>.Save(Instance, SettingsFilePath);
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================
    }
}
