# Buddy.Coroutines

Contains the Coroutine class.

## Contents

- [Coroutine Class](#coroutine-class)
- [CoroutineBehaviorException Class](#coroutinebehaviorexception-class)
- [CoroutineException Class](#coroutineexception-class)
- [CoroutineStatus Enumeration](#coroutinestatus-enumeration)
- [CoroutineStoppedException Class](#coroutinestoppedexception-class)
- [CoroutineUnhandledException Class](#coroutineunhandledexception-class)
- [ExternalTaskWaitResult(T) Structure](#externaltaskwaitresultt-structure)

---

### Coroutine Class

**Inheritance:** System.Object → Buddy.Coroutines.Coroutine

```csharp
public sealed class Coroutine : IDisposable
```

Represents a coroutine.

#### Coroutine Constructor

- **`Coroutine Constructor (Func(Task(Object)))`**
  ```csharp
  public Coroutine(
Func&lt;Task&lt;Object&gt;&gt; taskProducer
)
  ```
  Initializes a new Coroutine with the specified coroutine task producer.
  - *taskProducer*: Type: SystemAddLanguageSpecificTextSet("LSTE90101D1_4?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTE90101D1_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TaskAddLanguageSpecificTextSet("LSTE90101D1_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");ObjectAddLanguageSpecificTextSet("LSTE90101D1_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");AddLanguageSpecificTextSet("LSTE90101D1_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");A producer that kicks off the coroutine task and returns it. The result returned by the task is saved in Result when the coroutine finishes.

- **`Coroutine Constructor (Func(Task))`**
  ```csharp
  public Coroutine(
Func&lt;Task&gt; taskProducer
)
  ```
  Initializes a new Coroutine with the specified coroutine task producer.
  - *taskProducer*: Type: SystemAddLanguageSpecificTextSet("LST348EC581_2?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST348EC581_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TaskAddLanguageSpecificTextSet("LST348EC581_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");A producer that kicks off the coroutine task and returns it.

#### Coroutine Properties

- **`Current Property`**
  ```csharp
  public static Coroutine Current { get; }
  ```
  Gets the currently executing coroutine on this thread, or null, if not executing in a coroutine.

- **`FaultingException Property`**
  ```csharp
  public CoroutineException FaultingException { get; }
  ```
  Gets the exception that was thrown when this coroutine faulted.

- **`IsDisposed Property`**
  ```csharp
  public bool IsDisposed { get; }
  ```
  Gets a value indicating whether this coroutine has been disposed of.

- **`IsFinished Property`**
  ```csharp
  public bool IsFinished { get; }
  ```
  Gets a bool that indicates whether the coroutine is finished

- **`Result Property`**
  ```csharp
  public Object Result { get; }
  ```
  Gets the result returned by the original task.

- **`Status Property`**
  ```csharp
  public CoroutineStatus Status { get; }
  ```
  Gets the status this coroutine is currently in.

- **`Ticks Property`**
  ```csharp
  public int Ticks { get; }
  ```
  Gets the amount of times the coroutine has been resumed/ticked.

#### Coroutine Methods

- **`Dispose Method`**
  ```csharp
  public void Dispose()
  ```
  Disposes this coroutine.

- **`ExternalTask Method`**
  Gets a coroutine task that waits for completion of a produced external task.

- **`ExternalTask(T) Method (Func(Task(T)))`**
  ```csharp
  public static Task&lt;T&gt; ExternalTask&lt;T&gt;(
Func&lt;Task&lt;T&gt;&gt; taskProducer
)
  ```
  Gets a coroutine task that waits for completion of a produced external task.
  - *taskProducer*: Type: SystemAddLanguageSpecificTextSet("LSTA77F858B_7?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTA77F858B_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TaskAddLanguageSpecificTextSet("LSTA77F858B_9?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTA77F858B_10?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");AddLanguageSpecificTextSet("LSTA77F858B_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");A function that constructs the external task.
  - **Returns:** Exceptions

- **`ExternalTask Method (Func(Task))`**
  ```csharp
  public static Task ExternalTask(
Func&lt;Task&gt; taskProducer
)
  ```
  Gets a coroutine task that waits for completion of a produced external task.
  - *taskProducer*: Type: SystemAddLanguageSpecificTextSet("LST2FB7E3A0_3?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST2FB7E3A0_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TaskAddLanguageSpecificTextSet("LST2FB7E3A0_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");A function that constructs the external task.
  - **Returns:** ExceptionConditionArgumentNullExceptionThrown if taskProducer is null.ArgumentExceptionThrown if taskProducer produces a null value.

- **`ExternalTask Method (Task)`**
  ```csharp
  public static Task ExternalTask(
Task externalTask
)
  ```
  Gets a coroutine task that waits for completion of an external task (a task not running as a coroutine).
  - *externalTask*: Type: System.Threading.TasksAddLanguageSpecificTextSet("LST3A172697_1?cs=.|vb=.|cpp=::|nu=.|fs=.");TaskThe external task.
  - **Returns:** ExceptionConditionArgumentNullExceptionThrown if externalTask is null.InvalidOperationExceptionThrown if the function is not executing in a coroutine, or if it has already obtained a coroutine task this tick.

- **`ExternalTask(T) Method (Task(T))`**
  ```csharp
  public static Task&lt;T&gt; ExternalTask&lt;T&gt;(
Task&lt;T&gt; externalTask
)
  ```
  Gets a coroutine task that waits for completion of an external task (a task not running as a coroutine).
  - *externalTask*: Type: System.Threading.TasksAddLanguageSpecificTextSet("LST39165EE4_5?cs=.|vb=.|cpp=::|nu=.|fs=.");TaskAddLanguageSpecificTextSet("LST39165EE4_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST39165EE4_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The external task.
  - *T*: The return type of the external task.
  - **Returns:** Exceptions

- **`ExternalTask Method (Task, Int32)`**
  ```csharp
  [ObsoleteAttribute("Timeouts should be handled in the external task, not by this method. Use the overloads with infinite timeouts instead.")]
public static Task&lt;bool&gt; ExternalTask(
Task externalTask,
int millisecondsTimeout
)
  ```
  Gets a coroutine task that waits for completion of an external task (a task not running as a coroutine).
  - *externalTask*: Type: System.Threading.TasksAddLanguageSpecificTextSet("LST8DC6D56C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");TaskThe external task.
  - *millisecondsTimeout*: Type: SystemAddLanguageSpecificTextSet("LST8DC6D56C_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The max time to wait for the external task to complete, in milliseconds.
  - **Returns:** Exceptions

- **`ExternalTask Method (Task, TimeSpan)`**
  ```csharp
  [ObsoleteAttribute("Timeouts should be handled in the external task, not by this method. Use the overloads with infinite timeouts instead.")]
public static Task&lt;bool&gt; ExternalTask(
Task externalTask,
TimeSpan timeout
)
  ```
  Gets a coroutine task that waits for completion of an external task (a task not running as a coroutine).
  - *externalTask*: Type: System.Threading.TasksAddLanguageSpecificTextSet("LSTC66DD0FD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");TaskThe external task.
  - *timeout*: Type: SystemAddLanguageSpecificTextSet("LSTC66DD0FD_2?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe max time to wait for the external task to complete.
  - **Returns:** Exceptions

- **`ExternalTask(T) Method (Task(T), Int32)`**
  ```csharp
  [ObsoleteAttribute("Timeouts should be handled in the external task, not by this method. Use the overloads with infinite timeouts instead.")]
public static Task&lt;ExternalTaskWaitResult&lt;T&gt;&gt; ExternalTask&lt;T&gt;(
Task&lt;T&gt; externalTask,
int millisecondsTimeout
)
  ```
  Gets a coroutine task that waits for completion of an external task (a task not running as a coroutine).
  - *externalTask*: Type: System.Threading.TasksAddLanguageSpecificTextSet("LSTB44FC4D_5?cs=.|vb=.|cpp=::|nu=.|fs=.");TaskAddLanguageSpecificTextSet("LSTB44FC4D_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTB44FC4D_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The external task.
  - *millisecondsTimeout*: Type: SystemAddLanguageSpecificTextSet("LSTB44FC4D_8?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The max time to wait for the external task to complete, in milliseconds.
  - *T*: The return type of the external task.
  - **Returns:** Exceptions

- **`ExternalTask(T) Method (Task(T), TimeSpan)`**
  ```csharp
  [ObsoleteAttribute("Timeouts should be handled in the external taks, not by this method. Use the overloads with infinite timeouts instead.")]
public static Task&lt;ExternalTaskWaitResult&lt;T&gt;&gt; ExternalTask&lt;T&gt;(
Task&lt;T&gt; externalTask,
TimeSpan timeout
)
  ```
  Gets a coroutine task that waits for completion of an external task (a task not running as a coroutine).
  - *externalTask*: Type: System.Threading.TasksAddLanguageSpecificTextSet("LST22812140_5?cs=.|vb=.|cpp=::|nu=.|fs=.");TaskAddLanguageSpecificTextSet("LST22812140_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST22812140_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The external task.
  - *timeout*: Type: SystemAddLanguageSpecificTextSet("LST22812140_8?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe max time to wait for the external task to complete.
  - *T*: The return type of the external task.
  - **Returns:** Exceptions

- **`Resume Method`**
  ```csharp
  public void Resume()
  ```
  Resumes the coroutine.

- **`Sleep Method`**
  Gets a coroutine task that sleeps for the specified amount of milliseconds.

- **`Sleep Method (Int32)`**
  ```csharp
  public static Task Sleep(
int milliseconds
)
  ```
  Gets a coroutine task that sleeps for the specified amount of milliseconds.
  - *milliseconds*: Type: SystemAddLanguageSpecificTextSet("LSTDB8372A3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The amount of milliseconds to sleep.
  - **Returns:** ExceptionConditionInvalidOperationExceptionThrown if the function is not executing in a coroutine, or if it has already obtained a coroutine task this tick.ArgumentOutOfRangeExceptionThrown if milliseconds is negative and not equal to Infinite.

- **`Sleep Method (TimeSpan)`**
  ```csharp
  public static Task Sleep(
TimeSpan timeout
)
  ```
  Gets a coroutine task that sleeps for the specified timeout.
  - *timeout*: Type: SystemAddLanguageSpecificTextSet("LST18F8903C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe timeout to sleep.
  - **Returns:** ExceptionConditionInvalidOperationExceptionThrown if the function is not executing in a coroutine, or if it has already obtained a coroutine task this tick.ArgumentOutOfRangeExceptionThrown if timeout is negative and not equal to InfiniteTimeSpan.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Gets a string representation of this coroutine.
  - **Returns:** ReferenceCoroutine Class

- **`Wait Method`**
  Gets a coroutine task that waits for the specified condition to become true, for up to the specified max time.
            Returns true if the condition becomes true before the max wait time is over.

- **`Wait Method (Int32, Func(Boolean))`**
  ```csharp
  public static Task&lt;bool&gt; Wait(
int maxWaitMs,
Func&lt;bool&gt; condition
)
  ```
  Gets a coroutine task that waits for the specified condition to become true, for up to the specified max time.
            Returns true if the condition becomes true before the max wait time is over.
  - *maxWaitMs*: Type: SystemAddLanguageSpecificTextSet("LST1E5BB20B_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The max time to wait, in milliseconds, or Timeout.Infinite for an infinite wait.
  - *condition*: Type: SystemAddLanguageSpecificTextSet("LST1E5BB20B_4?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST1E5BB20B_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST1E5BB20B_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - **Returns:** Exceptions

- **`Wait Method (TimeSpan, Func(Boolean))`**
  ```csharp
  public static Task&lt;bool&gt; Wait(
TimeSpan maxWaitTimeout,
Func&lt;bool&gt; condition
)
  ```
  Gets a coroutine task that waits for the specified condition to become true, for up to the specified max time.
            Returns true if the condition becomes true before the max wait time is over.
  - *maxWaitTimeout*: Type: SystemAddLanguageSpecificTextSet("LST584E75C_3?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe max time to wait, or Timeout.InfiniteTimeSpan for an infinite wait.
  - *condition*: Type: SystemAddLanguageSpecificTextSet("LST584E75C_4?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST584E75C_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST584E75C_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - **Returns:** Exceptions

- **`Yield Method`**
  ```csharp
  public static Task Yield()
  ```
  Yields back to the coroutine, executing the rest of the current function in the next tick.
  - **Returns:** ExceptionConditionInvalidOperationExceptionThrown if the function is not executing in a coroutine, or if it has already obtained a coroutine task this tick.

---

### CoroutineBehaviorException Class

**Inheritance:** System.Object → System.Exception → Buddy.Coroutines.CoroutineException → Buddy.Coroutines.CoroutineBehaviorException

```csharp
public class CoroutineBehaviorException : CoroutineException
```

Represents an exception that is thrown when a coroutine behaves unexpectedly.

#### CoroutineBehaviorException Constructor

- **`CoroutineBehaviorException Constructor (String)`**
  ```csharp
  public CoroutineBehaviorException(
string message
)
  ```
  Initializes a new CoroutineBehaviorException with the specified message.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST6E2C9661_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.

- **`CoroutineBehaviorException Constructor (String, Exception)`**
  ```csharp
  public CoroutineBehaviorException(
string message,
Exception innerException
)
  ```
  Initializes a new CoroutineBehaviorException with the specified message and inner exception.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTEC89ED9_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.
  - *innerException*: Type: SystemAddLanguageSpecificTextSet("LSTEC89ED9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ExceptionThe inner exception.

---

### CoroutineException Class

**Inheritance:** System.Object → System.Exception → Buddy.Coroutines.CoroutineException → Buddy.Coroutines.CoroutineBehaviorException → Buddy.Coroutines.CoroutineUnhandledException

```csharp
public abstract class CoroutineException : Exception
```

Represents the base class for exceptions thrown by the coroutine framework.

#### CoroutineException Constructor

- **`CoroutineException Constructor (String)`**
  ```csharp
  protected CoroutineException(
string message
)
  ```
  Initializes a new CoroutineException with the specified message.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST2F48A8E5_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.

- **`CoroutineException Constructor (String, Exception)`**
  ```csharp
  protected CoroutineException(
string message,
Exception innerException
)
  ```
  Initializes a new CoroutineException with the specified message and inner exception.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST423FBA45_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.
  - *innerException*: Type: SystemAddLanguageSpecificTextSet("LST423FBA45_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ExceptionThe inner exception.

---

### CoroutineStatus Enumeration

```csharp
public enum CoroutineStatus
```

Represents the different states a coroutine can be in.

---

### CoroutineStoppedException Class

**Inheritance:** System.Object → System.Exception → Buddy.Coroutines.CoroutineStoppedException

```csharp
public class CoroutineStoppedException : Exception
```

Represents an exception that is thrown when the coroutine is stopped.

---

### CoroutineUnhandledException Class

**Inheritance:** System.Object → System.Exception → Buddy.Coroutines.CoroutineException → Buddy.Coroutines.CoroutineUnhandledException

```csharp
public class CoroutineUnhandledException : CoroutineException
```

Represents an exception that is thrown when a coroutine throws an exception.

#### CoroutineUnhandledException Constructor

- **`CoroutineUnhandledException Constructor (String)`**
  ```csharp
  public CoroutineUnhandledException(
string message
)
  ```
  Initializes a new CoroutineUnhandledException with the specified message.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTF31AA08C_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.

- **`CoroutineUnhandledException Constructor (String, Exception)`**
  ```csharp
  public CoroutineUnhandledException(
string message,
Exception innerException
)
  ```
  Initializes a new CoroutineUnhandledException with the specified message and inner exception.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTF273F694_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.
  - *innerException*: Type: SystemAddLanguageSpecificTextSet("LSTF273F694_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ExceptionThe inner exception.

---

### ExternalTaskWaitResult(T) Structure

```csharp
public struct ExternalTaskWaitResult&lt;T&gt;
```

Represents the result of an external task wait.

#### ExternalTaskWaitResult(T) Properties

- **`Completed Property`**
  ```csharp
  public bool Completed { get; }
  ```
  Gets a value that indicates whether the task completed in the allotted timeout period.

- **`Result Property`**
  ```csharp
  public T Result { get; }
  ```
  Gets the result of the external task. Only valid if Completed is true.

---
