using System;
using System.Collections.Generic;
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
		}

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

			// If destination changed, generate new path
			if (_destination != destination)
			{
				_destination = destination;
				_currentPath.Clear();

				// New destination implies a new movement attempt; reset stuck state.
				try
				{
					StuckHandler.Reset();
				}
				catch
				{
					// Ignore
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
						Logging.WriteDebug("[Navigator] Path generated with {0} points to: {1} ({2})", 
							_currentPath.Count, destinationName, destination);
					}
					else
					{
						// Fallback to direct movement
						_currentPath.Add(me.Location);
						_currentPath.Add(destination);
						Logging.WriteDebug("[Navigator] Pathfinding failed, using direct movement to: {0} ({1})", 
							destinationName, destination);
					}
				}
				else
				{
					// No navmesh, use direct path
					_currentPath.Add(me.Location);
					_currentPath.Add(destination);
					Logging.WriteDebug("[Navigator] Moving directly to: {0} ({1})", destinationName, destination);
				}

				_currentPathIndex = 0;
			}

			// Move along path
			// HB 4.3.4: Check both 2D distance and Z difference, push waypoint ahead
			if (_currentPath.Count > 0 && _currentPathIndex < _currentPath.Count)
			{
				WoWPoint nextPoint = _currentPath[_currentPathIndex];
				
				// For intermediate waypoints, use PathPrecision (2.0)
				// For final waypoint, use requested precision
				bool isFinalPoint = (_currentPathIndex == _currentPath.Count - 1);
				float waypointPrecision = isFinalPoint ? precision : PathPrecision;

				// HB: Check 2D distance AND Z difference (< 4.5 yards)
				float distance2DSqr = me.Location.Distance2DSqr(nextPoint);
				float zDiff = Math.Abs(me.Location.Z - nextPoint.Z);
				
				if (distance2DSqr < waypointPrecision * waypointPrecision && zDiff < 4.5f)
				{
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

				// HB: Push waypoint ahead by PathPrecision in movement direction (for flat terrain)
				WoWPoint direction = nextPoint - me.Location;
				float length = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z);
				if (length > 0.01f)
				{
					direction = new WoWPoint(direction.X / length, direction.Y / length, direction.Z / length);
					clickPoint = nextPoint + direction * PathPrecision;
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
	}
}
