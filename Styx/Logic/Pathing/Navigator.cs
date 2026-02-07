using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Styx.Helpers;
using Styx.Logic.Pathing.Interop;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TripperNav = Tripper.Navigation;

namespace Styx.Logic.Pathing
{
	public static class Navigator
	{
		private static WoWPoint _destination = WoWPoint.Zero;
		private static readonly List<WoWPoint> _currentPath = new List<WoWPoint>();
		private static int _currentPathIndex;
		private static TripperNav.Navigator? _navigator;
		private static IMover? _playerMover;
		private static IStuckHandler? _stuckHandler;
		private static WaitTimer _stuckCheckTimer = new WaitTimer(TimeSpan.FromSeconds(2));
		private static WaitTimer _doorInteractTimer = new WaitTimer(TimeSpan.FromSeconds(2)); // Cooldown between door interactions
		private static WaitTimer _pathRegenThrottle = new WaitTimer(TimeSpan.FromMilliseconds(500)); // Prevent path regen spam

		// Path metadata — stored alongside _currentPath to support off-mesh, terrain type, and ability checks
		private static TripperNav.StraightPathFlags[]? _currentFlags;
		private static TripperNav.AreaType[]? _currentPolyTypes;
		private static TripperNav.AbilityFlags[]? _currentAbilityFlags;
		private static bool _elevatorBoarded; // Tracks elevator state (HB 4.3.4 pattern)
		private static bool _ridingElevator; // True while actively riding an elevator (blocks mount-up)
		private static int _unstickAttempts; // AUDIT FIX: Max retry counter to prevent infinite unstick loops
		private const int MaxUnstickAttempts = 5; // After 5 failed unsticks, force path regeneration

		// WotLK no-fly zone IDs — areas where flying is forbidden or problematic
		private static readonly HashSet<uint> _noFlyZoneIds = new HashSet<uint>
		{
			4395, // Dalaran city (no flying allowed)
			4613, // The Pit of Saron (indoor dungeon entrance area)
			4820, // Halls of Reflection
		};

		public static float PathPrecision { get; set; } = 2.0f;
		public static int LoadTilesAroundRadius { get; set; } = 2;
		public static float FlyingMountHeight { get; set; } = 25f;

		/// <summary>
		/// Gets or sets the player mover used for movement control.
		/// </summary>
		public static IMover PlayerMover
		{
			get
			{
				_playerMover ??= new LocalPlayerMover();
				return _playerMover;
			}
			set => _playerMover = value;
		}

		/// <summary>
		/// Gets or sets the stuck handler used for stuck detection and recovery.
		/// </summary>
		public static IStuckHandler StuckHandler
		{
			get
			{
				_stuckHandler ??= new StuckHandler();
				return _stuckHandler;
			}
			set => _stuckHandler = value;
		}

		public static WoWPoint Destination => _destination;

		public static List<WoWPoint> CurrentPath => _currentPath;

		public static bool AtLocation => _destination != WoWPoint.Zero &&
			ObjectManager.Me != null &&
			ObjectManager.Me.Location.Distance(_destination) < PathPrecision;

		/// <summary>
		/// Gets the current map ID from the local player.
		/// </summary>
		private static uint GetCurrentMapId()
		{
			LocalPlayer? me = ObjectManager.Me;
			return me?.MapId ?? 0;
		}

		/// <summary>
		/// Gets or creates the Tripper navigator instance.
		/// </summary>
		public static TripperNav.Navigator TripperNavigator
		{
			get
			{
				if (_navigator == null)
				{
					_navigator = new TripperNav.Navigator();
					_navigator.LogMessage += msg => Logging.WriteDebug("[Tripper] {0}", msg);
				}
				return _navigator;
			}
		}

		/// <summary>
		/// Indicates if navigation meshes are loaded and ready.
		/// </summary>
		public static bool IsNavigatorLoaded => _navigator?.IsLoaded ?? false;

		static Navigator()
		{
			Logging.WriteDebug("[Navigator] Static constructor called - subscribing to events");
			BotEvents.Player.OnMapChanged += OnMapChanged;
			BotEvents.OnBotStart += OnBotStart;
			BotEvents.OnBotStop += OnBotStop;

			// HB 6.2.3 pattern: cancel mount-up while riding elevator
			Mount.OnMountUp += OnMountUpDuringElevator;
		}

		/// <summary>
		/// Prevents mounting while riding an elevator (HB 6.2.3 MeshNavigator.method_17).
		/// </summary>
		private static void OnMountUpDuringElevator(object? sender, MountUpEventArgs e)
		{
			if (_ridingElevator)
			{
				e.Cancel = true;
				Logging.WriteDebug("[Navigator] Cancelled mount-up while riding elevator");
			}
		}

		/// <summary>
		/// Checks if the current zone is a no-fly zone (P6.10 — Dalaran etc.).
		/// </summary>
		public static bool IsInNoFlyZone
		{
			get
			{
				var me = ObjectManager.Me;
				if (me == null) return false;
				return _noFlyZoneIds.Contains(me.ZoneId);
			}
		}

		/// <summary>
		/// Gets whether the player is currently riding an elevator (blocks mount-up).
		/// HB 6.2.3 pattern: MeshNavigator.method_17 cancels mount while on transport.
		/// </summary>
		public static bool IsRidingElevator => _ridingElevator;

		private static void OnMapChanged(BotEvents.Player.MapChangedEventArgs args)
		{
			Clear();
			Logging.WriteDebug("[Navigator] Map changed. Path cleared.");
		}

		private static void OnBotStart(EventArgs args)
		{
			Logging.WriteDebug("[Navigator] OnBotStart event received");
			Clear();
			// Load navigation meshes on bot start
			if (_navigator == null)
			{
				Logging.WriteDebug("[Navigator] Creating new Tripper.Navigator instance");
				_navigator = new TripperNav.Navigator();
				_navigator.LogMessage += msg => Logging.WriteDebug("[Tripper] {0}", msg);
			}
			if (!_navigator.IsLoaded)
			{
				Logging.Write("[Navigator] Loading navigation meshes...");
				try
				{
					if (_navigator.LoadMeshes())
					{
						Logging.Write("[Navigator] Navigation meshes loaded successfully.");
					}
					else
					{
						Logging.Write(LogLevel.Quiet, "[Navigator] Failed to load navigation meshes. Using direct movement.");
					}
				}
				catch (Exception ex)
				{
					Logging.Write(LogLevel.Quiet, "[Navigator] Exception loading navigation meshes: {0}", ex.Message);
				}
			}
			else
			{
				Logging.WriteDebug("[Navigator] Navigation meshes already loaded");
			}

			// Set faction-aware query filter (HB 6.2.3 pattern: OnBotStarted → SetFactionQueryFilter)
			// Excludes opposite faction's paths and applies 50x cost penalty on their areas
			if (IsNavigatorLoaded && _navigator != null)
			{
				try
				{
					var me = StyxWoW.Me;
					if (me != null)
					{
						_navigator.SetFactionQueryFilter(me.IsHorde);
					}
				}
				catch (Exception ex)
				{
					Logging.WriteDebug("[Navigator] Failed to set faction filter: {0}", ex.Message);
				}
			}

			// Initialize flight path system (HB 4.3.4 Class448 startup pattern):
			// Loads saved XmlFlightNode database and attaches TAXIMAP_OPENED Lua event handler.
			// Without this call, P6.9 flight path auto-detection is non-functional.
			try
			{
				FlightPaths.Initialize();
			}
			catch (Exception ex)
			{
				Logging.WriteDebug("[Navigator] Failed to initialize FlightPaths: {0}", ex.Message);
			}
		}

