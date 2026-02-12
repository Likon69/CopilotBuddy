# Styx.WoWInternals.WoWCache

Contains WoW cache related classes.

## Contents

- [CacheDb Enumeration](#cachedb-enumeration)
- [WoWCache Class](#wowcache-class)
- [WoWCache.Cache Class](#wowcache.cache-class)
- [WoWCache.CreatureCacheEntry Structure](#wowcache.creaturecacheentry-structure)
- [WoWCache.CreatureCacheTypeFlags Enumeration](#wowcache.creaturecachetypeflags-enumeration)
- [WoWCache.GameObjectCacheEntry Structure](#wowcache.gameobjectcacheentry-structure)
- [WoWCache.InfoBlock Class](#wowcache.infoblock-class)
- [WoWCache.JamDynamicString Structure](#wowcache.jamdynamicstring-structure)
- [WoWCache.QuestCacheEntry Structure](#wowcache.questcacheentry-structure)
- [WoWCache.QuestFlags Enumeration](#wowcache.questflags-enumeration)
- [WoWCache.QuestInfoTagType Enumeration](#wowcache.questinfotagtype-enumeration)
- [WoWCache.QuestObjectiveInfo Structure](#wowcache.questobjectiveinfo-structure)
- [WoWCache.QuestRewardChoice Structure](#wowcache.questrewardchoice-structure)
- [WoWCache.SocketColorFlags Enumeration](#wowcache.socketcolorflags-enumeration)

---

### CacheDb Enumeration

```csharp
public enum CacheDb
```

---

### WoWCache Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWCache.WoWCache

```csharp
public class WoWCache
```

A wow cache.

#### WoWCache Properties

- **`Item Property`**
  ```csharp
  public WoWCache.Cache this[
CacheDb index
] { get; }
  ```
  - *index*: Type: Styx.WoWInternals.WoWCacheAddLanguageSpecificTextSet("LST60397875_2?cs=.|vb=.|cpp=::|nu=.|fs=.");CacheDb

---

### WoWCache.Cache Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWCache.WoWCache.Cache

```csharp
public class Cache
```

Determines whether the specified object is equal to the current object.

#### Cache Methods

- **`GetInfoBlockById Method`**
  ```csharp
  public WoWCache.InfoBlock GetInfoBlockById(
uint id
)
  ```
  Gets an infoblock. Returns null if there was no infoblock for the passed id.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST83FC2FA0_3?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** See Also

#### Cache Fields

- **`Address Field`**
  ```csharp
  public readonly IntPtr Address
  ```

---

### WoWCache.CreatureCacheEntry Structure

```csharp
public struct CreatureCacheEntry
```

Gets the question items.

#### CreatureCacheEntry Properties

- **`QuestItems Property`**
  ```csharp
  public uint[] QuestItems { get; }
  ```
  Gets the question items.

#### CreatureCacheEntry Fields

- **`Classification Field`**
  ```csharp
  public int Classification
  ```

- **`CreatureMovementInfoID Field`**
  ```csharp
  public readonly uint CreatureMovementInfoID
  ```

- **`CursorName Field`**
  ```csharp
  public WoWCache.JamDynamicString CursorName
  ```

- **`FamilyID Field`**
  ```csharp
  public uint FamilyID
  ```

- **`GroupID Field`**
  ```csharp
  public uint[] GroupID
  ```

- **`HealthModifier Field`**
  ```csharp
  public float HealthModifier
  ```

- **`ModelID Field`**
  ```csharp
  public uint[] ModelID
  ```

- **`Name Field`**
  ```csharp
  public WoWCache.JamDynamicString[] Name
  ```

- **`Name2 Field`**
  ```csharp
  public WoWCache.JamDynamicString[] Name2
  ```

- **`PowerModifier Field`**
  ```csharp
  public float PowerModifier
  ```

- **`Rank Field`**
  ```csharp
  public uint Rank
  ```

- **`RequiredExpansion Field`**
  ```csharp
  public readonly int RequiredExpansion
  ```

- **`SubName Field`**
  ```csharp
  public WoWCache.JamDynamicString SubName
  ```

- **`SubNameAlt Field`**
  ```csharp
  public WoWCache.JamDynamicString SubNameAlt
  ```

- **`TypeFlags Field`**
  ```csharp
  public WoWCache.CreatureCacheTypeFlags[] TypeFlags
  ```

- **`TypeID Field`**
  ```csharp
  public uint TypeID
  ```

---

### WoWCache.CreatureCacheTypeFlags Enumeration

```csharp
[FlagsAttribute]
public enum CreatureCacheTypeFlags
```

---

### WoWCache.GameObjectCacheEntry Structure

```csharp
public struct GameObjectCacheEntry
```

A game object cache entry.

#### GameObjectCacheEntry Properties

- **`QuestItems Property`**
  ```csharp
  public uint[] QuestItems { get; }
  ```

#### GameObjectCacheEntry Fields

- **`CursorNamePtr Field`**
  ```csharp
  public IntPtr CursorNamePtr
  ```
  The cast bar caption pointer.

- **`DisplayId Field`**
  ```csharp
  public int DisplayId
  ```
  Identifier for the display.

- **`GameObjectType Field`**
  ```csharp
  public int GameObjectType
  ```
  Type of the game object.

- **`NamePtrs Field`**
  ```csharp
  public IntPtr[] NamePtrs
  ```
  The name ptrs.

- **`Properties Field`**
  ```csharp
  public int[] Properties
  ```
  The properties.

- **`Scale Field`**
  ```csharp
  public float Scale
  ```
  The scale.

- **`UnkString0Ptr Field`**
  ```csharp
  public IntPtr UnkString0Ptr
  ```
  The unk string 0 pointer.

- **`UnkString1Ptr Field`**
  ```csharp
  public IntPtr UnkString1Ptr
  ```
  The unk string 1 pointer.

---

### WoWCache.InfoBlock Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWCache.WoWCache.InfoBlock

```csharp
public class InfoBlock
```

An information block.

#### InfoBlock Properties

- **`Address Property`**
  ```csharp
  public IntPtr Address { get; }
  ```
  Gets the address.

- **`Creature Property`**
  ```csharp
  public WoWCache.CreatureCacheEntry Creature { get; }
  ```
  Gets the creature.

- **`GameObject Property`**
  ```csharp
  public WoWCache.GameObjectCacheEntry GameObject { get; }
  ```
  Gets the game object.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Gets the identifier.

- **`Quest Property`**
  ```csharp
  public WoWCache.QuestCacheEntry Quest { get; }
  ```
  Gets the quest.

#### InfoBlock Methods

- **`GetField(T) Method`**
  ```csharp
  public T GetField&lt;T&gt;(
uint index
)
where T : struct, new()
  ```
  Returns a field of a type you specify.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST627D3D73_4?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32.
  - *T*: .
  - **Returns:** See Also

- **`GetStringField Method`**
  Gets a string from a field in a specific type with the specific name.

- **`GetStringField(T) Method (String)`**
  ```csharp
  public string GetStringField&lt;T&gt;(
string fieldName
)
  ```
  Gets a string from a field in a specific type with the specific name.
  - *fieldName*: Type: SystemAddLanguageSpecificTextSet("LST4930A8D8_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *T*: .
  - **Returns:** ReferenceWoWCacheAddLanguageSpecificTextSet("LST4930A8D8_5?cs=.|vb=.|cpp=::|nu=.|fs=.");InfoBlock Class

- **`GetStringField Method (UInt32)`**
  ```csharp
  public string GetStringField(
uint index
)
  ```
  Gets a string from specific field.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST7E9DEFCF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32.
  - **Returns:** ReferenceWoWCacheAddLanguageSpecificTextSet("LST7E9DEFCF_3?cs=.|vb=.|cpp=::|nu=.|fs=.");InfoBlock Class

- **`GetStruct(T) Method`**
  ```csharp
  public T GetStruct&lt;T&gt;()
where T : struct, new()
  ```
  Returns a struct representation of this row. (The struct must be properly formatted.
            If it contains strings, it must be marked with the CharSet value to Ansi)
  - *T*: .
  - **Returns:** See Also

---

### WoWCache.JamDynamicString Structure

```csharp
public struct JamDynamicString
```

Indicates whether this instance and a specified object are equal.

#### JamDynamicString Fields

- **`Length Field`**
  ```csharp
  public int Length
  ```

- **`PtrStr Field`**
  ```csharp
  public IntPtr PtrStr
  ```

---

### WoWCache.QuestCacheEntry Structure

```csharp
public struct QuestCacheEntry
```

Indicates whether this instance and a specified object are equal.

#### QuestCacheEntry Methods

- **`GetObjectives Method`**
  Gets the objectives of this quest, including all intermediate objectives.

- **`GetObjectives Method`**
  ```csharp
  public WoWCache.QuestObjectiveInfo[] GetObjectives()
  ```
  Gets the objectives of this quest, including all intermediate objectives.
  - **Returns:** See Also

- **`GetObjectives Method (Boolean)`**
  ```csharp
  public WoWCache.QuestObjectiveInfo[] GetObjectives(
bool includeIntermediate
)
  ```
  Gets the objectives of this quest.
  - *includeIntermediate*: Type: SystemAddLanguageSpecificTextSet("LST4FF9CB44_3?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanIndicates whether to include intermediate objectives.
  - **Returns:** See Also

#### QuestCacheEntry Fields

- **`_unk15 Field`**
  ```csharp
  public uint[] _unk15
  ```

- **`AreaIdOrSortId Field`**
  ```csharp
  public uint AreaIdOrSortId
  ```

- **`CompletionText Field`**
  ```csharp
  public byte[] CompletionText
  ```

- **`Description Field`**
  ```csharp
  public byte[] Description
  ```

- **`EndText Field`**
  ```csharp
  public byte[] EndText
  ```

- **`Flags Field`**
  ```csharp
  public uint Flags
  ```

- **`Flags2 Field`**
  ```csharp
  public uint Flags2
  ```

- **`FriendlyFactionAmount Field`**
  ```csharp
  public uint FriendlyFactionAmount
  ```

- **`FriendlyFactionId Field`**
  ```csharp
  public uint FriendlyFactionId
  ```

- **`Id Field`**
  ```csharp
  public uint Id
  ```

- **`Level Field`**
  ```csharp
  public uint Level
  ```

- **`Method Field`**
  ```csharp
  public uint Method
  ```

- **`Name Field`**
  ```csharp
  public byte[] Name
  ```

- **`NumQuestObjectives Field`**
  ```csharp
  public int NumQuestObjectives
  ```

- **`ObjectiveId Field`**
  ```csharp
  public uint[] ObjectiveId
  ```

- **`ObjectiveRequiredCount Field`**
  ```csharp
  public uint[] ObjectiveRequiredCount
  ```

- **`ObjectiveText Field`**
  ```csharp
  public byte[] ObjectiveText
  ```

- **`PointOptional Field`**
  ```csharp
  public float PointOptional
  ```

- **`PointX Field`**
  ```csharp
  public float PointX
  ```

- **`PointY Field`**
  ```csharp
  public float PointY
  ```

- **`PortraitGiverId Field`**
  ```csharp
  public uint PortraitGiverId
  ```

- **`PortraitGiverName Field`**
  ```csharp
  public byte[] PortraitGiverName
  ```

- **`PortraitGiverText Field`**
  ```csharp
  public byte[] PortraitGiverText
  ```

- **`PortraitTurnInDescription Field`**
  ```csharp
  public byte[] PortraitTurnInDescription
  ```

- **`PortraitTurnInId Field`**
  ```csharp
  public uint PortraitTurnInId
  ```

- **`PortraitTurnInName Field`**
  ```csharp
  public byte[] PortraitTurnInName
  ```

- **`QuestObjectives Field`**
  ```csharp
  public IntPtr QuestObjectives
  ```

- **`QuestRewardChoiceCollection Field`**
  ```csharp
  public IntPtr QuestRewardChoiceCollection
  ```

- **`QuestType Field`**
  ```csharp
  public uint QuestType
  ```

- **`RequiredLevel Field`**
  ```csharp
  public uint RequiredLevel
  ```

- **`RewardHonor Field`**
  ```csharp
  public uint RewardHonor
  ```

- **`RewardHonorBonus Field`**
  ```csharp
  public uint RewardHonorBonus
  ```

- **`RewardItemCounts Field`**
  ```csharp
  public uint[] RewardItemCounts
  ```

- **`RewardItemIDs Field`**
  ```csharp
  public uint[] RewardItemIDs
  ```

- **`RewardMoney Field`**
  ```csharp
  public uint RewardMoney
  ```

- **`RewardMoneyInsteadOfXP Field`**
  ```csharp
  public uint RewardMoneyInsteadOfXP
  ```

- **`RewardSkillId Field`**
  ```csharp
  public uint RewardSkillId
  ```

- **`RewardSkillPoints Field`**
  ```csharp
  public uint RewardSkillPoints
  ```

- **`RewardSpellId Field`**
  ```csharp
  public uint RewardSpellId
  ```

- **`RewardSpellId2 Field`**
  ```csharp
  public uint RewardSpellId2
  ```

- **`RewardTalentPoints Field`**
  ```csharp
  public uint RewardTalentPoints
  ```

- **`RewardTitleId Field`**
  ```csharp
  public uint RewardTitleId
  ```

- **`SortId Field`**
  ```csharp
  public uint SortId
  ```

- **`StartingItemId Field`**
  ```csharp
  public uint StartingItemId
  ```

- **`SuggestedPlayers Field`**
  ```csharp
  public uint SuggestedPlayers
  ```

---

### WoWCache.QuestFlags Enumeration

```csharp
[FlagsAttribute]
public enum QuestFlags
```

---

### WoWCache.QuestInfoTagType Enumeration

```csharp
public enum QuestInfoTagType
```

---

### WoWCache.QuestObjectiveInfo Structure

```csharp
public struct QuestObjectiveInfo
```

Indicates whether this instance and a specified object are equal.

#### QuestObjectiveInfo Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceWoWCacheAddLanguageSpecificTextSet("LST6A13376D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");QuestObjectiveInfo Structure

#### QuestObjectiveInfo Fields

- **`dword0 Field`**
  ```csharp
  public uint dword0
  ```

- **`dword114 Field`**
  ```csharp
  public uint dword114
  ```

- **`dword118 Field`**
  ```csharp
  public uint dword118
  ```

- **`dword11C Field`**
  ```csharp
  public IntPtr dword11C
  ```

- **`dword120 Field`**
  ```csharp
  public uint dword120
  ```

- **`dword124 Field`**
  ```csharp
  public uint dword124
  ```

- **`dword128 Field`**
  ```csharp
  public uint dword128
  ```

- **`Flags Field`**
  ```csharp
  public uint Flags
  ```

- **`ObjectiveId Field`**
  ```csharp
  public uint ObjectiveId
  ```

- **`ObjectiveType Field`**
  ```csharp
  public Quest.QuestObjectiveType ObjectiveType
  ```

- **`QuestLogObjectiveIndex Field`**
  ```csharp
  public sbyte QuestLogObjectiveIndex
  ```

- **`RequiredCount Field`**
  ```csharp
  public uint RequiredCount
  ```

- **`Text Field`**
  ```csharp
  public string Text
  ```

- **`word6 Field`**
  ```csharp
  public ushort word6
  ```

---

### WoWCache.QuestRewardChoice Structure

```csharp
public struct QuestRewardChoice
```

Indicates whether this instance and a specified object are equal.

#### QuestRewardChoice Fields

- **`Count Field`**
  ```csharp
  public uint Count
  ```

- **`ItemId Field`**
  ```csharp
  public uint ItemId
  ```

---

### WoWCache.SocketColorFlags Enumeration

```csharp
[FlagsAttribute]
public enum SocketColorFlags
```

---
