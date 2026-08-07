using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.WoWInternals.World;
using TripperNav = Tripper.Navigation;

namespace Styx.Logic.Pathing
{
	public static class Navigator
	{
		private static TripperNav.Navigator? _navigator;
		private static IPlayerMover _playerMover = new ClickToMoveMover();
		private static ITerrainHeightProvider _heightProvider = new NavigatorTerrainHeightProvider();

		// HB WoD architecture: MeshNavigator is the concrete INavigationProvider that handles
		// all ground navmesh path following. Navigator is the static facade that routes to it.
		private static MeshNavigator? _meshNavigator;

		// WotLK no-fly zone IDs — indoor dungeon areas where flying makes no sense.
		// NOTE: Dalaran (4395) is NOT listed here. Flying was allowed in Dalaran in WotLK 3.3.5a
		// (the no-fly restriction was introduced in Cataclysm). Flightor relies on IsFlyableArea()
		// from the Lua API to detect the actual flyable status.
		private static readonly HashSet<uint> _noFlyZoneIds = new HashSet<uint>
		{
			4613, // The Pit of Saron (indoor dungeon entrance area)
			4820, // Halls of Reflection
		};

		// HB 6.2.3 AvoidanceNavigationProvider pattern: geometric obstacle avoidance.
		// WorldObstacleManager.Initialize() sets these two delegates on bot start so that
		// ALL bots (Quest, Grind, Dungeon) benefit from avoidance without circular coupling.
		// These are wired into MeshNavigator's MoveTo() internally.

		/// <summary>
		/// Called every WoWPulsator tick to scan ObjectManager and refresh avoidance zones.
		/// Set by Bots.DungeonBuddy.Avoidance.WorldObstacleManager.Initialize().
		/// </summary>
		public static Action? NavAvoidanceUpdater { get; set; }

		/// <summary>
		/// Returns the geometric detour path (WoWPoint[]) the bot should follow to route
		/// around registered obstacles for the given destination. Null = no avoidance needed.
		/// Set by Bots.DungeonBuddy.Avoidance.WorldObstacleManager.Initialize().
		/// HB 6.2.3 equivalent: AvoidanceNavigationProvider.MovePath() → Helpers.GetAvoidPath().
		/// </summary>
		public static Func<WoWPoint, WoWPoint[]?>? NavAvoidWaypointProvider { get; set; }

		/// <summary>
		/// Returns the remaining unvisited navmesh waypoints from the current path index.
		/// Used internally by Bots.DungeonBuddy.Avoidance.Helpers.GetAvoidPath().
		/// </summary>
		public static WoWPoint[] GetRemainingNavPath()
		{
			return _meshNavigator?.GetRemainingNavPath() ?? Array.Empty<WoWPoint>();
		}

		/// <summary>
		/// Pathfinds from → to via TripperNavigator and returns the waypoints as WoWPoint[].
		/// Lightweight wrapper: no tile pre-load, no blackspot sync. For avoidance use only.
		/// </summary>
		public static WoWPoint[] ComputeRawPath(WoWPoint from, WoWPoint to)
		{
			if (!IsNavigatorLoaded) return Array.Empty<WoWPoint>();
			try
			{
				uint mapId = GetCurrentMapId();
				var result = TripperNavigator.FindPath(mapId,
					new Vector3(from.X, from.Y, from.Z),
					new Vector3(to.X, to.Y, to.Z), true);
				if (result == null || !result.Status.Succeeded || result.Points == null || result.Points.Length == 0)
					return Array.Empty<WoWPoint>();
				return result.Points.Select(p => new WoWPoint(p.X, p.Y, p.Z)).ToArray();
			}
			catch { return Array.Empty<WoWPoint>(); }
		}

		/// <summary>
		/// Replaces the active navmesh path. Called by Helpers.GetAvoidPath() when the
		/// obstacle set changes and a fresh path from current position is needed.
		/// Equivalent to HB 6.2.3: CurrentMovePath.Path = FindPath(from, to); Index = 0.
		/// </summary>
		public static void OverrideCurrentPath(WoWPoint[] points)
		{
			_meshNavigator?.OverrideCurrentPath(points);
		}

		/// <summary>true when there are unvisited waypoints in the current navmesh path.</summary>
		public static bool HasActivePath => _meshNavigator?.HasActivePath ?? false;

		// HB 6.2.3: stored fallback precision used when there is no NavigationProvider.
		private static float _defaultPathPrecision = 2f;

