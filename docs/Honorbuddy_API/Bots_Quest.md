# Bots.Quest

Contains the quest bot.

## Contents

- [QuestBot Class](#questbot-class)
- [QuestDebug Class](#questdebug-class)
- [QuestManager Class](#questmanager-class)
- [QuestState Class](#queststate-class)

---

### QuestBot Class

**Inheritance:** System.Object → Styx.CommonBot.BotBase → Bots.Quest.QuestBot

```csharp
public class QuestBot : BotBase
```

The quest bot.

#### QuestBot Properties

- **`ConfigurationForm Property`**
  ```csharp
  public override Form ConfigurationForm { get; }
  ```
  Gets the configuration form.

- **`Name Property`**
  ```csharp
  public override string Name { get; }
  ```
  Gets the name.

- **`ProfileCompleted Property`**
  ```csharp
  public bool ProfileCompleted { get; }
  ```

- **`PulseFlags Property`**
  ```csharp
  public override PulseFlags PulseFlags { get; }
  ```
  Gets the pulse flags.

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

#### QuestBot Methods

- **`CreateRoot Method`**
  ```csharp
  public static Composite CreateRoot()
  ```
  Creates the root.
  - **Returns:** ReferenceQuestBot Class

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

- **`Stop Method`**
  ```csharp
  public override void Stop()
  ```
  Stops this object.

---

### QuestDebug Class

**Inheritance:** System.Object → Bots.Quest.QuestDebug

```csharp
public static class QuestDebug
```

A question debug.

#### QuestDebug Methods

- **`GetQuestDebugString Method`**
  ```csharp
  public static string GetQuestDebugString(
PlayerQuest quest
)
  ```
  Gets question debug string.
  - *quest*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST949A8A8F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");PlayerQuestThe question.
  - **Returns:** ReferenceQuestDebug Class

---

### QuestManager Class

**Inheritance:** System.Object → Bots.Quest.QuestManager

```csharp
public static class QuestManager
```

Manager for questions.

#### QuestManager Fields

- **`GossipFrame Field`**
  ```csharp
  public static readonly GossipFrame GossipFrame
  ```
  The gossip frame.

- **`QuestFrame Field`**
  ```csharp
  public static readonly QuestFrame QuestFrame
  ```
  The question frame.

---

### QuestState Class

**Inheritance:** System.Object → Bots.Quest.QuestState

```csharp
public class QuestState
```

A question state.

#### QuestState Properties

- **`CurrentGrindArea Property`**
  ```csharp
  public GrindArea CurrentGrindArea { get; set; }
  ```
  Gets or sets the current grind area.

- **`Order Property`**
  ```csharp
  public QuestOrder Order { get; }
  ```
  Gets the order.

#### QuestState Fields

- **`Instance Field`**
  ```csharp
  public static readonly QuestState Instance
  ```
  The instance.

---
