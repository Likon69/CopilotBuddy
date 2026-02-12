# Styx.CommonBot

Contains bot related classes.

## Contents

- [Blacklist Class](#blacklist-class)
- [Blacklist.BlacklistEntry Class](#blacklist.blacklistentry-class)
- [Blacklist.EntryRemovalDelegate Delegate](#blacklist.entryremovaldelegate-delegate)
- [BlacklistFlags Enumeration](#blacklistflags-enumeration)
- [BotBase Class](#botbase-class)
- [BotEvents Class](#botevents-class)
- [BotEvents.Battleground Class](#botevents.battleground-class)
- [BotEvents.Battleground.BattlegroundEnterDelegate Delegate](#botevents.battleground.battlegroundenterdelegate-delegate)
- [BotEvents.Battleground.BattlegroundLeftDelegate Delegate](#botevents.battleground.battlegroundleftdelegate-delegate)
- [BotEvents.BotChangedEventArgs Class](#botevents.botchangedeventargs-class)
- [BotEvents.BotStartStopRequestEventArgs Class](#botevents.botstartstoprequesteventargs-class)
- [BotEvents.BotStoppedEventArgs Class](#botevents.botstoppedeventargs-class)
- [BotEvents.OnBotChangedDelegate Delegate](#botevents.onbotchangeddelegate-delegate)
- [BotEvents.OnBotStartDelegate Delegate](#botevents.onbotstartdelegate-delegate)
- [BotEvents.OnBotStartStopRequestedDelegate Delegate](#botevents.onbotstartstoprequesteddelegate-delegate)
- [BotEvents.OnBotStopDelegate Delegate](#botevents.onbotstopdelegate-delegate)
- [BotEvents.Player Class](#botevents.player-class)
- [BotEvents.Player.LevelUpDelegate Delegate](#botevents.player.levelupdelegate-delegate)
- [BotEvents.Player.LevelUpEventArgs Class](#botevents.player.levelupeventargs-class)
- [BotEvents.Player.MapChangedDelegate Delegate](#botevents.player.mapchangeddelegate-delegate)
- [BotEvents.Player.MapChangedEventArgs Class](#botevents.player.mapchangedeventargs-class)
- [BotEvents.Player.MobKilledDelegate Delegate](#botevents.player.mobkilleddelegate-delegate)
- [BotEvents.Player.MobKilledEventArgs Class](#botevents.player.mobkilledeventargs-class)
- [BotEvents.Player.MobLootedDelegate Delegate](#botevents.player.moblooteddelegate-delegate)
- [BotEvents.Player.MobLootedEventArgs Class](#botevents.player.moblootedeventargs-class)
- [BotEvents.Player.ObjectHarvestedDelegate Delegate](#botevents.player.objectharvesteddelegate-delegate)
- [BotEvents.Player.ObjectHarvestedEventArgs Class](#botevents.player.objectharvestedeventargs-class)
- [BotEvents.Player.PlayerDiedDelegate Delegate](#botevents.player.playerdieddelegate-delegate)
- [BotEvents.Profile Class](#botevents.profile-class)
- [BotEvents.Profile.NewProfileLoadedDelegate Delegate](#botevents.profile.newprofileloadeddelegate-delegate)
- [BotEvents.Profile.NewProfileLoadedEventArgs Class](#botevents.profile.newprofileloadedeventargs-class)
- [BotEvents.Questing Class](#botevents.questing-class)
- [BotEvents.Questing.QuestAcceptedDelegate Delegate](#botevents.questing.questaccepteddelegate-delegate)
- [BotManager Class](#botmanager-class)
- [BuyItemsEventArgs Class](#buyitemseventargs-class)
- [BuyItemsEventHandler Delegate](#buyitemseventhandler-delegate)
- [Chat Class](#chat-class)
- [Chat.ChatAddonEventArgs Class](#chat.chataddoneventargs-class)
- [Chat.ChatAuthoredEventArgs Class](#chat.chatauthoredeventargs-class)
- [Chat.ChatChannelSpecificEventArgs Class](#chat.chatchannelspecificeventargs-class)
- [Chat.ChatGuildEventArgs Class](#chat.chatguildeventargs-class)
- [Chat.ChatLanguageSpecificEventArgs Class](#chat.chatlanguagespecificeventargs-class)
- [Chat.ChatMessageBaseEventArgs Class](#chat.chatmessagebaseeventargs-class)
- [Chat.ChatMessageHandlerEx(T) Delegate](#chat.chatmessagehandlerext-delegate)
- [Chat.ChatMonsterEventArgs Class](#chat.chatmonstereventargs-class)
- [Chat.ChatMonsterSayEventArgs Class](#chat.chatmonstersayeventargs-class)
- [Chat.ChatSimpleMessageEventArgs Class](#chat.chatsimplemessageeventargs-class)
- [Chat.ChatWhisperEventArgs Class](#chat.chatwhispereventargs-class)
- [GameStats Class](#gamestats-class)
- [GameStats.InfoPanelUpdatedDelegate Delegate](#gamestats.infopanelupdateddelegate-delegate)
- [GoalTextChangedEventArgs Class](#goaltextchangedeventargs-class)
- [HealTargeting Class](#healtargeting-class)
- [HonorbuddyExitCode Enumeration](#honorbuddyexitcode-enumeration)
- [InactivityDetector Class](#inactivitydetector-class)
- [IncludeTargetsFilterDelegate Delegate](#includetargetsfilterdelegate-delegate)
- [KnownFlightNodesManager Class](#knownflightnodesmanager-class)
- [Landmarks Class](#landmarks-class)
- [LootPredictor Class](#lootpredictor-class)
- [LootTargeting Class](#loottargeting-class)
- [MailItemsEventArgs Class](#mailitemseventargs-class)
- [MailItemsEventHandler Delegate](#mailitemseventhandler-delegate)
- [Mount Class](#mount-class)
- [Mount.MountWrapper Class](#mount.mountwrapper-class)
- [MountType Enumeration](#mounttype-enumeration)
- [MountUpEventArgs Class](#mountupeventargs-class)
- [PulseFlags Enumeration](#pulseflags-enumeration)
- [RaFHelper Class](#rafhelper-class)
- [RemoveTargetsFilterDelegate Delegate](#removetargetsfilterdelegate-delegate)
- [Rest Class](#rest-class)
- [SellItemsEventArgs Class](#sellitemseventargs-class)
- [ShutdownRequestedEventArgs Class](#shutdownrequestedeventargs-class)
- [SpellFindResults Class](#spellfindresults-class)
- [SpellManager Class](#spellmanager-class)
- [StatusTextChangedEventArgs Class](#statustextchangedeventargs-class)
- [TargetListUpdateFinishedDelegate Delegate](#targetlistupdatefinisheddelegate-delegate)
- [Targeting Class](#targeting-class)
- [Targeting.TargetPriority Class](#targeting.targetpriority-class)
- [TreeRoot Class](#treeroot-class)
- [TreeRootState Enumeration](#treerootstate-enumeration)
- [VendorItemsEventHandler Delegate](#vendoritemseventhandler-delegate)
- [Vendors Class](#vendors-class)
- [WeighTargetsDelegate Delegate](#weightargetsdelegate-delegate)

---

### Blacklist Class

**Inheritance:** System.Object → Styx.CommonBot.Blacklist

```csharp
public static class Blacklist
```

Contains objects the bot should ignore in certain situations.

#### Blacklist Methods

- **`Add Method`**
  Adds the specified object's GUID to the blacklist with the specified flags.

- **`Add Method (WoWGuid, BlacklistFlags, TimeSpan, String)`**
  ```csharp
  public static void Add(
WoWGuid guid,
BlacklistFlags flags,
TimeSpan duration,
string reason = ""
)
  ```
  Adds the specified object's GUID to the blacklist with the specified flags.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTC1B8A1EB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidThe GUID.
  - *flags*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LSTC1B8A1EB_2?cs=.|vb=.|cpp=::|nu=.|fs=.");BlacklistFlagsThe flags.
  - *duration*: Type: SystemAddLanguageSpecificTextSet("LSTC1B8A1EB_3?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe duration.

- **`Add Method (WoWObject, BlacklistFlags, TimeSpan, String)`**
  ```csharp
  public static void Add(
WoWObject obj,
BlacklistFlags flags,
TimeSpan duration,
string reason = ""
)
  ```
  Adds the specified object's GUID to the blacklist with the specified flags.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST8FA1484E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe object.
  - *flags*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LST8FA1484E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");BlacklistFlagsThe flags.
  - *duration*: Type: SystemAddLanguageSpecificTextSet("LST8FA1484E_3?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe duration.

- **`Add Method (UInt32, Vector3, BlacklistFlags, TimeSpan, String)`**
  ```csharp
  public static void Add(
uint entry,
Vector3 location,
BlacklistFlags flags,
TimeSpan duration,
string reason = ""
)
  ```
  Adds the specified entry to the blacklist with the specified flags.
  - *entry*: Type: SystemAddLanguageSpecificTextSet("LST303868FD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The entry of the object.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST303868FD_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location of the object
  - *flags*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LST303868FD_3?cs=.|vb=.|cpp=::|nu=.|fs=.");BlacklistFlagsThe flags.
  - *duration*: Type: SystemAddLanguageSpecificTextSet("LST303868FD_4?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe duration.

- **`Add Method (WoWGuid, BlacklistFlags, TimeSpan, Func(WoWGuid, Boolean), String)`**
  ```csharp
  public static void Add(
WoWGuid guid,
BlacklistFlags flags,
TimeSpan duration,
Func&lt;WoWGuid, bool&gt; dynamicCondition,
string reason = ""
)
  ```
  Adds the specified object's GUID to the blacklist with the specified flags.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST9A49C47A_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidThe GUID.
  - *flags*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LST9A49C47A_4?cs=.|vb=.|cpp=::|nu=.|fs=.");BlacklistFlagsThe flags.
  - *duration*: Type: SystemAddLanguageSpecificTextSet("LST9A49C47A_5?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe duration.
  - *dynamicCondition*: Type: SystemAddLanguageSpecificTextSet("LST9A49C47A_6?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST9A49C47A_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWGuid, BooleanAddLanguageSpecificTextSet("LST9A49C47A_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");An additional condition that must be true for the entry to take effect. If this
            condition returns false, the entry is not removed early, but Contains(WoWGuid, BlacklistFlags) returns false.

- **`Add Method (UInt32, Vector3, BlacklistFlags, TimeSpan, Func(WoWGuid, Boolean), String)`**
  ```csharp
  public static void Add(
uint entry,
Vector3 location,
BlacklistFlags flags,
TimeSpan duration,
Func&lt;WoWGuid, bool&gt; dynamicCondition,
string reason = ""
)
  ```
  Adds the specified entry to the blacklist with the specified flags.
  - *entry*: Type: SystemAddLanguageSpecificTextSet("LSTBB9D9A32_3?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The entry of the object.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LSTBB9D9A32_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location of the object
  - *flags*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LSTBB9D9A32_5?cs=.|vb=.|cpp=::|nu=.|fs=.");BlacklistFlagsThe flags.
  - *duration*: Type: SystemAddLanguageSpecificTextSet("LSTBB9D9A32_6?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe duration.
  - *dynamicCondition*: Type: SystemAddLanguageSpecificTextSet("LSTBB9D9A32_7?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTBB9D9A32_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWGuid, BooleanAddLanguageSpecificTextSet("LSTBB9D9A32_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");An extra dynamic condition. See Add(WoWGuid, BlacklistFlags, TimeSpan, FuncAddLanguageSpecificTextSet("LSTBB9D9A32_10?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWGuid, BooleanAddLanguageSpecificTextSet("LSTBB9D9A32_11?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");, String)

- **`Clear Method`**
  ```csharp
  public static void Clear(
Blacklist.EntryRemovalDelegate predicate
)
  ```
  Removes the entries which meet the predicate passed.
  - *predicate*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LST48321F90_2?cs=.|vb=.|cpp=::|nu=.|fs=.");BlacklistAddLanguageSpecificTextSet("LST48321F90_3?cs=.|vb=.|cpp=::|nu=.|fs=.");EntryRemovalDelegateThe predicate.

- **`Contains Method`**
  Determines whether the blacklist contains an entry where the specified predicate evalutes to true.

- **`Contains Method (Func(Blacklist.BlacklistEntry, Boolean))`**
  ```csharp
  public static bool Contains(
Func&lt;Blacklist.BlacklistEntry, bool&gt; predicate
)
  ```
  Determines whether the blacklist contains an entry where the specified predicate evalutes to true.
  - *predicate*: Type: SystemAddLanguageSpecificTextSet("LST2C5CAF74_5?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST2C5CAF74_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BlacklistAddLanguageSpecificTextSet("LST2C5CAF74_7?cs=.|vb=.|cpp=::|nu=.|fs=.");BlacklistEntry, BooleanAddLanguageSpecificTextSet("LST2C5CAF74_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The predicate. This is invoked on each active entry.
  - **Returns:** Exceptions

- **`Contains Method (WoWGuid, BlacklistFlags)`**
  ```csharp
  public static bool Contains(
WoWGuid guid,
BlacklistFlags flags
)
  ```
  Determines whether the blacklist contains an entry with the specified guid with any of the flags provided.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTDAF04029_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidThe GUID.
  - *flags*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LSTDAF04029_2?cs=.|vb=.|cpp=::|nu=.|fs=.");BlacklistFlagsThe flags. If multiple flags are passed this method tests whether the entry has any of these flags.
  - **Returns:** See Also

- **`Contains Method (WoWObject, BlacklistFlags)`**
  ```csharp
  public static bool Contains(
WoWObject obj,
BlacklistFlags flags
)
  ```
  Determines whether the blacklist contains an entry for the specified object with any of the flags provided.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST2A6E6040_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe object. Cannot be null.
  - *flags*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LST2A6E6040_2?cs=.|vb=.|cpp=::|nu=.|fs=.");BlacklistFlagsThe flags. If multiple flags are passed this method tests whether the entry has any of these flags.
  - **Returns:** Exceptions

- **`Flush Method`**
  ```csharp
  public static void Flush()
  ```
  Flushes the blacklist, removing all finished blacklist entries.

- **`GetEntries Method`**
  Gets the blacklist entries belonging to a guid.

- **`GetEntries Method (WoWGuid)`**
  ```csharp
  public static IEnumerable&lt;Blacklist.BlacklistEntry&gt; GetEntries(
WoWGuid guid
)
  ```
  Gets the blacklist entries belonging to a guid.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST2FA04C66_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidThe GUID.
  - **Returns:** See Also

- **`GetEntries Method (WoWObject)`**
  ```csharp
  public static IEnumerable&lt;Blacklist.BlacklistEntry&gt; GetEntries(
WoWObject obj
)
  ```
  Gets the active blacklist entries belonging to the specified WoW object.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST48110FAF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe object. Should not be null.
  - **Returns:** Exceptions

- **`GetEntry Method`**
  Gets the blacklist entry belonging to a guid.

- **`GetEntry Method (WoWGuid)`**
  ```csharp
  [ObsoleteAttribute("Use GetEntries instead")]
public static Blacklist.BlacklistEntry GetEntry(
WoWGuid guid
)
  ```
  Gets the blacklist entry belonging to a guid.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTBB0BEE3E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidThe GUID.
  - **Returns:** See Also

- **`GetEntry Method (WoWObject)`**
  ```csharp
  [ObsoleteAttribute("Use GetEntries instead")]
public static Blacklist.BlacklistEntry GetEntry(
WoWObject obj
)
  ```
  Gets the blacklist entry belonging to the specified WoW object.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST7CFBA1DD_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe WoW object.
  - **Returns:** Exceptions

---

### Blacklist.BlacklistEntry Class

**Inheritance:** System.Object → Styx.CommonBot.Blacklist.BlacklistEntry

```csharp
public class BlacklistEntry
```

A blacklist entry.

#### BlacklistEntry Properties

- **`DynamicCondition Property`**
  ```csharp
  public Func&lt;WoWGuid, bool&gt; DynamicCondition { get; }
  ```
  Gets the dynamic condition for this entry to be considered blacklisted.

- **`Entry Property`**
  ```csharp
  public uint Entry { get; }
  ```
  The entry ID of the object.

- **`Flags Property`**
  ```csharp
  public BlacklistFlags Flags { get; }
  ```
  Gets the flags.

- **`Guid Property`**
  ```csharp
  public WoWGuid Guid { get; }
  ```
  Gets a unique identifier.

- **`IsFinished Property`**
  ```csharp
  public bool IsFinished { get; }
  ```
  Gets a value indicating whether this object is finished.

- **`Length Property`**
  ```csharp
  public TimeSpan Length { get; }
  ```
  Gets the duration this entry is blacklisted for.

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; }
  ```
  The location of the object.

- **`MapId Property`**
  ```csharp
  public int MapId { get; }
  ```
  The map ID the object was on when blacklisted.

- **`Started Property`**
  ```csharp
  public DateTime Started { get; }
  ```
  A DateTime instance that represents the instant of time when this object was blacklisted.

- **`Timer Property`**
  ```csharp
  public Stopwatch Timer { get; }
  ```
  Gets a timer that was started when this entry was last updated.

---

### Blacklist.EntryRemovalDelegate Delegate

```csharp
public delegate bool EntryRemovalDelegate(
Blacklist.BlacklistEntry entry
)
```

Entry removal delegate.

---

### BlacklistFlags Enumeration

```csharp
[FlagsAttribute]
public enum BlacklistFlags
```

Bitfield of flags for specifying BlacklistFlags.

---

### BotBase Class

**Inheritance:** System.Object → Styx.CommonBot.BotBase → Bots.DungeonBuddy.DungeonBot → Bots.Gatherbuddy.GatherbuddyBot → Bots.Grind.LevelBot → Bots.Quest.QuestBot

```csharp
public abstract class BotBase
```

A bottom base.

#### BotBase Properties

- **`ConfigurationForm Property`**
  ```csharp
  public virtual Form ConfigurationForm { get; }
  ```
  Gets the configuration form.

- **`ConfigurationWindow Property`**
  ```csharp
  public virtual Window ConfigurationWindow { get; }
  ```
  Gets the WPF-based configuration window.

- **`Initialized Property`**
  ```csharp
  public bool Initialized { get; }
  ```
  Gets a value indicating whether the initialized.

- **`IsPrimaryType Property`**
  ```csharp
  public virtual bool IsPrimaryType { get; }
  ```
  Gets a value indicating whether this object is primary bot type. These will be used
            by default in mixed-mode.

- **`Name Property`**
  ```csharp
  public abstract string Name { get; }
  ```
  Gets the name.

- **`PulseFlags Property`**
  ```csharp
  public abstract PulseFlags PulseFlags { get; }
  ```
  Gets the pulse flags.

- **`RequirementsMet Property`**
  ```csharp
  public virtual bool RequirementsMet { get; }
  ```
  Gets a value indicating whether the requirements met for this bot to be run.
            Secondary type bots MUST implement this member, for the bot to be used when needed.

- **`RequiresProfile Property`**
  ```csharp
  public virtual bool RequiresProfile { get; }
  ```
  Gets a value indicating whether the botbase requires a profile to run.

- **`RequiresProfileScope Property`**
  ```csharp
  public virtual bool RequiresProfileScope { get; }
  ```
  Gets a value indicating whether this bot requires Min and Max Level checks in the
            profiles.

- **`Root Property`**
  ```csharp
  public abstract Composite Root { get; }
  ```
  Gets the root.

- **`StatusObject Property`**
  ```csharp
  [ObsoleteAttribute("Not used. Will be removed.")]
protected JObject StatusObject { get; }
  ```

- **`StatusText Property`**
  ```csharp
  [ObsoleteAttribute("Not used. Will be removed.")]
public string StatusText { get; }
  ```
  Gets the status text of the bot.

#### BotBase Methods

- **`AddMonitorMembers Method`**
  ```csharp
  [ObsoleteAttribute("Not used. Will be removed in the future.")]
public virtual void AddMonitorMembers()
  ```
  Adds members to the monitor. Override this instead of implementing a new StatusText.

- **`DoInitialize Method`**
  ```csharp
  public void DoInitialize()
  ```
  Executes the initialize operation.

- **`Initialize Method`**
  ```csharp
  public virtual void Initialize()
  ```
  Initializes this bot base.

- **`OnDeselected Method`**
  ```csharp
  public virtual void OnDeselected()
  ```
  Executes the deselected action.

- **`OnPaused Method`**
  ```csharp
  public virtual void OnPaused()
  ```
  Called when the user pauses the bot. This function is called from the bot thread.

- **`OnResumed Method`**
  ```csharp
  public virtual void OnResumed()
  ```
  Called when the user resumes the bot. This function is called from the bot thread.

- **`OnSelected Method`**
  ```csharp
  public virtual void OnSelected()
  ```
  Executes the selected action.

- **`Pulse Method`**
  ```csharp
  public virtual void Pulse()
  ```
  Pulses this object.

- **`Start Method`**
  ```csharp
  public virtual void Start()
  ```
  Starts this bot. This is guaranteed to be called from the bot thread.

- **`Stop Method`**
  ```csharp
  public virtual void Stop()
  ```
  Stops this object. This is guaranteed to be called from the bot thread.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceBotBase Class

---

### BotEvents Class

**Inheritance:** System.Object → Styx.CommonBot.BotEvents

```csharp
public static class BotEvents
```

Provides events for common situations in the bot.

#### BotEvents Methods

- **`PulseEvents Method`**
  ```csharp
  public static void PulseEvents()
  ```
  Pulse events.

#### BotEvents Events

- **`OnBotChanged Event`**
  ```csharp
  public static event BotEvents.OnBotChangedDelegate OnBotChanged
  ```
  Fired when the current bot is changed with the usage of TreeRoot.SetCurrent.

- **`OnBotPaused Event`**
  ```csharp
  public static event EventHandler&lt;EventArgs&gt; OnBotPaused
  ```
  Fired right after the bot has been paused.

- **`OnBotResumed Event`**
  ```csharp
  public static event EventHandler&lt;EventArgs&gt; OnBotResumed
  ```
  Fired right after the bot has been resumed.

- **`OnBotStarted Event`**
  ```csharp
  public static event BotEvents.OnBotStartDelegate OnBotStarted
  ```
  Fired right after the bot has started.
            This is fired from the bot thread, before any logic starts executing.

- **`OnBotStartRequested Event`**
  ```csharp
  public static event BotEvents.OnBotStartStopRequestedDelegate OnBotStartRequested
  ```
  Fired right before the bot is started.
            Only use this event to signal that a 'Start' should be canceled. In general, use the OnBotStarted event.

- **`OnBotStopped Event`**
  ```csharp
  public static event BotEvents.OnBotStopDelegate OnBotStopped
  ```
  Fired right after the bot has been stopped.
            This is fired from the bot thread, before the thread exits.

- **`OnBotStopRequested Event`**
  ```csharp
  public static event BotEvents.OnBotStartStopRequestedDelegate OnBotStopRequested
  ```
  Fired when a bot stop is requested.
            Only use this event to signal that a 'Stop' should be canceled. In general use the OnBotStopped event.

- **`OnPulse Event`**
  ```csharp
  public static event EventHandler&lt;EventArgs&gt; OnPulse
  ```
  Fired on every bot pulse if bot events are enabled.

- **`PreBotStarting Event`**
  ```csharp
  public static event EventHandler&lt;EventArgs&gt; PreBotStarting
  ```
  Fired right before the bot begins starting.

---

### BotEvents.Battleground Class

**Inheritance:** System.Object → Styx.CommonBot.BotEvents.Battleground

```csharp
public static class Battleground
```

A battleground.

#### Battleground Methods

- **`FireBattlegroundEntered Method`**
  ```csharp
  public static void FireBattlegroundEntered()
  ```
  Fire battleground entered.

- **`FireBattlegroundLeft Method`**
  ```csharp
  public static void FireBattlegroundLeft()
  ```
  Fire battleground left.

#### Battleground Events

- **`OnBattlegroundEntered Event`**
  ```csharp
  public static event BotEvents.Battleground.BattlegroundEnterDelegate OnBattlegroundEntered
  ```
  Event occurs as the localplayer enters a battleground, sender contains the current
            battlegroundtype.

- **`OnBattlegroundLeft Event`**
  ```csharp
  public static event BotEvents.Battleground.BattlegroundLeftDelegate OnBattlegroundLeft
  ```
  Event queue for all listeners interested in OnBattlegroundLeft events.

---

### BotEvents.Battleground.BattlegroundEnterDelegate Delegate

```csharp
public delegate void BattlegroundEnterDelegate(
BattlegroundType type
)
```

Battleground enter delegate.

---

### BotEvents.Battleground.BattlegroundLeftDelegate Delegate

```csharp
public delegate void BattlegroundLeftDelegate(
EventArgs args
)
```

Battleground left delegate.

---

### BotEvents.BotChangedEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.CommonBot.BotEvents.BotChangedEventArgs

```csharp
public class BotChangedEventArgs : EventArgs
```

Additional information for bottom changed events.

#### BotChangedEventArgs Properties

- **`NewBot Property`**
  ```csharp
  public BotBase NewBot { get; }
  ```
  Gets the new bottom.

- **`OldBot Property`**
  ```csharp
  public BotBase OldBot { get; }
  ```
  Gets the old bottom.

---

### BotEvents.BotStartStopRequestEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.CommonBot.BotEvents.BotStartStopRequestEventArgs

```csharp
public class BotStartStopRequestEventArgs : EventArgs
```

Represents event args for the OnBotStartRequested and OnBotStopRequested events.

#### BotStartStopRequestEventArgs Properties

- **`Cancel Property`**
  ```csharp
  public bool Cancel { get; set; }
  ```
  Gets or sets a bool that indicates whether this stop should be canceled.

---

### BotEvents.BotStoppedEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.CommonBot.BotEvents.BotStoppedEventArgs

```csharp
public class BotStoppedEventArgs : EventArgs
```

Determines whether the specified object is equal to the current object.

#### BotStoppedEventArgs Properties

- **`RunTime Property`**
  ```csharp
  public TimeSpan RunTime { get; }
  ```

---

### BotEvents.OnBotChangedDelegate Delegate

```csharp
public delegate void OnBotChangedDelegate(
BotEvents.BotChangedEventArgs args
)
```

Raises the bottom changed event.

---

### BotEvents.OnBotStartDelegate Delegate

```csharp
public delegate void OnBotStartDelegate(
EventArgs args
)
```

The bot start delegate event.

---

### BotEvents.OnBotStartStopRequestedDelegate Delegate

```csharp
public delegate void OnBotStartStopRequestedDelegate(
BotEvents.BotStartStopRequestEventArgs args
)
```

---

### BotEvents.OnBotStopDelegate Delegate

```csharp
public delegate void OnBotStopDelegate(
EventArgs args
)
```

The bot stop delegate event.

---

### BotEvents.Player Class

**Inheritance:** System.Object → Styx.CommonBot.BotEvents.Player

```csharp
public static class Player
```

A player.

#### Player Events

- **`OnLevelUp Event`**
  ```csharp
  public static event BotEvents.Player.LevelUpDelegate OnLevelUp
  ```
  Event queue for all listeners interested in OnLevelUp events.

- **`OnMapChanged Event`**
  ```csharp
  public static event BotEvents.Player.MapChangedDelegate OnMapChanged
  ```
  This event is fired when the map changes.

- **`OnMobKilled Event`**
  ```csharp
  public static event BotEvents.Player.MobKilledDelegate OnMobKilled
  ```
  Event queue for all listeners interested in OnMobKilled events.

- **`OnMobLooted Event`**
  ```csharp
  public static event BotEvents.Player.MobLootedDelegate OnMobLooted
  ```
  Event queue for all listeners interested in OnMobLooted events.

- **`OnObjectHarvested Event`**
  ```csharp
  public static event BotEvents.Player.ObjectHarvestedDelegate OnObjectHarvested
  ```
  Event queue for all listeners interested in OnObjectHarvested events.

- **`OnPlayerDied Event`**
  ```csharp
  public static event BotEvents.Player.PlayerDiedDelegate OnPlayerDied
  ```
  This event fires when the local player dies.

---

### BotEvents.Player.LevelUpDelegate Delegate

```csharp
public delegate void LevelUpDelegate(
BotEvents.Player.LevelUpEventArgs args
)
```

Level up delegate.

---

### BotEvents.Player.LevelUpEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.CommonBot.BotEvents.Player.LevelUpEventArgs

```csharp
public class LevelUpEventArgs : EventArgs
```

Additional information for level up events.

#### LevelUpEventArgs Fields

- **`NewLevel Field`**
  ```csharp
  public int NewLevel
  ```
  The new level.

- **`OldLevel Field`**
  ```csharp
  public int OldLevel
  ```
  The old level.

---

### BotEvents.Player.MapChangedDelegate Delegate

```csharp
public delegate void MapChangedDelegate(
BotEvents.Player.MapChangedEventArgs args
)
```

Map changed delegate.

---

### BotEvents.Player.MapChangedEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.CommonBot.BotEvents.Player.MapChangedEventArgs

```csharp
public class MapChangedEventArgs : EventArgs
```

Additional information for map changed events.

#### MapChangedEventArgs Fields

- **`NewMapName Field`**
  ```csharp
  public string NewMapName
  ```
  The new map name. This is the internal name used by WoW and the bot for navigation.

---

### BotEvents.Player.MobKilledDelegate Delegate

```csharp
public delegate void MobKilledDelegate(
BotEvents.Player.MobKilledEventArgs args
)
```

Mob killed delegate.

---

### BotEvents.Player.MobKilledEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.CommonBot.BotEvents.Player.MobKilledEventArgs

```csharp
public class MobKilledEventArgs : EventArgs
```

Additional information for mob killed events.

#### MobKilledEventArgs Fields

- **`KilledMob Field`**
  ```csharp
  public WoWUnit KilledMob
  ```
  The killed mob.

- **`KillTime Field`**
  ```csharp
  public DateTime KillTime
  ```
  The kill time.

---

### BotEvents.Player.MobLootedDelegate Delegate

```csharp
public delegate void MobLootedDelegate(
BotEvents.Player.MobLootedEventArgs args
)
```

Mob looted delegate.

---

### BotEvents.Player.MobLootedEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.CommonBot.BotEvents.Player.MobLootedEventArgs

```csharp
public class MobLootedEventArgs : EventArgs
```

Additional information for mob looted events.

#### MobLootedEventArgs Fields

- **`LootedMob Field`**
  ```csharp
  public WoWUnit LootedMob
  ```
  The looted mob.

---

### BotEvents.Player.ObjectHarvestedDelegate Delegate

```csharp
public delegate void ObjectHarvestedDelegate(
BotEvents.Player.ObjectHarvestedEventArgs args
)
```

Object harvested delegate.

---

### BotEvents.Player.ObjectHarvestedEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.CommonBot.BotEvents.Player.ObjectHarvestedEventArgs

```csharp
public class ObjectHarvestedEventArgs : EventArgs
```

Additional information for object harvested events.

#### ObjectHarvestedEventArgs Fields

- **`HarvestedObject Field`**
  ```csharp
  public WoWGameObject HarvestedObject
  ```
  The harvested object.

---

### BotEvents.Player.PlayerDiedDelegate Delegate

```csharp
public delegate void PlayerDiedDelegate()
```

Player died delegate.

---

### BotEvents.Profile Class

**Inheritance:** System.Object → Styx.CommonBot.BotEvents.Profile

```csharp
public static class Profile
```

A profile.

#### Profile Events

- **`OnCodeComposition Event`**
  ```csharp
  public static event EventHandler&lt;CodeCompositionEventArgs&gt; OnCodeComposition
  ```
  This event is fired when gathering all CSharp code used in a profile (Profile) for compilation.Profile will never be null when this event is triggered.See Add(IXmlObject) for more details

- **`OnNewOuterProfileLoaded Event`**
  ```csharp
  public static event BotEvents.Profile.NewProfileLoadedDelegate OnNewOuterProfileLoaded
  ```
  Fired when a new outer profile is loaded. That means when the actual .xml file
            changes.

- **`OnNewProfileLoaded Event`**
  ```csharp
  public static event BotEvents.Profile.NewProfileLoadedDelegate OnNewProfileLoaded
  ```
  Fired when a new inner profile is loaded. That means, without the actually .xml
            profile changing, but a new subprofile in the current profile is loaded.

---

### BotEvents.Profile.NewProfileLoadedDelegate Delegate

```csharp
public delegate void NewProfileLoadedDelegate(
BotEvents.Profile.NewProfileLoadedEventArgs args
)
```

Creates a new profile loaded delegate.

---

### BotEvents.Profile.NewProfileLoadedEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.CommonBot.BotEvents.Profile.NewProfileLoadedEventArgs

```csharp
public class NewProfileLoadedEventArgs : EventArgs
```

Additional information for new profile loaded events.

#### NewProfileLoadedEventArgs Fields

- **`NewProfile Field`**
  ```csharp
  public Profile NewProfile
  ```
  The new profile.

- **`OldProfile Field`**
  ```csharp
  public Profile OldProfile
  ```
  The old profile.

---

### BotEvents.Questing Class

**Inheritance:** System.Object → Styx.CommonBot.BotEvents.Questing

```csharp
public static class Questing
```

A questing.

#### Questing Methods

- **`FireQuestAccepted Method`**
  ```csharp
  public static void FireQuestAccepted(
Quest quest
)
  ```
  Fire question accepted.
  - *quest*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST2F190010_2?cs=.|vb=.|cpp=::|nu=.|fs=.");QuestThe question.

#### Questing Events

- **`OnQuestAccepted Event`**
  ```csharp
  public static event BotEvents.Questing.QuestAcceptedDelegate OnQuestAccepted
  ```
  Event occurs as the localplayer accepts a new quest from a questgiver
            (gameobject/npc/item)

---

### BotEvents.Questing.QuestAcceptedDelegate Delegate

```csharp
public delegate void QuestAcceptedDelegate(
Quest quest
)
```

Question accepted delegate.

---

### BotManager Class

**Inheritance:** System.Object → Styx.CommonBot.BotManager

```csharp
public class BotManager
```

Manager for bottoms.

#### BotManager Properties

- **`Bots Property`**
  ```csharp
  public Dictionary&lt;string, BotBase&gt; Bots { get; }
  ```
  Returns a dictionary of all the currently loaded bots.

- **`BotsDirectory Property`**
  ```csharp
  public static string BotsDirectory { get; }
  ```
  Gets the routine directory.

- **`Current Property`**
  ```csharp
  public static BotBase Current { get; }
  ```
  Gets the current.

#### BotManager Methods

- **`Add Method`**
  Adds a bot to the bot collection.

- **`Add Method (BotBase)`**
  ```csharp
  public void Add(
BotBase derievesFromBotBase
)
  ```
  Adds a bot to the bot collection.
  - *derievesFromBotBase*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LSTDD414ACF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BotBaseobject that derieves from BotBase.

- **`Add Method (String, BotBase)`**
  ```csharp
  public void Add(
string name,
BotBase derievesFromBotBase
)
  ```
  Adds a bot to the bot collection.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LST9B274B0D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String               name of the bot.
  - *derievesFromBotBase*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LST9B274B0D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");BotBaseobject that derieves from BotBase.

- **`GetBots Method`**
  ```csharp
  [ObsoleteAttribute("Use the Bots property instead")]
public Dictionary&lt;string, BotBase&gt; GetBots()
  ```
  Returns a dictionary of all the currently loaded bots.
  - **Returns:** See Also

- **`Remove Method`**
  ```csharp
  public bool Remove(
string name
)
  ```
  Removes a bot from the collection.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LSTE49CF122_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Stringname of the bot.
  - **Returns:** ReferenceBotManager Class

- **`SetCurrent Method`**
  ```csharp
  public void SetCurrent(
BotBase bot
)
  ```
  Sets the current bot.
  - *bot*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LSTCDC86381_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BotBaseThe new bot.

#### BotManager Fields

- **`Instance Field`**
  ```csharp
  public static readonly BotManager Instance
  ```
  The instance.

---

### BuyItemsEventArgs Class

**Inheritance:** System.Object → Styx.CommonBot.BuyItemsEventArgs

```csharp
public class BuyItemsEventArgs
```

Additional information for buy items events.

#### BuyItemsEventArgs Properties

- **`BuyItemsIds Property`**
  ```csharp
  public Dictionary&lt;uint, int&gt; BuyItemsIds { get; }
  ```
  Gets a list of identifiers of the buy items.

---

### BuyItemsEventHandler Delegate

```csharp
public delegate void BuyItemsEventHandler(
BuyItemsEventArgs e
)
```

Delegate for handling BuyItems events.

---

### Chat Class

**Inheritance:** System.Object → Styx.CommonBot.Chat

```csharp
public static class Chat
```

An extended version of the WoWChat class. Exposes much more in terms of availability
            of chat messages.

#### Chat Events

- **`Addon Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatAddonEventArgs&gt; Addon
  ```
  Event queue for all listeners interested in Addon events.

- **`Afk Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatAuthoredEventArgs&gt; Afk
  ```
  Event queue for all listeners interested in Afk events.

- **`AllianceBattleground Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatSimpleMessageEventArgs&gt; AllianceBattleground
  ```
  Event queue for all listeners interested in AllianceBattleground events.

- **`Battleground Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatLanguageSpecificEventArgs&gt; Battleground
  ```
  Event queue for all listeners interested in Battleground events.

- **`BattlegroundLeader Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatLanguageSpecificEventArgs&gt; BattlegroundLeader
  ```
  Event queue for all listeners interested in BattlegroundLeader events.

- **`Channel Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatChannelSpecificEventArgs&gt; Channel
  ```
  Event queue for all listeners interested in Channel events.

- **`Dnd Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatAuthoredEventArgs&gt; Dnd
  ```
  Event queue for all listeners interested in Dnd events.

- **`Emote Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatAuthoredEventArgs&gt; Emote
  ```
  Event queue for all listeners interested in Emote events.

- **`Guild Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatGuildEventArgs&gt; Guild
  ```
  Event queue for all listeners interested in Guild events.

- **`HordeBattleground Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatSimpleMessageEventArgs&gt; HordeBattleground
  ```
  Event queue for all listeners interested in HordeBattleground events.

- **`MonsterEmote Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatMonsterEventArgs&gt; MonsterEmote
  ```
  Event queue for all listeners interested in MonsterEmote events.

- **`MonsterParty Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatMonsterEventArgs&gt; MonsterParty
  ```
  Event queue for all listeners interested in MonsterParty events.

- **`MonsterSay Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatMonsterSayEventArgs&gt; MonsterSay
  ```
  Event queue for all listeners interested in MonsterSay events.

- **`MonsterWhisper Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatMonsterEventArgs&gt; MonsterWhisper
  ```
  Event queue for all listeners interested in MonsterWhisper events.

- **`MonsterYell Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatMonsterEventArgs&gt; MonsterYell
  ```
  Event queue for all listeners interested in MonsterYell events.

- **`NeutralBattleground Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatSimpleMessageEventArgs&gt; NeutralBattleground
  ```
  Event queue for all listeners interested in NeutralBattleground events.

- **`Officer Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatLanguageSpecificEventArgs&gt; Officer
  ```
  Event queue for all listeners interested in Officer events.

- **`Party Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatLanguageSpecificEventArgs&gt; Party
  ```
  Event queue for all listeners interested in Party events.

- **`PartyLeader Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatLanguageSpecificEventArgs&gt; PartyLeader
  ```
  Event queue for all listeners interested in PartyLeader events.

- **`Raid Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatLanguageSpecificEventArgs&gt; Raid
  ```
  Event queue for all listeners interested in Raid events.

- **`RaidBossEmote Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatMonsterEventArgs&gt; RaidBossEmote
  ```
  Event queue for all listeners interested in RaidBossEmote events.

- **`RaidBossWhisper Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatMonsterEventArgs&gt; RaidBossWhisper
  ```
  Event queue for all listeners interested in RaidBossWhisper events.

- **`RaidLeader Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatLanguageSpecificEventArgs&gt; RaidLeader
  ```
  Event queue for all listeners interested in RaidLeader events.

- **`RaidWarning Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatLanguageSpecificEventArgs&gt; RaidWarning
  ```
  Event queue for all listeners interested in RaidWarning events.

- **`Say Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatLanguageSpecificEventArgs&gt; Say
  ```
  Event queue for all listeners interested in Say events.

- **`System Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatSimpleMessageEventArgs&gt; System
  ```
  Event queue for all listeners interested in System events.

- **`TextEmote Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatAuthoredEventArgs&gt; TextEmote
  ```
  Event queue for all listeners interested in TextEmote events.

- **`Whisper Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatWhisperEventArgs&gt; Whisper
  ```
  Event queue for all listeners interested in Whisper events.

- **`WhisperTo Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatLanguageSpecificEventArgs&gt; WhisperTo
  ```
  Event queue for all listeners interested in WhisperTo events.

- **`Yell Event`**
  ```csharp
  public static event Chat.ChatMessageHandlerEx&lt;Chat.ChatLanguageSpecificEventArgs&gt; Yell
  ```
  Event queue for all listeners interested in Yell events.

---

### Chat.ChatAddonEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.WoWInternals.LuaEventArgs → Styx.CommonBot.Chat.ChatMessageBaseEventArgs → Styx.CommonBot.Chat.ChatAddonEventArgs

```csharp
public class ChatAddonEventArgs : Chat.ChatMessageBaseEventArgs
```

Additional information for chat addon events.

#### ChatAddonEventArgs Properties

- **`Message Property`**
  ```csharp
  public string Message { get; }
  ```
  Gets the message.

- **`Prefix Property`**
  ```csharp
  public string Prefix { get; }
  ```
  Gets the prefix.

- **`Sender Property`**
  ```csharp
  public string Sender { get; }
  ```
  Gets the sender.

- **`Type Property`**
  ```csharp
  public string Type { get; }
  ```
  PARTY, RAID, GUILD, BATLEGROUND, or WHISPER.

---

### Chat.ChatAuthoredEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.WoWInternals.LuaEventArgs → Styx.CommonBot.Chat.ChatMessageBaseEventArgs → Styx.CommonBot.Chat.ChatSimpleMessageEventArgs → Styx.CommonBot.Chat.ChatAuthoredEventArgs → Styx.CommonBot.Chat.ChatLanguageSpecificEventArgs

```csharp
public class ChatAuthoredEventArgs : Chat.ChatSimpleMessageEventArgs
```

Additional information for chat events with an author.

#### ChatAuthoredEventArgs Properties

- **`Author Property`**
  ```csharp
  public string Author { get; }
  ```
  Gets the author.

---

### Chat.ChatChannelSpecificEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.WoWInternals.LuaEventArgs → Styx.CommonBot.Chat.ChatMessageBaseEventArgs → Styx.CommonBot.Chat.ChatSimpleMessageEventArgs → Styx.CommonBot.Chat.ChatAuthoredEventArgs → Styx.CommonBot.Chat.ChatLanguageSpecificEventArgs → Styx.CommonBot.Chat.ChatChannelSpecificEventArgs

```csharp
public class ChatChannelSpecificEventArgs : Chat.ChatLanguageSpecificEventArgs
```

Additional information for chat channel specific events.

#### ChatChannelSpecificEventArgs Properties

- **`ChannelName Property`**
  ```csharp
  public string ChannelName { get; }
  ```
  Gets the name of the channel.

- **`ChannelNameWithNumber Property`**
  ```csharp
  public string ChannelNameWithNumber { get; }
  ```
  Gets the channel name with number.

- **`ChannelNumber Property`**
  ```csharp
  public int ChannelNumber { get; }
  ```
  Gets the channel number.

- **`ChatFlags Property`**
  ```csharp
  public string ChatFlags { get; }
  ```
  Gets the chat flags.

- **`LineId Property`**
  ```csharp
  public int LineId { get; }
  ```
  Gets the identifier of the line.

- **`Target Property`**
  ```csharp
  public string Target { get; }
  ```
  Gets target for the.

- **`ZoneFlags Property`**
  ```csharp
  public int ZoneFlags { get; }
  ```
  Gets the zone flags.

---

### Chat.ChatGuildEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.WoWInternals.LuaEventArgs → Styx.CommonBot.Chat.ChatMessageBaseEventArgs → Styx.CommonBot.Chat.ChatGuildEventArgs

```csharp
public class ChatGuildEventArgs : Chat.ChatMessageBaseEventArgs
```

Additional information for chat guild events.

#### ChatGuildEventArgs Properties

- **`Author Property`**
  ```csharp
  public string Author { get; }
  ```
  Gets the author.

- **`Language Property`**
  ```csharp
  public string Language { get; }
  ```
  Gets the language.

- **`Message Property`**
  ```csharp
  public string Message { get; }
  ```
  Gets the message.

---

### Chat.ChatLanguageSpecificEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.WoWInternals.LuaEventArgs → Styx.CommonBot.Chat.ChatMessageBaseEventArgs → Styx.CommonBot.Chat.ChatSimpleMessageEventArgs → Styx.CommonBot.Chat.ChatAuthoredEventArgs → Styx.CommonBot.Chat.ChatLanguageSpecificEventArgs → Styx.CommonBot.Chat.ChatChannelSpecificEventArgs → Styx.CommonBot.Chat.ChatWhisperEventArgs

```csharp
public class ChatLanguageSpecificEventArgs : Chat.ChatAuthoredEventArgs
```

Additional information for chat language specific events.

#### ChatLanguageSpecificEventArgs Properties

- **`Language Property`**
  ```csharp
  public string Language { get; }
  ```
  Gets the language.

---

### Chat.ChatMessageBaseEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.WoWInternals.LuaEventArgs → Styx.CommonBot.Chat.ChatMessageBaseEventArgs → Styx.CommonBot.Chat.ChatAddonEventArgs → Styx.CommonBot.Chat.ChatGuildEventArgs → Styx.CommonBot.Chat.ChatMonsterEventArgs → Styx.CommonBot.Chat.ChatSimpleMessageEventArgs

```csharp
public class ChatMessageBaseEventArgs : LuaEventArgs
```

Additional information for chat message base events.

---

### Chat.ChatMessageHandlerEx(T) Delegate

```csharp
public delegate void ChatMessageHandlerEx&lt;T&gt;(
T e
)
where T : Chat.ChatMessageBaseEventArgs
```

A simple generic delegate to help simplify chat based events.

---

### Chat.ChatMonsterEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.WoWInternals.LuaEventArgs → Styx.CommonBot.Chat.ChatMessageBaseEventArgs → Styx.CommonBot.Chat.ChatMonsterEventArgs → Styx.CommonBot.Chat.ChatMonsterSayEventArgs

```csharp
public class ChatMonsterEventArgs : Chat.ChatMessageBaseEventArgs
```

Additional information for chat monster events.

#### ChatMonsterEventArgs Properties

- **`Message Property`**
  ```csharp
  public string Message { get; }
  ```
  Gets the message.

- **`MonsterName Property`**
  ```csharp
  public string MonsterName { get; }
  ```
  Gets the name of the monster.

---

### Chat.ChatMonsterSayEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.WoWInternals.LuaEventArgs → Styx.CommonBot.Chat.ChatMessageBaseEventArgs → Styx.CommonBot.Chat.ChatMonsterEventArgs → Styx.CommonBot.Chat.ChatMonsterSayEventArgs

```csharp
public class ChatMonsterSayEventArgs : Chat.ChatMonsterEventArgs
```

Additional information for chat monster say events.

#### ChatMonsterSayEventArgs Properties

- **`Language Property`**
  ```csharp
  public string Language { get; }
  ```
  Gets the language.

- **`Receiver Property`**
  ```csharp
  public string Receiver { get; }
  ```
  Gets the receiver.

---

### Chat.ChatSimpleMessageEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.WoWInternals.LuaEventArgs → Styx.CommonBot.Chat.ChatMessageBaseEventArgs → Styx.CommonBot.Chat.ChatSimpleMessageEventArgs → Styx.CommonBot.Chat.ChatAuthoredEventArgs

```csharp
public class ChatSimpleMessageEventArgs : Chat.ChatMessageBaseEventArgs
```

Additional information for chat simple message events.

#### ChatSimpleMessageEventArgs Properties

- **`Message Property`**
  ```csharp
  public string Message { get; }
  ```
  Gets the message.

---

### Chat.ChatWhisperEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.WoWInternals.LuaEventArgs → Styx.CommonBot.Chat.ChatMessageBaseEventArgs → Styx.CommonBot.Chat.ChatSimpleMessageEventArgs → Styx.CommonBot.Chat.ChatAuthoredEventArgs → Styx.CommonBot.Chat.ChatLanguageSpecificEventArgs → Styx.CommonBot.Chat.ChatWhisperEventArgs

```csharp
public class ChatWhisperEventArgs : Chat.ChatLanguageSpecificEventArgs
```

Additional information for chat whisper events.

#### ChatWhisperEventArgs Properties

- **`MessageId Property`**
  ```csharp
  public int MessageId { get; }
  ```
  Gets the identifier of the message.

- **`Status Property`**
  ```csharp
  public string Status { get; }
  ```
  Gets the status.

---

### GameStats Class

**Inheritance:** System.Object → Styx.CommonBot.GameStats

```csharp
public static class GameStats
```

A game stats.

#### GameStats Properties

- **`BGsCompleted Property`**
  ```csharp
  public static uint BGsCompleted { get; }
  ```
  Gets the gs completed.

- **`BGsLost Property`**
  ```csharp
  public static uint BGsLost { get; }
  ```
  Gets the gs lost.

- **`BGsLostPerHour Property`**
  ```csharp
  public static float BGsLostPerHour { get; }
  ```
  Gets the gs lost per hour.

- **`BGsPerHour Property`**
  ```csharp
  public static float BGsPerHour { get; }
  ```
  Gets the gs per hour.

- **`BGsWon Property`**
  ```csharp
  public static uint BGsWon { get; }
  ```
  Gets the gs won.

- **`BGsWonPerHour Property`**
  ```csharp
  public static float BGsWonPerHour { get; }
  ```
  Gets the gs won per hour.

- **`Deaths Property`**
  ```csharp
  public static uint Deaths { get; }
  ```
  Gets the deaths.

- **`DeathsPerHour Property`**
  ```csharp
  public static float DeathsPerHour { get; }
  ```
  Gets the deaths per hour.

- **`GoldGained Property`**
  ```csharp
  public static float GoldGained { get; }
  ```

- **`GoldPerHour Property`**
  ```csharp
  public static float GoldPerHour { get; }
  ```

- **`HonorGained Property`**
  ```csharp
  public static uint HonorGained { get; }
  ```
  Gets the honor gained.

- **`HonorPerHour Property`**
  ```csharp
  public static float HonorPerHour { get; }
  ```
  Gets the honor per hour.

- **`IsMeasuring Property`**
  ```csharp
  public static bool IsMeasuring { get; }
  ```
  Gets a value indicating whether this object is measuring.

- **`Loots Property`**
  ```csharp
  public static uint Loots { get; }
  ```
  Gets the loots.

- **`LootsPerHour Property`**
  ```csharp
  public static float LootsPerHour { get; }
  ```
  Gets the loots per hour.

- **`MobsKilled Property`**
  ```csharp
  public static uint MobsKilled { get; }
  ```
  Gets the mobs killed.

- **`MobsPerHour Property`**
  ```csharp
  public static float MobsPerHour { get; }
  ```
  Gets the mobs per hour.

- **`TicksPerSecond Property`**
  ```csharp
  public static float TicksPerSecond { get; }
  ```
  Gets the ticks per second.

- **`TimeToLevel Property`**
  ```csharp
  public static TimeSpan TimeToLevel { get; }
  ```
  Gets the time to level.

- **`XPPerHour Property`**
  ```csharp
  public static float XPPerHour { get; }
  ```
  Gets the XP per hour.

#### GameStats Methods

- **`BGLost Method`**
  ```csharp
  public static void BGLost()
  ```
  Background lost.

- **`BGWon Method`**
  ```csharp
  public static void BGWon()
  ```
  Background won.

- **`Died Method`**
  ```csharp
  public static void Died()
  ```
  Died this object.

- **`KilledMob Method`**
  ```csharp
  public static void KilledMob()
  ```
  Killed mob.

- **`LootedObject Method`**
  ```csharp
  public static void LootedObject()
  ```
  Increments the number of loots performed.

- **`Reset Method`**
  ```csharp
  public static void Reset()
  ```
  Resets this object.

- **`StartMeasuring Method`**
  ```csharp
  public static void StartMeasuring()
  ```
  Starts a measuring.

- **`StopMeasuring Method`**
  ```csharp
  public static void StopMeasuring()
  ```
  Stops a measuring.

#### GameStats Events

- **`OnInfoPanelUpdated Event`**
  ```csharp
  public static event GameStats.InfoPanelUpdatedDelegate OnInfoPanelUpdated
  ```
  Event queue for all listeners interested in OnInfoPanelUpdated events.

---

### GameStats.InfoPanelUpdatedDelegate Delegate

```csharp
public delegate void InfoPanelUpdatedDelegate()
```

Information panel updated delegate.

---

### GoalTextChangedEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.CommonBot.GoalTextChangedEventArgs

```csharp
public class GoalTextChangedEventArgs : EventArgs
```

Gets or sets the new goal.

#### GoalTextChangedEventArgs Properties

- **`NewGoal Property`**
  ```csharp
  public string NewGoal { get; }
  ```
  Gets or sets the new goal.

- **`OldGoal Property`**
  ```csharp
  public string OldGoal { get; }
  ```
  Gets or sets the old goal.

---

### HealTargeting Class

**Inheritance:** System.Object → Styx.CommonBot.Targeting → Styx.CommonBot.HealTargeting

```csharp
public class HealTargeting : Targeting
```

A heal targeting.

#### HealTargeting Properties

- **`Instance Property`**
  ```csharp
  public static HealTargeting Instance { get; set; }
  ```
  Gets or sets the instance.

#### HealTargeting Methods

- **`DefaultIncludeTargetsFilter Method`**
  ```csharp
  protected override void DefaultIncludeTargetsFilter(
List&lt;WoWObject&gt; incomingUnits,
HashSet&lt;WoWObject&gt; outgoingUnits
)
  ```
  - *incomingUnits*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST7DF203C8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST7DF203C8_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST7DF203C8_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *outgoingUnits*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST7DF203C8_4?cs=.|vb=.|cpp=::|nu=.|fs=.");HashSetAddLanguageSpecificTextSet("LST7DF203C8_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST7DF203C8_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`DefaultRemoveTargetsFilter Method`**
  ```csharp
  protected override void DefaultRemoveTargetsFilter(
List&lt;WoWObject&gt; units
)
  ```
  - *units*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTDECAD11F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LSTDECAD11F_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LSTDECAD11F_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`DefaultTargetWeight Method`**
  ```csharp
  protected override void DefaultTargetWeight(
List&lt;Targeting.TargetPriority&gt; units
)
  ```
  - *units*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST78933600_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST78933600_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TargetingAddLanguageSpecificTextSet("LST78933600_4?cs=.|vb=.|cpp=::|nu=.|fs=.");TargetPriorityAddLanguageSpecificTextSet("LST78933600_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`GetInitialObjectList Method`**
  ```csharp
  protected override List&lt;WoWObject&gt; GetInitialObjectList()
  ```
  - **Returns:** See Also

- **`Pulse Method`**
  ```csharp
  public override void Pulse()
  ```
  Pulses this object.

---

### HonorbuddyExitCode Enumeration

```csharp
public enum HonorbuddyExitCode
```

Represents exit codes used by Honorbuddy and its subsystems.

---

### InactivityDetector Class

**Inheritance:** System.Object → Styx.CommonBot.InactivityDetector

```csharp
public static class InactivityDetector
```

Handles logging out if we are inactive for a certain period of time.

#### InactivityDetector Properties

- **`LogoutTime Property`**
  ```csharp
  public static DateTime LogoutTime { get; }
  ```
  The exact time that the bot will log out or quit.

- **`TimeUntilLogout Property`**
  ```csharp
  public static TimeSpan TimeUntilLogout { get; }
  ```
  Time remaining until the bot will log out, or quit.

#### InactivityDetector Methods

- **`Init Method`**
  ```csharp
  public static bool Init()
  ```
  Initializes this object.
  - **Returns:** ReferenceInactivityDetector Class

- **`Reset Method`**
  ```csharp
  public static void Reset()
  ```
  Resets the inactivity timer.

---

### IncludeTargetsFilterDelegate Delegate

```csharp
public delegate void IncludeTargetsFilterDelegate(
List&lt;WoWObject&gt; incomingUnits,
HashSet&lt;WoWObject&gt; outgoingUnits
)
```

A filter for the target list. This filter adds all potential targets to the outgoing
            units.

---

### KnownFlightNodesManager Class

**Inheritance:** System.Object → Styx.CommonBot.KnownFlightNodesManager

```csharp
public static class KnownFlightNodesManager
```

Manages the known flight paths from a file on disk.

#### KnownFlightNodesManager Properties

- **`ElapsedSinceUpdate Property`**
  ```csharp
  public static TimeSpan ElapsedSinceUpdate { get; }
  ```
  Gets the time elapsed since the last time UpdateKnownFlightNodes(.Byte) was called
            on the current map.

- **`Mask Property`**
  ```csharp
  public static IReadOnlyList&lt;byte&gt; Mask { get; }
  ```
  Gets the mask for known flights for this toon.

#### KnownFlightNodesManager Methods

- **`ClearUpdateTime Method`**
  ```csharp
  public static void ClearUpdateTime()
  ```
  Clears the update time for the current map.
            Depending on the bot being used, this will cause HB
            to go to the next flight master it sees to update known flight nodes.

- **`EnumerateKnownFlightNodes Method`**
  ```csharp
  public static IEnumerable&lt;TaxiNode&gt; EnumerateKnownFlightNodes()
  ```
  Enumerates the flight nodes currently known.
            This is updated after UpdateKnownFlightNodes(.Byte) is called.
  - **Returns:** See Also

- **`UpdateKnownFlightNodes Method`**
  ```csharp
  public static void UpdateKnownFlightNodes(
byte[] newMask
)
  ```
  Updates the current known flight nodes from the specified mask.
  - *newMask*: Type: AddLanguageSpecificTextSet("LST50BE30DB_1?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST50BE30DB_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ByteAddLanguageSpecificTextSet("LST50BE30DB_3?cpp=&gt;|vb=()|nu=[]");The new mask. The flights known in this mask are added on top of
            the ones currently known.

---

### Landmarks Class

**Inheritance:** System.Object → Styx.CommonBot.Landmarks

```csharp
public class Landmarks
```

A landmarks.

#### Landmarks Properties

- **`NumMapLandmarks Property`**
  ```csharp
  public int NumMapLandmarks { get; }
  ```
  Returns the number of visible MapLandmarks.

#### Landmarks Methods

- **`Display Method`**
  ```csharp
  public void Display()
  ```
  Displays all visible landmarks.

- **`GetLandmarkById Method`**
  ```csharp
  public WoWLandMark GetLandmarkById(
uint id
)
  ```
  Gets landmark by identifier.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST68636325_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The identifier.
  - **Returns:** ReferenceLandmarks Class

- **`Refresh Method`**
  ```csharp
  public void Refresh()
  ```
  Refreshes the list if landmarks.

#### Landmarks Fields

- **`LandmarkList Field`**
  ```csharp
  public readonly List&lt;WoWLandMark&gt; LandmarkList
  ```
  List of landmarks.

---

### LootPredictor Class

**Inheritance:** System.Object → Styx.CommonBot.LootPredictor

```csharp
public static class LootPredictor
```

#### LootPredictor Properties

- **`UseLootPredictor Property`**
  ```csharp
  public static bool UseLootPredictor { get; set; }
  ```

---

### LootTargeting Class

**Inheritance:** System.Object → Styx.CommonBot.Targeting → Styx.CommonBot.LootTargeting

```csharp
public class LootTargeting : Targeting
```

A loot targeting.

#### LootTargeting Properties

- **`FirstObject Property`**
  ```csharp
  public WoWObject FirstObject { get; }
  ```
  Gets the first object.

- **`HarvestHerbs Property`**
  ```csharp
  public static bool HarvestHerbs { get; }
  ```
  Gets a value indicating whether the harvest herbs.

- **`HarvestMinerals Property`**
  ```csharp
  public static bool HarvestMinerals { get; }
  ```
  Gets a value indicating whether the harvest minerals.

- **`Instance Property`**
  ```csharp
  public static LootTargeting Instance { get; }
  ```
  Gets the instance.

- **`LootChests Property`**
  ```csharp
  public static bool LootChests { get; }
  ```
  Gets a value indicating whether the loot chests.

- **`LootFrameIsOpen Property`**
  ```csharp
  public static bool LootFrameIsOpen { get; }
  ```
  Gets a value indicating whether the loot frame is open.

- **`LootingList Property`**
  ```csharp
  public List&lt;WoWObject&gt; LootingList { get; }
  ```
  Gets a list of lootings.

- **`LootMobs Property`**
  ```csharp
  public static bool LootMobs { get; }
  ```
  Gets a value indicating whether the loot mobs.

- **`LootRadius Property`**
  ```csharp
  public static double LootRadius { get; }
  ```
  Gets the loot radius.

- **`NinjaSkin Property`**
  ```csharp
  public static bool NinjaSkin { get; }
  ```
  Gets a value indicating whether the ninja skin.

- **`SkinMobs Property`**
  ```csharp
  public static bool SkinMobs { get; }
  ```
  Gets a value indicating whether the skin mobs.

#### LootTargeting Methods

- **`DefaultIncludeTargetsFilter Method`**
  ```csharp
  protected override void DefaultIncludeTargetsFilter(
List&lt;WoWObject&gt; incomingObjects,
HashSet&lt;WoWObject&gt; outgoingObjects
)
  ```
  - *incomingObjects*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST4EAE70AC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST4EAE70AC_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST4EAE70AC_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *outgoingObjects*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST4EAE70AC_4?cs=.|vb=.|cpp=::|nu=.|fs=.");HashSetAddLanguageSpecificTextSet("LST4EAE70AC_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST4EAE70AC_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`DefaultRemoveTargetsFilter Method`**
  ```csharp
  protected override void DefaultRemoveTargetsFilter(
List&lt;WoWObject&gt; objects
)
  ```
  - *objects*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTDF47CF4F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LSTDF47CF4F_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LSTDF47CF4F_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`DefaultTargetWeight Method`**
  ```csharp
  protected override void DefaultTargetWeight(
List&lt;Targeting.TargetPriority&gt; objs
)
  ```
  - *objs*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTDF1D1CEC_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LSTDF1D1CEC_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TargetingAddLanguageSpecificTextSet("LSTDF1D1CEC_4?cs=.|vb=.|cpp=::|nu=.|fs=.");TargetPriorityAddLanguageSpecificTextSet("LSTDF1D1CEC_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`GetInitialObjectList Method`**
  ```csharp
  protected override List&lt;WoWObject&gt; GetInitialObjectList()
  ```
  - **Returns:** See Also

---

### MailItemsEventArgs Class

**Inheritance:** System.Object → Styx.CommonBot.MailItemsEventArgs

```csharp
public class MailItemsEventArgs
```

Additional information for mail items events.

#### MailItemsEventArgs Properties

- **`AdditionalItems Property`**
  ```csharp
  public List&lt;WoWItem&gt; AdditionalItems { get; }
  ```
  Gets the additional items.

---

### MailItemsEventHandler Delegate

```csharp
public delegate void MailItemsEventHandler(
MailItemsEventArgs e
)
```

Delegate for handling MailItems events.

---

### Mount Class

**Inheritance:** System.Object → Styx.CommonBot.Mount

```csharp
public static class Mount
```

A mount.

#### Mount Properties

- **`Current Property`**
  ```csharp
  public static Mount.MountWrapper Current { get; }
  ```
  Gets the current mount. This includes mounts such as
            travel form and running wild.

- **`CurrentMountSpell Property`**
  ```csharp
  public static WoWSpell CurrentMountSpell { get; }
  ```
  Gets the current spell for the mount. This can also be non-mount spells,
            like travel form.

- **`FlyingMounts Property`**
  ```csharp
  public static List&lt;Mount.MountWrapper&gt; FlyingMounts { get; }
  ```
  Gets the known flying mounts See Mounts.

- **`GroundMounts Property`**
  ```csharp
  public static List&lt;Mount.MountWrapper&gt; GroundMounts { get; }
  ```
  Gets the known ground mounts. See Mounts.

- **`HasHeirloomMount Property`**
  ```csharp
  [ObsoleteAttribute("This will be removed in the future.")]
public static bool HasHeirloomMount { get; }
  ```

- **`Mounts Property`**
  ```csharp
  public static List&lt;Mount.MountWrapper&gt; Mounts { get; }
  ```
  Gets all the mounts known by the player.
            This can also include spells that can act as mounts,
            like Travel Form and Running Wild.

- **`NumMounts Property`**
  ```csharp
  [ObsoleteAttribute("Do not use. Will be removed in the future.")]
public static int NumMounts { get; }
  ```
  Gets the number of mounts.

- **`UnderwaterMounts Property`**
  ```csharp
  [ObsoleteAttribute("Always returns an empty list. Will be removed in the future.")]
public static List&lt;Mount.MountWrapper&gt; UnderwaterMounts { get; }
  ```
  Gets the underwater mounts.

- **`UsableMounts Property`**
  ```csharp
  public static List&lt;Mount.MountWrapper&gt; UsableMounts { get; }
  ```
  Gets all the known mounts that are usable right now.
            See Mounts for more information.

- **`UseMount Property`**
  ```csharp
  public static bool UseMount { get; }
  ```
  Gets a value indicating whether this object use mount.

#### Mount Methods

- **`AddCantMountSpot Method`**
  ```csharp
  [ObsoleteAttribute("This is no longer used and will be removed in the future.")]
public static void AddCantMountSpot(
Vector3 loc
)
  ```
  We store the area where we can't mount.
  - *loc*: Type: System.NumericsAddLanguageSpecificTextSet("LST84EF024E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3

- **`CanMount Method`**
  ```csharp
  public static bool CanMount()
  ```
  Determine if we can mount.
  - **Returns:** ReferenceMount Class

- **`GetGroundMountSpell Method`**
  ```csharp
  public static WoWSpell GetGroundMountSpell()
  ```
  Gets the mount spell used for the ground mount.
            This can be a flying mount if the user has chosen so,
            or if no ground mounts are available.
            Returns null if no mount are usable for us currently.
  - **Returns:** ReferenceMount Class

- **`GetMountSpell Method`**
  ```csharp
  [ObsoleteAttribute("Use GetGroundMountSpell.")]
public static WoWSpell GetMountSpell(
bool includeFlyingMounts = false
)
  ```
  Gets the mount spell.
  - **Returns:** ReferenceMount Class

- **`ShouldDismount Method`**
  ```csharp
  public static bool ShouldDismount(
Vector3 travelingTo
)
  ```
  Determine if we should dismount.
  - *travelingTo*: Type: System.NumericsAddLanguageSpecificTextSet("LST6AB77D6A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The traveling to.
  - **Returns:** ReferenceMount Class

- **`SummonMount Method`**
  ```csharp
  public static bool SummonMount(
int mountSpellId
)
  ```
  Uses lua to summon the mount. This does no checks.
  - *mountSpellId*: Type: SystemAddLanguageSpecificTextSet("LSTF21F4370_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32CreatureSpellId of the mount.
  - **Returns:** raphus, 16/10/2013.

#### Mount Events

- **`OnDismount Event`**
  ```csharp
  [ObsoleteAttribute("Use lua events instead. This will be removed in the future.")]
public static event EventHandler&lt;EventArgs&gt; OnDismount
  ```
  Fired when the player has finished dismounting.

- **`OnMountUp Event`**
  ```csharp
  public static event EventHandler&lt;MountUpEventArgs&gt; OnMountUp
  ```
  Fired when the player starts to mount.

---

### Mount.MountWrapper Class

**Inheritance:** System.Object → Styx.CommonBot.Mount.MountWrapper

```csharp
public sealed class MountWrapper
```

A mount wrapper.

#### MountWrapper Properties

- **`CanMount Property`**
  ```csharp
  [ObsoleteAttribute("Do not use - Mount.Mounts only returns usable mounts, so get a new snapshot instead.This will be removed soon.")]
public bool CanMount { get; }
  ```
  Gets a value indicating whether we can mount.

- **`CreatureSpell Property`**
  ```csharp
  public WoWSpell CreatureSpell { get; }
  ```
  Gets the creature spell.

- **`CreatureSpellId Property`**
  ```csharp
  public int CreatureSpellId { get; }
  ```
  Gets the identifier of the creature spell.

- **`Icon Property`**
  ```csharp
  [ObsoleteAttribute("Always returns empty string. Will be removed in the future.")]
public string Icon { get; }
  ```
  Gets the icon.

- **`IsClassMount Property`**
  ```csharp
  [ObsoleteAttribute("Do not use. Will be removed in the future.")]
public bool IsClassMount { get; }
  ```
  Gets a value indicating whether this instance is a class mount.

- **`IsCloudSerpentMount Property`**
  ```csharp
  [ObsoleteAttribute("Do not use. Will be removed in the future.")]
public bool IsCloudSerpentMount { get; }
  ```
  Gets a value indicating whether this instance is a cloud serpent mount.

- **`IsFlyingMount Property`**
  ```csharp
  public bool IsFlyingMount { get; }
  ```
  Gets a value indicating whether this instance is a flying mount.

- **`IsGroundMount Property`**
  ```csharp
  public bool IsGroundMount { get; }
  ```
  Gets a value indicating whether this instance is a ground mount.

- **`IsHybridMount Property`**
  ```csharp
  [ObsoleteAttribute("Do not use. Will be removed in the future.")]
public bool IsHybridMount { get; }
  ```
  Gets a value indicating whether this instance is a hybrid mount.

- **`IsUnderwaterMount Property`**
  ```csharp
  [ObsoleteAttribute("Do not use. Always returns false.")]
public bool IsUnderwaterMount { get; }
  ```
  Gets a value indicating whether this instance is an underwater mount.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`Type Property`**
  ```csharp
  public MountType Type { get; }
  ```
  Gets the type.

#### MountWrapper Methods

- **`Equals Method`**
  ```csharp
  public override bool Equals(
Object obj
)
  ```
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LST3836F2EE_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceMountAddLanguageSpecificTextSet("LST3836F2EE_3?cs=.|vb=.|cpp=::|nu=.|fs=.");MountWrapper Class

- **`FromSpellId Method`**
  ```csharp
  [ObsoleteAttribute("Do not use. Will be removed in the future.")]
public static Mount.MountWrapper FromSpellId(
int spellId
)
  ```
  Returns a MountWrapper from a spell Id.
  - *spellId*: Type: SystemAddLanguageSpecificTextSet("LST492D8C53_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The spell identifier.
  - **Returns:** See Also

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  - **Returns:** ReferenceMountAddLanguageSpecificTextSet("LST5CD8D90F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");MountWrapper Class

#### MountWrapper Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
Mount.MountWrapper left,
Mount.MountWrapper right
)
  ```
  - *left*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LST1AC325A3_4?cs=.|vb=.|cpp=::|nu=.|fs=.");MountAddLanguageSpecificTextSet("LST1AC325A3_5?cs=.|vb=.|cpp=::|nu=.|fs=.");MountWrapper
  - *right*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LST1AC325A3_6?cs=.|vb=.|cpp=::|nu=.|fs=.");MountAddLanguageSpecificTextSet("LST1AC325A3_7?cs=.|vb=.|cpp=::|nu=.|fs=.");MountWrapper
  - **Returns:** ReferenceMountAddLanguageSpecificTextSet("LST1AC325A3_8?cs=.|vb=.|cpp=::|nu=.|fs=.");MountWrapper Class

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
Mount.MountWrapper left,
Mount.MountWrapper right
)
  ```
  - *left*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LSTB42B2BA_4?cs=.|vb=.|cpp=::|nu=.|fs=.");MountAddLanguageSpecificTextSet("LSTB42B2BA_5?cs=.|vb=.|cpp=::|nu=.|fs=.");MountWrapper
  - *right*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LSTB42B2BA_6?cs=.|vb=.|cpp=::|nu=.|fs=.");MountAddLanguageSpecificTextSet("LSTB42B2BA_7?cs=.|vb=.|cpp=::|nu=.|fs=.");MountWrapper
  - **Returns:** ReferenceMountAddLanguageSpecificTextSet("LSTB42B2BA_8?cs=.|vb=.|cpp=::|nu=.|fs=.");MountWrapper Class

---

### MountType Enumeration

```csharp
public enum MountType
```

Values that represent MountType.

---

### MountUpEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.CommonBot.MountUpEventArgs

```csharp
public class MountUpEventArgs : EventArgs
```

Additional information for mount up events.

#### MountUpEventArgs Properties

- **`Cancel Property`**
  ```csharp
  public bool Cancel { get; set; }
  ```
  Gets or sets a boolean indicating whether to cancel this mount up.

- **`Destination Property`**
  ```csharp
  public Nullable&lt;Vector3&gt; Destination { get; }
  ```
  Gets the destination we are expecting to.

- **`MoveDistance Property`**
  ```csharp
  public Nullable&lt;float&gt; MoveDistance { get; }
  ```
  Gets the distance we are expecting to move after this mount up.

---

### PulseFlags Enumeration

```csharp
[FlagsAttribute]
public enum PulseFlags
```

Bitfield of flags for specifying PulseFlags.

---

### RaFHelper Class

**Inheritance:** System.Object → Styx.CommonBot.RaFHelper

```csharp
public static class RaFHelper
```

A ra f helper.

#### RaFHelper Properties

- **`Leader Property`**
  ```csharp
  public static WoWPlayer Leader { get; }
  ```
  Gets the leader.

#### RaFHelper Methods

- **`ClearLeader Method`**
  ```csharp
  public static void ClearLeader()
  ```
  Clears the leader.

- **`SetLeader Method`**
  Sets a leader.

- **`SetLeader Method (String)`**
  ```csharp
  [ObsoleteAttribute("Use ObjectManager.GetUnitByKeyword and SetLeader(WoWPlayer) instead")]
public static void SetLeader(
string keyword
)
  ```
  Sets a leader.
  - *keyword*: Type: SystemAddLanguageSpecificTextSet("LST376A9126_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe keyword.

- **`SetLeader Method (UInt32)`**
  ```csharp
  [ObsoleteAttribute("Use SetLeader(WoWPlayer) instead")]
public static void SetLeader(
uint ptr
)
  ```
  Sets a leader.
  - *ptr*: Type: SystemAddLanguageSpecificTextSet("LST7000B9AE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The pointer.

- **`SetLeader Method (WoWGuid)`**
  ```csharp
  [ObsoleteAttribute("Use ObjectManager.GetObjectByGuid and SetLeader(WoWPlayer) instead")]
public static void SetLeader(
WoWGuid guid
)
  ```
  Sets a leader.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST586DB2FD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidUnique identifier.

- **`SetLeader Method (WoWPlayer)`**
  ```csharp
  public static void SetLeader(
WoWPlayer unit
)
  ```
  Sets a leader.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST94A1007E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPlayerThe unit.

---

### RemoveTargetsFilterDelegate Delegate

```csharp
public delegate void RemoveTargetsFilterDelegate(
List&lt;WoWObject&gt; units
)
```

A filter for the target list. This filter should remove invalid units from the list.
            This is called before the include targets filter.

---

### Rest Class

**Inheritance:** System.Object → Styx.CommonBot.Rest

```csharp
public static class Rest
```

A rest.

#### Rest Properties

- **`NoDrink Property`**
  ```csharp
  [ObsoleteAttribute("Write your own rest behaviors")]
public static bool NoDrink { get; }
  ```
  Gets a value indicating whether the no drink.

- **`NoFood Property`**
  ```csharp
  [ObsoleteAttribute("Write your own rest behaviors")]
public static bool NoFood { get; }
  ```
  Gets a value indicating whether the no food.

- **`RestPercentageHealth Property`**
  ```csharp
  [ObsoleteAttribute("Write your own rest behaviors")]
public static double RestPercentageHealth { get; set; }
  ```
  Gets or sets the rest percentage health.

- **`RestPercentageMana Property`**
  ```csharp
  [ObsoleteAttribute("Write your own rest behaviors")]
public static double RestPercentageMana { get; set; }
  ```
  Gets or sets the rest percentage mana.

#### Rest Methods

- **`DrinkImmediate Method`**
  ```csharp
  public static void DrinkImmediate()
  ```
  Attempts to eat the best food currently in your bags. It will only attempt to eat
            once every 5 seconds. This method will return immediately. This method does not check for
            combat, or other contextual events!

- **`Feed Method`**
  ```csharp
  [ObsoleteAttribute("Write your own rest behaviors")]
public static void Feed()
  ```
  Feed this object.

- **`FeedImmediate Method`**
  ```csharp
  public static void FeedImmediate()
  ```
  Attempts to eat the best food currently in your bags. It will only attempt to eat
            once every 5 seconds. This method will return immediately. This method does not check for
            combat, or other contextual events!

---

### SellItemsEventArgs Class

**Inheritance:** System.Object → Styx.CommonBot.SellItemsEventArgs

```csharp
public class SellItemsEventArgs
```

Additional information for sell items events.

#### SellItemsEventArgs Properties

- **`IdExceptions Property`**
  ```csharp
  public List&lt;uint&gt; IdExceptions { get; }
  ```
  Gets the identifier exceptions.

- **`NameExceptions Property`**
  ```csharp
  public List&lt;string&gt; NameExceptions { get; }
  ```
  Gets the name exceptions.

---

### ShutdownRequestedEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.CommonBot.ShutdownRequestedEventArgs

```csharp
public class ShutdownRequestedEventArgs : EventArgs
```

Initializes a new instance of the ShutdownRequestedEventArgs class

#### ShutdownRequestedEventArgs Properties

- **`CloseWow Property`**
  ```csharp
  public bool CloseWow { get; set; }
  ```

- **`ExitCode Property`**
  ```csharp
  public HonorbuddyExitCode ExitCode { get; set; }
  ```

---

### SpellFindResults Class

**Inheritance:** System.Object → Styx.CommonBot.SpellFindResults

```csharp
public class SpellFindResults
```

A spell find results.

#### SpellFindResults Properties

- **`Original Property`**
  ```csharp
  public WoWSpell Original { get; }
  ```
  The original spell.

- **`Override Property`**
  ```csharp
  public WoWSpell Override { get; }
  ```
  The override spell.

---

### SpellManager Class

**Inheritance:** System.Object → Styx.CommonBot.SpellManager

```csharp
public static class SpellManager
```

A wrapper class to help in casting spells.

#### SpellManager Properties

- **`GlobalCooldown Property`**
  ```csharp
  public static bool GlobalCooldown { get; }
  ```
  Returns true if the Global Cooldown timer is running.

- **`GlobalCooldownLeft Property`**
  ```csharp
  public static TimeSpan GlobalCooldownLeft { get; }
  ```
  Returns the amount of time left on Global Cooldown.

- **`Spells Property`**
  ```csharp
  public static IReadOnlyDictionary&lt;string, WoWSpell&gt; Spells { get; }
  ```
  Gets the known spells using the spell name as the unique identifier.

#### SpellManager Methods

- **`CanCast Method`**
  Determines whether we can cast the specified spell on current target.

- **`CanCast Method (Int32)`**
  ```csharp
  public static bool CanCast(
int spellId
)
  ```
  Determines whether we can cast the specified spell on current target.
  - *spellId*: Type: SystemAddLanguageSpecificTextSet("LST61720BFB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The spell id.
  - **Returns:** Remarks

- **`CanCast Method (String)`**
  ```csharp
  public static bool CanCast(
string spellName
)
  ```
  Determines whether we can cast the specified spell on current target.
  - *spellName*: Type: SystemAddLanguageSpecificTextSet("LST9D980AB4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the spell.
  - **Returns:** Remarks

- **`CanCast Method (WoWSpell)`**
  ```csharp
  public static bool CanCast(
WoWSpell spell
)
  ```
  Determines whether we can cast the specified spell on current target.
  - *spell*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST67E8AB2A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpellThe spell.
  - **Returns:** Remarks

- **`CanCast Method (Int32, WoWUnit)`**
  ```csharp
  public static bool CanCast(
int spellId,
WoWUnit target
)
  ```
  Determines whether we can cast the specified spell on target.
  - *spellId*: Type: SystemAddLanguageSpecificTextSet("LSTF188441C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The spell id.
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTF188441C_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnit The target.
  - **Returns:** Remarks

- **`CanCast Method (Int32, Boolean)`**
  ```csharp
  public static bool CanCast(
int spellId,
bool checkRange
)
  ```
  Determines whether we can cast the specified spell on current target.
  - *spellId*: Type: SystemAddLanguageSpecificTextSet("LSTD5EB855E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32   The spell id.
  - *checkRange*: Type: SystemAddLanguageSpecificTextSet("LSTD5EB855E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Booleanif set to true [check range].
  - **Returns:** Remarks

- **`CanCast Method (String, WoWUnit)`**
  ```csharp
  public static bool CanCast(
string spellName,
WoWUnit target
)
  ```
  Determines whether we can cast the specified spell on target.
  - *spellName*: Type: SystemAddLanguageSpecificTextSet("LSTEF13CAC7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the spell.
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTEF13CAC7_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe target.
  - **Returns:** Remarks

- **`CanCast Method (String, Boolean)`**
  ```csharp
  public static bool CanCast(
string spellName,
bool checkRange
)
  ```
  Determines whether we can cast the specified spell on current target
  - *spellName*: Type: SystemAddLanguageSpecificTextSet("LST25F783AF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String Name of the spell.
  - *checkRange*: Type: SystemAddLanguageSpecificTextSet("LST25F783AF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Booleanif set to true [check range].
  - **Returns:** Remarks

- **`CanCast Method (WoWSpell, WoWUnit)`**
  ```csharp
  public static bool CanCast(
WoWSpell spell,
WoWUnit target
)
  ```
  Determines whether we can cast the specified spell.
  - *spell*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTCDA4D4ED_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpell The spell.
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTCDA4D4ED_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe target.
  - **Returns:** Remarks

- **`CanCast Method (WoWSpell, Boolean)`**
  ```csharp
  public static bool CanCast(
WoWSpell spell,
bool checkRange
)
  ```
  Determines whether we can cast the specified spell on current target.
  - *spell*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTFA358D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpell     The spell.
  - *checkRange*: Type: SystemAddLanguageSpecificTextSet("LSTFA358D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Booleanif set to true [check range].
  - **Returns:** Remarks

- **`CanCast Method (Int32, WoWUnit, Boolean)`**
  ```csharp
  public static bool CanCast(
int spellId,
WoWUnit target,
bool checkRange
)
  ```
  Determines whether we can cast the specified spell on target.
  - *spellId*: Type: SystemAddLanguageSpecificTextSet("LSTABB1FD57_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32   The spell id.
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTABB1FD57_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnit    The target.
  - *checkRange*: Type: SystemAddLanguageSpecificTextSet("LSTABB1FD57_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Booleanif set to true [check range].
  - **Returns:** Remarks

- **`CanCast Method (String, WoWUnit, Boolean)`**
  ```csharp
  public static bool CanCast(
string spellName,
WoWUnit target,
bool checkRange
)
  ```
  Determines whether we can cast the specified spell.
  - *spellName*: Type: SystemAddLanguageSpecificTextSet("LST90448BC4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String Name of the spell.
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST90448BC4_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnit    The target.
  - *checkRange*: Type: SystemAddLanguageSpecificTextSet("LST90448BC4_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Booleanif set to true [check range].
  - **Returns:** Remarks

- **`CanCast Method (String, WoWUnit, Boolean, Boolean)`**
  ```csharp
  public static bool CanCast(
string spellName,
WoWUnit target,
bool checkRange,
bool checkMoving
)
  ```
  Determines whether we can cast the specified spell.
  - *spellName*: Type: SystemAddLanguageSpecificTextSet("LSTB5EEBD3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String  Name of the spell.
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTB5EEBD3_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnit     The target.
  - *checkRange*: Type: SystemAddLanguageSpecificTextSet("LSTB5EEBD3_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean if set to true [check range].
  - *checkMoving*: Type: SystemAddLanguageSpecificTextSet("LSTB5EEBD3_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Booleanif set to true [check moving].
  - **Returns:** Remarks

- **`CanCast Method (Int32, WoWUnit, Boolean, Boolean, Boolean)`**
  ```csharp
  public static bool CanCast(
int spellId,
WoWUnit target,
bool checkRange,
bool checkMoving,
bool accountForLagTolerance = true
)
  ```
  Determines whether we can cast the specified spell.
  - *spellId*: Type: SystemAddLanguageSpecificTextSet("LST1FFD875B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32                 The spell.
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST1FFD875B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnit                The target.
  - *checkRange*: Type: SystemAddLanguageSpecificTextSet("LST1FFD875B_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean            if set to true [check range].
  - *checkMoving*: Type: SystemAddLanguageSpecificTextSet("LST1FFD875B_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean           if set to true [check moving].
  - **Returns:** See Also

- **`CanCast Method (WoWSpell, WoWUnit, Boolean, Boolean, Boolean)`**
  ```csharp
  public static bool CanCast(
WoWSpell spell,
WoWUnit target,
bool checkRange,
bool checkMoving = true,
bool accountForLagTolerance = true
)
  ```
  Determines whether we can cast the specified spell.
  - *spell*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST8961FD2C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpell                 The spell.
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST8961FD2C_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnit                The target.
  - *checkRange*: Type: SystemAddLanguageSpecificTextSet("LST8961FD2C_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean            if set to true [check range].
  - **Returns:** See Also

- **`Cast Method`**
  Attempts to cast the specified spell. Does not check if spell can be casted beforehand

- **`Cast Method (Int32, WoWUnit)`**
  ```csharp
  public static bool Cast(
int spellId,
WoWUnit target = null
)
  ```
  Attempts to cast the specified spell. Does not check if spell can be casted beforehand
  - *spellId*: Type: SystemAddLanguageSpecificTextSet("LST6F9076E2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The spell id.
  - **Returns:** ReferenceSpellManager Class

- **`Cast Method (String, WoWUnit)`**
  ```csharp
  public static bool Cast(
string spellName,
WoWUnit target = null
)
  ```
  Casts the specified spell.
  - *spellName*: Type: SystemAddLanguageSpecificTextSet("LST2AAA6B7B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the spell.
  - **Returns:** ReferenceSpellManager Class

- **`Cast Method (WoWSpell, WoWUnit)`**
  ```csharp
  public static bool Cast(
WoWSpell spell,
WoWUnit target = null
)
  ```
  Attempts to cast the specified spell on target. Does not check if spell can be casted beforehand
  - *spell*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTDCD323FD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpell The spell.
  - **Returns:** ReferenceSpellManager Class

- **`CastSpellById Method`**
  Casts a spell by id.

- **`CastSpellById Method (Int32)`**
  ```csharp
  public static bool CastSpellById(
int spellID
)
  ```
  Casts a spell by id.
  - *spellID*: Type: SystemAddLanguageSpecificTextSet("LST7013BEB3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The spell ID.
  - **Returns:** ReferenceSpellManager Class

- **`CastSpellById Method (UInt32)`**
  ```csharp
  public static bool CastSpellById(
uint spellID
)
  ```
  Casts a spell by id.
  - *spellID*: Type: SystemAddLanguageSpecificTextSet("LSTF872ACE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The spell ID.
  - **Returns:** ReferenceSpellManager Class

- **`CastSpellById Method (Int32, WoWGuid)`**
  ```csharp
  public static bool CastSpellById(
int spellID,
WoWGuid targetGuid
)
  ```
  Casts a spell by id.
  - *spellID*: Type: SystemAddLanguageSpecificTextSet("LSTC085F438_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32   The spell ID.
  - *targetGuid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTC085F438_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidThe target GUID.
  - **Returns:** ReferenceSpellManager Class

- **`CastSpellById Method (Int32, WoWUnit)`**
  ```csharp
  public static bool CastSpellById(
int spellID,
WoWUnit target
)
  ```
  Casts a spell by ID.
  - *spellID*: Type: SystemAddLanguageSpecificTextSet("LSTC2807EF2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The spell id.
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTC2807EF2_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnit The target.
  - **Returns:** ReferenceSpellManager Class

- **`ClickRemoteLocation Method`**
  ```csharp
  public static bool ClickRemoteLocation(
Vector3 location
)
  ```
  Clicks a remote location. Useful for using spells like Rain of Fire, which require
            you to press the ground.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST66EA5B4B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location to press.
  - **Returns:** ReferenceSpellManager Class

- **`FindSpell Method`**
  Finds a spell, by override or original, by spell ID. Returns true if the spell was
            found.

- **`FindSpell Method (Int32, SpellFindResults)`**
  ```csharp
  public static bool FindSpell(
int id,
out SpellFindResults results
)
  ```
  Finds a spell, by override or original, by spell ID. Returns true if the spell was
            found.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST3598A8F8_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32     The id of the spell to find.
  - *results*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LST3598A8F8_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SpellFindResultsAddLanguageSpecificTextSet("LST3598A8F8_4?cpp=%");[out] The search results for the spell.
  - **Returns:** ReferenceSpellManager Class

- **`FindSpell Method (String, SpellFindResults)`**
  ```csharp
  public static bool FindSpell(
string name,
out SpellFindResults results
)
  ```
  Finds a spell, by override or original, by name. Returns true if the spell was found.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LST5581405_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String   The name of the spell to find.
  - *results*: Type: Styx.CommonBotAddLanguageSpecificTextSet("LST5581405_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SpellFindResultsAddLanguageSpecificTextSet("LST5581405_4?cpp=%");[out] The search results for the spell.
  - **Returns:** ReferenceSpellManager Class

- **`HasSpell Method`**
  Determines whether we have the specified spell.

- **`HasSpell Method (Int32)`**
  ```csharp
  public static bool HasSpell(
int id
)
  ```
  Determines whether we have the specified spell.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTCC586DC0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The id.
  - **Returns:** See Also

- **`HasSpell Method (String)`**
  ```csharp
  public static bool HasSpell(
string name
)
  ```
  Determines whether we have the specified spell.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LST73CF1DFD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name.
  - **Returns:** See Also

- **`HasSpell Method (WoWSpell)`**
  ```csharp
  public static bool HasSpell(
WoWSpell spell
)
  ```
  Determines whether we have the specified spell.
  - *spell*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTAB00221B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpellThe WoWSpell object.
  - **Returns:** Exceptions

- **`HasSpell Method (Int32, WoWUnit)`**
  ```csharp
  public static bool HasSpell(
int id,
WoWUnit unit
)
  ```
  Determines whether the specified unit has the specified spell.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST50C8B915_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The ID of the spell.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST50C8B915_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe unit for which the check is performed.
            Must either be the player or the player's pet.
  - **Returns:** See Also

- **`StopCasting Method`**
  ```csharp
  public static void StopCasting()
  ```
  Stops you from casting.

---

### StatusTextChangedEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.CommonBot.StatusTextChangedEventArgs

```csharp
public class StatusTextChangedEventArgs : EventArgs
```

Additional information for status text changed events.

#### StatusTextChangedEventArgs Properties

- **`NewStatus Property`**
  ```csharp
  public string NewStatus { get; set; }
  ```
  Gets or sets the new status.

- **`OldStatus Property`**
  ```csharp
  public string OldStatus { get; set; }
  ```
  Gets or sets the old status.

---

### TargetListUpdateFinishedDelegate Delegate

```csharp
public delegate void TargetListUpdateFinishedDelegate(
Object context
)
```

Target list update finished delegate.

---

### Targeting Class

**Inheritance:** System.Object → Styx.CommonBot.Targeting → Styx.CommonBot.HealTargeting → Styx.CommonBot.LootTargeting

```csharp
public class Targeting
```

The targeting manager class.

#### Targeting Properties

- **`AllowEvents Property`**
  ```csharp
  protected bool AllowEvents { get; set; }
  ```

- **`CollectionRange Property`**
  ```csharp
  public static double CollectionRange { get; }
  ```
  The distance from ourselves, where targets will be collected. [Default: 100].

- **`DisplayTargetingExceptions Property`**
  ```csharp
  public bool DisplayTargetingExceptions { get; set; }
  ```
  Should we print out targeting exceptions? [Default: true].

- **`EnableLogging Property`**
  ```csharp
  public static bool EnableLogging { get; set; }
  ```

- **`FirstUnit Property`**
  ```csharp
  public WoWUnit FirstUnit { get; }
  ```
  Gets the first unit.

- **`IncludeElites Property`**
  ```csharp
  public bool IncludeElites { get; set; }
  ```
  Include Elites? [Default: false].

- **`IncludeWorldPlayers Property`**
  ```csharp
  public bool IncludeWorldPlayers { get; set; }
  ```
  Include players in non-battlegrounds/arenas? [Default: false].

- **`Instance Property`**
  ```csharp
  public static Targeting Instance { get; set; }
  ```
  Gets or sets the instance.

- **`KillBetweenHotspots Property`**
  ```csharp
  public bool KillBetweenHotspots { get; }
  ```
  Gets a value indicating whether the kill between hotspots.

- **`MaxTargets Property`**
  ```csharp
  public int MaxTargets { get; set; }
  ```
  The 'limit' on the number of possible targets at any one time. [Default: 5].

- **`ObjectList Property`**
  ```csharp
  protected List&lt;WoWObject&gt; ObjectList { get; }
  ```

- **`PullDistance Property`**
  ```csharp
  public static double PullDistance { get; }
  ```
  Pulldistance used by hb for pulling evil npc's.

- **`PullDistanceSqr Property`**
  ```csharp
  public static double PullDistanceSqr { get; }
  ```
  Gets the pull distance sqr.

- **`TargetList Property`**
  ```csharp
  public List&lt;WoWUnit&gt; TargetList { get; }
  ```
  The current list of viable targets.

#### Targeting Methods

- **`Clear Method`**
  ```csharp
  public void Clear()
  ```
  Clears the list of targets.

- **`DefaultIncludeTargetsFilter Method`**
  ```csharp
  protected virtual void DefaultIncludeTargetsFilter(
List&lt;WoWObject&gt; incomingUnits,
HashSet&lt;WoWObject&gt; outgoingUnits
)
  ```
  - *incomingUnits*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST639A723E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST639A723E_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST639A723E_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - *outgoingUnits*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST639A723E_4?cs=.|vb=.|cpp=::|nu=.|fs=.");HashSetAddLanguageSpecificTextSet("LST639A723E_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LST639A723E_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`DefaultRemoveTargetsFilter Method`**
  ```csharp
  protected virtual void DefaultRemoveTargetsFilter(
List&lt;WoWObject&gt; units
)
  ```
  - *units*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTAEA2CD83_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LSTAEA2CD83_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWObjectAddLanguageSpecificTextSet("LSTAEA2CD83_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`DefaultTargetWeight Method`**
  ```csharp
  protected virtual void DefaultTargetWeight(
List&lt;Targeting.TargetPriority&gt; units
)
  ```
  - *units*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST5A364BB6_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LST5A364BB6_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TargetingAddLanguageSpecificTextSet("LST5A364BB6_4?cs=.|vb=.|cpp=::|nu=.|fs=.");TargetPriorityAddLanguageSpecificTextSet("LST5A364BB6_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");

- **`GetAggroOnMeWithin Method`**
  ```csharp
  public static int GetAggroOnMeWithin(
Vector3 position,
float range
)
  ```
  Gets aggro on me within.
  - *position*: Type: System.NumericsAddLanguageSpecificTextSet("LST7C483C67_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The position.
  - *range*: Type: SystemAddLanguageSpecificTextSet("LST7C483C67_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single   The range.
  - **Returns:** ReferenceTargeting Class

- **`GetAggroWithin Method`**
  ```csharp
  public static int GetAggroWithin(
Vector3 position,
float range
)
  ```
  Gets aggro within.
  - *position*: Type: System.NumericsAddLanguageSpecificTextSet("LST62F79656_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The position.
  - *range*: Type: SystemAddLanguageSpecificTextSet("LST62F79656_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single   The range.
  - **Returns:** ReferenceTargeting Class

- **`GetInitialObjectList Method`**
  ```csharp
  protected virtual List&lt;WoWObject&gt; GetInitialObjectList()
  ```
  - **Returns:** See Also

- **`IsTooNearBlackspot Method`**
  ```csharp
  public static bool IsTooNearBlackspot(
IEnumerable&lt;Blackspot&gt; blackspots,
Vector3 point
)
  ```
  Query if 'blackspots' is too near blackspot.
  - *blackspots*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTE9BB2AC8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTE9BB2AC8_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BlackspotAddLanguageSpecificTextSet("LSTE9BB2AC8_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The blackspots.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LSTE9BB2AC8_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3     The point.
  - **Returns:** ReferenceTargeting Class

- **`Log Method`**

- **`Log Method (String)`**
  ```csharp
  protected static void Log(
string message
)
  ```
  - *message*: Type: SystemAddLanguageSpecificTextSet("LST910A04F1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`Log Method (String, Object[])`**
  ```csharp
  protected static void Log(
string format,
params Object[] args
)
  ```
  - *format*: Type: SystemAddLanguageSpecificTextSet("LST9AA6F6F1_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *args*: Type: AddLanguageSpecificTextSet("LST9AA6F6F1_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST9AA6F6F1_5?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LST9AA6F6F1_6?cpp=&gt;|vb=()|nu=[]");

- **`LogRemoval Method`**
  ```csharp
  protected static void LogRemoval(
WoWObject obj,
string reason
)
  ```
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST24CF7E6B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObject
  - *reason*: Type: SystemAddLanguageSpecificTextSet("LST24CF7E6B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String

- **`Pulse Method`**
  ```csharp
  public virtual void Pulse()
  ```
  Pulses this object.

#### Targeting Events

- **`IncludeTargetsFilter Event`**
  ```csharp
  public event IncludeTargetsFilterDelegate IncludeTargetsFilter
  ```
  Occurrs when a request is made to include targets in the targeting list.

- **`OnTargetListUpdateFinished Event`**
  ```csharp
  public event TargetListUpdateFinishedDelegate OnTargetListUpdateFinished
  ```
  Occurs when the target list has been fully updated.

- **`RemoveTargetsFilter Event`**
  ```csharp
  public event RemoveTargetsFilterDelegate RemoveTargetsFilter
  ```
  Occurs when a request is made to remove targets in the targeting list.

- **`WeighTargetsFilter Event`**
  ```csharp
  public event WeighTargetsDelegate WeighTargetsFilter
  ```
  Occurs when a request is made to weigh the targets in the targeting list.

#### Targeting Fields

- **`_targetingLog Field`**
  ```csharp
  protected static StyxLog _targetingLog
  ```

---

### Targeting.TargetPriority Class

**Inheritance:** System.Object → Styx.CommonBot.Targeting.TargetPriority

```csharp
public sealed class TargetPriority
```

A target priority.

#### TargetPriority Fields

- **`Object Field`**
  ```csharp
  public WoWObject Object
  ```
  The object.

- **`Score Field`**
  ```csharp
  public double Score
  ```
  The score.

---

### TreeRoot Class

**Inheritance:** System.Object → Styx.CommonBot.TreeRoot

```csharp
public static class TreeRoot
```

Class that will tick/initialize/start/dispose the behavior tree.

#### TreeRoot Properties

- **`Current Property`**
  ```csharp
  public static BotBase Current { get; }
  ```
  Gets the current BotBase.

- **`CurrentThreadIsBotThread Property`**
  ```csharp
  public static bool CurrentThreadIsBotThread { get; }
  ```
  Gets a bool that indicates whether the current thread is the bot thread.

- **`GoalText Property`**
  ```csharp
  public static string GoalText { get; set; }
  ```
  Gets or sets the goal text.

- **`IsPaused Property`**
  ```csharp
  public static bool IsPaused { get; }
  ```
  Gets or sets a value indicating whether the TreeRoot is paused.

- **`IsRunning Property`**
  ```csharp
  public static bool IsRunning { get; }
  ```
  Gets or sets a value indicating whether the TreeRoot is running.

- **`State Property`**
  ```csharp
  public static TreeRootState State { get; }
  ```

- **`StatusText Property`**
  ```csharp
  public static string StatusText { get; set; }
  ```
  Provides the status bar text for the main form and useful loggin info if needed.

- **`TicksPerSecond Property`**
  ```csharp
  public static byte TicksPerSecond { get; set; }
  ```
  Gets or sets the frequency of how often the bot is "Ticked". Default: 15. Nulling this value
            will make it fall back to TicksPerSecond user value.

#### TreeRoot Methods

- **`Pause Method`**
  ```csharp
  public static void Pause()
  ```
  Pauses the currently running bot.

- **`ResetTicksPerSecond Method`**
  ```csharp
  public static void ResetTicksPerSecond()
  ```
  Resets the ticks per second.

- **`Resume Method`**
  ```csharp
  public static void Resume()
  ```
  Resumes the currently paused bot.

- **`Shutdown Method`**
  ```csharp
  public static void Shutdown(
HonorbuddyExitCode exitCode = HonorbuddyExitCode.Default,
bool closeWow = false
)
  ```
  Shuts down Honorbuddy.

- **`Start Method`**
  ```csharp
  public static void Start()
  ```
  Initializes and starts the current bot.

- **`Stop Method`**
  ```csharp
  public static void Stop(
string reason = null
)
  ```
  Attempts to stop the current running bot.

- **`WaitForStop Method`**
  ```csharp
  public static bool WaitForStop(
int timeout = -1
)
  ```
  Synchronously blocks the current thread for the specified timeout or until the bot has stopped.
  - **Returns:** ExceptionConditionInvalidOperationExceptionThrown if called from within the bot thread.

#### TreeRoot Events

- **`OnGoalTextChanged Event`**
  ```csharp
  public static event EventHandler&lt;GoalTextChangedEventArgs&gt; OnGoalTextChanged
  ```
  Occurs when [goal text changed].

- **`OnShutdownRequested Event`**
  ```csharp
  public static event EventHandler&lt;ShutdownRequestedEventArgs&gt; OnShutdownRequested
  ```

- **`OnStatusTextChanged Event`**
  ```csharp
  public static event EventHandler&lt;StatusTextChangedEventArgs&gt; OnStatusTextChanged
  ```
  Occurs when [the status text changes].

---

### TreeRootState Enumeration

```csharp
public enum TreeRootState
```

---

### VendorItemsEventHandler Delegate

```csharp
public delegate void VendorItemsEventHandler(
SellItemsEventArgs e
)
```

Delegate for handling VendorItems events.

---

### Vendors Class

**Inheritance:** System.Object → Styx.CommonBot.Vendors

```csharp
public static class Vendors
```

A vendors.

#### Vendors Properties

- **`ForceBuy Property`**
  ```csharp
  public static bool ForceBuy { get; set; }
  ```
  Gets or sets a value indicating whether the buy should be forced.

- **`ForceMail Property`**
  ```csharp
  public static bool ForceMail { get; set; }
  ```
  Gets or sets a value indicating whether the mail should be forced.

- **`ForceRepair Property`**
  ```csharp
  public static bool ForceRepair { get; set; }
  ```
  Gets or sets a value indicating whether the repair should be forced.

- **`ForceSell Property`**
  ```csharp
  public static bool ForceSell { get; set; }
  ```
  Gets or sets a value indicating whether the sell should be forced.

- **`ForceTrainer Property`**
  ```csharp
  public static bool ForceTrainer { get; set; }
  ```
  Gets or sets a value indicating whether the trainer should be forced.

- **`NavType Property`**
  ```csharp
  public static Nullable&lt;NavType&gt; NavType { get; set; }
  ```
  Gets or sets the type of the navigaiton.

- **`RepairDisabled Property`**
  ```csharp
  public static bool RepairDisabled { get; set; }
  ```
  Gets or sets a value indicating whether the repair is disabled.

#### Vendors Methods

- **`BuyItems Method`**
  ```csharp
  public static void BuyItems()
  ```
  Handles buying items.

- **`MailAllItemsCoroutine Method`**
  ```csharp
  public static Task MailAllItemsCoroutine()
  ```
  - **Returns:** ReferenceVendors Class

- **`RepairAllItems Method`**
  ```csharp
  public static void RepairAllItems()
  ```
  Handles repairing items.

- **`SellAllItems Method`**
  ```csharp
  public static void SellAllItems()
  ```
  Handles selling items.

- **`SellAllItemsCoroutine Method`**
  ```csharp
  public static Task SellAllItemsCoroutine()
  ```
  - **Returns:** ReferenceVendors Class

- **`SellItems Method`**
  ```csharp
  public static Task SellItems(
IEnumerable&lt;WoWItem&gt; itemsToSell
)
  ```
  - *itemsToSell*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTD35F9857_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTD35F9857_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWItemAddLanguageSpecificTextSet("LSTD35F9857_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - **Returns:** ReferenceVendors Class

- **`TrainSkills Method`**
  ```csharp
  public static void TrainSkills()
  ```
  Train skills.

#### Vendors Fields

- **`OnBuyItems Field`**
  ```csharp
  public static BuyItemsEventHandler OnBuyItems
  ```
  The on buy items.

- **`OnMailItems Field`**
  ```csharp
  public static MailItemsEventHandler OnMailItems
  ```
  The on mail items.

- **`OnRepairItems Field`**
  ```csharp
  public static EventHandler OnRepairItems
  ```
  The on repair items.

- **`OnVendorItems Field`**
  ```csharp
  public static VendorItemsEventHandler OnVendorItems
  ```
  The on vendor items.

---

### WeighTargetsDelegate Delegate

```csharp
public delegate void WeighTargetsDelegate(
List&lt;Targeting.TargetPriority&gt; units
)
```

Allows a registered callee to set the weights on the specified units.

---
