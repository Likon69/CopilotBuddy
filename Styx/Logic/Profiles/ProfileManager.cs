using System;
using System.IO;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.WoWInternals;

namespace Styx.Logic.Profiles
{
	public static class ProfileManager
	{
		private static Profile? _currentOuterProfile;
		private static Profile? _currentProfile;
		private static bool _profileless = false; // Flag for bots that don't use profiles

		static ProfileManager()
		{
			BotEvents.Player.OnLevelUp += OnLevelUp;
			BotEvents.Profile.OnNewProfileLoaded += OnNewProfileLoaded;
			XmlLocation = "";
		}

		private static void OnNewProfileLoaded(BotEvents.Profile.NewProfileLoadedEventArgs args)
		{
			Profile? newProfile = args.NewProfile;
			if (newProfile != null)
			{
				string text = string.Format("Profile loaded: {0}",
					!string.IsNullOrEmpty(newProfile.Name) ? newProfile.Name :
					string.Format("L{0}-{1}", newProfile.MinLevel, newProfile.MaxLevel));
				Logging.Write(text);
			}
			else
			{
				Logging.Write("We either outleveled the currently loaded profile, or something has gone terribly wrong. Can't find a new sub-profile to use.");
			}
		}

		public static string XmlLocation { get; private set; }

		public static Profile? CurrentOuterProfile
		{
			get { return _currentOuterProfile; }
			private set
			{
				Profile? old = _currentOuterProfile;
				_currentOuterProfile = value;
				if (old != _currentOuterProfile && _currentOuterProfile != null)
				{
					BotEvents.Profile.RaiseOuterProfileLoaded(old, _currentOuterProfile);
				}
			}
		}

		public static Profile? CurrentProfile
		{
			get
			{
				// Bots like CombatBot, LazyRaider don't use profiles
				if (_profileless)
					return _currentProfile;
					
				if (_currentProfile == null)
				{
					LoadProfileForLevel();
				}
				if (_currentProfile == null)
				{
					// Check if current bot requires profile (HB 6.2.3 pattern)
					if (BotManager.Current != null && !BotManager.Current.RequiresProfile)
					{
						// Bot doesn't require profile, return null without stopping
						return null;
					}
					
					Logging.Write("No profile loaded. Stopping bot.");
					TreeRoot.Stop();
					return null;
				}
				return _currentProfile;
			}
			private set
			{
				Profile? old = _currentProfile;
				_currentProfile = value;
				if (old != _currentProfile && _currentProfile != null)
				{
					BotEvents.Profile.RaiseNewProfileLoaded(old, _currentProfile);
				}
			}
		}

		private static void OnLevelUp(BotEvents.Player.LevelUpEventArgs args)
		{
			if (Battlegrounds.IsInsideBattleground)
				return;
			Logging.Write("We leveled up! Checking if we need to switch profiles.");
			LoadProfileForLevel();
		}

		private static void LoadProfileForLevel()
		{
			var profile = GetProfileForLevel(ObjectManager.Me?.Level ?? 1);
			CurrentProfile = profile;
			if (profile != null)
				Logging.WriteDebug("Selected sub-profile: {0} (L{1}-{2})", profile.Name, profile.MinLevel, profile.MaxLevel);
		}

		private static Profile? GetProfileForLevel(int level)
		{
			if (CurrentOuterProfile == null)
			{
				return null;
			}

			var sortedProfiles = CurrentOuterProfile.GetScopeSortedProfiles();
			uint mapId = ObjectManager.Me?.MapId ?? 0;
			for (int i = 0; i < sortedProfiles.Count; i++)
			{
				// BUG-10: Check ContinentId matches current map (HB 4.3.4)
				if ((sortedProfiles[i].ContinentId == -1 || (long)sortedProfiles[i].ContinentId == (long)mapId)
					&& level >= sortedProfiles[i].MinLevel && level < sortedProfiles[i].MaxLevel)
				{
					return sortedProfiles[i];
				}
				Logging.WriteDebug("Skipping sub-profile '{0}' (L{1}-{2}, continent {3}) — current: level {4}, map {5}",
					sortedProfiles[i].Name, sortedProfiles[i].MinLevel, sortedProfiles[i].MaxLevel,
					sortedProfiles[i].ContinentId, level, mapId);
			}
			return null;
		}

		/// <summary>
		/// Composes every expression of the profile into its single batch and compiles it before any
		/// node runs. HB 6.2.3 ProfileManager.smethod_4: a compile failure stops the bot there.
		/// </summary>
		private static bool CompileProfileCode(Profile? profile)
		{
			if (profile == null)
				return true;

			profile.CodeComposition.AddProfile(profile);
			if (profile.CodeComposition.Batch.Compile())
				return true;

			Styx.Logic.Profiles.Quest.CompileError[] errors = profile.CodeComposition.Batch.Errors;
			Logging.Write(System.Drawing.Color.Red, "{0} compiler error(s) in profile '{1}'",
				errors.Length, profile.Name);
			foreach (Styx.Logic.Profiles.Quest.CompileError error in errors)
			{
				var element = error.Context as System.Xml.Linq.XElement;
				var lineInfo = element as System.Xml.IXmlLineInfo;
				if (lineInfo != null && lineInfo.HasLineInfo())
				{
					Logging.Write(System.Drawing.Color.Red, "  line {0} <{1}>: {2}",
						lineInfo.LineNumber, element.Name.LocalName, error.Error);
				}
				else
				{
					Logging.Write(System.Drawing.Color.Red, "  {0}: {1}", error.Code, error.Error);
				}
			}
			return false;
		}

		public static void LoadNew(string path, bool rememberMe)
		{
			if (string.IsNullOrEmpty(path) || !File.Exists(path))
			{
				throw new FileNotFoundException("Profile file not found.", path);
			}

			_profileless = false; // Clear profileless mode when loading real profile
			XmlLocation = path;
			if (rememberMe)
			{
				LevelbotSettings.Instance.LastUsedPath = path;
			}

			StyxWoW.AreaManager.SetArea(null);
			Logging.WriteDebug("Loading profile from {0}", path);
			Profile profile = new Profile(path, null);
			if (!CompileProfileCode(profile))
			{
				TreeRoot.Stop();
				return;
			}
			CurrentOuterProfile = profile;
			if (!ReferenceEquals(CurrentOuterProfile, profile))
			{
				Logging.WriteDebug("Profile {0} was replaced by a nested load of {1} while it was being announced", path, XmlLocation);
				return;
			}
			LoadProfileForLevel();
		}

		public static void LoadNew(string path)
		{
			LoadNew(path, true);
		}

	/// <summary>
	/// Loads an empty profile. Used by bots that don't require a profile (BGBuddy, LazyRaider, CombatBot, etc.)
	/// </summary>
	public static void LoadEmpty()
	{
		_profileless = true;
		StyxWoW.AreaManager.SetArea(null);
		CurrentOuterProfile = new Profile();
		CurrentProfile = null;
		Logging.Write("Running in profileless mode (CombatBot, LazyRaider, etc.)");
	}

	/// <summary>
	/// Clears profileless mode when loading a real profile
	/// </summary>
	public static void ClearProfilelessMode()
	{
		_profileless = false;
	}
}
}
