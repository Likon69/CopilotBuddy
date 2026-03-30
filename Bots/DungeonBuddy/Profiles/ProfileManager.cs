using System;
using System.IO;
using System.Xml.Linq;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;

namespace Bots.DungeonBuddy.Profiles
{
    /// <summary>
    /// Gère les profils XML de DungeonBuddy.
    /// Les profils XML sont placés dans Default Profiles\DungeonBuddy\ à côté de l'exe.
    /// Portage de HB 4.3.4 Bots\DungeonBuddy\Profiles\ProfileManager.cs.
    /// </summary>
    public static class ProfileManager
    {
        private static readonly string _profilesPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Default Profiles", "DungeonBuddy");

        // HB 4.3.4: string_0 = Path.Combine(Logging.ApplicationPath, "Default Profiles\\DungeonBuddy\\")
        public static DungeonProfile CurrentProfile { get; private set; }

        /// <summary>
        /// Scanne Default Profiles\DungeonBuddy\ pour trouver le profil XML correspondant au dungeonId.
        /// </summary>
        public static void LoadProfileForDungeon(uint dungeonId)
        {
            if (!Directory.Exists(_profilesPath))
            {
                Logging.Write("[DungeonBuddy] Profiles folder not found: {0}", _profilesPath);
                return;
            }

            string[] files = Directory.GetFiles(_profilesPath, "*.xml", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                try
                {
                    var root = XElement.Load(file);
                    var dungeonIdEl = root.Element("DungeonId");
                    if (dungeonIdEl == null ||
                        !uint.TryParse(dungeonIdEl.Value, out uint id) ||
                        id != dungeonId)
                    {
                        continue;
                    }

                    UnloadProfile();
                    CurrentProfile = DungeonProfile.Load(root);

                    if (CurrentProfile.Blackspots.Count > 0)
                        BlackspotManager.AddBlackspots(CurrentProfile.Blackspots);

                    Logging.Write("[DungeonBuddy] Loaded profile: {0} ({1} hotspots, {2} blackspots)",
                        Path.GetFileNameWithoutExtension(file),
                        CurrentProfile.HotSpots.Count,
                        CurrentProfile.Blackspots.Count);
                    return;
                }
                catch (Exception ex)
                {
                    Logging.WriteDiagnostic("[DungeonBuddy] Error reading profile {0}: {1}", file, ex.Message);
                }
            }

            Logging.Write("[DungeonBuddy] No profile found for dungeonId: {0}", dungeonId);
        }

        /// <summary>
        /// Charge un profil directement depuis un chemin XML (utilisé par FormConfig).
        /// </summary>
        public static void LoadFromPath(string path)
        {
            try
            {
                var root = XElement.Load(path);
                UnloadProfile();
                CurrentProfile = DungeonProfile.Load(root);

                if (CurrentProfile.Blackspots.Count > 0)
                    BlackspotManager.AddBlackspots(CurrentProfile.Blackspots);

                Logging.Write("[DungeonBuddy] Loaded profile: {0} ({1} hotspots, {2} blackspots)",
                    System.IO.Path.GetFileNameWithoutExtension(path),
                    CurrentProfile.HotSpots.Count,
                    CurrentProfile.Blackspots.Count);
            }
            catch (Exception ex)
            {
                Logging.WriteDiagnostic("[DungeonBuddy] Error loading profile {0}: {1}", path, ex.Message);
            }
        }

        public static void UnloadProfile()
        {
            if (CurrentProfile != null && CurrentProfile.Blackspots.Count > 0)
                BlackspotManager.RemoveBlackspots(CurrentProfile.Blackspots);
            CurrentProfile = null;
        }
    }
}
