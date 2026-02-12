# Styx.WoWInternals.DB

## Contents

- [BattlePetSpecies Class](#battlepetspecies-class)
- [CharShipment Class](#charshipment-class)
- [CharShipmentContainer Class](#charshipmentcontainer-class)
- [Creature Class](#creature-class)
- [CriteriaTree Class](#criteriatree-class)
- [CurrencyType Class](#currencytype-class)
- [Db2Table Class](#db2table-class)
- [GameObject Class](#gameobject-class)
- [GarrAbility Class](#garrability-class)
- [GarrAbilityCategory Class](#garrabilitycategory-class)
- [GarrAbilityEffect Class](#garrabilityeffect-class)
- [GarrAbilityEffectCategory Enumeration](#garrabilityeffectcategory-enumeration)
- [GarrBuilding Class](#garrbuilding-class)
- [GarrClassSpec Class](#garrclassspec-class)
- [GarrEncounter Class](#garrencounter-class)
- [GarrEncounterXMechanic Class](#garrencounterxmechanic-class)
- [GarrFollower Class](#garrfollower-class)
- [GarrMechanic Class](#garrmechanic-class)
- [GarrMechanicType Class](#garrmechanictype-class)
- [GarrMission Class](#garrmission-class)
- [GarrPlotInstance Class](#garrplotinstance-class)
- [GarrSiteLevel Class](#garrsitelevel-class)
- [GarrisonBuildingType Enumeration](#garrisonbuildingtype-enumeration)
- [GarrisonFollowerType Enumeration](#garrisonfollowertype-enumeration)
- [GarrisonMissionType Enumeration](#garrisonmissiontype-enumeration)
- [ItemDisenchantLoot Class](#itemdisenchantloot-class)
- [ItemEffect Class](#itemeffect-class)
- [ItemEffectList Class](#itemeffectlist-class)
- [ItemEffectTriggerType Enumeration](#itemeffecttriggertype-enumeration)
- [ItemEntry Structure](#itementry-structure)
- [ItemExtendedCost Class](#itemextendedcost-class)
- [ItemSparseEntry Structure](#itemsparseentry-structure)
- [MissileTargeting Class](#missiletargeting-class)
- [PetType Enumeration](#pettype-enumeration)
- [PlotType Enumeration](#plottype-enumeration)
- [Scenario Class](#scenario-class)
- [ScenarioStep Class](#scenariostep-class)
- [ScenarioType Enumeration](#scenariotype-enumeration)
- [SpellMissile Class](#spellmissile-class)
- [TaxiNode Class](#taxinode-class)
- [TotemCategory Class](#totemcategory-class)
- [UILocomotionType Enumeration](#uilocomotiontype-enumeration)
- [Vehicle Class](#vehicle-class)
- [VehicleFlags Enumeration](#vehicleflags-enumeration)
- [WoWDb Class](#wowdb-class)
- [WoWDbRow Class](#wowdbrow-class)
- [WoWDbTable Class](#wowdbtable-class)

---

### BattlePetSpecies Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.BattlePetSpecies

```csharp
public class BattlePetSpecies
```

Represents a battle pet.

#### BattlePetSpecies Properties

- **`CanBattle Property`**
  ```csharp
  public bool CanBattle { get; }
  ```
  true if the species can be used in pet battles; otherwise false.

- **`CreatureId Property`**
  ```csharp
  public uint CreatureId { get; }
  ```
  The creature ID of this species.

- **`Flags Property`**
  ```csharp
  public uint Flags { get; }
  ```
  The flags.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  The ID of this species.

- **`IsObtainable Property`**
  ```csharp
  public bool IsObtainable { get; }
  ```
  true if the species is obtainable; otherwise false.

- **`IsTradeable Property`**
  ```csharp
  public bool IsTradeable { get; }
  ```
  true if the species is tradeable; otherwise false.

- **`IsUnique Property`**
  ```csharp
  public bool IsUnique { get; }
  ```
  true if the species is unique; otherwise false.

- **`PetType Property`**
  ```csharp
  public PetType PetType { get; }
  ```
  The type of this species.

- **`Spell Property`**
  ```csharp
  public WoWSpell Spell { get; }
  ```
  The spell for this species.

- **`SpellId Property`**
  ```csharp
  public int SpellId { get; }
  ```
  The spell ID for this species.

- **`TooltipDescription Property`**
  ```csharp
  public string TooltipDescription { get; }
  ```
  The description to show in the tooltip of this species.

- **`TooltipSource Property`**
  ```csharp
  public string TooltipSource { get; }
  ```
  The source to show in the tooltip of this species.

#### BattlePetSpecies Methods

- **`FromId Method`**
  ```csharp
  public static BattlePetSpecies FromId(
uint id
)
  ```
  Gets a BattlePetSpecies instance for the specifies ID.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST5467CEF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The ID.
  - **Returns:** ReferenceBattlePetSpecies Class

---

### CharShipment Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.CharShipment

```csharp
public class CharShipment
```

Determines whether the specified object is equal to the current object.

#### CharShipment Properties

- **`CharShipmentContainer Property`**
  ```csharp
  public CharShipmentContainer CharShipmentContainer { get; }
  ```

- **`CharShipmentContainerId Property`**
  ```csharp
  public uint CharShipmentContainerId { get; }
  ```

- **`Duration Property`**
  ```csharp
  public int Duration { get; }
  ```

- **`Dword0 Property`**
  ```csharp
  public int Dword0 { get; }
  ```

- **`Dword10 Property`**
  ```csharp
  public int Dword10 { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`ItemSparseId Property`**
  ```csharp
  public int ItemSparseId { get; }
  ```

- **`Spell Property`**
  ```csharp
  public WoWSpell Spell { get; }
  ```

- **`SpellId Property`**
  ```csharp
  public int SpellId { get; }
  ```

- **`Word14 Property`**
  ```csharp
  public short Word14 { get; }
  ```

#### CharShipment Methods

- **`FromId Method`**
  ```csharp
  public static CharShipment FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST87D3A6BA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceCharShipment Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a string that represents the current object.
  - **Returns:** ReferenceCharShipment Class

---

### CharShipmentContainer Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.CharShipmentContainer

```csharp
public class CharShipmentContainer
```

Determines whether the specified object is equal to the current object.

#### CharShipmentContainer Properties

- **`BuildingType Property`**
  ```csharp
  public GarrisonBuildingType BuildingType { get; }
  ```

- **`Byte14 Property`**
  ```csharp
  public byte Byte14 { get; }
  ```

- **`Byte16 Property`**
  ```csharp
  public byte Byte16 { get; }
  ```

- **`Byte17 Property`**
  ```csharp
  public byte Byte17 { get; }
  ```

- **`Byte18 Property`**
  ```csharp
  public byte Byte18 { get; }
  ```

- **`Byte1A Property`**
  ```csharp
  public byte Byte1A { get; }
  ```

- **`Description Property`**
  ```csharp
  public string Description { get; }
  ```

- **`Dword1B Property`**
  ```csharp
  public int Dword1B { get; }
  ```

- **`FactionIndex Property`**
  ```csharp
  public byte FactionIndex { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```

- **`Word10 Property`**
  ```csharp
  public short Word10 { get; }
  ```

- **`Word12 Property`**
  ```csharp
  public short Word12 { get; }
  ```

- **`Word8 Property`**
  ```csharp
  public short Word8 { get; }
  ```

- **`WordA Property`**
  ```csharp
  public short WordA { get; }
  ```

- **`WordC Property`**
  ```csharp
  public short WordC { get; }
  ```

- **`WordE Property`**
  ```csharp
  public short WordE { get; }
  ```

#### CharShipmentContainer Methods

- **`FromId Method`**
  ```csharp
  public static CharShipmentContainer FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTDE776845_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceCharShipmentContainer Class

---

### Creature Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.Creature

```csharp
public class Creature
```

Determines whether the specified object is equal to the current object.

#### Creature Properties

- **`byte40 Property`**
  ```csharp
  public byte byte40 { get; }
  ```

- **`byte41 Property`**
  ```csharp
  public byte byte41 { get; }
  ```

- **`byte42 Property`**
  ```csharp
  public byte byte42 { get; }
  ```

- **`byte43 Property`**
  ```csharp
  public byte byte43 { get; }
  ```

- **`CursorName Property`**
  ```csharp
  public string CursorName { get; }
  ```

- **`dwordC Property`**
  ```csharp
  public int dwordC { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```

- **`Title Property`**
  ```csharp
  public string Title { get; }
  ```

- **`TitleAlt Property`**
  ```csharp
  public string TitleAlt { get; }
  ```

#### Creature Methods

- **`FromId Method`**
  ```csharp
  public static Creature FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTFCF3C741_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceCreature Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceCreature Class

---

### CriteriaTree Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.CriteriaTree

```csharp
public class CriteriaTree
```

Determines whether the specified object is equal to the current object.

#### CriteriaTree Properties

- **`Amount Property`**
  ```csharp
  public int Amount { get; }
  ```

- **`CriteriaId Property`**
  ```csharp
  public uint CriteriaId { get; }
  ```

- **`Description Property`**
  ```csharp
  public string Description { get; }
  ```

- **`Flags Property`**
  ```csharp
  public uint Flags { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

#### CriteriaTree Methods

- **`FromId Method`**
  ```csharp
  public static CriteriaTree FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST117F31AF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceCriteriaTree Class

---

### CurrencyType Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.CurrencyType

```csharp
public class CurrencyType
```

Represents a currency type in WoW

#### CurrencyType Properties

- **`CategoryId Property`**
  ```csharp
  public uint CategoryId { get; }
  ```
  Gets the category identifier.

- **`Description Property`**
  ```csharp
  public string Description { get; }
  ```
  Gets the description.

- **`Flags Property`**
  ```csharp
  public uint Flags { get; }
  ```
  Gets the flags.

- **`HasDynamicCap Property`**
  ```csharp
  public bool HasDynamicCap { get; }
  ```
  Gets a bool that indicates whether this currency has a dynamic cap.

- **`HasWeeklyDynamicCap Property`**
  ```csharp
  public bool HasWeeklyDynamicCap { get; }
  ```
  Gets a bool that indicates whether this currency has a weekly dynamic cap.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Gets the identifier.

- **`InventoryIconId Property`**
  ```csharp
  public int InventoryIconId { get; }
  ```
  Gets the inventory icon ID.

- **`MaxEarnablePerWeek Property`**
  ```csharp
  public int MaxEarnablePerWeek { get; }
  ```
  Gets the maximum earnable per week.

- **`MaxQuantity Property`**
  ```csharp
  public int MaxQuantity { get; }
  ```
  Gets the maximum quantity.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`Quality Property`**
  ```csharp
  public WoWItemQuality Quality { get; }
  ```
  Gets the quality.

- **`SpellCategory Property`**
  ```csharp
  public int SpellCategory { get; }
  ```
  Gets the spell category.

#### CurrencyType Methods

- **`FromId Method`**
  ```csharp
  public static CurrencyType FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST2E4A33B5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceCurrencyType Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceCurrencyType Class

---

### Db2Table Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.WoWDbTable → Styx.WoWInternals.DB.Db2Table

```csharp
public class Db2Table : WoWDbTable
```

Gets the base address of this table.

#### Db2Table Properties

- **`MaxId Property`**
  ```csharp
  public override int MaxId { get; }
  ```

- **`MinId Property`**
  ```csharp
  public override int MinId { get; }
  ```

- **`NumRows Property`**
  ```csharp
  public override int NumRows { get; }
  ```

- **`RequiredFieldCount Property`**
  ```csharp
  public int RequiredFieldCount { get; }
  ```

- **`RowSize Property`**
  ```csharp
  public override int RowSize { get; }
  ```

- **`TotalFieldCount Property`**
  ```csharp
  public int TotalFieldCount { get; }
  ```

#### Db2Table Methods

- **`EnumerateIdRowPairs Method`**
  ```csharp
  public override IEnumerable&lt;KeyValuePair&lt;int, WoWDbRow&gt;&gt; EnumerateIdRowPairs()
  ```
  - **Returns:** See Also

- **`GetEnumerator Method`**
  ```csharp
  public override IEnumerator&lt;WoWDbRow&gt; GetEnumerator()
  ```
  - **Returns:** See Also

- **`GetRow Method`**
  ```csharp
  public override WoWDbRow GetRow(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTD0925C73_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceDb2Table Class

---

### GameObject Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.GameObject

```csharp
public class GameObject
```

Gets the iterator.

#### GameObject Properties

- **`Collection Property`**
  ```csharp
  public static IEnumerable&lt;GameObject&gt; Collection { get; }
  ```
  Gets the iterator.

- **`DisplayId Property`**
  ```csharp
  public int DisplayId { get; }
  ```
  Gets the display identifier.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Gets the identifier.

- **`MapId Property`**
  ```csharp
  public int MapId { get; }
  ```
  Gets the map identifier.

- **`Matrix Property`**
  ```csharp
  public Matrix4x4 Matrix { get; }
  ```

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`Position Property`**
  ```csharp
  public Vector3 Position { get; }
  ```
  Gets the position.

- **`Properties Property`**
  ```csharp
  public int[] Properties { get; }
  ```
  Gets the properties.

- **`Quaternion Property`**
  ```csharp
  public Quaternion Quaternion { get; }
  ```
  Gets the quaternion.

- **`Scale Property`**
  ```csharp
  public float Scale { get; }
  ```
  Gets the scale.

- **`Type Property`**
  ```csharp
  public WoWGameObjectType Type { get; }
  ```
  Gets the type.

#### GameObject Methods

- **`FromId Method`**
  ```csharp
  public static GameObject FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTB7D3D041_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceGameObject Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents this instance.
  - **Returns:** ReferenceGameObject Class

---

### GarrAbility Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.GarrAbility

```csharp
public class GarrAbility
```

Determines whether the specified object is equal to the current object.

#### GarrAbility Properties

- **`Description Property`**
  ```csharp
  public string Description { get; }
  ```

- **`Effects Property`**
  ```csharp
  public IReadOnlyList&lt;GarrAbilityEffect&gt; Effects { get; }
  ```

- **`Flags Property`**
  ```csharp
  public ushort Flags { get; }
  ```

- **`FollowerTypeId Property`**
  ```csharp
  public byte FollowerTypeId { get; }
  ```

- **`GarrAbilityCategoryId Property`**
  ```csharp
  public byte GarrAbilityCategoryId { get; }
  ```

- **`IconFileDataId Property`**
  ```csharp
  public uint IconFileDataId { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`IsTrait Property`**
  ```csharp
  public bool IsTrait { get; }
  ```

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```

- **`OtherFactionGarrAbilityId Property`**
  ```csharp
  public ushort OtherFactionGarrAbilityId { get; }
  ```

#### GarrAbility Methods

- **`FromId Method`**
  ```csharp
  public static GarrAbility FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST45339206_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceGarrAbility Class

---

### GarrAbilityCategory Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.GarrAbilityCategory

```csharp
public class GarrAbilityCategory
```

Determines whether the specified object is equal to the current object.

#### GarrAbilityCategory Properties

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```

#### GarrAbilityCategory Methods

- **`FromId Method`**
  ```csharp
  public static GarrAbilityCategory FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTC3EC424E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceGarrAbilityCategory Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a string that represents the current object.
  - **Returns:** ReferenceGarrAbilityCategory Class

---

### GarrAbilityEffect Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.GarrAbilityEffect

```csharp
public class GarrAbilityEffect
```

Determines whether the specified object is equal to the current object.

#### GarrAbilityEffect Properties

- **`Ability Property`**
  ```csharp
  public GarrAbility Ability { get; }
  ```

- **`ApplyType Property`**
  ```csharp
  public int ApplyType { get; }
  ```

- **`Category Property`**
  ```csharp
  public GarrAbilityEffectCategory Category { get; }
  ```

- **`CounterGarrMechanicType Property`**
  ```csharp
  public GarrMechanicType CounterGarrMechanicType { get; }
  ```

- **`CounterGarrMechanicTypeId Property`**
  ```csharp
  public uint CounterGarrMechanicTypeId { get; }
  ```

- **`GarrAbilityId Property`**
  ```csharp
  public uint GarrAbilityId { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`Modifier Property`**
  ```csharp
  public float Modifier { get; }
  ```

- **`UiAnimRaceInfoId Property`**
  ```csharp
  public uint UiAnimRaceInfoId { get; }
  ```

#### GarrAbilityEffect Methods

- **`FromId Method`**
  ```csharp
  public static GarrAbilityEffect FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST388619C5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceGarrAbilityEffect Class

- **`FromParentId Method`**
  ```csharp
  public static IReadOnlyList&lt;GarrAbilityEffect&gt; FromParentId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST21CB1449_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** See Also

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a string that represents the current object.
  - **Returns:** ReferenceGarrAbilityEffect Class

---

### GarrAbilityEffectCategory Enumeration

```csharp
public enum GarrAbilityEffectCategory
```

---

### GarrBuilding Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.GarrBuilding

```csharp
public class GarrBuilding
```

Determines whether the specified object is equal to the current object.

#### GarrBuilding Properties

- **`AllianceGameObject Property`**
  ```csharp
  public GameObject AllianceGameObject { get; }
  ```

- **`AllianceGameObjectId Property`**
  ```csharp
  public uint AllianceGameObjectId { get; }
  ```

- **`BuildDuration Property`**
  ```csharp
  public int BuildDuration { get; }
  ```

- **`Description Property`**
  ```csharp
  public string Description { get; }
  ```

- **`HordeGameObject Property`**
  ```csharp
  public GameObject HordeGameObject { get; }
  ```

- **`HordeGameObjectId Property`**
  ```csharp
  public uint HordeGameObjectId { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`Level Property`**
  ```csharp
  public byte Level { get; }
  ```

- **`NameAlliance Property`**
  ```csharp
  public string NameAlliance { get; }
  ```

- **`NameHorde Property`**
  ```csharp
  public string NameHorde { get; }
  ```

- **`Rank Property`**
  ```csharp
  public byte Rank { get; }
  ```

- **`ShipmentCapacity Property`**
  ```csharp
  public int ShipmentCapacity { get; }
  ```

- **`Tooltip Property`**
  ```csharp
  public string Tooltip { get; }
  ```

- **`Type Property`**
  ```csharp
  public GarrisonBuildingType Type { get; }
  ```

#### GarrBuilding Methods

- **`FromId Method`**
  ```csharp
  public static GarrBuilding FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTDA9B2578_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceGarrBuilding Class

---

### GarrClassSpec Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.GarrClassSpec

```csharp
public class GarrClassSpec
```

Determines whether the specified object is equal to the current object.

#### GarrClassSpec Properties

- **`ClassAtlasId Property`**
  ```csharp
  public ushort ClassAtlasId { get; }
  ```

- **`Flags Property`**
  ```csharp
  public byte Flags { get; }
  ```

- **`GarrFollItemSetID Property`**
  ```csharp
  public byte GarrFollItemSetID { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`Limit Property`**
  ```csharp
  public byte Limit { get; }
  ```

- **`NameFemale Property`**
  ```csharp
  public string NameFemale { get; }
  ```

- **`NameGenderless Property`**
  ```csharp
  public string NameGenderless { get; }
  ```

- **`NameMale Property`**
  ```csharp
  public string NameMale { get; }
  ```

#### GarrClassSpec Methods

- **`FromId Method`**
  ```csharp
  public static GarrClassSpec FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTEA6C76A7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceGarrClassSpec Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceGarrClassSpec Class

---

### GarrEncounter Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.GarrEncounter

```csharp
public class GarrEncounter
```

Determines whether the specified object is equal to the current object.

#### GarrEncounter Properties

- **`CreatureId Property`**
  ```csharp
  public int CreatureId { get; }
  ```

- **`dword16 Property`**
  ```csharp
  public int dword16 { get; }
  ```

- **`Height Property`**
  ```csharp
  public float Height { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`Mechanics Property`**
  ```csharp
  public IReadOnlyList&lt;GarrEncounterXMechanic&gt; Mechanics { get; }
  ```

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```

- **`PortraitFileDataId Property`**
  ```csharp
  public int PortraitFileDataId { get; }
  ```

- **`Scale Property`**
  ```csharp
  public float Scale { get; }
  ```

- **`UiTextureKitId Property`**
  ```csharp
  public short UiTextureKitId { get; }
  ```

#### GarrEncounter Methods

- **`FromId Method`**
  ```csharp
  public static GarrEncounter FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST855A1177_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceGarrEncounter Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a string that represents the current object.
  - **Returns:** ReferenceGarrEncounter Class

---

### GarrEncounterXMechanic Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.GarrEncounterXMechanic

```csharp
public class GarrEncounterXMechanic
```

Determines whether the specified object is equal to the current object.

#### GarrEncounterXMechanic Properties

- **`GarrEncounter Property`**
  ```csharp
  public GarrEncounter GarrEncounter { get; }
  ```

- **`GarrEncounterId Property`**
  ```csharp
  public uint GarrEncounterId { get; }
  ```

- **`GarrMechanic Property`**
  ```csharp
  public GarrMechanic GarrMechanic { get; }
  ```

- **`GarrMechanicId Property`**
  ```csharp
  public uint GarrMechanicId { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

#### GarrEncounterXMechanic Methods

- **`FromId Method`**
  ```csharp
  public static GarrEncounterXMechanic FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST948D8481_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceGarrEncounterXMechanic Class

- **`FromParentId Method`**
  ```csharp
  public static IReadOnlyList&lt;GarrEncounterXMechanic&gt; FromParentId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTCEFDF2E5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** See Also

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a string that represents the current object.
  - **Returns:** ReferenceGarrEncounterXMechanic Class

---

### GarrFollower Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.GarrFollower

```csharp
public class GarrFollower
```

Determines whether the specified object is equal to the current object.

#### GarrFollower Properties

- **`AllianceAddedBroadcastTextId Property`**
  ```csharp
  public uint AllianceAddedBroadcastTextId { get; }
  ```

- **`AllianceFlavorTextGarrStringId Property`**
  ```csharp
  public byte AllianceFlavorTextGarrStringId { get; }
  ```

- **`AllianceGarrClassSpecId Property`**
  ```csharp
  public byte AllianceGarrClassSpecId { get; }
  ```

- **`AllianceGarrFollItemSetId Property`**
  ```csharp
  public ushort AllianceGarrFollItemSetId { get; }
  ```

- **`AllianceListPortraitTextureKitId Property`**
  ```csharp
  public ushort AllianceListPortraitTextureKitId { get; }
  ```

- **`AlliancePortraitIconId Property`**
  ```csharp
  public uint AlliancePortraitIconId { get; }
  ```

- **`AllianceUiAnimRaceInfoId Property`**
  ```csharp
  public byte AllianceUiAnimRaceInfoId { get; }
  ```

- **`Class Property`**
  ```csharp
  public byte Class { get; }
  ```

- **`CreatureAlliance Property`**
  ```csharp
  public Creature CreatureAlliance { get; }
  ```

- **`CreatureHorde Property`**
  ```csharp
  public Creature CreatureHorde { get; }
  ```

- **`CreatureIdAlliance Property`**
  ```csharp
  public uint CreatureIdAlliance { get; }
  ```

- **`CreatureIdHorde Property`**
  ```csharp
  public uint CreatureIdHorde { get; }
  ```

- **`Flags Property`**
  ```csharp
  public byte Flags { get; }
  ```

- **`FollowerTypeId Property`**
  ```csharp
  public byte FollowerTypeId { get; }
  ```

- **`GarrTypeId Property`**
  ```csharp
  public byte GarrTypeId { get; }
  ```

- **`HordeAddedBroadcastTextId Property`**
  ```csharp
  public uint HordeAddedBroadcastTextId { get; }
  ```

- **`HordeFlavorTextGarrStringId Property`**
  ```csharp
  public byte HordeFlavorTextGarrStringId { get; }
  ```

- **`HordeGarrClassSpecId Property`**
  ```csharp
  public byte HordeGarrClassSpecId { get; }
  ```

- **`HordeGarrFollItemSetId Property`**
  ```csharp
  public ushort HordeGarrFollItemSetId { get; }
  ```

- **`HordeListPortraitTextureKitId Property`**
  ```csharp
  public ushort HordeListPortraitTextureKitId { get; }
  ```

- **`HordePortraitIconId Property`**
  ```csharp
  public uint HordePortraitIconId { get; }
  ```

- **`HordeUiAnimRaceInfoId Property`**
  ```csharp
  public byte HordeUiAnimRaceInfoId { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`ItemLevelArmor Property`**
  ```csharp
  public ushort ItemLevelArmor { get; }
  ```

- **`ItemLevelWeapon Property`**
  ```csharp
  public ushort ItemLevelWeapon { get; }
  ```

- **`Level Property`**
  ```csharp
  public byte Level { get; }
  ```

- **`MaxDurability Property`**
  ```csharp
  public byte MaxDurability { get; }
  ```

- **`Quality Property`**
  ```csharp
  public byte Quality { get; }
  ```

- **`SourceTextAlliance Property`**
  ```csharp
  public string SourceTextAlliance { get; }
  ```

- **`SourceTextHorde Property`**
  ```csharp
  public string SourceTextHorde { get; }
  ```

- **`Type Property`**
  ```csharp
  public GarrisonFollowerType Type { get; }
  ```

- **`Unknown1 Property`**
  ```csharp
  public byte Unknown1 { get; }
  ```

- **`Unknown2 Property`**
  ```csharp
  public byte Unknown2 { get; }
  ```

- **`Unknown3 Property`**
  ```csharp
  public byte Unknown3 { get; }
  ```

#### GarrFollower Methods

- **`FromId Method`**
  ```csharp
  public static GarrFollower FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST2D5600E2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceGarrFollower Class

---

### GarrMechanic Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.GarrMechanic

```csharp
public class GarrMechanic
```

Determines whether the specified object is equal to the current object.

#### GarrMechanic Properties

- **`dword5 Property`**
  ```csharp
  public int dword5 { get; }
  ```

- **`float0 Property`**
  ```csharp
  public float float0 { get; }
  ```

- **`GarrMechanicTypeId Property`**
  ```csharp
  public uint GarrMechanicTypeId { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`MechanicType Property`**
  ```csharp
  public GarrMechanicType MechanicType { get; }
  ```

#### GarrMechanic Methods

- **`FromId Method`**
  ```csharp
  public static GarrMechanic FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTC7DD9572_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceGarrMechanic Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a string that represents the current object.
  - **Returns:** ReferenceGarrMechanic Class

---

### GarrMechanicType Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.GarrMechanicType

```csharp
public class GarrMechanicType
```

Determines whether the specified object is equal to the current object.

#### GarrMechanicType Properties

- **`Description Property`**
  ```csharp
  public string Description { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```

- **`Type Property`**
  ```csharp
  public int Type { get; }
  ```

#### GarrMechanicType Methods

- **`FromId Method`**
  ```csharp
  public static GarrMechanicType FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTD4D5DBE2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceGarrMechanicType Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a string that represents the current object.
  - **Returns:** ReferenceGarrMechanicType Class

---

### GarrMission Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.GarrMission

```csharp
public class GarrMission
```

Determines whether the specified object is equal to the current object.

#### GarrMission Properties

- **`Cooldown Property`**
  ```csharp
  public int Cooldown { get; }
  ```

- **`Cost Property`**
  ```csharp
  public int Cost { get; }
  ```

- **`Description Property`**
  ```csharp
  public string Description { get; }
  ```

- **`DurationInSeconds Property`**
  ```csharp
  public int DurationInSeconds { get; }
  ```

- **`ExperienceGain Property`**
  ```csharp
  public int ExperienceGain { get; }
  ```

- **`Flags Property`**
  ```csharp
  public int Flags { get; }
  ```

- **`FollowerType Property`**
  ```csharp
  public GarrisonFollowerType FollowerType { get; }
  ```

- **`GarrMechanicType Property`**
  ```csharp
  public GarrMechanicType GarrMechanicType { get; }
  ```

- **`GarrMechanicTypeId Property`**
  ```csharp
  public uint GarrMechanicTypeId { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`IsExhausting Property`**
  ```csharp
  public bool IsExhausting { get; }
  ```

- **`IsRare Property`**
  ```csharp
  public bool IsRare { get; }
  ```

- **`ItemLevel Property`**
  ```csharp
  public int ItemLevel { get; }
  ```

- **`Level Property`**
  ```csharp
  public int Level { get; }
  ```

- **`Location Property`**
  ```csharp
  public string Location { get; }
  ```

- **`MapPos Property`**
  ```csharp
  public Vector2 MapPos { get; }
  ```

- **`MissionType Property`**
  ```csharp
  public GarrisonMissionType MissionType { get; }
  ```

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```

- **`NumFollowers Property`**
  ```csharp
  public int NumFollowers { get; }
  ```

- **`PrefixUiTextureKitId Property`**
  ```csharp
  public int PrefixUiTextureKitId { get; }
  ```

#### GarrMission Methods

- **`FromId Method`**
  ```csharp
  public static GarrMission FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST2845D742_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceGarrMission Class

---

### GarrPlotInstance Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.GarrPlotInstance

```csharp
public class GarrPlotInstance
```

Determines whether the specified object is equal to the current object.

#### GarrPlotInstance Properties

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```

- **`PlotType Property`**
  ```csharp
  public PlotType PlotType { get; }
  ```

#### GarrPlotInstance Methods

- **`FromId Method`**
  ```csharp
  public static GarrPlotInstance FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTD4A7950_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceGarrPlotInstance Class

---

### GarrSiteLevel Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.GarrSiteLevel

```csharp
public class GarrSiteLevel
```

Determines whether the specified object is equal to the current object.

#### GarrSiteLevel Properties

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`Level Property`**
  ```csharp
  public byte Level { get; }
  ```

- **`Level2 Property`**
  ```csharp
  public byte Level2 { get; }
  ```

- **`MapId Property`**
  ```csharp
  public short MapId { get; }
  ```

- **`MovieId Property`**
  ```csharp
  public byte MovieId { get; }
  ```

- **`SiteId Property`**
  ```csharp
  public short SiteId { get; }
  ```

- **`TownHallPosition Property`**
  ```csharp
  public Vector2 TownHallPosition { get; }
  ```

- **`UpgradeMoneyCost Property`**
  ```csharp
  public short UpgradeMoneyCost { get; }
  ```

- **`UpgradeResourceCost Property`**
  ```csharp
  public short UpgradeResourceCost { get; }
  ```

#### GarrSiteLevel Methods

- **`FromId Method`**
  ```csharp
  public static GarrSiteLevel FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTA2AF2261_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceGarrSiteLevel Class

#### GarrSiteLevel Fields

- **`UITextureKitId Field`**
  ```csharp
  public byte UITextureKitId
  ```

---

### GarrisonBuildingType Enumeration

```csharp
public enum GarrisonBuildingType
```

---

### GarrisonFollowerType Enumeration

```csharp
public enum GarrisonFollowerType
```

---

### GarrisonMissionType Enumeration

```csharp
public enum GarrisonMissionType
```

---

### ItemDisenchantLoot Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.ItemDisenchantLoot

```csharp
public class ItemDisenchantLoot
```

Gets the iterator.

#### ItemDisenchantLoot Properties

- **`Collection Property`**
  ```csharp
  public static IEnumerable&lt;ItemDisenchantLoot&gt; Collection { get; }
  ```
  Gets the iterator.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Gets the identifier

- **`ItemClass Property`**
  ```csharp
  public WoWItemClass ItemClass { get; }
  ```
  Gets the item class

- **`ItemQuality Property`**
  ```csharp
  public WoWItemQuality ItemQuality { get; }
  ```
  Gets the item level (inclusive) for items belonging in this group

- **`ItemSubClass Property`**
  ```csharp
  public int ItemSubClass { get; }
  ```
  Gets the item subclass

- **`MaxItemLevel Property`**
  ```csharp
  public int MaxItemLevel { get; }
  ```
  Gets the maximum item level (inclusive)

- **`MinItemLevel Property`**
  ```csharp
  public int MinItemLevel { get; }
  ```
  Gets the minimum item level

- **`RequiredEnchantingLevel Property`**
  ```csharp
  public int RequiredEnchantingLevel { get; }
  ```
  Gets the required enchanting level to disenchant items belonging to this group.

#### ItemDisenchantLoot Methods

- **`FromId Method`**
  ```csharp
  public static ItemDisenchantLoot FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTE9779518_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceItemDisenchantLoot Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceItemDisenchantLoot Class

---

### ItemEffect Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.ItemEffect

```csharp
public class ItemEffect
```

Gets the cooldown of the category of this item effect.

#### ItemEffect Properties

- **`CategoryCooldown Property`**
  ```csharp
  public int CategoryCooldown { get; }
  ```
  Gets the cooldown of the category of this item effect.

- **`Charges Property`**
  ```csharp
  public int Charges { get; }
  ```
  Gets the number of charges on this item effect.

- **`Cooldown Property`**
  ```csharp
  public int Cooldown { get; }
  ```
  Gets the cooldown of this item effect in milliseconds.

- **`Spell Property`**
  ```csharp
  public WoWSpell Spell { get; }
  ```
  Gets the spell that this item effect triggers.

- **`SpellCategoryId Property`**
  ```csharp
  public int SpellCategoryId { get; }
  ```
  Gets the ID of the spell category of this item effect.

- **`SpellId Property`**
  ```csharp
  public int SpellId { get; }
  ```
  Gets the ID of the spell that this item effect triggers.

- **`TriggerType Property`**
  ```csharp
  public ItemEffectTriggerType TriggerType { get; }
  ```
  Gets the trigger type of this item effect.

#### ItemEffect Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceItemEffect Class

---

### ItemEffectList Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.ItemEffectList

```csharp
public class ItemEffectList : IReadOnlyList&lt;ItemEffect&gt;,
IReadOnlyCollection&lt;ItemEffect&gt;, IEnumerable&lt;ItemEffect&gt;, IEnumerable
```

Determines whether the specified object is equal to the current object.

#### ItemEffectList Properties

- **`Count Property`**
  ```csharp
  public int Count { get; }
  ```

- **`Item Property`**
  ```csharp
  public ItemEffect this[
int index
] { get; }
  ```
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST80A33292_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32

#### ItemEffectList Methods

- **`FromItemId Method`**
  ```csharp
  public static ItemEffectList FromItemId(
int id
)
  ```
  Gets the list of item effects on an item by the specified ID.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST62B0F4BA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
  - **Returns:** ReferenceItemEffectList Class

- **`FromItemInfo Method`**
  ```csharp
  public static ItemEffectList FromItemInfo(
ItemInfo itemInfo
)
  ```
  Gets the list of item effects on an item by the specified item info.
  - *itemInfo*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST96BC0839_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ItemInfo
  - **Returns:** ExceptionConditionArgumentNullExceptionThrown if itemInfo is null.

- **`GetEnumerator Method`**
  ```csharp
  public IEnumerator&lt;ItemEffect&gt; GetEnumerator()
  ```
  - **Returns:** See Also

---

### ItemEffectTriggerType Enumeration

```csharp
public enum ItemEffectTriggerType
```

---

### ItemEntry Structure

```csharp
public struct ItemEntry
```

An item entry.

#### ItemEntry Fields

- **`ClassId Field`**
  ```csharp
  public byte ClassId
  ```
  Identifier for the class.

- **`DisplayInfoId Field`**
  ```csharp
  public int DisplayInfoId
  ```
  The identifier.

- **`InventoryType Field`**
  ```csharp
  public byte InventoryType
  ```
  Type of the inventory.

- **`MaterialId Field`**
  ```csharp
  public byte MaterialId
  ```
  Identifier for the material.

- **`SheathType Field`**
  ```csharp
  public byte SheathType
  ```
  Type of the sheath.

- **`SubClassId Field`**
  ```csharp
  public byte SubClassId
  ```
  Identifier for the sub class.

---

### ItemExtendedCost Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.ItemExtendedCost

```csharp
public class ItemExtendedCost
```

Determines whether the specified object is equal to the current object.

#### ItemExtendedCost Properties

- **`Flags Property`**
  ```csharp
  public uint Flags { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`RequiredCurrencyCounts Property`**
  ```csharp
  public IReadOnlyList&lt;int&gt; RequiredCurrencyCounts { get; }
  ```

- **`RequiredCurrencyIds Property`**
  ```csharp
  public IReadOnlyList&lt;uint&gt; RequiredCurrencyIds { get; }
  ```

- **`RequiredItemCounts Property`**
  ```csharp
  public IReadOnlyList&lt;int&gt; RequiredItemCounts { get; }
  ```

- **`RequiredItemIds Property`**
  ```csharp
  public IReadOnlyList&lt;uint&gt; RequiredItemIds { get; }
  ```

#### ItemExtendedCost Methods

- **`FromId Method`**
  ```csharp
  public static ItemExtendedCost FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST63319887_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceItemExtendedCost Class

- **`GetRequiredCurrencyAmount Method`**
  Returns the amount of currency of the specified type that this extended cost has.

- **`GetRequiredCurrencyAmount Method (WoWCurrencyType)`**
  ```csharp
  public long GetRequiredCurrencyAmount(
WoWCurrencyType type
)
  ```
  Returns the amount of currency of the specified type that this extended cost has.
  - *type*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST33B043C1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWCurrencyType
  - **Returns:** ReferenceItemExtendedCost Class

- **`GetRequiredCurrencyAmount Method (WoWCurrencyType, Boolean)`**
  ```csharp
  public long GetRequiredCurrencyAmount(
WoWCurrencyType type,
out bool isSeasonTotal
)
  ```
  Returns the amount of currency of the specified type that this extended cost has.
  - *type*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST3FD2852C_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWCurrencyTypeThe type of currency to look for.
  - *isSeasonTotal*: Type: SystemAddLanguageSpecificTextSet("LST3FD2852C_3?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanAddLanguageSpecificTextSet("LST3FD2852C_4?cpp=%");Whether the value returned is by season total, or current amount.
  - **Returns:** ReferenceItemExtendedCost Class

- **`PlayerHasEnoughCurrency Method`**
  ```csharp
  public bool PlayerHasEnoughCurrency()
  ```
  - **Returns:** ReferenceItemExtendedCost Class

---

### ItemSparseEntry Structure

```csharp
public struct ItemSparseEntry
```

An item sparse entry.

#### ItemSparseEntry Fields

- **`AllowedClasses Field`**
  ```csharp
  public short AllowedClasses
  ```

- **`AllowedRaces Field`**
  ```csharp
  public int AllowedRaces
  ```

- **`AreaTableId Field`**
  ```csharp
  public short AreaTableId
  ```

- **`ArmorDamageModifier Field`**
  ```csharp
  public float ArmorDamageModifier
  ```

- **`ArtifactId Field`**
  ```csharp
  public byte ArtifactId
  ```

- **`BagFamilyId Field`**
  ```csharp
  public int BagFamilyId
  ```

- **`BagSlots Field`**
  ```csharp
  public byte BagSlots
  ```

- **`BondingId Field`**
  ```csharp
  public byte BondingId
  ```

- **`BuyCount Field`**
  ```csharp
  public int BuyCount
  ```

- **`BuyPrice Field`**
  ```csharp
  public int BuyPrice
  ```

- **`DamageType Field`**
  ```csharp
  public byte DamageType
  ```

- **`Description Field`**
  ```csharp
  public IntPtr Description
  ```

- **`Duration Field`**
  ```csharp
  public int Duration
  ```

- **`EquipSlot Field`**
  ```csharp
  public byte EquipSlot
  ```

- **`Flags Field`**
  ```csharp
  public int Flags
  ```

- **`Flags2 Field`**
  ```csharp
  public int Flags2
  ```

- **`Flags3 Field`**
  ```csharp
  public int Flags3
  ```

- **`Flags4 Field`**
  ```csharp
  public int Flags4
  ```

- **`GemPropertiesId Field`**
  ```csharp
  public short GemPropertiesId
  ```

- **`HolidayId Field`**
  ```csharp
  public short HolidayId
  ```

- **`ItemLevel Field`**
  ```csharp
  public short ItemLevel
  ```

- **`ItemLimitCategoryId Field`**
  ```csharp
  public short ItemLimitCategoryId
  ```

- **`ItemSetId Field`**
  ```csharp
  public short ItemSetId
  ```

- **`LockId Field`**
  ```csharp
  public short LockId
  ```

- **`MapId Field`**
  ```csharp
  public short MapId
  ```

- **`MaxStackSize Field`**
  ```csharp
  public int MaxStackSize
  ```

- **`PageTextId Field`**
  ```csharp
  public short PageTextId
  ```

- **`QuestId Field`**
  ```csharp
  public ushort QuestId
  ```

- **`RandomSuffixId Field`**
  ```csharp
  public short RandomSuffixId
  ```

- **`Rarity Field`**
  ```csharp
  public byte Rarity
  ```

- **`RequiredCityRank Field`**
  ```csharp
  public byte RequiredCityRank
  ```

- **`RequiredHonorRank Field`**
  ```csharp
  public byte RequiredHonorRank
  ```

- **`RequiredLevel Field`**
  ```csharp
  public byte RequiredLevel
  ```

- **`RequiredReputationFaction Field`**
  ```csharp
  public short RequiredReputationFaction
  ```

- **`RequiredReputationRank Field`**
  ```csharp
  public byte RequiredReputationRank
  ```

- **`RequiredSkill Field`**
  ```csharp
  public short RequiredSkill
  ```

- **`RequiredSkillLevel Field`**
  ```csharp
  public short RequiredSkillLevel
  ```

- **`RequiredSpell Field`**
  ```csharp
  public int RequiredSpell
  ```

- **`ScalingStatDistributionId Field`**
  ```csharp
  public short ScalingStatDistributionId
  ```

- **`SellPrice Field`**
  ```csharp
  public int SellPrice
  ```

- **`SocketColor Field`**
  ```csharp
  public fixed byte SocketColor[3]
  ```

- **`Speed Field`**
  ```csharp
  public short Speed
  ```

- **`SpellItemEnchantmentsId Field`**
  ```csharp
  public short SpellItemEnchantmentsId
  ```

- **`StatsScalingFactor Field`**
  ```csharp
  public float StatsScalingFactor
  ```

- **`TotemCategoryId Field`**
  ```csharp
  public ushort TotemCategoryId
  ```

- **`UniqueCount Field`**
  ```csharp
  public int UniqueCount
  ```

- **`wordD2 Field`**
  ```csharp
  public short wordD2
  ```

---

### MissileTargeting Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.MissileTargeting

```csharp
public class MissileTargeting
```

Gets the arc repeat for the missile target.

#### MissileTargeting Properties

- **`MissileTargetArcRepeat Property`**
  ```csharp
  public float MissileTargetArcRepeat { get; }
  ```
  Gets the arc repeat for the missile target.

- **`MissileTargetArcSpeed Property`**
  ```csharp
  public float MissileTargetArcSpeed { get; }
  ```
  Gets the arc speed for the missile target.

- **`MissileTargetArcTexture Property`**
  ```csharp
  public IntPtr MissileTargetArcTexture { get; }
  ```
  Gets the pointer for the arc texture.

- **`MissileTargetArcWidth Property`**
  ```csharp
  public float MissileTargetArcWidth { get; }
  ```
  Gets the arc width for the missile target.

- **`MissileTargetEndOpacity Property`**
  ```csharp
  public float MissileTargetEndOpacity { get; }
  ```
  Gets the end opactiy for the missile target.

- **`MissileTargetImpactModel1 Property`**
  ```csharp
  public IntPtr MissileTargetImpactModel1 { get; }
  ```
  Gets the pointer for the impact model 1.

- **`MissileTargetImpactModel2 Property`**
  ```csharp
  public IntPtr MissileTargetImpactModel2 { get; }
  ```
  Gets the pointer for the impact model 2.

- **`MissileTargetImpactRadius1 Property`**
  ```csharp
  public float MissileTargetImpactRadius1 { get; }
  ```
  Gets the impact radius 1 for the missile target.

- **`MissileTargetImpactRadius2 Property`**
  ```csharp
  public float MissileTargetImpactRadius2 { get; }
  ```
  Gets the impact radius 2 for the missile target.

- **`MissileTargetImpactTexRadius Property`**
  ```csharp
  public float MissileTargetImpactTexRadius { get; }
  ```
  Gets the impact tex radius for the missile target.

- **`MissileTargetImpactTexture Property`**
  ```csharp
  public IntPtr MissileTargetImpactTexture { get; }
  ```
  Gets the pointer for the impact texture.

- **`MissileTargetMouseLingering Property`**
  ```csharp
  public float MissileTargetMouseLingering { get; }
  ```
  Gets the mouse lingering for the missile target.

- **`MissileTargetPitchLingering Property`**
  ```csharp
  public float MissileTargetPitchLingering { get; }
  ```
  Gets the pitch lingering for the missile target.

- **`MissileTargetTurnLingering Property`**
  ```csharp
  public float MissileTargetTurnLingering { get; }
  ```
  Gets the turn lingering for the missile target.

#### MissileTargeting Methods

- **`FromId Method`**
  ```csharp
  public static MissileTargeting FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTA8FFF73_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceMissileTargeting Class

---

### PetType Enumeration

```csharp
public enum PetType
```

---

### PlotType Enumeration

```csharp
public enum PlotType
```

---

### Scenario Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.Scenario

```csharp
public class Scenario
```

Determines whether the specified object is equal to the current object.

#### Scenario Properties

- **`AreaId Property`**
  ```csharp
  public uint AreaId { get; }
  ```

- **`Collection Property`**
  ```csharp
  public static IEnumerable&lt;Scenario&gt; Collection { get; }
  ```

- **`Flags Property`**
  ```csharp
  public int Flags { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```

- **`Type Property`**
  ```csharp
  public ScenarioType Type { get; }
  ```

#### Scenario Methods

- **`FromId Method`**
  ```csharp
  public static Scenario FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST845634CA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceScenario Class

---

### ScenarioStep Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.ScenarioStep

```csharp
public class ScenarioStep
```

Determines whether the specified object is equal to the current object.

#### ScenarioStep Properties

- **`CriteriaTreeId Property`**
  ```csharp
  public uint CriteriaTreeId { get; }
  ```

- **`Description Property`**
  ```csharp
  public string Description { get; }
  ```

- **`Flags Property`**
  ```csharp
  public uint Flags { get; }
  ```

- **`OrderIndex Property`**
  ```csharp
  public int OrderIndex { get; }
  ```

- **`RelatedStepId Property`**
  ```csharp
  public uint RelatedStepId { get; }
  ```

- **`RewardQuestId Property`**
  ```csharp
  public uint RewardQuestId { get; }
  ```

- **`ScenarioId Property`**
  ```csharp
  public uint ScenarioId { get; }
  ```

- **`SupersedesId Property`**
  ```csharp
  public uint SupersedesId { get; }
  ```

- **`Title Property`**
  ```csharp
  public string Title { get; }
  ```

#### ScenarioStep Methods

- **`FromId Method`**
  ```csharp
  public static ScenarioStep FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST9CC2C114_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceScenarioStep Class

- **`FromParentId Method`**
  ```csharp
  public static IEnumerable&lt;ScenarioStep&gt; FromParentId(
uint parentId
)
  ```
  Gets objects by parent Id.
  - *parentId*: Type: SystemAddLanguageSpecificTextSet("LST983359A0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** See Also

---

### ScenarioType Enumeration

```csharp
public enum ScenarioType
```

---

### SpellMissile Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.SpellMissile

```csharp
public class SpellMissile
```

Gets the collision radius.

#### SpellMissile Properties

- **`CollisionRadius Property`**
  ```csharp
  public float CollisionRadius { get; }
  ```
  Gets the collision radius.

- **`DefaultPitchMax Property`**
  ```csharp
  public float DefaultPitchMax { get; }
  ```
  Gets the default maximum pitch.

- **`DefaultPitchMin Property`**
  ```csharp
  public float DefaultPitchMin { get; }
  ```
  Gets the default minimum pitch.

- **`DefaultSpeedMax Property`**
  ```csharp
  public float DefaultSpeedMax { get; }
  ```
  Gets the default maximum speed.

- **`DefaultSpeedMin Property`**
  ```csharp
  public float DefaultSpeedMin { get; }
  ```
  Gets the default minimum speed.

- **`Flags Property`**
  ```csharp
  public uint Flags { get; }
  ```
  Gets the flags.

- **`Gravity Property`**
  ```csharp
  public float Gravity { get; }
  ```
  Gets the gravity.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Gets the identifier.

- **`MaxDuration Property`**
  ```csharp
  public float MaxDuration { get; }
  ```
  Gets the maximum duration.

- **`RandomizeFacingMax Property`**
  ```csharp
  public float RandomizeFacingMax { get; }
  ```
  Gets the randomize maximum facing.

- **`RandomizeFacingMin Property`**
  ```csharp
  public float RandomizeFacingMin { get; }
  ```
  Gets the randomize minimum facing.

- **`RandomizePitchMax Property`**
  ```csharp
  public float RandomizePitchMax { get; }
  ```
  Gets the randomize maximum pitch.

- **`RandomizePitchMin Property`**
  ```csharp
  public float RandomizePitchMin { get; }
  ```
  Gets the randomize minimum pitch.

- **`RandomizeSpeedMax Property`**
  ```csharp
  public float RandomizeSpeedMax { get; }
  ```
  Gets the randomize maximum speed.

- **`RandomizeSpeedMin Property`**
  ```csharp
  public float RandomizeSpeedMin { get; }
  ```
  Gets the randomize minimum speed.

#### SpellMissile Methods

- **`FromId Method`**
  ```csharp
  public static SpellMissile FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTE24B30CA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceSpellMissile Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents this instance.
  - **Returns:** ReferenceSpellMissile Class

---

### TaxiNode Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.TaxiNode

```csharp
public class TaxiNode
```

A taxi nodes.

#### TaxiNode Properties

- **`AllianceMountCreatureId Property`**
  ```csharp
  public int AllianceMountCreatureId { get; }
  ```
  The alliance mount ID. This is only valid for actual flight paths.

- **`AllianceOnly Property`**
  ```csharp
  public bool AllianceOnly { get; }
  ```
  Whether this taxi node is alliance only.

- **`FlightPath Property`**
  ```csharp
  public bool FlightPath { get; }
  ```
  Whether this taxi node is actually a flight path, or a boat/zep or elevator.

- **`HordeMountCreatureId Property`**
  ```csharp
  public int HordeMountCreatureId { get; }
  ```
  The horde mount ID. This is only valid for actual flight paths.

- **`HordeOnly Property`**
  ```csharp
  public bool HordeOnly { get; }
  ```
  Whether this taxi node is horde only.

- **`Id Property`**
  ```csharp
  public int Id { get; }
  ```
  The ID of this taxi node.

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; }
  ```
  The physical location in the world where this taxi node exists.

- **`MapId Property`**
  ```csharp
  public int MapId { get; }
  ```
  The map ID that this taxi node is located on.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  The name of the taxi node.

- **`UITextureKitId Property`**
  ```csharp
  public int UITextureKitId { get; }
  ```

#### TaxiNode Methods

- **`FromId Method`**
  ```csharp
  public static TaxiNode FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTEA6C6824_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The ID of this taxi node.
  - **Returns:** ReferenceTaxiNode Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceTaxiNode Class

---

### TotemCategory Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.TotemCategory

```csharp
public class TotemCategory
```

Gets the identifier.

#### TotemCategory Properties

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Gets the identifier.

- **`Mask Property`**
  ```csharp
  public uint Mask { get; }
  ```
  Gets the mask.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`Type Property`**
  ```csharp
  public int Type { get; }
  ```
  Gets the type.

#### TotemCategory Methods

- **`FromId Method`**
  ```csharp
  public static TotemCategory FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST70BF604D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceTotemCategory Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceTotemCategory Class

---

### UILocomotionType Enumeration

```csharp
public enum UILocomotionType
```

---

### Vehicle Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.Vehicle

```csharp
public class Vehicle
```

Gets the camera fade distance scalar max.

#### Vehicle Properties

- **`CameraFadeDistanceScalarMax Property`**
  ```csharp
  public float CameraFadeDistanceScalarMax { get; }
  ```
  Gets the camera fade distance scalar max.

- **`CameraFadeDistanceScalarMin Property`**
  ```csharp
  public float CameraFadeDistanceScalarMin { get; }
  ```
  Gets the camera fade distance scalar min.

- **`CameraPitchOffset Property`**
  ```csharp
  public float CameraPitchOffset { get; }
  ```
  Gets the camera pitch offset.

- **`CameraYawOffset Property`**
  ```csharp
  public float CameraYawOffset { get; }
  ```
  Gets the camera yaw offset.

- **`FacingLimitLeft Property`**
  ```csharp
  public float FacingLimitLeft { get; }
  ```
  Gets the left face limit.

- **`FacingLimitRight Property`**
  ```csharp
  public float FacingLimitRight { get; }
  ```
  Gets the right face limit.

- **`Flags Property`**
  ```csharp
  public VehicleFlags Flags { get; }
  ```
  Gets the flags.

- **`FlagsB Property`**
  ```csharp
  public int FlagsB { get; }
  ```
  Gets the flags b.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Gets the identifier.

- **`MissileTargeting Property`**
  ```csharp
  public MissileTargeting MissileTargeting { get; }
  ```

- **`MissileTargetingId Property`**
  ```csharp
  public uint MissileTargetingId { get; }
  ```

- **`MouseLookOffsetPitch Property`**
  ```csharp
  public float MouseLookOffsetPitch { get; }
  ```
  Gets the mouse look offset pitch.

- **`PitchMax Property`**
  ```csharp
  public float PitchMax { get; }
  ```
  Gets the pitch max.

- **`PitchMin Property`**
  ```csharp
  public float PitchMin { get; }
  ```
  Gets the pitch min.

- **`PitchSpeed Property`**
  ```csharp
  public float PitchSpeed { get; }
  ```
  Gets the pitch speed.

- **`PowerDisplayIdentifiers Property`**
  ```csharp
  public ushort[] PowerDisplayIdentifiers { get; }
  ```
  Gets the power display identifiers.

- **`SeatIdentifiers Property`**
  ```csharp
  public ushort[] SeatIdentifiers { get; }
  ```
  Gets the seat identifiers.

- **`TurnSpeed Property`**
  ```csharp
  public float TurnSpeed { get; }
  ```
  Gets the turn speed.

- **`UILocomotionType Property`**
  ```csharp
  public UILocomotionType UILocomotionType { get; }
  ```
  Gets the UI locomotion type.

- **`VehicleUIIndicatorID Property`**
  ```csharp
  public uint VehicleUIIndicatorID { get; }
  ```
  Gets the vehicle UI indicator identifier.

#### Vehicle Methods

- **`FromId Method`**
  ```csharp
  public static Vehicle FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST4EAFCEA8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceVehicle Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceVehicle Class

---

### VehicleFlags Enumeration

```csharp
[FlagsAttribute]
public enum VehicleFlags
```

---

### WoWDb Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.WoWDb

```csharp
public class WoWDb
```

Determines whether the specified object is equal to the current object.

#### WoWDb Properties

- **`Item Property`**
  ```csharp
  public WoWDbTable this[
ClientDb db
] { get; }
  ```
  - *db*: Type: ClientDb

---

### WoWDbRow Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.WoWDbRow

```csharp
public class WoWDbRow
```

Returns whether or not this Row is valid.

#### WoWDbRow Properties

- **`Address Property`**
  ```csharp
  public IntPtr Address { get; }
  ```

- **`IsValid Property`**
  ```csharp
  [ObsoleteAttribute("Always returns false")]
public bool IsValid { get; }
  ```
  Returns whether or not this Row is valid.

#### WoWDbRow Methods

- **`GetField(T) Method`**
  ```csharp
  public T GetField&lt;T&gt;(
uint index
)
where T : struct, new()
  ```
  Returns a field of a type you specify.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTC323BEAF_3?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32.
  - *T*: .
  - **Returns:** See Also

- **`GetStringField Method`**
  Gets string field.

- **`GetStringField(T) Method (String)`**
  ```csharp
  public string GetStringField&lt;T&gt;(
string fieldName
)
  ```
  Gets string field.
  - *fieldName*: Type: SystemAddLanguageSpecificTextSet("LST6E83FD44_3?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the field.
  - *T*: Generic type parameter.
  - **Returns:** ReferenceWoWDbRow Class

- **`GetStringField Method (UInt32)`**
  ```csharp
  public string GetStringField(
uint index
)
  ```
  Gets string field.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST31382E8B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32.
  - **Returns:** ReferenceWoWDbRow Class

- **`GetStruct(T) Method`**
  ```csharp
  public T GetStruct&lt;T&gt;()
where T : struct, new()
  ```
  Returns a struct representation of this row.
  - *T*: .
  - **Returns:** See Also

---

### WoWDbTable Class

**Inheritance:** System.Object → Styx.WoWInternals.DB.WoWDbTable → Styx.WoWInternals.DB.Db2Table

```csharp
public abstract class WoWDbTable : IEnumerable&lt;WoWDbRow&gt;,
IEnumerable
```

Gets the base address of this table.

#### WoWDbTable Properties

- **`BaseAddress Property`**
  ```csharp
  public IntPtr BaseAddress { get; }
  ```
  Gets the base address of this table.

- **`Identifier Property`**
  ```csharp
  public ClientDb Identifier { get; }
  ```
  Gets an identifier that uniquely identifies which table this value represents in WoW.

- **`MaxId Property`**
  ```csharp
  public abstract int MaxId { get; }
  ```
  Gets the maximum ID that this table contains.

- **`MinId Property`**
  ```csharp
  public abstract int MinId { get; }
  ```
  Gets the minimum ID that this table contains.

- **`NumRows Property`**
  ```csharp
  public abstract int NumRows { get; }
  ```
  Gets the number of rows contains in this table.

- **`RowSize Property`**
  ```csharp
  public abstract int RowSize { get; }
  ```
  Gets the byte size of a single row in this table.

#### WoWDbTable Methods

- **`EnumerateIdRowPairs Method`**
  ```csharp
  public abstract IEnumerable&lt;KeyValuePair&lt;int, WoWDbRow&gt;&gt; EnumerateIdRowPairs()
  ```
  Enumerates the pairs of IDs and rows.
  - **Returns:** See Also

- **`GetEnumerator Method`**
  ```csharp
  public abstract IEnumerator&lt;WoWDbRow&gt; GetEnumerator()
  ```
  Returns an enumerator that iterates all rows in this table.
  - **Returns:** See Also

- **`GetRow Method`**
  ```csharp
  public abstract WoWDbRow GetRow(
uint id
)
  ```
  Returns a WoWDbRow representing the record with the specified id.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST483BFE38_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The ID of the row.
  - **Returns:** See Also

---
