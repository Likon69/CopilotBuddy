# Styx.CommonBot.AreaManagement

Contains area management related classes.

## Contents

- [Area Class](#area-class)
- [AreaManager Class](#areamanager-class)
- [AreaType Enumeration](#areatype-enumeration)
- [GrindArea Class](#grindarea-class)
- [Hotspot Class](#hotspot-class)
- [HotspotExtensions Class](#hotspotextensions-class)
- [HotspotManager Class](#hotspotmanager-class)
- [PolygonArea Class](#polygonarea-class)
- [PvPArea Class](#pvparea-class)
- [QuestArea Class](#questarea-class)

---

### Area Class

**Inheritance:** System.Object → Styx.CommonBot.AreaManagement.Area → Styx.CommonBot.AreaManagement.GrindArea → Styx.CommonBot.AreaManagement.PolygonArea

```csharp
public abstract class Area : IEquatable&lt;Area&gt;
```

An area.

#### Area Properties

- **`CircledHotspots Property`**
  ```csharp
  public CircularQueue&lt;Hotspot&gt; CircledHotspots { get; set; }
  ```
  Gets or sets the circled hotspots.

- **`Guid Property`**
  ```csharp
  public Guid Guid { get; }
  ```
  Gets a unique identifier.

- **`Hotspots Property`**
  ```csharp
  public List&lt;Hotspot&gt; Hotspots { get; set; }
  ```
  Gets or sets the hotspots.

- **`Type Property`**
  ```csharp
  public abstract AreaType Type { get; }
  ```
  Gets the type.

#### Area Methods

- **`CycleToNearest Method`**
  ```csharp
  public void CycleToNearest()
  ```
  Cycles to the nearest hotspot.

- **`Equals Method`**
  Determines whether the specified Object is equal to the
            current Object.

- **`Equals Method (Object)`**
  ```csharp
  public override bool Equals(
Object obj
)
  ```
  Determines whether the specified Object is equal to the
            current Object.
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LSTCCFC25FC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe Object to compare with the current
            Object.
  - **Returns:** ReferenceArea Class

- **`Equals Method (Area)`**
  ```csharp
  public bool Equals(
Area other
)
  ```
  Indicates whether the current object is equal to another object of the same type.
  - *other*: Type: Styx.CommonBot.AreaManagementAddLanguageSpecificTextSet("LST41CD16E3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");AreaAn object to compare with this object.
  - **Returns:** See Also

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  Serves as a hash function for a particular type.
  - **Returns:** ReferenceArea Class

- **`GetNextHotspot Method`**
  ```csharp
  public Hotspot GetNextHotspot()
  ```
  Dequeue's a hotspot in the que, returns null if the queue is empty.
  - **Returns:** ReferenceArea Class

- **`GetRandomHotspot Method`**
  ```csharp
  public Hotspot GetRandomHotspot()
  ```
  Gets a random hotspot that is not on the blacklist.
  - **Returns:** ReferenceArea Class

#### Area Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
Area left,
Area right
)
  ```
  Equality operator.
  - *left*: Type: Styx.CommonBot.AreaManagementAddLanguageSpecificTextSet("LSTD6792907_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Area The left.
  - *right*: Type: Styx.CommonBot.AreaManagementAddLanguageSpecificTextSet("LSTD6792907_2?cs=.|vb=.|cpp=::|nu=.|fs=.");AreaThe right.
  - **Returns:** ReferenceArea Class

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
Area left,
Area right
)
  ```
  Inequality operator.
  - *left*: Type: Styx.CommonBot.AreaManagementAddLanguageSpecificTextSet("LST60BA5436_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Area The left.
  - *right*: Type: Styx.CommonBot.AreaManagementAddLanguageSpecificTextSet("LST60BA5436_2?cs=.|vb=.|cpp=::|nu=.|fs=.");AreaThe right.
  - **Returns:** ReferenceArea Class

---

### AreaManager Class

**Inheritance:** System.Object → Styx.CommonBot.AreaManagement.AreaManager

```csharp
public class AreaManager
```

Manager for areas.

#### AreaManager Properties

- **`CurrentGrindArea Property`**
  ```csharp
  public GrindArea CurrentGrindArea { get; }
  ```
  Returns the current grind area, null if non is specified in the current subprofile.

#### AreaManager Methods

- **`SetArea Method`**
  ```csharp
  public void SetArea(
GrindArea area
)
  ```
  Sets the current GrindArea.
  - *area*: Type: Styx.CommonBot.AreaManagementAddLanguageSpecificTextSet("LSTF23B5B12_1?cs=.|vb=.|cpp=::|nu=.|fs=.");GrindArea.

---

### AreaType Enumeration

```csharp
public enum AreaType
```

Values that represent AreaType.

---

### GrindArea Class

**Inheritance:** System.Object → Styx.CommonBot.AreaManagement.Area → Styx.CommonBot.AreaManagement.GrindArea → Styx.CommonBot.AreaManagement.QuestArea

```csharp
public class GrindArea : Area
```

A grind area.

#### GrindArea Constructor

- **`GrindArea Constructor`**
  ```csharp
  public GrindArea()
  ```
  Default constructor.

- **`GrindArea Constructor (HotspotManager)`**
  ```csharp
  public GrindArea(
HotspotManager manager
)
  ```
  Constructor.
  - *manager*: Type: Styx.CommonBot.AreaManagementAddLanguageSpecificTextSet("LSTEBFA88E8_0?cs=.|vb=.|cpp=::|nu=.|fs=.");HotspotManagerThe manager.

#### GrindArea Properties

- **`AreaDefinitions Property`**
  ```csharp
  public virtual List&lt;List&lt;Vector3&gt;&gt; AreaDefinitions { get; }
  ```
  Gets the area definitions.

- **`CurrentHotSpot Property`**
  ```csharp
  public Hotspot CurrentHotSpot { get; }
  ```
  Returns the current hotspot the bot should move against.

- **`Factions Property`**
  ```csharp
  public List&lt;int&gt; Factions { get; set; }
  ```
  Gets or sets the factions.

- **`HotspotChanged Property`**
  ```csharp
  public bool HotspotChanged { get; }
  ```
  Gets a value indicating whether the hotspot was changed.

- **`LastHotSpot Property`**
  ```csharp
  public Hotspot LastHotSpot { get; set; }
  ```
  Gets or sets the last hot spot.

- **`LootMobs Property`**
  ```csharp
  public Nullable&lt;bool&gt; LootMobs { get; set; }
  ```
  Gets or sets the loot mobs.

- **`LootRadius Property`**
  ```csharp
  public Nullable&lt;double&gt; LootRadius { get; set; }
  ```
  Gets or sets the loot radius.

- **`MaxDistance Property`**
  ```csharp
  public Nullable&lt;double&gt; MaxDistance { get; set; }
  ```
  Gets or sets the maximum distance.

- **`MaximumHotspotTime Property`**
  ```csharp
  public Nullable&lt;int&gt; MaximumHotspotTime { get; set; }
  ```
  Gets or sets the maximum hotspot time.

- **`MobIDs Property`**
  ```csharp
  public List&lt;int&gt; MobIDs { get; set; }
  ```
  Gets or sets the mob i ds.

- **`Name Property`**
  ```csharp
  public string Name { get; set; }
  ```
  Gets or sets the name.

- **`RandomizeHotspots Property`**
  ```csharp
  public bool RandomizeHotspots { get; set; }
  ```
  Gets or sets a value indicating whether the randomize hotspots.

- **`TargetMaxLevel Property`**
  ```csharp
  public int TargetMaxLevel { get; set; }
  ```
  Gets or sets target maximum level.

- **`TargetMinLevel Property`**
  ```csharp
  public int TargetMinLevel { get; set; }
  ```
  Gets or sets target minimum level.

- **`Type Property`**
  ```csharp
  public override AreaType Type { get; }
  ```
  Gets the type.

- **`UseMount Property`**
  ```csharp
  public Nullable&lt;bool&gt; UseMount { get; set; }
  ```
  Gets or sets the use mount.

#### GrindArea Methods

- **`FromXML Method`**
  ```csharp
  public static GrindArea FromXML(
XElement value
)
  ```
  Returns an GrindArea object of the passede in XElement
            object.
  - *value*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST6C13B408_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe value.
  - **Returns:** ExceptionConditionProfileAttributeExpectedException      Thrown when a Profile Attribute
            Expected error condition occurs.ProfileUnknownAttributeException       Thrown when a Profile Unknown
            Attribute error condition occurs.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceGrindArea Class

#### GrindArea Fields

- **`Blacklist Field`**
  ```csharp
  public Dictionary&lt;uint, BlacklistFlags&gt; Blacklist
  ```
  The blacklist.

---

### Hotspot Class

**Inheritance:** System.Object → Styx.CommonBot.AreaManagement.Hotspot

```csharp
public class Hotspot
```

A hotspot.

#### Hotspot Constructor

- **`Hotspot Constructor`**
  ```csharp
  public Hotspot()
  ```
  Default constructor.

- **`Hotspot Constructor (XElement)`**
  ```csharp
  public Hotspot(
XElement element
)
  ```
  Constructor.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST523CAB12_0?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.

- **`Hotspot Constructor (Single, Single, Single)`**
  ```csharp
  public Hotspot(
float x,
float y,
float z
)
  ```
  Constructor.
  - *x*: Type: SystemAddLanguageSpecificTextSet("LST64D73C63_0?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe x coordinate.
  - *y*: Type: SystemAddLanguageSpecificTextSet("LST64D73C63_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe y coordinate.
  - *z*: Type: SystemAddLanguageSpecificTextSet("LST64D73C63_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe z coordinate.

#### Hotspot Properties

- **`Name Property`**
  ```csharp
  public string Name { get; set; }
  ```
  Gets or sets the name.

- **`Position Property`**
  ```csharp
  public Vector3 Position { get; set; }
  ```
  Gets or sets the position.

#### Hotspot Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceHotspot Class

- **`ToVector3 Method`**
  ```csharp
  public Vector3 ToVector3()
  ```
  Returns this Hotspot as a Vector3 object.
  - **Returns:** ReferenceHotspot Class

- **`ToXML Method`**
  ```csharp
  public XElement ToXML()
  ```
  Returns this hotspot as a XElement object.
  - **Returns:** ReferenceHotspot Class

#### Hotspot Type Conversions

- **`Implicit Conversion Operators`**
  Hotspot casting operator.

- **`Implicit Conversion (Vector3 to Hotspot)`**
  ```csharp
  public static implicit operator Hotspot (
Vector3 hotspot
)
  ```
  Hotspot casting operator.
  - *hotspot*: Type: System.NumericsAddLanguageSpecificTextSet("LST6520EDB4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The hotspot.
  - **Returns:** ReferenceHotspot Class

- **`Implicit Conversion (Hotspot to Vector3)`**
  ```csharp
  public static implicit operator Vector3 (
Hotspot hotspot
)
  ```
  Vector3 casting operator.
  - *hotspot*: Type: Styx.CommonBot.AreaManagementAddLanguageSpecificTextSet("LSTBD9D6B64_1?cs=.|vb=.|cpp=::|nu=.|fs=.");HotspotThe hotspot.
  - **Returns:** ReferenceHotspot Class

---

### HotspotExtensions Class

**Inheritance:** System.Object → Styx.CommonBot.AreaManagement.HotspotExtensions

```csharp
public static class HotspotExtensions
```

A hotspot extensions.

#### HotspotExtensions Methods

- **`ToHotspot Method`**
  ```csharp
  public static Hotspot ToHotspot(
this Vector3 value
)
  ```
  Returns this Vector3 as a Hotspot object.
  - *value*: Type: System.NumericsAddLanguageSpecificTextSet("LSTF1BF334E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3.
  - **Returns:** ReferenceHotspotExtensions Class

---

### HotspotManager Class

**Inheritance:** System.Object → Styx.CommonBot.AreaManagement.HotspotManager

```csharp
public class HotspotManager
```

Manager for hotspots.

#### HotspotManager Constructor

- **`HotspotManager Constructor (IEnumerable(Vector3))`**
  ```csharp
  public HotspotManager(
IEnumerable&lt;Vector3&gt; points
)
  ```
  Constructor.
  - *points*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTF9F9B1B6_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTF9F9B1B6_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTF9F9B1B6_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The points.

- **`HotspotManager Constructor (XElement)`**
  ```csharp
  public HotspotManager(
XElement element
)
  ```
  Constructor.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTABE9A31B_0?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.

#### HotspotManager Properties

- **`Blacklist Property`**
  ```csharp
  public Dictionary&lt;Vector3, DateTime&gt; Blacklist { get; }
  ```
  Gets the blacklist.

- **`CurrentHotSpot Property`**
  ```csharp
  public static Vector3 CurrentHotSpot { get; }
  ```
  Gets the current hot spot.

- **`Hotspots Property`**
  ```csharp
  public List&lt;Vector3&gt; Hotspots { get; }
  ```
  Gets the hotspots.

- **`LastHotSpot Property`**
  ```csharp
  public static Vector3 LastHotSpot { get; set; }
  ```
  Gets or sets the last hot spot.

- **`Timer Property`**
  ```csharp
  public static Stopwatch Timer { get; set; }
  ```
  Gets or sets the timer.

#### HotspotManager Methods

- **`BlacklistPoint Method`**
  Blacklist point.

- **`BlacklistPoint Method (Vector3, DateTime)`**
  ```csharp
  public void BlacklistPoint(
Vector3 pnt,
DateTime expiration
)
  ```
  Blacklist point.
  - *pnt*: Type: System.NumericsAddLanguageSpecificTextSet("LST22240A72_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3       The point.
  - *expiration*: Type: SystemAddLanguageSpecificTextSet("LST22240A72_2?cs=.|vb=.|cpp=::|nu=.|fs=.");DateTimeThe expiration Date/Time.

- **`BlacklistPoint Method (Vector3, TimeSpan)`**
  ```csharp
  public void BlacklistPoint(
Vector3 pnt,
TimeSpan forTime
)
  ```
  Blacklist point.
  - *pnt*: Type: System.NumericsAddLanguageSpecificTextSet("LST7D9152FE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3    The point.
  - *forTime*: Type: SystemAddLanguageSpecificTextSet("LST7D9152FE_2?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanfor time.

- **`CycleToNearest Method`**
  ```csharp
  public void CycleToNearest()
  ```
  Cycle to nearest.

- **`GetNextHotspot Method`**
  ```csharp
  public Vector3 GetNextHotspot()
  ```
  Gets the next hotspot.
  - **Returns:** ReferenceHotspotManager Class

- **`GetRandomHotspot Method`**
  ```csharp
  public Vector3 GetRandomHotspot()
  ```
  Gets a random hotspot that is not on the blacklist.
  - **Returns:** ReferenceHotspotManager Class

---

### PolygonArea Class

**Inheritance:** System.Object → Styx.CommonBot.AreaManagement.Area → Styx.CommonBot.AreaManagement.PolygonArea → Styx.CommonBot.AreaManagement.PvPArea

```csharp
public abstract class PolygonArea : Area
```

A polygon area.

#### PolygonArea Properties

- **`LocalPlayerIsInBounds Property`**
  ```csharp
  public bool LocalPlayerIsInBounds { get; }
  ```
  Gets a value indicating whether the local player is in bounds.

#### PolygonArea Methods

- **`IsPointInPoly Method`**
  ```csharp
  public bool IsPointInPoly(
Vector3 point
)
  ```
  Query if 'point' is point in polygon.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LST98464903_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The point.
  - **Returns:** ReferencePolygonArea Class

---

### PvPArea Class

**Inheritance:** System.Object → Styx.CommonBot.AreaManagement.Area → Styx.CommonBot.AreaManagement.PolygonArea → Styx.CommonBot.AreaManagement.PvPArea

```csharp
public class PvPArea : PolygonArea
```

A pv p area.

#### PvPArea Constructor

- **`PvPArea Constructor (Vector2[])`**
  ```csharp
  public PvPArea(
params Vector2[] areaDefinition
)
  ```
  Constructor.
  - *areaDefinition*: Type: AddLanguageSpecificTextSet("LST68071350_2?cpp=array&lt;");System.NumericsAddLanguageSpecificTextSet("LST68071350_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector2AddLanguageSpecificTextSet("LST68071350_4?cpp=&gt;|vb=()|nu=[]");A variable-length parameters list containing area definition.

- **`PvPArea Constructor (String, Vector2[])`**
  ```csharp
  public PvPArea(
string name,
params Vector2[] areaDefinition
)
  ```
  Constructor.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LSTEC273E90_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String          The name.
  - *areaDefinition*: Type: AddLanguageSpecificTextSet("LSTEC273E90_3?cpp=array&lt;");System.NumericsAddLanguageSpecificTextSet("LSTEC273E90_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector2AddLanguageSpecificTextSet("LSTEC273E90_5?cpp=&gt;|vb=()|nu=[]");A variable-length parameters list containing area definition.

#### PvPArea Properties

- **`Type Property`**
  ```csharp
  public override AreaType Type { get; }
  ```
  Gets the type.

#### PvPArea Fields

- **`Name Field`**
  ```csharp
  public readonly string Name
  ```
  The name.

---

### QuestArea Class

**Inheritance:** System.Object → Styx.CommonBot.AreaManagement.Area → Styx.CommonBot.AreaManagement.GrindArea → Styx.CommonBot.AreaManagement.QuestArea

```csharp
public class QuestArea : GrindArea
```

Class used to define a grind area for associated with one or more quests.

#### QuestArea Properties

- **`AreaDefinitions Property`**
  ```csharp
  public override List&lt;List&lt;Vector3&gt;&gt; AreaDefinitions { get; }
  ```
  Gets the area definitions.

- **`HotspotsCreated Property`**
  ```csharp
  public bool HotspotsCreated { get; }
  ```
  Gets a value indicating whether the hotspots created.

- **`Quest Property`**
  ```csharp
  public PlayerQuest Quest { get; }
  ```
  Gets the question.

- **`Type Property`**
  ```csharp
  public override AreaType Type { get; }
  ```
  Gets the type.

#### QuestArea Methods

- **`CreateHotspotsAsync Method`**
  ```csharp
  public Task&lt;int&gt; CreateHotspotsAsync(
Vector3 navigableFrom,
bool allowFallback
)
  ```
  Creates hotspots for this quest area. The created hotspots are navigable from the specified location.
  - *navigableFrom*: Type: System.NumericsAddLanguageSpecificTextSet("LSTDAFF314B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *allowFallback*: Type: SystemAddLanguageSpecificTextSet("LSTDAFF314B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanIf true and no hotspots could be generated, just try to find highly connected hotspots.
  - **Returns:** Remarks

---
