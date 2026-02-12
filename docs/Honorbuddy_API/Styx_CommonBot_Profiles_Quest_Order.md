# Styx.CommonBot.Profiles.Quest.Order

Contains profile related quest order node classes.

## Contents

- [AbandonQuestNode Class](#abandonquestnode-class)
- [CheckpointNode Class](#checkpointnode-class)
- [ClearAvoidMobsNode Class](#clearavoidmobsnode-class)
- [ClearBlacklistNode Class](#clearblacklistnode-class)
- [ClearGrindAreaNode Class](#cleargrindareanode-class)
- [ClearMailboxNode Class](#clearmailboxnode-class)
- [ClearVendorNode Class](#clearvendornode-class)
- [CodeNode Class](#codenode-class)
- [CompileBatch Class](#compilebatch-class)
- [CompileError Class](#compileerror-class)
- [DelayCompiledExpression Class](#delaycompiledexpression-class)
- [DelayCompiledExpression(T) Class](#delaycompiledexpressiont-class)
- [DisableRepairNode Class](#disablerepairnode-class)
- [Else Class](#else-class)
- [ElseIf Class](#elseif-class)
- [EnableRepairNode Class](#enablerepairnode-class)
- [GrindToNode Class](#grindtonode-class)
- [INodeContainer Interface](#inodecontainer-interface)
- [IfNode Class](#ifnode-class)
- [MoveToNode Class](#movetonode-class)
- [ObjectiveNode Class](#objectivenode-class)
- [OrderNode Class](#ordernode-class)
- [OrderNodeCollection Class](#ordernodecollection-class)
- [OrderNodeType Enumeration](#ordernodetype-enumeration)
- [PickUpNode Class](#pickupnode-class)
- [ProfileHelperFunctionsBase Class](#profilehelperfunctionsbase-class)
- [ProfileHelperFunctionsBase.ObjectFilterDelegate Delegate](#profilehelperfunctionsbase.objectfilterdelegate-delegate)
- [ProfileHelperFunctionsBase.UnitFilterDelegate Delegate](#profilehelperfunctionsbase.unitfilterdelegate-delegate)
- [QuestBehaviorHelper Class](#questbehaviorhelper-class)
- [QuestObjectType Enumeration](#questobjecttype-enumeration)
- [RecheckCheckpoints Class](#recheckcheckpoints-class)
- [SetAvoidMobsNode Class](#setavoidmobsnode-class)
- [SetBlacklistNode Class](#setblacklistnode-class)
- [SetGrindAreaNode Class](#setgrindareanode-class)
- [SetLootMobsNode Class](#setlootmobsnode-class)
- [SetLootRadiusNode Class](#setlootradiusnode-class)
- [SetMailboxNode Class](#setmailboxnode-class)
- [SetNavTypeNode Class](#setnavtypenode-class)
- [SetTargetingDistanceNode Class](#settargetingdistancenode-class)
- [SetUseMountNode Class](#setusemountnode-class)
- [SetVendorNode Class](#setvendornode-class)
- [ToggleBehaviorNode Class](#togglebehaviornode-class)
- [TurnInNode Class](#turninnode-class)
- [UseItemNode Class](#useitemnode-class)
- [UseItemNode.UseItemTargetType Enumeration](#useitemnode.useitemtargettype-enumeration)
- [WhileNode Class](#whilenode-class)

---

### AbandonQuestNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.AbandonQuestNode

```csharp
public class AbandonQuestNode : OrderNode
```

An abandon question node.

#### AbandonQuestNode Constructor

- **`AbandonQuestNode Constructor (IEnumerable(UInt32), XElement)`**
  ```csharp
  public AbandonQuestNode(
IEnumerable&lt;uint&gt; variantQuestIds,
XElement xml
)
  ```
  Constructor.
  - *variantQuestIds*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTC45A0018_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTC45A0018_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");UInt32AddLanguageSpecificTextSet("LSTC45A0018_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTC45A0018_5?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`AbandonQuestNode Constructor (UInt32, XElement)`**
  ```csharp
  [ObsoleteAttribute("Use other constuctor")]
public AbandonQuestNode(
uint questId,
XElement xml
)
  ```
  Initializes a new instance of the AbandonQuestNode class
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LST99FFB7F8_0?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST99FFB7F8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElement

#### AbandonQuestNode Properties

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

#### AbandonQuestNode Methods

- **`FromXml Method`**
  ```csharp
  public static AbandonQuestNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST1D020562_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileMissingAttributeException Thrown when a profile missing
            attribute exception error condition occurs.ProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceAbandonQuestNode Class

---

### CheckpointNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.CheckpointNode

```csharp
public class CheckpointNode : OrderNode
```

A quest checkpoint is a node that does not modify behavior. It only tells HB points
            of which to start from.

#### CheckpointNode Properties

- **`Level Property`**
  ```csharp
  public float Level { get; }
  ```
  Gets the level.

#### CheckpointNode Methods

- **`FromXml Method`**
  ```csharp
  public static CheckpointNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST9FB2CB7B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceCheckpointNode Class

---

### ClearAvoidMobsNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.ClearAvoidMobsNode

```csharp
public class ClearAvoidMobsNode : OrderNode
```

A clear avoid mobs node.

#### ClearAvoidMobsNode Methods

- **`FromXml Method`**
  ```csharp
  public static ClearAvoidMobsNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTDEB3CF12_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ReferenceClearAvoidMobsNode Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceClearAvoidMobsNode Class

---

### ClearBlacklistNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.ClearBlacklistNode

```csharp
public class ClearBlacklistNode : OrderNode
```

A clear blacklist node.

#### ClearBlacklistNode Methods

- **`FromXml Method`**
  ```csharp
  public static ClearBlacklistNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTD86C420D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ReferenceClearBlacklistNode Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceClearBlacklistNode Class

---

### ClearGrindAreaNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.ClearGrindAreaNode

```csharp
public class ClearGrindAreaNode : OrderNode
```

A clear grind area node.

#### ClearGrindAreaNode Methods

- **`FromXml Method`**
  ```csharp
  public static ClearGrindAreaNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST563F9761_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ReferenceClearGrindAreaNode Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceClearGrindAreaNode Class

---

### ClearMailboxNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.ClearMailboxNode

```csharp
public class ClearMailboxNode : OrderNode
```

A clear mailbox node.

#### ClearMailboxNode Methods

- **`FromXml Method`**
  ```csharp
  public static ClearMailboxNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTCEF0405C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ReferenceClearMailboxNode Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceClearMailboxNode Class

---

### ClearVendorNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.ClearVendorNode

```csharp
public class ClearVendorNode : OrderNode
```

A clear vendor node.

#### ClearVendorNode Methods

- **`FromXml Method`**
  ```csharp
  public static ClearVendorNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTEFAC7FA6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ReferenceClearVendorNode Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceClearVendorNode Class

---

### CodeNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.CodeNode

```csharp
public class CodeNode : OrderNode
```

A code node.

#### CodeNode Properties

- **`Arguments Property`**
  ```csharp
  public Dictionary&lt;string, string&gt; Arguments { get; }
  ```
  public Func&lt;Assembly&gt; AssemblyGetter { get; private set; }

- **`BehaviorType Property`**
  ```csharp
  public Type BehaviorType { get; }
  ```
  Gets the type of the behavior.

- **`Path Property`**
  ```csharp
  public string Path { get; }
  ```
  Gets the full pathname of the file.

#### CodeNode Methods

- **`FromXml Method`**
  ```csharp
  public static CodeNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST7F915A0A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileMissingAttributeExceptionThrown when a profile missing
            attribute exception error condition occurs.ProfileException                        Thrown when a Profile error
            condition occurs.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceCodeNode Class

---

### CompileBatch Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.CompileBatch

```csharp
public class CompileBatch
```

Represents a batch of types and class members that are compiled in one go.

#### CompileBatch Properties

- **`Errors Property`**
  ```csharp
  public CompileError[] Errors { get; }
  ```

- **`HasErrors Property`**
  ```csharp
  public bool HasErrors { get; }
  ```

- **`Imports Property`**
  ```csharp
  public IReadOnlyList&lt;string&gt; Imports { get; }
  ```

- **`IsCompiled Property`**
  ```csharp
  public bool IsCompiled { get; }
  ```

#### CompileBatch Methods

- **`Add Method`**
  ```csharp
  public void Add(
string code,
Object context = null
)
  ```
  Adds the specified code.
  - *code*: Type: SystemAddLanguageSpecificTextSet("LST875532AA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe code.

- **`AddExpression Method`**
  Adds the expression.

- **`AddExpression(T) Method (String, Object)`**
  ```csharp
  public T AddExpression&lt;T&gt;(
string expression,
Object context = null
)
where T : class
  ```
  Adds the expression.
  - *expression*: Type: SystemAddLanguageSpecificTextSet("LST5253C3CF_3?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe expression.
  - **Returns:** See Also

- **`AddExpression Method (DelayCompiledExpression, Object)`**
  ```csharp
  public Delegate AddExpression(
DelayCompiledExpression delayCompiledExpression,
Object context = null
)
  ```
  Adds the expression.
  - *delayCompiledExpression*: Type: Styx.CommonBot.Profiles.Quest.OrderAddLanguageSpecificTextSet("LST9304E39C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");DelayCompiledExpressionThe delay compiled expression.
  - **Returns:** ExceptionConditionNotSupportedExceptionOnly type parameters of System.Action and System.Func and their derivates are supported.InvalidOperationExceptionCannot add an expression to an already compiled expression set

- **`AddExpression Method (String, Type, Object)`**
  ```csharp
  public Delegate AddExpression(
string expression,
Type delegateType,
Object context = null
)
  ```
  Adds the expression.
  - *expression*: Type: SystemAddLanguageSpecificTextSet("LST4BB9C19_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe expression.
  - *delegateType*: Type: SystemAddLanguageSpecificTextSet("LST4BB9C19_2?cs=.|vb=.|cpp=::|nu=.|fs=.");TypeType of the delegate.
  - **Returns:** ReferenceCompileBatch Class

- **`AddImport Method`**
  ```csharp
  public void AddImport(
string import
)
  ```
  - *import*: Type: SystemAddLanguageSpecificTextSet("LSTF62C2B9F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`Compile Method`**
  ```csharp
  public bool Compile()
  ```
  - **Returns:** ReferenceCompileBatch Class

---

### CompileError Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.CompileError

```csharp
public class CompileError
```

Initializes a new instance of the CompileError class

#### CompileError Properties

- **`Code Property`**
  ```csharp
  public string Code { get; }
  ```
  Gets the code.

- **`Context Property`**
  ```csharp
  public Object Context { get; }
  ```
  Gets the element.

- **`Error Property`**
  ```csharp
  public string Error { get; }
  ```
  Gets the error.

- **`Line Property`**
  ```csharp
  public int Line { get; }
  ```
  Gets the line.

---

### DelayCompiledExpression Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.DelayCompiledExpression → Styx.CommonBot.Profiles.Quest.Order.DelayCompiledExpression.T

```csharp
public class DelayCompiledExpression
```

Creates a delegate that can be used to execute a lambda expression after it's compiled some time in the future.
            The delegate is created right away but an exception is thrown if it's invoked before expression has been compiled

#### DelayCompiledExpression Properties

- **`CallableExpression Property`**
  ```csharp
  public Delegate CallableExpression { get; }
  ```
  An object of type DelegateType, that is always callable without null reference exceptions. When ExpressionString is compiled, this will invoke the compiled expression.

- **`DelegateType Property`**
  ```csharp
  public Type DelegateType { get; }
  ```
  Gets the type of the delegate.

- **`ExpressionString Property`**
  ```csharp
  public string ExpressionString { get; }
  ```
  Gets the expression string.

#### DelayCompiledExpression Methods

- **`Condition Method`**
  ```csharp
  public static DelayCompiledExpression&lt;Func&lt;bool&gt;&gt; Condition(
string conditionString
)
  ```
  Generates a lambda expression that takes no parameters. This simply places a () =&gt; in front of conditionString
  - *conditionString*: Type: SystemAddLanguageSpecificTextSet("LST190043D3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe condition string.
  - **Returns:** Exceptions

---

### DelayCompiledExpression(T) Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.DelayCompiledExpression → Styx.CommonBot.Profiles.Quest.Order.DelayCompiledExpression.T

```csharp
public class DelayCompiledExpression&lt;T&gt; : DelayCompiledExpression
```

Contains a CSharp lambda expression that is compiled at a latter time however a delegate instance (CallableExpression)is created when a DelayCompiledExpression instance is constructed.The CallableExpression can be stored and used after expression is compiled.

#### DelayCompiledExpression(T) Properties

- **`CallableExpression Property`**
  ```csharp
  public T CallableExpression { get; }
  ```

---

### DisableRepairNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.DisableRepairNode

```csharp
public class DisableRepairNode : OrderNode
```

A disable repair node.

#### DisableRepairNode Methods

- **`FromXml Method`**
  ```csharp
  public static DisableRepairNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST8FBD339C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ReferenceDisableRepairNode Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceDisableRepairNode Class

---

### Else Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.Else

```csharp
public class Else : IXmlObject
```

An else.

#### Else Properties

- **`Body Property`**
  ```csharp
  public OrderNodeCollection Body { get; }
  ```
  Gets the body.

- **`Element Property`**
  ```csharp
  public XElement Element { get; }
  ```
  Gets the xml element.

#### Else Methods

- **`FromXml Method`**
  ```csharp
  public static Else FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST7101ED32_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileExceptionThrown when a Profile error condition occurs.

---

### ElseIf Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.ElseIf

```csharp
public class ElseIf : IXmlObject
```

An else if.

#### ElseIf Properties

- **`Body Property`**
  ```csharp
  public OrderNodeCollection Body { get; }
  ```
  Gets the body.

- **`Condition Property`**
  ```csharp
  public DelayCompiledExpression&lt;Func&lt;bool&gt;&gt; Condition { get; }
  ```
  Gets the condition.

- **`Element Property`**
  ```csharp
  public XElement Element { get; }
  ```
  Gets the xml element.

#### ElseIf Methods

- **`FromXml Method`**
  ```csharp
  public static ElseIf FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTC3E51175_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileMissingAttributeExceptionThrown when a Profile Missing Attribute
            error condition occurs.ProfileExceptionProfileException.

---

### EnableRepairNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.EnableRepairNode

```csharp
public class EnableRepairNode : OrderNode
```

An enable repair node.

#### EnableRepairNode Methods

- **`FromXml Method`**
  ```csharp
  public static EnableRepairNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST2C28490D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ReferenceEnableRepairNode Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceEnableRepairNode Class

---

### GrindToNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.GrindToNode

```csharp
public class GrindToNode : OrderNode
```

A grind to node.

#### GrindToNode Constructor

- **`GrindToNode Constructor (Single, Nullable(NavType), XElement)`**
  ```csharp
  public GrindToNode(
float level,
Nullable&lt;NavType&gt; navType,
XElement xml
)
  ```
  Constructor.
  - *level*: Type: SystemAddLanguageSpecificTextSet("LST6FEDF152_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe level.
  - *navType*: Type: SystemAddLanguageSpecificTextSet("LST6FEDF152_3?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST6FEDF152_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");NavTypeAddLanguageSpecificTextSet("LST6FEDF152_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The type of the navigation.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST6FEDF152_6?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`GrindToNode Constructor (Single, String, Nullable(NavType), XElement)`**
  ```csharp
  public GrindToNode(
float level,
string conditionText,
Nullable&lt;NavType&gt; navType,
XElement xml
)
  ```
  Constructor.
  - *level*: Type: SystemAddLanguageSpecificTextSet("LSTBDDA87F2_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe level.
  - *conditionText*: Type: SystemAddLanguageSpecificTextSet("LSTBDDA87F2_3?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe condition text.
  - *navType*: Type: SystemAddLanguageSpecificTextSet("LSTBDDA87F2_4?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LSTBDDA87F2_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");NavTypeAddLanguageSpecificTextSet("LSTBDDA87F2_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The type of the navigation.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTBDDA87F2_7?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

#### GrindToNode Properties

- **`Condition Property`**
  ```csharp
  public DelayCompiledExpression&lt;Func&lt;bool&gt;&gt; Condition { get; }
  ```
  Gets the condition.

- **`GoalText Property`**
  ```csharp
  public string GoalText { get; }
  ```
  Gets the goal text.

- **`Level Property`**
  ```csharp
  public float Level { get; }
  ```
  Gets the level.

- **`NavType Property`**
  ```csharp
  public Nullable&lt;NavType&gt; NavType { get; }
  ```
  Gets the type of the navigaiton.

#### GrindToNode Methods

- **`FromXml Method`**
  ```csharp
  public static GrindToNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST37B91C22_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileAttributeExpectedException       Thrown when a Profile Attribute
            Expected error condition occurs.ProfileException                        Thrown when a Profile error
            condition occurs.ProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceGrindToNode Class

---

### INodeContainer Interface

```csharp
public interface INodeContainer
```

Interface for node container.

#### INodeContainer Methods

- **`GetNodes Method`**
  ```csharp
  IEnumerable&lt;OrderNode&gt; GetNodes()
  ```
  Gets the nodes in this collection.
  - **Returns:** See Also

---

### IfNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.IfNode

```csharp
public class IfNode : OrderNode, INodeContainer
```

if node.

#### IfNode Constructor

- **`IfNode Constructor (String, IEnumerable(OrderNode), XElement)`**
  ```csharp
  public IfNode(
string conditionText,
IEnumerable&lt;OrderNode&gt; body,
XElement xml
)
  ```
  Constructor.
  - *conditionText*: Type: SystemAddLanguageSpecificTextSet("LSTE26F167C_2?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe condition text.
  - *body*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTE26F167C_3?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTE26F167C_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");OrderNodeAddLanguageSpecificTextSet("LSTE26F167C_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The body.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTE26F167C_6?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`IfNode Constructor (String, IEnumerable(OrderNode), Else, XElement)`**
  ```csharp
  public IfNode(
string conditionText,
IEnumerable&lt;OrderNode&gt; body,
Else else,
XElement xml
)
  ```
  Constructor.
  - *conditionText*: Type: SystemAddLanguageSpecificTextSet("LST35AC6AD5_2?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe condition text.
  - *body*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST35AC6AD5_3?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST35AC6AD5_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");OrderNodeAddLanguageSpecificTextSet("LST35AC6AD5_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The body.
  - *else*: Type: Styx.CommonBot.Profiles.Quest.OrderAddLanguageSpecificTextSet("LST35AC6AD5_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ElseThe else.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST35AC6AD5_7?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`IfNode Constructor (String, IEnumerable(OrderNode), IEnumerable(ElseIf), XElement)`**
  ```csharp
  public IfNode(
string conditionText,
IEnumerable&lt;OrderNode&gt; body,
IEnumerable&lt;ElseIf&gt; elseIfs,
XElement xml
)
  ```
  Constructor.
  - *conditionText*: Type: SystemAddLanguageSpecificTextSet("LSTC31948AC_4?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe condition text.
  - *body*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTC31948AC_5?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTC31948AC_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");OrderNodeAddLanguageSpecificTextSet("LSTC31948AC_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The body.
  - *elseIfs*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTC31948AC_8?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTC31948AC_9?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");ElseIfAddLanguageSpecificTextSet("LSTC31948AC_10?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The else ifs.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTC31948AC_11?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`IfNode Constructor (String, IEnumerable(OrderNode), IEnumerable(ElseIf), Else, XElement)`**
  ```csharp
  public IfNode(
string conditionText,
IEnumerable&lt;OrderNode&gt; body,
IEnumerable&lt;ElseIf&gt; elseIfs,
Else else,
XElement xml
)
  ```
  Constructor.
  - *conditionText*: Type: SystemAddLanguageSpecificTextSet("LST95656CBB_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String(Optional) The condition text.
  - *body*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST95656CBB_5?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST95656CBB_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");OrderNodeAddLanguageSpecificTextSet("LST95656CBB_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The body.
  - *elseIfs*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST95656CBB_8?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST95656CBB_9?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");ElseIfAddLanguageSpecificTextSet("LST95656CBB_10?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The else ifs.
  - *else*: Type: Styx.CommonBot.Profiles.Quest.OrderAddLanguageSpecificTextSet("LST95656CBB_11?cs=.|vb=.|cpp=::|nu=.|fs=.");ElseThe else.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST95656CBB_12?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

#### IfNode Properties

- **`Body Property`**
  ```csharp
  public OrderNodeCollection Body { get; }
  ```
  Gets the body.

- **`Condition Property`**
  ```csharp
  public DelayCompiledExpression&lt;Func&lt;bool&gt;&gt; Condition { get; }
  ```
  Gets the condition.

- **`ConditionText Property`**
  ```csharp
  public string ConditionText { get; }
  ```
  Gets the condition text.

- **`Else Property`**
  ```csharp
  public Else Else { get; }
  ```
  Gets the else.

- **`ElseIfs Property`**
  ```csharp
  public List&lt;ElseIf&gt; ElseIfs { get; }
  ```
  Gets the else ifs.

#### IfNode Methods

- **`FromXml Method`**
  ```csharp
  public static IfNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST8798F2E0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileMissingAttributeExceptionThrown when a Profile Missing Attribute
            error condition occurs.ProfileExceptionProfileException.

- **`GetNodes Method`**
  ```csharp
  public IEnumerable&lt;OrderNode&gt; GetNodes()
  ```
  Gets the nodes in this collection.
  - **Returns:** See Also

---

### MoveToNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.MoveToNode

```csharp
public class MoveToNode : OrderNode
```

A move to node.

#### MoveToNode Constructor

- **`MoveToNode Constructor (Vector3, String, Single, IEnumerable(UInt32), Nullable(NavType), Boolean, XElement)`**
  ```csharp
  [ObsoleteAttribute("Use the other constuctor")]
public MoveToNode(
Vector3 location,
string locationName,
float precision,
IEnumerable&lt;uint&gt; variantQuestIds,
Nullable&lt;NavType&gt; navType,
bool harvest,
XElement xml
)
  ```
  Initializes a new instance of the MoveToNode class
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LSTD72A2CE5_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - *locationName*: Type: SystemAddLanguageSpecificTextSet("LSTD72A2CE5_5?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *precision*: Type: SystemAddLanguageSpecificTextSet("LSTD72A2CE5_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *variantQuestIds*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTD72A2CE5_7?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTD72A2CE5_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");UInt32AddLanguageSpecificTextSet("LSTD72A2CE5_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *navType*: Type: SystemAddLanguageSpecificTextSet("LSTD72A2CE5_10?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LSTD72A2CE5_11?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");NavTypeAddLanguageSpecificTextSet("LSTD72A2CE5_12?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *harvest*: Type: SystemAddLanguageSpecificTextSet("LSTD72A2CE5_13?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTD72A2CE5_14?cs=.|vb=.|cpp=::|nu=.|fs=.");XElement

- **`MoveToNode Constructor (Vector3, String, Single, UInt32, Nullable(NavType), Boolean, XElement)`**
  ```csharp
  [ObsoleteAttribute("Use the other constuctor")]
public MoveToNode(
Vector3 location,
string locationName,
float precision,
uint questId,
Nullable&lt;NavType&gt; navType,
bool harvest,
XElement xml
)
  ```
  Constructor.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LSTB5FAD5AF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *locationName*: Type: SystemAddLanguageSpecificTextSet("LSTB5FAD5AF_3?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name of the location.
  - *precision*: Type: SystemAddLanguageSpecificTextSet("LSTB5FAD5AF_4?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe precision.
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LSTB5FAD5AF_5?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *navType*: Type: SystemAddLanguageSpecificTextSet("LSTB5FAD5AF_6?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LSTB5FAD5AF_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");NavTypeAddLanguageSpecificTextSet("LSTB5FAD5AF_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The navigation type
  - *harvest*: Type: SystemAddLanguageSpecificTextSet("LSTB5FAD5AF_9?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTB5FAD5AF_10?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`MoveToNode Constructor (Vector3, String, Single, IEnumerable(UInt32), Nullable(NavType), Boolean, Boolean, XElement)`**
  ```csharp
  public MoveToNode(
Vector3 location,
string locationName,
float precision,
IEnumerable&lt;uint&gt; variantQuestIds,
Nullable&lt;NavType&gt; navType,
bool harvest,
bool land,
XElement xml
)
  ```
  Constructor.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST16186CC2_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *locationName*: Type: SystemAddLanguageSpecificTextSet("LST16186CC2_5?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name of the location.
  - *precision*: Type: SystemAddLanguageSpecificTextSet("LST16186CC2_6?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe precision.
  - *variantQuestIds*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST16186CC2_7?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST16186CC2_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");UInt32AddLanguageSpecificTextSet("LST16186CC2_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *navType*: Type: SystemAddLanguageSpecificTextSet("LST16186CC2_10?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST16186CC2_11?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");NavTypeAddLanguageSpecificTextSet("LST16186CC2_12?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The navigation type
  - *harvest*: Type: SystemAddLanguageSpecificTextSet("LST16186CC2_13?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanHarvest nodes while navigating to location
  - *land*: Type: SystemAddLanguageSpecificTextSet("LST16186CC2_14?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanLands after arriving at location; Only applicable if navType is set to 'Fly'
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST16186CC2_15?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

#### MoveToNode Properties

- **`Harvest Property`**
  ```csharp
  public bool Harvest { get; }
  ```

- **`Land Property`**
  ```csharp
  public bool Land { get; }
  ```
  Lands after arriving at Location; Only applicable if NavType is set to 'Fly'

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
  public Nullable&lt;NavType&gt; NavType { get; }
  ```
  Gets or sets the type of the navigation.

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

#### MoveToNode Methods

- **`FromXml Method`**
  ```csharp
  public static MoveToNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTB79E83CF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileException                        Thrown when a Profile error
            condition occurs.ProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.ProfileAttributeExpectedException  Thrown when a profile attribute
            expected exception error condition occurs.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceMoveToNode Class

---

### ObjectiveNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.ObjectiveNode

```csharp
public class ObjectiveNode : OrderNode
```

A quest objective tells HB to perform an objective for a quest.

#### ObjectiveNode Constructor

- **`ObjectiveNode Constructor (IEnumerable(UInt32), String, ObjectiveType, UInt32, String, UInt32, Vector3, Nullable(NavType), XElement)`**
  ```csharp
  public ObjectiveNode(
IEnumerable&lt;uint&gt; variantQuestIds,
string questName,
ObjectiveType objectiveType,
uint objectiveId,
string objectiveName,
uint objectiveCount,
Vector3 forceHotspot,
Nullable&lt;NavType&gt; navType,
XElement xml
)
  ```
  Constructor.
  - *variantQuestIds*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTECEC6D2B_4?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTECEC6D2B_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");UInt32AddLanguageSpecificTextSet("LSTECEC6D2B_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The variant quest identifiers.
  - *questName*: Type: SystemAddLanguageSpecificTextSet("LSTECEC6D2B_7?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name of the question.
  - *objectiveType*: Type: Styx.CommonBot.Profiles.QuestAddLanguageSpecificTextSet("LSTECEC6D2B_8?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectiveTypeThe type of the objective.
  - *objectiveId*: Type: SystemAddLanguageSpecificTextSet("LSTECEC6D2B_9?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier of the objective.
  - *objectiveName*: Type: SystemAddLanguageSpecificTextSet("LSTECEC6D2B_10?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name of the objective.
  - *objectiveCount*: Type: SystemAddLanguageSpecificTextSet("LSTECEC6D2B_11?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The number of objectives.
  - *forceHotspot*: Type: System.NumericsAddLanguageSpecificTextSet("LSTECEC6D2B_12?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The force hotspot.
  - *navType*: Type: SystemAddLanguageSpecificTextSet("LSTECEC6D2B_13?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LSTECEC6D2B_14?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");NavTypeAddLanguageSpecificTextSet("LSTECEC6D2B_15?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The type of the navigaiton.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTECEC6D2B_16?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`ObjectiveNode Constructor (UInt32, String, ObjectiveType, UInt32, String, UInt32, Vector3, Nullable(NavType), XElement)`**
  ```csharp
  [ObsoleteAttribute("Use the other constructor")]
public ObjectiveNode(
uint questId,
string questName,
ObjectiveType objectiveType,
uint objectiveId,
string objectiveName,
uint objectiveCount,
Vector3 forceHotspot,
Nullable&lt;NavType&gt; navType,
XElement xml
)
  ```
  Initializes a new instance of the ObjectiveNode class
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LST22ACC7C9_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *questName*: Type: SystemAddLanguageSpecificTextSet("LST22ACC7C9_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *objectiveType*: Type: Styx.CommonBot.Profiles.QuestAddLanguageSpecificTextSet("LST22ACC7C9_4?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectiveType
  - *objectiveId*: Type: SystemAddLanguageSpecificTextSet("LST22ACC7C9_5?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *objectiveName*: Type: SystemAddLanguageSpecificTextSet("LST22ACC7C9_6?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *objectiveCount*: Type: SystemAddLanguageSpecificTextSet("LST22ACC7C9_7?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *forceHotspot*: Type: System.NumericsAddLanguageSpecificTextSet("LST22ACC7C9_8?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - *navType*: Type: SystemAddLanguageSpecificTextSet("LST22ACC7C9_9?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST22ACC7C9_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");NavTypeAddLanguageSpecificTextSet("LST22ACC7C9_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST22ACC7C9_12?cs=.|vb=.|cpp=::|nu=.|fs=.");XElement

#### ObjectiveNode Properties

- **`ForcedHotspot Property`**
  ```csharp
  public Vector3 ForcedHotspot { get; }
  ```
  Gets the forced hotspot.

- **`NavType Property`**
  ```csharp
  public Nullable&lt;NavType&gt; NavType { get; }
  ```
  Gets the type of the navigaiton.

- **`ObjectiveCount Property`**
  ```csharp
  public uint ObjectiveCount { get; }
  ```
  Gets the number of objectives.

- **`ObjectiveId Property`**
  ```csharp
  public uint ObjectiveId { get; }
  ```
  Gets the identifier of the objective.

- **`ObjectiveName Property`**
  ```csharp
  public string ObjectiveName { get; }
  ```
  Gets the name of the objective.

- **`ObjectiveType Property`**
  ```csharp
  public ObjectiveType ObjectiveType { get; }
  ```
  Gets the type of the objective.

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

#### ObjectiveNode Methods

- **`FromXml Method`**
  ```csharp
  public static ObjectiveNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST8DEE7DC2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileMissingAttributeException Thrown when a profile missing
            attribute exception error condition occurs.ProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.ProfileMissingAttributeException      Thrown when a Profile Missing
            Attribute error condition occurs.ProfileAttributeExpectedException     Thrown when a Profile Attribute
            Expected error condition occurs.ProfileMissingAttributeException Thrown when a profile missing
            attribute exception error condition occurs.ProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.ProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceObjectiveNode Class

---

### OrderNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → More.

```csharp
public abstract class OrderNode : IXmlObject
```

An order node.

#### OrderNode Properties

- **`Element Property`**
  ```csharp
  protected XElement Element { get; set; }
  ```
  Gets the element.

- **`Type Property`**
  ```csharp
  public OrderNodeType Type { get; }
  ```
  Gets the type.

#### OrderNode Methods

- **`FromXml Method`**
  ```csharp
  public static OrderNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST36D3FB3B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileUnknownElementExceptionThrown when a Profile Unknown Element error
            condition occurs.

---

### OrderNodeCollection Class

**Inheritance:** System.Object → System.Collections.Generic.List.OrderNode → Styx.CommonBot.Profiles.Quest.Order.OrderNodeCollection

```csharp
public class OrderNodeCollection : List&lt;OrderNode&gt;,
INodeContainer
```

TODO: Convert to use Styx.Helpers.IndexedList and check usages.

#### OrderNodeCollection Constructor

- **`OrderNodeCollection Constructor`**
  ```csharp
  public OrderNodeCollection()
  ```
  Default constructor.

- **`OrderNodeCollection Constructor (Int32)`**
  ```csharp
  public OrderNodeCollection(
int capacity
)
  ```
  Constructor.
  - *capacity*: Type: SystemAddLanguageSpecificTextSet("LST88B9C0A2_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The capacity.

- **`OrderNodeCollection Constructor (IEnumerable(OrderNode))`**
  ```csharp
  public OrderNodeCollection(
IEnumerable&lt;OrderNode&gt; collection
)
  ```
  Constructor.
  - *collection*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTFF090FA5_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTFF090FA5_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");OrderNodeAddLanguageSpecificTextSet("LSTFF090FA5_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The collection.

#### OrderNodeCollection Properties

- **`ContinuallySkipToCheckpoints Property`**
  ```csharp
  public bool ContinuallySkipToCheckpoints { get; set; }
  ```
  Evaluates checkpoints dynamically as bot is running

- **`IgnoreCheckpoints Property`**
  ```csharp
  public bool IgnoreCheckpoints { get; set; }
  ```
  Gets or sets a value indicating whether the ignore checkpoints.

#### OrderNodeCollection Methods

- **`FromXml Method`**
  ```csharp
  public static OrderNodeCollection FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST949559D1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ReferenceOrderNodeCollection Class

- **`GetNodes Method`**
  ```csharp
  public IEnumerable&lt;OrderNode&gt; GetNodes()
  ```
  Gets the nodes in this collection.
  - **Returns:** See Also

---

### OrderNodeType Enumeration

```csharp
public enum OrderNodeType
```

Values that represent OrderNodeType.

---

### PickUpNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.PickUpNode

```csharp
public class PickUpNode : OrderNode
```

A pick up tells HB to pick up a quest.

#### PickUpNode Constructor

- **`PickUpNode Constructor (Vector3, UInt32, String, Nullable(QuestObjectType), IEnumerable(UInt32), String, Nullable(NavType), XElement)`**
  ```csharp
  public PickUpNode(
Vector3 giverLocation,
uint giverId,
string giverName,
Nullable&lt;QuestObjectType&gt; giverType,
IEnumerable&lt;uint&gt; variantQuestIds,
string questName,
Nullable&lt;NavType&gt; navType,
XElement xml
)
  ```
  Constructor.
  - *giverLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LST723BA878_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The giver location.
  - *giverId*: Type: SystemAddLanguageSpecificTextSet("LST723BA878_7?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier of the giver.
  - *giverName*: Type: SystemAddLanguageSpecificTextSet("LST723BA878_8?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name of the giver.
  - *giverType*: Type: SystemAddLanguageSpecificTextSet("LST723BA878_9?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST723BA878_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");QuestObjectTypeAddLanguageSpecificTextSet("LST723BA878_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The type of the giver.
  - *variantQuestIds*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST723BA878_12?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST723BA878_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");UInt32AddLanguageSpecificTextSet("LST723BA878_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The variant quest IDs.
  - *questName*: Type: SystemAddLanguageSpecificTextSet("LST723BA878_15?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name of the question.
  - *navType*: Type: SystemAddLanguageSpecificTextSet("LST723BA878_16?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST723BA878_17?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");NavTypeAddLanguageSpecificTextSet("LST723BA878_18?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The type of the navigaiton.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST723BA878_19?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`PickUpNode Constructor (Vector3, UInt32, String, Nullable(QuestObjectType), UInt32, String, Nullable(NavType), XElement)`**
  ```csharp
  [ObsoleteAttribute("Use the other constuctor")]
public PickUpNode(
Vector3 giverLocation,
uint giverId,
string giverName,
Nullable&lt;QuestObjectType&gt; giverType,
uint questId,
string questName,
Nullable&lt;NavType&gt; navType,
XElement xml
)
  ```
  Initializes a new instance of the PickUpNode class
  - *giverLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LST5F5D28F0_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - *giverId*: Type: SystemAddLanguageSpecificTextSet("LST5F5D28F0_5?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *giverName*: Type: SystemAddLanguageSpecificTextSet("LST5F5D28F0_6?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *giverType*: Type: SystemAddLanguageSpecificTextSet("LST5F5D28F0_7?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST5F5D28F0_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");QuestObjectTypeAddLanguageSpecificTextSet("LST5F5D28F0_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LST5F5D28F0_10?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *questName*: Type: SystemAddLanguageSpecificTextSet("LST5F5D28F0_11?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *navType*: Type: SystemAddLanguageSpecificTextSet("LST5F5D28F0_12?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST5F5D28F0_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");NavTypeAddLanguageSpecificTextSet("LST5F5D28F0_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST5F5D28F0_15?cs=.|vb=.|cpp=::|nu=.|fs=.");XElement

#### PickUpNode Properties

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

- **`NavType Property`**
  ```csharp
  public Nullable&lt;NavType&gt; NavType { get; }
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

#### PickUpNode Methods

- **`FromXml Method`**
  ```csharp
  public static PickUpNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST453E05B1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.ProfileMissingAttributeException Thrown when a profile missing
            attribute exception error condition occurs.ProfileAttributeExpectedException     Thrown when a Profile Attribute
            Expected error condition occurs.ProfileMissingAttributeException Thrown when a profile missing
            attribute exception error condition occurs.ProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferencePickUpNode Class

---

### ProfileHelperFunctionsBase Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.ProfileHelperFunctionsBase

```csharp
public class ProfileHelperFunctionsBase
```

A profile helper functions base.

#### ProfileHelperFunctionsBase Properties

- **`CurrentScenarioStage Property`**
  ```csharp
  public int CurrentScenarioStage { get; }
  ```
  Gets the current scenario stage index, 1-based. Return zero if not in a scenario.

- **`HerbalismSkill Property`**
  ```csharp
  public WoWSkill HerbalismSkill { get; }
  ```
  Gets the herbalism skill.

- **`Me Property`**
  ```csharp
  public LocalPlayer Me { get; }
  ```
  Gets me.

- **`MiningSkill Property`**
  ```csharp
  public WoWSkill MiningSkill { get; }
  ```
  Gets the mining skill.

- **`OnTransport Property`**
  ```csharp
  public bool OnTransport { get; }
  ```
  Gets a value indicating whether the player is on a transport.

- **`SkinningSkill Property`**
  ```csharp
  public WoWSkill SkinningSkill { get; }
  ```
  Gets the skinning skill.

#### ProfileHelperFunctionsBase Methods

- **`CanFly Method`**
  ```csharp
  public bool CanFly()
  ```
  Determines if we can fly.
  - **Returns:** This function returns true in the following situations:
            The local player has control over a vehicle that can flyThe local player is on a flying mountThe local player is in an area that is flyable (even while indoors)

- **`Chance Method`**
  ```csharp
  public bool Chance(
double val
)
  ```
  Returns true randomly.
  - *val*: Type: SystemAddLanguageSpecificTextSet("LST99C0F411_1?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe chance this function should return true.
  - **Returns:** The following will enter the 'if' 25% of the time:
            	Copy

- **`DistanceTo Method`**
  ```csharp
  public float DistanceTo(
float x,
float y,
float z
)
  ```
  Gets the distance to the specified coordinates.
  - *x*: Type: SystemAddLanguageSpecificTextSet("LST31B35EC4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *y*: Type: SystemAddLanguageSpecificTextSet("LST31B35EC4_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *z*: Type: SystemAddLanguageSpecificTextSet("LST31B35EC4_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`DistanceToUnit Method`**
  ```csharp
  public double DistanceToUnit(
uint entry
)
  ```
  Gets the distance to a unit.
  - *entry*: Type: SystemAddLanguageSpecificTextSet("LST37FE6813_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The entry of the unit.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`GetCurrencyAmount Method`**
  ```csharp
  public uint GetCurrencyAmount(
uint currencyId
)
  ```
  Gets the amount of a currency.
  - *currencyId*: Type: SystemAddLanguageSpecificTextSet("LSTCE129568_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32Identifier for the currency.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`GetFactionReputation Method`**
  ```csharp
  public int GetFactionReputation(
int factionId
)
  ```
  Gets faction reputation.
  - *factionId*: Type: SystemAddLanguageSpecificTextSet("LST511E9075_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32Identifier for the faction.
  - **Returns:** Faction reputation is absolute and centered around zero being neutral. The ranges for the different standings are:
            StandingRangeHated[-42000, -6001]Hostile[-6000, -3001]Unfriendly[-3000, -1]Neutral[0, 2999]Friendly[3000, 8999]Honored[9000, 20999]Revered[21000, 41999]Exalted[42000, 42999]

- **`GetFactionStanding Method`**
  ```csharp
  public FactionStanding GetFactionStanding(
int factionId
)
  ```
  Gets the faction standing of a faction.
  - *factionId*: Type: SystemAddLanguageSpecificTextSet("LST1C718414_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32Identifier for the faction.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`GetItemCount Method`**
  ```csharp
  public int GetItemCount(
int itemId
)
  ```
  Gets the item count of an item.
  - *itemId*: Type: SystemAddLanguageSpecificTextSet("LSTCC6037DA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32Identifier for the item.
  - **Returns:** This function only looks at the local players carried items, but includes quest items not visible in the bags.

- **`HasFaction Method`**
  ```csharp
  public bool HasFaction(
int factionId
)
  ```
  Queries if the local player has seen a faction.
  - *factionId*: Type: SystemAddLanguageSpecificTextSet("LSTB1E14BB4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32ID for the faction.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`HasItem Method`**
  ```csharp
  public bool HasItem(
int itemId
)
  ```
  Checks if the local player is carrying an item.
  - *itemId*: Type: SystemAddLanguageSpecificTextSet("LSTFABCB0CB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The ID of the item to check.
  - **Returns:** Remarks

- **`HasMinion Method`**
  ```csharp
  public bool HasMinion(
uint entry
)
  ```
  Queries if the local player has a minion summoned.
  - *entry*: Type: SystemAddLanguageSpecificTextSet("LST36B3A9C1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The entry of the minion.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`HasMount Method`**
  ```csharp
  public bool HasMount(
uint creatureSpellId
)
  ```
  Determines whether the player has the mount with the specified creature spell identifier.
  - *creatureSpellId*: Type: SystemAddLanguageSpecificTextSet("LST9F794A7E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The creature spell identifier.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`HasQuest Method`**
  ```csharp
  public bool HasQuest(
uint questId
)
  ```
  Queries if the quest log contains a quest.
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LSTA807F111_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32ID for the quest.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`HasQuestAvailable Method`**
  Queries if there is an object that has a quest available.

- **`HasQuestAvailable Method (Int32)`**
  ```csharp
  public bool HasQuestAvailable(
int objectId
)
  ```
  Queries if there is an object that has a quest available.
  - *objectId*: Type: SystemAddLanguageSpecificTextSet("LST46FF74C9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The ID of the object.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`HasQuestAvailable Method (Int32, QuestGiverStatus[])`**
  ```csharp
  public bool HasQuestAvailable(
int objectId,
params QuestGiverStatus[] statuses
)
  ```
  Queries if there is an object that has one of the given quest giver statuses.
  - *objectId*: Type: SystemAddLanguageSpecificTextSet("LSTA1C1CF8E_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The ID of the object.
  - *statuses*: Type: AddLanguageSpecificTextSet("LSTA1C1CF8E_4?cpp=array&lt;");StyxAddLanguageSpecificTextSet("LSTA1C1CF8E_5?cs=.|vb=.|cpp=::|nu=.|fs=.");QuestGiverStatusAddLanguageSpecificTextSet("LSTA1C1CF8E_6?cpp=&gt;|vb=()|nu=[]");The statuses.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`HasQuestAvailable Method (Int32, HashSet(QuestGiverStatus))`**
  ```csharp
  public bool HasQuestAvailable(
int objectId,
HashSet&lt;QuestGiverStatus&gt; statuses
)
  ```
  Queries if there is an object that has one of the given quest giver statuses.
  - *objectId*: Type: SystemAddLanguageSpecificTextSet("LST6319A4A9_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The ID of the object.
  - *statuses*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST6319A4A9_4?cs=.|vb=.|cpp=::|nu=.|fs=.");HashSetAddLanguageSpecificTextSet("LST6319A4A9_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");QuestGiverStatusAddLanguageSpecificTextSet("LST6319A4A9_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The statuses.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`HasQuestAvailable Method (Int32, String)`**
  ```csharp
  public bool HasQuestAvailable(
int objectId,
string type
)
  ```
  Queries if there is an object that has one of the given quest giver statuses.
  - *objectId*: Type: SystemAddLanguageSpecificTextSet("LSTC78600C7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The ID of the object.
  - *type*: Type: SystemAddLanguageSpecificTextSet("LSTC78600C7_2?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe statuses.
  - **Returns:** This function accepts a string delimited list of QuestGiverStatus.

- **`HasSpell Method`**
  Queries if the local player knows a spell.

- **`HasSpell Method (Int32)`**
  ```csharp
  public bool HasSpell(
int spellId
)
  ```
  Queries if the local player knows a spell.
  - *spellId*: Type: SystemAddLanguageSpecificTextSet("LSTC5325476_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32ID of the spell.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`HasSpell Method (String)`**
  ```csharp
  public bool HasSpell(
string spellName
)
  ```
  Queries if the local player knows a spell.
  - *spellName*: Type: SystemAddLanguageSpecificTextSet("LST1044C96F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the spell.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`IsAchievementCompleted Method`**
  Queries if an achievement objective is completed.

- **`IsAchievementCompleted Method (Int32, Boolean)`**
  ```csharp
  public bool IsAchievementCompleted(
int id,
bool accountWide = false
)
  ```
  Queries if an achievement objective is completed.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST70F27517_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The achievement ID.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`IsAchievementCompleted Method (Int32, Int32)`**
  ```csharp
  public bool IsAchievementCompleted(
int achievementId,
int index
)
  ```
  Queries if an achievement objective is completed.
  - *achievementId*: Type: SystemAddLanguageSpecificTextSet("LSTE22CD231_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The achievement ID.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTE22CD231_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The index of the objective in the achievement.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`IsObjectiveComplete Method`**
  ```csharp
  public bool IsObjectiveComplete(
int objectiveId,
uint questId
)
  ```
  Queries if an objective for a quest is completed.
  - *objectiveId*: Type: SystemAddLanguageSpecificTextSet("LST16FFD264_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32Identifier for the objective.
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LST16FFD264_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32Identifier for the question.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`IsQuestCompleted Method`**
  ```csharp
  public bool IsQuestCompleted(
uint id
)
  ```
  Queries if a quest is in the quest log and completed, or has already been completed previously.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTEB97CFBC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The quest ID.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`IsScenarioCriteriaComplete Method`**
  Determines whether the criteria for the current stage is complete.

- **`IsScenarioCriteriaComplete Method (Int32)`**
  ```csharp
  public bool IsScenarioCriteriaComplete(
int criteriaIndex
)
  ```
  Determines whether the criteria for the current stage is complete.
  - *criteriaIndex*: Type: SystemAddLanguageSpecificTextSet("LST17225D88_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The criteria index. 1-based
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`IsScenarioCriteriaComplete Method (Int32, Int32)`**
  ```csharp
  public bool IsScenarioCriteriaComplete(
int stageIndex,
int criteriaIndex
)
  ```
  Determines whether the criteria for given scenario stage is complete.
  - *stageIndex*: Type: SystemAddLanguageSpecificTextSet("LST53086867_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The stage number, 1-based.
  - *criteriaIndex*: Type: SystemAddLanguageSpecificTextSet("LST53086867_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The criteria number, 1-based.
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`Object Method`**
  ```csharp
  public WoWObject Object(
uint entry,
ProfileHelperFunctionsBase.ObjectFilterDelegate filter = null
)
  ```
  Finds an object by ID.
  - *entry*: Type: SystemAddLanguageSpecificTextSet("LSTB20CC34D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The entry of the object.
  - **Returns:** If there are multiple object with an ID of entry that matches filter, the closest is returned.

- **`Random Method`**
  Gets a random number greater or equal to 0, but less than max.

- **`Random Method (Int32)`**
  ```csharp
  public int Random(
int max
)
  ```
  Gets a random number greater or equal to 0, but less than max.
  - *max*: Type: SystemAddLanguageSpecificTextSet("LST8E19C023_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The max bound (exclusive).
  - **Returns:** Random(1) always returns 0; Random(2) returns either 0 or 1 with 50% chance of each.

- **`Random Method (Int32, Int32)`**
  ```csharp
  public int Random(
int min,
int max
)
  ```
  Gets a random number greater or equal to min, but less than max.
  - *min*: Type: SystemAddLanguageSpecificTextSet("LST4513554A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The minimum bound (inclusive).
  - *max*: Type: SystemAddLanguageSpecificTextSet("LST4513554A_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The maximum bound (exclusive).
  - **Returns:** ReferenceProfileHelperFunctionsBase Class

- **`Unit Method`**
  ```csharp
  public WoWUnit Unit(
uint entry,
ProfileHelperFunctionsBase.UnitFilterDelegate filter = null
)
  ```
  Finds a unit by ID.
  - *entry*: Type: SystemAddLanguageSpecificTextSet("LST73A8D06D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The entry of the unit.
  - **Returns:** If there are multiple units with an ID of entry that matches filter, the closest is returned.

---

### ProfileHelperFunctionsBase.ObjectFilterDelegate Delegate

```csharp
public delegate bool ObjectFilterDelegate(
WoWObject unit
)
```

Object filter delegate.

---

### ProfileHelperFunctionsBase.UnitFilterDelegate Delegate

```csharp
public delegate bool UnitFilterDelegate(
WoWUnit unit
)
```

Unit filter delegate.

---

### QuestBehaviorHelper Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.QuestBehaviorHelper

```csharp
public class QuestBehaviorHelper
```

A question behavior helper.

#### QuestBehaviorHelper Properties

- **`Path Property`**
  ```csharp
  public string Path { get; }
  ```
  Gets the full pathname of the file.

#### QuestBehaviorHelper Methods

- **`GetBehaviorInstanceType Method`**
  ```csharp
  public static Type GetBehaviorInstanceType(
string path
)
  ```
  Gets behavior instance type.
  - *path*: Type: SystemAddLanguageSpecificTextSet("LST5E570969_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringFull pathname of the file.
  - **Returns:** ExceptionConditionHonorbuddyUnableToStartExceptionThrown when a Honorbuddy Unable To Start
            error condition occurs.

---

### QuestObjectType Enumeration

```csharp
public enum QuestObjectType
```

Values that represent QuestObjectType.

---

### RecheckCheckpoints Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.RecheckCheckpoints

```csharp
public class RecheckCheckpoints : OrderNode
```

A quest checkpoint is a node that does not modify behavior. It only tells HB points
            of which to start from.

#### RecheckCheckpoints Methods

- **`FromXml Method`**
  ```csharp
  public static RecheckCheckpoints FromXml(
XElement element
)
  ```
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTE3B79289_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElement
  - **Returns:** ReferenceRecheckCheckpoints Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceRecheckCheckpoints Class

---

### SetAvoidMobsNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.SetAvoidMobsNode

```csharp
public class SetAvoidMobsNode : OrderNode
```

A set vendor node.

#### SetAvoidMobsNode Constructor

- **`SetAvoidMobsNode Constructor (DualHashSet(UInt32, String), XElement)`**
  ```csharp
  public SetAvoidMobsNode(
DualHashSet&lt;uint, string&gt; avoidMobs,
XElement xml
)
  ```
  Initializes a new instance of the SetAvoidMobsNode class.
  - *avoidMobs*: Type: Styx.CommonAddLanguageSpecificTextSet("LST70C93BDA_2?cs=.|vb=.|cpp=::|nu=.|fs=.");DualHashSetAddLanguageSpecificTextSet("LST70C93BDA_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");UInt32, StringAddLanguageSpecificTextSet("LST70C93BDA_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The avoidmobs.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST70C93BDA_5?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`SetAvoidMobsNode Constructor (UInt32, String, XElement)`**
  ```csharp
  public SetAvoidMobsNode(
uint mobId,
string mobName,
XElement xml
)
  ```
  Constructor.
  - *mobId*: Type: SystemAddLanguageSpecificTextSet("LST212BC475_0?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The mob identifier.
  - *mobName*: Type: SystemAddLanguageSpecificTextSet("LST212BC475_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the mob.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST212BC475_2?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

#### SetAvoidMobsNode Properties

- **`AvoidMobs Property`**
  ```csharp
  public DualHashSet&lt;uint, string&gt; AvoidMobs { get; }
  ```
  Gets the vendors.

#### SetAvoidMobsNode Methods

- **`FromXml Method`**
  ```csharp
  public static SetAvoidMobsNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST4B4AC9FD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileMissingElementExceptionThrown when a Profile Missing Element error condition occurs.ProfileException              Thrown when a Profile error condition occurs.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
                Object.
  - **Returns:** ReferenceSetAvoidMobsNode Class

---

### SetBlacklistNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.SetBlacklistNode

```csharp
public class SetBlacklistNode : OrderNode
```

A set vendor node.

#### SetBlacklistNode Constructor

- **`SetBlacklistNode Constructor (Dictionary(UInt32, BlacklistFlags), XElement)`**
  ```csharp
  public SetBlacklistNode(
Dictionary&lt;uint, BlacklistFlags&gt; blacklist,
XElement xml
)
  ```
  Constructor.
  - *blacklist*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST5F5B2D09_2?cs=.|vb=.|cpp=::|nu=.|fs=.");DictionaryAddLanguageSpecificTextSet("LST5F5B2D09_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");UInt32, BlacklistFlagsAddLanguageSpecificTextSet("LST5F5B2D09_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The blacklist.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST5F5B2D09_5?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`SetBlacklistNode Constructor (UInt32, BlacklistFlags, XElement)`**
  ```csharp
  public SetBlacklistNode(
uint mobId,
BlacklistFlags blackListFlags,
XElement xml
)
  ```
  Constructor.
  - *mobId*: Type: SystemAddLanguageSpecificTextSet("LST57B0F90_0?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The mob identifier.
  - *blackListFlags*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LST57B0F90_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BlacklistFlagsThe blacklist flags.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST57B0F90_2?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

#### SetBlacklistNode Properties

- **`Blacklist Property`**
  ```csharp
  public Dictionary&lt;uint, BlacklistFlags&gt; Blacklist { get; }
  ```
  Gets the vendors.

#### SetBlacklistNode Methods

- **`FromXml Method`**
  ```csharp
  public static SetBlacklistNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST35B455F8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileMissingElementExceptionThrown when a Profile Missing Element error condition occurs.ProfileException              Thrown when a Profile error condition occurs.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
                Object.
  - **Returns:** ReferenceSetBlacklistNode Class

---

### SetGrindAreaNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.SetGrindAreaNode

```csharp
public class SetGrindAreaNode : OrderNode
```

A set grind area node.

#### SetGrindAreaNode Properties

- **`Area Property`**
  ```csharp
  public GrindArea Area { get; }
  ```
  Gets the area.

#### SetGrindAreaNode Methods

- **`FromXml Method`**
  ```csharp
  public static SetGrindAreaNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST1CCC369C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileMissingElementExceptionThrown when a Profile Missing Element error
            condition occurs.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceSetGrindAreaNode Class

---

### SetLootMobsNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.SetLootMobsNode

```csharp
public class SetLootMobsNode : OrderNode
```

A set loot mobs node.

#### SetLootMobsNode Properties

- **`LootMobs Property`**
  ```csharp
  public Nullable&lt;bool&gt; LootMobs { get; }
  ```
  Gets the loot mobs.

#### SetLootMobsNode Methods

- **`FromXml Method`**
  ```csharp
  public static SetLootMobsNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTA5E5F738_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileMissingAttributeException Thrown when a profile missing
            attribute exception error condition occurs.ProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.

---

### SetLootRadiusNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.SetLootRadiusNode

```csharp
public class SetLootRadiusNode : OrderNode
```

A set loot radius node.

#### SetLootRadiusNode Properties

- **`LootRadius Property`**
  ```csharp
  public Nullable&lt;double&gt; LootRadius { get; }
  ```
  Gets the loot radius.

#### SetLootRadiusNode Methods

- **`FromXml Method`**
  ```csharp
  public static SetLootRadiusNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTDDD1AF2F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileMissingAttributeException Thrown when a profile missing
            attribute exception error condition occurs.ProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.

---

### SetMailboxNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.SetMailboxNode

```csharp
public class SetMailboxNode : OrderNode
```

A set mailbox node.

#### SetMailboxNode Constructor

- **`SetMailboxNode Constructor (Mailbox, XElement)`**
  ```csharp
  public SetMailboxNode(
Mailbox mailbox,
XElement xml
)
  ```
  Constructor.
  - *mailbox*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LSTFE26053D_0?cs=.|vb=.|cpp=::|nu=.|fs=.");MailboxThe mailbox.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTFE26053D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`SetMailboxNode Constructor (List(Mailbox), XElement)`**
  ```csharp
  public SetMailboxNode(
List&lt;Mailbox&gt; mailboxes,
XElement xml
)
  ```
  Constructor.
  - *mailboxes*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTC61B8BF0_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LSTC61B8BF0_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");MailboxAddLanguageSpecificTextSet("LSTC61B8BF0_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The mailboxes.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTC61B8BF0_5?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

#### SetMailboxNode Properties

- **`Mailboxes Property`**
  ```csharp
  public List&lt;Mailbox&gt; Mailboxes { get; }
  ```
  Gets the mailboxes.

#### SetMailboxNode Methods

- **`FromXml Method`**
  ```csharp
  public static SetMailboxNode FromXml(
XElement element
)
  ```
  Gets the usable whens.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST575A86D3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileMissingElementExceptionThrown when a Profile Missing Element error
            condition occurs.ProfileException              Thrown when a Profile error condition occurs.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceSetMailboxNode Class

---

### SetNavTypeNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.SetNavTypeNode

```csharp
public class SetNavTypeNode : OrderNode
```

A set nav type node.

#### SetNavTypeNode Properties

- **`NavType Property`**
  ```csharp
  public NavType NavType { get; }
  ```
  Gets or sets the type of the navigation.

#### SetNavTypeNode Methods

- **`FromXml Method`**
  ```csharp
  public static SetNavTypeNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTB688A9F6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileMissingAttributeException Thrown when a profile missing
            attribute exception error condition occurs.ProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.

---

### SetTargetingDistanceNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.SetTargetingDistanceNode

```csharp
public class SetTargetingDistanceNode : OrderNode
```

A set targeting distance node.

#### SetTargetingDistanceNode Properties

- **`TargetingDistance Property`**
  ```csharp
  public Nullable&lt;double&gt; TargetingDistance { get; }
  ```
  Gets the targeting distance.

#### SetTargetingDistanceNode Methods

- **`FromXml Method`**
  ```csharp
  public static SetTargetingDistanceNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTFAB3B8B5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileMissingAttributeException Thrown when a profile missing
            attribute exception error condition occurs.ProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.

---

### SetUseMountNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.SetUseMountNode

```csharp
public class SetUseMountNode : OrderNode
```

A set use mount node.

#### SetUseMountNode Properties

- **`UseMount Property`**
  ```csharp
  public bool UseMount { get; }
  ```
  Gets a value indicating whether this object use mount.

#### SetUseMountNode Methods

- **`FromXml Method`**
  ```csharp
  public static SetUseMountNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST96B3E9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileMissingAttributeException Thrown when a profile missing
            attribute exception error condition occurs.ProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.

---

### SetVendorNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.SetVendorNode

```csharp
public class SetVendorNode : OrderNode
```

A set vendor node.

#### SetVendorNode Constructor

- **`SetVendorNode Constructor (Vendor, XElement)`**
  ```csharp
  public SetVendorNode(
Vendor vendor,
XElement xml
)
  ```
  Constructor.
  - *vendor*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST59DB817D_0?cs=.|vb=.|cpp=::|nu=.|fs=.");VendorThe vendor.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST59DB817D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`SetVendorNode Constructor (List(Vendor), XElement)`**
  ```csharp
  public SetVendorNode(
List&lt;Vendor&gt; vendors,
XElement xml
)
  ```
  Constructor.
  - *vendors*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTD1A9685A_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LSTD1A9685A_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");VendorAddLanguageSpecificTextSet("LSTD1A9685A_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The vendors.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTD1A9685A_5?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

#### SetVendorNode Properties

- **`Vendors Property`**
  ```csharp
  public List&lt;Vendor&gt; Vendors { get; }
  ```
  Gets the vendors.

#### SetVendorNode Methods

- **`FromXml Method`**
  ```csharp
  public static SetVendorNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST1381B23F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileMissingElementExceptionThrown when a Profile Missing Element error
            condition occurs.ProfileException              Thrown when a Profile error condition occurs.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceSetVendorNode Class

---

### ToggleBehaviorNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.ToggleBehaviorNode

```csharp
public class ToggleBehaviorNode : OrderNode
```

A toggle behavior node.

#### ToggleBehaviorNode Properties

- **`BehaviorName Property`**
  ```csharp
  public string BehaviorName { get; }
  ```
  Gets the name of the behavior.

- **`Enabled Property`**
  ```csharp
  public bool Enabled { get; }
  ```
  Gets a value indicating whether this object is enabled.

#### ToggleBehaviorNode Methods

- **`FromXml Method`**
  ```csharp
  public static ToggleBehaviorNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTA5DA26B7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ReferenceToggleBehaviorNode Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceToggleBehaviorNode Class

---

### TurnInNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.TurnInNode

```csharp
public class TurnInNode : OrderNode
```

A turn in node.

#### TurnInNode Constructor

- **`TurnInNode Constructor (Vector3, UInt32, String, Nullable(QuestObjectType), IEnumerable(UInt32), String, Nullable(NavType), XElement)`**
  ```csharp
  public TurnInNode(
Vector3 turninLocation,
uint turnInId,
string turnInName,
Nullable&lt;QuestObjectType&gt; turnInType,
IEnumerable&lt;uint&gt; variantQuestIds,
string questName,
Nullable&lt;NavType&gt; navType,
XElement xml
)
  ```
  Constructor.
  - *turninLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LST763EB180_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The turn in location.
  - *turnInId*: Type: SystemAddLanguageSpecificTextSet("LST763EB180_7?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier of the turn in.
  - *turnInName*: Type: SystemAddLanguageSpecificTextSet("LST763EB180_8?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name of the turn in.
  - *turnInType*: Type: SystemAddLanguageSpecificTextSet("LST763EB180_9?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST763EB180_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");QuestObjectTypeAddLanguageSpecificTextSet("LST763EB180_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The type of the turn in.
  - *variantQuestIds*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST763EB180_12?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST763EB180_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");UInt32AddLanguageSpecificTextSet("LST763EB180_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The variant quest identifiers.
  - *questName*: Type: SystemAddLanguageSpecificTextSet("LST763EB180_15?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name of the question.
  - *navType*: Type: SystemAddLanguageSpecificTextSet("LST763EB180_16?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST763EB180_17?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");NavTypeAddLanguageSpecificTextSet("LST763EB180_18?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The type of the navigaiton.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST763EB180_19?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`TurnInNode Constructor (Vector3, UInt32, String, Nullable(QuestObjectType), UInt32, String, Nullable(NavType), XElement)`**
  ```csharp
  [ObsoleteAttribute("Use the other constructor")]
public TurnInNode(
Vector3 turninLocation,
uint turnInId,
string turnInName,
Nullable&lt;QuestObjectType&gt; turnInType,
uint questId,
string questName,
Nullable&lt;NavType&gt; navType,
XElement xml
)
  ```
  Initializes a new instance of the TurnInNode class
  - *turninLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LST4A7850AC_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - *turnInId*: Type: SystemAddLanguageSpecificTextSet("LST4A7850AC_5?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *turnInName*: Type: SystemAddLanguageSpecificTextSet("LST4A7850AC_6?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *turnInType*: Type: SystemAddLanguageSpecificTextSet("LST4A7850AC_7?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST4A7850AC_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");QuestObjectTypeAddLanguageSpecificTextSet("LST4A7850AC_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LST4A7850AC_10?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *questName*: Type: SystemAddLanguageSpecificTextSet("LST4A7850AC_11?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *navType*: Type: SystemAddLanguageSpecificTextSet("LST4A7850AC_12?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST4A7850AC_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");NavTypeAddLanguageSpecificTextSet("LST4A7850AC_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST4A7850AC_15?cs=.|vb=.|cpp=::|nu=.|fs=.");XElement

#### TurnInNode Properties

- **`NavType Property`**
  ```csharp
  public Nullable&lt;NavType&gt; NavType { get; }
  ```
  Gets the type of the navigaiton.

- **`QuestId Property`**
  ```csharp
  [ObsoleteAttribute("Use VariantQuestIds")]
public uint QuestId { get; }
  ```
  Gets the identifier of the question.

- **`QuestName Property`**
  ```csharp
  public string QuestName { get; }
  ```
  Gets the name of the question.

- **`TurnInId Property`**
  ```csharp
  public uint TurnInId { get; }
  ```
  Gets the identifier of the turn in.

- **`TurnInLocation Property`**
  ```csharp
  public Vector3 TurnInLocation { get; }
  ```
  Gets the turn in location.

- **`TurnInName Property`**
  ```csharp
  public string TurnInName { get; }
  ```
  Gets the name of the turn in.

- **`TurnInType Property`**
  ```csharp
  public Nullable&lt;QuestObjectType&gt; TurnInType { get; }
  ```
  Gets the type of the turn in.

- **`VariantQuestIds Property`**
  ```csharp
  public IReadOnlyCollection&lt;uint&gt; VariantQuestIds { get; }
  ```
  An array of the variant quest IDs. 
            These quests have the same objectives but are only available depending on class/race.

#### TurnInNode Methods

- **`FromXml Method`**
  ```csharp
  public static TurnInNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST820D31DF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.ProfileAttributeExpectedException     Thrown when a Profile Attribute
            Expected error condition occurs.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceTurnInNode Class

---

### UseItemNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.UseItemNode

```csharp
public class UseItemNode : OrderNode
```

An use item node.

#### UseItemNode Constructor

- **`UseItemNode Constructor (Func(WoWItem), UInt32, Vector3, Nullable(NavType), XElement)`**
  ```csharp
  public UseItemNode(
Func&lt;WoWItem&gt; itemRetriever,
uint questId,
Vector3 location,
Nullable&lt;NavType&gt; navType,
XElement xml
)
  ```
  Constructor.
  - *itemRetriever*: Type: SystemAddLanguageSpecificTextSet("LST8B673ABC_4?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST8B673ABC_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWItemAddLanguageSpecificTextSet("LST8B673ABC_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The item retriever.
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LST8B673ABC_7?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier of the question.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST8B673ABC_8?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *navType*: Type: SystemAddLanguageSpecificTextSet("LST8B673ABC_9?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST8B673ABC_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");NavTypeAddLanguageSpecificTextSet("LST8B673ABC_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The type of the navigaiton.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST8B673ABC_12?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`UseItemNode Constructor (Func(WoWItem), Func(WoWObject), UInt32, Vector3, Nullable(NavType), XElement)`**
  ```csharp
  public UseItemNode(
Func&lt;WoWItem&gt; itemRetriever,
Func&lt;WoWObject&gt; targetRetriever,
uint questId,
Vector3 location,
Nullable&lt;NavType&gt; navType,
XElement xml
)
  ```
  Constructor.
  - *itemRetriever*: Type: SystemAddLanguageSpecificTextSet("LST1BCB95B3_6?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1BCB95B3_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWItemAddLanguageSpecificTextSet("LST1BCB95B3_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The item retriever.
  - *targetRetriever*: Type: SystemAddLanguageSpecificTextSet("LST1BCB95B3_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1BCB95B3_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST1BCB95B3_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The target retriever.
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LST1BCB95B3_12?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier of the question.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST1BCB95B3_13?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *navType*: Type: SystemAddLanguageSpecificTextSet("LST1BCB95B3_14?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST1BCB95B3_15?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");NavTypeAddLanguageSpecificTextSet("LST1BCB95B3_16?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The type of the navigaiton.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST1BCB95B3_17?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`UseItemNode Constructor (Func(WoWItem), Func(WoWObject), UseItemNode.UseItemTargetType, Boolean, IEnumerable(UInt32), Vector3, Nullable(NavType), XElement)`**
  ```csharp
  public UseItemNode(
Func&lt;WoWItem&gt; itemRetriever,
Func&lt;WoWObject&gt; targetRetriever,
UseItemNode.UseItemTargetType targetType,
bool forceUse,
IEnumerable&lt;uint&gt; variantQuestids,
Vector3 location,
Nullable&lt;NavType&gt; navType,
XElement xml
)
  ```
  Constructor.
  - *itemRetriever*: Type: SystemAddLanguageSpecificTextSet("LST9396D05D_10?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST9396D05D_11?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWItemAddLanguageSpecificTextSet("LST9396D05D_12?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The item retriever.
  - *targetRetriever*: Type: SystemAddLanguageSpecificTextSet("LST9396D05D_13?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST9396D05D_14?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST9396D05D_15?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The target retriever.
  - *targetType*: Type: Styx.CommonBot.Profiles.Quest.OrderAddLanguageSpecificTextSet("LST9396D05D_16?cs=.|vb=.|cpp=::|nu=.|fs=.");UseItemNodeAddLanguageSpecificTextSet("LST9396D05D_17?cs=.|vb=.|cpp=::|nu=.|fs=.");UseItemTargetTypeThe type of the target.
  - *forceUse*: Type: SystemAddLanguageSpecificTextSet("LST9396D05D_18?cs=.|vb=.|cpp=::|nu=.|fs=.");Booleantrue if force use, false if not.
  - *variantQuestids*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST9396D05D_19?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST9396D05D_20?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");UInt32AddLanguageSpecificTextSet("LST9396D05D_21?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST9396D05D_22?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *navType*: Type: SystemAddLanguageSpecificTextSet("LST9396D05D_23?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST9396D05D_24?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");NavTypeAddLanguageSpecificTextSet("LST9396D05D_25?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The type of the navigaiton.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST9396D05D_26?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`UseItemNode Constructor (Func(WoWItem), Func(WoWObject), UseItemNode.UseItemTargetType, Boolean, UInt32, Vector3, Nullable(NavType), XElement)`**
  ```csharp
  [ObsoleteAttribute("Use another constructor")]
public UseItemNode(
Func&lt;WoWItem&gt; itemRetriever,
Func&lt;WoWObject&gt; targetRetriever,
UseItemNode.UseItemTargetType targetType,
bool forceUse,
uint questId,
Vector3 location,
Nullable&lt;NavType&gt; navType,
XElement xml
)
  ```
  Initializes a new instance of the UseItemNode class
  - *itemRetriever*: Type: SystemAddLanguageSpecificTextSet("LST54C04DD3_8?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST54C04DD3_9?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWItemAddLanguageSpecificTextSet("LST54C04DD3_10?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *targetRetriever*: Type: SystemAddLanguageSpecificTextSet("LST54C04DD3_11?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST54C04DD3_12?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST54C04DD3_13?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *targetType*: Type: Styx.CommonBot.Profiles.Quest.OrderAddLanguageSpecificTextSet("LST54C04DD3_14?cs=.|vb=.|cpp=::|nu=.|fs=.");UseItemNodeAddLanguageSpecificTextSet("LST54C04DD3_15?cs=.|vb=.|cpp=::|nu=.|fs=.");UseItemTargetType
  - *forceUse*: Type: SystemAddLanguageSpecificTextSet("LST54C04DD3_16?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LST54C04DD3_17?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST54C04DD3_18?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - *navType*: Type: SystemAddLanguageSpecificTextSet("LST54C04DD3_19?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST54C04DD3_20?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");NavTypeAddLanguageSpecificTextSet("LST54C04DD3_21?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST54C04DD3_22?cs=.|vb=.|cpp=::|nu=.|fs=.");XElement

#### UseItemNode Properties

- **`ForceUse Property`**
  ```csharp
  public bool ForceUse { get; }
  ```
  Gets a value indicating whether the use should be forced.

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
  public Nullable&lt;NavType&gt; NavType { get; }
  ```
  Gets the type of the navigaiton.

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

- **`TargetType Property`**
  ```csharp
  public UseItemNode.UseItemTargetType TargetType { get; }
  ```
  Gets the type of the target.

- **`VariantQuestIds Property`**
  ```csharp
  public IReadOnlyCollection&lt;uint&gt; VariantQuestIds { get; }
  ```
  An array of the variant quest IDs. 
            These quests have the same objectives but availability depends on class/race.

#### UseItemNode Methods

- **`FromXml Method`**
  ```csharp
  public static UseItemNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST6F8A143_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileAttributeExpectedException              Thrown when a profile
            attribute expected exception error condition occurs.ProfileAttributeExpectedExceptionThrown when a profile
            attribute expected exception error condition occurs.ProfileMissingAttributeException              Thrown when a profile
            missing attribute exception error condition occurs.ProfileAttributeExpectedException              Thrown when a profile
            attribute expected exception error condition occurs.ProfileMissingAttributeException               Thrown when a profile
            missing attribute exception error condition occurs.ProfileAttributeExpectedException              Thrown when a profile
            attribute expected exception error condition occurs.ProfileMissingAttributeException             Thrown when a profile
            missing attribute exception error condition occurs.ProfileMissingAttributeException             Thrown when a profile
            missing attribute exception error condition occurs.ProfileMissingAttributeException             Thrown when a profile
            missing attribute exception error condition occurs.ProfileAttributeExpectedException            Thrown when a profile
            attribute expected exception error condition occurs.ProfileAttributeExpectedException            Thrown when a profile
            attribute expected exception error condition occurs.ProfileAttributeExpectedException            Thrown when a profile
            attribute expected exception error condition occurs.ProfileAttributeExpectedException                   Thrown when a Profile
            Attribute Expected error condition occurs.

---

### UseItemNode.UseItemTargetType Enumeration

```csharp
public enum UseItemTargetType
```

Values that represent UseItemTargetType.

---

### WhileNode Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.Order.OrderNode → Styx.CommonBot.Profiles.Quest.Order.WhileNode

```csharp
public class WhileNode : OrderNode, INodeContainer
```

A while node.

#### WhileNode Properties

- **`Body Property`**
  ```csharp
  public OrderNodeCollection Body { get; }
  ```
  Gets the body.

- **`Condition Property`**
  ```csharp
  public DelayCompiledExpression&lt;Func&lt;bool&gt;&gt; Condition { get; }
  ```
  Gets the condition.

#### WhileNode Methods

- **`FromXml Method`**
  ```csharp
  public static WhileNode FromXml(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST842F8DB8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileMissingAttributeExceptionThrown when a Profile Missing Attribute
            error condition occurs.ProfileException                Thrown when a Profile error condition
            occurs.

- **`GetNodes Method`**
  ```csharp
  public IEnumerable&lt;OrderNode&gt; GetNodes()
  ```
  Gets the nodes in this collection.
  - **Returns:** See Also

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceWhileNode Class

---