		// HB 6.2.3 Navigator.PathPrecision delegates to NavigationProvider.PathPrecision.
		// Falls back to a stored default when the provider is not yet initialized.
		public static float PathPrecision
		{
			get => _currentProvider != null ? _currentProvider.PathPrecision : _defaultPathPrecision;
			set
			{
				_defaultPathPrecision = value;
				if (_currentProvider != null)
					_currentProvider.PathPrecision = value;
			}
		}
		public static int LoadTilesAroundRadius { get; set; } = 1;
		public static float FlyingMountHeight { get; set; } = 25f;

		/// <summary>
		/// Gets or sets the player mover used for movement control.
		/// </summary>
		public static IPlayerMover PlayerMover
		{
			get => _playerMover;
			set
			{
				if (ReferenceEquals(_playerMover, value))
					return;

				var old = _playerMover;
				_playerMover = value ?? new ClickToMoveMover();
				OnPlayerMoverChanged?.Invoke(null, new NavigationProviderChangedEventArgs<IPlayerMover>(old, _playerMover));
			}
		}

		public static ITerrainHeightProvider HeightProvider
		{
			get => _heightProvider;
			set
			{
				if (ReferenceEquals(_heightProvider, value))
					return;

				var old = _heightProvider;
				_heightProvider = value ?? new NavigatorTerrainHeightProvider();
				OnHeightProviderChanged?.Invoke(null, new NavigationProviderChangedEventArgs<ITerrainHeightProvider>(old, _heightProvider));
			}
		}

		/// <summary>
		/// Gets or sets the stuck handler used for stuck detection and recovery.
		/// </summary>
		public static StuckHandler StuckHandler
		{
			get
			{
				if (_meshNavigator != null)
					return _meshNavigator.StuckHandler;
				var fallback = new DefaultStuckHandler();
				fallback.OnSetAsCurrent();
				return fallback;
			}
			set
			{
				if (_meshNavigator != null)
					_meshNavigator.StuckHandler = value;
			}
		}

		public static WoWPoint Destination => _meshNavigator?.Destination ?? WoWPoint.Zero;

		public static List<WoWPoint> CurrentPath => _meshNavigator?.CurrentPath ?? new List<WoWPoint>();

		/// <summary>true if the player is at the given point (HB 6.2.3 Navigator.AtLocation(WoWPoint)).</summary>
		public static bool AtLocation(WoWPoint point)
		{
			return AtLocation(ObjectManager.Me?.Location ?? WoWPoint.Zero, point);
		}

		/// <summary>HB 6.2.3 Navigator.AtLocation — delegates to NavigationProvider.AtLocation.</summary>
		public static bool AtLocation(WoWPoint point1, WoWPoint point2)
		{
			if (_currentProvider == null)
				throw new InvalidOperationException("No navigation provider is set");
			return _currentProvider.AtLocation(point1, point2);
		}

		/// <summary>true if currently riding an elevator (blocks mount-up).</summary>
		public static bool IsRidingElevator => _meshNavigator?.IsRidingElevator ?? false;

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

		// Navigation provider management (HB 6.2.3 compatibility)
		private static NavigationProvider? _currentProvider;

        /// <summary>
        /// The active navigation provider. Setting this property raises the
        /// <see cref="OnNavigationProviderChanged"/> event if the value differs.
        /// </summary>
		public static NavigationProvider? NavigationProvider
        {
            get => _currentProvider;
            set
            {
                if (ReferenceEquals(_currentProvider, value))
                    return;
                var old = _currentProvider;
                // HB 6.2.3 order: OnSetAsCurrent on new FIRST, then OnRemoveAsCurrent on old,
                // then store. If OnSetAsCurrent throws, _currentProvider is not changed.
                if (value != null)
                    value.OnSetAsCurrent();
                if (old != null)
                    old.OnRemoveAsCurrent();
                _currentProvider = value;
                OnNavigationProviderChanged?.Invoke(null, new NavigationProviderChangedEventArgs<NavigationProvider>(old, value));
            }
        }

        /// <summary>
        /// Convenience alias used by older HB code.
        /// </summary>
		public static NavigationProvider? CurrentProvider => _currentProvider;

        /// <summary>
        /// Fired when <see cref="NavigationProvider"/> is replaced.  Consumers
        /// such as <see cref="BlackspotManager"/> can (re)wire tile events.
        /// </summary>
		public static event EventHandler<NavigationProviderChangedEventArgs<NavigationProvider>>? OnNavigationProviderChanged;

		public static event EventHandler<NavigationProviderChangedEventArgs<IPlayerMover>>? OnPlayerMoverChanged;

