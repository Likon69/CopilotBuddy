// This work is licensed under the
//     Creative Commons Attribution-NonCommercial-ShareAlike 4.0 Unported License.
// also known as CC-BY-NC-SA.  To view a copy of this license, visit
//      http://creativecommons.org/licenses/by-nc-sa/4.0/
// or send a letter to
//      Creative Commons // 171 Second Street, Suite 300 // San Francisco, California, 94105, USA.
//
#if DEPRECATED

#region Usings
using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Bots.Grind;
using Bots.Quest;
using Styx.CommonBot;
using Styx.Logic.Profiles;

// ReSharper disable RedundantLambdaSignatureParentheses
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
#endregion


namespace BuddyControlPanel
{
	public class ProfileConfigurationManager : IDisposable
	{
#region Creation & Destruction
		private ProfileConfigurationManager()
		{
			BotEvents.Profile.OnNewOuterProfileLoaded += BotEvents_OnNewOuterProfileLoaded;
		}

		// Basic Dispose pattern (ref: https://msdn.microsoft.com/en-us/library/b1yfkh5e%28v=vs.110%29.aspx)
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;

			// Reclaim unmanaged resources (hooked elements) --
			BotEvents.Profile.OnNewOuterProfileLoaded -= BotEvents_OnNewOuterProfileLoaded;

			_instance = null;
		}


		private static ProfileConfigurationManager _instance;

		public static ProfileConfigurationManager Instance
		{
			get { return _instance ?? (_instance = new ProfileConfigurationManager()); }
		}

		/// <summary>
		/// De-initializes the singleton instance.
		/// </summary>
		public static void ToJunkyard()
		{
			if (_instance != null)
			{
				_instance.Dispose();
				_instance = null;
			}
		}
#endregion


#region Private data
		private static readonly List<ProfileConfigurationManagerSettings.ConfigurationOverride> _originalSettings =
			new List<ProfileConfigurationManagerSettings.ConfigurationOverride>();

		private static readonly Dictionary<string, string> _profileIdentifier = new Dictionary<string, string>(); 
#endregion


#region Events
		private void BotEvents_OnNewOuterProfileLoaded(BotEvents.Profile.NewProfileLoadedEventArgs args)
		{
			var profile = args.NewProfile;
			if (profile == null)
				return;

			CaptureOriginalSettings(profile, ProfileManager.XmlLocation);

			// If setting overrides have been saved for profile, apply them...
			var settings = FindConfigurationOverrideForProfile(profile);
			if (settings != null)
				Configuration_Set(profile, settings);
		}
#endregion


		private static void CaptureOriginalSettings(Profile profile, string xmlLocation)
		{
			Contract.Requires(profile != null, () => "Profile may not be null.");

			// Capture the profile identifier, if necessary...
			string identifier;
			var identifierLocator = FindIdentifierLocator(profile);
			if (!_profileIdentifier.TryGetValue(identifierLocator, out identifier))
			{
				// At first blush, it would appear Profile.Name would be a best choice for a unique profile identifier.
				// However, it has several characteristics that make it undesirable:
				// * First, it is not guaranteed to be unique.  Profile writers don't coordinate the naming of their work
				//   and this can result in conflicts.  Additionally, through cut-n-paste of profile snippets, the name may
				//   be easily and accidentally duplicated.
				// * Second, Profile.Name frequently contains a SVN $Rev$ as a component.  This means that different versions
				//   of the _same_ profile would be treated as distinct if we used the Profile.Name.  This is counter-intuitive to
				//   what a user would expect to happen--all versions of the same profile would use the same settings.
				//
				// The XmlLocation has the properties we desire in an identifier.  It is the same value for all versions
				// of the profile, and it cannot be accidently collided due to cut-n-paste errors, or non-coordination with
				// other profile writers.  As such, this makes XmlLocation our preferred identifier. 
				//
				// Alas, XmlLocation is not available for streamed profiles.  For this situation, we must simply do the best
				// we can.  We achieve this goal by using the identifierLocator itself as the profile identifier.  Not an ideal
				// solution, but serviceable.
				//
				_profileIdentifier[identifierLocator] = 
					!string.IsNullOrEmpty(xmlLocation) ? xmlLocation : identifierLocator;
			}

			// If original settings are not already captured, snag them...
			var originalProfileSettings = _originalSettings.FirstOrDefault(s => s.Identifier == identifier);
			if (originalProfileSettings == null)
				_originalSettings.Add(Configuration_Get(profile));
		}


		public static ProfileConfigurationManagerSettings.ConfigurationOverride Configuration_Get(Profile profile)
		{
			Contract.Requires(profile != null, () => "Profile may not be null.");

			return new ProfileConfigurationManagerSettings.ConfigurationOverride()
			{
				Identifier = GetProfileIdentifer(profile),
				IgnoreCheckpoints = profile.QuestOrder.IgnoreCheckpoints,

				MailGrey = profile.MailGrey,
				MailWhite = profile.MailWhite,
				MailGreen = profile.MailGreen,
				MailBlue = profile.MailBlue,
				MailPurple = profile.MailPurple,

				SellGrey = profile.SellGrey,
				SellWhite = profile.SellWhite,
				SellGreen = profile.SellGreen,
				SellBlue = profile.SellBlue,
				SellPurple = profile.SellPurple,
			};
		}


