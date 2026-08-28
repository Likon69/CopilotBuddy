using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Styx.Helpers;
using Styx.Combat.CombatRoutine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Styx.Logic.Questing;

#nullable disable
namespace Styx.Logic.Profiles.Quest
{
    /// <summary>
    /// Compiles and evaluates C# condition expressions at runtime.
    /// Implements same pattern as HB 6.2.3 ConditionHelper with:
    /// - Fast-path parser for common expressions (Class, Race, Level, HasQuest, IsQuestCompleted)
    /// - Roslyn fallback for complex expressions
    /// - Expression caching to prevent memory issues
    /// </summary>
    public class ConditionHelper
    {
        private static long _conditionCounter;
        private readonly string _conditionString;
        private readonly long _conditionId;
        
        // Script options with all required assemblies and imports (same as HB 4.3.4)
        private static ScriptOptions _scriptOptions;
        private static bool _scriptOptionsInitialized = false;
        private static readonly object _scriptOptionsLock = new object();
        
        private static ScriptOptions GetScriptOptions()
        {
            if (_scriptOptionsInitialized)
                return _scriptOptions;
                
            lock (_scriptOptionsLock)
            {
                if (_scriptOptionsInitialized)
                    return _scriptOptions;
                    
                // Build script options once - same imports as HB 4.3.4 ConditionHelper
                _scriptOptions = ScriptOptions.Default
                    .AddReferences(
                        typeof(object).Assembly,                           // System.Runtime
                        typeof(Enumerable).Assembly,                       // System.Linq
                        typeof(Styx.StyxWoW).Assembly,                     // CopilotBuddy (this assembly)
                        typeof(Styx.WoWInternals.ObjectManager).Assembly,
                        typeof(Styx.Combat.CombatRoutine.WoWClass).Assembly
                    )
                    .AddImports(
                        "System",
                        "System.Linq",
                        "Styx",
                        "Styx.Helpers",
                        "Styx.Logic.Combat",
                        "Styx.WoWInternals",
                        "Styx.WoWInternals.WoWObjects",
                        "Styx.Combat.CombatRoutine"
                    );
                _scriptOptionsInitialized = true;
                return _scriptOptions;
            }
        }

        public ConditionHelper(string conditionString)
        {
            _conditionString = conditionString;
            _conditionId = _conditionCounter++;
        }

        /// <summary>
        /// Compile expression and return bound delegate using Roslyn
        /// </summary>
        public bool CompileAndBindExpression(out string[] buildErrors, out Func<bool> boundExpression)
        {
            try
            {
                // Create a globals object that provides Me property (like ProfileHelperFunctionsBase)
                var script = CSharpScript.Create<bool>(
                    _conditionString,
                    GetScriptOptions(),
                    globalsType: typeof(ConditionGlobals)
                );
                
                // Compile the script
                var compilation = script.Compile();
                
                if (compilation.Any())
                {
                    buildErrors = compilation.Select(d => d.GetMessage()).ToArray();
                    boundExpression = null;
                    return false;
                }
                
                // Create the bound expression that evaluates with current game state
                boundExpression = () =>
                {
                    try
                    {
                        var globals = new ConditionGlobals();
                        var result = script.RunAsync(globals).GetAwaiter().GetResult();
                        return result.ReturnValue;
                    }
                    catch (Exception ex)
                    {
                        Logging.WriteDebug("Error evaluating condition '{0}': {1}", _conditionString, ex.Message);
                        return false;
                    }
                };
                
                buildErrors = null;
                return true;
            }
            catch (Exception ex)
            {
                buildErrors = new[] { ex.Message };
                boundExpression = null;
                return false;
            }
        }

        public static Func<bool> ParseConditionString(string str)
        {
            Func<bool> conditionString;
            try
            {
                conditionString = new Func<bool>(new ExpressionBinder(str).Evaluate);
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
                    ? new Func<bool>(new ExpressionBinder(conditionAttribute.Value).Evaluate)
                    : new Func<bool>(new ExpressionBinder(conditionAttribute.Value, xmlLineInfo.LineNumber).Evaluate);
            }
            catch (Styx.CantCompileException)
            {
                func = null;
            }
            return func;
        }

