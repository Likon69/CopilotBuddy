// QuestBehaviorHelper - Compiles Quest Behaviors from .cs files using Roslyn
// Ported from HonorBuddy 4.3.4, updated for .NET modern with Microsoft.CodeAnalysis

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Styx.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

#nullable disable
namespace Styx.Logic.Profiles.Quest;

/// <summary>
/// Compiles Quest Behaviors from C# source files at runtime using Roslyn.
/// Quest Behaviors are placed in the "Quest Behaviors" folder next to the executable.
/// Supports both single .cs files and folders containing multiple .cs files.
/// 
/// Usage in profile XML:
/// <CustomBehavior File="Message" Text="Hello World" LogColor="Green" />
/// <CustomBehavior File="InteractWith" MobId="12345" X="100" Y="200" Z="50" />
/// </summary>
public class QuestBehaviorHelper
{
    // HB 4.3.4 pattern: Dictionary + lock for compilation caching.
    private static readonly Dictionary<string, Assembly> _assemblyCache =
        new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);
    private static readonly object _lockObject = new object();
    private static readonly List<MetadataReference> _defaultReferences;
    
    private Assembly _cachedAssembly;
    private bool _isCompiled;

    /// <summary>
    /// Static constructor to initialize default assembly references for compilation.
    /// </summary>
    static QuestBehaviorHelper()
    {
        _defaultReferences = new List<MetadataReference>();
        
        // Add references to all assemblies currently loaded that Quest Behaviors might need
        var referencePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        // Core .NET assemblies - use assembly from instance of type (not static types)
        AddReferenceFromType<object>(referencePaths);           // System.Runtime / mscorlib
        AddReferenceFromType<string>(referencePaths);           // System.Runtime
        AddReferenceFromType<List<int>>(referencePaths);        // System.Collections
        AddReferenceFromType<Color>(referencePaths);            // System.Drawing
        AddReferenceFromType<System.Xml.XmlNode>(referencePaths); // System.Xml
        // Ensure LINQ/XDocument types are explicitly referenced so forwarded XElement resolves during compilation
        AddReferenceFromType<System.Xml.Linq.XDocument>(referencePaths);
        AddReferenceFromType<System.Xml.Linq.XElement>(referencePaths);
        
        // Add System.Runtime for fundamental types
        var runtimePath = System.IO.Path.GetDirectoryName(typeof(object).Assembly.Location);
        if (runtimePath != null)
        {
            var systemRuntime = System.IO.Path.Combine(runtimePath, "System.Runtime.dll");
            if (File.Exists(systemRuntime) && referencePaths.Add(systemRuntime))
                _defaultReferences.Add(MetadataReference.CreateFromFile(systemRuntime));
            
            var systemCollections = System.IO.Path.Combine(runtimePath, "System.Collections.dll");
            if (File.Exists(systemCollections) && referencePaths.Add(systemCollections))
                _defaultReferences.Add(MetadataReference.CreateFromFile(systemCollections));
            
            var systemLinq = System.IO.Path.Combine(runtimePath, "System.Linq.dll");
            if (File.Exists(systemLinq) && referencePaths.Add(systemLinq))
                _defaultReferences.Add(MetadataReference.CreateFromFile(systemLinq));

            var systemXmlXDocument = System.IO.Path.Combine(runtimePath, "System.Xml.XDocument.dll");
            if (File.Exists(systemXmlXDocument) && referencePaths.Add(systemXmlXDocument))
                _defaultReferences.Add(MetadataReference.CreateFromFile(systemXmlXDocument));
            
            var systemConsole = System.IO.Path.Combine(runtimePath, "System.Console.dll");
            if (File.Exists(systemConsole) && referencePaths.Add(systemConsole))
                _defaultReferences.Add(MetadataReference.CreateFromFile(systemConsole));
                
            var netstandard = System.IO.Path.Combine(runtimePath, "netstandard.dll");
            if (File.Exists(netstandard) && referencePaths.Add(netstandard))
                _defaultReferences.Add(MetadataReference.CreateFromFile(netstandard));
            
            // System.Linq.Expressions - Required for Expression<T> used in some Quest Behaviors
            var linqExpressions = System.IO.Path.Combine(runtimePath, "System.Linq.Expressions.dll");
            if (File.Exists(linqExpressions) && referencePaths.Add(linqExpressions))
                _defaultReferences.Add(MetadataReference.CreateFromFile(linqExpressions));
            
            // System.ObjectModel - Required for ObservableCollection and other types
            var objectModel = System.IO.Path.Combine(runtimePath, "System.ObjectModel.dll");
            if (File.Exists(objectModel) && referencePaths.Add(objectModel))
                _defaultReferences.Add(MetadataReference.CreateFromFile(objectModel));
        }
        
        // Add WPF assemblies from TRUSTED_PLATFORM_ASSEMBLIES (same runtime as CopilotBuddy)
        var trustedAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
        if (!string.IsNullOrEmpty(trustedAssemblies))
        {
            foreach (var asmPath in trustedAssemblies.Split(';'))
            {
                var fileName = System.IO.Path.GetFileName(asmPath);
                if (fileName.Equals("PresentationCore.dll", StringComparison.OrdinalIgnoreCase) ||
                    fileName.Equals("WindowsBase.dll", StringComparison.OrdinalIgnoreCase) ||
                    fileName.Equals("PresentationFramework.dll", StringComparison.OrdinalIgnoreCase) ||
                    fileName.Equals("System.Xaml.dll", StringComparison.OrdinalIgnoreCase))
                {
                    if (File.Exists(asmPath) && referencePaths.Add(asmPath))
                        _defaultReferences.Add(MetadataReference.CreateFromFile(asmPath));
                }
            }
        }
        
        // Add CopilotBuddy itself (contains Styx.*, TreeSharp, etc.)
        var executingAsm = Assembly.GetExecutingAssembly();
        if (!string.IsNullOrEmpty(executingAsm.Location) && referencePaths.Add(executingAsm.Location))
            _defaultReferences.Add(MetadataReference.CreateFromFile(executingAsm.Location));
        
        // Add entry assembly if different
        var entryAsm = Assembly.GetEntryAssembly();
        if (entryAsm != null && !string.IsNullOrEmpty(entryAsm.Location) && referencePaths.Add(entryAsm.Location))
            _defaultReferences.Add(MetadataReference.CreateFromFile(entryAsm.Location));
            
        Logging.WriteDebug("[QuestBehaviorHelper] Initialized with {0} assembly references", _defaultReferences.Count);
    }
    
    private static void AddReferenceFromType<T>(HashSet<string> paths)
    {
        try
        {
            var asm = typeof(T).Assembly;
            if (!string.IsNullOrEmpty(asm.Location) && File.Exists(asm.Location) && paths.Add(asm.Location))
                _defaultReferences.Add(MetadataReference.CreateFromFile(asm.Location));
        }
        catch { /* Ignore assemblies without file location */ }
    }

    /// <summary>
    /// Creates a new QuestBehaviorHelper for the specified path.
    /// </summary>
    /// <param name="path">Path to the .cs file or folder containing .cs files.</param>
    public QuestBehaviorHelper(string path)
    {
        this.Path = path;
        
        // If in debug mode, compile immediately to catch errors early
        if (StyxSettings.Instance.ProfileDebuggingMode)
        {
            this._cachedAssembly = this.CompileAssembly();
            this._isCompiled = true;
        }
    }

    /// <summary>
    /// Path to the Quest Behavior source file or folder.
    /// </summary>
    public string Path { get; private set; }

    /// <summary>
    /// Compiles the Quest Behavior(s) using Roslyn and returns the resulting assembly.
    /// Results are cached by path to avoid recompilation.
    /// </summary>
    private Assembly CompileAssembly()
    {
        string cacheKey = this.Path.ToLowerInvariant();

        // HB 4.3.4 pattern: TryGetValue outside lock, compile inside lock.
        if (_assemblyCache.TryGetValue(cacheKey, out Assembly cached))
            return cached;

        lock (_lockObject)
        {
            // Double-check after acquiring lock.
            if (_assemblyCache.TryGetValue(cacheKey, out cached))
                return cached;

            Assembly compiled = DoCompile();
            _assemblyCache[cacheKey] = compiled;
            return compiled;
        }
    }

    private Assembly DoCompile()
    {
        Assembly compiledAssembly = null;
        
        try
        {
            Logging.Write(Color.Cyan, "[QuestBehavior] Compiling: {0}", this.Path);
            
            // Collect source files
            string[] sourceFiles;
            if (Directory.Exists(this.Path))
            {
                sourceFiles = Directory.GetFiles(this.Path, "*.cs", SearchOption.AllDirectories);
                Logging.WriteDebug("[QuestBehavior] Found {0} source files in folder", sourceFiles.Length);
            }
            else if (File.Exists(this.Path))
            {
                sourceFiles = new[] { this.Path };
            }
            else
            {
                Logging.Write(Color.Red, "[QuestBehavior] ERROR: Path not found: {0}", this.Path);
                return null;
            }
            
            if (sourceFiles.Length == 0)
            {
                Logging.Write(Color.Red, "[QuestBehavior] ERROR: No .cs files found in {0}", this.Path);
                return null;
            }
            
            // Parse all source files into syntax trees
            var syntaxTrees = new List<SyntaxTree>();
            var encoding = Encoding.UTF8;
            
            foreach (string file in sourceFiles)
            {
                try
                {
                    string sourceCode = File.ReadAllText(file, encoding);
                    var parseOptions = CSharpParseOptions.Default
                        .WithLanguageVersion(LanguageVersion.Latest);
                    
                    var syntaxTree = CSharpSyntaxTree.ParseText(
                        sourceCode,
                        parseOptions,
                        path: file,
                        encoding: encoding);
                    
                    syntaxTrees.Add(syntaxTree);
                }
                catch (Exception ex)
                {
                    Logging.Write(Color.Red, "[QuestBehavior] ERROR reading file {0}: {1}", file, ex.Message);
                }
            }
            
            if (syntaxTrees.Count == 0)
            {
                Logging.Write(Color.Red, "[QuestBehavior] ERROR: No valid source files parsed");
                return null;
            }
            
            // Create compilation
            string assemblyName = "QuestBehavior_" + System.IO.Path.GetFileNameWithoutExtension(this.Path) + "_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            
            var compilationOptions = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                allowUnsafe: true,
                warningLevel: 0,  // Suppress warnings
                nullableContextOptions: NullableContextOptions.Disable);
            
            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees,
                _defaultReferences,
                compilationOptions);
            
            // Emit to memory
            using (var ms = new MemoryStream())
            {
                EmitResult emitResult = compilation.Emit(ms);
                
                if (!emitResult.Success)
                {
                    // Log compilation errors
                    var errors = emitResult.Diagnostics
                        .Where(d => d.Severity == DiagnosticSeverity.Error)
                        .ToList();
                    
                    Logging.Write(Color.Red, "[QuestBehavior] COMPILATION FAILED: {0} errors", errors.Count);
                    
                    foreach (var error in errors.Take(10)) // Show first 10 errors
                    {
                        var lineSpan = error.Location.GetMappedLineSpan();
                        Logging.Write(Color.Red, "  [{0}] Line {1}: {2}",
                            System.IO.Path.GetFileName(lineSpan.Path ?? "?"),
                            lineSpan.StartLinePosition.Line + 1,
                            error.GetMessage());
                    }
                    
                    if (errors.Count > 10)
                        Logging.Write(Color.Red, "  ... and {0} more errors", errors.Count - 10);
                    
                    return null;
                }
                
                // Load the compiled assembly
                ms.Seek(0, SeekOrigin.Begin);
                compiledAssembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                
                Logging.Write(Color.Green, "[QuestBehavior] Successfully compiled: {0}", System.IO.Path.GetFileName(this.Path));
            }
        }
        catch (Exception ex)
        {
            Logging.Write(Color.Red, "[QuestBehavior] EXCEPTION during compilation: {0}", ex.Message);
            Logging.WriteException(ex);
        }

        return compiledAssembly;
    }

    /// <summary>
    /// Gets the compiled assembly, compiling if necessary.
    /// </summary>
    public Assembly GetAssembly()
    {
        if (!this._isCompiled)
        {
            this._cachedAssembly = this.CompileAssembly();
            this._isCompiled = true;
        }
        return this._cachedAssembly;
    }

    /// <summary>
    /// Creates a function that returns the compiled assembly for the specified behavior.
    /// This is the main entry point used by CodeNode.FromXml().
    /// </summary>
    /// <param name="behaviorName">Name of the behavior (e.g., "Message", "InteractWith", "Escort")</param>
    /// <returns>A function that returns the compiled assembly when called.</returns>
    public static Func<Assembly> GetAssemblyCompiler(string behaviorName)
    {
        // Build path to the Quest Behavior
        string questBehaviorsRoot = System.IO.Path.Combine(Logging.ApplicationPath, "Quest Behaviors");
        string fullPath = System.IO.Path.Combine(questBehaviorsRoot, behaviorName);
        
        // Check if it's a folder (multi-file behavior like "DeathknightStart")
        if (Directory.Exists(fullPath))
        {
            Logging.WriteDebug("[QuestBehavior] Found folder behavior: {0}", fullPath);
            return new Func<Assembly>(new QuestBehaviorHelper(fullPath).GetAssembly);
        }
        
        // Check if it's a single .cs file
        string csPath = System.IO.Path.ChangeExtension(fullPath, ".cs");
        if (File.Exists(csPath))
        {
            Logging.WriteDebug("[QuestBehavior] Found file behavior: {0}", csPath);
            return new Func<Assembly>(new QuestBehaviorHelper(csPath).GetAssembly);
        }
        
        // Not found - log error
        Logging.Write(Color.Red, "[QuestBehavior] ERROR: Behavior not found: {0}", behaviorName);
        Logging.Write(Color.Red, "[QuestBehavior] Searched in: {0}", questBehaviorsRoot);
        
        // Return a function that returns null (will cause error later with clear message)
        return () => null;
    }
    
    /// <summary>
    /// Clears the assembly cache, forcing recompilation of all behaviors.
    /// Useful for development/debugging.
    /// </summary>
    public static void ClearCache()
    {
        lock (_lockObject)
        {
            _assemblyCache.Clear();
        }
        Logging.Write("[QuestBehavior] Assembly cache cleared");
    }
}
