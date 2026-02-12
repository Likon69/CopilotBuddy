# Styx.WoWInternals.WoWObjects.AreaTriggerShapes

## Contents

- [AreaTriggerBox Class](#areatriggerbox-class)
- [AreaTriggerCylinder Class](#areatriggercylinder-class)
- [AreaTriggerPolygon Class](#areatriggerpolygon-class)
- [AreaTriggerShape Class](#areatriggershape-class)
- [AreaTriggerSphere Class](#areatriggersphere-class)

---

### AreaTriggerBox Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.AreaTriggerShapes.AreaTriggerShape → Styx.WoWInternals.WoWObjects.AreaTriggerShapes.AreaTriggerBox

```csharp
public class AreaTriggerBox : AreaTriggerShape
```

Area Trigger Shape

#### AreaTriggerBox Properties

- **`CurrentExtents Property`**
  ```csharp
  public Vector3 CurrentExtents { get; }
  ```

- **`Extents Property`**
  ```csharp
  public Vector3 Extents { get; }
  ```
  Gets the extents.

- **`ExtentsTarget Property`**
  ```csharp
  public Vector3 ExtentsTarget { get; }
  ```
  The extents that this instance animates to

#### AreaTriggerBox Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceAreaTriggerBox Class

---

### AreaTriggerCylinder Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.AreaTriggerShapes.AreaTriggerShape → Styx.WoWInternals.WoWObjects.AreaTriggerShapes.AreaTriggerCylinder

```csharp
public class AreaTriggerCylinder : AreaTriggerShape
```

Area Trigger Shape

#### AreaTriggerCylinder Properties

- **`CurrentHeight Property`**
  ```csharp
  public float CurrentHeight { get; }
  ```
  Gets the current height.

- **`CurrentRadius Property`**
  ```csharp
  public float CurrentRadius { get; }
  ```
  Gets the current radius.

- **`Height Property`**
  ```csharp
  public float Height { get; }
  ```
  Gets the height.

- **`HeightTarget Property`**
  ```csharp
  public float HeightTarget { get; }
  ```
  The height that this instance animates to

- **`Radius Property`**
  ```csharp
  public float Radius { get; }
  ```
  Gets the radius.

- **`RadiusTarget Property`**
  ```csharp
  public float RadiusTarget { get; }
  ```
  The radius that this instance animates to

#### AreaTriggerCylinder Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceAreaTriggerCylinder Class

---

### AreaTriggerPolygon Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.AreaTriggerShapes.AreaTriggerShape → Styx.WoWInternals.WoWObjects.AreaTriggerShapes.AreaTriggerPolygon

```csharp
public class AreaTriggerPolygon : AreaTriggerShape
```

Area Trigger Shape

#### AreaTriggerPolygon Properties

- **`CurrentHeight Property`**
  ```csharp
  public float CurrentHeight { get; }
  ```
  Gets the current height.

- **`CurrentVertices Property`**
  ```csharp
  public Vector2[] CurrentVertices { get; }
  ```
  Gets the current vertices.

- **`Height Property`**
  ```csharp
  public float Height { get; }
  ```
  Gets the height.

- **`HeightTarget Property`**
  ```csharp
  public float HeightTarget { get; }
  ```
  The height that this instance animates to

- **`Vertices Property`**
  ```csharp
  public Vector2[] Vertices { get; }
  ```
  Gets the vectors that define a polygon.

- **`VerticesTarget Property`**
  ```csharp
  public Vector2[] VerticesTarget { get; }
  ```
  Gets the vertices target.

#### AreaTriggerPolygon Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceAreaTriggerPolygon Class

---

### AreaTriggerShape Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.AreaTriggerShapes.AreaTriggerShape → Styx.WoWInternals.WoWObjects.AreaTriggerShapes.AreaTriggerBox → Styx.WoWInternals.WoWObjects.AreaTriggerShapes.AreaTriggerCylinder → Styx.WoWInternals.WoWObjects.AreaTriggerShapes.AreaTriggerPolygon → Styx.WoWInternals.WoWObjects.AreaTriggerShapes.AreaTriggerSphere

```csharp
public class AreaTriggerShape
```

Area Trigger Shape

#### AreaTriggerShape Properties

- **`AnnimationProgress Property`**
  ```csharp
  public float AnnimationProgress { get; }
  ```

- **`FacingCurveId Property`**
  ```csharp
  public int FacingCurveId { get; }
  ```
  Gets the facing curve identifier.

- **`Flags Property`**
  ```csharp
  public AreaTriggerFlags Flags { get; }
  ```
  Gets the flags.

- **`IsDynamicShape Property`**
  ```csharp
  public bool IsDynamicShape { get; }
  ```
  Gets a value indicating whether this instance is dynamic shape.

- **`MorphCurveId Property`**
  ```csharp
  public int MorphCurveId { get; }
  ```
  Gets the morph curve identifier.

- **`MoveCurveId Property`**
  ```csharp
  public int MoveCurveId { get; }
  ```
  Gets the move curve identifier.

- **`ScaleCurveId Property`**
  ```csharp
  public int ScaleCurveId { get; }
  ```
  Gets the scale curve identifier.

- **`Type Property`**
  ```csharp
  public AreaTriggerShapeType Type { get; }
  ```
  Gets the type.

#### AreaTriggerShape Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceAreaTriggerShape Class

---

### AreaTriggerSphere Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.AreaTriggerShapes.AreaTriggerShape → Styx.WoWInternals.WoWObjects.AreaTriggerShapes.AreaTriggerSphere

```csharp
public class AreaTriggerSphere : AreaTriggerShape
```

Area Trigger Shape

#### AreaTriggerSphere Properties

- **`CurrentRadius Property`**
  ```csharp
  public float CurrentRadius { get; }
  ```
  Gets the current radius.

- **`Radius Property`**
  ```csharp
  public float Radius { get; }
  ```
  Gets the radius.

- **`RadiusTarget Property`**
  ```csharp
  public float RadiusTarget { get; }
  ```
  The radius that this instance animates to

#### AreaTriggerSphere Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceAreaTriggerSphere Class

---