        /// <summary>
        /// Fast-path expression parser for common profile conditions.
        /// Returns null if expression cannot be parsed fast-path (needs Roslyn).
        /// Same concept as Class1204.smethod_0 in HB 6.2.3
        /// </summary>
        private static class FastPathParser
        {
            // Pattern for Me.Class == WoWClass.XXX
            private static readonly Regex ClassPattern = new Regex(
                @"^\s*\(?\s*Me\.Class\s*==\s*WoWClass\.(\w+)\s*\)?\s*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            // Pattern for Me.Race == WoWRace.XXX
            private static readonly Regex RacePattern = new Regex(
                @"^\s*\(?\s*Me\.Race\s*==\s*WoWRace\.(\w+)\s*\)?\s*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            // Pattern for Me.Level < N or Me.Level > N or Me.Level >= N or Me.Level <= N
            private static readonly Regex LevelPattern = new Regex(
                @"^\s*Me\.Level\s*([<>=!]+)\s*(\d+)\s*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            // Pattern for HasQuest(N)
            private static readonly Regex HasQuestPattern = new Regex(
                @"^\s*\(?\s*HasQuest\s*\(\s*(\d+)\s*\)\s*\)?\s*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            // Pattern for IsQuestCompleted(N)
            private static readonly Regex IsQuestCompletedPattern = new Regex(
                @"^\s*\(?\s*IsQuestCompleted\s*\(\s*(\d+)\s*\)\s*\)?\s*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            // Pattern for !HasQuest(N)
            private static readonly Regex NotHasQuestPattern = new Regex(
                @"^\s*\(?\s*!HasQuest\s*\(\s*(\d+)\s*\)\s*\)?\s*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            // Pattern for !IsQuestCompleted(N)
            private static readonly Regex NotIsQuestCompletedPattern = new Regex(
                @"^\s*\(?\s*!IsQuestCompleted\s*\(\s*(\d+)\s*\)\s*\)?\s*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            // Pattern for (Me.Class == WoWClass.XXX) && (Me.Race == WoWRace.YYY)
            private static readonly Regex ClassAndRacePattern = new Regex(
                @"^\s*\(\s*Me\.Class\s*==\s*WoWClass\.(\w+)\s*\)\s*&&\s*\(\s*Me\.Race\s*==\s*WoWRace\.(\w+)\s*\)\s*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            // Pattern for !((Me.Class == WoWClass.XXX) && (Me.Race == WoWRace.YYY))
            private static readonly Regex NotClassAndRacePattern = new Regex(
                @"^\s*!\s*\(\s*\(\s*Me\.Class\s*==\s*WoWClass\.(\w+)\s*\)\s*&&\s*\(\s*Me\.Race\s*==\s*WoWRace\.(\w+)\s*\)\s*\)\s*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            // Pattern for ((!HasQuest(N)) && (!IsQuestCompleted(N)))
            private static readonly Regex NotHasQuestAndNotCompletedPattern = new Regex(
                @"^\s*\(\s*\(\s*!HasQuest\s*\(\s*(\d+)\s*\)\s*\)\s*&&\s*\(\s*!IsQuestCompleted\s*\(\s*(\d+)\s*\)\s*\)\s*\)\s*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            // Pattern for ((HasQuest(N)) && (!IsQuestCompleted(N)))
            private static readonly Regex HasQuestAndNotCompletedPattern = new Regex(
                @"^\s*\(\s*\(\s*HasQuest\s*\(\s*(\d+)\s*\)\s*\)\s*&&\s*\(\s*!IsQuestCompleted\s*\(\s*(\d+)\s*\)\s*\)\s*\)\s*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            // Pattern for Me.Class == WoWClass.XXX (simple, no parens)
            private static readonly Regex SimpleClassPattern = new Regex(
                @"^\s*Me\.Class\s*==\s*WoWClass\.(\w+)\s*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            public static Func<bool> TryParse(string expression)
            {
                if (string.IsNullOrWhiteSpace(expression))
                    return null;
                
                // Try simple bool first (like HB)
                if (bool.TryParse(expression.Trim(), out bool boolValue))
                    return () => boolValue;
                
                Match match;
                
                // Try (Me.Class == WoWClass.XXX) && (Me.Race == WoWRace.YYY)
                match = ClassAndRacePattern.Match(expression);
                if (match.Success)
                {
                    if (Enum.TryParse<WoWClass>(match.Groups[1].Value, true, out var wowClass) &&
                        Enum.TryParse<WoWRace>(match.Groups[2].Value, true, out var wowRace))
                    {
                        return () => Styx.WoWInternals.ObjectManager.Me?.Class == wowClass &&
                                    Styx.WoWInternals.ObjectManager.Me?.Race == wowRace;
                    }
                }
                
                // Try !((Me.Class == WoWClass.XXX) && (Me.Race == WoWRace.YYY))
                match = NotClassAndRacePattern.Match(expression);
                if (match.Success)
                {
                    if (Enum.TryParse<WoWClass>(match.Groups[1].Value, true, out var wowClass) &&
                        Enum.TryParse<WoWRace>(match.Groups[2].Value, true, out var wowRace))
                    {
                        return () => !(Styx.WoWInternals.ObjectManager.Me?.Class == wowClass &&
                                      Styx.WoWInternals.ObjectManager.Me?.Race == wowRace);
                    }
                }
                
                // Try ((!HasQuest(N)) && (!IsQuestCompleted(N)))
                match = NotHasQuestAndNotCompletedPattern.Match(expression);
                if (match.Success)
                {
                    if (int.TryParse(match.Groups[1].Value, out int questId1) &&
                        int.TryParse(match.Groups[2].Value, out int questId2) &&
                        questId1 == questId2)
                    {
                        return () => !ProfileHelperFunctions.HasQuest((uint)questId1) && 
                                    !ProfileHelperFunctions.IsQuestCompleted((uint)questId1);
                    }
                }
                
                // Try ((HasQuest(N)) && (!IsQuestCompleted(N)))
                match = HasQuestAndNotCompletedPattern.Match(expression);
                if (match.Success)
                {
                    if (int.TryParse(match.Groups[1].Value, out int questId1) &&
                        int.TryParse(match.Groups[2].Value, out int questId2) &&
                        questId1 == questId2)
                    {
                        return () => ProfileHelperFunctions.HasQuest((uint)questId1) && 
                                    !ProfileHelperFunctions.IsQuestCompleted((uint)questId1);
                    }
                }
                
                // Try Me.Class == WoWClass.XXX (with optional parens)
                match = ClassPattern.Match(expression);
                if (match.Success)
                {
                    if (Enum.TryParse<WoWClass>(match.Groups[1].Value, true, out var wowClass))
                    {
                        return () => Styx.WoWInternals.ObjectManager.Me?.Class == wowClass;
                    }
                }
                
                // Try simple class without parens
                match = SimpleClassPattern.Match(expression);
                if (match.Success)
                {
                    if (Enum.TryParse<WoWClass>(match.Groups[1].Value, true, out var wowClass))
                    {
                        return () => Styx.WoWInternals.ObjectManager.Me?.Class == wowClass;
                    }
                }
                
                // Try Me.Race == WoWRace.XXX
                match = RacePattern.Match(expression);
                if (match.Success)
                {
                    if (Enum.TryParse<WoWRace>(match.Groups[1].Value, true, out var wowRace))
                    {
                        return () => Styx.WoWInternals.ObjectManager.Me?.Race == wowRace;
                    }
                }
                
                // Try Me.Level comparisons
                match = LevelPattern.Match(expression);
                if (match.Success)
                {
                    string op = match.Groups[1].Value;
                    if (int.TryParse(match.Groups[2].Value, out int level))
                    {
                        return op switch
                        {
                            "<" => () => (Styx.WoWInternals.ObjectManager.Me?.Level ?? 0) < level,
                            ">" => () => (Styx.WoWInternals.ObjectManager.Me?.Level ?? 0) > level,
                            "<=" => () => (Styx.WoWInternals.ObjectManager.Me?.Level ?? 0) <= level,
                            ">=" => () => (Styx.WoWInternals.ObjectManager.Me?.Level ?? 0) >= level,
                            "==" => () => (Styx.WoWInternals.ObjectManager.Me?.Level ?? 0) == level,
                            "!=" => () => (Styx.WoWInternals.ObjectManager.Me?.Level ?? 0) != level,
                            _ => null
                        };
                    }
                }
                
                // Try HasQuest(N)
                match = HasQuestPattern.Match(expression);
                if (match.Success)
                {
                    if (int.TryParse(match.Groups[1].Value, out int questId))
                    {
                        return () => ProfileHelperFunctions.HasQuest((uint)questId);
                    }
                }
                
                // Try !HasQuest(N)
                match = NotHasQuestPattern.Match(expression);
                if (match.Success)
                {
                    if (int.TryParse(match.Groups[1].Value, out int questId))
                    {
                        return () => !ProfileHelperFunctions.HasQuest((uint)questId);
                    }
                }
                
                // Try IsQuestCompleted(N)
                match = IsQuestCompletedPattern.Match(expression);
                if (match.Success)
                {
                    if (int.TryParse(match.Groups[1].Value, out int questId))
                    {
                        return () => ProfileHelperFunctions.IsQuestCompleted((uint)questId);
                    }
                }
                
                // Try !IsQuestCompleted(N)
                match = NotIsQuestCompletedPattern.Match(expression);
                if (match.Success)
                {
                    if (int.TryParse(match.Groups[1].Value, out int questId))
                    {
                        return () => !ProfileHelperFunctions.IsQuestCompleted((uint)questId);
                    }
                }
                
                // Cannot fast-path parse, return null to fall back to Roslyn
                return null;
            }
        }

        /// <summary>
        /// Expression binder - same pattern as HB 6.2.3 ExpressionBinder
        /// </summary>
        private class ExpressionBinder
        {
            private static readonly Dictionary<string, Func<bool>> _cachedMethods = new Dictionary<string, Func<bool>>();
            private static readonly object _parserLock = new object();
            private readonly string _expression;
            private Func<bool> _method;
            private bool _methodAssigned;
            private readonly int? _line;

            public ExpressionBinder(string expression, int? line = null)
            {
                _expression = expression;
                _line = line;
                if (StyxSettings.Instance.ProfileDebuggingMode)
                {
                    _method = Compile(_expression, _line);
                    _methodAssigned = true;
                    if (_method == null)
                        throw new Styx.CantCompileException();
                }
            }

            public bool Evaluate()
            {
                if (!_methodAssigned)
                {
                    _method = Compile(_expression, _line);
                    _methodAssigned = true;
                }
                return _method != null && _method();
            }

            private static Func<bool> Compile(string expression, int? line)
            {
                // Check cache first (outside lock for performance)
                if (_cachedMethods.TryGetValue(expression, out Func<bool> func))
                    return func;

                bool lockTaken = false;
                try
                {
                    Monitor.Enter(_parserLock, ref lockTaken);
                    
                    // Double-check after acquiring lock
                    if (_cachedMethods.TryGetValue(expression, out func))
                        return func;
                    
                    // Try simple bool parse first (like HB)
                    if (bool.TryParse(expression, out bool value))
                    {
                        func = () => value;
                        _cachedMethods[expression] = func;
                        return func;
                    }
                    
                    if (line.HasValue)
                        Logging.WriteDebug("Compiling expression '{0}' @ line {1}", expression, line.Value);
                    else
                        Logging.WriteDebug("Compiling expression '{0}'", expression);
                    
                    func = FastPathParser.TryParse(expression);
                    if (func != null)
                    {
                        _cachedMethods[expression] = func;
                        return func;
                    }
                    
                    // Fall back to Roslyn compilation
                    var helper = new ConditionHelper(expression);
                    if (helper.CompileAndBindExpression(out string[] errors, out Func<bool> bound))
                    {
                        _cachedMethods[expression] = bound;
                        return bound;
                    }
                    else
                    {
                        if (errors != null && errors.Length > 0)
                        {
                            Logging.WriteDebug("{0} errors encountered while compiling condition '{1}'", errors.Length, expression);
                            foreach (var error in errors)
                                Logging.WriteDebug(error);
                        }
                        _cachedMethods[expression] = null;
                        return null;
                    }
                }
                finally
                {
                    if (lockTaken)
                        Monitor.Exit(_parserLock);
                }
            }
        }
    }
    
    /// <summary>
    /// Profile helper functions for quest conditions - same as HB
    /// </summary>
    public static class ProfileHelperFunctions
    {
        public static bool HasQuest(uint questId)
        {
            var me = Styx.WoWInternals.ObjectManager.Me;
            if (me == null) return false;
            // Check QuestLog first
            var questLog = me.QuestLog;
            if (questLog != null)
            {
                int index = questLog.GetIndexForQuest(questId);
                if (index >= 0) return true;
            }
            return false;
        }
        
        public static bool IsQuestCompleted(uint questId)
        {
            var me = Styx.WoWInternals.ObjectManager.Me;
            if (me == null) return false;
            // Check if quest is in log with completed objectives (State B)
            // This matches HB 4.3.4's ProfileHelperFunctionsBase.IsQuestCompleted()
            PlayerQuest questById = me.QuestLog?.GetQuestById(questId);
            if (questById != null) return questById.IsCompleted;
            // Fall back to completed quests list (State C - already turned in)
            var completedQuests = me.QuestLog?.GetCompletedQuests();
            return completedQuests?.Contains(questId) ?? false;
        }
    }
    
    /// <summary>
    /// Globals object for Roslyn script evaluation.
    /// Provides Me property (like ProfileHelperFunctionsBase in HB)
    /// </summary>
    public class ConditionGlobals
    {
        /// <summary>
        /// The local player - equivalent to StyxWoW.Me / ObjectManager.Me
        /// </summary>
        public Styx.WoWInternals.WoWObjects.LocalPlayer Me => Styx.WoWInternals.ObjectManager.Me;
        
        /// <summary>
        /// Check if player has quest in their log
        /// </summary>
        public bool HasQuest(uint questId) => ProfileHelperFunctions.HasQuest(questId);
        
        /// <summary>
        /// Check if player has completed quest
        /// </summary>
        public bool IsQuestCompleted(uint questId) => ProfileHelperFunctions.IsQuestCompleted(questId);
    }
}
