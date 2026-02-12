# Levelbot.Actions.Death

Contains levelbot death related actions.

## Contents

- [ActionReleaseFromCorpse Class](#actionreleasefromcorpse-class)
- [ActionSuceedIfDeadOrGhost Class](#actionsuceedifdeadorghost-class)

---

### ActionReleaseFromCorpse Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → Levelbot.Actions.Death.ActionReleaseFromCorpse

```csharp
[ObsoleteAttribute("Use HandleDeathCoroutineTask")]
public class ActionReleaseFromCorpse : Action
```

An action release from corpse.

#### ActionReleaseFromCorpse Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST90EFADA6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionReleaseFromCorpse Class

---

### ActionSuceedIfDeadOrGhost Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → Levelbot.Actions.Death.ActionSuceedIfDeadOrGhost

```csharp
public class ActionSuceedIfDeadOrGhost : Action
```

An action suceed if dead or ghost.

#### ActionSuceedIfDeadOrGhost Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST535E89C5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionSuceedIfDeadOrGhost Class

---
