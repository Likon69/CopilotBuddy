using System;
using System.IO;
using Styx;
using Styx.Helpers;
using Templar.Helpers;

namespace Templar.GUI.Tabs {
    public class MailSettings {

        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        public static MailSettings Instance = new MailSettings();

        public bool Mail;

        public bool MailWhites = true;
        public bool MailGreens = true;
        public bool MailBlues = true;
        public bool MailPurples = true;

        public string Recipient;

        // ===========================================================
        // Constructors
        // ===========================================================

        static MailSettings() {
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
            get { return Path.Combine(AppContext.BaseDirectory, string.Format(@"Settings\{0}\{1}-{2}\{3}.xml", "Templar", StyxWoW.Me.Name, StyxWoW.Me.RealmName, "MailSettings")); }
        }

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        public static void Load() {
            try {
                Instance = ObjectXMLSerializer<MailSettings>.Load(SettingsFilePath);
            } catch(Exception) {
                Instance = new MailSettings();
            }
        }

        public static void Save() {
            ObjectXMLSerializer<MailSettings>.Save(Instance, SettingsFilePath);
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

    }
}
