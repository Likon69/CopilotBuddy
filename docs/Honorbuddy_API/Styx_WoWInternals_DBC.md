# Styx.WoWInternals.DBC

Contains WoW DBC related classes.

## Contents

- [AreaPoi Class](#areapoi-class)
- [AreaPoi.IconFlags Enumeration](#areapoi.iconflags-enumeration)
- [AreaTable Class](#areatable-class)
- [CreatureFamily Class](#creaturefamily-class)
- [Faction Class](#faction-class)
- [Faction.FactionDbcRecord Structure](#faction.factiondbcrecord-structure)
- [FactionTemplate Class](#factiontemplate-class)
- [FactionTemplate.FactionTemplateDbcRecord Structure](#factiontemplate.factiontemplatedbcrecord-structure)
- [InstanceType Enumeration](#instancetype-enumeration)
- [ItemRandomProperties Class](#itemrandomproperties-class)
- [ItemRandomSuffix Class](#itemrandomsuffix-class)
- [ItemRandomSuffix.ItemRandomSuffixEntry Structure](#itemrandomsuffix.itemrandomsuffixentry-structure)
- [LfgDifficulty Enumeration](#lfgdifficulty-enumeration)
- [LfgDungeons Class](#lfgdungeons-class)
- [LfgDungeonsFlags Enumeration](#lfgdungeonsflags-enumeration)
- [LfgSubType Enumeration](#lfgsubtype-enumeration)
- [Map Class](#map-class)
- [MapDifficulty Class](#mapdifficulty-class)
- [MapType Enumeration](#maptype-enumeration)
- [PetFoodFlags Enumeration](#petfoodflags-enumeration)
- [RecipeAcquireMethod Enumeration](#recipeacquiremethod-enumeration)
- [ResearchSite Class](#researchsite-class)
- [ScalingStatDistribution Class](#scalingstatdistribution-class)
- [SkillLineAbility Class](#skilllineability-class)
- [SkillLineCategory Enumeration](#skilllinecategory-enumeration)
- [SkillLineInfo Class](#skilllineinfo-class)
- [SpellEffect Class](#spelleffect-class)
- [SpellItemEnchantment Class](#spellitemenchantment-class)

---

### AreaPoi Class

**Inheritance:** System.Object → Styx.WoWInternals.DBC.AreaPoi

```csharp
public class AreaPoi
```

An area poi.

#### AreaPoi Properties

- **`AreaID Property`**
  ```csharp
  public int AreaID { get; }
  ```
  Gets the identifier of the area.

- **`Description Property`**
  ```csharp
  public string Description { get; }
  ```
  Gets the description.

- **`GraveyardID Property`**
  ```csharp
  public int GraveyardID { get; }
  ```

- **`ID Property`**
  ```csharp
  public uint ID { get; }
  ```
  Gets the identifier.

- **`Location Property`**
  ```csharp
  public Vector2 Location { get; }
  ```
  Gets the location.

- **`MapID Property`**
  ```csharp
  public int MapID { get; }
  ```
  Gets the identifier of the map.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`NormalIcon Property`**
  ```csharp
  public int NormalIcon { get; }
  ```
  Gets the normal icon.

- **`PlayerConditionID Property`**
  ```csharp
  public int PlayerConditionID { get; }
  ```
  Gets the player condition identifier; MatchesCondition(UInt32).

- **`ShowInBattleMap Property`**
  ```csharp
  public bool ShowInBattleMap { get; }
  ```
  Returns true if this landmark is shown in the BattleMap.

- **`ShowInBg Property`**
  ```csharp
  public bool ShowInBg { get; }
  ```
  Returnsn true if this landmark is show in BG.

- **`ShowInZone Property`**
  ```csharp
  public bool ShowInZone { get; }
  ```
  Returns true if this landmark is show in Zone.

- **`WorldStateID Property`**
  ```csharp
  public int WorldStateID { get; }
  ```
  Gets the identifier of the world state.

#### AreaPoi Methods

- **`FromId Method`**
  ```csharp
  public static AreaPoi FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTA5C2AB56_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceAreaPoi Class

---

### AreaPoi.IconFlags Enumeration

```csharp
[FlagsAttribute]
public enum IconFlags
```

Bitfield of flags for specifying IconFlags.

---

### AreaTable Class

**Inheritance:** System.Object → Styx.WoWInternals.DBC.AreaTable

```csharp
public class AreaTable
```

Represents a area within wow.

#### AreaTable Properties

- **`AreaId Property`**
  ```csharp
  public uint AreaId { get; }
  ```
  The area id.

- **`AreaName Property`**
  ```csharp
  public string AreaName { get; }
  ```
  The name of the area.

- **`InternalAreaName Property`**
  ```csharp
  public string InternalAreaName { get; }
  ```
  Gets the internal area name of the area.

- **`MapId Property`**
  ```csharp
  public uint MapId { get; }
  ```
  The map id.

- **`ParentArea Property`**
  ```csharp
  public AreaTable ParentArea { get; }
  ```
  Gets the parent area.

- **`ParentAreaId Property`**
  ```csharp
  public uint ParentAreaId { get; }
  ```
  The map id.

- **`ParentMap Property`**
  ```csharp
  public Map ParentMap { get; }
  ```
  The parent map of this area.

#### AreaTable Methods

- **`FromId Method`**
  ```csharp
  public static AreaTable FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST1BD247BA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceAreaTable Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceAreaTable Class

---

### CreatureFamily Class

**Inheritance:** System.Object → Styx.WoWInternals.DBC.CreatureFamily

```csharp
public class CreatureFamily
```

Represents a creature family record.

#### CreatureFamily Properties

- **`Diet Property`**
  ```csharp
  public PetFoodFlags Diet { get; }
  ```
  The diet for this CreatureFamily, 'Meat', 'Fish', 'Fungus' etc.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  The id of this CreatureFamily.

- **`MaxScale Property`**
  ```csharp
  public float MaxScale { get; }
  ```
  The maximum scale for this CreatureFamily.

- **`MaxScaleLevel Property`**
  ```csharp
  public int MaxScaleLevel { get; }
  ```
  The maximum level for this scale for this CreatureFamily.

- **`MinScale Property`**
  ```csharp
  public float MinScale { get; }
  ```
  The minimum scale for this CreatureFamily.

- **`MinScaleLevel Property`**
  ```csharp
  public int MinScaleLevel { get; }
  ```
  The minimum level for this scale for this CreatureFamily.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  The name of this CreatureFamily.

- **`PetTalentType Property`**
  ```csharp
  public int PetTalentType { get; }
  ```
  The PetTalentType for this CreatureFamily.

- **`SkillLine Property`**
  ```csharp
  public SkillLine[] SkillLine { get; }
  ```
  Returns skills associated with this CreatureFamily.

#### CreatureFamily Methods

- **`FromId Method`**
  ```csharp
  public static CreatureFamily FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST4911EC9E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The id of this CreatureFamily.
  - **Returns:** ReferenceCreatureFamily Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceCreatureFamily Class

---

### Faction Class

**Inheritance:** System.Object → Styx.WoWInternals.DBC.Faction

```csharp
public class Faction
```

A faction.

#### Faction Properties

- **`Description Property`**
  ```csharp
  public string Description { get; }
  ```
  Gets the description.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Gets the identifier.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`ParentFaction Property`**
  ```csharp
  public Faction ParentFaction { get; }
  ```
  Gets the parent faction.

- **`Record Property`**
  ```csharp
  public Faction.FactionDbcRecord Record { get; }
  ```
  Gets the record.

#### Faction Methods

- **`FromId Method`**
  ```csharp
  public static Faction FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTE8EE64F1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceFaction Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceFaction Class

---

### Faction.FactionDbcRecord Structure

```csharp
public struct FactionDbcRecord
```

Information about the faction dbc.

#### FactionDbcRecord Fields

- **`Description Field`**
  ```csharp
  public IntPtr Description
  ```

- **`FriendshipReputationId Field`**
  ```csharp
  public byte FriendshipReputationId
  ```

- **`Name Field`**
  ```csharp
  public IntPtr Name
  ```

- **`ParentFactionId Field`**
  ```csharp
  public ushort ParentFactionId
  ```

- **`RepGainId Field`**
  ```csharp
  public ushort RepGainId
  ```

---

### FactionTemplate Class

**Inheritance:** System.Object → Styx.WoWInternals.DBC.FactionTemplate

```csharp
public class FactionTemplate
```

A faction template.

#### FactionTemplate Properties

- **`Faction Property`**
  ```csharp
  public Faction Faction { get; }
  ```
  Gets the faction.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Gets the identifier.

- **`Record Property`**
  ```csharp
  public FactionTemplate.FactionTemplateDbcRecord Record { get; }
  ```
  Gets the record.

#### FactionTemplate Methods

- **`FromId Method`**
  ```csharp
  public static FactionTemplate FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST98A9845D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceFactionTemplate Class

- **`GetReactionTowards Method`**
  Gets reaction towards.

- **`GetReactionTowards Method (FactionTemplate)`**
  ```csharp
  public WoWUnitReaction GetReactionTowards(
FactionTemplate otherFaction
)
  ```
  Gets reaction towards.
  - *otherFaction*: Type: Styx.WoWInternals.DBCAddLanguageSpecificTextSet("LSTC9462CEF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");FactionTemplateThe other faction.
  - **Returns:** ReferenceFactionTemplate Class

- **`GetReactionTowards Method (WoWUnit)`**
  ```csharp
  public WoWUnitReaction GetReactionTowards(
WoWUnit unit
)
  ```
  Gets reaction towards.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST6D1841D0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe unit.
  - **Returns:** ReferenceFactionTemplate Class

---

### FactionTemplate.FactionTemplateDbcRecord Structure

```csharp
public struct FactionTemplateDbcRecord
```

Information about the faction template dbc.

#### FactionTemplateDbcRecord Fields

- **`EnemyFactions Field`**
  ```csharp
  public ushort[] EnemyFactions
  ```

- **`FactionFlags Field`**
  ```csharp
  public ushort FactionFlags
  ```

- **`FactionGroupId Field`**
  ```csharp
  public byte FactionGroupId
  ```

- **`FactionId Field`**
  ```csharp
  public ushort FactionId
  ```

- **`FriendlyFactions Field`**
  ```csharp
  public ushort[] FriendlyFactions
  ```

- **`FriendlyMask Field`**
  ```csharp
  public byte FriendlyMask
  ```

- **`HostileMask Field`**
  ```csharp
  public byte HostileMask
  ```

---

### InstanceType Enumeration

```csharp
public enum InstanceType
```

Values that represent InstanceType.

---

### ItemRandomProperties Class

**Inheritance:** System.Object → Styx.WoWInternals.DBC.ItemRandomProperties

```csharp
public class ItemRandomProperties
```

An item random properties.

#### ItemRandomProperties Properties

- **`Enchantments Property`**
  ```csharp
  public List&lt;SpellItemEnchantment&gt; Enchantments { get; }
  ```
  Gets the enchantments.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Gets the identifier.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

#### ItemRandomProperties Methods

- **`FromId Method`**
  ```csharp
  public static ItemRandomProperties FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST742D74E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceItemRandomProperties Class

---

### ItemRandomSuffix Class

**Inheritance:** System.Object → Styx.WoWInternals.DBC.ItemRandomSuffix

```csharp
public class ItemRandomSuffix
```

Returns a list of enchantments this suffix adds to the item

#### ItemRandomSuffix Properties

- **`AllocationPercentages Property`**
  ```csharp
  public ushort[] AllocationPercentages { get; }
  ```

- **`Enchantments Property`**
  ```csharp
  public List&lt;SpellItemEnchantment&gt; Enchantments { get; }
  ```
  Returns a list of enchantments this suffix adds to the item

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Returns the name of this suffix, 'of the owl', 'of stamina' etc.

#### ItemRandomSuffix Methods

- **`FromId Method`**
  ```csharp
  public static ItemRandomSuffix FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST48B6B122_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceItemRandomSuffix Class

---

### ItemRandomSuffix.ItemRandomSuffixEntry Structure

```csharp
public struct ItemRandomSuffixEntry
```

Indicates whether this instance and a specified object are equal.

#### ItemRandomSuffixEntry Fields

- **`AllocationPct Field`**
  ```csharp
  public readonly ushort[] AllocationPct
  ```

- **`EnchantId Field`**
  ```csharp
  public readonly ushort[] EnchantId
  ```

- **`Name Field`**
  ```csharp
  public IntPtr Name
  ```

---

### LfgDifficulty Enumeration

```csharp
public enum LfgDifficulty
```

---

### LfgDungeons Class

**Inheritance:** System.Object → Styx.WoWInternals.DBC.LfgDungeons

```csharp
public class LfgDungeons : IEquatable&lt;LfgDungeons&gt;
```

Respresent a dungeon.

#### LfgDungeons Properties

- **`AllianceOnly Property`**
  ```csharp
  public bool AllianceOnly { get; }
  ```

- **`Collection Property`**
  ```csharp
  public static IEnumerable&lt;LfgDungeons&gt; Collection { get; }
  ```
  Gets the collection.

- **`Description Property`**
  ```csharp
  public string Description { get; }
  ```
  The description of this dungeon.

- **`Difficulty Property`**
  ```csharp
  public LfgDifficulty Difficulty { get; }
  ```
  The difficulty - 0 for normal 1 for heroic.

- **`Dungeons Property`**
  ```csharp
  public static IEnumerable&lt;LfgDungeons&gt; Dungeons { get; }
  ```
  Gets the dungeons.

- **`ExpansionId Property`**
  ```csharp
  public uint ExpansionId { get; }
  ```
  The expansion id of this dungeon.

- **`FlexRaids Property`**
  ```csharp
  public static IEnumerable&lt;LfgDungeons&gt; FlexRaids { get; }
  ```
  Gets the flex raids.

- **`ForceHide Property`**
  ```csharp
  public bool ForceHide { get; }
  ```
  Returns true if lfg is not shown in LFG list. This is used for solo scenarios

- **`GroupId Property`**
  ```csharp
  public uint GroupId { get; }
  ```
  The group id of this dungeon.

- **`HasQueue Property`**
  ```csharp
  public bool HasQueue { get; }
  ```
  Gets a value indicating whether this dungeon can be queued for.

- **`HeroicRaids Property`**
  ```csharp
  public static IEnumerable&lt;LfgDungeons&gt; HeroicRaids { get; }
  ```
  Gets the heroic raids.

- **`HordeOnly Property`**
  ```csharp
  public bool HordeOnly { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  The id of this dungeon.

- **`IsHolidayEvent Property`**
  ```csharp
  public bool IsHolidayEvent { get; }
  ```
  Returns a value indicating if this is a holiday event.

- **`IsTimewalker Property`**
  ```csharp
  public bool IsTimewalker { get; }
  ```
  Returns a value indicating if this is a timewalker dungeon.

- **`IsUserTeleportAllowed Property`**
  ```csharp
  public bool IsUserTeleportAllowed { get; }
  ```
  Returns true if lfg allows players to teleport outside

- **`LfRaids Property`**
  ```csharp
  public static IEnumerable&lt;LfgDungeons&gt; LfRaids { get; }
  ```
  Gets the lfraids.

- **`Map Property`**
  ```csharp
  public Map Map { get; }
  ```
  The Map object for this dungeon.

- **`MapId Property`**
  ```csharp
  public int MapId { get; }
  ```
  The mapid of this dungeon.

- **`MaxDamageCount Property`**
  ```csharp
  public int MaxDamageCount { get; }
  ```
  Gets the maximum number of damage dealers that can join dungeon.

- **`MaxHealerCount Property`**
  ```csharp
  public int MaxHealerCount { get; }
  ```
  Gets the maximum number of healers that can join dungeon.

- **`MaxLevel Property`**
  ```csharp
  public uint MaxLevel { get; }
  ```
  The highest level you can enter this dungeon (using the portal, not LFD)

- **`MaxPlayerCount Property`**
  ```csharp
  public int MaxPlayerCount { get; }
  ```

- **`MaxTankCount Property`**
  ```csharp
  public int MaxTankCount { get; }
  ```
  Gets the maximum number of tanks that can join dungeon.

- **`MaxTargetLevel Property`**
  ```csharp
  public int MaxTargetLevel { get; }
  ```

- **`MentorCharacterLevel Property`**
  ```csharp
  public int MentorCharacterLevel { get; }
  ```

- **`MinDamageCount Property`**
  ```csharp
  public int MinDamageCount { get; }
  ```
  Gets the minimum number of damage dealers required .

- **`MinHealerCount Property`**
  ```csharp
  public int MinHealerCount { get; }
  ```
  Gets the minimum number of healers required .

- **`MinLevel Property`**
  ```csharp
  public uint MinLevel { get; }
  ```
  The earliest level you can enter this dungeon (using the portal, not LFD).

- **`MinPlayerCount Property`**
  ```csharp
  public int MinPlayerCount { get; }
  ```

- **`MinTankCount Property`**
  ```csharp
  public int MinTankCount { get; }
  ```
  Gets the minimum number of tanks required .

- **`MinTargetLevel Property`**
  ```csharp
  public int MinTargetLevel { get; }
  ```

- **`MysticRaids Property`**
  ```csharp
  public static IEnumerable&lt;LfgDungeons&gt; MysticRaids { get; }
  ```
  Gets the mystic raids.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  The name of this dungeon.

- **`Neutral Property`**
  ```csharp
  public bool Neutral { get; }
  ```
  Returns true if dungeon is available to all factions

- **`NormalRaids Property`**
  ```csharp
  public static IEnumerable&lt;LfgDungeons&gt; NormalRaids { get; }
  ```
  Gets the normal raids.

- **`OrderIndex Property`**
  ```csharp
  public uint OrderIndex { get; }
  ```
  The order index of this dungeon.

- **`PlayerConditionId Property`**
  ```csharp
  public uint PlayerConditionId { get; }
  ```
  Gets the player condition identifier.

- **`Raids Property`**
  ```csharp
  public static IEnumerable&lt;LfgDungeons&gt; Raids { get; }
  ```
  Gets the raids.

- **`RequiresGroup Property`**
  ```csharp
  public bool RequiresGroup { get; }
  ```
  Returns true if lfg requires a group

- **`Scenarios Property`**
  ```csharp
  public static IEnumerable&lt;LfgDungeons&gt; Scenarios { get; }
  ```
  Gets the scenarios.

- **`SubType Property`**
  ```csharp
  public LfgSubType SubType { get; }
  ```
  Gets the type of the LFG.

- **`TargetLevel Property`**
  ```csharp
  public int TargetLevel { get; }
  ```

- **`TextureId Property`**
  ```csharp
  public int TextureId { get; }
  ```

- **`TypeId Property`**
  ```csharp
  public uint TypeId { get; }
  ```
  The typeid of this dungeon.

- **`WorldEvents Property`**
  ```csharp
  public static IEnumerable&lt;LfgDungeons&gt; WorldEvents { get; }
  ```
  Gets the world events.

- **`Zones Property`**
  ```csharp
  public static IEnumerable&lt;LfgDungeons&gt; Zones { get; }
  ```
  Gets the zones.

#### LfgDungeons Methods

- **`Equals Method`**

- **`Equals Method (Object)`**
  ```csharp
  public override bool Equals(
Object obj
)
  ```
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LST667EB259_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceLfgDungeons Class

- **`Equals Method (LfgDungeons)`**
  ```csharp
  public bool Equals(
LfgDungeons other
)
  ```
  - *other*: Type: Styx.WoWInternals.DBCAddLanguageSpecificTextSet("LST19B74EFF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LfgDungeons
  - **Returns:** See Also

- **`FromId Method`**
  ```csharp
  public static LfgDungeons FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST908AA51B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceLfgDungeons Class

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  - **Returns:** ReferenceLfgDungeons Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceLfgDungeons Class

#### LfgDungeons Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
LfgDungeons left,
LfgDungeons right
)
  ```
  - *left*: Type: Styx.WoWInternals.DBCAddLanguageSpecificTextSet("LSTE1C7DAE0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LfgDungeons
  - *right*: Type: Styx.WoWInternals.DBCAddLanguageSpecificTextSet("LSTE1C7DAE0_2?cs=.|vb=.|cpp=::|nu=.|fs=.");LfgDungeons
  - **Returns:** ReferenceLfgDungeons Class

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
LfgDungeons left,
LfgDungeons right
)
  ```
  - *left*: Type: Styx.WoWInternals.DBCAddLanguageSpecificTextSet("LST5BB89A23_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LfgDungeons
  - *right*: Type: Styx.WoWInternals.DBCAddLanguageSpecificTextSet("LST5BB89A23_2?cs=.|vb=.|cpp=::|nu=.|fs=.");LfgDungeons
  - **Returns:** ReferenceLfgDungeons Class

---

### LfgDungeonsFlags Enumeration

```csharp
[FlagsAttribute]
public enum LfgDungeonsFlags
```

---

### LfgSubType Enumeration

```csharp
public enum LfgSubType
```

---

### Map Class

**Inheritance:** System.Object → Styx.WoWInternals.DBC.Map

```csharp
public class Map
```

Represents a Map dbc entry.

#### Map Properties

- **`AreaTableId Property`**
  ```csharp
  public uint AreaTableId { get; }
  ```
  The area table id of this map.

- **`CosmeticParentMapId Property`**
  ```csharp
  public int CosmeticParentMapId { get; }
  ```
  The cosmetic parent map id.

- **`ExpansionId Property`**
  ```csharp
  public uint ExpansionId { get; }
  ```
  The expansion id.

- **`GhostEntranceLocation Property`**
  ```csharp
  public Vector2 GhostEntranceLocation { get; }
  ```
  The ghost entrance location.

- **`GhostEntranceMap Property`**
  ```csharp
  public Map GhostEntranceMap { get; }
  ```
  The ghost entrance map.

- **`InstanceType Property`**
  ```csharp
  public InstanceType InstanceType { get; }
  ```
  The InstanceType for this Map

- **`InternalName Property`**
  ```csharp
  public string InternalName { get; }
  ```
  The name of this map.

- **`IsArena Property`**
  ```csharp
  public bool IsArena { get; }
  ```
  Returns a value incidating if this is a arena.

- **`IsBattleground Property`**
  ```csharp
  public bool IsBattleground { get; }
  ```
  Returns a value incidating if this is a battleground.

- **`IsContinent Property`**
  ```csharp
  public bool IsContinent { get; }
  ```
  Returns a value incidating if this is one of the major continents.

- **`IsDungeon Property`**
  ```csharp
  public bool IsDungeon { get; }
  ```
  Returns a value incidating if this is a dungeon, (not raid).

- **`IsGarrison Property`**
  ```csharp
  public bool IsGarrison { get; }
  ```
  Returns a value indicating whether this is a garrison.

- **`IsInstance Property`**
  ```csharp
  public bool IsInstance { get; }
  ```
  Returns a value incidating if this is an instance.

- **`IsPveInstance Property`**
  ```csharp
  public bool IsPveInstance { get; }
  ```
  Returns a value incidating if this is a pve instance, (raid, dungeon or scenario).

- **`IsRaid Property`**
  ```csharp
  public bool IsRaid { get; }
  ```
  Returns a value incidating if this is a raid.

- **`IsScenario Property`**
  ```csharp
  public bool IsScenario { get; }
  ```
  Returns a value indicating if this is a scenario.

- **`LoadingScreenId Property`**
  ```csharp
  public uint LoadingScreenId { get; }
  ```
  The loading screen id, referes to LoadingScreens.dbc.

- **`MapDescription Property`**
  ```csharp
  public string MapDescription { get; }
  ```
  The description of this map.

- **`MapDescription2 Property`**
  ```csharp
  public string MapDescription2 { get; }
  ```
  The description 2 of this map.

- **`MapFlags Property`**
  ```csharp
  public uint MapFlags { get; }
  ```
  The flags of this Map

- **`MapId Property`**
  ```csharp
  public uint MapId { get; }
  ```
  The map id.

- **`MapType Property`**
  ```csharp
  public MapType MapType { get; }
  ```
  The MapType for this Map

- **`MaxPlayers Property`**
  ```csharp
  public uint MaxPlayers { get; }
  ```
  The max players of this map, usally only valid in battlegrounds.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  The name of this map.

- **`ParentMapId Property`**
  ```csharp
  public int ParentMapId { get; }
  ```
  The parent map id.

- **`TimeOfDayOverride Property`**
  ```csharp
  public uint TimeOfDayOverride { get; }
  ```
  The time of day override.

#### Map Methods

- **`FromId Method`**
  ```csharp
  public static Map FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTB0100015_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceMap Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceMap Class

---

### MapDifficulty Class

**Inheritance:** System.Object → Styx.WoWInternals.DBC.MapDifficulty

```csharp
public class MapDifficulty
```

Represents a map difficulty record.

#### MapDifficulty Properties

- **`AreaTriggerText Property`**
  ```csharp
  public string AreaTriggerText { get; }
  ```
  The area trigger text of this map difficulty.

- **`Difficulty Property`**
  ```csharp
  public uint Difficulty { get; }
  ```
  The difficulty of this dungeon difficulty.

- **`DifficultyString Property`**
  ```csharp
  public string DifficultyString { get; }
  ```
  The difficulty string of this map difficulty.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  The id of this map difficulty entry.

- **`MapId Property`**
  ```csharp
  public uint MapId { get; }
  ```
  The map id of this dungeon difficulty.

- **`MaxPlayers Property`**
  ```csharp
  public uint MaxPlayers { get; }
  ```
  The max players of this dungeon difficulty.

- **`RaidDuration Property`**
  ```csharp
  public uint RaidDuration { get; }
  ```
  The map id.

#### MapDifficulty Methods

- **`FromId Method`**
  ```csharp
  public static MapDifficulty FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTBE13AE7E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceMapDifficulty Class

---

### MapType Enumeration

```csharp
public enum MapType
```

Values that represent InstanceType.

---

### PetFoodFlags Enumeration

```csharp
[FlagsAttribute]
public enum PetFoodFlags
```

Bitfield of flags for specifying PetFoodFlags.

---

### RecipeAcquireMethod Enumeration

```csharp
public enum RecipeAcquireMethod
```

---

### ResearchSite Class

**Inheritance:** System.Object → Styx.WoWInternals.DBC.ResearchSite

```csharp
public class ResearchSite
```

A research site.

#### ResearchSite Properties

- **`ID Property`**
  ```csharp
  public uint ID { get; }
  ```
  Gets the identifier.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`QuestPoiBlobID Property`**
  ```csharp
  public uint QuestPoiBlobID { get; }
  ```
  Gets the identifier of the question poi BLOB.

- **`TextureIndex Property`**
  ```csharp
  public int TextureIndex { get; }
  ```
  Gets the zero-based index of the texture.

#### ResearchSite Methods

- **`FromId Method`**
  ```csharp
  public static ResearchSite FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST4DB2250F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceResearchSite Class

---

### ScalingStatDistribution Class

**Inheritance:** System.Object → Styx.WoWInternals.DBC.ScalingStatDistribution

```csharp
public class ScalingStatDistribution
```

Represents a scaling stat distribution DBC entry.

#### ScalingStatDistribution Properties

- **`ID Property`**
  ```csharp
  public uint ID { get; }
  ```
  The ID of this scaling stat distribution.

- **`MaxLevel Property`**
  ```csharp
  public int MaxLevel { get; }
  ```
  The max level of this scaling stat distribution.

- **`MinLevel Property`**
  ```csharp
  public int MinLevel { get; }
  ```
  The min level of this scaling stat distribution.

#### ScalingStatDistribution Methods

- **`FromId Method`**
  ```csharp
  public static ScalingStatDistribution FromId(
uint id
)
  ```
  Gets a ScalingStatDistribution instance from the specified ID.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTF86440C8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The ID.
  - **Returns:** ReferenceScalingStatDistribution Class

---

### SkillLineAbility Class

**Inheritance:** System.Object → Styx.WoWInternals.DBC.SkillLineAbility

```csharp
public class SkillLineAbility
```

How the recipe/ability is acquired

#### SkillLineAbility Properties

- **`AcquireMethod Property`**
  ```csharp
  public RecipeAcquireMethod AcquireMethod { get; }
  ```
  How the recipe/ability is acquired

- **`DisplayOrder Property`**
  ```csharp
  public int DisplayOrder { get; }
  ```
  The order in which the entries are displayed in tradeskill frame.

- **`GreenSkillLevel Property`**
  ```csharp
  public int GreenSkillLevel { get; }
  ```
  The skill level that recipe is shown as green (medium) difficulty

- **`GreySkillLevel Property`**
  ```csharp
  public int GreySkillLevel { get; }
  ```
  The skill level that recipe is shown as gray (trivial) difficulty

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  The Id

- **`NextSpellId Property`**
  ```csharp
  public int NextSpellId { get; }
  ```
  The next Skill rank spellId - 0 for Recipes

- **`ReqClasses Property`**
  ```csharp
  public uint ReqClasses { get; }
  ```
  Required class bitmask for ChrClasses.dbc

- **`ReqRaces Property`**
  ```csharp
  public uint ReqRaces { get; }
  ```
  Required race bitmask for ChrRaces.dbc

- **`SkillLine Property`**
  ```csharp
  public SkillLine SkillLine { get; }
  ```

- **`SkillPointsEarned Property`**
  ```csharp
  public int SkillPointsEarned { get; }
  ```
  The amount of skill points earned when gaining a skillup while recipe is at optimal difficulty

- **`SpellId Property`**
  ```csharp
  public int SpellId { get; }
  ```
  The SpellId for recipe

- **`TradeSkillCategoryId Property`**
  ```csharp
  public int TradeSkillCategoryId { get; }
  ```
  The TradeSkillCategory Id

- **`YellowSkillLevel Property`**
  ```csharp
  public int YellowSkillLevel { get; }
  ```
  The skill level that recipe is shown as yellow (medium) difficulty

#### SkillLineAbility Methods

- **`FromId Method`**
  ```csharp
  public static SkillLineAbility FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST526A0CA4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceSkillLineAbility Class

- **`FromSpellId Method`**
  ```csharp
  public static IEnumerable&lt;SkillLineAbility&gt; FromSpellId(
int spellId
)
  ```
  - *spellId*: Type: SystemAddLanguageSpecificTextSet("LSTD474D8BB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
  - **Returns:** See Also

- **`GetAbilities Method`**
  ```csharp
  public static IEnumerable&lt;SkillLineAbility&gt; GetAbilities()
  ```
  - **Returns:** See Also

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceSkillLineAbility Class

---

### SkillLineCategory Enumeration

```csharp
public enum SkillLineCategory
```

---

### SkillLineInfo Class

**Inheritance:** System.Object → Styx.WoWInternals.DBC.SkillLineInfo

```csharp
public class SkillLineInfo
```

Information about the skill line.

#### SkillLineInfo Properties

- **`CanLink Property`**
  ```csharp
  public bool CanLink { get; }
  ```
  Gets a value indicating whether we can link.

- **`Category Property`**
  ```csharp
  public SkillLineCategory Category { get; }
  ```
  Gets the SkillLine category.

- **`Description Property`**
  ```csharp
  public string Description { get; }
  ```
  Gets the description.

- **`Flags Property`**
  ```csharp
  public uint Flags { get; }
  ```

- **`ID Property`**
  ```csharp
  public uint ID { get; }
  ```
  Gets the identifier.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`ParentSkillLineId Property`**
  ```csharp
  public int ParentSkillLineId { get; }
  ```

- **`SpellIconId Property`**
  ```csharp
  public int SpellIconId { get; }
  ```
  Gets the spell icon ID.

- **`Verb Property`**
  ```csharp
  public string Verb { get; }
  ```
  Gets the verb.

#### SkillLineInfo Methods

- **`FromId Method`**
  Gets skill line info for the skill line with the specified ID.

- **`FromId Method (UInt32)`**
  ```csharp
  public static SkillLineInfo FromId(
uint id
)
  ```
  Gets skill line info for the skill line with the specified ID.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST4E7449BA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The ID.
  - **Returns:** ReferenceSkillLineInfo Class

- **`FromId Method (SkillLine)`**
  ```csharp
  public static SkillLineInfo FromId(
SkillLine skillLine
)
  ```
  Gets skill line info for the specified skill line.
  - *skillLine*: Type: StyxAddLanguageSpecificTextSet("LST2A25BD87_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SkillLineThe skill line.
  - **Returns:** ReferenceSkillLineInfo Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceSkillLineInfo Class

---

### SpellEffect Class

**Inheritance:** System.Object → Styx.WoWInternals.DBC.SpellEffect

```csharp
public class SpellEffect
```

A spell effect.

#### SpellEffect Properties

- **`AuraType Property`**
  ```csharp
  public WoWApplyAuraType AuraType { get; }
  ```
  Gets the type of the aura.

- **`BasePoints Property`**
  ```csharp
  public int BasePoints { get; }
  ```
  The base points of this effect. This usually indicates the value of the spell effect.

- **`EffectType Property`**
  ```csharp
  public WoWSpellEffectType EffectType { get; }
  ```
  Gets the type of the effect.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Gets the ID of the spell effect.

- **`ImplicitTargetA Property`**
  ```csharp
  public uint ImplicitTargetA { get; }
  ```
  Gets the implicit target a.

- **`ImplicitTargetB Property`**
  ```csharp
  public uint ImplicitTargetB { get; }
  ```
  Gets the implicit target b.

- **`ItemType Property`**
  ```csharp
  public uint ItemType { get; }
  ```
  Gets the type of the item.

- **`MiscValueA Property`**
  ```csharp
  public int MiscValueA { get; }
  ```
  Gets the misc value a.

- **`MiscValueB Property`**
  ```csharp
  public int MiscValueB { get; }
  ```
  Gets the misc value b.

#### SpellEffect Methods

- **`FromId Method`**
  ```csharp
  public static SpellEffect FromId(
uint id
)
  ```
  Initializes this object from the given from identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST15424F26_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceSpellEffect Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceSpellEffect Class

---

### SpellItemEnchantment Class

**Inheritance:** System.Object → Styx.WoWInternals.DBC.SpellItemEnchantment

```csharp
public class SpellItemEnchantment
```

Returns the Id of this enchant.

#### SpellItemEnchantment Properties

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Returns the Id of this enchant.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Returns the name of this enchant.

#### SpellItemEnchantment Methods

- **`FromId Method`**
  ```csharp
  public static SpellItemEnchantment FromId(
uint id
)
  ```
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST8CF8F5AF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceSpellItemEnchantment Class

- **`GetStat Method`**
  ```csharp
  public WoWItem.WoWItemStat GetStat(
int index
)
  ```
  Valid values are 0-2
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTBF1EC04D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The index.
  - **Returns:** See Also

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents this instance.
  - **Returns:** ReferenceSpellItemEnchantment Class

---
