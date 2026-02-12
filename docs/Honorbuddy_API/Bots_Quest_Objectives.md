# Bots.Quest.Objectives

Contains quest bot objectives.

## Contents

- [CollectItemObjective Class](#collectitemobjective-class)
- [DropDatabase Class](#dropdatabase-class)
- [GrindObjective Class](#grindobjective-class)
- [QuestObjective Class](#questobjective-class)
- [UseGameObjectObjective Class](#usegameobjectobjective-class)

---

### CollectItemObjective Class

**Inheritance:** System.Object → Bots.Quest.Objectives.QuestObjective → Bots.Quest.Objectives.CollectItemObjective

```csharp
public class CollectItemObjective : QuestObjective
```

This objective is responsible for collecting items from vendors, mobs or game objects.

#### CollectItemObjective Properties

- **`IsCompleted Property`**
  ```csharp
  public override bool IsCompleted { get; }
  ```
  Gets a value indicating whether this object is completed.

- **`Objective Property`**
  ```csharp
  public Quest.QuestObjective Objective { get; }
  ```
  Gets the objective.

- **`QuestObjects Property`**
  ```csharp
  public List&lt;WoWGameObject&gt; QuestObjects { get; }
  ```
  Gets the question objects.

#### CollectItemObjective Methods

- **`CreateBranch Method`**
  ```csharp
  public override Composite CreateBranch()
  ```
  Creates the branch.
  - **Returns:** ReferenceCollectItemObjective Class

- **`CreateRoamBehavior Method`**
  ```csharp
  public PrioritySelector CreateRoamBehavior()
  ```
  Creates roam behavior.
  - **Returns:** ReferenceCollectItemObjective Class

- **`Dispose Method`**
  ```csharp
  public override void Dispose()
  ```
  Performs application-defined tasks associated with freeing, releasing, or resetting
            unmanaged resources.

- **`OnStart Method`**
  ```csharp
  public override void OnStart()
  ```

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceCollectItemObjective Class

---

### DropDatabase Class

**Inheritance:** System.Object → Bots.Quest.Objectives.DropDatabase

```csharp
public static class DropDatabase
```

A drop database.

#### DropDatabase Properties

- **`GameObjects Property`**
  ```csharp
  public static Dictionary&lt;uint, HashSet&lt;uint&gt;&gt; GameObjects { get; }
  ```
  Returns a copy of the internal dictionary used as the database. The key is the game
            object ID, the value is a HashSet of the drops.

- **`Mobs Property`**
  ```csharp
  public static Dictionary&lt;uint, HashSet&lt;uint&gt;&gt; Mobs { get; }
  ```
  Returns a copy of the internal dictionary used as the database. The key is the mob ID,
            the value is a HashSet of the drops.

- **`Vendors Property`**
  ```csharp
  public static Dictionary&lt;uint, HashSet&lt;uint&gt;&gt; Vendors { get; }
  ```
  Returns a copy of the internal dictionary used as the database. The key is the vendor
            ID, the value is a HashSet of the items for sale.

#### DropDatabase Methods

- **`AddGameObjectDrop Method`**
  ```csharp
  public static void AddGameObjectDrop(
uint gameObjId,
uint dropItemId
)
  ```
  Adds a drop from a game object to the database.
  - *gameObjId*: Type: SystemAddLanguageSpecificTextSet("LST6CB533F9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 The ID of the game object.
  - *dropItemId*: Type: SystemAddLanguageSpecificTextSet("LST6CB533F9_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The ID of the item the game object drops.

- **`AddMobDrop Method`**
  ```csharp
  public static void AddMobDrop(
uint mobId,
uint dropItemId
)
  ```
  Adds a drop from a mob to the database.
  - *mobId*: Type: SystemAddLanguageSpecificTextSet("LST7B70F444_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32     The ID of the mob.
  - *dropItemId*: Type: SystemAddLanguageSpecificTextSet("LST7B70F444_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The ID of the item the mob drops.

- **`AddVendorDrop Method`**
  ```csharp
  public static void AddVendorDrop(
uint vendorId,
uint itemId
)
  ```
  Adds a vendor drop from a vendor to the database.
  - *vendorId*: Type: SystemAddLanguageSpecificTextSet("LST71A5E048_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32.
  - *itemId*: Type: SystemAddLanguageSpecificTextSet("LST71A5E048_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32  .

- **`GameObjectDropsItem Method`**
  ```csharp
  public static bool GameObjectDropsItem(
uint gameObjId,
uint itemId
)
  ```
  Determines whether a game object drops an item according to this database.
  - *gameObjId*: Type: SystemAddLanguageSpecificTextSet("LST7A3703A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The ID of the game object.
  - *itemId*: Type: SystemAddLanguageSpecificTextSet("LST7A3703A_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32   The ID of the item.
  - **Returns:** ReferenceDropDatabase Class

- **`UnitDropsItem Method`**
  ```csharp
  public static bool UnitDropsItem(
uint mobId,
uint itemId
)
  ```
  Determines whether a mob drops an item according to this database.
  - *mobId*: Type: SystemAddLanguageSpecificTextSet("LST1B3DBCDF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 The ID of the mob.
  - *itemId*: Type: SystemAddLanguageSpecificTextSet("LST1B3DBCDF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The ID of the item.
  - **Returns:** ReferenceDropDatabase Class

- **`VendorSellsItem Method`**
  ```csharp
  public static bool VendorSellsItem(
uint vendorId,
uint itemId
)
  ```
  Determines whether a vendor got an item for sale according to this database.
  - *vendorId*: Type: SystemAddLanguageSpecificTextSet("LSTF06F6F20_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The id of the vendor.
  - *itemId*: Type: SystemAddLanguageSpecificTextSet("LSTF06F6F20_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32  The ID of the item.
  - **Returns:** ReferenceDropDatabase Class

---

### GrindObjective Class

**Inheritance:** System.Object → Bots.Quest.Objectives.QuestObjective → Bots.Quest.Objectives.GrindObjective

```csharp
public class GrindObjective : QuestObjective
```

A grind objective.

#### GrindObjective Properties

- **`IsCompleted Property`**
  ```csharp
  public override bool IsCompleted { get; }
  ```
  Gets a bool that indicates whether this objective is completed.

- **`Objective Property`**
  ```csharp
  public Quest.QuestObjective Objective { get; }
  ```
  Gets the objective.

#### GrindObjective Methods

- **`CreateBranch Method`**
  ```csharp
  public override Composite CreateBranch()
  ```
  Creates the branch.
  - **Returns:** ReferenceGrindObjective Class

- **`Dispose Method`**
  ```csharp
  public override void Dispose()
  ```
  Performs application-defined tasks associated with freeing, releasing, or resetting
            unmanaged resources.

- **`GetOverrideInfo Method`**
  ```csharp
  public KillMobObjectiveInfo GetOverrideInfo()
  ```
  Gets override information.
  - **Returns:** ReferenceGrindObjective Class

- **`OnStart Method`**
  ```csharp
  public override void OnStart()
  ```

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceGrindObjective Class

---

### QuestObjective Class

**Inheritance:** System.Object → Bots.Quest.Objectives.QuestObjective → Bots.Quest.Objectives.CollectItemObjective → Bots.Quest.Objectives.GrindObjective → Bots.Quest.Objectives.UseGameObjectObjective

```csharp
public abstract class QuestObjective : IDisposable,
IEquatable&lt;QuestObjective&gt;
```

A question objective.

#### QuestObjective Properties

- **`DonePrerequisites Property`**
  ```csharp
  public bool DonePrerequisites { get; }
  ```
  Gets a value indicating whether the done prerequisites.

- **`ForcedHotspot Property`**
  ```csharp
  public Vector3 ForcedHotspot { get; }
  ```
  Gets the forced hotspot.

- **`IsCompleted Property`**
  ```csharp
  public abstract bool IsCompleted { get; }
  ```
  Gets a value indicating whether this object is completed.

- **`NavType Property`**
  ```csharp
  public Nullable&lt;NavType&gt; NavType { get; }
  ```
  Gets the type of the navigaiton.

- **`OverridedQuestInfo Property`**
  ```csharp
  public QuestInfo OverridedQuestInfo { get; }
  ```
  Gets information describing the overrided question.

- **`Quest Property`**
  ```csharp
  public PlayerQuest Quest { get; }
  ```
  Gets the question.

- **`QuestArea Property`**
  ```csharp
  public QuestArea QuestArea { get; }
  ```
  Gets the question area.

- **`QuestSteps Property`**
  ```csharp
  public List&lt;WoWQuestStep&gt; QuestSteps { get; }
  ```
  Gets the question steps.

#### QuestObjective Methods

- **`CreateBranch Method`**
  ```csharp
  public abstract Composite CreateBranch()
  ```
  Creates the branch.
  - **Returns:** ReferenceQuestObjective Class

- **`Dispose Method`**
  ```csharp
  public virtual void Dispose()
  ```
  Performs application-defined tasks associated with freeing, releasing, or resetting
            unmanaged resources.

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
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LST76A64C6E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe Object to compare with the current
            Object.
  - **Returns:** ReferenceQuestObjective Class

- **`Equals Method (QuestObjective)`**
  ```csharp
  public bool Equals(
QuestObjective other
)
  ```
  Tests if this QuestObjective is considered equal to another.
  - *other*: Type: Bots.Quest.ObjectivesAddLanguageSpecificTextSet("LSTB9B60923_1?cs=.|vb=.|cpp=::|nu=.|fs=.");QuestObjectiveThe question objective to compare to this object.
  - **Returns:** See Also

- **`GetClosestQuestStep Method`**
  ```csharp
  protected WoWQuestStep GetClosestQuestStep()
  ```
  - **Returns:** ReferenceQuestObjective Class

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  Serves as a hash function for a particular type.
  - **Returns:** ReferenceQuestObjective Class

- **`OnStart Method`**
  ```csharp
  public virtual void OnStart()
  ```

#### QuestObjective Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
QuestObjective left,
QuestObjective right
)
  ```
  Equality operator.
  - *left*: Type: Bots.Quest.ObjectivesAddLanguageSpecificTextSet("LSTDD94C78B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");QuestObjective The left.
  - *right*: Type: Bots.Quest.ObjectivesAddLanguageSpecificTextSet("LSTDD94C78B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");QuestObjectiveThe right.
  - **Returns:** ReferenceQuestObjective Class

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
QuestObjective left,
QuestObjective right
)
  ```
  Inequality operator.
  - *left*: Type: Bots.Quest.ObjectivesAddLanguageSpecificTextSet("LST28F06808_1?cs=.|vb=.|cpp=::|nu=.|fs=.");QuestObjective The left.
  - *right*: Type: Bots.Quest.ObjectivesAddLanguageSpecificTextSet("LST28F06808_2?cs=.|vb=.|cpp=::|nu=.|fs=.");QuestObjectiveThe right.
  - **Returns:** ReferenceQuestObjective Class

#### QuestObjective Fields

- **`Prerequisites Field`**
  ```csharp
  public readonly List&lt;QuestObjective&gt; Prerequisites
  ```
  The prerequisites.

---

### UseGameObjectObjective Class

**Inheritance:** System.Object → Bots.Quest.Objectives.QuestObjective → Bots.Quest.Objectives.UseGameObjectObjective

```csharp
public class UseGameObjectObjective : QuestObjective
```

An use game object objective.

#### UseGameObjectObjective Properties

- **`IsCompleted Property`**
  ```csharp
  public override bool IsCompleted { get; }
  ```
  Gets a value indicating whether this object is completed.

- **`Objective Property`**
  ```csharp
  public Quest.QuestObjective Objective { get; }
  ```
  Gets the objective.

#### UseGameObjectObjective Methods

- **`CreateBranch Method`**
  ```csharp
  public override Composite CreateBranch()
  ```
  Creates the branch.
  - **Returns:** ReferenceUseGameObjectObjective Class

- **`Dispose Method`**
  ```csharp
  public override void Dispose()
  ```
  Performs application-defined tasks associated with freeing, releasing, or resetting
            unmanaged resources.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceUseGameObjectObjective Class

---
