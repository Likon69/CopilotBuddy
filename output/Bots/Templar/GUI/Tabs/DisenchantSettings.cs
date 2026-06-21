using System;
using System.IO;
using Styx;
using Styx.Helpers;
using Templar.Helpers;

namespace Templar.GUI.Tabs {
    public class DisenchantSettings {

        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        public static DisenchantSettings Instance = new DisenchantSettings();

        public bool Disenchant;
        public bool DisenchantOnlyNewItems = true;
        public bool DisenchantWeapons = true;

        public bool DisenchantGreens;
        public bool DisenchantBlues;
        public bool DisenchantPurples;

        public double TimeBetweenDisenchants = 5;

        // ===========================================================
        // Constructors
        // ===========================================================

        static DisenchantSettings() {
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
            get { return Path.Combine(AppContext.BaseDirectory, string.Format(@"Settings\{0}\{1}-{2}\{3}.xml", "Templar", StyxWoW.Me.Name, StyxWoW.Me.RealmName, "DisenchantSettings")); }
        }

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        public static void Load() {
            try {
                Instance = ObjectXMLSerializer<DisenchantSettings>.Load(SettingsFilePath);
            } catch(Exception) {
                Instance = new DisenchantSettings();
            }
        }

        public static void Save() {
            ObjectXMLSerializer<DisenchantSettings>.Save(Instance, SettingsFilePath);
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

    }
}
