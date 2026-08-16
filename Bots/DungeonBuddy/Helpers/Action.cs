using System;
using TreeSharp;

namespace Bots.DungeonBuddy.Helpers
{
    public class Action<T> : TreeSharp.Action
    {
        private readonly Func<T, RunStatus>? _actionRunner;
        private readonly System.Action<T>? _successRunner;

        public Action(Func<T, RunStatus> actionRunner)
        {
            _actionRunner = actionRunner;
        }

        public Action(System.Action<T> successRunner)
        {
            _successRunner = successRunner;
        }

        protected override RunStatus Run(object context)
        {
            if (context is not T typedContext)
                return RunStatus.Failure;

            if (_actionRunner != null)
                return _actionRunner(typedContext);

            _successRunner!(typedContext);
            return RunStatus.Success;
        }
    }
}
