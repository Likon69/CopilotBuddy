using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Styx.Helpers;
using Styx.Logic.AreaManagement;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles.Quest;

namespace Styx.Logic.Profiles
{
	public class Profile : IEquatable<Profile>
	{

		public XElement XmlElement { get; private set; }

		public static event EventHandler<UnknownProfileElementEventArgs>? OnUnknownProfileElement;

		private readonly DualHashSet<uint, string> _protectedItems = new();
		private readonly DualHashSet<uint, string> _forceMail = new();
		private readonly DualHashSet<uint, string> _avoidMobs = new();
		private readonly HashSet<uint> _factions = new();
		private readonly List<QuestInfo> _quests = new();
		private readonly OrderNodeCollection _questOrder = new();
		private readonly List<Blackspot> _blackspots = new();
		private int? _continentId;
		private MailboxManager? _mailboxManager;
		private VendorManager? _vendorManager;
		private bool? _sellGrey;
		private bool? _sellWhite;
		private bool? _sellGreen;
		private bool? _sellBlue;
		private bool? _sellPurple;
		private bool? _mailGrey;
		private bool? _mailWhite;
		private bool? _mailGreen;
		private bool? _mailBlue;
		private bool? _mailPurple;
		private int? _minMailLevel;
		private float? _minDurability;
		private int? _minFreeBagSlots;

		public string? Name { get; set; }

		/// <summary>
		/// Faction IDs to target. Inherits from parent if empty.
		/// </summary>
		public HashSet<uint> Factions
		{
			get
			{
				if (_factions.Count > 0)
					return _factions;
				if (Parent != null)
					return Parent.Factions;
				return _factions;
			}
		}

		public int MinLevel { get; set; }

		public int MaxLevel { get; set; }

		/// <summary>
		/// Map ID this profile is for. -1 = any continent.
		/// BUG-10: HB 4.3.4 uses ContinentId to filter profiles by map.
		/// </summary>
		public int ContinentId
		{
			get
			{
				if (_continentId.HasValue)
					return _continentId.Value;
				if (Parent != null)
					return Parent.ContinentId;
				return -1;
			}
		}

		public GrindArea? GrindArea { get; set; }

		public HotspotManager? HotspotManager { get; set; }

		public Profile? Parent { get; set; }

		public List<Profile> SubProfiles { get; private set; }

		public string? FilePath { get; private set; }

		/// <summary>
		/// Whether to target elite mobs.
		/// If not set, inherits from parent profile.
		/// </summary>
		public bool? TargetElitesValue { get; set; }

		/// <summary>
		/// Gets whether to target elite mobs.
		/// Falls back to parent profile or false.
		/// </summary>
		public bool TargetElites
		{
			get
			{
				if (TargetElitesValue.HasValue)
					return TargetElitesValue.Value;
				if (Parent != null)
					return Parent.TargetElites;
				return false;
			}
		}

		/// <summary>
		/// Whether to sell grey quality items.
		/// Defaults to true.
		/// </summary>
		public bool SellGrey
		{
			get
			{
				if (_sellGrey.HasValue)
					return _sellGrey.Value;
				if (Parent != null)
					return Parent.SellGrey;
				return true;
			}
		}

		/// <summary>
		/// Whether to sell white (common) quality items.
		/// Defaults to false.
		/// </summary>
		public bool SellWhite
		{
			get
			{
				if (_sellWhite.HasValue)
					return _sellWhite.Value;
				if (Parent != null)
					return Parent.SellWhite;
				return false;
			}
		}

		/// <summary>
		/// Whether to sell green (uncommon) quality items.
		/// Defaults to false.
		/// </summary>
		public bool SellGreen
		{
			get
			{
				if (_sellGreen.HasValue)
					return _sellGreen.Value;
				if (Parent != null)
					return Parent.SellGreen;
				return false;
			}
		}

		/// <summary>
		/// Whether to sell blue (rare) quality items.
		/// Defaults to false.
		/// </summary>
		public bool SellBlue
		{
			get
			{
				if (_sellBlue.HasValue)
					return _sellBlue.Value;
				if (Parent != null)
					return Parent.SellBlue;
				return false;
			}
		}

