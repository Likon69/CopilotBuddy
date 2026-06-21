using System;
using System.Collections.Generic;
using Action = TreeSharp.Action;
using Styx.Logic.POI;
using Styx.WoWInternals.WoWObjects;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.World;
using Styx.Logic;
using Styx.Logic.BehaviorTree;
using TreeSharp;
using Styx;
using Styx.Helpers;
using System.Diagnostics;
using Styx.Logic.Combat;

namespace HighVoltz.Composites
{
    public class MoveToPoolAction : Action
    {
        LocalPlayer _me = ObjectManager.Me;
        public static readonly List<WoWPoint> PoolPoints = new List<WoWPoint>();
        ulong lastPoolGuid = 0;
        Stopwatch _movetoConcludingSW = new Stopwatch();
        public readonly static Stopwatch MoveToPoolSW = new Stopwatch(); // used to auto blacklist a pool if it takes too long to get to a point.
        protected override TreeSharp.RunStatus Run(object context)
        {
            if (BotPoi.Current != null && BotPoi.Current.Type == PoiType.Harvest)
            {
                var pool = (WoWGameObject)BotPoi.Current.AsObject;
                if (pool != null && pool.IsValid)
                {
                    return GotoPool(pool);
                }
                else
                    BotPoi.Current = null;
            }

            return TreeSharp.RunStatus.Failure;
        }

        bool FindPoolPoint(WoWGameObject pool)
        {
            int traceStep = AutoAngler.Instance.MySettings.TraceStep;
            float _PIx2 = 3.14159f * 2f;
            WoWPoint playerLoc = _me.Location;
            WoWPoint p = new WoWPoint();
            WoWPoint hPoint = new WoWPoint();
            WoWPoint lPoint = new WoWPoint();
            WorldLine[] traceLine = new WorldLine[traceStep];
            PoolPoints.Clear();

            // scans starting at 15 yards from player for water at every 18 degress 
            bool[] tracelineRetVals;

            float range = 15;
            int min = AutoAngler.Instance.MySettings.MinPoolRange;
            int max = AutoAngler.Instance.MySettings.MaxPoolRange;
            float step = AutoAngler.Instance.MySettings.PoolRangeStep;
            float delta = step;
            float avg = (min + max) / 2;
            while (true)
            {
                for (int i = 0; i < traceStep; i++)
                {
                    p = pool.Location.RayCast((i * _PIx2) / traceStep, range);
                    hPoint = p; hPoint.Z += 45; lPoint = p; lPoint.Z -= 1;
                    traceLine[i].Start = hPoint;
                    traceLine[i].End = lPoint;
                }
                WoWPoint[] hitPoints;
                GameWorld.MassTraceLine(traceLine, GameWorld.CGWorldFrameHitFlags.HitTestGroundAndStructures,
                    out tracelineRetVals, out hitPoints);
                // what I'm doing here is compare the elevation of 4 corners around a point with 
                // that point's elevation to determine if that point is too steep to stand on.
                List<WorldLine> slopetraces = new List<WorldLine>();
                List<WoWPoint> testPoints = new List<WoWPoint>();
                for (int i = 0; i < traceStep; i++)
                {
                    if (tracelineRetVals[i])
                    {
                        slopetraces.AddRange(GetQuadSloopTraceLines(hitPoints[i]));
                        testPoints.Add(hitPoints[i]);
                    }
                    else if (WaterWalking.CanCast)
                    {
                        traceLine[i].End.Z = pool.Z + 1;
                        PoolPoints.Add(traceLine[i].End);
                    }
                }
                // fire tracelines.. 
                bool[] slopelinesRetVals, lavaRetVals = null;
                WoWPoint[] slopeHits;
                using (new FrameLock())
                {
                    GameWorld.MassTraceLine(slopetraces.ToArray(), GameWorld.CGWorldFrameHitFlags.HitTestGroundAndStructures,
                    out slopelinesRetVals, out slopeHits);
                    if (AutoAngler.Instance.MySettings.AvoidLava)
                    {
                        GameWorld.MassTraceLine(slopetraces.ToArray(), GameWorld.CGWorldFrameHitFlags.HitTestLiquid2,
                         out lavaRetVals);
                    }
                }

                // process results
                PoolPoints.AddRange(ProcessSlopeAndLavaResults(testPoints, slopeHits, lavaRetVals));
                // perform LOS checks
                if (PoolPoints.Count > 0)
                {
                    WorldLine[] losLine = new WorldLine[PoolPoints.Count];
                    for (int i2 = 0; i2 < PoolPoints.Count; i2++)
                    {
                        WoWPoint point = PoolPoints[i2];
                        point.Z += 2;
                        losLine[i2].Start = point;
                        losLine[i2].End = pool.Location;
                    }
                    GameWorld.MassTraceLine(losLine, GameWorld.CGWorldFrameHitFlags.HitTestGroundAndStructures,
                        out tracelineRetVals);
                    for (int i2 = PoolPoints.Count - 1; i2 >= 0; i2--)
                    {
                        if (tracelineRetVals[i2])
                            PoolPoints.RemoveAt(i2);
                    }
                }
                // sort pools by distance to player                
                PoolPoints.Sort((p1, p2) => p1.Distance(_me.Location).CompareTo(p2.Distance(_me.Location)));
                if (!_me.IsFlying)
                {
                    // if we are not flying check if we can genorate a path to points.
                    for (int i = 0; i < PoolPoints.Count; )
                    {
                        WoWPoint[] testP = Navigator.GeneratePath(_me.Location, PoolPoints[i]);
                        if (testP.Length > 0)
                        {
                            return true;
                        }
                        else
                        {
                            PoolPoints.RemoveAt(i);
                            PoolPoints.Sort((a, b) => a.Distance(_me.Location).CompareTo(b.Distance(_me.Location)));
                        }
                    }
                }
                if (PoolPoints.Count > 0)
                    return true;
                bool minCaped = (15 - delta) < min;
                bool maxCaped = (15 + delta) > max;
                if (minCaped && maxCaped)
                    break;

                if ((range <= 15 && (15 + delta) <= max) || minCaped)
                {
                    range = 15 + delta;
                    if (avg < 15 || minCaped)
                        delta += step;
                    continue;
                }

                if ((range > 15 && (15 - delta) >= min) || maxCaped)
                {
                    range = 15 - delta;
                    if (avg >= 15 || maxCaped)
                        delta += step;
                    continue;
                }
            }
            return false;
        }

