using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = TreeSharp.Action;
using Styx.Logic.POI;
using Styx.WoWInternals.WoWObjects;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.World;
using Styx.Logic;
using Styx.Logic.BehaviorTree;
using TreeSharp;
using Styx.Helpers;
using Styx;
using System.Diagnostics;
using Styx.Logic.Combat;
using Styx.Logic.Inventory.Frames.LootFrame;


namespace HighVoltz.Composites
{
    public class FishAction : Action
    {
        LocalPlayer _me = ObjectManager.Me;
        ulong _lastPoolGuid;
        Stopwatch _timeAtPoolSW = new Stopwatch();
        int _castCounter = 0;
        public FishAction()
        {

        }
        protected override TreeSharp.RunStatus Run(object context)
        {
            WoWGameObject pool = null;
            if (_me.IsMoving || _me.IsFalling)
            {
                Styx.WoWInternals.WoWMovement.MoveStop();
                if (!_me.HasAura("Levitate"))
                    return RunStatus.Success;
            }
            if (BotPoi.Current != null && BotPoi.Current.Type == PoiType.Harvest)
            {
                pool = (WoWGameObject)BotPoi.Current.AsObject;
                if (pool == null || !pool.IsValid)
                {
                    BotPoi.Current = null;
                    return RunStatus.Failure;
                }
                if (pool.Guid != _lastPoolGuid)
                {
                    _lastPoolGuid = pool.Guid;
                    _timeAtPoolSW.Reset();
                    _timeAtPoolSW.Start();
                }
                // safety check. if spending more than 5 mins at pool than black list it.
                if (_timeAtPoolSW.ElapsedMilliseconds >= AutoAngler.Instance.MySettings.MaxTimeAtPool * 60000)
                {
                    Util1.BlacklistPool(pool, TimeSpan.FromMinutes(10), "Spend too much time at pool");
                    return RunStatus.Failure;
                }
                // Blacklist pool if we have too many failed casts
                if (_castCounter >= AutoAngler.Instance.MySettings.MaxFailedCasts)
                {
                    AutoAngler.Instance.Log("Moving to a new fishing location since we have {0} failed casts", _castCounter);
                    _castCounter = 0;
                    MoveToPoolAction.PoolPoints.RemoveAt(0);
                    return RunStatus.Success;
                }

                // face pool if not facing it already.
                if (!IsFacing2D(_me.Location, _me.Rotation, pool.Location, WoWMathHelper.DegreesToRadians(5)))
                {
                    FishAction.LineRecastSW.Reset();
                    LineRecastSW.Start();
                    _me.SetFacing(pool.Location);
                    // SetFacing doesn't really update my angle in game.. still tries to fish using prev angle. so I need to move to update in-game angle
                    WoWMovement.Move(WoWMovement.MovementDirection.ForwardBackMovement);
                    WoWMovement.MoveStop(WoWMovement.MovementDirection.ForwardBackMovement);
                    return RunStatus.Success;
                }
            }
            if (_me.IsCasting)
            {
                WoWGameObject bobber = null;
                try
                {
                    bobber = ObjectManager.GetObjectsOfType<WoWGameObject>()
                        .FirstOrDefault(o => o.IsValid && o.SubType == WoWGameObjectType.FishingBobber &&
                            o.CreatedBy.Guid == _me.Guid);
                }
                catch { }
                if (bobber != null)
                {
                    // recast line if it's not close enough to pool
                    if (AutoAngler.Instance.MySettings.Fly && pool != null &&
                        bobber.Location.Distance(pool.Location) > 3.6f)
                    {
                        CastLine();
                    }
                    // else lets see if there's a bite!
                    else if (((WoWFishingBobber)bobber.SubObj).IsBobbing)
                    {
                        _castCounter = 0;
                        ((WoWFishingBobber)bobber.SubObj).Use();
                        LootAction.WaitingForLootSW.Reset();
                        LootAction.WaitingForLootSW.Start();
                    }
                }
                return RunStatus.Success;
            }
            else
            {
                CastLine();
                return RunStatus.Success;
            }
        }


        static public readonly Stopwatch LineRecastSW = new Stopwatch();
        void CastLine()
        {
            if (LineRecastSW.IsRunning && LineRecastSW.ElapsedMilliseconds < 2000)
                return;
            LineRecastSW.Reset();
            _castCounter++;
            SpellManager.Cast("Fishing");
            LineRecastSW.Start();
        }

        public static bool IsFacing2D(WoWPoint me, float myFacingRadians, WoWPoint target, float arcRadians)
        {
            bool result;
            me.Z = target.Z = 0;
            arcRadians = WoWMathHelper.NormalizeRadian(arcRadians);
            float num = WoWMathHelper.CalculateNeededFacing(me, target);
            float num2 = WoWMathHelper.NormalizeRadian(num - myFacingRadians);
            if ((double)num2 > 3.1415926535897931)
            {
                num2 = (float)(6.2831853071795862 - (double)num2);
            }
            result = (num2 <= arcRadians / 2f);
            return result;
        }
    }
}