		public static ProfileConfigurationManagerSettings.ConfigurationOverride Configuration_GetOriginalSettings(Profile profile)
		{
			Contract.Requires(profile != null, () => "Profile may not be null.");

			var identifier = GetProfileIdentifer(profile);
			var originalSettings = _originalSettings.FirstOrDefault(s => s.Identifier == identifier);
			Contract.Provides(originalSettings != null, () => "Profile's original settings were not captured.");

			// Return a copy of the original settings, so if caller fiddles with them it won't alter the actual
			// original settings...
			return originalSettings.Copy();
		}


		public static void Configuration_Set(Profile profile,
											ProfileConfigurationManagerSettings.ConfigurationOverride settings)
		{
			Contract.Requires(profile != null, () => "Profile may not be null.");
			Contract.Requires(settings != null, () => "settings may not be null.");

			settings.Identifier = GetProfileIdentifer(profile);

			// Update persistent storage with new settings...
			var settingsInstance = ProfileConfigurationManagerSettings.Instance;
			settingsInstance.ConfigurationOverrides.RemoveAll(o => o.Identifier == settings.Identifier);

			var originalSettings = Configuration_GetOriginalSettings(profile);
			if (!settings.Equals(originalSettings))
				settingsInstance.ConfigurationOverrides.Add(settings);

			settingsInstance.Save();

			// Apply settings to profile...
			profile.QuestOrder.IgnoreCheckpoints = settings.IgnoreCheckpoints;

			profile.MailGrey = settings.MailGrey;
			profile.MailWhite = settings.MailWhite;
			profile.MailGreen = settings.MailGreen;
			profile.MailBlue = settings.MailBlue;
			profile.MailPurple = settings.MailPurple;

			profile.SellGrey = settings.SellGrey;
			profile.SellWhite = settings.SellWhite;
			profile.SellGreen = settings.SellGreen;
			profile.SellBlue = settings.SellBlue;
			profile.SellPurple = settings.SellPurple;;
		}


		private static ProfileConfigurationManagerSettings.ConfigurationOverride FindConfigurationOverrideForProfile(Profile profile)
		{
			Contract.Requires(profile != null, () => "Profile may not be null.");

			var identifier = GetProfileIdentifer(profile);

			return
				ProfileConfigurationManagerSettings.Instance
				.ConfigurationOverrides
				.FirstOrDefault(o => o.Identifier == identifier);
		}


		private static string FindIdentifierLocator(Profile profile)
		{
			Contract.Requires(profile != null, () => "profile may not be null.");

			// Use information contained in Name element, if present...
			if (!string.IsNullOrEmpty(profile.Name))
				return profile.Name.Trim();

			// Use Path, if present...
			if (!string.IsNullOrEmpty(profile.Path))
				return profile.Path.Trim();

			// Otherwise, build a crude identifier from the profile contents...
			return GenerateIdentifierLocatorFromProfileContents(profile);
		}


		private static string GenerateIdentifierLocatorFromProfileContents(Profile profile)
		{
			var crudeHash = new StringBuilder();

			crudeHash.Append(profile.AerialBlackspots.Count).Append("-");

			var avoidMobsHash = profile.AvoidMobs.HashSet1.Aggregate<uint, uint>(0, (current, avoidMob) => current + avoidMob);
			crudeHash.Append(profile.AvoidMobs.Count).Append(avoidMobsHash).Append("-");

			var blacklistHash = profile.Blacklist
				.Aggregate<KeyValuePair<uint, BlacklistFlags>, uint>(0, (current, blacklistKvp) => current + blacklistKvp.Key);
			crudeHash.Append(profile.Blacklist.Count).Append(blacklistHash).Append("-");

			int blackspotHash = profile.Blackspots.Sum(blackspot => blackspot.GetHashCode());
			crudeHash.Append(profile.Blackspots.Count).Append(blackspotHash).Append("-");

			crudeHash.Append(profile.ContinentId).Append("-");

			// N.B.: We can not use any of the MailX values as part of the identifier,
			// because we might be modifying these values.

			crudeHash.Append(profile.MaxLevel).Append("-");
			crudeHash.Append(profile.MinLevel).Append("-");

			crudeHash.Append((profile.Name ?? string.Empty).GetHashCode()).Append("-");
			crudeHash.Append((profile.Path ?? string.Empty).GetHashCode()).Append("-");

			// N.B.: We can not use any of the SellX values as part of the identifier,
			// because we might be modifying these values.

			crudeHash.Append(profile.QuestOrder.Count).Append("-");

			if (profile.GrindArea != null)
			{
				var hotspotsHash = profile.GrindArea.Hotspots.Sum(hotspot => hotspot.GetHashCode());
				crudeHash.Append(profile.GrindArea.Hotspots.Count).Append(hotspotsHash).Append("-");
			}

			return crudeHash.ToString();
		}

	
		private static string GetProfileIdentifer(Profile profile)
		{
			Contract.Requires(profile != null, () => "profile may not be null.");

			string identifier;
			var identifierLocator = FindIdentifierLocator(profile);

			var isFound = _profileIdentifier.TryGetValue(identifierLocator, out identifier);
			Contract.Provides(isFound, () => string.Format("identifier for profile \"{0}\" not found.", profile.Name));
			Contract.Provides(!string.IsNullOrEmpty(identifier), () => "identifier may not be null or empty.");

			return identifier;
		}


		public static string GetProfileType(Profile profile)
		{
			var botBase =
				IsQuestProfile(profile)
				? Utility.FindBotBase<QuestBot>()
				: Utility.FindBotBase<LevelBot>();

			return (botBase != null) ? botBase.Name : string.Empty;
		}


		public static bool IsQuestProfile(Profile profile)
		{
			Contract.Requires(profile != null, () => "Profile may not be null.");

			return profile.QuestOrder.Count > 0;
		}
	}
}
#endif  // DEPRECATED