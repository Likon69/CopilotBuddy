# Styx.CommonBot.Routines

Contains routine related classes.

## Contents

- [CapabilityFlags Enumeration](#capabilityflags-enumeration)
- [CapabilityManager Class](#capabilitymanager-class)
- [CapabilityManagerHandle Class](#capabilitymanagerhandle-class)
- [CapabilityState Enumeration](#capabilitystate-enumeration)
- [CapabilityStateChangedArgs Class](#capabilitystatechangedargs-class)
- [CombatRoutine Class](#combatroutine-class)
- [InvalidRoutineWrapper Class](#invalidroutinewrapper-class)
- [RoutineManager Class](#routinemanager-class)

---

### CapabilityFlags Enumeration

```csharp
[FlagsAttribute]
public enum CapabilityFlags
```

Combat Routine's capabilities.

---

### CapabilityManager Class

**Inheritance:** System.Object → Styx.CommonBot.Routines.CapabilityManager

```csharp
public class CapabilityManager
```

This manager allows multiple sources to disallow a CombatRoutine capability for specific time or until a condition evaluates to false and once no sources are disallowing a capability, that capability is then allowed

#### CapabilityManager Properties

- **`Instance Property`**
  ```csharp
  public static CapabilityManager Instance { get; }
  ```
  Gets the instance.

#### CapabilityManager Methods

- **`Add Method`**
  Adds a new entry that disallows capability untilcondition evaluates to false, at which point the entry is removed and capability is allowed if no other entries for capability are maintained.

- **`Add Method (CapabilityFlags, Func(Boolean), String)`**
  ```csharp
  public CapabilityManagerHandle Add(
CapabilityFlags capability,
Func&lt;bool&gt; condition,
string reason = null
)
  ```
  Adds a new entry that disallows capability untilcondition evaluates to false, at which point the entry is removed and capability is allowed if no other entries for capability are maintained.
  - *capability*: Type: Styx.CommonBot.RoutinesAddLanguageSpecificTextSet("LST495FD217_3?cs=.|vb=.|cpp=::|nu=.|fs=.");CapabilityFlagsThe capability.
  - *condition*: Type: SystemAddLanguageSpecificTextSet("LST495FD217_4?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST495FD217_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST495FD217_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - **Returns:** ReferenceCapabilityManager Class

- **`Add Method (CapabilityFlags, Int32, String)`**
  ```csharp
  public CapabilityManagerHandle Add(
CapabilityFlags capability,
int timeSpanMs,
string reason = null
)
  ```
  Adds a new entry that disallows capability fortimeSpanMs duration in milliseconds, at which point the entry is removed andcapability is allowed if no other entries for capability are maintained.
  - *capability*: Type: Styx.CommonBot.RoutinesAddLanguageSpecificTextSet("LST1158A4D8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CapabilityFlagsThe capability.
  - *timeSpanMs*: Type: SystemAddLanguageSpecificTextSet("LST1158A4D8_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The timeSpanMs.
  - **Returns:** ExceptionConditionOverflowExceptiontimeSpanMs is less than MinValue or greater than
                MaxValue.-or-timeSpanMs is
                PositiveInfinity.-or-timeSpanMs is
                NegativeInfinity.

- **`Add Method (CapabilityFlags, TimeSpan, String)`**
  ```csharp
  public CapabilityManagerHandle Add(
CapabilityFlags capability,
TimeSpan timeSpan,
string reason = null
)
  ```
  Adds a new entry that disallows capability fortimeSpan duration, at which point the entry is removed and capability is allowed if no other entries for capability are maintained.
  - *capability*: Type: Styx.CommonBot.RoutinesAddLanguageSpecificTextSet("LSTBFCBB09D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CapabilityFlagsThe capability.
  - *timeSpan*: Type: SystemAddLanguageSpecificTextSet("LSTBFCBB09D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe timeSpan.
  - **Returns:** ReferenceCapabilityManager Class

- **`Clear Method`**
  Clears all the entries that matches any of capabilities.

- **`Clear Method (CapabilityFlags, String)`**
  ```csharp
  public void Clear(
CapabilityFlags capabilities = CapabilityFlags.All,
string reason = null
)
  ```
  Clears all the entries that matches any of capabilities.

- **`Clear Method (CapabilityManagerHandle, CapabilityFlags, String)`**
  ```csharp
  public void Clear(
CapabilityManagerHandle handle,
CapabilityFlags capabilities = CapabilityFlags.All,
string reason = null
)
  ```
  Clears all the entries that matches any of capabilitiesand are associated with handle or all matching entries if handle is null.
  - *handle*: Type: Styx.CommonBot.RoutinesAddLanguageSpecificTextSet("LST5A6AD143_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CapabilityManagerHandleThe handle.

- **`CreateNewHandle Method`**
  ```csharp
  public CapabilityManagerHandle CreateNewHandle()
  ```
  Creates a new handle that can be used to add or update an entry
  - **Returns:** ReferenceCapabilityManager Class

- **`Pulse Method`**
  ```csharp
  public void Pulse()
  ```

- **`Update Method`**
  Updates an entry (or adds a new one if entry is not present) to disallowcapability until condition evaluates to false, at which  point the entry is removed and capabilityis allowed if no other entries for capability are maintained.

- **`Update Method (CapabilityManagerHandle, CapabilityFlags, Func(Boolean), String)`**
  ```csharp
  public void Update(
CapabilityManagerHandle handle,
CapabilityFlags capability,
Func&lt;bool&gt; condition,
string reason = null
)
  ```
  Updates an entry (or adds a new one if entry is not present) to disallowcapability until condition evaluates to false, at which  point the entry is removed and capabilityis allowed if no other entries for capability are maintained.
  - *handle*: Type: Styx.CommonBot.RoutinesAddLanguageSpecificTextSet("LST60B0005F_3?cs=.|vb=.|cpp=::|nu=.|fs=.");CapabilityManagerHandleThe handle.
  - *capability*: Type: Styx.CommonBot.RoutinesAddLanguageSpecificTextSet("LST60B0005F_4?cs=.|vb=.|cpp=::|nu=.|fs=.");CapabilityFlagsThe capability.
  - *condition*: Type: SystemAddLanguageSpecificTextSet("LST60B0005F_5?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST60B0005F_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST60B0005F_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - **Returns:** ExceptionConditionArgumentNullExceptioncondition

- **`Update Method (CapabilityManagerHandle, CapabilityFlags, Int32, String)`**
  ```csharp
  public void Update(
CapabilityManagerHandle handle,
CapabilityFlags capability,
int timeSpanMs,
string reason = null
)
  ```
  Updates an entry (or adds a new one if entry is not present) to disallow capability fortimeSpanMs duration in milliseconds. The entry is removed upon expiration andcapability is allowed if no other entries for capability are maintained.
  - *handle*: Type: Styx.CommonBot.RoutinesAddLanguageSpecificTextSet("LST35BBAEAC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CapabilityManagerHandleThe handle.
  - *capability*: Type: Styx.CommonBot.RoutinesAddLanguageSpecificTextSet("LST35BBAEAC_2?cs=.|vb=.|cpp=::|nu=.|fs=.");CapabilityFlagsThe capability.
  - *timeSpanMs*: Type: SystemAddLanguageSpecificTextSet("LST35BBAEAC_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The timeSpanMs.

- **`Update Method (CapabilityManagerHandle, CapabilityFlags, TimeSpan, String)`**
  ```csharp
  public void Update(
CapabilityManagerHandle handle,
CapabilityFlags capability,
TimeSpan timeSpan,
string reason = null
)
  ```
  Updates an entry (or adds a new one if entry is not present) to disallow capability fortimeSpan duration. The entry is removed upon expiration and capability is allowed if no other entries for capability are maintained.
  - *handle*: Type: Styx.CommonBot.RoutinesAddLanguageSpecificTextSet("LST712CE451_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CapabilityManagerHandleThe handle.
  - *capability*: Type: Styx.CommonBot.RoutinesAddLanguageSpecificTextSet("LST712CE451_2?cs=.|vb=.|cpp=::|nu=.|fs=.");CapabilityFlagsThe capability.
  - *timeSpan*: Type: SystemAddLanguageSpecificTextSet("LST712CE451_3?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe timeSpan.

---

### CapabilityManagerHandle Class

**Inheritance:** System.Object → Styx.CommonBot.Routines.CapabilityManagerHandle

```csharp
public class CapabilityManagerHandle
```

Determines whether the specified object is equal to the current object.

---

### CapabilityState Enumeration

```csharp
public enum CapabilityState
```

Represents the state of capabilities, as requested by the user or bot.

---

### CapabilityStateChangedArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.CommonBot.Routines.CapabilityStateChangedArgs

```csharp
public sealed class CapabilityStateChangedArgs : EventArgs
```

Initializes a new instance of the CapabilityStateChangedArgs class

#### CapabilityStateChangedArgs Properties

- **`Capability Property`**
  ```csharp
  public CapabilityFlags Capability { get; }
  ```
  Gets the capability.

- **`NewState Property`**
  ```csharp
  public CapabilityState NewState { get; }
  ```
  Gets the new state.

- **`OldState Property`**
  ```csharp
  public CapabilityState OldState { get; }
  ```
  Gets the old state.

---

### CombatRoutine Class

**Inheritance:** System.Object → Styx.CommonBot.Routines.CombatRoutine → Styx.CommonBot.Routines.InvalidRoutineWrapper

```csharp
public abstract class CombatRoutine
```

A combat routine.

#### CombatRoutine Properties

- **`ButtonText Property`**
  ```csharp
  public string ButtonText { get; }
  ```
  The text to appear on the button in the form. [Default: "CC Configuration"].

- **`Class Property`**
  ```csharp
  public abstract WoWClass Class { get; }
  ```
  The WoWClass to be used with this routine.

- **`CombatBehavior Property`**
  ```csharp
  public virtual Composite CombatBehavior { get; }
  ```
  Behavior used in combat.

- **`CombatBuffBehavior Property`**
  ```csharp
  public virtual Composite CombatBuffBehavior { get; }
  ```
  Behavior used for combat buffs. eg; 'Horn of Winter', 'Power Infusion' etc.

- **`DeathBehavior Property`**
  ```csharp
  public virtual Composite DeathBehavior { get; }
  ```
  Behavior used when death. This should be used to handle things like Soulstones etc.
            This will block the execution of DeathBehavior of the bot when it returns RunStatus.Success

- **`HealBehavior Property`**
  ```csharp
  public virtual Composite HealBehavior { get; }
  ```
  Behavior used when healing.

- **`MoveToTargetBehavior Property`**
  ```csharp
  public virtual Composite MoveToTargetBehavior { get; }
  ```
  Behavior used for moving to targets.

- **`Name Property`**
  ```csharp
  public abstract string Name { get; }
  ```
  The name of this CombatRoutine.

- **`NeedCombatBuffs Property`**
  ```csharp
  public virtual bool NeedCombatBuffs { get; }
  ```
  Property indicating if you need combat buffs.

- **`NeedDeath Property`**
  ```csharp
  public virtual bool NeedDeath { get; }
  ```
  Whether or not to handle being dead. For casting special abilities (Soulstone, Ankh,
            etc)

- **`NeedHeal Property`**
  ```csharp
  public virtual bool NeedHeal { get; }
  ```
  Property indicating if you to heal.

- **`NeedPreCombatBuffs Property`**
  ```csharp
  public virtual bool NeedPreCombatBuffs { get; }
  ```
  Property indicating if you need PreCombatBuffs.

- **`NeedPullBuffs Property`**
  ```csharp
  public virtual bool NeedPullBuffs { get; }
  ```
  Property indicating if you need Pull Buffs.

- **`NeedRest Property`**
  ```csharp
  public virtual bool NeedRest { get; }
  ```
  Property indicating if you need to rest.

- **`PreCombatBuffBehavior Property`**
  ```csharp
  public virtual Composite PreCombatBuffBehavior { get; }
  ```
  Behavior used for buffing, regular buffs like 'Power Word: Fortitude', 'MotW' etc.

- **`PullBehavior Property`**
  ```csharp
  public virtual Composite PullBehavior { get; }
  ```
  Behavior used when engaging mobs in combat.

- **`PullBuffBehavior Property`**
  ```csharp
  public virtual Composite PullBuffBehavior { get; }
  ```
  Behavior used when buffing prior to pulling.

- **`PullDistance Property`**
  ```csharp
  public virtual double PullDistance { get; }
  ```
  Gets the pull distance. This is the range at which the combat routine is able to pull a mob.

- **`RestBehavior Property`**
  ```csharp
  public virtual Composite RestBehavior { get; }
  ```
  Behavior used when resting.

- **`SupportedCapabilities Property`**
  ```csharp
  public abstract CapabilityFlags SupportedCapabilities { get; }
  ```
  Gets the currently supported capabilities ORed together.
            This should use the current dynamic state of the capabilities but 
            not the state returned by GetCapabilityState(CapabilityFlags)

- **`WantButton Property`**
  ```csharp
  public virtual bool WantButton { get; }
  ```
  Whether this CC want the button on the form to be enabled.

#### CombatRoutine Methods

- **`Combat Method`**
  ```csharp
  public virtual void Combat()
  ```
  Combat actions.

- **`CombatBuff Method`**
  ```csharp
  public virtual void CombatBuff()
  ```
  Combat buff actions.

- **`Death Method`**
  ```csharp
  public virtual void Death()
  ```
  Death actions.

- **`Dispose Method`**
  ```csharp
  public void Dispose()
  ```
  Performs application-defined tasks associated with freeing, releasing, or resetting
            unmanaged resources.

- **`GetCapabilityState Method`**
  ```csharp
  protected CapabilityState GetCapabilityState(
CapabilityFlags capability
)
  ```
  Returns the state of a capability. This can only be used by the combat routine
  - *capability*: Type: Styx.CommonBot.RoutinesAddLanguageSpecificTextSet("LST8018E596_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CapabilityFlagsThe capability.
  - **Returns:** See GetCapabilityState(CapabilityFlags).

- **`HasCapability Method`**
  ```csharp
  public bool HasCapability(
CapabilityFlags capability
)
  ```
  Determines whether the Combat Routine has the given capability/capabilities.
            It is assumed that capabilities can be toggled if available.
  - *capability*: Type: Styx.CommonBot.RoutinesAddLanguageSpecificTextSet("LSTBF398531_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CapabilityFlagsThe capability.
  - **Returns:** See Also

- **`Heal Method`**
  ```csharp
  public virtual void Heal()
  ```
  Heal actions.

- **`Initialize Method`**
  ```csharp
  public virtual void Initialize()
  ```
  Called when this CC is selected as the current CC.

- **`OnButtonPress Method`**
  ```csharp
  public virtual void OnButtonPress()
  ```
  Called when the button for this CC is pressed.

- **`PreCombatBuff Method`**
  ```csharp
  public virtual void PreCombatBuff()
  ```
  PreCombatBuff actions.

- **`Pull Method`**
  ```csharp
  public virtual void Pull()
  ```
  Pull actions.

- **`PullBuff Method`**
  ```csharp
  public virtual void PullBuff()
  ```
  Pull Buff actions.

- **`Pulse Method`**
  ```csharp
  public virtual void Pulse()
  ```
  Called in every pulse of the bot. This way you can maintain stuff per-pulse like a
            plugin.

- **`Rest Method`**
  ```csharp
  public virtual void Rest()
  ```
  Rest actions.

- **`ShutDown Method`**
  ```csharp
  public virtual void ShutDown()
  ```
  Called when this routine is disposed.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceCombatRoutine Class

---

### InvalidRoutineWrapper Class

**Inheritance:** System.Object → Styx.CommonBot.Routines.CombatRoutine → Styx.CommonBot.Routines.InvalidRoutineWrapper

```csharp
public sealed class InvalidRoutineWrapper : CombatRoutine
```

An invalid routine wrapper.

#### InvalidRoutineWrapper Properties

- **`Class Property`**
  ```csharp
  public override WoWClass Class { get; }
  ```
  The WoWClass to be used with this routine.

- **`Name Property`**
  ```csharp
  public override string Name { get; }
  ```
  The name of this CombatRoutine.

- **`SupportedCapabilities Property`**
  ```csharp
  public override CapabilityFlags SupportedCapabilities { get; }
  ```

#### InvalidRoutineWrapper Methods

- **`Initialize Method`**
  ```csharp
  public override void Initialize()
  ```
  Called when this CC is selected as the current CC.

---

### RoutineManager Class

**Inheritance:** System.Object → Styx.CommonBot.Routines.RoutineManager

```csharp
public static class RoutineManager
```

/// This class just lets us do some things to make sure we always have a valid
            routine, but don't break the bot. /// It's a good thing, I swear! ///.

#### RoutineManager Properties

- **`AllRoutines Property`**
  ```csharp
  public static List&lt;CombatRoutine&gt; AllRoutines { get; set; }
  ```
  Gets or sets a list of all loaded routines, for every class.

- **`CommandLineRoutine Property`**
  ```csharp
  public static string CommandLineRoutine { get; }
  ```
  Gets the command line routine.

- **`Current Property`**
  ```csharp
  public static CombatRoutine Current { get; set; }
  ```
  Gets or sets the current routine.

- **`RoutineDirectory Property`**
  ```csharp
  public static string RoutineDirectory { get; }
  ```
  Gets the routine directory.

- **`Routines Property`**
  ```csharp
  public static List&lt;CombatRoutine&gt; Routines { get; }
  ```
  Gets a list of routines for the current class.

#### RoutineManager Methods

- **`ChangeRoutine Method`**
  ```csharp
  public static void ChangeRoutine(
string routineNamePartial = null,
bool reloadAssemblies = false
)
  ```
  Change routine.

- **`GetCapabilityState Method`**
  ```csharp
  public static CapabilityState GetCapabilityState(
CapabilityFlags capability
)
  ```
  Returns the state of a capability.
  - *capability*: Type: Styx.CommonBot.RoutinesAddLanguageSpecificTextSet("LST16FE5067_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CapabilityFlagsThe capability. Should not have multiple flags set.
  - **Returns:** ExceptionConditionArgumentOutOfRangeExceptionThrown if capability has multiple flags set.

- **`IsAnyDisallowed Method`**
  ```csharp
  public static bool IsAnyDisallowed(
CapabilityFlags capability
)
  ```
  Determines whether any of the capabilities are disallowed
  - *capability*: Type: Styx.CommonBot.RoutinesAddLanguageSpecificTextSet("LSTD44AD714_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CapabilityFlagsThe capability.
  - **Returns:** ReferenceRoutineManager Class

- **`Reload Method`**
  ```csharp
  public static void Reload()
  ```
  Reloads this object.

- **`ReloadRoutineAssemblies Method`**
  ```csharp
  public static void ReloadRoutineAssemblies()
  ```
  Recompiles any routine assemblies from disk. You should call Reload
            after calling this to ensure the references are correct.

- **`SetCapabilityState Method`**
  ```csharp
  public static void SetCapabilityState(
CapabilityFlags capability,
CapabilityState state,
string reason = null
)
  ```
  Sets the state of the passed in capabilities.
  - *capability*: Type: Styx.CommonBot.RoutinesAddLanguageSpecificTextSet("LST2ECC6BCB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CapabilityFlagsThe capability. Should not have multiple flags set.
  - *state*: Type: Styx.CommonBot.RoutinesAddLanguageSpecificTextSet("LST2ECC6BCB_2?cs=.|vb=.|cpp=::|nu=.|fs=.");CapabilityStateThe new state to set the capability into.

#### RoutineManager Events

- **`OnCapabilityStateChanged Event`**
  ```csharp
  public static event EventHandler&lt;CapabilityStateChangedArgs&gt; OnCapabilityStateChanged
  ```

- **`Reloaded Event`**
  ```csharp
  public static event EventHandler Reloaded
  ```
  Occurs when routines are reloaded.

---
