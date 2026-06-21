using Styx;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using TreeSharp;
using Action = TreeSharp.Action;

namespace ObjectGatherer.Helpers {
    public class FollowPathAction : Action {
        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        // ===========================================================
        // Constructors
        // ===========================================================

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        protected override RunStatus Run(object context) {
            if(ProfileHandler.CurrentPoint == WoWPoint.Zero)
                return RunStatus.Failure;
            if((ProfileHandler.WayPoints.Count == 1) && StyxWoW.Me.Location.Distance(ProfileHandler.CurrentPoint) < 5f) {
                return RunStatus.Failure;
            }

            var precision = StyxWoW.Me.IsFlying ? 15f : 3f;
            if(StyxWoW.Me.Location.Distance(ProfileHandler.CurrentPoint) <= precision) {
                ProfileHandler.CycleToNextPoint();
            }

            if(StyxWoW.Me.IsSwimming) {
                if(StyxWoW.Me.GetMirrorTimerInfo(MirrorTimerType.Breath).CurrentTime > 0) {
                    WoWMovement.Move(WoWMovement.MovementDirection.JumpAscend);
                } else if(StyxWoW.Me.MovementInfo.IsAscending || StyxWoW.Me.MovementInfo.JumpingOrShortFalling) {
                    WoWMovement.MoveStop(WoWMovement.MovementDirection.JumpAscend);
                }
            }

            if(!StyxWoW.Me.Mounted && Flightor.MountHelper.CanMount) {
                Flightor.MountHelper.MountUp();
            } else {
                Flightor.MoveTo(ProfileHandler.CurrentPoint, 15f);
            }

            return RunStatus.Success;
        }

        // ===========================================================
        // Methods
        // ===========================================================

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================


    }
}
