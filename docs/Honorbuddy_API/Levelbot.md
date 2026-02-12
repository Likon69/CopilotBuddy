# Levelbot

Contains level bot related classes.

## Contents

- [FormLevelbotSettings Class](#formlevelbotsettings-class)
- [HandleCombatCoroutineTask Class](#handlecombatcoroutinetask-class)
- [HandleDeathCoroutineTask Class](#handledeathcoroutinetask-class)
- [HandleLootingCoroutineTask Class](#handlelootingcoroutinetask-class)

---

### FormLevelbotSettings Class

**Inheritance:** System.Object → System.MarshalByRefObject → System.ComponentModel.Component → System.Windows.Forms.Control → System.Windows.Forms.ScrollableControl → System.Windows.Forms.ContainerControl → System.Windows.Forms.Form → Levelbot.FormLevelbotSettings

```csharp
public class FormLevelbotSettings : Form
```

A form levelbot settings.

#### FormLevelbotSettings Methods

- **`Dispose Method`**
  Releases all resources used by the Component.

- **`Dispose Method (Boolean)`**
  ```csharp
  protected override void Dispose(
bool disposing
)
  ```
  Clean up any resources being used.
  - *disposing*: Type: SystemAddLanguageSpecificTextSet("LSTE5ACA49D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Booleantrue if managed resources should be disposed; otherwise, false.

---

### HandleCombatCoroutineTask Class

**Inheritance:** System.Object → Styx.CommonBot.Coroutines.CoroutineTask.Boolean → Levelbot.HandleCombatCoroutineTask

```csharp
public class HandleCombatCoroutineTask : CoroutineTask&lt;bool&gt;
```

Initializes a new instance of the HandleCombatCoroutineTask class

#### HandleCombatCoroutineTask Methods

- **`Run Method`**
  ```csharp
  public override Task&lt;bool&gt; Run()
  ```
  - **Returns:** See Also

---

### HandleDeathCoroutineTask Class

**Inheritance:** System.Object → Styx.CommonBot.Coroutines.CoroutineTask.Boolean → Levelbot.HandleDeathCoroutineTask

```csharp
public class HandleDeathCoroutineTask : CoroutineTask&lt;bool&gt;
```

Initializes a new instance of the HandleDeathCoroutineTask class

#### HandleDeathCoroutineTask Methods

- **`Run Method`**
  ```csharp
  public override Task&lt;bool&gt; Run()
  ```
  - **Returns:** See Also

---

### HandleLootingCoroutineTask Class

**Inheritance:** System.Object → Styx.CommonBot.Coroutines.CoroutineTask.Boolean → Levelbot.HandleLootingCoroutineTask

```csharp
public class HandleLootingCoroutineTask : CoroutineTask&lt;bool&gt;
```

Initializes a new instance of the HandleLootingCoroutineTask class

#### HandleLootingCoroutineTask Methods

- **`Run Method`**
  ```csharp
  public override Task&lt;bool&gt; Run()
  ```
  - **Returns:** See Also

---
