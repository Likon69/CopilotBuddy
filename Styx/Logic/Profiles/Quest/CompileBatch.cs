using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Styx.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Styx.Logic.Profiles.Quest
{
    /// <summary>
    /// Batches C# expression strings from quest behaviors and compiles them together into a single
    /// assembly whose class inherits ProfileHelperFunctionsBase.
    /// Ported from HB 6.2.3 Styx.CommonBot.Profiles.Quest.Order.CompileBatch, using Roslyn instead of CodeDom.
    /// </summary>
    public class CompileBatch
    {
        private static int _batchCounter;

        private readonly List<BatchEntry> _entries = new List<BatchEntry>();
        private readonly HashSet<string> _imports;
        private int _expressionCounter;

        public CompileBatch()
        {
            Errors = new CompileError[0];
            _imports = new HashSet<string>
            {
                "System",
                "System.Collections.Generic",
                "System.Diagnostics",
                "System.IO",
                "System.Linq",
                "System.Linq.Expressions",
                "System.Threading",
                "System.Threading.Tasks",
                "System.Windows.Media",
                "Styx",
                "Styx.Common",
                "Styx.Helpers",
                "Styx.Logic",
                "Styx.Logic.Combat",
                "Styx.Logic.Pathing",
                "Styx.Logic.Profiles",
                "Styx.Logic.Profiles.Quest",
                "Styx.Logic.Questing",
                "Styx.WoWInternals",
                "Styx.WoWInternals.WoWObjects",
                "Styx.Logic.BehaviorTree",
                "Buddy.Coroutines",
                "TreeSharp",
                // The rest of the namespaces a profile condition could name under the old
                // per-condition ConditionHelper script, kept so batching does not shrink the
                // vocabulary. HB 6.2.3 CompileBatch carries the same set under its own names.
                "System.Drawing",
                "Styx.Combat.CombatRoutine",
                "Styx.Logic.AreaManagement",
                "Styx.Logic.Inventory.Frames.Gossip",
                "Styx.Logic.Inventory.Frames.LootFrame",
                "Styx.Logic.POI",
                "Styx.Plugins",
                "Styx.WoWInternals.World",
            };
        }

        public bool IsCompiled { get; private set; }
        public bool HasErrors => Errors.Any();
        public bool HasPendingExpressions => _expressionCounter > 0;
        public CompileError[] Errors { get; private set; }
        public IReadOnlyList<string> Imports => _imports.ToList().AsReadOnly();

        /// <summary>
        /// Returns all raw code strings registered via Add() (Type="Definition" entries).
        /// Used by ProfileBatchManager to carry definitions forward into new batches.
        /// </summary>
        public IEnumerable<string> GetDefinitionCode()
            => _entries.OfType<CodeEntry>().Select(e => e.Code);

        public void AddImport(string import)
        {
            if (IsCompiled)
                throw new InvalidOperationException("Cannot add an import to an already compiled batch");
            _imports.Add(import);
        }

        /// <summary>
        /// Registers a raw code string (Type="Definition" from RunCode) as a class-body member.
        /// </summary>
        public void Add(string code, object context = null)
        {
            if (IsCompiled)
                throw new InvalidOperationException("Cannot add code to an already compiled batch");
            _entries.Add(new CodeEntry(code, context));
        }

        /// <summary>
        /// Registers a DelayCompiledExpression with the batch.
        /// The expression's CompiledExpression field is populated after Compile() succeeds.
        /// </summary>
        public Delegate AddExpression(DelayCompiledExpression expression, object context = null)
        {
            if (!IsSupportedDelegateType(expression.DelegateType))
                throw new NotSupportedException("Only Action/Func delegate types are supported.");
            if (IsCompiled)
                throw new InvalidOperationException("Cannot add an expression to an already compiled batch");

            string funcName = "__ExpressionFunc__" + _expressionCounter++;
            var entry = new ExpressionEntry(expression, funcName, context);
            _entries.Add(entry);
            return entry.Expression.CallableExpression;
        }

        /// <summary>
        /// Compiles all registered code and expressions into a single assembly.
        /// Sets CompiledExpression on each registered DelayCompiledExpression.
        /// </summary>
        public bool Compile()
        {
            if (IsCompiled)
                throw new InvalidOperationException("CompileBatch can only be compiled once");
            IsCompiled = true;

            if (!_entries.Any())
                return true;

            string namespaceName = "__CompileBatchNamespace" + _batchCounter++ + "__";
            string className = namespaceName + ".__CompiledBatchClass__";
            string source = BuildSource(namespaceName);

            Logging.WriteDebug("[CompileBatch] Compiling {0} entries...", _entries.Count);

            // Collect references from all currently loaded assemblies
            var references = new List<MetadataReference>();
            var seenPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (!asm.IsDynamic && !string.IsNullOrEmpty(asm.Location) && seenPaths.Add(asm.Location))
                        references.Add(MetadataReference.CreateFromFile(asm.Location));
                }
                catch { /* dynamic or reflection-only, skip */ }
            }

            // Ensure core runtime refs are present
            string runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
            if (runtimeDir != null)
            {
                foreach (string name in new[] { "System.Runtime.dll", "System.Collections.dll",
                    "System.Linq.dll", "System.Linq.Expressions.dll", "netstandard.dll",
                    "System.Threading.Tasks.dll" })
                {
                    string path = Path.Combine(runtimeDir, name);
                    if (File.Exists(path) && seenPaths.Add(path))
                        references.Add(MetadataReference.CreateFromFile(path));
                }
            }

            var syntaxTree = CSharpSyntaxTree.ParseText(source,
                new CSharpParseOptions(LanguageVersion.Latest));

            var compilation = CSharpCompilation.Create(
                "CompileBatch_" + namespaceName,
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOptimizationLevel(OptimizationLevel.Release)
                    .WithNullableContextOptions(NullableContextOptions.Disable));

            using (var ms = new MemoryStream())
            {
                var emitResult = compilation.Emit(ms);
                if (!emitResult.Success)
                {
                    var errors = emitResult.Diagnostics
                        .Where(d => d.Severity == DiagnosticSeverity.Error)
                        .ToList();
                    Errors = BuildErrors(errors);
                    foreach (var err in Errors)
                        Logging.Write(Color.Red, "[CompileBatch] Error (line {0}): {1}", err.Line, err.Error);
                    return false;
                }

                ms.Seek(0, SeekOrigin.Begin);
                Assembly compiled = AssemblyLoadContext.Default.LoadFromStream(ms);
                object instance = Activator.CreateInstance(compiled.GetType(className));

                // Wire up compiled delegates to each registered expression
                foreach (ExpressionEntry entry in _entries.OfType<ExpressionEntry>())
                {
                    MethodInfo method = instance.GetType().GetMethod(entry.FuncName,
                        BindingFlags.Public | BindingFlags.Instance);
                    entry.Expression.CompiledExpression = Delegate.CreateDelegate(
                        entry.Expression.DelegateType, instance, method);
                }

                Errors = new CompileError[0];
                Logging.WriteDebug("[CompileBatch] Compiled successfully.");
                return true;
            }
        }

        private string BuildSource(string namespaceName)
        {
            var sb = new StringBuilder();
            foreach (string ns in _imports)
                sb.AppendFormat("using {0};\n", ns);

            sb.AppendFormat("namespace {0}\n{{\n", namespaceName);
            sb.Append("internal class __CompiledBatchClass__ : Styx.Logic.Profiles.Quest.ProfileHelperFunctionsBase\n{\n");

            int currentLine = sb.ToString().Count(c => c == '\n') + 1;
            foreach (BatchEntry entry in _entries)
            {
                entry.StartLine = currentLine;
                if (entry is ExpressionEntry expr)
                {
                    string methodBody = BuildExpressionMethod(expr);
                    sb.Append(methodBody);
                    currentLine += methodBody.Count(c => c == '\n') + 1;
                }
                else
                {
                    sb.AppendLine(entry.Code);
                    currentLine += entry.LineCount + 1;
                }
            }

            sb.Append("}\n}\n");
            return sb.ToString();
        }

        private static string BuildExpressionMethod(ExpressionEntry entry)
        {
            MethodInfo invoke = entry.Expression.DelegateType.GetMethod("Invoke");
            var sb = new StringBuilder();
            var paramNames = new List<string>();
            ParameterInfo[] parameters = invoke.GetParameters();

            sb.AppendFormat("public {0} {1}(", GetTypeName(invoke.ReturnType), entry.FuncName);
            for (int i = 0; i < parameters.Length; i++)
            {
                string argName = "__arg" + i + "__";
                paramNames.Add(argName);
                sb.Append(GetTypeName(parameters[i].ParameterType));
                sb.Append(" ");
                sb.Append(argName);
                if (i < parameters.Length - 1)
                    sb.Append(",");
            }
            sb.AppendLine(")");
            sb.AppendLine("{");
            if (invoke.ReturnType != typeof(void))
                sb.Append("return ");
            sb.AppendFormat("new {0}({1})({2});",
                GetTypeName(entry.Expression.DelegateType),
                entry.Expression.ExpressionString,
                string.Join(",", paramNames));
            sb.AppendLine();
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GetTypeName(Type type)
        {
            if (type == typeof(void)) return "void";
            if (!type.IsGenericType) return type.FullName;

            Type def = type.GetGenericTypeDefinition();
            string baseName = def.FullName.Remove(def.FullName.IndexOf('`'));
            var sb = new StringBuilder(baseName);
            sb.Append('<');
            Type[] args = type.GetGenericArguments();
            for (int i = 0; i < args.Length; i++)
            {
                sb.Append(GetTypeName(args[i]));
                if (i < args.Length - 1)
                    sb.Append(',');
            }
            sb.Append('>');
            return sb.ToString();
        }

        private CompileError[] BuildErrors(List<Diagnostic> diagnostics)
        {
            var result = new CompileError[diagnostics.Count];
            for (int i = 0; i < diagnostics.Count; i++)
            {
                var diag = diagnostics[i];
                var span = diag.Location.GetMappedLineSpan();
                int line = span.StartLinePosition.Line + 1;
                // Find the entry that owns this line
                BatchEntry owner = _entries.LastOrDefault(e => line >= e.StartLine) ?? _entries.FirstOrDefault();
                result[i] = new CompileError(
                    owner?.Code ?? "",
                    diag.GetMessage(),
                    owner != null ? line - owner.StartLine : line,
                    owner?.Context);
            }
            return result;
        }

        private static bool IsSupportedDelegateType(Type type)
        {
            if (type == typeof(Action)) return true;
            if (!type.IsGenericType) return false;
            Type def = type.GetGenericTypeDefinition();
            return def == typeof(Action<>) || def == typeof(Action<,>) ||
                   def == typeof(Action<,,>) || def == typeof(Action<,,,>) ||
                   def == typeof(Func<>) || def == typeof(Func<,>) ||
                   def == typeof(Func<,,>) || def == typeof(Func<,,,>) ||
                   def == typeof(Func<,,,,>);
        }

        #region Entry types

        private abstract class BatchEntry
        {
            protected BatchEntry(string code, object context)
            {
                Code = code;
                Context = context;
                LineCount = code.Count(c => c == '\n') + 1;
            }

            public string Code { get; }
            public object Context { get; }
            public int LineCount { get; }
            public int StartLine { get; set; }
        }

        private class CodeEntry : BatchEntry
        {
            public CodeEntry(string code, object context) : base(code, context) { }
        }

        private class ExpressionEntry : BatchEntry
        {
            public ExpressionEntry(DelayCompiledExpression expression, string funcName, object context)
                : base(BuildCode(expression, funcName), context)
            {
                Expression = expression;
                FuncName = funcName;
            }

            public DelayCompiledExpression Expression { get; }
            public string FuncName { get; }

            private static string BuildCode(DelayCompiledExpression expression, string funcName)
            {
                // Placeholder — actual method body is built in BuildExpressionMethod.
                // This just counts lines for error mapping.
                return string.Format("/* expression: {0} */", funcName);
            }
        }

        #endregion
    }
}
