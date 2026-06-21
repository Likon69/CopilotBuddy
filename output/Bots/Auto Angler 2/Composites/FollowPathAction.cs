using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = TreeSharp.Action;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx;
using Styx.WoWInternals;
using Styx.Logic.Profiles;
using TreeSharp;
using Styx.Logic.POI;
using Styx.Logic;
using Styx.Logic.Inventory.Frames.LootFrame;
using Styx.WoWInternals.WoWObjects;

namespace HighVoltz.Composites
{
    public class FollowPathAction : Action
    {
        LocalPlayer _me = ObjectManager.Me;
        AutoAnglerSettings _settings = AutoAngler.Instance.MySettings;
        public static List<WoWPoint> WayPoints = new List<WoWPoint>();
        static int _currentIndex = 0;
        public static WoWPoint CurrentPoint
        {
            get
            {
                return WayPoints != null && WayPoints.Count > 0 ?
                    WayPoints[_currentIndex] : WoWPoint.Zero;
            }
        }
        public FollowPathAction()
        {
            BotEvents.Profile.OnNewOuterProfileLoaded += Profile_OnNewOuterProfileLoaded;
            Styx.Logic.Profiles.Profile.OnUnknownProfileElement += Profile_OnUnknownProfileElement;
            if (ProfileManager.CurrentProfile != null)
                LoadWayPoints(ProfileManager.CurrentProfile);
        }

        void WayPoints_OnStartOfQueue(object sender, EventArgs e)
        {
            if (AutoAngler.Instance.MySettings.PathingType == PathingType.Bounce)
            {
                WayPoints.Reverse();
            }
        }

        void WayPoints_OnEndOfQueue(object sender, EventArgs e)
        {
            if (AutoAngler.Instance.MySettings.PathingType == PathingType.Bounce)
            {
                WayPoints.Reverse();
            }
        }

        ~FollowPathAction()
        {
            BotEvents.Profile.OnNewOuterProfileLoaded -= Profile_OnNewOuterProfileLoaded;
            Styx.Logic.Profiles.Profile.OnUnknownProfileElement -= Profile_OnUnknownProfileElement;
        }

        void Profile_OnNewOuterProfileLoaded(BotEvents.Profile.NewProfileLoadedEventArgs args)
        {
            try
            {
                LoadWayPoints(ProfileManager.CurrentProfile);
            }
            catch (Exception ex) { Logging.WriteException(ex); }
        }

        int _lastUkTagCallTime = 0;
        void Profile_OnUnknownProfileElement(object sender, UnknownProfileElementEventArgs e)
        {
            if (e.Element.Name == "FishingSchool")
            {
                // hackish way to clear my list of pool before loading new profile... wtb OnNewOuterProfileLoading event
                if (Environment.TickCount - _lastUkTagCallTime > 4000)
                    AutoAngler.PoolsToFish.Clear();
                _lastUkTagCallTime = Environment.TickCount;
                var entryAttrib = e.Element.Attribute("Entry");
                if (entryAttrib != null)
                {
                    uint entry;
                    uint.TryParse(entryAttrib.Value, out entry);
                    if (!AutoAngler.PoolsToFish.Contains(entry))
                    {
                        AutoAngler.PoolsToFish.Add(entry);
                        var nameAttrib = e.Element.Attribute("Name");
                        if (nameAttrib != null)
                            AutoAngler.Instance.Log("Adding Pool Entry: {0} to the list of pools to fish from", nameAttrib.Value);
                        else
                            AutoAngler.Instance.Log("Adding Pool Entry: {0} to the list of pools to fish from", entry);
                    }
                }
                else
                {
                    AutoAngler.Instance.Err("<FishingSchool> tag must have the 'Entry' Attribute, e.g <FishingSchool Entry=\"202780\"/>\nAlso supports 'Name' attribute but only used for display purposes");
                }
                e.Handled = true;
            }
            else if (e.Element.Name == "Pathing")
            {
                var typeAttrib = e.Element.Attribute("Type");
                if (typeAttrib != null)
                {
                    AutoAngler.Instance.MySettings.PathingType = (PathingType)
                        Enum.Parse(typeof(PathingType), typeAttrib.Value, true);
                    AutoAngler.Instance.Log("Setting Pathing Type to {0} Mode", 
                        AutoAngler.Instance.MySettings.PathingType);
                }
                else
                {
                    AutoAngler.Instance.Err("<Pathing> tag must have the 'Type' Attribute, e.g <Pathing Type=\"Circle\"/>");
                }
                e.Handled = true;
            }
        }

