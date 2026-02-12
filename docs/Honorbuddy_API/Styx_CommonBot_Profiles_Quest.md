# Styx.CommonBot.Profiles.Quest

Contains profile related quest classes.

## Contents

- [CollectFrom Class](#collectfrom-class)
- [CollectFromCollection Class](#collectfromcollection-class)
- [CollectFromType Enumeration](#collectfromtype-enumeration)
- [CollectItemObjectiveInfo Class](#collectitemobjectiveinfo-class)
- [KillMobObjectiveInfo Class](#killmobobjectiveinfo-class)
- [ObjectiveInfo Class](#objectiveinfo-class)
- [ObjectiveType Enumeration](#objectivetype-enumeration)
- [QuestInfo Class](#questinfo-class)
- [TurnInObjectiveInfo Class](#turninobjectiveinfo-class)
- [UseObjectObjectiveInfo Class](#useobjectobjectiveinfo-class)

---

### CollectFrom Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.CollectFrom

```csharp
public class CollectFrom
```

A collect from.

#### CollectFrom Methods

- **`FromXML Method`**
  ```csharp
  public static CollectFrom FromXML(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST19726751_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.ProfileMissingAttributeException Thrown when a profile missing
            attribute exception error condition occurs.ProfileException                       Thrown when a Profile error condition
            occurs.

#### CollectFrom Fields

- **`ID Field`**
  ```csharp
  public uint ID
  ```
  The identifier.

- **`Name Field`**
  ```csharp
  public string Name
  ```
  The name.

- **`Type Field`**
  ```csharp
  public CollectFromType Type
  ```
  The type.

---

### CollectFromCollection Class

**Inheritance:** System.Object → System.Collections.Generic.List.CollectFrom → Styx.CommonBot.Profiles.Quest.CollectFromCollection

```csharp
public class CollectFromCollection : List&lt;CollectFrom&gt;
```

Collection of collect froms.

#### CollectFromCollection Constructor

- **`CollectFromCollection Constructor`**
  ```csharp
  public CollectFromCollection()
  ```
  Default constructor.

- **`CollectFromCollection Constructor (Int32)`**
  ```csharp
  public CollectFromCollection(
int capacity
)
  ```
  Constructor.
  - *capacity*: Type: SystemAddLanguageSpecificTextSet("LSTF3CC8472_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The capacity.

- **`CollectFromCollection Constructor (IEnumerable(CollectFrom))`**
  ```csharp
  public CollectFromCollection(
IEnumerable&lt;CollectFrom&gt; collection
)
  ```
  Constructor.
  - *collection*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTD68B1B51_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTD68B1B51_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");CollectFromAddLanguageSpecificTextSet("LSTD68B1B51_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The collection.

#### CollectFromCollection Methods

- **`ContainsGameObject Method`**
  ```csharp
  public bool ContainsGameObject(
uint gameObjectID
)
  ```
  Query if 'gameObjectID' contains game object.
  - *gameObjectID*: Type: SystemAddLanguageSpecificTextSet("LST1D139AF4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32Identifier for the game object.
  - **Returns:** ReferenceCollectFromCollection Class

- **`ContainsMob Method`**
  ```csharp
  public bool ContainsMob(
uint mobID
)
  ```
  Query if 'mobID' contains mob.
  - *mobID*: Type: SystemAddLanguageSpecificTextSet("LST689EEA25_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32Identifier for the mob.
  - **Returns:** ReferenceCollectFromCollection Class

- **`ContainsVendor Method`**
  ```csharp
  public bool ContainsVendor(
uint vendorID
)
  ```
  Query if 'vendorID' contains vendor.
  - *vendorID*: Type: SystemAddLanguageSpecificTextSet("LSTC8221965_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32Identifier for the vendor.
  - **Returns:** ReferenceCollectFromCollection Class

---

### CollectFromType Enumeration

```csharp
public enum CollectFromType
```

Values that represent CollectFromType.

---

### CollectItemObjectiveInfo Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.ObjectiveInfo → Styx.CommonBot.Profiles.Quest.CollectItemObjectiveInfo

```csharp
public class CollectItemObjectiveInfo : ObjectiveInfo
```

Information about the collect item objective.

#### CollectItemObjectiveInfo Properties

- **`Count Property`**
  ```csharp
  public uint Count { get; }
  ```
  Gets the number of.

- **`ItemID Property`**
  ```csharp
  public uint ItemID { get; }
  ```
  Gets the identifier of the item.

- **`OverridedCollectFrom Property`**
  ```csharp
  public CollectFromCollection OverridedCollectFrom { get; }
  ```
  Gets the overrided collect from.

- **`OverridedHotspots Property`**
  ```csharp
  public HotspotCollection OverridedHotspots { get; }
  ```
  Gets the overrided hotspots.

- **`TargetMaxLevel Property`**
  ```csharp
  public int TargetMaxLevel { get; }
  ```
  Gets target maximum level.

- **`TargetMinLevel Property`**
  ```csharp
  public int TargetMinLevel { get; }
  ```
  Gets target minimum level.

---

### KillMobObjectiveInfo Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.ObjectiveInfo → Styx.CommonBot.Profiles.Quest.KillMobObjectiveInfo

```csharp
public class KillMobObjectiveInfo : ObjectiveInfo
```

Information about the kill mob objective.

#### KillMobObjectiveInfo Properties

- **`Count Property`**
  ```csharp
  public uint Count { get; }
  ```
  Gets the number of.

- **`MobID Property`**
  ```csharp
  public uint MobID { get; }
  ```
  Gets the identifier of the mob.

- **`OverridedHotspots Property`**
  ```csharp
  public HotspotCollection OverridedHotspots { get; }
  ```
  Gets the overrided hotspots.

- **`TargetMaxLevel Property`**
  ```csharp
  public int TargetMaxLevel { get; }
  ```
  Gets target maximum level.

- **`TargetMinLevel Property`**
  ```csharp
  public int TargetMinLevel { get; }
  ```
  Gets target minimum level.

---

### ObjectiveInfo Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.ObjectiveInfo → Styx.CommonBot.Profiles.Quest.CollectItemObjectiveInfo → Styx.CommonBot.Profiles.Quest.KillMobObjectiveInfo → Styx.CommonBot.Profiles.Quest.TurnInObjectiveInfo → Styx.CommonBot.Profiles.Quest.UseObjectObjectiveInfo

```csharp
public class ObjectiveInfo
```

Information about the objective.

#### ObjectiveInfo Properties

- **`Type Property`**
  ```csharp
  public ObjectiveType Type { get; }
  ```
  Gets the type.

#### ObjectiveInfo Methods

- **`FromXML Method`**
  ```csharp
  public static ObjectiveInfo FromXML(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST4E4D3930_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileException              Thrown when a Profile error condition occurs.ProfileUnknownElementExceptionThrown when a Profile Unknown Element error
            condition occurs.

---

### ObjectiveType Enumeration

```csharp
public enum ObjectiveType
```

Values that represent ObjectiveType.

---

### QuestInfo Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.QuestInfo

```csharp
public class QuestInfo
```

Information about the question.

#### QuestInfo Constructor

- **`QuestInfo Constructor (IEnumerable(UInt32), String)`**
  ```csharp
  public QuestInfo(
IEnumerable&lt;uint&gt; variantIds,
string name
)
  ```
  Constructor.
  - *variantIds*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST58EB6149_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST58EB6149_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");UInt32AddLanguageSpecificTextSet("LST58EB6149_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");  The variant identifiers.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LST58EB6149_5?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name.

- **`QuestInfo Constructor (UInt32, String)`**
  ```csharp
  [ObsoleteAttribute("Use the other constructor")]
public QuestInfo(
uint id,
string name
)
  ```
  Initializes a new instance of the QuestInfo class
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST1571E1EB_0?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *name*: Type: SystemAddLanguageSpecificTextSet("LST1571E1EB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String

#### QuestInfo Properties

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`Objectives Property`**
  ```csharp
  public List&lt;ObjectiveInfo&gt; Objectives { get; }
  ```
  Gets the objectives.

- **`VariantIds Property`**
  ```csharp
  public IReadOnlyCollection&lt;uint&gt; VariantIds { get; }
  ```
  Gets the identifier.

#### QuestInfo Methods

- **`FindCollectItem Method`**
  ```csharp
  public CollectItemObjectiveInfo FindCollectItem(
uint itemID
)
  ```
  Searches for the first collect item.
  - *itemID*: Type: SystemAddLanguageSpecificTextSet("LST3B762B76_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32Identifier for the item.
  - **Returns:** ReferenceQuestInfo Class

- **`FindKillMob Method`**
  ```csharp
  public KillMobObjectiveInfo FindKillMob(
uint mobId
)
  ```
  Searches for the first kill mob.
  - *mobId*: Type: SystemAddLanguageSpecificTextSet("LSTDDC93EA7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32Identifier for the mob.
  - **Returns:** ReferenceQuestInfo Class

- **`FindTurnIn Method`**
  ```csharp
  public TurnInObjectiveInfo FindTurnIn()
  ```
  Searches for the first turn in.
  - **Returns:** ReferenceQuestInfo Class

- **`FindUseGameObject Method`**
  ```csharp
  public UseObjectObjectiveInfo FindUseGameObject(
uint gameObjectID
)
  ```
  Searches for the first use game object.
  - *gameObjectID*: Type: SystemAddLanguageSpecificTextSet("LSTCC76021_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32Identifier for the game object.
  - **Returns:** ReferenceQuestInfo Class

- **`FromXML Method`**
  ```csharp
  public static QuestInfo FromXML(
XElement element
)
  ```
  Convers an XML element to quest info.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTFB133511_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element that contains the quest info.
  - **Returns:** ExceptionConditionProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.ProfileMissingAttributeException Thrown when a profile missing
            attribute exception error condition occurs.

---

### TurnInObjectiveInfo Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.ObjectiveInfo → Styx.CommonBot.Profiles.Quest.TurnInObjectiveInfo

```csharp
public class TurnInObjectiveInfo : ObjectiveInfo
```

Information about the turn in objective.

#### TurnInObjectiveInfo Properties

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; }
  ```
  Gets the location.

---

### UseObjectObjectiveInfo Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Quest.ObjectiveInfo → Styx.CommonBot.Profiles.Quest.UseObjectObjectiveInfo

```csharp
public class UseObjectObjectiveInfo : ObjectiveInfo
```

Information about the use object objective.

#### UseObjectObjectiveInfo Properties

- **`Count Property`**
  ```csharp
  public uint Count { get; }
  ```
  Gets the number of.

- **`ObjectID Property`**
  ```csharp
  public uint ObjectID { get; }
  ```
  Gets the identifier of the object.

- **`OverridedHotspots Property`**
  ```csharp
  public HotspotCollection OverridedHotspots { get; }
  ```
  Gets the overrided hotspots.

#### UseObjectObjectiveInfo Methods

- **`FromXML Method`**
  ```csharp
  public static UseObjectObjectiveInfo FromXML(
XElement element
)
  ```
  Initializes this object from the given from XML.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST91BAA6EA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.
  - **Returns:** ExceptionConditionProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.ProfileAttributeExpectedExceptionThrown when a profile attribute
            expected exception error condition occurs.ProfileException                       Thrown when a Profile error condition
            occurs.

---
