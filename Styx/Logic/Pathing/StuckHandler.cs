using System;
using System.Diagnostics;
using System.Threading;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.World;

namespace Styx.Logic.Pathing
{
    /// <summary>
    /// Handles stuck detection and recovery for WoW 3.3.5a.
    /// Based on HB 4.3.4 Class471 implementation.
    /// </summary>
    internal class StuckHandler : IStuckHandler
    {
        private const float UnstickResetDistanceSqr = 100f;
        private const float DismountRaycastDistance = 3f;
        private const float JumpRaycastDistance = 2f;
        private const float ExpectedDistanceScale = 0.6f;

        private readonly WaitTimer _mountUpBlockTimer = new WaitTimer(TimeSpan.FromSeconds(10.0));
        private readonly Stopwatch _movementStopwatch = new Stopwatch();

        private WoWPoint _lastCheckLocation = WoWPoint.Empty;
        private WoWPoint _lastUnstickLocation = WoWPoint.Empty;
        private long _unstickAttemptCount = 1;

        private bool _triedDismount;
        private bool _triedJump;
        private bool _triedStrafeForwardLeft;
        private bool _triedStrafeForwardRight;
        private bool _triedStrafeLeft;
        private bool _triedStrafeRight;

        public StuckHandler()
        {
            _movementStopwatch.Restart();
        }

        public bool IsStuck()
        {
            var me = ObjectManager.Me;
            if (me == null)
                return false;

            if (me.Stunned || me.Fleeing || me.Dazed || me.Rooted)
            {
                Reset();
                return false;
            }

            // Don't check IsMoving here - when stuck, IsMoving stays true but player doesn't move
            // WoD uses OnMovementFlagsChanged event to reset, but we don't have that in WotLK
            // So we let the stopwatch run and rely on PathDistance check below

            if (_movementStopwatch.ElapsedMilliseconds < 500L)
                return false;

            WoWPoint currentLocation = me.Location;
            if (_lastCheckLocation != WoWPoint.Empty)
            {
                float expectedDistance = GetExpectedTravelDistance(me, _movementStopwatch.Elapsed) * ExpectedDistanceScale;
                float? pathDistance = TryGetPathDistance(_lastCheckLocation, currentLocation);

                if (pathDistance.HasValue && pathDistance.Value < expectedDistance)
                {
                    Logging.WriteDebug("[STUCK] Movement stalled.");
                    _movementStopwatch.Restart();
                    _lastCheckLocation = currentLocation;
                    return true;
                }
            }

            _movementStopwatch.Restart();
            _lastCheckLocation = currentLocation;
            return false;
        }

        public void Unstick()
        {
            var me = ObjectManager.Me;
            if (me == null)
                return;

            _mountUpBlockTimer.Reset();

            WoWPoint location = me.Location;
            if (_lastUnstickLocation.DistanceSqr(location) >= UnstickResetDistanceSqr)
            {
                ResetUnstickAttempts();
            }
            _lastUnstickLocation = location;

            int duration = (_unstickAttemptCount % 2L == 0L) ? 1000 : 600;
            _unstickAttemptCount++;

            float rotation = me.Rotation;

            if (!_triedDismount && me.Mounted)
            {
                WoWPoint forward = location.RayCast(rotation, DismountRaycastDistance).Add(0f, 0f, 1f);
                WoWPoint upper = location.Add(0f, 0f, me.BoundingHeight + 2f);
                WoWPoint middle = location.Add(0f, 0f, me.BoundingHeight / 2f);

                if (!GameWorld.IsInLineOfSight(upper, forward) || !GameWorld.IsInLineOfSight(middle, forward))
                {
                    Logging.WriteDebug("[STUCK] Trying dismount.");
                    Mount.Dismount("[STUCK] Dismounting to navigate obstacle");
                }
                _triedDismount = true;
            }
            else if (!_triedJump)
            {
                WoWPoint forward = location.RayCast(rotation, JumpRaycastDistance).Add(0f, 0f, 2f);
                WoWPoint start = location.Add(0f, 0f, 2f);
                if (GameWorld.IsInLineOfSight(start, forward))
                {
                    Logging.WriteDebug("[STUCK] Trying jump.");
                    MoveInDirection(WoWMovement.MovementDirection.Forward | WoWMovement.MovementDirection.JumpAscend, 100);
                    Thread.Sleep(200);
                }
                _triedJump = true;
            }
            else if (!_triedStrafeForwardLeft)
            {
                Logging.WriteDebug("[STUCK] Trying strafe forward left for {0}ms", duration);
                MoveInDirection(WoWMovement.MovementDirection.Forward | WoWMovement.MovementDirection.StrafeLeft, duration);
                _triedStrafeForwardLeft = true;
            }
            else if (!_triedStrafeForwardRight)
            {
                Logging.WriteDebug("[STUCK] Trying strafe forward right for {0}ms", duration);
                MoveInDirection(WoWMovement.MovementDirection.Forward | WoWMovement.MovementDirection.StrafeRight, duration);
                _triedStrafeForwardRight = true;
            }
            else if (me.Mounted)
            {
                Logging.WriteDebug("[STUCK] Trying dismount.");
                Mount.Dismount("[STUCK] Dismounting to navigate obstacle");
            }
            else if (!_triedStrafeLeft)
            {
                Logging.WriteDebug("[STUCK] Trying strafe left for {0}ms", duration);
                MoveInDirection(WoWMovement.MovementDirection.StrafeLeft, duration);
                _triedStrafeLeft = true;
            }
            else if (!_triedStrafeRight)
            {
                Logging.WriteDebug("[STUCK] Trying strafe right for {0}ms", duration);
                MoveInDirection(WoWMovement.MovementDirection.StrafeRight, duration);
                _triedStrafeRight = true;
            }
            else
            {
                AddBlackspotAndReverse(duration * 2);
                ResetUnstickAttempts();
            }

            _movementStopwatch.Restart();
        }

