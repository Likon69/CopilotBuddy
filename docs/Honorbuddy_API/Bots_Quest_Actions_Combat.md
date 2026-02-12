# Bots.Quest.Actions.Combat

Contains quest bot combat related actions.

## Contents

- [ActionMoveToTarget Class](#actionmovetotarget-class)
- [ActionPull Class](#actionpull-class)
- [ActionSetTarget Class](#actionsettarget-class)

---

### ActionMoveToTarget Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → CommonBehaviors.Actions.NavigationAction → Bots.Quest.Actions.Combat.ActionMoveToTarget

```csharp
public class ActionMoveToTarget : NavigationAction
```

An action move to target.

#### ActionMoveToTarget Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTF3F576D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionMoveToTarget Class

---

### ActionPull Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → Bots.Quest.Actions.Combat.ActionPull

```csharp
[ObsoleteAttribute("Do not use. Will be removed in the future.")]
public class ActionPull : Action
```

An action pull.

#### ActionPull Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTC1021AB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionPull Class

---

### ActionSetTarget Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → Levelbot.Actions.Combat.ActionSetTarget → Bots.Quest.Actions.Combat.ActionSetTarget

```csharp
[ObsoleteAttribute("Will be removed soon")]
public class ActionSetTarget : ActionSetTarget
```

An action set target.

---
