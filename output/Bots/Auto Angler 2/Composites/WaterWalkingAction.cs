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
using Styx;
using Styx.Helpers;
using System.Diagnostics;

namespace HighVoltz.Composites
{
    public class WaterWalkingAction : Action
    {
        LocalPlayer _me = ObjectManager.Me;
        protected override RunStatus Run(object context)
        {
            // refresh water walking if needed
            if (!_me.Mounted && WaterWalking.CanCast && (!WaterWalking.IsActive || _me.IsSwimming))
            {
                WaterWalking.Cast();
                return RunStatus.Success;
            }
            return RunStatus.Failure;
        }
    }
}
