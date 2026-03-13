using System;
using System.Collections.Generic;
using System.Threading;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.WoWInternals.World;

namespace Styx.Logic
{
	public static class Mount
	{
		private static readonly WaitTimer _mountTimer = WaitTimer.TenSeconds;
		private static readonly WaitTimer _combatTimer = WaitTimer.TenSeconds;
		private static readonly List<WoWPoint> _cantMountSpots = new List<WoWPoint>();
		private static CanMountDelegate? _defaultCanMount;
		private static bool _wasMounted;
		private static LocationRetriever? _currentDestinationRetriever;

		/// <summary>
		/// Fired when the player mounts up (HB 4.3.4 compatibility).
		/// </summary>
		public static event EventHandler<MountUpEventArgs>? OnMountUp;

		/// <summary>
		/// Fired when the player dismounts (HB 4.3.4 compatibility).
		/// </summary>
		public static event EventHandler? OnDismount;

		private static LocalPlayer? Me => ObjectManager.Me;

		static Mount()
		{
			BotEvents.Player.OnMobKilled += OnMobKilled;
		}

		private static void OnMobKilled(BotEvents.Player.MobKilledEventArgs args)
		{
			_combatTimer.Reset();
		}

		public static void Dismount() => Dismount(string.Empty);

		public static void ClearShapeshift()
		{
			LocalPlayer? me = Me;
			if (me == null)
				return;

			if (me.Shapeshift != ShapeshiftForm.Normal)
				Lua.DoString("CancelShapeshiftForm()");
		}

		public static void Dismount(string reason)
		{
			LocalPlayer? me = Me;
			if (me == null) return;

			if (!string.IsNullOrEmpty(reason))
				Logging.WriteDebug("Stop and dismount. Reason: {0}", reason);

			ShapeshiftForm shapeshift = me.Shapeshift;
			if (me.Mounted || shapeshift == ShapeshiftForm.FlightForm || shapeshift == ShapeshiftForm.EpicFlightForm)
			{
				if (string.IsNullOrEmpty(reason))
					Logging.WriteDebug("Stop and dismount.");

				// BUG-04 fix: Descend safely before dismounting if flying
				if (me.IsFlying)
				{
					Logging.WriteDebug("Descending before dismount (flying safety).");
					WoWMovement.MoveStop();
					WoWMovement.Descend();
					int maxTicks = 300; // ~30 seconds max descent
					while (me.IsFlying && maxTicks-- > 0)
					{
						StyxWoW.Sleep(100);
					}
					WoWMovement.DescendStop();
					StyxWoW.Sleep(500); // Allow landing to settle
				}
				
				WoWMovement.MoveStop();

				if (shapeshift == ShapeshiftForm.FlightForm || shapeshift == ShapeshiftForm.EpicFlightForm)
				{
					Lua.DoString("CancelShapeshiftForm()");
				}
				else
				{
					Lua.DoString("Dismount()");
				}

				// HB 6.2.3: Fire OnDismount event after dismounting
				RaiseOnDismount(reason);
			}
		}

		/// <summary>
		/// HB 6.2.3 Mount.smethod_1: Safely raises OnDismount event,
		/// catching exceptions from individual subscribers.
		/// </summary>
		internal static void RaiseOnDismount(string? reason)
		{
			reason ??= string.Empty;
			EventHandler? handler = OnDismount;
			if (handler == null)
				return;

			foreach (Delegate d in handler.GetInvocationList())
			{
				try
				{
					d.DynamicInvoke(reason, EventArgs.Empty);
				}
				catch (Exception ex)
				{
					Logging.WriteException(ex);
				}
			}
		}

		private static readonly Random _random = new Random();

		/// <summary>
		/// Auto-detects and sets mount name if FindMountAutomatically is enabled.
		/// Ported from HB 4.3.4.
		/// </summary>
		public static void AutoDetectMount()
		{
			if (!CharacterSettings.Instance.UseMount || !CharacterSettings.Instance.FindMountAutomatically)
				return;

			if (CharacterSettings.Instance.UseRandomMount)
			{
				// Random mount selection
				var groundMounts = MountHelper.GroundMounts;
				if (groundMounts != null && groundMounts.Count > 0)
				{
					var mount = groundMounts[_random.Next(0, groundMounts.Count)];
					CharacterSettings.Instance.MountName = mount.CreatureSpellId.ToString();
					Logging.WriteDebug("Auto-detected random ground mount: {0}", mount.Name);
				}

				var flyingMounts = MountHelper.FlyingMounts;
				if (flyingMounts != null && flyingMounts.Count > 0)
				{
					var mount = flyingMounts[_random.Next(0, flyingMounts.Count)];
					CharacterSettings.Instance.FlyingMountName = mount.CreatureSpellId.ToString();
					Logging.WriteDebug("Auto-detected random flying mount: {0}", mount.Name);
				}
			}
			else
			{
				// Use first available mount if not set
				string mountName = CharacterSettings.Instance.MountName;
				if (string.IsNullOrEmpty(mountName) || mountName == "Mount Name Here" || mountName.Contains("Automatically detected"))
				{
					var groundMounts = MountHelper.GroundMounts;
					if (groundMounts != null && groundMounts.Count > 0)
					{
						var mount = groundMounts[0];
						CharacterSettings.Instance.MountName = mount.CreatureSpellId.ToString();
						Logging.WriteDebug("Auto-detected ground mount: {0}", mount.Name);
					}
				}

				string flyingMount = CharacterSettings.Instance.FlyingMountName;
				if (string.IsNullOrEmpty(flyingMount) || flyingMount.Contains("Automatically detected"))
				{
					var flyingMounts = MountHelper.FlyingMounts;
					if (flyingMounts != null && flyingMounts.Count > 0)
					{
						var mount = flyingMounts[0];
						CharacterSettings.Instance.FlyingMountName = mount.CreatureSpellId.ToString();
						Logging.WriteDebug("Auto-detected flying mount: {0}", mount.Name);
					}
				}
			}
		}