        public void Reset()
        {
            _movementStopwatch.Restart();
            _lastCheckLocation = WoWPoint.Empty;
            _lastUnstickLocation = WoWPoint.Empty;
            ResetUnstickAttempts();
        }

        private void ResetUnstickAttempts()
        {
            _triedDismount = false;
            _triedJump = false;
            _triedStrafeForwardLeft = false;
            _triedStrafeForwardRight = false;
            _triedStrafeLeft = false;
            _triedStrafeRight = false;
        }

        private void MoveInDirection(WoWMovement.MovementDirection direction, int milliseconds)
        {
            WoWMovement.Move(direction);
            RefreshClickToMove();
            Thread.Sleep(milliseconds);
            WoWMovement.MoveStop(direction);
            Thread.Sleep(100);
        }

        private void AddBlackspotAndReverse(int milliseconds)
        {
            var me = ObjectManager.Me;
            if (me == null)
                return;

            Logging.WriteDebug("[STUCK] Adding blackspot.");
            // TODO: BlackspotManager when implemented

            Logging.WriteDebug("[STUCK] Moving backwards for {0}ms", milliseconds);
            WoWMovement.MoveStop();
            Thread.Sleep(100);
            WoWMovement.Move(WoWMovement.MovementDirection.Backwards);
            Thread.Sleep(milliseconds);
            WoWMovement.MoveStop();
            Thread.Sleep(200);

            Logging.WriteDebug("[STUCK] Clearing current path to regenerate around stuck location.");
            Navigator.Clear();
        }

        private void RefreshClickToMove()
        {
            try
            {
                var currentPath = Navigator.CurrentPath;
                var me = ObjectManager.Me;
                if (currentPath != null && currentPath.Count > 0 && me != null)
                {
                    int currentIndex = 0;
                    for (int i = 0; i < currentPath.Count; i++)
                    {
                        if (me.Location.Distance(currentPath[i]) < Navigator.PathPrecision)
                        {
                            currentIndex = Math.Min(i + 1, currentPath.Count - 1);
                            break;
                        }
                    }
                    
                    if (currentIndex < currentPath.Count)
                    {
                        WoWMovement.ClickToMove(currentPath[currentIndex]);
                    }
                }
            }
            catch
            {
                // Ignore
            }
        }

        private float GetExpectedTravelDistance(Styx.WoWInternals.WoWObjects.LocalPlayer me, TimeSpan timeSpan)
        {
            if (me.MovementInfo.IsSwimming)
                return me.MovementInfo.SwimSpeed * (float)timeSpan.TotalSeconds;

            return me.MovementInfo.RunSpeed * (float)timeSpan.TotalSeconds;
        }

        private float? TryGetPathDistance(WoWPoint from, WoWPoint to)
        {
            var path = Navigator.CurrentPath;
            if (path == null || path.Count == 0)
                return from.Distance(to);

            int fromIndex = FindClosestPathIndex(from, path);
            int toIndex = FindClosestPathIndex(to, path);
            if (fromIndex < 0 || toIndex < 0)
                return from.Distance(to);

            float distance = from.Distance(path[fromIndex]) + to.Distance(path[toIndex]);

            if (fromIndex <= toIndex)
            {
                for (int i = fromIndex; i < toIndex; i++)
                {
                    distance += path[i].Distance(path[i + 1]);
                }
            }
            else
            {
                for (int i = fromIndex; i > toIndex; i--)
                {
                    distance += path[i].Distance(path[i - 1]);
                }
            }

            return distance;
        }

        private int FindClosestPathIndex(WoWPoint point, System.Collections.Generic.IList<WoWPoint> path)
        {
            int closestIndex = -1;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < path.Count; i++)
            {
                float distance = point.Distance(path[i]);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }
    }
}
