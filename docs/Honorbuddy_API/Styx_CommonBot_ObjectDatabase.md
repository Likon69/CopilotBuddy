# Styx.CommonBot.ObjectDatabase

Contains classes for the NPC database.

## Contents

- [Connection Class](#connection-class)
- [Connection.Distance2DFunction Class](#connection.distance2dfunction-class)
- [Connection.DistanceFunction Class](#connection.distancefunction-class)
- [Connection.PathDistanceFunction Class](#connection.pathdistancefunction-class)
- [MailboxResult Class](#mailboxresult-class)
- [NpcResult Class](#npcresult-class)
- [Query Class](#query-class)

---

### Connection Class

**Inheritance:** System.Object → Styx.CommonBot.ObjectDatabase.Connection

```csharp
public static class Connection
```

A connection.

#### Connection Properties

- **`Instance Property`**
  ```csharp
  public static SQLiteConnection Instance { get; }
  ```
  Gets the instance.

---

### Connection.Distance2DFunction Class

**Inheritance:** System.Object → SQLiteFunction → Styx.CommonBot.ObjectDatabase.Connection.Distance2DFunction

```csharp
public sealed class Distance2DFunction : SQLiteFunction
```

A distance function.

#### Distance2DFunction Methods

- **`Invoke Method`**
  ```csharp
  public override Object Invoke(
Object[] args
)
  ```
  Scalar functions override this method to do their magic.
  - *args*: Type: AddLanguageSpecificTextSet("LST299D5CDF_2?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST299D5CDF_3?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST299D5CDF_4?cpp=&gt;|vb=()|nu=[]");The arguments for the command to process.
  - **Returns:** ReferenceConnectionAddLanguageSpecificTextSet("LST299D5CDF_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Distance2DFunction Class

---

### Connection.DistanceFunction Class

**Inheritance:** System.Object → SQLiteFunction → Styx.CommonBot.ObjectDatabase.Connection.DistanceFunction

```csharp
public sealed class DistanceFunction : SQLiteFunction
```

A distance function.

#### DistanceFunction Methods

- **`Invoke Method`**
  ```csharp
  public override Object Invoke(
Object[] args
)
  ```
  Scalar functions override this method to do their magic.
  - *args*: Type: AddLanguageSpecificTextSet("LSTF98C0A69_2?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTF98C0A69_3?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTF98C0A69_4?cpp=&gt;|vb=()|nu=[]");The arguments for the command to process.
  - **Returns:** ReferenceConnectionAddLanguageSpecificTextSet("LSTF98C0A69_5?cs=.|vb=.|cpp=::|nu=.|fs=.");DistanceFunction Class

---

### Connection.PathDistanceFunction Class

**Inheritance:** System.Object → SQLiteFunction → Styx.CommonBot.ObjectDatabase.Connection.PathDistanceFunction

```csharp
public sealed class PathDistanceFunction : SQLiteFunction
```

A path distance function.

#### PathDistanceFunction Methods

- **`Invoke Method`**
  ```csharp
  public override Object Invoke(
Object[] args
)
  ```
  Scalar functions override this method to do their magic.
  - *args*: Type: AddLanguageSpecificTextSet("LST8E5FB5C6_2?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST8E5FB5C6_3?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST8E5FB5C6_4?cpp=&gt;|vb=()|nu=[]");The arguments for the command to process.
  - **Returns:** ReferenceConnectionAddLanguageSpecificTextSet("LST8E5FB5C6_5?cs=.|vb=.|cpp=::|nu=.|fs=.");PathDistanceFunction Class

---

### MailboxResult Class

**Inheritance:** System.Object → Styx.CommonBot.ObjectDatabase.MailboxResult

```csharp
public class MailboxResult : IEquatable&lt;MailboxResult&gt;
```

Encapsulates the result of a npc.

#### MailboxResult Properties

- **`Entry Property`**
  ```csharp
  public int Entry { get; }
  ```
  Gets or sets the entry.

- **`FactionId Property`**
  ```csharp
  public uint FactionId { get; }
  ```
  Gets or sets the faction.

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; }
  ```
  Gets the location.

- **`MapId Property`**
  ```csharp
  public int MapId { get; }
  ```
  Gets or sets the map ID.

- **`PlayerConditionId Property`**
  ```csharp
  public uint PlayerConditionId { get; }
  ```
  Gets or sets the player condition identifier.

- **`X Property`**
  ```csharp
  public float X { get; }
  ```
  Gets or sets the x coordinate.

- **`Y Property`**
  ```csharp
  public float Y { get; }
  ```
  Gets or sets the y coordinate.

- **`Z Property`**
  ```csharp
  public float Z { get; }
  ```
  Gets or sets the z coordinate.

#### MailboxResult Methods

- **`Equals Method`**

- **`Equals Method (Object)`**
  ```csharp
  public override bool Equals(
Object obj
)
  ```
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LSTA7EEB52A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceMailboxResult Class

- **`Equals Method (MailboxResult)`**
  ```csharp
  public bool Equals(
MailboxResult other
)
  ```
  - *other*: Type: Styx.CommonBot.ObjectDatabaseAddLanguageSpecificTextSet("LSTDE913F03_1?cs=.|vb=.|cpp=::|nu=.|fs=.");MailboxResult
  - **Returns:** See Also

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  - **Returns:** ReferenceMailboxResult Class

#### MailboxResult Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
MailboxResult left,
MailboxResult right
)
  ```
  - *left*: Type: Styx.CommonBot.ObjectDatabaseAddLanguageSpecificTextSet("LST26C98D59_1?cs=.|vb=.|cpp=::|nu=.|fs=.");MailboxResult
  - *right*: Type: Styx.CommonBot.ObjectDatabaseAddLanguageSpecificTextSet("LST26C98D59_2?cs=.|vb=.|cpp=::|nu=.|fs=.");MailboxResult
  - **Returns:** ReferenceMailboxResult Class

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
MailboxResult left,
MailboxResult right
)
  ```
  - *left*: Type: Styx.CommonBot.ObjectDatabaseAddLanguageSpecificTextSet("LSTA82455A2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");MailboxResult
  - *right*: Type: Styx.CommonBot.ObjectDatabaseAddLanguageSpecificTextSet("LSTA82455A2_2?cs=.|vb=.|cpp=::|nu=.|fs=.");MailboxResult
  - **Returns:** ReferenceMailboxResult Class

---

### NpcResult Class

**Inheritance:** System.Object → Styx.CommonBot.ObjectDatabase.NpcResult

```csharp
public class NpcResult : IEquatable&lt;NpcResult&gt;
```

Encapsulates the result of a npc.

#### NpcResult Properties

- **`Entry Property`**
  ```csharp
  public int Entry { get; }
  ```
  Gets the entry.

- **`Faction Property`**
  ```csharp
  public uint Faction { get; }
  ```
  Gets the faction template ID.

- **`IsHostile Property`**
  ```csharp
  public bool IsHostile { get; }
  ```
  Gets a bool that indicates whether this unit is hostile towards the local player.

- **`Level Property`**
  ```csharp
  public int Level { get; }
  ```
  Gets the npc level.

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; }
  ```
  Gets the location.

- **`MapId Property`**
  ```csharp
  public int MapId { get; }
  ```
  Gets the identifier of the map.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`NpcFlags Property`**
  ```csharp
  public uint NpcFlags { get; }
  ```
  Gets the npc flags.

- **`NpcFlags2 Property`**
  ```csharp
  public uint NpcFlags2 { get; }
  ```
  Gets the npc flags 2.

- **`Title Property`**
  ```csharp
  public string Title { get; }
  ```
  Gets the title.

- **`TrainerClass Property`**
  ```csharp
  public int TrainerClass { get; }
  ```
  Gets the trainer class.

- **`X Property`**
  ```csharp
  public float X { get; }
  ```
  Gets the x coordinate.

- **`Y Property`**
  ```csharp
  public float Y { get; }
  ```
  Gets the y coordinate.

- **`Z Property`**
  ```csharp
  public float Z { get; }
  ```
  Gets the z coordinate.

#### NpcResult Methods

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
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LST9B70765D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe Object to compare with the current
            Object.
  - **Returns:** ReferenceNpcResult Class

- **`Equals Method (NpcResult)`**
  ```csharp
  public bool Equals(
NpcResult other
)
  ```
  Indicates whether the current object is equal to another object of the same type.
  - *other*: Type: Styx.CommonBot.ObjectDatabaseAddLanguageSpecificTextSet("LST6B9DA89F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");NpcResultAn object to compare with this object.
  - **Returns:** See Also

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  Serves as a hash function for a particular type.
  - **Returns:** ReferenceNpcResult Class

#### NpcResult Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
NpcResult left,
NpcResult right
)
  ```
  Equality operator.
  - *left*: Type: Styx.CommonBot.ObjectDatabaseAddLanguageSpecificTextSet("LSTF4FB3E46_1?cs=.|vb=.|cpp=::|nu=.|fs=.");NpcResult The left.
  - *right*: Type: Styx.CommonBot.ObjectDatabaseAddLanguageSpecificTextSet("LSTF4FB3E46_2?cs=.|vb=.|cpp=::|nu=.|fs=.");NpcResultThe right.
  - **Returns:** ReferenceNpcResult Class

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
NpcResult left,
NpcResult right
)
  ```
  Inequality operator.
  - *left*: Type: Styx.CommonBot.ObjectDatabaseAddLanguageSpecificTextSet("LST6828C56B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");NpcResult The left.
  - *right*: Type: Styx.CommonBot.ObjectDatabaseAddLanguageSpecificTextSet("LST6828C56B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");NpcResultThe right.
  - **Returns:** ReferenceNpcResult Class

---

### Query Class

**Inheritance:** System.Object → Styx.CommonBot.ObjectDatabase.Query

```csharp
public static class Query
```

Database queries.

#### Query Methods

- **`GetNearestMailbox Method`**
  Gets the nearest mailbox that can be used by player on currently active maps (continent and phased maps).

- **`GetNearestMailbox Method (Vector3, Func(MailboxResult, Boolean))`**
  ```csharp
  public static MailboxResult GetNearestMailbox(
Vector3 searchLocation,
Func&lt;MailboxResult, bool&gt; ignoreFilter = null
)
  ```
  Gets the nearest mailbox that can be used by player on currently active maps (continent and phased maps).
  - *searchLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LSTE7DD0CAE_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The search location.
  - **Returns:** ReferenceQuery Class

- **`GetNearestMailbox Method (UInt32, Vector3, Func(MailboxResult, Boolean))`**
  ```csharp
  public static MailboxResult GetNearestMailbox(
uint mapId,
Vector3 searchLocation,
Func&lt;MailboxResult, bool&gt; ignoreFilter = null
)
  ```
  Gets the nearest mailbox that can be used by player.
  - *mapId*: Type: SystemAddLanguageSpecificTextSet("LSTA548929E_3?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The map ID.
  - *searchLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LSTA548929E_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The search location.
  - **Returns:** ReferenceQuery Class

- **`GetNearestNpc Method`**
  ```csharp
  public static NpcResult GetNearestNpc(
uint mapId,
Vector3 searchLocation,
UnitNPCFlags npcFlags,
Func&lt;NpcResult, bool&gt; extraConditions = null
)
  ```
  Gets a nearest npc.
  - *mapId*: Type: SystemAddLanguageSpecificTextSet("LST43F0CE32_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32Identifier for the map.
  - *searchLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LST43F0CE32_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The search location.
  - *npcFlags*: Type: StyxAddLanguageSpecificTextSet("LST43F0CE32_3?cs=.|vb=.|cpp=::|nu=.|fs=.");UnitNPCFlagsThe npc flags.
  - **Returns:** Created 10/1/2010.

- **`GetNearestNpcById Method`**
  ```csharp
  public static NpcResult GetNearestNpcById(
uint id,
Vector3 searchLocation
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST1715F886_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *searchLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LST1715F886_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - **Returns:** ReferenceQuery Class

- **`GetNearestTrainer Method`**
  ```csharp
  public static NpcResult GetNearestTrainer(
uint mapId,
Vector3 searchLocation,
WoWClass searchClass
)
  ```
  Gets the nearest trainer.
  - *mapId*: Type: SystemAddLanguageSpecificTextSet("LST4038D467_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32         Identifier for the map.
  - *searchLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LST4038D467_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The search location.
  - *searchClass*: Type: StyxAddLanguageSpecificTextSet("LST4038D467_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWClass   The search class.
  - **Returns:** ReferenceQuery Class

- **`GetNpcById Method`**
  ```csharp
  public static NpcResult GetNpcById(
uint id
)
  ```
  Gets a npc by its ID.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTAB8244C3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceQuery Class

- **`GetNpcByName Method`**
  ```csharp
  public static NpcResult GetNpcByName(
string name
)
  ```
  Gets a npc by name.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LST14FE8C97_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name.
  - **Returns:** ReferenceQuery Class

---
