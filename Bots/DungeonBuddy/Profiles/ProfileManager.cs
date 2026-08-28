using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Bots.DungeonBuddy.Helpers;
using Bots.DungeonBuddy.Profiles.Handlers;
using Styx;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;
using Styx.Patchables;
using Styx.WoWInternals;
using Styx.WoWInternals.DBC;
using Styx.WoWInternals.WoWObjects;

namespace Bots.DungeonBuddy.Profiles
{
    /// <summary>
    /// Gère les profils XML de DungeonBuddy.
    /// Les profils XML sont placés dans Default Profiles\DungeonBuddy\ à côté de l'exe.
    /// Portage de HB 4.3.4 Bots\DungeonBuddy\Profiles\ProfileManager.cs.
    /// </summary>
    public static class ProfileManager
    {
        private static readonly string _profilesPath = Path.Combine(Logging.ApplicationPath, "Default Profiles\\DungeonBuddy\\");

        // HB 4.3.4: string_0 = Path.Combine(Logging.ApplicationPath, "Default Profiles\\DungeonBuddy\\")
        public static Profile CurrentProfile { get; private set; }

        public static event EventHandler<EventArgs> OnProfileSet;

        public static void Load(Profile profile)
        {
            if (CurrentProfile != null)
                UnloadProfile();

            CurrentProfile = profile;
            if (CurrentProfile != null && CurrentProfile.Blackspots != null)
            {
                var blackspots = CurrentProfile.Blackspots;
                BlackspotManager.AddBlackspots(blackspots.ConvertAll<global::Styx.Logic.Profiles.Blackspot>(b => b));
            }

            OnProfileSet?.Invoke(null, EventArgs.Empty);
        }

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

                    ErrorCollection errors;
                    Profile profile;
                    try
                    {
                        profile = Profile.Load(file, out errors);
                    }
                    catch (XmlException ex)
                    {
                        Logger.Write("There was an XML error in your profile!");
                        Logger.Write(ex.Message);
                        return;
                    }

                    foreach (Error error in errors)
                        Logger.WriteError(error);

                    if (!errors.HasErrors)
                    {
                        Load(profile);
                        Logger.Write("Successfully loaded: {0}", Path.GetFileNameWithoutExtension(file));
                    }

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
                ErrorCollection errors;
                Profile profile = Profile.Load(path, out errors);

                foreach (Error error in errors)
                    Logger.WriteError(error);

                if (!errors.HasErrors)
                {
                    Load(profile);
                    Logger.Write("Successfully loaded: {0}", System.IO.Path.GetFileNameWithoutExtension(path));
                }
            }
            catch (Exception ex)
            {
                Logging.WriteDiagnostic("[DungeonBuddy] Error loading profile {0}: {1}", path, ex.Message);
            }
        }

        public static void UnloadProfile()
        {
            if (CurrentProfile != null && CurrentProfile.Blackspots.Count > 0)
                BlackspotManager.RemoveBlackspots(CurrentProfile.Blackspots.ConvertAll<global::Styx.Logic.Profiles.Blackspot>(b => b));

            CurrentProfile = null;
        }

        /// <summary>HB 4.3.4: GetLfgDungeonIdFromMapId</summary>
        public static uint GetLfgDungeonIdFromMapId(uint mapId)
        {
            int difficultyIndex = Lua.GetReturnVal<int>("return GetInstanceDifficulty()", 0) - 1;
            var candidates = new List<LfgDungeons>();

            var table = StyxWoW.Db?[ClientDb.LfgDungeons];
            if (table == null)
                throw new InstanceNotFoundException("Unable to find LfgDungeon for mapId " + mapId);

            int validRows = 0;
            int mapMatches = 0;

            for (uint i = (uint)table.MinIndex; i <= (uint)table.MaxIndex; i++)
            {
                var dungeon = new LfgDungeons(i);
                if (!dungeon.IsValid || dungeon.IsHolidayEvent)
                    continue;
                validRows++;
                if (dungeon.MapId != (int)mapId)
                    continue;
                mapMatches++;
                if (dungeon.Difficulty == (uint)difficultyIndex)
                    candidates.Add(dungeon);
            }

            int beforeLevelFilter = candidates.Count;
            int dungeonLevel = -1;
            if (candidates.Count > 1)
            {
                dungeonLevel = Lua.GetReturnVal<int>("return GetCurrentMapDungeonLevel()", 0);
                if (dungeonLevel > 0)
                    candidates.RemoveAll(d => d.RecommendedLevel != (uint)dungeonLevel);
            }

            if (candidates.Count > 1)
            {
                uint myLevel = (uint)(StyxWoW.Me?.Level ?? 0);
                var inBracket = candidates.FindAll(d => myLevel >= d.MinLevel && myLevel <= d.MaxLevel);
                if (inBracket.Count > 0)
                    candidates = inBracket;
            }

            if (candidates.Count > 0)
                return candidates[0].Id;

            Logging.WriteDiagnostic(
                "[LfgDungeons] mapId={0} rows={1}..{2} valid={3} mapMatches={4} difficultyIndex={5} candidates={6} dungeonLevel={7}",
                mapId, table.MinIndex, table.MaxIndex, validRows, mapMatches, difficultyIndex, beforeLevelFilter, dungeonLevel);

            throw new InstanceNotFoundException("Unable to find LfgDungeon for mapId " + mapId);
        }

        public static bool IsNpcInPullBlackspot(WoWUnit unit)
        {
            if (CurrentProfile == null || CurrentProfile.PullBlackspots == null)
                return false;

            return CurrentProfile.PullBlackspots
                .Select(p => new { Pull = p, RadiusSqr = p.Radius * p.Radius })
                .Where(x => unit.Location.DistanceSqr(x.Pull.Location) < x.RadiusSqr)
                .Select(x => x.Pull)
                .Any(pull =>
                {
                    if (pull.Height == 0f)
                        return true;

                    if (pull.Height > 0f && pull.Z + pull.Height < unit.Z)
                        return false;

                    if (pull.Height < 0f && pull.Z + pull.Height > unit.Z)
                        return false;

                    return unit.Z >= pull.Z;
                });
        }
    }
}
