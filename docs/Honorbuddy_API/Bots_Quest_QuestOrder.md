# Bots.Quest.QuestOrder

Contains quest bot quest order nodes.

## Contents

- [ForcedBehavior Class](#forcedbehavior-class)
- [ForcedCodeBehavior Class](#forcedcodebehavior-class)
- [ForcedGrindTo Class](#forcedgrindto-class)
- [ForcedIf Class](#forcedif-class)
- [ForcedMoveTo Class](#forcedmoveto-class)
- [ForcedNothing Class](#forcednothing-class)
- [ForcedQuestObjective Class](#forcedquestobjective-class)
- [ForcedQuestPickUp Class](#forcedquestpickup-class)
- [ForcedQuestTurnIn Class](#forcedquestturnin-class)
- [ForcedSingleton Class](#forcedsingleton-class)
- [ForcedUseItem Class](#forceduseitem-class)
- [ForcedWhile Class](#forcedwhile-class)
- [QuestOrder Class](#questorder-class)

---

### ForcedBehavior Class

**Inheritance:** System.Object → Bots.Quest.QuestOrder.ForcedBehavior → More.

```csharp
public abstract class ForcedBehavior : IDisposable
```

A forced behavior.

#### ForcedBehavior Properties

- **`Branch Property`**
  ```csharp
  public Composite Branch { get; }
  ```
  Gets the branch.

- **`IsDone Property`**
  ```csharp
  public abstract bool IsDone { get; }
  ```
  Gets a value indicating whether this object is done.

- **`LineNumber Property`**
  ```csharp
  public int LineNumber { get; }
  ```
  Gets the line number.

- **`LinePosition Property`**
  ```csharp
  public int LinePosition { get; }
  ```
  Gets the line position.

- **`NavType Property`**
  ```csharp
  public virtual Nullable&lt;NavType&gt; NavType { get; }
  ```
  Gets or sets the type of the navigation.

- **`Node Property`**
  ```csharp
  public OrderNode Node { get; }
  ```
  Gets the node.

#### ForcedBehavior Methods

- **`CreateBehavior Method`**
  ```csharp
  protected abstract Composite CreateBehavior()
  ```
  - **Returns:** ReferenceForcedBehavior Class

- **`Dispose Method`**
  ```csharp
  [ObsoleteAttribute("Use OnFinished instead.")]
public virtual void Dispose()
  ```
  Called right before this behavior is removed as the current behavior, or when the bot
            is stopped.

- **`OnFinished Method`**
  ```csharp
  public virtual void OnFinished()
  ```
  Called when the behavior is finished, that is when moving to the next behavior. Also
            called when the bot stops.

- **`OnStart Method`**
  ```csharp
  public virtual void OnStart()
  ```
  Called right after this behavior has been set as the current behavior.

- **`OnTick Method`**
  ```csharp
  public virtual void OnTick()
  ```
  Called when the behavior tree is ticked.

---

### ForcedCodeBehavior Class

**Inheritance:** System.Object → Bots.Quest.QuestOrder.ForcedBehavior → Bots.Quest.QuestOrder.ForcedCodeBehavior

```csharp
public class ForcedCodeBehavior : ForcedBehavior
```

A forced code behavior.

#### ForcedCodeBehavior Properties

- **`IsDone Property`**
  ```csharp
  public override bool IsDone { get; }
  ```
  Gets a value indicating whether this object is done.

- **`NavType Property`**
  ```csharp
  public override Nullable&lt;NavType&gt; NavType { get; }
  ```

#### ForcedCodeBehavior Methods

- **`CreateBehavior Method`**
  ```csharp
  protected override Composite CreateBehavior()
  ```
  - **Returns:** ReferenceForcedCodeBehavior Class

- **`Dispose Method`**
  ```csharp
  [ObsoleteAttribute("Use OnFinished instead.")]
public override void Dispose()
  ```
  Called right before this behavior is removed as the current behavior, or when the bot
            is stopped.

- **`OnFinished Method`**
  ```csharp
  public override void OnFinished()
  ```
  Called when the behavior is finished, that is when moving to the next behavior. Also
            called when the bot stops.

- **`OnStart Method`**
  ```csharp
  public override void OnStart()
  ```
  Called right after this behavior has been set as the current behavior.

- **`OnTick Method`**
  ```csharp
  public override void OnTick()
  ```
  Called when the behavior tree is ticked.

---

### ForcedGrindTo Class

**Inheritance:** System.Object → Bots.Quest.QuestOrder.ForcedBehavior → Bots.Quest.QuestOrder.ForcedGrindTo

```csharp
public class ForcedGrindTo : ForcedBehavior
```

A forced grind to.

#### ForcedGrindTo Properties

- **`IsDone Property`**
  ```csharp
  public override bool IsDone { get; }
  ```
  Gets a value indicating whether this object is done.

- **`NavType Property`**
  ```csharp
  public override Nullable&lt;NavType&gt; NavType { get; }
  ```

- **`Node Property`**
  ```csharp
  public GrindToNode Node { get; }
  ```
  Gets the node.

#### ForcedGrindTo Methods

- **`CreateBehavior Method`**
  ```csharp
  protected override Composite CreateBehavior()
  ```
  - **Returns:** ReferenceForcedGrindTo Class

- **`OnFinished Method`**
  ```csharp
  public override void OnFinished()
  ```
  Called right before this behavior is removed as the current behavior, or when the bot
            is stopped.

- **`OnStart Method`**
  ```csharp
  public override void OnStart()
  ```
  Called right after this behavior has been set as the current behavior.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceForcedGrindTo Class

---

### ForcedIf Class

**Inheritance:** System.Object → Bots.Quest.QuestOrder.ForcedBehavior → Bots.Quest.QuestOrder.ForcedIf

```csharp
public class ForcedIf : ForcedBehavior
```

A forced if.

#### ForcedIf Properties

- **`IfNode Property`**
  ```csharp
  public IfNode IfNode { get; }
  ```
  Gets if node.

- **`IsDone Property`**
  ```csharp
  public override bool IsDone { get; }
  ```
  Gets a value indicating whether this object is done.

- **`NavType Property`**
  ```csharp
  public override Nullable&lt;NavType&gt; NavType { get; }
  ```

#### ForcedIf Methods

- **`CreateBehavior Method`**
  ```csharp
  protected override Composite CreateBehavior()
  ```
  - **Returns:** ReferenceForcedIf Class

- **`Dispose Method`**
  ```csharp
  [ObsoleteAttribute("Use OnFinished instead.")]
public override void Dispose()
  ```
  Releases all resources used by the ForcedIf

- **`OnFinished Method`**
  ```csharp
  public override void OnFinished()
  ```

- **`OnStart Method`**
  ```csharp
  public override void OnStart()
  ```
  Called right after this behavior has been set as the current behavior.

---

### ForcedMoveTo Class

**Inheritance:** System.Object → Bots.Quest.QuestOrder.ForcedBehavior → Bots.Quest.QuestOrder.ForcedMoveTo

```csharp
public sealed class ForcedMoveTo : ForcedBehavior
```

A forced move to.

#### ForcedMoveTo Properties

- **`Harvest Property`**
  ```csharp
  public bool Harvest { get; }
  ```

- **`IsDone Property`**
  ```csharp
  public override bool IsDone { get; }
  ```
  Gets a value indicating whether this object is done.

- **`Land Property`**
  ```csharp
  public bool Land { get; }
  ```

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; }
  ```
  Gets the location.

- **`LocationName Property`**
  ```csharp
  public string LocationName { get; }
  ```
  Gets the name of the location.

- **`NavType Property`**
  ```csharp
  public override Nullable&lt;NavType&gt; NavType { get; }
  ```

- **`Precision Property`**
  ```csharp
  public float Precision { get; }
  ```
  Gets the precision.

- **`QuestId Property`**
  ```csharp
  [ObsoleteAttribute("Use VariantQuestIds instead")]
public uint QuestId { get; }
  ```
  Gets the identifier of the question.

- **`VariantQuestIds Property`**
  ```csharp
  public IReadOnlyCollection&lt;uint&gt; VariantQuestIds { get; }
  ```
  An array of the variant quest IDs. 
            These quests have the same objectives but availability depends on class/race.

#### ForcedMoveTo Methods

- **`OnFinished Method`**
  ```csharp
  public override void OnFinished()
  ```

- **`OnStart Method`**
  ```csharp
  public override void OnStart()
  ```
  Called right after this behavior has been set as the current behavior.

---

### ForcedNothing Class

**Inheritance:** System.Object → Bots.Quest.QuestOrder.ForcedBehavior → Bots.Quest.QuestOrder.ForcedNothing

```csharp
public class ForcedNothing : ForcedBehavior
```

A forced nothing.

#### ForcedNothing Constructor

- **`ForcedNothing Constructor`**
  ```csharp
  [ObsoleteAttribute("Do not use. We will be removing this constructor in the future.")]
public ForcedNothing()
  ```
  Initializes a new instance of the ForcedNothing class

- **`ForcedNothing Constructor (OrderNode)`**
  ```csharp
  public ForcedNothing(
OrderNode node
)
  ```
  Initializes a new instance of the ForcedNothing class
  - *node*: Type: Styx.CommonBot.Profiles.Quest.OrderAddLanguageSpecificTextSet("LST3D14BB29_0?cs=.|vb=.|cpp=::|nu=.|fs=.");OrderNode

#### ForcedNothing Properties

- **`IsDone Property`**
  ```csharp
  public override bool IsDone { get; }
  ```
  Gets a value indicating whether this object is done.

#### ForcedNothing Methods

- **`CreateBehavior Method`**
  ```csharp
  protected override Composite CreateBehavior()
  ```
  - **Returns:** ReferenceForcedNothing Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceForcedNothing Class

---

### ForcedQuestObjective Class

**Inheritance:** System.Object → Bots.Quest.QuestOrder.ForcedBehavior → Bots.Quest.QuestOrder.ForcedQuestObjective

```csharp
public class ForcedQuestObjective : ForcedBehavior
```

A forced question objective.

#### ForcedQuestObjective Properties

- **`IsDone Property`**
  ```csharp
  public override bool IsDone { get; }
  ```
  Gets a value indicating whether this object is done.

- **`NavType Property`**
  ```csharp
  public override Nullable&lt;NavType&gt; NavType { get; }
  ```

- **`Objective Property`**
  ```csharp
  public QuestObjective Objective { get; }
  ```
  Gets the objective.

#### ForcedQuestObjective Methods

- **`CreateBehavior Method`**
  ```csharp
  protected override Composite CreateBehavior()
  ```
  - **Returns:** ReferenceForcedQuestObjective Class

- **`OnFinished Method`**
  ```csharp
  public override void OnFinished()
  ```
  Called right before this behavior is removed as the current behavior, or when the bot
            is stopped.

- **`OnStart Method`**
  ```csharp
  public override void OnStart()
  ```
  Called right after this behavior has been set as the current behavior.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceForcedQuestObjective Class

---

### ForcedQuestPickUp Class

**Inheritance:** System.Object → Bots.Quest.QuestOrder.ForcedBehavior → Bots.Quest.QuestOrder.ForcedQuestPickUp

```csharp
public class ForcedQuestPickUp : ForcedBehavior
```

A forced question pick up.

#### ForcedQuestPickUp Properties

- **`GiverId Property`**
  ```csharp
  public uint GiverId { get; }
  ```
  Gets the identifier of the giver.

- **`GiverLocation Property`**
  ```csharp
  public Vector3 GiverLocation { get; }
  ```
  Gets the giver location.

- **`GiverName Property`**
  ```csharp
  public string GiverName { get; }
  ```
  Gets the name of the giver.

- **`GiverType Property`**
  ```csharp
  public Nullable&lt;QuestObjectType&gt; GiverType { get; }
  ```
  Gets the type of the giver.

- **`IsDone Property`**
  ```csharp
  public override bool IsDone { get; }
  ```
  Gets a value indicating whether this object is done.

- **`IsExplicitLocation Property`**
  ```csharp
  public bool IsExplicitLocation { get; }
  ```
  Whether the location was explicitly defined by the user.

- **`NavType Property`**
  ```csharp
  public override Nullable&lt;NavType&gt; NavType { get; }
  ```
  Gets the type of the navigaiton.

- **`QuestId Property`**
  ```csharp
  [ObsoleteAttribute("Use VariantQuestIds instead")]
public uint QuestId { get; }
  ```
  Gets the identifier of the question.

- **`QuestName Property`**
  ```csharp
  public string QuestName { get; }
  ```
  Gets the name of the question.

- **`VariantQuestIds Property`**
  ```csharp
  public IReadOnlyCollection&lt;uint&gt; VariantQuestIds { get; }
  ```
  An array of the variant quest IDs. 
            These quests have the same objectives but availability depends on class/race.

#### ForcedQuestPickUp Methods

- **`CreateBehavior Method`**
  ```csharp
  protected override Composite CreateBehavior()
  ```
  - **Returns:** ReferenceForcedQuestPickUp Class

- **`OnFinished Method`**
  ```csharp
  public override void OnFinished()
  ```
  Called right before this behavior is removed as the current behavior, or when the bot
            is stopped.

- **`OnStart Method`**
  ```csharp
  public override void OnStart()
  ```
  Called right after this behavior has been set as the current behavior.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceForcedQuestPickUp Class

---

### ForcedQuestTurnIn Class

**Inheritance:** System.Object → Bots.Quest.QuestOrder.ForcedBehavior → Bots.Quest.QuestOrder.ForcedQuestTurnIn

```csharp
public sealed class ForcedQuestTurnIn : ForcedBehavior
```

A forced question turn in.

#### ForcedQuestTurnIn Constructor

- **`ForcedQuestTurnIn Constructor (UInt32, String, UInt32, String, Vector3, Nullable(NavType), Boolean)`**
  ```csharp
  [ObsoleteAttribute("Do not use. We will be removing this constructor in the future.")]
public ForcedQuestTurnIn(
uint questId,
string questName,
uint npcId,
string turnInName,
Vector3 location,
Nullable&lt;NavType&gt; navType = null,
bool isExplicitLocation = true
)
  ```
  Initializes a new instance of the ForcedQuestTurnIn class
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LST5FB4F9CB_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *questName*: Type: SystemAddLanguageSpecificTextSet("LST5FB4F9CB_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *npcId*: Type: SystemAddLanguageSpecificTextSet("LST5FB4F9CB_4?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *turnInName*: Type: SystemAddLanguageSpecificTextSet("LST5FB4F9CB_5?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST5FB4F9CB_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3

- **`ForcedQuestTurnIn Constructor (TurnInNode, String, UInt32, String, Vector3, Nullable(NavType), Boolean)`**
  ```csharp
  public ForcedQuestTurnIn(
TurnInNode node,
string questName,
uint npcId,
string turnInName,
Vector3 location,
Nullable&lt;NavType&gt; navType = null,
bool isExplicitLocation = true
)
  ```
  Constructor.
  - *node*: Type: Styx.CommonBot.Profiles.Quest.OrderAddLanguageSpecificTextSet("LST18EDA0C1_2?cs=.|vb=.|cpp=::|nu=.|fs=.");TurnInNodeThe node.
  - *questName*: Type: SystemAddLanguageSpecificTextSet("LST18EDA0C1_3?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name of the question.
  - *npcId*: Type: SystemAddLanguageSpecificTextSet("LST18EDA0C1_4?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier of the npc.
  - *turnInName*: Type: SystemAddLanguageSpecificTextSet("LST18EDA0C1_5?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the turn in.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST18EDA0C1_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.

- **`ForcedQuestTurnIn Constructor (TurnInNode, UInt32, String, UInt32, String, Vector3, Nullable(NavType), Boolean)`**
  ```csharp
  [ObsoleteAttribute("Do not use. We will be removing this constructor in the future.")]
public ForcedQuestTurnIn(
TurnInNode node,
uint questId,
string questName,
uint npcId,
string turnInName,
Vector3 location,
Nullable&lt;NavType&gt; navType = null,
bool isExplicitLocation = true
)
  ```
  Initializes a new instance of the ForcedQuestTurnIn class
  - *node*: Type: Styx.CommonBot.Profiles.Quest.OrderAddLanguageSpecificTextSet("LST1287B389_2?cs=.|vb=.|cpp=::|nu=.|fs=.");TurnInNode
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LST1287B389_3?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *questName*: Type: SystemAddLanguageSpecificTextSet("LST1287B389_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *npcId*: Type: SystemAddLanguageSpecificTextSet("LST1287B389_5?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *turnInName*: Type: SystemAddLanguageSpecificTextSet("LST1287B389_6?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST1287B389_7?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3

#### ForcedQuestTurnIn Properties

- **`IsDone Property`**
  ```csharp
  public override bool IsDone { get; }
  ```
  Gets a value indicating whether this object is done.

- **`IsExplicitLocation Property`**
  ```csharp
  public bool IsExplicitLocation { get; }
  ```
  Whether the location was explicitly defined by the user.

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; }
  ```
  Gets the location.

- **`NavType Property`**
  ```csharp
  public override Nullable&lt;NavType&gt; NavType { get; }
  ```

- **`NpcId Property`**
  ```csharp
  public uint NpcId { get; }
  ```
  Gets the identifier of the npc.

- **`NpcName Property`**
  ```csharp
  public string NpcName { get; }
  ```
  Gets the name of the npc.

- **`QuestId Property`**
  ```csharp
  [ObsoleteAttribute("Use VariantQuestIds instead")]
public uint QuestId { get; }
  ```
  Gets the identifier of the question.

- **`QuestName Property`**
  ```csharp
  public string QuestName { get; }
  ```
  Gets the name of the question.

- **`VariantQuestIds Property`**
  ```csharp
  public IReadOnlyCollection&lt;uint&gt; VariantQuestIds { get; }
  ```
  An array of the variant quest IDs. 
            These quests have the same objectives but availability depends on class/race.

#### ForcedQuestTurnIn Methods

- **`OnFinished Method`**
  ```csharp
  public override void OnFinished()
  ```
  Called right before this behavior is removed as the current behavior, or when the bot
            is stopped.

- **`OnStart Method`**
  ```csharp
  public override void OnStart()
  ```
  Called right after this behavior has been set as the current behavior.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceForcedQuestTurnIn Class

---

### ForcedSingleton Class

**Inheritance:** System.Object → Bots.Quest.QuestOrder.ForcedBehavior → Bots.Quest.QuestOrder.ForcedSingleton

```csharp
public class ForcedSingleton : ForcedBehavior
```

A forced singleton.

#### ForcedSingleton Constructor

- **`ForcedSingleton Constructor (Action)`**
  ```csharp
  [ObsoleteAttribute("Do not use. We will be removing this constructor in the future.")]
public ForcedSingleton(
Action action
)
  ```
  Initializes a new instance of the ForcedSingleton class
  - *action*: Type: SystemAddLanguageSpecificTextSet("LST98AC8E82_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Action

- **`ForcedSingleton Constructor (OrderNode, Action)`**
  ```csharp
  public ForcedSingleton(
OrderNode node,
Action action
)
  ```
  Constructor.
  - *node*: Type: Styx.CommonBot.Profiles.Quest.OrderAddLanguageSpecificTextSet("LST8F8A56AC_0?cs=.|vb=.|cpp=::|nu=.|fs=.");OrderNodeThe node.
  - *action*: Type: SystemAddLanguageSpecificTextSet("LST8F8A56AC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ActionThe action.

#### ForcedSingleton Properties

- **`Action Property`**
  ```csharp
  public Action Action { get; }
  ```
  Gets the action.

- **`IsDone Property`**
  ```csharp
  public override bool IsDone { get; }
  ```
  Gets a value indicating whether this object is done.

#### ForcedSingleton Methods

- **`CreateBehavior Method`**
  ```csharp
  protected override Composite CreateBehavior()
  ```
  - **Returns:** ReferenceForcedSingleton Class

- **`OnStart Method`**
  ```csharp
  public override void OnStart()
  ```
  Called right after this behavior has been set as the current behavior.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceForcedSingleton Class

---

### ForcedUseItem Class

**Inheritance:** System.Object → Bots.Quest.QuestOrder.ForcedBehavior → Bots.Quest.QuestOrder.ForcedUseItem

```csharp
public sealed class ForcedUseItem : ForcedBehavior
```

A forced use item.

#### ForcedUseItem Properties

- **`ForceUse Property`**
  ```csharp
  public bool ForceUse { get; }
  ```
  Gets a value indicating whether the use should be forced.

- **`IsDone Property`**
  ```csharp
  public override bool IsDone { get; }
  ```
  Gets a value indicating whether this object is done.

- **`ItemRetriever Property`**
  ```csharp
  public Func&lt;WoWItem&gt; ItemRetriever { get; }
  ```
  Gets the item retriever.

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; }
  ```
  Gets the location.

- **`NavType Property`**
  ```csharp
  public override Nullable&lt;NavType&gt; NavType { get; }
  ```

- **`QuestId Property`**
  ```csharp
  [ObsoleteAttribute("Use VariantQuestIds instead")]
public uint QuestId { get; }
  ```
  Gets the identifier of the question.

- **`TargetRetriever Property`**
  ```csharp
  public Func&lt;WoWObject&gt; TargetRetriever { get; }
  ```
  Gets target retriever.

- **`VariantQuestIds Property`**
  ```csharp
  public IReadOnlyCollection&lt;uint&gt; VariantQuestIds { get; }
  ```
  An array of the variant quest IDs. 
            These quests have the same objectives but availability depends on class/race.

#### ForcedUseItem Methods

- **`OnStart Method`**
  ```csharp
  public override void OnStart()
  ```
  Called right after this behavior has been set as the current behavior.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceForcedUseItem Class

---

### ForcedWhile Class

**Inheritance:** System.Object → Bots.Quest.QuestOrder.ForcedBehavior → Bots.Quest.QuestOrder.ForcedWhile

```csharp
public class ForcedWhile : ForcedBehavior
```

A forced while.

#### ForcedWhile Properties

- **`IsDone Property`**
  ```csharp
  public override bool IsDone { get; }
  ```
  Gets a value indicating whether this object is done.

- **`NavType Property`**
  ```csharp
  public override Nullable&lt;NavType&gt; NavType { get; }
  ```

- **`WhileNode Property`**
  ```csharp
  public WhileNode WhileNode { get; }
  ```
  Gets the while node.

#### ForcedWhile Methods

- **`CreateBehavior Method`**
  ```csharp
  protected override Composite CreateBehavior()
  ```
  - **Returns:** ReferenceForcedWhile Class

- **`Dispose Method`**
  ```csharp
  [ObsoleteAttribute("Use OnFinished instead.")]
public override void Dispose()
  ```
  Releases all resources used by the ForcedWhile

- **`OnFinished Method`**
  ```csharp
  public override void OnFinished()
  ```

---

### QuestOrder Class

**Inheritance:** System.Object → Bots.Quest.QuestOrder.QuestOrder

```csharp
public class QuestOrder
```

A question order.

#### QuestOrder Constructor

- **`QuestOrder Constructor`**
  ```csharp
  public QuestOrder()
  ```
  Default constructor.

- **`QuestOrder Constructor (OrderNodeCollection)`**
  ```csharp
  public QuestOrder(
OrderNodeCollection order
)
  ```
  Constructor.
  - *order*: Type: Styx.CommonBot.Profiles.Quest.OrderAddLanguageSpecificTextSet("LST20746C93_0?cs=.|vb=.|cpp=::|nu=.|fs=.");OrderNodeCollectionThe order.

#### QuestOrder Properties

- **`ContinuallySkipToCheckpoints Property`**
  ```csharp
  [ObsoleteAttribute("Please use the 'RecheckCheckpoints' tag instead")]
public bool ContinuallySkipToCheckpoints { get; set; }
  ```
  Gets or sets a value indicating whether the checkpoints.

- **`CurrentBehavior Property`**
  ```csharp
  public ForcedBehavior CurrentBehavior { get; set; }
  ```
  Gets or sets the current behavior.

- **`CurrentNode Property`**
  ```csharp
  public OrderNode CurrentNode { get; }
  ```
  Gets the current node.

- **`IgnoreCheckpoints Property`**
  ```csharp
  public bool IgnoreCheckpoints { get; set; }
  ```
  Gets or sets a value indicating whether the ignore checkpoints.

- **`Instance Property`**
  ```csharp
  public static QuestOrder Instance { get; }
  ```
  Gets the instance.

- **`NavType Property`**
  ```csharp
  public NavType NavType { get; }
  ```
  Gets or sets the type of the navigation.

- **`NextNode Property`**
  ```csharp
  public OrderNode NextNode { get; }
  ```
  Gets the next node.

- **`Nodes Property`**
  ```csharp
  public OrderNodeCollection Nodes { get; set; }
  ```
  Gets or sets the nodes.

#### QuestOrder Methods

- **`Advance Method`**
  Advances.

- **`Advance Method`**
  ```csharp
  public void Advance()
  ```
  Advances.

- **`Advance Method (Int32)`**
  ```csharp
  public void Advance(
int times
)
  ```
  Advances.
  - *times*: Type: SystemAddLanguageSpecificTextSet("LSTBC93389D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The times.

- **`RemoveCompletedNodes Method`**
  ```csharp
  public void RemoveCompletedNodes(
HashSet&lt;uint&gt; completedQuests
)
  ```
  Removes the completed nodes described by completedQuests.
  - *completedQuests*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST28C856E3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");HashSetAddLanguageSpecificTextSet("LST28C856E3_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");UInt32AddLanguageSpecificTextSet("LST28C856E3_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The completed quests.

- **`SkipToCheckpoint Method`**
  ```csharp
  public void SkipToCheckpoint(
float level
)
  ```
  Skip to checkpoint.
  - *level*: Type: SystemAddLanguageSpecificTextSet("LSTF5144DF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe level.

- **`UpdateNodes Method`**
  ```csharp
  public void UpdateNodes()
  ```
  First skips to the current checkpoint based on the characters level (if
            IgnoreCheckpoints isn't true), then removes all already completed nodes.

#### QuestOrder Events

- **`OnNoMoreNodes Event`**
  ```csharp
  public event EventHandler&lt;EventArgs&gt; OnNoMoreNodes
  ```
  Event queue for all listeners interested in OnNoMoreNodes events.

---
