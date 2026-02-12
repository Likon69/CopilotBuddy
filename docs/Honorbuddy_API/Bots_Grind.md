# Bots.Grind

Contains the LevelBot class, supplied for backwards compatibility.

## Contents

- [BehaviorFlags Enumeration](#behaviorflags-enumeration)
- [BehaviorFlagsExtensions Class](#behaviorflagsextensions-class)
- [LevelBot Class](#levelbot-class)

---

### BehaviorFlags Enumeration

```csharp
[FlagsAttribute]
public enum BehaviorFlags
```

Bitfield of flags for specifying BehaviorFlags.

---

### BehaviorFlagsExtensions Class

**Inheritance:** System.Object → Bots.Grind.BehaviorFlagsExtensions

```csharp
public static class BehaviorFlagsExtensions
```

#### BehaviorFlagsExtensions Methods

- **`HasAll Method`**
  ```csharp
  public static bool HasAll(
this BehaviorFlags flags,
BehaviorFlags flag
)
  ```
  - *flags*: Type: Bots.GrindAddLanguageSpecificTextSet("LST97AF4D88_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BehaviorFlags
  - *flag*: Type: Bots.GrindAddLanguageSpecificTextSet("LST97AF4D88_2?cs=.|vb=.|cpp=::|nu=.|fs=.");BehaviorFlags
  - **Returns:** ReferenceBehaviorFlagsExtensions Class

- **`HasAny Method`**
  ```csharp
  public static bool HasAny(
this BehaviorFlags flags,
BehaviorFlags flag
)
  ```
  - *flags*: Type: Bots.GrindAddLanguageSpecificTextSet("LSTA69DD0FD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BehaviorFlags
  - *flag*: Type: Bots.GrindAddLanguageSpecificTextSet("LSTA69DD0FD_2?cs=.|vb=.|cpp=::|nu=.|fs=.");BehaviorFlags
  - **Returns:** ReferenceBehaviorFlagsExtensions Class

---

### LevelBot Class

**Inheritance:** System.Object → Styx.CommonBot.BotBase → Bots.Grind.LevelBot

```csharp
public class LevelBot : BotBase
```

The level bot.

#### LevelBot Properties

- **`BehaviorFlags Property`**
  ```csharp
  public static BehaviorFlags BehaviorFlags { get; set; }
  ```
  Gets or sets the behavior flags.

- **`ConfigurationForm Property`**
  ```csharp
  public override Form ConfigurationForm { get; }
  ```
  Gets the configuration form.

- **`IsPrimaryType Property`**
  ```csharp
  public override bool IsPrimaryType { get; }
  ```
  Gets a value indicating whether this object is primary bot type. These will be used
            by default in mixed-mode.

- **`Name Property`**
  ```csharp
  public override string Name { get; }
  ```
  Gets the name.

- **`NeedToFindTarget Property`**
  ```csharp
  public static bool NeedToFindTarget { get; }
  ```
  Gets a value indicating whether [need to find target].

- **`PulseFlags Property`**
  ```csharp
  public override PulseFlags PulseFlags { get; }
  ```
  Gets the pulse flags.

- **`RequirementsMet Property`**
  ```csharp
  public override bool RequirementsMet { get; }
  ```
  Gets a value indicating whether the requirements met for this bot to be run.
            Secondary type bots MUST implement this member, for the bot to be used when needed.

- **`RequiresProfile Property`**
  ```csharp
  public override bool RequiresProfile { get; }
  ```

- **`RequiresProfileScope Property`**
  ```csharp
  public override bool RequiresProfileScope { get; }
  ```

- **`Root Property`**
  ```csharp
  public override Composite Root { get; }
  ```
  Gets the root.

- **`ShouldUseSpiritHealer Property`**
  ```csharp
  [ObsoleteAttribute("Override death behavior instead")]
public static bool ShouldUseSpiritHealer { get; set; }
  ```
  Gets a value indicating whether we should use the spirit healer.

#### LevelBot Methods

- **`CreateCombatBehavior Method`**
  ```csharp
  public static Composite CreateCombatBehavior()
  ```
  Creates the behavior used in combat.
  - **Returns:** ReferenceLevelBot Class

- **`CreateDeathBehavior Method`**
  ```csharp
  public static Composite CreateDeathBehavior()
  ```
  - **Returns:** ReferenceLevelBot Class

- **`CreateLootBehavior Method`**
  ```csharp
  public static Composite CreateLootBehavior()
  ```
  Creates the behavior used for looting and moving to a lootable object.
  - **Returns:** ReferenceLevelBot Class

- **`CreateVendorBehavior Method`**
  ```csharp
  public static PrioritySelector CreateVendorBehavior()
  ```
  Creates the behavior used for the vendor runs.
  - **Returns:** ReferenceLevelBot Class

- **`FindTarget Method`**
  ```csharp
  public static Task&lt;bool&gt; FindTarget(
NavType navType
)
  ```
  Finds the target.
  - *navType*: Type: StyxAddLanguageSpecificTextSet("LST34728AAC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");NavTypeType of the nav.
  - **Returns:** See Also

- **`IsTooNearBlackspot Method`**
  ```csharp
  public static bool IsTooNearBlackspot(
IEnumerable&lt;Blackspot&gt; blackspots,
Vector3 point
)
  ```
  Query if 'blackspots' is too near blackspot.
  - *blackspots*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST5E342C3E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST5E342C3E_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BlackspotAddLanguageSpecificTextSet("LST5E342C3E_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The blackspots.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LST5E342C3E_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3     The point.
  - **Returns:** ReferenceLevelBot Class

- **`LevelbotIncludeLootsFilter Method`**
  ```csharp
  public static void LevelbotIncludeLootsFilter(
List&lt;WoWObject&gt; incomingObjects,
HashSet&lt;WoWObject&gt; outgoingObjects
)
  ```
  Levelbot include loots filter.
  - *incomingObjects*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTF69400ED_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LSTF69400ED_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LSTF69400ED_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The incoming objects.
  - *outgoingObjects*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTF69400ED_4?cs=.|vb=.|cpp=::|nu=.|fs=.");HashSetAddLanguageSpecificTextSet("LSTF69400ED_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LSTF69400ED_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The outgoing objects.

- **`LevelBotIncludeTargetsFilter Method`**
  ```csharp
  public static void LevelBotIncludeTargetsFilter(
List&lt;WoWObject&gt; incomingUnits,
HashSet&lt;WoWObject&gt; outgoingUnits
)
  ```
  Level bottom include targets filter.
  - *incomingUnits*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST69774CD6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST69774CD6_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST69774CD6_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The incoming units.
  - *outgoingUnits*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST69774CD6_4?cs=.|vb=.|cpp=::|nu=.|fs=.");HashSetAddLanguageSpecificTextSet("LST69774CD6_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST69774CD6_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The outgoing units.

- **`MainVendorLogic Method`**
  ```csharp
  public static Task&lt;bool&gt; MainVendorLogic()
  ```
  - **Returns:** See Also

- **`MountVendor Method`**
  ```csharp
  public static Task&lt;bool&gt; MountVendor(
PoiType poiType
)
  ```
  Mounts a vendor mount and sets the POI to the vendor unit on the mount.
            Returns true if we mounted and set the POI.
            Otherwise returns false if we are already mounted with the POI set.
  - *poiType*: Type: Styx.CommonBot.POIAddLanguageSpecificTextSet("LSTBA8AA944_1?cs=.|vb=.|cpp=::|nu=.|fs=.");PoiType
  - **Returns:** See Also

- **`MoveToHotspot Method`**
  ```csharp
  public static Task&lt;bool&gt; MoveToHotspot(
NavType navType
)
  ```
  Moves to hotspot.
  - *navType*: Type: StyxAddLanguageSpecificTextSet("LSTAE1FE513_1?cs=.|vb=.|cpp=::|nu=.|fs=.");NavType
  - **Returns:** See Also

- **`MoveToKillPoi Method`**
  ```csharp
  public static Task&lt;bool&gt; MoveToKillPoi()
  ```
  - **Returns:** See Also

- **`Pulse Method`**
  ```csharp
  public override void Pulse()
  ```
  Pulses this object.

- **`Roam Method`**
  ```csharp
  public static Task&lt;bool&gt; Roam(
Nullable&lt;NavType&gt; navType = null
)
  ```
  Creates the behavior used for roaming around hotspots.
  - **Returns:** See Also

- **`Start Method`**
  ```csharp
  public override void Start()
  ```
  Starts this object.

- **`Stop Method`**
  ```csharp
  public override void Stop()
  ```
  Stops this object.

---
