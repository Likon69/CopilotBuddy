using System;
using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;

namespace ObjectGatherer.Helpers {
    class ProfileHandler {
        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        private static int _currentIndex;
        public static List<WoWPoint> WayPoints = new List<WoWPoint>();

        // ===========================================================
        // Constructors
        // ===========================================================

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        public static WoWPoint CurrentPoint {
            get {
                return WayPoints != null && WayPoints.Count > 0 ? WayPoints[_currentIndex] : WoWPoint.Zero;
            }
        }

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        public static void OnNewProfileLoaded(BotEvents.Profile.NewProfileLoadedEventArgs args) {
            try {
                LoadWayPoints(ProfileManager.CurrentProfile);
            } catch(Exception ex) {
                CustomLog.Diagnostic(ex.ToString());
            }
        }

        public static void OnNewOuterProfileLoaded(BotEvents.Profile.NewProfileLoadedEventArgs args) {
            try {
                LoadWayPoints(ProfileManager.CurrentProfile);
            } catch(Exception ex) {
                CustomLog.Diagnostic(ex.ToString());
            }
        }

        private static void LoadWayPoints(Profile profile) {
            WayPoints.Clear();
            if(profile == null || profile.GrindArea == null) {
                return;
            }

            if(profile.GrindArea.Hotspots != null) {
                WayPoints = profile.GrindArea.Hotspots.ConvertAll(hs => hs.Position);
                var closestPoint = WayPoints.OrderBy(u => u.Distance(StyxWoW.Me.Location)).FirstOrDefault();
                _currentIndex = WayPoints.FindIndex(w => w == closestPoint);
            } else {
                WayPoints = new List<WoWPoint>();
            }
        }

        public static void CycleToNextPoint() {
            if(WayPoints == null) {
                return;
            }

            if(_currentIndex >= WayPoints.Count - 1) {
                _currentIndex = 0;
            } else {
                _currentIndex++;
            }
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================
    }
}
