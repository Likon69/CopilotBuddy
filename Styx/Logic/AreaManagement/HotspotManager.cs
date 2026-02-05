using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;
using Styx.WoWInternals;

namespace Styx.Logic.AreaManagement
{
	/// <summary>
	/// Manages hotspots for grinding/questing.
	/// </summary>
	public class HotspotManager
	{
		private readonly CircularQueue<WoWPoint> _hotspotQueue;
		private readonly Dictionary<WoWPoint, DateTime> _blacklistedPoints = new Dictionary<WoWPoint, DateTime>();
		private readonly Random _random = new Random();
		private static WoWPoint _currentHotspot;
		private static Profile? _cachedProfile;
		private static WoWPoint _lastHotspot;
		private static Stopwatch? _timer;

		public List<WoWPoint> Hotspots { get; private set; }

		public HotspotManager(IEnumerable<WoWPoint> points)
		{
			Hotspots = points.ToList();
			_hotspotQueue = new CircularQueue<WoWPoint>();
			Hotspots.ForEach(_hotspotQueue.Enqueue);
			CycleToNearest();
		}

		public HotspotManager(XElement element)
		{
			if (Timer == null)
			{
				Timer = new Stopwatch();
			}
			Hotspots = new List<WoWPoint>();
			List<XElement> list = element.Elements().ToList();
			foreach (XElement xelement in list)
			{
				try
				{
					if (xelement.Name == "Hotspot")
					{
						try
						{
							float? xCoord = null;
							float? yCoord = null;
							float? zCoord = null;
							XAttribute[] array = xelement.Attributes().ToArray();
							foreach (XAttribute xattribute in array)
							{
								try
								{
									string? text;
									if ((text = xattribute.Name.ToString().ToUpper()) != null)
									{
										if (text == "X")
										{
											if (!float.TryParse(xattribute.Value, out float parsedX))
											{
												throw new ProfileAttributeExpectedException<float>(xattribute);
											}
											xCoord = parsedX;
										}
										else if (text == "Y")
										{
											if (!float.TryParse(xattribute.Value, out float parsedY))
											{
												throw new ProfileAttributeExpectedException<float>(xattribute);
											}
											yCoord = parsedY;
										}
										else if (text == "Z")
										{
											if (xCoord == null || yCoord == null)
											{
												throw new ProfileException("You have placed the 'Z' attribute before the 'X' and 'Y' attribute!");
											}
											if (!float.TryParse(xattribute.Value, out float parsedZ))
											{
												throw new ProfileAttributeExpectedException<float>(xattribute);
											}
											zCoord = parsedZ;
										}
										else
										{
											throw new ProfileUnknownAttributeException(xattribute, "X", "Y", "Z");
										}
									}
									else
									{
										throw new ProfileUnknownAttributeException(xattribute, "X", "Y", "Z");
									}
								}
								catch (ProfileException ex)
								{
									Logging.WriteException(ex);
								}
							}
							if (xCoord == null || yCoord == null || zCoord == null)
							{
								throw new ProfileMissingAttributeException<float>((xCoord != null) ? ((yCoord != null) ? "Z" : "Y") : "X", xelement);
							}
							Hotspots.Add(new WoWPoint(xCoord.Value, yCoord.Value, zCoord.Value));
							continue;
						}
						catch (ProfileException ex2)
						{
							Logging.WriteException(ex2);
							continue;
						}
					}
					throw new ProfileUnknownElementException(xelement, "Hotspot");
				}
				catch (ProfileException ex3)
				{
					Logging.WriteException(ex3);
				}
			}
			_hotspotQueue = new CircularQueue<WoWPoint>();
			Hotspots.ForEach(_hotspotQueue.Enqueue);
			CycleToNearest();
		}

		static HotspotManager()
		{
			_currentHotspot = WoWPoint.Zero;
		}

		public Dictionary<WoWPoint, DateTime> Blacklist
		{
			get
			{
				CleanupBlacklist();
				return _blacklistedPoints;
			}
		}

		public void CycleToNearest()
		{
			WoWPoint woWPoint = WoWPoint.Zero;
			float shortestDistance = float.MaxValue;
			foreach (WoWPoint woWPoint2 in Hotspots)
			{
				float currentDistance = woWPoint2.Distance(ObjectManager.Me.Location);
				if (currentDistance < shortestDistance)
				{
					shortestDistance = currentDistance;
					woWPoint = woWPoint2;
				}
			}
			if (woWPoint != WoWPoint.Zero)
			{
				_hotspotQueue.CycleTo(woWPoint);
			}
		}

		public WoWPoint GetNextHotspot()
		{
			if (_hotspotQueue.Count <= 0)
			{
				return WoWPoint.Zero;
			}
			return _hotspotQueue.Dequeue();
		}

		public WoWPoint GetRandomHotspot()
		{
			List<WoWPoint> list = Hotspots.Where(pnt => !Blacklist.ContainsKey(pnt)).ToList();
			if (list.Count <= 0)
			{
				return WoWPoint.Zero;
			}
			return list[_random.Next(0, list.Count)];
		}

		public void BlacklistPoint(WoWPoint pnt, DateTime expiration)
		{
			if (!_blacklistedPoints.ContainsKey(pnt))
			{
				_blacklistedPoints.Add(pnt, expiration);
			}
			else
			{
				_blacklistedPoints[pnt] = expiration;
			}
		}

		private void CleanupBlacklist()
		{
			DateTime now = DateTime.Now;
			foreach (KeyValuePair<WoWPoint, DateTime> keyValuePair in _blacklistedPoints)
			{
				if (now > keyValuePair.Value)
				{
					_blacklistedPoints.Remove(keyValuePair.Key);
				}
			}
		}

		public static WoWPoint CurrentHotSpot
		{
			get
			{
				try
				{
					UpdateCurrentHotspot();
				}
				catch (UserException ex)
				{
					Logging.Write(ex.Message);
				}
				catch (Exception ex2)
				{
					Logging.WriteException(ex2);
				}
				return _currentHotspot;
			}
		}

		public static WoWPoint LastHotSpot
		{
			get { return _lastHotspot; }
			set { _lastHotspot = value; }
		}

		public static Stopwatch? Timer
		{
			get { return _timer; }
			set { _timer = value; }
		}

		private static void UpdateCurrentHotspot()
		{
			if (ProfileManager.CurrentOuterProfile == null)
			{
				throw new UserException("No profile is loaded!");
			}
			HotspotManager? hotspotManager = ProfileManager.CurrentProfile?.HotspotManager;
			if (hotspotManager == null)
			{
				Logging.Write("No suitable hotspots in {0}.", ProfileManager.CurrentProfile?.Name ?? "profile");
				throw new UserException("No hotspots have been defined!");
			}
			if (ProfileManager.CurrentProfile != _cachedProfile)
			{
				_cachedProfile = ProfileManager.CurrentProfile;
				LastHotSpot = _currentHotspot;
				_currentHotspot = hotspotManager.GetNextHotspot();
				Timer?.Reset();
			}
			TimeSpan elapsed = Timer?.Elapsed ?? TimeSpan.Zero;
			if (elapsed.TotalMinutes > 5.0 || (LastHotSpot == _currentHotspot && Targeting.Instance.TargetList.Count == 0))
			{
				LastHotSpot = _currentHotspot;
				_currentHotspot = hotspotManager.GetNextHotspot();
				Timer?.Reset();
			}
		}
	}
}
