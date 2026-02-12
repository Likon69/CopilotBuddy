# Bots.DungeonBuddy.Attributes

Contains DungeonBuddy attributes.

## Contents

- [CallBehaviorMode Enumeration](#callbehaviormode-enumeration)
- [DynamicStringListAttribute Class](#dynamicstringlistattribute-class)
- [EncounterHandlerAttribute Class](#encounterhandlerattribute-class)
- [LocationHandlerAttribute Class](#locationhandlerattribute-class)
- [ObjectHandlerAttribute Class](#objecthandlerattribute-class)
- [ScenarioStageAttribute Class](#scenariostageattribute-class)

---

### CallBehaviorMode Enumeration

```csharp
public enum CallBehaviorMode
```

---

### DynamicStringListAttribute Class

**Inheritance:** System.Object → System.Attribute → Bots.DungeonBuddy.Attributes.DynamicStringListAttribute

```csharp
public sealed class DynamicStringListAttribute : Attribute
```

This attribute is for use with a property grid that needs to show a dynamic enumeration inside a combo box.

#### DynamicStringListAttribute Properties

- **`ValueRetriever Property`**
  ```csharp
  public Func&lt;ITypeDescriptorContext, ICollection&gt; ValueRetriever { get; }
  ```

---

### EncounterHandlerAttribute Class

**Inheritance:** System.Object → System.Attribute → Bots.DungeonBuddy.Attributes.EncounterHandlerAttribute

```csharp
public sealed class EncounterHandlerAttribute : Attribute
```

Initializes a new instance of the EncounterHandlerAttribute class.

#### EncounterHandlerAttribute Constructor

- **`EncounterHandlerAttribute Constructor (UInt32)`**
  ```csharp
  public EncounterHandlerAttribute(
uint bossEntry
)
  ```
  Initializes a new instance of the EncounterHandlerAttribute class.
  - *bossEntry*: Type: SystemAddLanguageSpecificTextSet("LST4C9BC897_0?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The boss entry id.

- **`EncounterHandlerAttribute Constructor (UInt32, String)`**
  ```csharp
  public EncounterHandlerAttribute(
uint bossEntry,
string bossDisplayName
)
  ```
  Initializes a new instance of the EncounterHandlerAttribute class.
  - *bossEntry*: Type: SystemAddLanguageSpecificTextSet("LST7DE943AD_0?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The boss entry id.
  - *bossDisplayName*: Type: SystemAddLanguageSpecificTextSet("LST7DE943AD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringDisplay name of the boss.

- **`EncounterHandlerAttribute Constructor (UInt32, String, Int32, CallBehaviorMode)`**
  ```csharp
  public EncounterHandlerAttribute(
uint bossEntry,
string bossDisplayName,
int bossRange,
CallBehaviorMode mode
)
  ```
  Initializes a new instance of the EncounterHandlerAttribute class.
  - *bossEntry*: Type: SystemAddLanguageSpecificTextSet("LST86C9C4E0_0?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The boss entry id.
  - *bossDisplayName*: Type: SystemAddLanguageSpecificTextSet("LST86C9C4E0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringDisplay name of the boss.
  - *bossRange*: Type: SystemAddLanguageSpecificTextSet("LST86C9C4E0_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The boss range. Default (75).
  - *mode*: Type: Bots.DungeonBuddy.AttributesAddLanguageSpecificTextSet("LST86C9C4E0_3?cs=.|vb=.|cpp=::|nu=.|fs=.");CallBehaviorModeThe mode. Default(CallBehavior.InCombat)

#### EncounterHandlerAttribute Properties

- **`BossEntry Property`**
  ```csharp
  public uint BossEntry { get; set; }
  ```

- **`BossName Property`**
  ```csharp
  public string BossName { get; set; }
  ```

- **`BossRange Property`**
  ```csharp
  public int BossRange { get; set; }
  ```

- **`BossRangeSqr Property`**
  ```csharp
  public int BossRangeSqr { get; }
  ```

- **`Mode Property`**
  ```csharp
  public CallBehaviorMode Mode { get; set; }
  ```

---

### LocationHandlerAttribute Class

**Inheritance:** System.Object → System.Attribute → Bots.DungeonBuddy.Attributes.LocationHandlerAttribute

```csharp
public sealed class LocationHandlerAttribute : Attribute
```

Initializes a new instance of the LocationHandlerAttribute class

#### LocationHandlerAttribute Constructor

- **`LocationHandlerAttribute Constructor (Double, Double, Double)`**
  ```csharp
  public LocationHandlerAttribute(
double x,
double y,
double z
)
  ```
  Initializes a new instance of the LocationHandlerAttribute class
  - *x*: Type: SystemAddLanguageSpecificTextSet("LST7F65BADF_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Double
  - *y*: Type: SystemAddLanguageSpecificTextSet("LST7F65BADF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Double
  - *z*: Type: SystemAddLanguageSpecificTextSet("LST7F65BADF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Double

- **`LocationHandlerAttribute Constructor (Single, Single, Single)`**
  ```csharp
  public LocationHandlerAttribute(
float x,
float y,
float z
)
  ```
  Initializes a new instance of the LocationHandlerAttribute class
  - *x*: Type: SystemAddLanguageSpecificTextSet("LST4C759E40_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *y*: Type: SystemAddLanguageSpecificTextSet("LST4C759E40_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *z*: Type: SystemAddLanguageSpecificTextSet("LST4C759E40_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single

- **`LocationHandlerAttribute Constructor (Double, Double, Double, Single)`**
  ```csharp
  public LocationHandlerAttribute(
double x,
double y,
double z,
float radius
)
  ```
  Initializes a new instance of the LocationHandlerAttribute class
  - *x*: Type: SystemAddLanguageSpecificTextSet("LSTC652B298_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Double
  - *y*: Type: SystemAddLanguageSpecificTextSet("LSTC652B298_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Double
  - *z*: Type: SystemAddLanguageSpecificTextSet("LSTC652B298_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Double
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LSTC652B298_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single

- **`LocationHandlerAttribute Constructor (Single, Single, Single, Single)`**
  ```csharp
  public LocationHandlerAttribute(
float x,
float y,
float z,
float radius
)
  ```
  Initializes a new instance of the LocationHandlerAttribute class
  - *x*: Type: SystemAddLanguageSpecificTextSet("LST52E92A11_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *y*: Type: SystemAddLanguageSpecificTextSet("LST52E92A11_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *z*: Type: SystemAddLanguageSpecificTextSet("LST52E92A11_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST52E92A11_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single

- **`LocationHandlerAttribute Constructor (Double, Double, Double, Single, String)`**
  ```csharp
  public LocationHandlerAttribute(
double x,
double y,
double z,
float radius,
string locationDisplayName
)
  ```
  Initializes a new instance of the LocationHandlerAttribute class
  - *x*: Type: SystemAddLanguageSpecificTextSet("LST454B19EC_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Double
  - *y*: Type: SystemAddLanguageSpecificTextSet("LST454B19EC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Double
  - *z*: Type: SystemAddLanguageSpecificTextSet("LST454B19EC_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Double
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST454B19EC_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *locationDisplayName*: Type: SystemAddLanguageSpecificTextSet("LST454B19EC_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`LocationHandlerAttribute Constructor (Single, Single, Single, Single, String)`**
  ```csharp
  public LocationHandlerAttribute(
float x,
float y,
float z,
float radius,
string locationDisplayName
)
  ```
  Initializes a new instance of the LocationHandlerAttribute class
  - *x*: Type: SystemAddLanguageSpecificTextSet("LSTCE2FFA07_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *y*: Type: SystemAddLanguageSpecificTextSet("LSTCE2FFA07_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *z*: Type: SystemAddLanguageSpecificTextSet("LSTCE2FFA07_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LSTCE2FFA07_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single
  - *locationDisplayName*: Type: SystemAddLanguageSpecificTextSet("LSTCE2FFA07_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String

#### LocationHandlerAttribute Properties

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; }
  ```

- **`LocationDisplayName Property`**
  ```csharp
  public string LocationDisplayName { get; }
  ```

- **`Radius Property`**
  ```csharp
  public float Radius { get; }
  ```

#### LocationHandlerAttribute Fields

- **`RadiusSqr Field`**
  ```csharp
  public readonly float RadiusSqr
  ```

---

### ObjectHandlerAttribute Class

**Inheritance:** System.Object → System.Attribute → Bots.DungeonBuddy.Attributes.ObjectHandlerAttribute

```csharp
public sealed class ObjectHandlerAttribute : Attribute
```

Initializes a new instance of the ObjectHandlerAttribute class

#### ObjectHandlerAttribute Constructor

- **`ObjectHandlerAttribute Constructor (Int32)`**
  ```csharp
  public ObjectHandlerAttribute(
int objectEntryId
)
  ```
  Initializes a new instance of the ObjectHandlerAttribute class
  - *objectEntryId*: Type: SystemAddLanguageSpecificTextSet("LSTDA8786FE_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32

- **`ObjectHandlerAttribute Constructor (Int32, String)`**
  ```csharp
  public ObjectHandlerAttribute(
int objectEntryId,
string objectDisplayName
)
  ```
  Initializes a new instance of the ObjectHandlerAttribute class
  - *objectEntryId*: Type: SystemAddLanguageSpecificTextSet("LSTC781C28A_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
  - *objectDisplayName*: Type: SystemAddLanguageSpecificTextSet("LSTC781C28A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`ObjectHandlerAttribute Constructor (Int32, String, Single)`**
  ```csharp
  public ObjectHandlerAttribute(
int objectEntryId,
string objectDisplayName,
float range
)
  ```
  Initializes a new instance of the ObjectHandlerAttribute class
  - *objectEntryId*: Type: SystemAddLanguageSpecificTextSet("LSTD274DD3F_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
  - *objectDisplayName*: Type: SystemAddLanguageSpecificTextSet("LSTD274DD3F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *range*: Type: SystemAddLanguageSpecificTextSet("LSTD274DD3F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single

#### ObjectHandlerAttribute Properties

- **`ObjectEntry Property`**
  ```csharp
  public int ObjectEntry { get; set; }
  ```

- **`ObjectName Property`**
  ```csharp
  public string ObjectName { get; set; }
  ```

- **`ObjectRange Property`**
  ```csharp
  public float ObjectRange { get; set; }
  ```

- **`ObjectRangeSqr Property`**
  ```csharp
  public float ObjectRangeSqr { get; }
  ```

---

### ScenarioStageAttribute Class

**Inheritance:** System.Object → System.Attribute → Bots.DungeonBuddy.Attributes.ScenarioStageAttribute

```csharp
public sealed class ScenarioStageAttribute : Attribute
```

Initializes a new instance of the ScenarioStageAttribute class

#### ScenarioStageAttribute Constructor

- **`ScenarioStageAttribute Constructor (Int32)`**
  ```csharp
  public ScenarioStageAttribute(
int stageNumber
)
  ```
  Initializes a new instance of the ScenarioStageAttribute class
  - *stageNumber*: Type: SystemAddLanguageSpecificTextSet("LSTFE8A5E6F_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32

- **`ScenarioStageAttribute Constructor (Int32, String)`**
  ```csharp
  public ScenarioStageAttribute(
int stageNumber,
string stageName
)
  ```
  Initializes a new instance of the ScenarioStageAttribute class
  - *stageNumber*: Type: SystemAddLanguageSpecificTextSet("LST40205605_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
  - *stageName*: Type: SystemAddLanguageSpecificTextSet("LST40205605_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`ScenarioStageAttribute Constructor (Int32, String, Int32)`**
  ```csharp
  public ScenarioStageAttribute(
int stageNumber,
string stageName,
int stepNumber
)
  ```
  Initializes a new instance of the ScenarioStageAttribute class
  - *stageNumber*: Type: SystemAddLanguageSpecificTextSet("LSTC1431788_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
  - *stageName*: Type: SystemAddLanguageSpecificTextSet("LSTC1431788_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *stepNumber*: Type: SystemAddLanguageSpecificTextSet("LSTC1431788_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32

#### ScenarioStageAttribute Properties

- **`StageName Property`**
  ```csharp
  public string StageName { get; set; }
  ```

- **`StageNumber Property`**
  ```csharp
  public int StageNumber { get; set; }
  ```

- **`StepNumber Property`**
  ```csharp
  public int StepNumber { get; set; }
  ```

---
