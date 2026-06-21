using System;
using System.IO;
using Styx;
using Styx.Helpers;
using Templar.Helpers;

namespace Templar.GUI.Tabs {
    public class GeneralSettings {

        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        public static GeneralSettings Instance = new GeneralSettings();

        public int HealthPercentThreshold = 50;
        public int MobThreshold = 3;

        public int StartingLocationMaxDistance = 1000;

        public bool IgnoreElites = true;
        public bool MultiPull;

        public bool LootMobs = true;
        public bool SkinMobs;
        public bool NinjaSkin;

        public double TimeBetweenLoots = 3;
        public double TimeBetweenSkins = 3;

        public int MinimumFreeBagSlots = 2;

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
            get { return Path.Combine(AppContext.BaseDirectory, string.Format(@"Settings\{0}\{1}-{2}\{3}.xml", "Templar", StyxWoW.Me.Name, StyxWoW.Me.RealmName, "GeneralSettings")); }
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
