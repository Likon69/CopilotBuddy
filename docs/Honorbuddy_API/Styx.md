# Styx

Contains Honorbuddy's WoW API and also bot related classes.

## Contents

- [AreaTriggerFlags Enumeration](#areatriggerflags-enumeration)
- [AreaTriggerShapeType Enumeration](#areatriggershapetype-enumeration)
- [BuildType Enumeration](#buildtype-enumeration)
- [CantCompileException Class](#cantcompileexception-class)
- [DifficultyColor Enumeration](#difficultycolor-enumeration)
- [EmoteState Enumeration](#emotestate-enumeration)
- [FactionId Enumeration](#factionid-enumeration)
- [GameError Enumeration](#gameerror-enumeration)
- [GameObjectDataSlot Enumeration](#gameobjectdataslot-enumeration)
- [GameState Enumeration](#gamestate-enumeration)
- [GeoRestriction Class](#georestriction-class)
- [GraphicsApi Enumeration](#graphicsapi-enumeration)
- [Guard Class](#guard-class)
- [HonorbuddyUnableToStartException Class](#honorbuddyunabletostartexception-class)
- [InvalidExecutorException Class](#invalidexecutorexception-class)
- [InvalidObjectPointerException Class](#invalidobjectpointerexception-class)
- [InventoryType Enumeration](#inventorytype-enumeration)
- [LfgCategory Enumeration](#lfgcategory-enumeration)
- [LfgState Enumeration](#lfgstate-enumeration)
- [MirrorTimerType Enumeration](#mirrortimertype-enumeration)
- [NavType Enumeration](#navtype-enumeration)
- [Pulsator Class](#pulsator-class)
- [PvPState Enumeration](#pvpstate-enumeration)
- [QuestGiverStatus Enumeration](#questgiverstatus-enumeration)
- [ShapeshiftForm Enumeration](#shapeshiftform-enumeration)
- [SheathType Enumeration](#sheathtype-enumeration)
- [SkillLine Enumeration](#skillline-enumeration)
- [SpellAttributes Enumeration](#spellattributes-enumeration)
- [SpellAttributesEx Enumeration](#spellattributesex-enumeration)
- [SpellAttributesEx2 Enumeration](#spellattributesex2-enumeration)
- [SpellAttributesEx3 Enumeration](#spellattributesex3-enumeration)
- [SpellAttributesEx4 Enumeration](#spellattributesex4-enumeration)
- [SpellAttributesEx5 Enumeration](#spellattributesex5-enumeration)
- [SpellAttributesEx6 Enumeration](#spellattributesex6-enumeration)
- [SpellAttributesEx7 Enumeration](#spellattributesex7-enumeration)
- [SpellAttributesEx8 Enumeration](#spellattributesex8-enumeration)
- [StatType Enumeration](#stattype-enumeration)
- [StyxWoW Class](#styxwow-class)
- [ThreatStatus Enumeration](#threatstatus-enumeration)
- [UnitNPCFlags Enumeration](#unitnpcflags-enumeration)
- [UserException Class](#userexception-class)
- [WoWBagSlot Enumeration](#wowbagslot-enumeration)
- [WoWClass Enumeration](#wowclass-enumeration)
- [WoWCreatureSkinType Enumeration](#wowcreatureskintype-enumeration)
- [WoWCreatureType Enumeration](#wowcreaturetype-enumeration)
- [WoWCursorType Enumeration](#wowcursortype-enumeration)
- [WoWEquipSlot Enumeration](#wowequipslot-enumeration)
- [WoWFactionGroup Enumeration](#wowfactiongroup-enumeration)
- [WoWGameObjectState Enumeration](#wowgameobjectstate-enumeration)
- [WoWGameObjectType Enumeration](#wowgameobjecttype-enumeration)
- [WoWGender Enumeration](#wowgender-enumeration)
- [WoWInteractType Enumeration](#wowinteracttype-enumeration)
- [WoWInventorySlot Enumeration](#wowinventoryslot-enumeration)
- [WoWItemArmorClass Enumeration](#wowitemarmorclass-enumeration)
- [WoWItemBattlePetsClass Enumeration](#wowitembattlepetsclass-enumeration)
- [WoWItemBondType Enumeration](#wowitembondtype-enumeration)
- [WoWItemClass Enumeration](#wowitemclass-enumeration)
- [WoWItemConsumableClass Enumeration](#wowitemconsumableclass-enumeration)
- [WoWItemContainerClass Enumeration](#wowitemcontainerclass-enumeration)
- [WoWItemEnhancementClass Enumeration](#wowitemenhancementclass-enumeration)
- [WoWItemGemClass Enumeration](#wowitemgemclass-enumeration)
- [WoWItemGlyphClass Enumeration](#wowitemglyphclass-enumeration)
- [WoWItemKeyClass Enumeration](#wowitemkeyclass-enumeration)
- [WoWItemMiscClass Enumeration](#wowitemmiscclass-enumeration)
- [WoWItemProjectileClass Enumeration](#wowitemprojectileclass-enumeration)
- [WoWItemQuality Enumeration](#wowitemquality-enumeration)
- [WoWItemQuestClass Enumeration](#wowitemquestclass-enumeration)
- [WoWItemQuiverClass Enumeration](#wowitemquiverclass-enumeration)
- [WoWItemReagentClass Enumeration](#wowitemreagentclass-enumeration)
- [WoWItemRecipeClass Enumeration](#wowitemrecipeclass-enumeration)
- [WoWItemStatType Enumeration](#wowitemstattype-enumeration)
- [WoWItemTokenClass Enumeration](#wowitemtokenclass-enumeration)
- [WoWItemTradeGoodsClass Enumeration](#wowitemtradegoodsclass-enumeration)
- [WoWItemWeaponClass Enumeration](#wowitemweaponclass-enumeration)
- [WoWObjectType Enumeration](#wowobjecttype-enumeration)
- [WoWObjectTypeFlag Enumeration](#wowobjecttypeflag-enumeration)
- [WoWPoint Structure](#wowpoint-structure)
- [WoWPowerType Enumeration](#wowpowertype-enumeration)
- [WoWQuestType Enumeration](#wowquesttype-enumeration)
- [WoWRace Enumeration](#wowrace-enumeration)
- [WoWSocketColor Enumeration](#wowsocketcolor-enumeration)
- [WoWSpec Enumeration](#wowspec-enumeration)
- [WoWStateFlag Enumeration](#wowstateflag-enumeration)
- [WoWUnitClassificationType Enumeration](#wowunitclassificationtype-enumeration)
- [WoWUnitReaction Enumeration](#wowunitreaction-enumeration)

---

### AreaTriggerFlags Enumeration

```csharp
[FlagsAttribute]
public enum AreaTriggerFlags
```

---

### AreaTriggerShapeType Enumeration

```csharp
public enum AreaTriggerShapeType
```

---

### BuildType Enumeration

```csharp
public enum BuildType
```

Values that represent BuildType.

---

### CantCompileException Class

**Inheritance:** System.Object → System.Exception → System.ApplicationException → Styx.CantCompileException

```csharp
public class CantCompileException : ApplicationException
```

Exception for signalling cant compile errors.

---

### DifficultyColor Enumeration

```csharp
public enum DifficultyColor
```

Values that represent DifficultyColor.

---

### EmoteState Enumeration

```csharp
public enum EmoteState
```

Values that represent EmoteState.

---

### FactionId Enumeration

```csharp
public enum FactionId
```

Values that represent FactionId.

---

### GameError Enumeration

```csharp
public enum GameError
```

Values that represent GameError.

---

### GameObjectDataSlot Enumeration

```csharp
public enum GameObjectDataSlot
```

Values that represent GameObjectDataSlot.

---

### GameState Enumeration

```csharp
public enum GameState
```

Values that represent GameState.

---

### GeoRestriction Class

**Inheritance:** System.Object → Styx.GeoRestriction

```csharp
public static class GeoRestriction
```

Returns whether or not this build of the bot is geo-restricted.

#### GeoRestriction Properties

- **`IsRestricted Property`**
  ```csharp
  public static bool IsRestricted { get; }
  ```
  Returns whether or not this build of the bot is geo-restricted.

---

### GraphicsApi Enumeration

```csharp
public enum GraphicsApi
```

Values that represent GraphicsApi.

---

### Guard Class

**Inheritance:** System.Object → Styx.Guard

```csharp
public static class Guard
```

Guard.

---

### HonorbuddyUnableToStartException Class

**Inheritance:** System.Object → System.Exception → Styx.UserException → Styx.HonorbuddyUnableToStartException

```csharp
[SerializableAttribute]
public class HonorbuddyUnableToStartException : UserException
```

Exception for signalling honorbuddy unable to start errors.

#### HonorbuddyUnableToStartException Constructor

- **`HonorbuddyUnableToStartException Constructor`**
  ```csharp
  public HonorbuddyUnableToStartException()
  ```
  Default constructor.

- **`HonorbuddyUnableToStartException Constructor (String)`**
  ```csharp
  public HonorbuddyUnableToStartException(
string message
)
  ```
  Constructor.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTBD39AF84_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.

- **`HonorbuddyUnableToStartException Constructor (SerializationInfo, StreamingContext)`**
  ```csharp
  protected HonorbuddyUnableToStartException(
SerializationInfo info,
StreamingContext context
)
  ```
  Initializes a new instance of the HonorbuddyUnableToStartException class
  - *info*: Type: System.Runtime.SerializationAddLanguageSpecificTextSet("LST609762A5_0?cs=.|vb=.|cpp=::|nu=.|fs=.");SerializationInfo
  - *context*: Type: System.Runtime.SerializationAddLanguageSpecificTextSet("LST609762A5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StreamingContext

- **`HonorbuddyUnableToStartException Constructor (String, Exception)`**
  ```csharp
  public HonorbuddyUnableToStartException(
string message,
Exception inner
)
  ```
  Constructor.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTF7ECA0DC_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.
  - *inner*: Type: SystemAddLanguageSpecificTextSet("LSTF7ECA0DC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Exception  The inner.

#### HonorbuddyUnableToStartException Properties

- **`HelpLink Property`**
  ```csharp
  public override string HelpLink { get; }
  ```
  Gets or sets a link to the help file associated with this exception.

---

### InvalidExecutorException Class

**Inheritance:** System.Object → System.Exception → Styx.InvalidExecutorException

```csharp
[SerializableAttribute]
public class InvalidExecutorException : Exception
```

Exception for signalling invalid executor errors.

#### InvalidExecutorException Constructor

- **`InvalidExecutorException Constructor`**
  ```csharp
  public InvalidExecutorException()
  ```
  For guidelines regarding the creation of new exception types, see
            msdn.microsoft.com/library/default.asp?url=/library/en-
            us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
             and
            msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp.

- **`InvalidExecutorException Constructor (String)`**
  ```csharp
  public InvalidExecutorException(
string message
)
  ```
  Constructor.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTA9AA6E14_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.

- **`InvalidExecutorException Constructor (SerializationInfo, StreamingContext)`**
  ```csharp
  protected InvalidExecutorException(
SerializationInfo info,
StreamingContext context
)
  ```
  Initializes a new instance of the InvalidExecutorException class
  - *info*: Type: System.Runtime.SerializationAddLanguageSpecificTextSet("LST63D264B7_0?cs=.|vb=.|cpp=::|nu=.|fs=.");SerializationInfo
  - *context*: Type: System.Runtime.SerializationAddLanguageSpecificTextSet("LST63D264B7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StreamingContext

- **`InvalidExecutorException Constructor (String, Exception)`**
  ```csharp
  public InvalidExecutorException(
string message,
Exception inner
)
  ```
  Constructor.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST8439A574_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.
  - *inner*: Type: SystemAddLanguageSpecificTextSet("LST8439A574_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Exception  The inner.

---

### InvalidObjectPointerException Class

**Inheritance:** System.Object → System.Exception → Styx.InvalidObjectPointerException

```csharp
[SerializableAttribute]
public class InvalidObjectPointerException : Exception
```

Exception for signalling invalid object pointer errors.

#### InvalidObjectPointerException Constructor

- **`InvalidObjectPointerException Constructor`**
  ```csharp
  public InvalidObjectPointerException()
  ```
  For guidelines regarding the creation of new exception types, see
            msdn.microsoft.com/library/default.asp?url=/library/en-
            us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
             and
            msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp.

- **`InvalidObjectPointerException Constructor (String)`**
  ```csharp
  public InvalidObjectPointerException(
string message
)
  ```
  Constructor.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTE0CF1727_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.

- **`InvalidObjectPointerException Constructor (SerializationInfo, StreamingContext)`**
  ```csharp
  protected InvalidObjectPointerException(
SerializationInfo info,
StreamingContext context
)
  ```
  Initializes a new instance of the InvalidObjectPointerException class
  - *info*: Type: System.Runtime.SerializationAddLanguageSpecificTextSet("LST1E05983E_0?cs=.|vb=.|cpp=::|nu=.|fs=.");SerializationInfo
  - *context*: Type: System.Runtime.SerializationAddLanguageSpecificTextSet("LST1E05983E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StreamingContext

- **`InvalidObjectPointerException Constructor (String, Exception)`**
  ```csharp
  public InvalidObjectPointerException(
string message,
Exception inner
)
  ```
  Constructor.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTB07F9EC7_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.
  - *inner*: Type: SystemAddLanguageSpecificTextSet("LSTB07F9EC7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Exception  The inner.

---

### InventoryType Enumeration

```csharp
public enum InventoryType
```

Values that represent InventoryType.

---

### LfgCategory Enumeration

```csharp
public enum LfgCategory
```

Values that represent LfgCategory.

---

### LfgState Enumeration

```csharp
public enum LfgState
```

Values that represent LfgState.

---

### MirrorTimerType Enumeration

```csharp
public enum MirrorTimerType
```

Values that represent MirrorTimerType.

---

### NavType Enumeration

```csharp
public enum NavType
```

Values that represent NavType.

---

### Pulsator Class

**Inheritance:** System.Object → Styx.Pulsator

```csharp
public static class Pulsator
```

A pulsator.

#### Pulsator Methods

- **`Pulse Method`**
  ```csharp
  public static void Pulse(
PulseFlags flags
)
  ```
  Pulses the given flags.
  - *flags*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LST426EDDB3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");PulseFlagsThe flags.

---

### PvPState Enumeration

```csharp
[FlagsAttribute]
public enum PvPState
```

Used in UNIT_FIELD_BYTES_2, 2nd byte.

---

### QuestGiverStatus Enumeration

```csharp
public enum QuestGiverStatus
```

Values that represent QuestGiverStatus.

---

### ShapeshiftForm Enumeration

```csharp
public enum ShapeshiftForm
```

Values that represent ShapeshiftForm.

---

### SheathType Enumeration

```csharp
public enum SheathType
```

Used in UNIT_FIELD_BYTES_2, 1st byte.

---

### SkillLine Enumeration

```csharp
public enum SkillLine
```

Values that represent SkillLine.

---

### SpellAttributes Enumeration

```csharp
[FlagsAttribute]
public enum SpellAttributes
```

---

### SpellAttributesEx Enumeration

```csharp
[FlagsAttribute]
public enum SpellAttributesEx
```

---

### SpellAttributesEx2 Enumeration

```csharp
[FlagsAttribute]
public enum SpellAttributesEx2
```

---

### SpellAttributesEx3 Enumeration

```csharp
[FlagsAttribute]
public enum SpellAttributesEx3
```

---

### SpellAttributesEx4 Enumeration

```csharp
[FlagsAttribute]
public enum SpellAttributesEx4
```

---

### SpellAttributesEx5 Enumeration

```csharp
[FlagsAttribute]
public enum SpellAttributesEx5
```

---

### SpellAttributesEx6 Enumeration

```csharp
[FlagsAttribute]
public enum SpellAttributesEx6
```

---

### SpellAttributesEx7 Enumeration

```csharp
[FlagsAttribute]
public enum SpellAttributesEx7
```

---

### SpellAttributesEx8 Enumeration

```csharp
[FlagsAttribute]
public enum SpellAttributesEx8
```

---

### StatType Enumeration

```csharp
public enum StatType
```

---

### StyxWoW Class

**Inheritance:** System.Object → Styx.StyxWoW

```csharp
public static class StyxWoW
```

Represents the "entrance" to Honorbuddy's WoW API.

#### StyxWoW Properties

- **`AreaManager Property`**
  ```csharp
  public static AreaManager AreaManager { get; }
  ```
  Gets the manager for area.

- **`BuildType Property`**
  ```csharp
  public static BuildType BuildType { get; }
  ```
  Gets the type of this build.

- **`Cache Property`**
  ```csharp
  public static WoWCache Cache { get; }
  ```
  Gets the cache.

- **`Camera Property`**
  ```csharp
  public static WoWCamera Camera { get; }
  ```
  Gets the current game camera.

- **`Db Property`**
  ```csharp
  public static WoWDb Db { get; }
  ```
  Gets the database.

- **`ExecutorLogging Property`**
  ```csharp
  public static bool ExecutorLogging { get; }
  ```

- **`GameGraphicsApi Property`**
  ```csharp
  public static GraphicsApi GameGraphicsApi { get; }
  ```
  Gets the game graphics api.

- **`GameState Property`**
  ```csharp
  public static GameState GameState { get; }
  ```
  Gets the state of the game (Idling, zoning, etc).

- **`GameVersion Property`**
  ```csharp
  public static Version GameVersion { get; }
  ```
  Gets the game version.

- **`IsInGame Property`**
  ```csharp
  public static bool IsInGame { get; }
  ```
  Gets a value indicating whether this object is in game.

- **`IsInWorld Property`**
  ```csharp
  public static bool IsInWorld { get; }
  ```
  Returns whether the current character is logged in, and not zoning. (Loading screen).

- **`Landmarks Property`**
  ```csharp
  public static Landmarks Landmarks { get; }
  ```
  Gets the landmarks.

- **`LastHardwareAction Property`**
  ```csharp
  public static uint LastHardwareAction { get; }
  ```
  Gets the last hardware action.

- **`LastRedErrorMessage Property`**
  ```csharp
  public static string LastRedErrorMessage { get; }
  ```
  Returns the last red error message.

- **`Me Property`**
  ```csharp
  public static LocalPlayer Me { get; }
  ```
  Gets me every time.

- **`Memory Property`**
  ```csharp
  public static ExternalProcessMemory Memory { get; }
  ```
  Gets the Memory reader used in Styx.

- **`Overlay Property`**
  ```csharp
  public static OverlayManager Overlay { get; }
  ```

- **`Random Property`**
  ```csharp
  public static ThreadSafeRandom Random { get; }
  ```
  Gets a thread safe wrapper of the Random class, that can be used to generate random values.

- **`WorldScene Property`**
  ```csharp
  public static WorldScene WorldScene { get; }
  ```
  Gets the world scene.

- **`WoWClient Property`**
  ```csharp
  public static WoWClient WoWClient { get; }
  ```
  Gets the WoW client.

- **`WowExited Property`**
  ```csharp
  public static bool WowExited { get; }
  ```
  Gets a value that indicates whether the WoW instance this HB is attached to has exited.

#### StyxWoW Methods

- **`DisableExecutorLogging Method`**
  ```csharp
  public static void DisableExecutorLogging()
  ```

- **`EnableExecutorLogging Method`**
  ```csharp
  public static void EnableExecutorLogging()
  ```

- **`IsStoreProductLoaded Method`**
  ```csharp
  public static bool IsStoreProductLoaded(
string name
)
  ```
  Checks if a store product by the specified name is loaded.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LSTECEB03B6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name.
  - **Returns:** See Also

- **`ResetAfk Method`**
  ```csharp
  public static void ResetAfk()
  ```
  Resets the afk.

- **`SetSeparatedLogFolders Method`**
  ```csharp
  public static void SetSeparatedLogFolders(
bool separate
)
  ```
  Used to separate or unseparate log folders
  - *separate*: Type: SystemAddLanguageSpecificTextSet("LSTBCFE7760_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean

- **`ShowStoreProfileBrowserAndGetPath Method`**
  ```csharp
  public static string ShowStoreProfileBrowserAndGetPath(
Window ownerWindow = null,
Nullable&lt;Point&gt; topLeftPosition = null
)
  ```
  This will open a store profile browser window and return the path of the selected profile upon user interaction. 
            Returns null if no profile is selected.
  - **Returns:** ReferenceStyxWoW Class

- **`Sleep Method`**
  Sleeps the given duration in milliseconds.

- **`Sleep Method (Int32)`**
  ```csharp
  [ObsoleteAttribute("Use Coroutine.Sleep or Styx.TreeSharp.Sleep")]
public static void Sleep(
int duration
)
  ```
  Sleeps the given duration in milliseconds.
  - *duration*: Type: SystemAddLanguageSpecificTextSet("LSTB4B6D36F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The duration in milliseconds.

- **`Sleep Method (TimeSpan)`**
  ```csharp
  [ObsoleteAttribute("Use Coroutine.Sleep or Styx.TreeSharp.Sleep")]
public static void Sleep(
TimeSpan timeSpan
)
  ```
  Sleeps for the given time span.
  - *timeSpan*: Type: SystemAddLanguageSpecificTextSet("LST91DBEB80_1?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe time span.

- **`SleepForLagDuration Method`**
  ```csharp
  [ObsoleteAttribute("Use CommonCoroutines.SleepForLagDuration or CommonBehaviors.Actions.SleepForLagDuration")]
public static void SleepForLagDuration()
  ```
  Causes the current thread to sleep, for the amount of time it takes a packet to be
            sent to the server, and a possible update packet to be recieved. Will also sleep for an extra
            50ms to include 'processing time'.

---

### ThreatStatus Enumeration

```csharp
public enum ThreatStatus
```

Values that represent ThreatStatus.

---

### UnitNPCFlags Enumeration

```csharp
[FlagsAttribute]
public enum UnitNPCFlags
```

Bitfield of flags for specifying UnitNPCFlags.

---

### UserException Class

**Inheritance:** System.Object → System.Exception → Styx.UserException → Styx.HonorbuddyUnableToStartException

```csharp
[SerializableAttribute]
public class UserException : Exception
```

Represents an exception that should be logged to the user.

#### UserException Constructor

- **`UserException Constructor`**
  ```csharp
  public UserException()
  ```
  Default constructor.

- **`UserException Constructor (String)`**
  ```csharp
  public UserException(
string message
)
  ```
  Constructor.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST9A56B305_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.

- **`UserException Constructor (SerializationInfo, StreamingContext)`**
  ```csharp
  protected UserException(
SerializationInfo info,
StreamingContext context
)
  ```
  Initializes a new instance of the UserException class
  - *info*: Type: System.Runtime.SerializationAddLanguageSpecificTextSet("LSTA94C9C80_0?cs=.|vb=.|cpp=::|nu=.|fs=.");SerializationInfo
  - *context*: Type: System.Runtime.SerializationAddLanguageSpecificTextSet("LSTA94C9C80_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StreamingContext

- **`UserException Constructor (String, Exception)`**
  ```csharp
  public UserException(
string message,
Exception inner
)
  ```
  Constructor.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST98D9D83D_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.
  - *inner*: Type: SystemAddLanguageSpecificTextSet("LST98D9D83D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Exception  The inner.

#### UserException Properties

- **`HelpLink Property`**
  ```csharp
  public override string HelpLink { get; }
  ```
  Gets a link to the help file associated with this exception.

---

### WoWBagSlot Enumeration

```csharp
public enum WoWBagSlot
```

Values that represent WoWBagSlot.

---

### WoWClass Enumeration

```csharp
public enum WoWClass
```

Values that represent WoWClass.

---

### WoWCreatureSkinType Enumeration

```csharp
public enum WoWCreatureSkinType
```

Values that represent WoWCreatureSkinType.

---

### WoWCreatureType Enumeration

```csharp
public enum WoWCreatureType
```

Values that represent WoWCreatureType.

---

### WoWCursorType Enumeration

```csharp
public enum WoWCursorType
```

Values that represent WoWCursorType.

---

### WoWEquipSlot Enumeration

```csharp
public enum WoWEquipSlot
```

Values that represent WoWEquipSlot.

---

### WoWFactionGroup Enumeration

```csharp
public enum WoWFactionGroup
```

Values that represent WoWFactionGroup.

---

### WoWGameObjectState Enumeration

```csharp
[FlagsAttribute]
public enum WoWGameObjectState
```

Values that represent WoWGameObjectState.

---

### WoWGameObjectType Enumeration

```csharp
public enum WoWGameObjectType
```

Values that represent WoWGameObjectType.

---

### WoWGender Enumeration

```csharp
public enum WoWGender
```

Values that represent WoWGender.

---

### WoWInteractType Enumeration

```csharp
public enum WoWInteractType
```

Values that represent WoWInteractType.

---

### WoWInventorySlot Enumeration

```csharp
public enum WoWInventorySlot
```

Values that represent WoWInventorySlot.

---

### WoWItemArmorClass Enumeration

```csharp
public enum WoWItemArmorClass
```

---

### WoWItemBattlePetsClass Enumeration

```csharp
public enum WoWItemBattlePetsClass
```

---

### WoWItemBondType Enumeration

```csharp
public enum WoWItemBondType
```

Values that represent WoWItemBondType.

---

### WoWItemClass Enumeration

```csharp
public enum WoWItemClass
```

---

### WoWItemConsumableClass Enumeration

```csharp
public enum WoWItemConsumableClass
```

---

### WoWItemContainerClass Enumeration

```csharp
public enum WoWItemContainerClass
```

---

### WoWItemEnhancementClass Enumeration

```csharp
public enum WoWItemEnhancementClass
```

---

### WoWItemGemClass Enumeration

```csharp
public enum WoWItemGemClass
```

---

### WoWItemGlyphClass Enumeration

```csharp
[ObsoleteAttribute("No longer used; will be removed in the future.")]
public enum WoWItemGlyphClass
```

---

### WoWItemKeyClass Enumeration

```csharp
public enum WoWItemKeyClass
```

---

### WoWItemMiscClass Enumeration

```csharp
public enum WoWItemMiscClass
```

---

### WoWItemProjectileClass Enumeration

```csharp
public enum WoWItemProjectileClass
```

---

### WoWItemQuality Enumeration

```csharp
public enum WoWItemQuality
```

Values that represent WoWItemQuality.

---

### WoWItemQuestClass Enumeration

```csharp
public enum WoWItemQuestClass
```

---

### WoWItemQuiverClass Enumeration

```csharp
public enum WoWItemQuiverClass
```

---

### WoWItemReagentClass Enumeration

```csharp
public enum WoWItemReagentClass
```

---

### WoWItemRecipeClass Enumeration

```csharp
public enum WoWItemRecipeClass
```

---

### WoWItemStatType Enumeration

```csharp
public enum WoWItemStatType
```

Values that represent WoWItemStatType.

---

### WoWItemTokenClass Enumeration

```csharp
public enum WoWItemTokenClass
```

---

### WoWItemTradeGoodsClass Enumeration

```csharp
public enum WoWItemTradeGoodsClass
```

---

### WoWItemWeaponClass Enumeration

```csharp
public enum WoWItemWeaponClass
```

---

### WoWObjectType Enumeration

```csharp
public enum WoWObjectType
```

Values that represent WoWObjectType.

---

### WoWObjectTypeFlag Enumeration

```csharp
[FlagsAttribute]
public enum WoWObjectTypeFlag
```

Bitfield of flags for specifying WoWObjectTypeFlag.

---

### WoWPoint Structure

```csharp
[ObsoleteAttribute("Use System.Numerics.Vector3")]
public struct WoWPoint : IEquatable&lt;WoWPoint&gt;,
IFormattable, IRangeAble
```

A WoW point.

#### WoWPoint Constructor

- **`WoWPoint Constructor (Single)`**
  ```csharp
  public WoWPoint(
float value
)
  ```
  Constructor.
  - *value*: Type: SystemAddLanguageSpecificTextSet("LSTDD0B3EB_0?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe value.

- **`WoWPoint Constructor (Double, Double, Double)`**
  ```csharp
  public WoWPoint(
double x,
double y,
double z
)
  ```
  Constructor.
  - *x*: Type: SystemAddLanguageSpecificTextSet("LST50A97FA4_0?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe x coordinate.
  - *y*: Type: SystemAddLanguageSpecificTextSet("LST50A97FA4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe y coordinate.
  - *z*: Type: SystemAddLanguageSpecificTextSet("LST50A97FA4_2?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe z coordinate.

- **`WoWPoint Constructor (Single, Single, Single)`**
  ```csharp
  public WoWPoint(
float x,
float y,
float z
)
  ```
  Constructor.
  - *x*: Type: SystemAddLanguageSpecificTextSet("LST7342628B_0?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe x coordinate.
  - *y*: Type: SystemAddLanguageSpecificTextSet("LST7342628B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe y coordinate.
  - *z*: Type: SystemAddLanguageSpecificTextSet("LST7342628B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe z coordinate.

#### WoWPoint Properties

- **`ComponentSum Property`**
  ```csharp
  public float ComponentSum { get; }
  ```
  Gets the component sum.

- **`Length Property`**
  ```csharp
  public float Length { get; }
  ```
  Gets the length.

#### WoWPoint Methods

- **`Add Method`**
  ```csharp
  public WoWPoint Add(
float xOffset,
float yOffset,
float zOffset
)
  ```
  Adds xOffset.
  - *xOffset*: Type: SystemAddLanguageSpecificTextSet("LST7A6238D3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe offset.
  - *yOffset*: Type: SystemAddLanguageSpecificTextSet("LST7A6238D3_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe offset.
  - *zOffset*: Type: SystemAddLanguageSpecificTextSet("LST7A6238D3_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe offset.
  - **Returns:** ReferenceWoWPoint Structure

- **`Distance Method`**
  ```csharp
  public float Distance(
WoWPoint other
)
  ```
  Distances the given other.
  - *other*: Type: StyxAddLanguageSpecificTextSet("LSTC14772D3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe WoW point to compare to this object.
  - **Returns:** ReferenceWoWPoint Structure

- **`Distance2D Method`**
  ```csharp
  public float Distance2D(
WoWPoint other
)
  ```
  Distance 2 d.
  - *other*: Type: StyxAddLanguageSpecificTextSet("LST502AF02D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe WoW point to compare to this object.
  - **Returns:** ReferenceWoWPoint Structure

- **`Distance2DSqr Method`**
  ```csharp
  public float Distance2DSqr(
WoWPoint other
)
  ```
  Distance 2 d sqr.
  - *other*: Type: StyxAddLanguageSpecificTextSet("LSTAEFBCA2D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe WoW point to compare to this object.
  - **Returns:** ReferenceWoWPoint Structure

- **`DistanceSqr Method`**
  ```csharp
  public float DistanceSqr(
WoWPoint other
)
  ```
  Distance sqr.
  - *other*: Type: StyxAddLanguageSpecificTextSet("LST7967CE8F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe WoW point to compare to this object.
  - **Returns:** ReferenceWoWPoint Structure

- **`Equals Method`**
  Indicates whether this instance and a specified object are equal.

- **`Equals Method (Object)`**
  ```csharp
  public override bool Equals(
Object obj
)
  ```
  Indicates whether this instance and a specified object are equal.
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LSTB5D820B8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAnother object to compare to.
  - **Returns:** See Also

- **`Equals Method (WoWPoint)`**
  ```csharp
  public bool Equals(
WoWPoint other
)
  ```
  Tests if this WoWPoint is considered equal to another.
  - *other*: Type: StyxAddLanguageSpecificTextSet("LST9115579B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe WoW point to compare to this object.
  - **Returns:** See Also

- **`GetDirection Method`**
  ```csharp
  public static WoWPoint GetDirection(
WoWPoint from,
WoWPoint to
)
  ```
  Gets a direction.
  - *from*: Type: StyxAddLanguageSpecificTextSet("LSTFC924F5E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointSource for the.
  - *to*: Type: StyxAddLanguageSpecificTextSet("LSTFC924F5E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPoint  to.
  - **Returns:** ReferenceWoWPoint Structure

- **`GetDirectionTo Method`**
  ```csharp
  public WoWPoint GetDirectionTo(
WoWPoint to
)
  ```
  Gets direction to.
  - *to*: Type: StyxAddLanguageSpecificTextSet("LST1ECE95BC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointto.
  - **Returns:** ReferenceWoWPoint Structure

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  Returns the hash code for this instance.
  - **Returns:** ReferenceWoWPoint Structure

- **`GetRange Method`**
  ```csharp
  public Range GetRange()
  ```
  Gets the range.
  - **Returns:** See Also

- **`MakeNegative Method`**
  ```csharp
  public void MakeNegative()
  ```
  Makes the negative.

- **`MakePositive Method`**
  ```csharp
  public void MakePositive()
  ```
  Makes the positive.

- **`Negate Method`**
  ```csharp
  public void Negate()
  ```
  Flips the components.

- **`Normalize Method`**
  ```csharp
  public void Normalize()
  ```
  Normalizes this object.

- **`RayCast Method`**
  Ray cast.

- **`RayCast Method (Single, Single)`**
  ```csharp
  public WoWPoint RayCast(
float headingRadians,
float distance
)
  ```
  Ray cast.
  - *headingRadians*: Type: SystemAddLanguageSpecificTextSet("LSTA7CDA650_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe heading in radians.
  - *distance*: Type: SystemAddLanguageSpecificTextSet("LSTA7CDA650_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single      The distance.
  - **Returns:** ReferenceWoWPoint Structure

- **`RayCast Method (WoWPoint, Single, Single)`**
  ```csharp
  public static WoWPoint RayCast(
WoWPoint from,
float headingRadians,
float distance
)
  ```
  Ray cast.
  - *from*: Type: StyxAddLanguageSpecificTextSet("LST7A7C4A5B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPoint          Source for the.
  - *headingRadians*: Type: SystemAddLanguageSpecificTextSet("LST7A7C4A5B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe heading in radians.
  - *distance*: Type: SystemAddLanguageSpecificTextSet("LST7A7C4A5B_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single      The distance.
  - **Returns:** ReferenceWoWPoint Structure

- **`ToString Method`**
  Returns the fully qualified type name of this instance.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns the fully qualified type name of this instance.
  - **Returns:** ReferenceWoWPoint Structure

- **`ToString Method (String)`**
  ```csharp
  public string ToString(
string format
)
  ```
  Formats the value of the current instance using the specified format.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST5D8F20F3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringDescribes the format to use.
  - **Returns:** ReferenceWoWPoint Structure

- **`ToString Method (String, IFormatProvider)`**
  ```csharp
  public string ToString(
string format,
IFormatProvider formatProvider
)
  ```
  Formats the value of the current instance using the specified format.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST69208AFF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String        Describes the format to use.
  - *formatProvider*: Type: SystemAddLanguageSpecificTextSet("LST69208AFF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IFormatProviderThe format provider.
  - **Returns:** See Also

#### WoWPoint Operators and Type Conversions

- **`Addition Operator`**
  Addition operator.

- **`Addition Operator (WoWPoint, WoWPoint)`**
  ```csharp
  public static WoWPoint operator +(
WoWPoint pnt1,
WoWPoint pnt2
)
  ```
  Addition operator.
  - *pnt1*: Type: StyxAddLanguageSpecificTextSet("LSTB89408F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe first point.
  - *pnt2*: Type: StyxAddLanguageSpecificTextSet("LSTB89408F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe second point.
  - **Returns:** ReferenceWoWPoint Structure

- **`Addition Operator (WoWPoint, Single)`**
  ```csharp
  public static WoWPoint operator +(
WoWPoint pnt,
float val
)
  ```
  Addition operator.
  - *pnt*: Type: StyxAddLanguageSpecificTextSet("LST39BF5A61_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe point.
  - *val*: Type: SystemAddLanguageSpecificTextSet("LST39BF5A61_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe value.
  - **Returns:** ReferenceWoWPoint Structure

- **`Division Operator`**
  Division operator.

- **`Division Operator (WoWPoint, WoWPoint)`**
  ```csharp
  public static WoWPoint operator /(
WoWPoint pnt1,
WoWPoint pnt2
)
  ```
  Division operator.
  - *pnt1*: Type: StyxAddLanguageSpecificTextSet("LSTCDB8E2A4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe first point.
  - *pnt2*: Type: StyxAddLanguageSpecificTextSet("LSTCDB8E2A4_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe second point.
  - **Returns:** ReferenceWoWPoint Structure

- **`Division Operator (WoWPoint, Single)`**
  ```csharp
  public static WoWPoint operator /(
WoWPoint pnt,
float scalar
)
  ```
  Division operator.
  - *pnt*: Type: StyxAddLanguageSpecificTextSet("LSTAF5D791E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPoint   The point.
  - *scalar*: Type: SystemAddLanguageSpecificTextSet("LSTAF5D791E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe scalar.
  - **Returns:** ReferenceWoWPoint Structure

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
WoWPoint pnt1,
WoWPoint pnt2
)
  ```
  Equality operator.
  - *pnt1*: Type: StyxAddLanguageSpecificTextSet("LSTA85D89CB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe first point.
  - *pnt2*: Type: StyxAddLanguageSpecificTextSet("LSTA85D89CB_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe second point.
  - **Returns:** ReferenceWoWPoint Structure

- **`Implicit Conversion Operators`**
  WoWPoint casting operator.

- **`Implicit Conversion (Vector3 to WoWPoint)`**
  ```csharp
  public static implicit operator WoWPoint (
Vector3 pnt
)
  ```
  WoWPoint casting operator.
  - *pnt*: Type: System.NumericsAddLanguageSpecificTextSet("LSTB3F2DFD4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The point.
  - **Returns:** ReferenceWoWPoint Structure

- **`Implicit Conversion (WoWPoint to Vector3)`**
  ```csharp
  public static implicit operator Vector3 (
WoWPoint pnt
)
  ```
  Vector3 casting operator.
  - *pnt*: Type: StyxAddLanguageSpecificTextSet("LSTAB522924_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe point.
  - **Returns:** ReferenceWoWPoint Structure

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
WoWPoint pnt1,
WoWPoint pnt2
)
  ```
  Inequality operator.
  - *pnt1*: Type: StyxAddLanguageSpecificTextSet("LSTA7577D50_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe first point.
  - *pnt2*: Type: StyxAddLanguageSpecificTextSet("LSTA7577D50_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe second point.
  - **Returns:** ReferenceWoWPoint Structure

- **`Multiply Operator`**
  Multiplication operator.

- **`Multiply Operator (WoWPoint, WoWPoint)`**
  ```csharp
  public static WoWPoint operator *(
WoWPoint pnt1,
WoWPoint pnt2
)
  ```
  Multiplication operator.
  - *pnt1*: Type: StyxAddLanguageSpecificTextSet("LST26C1A02F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe first point.
  - *pnt2*: Type: StyxAddLanguageSpecificTextSet("LST26C1A02F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe second point.
  - **Returns:** ReferenceWoWPoint Structure

- **`Multiply Operator (WoWPoint, Single)`**
  ```csharp
  public static WoWPoint operator *(
WoWPoint pnt,
float scalar
)
  ```
  Multiplication operator.
  - *pnt*: Type: StyxAddLanguageSpecificTextSet("LSTF42F8DAD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPoint   The point.
  - *scalar*: Type: SystemAddLanguageSpecificTextSet("LSTF42F8DAD_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe scalar.
  - **Returns:** ReferenceWoWPoint Structure

- **`Subtraction Operator`**
  Subtraction operator.

- **`Subtraction Operator (WoWPoint, WoWPoint)`**
  ```csharp
  public static WoWPoint operator -(
WoWPoint pnt1,
WoWPoint pnt2
)
  ```
  Subtraction operator.
  - *pnt1*: Type: StyxAddLanguageSpecificTextSet("LSTDCE92A1F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe first point.
  - *pnt2*: Type: StyxAddLanguageSpecificTextSet("LSTDCE92A1F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe second point.
  - **Returns:** ReferenceWoWPoint Structure

- **`Subtraction Operator (WoWPoint, Single)`**
  ```csharp
  public static WoWPoint operator -(
WoWPoint pnt,
float val
)
  ```
  Subtraction operator.
  - *pnt*: Type: StyxAddLanguageSpecificTextSet("LSTC8B3DA95_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPointThe point.
  - *val*: Type: SystemAddLanguageSpecificTextSet("LSTC8B3DA95_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe value.
  - **Returns:** ReferenceWoWPoint Structure

#### WoWPoint Fields

- **`Center Field`**
  ```csharp
  public static readonly WoWPoint Center
  ```
  The center.

- **`Empty Field`**
  ```csharp
  public static readonly WoWPoint Empty
  ```
  The empty.

- **`MaxValue Field`**
  ```csharp
  public static readonly WoWPoint MaxValue
  ```
  The maximum value.

- **`MinValue Field`**
  ```csharp
  public static readonly WoWPoint MinValue
  ```
  The minimum value.

- **`X Field`**
  ```csharp
  public float X
  ```
  The X coordinate.

- **`XUnit Field`**
  ```csharp
  public static readonly WoWPoint XUnit
  ```
  The unit.

- **`Y Field`**
  ```csharp
  public float Y
  ```
  The Y coordinate.

- **`YUnit Field`**
  ```csharp
  public static readonly WoWPoint YUnit
  ```
  The unit.

- **`Z Field`**
  ```csharp
  public float Z
  ```
  The Z coordinate.

- **`Zero Field`**
  ```csharp
  public static readonly WoWPoint Zero
  ```
  The zero.

- **`ZUnit Field`**
  ```csharp
  public static readonly WoWPoint ZUnit
  ```
  The unit.

---

### WoWPowerType Enumeration

```csharp
public enum WoWPowerType
```

Values that represent WoWPowerType.

---

### WoWQuestType Enumeration

```csharp
public enum WoWQuestType
```

Values that represent WoWQuestType.

---

### WoWRace Enumeration

```csharp
public enum WoWRace
```

Values that represent WoWRace.

---

### WoWSocketColor Enumeration

```csharp
[FlagsAttribute]
[ObsoleteAttribute("This is no longer used and will be removed in the future")]
public enum WoWSocketColor
```

Bitfield of flags for specifying WoWSocketColor.

---

### WoWSpec Enumeration

```csharp
public enum WoWSpec
```

Values that represent WoWSpec.

---

### WoWStateFlag Enumeration

```csharp
[FlagsAttribute]
public enum WoWStateFlag
```

Bitfield of flags for specifying WoWStateFlag.

---

### WoWUnitClassificationType Enumeration

```csharp
public enum WoWUnitClassificationType
```

Values that represent WoWUnitClassificationType.

---

### WoWUnitReaction Enumeration

```csharp
public enum WoWUnitReaction
```

Values that represent WoWUnitReaction.

---