		/// <summary>
		/// Whether to sell purple (epic) quality items.
		/// Defaults to false.
		/// </summary>
		public bool SellPurple
		{
			get
			{
				if (_sellPurple.HasValue)
					return _sellPurple.Value;
				if (Parent != null)
					return Parent.SellPurple;
				return false;
			}
		}

		/// <summary>
		/// Whether to mail grey quality items.
		/// </summary>
		public bool MailGrey
		{
			get
			{
				if (_mailGrey.HasValue)
					return _mailGrey.Value;
				if (Parent != null)
					return Parent.MailGrey;
				return false;
			}
		}

		/// <summary>
		/// Whether to mail white (common) quality items.
		/// </summary>
		public bool MailWhite
		{
			get
			{
				if (_mailWhite.HasValue)
					return _mailWhite.Value;
				if (Parent != null)
					return Parent.MailWhite;
				return true;
			}
		}

		/// <summary>
		/// Whether to mail green (uncommon) quality items.
		/// </summary>
		public bool MailGreen
		{
			get
			{
				if (_mailGreen.HasValue)
					return _mailGreen.Value;
				if (Parent != null)
					return Parent.MailGreen;
				return true;
			}
		}

		/// <summary>
		/// Whether to mail blue (rare) quality items.
		/// </summary>
		public bool MailBlue
		{
			get
			{
				if (_mailBlue.HasValue)
					return _mailBlue.Value;
				if (Parent != null)
					return Parent.MailBlue;
				return true;
			}
		}

		/// <summary>
		/// Whether to mail purple (epic) quality items.
		/// </summary>
		public bool MailPurple
		{
			get
			{
				if (_mailPurple.HasValue)
					return _mailPurple.Value;
				if (Parent != null)
					return Parent.MailPurple;
				return true;
			}
		}

		/// <summary>
		/// Gets the list of item qualities that should be mailed.
		/// </summary>
		public List<WoWItemQuality> MailQualities
		{
			get
			{
				var list = new List<WoWItemQuality>();
				if (MailGrey)
					list.Add(WoWItemQuality.Poor);
				if (MailWhite)
					list.Add(WoWItemQuality.Common);
				if (MailGreen)
					list.Add(WoWItemQuality.Uncommon);
				if (MailBlue)
					list.Add(WoWItemQuality.Rare);
				if (MailPurple)
					list.Add(WoWItemQuality.Epic);
				return list;
			}
		}

		/// <summary>
		/// Gets protected items that should not be sold or mailed.
		/// Falls back to parent profile if empty.
		/// </summary>
		public DualHashSet<uint, string> ProtectedItems
		{
			get
			{
				if (_protectedItems.Count > 0)
					return _protectedItems;
				if (Parent != null)
					return Parent.ProtectedItems;
				return _protectedItems;
			}
		}

		/// <summary>
		/// Gets items that should be force-mailed.
		/// Falls back to parent profile if empty.
		/// </summary>
		public DualHashSet<uint, string> ForceMail
		{
			get
			{
				if (_forceMail.Count > 0)
					return _forceMail;
				if (Parent != null)
					return Parent.ForceMail;
				return _forceMail;
			}
		}

		/// <summary>
		/// Gets mobs that should be avoided.
		/// Falls back to parent profile if empty.
		/// </summary>
		public DualHashSet<uint, string> AvoidMobs
		{
			get
			{
				if (_avoidMobs.Count > 0)
					return _avoidMobs;
				if (Parent != null)
					return Parent.AvoidMobs;
				return _avoidMobs;
			}
		}

		/// <summary>
		/// Gets the minimum level required to send mail.
		/// Defaults to 5 if not set.
		/// </summary>
		public int MinMailLevel
		{
			get
			{
				if (_minMailLevel.HasValue)
					return _minMailLevel.Value;
				if (Parent != null)
					return Parent.MinMailLevel;
				return 5;
			}
		}

		/// <summary>
		/// Gets the minimum durability percentage before repairing.
		/// Defaults to 0.4 (40%) if not set.
		/// </summary>
		public float MinDurability
		{
			get
			{
				if (_minDurability.HasValue)
					return _minDurability.Value;
				if (Parent != null)
					return Parent.MinDurability;
				return 0.4f;
			}
		}

