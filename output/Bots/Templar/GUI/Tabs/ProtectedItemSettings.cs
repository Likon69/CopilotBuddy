using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using Styx;
using Styx.Helpers;
using Templar.Helpers;

namespace Templar.GUI.Tabs {
    public class ProtectedItemEntry : INotifyPropertyChanged {
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

    public class ProtectedItemSettings {

        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        public static ProtectedItemSettings Instance = new ProtectedItemSettings();

        public BindingList<ProtectedItemEntry> ProtectedItems = new BindingList<ProtectedItemEntry>();

        // ===========================================================
        // Constructors
        // ===========================================================

        static ProtectedItemSettings() {
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
                return Path.Combine(AppContext.BaseDirectory, string.Format(@"Settings\{0}\{1}-{2}\{3}.xml", "Templar", StyxWoW.Me.Name, StyxWoW.Me.RealmName, "ProtectedItemSettings"));
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
                Instance = ObjectXMLSerializer<ProtectedItemSettings>.Load(SettingsFilePath);
            } catch(Exception) {
                Instance = new ProtectedItemSettings();
            }
        }

        public static void Save() {
            ObjectXMLSerializer<ProtectedItemSettings>.Save(Instance, SettingsFilePath);
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

    }
}
