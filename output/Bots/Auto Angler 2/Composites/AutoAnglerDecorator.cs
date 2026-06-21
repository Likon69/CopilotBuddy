using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSharp;
using Styx.WoWInternals;
using Styx.Logic.Combat;

namespace HighVoltz.Composites
{
    class AutoAnglerDecorator : Decorator
    {
        public AutoAnglerDecorator(Composite child) : base(child) { }
        //public override RunStatus Tick(object context)
        //{
        //    if (!CanRun(context))
        //        return RunStatus.Failure;
        //    if (!DecoratedChild.IsRunning)
        //        DecoratedChild.Start(context);
        //    return DecoratedChild.Tick(context);
        //}

        //protected override IEnumerable<RunStatus> Execute(object context)
        //{
        //    if (!CanRun(context))
        //    {
        //        yield return RunStatus.Failure;
        //        yield break;
        //    }
        //    DecoratedChild.Start(context);
        //    // stop executing children if 'CanRun' turns false...
        //    while (DecoratedChild.Tick(context) == RunStatus.Running && CanRun(context))
        //    {
        //        yield return RunStatus.Running;
        //    }
        //    DecoratedChild.Stop(context);
        //    if (DecoratedChild.LastStatus == RunStatus.Failure)
        //    {
        //        yield return RunStatus.Failure;
        //        yield break;
        //    }

        //    yield return RunStatus.Success;
        //    yield break;
        //}

        protected override bool CanRun(object context)
        {
            return ObjectManager.Me.IsAlive && !ObjectManager.Me.Combat &&
                !RoutineManager.Current.NeedRest;
        }
    }

}
