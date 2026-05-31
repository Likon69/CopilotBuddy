using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;

namespace CommonBehaviors.Actions
{
	/// <summary>
	/// HB MoP/WoD style ActionMoveToPoi with anti-spam logging.
	/// Tracks last target GUID and location to avoid spamming logs.
	/// </summary>
	public class ActionMoveToPoi : NavigationAction
	{
		private WoWPoint _lastLocation = WoWPoint.Empty;
		private ulong _lastGuid;
		private bool _hasLoggedMove;

		protected override RunStatus Run(object context)
		{
			BotPoi botPoi = BotPoi.Current;

			if (botPoi.Location == WoWPoint.Zero)
			{
				Logging.Write("I don't want to move to (0, 0, 0).");
				_hasLoggedMove = false;
				return RunStatus.Failure;
			}

			// HB MoP/WoD: Track target unit for moving targets
			WoWObject? asObject = botPoi.AsObject;
			WoWUnit? unit = asObject?.ToUnit();
			bool targetChanged = false;

			if (unit != null)
			{
				ulong guid = unit.Guid;
				WoWPoint location = unit.Location;

				// If target is moving, update location only if significantly changed
				if (unit.IsMoving)
				{
					LocalPlayer? me = ObjectManager.Me;
					if (_lastLocation == WoWPoint.Empty || _lastGuid != guid || 
					    (me != null && _lastLocation.DistanceSqr(me.Location) < 900f))
					{
						targetChanged = (_lastGuid != guid);
						_lastGuid = guid;
						_lastLocation = location;
					}
				}
				else
				{
					// Target stopped, update if changed
					if (_lastGuid != guid || _lastLocation != location)
					{
						targetChanged = (_lastGuid != guid);
						_lastGuid = guid;
						_lastLocation = location;
					}
				}
			}
			else
			{
				// No unit target, use POI location
				if (_lastGuid != 0UL || _lastLocation != botPoi.Location)
				{
					targetChanged = true;
				}
				_lastGuid = 0UL;
				_lastLocation = botPoi.Location;
			}

			// Log only once when target changes (not every tick)
			if (targetChanged || !_hasLoggedMove)
			{
				Logging.Write("Moving to {0}", BotPoi.Current);
				_hasLoggedMove = true;
			}

			// Mount if needed
			float precision = 40f;
			switch (botPoi.Type)
			{
				case PoiType.Hotspot:
				case PoiType.Kill:
					precision = 15f;
					break;
				case PoiType.Loot:
				case PoiType.Skin:
				case PoiType.Harvest:
					precision = 4.5f;  // Close enough to interact
					break;
				case PoiType.Quest:
				case PoiType.QuestPickUp:
				case PoiType.QuestTurnIn:
					precision = 5f;  // Quest interactions need close range
					break;
				case PoiType.Sell:
				case PoiType.Buy:
				case PoiType.Mail:
				case PoiType.Repair:
				case PoiType.Train:  // Trainer needs close range like vendors
				case PoiType.Fly:    // Flight master needs close range
					precision = 4f;  // Close enough to interact
					break;
			}

			// HB 6.2.3 pattern: movement always goes through Flightor, which dispatches to
			// Navigator internally when ground nav is needed (ShouldWalk / IsInNoFlyZone).
			// FlightPaths still owns flight-master POIs; Flightor must not override that.
			Flightor.MoveTo(_lastLocation);
			return RunStatus.Success;
		}
	}
}
