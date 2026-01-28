// Decompiled with JetBrains decompiler
// Type: Styx.Logic.Profiles.Quest.QuestBehaviorHelper
// Assembly: Honorbuddy, Version=2.0.0.5999, Culture=neutral, PublicKeyToken=50a565ab5c01ae50
// MVID: FB7FEB85-27C0-4D17-B8DE-615FDFDA7752
// Assembly location: C:\Users\Texy6\Desktop\Honorbuddy-cleaned.exe

using Styx.Helpers;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;

#nullable disable
namespace Styx.Logic.Profiles.Quest;

public class QuestBehaviorHelper
{
    private static readonly Dictionary<string, Assembly> _assemblyCache = new Dictionary<string, Assembly>();
    private static readonly object _lockObject = new object();
    private Assembly _cachedAssembly;
    private bool _isCompiled;

    public QuestBehaviorHelper(string path)
    {
        this.Path = path;
        if (!StyxSettings.Instance.ProfileDebuggingMode)
            return;
        this._cachedAssembly = this.CompileAssembly();
        this._isCompiled = true;
    }

    public string Path { get; private set; }

    private Assembly CompileAssembly()
    {
        string lower = this.Path.ToLower();
        Assembly assembly1;
        if (_assemblyCache.TryGetValue(lower, out assembly1))
            return assembly1;
        Assembly assembly2;
        lock (_lockObject)
        {
            Logging.WriteDebug("Compiling quest behavior from '{0}'", this.Path);
            
            // Compile the quest behavior
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };
            
            // Add references
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Xml.dll");
            parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            
            CompilerResults compilerResults;
            if (Directory.Exists(this.Path))
            {
                string[] files = Directory.GetFiles(this.Path, "*.cs", SearchOption.AllDirectories);
                compilerResults = provider.CompileAssemblyFromFile(parameters, files);
            }
            else
            {
                compilerResults = provider.CompileAssemblyFromFile(parameters, this.Path);
            }
            
            if (compilerResults.Errors.HasErrors)
            {
                Logging.WriteDebug("Could not compile quest behavior from '{0}'", this.Path);
                foreach (CompilerError compilerError in compilerResults.Errors.OfType<CompilerError>().Where(e => !e.IsWarning))
                    Logging.WriteDebug("Line {0}: {1}", compilerError.Line, compilerError.ErrorText);
                _assemblyCache.Add(lower, null);
                assembly2 = null;
            }
            else
            {
                _assemblyCache.Add(lower, compilerResults.CompiledAssembly);
                assembly2 = compilerResults.CompiledAssembly;
            }
        }
        return assembly2;
    }

    public Assembly GetAssembly()
    {
        if (!this._isCompiled)
        {
            this._cachedAssembly = this.CompileAssembly();
            this._isCompiled = true;
        }
        return this._cachedAssembly;
    }

    public static Func<Assembly> GetAssemblyCompiler(string path)
    {
        string path1 = System.IO.Path.Combine(System.IO.Path.Combine(Logging.ApplicationPath, "Quest Behaviors"), path);
        if (!Directory.Exists(path1))
            path1 = System.IO.Path.ChangeExtension(path1, ".cs");
        return new Func<Assembly>(new QuestBehaviorHelper(path1).GetAssembly);
    }
}