		/// <summary>
		/// Gets the minimum free bag slots before selling.
		/// Defaults to 0 if not set.
		/// </summary>
		public int MinFreeBagSlots
		{
			get
			{
				if (_minFreeBagSlots.HasValue)
					return _minFreeBagSlots.Value;
				if (Parent != null)
					return Parent.MinFreeBagSlots;
				return 0;
			}
		}

		/// <summary>
		/// Gets blackspot areas to avoid.
		/// Falls back to parent profile if empty.
		/// </summary>
		public List<Blackspot> Blackspots
		{
			get
			{
				if (_blackspots.Count > 0)
					return _blackspots;
				if (Parent != null)
					return Parent.Blackspots;
				return _blackspots;
			}
		}

		/// <summary>
		/// Gets the vendor manager for this profile.
		/// Falls back to parent profile or creates empty one.
		/// </summary>
		public VendorManager VendorManager
		{
			get
			{
				if (_vendorManager != null)
					return _vendorManager;
				if (Parent != null)
					return Parent.VendorManager;
				_vendorManager ??= new VendorManager();
				return _vendorManager;
			}
		}

		/// <summary>
		/// Gets the mailbox manager for this profile.
		/// Falls back to parent profile.
		/// </summary>
		public MailboxManager? MailboxManager
		{
			get
			{
				if (_mailboxManager != null)
					return _mailboxManager;
				return Parent?.MailboxManager;
			}
		}

		/// <summary>
		/// Gets quest info objects defined in this profile.
		/// Falls back to parent profile if empty.
		/// </summary>
		public List<QuestInfo> Quests
		{
			get
			{
				if (_quests.Count > 0)
					return _quests;
				if (Parent != null)
					return Parent.Quests;
				return _quests;
			}
		}

		/// <summary>
		/// Gets the quest order nodes for this profile.
		/// Falls back to parent profile if empty.
		/// </summary>
		public OrderNodeCollection QuestOrder
		{
			get
			{
				if (_questOrder.Count > 0)
					return _questOrder;
				if (Parent != null)
					return Parent.QuestOrder;
				return _questOrder;
			}
		}

		/// <summary>
		/// Finds quest info by ID.
		/// </summary>
		/// <param name="id">Quest ID to find.</param>
		/// <returns>QuestInfo if found, null otherwise.</returns>
		public QuestInfo? FindQuest(uint id)
		{
			return Quests.FirstOrDefault(q => q.ID == id);
		}

		public Profile()
		{
			MinLevel = 1;
			MaxLevel = 85;
			HotspotManager = new HotspotManager(new List<WoWPoint>());
			SubProfiles = new List<Profile>();
		}

		public Profile(string path, Profile? parent) : this()
		{
			FilePath = path;
			Parent = parent;

			if (!File.Exists(path))
			{
				Logging.Write("Profile file not found: {0}", path);
				return;
			}

			try
			{
				XDocument doc = XDocument.Load(path);
				if (doc.Root == null)
				{
					Logging.Write("Profile has no root element: {0}", path);
					return;
				}
				// Preserve original XML element for callers that expect it (HB compatibility)
				XmlElement = doc.Root;
				ParseFromXml(doc.Root);
			}
			catch (Exception ex)
			{
				Logging.WriteException(ex);
			}
		}

		public Profile(XContainer xml, Profile? parent) : this()
		{
			Parent = parent;
			// Keep the original XML element for HB compatibility
			XmlElement = xml as XElement ?? (xml as XDocument)?.Root;
			ParseFromXml(xml);
		}

