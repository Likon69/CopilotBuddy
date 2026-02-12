# Styx.WoWInternals.Garrison

## Contents

- [GarrisonBuilding Class](#garrisonbuilding-class)
- [GarrisonFollower Class](#garrisonfollower-class)
- [GarrisonFollowerStatus Enumeration](#garrisonfollowerstatus-enumeration)
- [GarrisonInfo Class](#garrisoninfo-class)
- [GarrisonMission Class](#garrisonmission-class)
- [GarrisonMission.GarrisonMissionFollower Class](#garrisonmission.garrisonmissionfollower-class)
- [GarrisonMissionReward Class](#garrisonmissionreward-class)
- [GarrisonMissionRewardInfo Class](#garrisonmissionrewardinfo-class)
- [GarrisonMissionRewardInfo.NativeGarrisonMissionInfo Structure](#garrisonmissionrewardinfo.nativegarrisonmissioninfo-structure)
- [GarrisonMissionRewardInfo.NativeMissionRewardContainer Structure](#garrisonmissionrewardinfo.nativemissionrewardcontainer-structure)
- [GarrisonMissionSimulator Class](#garrisonmissionsimulator-class)
- [GarrisonPlot Class](#garrisonplot-class)
- [GarrisonShipmentInfo Class](#garrisonshipmentinfo-class)
- [LandingPageShipmentInfo Structure](#landingpageshipmentinfo-structure)
- [MissionSimulatorOptions Class](#missionsimulatoroptions-class)
- [MissionSimulatorResults Class](#missionsimulatorresults-class)
- [MissionState Enumeration](#missionstate-enumeration)
- [OwnedBuildingInfo Structure](#ownedbuildinginfo-structure)

---

### GarrisonBuilding Class

**Inheritance:** System.Object → Styx.WoWInternals.Garrison.GarrisonBuilding

```csharp
public class GarrisonBuilding
```

The GarrBuilding Id of this building

#### GarrisonBuilding Properties

- **`BuildingId Property`**
  ```csharp
  public uint BuildingId { get; }
  ```
  The GarrBuilding Id of this building

- **`BuildTime Property`**
  ```csharp
  public TimeSpan BuildTime { get; }
  ```
  How long it takes for this building to be fully built.

- **`CurrencyCost Property`**
  ```csharp
  public int CurrencyCost { get; }
  ```
  The cost (in currency, usually Garrison Resources) to build this building.

- **`GameObject Property`**
  ```csharp
  public WoWGameObject GameObject { get; }
  ```
  Returns the WoWGameObject instance of this building.

- **`GoldCost Property`**
  ```csharp
  public int GoldCost { get; }
  ```
  The cost (in gold/copper) to build this building.

- **`IsBuilt Property`**
  ```csharp
  public bool IsBuilt { get; }
  ```
  Whether or not this building has been placed and is either fully built, or being built.

- **`Level Property`**
  ```csharp
  public int Level { get; }
  ```
  The rank (or level) of this building. Typically between 0-3.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  The localized name of this building.

- **`NeedsPlan Property`**
  ```csharp
  public bool NeedsPlan { get; }
  ```
  Whether or not the player still needs the blueprint to build this building.

- **`PlotId Property`**
  ```csharp
  public int PlotId { get; }
  ```
  The GarrPlot Id of this building.

- **`PlotInstanceId Property`**
  ```csharp
  public uint PlotInstanceId { get; }
  ```
  The GarrPlotInstance Id of this building.

- **`PossibleSpecializations Property`**
  ```csharp
  public IReadOnlyList&lt;int&gt; PossibleSpecializations { get; }
  ```
  Any possible specializations (GarrSpecialization records) that this building has.

- **`Record Property`**
  ```csharp
  public GarrBuilding Record { get; }
  ```
  The backing record for this garrison building.

- **`Size Property`**
  ```csharp
  public int Size { get; }
  ```
  The size of this building (small, medium, large, etc)

- **`Type Property`**
  ```csharp
  public GarrisonBuildingType Type { get; }
  ```
  The type of this garrison building.

- **`UpgradeBuildingIds Property`**
  ```csharp
  public IReadOnlyList&lt;int&gt; UpgradeBuildingIds { get; }
  ```
  A collection of GarrBuilding Ids that this building can be upgraded to.

#### GarrisonBuilding Methods

- **`AssignFollower Method`**
  ```csharp
  public void AssignFollower(
GarrisonFollower follower
)
  ```
  Assigns the specified follower to work in this garrison building.
  - *follower*: Type: Styx.WoWInternals.GarrisonAddLanguageSpecificTextSet("LST44A8272B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");GarrisonFollower

- **`RemoveFollower Method`**
  ```csharp
  public void RemoveFollower()
  ```
  Removes the current follower working in this garrison building.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a string that represents the current object.
  - **Returns:** ReferenceGarrisonBuilding Class

---

### GarrisonFollower Class

**Inheritance:** System.Object → Styx.WoWInternals.Garrison.GarrisonFollower

```csharp
public class GarrisonFollower : IEquatable&lt;GarrisonFollower&gt;
```

The abilities that this follower has.

#### GarrisonFollower Properties

- **`Abilities Property`**
  ```csharp
  public IEnumerable&lt;GarrAbility&gt; Abilities { get; }
  ```
  The abilities that this follower has.

- **`AbilityIds Property`**
  ```csharp
  public int[] AbilityIds { get; }
  ```
  An array of all abilities this follower currently has. (This includes traits)

- **`AllAbilities Property`**
  ```csharp
  public IEnumerable&lt;GarrAbility&gt; AllAbilities { get; }
  ```
  All abilities and traits that this follower has.

- **`ArmorLevel Property`**
  ```csharp
  public int ArmorLevel { get; }
  ```
  The current armor item level of this follower.

- **`BaseAddress Property`**
  ```csharp
  public IntPtr BaseAddress { get; }
  ```

- **`CountersMechanicTypes Property`**
  ```csharp
  public IEnumerable&lt;uint&gt; CountersMechanicTypes { get; }
  ```
  A collection of mechanics that this follower counters.

- **`CreatureDisplayId Property`**
  ```csharp
  public int CreatureDisplayId { get; }
  ```

- **`CurrentMissionId Property`**
  ```csharp
  public int CurrentMissionId { get; }
  ```
  The mission ID that this follower is currently on.

- **`EffectiveLevel Property`**
  ```csharp
  public int EffectiveLevel { get; }
  ```
  Returns the effective level of this follower. If the follower is below level 100, this returns the actual level,
                otherwise, it returns the item level.

- **`Experience Property`**
  ```csharp
  public int Experience { get; }
  ```
  The current experience of this follower.

- **`GarrFollowerId Property`**
  ```csharp
  public int GarrFollowerId { get; }
  ```
  The ID of the GarrFollower record that backs this GarrisonFollower

- **`Id Property`**
  ```csharp
  public long Id { get; }
  ```
  The server-side ID of this follower (used for mission tracking, and equality)

- **`IsExperienceCapped Property`**
  ```csharp
  public bool IsExperienceCapped { get; }
  ```
  Whether or not this follower is currently "XP Capped", meaning that it is level 100, and also a quality of Epic.

- **`IsFavorite Property`**
  ```csharp
  public bool IsFavorite { get; }
  ```
  Whether or not this follower is flagged as a "favorite" follower.

- **`IsMaxItemLevel Property`**
  ```csharp
  public bool IsMaxItemLevel { get; }
  ```
  Returns whether or not this follower has the maximum possible item level (currently 655)

- **`ItemLevel Property`**
  ```csharp
  public int ItemLevel { get; }
  ```
  The current item level of this follower.

- **`Level Property`**
  ```csharp
  public int Level { get; }
  ```
  The level of this follower.

- **`LevelExperience Property`**
  ```csharp
  public int LevelExperience { get; }
  ```
  The experience of this follower for the current level.

- **`MaxItemLevel Property`**
  ```csharp
  public static int MaxItemLevel { get; }
  ```
  The current maximum item level a follower can have.

- **`MaxLevel Property`**
  ```csharp
  public static int MaxLevel { get; }
  ```
  The current maximum level a follower can have.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Returns the name of this follower. (Localized to the current client)

- **`Quality Property`**
  ```csharp
  public WoWItemQuality Quality { get; }
  ```
  The quality of this follower.

- **`Status Property`**
  ```csharp
  public GarrisonFollowerStatus Status { get; }
  ```
  Returns the status of this follower.

- **`Traits Property`**
  ```csharp
  public IEnumerable&lt;GarrAbility&gt; Traits { get; }
  ```
  The traits that this follower has.

- **`Type Property`**
  ```csharp
  public GarrisonFollowerType Type { get; }
  ```
  Gets the type.

- **`WeaponLevel Property`**
  ```csharp
  public int WeaponLevel { get; }
  ```
  The current weapon item level of this follower.

#### GarrisonFollower Methods

- **`Equals Method`**
  Determines whether the specified object is equal to the current object.

- **`Equals Method (Object)`**
  ```csharp
  public override bool Equals(
Object obj
)
  ```
  Determines whether the specified object is equal to the current object.
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LST32D2ACBC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe object to compare with the current object.
  - **Returns:** ReferenceGarrisonFollower Class

- **`Equals Method (GarrisonFollower)`**
  ```csharp
  public bool Equals(
GarrisonFollower other
)
  ```
  Indicates whether the current object is equal to another object of the same type.
  - *other*: Type: Styx.WoWInternals.GarrisonAddLanguageSpecificTextSet("LSTEC4A35C3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");GarrisonFollowerAn object to compare with this object.
  - **Returns:** See Also

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  Serves as a hash function for a particular type.
  - **Returns:** ReferenceGarrisonFollower Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a string that represents the current object.
  - **Returns:** ReferenceGarrisonFollower Class

#### GarrisonFollower Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
GarrisonFollower left,
GarrisonFollower right
)
  ```
  - *left*: Type: Styx.WoWInternals.GarrisonAddLanguageSpecificTextSet("LSTAFA699D5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");GarrisonFollower
  - *right*: Type: Styx.WoWInternals.GarrisonAddLanguageSpecificTextSet("LSTAFA699D5_2?cs=.|vb=.|cpp=::|nu=.|fs=.");GarrisonFollower
  - **Returns:** ReferenceGarrisonFollower Class

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
GarrisonFollower left,
GarrisonFollower right
)
  ```
  - *left*: Type: Styx.WoWInternals.GarrisonAddLanguageSpecificTextSet("LST589594BC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");GarrisonFollower
  - *right*: Type: Styx.WoWInternals.GarrisonAddLanguageSpecificTextSet("LST589594BC_2?cs=.|vb=.|cpp=::|nu=.|fs=.");GarrisonFollower
  - **Returns:** ReferenceGarrisonFollower Class

---

### GarrisonFollowerStatus Enumeration

```csharp
public enum GarrisonFollowerStatus
```

---

### GarrisonInfo Class

**Inheritance:** System.Object → Styx.WoWInternals.Garrison.GarrisonInfo

```csharp
public static class GarrisonInfo
```

Returns a list of all buildings in your garrison information.

#### GarrisonInfo Properties

- **`AllBuildings Property`**
  ```csharp
  public static IReadOnlyList&lt;GarrisonBuilding&gt; AllBuildings { get; }
  ```
  Returns a list of all buildings in your garrison information.

- **`CurrentCharShipmentId Property`**
  ```csharp
  public static uint CurrentCharShipmentId { get; }
  ```
  Returns the current shipment ID of the work order NPC currently opened. This is effectively the same as "WorkOrderFrame.IsVisible".

- **`CurrentShipmentOrder Property`**
  ```csharp
  public static GarrisonShipmentInfo CurrentShipmentOrder { get; }
  ```

- **`Followers Property`**
  ```csharp
  public static IReadOnlyList&lt;GarrisonFollower&gt; Followers { get; }
  ```
  Returns a list of all followers in your garrison information.

- **`HasGarrison Property`**
  ```csharp
  public static bool HasGarrison { get; }
  ```
  Gets a value indicating wheter the player has a garrison or not.

- **`LandingPageShipmentInfos Property`**
  ```csharp
  public static IReadOnlyList&lt;LandingPageShipmentInfo&gt; LandingPageShipmentInfos { get; }
  ```
  Returns a list of currently running shipments.
            If the garrison has no shipments either waiting to be picked up, or being "processed", this list is empty.

- **`Missions Property`**
  ```csharp
  public static IEnumerable&lt;GarrisonMission&gt; Missions { get; }
  ```
  Returns a list of all missions currently visible in the garrison.

- **`OwnedBuildings Property`**
  ```csharp
  public static IReadOnlyList&lt;OwnedBuildingInfo&gt; OwnedBuildings { get; }
  ```
  Returns static information about currently owned buildings.

- **`Plots Property`**
  ```csharp
  public static IReadOnlyList&lt;GarrisonPlot&gt; Plots { get; }
  ```
  Returns information about all the plots for the current garrison.

- **`SiteLevel Property`**
  ```csharp
  public static GarrSiteLevel SiteLevel { get; }
  ```
  Gets the garrison site level.

- **`WodGarrisonLevel Property`**
  ```csharp
  public static int WodGarrisonLevel { get; }
  ```
  Gets the level of the player's WoD garrison.

#### GarrisonInfo Methods

- **`GetBuildingByType Method`**
  ```csharp
  public static GarrisonBuilding GetBuildingByType(
GarrisonBuildingType type
)
  ```
  Returns information about a building, by it's type.
  - *type*: Type: Styx.WoWInternals.DBAddLanguageSpecificTextSet("LST63B8C278_1?cs=.|vb=.|cpp=::|nu=.|fs=.");GarrisonBuildingType
  - **Returns:** ReferenceGarrisonInfo Class

- **`GetOwnedBuildingByType Method`**
  ```csharp
  public static OwnedBuildingInfo GetOwnedBuildingByType(
GarrisonBuildingType type
)
  ```
  Returns a  for the specified building type.
  - *type*: Type: Styx.WoWInternals.DBAddLanguageSpecificTextSet("LST9786A603_1?cs=.|vb=.|cpp=::|nu=.|fs=.");GarrisonBuildingType
  - **Returns:** ReferenceGarrisonInfo Class

- **`GetShipmentInfoByType Method`**
  ```csharp
  public static GarrisonShipmentInfo GetShipmentInfoByType(
GarrisonBuildingType type
)
  ```
  Returns shipment information for a building, by it's type.
            This method will attempt to look up the landing page information (only info available), and if that should fail, will look up the shipment through the client DB files.
            If that should also fail, the returned info.CharShipmentId will be zero.
  - *type*: Type: Styx.WoWInternals.DBAddLanguageSpecificTextSet("LST258B02BA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");GarrisonBuildingType
  - **Returns:** ReferenceGarrisonInfo Class

- **`IsBuildingOwned Method`**
  ```csharp
  public static bool IsBuildingOwned(
GarrisonBuildingType type
)
  ```
  Returns whether or not a specific building type is owned.
  - *type*: Type: Styx.WoWInternals.DBAddLanguageSpecificTextSet("LSTCFD49C18_1?cs=.|vb=.|cpp=::|nu=.|fs=.");GarrisonBuildingType
  - **Returns:** ReferenceGarrisonInfo Class

---

### GarrisonMission Class

**Inheritance:** System.Object → Styx.WoWInternals.Garrison.GarrisonMission

```csharp
public class GarrisonMission
```

The time that this mission is available for starting.

#### GarrisonMission Properties

- **`AvailableForDuration Property`**
  ```csharp
  public TimeSpan AvailableForDuration { get; }
  ```
  The time that this mission is available for starting.

- **`BaseAddress Property`**
  ```csharp
  public IntPtr BaseAddress { get; }
  ```

- **`Encounters Property`**
  ```csharp
  public IReadOnlyList&lt;GarrEncounter&gt; Encounters { get; }
  ```
  A collection of GarrEncounters that this mission will have. This is the list of "enemies" that you will face on this mission.

- **`Followers Property`**
  ```csharp
  public IEnumerable&lt;GarrisonMission.GarrisonMissionFollower&gt; Followers { get; }
  ```
  The followers currently assigned to this mission.

- **`FollowerType Property`**
  ```csharp
  public GarrisonFollowerType FollowerType { get; }
  ```
  The Follower type of this mission

- **`Id Property`**
  ```csharp
  public int Id { get; }
  ```
  The ID of this mission.

- **`IsComplete Property`**
  ```csharp
  public bool IsComplete { get; }
  ```
  Returns whether or not this mission is currently in a "completed" state, meaning that the mission can be completed from the GarrisonMissionFrame UI.

- **`LuaState Property`**
  ```csharp
  public int LuaState { get; }
  ```
  Returns the value that the Lua API C_Garrison.GetPartyMissionInfo returns for the current state of a mission. This is helpful for moving event-based mission completion to HB API.

- **`MaxFollowers Property`**
  ```csharp
  public int MaxFollowers { get; }
  ```
  The amount of followers this mission needs to be started.

- **`Mechanics Property`**
  ```csharp
  public IReadOnlyList&lt;GarrMechanic&gt; Mechanics { get; }
  ```
  A collection of GarrMechanics that this mission has. This is effectively a list of ability mechanics that the "enemies" have in this mission. This also includes the environment mechanic.

- **`MissionDuration Property`**
  ```csharp
  public TimeSpan MissionDuration { get; }
  ```
  The base duration of this mission.

- **`MissionRecord Property`**
  ```csharp
  public GarrMission MissionRecord { get; }
  ```
  The backing GarrMission record for this mission.

- **`MissionStartTime Property`**
  ```csharp
  public DateTime MissionStartTime { get; }
  ```
  Returns the time when this mission was actually started.

- **`MissionSuccessChance Property`**
  ```csharp
  public int MissionSuccessChance { get; }
  ```
  Returns the current mission success chance of this mission, with the given followers in the current "mission party".

- **`MissionTimeLeft Property`**
  ```csharp
  public TimeSpan MissionTimeLeft { get; }
  ```
  The current time remaining on this mission, if it is actually in progress. Zero otherwise.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  The name of this mission.

- **`State Property`**
  ```csharp
  public MissionState State { get; }
  ```
  The current state of this mission.

#### GarrisonMission Methods

- **`GetEncountersMechanics Method`**
  ```csharp
  public IReadOnlyList&lt;uint&gt; GetEncountersMechanics()
  ```
  Returns the IDs of any mechanics from the encounters in this mission.
  - **Returns:** See Also

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a string that represents the current object.
  - **Returns:** ReferenceGarrisonMission Class

---

### GarrisonMission.GarrisonMissionFollower Class

**Inheritance:** System.Object → Styx.WoWInternals.Garrison.GarrisonMission.GarrisonMissionFollower

```csharp
public class GarrisonMissionFollower
```

The armor level of this follower.

#### GarrisonMissionFollower Properties

- **`ArmorLevel Property`**
  ```csharp
  public int ArmorLevel { get; }
  ```
  The armor level of this follower.

- **`Experience Property`**
  ```csharp
  public int Experience { get; }
  ```
  The current experience of this follower.

- **`FollowerRecord Property`**
  ```csharp
  public GarrFollower FollowerRecord { get; }
  ```
  The corresponding GarrFollower record for this follower.

- **`GarrFollowerId Property`**
  ```csharp
  public int GarrFollowerId { get; }
  ```
  The GarrFollower ID of this follower.

- **`Id Property`**
  ```csharp
  public long Id { get; }
  ```
  The "server" Id of this follower. This is used in most requests to Lua and the server when making follower changes.

- **`Level Property`**
  ```csharp
  public int Level { get; }
  ```
  The current level of this follower.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Returns the faction, and locale specific name of this follower.

- **`WeaponLevel Property`**
  ```csharp
  public int WeaponLevel { get; }
  ```
  The weapon level of this follower.

#### GarrisonMissionFollower Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a string that represents the current object.
  - **Returns:** ReferenceGarrisonMissionAddLanguageSpecificTextSet("LST5F0D319_2?cs=.|vb=.|cpp=::|nu=.|fs=.");GarrisonMissionFollower Class

---

### GarrisonMissionReward Class

**Inheritance:** System.Object → Styx.WoWInternals.Garrison.GarrisonMissionReward

```csharp
public class GarrisonMissionReward
```

Bonus ability identifier.

#### GarrisonMissionReward Properties

- **`BonusAbilityId Property`**
  ```csharp
  public int BonusAbilityId { get; }
  ```
  Bonus ability identifier.

- **`CurrencyId Property`**
  ```csharp
  public int CurrencyId { get; }
  ```
  The currency identifier.

- **`CurrencyType Property`**
  ```csharp
  public WoWCurrencyType CurrencyType { get; }
  ```
  The currency type.

- **`FollowerXP Property`**
  ```csharp
  public int FollowerXP { get; }
  ```
  Amount of follower xp.

- **`ItemId Property`**
  ```csharp
  public int ItemId { get; }
  ```
  The item identifier.

- **`Quantity Property`**
  ```csharp
  public int Quantity { get; }
  ```
  The quantity.

#### GarrisonMissionReward Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceGarrisonMissionReward Class

---

### GarrisonMissionRewardInfo Class

**Inheritance:** System.Object → Styx.WoWInternals.Garrison.GarrisonMissionRewardInfo

```csharp
public class GarrisonMissionRewardInfo
```

The overmax rewards.

#### GarrisonMissionRewardInfo Properties

- **`OvermaxRewards Property`**
  ```csharp
  public GarrisonMissionReward[] OvermaxRewards { get; }
  ```
  The overmax rewards.

- **`Rewards Property`**
  ```csharp
  public GarrisonMissionReward[] Rewards { get; }
  ```
  The rewards.

#### GarrisonMissionRewardInfo Methods

- **`FromId Method`**
  ```csharp
  public static GarrisonMissionRewardInfo FromId(
int missionId
)
  ```
  Gets a mission reward info object by the mission id.
  - *missionId*: Type: SystemAddLanguageSpecificTextSet("LST5965F1B8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The mission identifier.
  - **Returns:** ReferenceGarrisonMissionRewardInfo Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceGarrisonMissionRewardInfo Class

---

### GarrisonMissionRewardInfo.NativeGarrisonMissionInfo Structure

```csharp
public struct NativeGarrisonMissionInfo
```

Indicates whether this instance and a specified object are equal.

#### NativeGarrisonMissionInfo Fields

- **`dword2C Field`**
  ```csharp
  public int dword2C
  ```

- **`dword30 Field`**
  ```csharp
  public int dword30
  ```

- **`dword34 Field`**
  ```csharp
  public int dword34
  ```

- **`dword38 Field`**
  ```csharp
  public int dword38
  ```

- **`dword3C Field`**
  ```csharp
  public int dword3C
  ```

- **`Dword44 Field`**
  ```csharp
  public int Dword44
  ```

- **`Dword48 Field`**
  ```csharp
  public int Dword48
  ```

- **`Dword4C Field`**
  ```csharp
  public int Dword4C
  ```

- **`dword50 Field`**
  ```csharp
  public int dword50
  ```

- **`dword54 Field`**
  ```csharp
  public int dword54
  ```

- **`dword58 Field`**
  ```csharp
  public int dword58
  ```

- **`dword5C Field`**
  ```csharp
  public int dword5C
  ```

- **`dword60 Field`**
  ```csharp
  public int dword60
  ```

- **`dword64 Field`**
  ```csharp
  public int dword64
  ```

- **`dword68 Field`**
  ```csharp
  public int dword68
  ```

- **`dword6C Field`**
  ```csharp
  public int dword6C
  ```

- **`dword70 Field`**
  ```csharp
  public int dword70
  ```

- **`dword74 Field`**
  ```csharp
  public int dword74
  ```

- **`dword80 Field`**
  ```csharp
  public int dword80
  ```

- **`dword84 Field`**
  ```csharp
  public int dword84
  ```

- **`dword90 Field`**
  ```csharp
  public int dword90
  ```

- **`MissionId Field`**
  ```csharp
  public int MissionId
  ```

- **`MissionStatus Field`**
  ```csharp
  public int MissionStatus
  ```

- **`OvermaxRewards Field`**
  ```csharp
  public GarrisonMissionRewardInfo.NativeMissionRewardContainer OvermaxRewards
  ```

- **`Rewards Field`**
  ```csharp
  public GarrisonMissionRewardInfo.NativeMissionRewardContainer Rewards
  ```

---

### GarrisonMissionRewardInfo.NativeMissionRewardContainer Structure

```csharp
public struct NativeMissionRewardContainer
```

Indicates whether this instance and a specified object are equal.

#### NativeMissionRewardContainer Fields

- **`Count Field`**
  ```csharp
  public int Count
  ```

- **`RewardsPtr Field`**
  ```csharp
  public IntPtr RewardsPtr
  ```

---

### GarrisonMissionSimulator Class

**Inheritance:** System.Object → Styx.WoWInternals.Garrison.GarrisonMissionSimulator

```csharp
public class GarrisonMissionSimulator
```

A class that helps simulate garrison missions, given the mission, and followers that will be used on the mission.

#### GarrisonMissionSimulator Methods

- **`Simulate Method`**
  ```csharp
  public static MissionSimulatorResults Simulate(
GarrisonMission mission,
IEnumerable&lt;GarrisonFollower&gt; followers,
MissionSimulatorOptions options
)
  ```
  Simulates a garrison mission with the specified follower group, given the specified options.
  - *mission*: Type: Styx.WoWInternals.GarrisonAddLanguageSpecificTextSet("LSTFBFE4A4B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");GarrisonMissionThe mission to be simulated.
  - *followers*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTFBFE4A4B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTFBFE4A4B_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");GarrisonFollowerAddLanguageSpecificTextSet("LSTFBFE4A4B_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
                The group of followers to use in the mission being simulated. This must contain exactly enough
                followers to start the mission normally.
  - *options*: Type: Styx.WoWInternals.GarrisonAddLanguageSpecificTextSet("LSTFBFE4A4B_5?cs=.|vb=.|cpp=::|nu=.|fs=.");MissionSimulatorOptionsThe options to simulate.
  - **Returns:** ReferenceGarrisonMissionSimulator Class

---

### GarrisonPlot Class

**Inheritance:** System.Object → Styx.WoWInternals.Garrison.GarrisonPlot

```csharp
public class GarrisonPlot
```

Determines whether the specified object is equal to the current object.

#### GarrisonPlot Properties

- **`CurrentBuilding Property`**
  ```csharp
  public GarrisonBuilding CurrentBuilding { get; }
  ```

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; }
  ```

- **`MapOffset Property`**
  ```csharp
  public Vector2 MapOffset { get; }
  ```

- **`PlotInstanceId Property`**
  ```csharp
  public uint PlotInstanceId { get; }
  ```

- **`Rotation Property`**
  ```csharp
  public float Rotation { get; }
  ```

- **`Size Property`**
  ```csharp
  public int Size { get; }
  ```

#### GarrisonPlot Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a string that represents the current object.
  - **Returns:** ReferenceGarrisonPlot Class

---

### GarrisonShipmentInfo Class

**Inheritance:** System.Object → Styx.WoWInternals.Garrison.GarrisonShipmentInfo

```csharp
public class GarrisonShipmentInfo
```

Determines whether the specified object is equal to the current object.

#### GarrisonShipmentInfo Properties

- **`ContainerRecord Property`**
  ```csharp
  public CharShipmentContainer ContainerRecord { get; }
  ```

- **`LandingPageInfo Property`**
  ```csharp
  public LandingPageShipmentInfo LandingPageInfo { get; }
  ```

- **`ProducesItem Property`**
  ```csharp
  public int ProducesItem { get; }
  ```

- **`ReagentCounts Property`**
  ```csharp
  public IReadOnlyList&lt;uint&gt; ReagentCounts { get; }
  ```

- **`ReagentIds Property`**
  ```csharp
  public IReadOnlyList&lt;uint&gt; ReagentIds { get; }
  ```

- **`Record Property`**
  ```csharp
  public CharShipment Record { get; }
  ```

- **`ShipmentCapacity Property`**
  ```csharp
  public int ShipmentCapacity { get; }
  ```

- **`ShipmentsCreated Property`**
  ```csharp
  public int ShipmentsCreated { get; }
  ```

- **`Spell Property`**
  ```csharp
  public WoWSpell Spell { get; }
  ```

#### GarrisonShipmentInfo Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a string that represents the current object.
  - **Returns:** ReferenceGarrisonShipmentInfo Class

---

### LandingPageShipmentInfo Structure

```csharp
public struct LandingPageShipmentInfo
```

Indicates whether this instance and a specified object are equal.

#### LandingPageShipmentInfo Properties

- **`Building Property`**
  ```csharp
  public GarrisonBuilding Building { get; }
  ```

- **`CreationTime Property`**
  ```csharp
  public DateTime CreationTime { get; }
  ```

- **`Duration Property`**
  ```csharp
  public TimeSpan Duration { get; }
  ```

- **`ShipmentInfo Property`**
  ```csharp
  public GarrisonShipmentInfo ShipmentInfo { get; }
  ```

- **`ShipmentsCreated Property`**
  ```csharp
  public int ShipmentsCreated { get; }
  ```

- **`ShipmentsReady Property`**
  ```csharp
  public int ShipmentsReady { get; }
  ```

#### LandingPageShipmentInfo Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns the fully qualified type name of this instance.
  - **Returns:** ReferenceLandingPageShipmentInfo Structure

#### LandingPageShipmentInfo Fields

- **`CharShipmentContainerId Field`**
  ```csharp
  public readonly uint CharShipmentContainerId
  ```

- **`CharShipmentId Field`**
  ```csharp
  public readonly uint CharShipmentId
  ```

---

### MissionSimulatorOptions Class

**Inheritance:** System.Object → Styx.WoWInternals.Garrison.MissionSimulatorOptions

```csharp
public class MissionSimulatorOptions
```

Initializes a new instance of the Object class.

#### MissionSimulatorOptions Properties

- **`ClampSuccessTo100 Property`**
  ```csharp
  public bool ClampSuccessTo100 { get; set; }
  ```

- **`ComputeBuffsAndEnvironmentCounter Property`**
  ```csharp
  public bool ComputeBuffsAndEnvironmentCounter { get; set; }
  ```

- **`ComputeGoldBonusModifier Property`**
  ```csharp
  public bool ComputeGoldBonusModifier { get; set; }
  ```

- **`ComputeMaterialBonusModifier Property`**
  ```csharp
  public bool ComputeMaterialBonusModifier { get; set; }
  ```

- **`ComputeModifiedDuration Property`**
  ```csharp
  public bool ComputeModifiedDuration { get; set; }
  ```

- **`ComputeModifiedRewards Property`**
  ```csharp
  public bool ComputeModifiedRewards { get; set; }
  ```

- **`ComputeRewards Property`**
  ```csharp
  public bool ComputeRewards { get; set; }
  ```

- **`ComputeSuccess Property`**
  ```csharp
  public bool ComputeSuccess { get; set; }
  ```

- **`ComputeXpBonusModifier Property`**
  ```csharp
  public bool ComputeXpBonusModifier { get; set; }
  ```

#### MissionSimulatorOptions Fields

- **`All Field`**
  ```csharp
  public static readonly MissionSimulatorOptions All
  ```

---

### MissionSimulatorResults Class

**Inheritance:** System.Object → Styx.WoWInternals.Garrison.MissionSimulatorResults

```csharp
public class MissionSimulatorResults
```

Initializes a new instance of the MissionSimulatorResults class

#### MissionSimulatorResults Properties

- **`BuffIds Property`**
  ```csharp
  public uint[] BuffIds { get; }
  ```
  The IDs of the buffs the group will have for this mission.

- **`EnvironmentCountered Property`**
  ```csharp
  public bool EnvironmentCountered { get; }
  ```
  Whether or not the environment has been countered with the follower group.

- **`ExperienceBonusModifier Property`**
  ```csharp
  public float ExperienceBonusModifier { get; }
  ```
  An experience modifier gained from this mission.

- **`Followers Property`**
  ```csharp
  public IEnumerable&lt;GarrisonFollower&gt; Followers { get; }
  ```

- **`GoldBonusModifier Property`**
  ```csharp
  public float GoldBonusModifier { get; }
  ```
  A bonus modifier on gold gained from this mission.

- **`MaterialBonusModifier Property`**
  ```csharp
  public float MaterialBonusModifier { get; }
  ```
  A bonus modifier on materials gained from this mission.

- **`Mission Property`**
  ```csharp
  public GarrisonMission Mission { get; }
  ```

- **`ModifiedDuration Property`**
  ```csharp
  public int ModifiedDuration { get; }
  ```
  The modified duration of this mission.

- **`SuccessChance Property`**
  ```csharp
  public float SuccessChance { get; }
  ```
  The chance of success for this mission.

---

### MissionState Enumeration

```csharp
public enum MissionState
```

---

### OwnedBuildingInfo Structure

```csharp
public struct OwnedBuildingInfo
```

Indicates whether this instance and a specified object are equal.

#### OwnedBuildingInfo Properties

- **`BuildingRecord Property`**
  ```csharp
  public GarrBuilding BuildingRecord { get; }
  ```

- **`BuildTimeLeft Property`**
  ```csharp
  public TimeSpan BuildTimeLeft { get; }
  ```

- **`CanActivate Property`**
  ```csharp
  public bool CanActivate { get; }
  ```

- **`IsBuilding Property`**
  ```csharp
  public bool IsBuilding { get; }
  ```

- **`IsFinalized Property`**
  ```csharp
  public bool IsFinalized { get; }
  ```

- **`Type Property`**
  ```csharp
  public GarrisonBuildingType Type { get; }
  ```

#### OwnedBuildingInfo Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns the fully qualified type name of this instance.
  - **Returns:** ReferenceOwnedBuildingInfo Structure

#### OwnedBuildingInfo Fields

- **`CurrentSpec Field`**
  ```csharp
  public int CurrentSpec
  ```

- **`GarrBuildingId Field`**
  ```csharp
  public uint GarrBuildingId
  ```

- **`Id Field`**
  ```csharp
  public int Id
  ```

- **`PlotInstanceId Field`**
  ```csharp
  public uint PlotInstanceId
  ```

- **`SpecCooldownStart Field`**
  ```csharp
  public int SpecCooldownStart
  ```

- **`TimeStart Field`**
  ```csharp
  public uint TimeStart
  ```

---
