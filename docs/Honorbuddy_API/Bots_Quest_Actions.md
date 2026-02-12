# Bots.Quest.Actions

Contains quest bot related actions.

## Contents

- [ActionSelectActiveQuest Class](#actionselectactivequest-class)
- [ActionSelectAvailableQuest Class](#actionselectavailablequest-class)
- [ForcedBehaviorExecutor Class](#forcedbehaviorexecutor-class)

---

### ActionSelectActiveQuest Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → Bots.Quest.Actions.ActionSelectActiveQuest

```csharp
public class ActionSelectActiveQuest : Action
```

An action select active question.

#### ActionSelectActiveQuest Constructor

- **`ActionSelectActiveQuest Constructor (IEnumerable(Int32))`**
  ```csharp
  public ActionSelectActiveQuest(
IEnumerable&lt;int&gt; variantQuestIds
)
  ```
  Constructor.
  - *variantQuestIds*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST105683C1_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST105683C1_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Int32AddLanguageSpecificTextSet("LST105683C1_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The variant identifiers.

- **`ActionSelectActiveQuest Constructor (Int32)`**
  ```csharp
  public ActionSelectActiveQuest(
int id
)
  ```
  Initializes a new instance of the ActionSelectActiveQuest class
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST832B1777_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32

#### ActionSelectActiveQuest Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST6A4366EE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionSelectActiveQuest Class

---

### ActionSelectAvailableQuest Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → Bots.Quest.Actions.ActionSelectAvailableQuest

```csharp
public class ActionSelectAvailableQuest : Action
```

An action select available question.

#### ActionSelectAvailableQuest Constructor

- **`ActionSelectAvailableQuest Constructor (IEnumerable(Int32))`**
  ```csharp
  public ActionSelectAvailableQuest(
IEnumerable&lt;int&gt; variantQuestIds
)
  ```
  Constructor.
  - *variantQuestIds*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST735708EE_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST735708EE_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Int32AddLanguageSpecificTextSet("LST735708EE_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The variant identifier.

- **`ActionSelectAvailableQuest Constructor (Int32)`**
  ```csharp
  public ActionSelectAvailableQuest(
int id
)
  ```
  Initializes a new instance of the ActionSelectAvailableQuest class
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST50B0959E_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32

#### ActionSelectAvailableQuest Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST19A4DE05_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionSelectAvailableQuest Class

---

### ForcedBehaviorExecutor Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Bots.Quest.Actions.ForcedBehaviorExecutor

```csharp
public class ForcedBehaviorExecutor : Composite
```

A forced behavior executor.

#### ForcedBehaviorExecutor Properties

- **`Order Property`**
  ```csharp
  public QuestOrder Order { get; }
  ```
  Gets the order.

#### ForcedBehaviorExecutor Methods

- **`Execute Method`**
  ```csharp
  protected override IEnumerable&lt;RunStatus&gt; Execute(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST57E534D4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** See Also

---
