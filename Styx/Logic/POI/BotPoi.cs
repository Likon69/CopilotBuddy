using System;
using System.Linq;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;
using Styx.Logic.Profiles.Quest;
using Styx.Logic.Questing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Styx.Logic.POI
{
	public class BotPoi
	{
		private static BotPoi _current;
		private WoWObject? _asObject;
		private object? _object0;
		private WoWPoint _location;

		private void OnObjectInvalidated()
		{
			if (_asObject != null)
			{
				_asObject.OnInvalidate -= OnObjectInvalidated;
				_asObject = null;
			}
		}

		static BotPoi()
		{
			_current = new BotPoi(PoiType.None);
			BotEvents.Player.OnPlayerDied += OnPlayerDied;
		}

		private static void OnPlayerDied()
		{
			Clear();
		}

		public BotPoi(PoiType type)
		{
			Type = type;
		}

		public BotPoi(WoWObject obj, PoiType type)
			: this(type)
		{
			if (obj == null || obj.BaseAddress == 0U)
			{
				Type = PoiType.None;
			}
			else
			{
				try
				{
					Name = obj.Name;
					Guid = obj.Guid;
					Entry = obj.Entry;
					Location = obj.Location;
					_asObject = obj;
					obj.OnInvalidate += OnObjectInvalidated;
				}
				catch (Exception ex)
				{
					Logging.WriteException(ex);
					Name = null;
					Guid = 0UL;
					Entry = 0U;
					Type = PoiType.None;
					_asObject = null;
				}
			}
		}

		public BotPoi(WoWPoint location, PoiType type)
			: this(type)
		{
			Location = location;
		}

		public BotPoi(Quest quest)
			: this(PoiType.Quest)
		{
			Entry = quest.Id;
			Name = quest.Name;
			_object0 = quest;
		}

		public BotPoi(Mailbox mailbox)
			: this(PoiType.Mail)
		{
			Location = mailbox.Location;
			_object0 = mailbox;
		}

		public BotPoi(Vendor vendor, PoiType type)
		{
			Type = type;
			Name = vendor.Name;
			Entry = (uint)vendor.Entry;
			Location = vendor.Location;
			_object0 = vendor;
		}

		public BotPoi(PickUpNode pickUp)
			: this(PoiType.QuestPickUp)
		{
			_object0 = pickUp;
			Entry = pickUp.GiverId;
			Location = pickUp.GiverLocation;
			Name = pickUp.GiverName;
		}

		public BotPoi(TurnInNode turnIn)
			: this(PoiType.QuestTurnIn)
		{
			_object0 = turnIn;
			Entry = turnIn.TurnInId;
			Name = turnIn.TurnInName;
		}

		public static BotPoi Current
		{
			get { return _current; }
			set
			{
				if (_current != value)
				{
					_current = value ?? new BotPoi(PoiType.None);
					if (_current.Type == PoiType.None)
					{
						Logging.WriteDebug("Cleared POI");
					}
					else
					{
						Logging.WriteDebug("Changed POI to: {0}", value);
					}
				}
			}
		}

		public PoiType Type { get; set; }

		public string? Name { get; set; }

		public ulong Guid { get; set; }

		public uint Entry { get; set; }

		public WoWPoint Location
		{
			get
			{
				switch (Type)
				{
					case PoiType.Quest:
					case PoiType.QuestTurnIn:
					case PoiType.QuestPickUp:
					case PoiType.Kill:
					case PoiType.Loot:
					case PoiType.Skin:
					case PoiType.Buy:
					case PoiType.Sell:
					case PoiType.Repair:
					case PoiType.Train:
					{
						try
						{
							WoWObject? asObject = AsObject;
							if (asObject != null && asObject.IsValid)
							{
								_location = asObject.Location;
							}
						}
						catch (Exception)
						{
							// Object despawned — use cached _location
						}
						return _location;
					}
				}
				return _location;
			}
			set
			{
				_location = value;
			}
		}

		public WoWObject? AsObject
		{
			get
			{
				// HB 4.3.4 style: Always re-search if _asObject is null (don't cache null)
				if (_asObject == null || !_asObject.IsValid)
				{
					_asObject = null; // Reset if invalid
					
					try
					{
					switch (Type)
					{
						case PoiType.Buy:
						case PoiType.Sell:
						case PoiType.Repair:
						case PoiType.Train:
						case PoiType.Fly:
							// For vendor POIs from profile, search by Entry in ObjectManager
							// Use _location directly to avoid recursion with Location property
							if (_object0 is Vendor || Entry > 0)
							{
							_asObject = ObjectManager.CachedUnits
								.Where(u => u.IsValid && u.Entry == Entry && u.Location.Distance(_location) < 50)
								.FirstOrDefault();
							}
							break;
							
						case PoiType.Mail:
							// Find nearest mailbox
							_asObject = ObjectManager.ObjectList
								.OfType<WoWGameObject>()
								.Where(g => g.IsValid && g.SubType == WoWGameObjectType.Mailbox)
								.OrderBy(g => g.DistanceSqr)
								.FirstOrDefault();
							break;
							
						case PoiType.QuestPickUp:
						case PoiType.QuestTurnIn:
							// Search for unit or gameobject by entry
							_asObject = ObjectManager.ObjectList
								.Where(o => (o is WoWUnit || o is WoWGameObject) && o.IsValid)
								.FirstOrDefault(o => o.Entry == Entry);
							break;
						
						case PoiType.Kill:
						case PoiType.Loot:
						case PoiType.Skin:
							// For kill/loot/skin, search by GUID first, then by Entry
							if (Guid != 0UL)
							{
								_asObject = ObjectManager.GetObjectByGuid<WoWObject>(Guid);
								if (_asObject != null && !_asObject.IsValid)
									_asObject = null;
							}
							if (_asObject == null && Entry > 0)
							{
								_asObject = ObjectManager.ObjectList
									.Where(o => (o is WoWUnit || o is WoWGameObject) && o.IsValid)
									.FirstOrDefault(o => o.Entry == Entry);
							}
							break;
							
						default:
							// Default: search by GUID if we have one
							if (Guid != 0UL)
							{
								_asObject = ObjectManager.GetObjectByGuid<WoWObject>(Guid);
								if (_asObject != null && !_asObject.IsValid)
									_asObject = null;
							}
							break;
					}
					}
					catch (Exception)
					{
						// Object may have despawned during LINQ enumeration (stale descriptor read)
						_asObject = null;
					}
				}
				if (_asObject != null)
				{
					_asObject.OnInvalidate += OnObjectInvalidated;
				}
				return _asObject;
			}
		}

		public WoWUnit? AsUnit => AsObject as WoWUnit;

		public WoWPlayer? AsPlayer => AsObject as WoWPlayer;

		public WoWGameObject? AsGameObject => AsObject as WoWGameObject;

		public WoWItem? AsItem => AsObject as WoWItem;

		public Vendor? AsVendor => _object0 as Vendor;

		public Mailbox? AsMailbox => _object0 as Mailbox;

		public PickUpNode? AsPickUp => _object0 as PickUpNode;

		public TurnInNode? AsTurnIn => _object0 as TurnInNode;

		public Quest? AsQuest => _object0 as Quest;

		public static void Clear(string reason = "")
		{
			if (!string.IsNullOrEmpty(reason))
			{
				Logging.WriteDebug("Cleared POI - Reason {0}", reason);
			}
			// HB 4.3.4 order: Navigator → Current → FlightPaths
			Pathing.Navigator.Clear();
			Current = new BotPoi(PoiType.None);
			FlightPaths.Reset();
		}

		public double DistanceToPoi
		{
			get
			{
				if (ObjectManager.Me == null)
					return double.MaxValue;
				return ObjectManager.Me.Location.Distance(Location);
			}
		}

		public override string ToString()
		{
			return string.Format("Type: {0}, Name: {1}, Guid: {2:X}, Entry: {3}, Location: {4}",
				Type, Name ?? "Unknown", Guid, Entry, Location);
		}
	}
}
