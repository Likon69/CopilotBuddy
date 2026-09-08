using System.Linq;
using Styx.Logic.Inventory.Frames.Gossip;

namespace Styx.Logic.POI
{
	/// <summary>
	/// Extension methods for PoiType.
	/// </summary>
	public static class PoiTypeExtensions
	{
		/// <summary>
		/// Converts a PoiType to its corresponding GossipEntryType.
		/// </summary>
		public static GossipEntry.GossipEntryType GetGossipType(this PoiType poiType)
		{
			switch (poiType)
			{
				case PoiType.Buy:
				case PoiType.Sell:
				case PoiType.Repair:
					return GossipEntry.GossipEntryType.Vendor;
				case PoiType.Train:
					return GossipEntry.GossipEntryType.Trainer;
				case PoiType.Mail:
					return GossipEntry.GossipEntryType.Gossip;
				case PoiType.Fly:
				{
					GossipFrame instance = GossipFrame.Instance;
					if (instance.IsVisible)
					{
						var entries = instance.GossipOptionEntries;
						if (entries == null || !entries.Any(e => e.Type == GossipEntry.GossipEntryType.Taxi))
							return GossipEntry.GossipEntryType.Gossip;
					}
					return GossipEntry.GossipEntryType.Taxi;
				}
				default:
					return GossipEntry.GossipEntryType.Unknown;
			}
		}
	}
}