		public static void MountUp()
		{
			if (_defaultCanMount == null)
			{
				_defaultCanMount = DefaultCanMount;
			}
			MountUp(_defaultCanMount);
		}

		private static bool DefaultCanMount()
		{
			return true;
		}

		public static void MountUp(CanMountDelegate extra)
		{
			if (!extra())
				return;

			if (!LevelbotSettings.Instance.UseMount)
				return;

			// Auto-detect mount if enabled
			AutoDetectMount();

			if (string.IsNullOrEmpty(LevelbotSettings.Instance.MountName))
				return;

			LocalPlayer? me = Me;
			if (me == null || me.Level < 20)
				return;

			if (!CanMount())
				return;

			WoWMovement.MoveStop();
			Logging.Write("Mounting: {0}", LevelbotSettings.Instance.MountName);
			StyxWoW.Sleep(200);

			DoMount();
			_mountTimer.Reset();
		}

		private static void DoMount()
		{
			LocalPlayer? me = Me;
			if (me == null) return;

			string mountName = LevelbotSettings.Instance.MountName;
			if (string.IsNullOrEmpty(mountName)) return;

			// Handle Blood Elf Paladin mount name differences
			if (me.Race == WoWRace.BloodElf && me.Class == WoWClass.Paladin)
			{
				string lowerMount = mountName.ToLowerInvariant();
				if (lowerMount == "warhorse" || lowerMount == "summon warhorse")
				{
					mountName = "Summon Charger";
				}
				else if (lowerMount == "charger" || lowerMount == "summon charger")
				{
					mountName = "Summon Charger";
				}
			}

			Lua.DoString(string.Format("CallCompanion('MOUNT', {0})", GetMountIndex(mountName)));

			int startTime = Environment.TickCount;
			string lastError = me.LastRedErrorMessage;

			while (!me.Mounted && Environment.TickCount - startTime < 6500)
			{
				if (me.Combat)
					break;

				if (!string.IsNullOrEmpty(lastError) && me.LastRedErrorMessage != lastError)
				{
					AddCantMountSpot(me.Location);
					break;
				}

				StyxWoW.Sleep(250);
			}
		}

