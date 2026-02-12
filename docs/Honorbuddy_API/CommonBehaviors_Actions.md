# CommonBehaviors.Actions

Contains common actions.

## Contents

- [ActionAlwaysFail Class](#actionalwaysfail-class)
- [ActionAlwaysSucceed Class](#actionalwayssucceed-class)
- [ActionClearPoi Class](#actionclearpoi-class)
- [ActionDebugString Class](#actiondebugstring-class)
- [ActionIdle Class](#actionidle-class)
- [ActionMoveToPoi Class](#actionmovetopoi-class)
- [ActionRunCoroutine Class](#actionruncoroutine-class)
- [ActionSetActivity Class](#actionsetactivity-class)
- [ActionSetActivity.GetStatusTextDelegate Delegate](#actionsetactivity.getstatustextdelegate-delegate)
- [ActionSetPoi Class](#actionsetpoi-class)
- [DebugStringDelegate Delegate](#debugstringdelegate-delegate)
- [NavigationAction Class](#navigationaction-class)
- [NavigationInfo Class](#navigationinfo-class)
- [RetrieveBotPoiDelegate Delegate](#retrievebotpoidelegate-delegate)
- [SleepForLagDuration Class](#sleepforlagduration-class)

---

### ActionAlwaysFail Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → CommonBehaviors.Actions.ActionAlwaysFail

```csharp
public class ActionAlwaysFail : Action
```

An action always fail.

#### ActionAlwaysFail Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST7B965FAD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionAlwaysFail Class

---

### ActionAlwaysSucceed Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → CommonBehaviors.Actions.ActionAlwaysSucceed → CommonBehaviors.Actions.ActionIdle

```csharp
public class ActionAlwaysSucceed : Action
```

An action always succeed.

#### ActionAlwaysSucceed Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST1E5BD2F1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionAlwaysSucceed Class

---

### ActionClearPoi Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → CommonBehaviors.Actions.ActionClearPoi

```csharp
public class ActionClearPoi : Action
```

An action clear poi.

#### ActionClearPoi Constructor

- **`ActionClearPoi Constructor`**
  ```csharp
  public ActionClearPoi()
  ```
  Default constructor.

- **`ActionClearPoi Constructor (String)`**
  ```csharp
  public ActionClearPoi(
string reason
)
  ```
  Constructor.
  - *reason*: Type: SystemAddLanguageSpecificTextSet("LSTCC9B89_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe reason.

#### ActionClearPoi Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTF757CAC9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionClearPoi Class

---

### ActionDebugString Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → CommonBehaviors.Actions.ActionDebugString

```csharp
public class ActionDebugString : Action
```

An action debug string.

#### ActionDebugString Constructor

- **`ActionDebugString Constructor (String)`**
  ```csharp
  public ActionDebugString(
string message
)
  ```
  Constructor.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTFF4FAC30_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.

- **`ActionDebugString Constructor (DebugStringDelegate)`**
  ```csharp
  public ActionDebugString(
DebugStringDelegate stringDelegate
)
  ```
  Constructor.
  - *stringDelegate*: Type: CommonBehaviors.ActionsAddLanguageSpecificTextSet("LST586F52E2_0?cs=.|vb=.|cpp=::|nu=.|fs=.");DebugStringDelegateThe string delegate.

#### ActionDebugString Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTB12BA9D0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionDebugString Class

---

### ActionIdle Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → CommonBehaviors.Actions.ActionAlwaysSucceed → CommonBehaviors.Actions.ActionIdle

```csharp
public class ActionIdle : ActionAlwaysSucceed
```

An action idle.

#### ActionIdle Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST463CDA30_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionIdle Class

---

### ActionMoveToPoi Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → CommonBehaviors.Actions.ActionRunCoroutine → CommonBehaviors.Actions.ActionMoveToPoi

```csharp
[ObsoleteAttribute("Use CommonCoroutines.MoveToPoi instead")]
public class ActionMoveToPoi : ActionRunCoroutine
```

An action move to poi.

---

### ActionRunCoroutine Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → CommonBehaviors.Actions.ActionRunCoroutine → CommonBehaviors.Actions.ActionMoveToPoi → Levelbot.Actions.Combat.ActionMoveToKillPoi

```csharp
public class ActionRunCoroutine : Action
```

Represents an action that runs a coroutine.

#### ActionRunCoroutine Constructor

- **`ActionRunCoroutine Constructor (Func(Object, Coroutine))`**
  ```csharp
  public ActionRunCoroutine(
Func&lt;Object, Coroutine&gt; coroutineProducer
)
  ```
  Initializes a new instance with the specified coroutine producer.
  - *coroutineProducer*: Type: SystemAddLanguageSpecificTextSet("LST75DBB543_2?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST75DBB543_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, CoroutineAddLanguageSpecificTextSet("LST75DBB543_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");A function that produces a coroutine.

- **`ActionRunCoroutine Constructor (Func(Object, CoroutineTask(Boolean)))`**
  ```csharp
  public ActionRunCoroutine(
Func&lt;Object, CoroutineTask&lt;bool&gt;&gt; taskProducer
)
  ```
  Initializes a new instance with the specified coroutine task producer.
  - *taskProducer*: Type: SystemAddLanguageSpecificTextSet("LST8BC64355_4?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST8BC64355_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, CoroutineTaskAddLanguageSpecificTextSet("LST8BC64355_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST8BC64355_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");AddLanguageSpecificTextSet("LST8BC64355_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");A function that produces a coroutine task that is regarded as the root of a coroutine.

- **`ActionRunCoroutine Constructor (Func(Object, CoroutineTask))`**
  ```csharp
  public ActionRunCoroutine(
Func&lt;Object, CoroutineTask&gt; taskProducer
)
  ```
  Initializes a new instance with the specified coroutine task producer.
  - *taskProducer*: Type: SystemAddLanguageSpecificTextSet("LST332E5534_2?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST332E5534_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, CoroutineTaskAddLanguageSpecificTextSet("LST332E5534_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");A function that produces a coroutine task that is regarded as the root of a coroutine.

- **`ActionRunCoroutine Constructor (Func(Object, Task(Boolean)))`**
  ```csharp
  public ActionRunCoroutine(
Func&lt;Object, Task&lt;bool&gt;&gt; taskProducer
)
  ```
  Initializes a new instance with the specified task producer.
  - *taskProducer*: Type: SystemAddLanguageSpecificTextSet("LST32FEC335_4?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST32FEC335_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, TaskAddLanguageSpecificTextSet("LST32FEC335_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST32FEC335_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");AddLanguageSpecificTextSet("LST32FEC335_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");A function that produces a task that is regarded as the root of a coroutine.

- **`ActionRunCoroutine Constructor (Func(Object, Task))`**
  ```csharp
  public ActionRunCoroutine(
Func&lt;Object, Task&gt; taskProducer
)
  ```
  Initializes a new instance with the specified task producer.
  - *taskProducer*: Type: SystemAddLanguageSpecificTextSet("LSTBDEEC96C_2?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTBDEEC96C_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, TaskAddLanguageSpecificTextSet("LSTBDEEC96C_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");A function that produces a task that is regarded as the root of a coroutine.

#### ActionRunCoroutine Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTBBA2F549_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionRunCoroutine Class

- **`Start Method`**
  ```csharp
  public override void Start(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST40C438BE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object

- **`Stop Method`**
  ```csharp
  public override void Stop(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTD314DCE0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object

---

### ActionSetActivity Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → CommonBehaviors.Actions.ActionSetActivity

```csharp
public class ActionSetActivity : Action
```

An action set activity.

#### ActionSetActivity Constructor

- **`ActionSetActivity Constructor (String)`**
  ```csharp
  public ActionSetActivity(
string statusText
)
  ```
  Constructor.
  - *statusText*: Type: SystemAddLanguageSpecificTextSet("LST3B3C13CD_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe status text.

- **`ActionSetActivity Constructor (ActionSetActivity.GetStatusTextDelegate)`**
  ```csharp
  public ActionSetActivity(
ActionSetActivity.GetStatusTextDelegate getStatusText
)
  ```
  Constructor.
  - *getStatusText*: Type: CommonBehaviors.ActionsAddLanguageSpecificTextSet("LST6841D33B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ActionSetActivityAddLanguageSpecificTextSet("LST6841D33B_3?cs=.|vb=.|cpp=::|nu=.|fs=.");GetStatusTextDelegateThe get status text.

- **`ActionSetActivity Constructor (String, Object[])`**
  ```csharp
  public ActionSetActivity(
string format,
params Object[] args
)
  ```
  Constructor.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST758484C5_2?cs=.|vb=.|cpp=::|nu=.|fs=.");StringDescribes the format to use.
  - *args*: Type: AddLanguageSpecificTextSet("LST758484C5_3?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST758484C5_4?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST758484C5_5?cpp=&gt;|vb=()|nu=[]");  A variable-length parameters list containing arguments.

#### ActionSetActivity Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTAC586A4D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionSetActivity Class

---

### ActionSetActivity.GetStatusTextDelegate Delegate

```csharp
public delegate string GetStatusTextDelegate(
Object context
)
```

Gets status text delegate.

---

### ActionSetPoi Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → CommonBehaviors.Actions.ActionSetPoi

```csharp
public class ActionSetPoi : Action
```

An action set poi.

#### ActionSetPoi Constructor

- **`ActionSetPoi Constructor (RetrieveBotPoiDelegate)`**
  ```csharp
  public ActionSetPoi(
RetrieveBotPoiDelegate poiRetrievalFunc
)
  ```
  Constructor.
  - *poiRetrievalFunc*: Type: CommonBehaviors.ActionsAddLanguageSpecificTextSet("LST74990FB5_0?cs=.|vb=.|cpp=::|nu=.|fs=.");RetrieveBotPoiDelegateThe poi retrieval function.

- **`ActionSetPoi Constructor (Boolean, RetrieveBotPoiDelegate)`**
  ```csharp
  public ActionSetPoi(
bool ignorePreviousPoi,
RetrieveBotPoiDelegate poiRetrievalFunc
)
  ```
  Constructor.
  - *ignorePreviousPoi*: Type: SystemAddLanguageSpecificTextSet("LST22CC3D8E_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Booleantrue to ignore previous poi.
  - *poiRetrievalFunc*: Type: CommonBehaviors.ActionsAddLanguageSpecificTextSet("LST22CC3D8E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");RetrieveBotPoiDelegate The poi retrieval function.

#### ActionSetPoi Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST5AD96C32_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionSetPoi Class

---

### DebugStringDelegate Delegate

```csharp
public delegate string DebugStringDelegate(
Object context
)
```

Debug string delegate.

---

### NavigationAction Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → CommonBehaviors.Actions.NavigationAction → Bots.Quest.Actions.Combat.ActionMoveToTarget → Levelbot.Actions.Combat.ActionMoveToTarget

```csharp
public class NavigationAction : Action
```

A navigation action.

#### NavigationAction Constructor

- **`NavigationAction Constructor (Func(Object, NavType))`**
  ```csharp
  public NavigationAction(
Func&lt;Object, NavType&gt; navType
)
  ```
  Constructor.
  - *navType*: Type: SystemAddLanguageSpecificTextSet("LST49C2A222_2?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST49C2A222_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, NavTypeAddLanguageSpecificTextSet("LST49C2A222_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Type of the navigaiton.

- **`NavigationAction Constructor (Func(Object, Vector3))`**
  ```csharp
  public NavigationAction(
Func&lt;Object, Vector3&gt; getPointDel
)
  ```
  Constructor.
  - *getPointDel*: Type: SystemAddLanguageSpecificTextSet("LST4511EFDE_2?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST4511EFDE_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, Vector3AddLanguageSpecificTextSet("LST4511EFDE_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");the get point delete.

- **`NavigationAction Constructor (Vector3)`**
  ```csharp
  public NavigationAction(
Vector3 point
)
  ```
  Constructor.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LST3BEC762D_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The point.

- **`NavigationAction Constructor (Func(Object, Vector3), Func(Object, NavType))`**
  ```csharp
  public NavigationAction(
Func&lt;Object, Vector3&gt; getPointDel = null,
Func&lt;Object, NavType&gt; navTypeDel = null
)
  ```
  Constructor.

#### NavigationAction Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTA07F280_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceNavigationAction Class

---

### NavigationInfo Class

**Inheritance:** System.Object → CommonBehaviors.Actions.NavigationInfo

```csharp
public class NavigationInfo
```

Information about the navigation.

#### NavigationInfo Properties

- **`Destination Property`**
  ```csharp
  public Vector3 Destination { get; }
  ```
  Gets the Destination for the.

- **`Height Property`**
  ```csharp
  public float Height { get; }
  ```
  Gets the height.

---

### RetrieveBotPoiDelegate Delegate

```csharp
public delegate BotPoi RetrieveBotPoiDelegate(
Object context
)
```

Retrieves bottom poi delegate.

---

### SleepForLagDuration Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Sleep → CommonBehaviors.Actions.SleepForLagDuration

```csharp
public class SleepForLagDuration : Sleep
```

A sleep for lag duration.

---
