using System;
using System.IO;
using ObjectGatherer.Helpers;
using Styx;
using Styx.Helpers;

namespace ObjectGatherer.GUI.Tabs {
    public class GeneralSettings {
        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        public static GeneralSettings Instance = new GeneralSettings();

        public bool LootCorpses = true;
        public bool GatherOre;
        public bool GatherHerbs;

        // ===========================================================
        // Constructors
        // ===========================================================

        static GeneralSettings() {
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
                return Path.Combine(AppContext.BaseDirectory, string.Format(@"Settings\{0}\{1}-{2}\{3}.xml", "ObjectGatherer", StyxWoW.Me.Name, StyxWoW.Me.RealmName, "GeneralSettings"));
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
                Instance = ObjectXMLSerializer<GeneralSettings>.Load(SettingsFilePath);
            } catch(Exception) {
                Instance = new GeneralSettings();
            }
        }

        public static void Save() {
            ObjectXMLSerializer<GeneralSettings>.Save(Instance, SettingsFilePath);
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================
    }
}