		private static void OnBotStop(EventArgs args)
		{
			Clear();
		}

		public static void Clear()
		{
			// When we clear navigation (e.g. after an unstick), also reset stuck detection state.
			// Otherwise, the stuck logic can immediately re-trigger on the next MoveTo call.
			try
			{
				StuckHandler.Reset();
			}
			catch
			{
				// Ignore
			}

			_destination = WoWPoint.Zero;
			_currentPath.Clear();
			_currentPathIndex = 0;
			_currentFlags = null;
			_currentPolyTypes = null;
			_currentAbilityFlags = null;
			_elevatorBoarded = false;
			_ridingElevator = false;
			_unstickAttempts = 0;
			_doorCenterTarget = WoWPoint.Zero;
			_pathRegenThrottle = new WaitTimer(TimeSpan.FromMilliseconds(500)); // Reset so next path gen is immediate
			WoWMovement.MoveStop();
		}

		public static MoveResult MoveTo(WoWPoint destination)
		{
			return MoveTo(destination, "Navigation");
		}

		public static MoveResult MoveTo(WoWPoint destination, string destinationName)
		{
			return MoveTo(destination, PathPrecision, destinationName);
		}

		public static MoveResult MoveTo(WoWPoint destination, float precision)
		{
			return MoveTo(destination, precision, "Navigation");
		}

