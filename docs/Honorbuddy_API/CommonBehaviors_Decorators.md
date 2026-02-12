# CommonBehaviors.Decorators

Contains common decorators.

## Contents

- [DecoratorContextIs(T) Class](#decoratorcontextist-class)
- [DecoratorFrameIsVisible(T) Class](#decoratorframeisvisiblet-class)
- [DecoratorIsNotPoiType Class](#decoratorisnotpoitype-class)
- [DecoratorIsPoiType Class](#decoratorispoitype-class)

---

### DecoratorContextIs(T) Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → CommonBehaviors.Decorators.DecoratorContextIs.T

```csharp
public class DecoratorContextIs&lt;T&gt; : Decorator
```

A decorator context is.

#### DecoratorContextIs(T) Methods

- **`CanRun Method`**
  ```csharp
  protected override bool CanRun(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST23F81694_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceDecoratorContextIsAddLanguageSpecificTextSet("LST23F81694_4?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST23F81694_5?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### DecoratorFrameIsVisible(T) Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → CommonBehaviors.Decorators.DecoratorFrameIsVisible.T

```csharp
public class DecoratorFrameIsVisible&lt;T&gt; : Decorator
where T : new(), Frame
```

A decorator frame is visible.

#### DecoratorFrameIsVisible(T) Methods

- **`CanRun Method`**
  ```csharp
  protected override bool CanRun(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTEDF69BB2_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceDecoratorFrameIsVisibleAddLanguageSpecificTextSet("LSTEDF69BB2_4?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LSTEDF69BB2_5?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### DecoratorIsNotPoiType Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → CommonBehaviors.Decorators.DecoratorIsPoiType → CommonBehaviors.Decorators.DecoratorIsNotPoiType

```csharp
public class DecoratorIsNotPoiType : DecoratorIsPoiType
```

A decorator is not poi type.

#### DecoratorIsNotPoiType Constructor

- **`DecoratorIsNotPoiType Constructor (PoiType, Composite)`**
  ```csharp
  public DecoratorIsNotPoiType(
PoiType type,
Composite child
)
  ```
  Constructor.
  - *type*: Type: Styx.CommonBot.POIAddLanguageSpecificTextSet("LST3150D756_0?cs=.|vb=.|cpp=::|nu=.|fs=.");PoiType The type.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST3150D756_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe child.

- **`DecoratorIsNotPoiType Constructor (IEnumerable(PoiType), Composite)`**
  ```csharp
  public DecoratorIsNotPoiType(
IEnumerable&lt;PoiType&gt; types,
Composite child
)
  ```
  Constructor.
  - *types*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST6F3E5688_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST6F3E5688_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");PoiTypeAddLanguageSpecificTextSet("LST6F3E5688_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The types.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST6F3E5688_5?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe child.

#### DecoratorIsNotPoiType Methods

- **`CanRun Method`**
  ```csharp
  protected override bool CanRun(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTFB87A423_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceDecoratorIsNotPoiType Class

---

### DecoratorIsPoiType Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → CommonBehaviors.Decorators.DecoratorIsPoiType → CommonBehaviors.Decorators.DecoratorIsNotPoiType

```csharp
public class DecoratorIsPoiType : Decorator
```

A decorator is poi type.

#### DecoratorIsPoiType Constructor

- **`DecoratorIsPoiType Constructor (PoiType, Composite)`**
  ```csharp
  public DecoratorIsPoiType(
PoiType type,
Composite child
)
  ```
  Constructor.
  - *type*: Type: Styx.CommonBot.POIAddLanguageSpecificTextSet("LST95FD9D7D_0?cs=.|vb=.|cpp=::|nu=.|fs=.");PoiType The type.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST95FD9D7D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe child.

- **`DecoratorIsPoiType Constructor (IEnumerable(PoiType), Composite)`**
  ```csharp
  public DecoratorIsPoiType(
IEnumerable&lt;PoiType&gt; types,
Composite child
)
  ```
  Constructor.
  - *types*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST4C31D0E3_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST4C31D0E3_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");PoiTypeAddLanguageSpecificTextSet("LST4C31D0E3_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The types.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST4C31D0E3_5?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe child.

#### DecoratorIsPoiType Methods

- **`CanRun Method`**
  ```csharp
  protected override bool CanRun(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST124288EC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceDecoratorIsPoiType Class

---