        static public WorldLine GetSlopeTraceLine(WoWPoint point, float xDelta, float yDelta)
        {
            WoWPoint topP = point;
            topP.X += xDelta;
            topP.Y += yDelta;
            topP.Z += 6;
            WoWPoint botP = topP;
            botP.Z -= 12;
            return new WorldLine(topP, botP);
        }

        static public List<WorldLine> GetQuadSloopTraceLines(WoWPoint point)
        {
            //float delta = AutoAngler2.Instance.MySettings.LandingSpotWidth / 2;
            float delta = 0.5f;
            List<WorldLine> wl = new List<WorldLine>();
            // north west
            wl.Add(GetSlopeTraceLine(point, delta, -delta));
            // north east
            wl.Add(GetSlopeTraceLine(point, delta, delta));
            // south east
            wl.Add(GetSlopeTraceLine(point, -delta, delta));
            // south west
            wl.Add(GetSlopeTraceLine(point, -delta, -delta));
            return wl;
        }

        static public List<WoWPoint> ProcessSlopeAndLavaResults(List<WoWPoint> testPoints, WoWPoint[] slopePoints,
            bool[] lavaHits)
        {
            //float slopeRise = AutoAngler2.Instance.MySettings.LandingSpotSlope / 2;
            float slopeRise = 0.60f;
            List<WoWPoint> retList = new List<WoWPoint>();
            for (int i = 0; i < testPoints.Count; i++)
            {
                if (slopePoints[i * 4] != WoWPoint.Zero &&
                    slopePoints[i * 4 + 1] != WoWPoint.Zero &&
                    slopePoints[i * 4 + 2] != WoWPoint.Zero &&
                    slopePoints[i * 4 + 3] != WoWPoint.Zero &&
                    // check for lava hits
                    (lavaHits == null ||
                    (lavaHits != null &&
                    !lavaHits[i * 4] &&
                    !lavaHits[i * 4 + 1] &&
                    !lavaHits[i * 4 + 2] &&
                    !lavaHits[i * 4 + 3]))
                    )
                {
                    if (ElevationDifference(testPoints[i], slopePoints[(i * 4)]) <= slopeRise &&
                        ElevationDifference(testPoints[i], slopePoints[(i * 4) + 1]) <= slopeRise &&
                        ElevationDifference(testPoints[i], slopePoints[(i * 4) + 2]) <= slopeRise &&
                        ElevationDifference(testPoints[i], slopePoints[(i * 4) + 3]) <= slopeRise)
                    {
                        retList.Add(testPoints[i]);
                    }
                }
            }
            return retList;
        }

        static public float ElevationDifference(WoWPoint p1, WoWPoint p2)
        {
            if (p1.Z > p2.Z)
                return p1.Z - p2.Z;
            else
                return p2.Z - p1.Z;
        }

