# Levelbot.Actions.Combat

Contains levelbot combat related actions.

## Contents

- [ActionMoveToKillPoi Class](#actionmovetokillpoi-class)
- [ActionMoveToTarget Class](#actionmovetotarget-class)
- [ActionSetTarget Class](#actionsettarget-class)

---

### ActionMoveToKillPoi Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → CommonBehaviors.Actions.ActionRunCoroutine → Levelbot.Actions.Combat.ActionMoveToKillPoi

```csharp
[ObsoleteAttribute("Use LevelBot.MoveToKillPoi instead")]
public class ActionMoveToKillPoi : ActionRunCoroutine
```

Initializes a new instance of the ActionMoveToKillPoi class

---

### ActionMoveToTarget Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → CommonBehaviors.Actions.NavigationAction → Levelbot.Actions.Combat.ActionMoveToTarget

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
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST753B38F4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionMoveToTarget Class

---

### ActionSetTarget Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → Levelbot.Actions.Combat.ActionSetTarget → Bots.Quest.Actions.Combat.ActionSetTarget

```csharp
[ObsoleteAttribute("Will be removed soon")]
public class ActionSetTarget : Action
```

An action set target.

#### ActionSetTarget Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST1741CCA2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionSetTarget Class

---
