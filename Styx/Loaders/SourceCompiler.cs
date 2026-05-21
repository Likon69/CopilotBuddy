#nullable disable
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CSharp;

namespace Styx.Loaders
{
    /// <summary>
    /// Compiles C# source files at runtime.
    /// Ported from HB's Class52 (ns5).
    /// </summary>
    internal class SourceCompiler
    {
        private readonly long _timestamp;

        public SourceCompiler(string path)
        {
            _timestamp = DateTime.Now.Ticks;
            CompilerVersion = 3.5f;
            SourceFilePaths = new List<string>();

            if (File.Exists(path))
            {
                FileStructure = FileStructureType.SingleFile;
            }
            else if (Directory.Exists(path))
            {
                FileStructure = FileStructureType.Directory;
            }

            SourcePath = path;
            Options = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                IncludeDebugInformation = true,
                CompilerOptions = "/d:COPILOTBUDDY",
                TempFiles = new TempFileCollection(Path.GetTempPath()),
                OutputAssembly = Path.Combine(Path.GetTempPath(), AssemblyName)
            };
            CompiledToLocation = Options.OutputAssembly;

            // Add all currently loaded assemblies as references
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                AddReference(assembly.Location);
            }

            // Try to add common WPF / Windows Forms integration assemblies if available
            var wpfAssemblyNames = new[] {
                "PresentationCore",
                "PresentationFramework",
                "WindowsBase",
                "WindowsFormsIntegration",
                "System.Xaml",
                "System.Windows.Forms"
            };