		public static MoveResult MoveTo(WoWPoint destination, float precision, string destinationName)
		{
			if (destination == WoWPoint.Zero)
				return MoveResult.Failed;

			// Check for NaN coordinates - would crash WoW with INT_DIVIDE_BY_ZERO
			if (float.IsNaN(destination.X) || float.IsNaN(destination.Y) || float.IsNaN(destination.Z))
			{
				Logging.WriteDebug("[Navigator] ERROR: Destination contains NaN coordinates, aborting movement");
				return MoveResult.Failed;
			}

			LocalPlayer? me = ObjectManager.Me;
			if (me == null)
				return MoveResult.Failed;

			// Check if we're already at the destination
			float distance = me.Location.Distance(destination);
			if (distance < precision)
			{
				_destination = WoWPoint.Zero;
				_currentPath.Clear();
				return MoveResult.ReachedDestination;
			}

			// Auto-mount check (HB 6.2.3 MeshNavigator L231):
			// If distance >= MountDistance and we can mount, mount up before path following.
			// This ensures all callers (quest behaviors, plugins, etc.) get auto-mounting.
			if (Mount.ShouldMount(destination))
			{
				WoWPoint dest = destination;
				Mount.StateMount(() => dest);
			}

			// Auto-dismount near destination (HB pattern):
			// Dismount when close to interaction POIs (loot, vendor, quest NPC, etc.)
			// to avoid running into NPCs mounted and failing to interact.
			if (Mount.ShouldDismount(destination))
			{
				Mount.Dismount("Near destination");
			}

			// P6.14 — Combat abort: If we're in combat, skip expensive pathfinding.
			// In synchronous mode we can't abort mid-pathfind, but we CAN short-circuit
			// before calling FindPath() and use direct click-to-move instead.
			// This lets the combat routine take over faster (HB 6.2.3 equivalent of
			// the 4-second combat abort timeout in method_16).
			if (me.Combat && _currentPath.Count == 0)
			{
				// In combat with no existing path — move directly toward target
				// to close distance or flee, without wasting time on pathfinding.
				WoWMovement.ClickToMove(destination);
				return MoveResult.Moved;
			}

			// If destination changed, generate new path (with throttle to prevent regen spam)
			if (_destination != destination)
			{
				if (!_pathRegenThrottle.IsFinished)
				{
					// Too soon since last path generation — use direct movement as fallback
					WoWMovement.ClickToMove(destination);
					return MoveResult.Moved;
				}
				_pathRegenThrottle.Reset();

				_destination = destination;
				_currentPath.Clear();
				_stuckCheckTimer.Reset();
				_unstickAttempts = 0;

				// New destination implies a new movement attempt; reset stuck state.
				try
				{
					StuckHandler.Reset();
				}
				catch
				{
					// Ignore
				}

				// P6.14 — Combat abort: Skip pathfinding entirely if in combat.
				// Direct click-to-move is sufficient for combat movement (kiting, chasing).
				if (me.Combat)
				{
					_currentPath.Add(me.Location);
					_currentPath.Add(destination);
					_currentFlags = null;
					_currentPolyTypes = null;
					_currentAbilityFlags = null;
					_currentPathIndex = 0;
					Logging.WriteDebug("[Navigator] In combat — using direct movement to: {0}", destinationName);
					WoWMovement.ClickToMove(destination);
					return MoveResult.Moved;
				}

				// P6.9 — Flight path auto-detection (HB 6.2.3 method_10):
				// For long distances (>400 yards), check if taking a taxi would be faster than running.
				// ShouldTakeFlightpath compares run time vs flight+walk time, needs >30s savings.
				// SetFlightPathUsage sets up a BotPoi(PoiType.Fly) to walk to the flight master.
				float distanceSqr = me.Location.DistanceSqr(destination);
				if (distanceSqr > 160000f) // 400² yards
				{
					if (FlightPaths.ShouldTakeFlightpath(me.Location, destination, me.MovementInfo.RunSpeed))
					{
						if (FlightPaths.SetFlightPathUsage(me.Location, destination, out _, out _))
						{
							Logging.Write("[Navigator] Flight path would be faster — setting taxi POI");
							return MoveResult.PathGenerated;
						}
					}
				}

				// Try to use Tripper for pathfinding
				if (IsNavigatorLoaded)
				{
					uint mapId = (uint)(GetCurrentMapId());
					var start = new Vector3(me.Location.X, me.Location.Y, me.Location.Z);
					var end = new Vector3(destination.X, destination.Y, destination.Z);

					// Ensure blackspots are marked on navmesh before pathfinding
					// This is HB 4.3.4's OnTileLoaded workaround
					BlackspotManager.EnsureBlackspotsMarked();

					var result = TripperNavigator.FindPath(mapId, start, end, true);
					if (result.Status.Succeeded && result.Points != null && result.Points.Length > 0)
					{
						foreach (var point in result.Points)
						{
							_currentPath.Add(new WoWPoint(point.X, point.Y, point.Z));
						}

						// Store path metadata for off-mesh/terrain handling (P4.1 fix)
						_currentFlags = result.Flags;
						_currentPolyTypes = result.PolyTypes;
						_currentAbilityFlags = result.AbilityFlags;

						if (result.IsPartialPath)
							Logging.Write("[Navigator] WARNING: Partial path — destination may be unreachable");

						Logging.WriteDebug("[Navigator] Path generated with {0} points to: {1} ({2})", 
							_currentPath.Count, destinationName, destination);
					}
					else
					{
						// Fallback to direct movement
						_currentPath.Add(me.Location);
						_currentPath.Add(destination);
						_currentFlags = null;
						_currentPolyTypes = null;
						_currentAbilityFlags = null;
						Logging.WriteDebug("[Navigator] Pathfinding failed, using direct movement to: {0} ({1})", 
							destinationName, destination);
					}
				}
				else
				{
					// No navmesh, use direct path
					_currentPath.Add(me.Location);
					_currentPath.Add(destination);
					_currentFlags = null;
					_currentPolyTypes = null;
					_currentAbilityFlags = null;
					Logging.WriteDebug("[Navigator] Moving directly to: {0} ({1})", destinationName, destination);
				}

				_currentPathIndex = 0;

				// Path start skip (HB 6.2.3 method_14): skip early visible waypoints via raycast.
				// Walk forward through path and raycast from player position — if we can see
				// waypoint N directly, skip to it for smoother movement (avoids zigzag on flat terrain).
				// Stop before any off-mesh connection.
				if (_currentPath.Count > 2 && _currentFlags != null)
				{
					uint skipMapId = (uint)(GetCurrentMapId());
					var playerVec = new Vector3(me.Location.X, me.Location.Y, me.Location.Z);
					int lastVisible = 0;
					for (int i = 1; i < Math.Min(_currentPath.Count - 1, 6); i++) // Check up to 5 waypoints ahead
					{
						// Don't skip past off-mesh connections
						if (i < _currentFlags.Length &&
						    (_currentFlags[i] & TripperNav.StraightPathFlags.OffMeshConnection) != 0)
							break;

						var wp = new Vector3(_currentPath[i].X, _currentPath[i].Y, _currentPath[i].Z);
						var status = TripperNavigator.Raycast(skipMapId, playerVec, wp, out float hitT, out _);
						if (status.Succeeded && hitT >= 1.0f)
						{
							lastVisible = i;
						}
						else
						{
							break; // No point checking further if this one is blocked
						}
					}
					if (lastVisible > 0)
					{
						// Remove skipped waypoints from the path instead of advancing _currentPathIndex.
						// This keeps _currentPathIndex = 0, preventing the drift detection from comparing
						// player position against a segment that is far ahead (which caused infinite
						// path regeneration loops — drift detected → regen → skip → drift → regen).
						_currentPath.RemoveRange(0, lastVisible);
						if (_currentFlags != null && _currentFlags.Length > lastVisible)
							_currentFlags = _currentFlags[lastVisible..];
						if (_currentPolyTypes != null && _currentPolyTypes.Length > lastVisible)
							_currentPolyTypes = _currentPolyTypes[lastVisible..];
						if (_currentAbilityFlags != null && _currentAbilityFlags.Length > lastVisible)
							_currentAbilityFlags = _currentAbilityFlags[lastVisible..];
						Logging.WriteDebug("[Navigator] Skipped {0} visible early waypoints", lastVisible);
					}
				}
			}

			// Move along path
			// HB 4.3.4: Check both 2D distance and Z difference, push waypoint ahead
			if (_currentPath.Count > 0 && _currentPathIndex < _currentPath.Count)
			{
				// P6.14 — Combat abort during path following: when combat starts mid-path,
				// abandon the current path and let the behavior tree's combat routine take over.
				// This is the synchronous equivalent of HB 6.2.3's 4-second combat abort timeout.
				// We clear the path so next MoveTo() call will use the direct-movement shortcut above.
				if (me.Combat)
				{
					_destination = WoWPoint.Zero;
					_currentPath.Clear();
					_currentPathIndex = 0;
					_currentFlags = null;
					_currentPolyTypes = null;
					_currentAbilityFlags = null;
					_pathRegenThrottle = new WaitTimer(TimeSpan.FromMilliseconds(500)); // Allow immediate regen after combat
					Logging.WriteDebug("[Navigator] Combat detected — aborting path to let combat routine take over");
					return MoveResult.Moved;
				}

				// Door handling (HB 6.2.3 method_7): auto-detect and interact with closed doors on path
				var doorResult = HandleDoors(me);
				if (doorResult != null)
					return doorResult.Value;

				// Stuck detection — integrated directly in MoveTo() to cover ALL callers
				// (ActionMoveToPoi, corpse run, loot, hotspot, plugins, etc.)
				if (_stuckCheckTimer.IsFinished)
				{
					_stuckCheckTimer.Reset();
					if (StuckHandler.IsStuck())
					{
						_unstickAttempts++;
						if (_unstickAttempts >= MaxUnstickAttempts)
						{
							// AUDIT FIX: After MaxUnstickAttempts failed attempts, force path regeneration
							Logging.Write("[Navigator] {0} unstick attempts failed — forcing path regeneration", _unstickAttempts);
							_unstickAttempts = 0;
							_destination = WoWPoint.Zero; // Force new path on next call
							_pathRegenThrottle = new WaitTimer(TimeSpan.FromMilliseconds(500)); // Allow immediate regen
							return MoveResult.Failed;
						}
						StuckHandler.Unstick();
						return MoveResult.UnstuckAttempt;
					}
				}

				WoWPoint nextPoint = _currentPath[_currentPathIndex];

				// Path validity check (P6.7): if player has drifted far from the current path segment
				// (knockback, teleport, fear, etc.), force path regeneration instead of following stale path
				if (_currentPathIndex > 0)
				{
					WoWPoint prevPoint = _currentPath[_currentPathIndex - 1];
					float distToSegment = DistanceToLineSegment(me.Location, prevPoint, nextPoint);
					if (distToSegment > PathPrecision * 5f) // >10 yards off path = stale
					{
						Logging.WriteDebug("[Navigator] Player drifted {0:F1}yd from path — regenerating", distToSegment);
						_destination = WoWPoint.Zero; // Force new path on next call
						_pathRegenThrottle = new WaitTimer(TimeSpan.FromMilliseconds(500)); // Allow immediate regen
						return MoveResult.Moved;
					}
				}
				
				// For intermediate waypoints, use PathPrecision (2.0)
				// For final waypoint, use requested precision
				bool isFinalPoint = (_currentPathIndex == _currentPath.Count - 1);
				float waypointPrecision = isFinalPoint ? precision : PathPrecision;

				// Stair entry detection: if the NEXT waypoint has a big elevation change
				// relative to the CURRENT waypoint, this waypoint is a stair entry/exit point.
				// Tighten precision to ensure we actually reach it before turning toward stairs.
				// Without this, the bot turns early (2yd away) and walks into the railing.
				bool isStairEntryPoint = false;
				if (!isFinalPoint && _currentPathIndex + 1 < _currentPath.Count)
				{
					float nextSegmentZDiff = Math.Abs(_currentPath[_currentPathIndex + 1].Z - nextPoint.Z);
					if (nextSegmentZDiff > 2.5f)
					{
						isStairEntryPoint = true;
						waypointPrecision = 1.0f; // Must reach within 1yd before advancing to stair waypoint
					}
				}

				// Waypoint advance: use 2D distance + Z threshold for flat terrain,
				// but use 3D distance for elevation changes (stairs/ramps) to avoid
				// premature advance that changes direction and causes railing collisions.
				float distance2DSqr = me.Location.Distance2DSqr(nextPoint);
				float zDiff = Math.Abs(me.Location.Z - nextPoint.Z);
				
				bool reachedWaypoint;
				if (zDiff > 2.0f)
				{
					// On stairs: use 3D distance — prevents skipping when Z gap is large
					float dist3D = me.Location.Distance(nextPoint);
					reachedWaypoint = dist3D < waypointPrecision + 0.5f;
				}
				else
				{
					// Flat terrain (or stair entry with tightened precision): 2D distance + Z < 4.5
					reachedWaypoint = distance2DSqr < waypointPrecision * waypointPrecision && zDiff < 4.5f;
				}
				
				if (reachedWaypoint)
				{
					// Off-mesh guard: if this waypoint is an off-mesh entry point (elevator, portal),
					// require tighter precision before advancing — don't skip critical transition points
					if (_currentFlags != null && _currentPathIndex < _currentFlags.Length &&
					    (_currentFlags[_currentPathIndex] & TripperNav.StraightPathFlags.OffMeshConnection) != 0)
					{
						const float offMeshPrecision = 1.0f; // 1 yard tight precision for off-mesh points
						if (distance2DSqr > offMeshPrecision * offMeshPrecision)
						{
							// Not close enough to off-mesh connection point yet, click directly to it
							WoWMovement.ClickToMove(nextPoint);
							return MoveResult.Moved;
						}
						Logging.WriteDebug("[Navigator] Reached off-mesh connection at waypoint {0}", _currentPathIndex);
						
						// Dispatch off-mesh connection based on AreaType (ported from HB 4.3.4 method_4)
						// AUDIT FIX: Use _currentPathIndex (the off-mesh polygon) not _currentPathIndex-1 (previous = Ground/Road)
						// In Detour, straightPathPolys[i] is the polygon AT point i, so when flags[i] = OffMesh,
						// polyTypes[i] gives the off-mesh area type (Elevator/Portal/InteractUnit/InteractObject).
						if (_currentPolyTypes != null && _currentPathIndex < _currentPolyTypes.Length)
						{
							var offMeshResult = HandleOffMeshConnection(me, nextPoint, _currentPolyTypes[_currentPathIndex]);
							if (offMeshResult != null)
								return offMeshResult.Value;
						}
					}

					// Reset stuck timer and unstick counter on successful waypoint advance
					_stuckCheckTimer.Reset();
					_unstickAttempts = 0;
					try { StuckHandler.Reset(); } catch { }

					_currentPathIndex++;
					if (_currentPathIndex >= _currentPath.Count)
					{
						return MoveResult.ReachedDestination;
					}
					nextPoint = _currentPath[_currentPathIndex];
				}

				// =========== STAIR HANDLING IMPROVEMENT ===========
				// When approaching stairs/ramps, click at the entry/exit point (top or bottom)
				// instead of clicking at the center of the stair waypoint.
				// This prevents the character from walking into railings or getting stuck.
				const float StairZThreshold = 2.5f; // Min Z difference to consider as stair/ramp
				
				WoWPoint clickPoint = nextPoint;
				
				// Check if current segment has significant height change (stair/ramp)
				float segmentZDiff = Math.Abs(nextPoint.Z - me.Location.Z);
				float segment2DDist = (float)Math.Sqrt(me.Location.Distance2DSqr(nextPoint));
				
				// If there's a significant height change relative to horizontal distance = steep stair
				if (segmentZDiff > StairZThreshold && segment2DDist > 0.1f)
				{
					bool goingUp = nextPoint.Z > me.Location.Z;
					
					if (goingUp)
					{
						// Going UP stairs: click at the TOP of the stairs (the waypoint itself)
						// Don't push ahead, use exact waypoint which is at the top
						clickPoint = nextPoint;
						
						// If there's a next waypoint that's roughly at the same level as the top,
						// we can look ahead to that for smoother movement
						if (_currentPathIndex + 1 < _currentPath.Count)
						{
							WoWPoint afterStair = _currentPath[_currentPathIndex + 1];
							float zDiffNext = Math.Abs(afterStair.Z - nextPoint.Z);
							// If next point is at similar height (plateau at top of stairs)
							if (zDiffNext < 1.0f)
							{
								// Click towards the plateau, not center of stairs
								clickPoint = nextPoint;
							}
						}
					}
					else
					{
						// Going DOWN stairs: click at the BOTTOM of the stairs (the waypoint)
						// The waypoint is already at the bottom, just use it
						clickPoint = nextPoint;
					}
					
					// For stairs, don't push the waypoint ahead - click exactly where the stair ends
					WoWMovement.ClickToMove(clickPoint);
					return MoveResult.Moved;
				}
				// =========== END STAIR HANDLING ===========

				// HB 6.2.3 method_26: Push waypoint ahead by PathPrecision in movement direction,
				// BUT validate the extended point via navmesh raycast from BOTH the waypoint AND
				// the player position. This prevents cutting corners near walls/barriers.
				// Also reduce push near direction changes (turns) to prevent wall grazing.
				WoWPoint direction = nextPoint - me.Location;
				float length = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z);
				if (length > 0.01f)
				{
					direction = new WoWPoint(direction.X / length, direction.Y / length, direction.Z / length);
					
					// Check if next segment has a significant direction change (turn)
					float pushDistance = PathPrecision;
					if (_currentPathIndex + 1 < _currentPath.Count)
					{
						WoWPoint nextNextPoint = _currentPath[_currentPathIndex + 1];
						WoWPoint nextDir = nextNextPoint - nextPoint;
						float nextLen = (float)Math.Sqrt(nextDir.X * nextDir.X + nextDir.Y * nextDir.Y + nextDir.Z * nextDir.Z);
						if (nextLen > 0.01f)
						{
							nextDir = new WoWPoint(nextDir.X / nextLen, nextDir.Y / nextLen, nextDir.Z / nextLen);
							float dot = direction.X * nextDir.X + direction.Y * nextDir.Y;
							// dot < 0.7 means >45° turn — reduce push to avoid wall grazing
							if (dot < 0.7f)
								pushDistance = 0f; // No push at turns — click exact waypoint
							else if (dot < 0.9f)
								pushDistance = PathPrecision * 0.5f; // Slight turn — half push
						}
					}
					else
					{
						// Final waypoint — no push
						pushDistance = 0f;
					}
					
					if (pushDistance > 0.01f)
					{
						WoWPoint pushedPoint = nextPoint + direction * pushDistance;

						// Double raycast validation (HB 6.2.3 pattern):
						// 1. Waypoint → pushed point: no wall past the waypoint
						// 2. Player → pushed point: no wall between player and overshoot target
						bool clearFromWaypoint = !Raycast(nextPoint, pushedPoint, out _);
						bool clearFromPlayer = !Raycast(me.Location, pushedPoint, out _);
						
						if (clearFromWaypoint && clearFromPlayer)
						{
							clickPoint = pushedPoint;
						}
						else
						{
							clickPoint = nextPoint;
						}
					}
					else
					{
						clickPoint = nextPoint;
					}
				}

				WoWMovement.ClickToMove(clickPoint);
				return MoveResult.Moved;
			}

