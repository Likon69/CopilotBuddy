# Styx.WoWInternals

Contains the majority of the wrapped WoW API.

## Contents

- [AlteracValleyLandmark Class](#alteracvalleylandmark-class)
- [AlteracValleyLandmarkType Enumeration](#alteracvalleylandmarktype-enumeration)
- [ArathiBasinLandmark Class](#arathibasinlandmark-class)
- [ArathiBasinLandmarkType Enumeration](#arathibasinlandmarktype-enumeration)
- [AreaPoiLandmark Class](#areapoilandmark-class)
- [ArenaType Enumeration](#arenatype-enumeration)
- [BattleForGilneasLandmark Class](#battleforgilneaslandmark-class)
- [BattleForGilneasLandmarkType Enumeration](#battleforgilneaslandmarktype-enumeration)
- [BattlefieldWinner Enumeration](#battlefieldwinner-enumeration)
- [BattlegroundJoinError Enumeration](#battlegroundjoinerror-enumeration)
- [BattlegroundSide Enumeration](#battlegroundside-enumeration)
- [BattlegroundStatus Enumeration](#battlegroundstatus-enumeration)
- [BattlegroundType Enumeration](#battlegroundtype-enumeration)
- [Battlegrounds Class](#battlegrounds-class)
- [ClickToMoveInfo Class](#clicktomoveinfo-class)
- [DeepwindGorgeLandmark Class](#deepwindgorgelandmark-class)
- [DeepwindGorgeLandmarkType Enumeration](#deepwindgorgelandmarktype-enumeration)
- [EyeOfTheStormLandmark Class](#eyeofthestormlandmark-class)
- [EyeOfTheStormLandmarkType Enumeration](#eyeofthestormlandmarktype-enumeration)
- [GameInput Class](#gameinput-class)
- [InputMouseButton Enumeration](#inputmousebutton-enumeration)
- [IsleOfConquestLandmark Class](#isleofconquestlandmark-class)
- [IsleOfConquestLandmarkType Enumeration](#isleofconquestlandmarktype-enumeration)
- [ItemContext Enumeration](#itemcontext-enumeration)
- [LandmarkControlType Enumeration](#landmarkcontroltype-enumeration)
- [LandmarkType Enumeration](#landmarktype-enumeration)
- [Lua Class](#lua-class)
- [LuaEventArgs Class](#luaeventargs-class)
- [LuaEventHandlerDelegate Delegate](#luaeventhandlerdelegate-delegate)
- [LuaEvents Class](#luaevents-class)
- [LuaNode Class](#luanode-class)
- [LuaRunStatus Enumeration](#luarunstatus-enumeration)
- [LuaState Class](#luastate-class)
- [LuaTKey Class](#luatkey-class)
- [LuaTString Class](#luatstring-class)
- [LuaTValue Class](#luatvalue-class)
- [LuaTable Class](#luatable-class)
- [LuaType Enumeration](#luatype-enumeration)
- [LuaValue Class](#luavalue-class)
- [MoveFlags Enumeration](#moveflags-enumeration)
- [NativeLuaCommonHeader Structure](#nativeluacommonheader-structure)
- [NativeLuaNode Structure](#nativeluanode-structure)
- [NativeLuaTKey Structure](#nativeluatkey-structure)
- [NativeLuaTString Structure](#nativeluatstring-structure)
- [NativeLuaTValue Structure](#nativeluatvalue-structure)
- [NativeLuaTable Structure](#nativeluatable-structure)
- [NativeLuaValue Structure](#nativeluavalue-structure)
- [NativeObject Class](#nativeobject-class)
- [ObjectListUpdateFinishedDelegate Delegate](#objectlistupdatefinisheddelegate-delegate)
- [ObjectManager Class](#objectmanager-class)
- [PetStance Enumeration](#petstance-enumeration)
- [PlayerQuest Class](#playerquest-class)
- [Quest Class](#quest-class)
- [Quest.QuestObjective Structure](#quest.questobjective-structure)
- [Quest.QuestObjectiveType Enumeration](#quest.questobjectivetype-enumeration)
- [QuestLog Class](#questlog-class)
- [ResearchSiteLandmark Class](#researchsitelandmark-class)
- [SpellChargeInfo Class](#spellchargeinfo-class)
- [SpellCooldownInfo Class](#spellcooldowninfo-class)
- [SpellDetailedPowerCost Class](#spelldetailedpowercost-class)
- [StrandOfTheAncientsLandmark Class](#strandoftheancientslandmark-class)
- [StrandOfTheAncientsLandmarkType Enumeration](#strandoftheancientslandmarktype-enumeration)
- [TaxiMapNode Class](#taximapnode-class)
- [WoWApplyAuraType Enumeration](#wowapplyauratype-enumeration)
- [WoWAura Class](#wowaura-class)
- [WoWAuraCollection Class](#wowauracollection-class)
- [WoWBag Class](#wowbag-class)
- [WoWCamera Class](#wowcamera-class)
- [WoWCurrency Class](#wowcurrency-class)
- [WoWCurrencyType Enumeration](#wowcurrencytype-enumeration)
- [WoWDescriptorQuest Structure](#wowdescriptorquest-structure)
- [WoWDescriptorQuestFlags Enumeration](#wowdescriptorquestflags-enumeration)
- [WoWDispelType Enumeration](#wowdispeltype-enumeration)
- [WoWGroupInfo Class](#wowgroupinfo-class)
- [WoWGroupInfo.GroupFlags Enumeration](#wowgroupinfo.groupflags-enumeration)
- [WoWGroupInfo.GroupMemberInfo Structure](#wowgroupinfo.groupmemberinfo-structure)
- [WoWGroupInfo.WorldMarker Enumeration](#wowgroupinfo.worldmarker-enumeration)
- [WoWGuid Structure](#wowguid-structure)
- [WoWGuidType Enumeration](#wowguidtype-enumeration)
- [WoWLandMark Class](#wowlandmark-class)
- [WoWMissile Class](#wowmissile-class)
- [WoWMovement Class](#wowmovement-class)
- [WoWMovement.ClickToMoveType Enumeration](#wowmovement.clicktomovetype-enumeration)
- [WoWMovement.InputControl Structure](#wowmovement.inputcontrol-structure)
- [WoWMovement.MovementControl Structure](#wowmovement.movementcontrol-structure)
- [WoWMovement.MovementDirection Enumeration](#wowmovement.movementdirection-enumeration)
- [WoWMovementInfo Class](#wowmovementinfo-class)
- [WoWPaperDoll Class](#wowpaperdoll-class)
- [WoWPetBattleState Enumeration](#wowpetbattlestate-enumeration)
- [WoWPetControl Class](#wowpetcontrol-class)
- [WoWPetSpell Class](#wowpetspell-class)
- [WoWPetSpell.PetAction Enumeration](#wowpetspell.petaction-enumeration)
- [WoWPetSpell.PetSpellType Enumeration](#wowpetspell.petspelltype-enumeration)
- [WoWPetSpell.PetStance Enumeration](#wowpetspell.petstance-enumeration)
- [WoWPlayerInventory Class](#wowplayerinventory-class)
- [WoWQuestPOIInfo Structure](#wowquestpoiinfo-structure)
- [WoWQuestState Enumeration](#wowqueststate-enumeration)
- [WoWQuestStep Structure](#wowqueststep-structure)
- [WoWQuestStepsCollection Structure](#wowqueststepscollection-structure)
- [WoWSimpleMovementInfo Class](#wowsimplemovementinfo-class)
- [WoWSkill Class](#wowskill-class)
- [WoWSpell Class](#wowspell-class)
- [WoWSpell.SpellCastTimesEntry Structure](#wowspell.spellcasttimesentry-structure)
- [WoWSpell.SpellCastingRequirementsEntry Structure](#wowspell.spellcastingrequirementsentry-structure)
- [WoWSpell.SpellCategoriesEntry Structure](#wowspell.spellcategoriesentry-structure)
- [WoWSpell.SpellDurationEntry Structure](#wowspell.spelldurationentry-structure)
- [WoWSpell.SpellEntry Structure](#wowspell.spellentry-structure)
- [WoWSpell.SpellInterruptsEntry Structure](#wowspell.spellinterruptsentry-structure)
- [WoWSpell.SpellMiscRec Structure](#wowspell.spellmiscrec-structure)
- [WoWSpell.SpellRangeEntry Structure](#wowspell.spellrangeentry-structure)
- [WoWSpell.SpellReagentsEntry Structure](#wowspell.spellreagentsentry-structure)
- [WoWSpell.SpellShapeshiftEntry Structure](#wowspell.spellshapeshiftentry-structure)
- [WoWSpell.SpellTargetRestrictionsEntry Structure](#wowspell.spelltargetrestrictionsentry-structure)
- [WoWSpell.SpellTotemsEntry Structure](#wowspell.spelltotemsentry-structure)
- [WoWSpellEffectType Enumeration](#wowspelleffecttype-enumeration)
- [WoWSpellFocus Enumeration](#wowspellfocus-enumeration)
- [WoWSpellMechanic Enumeration](#wowspellmechanic-enumeration)
- [WoWSpellSchool Enumeration](#wowspellschool-enumeration)
- [WoWTaxi Class](#wowtaxi-class)
- [WoWTotem Enumeration](#wowtotem-enumeration)
- [WoWTotemExtensions Class](#wowtotemextensions-class)
- [WoWTotemInfo Class](#wowtoteminfo-class)
- [WoWTotemType Enumeration](#wowtotemtype-enumeration)
- [WoWVehicle Class](#wowvehicle-class)

---

### AlteracValleyLandmark Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWLandMark → Styx.WoWInternals.AreaPoiLandmark → Styx.WoWInternals.AlteracValleyLandmark

```csharp
public class AlteracValleyLandmark : AreaPoiLandmark
```

An alterac valley landmark.

#### AlteracValleyLandmark Properties

- **`ControlType Property`**
  ```csharp
  public LandmarkControlType ControlType { get; }
  ```
  Gets the type of the control.

- **`LandmarkType Property`**
  ```csharp
  public AlteracValleyLandmarkType LandmarkType { get; }
  ```
  Gets the type of the landmark.

---

### AlteracValleyLandmarkType Enumeration

```csharp
[FlagsAttribute]
public enum AlteracValleyLandmarkType
```

Bitfield of flags for specifying AlteracValleyLandmarkType.

---

### ArathiBasinLandmark Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWLandMark → Styx.WoWInternals.AreaPoiLandmark → Styx.WoWInternals.ArathiBasinLandmark

```csharp
public class ArathiBasinLandmark : AreaPoiLandmark
```

An arathi basin landmark.

#### ArathiBasinLandmark Properties

- **`ControlType Property`**
  ```csharp
  public LandmarkControlType ControlType { get; }
  ```
  Gets the type of the control.

- **`LandmarkType Property`**
  ```csharp
  public ArathiBasinLandmarkType LandmarkType { get; }
  ```
  Gets the type of the landmark.

---

### ArathiBasinLandmarkType Enumeration

```csharp
public enum ArathiBasinLandmarkType
```

Values that represent ArathiBasinLandmarkType.

---

### AreaPoiLandmark Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWLandMark → Styx.WoWInternals.AreaPoiLandmark → More.

```csharp
public class AreaPoiLandmark : WoWLandMark
```

An area poi landmark.

#### AreaPoiLandmark Properties

- **`AreaId Property`**
  ```csharp
  public override int AreaId { get; }
  ```
  Gets the identifier of the area.

- **`Description Property`**
  ```csharp
  public override string Description { get; }
  ```
  Gets the description.

- **`IsValid Property`**
  ```csharp
  public override bool IsValid { get; }
  ```
  Returns true if this landmark is a valid object.

- **`Location Property`**
  ```csharp
  public override Vector2 Location { get; }
  ```
  Gets the location.

- **`MapId Property`**
  ```csharp
  public override int MapId { get; }
  ```
  Gets the identifier of the map.

- **`Name Property`**
  ```csharp
  public override string Name { get; }
  ```
  Gets the name.

- **`NormalIcon Property`**
  ```csharp
  public override int NormalIcon { get; }
  ```
  Gets the normal icon.

- **`ShowInBattleMap Property`**
  ```csharp
  public override bool ShowInBattleMap { get; }
  ```
  Gets a value indicating whether the in battle map is shown.

- **`ShowInBg Property`**
  ```csharp
  public override bool ShowInBg { get; }
  ```
  Gets a value indicating whether the in background is shown.

- **`ShowInZone Property`**
  ```csharp
  public override bool ShowInZone { get; }
  ```
  Gets a value indicating whether the in zone is shown.

- **`TextureIndex Property`**
  ```csharp
  public override int TextureIndex { get; }
  ```
  Gets the zero-based index of the texture.

- **`WorldState Property`**
  ```csharp
  public override uint WorldState { get; }
  ```
  Gets the state of the world.

---

### ArenaType Enumeration

```csharp
public enum ArenaType
```

Values that represent ArenaType.

---

### BattleForGilneasLandmark Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWLandMark → Styx.WoWInternals.AreaPoiLandmark → Styx.WoWInternals.BattleForGilneasLandmark

```csharp
public class BattleForGilneasLandmark : AreaPoiLandmark
```

A battle for gilneas landmark.

#### BattleForGilneasLandmark Properties

- **`ControlType Property`**
  ```csharp
  public LandmarkControlType ControlType { get; }
  ```
  Gets the type of the control.

- **`LandmarkType Property`**
  ```csharp
  public BattleForGilneasLandmarkType LandmarkType { get; }
  ```
  Gets the type of the landmark.

---

### BattleForGilneasLandmarkType Enumeration

```csharp
public enum BattleForGilneasLandmarkType
```

Values that represent BattleForGilneasLandmarkType.

---

### BattlefieldWinner Enumeration

```csharp
public enum BattlefieldWinner
```

Values that represent BattlefieldWinner.

---

### BattlegroundJoinError Enumeration

```csharp
public enum BattlegroundJoinError
```

Values that represent BattlegroundJoinError.

---

### BattlegroundSide Enumeration

```csharp
public enum BattlegroundSide
```

Values that represent BattlegroundSide.

---

### BattlegroundStatus Enumeration

```csharp
public enum BattlegroundStatus
```

Values that represent BattlegroundStatus.

---

### BattlegroundType Enumeration

```csharp
public enum BattlegroundType
```

Values that represent BattlegroundType.

---

### Battlegrounds Class

**Inheritance:** System.Object → Styx.WoWInternals.Battlegrounds

```csharp
public static class Battlegrounds
```

The battlegrounds manager.

#### Battlegrounds Properties

- **`BattlefieldInstanceRunTime Property`**
  ```csharp
  public static TimeSpan BattlefieldInstanceRunTime { get; }
  ```
  Returns the time since the battleground started.

- **`BattlefieldStartTime Property`**
  ```csharp
  public static DateTime BattlefieldStartTime { get; }
  ```
  Gets the battlefield start time.

- **`BattlegroundStatuses Property`**
  ```csharp
  public static List&lt;BattlegroundStatus&gt; BattlegroundStatuses { get; }
  ```
  Returns a list of battleground statuses.

- **`Current Property`**
  ```csharp
  public static BattlegroundType Current { get; }
  ```
  Returns the current battleground the local player is inside.

- **`Finished Property`**
  ```csharp
  public static bool Finished { get; }
  ```
  Returns true if the battlefield has ended.

- **`IsInsideBattleground Property`**
  ```csharp
  public static bool IsInsideBattleground { get; }
  ```
  Returns true if you are inside a battleground.

- **`LandMarks Property`**
  ```csharp
  public static Landmarks LandMarks { get; }
  ```
  Returns a list of all visible landmarks.

- **`NumVehicles Property`**
  ```csharp
  public static int NumVehicles { get; }
  ```
  Gets the number of vehicles currently in this bg.

- **`VehicleGuids Property`**
  ```csharp
  public static IEnumerable&lt;WoWGuid&gt; VehicleGuids { get; }
  ```
  Gets the guids of all the vehicles in this bg.

- **`Vehicles Property`**
  ```csharp
  public static IEnumerable&lt;WoWUnit&gt; Vehicles { get; }
  ```
  Gets the vehicles of this battleground.

- **`WaitingForConfirmation Property`**
  ```csharp
  public static bool WaitingForConfirmation { get; }
  ```
  Gets a value indicating whether a BG queue has popped, and the game is waiting for
            confirmation to enter.

- **`Winner Property`**
  ```csharp
  public static BattlefieldWinner Winner { get; }
  ```
  Returns the battlefield winner.

#### Battlegrounds Methods

- **`AcceptBattlefieldPort Method`**
  ```csharp
  public static void AcceptBattlefieldPort(
int index,
bool accept
)
  ```
  Accepts or declines a battlefield port.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTA9A9B4D0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 The zero based index of the port.
  - *accept*: Type: SystemAddLanguageSpecificTextSet("LSTA9A9B4D0_2?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanIf true, will accept. Otherwise will decline.

- **`AcceptBattlegroundConfirmation Method`**
  ```csharp
  public static void AcceptBattlegroundConfirmation()
  ```
  Accept battleground confirmation.

- **`GetBattlefieldWinner Method`**
  ```csharp
  public static BattlefieldWinner GetBattlefieldWinner()
  ```
  Get the battlefields winner. Integer - Faction/team that has won the battlefield.
            Results are: nil if nobody has won, 0 for Horde and 1 for Alliance in a battleground, 0 for
            Green Team and 1 for Yellow in an arena.
  - **Returns:** ReferenceBattlegrounds Class

- **`GetBGIndexWithStatus Method`**
  ```csharp
  public static int GetBGIndexWithStatus(
BattlegroundStatus status
)
  ```
  Returns the first found index of the battlegournd with x status.
  - *status*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTB8F0634A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BattlegroundStatus.
  - **Returns:** ReferenceBattlegrounds Class

- **`GetCurrentBattleground Method`**
  ```csharp
  public static BattlegroundType GetCurrentBattleground()
  ```
  Returns the current battleground  you are in.
  - **Returns:** ReferenceBattlegrounds Class

- **`GetProfileName Method`**
  ```csharp
  public static string GetProfileName(
BattlegroundType type
)
  ```
  Returns the profile name for type
  - *type*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTEEF3B621_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BattlegroundTypethe battleground type.
  - **Returns:** ReferenceBattlegrounds Class

- **`GetQueuedBattlegroundInfo Method`**
  ```csharp
  public static QueuedBattlegroundInfo GetQueuedBattlegroundInfo(
uint index
)
  ```
  Returns detailed information on the battlegrounds in the que.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST6E5DE1D3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The index of the BG to get the info for. (Valid values are 0 and 1)
  - **Returns:** Exceptions

- **`GetQueuedBattlegroundWaitTime Method`**
  ```csharp
  public static TimeSpan GetQueuedBattlegroundWaitTime(
uint index
)
  ```
  Gets a queued battleground wait time in miliseconds, or 0 if the battleground is not
            queued.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST222CBC13_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32.
  - **Returns:** ReferenceBattlegrounds Class

- **`GetStatus Method`**
  ```csharp
  public static BattlegroundStatus GetStatus(
uint index
)
  ```
  Get's the current battleground status.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTAC8C2EBF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32.
  - **Returns:** ExceptionConditionArgumentOutOfRangeExceptionThrown when one or more arguments are outside the
            required range.

- **`IsQueuedForBattleground Method`**
  ```csharp
  public static bool IsQueuedForBattleground(
BattlegroundType type
)
  ```
  Query if 'type' is queued for battleground.
  - *type*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTD07F12E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BattlegroundTypeBattlegroundType ie. WSG EotS AB etc...
  - **Returns:** ReferenceBattlegrounds Class

- **`JoinBattlefield Method`**
  ```csharp
  public static void JoinBattlefield(
BattlegroundType type,
bool asGroup = false
)
  ```
  Join's the que for a battlefield.
  - *type*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST5CAEF00_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BattlegroundType   BattlegroundType ie. WSG EotS AB etc...

- **`LeaveBattlefield Method`**
  ```csharp
  public static void LeaveBattlefield()
  ```
  Leaves the current battlefield the player is inside, pre-2.0.1 this would only leave
            the battlefield if it had been won or lost this was changed in 2.0.1 to exit you from the
            battlefield regardless if it was finished or not and will give you deserter.

---

### ClickToMoveInfo Class

**Inheritance:** System.Object → Styx.WoWInternals.ClickToMoveInfo

```csharp
public class ClickToMoveInfo
```

Initializes a new instance of the ClickToMoveInfo class

#### ClickToMoveInfo Properties

- **`ClickPos Property`**
  ```csharp
  public Vector3 ClickPos { get; }
  ```

- **`InteractGuid Property`**
  ```csharp
  public WoWGuid InteractGuid { get; }
  ```

- **`IsClickMoving Property`**
  ```csharp
  public bool IsClickMoving { get; }
  ```
  Gets a value indicating whether this object is click moving.

- **`IsUsing Property`**
  ```csharp
  public bool IsUsing { get; }
  ```
  Gets a value indicating whether this object is using.

- **`Type Property`**
  ```csharp
  public WoWMovement.ClickToMoveType Type { get; }
  ```

---

### DeepwindGorgeLandmark Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWLandMark → Styx.WoWInternals.AreaPoiLandmark → Styx.WoWInternals.DeepwindGorgeLandmark

```csharp
public class DeepwindGorgeLandmark : AreaPoiLandmark
```

A deepwind gorge landmark.

#### DeepwindGorgeLandmark Properties

- **`ControlType Property`**
  ```csharp
  public LandmarkControlType ControlType { get; }
  ```
  Gets the type of the control.

- **`LandmarkType Property`**
  ```csharp
  public DeepwindGorgeLandmarkType LandmarkType { get; }
  ```
  Gets the type of the landmark.

---

### DeepwindGorgeLandmarkType Enumeration

```csharp
public enum DeepwindGorgeLandmarkType
```

Values that represent DeepwindGorgeLandmarkType.

---

### EyeOfTheStormLandmark Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWLandMark → Styx.WoWInternals.AreaPoiLandmark → Styx.WoWInternals.EyeOfTheStormLandmark

```csharp
public class EyeOfTheStormLandmark : AreaPoiLandmark
```

An eye of the storm landmark.

#### EyeOfTheStormLandmark Properties

- **`ControlType Property`**
  ```csharp
  public LandmarkControlType ControlType { get; }
  ```
  Gets the type of the control.

- **`LandmarkType Property`**
  ```csharp
  public EyeOfTheStormLandmarkType LandmarkType { get; }
  ```
  Gets the type of the landmark.

---

### EyeOfTheStormLandmarkType Enumeration

```csharp
public enum EyeOfTheStormLandmarkType
```

Values that represent EyeOfTheStormLandmarkType.

---

### GameInput Class

**Inheritance:** System.Object → Styx.WoWInternals.GameInput

```csharp
public static class GameInput
```

A game input.

#### GameInput Methods

- **`IsMouseButtonDown Method`**
  ```csharp
  public static bool IsMouseButtonDown(
InputMouseButton button
)
  ```
  Query if 'button' is mouse button down.
  - *button*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTE7142226_1?cs=.|vb=.|cpp=::|nu=.|fs=.");InputMouseButtonThe button.
  - **Returns:** ReferenceGameInput Class

---

### InputMouseButton Enumeration

```csharp
public enum InputMouseButton
```

Values that represent InputMouseButton.

---

### IsleOfConquestLandmark Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWLandMark → Styx.WoWInternals.AreaPoiLandmark → Styx.WoWInternals.IsleOfConquestLandmark

```csharp
public class IsleOfConquestLandmark : AreaPoiLandmark
```

An isle of conquest landmark.

#### IsleOfConquestLandmark Properties

- **`ControlType Property`**
  ```csharp
  public LandmarkControlType ControlType { get; }
  ```
  Gets the type of the control.

- **`LandmarkType Property`**
  ```csharp
  public IsleOfConquestLandmarkType LandmarkType { get; }
  ```
  Gets the type of the landmark.

---

### IsleOfConquestLandmarkType Enumeration

```csharp
[FlagsAttribute]
public enum IsleOfConquestLandmarkType
```

Bitfield of flags for specifying IsleOfConquestLandmarkType.

---

### ItemContext Enumeration

```csharp
public enum ItemContext
```

Represents item contexts.

---

### LandmarkControlType Enumeration

```csharp
public enum LandmarkControlType
```

Values that represent LandmarkControlType.

---

### LandmarkType Enumeration

```csharp
public enum LandmarkType
```

Values that represent LandmarkType.

---

### Lua Class

**Inheritance:** System.Object → Styx.WoWInternals.Lua

```csharp
public static class Lua
```

A lua.

#### Lua Properties

- **`Events Property`**
  ```csharp
  public static LuaEvents Events { get; }
  ```
  Gets the events.

- **`State Property`**
  ```csharp
  public static LuaState State { get; }
  ```
  Gets the current Lua state. This object should never be cached, but should be used
            immediately and then disposed of, before any control is transferred to HB.

#### Lua Methods

- **`DoString Method`**
  Executes a lua string.

- **`DoString Method (String, Object[])`**
  ```csharp
  public static void DoString(
string format,
params Object[] args
)
  ```
  Executes a lua string.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTBE3408D_3?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe format.
  - *args*: Type: AddLanguageSpecificTextSet("LSTBE3408D_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTBE3408D_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTBE3408D_6?cpp=&gt;|vb=()|nu=[]");  The args.

- **`DoString Method (String, String)`**
  ```csharp
  public static void DoString(
string szLua,
string szLuaFile = "WoW.lua"
)
  ```
  Executes a lua string.
  - *szLua*: Type: SystemAddLanguageSpecificTextSet("LSTABB0F157_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String    The lua.

- **`DoString Method (String, String, IntPtr)`**
  ```csharp
  public static void DoString(
string lua,
string luaFile,
IntPtr pState
)
  ```
  Executes a lua string.
  - *lua*: Type: SystemAddLanguageSpecificTextSet("LST6B32261_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String    The lua.
  - *luaFile*: Type: SystemAddLanguageSpecificTextSet("LST6B32261_2?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe lua file.
  - *pState*: Type: SystemAddLanguageSpecificTextSet("LST6B32261_3?cs=.|vb=.|cpp=::|nu=.|fs=.");IntPtr State of the p.

- **`Escape Method`**
  ```csharp
  public static string Escape(
string unescaped
)
  ```
  Escapes a string, replacing invalid characters like '\' with LUA ones, making it
            ready for use in LUA statements.
  - *unescaped*: Type: SystemAddLanguageSpecificTextSet("LSTA0779D18_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe unescaped.
  - **Returns:** ReferenceLua Class

- **`GetReturnVal(T) Method`**
  ```csharp
  public static T GetReturnVal&lt;T&gt;(
string lua,
uint retVal
)
  ```
  Gets the return value of a lua statement.
  - *lua*: Type: SystemAddLanguageSpecificTextSet("LST6C775C21_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String   The lua.
  - *retVal*: Type: SystemAddLanguageSpecificTextSet("LST6C775C21_4?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The ret val.
  - *T*: .
  - **Returns:** Remarks

- **`GetReturnValues Method`**
  Runs a chunk of lua code and returns a list of values returned by the statement.

- **`GetReturnValues Method (String)`**
  ```csharp
  public static List&lt;string&gt; GetReturnValues(
string lua
)
  ```
  Runs a chunk of lua code and returns a list of values returned by the statement.
  - *lua*: Type: SystemAddLanguageSpecificTextSet("LST6FF8FC33_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe lua code.
  - **Returns:** Exceptions

- **`GetReturnValues Method (String, String)`**
  ```csharp
  public static List&lt;string&gt; GetReturnValues(
string lua,
string scriptName
)
  ```
  Runs a chunk of lua code and returns a list of values returned by the statement.
  - *lua*: Type: SystemAddLanguageSpecificTextSet("LST4409ECF5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe lua code.
  - *scriptName*: Type: SystemAddLanguageSpecificTextSet("LST4409ECF5_2?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name of the script file to associate with the lua.
  - **Returns:** Exceptions

- **`ParseLuaValue(T) Method`**
  ```csharp
  public static T ParseLuaValue&lt;T&gt;(
string val
)
  ```
  Parses the specified string, returned by GetReturnValues(String), into the specified type.
  - *val*: Type: SystemAddLanguageSpecificTextSet("LST103726C4_3?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe string.
  - *T*: The type to convert into.
  - **Returns:** Remarks

---

### LuaEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.WoWInternals.LuaEventArgs → Styx.CommonBot.Chat.ChatMessageBaseEventArgs

```csharp
public class LuaEventArgs : EventArgs
```

Additional information for lua events.

#### LuaEventArgs Properties

- **`Args Property`**
  ```csharp
  public Object[] Args { get; }
  ```
  The args that were passed to the event. For basic types like 'double', HB will change
            them to 'double'. If something is not a number or string, the arg will be a 'LuaTValue'.

- **`EventName Property`**
  ```csharp
  public string EventName { get; }
  ```
  The name of the event.

- **`FireTimeStamp Property`**
  ```csharp
  public uint FireTimeStamp { get; }
  ```
  A timestamp corresponding to StyxWoW.Client.PerformanceCounter(), of the exact time
            the event was fired.

---

### LuaEventHandlerDelegate Delegate

```csharp
public delegate void LuaEventHandlerDelegate(
Object sender,
LuaEventArgs args
)
```

Lua event handler delegate.

---

### LuaEvents Class

**Inheritance:** System.Object → Styx.WoWInternals.LuaEvents

```csharp
public class LuaEvents
```

A lua events.

#### LuaEvents Properties

- **`IsInitialized Property`**
  ```csharp
  public bool IsInitialized { get; }
  ```
  Gets a value indicating whether this object is initialized.

- **`PrintAllEvents Property`**
  ```csharp
  public static bool PrintAllEvents { get; set; }
  ```
  Gets or sets a value indicating whether the print all events.

#### LuaEvents Methods

- **`AttachEvent Method`**
  Attaches to a Lua event.

- **`AttachEvent Method (String, LuaEventHandlerDelegate)`**
  ```csharp
  public void AttachEvent(
string eventName,
LuaEventHandlerDelegate handler
)
  ```
  Attaches to a Lua event.
  - *eventName*: Type: SystemAddLanguageSpecificTextSet("LST6264A7B5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name of the event to attach to.
  - *handler*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST6264A7B5_2?cs=.|vb=.|cpp=::|nu=.|fs=.");LuaEventHandlerDelegate  The handler to attach.

- **`AttachEvent Method (String, LuaEventHandlerDelegate, String)`**
  ```csharp
  public void AttachEvent(
string eventName,
LuaEventHandlerDelegate handler,
string filter
)
  ```
  Attaches to a lua event with the specified handler and filter.
  - *eventName*: Type: SystemAddLanguageSpecificTextSet("LST522880AB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name of the event to attach to.
  - *handler*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST522880AB_2?cs=.|vb=.|cpp=::|nu=.|fs=.");LuaEventHandlerDelegateThe handler to attach.
  - *filter*: Type: SystemAddLanguageSpecificTextSet("LST522880AB_3?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe filter code. May be null.

- **`DetachEvent Method`**
  ```csharp
  public void DetachEvent(
string eventName,
LuaEventHandlerDelegate handler
)
  ```
  Detaches from a Lua event.
  - *eventName*: Type: SystemAddLanguageSpecificTextSet("LSTD263AE9D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name of the event to detach from.
  - *handler*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTD263AE9D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");LuaEventHandlerDelegate  The handler to detach.

---

### LuaNode Class

**Inheritance:** System.Object → Styx.WoWInternals.LuaNode

```csharp
public class LuaNode
```

A lua node.

#### LuaNode Properties

- **`Address Property`**
  ```csharp
  public IntPtr Address { get; }
  ```
  Gets the address.

- **`Key Property`**
  ```csharp
  public LuaTKey Key { get; }
  ```
  Gets the key.

- **`NativeSize Property`**
  ```csharp
  public static int NativeSize { get; }
  ```
  Gets the size of the native.

- **`Value Property`**
  ```csharp
  public LuaTValue Value { get; }
  ```
  Gets the value.

---

### LuaRunStatus Enumeration

```csharp
public enum LuaRunStatus
```

---

### LuaState Class

**Inheritance:** System.Object → Styx.WoWInternals.LuaState

```csharp
public class LuaState
```

A lua state.

#### LuaState Properties

- **`Address Property`**
  ```csharp
  public IntPtr Address { get; }
  ```
  Gets the address.

- **`Globals Property`**
  ```csharp
  public LuaTable Globals { get; }
  ```
  Gets the globals.

---

### LuaTKey Class

**Inheritance:** System.Object → Styx.WoWInternals.LuaTKey

```csharp
public class LuaTKey
```

A lua t key.

#### LuaTKey Properties

- **`Address Property`**
  ```csharp
  public IntPtr Address { get; }
  ```
  Gets the address.

- **`NativeSize Property`**
  ```csharp
  public static int NativeSize { get; }
  ```
  Gets the size of the native.

- **`Next Property`**
  ```csharp
  public LuaNode Next { get; }
  ```
  Gets the next.

- **`Type Property`**
  ```csharp
  public LuaType Type { get; }
  ```
  Gets the type.

- **`Value Property`**
  ```csharp
  public LuaValue Value { get; }
  ```
  Gets the value.

---

### LuaTString Class

**Inheritance:** System.Object → Styx.WoWInternals.LuaTString

```csharp
public class LuaTString
```

A lua t string.

#### LuaTString Properties

- **`Address Property`**
  ```csharp
  public IntPtr Address { get; }
  ```
  Gets the address.

- **`Hash Property`**
  ```csharp
  public uint Hash { get; }
  ```
  Gets the hash.

- **`NativeSize Property`**
  ```csharp
  public static int NativeSize { get; }
  ```
  Gets the size of the native.

- **`Value Property`**
  ```csharp
  public string Value { get; }
  ```
  Gets the value.

---

### LuaTValue Class

**Inheritance:** System.Object → Styx.WoWInternals.LuaTValue

```csharp
public class LuaTValue
```

A lua t value.

#### LuaTValue Properties

- **`Address Property`**
  ```csharp
  public IntPtr Address { get; }
  ```
  Gets the address.

- **`NativeSize Property`**
  ```csharp
  public static int NativeSize { get; }
  ```
  Gets the size of the native.

- **`Type Property`**
  ```csharp
  public LuaType Type { get; }
  ```
  Gets the type.

- **`Value Property`**
  ```csharp
  public LuaValue Value { get; }
  ```
  Gets the value.

#### LuaTValue Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceLuaTValue Class

---

### LuaTable Class

**Inheritance:** System.Object → Styx.WoWInternals.LuaTable

```csharp
public class LuaTable
```

Represents a Lua table.

#### LuaTable Properties

- **`Address Property`**
  ```csharp
  public IntPtr Address { get; }
  ```
  Gets the address.

- **`Count Property`**
  ```csharp
  public int Count { get; }
  ```
  Gets the number of.

- **`Flags Property`**
  ```csharp
  public byte Flags { get; }
  ```
  Gets the flags.

- **`MetaTable Property`**
  ```csharp
  public LuaTable MetaTable { get; }
  ```
  Gets the meta table.

- **`NativeSize Property`**
  ```csharp
  public static int NativeSize { get; }
  ```
  Gets the size of the native.

#### LuaTable Methods

- **`GetField Method`**
  Gets a field indexed by a number. This number can also be a raw index. In that case,
            it is zero based.

- **`GetField Method (Double)`**
  ```csharp
  public LuaTValue GetField(
double key
)
  ```
  Gets a field indexed by a number. This number can also be a raw index. In that case,
            it is zero based.
  - *key*: Type: SystemAddLanguageSpecificTextSet("LSTB49EDC49_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Double.
  - **Returns:** ReferenceLuaTable Class

- **`GetField Method (String)`**
  ```csharp
  public LuaTValue GetField(
string key
)
  ```
  Gets a field indexed by a string.
  - *key*: Type: SystemAddLanguageSpecificTextSet("LST5690DE75_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - **Returns:** ReferenceLuaTable Class

---

### LuaType Enumeration

```csharp
public enum LuaType
```

Values that represent LuaType.

---

### LuaValue Class

**Inheritance:** System.Object → Styx.WoWInternals.LuaValue

```csharp
public class LuaValue
```

A lua value.

#### LuaValue Properties

- **`Address Property`**
  ```csharp
  public IntPtr Address { get; }
  ```
  Gets the address.

- **`Bool Property`**
  ```csharp
  public bool Bool { get; }
  ```
  Gets a value indicating whether the.

- **`Double Property`**
  ```csharp
  public double Double { get; }
  ```
  Gets the double.

- **`NativeSize Property`**
  ```csharp
  public static int NativeSize { get; }
  ```
  Gets the size of the native.

- **`Pointer Property`**
  ```csharp
  public IntPtr Pointer { get; }
  ```
  Gets the pointer.

- **`String Property`**
  ```csharp
  public LuaTString String { get; }
  ```
  Gets the string.

- **`Table Property`**
  ```csharp
  public LuaTable Table { get; }
  ```
  Gets the table.

---

### MoveFlags Enumeration

```csharp
[FlagsAttribute]
public enum MoveFlags
```

Bitfield of flags for specifying MoveFlags.

---

### NativeLuaCommonHeader Structure

```csharp
public struct NativeLuaCommonHeader
```

A native lua common header.

---

### NativeLuaNode Structure

```csharp
public struct NativeLuaNode
```

A native lua node.

#### NativeLuaNode Fields

- **`Key Field`**
  ```csharp
  public NativeLuaTKey Key
  ```
  The key.

- **`Value Field`**
  ```csharp
  public NativeLuaTValue Value
  ```
  The value.

---

### NativeLuaTKey Structure

```csharp
public struct NativeLuaTKey
```

A native lua t key.

#### NativeLuaTKey Fields

- **`NextNodePtr Field`**
  ```csharp
  public IntPtr NextNodePtr
  ```
  The next node pointer.

- **`Type Field`**
  ```csharp
  public LuaType Type
  ```
  The type.

- **`Value Field`**
  ```csharp
  public NativeLuaValue Value
  ```
  The value.

---

### NativeLuaTString Structure

```csharp
public struct NativeLuaTString
```

A native lua t string.

#### NativeLuaTString Fields

- **`Hash Field`**
  ```csharp
  public uint Hash
  ```
  The hash.

- **`Header Field`**
  ```csharp
  public NativeLuaCommonHeader Header
  ```
  The header.

- **`Length Field`**
  ```csharp
  public IntPtr Length
  ```
  The length.

---

### NativeLuaTValue Structure

```csharp
public struct NativeLuaTValue
```

A native lua t value.

#### NativeLuaTValue Fields

- **`Type Field`**
  ```csharp
  public LuaType Type
  ```
  The type.

- **`Value Field`**
  ```csharp
  public NativeLuaValue Value
  ```
  The value.

---

### NativeLuaTable Structure

```csharp
public struct NativeLuaTable
```

A native lua table.

#### NativeLuaTable Properties

- **`NodesCount Property`**
  ```csharp
  public uint NodesCount { get; }
  ```
  Gets the number of nodes.

#### NativeLuaTable Fields

- **`Flags Field`**
  ```csharp
  public byte Flags
  ```
  The flags.

- **`Header Field`**
  ```csharp
  public NativeLuaCommonHeader Header
  ```
  The header.

- **`MetaTablePtr Field`**
  ```csharp
  public IntPtr MetaTablePtr
  ```
  The meta table pointer.

- **`NodePtr Field`**
  ```csharp
  public IntPtr NodePtr
  ```
  The node pointer.

- **`ValuesCount Field`**
  ```csharp
  public uint ValuesCount
  ```
  Number of values.

- **`ValuesPtr Field`**
  ```csharp
  public IntPtr ValuesPtr
  ```
  The values pointer.

---

### NativeLuaValue Structure

```csharp
public struct NativeLuaValue
```

A native lua value.

#### NativeLuaValue Properties

- **`Bool Property`**
  ```csharp
  public bool Bool { get; }
  ```
  Gets a value indicating whether the.

#### NativeLuaValue Fields

- **`Number Field`**
  ```csharp
  public double Number
  ```
  Number of.

- **`Pointer Field`**
  ```csharp
  public IntPtr Pointer
  ```
  The pointer.

---

### NativeObject Class

**Inheritance:** System.Object → Styx.WoWInternals.NativeObject → Styx.WoWInternals.WoWVehicle

```csharp
public abstract class NativeObject
```

Represent a native object.

#### NativeObject Properties

- **`BaseAddress Property`**
  ```csharp
  public IntPtr BaseAddress { get; }
  ```
  The Baseaddress of this native object.

- **`Memory Property`**
  ```csharp
  protected ExternalProcessMemory Memory { get; }
  ```
  Memory shortcut.

#### NativeObject Methods

- **`CallVFunc(T) Method`**
  ```csharp
  protected T CallVFunc&lt;T&gt;(
int index,
params Object[] args
)
where T : struct, new()
  ```
  Calls a VFunc.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTAC56DE63_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The index.
  - *args*: Type: AddLanguageSpecificTextSet("LSTAC56DE63_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTAC56DE63_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTAC56DE63_6?cpp=&gt;|vb=()|nu=[]");The arguments.
  - *T*: The type.
  - **Returns:** See Also

- **`GetVFunc Method`**
  ```csharp
  protected IntPtr GetVFunc(
IntPtr vtable,
int index
)
  ```
  Returns the adress of a virtual function.
            Note: Only valid on objects that actually got a VTable!
  - *vtable*: Type: SystemAddLanguageSpecificTextSet("LST67815E26_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IntPtrPointer to the function table.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST67815E26_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The index of the function.
  - **Returns:** ReferenceNativeObject Class

---

### ObjectListUpdateFinishedDelegate Delegate

```csharp
public delegate void ObjectListUpdateFinishedDelegate(
Object context
)
```

Object list update finished delegate.

---

### ObjectManager Class

**Inheritance:** System.Object → Styx.WoWInternals.ObjectManager

```csharp
public static class ObjectManager
```

Manager for objects.

#### ObjectManager Properties

- **`LocalGuid Property`**
  ```csharp
  public static WoWGuid LocalGuid { get; }
  ```
  Gets the local GUID.

- **`ObjectList Property`**
  ```csharp
  public static List&lt;WoWObject&gt; ObjectList { get; }
  ```
  Gets the object list.

#### ObjectManager Methods

- **`GetObjectByGuid(T) Method`**
  ```csharp
  public static T GetObjectByGuid&lt;T&gt;(
WoWGuid guid
)
where T : WoWObject
  ```
  Gets the object by GUID.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST7288B9D6_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidThe GUID.
  - *T*: .
  - **Returns:** See Also

- **`GetObjectsOfType(T) Method`**
  ```csharp
  public static List&lt;T&gt; GetObjectsOfType&lt;T&gt;(
bool allowInheritance = false,
bool includeMeIfFound = false
)
where T : WoWObject
  ```
  Gets object of the specified type.
  - *T*: .
  - **Returns:** See Also

- **`GetObjectsOfTypeFast(T) Method`**
  ```csharp
  public static List&lt;T&gt; GetObjectsOfTypeFast&lt;T&gt;()
where T : WoWObject
  ```
  A faster method of returning a list of specific types of objects. This will always
            include inheritance, and "Me" if found. Do your own filtering.
  - *T*: .
  - **Returns:** See Also

- **`Update Method`**
  ```csharp
  public static void Update()
  ```
  Updates this instance.

#### ObjectManager Events

- **`OnObjectListUpdateFinished Event`**
  ```csharp
  [ObsoleteAttribute("Use tree hooks, pulse methods or BotEvents.OnPulse instead. This event will be removed in the future.")]
public static event ObjectListUpdateFinishedDelegate OnObjectListUpdateFinished
  ```
  Occurs when [the object list has finished updating].

---

### PetStance Enumeration

```csharp
public enum PetStance
```

---

### PlayerQuest Class

**Inheritance:** System.Object → Styx.WoWInternals.Quest → Styx.WoWInternals.PlayerQuest

```csharp
public class PlayerQuest : Quest
```

Represents a quest the player has in his questlog.

#### PlayerQuest Properties

- **`IsCompleted Property`**
  ```csharp
  public bool IsCompleted { get; }
  ```
  Gets a value indicating whether this object is completed.

- **`IsFailed Property`**
  ```csharp
  public bool IsFailed { get; }
  ```
  Gets a value indicating whether this object is failed.

#### PlayerQuest Methods

- **`GetData Method`**
  ```csharp
  public bool GetData(
out WoWDescriptorQuest data
)
  ```
  Gets a data.
  - *data*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTE8B8E5D5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWDescriptorQuestAddLanguageSpecificTextSet("LSTE8B8E5D5_2?cpp=%");[out] The data.
  - **Returns:** ReferencePlayerQuest Class

- **`GetQuestPOIInfo Method`**
  Gets the quest POI info for this quest.

- **`GetQuestPOIInfo Method`**
  ```csharp
  [ObsoleteAttribute("Use the overload that returns a bool which can indicate that no POI info is available")]
public WoWQuestPOIInfo GetQuestPOIInfo()
  ```
  Gets the quest POI info for this quest.
  - **Returns:** ReferencePlayerQuest Class

- **`GetQuestPOIInfo Method (WoWQuestPOIInfo)`**
  ```csharp
  public bool GetQuestPOIInfo(
out WoWQuestPOIInfo info
)
  ```
  Gets the quest POI info for this quest.
  - *info*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTF2F0A7E0_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWQuestPOIInfoAddLanguageSpecificTextSet("LSTF2F0A7E0_3?cpp=%");[out] The info.
  - **Returns:** ReferencePlayerQuest Class

---

### Quest Class

**Inheritance:** System.Object → Styx.WoWInternals.Quest → Styx.WoWInternals.PlayerQuest

```csharp
public class Quest
```

A question.

#### Quest Properties

- **`CompletionText Property`**
  ```csharp
  public string CompletionText { get; }
  ```
  The text told by the NPC when delivering the quest.

- **`Description Property`**
  ```csharp
  public string Description { get; }
  ```
  The description of this quest.

- **`FlagsPVP Property`**
  ```csharp
  public bool FlagsPVP { get; }
  ```
  A boolean indicating whether accepting this quest will flag you for PVP.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Gets the identifier.

- **`InternalInfo Property`**
  ```csharp
  public WoWCache.QuestCacheEntry InternalInfo { get; }
  ```
  The internal info of this quest.

- **`IsAutoAccepted Property`**
  ```csharp
  public bool IsAutoAccepted { get; }
  ```
  A boolean indicating whether opening the quest description is enough to accept it
            automatically.

- **`IsAutoComplete Property`**
  ```csharp
  public bool IsAutoComplete { get; }
  ```
  A boolean indicating whether the quest can be completed from anywhere.

- **`IsDaily Property`**
  ```csharp
  public bool IsDaily { get; }
  ```
  A boolean indicating whether this quest is a daily quest.

- **`IsPartyQuest Property`**
  ```csharp
  public bool IsPartyQuest { get; }
  ```
  A boolean indicating whether this quest will be started for all party members on
            accept; like escort quests.

- **`IsShareable Property`**
  ```csharp
  public bool IsShareable { get; }
  ```
  A boolean indicating whether this quest is shareable.

- **`IsStayAliveQuest Property`**
  ```csharp
  public bool IsStayAliveQuest { get; }
  ```
  A boolean indicating whether this quest is a stay alive quest; meaning it will be
            failed if you die.

- **`IsWeekly Property`**
  ```csharp
  public bool IsWeekly { get; }
  ```
  A boolean indicating whether this quest is a weekly quest.

- **`Level Property`**
  ```csharp
  public int Level { get; }
  ```
  The level of this quest.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  The name of this quest.

- **`NormalObjectiveRequiredCounts Property`**
  ```csharp
  public uint[] NormalObjectiveRequiredCounts { get; }
  ```
  The required count of the normal objectives of this quest.

- **`Objectives Property`**
  ```csharp
  public string[] Objectives { get; }
  ```
  The special objectives of this quest. These will only be filled for objectives that
            aren't normal. They won't be filled for a "slay 10 XX" objective.

- **`ObjectiveText Property`**
  ```csharp
  public string ObjectiveText { get; }
  ```
  The objective text of this quest (the text over the actual objectives, eg. the text
            over "Voidwalker's slain 0/20")

- **`RequiredLevel Property`**
  ```csharp
  public int RequiredLevel { get; }
  ```
  The required level to pick up this quest.

- **`RewardMoney Property`**
  ```csharp
  public uint RewardMoney { get; }
  ```
  The amount of money received by turning in this quest.

- **`RewardMoneyAtMaxLevel Property`**
  ```csharp
  public uint RewardMoneyAtMaxLevel { get; }
  ```
  The amount of money received by turning in this quest at level 80.

- **`RewardNumTalentPoints Property`**
  ```csharp
  public uint RewardNumTalentPoints { get; }
  ```
  The amount of talent points rewarded by completing this quest. Currently only exists
            in DK starting place.

- **`RewardSpell Property`**
  ```csharp
  public WoWSpell RewardSpell { get; }
  ```
  The spell rewarded by completing this quest.

- **`RewardSpellId Property`**
  ```csharp
  public int RewardSpellId { get; }
  ```
  The spell ID rewarded by completing this quest, or 0 if the reward is not a spell.

- **`RewardTitleId Property`**
  ```csharp
  public uint RewardTitleId { get; }
  ```
  The ID of the title rewarded for completing this quest.

- **`RewardXp Property`**
  ```csharp
  public int RewardXp { get; }
  ```
  The amount of XP that is rewarded by completing this quest.

- **`SubDescription Property`**
  ```csharp
  public string SubDescription { get; }
  ```
  The sub description of this quest.

- **`SuggestedPlayers Property`**
  ```csharp
  public uint SuggestedPlayers { get; }
  ```
  Suggested players for this quest, or 0 if it's not a party quest.

#### Quest Methods

- **`FromId Method`**
  ```csharp
  public static Quest FromId(
uint id
)
  ```
  Gets a quest by ID. Returns null if there is no information available for the quest.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST97A91BC6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32.
  - **Returns:** ReferenceQuest Class

- **`GetObjectives Method`**
  ```csharp
  public List&lt;Quest.QuestObjective&gt; GetObjectives()
  ```
  Gets the objectives of this quest.
  - **Returns:** See Also

- **`GetRewardChoices Method`**
  ```csharp
  public WoWCache.QuestRewardChoice[] GetRewardChoices()
  ```
  Gets the reward choices of this quest.
  - **Returns:** See Also

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceQuest Class

---

### Quest.QuestObjective Structure

```csharp
public struct QuestObjective
```

A question objective.

#### QuestObjective Properties

- **`IsEmpty Property`**
  ```csharp
  public bool IsEmpty { get; }
  ```
  Gets a value indicating whether this object is empty.

#### QuestObjective Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceQuestAddLanguageSpecificTextSet("LST24653037_2?cs=.|vb=.|cpp=::|nu=.|fs=.");QuestObjective Structure

#### QuestObjective Fields

- **`Count Field`**
  ```csharp
  public uint Count
  ```
  The count that is needed of the specified objective.

- **`ID Field`**
  ```csharp
  public uint ID
  ```
  The ID of the objective. If the objective is CollectItem, this will be an item ID. If
            it's KillMob, it will be a mob ID. If it's UseGameObject, it will be a game object ID.

- **`Index Field`**
  ```csharp
  public int Index
  ```
  Zero-based index of the.

- **`Objective Field`**
  ```csharp
  public string Objective
  ```
  The string objective. If the objective type is special, this will be a string.

- **`Type Field`**
  ```csharp
  public Quest.QuestObjectiveType Type
  ```
  The type of objective.

---

### Quest.QuestObjectiveType Enumeration

```csharp
public enum QuestObjectiveType
```

Values that represent QuestObjectiveType.

---

### QuestLog Class

**Inheritance:** System.Object → Styx.WoWInternals.QuestLog

```csharp
public class QuestLog
```

A question log.

#### QuestLog Properties

- **`Instance Property`**
  ```csharp
  public static QuestLog Instance { get; }
  ```
  Gets the instance.

- **`IsFull Property`**
  ```csharp
  public bool IsFull { get; }
  ```
  Returns whether or not the quest log is currently full, and cannot accept more quests.

- **`QuestCount Property`**
  ```csharp
  public uint QuestCount { get; }
  ```
  Number of quests in the questlog including the headers. eg; 'Dun Morogh', 'Loch
            Modan', 'Alterac Valley' etc.

#### QuestLog Methods

- **`AbandonQuestById Method`**
  ```csharp
  public void AbandonQuestById(
uint questId
)
  ```
  Abandons a quest by it's id.
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LSTE47BBE98_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The quest id.

- **`ContainsQuest Method`**
  ```csharp
  public bool ContainsQuest(
uint questId
)
  ```
  Determines whether the specified quest id is contained in the questlog.
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LST7CB4302C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The quest id.
  - **Returns:** See Also

- **`GetAllQuests Method`**
  ```csharp
  public List&lt;PlayerQuest&gt; GetAllQuests()
  ```
  Gets all the quests in your questlog.
  - **Returns:** See Also

- **`GetCompletedQuests Method`**
  ```csharp
  public ReadOnlyCollection&lt;uint&gt; GetCompletedQuests()
  ```
  Gets the completed quests.
  - **Returns:** See Also

- **`GetIndexForQuest Method`**
  ```csharp
  public int GetIndexForQuest(
uint questId
)
  ```
  Gets the index for a quest - to be used with GetQuest(UInt32) and not consistent with the lua function 'GetQuestLogIndexByID'
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LST3E877AA6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The quest id.
  - **Returns:** ReferenceQuestLog Class

- **`GetQuest Method`**
  ```csharp
  public PlayerQuest GetQuest(
uint index
)
  ```
  Gets a quest by the index.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST9ED2017_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The index.
  - **Returns:** ExceptionConditionArgumentOutOfRangeExceptionThrown when one or more arguments are outside the
            required range.

- **`GetQuestById Method`**
  ```csharp
  public PlayerQuest GetQuestById(
uint questID
)
  ```
  Gets the quest by id.
  - *questID*: Type: SystemAddLanguageSpecificTextSet("LST888D9C9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The quest ID.
  - **Returns:** ReferenceQuestLog Class

- **`GetQuestId Method`**
  ```csharp
  public uint GetQuestId(
uint index
)
  ```
  Gets the quest id of a quest in your questlog by the index.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST6352B336_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The index.
  - **Returns:** ExceptionConditionArgumentOutOfRangeExceptionThrown when one or more arguments are outside the
            required range.

- **`GetQuestInfo Method`**
  ```csharp
  public WoWDescriptorQuest GetQuestInfo(
int index
)
  ```
  Gets detailed info on a quest.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST8DFD2638_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The index.
  - **Returns:** ExceptionConditionArgumentOutOfRangeExceptionThrown if index is negative or greater than or equal to MaxQuestsInLog.

- **`GetQuestItemCount Method`**
  ```csharp
  public int GetQuestItemCount(
uint itemID
)
  ```
  Gets the item count of a quest item that is not contained in the normal bags.
  - *itemID*: Type: SystemAddLanguageSpecificTextSet("LSTAFA077E9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The item ID.
  - **Returns:** ReferenceQuestLog Class

#### QuestLog Fields

- **`MaxQuests Field`**
  ```csharp
  public const int MaxQuests = 25
  ```
  The maximum actual quests in the log.

- **`MaxQuestsInLog Field`**
  ```csharp
  public const int MaxQuestsInLog = 50
  ```
  The maximum quests in log.

---

### ResearchSiteLandmark Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWLandMark → Styx.WoWInternals.ResearchSiteLandmark

```csharp
public class ResearchSiteLandmark : WoWLandMark
```

A research site landmark.

#### ResearchSiteLandmark Properties

- **`AreaId Property`**
  ```csharp
  public override int AreaId { get; }
  ```
  Gets the identifier of the area.

- **`Description Property`**
  ```csharp
  public override string Description { get; }
  ```
  Gets the description.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```

- **`IsValid Property`**
  ```csharp
  public override bool IsValid { get; }
  ```
  Returns true if this landmark is a valid object.

- **`Location Property`**
  ```csharp
  public override Vector2 Location { get; }
  ```
  Gets the location.

- **`MapId Property`**
  ```csharp
  public override int MapId { get; }
  ```
  Gets the identifier of the map.

- **`Name Property`**
  ```csharp
  public override string Name { get; }
  ```
  Gets the name.

- **`NormalIcon Property`**
  ```csharp
  public override int NormalIcon { get; }
  ```
  Gets the normal icon.

- **`QuestPoiBlobId Property`**
  ```csharp
  public uint QuestPoiBlobId { get; }
  ```
  Gets the identifier of the question poi BLOB.

- **`ShowInBattleMap Property`**
  ```csharp
  public override bool ShowInBattleMap { get; }
  ```
  Gets a value indicating whether the in battle map is shown.

- **`ShowInBg Property`**
  ```csharp
  public override bool ShowInBg { get; }
  ```
  Gets a value indicating whether the in background is shown.

- **`ShowInZone Property`**
  ```csharp
  public override bool ShowInZone { get; }
  ```
  Gets a value indicating whether the in zone is shown.

- **`TextureIndex Property`**
  ```csharp
  public override int TextureIndex { get; }
  ```
  Gets the zero-based index of the texture.

- **`WorldState Property`**
  ```csharp
  public override uint WorldState { get; }
  ```
  Gets the state of the world.

---

### SpellChargeInfo Class

**Inheritance:** System.Object → Styx.WoWInternals.SpellChargeInfo

```csharp
public class SpellChargeInfo
```

Represents charge info.

#### SpellChargeInfo Properties

- **`ChargesLeft Property`**
  ```csharp
  public int ChargesLeft { get; }
  ```
  Gets the number of charges left.

- **`MaxCharges Property`**
  ```csharp
  public int MaxCharges { get; }
  ```
  The maximum number of charges that can be stored for this spell.

- **`RechargeCooldown Property`**
  ```csharp
  public TimeSpan RechargeCooldown { get; }
  ```
  The cooldown of recharges. This is the time it takes for another charge to be available after the first charge has been used.

- **`TimeUntilNextCharge Property`**
  ```csharp
  public TimeSpan TimeUntilNextCharge { get; }
  ```
  The time left until the next charge will become available.

#### SpellChargeInfo Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceSpellChargeInfo Class

---

### SpellCooldownInfo Class

**Inheritance:** System.Object → Styx.WoWInternals.SpellCooldownInfo

```csharp
public class SpellCooldownInfo
```

Information about the spell cooldown.

#### SpellCooldownInfo Properties

- **`CooldownBeginsWhenUsed Property`**
  ```csharp
  public bool CooldownBeginsWhenUsed { get; set; }
  ```
  Gets or sets a value indicating whether the cooldown begins when used.

- **`CooldownTimeRemaining Property`**
  ```csharp
  public int CooldownTimeRemaining { get; set; }
  ```
  Gets or sets the cooldown time remaining.

- **`EndTime Property`**
  ```csharp
  public int EndTime { get; set; }
  ```
  Gets or sets the end time.

- **`StartTime Property`**
  ```csharp
  public int StartTime { get; set; }
  ```
  Gets or sets the start time.

---

### SpellDetailedPowerCost Class

**Inheritance:** System.Object → Styx.WoWInternals.SpellDetailedPowerCost

```csharp
public class SpellDetailedPowerCost
```

Represents detailed power costs of a spell.

#### SpellDetailedPowerCost Properties

- **`IsPerSecond Property`**
  ```csharp
  public bool IsPerSecond { get; }
  ```
  Whether the cost is per second.

- **`PowerType Property`**
  ```csharp
  public WoWPowerType PowerType { get; }
  ```
  The power type of the cost.

- **`Value Property`**
  ```csharp
  public double Value { get; }
  ```
  The value of the cost.

#### SpellDetailedPowerCost Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceSpellDetailedPowerCost Class

---

### StrandOfTheAncientsLandmark Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWLandMark → Styx.WoWInternals.AreaPoiLandmark → Styx.WoWInternals.StrandOfTheAncientsLandmark

```csharp
public class StrandOfTheAncientsLandmark : AreaPoiLandmark
```

A strand of the ancients landmark.

#### StrandOfTheAncientsLandmark Properties

- **`ControlType Property`**
  ```csharp
  public LandmarkControlType ControlType { get; }
  ```
  Gets the type of the control.

- **`LandmarkType Property`**
  ```csharp
  public StrandOfTheAncientsLandmarkType LandmarkType { get; }
  ```
  Gets the type of the landmark.

---

### StrandOfTheAncientsLandmarkType Enumeration

```csharp
[FlagsAttribute]
public enum StrandOfTheAncientsLandmarkType
```

Bitfield of flags for specifying StrandOfTheAncientsLandmarkType.

---

### TaxiMapNode Class

**Inheritance:** System.Object → Styx.WoWInternals.TaxiMapNode

```csharp
public class TaxiMapNode
```

Represents a node shown on the taxi map.

#### TaxiMapNode Properties

- **`Cost Property`**
  ```csharp
  public int Cost { get; }
  ```
  Gets the cost, in copper, of flying to this node.

- **`Index Property`**
  ```csharp
  public int Index { get; }
  ```
  Gets the index of this node on the taxi map.

- **`Info Property`**
  ```csharp
  public TaxiNode Info { get; }
  ```
  Gets more information about this node.

- **`NodeId Property`**
  ```csharp
  public int NodeId { get; }
  ```
  Gets the ID of this node.

- **`Position Property`**
  ```csharp
  public Vector2 Position { get; }
  ```
  Gets the position on the taxi map.

#### TaxiMapNode Methods

- **`TakeFlight Method`**
  ```csharp
  public void TakeFlight()
  ```
  Takes this flight, as if the player pressed its icon on the taxi map.

---

### WoWApplyAuraType Enumeration

```csharp
public enum WoWApplyAuraType
```

Values that represent WoWApplyAuraType.

---

### WoWAura Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWAura

```csharp
public class WoWAura
```

Represents an aura in World of Warcraft. This class stores static data. To refresh aura info, get a new instance from Auras and similar API's.

#### WoWAura Properties

- **`ApplyAuraType Property`**
  ```csharp
  [ObsoleteAttribute("Use Spell.SpellEffects")]
public WoWApplyAuraType ApplyAuraType { get; }
  ```
  The first Auratype of this aura.

- **`Cancellable Property`**
  ```csharp
  public bool Cancellable { get; }
  ```
  Returns whether or not the aura can be cancelled by the user.

- **`CreatorGuid Property`**
  ```csharp
  public WoWGuid CreatorGuid { get; }
  ```
  Gets the creator Guid of this WoWAura.

- **`Duration Property`**
  ```csharp
  public uint Duration { get; }
  ```
  Gets the duration of this WoWAura.

- **`EndTime Property`**
  ```csharp
  public uint EndTime { get; }
  ```
  Gets the end time of this WoWAura.

- **`Flags Property`**
  ```csharp
  public AuraFlags Flags { get; }
  ```
  Flags of this aura.

- **`IsActive Property`**
  ```csharp
  public bool IsActive { get; }
  ```
  Gets a value indicating whether this instance is active.

- **`IsHarmful Property`**
  ```csharp
  public bool IsHarmful { get; }
  ```
  Gets a value indicating whether this instance is harmful.

- **`IsPassive Property`**
  ```csharp
  public bool IsPassive { get; }
  ```
  Gets a value indicating whether this instance is passive.

- **`Level Property`**
  ```csharp
  public uint Level { get; }
  ```
  Gets the level of this WoWAura.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name of this aura.

- **`Spell Property`**
  ```csharp
  public WoWSpell Spell { get; }
  ```
  Returns a WoWSpell with the Id of this Aura.

- **`SpellId Property`**
  ```csharp
  public int SpellId { get; }
  ```
  Gets the spell id of this WoWAura.

- **`StackCount Property`**
  ```csharp
  public uint StackCount { get; }
  ```
  Gets the stack count of this WoWAura.

- **`TimeLeft Property`**
  ```csharp
  public TimeSpan TimeLeft { get; }
  ```
  Gets the time left until this buff wears off.

- **`VariableEffects Property`**
  ```csharp
  public float[] VariableEffects { get; }
  ```
  Gets the variable effects for this aura.

#### WoWAura Methods

- **`Equals Method`**
  Determines whether the specified Object is equal to this
            instance.

- **`Equals Method (Object)`**
  ```csharp
  public override bool Equals(
Object obj
)
  ```
  Determines whether the specified Object is equal to this
            instance.
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LSTEE8CC502_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe Object to compare with this instance.
  - **Returns:** See Also

- **`Equals Method (WoWAura)`**
  ```csharp
  public bool Equals(
WoWAura other
)
  ```
  Equalses the specified other.
  - *other*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST148A6C63_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWAuraThe other.
  - **Returns:** ReferenceWoWAura Class

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  Returns a hash code for this instance.
  - **Returns:** ReferenceWoWAura Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceWoWAura Class

- **`TryCancelAura Method`**
  ```csharp
  public void TryCancelAura()
  ```
  Attempts to cancel this aura if possible.

#### WoWAura Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
WoWAura left,
WoWAura right
)
  ```
  Implements the operator ==.
  - *left*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTD53FD519_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWAura The left.
  - *right*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTD53FD519_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWAuraThe right.
  - **Returns:** ReferenceWoWAura Class

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
WoWAura left,
WoWAura right
)
  ```
  Implements the operator !=.
  - *left*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST6CC6F8B6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWAura The left.
  - *right*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST6CC6F8B6_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWAuraThe right.
  - **Returns:** ReferenceWoWAura Class

---

### WoWAuraCollection Class

**Inheritance:** System.Object → System.Collections.Generic.List.WoWAura → Styx.WoWInternals.WoWAuraCollection

```csharp
public class WoWAuraCollection : List&lt;WoWAura&gt;
```

Collection of WoW auras.

#### WoWAuraCollection Constructor

- **`WoWAuraCollection Constructor`**
  ```csharp
  public WoWAuraCollection()
  ```
  Default constructor.

- **`WoWAuraCollection Constructor (Int32)`**
  ```csharp
  public WoWAuraCollection(
int capacity
)
  ```
  Constructor.
  - *capacity*: Type: SystemAddLanguageSpecificTextSet("LST9CBA36E3_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The capacity.

- **`WoWAuraCollection Constructor (IEnumerable(WoWAura))`**
  ```csharp
  public WoWAuraCollection(
IEnumerable&lt;WoWAura&gt; collection
)
  ```
  Constructor.
  - *collection*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST16D97F35_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST16D97F35_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWAuraAddLanguageSpecificTextSet("LST16D97F35_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The collection.

#### WoWAuraCollection Methods

- **`GetTotalAuraModifier Method`**
  ```csharp
  public int GetTotalAuraModifier(
WoWApplyAuraType auraType
)
  ```
  Sums the BasePoints value of all spell effects of all auras in this collection, and
            returns the result.
  - *auraType*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTDAAFBDF4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWApplyAuraType.
  - **Returns:** ReferenceWoWAuraCollection Class

---

### WoWBag Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWBag → Styx.WoWInternals.WoWPaperDoll → Styx.WoWInternals.WoWPlayerInventory

```csharp
public class WoWBag
```

A WoW bag.

#### WoWBag Properties

- **`FirstSlotIndex Property`**
  ```csharp
  public uint FirstSlotIndex { get; }
  ```
  Gets the zero-based index of the first slot.

- **`FreeSlots Property`**
  ```csharp
  public uint FreeSlots { get; }
  ```
  Gets the number of free slots in this bag.

- **`Guid Property`**
  ```csharp
  public WoWGuid Guid { get; }
  ```
  Returns the GUID of this bag.

- **`IsInventory Property`**
  ```csharp
  public bool IsInventory { get; }
  ```
  Returns a bool indicating whether this bag is the players inventory.

- **`ItemGuids Property`**
  ```csharp
  public WoWGuid[] ItemGuids { get; }
  ```
  Gets an array of item GUIDs in this bag. If an item is 0, there is no item in that
            slot.

- **`Items Property`**
  ```csharp
  public WoWItem[] Items { get; }
  ```
  Gets an array of items in this bag. If an item is null, there is no item in that slot.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Returns the name of this bag.

- **`PhysicalItemGuids Property`**
  ```csharp
  public WoWGuid[] PhysicalItemGuids { get; }
  ```
  Gets an array of all item GUIDs in this bag.

- **`PhysicalItems Property`**
  ```csharp
  public WoWItem[] PhysicalItems { get; }
  ```
  Gets an array of all items in this bag.

- **`Slots Property`**
  ```csharp
  public uint Slots { get; }
  ```
  Gets the slots.

- **`UsedSlots Property`**
  ```csharp
  public uint UsedSlots { get; }
  ```
  Gets the number of used slots in this bag.

#### WoWBag Methods

- **`Contains Method`**
  Returns a bool indicating whether this bag contains an item.

- **`Contains Method (WoWGuid)`**
  ```csharp
  public bool Contains(
WoWGuid itemGuid
)
  ```
  Returns a bool indicating whether this bag contains an item.
  - *itemGuid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTD6ABF18A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuid.
  - **Returns:** ReferenceWoWBag Class

- **`Contains Method (WoWItem)`**
  ```csharp
  public bool Contains(
WoWItem item
)
  ```
  Returns a bool indicating whether this bag contains an item.
  - *item*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTEA7F2B5D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWItem.
  - **Returns:** ExceptionConditionArgumentNullExceptionitem is null.

- **`GetItemBySlot Method`**
  ```csharp
  public WoWItem GetItemBySlot(
uint slot
)
  ```
  Return an item by slot. The max slot is Slots - 1. If the returned value is null, the
            slot is empty.
  - *slot*: Type: SystemAddLanguageSpecificTextSet("LSTEBD63D04_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32.
  - **Returns:** ExceptionConditionArgumentOutOfRangeExceptionslot is out of range.

- **`GetItemGuidBySlot Method`**
  ```csharp
  public WoWGuid GetItemGuidBySlot(
uint slot
)
  ```
  Returns an items GUID by slot. The max slot is Slots - 1. If the returned value is 0,
            the slot is empty.
  - *slot*: Type: SystemAddLanguageSpecificTextSet("LSTE92C90E1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32.
  - **Returns:** ExceptionConditionArgumentOutOfRangeExceptionslot is out of range.

- **`IndexOf Method`**
  Returns the index of an item or -1 if it's not in this bag.

- **`IndexOf Method (WoWGuid)`**
  ```csharp
  public int IndexOf(
WoWGuid guid
)
  ```
  Returns the index of an item or -1 if it's not in this bag.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTE9A7DEF0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuid.
  - **Returns:** ReferenceWoWBag Class

- **`IndexOf Method (WoWItem)`**
  ```csharp
  public int IndexOf(
WoWItem item
)
  ```
  Returns the index of an item or -1 if it's not in this bag.
  - *item*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTBE32B171_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWItem.
  - **Returns:** ExceptionConditionArgumentNullExceptionitem is null.

---

### WoWCamera Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWCamera

```csharp
public class WoWCamera
```

A WoW camera.

#### WoWCamera Properties

- **`Aspect Property`**
  ```csharp
  public float Aspect { get; }
  ```
  Gets the aspect ratio. (WoW currently does NOT set this value. It is here for support
            once we figure out where exactly they store this value)

- **`FarZ Property`**
  ```csharp
  public float FarZ { get; }
  ```
  Gets the far z coordinate.

- **`FieldOfView Property`**
  ```csharp
  public float FieldOfView { get; }
  ```
  Gets the field of view.

- **`Forward Property`**
  ```csharp
  public Vector3 Forward { get; }
  ```
  Gets the forward vector of the camera.

- **`NearZ Property`**
  ```csharp
  public float NearZ { get; }
  ```
  Gets the near z coordinate.

- **`Position Property`**
  ```csharp
  public Vector3 Position { get; }
  ```
  Gets the position.

- **`Projection Property`**
  ```csharp
  public Matrix4x4 Projection { get; }
  ```
  Gets the projection matrix.

- **`View Property`**
  ```csharp
  public Matrix4x4 View { get; }
  ```
  Gets the view matrix.

- **`World Property`**
  ```csharp
  public Matrix4x4 World { get; }
  ```
  Gets the world matrix.

---

### WoWCurrency Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWCurrency

```csharp
public class WoWCurrency
```

Provides information on a specific currency.

#### WoWCurrency Properties

- **`Amount Property`**
  ```csharp
  public uint Amount { get; }
  ```
  The amount of currency.

- **`CurrencyCategoryEntry Property`**
  ```csharp
  public uint CurrencyCategoryEntry { get; }
  ```
  The currency-category dbc entry of this WoWCurrency.

- **`CurrencyType Property`**
  ```csharp
  public WoWCurrencyType CurrencyType { get; }
  ```
  The type of currency.

- **`CurrencyTypeInfo Property`**
  ```csharp
  public CurrencyType CurrencyTypeInfo { get; }
  ```
  Gets information about the currency type.

- **`EarnedThisWeek Property`**
  ```csharp
  public uint EarnedThisWeek { get; }
  ```
  The amount of currency you received this week.

- **`Entry Property`**
  ```csharp
  public uint Entry { get; }
  ```
  The dbc entry of this WoWCurrency.

- **`IsDiscovered Property`**
  ```csharp
  public bool IsDiscovered { get; }
  ```
  Gets a bool that indicates whether this currency has been discovered by the local player.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  The localized name of this currency.

- **`SeasonTotal Property`**
  ```csharp
  public uint SeasonTotal { get; }
  ```
  The amount of currency you have received this 'season'.

- **`TotalMax Property`**
  ```csharp
  public uint TotalMax { get; }
  ```
  The total max amount of currency.

- **`WeeklyMax Property`**
  ```csharp
  public uint WeeklyMax { get; }
  ```
  The amount of currency you can get in a week.

#### WoWCurrency Methods

- **`GetCurrencyById Method`**
  ```csharp
  public static WoWCurrency GetCurrencyById(
uint id
)
  ```
  Returns a WoWCurrency object containing a snapshot of 
            detailed info on a specific currency.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST1C1FCC4E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32ID of the currency.
  - **Returns:** See Also

- **`GetCurrencyByType Method`**
  ```csharp
  public static WoWCurrency GetCurrencyByType(
WoWCurrencyType type
)
  ```
  Returns a WoWCurrency object containing a snapshot of
            detailed info on a specific currency.
  - *type*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTFE23BB92_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWCurrencyTypecurrency type, check the WoWCurrencyType enumeration for
            different types.
  - **Returns:** ReferenceWoWCurrency Class

- **`GetDiscoveredCurrencies Method`**
  ```csharp
  public static IEnumerable&lt;WoWCurrency&gt; GetDiscoveredCurrencies()
  ```
  Gets all the currencies that have been discovered by the local player.
  - **Returns:** See Also

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceWoWCurrency Class

---

### WoWCurrencyType Enumeration

```csharp
public enum WoWCurrencyType
```

Values that represent WoWCurrencyType.

---

### WoWDescriptorQuest Structure

```csharp
public struct WoWDescriptorQuest
```

A WoW descriptor question.

#### WoWDescriptorQuest Fields

- **`Flags Field`**
  ```csharp
  public WoWDescriptorQuestFlags Flags
  ```
  The flags.

- **`Id Field`**
  ```csharp
  public uint Id
  ```
  The identifier.

- **`ObjectivesDone Field`**
  ```csharp
  public ushort[] ObjectivesDone
  ```
  The objectives done.

- **`SecondsBeforeFailed Field`**
  ```csharp
  public uint SecondsBeforeFailed
  ```
  The seconds before failed.

---

### WoWDescriptorQuestFlags Enumeration

```csharp
[FlagsAttribute]
public enum WoWDescriptorQuestFlags
```

Bitfield of flags for specifying WoWDescriptorQuestFlags.

---

### WoWDispelType Enumeration

```csharp
public enum WoWDispelType
```

Values that represent WoWDispelType.

---

### WoWGroupInfo Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWGroupInfo

```csharp
public class WoWGroupInfo
```

Information about the WoW group.

#### WoWGroupInfo Properties

- **`DungeonDifficultyId Property`**
  ```csharp
  public uint DungeonDifficultyId { get; }
  ```
  Gets the identifier of the dungeon difficulty.

- **`Flags Property`**
  ```csharp
  public WoWGroupInfo.GroupFlags Flags { get; }
  ```
  Gets the flags.

- **`GroupLeader Property`**
  ```csharp
  public WoWPlayer GroupLeader { get; }
  ```
  Gets the group leader.

- **`GroupLeaderGuid Property`**
  ```csharp
  public WoWGuid GroupLeaderGuid { get; }
  ```
  Gets a unique identifier of the group leader.

- **`GroupSize Property`**
  ```csharp
  public uint GroupSize { get; }
  ```
  Gets the size of the group.

- **`IsInBattlegroundParty Property`**
  ```csharp
  public bool IsInBattlegroundParty { get; }
  ```
  Gets a value indicating whether this object is in battleground party.

- **`IsInLfgParty Property`**
  ```csharp
  public bool IsInLfgParty { get; }
  ```
  Gets a value indicating whether this object is in lfg party.

- **`IsInParty Property`**
  ```csharp
  public bool IsInParty { get; }
  ```
  Gets a value indicating whether this object is in party.

- **`IsInRaid Property`**
  ```csharp
  public bool IsInRaid { get; }
  ```
  Gets a value indicating whether this object is in RAID.

- **`LfgDungeonId Property`**
  ```csharp
  public uint LfgDungeonId { get; }
  ```
  Gets the identifier of the lfg dungeon.

- **`NumPartyMembers Property`**
  ```csharp
  public int NumPartyMembers { get; }
  ```
  Gets the number of party members.

- **`NumRaidMembers Property`**
  ```csharp
  public int NumRaidMembers { get; }
  ```
  Gets the number of RAID members.

- **`PartyMemberGuids Property`**
  ```csharp
  public WoWGuid[] PartyMemberGuids { get; }
  ```
  Gets the party member guids.

- **`PartyMembers Property`**
  ```csharp
  public IEnumerable&lt;WoWPartyMember&gt; PartyMembers { get; }
  ```
  Gets the party members, including the local player.

- **`PartySize Property`**
  ```csharp
  public uint PartySize { get; }
  ```
  Gets the size of the party.

- **`RaidDifficultyId Property`**
  ```csharp
  public uint RaidDifficultyId { get; }
  ```
  Gets the identifier of the RAID difficulty.

- **`RaidMemberGuids Property`**
  ```csharp
  public WoWGuid[] RaidMemberGuids { get; }
  ```
  Gets the RAID member guids.

- **`RaidMembers Property`**
  ```csharp
  public IEnumerable&lt;WoWPartyMember&gt; RaidMembers { get; }
  ```
  Gets the RAID members.

#### WoWGroupInfo Methods

- **`ClearWorldMarker Method`**
  ```csharp
  public void ClearWorldMarker(
WoWGroupInfo.WorldMarker mark
)
  ```
  Clears the world marker described by mark.
  - *mark*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST63CB0CBB_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGroupInfoAddLanguageSpecificTextSet("LST63CB0CBB_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WorldMarkerThe mark.

- **`IsInCurrentParty Method`**
  ```csharp
  public bool IsInCurrentParty(
WoWGuid guid
)
  ```
  Query if 'guid' is in current party.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTECE414DC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidUnique identifier.
  - **Returns:** ReferenceWoWGroupInfo Class

- **`IsInCurrentRaid Method`**
  ```csharp
  public bool IsInCurrentRaid(
WoWGuid guid
)
  ```
  Query if 'guid' is in current RAID.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST3E15AFC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidUnique identifier.
  - **Returns:** ReferenceWoWGroupInfo Class

- **`PlaceWorldMarker Method`**
  ```csharp
  public void PlaceWorldMarker(
WoWGroupInfo.WorldMarker mark,
Vector3 location
)
  ```
  Place world marker.
  - *mark*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST7DE9EB94_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGroupInfoAddLanguageSpecificTextSet("LST7DE9EB94_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WorldMarker    The mark.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST7DE9EB94_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.

---

### WoWGroupInfo.GroupFlags Enumeration

```csharp
[FlagsAttribute]
public enum GroupFlags
```

Bitfield of flags for specifying GroupFlags.

---

### WoWGroupInfo.GroupMemberInfo Structure

```csharp
public struct GroupMemberInfo
```

Information about the group member.

#### GroupMemberInfo Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns the fully qualified type name of this instance.
  - **Returns:** ReferenceWoWGroupInfoAddLanguageSpecificTextSet("LSTE91AC157_2?cs=.|vb=.|cpp=::|nu=.|fs=.");GroupMemberInfo Structure

#### GroupMemberInfo Fields

- **`byte3 Field`**
  ```csharp
  public readonly byte byte3
  ```

- **`dword2C Field`**
  ```csharp
  public readonly uint dword2C
  ```

- **`dword30 Field`**
  ```csharp
  public readonly uint dword30
  ```

- **`dword34 Field`**
  ```csharp
  public readonly uint dword34
  ```

- **`dword38 Field`**
  ```csharp
  public readonly uint dword38
  ```

- **`dword3C Field`**
  ```csharp
  public readonly uint dword3C
  ```

- **`dword8 Field`**
  ```csharp
  public readonly uint dword8
  ```

- **`dwordC Field`**
  ```csharp
  public readonly uint dwordC
  ```

- **`Flags Field`**
  ```csharp
  public readonly byte Flags
  ```
  The flags.

- **`Group Field`**
  ```csharp
  public readonly byte Group
  ```
  The group.

- **`Guid Field`**
  ```csharp
  public readonly WoWGuid Guid
  ```
  Unique identifier.

- **`ParentGroupInfo Field`**
  ```csharp
  public readonly IntPtr ParentGroupInfo
  ```
  Information describing the parent group.

- **`Rank Field`**
  ```csharp
  public readonly uint Rank
  ```

- **`Role Field`**
  ```csharp
  public readonly byte Role
  ```
  The role.

---

### WoWGroupInfo.WorldMarker Enumeration

```csharp
public enum WorldMarker
```

Values that represent WorldMarker.

---

### WoWGuid Structure

```csharp
[SerializableAttribute]
public struct WoWGuid : IEquatable&lt;WoWGuid&gt;
```

Represents a GUID in WoW, which is a unique identifier for a WoW entity.

#### WoWGuid Properties

- **`Entry Property`**
  ```csharp
  public int Entry { get; }
  ```
  Gets the entry ID. This value is only valid for some types of GUIDs.

- **`IsValid Property`**
  ```csharp
  public bool IsValid { get; }
  ```
  Gets a bool that indicates whether this GUID is a valid GUID that represents an entity.

- **`RealmId Property`**
  ```csharp
  public int RealmId { get; }
  ```
  Gets the realm ID. This value is only valid for some types of GUIDs.

- **`Type Property`**
  ```csharp
  public WoWGuidType Type { get; }
  ```
  The type of entity that this GUID represents.

#### WoWGuid Methods

- **`Equals Method`**

- **`Equals Method (Object)`**
  ```csharp
  public override bool Equals(
Object obj
)
  ```
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LST7CD599D4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceWoWGuid Structure

- **`Equals Method (WoWGuid)`**
  ```csharp
  public bool Equals(
WoWGuid other
)
  ```
  - *other*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST36DB09D3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuid
  - **Returns:** See Also

- **`GetFriendlyString Method`**
  ```csharp
  public string GetFriendlyString()
  ```
  Gets a friendly representation of this GUID. This does an injection into WoW.
  - **Returns:** ReferenceWoWGuid Structure

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  - **Returns:** ReferenceWoWGuid Structure

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a string representation of this GUID.
  - **Returns:** The format returned is a 32 character long hex string, starting with Highest.

- **`TryParse Method`**
  ```csharp
  public static bool TryParse(
string input,
out WoWGuid result
)
  ```
  Tries parsing a GUID from a string.
  - *input*: Type: SystemAddLanguageSpecificTextSet("LST8EC4ADA6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe string.
  - *result*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST8EC4ADA6_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidAddLanguageSpecificTextSet("LST8EC4ADA6_3?cpp=%");The result.
  - **Returns:** Remarks

- **`TryParseFriendly Method`**
  ```csharp
  public static bool TryParseFriendly(
string input,
out WoWGuid result
)
  ```
  Tries parsing a GUID from a friendly string representation.
  - *input*: Type: SystemAddLanguageSpecificTextSet("LST4D385B63_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe input.
  - *result*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST4D385B63_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidAddLanguageSpecificTextSet("LST4D385B63_3?cpp=%");The result.
  - **Returns:** Remarks

#### WoWGuid Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
WoWGuid left,
WoWGuid right
)
  ```
  - *left*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTEC7B7A85_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuid
  - *right*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTEC7B7A85_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuid
  - **Returns:** ReferenceWoWGuid Structure

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
WoWGuid left,
WoWGuid right
)
  ```
  - *left*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTC4A69714_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuid
  - *right*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTC4A69714_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuid
  - **Returns:** ReferenceWoWGuid Structure

#### WoWGuid Fields

- **`Empty Field`**
  ```csharp
  public static readonly WoWGuid Empty
  ```
  Gets an empty GUID.

- **`Higher Field`**
  ```csharp
  public readonly uint Higher
  ```

- **`Highest Field`**
  ```csharp
  public readonly uint Highest
  ```

- **`Lower Field`**
  ```csharp
  public readonly uint Lower
  ```

- **`Lowest Field`**
  ```csharp
  public readonly uint Lowest
  ```

---

### WoWGuidType Enumeration

```csharp
public enum WoWGuidType
```

---

### WoWLandMark Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWLandMark → Styx.WoWInternals.AreaPoiLandmark → Styx.WoWInternals.ResearchSiteLandmark

```csharp
public abstract class WoWLandMark
```

A WoW land mark.

#### WoWLandMark Properties

- **`AreaId Property`**
  ```csharp
  public abstract int AreaId { get; }
  ```
  Gets the identifier of the area.

- **`Data Property`**
  ```csharp
  public LandMarkEntry Data { get; }
  ```
  Gets the data for this LandMarkEntry.

- **`Description Property`**
  ```csharp
  public abstract string Description { get; }
  ```
  Gets the description.

- **`Entry Property`**
  ```csharp
  public uint Entry { get; }
  ```
  The data of this landmark.

- **`Index Property`**
  ```csharp
  public int Index { get; }
  ```
  The index of this landmark. This can be used with GetMapLandmarkInfo by adding 1.

- **`IsValid Property`**
  ```csharp
  public virtual bool IsValid { get; }
  ```
  Returns true if this landmark is a valid object.

- **`Location Property`**
  ```csharp
  public abstract Vector2 Location { get; }
  ```
  Gets the location.

- **`MapId Property`**
  ```csharp
  public abstract int MapId { get; }
  ```
  Gets the identifier of the map.

- **`MapX Property`**
  ```csharp
  public float MapX { get; }
  ```
  The Map-X Location of this landmark.

- **`MapY Property`**
  ```csharp
  public float MapY { get; }
  ```
  The Map-Y Location of this landmark.

- **`Name Property`**
  ```csharp
  public abstract string Name { get; }
  ```
  Gets the name.

- **`NormalIcon Property`**
  ```csharp
  public abstract int NormalIcon { get; }
  ```
  Gets the normal icon.

- **`ShowInBattleMap Property`**
  ```csharp
  public abstract bool ShowInBattleMap { get; }
  ```
  Gets a value indicating whether the in battle map is shown.

- **`ShowInBg Property`**
  ```csharp
  public abstract bool ShowInBg { get; }
  ```
  Gets a value indicating whether the in background is shown.

- **`ShowInZone Property`**
  ```csharp
  public abstract bool ShowInZone { get; }
  ```
  Gets a value indicating whether the in zone is shown.

- **`TextureIndex Property`**
  ```csharp
  public abstract int TextureIndex { get; }
  ```
  Gets the zero-based index of the texture.

- **`Type Property`**
  ```csharp
  public LandmarkType Type { get; }
  ```
  Gets the type of this landmark.

- **`WorldState Property`**
  ```csharp
  public abstract uint WorldState { get; }
  ```
  Gets the state of the world.

#### WoWLandMark Methods

- **`ToAlteracValleyLandmark Method`**
  ```csharp
  public AlteracValleyLandmark ToAlteracValleyLandmark()
  ```
  Converts this object to an alterac valley landmark.
  - **Returns:** ReferenceWoWLandMark Class

- **`ToArathiBasinLandmark Method`**
  ```csharp
  public ArathiBasinLandmark ToArathiBasinLandmark()
  ```
  Converts this object to an arathi basin landmark.
  - **Returns:** ReferenceWoWLandMark Class

- **`ToBattleForGilneasLandmark Method`**
  ```csharp
  public BattleForGilneasLandmark ToBattleForGilneasLandmark()
  ```
  Converts this object to a battle for gilneas landmark.
  - **Returns:** ReferenceWoWLandMark Class

- **`ToDeepwindGorgeLandmark Method`**
  ```csharp
  public DeepwindGorgeLandmark ToDeepwindGorgeLandmark()
  ```
  Converts this object to a deepwind gorge landmark.
  - **Returns:** ReferenceWoWLandMark Class

- **`ToEyeOfTheStormLandmark Method`**
  ```csharp
  public EyeOfTheStormLandmark ToEyeOfTheStormLandmark()
  ```
  Converts this object to an eye of the storm landmark.
  - **Returns:** ReferenceWoWLandMark Class

- **`ToIsleOfConquestLandmark Method`**
  ```csharp
  public IsleOfConquestLandmark ToIsleOfConquestLandmark()
  ```
  Converts this object to an isle of conquest landmark.
  - **Returns:** ReferenceWoWLandMark Class

- **`ToResearchSiteLandmark Method`**
  ```csharp
  public ResearchSiteLandmark ToResearchSiteLandmark()
  ```
  Converts this object to a research site landmark.
  - **Returns:** ReferenceWoWLandMark Class

- **`ToStrandOfTheAncientsLandmark Method`**
  ```csharp
  public StrandOfTheAncientsLandmark ToStrandOfTheAncientsLandmark()
  ```
  Converts this object to a strand of the ancients landmark.
  - **Returns:** ReferenceWoWLandMark Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceWoWLandMark Class

---

### WoWMissile Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWMissile

```csharp
public class WoWMissile
```

A WoW missile.

#### WoWMissile Properties

- **`AllMissiles Property`**
  ```csharp
  public static IEnumerable&lt;WoWMissile&gt; AllMissiles { get; }
  ```
  Gets all missiles.

- **`BaseAddress Property`**
  ```csharp
  public IntPtr BaseAddress { get; }
  ```
  Gets the base address.

- **`Caster Property`**
  ```csharp
  public WoWUnit Caster { get; }
  ```
  Gets the caster.

- **`CasterGuid Property`**
  ```csharp
  public WoWGuid CasterGuid { get; }
  ```
  Gets a unique identifier of the caster.

- **`FirePosition Property`**
  ```csharp
  public Vector3 FirePosition { get; }
  ```
  Gets the fire position.

- **`Flags Property`**
  ```csharp
  public uint Flags { get; }
  ```
  Gets the flags.

- **`ImpactedMissiles Property`**
  ```csharp
  public static IEnumerable&lt;WoWMissile&gt; ImpactedMissiles { get; }
  ```
  Gets the impacted missiles.

- **`ImpactPosition Property`**
  ```csharp
  public Vector3 ImpactPosition { get; }
  ```
  Gets the impact position.

- **`InFlightMissiles Property`**
  ```csharp
  public static IEnumerable&lt;WoWMissile&gt; InFlightMissiles { get; }
  ```
  Gets the in flight missiles.

- **`Position Property`**
  ```csharp
  public Vector3 Position { get; }
  ```
  Gets the position.

- **`Spell Property`**
  ```csharp
  public WoWSpell Spell { get; }
  ```
  Gets the spell.

- **`SpellId Property`**
  ```csharp
  public int SpellId { get; }
  ```
  Gets the identifier of the spell.

- **`SpellVisualId Property`**
  ```csharp
  public int SpellVisualId { get; }
  ```
  Gets the identifier of the spell visual.

- **`Target Property`**
  ```csharp
  public WoWUnit Target { get; }
  ```
  Gets the Target for the.

- **`TargetGuid Property`**
  ```csharp
  public WoWGuid TargetGuid { get; }
  ```
  Gets a unique identifier of the target.

#### WoWMissile Fields

- **`_inFlightMissiles Field`**
  ```csharp
  public static PerFrameCachedValue&lt;List&lt;WoWMissile&gt;&gt; _inFlightMissiles
  ```

---

### WoWMovement Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWMovement

```csharp
public static class WoWMovement
```

Provides functions for moving in WoW.

#### WoWMovement Properties

- **`ActiveInputControl Property`**
  ```csharp
  public static WoWMovement.InputControl ActiveInputControl { get; }
  ```
  The current active input control.

- **`ActiveMover Property`**
  ```csharp
  public static WoWUnit ActiveMover { get; }
  ```
  Gets the active mover.

- **`ActiveMoverGuid Property`**
  ```csharp
  public static WoWGuid ActiveMoverGuid { get; }
  ```
  Gets the active mover GUID.

- **`ClickToMoveInfo Property`**
  ```csharp
  public static ClickToMoveInfo ClickToMoveInfo { get; }
  ```
  Provides information about the current click to move.

- **`IsFacing Property`**
  ```csharp
  public static bool IsFacing { get; }
  ```
  Returns a value indicating if you current are constant facing with CTM.

#### WoWMovement Methods

- **`CalculatePointFrom Method`**
  ```csharp
  public static Vector3 CalculatePointFrom(
Vector3 target,
float distance
)
  ```
  Calculates a point at a given distance from the target point, using the local players
            position as the start position.
  - *target*: Type: System.NumericsAddLanguageSpecificTextSet("LSTD353EB71_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3  The target.
  - *distance*: Type: SystemAddLanguageSpecificTextSet("LSTD353EB71_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe distance.
  - **Returns:** ReferenceWoWMovement Class

- **`ClickToMove Method`**
  Uses Click-To-Move to move to a point in-game.

- **`ClickToMove Method (Vector3)`**
  ```csharp
  public static void ClickToMove(
Vector3 point
)
  ```
  Uses Click-To-Move to move to a point in-game.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LST89F493E8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The destination point.

- **`ClickToMove Method (Vector3, Single)`**
  ```csharp
  public static void ClickToMove(
Vector3 point,
float distance
)
  ```
  Uses Click-To-Move to move to a point ingame. An additional parameter defines at what
            distance to the point the click is performed.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LST9454BBAF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3   The destination point.
  - *distance*: Type: SystemAddLanguageSpecificTextSet("LST9454BBAF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe distance to the destination point to move to - if you want to
            stop at 5 yards before reaching the destination, you would specify 5 here.

- **`ClickToMove Method (WoWGuid, WoWMovement.ClickToMoveType)`**
  ```csharp
  public static void ClickToMove(
WoWGuid guid,
WoWMovement.ClickToMoveType type
)
  ```
  Uses Click-To-Move to perform object based in-game interaction.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST49757CBF_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidThe GUID.
  - *type*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST49757CBF_4?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWMovementAddLanguageSpecificTextSet("LST49757CBF_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ClickToMoveTypeThe type.

- **`ClickToMove Method (Single, Single, Single)`**
  ```csharp
  public static void ClickToMove(
float x,
float y,
float z
)
  ```
  Uses Click-To-Move to move to a point in-game.
  - *x*: Type: SystemAddLanguageSpecificTextSet("LST80BD8DB2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe X unit of the point.
  - *y*: Type: SystemAddLanguageSpecificTextSet("LST80BD8DB2_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe Y unit of the point.
  - *z*: Type: SystemAddLanguageSpecificTextSet("LST80BD8DB2_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe Z unit of the point.

- **`ClickToMove Method (WoWGuid, Vector3, WoWMovement.ClickToMoveType)`**
  ```csharp
  public static void ClickToMove(
WoWGuid guid,
Vector3 loc,
WoWMovement.ClickToMoveType type
)
  ```
  Uses Click-To-Move to perform in-game interaction.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTD04B71A6_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidThe GUID.
  - *loc*: Type: System.NumericsAddLanguageSpecificTextSet("LSTD04B71A6_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3 The loc.
  - *type*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTD04B71A6_5?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWMovementAddLanguageSpecificTextSet("LSTD04B71A6_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ClickToMoveTypeThe type.

- **`ClickToMove Method (WoWGuid, Vector3, Single, WoWMovement.ClickToMoveType)`**
  ```csharp
  public static void ClickToMove(
WoWGuid guid,
Vector3 loc,
float facing,
WoWMovement.ClickToMoveType type
)
  ```
  Uses Click-To-Move to move to a point in-game.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTFEB00355_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuid  .
  - *loc*: Type: System.NumericsAddLanguageSpecificTextSet("LSTFEB00355_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3   .
  - *facing*: Type: SystemAddLanguageSpecificTextSet("LSTFEB00355_5?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe facing.
  - *type*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTFEB00355_6?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWMovementAddLanguageSpecificTextSet("LSTFEB00355_7?cs=.|vb=.|cpp=::|nu=.|fs=.");ClickToMoveType  .

- **`ConstantFace Method`**
  ```csharp
  public static void ConstantFace(
WoWGuid guid
)
  ```
  Faces an object by GUID, and keeps facing it until told to stop.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTFA533FBF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidGUID of the object to face.

- **`ConstantFaceStop Method`**
  ```csharp
  public static void ConstantFaceStop(
WoWGuid guid
)
  ```
  Stops constant facing a.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTC02AD78F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidGUID of object to stop facing.

- **`Face Method`**
  Constant faces the target using CTM.

- **`Face Method`**
  ```csharp
  public static void Face()
  ```
  Constant faces the target using CTM.

- **`Face Method (WoWGuid)`**
  ```csharp
  public static void Face(
WoWGuid guid
)
  ```
  Faces an object based on GUID.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTF4557351_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidThe GUID of the object.

- **`GetHeadingDiff Method`**
  ```csharp
  public static void GetHeadingDiff(
double currentHeading,
double destHeading,
out double headingDiff,
out int directionCoeff
)
  ```
  Finds the heading different of two different headings.
  - *currentHeading*: Type: SystemAddLanguageSpecificTextSet("LST7466645_1?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe current heading.
  - *destHeading*: Type: SystemAddLanguageSpecificTextSet("LST7466645_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Double   The dest heading.
  - *headingDiff*: Type: SystemAddLanguageSpecificTextSet("LST7466645_3?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleAddLanguageSpecificTextSet("LST7466645_4?cpp=%");   [out] The heading diff.
  - *directionCoeff*: Type: SystemAddLanguageSpecificTextSet("LST7466645_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LST7466645_6?cpp=%");[out] The direction coeff.

- **`Move Method`**
  Sets a control bit.

- **`Move Method (WoWMovement.MovementDirection)`**
  ```csharp
  public static void Move(
WoWMovement.MovementDirection direction
)
  ```
  Sets a control bit.
  - *direction*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTEBDB6961_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWMovementAddLanguageSpecificTextSet("LSTEBDB6961_4?cs=.|vb=.|cpp=::|nu=.|fs=.");MovementDirectionThe direction.

- **`Move Method (WoWMovement.MovementDirection, Int32)`**
  ```csharp
  public static void Move(
WoWMovement.MovementDirection dir,
int durationMs
)
  ```
  Moves in the specified direction for a specified duration.
  - *dir*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTCAEF7D86_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWMovementAddLanguageSpecificTextSet("LSTCAEF7D86_4?cs=.|vb=.|cpp=::|nu=.|fs=.");MovementDirectionThe direction.
  - *durationMs*: Type: SystemAddLanguageSpecificTextSet("LSTCAEF7D86_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The duration to move in the specified direction, in milliseconds.

- **`Move Method (WoWMovement.MovementDirection, TimeSpan)`**
  ```csharp
  public static void Move(
WoWMovement.MovementDirection dir,
TimeSpan duration
)
  ```
  Moves in the specified direction for a specified duration.
  - *dir*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTDA915B43_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWMovementAddLanguageSpecificTextSet("LSTDA915B43_4?cs=.|vb=.|cpp=::|nu=.|fs=.");MovementDirectionThe direction.
  - *duration*: Type: SystemAddLanguageSpecificTextSet("LSTDA915B43_5?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe duration to move in the specified direction.

- **`MoveStop Method`**
  Makes you stop moving.

- **`MoveStop Method`**
  ```csharp
  public static void MoveStop()
  ```
  Makes you stop moving.

- **`MoveStop Method (WoWMovement.MovementDirection)`**
  ```csharp
  public static void MoveStop(
WoWMovement.MovementDirection direction
)
  ```
  Unsets a control bit flag.
  - *direction*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST3E7754D1_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWMovementAddLanguageSpecificTextSet("LST3E7754D1_4?cs=.|vb=.|cpp=::|nu=.|fs=.");MovementDirectionThe direction.

- **`Pulse Method`**
  ```csharp
  public static void Pulse()
  ```
  Pulses this instance.

- **`StopFace Method`**
  ```csharp
  public static void StopFace()
  ```
  Stops facing the current target.

---

### WoWMovement.ClickToMoveType Enumeration

```csharp
public enum ClickToMoveType
```

Values that represent ClickToMoveType.

---

### WoWMovement.InputControl Structure

```csharp
public struct InputControl
```

An input control.

#### InputControl Fields

- **`Flags Field`**
  ```csharp
  public WoWMovement.MovementDirection Flags
  ```
  The flags.

- **`Movement Field`**
  ```csharp
  public WoWMovement.MovementControl Movement
  ```
  The movement.

- **`Time Field`**
  ```csharp
  public uint Time
  ```
  The time.

---

### WoWMovement.MovementControl Structure

```csharp
public struct MovementControl
```

A movement control.

---

### WoWMovement.MovementDirection Enumeration

```csharp
[FlagsAttribute]
public enum MovementDirection
```

Bitfield of flags for specifying MovementDirection.

---

### WoWMovementInfo Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWSimpleMovementInfo → Styx.WoWInternals.WoWMovementInfo

```csharp
public class WoWMovementInfo : WoWSimpleMovementInfo
```

Information about the WoW movement.

#### WoWMovementInfo Properties

- **`BackwardSpeed Property`**
  ```csharp
  public float BackwardSpeed { get; }
  ```
  Gets the backward speed.

- **`CanFly Property`**
  ```csharp
  public bool CanFly { get; }
  ```
  This value indicates whether we are ready to fly (ready to liftoff).
            		 N.B.: This property will return 'false' if we are able to fly, but not mounted and ready for liftoff.
            		 If you need to know merely whether the toon is *eligible* to fly at the current location, use CanFly.

- **`CurrentSpeed Property`**
  ```csharp
  public float CurrentSpeed { get; }
  ```
  Gets the current speed.

- **`FallingFar Property`**
  ```csharp
  public bool FallingFar { get; }
  ```
  Gets a value indicating whether the falling far.

- **`FallStartHeight Property`**
  ```csharp
  public float FallStartHeight { get; }
  ```
  Gets the height of the fall start.

- **`FallTime Property`**
  ```csharp
  public uint FallTime { get; }
  ```
  Gets the fall time.

- **`FlyingBackwardSpeed Property`**
  ```csharp
  public float FlyingBackwardSpeed { get; }
  ```
  Gets the flying backward speed.

- **`FlyingForwardSpeed Property`**
  ```csharp
  public float FlyingForwardSpeed { get; }
  ```
  Gets the flying forward speed.

- **`ForwardSpeed Property`**
  ```csharp
  public float ForwardSpeed { get; }
  ```
  Gets the forward speed.

- **`GroundNormal Property`**
  ```csharp
  public Vector3 GroundNormal { get; }
  ```
  Gets the ground normal.

- **`HoverHeight Property`**
  ```csharp
  public float HoverHeight { get; }
  ```
  Gets the height of the hover.

- **`IsAscending Property`**
  ```csharp
  public bool IsAscending { get; }
  ```
  Gets a value indicating whether this object is ascending.

- **`IsDescending Property`**
  ```csharp
  public bool IsDescending { get; }
  ```
  Gets a value indicating whether this object is descending.

- **`IsFalling Property`**
  ```csharp
  public bool IsFalling { get; }
  ```
  Gets a value indicating whether this object is falling.

- **`IsFlying Property`**
  ```csharp
  public bool IsFlying { get; }
  ```
  Gets a value indicating whether this object is flying.

- **`IsMoving Property`**
  ```csharp
  public bool IsMoving { get; }
  ```
  Gets a value indicating whether this object is moving.

- **`IsStrafing Property`**
  ```csharp
  public bool IsStrafing { get; }
  ```
  Gets a value indicating whether this object is strafing.

- **`IsSwimming Property`**
  ```csharp
  public bool IsSwimming { get; }
  ```
  Gets a value indicating whether this object is swimming.

- **`JumpingOrShortFalling Property`**
  ```csharp
  public bool JumpingOrShortFalling { get; }
  ```
  Gets a value indicating whether the jumping or short falling.

- **`LastFallHeight Property`**
  ```csharp
  public float LastFallHeight { get; }
  ```
  Gets the height of the last fall.

- **`MovementFlags Property`**
  ```csharp
  public MoveFlags MovementFlags { get; }
  ```
  Gets the movement flags.

- **`MovingBackward Property`**
  ```csharp
  public bool MovingBackward { get; }
  ```
  Gets a value indicating whether the moving backward.

- **`MovingForward Property`**
  ```csharp
  public bool MovingForward { get; }
  ```
  Gets a value indicating whether the moving forward.

- **`MovingStrafeLeft Property`**
  ```csharp
  public bool MovingStrafeLeft { get; }
  ```
  Gets a value indicating whether the moving strafe left.

- **`MovingStrafeRight Property`**
  ```csharp
  public bool MovingStrafeRight { get; }
  ```
  Gets a value indicating whether the moving strafe right.

- **`MovingTurnLeft Property`**
  ```csharp
  public bool MovingTurnLeft { get; }
  ```
  Gets a value indicating whether the moving turn left.

- **`MovingTurnRight Property`**
  ```csharp
  public bool MovingTurnRight { get; }
  ```
  Gets a value indicating whether the moving turn right.

- **`RunSpeed Property`**
  ```csharp
  public float RunSpeed { get; }
  ```
  Gets the run speed.

- **`SwimmingBackwardSpeed Property`**
  ```csharp
  public float SwimmingBackwardSpeed { get; }
  ```
  Gets the swimming backward speed.

- **`SwimmingForwardSpeed Property`**
  ```csharp
  public float SwimmingForwardSpeed { get; }
  ```
  Gets the swimming forward speed.

- **`TimeMoved Property`**
  ```csharp
  public uint TimeMoved { get; }
  ```
  Gets the time moved.

- **`TurnSpeed Property`**
  ```csharp
  public float TurnSpeed { get; }
  ```
  Gets the turn rate in radians per second.

#### WoWMovementInfo Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  public override string ToString()
            {
                return ((MoveFlags)MovementFlags).ToString();
            }
  - **Returns:** ReferenceWoWMovementInfo Class

---

### WoWPaperDoll Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWBag → Styx.WoWInternals.WoWPaperDoll

```csharp
public class WoWPaperDoll : WoWBag
```

A wow paper doll.

#### WoWPaperDoll Properties

- **`Back Property`**
  ```csharp
  public WoWItem Back { get; }
  ```
  Gets the back.

- **`Bag1 Property`**
  ```csharp
  public WoWItem Bag1 { get; }
  ```
  Gets the bag 1.

- **`Bag2 Property`**
  ```csharp
  public WoWItem Bag2 { get; }
  ```
  Gets the bag 2.

- **`Bag3 Property`**
  ```csharp
  public WoWItem Bag3 { get; }
  ```
  Gets the bag 3.

- **`Bag4 Property`**
  ```csharp
  public WoWItem Bag4 { get; }
  ```
  Gets the bag 4.

- **`Chest Property`**
  ```csharp
  public WoWItem Chest { get; }
  ```
  Gets the chest.

- **`Feet Property`**
  ```csharp
  public WoWItem Feet { get; }
  ```
  Gets the feet.

- **`Finger1 Property`**
  ```csharp
  public WoWItem Finger1 { get; }
  ```
  Gets the finger 1.

- **`Finger2 Property`**
  ```csharp
  public WoWItem Finger2 { get; }
  ```
  Gets the finger 2.

- **`Hands Property`**
  ```csharp
  public WoWItem Hands { get; }
  ```
  Gets the hands.

- **`Head Property`**
  ```csharp
  public WoWItem Head { get; }
  ```
  Gets the head.

- **`Legs Property`**
  ```csharp
  public WoWItem Legs { get; }
  ```
  Gets the legs.

- **`MainHand Property`**
  ```csharp
  public WoWItem MainHand { get; }
  ```
  Gets the main hand.

- **`Neck Property`**
  ```csharp
  public WoWItem Neck { get; }
  ```
  Gets the neck.

- **`OffHand Property`**
  ```csharp
  public WoWItem OffHand { get; }
  ```
  Gets the off hand.

- **`Ranged Property`**
  ```csharp
  public WoWItem Ranged { get; }
  ```
  Gets the ranged.

- **`Shirt Property`**
  ```csharp
  public WoWItem Shirt { get; }
  ```
  Gets the shirt.

- **`Shoulder Property`**
  ```csharp
  public WoWItem Shoulder { get; }
  ```
  Gets the shoulder.

- **`Tabard Property`**
  ```csharp
  public WoWItem Tabard { get; }
  ```
  Gets the tabard.

- **`Trinket1 Property`**
  ```csharp
  public WoWItem Trinket1 { get; }
  ```
  Gets the trinket 1.

- **`Trinket2 Property`**
  ```csharp
  public WoWItem Trinket2 { get; }
  ```
  Gets the trinket 2.

- **`Waist Property`**
  ```csharp
  public WoWItem Waist { get; }
  ```
  Gets the waist.

- **`Wrist Property`**
  ```csharp
  public WoWItem Wrist { get; }
  ```
  Gets the wrist.

#### WoWPaperDoll Methods

- **`GetEquippedItem Method`**
  ```csharp
  public WoWItem GetEquippedItem(
WoWInventorySlot slot
)
  ```
  Gets equipped item.
  - *slot*: Type: StyxAddLanguageSpecificTextSet("LSTD10247C4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWInventorySlotThe slot.
  - **Returns:** ReferenceWoWPaperDoll Class

---

### WoWPetBattleState Enumeration

```csharp
public enum WoWPetBattleState
```

Values that represent WoWPetBattleState.

---

### WoWPetControl Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWPetControl

```csharp
public class WoWPetControl
```

Initializes a new instance of the WoWPetControl class

#### WoWPetControl Properties

- **`CanPetBeDismissed Property`**
  ```csharp
  public static bool CanPetBeDismissed { get; }
  ```
  Determines whether the user's pet can be dismissed.
  - **Returns:** See Also

#### WoWPetControl Methods

- **`Attack Method`**
  ```csharp
  public static void Attack(
WoWUnit wowUnit
)
  ```
  Sends the user's pet to attack the target identified by WOWUNIT.
  - *wowUnit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST3CFF9D2F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe wow unit.

- **`CanCastAction Method`**
  ```csharp
  public static bool CanCastAction(
WoWPetSpell petAction
)
  ```
  Returns true if you can cast the PETACTION; otherwise, false.This method checks for both spell existence, and if the spell is on cooldown.
                    Notes:
                    
                                    * To return 'true', the PETACTION spell must be hot-barred!  Existence in the spellbook
                                    is insufficient.  This is a limitation of the WoWclient and HB APIs.
  - *petAction*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST8244235C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPetSpellThe pet action.
  - **Returns:** ReferenceWoWPetControl Class

- **`CastAction Method`**
  ```csharp
  public static void CastAction(
WoWPetSpell action,
WoWUnit wowUnit = null
)
  ```
  Casts the PETACTION on WOWUNIT.
                    Notes:
                    
                                    * The PETACTION spell must be hot-barred!  Existence in the spellbook
                                    is insufficient.  This is a limitation of the WoWclient and HB APIs.
                                * If PETACTION is on cooldown, no action is performed.
  - *action*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTFDAEAB8D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPetSpellThe pet action to cast.

- **`DismissPet Method`**
  ```csharp
  public static Task&lt;bool&gt; DismissPet()
  ```
  Dismisses the users pet
  - **Returns:** See Also

- **`FindActionByName Method`**
  ```csharp
  public static WoWPetSpell FindActionByName(
string actionName
)
  ```
  Finds a pet action but naStyxWoW.Me.
  - *actionName*: Type: SystemAddLanguageSpecificTextSet("LSTA3826975_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the pet action.
  - **Returns:** ReferenceWoWPetControl Class

- **`Follow Method`**
  ```csharp
  public static void Follow()
  ```
  Instructs the user's pet to follow its owner.

- **`GetActionName Method`**
  ```csharp
  public static string GetActionName(
WoWPetSpell action
)
  ```
  Gets the name of the pet action.
  - *action*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST4D0E3F06_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPetSpellThe pet action.
  - **Returns:** ReferenceWoWPetControl Class

- **`IsActionActive Method`**
  ```csharp
  public static bool IsActionActive(
WoWPetSpell petAction
)
  ```
  Returns true if the provided pet action is being executed by the user's pet such as the 'Follow' command
  - *petAction*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST9EC60969_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPetSpellThe pet action.
  - **Returns:** See Also

- **`SetStance Method`**
  ```csharp
  public static void SetStance(
PetStance stance
)
  ```
  Instructs the user's pet to assume the specified stance
  - *stance*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST52732C8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");PetStance

---

### WoWPetSpell Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWPetSpell

```csharp
public class WoWPetSpell
```

Defines a pet "action" spell. (From the action bar. All known pet actions.)

#### WoWPetSpell Properties

- **`Action Property`**
  ```csharp
  public WoWPetSpell.PetAction Action { get; }
  ```
  Gets the action type.

- **`ActionBarIndex Property`**
  ```csharp
  public int ActionBarIndex { get; }
  ```
  Gets the zero-based index of the action bar, where this spell resides.

- **`Cooldown Property`**
  ```csharp
  public bool Cooldown { get; }
  ```
  Gets a value indicating whether the spell is on cooldown. Only valid if SpellType is
            "Spell".

- **`Spell Property`**
  ```csharp
  public WoWSpell Spell { get; }
  ```
  Gets the actual spell, if SpellType is "Spell".

- **`SpellType Property`**
  ```csharp
  public WoWPetSpell.PetSpellType SpellType { get; }
  ```
  Returns the type of spell this WoWPetSpell is for.

- **`Stance Property`**
  ```csharp
  public WoWPetSpell.PetStance Stance { get; }
  ```
  Gets the stance this spell sets the pet into.

#### WoWPetSpell Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents this instance.
  - **Returns:** ExceptionConditionArgumentOutOfRangeExceptionThrown when one or more arguments are outside the
            required range.

---

### WoWPetSpell.PetAction Enumeration

```csharp
public enum PetAction
```

Values that represent PetAction.

---

### WoWPetSpell.PetSpellType Enumeration

```csharp
public enum PetSpellType
```

Values that represent PetSpellType.

---

### WoWPetSpell.PetStance Enumeration

```csharp
public enum PetStance
```

Values that represent PetStance.

---

### WoWPlayerInventory Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWBag → Styx.WoWInternals.WoWPlayerInventory

```csharp
public class WoWPlayerInventory : WoWBag
```

A WoW player inventory.

#### WoWPlayerInventory Properties

- **`Backpack Property`**
  ```csharp
  public WoWBag Backpack { get; }
  ```
  Gets a bag representing the backpack.

- **`Bank Property`**
  ```csharp
  public WoWBag Bank { get; }
  ```
  Gets a bag representing the bank.

- **`BankBags Property`**
  ```csharp
  public WoWBag BankBags { get; }
  ```
  Gets a bag representing the 7 bank bag slots.

- **`Buyback Property`**
  ```csharp
  public WoWBag Buyback { get; }
  ```
  Gets a bag representing the buyback frame.

- **`Equipped Property`**
  ```csharp
  public WoWPaperDoll Equipped { get; }
  ```
  Gets a bag representing all equipped items. The last 4 slots are the equipped bags.

- **`ReagentBank Property`**
  ```csharp
  public WoWBag ReagentBank { get; }
  ```
  Gets the reagent bank.

---

### WoWQuestPOIInfo Structure

```csharp
public struct WoWQuestPOIInfo
```

Indicates whether this instance and a specified object are equal.

#### WoWQuestPOIInfo Fields

- **`QuestID Field`**
  ```csharp
  public int QuestID
  ```

- **`Steps Field`**
  ```csharp
  public WoWQuestStepsCollection Steps
  ```

---

### WoWQuestState Enumeration

```csharp
public enum WoWQuestState
```

Values that represent WoWQuestState.

---

### WoWQuestStep Structure

```csharp
public struct WoWQuestStep
```

A WoW question step.

#### WoWQuestStep Properties

- **`AreaPoints Property`**
  ```csharp
  public Vector2[] AreaPoints { get; }
  ```
  Gets the area points.

#### WoWQuestStep Fields

- **`PoiFloorID Field`**
  ```csharp
  public int PoiFloorID
  ```

- **`PoiID Field`**
  ```csharp
  public uint PoiID
  ```

- **`PoiMapAreaID Field`**
  ```csharp
  public int PoiMapAreaID
  ```

- **`PoiMapID Field`**
  ```csharp
  public int PoiMapID
  ```

- **`PoiObjectiveIndex Field`**
  ```csharp
  public int PoiObjectiveIndex
  ```

- **`StepPositionX Field`**
  ```csharp
  public int StepPositionX
  ```

- **`StepPositionY Field`**
  ```csharp
  public int StepPositionY
  ```

- **`WorldEffectId Field`**
  ```csharp
  public int WorldEffectId
  ```

---

### WoWQuestStepsCollection Structure

```csharp
public struct WoWQuestStepsCollection
```

Collection of WoW question steps.

#### WoWQuestStepsCollection Properties

- **`Steps Property`**
  ```csharp
  public WoWQuestStep[] Steps { get; }
  ```
  Gets the steps.

#### WoWQuestStepsCollection Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns the fully qualified type name of this instance.
  - **Returns:** ReferenceWoWQuestStepsCollection Structure

#### WoWQuestStepsCollection Fields

- **`StepsCount Field`**
  ```csharp
  public uint StepsCount
  ```
  Number of steps.

---

### WoWSimpleMovementInfo Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWSimpleMovementInfo → Styx.WoWInternals.WoWMovementInfo

```csharp
public class WoWSimpleMovementInfo
```

Gets the heading.

#### WoWSimpleMovementInfo Properties

- **`Heading Property`**
  ```csharp
  public float Heading { get; }
  ```
  Gets the heading.

- **`IsValid Property`**
  ```csharp
  public bool IsValid { get; }
  ```
  Gets a value indicating whether this object is valid.

- **`Pitch Property`**
  ```csharp
  public float Pitch { get; }
  ```
  Gets the pitch.

- **`Position Property`**
  ```csharp
  public Vector3 Position { get; }
  ```
  Gets the position.

- **`TransportGuid Property`**
  ```csharp
  public WoWGuid TransportGuid { get; }
  ```
  Gets a unique identifier of the transport.

---

### WoWSkill Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWSkill

```csharp
public class WoWSkill
```

A WoW skill.

#### WoWSkill Properties

- **`Bonus Property`**
  ```csharp
  public uint Bonus { get; }
  ```
  Gets the bonus.

- **`CanLink Property`**
  ```csharp
  public bool CanLink { get; }
  ```
  Gets a value indicating whether we can link.

- **`CategoryId Property`**
  ```csharp
  public SkillLineCategory CategoryId { get; }
  ```
  Gets the identifier of the category.

- **`CurrentValue Property`**
  ```csharp
  public int CurrentValue { get; }
  ```
  Gets the current value.

- **`Id Property`**
  ```csharp
  public int Id { get; }
  ```
  Gets the identifier.

- **`IsValid Property`**
  ```csharp
  public bool IsValid { get; }
  ```
  Gets a value indicating whether this object is valid.

- **`MaxValue Property`**
  ```csharp
  public int MaxValue { get; }
  ```
  Gets the maximum value.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`SpellIcon Property`**
  ```csharp
  public int SpellIcon { get; }
  ```
  Gets the spell icon.

#### WoWSkill Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceWoWSkill Class

---

### WoWSpell Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWSpell

```csharp
public class WoWSpell : IEquatable&lt;WoWSpell&gt;
```

A WoW spell.

#### WoWSpell Properties

- **`Attributes Property`**
  ```csharp
  public SpellAttributes Attributes { get; }
  ```
  Gets the attributes of this spell.

- **`AttributesEx Property`**
  ```csharp
  public SpellAttributesEx AttributesEx { get; }
  ```
  Gets the extended attributes of this spell.

- **`AttributesEx2 Property`**
  ```csharp
  public SpellAttributesEx2 AttributesEx2 { get; }
  ```
  Gets the extended attributes 2 of this spell.

- **`AttributesEx3 Property`**
  ```csharp
  public SpellAttributesEx3 AttributesEx3 { get; }
  ```
  Gets the extended attributes 3 of this spell.

- **`AttributesEx4 Property`**
  ```csharp
  public SpellAttributesEx4 AttributesEx4 { get; }
  ```
  Gets the extended attributes 4 of this spell.

- **`AttributesEx5 Property`**
  ```csharp
  public SpellAttributesEx5 AttributesEx5 { get; }
  ```
  Gets the extended attributes 5 of this spell.

- **`AttributesEx6 Property`**
  ```csharp
  public SpellAttributesEx6 AttributesEx6 { get; }
  ```
  Gets the extended attributes 6 of this spell.

- **`AttributesEx7 Property`**
  ```csharp
  public SpellAttributesEx7 AttributesEx7 { get; }
  ```
  Gets the extended attributes 7 of this spell.

- **`AttributesEx8 Property`**
  ```csharp
  public SpellAttributesEx8 AttributesEx8 { get; }
  ```
  Gets the extended attributes 8 of this spell.

- **`AuraDescription Property`**
  ```csharp
  public string AuraDescription { get; }
  ```
  Returns the aura description of this spell, this string is localized.

- **`BaseCooldown Property`**
  ```csharp
  public uint BaseCooldown { get; }
  ```
  Returns the base cooldown amount.

- **`BaseDuration Property`**
  ```csharp
  public int BaseDuration { get; }
  ```
  Returns the base duration of this spell or it's effect, eg; the duration of dots,
            buffs etc.

- **`CanCast Property`**
  ```csharp
  public bool CanCast { get; }
  ```
  Returns true if this is a usable spell. This does not check cooldown.
            To check if this spell can be cast right now, see CanCast(WoWSpell) and similar overloads.

- **`CastTime Property`**
  ```csharp
  public uint CastTime { get; }
  ```
  Returns the casttime of this spell.

- **`Category Property`**
  ```csharp
  public uint Category { get; }
  ```
  Returns the category id of this spell.

- **`Cooldown Property`**
  ```csharp
  public bool Cooldown { get; }
  ```
  Returns true if this spell is on cooldown.

- **`CooldownTimeLeft Property`**
  ```csharp
  public TimeSpan CooldownTimeLeft { get; }
  ```
  Gets the cooldown time left.

- **`CreatesItemId Property`**
  ```csharp
  public uint CreatesItemId { get; }
  ```
  Returns the item ID created by this spell.

- **`Description Property`**
  ```csharp
  public string Description { get; }
  ```
  Returns the description of this spell, this string is localized.

- **`DispelType Property`**
  ```csharp
  public WoWDispelType DispelType { get; }
  ```
  Returns the DispelType of this spell.

- **`DurationPerLevel Property`**
  ```csharp
  public int DurationPerLevel { get; }
  ```
  Returns the duration per level multiplier if any otherwise 0.

- **`HasRange Property`**
  ```csharp
  public bool HasRange { get; }
  ```
  Returns true if this is a ranged spell.

- **`Icon Property`**
  ```csharp
  public string Icon { get; }
  ```
  Gets the ingame icon of this WoWSpell

- **`Id Property`**
  ```csharp
  public int Id { get; }
  ```
  Returns the id of this spell.

- **`InternalInfo Property`**
  ```csharp
  public WoWSpell.SpellEntry InternalInfo { get; }
  ```
  Returns the internal cached dbc info used for this spell.

- **`IsChanneled Property`**
  ```csharp
  public bool IsChanneled { get; }
  ```
  Gets a bool that indicates whether this spell is channeled.

- **`IsHealingSpell Property`**
  ```csharp
  public bool IsHealingSpell { get; }
  ```
  Gets a value indicating whether this spell is healing spell.

- **`IsMeleeSpell Property`**
  ```csharp
  public bool IsMeleeSpell { get; }
  ```
  Gets a value indicating whether this object is melee spell (can only be cast within
            melee range).

- **`IsSelfOnlySpell Property`**
  ```csharp
  public bool IsSelfOnlySpell { get; }
  ```
  Gets a value indicating whether this object is self buff spell (can only cast it on
            yourself).

- **`IsValid Property`**
  ```csharp
  public bool IsValid { get; }
  ```
  Returns true if this is a valid spell.

- **`LocalizedName Property`**
  ```csharp
  public string LocalizedName { get; }
  ```
  Returns the localized name of this spell.

- **`MaxDuration Property`**
  ```csharp
  public int MaxDuration { get; }
  ```
  Rerturn the maxduration of this spell.

- **`MaxRange Property`**
  ```csharp
  public float MaxRange { get; }
  ```
  Returns the maxrange of this spell.

- **`MaxTargets Property`**
  ```csharp
  public uint MaxTargets { get; }
  ```
  Returns the number of max targets for this spell, usally only applies to dots. eg;
            'Shadow Word: Pain'.

- **`Mechanic Property`**
  ```csharp
  public WoWSpellMechanic Mechanic { get; }
  ```
  Returns the Mechanic of this spell.

- **`MinRange Property`**
  ```csharp
  public float MinRange { get; }
  ```
  Returns the minrange of this spell.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Returns the English name of this spell.

- **`NameSubText Property`**
  ```csharp
  public string NameSubText { get; }
  ```
  Returns the description of this spell, this string is localized.

- **`NumSpellEffects Property`**
  ```csharp
  [ObsoleteAttribute("Use SpellEffects.Length")]
public int NumSpellEffects { get; }
  ```
  Returns the number of spell effects this spell has.

- **`School Property`**
  ```csharp
  public WoWSpellSchool School { get; }
  ```
  Returns the WoWSpellSchool of this item. eg; 'Shadow', 'Holy' etc.

- **`SpellDescriptionVariableId Property`**
  ```csharp
  public uint SpellDescriptionVariableId { get; }
  ```
  Gets the spell description variable id of this spell.

- **`SpellEffect1 Property`**
  ```csharp
  [ObsoleteAttribute("Use SpellEffects")]
public SpellEffect SpellEffect1 { get; }
  ```
  Returns the #1 effect of this spell.

- **`SpellEffect2 Property`**
  ```csharp
  [ObsoleteAttribute("Use SpellEffects")]
public SpellEffect SpellEffect2 { get; }
  ```
  Returns the #2 effect of this spell.

- **`SpellEffect3 Property`**
  ```csharp
  [ObsoleteAttribute("Use SpellEffects")]
public SpellEffect SpellEffect3 { get; }
  ```
  Returns the #3 effect of this spell.

- **`SpellEffects Property`**
  ```csharp
  public SpellEffect[] SpellEffects { get; }
  ```
  Returns all spell effects of this spell.

- **`TargetType Property`**
  ```csharp
  public WoWCreatureType TargetType { get; }
  ```
  Returns the type of target this spell can usually be cast on.

#### WoWSpell Methods

- **`Cast Method`**
  ```csharp
  public void Cast()
  ```
  Casts this spell.

- **`Equals Method`**

- **`Equals Method (Object)`**
  ```csharp
  public override bool Equals(
Object obj
)
  ```
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LST7D147F99_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceWoWSpell Class

- **`Equals Method (WoWSpell)`**
  ```csharp
  public bool Equals(
WoWSpell other
)
  ```
  - *other*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTF00FEFC3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpell
  - **Returns:** See Also

- **`FromId Method`**
  ```csharp
  public static WoWSpell FromId(
int id
)
  ```
  Converts Id to WoWSpell.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST423E3646_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The identifier.
  - **Returns:** ReferenceWoWSpell Class

- **`GetAttributes Method`**
  ```csharp
  public uint GetAttributes(
int index
)
  ```
  Gets the attribute flags at a specified index.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTAFEC948_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The index.
  - **Returns:** ExceptionConditionArgumentOutOfRangeExceptionThrown if index is negative or greater than or equal to NumAttributeFields.

- **`GetChargeInfo Method`**
  ```csharp
  public SpellChargeInfo GetChargeInfo()
  ```
  Gets information about this spell's charges.
  - **Returns:** See Also

- **`GetDetailedPowerCosts Method`**
  Gets detailed power costs for this spell.

- **`GetDetailedPowerCosts Method`**
  ```csharp
  public ICollection&lt;SpellDetailedPowerCost&gt; GetDetailedPowerCosts()
  ```
  Gets detailed power costs for this spell.
  - **Returns:** Exceptions

- **`GetDetailedPowerCosts Method (WoWUnit)`**
  ```csharp
  public ICollection&lt;SpellDetailedPowerCost&gt; GetDetailedPowerCosts(
WoWUnit caster
)
  ```
  Gets detailed power costs for this spell.
  - *caster*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST58F33316_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe caster. Only valid for the local player and the local player's pet.
  - **Returns:** Exceptions

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  - **Returns:** ReferenceWoWSpell Class

- **`GetPowerCost Method`**
  ```csharp
  public double GetPowerCost(
WoWPowerType type
)
  ```
  Gets the cost of this spell in the specified power type.
  - *type*: Type: StyxAddLanguageSpecificTextSet("LST2ED4F19C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPowerTypeThe type.
  - **Returns:** ReferenceWoWSpell Class

- **`GetPowerCostPerSecond Method`**
  ```csharp
  public double GetPowerCostPerSecond(
WoWPowerType type
)
  ```
  Gets the cost of this spell per second in the specified power type.
  - *type*: Type: StyxAddLanguageSpecificTextSet("LSTA7C7FF9D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPowerTypeThe type.
  - **Returns:** ReferenceWoWSpell Class

- **`GetSpellEffect Method`**
  ```csharp
  [ObsoleteAttribute("Use SpellEffects")]
public SpellEffect GetSpellEffect(
int index
)
  ```
  Gets a spell effect for this spell.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST6DB7E4E8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.
  - **Returns:** ReferenceWoWSpell Class

- **`HasAttribute Method`**
  Checks if this spell has the specified attribute.

- **`HasAttribute Method (SpellAttributes)`**
  ```csharp
  public bool HasAttribute(
SpellAttributes attribute
)
  ```
  Checks if this spell has the specified attribute.
  - *attribute*: Type: StyxAddLanguageSpecificTextSet("LSTED33B4F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SpellAttributesThe attribute to check.
  - **Returns:** ReferenceWoWSpell Class

- **`HasAttribute Method (SpellAttributesEx)`**
  ```csharp
  public bool HasAttribute(
SpellAttributesEx attribute
)
  ```
  Checks if this spell has the specified attribute.
  - *attribute*: Type: StyxAddLanguageSpecificTextSet("LST5364D7A4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SpellAttributesExThe attribute to check.
  - **Returns:** ReferenceWoWSpell Class

- **`HasAttribute Method (SpellAttributesEx2)`**
  ```csharp
  public bool HasAttribute(
SpellAttributesEx2 attribute
)
  ```
  Checks if this spell has the specified attribute.
  - *attribute*: Type: StyxAddLanguageSpecificTextSet("LST388F6F00_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SpellAttributesEx2The attribute to check.
  - **Returns:** ReferenceWoWSpell Class

- **`HasAttribute Method (SpellAttributesEx3)`**
  ```csharp
  public bool HasAttribute(
SpellAttributesEx3 attribute
)
  ```
  Checks if this spell has the specified attribute.
  - *attribute*: Type: StyxAddLanguageSpecificTextSet("LST388F6F01_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SpellAttributesEx3The attribute to check.
  - **Returns:** ReferenceWoWSpell Class

- **`HasAttribute Method (SpellAttributesEx4)`**
  ```csharp
  public bool HasAttribute(
SpellAttributesEx4 attribute
)
  ```
  Checks if this spell has the specified attribute.
  - *attribute*: Type: StyxAddLanguageSpecificTextSet("LST388F6F02_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SpellAttributesEx4The attribute to check.
  - **Returns:** ReferenceWoWSpell Class

- **`HasAttribute Method (SpellAttributesEx5)`**
  ```csharp
  public bool HasAttribute(
SpellAttributesEx5 attribute
)
  ```
  Checks if this spell has the specified attribute.
  - *attribute*: Type: StyxAddLanguageSpecificTextSet("LST388F6F03_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SpellAttributesEx5The attribute to check.
  - **Returns:** ReferenceWoWSpell Class

- **`HasAttribute Method (SpellAttributesEx6)`**
  ```csharp
  public bool HasAttribute(
SpellAttributesEx6 attribute
)
  ```
  Checks if this spell has the specified attribute.
  - *attribute*: Type: StyxAddLanguageSpecificTextSet("LST388F6F04_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SpellAttributesEx6The attribute to check.
  - **Returns:** ReferenceWoWSpell Class

- **`HasAttribute Method (SpellAttributesEx7)`**
  ```csharp
  public bool HasAttribute(
SpellAttributesEx7 attribute
)
  ```
  Checks if this spell has the specified attribute.
  - *attribute*: Type: StyxAddLanguageSpecificTextSet("LST388F6F05_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SpellAttributesEx7The attribute to check.
  - **Returns:** ReferenceWoWSpell Class

- **`HasAttribute Method (SpellAttributesEx8)`**
  ```csharp
  public bool HasAttribute(
SpellAttributesEx8 attribute
)
  ```
  Checks if this spell has the specified attribute.
  - *attribute*: Type: StyxAddLanguageSpecificTextSet("LST388F6F06_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SpellAttributesEx8The attribute to check.
  - **Returns:** ReferenceWoWSpell Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceWoWSpell Class

#### WoWSpell Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
WoWSpell left,
WoWSpell right
)
  ```
  - *left*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST86C1536E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpell
  - *right*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST86C1536E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpell
  - **Returns:** ReferenceWoWSpell Class

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
WoWSpell left,
WoWSpell right
)
  ```
  - *left*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTA07AE78B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpell
  - *right*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTA07AE78B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpell
  - **Returns:** ReferenceWoWSpell Class

---

### WoWSpell.SpellCastTimesEntry Structure

```csharp
public struct SpellCastTimesEntry
```

A spell cast times entry.

#### SpellCastTimesEntry Fields

- **`CastTime Field`**
  ```csharp
  public int CastTime
  ```

- **`CastTimePerLevel Field`**
  ```csharp
  public short CastTimePerLevel
  ```

- **`MinCastTime Field`**
  ```csharp
  public int MinCastTime
  ```

---

### WoWSpell.SpellCastingRequirementsEntry Structure

```csharp
public struct SpellCastingRequirementsEntry
```

A spell casting requirements entry.

#### SpellCastingRequirementsEntry Fields

- **`FacingCasterFlags Field`**
  ```csharp
  public byte FacingCasterFlags
  ```

- **`MinFactionId Field`**
  ```csharp
  public ushort MinFactionId
  ```

- **`MinReputation Field`**
  ```csharp
  public byte MinReputation
  ```

- **`RequiredAreasId Field`**
  ```csharp
  public ushort RequiredAreasId
  ```

- **`RequiredAuraVision Field`**
  ```csharp
  public byte RequiredAuraVision
  ```

- **`RequiresSpellFocusId Field`**
  ```csharp
  public ushort RequiresSpellFocusId
  ```

- **`SpellId Field`**
  ```csharp
  public uint SpellId
  ```

---

### WoWSpell.SpellCategoriesEntry Structure

```csharp
public struct SpellCategoriesEntry
```

A spell categories entry.

#### SpellCategoriesEntry Fields

- **`Category Field`**
  ```csharp
  public ushort Category
  ```

- **`ChargeCategory Field`**
  ```csharp
  public short ChargeCategory
  ```

- **`DefenseType Field`**
  ```csharp
  public byte DefenseType
  ```

- **`DifficultyId Field`**
  ```csharp
  public byte DifficultyId
  ```

- **`DispelType Field`**
  ```csharp
  public byte DispelType
  ```

- **`Mechanic Field`**
  ```csharp
  public byte Mechanic
  ```

- **`PreventionType Field`**
  ```csharp
  public byte PreventionType
  ```

- **`SpellId Field`**
  ```csharp
  public uint SpellId
  ```

- **`StartRecoveryCategory Field`**
  ```csharp
  public short StartRecoveryCategory
  ```

---

### WoWSpell.SpellDurationEntry Structure

```csharp
public struct SpellDurationEntry
```

A spell duration entry.

#### SpellDurationEntry Fields

- **`Duration Field`**
  ```csharp
  public int Duration
  ```

- **`DurationPerLevel Field`**
  ```csharp
  public int DurationPerLevel
  ```

- **`MaxDuration Field`**
  ```csharp
  public int MaxDuration
  ```

---

### WoWSpell.SpellEntry Structure

```csharp
public struct SpellEntry
```

A spell entry.

#### SpellEntry Properties

- **`CastingTime Property`**
  ```csharp
  public WoWSpell.SpellCastTimesEntry CastingTime { get; }
  ```
  Gets the casting time.

- **`Misc Property`**
  ```csharp
  public WoWSpell.SpellMiscRec Misc { get; }
  ```
  Gets the misc.

- **`SpellCastingRequirements Property`**
  ```csharp
  public Nullable&lt;WoWSpell.SpellCastingRequirementsEntry&gt; SpellCastingRequirements { get; }
  ```
  Gets the spell casting requirements.

- **`SpellCategories Property`**
  ```csharp
  public Nullable&lt;WoWSpell.SpellCategoriesEntry&gt; SpellCategories { get; }
  ```
  Gets the categories the spell belongs to.

- **`SpellDuration Property`**
  ```csharp
  public WoWSpell.SpellDurationEntry SpellDuration { get; }
  ```
  Gets the duration of the spell.

- **`SpellInterrupts Property`**
  ```csharp
  public Nullable&lt;WoWSpell.SpellInterruptsEntry&gt; SpellInterrupts { get; }
  ```
  Gets the spell interrupts.

- **`SpellMissile Property`**
  ```csharp
  public SpellMissile SpellMissile { get; }
  ```
  Gets the spell missile.

- **`SpellRange Property`**
  ```csharp
  public WoWSpell.SpellRangeEntry SpellRange { get; }
  ```
  Gets the spell range.

- **`SpellReagents Property`**
  ```csharp
  public Nullable&lt;WoWSpell.SpellReagentsEntry&gt; SpellReagents { get; }
  ```
  Gets the spell reagents.

- **`SpellTargetRestrictions Property`**
  ```csharp
  public Nullable&lt;WoWSpell.SpellTargetRestrictionsEntry&gt; SpellTargetRestrictions { get; }
  ```
  Gets the spell target restrictions.

- **`SpellTotems Property`**
  ```csharp
  public Nullable&lt;WoWSpell.SpellTotemsEntry&gt; SpellTotems { get; }
  ```
  Gets the spell totems.

#### SpellEntry Fields

- **`AuraDescription Field`**
  ```csharp
  public IntPtr AuraDescription
  ```
  Information describing the aura.

- **`Description Field`**
  ```csharp
  public IntPtr Description
  ```
  The description.

- **`Id Field`**
  ```csharp
  public uint Id
  ```
  Identifier for the spell missile.

- **`MiscId Field`**
  ```csharp
  public uint MiscId
  ```
  Identifier for the rune cost.

- **`Name Field`**
  ```csharp
  public IntPtr Name
  ```
  The name.

- **`NameSubText Field`**
  ```csharp
  public IntPtr NameSubText
  ```
  The name sub text.

- **`SpellDescriptionVariableId Field`**
  ```csharp
  public uint SpellDescriptionVariableId
  ```
  Identifier for the spell description variable.

---

### WoWSpell.SpellInterruptsEntry Structure

```csharp
public struct SpellInterruptsEntry
```

A spell interrupts entry.

#### SpellInterruptsEntry Fields

- **`AuraInterruptFlags Field`**
  ```csharp
  public uint[] AuraInterruptFlags
  ```

- **`ChannelInterruptFlags Field`**
  ```csharp
  public uint[] ChannelInterruptFlags
  ```

- **`DifficultyId Field`**
  ```csharp
  public byte DifficultyId
  ```

- **`InterruptFlags Field`**
  ```csharp
  public ushort InterruptFlags
  ```

- **`SpellId Field`**
  ```csharp
  public uint SpellId
  ```

---

### WoWSpell.SpellMiscRec Structure

```csharp
public struct SpellMiscRec
```

Information about the spell misc.

#### SpellMiscRec Fields

- **`ActiveIconId Field`**
  ```csharp
  public ushort ActiveIconId
  ```

- **`Attributes Field`**
  ```csharp
  public fixed uint Attributes[14]
  ```

- **`MultistrikeSpeedMod Field`**
  ```csharp
  public float MultistrikeSpeedMod
  ```

- **`NumAttributeFields Field`**
  ```csharp
  public const int NumAttributeFields = 14
  ```

- **`Speed Field`**
  ```csharp
  public float Speed
  ```

- **`SpellCastTimesId Field`**
  ```csharp
  public ushort SpellCastTimesId
  ```

- **`SpellDurationId Field`**
  ```csharp
  public ushort SpellDurationId
  ```

- **`SpellIconId Field`**
  ```csharp
  public ushort SpellIconId
  ```

- **`SpellRangeId Field`**
  ```csharp
  public ushort SpellRangeId
  ```

- **`SpellSchoolMask Field`**
  ```csharp
  public short SpellSchoolMask
  ```

---

### WoWSpell.SpellRangeEntry Structure

```csharp
public struct SpellRangeEntry
```

A spell range entry.

#### SpellRangeEntry Properties

- **`IsMelee Property`**
  ```csharp
  public bool IsMelee { get; }
  ```

- **`Range1Str Property`**
  ```csharp
  public string Range1Str { get; }
  ```
  Gets the range 1 string.

- **`Range2Str Property`**
  ```csharp
  public string Range2Str { get; }
  ```
  Gets the range 2 string.

- **`UsesDefaultMinRange Property`**
  ```csharp
  public bool UsesDefaultMinRange { get; }
  ```

#### SpellRangeEntry Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns the fully qualified type name of this instance.
  - **Returns:** ReferenceWoWSpellAddLanguageSpecificTextSet("LST3FC04D94_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SpellRangeEntry Structure

#### SpellRangeEntry Fields

- **`Flags Field`**
  ```csharp
  public byte Flags
  ```

- **`MaxRange Field`**
  ```csharp
  public float MaxRange
  ```
  Gets the maximum range friendly.

- **`MaxRangeFriendly Field`**
  ```csharp
  public float MaxRangeFriendly
  ```
  Gets the maximum range friendly.

- **`MinRange Field`**
  ```csharp
  public float MinRange
  ```
  Gets the maximum range friendly.

- **`MinRangeFriendly Field`**
  ```csharp
  public float MinRangeFriendly
  ```
  Gets the maximum range friendly.

- **`Range1 Field`**
  ```csharp
  public IntPtr Range1
  ```
  The flags.

- **`Range2 Field`**
  ```csharp
  public IntPtr Range2
  ```
  The flags.

---

### WoWSpell.SpellReagentsEntry Structure

```csharp
public struct SpellReagentsEntry
```

A spell reagents entry.

#### SpellReagentsEntry Fields

- **`Reagent Field`**
  ```csharp
  public int[] Reagent
  ```

- **`ReagentCount Field`**
  ```csharp
  public ushort[] ReagentCount
  ```

- **`SpellId Field`**
  ```csharp
  public uint SpellId
  ```

---

### WoWSpell.SpellShapeshiftEntry Structure

```csharp
public struct SpellShapeshiftEntry
```

A spell shapeshift entry.

#### SpellShapeshiftEntry Fields

- **`ShapeshiftExclude Field`**
  ```csharp
  public fixed uint ShapeshiftExclude[2]
  ```

- **`ShapeshiftMask Field`**
  ```csharp
  public fixed uint ShapeshiftMask[2]
  ```

- **`SpellId Field`**
  ```csharp
  public uint SpellId
  ```

- **`StanceBarOrder Field`**
  ```csharp
  public short StanceBarOrder
  ```

---

### WoWSpell.SpellTargetRestrictionsEntry Structure

```csharp
public struct SpellTargetRestrictionsEntry
```

A spell target restrictions entry.

#### SpellTargetRestrictionsEntry Fields

- **`ConeAngle Field`**
  ```csharp
  public float ConeAngle
  ```

- **`DifficultyId Field`**
  ```csharp
  public byte DifficultyId
  ```

- **`MaxAffectedTargets Field`**
  ```csharp
  public byte MaxAffectedTargets
  ```

- **`MaxTargetLevel Field`**
  ```csharp
  public uint MaxTargetLevel
  ```

- **`SpellId Field`**
  ```csharp
  public uint SpellId
  ```

- **`TargetCreatureType Field`**
  ```csharp
  public short TargetCreatureType
  ```

- **`Targets Field`**
  ```csharp
  public uint Targets
  ```

- **`Width Field`**
  ```csharp
  public float Width
  ```

---

### WoWSpell.SpellTotemsEntry Structure

```csharp
public struct SpellTotemsEntry
```

A spell totems entry.

#### SpellTotemsEntry Fields

- **`dword0 Field`**
  ```csharp
  public int dword0
  ```

- **`RequiredTotemCategoryId Field`**
  ```csharp
  public ushort[] RequiredTotemCategoryId
  ```

- **`Totem Field`**
  ```csharp
  public uint[] Totem
  ```

---

### WoWSpellEffectType Enumeration

```csharp
public enum WoWSpellEffectType
```

Values that represent WoWSpellEffectType.

---

### WoWSpellFocus Enumeration

```csharp
public enum WoWSpellFocus
```

Values that represent WoWSpellFocus.

---

### WoWSpellMechanic Enumeration

```csharp
public enum WoWSpellMechanic
```

Values that represent WoWSpellMechanic.

---

### WoWSpellSchool Enumeration

```csharp
[FlagsAttribute]
public enum WoWSpellSchool
```

Bitfield of flags for specifying WoWSpellSchool.

---

### WoWTaxi Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWTaxi

```csharp
public static class WoWTaxi
```

Helper functions for WoW taxi.

#### WoWTaxi Methods

- **`GetKnownFlightNodesMask Method`**
  ```csharp
  public static byte[] GetKnownFlightNodesMask()
  ```
  Gets the known flight nodes mask from the current taxi map.
  - **Returns:** Exceptions

- **`GetTaxiMapNodes Method`**
  ```csharp
  public static TaxiMapNode[] GetTaxiMapNodes()
  ```
  Gets the map nodes shown on the current taxi map that are usable.
  - **Returns:** Exceptions

---

### WoWTotem Enumeration

```csharp
public enum WoWTotem
```

Values that represent WoWTotem.

---

### WoWTotemExtensions Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWTotemExtensions

```csharp
public static class WoWTotemExtensions
```

A WoW totem extensions.

#### WoWTotemExtensions Methods

- **`GetTotemSpellId Method`**
  ```csharp
  public static int GetTotemSpellId(
this WoWTotem totem
)
  ```
  A WoWTotem extension method that gets totem spell identifier.
  - *totem*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST60A9114C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWTotemThe totem to act on.
  - **Returns:** ReferenceWoWTotemExtensions Class

- **`GetTotemType Method`**
  ```csharp
  public static WoWTotemType GetTotemType(
this WoWTotem totem
)
  ```
  A WoWTotem extension method that gets totem type.
  - *totem*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTA66FECFB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWTotemThe totem to act on.
  - **Returns:** ReferenceWoWTotemExtensions Class

---

### WoWTotemInfo Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWTotemInfo

```csharp
public class WoWTotemInfo
```

Information about the WoW totem.

#### WoWTotemInfo Properties

- **`CanDismiss Property`**
  ```csharp
  public bool CanDismiss { get; }
  ```
  Gets a bool that indicates whether this totem can be dismissed.

- **`Duration Property`**
  ```csharp
  public uint Duration { get; }
  ```
  Gets the duration, in milliseconds, that the totem will stay out.

- **`Expired Property`**
  ```csharp
  public bool Expired { get; }
  ```
  Returns true if the totem has expired. (Ran its full duration)

- **`Expires Property`**
  ```csharp
  public DateTime Expires { get; }
  ```
  Gets the time when this totem will expire.

- **`Guid Property`**
  ```csharp
  public WoWGuid Guid { get; }
  ```
  Gets a unique identifier for the totem (Only valid if the totem is actually out!).

- **`IconPath Property`**
  ```csharp
  public string IconPath { get; }
  ```
  Gets the full pathname of the icon file. (This is an MPQ path!)

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name of this totem. (This is NOT always in English! Do not use this as an
            identifier!)

- **`Slot Property`**
  ```csharp
  public uint Slot { get; }
  ```
  Gets the slot for this totem. (0-3).

- **`Spell Property`**
  ```csharp
  public WoWSpell Spell { get; }
  ```
  Gets the spell that creates this totem.

- **`StartTime Property`**
  ```csharp
  public DateTime StartTime { get; }
  ```
  Gets the time of when this totem was laid out.

- **`TimeLeft Property`**
  ```csharp
  public TimeSpan TimeLeft { get; }
  ```
  Gets the amount of time left before this totem despawns.

- **`Type Property`**
  ```csharp
  public WoWTotemType Type { get; }
  ```
  Gets the type of totem this represents. (Fire, Earth, Water, Air)

- **`Unit Property`**
  ```csharp
  public WoWUnit Unit { get; }
  ```
  Gets the unit that this totem info represents.

- **`WoWTotem Property`**
  ```csharp
  public WoWTotem WoWTotem { get; }
  ```
  Gets the totem this represents. (Strength of Earth, Healing Stream, etc)

---

### WoWTotemType Enumeration

```csharp
public enum WoWTotemType
```

Values that represent WoWTotemType.

---

### WoWVehicle Class

**Inheritance:** System.Object → Styx.WoWInternals.NativeObject → Styx.WoWInternals.WoWVehicle

```csharp
public class WoWVehicle : NativeObject
```

Wrapper for CVehicle_C.

#### WoWVehicle Properties

- **`Rotation Property`**
  ```csharp
  public float Rotation { get; }
  ```

- **`VehicleRecord Property`**
  ```csharp
  public Vehicle VehicleRecord { get; }
  ```
  Gets the Vehicle record of this CVehicle.

- **`WorldMatrix Property`**
  ```csharp
  public Matrix4x4 WorldMatrix { get; }
  ```
  Gets the vehicle's world matrix.

#### WoWVehicle Methods

- **`CanProjectileReachLocation Method`**
  ```csharp
  public bool CanProjectileReachLocation(
Vector3 location,
WoWSpell projectileSpell,
Nullable&lt;Vector3&gt; projectileStart = null
)
  ```
  Determines whether vehicle can fire projectileSpell at location.
            This does not determine if anything is blocking projectile path.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST2DCCB2DB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *projectileSpell*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST2DCCB2DB_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpellThe projectile spell.
  - **Returns:** ReferenceWoWVehicle Class

- **`CanRotateToAngle Method`**
  ```csharp
  public bool CanRotateToAngle(
float angleInRadians
)
  ```
  Determines whether vehicle can be rotated to face in the angleInRadians direction.
  - *angleInRadians*: Type: SystemAddLanguageSpecificTextSet("LST3B82206C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe angle in radians.
  - **Returns:** ReferenceWoWVehicle Class

- **`CanRotateTowardsLocation Method`**
  ```csharp
  public bool CanRotateTowardsLocation(
Vector3 targetLocation
)
  ```
  Determines whether the vehicle can be rotated to face in the direction of targetLocation.
  - *targetLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LST419E7AF9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The targetLocation.
  - **Returns:** ReferenceWoWVehicle Class

- **`FindPassengerUnit Method`**
  ```csharp
  public WoWUnit FindPassengerUnit(
int seatIndex
)
  ```
  Finds a passenger of this vehicle.
  - *seatIndex*: Type: SystemAddLanguageSpecificTextSet("LST84068B90_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
  - **Returns:** ReferenceWoWVehicle Class

- **`GetPassengers Method`**
  ```csharp
  public IEnumerable&lt;WoWUnit&gt; GetPassengers()
  ```
  Gets all passengers of this vehicle.
  - **Returns:** See Also

- **`GetVirtualSeatCount Method`**
  ```csharp
  public int GetVirtualSeatCount()
  ```
  Gets the virtual seat count.
  - **Returns:** ReferenceWoWVehicle Class

---
