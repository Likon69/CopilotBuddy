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
using Styx.WoWInternals.WoWObjects;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Inventory.Frames.MailBox;
using System.Threading;
using System.Diagnostics;
using Styx.Logic.Combat;

namespace HighVoltz.Composites
{
    public class LogoutAction : Action
    {
        protected override RunStatus Run(object context)
        {
            if (ObjectManager.Me.Mounted)
                Mount.Dismount();
            Util1.UseItemByID(6948);
            Stopwatch hearthSW = new Stopwatch();
            hearthSW.Start();
            // since I'm logging out lets just abuse sleep anyways :D
            while (hearthSW.ElapsedMilliseconds < 20000)
            {
                // damn.. we got something beating on us... 
                if (ObjectManager.Me.Combat)
                    return RunStatus.Success;
                Thread.Sleep(100); // I feel so teribad... not!
            }
            AutoAngler.Instance.Log("Logging out");
            Lua.DoString("Logout()");
            TreeRoot.Stop();
            return RunStatus.Success;
        }
    }
}
