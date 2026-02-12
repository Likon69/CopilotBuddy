# Styx.CommonBot.Bars

## Contents

- [ActionBar Class](#actionbar-class)
- [ActionBarType Enumeration](#actionbartype-enumeration)
- [ActionButton Class](#actionbutton-class)
- [ActionButtonSubType Enumeration](#actionbuttonsubtype-enumeration)
- [ActionButtonType Enumeration](#actionbuttontype-enumeration)
- [SpellActionButton Class](#spellactionbutton-class)

---

### ActionBar Class

**Inheritance:** System.Object → Styx.CommonBot.Bars.ActionBar

```csharp
public class ActionBar
```

Gets the active action bar.

#### ActionBar Properties

- **`Active Property`**
  ```csharp
  public static ActionBar Active { get; }
  ```
  Gets the active action bar.

- **`Buttons Property`**
  ```csharp
  public IReadOnlyList&lt;ActionButton&gt; Buttons { get; }
  ```
  Gets the buttons being used on action bar

- **`Extra Property`**
  ```csharp
  public static ActionBar Extra { get; }
  ```
  Gets the extra action bar.

- **`IsActive Property`**
  ```csharp
  public bool IsActive { get; }
  ```
  Gets a value indicating whether this action bar is active.

- **`Page Property`**
  ```csharp
  public int Page { get; }
  ```
  Gets the action bar page number

- **`Type Property`**
  ```csharp
  public ActionBarType Type { get; }
  ```
  Gets the action bar type.

---

### ActionBarType Enumeration

```csharp
public enum ActionBarType
```

---

### ActionButton Class

**Inheritance:** System.Object → Styx.CommonBot.Bars.ActionButton → Styx.CommonBot.Bars.SpellActionButton

```csharp
public class ActionButton
```

return true if action can be used and is off cooldown

#### ActionButton Properties

- **`CanUse Property`**
  ```csharp
  public bool CanUse { get; }
  ```
  return true if action can be used and is off cooldown

- **`ChargeNum Property`**
  ```csharp
  public int ChargeNum { get; }
  ```
  Gets the number of charges

- **`Cooldown Property`**
  ```csharp
  public bool Cooldown { get; }
  ```
  Gets a value indicating whether [on cooldown].

- **`CoolDownLeft Property`**
  ```csharp
  public TimeSpan CoolDownLeft { get; }
  ```
  Gets the cooldown left.

- **`Id Property`**
  ```csharp
  public int Id { get; }
  ```
  Gets the identifier.

- **`IdString Property`**
  ```csharp
  public string IdString { get; }
  ```
  Gets the identifier string. Some Ids are not numerical like for equipment set buttons

- **`Index Property`**
  ```csharp
  public int Index { get; }
  ```
  Gets the button index. This is between 1 and 12 where 1 is the first button on page

- **`IsEnabled Property`**
  ```csharp
  public virtual bool IsEnabled { get; }
  ```
  Gets a value indicating whether this button is enabled.

- **`MaxChargeNum Property`**
  ```csharp
  public int MaxChargeNum { get; }
  ```
  Gets the maximum number of charges

- **`Name Property`**
  ```csharp
  public virtual string Name { get; }
  ```
  Gets the name.

- **`Slot Property`**
  ```csharp
  public int Slot { get; }
  ```
  Gets the button slot.

- **`SubType Property`**
  ```csharp
  public ActionButtonSubType SubType { get; }
  ```
  Gets the type of the sub.

- **`Type Property`**
  ```csharp
  public ActionButtonType Type { get; }
  ```
  Gets the type.

#### ActionButton Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceActionButton Class

- **`Use Method`**
  ```csharp
  public void Use()
  ```
  Uses this button.

---

### ActionButtonSubType Enumeration

```csharp
public enum ActionButtonSubType
```

---

### ActionButtonType Enumeration

```csharp
public enum ActionButtonType
```

---

### SpellActionButton Class

**Inheritance:** System.Object → Styx.CommonBot.Bars.ActionButton → Styx.CommonBot.Bars.SpellActionButton

```csharp
public sealed class SpellActionButton : ActionButton
```

return true if action can be used and is off cooldown

#### SpellActionButton Properties

- **`Name Property`**
  ```csharp
  public override string Name { get; }
  ```

- **`Spell Property`**
  ```csharp
  public WoWSpell Spell { get; }
  ```

- **`SpellId Property`**
  ```csharp
  public int SpellId { get; }
  ```

---