		private void ParseFromXml(XContainer xml)
		{
			foreach (XElement element in xml.Elements())
			{
				bool handled = false;
				string name = element.Name.LocalName.ToLowerInvariant();
				switch (name)
				{
					case "name":
						handled = true;
						Name = element.Value;
						break;
					case "minlevel":
						handled = true;
						if (int.TryParse(element.Value, out int minLevel))
							MinLevel = minLevel;
						break;
					case "maxlevel":
						handled = true;
						if (int.TryParse(element.Value, out int maxLevel))
							MaxLevel = maxLevel;
						break;
					case "continentid":
						handled = true;
						if (int.TryParse(element.Value, out int continentId))
							_continentId = continentId;
						break;
					case "sellgrey":
					case "sellgray":
						handled = true;
						if (bool.TryParse(element.Value, out bool sellGrey))
							_sellGrey = sellGrey;
						break;
					case "sellwhite":
						handled = true;
						if (bool.TryParse(element.Value, out bool sellWhite))
							_sellWhite = sellWhite;
						break;
					case "sellgreen":
						handled = true;
						if (bool.TryParse(element.Value, out bool sellGreen))
							_sellGreen = sellGreen;
						break;
					case "sellblue":
						handled = true;
						if (bool.TryParse(element.Value, out bool sellBlue))
							_sellBlue = sellBlue;
						break;
					case "sellpurple":
						handled = true;
						if (bool.TryParse(element.Value, out bool sellPurple))
							_sellPurple = sellPurple;
						break;
					case "mailgrey":
						handled = true;
						if (bool.TryParse(element.Value, out bool mailGrey))
							_mailGrey = mailGrey;
						break;
					case "mailwhite":
						handled = true;
						if (bool.TryParse(element.Value, out bool mailWhite))
							_mailWhite = mailWhite;
						break;
					case "mailgreen":
						handled = true;
						if (bool.TryParse(element.Value, out bool mailGreen))
							_mailGreen = mailGreen;
						break;
					case "mailblue":
						handled = true;
						if (bool.TryParse(element.Value, out bool mailBlue))
							_mailBlue = mailBlue;
						break;
					case "mailpurple":
						handled = true;
						if (bool.TryParse(element.Value, out bool mailPurple))
							_mailPurple = mailPurple;
						break;
					case "targetelites":
					case "elites":
					case "elite":
						handled = true;
						if (bool.TryParse(element.Value, out bool targetElites))
							TargetElitesValue = targetElites;
						break;
					case "protecteditems":
						handled = true;
						ParseItemList(element, _protectedItems);
						break;
					case "forcemail":
						handled = true;
						ParseItemList(element, _forceMail);
						break;
					case "avoidmobs":
					case "avoids":
						handled = true;
						ParseAvoidMobs(element);
						break;
					case "minmaillevel":
						handled = true;
						if (int.TryParse(element.Value, out int minMailLevel))
							_minMailLevel = minMailLevel;
						break;
					case "mindurability":
						handled = true;
						if (float.TryParse(element.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float minDurability))
							_minDurability = minDurability;
						break;
					case "minfreebagslots":
						handled = true;
						if (int.TryParse(element.Value, out int minFreeBagSlots))
							_minFreeBagSlots = minFreeBagSlots;
						break;
					case "blackspots":
					case "ignorespots":
						handled = true;
						ParseBlackspots(element);
						break;
					case "mailing":
					case "mailboxes":
					case "mails":
						handled = true;
						_mailboxManager = new MailboxManager(element);
						break;
					case "vendors":
						handled = true;
						_vendorManager = new VendorManager(element);
						break;
					case "grindarea":
						handled = true;
						GrindArea = GrindArea.FromXML(element);
						// Copy factions to grindarea if specified at profile level
						if (Factions.Count > 0 && GrindArea.Factions.Count == 0)
						{
							foreach (uint faction in Factions)
								GrindArea.Factions.Add((int)faction);
						}
						break;
					case "hotspots":
						handled = true;
						HotspotManager = new HotspotManager(element);
						GrindArea = new GrindArea(HotspotManager);
						break;
					case "factions":
						handled = true;
						foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(element.Value, "\\d+"))
							_factions.Add(Convert.ToUInt32(match.Value));
						if (GrindArea != null && GrindArea.Factions.Count == 0)
						{
							foreach (uint faction in _factions)
								GrindArea.Factions.Add((int)faction);
						}
						break;
					case "quest":
					case "questinfo":
						handled = true;
						try
						{
							GetRootProfile()._quests.Add(QuestInfo.FromXML(element));
						}
						catch (ProfileException ex)
						{
							Logging.WriteDebug(ex.Message);
						}
						break;
					case "questorder":
						handled = true;
						foreach (OrderNode node in OrderNodeCollection.FromXml(element))
						{
							_questOrder.Add(node);
						}
						break;
					case "subprofile":
						handled = true;
						Profile sub = new Profile(element, this);
						SubProfiles.Add(sub);
						break;
				}
				// HonorBuddy behavior: if element wasn't handled by switch, treat it as unknown
				if (!handled && element.NodeType != XmlNodeType.XmlDeclaration && element.NodeType != XmlNodeType.Comment)
				{
					if (OnUnknownProfileElement == null)
						throw new ProfileUnknownElementException(element);
					var args = new UnknownProfileElementEventArgs(element);
					OnUnknownProfileElement?.Invoke(this, args);
					if (!args.Handled)
						throw new ProfileUnknownElementException(element);
				}
			}
		}

