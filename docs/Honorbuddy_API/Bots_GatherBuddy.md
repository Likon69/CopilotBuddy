# Bots.Gatherbuddy

Contains GatherBuddy classes.

## Contents

- [GatherbuddyBot Class](#gatherbuddybot-class)
- [GatherbuddySettings Class](#gatherbuddysettings-class)
- [PathType Enumeration](#pathtype-enumeration)
- [Profile Class](#profile-class)

---

### GatherbuddyBot Class

**Inheritance:** System.Object → Styx.CommonBot.BotBase → Bots.Gatherbuddy.GatherbuddyBot

```csharp
public class GatherbuddyBot : BotBase
```

The Gatherbuddy bot.

#### GatherbuddyBot Properties

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

- **`PulseFlags Property`**
  ```csharp
  public override PulseFlags PulseFlags { get; }
  ```
  Gets the pulse flags.

- **`RequiresProfile Property`**
  ```csharp
  public override bool RequiresProfile { get; }
  ```

- **`Root Property`**
  ```csharp
  public override Composite Root { get; }
  ```
  Gets the root.

- **`runningTime Property`**
  ```csharp
  public static TimeSpan runningTime { get; }
  ```
  Gets the running time.

#### GatherbuddyBot Methods

- **`Initialize Method`**
  ```csharp
  public override void Initialize()
  ```

- **`Pulse Method`**
  ```csharp
  public override void Pulse()
  ```
  Pulses this object.

- **`Start Method`**
  ```csharp
  public override void Start()
  ```
  Starts this object.

- **`StatusReport Method`**
  ```csharp
  public static void StatusReport()
  ```
  Status report.

- **`Stop Method`**
  ```csharp
  public override void Stop()
  ```
  Stops this object.

#### GatherbuddyBot Fields

- **`BlacklistNodes Field`**
  ```csharp
  public static Dictionary&lt;Vector3, string&gt; BlacklistNodes
  ```
  The blacklist nodes.

- **`NodeCollectionCount Field`**
  ```csharp
  public static readonly Dictionary&lt;string, int&gt; NodeCollectionCount
  ```
  Number of node collections.

- **`Version Field`**
  ```csharp
  public static Version Version
  ```
  The version.

---

### GatherbuddySettings Class

**Inheritance:** System.Object → Styx.Helpers.Settings → Bots.Gatherbuddy.GatherbuddySettings

```csharp
public class GatherbuddySettings : Settings
```

A gatherbuddy settings.

#### GatherbuddySettings Properties

- **`BlacklistTimer Property`**
  ```csharp
  public int BlacklistTimer { get; set; }
  ```
  Gets or sets the blacklist timer, in seconds.

- **`BottingHours Property`**
  ```csharp
  public float BottingHours { get; set; }
  ```
  Gets or sets the botting hours.

- **`DiagnosticLogging Property`**
  ```csharp
  public bool DiagnosticLogging { get; set; }
  ```
  Gets or sets a value indicating whether diagnostic logging is on or not.

- **`FaceNodes Property`**
  ```csharp
  public bool FaceNodes { get; set; }
  ```
  Gets or sets a value indicating whether nodes should be faces when being gathered.

- **`GatherHerbs Property`**
  ```csharp
  public bool GatherHerbs { get; set; }
  ```
  Gets or sets a value indicating whether the bot should gather herbs.

- **`GatherMinerals Property`**
  ```csharp
  public bool GatherMinerals { get; set; }
  ```
  Gets or sets a value indicating whether the bot should gather minerals.

- **`HearthAndExit Property`**
  ```csharp
  public bool HearthAndExit { get; set; }
  ```
  Gets or sets a value indicating whether the bot should hearth and exit on full inventory.

- **`HeightModifier Property`**
  ```csharp
  public float HeightModifier { get; set; }
  ```
  Gets or sets the height modifier.

- **`IgnoreElites Property`**
  ```csharp
  public bool IgnoreElites { get; set; }
  ```
  Gets or sets a value indicating whether the bot should try to ignore elites.

- **`Instance Property`**
  ```csharp
  public static GatherbuddySettings Instance { get; }
  ```
  Gets the instance.

- **`LootMobs Property`**
  ```csharp
  public bool LootMobs { get; set; }
  ```
  Gets or sets a value indicating whether the bot should loot mobs.

- **`MailToAlt Property`**
  ```csharp
  public bool MailToAlt { get; set; }
  ```
  Gets or sets a value indicating whether the bot should mail items to an alt on full inventory.

- **`NoNinja Property`**
  ```csharp
  public bool NoNinja { get; set; }
  ```
  Gets or sets a value indicating whether the bot should not ninja nodes.

- **`PathingType Property`**
  ```csharp
  public PathType PathingType { get; set; }
  ```
  Gets or sets the type of the pathing.

- **`RandomizationFactor Property`**
  ```csharp
  public double RandomizationFactor { get; set; }
  ```
  Determines how heavily hotspots are randomized. Defaults to 1 which adjusts hotspots by a distance from 5 to 25 yards.

- **`RandomizeHotspots Property`**
  ```csharp
  public bool RandomizeHotspots { get; set; }
  ```
  Gets or sets if hotspots should be randomized or not. 
            		  When this is enabled hotspots are moved in a random direction by a random distance 
            		  where the random distance is calculated by: rand(0, 1) * 20 * RandomizationFactor + 5 * RandomizationFactor.
            		  The height is only randomized from -5 to +5 yards.

- **`RepairFromGuildBank Property`**
  ```csharp
  public bool RepairFromGuildBank { get; set; }
  ```
  Gets or sets a value indicating whether the bot should repair from guild bank.

- **`UseGuildVault Property`**
  ```csharp
  public bool UseGuildVault { get; set; }
  ```
  Gets or sets a value indicating whether the bot should use guild vault.

- **`UseSpiritHealer Property`**
  ```csharp
  public bool UseSpiritHealer { get; set; }
  ```
  Gets or sets a value indicating whether the bot should use spirit healer.

- **`VendorIntervalHours Property`**
  ```csharp
  public Nullable&lt;double&gt; VendorIntervalHours { get; set; }
  ```

- **`WaitRezSickness Property`**
  ```csharp
  public bool WaitRezSickness { get; set; }
  ```
  Gets or sets a value indicating whether the bot should wait for rez sickness when dead.

#### GatherbuddySettings Methods

- **`Finalize Method`**
  ```csharp
  protected override void Finalize()
  ```
  Finaliser.

---

### PathType Enumeration

```csharp
public enum PathType
```

Values that represent PathType.

---

### Profile Class

**Inheritance:** System.Object → Bots.Gatherbuddy.Profile

```csharp
public class Profile
```

A profile.

#### Profile Properties

- **`Blackspots Property`**
  ```csharp
  public List&lt;Blackspot&gt; Blackspots { get; }
  ```
  Gets the blackspots.

- **`Current Property`**
  ```csharp
  [ObsoleteAttribute("Use GetCurrent() instead")]
public Vector3 Current { get; }
  ```

- **`CurrentProfile Property`**
  ```csharp
  public static Profile CurrentProfile { get; set; }
  ```
  Gets or sets the current profile.

- **`Path Property`**
  ```csharp
  public CircularQueue&lt;Vector3&gt; Path { get; }
  ```
  Gets the full pathname of the file.

- **`Waypoints Property`**
  ```csharp
  public List&lt;Vector3&gt; Waypoints { get; }
  ```
  Gets the waypoints.

#### Profile Methods

- **`DequeueHotspot Method`**
  ```csharp
  public void DequeueHotspot()
  ```
  Dequeue hotspot.

- **`GetCurrent Method`**
  ```csharp
  public Task&lt;Vector3&gt; GetCurrent()
  ```
  Gets the current.

---
