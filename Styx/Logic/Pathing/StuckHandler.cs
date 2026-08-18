using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.WoWInternals.World;

namespace Styx.Logic.Pathing
{
    /// <summary>
    /// Handles stuck detection and recovery for WoW 3.3.5a.
    /// Stuck detection: HB 6.2.3 Class469 — 500ms Stopwatch + Navigator.PathDistance + movement-stop reset.
    /// Unstick sequence: Dismount → Jump(1x, LOS-gated) → StrafeFwdL → StrafeFwdR → Dismount2 → StrafeL → StrafeR → Blackspot+Reverse
    /// </summary>
    internal class DefaultStuckHandler : StuckHandler
    {
        private const float UnstickResetDistanceSqr = 100f;
        private const float DismountRaycastDistance = 3f;
        private const float JumpRaycastDistance = 2f;

        private readonly WaitTimer _mountUpBlockTimer = new WaitTimer(TimeSpan.FromSeconds(10.0));
        // HB 6.2.3 Class469: Stopwatch replaces the 2-second WaitTimer.
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private WoWPoint _lastCheckLocation = WoWPoint.Empty;
        private WoWPoint _lastUnstickLocation = WoWPoint.Empty;
        private long _unstickAttemptCount = 1;

        private bool _triedDismount;
        private bool _triedJump;
        private bool _triedStrafeForwardLeft;
        private bool _triedStrafeForwardRight;
        private bool _triedStrafeLeft;
        private bool _triedStrafeRight;
        private bool _isCurrent;

        public DefaultStuckHandler()
        {
            _lastCheckLocation = WoWPoint.Empty;
            _stopwatch.Start();
        }

        public override void OnSetAsCurrent()
        {
            if (_isCurrent)
                return;

            Mount.OnMountUp += OnMountUp;
            WoWMovement.OnMovementFlagsChanged += OnMovementFlagsChanged;
            _isCurrent = true;
        }

        public override void OnRemoveAsCurrent()
        {
            if (!_isCurrent)
                return;

            WoWMovement.OnMovementFlagsChanged -= OnMovementFlagsChanged;
            Mount.OnMountUp -= OnMountUp;
            _isCurrent = false;
        }

        private void OnMountUp(object? sender, MountUpEventArgs e)
        {
            e.Cancel = !_mountUpBlockTimer.IsFinished;
        }

        private void OnMovementFlagsChanged(WoWMovement.MovementEventArgs e)
        {
            // HB 6.2.3 Class469.method_1: restart the stopwatch and clear the reference
            // location whenever the player stops moving. This prevents false stuck detections
            // between waypoints when the bot intentionally stops (e.g. targeting, looting).
            if (e.Stop)
            {
                _stopwatch.Restart();
                _lastCheckLocation = WoWPoint.Empty;
            }
        }

        public override bool IsStuck()
        {
            var me = ObjectManager.Me;
            if (me == null)
                return false;

            if (me.Stunned || me.Fleeing || me.Dazed || me.Rooted)
            {
                Reset();
                return false;
            }

            WoWUnit? activeMover = WoWMovement.ActiveMover;
            if (activeMover == null || !activeMover.IsMe)
                return false;

            // HB 6.2.3 Class469.IsStuck(): 500ms minimum interval.
            if (_stopwatch.ElapsedMilliseconds < 500L)
                return false;

            WoWPoint currentLocation = activeMover.Location;
            if (_lastCheckLocation != WoWPoint.Empty)
            {
                float expectedDist = GetExpectedDistance(activeMover, _stopwatch.Elapsed);
                float? pathDist = Navigator.PathDistance(_lastCheckLocation, currentLocation, expectedDist);
                // HB 6.2.3 Class469: only evaluate stuck when PathDistance is available.
                // Null typically means partial/off-mesh path; treating that as zero causes
                // false stuck detections during normal looting/micro-adjust movement.
                if (pathDist.HasValue && pathDist.Value < expectedDist)
                {
                    Logging.WriteVerbose(Colors.Red,
                        "We are stuck! (TPS: {0:F1}, Latency: {1}, loc: {2})!",
                        GameStats.TicksPerSecond,
                        StyxWoW.WoWClient.Latency,
                        currentLocation);
                    LogNearestGameObject();
                    _lastCheckLocation = currentLocation;
                    return true;
                }
            }

            _stopwatch.Restart();
            _lastCheckLocation = currentLocation;
            return false;
        }