            foreach (var name in wpfAssemblyNames)
            {
                try
                {
                    var asm = Assembly.Load(new AssemblyName(name));
                    if (asm != null && !string.IsNullOrEmpty(asm.Location))
                        AddReference(asm.Location);
                }
                catch
                {
                    // ignore if assembly not available
                }
            }
        }

        public Assembly CompiledAssembly { get; private set; }
        public string SourcePath { get; private set; }
        public FileStructureType FileStructure { get; private set; }
        public CompilerParameters Options { get; private set; }
        public float CompilerVersion { get; private set; }
        public string CompiledToLocation { get; private set; }
        public List<string> SourceFilePaths { get; private set; }

        public string AssemblyName
        {
            get
            {
                string name = FileStructure == FileStructureType.SingleFile
                    ? Path.GetFileNameWithoutExtension(SourcePath)
                    : new DirectoryInfo(SourcePath).Name;
                return $"{name}_{_timestamp}.dll";
            }
        }

        public void AddReference(string assembly)
        {
            if (!string.IsNullOrEmpty(assembly) && !Options.ReferencedAssemblies.Contains(assembly))
            {
                Options.ReferencedAssemblies.Add(assembly);
            }
        }

        public void AddEmbeddedResource(string path)
        {
            Options.EmbeddedResources.Add(path);
        }

        /// <summary>
        /// Collects source files from the path.
        /// </summary>
        private void CollectSourceFiles()
        {
            if (FileStructure == FileStructureType.Directory)
            {
                foreach (string file in Directory.GetFiles(SourcePath, "*.cs", SearchOption.AllDirectories))
                {
                    SourceFilePaths.Add(file);
                }
                foreach (string resx in Directory.GetFiles(SourcePath, ".resx", SearchOption.AllDirectories))
                {
                    AddEmbeddedResource(resx);
                }
            }
            else
            {
                SourceFilePaths.Add(SourcePath);
            }
        }

        /// <summary>
        /// Parses source files for compiler options.
        /// </summary>
        private void ParseCompilerOptions()
        {
            foreach (string filePath in SourceFilePaths)
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string trimmed = line.Trim();
                    if (trimmed.StartsWith("//!CompilerOption:"))
                    {
                        string[] parts = trimmed.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                        switch (parts[1])
                        {
                            case "AddRef":
                                if (parts.Length == 3 && !string.IsNullOrEmpty(parts[2]) && parts[2].EndsWith(".dll"))
                                {
                                    AddReference(parts[2]);
                                }
                                break;

                            case "Optimise":
                            case "Optimize":
                                if (parts.Length == 3 && !string.IsNullOrEmpty(parts[2]) && parts[2] == "On" 
                                    && !Options.CompilerOptions.Contains("/optimise"))
                                {
                                    Options.IncludeDebugInformation = false;
                                    Options.CompilerOptions += " /optimise";
                                }
                                break;

                            case "Version":
                                if (parts.Length == 3 && !string.IsNullOrEmpty(parts[2]) && parts[2] == "v4.0" 
                                    && FrameworkVersionDetection.DotNet4Installed)
                                {
                                    CompilerVersion = 4f;
                                }
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compiles the source files using Roslyn compiler.
        /// </summary>
        /// <returns>The compiler results.</returns>
        public CompilerResults Compile()
        {
            CollectSourceFiles();
            ParseCompilerOptions();

            if (SourceFilePaths.Count == 0)
            {
                return null;
            }

            // Add to trusted assemblies list
            AssemblyVerifier.TrustedAssemblies.Add(Path.GetFileNameWithoutExtension(AssemblyName));

            // Parse all source files into syntax trees
            var syntaxTrees = new List<SyntaxTree>();
            foreach (var sourceFile in SourceFilePaths)
            {
                try
                {
                    var code = File.ReadAllText(sourceFile);
                    var syntaxTree = CSharpSyntaxTree.ParseText(code,
                        path: sourceFile,
                        options: new CSharpParseOptions(LanguageVersion.Latest));
                    syntaxTrees.Add(syntaxTree);
                }
                catch (Exception ex)
                {
                    // Create error result
                    var errorResults = new CompilerResults(new TempFileCollection(Path.GetTempPath()));
                    errorResults.Errors.Add(new CompilerError(sourceFile, 0, 0, "Parse", 
                        $"Failed to parse source file: {ex.Message}"));
                    return errorResults;
                }
            }

            // Collect references from loaded assemblies
            var references = new List<MetadataReference>();
            foreach (var assemblyPath in Options.ReferencedAssemblies)
            {
                try
                {
                    string resolvedPath = assemblyPath;

                    // If bare filename (e.g. "System.Design.dll"), try to resolve from loaded assemblies
                    // or shared framework directories
                    if (!string.IsNullOrWhiteSpace(resolvedPath) && !File.Exists(resolvedPath) 
                        && !Path.IsPathRooted(resolvedPath))
                    {
                        // Try loaded assemblies first
                        var loaded = AppDomain.CurrentDomain.GetAssemblies();
                        var match = loaded.FirstOrDefault(a => 
                            !string.IsNullOrEmpty(a.Location) &&
                            Path.GetFileName(a.Location).Equals(resolvedPath, StringComparison.OrdinalIgnoreCase));
                        if (match != null)
                        {
                            resolvedPath = match.Location;
                        }
                        else
                        {
                            // Try to load by assembly name
                            try
                            {
                                var asm = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(resolvedPath)));
                                if (asm != null && !string.IsNullOrEmpty(asm.Location))
                                    resolvedPath = asm.Location;
                            }
                            catch { }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(resolvedPath) || !File.Exists(resolvedPath))
                        continue;

                    references.Add(MetadataReference.CreateFromFile(resolvedPath));
                }
                catch
                {
                    // Skip assemblies we cannot reference
                }
            }

            // Add essential .NET runtime references
            var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
            if (runtimeDir != null)
            {
                var essentialRefs = new[] {
                    "System.Runtime.dll",
                    "System.Collections.dll",
                    "netstandard.dll",
                    "System.Drawing.Primitives.dll",
                    "System.Text.RegularExpressions.dll",
                    "System.Linq.dll",
                    "System.Linq.Expressions.dll",
                    "System.Data.Common.dll",
                    // Type-forwarded APIs required by dynamic plugins
                    "System.IO.FileSystem.Watcher.dll",             // FileSystemWatcher, FileSystemEventArgs, NotifyFilters
                    "System.Private.DataContractSerialization.dll", // DataContractSerializer
                    "System.Net.Mail.dll",                           // System.Net.Mime.MediaTypeNames
                    "System.IO.Packaging.dll",               // System.IO.Packaging.Package
                    "System.Windows.Forms.dll",
                    "System.Drawing.Common.dll",
                };
                foreach (var refName in essentialRefs)
                {
                    var refPath = Path.Combine(runtimeDir, refName);
                    if (File.Exists(refPath) && !references.Any(r => r.Display?.Contains(refName) == true))
                    {
                        references.Add(MetadataReference.CreateFromFile(refPath));
                    }
                }
            }

            // Add System.Drawing.Common from application directory (NuGet package)
            var appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (appDir != null)
            {
                var drawingCommonPath = Path.Combine(appDir, "System.Drawing.Common.dll");
                if (File.Exists(drawingCommonPath) && !references.Any(r => r.Display?.Contains("System.Drawing.Common") == true))
                {
                    references.Add(MetadataReference.CreateFromFile(drawingCommonPath));
                }
            }

            // Determine optimization level from compiler options
            var optimizationLevel = Options.CompilerOptions?.Contains("/optimise") == true
                ? OptimizationLevel.Release
                : OptimizationLevel.Debug;

            // Create compilation
            var compilation = CSharpCompilation.Create(
                assemblyName: Path.GetFileNameWithoutExtension(AssemblyName),
                syntaxTrees: syntaxTrees,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOptimizationLevel(optimizationLevel)
                    .WithPlatform(Platform.AnyCpu));

            // Emit to a temp file so the loaded assembly has a valid Location.
            // Assembly.Load(bytes) produces an assembly with empty Location, which breaks any
            // secondary Roslyn compilation (e.g. DynamicCodeCompiler) that needs to reference it.
            string tempAsmPath = Path.Combine(Path.GetTempPath(), $"{Path.GetFileNameWithoutExtension(AssemblyName)}_{Guid.NewGuid():N}.dll");

            EmitResult emitResult = compilation.Emit(tempAsmPath);

            // Create CompilerResults for compatibility
            var results = new CompilerResults(new TempFileCollection(Path.GetTempPath()));

            if (!emitResult.Success)
            {
                // Clean up the (empty) temp file on failure.
                try { if (File.Exists(tempAsmPath)) File.Delete(tempAsmPath); } catch { }

                foreach (var diagnostic in emitResult.Diagnostics)
                {
                    if (diagnostic.Severity == DiagnosticSeverity.Error || 
                        diagnostic.Severity == DiagnosticSeverity.Warning)
                    {
                        var lineSpan = diagnostic.Location.GetLineSpan();
                        var error = new CompilerError
                        {
                            FileName = lineSpan.Path ?? "",
                            Line = lineSpan.StartLinePosition.Line + 1,
                            Column = lineSpan.StartLinePosition.Character + 1,
                            ErrorNumber = diagnostic.Id,
                            ErrorText = diagnostic.GetMessage(),
                            IsWarning = diagnostic.Severity == DiagnosticSeverity.Warning
                        };
                        results.Errors.Add(error);
                    }
                }
            }
            else
            {
                CompiledAssembly = Assembly.LoadFrom(tempAsmPath);

                // Set the compiled assembly in results
                typeof(CompilerResults)
                    .GetField("compiledAssembly", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.SetValue(results, CompiledAssembly);
            }

            return results;
        }

        public enum FileStructureType
        {
            SingleFile,
            Directory
        }
    }
}