		private static int GetMountIndex(string mountName)
		{
			// Use Lua to find mount index
			string luaCode = string.Format(@"
				local mountName = string.lower('{0}')
				for i = 1, GetNumCompanions('MOUNT') do
					local _, name, id = GetCompanionInfo('MOUNT', i)
					if string.lower(name) == mountName or tostring(id) == mountName then
						return i
					end
				end
				return 0
			", mountName.Replace("'", "\\'"));

			string result = Lua.GetReturnVal<string>(luaCode, 0);
			if (int.TryParse(result, out int index))
			{
				return index;
			}
			return 0;
		}

		public static bool CanMount()
		{
			LocalPlayer? me = Me;
			if (me == null)
				return false;

			// Check if player can use mounts at all
			// WotLK: Ground mounts at level 20 (except Paladin/Warlock at 20)
			int requiredLevel = 20;
			if (me.Level < requiredLevel)
				return false;

			// Check if player has any mounts available
			if (MountHelper.NumMounts <= 0)
				return false;

			if (!_combatTimer.IsFinished)
				return false;

			if (!_mountTimer.IsFinished)
				return false;

			if (me.Dead || me.IsGhost)
				return false;

			WoWPoint location = me.Location;

			// Check if we're in a known "can't mount" spot
			foreach (WoWPoint spot in _cantMountSpots)
			{
				if (location.Distance(spot) < 10f)
					return false;
			}

			bool canMount = me.IsOutdoors && !me.IsSwimming && !me.Combat;

			if (!canMount)
			{
				AddCantMountSpot(location);
				return false;
			}

			// HB 4.3.4 ceiling raycast — prevent mount attempts in low-ceiling areas
			float boundingHeight = me.BoundingHeight;
			WoWPoint headPos = location + new WoWPoint(0f, 0f, boundingHeight);
			WoWPoint aboveHead = headPos + new WoWPoint(0f, 0f, boundingHeight / 2f);
			if (GameWorld.TraceLine(headPos, aboveHead, GameWorld.CGWorldFrameHitFlags.HitTestLOS))
			{
				AddCantMountSpot(location);
				return false;
			}

			return true;
		}

		public static bool IsOutdoors
		{
			get
			{
				LocalPlayer? me = Me;
				return me?.IsOutdoors ?? false;
			}
		}

		public static void AddCantMountSpot(WoWPoint location)
		{
			if (!_cantMountSpots.Contains(location))
			{
				_cantMountSpots.Add(location);
				Logging.WriteDebug("Added can't mount spot at: {0}", location);
			}
		}

		public static void ClearCantMountSpots()
		{
			_cantMountSpots.Clear();
		}

		public static void StateMount(LocationRetriever travelingTo)
		{
			if (!LevelbotSettings.Instance.UseMount || Me?.Mounted == true || !CanMount())
				return;

			MountUp(travelingTo);
		}

		public static void MountUp(LocationRetriever travelingTo)
		{
			_currentDestinationRetriever = travelingTo;
			MountUp(() =>
			{
				WoWUnit? firstUnit = Targeting.Instance.FirstUnit;
				if (firstUnit != null && firstUnit.Distance < MountDistance)
					return false;

				return true;
			});
		}

		public static bool ShouldMount(WoWPoint travelingTo)
		{
			LocalPlayer? me = Me;
			if (me == null)
				return false;

			if (Battlegrounds.IsInsideBattleground || me.IsInInstance)
				return true;

			float distanceSqr = me.Location.DistanceSqr(travelingTo);
			float mountDistanceSqr = MountDistance * MountDistance;

			return distanceSqr >= mountDistanceSqr;
		}

		/// <summary>
		/// Check if we should dismount for a given destination.
		/// Ported from HB 4.3.4.
		/// </summary>
		public static bool ShouldDismount(WoWPoint travelingTo)
		{
			LocalPlayer? me = Me;
			if (me == null)
				return false;

			if (!me.Mounted)
				return false;

			// Dismount if in combat and not moving
			if (me.Combat && !me.IsMoving)
			{
				Logging.WriteDebug("Dismount for attacker.");
				return true;
			}

			if (travelingTo == WoWPoint.Empty)
				return false;

			WoWPoint location = me.Location;
			float distance = location.Distance(travelingTo);

			// If at a hotspot and there's a target nearby
			if (BotPoi.Current.Type == PoiType.Hotspot)
			{
				if (distance <= 100f && Targeting.Instance.FirstUnit != null)
				{
					Logging.WriteDebug("Dismount to pull near hotspot.");
					return true;
				}
			}

			// If at a kill POI and we're close
			if (BotPoi.Current.Type == PoiType.Kill)
			{
				if (distance <= CharacterSettings.Instance.PullDistance)
				{
					Logging.WriteDebug("Dismount to kill bot poi.");
					return true;
				}
			}

			// Dismount for interacting with objects/NPCs
			if (BotPoi.Current.Type == PoiType.Loot || 
				BotPoi.Current.Type == PoiType.Skin ||
				BotPoi.Current.Type == PoiType.Harvest ||
				BotPoi.Current.Type == PoiType.Sell ||
				BotPoi.Current.Type == PoiType.Repair ||
				BotPoi.Current.Type == PoiType.Train ||
				BotPoi.Current.Type == PoiType.Mail)
			{
				if (distance <= 10f)
				{
					Logging.WriteDebug("Dismount for interaction.");
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Pulses mount state and fires events (call from main bot pulse).
		/// </summary>
		public static void Pulse()
		{
			var me = Me;
			if (me == null) return;

			bool isMounted = me.Mounted;

			if (isMounted && !_wasMounted)
			{
				// Just mounted — fire event and check Cancel flag (HB 6.2.3 pattern)
				var args = new MountUpEventArgs(me.IsFlying, "Mount");
				args.Destination = _currentDestinationRetriever?.Invoke() ?? WoWPoint.Empty;
				OnMountUp?.Invoke(null, args);
				if (args.Cancel)
				{
					Logging.WriteDebug("Mount-up cancelled by event handler");
					Dismount("cancelled by event handler");
					_wasMounted = false;
					return;
				}
			}
			else if (!isMounted && _wasMounted)
			{
				// Just dismounted
				OnDismount?.Invoke(null, EventArgs.Empty);
			}

			_wasMounted = isMounted;
		}

		public static float MountDistance => (float)LevelbotSettings.Instance.MountDistance;

		public delegate bool CanMountDelegate();
	}
}
