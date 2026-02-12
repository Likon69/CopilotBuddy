# Bots.DungeonBuddy

Contains DungeonBuddy related members.

## Contents

- [BossManager Class](#bossmanager-class)
- [Dungeon Class](#dungeon-class)
- [DungeonBot Class](#dungeonbot-class)
- [DungeonManager Class](#dungeonmanager-class)
- [GroupMember Class](#groupmember-class)

---

### BossManager Class

**Inheritance:** System.Object → Bots.DungeonBuddy.BossManager

```csharp
public static class BossManager
```

Returns the Boss that is at the top of the list to kill or null if no bosses are left to kill

#### BossManager Properties

- **`BossEncounters Property`**
  ```csharp
  public static List&lt;Boss&gt; BossEncounters { get; }
  ```

- **`CurrentBoss Property`**
  ```csharp
  public static Boss CurrentBoss { get; }
  ```
  Returns the Boss that is at the top of the list to kill or null if no bosses are left to kill

#### BossManager Methods

- **`Attach Method`**
  ```csharp
  public static void Attach(
Profile profile
)
  ```
  - *profile*: Type: Profile

- **`CreateCheckForDeadBoss Method`**
  ```csharp
  public static Task&lt;bool&gt; CreateCheckForDeadBoss()
  ```
  - **Returns:** See Also

- **`Detach Method`**
  ```csharp
  public static void Detach()
  ```

- **`Reset Method`**
  ```csharp
  public static void Reset()
  ```
  Resets each boss encounter entry

#### BossManager Events

- **`OnBossKill Event`**
  ```csharp
  public static event Action&lt;Boss&gt; OnBossKill
  ```

#### BossManager Fields

- **`BossTimer Field`**
  ```csharp
  public static readonly Stopwatch BossTimer
  ```

---

### Dungeon Class

**Inheritance:** System.Object → Bots.DungeonBuddy.Dungeon

```csharp
public abstract class Dungeon
```

Dungeon, contrains all required information about a specific dungeon.

#### Dungeon Properties

- **`CanExitNow Property`**
  ```csharp
  public virtual bool CanExitNow { get; }
  ```
  Gets a value indicating whether dungeon can be exited now, usually to do a vendor run e.g repair items, buy drinks, empty bags
            Should be returning 'false' during events where staying until event is over would be a better idea. 
            Prime example would be Violet Hold while the door is blocked.

- **`CorpseRunBreadCrumb Property`**
  ```csharp
  public virtual CircularQueue&lt;Vector3&gt; CorpseRunBreadCrumb { get; }
  ```
  Needs to be overriden if the corpse run to the dungeon needs a breadcrumb

- **`DungeonId Property`**
  ```csharp
  public abstract uint DungeonId { get; }
  ```
  The mapid of this dungeon.

- **`Entrance Property`**
  ```csharp
  public virtual Vector3 Entrance { get; }
  ```
  The entrance of this dungeon as a Vector3.

- **`ExitLocation Property`**
  ```csharp
  public virtual Vector3 ExitLocation { get; }
  ```
  The exit of this dungeon as a Vector3.

- **`IsComplete Property`**
  ```csharp
  public virtual bool IsComplete { get; }
  ```
  Gets a value indicating whether this instance is complete. By default this returns true when all bosses are
                dead when 'Kill Optional bosses' setting is true or all non-optional bosses are dead if 'Kill Optional bosses' is
                false

- **`IsFlyingCorpseRun Property`**
  ```csharp
  public virtual bool IsFlyingCorpseRun { get; }
  ```
  True if it's required to fly to get back to the dungeon entrance when doing a corpse run.

- **`LfgDungeon Property`**
  ```csharp
  public LfgDungeons LfgDungeon { get; }
  ```
  The map of this dungeon.

- **`Name Property`**
  ```csharp
  public virtual string Name { get; }
  ```
  The name of this dungeon.

- **`PathForks Property`**
  ```csharp
  public virtual DungeonPathForkInfo[] PathForks { get; }
  ```

- **`SnapToWaterSurfaceWhileSwimming Property`**
  ```csharp
  public virtual bool SnapToWaterSurfaceWhileSwimming { get; }
  ```
  Snaps start postion to water surface while swimming if true (Default); otherwise smaps to nearest surface.

- **`StuckHandlers Property`**
  ```csharp
  public virtual DungeonStuckHandler[] StuckHandlers { get; }
  ```
  Collection of stuck handlers for this dungeon. See DungeonStuckHandler.

#### Dungeon Methods

- **`AddAvoidLine Method`**
  Avoids a line

- **`AddAvoidLine Method (Func(Boolean), Func(Single), Func(Vector3), Func(Vector3), Boolean, AvoidancePriority)`**
  ```csharp
  [ObsoleteAttribute("Use the other versions of AddAvoidLine")]
protected void AddAvoidLine(
Func&lt;bool&gt; canRun,
Func&lt;float&gt; lineThicknessProducer,
Func&lt;Vector3&gt; startLocationProducer,
Func&lt;Vector3&gt; endLocationProducer,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
  ```
  Avoids a line
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST88C16B4E_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST88C16B4E_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST88C16B4E_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - *lineThicknessProducer*: Type: SystemAddLanguageSpecificTextSet("LST88C16B4E_12?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST88C16B4E_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");SingleAddLanguageSpecificTextSet("LST88C16B4E_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The line thickness producer.
  - *startLocationProducer*: Type: SystemAddLanguageSpecificTextSet("LST88C16B4E_15?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST88C16B4E_16?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST88C16B4E_17?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The start location producer.
  - *endLocationProducer*: Type: SystemAddLanguageSpecificTextSet("LST88C16B4E_18?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST88C16B4E_19?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST88C16B4E_20?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The end location producer.

- **`AddAvoidLine(T) Method (Func(Boolean), Func(T, Vector3), Func(T, Single), Func(T, Single), Func(T, Single), Predicate(T), Boolean, AvoidancePriority)`**
  ```csharp
  protected void AddAvoidLine&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;T, Vector3&gt; startLocationProducer,
Func&lt;T, float&gt; rotationProducer,
Func&lt;T, float&gt; lengthProducer,
Func&lt;T, float&gt; thicknessProducer,
Predicate&lt;T&gt; objectSelector,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
where T : WoWObject
  ```
  Avoids a line
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTE3B4E6D_15?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTE3B4E6D_16?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTE3B4E6D_17?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - *startLocationProducer*: Type: SystemAddLanguageSpecificTextSet("LSTE3B4E6D_18?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTE3B4E6D_19?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, Vector3AddLanguageSpecificTextSet("LSTE3B4E6D_20?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The line start location producer.
  - *rotationProducer*: Type: SystemAddLanguageSpecificTextSet("LSTE3B4E6D_21?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTE3B4E6D_22?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LSTE3B4E6D_23?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The line rotation around the line start position startLocationProducer
  - *lengthProducer*: Type: SystemAddLanguageSpecificTextSet("LSTE3B4E6D_24?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTE3B4E6D_25?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LSTE3B4E6D_26?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Length of line producer
  - *thicknessProducer*: Type: SystemAddLanguageSpecificTextSet("LSTE3B4E6D_27?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTE3B4E6D_28?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LSTE3B4E6D_29?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The line thickness producer.
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LSTE3B4E6D_30?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LSTE3B4E6D_31?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTE3B4E6D_32?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Selects the WoWObject

- **`AddAvoidLine(T) Method (Func(Boolean), Func(T, Vector3), Func(T, Single), Func(T, Single), Func(T, Single), Func(Vector3), Single, Predicate(T), Boolean, AvoidancePriority)`**
  ```csharp
  protected void AddAvoidLine&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;T, Vector3&gt; startLocationProducer,
Func&lt;T, float&gt; rotationProducer,
Func&lt;T, float&gt; lengthProducer,
Func&lt;T, float&gt; thicknessProducer,
Func&lt;Vector3&gt; leashPointProducer,
float leashRadius,
Predicate&lt;T&gt; objectSelector,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
where T : WoWObject
  ```
  Avoids a line
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTE486EE8_17?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTE486EE8_18?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTE486EE8_19?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - *startLocationProducer*: Type: SystemAddLanguageSpecificTextSet("LSTE486EE8_20?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTE486EE8_21?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, Vector3AddLanguageSpecificTextSet("LSTE486EE8_22?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The line start location producer.
  - *rotationProducer*: Type: SystemAddLanguageSpecificTextSet("LSTE486EE8_23?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTE486EE8_24?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LSTE486EE8_25?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The line rotation around the line start position startLocationProducer
  - *lengthProducer*: Type: SystemAddLanguageSpecificTextSet("LSTE486EE8_26?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTE486EE8_27?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LSTE486EE8_28?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Length of line producer
  - *thicknessProducer*: Type: SystemAddLanguageSpecificTextSet("LSTE486EE8_29?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTE486EE8_30?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LSTE486EE8_31?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The line thickness producer.
  - *leashPointProducer*: Type: SystemAddLanguageSpecificTextSet("LSTE486EE8_32?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTE486EE8_33?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTE486EE8_34?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The leash point producer
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LSTE486EE8_35?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe leash radius
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LSTE486EE8_36?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LSTE486EE8_37?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTE486EE8_38?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Selects the WoWObject

- **`AddAvoidLine(T) Method (Func(Boolean), Func(T, Vector3), Func(T, Single), Func(T, Single), Func(T, Single), Func(IEnumerable(T)), Func(Vector3), Single, Func(T, Boolean), Boolean, AvoidancePriority)`**
  ```csharp
  protected void AddAvoidLine&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;T, Vector3&gt; startLocationProducer,
Func&lt;T, float&gt; rotationProducer,
Func&lt;T, float&gt; lengthProducer,
Func&lt;T, float&gt; thicknessProducer,
Func&lt;IEnumerable&lt;T&gt;&gt; collectionProducer,
Func&lt;Vector3&gt; leashPointProducer,
float leashRadius,
Func&lt;T, bool&gt; objectValidator = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
  ```
  Avoids a line
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST2453E509_21?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST2453E509_22?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST2453E509_23?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - *startLocationProducer*: Type: SystemAddLanguageSpecificTextSet("LST2453E509_24?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST2453E509_25?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, Vector3AddLanguageSpecificTextSet("LST2453E509_26?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The line start location producer.
  - *rotationProducer*: Type: SystemAddLanguageSpecificTextSet("LST2453E509_27?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST2453E509_28?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LST2453E509_29?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The line rotation around the line start position startLocationProducer
  - *lengthProducer*: Type: SystemAddLanguageSpecificTextSet("LST2453E509_30?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST2453E509_31?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LST2453E509_32?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Length of line producer
  - *thicknessProducer*: Type: SystemAddLanguageSpecificTextSet("LST2453E509_33?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST2453E509_34?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LST2453E509_35?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The line thickness producer.
  - *collectionProducer*: Type: SystemAddLanguageSpecificTextSet("LST2453E509_36?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST2453E509_37?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");IEnumerableAddLanguageSpecificTextSet("LST2453E509_38?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST2453E509_39?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");AddLanguageSpecificTextSet("LST2453E509_40?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The collection producer
  - *leashPointProducer*: Type: SystemAddLanguageSpecificTextSet("LST2453E509_41?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST2453E509_42?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST2453E509_43?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The leash point producer
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LST2453E509_44?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe leash radius

- **`AddAvoidLocation Method`**
  Run away from location.

- **`AddAvoidLocation Method (Func(Boolean), Func(Single), Func(Vector3))`**
  ```csharp
  protected void AddAvoidLocation(
Func&lt;bool&gt; canRun,
Func&lt;float&gt; radiusProducer,
Func&lt;Vector3&gt; locationProducer
)
  ```
  Run away from location.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST82757CF_7?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST82757CF_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST82757CF_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - *radiusProducer*: Type: SystemAddLanguageSpecificTextSet("LST82757CF_10?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST82757CF_11?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");SingleAddLanguageSpecificTextSet("LST82757CF_12?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The radius producer.
  - *locationProducer*: Type: SystemAddLanguageSpecificTextSet("LST82757CF_13?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST82757CF_14?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST82757CF_15?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The location selector.

- **`AddAvoidLocation Method (Func(Boolean), Single, Func(Vector3))`**
  ```csharp
  protected void AddAvoidLocation(
Func&lt;bool&gt; canRun,
float radius,
Func&lt;Vector3&gt; locationProducer
)
  ```
  Run away from location.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTCDEF4C_5?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTCDEF4C_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTCDEF4C_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LSTCDEF4C_8?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe radius.
  - *locationProducer*: Type: SystemAddLanguageSpecificTextSet("LSTCDEF4C_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTCDEF4C_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTCDEF4C_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The location selector.

- **`AddAvoidLocation(T) Method (Func(Boolean), Func(T, Single), Func(T, Vector3), Func(IEnumerable(T)), Func(T, Boolean), Boolean, AvoidancePriority)`**
  ```csharp
  protected void AddAvoidLocation&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;T, float&gt; radiusSelector,
Func&lt;T, Vector3&gt; locationSelector,
Func&lt;IEnumerable&lt;T&gt;&gt; collectionSelector,
Func&lt;T, bool&gt; objectValidator = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
  ```
  Run away from location.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST2BF1229B_15?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST2BF1229B_16?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST2BF1229B_17?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - *radiusSelector*: Type: SystemAddLanguageSpecificTextSet("LST2BF1229B_18?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST2BF1229B_19?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LST2BF1229B_20?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The max distance to run.
  - *locationSelector*: Type: SystemAddLanguageSpecificTextSet("LST2BF1229B_21?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST2BF1229B_22?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, Vector3AddLanguageSpecificTextSet("LST2BF1229B_23?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The location selector.
  - *collectionSelector*: Type: SystemAddLanguageSpecificTextSet("LST2BF1229B_24?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST2BF1229B_25?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");IEnumerableAddLanguageSpecificTextSet("LST2BF1229B_26?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST2BF1229B_27?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");AddLanguageSpecificTextSet("LST2BF1229B_28?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Optional collection of objects that are passed as args to LocationSelector

- **`AddAvoidLocation(T) Method (Func(Boolean), Single, Func(T, Vector3), Func(IEnumerable(T)), Func(T, Boolean), Boolean, AvoidancePriority)`**
  ```csharp
  protected void AddAvoidLocation&lt;T&gt;(
Func&lt;bool&gt; canRun,
float radius,
Func&lt;T, Vector3&gt; locationSelector,
Func&lt;IEnumerable&lt;T&gt;&gt; collectionSelector,
Func&lt;T, bool&gt; objectValidator = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
  ```
  Run away from location.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST41484444_13?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST41484444_14?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST41484444_15?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST41484444_16?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe radius.
  - *locationSelector*: Type: SystemAddLanguageSpecificTextSet("LST41484444_17?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST41484444_18?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, Vector3AddLanguageSpecificTextSet("LST41484444_19?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The location selector.
  - *collectionSelector*: Type: SystemAddLanguageSpecificTextSet("LST41484444_20?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST41484444_21?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");IEnumerableAddLanguageSpecificTextSet("LST41484444_22?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST41484444_23?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");AddLanguageSpecificTextSet("LST41484444_24?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Optional collection of objects that are passed as args to LocationSelector

- **`AddAvoidLocation(T) Method (Func(Boolean), Func(Vector3), Single, Func(T, Single), Func(T, Vector3), Func(IEnumerable(T)), Func(T, Boolean), Boolean, AvoidancePriority)`**
  ```csharp
  protected void AddAvoidLocation&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;Vector3&gt; leashPointSelector,
float leashRadius,
Func&lt;T, float&gt; radiusSelector,
Func&lt;T, Vector3&gt; locationSelector,
Func&lt;IEnumerable&lt;T&gt;&gt; collectionSelector,
Func&lt;T, bool&gt; objectValidator = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
  ```
  Run away from location.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST9A459D94_17?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST9A459D94_18?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST9A459D94_19?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - *leashPointSelector*: Type: SystemAddLanguageSpecificTextSet("LST9A459D94_20?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST9A459D94_21?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST9A459D94_22?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The leash point selector.
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LST9A459D94_23?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe leash radius.
  - *radiusSelector*: Type: SystemAddLanguageSpecificTextSet("LST9A459D94_24?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST9A459D94_25?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LST9A459D94_26?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The max distance to run.
  - *locationSelector*: Type: SystemAddLanguageSpecificTextSet("LST9A459D94_27?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST9A459D94_28?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, Vector3AddLanguageSpecificTextSet("LST9A459D94_29?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The location selector.
  - *collectionSelector*: Type: SystemAddLanguageSpecificTextSet("LST9A459D94_30?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST9A459D94_31?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");IEnumerableAddLanguageSpecificTextSet("LST9A459D94_32?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST9A459D94_33?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");AddLanguageSpecificTextSet("LST9A459D94_34?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Optional collection of objects that are passed as args to LocationSelector

- **`AddAvoidLocation(T) Method (Func(Boolean), Func(Vector3), Single, Single, Func(T, Vector3), Func(IEnumerable(T)), Func(T, Boolean), Boolean, AvoidancePriority)`**
  ```csharp
  protected void AddAvoidLocation&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;Vector3&gt; leashPointSelector,
float leashRadius,
float radius,
Func&lt;T, Vector3&gt; locationSelector,
Func&lt;IEnumerable&lt;T&gt;&gt; collectionSelector,
Func&lt;T, bool&gt; objectValidator = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
  ```
  Run away from location.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTAAA390C9_15?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTAAA390C9_16?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTAAA390C9_17?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - *leashPointSelector*: Type: SystemAddLanguageSpecificTextSet("LSTAAA390C9_18?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTAAA390C9_19?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTAAA390C9_20?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The leash point selector.
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LSTAAA390C9_21?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe leash radius.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LSTAAA390C9_22?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe radius.
  - *locationSelector*: Type: SystemAddLanguageSpecificTextSet("LSTAAA390C9_23?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTAAA390C9_24?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, Vector3AddLanguageSpecificTextSet("LSTAAA390C9_25?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The location selector.
  - *collectionSelector*: Type: SystemAddLanguageSpecificTextSet("LSTAAA390C9_26?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTAAA390C9_27?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");IEnumerableAddLanguageSpecificTextSet("LSTAAA390C9_28?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTAAA390C9_29?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");AddLanguageSpecificTextSet("LSTAAA390C9_30?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Optional collection of objects that are passed as args to LocationSelector

- **`AddAvoidObject Method`**
  Runs away from a unit or object if within range.

- **`AddAvoidObject Method (Single, UInt32[])`**
  ```csharp
  [ObsoleteAttribute("Use AddAvoidObject&lt;T&gt; instead")]
protected void AddAvoidObject(
float radius,
params uint[] unitIds
)
  ```
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST9BD8D6D2_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *unitIds*: Type: AddLanguageSpecificTextSet("LST9BD8D6D2_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST9BD8D6D2_5?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32AddLanguageSpecificTextSet("LST9BD8D6D2_6?cpp=&gt;|vb=()|nu=[]");

- **`AddAvoidObject(T) Method (Single, UInt32[])`**
  ```csharp
  protected void AddAvoidObject&lt;T&gt;(
float radius,
params uint[] unitIds
)
where T : WoWObject
  ```
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST5F79E613_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *unitIds*: Type: AddLanguageSpecificTextSet("LST5F79E613_6?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST5F79E613_7?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32AddLanguageSpecificTextSet("LST5F79E613_8?cpp=&gt;|vb=()|nu=[]");

- **`AddAvoidObject Method (Func(Boolean), Func(WoWObject, Single), UInt32[])`**
  ```csharp
  [ObsoleteAttribute("Use AddAvoidObject&lt;T&gt; instead")]
protected void AddAvoidObject(
Func&lt;bool&gt; canRun,
Func&lt;WoWObject, float&gt; radiusSelector,
params uint[] unitIds
)
  ```
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTECFD5685_7?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTECFD5685_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTECFD5685_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *radiusSelector*: Type: SystemAddLanguageSpecificTextSet("LSTECFD5685_10?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTECFD5685_11?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObject, SingleAddLanguageSpecificTextSet("LSTECFD5685_12?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *unitIds*: Type: AddLanguageSpecificTextSet("LSTECFD5685_13?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTECFD5685_14?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32AddLanguageSpecificTextSet("LSTECFD5685_15?cpp=&gt;|vb=()|nu=[]");

- **`AddAvoidObject(T) Method (Func(Boolean), Func(T, Single), UInt32[])`**
  ```csharp
  protected void AddAvoidObject&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;T, float&gt; radiusSelector,
params uint[] unitIds
)
where T : WoWObject
  ```
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST46B85AF4_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST46B85AF4_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST46B85AF4_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *radiusSelector*: Type: SystemAddLanguageSpecificTextSet("LST46B85AF4_12?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST46B85AF4_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LST46B85AF4_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *unitIds*: Type: AddLanguageSpecificTextSet("LST46B85AF4_15?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST46B85AF4_16?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32AddLanguageSpecificTextSet("LST46B85AF4_17?cpp=&gt;|vb=()|nu=[]");

- **`AddAvoidObject Method (Func(Boolean), Single, UInt32[])`**
  ```csharp
  protected void AddAvoidObject(
Func&lt;bool&gt; canRun,
float radius,
params uint[] unitIds
)
  ```
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST7FC4F128_5?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST7FC4F128_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST7FC4F128_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST7FC4F128_8?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *unitIds*: Type: AddLanguageSpecificTextSet("LST7FC4F128_9?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST7FC4F128_10?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32AddLanguageSpecificTextSet("LST7FC4F128_11?cpp=&gt;|vb=()|nu=[]");

- **`AddAvoidObject Method (Func(Boolean), Func(Vector3), Single, Func(WoWObject, Single), UInt32[])`**
  ```csharp
  protected void AddAvoidObject(
Func&lt;bool&gt; canRun,
Func&lt;Vector3&gt; leashPointSelector,
float leashRadius,
Func&lt;WoWObject, float&gt; radiusSelector,
params uint[] unitIds
)
  ```
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST1918B206_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1918B206_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST1918B206_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *leashPointSelector*: Type: SystemAddLanguageSpecificTextSet("LST1918B206_12?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1918B206_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST1918B206_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LST1918B206_15?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *radiusSelector*: Type: SystemAddLanguageSpecificTextSet("LST1918B206_16?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1918B206_17?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObject, SingleAddLanguageSpecificTextSet("LST1918B206_18?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *unitIds*: Type: AddLanguageSpecificTextSet("LST1918B206_19?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST1918B206_20?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32AddLanguageSpecificTextSet("LST1918B206_21?cpp=&gt;|vb=()|nu=[]");

- **`AddAvoidObject(T) Method (Func(Boolean), Func(Vector3), Single, Func(T, Single), UInt32[])`**
  ```csharp
  protected void AddAvoidObject&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;Vector3&gt; leashPointSelector,
float leashRadius,
Func&lt;T, float&gt; radiusSelector,
params uint[] unitIds
)
where T : WoWObject
  ```
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTA22DAB01_11?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTA22DAB01_12?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTA22DAB01_13?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *leashPointSelector*: Type: SystemAddLanguageSpecificTextSet("LSTA22DAB01_14?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTA22DAB01_15?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTA22DAB01_16?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LSTA22DAB01_17?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *radiusSelector*: Type: SystemAddLanguageSpecificTextSet("LSTA22DAB01_18?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTA22DAB01_19?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LSTA22DAB01_20?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *unitIds*: Type: AddLanguageSpecificTextSet("LSTA22DAB01_21?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTA22DAB01_22?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32AddLanguageSpecificTextSet("LSTA22DAB01_23?cpp=&gt;|vb=()|nu=[]");

- **`AddAvoidObject Method (Func(Boolean), Func(Vector3), Single, Single, UInt32[])`**
  ```csharp
  protected void AddAvoidObject(
Func&lt;bool&gt; canRun,
Func&lt;Vector3&gt; leashPointSelector,
float leashRadius,
float radius,
params uint[] unitIds
)
  ```
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTD9EF7013_7?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTD9EF7013_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTD9EF7013_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *leashPointSelector*: Type: SystemAddLanguageSpecificTextSet("LSTD9EF7013_10?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTD9EF7013_11?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTD9EF7013_12?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LSTD9EF7013_13?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LSTD9EF7013_14?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *unitIds*: Type: AddLanguageSpecificTextSet("LSTD9EF7013_15?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTD9EF7013_16?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32AddLanguageSpecificTextSet("LSTD9EF7013_17?cpp=&gt;|vb=()|nu=[]");

- **`AddAvoidObject Method (Single, Predicate(WoWObject), Func(WoWObject, Vector3), Boolean, AvoidancePriority)`**
  ```csharp
  [ObsoleteAttribute("Use AddAvoidObject&lt;T&gt; instead")]
protected void AddAvoidObject(
float radius,
Predicate&lt;WoWObject&gt; objectSelector,
Func&lt;WoWObject, Vector3&gt; locationSelector = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
  ```
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST78D582F4_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LST78D582F4_6?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LST78D582F4_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST78D582F4_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`AddAvoidObject(T) Method (Single, Predicate(T), Func(T, Vector3), Boolean, AvoidancePriority)`**
  ```csharp
  protected void AddAvoidObject&lt;T&gt;(
float radius,
Predicate&lt;T&gt; objectSelector,
Func&lt;T, Vector3&gt; locationSelector = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
where T : WoWObject
  ```
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LSTF87BECDD_7?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LSTF87BECDD_8?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LSTF87BECDD_9?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTF87BECDD_10?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`AddAvoidObject Method (Func(Boolean), Func(WoWObject, Single), Predicate(WoWObject), Func(WoWObject, Vector3), Boolean, AvoidancePriority)`**
  ```csharp
  [ObsoleteAttribute("Use AddAvoidObject&lt;T&gt; instead")]
protected void AddAvoidObject(
Func&lt;bool&gt; canRun,
Func&lt;WoWObject, float&gt; radiusSelector,
Predicate&lt;WoWObject&gt; objectSelector,
Func&lt;WoWObject, Vector3&gt; locationSelector = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
  ```
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTD6256C27_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTD6256C27_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTD6256C27_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *radiusSelector*: Type: SystemAddLanguageSpecificTextSet("LSTD6256C27_12?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTD6256C27_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObject, SingleAddLanguageSpecificTextSet("LSTD6256C27_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LSTD6256C27_15?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LSTD6256C27_16?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LSTD6256C27_17?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`AddAvoidObject(T) Method (Func(Boolean), Func(T, Single), Predicate(T), Func(T, Vector3), Boolean, AvoidancePriority)`**
  ```csharp
  protected void AddAvoidObject&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;T, float&gt; radiusSelector,
Predicate&lt;T&gt; objectSelector,
Func&lt;T, Vector3&gt; locationSelector = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
where T : WoWObject
  ```
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTD10EBE4_11?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTD10EBE4_12?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTD10EBE4_13?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *radiusSelector*: Type: SystemAddLanguageSpecificTextSet("LSTD10EBE4_14?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTD10EBE4_15?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LSTD10EBE4_16?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LSTD10EBE4_17?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LSTD10EBE4_18?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTD10EBE4_19?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`AddAvoidObject Method (Func(Boolean), Single, Predicate(WoWObject), Func(WoWObject, Vector3), Boolean, AvoidancePriority)`**
  ```csharp
  protected void AddAvoidObject(
Func&lt;bool&gt; canRun,
float radius,
Predicate&lt;WoWObject&gt; objectSelector,
Func&lt;WoWObject, Vector3&gt; locationSelector = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
  ```
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST53B852EA_7?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST53B852EA_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST53B852EA_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST53B852EA_10?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LST53B852EA_11?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LST53B852EA_12?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST53B852EA_13?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`AddAvoidObject(T) Method (Func(Boolean), Single, Predicate(T), Func(T, Vector3), Boolean, AvoidancePriority)`**
  ```csharp
  protected void AddAvoidObject&lt;T&gt;(
Func&lt;bool&gt; canRun,
float radius,
Predicate&lt;T&gt; objectSelector,
Func&lt;T, Vector3&gt; locationSelector = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
where T : WoWObject
  ```
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST34A09129_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST34A09129_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST34A09129_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST34A09129_12?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LST34A09129_13?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LST34A09129_14?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST34A09129_15?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`AddAvoidObject Method (Func(Boolean), Func(Vector3), Single, Func(WoWObject, Single), Predicate(WoWObject), Func(WoWObject, Vector3), Boolean, AvoidancePriority)`**
  ```csharp
  protected void AddAvoidObject(
Func&lt;bool&gt; canRun,
Func&lt;Vector3&gt; leashPointSelector,
float leashRadius,
Func&lt;WoWObject, float&gt; radiusSelector,
Predicate&lt;WoWObject&gt; objectSelector,
Func&lt;WoWObject, Vector3&gt; locationSelector = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
  ```
  Runs away from a unit or object if within range.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTF34673D0_11?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTF34673D0_12?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTF34673D0_13?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition to run.
  - *leashPointSelector*: Type: SystemAddLanguageSpecificTextSet("LSTF34673D0_14?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTF34673D0_15?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTF34673D0_16?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The leash center point selector. Unit location if null
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LSTF34673D0_17?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe max distance to run from leash.
  - *radiusSelector*: Type: SystemAddLanguageSpecificTextSet("LSTF34673D0_18?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTF34673D0_19?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObject, SingleAddLanguageSpecificTextSet("LSTF34673D0_20?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The distance to run
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LSTF34673D0_21?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LSTF34673D0_22?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LSTF34673D0_23?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The object selector.

- **`AddAvoidObject(T) Method (Func(Boolean), Func(Vector3), Single, Func(T, Single), Predicate(T), Func(T, Vector3), Boolean, AvoidancePriority)`**
  ```csharp
  protected void AddAvoidObject&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;Vector3&gt; leashPointSelector,
float leashRadius,
Func&lt;T, float&gt; radiusSelector,
Predicate&lt;T&gt; objectSelector,
Func&lt;T, Vector3&gt; locationSelector = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
where T : WoWObject
  ```
  Runs away from a unit or object if within range.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTED76F3EF_13?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTED76F3EF_14?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTED76F3EF_15?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition to run.
  - *leashPointSelector*: Type: SystemAddLanguageSpecificTextSet("LSTED76F3EF_16?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTED76F3EF_17?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTED76F3EF_18?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The leash center point selector. Unit location if null
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LSTED76F3EF_19?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe max distance to run from leash.
  - *radiusSelector*: Type: SystemAddLanguageSpecificTextSet("LSTED76F3EF_20?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTED76F3EF_21?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LSTED76F3EF_22?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The distance to run
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LSTED76F3EF_23?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LSTED76F3EF_24?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTED76F3EF_25?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The object selector.

- **`AddAvoidObject Method (Func(Boolean), Func(Vector3), Single, Single, Predicate(WoWObject), Func(WoWObject, Vector3), Boolean, AvoidancePriority)`**
  ```csharp
  protected void AddAvoidObject(
Func&lt;bool&gt; canRun,
Func&lt;Vector3&gt; leashPointSelector,
float leashRadius,
float radius,
Predicate&lt;WoWObject&gt; objectSelector,
Func&lt;WoWObject, Vector3&gt; locationSelector = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
  ```
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST23FD68BD_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST23FD68BD_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST23FD68BD_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *leashPointSelector*: Type: SystemAddLanguageSpecificTextSet("LST23FD68BD_12?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST23FD68BD_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST23FD68BD_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LST23FD68BD_15?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST23FD68BD_16?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LST23FD68BD_17?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LST23FD68BD_18?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST23FD68BD_19?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`AddAvoidObject(T) Method (Func(Boolean), Func(Vector3), Single, Single, Predicate(T), Func(T, Vector3), Boolean, AvoidancePriority)`**
  ```csharp
  protected void AddAvoidObject&lt;T&gt;(
Func&lt;bool&gt; canRun,
Func&lt;Vector3&gt; leashPointSelector,
float leashRadius,
float radius,
Predicate&lt;T&gt; objectSelector,
Func&lt;T, Vector3&gt; locationSelector = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
where T : WoWObject
  ```
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST210A5772_11?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST210A5772_12?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST210A5772_13?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *leashPointSelector*: Type: SystemAddLanguageSpecificTextSet("LST210A5772_14?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST210A5772_15?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST210A5772_16?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LST210A5772_17?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST210A5772_18?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LST210A5772_19?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LST210A5772_20?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST210A5772_21?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`AddAvoidPolygon(T) Method`**
  ```csharp
  public void AddAvoidPolygon&lt;T&gt;(
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
  - *condition*: Type: SystemAddLanguageSpecificTextSet("LSTED906EC_5?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTED906EC_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTED906EC_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - *leashPointProducer*: Type: SystemAddLanguageSpecificTextSet("LSTED906EC_8?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTED906EC_9?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTED906EC_10?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The leash point producer. Can be null. Used in conjunction with [!:leashRadius]
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LSTED906EC_11?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
                The leash radius. 
                If [!:leashPointProducer] is not null, 
                bot will not navigate furthar than this distance from point returned from [!:leashPointProducer] while running out of avoid
  - *rotationProducer*: Type: SystemAddLanguageSpecificTextSet("LSTED906EC_12?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTED906EC_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LSTED906EC_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Produces the polygon's rotation.
  - *scaleProducer*: Type: SystemAddLanguageSpecificTextSet("LSTED906EC_15?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTED906EC_16?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LSTED906EC_17?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Produces the polygon's scale
  - *heightProducer*: Type: SystemAddLanguageSpecificTextSet("LSTED906EC_18?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTED906EC_19?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, SingleAddLanguageSpecificTextSet("LSTED906EC_20?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
                Produces the height of the polygon. 
                Half of the produced height extends above location produced by [!:locationProducer] and the other half below
  - *pointsProducer*: Type: SystemAddLanguageSpecificTextSet("LSTED906EC_21?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTED906EC_22?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, AddLanguageSpecificTextSet("LSTED906EC_23?cpp=array&lt;");Vector2AddLanguageSpecificTextSet("LSTED906EC_24?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LSTED906EC_25?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Produces the points that form the polygon
  - *locationProducer*: Type: SystemAddLanguageSpecificTextSet("LSTED906EC_26?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTED906EC_27?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, Vector3AddLanguageSpecificTextSet("LSTED906EC_28?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
                Produces the location. 
                Polygon is rotated and scaled around produced location on the xy plane.
  - *collectionProducer*: Type: SystemAddLanguageSpecificTextSet("LSTED906EC_29?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTED906EC_30?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");IEnumerableAddLanguageSpecificTextSet("LSTED906EC_31?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTED906EC_32?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");AddLanguageSpecificTextSet("LSTED906EC_33?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Produces a collection of objects that should be avoided.

- **`AddAvoidUnitCone Method`**
  Avoids cone-shaped area effects on a WoWUnit

- **`AddAvoidUnitCone Method (Single, Single, Single, Predicate(WoWUnit), Func(WoWUnit, Vector3), Boolean, AvoidancePriority)`**
  ```csharp
  public void AddAvoidUnitCone(
float rotationDegrees,
float radius,
float arcDegrees,
Predicate&lt;WoWUnit&gt; objectSelector,
Func&lt;WoWUnit, Vector3&gt; locationProducer = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
  ```
  Avoids cone-shaped area effects on a WoWUnit
  - *rotationDegrees*: Type: SystemAddLanguageSpecificTextSet("LSTEBD8BB02_5?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleRotation in degrees that's relative to the WoWUnit's rotaton
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LSTEBD8BB02_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Singlethe radius of the cone
  - *arcDegrees*: Type: SystemAddLanguageSpecificTextSet("LSTEBD8BB02_7?cs=.|vb=.|cpp=::|nu=.|fs=.");Singlethe arc of the cone in degrees
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LSTEBD8BB02_8?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LSTEBD8BB02_9?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWUnitAddLanguageSpecificTextSet("LSTEBD8BB02_10?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Selects the WoWUnit

- **`AddAvoidUnitCone Method (Func(Boolean), Single, Single, Single, Predicate(WoWUnit), Func(WoWUnit, Vector3), Boolean, AvoidancePriority)`**
  ```csharp
  public void AddAvoidUnitCone(
Func&lt;bool&gt; canRun,
float rotationDegrees,
float radius,
float arcDegrees,
Predicate&lt;WoWUnit&gt; objectSelector,
Func&lt;WoWUnit, Vector3&gt; locationProducer = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
  ```
  Avoids cone-shaped area effects on a WoWUnit
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LSTE6BC598C_7?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTE6BC598C_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTE6BC598C_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *rotationDegrees*: Type: SystemAddLanguageSpecificTextSet("LSTE6BC598C_10?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleRotation in degrees that's relative to the WoWUnit's rotaton
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LSTE6BC598C_11?cs=.|vb=.|cpp=::|nu=.|fs=.");Singlethe radius of the cone
  - *arcDegrees*: Type: SystemAddLanguageSpecificTextSet("LSTE6BC598C_12?cs=.|vb=.|cpp=::|nu=.|fs=.");Singlethe arc of the cone in degrees
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LSTE6BC598C_13?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LSTE6BC598C_14?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWUnitAddLanguageSpecificTextSet("LSTE6BC598C_15?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Selects the WoWUnit

- **`AddAvoidUnitCone Method (Func(Boolean), Func(Vector3), Single, Single, Single, Single, Predicate(WoWUnit), Func(WoWUnit, Vector3), Boolean, AvoidancePriority)`**
  ```csharp
  public void AddAvoidUnitCone(
Func&lt;bool&gt; canRun,
Func&lt;Vector3&gt; leashPointProducer,
float leashRadius,
float rotationDegrees,
float radius,
float arcDegrees,
Predicate&lt;WoWUnit&gt; objectSelector,
Func&lt;WoWUnit, Vector3&gt; locationProducer = null,
bool ignoreIfBlocking = false,
AvoidancePriority priority = AvoidancePriority.Medium
)
  ```
  Avoids cone-shaped area effects on a WoWUnit
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST32ED3479_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST32ED3479_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST32ED3479_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *leashPointProducer*: Type: SystemAddLanguageSpecificTextSet("LST32ED3479_12?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST32ED3479_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST32ED3479_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *leashRadius*: Type: SystemAddLanguageSpecificTextSet("LST32ED3479_15?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *rotationDegrees*: Type: SystemAddLanguageSpecificTextSet("LST32ED3479_16?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleRotation in degrees that's relative to the WoWUnit's rotaton
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST32ED3479_17?cs=.|vb=.|cpp=::|nu=.|fs=.");Singlethe radius of the cone
  - *arcDegrees*: Type: SystemAddLanguageSpecificTextSet("LST32ED3479_18?cs=.|vb=.|cpp=::|nu=.|fs=.");Singlethe arc of the cone in degrees
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LST32ED3479_19?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LST32ED3479_20?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWUnitAddLanguageSpecificTextSet("LST32ED3479_21?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");Selects the WoWUnit

- **`Debug Method`**
  ```csharp
  protected void Debug(
string msg,
string caller = ""
)
  ```
  - *msg*: Type: SystemAddLanguageSpecificTextSet("LST118FED71_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`HandleMovement Method`**
  ```csharp
  public virtual Task&lt;bool&gt; HandleMovement(
MoveToParameters moveToParameters
)
  ```
  Allows custom dungeon navigation such as handling elevators, using portals etc. Return false
                if not controlling movement in override.
  - *moveToParameters*: Type: Styx.PathingAddLanguageSpecificTextSet("LSTA600F0C6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");MoveToParametersThe movement parameters.
  - **Returns:** See Also

- **`IncludeHealTargetsFilter Method`**
  ```csharp
  public virtual void IncludeHealTargetsFilter(
List&lt;WoWObject&gt; incomingObjects,
HashSet&lt;WoWObject&gt; outgoingObjects
)
  ```
  Dungeon specific heal target inclusions.
  - *incomingObjects*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST640E841C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST640E841C_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST640E841C_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The incomingunits.
  - *outgoingObjects*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST640E841C_4?cs=.|vb=.|cpp=::|nu=.|fs=.");HashSetAddLanguageSpecificTextSet("LST640E841C_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST640E841C_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The outgoingunits.

- **`IncludeLootTargetsFilter Method`**
  ```csharp
  public virtual void IncludeLootTargetsFilter(
List&lt;WoWObject&gt; incomingObjects,
HashSet&lt;WoWObject&gt; outgoingObjects
)
  ```
  Dungeon specific loot target inclusions.
  - *incomingObjects*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST71D6A380_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST71D6A380_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST71D6A380_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The incoming loots.
  - *outgoingObjects*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST71D6A380_4?cs=.|vb=.|cpp=::|nu=.|fs=.");HashSetAddLanguageSpecificTextSet("LST71D6A380_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST71D6A380_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The outgoing loots.

- **`IncludeTargetsFilter Method`**
  ```csharp
  public virtual void IncludeTargetsFilter(
List&lt;WoWObject&gt; incomingObjects,
HashSet&lt;WoWObject&gt; outgoingObjects
)
  ```
  Dungeon specific unit target inclusions.
  - *incomingObjects*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTA966942_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LSTA966942_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LSTA966942_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The incoming units.
  - *outgoingObjects*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTA966942_4?cs=.|vb=.|cpp=::|nu=.|fs=.");HashSetAddLanguageSpecificTextSet("LSTA966942_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LSTA966942_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The outgoing units.

- **`Log Method`**
  ```csharp
  protected void Log(
string msg,
string caller = ""
)
  ```
  - *msg*: Type: SystemAddLanguageSpecificTextSet("LSTDD870594_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`OnEnter Method`**
  ```csharp
  public virtual void OnEnter()
  ```
  Called when entering a dungeon.

- **`OnExit Method`**
  ```csharp
  public virtual void OnExit()
  ```
  Called when exiting a dungeon.

- **`RemoveHealTargetsFilter Method`**
  ```csharp
  public virtual void RemoveHealTargetsFilter(
List&lt;WoWObject&gt; objects
)
  ```
  Dungeon specific heal target removal.
  - *objects*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTEA256813_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LSTEA256813_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LSTEA256813_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The incomingunits.

- **`RemoveLootTargetsFilter Method`**
  ```csharp
  public virtual void RemoveLootTargetsFilter(
List&lt;WoWObject&gt; objects
)
  ```
  Dungeon specific loot target removal.
  - *objects*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST37C15AE3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST37C15AE3_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST37C15AE3_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The loots.

- **`RemoveTargetsFilter Method`**
  ```csharp
  public virtual void RemoveTargetsFilter(
List&lt;WoWObject&gt; objects
)
  ```
  Dungeon specific unit target removal.
  - *objects*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTFF1F1747_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LSTFF1F1747_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LSTFF1F1747_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The incomingunits.

- **`WeighHealTargetsFilter Method`**
  ```csharp
  public virtual void WeighHealTargetsFilter(
List&lt;Targeting.TargetPriority&gt; objPriorities
)
  ```
  Dungeon specific heal weighting.
  - *objPriorities*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST31EB571B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST31EB571B_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TargetingAddLanguageSpecificTextSet("LST31EB571B_4?cs=.|vb=.|cpp=::|nu=.|fs=.");TargetPriorityAddLanguageSpecificTextSet("LST31EB571B_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The units.

- **`WeighLootTargetsFilter Method`**
  ```csharp
  public virtual void WeighLootTargetsFilter(
List&lt;Targeting.TargetPriority&gt; objPriorities
)
  ```
  Dungeon specific loot weighting.
  - *objPriorities*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST2CC7509B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST2CC7509B_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TargetingAddLanguageSpecificTextSet("LST2CC7509B_4?cs=.|vb=.|cpp=::|nu=.|fs=.");TargetPriorityAddLanguageSpecificTextSet("LST2CC7509B_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`WeighTargetsFilter Method`**
  ```csharp
  public virtual void WeighTargetsFilter(
List&lt;Targeting.TargetPriority&gt; objPriorities
)
  ```
  Dungeon specific unit weighting.
  - *objPriorities*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST4AC31F37_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST4AC31F37_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TargetingAddLanguageSpecificTextSet("LST4AC31F37_4?cs=.|vb=.|cpp=::|nu=.|fs=.");TargetPriorityAddLanguageSpecificTextSet("LST4AC31F37_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The object priorities.

---

### DungeonBot Class

**Inheritance:** System.Object → Styx.CommonBot.BotBase → Bots.DungeonBuddy.DungeonBot

```csharp
public class DungeonBot : BotBase
```

Initializes a new instance of the DungeonBot class

#### DungeonBot Properties

- **`ConfigurationForm Property`**
  ```csharp
  public override Form ConfigurationForm { get; }
  ```

- **`DungeonCompleted Property`**
  ```csharp
  public static CompleteReason DungeonCompleted { get; }
  ```

- **`IsPrimaryType Property`**
  ```csharp
  public override bool IsPrimaryType { get; }
  ```

- **`Name Property`**
  ```csharp
  public override string Name { get; }
  ```

- **`PulseFlags Property`**
  ```csharp
  public override PulseFlags PulseFlags { get; }
  ```

- **`RequirementsMet Property`**
  ```csharp
  public override bool RequirementsMet { get; }
  ```

- **`Root Property`**
  ```csharp
  public override Composite Root { get; }
  ```

#### DungeonBot Methods

- **`Initialize Method`**
  ```csharp
  public override void Initialize()
  ```

- **`OnDeselected Method`**
  ```csharp
  public override void OnDeselected()
  ```

- **`OnSelected Method`**
  ```csharp
  public override void OnSelected()
  ```

- **`Pulse Method`**
  ```csharp
  public override void Pulse()
  ```

- **`Start Method`**
  ```csharp
  public override void Start()
  ```

- **`Stop Method`**
  ```csharp
  public override void Stop()
  ```

---

### DungeonManager Class

**Inheritance:** System.Object → Bots.DungeonBuddy.DungeonManager

```csharp
public static class DungeonManager
```

Gets the dungeon that player's corpse is inside.

#### DungeonManager Properties

- **`CorpseDungeon Property`**
  ```csharp
  public static Dungeon CorpseDungeon { get; }
  ```
  Gets the dungeon that player's corpse is inside.

- **`CurrentDungeon Property`**
  ```csharp
  public static Dungeon CurrentDungeon { get; }
  ```
  Gets the dungeon that player is standing in

- **`CustomDungeons Property`**
  ```csharp
  public static ReadOnlyCollection&lt;Dungeon&gt; CustomDungeons { get; }
  ```

- **`DefaultDungeons Property`**
  ```csharp
  public static ReadOnlyCollection&lt;Dungeon&gt; DefaultDungeons { get; }
  ```

- **`Initialized Property`**
  ```csharp
  public static bool Initialized { get; }
  ```
  Gets a value indicating whether this DungeonManager is initialized.

- **`SelectedDungeon Property`**
  ```csharp
  public static Dungeon SelectedDungeon { get; set; }
  ```
  The selected dungeon. This is the dungeon that was queued for or selected to farm

#### DungeonManager Methods

- **`HasScriptForDungeon Method`**
  ```csharp
  public static bool HasScriptForDungeon(
uint lfgId
)
  ```
  - *lfgId*: Type: SystemAddLanguageSpecificTextSet("LSTF60BE3C5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceDungeonManager Class

- **`Initialize Method`**
  ```csharp
  public static void Initialize()
  ```

#### DungeonManager Events

- **`OnInitialized Event`**
  ```csharp
  public static event EventHandler&lt;EventArgs&gt; OnInitialized
  ```

---

### GroupMember Class

**Inheritance:** System.Object → Bots.DungeonBuddy.GroupMember

```csharp
public class GroupMember : IEquatable&lt;GroupMember&gt;
```

Determines whether the specified party member is dps.

#### GroupMember Properties

- **`CurrentHealth Property`**
  ```csharp
  public float CurrentHealth { get; }
  ```

- **`CurrentMap Property`**
  ```csharp
  public Map CurrentMap { get; }
  ```

- **`CurrentPower Property`**
  ```csharp
  public uint CurrentPower { get; }
  ```

- **`Distance Property`**
  ```csharp
  public float Distance { get; }
  ```

- **`GroupMembers Property`**
  ```csharp
  public static List&lt;GroupMember&gt; GroupMembers { get; }
  ```

- **`Guid Property`**
  ```csharp
  public WoWGuid Guid { get; }
  ```

- **`HealthPercent Property`**
  ```csharp
  public double HealthPercent { get; }
  ```

- **`InternalInfo Property`**
  ```csharp
  public WoWPartyMember InternalInfo { get; }
  ```

- **`IsAlive Property`**
  ```csharp
  public bool IsAlive { get; }
  ```

- **`IsDead Property`**
  ```csharp
  public bool IsDead { get; }
  ```

- **`IsDps Property`**
  ```csharp
  public bool IsDps { get; }
  ```
  Determines whether the specified party member is dps.

- **`IsGhost Property`**
  ```csharp
  public bool IsGhost { get; }
  ```

- **`IsHealer Property`**
  ```csharp
  public bool IsHealer { get; }
  ```
  Determines whether the specified party member is healer.

- **`IsMelee Property`**
  ```csharp
  public bool IsMelee { get; }
  ```
  Determines whether the specified party member is melee.

- **`IsOnline Property`**
  ```csharp
  public bool IsOnline { get; }
  ```

- **`IsRange Property`**
  ```csharp
  public bool IsRange { get; }
  ```
  Determines whether the specified party member is range.

- **`IsTank Property`**
  ```csharp
  public bool IsTank { get; }
  ```
  Determines whether the specified party member is tank.

- **`Level Property`**
  ```csharp
  public int Level { get; }
  ```

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; }
  ```

- **`MapId Property`**
  ```csharp
  public uint MapId { get; }
  ```

- **`MaxHealth Property`**
  ```csharp
  public float MaxHealth { get; }
  ```

- **`MaxPower Property`**
  ```csharp
  public uint MaxPower { get; }
  ```

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```

- **`Player Property`**
  ```csharp
  public WoWPlayer Player { get; }
  ```

- **`PowerPercent Property`**
  ```csharp
  public float PowerPercent { get; }
  ```

- **`Role Property`**
  ```csharp
  public WoWPartyMember.GroupRole Role { get; }
  ```

- **`Server Property`**
  ```csharp
  public string Server { get; }
  ```

- **`Specialization Property`**
  ```csharp
  public WoWSpec Specialization { get; }
  ```

- **`Unit Property`**
  ```csharp
  public WoWUnit Unit { get; }
  ```

- **`UnitId Property`**
  ```csharp
  public string UnitId { get; }
  ```

#### GroupMember Methods

- **`Equals Method`**

- **`Equals Method (Object)`**
  ```csharp
  public override bool Equals(
Object obj
)
  ```
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LSTA4804716_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceGroupMember Class

- **`Equals Method (GroupMember)`**
  ```csharp
  public bool Equals(
GroupMember other
)
  ```
  - *other*: Type: Bots.DungeonBuddyAddLanguageSpecificTextSet("LSTC72742BB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");GroupMember
  - **Returns:** See Also

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  - **Returns:** ReferenceGroupMember Class

- **`NameEquals Method`**
  ```csharp
  public bool NameEquals(
string fullName
)
  ```
  Checks if name and optionally server matches Name and Server. 
            Supported formats for fullName are 'Name-Server' or just 'Name'. Case doesn't mater.
  - *fullName*: Type: SystemAddLanguageSpecificTextSet("LSTA2139DF1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - **Returns:** ReferenceGroupMember Class

#### GroupMember Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
GroupMember a,
GroupMember b
)
  ```
  - *a*: Type: Bots.DungeonBuddyAddLanguageSpecificTextSet("LST1FB24D23_1?cs=.|vb=.|cpp=::|nu=.|fs=.");GroupMember
  - *b*: Type: Bots.DungeonBuddyAddLanguageSpecificTextSet("LST1FB24D23_2?cs=.|vb=.|cpp=::|nu=.|fs=.");GroupMember
  - **Returns:** ReferenceGroupMember Class

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
GroupMember a,
GroupMember b
)
  ```
  - *a*: Type: Bots.DungeonBuddyAddLanguageSpecificTextSet("LST25084CC2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");GroupMember
  - *b*: Type: Bots.DungeonBuddyAddLanguageSpecificTextSet("LST25084CC2_2?cs=.|vb=.|cpp=::|nu=.|fs=.");GroupMember
  - **Returns:** ReferenceGroupMember Class

---
