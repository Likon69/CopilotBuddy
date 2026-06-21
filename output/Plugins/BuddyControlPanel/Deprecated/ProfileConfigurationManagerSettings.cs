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
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
#endregion


namespace BuddyControlPanel
{

	[XmlRoot("ProfileConfigurationManagerSettings", Namespace = null)]
	public class ProfileConfigurationManagerSettings : XmlSettings<ProfileConfigurationManagerSettings>, IDisposable
	{
#region Creation & Destruction
		private static string dataFileName = Path.Combine(SettingsDirectory, "ProfileConfigurationManagerSettings.xml");

		public ProfileConfigurationManagerSettings()
			: base(dataFileName)
		{
			//SetDefaultValues();
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

			// Reclaim unmanaged resources --
			_instance = null;
		}

		[XmlIgnore]
		private static ProfileConfigurationManagerSettings _instance;

		[XmlIgnore]
		public static ProfileConfigurationManagerSettings Instance
		{
			get { return _instance ?? (_instance = Load(dataFileName)); }
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


#region Overrides of XmlSettings<ProfileConfigurationManagerSettings>
		public override void SetDefaultValues()
		{
			// empty
		}
#endregion


		// Actual settings...
		public List<ConfigurationOverride> ConfigurationOverrides { get; set; }


#region Types
		public class ConfigurationOverride
		{
			// This is the 'key' value...
			[XmlElement]
			public string Identifier { get; set; }


			[XmlElement]
			public bool IgnoreCheckpoints { get; set; }


			[XmlElement]
			public bool MailGrey { get; set; }

			[XmlElement]
			public bool MailWhite { get; set; }

			[XmlElement]
			public bool MailGreen { get; set; }

			[XmlElement]
			public bool MailBlue { get; set; }

			[XmlElement]
			public bool MailPurple { get; set; }


			[XmlElement]
			public bool SellGrey { get; set; }

			[XmlElement]
			public bool SellWhite { get; set; }

			[XmlElement]
			public bool SellGreen { get; set; }

			[XmlElement]
			public bool SellBlue { get; set; }

			[XmlElement]
			public bool SellPurple { get; set; }


			public ConfigurationOverride Copy()
			{
				return new ConfigurationOverride()
				{
					Identifier = this.Identifier,
					IgnoreCheckpoints = this.IgnoreCheckpoints,

					MailGrey = this.MailGrey,
					MailWhite = this.MailWhite,
					MailGreen = this.MailGreen,
					MailBlue = this.MailBlue,
					MailPurple = this.MailPurple,

					SellGrey = this.SellGrey,
					SellWhite = this.SellWhite,
					SellGreen = this.SellGreen,
					SellBlue = this.SellBlue,
					SellPurple = this.SellPurple
				};
			}


			public override bool Equals(object obj)
			{
				var rhs = obj as ConfigurationOverride;

				if (rhs == null)
					return false;

				return
					(Identifier == rhs.Identifier)
					&& (IgnoreCheckpoints == rhs.IgnoreCheckpoints)

					&& (MailGrey == rhs.MailGrey)
					&& (MailWhite == rhs.MailWhite)
					&& (MailGreen == rhs.MailGreen)
					&& (MailBlue == rhs.MailBlue)
					&& (MailPurple == rhs.MailPurple)

					&& (SellGrey == rhs.SellGrey)
					&& (SellWhite == rhs.SellWhite)
					&& (SellGreen == rhs.SellGreen)
					&& (SellBlue == rhs.SellBlue)
					&& (SellPurple == rhs.SellPurple);
			}


			public override int GetHashCode()
			{
				return Identifier.GetHashCode();
			}


			public override string ToString()
			{
				var xElementRoot = new XElement("HBProfile");

				xElementRoot.Add(new XAttribute("Identifier", Identifier ?? string.Empty));

				xElementRoot.Add(new XElement("IgnoreCheckpoints", IgnoreCheckpoints));

				xElementRoot.Add(new XElement("MailGrey", MailGrey));
				xElementRoot.Add(new XElement("MailWhite", MailWhite));
				xElementRoot.Add(new XElement("MailGreen", MailGreen));
				xElementRoot.Add(new XElement("MailBlue", MailBlue));
				xElementRoot.Add(new XElement("MailPurple", MailPurple));

				xElementRoot.Add(new XElement("SellGrey", SellGrey));
				xElementRoot.Add(new XElement("SellWhite", SellWhite));
				xElementRoot.Add(new XElement("SellGreen", SellGreen));
				xElementRoot.Add(new XElement("SellBlue", SellBlue));
				xElementRoot.Add(new XElement("SellPurple", SellPurple));

				return xElementRoot.ToString();
			}
		}
#endregion
	}
}

#endif // DEPRECATED