        void LoadWayPoints(Profile profile)
        {
            WayPoints.Clear();
            if (profile != null && profile.GrindArea != null)
            {
                if (profile.GrindArea.Hotspots != null)
                {
                    WayPoints = profile.GrindArea.Hotspots.ConvertAll(hs => hs.Position);
                    WoWPoint closestPoint = WayPoints.OrderBy(u => u.Distance(ObjectManager.Me.Location)).FirstOrDefault();
                    _currentIndex = WayPoints.FindIndex(w => w == closestPoint);
                }
                else
                    WayPoints = new List<WoWPoint>();
            }
        }

        protected override RunStatus Run(object context)
        {
            if (LootAction.GetLoot())
                return RunStatus.Success;
            //  dks can refresh water walking while flying around.
            if (AutoAngler.Instance.MySettings.UseWaterWalking &&
                ObjectManager.Me.Class == Styx.Combat.CombatRoutine.WoWClass.DeathKnight && !WaterWalking.IsActive)
            {
                WaterWalking.Cast();
            }
            if (CurrentPoint == WoWPoint.Zero)
                return RunStatus.Failure;
            float speed = ObjectManager.Me.MovementInfo.CurrentSpeed;
            float modifier = _settings.Fly ? 4f : 2f;
            float precision = speed > 7 ? (modifier * speed) / 7f : modifier;
            if (ObjectManager.Me.Location.Distance(CurrentPoint) <= precision)
                CycleToNextPoint();
            if (_settings.Fly)
            {
                if (_me.IsSwimming)
                {
                    if (_me.GetMirrorTimerInfo(MirrorTimerType.Breath).CurrentTime > 0)
                        WoWMovement.Move(WoWMovement.MovementDirection.JumpAscend);
                    else if (_me.MovementInfo.IsAscending || _me.MovementInfo.JumpingOrShortFalling)
                        WoWMovement.MoveStop(WoWMovement.MovementDirection.JumpAscend);
                }
                if (!ObjectManager.Me.Mounted)
                    Flightor.MountHelper.MountUp();
                Flightor.MoveTo(CurrentPoint);
            }
            else
            {
                if (!ObjectManager.Me.Mounted && Mount.ShouldMount(CurrentPoint) && Mount.CanMount())
                    Mount.MountUp();
                Navigator.MoveTo(CurrentPoint);
            }
            return RunStatus.Success;
        }
        public static void CycleToNextPoint()
        {
            if (WayPoints != null)
            {
                if (_currentIndex >= WayPoints.Count - 1)
                {
                    if (AutoAngler.Instance.MySettings.PathingType == PathingType.Bounce)
                    {
                        WayPoints.Reverse();
                        _currentIndex = 1;
                    }
                    else
                        _currentIndex = 0;
                }
                else
                    _currentIndex++;
            }
        }

        static WoWPoint GetNextWayPoint()
        {
            int i = _currentIndex + 1;
            if (i >= WayPoints.Count)
            {
                if (AutoAngler.Instance.MySettings.PathingType == PathingType.Bounce)
                    i = WayPoints.Count - 2;
                else
                    i = 0;
            }
            if (WayPoints != null && i < WayPoints.Count)
                return WayPoints[i];
            else
                return WoWPoint.Zero;
        }

        //if pool is between CurrentPoint and NextPoint then cycle to nextPoint
        public static void CycleToNextIfBehind(WoWGameObject pool)
        {
            WoWPoint cp = CurrentPoint;
            WoWPoint point = GetNextWayPoint();
            point = new WoWPoint(point.X - cp.X, point.Y - cp.Y, 0);
            point.Normalize();
            float angle = WoWMathHelper.NormalizeRadian((float)Math.Atan2(point.Y, point.X - 1));
            if (WoWMathHelper.IsFacing(CurrentPoint, angle, pool.Location, (float)Math.PI) &&
                CurrentPoint != WayPoints[WayPoints.Count - 1])
            {
                CycleToNextPoint();
            }
        }
    }
}
