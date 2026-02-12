# Styx.CommonBot.Coroutines

Contains classes for easing the usage of Coroutines to develop bot logic.

## Contents

- [CommonCoroutines Class](#commoncoroutines-class)
- [CoroutineCompositeExtensions Class](#coroutinecompositeextensions-class)
- [CoroutineTask Class](#coroutinetask-class)
- [CoroutineTask(T) Class](#coroutinetaskt-class)
- [CoroutineTaskAwaiter Structure](#coroutinetaskawaiter-structure)
- [CoroutineTaskAwaiter(T) Structure](#coroutinetaskawaitert-structure)

---

### CommonCoroutines Class

**Inheritance:** System.Object → Styx.CommonBot.Coroutines.CommonCoroutines

```csharp
public static class CommonCoroutines
```

Attempts to dismount the player.

#### CommonCoroutines Methods

- **`Dismount Method`**
  ```csharp
  public static Task&lt;bool&gt; Dismount(
string reason = null,
bool descend = true
)
  ```
  Attempts to dismount the player.
  - **Returns:** See Also

- **`LandAndDismount Method`**
  Lands and dismounts

- **`LandAndDismount Method (String, Boolean, Nullable(Vector3))`**
  ```csharp
  public static Task&lt;bool&gt; LandAndDismount(
string reason = "",
bool dismount = true,
Nullable&lt;Vector3&gt; landPoint = null
)
  ```
  Lands and dismounts
  - **Returns:** See Also

- **`LandAndDismount Method (String, Boolean, Nullable(Vector3), Nullable(Single))`**
  ```csharp
  public static Task&lt;bool&gt; LandAndDismount(
string reason,
bool dismount,
Nullable&lt;Vector3&gt; landPoint,
Nullable&lt;float&gt; landPointRadius
)
  ```
  Lands and dismounts
  - *reason*: Type: SystemAddLanguageSpecificTextSet("LSTFA8F25D_5?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe reason.
  - *dismount*: Type: SystemAddLanguageSpecificTextSet("LSTFA8F25D_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Booleanif set to true dismount after landing.
  - *landPoint*: Type: SystemAddLanguageSpecificTextSet("LSTFA8F25D_7?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LSTFA8F25D_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTFA8F25D_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The land point. If null or not specified then a landing point is found automatically
  - *landPointRadius*: Type: SystemAddLanguageSpecificTextSet("LSTFA8F25D_10?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LSTFA8F25D_11?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");SingleAddLanguageSpecificTextSet("LSTFA8F25D_12?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
                The land point radius.
                The radius of the area around [!:landPoint] to land inside of
                If null, this will land on top of [!:landPoint].
                Value must be between 5 and 100 (inclusive)
  - **Returns:** See Also

- **`MoveTo Method`**
  Moves to a location.

- **`MoveTo Method (Vector3, String)`**
  ```csharp
  public static Task&lt;MoveResult&gt; MoveTo(
Vector3 destination,
string destinationName = null
)
  ```
  Moves to a location.
  - *destination*: Type: System.NumericsAddLanguageSpecificTextSet("LST8AA5DEFC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The destination.
  - **Returns:** See Also

- **`MoveTo Method (MoveToParameters, String)`**
  ```csharp
  public static Task&lt;MoveResult&gt; MoveTo(
MoveToParameters parameters,
string destinationName = null
)
  ```
  Moves to a location.
  - *parameters*: Type: Styx.PathingAddLanguageSpecificTextSet("LST561F3292_1?cs=.|vb=.|cpp=::|nu=.|fs=.");MoveToParametersThe parameters.
  - **Returns:** See Also

- **`MoveToPoi Method`**
  ```csharp
  public static Task&lt;MoveResult&gt; MoveToPoi()
  ```
  - **Returns:** See Also

- **`SleepForLagDuration Method`**
  ```csharp
  public static Task SleepForLagDuration()
  ```
  Sleeps for the maximum expected round trip time between local and remote blizard servers.
  - **Returns:** ReferenceCommonCoroutines Class

- **`SleepForRandomReactionTime Method`**
  ```csharp
  public static Task SleepForRandomReactionTime()
  ```
  Sleeps for approximately the time it takes for a human to react to in-game events using keyboard as input
  - **Returns:** ReferenceCommonCoroutines Class

- **`SleepForRandomUiInteractionTime Method`**
  ```csharp
  public static Task SleepForRandomUiInteractionTime()
  ```
  Sleeps for approximately the time it takes for a human to interact with UI elements using mouse.
  - **Returns:** ReferenceCommonCoroutines Class

- **`StopMoving Method`**
  ```csharp
  public static Task&lt;bool&gt; StopMoving(
string reason = null
)
  ```
  Executes a MoveStop command if moving and waits for movement to stop
  - **Returns:** See Also

- **`SummonFlyingMount Method`**
  ```csharp
  public static Task&lt;bool&gt; SummonFlyingMount()
  ```
  Summons a mount that can fly at the current location.
            Returns false if the current area is not flyable or if we did not
            summon a flying mount.
  - **Returns:** See Also

- **`SummonGroundMount Method`**
  Attempts to summon a ground mount. Returns true if we summoned a mount.

- **`SummonGroundMount Method`**
  ```csharp
  public static Task&lt;bool&gt; SummonGroundMount()
  ```
  Attempts to summon a ground mount. Returns true if we summoned a mount.
  - **Returns:** See Also

- **`SummonGroundMount Method (Nullable(Vector3))`**
  ```csharp
  public static Task&lt;bool&gt; SummonGroundMount(
Nullable&lt;Vector3&gt; destination
)
  ```
  Attempts to summon a ground mount. Returns true if we summoned a mount.
  - *destination*: Type: SystemAddLanguageSpecificTextSet("LSTDD6D4F4D_3?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LSTDD6D4F4D_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTDD6D4F4D_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The destination we are expecting to move to.
  - **Returns:** See Also

- **`SummonGroundMount Method (Nullable(Vector3), Nullable(Single))`**
  ```csharp
  public static Task&lt;bool&gt; SummonGroundMount(
Nullable&lt;Vector3&gt; destination,
Nullable&lt;float&gt; moveDistance
)
  ```
  Attempts to summon a ground mount. Returns true if we summoned a mount.
  - *destination*: Type: SystemAddLanguageSpecificTextSet("LST229F07C2_5?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST229F07C2_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST229F07C2_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The destination we are expecting to move to.
  - *moveDistance*: Type: SystemAddLanguageSpecificTextSet("LST229F07C2_8?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST229F07C2_9?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");SingleAddLanguageSpecificTextSet("LST229F07C2_10?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The expected distance until we will be dismounted.
  - **Returns:** See Also

- **`SummonGroundMount Method (Nullable(Vector3), Nullable(Single), Boolean)`**
  ```csharp
  [ObsoleteAttribute("Use the version without the allowFlyingMounts parameter")]
public static Task&lt;bool&gt; SummonGroundMount(
Nullable&lt;Vector3&gt; destination = null,
Nullable&lt;float&gt; moveDistance = null,
bool allowFlyingMounts = false
)
  ```
  Attempts to summon a ground mount. Returns true if we summoned a mount.
  - **Returns:** See Also

- **`WaitForLuaEvent Method`**
  Waits for a lua event to trigger

- **`WaitForLuaEvent Method (String, Int32, Func(Boolean), Action)`**
  ```csharp
  public static Task&lt;bool&gt; WaitForLuaEvent(
string eventName,
int maxWaitMs,
Func&lt;bool&gt; alternateCondition = null,
Action action = null
)
  ```
  Waits for a lua event to trigger
  - *eventName*: Type: SystemAddLanguageSpecificTextSet("LSTACCD6263_3?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the event.
  - *maxWaitMs*: Type: SystemAddLanguageSpecificTextSet("LSTACCD6263_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The maximum wait timeout in milliseconds.
  - **Returns:** See Also

- **`WaitForLuaEvent Method (String, TimeSpan, Func(Boolean), Action)`**
  ```csharp
  public static Task&lt;bool&gt; WaitForLuaEvent(
string eventName,
TimeSpan maxWaitTimeout,
Func&lt;bool&gt; alternateCondition = null,
Action action = null
)
  ```
  Waits for a lua event to trigger
  - *eventName*: Type: SystemAddLanguageSpecificTextSet("LST1BD6CA70_3?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the event.
  - *maxWaitTimeout*: Type: SystemAddLanguageSpecificTextSet("LST1BD6CA70_4?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe maximum wait timeout.
  - **Returns:** See Also

---

### CoroutineCompositeExtensions Class

**Inheritance:** System.Object → Styx.CommonBot.Coroutines.CoroutineCompositeExtensions

```csharp
public static class CoroutineCompositeExtensions
```

Executes the composite inside a coroutine.

#### CoroutineCompositeExtensions Methods

- **`ExecuteCoroutine Method`**
  ```csharp
  public static Task&lt;bool&gt; ExecuteCoroutine(
this Composite composite,
Object context = null
)
  ```
  Executes the composite inside a coroutine.
  - *composite*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST99439280_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite
  - **Returns:** See Also

---

### CoroutineTask Class

**Inheritance:** System.Object → Styx.CommonBot.Coroutines.CoroutineTask

```csharp
public abstract class CoroutineTask
```

Represents a coroutine task that can be derived from to implement coroutine parts with state. This class is awaitable.

#### CoroutineTask Methods

- **`GetAwaiter Method`**
  ```csharp
  public CoroutineTaskAwaiter GetAwaiter()
  ```
  Gets the awaiter for this coroutine task.
  - **Returns:** ReferenceCoroutineTask Class

- **`Run Method`**
  ```csharp
  public abstract Task Run()
  ```
  Provides the body of this coroutine task.
  - **Returns:** ReferenceCoroutineTask Class

---

### CoroutineTask(T) Class

**Inheritance:** System.Object → Styx.CommonBot.Coroutines.CoroutineTask.T → Levelbot.HandleCombatCoroutineTask → Levelbot.HandleDeathCoroutineTask → Levelbot.HandleLootingCoroutineTask

```csharp
public abstract class CoroutineTask&lt;T&gt;
```

Represents a coroutine task that returns a value and that can be derived from to implement coroutine parts with state. This class is awaitable.

#### CoroutineTask(T) Methods

- **`GetAwaiter Method`**
  ```csharp
  public CoroutineTaskAwaiter&lt;T&gt; GetAwaiter()
  ```
  Gets the awaiter for this coroutine task.
  - **Returns:** See Also

- **`Run Method`**
  ```csharp
  public abstract Task&lt;T&gt; Run()
  ```
  Provides the body of this coroutine task.
  - **Returns:** See Also

---

### CoroutineTaskAwaiter Structure

```csharp
public struct CoroutineTaskAwaiter : INotifyCompletion
```

Indicates whether this instance and a specified object are equal.

#### CoroutineTaskAwaiter Properties

- **`IsCompleted Property`**
  ```csharp
  public bool IsCompleted { get; }
  ```

#### CoroutineTaskAwaiter Methods

- **`GetResult Method`**
  ```csharp
  public void GetResult()
  ```

- **`OnCompleted Method`**
  ```csharp
  public void OnCompleted(
Action continuation
)
  ```
  - *continuation*: Type: SystemAddLanguageSpecificTextSet("LST8B282FAC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Action

---

### CoroutineTaskAwaiter(T) Structure

```csharp
public struct CoroutineTaskAwaiter&lt;T&gt; : INotifyCompletion
```

Indicates whether this instance and a specified object are equal.

#### CoroutineTaskAwaiter(T) Properties

- **`IsCompleted Property`**
  ```csharp
  public bool IsCompleted { get; }
  ```

#### CoroutineTaskAwaiter(T) Methods

- **`GetResult Method`**
  ```csharp
  public T GetResult()
  ```
  - **Returns:** See Also

- **`OnCompleted Method`**
  ```csharp
  public void OnCompleted(
Action continuation
)
  ```
  - *continuation*: Type: SystemAddLanguageSpecificTextSet("LSTEA5C0F7_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Action

---
