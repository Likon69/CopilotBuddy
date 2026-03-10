using System;
using System.Collections.Generic;
using System.Linq;
using TreeSharp;

namespace Styx.Common
{
    /// <summary>
    /// Port of HB 6.2.3 Styx.Common.HookExecutor.
    /// A TreeSharp Action that reads TreeHooks at Start() time, rebuilds
    /// a PrioritySelector from registered operations, and ticks it.
    /// Uses a guard counter to detect when hooks change.
    /// </summary>
    public class HookExecutor : TreeSharp.Action
    {
        private readonly Composite? _defaultComposite;
        private readonly string _location;
        private Composite? _compiled;
        private int _guardCounter = -1;

        public HookExecutor(string location, string? description = null, Composite? defaultComposite = null)
        {
            _location = location;
            _defaultComposite = defaultComposite;

            if (description != null &&
                !TreeHooks.Instance.HookDescriptions.Any(
                    hd => hd.Name == location && hd.Description == description))
            {
                TreeHooks.Instance.HookDescriptions.Add(new HookDescription(location, description));
            }
        }

        public override void Start(object? context)
        {
            base.Start(context);

            int num = TreeHooks.Instance.GetGuardCounter(_location);
            if (num != _guardCounter)
            {
                _guardCounter = num;

                List<Composite> list = new List<Composite>();
                if (_defaultComposite != null)
                {
                    list.Add(_defaultComposite);
                }

                TreeHooks.Instance.ApplyCompositeOperations(_location, list);

                PrioritySelector ps = new PrioritySelector(Array.Empty<Composite>());
                list.ForEach(c => ps.AddChild(c));
                _compiled = ps;
            }

            _compiled!.Start(context);
        }

        public override void Stop(object? context)
        {
            _compiled?.Stop(context);
            base.Stop(context);
        }

        protected override RunStatus Run(object? context)
        {
            return _compiled!.Tick(context);
        }
    }
}
