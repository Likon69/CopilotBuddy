# Styx.WoWInternals.World

Contains WoW world related classes.

## Contents

- [AreaTable(T) Class](#areatablet-class)
- [GameWorld Class](#gameworld-class)
- [JbnMap Class](#jbnmap-class)
- [JbnMapAreaTableEntry Structure](#jbnmapareatableentry-structure)
- [TraceLineHitFlags Enumeration](#tracelinehitflags-enumeration)
- [Triangle Structure](#triangle-structure)
- [UnitSpellLineOfSightTestEventArgs Class](#unitspelllineofsighttesteventargs-class)
- [WorldLine Structure](#worldline-structure)
- [WorldMap Class](#worldmap-class)
- [WorldMapAreaTableEntry Structure](#worldmapareatableentry-structure)
- [WorldScene Class](#worldscene-class)

---

### AreaTable(T) Class

**Inheritance:** System.Object → Styx.WoWInternals.World.AreaTable.T

```csharp
public class AreaTable&lt;T&gt;
where T : struct, new()
```

An area table.

#### AreaTable(T) Properties

- **`Height Property`**
  ```csharp
  public int Height { get; }
  ```
  Gets the height of this area table. This should be equal to MaxX -
            MaxY + 1.

- **`MaxX Property`**
  ```csharp
  public int MaxX { get; }
  ```
  Gets the max X coordinate.

- **`MaxY Property`**
  ```csharp
  public int MaxY { get; }
  ```
  Gets the max Y coordinate.

- **`MinX Property`**
  ```csharp
  public int MinX { get; }
  ```
  Gets the min X coordinate.

- **`MinY Property`**
  ```csharp
  public int MinY { get; }
  ```
  Gets the min Y coordinate.

- **`Width Property`**
  ```csharp
  public int Width { get; }
  ```
  Gets the width of this area table. This should be equal to MaxX -
            MaxY + 1.

#### AreaTable(T) Methods

- **`GetEntries Method`**
  ```csharp
  public T[] GetEntries()
  ```
  Gets all entries in this area table.
  - **Returns:** See Also

- **`GetEntry Method`**
  ```csharp
  public T GetEntry(
int x,
int y
)
  ```
  Gets an entry from this area table.
  - *x*: Type: SystemAddLanguageSpecificTextSet("LSTD47F4063_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.
  - *y*: Type: SystemAddLanguageSpecificTextSet("LSTD47F4063_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.
  - **Returns:** Exceptions

- **`HasEntry Method`**
  ```csharp
  public bool HasEntry(
int x,
int y
)
  ```
  Gets a value that indicates whether this AreaTable.T has an entry at the
            specified X and Y coordinates.
  - *x*: Type: SystemAddLanguageSpecificTextSet("LSTB146600F_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.
  - *y*: Type: SystemAddLanguageSpecificTextSet("LSTB146600F_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.
  - **Returns:** ReferenceAreaTableAddLanguageSpecificTextSet("LSTB146600F_7?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LSTB146600F_8?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

- **`TryGetEntry Method`**
  ```csharp
  public bool TryGetEntry(
int x,
int y,
out T value
)
  ```
  Tries to get an entry from this area table.
  - *x*: Type: SystemAddLanguageSpecificTextSet("LST9D1805DC_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32    .
  - *y*: Type: SystemAddLanguageSpecificTextSet("LST9D1805DC_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32    .
  - *value*: Type: TAddLanguageSpecificTextSet("LST9D1805DC_5?cpp=%");[out].
  - **Returns:** ReferenceAreaTableAddLanguageSpecificTextSet("LST9D1805DC_6?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST9D1805DC_7?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### GameWorld Class

**Inheritance:** System.Object → Styx.WoWInternals.World.GameWorld

```csharp
public static class GameWorld
```

Provides functions for interacting and checking the game world.

#### GameWorld Properties

- **`SceneTimeMs Property`**
  ```csharp
  public static uint SceneTimeMs { get; }
  ```
  Gets the scene time.

#### GameWorld Methods

- **`GetTriangles Method`**
  ```csharp
  public static bool GetTriangles(
Vector3 boundsMin,
Vector3 boundsMax,
TraceLineHitFlags flags,
out Triangle[] triangles
)
  ```
  Gets the triangles in the game that are inside the box defined by boundsMin and boundsMax
  - *boundsMin*: Type: System.NumericsAddLanguageSpecificTextSet("LSTC932815F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The lowest location of the axis-aligned box
  - *boundsMax*: Type: System.NumericsAddLanguageSpecificTextSet("LSTC932815F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The highest location of the axis-aligned box.
  - *flags*: Type: Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LSTC932815F_3?cs=.|vb=.|cpp=::|nu=.|fs=.");TraceLineHitFlagsThe flags.
  - *triangles*: Type: AddLanguageSpecificTextSet("LSTC932815F_4?cpp=array&lt;");Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LSTC932815F_5?cs=.|vb=.|cpp=::|nu=.|fs=.");TriangleAddLanguageSpecificTextSet("LSTC932815F_6?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LSTC932815F_7?cpp=%");The triangles.
  - **Returns:** See Also

- **`IsInLineOfSight Method`**
  Used to check line of sight between Me and unit.

- **`IsInLineOfSight Method (WoWUnit)`**
  ```csharp
  public static bool IsInLineOfSight(
WoWUnit unit
)
  ```
  Used to check line of sight between Me and unit.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST8DF4D681_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitStart point.
  - **Returns:** See Also

- **`IsInLineOfSight Method (Vector3, Vector3)`**
  ```csharp
  public static bool IsInLineOfSight(
Vector3 from,
Vector3 to
)
  ```
  Used to check visual sight between two points.
  - *from*: Type: System.NumericsAddLanguageSpecificTextSet("LSTE88954B0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3Start point.
  - *to*: Type: System.NumericsAddLanguageSpecificTextSet("LSTE88954B0_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3  End point.
  - **Returns:** See Also

- **`IsInLineOfSpellSight Method`**
  Used to check spell line of sight between Me and unit.

- **`IsInLineOfSpellSight Method (WoWUnit)`**
  ```csharp
  public static bool IsInLineOfSpellSight(
WoWUnit unit
)
  ```
  Used to check spell line of sight between Me and unit.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST35C80733_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitStart point.
  - **Returns:** See Also

- **`IsInLineOfSpellSight Method (Vector3, Vector3)`**
  ```csharp
  public static bool IsInLineOfSpellSight(
Vector3 from,
Vector3 to
)
  ```
  Used to check spell LoS between two points. This will make sure you can cast
            heal/damage spells. Two points may not be in visual sight while this returns true!
  - *from*: Type: System.NumericsAddLanguageSpecificTextSet("LST77A11BF6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3Start point.
  - *to*: Type: System.NumericsAddLanguageSpecificTextSet("LST77A11BF6_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3  End point.
  - **Returns:** See Also

- **`MassTraceLine Method`**
  Performs a mass traceline.

- **`MassTraceLine Method (WorldLine[], TraceLineHitFlags, Boolean[])`**
  ```csharp
  public static void MassTraceLine(
WorldLine[] lines,
TraceLineHitFlags flag,
out bool[] hitResults
)
  ```
  Performs a mass traceline.
  - *lines*: Type: AddLanguageSpecificTextSet("LST94F693DE_6?cpp=array&lt;");Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LST94F693DE_7?cs=.|vb=.|cpp=::|nu=.|fs=.");WorldLineAddLanguageSpecificTextSet("LST94F693DE_8?cpp=&gt;|vb=()|nu=[]");     The lines.
  - *flag*: Type: Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LST94F693DE_9?cs=.|vb=.|cpp=::|nu=.|fs=.");TraceLineHitFlags      The flag.
  - *hitResults*: Type: AddLanguageSpecificTextSet("LST94F693DE_10?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST94F693DE_11?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanAddLanguageSpecificTextSet("LST94F693DE_12?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LST94F693DE_13?cpp=%");[out] The hit results.

- **`MassTraceLine Method (WorldLine[], TraceLineHitFlags[], Boolean[])`**
  ```csharp
  public static void MassTraceLine(
WorldLine[] lines,
TraceLineHitFlags[] flags,
out bool[] hitResults
)
  ```
  Performs a mass traceline.
  - *lines*: Type: AddLanguageSpecificTextSet("LSTE2CB746E_8?cpp=array&lt;");Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LSTE2CB746E_9?cs=.|vb=.|cpp=::|nu=.|fs=.");WorldLineAddLanguageSpecificTextSet("LSTE2CB746E_10?cpp=&gt;|vb=()|nu=[]");     The lines.
  - *flags*: Type: AddLanguageSpecificTextSet("LSTE2CB746E_11?cpp=array&lt;");Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LSTE2CB746E_12?cs=.|vb=.|cpp=::|nu=.|fs=.");TraceLineHitFlagsAddLanguageSpecificTextSet("LSTE2CB746E_13?cpp=&gt;|vb=()|nu=[]");     The flags.
  - *hitResults*: Type: AddLanguageSpecificTextSet("LSTE2CB746E_14?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTE2CB746E_15?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanAddLanguageSpecificTextSet("LSTE2CB746E_16?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LSTE2CB746E_17?cpp=%");[out] The hit results.

- **`MassTraceLine Method (WorldLine[], TraceLineHitFlags, Boolean[], Vector3[])`**
  ```csharp
  public static void MassTraceLine(
WorldLine[] lines,
TraceLineHitFlags flag,
out bool[] hitResults,
out Vector3[] hitPoints
)
  ```
  Performs a mass traceline.
  - *lines*: Type: AddLanguageSpecificTextSet("LST7F3CDAAD_9?cpp=array&lt;");Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LST7F3CDAAD_10?cs=.|vb=.|cpp=::|nu=.|fs=.");WorldLineAddLanguageSpecificTextSet("LST7F3CDAAD_11?cpp=&gt;|vb=()|nu=[]");     The lines.
  - *flag*: Type: Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LST7F3CDAAD_12?cs=.|vb=.|cpp=::|nu=.|fs=.");TraceLineHitFlags      The flag.
  - *hitResults*: Type: AddLanguageSpecificTextSet("LST7F3CDAAD_13?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST7F3CDAAD_14?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanAddLanguageSpecificTextSet("LST7F3CDAAD_15?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LST7F3CDAAD_16?cpp=%");[out] The hit results.
  - *hitPoints*: Type: AddLanguageSpecificTextSet("LST7F3CDAAD_17?cpp=array&lt;");System.NumericsAddLanguageSpecificTextSet("LST7F3CDAAD_18?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3AddLanguageSpecificTextSet("LST7F3CDAAD_19?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LST7F3CDAAD_20?cpp=%"); [out] The hit points.

- **`MassTraceLine Method (WorldLine[], TraceLineHitFlags[], Boolean[], Vector3[])`**
  ```csharp
  public static void MassTraceLine(
WorldLine[] lines,
TraceLineHitFlags[] flags,
out bool[] hitResults,
out Vector3[] hitPoints
)
  ```
  Performs a mass traceline.
  - *lines*: Type: AddLanguageSpecificTextSet("LST592013FF_11?cpp=array&lt;");Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LST592013FF_12?cs=.|vb=.|cpp=::|nu=.|fs=.");WorldLineAddLanguageSpecificTextSet("LST592013FF_13?cpp=&gt;|vb=()|nu=[]");     The lines.
  - *flags*: Type: AddLanguageSpecificTextSet("LST592013FF_14?cpp=array&lt;");Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LST592013FF_15?cs=.|vb=.|cpp=::|nu=.|fs=.");TraceLineHitFlagsAddLanguageSpecificTextSet("LST592013FF_16?cpp=&gt;|vb=()|nu=[]");     The flags.
  - *hitResults*: Type: AddLanguageSpecificTextSet("LST592013FF_17?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST592013FF_18?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanAddLanguageSpecificTextSet("LST592013FF_19?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LST592013FF_20?cpp=%");[out] The hit results.
  - *hitPoints*: Type: AddLanguageSpecificTextSet("LST592013FF_21?cpp=array&lt;");System.NumericsAddLanguageSpecificTextSet("LST592013FF_22?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3AddLanguageSpecificTextSet("LST592013FF_23?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LST592013FF_24?cpp=%"); [out] The hit points.

- **`TraceLine Method`**
  Checks if a path between 2 points is obstructed.

- **`TraceLine Method (Vector3, Vector3, TraceLineHitFlags)`**
  ```csharp
  public static bool TraceLine(
Vector3 from,
Vector3 to,
TraceLineHitFlags flags
)
  ```
  Checks if a path between 2 points is obstructed.
  - *from*: Type: System.NumericsAddLanguageSpecificTextSet("LSTCA28BE1E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3 From.
  - *to*: Type: System.NumericsAddLanguageSpecificTextSet("LSTCA28BE1E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3   To.
  - *flags*: Type: Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LSTCA28BE1E_3?cs=.|vb=.|cpp=::|nu=.|fs=.");TraceLineHitFlagsThe flags.
  - **Returns:** See Also

- **`TraceLine Method (Vector3, Vector3, TraceLineHitFlags, Vector3)`**
  ```csharp
  public static bool TraceLine(
Vector3 from,
Vector3 to,
TraceLineHitFlags flags,
out Vector3 hitPoint
)
  ```
  Checks if a path between 2 points is obstructed.
  - *from*: Type: System.NumericsAddLanguageSpecificTextSet("LST10E3CA01_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3    From.
  - *to*: Type: System.NumericsAddLanguageSpecificTextSet("LST10E3CA01_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3      To.
  - *flags*: Type: Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LST10E3CA01_4?cs=.|vb=.|cpp=::|nu=.|fs=.");TraceLineHitFlags   The flags.
  - *hitPoint*: Type: System.NumericsAddLanguageSpecificTextSet("LST10E3CA01_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3AddLanguageSpecificTextSet("LST10E3CA01_6?cpp=%");[out] The hit point.
  - **Returns:** See Also

#### GameWorld Events

- **`UnitSpellLineOfSightTest Event`**
  ```csharp
  public static event EventHandler&lt;UnitSpellLineOfSightTestEventArgs&gt; UnitSpellLineOfSightTest
  ```

---

### JbnMap Class

**Inheritance:** System.Object → Styx.WoWInternals.World.JbnMap

```csharp
public class JbnMap
```

A jbn map.

#### JbnMap Properties

- **`BaseAddress Property`**
  ```csharp
  public IntPtr BaseAddress { get; }
  ```
  Gets the base address of this map.

- **`IsActive Property`**
  ```csharp
  public bool IsActive { get; }
  ```
  Gets a value that indicates whether this map is currently active.

- **`Map Property`**
  ```csharp
  public Map Map { get; }
  ```
  Gets the DBC map entry for this map.

- **`MapID Property`**
  ```csharp
  public int MapID { get; }
  ```
  Gets the map ID of this map.

- **`MapName Property`**
  ```csharp
  public string MapName { get; }
  ```
  Gets the name of this map.

- **`MapPath Property`**
  ```csharp
  public string MapPath { get; }
  ```
  Gets the path to this maps directory in Blizzard's internal files.

#### JbnMap Methods

- **`GetAreaTable Method`**
  ```csharp
  public AreaTable&lt;JbnMapAreaTableEntry&gt; GetAreaTable()
  ```
  Gets the area table of this map.
  - **Returns:** See Also

- **`HasAreaAt Method`**
  ```csharp
  public bool HasAreaAt(
int x,
int y
)
  ```
  Gets a value that indicates whether this map has an area at the specified X and Y.
  - *x*: Type: SystemAddLanguageSpecificTextSet("LSTE2186BF3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.
  - *y*: Type: SystemAddLanguageSpecificTextSet("LSTE2186BF3_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.
  - **Returns:** ReferenceJbnMap Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a string that represents the current object.
  - **Returns:** ReferenceJbnMap Class

---

### JbnMapAreaTableEntry Structure

```csharp
public struct JbnMapAreaTableEntry
```

A jbn map area table entry.

#### JbnMapAreaTableEntry Properties

- **`HasTileHere Property`**
  ```csharp
  public bool HasTileHere { get; }
  ```
  Gets a value indicating whether this object has tile here.

#### JbnMapAreaTableEntry Fields

- **`dword4 Field`**
  ```csharp
  public uint dword4
  ```
  The fourth double-word.

- **`dword8 Field`**
  ```csharp
  public uint dword8
  ```
  The double-word 8.

- **`dwordC Field`**
  ```csharp
  public uint dwordC
  ```
  The double-word c.

- **`f1 Field`**
  ```csharp
  public fixed byte f1[3]
  ```
  Gets the 3].

- **`Flags Field`**
  ```csharp
  public byte Flags
  ```
  The flags.

---

### TraceLineHitFlags Enumeration

```csharp
[FlagsAttribute]
public enum TraceLineHitFlags
```

Flags for trace line.

---

### Triangle Structure

```csharp
public struct Triangle : IEquatable&lt;Triangle&gt;
```

Represents a triangle

#### Triangle Properties

- **`Vertices Property`**
  ```csharp
  public IEnumerable&lt;Vector3&gt; Vertices { get; }
  ```
  Gets the vertices.

#### Triangle Methods

- **`Equals Method`**

- **`Equals Method (Object)`**
  ```csharp
  public override bool Equals(
Object obj
)
  ```
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LST1CF4C454_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceTriangle Structure

- **`Equals Method (Triangle)`**
  ```csharp
  public bool Equals(
Triangle other
)
  ```
  - *other*: Type: Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LST6DD2B23_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Triangle
  - **Returns:** See Also

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  - **Returns:** ReferenceTriangle Structure

#### Triangle Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
Triangle left,
Triangle right
)
  ```
  - *left*: Type: Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LST7811199F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Triangle
  - *right*: Type: Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LST7811199F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Triangle
  - **Returns:** ReferenceTriangle Structure

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
Triangle left,
Triangle right
)
  ```
  - *left*: Type: Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LST8882E2BA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Triangle
  - *right*: Type: Styx.WoWInternals.WorldAddLanguageSpecificTextSet("LST8882E2BA_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Triangle
  - **Returns:** ReferenceTriangle Structure

#### Triangle Fields

- **`Normal Field`**
  ```csharp
  public readonly Vector3 Normal
  ```
  The normal

- **`Vertex1 Field`**
  ```csharp
  public readonly Vector3 Vertex1
  ```
  The vertex1

- **`Vertex2 Field`**
  ```csharp
  public readonly Vector3 Vertex2
  ```
  The vertex2

- **`Vertex3 Field`**
  ```csharp
  public readonly Vector3 Vertex3
  ```
  The vertex3

---

### UnitSpellLineOfSightTestEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.WoWInternals.World.UnitSpellLineOfSightTestEventArgs

```csharp
public class UnitSpellLineOfSightTestEventArgs : EventArgs
```

Initializes a new instance of the UnitSpellLineOfSightTestEventArgs class

#### UnitSpellLineOfSightTestEventArgs Properties

- **`Handled Property`**
  ```csharp
  public bool Handled { get; set; }
  ```
  Determines whether spell line-of-sight test was handled.
            InSpellLineOfSight value will be ignored if this value isn't set to true

- **`InSpellLineOfSight Property`**
  ```csharp
  public bool InSpellLineOfSight { get; set; }
  ```
  Contains the spell line-of-sight result. This value is ignored if Handled is not set to true

- **`Unit Property`**
  ```csharp
  public WoWUnit Unit { get; }
  ```
  The NPC that spell line-of-sight is being tested on.

---

### WorldLine Structure

```csharp
public struct WorldLine
```

A world line.

#### WorldLine Fields

- **`End Field`**
  ```csharp
  public Vector3 End
  ```
  The end point.

- **`Start Field`**
  ```csharp
  public Vector3 Start
  ```
  The start point.

---

### WorldMap Class

**Inheritance:** System.Object → Styx.WoWInternals.World.WorldMap

```csharp
public class WorldMap
```

A world map.

#### WorldMap Properties

- **`BaseAddress Property`**
  ```csharp
  public IntPtr BaseAddress { get; }
  ```
  Gets the base address.

- **`MainMap Property`**
  ```csharp
  public Map MainMap { get; }
  ```
  Gets the main map of this world map.

- **`MainMapID Property`**
  ```csharp
  public int MainMapID { get; }
  ```
  Gets the main map ID of this world map.

#### WorldMap Methods

- **`GetAreaTable Method`**
  ```csharp
  public AreaTable&lt;WorldMapAreaTableEntry&gt; GetAreaTable()
  ```
  Gets area table.
  - **Returns:** See Also

- **`GetMap Method`**
  ```csharp
  public JbnMap GetMap(
int index
)
  ```
  Gets a map at a specified index.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTD56ADCB0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.
  - **Returns:** ReferenceWorldMap Class

- **`GetMapAt Method`**
  ```csharp
  public JbnMap GetMapAt(
Vector3 point
)
  ```
  Gets the map at a specified ingame position.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LST3277D1E7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3.
  - **Returns:** ReferenceWorldMap Class

- **`GetMapIdAt Method`**
  ```csharp
  public int GetMapIdAt(
Vector3 point
)
  ```
  Gets the map ID at a specified ingame position. Returns -1 if the point is not on any
            currently loaded map.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LSTD60FCBC0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3.
  - **Returns:** ReferenceWorldMap Class

- **`GetMaps Method`**
  ```csharp
  public IEnumerable&lt;JbnMap&gt; GetMaps()
  ```
  Gets all maps in this world map.
  - **Returns:** See Also

- **`GetMapTileByPosition Method`**
  ```csharp
  public Point GetMapTileByPosition(
Vector2 position
)
  ```
  - *position*: Type: System.NumericsAddLanguageSpecificTextSet("LST8547F541_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector2
  - **Returns:** ReferenceWorldMap Class

#### WorldMap Fields

- **`MaxNumSubMaps Field`**
  ```csharp
  public const int MaxNumSubMaps = 12
  ```
  The maximum number sub maps.

---

### WorldMapAreaTableEntry Structure

```csharp
public struct WorldMapAreaTableEntry
```

A world map area table entry.

#### WorldMapAreaTableEntry Properties

- **`HasAreaHere Property`**
  ```csharp
  public bool HasAreaHere { get; }
  ```
  Gets a value indicating whether this object has area here.

#### WorldMapAreaTableEntry Fields

- **`Flags Field`**
  ```csharp
  public byte Flags
  ```
  The flags.

- **`MapIndex Field`**
  ```csharp
  public sbyte MapIndex
  ```
  Zero-based index of the map.

---

### WorldScene Class

**Inheritance:** System.Object → Styx.WoWInternals.World.WorldScene

```csharp
public class WorldScene
```

A world scene.

#### WorldScene Properties

- **`BaseAddress Property`**
  ```csharp
  public IntPtr BaseAddress { get; }
  ```
  Gets the base address.

- **`WorldMap Property`**
  ```csharp
  public WorldMap WorldMap { get; }
  ```
  Gets the world map.

---
