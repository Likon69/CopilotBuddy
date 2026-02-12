# Bots.DungeonBuddy.Helpers

Contains DungeonBuddy helpers.

## Contents

- [Action(T) Class](#actiont-class)
- [Alert Class](#alert-class)
- [AlwaysFailAction(T) Class](#alwaysfailactiont-class)
- [Decorator(T) Class](#decoratort-class)
- [DecoratorContinue(T) Class](#decoratorcontinuet-class)
- [DungeonBuddySettings Class](#dungeonbuddysettings-class)
- [DungeonBuddySettings.QueueRole Enumeration](#dungeonbuddysettings.queuerole-enumeration)
- [DungeonPathForkInfo Class](#dungeonpathforkinfo-class)
- [DungeonStuckHandler Class](#dungeonstuckhandler-class)
- [DynamicStringListConverter Class](#dynamicstringlistconverter-class)
- [Error Class](#error-class)
- [ErrorCollection Class](#errorcollection-class)
- [ErrorType Enumeration](#errortype-enumeration)
- [Logger Class](#logger-class)
- [ScenarioCriteria Class](#scenariocriteria-class)
- [ScenarioInfo Class](#scenarioinfo-class)
- [ScenarioStage Class](#scenariostage-class)
- [ScriptHelpers Class](#scripthelpers-class)
- [ScriptHelpers.AngleSpan Structure](#scripthelpers.anglespan-structure)
- [ScriptHelpers.EnemyDispelType Enumeration](#scripthelpers.enemydispeltype-enumeration)
- [ScriptHelpers.GroupRoleFlags Enumeration](#scripthelpers.grouproleflags-enumeration)
- [ScriptHelpers.PartyDispelType Enumeration](#scripthelpers.partydispeltype-enumeration)
- [SpellActionButton Class](#spellactionbutton-class)
- [StrafeManager Class](#strafemanager-class)
- [WaitContinue(T) Class](#waitcontinuet-class)

---

### Action(T) Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → Bots.DungeonBuddy.Helpers.Action.T → Bots.DungeonBuddy.Helpers.AlwaysFailAction.T

```csharp
[ObsoleteAttribute("Use coroutines")]
public class Action&lt;T&gt; : Action
```

Uses a strongly typed context

#### Action(T) Constructor

- **`Action(T) Constructor (Action(T))`**
  ```csharp
  public Action(
Action&lt;T&gt; successRunner
)
  ```
  Initializes a new instance of the Action.T class
  - *successRunner*: Type: SystemAddLanguageSpecificTextSet("LSTBEA814F6_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ActionAddLanguageSpecificTextSet("LSTBEA814F6_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTBEA814F6_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`Action(T) Constructor (Func(T, RunStatus))`**
  ```csharp
  public Action(
Func&lt;T, RunStatus&gt; actionRunner
)
  ```
  Initializes a new instance of the Action.T class
  - *actionRunner*: Type: SystemAddLanguageSpecificTextSet("LST37BF3957_6?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST37BF3957_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");T, RunStatusAddLanguageSpecificTextSet("LST37BF3957_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

#### Action(T) Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST5DFC39BF_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceActionAddLanguageSpecificTextSet("LST5DFC39BF_4?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST5DFC39BF_5?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

#### Action(T) Fields

- **`ActionRunner Field`**
  ```csharp
  protected readonly Func&lt;T, RunStatus&gt; ActionRunner
  ```

- **`SuccessRunner Field`**
  ```csharp
  protected readonly Action&lt;T&gt; SuccessRunner
  ```

---

### Alert Class

**Inheritance:** System.Object → Bots.DungeonBuddy.Helpers.Alert

```csharp
public static class Alert
```

Shows a popup that displays an important message to user before automatically closing after a defined number of
                seconds. If popup is automatically closed then cancelAction (okAction if there is no cancel button
                displayed.) is executed if not null.

#### Alert Methods

- **`Show Method`**
  ```csharp
  public static void Show(
string title,
string message,
int showTimeInSeconds,
bool useAudibleCue = true,
bool showCancelButton = true,
Action okAction = null,
Action cancelAction = null,
string okButtonText = "Continue",
string cancelButtonText = "Stop",
bool temporaryDisableTargeting = true,
bool temporaryDisableMovement = true
)
  ```
  Shows a popup that displays an important message to user before automatically closing after a defined number of
                seconds. If popup is automatically closed then cancelAction (okAction if there is no cancel button
                displayed.) is executed if not null.
  - *title*: Type: SystemAddLanguageSpecificTextSet("LST5B2A27BF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe title.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST5B2A27BF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.
  - *showTimeInSeconds*: Type: SystemAddLanguageSpecificTextSet("LST5B2A27BF_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The show time in seconds.

---

### AlwaysFailAction(T) Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → Bots.DungeonBuddy.Helpers.Action.T → Bots.DungeonBuddy.Helpers.AlwaysFailAction.T

```csharp
[ObsoleteAttribute("Use coroutines")]
public class AlwaysFailAction&lt;T&gt; : Action&lt;T&gt;
```

Always returns RunStatus.Failure after optionally performing an action

#### AlwaysFailAction(T) Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTEEBE9534_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceAlwaysFailActionAddLanguageSpecificTextSet("LSTEEBE9534_4?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LSTEEBE9534_5?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### Decorator(T) Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → Bots.DungeonBuddy.Helpers.Decorator.T

```csharp
[ObsoleteAttribute("Use coroutines")]
public class Decorator&lt;T&gt; : Decorator
```

Uses a strongly typed context

#### Decorator(T) Methods

- **`CanRun Method`**
  ```csharp
  protected override bool CanRun(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTCAB68084_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceDecoratorAddLanguageSpecificTextSet("LSTCAB68084_4?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LSTCAB68084_5?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### DecoratorContinue(T) Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → Styx.TreeSharp.DecoratorContinue → Bots.DungeonBuddy.Helpers.DecoratorContinue.T

```csharp
[ObsoleteAttribute("Use coroutines")]
public class DecoratorContinue&lt;T&gt; : DecoratorContinue
```

Uses a strongly typed context

#### DecoratorContinue(T) Methods

- **`CanRun Method`**
  ```csharp
  protected override bool CanRun(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST1F849ABF_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceDecoratorContinueAddLanguageSpecificTextSet("LST1F849ABF_4?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST1F849ABF_5?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### DungeonBuddySettings Class

**Inheritance:** System.Object → Styx.Helpers.Settings → Bots.DungeonBuddy.Helpers.DungeonBuddySettings

```csharp
public class DungeonBuddySettings : Settings
```

Represents settings used in DungeonBuddy

#### DungeonBuddySettings Properties

- **`AbandonLowLevelQuests Property`**
  ```csharp
  public bool AbandonLowLevelQuests { get; set; }
  ```

- **`CombatMovement Property`**
  ```csharp
  public bool CombatMovement { get; set; }
  ```

- **`CustomProfileAndScriptsPath Property`**
  ```csharp
  public string CustomProfileAndScriptsPath { get; set; }
  ```

- **`DungeonType Property`**
  ```csharp
  public DungeonType DungeonType { get; set; }
  ```

- **`FollowingDistance Property`**
  ```csharp
  public int FollowingDistance { get; set; }
  ```

- **`GroupLootMode Property`**
  ```csharp
  public GroupLootMode GroupLootMode { get; set; }
  ```

- **`HearthOnVendorRun Property`**
  ```csharp
  public bool HearthOnVendorRun { get; set; }
  ```

- **`HearthStoneType Property`**
  ```csharp
  public HearthStoneType HearthStoneType { get; set; }
  ```

- **`InsideInstanceMinFreeBagSlots Property`**
  ```csharp
  public int InsideInstanceMinFreeBagSlots { get; set; }
  ```

- **`InsideInstanceRepairPercent Property`**
  ```csharp
  public float InsideInstanceRepairPercent { get; set; }
  ```

- **`KillOptionalBosses Property`**
  ```csharp
  public bool KillOptionalBosses { get; set; }
  ```

- **`LeaderName Property`**
  ```csharp
  public string LeaderName { get; set; }
  ```

- **`LootMode Property`**
  ```csharp
  public LootMode LootMode { get; set; }
  ```

- **`MailBoeItems Property`**
  ```csharp
  public bool MailBoeItems { get; set; }
  ```

- **`MailItemQuality Property`**
  ```csharp
  public WoWItemQuality MailItemQuality { get; set; }
  ```

- **`OutsideInstanceMinFreeBagSlots Property`**
  ```csharp
  public int OutsideInstanceMinFreeBagSlots { get; set; }
  ```

- **`OutsideInstanceRepairPercent Property`**
  ```csharp
  public float OutsideInstanceRepairPercent { get; set; }
  ```

- **`PartyMembers Property`**
  ```csharp
  public string[] PartyMembers { get; set; }
  ```

- **`PartyMode Property`**
  ```csharp
  public PartyMode PartyMode { get; set; }
  ```

- **`PickupQuests Property`**
  ```csharp
  public bool PickupQuests { get; set; }
  ```

- **`PortOutsideInstanceToBuy Property`**
  ```csharp
  public bool PortOutsideInstanceToBuy { get; set; }
  ```

- **`PortOutsideInstanceToSell Property`**
  ```csharp
  public bool PortOutsideInstanceToSell { get; set; }
  ```

- **`PortOutsideWhenMemberReleasesSpirit Property`**
  ```csharp
  public bool PortOutsideWhenMemberReleasesSpirit { get; set; }
  ```

- **`QueueForBestAlternative Property`**
  ```csharp
  public bool QueueForBestAlternative { get; set; }
  ```

- **`RaidType Property`**
  ```csharp
  public RaidType RaidType { get; set; }
  ```

- **`RepairWithGuildBankFunds Property`**
  ```csharp
  public bool RepairWithGuildBankFunds { get; set; }
  ```

- **`Role Property`**
  ```csharp
  public DungeonBuddySettings.QueueRole Role { get; set; }
  ```

- **`ScenarioType Property`**
  ```csharp
  public ScenarioType ScenarioType { get; set; }
  ```

- **`SelectedDungeonIds Property`**
  ```csharp
  public uint[] SelectedDungeonIds { get; set; }
  ```

- **`SelectedRaidIds Property`**
  ```csharp
  public uint[] SelectedRaidIds { get; set; }
  ```

- **`SelectedRandomDungeonId Property`**
  ```csharp
  public uint SelectedRandomDungeonId { get; set; }
  ```

- **`SelectedScenarioId Property`**
  ```csharp
  public uint SelectedScenarioId { get; set; }
  ```

- **`SellItemQuality Property`**
  ```csharp
  public WoWItemQuality SellItemQuality { get; set; }
  ```

- **`ShowAllDungeons Property`**
  ```csharp
  public bool ShowAllDungeons { get; set; }
  ```

- **`ShowAllScenarios Property`**
  ```csharp
  public bool ShowAllScenarios { get; set; }
  ```

- **`SpecifiedScenariosIds Property`**
  ```csharp
  public uint[] SpecifiedScenariosIds { get; set; }
  ```

- **`TankInRandomGroups Property`**
  ```csharp
  public bool TankInRandomGroups { get; set; }
  ```

- **`UseArtifactPowerItems Property`**
  ```csharp
  public bool UseArtifactPowerItems { get; set; }
  ```

#### DungeonBuddySettings Fields

- **`Instance Field`**
  ```csharp
  public static readonly DungeonBuddySettings Instance
  ```
  Singelton

---

### DungeonBuddySettings.QueueRole Enumeration

```csharp
public enum QueueRole
```

---

### DungeonPathForkInfo Class

**Inheritance:** System.Object → Bots.DungeonBuddy.Helpers.DungeonPathForkInfo

```csharp
public class DungeonPathForkInfo
```

Used for taking one of two possible routes at a fork in a path, that later come together again. 
                 'RightSide' refers to the route that's to the right when facing fork from the entrance side of dungeon 
                 and 'LeftSide' refers to the other route.

#### DungeonPathForkInfo Methods

- **`HandleMovement Method`**
  ```csharp
  public Task&lt;bool&gt; HandleMovement(
Vector3 moveTo
)
  ```
  - *moveTo*: Type: System.NumericsAddLanguageSpecificTextSet("LST9141AD8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - **Returns:** See Also

- **`IsLocationOnEntranceSide Method`**
  ```csharp
  public virtual bool IsLocationOnEntranceSide(
Vector3 location
)
  ```
  Determines whether a location is on the entrance side partition. See remarks for DungeonPathForkInfo for an explaination.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST8ABF9260_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - **Returns:** This just checks which side of the partiton divider the point is on the 2d plane (X/Y).
                You will need to override this function and add additonal checks when dealing with a dungeon that has multiple floors.

---

### DungeonStuckHandler Class

**Inheritance:** System.Object → Bots.DungeonBuddy.Helpers.DungeonStuckHandler

```csharp
public abstract class DungeonStuckHandler
```

Initializes a new instance of the DungeonStuckHandler class

#### DungeonStuckHandler Properties

- **`IsStuck Property`**
  ```csharp
  protected abstract bool IsStuck { get; }
  ```
  Returns true if stuck; otherwise false

- **`StuckAreaName Property`**
  ```csharp
  protected abstract string StuckAreaName { get; }
  ```
  The name of the stuck area

- **`UnstickMoveTo Property`**
  ```csharp
  protected abstract Vector3 UnstickMoveTo { get; }
  ```
  Returns the location the bot needs to move to in order free itself

#### DungeonStuckHandler Methods

- **`HandleStuck Method`**
  ```csharp
  public Task&lt;bool&gt; HandleStuck()
  ```
  Tries to unstick if stuck
  - **Returns:** See Also

- **`Unstick Method`**
  ```csharp
  protected virtual Task Unstick()
  ```
  The movement logic used to free bot
  - **Returns:** This just calls Navigator.PlayerMover.MoveToward with UnstickMoveTo as argument
                If this is not suitable then you can override this method with your own version.

---

### DynamicStringListConverter Class

**Inheritance:** System.Object → System.ComponentModel.TypeConverter → Bots.DungeonBuddy.Helpers.DynamicStringListConverter

```csharp
public class DynamicStringListConverter : TypeConverter
```

Initializes a new instance of the DynamicStringListConverter class

#### DynamicStringListConverter Methods

- **`GetStandardValues Method`**
  Returns a collection of standard values from the default context for the data type this type converter is designed for.

- **`GetStandardValues Method (ITypeDescriptorContext)`**
  ```csharp
  public override TypeConverter.StandardValuesCollection GetStandardValues(
ITypeDescriptorContext context
)
  ```
  - *context*: Type: System.ComponentModelAddLanguageSpecificTextSet("LST7CF785F6_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ITypeDescriptorContext
  - **Returns:** See Also

- **`GetStandardValuesExclusive Method`**
  Returns whether the collection of standard values returned from GetStandardValues is an exclusive list.

- **`GetStandardValuesExclusive Method (ITypeDescriptorContext)`**
  ```csharp
  public override bool GetStandardValuesExclusive(
ITypeDescriptorContext context
)
  ```
  - *context*: Type: System.ComponentModelAddLanguageSpecificTextSet("LSTB65C66A0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ITypeDescriptorContext
  - **Returns:** ReferenceDynamicStringListConverter Class

- **`GetStandardValuesSupported Method`**
  Returns whether this object supports a standard set of values that can be picked from a list.

- **`GetStandardValuesSupported Method (ITypeDescriptorContext)`**
  ```csharp
  public override bool GetStandardValuesSupported(
ITypeDescriptorContext context
)
  ```
  - *context*: Type: System.ComponentModelAddLanguageSpecificTextSet("LSTC6CE9672_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ITypeDescriptorContext
  - **Returns:** ReferenceDynamicStringListConverter Class

---

### Error Class

**Inheritance:** System.Object → Bots.DungeonBuddy.Helpers.Error

```csharp
public class Error
```

Initializes a new instance of the Error class

#### Error Properties

- **`LineNumber Property`**
  ```csharp
  public int LineNumber { get; set; }
  ```

- **`LinePosition Property`**
  ```csharp
  public int LinePosition { get; set; }
  ```

- **`Message Property`**
  ```csharp
  public string Message { get; set; }
  ```

- **`Type Property`**
  ```csharp
  public ErrorType Type { get; set; }
  ```

#### Error Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceError Class

---

### ErrorCollection Class

**Inheritance:** System.Object → System.Collections.Generic.List.Error → Bots.DungeonBuddy.Helpers.ErrorCollection

```csharp
public class ErrorCollection : List&lt;Error&gt;
```

Initializes a new instance of the ErrorCollection class

#### ErrorCollection Constructor

- **`ErrorCollection Constructor`**
  ```csharp
  public ErrorCollection()
  ```
  Initializes a new instance of the ErrorCollection class

- **`ErrorCollection Constructor (Int32)`**
  ```csharp
  public ErrorCollection(
int capacity
)
  ```
  Initializes a new instance of the ErrorCollection class
  - *capacity*: Type: SystemAddLanguageSpecificTextSet("LSTD3898AE9_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32

- **`ErrorCollection Constructor (IEnumerable(Error))`**
  ```csharp
  public ErrorCollection(
IEnumerable&lt;Error&gt; collection
)
  ```
  Initializes a new instance of the ErrorCollection class
  - *collection*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST31210D09_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST31210D09_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");ErrorAddLanguageSpecificTextSet("LST31210D09_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

#### ErrorCollection Properties

- **`ErrorCount Property`**
  ```csharp
  public int ErrorCount { get; }
  ```

- **`Errors Property`**
  ```csharp
  public IEnumerable&lt;Error&gt; Errors { get; }
  ```

- **`HasErrors Property`**
  ```csharp
  public bool HasErrors { get; }
  ```

- **`HasWarnings Property`**
  ```csharp
  public bool HasWarnings { get; }
  ```

- **`WarningCount Property`**
  ```csharp
  public int WarningCount { get; }
  ```

- **`Warnings Property`**
  ```csharp
  public IEnumerable&lt;Error&gt; Warnings { get; }
  ```

---

### ErrorType Enumeration

```csharp
public enum ErrorType
```

---

### Logger Class

**Inheritance:** System.Object → Bots.DungeonBuddy.Helpers.Logger

```csharp
public class Logger
```

Initializes a new instance of the Logger class

#### Logger Methods

- **`Write Method`**

- **`Write Method (String)`**
  ```csharp
  public static void Write(
string message
)
  ```
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST93BDD8FC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`Write Method (String, Object[])`**
  ```csharp
  public static void Write(
string format,
params Object[] args
)
  ```
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST6436E67E_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *args*: Type: AddLanguageSpecificTextSet("LST6436E67E_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST6436E67E_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST6436E67E_6?cpp=&gt;|vb=()|nu=[]");

- **`Write Method (Color, String, Object[])`**
  ```csharp
  public static void Write(
Color color,
string format,
params Object[] args
)
  ```
  - *color*: Type: System.Windows.MediaAddLanguageSpecificTextSet("LST9AAA2109_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Color
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST9AAA2109_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *args*: Type: AddLanguageSpecificTextSet("LST9AAA2109_5?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST9AAA2109_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST9AAA2109_7?cpp=&gt;|vb=()|nu=[]");

- **`WriteDebug Method`**

- **`WriteDebug Method (String)`**
  ```csharp
  public static void WriteDebug(
string message
)
  ```
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTE18967AD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`WriteDebug Method (String, Object[])`**
  ```csharp
  public static void WriteDebug(
string format,
params Object[] args
)
  ```
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTDE051B35_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *args*: Type: AddLanguageSpecificTextSet("LSTDE051B35_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTDE051B35_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTDE051B35_6?cpp=&gt;|vb=()|nu=[]");

- **`WriteError Method`**

- **`WriteError Method (String)`**
  ```csharp
  public static void WriteError(
string message
)
  ```
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST1D0E3B90_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`WriteError Method (Error)`**
  ```csharp
  public static void WriteError(
Error error
)
  ```
  - *error*: Type: Bots.DungeonBuddy.HelpersAddLanguageSpecificTextSet("LST92A5EF69_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Error

- **`WriteError Method (String, Object[])`**
  ```csharp
  public static void WriteError(
string format,
params Object[] args
)
  ```
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST3B95E8EA_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *args*: Type: AddLanguageSpecificTextSet("LST3B95E8EA_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST3B95E8EA_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST3B95E8EA_6?cpp=&gt;|vb=()|nu=[]");

- **`WriteWarning Method`**

- **`WriteWarning Method (String)`**
  ```csharp
  public static void WriteWarning(
string message
)
  ```
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST73CE761E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`WriteWarning Method (String, Object[])`**
  ```csharp
  public static void WriteWarning(
string format,
params Object[] args
)
  ```
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST6DAB6F50_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *args*: Type: AddLanguageSpecificTextSet("LST6DAB6F50_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST6DAB6F50_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST6DAB6F50_6?cpp=&gt;|vb=()|nu=[]");

---

### ScenarioCriteria Class

**Inheritance:** System.Object → Bots.DungeonBuddy.Helpers.ScenarioCriteria

```csharp
public class ScenarioCriteria
```

The AssetId

#### ScenarioCriteria Properties

- **`AssetId Property`**
  ```csharp
  public uint AssetId { get; }
  ```
  The AssetId

- **`CriteriaNumber Property`**
  ```csharp
  public int CriteriaNumber { get; }
  ```
  The 1-based criteria number

- **`CurrentQuantity Property`**
  ```csharp
  public int CurrentQuantity { get; }
  ```
  The current quantity

- **`Duration Property`**
  ```csharp
  public TimeSpan Duration { get; }
  ```
  The duration that criteria must be completed in.

- **`ElapsedTime Property`**
  ```csharp
  public TimeSpan ElapsedTime { get; }
  ```
  The elapsed time

- **`Failed Property`**
  ```csharp
  public uint Failed { get; }
  ```
  Determines if this criteria has failed.

- **`Flags Property`**
  ```csharp
  public uint Flags { get; }
  ```
  The Flags

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  The Id

- **`IsComplete Property`**
  ```csharp
  public bool IsComplete { get; }
  ```
  Indicates whether [is complete]

- **`StepNumber Property`**
  ```csharp
  [ObsoleteAttribute("Use CriteriaNumber instead")]
public int StepNumber { get; }
  ```
  The 1-based step number

- **`Text Property`**
  ```csharp
  public string Text { get; }
  ```
  The text

- **`TotalQuantity Property`**
  ```csharp
  public int TotalQuantity { get; }
  ```
  The total quantity

#### ScenarioCriteria Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceScenarioCriteria Class

---

### ScenarioInfo Class

**Inheritance:** System.Object → Bots.DungeonBuddy.Helpers.ScenarioInfo

```csharp
public class ScenarioInfo
```

Gets the bonus stages.

#### ScenarioInfo Properties

- **`BonusStages Property`**
  ```csharp
  public List&lt;ScenarioStage&gt; BonusStages { get; }
  ```
  Gets the bonus stages.

- **`Current Property`**
  ```csharp
  public static ScenarioInfo Current { get; }
  ```
  Gets information about the current scenario.

- **`CurrentStage Property`**
  ```csharp
  public ScenarioStage CurrentStage { get; }
  ```
  Gets the current stage.

- **`CurrentStageNumber Property`**
  ```csharp
  public int CurrentStageNumber { get; }
  ```
  Current Stage zero based index.

- **`Experience Property`**
  ```csharp
  public int Experience { get; }
  ```
  Gets the experience.

- **`Flags Property`**
  ```csharp
  public uint Flags { get; }
  ```
  Gets the flags.

- **`HasBonusStep Property`**
  ```csharp
  public bool HasBonusStep { get; }
  ```
  Gets a value indicating whether this instance has bonus step.

- **`IsBonusStepComplete Property`**
  ```csharp
  public bool IsBonusStepComplete { get; }
  ```
  Gets a value indicating whether this instance is bonus step complete.

- **`IsComplete Property`**
  ```csharp
  public bool IsComplete { get; }
  ```
  Gets a value indicating whether this instance is complete.

- **`Money Property`**
  ```csharp
  public int Money { get; }
  ```
  Gets the money.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`NumberOfStages Property`**
  ```csharp
  public int NumberOfStages { get; }
  ```
  Gets the number of stages.

- **`Stages Property`**
  ```csharp
  public List&lt;ScenarioStage&gt; Stages { get; }
  ```
  Gets the stages.

#### ScenarioInfo Methods

- **`GetStage Method`**
  ```csharp
  public ScenarioStage GetStage(
int stageNumber
)
  ```
  Gets the stage.
  - *stageNumber*: Type: SystemAddLanguageSpecificTextSet("LSTA8F1D0DC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
  - **Returns:** ReferenceScenarioInfo Class

- **`GetStageById Method`**
  ```csharp
  public ScenarioStage GetStageById(
uint stageId
)
  ```
  - *stageId*: Type: SystemAddLanguageSpecificTextSet("LSTF5492017_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceScenarioInfo Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceScenarioInfo Class

---

### ScenarioStage Class

**Inheritance:** System.Object → Bots.DungeonBuddy.Helpers.ScenarioStage

```csharp
public class ScenarioStage
```

The description

#### ScenarioStage Properties

- **`Description Property`**
  ```csharp
  public string Description { get; }
  ```
  The description

- **`Failed Property`**
  ```csharp
  public bool Failed { get; }
  ```
  Gets a value indicating whether this ScenarioStage is failed.

- **`IsBonusStage Property`**
  ```csharp
  public bool IsBonusStage { get; }
  ```
  Gets a value indicating whether this instance is bonus stage.

- **`IsForCurrentStageOnly Property`**
  ```csharp
  public bool IsForCurrentStageOnly { get; }
  ```
  Gets a value indicating whether this bonus stage is for current stage only.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  The name

- **`NumberOfCriteria Property`**
  ```csharp
  public int NumberOfCriteria { get; }
  ```
  The number of steps

- **`StageId Property`**
  ```csharp
  public uint StageId { get; }
  ```

- **`StageNumber Property`**
  ```csharp
  public int StageNumber { get; }
  ```
  The 1-based stage index

#### ScenarioStage Methods

- **`GetCriteria Method`**
  ```csharp
  public ScenarioCriteria GetCriteria(
int criteriaNumber
)
  ```
  Returns scenario criteria info for the given index. 1 based.
  - *criteriaNumber*: Type: SystemAddLanguageSpecificTextSet("LST18946A81_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 1-based criteria number.
  - **Returns:** ExceptionConditionIndexOutOfRangeExceptioncriteriaNumber is out of range.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceScenarioStage Class

---

### ScriptHelpers Class

**Inheritance:** System.Object → Bots.DungeonBuddy.Helpers.ScriptHelpers

```csharp
public static class ScriptHelpers
```

Gets the group members.

#### ScriptHelpers Properties

- **`GroupMembers Property`**
  ```csharp
  public static List&lt;GroupMember&gt; GroupMembers { get; }
  ```
  Gets the group members.

- **`Healer Property`**
  ```csharp
  public static WoWPlayer Healer { get; }
  ```
  Gets the healer.

- **`InCinematic Property`**
  ```csharp
  public static bool InCinematic { get; }
  ```
  Gets a value indicating whether [in cenematic].

- **`InRoleBasedContent Property`**
  ```csharp
  public static bool InRoleBasedContent { get; }
  ```
  Gets a value indicating whether current dungeon requires a role.

- **`IsFarming Property`**
  ```csharp
  public static bool IsFarming { get; }
  ```

- **`Leader Property`**
  ```csharp
  public static WoWPlayer Leader { get; }
  ```
  Gets the person that will lead group through instance. 
                In content that requires a role this returns the player with highest max hp and has 'tank' role or
                tank spec otherwise player with highest maxHP is returned

- **`MovementEnabled Property`**
  ```csharp
  public static bool MovementEnabled { get; }
  ```
  Returns true if 'ControlledMovement' is active.

- **`SupportsQuesting Property`**
  ```csharp
  public static bool SupportsQuesting { get; }
  ```
  Gets a value indicating whether questing is supported by the Honorbuddy client

- **`Tank Property`**
  ```csharp
  public static WoWPlayer Tank { get; }
  ```
  Gets the tank. In content that requires a role this returns the player with highest max hp and has 'tank' role or
                tank spec otherwise player with highest maxHP is returned.
                Currently this is exactly the same as Leader but might be changed or deprecated in the future 
                since the name doesn't fit for all content e.g. in scenarios or farm mode where spec does not matter.

#### ScriptHelpers Methods

- **`AbandonLowLevelQuests Method`**
  ```csharp
  public static Task&lt;bool&gt; AbandonLowLevelQuests()
  ```
  Abandons all low level quests.
  - **Returns:** See Also

- **`ActualMaxRange Method`**
  ```csharp
  public static float ActualMaxRange(
this WoWSpell spell,
WoWUnit unit
)
  ```
  Max range of a spell on unit. Calculates hitbox
  - *spell*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTB182AB9F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpellThe spell.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTB182AB9F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe unit.
  - **Returns:** ReferenceScriptHelpers Class

- **`ActualMinRange Method`**
  ```csharp
  public static float ActualMinRange(
this WoWSpell spell,
WoWUnit unit
)
  ```
  Min range of a spell on unit. Calculates hitbox.
  - *spell*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST21DA4A49_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpellThe spell.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST21DA4A49_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe unit.
  - **Returns:** ReferenceScriptHelpers Class

- **`AtLocation Method`**
  ```csharp
  public static bool AtLocation(
Vector3 myLoc,
Vector3 location,
float precision
)
  ```
  Determines if myLoc is within a tolerable proximity of location
                where precision is the tolerance value.
  - *myLoc*: Type: System.NumericsAddLanguageSpecificTextSet("LST49428FDC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3My loc.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST49428FDC_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *precision*: Type: SystemAddLanguageSpecificTextSet("LST49428FDC_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe precision.
  - **Returns:** ReferenceScriptHelpers Class

- **`BuyItem Method`**
  ```csharp
  public static void BuyItem(
uint itemId,
int amount
)
  ```
  Buys item from open merchant frame
  - *itemId*: Type: SystemAddLanguageSpecificTextSet("LSTF35539D7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 item id to buy
  - *amount*: Type: SystemAddLanguageSpecificTextSet("LSTF35539D7_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 Amount to buy

- **`Cancel Method`**
  ```csharp
  public static void Cancel(
this WoWAura aura
)
  ```
  Cancels the specified aura.
  - *aura*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTE0FD51E8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWAuraThe aura.

- **`CancelCinematicIfPlaying Method`**
  ```csharp
  public static Task&lt;bool&gt; CancelCinematicIfPlaying()
  ```
  Cancels a cinematic if there is one playing in WoW
  - **Returns:** See Also

- **`CanReachUnitFromCircle Method`**
  ```csharp
  public static bool CanReachUnitFromCircle(
WoWUnit unit,
Vector3 location,
float radius
)
  ```
  Determines whether player can dps/heal a unit while within the circle
            defined by location and radius
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTB10A6007_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe unit.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LSTB10A6007_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LSTB10A6007_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe radius.
  - **Returns:** ReferenceScriptHelpers Class

- **`CastActiveMitigationSpell Method`**
  ```csharp
  public static Task&lt;bool&gt; CastActiveMitigationSpell(
WoWUnit target
)
  ```
  Casts an active mitigation spell.
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTC557731E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnit The target.
  - **Returns:** Remarks

- **`CastHeroism Method`**
  ```csharp
  public static Task&lt;bool&gt; CastHeroism()
  ```
  Casts Heroism, Bloodlust or Time Warp type spell.
  - **Returns:** See Also

- **`CastRangedAbility Method`**
  ```csharp
  public static Task&lt;bool&gt; CastRangedAbility(
WoWUnit target = null
)
  ```
  Casts a ranged ability on a target that is within range.
  - **Returns:** See Also

- **`ClearArea Method`**
  ```csharp
  public static Task&lt;bool&gt; ClearArea(
Vector3 location,
float radius,
Predicate&lt;WoWUnit&gt; unitSelector = null,
bool checkPathDistance = true
)
  ```
  Clears an area centered around a location of all the specified NPCs or all non-friendly NPCs if no none are
            specified.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LSTF4470C91_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3Center of the location to clear NPCs.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LSTF4470C91_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleRadius of the area to clear
  - **Returns:** See Also

- **`CompletePopupQuest Method`**
  ```csharp
  public static Task CompletePopupQuest(
uint questId
)
  ```
  Completes a popup quest.
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LSTAA2C9AB4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 The quest id.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateCancelCinematicIfPlaying Method`**
  ```csharp
  public static Composite CreateCancelCinematicIfPlaying()
  ```
  Creates a behavior that cancels a cenematic if there is one playing in WoW
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateCastRangedAbility Method`**
  ```csharp
  public static Composite CreateCastRangedAbility()
  ```
  Casts a ranged ability on current target or first unit in targeting list.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateClearArea Method`**
  ```csharp
  public static Composite CreateClearArea(
Func&lt;Vector3&gt; centerLocationSelector,
float radius,
Predicate&lt;WoWUnit&gt; unitSelector = null
)
  ```
  Clears an area centered around a location of all the specified NPCs or all non-friendly NPCs if no none are
                specified.
  - *centerLocationSelector*: Type: SystemAddLanguageSpecificTextSet("LST204E74DD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST204E74DD_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST204E74DD_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); Center of the location to clear NPCs.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST204E74DD_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Single Radius of the area to clear
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateCompletePopupQuest Method`**
  ```csharp
  public static Composite CreateCompletePopupQuest(
uint questId
)
  ```
  Completes a popup quest.
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LST1AC6DC7C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 The quest id.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateDispelEnemy Method`**
  ```csharp
  public static Composite CreateDispelEnemy(
string spellName,
ScriptHelpers.EnemyDispelType dispelType,
Func&lt;Object, WoWUnit&gt; targetSelector
)
  ```
  dispels magic from select NPC.
  - *spellName*: Type: SystemAddLanguageSpecificTextSet("LST72F9611E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String Name of the spell.
  - *dispelType*: Type: Bots.DungeonBuddy.HelpersAddLanguageSpecificTextSet("LST72F9611E_3?cs=.|vb=.|cpp=::|nu=.|fs=.");ScriptHelpersAddLanguageSpecificTextSet("LST72F9611E_4?cs=.|vb=.|cpp=::|nu=.|fs=.");EnemyDispelType The dispel type.
  - *targetSelector*: Type: SystemAddLanguageSpecificTextSet("LST72F9611E_5?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST72F9611E_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWUnitAddLanguageSpecificTextSet("LST72F9611E_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The target selector.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateDispelFriendlyUnit Method`**
  ```csharp
  public static Composite CreateDispelFriendlyUnit(
string spellName,
ScriptHelpers.PartyDispelType dispelType,
Func&lt;Object, WoWUnit&gt; targetSelector
)
  ```
  dispels harmful magic from friendly unit
  - *spellName*: Type: SystemAddLanguageSpecificTextSet("LSTF868C789_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String The spell name.
  - *dispelType*: Type: Bots.DungeonBuddy.HelpersAddLanguageSpecificTextSet("LSTF868C789_3?cs=.|vb=.|cpp=::|nu=.|fs=.");ScriptHelpersAddLanguageSpecificTextSet("LSTF868C789_4?cs=.|vb=.|cpp=::|nu=.|fs=.");PartyDispelType The dispel type.
  - *targetSelector*: Type: SystemAddLanguageSpecificTextSet("LSTF868C789_5?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTF868C789_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWUnitAddLanguageSpecificTextSet("LSTF868C789_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateDispelGroup Method`**
  ```csharp
  public static Composite CreateDispelGroup(
string spellName,
ScriptHelpers.PartyDispelType dispelType
)
  ```
  dispels harmful magic from party members.
  - *spellName*: Type: SystemAddLanguageSpecificTextSet("LST38134095_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String The spell name.
  - *dispelType*: Type: Bots.DungeonBuddy.HelpersAddLanguageSpecificTextSet("LST38134095_3?cs=.|vb=.|cpp=::|nu=.|fs=.");ScriptHelpersAddLanguageSpecificTextSet("LST38134095_4?cs=.|vb=.|cpp=::|nu=.|fs=.");PartyDispelType The dispel type.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateInteractWithObject Method`**
  Moves to an object and interacts with it

- **`CreateInteractWithObject Method (Func(Object, WoWObject))`**
  ```csharp
  public static Composite CreateInteractWithObject(
Func&lt;Object, WoWObject&gt; objectSelector
)
  ```
  Moves to an object and interacts with it
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LSTF72FE51_3?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTF72FE51_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWObjectAddLanguageSpecificTextSet("LSTF72FE51_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The object selector.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateInteractWithObject Method (UInt32)`**
  ```csharp
  public static Composite CreateInteractWithObject(
uint id
)
  ```
  Moves to an object and interacts with it
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTB59D9062_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 Object id
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateInteractWithObject Method (Func(Object, WoWObject), Int32, Boolean)`**
  ```csharp
  public static Composite CreateInteractWithObject(
Func&lt;Object, WoWObject&gt; objectSelector,
int channelTime,
bool ignoreCombat = false
)
  ```
  Moves to an object and interacts with it
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LST325B4583_3?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST325B4583_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWObjectAddLanguageSpecificTextSet("LST325B4583_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The object selector.
  - *channelTime*: Type: SystemAddLanguageSpecificTextSet("LST325B4583_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 The time in seconds to keep channeling for
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateInteractWithObject Method (UInt32, Int32, Boolean)`**
  ```csharp
  public static Composite CreateInteractWithObject(
uint id,
int channelTime,
bool ignoreCombat = false
)
  ```
  Moves to an object and interacts with it
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST1A7DA1C0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 Object id
  - *channelTime*: Type: SystemAddLanguageSpecificTextSet("LST1A7DA1C0_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 The time in seconds to keep channeling for
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateInteractWithObjectContinue Method`**
  Moves to an object and interacts with it. For use with Sequences.

- **`CreateInteractWithObjectContinue Method (Func(Object, WoWObject))`**
  ```csharp
  public static Composite CreateInteractWithObjectContinue(
Func&lt;Object, WoWObject&gt; objectSelector
)
  ```
  Moves to an object and interacts with it. For use with Sequences.
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LSTFC6DADF2_3?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTFC6DADF2_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWObjectAddLanguageSpecificTextSet("LSTFC6DADF2_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The object selector.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateInteractWithObjectContinue Method (UInt32)`**
  ```csharp
  public static Composite CreateInteractWithObjectContinue(
uint id
)
  ```
  Moves to an object and interacts with it. For use with Sequences.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST3A35D2C1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 The id.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateInteractWithObjectContinue Method (Func(Object, WoWObject), Int32, Boolean)`**
  ```csharp
  public static Composite CreateInteractWithObjectContinue(
Func&lt;Object, WoWObject&gt; objectSelector,
int channelTime,
bool ignoreCombat = false
)
  ```
  Moves to an object and interacts with it. For use with Sequences.
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LST80D2D3C4_3?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST80D2D3C4_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWObjectAddLanguageSpecificTextSet("LST80D2D3C4_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The object selector.
  - *channelTime*: Type: SystemAddLanguageSpecificTextSet("LST80D2D3C4_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 The time in seconds to keep channeling for
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateInteractWithObjectContinue Method (UInt32, Int32, Boolean)`**
  ```csharp
  public static Composite CreateInteractWithObjectContinue(
uint id,
int channelTime,
bool ignoreCombat = false
)
  ```
  Moves to an object and interacts with it. For use with Sequences.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST7B48E87F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 The object id.
  - *channelTime*: Type: SystemAddLanguageSpecificTextSet("LST7B48E87F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 The time in seconds to keep channeling for
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateInterruptCast Method`**
  ```csharp
  public static Composite CreateInterruptCast(
Func&lt;Object, WoWUnit&gt; targetSelector,
params int[] spellIds
)
  ```
  - *targetSelector*: Type: SystemAddLanguageSpecificTextSet("LST2779E236_1?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST2779E236_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWUnitAddLanguageSpecificTextSet("LST2779E236_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *spellIds*: Type: AddLanguageSpecificTextSet("LST2779E236_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST2779E236_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LST2779E236_6?cpp=&gt;|vb=()|nu=[]");
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateLootChest Method`**
  ```csharp
  public static Composite CreateLootChest(
Func&lt;Object, WoWGameObject&gt; chestSelector,
bool encounterReward = true
)
  ```
  Creates a behavior that moves to chest and loots it
  - *chestSelector*: Type: SystemAddLanguageSpecificTextSet("LSTF49E13F8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTF49E13F8_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWGameObjectAddLanguageSpecificTextSet("LSTF49E13F8_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The chest selector.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateLosLocation Method`**
  ```csharp
  public static Composite CreateLosLocation(
CanRunDecoratorDelegate canRun,
Func&lt;Object, Vector3&gt; locationToLos,
Func&lt;Object, Vector3&gt; objectLocationToLosAt,
Func&lt;Object, float&gt; objectRadius
)
  ```
  This behavior will run out of Line-Of-Sight of a location.
  - *canRun*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTC14866E1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The condition to run this behavior
  - *locationToLos*: Type: SystemAddLanguageSpecificTextSet("LSTC14866E1_2?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTC14866E1_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, Vector3AddLanguageSpecificTextSet("LSTC14866E1_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The location you want to get out of LOS with.
  - *objectLocationToLosAt*: Type: SystemAddLanguageSpecificTextSet("LSTC14866E1_5?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTC14866E1_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, Vector3AddLanguageSpecificTextSet("LSTC14866E1_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The center location of the object you want to use to LOS
  - *objectRadius*: Type: SystemAddLanguageSpecificTextSet("LSTC14866E1_8?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTC14866E1_9?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, SingleAddLanguageSpecificTextSet("LSTC14866E1_10?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The radius of the object you want to use to LOS
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateMoveToContinue Method`**
  Moves to a location. For use with a Sequence

- **`CreateMoveToContinue Method (Func(Object, WoWObject))`**
  ```csharp
  public static Composite CreateMoveToContinue(
Func&lt;Object, WoWObject&gt; objectSelector
)
  ```
  Moves to a location. For use with a Sequence
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LST7182DB7_3?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST7182DB7_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWObjectAddLanguageSpecificTextSet("LST7182DB7_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The object selector.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateMoveToContinue Method (Func(Object, Vector3))`**
  ```csharp
  public static Composite CreateMoveToContinue(
Func&lt;Object, Vector3&gt; locationSelector
)
  ```
  Moves to a location. For use with a Sequence
  - *locationSelector*: Type: SystemAddLanguageSpecificTextSet("LSTD442D90_3?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTD442D90_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, Vector3AddLanguageSpecificTextSet("LSTD442D90_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The location selector.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateMoveToContinue Method (UInt32)`**
  ```csharp
  public static Composite CreateMoveToContinue(
uint objectId
)
  ```
  Moves to a location. For use with a Sequence
  - *objectId*: Type: SystemAddLanguageSpecificTextSet("LST24F83EBA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 The object id.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateMoveToContinue Method (CanRunDecoratorDelegate, Func(Object, WoWObject), Boolean)`**
  ```csharp
  public static Composite CreateMoveToContinue(
CanRunDecoratorDelegate canRun,
Func&lt;Object, WoWObject&gt; objectSelector,
bool ignoreCombat
)
  ```
  Moves to a location. For use with a Sequence
  - *canRun*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTC130EF65_3?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The can run.
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LSTC130EF65_4?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTC130EF65_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWObjectAddLanguageSpecificTextSet("LSTC130EF65_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The object selector.
  - *ignoreCombat*: Type: SystemAddLanguageSpecificTextSet("LSTC130EF65_7?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean Keeps moving even when in combat
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateMoveToContinue Method (CanRunDecoratorDelegate, UInt32, Boolean)`**
  ```csharp
  public static Composite CreateMoveToContinue(
CanRunDecoratorDelegate canRun,
uint objectId,
bool ignoreCombat
)
  ```
  Moves to a location. For use with a Sequence
  - *canRun*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTDB6A51AA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The can run.
  - *objectId*: Type: SystemAddLanguageSpecificTextSet("LSTDB6A51AA_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 Object Id to move to
  - *ignoreCombat*: Type: SystemAddLanguageSpecificTextSet("LSTDB6A51AA_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean Keeps moving even when in combat
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateMoveToContinue Method (CanRunDecoratorDelegate, Func(Object, Vector3), Boolean, Func(Object, Nullable(Single)))`**
  ```csharp
  public static Composite CreateMoveToContinue(
CanRunDecoratorDelegate canRun,
Func&lt;Object, Vector3&gt; locationSelector,
bool ignoreCombat,
Func&lt;Object, Nullable&lt;float&gt;&gt; distanceToleranceSelector = null
)
  ```
  Moves to a location. For use with a Sequence
  - *canRun*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTD0D42C18_7?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The can run.
  - *locationSelector*: Type: SystemAddLanguageSpecificTextSet("LSTD0D42C18_8?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTD0D42C18_9?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, Vector3AddLanguageSpecificTextSet("LSTD0D42C18_10?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); Location to move to
  - *ignoreCombat*: Type: SystemAddLanguageSpecificTextSet("LSTD0D42C18_11?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean Keeps moving even when in combat
  - **Returns:** ReferenceScriptHelpers Class

- **`CreatePickupQuest Method`**
  picks up a quest from object

- **`CreatePickupQuest Method (Func(Object, WoWObject), UInt32)`**
  ```csharp
  public static Composite CreatePickupQuest(
Func&lt;Object, WoWObject&gt; objSelector,
uint questId = 0
)
  ```
  picks up a quest from object
  - *objSelector*: Type: SystemAddLanguageSpecificTextSet("LST545CC4C4_3?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST545CC4C4_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWObjectAddLanguageSpecificTextSet("LST545CC4C4_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The object selector
  - **Returns:** ReferenceScriptHelpers Class

- **`CreatePickupQuest Method (UInt32, UInt32)`**
  ```csharp
  public static Composite CreatePickupQuest(
uint objectId,
uint questId = 0
)
  ```
  picks up a quest from object
  - *objectId*: Type: SystemAddLanguageSpecificTextSet("LST5BDD8533_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 The object WoWHead id.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreatePullNpcToLocation Method`**
  Pull NPC to location.

- **`CreatePullNpcToLocation Method (CanRunDecoratorDelegate, Func(Object, WoWUnit), Func(Object, Vector3), Int32)`**
  ```csharp
  public static Composite CreatePullNpcToLocation(
CanRunDecoratorDelegate canRun,
Func&lt;Object, WoWUnit&gt; unitSelector,
Func&lt;Object, Vector3&gt; pullToLocationSelector,
int waitTimeAtPullLocationTimeout
)
  ```
  Pull NPC to location.
  - *canRun*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTB4AF14C7_5?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The 'can run' condition.
  - *unitSelector*: Type: SystemAddLanguageSpecificTextSet("LSTB4AF14C7_6?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTB4AF14C7_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWUnitAddLanguageSpecificTextSet("LSTB4AF14C7_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The unit selector.
  - *pullToLocationSelector*: Type: SystemAddLanguageSpecificTextSet("LSTB4AF14C7_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTB4AF14C7_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, Vector3AddLanguageSpecificTextSet("LSTB4AF14C7_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The pull to location selector. if null then current locaton is used
  - *waitTimeAtPullLocationTimeout*: Type: SystemAddLanguageSpecificTextSet("LSTB4AF14C7_12?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
                The max time in seconds to wait at pull location for target to come in
                melee range.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreatePullNpcToLocation Method (CanRunDecoratorDelegate, CanRunDecoratorDelegate, Func(Object, WoWUnit), Func(Object, Vector3), Func(Object, Vector3))`**
  ```csharp
  public static Composite CreatePullNpcToLocation(
CanRunDecoratorDelegate canRun,
CanRunDecoratorDelegate readyToPull,
Func&lt;Object, WoWUnit&gt; unitSelector,
Func&lt;Object, Vector3&gt; pullToLocationSelector,
Func&lt;Object, Vector3&gt; waitAtLocationSellector
)
  ```
  Pull NPC to location.
  - *canRun*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTCAFADA81_7?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The 'can run' condition.
  - *readyToPull*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTCAFADA81_8?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The 'ready to pull' condition.
  - *unitSelector*: Type: SystemAddLanguageSpecificTextSet("LSTCAFADA81_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTCAFADA81_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWUnitAddLanguageSpecificTextSet("LSTCAFADA81_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The unit selector.
  - *pullToLocationSelector*: Type: SystemAddLanguageSpecificTextSet("LSTCAFADA81_12?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTCAFADA81_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, Vector3AddLanguageSpecificTextSet("LSTCAFADA81_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The pull to location selector. if null then current locaton is used
  - *waitAtLocationSellector*: Type: SystemAddLanguageSpecificTextSet("LSTCAFADA81_15?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTCAFADA81_16?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, Vector3AddLanguageSpecificTextSet("LSTCAFADA81_17?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The 'wait at' location sellector. if null then current locaton is used
  - **Returns:** ReferenceScriptHelpers Class

- **`CreatePullNpcToLocation Method (CanRunDecoratorDelegate, CanRunDecoratorDelegate, Func(Object, WoWUnit), Func(Object, Vector3), Int32)`**
  ```csharp
  public static Composite CreatePullNpcToLocation(
CanRunDecoratorDelegate canRun,
CanRunDecoratorDelegate readyToPull,
Func&lt;Object, WoWUnit&gt; unitSelector,
Func&lt;Object, Vector3&gt; pullToLocationSelector,
int waitTimeAtPullLocationTimeout
)
  ```
  Pull NPC to location.
  - *canRun*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST8A0A7904_5?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The 'can run' condition.
  - *readyToPull*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST8A0A7904_6?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The 'ready to pull' condition.
  - *unitSelector*: Type: SystemAddLanguageSpecificTextSet("LST8A0A7904_7?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST8A0A7904_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWUnitAddLanguageSpecificTextSet("LST8A0A7904_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The unit selector.
  - *pullToLocationSelector*: Type: SystemAddLanguageSpecificTextSet("LST8A0A7904_10?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST8A0A7904_11?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, Vector3AddLanguageSpecificTextSet("LST8A0A7904_12?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The pull to location selector. if null then current locaton is used
  - *waitTimeAtPullLocationTimeout*: Type: SystemAddLanguageSpecificTextSet("LST8A0A7904_13?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
                The max time in seconds to wait at pull location for target to come in
                melee range.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreatePullNpcToLocation Method (CanRunDecoratorDelegate, CanRunDecoratorDelegate, Func(Object, WoWUnit), Func(Object, Vector3), Func(Object, Vector3), Int32)`**
  ```csharp
  public static Composite CreatePullNpcToLocation(
CanRunDecoratorDelegate canRun,
CanRunDecoratorDelegate readyToPull,
Func&lt;Object, WoWUnit&gt; unitSelector,
Func&lt;Object, Vector3&gt; pullToLocationSelector,
Func&lt;Object, Vector3&gt; waitAtLocationSellector,
int waitTimeAtPullLocationTimeout
)
  ```
  Pull NPC to location.
  - *canRun*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTE152DB38_7?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The 'can run' condition.
  - *readyToPull*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTE152DB38_8?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The 'ready to pull' condition.
  - *unitSelector*: Type: SystemAddLanguageSpecificTextSet("LSTE152DB38_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTE152DB38_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWUnitAddLanguageSpecificTextSet("LSTE152DB38_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The unit selector.
  - *pullToLocationSelector*: Type: SystemAddLanguageSpecificTextSet("LSTE152DB38_12?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTE152DB38_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, Vector3AddLanguageSpecificTextSet("LSTE152DB38_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The pull to location selector. if null then current locaton is used
  - *waitAtLocationSellector*: Type: SystemAddLanguageSpecificTextSet("LSTE152DB38_15?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTE152DB38_16?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, Vector3AddLanguageSpecificTextSet("LSTE152DB38_17?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The 'wait at' location sellector. if null then current locaton is used
  - *waitTimeAtPullLocationTimeout*: Type: SystemAddLanguageSpecificTextSet("LSTE152DB38_18?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
                The max time in seconds to wait at pull location for target to come in
                melee range.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateRunToTankIfAggroed Method`**
  ```csharp
  public static Composite CreateRunToTankIfAggroed()
  ```
  Creates behavior that has follower run to tank if has aggro
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateSpreadOutLogic Method`**
  Spreads the ranged dps out, Healer doesn't move unless too close to melee.

- **`CreateSpreadOutLogic Method (CanRunDecoratorDelegate, Int32)`**
  ```csharp
  [ObsoleteAttribute("Use the Avoidance API instead because it does a better job.")]
public static Composite CreateSpreadOutLogic(
CanRunDecoratorDelegate canRun,
int distanceToSpread
)
  ```
  Spreads the ranged dps out, Healer doesn't move unless too close to melee.
  - *canRun*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST5E90E066_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The can run.
  - *distanceToSpread*: Type: SystemAddLanguageSpecificTextSet("LST5E90E066_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 The distance to spread.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateSpreadOutLogic Method (CanRunDecoratorDelegate, Func(Object, Vector3), Int32, Single)`**
  ```csharp
  [ObsoleteAttribute("Use the Avoidance API instead because it does a better job.")]
public static Composite CreateSpreadOutLogic(
CanRunDecoratorDelegate canRun,
Func&lt;Object, Vector3&gt; centerPointSelector,
int distanceToSpread,
float maxDistanceFromCenterPoint
)
  ```
  Spreads the ranged dps out, Healer doesn't move unless too close to melee.
  - *canRun*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST5612A267_3?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The condition under which to run this behavior.
  - *centerPointSelector*: Type: SystemAddLanguageSpecificTextSet("LST5612A267_4?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST5612A267_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, Vector3AddLanguageSpecificTextSet("LST5612A267_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); center location of the area to spread out
  - *distanceToSpread*: Type: SystemAddLanguageSpecificTextSet("LST5612A267_7?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 The distance to spread out from other party members.
  - *maxDistanceFromCenterPoint*: Type: SystemAddLanguageSpecificTextSet("LST5612A267_8?cs=.|vb=.|cpp=::|nu=.|fs=.");Single max distance to move from center point
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateTalkToNpc Method`**
  Move and talk to npc

- **`CreateTalkToNpc Method (Func(Object, WoWUnit))`**
  ```csharp
  public static Composite CreateTalkToNpc(
Func&lt;Object, WoWUnit&gt; unitSelector = null
)
  ```
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateTalkToNpc Method (UInt32)`**
  ```csharp
  public static Composite CreateTalkToNpc(
uint unitId
)
  ```
  Move and talk to npc
  - *unitId*: Type: SystemAddLanguageSpecificTextSet("LST9D9E79C1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 The unit id.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateTalkToNpc Method (Func(Object, WoWUnit), Int32)`**
  ```csharp
  public static Composite CreateTalkToNpc(
Func&lt;Object, WoWUnit&gt; unitSelector,
int gossipIndex
)
  ```
  - *unitSelector*: Type: SystemAddLanguageSpecificTextSet("LST146AC5EC_3?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST146AC5EC_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWUnitAddLanguageSpecificTextSet("LST146AC5EC_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *gossipIndex*: Type: SystemAddLanguageSpecificTextSet("LST146AC5EC_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateTalkToNpc Method (UInt32, Int32)`**
  ```csharp
  public static Composite CreateTalkToNpc(
uint unitId,
int gossipIndex
)
  ```
  Move and talk to npc, selecting gossip at specified index
  - *unitId*: Type: SystemAddLanguageSpecificTextSet("LST64BCD6BA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 The unit id.
  - *gossipIndex*: Type: SystemAddLanguageSpecificTextSet("LST64BCD6BA_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 Index of the gossip.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateTankAgainstObject Method`**
  ```csharp
  public static Composite CreateTankAgainstObject(
CanRunDecoratorDelegate canRun,
Func&lt;Object, Vector3&gt; objectLocationToTankAt,
Func&lt;Object, float&gt; objectRadius
)
  ```
  Tanks unit against an object e.g it could be a pillar.
  - *canRun*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST45CF8FE4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The can run.
  - *objectLocationToTankAt*: Type: SystemAddLanguageSpecificTextSet("LST45CF8FE4_2?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST45CF8FE4_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, Vector3AddLanguageSpecificTextSet("LST45CF8FE4_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The object location to tank at.
  - *objectRadius*: Type: SystemAddLanguageSpecificTextSet("LST45CF8FE4_5?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST45CF8FE4_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, SingleAddLanguageSpecificTextSet("LST45CF8FE4_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The object radius.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateTankFaceAwayGroupUnit Method`**
  tank faces current target away from group.

- **`CreateTankFaceAwayGroupUnit Method (Int32)`**
  ```csharp
  public static Composite CreateTankFaceAwayGroupUnit(
int maxDistance
)
  ```
  tank faces current target away from group.
  - *maxDistance*: Type: SystemAddLanguageSpecificTextSet("LST617AEB40_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 The max distance
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateTankFaceAwayGroupUnit Method (Func(Object, WoWUnit), Int32)`**
  ```csharp
  public static Composite CreateTankFaceAwayGroupUnit(
Func&lt;Object, WoWUnit&gt; unitSelector,
int maxDistance
)
  ```
  Tank faces unit away from group.
  - *unitSelector*: Type: SystemAddLanguageSpecificTextSet("LSTAF28A806_3?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTAF28A806_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWUnitAddLanguageSpecificTextSet("LSTAF28A806_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The unit.
  - *maxDistance*: Type: SystemAddLanguageSpecificTextSet("LSTAF28A806_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 The max distance.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateTankTalkToThenEscortNpc Method`**
  Talks to a NPC and then escorts the npc

- **`CreateTankTalkToThenEscortNpc Method (UInt32, Vector3, Vector3)`**
  ```csharp
  public static Composite CreateTankTalkToThenEscortNpc(
uint escortNpcId,
Vector3 escortNpcLocation,
Vector3 endLocation
)
  ```
  Talks to a NPC and then escorts the npc
  - *escortNpcId*: Type: SystemAddLanguageSpecificTextSet("LST360B3CEC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 The escort NPC id.
  - *escortNpcLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LST360B3CEC_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3 The escort NPC location.
  - *endLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LST360B3CEC_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3 The end location.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateTankTalkToThenEscortNpc Method (UInt32, Int32, Vector3, Vector3, Single, Func(Object, Boolean))`**
  ```csharp
  public static Composite CreateTankTalkToThenEscortNpc(
uint escortNpcId,
int gossipIndex,
Vector3 escortNpcLocation,
Vector3 endLocation,
float followDistance,
Func&lt;Object, bool&gt; isDoneCondition = null
)
  ```
  Talks to a NPC and then escorts the npc
  - *escortNpcId*: Type: SystemAddLanguageSpecificTextSet("LSTCB2B2388_3?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 The escort NPC id.
  - *gossipIndex*: Type: SystemAddLanguageSpecificTextSet("LSTCB2B2388_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 index of the gossip to choose (0 based)
  - *escortNpcLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LSTCB2B2388_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3 The escort NPC location.
  - *endLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LSTCB2B2388_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3 The end location.
  - *followDistance*: Type: SystemAddLanguageSpecificTextSet("LSTCB2B2388_7?cs=.|vb=.|cpp=::|nu=.|fs=.");Single The follow distance.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateTankUnitAtLocation Method`**
  ```csharp
  public static Composite CreateTankUnitAtLocation(
Func&lt;Object, Vector3&gt; locationSelector,
float precision
)
  ```
  tank unit at location.
  - *locationSelector*: Type: SystemAddLanguageSpecificTextSet("LST534304D6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST534304D6_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, Vector3AddLanguageSpecificTextSet("LST534304D6_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The location selector.
  - *precision*: Type: SystemAddLanguageSpecificTextSet("LST534304D6_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Single The precision.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateTurninQuest Method`**
  Turns in a quest at object

- **`CreateTurninQuest Method (Func(Object, WoWObject), UInt32)`**
  ```csharp
  public static Composite CreateTurninQuest(
Func&lt;Object, WoWObject&gt; objectSelector,
uint questId = 0
)
  ```
  Turns in a quest at object
  - *objectSelector*: Type: SystemAddLanguageSpecificTextSet("LST7DD73378_3?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST7DD73378_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWObjectAddLanguageSpecificTextSet("LST7DD73378_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The object selector.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateTurninQuest Method (UInt32, UInt32)`**
  ```csharp
  public static Composite CreateTurninQuest(
uint objectId,
uint questId = 0
)
  ```
  Turns in a quest at object
  - *objectId*: Type: SystemAddLanguageSpecificTextSet("LST3668D295_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 The object WoWHead id.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateWaitAtLocationUntilTankPulled Method`**
  ```csharp
  public static Composite CreateWaitAtLocationUntilTankPulled(
CanRunDecoratorDelegate canrun,
Func&lt;Object, Vector3&gt; locationSelector
)
  ```
  waits at location until tank pulled.
  - *canrun*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST8D94616B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The canrun.
  - *locationSelector*: Type: SystemAddLanguageSpecificTextSet("LST8D94616B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST8D94616B_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, Vector3AddLanguageSpecificTextSet("LST8D94616B_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The location selector. if null then current locaton is used.
  - **Returns:** ReferenceScriptHelpers Class

- **`CreateWaitAtLocationWhile Method`**
  ```csharp
  [ObsoleteAttribute("Use the coroutine version")]
public static Composite CreateWaitAtLocationWhile(
CanRunDecoratorDelegate canRun,
Func&lt;Object, Vector3&gt; locationSelector,
string locationDescription = null,
float precision = 8f,
params ScriptHelpers.GroupRoleFlags[] roles
)
  ```
  wait at location while condition is true
  - *canRun*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST48F3EBBC_2?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The canrun.
  - *locationSelector*: Type: SystemAddLanguageSpecificTextSet("LST48F3EBBC_3?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST48F3EBBC_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, Vector3AddLanguageSpecificTextSet("LST48F3EBBC_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The location selector.
  - *roles*: Type: AddLanguageSpecificTextSet("LST48F3EBBC_8?cpp=array&lt;");Bots.DungeonBuddy.HelpersAddLanguageSpecificTextSet("LST48F3EBBC_9?cs=.|vb=.|cpp=::|nu=.|fs=.");ScriptHelpersAddLanguageSpecificTextSet("LST48F3EBBC_10?cs=.|vb=.|cpp=::|nu=.|fs=.");GroupRoleFlagsAddLanguageSpecificTextSet("LST48F3EBBC_11?cpp=&gt;|vb=()|nu=[]"); The roles.
  - **Returns:** ReferenceScriptHelpers Class

- **`Cross Method`**
  ```csharp
  public static Vector3 Cross(
this Vector3 point1,
Vector3 point2
)
  ```
  Returns the cross product.
  - *point1*: Type: System.NumericsAddLanguageSpecificTextSet("LSTABBEB827_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The point1.
  - *point2*: Type: System.NumericsAddLanguageSpecificTextSet("LSTABBEB827_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The point2.
  - **Returns:** ReferenceScriptHelpers Class

- **`DisableMovement Method`**
  ```csharp
  public static void DisableMovement(
Func&lt;bool&gt; whileTrue
)
  ```
  Disables movement until expresion evaluates to 'false'. Use with care.
  - *whileTrue*: Type: SystemAddLanguageSpecificTextSet("LST332027B8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST332027B8_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST332027B8_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The while true.

- **`DispelEnemy Method`**
  ```csharp
  public static Task&lt;bool&gt; DispelEnemy(
string spellName,
ScriptHelpers.EnemyDispelType dispelType,
WoWUnit target
)
  ```
  dispels magic from select NPC.
  - *spellName*: Type: SystemAddLanguageSpecificTextSet("LSTD8E75443_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String Name of the spell.
  - *dispelType*: Type: Bots.DungeonBuddy.HelpersAddLanguageSpecificTextSet("LSTD8E75443_3?cs=.|vb=.|cpp=::|nu=.|fs=.");ScriptHelpersAddLanguageSpecificTextSet("LSTD8E75443_4?cs=.|vb=.|cpp=::|nu=.|fs=.");EnemyDispelType The dispel type.
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTD8E75443_5?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnit The target.
  - **Returns:** See Also

- **`DispelFriendlyUnit Method`**
  dispels harmful magic from friendly unit

- **`DispelFriendlyUnit Method (Int32, ScriptHelpers.PartyDispelType, WoWUnit)`**
  ```csharp
  public static Task&lt;bool&gt; DispelFriendlyUnit(
int spellId,
ScriptHelpers.PartyDispelType dispelType,
WoWUnit target
)
  ```
  dispels harmful magic from friendly unit
  - *spellId*: Type: SystemAddLanguageSpecificTextSet("LST105549E3_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
  - *dispelType*: Type: Bots.DungeonBuddy.HelpersAddLanguageSpecificTextSet("LST105549E3_4?cs=.|vb=.|cpp=::|nu=.|fs=.");ScriptHelpersAddLanguageSpecificTextSet("LST105549E3_5?cs=.|vb=.|cpp=::|nu=.|fs=.");PartyDispelType
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST105549E3_6?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnit
  - **Returns:** See Also

- **`DispelFriendlyUnit Method (String, ScriptHelpers.PartyDispelType, WoWUnit)`**
  ```csharp
  public static Task&lt;bool&gt; DispelFriendlyUnit(
string spellName,
ScriptHelpers.PartyDispelType dispelType,
WoWUnit target
)
  ```
  dispels harmful magic from friendly unit
  - *spellName*: Type: SystemAddLanguageSpecificTextSet("LST31A432CE_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *dispelType*: Type: Bots.DungeonBuddy.HelpersAddLanguageSpecificTextSet("LST31A432CE_4?cs=.|vb=.|cpp=::|nu=.|fs=.");ScriptHelpersAddLanguageSpecificTextSet("LST31A432CE_5?cs=.|vb=.|cpp=::|nu=.|fs=.");PartyDispelType
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST31A432CE_6?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnit
  - **Returns:** See Also

- **`DispelGroup Method`**
  dispels harmful magic from party members.

- **`DispelGroup Method (Int32, ScriptHelpers.PartyDispelType)`**
  ```csharp
  public static Task&lt;bool&gt; DispelGroup(
int spellId,
ScriptHelpers.PartyDispelType dispelType
)
  ```
  dispels harmful magic from party members.
  - *spellId*: Type: SystemAddLanguageSpecificTextSet("LST3C1D37F0_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
  - *dispelType*: Type: Bots.DungeonBuddy.HelpersAddLanguageSpecificTextSet("LST3C1D37F0_4?cs=.|vb=.|cpp=::|nu=.|fs=.");ScriptHelpersAddLanguageSpecificTextSet("LST3C1D37F0_5?cs=.|vb=.|cpp=::|nu=.|fs=.");PartyDispelType
  - **Returns:** See Also

- **`DispelGroup Method (String, ScriptHelpers.PartyDispelType)`**
  ```csharp
  public static Task&lt;bool&gt; DispelGroup(
string spellName,
ScriptHelpers.PartyDispelType dispelType
)
  ```
  dispels harmful magic from party members.
  - *spellName*: Type: SystemAddLanguageSpecificTextSet("LST6765039D_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *dispelType*: Type: Bots.DungeonBuddy.HelpersAddLanguageSpecificTextSet("LST6765039D_4?cs=.|vb=.|cpp=::|nu=.|fs=.");ScriptHelpersAddLanguageSpecificTextSet("LST6765039D_5?cs=.|vb=.|cpp=::|nu=.|fs=.");PartyDispelType
  - **Returns:** See Also

- **`Dot Method`**
  ```csharp
  public static float Dot(
this Vector3 point1,
Vector3 point2
)
  ```
  Returns the dot product.
  - *point1*: Type: System.NumericsAddLanguageSpecificTextSet("LST30B13FB8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The point1.
  - *point2*: Type: System.NumericsAddLanguageSpecificTextSet("LST30B13FB8_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The point2.
  - **Returns:** ReferenceScriptHelpers Class

- **`FaceIfNotFacing Method`**
  ```csharp
  public static Task&lt;bool&gt; FaceIfNotFacing(
WoWUnit target,
string reason = null
)
  ```
  Faces towards target if not already facing it 
            and waits for up to 4 seconds the WoW client complete the face request
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST8E014FB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe WoWUnit to face towards
  - **Returns:** See Also

- **`FindBestTargetWithIdsRange Method`**
  ```csharp
  public static WoWUnit FindBestTargetWithIdsRange(
int maxRange,
params uint[] unitIds
)
  ```
  Finds the best target with ids range.
  - *maxRange*: Type: SystemAddLanguageSpecificTextSet("LST7E574D0D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 The max range.
  - *unitIds*: Type: AddLanguageSpecificTextSet("LST7E574D0D_2?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST7E574D0D_3?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32AddLanguageSpecificTextSet("LST7E574D0D_4?cpp=&gt;|vb=()|nu=[]"); The unit ids.
  - **Returns:** ReferenceScriptHelpers Class

- **`GetBehindUnit Method`**
  Gets behind a given unit

- **`GetBehindUnit Method (WoWUnit)`**
  ```csharp
  public static Task&lt;bool&gt; GetBehindUnit(
WoWUnit unit
)
  ```
  Gets behind a given unit
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTEA364204_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnit the WoWUnit to get behind.
  - **Returns:** See Also

- **`GetBehindUnit Method (CanRunDecoratorDelegate, Func(Object, WoWUnit))`**
  ```csharp
  public static Composite GetBehindUnit(
CanRunDecoratorDelegate canRun,
Func&lt;Object, WoWUnit&gt; targetProducer
)
  ```
  Gets behind a given unit
  - *canRun*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LSTC20AA74E_3?cs=.|vb=.|cpp=::|nu=.|fs=.");CanRunDecoratorDelegate The can run.
  - *targetProducer*: Type: SystemAddLanguageSpecificTextSet("LSTC20AA74E_4?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTC20AA74E_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Object, WoWUnitAddLanguageSpecificTextSet("LSTC20AA74E_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)"); The who.
  - **Returns:** ReferenceScriptHelpers Class

- **`GetCurseDispel Method`**
  ```csharp
  public static WoWSpell GetCurseDispel()
  ```
  - **Returns:** ReferenceScriptHelpers Class

- **`GetDefensiveMagicDispel Method`**
  ```csharp
  public static WoWSpell GetDefensiveMagicDispel()
  ```
  - **Returns:** ReferenceScriptHelpers Class

- **`GetDiseaseDispel Method`**
  ```csharp
  public static WoWSpell GetDiseaseDispel()
  ```
  - **Returns:** ReferenceScriptHelpers Class

- **`GetEnrageDispel Method`**
  ```csharp
  public static WoWSpell GetEnrageDispel()
  ```
  - **Returns:** ReferenceScriptHelpers Class

- **`GetGroupCenterLocation Method`**
  ```csharp
  public static Vector3 GetGroupCenterLocation(
Predicate&lt;GroupMember&gt; predicate = null,
float radius = 40f,
Func&lt;Vector3, Vector3[], float&gt; getPriority = null
)
  ```
  Returns the center location of group. First the algorithm finds group member who has the highest number of others
            within 
            radius distance then the arithmetic mean of thier locations is returned. Excludes self and the dead
  - **Returns:** ReferenceScriptHelpers Class

- **`GetGroupMembersByRole Method`**
  ```csharp
  public static IEnumerable&lt;WoWPlayer&gt; GetGroupMembersByRole(
ScriptHelpers.GroupRoleFlags role
)
  ```
  Gets the party members by role including player.
  - *role*: Type: Bots.DungeonBuddy.HelpersAddLanguageSpecificTextSet("LST1C8CBCEF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ScriptHelpersAddLanguageSpecificTextSet("LST1C8CBCEF_3?cs=.|vb=.|cpp=::|nu=.|fs=.");GroupRoleFlags The role. Can 'Or'ed multiple roles together e.g. dps and ranged
  - **Returns:** See Also

- **`GetInsideCircularBossRoom Method`**
  ```csharp
  public static Task&lt;bool&gt; GetInsideCircularBossRoom(
WoWUnit boss,
Vector3 roomCenter,
float roomRadius,
Vector3 pointInsideRoom,
Func&lt;WoWUnit, bool&gt; extraCondition = null
)
  ```
  Gets the inside circular boss room or waits for encounter to end if locked out.
  - *boss*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST38D8FE01_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe boss.
  - *roomCenter*: Type: System.NumericsAddLanguageSpecificTextSet("LST38D8FE01_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The room center.
  - *roomRadius*: Type: SystemAddLanguageSpecificTextSet("LST38D8FE01_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe room radius.
  - *pointInsideRoom*: Type: System.NumericsAddLanguageSpecificTextSet("LST38D8FE01_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The point inside room.
  - **Returns:** See Also

- **`GetInterruptSpell Method`**
  ```csharp
  public static WoWSpell GetInterruptSpell()
  ```
  - **Returns:** ReferenceScriptHelpers Class

- **`GetNearestPointOnLine Method`**
  ```csharp
  public static Vector3 GetNearestPointOnLine(
this Vector3 point,
Vector3 lineStart,
Vector3 lineEnd
)
  ```
  Gets the nearest point on line.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LST294CF9C5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The point.
  - *lineStart*: Type: System.NumericsAddLanguageSpecificTextSet("LST294CF9C5_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The line start.
  - *lineEnd*: Type: System.NumericsAddLanguageSpecificTextSet("LST294CF9C5_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The line end.
  - **Returns:** ReferenceScriptHelpers Class

- **`GetNearestPointOnSegment Method`**
  ```csharp
  public static Vector3 GetNearestPointOnSegment(
this Vector3 point,
Vector3 segmentStart,
Vector3 segmentEnd
)
  ```
  Gets the nearest point on segment.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LSTFAAA4896_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The point.
  - *segmentStart*: Type: System.NumericsAddLanguageSpecificTextSet("LSTFAAA4896_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The segment start point.
  - *segmentEnd*: Type: System.NumericsAddLanguageSpecificTextSet("LSTFAAA4896_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The segment end point.
  - **Returns:** ReferenceScriptHelpers Class

- **`GetOffensiveMagicDispel Method`**
  ```csharp
  public static WoWSpell GetOffensiveMagicDispel()
  ```
  - **Returns:** ReferenceScriptHelpers Class

- **`GetPointsAlongLineSegment Method`**
  ```csharp
  public static IEnumerable&lt;Vector3&gt; GetPointsAlongLineSegment(
Vector3 start,
Vector3 end,
float spacing
)
  ```
  Gets the points along a line segment.
  - *start*: Type: System.NumericsAddLanguageSpecificTextSet("LSTB2BADB3F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The line start.
  - *end*: Type: System.NumericsAddLanguageSpecificTextSet("LSTB2BADB3F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The line end.
  - *spacing*: Type: SystemAddLanguageSpecificTextSet("LSTB2BADB3F_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe distance between points.
  - **Returns:** See Also

- **`GetPoisonDispel Method`**
  ```csharp
  public static WoWSpell GetPoisonDispel()
  ```
  - **Returns:** ReferenceScriptHelpers Class

- **`GetRangedSpell Method`**
  ```csharp
  public static WoWSpell GetRangedSpell(
WoWUnit target
)
  ```
  Returns a ranged spell that class can use on target.
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST2C82AA51_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnit The target.
  - **Returns:** Requires target to be within range and LOS of player

- **`GetSpreadOutLocation Method`**
  ```csharp
  public static Task&lt;Vector3&gt; GetSpreadOutLocation(
Vector3 centerPoint,
float spreadDistance,
float maxDistanceFromCenterPoint
)
  ```
  Gets the spread out location.
  - *centerPoint*: Type: System.NumericsAddLanguageSpecificTextSet("LST4B6C596A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3 The center point.
  - *spreadDistance*: Type: SystemAddLanguageSpecificTextSet("LST4B6C596A_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single The distance to spread from other party members
  - *maxDistanceFromCenterPoint*: Type: SystemAddLanguageSpecificTextSet("LST4B6C596A_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single The max distance from center point.
  - **Returns:** See Also

- **`GetUnfriendlyNpcsAtArea Method`**
  ```csharp
  public static List&lt;WoWUnit&gt; GetUnfriendlyNpcsAtArea(
Vector2[] area,
Predicate&lt;WoWUnit&gt; unitSelector = null
)
  ```
  Gets the unfriendly NPS at area.
  - *area*: Type: AddLanguageSpecificTextSet("LSTE1FF96EE_1?cpp=array&lt;");System.NumericsAddLanguageSpecificTextSet("LSTE1FF96EE_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector2AddLanguageSpecificTextSet("LSTE1FF96EE_3?cpp=&gt;|vb=()|nu=[]"); The area.
  - **Returns:** See Also

- **`GetUnfriendlyNpsAtLocation Method`**
  ```csharp
  public static List&lt;WoWUnit&gt; GetUnfriendlyNpsAtLocation(
Vector3 location,
float radius,
Predicate&lt;WoWUnit&gt; unitSelector = null,
bool checkNavigatable = false
)
  ```
  Returns a list of unfriendly Nps at location.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST41283A88_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST41283A88_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe radius.
  - **Returns:** See Also

- **`HasAura Method`**
  Determines whether the unit has the specified aura and aura meets the conditons given by
                extraQualifier.

- **`HasAura Method (WoWUnit, Int32, Func(WoWAura, Boolean))`**
  ```csharp
  public static bool HasAura(
this WoWUnit unit,
int auraId,
Func&lt;WoWAura, bool&gt; extraQualifier
)
  ```
  Determines whether the unit has the specified aura and aura meets the conditons given by
                extraQualifier.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST11D7C647_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe unit.
  - *auraId*: Type: SystemAddLanguageSpecificTextSet("LST11D7C647_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The aura identifier.
  - *extraQualifier*: Type: SystemAddLanguageSpecificTextSet("LST11D7C647_5?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST11D7C647_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWAura, BooleanAddLanguageSpecificTextSet("LST11D7C647_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The extra qualifier.
  - **Returns:** ReferenceScriptHelpers Class

- **`HasAura Method (WoWUnit, String, Func(WoWAura, Boolean))`**
  ```csharp
  public static bool HasAura(
this WoWUnit unit,
string auraName,
Func&lt;WoWAura, bool&gt; extraQualifier
)
  ```
  Determines whether the unit has the specified aura and aura meets the conditons given by
                extraQualifier.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST6D33A3E4_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe unit.
  - *auraName*: Type: SystemAddLanguageSpecificTextSet("LST6D33A3E4_4?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the aura.
  - *extraQualifier*: Type: SystemAddLanguageSpecificTextSet("LST6D33A3E4_5?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST6D33A3E4_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWAura, BooleanAddLanguageSpecificTextSet("LST6D33A3E4_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The extra qualifier.
  - **Returns:** ReferenceScriptHelpers Class

- **`HasQuest Method`**
  ```csharp
  public static bool HasQuest(
uint questId
)
  ```
  Determines whether the quest is currently in questlog
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LSTF9A07075_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The quest id.
  - **Returns:** See Also

- **`HasQuestAvailable Method`**
  ```csharp
  public static bool HasQuestAvailable(
this WoWUnit unit,
bool lowLevel = false,
bool repeatable = false
)
  ```
  Determines whether unit is offering a quest to the player.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST85A14B7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe unit.
  - **Returns:** ReferenceScriptHelpers Class

- **`HasQuestTurnin Method`**
  ```csharp
  public static bool HasQuestTurnin(
this WoWUnit unit,
bool lowLevel = true,
bool repeatable = false
)
  ```
  Determines whether player has a quest that can be turned in at the unit.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST147291CA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe unit.
  - **Returns:** ReferenceScriptHelpers Class

- **`HasRole Method`**
  Determines whether GroupMember has the specific role

- **`HasRole Method (GroupMember, ScriptHelpers.GroupRoleFlags)`**
  ```csharp
  public static bool HasRole(
this GroupMember groupMember,
ScriptHelpers.GroupRoleFlags role
)
  ```
  Determines whether GroupMember has the specific role
  - *groupMember*: Type: Bots.DungeonBuddyAddLanguageSpecificTextSet("LSTEB83E30B_3?cs=.|vb=.|cpp=::|nu=.|fs=.");GroupMemberA group member
  - *role*: Type: Bots.DungeonBuddy.HelpersAddLanguageSpecificTextSet("LSTEB83E30B_4?cs=.|vb=.|cpp=::|nu=.|fs=.");ScriptHelpersAddLanguageSpecificTextSet("LSTEB83E30B_5?cs=.|vb=.|cpp=::|nu=.|fs=.");GroupRoleFlagsThe role.
  - **Returns:** See Also

- **`HasRole Method (LocalPlayer, ScriptHelpers.GroupRoleFlags)`**
  ```csharp
  public static bool HasRole(
this LocalPlayer me,
ScriptHelpers.GroupRoleFlags role
)
  ```
  Determines whether LocalPlayer has the specific role
  - *me*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTFDFAE700_3?cs=.|vb=.|cpp=::|nu=.|fs=.");LocalPlayerLocalplayer
  - *role*: Type: Bots.DungeonBuddy.HelpersAddLanguageSpecificTextSet("LSTFDFAE700_4?cs=.|vb=.|cpp=::|nu=.|fs=.");ScriptHelpersAddLanguageSpecificTextSet("LSTFDFAE700_5?cs=.|vb=.|cpp=::|nu=.|fs=.");GroupRoleFlagsThe role.
  - **Returns:** See Also

- **`InteractWithObject Method`**
  ```csharp
  public static Task&lt;bool&gt; InteractWithObject(
WoWObject obj,
int channelTimeMs = 0,
bool ignoreCombat = false
)
  ```
  Moves to an object and interacts with it
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTC4749D62_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObject The object selector.
  - **Returns:** See Also

- **`InterruptCast Method`**
  ```csharp
  public static Task&lt;bool&gt; InterruptCast(
WoWUnit target,
params int[] spellIds
)
  ```
  Interrupts the cast.
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST463B64F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe target.
  - *spellIds*: Type: AddLanguageSpecificTextSet("LST463B64F_2?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST463B64F_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LST463B64F_4?cpp=&gt;|vb=()|nu=[]");The spell ids.
  - **Returns:** See Also

- **`IsBossAlive Method`**
  Returns 'true' if specified boss is alive

- **`IsBossAlive Method (String)`**
  ```csharp
  public static bool IsBossAlive(
string name
)
  ```
  Returns 'true' if specified boss is alive
  - *name*: Type: SystemAddLanguageSpecificTextSet("LSTB38385DF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String name of the boss
  - **Returns:** ReferenceScriptHelpers Class

- **`IsBossAlive Method (UInt32)`**
  ```csharp
  public static bool IsBossAlive(
uint id
)
  ```
  Returns 'true' if specified boss is alive
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTC580A5CD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32 Id of the boss
  - **Returns:** ReferenceScriptHelpers Class

- **`IsEmpty Method`**
  ```csharp
  public static bool IsEmpty(
this Targeting targeting
)
  ```
  Determines whether the specified targeting list is empty.
  - *targeting*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LSTC4A65D4D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");TargetingThe targeting.
  - **Returns:** See Also

- **`IsFollower Method`**
  ```csharp
  public static bool IsFollower(
this LocalPlayer me
)
  ```
  Determines whether the bot is follower.
  - *me*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST461A601D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LocalPlayerMe.
  - **Returns:** See Also

- **`IsLeader Method`**
  Determines whether the specified party member is tank.

- **`IsLeader Method (GroupMember)`**
  ```csharp
  public static bool IsLeader(
this GroupMember groupMember
)
  ```
  Determines whether the specified party member is tank.
  - *groupMember*: Type: Bots.DungeonBuddyAddLanguageSpecificTextSet("LSTA3914827_1?cs=.|vb=.|cpp=::|nu=.|fs=.");GroupMemberThe party member.
  - **Returns:** ReferenceScriptHelpers Class

- **`IsLeader Method (LocalPlayer)`**
  ```csharp
  public static bool IsLeader(
this LocalPlayer me
)
  ```
  Determines whether the bot should lead group. same as IsTank for backwards compatibility with scripts.
  - *me*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST1A0F15A6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LocalPlayerMe.
  - **Returns:** See Also

- **`IsMelee Method`**
  ```csharp
  public static bool IsMelee(
this LocalPlayer me
)
  ```
  Returns 'true' if player primarily uses melee range abilites
  - *me*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST9EEEBA97_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LocalPlayerMe.
  - **Returns:** See Also

- **`IsMeleeDps Method`**
  ```csharp
  public static bool IsMeleeDps(
this LocalPlayer me
)
  ```
  Returns 'true' if player primarily uses melee range abilites and has a DPS role
  - *me*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST5FA1BFE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LocalPlayerMe.
  - **Returns:** See Also

- **`IsPointLeftOfLine Method`**
  ```csharp
  public static bool IsPointLeftOfLine(
this Vector3 point,
Vector3 lineStart,
Vector3 lineEnd
)
  ```
  Determines whether point is left of a line.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LST916BB39C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The point.
  - *lineStart*: Type: System.NumericsAddLanguageSpecificTextSet("LST916BB39C_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The line start.
  - *lineEnd*: Type: System.NumericsAddLanguageSpecificTextSet("LST916BB39C_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The line end.
  - **Returns:** See Also

- **`IsQuestInLogComplete Method`**
  ```csharp
  public static bool IsQuestInLogComplete(
uint questId
)
  ```
  Determines whether the quest in the questlog is complete
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LSTC835513F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The quest id.
  - **Returns:** See Also

- **`IsQuestObjectiveComplete Method`**
  ```csharp
  public static bool IsQuestObjectiveComplete(
int objectiveIndex,
uint questId
)
  ```
  Determines whether a quest objective is complete
  - *objectiveIndex*: Type: SystemAddLanguageSpecificTextSet("LSTB60CB32_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int321-based Index of the objective.
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LSTB60CB32_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The quest identifier.
  - **Returns:** ExceptionConditionIndexOutOfRangeExceptionobjectiveIndex is out of range

- **`IsQuestTurnedIn Method`**
  ```csharp
  public static bool IsQuestTurnedIn(
uint questId
)
  ```
  Determines whether specific quest is turned in.
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LST242ED748_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The quest id.
  - **Returns:** See Also

- **`IsRange Method`**
  ```csharp
  public static bool IsRange(
this LocalPlayer me
)
  ```
  Returns 'true' if player primarily uses ranged abilites
  - *me*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTA473B2E2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LocalPlayerMe.
  - **Returns:** See Also

- **`IsRangeDps Method`**
  ```csharp
  public static bool IsRangeDps(
this LocalPlayer me
)
  ```
  Returns 'true' if player primarily uses ranged abilites and has a DPS role
  - *me*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTE99ABC7B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LocalPlayerMe.
  - **Returns:** See Also

- **`IsTank Method`**
  ```csharp
  public static bool IsTank(
this LocalPlayer me
)
  ```
  Determines whether the bot is tank.
  - *me*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST718E8D57_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LocalPlayerMe.
  - **Returns:** See Also

- **`IsType Method`**
  ```csharp
  public static bool IsType(
this PoiType poiType,
params PoiType[] poiTypes
)
  ```
  Determines whether poiType is one of the poiTypes
  - *poiType*: Type: Styx.CommonBot.POIAddLanguageSpecificTextSet("LST481EFED_1?cs=.|vb=.|cpp=::|nu=.|fs=.");PoiType
  - *poiTypes*: Type: AddLanguageSpecificTextSet("LST481EFED_2?cpp=array&lt;");Styx.CommonBot.POIAddLanguageSpecificTextSet("LST481EFED_3?cs=.|vb=.|cpp=::|nu=.|fs=.");PoiTypeAddLanguageSpecificTextSet("LST481EFED_4?cpp=&gt;|vb=()|nu=[]");
  - **Returns:** ReferenceScriptHelpers Class

- **`IsViable Method`**
  ```csharp
  public static bool IsViable(
WoWObject wowObject
)
  ```
  Determines whether the specified wow object is viable e.g. its not null and is valid
  - *wowObject*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST64CF6BE4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe wow object.
  - **Returns:** See Also

- **`LootChest Method`**
  ```csharp
  public static Task&lt;bool&gt; LootChest(
WoWGameObject chest,
bool encounterReward = true
)
  ```
  Loots a chest while respecting loot settings.
  - *chest*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTEAF8CF91_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGameObjectThe chest.
  - **Returns:** See Also

- **`MarkBossAsDead Method`**
  ```csharp
  public static void MarkBossAsDead(
string name,
string reason = null
)
  ```
  Marks the boss as dead.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LSTAC7B8F07_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String The name.
  - **Returns:** ReferenceScriptHelpers Class

- **`MeleeRange Method`**
  ```csharp
  [ObsoleteAttribute("Use WoWUnit.MeleeRange instead")]
public static float MeleeRange(
this WoWUnit unit
)
  ```
  Melee range of unit
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTCA4F2B39_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe unit.
  - **Returns:** ReferenceScriptHelpers Class

- **`MoveInsideBossRoom Method`**
  ```csharp
  public static Task&lt;bool&gt; MoveInsideBossRoom(
WoWUnit boss,
Vector3 leftDoorEdge,
Vector3 rightDoorEdge,
Vector3 pointInsideRoom,
Func&lt;WoWUnit, bool&gt; extraCondition = null
)
  ```
  Gets the inside boss room or waits for encounter to end if locked out.
  - *boss*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST2551EC1D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe boss.
  - *leftDoorEdge*: Type: System.NumericsAddLanguageSpecificTextSet("LST2551EC1D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The left door edge.
  - *rightDoorEdge*: Type: System.NumericsAddLanguageSpecificTextSet("LST2551EC1D_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The right door edge.
  - *pointInsideRoom*: Type: System.NumericsAddLanguageSpecificTextSet("LST2551EC1D_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The point inside room.
  - **Returns:** See Also

- **`MoveOutOfLos Method`**
  ```csharp
  public static Task&lt;bool&gt; MoveOutOfLos(
Func&lt;bool&gt; losWhile,
Vector3 locationToLos,
Vector3 objectLocationToLosAt,
float objectRadius
)
  ```
  This behavior will run out of Line-Of-Sight of locationToLos
            Movement is disabled while not moving so CR will not need to be block since it can't move 
            and interfere with our LOSing
  - *losWhile*: Type: SystemAddLanguageSpecificTextSet("LSTD46C53BC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTD46C53BC_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTD46C53BC_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The los condition. This is passed to DisableMovement(FuncAddLanguageSpecificTextSet("LSTD46C53BC_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LSTD46C53BC_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");)
  - *locationToLos*: Type: System.NumericsAddLanguageSpecificTextSet("LSTD46C53BC_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location you want to get out of LOS with.
  - *objectLocationToLosAt*: Type: System.NumericsAddLanguageSpecificTextSet("LSTD46C53BC_7?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The center location of the object you want to use to LOS
  - *objectRadius*: Type: SystemAddLanguageSpecificTextSet("LSTD46C53BC_8?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe radius of the object you want to use to LOS
  - **Returns:** See Also

- **`MoveToContinue Method`**
  ```csharp
  public static Task&lt;bool&gt; MoveToContinue(
Func&lt;Vector3&gt; destinationProducer,
Func&lt;bool&gt; whileTrue = null,
bool ignoreCombat = false,
string name = ""
)
  ```
  Move to a location and does not return until destination is reached,
            whileTrue returns false, player gets in combat
            and ignoreCombat  is false, player is killed 
            or there's a navigation error.
  - *destinationProducer*: Type: SystemAddLanguageSpecificTextSet("LSTB9839873_1?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTB9839873_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTB9839873_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The destination producer.
  - **Returns:** See Also

- **`NoPoi Method`**
  ```csharp
  public static bool NoPoi(
this BotPoi poi
)
  ```
  Determines whether bot has a POI set.
  - *poi*: Type: Styx.CommonBot.POIAddLanguageSpecificTextSet("LST389CA467_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BotPoiThe poi.
  - **Returns:** ReferenceScriptHelpers Class

- **`PickupQuest Method`**
  ```csharp
  public static Task&lt;bool&gt; PickupQuest(
WoWObject obj,
uint questId = 0
)
  ```
  picks up a quest from object
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST7CF42487_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObject The object selector
  - **Returns:** See Also

- **`PortOutsideAndBackIn Method`**
  ```csharp
  public static Task&lt;bool&gt; PortOutsideAndBackIn()
  ```
  Ports outside of Lfg dungeon and back in to place bot at entrance.
  - **Returns:** See Also

- **`PullNpcToLocation Method`**
  Pull a NPC to location.

- **`PullNpcToLocation Method (Func(Boolean), WoWUnit, Nullable(Vector3), Int32, Single)`**
  ```csharp
  public static Task&lt;bool&gt; PullNpcToLocation(
Func&lt;bool&gt; canRun,
WoWUnit unit,
Nullable&lt;Vector3&gt; pullToLocation,
int waitTimeAtPullLocationTimeoutMs,
float pullToLocationRadius = 1f
)
  ```
  Pull a NPC to location.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST52C88E83_5?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST52C88E83_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST52C88E83_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The 'can run' condition.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST52C88E83_8?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe unit.
  - *pullToLocation*: Type: SystemAddLanguageSpecificTextSet("LST52C88E83_9?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST52C88E83_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST52C88E83_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The pull to location.
  - *waitTimeAtPullLocationTimeoutMs*: Type: SystemAddLanguageSpecificTextSet("LST52C88E83_12?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The wait time at pull location timeout ms.
  - **Returns:** See Also

- **`PullNpcToLocation Method (Func(Boolean), Func(Boolean), WoWUnit, Nullable(Vector3), Nullable(Vector3), Int32, Single, Single)`**
  ```csharp
  public static Task&lt;bool&gt; PullNpcToLocation(
Func&lt;bool&gt; canRun,
Func&lt;bool&gt; readyToPull,
WoWUnit unit,
Nullable&lt;Vector3&gt; pullToLocation,
Nullable&lt;Vector3&gt; waitAtLocation,
int waitTimeAtPullLocationTimeoutMs,
float pullToLocationRadius = 1f,
float waitAtLocationRadius = 2f
)
  ```
  Pull a NPC to location.
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST4ECF64DD_9?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST4ECF64DD_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST4ECF64DD_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The 'can run' condition.
  - *readyToPull*: Type: SystemAddLanguageSpecificTextSet("LST4ECF64DD_12?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST4ECF64DD_13?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST4ECF64DD_14?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The 'ready to pull' condition.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST4ECF64DD_15?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe unit.
  - *pullToLocation*: Type: SystemAddLanguageSpecificTextSet("LST4ECF64DD_16?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST4ECF64DD_17?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST4ECF64DD_18?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The pull to location.
  - *waitAtLocation*: Type: SystemAddLanguageSpecificTextSet("LST4ECF64DD_19?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LST4ECF64DD_20?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST4ECF64DD_21?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The wait at location.
  - *waitTimeAtPullLocationTimeoutMs*: Type: SystemAddLanguageSpecificTextSet("LST4ECF64DD_22?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The wait time at pull location timeout ms.
  - **Returns:** See Also

- **`RestoreMovement Method`**
  ```csharp
  public static void RestoreMovement()
  ```
  Restores movement.

- **`Roles Method`**
  party role.

- **`Roles Method (GroupMember)`**
  ```csharp
  public static ScriptHelpers.GroupRoleFlags Roles(
this GroupMember partyMember
)
  ```
  party role.
  - *partyMember*: Type: Bots.DungeonBuddyAddLanguageSpecificTextSet("LST51B2DD81_2?cs=.|vb=.|cpp=::|nu=.|fs=.");GroupMemberThe party member.
  - **Returns:** See Also

- **`Roles Method (LocalPlayer)`**
  ```csharp
  public static ScriptHelpers.GroupRoleFlags Roles(
this LocalPlayer me
)
  ```
  My party role.
  - *me*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST5008D4FA_2?cs=.|vb=.|cpp=::|nu=.|fs=.");LocalPlayerMe.
  - **Returns:** See Also

- **`RunToTankIfAttacked Method`**
  ```csharp
  public static Task&lt;bool&gt; RunToTankIfAttacked()
  ```
  If a follower, move to tank if being attacked
  - **Returns:** See Also

- **`SetInteractPoi Method`**
  ```csharp
  public static bool SetInteractPoi(
WoWObject interactObject
)
  ```
  Sets the interact POI. This has no effect if POI is already set on the interactObject
            or if there is a more important POI active. 
            Dungeonbuddy will work it's way towards interactObject while 
            prioritizing more important duties such killing aggro or waiting for the dead to be rezed.
            
            Because dungeon scripts have higher execution priority than the actual interaction behavior
            they should fall through (by returning false in
            a coroutine or RunStatus.Failure in a behavior tree) when calling this function; othewise no interaction
            will take place.
  - *interactObject*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTCB727F17_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe interact object.
  - **Returns:** See Also

- **`SetLeaderMoveToPoi Method`**
  ```csharp
  public static bool SetLeaderMoveToPoi(
Vector3 destination,
string destinationName = null
)
  ```
  Sets a moveto POI on tank if 
            PoiType is not set to anything important
            and tank is not within 4 yards of 
            destination.
            Tank will clear a path to destination and wait on group members if needed.
  - *destination*: Type: System.NumericsAddLanguageSpecificTextSet("LSTFF1F0065_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location
  - **Returns:** See Also

- **`SetLeaderMoveToPoiPS Method`**
  ```csharp
  public static RunStatus SetLeaderMoveToPoiPS(
Vector3 destination,
string destinationName = ""
)
  ```
  Sets a moveto POI on tank if
                PoiType is not set to anything important and tank is not within 4 yards of
                destination. Tank will clear a path to destination and wait on group members if needed.
  - *destination*: Type: System.NumericsAddLanguageSpecificTextSet("LSTEE1CCFEA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location
  - **Returns:** See Also

- **`ShouldKillBoss Method`**
  ```csharp
  public static bool ShouldKillBoss(
string name
)
  ```
  Returns true if the boss is not optional or if optional and 'Kill Optional Bosses' is setting is true.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LSTB973E385_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String The name.
  - **Returns:** ReferenceScriptHelpers Class

- **`SleepForRandomReactionTime Method`**
  ```csharp
  public static Task SleepForRandomReactionTime()
  ```
  Sleeps for a random duration in  in the range [200, 700). 
            	Aproximately the time it takes for a human to react to
                in-game events using keyboard as input
  - **Returns:** ReferenceScriptHelpers Class

- **`SleepForRandomUiInteractionTime Method`**
  ```csharp
  public static Task SleepForRandomUiInteractionTime()
  ```
  Sleeps for a random duration in  in the range [900, 2000). Aproximately the time it takes for a human to interect
                with UI elements using mouse.
  - **Returns:** ReferenceScriptHelpers Class

- **`SpreadOut Method`**
  ```csharp
  [ObsoleteAttribute("Use the Avoidance API instead because it does a better job.")]
public static Task&lt;bool&gt; SpreadOut(
Vector3 center,
int distanceToSpread,
float maxDistanceFromCenterPoint
)
  ```
  Spreads the ranged dps out, Healer doesn't move unless too close to melee.
  - *center*: Type: System.NumericsAddLanguageSpecificTextSet("LSTB2826BF1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3 center location of the area to spread out
  - *distanceToSpread*: Type: SystemAddLanguageSpecificTextSet("LSTB2826BF1_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 The distance to spread out from other party members.
  - *maxDistanceFromCenterPoint*: Type: SystemAddLanguageSpecificTextSet("LSTB2826BF1_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single max distance to move from center point
  - **Returns:** See Also

- **`StayAtLocationWhile Method`**
  ```csharp
  public static Task&lt;bool&gt; StayAtLocationWhile(
Func&lt;bool&gt; condition,
Vector3 location,
string locationDescription = null,
float precision = 8f,
params ScriptHelpers.GroupRoleFlags[] roles
)
  ```
  Moves to and stays at location while condition 
            returns true.
            Move is disabled after getting within precision distance of 
            location if current kill/heal target is not reachable
            Scripts SHOULD not make any movement calls while after calling this coroutine to ensure there is no confliction.
  - *condition*: Type: SystemAddLanguageSpecificTextSet("LST7269AE52_2?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST7269AE52_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST7269AE52_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST7269AE52_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *roles*: Type: AddLanguageSpecificTextSet("LST7269AE52_8?cpp=array&lt;");Bots.DungeonBuddy.HelpersAddLanguageSpecificTextSet("LST7269AE52_9?cs=.|vb=.|cpp=::|nu=.|fs=.");ScriptHelpersAddLanguageSpecificTextSet("LST7269AE52_10?cs=.|vb=.|cpp=::|nu=.|fs=.");GroupRoleFlagsAddLanguageSpecificTextSet("LST7269AE52_11?cpp=&gt;|vb=()|nu=[]");The roles.
  - **Returns:** See Also

- **`StopMovingIfMoving Method`**
  ```csharp
  public static Task&lt;bool&gt; StopMovingIfMoving(
string reason = null
)
  ```
  Executes a MoveStop command (MoveStop) 
            if ActiveMover
            is moving and waits for movement to stop
  - **Returns:** See Also

- **`TalkToNpc Method`**
  ```csharp
  public static Task&lt;bool&gt; TalkToNpc(
WoWUnit unit,
params int[] gossipIndexes
)
  ```
  Moves to NPC when not within interact range and then talks to NPC.
            Coroutine returns while moving within interact range however
            once within interact range this coroutine only returns after
            done talking to NPC
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTA241AB39_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe unit to talk to.
  - *gossipIndexes*: Type: AddLanguageSpecificTextSet("LSTA241AB39_2?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTA241AB39_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LSTA241AB39_4?cpp=&gt;|vb=()|nu=[]");The 1-based gossip indexes. defaults selecting gossip index 1 if empty
  - **Returns:** See Also

- **`TankFaceUnitAwayFromGroup Method`**
  Faces current target away from group if have aggro and is tank

- **`TankFaceUnitAwayFromGroup Method (Int32)`**
  ```csharp
  public static Task&lt;bool&gt; TankFaceUnitAwayFromGroup(
int maxDistance
)
  ```
  Faces current target away from group if have aggro and is tank
  - *maxDistance*: Type: SystemAddLanguageSpecificTextSet("LST682D65AC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 The max distance.
  - **Returns:** See Also

- **`TankFaceUnitAwayFromGroup Method (WoWUnit, Int32)`**
  ```csharp
  public static Task&lt;bool&gt; TankFaceUnitAwayFromGroup(
WoWUnit unit,
int maxDistance
)
  ```
  Faces unit away from group if have aggro and is tank
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST8B94DB5B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnit The unit.
  - *maxDistance*: Type: SystemAddLanguageSpecificTextSet("LST8B94DB5B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 The max distance.
  - **Returns:** See Also

- **`TankTalkToAndEscortNpc Method`**
  If tank then talk to and escort NPC; defending NPC from attackers.
            It is the callers responsibility to decide if esort is over.

- **`TankTalkToAndEscortNpc Method (UInt32, Vector3, Single, Int32[])`**
  ```csharp
  public static Task&lt;bool&gt; TankTalkToAndEscortNpc(
uint escortNpcId,
Vector3 escortNpcStartLocation,
float followDistance = 10f,
params int[] gossipIndexes
)
  ```
  If tank then talk to and escort NPC; defending NPC from attackers.
            It is the callers responsibility to decide if esort is over.
  - *escortNpcId*: Type: SystemAddLanguageSpecificTextSet("LSTEC3F8357_3?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The escort NPC identifier.
  - *escortNpcStartLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LSTEC3F8357_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The escort NPC starting location.
  - *gossipIndexes*: Type: AddLanguageSpecificTextSet("LSTEC3F8357_6?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTEC3F8357_7?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LSTEC3F8357_8?cpp=&gt;|vb=()|nu=[]");1-based indexes of the gossip. Selects gossip index 1 if empty
  - **Returns:** See Also

- **`TankTalkToAndEscortNpc Method (WoWUnit, Vector3, Single, Int32[])`**
  ```csharp
  public static Task&lt;bool&gt; TankTalkToAndEscortNpc(
WoWUnit escortNpc,
Vector3 escortNpcStartLocation,
float followDistance = 10f,
params int[] gossipIndexes
)
  ```
  If tank then talk to and escort NPC; defending NPC from attackers.
            It is the callers responsibility to decide if esort is over.
  - *escortNpc*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST76B969E6_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe escort NPC.
  - *escortNpcStartLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LST76B969E6_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
            The escort NPC starting location.
            Tank will move to this location if escortNpc is null
  - *gossipIndexes*: Type: AddLanguageSpecificTextSet("LST76B969E6_6?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST76B969E6_7?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LST76B969E6_8?cpp=&gt;|vb=()|nu=[]");1-based indexes of the gossip. Selects gossip index 1 if empty
  - **Returns:** See Also

- **`TankUnitAtLocation Method`**
  ```csharp
  public static Task&lt;bool&gt; TankUnitAtLocation(
Vector3 location,
float precision
)
  ```
  Tanks the unit at location.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST65D6ED31_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location to tanke at.
  - *precision*: Type: SystemAddLanguageSpecificTextSet("LST65D6ED31_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe precision.
  - **Returns:** See Also

- **`TelportOutsideLfgInstance Method`**
  ```csharp
  public static void TelportOutsideLfgInstance()
  ```
  Telports outside current LFG instance

- **`TurninQuest Method`**
  ```csharp
  public static Task&lt;bool&gt; TurninQuest(
WoWObject obj,
uint questId = 0
)
  ```
  Turns in a quest at object
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTB73F9359_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObject The turnin object.
  - **Returns:** See Also

- **`UnfriendlyNpcsArePulledOrGone Method`**
  ```csharp
  public static bool UnfriendlyNpcsArePulledOrGone(
Vector3 location,
float radius
)
  ```
  Returns true if there are no unfriendly NPCs near location or if any of the NPCs have been pulled.
            Used for blacklisting of packs.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST4F734B71_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST4F734B71_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe radius.
  - **Returns:** ReferenceScriptHelpers Class

- **`UnitId Method`**
  ```csharp
  public static string UnitId(
this WoWPlayer player
)
  ```
  - *player*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST760925BC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPlayer
  - **Returns:** ReferenceScriptHelpers Class

- **`WillPullAggroAtLocation Method`**
  ```csharp
  public static bool WillPullAggroAtLocation(
Vector3 loc
)
  ```
  Checks if moving to location will draw aggro.
  - *loc*: Type: System.NumericsAddLanguageSpecificTextSet("LST9E75875D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3 The loc.
  - **Returns:** ReferenceScriptHelpers Class

#### ScriptHelpers Fields

- **`Rnd Field`**
  ```csharp
  public static readonly Random Rnd
  ```

- **`StrafeManager Field`**
  ```csharp
  public static readonly StrafeManager StrafeManager
  ```
  The strafe manager

---

### ScriptHelpers.AngleSpan Structure

```csharp
public struct AngleSpan
```

Initializes a new instance of the ScriptHelpers.AngleSpan class

#### AngleSpan Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceScriptHelpersAddLanguageSpecificTextSet("LSTAB4D6AED_2?cs=.|vb=.|cpp=::|nu=.|fs=.");AngleSpan Structure

#### AngleSpan Fields

- **`Angle Field`**
  ```csharp
  public readonly int Angle
  ```
  Direction in Degrees

- **`BehindAngle Field`**
  ```csharp
  public const int BehindAngle = 180
  ```

- **`FrontAngle Field`**
  ```csharp
  public const int FrontAngle = 0
  ```

- **`LeftSideAngle Field`**
  ```csharp
  public const int LeftSideAngle = 90
  ```

- **`RightSideAngle Field`**
  ```csharp
  public const int RightSideAngle = 270
  ```

- **`Span Field`**
  ```csharp
  public readonly int Span
  ```
  The Span

---

### ScriptHelpers.EnemyDispelType Enumeration

```csharp
public enum EnemyDispelType
```

Enemy Dispel Type

---

### ScriptHelpers.GroupRoleFlags Enumeration

```csharp
[FlagsAttribute]
public enum GroupRoleFlags
```

Group Role flags

---

### ScriptHelpers.PartyDispelType Enumeration

```csharp
public enum PartyDispelType
```

---

### SpellActionButton Class

**Inheritance:** System.Object → Bots.DungeonBuddy.Helpers.SpellActionButton

```csharp
[ObsoleteAttribute("Use the Styx.CommonBot.Bars.ActionBar API")]
public class SpellActionButton
```

Initializes a new instance of the SpellActionButton class

#### SpellActionButton Properties

- **`CanUse Property`**
  ```csharp
  public bool CanUse { get; }
  ```
  return true if action can be used and is off cooldown

- **`Charges Property`**
  ```csharp
  public int Charges { get; }
  ```

- **`CoolDownLeft Property`**
  ```csharp
  public TimeSpan CoolDownLeft { get; }
  ```

- **`ExtraActionButton Property`**
  ```csharp
  public static SpellActionButton ExtraActionButton { get; }
  ```
  Gets the extra action button.

- **`IsEnabled Property`**
  ```csharp
  public bool IsEnabled { get; }
  ```

- **`IsValid Property`**
  ```csharp
  public bool IsValid { get; }
  ```

- **`MaxCharges Property`**
  ```csharp
  public int MaxCharges { get; }
  ```

- **`OnCooldown Property`**
  ```csharp
  public bool OnCooldown { get; }
  ```

- **`OverrideActionBarButtons Property`**
  ```csharp
  public static List&lt;SpellActionButton&gt; OverrideActionBarButtons { get; }
  ```

- **`PetActionButtons Property`**
  ```csharp
  public static List&lt;SpellActionButton&gt; PetActionButtons { get; }
  ```
  returns active pet's list of actions. Note: some pet actions such as 'Attack' have no spell Id

- **`Slot Property`**
  ```csharp
  public int Slot { get; }
  ```

- **`Spell Property`**
  ```csharp
  public WoWSpell Spell { get; }
  ```

- **`SpellId Property`**
  ```csharp
  public int SpellId { get; }
  ```

- **`VehicleActionButtons Property`**
  ```csharp
  public static List&lt;SpellActionButton&gt; VehicleActionButtons { get; }
  ```

#### SpellActionButton Methods

- **`Update Method`**
  ```csharp
  public void Update()
  ```

- **`Use Method`**
  ```csharp
  public void Use()
  ```

---

### StrafeManager Class

**Inheritance:** System.Object → Bots.DungeonBuddy.Helpers.StrafeManager

```csharp
public class StrafeManager
```

Initializes a new instance of the StrafeManager class

#### StrafeManager Methods

- **`Move Method`**
  ```csharp
  public void Move(
Func&lt;Vector3&gt; moveToProducer,
Func&lt;Vector3&gt; faceLocationProducer,
Func&lt;bool&gt; condition = null,
string reason = null
)
  ```
  Moves to the specific location while constantly facing the given location. Useful for tanks dragging mobs to
                tanking position or side step powerful attacks.
  - *moveToProducer*: Type: SystemAddLanguageSpecificTextSet("LST3009629E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST3009629E_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST3009629E_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The move to producer.
  - *faceLocationProducer*: Type: SystemAddLanguageSpecificTextSet("LST3009629E_4?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST3009629E_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LST3009629E_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The face location producer.

- **`Pulse Method`**
  ```csharp
  public void Pulse()
  ```

- **`Stop Method`**
  ```csharp
  public void Stop()
  ```

---

### WaitContinue(T) Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.GroupComposite → Styx.TreeSharp.Decorator → Styx.TreeSharp.Wait → Styx.TreeSharp.WaitContinue → Bots.DungeonBuddy.Helpers.WaitContinue.T

```csharp
[ObsoleteAttribute("Use coroutines")]
public class WaitContinue&lt;T&gt; : WaitContinue
```

Uses a strongly typed context

#### WaitContinue(T) Constructor

- **`WaitContinue(T) Constructor (Int32, Predicate(T), Composite)`**
  ```csharp
  public WaitContinue(
int timeoutSeconds,
Predicate&lt;T&gt; canRun,
Composite child
)
  ```
  Initializes a new instance of the WaitContinue.T class
  - *timeoutSeconds*: Type: SystemAddLanguageSpecificTextSet("LST73EFC631_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST73EFC631_7?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LST73EFC631_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST73EFC631_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST73EFC631_10?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite

- **`WaitContinue(T) Constructor (TimeSpan, Predicate(T), Composite)`**
  ```csharp
  public WaitContinue(
TimeSpan timeout,
Predicate&lt;T&gt; canRun,
Composite child
)
  ```
  Initializes a new instance of the WaitContinue.T class
  - *timeout*: Type: SystemAddLanguageSpecificTextSet("LST1C445000_6?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpan
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST1C445000_7?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LST1C445000_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST1C445000_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST1C445000_10?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite

- **`WaitContinue(T) Constructor (WaitGetTimeoutDelegate, Predicate(T), Composite)`**
  ```csharp
  public WaitContinue(
WaitGetTimeoutDelegate timeoutRetriever,
Predicate&lt;T&gt; canRun,
Composite child
)
  ```
  Initializes a new instance of the WaitContinue.T class
  - *timeoutRetriever*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST6787BA61_6?cs=.|vb=.|cpp=::|nu=.|fs=.");WaitGetTimeoutDelegate
  - *canRun*: Type: SystemAddLanguageSpecificTextSet("LST6787BA61_7?cs=.|vb=.|cpp=::|nu=.|fs=.");PredicateAddLanguageSpecificTextSet("LST6787BA61_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST6787BA61_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *child*: Type: Styx.TreeSharpAddLanguageSpecificTextSet("LST6787BA61_10?cs=.|vb=.|cpp=::|nu=.|fs=.");Composite

#### WaitContinue(T) Methods

- **`CanRun Method`**
  ```csharp
  protected override bool CanRun(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LST6AA478BF_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceWaitContinueAddLanguageSpecificTextSet("LST6AA478BF_4?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST6AA478BF_5?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---
