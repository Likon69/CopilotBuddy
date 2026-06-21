using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using Styx;
using Styx.Helpers;
using Templar.Helpers;

namespace Templar.GUI.Tabs {
    public class WhitelistEntry : INotifyPropertyChanged {
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

    public class WhitelistSettings {
        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        public static WhitelistSettings Instance = new WhitelistSettings();

        public BindingList<WhitelistEntry> WhitelistedUnits = new BindingList<WhitelistEntry>();

        public bool AttackWhitelistOnly;

        // ===========================================================
        // Constructors
        // ===========================================================

        static WhitelistSettings() {
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
                return Path.Combine(AppContext.BaseDirectory, string.Format(@"Settings\{0}\{1}-{2}\{3}.xml", "Templar", StyxWoW.Me.Name, StyxWoW.Me.RealmName, "WhitelistSettings"));
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
                Instance = ObjectXMLSerializer<WhitelistSettings>.Load(SettingsFilePath);
            } catch(Exception) {
                Instance = new WhitelistSettings();
            }
        }

        public static void Save() {
            ObjectXMLSerializer<WhitelistSettings>.Save(Instance, SettingsFilePath);
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================
    }
}
