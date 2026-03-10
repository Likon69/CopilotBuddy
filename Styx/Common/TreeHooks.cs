using System;
using System.Collections.Generic;
using System.Threading;
using Styx.Helpers;
using TreeSharp;

namespace Styx.Common
{
    /// <summary>
    /// Port of HB 6.2.3 Styx.Common.TreeHooks.
    /// Singleton registry that plugins use to inject behaviors into the bot loop.
    /// Plugins call AddHook/InsertHook/RemoveHook/ReplaceHook to modify named
    /// hook locations. HookExecutor reads these at Start() time and rebuilds
    /// its PrioritySelector when the guard counter changes.
    /// </summary>
    public sealed class TreeHooks
    {
        private static TreeHooks? _instance;

        private TreeHooks()
        {
            Hooks = new Dictionary<string, List<CompositeListOperation>>();
            HookDescriptions = new List<HookDescription>();
            _guardCounters = new Dictionary<string, int>();
        }

        /// <summary>HB 6.2.3: Lazy singleton.</summary>
        public static TreeHooks Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TreeHooks();
                }
                return _instance;
            }
        }

        /// <summary>Hook location name → list of pending operations.</summary>
        public Dictionary<string, List<CompositeListOperation>> Hooks { get; private set; }

        /// <summary>Metadata for registered hooks.</summary>
        public List<HookDescription> HookDescriptions { get; private set; }

        /// <summary>HB 6.2.3 dictionary_1: Guard counter per hook location.
        /// Incremented on every Add/Insert/Remove/Replace. HookExecutor compares
        /// its cached counter to detect changes.</summary>
        private readonly Dictionary<string, int> _guardCounters;

        private System.Action? _hooksCleared;

        /// <summary>HB 6.2.3: Fired when ClearAll() is called.</summary>
        public event System.Action HooksCleared
        {
            add
            {
                System.Action? action = _hooksCleared;
                System.Action? action2;
                do
                {
                    action2 = action;
                    action = Interlocked.CompareExchange(ref _hooksCleared, (System.Action?)Delegate.Combine(action2, value), action);
                }
                while (action != action2);
            }
            remove
            {
                System.Action? action = _hooksCleared;
                System.Action? action2;
                do
                {
                    action2 = action;
                    action = Interlocked.CompareExchange(ref _hooksCleared, (System.Action?)Delegate.Remove(action2, value), action);
                }
                while (action != action2);
            }
        }

        /// <summary>HB 6.2.3: Append a composite to a named hook location.</summary>
        public void AddHook(string location, Composite behavior)
        {
            EnsureHookAndIncrementGuard(location);
            Hooks[location].Add(new AddCompositeListOperation(behavior));
            Logging.WriteDiagnostic("Added new hook [{0}] {1}", location, behavior.Guid);
        }

        /// <summary>HB 6.2.3: Insert a composite at a specific index.</summary>
        public void InsertHook(string location, int index, Composite behavior)
        {
            EnsureHookAndIncrementGuard(location);
            Hooks[location].Add(new InsertCompositeListOperation(behavior, index));
            Logging.WriteDiagnostic("Inserted new hook [{0} @{1}] {2}", location, index, behavior.Guid);
        }

        /// <summary>HB 6.2.3: Remove a specific composite from a hook location.</summary>
        public void RemoveHook(string location, Composite behavior)
        {
            if (!Hooks.TryGetValue(location, out List<CompositeListOperation>? list))
                return;
            if (list.RemoveAll(op => op.Composite == behavior) > 0)
            {
                EnsureHookAndIncrementGuard(location);
            }
            Logging.WriteDiagnostic("Removed hook [{0}] {1}", location, behavior.Guid);
        }

        /// <summary>HB 6.2.3: Replace all operations at a location with a single composite.</summary>
        public void ReplaceHook(string location, Composite behavior)
        {
            EnsureHookAndIncrementGuard(location);
            Hooks[location].Clear();
            Hooks[location].Add(new ReplaceCompositeListOperation(behavior));
            if (behavior != null)
            {
                Logging.WriteDiagnostic("Replaced hook [{0}] {1}", location, behavior.Guid);
            }
        }

        /// <summary>HB 6.2.3: Apply all registered operations to a composite list.
        /// Called by HookExecutor.Start() to build the final PrioritySelector.</summary>
        public void ApplyCompositeOperations(string location, List<Composite> compositeList)
        {
            if (compositeList == null)
                throw new ArgumentNullException(nameof(compositeList));
            if (!Hooks.TryGetValue(location, out List<CompositeListOperation>? list))
                return;
            foreach (CompositeListOperation op in list)
            {
                op.ApplyTo(compositeList);
            }
        }

        /// <summary>HB 6.2.3: Clear all hooks and fire event.</summary>
        public void ClearAll()
        {
            Hooks.Clear();
            HookDescriptions.Clear();
            _hooksCleared?.Invoke();
        }

        /// <summary>HB 6.2.3 method_1: Get guard counter for a hook location.
        /// HookExecutor compares this against its cached value to detect changes.</summary>
        internal int GetGuardCounter(string location)
        {
            if (!_guardCounters.TryGetValue(location, out int num))
                return 0;
            return num;
        }

        /// <summary>HB 6.2.3 method_0: Initialize hook list if needed + increment guard counter.</summary>
        private void EnsureHookAndIncrementGuard(string location)
        {
            if (!Hooks.ContainsKey(location))
            {
                Hooks.Add(location, new List<CompositeListOperation>());
            }
            if (!_guardCounters.TryGetValue(location, out int num))
            {
                _guardCounters[location] = 1;
                return;
            }
            _guardCounters[location] = num + 1;
        }
    }

    /// <summary>HB 6.2.3: Abstract base for hook operations.</summary>
    public abstract class CompositeListOperation
    {
        protected CompositeListOperation(Composite composite)
        {
            Composite = composite;
        }

        public Composite Composite { get; }
        public abstract void ApplyTo(List<Composite> compositeList);
    }

    /// <summary>HB 6.2.3: Appends composite to end of list.</summary>
    public class AddCompositeListOperation : CompositeListOperation
    {
        public AddCompositeListOperation(Composite composite) : base(composite) { }

        public override void ApplyTo(List<Composite> compositeList)
        {
            compositeList.Add(Composite);
        }
    }

    /// <summary>HB 6.2.3: Inserts composite at index (bounds-safe).</summary>
    public class InsertCompositeListOperation : CompositeListOperation
    {
        public InsertCompositeListOperation(Composite composite, int index) : base(composite)
        {
            Index = index;
        }

        public int Index { get; }

        public override void ApplyTo(List<Composite> compositeList)
        {
            compositeList.Insert(Math.Max(0, Math.Min(compositeList.Count, Index)), Composite);
        }
    }

    /// <summary>HB 6.2.3: Replaces entire list with a single composite.</summary>
    public class ReplaceCompositeListOperation : CompositeListOperation
    {
        public ReplaceCompositeListOperation(Composite composite) : base(composite) { }

        public override void ApplyTo(List<Composite> compositeList)
        {
            compositeList.Clear();
            if (Composite != null)
            {
                compositeList.Add(Composite);
            }
        }
    }

    /// <summary>HB 6.2.3: Metadata for registered hooks (name + description), identified by GUID.</summary>
    public class HookDescription
    {
        private readonly Guid _guid = Guid.NewGuid();

        public HookDescription(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }
        public string? Description { get; set; }

        public bool Equals(HookDescription other)
        {
            return other != null && (this == other || other._guid.Equals(_guid));
        }

        public override bool Equals(object? obj)
        {
            return obj != null && (this == obj || (obj is HookDescription hd && Equals(hd)));
        }

        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }
    }
}