			return MoveResult.PathGenerationFailed;
		}

		public static bool CanNavigateFully(WoWPoint start, WoWPoint destination)
		{
			if (!IsNavigatorLoaded)
				return true; // Assume we can if no mesh loaded

			uint mapId = (uint)(GetCurrentMapId());
			var startVec = new Vector3(start.X, start.Y, start.Z);
			var endVec = new Vector3(destination.X, destination.Y, destination.Z);

			// Ensure blackspots are marked before pathfinding
			BlackspotManager.EnsureBlackspotsMarked();

			var result = TripperNavigator.FindPath(mapId, startVec, endVec, true);
			
			// Check if path is complete (reached destination)
			if (result.Status.Succeeded && result.Points != null && result.Points.Length > 0)
			{
				var lastPoint = result.Points[^1];
				float distToEnd = Vector3.Distance(lastPoint, endVec);
				return distToEnd < PathPrecision;
			}

			return false;
		}

		public static bool CanNavigateFully(WoWPoint destination)
		{
			LocalPlayer? me = ObjectManager.Me;
			if (me == null) return false;
			return CanNavigateFully(me.Location, destination);
		}

		public static WoWPoint[]? GeneratePath(WoWPoint start, WoWPoint destination)
		{
			if (!IsNavigatorLoaded)
				return null;

			uint mapId = (uint)(GetCurrentMapId());
			var startVec = new Vector3(start.X, start.Y, start.Z);
			var endVec = new Vector3(destination.X, destination.Y, destination.Z);

			// Ensure blackspots are marked before pathfinding
			BlackspotManager.EnsureBlackspotsMarked();

			var result = TripperNavigator.FindPath(mapId, startVec, endVec, true);
			if (result.Status.Succeeded && result.Points != null && result.Points.Length > 0)
			{
				var path = new WoWPoint[result.Points.Length];
				for (int i = 0; i < result.Points.Length; i++)
				{
					var point = result.Points[i];
					path[i] = new WoWPoint(point.X, point.Y, point.Z);
				}
				return path;
			}

			return null;
		}

