# Styx.Common.Compiler

Contains compiler helpers.

## Contents

- [CodeCompiler Class](#codecompiler-class)
- [CodeCompiler.FileStructureType Enumeration](#codecompiler.filestructuretype-enumeration)

---

### CodeCompiler Class

**Inheritance:** System.Object → Styx.Common.Compiler.CodeCompiler

```csharp
public class CodeCompiler
```

This class handles compiling any code that we need to do dynamically. CCs, plugins,
            etc.

#### CodeCompiler Properties

- **`AssemblyName Property`**
  ```csharp
  [ObsoleteAttribute("Always null - do not use")]
public string AssemblyName { get; }
  ```
  The name of the assembly on disk.

- **`CompiledAssembly Property`**
  ```csharp
  public Assembly CompiledAssembly { get; }
  ```
  The final, compiled assembly.

- **`CompiledToLocation Property`**
  ```csharp
  public string CompiledToLocation { get; }
  ```
  Gets the compiled to location.

- **`CompilerVersion Property`**
  ```csharp
  [ObsoleteAttribute("Always the latest version - do not use")]
public float CompilerVersion { get; }
  ```
  The compiler version. [Default: 6.0].

- **`FileStructure Property`**
  ```csharp
  public CodeCompiler.FileStructureType FileStructure { get; }
  ```
  The type of source structure this compiler uses. File, folder, etc.

- **`Options Property`**
  ```csharp
  public CompilerParameters Options { get; }
  ```
  Retrieves the current compiler options.

- **`SourceFilePaths Property`**
  ```csharp
  public List&lt;string&gt; SourceFilePaths { get; }
  ```
  Gets source file paths.

- **`SourcePath Property`**
  ```csharp
  public string SourcePath { get; }
  ```
  The path to the source files. This can be a directory, or a single file. See
            FileStructure for the current type.

#### CodeCompiler Methods

- **`AddReference Method`**
  ```csharp
  public void AddReference(
string assembly
)
  ```
  Adds a reference.
  - *assembly*: Type: SystemAddLanguageSpecificTextSet("LSTDA13D0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe assembly.

- **`Compile Method`**
  ```csharp
  public CompilerResults Compile()
  ```
  Gets the compile.
  - **Returns:** ReferenceCodeCompiler Class

- **`CompileAsync Method`**
  ```csharp
  public Task&lt;CompilerResults&gt; CompileAsync()
  ```
  - **Returns:** See Also

- **`CreateLatestCSharpProvider Method`**
  ```csharp
  public static CodeDomProvider CreateLatestCSharpProvider()
  ```
  Creates a CodeDomProvider that supports compiling code for the latest C# version supported by Bossland GmbH.
  - **Returns:** ExceptionConditionFileNotFoundExceptionThrown if 'base path\roslyn\csc.exe' does not exist.

- **`DeleteOldAssemblies Method`**
  ```csharp
  public static void DeleteOldAssemblies()
  ```
  Deletes the old assemblies.

---

### CodeCompiler.FileStructureType Enumeration

```csharp
public enum FileStructureType
```

Values that represent FileStructureType.

---
