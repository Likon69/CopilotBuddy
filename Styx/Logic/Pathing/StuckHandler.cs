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
    /// Direct port of HB 6.2.3 Class469 (MeshNavigator stuck handler).
    /// Sequence: Dismount → Jump(1x, LOS-gated) → StrafeFwdL → StrafeFwdR → Dismount2 → StrafeL → StrafeR → Blackspot+Reverse
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
            _lastCheckLocation = WoWPoint.Empty;
            _movementStopwatch.Restart();
        }

        public bool IsStuck()
        {
            var me = ObjectManager.Me;
            if (me == null)
                return false;

            if (me.Stunned || me.Fleeing || me.Dazed || me.Rooted || me.IsFalling)
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
                float? pathDistance = Navigator.PathDistance(_lastCheckLocation, currentLocation, expectedDistance);
                if (pathDistance != null && pathDistance < expectedDistance)
                {
                    Logging.WriteDebug("[STUCK] Movement stalled — path distance {0:F1}yd in {1:F1}s (expected {2:F1}yd).",
                        pathDistance.Value, (float)_movementStopwatch.Elapsed.TotalSeconds, expectedDistance);
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

            // HB pattern: Dismount → Jump → Strafe sides → Blackspot
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
                // HB 6.2.3 Class469: ONE jump attempt, only if forward path is clear (LOS check).
                // If blocked ahead, jumping won't help — skip straight to strafe.
                WoWPoint fwd = location.RayCast(rotation, JumpRaycastDistance).Add(0f, 0f, 2f);
                WoWPoint src = location.Add(0f, 0f, 2f);

                if (GameWorld.IsInLineOfSight(src, fwd))
                {
                    Logging.WriteDebug("[STUCK] Jump attempt — forward path clear, jumping over obstacle.");
                    WoWMovement.Move(WoWMovement.MovementDirection.Forward | WoWMovement.MovementDirection.JumpAscend);
                    StyxWoW.Sleep(100);
                    WoWMovement.MoveStop(WoWMovement.MovementDirection.Forward | WoWMovement.MovementDirection.JumpAscend);
                    StyxWoW.Sleep(200);
                }
                else
                {
                    Logging.WriteDebug("[STUCK] Jump skipped — forward path blocked, will try strafe.");
                }
                _triedJump = true;
            }
            else if (!_triedStrafeForwardLeft)
            {
                Logging.WriteDebug("[STUCK] Trying strafe forward-left for {0}ms.", duration);
                MoveInDirection(WoWMovement.MovementDirection.Forward | WoWMovement.MovementDirection.StrafeLeft, duration);
                _triedStrafeForwardLeft = true;
            }
            else if (!_triedStrafeForwardRight)
            {
                Logging.WriteDebug("[STUCK] Trying strafe forward-right for {0}ms.", duration);
                MoveInDirection(WoWMovement.MovementDirection.Forward | WoWMovement.MovementDirection.StrafeRight, duration);
                _triedStrafeForwardRight = true;
            }
            else if (me.Mounted)
            {
                Logging.WriteDebug("[STUCK] Still mounted after strafe attempts — dismounting.");
                Mount.Dismount("[STUCK] Dismounting to navigate obstacle");
            }
            else if (!_triedStrafeLeft)
            {
                Logging.WriteDebug("[STUCK] Trying strafe left for {0}ms.", duration);
                MoveInDirection(WoWMovement.MovementDirection.StrafeLeft, duration);
                _triedStrafeLeft = true;
            }
            else if (!_triedStrafeRight)
            {
                Logging.WriteDebug("[STUCK] Trying strafe right for {0}ms.", duration);
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

        /// <summary>
        /// HB 6.2.3 pattern: Move → Sleep(duration) → MoveStop → Sleep(200).
        /// </summary>
        private void MoveInDirection(WoWMovement.MovementDirection direction, int milliseconds)
        {
            WoWMovement.Move(direction);
            StyxWoW.Sleep(milliseconds);
            WoWMovement.MoveStop(direction);
            StyxWoW.Sleep(200);
        }

        private void AddBlackspotAndReverse(int milliseconds)
        {
            var me = ObjectManager.Me;
            if (me == null)
                return;

            Logging.WriteDebug("[STUCK] All attempts failed — adding blackspot and reversing.");
            BlackspotManager.AddBlackspot(me.Location, 5f, 3f);
            WoWMovement.MoveStop();
            StyxWoW.Sleep(100);
            WoWMovement.Move(WoWMovement.MovementDirection.Backwards);
            StyxWoW.Sleep(milliseconds);
            WoWMovement.MoveStop();
            StyxWoW.Sleep(200);

            Logging.WriteDebug("[STUCK] Clearing path to regenerate around blackspot.");
            Navigator.Clear();
        }

        private float GetExpectedTravelDistance(Styx.WoWInternals.WoWObjects.LocalPlayer me, TimeSpan timeSpan)
        {
            float speed;
            if (me.MovementInfo.IsSwimming)
                speed = me.MovementInfo.SwimSpeed;
            else if (me.MovementInfo.IsFlying)
                speed = me.MovementInfo.FlySpeed;
            else
                speed = me.MovementInfo.RunSpeed;

            return speed * (float)timeSpan.TotalSeconds;
        }


    }
}
