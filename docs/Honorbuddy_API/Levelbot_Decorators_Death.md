# Levelbot.Decorators.Death

Contains level bot death related decorators.

## Contents

- [DecoratorInstanceRelease Class](#decoratorinstancerelease-class)
- [DecoratorNeedToMoveToCorpse Class](#decoratorneedtomovetocorpse-class)
- [DecoratorNeedToRelease Class](#decoratorneedtorelease-class)
- [DecoratorNeedToTakeCorpse Class](#decoratorneedtotakecorpse-class)

---

### DecoratorInstanceRelease Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → Levelbot.Decorators.Death.DecoratorNeedToRelease → Levelbot.Decorators.Death.DecoratorInstanceRelease

```csharp
public class DecoratorInstanceRelease : DecoratorNeedToRelease
```

A decorator instance release.

#### DecoratorInstanceRelease Methods

- **`CanRun Method`**
  ```csharp
  protected override bool CanRun(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTC24E48F9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceDecoratorInstanceRelease Class

---

### DecoratorNeedToMoveToCorpse Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → Levelbot.Decorators.Death.DecoratorNeedToMoveToCorpse

```csharp
public class DecoratorNeedToMoveToCorpse : Decorator
```

A decorator need to move to corpse.

#### DecoratorNeedToMoveToCorpse Methods

- **`CanRun Method`**
  ```csharp
  protected override bool CanRun(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTB7F4BCE6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceDecoratorNeedToMoveToCorpse Class

---

### DecoratorNeedToRelease Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → Levelbot.Decorators.Death.DecoratorNeedToRelease → Levelbot.Decorators.Death.DecoratorInstanceRelease

```csharp
public class DecoratorNeedToRelease : Decorator
```

A decorator need to release.

#### DecoratorNeedToRelease Methods

- **`CanRun Method`**
  ```csharp
  protected override bool CanRun(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST65831309_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceDecoratorNeedToRelease Class

---

### DecoratorNeedToTakeCorpse Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → Levelbot.Decorators.Death.DecoratorNeedToTakeCorpse

```csharp
public class DecoratorNeedToTakeCorpse : Decorator
```

A decorator need to take corpse.

#### DecoratorNeedToTakeCorpse Methods

- **`CanRun Method`**
  ```csharp
  protected override bool CanRun(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST84E12B37_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceDecoratorNeedToTakeCorpse Class

---
