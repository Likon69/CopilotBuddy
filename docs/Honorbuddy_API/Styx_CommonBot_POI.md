# Styx.CommonBot.POI

Contains POI related classes.

## Contents

- [BotPoi Class](#botpoi-class)
- [PoiType Enumeration](#poitype-enumeration)
- [PoiTypeExtensions Class](#poitypeextensions-class)

---

### BotPoi Class

**Inheritance:** System.Object → Styx.CommonBot.POI.BotPoi

```csharp
public class BotPoi
```

A bottom poi.

#### BotPoi Constructor

- **`BotPoi Constructor (Mailbox)`**
  ```csharp
  public BotPoi(
Mailbox mailbox
)
  ```
  Constructs a new bot POI from a mailbox, and sets the PoiType to Mail.
  - *mailbox*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST526EAA7E_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Mailbox

- **`BotPoi Constructor (PickUpNode)`**
  ```csharp
  public BotPoi(
PickUpNode pickUp
)
  ```
  Constructor.
  - *pickUp*: Type: Styx.CommonBot.Profiles.Quest.OrderAddLanguageSpecificTextSet("LSTEE6BBD92_0?cs=.|vb=.|cpp=::|nu=.|fs=.");PickUpNodeThe pick up.

- **`BotPoi Constructor (TurnInNode)`**
  ```csharp
  public BotPoi(
TurnInNode turnIn
)
  ```
  Constructor.
  - *turnIn*: Type: Styx.CommonBot.Profiles.Quest.OrderAddLanguageSpecificTextSet("LST4F13BFE2_0?cs=.|vb=.|cpp=::|nu=.|fs=.");TurnInNodeThe turn in.

- **`BotPoi Constructor (Mailbox, Nullable(NavType))`**
  ```csharp
  public BotPoi(
Mailbox mailbox,
Nullable&lt;NavType&gt; navType = null
)
  ```
  Constructor.
  - *mailbox*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST7E04DDA5_2?cs=.|vb=.|cpp=::|nu=.|fs=.");MailboxThe mailbox.

- **`BotPoi Constructor (PoiType, Nullable(NavType))`**
  ```csharp
  public BotPoi(
PoiType type,
Nullable&lt;NavType&gt; navType = null
)
  ```
  Constructor.
  - *type*: Type: Styx.CommonBot.POIAddLanguageSpecificTextSet("LST9DCA4C9_2?cs=.|vb=.|cpp=::|nu=.|fs=.");PoiType   The type.

- **`BotPoi Constructor (Quest, Nullable(NavType))`**
  ```csharp
  public BotPoi(
Quest quest,
Nullable&lt;NavType&gt; navType = null
)
  ```
  Constructor.
  - *quest*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST510084E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Quest  The question.

- **`BotPoi Constructor (Vendor, PoiType)`**
  ```csharp
  public BotPoi(
Vendor vendor,
PoiType type
)
  ```
  Constructor.
  - *vendor*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST6D2CDF62_0?cs=.|vb=.|cpp=::|nu=.|fs=.");VendorThe vendor.
  - *type*: Type: Styx.CommonBot.POIAddLanguageSpecificTextSet("LST6D2CDF62_1?cs=.|vb=.|cpp=::|nu=.|fs=.");PoiType  The type.

- **`BotPoi Constructor (Vector3, PoiType, Nullable(NavType))`**
  ```csharp
  public BotPoi(
Vector3 location,
PoiType type,
Nullable&lt;NavType&gt; navType = null
)
  ```
  Constructor.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LSTFEEE8DE0_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *type*: Type: Styx.CommonBot.POIAddLanguageSpecificTextSet("LSTFEEE8DE0_3?cs=.|vb=.|cpp=::|nu=.|fs=.");PoiType    The type.

- **`BotPoi Constructor (WoWObject, PoiType, Nullable(NavType))`**
  ```csharp
  public BotPoi(
WoWObject obj,
PoiType type,
Nullable&lt;NavType&gt; navType = null
)
  ```
  Constructor.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST446B55E1_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObject    The object.
  - *type*: Type: Styx.CommonBot.POIAddLanguageSpecificTextSet("LST446B55E1_3?cs=.|vb=.|cpp=::|nu=.|fs=.");PoiType   The type.

#### BotPoi Properties

- **`AsMailbox Property`**
  ```csharp
  public Mailbox AsMailbox { get; }
  ```
  Gets as mailbox.

- **`AsObject Property`**
  ```csharp
  public WoWObject AsObject { get; }
  ```
  Gets as object.

- **`AsQuest Property`**
  ```csharp
  public Quest AsQuest { get; }
  ```
  Gets as question.

- **`AsVendor Property`**
  ```csharp
  public Vendor AsVendor { get; }
  ```
  Gets as vendor.

- **`Current Property`**
  ```csharp
  public static BotPoi Current { get; set; }
  ```
  The current POI that the bot has decided on. This is guaranteed to be nonnull.

- **`Entry Property`**
  ```csharp
  public uint Entry { get; set; }
  ```
  Invalid for hotspots.

- **`Guid Property`**
  ```csharp
  public WoWGuid Guid { get; set; }
  ```
  Valid only for types of WoWObject.

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; set; }
  ```
  Valid for most.

- **`Name Property`**
  ```csharp
  public string Name { get; set; }
  ```
  Invalid for hotspots.

- **`NavType Property`**
  ```csharp
  public NavType NavType { get; set; }
  ```
  Gets or sets the type of the navigation.

- **`Type Property`**
  ```csharp
  public PoiType Type { get; set; }
  ```
  Valid for all POI types.

#### BotPoi Methods

- **`Clear Method`**
  Clears the current POI, and sets its type to None.

- **`Clear Method`**
  ```csharp
  public static void Clear()
  ```
  Clears the current POI, and sets its type to None.

- **`Clear Method (String)`**
  ```csharp
  public static void Clear(
string reason
)
  ```
  Clears the current POI, and sets its type to None.
  - *reason*: Type: SystemAddLanguageSpecificTextSet("LST2A96D500_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe reason.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceBotPoi Class

---

### PoiType Enumeration

```csharp
public enum PoiType
```

Values that represent PoiType.

---

### PoiTypeExtensions Class

**Inheritance:** System.Object → Styx.CommonBot.POI.PoiTypeExtensions

```csharp
public static class PoiTypeExtensions
```

A poi type extensions.

#### PoiTypeExtensions Methods

- **`GetBlacklistFlags Method`**
  ```csharp
  public static BlacklistFlags GetBlacklistFlags(
this PoiType type
)
  ```
  A PoiType extension method that gets related BlacklistFlags.
  - *type*: Type: Styx.CommonBot.POIAddLanguageSpecificTextSet("LST90052F5C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");PoiTypeThe type to act on.
  - **Returns:** ReferencePoiTypeExtensions Class

- **`GetGossipType Method`**
  ```csharp
  public static GossipEntry.GossipEntryType GetGossipType(
this PoiType type
)
  ```
  A PoiType extension method that gets gossip type.
  - *type*: Type: Styx.CommonBot.POIAddLanguageSpecificTextSet("LSTB2FDDBD_2?cs=.|vb=.|cpp=::|nu=.|fs=.");PoiTypeThe type to act on.
  - **Returns:** See Also

- **`IsOneOf Method`**
  ```csharp
  public static bool IsOneOf(
this PoiType type,
params PoiType[] types
)
  ```
  Determines if the POI type identified by type is any of the ones passed in types.
  - *type*: Type: Styx.CommonBot.POIAddLanguageSpecificTextSet("LSTF6D9C453_1?cs=.|vb=.|cpp=::|nu=.|fs=.");PoiType
  - *types*: Type: AddLanguageSpecificTextSet("LSTF6D9C453_2?cpp=array&lt;");Styx.CommonBot.POIAddLanguageSpecificTextSet("LSTF6D9C453_3?cs=.|vb=.|cpp=::|nu=.|fs=.");PoiTypeAddLanguageSpecificTextSet("LSTF6D9C453_4?cpp=&gt;|vb=()|nu=[]");
  - **Returns:** ReferencePoiTypeExtensions Class

---
