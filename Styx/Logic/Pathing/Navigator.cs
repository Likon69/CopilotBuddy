using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
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
		private static bool _ridingElevator; // True while actively riding an elevator (blocks mount-up)
		private static int _unstickAttempts; // AUDIT FIX: Max retry counter to prevent infinite unstick loops
		private const int MaxUnstickAttempts = 5; // After 5 failed unsticks, force path regeneration

		// Path drift suppression after an initial node skip.
		// We may set _currentPathIndex > 0 immediately after generating a new path (HB 6.2.3 method_14).
		// Our drift detection (P6.7) would otherwise see the player "far from segment" and regen endlessly.
		private static bool _suppressDriftCheck;
		private static int _suppressDriftCheckIndex;

		// Elevator sequence tracking (HB 6.2.3: once method_18 enters method_20, the elevator
		// handler keeps being called every tick via the path index — no re-checking player
		// position vs offmesh start). This flag locks the bot into elevator mode.
		private static bool _inElevatorSequence;
		private static WoWPoint _elevatorTarget = WoWPoint.Zero;
		// HB 4.3.4 MeshNavigator.bool_0: state flag used by elevator off-mesh logic
		private static bool _hbElevatorFlag;
		// Track whether the current path is partial (navmesh doesn't fully connect start→dest).
		// HB 6.2.3 method_24: returns MoveResult.Failed when reaching end of a partial path,
		// instead of ReachedDestination, so the behavior tree knows the bot isn't at the goal.
		private static bool _isPartialPath;

		// HB 6.2.3 areaType_0: player faction-specific area type, updated on bot start.
		// RaycastBlocked must allow this area in addition to Ground/Water/Road.
		private static TripperNav.AreaType _factionAreaType = TripperNav.AreaType.Ground;

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
				}
				return _navigator;
			}
		}

        // Navigation provider management (HB 5.x/6.x compatibility)
        private static INavigationProvider _currentProvider;

        /// <summary>
        /// The active navigation provider. Setting this property raises the
        /// <see cref="OnNavigationProviderChanged"/> event if the value differs.
        /// </summary>
        public static INavigationProvider NavigationProvider
        {
            get => _currentProvider;
            set
            {
                if (ReferenceEquals(_currentProvider, value))
                    return;
                var old = _currentProvider;
                _currentProvider = value;
                OnNavigationProviderChanged?.Invoke(null, new NavigationProviderChangedEventArgs<INavigationProvider>(old, value));
            }
        }

        /// <summary>
        /// Convenience alias used by older HB code.
        /// </summary>
        public static INavigationProvider CurrentProvider => _currentProvider;

        /// <summary>
        /// Fired when <see cref="NavigationProvider"/> is replaced.  Consumers
        /// such as <see cref="BlackspotManager"/> can (re)wire tile events.
        /// </summary>
        public static event EventHandler<NavigationProviderChangedEventArgs<INavigationProvider>> OnNavigationProviderChanged;

        /// <summary>
        /// Whether the underlying Tripper navigator instance has finished loading
        /// its meshes.  Mirrors HB's Navigator.IsNavigatorLoaded property.
        /// </summary>
        public static bool IsNavigatorLoaded => _navigator != null && _navigator.IsLoaded;

        /// <summary>
        /// Computes the path distance between two points using TripperNavigator.
        /// Returns null if no path could be generated or if the calculated
        /// distance exceeds <paramref name="maxDistance"/>.
        /// This helper backs <see cref="StuckHandler"/> and was removed earlier
        /// during refactoring; it has now been restored.
        /// </summary>
        public static float? PathDistance(WoWPoint from, WoWPoint to, float maxDistance)
        {
            try
            {
                if (from == WoWPoint.Zero || to == WoWPoint.Zero)
                    return null;
                uint mapId = GetCurrentMapId();
                var start = new System.Numerics.Vector3(from.X, from.Y, from.Z);
                var end = new System.Numerics.Vector3(to.X, to.Y, to.Z);
                var result = TripperNavigator.FindPath(mapId, start, end, true);
                if (result == null || !result.Succeeded || result.Points == null || result.Points.Length == 0)
                    return null;
                float dist = 0f;
                var pts = result.Points;
                for (int i = 1; i < pts.Length; i++)
                    dist += System.Numerics.Vector3.Distance(pts[i - 1], pts[i]);
                if (dist > maxDistance) return null;
                return dist;
            }
            catch
            {
                return null;
            }
        }

		static Navigator()
		{
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
		}

		private static void OnBotStart(EventArgs args)
		{
			Clear();
			// Load navigation meshes on bot start
			if (_navigator == null)
			{
				_navigator = new TripperNav.Navigator();
			}

			if (!_navigator.IsLoaded)
			{
				try
				{
					if (_navigator.LoadMeshes())
					{
					}
					else
					{
					}
				}
				catch (Exception ex)
				{
					_ = ex;
				}
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
						// HB 6.2.3 method_1: cache faction area type for RaycastBlocked
						_factionAreaType = me.IsHorde ? TripperNav.AreaType.Horde : TripperNav.AreaType.Alliance;
					}
				}
				catch (Exception ex)
				{
					_ = ex;
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
				_ = ex;
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
			_ridingElevator = false;
			_inElevatorSequence = false;
			_elevatorTarget = WoWPoint.Zero;
			_unstickAttempts = 0;
			_doorCenterTarget = WoWPoint.Zero;
			_pathRegenThrottle = new WaitTimer(TimeSpan.FromMilliseconds(500)); // Reset so next path gen is immediate
			// HB 4.3.4 MeshNavigator.Clear(): sets CurrentMovePath=null + StuckHandler.Reset().
			// Does NOT call MoveStop(). Callers that need to stop (like explicit user stop)
			// do their own MoveStop. Calling MoveStop here causes WoW auto-sit on every
			// BotPoi.Clear() or exception recovery.
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
			{
				return MoveResult.Failed;
			}

			// Check for NaN coordinates - would crash WoW with INT_DIVIDE_BY_ZERO
			if (float.IsNaN(destination.X) || float.IsNaN(destination.Y) || float.IsNaN(destination.Z))
			{
				return MoveResult.Failed;
			}

			LocalPlayer? me = ObjectManager.Me;
			if (me == null)
			{
				return MoveResult.Failed;
			}

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
			try
			{
				if (Mount.ShouldMount(destination))
				{
					WoWPoint dest = destination;
					Mount.StateMount(() => dest);
				}
			}
			catch (System.Exception ex)
			{
				_ = ex;
			}

			// Auto-dismount near destination (HB pattern):
			// Dismount when close to interaction POIs (loot, vendor, quest NPC, etc.)
			// to avoid running into NPCs mounted and failing to interact.
			try
			{
				if (Mount.ShouldDismount(destination))
				{
					Mount.Dismount("Near destination");
				}
			}
			catch (System.Exception ex)
			{
				_ = ex;
			}

			// HB 4.3.4/6.2.3: NEVER bypass navmesh pathfinding during combat.
			// HB limits pathfinding DURATION (4-second abort) but always paths via navmesh.
			// Our old code did direct ClickToMove in combat — this caused the bot to walk
			// through walls when approaching targets after aggro.
			// Now: if we're in combat with no path, we fall through to normal pathfinding
			// below. The pathfinding is synchronous so it may block briefly, but that's
			// how HB 4.3.4 works too (it blocks, with a 4-second combat abort timeout).

			// HB 6.2.3 method_9: Compare new destination against the PATH ENDPOINT (not raw _destination).
			// A navmesh path endpoint is more stable than a moving mob's position — it stays at the
			// closest reachable mesh point even as the mob wanders slightly. This prevents constant
			// stuck-state resets when chasing a wandering mob.
			// Threshold: PathPrecision (2yd), matching HB 6.2.3 method_27.
			float destThresholdSqr = PathPrecision * PathPrecision; // 4.0f = 2yd
			WoWPoint pathEndpoint = _currentPath.Count > 0 ? _currentPath[_currentPath.Count - 1] : _destination;
			bool destinationChanged = _destination == WoWPoint.Zero
				|| (pathEndpoint != WoWPoint.Zero
					? pathEndpoint.DistanceSqr(destination) > destThresholdSqr
					: _destination.DistanceSqr(destination) > destThresholdSqr);
			// Also regen if path was cleared (drift, combat, MaxUnstick) but destination hasn't changed.
			// In that case we do NOT reset stuck state — the unstick sequence continues.
			bool needsPathRegen = !destinationChanged && _currentPath.Count == 0;
			if (destinationChanged || needsPathRegen)
			{
				if (!_pathRegenThrottle.IsFinished)
				{
					// Too soon since last path generation — wait (HB pattern: never blindly ClickToMove)
					return MoveResult.Moved;
				}
				_pathRegenThrottle.Reset();

				_destination = destination;
				_currentPath.Clear();

				// Only reset stuck state when the destination truly changed.
				// If we're just regenerating after drift/combat/MaxUnstick, keep stuck state intact
				// so the unstick sequence (jump 1→2→3→strafe→...) can continue.
				if (destinationChanged)
				{
					_stuckCheckTimer.Reset();
					_unstickAttempts = 0;

					try
					{
						StuckHandler.Reset();
					}
					catch
					{
						// Ignore
					}

					// Clear stale elevator state: if bot died during elevator ride, the sequence
					// flag persists and blocks all subsequent navigation (e.g., corpse run).
					// Resetting here ensures a fresh start when destination changes.
					_inElevatorSequence = false;
					_elevatorTarget = WoWPoint.Zero;
					_ridingElevator = false;
				}

				// P6.14 — Combat bypass REMOVED (see comment at top of method about HB 4.3.4/6.2.3):
				// HB NEVER skips navmesh in combat. Direct ClickToMove caused the bot to walk
				// straight into hills/cliffs when targets are behind terrain. Now we always
				// use navmesh pathfinding. The 500ms throttle prevents excessive recalculation.

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

					// HB-style tile streaming: ensure navmesh tiles are loaded at BOTH
					// start AND destination before pathfinding. Without this, the A*
					// search can't discover offmesh connections (elevators, portals) that
					// sit in tiles far from the player, causing the pathfinder to generate
					// a long detour path or return a partial path.
					// NOTE: HB's WowNavigator loads tiles on-demand during A* via MeshProvider.
					// We can't do that, so we pre-load tiles explicitly instead.
					try
					{
						TripperNavigator.EnsureTilesAroundPosition(mapId, start, LoadTilesAroundRadius);
						TripperNavigator.EnsureTilesAroundPosition(mapId, end, LoadTilesAroundRadius);

						// COPILOTBUDDY ENHANCEMENT (not in HB): also load tiles at the midpoint
						// to bridge large gaps. HB doesn't need this because it loads tiles
						// on-demand during A*; we pre-load so need to cover the corridor.
						float distSqr = Vector3.DistanceSquared(start, end);
						if (distSqr > 40000f) // > 200 yards
						{
							var mid = (start + end) * 0.5f;
							TripperNavigator.EnsureTilesAroundPosition(mapId, mid, LoadTilesAroundRadius);
						}
					}
					catch
					{
						// Non-fatal: pathfinding still works with whatever tiles are loaded
					}

					// Ensure blackspots are marked on navmesh before pathfinding
					// This is HB 4.3.4's OnTileLoaded workaround
					BlackspotManager.EnsureBlackspotsMarked();

					// HB 6.2.3 pattern: run pathfinding on a background thread so we
					// can release the FrameLock and let WoW render while we wait.
					var capturedMapId = mapId;
					var capturedStart = start;
					var capturedEnd = end;
					var pathTask = Task<TripperNav.PathFindResult>.Factory.StartNew(
						() => TripperNavigator.FindPath(capturedMapId, capturedStart, capturedEnd, true));

					TripperNav.PathFindResult result;
					try
					{
						if (pathTask.Wait(10))
						{
							// Fast path — pathfinding completed within 10ms, no need to release frame
							result = pathTask.Result;
						}
						else
						{
							// Slow path — release FrameLock so WoW can render while we wait
							using (StyxWoW.Memory.ReleaseFrame(true))
							{
								int tickInterval = TreeRoot.TicksPerSecond > 0
									? 1000 / TreeRoot.TicksPerSecond
									: 50;
								while (!pathTask.Wait(tickInterval))
								{
									try
									{
										StyxWoW.Memory.ClearCache();
										using (StyxWoW.Memory.AcquireFrame())
										{
											ObjectManager.Update();
											WoWMovement.Pulse();
										}
										StyxWoW.ResetAfk();
									}
									catch (Exception ex)
									{
										Logging.WriteDebug("Exception during pathfind wait: {0}", ex.Message);
									}
								}
								result = pathTask.Result;
							}
							// Re-update ObjectManager after re-acquiring frame
							ObjectManager.Update();
						}
					}
					catch (AggregateException ex)
					{
						Logging.WriteDebug("Pathfinding failed with exception: {0}", ex.InnerException?.Message ?? ex.Message);
						return MoveResult.PathGenerationFailed;
					}
					finally
					{
						pathTask.Dispose();
					}

					LogPathResult(result, me.Location, destination, mapId);
					if (result.Status.Succeeded && result.Points != null && result.Points.Length > 0)
					{
						// HB 4.3.4: if the path is partial (navmesh doesn't fully connect
						// start to destination), log it and consider flight paths.
						if (result.IsPartialPath)
						{
							Logging.WriteDebug("Could not generate full path from {0} to {1}", me.Location, destination);

							// COPILOTBUDDY ENHANCEMENT (not in HB): try flight paths on partial
							// paths regardless of distance. HB only checks flights BEFORE
							// pathfinding (distance > threshold). We add this because our
							// pre-loaded tiles may miss corridors that HB's on-demand loading
							// would find, resulting in partial paths where HB gets full paths.
							if (FlightPaths.ShouldTakeFlightpath(me.Location, destination, me.MovementInfo.RunSpeed))
							{
								if (FlightPaths.SetFlightPathUsage(me.Location, destination, out _, out _))
								{
									return MoveResult.PathGenerated;
								}
							}
							// No flight path available — follow the partial path anyway
							// (HB 4.3.4 logs warning but still follows partial paths).
						}

						foreach (var point in result.Points)
						{
							_currentPath.Add(new WoWPoint(point.X, point.Y, point.Z));
						}

						// Store path metadata for off-mesh/terrain handling (P4.1 fix)
						_currentFlags = result.Flags;
						_currentPolyTypes = result.PolyTypes;
						_currentAbilityFlags = result.AbilityFlags;
						_isPartialPath = result.IsPartialPath;
					}
					else
					{
						// HB 4.3.4: path generation failed — return failure WITHOUT stopping movement.
						// HB never calls MoveStop here; the behavior tree resumes and
						// retries on the next tick. Calling MoveStop would halt the character,
						// causing WoW to auto-sit after ~4s of idleness.
						return MoveResult.PathGenerationFailed;
					}
				}
				else
				{
					// HB 4.3.4: no navmesh available — return failure WITHOUT stopping movement.
					// HB never calls MoveStop here (MeshNavigator returns PathGenerationFailed only).
					return MoveResult.PathGenerationFailed;
				}

				_currentPathIndex = 0;
				_suppressDriftCheck = false;
				_suppressDriftCheckIndex = 0;

				// Path start skip (HB 6.2.3 method_14): skip early visible waypoints via raycast.
				// Walk forward through path and raycast from player position — if we can see
				// waypoint N directly, skip to it for smoother movement (avoids zigzag on flat terrain).
				// Stop before any off-mesh connection.
				if (_currentPath.Count > 2 && _currentFlags != null)
				{
					uint skipMapId = (uint)(GetCurrentMapId());
					var playerVec = new Vector3(me.Location.X, me.Location.Y, me.Location.Z);
					int lastVisible = 0;

					// GAP 8 — HB 6.2.3 method_13: ensure player tile is loaded once before the loop.
					try { TripperNavigator.EnsureTilesAroundPosition(skipMapId, playerVec, 0); }
					catch { /* safe failure */ }

					for (int i = 1; i < Math.Min(_currentPath.Count - 1, 6); i++) // Check up to 5 waypoints ahead
					{
						// Don't skip past off-mesh connections (HB 6.2.3 method_14: flag & 4)
						if (i < _currentFlags.Length &&
						    (_currentFlags[i] & TripperNav.StraightPathFlags.OffMeshConnection) != 0)
							break;

						var wp = new Vector3(_currentPath[i].X, _currentPath[i].Y, _currentPath[i].Z);

						// GAP 8 — HB 6.2.3 method_13: ensure tile at candidate waypoint is loaded.
						try { TripperNavigator.EnsureTilesAroundPosition(skipMapId, wp, 0); }
						catch { /* safe failure — tiles may already be loaded */ }

						// GAP 7 — HB 6.2.3 method_13: raycast with area type validation.
						// Not only checks hitT for boundary intersection, but also iterates
						// visited polygons and rejects rays crossing non-Ground/Water/Road/faction polys
						// (e.g., Steep, Lava, Blocked areas).
						bool blocked = TripperNavigator.RaycastBlocked(skipMapId, playerVec, wp, out float hitT, _factionAreaType);
						if (!blocked && hitT >= 1.0f)
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
						// HB behavior: advance the path index instead of mutating the path.
						_currentPathIndex = lastVisible;
						_suppressDriftCheck = true;
						_suppressDriftCheckIndex = lastVisible;

						// HB 6.2.3 logs only when skipping 2+ nodes.
						if (lastVisible >= 2)
							Logging.WriteDebug("Skipped {0} path nodes", lastVisible);
					}
				}
			}

			// Move along path
			// HB 4.3.4/6.2.3: Keep following the existing navmesh path even during combat.
			// HB NEVER clears a path just because combat started — the path is valid navigation
			// data and should be followed to avoid walking through walls/obstacles.
			if (_currentPath.Count > 0 && _currentPathIndex < _currentPath.Count)
			{

				// HB 6.2.3: Once in elevator mode (method_20), keep calling the elevator handler
				// every tick regardless of player position relative to offmesh start.
				// This prevents the offmesh precision check from pulling the bot back to the gate.
				if (_inElevatorSequence)
				{
					var elevResult = HandleElevator(me, _elevatorTarget);
					if (elevResult != null)
						return elevResult.Value;
					// HandleElevator returned null = ride complete.
					// Force path regeneration from current position: the old path's
					// remaining waypoints were rooted at the offmesh endpoint, which may
					// be offset from the actual elevator exit (mmap-extractor tiles).
					_inElevatorSequence = false;
					_elevatorTarget = WoWPoint.Zero;
					_stuckCheckTimer.Reset();
					_unstickAttempts = 0;
					try { StuckHandler.Reset(); } catch { }
					_currentPath.Clear();
					_currentPathIndex = 0;
					_suppressDriftCheck = false;
					_suppressDriftCheckIndex = 0;
					_currentFlags = null;
					_currentPolyTypes = null;
					_currentAbilityFlags = null;
					return MoveResult.Moved;
				}

				// Door handling (HB 6.2.3 method_7): auto-detect and interact with closed doors on path
				var doorResult = HandleDoors(me);
				if (doorResult != null)
					return doorResult.Value;

				// Stuck detection — integrated directly in MoveTo() to cover ALL callers
				// (ActionMoveToPoi, corpse run, loot, hotspot, plugins, etc.)
				// Suppress stuck detection when at an off-mesh connection point (elevator, portal):
				// the bot may be intentionally stopped, waiting for elevator to arrive.
				bool isAtOffMesh = _currentFlags != null && _currentPathIndex < _currentFlags.Length
					&& (_currentFlags[_currentPathIndex] & TripperNav.StraightPathFlags.OffMeshConnection) != 0;
				if (_stuckCheckTimer.IsFinished && !isAtOffMesh && !_ridingElevator && !_inElevatorSequence)
				{
					_stuckCheckTimer.Reset();
					if (StuckHandler.IsStuck())
					{
						_unstickAttempts++;
						if (_unstickAttempts >= MaxUnstickAttempts)
						{
							// After MaxUnstickAttempts failed attempts, add blackspot and force path regeneration.
							// Without the blackspot, the new path goes through the same impassable terrain.
							var me2 = ObjectManager.Me;
							if (me2 != null)
							{
								Logging.WriteDebug("[NAV] {0} unstick attempts failed — adding blackspot at current location.", MaxUnstickAttempts);
								BlackspotManager.AddBlackspot(me2.Location, 5f, 3f);
							}
							_unstickAttempts = 0;
							_currentPath.Clear();
							_currentPathIndex = 0;
							_suppressDriftCheck = false;
							_suppressDriftCheckIndex = 0;
							_pathRegenThrottle = new WaitTimer(TimeSpan.FromMilliseconds(500)); // Allow immediate regen
							try { StuckHandler.Reset(); } catch { }
							return MoveResult.Failed;
						}
						StuckHandler.Unstick();
						return MoveResult.UnstuckAttempt;
					}
				}

				WoWPoint nextPoint = _currentPath[_currentPathIndex];

				// Path validity check (P6.7): if player has drifted far from the current path segment
				// (knockback, teleport, fear, etc.), force path regeneration instead of following stale path
				if (_currentPathIndex > 0 && !_ridingElevator && !_inElevatorSequence
				    && !(_suppressDriftCheck && _currentPathIndex == _suppressDriftCheckIndex)
				    // GAP 2a — HB 6.2.3 method_15 line 1: skip drift check while falling
				    && !me.IsFalling
				    // GAP 2b — HB 6.2.3 method_15: skip drift check on off-mesh connection segments (flag & 4)
				    && !(_currentFlags != null && (_currentPathIndex - 1) < _currentFlags.Length
				         && (_currentFlags[_currentPathIndex - 1] & TripperNav.StraightPathFlags.OffMeshConnection) != 0))
				{
					WoWPoint prevPoint = _currentPath[_currentPathIndex - 1];

					// GAP 4 — HB 6.2.3 method_15: raycast pre-check before distance-to-segment.
					// If player can see the next waypoint via clear raycast, the player is
					// on valid mesh heading the right way → NOT drifted, regardless of geometric distance.
					// Only fall through to distance check if raycast is blocked.
					// AUDIT FIX: Use RaycastBlocked (area-type checking) to match HB 6.2.3 method_15
					// which calls method_13 (not plain raycast). This prevents declaring "on path"
					// when the ray crosses a non-walkable area type (Lava, Blocked, Steep).
					bool raycastClear = false;
					uint driftMapId = (uint)(GetCurrentMapId());
					var playerVecDrift = new Vector3(me.Location.X, me.Location.Y, me.Location.Z);
					var nextVecDrift = new Vector3(nextPoint.X, nextPoint.Y, nextPoint.Z);
					bool blocked = TripperNavigator.RaycastBlocked(driftMapId, playerVecDrift, nextVecDrift, out float driftHitT, _factionAreaType);
					if (!blocked && driftHitT >= 1.0f)
					{
						raycastClear = true; // clear path on valid mesh → not drifted
					}

					if (!raycastClear)
					{
						// HB 6.2.3 method_15: use 2D distance (Z ignored) and tighter threshold.
						// HB uses PathPrecision (2yd), not 5× that. 10yd was too loose —
						// the bot could be on the wrong side of a wall and still follow stale waypoints.
						float distToSegment = DistanceToLineSegment2D(me.Location, prevPoint, nextPoint);
						if (distToSegment > PathPrecision * 2f) // >4 yards off path = stale (HB 6.2.3 uses PathPrecision)
						{
							// BUG FIX: Don't set _destination = WoWPoint.Zero here.
							// That caused destinationChanged=true on next MoveTo(), resetting
							// StuckHandler mid-unstick sequence. Instead, just clear the path
							// so it gets regenerated, but keep _destination intact.
							_currentPath.Clear();
							_currentPathIndex = 0;
							_suppressDriftCheck = false;
							_suppressDriftCheckIndex = 0;
							_pathRegenThrottle = new WaitTimer(TimeSpan.FromMilliseconds(500)); // Allow immediate regen
							return MoveResult.Moved;
						}
					}
				}
				
				// HB 4.3.4 method_5 / HB 6.2.3 method_24: while-loop to advance through
				// ALL reached waypoints in a single tick, preventing micro-pauses between nodes.
				// Uses 2D distance² ≤ precision² AND |ΔZ| < 4.5 (HB 6.2.3 method_27).
				while (_currentPathIndex < _currentPath.Count)
				{
					nextPoint = _currentPath[_currentPathIndex];
					bool isFinalPoint = (_currentPathIndex == _currentPath.Count - 1);
					float waypointPrecision = isFinalPoint ? precision : PathPrecision;
					float distance2DSqr = me.Location.Distance2DSqr(nextPoint);
					float zDiff = Math.Abs(me.Location.Z - nextPoint.Z);
					bool reachedWaypoint = distance2DSqr <= waypointPrecision * waypointPrecision && zDiff < 4.5f;

					if (!reachedWaypoint)
						break; // Not at this waypoint yet — stop advancing

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
						
						// Dispatch off-mesh connection based on AreaType (ported from HB 4.3.4 method_4)
						WoWPoint offMeshEnd = (_currentPathIndex + 1 < _currentPath.Count)
							? _currentPath[_currentPathIndex + 1]
							: nextPoint;

						if (_currentPolyTypes != null && _currentPathIndex < _currentPolyTypes.Length)
						{
							var offMeshResult = HandleOffMeshConnection(me, offMeshEnd, _currentPolyTypes[_currentPathIndex]);
							if (offMeshResult != null)
								return offMeshResult.Value;
						}
						else
						{
							var offMeshResult = HandleOffMeshConnection(me, offMeshEnd, TripperNav.AreaType.Ground);
							if (offMeshResult != null)
								return offMeshResult.Value;
						}
						// Off-mesh connection is also an advance boundary (HB 4.3.4/6.2.3: break on flag & 4)
						break;
					}

					// Reset stuck timer on successful waypoint advance
					_stuckCheckTimer.Reset();
					_unstickAttempts = 0;
					try { StuckHandler.Reset(); } catch { }

					_currentPathIndex++;
					if (_suppressDriftCheck && _currentPathIndex > _suppressDriftCheckIndex)
					{
						_suppressDriftCheck = false;
						_suppressDriftCheckIndex = 0;
					}
					if (_currentPathIndex >= _currentPath.Count)
					{
						if (_isPartialPath)
							return MoveResult.Failed;
						return MoveResult.ReachedDestination;
					}

					// HB 4.3.4 method_5 / HB 6.2.3 method_24: after advancing, immediately
					// issue MoveTowards on the new next waypoint so movement never pauses.
					// (The loop will check if we've also reached THIS new waypoint.)
				}

				// Now move towards the current waypoint with push-ahead
				nextPoint = _currentPath[_currentPathIndex];
				bool isLastPoint = (_currentPathIndex == _currentPath.Count - 1);

				// GAP 5 — HB 6.2.3 method_25: apply water/lava +2f to nextPoint BEFORE push-ahead.
				// Previously +2f was applied to clickPoint, but push-ahead recomputed pushedPoint
				// from nextPoint.Z (without +2f), then replaced clickPoint → +2f was LOST.
				// HB 6.2.3 method_25 applies +2f before calling method_26 (push-ahead).
				if (_currentPolyTypes != null && _currentPathIndex < _currentPolyTypes.Length)
				{
					var polyType = _currentPolyTypes[_currentPathIndex];
					if (polyType == TripperNav.AreaType.Water || polyType == TripperNav.AreaType.Lava)
					{
						nextPoint = new WoWPoint(nextPoint.X, nextPoint.Y, nextPoint.Z + 2f);
					}
				}

				WoWPoint clickPoint = nextPoint;

				// HB 6.2.3 method_26: push waypoint ahead by PathPrecision in movement direction,
				// but ONLY if a navmesh raycast confirms the pushed point is still on valid mesh.
				// Without raycast validation, the push goes through wall corners causing wall-running.
				// GAP 3: use tight extents (0.5, 3, 0.5) for FindNearestPoly — HB 6.2.3 method_26
				// prevents snapping to wrong floor in multi-layer areas.
				if (!isLastPoint && _currentPathIndex > 0)
				{
					WoWPoint direction = nextPoint - me.Location;
					float length = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z);
					if (length > 0.01f)
					{
						direction = new WoWPoint(direction.X / length, direction.Y / length, direction.Z / length);
						WoWPoint pushedPoint = new WoWPoint(
							nextPoint.X + direction.X * PathPrecision,
							nextPoint.Y + direction.Y * PathPrecision,
							nextPoint.Z + direction.Z * PathPrecision);

						// GAP 3: HB 6.2.3 method_26 uses tight extents (0.5f, 3f, 0.5f) for the
						// FindNearestPoly that resolves the start polygon for the raycast.
						// This prevents the push-ahead from snapping to a wrong-floor poly.
						uint pushMapId = (uint)(GetCurrentMapId());
						var waypointVec = new Vector3(nextPoint.X, nextPoint.Y, nextPoint.Z);
						var pushedVec = new Vector3(pushedPoint.X, pushedPoint.Y, pushedPoint.Z);
						var tightExtents = new Vector3(0.5f, 3f, 0.5f);

						var status = TripperNavigator.RaycastWithExtents(
							pushMapId, waypointVec, pushedVec, tightExtents,
							out float hitT, out _, out _, out _);

						// HB 6.2.3 method_26: only apply push if raycast is completely clear.
						// HB checks num == 3.40282347E+38f (FLT_MAX); Detour returns FLT_MAX when no wall hit.
						if (status.Succeeded && hitT == float.MaxValue)
						{
							clickPoint = pushedPoint;
						}
						// else: raycast hit an obstruction — use exact waypoint (no push)
					}
				}

				// HB 4.3.4/6.2.3: route through Navigator.PlayerMover.MoveTowards()
				// instead of calling WoWMovement.ClickToMove() directly.
				// This respects the standard movement pipeline and allows custom movers.
				PlayerMover.MoveTowards(new Tripper.XNAMath.Vector3(clickPoint.X, clickPoint.Y, clickPoint.Z));
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

			// TEST: Commenté pour isoler crash - EnsureTilesAroundPosition suspect
			// try
			// {
			// 	TripperNavigator.EnsureTilesAroundPosition(mapId, startVec, LoadTilesAroundRadius);
			// 	TripperNavigator.EnsureTilesAroundPosition(mapId, endVec, LoadTilesAroundRadius);
			// }
			// catch { }

			BlackspotManager.EnsureBlackspotsMarked();

			var result = TripperNavigator.FindPath(mapId, startVec, endVec, true);
			
			// HB 4.3.4 MeshNavigator.CanNavigateFully:
			// path.Succeeded && !path.IsPartialPath
			if (result.Status.Succeeded && !result.IsPartialPath)
			{
				return true;
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

			// TEST: Commenté pour isoler crash - EnsureTilesAroundPosition suspect
			// try
			// {
			// 	TripperNavigator.EnsureTilesAroundPosition(mapId, startVec, LoadTilesAroundRadius);
			// 	TripperNavigator.EnsureTilesAroundPosition(mapId, endVec, LoadTilesAroundRadius);
			// }
			// catch { }

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
		/// <summary>
		/// 2D distance from a point to a line segment (Z axis ignored).
		/// HB 6.2.3 method_15/smethod_1: zeroes Z for drift detection to avoid
		/// false positives on ramps/stairs where Z delta inflates 3D distance.
		/// </summary>
		private static float DistanceToLineSegment2D(WoWPoint point, WoWPoint segA, WoWPoint segB)
		{
			float dx = segB.X - segA.X;
			float dy = segB.Y - segA.Y;
			float lenSqr = dx * dx + dy * dy;

			if (lenSqr < 0.0001f)
			{
				// Segment is essentially a point — 2D distance
				float px = point.X - segA.X;
				float py = point.Y - segA.Y;
				return (float)Math.Sqrt(px * px + py * py);
			}

			// Project point onto segment, clamped to [0, 1]
			float t = ((point.X - segA.X) * dx + (point.Y - segA.Y) * dy) / lenSqr;
			t = Math.Max(0f, Math.Min(1f, t));

			// Closest point on segment (2D only)
			float closestX = segA.X + t * dx;
			float closestY = segA.Y + t * dy;

			float ex = point.X - closestX;
			float ey = point.Y - closestY;
			return (float)Math.Sqrt(ex * ex + ey * ey);
		}

		/// <summary>
		/// Handles off-mesh connection dispatch based on AreaType.
		/// Ported from HB 4.3.4 MeshNavigator.method_4 / HB 6.2.3 method_18.
		/// Returns null to continue normal waypoint advancement, or a MoveResult to return immediately.
		/// </summary>
		/// <remarks>
		/// MaNGOS mmap-extractor bakes offmesh connections with Recast default area type 63
		/// (RC_WALKABLE_AREA). Additionally, the Detour straightPathPolys array sometimes maps
		/// the offmesh start vertex to the adjacent GROUND poly (AreaType=1) rather than the
		/// offmesh poly itself, depending on path geometry. Therefore we detect elevator by
		/// Z-delta FIRST, regardless of AreaType — this is the only reliable indicator.
		/// </remarks>
		private static MoveResult? HandleOffMeshConnection(LocalPlayer me, WoWPoint targetPoint, TripperNav.AreaType areaType)
		{
			// Detect elevator by Z-delta geometry FIRST, regardless of AreaType.
			// This handles both RC_WALKABLE_AREA=63 (mmap-extractor default) and Ground=1
			// (when Detour assigns the ground poly at the offmesh start vertex).
			float zDelta = Math.Abs(me.Z - targetPoint.Z);
			if (zDelta > 10f)
			{
				return HandleElevator(me, targetPoint);
			}
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
					// Standard run/jump offmesh — walk to target, advance path
					WoWMovement.ClickToMove(targetPoint);
					return null;
			}
		}

		/// <summary>
		/// Standard off-mesh connection handler for Run/Jump connections.
		/// Ported from HB 6.2.3 MeshNavigator.method_19.
		/// NOTE: MaNGOS mmap-extractor sets default flags (255) on all offmesh connections.
		/// These don't match HB's custom flag convention, so we only check ability flags
		/// when the area type was set by HB's custom mesh builder (not Recast default 63).
		/// For mmap-extractor connections, HandleOffMeshConnection auto-detects by geometry.
		/// </summary>
		private static MoveResult? HandleStandardOffMesh(LocalPlayer me, WoWPoint targetPoint)
		{
			// Check ability flags — only meaningful for HB-format meshes where
			// the area type was explicitly set (not RC_WALKABLE_AREA = 63).
			// mmap-extractor offmesh connections are handled in HandleOffMeshConnection
			// before reaching here, so this code path is only for HB-compatible tiles.
			if (_currentAbilityFlags != null && _currentPathIndex < _currentAbilityFlags.Length)
			{
				var abilityFlags = _currentAbilityFlags[_currentPathIndex];

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
		/// Elevator handling — matches HB 4.3.4 MeshNavigator (AreaType.Elevator).
		/// Called every tick while at an elevator offmesh connection.
		/// </summary>
		private static MoveResult? HandleElevator(LocalPlayer me, WoWPoint targetPoint)
		{
			static WoWPoint GetTransportWorldLocation(WoWGameObject transportGo)
			{
				if (transportGo.SubType == WoWGameObjectType.Transport
				    || transportGo.SubType == WoWGameObjectType.MapObjectTransport)
				{
					try
					{
						Tripper.Tools.Math.Matrix worldMatrix = transportGo.GetWorldMatrix();
						Matrix4x4 mat = worldMatrix;
						Vector3 translation = mat.Translation;
						return new WoWPoint(translation.X, translation.Y, translation.Z);
					}
					catch
					{
						// Fall back to base position if matrix read fails
					}
				}
				return transportGo.Location;
			}

			// HB 4.3.4: reset stuck handler every tick
			try { StuckHandler.Reset(); } catch { }
			_stuckCheckTimer.Reset();

			// HB 6.2.3-style lock: keep calling elevator handler every tick once entered
			if (!_inElevatorSequence)
			{
				_inElevatorSequence = true;
				_elevatorTarget = targetPoint;
				_hbElevatorFlag = false;
			}

			WoWPoint playerPos = me.Location;

			// HB 4.3.4: find nearest transport GO
			var transport = ObjectManager.GetObjectsOfType<WoWGameObject>(false, false)
				.Where(go => go.SubType == WoWGameObjectType.Transport
				              && go.Entry != 20657 && go.Entry != 20656)
				.OrderBy(go => GetTransportWorldLocation(go).Distance2DSqr(playerPos))
				.FirstOrDefault();

			WoWPoint endPoint = targetPoint; // offmesh END
			WoWPoint startPoint = (_currentPathIndex >= 0 && _currentPathIndex < _currentPath.Count)
				? _currentPath[_currentPathIndex]
				: playerPos; // offmesh START (waiting spot)

			if (transport == null)
			{
				Logging.WriteDiagnostic("There is no elevator around. Something is wrong");
				return MoveResult.Failed;
			}

			WoWPoint transportWorldLocation = GetTransportWorldLocation(transport);

			// HB 4.3.4: if at endpoint, complete offmesh and reset flag
			if (playerPos.DistanceSqr(endPoint) < 4f)
			{
				_hbElevatorFlag = false;
				return null;
			}

			if (me.IsOnTransport)
			{
				_ridingElevator = true;
				if (Math.Abs(playerPos.Z - endPoint.Z) > 2f)
				{
					Styx.Logic.BehaviorTree.TreeRoot.StatusText = "Waiting for elevator to reach end point";
					return MoveResult.Moved;
				}
				if (!_hbElevatorFlag)
					_hbElevatorFlag = true;
				Logging.WriteDiagnostic("Moving out of elevator");
				WoWMovement.ClickToMove(endPoint);
				return MoveResult.Moved;
			}

			// NOT on transport
			_ridingElevator = false;
			if (Math.Abs(transportWorldLocation.Z - playerPos.Z) > 2f)
			{
				if (me.Location.DistanceSqr(startPoint) > 4f)
				{
					Logging.WriteDiagnostic("Woops, we are not in the waiting spot");
					WoWMovement.ClickToMove(startPoint);
					return MoveResult.Moved;
				}
				// GAP 6 — HB 6.2.3 method_20: face toward elevator while waiting
				// Prevents the character from standing sideways and falling off when it arrives.
				// Uses 15° arc (tight) — HB 6.2.3 line ~869.
				if (!me.IsSafelyFacing(transport, 15f))
				{
					WoWMovement.MoveStop();
					me.SetFacing(transport.Location);
				}
				Styx.Logic.BehaviorTree.TreeRoot.StatusText = "Waiting for the elevator";
				return MoveResult.Moved;
			}

			if (!_hbElevatorFlag)
			{
				Logging.WriteDiagnostic("Moving inside elevator");
				if (me.Mounted)
					Mount.Dismount();
				// Center on the platform like HB: move toward transport.Location (world-space)
				WoWMovement.ClickToMove(transportWorldLocation);
				return MoveResult.Moved;
			}

			return MoveResult.Failed;
		}

		/// <summary>
		/// Portal handling: find nearest Goober or SpellCaster game object and interact.
		/// HB 4.3.4 pattern from MeshNavigator.method_4.
		/// </summary>
		private static MoveResult? HandlePortal(LocalPlayer me)
		{
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
				Logging.WriteDiagnostic("Could not find portal to take.");
				return MoveResult.Failed;
			}

			if (bestPortal.WithinInteractRange)
			{
				Logging.WriteDiagnostic("Interacting with:{0}", bestPortal.Name);
				bestPortal.Interact();
				return MoveResult.Moved;
			}

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
var unit = ObjectManager.CachedUnits
				.Where(u => !u.IsDead && !u.IsHostile && !u.PlayerControlled && !u.IsPlayer
				            && u.Location.DistanceSqr(me.Location) < maxSearchDistSqr)
				.OrderBy(u => u.Location.DistanceSqr(me.Location))
				.FirstOrDefault();

			if (unit == null)
			{
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
		/// Logs the result of a pathfinding operation.
		/// Matches HB 6.2.3 MeshNavigator.method_12 logging pattern.
		/// </summary>
		private static void LogPathResult(Tripper.Navigation.PathFindResult result, WoWPoint start, WoWPoint end, uint mapId)
		{
			_ = result;
			_ = start;
			_ = end;
			_ = mapId;

			try
			{
				// Read native navstats (if available) to expose timing / polys visited
				var stats = TripperNavigator.GetNavStats();
				Logging.WriteDebug("[NAV] Pathfind: time={0}ms polysVisited={1} pathLength={2:F1} shortcuts={3} stuckRecoveries={4}",
					stats.PathfindTimeMs, stats.PolysVisited, stats.PathLength, stats.ShortcutsApplied, stats.StuckRecoveries);
			}
			catch (Exception ex)
			{
				Logging.WriteDebug("[NAV] Failed to read NavStats: {0}", ex.Message);
			}
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