		public static bool IsPathSafe(IList<WoWPoint> path)
		{
			if (path == null || path.Count < 2)
				return false;

			if (!IsNavigatorLoaded)
				return true; // Assume safe if no mesh

			uint mapId = (uint)(GetCurrentMapId());

			// Check each segment of the path with raycast
			for (int i = 0; i < path.Count - 1; i++)
			{
				var start = new Vector3(path[i].X, path[i].Y, path[i].Z);
				var end = new Vector3(path[i + 1].X, path[i + 1].Y, path[i + 1].Z);

				var status = TripperNavigator.Raycast(mapId, start, end, out float hitT, out _);
				if (status.Succeeded)
				{
					// Hit something before reaching next waypoint (hitT < 1.0 means hit)
					if (hitT < 0.99f)
						return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Finds the mesh height at a given XY position.
		/// </summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <param name="z">Output Z coordinate (height).</param>
		/// <returns>True if a valid height was found.</returns>
		public static bool FindMeshHeight(float x, float y, out float z)
		{
			z = 0f;

			if (!IsNavigatorLoaded)
				return false;

			uint mapId = (uint)(GetCurrentMapId());
			var position = new Vector3(x, y, 10000f); // Start from high up

			if (TripperNavigator.FindNearestPoint(mapId, position, out Vector3 nearestPoint))
			{
				z = nearestPoint.Z;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Finds the mesh height at a given position.
		/// </summary>
		/// <param name="pos">The position to check (Z will be modified).</param>
		/// <returns>True if a valid height was found.</returns>
		public static bool FindMeshHeight(ref Tripper.XNAMath.Vector3 pos)
		{
			if (FindMeshHeight(pos.X, pos.Y, out float z))
			{
				pos.Z = z;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Finds the mesh height at a given position (alias for FindMeshHeight).
		/// </summary>
		/// <param name="v">The position to check (Z will be modified).</param>
		/// <returns>True if a valid height was found.</returns>
		public static bool FindHeight(ref Tripper.XNAMath.Vector3 v)
		{
			return FindMeshHeight(ref v);
		}

		/// <summary>
		/// Finds the mesh height at a given XY position.
		/// </summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <param name="z">Output Z coordinate (height).</param>
		/// <returns>True if a valid height was found.</returns>
		public static bool FindHeight(float x, float y, out float z)
		{
			return FindMeshHeight(x, y, out z);
		}

		/// <summary>
		/// Finds all mesh heights at a given XY position.
		/// </summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <returns>List of heights found at the position.</returns>
		public static List<float> FindHeights(float x, float y)
		{
			var heights = new List<float>();

			if (!IsNavigatorLoaded)
				return heights;

			// FindNearestPoint returns single height - for multi-level we'd need additional API
			if (FindMeshHeight(x, y, out float z))
			{
				heights.Add(z);
			}

			return heights;
		}

		/// <summary>
		/// Finds a random navigable point within radius of center position.
		/// </summary>
		/// <param name="center">Center position.</param>
		/// <param name="radius">Search radius in yards.</param>
		/// <returns>Random navigable point, or center if none found.</returns>
		public static WoWPoint FindRandomPoint(WoWPoint center, float radius)
		{
			if (!IsNavigatorLoaded)
				return center;

			uint mapId = (uint)(GetCurrentMapId());
			var centerVec = new Vector3(center.X, center.Y, center.Z);

			if (TripperNavigator.FindRandomPoint(mapId, centerVec, radius, out Vector3 randomPoint))
			{
				return new WoWPoint(randomPoint.X, randomPoint.Y, randomPoint.Z);
			}

			return center;
		}

		/// <summary>
		/// Finds the nearest navigable point to a given position.
		/// </summary>
		/// <param name="position">Position to search from.</param>
		/// <returns>Nearest navigable point, or original position if none found.</returns>
		public static WoWPoint FindNearestPoint(WoWPoint position)
		{
			if (!IsNavigatorLoaded)
				return position;

			uint mapId = (uint)(GetCurrentMapId());
			var posVec = new Vector3(position.X, position.Y, position.Z);

			if (TripperNavigator.FindNearestPoint(mapId, posVec, out Vector3 nearestPoint))
			{
				return new WoWPoint(nearestPoint.X, nearestPoint.Y, nearestPoint.Z);
			}

			return position;
		}

		/// <summary>
		/// Performs a raycast from start to end position on the navmesh.
		/// </summary>
		/// <param name="start">Ray start position.</param>
		/// <param name="end">Ray end position.</param>
		/// <param name="hitPosition">Output hit position if raycast hits.</param>
		/// <returns>True if raycast hit a navmesh boundary (path is NOT clear).</returns>
		public static bool Raycast(WoWPoint start, WoWPoint end, out WoWPoint hitPosition)
		{
			hitPosition = end;

			if (!IsNavigatorLoaded)
				return false;

			uint mapId = (uint)(GetCurrentMapId());
			var startVec = new Vector3(start.X, start.Y, start.Z);
			var endVec = new Vector3(end.X, end.Y, end.Z);

			var status = TripperNavigator.Raycast(mapId, startVec, endVec, out float hitT, out _);
			if (status.Succeeded && hitT < 1.0f)
			{
				// Calculate hit position along the ray
				var direction = endVec - startVec;
				var hitVec = startVec + direction * hitT;
				hitPosition = new WoWPoint(hitVec.X, hitVec.Y, hitVec.Z);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Disposes the navigator and releases resources.
		/// </summary>
		public static void Dispose()
		{
			if (_navigator != null)
			{
				_navigator.Dispose();
				_navigator = null;
			}
		}

		/// <summary>
		/// Converts a MoveResult to a TreeSharp RunStatus.
		/// </summary>
		public static TreeSharp.RunStatus GetRunStatusFromMoveResult(MoveResult moveResult)
		{
			switch (moveResult)
			{
				case MoveResult.Failed:
				case MoveResult.PathGenerationFailed:
					return TreeSharp.RunStatus.Failure;
				case MoveResult.ReachedDestination:
				case MoveResult.PathGenerated:
				case MoveResult.UnstuckAttempt:
				case MoveResult.Moved:
					return TreeSharp.RunStatus.Success;
				default:
					throw new ArgumentOutOfRangeException(nameof(moveResult));
			}
		}

		/// <summary>
		/// Calculates the shortest distance from a point to a line segment (2D, XY plane).
		/// Used for path validity checking — detects when player has drifted off the current path.
		/// </summary>
		private static float DistanceToLineSegment(WoWPoint point, WoWPoint segA, WoWPoint segB)
		{
			float dx = segB.X - segA.X;
			float dy = segB.Y - segA.Y;
			float lenSqr = dx * dx + dy * dy;

			if (lenSqr < 0.0001f)
			{
				// Segment is essentially a point
				return point.Distance(segA);
			}

			// Project point onto segment, clamped to [0, 1]
			float t = ((point.X - segA.X) * dx + (point.Y - segA.Y) * dy) / lenSqr;
			t = Math.Max(0f, Math.Min(1f, t));

			// Closest point on segment
			float closestX = segA.X + t * dx;
			float closestY = segA.Y + t * dy;
			float closestZ = segA.Z + t * (segB.Z - segA.Z);

			float ex = point.X - closestX;
			float ey = point.Y - closestY;
			float ez = point.Z - closestZ;
			return (float)Math.Sqrt(ex * ex + ey * ey + ez * ez);
		}

		/// <summary>
		/// Handles off-mesh connection dispatch based on AreaType.
		/// Ported from HB 4.3.4 MeshNavigator.method_4 — Elevator/Portal/InteractUnit/InteractObject.
		/// Returns null to continue normal waypoint advancement, or a MoveResult to return immediately.
		/// </summary>
		private static MoveResult? HandleOffMeshConnection(LocalPlayer me, WoWPoint targetPoint, TripperNav.AreaType areaType)
		{
			Logging.WriteDebug("[Navigator] Off-mesh dispatch: AreaType={0}, target={1}", areaType, targetPoint);
			switch (areaType)
			{
				case TripperNav.AreaType.Elevator:
					return HandleElevator(me, targetPoint);

				case TripperNav.AreaType.Portal:
				case TripperNav.AreaType.DefendersPortal:
				case TripperNav.AreaType.HordePortal:
				case TripperNav.AreaType.AlliancePortal:
					return HandlePortal(me);

				case TripperNav.AreaType.InteractUnit:
					return HandleInteractUnit(me);

				case TripperNav.AreaType.InteractObject:
					return HandleInteractObject(me);

				default:
					// Standard off-mesh: Run/Jump connections (HB 6.2.3 method_19)
					// Check ability flags to determine if this connection is traversable
					return HandleStandardOffMesh(me, targetPoint);
			}
		}

		/// <summary>
		/// Standard off-mesh connection handler for Run/Jump connections.
		/// Ported from HB 6.2.3 MeshNavigator.method_19.
		/// Only allows connections with Run and/or Jump flags.
		/// If the connection requires Teleport, Transport, or Swim and isn't handled
		/// by a specific handler above, we fail.
		/// </summary>
		private static MoveResult? HandleStandardOffMesh(LocalPlayer me, WoWPoint targetPoint)
		{
			// Check ability flags for this off-mesh connection
			if (_currentAbilityFlags != null && _currentPathIndex < _currentAbilityFlags.Length)
			{
				var abilityFlags = _currentAbilityFlags[_currentPathIndex];
				var allowedFlags = TripperNav.AbilityFlags.Run | TripperNav.AbilityFlags.Jump | TripperNav.AbilityFlags.RunSafe;

				// If the connection requires abilities beyond Run/Jump (e.g. Teleport, Transport),
				// and we haven't handled it via a specific handler, fail gracefully
				if ((abilityFlags & ~allowedFlags) != TripperNav.AbilityFlags.None &&
				    (abilityFlags & TripperNav.AbilityFlags.Unwalkable) != 0)
				{
					Logging.WriteDebug("[Navigator] Off-mesh connection requires unsupported abilities: {0}", abilityFlags);
					return MoveResult.Failed;
				}

				// Jump connection — dismount first if mounted (can't jump mounted in WotLK)
				if ((abilityFlags & TripperNav.AbilityFlags.Jump) != 0)
				{
					if (me.Mounted)
					{
						Mount.Dismount();
						return MoveResult.Moved;
					}
				}
			}

			// Standard Run/Jump — click to the target point and advance
			WoWMovement.ClickToMove(targetPoint);
			return null; // Advance to next waypoint normally
		}

		/// <summary>
		/// Elevator handling: find transport, wait for it, board, ride, exit.
		/// HB 4.3.4 pattern: detect elevator via GameObjects with Transport type.
		/// AUDIT FIX: When multiple transports exist, prefer the one closest to target Z
		/// (direction-aware selection) over pure proximity.
		/// </summary>
		private static MoveResult? HandleElevator(LocalPlayer me, WoWPoint targetPoint)
		{
			// Find elevator/transport game objects within reasonable range (50 yards)
			var transports = ObjectManager.GetObjectsOfType<WoWGameObject>(false, false)
				.Where(go => (go.SubType == WoWGameObjectType.Transport || go.SubType == WoWGameObjectType.MapObjectTransport)
				              && go.Location.DistanceSqr(me.Location) < 2500f) // 50 yards
				.ToList();

			if (transports.Count == 0)
			{
				Logging.WriteNavigator("No elevator/transport found nearby. Continuing.");
				return null;
			}

			// Direction-aware elevator selection (AUDIT FIX):
			// When multiple transports exist (e.g., Undercity, Thunder Bluff), pick the one
			// whose current Z matches our boarding level AND is heading toward target Z.
			// Step 1: Filter to elevators at our level (boardable = within 3yd Z).
			// Step 2: Among those, prefer the one closest to target Z (going our direction).
			// Fallback: if none at our level, pick closest Z to player (waiting for it).
			WoWGameObject transport;
			if (transports.Count > 1)
			{
				var boardable = transports.Where(go => Math.Abs(go.Z - me.Z) < 3f).ToList();
				if (boardable.Count > 1)
				{
					// Multiple at our level — pick the one whose Z is closest to target
					// (i.e., heading in the right direction: up toward high target, down toward low)
					transport = boardable.OrderBy(go => Math.Abs(go.Z - targetPoint.Z)).First();
				}
				else if (boardable.Count == 1)
				{
					transport = boardable[0];
				}
				else
				{
					// None at our level — pick closest to player Z (waiting for arrival)
					transport = transports.OrderBy(go => Math.Abs(go.Z - me.Z)).First();
				}
				Logging.WriteDebug("[Navigator] Multiple transports nearby ({0}), chose: {1} (targetZ={2:F1})",
					transports.Count, transport.Name, targetPoint.Z);
			}
			else
			{
				transport = transports[0];
			}

			// If we're already close to the target point, elevator ride is complete
			if (me.Location.DistanceSqr(targetPoint) < 4f)
			{
				_elevatorBoarded = false;
				_ridingElevator = false;
				Logging.WriteNavigator("Elevator ride complete.");
				return null; // Continue normal waypoint advancement
			}

			// If we're on the transport
			if (me.IsOnTransport)
			{
				_ridingElevator = true; // Block mount-up while riding (HB 6.2.3 pattern)

				// Check if elevator has reached destination height
				if (Math.Abs(me.Z - targetPoint.Z) > 2f)
				{
					Logging.WriteNavigator("Riding elevator, waiting for destination height...");
					return MoveResult.Moved; // Wait on elevator
				}

				// Elevator reached destination — move off it
				_elevatorBoarded = true;
				_ridingElevator = false;
				Logging.WriteNavigator("Moving out of elevator.");
				WoWMovement.ClickToMove(targetPoint);
				return MoveResult.Moved;
			}

			// Dismount before elevator interaction (can't board mounted)
			if (me.Mounted)
			{
				Mount.Dismount("[Navigator] Dismounting for elevator");
				return MoveResult.Moved;
			}

			// Not on transport yet — check if elevator is at our level
			if (Math.Abs(transport.Z - me.Z) > 2f)
			{
				// Elevator isn't at our level — move to waiting spot and wait
				if (_currentPathIndex > 0 && _currentPathIndex - 1 < _currentPath.Count)
				{
					var waitSpot = _currentPath[_currentPathIndex - 1];
					if (me.Location.DistanceSqr(waitSpot) > 4f)
					{
						Logging.WriteNavigator("Moving to elevator waiting spot.");
						WoWMovement.ClickToMove(waitSpot);
						return MoveResult.Moved;
					}
				}
				// Stop walking while waiting for elevator (HB pattern)
				WoWMovement.MoveStop();
				Logging.WriteNavigator("Waiting for elevator...");
				return MoveResult.Moved;
			}

			// Elevator is at our level — board it
			if (!_elevatorBoarded)
			{
				Logging.WriteNavigator("Boarding elevator.");
				WoWMovement.ClickToMove(transport.Location);
				return MoveResult.Moved;
			}

			return MoveResult.Failed;
		}

		/// <summary>
		/// Portal handling: find nearest Goober or SpellCaster game object and interact.
		/// HB 4.3.4 pattern from MeshNavigator.method_4.
		/// AUDIT FIX: Added 30-yard range limit to avoid interacting with distant random objects.
		/// </summary>
		private static MoveResult? HandlePortal(LocalPlayer me)
		{
			// Find nearest portal-type game object within 30 yards (Goober or SpellCaster)
			const float maxSearchDistSqr = 900f; // 30 yards squared
			WoWGameObject? bestPortal = null;
			float bestDistSqr = maxSearchDistSqr;

			foreach (var go in ObjectManager.GetObjectsOfType<WoWGameObject>())
			{
				if (go.SubType == WoWGameObjectType.Goober || go.SubType == WoWGameObjectType.SpellCaster)
				{
					float distSqr = go.Location.DistanceSqr(me.Location);
					if (distSqr < bestDistSqr)
					{
						bestDistSqr = distSqr;
						bestPortal = go;
					}
				}
			}

			if (bestPortal == null)
			{
				Logging.WriteNavigator("Could not find portal to interact with.");
				return MoveResult.Failed;
			}

			if (bestPortal.WithinInteractRange)
			{
				Logging.WriteDebug("[Navigator] Interacting with portal: {0}", bestPortal.Name);
				bestPortal.Interact();
				return MoveResult.Moved;
			}

			// Move closer to the portal
			WoWMovement.ClickToMove(bestPortal.Location);
			return MoveResult.Moved;
		}

		/// <summary>
		/// InteractUnit handling: find nearest non-hostile, alive NPC and interact.
		/// HB 4.3.4 pattern from MeshNavigator.method_4.
		/// AUDIT FIX: Filter out hostile units, dead units, and player-controlled units
		/// to avoid targeting enemy mobs during off-mesh traversal.
		/// </summary>
		private static MoveResult? HandleInteractUnit(LocalPlayer me)
		{
			// AUDIT FIX: Add 30yd range limit to prevent interacting with distant NPCs
			const float maxSearchDistSqr = 900f; // 30 yards squared
			var unit = ObjectManager.GetObjectsOfType<WoWUnit>(false, false)
				.Where(u => !u.IsDead && !u.IsHostile && !u.PlayerControlled && !u.IsPlayer
				            && u.Location.DistanceSqr(me.Location) < maxSearchDistSqr)
				.OrderBy(u => u.Location.DistanceSqr(me.Location))
				.FirstOrDefault();

			if (unit == null)
			{
				Logging.WriteNavigator("Could not find unit to interact with.");
				return MoveResult.Failed;
			}

			if (!unit.WithinInteractRange)
			{
				WoWMovement.ClickToMove(unit.Location);
				return MoveResult.Moved;
			}

			if (me.Mounted)
				Mount.Dismount("InteractUnit in path");
			unit.Interact();
			return MoveResult.Moved;
		}

		/// <summary>
		/// InteractObject handling: find nearest interactable game object and interact.
		/// HB 4.3.4 pattern from MeshNavigator.method_4.
		/// </summary>
		private static MoveResult? HandleInteractObject(LocalPlayer me)
		{
			// AUDIT FIX: Add 30yd range limit + move to object if in range but not interact range
			const float maxSearchDistSqr = 900f; // 30 yards squared
			var gameObject = ObjectManager.GetObjectsOfType<WoWGameObject>(false, false)
				.Where(go => go.Location.DistanceSqr(me.Location) < maxSearchDistSqr)
				.OrderBy(go => go.Location.DistanceSqr(me.Location))
				.FirstOrDefault();

			if (gameObject == null)
			{
				// No interactable object in range — skip this off-mesh point
				return null;
			}

			if (!gameObject.WithinInteractRange)
			{
				WoWMovement.ClickToMove(gameObject.Location);
				return MoveResult.Moved;
			}

			if (me.Mounted)
				Mount.Dismount("InteractObject in path");
			gameObject.Interact();
			return MoveResult.Moved;
		}

		/// <summary>
		/// Door handling: auto-detect closed doors on the path and interact to open them.
		/// Ported from HB 6.2.3 MeshNavigator.method_7/method_8.
		/// Improvement: steers through the door center point before resuming normal path,
		/// so the bot passes cleanly through the doorframe like HB does.
		/// Returns null if no door action needed, MoveResult.Moved if interacting with a door.
		/// </summary>
		private static WoWPoint _doorCenterTarget = WoWPoint.Zero;
		
		private static MoveResult? HandleDoors(LocalPlayer me)
		{
			// If we're actively steering through a door center, continue until we pass through
			if (_doorCenterTarget != WoWPoint.Zero)
			{
				float distToDoorCenter = me.Location.Distance(_doorCenterTarget);
				if (distToDoorCenter < 1.5f)
				{
					// Passed through door center — resume normal path following
					_doorCenterTarget = WoWPoint.Zero;
					return null;
				}
				// Keep steering toward door center
				WoWMovement.ClickToMove(_doorCenterTarget);
				return MoveResult.Moved;
			}
			
			// Cooldown between door interactions to avoid spam
			if (!_doorInteractTimer.IsFinished)
				return null;

			// Find closed/ready Door-type GameObjects within 10 yards
			const float doorSearchDistSqr = 100f; // 10 yards squared
			WoWGameObject? closestDoor = null;
			float closestDistSqr = doorSearchDistSqr;

			foreach (var go in ObjectManager.GetObjectsOfType<WoWGameObject>(false, false))
			{
				if (go.SubType != WoWGameObjectType.Door)
					continue;

				// State.Ready = closed and interactable; Active = already open
				if (go.State != WoWGameObjectState.Ready)
					continue;

				// Skip locked doors (can't open without key)
				if (go.Locked)
					continue;

				float distSqr = go.Location.DistanceSqr(me.Location);
				if (distSqr < closestDistSqr)
				{
					// Verify the door is actually on our path direction
					// (avoid opening random doors that are beside us but not blocking)
					if (_currentPathIndex < _currentPath.Count)
					{
						WoWPoint nextWp = _currentPath[_currentPathIndex];
						// Door must be roughly between us and the next waypoint
						float distToNext = go.Location.DistanceSqr(nextWp);
						float playerToNext = me.Location.DistanceSqr(nextWp);
						if (distToNext > playerToNext)
							continue; // Door is behind us relative to movement direction
					}

					closestDistSqr = distSqr;
					closestDoor = go;
				}
			}

			if (closestDoor == null)
				return null;

			// Move toward door center if not in interact range
			if (!closestDoor.WithinInteractRange)
			{
				// Steer to door center (not nearby waypoint) to pass through cleanly
				WoWMovement.ClickToMove(closestDoor.Location);
				return MoveResult.Moved;
			}

			// HB 6.2.3 pattern: stop before interacting
			if (me.IsMoving)
			{
				WoWMovement.MoveStop();
				return MoveResult.Moved;
			}

			// Interact to open the door
			Logging.WriteDebug("[Navigator] Opening door: {0}", closestDoor.Name);
			closestDoor.Interact();
			_doorInteractTimer.Reset();
			
			// Set door center as steering target — bot will walk through the center
			// of the doorframe before resuming normal waypoint following.
			// Calculate a point slightly past the door in our movement direction.
			if (_currentPathIndex < _currentPath.Count)
			{
				WoWPoint doorCenter = closestDoor.Location;
				WoWPoint nextWp = _currentPath[_currentPathIndex];
				WoWPoint dir = nextWp - doorCenter;
				float dirLen = (float)Math.Sqrt(dir.X * dir.X + dir.Y * dir.Y + dir.Z * dir.Z);
				if (dirLen > 0.01f)
				{
					// Target is 2 yards past the door center in path direction
					dir = new WoWPoint(dir.X / dirLen, dir.Y / dirLen, dir.Z / dirLen);
					_doorCenterTarget = doorCenter + dir * 2.0f;
				}
				else
				{
					_doorCenterTarget = doorCenter;
				}
			}
			else
			{
				_doorCenterTarget = closestDoor.Location;
			}

			return MoveResult.Moved;
		}
	}
}
