# CommonBehaviors

Contains common behaviors.

## Contents

- [ActionWaitForLuaEvent Class](#actionwaitforluaevent-class)
- [WaitLuaEvent Class](#waitluaevent-class)

---

### ActionWaitForLuaEvent Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → CommonBehaviors.ActionWaitForLuaEvent

```csharp
public class ActionWaitForLuaEvent : Action
```

An action wait for lua event.

#### ActionWaitForLuaEvent Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST42B4B3C5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionWaitForLuaEvent Class

- **`Start Method`**
  ```csharp
  public override void Start(
Object context
)
  ```
  Initializes the Task into a runnable state by clearing the LastStatus (if any),
            clearing the cleanup handlers remaining (if any -- note that this may also trigger the
            IDisposable behavior of any remaining cleanup handlers on the cleanup stack), and
            reinitializing the execution enumerator.
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTF3FFEA84_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object    The context.The context.

- **`Stop Method`**
  ```csharp
  public override void Stop(
Object context
)
  ```
  Stop will call all cleanup handlers!
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST885A988A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe context.

---

### WaitLuaEvent Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → Styx.TreeSharp.Wait → Styx.TreeSharp.WaitContinue → CommonBehaviors.WaitLuaEvent

```csharp
public class WaitLuaEvent : WaitContinue
```

A wait lua event.

#### WaitLuaEvent Constructor

- **`WaitLuaEvent Constructor (String, WaitGetTimeoutDelegate, Composite)`**
  ```csharp
  public WaitLuaEvent(
string luaEvent,
WaitGetTimeoutDelegate timeoutRetriever,
Composite child
)
  ```
  Constructor.
  - *luaEvent*: Type: SystemAddLanguageSpecificTextSet("LSTB572549E_0?cs=.|vb=.|cpp=::|nu=.|fs=.");String        The lua event.
  - *timeoutRetriever*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTB572549E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WaitGetTimeoutDelegateThe timeout retriever.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTB572549E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite           The child.

- **`WaitLuaEvent Constructor (String, Int32, Composite)`**
  ```csharp
  public WaitLuaEvent(
string luaEvent,
int timeoutSeconds,
Composite child
)
  ```
  Constructor.
  - *luaEvent*: Type: SystemAddLanguageSpecificTextSet("LST7AAE6764_0?cs=.|vb=.|cpp=::|nu=.|fs=.");String      The lua event.
  - *timeoutSeconds*: Type: SystemAddLanguageSpecificTextSet("LST7AAE6764_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The timeout in seconds.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST7AAE6764_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite         The child.

- **`WaitLuaEvent Constructor (String, TimeSpan, Composite)`**
  ```csharp
  public WaitLuaEvent(
string luaEvent,
TimeSpan timeout,
Composite child
)
  ```
  Constructor.
  - *luaEvent*: Type: SystemAddLanguageSpecificTextSet("LST3A9E4DDF_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe lua event.
  - *timeout*: Type: SystemAddLanguageSpecificTextSet("LST3A9E4DDF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpan The timeout.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST3A9E4DDF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite   The child.

- **`WaitLuaEvent Constructor (String, WaitGetTimeoutDelegate, CanRunDecoratorDelegate, Composite)`**
  ```csharp
  public WaitLuaEvent(
string luaEvent,
WaitGetTimeoutDelegate timeoutRetriever,
CanRunDecoratorDelegate alternateCondition,
Composite child
)
  ```
  Constructor.
  - *luaEvent*: Type: SystemAddLanguageSpecificTextSet("LST6480AF39_0?cs=.|vb=.|cpp=::|nu=.|fs=.");String				The lua event.
  - *timeoutRetriever*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST6480AF39_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WaitGetTimeoutDelegate		The timeout retriever.
  - *alternateCondition*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST6480AF39_2?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate   The alternate condition to check.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST6480AF39_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite				The child.

- **`WaitLuaEvent Constructor (String, Int32, CanRunDecoratorDelegate, Composite)`**
  ```csharp
  public WaitLuaEvent(
string luaEvent,
int timeoutSeconds,
CanRunDecoratorDelegate alternateCondition,
Composite child
)
  ```
  Constructor.
  - *luaEvent*: Type: SystemAddLanguageSpecificTextSet("LSTFA0AC615_0?cs=.|vb=.|cpp=::|nu=.|fs=.");String				The lua event.
  - *timeoutSeconds*: Type: SystemAddLanguageSpecificTextSet("LSTFA0AC615_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32		The timeout in seconds.
  - *alternateCondition*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTFA0AC615_2?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate   The alternate condition to check.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTFA0AC615_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite         The child.

#### WaitLuaEvent Methods

- **`CanRun Method`**
  ```csharp
  protected override bool CanRun(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTB24F6856_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceWaitLuaEvent Class

- **`Start Method`**
  ```csharp
  public override void Start(
Object context
)
  ```
  Initializes the Task into a runnable state by clearing the LastStatus (if any),
            clearing the cleanup handlers remaining (if any -- note that this may also trigger the
            IDisposable behavior of any remaining cleanup handlers on the cleanup stack), and
            reinitializing the execution enumerator.
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTFFD4ED75_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object    The context.The context.

- **`Stop Method`**
  ```csharp
  public override void Stop(
Object context
)
  ```
  Stop will call all cleanup handlers!
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTD21B8591_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe context.

---
