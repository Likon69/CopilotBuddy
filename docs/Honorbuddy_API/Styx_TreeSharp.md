# Styx.TreeSharp

Contains behavior tree related classes.

## Contents

- [Action Class](#action-class)
- [ActionDelegate Delegate](#actiondelegate-delegate)
- [ActionSucceedDelegate Delegate](#actionsucceeddelegate-delegate)
- [CanRunDecoratorDelegate Delegate](#canrundecoratordelegate-delegate)
- [Composite Class](#composite-class)
- [Composite.CleanupHandler Class](#composite.cleanuphandler-class)
- [ContextChangeHandler Delegate](#contextchangehandler-delegate)
- [Decorator Class](#decorator-class)
- [DecoratorContinue Class](#decoratorcontinue-class)
- [DynamicChildSelector Class](#dynamicchildselector-class)
- [GroupComposite Class](#groupcomposite-class)
- [GroupComposite.ChildrenCleanupHandler Class](#groupcomposite.childrencleanuphandler-class)
- [PrioritySelector Class](#priorityselector-class)
- [ProbabilitySelector Class](#probabilityselector-class)
- [RetrieveSwitchParameterDelegate(T) Delegate](#retrieveswitchparameterdelegatet-delegate)
- [RunStatus Enumeration](#runstatus-enumeration)
- [Selector Class](#selector-class)
- [Sequence Class](#sequence-class)
- [Sleep Class](#sleep-class)
- [Switch(T) Class](#switcht-class)
- [SwitchArgument(T) Class](#switchargumentt-class)
- [Wait Class](#wait-class)
- [WaitContinue Class](#waitcontinue-class)
- [WaitGetTimeSpanTimeoutDelegate Delegate](#waitgettimespantimeoutdelegate-delegate)
- [WaitGetTimeoutDelegate Delegate](#waitgettimeoutdelegate-delegate)
- [WhileLoop Class](#whileloop-class)

---

### Action Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → More.

```csharp
public class Action : Composite
```

The base Action class. A simple, easy to use, way to execute actions, and return
            their status of execution. These are normally considered 'atoms' in that they are executed in
            their entirety.

#### Action Constructor

- **`Action Constructor`**
  ```csharp
  public Action()
  ```
  Default constructor.

- **`Action Constructor (ActionDelegate)`**
  ```csharp
  public Action(
ActionDelegate action
)
  ```
  Constructor.
  - *action*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST13019EEC_0?cs=.|vb=.|cpp=::|nu=.|fs=.");ActionDelegateThe action.

- **`Action Constructor (ActionSucceedDelegate)`**
  ```csharp
  public Action(
ActionSucceedDelegate action
)
  ```
  Constructor.
  - *action*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST44CD878C_0?cs=.|vb=.|cpp=::|nu=.|fs=.");ActionSucceedDelegateThe action.

#### Action Properties

- **`Runner Property`**
  ```csharp
  public ActionDelegate Runner { get; }
  ```
  Gets the runner.

- **`SucceedRunner Property`**
  ```csharp
  public ActionSucceedDelegate SucceedRunner { get; }
  ```
  Gets the succeed runner.

#### Action Methods

- **`Execute Method`**
  ```csharp
  protected override sealed IEnumerable&lt;RunStatus&gt; Execute(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST4FAD062F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** See Also

- **`Run Method`**
  ```csharp
  protected virtual RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTCF8A90AD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceAction Class

- **`RunAction Method`**
  ```csharp
  protected RunStatus RunAction(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST86DFB4D7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceAction Class

---

### ActionDelegate Delegate

```csharp
public delegate RunStatus ActionDelegate(
Object context
)
```

Action delegate.

---

### ActionSucceedDelegate Delegate

```csharp
public delegate void ActionSucceedDelegate(
Object context
)
```

Action succeed delegate.

---

### CanRunDecoratorDelegate Delegate

```csharp
public delegate bool CanRunDecoratorDelegate(
Object context
)
```

Determine if we can run decorator delegate.

---

### Composite Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Bots.Quest.Actions.ForcedBehaviorExecutor → Styx.TreeSharp.Action → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Sleep

```csharp
public abstract class Composite : IEquatable&lt;Composite&gt;
```

The base class of the entire behavior tree system. Nearly all branches derive from
            this class.

#### Composite Properties

- **`CleanupHandlers Property`**
  ```csharp
  protected Stack&lt;Composite.CleanupHandler&gt; CleanupHandlers { get; set; }
  ```

- **`ContextChanger Property`**
  ```csharp
  protected ContextChangeHandler ContextChanger { get; set; }
  ```

- **`Guid Property`**
  ```csharp
  public Guid Guid { get; }
  ```
  Gets a unique identifier.

- **`IsRunning Property`**
  ```csharp
  public bool IsRunning { get; }
  ```
  Returns true if the current behavior is running.

- **`LastStatus Property`**
  ```csharp
  public Nullable&lt;RunStatus&gt; LastStatus { get; set; }
  ```
  Contains the execution status (if any) provided by the last Tick()

- **`Parent Property`**
  ```csharp
  public Composite Parent { get; set; }
  ```
  Gets or sets the parent.

#### Composite Methods

- **`Cleanup Method`**
  ```csharp
  protected void Cleanup()
  ```

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
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LST69D673F0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe Object to compare with the current
            Object.
  - **Returns:** ReferenceComposite Class

- **`Equals Method (Composite)`**
  ```csharp
  public bool Equals(
Composite other
)
  ```
  Indicates whether the current object is equal to another object of the same type.
  - *other*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST969360A3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeAn object to compare with this object.
  - **Returns:** See Also

- **`Execute Method`**
  ```csharp
  protected abstract IEnumerable&lt;RunStatus&gt; Execute(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST470A0A0A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** See Also

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  Serves as a hash function for a particular type.
  - **Returns:** ReferenceComposite Class

- **`Start Method`**
  ```csharp
  public virtual void Start(
Object context
)
  ```
  Initializes the Task into a runnable state by clearing the LastStatus (if any),
            clearing the cleanup handlers remaining (if any -- note that this may also trigger the
            IDisposable behavior of any remaining cleanup handlers on the cleanup stack), and
            reinitializing the execution enumerator.
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST5E873457_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe context.

- **`Stop Method`**
  ```csharp
  public virtual void Stop(
Object context
)
  ```
  Stop will call all cleanup handlers!
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTE3381FB3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe context.

- **`Tick Method`**
  ```csharp
  public virtual RunStatus Tick(
Object context
)
  ```
  Executes the user code of the function, as expressed in Execute()
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTBF76DAF4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe context.
  - **Returns:** ExceptionConditionApplicationExceptionThrown when an Application error condition occurs.

#### Composite Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
Composite left,
Composite right
)
  ```
  Equality operator.
  - *left*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTF609E759_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite The left.
  - *right*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTF609E759_2?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe right.
  - **Returns:** ReferenceComposite Class

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
Composite left,
Composite right
)
  ```
  Inequality operator.
  - *left*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST424963DE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite The left.
  - *right*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST424963DE_2?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe right.
  - **Returns:** ReferenceComposite Class

#### Composite Fields

- **`Locker Field`**
  ```csharp
  protected static readonly Object Locker
  ```
  Provides a lockable object so we can gain a mutex over the operations done in Tick(.)

---

### Composite.CleanupHandler Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite.CleanupHandler → Styx.TreeSharp.GroupComposite.ChildrenCleanupHandler

```csharp
protected abstract class CleanupHandler : IDisposable
```

Initializes a new instance of the Composite.CleanupHandler class

#### CleanupHandler Properties

- **`Context Property`**
  ```csharp
  public Object Context { get; }
  ```
  Gets the context.

- **`IsDisposed Property`**
  ```csharp
  public bool IsDisposed { get; }
  ```
  Gets a value indicating whether this object is disposed.

- **`Owner Property`**
  ```csharp
  public Composite Owner { get; }
  ```
  Gets the owner.

#### CleanupHandler Methods

- **`Dispose Method`**
  ```csharp
  public void Dispose()
  ```
  Performs application-defined tasks associated with freeing, releasing, or resetting
            unmanaged resources.

- **`DoCleanup Method`**
  ```csharp
  protected abstract void DoCleanup(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST420B9E94_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Object

---

### ContextChangeHandler Delegate

```csharp
public delegate Object ContextChangeHandler(
Object original
)
```

Handler, called when the context change.

---

### Decorator Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → More.

```csharp
public class Decorator : GroupComposite
```

A decorator.

#### Decorator Constructor

- **`Decorator Constructor`**
  ```csharp
  public Decorator()
  ```
  Default constructor.

- **`Decorator Constructor (Composite)`**
  ```csharp
  public Decorator(
Composite child
)
  ```
  Constructor.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTDF89B673_0?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe child.

- **`Decorator Constructor (CanRunDecoratorDelegate, Composite)`**
  ```csharp
  public Decorator(
CanRunDecoratorDelegate func,
Composite decorated
)
  ```
  Constructor.
  - *func*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTB4F9F508_0?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate     The function.
  - *decorated*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTB4F9F508_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe decorated.

- **`Decorator Constructor (Composite, CanRunDecoratorDelegate)`**
  ```csharp
  public Decorator(
Composite decorated,
CanRunDecoratorDelegate func
)
  ```
  Constructor.
  - *decorated*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST78AB1F68_0?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe decorated.
  - *func*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST78AB1F68_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate     The function.

#### Decorator Properties

- **`DecoratedChild Property`**
  ```csharp
  public Composite DecoratedChild { get; }
  ```
  Gets the decorated child.

- **`Runner Property`**
  ```csharp
  protected CanRunDecoratorDelegate Runner { get; }
  ```

#### Decorator Methods

- **`CanRun Method`**
  ```csharp
  protected virtual bool CanRun(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST8044E7FC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceDecorator Class

- **`Execute Method`**
  ```csharp
  protected override IEnumerable&lt;RunStatus&gt; Execute(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST2A2BA054_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** See Also

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
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST62415105_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe context.

---

### DecoratorContinue Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → Styx.TreeSharp.DecoratorContinue → Bots.DungeonBuddy.Helpers.DecoratorContinue.T

```csharp
public class DecoratorContinue : Decorator
```

A decorator continue.

#### DecoratorContinue Constructor

- **`DecoratorContinue Constructor`**
  ```csharp
  public DecoratorContinue()
  ```
  Default constructor.

- **`DecoratorContinue Constructor (Composite)`**
  ```csharp
  public DecoratorContinue(
Composite child
)
  ```
  Constructor.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST2172BDF2_0?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe child.

- **`DecoratorContinue Constructor (CanRunDecoratorDelegate, Composite)`**
  ```csharp
  public DecoratorContinue(
CanRunDecoratorDelegate func,
Composite decorated
)
  ```
  Constructor.
  - *func*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST20725E27_0?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate     The function.
  - *decorated*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST20725E27_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe decorated.

- **`DecoratorContinue Constructor (Composite, CanRunDecoratorDelegate)`**
  ```csharp
  public DecoratorContinue(
Composite decorated,
CanRunDecoratorDelegate func
)
  ```
  Constructor.
  - *decorated*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST4B6CB947_0?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe decorated.
  - *func*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST4B6CB947_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate     The function.

#### DecoratorContinue Methods

- **`Execute Method`**
  ```csharp
  protected override IEnumerable&lt;RunStatus&gt; Execute(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST72032153_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** See Also

---

### DynamicChildSelector Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Selector → Styx.TreeSharp.PrioritySelector → Styx.TreeSharp.DynamicChildSelector

```csharp
public class DynamicChildSelector : PrioritySelector
```

A dynamic child selector.

#### DynamicChildSelector Methods

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
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST4DAF9DD4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe context.

---

### GroupComposite Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → Styx.TreeSharp.Selector → Styx.TreeSharp.Sequence → Styx.TreeSharp.Switch.T → Styx.TreeSharp.WhileLoop

```csharp
public abstract class GroupComposite : Composite
```

A group composite.

#### GroupComposite Properties

- **`Children Property`**
  ```csharp
  public List&lt;Composite&gt; Children { get; set; }
  ```
  Gets or sets the children.

- **`Selection Property`**
  ```csharp
  public Composite Selection { get; protected set; }
  ```
  Gets the selection.

#### GroupComposite Methods

- **`AddChild Method`**
  ```csharp
  public void AddChild(
Composite child
)
  ```
  Adds a child.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST94BEB138_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe child.

- **`InsertChild Method`**
  ```csharp
  public void InsertChild(
int index,
Composite child
)
  ```
  Inserts a child.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST92C0A83F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32Zero-based index of the.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST92C0A83F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe child.

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
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST5C419286_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe context.

---

### GroupComposite.ChildrenCleanupHandler Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite.CleanupHandler → Styx.TreeSharp.GroupComposite.ChildrenCleanupHandler

```csharp
protected class ChildrenCleanupHandler : Composite.CleanupHandler
```

Constructor.

#### ChildrenCleanupHandler Methods

- **`DoCleanup Method`**
  ```csharp
  protected override void DoCleanup(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTBE3CC762_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Object

---

### PrioritySelector Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Selector → Styx.TreeSharp.PrioritySelector → Styx.TreeSharp.DynamicChildSelector

```csharp
public class PrioritySelector : Selector
```

Will execute each branch of logic in order, until one succeeds. This composite will
            fail only if all branches fail as well.

#### PrioritySelector Constructor

- **`PrioritySelector Constructor (Composite[])`**
  ```csharp
  public PrioritySelector(
params Composite[] children
)
  ```
  Constructor.
  - *children*: Type: AddLanguageSpecificTextSet("LST5E709EF9_2?cpp=array&lt;");Styx.TreeSharpAddLanguageSpecificTextSet("LST5E709EF9_3?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeAddLanguageSpecificTextSet("LST5E709EF9_4?cpp=&gt;|vb=()|nu=[]");A variable-length parameters list containing children.

- **`PrioritySelector Constructor (ContextChangeHandler, Composite[])`**
  ```csharp
  public PrioritySelector(
ContextChangeHandler contextChange,
params Composite[] children
)
  ```
  Constructor.
  - *contextChange*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST4B51750E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ContextChangeHandlerThe context change.
  - *children*: Type: AddLanguageSpecificTextSet("LST4B51750E_3?cpp=array&lt;");Styx.TreeSharpAddLanguageSpecificTextSet("LST4B51750E_4?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeAddLanguageSpecificTextSet("LST4B51750E_5?cpp=&gt;|vb=()|nu=[]");     A variable-length parameters list containing children.

#### PrioritySelector Methods

- **`Execute Method`**
  ```csharp
  protected override IEnumerable&lt;RunStatus&gt; Execute(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST1768121E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** See Also

---

### ProbabilitySelector Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Selector → Styx.TreeSharp.ProbabilitySelector

```csharp
public class ProbabilitySelector : Selector
```

Will execute random branches of logic, until one succeeds. This composite will fail
            only if all branches fail as well.

#### ProbabilitySelector Properties

- **`Randomizer Property`**
  ```csharp
  protected Random Randomizer { get; }
  ```

#### ProbabilitySelector Methods

- **`Execute Method`**
  ```csharp
  protected override IEnumerable&lt;RunStatus&gt; Execute(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTDDA417D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** See Also

---

### RetrieveSwitchParameterDelegate(T) Delegate

```csharp
public delegate T RetrieveSwitchParameterDelegate&lt;T&gt;(
Object context
)
```

Retrieves switch parameter delegate.

---

### RunStatus Enumeration

```csharp
public enum RunStatus
```

Values that can be returned from composites and the like.

---

### Selector Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Selector → Styx.TreeSharp.PrioritySelector → Styx.TreeSharp.ProbabilitySelector

```csharp
public abstract class Selector : GroupComposite
```

The base selector class. This will attempt to execute all branches of logic, until
            one succeeds. This composite will fail only if all branches fail as well.

#### Selector Methods

- **`Execute Method`**
  ```csharp
  protected abstract IEnumerable&lt;RunStatus&gt; Execute(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTFE48650A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** See Also

---

### Sequence Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Sequence

```csharp
public class Sequence : GroupComposite
```

The base sequence class. This will execute each branch of logic, in order. If all
            branches succeed, this composite will return a successful run status. If any branch fails,
            this composite will return a failed run status.

#### Sequence Constructor

- **`Sequence Constructor (Composite[])`**
  ```csharp
  public Sequence(
params Composite[] children
)
  ```
  Constructor.
  - *children*: Type: AddLanguageSpecificTextSet("LSTD3744FE1_2?cpp=array&lt;");Styx.TreeSharpAddLanguageSpecificTextSet("LSTD3744FE1_3?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeAddLanguageSpecificTextSet("LSTD3744FE1_4?cpp=&gt;|vb=()|nu=[]");A variable-length parameters list containing children.

- **`Sequence Constructor (ContextChangeHandler, Composite[])`**
  ```csharp
  public Sequence(
ContextChangeHandler contextChange,
params Composite[] children
)
  ```
  Constructor.
  - *contextChange*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST344D85A6_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ContextChangeHandlerThe context change.
  - *children*: Type: AddLanguageSpecificTextSet("LST344D85A6_3?cpp=array&lt;");Styx.TreeSharpAddLanguageSpecificTextSet("LST344D85A6_4?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeAddLanguageSpecificTextSet("LST344D85A6_5?cpp=&gt;|vb=()|nu=[]");     A variable-length parameters list containing children.

#### Sequence Methods

- **`Execute Method`**
  ```csharp
  protected override IEnumerable&lt;RunStatus&gt; Execute(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTD5EA1606_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** See Also

---

### Sleep Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Sleep → CommonBehaviors.Actions.SleepForLagDuration

```csharp
public class Sleep : Composite
```

A small action composite to force any part of the tree to not execute for a given
            amount of time.

#### Sleep Constructor

- **`Sleep Constructor (Func(Object, TimeSpan))`**
  ```csharp
  public Sleep(
Func&lt;Object, TimeSpan&gt; retriever
)
  ```
  Initializes a new instance of the Sleep class.
  - *retriever*: Type: SystemAddLanguageSpecificTextSet("LST67E92C91_2?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST67E92C91_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, TimeSpanAddLanguageSpecificTextSet("LST67E92C91_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The retriever.

- **`Sleep Constructor (Int32)`**
  ```csharp
  public Sleep(
int milliseconds
)
  ```
  Initializes a new instance of the Sleep class.
  - *milliseconds*: Type: SystemAddLanguageSpecificTextSet("LSTE1A9906F_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The milliseconds.

- **`Sleep Constructor (TimeSpan)`**
  ```csharp
  public Sleep(
TimeSpan time
)
  ```
  Initializes a new instance of the Sleep class.
  - *time*: Type: SystemAddLanguageSpecificTextSet("LSTDA03D5F0_0?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe time.

#### Sleep Methods

- **`Execute Method`**
  ```csharp
  protected override IEnumerable&lt;RunStatus&gt; Execute(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST9613BB06_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** See Also

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
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTCD5D18C1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object.

---

### Switch(T) Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Switch.T

```csharp
public class Switch&lt;T&gt; : GroupComposite
```

This composite will perform a 'switch' statement to execute a specific branch of
            logic. This is useful for selecting specific branches, for different types of agents. (e.g.
            rogue, mage, and warrior branches)

#### Switch(T) Constructor

- **`Switch(T) Constructor (RetrieveSwitchParameterDelegate(T), SwitchArgument(T)[])`**
  ```csharp
  public Switch(
RetrieveSwitchParameterDelegate&lt;T&gt; statement,
params SwitchArgument&lt;T&gt;[] args
)
  ```
  Constructor.
  - *statement*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST686657EB_8?cs=.|vb=.|cpp=::|nu=.|fs=.");RetrieveSwitchParameterDelegateAddLanguageSpecificTextSet("LST686657EB_9?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST686657EB_10?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The statement.
  - *args*: Type: AddLanguageSpecificTextSet("LST686657EB_11?cpp=array&lt;");Styx.TreeSharpAddLanguageSpecificTextSet("LST686657EB_12?cs=.|vb=.|cpp=::|nu=.|fs=.");SwitchArgumentAddLanguageSpecificTextSet("LST686657EB_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST686657EB_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");AddLanguageSpecificTextSet("LST686657EB_15?cpp=&gt;|vb=()|nu=[]");     A variable-length parameters list containing arguments.

- **`Switch(T) Constructor (RetrieveSwitchParameterDelegate(T), Composite, SwitchArgument(T)[])`**
  ```csharp
  public Switch(
RetrieveSwitchParameterDelegate&lt;T&gt; statement,
Composite defaultArgument,
params SwitchArgument&lt;T&gt;[] args
)
  ```
  Constructor.
  - *statement*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST96F9B4C8_8?cs=.|vb=.|cpp=::|nu=.|fs=.");RetrieveSwitchParameterDelegateAddLanguageSpecificTextSet("LST96F9B4C8_9?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST96F9B4C8_10?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");      The statement.
  - *defaultArgument*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST96F9B4C8_11?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeThe default argument.
  - *args*: Type: AddLanguageSpecificTextSet("LST96F9B4C8_12?cpp=array&lt;");Styx.TreeSharpAddLanguageSpecificTextSet("LST96F9B4C8_13?cs=.|vb=.|cpp=::|nu=.|fs=.");SwitchArgumentAddLanguageSpecificTextSet("LST96F9B4C8_14?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST96F9B4C8_15?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");AddLanguageSpecificTextSet("LST96F9B4C8_16?cpp=&gt;|vb=()|nu=[]");           A variable-length parameters list containing arguments.

#### Switch(T) Properties

- **`Arguments Property`**
  ```csharp
  protected SwitchArgument&lt;T&gt;[] Arguments { get; set; }
  ```

- **`Default Property`**
  ```csharp
  protected Composite Default { get; set; }
  ```

- **`Statement Property`**
  ```csharp
  protected RetrieveSwitchParameterDelegate&lt;T&gt; Statement { get; set; }
  ```

#### Switch(T) Methods

- **`Execute Method`**
  ```csharp
  protected override IEnumerable&lt;RunStatus&gt; Execute(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST9C5D49D8_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** See Also

- **`RunSwitch Method`**
  ```csharp
  protected void RunSwitch(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTFF799186_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Object

---

### SwitchArgument(T) Class

**Inheritance:** System.Object → Styx.TreeSharp.SwitchArgument.T

```csharp
public class SwitchArgument&lt;T&gt;
```

A switch argument.

#### SwitchArgument(T) Constructor

- **`SwitchArgument(T) Constructor (T, Composite)`**
  ```csharp
  public SwitchArgument(
T requiredValue,
Composite branch
)
  ```
  Constructor.
  - *requiredValue*: Type: TThe required value.
  - *branch*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST726E1EE8_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite       The branch.

- **`SwitchArgument(T) Constructor (Composite, T)`**
  ```csharp
  public SwitchArgument(
Composite branch,
T requiredValue
)
  ```
  Constructor.
  - *branch*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST638F9286_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite       The branch.
  - *requiredValue*: Type: TThe required value.

#### SwitchArgument(T) Properties

- **`Branch Property`**
  ```csharp
  public Composite Branch { get; set; }
  ```
  A branch of logic that will be executed if this argument is the correct switch.

- **`RequiredValue Property`**
  ```csharp
  public T RequiredValue { get; set; }
  ```
  The value required for this logic branch to be executed.

---

### Wait Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → Styx.TreeSharp.Wait → Styx.TreeSharp.WaitContinue

```csharp
public class Wait : Decorator
```

Creates a Composite that will wait while the defined condition is false.

#### Wait Constructor

- **`Wait Constructor (Int32, Composite)`**
  ```csharp
  public Wait(
int timeoutSeconds,
Composite child
)
  ```
  Constructor.
  - *timeoutSeconds*: Type: SystemAddLanguageSpecificTextSet("LSTCAE6AB98_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The timeout in seconds.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTCAE6AB98_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite         The child.

- **`Wait Constructor (TimeSpan, Composite)`**
  ```csharp
  public Wait(
TimeSpan timeout,
Composite child
)
  ```
  Constructor.
  - *timeout*: Type: SystemAddLanguageSpecificTextSet("LST81E59435_0?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe timeout.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST81E59435_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite  The child.

- **`Wait Constructor (WaitGetTimeoutDelegate, Composite)`**
  ```csharp
  public Wait(
WaitGetTimeoutDelegate timeoutRetriever,
Composite child
)
  ```
  Constructor.
  - *timeoutRetriever*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST70C0461A_0?cs=.|vb=.|cpp=::|nu=.|fs=.");WaitGetTimeoutDelegateThe timeout retriever.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST70C0461A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite           The child.

- **`Wait Constructor (Int32, CanRunDecoratorDelegate, Composite)`**
  ```csharp
  public Wait(
int timeoutSeconds,
CanRunDecoratorDelegate runFunc,
Composite child
)
  ```
  Constructor.
  - *timeoutSeconds*: Type: SystemAddLanguageSpecificTextSet("LSTB18647C3_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The timeout in seconds.
  - *runFunc*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTB18647C3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate       The run function.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTB18647C3_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite         The child.

- **`Wait Constructor (TimeSpan, CanRunDecoratorDelegate, Composite)`**
  ```csharp
  public Wait(
TimeSpan timeout,
CanRunDecoratorDelegate runFunc,
Composite child
)
  ```
  Constructor.
  - *timeout*: Type: SystemAddLanguageSpecificTextSet("LST92292C26_0?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe timeout.
  - *runFunc*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST92292C26_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegateThe run function.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST92292C26_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite  The child.

- **`Wait Constructor (WaitGetTimeoutDelegate, CanRunDecoratorDelegate, Composite)`**
  ```csharp
  public Wait(
WaitGetTimeoutDelegate timeoutRetriever,
CanRunDecoratorDelegate runFunc,
Composite child
)
  ```
  Constructor.
  - *timeoutRetriever*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTDC920BDB_0?cs=.|vb=.|cpp=::|nu=.|fs=.");WaitGetTimeoutDelegateThe timeout retriever.
  - *runFunc*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTDC920BDB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate         The run function.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTDC920BDB_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite           The child.

#### Wait Properties

- **`Timeout Property`**
  ```csharp
  public TimeSpan Timeout { get; set; }
  ```
  Gets or sets the timeout.

#### Wait Methods

- **`Execute Method`**
  ```csharp
  protected override IEnumerable&lt;RunStatus&gt; Execute(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTF412AF8A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** See Also

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
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTEAAB85F9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object    The context.The context.

- **`Stop Method`**
  ```csharp
  public override void Stop(
Object context
)
  ```
  Stop will call all cleanup handlers!
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST869C59DD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe context.

#### Wait Fields

- **`End Field`**
  ```csharp
  protected DateTime End
  ```

---

### WaitContinue Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → Styx.TreeSharp.Wait → Styx.TreeSharp.WaitContinue → Bots.DungeonBuddy.Helpers.WaitContinue.T → CommonBehaviors.WaitLuaEvent

```csharp
public class WaitContinue : Wait
```

A wait continue.

#### WaitContinue Constructor

- **`WaitContinue Constructor (Int32, Composite)`**
  ```csharp
  public WaitContinue(
int timeoutSeconds,
Composite child
)
  ```
  Constructor.
  - *timeoutSeconds*: Type: SystemAddLanguageSpecificTextSet("LSTA0513CD3_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The timeout in seconds.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTA0513CD3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite         The child.

- **`WaitContinue Constructor (TimeSpan, Composite)`**
  ```csharp
  public WaitContinue(
TimeSpan timeout,
Composite child
)
  ```
  Constructor.
  - *timeout*: Type: SystemAddLanguageSpecificTextSet("LST6588D390_0?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe timeout.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST6588D390_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite  The child.

- **`WaitContinue Constructor (WaitGetTimeoutDelegate, Composite)`**
  ```csharp
  public WaitContinue(
WaitGetTimeoutDelegate timeoutRetriever,
Composite child
)
  ```
  Constructor.
  - *timeoutRetriever*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST9B17ADFF_0?cs=.|vb=.|cpp=::|nu=.|fs=.");WaitGetTimeoutDelegateThe timeout retriever.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST9B17ADFF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite           The child.

- **`WaitContinue Constructor (Int32, CanRunDecoratorDelegate, Composite)`**
  ```csharp
  public WaitContinue(
int timeoutSeconds,
CanRunDecoratorDelegate runFunc,
Composite child
)
  ```
  Constructor.
  - *timeoutSeconds*: Type: SystemAddLanguageSpecificTextSet("LST38C9DE08_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The timeout in seconds.
  - *runFunc*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST38C9DE08_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate       The run function.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST38C9DE08_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite         The child.

- **`WaitContinue Constructor (TimeSpan, CanRunDecoratorDelegate, Composite)`**
  ```csharp
  public WaitContinue(
TimeSpan timeout,
CanRunDecoratorDelegate runFunc,
Composite child
)
  ```
  Constructor.
  - *timeout*: Type: SystemAddLanguageSpecificTextSet("LSTD6125961_0?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe timeout.
  - *runFunc*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTD6125961_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegateThe run function.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTD6125961_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite  The child.

- **`WaitContinue Constructor (WaitGetTimeoutDelegate, CanRunDecoratorDelegate, Composite)`**
  ```csharp
  public WaitContinue(
WaitGetTimeoutDelegate timeoutRetriever,
CanRunDecoratorDelegate runFunc,
Composite child
)
  ```
  Constructor.
  - *timeoutRetriever*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST6EF96E0_0?cs=.|vb=.|cpp=::|nu=.|fs=.");WaitGetTimeoutDelegateThe timeout retriever.
  - *runFunc*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST6EF96E0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate         The run function.
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST6EF96E0_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite           The child.

#### WaitContinue Methods

- **`Execute Method`**
  ```csharp
  protected override IEnumerable&lt;RunStatus&gt; Execute(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTB75960CF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** See Also

---

### WaitGetTimeSpanTimeoutDelegate Delegate

```csharp
public delegate TimeSpan WaitGetTimeSpanTimeoutDelegate()
```

Wait get time span timeout delegate.

---

### WaitGetTimeoutDelegate Delegate

```csharp
public delegate int WaitGetTimeoutDelegate()
```

Wait get timeout delegate.

---

### WhileLoop Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.WhileLoop

```csharp
public class WhileLoop : GroupComposite
```

This represents a "While" loop.

#### WhileLoop Constructor

- **`WhileLoop Constructor (CanRunDecoratorDelegate, Composite[])`**
  ```csharp
  public WhileLoop(
CanRunDecoratorDelegate condition,
params Composite[] children
)
  ```
  Constructor.
  - *condition*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST6E1C19E2_2?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegateThe condition.
  - *children*: Type: AddLanguageSpecificTextSet("LST6E1C19E2_3?cpp=array&lt;");Styx.TreeSharpAddLanguageSpecificTextSet("LST6E1C19E2_4?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeAddLanguageSpecificTextSet("LST6E1C19E2_5?cpp=&gt;|vb=()|nu=[]"); A variable-length parameters list containing children.

- **`WhileLoop Constructor (RunStatus, CanRunDecoratorDelegate, Composite[])`**
  ```csharp
  public WhileLoop(
RunStatus returnStatus,
CanRunDecoratorDelegate condition,
params Composite[] children
)
  ```
  Constructor.
  - *returnStatus*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST2C63ABCD_2?cs=.|vb=.|cpp=::|nu=.|fs=.");RunStatusThe return status.
  - *condition*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST2C63ABCD_3?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate   The condition.
  - *children*: Type: AddLanguageSpecificTextSet("LST2C63ABCD_4?cpp=array&lt;");Styx.TreeSharpAddLanguageSpecificTextSet("LST2C63ABCD_5?cs=.|vb=.|cpp=::|nu=.|fs=.");CompositeAddLanguageSpecificTextSet("LST2C63ABCD_6?cpp=&gt;|vb=()|nu=[]");    A variable-length parameters list containing children.

#### WhileLoop Methods

- **`Execute Method`**
  ```csharp
  protected override IEnumerable&lt;RunStatus&gt; Execute(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST396CEA4A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** See Also

---
