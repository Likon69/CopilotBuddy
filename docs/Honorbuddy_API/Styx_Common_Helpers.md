# Styx.Common.Helpers

Contains common helpers.-

## Contents

- [AutoTimer Class](#autotimer-class)
- [ByteArray Class](#bytearray-class)
- [WaitTimer Class](#waittimer-class)
- [WaitTimer.WaitTimerEventArgs Class](#waittimer.waittimereventargs-class)
- [WaitTimerFinishedHandler Delegate](#waittimerfinishedhandler-delegate)

---

### AutoTimer Class

**Inheritance:** System.Object → Styx.Common.Helpers.AutoTimer

```csharp
public class AutoTimer : IDisposable
```

An automatic timer.

#### AutoTimer Constructor

- **`AutoTimer Constructor (String)`**
  ```csharp
  public AutoTimer(
string codePart
)
  ```
  Constructor.
  - *codePart*: Type: SystemAddLanguageSpecificTextSet("LST403DF0A3_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe code part.

- **`AutoTimer Constructor (String, Object[])`**
  ```csharp
  public AutoTimer(
string codePartFormat,
params Object[] args
)
  ```
  Constructor.
  - *codePartFormat*: Type: SystemAddLanguageSpecificTextSet("LST486D6D33_2?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe code part format.
  - *args*: Type: AddLanguageSpecificTextSet("LST486D6D33_3?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST486D6D33_4?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST486D6D33_5?cpp=&gt;|vb=()|nu=[]");          A variable-length parameters list containing arguments.

#### AutoTimer Properties

- **`Watch Property`**
  ```csharp
  public Stopwatch Watch { get; }
  ```
  Gets the watch.

#### AutoTimer Methods

- **`Dispose Method`**
  ```csharp
  public void Dispose()
  ```
  Performs application-defined tasks associated with freeing, releasing, or resetting
            unmanaged resources.

---

### ByteArray Class

**Inheritance:** System.Object → Styx.Common.Helpers.ByteArray

```csharp
public static class ByteArray
```

A byte array.

#### ByteArray Methods

- **`Dump Method`**
  ```csharp
  public static string Dump(
byte[] array,
int indent = 0
)
  ```
  Dumps a byte array in a "hex editor" fashin.
  - *array*: Type: AddLanguageSpecificTextSet("LST84FB89D5_1?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST84FB89D5_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ByteAddLanguageSpecificTextSet("LST84FB89D5_3?cpp=&gt;|vb=()|nu=[]"); .
  - **Returns:** ExceptionConditionArgumentNullExceptionThrown when one or more required arguments are null.

---

### WaitTimer Class

**Inheritance:** System.Object → Styx.Common.Helpers.WaitTimer

```csharp
public class WaitTimer
```

A simple timer class to help keep track of waits, and other useful timing info.

#### WaitTimer Properties

- **`EndTime Property`**
  ```csharp
  public DateTime EndTime { get; }
  ```
  The time that this timer will end it's run, and be considered finished.

- **`FiveSeconds Property`**
  ```csharp
  public static WaitTimer FiveSeconds { get; }
  ```
  Gets the five seconds.

- **`IsFinished Property`**
  ```csharp
  public bool IsFinished { get; }
  ```
  Returns whether or not this timer has finished.

- **`OneSecond Property`**
  ```csharp
  public static WaitTimer OneSecond { get; }
  ```
  Gets the one second.

- **`StartTime Property`**
  ```csharp
  public DateTime StartTime { get; }
  ```
  The time this timer was physically started.

- **`TenSeconds Property`**
  ```csharp
  public static WaitTimer TenSeconds { get; }
  ```
  Gets the ten seconds.

- **`ThirtySeconds Property`**
  ```csharp
  public static WaitTimer ThirtySeconds { get; }
  ```
  Gets the thirty seconds.

- **`TimeLeft Property`**
  ```csharp
  public TimeSpan TimeLeft { get; }
  ```
  Returns the time left on this timer before it finishes.

- **`WaitTime Property`**
  ```csharp
  public TimeSpan WaitTime { get; set; }
  ```
  The time to wait before this timer is considered 'finished'.

#### WaitTimer Methods

- **`Reset Method`**
  ```csharp
  public void Reset()
  ```
  Resets the WaitTimer to a time in the future, specified by the
            WaitTime property.

- **`Stop Method`**
  ```csharp
  public void Stop()
  ```
  Stops this timer, and causes the IsFinished property to
            return true.

- **`Update Method`**
  ```csharp
  public void Update()
  ```
  Updates this timer. It must be called fairly frequently if you wish to use the
            Finished event.

#### WaitTimer Events

- **`Finished Event`**
  ```csharp
  public event WaitTimerFinishedHandler Finished
  ```
  Occurs when the wait timer has finished.

---

### WaitTimer.WaitTimerEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.Common.Helpers.WaitTimer.WaitTimerEventArgs

```csharp
public sealed class WaitTimerEventArgs : EventArgs
```

Additional information for wait timer events.

#### WaitTimerEventArgs Properties

- **`TimeFinished Property`**
  ```csharp
  public DateTime TimeFinished { get; set; }
  ```
  The actual time when this event was fired.

- **`TimeStarted Property`**
  ```csharp
  public DateTime TimeStarted { get; set; }
  ```
  The time the timer was physically started.

- **`WaitTime Property`**
  ```csharp
  public TimeSpan WaitTime { get; set; }
  ```
  The time spent waiting.

---

### WaitTimerFinishedHandler Delegate

```csharp
public delegate void WaitTimerFinishedHandler(
Object sender,
WaitTimer.WaitTimerEventArgs e
)
```

Handler, called when the wait timer finished.

---