		public static event EventHandler<NavigationProviderChangedEventArgs<ITerrainHeightProvider>>? OnHeightProviderChanged;

        /// <summary>
        /// Whether the underlying Tripper navigator instance has finished loading
        /// its meshes.  Mirrors HB's Navigator.IsNavigatorLoaded property.
        /// </summary>
        public static bool IsNavigatorLoaded => _navigator != null && _navigator.IsLoaded;

		/// <summary>
		/// HB 6.2.3 MeshNavigator.method_28: OnlyWhileAlive polygons are allowed while alive,
		/// and excluded while dead/ghost-running.
		/// </summary>
		private static void ApplyAliveQueryFilter(bool isAlive)
		{
			try
			{
				if (_navigator == null || !_navigator.IsLoaded)
					return;

				ushort onlyWhileAlive = (ushort)TripperNav.AbilityFlags.OnlyWhileAlive;
				ushort include = _navigator.GetIncludeFlags();
				ushort exclude = _navigator.GetExcludeFlags();

				if (isAlive)
				{
					exclude = (ushort)(exclude & ~onlyWhileAlive);
					include = (ushort)(include | onlyWhileAlive);
				}
				else
				{
					include = (ushort)(include & ~onlyWhileAlive);
					exclude = (ushort)(exclude | onlyWhileAlive);
				}

				_navigator.SetIncludeFlags(include);
				_navigator.SetExcludeFlags(exclude);
			}
			catch
			{
				// Match navigation's fail-soft behavior: pathfinding will decide with the current filter.
			}
		}

		/// <summary>
		/// HB 6.2.3 Navigator.PathDistance — delegates to NavigationProvider.PathDistance.
		/// Falls back to raw Tripper path calculation when no provider is set.
		/// </summary>
		public static float? PathDistance(WoWPoint from, WoWPoint to, float maxDistance = float.MaxValue)
		{
			if (_currentProvider != null)
				return _currentProvider.PathDistance(from, to, maxDistance);

			// Fallback: direct Tripper calculation when provider not yet initialized.
			try
			{
				if (from == WoWPoint.Zero || to == WoWPoint.Zero)
					return null;
				uint mapId = GetCurrentMapId();
				var start = new System.Numerics.Vector3(from.X, from.Y, from.Z);
				var end = new System.Numerics.Vector3(to.X, to.Y, to.Z);
				try
				{
					TripperNavigator.EnsureTilesAroundPosition(mapId, start, LoadTilesAroundRadius);
					TripperNavigator.EnsureTilesAroundPosition(mapId, end, LoadTilesAroundRadius);
				}
				catch { }

				BlackspotManager.EnsureBlackspotsMarked();
				ApplyAliveQueryFilter(StyxWoW.Me?.IsAlive ?? true);

				var result = TripperNavigator.FindPath(mapId, start, end, true);
				if (result == null || !result.Succeeded || result.IsPartialPath || result.Points == null || result.Points.Length == 0)
					return null;

				var pts = result.Points;
				float dist = System.Numerics.Vector3.Distance(start, pts[0]);
				dist += System.Numerics.Vector3.Distance(pts[pts.Length - 1], end);
				if (dist > maxDistance)
					return maxDistance;
				for (int i = 1; i < pts.Length; i++)
				{
					dist += System.Numerics.Vector3.Distance(pts[i - 1], pts[i]);
					if (dist > maxDistance)
						return maxDistance;
				}
				return (float?)dist;
			}
			catch { return null; }
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
			if (IsRidingElevator)
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

		private static void OnMapChanged(BotEvents.Player.MapChangedEventArgs args)
		{
			Clear();
		}

		/// <summary>
		/// Logs tile loading events from Navigation.dll.
		/// Mirrors HB 6.2.3 WorldMeshManager.TileLoaded → log pattern.
		/// </summary>
		private static void OnTileLoaded(object? sender, TripperNav.TileLoadedEventArgs e)
		{
			Logging.Write("Loading {0}_{1}_{2}", e.MapId, e.TileX, e.TileY);
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
						_navigator.TileLoaded += OnTileLoaded;
					}
				}
				catch (Exception ex) { _ = ex; }
			}