        RunStatus GotoPool(WoWGameObject pool)
        {
            if (lastPoolGuid != pool.Guid)
            {
                MoveToPoolSW.Reset();
                MoveToPoolSW.Start();
                lastPoolGuid = pool.Guid;
                if (!FindPoolPoint(pool) || PoolPoints.Count == 0)
                {
                    Util1.BlacklistPool(pool, TimeSpan.FromDays(1), "Found no landing spots");
                    return RunStatus.Failure;
                }
            }
            // should never be true.. but being safe..
            if (PoolPoints.Count == 0)
            {
                Util1.BlacklistPool(pool, TimeSpan.FromDays(1), "Pool landing points mysteriously disapear...");
                return RunStatus.Failure;
            }
            TreeRoot.StatusText = "Moving to " + pool.Name;
            if (_me.Location.Distance(PoolPoints[0]) > 3)
            {
                _movetoConcludingSW.Reset();
                if (!MoveToPoolSW.IsRunning)
                    MoveToPoolSW.Start();
                if (_me.IsSwimming)
                {
                    if (_me.GetMirrorTimerInfo(MirrorTimerType.Breath).CurrentTime > 0)
                        WoWMovement.Move(WoWMovement.MovementDirection.JumpAscend);
                    else if (_me.MovementInfo.IsAscending || _me.MovementInfo.JumpingOrShortFalling)
                        WoWMovement.MoveStop(WoWMovement.MovementDirection.JumpAscend);
                }
                if (AutoAngler.Instance.MySettings.Fly)
                {
                    // don't bother mounting up if we can use navigator to walk over if it's less than 25 units away
                    if (_me.Location.Distance(PoolPoints[0]) < 25 && !_me.Mounted)
                    {
                        var moveResult = Navigator.MoveTo(PoolPoints[0]);
                        if (moveResult == MoveResult.Failed || moveResult == MoveResult.PathGenerationFailed)
                        {
                            Flightor.MountHelper.MountUp();
                        }
                        else
                            return RunStatus.Success;
                    }
                    else if (!_me.Mounted && !SpellManager.GlobalCooldown)
                        Flightor.MountHelper.MountUp();
                    Flightor.MoveTo(WoWMathHelper.CalculatePointFrom(_me.Location, PoolPoints[0], -1f));
                }
                else
                {
                    if (!ObjectManager.Me.Mounted && Mount.ShouldMount(PoolPoints[0]) && Mount.CanMount())
                        Mount.MountUp();
                    var moveResult = Navigator.MoveTo(PoolPoints[0]);
                    if (moveResult == MoveResult.UnstuckAttempt ||
                        moveResult == MoveResult.PathGenerationFailed || moveResult == MoveResult.Failed)
                    {
                        if (!RemovePointAtTop(pool))
                            return RunStatus.Failure;
                        AutoAngler.Instance.Debug("Unable to path to pool point, switching to a new point");
                        PoolPoints.Sort((a, b) => a.Distance(_me.Location).CompareTo(b.Distance(_me.Location)));
                    }
                }
                // if it takes more than 25 seconds to get to a point remove that point and try another.
                if (MoveToPoolSW.ElapsedMilliseconds > 25000)
                {
                    if (!RemovePointAtTop(pool))
                        return RunStatus.Failure;
                    MoveToPoolSW.Reset();
                    MoveToPoolSW.Start();
                }
                return RunStatus.Success;
            }
            else
            {
                // allow small delay so clickToMove can run its course before dismounting. better landing precision..
                if (!_movetoConcludingSW.IsRunning)
                    _movetoConcludingSW.Start();
                if (_movetoConcludingSW.ElapsedMilliseconds < 1500)
                {
                    if (_me.Location.Distance2D(PoolPoints[0]) > 0.5)
                        WoWMovement.ClickToMove(PoolPoints[0]);
                    return RunStatus.Success;
                }
                if (_me.Mounted)
                {
                    if (_me.Class == Styx.Combat.CombatRoutine.WoWClass.Druid &&
                        (_me.Shapeshift == ShapeshiftForm.FlightForm || _me.Shapeshift == ShapeshiftForm.EpicFlightForm))
                    {
                        Lua.DoString("CancelShapeshiftForm()");
                    }
                    else
                        Lua.DoString("Dismount()");
                }
                // can't fish while swimming..
                if (_me.IsSwimming && !WaterWalking.CanCast)
                {
                    AutoAngler.Instance.Debug("Moving to new PoolPoint since I'm swimming at current PoolPoint");
                    RemovePointAtTop(pool);
                    return RunStatus.Success;
                }
                return RunStatus.Failure;
            }
        }

        bool RemovePointAtTop(WoWGameObject pool)
        {
            PoolPoints.RemoveAt(0);
            _movetoConcludingSW.Reset();
            if (PoolPoints.Count == 0)
            {
                Util1.BlacklistPool(pool, TimeSpan.FromMinutes(10), "No Landing spot found");
                return false;
            }
            return true;
        }
    }
}