		/// <summary>
		/// Gets the root profile in the hierarchy.
		/// </summary>
		public Profile GetRootProfile()
		{
			Profile root = this;
			while (root.Parent != null)
				root = root.Parent;
			return root;
		}

		private void ParseItemList(XElement parent, DualHashSet<uint, string> items)
		{
			foreach (XElement element in parent.Elements())
			{
				string tagName = element.Name.LocalName.ToLowerInvariant();
				if (tagName != "item")
					continue;

				uint itemId = 0;
				string itemName = "";

				foreach (XAttribute attr in element.Attributes())
				{
					string attrName = attr.Name.LocalName.ToLowerInvariant();
					switch (attrName)
					{
						case "name":
							itemName = attr.Value;
							break;
						case "entry":
						case "id":
							uint.TryParse(attr.Value, out itemId);
							break;
					}
				}

				if (itemId == 0)
					uint.TryParse(element.Value, out itemId);

				if (itemId > 0)
					items.Add(itemId);
				if (!string.IsNullOrEmpty(itemName))
					items.Add(itemName.ToLower());
			}
		}

		private void ParseAvoidMobs(XElement parent)
		{
			foreach (XElement element in parent.Elements())
			{
				string tagName = element.Name.LocalName.ToLowerInvariant();
				if (tagName != "avoid" && tagName != "avoidmob" && tagName != "mob")
					continue;

				foreach (XAttribute attr in element.Attributes())
				{
					string attrName = attr.Name.LocalName.ToLowerInvariant();
					switch (attrName)
					{
						case "name":
							string mobName = attr.Value.ToLower();
							if (!_avoidMobs.Contains(mobName))
								_avoidMobs.Add(mobName);
							break;
						case "id":
						case "entry":
							if (uint.TryParse(attr.Value, out uint mobId) && !_avoidMobs.Contains(mobId))
								_avoidMobs.Add(mobId);
							break;
					}
				}
			}
		}

		private void ParseBlackspots(XElement parent)
		{
			foreach (XElement element in parent.Elements())
			{
				string tagName = element.Name.LocalName.ToLowerInvariant();
				// HB accepts both <Spot> and <Blackspot>
				if (tagName != "blackspot" && tagName != "spot")
					continue;

				// HB default radius is 40, height is 10
				float x = 0, y = 0, z = 0, radius = 40, height = 10;

				foreach (XAttribute attr in element.Attributes())
				{
					string attrName = attr.Name.LocalName.ToLowerInvariant();
					switch (attrName)
					{
						case "x":
							float.TryParse(attr.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x);
							break;
						case "y":
							float.TryParse(attr.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out y);
							break;
						case "z":
							float.TryParse(attr.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out z);
							break;
						case "radius":
						case "rad":  // HB supports both "radius" and "rad"
							float.TryParse(attr.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out radius);
							break;
						case "height":
							float.TryParse(attr.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out height);
							break;
					}
				}

				_blackspots.Add(new Blackspot(new WoWPoint(x, y, z), radius, height));
			}
		}

		public List<Profile> GetScopeSortedProfiles()
		{
			List<Profile> result = new List<Profile> { this };
			foreach (Profile sub in SubProfiles)
			{
				result.AddRange(sub.GetScopeSortedProfiles());
			}
			result.Sort((a, b) => (b.MaxLevel - b.MinLevel).CompareTo(a.MaxLevel - a.MinLevel));
			return result;
		}

		public bool Equals(Profile? other)
		{
			if (other is null)
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return Name == other.Name && MinLevel == other.MinLevel && MaxLevel == other.MaxLevel;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as Profile);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Name, MinLevel, MaxLevel);
		}

		public static bool operator ==(Profile? left, Profile? right)
		{
			if (left is null)
				return right is null;
			return left.Equals(right);
		}

		public static bool operator !=(Profile? left, Profile? right)
		{
			return !(left == right);
		}
	}
}
