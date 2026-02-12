# Styx.CommonBot.CharacterManagement

Contains classes related to managing the character while the bot runs.

## Contents

- [AutoEquipper Class](#autoequipper-class)
- [CharacterManager Class](#charactermanager-class)
- [ClassProfile Class](#classprofile-class)
- [ClassProfileLoadException Class](#classprofileloadexception-class)
- [ClassProfileLocalization Class](#classprofilelocalization-class)
- [DetailedWeaponStyle Enumeration](#detailedweaponstyle-enumeration)
- [TalentPlacement Class](#talentplacement-class)
- [TalentPlacementSet Class](#talentplacementset-class)
- [TalentSelector Class](#talentselector-class)
- [WeaponStyle Enumeration](#weaponstyle-enumeration)
- [WeighableStatType Enumeration](#weighablestattype-enumeration)
- [WeightSet Class](#weightset-class)

---

### AutoEquipper Class

**Inheritance:** System.Object → Styx.CommonBot.CharacterManagement.AutoEquipper

```csharp
public class AutoEquipper
```

Represents a class that implements operations to auto equip items for the current character.

#### AutoEquipper Properties

- **`DisregardTime Property`**
  ```csharp
  public int DisregardTime { get; set; }
  ```
  Gets or sets the "disregard" time, in milliseconds, of the AutoEquip item check. AutoEquip will automatically recheck an item after it has not been checked for this period of time.

- **`TargetCheckTime Property`**
  ```csharp
  public int TargetCheckTime { get; set; }
  ```
  Gets or sets the target time, in milliseconds, of the AutoEquip item check. AutoEquip will run through all bag items over a period of this time.

#### AutoEquipper Methods

- **`GetInventorySlots Method`**
  ```csharp
  public IEnumerable&lt;WoWInventorySlot&gt; GetInventorySlots(
ItemInfo item
)
  ```
  - *item*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST82724D00_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ItemInfo
  - **Returns:** See Also

---

### CharacterManager Class

**Inheritance:** System.Object → Styx.CommonBot.CharacterManagement.CharacterManager

```csharp
public static class CharacterManager
```

Gets the auto equipper instance.

#### CharacterManager Properties

- **`AutoEquip Property`**
  ```csharp
  public static AutoEquipper AutoEquip { get; }
  ```
  Gets the auto equipper instance.

- **`ClassProfiles Property`**
  ```csharp
  public static ReadOnlyCollection&lt;ClassProfile&gt; ClassProfiles { get; }
  ```
  Gets a list of class profiles loaded from the HB data folder.

- **`CurrentClassProfile Property`**
  ```csharp
  public static ClassProfile CurrentClassProfile { get; }
  ```
  Gets the current class profile.

- **`Talented Property`**
  ```csharp
  public static TalentSelector Talented { get; }
  ```
  Gets the talent selector instance.

#### CharacterManager Methods

- **`SetClassProfile Method`**
  ```csharp
  public static void SetClassProfile(
ClassProfile classProfile
)
  ```
  Sets the current class profile to the specified profile.
  - *classProfile*: Type: Styx.CommonBot.CharacterManagementAddLanguageSpecificTextSet("LST6F2E434E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ClassProfileThe class profile.

---

### ClassProfile Class

**Inheritance:** System.Object → Styx.CommonBot.CharacterManagement.ClassProfile

```csharp
public class ClassProfile
```

Represents a class profile.

#### ClassProfile Properties

- **`Class Property`**
  ```csharp
  public WoWClass Class { get; }
  ```
  Gets the class that this class profile corresponds to.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name of this class profile.

- **`Specialization Property`**
  ```csharp
  public int Specialization { get; }
  ```
  Gets the talent specialization index of this class profile. This is 1-based - Warrior Arms has Specialization 1, for example.

- **`Talents Property`**
  ```csharp
  public TalentPlacementSet Talents { get; }
  ```
  Gets the talents.

- **`WeightSet Property`**
  ```csharp
  public WeightSet WeightSet { get; }
  ```
  Gets the weight set of this class profile.

#### ClassProfile Methods

- **`GetSpec Method`**
  ```csharp
  public WoWSpec GetSpec()
  ```
  Gets the WoWSpec from the Class and Specialization members.
  - **Returns:** ReferenceClassProfile Class

- **`Load Method`**
  ```csharp
  public static ClassProfile Load(
string filePath
)
  ```
  Loads a ClassProfile instance from a file path.
  - *filePath*: Type: SystemAddLanguageSpecificTextSet("LSTD55B2903_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe path to the file.
  - **Returns:** ExceptionConditionArgumentNullExceptionThrown if filePath is null.FileNotFoundExceptionThrown if the file referred to by filePath doesn't exist.ClassProfileLoadExceptionThrown if the class profile fails loading.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Gets a string representation of this ClassProfile.
  - **Returns:** ReferenceClassProfile Class

---

### ClassProfileLoadException Class

**Inheritance:** System.Object → System.Exception → Styx.CommonBot.CharacterManagement.ClassProfileLoadException

```csharp
public class ClassProfileLoadException : Exception
```

Initializes a new instance of the ClassProfileLoadException class

#### ClassProfileLoadException Constructor

- **`ClassProfileLoadException Constructor (String, Exception)`**
  ```csharp
  public ClassProfileLoadException(
string message,
Exception innerException = null
)
  ```
  Initializes a new instance of the ClassProfileLoadException class
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST86EB011D_0?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`ClassProfileLoadException Constructor (String, String, Exception)`**
  ```csharp
  public ClassProfileLoadException(
string filePath,
string additionalInfo = null,
Exception innerException = null
)
  ```
  Initializes a new instance of the ClassProfileLoadException class
  - *filePath*: Type: SystemAddLanguageSpecificTextSet("LSTE6E1C873_0?cs=.|vb=.|cpp=::|nu=.|fs=.");String

#### ClassProfileLoadException Properties

- **`FilePath Property`**
  ```csharp
  public string FilePath { get; }
  ```

---

### ClassProfileLocalization Class

**Inheritance:** System.Object → Styx.CommonBot.CharacterManagement.ClassProfileLocalization

```csharp
public static class ClassProfileLocalization
```

Returns the localized name of the specified WoWClass using HB's current UI culture.

#### ClassProfileLocalization Methods

- **`GetClassName Method`**
  ```csharp
  public static string GetClassName(
WoWClass cls
)
  ```
  Returns the localized name of the specified WoWClass using HB's current UI culture.
  - *cls*: Type: StyxAddLanguageSpecificTextSet("LST4BE1FF46_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWClass
  - **Returns:** ReferenceClassProfileLocalization Class

- **`GetSpecName Method`**
  ```csharp
  public static string GetSpecName(
WoWSpec spec
)
  ```
  Returns the localized name of the specified WoWSpec using HB's current UI culture.
  - *spec*: Type: StyxAddLanguageSpecificTextSet("LST85261A3C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpec
  - **Returns:** ReferenceClassProfileLocalization Class

---

### DetailedWeaponStyle Enumeration

```csharp
public enum DetailedWeaponStyle
```

---

### TalentPlacement Class

**Inheritance:** System.Object → Styx.CommonBot.CharacterManagement.TalentPlacement

```csharp
public class TalentPlacement
```

Represents a talent placement.

#### TalentPlacement Properties

- **`Index Property`**
  ```csharp
  public int Index { get; }
  ```
  Gets the index of this talent.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name of this talent.

- **`Tier Property`**
  ```csharp
  public int Tier { get; }
  ```
  Gets the tier of this talent.

---

### TalentPlacementSet Class

**Inheritance:** System.Object → Styx.CommonBot.CharacterManagement.TalentPlacementSet

```csharp
public class TalentPlacementSet : IEnumerable&lt;TalentPlacement&gt;,
IEnumerable
```

Represents a set of talent placements.

#### TalentPlacementSet Constructor

- **`TalentPlacementSet Constructor`**
  ```csharp
  public TalentPlacementSet()
  ```
  Constructs a new empty instance of a talent placement set.

- **`TalentPlacementSet Constructor (IEnumerable(TalentPlacement))`**
  ```csharp
  public TalentPlacementSet(
IEnumerable&lt;TalentPlacement&gt; talents
)
  ```
  Constructs a new instance from an enumerable of talent placements.
  - *talents*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTCAF5C851_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTCAF5C851_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TalentPlacementAddLanguageSpecificTextSet("LSTCAF5C851_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
            The talent placements. The length must be less than or equal to NumTalents.
            If a talent placement has the same Tier as a previous TalentPlacement, an exception is thrown.

#### TalentPlacementSet Properties

- **`Item Property`**
  ```csharp
  public TalentPlacement this[
int tier
] { get; }
  ```
  Gets a TalentPlacement for the specified 0-based tier.
  - *tier*: Type: SystemAddLanguageSpecificTextSet("LST5697BDF0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The 0-based tier.
  - **Returns:** ExceptionConditionArgumentOutOfRangeExceptionThrown if tier is less than 0, or greater than or equal to NumTalents.

#### TalentPlacementSet Methods

- **`GetEnumerator Method`**
  ```csharp
  public IEnumerator&lt;TalentPlacement&gt; GetEnumerator()
  ```
  Gets an enumerator for this TalentPlacementSet.
  - **Returns:** See Also

#### TalentPlacementSet Fields

- **`NumTalentChoicesPerTier Field`**
  ```csharp
  public const int NumTalentChoicesPerTier = 3
  ```
  The number of talent choices per tier.

- **`NumTalents Field`**
  ```csharp
  public const int NumTalents = 7
  ```
  The number of talent tiers.

---

### TalentSelector Class

**Inheritance:** System.Object → Styx.CommonBot.CharacterManagement.TalentSelector

```csharp
public class TalentSelector
```

Represents a class that implements operations to automatically select talents as the character levels.

#### TalentSelector Properties

- **`CheckInterval Property`**
  ```csharp
  public int CheckInterval { get; set; }
  ```
  Gets or sets the interval between checks for learning new talents, in milliseconds. Defaults to 5000 ms.

---

### WeaponStyle Enumeration

```csharp
public enum WeaponStyle
```

Represents weapon styles.

---

### WeighableStatType Enumeration

```csharp
public enum WeighableStatType
```

Represents stat types that can be weighed, and are used by WeightSet.

---

### WeightSet Class

**Inheritance:** System.Object → Styx.CommonBot.CharacterManagement.WeightSet

```csharp
public class WeightSet
```

Represents a set of weights.

#### WeightSet Properties

- **`Weights Property`**
  ```csharp
  public Dictionary&lt;WeighableStatType, float&gt; Weights { get; }
  ```
  The weights. This is guaranteed to be nonnull.

#### WeightSet Methods

- **`EvaluateItem Method`**
  Evaluates an item by summing the scores for each weighable stat.

- **`EvaluateItem Method (WoWItem, Boolean)`**
  ```csharp
  public float EvaluateItem(
WoWItem item,
bool factorArmorClass = false
)
  ```
  Evaluates an item by summing the scores for each weighable stat.
  - *item*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST689C770D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWItemThe item.
  - **Returns:** ExceptionConditionArgumentNullExceptionThrown if item is null.

- **`EvaluateItem Method (ItemInfo, ItemStats, Boolean)`**
  ```csharp
  public float EvaluateItem(
ItemInfo itemInfo,
ItemStats itemStats,
bool factorArmorClass = false
)
  ```
  Evaluates an item by summing the scores for each weighable stat.
  - *itemInfo*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST5024FFA8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ItemInfoThe item info.
  - *itemStats*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST5024FFA8_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ItemStatsThe item stats.
  - **Returns:** ExceptionConditionArgumentNullExceptionThrown if itemInfo or itemStats is null.

---
