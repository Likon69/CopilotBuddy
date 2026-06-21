using System;
using System.IO;
using Styx;
using Styx.Helpers;
using Templar.Helpers;

namespace Templar.GUI.Tabs {
    public class VendorSettings {

        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        public static VendorSettings Instance = new VendorSettings();

        public bool Vendor = true;

        public bool SellGrays = true;
        public bool SellWhites;
        public bool SellGreens;
        public bool SellBlues;
        public bool SellPurples;

        public int MinimumDurabilityForRepairs = 25;

        // ===========================================================
        // Constructors
        // ===========================================================

        static VendorSettings() {
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
            get { return Path.Combine(AppContext.BaseDirectory, string.Format(@"Settings\{0}\{1}-{2}\{3}.xml", "Templar", StyxWoW.Me.Name, StyxWoW.Me.RealmName, "VendorSettings")); }
        }

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        public static void Load() {
            try {
                Instance = ObjectXMLSerializer<VendorSettings>.Load(SettingsFilePath);
            } catch(Exception) {
                Instance = new VendorSettings();
            }
        }

        public static void Save() {
            ObjectXMLSerializer<VendorSettings>.Save(Instance, SettingsFilePath);
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

    }
}