			// Set faction-aware query filter
			if (IsNavigatorLoaded && _navigator != null)
			{
				try
				{
					var me = StyxWoW.Me;
					if (me != null)
					{
						_navigator.SetFactionQueryFilter(me.IsHorde);
						var factionArea = me.IsHorde ? TripperNav.AreaType.Horde : TripperNav.AreaType.Alliance;

						// Create MeshNavigator and set faction area
						_meshNavigator = new MeshNavigator();
						_meshNavigator.SetFactionAreaType(factionArea);

						// Wire as active INavigationProvider
						NavigationProvider = _meshNavigator;
					}
				}
				catch (Exception ex) { _ = ex; }
			}

			// Initialize flight path system
			try { FlightPaths.Initialize(); }
			catch (Exception ex) { _ = ex; }
		}

		private static void OnBotStop(EventArgs args)
		{
			if (_navigator != null)
				_navigator.TileLoaded -= OnTileLoaded;
			Clear();
		}

		public static void Clear()
		{
			_meshNavigator?.Clear();
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

			if (float.IsNaN(destination.X) || float.IsNaN(destination.Y) || float.IsNaN(destination.Z))
				return MoveResult.Failed;

			LocalPlayer? me = ObjectManager.Me;
			if (me == null)
				return MoveResult.Failed;

			// Check if already at destination
			float distance = me.Location.Distance(destination);
			if (distance < precision)
				return MoveResult.ReachedDestination;

			// Auto-mount check (HB 6.2.3 MeshNavigator L231)
			// Guard: skip ground-mount logic when in a flyable zone — Flightor owns mounting there.
			// Without this guard, Navigator.MoveTo fires Mount.StateMount (LevelBot ground path)
			// the moment Flightor.MountHelper._mountTimer expires, casting a ground mount instead
			// of the flying mount and causing a 10-second CalculatePathEx FAILED spam loop.
			try
			{
				if (!Flightor.CanFly && Mount.ShouldMount(destination))
				{
					WoWPoint dest = destination;
					Mount.StateMount(() => dest);
				}
			}
			catch { }

			// Do not issue movement while a mount cast is in progress.
			if (me.IsCasting)
				return MoveResult.Moved;

			// NOTE: Navigator.MoveTo is PURE GROUND NAV — no Flightor routing here.
			// In HB, Flightor is called by the bot base directly. If Navigator.MoveTo
			// delegated to Flightor, and Flightor checks CanNavigateFully/GetPathDistance
			// which call Navigator.MoveTo → infinite recursion → StackOverflow.

			// Auto-dismount near destination
			try
			{
				if (Mount.ShouldDismount(destination))
					Mount.Dismount("Near destination");
			}
			catch { }

			// P6.9 — Flight path auto-detection (long distances only)
			try
			{
				float distanceSqr = me.Location.DistanceSqr(destination);
				if (distanceSqr > 160000f)
				{
					if (FlightPaths.ShouldTakeFlightpath(me.Location, destination, me.MovementInfo.RunSpeed))
					{
						if (FlightPaths.SetFlightPathUsage(me.Location, destination, out _, out _))
							return MoveResult.PathGenerated;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.WriteDebug("Flight path check threw: {0}", ex.Message);
			}

			// HB WoD architecture: delegate ground navmesh movement to MeshNavigator
			if (_meshNavigator == null)
				return MoveResult.Failed;

			return _meshNavigator.MoveTo(destination, precision, destinationName);
		}


		// HB 6.2.3 Navigator.CanNavigateFully — delegates to NavigationProvider.CanNavigateFully.
		// Throws InvalidOperationException if no provider is set (mirrors HB smethod_1 guard).
		public static bool CanNavigateFully(WoWPoint start, WoWPoint destination)
		{
			if (NavigationProvider == null)
				throw new InvalidOperationException("No navigation provider is set");
			return NavigationProvider.CanNavigateFully(start, destination);
		}

		public static bool CanNavigateWithin(WoWPoint start, WoWPoint destination, float distanceTolerance)
		{
			if (NavigationProvider == null)
				throw new InvalidOperationException("No navigation provider is set");
			return NavigationProvider.CanNavigateWithin(start, destination, distanceTolerance);
		}

		public static bool CanNavigateFully(WoWPoint destination)
		{
			LocalPlayer? me = ObjectManager.Me;
			if (me == null) return false;
			return CanNavigateFully(me.Location, destination);
		}

		/// <summary>
		/// Mirrors HB 4.3.4 Class81.method_10: returns true only if the actual nav path
		/// from <paramref name="start"/> to <paramref name="destination"/> is complete
		/// (not partial) and its total length in yards is ≤ <paramref name="maxPathDistance"/>.
		/// Used by DungeonTargeting to reject through-wall mobs whose straight-line distance
		/// is within range but whose nav path winds far around the dungeon geometry.
		/// </summary>
		public static bool CanNavigateWithinDistance(WoWPoint start, WoWPoint destination, float maxPathDistance)
		{
			if (!IsNavigatorLoaded)
				return true;

			uint mapId = (uint)(GetCurrentMapId());
			var startVec = new Vector3(start.X, start.Y, start.Z);
			var endVec = new Vector3(destination.X, destination.Y, destination.Z);

			try
			{
				TripperNavigator.EnsureTilesAroundPosition(mapId, startVec, LoadTilesAroundRadius);
				TripperNavigator.EnsureTilesAroundPosition(mapId, endVec, LoadTilesAroundRadius);
			}
			catch { }

			BlackspotManager.EnsureBlackspotsMarked();

			var result = TripperNavigator.FindPath(mapId, startVec, endVec, true);
			if (!result.Status.Succeeded || result.IsPartialPath)
				return false;

			float totalLength = 0f;
			Vector3[] pts = result.Points;
			if (pts != null && pts.Length > 1)
			{
				for (int i = 1; i < pts.Length; i++)
					totalLength += Vector3.Distance(pts[i - 1], pts[i]);
			}
			return totalLength <= maxPathDistance;
		}

		public static WoWPoint[] GeneratePath(WoWPoint start, WoWPoint destination)
		{
			if (!IsNavigatorLoaded)
				return Array.Empty<WoWPoint>();

			uint mapId = (uint)(GetCurrentMapId());
			var startVec = new Vector3(start.X, start.Y, start.Z);
			var endVec = new Vector3(destination.X, destination.Y, destination.Z);

			try
			{
				TripperNavigator.EnsureTilesAroundPosition(mapId, startVec, LoadTilesAroundRadius);
				TripperNavigator.EnsureTilesAroundPosition(mapId, endVec, LoadTilesAroundRadius);
			}
			catch { }

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

			return Array.Empty<WoWPoint>();
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
		/// HB 6.2.3 Navigator.FindHeights — delegates to HeightProvider.FindHeights.
		/// </summary>
		public static List<float> FindHeights(float x, float y)
		{
			if (_heightProvider == null)
				throw new InvalidOperationException("HeightProvider cannot be null");
			return _heightProvider.FindHeights(x, y);
		}

		/// <summary>
		/// HB 6.2.3 Navigator.FindHeight(ref Vector3) — modifies Z in place.
		/// </summary>
		public static bool FindHeight(ref Tripper.XNAMath.Vector3 v)
		{
			return FindHeight(v.X, v.Y, out v.Z);
		}

		/// <summary>
		/// HB 6.2.3 Navigator.FindHeight(x,y,out z) — takes first height from FindHeights.
		/// </summary>
		public static bool FindHeight(float x, float y, out float z)
		{
			List<float> list = FindHeights(x, y);
			if (list.Count == 0)
			{
				z = 0f;
				return false;
			}
			z = list[0];
			return true;
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
		/// HB 6.2.3 MeshNavigator.DistToWall — finds the nearest navmesh wall/boundary distance.
		/// Used by GetBestLocationOutsideCluster to reject candidates that are too close to walls
		/// (distance &lt; 1 yard). A candidate right next to a wall means the character would
		/// immediately re-enter the avoid zone the next time it bumps into geometry.
		/// Thin wrapper over TripperNavigator.FindDistanceToWall → Navigation.dll FindDistanceToWall.
		/// </summary>
		/// <param name="position">Point to measure from.</param>
		/// <param name="maxRadius">Maximum search radius (use 5f to match HB 6.2.3 Helpers.smethod_4).</param>
		/// <returns>Distance to the nearest wall, or 0f if the navigator is not loaded.</returns>
		public static float FindDistanceToWall(WoWPoint position, float maxRadius = 5f)
		{
			if (!IsNavigatorLoaded)
				return 0f;

			uint mapId = GetCurrentMapId();
			var posVec = new Vector3(position.X, position.Y, position.Z);
			try
			{
				return TripperNavigator.FindDistanceToWall(mapId, posVec, maxRadius, out _);
			}
			catch
			{
				return 0f;
			}
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
		/// Direct port of HB 6.2.3 Navigator.GetRunStatusFromMoveResult.
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
		/// HB 6.2.3 Navigator.GetNavigationProviderAs — convenience cast helper.
		/// </summary>
		[Obsolete("Use C# casts with NavigationProvider instead")]
		public static T? GetNavigationProviderAs<T>() where T : NavigationProvider
		{
			return _currentProvider as T;
		}
	}
}
