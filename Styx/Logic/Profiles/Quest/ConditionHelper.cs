using Microsoft.CSharp;
using Styx.Helpers;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace Styx.Logic.Profiles.Quest
{
    public class ConditionHelper
    {
        private static long _conditionCounter;
        private readonly string _conditionString;
        private readonly long _conditionId;

        public ConditionHelper(string conditionString)
        {
            _conditionString = conditionString;
            _conditionId = _conditionCounter++;
        }

        private string GenerateCode()
        {
            return $@"using System;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using Styx;
using Styx.Helpers;
using Styx.Logic.Combat;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Logic;
using Styx.Logic.AreaManagement;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Inventory.Frames.LootFrame;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;
using Styx.Logic.Questing;
using Styx.Plugins;
using Styx.Plugins.PluginClass;
using Styx.WoWInternals.World;
using Styx.Combat.CombatRoutine;

namespace DynamicCondition{_conditionId}
{{
    public class DynamicCondition : Styx.Logic.Profiles.Quest.ProfileHelperFunctionsBase
    {{
        public bool EvaluateExpression()
        {{
            return {_conditionString};
        }}
    }}
}}";
        }

        public bool CompileAndBindExpression(out string[] buildErrors, out Func<bool> boundExpression)
        {
            CompilerResults compilerResults;
            using (CSharpCodeProvider provider = new CSharpCodeProvider())
            {
                CompilerParameters options = new CompilerParameters
                {
                    GenerateInMemory = true
                };
                
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(assembly.Location))
                            options.ReferencedAssemblies.Add(assembly.Location);
                    }
                    catch { }
                }
                
                compilerResults = provider.CompileAssemblyFromSource(options, GenerateCode());
            }

            if (compilerResults.Errors.HasErrors)
            {
                buildErrors = compilerResults.Errors.Cast<CompilerError>()
                    .Select(e => e.ErrorText).ToArray();
                boundExpression = null;
                return false;
            }

            object instance = Activator.CreateInstance(
                compilerResults.CompiledAssembly.GetType($"DynamicCondition{_conditionId}.DynamicCondition"));
            boundExpression = (Func<bool>)Delegate.CreateDelegate(
                typeof(Func<bool>), instance, "EvaluateExpression");
            buildErrors = null;
            return true;
        }

        public static Func<bool> ParseConditionString(string str)
        {
            Func<bool> conditionString;
            try
            {
                conditionString = new Func<bool>(new CompiledCondition(str).Evaluate);
            }
            catch (Styx.CantCompileException)
            {
                conditionString = null;
            }
            return conditionString;
        }

        internal static Func<bool> CompileCondition(XAttribute conditionAttribute)
        {
            Func<bool> func;
            try
            {
                IXmlLineInfo xmlLineInfo = (IXmlLineInfo)conditionAttribute;
                func = !xmlLineInfo.HasLineInfo()
                    ? new Func<bool>(new CompiledCondition(conditionAttribute.Value).Evaluate)
                    : new Func<bool>(new CompiledCondition(conditionAttribute.Value, xmlLineInfo.LineNumber).Evaluate);
            }
            catch (Styx.CantCompileException)
            {
                func = null;
            }
            return func;
        }

        private class CompiledCondition
        {
            private static readonly Dictionary<string, Func<bool>> _cache = new Dictionary<string, Func<bool>>();
            private static readonly object _lock = new object();
            private readonly string _expression;
            private Func<bool> _compiled;
            private bool _initialized;
            private readonly int? _lineNumber;

            public CompiledCondition(string expression, int? lineNumber = null)
            {
                _expression = expression;
                _lineNumber = lineNumber;
                if (StyxSettings.Instance.ProfileDebuggingMode)
                {
                    _compiled = Compile();
                    _initialized = true;
                    if (_compiled == null)
                        throw new Styx.CantCompileException();
                }
            }

            public bool Evaluate()
            {
                if (!_initialized)
                {
                    _compiled = Compile();
                    _initialized = true;
                }
                return _compiled != null && _compiled();
            }

            private Func<bool> Compile()
            {
                if (_cache.TryGetValue(_expression, out Func<bool> cached))
                    return cached;

                lock (_lock)
                {
                    if (_cache.TryGetValue(_expression, out cached))
                        return cached;

                    var helper = new ConditionHelper(_expression);
                    if (_lineNumber.HasValue)
                        Logging.WriteDebug("Compiling expression '{0}' @ line {1}", (object)_expression, (object)_lineNumber.Value);
                    else
                        Logging.WriteDebug("Compiling expression '{0}'", (object)_expression);

                    if (helper.CompileAndBindExpression(out string[] errors, out Func<bool> bound))
                    {
                        _cache[_expression] = bound;
                        return bound;
                    }
                    else
                    {
                        if (errors.Length > 0)
                        {
                            Logging.WriteDebug("{0} errors encountered while compiling condition '{1}'", (object)errors.Length, (object)_expression);
                            foreach (var error in errors)
                                Logging.WriteDebug(error);
                        }
                        _cache[_expression] = null;
                        return null;
                    }
                }
            }
        }
    }
}
