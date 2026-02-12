# Styx.Common

Contains commonly used classes.

## Contents

- [AddCompositeListOperation Class](#addcompositelistoperation-class)
- [Arguments Class](#arguments-class)
- [AsmHelper Class](#asmhelper-class)
- [AssemblyLoader(T) Class](#assemblyloadert-class)
- [Beta Class](#beta-class)
- [BoundingBox2 Structure](#boundingbox2-structure)
- [BoundingBox3 Structure](#boundingbox3-structure)
- [BoundingSphere Structure](#boundingsphere-structure)
- [CapacityQueue(T) Class](#capacityqueuet-class)
- [CircularQueue(T) Class](#circularqueuet-class)
- [CircularQueue(T).EndOfQueue Delegate](#circularqueuet.endofqueue-delegate)
- [CircularQueue(T).StartOfQueue Delegate](#circularqueuet.startofqueue-delegate)
- [CommandLine Class](#commandline-class)
- [CompositeListOperation Class](#compositelistoperation-class)
- [ContainmentType Enumeration](#containmenttype-enumeration)
- [DualHashSet(T1, T2) Class](#dualhashsett1,-t2-class)
- [Extensions Class](#extensions-class)
- [Extensions.ForEachDelegate(T) Delegate](#extensions.foreachdelegatet-delegate)
- [FileCache(T) Class](#filecachet-class)
- [FinishedMeasuringCallback Delegate](#finishedmeasuringcallback-delegate)
- [Flash Class](#flash-class)
- [FlashFlags Enumeration](#flashflags-enumeration)
- [ForcedCulture Class](#forcedculture-class)
- [HookDescription Class](#hookdescription-class)
- [HookExecutor Class](#hookexecutor-class)
- [Hotkey Class](#hotkey-class)
- [HotkeysManager Class](#hotkeysmanager-class)
- [IRangeAble Interface](#irangeable-interface)
- [IndexedList(T) Class](#indexedlistt-class)
- [InsertCompositeListOperation Class](#insertcompositelistoperation-class)
- [LineSegment Structure](#linesegment-structure)
- [LinqExtensions Class](#linqextensions-class)
- [LogLevel Enumeration](#loglevel-enumeration)
- [Logging Class](#logging-class)
- [Logging.LogMessage Class](#logging.logmessage-class)
- [Logging.LogMessageDelegate Delegate](#logging.logmessagedelegate-delegate)
- [LruCache(TKey, TValue) Class](#lrucachetkey,-tvalue-class)
- [MathEx Class](#mathex-class)
- [ModifierKeys Enumeration](#modifierkeys-enumeration)
- [PerformanceTimer Class](#performancetimer-class)
- [Range Structure](#range-structure)
- [RangedDictionary(T) Class](#rangeddictionaryt-class)
- [Ray Structure](#ray-structure)
- [ReplaceCompositeListOperation Class](#replacecompositelistoperation-class)
- [ShapeHelper Class](#shapehelper-class)
- [StyxLog Class](#styxlog-class)
- [ThreadSafeRandom Class](#threadsaferandom-class)
- [TimedRecordKeeper(T) Class](#timedrecordkeepert-class)
- [TimestampType Enumeration](#timestamptype-enumeration)
- [TreeHooks Class](#treehooks-class)
- [TypeLoader(T) Class](#typeloadert-class)
- [TypeOnlyLoader(T) Class](#typeonlyloadert-class)
- [Utilities Class](#utilities-class)
- [ValuePair(T1, T2) Structure](#valuepairt1,-t2-structure)
- [Vector3Extensions Class](#vector3extensions-class)

---

### AddCompositeListOperation Class

**Inheritance:** System.Object → Styx.Common.CompositeListOperation → Styx.Common.AddCompositeListOperation

```csharp
public class AddCompositeListOperation : CompositeListOperation
```

An add composite list operation.

#### AddCompositeListOperation Methods

- **`ApplyTo Method`**
  ```csharp
  public override void ApplyTo(
List&lt;Composite&gt; compositeList
)
  ```
  Applies to described by compositeList.
  - *compositeList*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST769FB1D5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST769FB1D5_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");CompositeAddLanguageSpecificTextSet("LST769FB1D5_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");List of composites.

---

### Arguments Class

**Inheritance:** System.Object → Styx.Common.Arguments

```csharp
public class Arguments
```

Arguments class.

#### Arguments Properties

- **`Count Property`**
  ```csharp
  public int Count { get; }
  ```
  Gets the count.

- **`Item Property`**
  ```csharp
  public Collection&lt;string&gt; this[
string parameter
] { get; }
  ```
  Gets the Collection.T with the
            specified parameter.
  - *parameter*: Type: SystemAddLanguageSpecificTextSet("LSTA020B8C5_3?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe parameter.
  - **Returns:** See Also

#### Arguments Methods

- **`Exists Method`**
  ```csharp
  public bool Exists(
string argument
)
  ```
  Exists.
  - *argument*: Type: SystemAddLanguageSpecificTextSet("LST7DE60241_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe argument.
  - **Returns:** ReferenceArguments Class

- **`IsTrue Method`**
  ```csharp
  public bool IsTrue(
string argument
)
  ```
  Determines whether the specified argument is true.
  - *argument*: Type: SystemAddLanguageSpecificTextSet("LST5CF51A05_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe argument.
  - **Returns:** See Also

- **`Single Method`**
  ```csharp
  public string Single(
string argument
)
  ```
  Singles.
  - *argument*: Type: SystemAddLanguageSpecificTextSet("LST30D20AFF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe argument.
  - **Returns:** ReferenceArguments Class

---

### AsmHelper Class

**Inheritance:** System.Object → Styx.Common.AsmHelper

```csharp
public static class AsmHelper
```

An assembly helper.

#### AsmHelper Methods

- **`GetFloatInt Method`**
  ```csharp
  public static int GetFloatInt(
float val
)
  ```
  Gets the integer representation of a float.
  - *val*: Type: SystemAddLanguageSpecificTextSet("LSTCC678906_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Single.
  - **Returns:** ReferenceAsmHelper Class

- **`GetFloatUInt Method`**
  ```csharp
  public static uint GetFloatUInt(
float val
)
  ```
  Gets float u int.
  - *val*: Type: SystemAddLanguageSpecificTextSet("LST89E9EB37_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Single.
  - **Returns:** ReferenceAsmHelper Class

---

### AssemblyLoader(T) Class

**Inheritance:** System.Object → Styx.Common.AssemblyLoader.T

```csharp
public class AssemblyLoader&lt;T&gt;
```

An assembly loader.

#### AssemblyLoader(T) Properties

- **`Instances Property`**
  ```csharp
  public List&lt;T&gt; Instances { get; }
  ```
  Gets the instances.

#### AssemblyLoader(T) Methods

- **`CreateInstances Method`**
  ```csharp
  public void CreateInstances()
  ```

- **`Reload Method`**
  ```csharp
  public void Reload(
string reason,
bool createInstances = true
)
  ```
  Reloads.
  - *reason*: Type: SystemAddLanguageSpecificTextSet("LSTA19392E1_3?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe reason.

#### AssemblyLoader(T) Events

- **`Reloaded Event`**
  ```csharp
  public event EventHandler Reloaded
  ```
  Event queue for all listeners interested in Reloaded events.

---

### Beta Class

**Inheritance:** System.Object → Styx.Common.Beta

```csharp
public static class Beta
```

#### Beta Methods

- **`Assert Method`**

- **`Assert Method (Boolean)`**
  ```csharp
  public static void Assert(
bool condition
)
  ```
  - *condition*: Type: SystemAddLanguageSpecificTextSet("LST64487988_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean

- **`Assert Method (Boolean, String)`**
  ```csharp
  public static void Assert(
bool condition,
string message
)
  ```
  - *condition*: Type: SystemAddLanguageSpecificTextSet("LST5AE629DC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST5AE629DC_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`Assert Method (Boolean, String, String)`**
  ```csharp
  public static void Assert(
bool condition,
string message,
string detailMessage
)
  ```
  - *condition*: Type: SystemAddLanguageSpecificTextSet("LSTBED7E2C8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTBED7E2C8_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *detailMessage*: Type: SystemAddLanguageSpecificTextSet("LSTBED7E2C8_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`Fail Method`**

- **`Fail Method (String)`**
  ```csharp
  public static void Fail(
string message
)
  ```
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTA9EEDA81_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`Fail Method (String, String)`**
  ```csharp
  public static void Fail(
string message,
string detailMessage
)
  ```
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST6B5E597_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *detailMessage*: Type: SystemAddLanguageSpecificTextSet("LST6B5E597_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String

---

### BoundingBox2 Structure

```csharp
public struct BoundingBox2
```

Initializes a new instance of the BoundingBox2 class

#### BoundingBox2 Methods

- **`Contains Method`**
  ```csharp
  public bool Contains(
Vector2 v
)
  ```
  - *v*: Type: System.NumericsAddLanguageSpecificTextSet("LST87F182BB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector2
  - **Returns:** ReferenceBoundingBox2 Structure

- **`CreateFromPoints Method`**
  ```csharp
  public static BoundingBox2 CreateFromPoints(
IEnumerable&lt;Vector2&gt; points
)
  ```
  - *points*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST8D0B72BF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST8D0B72BF_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector2AddLanguageSpecificTextSet("LST8D0B72BF_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - **Returns:** ReferenceBoundingBox2 Structure

- **`Normalize Method`**
  ```csharp
  public void Normalize()
  ```
  Swaps the components in the Min and Max vectors so the 'Min' is the smallest and 'Max' is the biggest.
            This changes Min/Max, but keeps the box intact.

#### BoundingBox2 Fields

- **`Max Field`**
  ```csharp
  public Vector2 Max
  ```

- **`Min Field`**
  ```csharp
  public Vector2 Min
  ```

---

### BoundingBox3 Structure

```csharp
public struct BoundingBox3
```

Initializes a new instance of the BoundingBox3 class

#### BoundingBox3 Methods

- **`Contains Method`**

- **`Contains Method (Vector3)`**
  ```csharp
  public bool Contains(
Vector3 v
)
  ```
  - *v*: Type: System.NumericsAddLanguageSpecificTextSet("LST7BFA5ABF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - **Returns:** ReferenceBoundingBox3 Structure

- **`Contains Method (BoundingBox3)`**
  ```csharp
  public ContainmentType Contains(
BoundingBox3 box
)
  ```
  - *box*: Type: Styx.CommonAddLanguageSpecificTextSet("LST18BD4557_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BoundingBox3
  - **Returns:** ReferenceBoundingBox3 Structure

- **`Contains Method (BoundingBox3, BoundingBox3)`**
  ```csharp
  public static ContainmentType Contains(
ref BoundingBox3 box1,
ref BoundingBox3 box2
)
  ```
  - *box1*: Type: Styx.CommonAddLanguageSpecificTextSet("LST7E3557C8_3?cs=.|vb=.|cpp=::|nu=.|fs=.");BoundingBox3AddLanguageSpecificTextSet("LST7E3557C8_4?cpp=%");
  - *box2*: Type: Styx.CommonAddLanguageSpecificTextSet("LST7E3557C8_5?cs=.|vb=.|cpp=::|nu=.|fs=.");BoundingBox3AddLanguageSpecificTextSet("LST7E3557C8_6?cpp=%");
  - **Returns:** ReferenceBoundingBox3 Structure

- **`CreateFromPoints Method`**
  ```csharp
  public static BoundingBox3 CreateFromPoints(
IEnumerable&lt;Vector3&gt; points
)
  ```
  - *points*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST6AB05CBF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST6AB05CBF_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST6AB05CBF_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - **Returns:** ReferenceBoundingBox3 Structure

- **`Merge Method`**

- **`Merge Method (BoundingBox3, BoundingBox3)`**
  ```csharp
  public static BoundingBox3 Merge(
BoundingBox3 bb1,
BoundingBox3 bb2
)
  ```
  - *bb1*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTDD938DF3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BoundingBox3
  - *bb2*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTDD938DF3_2?cs=.|vb=.|cpp=::|nu=.|fs=.");BoundingBox3
  - **Returns:** ReferenceBoundingBox3 Structure

- **`Merge Method (BoundingBox3, BoundingBox3, BoundingBox3)`**
  ```csharp
  public static void Merge(
ref BoundingBox3 bb1,
ref BoundingBox3 bb2,
out BoundingBox3 result
)
  ```
  - *bb1*: Type: Styx.CommonAddLanguageSpecificTextSet("LST9FEB4B9C_4?cs=.|vb=.|cpp=::|nu=.|fs=.");BoundingBox3AddLanguageSpecificTextSet("LST9FEB4B9C_5?cpp=%");
  - *bb2*: Type: Styx.CommonAddLanguageSpecificTextSet("LST9FEB4B9C_6?cs=.|vb=.|cpp=::|nu=.|fs=.");BoundingBox3AddLanguageSpecificTextSet("LST9FEB4B9C_7?cpp=%");
  - *result*: Type: Styx.CommonAddLanguageSpecificTextSet("LST9FEB4B9C_8?cs=.|vb=.|cpp=::|nu=.|fs=.");BoundingBox3AddLanguageSpecificTextSet("LST9FEB4B9C_9?cpp=%");

- **`Normalize Method`**
  ```csharp
  public void Normalize()
  ```
  Swaps the components in the Min and Max vectors so the 'Min' is the smallest and 'Max' is the biggest.
            This changes Min/Max, but keeps the box intact.

#### BoundingBox3 Fields

- **`Max Field`**
  ```csharp
  public Vector3 Max
  ```

- **`Min Field`**
  ```csharp
  public Vector3 Min
  ```

---

### BoundingSphere Structure

```csharp
public struct BoundingSphere
```

A sphere.

#### BoundingSphere Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns the fully qualified type name of this instance.
  - **Returns:** ReferenceBoundingSphere Structure

#### BoundingSphere Fields

- **`Center Field`**
  ```csharp
  public Vector3 Center
  ```
  The center.

- **`Radius Field`**
  ```csharp
  public float Radius
  ```
  The radius.

---

### CapacityQueue(T) Class

**Inheritance:** System.Object → System.Collections.Generic.Queue.T → Styx.Common.CapacityQueue.T

```csharp
public class CapacityQueue&lt;T&gt; : Queue&lt;T&gt;
```

Queue of capacities.

#### CapacityQueue(T) Properties

- **`Capacity Property`**
  ```csharp
  public int Capacity { get; }
  ```
  Gets the capacity.

#### CapacityQueue(T) Methods

- **`Enqueue Method`**
  ```csharp
  public void Enqueue(
T item
)
  ```
  Adds an object onto the end of this queue.
  - *item*: Type: TThe item.

---

### CircularQueue(T) Class

**Inheritance:** System.Object → System.Collections.Generic.Queue.T → Styx.Common.CircularQueue.T

```csharp
public class CircularQueue&lt;T&gt; : Queue&lt;T&gt;
```

Queue of circulars.

#### CircularQueue(T) Constructor

- **`CircularQueue(T) Constructor`**
  ```csharp
  public CircularQueue()
  ```
  Initializes a new instance of the CircularQueue.T class

- **`CircularQueue(T) Constructor (IEnumerable(T))`**
  ```csharp
  public CircularQueue(
IEnumerable&lt;T&gt; collection
)
  ```
  Initializes a new instance of the CircularQueue.T class
  - *collection*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTBB0B2541_6?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTBB0B2541_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTBB0B2541_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

#### CircularQueue(T) Properties

- **`First Property`**
  ```csharp
  public T First { get; }
  ```
  Gets the first.

#### CircularQueue(T) Methods

- **`Add Method`**
  ```csharp
  public void Add(
T item
)
  ```
  Enqueues the item. Needed for constructor.
  - *item*: Type: T.

- **`CycleTo Method`**
  ```csharp
  public void CycleTo(
T item
)
  ```
  Cycle to.
  - *item*: Type: TThe item.

- **`Dequeue Method`**
  ```csharp
  public T Dequeue()
  ```
  Removes the head object from this queue.
  - **Returns:** See Also

- **`Enqueue Method`**
  ```csharp
  public void Enqueue(
T item
)
  ```
  Adds an object onto the end of this queue.
  - *item*: Type: TThe item.

#### CircularQueue(T) Events

- **`OnEndOfQueue Event`**
  ```csharp
  public event CircularQueue.T.EndOfQueue OnEndOfQueue
  ```
  Event queue for all listeners interested in OnEndOfQueue events.

- **`OnStartOfQueue Event`**
  ```csharp
  public event CircularQueue.T.StartOfQueue OnStartOfQueue
  ```
  Event queue for all listeners interested in OnStartOfQueue events.

---

### CircularQueue(T).EndOfQueue Delegate

```csharp
public delegate void EndOfQueue(
Object sender,
EventArgs e
)
```

Ends of queue.

---

### CircularQueue(T).StartOfQueue Delegate

```csharp
public delegate void StartOfQueue(
Object sender,
EventArgs e
)
```

Starts of queue.

---

### CommandLine Class

**Inheritance:** System.Object → Styx.Common.CommandLine

```csharp
public static class CommandLine
```

A command line.

#### CommandLine Properties

- **`Arguments Property`**
  ```csharp
  public static Arguments Arguments { get; }
  ```
  Gets the arguments.

---

### CompositeListOperation Class

**Inheritance:** System.Object → Styx.Common.CompositeListOperation → Styx.Common.AddCompositeListOperation → Styx.Common.InsertCompositeListOperation → Styx.Common.ReplaceCompositeListOperation

```csharp
public abstract class CompositeListOperation
```

A composite list operation.

#### CompositeListOperation Properties

- **`Composite Property`**
  ```csharp
  public Composite Composite { get; }
  ```
  Gets the composite.

#### CompositeListOperation Methods

- **`ApplyTo Method`**
  ```csharp
  public abstract void ApplyTo(
List&lt;Composite&gt; compositeList
)
  ```
  Applies to described by compositeList.
  - *compositeList*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST9E99AA54_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST9E99AA54_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");CompositeAddLanguageSpecificTextSet("LST9E99AA54_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");List of composites.

---

### ContainmentType Enumeration

```csharp
public enum ContainmentType
```

---

### DualHashSet(T1, T2) Class

**Inheritance:** System.Object → Styx.Common.DualHashSet.T1, T2

```csharp
public class DualHashSet&lt;T1, T2&gt; : IEnumerable&lt;Object&gt;,
IEnumerable
```

A dual hash set.

#### DualHashSet(T1, T2) Properties

- **`Count Property`**
  ```csharp
  public int Count { get; }
  ```
  Gets the number of.

- **`HashSet1 Property`**
  ```csharp
  public HashSet&lt;T1&gt; HashSet1 { get; }
  ```
  Gets the hash set 1.

- **`HashSet2 Property`**
  ```csharp
  public HashSet&lt;T2&gt; HashSet2 { get; }
  ```
  Gets the hash set 2.

#### DualHashSet(T1, T2) Methods

- **`Add Method`**
  Adds item.

- **`Add Method (T1)`**
  ```csharp
  public bool Add(
T1 item
)
  ```
  Adds item.
  - *item*: Type: T1The T2 to test for containment.
  - **Returns:** ReferenceDualHashSetAddLanguageSpecificTextSet("LST49B4EA2E_3?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");T1, T2AddLanguageSpecificTextSet("LST49B4EA2E_4?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

- **`Add Method (T2)`**
  ```csharp
  public bool Add(
T2 item
)
  ```
  Adds item.
  - *item*: Type: T2The T2 to test for containment.
  - **Returns:** ReferenceDualHashSetAddLanguageSpecificTextSet("LSTEC5C5EC9_3?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");T1, T2AddLanguageSpecificTextSet("LSTEC5C5EC9_4?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

- **`Add Method (ValuePair(T1, T2))`**
  ```csharp
  public void Add(
ValuePair&lt;T1, T2&gt; item
)
  ```
  Adds item.
  - *item*: Type: Styx.CommonAddLanguageSpecificTextSet("LST2EAC6B3D_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ValuePairAddLanguageSpecificTextSet("LST2EAC6B3D_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T1, T2AddLanguageSpecificTextSet("LST2EAC6B3D_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The T2 to test for containment.

- **`Contains Method`**
  Query if this object contains the given vp.

- **`Contains Method (T1)`**
  ```csharp
  public bool Contains(
T1 item
)
  ```
  Query if this object contains the given vp.
  - *item*: Type: T1The T2 to test for containment.
  - **Returns:** ReferenceDualHashSetAddLanguageSpecificTextSet("LST7CAA325C_3?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");T1, T2AddLanguageSpecificTextSet("LST7CAA325C_4?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

- **`Contains Method (T2)`**
  ```csharp
  public bool Contains(
T2 item
)
  ```
  Query if this object contains the given vp.
  - *item*: Type: T2The T2 to test for containment.
  - **Returns:** ReferenceDualHashSetAddLanguageSpecificTextSet("LST7CAA325B_3?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");T1, T2AddLanguageSpecificTextSet("LST7CAA325B_4?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

- **`Contains Method (ValuePair(T1, T2))`**
  ```csharp
  public bool Contains(
ValuePair&lt;T1, T2&gt; vp
)
  ```
  Query if this object contains the given vp.
  - *vp*: Type: Styx.CommonAddLanguageSpecificTextSet("LST34722E47_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ValuePairAddLanguageSpecificTextSet("LST34722E47_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T1, T2AddLanguageSpecificTextSet("LST34722E47_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The ValuePair&lt;T1,T2&gt; to test for containment.
  - **Returns:** ReferenceDualHashSetAddLanguageSpecificTextSet("LST34722E47_8?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");T1, T2AddLanguageSpecificTextSet("LST34722E47_9?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

- **`GetEnumerator Method`**
  ```csharp
  public IEnumerator&lt;Object&gt; GetEnumerator()
  ```
  Gets the enumerator.
  - **Returns:** See Also

- **`Remove Method`**
  Removes the given item.

- **`Remove Method (T1)`**
  ```csharp
  public bool Remove(
T1 item
)
  ```
  Removes the given item.
  - *item*: Type: T1The T2 to test for containment.
  - **Returns:** ReferenceDualHashSetAddLanguageSpecificTextSet("LST1A474B5F_3?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");T1, T2AddLanguageSpecificTextSet("LST1A474B5F_4?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

- **`Remove Method (T2)`**
  ```csharp
  public bool Remove(
T2 item
)
  ```
  Removes the given item.
  - *item*: Type: T2The T2 to test for containment.
  - **Returns:** ReferenceDualHashSetAddLanguageSpecificTextSet("LST1A474B60_3?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");T1, T2AddLanguageSpecificTextSet("LST1A474B60_4?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### Extensions Class

**Inheritance:** System.Object → Styx.Common.Extensions

```csharp
public static class Extensions
```

An extensions.

#### Extensions Methods

- **`ForEach(T) Method`**
  ```csharp
  public static void ForEach&lt;T&gt;(
this IEnumerable&lt;T&gt; e,
Extensions.ForEachDelegate&lt;T&gt; do
)
  ```
  An IEnumerable&lt;T&gt; extension method that applies an operation to all items in
            this collection.
  - *e*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST53EACB9E_4?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST53EACB9E_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST53EACB9E_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The e to act on.
  - *do*: Type: Styx.CommonAddLanguageSpecificTextSet("LST53EACB9E_7?cs=.|vb=.|cpp=::|nu=.|fs=.");ExtensionsAddLanguageSpecificTextSet("LST53EACB9E_8?cs=.|vb=.|cpp=::|nu=.|fs=.");ForEachDelegateAddLanguageSpecificTextSet("LST53EACB9E_9?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST53EACB9E_10?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The do.
  - *T*: Generic type parameter.

- **`IndexOf(T) Method`**
  ```csharp
  public static int IndexOf&lt;T&gt;(
this T[] bytes,
T value
)
  ```
  A T[] extension method that searches for the first match.
  - *bytes*: Type: AddLanguageSpecificTextSet("LSTFAB76B75_3?cpp=array&lt;");TAddLanguageSpecificTextSet("LSTFAB76B75_4?cpp=&gt;|vb=()|nu=[]");The bytes to act on.
  - *value*: Type: TThe value.
  - *T*: Generic type parameter.
  - **Returns:** ReferenceExtensions Class

- **`Like Method`**
  ```csharp
  public static bool Like(
this string str,
string wildcard
)
  ```
  Compares the string against a given pattern.
  - *str*: Type: SystemAddLanguageSpecificTextSet("LST463E7EE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String     The string.
  - *wildcard*: Type: SystemAddLanguageSpecificTextSet("LST463E7EE_2?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe wildcard, where "*" means any sequence of characters, and "?"
            means any single character.
  - **Returns:** See Also

- **`ToArray(T) Method`**
  ```csharp
  public static byte[] ToArray&lt;T&gt;(
byte* ptr,
int length
)
  ```
  Convert this object into an array representation.
  - *ptr*: Type: SystemAddLanguageSpecificTextSet("LST269EBC76_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Byte*   [in,out] If non-null, the pointer.
  - *length*: Type: SystemAddLanguageSpecificTextSet("LST269EBC76_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The length.
  - *T*: Generic type parameter.
  - **Returns:** See Also

- **`ToPointer Method`**
  ```csharp
  public static double** ToPointer(
this double[][] array
)
  ```
  A double[][] extension method that converts an array to a pointer.
  - *array*: Type: AddLanguageSpecificTextSet("LSTD82BA9BF_1?cpp=array&lt;");AddLanguageSpecificTextSet("LSTD82BA9BF_2?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTD82BA9BF_3?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleAddLanguageSpecificTextSet("LSTD82BA9BF_4?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LSTD82BA9BF_5?cpp=&gt;|vb=()|nu=[]");The array to act on.
  - **Returns:** ReferenceExtensions Class

---

### Extensions.ForEachDelegate(T) Delegate

```csharp
public delegate void ForEachDelegate&lt;T&gt;(
T item
)
```

throw new Exception("String is not null terminated!");
            }

---

### FileCache(T) Class

**Inheritance:** System.Object → Styx.Common.FileCache.T

```csharp
public class FileCache&lt;T&gt; : IList&lt;T&gt;,
ICollection&lt;T&gt;, IEnumerable&lt;T&gt;, IEnumerable, IDisposable
where T : struct, new()
```

Acts as a file based cache implementing List&lt;T&gt;. Use in place of a typical List
            to gain the advantage of file storage for persistent data.

#### FileCache(T) Properties

- **`CacheFilePath Property`**
  ```csharp
  public string CacheFilePath { get; }
  ```
  Gets the full pathname of the cache file.

- **`Count Property`**
  ```csharp
  public int Count { get; }
  ```
  Gets the number of elements contained in the
            ICollection.T.

- **`Disposed Property`**
  ```csharp
  protected bool Disposed { get; }
  ```

- **`IsReadOnly Property`**
  ```csharp
  public bool IsReadOnly { get; }
  ```
  Gets a value indicating whether the
            ICollection.T is read-only.

#### FileCache(T) Methods

- **`Add Method`**
  Adds value.

- **`Add Method (T)`**
  ```csharp
  public void Add(
T value
)
  ```
  Adds value.
  - *value*: Type: TThe value.

- **`Add Method (T, Boolean)`**
  ```csharp
  public void Add(
T value,
bool isTemporary
)
  ```
  Adds value.
  - *value*: Type: T      The value.
  - *isTemporary*: Type: SystemAddLanguageSpecificTextSet("LST88C927A8_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Booleantrue if this object is temporary.

- **`AsReadOnly Method`**
  ```csharp
  public ReadOnlyCollection&lt;T&gt; AsReadOnly()
  ```
  Converts this object to a read only.
  - **Returns:** See Also

- **`Clear Method`**
  ```csharp
  public void Clear()
  ```
  Removes all items from the ICollection.T.

- **`Contains Method`**
  ```csharp
  public bool Contains(
T item
)
  ```
  Determines whether the ICollection.T
            contains a specific value.
  - *item*: Type: TThe object to locate in the
            ICollectionAddLanguageSpecificTextSet("LST990DD17F_5?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST990DD17F_6?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;");.
  - **Returns:** See Also

- **`CopyTo Method`**
  ```csharp
  public void CopyTo(
T[] array,
int arrayIndex
)
  ```
  Copies the elements of the ICollection.T
            to an Array, starting at a particular Array
            index.
  - *array*: Type: AddLanguageSpecificTextSet("LST511850B1_5?cpp=array&lt;");TAddLanguageSpecificTextSet("LST511850B1_6?cpp=&gt;|vb=()|nu=[]");     The one-dimensional Array that is the
            destination of the elements copied from
            ICollectionAddLanguageSpecificTextSet("LST511850B1_7?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST511850B1_8?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;");. The Array
            must have zero-based indexing.
  - *arrayIndex*: Type: SystemAddLanguageSpecificTextSet("LST511850B1_9?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32Zero-based index of the array.

- **`Dispose Method`**
  ```csharp
  public void Dispose()
  ```
  Performs application-defined tasks associated with freeing, releasing, or resetting
            unmanaged resources.

- **`Finalize Method`**
  ```csharp
  protected override void Finalize()
  ```
  Finaliser.

- **`GetEnumerator Method`**
  ```csharp
  public IEnumerator&lt;T&gt; GetEnumerator()
  ```
  Returns an enumerator that iterates through the collection.
  - **Returns:** See Also

- **`IndexOf Method`**
  ```csharp
  public int IndexOf(
T item
)
  ```
  Determines the index of a specific item in the
            IList.T.
  - *item*: Type: TThe object to locate in the
            IListAddLanguageSpecificTextSet("LST4617B3FB_5?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST4617B3FB_6?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;");.
  - **Returns:** See Also

- **`Insert Method`**
  ```csharp
  public void Insert(
int index,
T item
)
  ```
  Inserts an item to the IList.T at the
            specified index.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST2C9D3C22_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based index at which item should be inserted.
  - *item*: Type: T The item.

- **`LoadFile Method`**
  ```csharp
  protected void LoadFile()
  ```

- **`Read Method`**
  ```csharp
  protected T Read(
BinaryReader reader
)
  ```
  - *reader*: Type: System.IOAddLanguageSpecificTextSet("LST86B4231F_3?cs=.|vb=.|cpp=::|nu=.|fs=.");BinaryReader
  - **Returns:** See Also

- **`Remove Method`**
  ```csharp
  public bool Remove(
T item
)
  ```
  Removes the first occurrence of a specific object from the
            ICollection.T.
  - *item*: Type: TThe object to remove from the
            ICollectionAddLanguageSpecificTextSet("LST544E0A24_5?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST544E0A24_6?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;");.
  - **Returns:** See Also

- **`RemoveAt Method`**
  ```csharp
  public void RemoveAt(
int index
)
  ```
  Removes the IList.T item at the specified
            index.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTD74BE730_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based index of the item to remove.

- **`SaveFile Method`**
  ```csharp
  public void SaveFile()
  ```
  Saves the file.

- **`Write Method`**
  ```csharp
  protected void Write(
BinaryWriter writer,
T value
)
  ```
  - *writer*: Type: System.IOAddLanguageSpecificTextSet("LST3B884AB2_3?cs=.|vb=.|cpp=::|nu=.|fs=.");BinaryWriter
  - *value*: Type: T

#### FileCache(T) Fields

- **`StructureSize Field`**
  ```csharp
  protected readonly int StructureSize
  ```

---

### FinishedMeasuringCallback Delegate

```csharp
public delegate void FinishedMeasuringCallback(
long time
)
```

Callback, called when the finished measuring.

---

### Flash Class

**Inheritance:** System.Object → Styx.Common.Flash

```csharp
public static class Flash
```

A flash.

#### Flash Methods

- **`FlashWindow Method`**
  ```csharp
  public static bool FlashWindow(
IntPtr hwnd,
FlashFlags flags,
int timeout = 0,
int count = 2147483647
)
  ```
  Flashes a window.
  - *hwnd*: Type: SystemAddLanguageSpecificTextSet("LST8C53CA71_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IntPtr   The handle to the window.
  - *flags*: Type: Styx.CommonAddLanguageSpecificTextSet("LST8C53CA71_2?cs=.|vb=.|cpp=::|nu=.|fs=.");FlashFlags  The FlashFlags flags..
  - **Returns:** ReferenceFlash Class

---

### FlashFlags Enumeration

```csharp
[FlagsAttribute]
public enum FlashFlags
```

Bitfield of flags for specifying FlashFlags.

---

### ForcedCulture Class

**Inheritance:** System.Object → Styx.Common.ForcedCulture

```csharp
public class ForcedCulture : IDisposable
```

A forced culture.

#### ForcedCulture Methods

- **`Dispose Method`**
  ```csharp
  public void Dispose()
  ```
  Performs application-defined tasks associated with freeing, releasing, or resetting
            unmanaged resources.

---

### HookDescription Class

**Inheritance:** System.Object → Styx.Common.HookDescription

```csharp
public class HookDescription
```

Description of the hook.

#### HookDescription Properties

- **`Description Property`**
  ```csharp
  public string Description { get; set; }
  ```
  Gets or sets the description.

- **`Name Property`**
  ```csharp
  public string Name { get; set; }
  ```
  Gets or sets the name.

#### HookDescription Methods

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
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LST1DEC15EB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe Object to compare with the current
            Object.
  - **Returns:** ReferenceHookDescription Class

- **`Equals Method (HookDescription)`**
  ```csharp
  public bool Equals(
HookDescription other
)
  ```
  Determines whether the specified Object is equal to the
            current Object.
  - *other*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTEC504187_1?cs=.|vb=.|cpp=::|nu=.|fs=.");HookDescriptionThe hook description to compare to this object.
  - **Returns:** ReferenceHookDescription Class

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  Serves as a hash function for a particular type.
  - **Returns:** ReferenceHookDescription Class

---

### HookExecutor Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → Styx.Common.HookExecutor

```csharp
public class HookExecutor : Action
```

A simple Action composite, to facilitate executing TreeHook composites. It is not
            recommended you use this for any built-in hooks. Only use this if using a custom hook
            location!

#### HookExecutor Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST328D6186_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceHookExecutor Class

- **`Start Method`**
  ```csharp
  public override void Start(
Object context
)
  ```
  Initializes the Task into a runnable state by clearing the LastStatus (if any),
            clearing the cleanup handlers remaining (if any -- note that this may also trigger the
            IDisposable behavior of any remaining cleanup handlers on the cleanup stack), and
            reinitializing the execution enumerator.
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST820E93F9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe context.

- **`Stop Method`**
  ```csharp
  public override void Stop(
Object context
)
  ```
  Stop will call all cleanup handlers!
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST2AC922A5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe context.

---

### Hotkey Class

**Inheritance:** System.Object → Styx.Common.Hotkey

```csharp
public class Hotkey : IEquatable&lt;Hotkey&gt;
```

A hotkey.

#### Hotkey Properties

- **`Callback Property`**
  ```csharp
  public Action&lt;Hotkey&gt; Callback { get; set; }
  ```
  Gets or sets the callback.

- **`Id Property`**
  ```csharp
  public int Id { get; }
  ```
  Gets the identifier.

- **`Key Property`**
  ```csharp
  public Keys Key { get; }
  ```
  Gets the key.

- **`ModifierKeys Property`**
  ```csharp
  public ModifierKeys ModifierKeys { get; }
  ```
  Gets the modifier keys.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

#### Hotkey Methods

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
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LSTDFCB296E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe Object to compare with the current
            Object.
  - **Returns:** ReferenceHotkey Class

- **`Equals Method (Hotkey)`**
  ```csharp
  public bool Equals(
Hotkey other
)
  ```
  Tests if this Hotkey is considered equal to another.
  - *other*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTF6F93663_1?cs=.|vb=.|cpp=::|nu=.|fs=.");HotkeyThe hotkey to compare to this object.
  - **Returns:** See Also

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  Serves as a hash function for a particular type.
  - **Returns:** ReferenceHotkey Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceHotkey Class

#### Hotkey Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
Hotkey left,
Hotkey right
)
  ```
  Equality operator.
  - *left*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTBA1DDB5B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Hotkey The left.
  - *right*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTBA1DDB5B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");HotkeyThe right.
  - **Returns:** ReferenceHotkey Class

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
Hotkey left,
Hotkey right
)
  ```
  Inequality operator.
  - *left*: Type: Styx.CommonAddLanguageSpecificTextSet("LST5E1520D4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Hotkey The left.
  - *right*: Type: Styx.CommonAddLanguageSpecificTextSet("LST5E1520D4_2?cs=.|vb=.|cpp=::|nu=.|fs=.");HotkeyThe right.
  - **Returns:** ReferenceHotkey Class

---

### HotkeysManager Class

**Inheritance:** System.Object → Styx.Common.HotkeysManager

```csharp
public static class HotkeysManager
```

Manager for hotkeys.

#### HotkeysManager Properties

- **`Hotkeys Property`**
  ```csharp
  public static IEnumerable&lt;Hotkey&gt; Hotkeys { get; }
  ```
  Gets the hotkeys.

#### HotkeysManager Methods

- **`Initialize Method`**
  ```csharp
  public static void Initialize(
IntPtr windowHandleToWatch
)
  ```
  Initializes the HotkeysManager with the specified window handle as the currently
            "watched" window. Only hotkey events raised while this window is focused will be processed.
  - *windowHandleToWatch*: Type: SystemAddLanguageSpecificTextSet("LSTC6729031_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IntPtr.

- **`Register Method`**
  ```csharp
  public static Hotkey Register(
string name,
Keys key,
ModifierKeys modifierKeys,
Action&lt;Hotkey&gt; callback
)
  ```
  Registers a hotkey for the current game.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LST32A98BDD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String        The name of the hotkey. For use in diagnostics, and display.
  - *key*: Type: System.Windows.FormsAddLanguageSpecificTextSet("LST32A98BDD_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Keys         The main key used for the hotkey.
  - *modifierKeys*: Type: Styx.CommonAddLanguageSpecificTextSet("LST32A98BDD_3?cs=.|vb=.|cpp=::|nu=.|fs=.");ModifierKeysThe modifiers for the hotkey. This is required, as a non-modifier
            hotkey will cause typing issues for users.
  - *callback*: Type: SystemAddLanguageSpecificTextSet("LST32A98BDD_4?cs=.|vb=.|cpp=::|nu=.|fs=.");ActionAddLanguageSpecificTextSet("LST32A98BDD_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");HotkeyAddLanguageSpecificTextSet("LST32A98BDD_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");    The callback function for when the hotkey is pressed.
  - **Returns:** ReferenceHotkeysManager Class

- **`Unregister Method`**
  Removes a hotkey by name.

- **`Unregister Method (String)`**
  ```csharp
  public static void Unregister(
string name
)
  ```
  Removes a hotkey by name.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LSTB974FBF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`Unregister Method (Hotkey)`**
  ```csharp
  public static void Unregister(
Hotkey hotkey
)
  ```
  Removes a hotkey by it's instance.
  - *hotkey*: Type: Styx.CommonAddLanguageSpecificTextSet("LST229AF3D0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Hotkey.

---

### IRangeAble Interface

```csharp
public interface IRangeAble
```

Interface for range able.

#### IRangeAble Methods

- **`GetRange Method`**
  ```csharp
  Range GetRange()
  ```
  Gets the range.
  - **Returns:** ReferenceIRangeAble Interface

---

### IndexedList(T) Class

**Inheritance:** System.Object → System.Collections.Generic.List.T → Styx.Common.IndexedList.T

```csharp
public class IndexedList&lt;T&gt; : List&lt;T&gt;
```

List of indexed.

#### IndexedList(T) Constructor

- **`IndexedList(T) Constructor (Boolean)`**
  ```csharp
  public IndexedList(
bool isCyclic = false
)
  ```
  Constructor.

- **`IndexedList(T) Constructor (IEnumerable(T), Boolean)`**
  ```csharp
  public IndexedList(
IEnumerable&lt;T&gt; collection,
bool isCyclic = false
)
  ```
  Constructor.
  - *collection*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTBAFBF77F_4?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTBAFBF77F_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTBAFBF77F_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The collection.

- **`IndexedList(T) Constructor (Int32, Boolean)`**
  ```csharp
  public IndexedList(
int capacity,
bool isCyclic = false
)
  ```
  Constructor.
  - *capacity*: Type: SystemAddLanguageSpecificTextSet("LST3534816C_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The capacity.

#### IndexedList(T) Properties

- **`Current Property`**
  ```csharp
  public T Current { get; }
  ```
  Gets the current.

- **`CurrentOrDefault Property`**
  ```csharp
  public T CurrentOrDefault { get; }
  ```
  Gets the current or default.

- **`Index Property`**
  ```csharp
  public int Index { get; set; }
  ```
  Gets or sets the index. An exception is not thrown if the index is invalid, but
            rather Index is updated according to IsCyclic. If IsCyclic is
            true, index is cycled into a valid range. If it is false, it is clamped into a valid range.

- **`IsCyclic Property`**
  ```csharp
  public bool IsCyclic { get; set; }
  ```
  Gets or sets a value indicating whether this object is cyclic.

#### IndexedList(T) Methods

- **`Next Method`**
  ```csharp
  public bool Next()
  ```
  Advances to the next item in the list.
  - **Returns:** ReferenceIndexedListAddLanguageSpecificTextSet("LST514E0765_3?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST514E0765_4?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

- **`Previous Method`**
  ```csharp
  public bool Previous()
  ```
  Returns to the previous item in the list.
  - **Returns:** ReferenceIndexedListAddLanguageSpecificTextSet("LST79678C1B_3?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST79678C1B_4?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### InsertCompositeListOperation Class

**Inheritance:** System.Object → Styx.Common.CompositeListOperation → Styx.Common.InsertCompositeListOperation

```csharp
public class InsertCompositeListOperation : CompositeListOperation
```

An insert composite list operation.

#### InsertCompositeListOperation Properties

- **`Index Property`**
  ```csharp
  public int Index { get; }
  ```
  Gets the zero-based index of this object.

#### InsertCompositeListOperation Methods

- **`ApplyTo Method`**
  ```csharp
  public override void ApplyTo(
List&lt;Composite&gt; compositeList
)
  ```
  Applies to described by compositeList.
  - *compositeList*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST257F4D53_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST257F4D53_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");CompositeAddLanguageSpecificTextSet("LST257F4D53_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");List of composites.

---

### LineSegment Structure

```csharp
public struct LineSegment
```

A line segment.

#### LineSegment Properties

- **`Direction Property`**
  ```csharp
  public Vector3 Direction { get; }
  ```
  Gets the direction.

- **`Distance Property`**
  ```csharp
  public float Distance { get; }
  ```
  Gets the distance.

- **`DistanceSqr Property`**
  ```csharp
  public float DistanceSqr { get; }
  ```
  Gets the distance sqr.

- **`UnitDirection Property`**
  ```csharp
  public Vector3 UnitDirection { get; }
  ```
  Gets the unit direction.

#### LineSegment Fields

- **`End Field`**
  ```csharp
  public Vector3 End
  ```
  The end.

- **`Start Field`**
  ```csharp
  public Vector3 Start
  ```
  The start.

---

### LinqExtensions Class

**Inheritance:** System.Object → Styx.Common.LinqExtensions

```csharp
public static class LinqExtensions
```

A linq extensions.

#### LinqExtensions Methods

- **`MaxBy(TSource, TKey) Method`**
  ```csharp
  public static TSource MaxBy&lt;TSource, TKey&gt;(
this IEnumerable&lt;TSource&gt; source,
Func&lt;TSource, TKey&gt; keySelector
)
  ```
  Finds the maximum TSource in source based on a key.
  - *source*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTFD8FA1D8_3?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTFD8FA1D8_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TSourceAddLanguageSpecificTextSet("LSTFD8FA1D8_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The source.
  - *keySelector*: Type: SystemAddLanguageSpecificTextSet("LSTFD8FA1D8_6?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTFD8FA1D8_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TSource, TKeyAddLanguageSpecificTextSet("LSTFD8FA1D8_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");A delegate that extracts a key from an item.
  - *TSource*: The source parameter type.
  - *TKey*: The key type to compare the source on.
  - **Returns:** Exceptions

- **`MaxByOrDefault(TSource, TKey) Method`**
  ```csharp
  public static TSource MaxByOrDefault&lt;TSource, TKey&gt;(
this IEnumerable&lt;TSource&gt; source,
Func&lt;TSource, TKey&gt; keySelector
)
  ```
  Finds the maximum TSource in source based on a key.
  - *source*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST191C75B4_3?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST191C75B4_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TSourceAddLanguageSpecificTextSet("LST191C75B4_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The source.
  - *keySelector*: Type: SystemAddLanguageSpecificTextSet("LST191C75B4_6?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST191C75B4_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TSource, TKeyAddLanguageSpecificTextSet("LST191C75B4_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");A delegate that extracts a key from an item.
  - *TSource*: The source parameter type.
  - *TKey*: The key type to compare the source on.
  - **Returns:** Exceptions

- **`MinBy(TSource, TKey) Method`**
  ```csharp
  public static TSource MinBy&lt;TSource, TKey&gt;(
this IEnumerable&lt;TSource&gt; source,
Func&lt;TSource, TKey&gt; keySelector
)
  ```
  Finds the minimum TSource in source based on a key.
  - *source*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST4C8EF5BE_3?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST4C8EF5BE_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TSourceAddLanguageSpecificTextSet("LST4C8EF5BE_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The source.
  - *keySelector*: Type: SystemAddLanguageSpecificTextSet("LST4C8EF5BE_6?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST4C8EF5BE_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TSource, TKeyAddLanguageSpecificTextSet("LST4C8EF5BE_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");A delegate that extracts a key from an item.
  - *TSource*: The source parameter type.
  - *TKey*: The key type to compare the source on.
  - **Returns:** Exceptions

- **`MinByOrDefault(TSource, TKey) Method`**
  ```csharp
  public static TSource MinByOrDefault&lt;TSource, TKey&gt;(
this IEnumerable&lt;TSource&gt; source,
Func&lt;TSource, TKey&gt; keySelector
)
  ```
  Finds the minimum TSource in source based on a key.
  - *source*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTECA44F92_3?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTECA44F92_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TSourceAddLanguageSpecificTextSet("LSTECA44F92_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The source.
  - *keySelector*: Type: SystemAddLanguageSpecificTextSet("LSTECA44F92_6?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTECA44F92_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TSource, TKeyAddLanguageSpecificTextSet("LSTECA44F92_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");A delegate that extracts a key from an item.
  - *TSource*: The source parameter type.
  - *TKey*: The key type to compare the source on.
  - **Returns:** Exceptions

- **`None(TSource) Method`**
  ```csharp
  public static bool None&lt;TSource&gt;(
this IEnumerable&lt;TSource&gt; source,
Func&lt;TSource, bool&gt; predicate
)
  ```
  Checks if None of the elements in the IEnumerable match the specified predicate.
  - *source*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST2A5EC1D8_3?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST2A5EC1D8_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TSourceAddLanguageSpecificTextSet("LST2A5EC1D8_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");   The source.
  - *predicate*: Type: SystemAddLanguageSpecificTextSet("LST2A5EC1D8_6?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST2A5EC1D8_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TSource, BooleanAddLanguageSpecificTextSet("LST2A5EC1D8_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The predicate.
  - *TSource*: The type of the source.
  - **Returns:** See Also

---

### LogLevel Enumeration

```csharp
public enum LogLevel
```

Values that represent LogLevel.

---

### Logging Class

**Inheritance:** System.Object → Styx.Common.Logging

```csharp
public static class Logging
```

A logging.

#### Logging Properties

- **`DefaultLogFilePath Property`**
  ```csharp
  public static string DefaultLogFilePath { get; }
  ```
  Gets the default log file path.

- **`LogFileLevel Property`**
  ```csharp
  public static LogLevel LogFileLevel { get; set; }
  ```
  Gets or sets the log file level.

- **`LogFilePath Property`**
  ```csharp
  public static string LogFilePath { get; set; }
  ```
  Gets or sets the full pathname of the log file.

- **`LoggingLevel Property`**
  ```csharp
  public static LogLevel LoggingLevel { get; set; }
  ```
  Gets or sets the logging level.

- **`StartTime Property`**
  ```csharp
  public static DateTime StartTime { get; }
  ```
  Gets or sets the start time.

#### Logging Methods

- **`Write Method`**
  Writes.

- **`Write Method (String)`**
  ```csharp
  public static void Write(
string message
)
  ```
  Writes.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTBAE3ADB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`Write Method (String, Object[])`**
  ```csharp
  public static void Write(
string format,
params Object[] args
)
  ```
  Writes.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST3C16CFCB_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LST3C16CFCB_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST3C16CFCB_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST3C16CFCB_6?cpp=&gt;|vb=()|nu=[]");  .

- **`Write Method (Color, String)`**
  ```csharp
  public static void Write(
Color color,
string message
)
  ```
  Writes.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LSTF2EF3830_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Color  The color.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTF2EF3830_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`Write Method (LogLevel, String)`**
  ```csharp
  public static void Write(
LogLevel level,
string message
)
  ```
  Writes.
  - *level*: Type: Styx.CommonAddLanguageSpecificTextSet("LST2A996D2C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevel  .
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST2A996D2C_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`Write Method (Color, String, Object[])`**
  ```csharp
  public static void Write(
Color color,
string format,
params Object[] args
)
  ```
  Writes.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LST609F014E_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Color The color.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST609F014E_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LST609F014E_5?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST609F014E_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST609F014E_7?cpp=&gt;|vb=()|nu=[]");  .

- **`Write Method (LogLevel, String, Object[])`**
  ```csharp
  public static void Write(
LogLevel level,
string format,
params Object[] args
)
  ```
  Writes.
  - *level*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTA1B604AA_3?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevel .
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTA1B604AA_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LSTA1B604AA_5?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTA1B604AA_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTA1B604AA_7?cpp=&gt;|vb=()|nu=[]");  .

- **`Write Method (LogLevel, Color, String)`**
  ```csharp
  public static void Write(
LogLevel level,
Color color,
string message
)
  ```
  Writes.
  - *level*: Type: Styx.CommonAddLanguageSpecificTextSet("LST62A8E9A3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevel  .
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LST62A8E9A3_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Color  The color.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST62A8E9A3_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`Write Method (LogLevel, Color, String, Object[])`**
  ```csharp
  public static void Write(
LogLevel level,
Color color,
string format,
params Object[] args
)
  ```
  Writes.
  - *level*: Type: Styx.CommonAddLanguageSpecificTextSet("LST1CB72913_3?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevel .
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LST1CB72913_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Color The color.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST1CB72913_5?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LST1CB72913_6?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST1CB72913_7?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST1CB72913_8?cpp=&gt;|vb=()|nu=[]");  .

- **`WriteDiagnostic Method`**
  Writes a diagnostic.

- **`WriteDiagnostic Method (String)`**
  ```csharp
  public static void WriteDiagnostic(
string message
)
  ```
  Writes a diagnostic.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTD8B570FA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`WriteDiagnostic Method (String, Object[])`**
  ```csharp
  public static void WriteDiagnostic(
string format,
params Object[] args
)
  ```
  Writes a diagnostic.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTFAACD948_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LSTFAACD948_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTFAACD948_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTFAACD948_6?cpp=&gt;|vb=()|nu=[]");  .

- **`WriteDiagnostic Method (Color, String)`**
  ```csharp
  public static void WriteDiagnostic(
Color color,
string message
)
  ```
  Writes a diagnostic.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LSTA4529AB1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Color  The color.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTA4529AB1_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`WriteDiagnostic Method (Color, String, Object[])`**
  ```csharp
  public static void WriteDiagnostic(
Color color,
string format,
params Object[] args
)
  ```
  Writes a diagnostic.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LSTCEF495B9_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Color The color.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTCEF495B9_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LSTCEF495B9_5?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTCEF495B9_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTCEF495B9_7?cpp=&gt;|vb=()|nu=[]");  .

- **`WriteException Method`**
  Writes an exception.

- **`WriteException Method (Exception)`**
  ```csharp
  public static void WriteException(
Exception ex
)
  ```
  Writes an exception.
  - *ex*: Type: SystemAddLanguageSpecificTextSet("LST944AB1C4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ExceptionThe ex.

- **`WriteException Method (Color, Exception)`**
  ```csharp
  public static void WriteException(
Color color,
Exception ex
)
  ```
  Writes an exception.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LST77E99133_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ColorThe color.
  - *ex*: Type: SystemAddLanguageSpecificTextSet("LST77E99133_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Exception   The ex.

- **`WriteException Method (LogLevel, Exception)`**
  ```csharp
  public static void WriteException(
LogLevel logLevel,
Exception ex
)
  ```
  Writes an exception.
  - *logLevel*: Type: Styx.CommonAddLanguageSpecificTextSet("LST1659CCBF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevelThe log level.
  - *ex*: Type: SystemAddLanguageSpecificTextSet("LST1659CCBF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Exception      The ex.

- **`WriteException Method (LogLevel, Color, Exception)`**
  ```csharp
  public static void WriteException(
LogLevel logLevel,
Color color,
Exception ex
)
  ```
  Writes an exception.
  - *logLevel*: Type: Styx.CommonAddLanguageSpecificTextSet("LST3C93EC22_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevelThe log level.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LST3C93EC22_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Color   The color.
  - *ex*: Type: SystemAddLanguageSpecificTextSet("LST3C93EC22_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Exception      The ex.

- **`WriteQuiet Method`**
  Writes a quiet.

- **`WriteQuiet Method (String)`**
  ```csharp
  public static void WriteQuiet(
string message
)
  ```
  Writes a quiet.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTAEA466B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`WriteQuiet Method (String, Object[])`**
  ```csharp
  public static void WriteQuiet(
string format,
params Object[] args
)
  ```
  Writes a quiet.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTF59EFE63_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LSTF59EFE63_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTF59EFE63_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTF59EFE63_6?cpp=&gt;|vb=()|nu=[]");  .

- **`WriteQuiet Method (Color, String)`**
  ```csharp
  public static void WriteQuiet(
Color color,
string message
)
  ```
  Writes a quiet.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LST78786E14_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Color  The color.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST78786E14_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`WriteQuiet Method (Color, String, Object[])`**
  ```csharp
  public static void WriteQuiet(
Color color,
string format,
params Object[] args
)
  ```
  Writes a quiet.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LST1935BFDA_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Color The color.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST1935BFDA_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LST1935BFDA_5?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST1935BFDA_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST1935BFDA_7?cpp=&gt;|vb=()|nu=[]");  .

- **`WriteToFile Method`**
  Writes a message only to the log file.

- **`WriteToFile Method (LogLevel, String)`**
  ```csharp
  public static void WriteToFile(
LogLevel level,
string message
)
  ```
  Writes a message only to the log file.
  - *level*: Type: Styx.CommonAddLanguageSpecificTextSet("LST155B3AFF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevelThe level.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST155B3AFF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.

- **`WriteToFile Method (LogLevel, String, Object[])`**
  ```csharp
  public static void WriteToFile(
LogLevel level,
string format,
params Object[] args
)
  ```
  Writes a message only to the log file.
  - *level*: Type: Styx.CommonAddLanguageSpecificTextSet("LST2C981BD7_3?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevelThe level.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST2C981BD7_4?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe format.
  - *args*: Type: AddLanguageSpecificTextSet("LST2C981BD7_5?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST2C981BD7_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST2C981BD7_7?cpp=&gt;|vb=()|nu=[]");The args to format.

- **`WriteToFileSync Method`**
  Writes a message directly to the log file and waits until the message has been
            successfully written.

- **`WriteToFileSync Method (LogLevel, String)`**
  ```csharp
  public static void WriteToFileSync(
LogLevel level,
string message
)
  ```
  Writes a message directly to the log file and waits until the message has been
            successfully written.
  - *level*: Type: Styx.CommonAddLanguageSpecificTextSet("LST45139308_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevel  .
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST45139308_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`WriteToFileSync Method (LogLevel, String, Object[])`**
  ```csharp
  public static void WriteToFileSync(
LogLevel level,
string format,
params Object[] args
)
  ```
  Writes a message directly to the log file and waits until the message has been
            successfully written.
  - *level*: Type: Styx.CommonAddLanguageSpecificTextSet("LST372BAB2_3?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevel .
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST372BAB2_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LST372BAB2_5?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST372BAB2_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST372BAB2_7?cpp=&gt;|vb=()|nu=[]");  .

- **`WriteVerbose Method`**
  Writes a verbose.

- **`WriteVerbose Method (String)`**
  ```csharp
  public static void WriteVerbose(
string message
)
  ```
  Writes a verbose.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTE3A54AC5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`WriteVerbose Method (String, Object[])`**
  ```csharp
  public static void WriteVerbose(
string format,
params Object[] args
)
  ```
  Writes a verbose.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST46C6A0BD_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LST46C6A0BD_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST46C6A0BD_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST46C6A0BD_6?cpp=&gt;|vb=()|nu=[]");  .

- **`WriteVerbose Method (Color, String)`**
  ```csharp
  public static void WriteVerbose(
Color color,
string message
)
  ```
  Writes a verbose.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LSTB02654BA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Color  The color.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTB02654BA_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`WriteVerbose Method (Color, String, Object[])`**
  ```csharp
  public static void WriteVerbose(
Color color,
string format,
params Object[] args
)
  ```
  Writes a verbose.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LST20DD3C88_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Color The color.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST20DD3C88_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LST20DD3C88_5?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST20DD3C88_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST20DD3C88_7?cpp=&gt;|vb=()|nu=[]");  .

#### Logging Events

- **`OnLogMessage Event`**
  ```csharp
  public static event Logging.LogMessageDelegate OnLogMessage
  ```
  Event queue for all listeners interested in OnLogMessage events.

---

### Logging.LogMessage Class

**Inheritance:** System.Object → Styx.Common.Logging.LogMessage

```csharp
public class LogMessage
```

A log message.

#### LogMessage Properties

- **`Color Property`**
  ```csharp
  public Color Color { get; set; }
  ```
  Gets or sets the color.

- **`FileOnly Property`**
  ```csharp
  public bool FileOnly { get; set; }
  ```
  Gets or sets a bool that indicates whether this message should only
            go the file.

- **`Level Property`**
  ```csharp
  public LogLevel Level { get; set; }
  ```
  Gets or sets the level.

- **`Message Property`**
  ```csharp
  public string Message { get; set; }
  ```
  Gets or sets the message.

- **`TimerTimestamp Property`**
  ```csharp
  public TimeSpan TimerTimestamp { get; set; }
  ```
  Gets or sets the timer timestamp.

- **`Timestamp Property`**
  ```csharp
  public DateTime Timestamp { get; set; }
  ```
  Gets or sets the Date/Time of the timestamp.

- **`TimestampType Property`**
  ```csharp
  public TimestampType TimestampType { get; set; }
  ```
  Gets or sets the timestamp type.

#### LogMessage Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceLoggingAddLanguageSpecificTextSet("LST58A240A6_2?cs=.|vb=.|cpp=::|nu=.|fs=.");LogMessage Class

---

### Logging.LogMessageDelegate Delegate

```csharp
public delegate void LogMessageDelegate(
ReadOnlyCollection&lt;Logging.LogMessage&gt; messages
)
```

Logs message delegate.

---

### LruCache(TKey, TValue) Class

**Inheritance:** System.Object → Styx.Common.LruCache.TKey, TValue

```csharp
public sealed class LruCache&lt;TKey, TValue&gt;
```

Represents a least-recently-used cache.

#### LruCache(TKey, TValue) Properties

- **`Capacity Property`**
  ```csharp
  public int Capacity { get; }
  ```

- **`Count Property`**
  ```csharp
  public int Count { get; }
  ```

- **`Item Property`**
  ```csharp
  public TValue this[
TKey key
] { get; set; }
  ```
  - *key*: Type: TKey

#### LruCache(TKey, TValue) Methods

- **`TryGetValue Method`**
  ```csharp
  public bool TryGetValue(
TKey key,
out TValue value
)
  ```
  - *key*: Type: TKey
  - *value*: Type: TValueAddLanguageSpecificTextSet("LSTA1156B95_3?cpp=%");
  - **Returns:** ReferenceLruCacheAddLanguageSpecificTextSet("LSTA1156B95_4?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TKey, TValueAddLanguageSpecificTextSet("LSTA1156B95_5?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### MathEx Class

**Inheritance:** System.Object → Styx.Common.MathEx

```csharp
public static class MathEx
```

The mathematics ex.

#### MathEx Methods

- **`CalculatePointFrom Method`**
  ```csharp
  public static Vector3 CalculatePointFrom(
Vector3 myLoc,
Vector3 targetLoc,
float pointDistanceToTarget
)
  ```
  Calculates the point from.
  - *myLoc*: Type: System.NumericsAddLanguageSpecificTextSet("LST54FC4431_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3                my location.
  - *targetLoc*: Type: System.NumericsAddLanguageSpecificTextSet("LST54FC4431_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3            Target location.
  - *pointDistanceToTarget*: Type: SystemAddLanguageSpecificTextSet("LST54FC4431_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe point distance to target.
  - **Returns:** ReferenceMathEx Class

- **`Clamp Method`**
  ```csharp
  public static float Clamp(
float min,
float max,
float value
)
  ```
  Clamp the given minimum.
  - *min*: Type: SystemAddLanguageSpecificTextSet("LST497FABBE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Single  .
  - *max*: Type: SystemAddLanguageSpecificTextSet("LST497FABBE_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single  .
  - *value*: Type: SystemAddLanguageSpecificTextSet("LST497FABBE_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single.
  - **Returns:** ReferenceMathEx Class

- **`ComputeDirection Method`**
  ```csharp
  public static int ComputeDirection(
double xi,
double yi,
double xj,
double yj,
double xk,
double yk
)
  ```
  Calculates the direction.
  - *xi*: Type: SystemAddLanguageSpecificTextSet("LST7D91DB58_1?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe xi.
  - *yi*: Type: SystemAddLanguageSpecificTextSet("LST7D91DB58_2?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe yi.
  - *xj*: Type: SystemAddLanguageSpecificTextSet("LST7D91DB58_3?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe xj.
  - *yj*: Type: SystemAddLanguageSpecificTextSet("LST7D91DB58_4?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe yj.
  - *xk*: Type: SystemAddLanguageSpecificTextSet("LST7D91DB58_5?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe xk.
  - *yk*: Type: SystemAddLanguageSpecificTextSet("LST7D91DB58_6?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe yk.
  - **Returns:** ReferenceMathEx Class

- **`Distance Method`**
  ```csharp
  public static float Distance(
Vector3 point,
LineSegment line
)
  ```
  Distances.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LSTA90F09D2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3[in,out] The point.
  - *line*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTA90F09D2_2?cs=.|vb=.|cpp=::|nu=.|fs=.");LineSegment [in,out] The line.
  - **Returns:** ReferenceMathEx Class

- **`DistanceSquared Method`**
  ```csharp
  public static float DistanceSquared(
Vector3 point,
LineSegment line
)
  ```
  Distance sqr.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LSTFFB5CF35_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3[in,out] The point.
  - *line*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTFFB5CF35_2?cs=.|vb=.|cpp=::|nu=.|fs=.");LineSegment [in,out] The line.
  - **Returns:** ReferenceMathEx Class

- **`DoLineSegmentsIntersect Method`**
  Executes the line segments intersect operation.

- **`DoLineSegmentsIntersect Method (Vector2, Vector2, Vector2, Vector2)`**
  ```csharp
  public static bool DoLineSegmentsIntersect(
Vector2 line1Start,
Vector2 line1End,
Vector2 line2Start,
Vector2 line2End
)
  ```
  Executes the line segments intersect operation.
  - *line1Start*: Type: System.NumericsAddLanguageSpecificTextSet("LSTF66B0A2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector2The line 1 start.
  - *line1End*: Type: System.NumericsAddLanguageSpecificTextSet("LSTF66B0A2_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector2  The line 1 end.
  - *line2Start*: Type: System.NumericsAddLanguageSpecificTextSet("LSTF66B0A2_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector2The line 2 start.
  - *line2End*: Type: System.NumericsAddLanguageSpecificTextSet("LSTF66B0A2_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector2  The line 2 end.
  - **Returns:** ReferenceMathEx Class

- **`DoLineSegmentsIntersect Method (Double, Double, Double, Double, Double, Double, Double, Double)`**
  ```csharp
  public static bool DoLineSegmentsIntersect(
double x1,
double y1,
double x2,
double y2,
double x3,
double y3,
double x4,
double y4
)
  ```
  Executes the line segments intersect operation.
  - *x1*: Type: SystemAddLanguageSpecificTextSet("LST72FB4922_1?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe first x value.
  - *y1*: Type: SystemAddLanguageSpecificTextSet("LST72FB4922_2?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe first y value.
  - *x2*: Type: SystemAddLanguageSpecificTextSet("LST72FB4922_3?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe second x value.
  - *y2*: Type: SystemAddLanguageSpecificTextSet("LST72FB4922_4?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe second y value.
  - *x3*: Type: SystemAddLanguageSpecificTextSet("LST72FB4922_5?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe third double.
  - *y3*: Type: SystemAddLanguageSpecificTextSet("LST72FB4922_6?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe third double.
  - *x4*: Type: SystemAddLanguageSpecificTextSet("LST72FB4922_7?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe fourth double.
  - *y4*: Type: SystemAddLanguageSpecificTextSet("LST72FB4922_8?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe fourth double.
  - **Returns:** ReferenceMathEx Class

- **`GetAmount Method`**
  ```csharp
  public static float GetAmount(
float min,
float max,
float value
)
  ```
  Returns a value between 0 and 1 if the value passed is between min and max.
  - *min*: Type: SystemAddLanguageSpecificTextSet("LSTF8DB72A1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Single  .
  - *max*: Type: SystemAddLanguageSpecificTextSet("LSTF8DB72A1_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single  .
  - *value*: Type: SystemAddLanguageSpecificTextSet("LSTF8DB72A1_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single.
  - **Returns:** ReferenceMathEx Class

- **`GetPointAt Method`**
  ```csharp
  public static Vector3 GetPointAt(
Vector3 from,
float distance,
float rotationRadians
)
  ```
  Gets point at.
  - *from*: Type: System.NumericsAddLanguageSpecificTextSet("LST609C16E6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3           Source for the.
  - *distance*: Type: SystemAddLanguageSpecificTextSet("LST609C16E6_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single       The distance.
  - *rotationRadians*: Type: SystemAddLanguageSpecificTextSet("LST609C16E6_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe rotation in radians.
  - **Returns:** ReferenceMathEx Class

- **`IntersectsPath Method`**
  ```csharp
  public static bool IntersectsPath(
Vector3 target,
float radius,
Vector3 start,
Vector3 destination
)
  ```
  Query if 'target' is in path.
  - *target*: Type: System.NumericsAddLanguageSpecificTextSet("LSTB4A0D8BF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3     The position of the target.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LSTB4A0D8BF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single     The radius.
  - *start*: Type: System.NumericsAddLanguageSpecificTextSet("LSTB4A0D8BF_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3      The start point.
  - *destination*: Type: System.NumericsAddLanguageSpecificTextSet("LSTB4A0D8BF_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3Destination for the.
  - **Returns:** ReferenceMathEx Class

- **`InverseLerp Method`**
  ```csharp
  public static float InverseLerp(
float min,
float max,
float amount
)
  ```
  Inverse linearly interpolate.
  - *min*: Type: SystemAddLanguageSpecificTextSet("LST648A02B4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Single   .
  - *max*: Type: SystemAddLanguageSpecificTextSet("LST648A02B4_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single   .
  - *amount*: Type: SystemAddLanguageSpecificTextSet("LST648A02B4_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe amount.
  - **Returns:** ReferenceMathEx Class

- **`IsOnSegment Method`**
  ```csharp
  public static bool IsOnSegment(
double xi,
double yi,
double xj,
double yj,
double xk,
double yk
)
  ```
  Query if this object is on segment.
  - *xi*: Type: SystemAddLanguageSpecificTextSet("LST34367C8A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe xi.
  - *yi*: Type: SystemAddLanguageSpecificTextSet("LST34367C8A_2?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe yi.
  - *xj*: Type: SystemAddLanguageSpecificTextSet("LST34367C8A_3?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe xj.
  - *yj*: Type: SystemAddLanguageSpecificTextSet("LST34367C8A_4?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe yj.
  - *xk*: Type: SystemAddLanguageSpecificTextSet("LST34367C8A_5?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe xk.
  - *yk*: Type: SystemAddLanguageSpecificTextSet("LST34367C8A_6?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe yk.
  - **Returns:** ReferenceMathEx Class

- **`Lerp Method`**
  ```csharp
  public static float Lerp(
float min,
float max,
float amount
)
  ```
  Lerps.
  - *min*: Type: SystemAddLanguageSpecificTextSet("LSTD6C7A1F6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Single   .
  - *max*: Type: SystemAddLanguageSpecificTextSet("LSTD6C7A1F6_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single   .
  - *amount*: Type: SystemAddLanguageSpecificTextSet("LSTD6C7A1F6_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe amount.
  - **Returns:** ReferenceMathEx Class

- **`Random Method`**
  ```csharp
  public static double Random(
double min,
double max
)
  ```
  Randoms.
  - *min*: Type: SystemAddLanguageSpecificTextSet("LSTDB58BEF9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Double.
  - *max*: Type: SystemAddLanguageSpecificTextSet("LSTDB58BEF9_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Double.
  - **Returns:** ReferenceMathEx Class

- **`ToDegrees Method`**
  ```csharp
  public static float ToDegrees(
float radians
)
  ```
  Converts the radians to the degrees.
  - *radians*: Type: SystemAddLanguageSpecificTextSet("LSTE1BC321B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe radians.
  - **Returns:** ReferenceMathEx Class

- **`ToRadians Method`**
  ```csharp
  public static float ToRadians(
float degrees
)
  ```
  Converts the degrees to the radians.
  - *degrees*: Type: SystemAddLanguageSpecificTextSet("LSTC4E4193C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe degrees.
  - **Returns:** ReferenceMathEx Class

- **`WrapAngle Method`**
  ```csharp
  public static float WrapAngle(
float radian
)
  ```
  Normalizes a radian so it's between -pi and pi.
  - *radian*: Type: SystemAddLanguageSpecificTextSet("LSTA354D23A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Single.
  - **Returns:** ReferenceMathEx Class

---

### ModifierKeys Enumeration

```csharp
[FlagsAttribute]
public enum ModifierKeys
```

Bitfield of flags for specifying ModifierKeys.

---

### PerformanceTimer Class

**Inheritance:** System.Object → Styx.Common.PerformanceTimer

```csharp
public class PerformanceTimer : IDisposable
```

A performance timer.

#### PerformanceTimer Properties

- **`ElapsedMilliseconds Property`**
  ```csharp
  public long ElapsedMilliseconds { get; }
  ```
  Gets the elapsed milliseconds.

#### PerformanceTimer Methods

- **`Dispose Method`**
  ```csharp
  public void Dispose()
  ```
  Performs application-defined tasks associated with freeing, releasing, or resetting
            unmanaged resources.

- **`DontPrint Method`**
  ```csharp
  public void DontPrint()
  ```
  Dont print.

- **`Start Method`**
  ```csharp
  public void Start()
  ```
  Starts the timer.

- **`StopAndPrint Method`**
  ```csharp
  public void StopAndPrint()
  ```
  Stops the timers and prints the time.

---

### Range Structure

```csharp
public struct Range : IEquatable&lt;Range&gt;
```

A range.

#### Range Properties

- **`Higher Property`**
  ```csharp
  public int Higher { get; set; }
  ```
  Gets or sets the higher.

- **`Lower Property`**
  ```csharp
  public int Lower { get; set; }
  ```
  Gets or sets the lower.

#### Range Methods

- **`Equals Method`**
  Indicates whether this instance and a specified object are equal.

- **`Equals Method (Range)`**
  ```csharp
  public bool Equals(
Range other
)
  ```
  Tests if this Range is considered equal to another.
  - *other*: Type: Styx.CommonAddLanguageSpecificTextSet("LST64DFCC9F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");RangeThe range to compare to this object.
  - **Returns:** See Also

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  Returns the hash code for this instance.
  - **Returns:** ReferenceRange Structure

---

### RangedDictionary(T) Class

**Inheritance:** System.Object → System.Collections.Generic.Dictionary.Range, List.T → Styx.Common.RangedDictionary.T

```csharp
public class RangedDictionary&lt;T&gt; : Dictionary&lt;Range, List&lt;T&gt;&gt;
where T : IRangeAble
```

Dictionary of ranged.

#### RangedDictionary(T) Properties

- **`ValueCount Property`**
  ```csharp
  public int ValueCount { get; }
  ```
  Gets the total value count with all ranges.

#### RangedDictionary(T) Methods

- **`Add Method`**
  Adds item.

- **`Add Method (T)`**
  ```csharp
  public void Add(
T item
)
  ```
  Adds item.
  - *item*: Type: TThe T to test for containment.

- **`Contains Method`**
  ```csharp
  public bool Contains(
T item
)
  ```
  Query if this object contains the given item.
  - *item*: Type: TThe T to test for containment.
  - **Returns:** ReferenceRangedDictionaryAddLanguageSpecificTextSet("LST443DB128_3?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST443DB128_4?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

- **`GetAllValues Method`**
  ```csharp
  public List&lt;T&gt; GetAllValues()
  ```
  Gets all values in all ranges.
  - **Returns:** See Also

- **`GetByBigRange Method`**
  ```csharp
  public List&lt;T&gt; GetByBigRange(
Range range
)
  ```
  Finds the bybigrange of the given arguments.
  - *range*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTBC808D4B_3?cs=.|vb=.|cpp=::|nu=.|fs=.");RangeThe range.
  - **Returns:** See Also

- **`Remove Method`**
  Removes the given item.

- **`Remove Method (T)`**
  ```csharp
  public bool Remove(
T item
)
  ```
  Removes the given item.
  - *item*: Type: TThe T to test for containment.
  - **Returns:** ReferenceRangedDictionaryAddLanguageSpecificTextSet("LST636BE147_3?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST636BE147_4?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

- **`RemoveIndexFromRange Method`**
  ```csharp
  public void RemoveIndexFromRange(
Range range,
int index
)
  ```
  Removes the index from range.
  - *range*: Type: Styx.CommonAddLanguageSpecificTextSet("LST9F38FC85_3?cs=.|vb=.|cpp=::|nu=.|fs=.");RangeThe range.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST9F38FC85_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32Zero-based index of the.

- **`RemoveWhereValue Method`**
  ```csharp
  public int RemoveWhereValue(
Predicate&lt;T&gt; predicate
)
  ```
  Removes the where value described by predicate.
  - *predicate*: Type: SystemAddLanguageSpecificTextSet("LSTE911BD11_3?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LSTE911BD11_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTE911BD11_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The predicate.
  - **Returns:** ReferenceRangedDictionaryAddLanguageSpecificTextSet("LSTE911BD11_6?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LSTE911BD11_7?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### Ray Structure

```csharp
public struct Ray
```

A ray.

#### Ray Methods

- **`Intersects Method`**
  ```csharp
  public Nullable&lt;float&gt; Intersects(
BoundingSphere boundingSphere
)
  ```
  Intersects the given sphere.
  - *boundingSphere*: Type: Styx.CommonAddLanguageSpecificTextSet("LST7D1D0DA7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BoundingSphereThe sphere.
  - **Returns:** See Also

#### Ray Fields

- **`Direction Field`**
  ```csharp
  public Vector3 Direction
  ```
  The direction.

- **`Position Field`**
  ```csharp
  public Vector3 Position
  ```
  The position.

---

### ReplaceCompositeListOperation Class

**Inheritance:** System.Object → Styx.Common.CompositeListOperation → Styx.Common.ReplaceCompositeListOperation

```csharp
public class ReplaceCompositeListOperation : CompositeListOperation
```

A replace composite list operation.

#### ReplaceCompositeListOperation Methods

- **`ApplyTo Method`**
  ```csharp
  public override void ApplyTo(
List&lt;Composite&gt; compositeList
)
  ```
  Applies to described by compositeList.
  - *compositeList*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST29A429B4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST29A429B4_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");CompositeAddLanguageSpecificTextSet("LST29A429B4_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");List of composites.

---

### ShapeHelper Class

**Inheritance:** System.Object → Styx.Common.ShapeHelper

```csharp
public static class ShapeHelper
```

A shape helper.

#### ShapeHelper Methods

- **`Box Method`**
  ```csharp
  public static void Box(
Vector3 center,
float width,
float height,
float depth,
out Vector3[] vertices,
out int[] indices
)
  ```
  Boxes.
  - *center*: Type: System.NumericsAddLanguageSpecificTextSet("LST4F78D877_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3  The center.
  - *width*: Type: SystemAddLanguageSpecificTextSet("LST4F78D877_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single   The width.
  - *height*: Type: SystemAddLanguageSpecificTextSet("LST4F78D877_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single  The height.
  - *depth*: Type: SystemAddLanguageSpecificTextSet("LST4F78D877_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Single   The depth.
  - *vertices*: Type: AddLanguageSpecificTextSet("LST4F78D877_5?cpp=array&lt;");System.NumericsAddLanguageSpecificTextSet("LST4F78D877_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3AddLanguageSpecificTextSet("LST4F78D877_7?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LST4F78D877_8?cpp=%");[out] The vertices.
  - *indices*: Type: AddLanguageSpecificTextSet("LST4F78D877_9?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST4F78D877_10?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LST4F78D877_11?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LST4F78D877_12?cpp=%"); [out] The indices.

- **`Disc Method`**
  ```csharp
  public static void Disc(
Vector3 center,
float radius,
int slices,
out Vector3[] vertices,
out int[] indices
)
  ```
  Discs.
  - *center*: Type: System.NumericsAddLanguageSpecificTextSet("LSTD422C7EE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3  The center.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LSTD422C7EE_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single  The radius.
  - *slices*: Type: SystemAddLanguageSpecificTextSet("LSTD422C7EE_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32  The slices.
  - *vertices*: Type: AddLanguageSpecificTextSet("LSTD422C7EE_4?cpp=array&lt;");System.NumericsAddLanguageSpecificTextSet("LSTD422C7EE_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3AddLanguageSpecificTextSet("LSTD422C7EE_6?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LSTD422C7EE_7?cpp=%");[out] The vertices.
  - *indices*: Type: AddLanguageSpecificTextSet("LSTD422C7EE_8?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTD422C7EE_9?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LSTD422C7EE_10?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LSTD422C7EE_11?cpp=%"); [out] The indices.

- **`Sphere Method`**
  ```csharp
  public static void Sphere(
Vector3 center,
float radius,
int stacks,
int slices,
out Vector3[] vertices,
out int[] indices
)
  ```
  Spheres.
  - *center*: Type: System.NumericsAddLanguageSpecificTextSet("LSTF66E0BD7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3  The center.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LSTF66E0BD7_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single  The radius.
  - *stacks*: Type: SystemAddLanguageSpecificTextSet("LSTF66E0BD7_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32  The stacks.
  - *slices*: Type: SystemAddLanguageSpecificTextSet("LSTF66E0BD7_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32  The slices.
  - *vertices*: Type: AddLanguageSpecificTextSet("LSTF66E0BD7_5?cpp=array&lt;");System.NumericsAddLanguageSpecificTextSet("LSTF66E0BD7_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3AddLanguageSpecificTextSet("LSTF66E0BD7_7?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LSTF66E0BD7_8?cpp=%");[out] The vertices.
  - *indices*: Type: AddLanguageSpecificTextSet("LSTF66E0BD7_9?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTF66E0BD7_10?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LSTF66E0BD7_11?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LSTF66E0BD7_12?cpp=%"); [out] The indices.

---

### StyxLog Class

**Inheritance:** System.Object → Styx.Common.StyxLog

```csharp
public class StyxLog
```

Initializes a new instance of the StyxLog class

#### StyxLog Properties

- **`FileLogging Property`**
  ```csharp
  public bool FileLogging { get; set; }
  ```
  Gets or sets a bool that indicates whether this log should log to a file.

- **`IsLogThreadRunning Property`**
  ```csharp
  public static bool IsLogThreadRunning { get; }
  ```
  Gets a bool that indicates whether the log thread is running.

- **`LogFileLevel Property`**
  ```csharp
  public LogLevel LogFileLevel { get; set; }
  ```
  Gets or sets the log file level.

- **`LogFilePath Property`**
  ```csharp
  public string LogFilePath { get; }
  ```
  Gets the full pathname of the log file.

- **`LoggingLevel Property`**
  ```csharp
  public LogLevel LoggingLevel { get; set; }
  ```
  Gets or sets the logging level.

- **`StartTime Property`**
  ```csharp
  public DateTime StartTime { get; }
  ```
  Gets the start time of this log instance.

- **`TimestampType Property`**
  ```csharp
  public TimestampType TimestampType { get; }
  ```
  Gets the timestamp type.

#### StyxLog Methods

- **`ChangeLogFilePath Method`**
  ```csharp
  public void ChangeLogFilePath(
string newPath
)
  ```
  Changes the log file path, and moves the old log file if necessary.
  - *newPath*: Type: SystemAddLanguageSpecificTextSet("LST811F40AC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`GetLogs Method`**
  ```csharp
  public static IEnumerable&lt;StyxLog&gt; GetLogs()
  ```
  Gets all the log instances that have been created.
  - **Returns:** See Also

- **`StartLogThread Method`**
  ```csharp
  public static void StartLogThread()
  ```
  Starts the log thread.

- **`StopLogThread Method`**
  ```csharp
  public static void StopLogThread()
  ```
  Stops the log thread.

- **`Write Method`**
  Writes.

- **`Write Method (String)`**
  ```csharp
  public void Write(
string message
)
  ```
  Writes.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST46AB4C88_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`Write Method (String, Object[])`**
  ```csharp
  public void Write(
string format,
params Object[] args
)
  ```
  Writes.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST7299EE36_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LST7299EE36_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST7299EE36_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST7299EE36_6?cpp=&gt;|vb=()|nu=[]");  .

- **`Write Method (Color, String)`**
  ```csharp
  public void Write(
Color color,
string message
)
  ```
  Writes.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LSTE10C49A3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Color  The color.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTE10C49A3_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`Write Method (LogLevel, String)`**
  ```csharp
  public void Write(
LogLevel level,
string message
)
  ```
  Writes.
  - *level*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTC57448F7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevel  .
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTC57448F7_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`Write Method (Color, String, Object[])`**
  ```csharp
  public void Write(
Color color,
string format,
params Object[] args
)
  ```
  Writes.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LST705ABCA3_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Color The color.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST705ABCA3_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LST705ABCA3_5?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST705ABCA3_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST705ABCA3_7?cpp=&gt;|vb=()|nu=[]");  .

- **`Write Method (LogLevel, String, Object[])`**
  ```csharp
  public void Write(
LogLevel level,
string format,
params Object[] args
)
  ```
  Writes.
  - *level*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTB509A57_3?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevel .
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTB509A57_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LSTB509A57_5?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTB509A57_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTB509A57_7?cpp=&gt;|vb=()|nu=[]");  .

- **`Write Method (LogLevel, Color, String)`**
  ```csharp
  public void Write(
LogLevel level,
Color color,
string message
)
  ```
  Writes.
  - *level*: Type: Styx.CommonAddLanguageSpecificTextSet("LST2D3DDC8E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevel  .
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LST2D3DDC8E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Color  The color.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST2D3DDC8E_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`Write Method (LogLevel, Color, String, Object[])`**
  ```csharp
  public void Write(
LogLevel level,
Color color,
string format,
params Object[] args
)
  ```
  Writes.
  - *level*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTD9ACCAC0_3?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevel .
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LSTD9ACCAC0_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Color The color.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTD9ACCAC0_5?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LSTD9ACCAC0_6?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTD9ACCAC0_7?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTD9ACCAC0_8?cpp=&gt;|vb=()|nu=[]");  .

- **`WriteDiagnostic Method`**
  Writes a diagnostic.

- **`WriteDiagnostic Method (String)`**
  ```csharp
  public void WriteDiagnostic(
string message
)
  ```
  Writes a diagnostic.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST5E12CF05_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`WriteDiagnostic Method (String, Object[])`**
  ```csharp
  public void WriteDiagnostic(
string format,
params Object[] args
)
  ```
  Writes a diagnostic.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTB7870695_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LSTB7870695_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTB7870695_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTB7870695_6?cpp=&gt;|vb=()|nu=[]");  .

- **`WriteDiagnostic Method (Color, String)`**
  ```csharp
  public void WriteDiagnostic(
Color color,
string message
)
  ```
  Writes a diagnostic.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LST9633D5C6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Color  The color.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST9633D5C6_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`WriteDiagnostic Method (Color, String, Object[])`**
  ```csharp
  public void WriteDiagnostic(
Color color,
string format,
params Object[] args
)
  ```
  Writes a diagnostic.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LSTA842F19C_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Color The color.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTA842F19C_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LSTA842F19C_5?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTA842F19C_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTA842F19C_7?cpp=&gt;|vb=()|nu=[]");  .

- **`WriteException Method`**
  Writes an exception.

- **`WriteException Method (Exception)`**
  ```csharp
  public void WriteException(
Exception ex
)
  ```
  Writes an exception.
  - *ex*: Type: SystemAddLanguageSpecificTextSet("LST2B344CEF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ExceptionThe ex.

- **`WriteException Method (Color, Exception)`**
  ```csharp
  public void WriteException(
Color color,
Exception ex
)
  ```
  Writes an exception.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LST5482EFDE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ColorThe color.
  - *ex*: Type: SystemAddLanguageSpecificTextSet("LST5482EFDE_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Exception   The ex.

- **`WriteException Method (LogLevel, Exception)`**
  ```csharp
  public void WriteException(
LogLevel logLevel,
Exception ex
)
  ```
  Writes an exception.
  - *logLevel*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTCF286462_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevelThe log level.
  - *ex*: Type: SystemAddLanguageSpecificTextSet("LSTCF286462_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Exception      The ex.

- **`WriteException Method (LogLevel, Color, Exception)`**
  ```csharp
  public void WriteException(
LogLevel logLevel,
Color color,
Exception ex
)
  ```
  Writes an exception.
  - *logLevel*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTF5632C3F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevelThe log level.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LSTF5632C3F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Color   The color.
  - *ex*: Type: SystemAddLanguageSpecificTextSet("LSTF5632C3F_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Exception      The ex.

- **`WriteQuiet Method`**
  Writes a quiet.

- **`WriteQuiet Method (String)`**
  ```csharp
  public void WriteQuiet(
string message
)
  ```
  Writes a quiet.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTB4244216_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`WriteQuiet Method (String, Object[])`**
  ```csharp
  public void WriteQuiet(
string format,
params Object[] args
)
  ```
  Writes a quiet.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST7F26FF40_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LST7F26FF40_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST7F26FF40_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST7F26FF40_6?cpp=&gt;|vb=()|nu=[]");  .

- **`WriteQuiet Method (Color, String)`**
  ```csharp
  public void WriteQuiet(
Color color,
string message
)
  ```
  Writes a quiet.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LST6BE1D59F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Color  The color.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST6BE1D59F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`WriteQuiet Method (Color, String, Object[])`**
  ```csharp
  public void WriteQuiet(
Color color,
string format,
params Object[] args
)
  ```
  Writes a quiet.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LSTF95205B7_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Color The color.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTF95205B7_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LSTF95205B7_5?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTF95205B7_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTF95205B7_7?cpp=&gt;|vb=()|nu=[]");  .

- **`WriteToFile Method`**
  Writes a message only to the log file.

- **`WriteToFile Method (LogLevel, String)`**
  ```csharp
  public void WriteToFile(
LogLevel level,
string message
)
  ```
  Writes a message only to the log file.
  - *level*: Type: Styx.CommonAddLanguageSpecificTextSet("LST7C073CC2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevelThe log level to log.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST7C073CC2_2?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.

- **`WriteToFile Method (LogLevel, String, Object[])`**
  ```csharp
  public void WriteToFile(
LogLevel level,
string format,
params Object[] args
)
  ```
  Writes a message only to the log file.
  - *level*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTCF65434C_3?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevelThe log level.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTCF65434C_4?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe format of the message.
  - *args*: Type: AddLanguageSpecificTextSet("LSTCF65434C_5?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTCF65434C_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTCF65434C_7?cpp=&gt;|vb=()|nu=[]");The args to format.

- **`WriteToFileSync Method`**
  Writes a message directly to the log file and waits until the message has been
                successfully written.

- **`WriteToFileSync Method (LogLevel, String)`**
  ```csharp
  public void WriteToFileSync(
LogLevel level,
string message
)
  ```
  Writes a message directly to the log file and waits until the message has been
                successfully written.
  - *level*: Type: Styx.CommonAddLanguageSpecificTextSet("LST1AF9617D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevel  .
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST1AF9617D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`WriteToFileSync Method (LogLevel, String, Object[])`**
  ```csharp
  public void WriteToFileSync(
LogLevel level,
string format,
params Object[] args
)
  ```
  Writes a message directly to the log file and waits until the message has been
                successfully written.
  - *level*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTCD53DDF5_3?cs=.|vb=.|cpp=::|nu=.|fs=.");LogLevel .
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTCD53DDF5_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LSTCD53DDF5_5?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTCD53DDF5_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTCD53DDF5_7?cpp=&gt;|vb=()|nu=[]");  .

- **`WriteVerbose Method`**
  Writes a verbose.

- **`WriteVerbose Method (String)`**
  ```csharp
  public void WriteVerbose(
string message
)
  ```
  Writes a verbose.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST7B5AD748_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`WriteVerbose Method (String, Object[])`**
  ```csharp
  public void WriteVerbose(
string format,
params Object[] args
)
  ```
  Writes a verbose.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTC7169672_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LSTC7169672_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTC7169672_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTC7169672_6?cpp=&gt;|vb=()|nu=[]");  .

- **`WriteVerbose Method (Color, String)`**
  ```csharp
  public void WriteVerbose(
Color color,
string message
)
  ```
  Writes a verbose.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LST6533106D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Color  The color.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST6533106D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String.

- **`WriteVerbose Method (Color, String, Object[])`**
  ```csharp
  public void WriteVerbose(
Color color,
string format,
params Object[] args
)
  ```
  Writes a verbose.
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LST8FDBF27D_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Color The color.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST8FDBF27D_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *args*: Type: AddLanguageSpecificTextSet("LST8FDBF27D_5?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST8FDBF27D_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST8FDBF27D_7?cpp=&gt;|vb=()|nu=[]");  .

#### StyxLog Events

- **`OnLogMessage Event`**
  ```csharp
  public event Logging.LogMessageDelegate OnLogMessage
  ```
  Event queue for all listeners interested in OnLogMessage events.

---

### ThreadSafeRandom Class

**Inheritance:** System.Object → Styx.Common.ThreadSafeRandom

```csharp
public class ThreadSafeRandom
```

Represents a thread safe random implementation.

#### ThreadSafeRandom Methods

- **`Next Method`**
  Returns a non-negative random integer.

- **`Next Method`**
  ```csharp
  public int Next()
  ```
  Returns a non-negative random integer.
  - **Returns:** ReferenceThreadSafeRandom Class

- **`Next Method (Int32)`**
  ```csharp
  public int Next(
int maxValue
)
  ```
  Returns a non-negative random integer that is less than the specified maximum.
  - *maxValue*: Type: SystemAddLanguageSpecificTextSet("LSTBCF52809_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to 0.
  - **Returns:** ExceptionConditionArgumentOutOfRangeExceptionmaxValuemaxValue is less than 0.

- **`Next Method (Int32, Int32)`**
  ```csharp
  public int Next(
int minValue,
int maxValue
)
  ```
  Returns a random integer that is within a specified range.
  - *minValue*: Type: SystemAddLanguageSpecificTextSet("LSTC8E8399E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The inclusive lower bound of the random number returned.
  - *maxValue*: Type: SystemAddLanguageSpecificTextSet("LSTC8E8399E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.
  - **Returns:** ExceptionConditionArgumentOutOfRangeExceptionminValue is greater than maxValue.

- **`NextBytes Method`**
  ```csharp
  public void NextBytes(
byte[] buffer
)
  ```
  Fills the elements of a specified array of bytes with random numbers.
  - *buffer*: Type: AddLanguageSpecificTextSet("LSTC16B93CE_1?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTC16B93CE_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ByteAddLanguageSpecificTextSet("LSTC16B93CE_3?cpp=&gt;|vb=()|nu=[]");An array of bytes to contain random numbers.

- **`NextDouble Method`**
  ```csharp
  public double NextDouble()
  ```
  Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.
  - **Returns:** ReferenceThreadSafeRandom Class

---

### TimedRecordKeeper(T) Class

**Inheritance:** System.Object → Styx.Common.TimedRecordKeeper.T

```csharp
public class TimedRecordKeeper&lt;T&gt;
```

A class that keeps record of an entry and a time at an interval for the last X
            milliseconds.

#### TimedRecordKeeper(T) Properties

- **`NumRecords Property`**
  ```csharp
  public int NumRecords { get; }
  ```
  Gets the amount of records this keeper keeps, based on the store interval and store
            time.

- **`StoreIntervalMilliseconds Property`**
  ```csharp
  public int StoreIntervalMilliseconds { get; }
  ```
  Gets the interval, in milliseconds, that the record keeper stores records.

- **`StoreTimeMilliseconds Property`**
  ```csharp
  public int StoreTimeMilliseconds { get; }
  ```
  Gets the amount of time the record keeper will keep records for.

#### TimedRecordKeeper(T) Methods

- **`AddRecord Method`**
  ```csharp
  public void AddRecord(
T value
)
  ```
  Adds a record into the time keeper.
  - *value*: Type: T.

- **`GetRecord Method`**
  ```csharp
  public bool GetRecord(
TimeSpan timeAgo,
out T value,
out TimeSpan elapsed
)
  ```
  Find the record that has been stored for an amount of time closest to the passed time.
  - *timeAgo*: Type: SystemAddLanguageSpecificTextSet("LSTD1ADBB19_3?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpan.
  - *value*: Type: TAddLanguageSpecificTextSet("LSTD1ADBB19_4?cpp=%");  [out].
  - *elapsed*: Type: SystemAddLanguageSpecificTextSet("LSTD1ADBB19_5?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanAddLanguageSpecificTextSet("LSTD1ADBB19_6?cpp=%");[out].
  - **Returns:** ReferenceTimedRecordKeeperAddLanguageSpecificTextSet("LSTD1ADBB19_7?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LSTD1ADBB19_8?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

- **`Reset Method`**
  ```csharp
  public void Reset()
  ```
  Resets this record keeper to contain zero active records.

---

### TimestampType Enumeration

```csharp
public enum TimestampType
```

Represents different types of timestamps.

---

### TreeHooks Class

**Inheritance:** System.Object → Styx.Common.TreeHooks

```csharp
public sealed class TreeHooks
```

A simplistic class to facilitate hooking into logic trees.

#### TreeHooks Properties

- **`HookDescriptions Property`**
  ```csharp
  public List&lt;HookDescription&gt; HookDescriptions { get; }
  ```
  Gets the hook descriptions.

- **`Hooks Property`**
  ```csharp
  public Dictionary&lt;string, List&lt;CompositeListOperation&gt;&gt; Hooks { get; }
  ```
  Gets the hooks.

- **`Instance Property`**
  ```csharp
  public static TreeHooks Instance { get; }
  ```
  Gets the instance.

#### TreeHooks Methods

- **`AddHook Method`**
  ```csharp
  public void AddHook(
string location,
Composite behavior
)
  ```
  Inserts a hook at the specified location. This appends the behavior to the end of the
            list.
  - *location*: Type: SystemAddLanguageSpecificTextSet("LST87D8A80A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe location.
  - *behavior*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST87D8A80A_2?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe behavior.

- **`ApplyCompositeOperations Method`**
  ```csharp
  public void ApplyCompositeOperations(
string location,
List&lt;Composite&gt; compositeList
)
  ```
  Applies the composite operations for the specified location, to
            the specified composite list.
  - *location*: Type: SystemAddLanguageSpecificTextSet("LST8AE3646C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String     .
  - *compositeList*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST8AE3646C_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST8AE3646C_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");CompositeAddLanguageSpecificTextSet("LST8AE3646C_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");.

- **`ClearAll Method`**
  ```csharp
  public void ClearAll()
  ```
  Clears all.

- **`InsertHook Method`**
  ```csharp
  public void InsertHook(
string location,
int index,
Composite behavior
)
  ```
  Inserts a hook at the specified location. This inserts the behavior to the specified
            index in the hook list.
  - *location*: Type: SystemAddLanguageSpecificTextSet("LSTE3B37D67_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe location.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTE3B37D67_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32   .
  - *behavior*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTE3B37D67_3?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe behavior.

- **`RemoveHook Method`**
  ```csharp
  public void RemoveHook(
string location,
Composite behavior
)
  ```
  Removes a hook at the specified location.
  - *location*: Type: SystemAddLanguageSpecificTextSet("LST87B857B3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe location.
  - *behavior*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST87B857B3_2?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe behavior.

- **`ReplaceHook Method`**
  ```csharp
  public void ReplaceHook(
string location,
Composite behavior
)
  ```
  Replaces all hooks at the specified location, with one that you provide.
  - *location*: Type: SystemAddLanguageSpecificTextSet("LST7FDABBE9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe location.
  - *behavior*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST7FDABBE9_2?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe behavior.

#### TreeHooks Events

- **`HooksCleared Event`**
  ```csharp
  public event Action HooksCleared
  ```
  Event queue for all listeners interested in HooksCleared events.

---

### TypeLoader(T) Class

**Inheritance:** System.Object → System.Collections.Generic.List.T → Styx.Common.TypeLoader.T

```csharp
public class TypeLoader&lt;T&gt; : List&lt;T&gt;
```

A type loader.

#### TypeLoader(T) Methods

- **`Reload Method`**
  ```csharp
  public void Reload()
  ```
  Reloads this object.

#### TypeLoader(T) Fields

- **`ConstructorSearch Field`**
  ```csharp
  public const BindingFlags ConstructorSearch = BindingFlags.Default|BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.CreateInstance
  ```
  The constructor search.

---

### TypeOnlyLoader(T) Class

**Inheritance:** System.Object → System.Collections.Generic.List.Type → Styx.Common.TypeOnlyLoader.T

```csharp
public class TypeOnlyLoader&lt;T&gt; : List&lt;Type&gt;
```

A type only loader.

#### TypeOnlyLoader(T) Methods

- **`CreateInstance Method`**
  ```csharp
  public T CreateInstance(
Type t,
params Object[] args
)
  ```
  Creates an instance.
  - *t*: Type: SystemAddLanguageSpecificTextSet("LSTB9E57439_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Type   The Type to process.
  - *args*: Type: AddLanguageSpecificTextSet("LSTB9E57439_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTB9E57439_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTB9E57439_6?cpp=&gt;|vb=()|nu=[]");A variable-length parameters list containing arguments.
  - **Returns:** See Also

- **`Reload Method`**
  ```csharp
  public void Reload()
  ```
  Reloads this object.

---

### Utilities Class

**Inheritance:** System.Object → Styx.Common.Utilities

```csharp
public static class Utilities
```

An utilities.

#### Utilities Properties

- **`AssemblyDirectory Property`**
  ```csharp
  public static string AssemblyDirectory { get; }
  ```
  Gets the pathname of the assembly directory.

- **`HonorbuddyAssembly Property`**
  ```csharp
  public static Assembly HonorbuddyAssembly { get; }
  ```
  Gets the Honorbuddy assembly.

- **`IsWindowsXp Property`**
  ```csharp
  public static bool IsWindowsXp { get; }
  ```
  Returns true if the current OS is Windows XP.

#### Utilities Methods

- **`BuildCompilerErrorsString Method`**
  ```csharp
  public static string BuildCompilerErrorsString(
CompilerResults results
)
  ```
  Builds compiler errors string.
  - *results*: Type: System.CodeDom.CompilerAddLanguageSpecificTextSet("LST65C40F44_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CompilerResultsThe results.
  - **Returns:** ReferenceUtilities Class

- **`ConvertFromUnixTimestamp Method`**
  ```csharp
  public static DateTime ConvertFromUnixTimestamp(
ulong timestamp
)
  ```
  Converts a unix timestamp to a DateTime .
  - *timestamp*: Type: SystemAddLanguageSpecificTextSet("LST92EFF0AF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt64The timestamp.
  - **Returns:** ReferenceUtilities Class

- **`ConvertToUnixTimestamp Method`**
  ```csharp
  public static int ConvertToUnixTimestamp(
DateTime date
)
  ```
  Converts a DateTime to a unix timestamp.
  - *date*: Type: SystemAddLanguageSpecificTextSet("LSTFBC92AF7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");DateTimeThe date.
  - **Returns:** ReferenceUtilities Class

- **`FNV1a Method`**
  ```csharp
  public static int FNV1a(
int value
)
  ```
  Fnv 1a.
  - *value*: Type: SystemAddLanguageSpecificTextSet("LST549C8AF0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The value.
  - **Returns:** ReferenceUtilities Class

- **`FormatString Method`**
  ```csharp
  public static string FormatString(
string format,
params Object[] args
)
  ```
  Format string.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTB5E5B4F9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringDescribes the format to use.
  - *args*: Type: AddLanguageSpecificTextSet("LSTB5E5B4F9_2?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTB5E5B4F9_3?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTB5E5B4F9_4?cpp=&gt;|vb=()|nu=[]");  A variable-length parameters list containing arguments.
  - **Returns:** ReferenceUtilities Class

- **`GenerateRandomString Method`**
  ```csharp
  public static string GenerateRandomString(
int minLength,
int maxLength
)
  ```
  Generates the random string with upper and lowercase letters only.
  - *minLength*: Type: SystemAddLanguageSpecificTextSet("LST56E5BFBC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
  - *maxLength*: Type: SystemAddLanguageSpecificTextSet("LST56E5BFBC_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
  - **Returns:** ReferenceUtilities Class

- **`GetObjectString Method`**
  ```csharp
  public static string GetObjectString(
Object obj,
string nullRet
)
  ```
  Gets a string for an object. If the object is null, it returns
            nullRet. If it's nonnull, it returns obj.ToString().
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LSTA9492BB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object    .
  - *nullRet*: Type: SystemAddLanguageSpecificTextSet("LSTA9492BB_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - **Returns:** ReferenceUtilities Class

- **`GetRelativePath Method`**
  Gets a relative path for the specified path, relative to the directory of the startup
            executable.

- **`GetRelativePath Method (String)`**
  ```csharp
  public static string GetRelativePath(
string path
)
  ```
  Gets a relative path for the specified path, relative to the directory of the startup
            executable.
  - *path*: Type: SystemAddLanguageSpecificTextSet("LST8690C654_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - **Returns:** ReferenceUtilities Class

- **`GetRelativePath Method (String, String)`**
  ```csharp
  public static string GetRelativePath(
string basePath,
string path
)
  ```
  Gets a relative path for a specified base path and path combination.
  - *basePath*: Type: SystemAddLanguageSpecificTextSet("LSTA56F82D4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe base path; for example Field ValueType: StringC:\Honorbuddy
  - *path*: Type: SystemAddLanguageSpecificTextSet("LSTA56F82D4_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String    The full path; for example Field ValueType: StringC:\Honorbuddy\Another Folder\
            Spells.bin
  - **Returns:** ReferenceUtilities Class

- **`GetStackTrace Method`**
  ```csharp
  public static string GetStackTrace(
int startFrame = 1,
int frameCount = 10
)
  ```
  - **Returns:** ReferenceUtilities Class

- **`MakeTempFile Method`**
  ```csharp
  public static string MakeTempFile(
string extension = "tmp"
)
  ```
  Creates a temporary 0-byte file and returns a path to it.
  - **Returns:** ReferenceUtilities Class

- **`PrintStackTrace Method`**
  ```csharp
  public static void PrintStackTrace(
string reason = "Debug"
)
  ```
  Print stack trace.

- **`RetryingDelete Method`**
  ```csharp
  public static void RetryingDelete(
string path,
int maxTries = 5,
int delayBetweenTries = 20
)
  ```
  Tries to delete a file, and retries if this fails.
  - *path*: Type: SystemAddLanguageSpecificTextSet("LST90D5A208_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe path to the file.

- **`RetryingFileOperation Method`**
  Tries to perform a retrying file operation.

- **`RetryingFileOperation Method (Action, Int32, Int32)`**
  ```csharp
  public static void RetryingFileOperation(
Action op,
int maxTries = 5,
int delayBetweenTries = 20
)
  ```
  Tries to perform a retrying file operation.
  - *op*: Type: SystemAddLanguageSpecificTextSet("LST1596033F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ActionThe operation.

- **`RetryingFileOperation(T) Method (Func(T), Int32, Int32)`**
  ```csharp
  public static T RetryingFileOperation&lt;T&gt;(
Func&lt;T&gt; op,
int maxTries = 5,
int delayBetweenTries = 20
)
  ```
  Tries to perform a retrying file operation.
  - *op*: Type: SystemAddLanguageSpecificTextSet("LSTB2E562DE_5?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTB2E562DE_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTB2E562DE_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The operation.
  - **Returns:** Remarks

---

### ValuePair(T1, T2) Structure

```csharp
public struct ValuePair&lt;T1, T2&gt;
```

A value pair.

#### ValuePair(T1, T2) Fields

- **`Value1 Field`**
  ```csharp
  public T1 Value1
  ```
  The first value.

- **`Value2 Field`**
  ```csharp
  public T2 Value2
  ```
  The second value.

---

### Vector3Extensions Class

**Inheritance:** System.Object → Styx.Common.Vector3Extensions

```csharp
public static class Vector3Extensions
```

#### Vector3Extensions Methods

- **`Add Method`**
  ```csharp
  public static Vector3 Add(
this Vector3 v,
float x,
float y,
float z
)
  ```
  - *v*: Type: System.NumericsAddLanguageSpecificTextSet("LST1778893C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - *x*: Type: SystemAddLanguageSpecificTextSet("LST1778893C_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *y*: Type: SystemAddLanguageSpecificTextSet("LST1778893C_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *z*: Type: SystemAddLanguageSpecificTextSet("LST1778893C_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - **Returns:** ReferenceVector3Extensions Class

- **`Distance Method`**
  ```csharp
  public static float Distance(
this Vector3 v1,
Vector3 v2
)
  ```
  - *v1*: Type: System.NumericsAddLanguageSpecificTextSet("LST3A19823E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - *v2*: Type: System.NumericsAddLanguageSpecificTextSet("LST3A19823E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - **Returns:** ReferenceVector3Extensions Class

- **`Distance2D Method`**
  ```csharp
  public static float Distance2D(
this Vector3 v1,
Vector3 v2
)
  ```
  - *v1*: Type: System.NumericsAddLanguageSpecificTextSet("LST5DA54938_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - *v2*: Type: System.NumericsAddLanguageSpecificTextSet("LST5DA54938_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - **Returns:** ReferenceVector3Extensions Class

- **`Distance2DSqr Method`**
  ```csharp
  [ObsoleteAttribute("Use Distance2DSquared")]
public static float Distance2DSqr(
this Vector3 v1,
Vector3 v2
)
  ```
  - *v1*: Type: System.NumericsAddLanguageSpecificTextSet("LSTEE33F160_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - *v2*: Type: System.NumericsAddLanguageSpecificTextSet("LSTEE33F160_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - **Returns:** ReferenceVector3Extensions Class

- **`Distance2DSquared Method`**
  ```csharp
  public static float Distance2DSquared(
this Vector3 v1,
Vector3 v2
)
  ```
  - *v1*: Type: System.NumericsAddLanguageSpecificTextSet("LST14AEF09B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - *v2*: Type: System.NumericsAddLanguageSpecificTextSet("LST14AEF09B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - **Returns:** ReferenceVector3Extensions Class

- **`DistanceSqr Method`**
  ```csharp
  [ObsoleteAttribute("Use DistanceSquared")]
public static float DistanceSqr(
this Vector3 v1,
Vector3 v2
)
  ```
  - *v1*: Type: System.NumericsAddLanguageSpecificTextSet("LSTAFEC97E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - *v2*: Type: System.NumericsAddLanguageSpecificTextSet("LSTAFEC97E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - **Returns:** ReferenceVector3Extensions Class

- **`DistanceSquared Method`**
  ```csharp
  public static float DistanceSquared(
this Vector3 v1,
Vector3 v2
)
  ```
  - *v1*: Type: System.NumericsAddLanguageSpecificTextSet("LST3CD8CE01_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - *v2*: Type: System.NumericsAddLanguageSpecificTextSet("LST3CD8CE01_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - **Returns:** ReferenceVector3Extensions Class

- **`GetDirectionTo Method`**
  ```csharp
  public static Vector3 GetDirectionTo(
this Vector3 v,
Vector3 other
)
  ```
  - *v*: Type: System.NumericsAddLanguageSpecificTextSet("LST97EF1CFB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - *other*: Type: System.NumericsAddLanguageSpecificTextSet("LST97EF1CFB_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - **Returns:** ReferenceVector3Extensions Class

- **`RayCast Method`**
  ```csharp
  public static Vector3 RayCast(
this Vector3 from,
float headingRadians,
float distance
)
  ```
  - *from*: Type: System.NumericsAddLanguageSpecificTextSet("LST9EF39C8B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - *headingRadians*: Type: SystemAddLanguageSpecificTextSet("LST9EF39C8B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *distance*: Type: SystemAddLanguageSpecificTextSet("LST9EF39C8B_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - **Returns:** ReferenceVector3Extensions Class

- **`ToVector2 Method`**
  ```csharp
  public static Vector2 ToVector2(
this Vector3 v
)
  ```
  - *v*: Type: System.NumericsAddLanguageSpecificTextSet("LST856475D6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - **Returns:** ReferenceVector3Extensions Class

---
