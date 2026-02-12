# Styx.Pathing.Avoidance

Contains avoidance related classes.

## Contents

- [Avoid Class](#avoid-class)
- [Avoid(T) Class](#avoidt-class)
- [AvoidCircle(T) Class](#avoidcirclet-class)
- [AvoidCircleInfo(T) Class](#avoidcircleinfot-class)
- [AvoidInfo Class](#avoidinfo-class)
- [AvoidInfo(T) Class](#avoidinfot-class)
- [AvoidLocation Class](#avoidlocation-class)
- [AvoidLocation(T) Class](#avoidlocationt-class)
- [AvoidLocationInfo Class](#avoidlocationinfo-class)
- [AvoidLocationInfo(T) Class](#avoidlocationinfot-class)
- [AvoidObject Class](#avoidobject-class)
- [AvoidObject(T) Class](#avoidobjectt-class)
- [AvoidObjectInfo Class](#avoidobjectinfo-class)
- [AvoidObjectInfo(T) Class](#avoidobjectinfot-class)
- [AvoidPolygon(T) Class](#avoidpolygont-class)
- [AvoidPolygonInfo(T) Class](#avoidpolygoninfot-class)
- [AvoidanceManager Class](#avoidancemanager-class)
- [AvoidancePriority Enumeration](#avoidancepriority-enumeration)

---

### Avoid Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.Avoid → Styx.Pathing.Avoidance.Avoid.T

```csharp
public abstract class Avoid
```

Initializes a new instance of the Avoid class

#### Avoid Properties

- **`AvoidInfo Property`**
  ```csharp
  public AvoidInfo AvoidInfo { get; }
  ```

- **`Bounds Property`**
  ```csharp
  public BoundingBox3 Bounds { get; }
  ```

- **`IgnoreIfBlocking Property`**
  ```csharp
  public bool IgnoreIfBlocking { get; }
  ```
  Gets a value indicating whether to ignore avoid if no avoid path is found.

- **`IsBlocking Property`**
  ```csharp
  public bool IsBlocking { get; }
  ```
  Gets a value indicating whether this avoid is blocking path.

- **`IsValid Property`**
  ```csharp
  public bool IsValid { get; protected set; }
  ```
  Gets a value indicating whether this instance is valid.

- **`LeashPoint Property`**
  ```csharp
  public Vector3 LeashPoint { get; }
  ```
  Gets the leash point.

- **`LeashRadius Property`**
  ```csharp
  public float LeashRadius { get; }
  ```
  Gets the leash radius.

- **`LeashRadiusSqr Property`**
  ```csharp
  public float LeashRadiusSqr { get; }
  ```
  Gets the leash radius squared.

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; protected set; }
  ```
  Gets the location.

- **`Object Property`**
  ```csharp
  public Object Object { get; }
  ```

- **`ShouldIgnore Property`**
  ```csharp
  public bool ShouldIgnore { get; }
  ```
  Gets a value indicating whether [should ignore].

#### Avoid Methods

- **`CreateBounds Method`**
  ```csharp
  protected abstract BoundingBox3 CreateBounds()
  ```
  - **Returns:** ReferenceAvoid Class

- **`IsPointInAvoid Method`**
  ```csharp
  public abstract bool IsPointInAvoid(
Vector3 point
)
  ```
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LSTDBD9481B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - **Returns:** ReferenceAvoid Class

- **`OnUpdate Method`**
  ```csharp
  protected abstract void OnUpdate(
out bool updateBounds
)
  ```
  Updates internal avoid state. Only called if Object is valid and after location has been updated.
  - *updateBounds*: Type: SystemAddLanguageSpecificTextSet("LSTD676506F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanAddLanguageSpecificTextSet("LSTD676506F_2?cpp=%");

---

### Avoid(T) Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.Avoid → Styx.Pathing.Avoidance.Avoid.T → Styx.Pathing.Avoidance.AvoidCircle.T → Styx.Pathing.Avoidance.AvoidPolygon.T

```csharp
public abstract class Avoid&lt;T&gt; : Avoid
```

Initializes a new instance of the Avoid.T class

#### Avoid(T) Properties

- **`AvoidInfo Property`**
  ```csharp
  public AvoidInfo&lt;T&gt; AvoidInfo { get; }
  ```

- **`Object Property`**
  ```csharp
  public T Object { get; }
  ```

---

### AvoidCircle(T) Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.Avoid → Styx.Pathing.Avoidance.Avoid.T → Styx.Pathing.Avoidance.AvoidCircle.T → Styx.Pathing.Avoidance.AvoidLocation → Styx.Pathing.Avoidance.AvoidLocation.T → Styx.Pathing.Avoidance.AvoidObject → Styx.Pathing.Avoidance.AvoidObject.T

```csharp
public class AvoidCircle&lt;T&gt; : Avoid&lt;T&gt;
```

The height from bottom of a cylinder to the top

#### AvoidCircle(T) Properties

- **`AvoidInfo Property`**
  ```csharp
  public AvoidCircleInfo&lt;T&gt; AvoidInfo { get; }
  ```

- **`Height Property`**
  ```csharp
  public float Height { get; }
  ```
  The height from bottom of a cylinder to the top

- **`HeightfieldHeight Property`**
  ```csharp
  protected float HeightfieldHeight { get; }
  ```

- **`HeightfieldLocation Property`**
  ```csharp
  protected Vector3 HeightfieldLocation { get; set; }
  ```
  The location used when this instance was added to height field. We'll use it again when we remove

- **`HeightfieldRadius Property`**
  ```csharp
  protected float HeightfieldRadius { get; }
  ```

- **`Radius Property`**
  ```csharp
  public float Radius { get; }
  ```

#### AvoidCircle(T) Methods

- **`CreateBounds Method`**
  ```csharp
  protected override BoundingBox3 CreateBounds()
  ```
  - **Returns:** ReferenceAvoidCircleAddLanguageSpecificTextSet("LSTB2BA213F_3?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LSTB2BA213F_4?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

- **`IsPointInAvoid Method`**
  ```csharp
  public override sealed bool IsPointInAvoid(
Vector3 point
)
  ```
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LST5CDE1CA6_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - **Returns:** ReferenceAvoidCircleAddLanguageSpecificTextSet("LST5CDE1CA6_4?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST5CDE1CA6_5?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

- **`OnUpdate Method`**
  ```csharp
  protected override sealed void OnUpdate(
out bool updateBounds
)
  ```
  - *updateBounds*: Type: SystemAddLanguageSpecificTextSet("LSTE8EBECEE_3?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanAddLanguageSpecificTextSet("LSTE8EBECEE_4?cpp=%");

---

### AvoidCircleInfo(T) Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.AvoidInfo → Styx.Pathing.Avoidance.AvoidInfo.T → Styx.Pathing.Avoidance.AvoidCircleInfo.T → Styx.Pathing.Avoidance.AvoidLocationInfo → Styx.Pathing.Avoidance.AvoidLocationInfo.T → Styx.Pathing.Avoidance.AvoidObjectInfo → Styx.Pathing.Avoidance.AvoidObjectInfo.T

```csharp
public class AvoidCircleInfo&lt;T&gt; : AvoidInfo&lt;T&gt;
```

Initializes a new instance of the AvoidInfo class.

#### AvoidCircleInfo(T) Properties

- **`HeightProducer Property`**
  ```csharp
  public Func&lt;T, float&gt; HeightProducer { get; protected set; }
  ```

- **`RadiusProducer Property`**
  ```csharp
  public Func&lt;T, float&gt; RadiusProducer { get; }
  ```

---

### AvoidInfo Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.AvoidInfo → Styx.Pathing.Avoidance.AvoidInfo.T

```csharp
public abstract class AvoidInfo
```

Initializes a new instance of the AvoidInfo class.

#### AvoidInfo Properties

- **`CanRun Property`**
  ```csharp
  public bool CanRun { get; }
  ```
  Determines whether this instance can run.
  - **Returns:** See Also

- **`Collection Property`**
  ```csharp
  public List&lt;Object&gt; Collection { get; }
  ```

- **`Condition Property`**
  ```csharp
  public Func&lt;bool&gt; Condition { get; }
  ```
  Gets the condition.

- **`IgnoreIfBlocking Property`**
  ```csharp
  public bool IgnoreIfBlocking { get; }
  ```
  Gets a value indicating whether to ignore avoid if no avoid path is found.

- **`LeashPointProducer Property`**
  ```csharp
  public Func&lt;Vector3&gt; LeashPointProducer { get; }
  ```
  Gets the leash point selector.

- **`LeashRadius Property`**
  ```csharp
  public float LeashRadius { get; }
  ```
  Gets the leash radius.

- **`LocationProducer Property`**
  ```csharp
  public Func&lt;Object, Vector3&gt; LocationProducer { get; }
  ```

- **`ObjectValidator Property`**
  ```csharp
  public Func&lt;Object, bool&gt; ObjectValidator { get; }
  ```

- **`Priority Property`**
  ```csharp
  public AvoidancePriority Priority { get; }
  ```

---

### AvoidInfo(T) Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.AvoidInfo → Styx.Pathing.Avoidance.AvoidInfo.T → Styx.Pathing.Avoidance.AvoidCircleInfo.T → Styx.Pathing.Avoidance.AvoidPolygonInfo.T

```csharp
public abstract class AvoidInfo&lt;T&gt; : AvoidInfo
```

Initializes a new instance of the AvoidInfo class.

#### AvoidInfo(T) Properties

- **`CollectionProducer Property`**
  ```csharp
  public Func&lt;IEnumerable&lt;T&gt;&gt; CollectionProducer { get; }
  ```

- **`Condition Property`**
  ```csharp
  public Func&lt;bool&gt; Condition { get; }
  ```
  Gets the condition.

- **`LeashPointProducer Property`**
  ```csharp
  public Func&lt;Vector3&gt; LeashPointProducer { get; }
  ```
  Gets the leash point selector.

- **`LocationProducer Property`**
  ```csharp
  public Func&lt;T, Vector3&gt; LocationProducer { get; }
  ```

- **`ObjectValidator Property`**
  ```csharp
  public Func&lt;T, bool&gt; ObjectValidator { get; }
  ```

---

### AvoidLocation Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.Avoid → Styx.Pathing.Avoidance.Avoid.Object → Styx.Pathing.Avoidance.AvoidCircle.Object → Styx.Pathing.Avoidance.AvoidLocation

```csharp
public class AvoidLocation : AvoidCircle&lt;Object&gt;
```

Initializes a new instance of the AvoidLocation class.

---

### AvoidLocation(T) Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.Avoid → Styx.Pathing.Avoidance.Avoid.T → Styx.Pathing.Avoidance.AvoidCircle.T → Styx.Pathing.Avoidance.AvoidLocation.T

```csharp
public class AvoidLocation&lt;T&gt; : AvoidCircle&lt;T&gt;
```

Initializes a new instance of the AvoidLocation class.

---

### AvoidLocationInfo Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.AvoidInfo → Styx.Pathing.Avoidance.AvoidInfo.Object → Styx.Pathing.Avoidance.AvoidCircleInfo.Object → Styx.Pathing.Avoidance.AvoidLocationInfo

```csharp
public class AvoidLocationInfo : AvoidCircleInfo&lt;Object&gt;
```

Initializes a new instance of the AvoidLocationInfo class.

---

### AvoidLocationInfo(T) Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.AvoidInfo → Styx.Pathing.Avoidance.AvoidInfo.T → Styx.Pathing.Avoidance.AvoidCircleInfo.T → Styx.Pathing.Avoidance.AvoidLocationInfo.T

```csharp
public class AvoidLocationInfo&lt;T&gt; : AvoidCircleInfo&lt;T&gt;
```

Initializes a new instance of the AvoidObjectInfo class.

---

### AvoidObject Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.Avoid → Styx.Pathing.Avoidance.Avoid.WoWObject → Styx.Pathing.Avoidance.AvoidCircle.WoWObject → Styx.Pathing.Avoidance.AvoidObject

```csharp
public class AvoidObject : AvoidCircle&lt;WoWObject&gt;
```

Initializes a new instance of the AvoidObject class

#### AvoidObject Properties

- **`AvoidInfo Property`**
  ```csharp
  public AvoidObjectInfo AvoidInfo { get; }
  ```

---

### AvoidObject(T) Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.Avoid → Styx.Pathing.Avoidance.Avoid.T → Styx.Pathing.Avoidance.AvoidCircle.T → Styx.Pathing.Avoidance.AvoidObject.T

```csharp
public class AvoidObject&lt;T&gt; : AvoidCircle&lt;T&gt;
where T : WoWObject
```

Initializes a new instance of the AvoidObject.T class

#### AvoidObject(T) Properties

- **`AvoidInfo Property`**
  ```csharp
  public AvoidObjectInfo&lt;T&gt; AvoidInfo { get; }
  ```

---

### AvoidObjectInfo Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.AvoidInfo → Styx.Pathing.Avoidance.AvoidInfo.WoWObject → Styx.Pathing.Avoidance.AvoidCircleInfo.WoWObject → Styx.Pathing.Avoidance.AvoidObjectInfo

```csharp
public class AvoidObjectInfo : AvoidCircleInfo&lt;WoWObject&gt;
```

Initializes a new instance of the AvoidObjectInfo class.

---

### AvoidObjectInfo(T) Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.AvoidInfo → Styx.Pathing.Avoidance.AvoidInfo.T → Styx.Pathing.Avoidance.AvoidCircleInfo.T → Styx.Pathing.Avoidance.AvoidObjectInfo.T

```csharp
public class AvoidObjectInfo&lt;T&gt; : AvoidCircleInfo&lt;T&gt;
where T : WoWObject
```

Initializes a new instance of the AvoidObjectInfo class.

---

### AvoidPolygon(T) Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.Avoid → Styx.Pathing.Avoidance.Avoid.T → Styx.Pathing.Avoidance.AvoidPolygon.T

```csharp
public class AvoidPolygon&lt;T&gt; : Avoid&lt;T&gt;
```

The height from bottom of a cylinder to the top

#### AvoidPolygon(T) Properties

- **`AvoidInfo Property`**
  ```csharp
  public AvoidPolygonInfo&lt;T&gt; AvoidInfo { get; }
  ```

- **`Height Property`**
  ```csharp
  public float Height { get; }
  ```
  The height from bottom of a cylinder to the top

- **`HeightfieldHeight Property`**
  ```csharp
  protected float HeightfieldHeight { get; }
  ```

- **`HeightfieldLocation Property`**
  ```csharp
  protected Vector3 HeightfieldLocation { get; }
  ```

- **`HeightfieldPoints Property`**
  ```csharp
  protected Vector2[] HeightfieldPoints { get; set; }
  ```
  The location used when this instance was added to height field. We'll use it again when we remove

- **`Points Property`**
  ```csharp
  public Vector2[] Points { get; }
  ```

- **`Rotation Property`**
  ```csharp
  public float Rotation { get; }
  ```

- **`Scale Property`**
  ```csharp
  public float Scale { get; }
  ```

#### AvoidPolygon(T) Methods

- **`CreateBounds Method`**
  ```csharp
  protected override BoundingBox3 CreateBounds()
  ```
  - **Returns:** ReferenceAvoidPolygonAddLanguageSpecificTextSet("LSTE17E4631_3?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LSTE17E4631_4?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

- **`IsPointInAvoid Method`**
  ```csharp
  public override sealed bool IsPointInAvoid(
Vector3 point
)
  ```
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LSTF9175452_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - **Returns:** ReferenceAvoidPolygonAddLanguageSpecificTextSet("LSTF9175452_4?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LSTF9175452_5?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

- **`OnUpdate Method`**
  ```csharp
  protected override sealed void OnUpdate(
out bool updateBounds
)
  ```
  - *updateBounds*: Type: SystemAddLanguageSpecificTextSet("LSTF8938ACE_3?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanAddLanguageSpecificTextSet("LSTF8938ACE_4?cpp=%");

---

### AvoidPolygonInfo(T) Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.AvoidInfo → Styx.Pathing.Avoidance.AvoidInfo.T → Styx.Pathing.Avoidance.AvoidPolygonInfo.T

```csharp
public class AvoidPolygonInfo&lt;T&gt; : AvoidInfo&lt;T&gt;
```

Initializes a new instance of the AvoidPolygonInfo.T class.

#### AvoidPolygonInfo(T) Properties

- **`HeightProducer Property`**
  ```csharp
  public Func&lt;T, float&gt; HeightProducer { get; protected set; }
  ```

- **`PointsProducer Property`**
  ```csharp
  public Func&lt;T, Vector2[]&gt; PointsProducer { get; }
  ```

- **`RotationProducer Property`**
  ```csharp
  public Func&lt;T, float&gt; RotationProducer { get; }
  ```

- **`ScaleProducer Property`**
  ```csharp
  public Func&lt;T, float&gt; ScaleProducer { get; }
  ```

---

### AvoidanceManager Class

**Inheritance:** System.Object → Styx.Pathing.Avoidance.AvoidanceManager

```csharp
public static class AvoidanceManager
```

Gets a value indicating whether toon is running out of an area being avoided.

#### AvoidanceManager Properties

- **`AvoidInfos Property`**
  ```csharp
  public static List&lt;AvoidInfo&gt; AvoidInfos { get; }
  ```

- **`Avoids Property`**
  ```csharp
  public static List&lt;Avoid&gt; Avoids { get; }
  ```

- **`IsRunningOutOfAvoid Property`**
  ```csharp
  public static bool IsRunningOutOfAvoid { get; }
  ```
  Gets a value indicating whether toon is running out of an area being avoided.

#### AvoidanceManager Methods

- **`AddAvoid Method`**
  ```csharp
  public static void AddAvoid(
AvoidInfo avoid
)
  ```
  - *avoid*: Type: Styx.Pathing.AvoidanceAddLanguageSpecificTextSet("LST3931216A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");AvoidInfo

- **`AddAvoidLocation Method`**
  Run away from location.

- **`AddAvoidLocation Method (Func(Boolean), Single, Func(Vector3), Boolean)`**
  ```csharp
  public static AvoidInfo AddAvoidLocation(
Func&lt;bool&gt; canRun,
float radius,
Func&lt;Vector3&gt; locationProducer,
bool ignoreIfBlocking = false
)
  ```
  Run away from location.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST59B81B5D_5?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST59B81B5D_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST59B81B5D_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST59B81B5D_8?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe radius.
  - *locationProducer*: Type: SystemAddLanguageSpecificTextSet("LST59B81B5D_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST59B81B5D_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST59B81B5D_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The location selector.
  - **Returns:** ReferenceAvoidanceManager Class

- **`AddAvoidLocation Method (Func(Boolean), Func(Vector3), Single, Single, Func(Vector3), Boolean)`**
  ```csharp
  public static AvoidInfo AddAvoidLocation(
Func&lt;bool&gt; canRun,
Func&lt;Vector3&gt; leashPointProducer,
float leashRadius,
float radius,
Func&lt;Vector3&gt; locationProducer,
bool ignoreIfBlocking = false
)
  ```
  Run away from location.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTCE0A50_7?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTCE0A50_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTCE0A50_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - *leashPointProducer*: Type: SystemAddLanguageSpecificTextSet("LSTCE0A50_10?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTCE0A50_11?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTCE0A50_12?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The leash point selector.
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LSTCE0A50_13?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe leash radius.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LSTCE0A50_14?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe max distance to run.
  - *locationProducer*: Type: SystemAddLanguageSpecificTextSet("LSTCE0A50_15?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTCE0A50_16?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTCE0A50_17?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The location selector.
  - **Returns:** ReferenceAvoidanceManager Class

- **`AddAvoidLocation(T) Method (Func(Boolean), Func(T, Single), Func(T, Vector3), Func(IEnumerable(T)), Func(T, Boolean), Boolean)`**
  ```csharp
  public static AvoidInfo AddAvoidLocation&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;T, float&gt; radiusProducer,
Func&lt;T, Vector3&gt; locationProducer,
Func&lt;IEnumerable&lt;T&gt;&gt; collectionProducer,
Func&lt;T, bool&gt; objectValidator = null,
bool ignoreIfBlocking = false
)
  ```
  Run away from location.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTC57CFFFA_15?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTC57CFFFA_16?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTC57CFFFA_17?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The condition.
  - *radiusProducer*: Type: SystemAddLanguageSpecificTextSet("LSTC57CFFFA_18?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTC57CFFFA_19?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LSTC57CFFFA_20?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The max distance to run.
  - *locationProducer*: Type: SystemAddLanguageSpecificTextSet("LSTC57CFFFA_21?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTC57CFFFA_22?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, Vector3AddLanguageSpecificTextSet("LSTC57CFFFA_23?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The location selector.
  - *collectionProducer*: Type: SystemAddLanguageSpecificTextSet("LSTC57CFFFA_24?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTC57CFFFA_25?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");IEnumerableAddLanguageSpecificTextSet("LSTC57CFFFA_26?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTC57CFFFA_27?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");AddLanguageSpecificTextSet("LSTC57CFFFA_28?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); Optional collection of objects that are passed as args to locationProducer
  - **Returns:** ReferenceAvoidanceManager Class

- **`AddAvoidLocation(T) Method (Func(Boolean), Func(Vector3), Single, Func(T, Single), Func(T, Vector3), Func(IEnumerable(T)), Func(T, Boolean), Boolean)`**
  ```csharp
  public static AvoidInfo AddAvoidLocation&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;Vector3&gt; leashPointProducer,
float leashRadius,
Func&lt;T, float&gt; radiusProducer,
Func&lt;T, Vector3&gt; locationProducer,
Func&lt;IEnumerable&lt;T&gt;&gt; collectionProducer,
Func&lt;T, bool&gt; objectValidator = null,
bool ignoreIfBlocking = false
)
  ```
  Run away from location.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTBA84A967_17?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTBA84A967_18?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTBA84A967_19?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - *leashPointProducer*: Type: SystemAddLanguageSpecificTextSet("LSTBA84A967_20?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTBA84A967_21?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTBA84A967_22?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The leash point selector.
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LSTBA84A967_23?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe leash radius.
  - *radiusProducer*: Type: SystemAddLanguageSpecificTextSet("LSTBA84A967_24?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTBA84A967_25?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LSTBA84A967_26?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The max distance to run.
  - *locationProducer*: Type: SystemAddLanguageSpecificTextSet("LSTBA84A967_27?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTBA84A967_28?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, Vector3AddLanguageSpecificTextSet("LSTBA84A967_29?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The location selector.
  - *collectionProducer*: Type: SystemAddLanguageSpecificTextSet("LSTBA84A967_30?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTBA84A967_31?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");IEnumerableAddLanguageSpecificTextSet("LSTBA84A967_32?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTBA84A967_33?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");AddLanguageSpecificTextSet("LSTBA84A967_34?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Optional collection of objects that are passed as args to locationProducer
  - **Returns:** ReferenceAvoidanceManager Class

- **`AddAvoidObject Method`**
  Adds the avoid object.

- **`AddAvoidObject(T) Method (Func(Boolean), Func(T, Single), UInt32[])`**
  ```csharp
  public static AvoidInfo AddAvoidObject&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;T, float&gt; radiusProducer,
params uint[] unitIds
)
where T : WoWObject
  ```
  Adds the avoid object.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTA273AAA6_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTA273AAA6_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTA273AAA6_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The can run.
  - *radiusProducer*: Type: SystemAddLanguageSpecificTextSet("LSTA273AAA6_12?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTA273AAA6_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LSTA273AAA6_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The radius selector.
  - *unitIds*: Type: AddLanguageSpecificTextSet("LSTA273AAA6_15?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTA273AAA6_16?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32AddLanguageSpecificTextSet("LSTA273AAA6_17?cpp=&gt;|vb=()|nu=[]");The unit ids.
  - **Returns:** ReferenceAvoidanceManager Class

- **`AddAvoidObject(T) Method (Func(Boolean), Single, UInt32[])`**
  ```csharp
  public static AvoidInfo AddAvoidObject&lt;T&gt;(
Func&lt;bool&gt; canRun,
float radius,
params uint[] unitIds
)
where T : WoWObject
  ```
  Adds the avoid object.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST658602DD_7?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST658602DD_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST658602DD_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The can run.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST658602DD_10?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe radius.
  - *unitIds*: Type: AddLanguageSpecificTextSet("LST658602DD_11?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST658602DD_12?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32AddLanguageSpecificTextSet("LST658602DD_13?cpp=&gt;|vb=()|nu=[]");The unit ids.
  - **Returns:** ReferenceAvoidanceManager Class

- **`AddAvoidObject(T) Method (Func(Boolean), Func(Vector3), Single, Func(T, Single), UInt32[])`**
  ```csharp
  public static AvoidInfo AddAvoidObject&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;Vector3&gt; leashPointProducer,
float leashRadius,
Func&lt;T, float&gt; radiusProducer,
params uint[] unitIds
)
where T : WoWObject
  ```
  Runs away from a unit or object if within range.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST696AF6FB_11?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST696AF6FB_12?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST696AF6FB_13?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The 'can run' condition.
  - *leashPointProducer*: Type: SystemAddLanguageSpecificTextSet("LST696AF6FB_14?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST696AF6FB_15?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST696AF6FB_16?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The leash center point selector. Unit location if null
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LST696AF6FB_17?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe max distance to run from leash.
  - *radiusProducer*: Type: SystemAddLanguageSpecificTextSet("LST696AF6FB_18?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST696AF6FB_19?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LST696AF6FB_20?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The distance to avoid
  - *unitIds*: Type: AddLanguageSpecificTextSet("LST696AF6FB_21?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST696AF6FB_22?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32AddLanguageSpecificTextSet("LST696AF6FB_23?cpp=&gt;|vb=()|nu=[]");The unit ids.
  - **Returns:** ReferenceAvoidanceManager Class

- **`AddAvoidObject(T) Method (Func(Boolean), Func(Vector3), Single, Single, UInt32[])`**
  ```csharp
  public static AvoidInfo AddAvoidObject&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;Vector3&gt; leashPointProducer,
float leashRadius,
float radius,
params uint[] unitIds
)
where T : WoWObject
  ```
  Runs away from a unit or object if within range.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST53EED6C8_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST53EED6C8_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST53EED6C8_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The 'can run' condition.
  - *leashPointProducer*: Type: SystemAddLanguageSpecificTextSet("LST53EED6C8_12?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST53EED6C8_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST53EED6C8_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The leash center point selector. Unit location if null
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LST53EED6C8_15?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe max distance to run from leash.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST53EED6C8_16?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe radius.
  - *unitIds*: Type: AddLanguageSpecificTextSet("LST53EED6C8_17?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST53EED6C8_18?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32AddLanguageSpecificTextSet("LST53EED6C8_19?cpp=&gt;|vb=()|nu=[]");The unit ids.
  - **Returns:** ReferenceAvoidanceManager Class

- **`AddAvoidObject(T) Method (Func(Boolean), Func(T, Single), Predicate(T), Func(T, Vector3), Boolean)`**
  ```csharp
  public static AvoidInfo AddAvoidObject&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;T, float&gt; radiusProducer,
Predicate&lt;T&gt; objectSelector,
Func&lt;T, Vector3&gt; locationProducer = null,
bool ignoreIfBlocking = false
)
where T : WoWObject
  ```
  Runs away from a unit or object if within range.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST6DAEE5C7_11?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST6DAEE5C7_12?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST6DAEE5C7_13?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The 'can run' condition.
  - *radiusProducer*: Type: SystemAddLanguageSpecificTextSet("LST6DAEE5C7_14?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST6DAEE5C7_15?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LST6DAEE5C7_16?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The distance to avoid
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LST6DAEE5C7_17?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LST6DAEE5C7_18?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST6DAEE5C7_19?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The object selector.
  - **Returns:** ReferenceAvoidanceManager Class

- **`AddAvoidObject(T) Method (Func(Boolean), Single, Predicate(T), Func(T, Vector3), Boolean)`**
  ```csharp
  public static AvoidInfo AddAvoidObject&lt;T&gt;(
Func&lt;bool&gt; canRun,
float radius,
Predicate&lt;T&gt; objectSelector,
Func&lt;T, Vector3&gt; locationProducer = null,
bool ignoreIfBlocking = false
)
where T : WoWObject
  ```
  Adds the avoid object.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST1F1C4ABC_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1F1C4ABC_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST1F1C4ABC_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The can run.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST1F1C4ABC_12?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe radius.
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LST1F1C4ABC_13?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LST1F1C4ABC_14?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST1F1C4ABC_15?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The object selector.
  - **Returns:** ReferenceAvoidanceManager Class

- **`AddAvoidObject(T) Method (Func(Boolean), Func(Vector3), Single, Func(T, Single), Predicate(T), Func(T, Vector3), Boolean)`**
  ```csharp
  public static AvoidInfo AddAvoidObject&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;Vector3&gt; leashPointProducer,
float leashRadius,
Func&lt;T, float&gt; radiusProducer,
Predicate&lt;T&gt; objectSelector,
Func&lt;T, Vector3&gt; locationProducer = null,
bool ignoreIfBlocking = false
)
where T : WoWObject
  ```
  Runs away from a unit or object if within range.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST84FA740_13?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST84FA740_14?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST84FA740_15?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The 'can run' condition.
  - *leashPointProducer*: Type: SystemAddLanguageSpecificTextSet("LST84FA740_16?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST84FA740_17?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST84FA740_18?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The leash center point selector. Unit location if null
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LST84FA740_19?cs=.|vb=.|cpp=::|nu=.|fs=.");Single The max distance to run from leash.
  - *radiusProducer*: Type: SystemAddLanguageSpecificTextSet("LST84FA740_20?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST84FA740_21?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LST84FA740_22?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The distance to avoid
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LST84FA740_23?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LST84FA740_24?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST84FA740_25?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The object selector.
  - **Returns:** ReferenceAvoidanceManager Class

- **`AddAvoidObject(T) Method (Func(Boolean), Func(Vector3), Single, Single, Predicate(T), Func(T, Vector3), Boolean)`**
  ```csharp
  public static AvoidInfo AddAvoidObject&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;Vector3&gt; leashPointProducer,
float leashRadius,
float radius,
Predicate&lt;T&gt; objectSelector,
Func&lt;T, Vector3&gt; locationProducer = null,
bool ignoreIfBlocking = false
)
where T : WoWObject
  ```
  Runs away from a unit or object if within range.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST1A2AC195_11?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1A2AC195_12?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST1A2AC195_13?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The 'can run' condition.
  - *leashPointProducer*: Type: SystemAddLanguageSpecificTextSet("LST1A2AC195_14?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1A2AC195_15?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST1A2AC195_16?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The leash center point selector. Unit location if null
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LST1A2AC195_17?cs=.|vb=.|cpp=::|nu=.|fs=.");Single The max distance to run from leash.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST1A2AC195_18?cs=.|vb=.|cpp=::|nu=.|fs=.");Single The distance to avoid
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LST1A2AC195_19?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LST1A2AC195_20?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST1A2AC195_21?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The object selector.
  - **Returns:** ReferenceAvoidanceManager Class

- **`AddAvoidPolygon(T) Method`**
  ```csharp
  public static AvoidInfo AddAvoidPolygon&lt;T&gt;(
Func&lt;bool&gt; condition,
Func&lt;Vector3&gt; leashPointProducer,
float leashRadius,
Func&lt;T, float&gt; rotationProducer,
Func&lt;T, float&gt; scaleProducer,
Func&lt;T, float&gt; heightProducer,
Func&lt;T, Vector2[]&gt; pointsProducer,
Func&lt;T, Vector3&gt; locationProducer,
Func&lt;IEnumerable&lt;T&gt;&gt; collectionProducer,
Func&lt;T, bool&gt; objectValidator = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
  ```
  Initializes a new instance of the AvoidPolygonInfo.T class.
  - *condition*: Type: SystemAddLanguageSpecificTextSet("LST1F687C6E_5?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1F687C6E_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST1F687C6E_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - *leashPointProducer*: Type: SystemAddLanguageSpecificTextSet("LST1F687C6E_8?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1F687C6E_9?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST1F687C6E_10?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The leash point producer. Can be null. Used in conjunction with [!:leashRadius]
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LST1F687C6E_11?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
                The leash radius. 
                If [!:leashPointProducer] is not null, 
                bot will not navigate furthar than this distance from point returned from [!:leashPointProducer] while running out of avoid
  - *rotationProducer*: Type: SystemAddLanguageSpecificTextSet("LST1F687C6E_12?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1F687C6E_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LST1F687C6E_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Produces the polygon's rotation.
  - *scaleProducer*: Type: SystemAddLanguageSpecificTextSet("LST1F687C6E_15?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1F687C6E_16?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LST1F687C6E_17?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Produces the polygon's scale
  - *heightProducer*: Type: SystemAddLanguageSpecificTextSet("LST1F687C6E_18?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1F687C6E_19?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LST1F687C6E_20?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
                Produces the height of the polygon. 
                Half of the produced height extends above location produced by [!:locationProducer] and the other half below
  - *pointsProducer*: Type: SystemAddLanguageSpecificTextSet("LST1F687C6E_21?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1F687C6E_22?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, AddLanguageSpecificTextSet("LST1F687C6E_23?cpp=array&lt;");Vector2AddLanguageSpecificTextSet("LST1F687C6E_24?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LST1F687C6E_25?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Produces the points that form the polygon
  - *locationProducer*: Type: SystemAddLanguageSpecificTextSet("LST1F687C6E_26?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1F687C6E_27?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, Vector3AddLanguageSpecificTextSet("LST1F687C6E_28?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
                Produces the location. 
                Polygon is rotated and scaled around produced location on the xy plane.
  - *collectionProducer*: Type: SystemAddLanguageSpecificTextSet("LST1F687C6E_29?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1F687C6E_30?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");IEnumerableAddLanguageSpecificTextSet("LST1F687C6E_31?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST1F687C6E_32?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");AddLanguageSpecificTextSet("LST1F687C6E_33?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Produces a collection of objects that should be avoided.
  - **Returns:** ReferenceAvoidanceManager Class

- **`AddAvoids Method`**
  ```csharp
  public static void AddAvoids(
IEnumerable&lt;AvoidInfo&gt; avoids
)
  ```
  - *avoids*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST456980C1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST456980C1_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");AvoidInfoAddLanguageSpecificTextSet("LST456980C1_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`AddAvoidUnitCone Method`**
  Avoids cone-shaped area effects on a WoWUnit

- **`AddAvoidUnitCone(T) Method (Func(Boolean), Predicate(T), Func(Vector3), Single, Single, Single, Single, Boolean, AvoidancePriority)`**
  ```csharp
  [ObsoleteAttribute("This ovveride will be removed in the future.")]
public static AvoidInfo AddAvoidUnitCone&lt;T&gt;(
Func&lt;bool&gt; canRun,
Predicate&lt;T&gt; objectSelector,
Func&lt;Vector3&gt; leashPointProducer,
float leashRadius,
float rotationDegrees,
float radius,
float arcDegrees,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
where T : WoWUnit
  ```
  Avoids cone-shaped area effects on a WoWUnit
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST280DE266_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST280DE266_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST280DE266_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LST280DE266_12?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LST280DE266_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST280DE266_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Selects the WoWUnit
  - *leashPointProducer*: Type: SystemAddLanguageSpecificTextSet("LST280DE266_15?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST280DE266_16?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST280DE266_17?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LST280DE266_18?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *rotationDegrees*: Type: SystemAddLanguageSpecificTextSet("LST280DE266_19?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleRotation in degrees that's relative to the WoWUnit's rotaton
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST280DE266_20?cs=.|vb=.|cpp=::|nu=.|fs=.");Singlethe radius of the cone
  - *arcDegrees*: Type: SystemAddLanguageSpecificTextSet("LST280DE266_21?cs=.|vb=.|cpp=::|nu=.|fs=.");Singlethe arc of the cone in degrees
  - **Returns:** ReferenceAvoidanceManager Class

- **`AddAvoidUnitCone(T) Method (Func(Boolean), Predicate(T), Func(Vector3), Single, Single, Single, Single, Func(T, Vector3), Boolean, AvoidancePriority)`**
  ```csharp
  public static AvoidInfo AddAvoidUnitCone&lt;T&gt;(
Func&lt;bool&gt; canRun,
Predicate&lt;T&gt; objectSelector,
Func&lt;Vector3&gt; leashPointProducer,
float leashRadius,
float rotationDegrees,
float radius,
float arcDegrees,
Func&lt;T, Vector3&gt; locationProducer = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
where T : WoWUnit
  ```
  Avoids cone-shaped area effects on a WoWUnit
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTEE19ECE2_11?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTEE19ECE2_12?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTEE19ECE2_13?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LSTEE19ECE2_14?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LSTEE19ECE2_15?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTEE19ECE2_16?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Selects the WoWUnit
  - *leashPointProducer*: Type: SystemAddLanguageSpecificTextSet("LSTEE19ECE2_17?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTEE19ECE2_18?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTEE19ECE2_19?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LSTEE19ECE2_20?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *rotationDegrees*: Type: SystemAddLanguageSpecificTextSet("LSTEE19ECE2_21?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleRotation in degrees that's relative to the WoWUnit's rotaton
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LSTEE19ECE2_22?cs=.|vb=.|cpp=::|nu=.|fs=.");Singlethe radius of the cone
  - *arcDegrees*: Type: SystemAddLanguageSpecificTextSet("LSTEE19ECE2_23?cs=.|vb=.|cpp=::|nu=.|fs=.");Singlethe arc of the cone in degrees
  - **Returns:** ReferenceAvoidanceManager Class

- **`ClearAvoids Method`**
  ```csharp
  [ObsoleteAttribute("This should never be used by 3rd party. Removed in future update.")]
public static void ClearAvoids()
  ```

- **`Pulse Method`**
  ```csharp
  public static void Pulse()
  ```
  Pulses this instance.

- **`RemoveAllAvoids Method`**
  ```csharp
  public static void RemoveAllAvoids(
Predicate&lt;AvoidInfo&gt; match
)
  ```
  - *match*: Type: SystemAddLanguageSpecificTextSet("LSTE5D2179_1?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LSTE5D2179_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");AvoidInfoAddLanguageSpecificTextSet("LSTE5D2179_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`RemoveAvoid Method`**
  ```csharp
  public static void RemoveAvoid(
AvoidInfo avoidInfo
)
  ```
  - *avoidInfo*: Type: Styx.Pathing.AvoidanceAddLanguageSpecificTextSet("LST4B710277_1?cs=.|vb=.|cpp=::|nu=.|fs=.");AvoidInfo

- **`ResetNavigation Method`**
  ```csharp
  public static void ResetNavigation()
  ```
  Resets the internal navigation system.

#### AvoidanceManager Events

- **`OnRunningOut Event`**
  ```csharp
  public static event EventHandler&lt;EventArgs&gt; OnRunningOut
  ```
  Triggered when running out of an avoided area

---

### AvoidancePriority Enumeration

```csharp
public enum AvoidancePriority
```

---
