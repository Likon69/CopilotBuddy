using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Logic;
using Styx;
using Styx.Logic.POI;
using Styx.Logic.Profiles;
using Styx.Helpers;

namespace HighVoltz.Composites
{
    class HarvestPoolDecorator:Decorator
    {
        public HarvestPoolDecorator(Composite child) : base(child) { }
        protected override bool CanRun(object context)
        {
            if (!AutoAngler.Instance.MySettings.Poolfishing)
                return true;
            WoWGameObject pool = ObjectManager.GetObjectsOfType<WoWGameObject>()
            .OrderBy(o => o.Distance)
            .FirstOrDefault(o => o.SubType == WoWGameObjectType.FishingHole && !Blacklist.Contains(o.Guid) &&
                // Check if we're fishing from specific pools
                ((AutoAngler.PoolsToFish.Count > 0 && AutoAngler.PoolsToFish.Contains(o.Entry))
                || AutoAngler.PoolsToFish.Count == 0) && 
                // chaeck if pool is in a blackspot
                !IsInBlackspot(o) && 
                // check if player is near pool
                NinjaCheck(o) );

            WoWGameObject poiObj = BotPoi.Current != null && BotPoi.Current.Type == PoiType.Harvest ?
                (WoWGameObject)BotPoi.Current.AsObject : null;
            if (pool != null )
            {
                if (poiObj == null || poiObj.Entry != pool.Entry)
                {
                    BotPoi.Current = new BotPoi(pool, PoiType.Harvest);
                    FollowPathAction.CycleToNextIfBehind(pool);
                }
                return true;
            }
            return false;
        }

        ulong _lastPoolGuid = 0;
        bool NinjaCheck(WoWGameObject pool)
        {
            if (pool.Guid == _lastPoolGuid)
                return true;
            _lastPoolGuid = pool.Guid;
            bool ret = (!(!AutoAngler.Instance.MySettings.NinjaNodes &&
                ObjectManager.GetObjectsOfType<WoWPlayer>(false, false).
                Any(p => !p.IsFlying && p.Location.Distance2D(pool.Location) < 20 )));
            if (!ret)
                Util1.BlacklistPool(pool, TimeSpan.FromMinutes(1), "Another player fishing that pool");
            return ret;
        }

        bool IsInBlackspot(WoWGameObject pool)
        {
            if (ProfileManager.CurrentProfile != null && ProfileManager.CurrentProfile.Blackspots != null)
            {
                foreach (var blackSpot in ProfileManager.CurrentProfile.Blackspots)
                {
                    if (blackSpot.Location.Distance2D(pool.Location) <= blackSpot.Radius)
                    {
                        AutoAngler.Instance.Log("Ignoring {0} at {1} since it's in a BlackSpot",pool.Name,pool.Location);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
