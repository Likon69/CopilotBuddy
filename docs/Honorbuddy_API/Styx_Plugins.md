# Styx.Plugins

Contains plugin related classes.

## Contents

- [HBPlugin Class](#hbplugin-class)
- [PluginContainer Class](#plugincontainer-class)
- [PluginManager Class](#pluginmanager-class)
- [PluginWrapper Class](#pluginwrapper-class)

---

### HBPlugin Class

**Inheritance:** System.Object → Styx.Plugins.HBPlugin

```csharp
public abstract class HBPlugin
```

Represents a plugin for HB.

#### HBPlugin Properties

- **`Author Property`**
  ```csharp
  public abstract string Author { get; }
  ```
  The author of this plugin.

- **`ButtonText Property`**
  ```csharp
  public virtual string ButtonText { get; }
  ```
  The text of the button if the plugin wants it. [Default: "Settings"].

- **`Name Property`**
  ```csharp
  public abstract string Name { get; }
  ```
  The name of this plugin.

- **`Version Property`**
  ```csharp
  public abstract Version Version { get; }
  ```
  The version of the plugin.

- **`WantButton Property`**
  ```csharp
  public virtual bool WantButton { get; }
  ```
  Does this plugin want a button? If set to true, the button in the plugin manager will
            be enabled. [Default: false].

#### HBPlugin Methods

- **`OnButtonPress Method`**
  ```csharp
  public virtual void OnButtonPress()
  ```
  Called when the user presses the button while having this plugin selected. The plugin
            can start a thread, show a form, or just do what the hell it wants.

- **`OnDisable Method`**
  ```csharp
  public virtual void OnDisable()
  ```
  Called when this plugin is disabled, and also on shut down of the bot.

- **`OnEnable Method`**
  ```csharp
  public virtual void OnEnable()
  ```
  Called when this plugin is enabled.

- **`Pulse Method`**
  ```csharp
  public abstract void Pulse()
  ```
  Called everytime the engine pulses.

---

### PluginContainer Class

**Inheritance:** System.Object → Styx.Plugins.PluginContainer

```csharp
public class PluginContainer : IComparable&lt;PluginContainer&gt;
```

A plugin container.

#### PluginContainer Properties

- **`Author Property`**
  ```csharp
  public string Author { get; }
  ```
  Gets the author.

- **`ButtonText Property`**
  ```csharp
  public string ButtonText { get; }
  ```
  Gets the button text.

- **`Enabled Property`**
  ```csharp
  public bool Enabled { get; set; }
  ```
  Gets or sets a value indicating whether this object is enabled.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`Plugin Property`**
  ```csharp
  public PluginWrapper Plugin { get; }
  ```
  Gets the plugin.

- **`Version Property`**
  ```csharp
  public Version Version { get; }
  ```
  Gets the version.

- **`WantButton Property`**
  ```csharp
  public bool WantButton { get; }
  ```
  Gets a value indicating whether the want button.

#### PluginContainer Methods

- **`CompareTo Method`**
  ```csharp
  public int CompareTo(
PluginContainer other
)
  ```
  Compares this PluginContainer object to another to determine their relative
            ordering.
  - *other*: Type: Styx.PluginsAddLanguageSpecificTextSet("LSTD827230A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");PluginContainerAnother instance to compare.
  - **Returns:** See Also

---

### PluginManager Class

**Inheritance:** System.Object → Styx.Plugins.PluginManager

```csharp
public static class PluginManager
```

Manager for plugins.

#### PluginManager Properties

- **`IsInitialized Property`**
  ```csharp
  public static bool IsInitialized { get; }
  ```
  Gets a value indicating whether this object is initialized.

- **`Plugins Property`**
  ```csharp
  public static List&lt;PluginContainer&gt; Plugins { get; }
  ```
  Gets the plugins.

- **`PluginsDirectory Property`**
  ```csharp
  public static string PluginsDirectory { get; }
  ```
  Gets the routine directory.

#### PluginManager Methods

- **`RefreshPlugins Method`**
  ```csharp
  [ObsoleteAttribute("Use ReloadPluginsFromSource")]
public static void RefreshPlugins(
params string[] defaultEnabled
)
  ```
  Refresh plugins.
  - *defaultEnabled*: Type: AddLanguageSpecificTextSet("LSTFF7C4379_1?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTFF7C4379_2?cs=.|vb=.|cpp=::|nu=.|fs=.");StringAddLanguageSpecificTextSet("LSTFF7C4379_3?cpp=&gt;|vb=()|nu=[]");The default enabled.

- **`ReloadPluginsFromSource Method`**
  ```csharp
  public static Task ReloadPluginsFromSource()
  ```
  Reloads all plugins from source.
  - **Returns:** ExceptionConditionInvalidOperationExceptionThrown if the bot is running onpaused.

---

### PluginWrapper Class

**Inheritance:** System.Object → Styx.Plugins.PluginWrapper

```csharp
public class PluginWrapper
```

A plugin wrapper.

#### PluginWrapper Properties

- **`ButtonText Property`**
  ```csharp
  public string ButtonText { get; }
  ```
  Gets the button text.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`WantButton Property`**
  ```csharp
  public bool WantButton { get; }
  ```
  Gets a value indicating whether the want button.

#### PluginWrapper Methods

- **`OnButtonPress Method`**
  ```csharp
  public void OnButtonPress()
  ```
  Executes the button press action.

- **`OnDisable Method`**
  ```csharp
  public void OnDisable()
  ```

- **`OnEnable Method`**
  ```csharp
  public void OnEnable()
  ```

- **`Pulse Method`**
  ```csharp
  public void Pulse()
  ```
  Pulses this object.

#### PluginWrapper Type Conversions

- **`Implicit Conversion (HBPlugin to PluginWrapper)`**
  ```csharp
  public static implicit operator PluginWrapper (
HBPlugin plugin
)
  ```
  PluginWrapper casting operator.
  - *plugin*: Type: Styx.PluginsAddLanguageSpecificTextSet("LSTAF19D0A4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");HBPluginThe plugin.
  - **Returns:** ReferencePluginWrapper Class

---