        // HB 6.2.3 Class469.method_3: expected travel distance over elapsed time.
        // WotLK: MovementInfo has SwimSpeed (offset 0xA4) and RunSpeed (offset 0x94).
        // IsSwimming is a flag on WoWUnit (movement flag 0x200000 at offset 0xA30).
        private static float GetExpectedDistance(WoWUnit unit, TimeSpan elapsed)
        {
            var mi = unit.MovementInfo;
            float speed = mi.CurrentSpeed;
            return speed * (float)elapsed.TotalSeconds * 0.6f;
        }

        public override void Unstick()
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
                    Logging.WriteVerbose("Trying dismount");
                    Mount.Dismount("Stuck Handler");
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
                    Logging.WriteVerbose("Trying jump");
                    WoWMovement.Move(WoWMovement.MovementDirection.Forward | WoWMovement.MovementDirection.JumpAscend);
                    StyxWoW.Sleep(100);
                    WoWMovement.MoveStop(WoWMovement.MovementDirection.Forward | WoWMovement.MovementDirection.JumpAscend);
                    StyxWoW.Sleep(200);
                }
                _triedJump = true;
            }
            else if (!_triedStrafeForwardLeft)
            {
                Logging.WriteVerbose("Trying strafe forward left for {0}ms", duration);
                MoveInDirection(WoWMovement.MovementDirection.Forward | WoWMovement.MovementDirection.StrafeLeft, duration);
                _triedStrafeForwardLeft = true;
            }
            else if (!_triedStrafeForwardRight)
            {
                Logging.WriteVerbose("Trying strafe forward right for {0}ms", duration);
                MoveInDirection(WoWMovement.MovementDirection.Forward | WoWMovement.MovementDirection.StrafeRight, duration);
                _triedStrafeForwardRight = true;
            }
            else if (me.Mounted)
            {
                Logging.WriteVerbose("Trying dismount");
                Mount.Dismount("Stuck Handler");
            }
            else if (!_triedStrafeLeft)
            {
                Logging.WriteVerbose("Trying strafe left for {0}ms", duration);
                MoveInDirection(WoWMovement.MovementDirection.StrafeLeft, duration);
                _triedStrafeLeft = true;
            }
            else if (!_triedStrafeRight)
            {
                Logging.WriteVerbose("Trying strafe right for {0}ms", duration);
                MoveInDirection(WoWMovement.MovementDirection.StrafeRight, duration);
                _triedStrafeRight = true;
            }
            else
            {
                AddBlackspotAndReverse(duration * 2);
                ResetUnstickAttempts();
            }

            _stopwatch.Restart();
        }

        public override void Reset()
        {
            _stopwatch.Restart();
            _lastCheckLocation = WoWPoint.Empty;
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
            StyxWoW.Sleep(milliseconds);
            WoWMovement.MoveStop(direction);
            StyxWoW.Sleep(200);
        }

        private void AddBlackspotAndReverse(int milliseconds)
        {
            var me = ObjectManager.Me;
            if (me == null)
                return;

            Logging.WriteVerbose("Adding blackspot at current location and backing off for {0}ms", milliseconds);
            BlackspotManager.AddBlackspot(me.Location, 5f, 3f, "StuckHandler");
            WoWMovement.MoveStop();
            StyxWoW.Sleep(100);
            WoWMovement.Move(WoWMovement.MovementDirection.Backwards);
            StyxWoW.Sleep(milliseconds);
            WoWMovement.MoveStop();
            StyxWoW.Sleep(200);

            Logging.WriteDebug("[STUCK] Clearing path to regenerate around blackspot.");
            Navigator.Clear();
        }

        private void LogNearestGameObject()
        {
            WoWGameObject? nearestGameObject = ObjectManager.GetObjectsOfType<WoWGameObject>()
                .Where(gameObject => gameObject.DistanceSqr < 2500.0)
                .OrderBy(gameObject => gameObject.DistanceSqr)
                .FirstOrDefault();

            if (nearestGameObject == null)
                return;

            Logging.WriteDiagnostic("Nearest game object, distance {0:F2} yards, entry {1}, type {2}, name {3}",
                nearestGameObject.Distance,
                nearestGameObject.Entry,
                nearestGameObject.SubType,
                nearestGameObject.Name);
        }



    }
}
