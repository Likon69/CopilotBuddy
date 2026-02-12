# Styx.Helpers

Contains commonly used helpers.

## Contents

- [ActivitySetter Class](#activitysetter-class)
- [BGBotSettings Class](#bgbotsettings-class)
- [CachedValue(T) Class](#cachedvaluet-class)
- [CharacterSettings Class](#charactersettings-class)
- [CombatAssistSettings Class](#combatassistsettings-class)
- [DefaultValueAttribute Class](#defaultvalueattribute-class)
- [DictionaryExtensions Class](#dictionaryextensions-class)
- [EncryptedAttribute Class](#encryptedattribute-class)
- [Extensions Class](#extensions-class)
- [FlagCheckedListBox Class](#flagcheckedlistbox-class)
- [FlagCheckedListBoxItem Class](#flagcheckedlistboxitem-class)
- [FlagEnumUIEditor Class](#flagenumuieditor-class)
- [GameDebugAddStringDelegate Delegate](#gamedebugaddstringdelegate-delegate)
- [GlobalSettings Class](#globalsettings-class)
- [KeyHelpers Class](#keyhelpers-class)
- [KeyboardManager Class](#keyboardmanager-class)
- [KeyboardManager.eActionBar Enumeration](#keyboardmanager.eactionbar-enumeration)
- [KeyboardManager.eActionButton Enumeration](#keyboardmanager.eactionbutton-enumeration)
- [KeyboardManager.eVirtualKeyMessages Enumeration](#keyboardmanager.evirtualkeymessages-enumeration)
- [LevelbotSettings Class](#levelbotsettings-class)
- [PVPSettings Class](#pvpsettings-class)
- [PerFrameCachedValue(T) Class](#perframecachedvaluet-class)
- [SettingAttribute Class](#settingattribute-class)
- [Settings Class](#settings-class)
- [TimeCachedValue(T) Class](#timecachedvaluet-class)
- [UISettings Class](#uisettings-class)
- [WoWItemQualityExtensions Class](#wowitemqualityextensions-class)
- [WoWMathHelper Class](#wowmathhelper-class)
- [WoWSpecExtensions Class](#wowspecextensions-class)
- [XmlExtensions Class](#xmlextensions-class)
- [XmlUtils Class](#xmlutils-class)

---

### ActivitySetter Class

**Inheritance:** System.Object → Styx.Helpers.ActivitySetter

```csharp
public class ActivitySetter : IDisposable
```

An activity setter.

#### ActivitySetter Methods

- **`Dispose Method`**
  ```csharp
  public void Dispose()
  ```
  Performs application-defined tasks associated with freeing, releasing, or resetting
            unmanaged resources.

---

### BGBotSettings Class

**Inheritance:** System.Object → Styx.Helpers.Settings → Styx.Helpers.BGBotSettings

```csharp
public class BGBotSettings : Settings
```

A background bottom settings.

#### BGBotSettings Properties

- **`BG1 Property`**
  ```csharp
  public BattlegroundType BG1 { get; set; }
  ```
  Gets or sets the background 1.

- **`BG2 Property`**
  ```csharp
  public BattlegroundType BG2 { get; set; }
  ```
  Gets or sets the background 2.

- **`IncludeMountedPlayers Property`**
  ```csharp
  public bool IncludeMountedPlayers { get; set; }
  ```
  Gets or sets a value indicating whether the mounted players should be included.

- **`IsHealer Property`**
  ```csharp
  public bool IsHealer { get; set; }
  ```
  Gets or sets a value indicating whether this object is healer.

#### BGBotSettings Fields

- **`Instance Field`**
  ```csharp
  public static readonly BGBotSettings Instance
  ```
  The instance.

---

### CachedValue(T) Class

**Inheritance:** System.Object → Styx.Helpers.CachedValue.T

```csharp
public class CachedValue&lt;T&gt;
```

Represents a value that is cached.

#### CachedValue(T) Properties

- **`Value Property`**
  ```csharp
  public T Value { get; }
  ```
  Gets the value.

#### CachedValue(T) Methods

- **`Reset Method`**
  ```csharp
  public void Reset()
  ```
  Resets the state of this CachedValue.T, causing the value to be recreated the next time it is accessed.

#### CachedValue(T) Type Conversions

- **`Implicit Conversion (CachedValue(T) to T)`**
  ```csharp
  public static implicit operator T (
CachedValue&lt;T&gt; pfcv
)
  ```
  - *pfcv*: Type: Styx.HelpersAddLanguageSpecificTextSet("LST69C6FE66_5?cs=.|vb=.|cpp=::|nu=.|fs=.");CachedValueAddLanguageSpecificTextSet("LST69C6FE66_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST69C6FE66_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - **Returns:** See Also

---

### CharacterSettings Class

**Inheritance:** System.Object → Styx.Helpers.Settings → Styx.Helpers.CharacterSettings

```csharp
public class CharacterSettings : Settings
```

A character settings.

#### CharacterSettings Properties

- **`AutoEquip Property`**
  ```csharp
  public bool AutoEquip { get; set; }
  ```
  Gets or sets a bool that indicates whether the bot will auto equip items.

- **`AutoEquipArmor Property`**
  ```csharp
  public bool AutoEquipArmor { get; set; }
  ```
  Gets or sets a bool that indicates whether the bot will auto equip armor.

- **`AutoEquipBags Property`**
  ```csharp
  public bool AutoEquipBags { get; set; }
  ```
  Gets or sets a bool that indicates whether the bot will auto equip bags.

- **`AutoEquipBoEEpics Property`**
  ```csharp
  public bool AutoEquipBoEEpics { get; set; }
  ```
  Gets or sets a bool that indicates whether the bot will auto equip BoE epics.

- **`AutoEquipWeapons Property`**
  ```csharp
  public bool AutoEquipWeapons { get; set; }
  ```
  Gets or sets a bool that indicates whether the bot will auto equip weapons.

- **`ClassProfileName Property`**
  ```csharp
  public string ClassProfileName { get; }
  ```
  Gets the name of the saved class profile. To set this, use SetClassProfile(ClassProfile).

- **`ContinuallySkipToCheckpoints Property`**
  ```csharp
  [ObsoleteAttribute("Please use the 'RecheckCheckpoints' tag instead")]
public bool ContinuallySkipToCheckpoints { get; set; }
  ```

- **`DrinkAmount Property`**
  ```csharp
  public int DrinkAmount { get; set; }
  ```
  Gets or sets the drink amount.

- **`DrinkName Property`**
  ```csharp
  public string DrinkName { get; set; }
  ```
  Gets or sets the name of the drink.

- **`EnabledPlugins Property`**
  ```csharp
  public string[] EnabledPlugins { get; set; }
  ```
  Gets or sets the enabled plugins.

- **`FindMountAutomatically Property`**
  ```csharp
  [ObsoleteAttribute("No longer used. Assign 0 to GroundMountId or FlyingMountId instead. This will be removed in the future.")]
public bool FindMountAutomatically { get; set; }
  ```
  Gets or sets a value indicating whether the find mount automatically.

- **`FlyingMountName Property`**
  ```csharp
  [ObsoleteAttribute("Use FlyingMountSpellId. This will be removed in the future.")]
public string FlyingMountName { get; set; }
  ```
  Gets or sets the name of the flying mount.

- **`FlyingMountSpellId Property`**
  ```csharp
  public int FlyingMountSpellId { get; set; }
  ```
  The spell ID of the flying mount to use.
            If 0, a flying mount is automatically found.

- **`FoodAmount Property`**
  ```csharp
  public int FoodAmount { get; set; }
  ```
  Gets or sets the food amount.

- **`FoodName Property`**
  ```csharp
  public string FoodName { get; set; }
  ```
  Gets or sets the name of the food.

- **`GroundMountSpellId Property`**
  ```csharp
  public int GroundMountSpellId { get; set; }
  ```
  The spell ID of the ground mount to use.
            If 0, a ground mount is automatically found.

- **`HarvestHerbs Property`**
  ```csharp
  public bool HarvestHerbs { get; set; }
  ```
  Gets or sets a value indicating whether the harvest herbs.

- **`HarvestMinerals Property`**
  ```csharp
  public bool HarvestMinerals { get; set; }
  ```
  Gets or sets a value indicating whether the harvest minerals.

- **`IgnoreCheckpoints Property`**
  ```csharp
  public bool IgnoreCheckpoints { get; set; }
  ```

- **`Instance Property`**
  ```csharp
  public static CharacterSettings Instance { get; }
  ```
  The instance.

- **`LastUsedPath Property`**
  ```csharp
  public string LastUsedPath { get; set; }
  ```
  Gets or sets the full pathname of the last used file.

- **`LootChests Property`**
  ```csharp
  public bool LootChests { get; set; }
  ```
  Gets or sets a value indicating whether the loot chests.

- **`LootMobs Property`**
  ```csharp
  public bool LootMobs { get; set; }
  ```
  Gets or sets a value indicating whether the loot mobs.

- **`LootRadius Property`**
  ```csharp
  public int LootRadius { get; set; }
  ```
  Gets or sets the loot radius.

- **`MailBlue Property`**
  ```csharp
  public bool MailBlue { get; set; }
  ```

- **`MailGreen Property`**
  ```csharp
  public bool MailGreen { get; set; }
  ```

- **`MailGrey Property`**
  ```csharp
  public bool MailGrey { get; set; }
  ```

- **`MailPurple Property`**
  ```csharp
  public bool MailPurple { get; set; }
  ```

- **`MailRecipient Property`**
  ```csharp
  public string MailRecipient { get; set; }
  ```
  Gets or sets the mail recipient.

- **`MailWhite Property`**
  ```csharp
  public bool MailWhite { get; set; }
  ```

- **`MaxBoEDisenchantQuality Property`**
  ```csharp
  public WoWItemQuality MaxBoEDisenchantQuality { get; set; }
  ```
  Gets or sets the maximum item quality for BoE items that the bot will roll disenchant on.

- **`MountName Property`**
  ```csharp
  [ObsoleteAttribute("Use GroundMountSpellId. This will be removed in the future.")]
public string MountName { get; set; }
  ```
  Gets or sets the name of the mount.

- **`NinjaSkin Property`**
  ```csharp
  public bool NinjaSkin { get; set; }
  ```
  Gets or sets a value indicating whether the ninja skin.

- **`OverrideProfileSettings Property`**
  ```csharp
  public bool OverrideProfileSettings { get; set; }
  ```

- **`PullDistance Property`**
  ```csharp
  [ObsoleteAttribute("Use Targeting.CollectionRange or Targeting.PullDistance. This is no longer used.")]
public int PullDistance { get; set; }
  ```
  Gets or sets the pull distance.

- **`RecentProfiles Property`**
  ```csharp
  public string[] RecentProfiles { get; set; }
  ```
  Gets or sets the recent profiles.

- **`RessAtSpiritHealers Property`**
  ```csharp
  public bool RessAtSpiritHealers { get; set; }
  ```
  Gets or sets a value indicating whether the ress at spirit healers.

- **`RollDisenchantWhenGreed Property`**
  ```csharp
  public bool RollDisenchantWhenGreed { get; set; }
  ```
  Gets or sets a bool that indicates whether the bot should roll disenchant instead of greed, when possible, and according to MaxBoEDisenchantQuality.

- **`RollOnItems Property`**
  ```csharp
  public bool RollOnItems { get; set; }
  ```
  Gets or sets a bool that indicates whether the bot should roll on items by itself.

- **`SelectedBotName Property`**
  ```csharp
  public string SelectedBotName { get; set; }
  ```

- **`SelectTalents Property`**
  ```csharp
  public bool SelectTalents { get; set; }
  ```
  Gets or sets a bool that indicates whether the bot will select talents.

- **`SellBlue Property`**
  ```csharp
  public bool SellBlue { get; set; }
  ```

- **`SellGreen Property`**
  ```csharp
  public bool SellGreen { get; set; }
  ```

- **`SellGrey Property`**
  ```csharp
  public bool SellGrey { get; set; }
  ```

- **`SellPurple Property`**
  ```csharp
  public bool SellPurple { get; set; }
  ```

- **`SellWhite Property`**
  ```csharp
  public bool SellWhite { get; set; }
  ```

- **`SkinMobs Property`**
  ```csharp
  public bool SkinMobs { get; set; }
  ```
  Gets or sets a value indicating whether the skin mobs.

- **`TicksPerSecond Property`**
  ```csharp
  public byte TicksPerSecond { get; set; }
  ```
  Gets or sets the ticks per second.

- **`UseFlightPaths Property`**
  ```csharp
  [ObsoleteAttribute("No longer used")]
public bool UseFlightPaths { get; set; }
  ```
  Gets or sets a value indicating whether this object use flight paths.

- **`UseGroundMount Property`**
  ```csharp
  public bool UseGroundMount { get; set; }
  ```
  Gets or sets a value indicating whether to use ground mount.

- **`UseRandomMount Property`**
  ```csharp
  [ObsoleteAttribute("No longer used. If GroundMountSpellId or FlyingMountSpellId are 0 a mount is automatically found. This will be removed in the future.")]
public bool UseRandomMount { get; set; }
  ```
  Gets or sets a value indicating whether this object use random mount.

- **`WeaponStyle Property`**
  ```csharp
  public WeaponStyle WeaponStyle { get; set; }
  ```
  Gets or sets the weapon style used by the AutoEquip system.

#### CharacterSettings Fields

- **`Initialized Field`**
  ```csharp
  public bool Initialized
  ```
  true if initialized.

---

### CombatAssistSettings Class

**Inheritance:** System.Object → Styx.Helpers.Settings → Styx.Helpers.CombatAssistSettings

```csharp
public class CombatAssistSettings : Settings
```

A combat assist settings.

#### CombatAssistSettings Properties

- **`RafFollowDistance Property`**
  ```csharp
  public int RafFollowDistance { get; set; }
  ```
  Gets or sets the raf follow distance.

#### CombatAssistSettings Fields

- **`Instance Field`**
  ```csharp
  public static readonly CombatAssistSettings Instance
  ```
  The instance.

---

### DefaultValueAttribute Class

**Inheritance:** System.Object → System.Attribute → Styx.Helpers.DefaultValueAttribute

```csharp
public class DefaultValueAttribute : Attribute
```

Gives a setting a default value it will be initialized with.

#### DefaultValueAttribute Fields

- **`Value Field`**
  ```csharp
  public readonly Object Value
  ```
  The value.

---

### DictionaryExtensions Class

**Inheritance:** System.Object → Styx.Helpers.DictionaryExtensions

```csharp
public static class DictionaryExtensions
```

A dictionary extensions.

#### DictionaryExtensions Methods

- **`RemoveAll(TKey, TValue) Method`**
  ```csharp
  public static void RemoveAll&lt;TKey, TValue&gt;(
this Dictionary&lt;TKey, TValue&gt; dic,
Func&lt;TValue, bool&gt; predicate
)
  ```
  A Dictionary&lt;TKey,TValue&gt; extension method that removes all.
  - *dic*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTA7AC5AC9_3?cs=.|vb=.|cpp=::|nu=.|fs=.");DictionaryAddLanguageSpecificTextSet("LSTA7AC5AC9_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TKey, TValueAddLanguageSpecificTextSet("LSTA7AC5AC9_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");      The dic to act on.
  - *predicate*: Type: SystemAddLanguageSpecificTextSet("LSTA7AC5AC9_6?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTA7AC5AC9_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TValue, BooleanAddLanguageSpecificTextSet("LSTA7AC5AC9_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The predicate.
  - *TKey*: Type of the key.
  - *TValue*: Type of the value.

---

### EncryptedAttribute Class

**Inheritance:** System.Object → System.Attribute → Styx.Helpers.SettingAttribute → Styx.Helpers.EncryptedAttribute

```csharp
public class EncryptedAttribute : SettingAttribute
```

Encrypts a setting using passed in key and IV.

#### EncryptedAttribute Methods

- **`Decrypt Method`**
  ```csharp
  public string Decrypt(
byte[] encryptedData
)
  ```
  Decrypts the given encrypted data.
  - *encryptedData*: Type: AddLanguageSpecificTextSet("LSTBA59CEA7_1?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTBA59CEA7_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ByteAddLanguageSpecificTextSet("LSTBA59CEA7_3?cpp=&gt;|vb=()|nu=[]");Information describing the encrypted.
  - **Returns:** ReferenceEncryptedAttribute Class

- **`Encrypt Method`**
  ```csharp
  public byte[] Encrypt(
string stringToEncrypt
)
  ```
  Encrypts.
  - *stringToEncrypt*: Type: SystemAddLanguageSpecificTextSet("LSTBE5FF74_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe string to encrypt.
  - **Returns:** See Also

---

### Extensions Class

**Inheritance:** System.Object → Styx.Helpers.Extensions

```csharp
public static class Extensions
```

An extensions.

#### Extensions Methods

- **`ContainsAny Method`**
  A string extension method that query if 'str' contains any.

- **`ContainsAny Method (IEnumerable(String), IEnumerable(String))`**
  ```csharp
  public static bool ContainsAny(
this IEnumerable&lt;string&gt; enumer,
IEnumerable&lt;string&gt; list
)
  ```
  A string extension method that query if 'str' contains any.
  - *enumer*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST98DBFC96_5?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST98DBFC96_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");StringAddLanguageSpecificTextSet("LST98DBFC96_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The enumer to act on.
  - *list*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST98DBFC96_8?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST98DBFC96_9?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");StringAddLanguageSpecificTextSet("LST98DBFC96_10?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");  The list.
  - **Returns:** See Also

- **`ContainsAny Method (String, IEnumerable(String))`**
  ```csharp
  public static bool ContainsAny(
this string str,
IEnumerable&lt;string&gt; list
)
  ```
  A string extension method that query if 'str' contains any.
  - *str*: Type: SystemAddLanguageSpecificTextSet("LSTA847DD0E_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String The str to act on.
  - *list*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTA847DD0E_4?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTA847DD0E_5?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");StringAddLanguageSpecificTextSet("LSTA847DD0E_6?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The list.
  - **Returns:** ReferenceExtensions Class

- **`IndexOf Method`**
  A T[] extension method that searches for the first match.

- **`IndexOf(T) Method (T[], T)`**
  ```csharp
  public static int IndexOf&lt;T&gt;(
this T[] arr,
T val
)
  ```
  A T[] extension method that searches for the first match.
  - *arr*: Type: AddLanguageSpecificTextSet("LSTC3255E0D_5?cpp=array&lt;");TAddLanguageSpecificTextSet("LSTC3255E0D_6?cpp=&gt;|vb=()|nu=[]");The arr to act on.
  - *val*: Type: TThe value.
  - *T*: Generic type parameter.
  - **Returns:** ReferenceExtensions Class

- **`IndexOf(T) Method (T[], T, Int32)`**
  ```csharp
  public static int IndexOf&lt;T&gt;(
this T[] arr,
T val,
int startIndex
)
  ```
  A T[] extension method that searches for the first match.
  - *arr*: Type: AddLanguageSpecificTextSet("LST20EB3118_5?cpp=array&lt;");TAddLanguageSpecificTextSet("LST20EB3118_6?cpp=&gt;|vb=()|nu=[]");       The arr to act on.
  - *val*: Type: T       The value.
  - *startIndex*: Type: SystemAddLanguageSpecificTextSet("LST20EB3118_7?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The start index.
  - *T*: Generic type parameter.
  - **Returns:** ReferenceExtensions Class

- **`IndexOf(T) Method (T[], T, Int32, Int32)`**
  ```csharp
  public static int IndexOf&lt;T&gt;(
this T[] arr,
T val,
int startIndex,
int count
)
  ```
  A T[] extension method that searches for the first match.
  - *arr*: Type: AddLanguageSpecificTextSet("LSTC4461B19_5?cpp=array&lt;");TAddLanguageSpecificTextSet("LSTC4461B19_6?cpp=&gt;|vb=()|nu=[]");       The arr to act on.
  - *val*: Type: T       The value.
  - *startIndex*: Type: SystemAddLanguageSpecificTextSet("LSTC4461B19_7?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The start index.
  - *count*: Type: SystemAddLanguageSpecificTextSet("LSTC4461B19_8?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32     Number of.
  - *T*: Generic type parameter.
  - **Returns:** ReferenceExtensions Class

- **`IndexOf(T) Method (T[], T, Int32, Int32, Int32)`**
  ```csharp
  public static int IndexOf&lt;T&gt;(
this T[] arr,
T val,
int startIndex,
int count,
int failReturn
)
  ```
  A T[] extension method that searches for the first match.
  - *arr*: Type: AddLanguageSpecificTextSet("LST5E082A56_5?cpp=array&lt;");TAddLanguageSpecificTextSet("LST5E082A56_6?cpp=&gt;|vb=()|nu=[]");       The arr to act on.
  - *val*: Type: T       The value.
  - *startIndex*: Type: SystemAddLanguageSpecificTextSet("LST5E082A56_7?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The start index.
  - *count*: Type: SystemAddLanguageSpecificTextSet("LST5E082A56_8?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32     Number of.
  - *failReturn*: Type: SystemAddLanguageSpecificTextSet("LST5E082A56_9?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The fail return.
  - *T*: Generic type parameter.
  - **Returns:** ReferenceExtensions Class

- **`Parse(T) Method`**
  ```csharp
  public static T Parse&lt;T&gt;(
string value
)
  ```
  Parses.
  - *value*: Type: SystemAddLanguageSpecificTextSet("LSTF32AFDD7_3?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe value to act on.
  - *T*: Generic type parameter.
  - **Returns:** See Also

- **`ToBoolean Method`**
  ```csharp
  public static bool ToBoolean(
this string value
)
  ```
  A string extension method that converts a value to a boolean.
  - *value*: Type: SystemAddLanguageSpecificTextSet("LST88653238_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe value to act on.
  - **Returns:** ReferenceExtensions Class

- **`ToFloat Method`**
  ```csharp
  public static float ToFloat(
this string value
)
  ```
  A string extension method that converts a value to a float.
  - *value*: Type: SystemAddLanguageSpecificTextSet("LSTC04D0376_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe value to act on.
  - **Returns:** ReferenceExtensions Class

- **`ToInt32 Method`**
  ```csharp
  public static int ToInt32(
this string value
)
  ```
  A string extension method that converts a value to an int 32.
  - *value*: Type: SystemAddLanguageSpecificTextSet("LST878AA4D4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe value to act on.
  - **Returns:** ReferenceExtensions Class

- **`ToRealString Method`**
  An IList extension method that converts a lst to a real string.

- **`ToRealString Method (IList)`**
  ```csharp
  public static string ToRealString(
this IList lst
)
  ```
  An IList extension method that converts a lst to a real string.
  - *lst*: Type: System.CollectionsAddLanguageSpecificTextSet("LST659111FC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IListThe lst to act on.
  - **Returns:** ReferenceExtensions Class

- **`ToRealString(T) Method (T[])`**
  ```csharp
  public static string ToRealString&lt;T&gt;(
this T[] lst
)
  ```
  A T[] extension method that converts a lst to a real string.
  - *lst*: Type: AddLanguageSpecificTextSet("LST182B8D58_5?cpp=array&lt;");TAddLanguageSpecificTextSet("LST182B8D58_6?cpp=&gt;|vb=()|nu=[]");The lst to act on.
  - *T*: Generic type parameter.
  - **Returns:** ReferenceExtensions Class

- **`ToStringInvariant Method`**
  A double extension method that converts a value to a string invariant.

- **`ToStringInvariant Method (Double)`**
  ```csharp
  public static string ToStringInvariant(
this double value
)
  ```
  A double extension method that converts a value to a string invariant.
  - *value*: Type: SystemAddLanguageSpecificTextSet("LSTAD8F56E5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe value to act on.
  - **Returns:** ReferenceExtensions Class

- **`ToStringInvariant Method (Single)`**
  ```csharp
  public static string ToStringInvariant(
this float value
)
  ```
  A double extension method that converts a value to a string invariant.
  - *value*: Type: SystemAddLanguageSpecificTextSet("LST95456576_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe value to act on.
  - **Returns:** ReferenceExtensions Class

- **`ToUInt32 Method`**
  A string extension method that converts a value to an u int 32.

- **`ToUInt32 Method (IntPtr)`**
  ```csharp
  public static uint ToUInt32(
this IntPtr val
)
  ```
  A string extension method that converts a value to an u int 32.
  - *val*: Type: SystemAddLanguageSpecificTextSet("LSTF802FC53_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IntPtrThe value.
  - **Returns:** ReferenceExtensions Class

- **`ToUInt32 Method (String)`**
  ```csharp
  public static uint ToUInt32(
this string value
)
  ```
  A string extension method that converts a value to an u int 32.
  - *value*: Type: SystemAddLanguageSpecificTextSet("LST118BBBA3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe value to act on.
  - **Returns:** ReferenceExtensions Class

- **`ToUInt64 Method`**
  ```csharp
  public static ulong ToUInt64(
this IntPtr val
)
  ```
  An IntPtr extension method that converts a val to an u int 64.
  - *val*: Type: SystemAddLanguageSpecificTextSet("LST1BF53A74_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IntPtrThe value.
  - **Returns:** ReferenceExtensions Class

- **`ToUTF8String Method`**
  A byte[] extension method that converts this object to an UTF 8 string.

- **`ToUTF8String Method (Byte[])`**
  ```csharp
  public static string ToUTF8String(
this byte[] arr
)
  ```
  A byte[] extension method that converts this object to an UTF 8 string.
  - *arr*: Type: AddLanguageSpecificTextSet("LST3C99E127_3?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST3C99E127_4?cs=.|vb=.|cpp=::|nu=.|fs=.");ByteAddLanguageSpecificTextSet("LST3C99E127_5?cpp=&gt;|vb=()|nu=[]");The arr to act on.
  - **Returns:** ReferenceExtensions Class

- **`ToUTF8String Method (Byte[], Int32)`**
  ```csharp
  public static string ToUTF8String(
this byte[] arr,
int startIndex
)
  ```
  A byte[] extension method that converts this object to an UTF 8 string.
  - *arr*: Type: AddLanguageSpecificTextSet("LST63F54EA0_3?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST63F54EA0_4?cs=.|vb=.|cpp=::|nu=.|fs=.");ByteAddLanguageSpecificTextSet("LST63F54EA0_5?cpp=&gt;|vb=()|nu=[]");       The arr to act on.
  - *startIndex*: Type: SystemAddLanguageSpecificTextSet("LST63F54EA0_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The start index.
  - **Returns:** ReferenceExtensions Class

- **`ToUTF8String Method (Byte[], Int32, Int32)`**
  ```csharp
  public static string ToUTF8String(
this byte[] arr,
int startIndex,
int maxCount
)
  ```
  A byte[] extension method that converts this object to an UTF 8 string.
  - *arr*: Type: AddLanguageSpecificTextSet("LSTC0DD09B_3?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTC0DD09B_4?cs=.|vb=.|cpp=::|nu=.|fs=.");ByteAddLanguageSpecificTextSet("LSTC0DD09B_5?cpp=&gt;|vb=()|nu=[]");       The arr to act on.
  - *startIndex*: Type: SystemAddLanguageSpecificTextSet("LSTC0DD09B_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The start index.
  - *maxCount*: Type: SystemAddLanguageSpecificTextSet("LSTC0DD09B_7?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32  Number of maximums.
  - **Returns:** ReferenceExtensions Class

---

### FlagCheckedListBox Class

**Inheritance:** System.Object → System.MarshalByRefObject → System.ComponentModel.Component → System.Windows.Forms.Control → System.Windows.Forms.ListControl → System.Windows.Forms.ListBox → System.Windows.Forms.CheckedListBox → Styx.Helpers.FlagCheckedListBox

```csharp
public class FlagCheckedListBox : CheckedListBox
```

A flag checked list box.

#### FlagCheckedListBox Properties

- **`EnumValue Property`**
  ```csharp
  public Enum EnumValue { get; set; }
  ```
  Gets or sets the enum value.

#### FlagCheckedListBox Methods

- **`Add Method`**
  Adds item.

- **`Add Method (FlagCheckedListBoxItem)`**
  ```csharp
  public FlagCheckedListBoxItem Add(
FlagCheckedListBoxItem item
)
  ```
  Adds item.
  - *item*: Type: Styx.HelpersAddLanguageSpecificTextSet("LST65B335EC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");FlagCheckedListBoxItemThe item to add.
  - **Returns:** ReferenceFlagCheckedListBox Class

- **`Add Method (Int32, String)`**
  ```csharp
  public FlagCheckedListBoxItem Add(
int v,
string c
)
  ```
  Adds an integer value and its associated description.
  - *v*: Type: SystemAddLanguageSpecificTextSet("LST2D8CF061_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The int to process.
  - *c*: Type: SystemAddLanguageSpecificTextSet("LST2D8CF061_2?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe string to process.
  - **Returns:** ReferenceFlagCheckedListBox Class

- **`Dispose Method`**
  Releases all resources used by the Component.

- **`Dispose Method (Boolean)`**
  ```csharp
  protected override void Dispose(
bool disposing
)
  ```
  Releases the unmanaged resources used by the FlagCheckedListBox and optionally releases the managed resources
  - *disposing*: Type: SystemAddLanguageSpecificTextSet("LST7C57C96B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanTrue to release both managed and unmanaged resources; false to release only unmanaged resources

- **`GetCurrentValue Method`**
  ```csharp
  public int GetCurrentValue()
  ```
  Gets the current bit value corresponding to all checked items.
  - **Returns:** ReferenceFlagCheckedListBox Class

- **`OnItemCheck Method`**
  ```csharp
  protected override void OnItemCheck(
ItemCheckEventArgs e
)
  ```
  - *e*: Type: System.Windows.FormsAddLanguageSpecificTextSet("LSTA92760BE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ItemCheckEventArgs

- **`UpdateCheckedItems Method`**

- **`UpdateCheckedItems Method (Int32)`**
  ```csharp
  protected void UpdateCheckedItems(
int value
)
  ```
  - *value*: Type: SystemAddLanguageSpecificTextSet("LSTF236F084_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32

- **`UpdateCheckedItems Method (FlagCheckedListBoxItem, CheckState)`**
  ```csharp
  protected void UpdateCheckedItems(
FlagCheckedListBoxItem composite,
CheckState cs
)
  ```
  - *composite*: Type: Styx.HelpersAddLanguageSpecificTextSet("LSTBF044A1D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");FlagCheckedListBoxItem
  - *cs*: Type: System.Windows.FormsAddLanguageSpecificTextSet("LSTBF044A1D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");CheckState

---

### FlagCheckedListBoxItem Class

**Inheritance:** System.Object → Styx.Helpers.FlagCheckedListBoxItem

```csharp
public class FlagCheckedListBoxItem
```

Represents an item in the checklistbox.

#### FlagCheckedListBoxItem Properties

- **`IsFlag Property`**
  ```csharp
  public bool IsFlag { get; }
  ```
  Returns true if the value corresponds to a single bit being set.

#### FlagCheckedListBoxItem Methods

- **`IsMemberFlag Method`**
  ```csharp
  public bool IsMemberFlag(
FlagCheckedListBoxItem composite
)
  ```
  Returns true if this value is a member of the composite bit value.
  - *composite*: Type: Styx.HelpersAddLanguageSpecificTextSet("LSTF2EA4AD6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");FlagCheckedListBoxItemThe composite.
  - **Returns:** ReferenceFlagCheckedListBoxItem Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceFlagCheckedListBoxItem Class

#### FlagCheckedListBoxItem Fields

- **`caption Field`**
  ```csharp
  public string caption
  ```
  The caption.

- **`value Field`**
  ```csharp
  public int value
  ```
  The value.

---

### FlagEnumUIEditor Class

**Inheritance:** System.Object → System.Drawing.Design.UITypeEditor → Styx.Helpers.FlagEnumUIEditor

```csharp
public class FlagEnumUIEditor : UITypeEditor
```

UITypeEditor for flag enums.

#### FlagEnumUIEditor Methods

- **`EditValue Method`**
  Edits the value of the specified object using the editor style indicated by the GetEditStyle method.

- **`EditValue Method (ITypeDescriptorContext, IServiceProvider, Object)`**
  ```csharp
  public override Object EditValue(
ITypeDescriptorContext context,
IServiceProvider provider,
Object value
)
  ```
  Edits the specified object's value using the editor style indicated by the
            GetEditStyle method.
  - *context*: Type: System.ComponentModelAddLanguageSpecificTextSet("LST7291B156_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ITypeDescriptorContext An ITypeDescriptorContext that
            can be used to gain additional context information.
  - *provider*: Type: SystemAddLanguageSpecificTextSet("LST7291B156_3?cs=.|vb=.|cpp=::|nu=.|fs=.");IServiceProviderAn IServiceProvider that this editor can use
            to obtain services.
  - *value*: Type: SystemAddLanguageSpecificTextSet("LST7291B156_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Object   The object to edit.
  - **Returns:** ReferenceFlagEnumUIEditor Class

- **`GetEditStyle Method`**
  Gets the editor style used by the EditValue(IServiceProvider, Object) method.

- **`GetEditStyle Method (ITypeDescriptorContext)`**
  ```csharp
  public override UITypeEditorEditStyle GetEditStyle(
ITypeDescriptorContext context
)
  ```
  Gets the editor style used by the
            EditValue(IServiceProvider, Object)
            method.
  - *context*: Type: System.ComponentModelAddLanguageSpecificTextSet("LST664C870E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ITypeDescriptorContextAn ITypeDescriptorContext that can
            be used to gain additional context information.
  - **Returns:** See Also

---

### GameDebugAddStringDelegate Delegate

```csharp
public delegate void GameDebugAddStringDelegate(
StringBuilder sb
)
```

Game debug add string delegate.

---

### GlobalSettings Class

**Inheritance:** System.Object → Styx.Helpers.Settings → Styx.Helpers.GlobalSettings

```csharp
public class GlobalSettings : Settings,
INotifyPropertyChanged
```

A global settings.

#### GlobalSettings Properties

- **`AdvancedSettingsMode Property`**
  ```csharp
  public bool AdvancedSettingsMode { get; set; }
  ```
  Gets or sets a value indicating whether the advanced settings mode.

- **`BotsPath Property`**
  ```csharp
  public string BotsPath { get; set; }
  ```
  Gets or sets the full pathname of the bots file.

- **`CharacterDetailsInTitle Property`**
  ```csharp
  public bool CharacterDetailsInTitle { get; set; }
  ```

- **`CombatRoutinesPath Property`**
  ```csharp
  public string CombatRoutinesPath { get; set; }
  ```
  Gets or sets the full pathname of the combat routines file.

- **`Instance Property`**
  ```csharp
  public static GlobalSettings Instance { get; }
  ```
  The instance.

- **`KillBetweenHotspots Property`**
  ```csharp
  public bool KillBetweenHotspots { get; set; }
  ```
  Gets or sets a value indicating whether the kill between hotspots.

- **`LogLevel Property`**
  ```csharp
  public LogLevel LogLevel { get; set; }
  ```
  Gets or sets the log level.

- **`LogoutForInactivity Property`**
  ```csharp
  public bool LogoutForInactivity { get; set; }
  ```
  Gets or sets a value indicating whether the logout for inactivity.

- **`LogoutInactivityTimer Property`**
  ```csharp
  public int LogoutInactivityTimer { get; set; }
  ```
  Gets or sets the logout inactivity timer.

- **`LogoutInactivityUseForceQuit Property`**
  ```csharp
  public bool LogoutInactivityUseForceQuit { get; set; }
  ```
  Gets or sets a value indicating whether the logout inactivity use force quit.

- **`MeshesFolderPath Property`**
  ```csharp
  public string MeshesFolderPath { get; set; }
  ```
  Gets or sets the full pathname of the meshes folder file.

- **`MinimizeToTray Property`**
  ```csharp
  public bool MinimizeToTray { get; set; }
  ```
  Gets or sets a value indicating whether to minimize to tray.

- **`PluginsPath Property`**
  ```csharp
  public string PluginsPath { get; set; }
  ```
  Gets or sets the full pathname of the plugins file.

- **`QuestBehaviorsPath Property`**
  ```csharp
  public string QuestBehaviorsPath { get; set; }
  ```
  Gets or sets the full pathname of the question behaviors file.

- **`ReloadBotsOnFileChange Property`**
  ```csharp
  public bool ReloadBotsOnFileChange { get; set; }
  ```
  Gets or sets a value indicating whether the reload bots on file change.

- **`ReloadPluginsOnFileChange Property`**
  ```csharp
  public bool ReloadPluginsOnFileChange { get; set; }
  ```
  Gets or sets a value indicating whether the reload plugins on file change.

- **`ReloadRoutinesOnFileChange Property`**
  ```csharp
  public bool ReloadRoutinesOnFileChange { get; set; }
  ```
  Gets or sets a value indicating whether the reload routines on file change.

- **`SelectedRegion Property`**
  ```csharp
  public ConnectRegion SelectedRegion { get; set; }
  ```

- **`SendUsageInformation Property`**
  ```csharp
  public Nullable&lt;bool&gt; SendUsageInformation { get; set; }
  ```

- **`SeperatedLogFolders Property`**
  ```csharp
  public bool SeperatedLogFolders { get; }
  ```
  Gets or sets a value indicating whether the seperated log folders. 
            Use SetSeparatedLogFolders(Boolean) to set this.

- **`UICulture Property`**
  ```csharp
  public string UICulture { get; set; }
  ```

---

### KeyHelpers Class

**Inheritance:** System.Object → Styx.Helpers.KeyHelpers

```csharp
public static class KeyHelpers
```

A key helpers.

#### KeyHelpers Methods

- **`ControlSend Method`**
  ```csharp
  public static void ControlSend(
string text,
IntPtr hWnd
)
  ```
  Control send.
  - *text*: Type: SystemAddLanguageSpecificTextSet("LST82E6C5CE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe text.
  - *hWnd*: Type: SystemAddLanguageSpecificTextSet("LST82E6C5CE_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IntPtrThe window.

---

### KeyboardManager Class

**Inheritance:** System.Object → Styx.Helpers.KeyboardManager

```csharp
public static class KeyboardManager
```

Manager for keyboards.

#### KeyboardManager Methods

- **`AntiAfk Method`**
  ```csharp
  public static void AntiAfk()
  ```
  Anti afk.

- **`DownArrow Method`**
  ```csharp
  public static void DownArrow()
  ```
  Down arrow.

- **`KeyUpDown Method`**
  ```csharp
  [ObsoleteAttribute("Use PressKey/ReleaseKey")]
public static void KeyUpDown(
char key
)
  ```
  Key up down.
  - *key*: Type: SystemAddLanguageSpecificTextSet("LST88CE1C9E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CharThe key.

- **`PostMessage Method`**
  ```csharp
  public static void PostMessage(
uint msg,
uint wParam,
uint lParam
)
  ```
  Posts a message.
  - *msg*: Type: SystemAddLanguageSpecificTextSet("LSTD5A9EA28_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32   The message.
  - *wParam*: Type: SystemAddLanguageSpecificTextSet("LSTD5A9EA28_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The parameter.
  - *lParam*: Type: SystemAddLanguageSpecificTextSet("LSTD5A9EA28_3?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The parameter.

- **`PressKey Method`**
  ```csharp
  public static void PressKey(
char key
)
  ```
  Press key.
  - *key*: Type: SystemAddLanguageSpecificTextSet("LST7EF93046_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CharThe key.

- **`ReleaseKey Method`**
  ```csharp
  public static void ReleaseKey(
char key
)
  ```
  Releases the key described by key.
  - *key*: Type: SystemAddLanguageSpecificTextSet("LSTB3BD3CD0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");CharThe key.

- **`SendMessage Method`**
  ```csharp
  public static void SendMessage(
uint msg,
uint wParam,
uint lParam
)
  ```
  Sends a message.
  - *msg*: Type: SystemAddLanguageSpecificTextSet("LST635A183C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32   The message.
  - *wParam*: Type: SystemAddLanguageSpecificTextSet("LST635A183C_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The parameter.
  - *lParam*: Type: SystemAddLanguageSpecificTextSet("LST635A183C_3?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The parameter.

---

### KeyboardManager.eActionBar Enumeration

```csharp
public enum eActionBar
```

Values that represent eActionBar.

---

### KeyboardManager.eActionButton Enumeration

```csharp
public enum eActionButton
```

Values that represent eActionButton.

---

### KeyboardManager.eVirtualKeyMessages Enumeration

```csharp
public enum eVirtualKeyMessages
```

Values that represent eVirtualKeyMessages.

---

### LevelbotSettings Class

**Inheritance:** System.Object → Styx.Helpers.Settings → Styx.Helpers.LevelbotSettings

```csharp
public class LevelbotSettings : Settings
```

A levelbot settings.

#### LevelbotSettings Properties

- **`GroundMountFarmingMode Property`**
  ```csharp
  public bool GroundMountFarmingMode { get; set; }
  ```
  Gets or sets a value indicating whether the ground mount farming mode.

#### LevelbotSettings Fields

- **`Instance Field`**
  ```csharp
  public static readonly LevelbotSettings Instance
  ```
  The instance.

---

### PVPSettings Class

**Inheritance:** System.Object → Styx.Helpers.Settings → Styx.Helpers.PVPSettings

```csharp
public class PVPSettings : Settings
```

A pvp settings.

#### PVPSettings Properties

- **`BG1 Property`**
  ```csharp
  public BattlegroundType BG1 { get; set; }
  ```
  Gets or sets the background 1.

- **`BG2 Property`**
  ```csharp
  public BattlegroundType BG2 { get; set; }
  ```
  Gets or sets the background 2.

- **`IncludeMountedPlayers Property`**
  ```csharp
  public bool IncludeMountedPlayers { get; set; }
  ```
  Gets or sets a value indicating whether the mounted players should be included.

#### PVPSettings Fields

- **`Instance Field`**
  ```csharp
  public static readonly PVPSettings Instance
  ```
  The instance.

---

### PerFrameCachedValue(T) Class

**Inheritance:** System.Object → Styx.Helpers.PerFrameCachedValue.T

```csharp
public class PerFrameCachedValue&lt;T&gt;
```

Represents a value that is cached on a per-WoW frame basis.

#### PerFrameCachedValue(T) Properties

- **`Value Property`**
  ```csharp
  public T Value { get; }
  ```
  Gets the cached value.The cache is automatically updated when expired before it's returned

#### PerFrameCachedValue(T) Type Conversions

- **`Implicit Conversion (PerFrameCachedValue(T) to T)`**
  ```csharp
  public static implicit operator T (
PerFrameCachedValue&lt;T&gt; pfcv
)
  ```
  - *pfcv*: Type: Styx.HelpersAddLanguageSpecificTextSet("LST41460A5E_5?cs=.|vb=.|cpp=::|nu=.|fs=.");PerFrameCachedValueAddLanguageSpecificTextSet("LST41460A5E_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST41460A5E_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - **Returns:** See Also

---

### SettingAttribute Class

**Inheritance:** System.Object → System.Attribute → Styx.Helpers.SettingAttribute → Styx.Helpers.EncryptedAttribute

```csharp
public class SettingAttribute : Attribute
```

Marks a property as a setting, optionally providing an explanation and element name
            for the output XML file.

#### SettingAttribute Constructor

- **`SettingAttribute Constructor`**
  ```csharp
  public SettingAttribute()
  ```
  Default constructor.

- **`SettingAttribute Constructor (String)`**
  ```csharp
  public SettingAttribute(
string elementName
)
  ```
  Constructor.
  - *elementName*: Type: SystemAddLanguageSpecificTextSet("LST4D1003C6_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the element.

- **`SettingAttribute Constructor (String, String)`**
  ```csharp
  public SettingAttribute(
string elementName,
string explanation
)
  ```
  Constructor.
  - *elementName*: Type: SystemAddLanguageSpecificTextSet("LST3C24D906_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the element.
  - *explanation*: Type: SystemAddLanguageSpecificTextSet("LST3C24D906_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe explanation.

#### SettingAttribute Fields

- **`ElementName Field`**
  ```csharp
  public string ElementName
  ```
  Name of the element.

- **`Explanation Field`**
  ```csharp
  public string Explanation
  ```
  The explanation.

---

### Settings Class

**Inheritance:** System.Object → Styx.Helpers.Settings → More.

```csharp
public abstract class Settings : INotifyPropertyChanged
```

A settings.

#### Settings Properties

- **`CharacterSettingsDirectory Property`**
  ```csharp
  public static string CharacterSettingsDirectory { get; }
  ```
  Gets the pathname of the character settings directory.

- **`SettingsDirectory Property`**
  ```csharp
  public static string SettingsDirectory { get; }
  ```
  Gets the pathname of the settings directory.

- **`SettingsPath Property`**
  ```csharp
  public string SettingsPath { get; }
  ```
  The path to the settings file.

#### Settings Methods

- **`GetSettings Method`**
  ```csharp
  public Dictionary&lt;string, Object&gt; GetSettings()
  ```
  Gets the settings and their values.
  - **Returns:** Remarks

- **`GetXML Method`**
  ```csharp
  public XElement GetXML()
  ```
  Gets the XML.
  - **Returns:** ReferenceSettings Class

- **`InitializeDefaultValues Method`**
  ```csharp
  public void InitializeDefaultValues()
  ```
  Initializes the default values.

- **`Load Method`**
  ```csharp
  public void Load()
  ```
  (Re)loads the settings from SettingsPath.

- **`LoadFromFile(T) Method`**
  ```csharp
  public static T LoadFromFile&lt;T&gt;(
string path
)
where T : new(), Settings
  ```
  Loads from file.
  - *path*: Type: SystemAddLanguageSpecificTextSet("LST3CC73397_3?cs=.|vb=.|cpp=::|nu=.|fs=.");StringFull pathname of the file.
  - *T*: Generic type parameter.
  - **Returns:** Exceptions

- **`LoadFromXML Method`**
  Loads from XML.

- **`LoadFromXML Method (XElement)`**
  ```csharp
  public void LoadFromXML(
XElement root
)
  ```
  Loads from XML.
  - *root*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTD14D8F6D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe root.

- **`LoadFromXML(T) Method (XElement)`**
  ```csharp
  public static T LoadFromXML&lt;T&gt;(
XElement root
)
where T : new(), Settings
  ```
  Loads from XML.
  - *root*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST6E7C65F8_3?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe root.
  - *T*: Generic type parameter.
  - **Returns:** See Also

- **`OnPropertyChanged Method`**
  ```csharp
  protected virtual void OnPropertyChanged(
string propertyName = null
)
  ```

- **`Save Method`**
  ```csharp
  public void Save()
  ```
  Saves the settings to SettingsPath.

- **`SaveToFile Method`**
  ```csharp
  public void SaveToFile(
string path
)
  ```
  Saves to file.
  - *path*: Type: SystemAddLanguageSpecificTextSet("LSTC60BF70C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringFull pathname of the file.

#### Settings Events

- **`PropertyChanged Event`**
  ```csharp
  public event PropertyChangedEventHandler PropertyChanged
  ```

---

### TimeCachedValue(T) Class

**Inheritance:** System.Object → Styx.Helpers.TimeCachedValue.T

```csharp
public class TimeCachedValue&lt;T&gt;
```

Represents a value that is cached for a given length of time

#### TimeCachedValue(T) Constructor

- **`TimeCachedValue(T) Constructor (Int32, Func(T))`**
  ```csharp
  public TimeCachedValue(
int periodMs,
Func&lt;T&gt; producer
)
  ```
  Initializes a new instance of the TimeCachedValue.T class.
  - *periodMs*: Type: SystemAddLanguageSpecificTextSet("LST713F12D3_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The period for which the value is kept cached (ie. the interval at which the value refreshes).
  - *producer*: Type: SystemAddLanguageSpecificTextSet("LST713F12D3_7?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST713F12D3_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST713F12D3_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The value producer.

- **`TimeCachedValue(T) Constructor (TimeSpan, Func(T))`**
  ```csharp
  public TimeCachedValue(
TimeSpan period,
Func&lt;T&gt; producer
)
  ```
  Initializes a new instance of the TimeCachedValue.T class.
  - *period*: Type: SystemAddLanguageSpecificTextSet("LSTB6F6AC02_6?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeSpanThe period for which the value is kept cached (ie. the interval at which the value refreshes).
  - *producer*: Type: SystemAddLanguageSpecificTextSet("LSTB6F6AC02_7?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LSTB6F6AC02_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTB6F6AC02_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The value producer.

#### TimeCachedValue(T) Properties

- **`Value Property`**
  ```csharp
  public T Value { get; }
  ```
  Gets the value.

#### TimeCachedValue(T) Methods

- **`Refreshed Method`**
  ```csharp
  public T Refreshed()
  ```
  Refreshes the cache and returns the newly produced value.
  - **Returns:** See Also

#### TimeCachedValue(T) Type Conversions

- **`Implicit Conversion (TimeCachedValue(T) to T)`**
  ```csharp
  public static implicit operator T (
TimeCachedValue&lt;T&gt; tcv
)
  ```
  - *tcv*: Type: Styx.HelpersAddLanguageSpecificTextSet("LSTAC2AF17C_5?cs=.|vb=.|cpp=::|nu=.|fs=.");TimeCachedValueAddLanguageSpecificTextSet("LSTAC2AF17C_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTAC2AF17C_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");
  - **Returns:** See Also

---

### UISettings Class

**Inheritance:** System.Object → Styx.Helpers.Settings → Styx.Helpers.UISettings

```csharp
public class UISettings : Settings
```

A settings.

#### UISettings Properties

- **`DevToolsWindowHeight Property`**
  ```csharp
  public int DevToolsWindowHeight { get; set; }
  ```
  Gets or sets the height of the development tools window.

- **`DevToolsWindowLocationX Property`**
  ```csharp
  public int DevToolsWindowLocationX { get; set; }
  ```
  Gets or sets the development tools window location x coordinate.

- **`DevToolsWindowLocationY Property`**
  ```csharp
  public int DevToolsWindowLocationY { get; set; }
  ```
  Gets or sets the development tools window location y coordinate.

- **`DevToolsWindowState Property`**
  ```csharp
  public WindowState DevToolsWindowState { get; set; }
  ```
  Gets or sets the state of the development tools window.

- **`DevToolsWindowWidth Property`**
  ```csharp
  public int DevToolsWindowWidth { get; set; }
  ```
  Gets or sets the width of the development tools window.

- **`EnhancedMode Property`**
  ```csharp
  public bool EnhancedMode { get; set; }
  ```
  Gets or sets a value indicating whether the enhanced mode.

- **`MainWindowHeight Property`**
  ```csharp
  public int MainWindowHeight { get; set; }
  ```
  Gets or sets the height of the main window.

- **`MainWindowLocationX Property`**
  ```csharp
  public int MainWindowLocationX { get; set; }
  ```
  Gets or sets the main window location x coordinate.

- **`MainWindowLocationY Property`**
  ```csharp
  public int MainWindowLocationY { get; set; }
  ```
  Gets or sets the main window location y coordinate.

- **`MainWindowState Property`**
  ```csharp
  public WindowState MainWindowState { get; set; }
  ```
  Gets or sets the state of the main window.

- **`MainWindowWidth Property`**
  ```csharp
  public int MainWindowWidth { get; set; }
  ```
  Gets or sets the width of the main window.

- **`PluginsWindowHeight Property`**
  ```csharp
  public int PluginsWindowHeight { get; set; }
  ```
  Gets or sets the height of the plugins window.

- **`PluginsWindowLocationX Property`**
  ```csharp
  public int PluginsWindowLocationX { get; set; }
  ```
  Gets or sets the plugins window location x coordinate.

- **`PluginsWindowLocationY Property`**
  ```csharp
  public int PluginsWindowLocationY { get; set; }
  ```
  Gets or sets the plugins window location y coordinate.

- **`PluginsWindowState Property`**
  ```csharp
  public WindowState PluginsWindowState { get; set; }
  ```
  Gets or sets the state of the plugins window.

- **`PluginsWindowWidth Property`**
  ```csharp
  public int PluginsWindowWidth { get; set; }
  ```
  Gets or sets the width of the plugins window.

- **`SettingsWindowHeight Property`**
  ```csharp
  public int SettingsWindowHeight { get; set; }
  ```
  Gets or sets the height of the settings window.

- **`SettingsWindowLocationX Property`**
  ```csharp
  public int SettingsWindowLocationX { get; set; }
  ```
  Gets or sets the settings window location x coordinate.

- **`SettingsWindowLocationY Property`**
  ```csharp
  public int SettingsWindowLocationY { get; set; }
  ```
  Gets or sets the settings window location y coordinate.

- **`SettingsWindowState Property`**
  ```csharp
  public WindowState SettingsWindowState { get; set; }
  ```
  Gets or sets the state of the settings window.

- **`SettingsWindowWidth Property`**
  ```csharp
  public int SettingsWindowWidth { get; set; }
  ```
  Gets or sets the width of the settings window.

#### UISettings Fields

- **`Instance Field`**
  ```csharp
  public static readonly UISettings Instance
  ```
  The instance.

---

### WoWItemQualityExtensions Class

**Inheritance:** System.Object → Styx.Helpers.WoWItemQualityExtensions

```csharp
public static class WoWItemQualityExtensions
```

#### WoWItemQualityExtensions Methods

- **`ToFlag Method`**
  ```csharp
  public static ItemQuality ToFlag(
this WoWItemQuality quality
)
  ```
  - *quality*: Type: StyxAddLanguageSpecificTextSet("LST9F22DEFE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWItemQuality
  - **Returns:** ReferenceWoWItemQualityExtensions Class

---

### WoWMathHelper Class

**Inheritance:** System.Object → Styx.Helpers.WoWMathHelper

```csharp
public static class WoWMathHelper
```

A wow mathematics helper.

#### WoWMathHelper Methods

- **`AngularDifference Method`**
  ```csharp
  public static float AngularDifference(
float first,
float second
)
  ```
  Calculates the signed angular difference between two angles in radians.
  - *first*: Type: SystemAddLanguageSpecificTextSet("LSTA602282F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe first angle.
  - *second*: Type: SystemAddLanguageSpecificTextSet("LSTA602282F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe second angle.
  - **Returns:** See Also

- **`CalculateNeededFacing Method`**
  ```csharp
  public static float CalculateNeededFacing(
Vector3 start,
Vector3 faceTarget
)
  ```
  Calculates the required facing to face a point from any other point.
  - *start*: Type: System.NumericsAddLanguageSpecificTextSet("LST67E7715_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3     The start point.
  - *faceTarget*: Type: System.NumericsAddLanguageSpecificTextSet("LST67E7715_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The end point.
  - **Returns:** ReferenceWoWMathHelper Class

- **`CalculatePointAtSide Method`**
  ```csharp
  public static Vector3 CalculatePointAtSide(
Vector3 target,
float targetFacingInRadians,
float distanceToTarget,
bool rightSide
)
  ```
  Calculates a point at the side of a location.
  - *target*: Type: System.NumericsAddLanguageSpecificTextSet("LST22406E02_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3               The desired location to get the side of.
  - *targetFacingInRadians*: Type: SystemAddLanguageSpecificTextSet("LST22406E02_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe facing of the mob standing at the location.
  - *distanceToTarget*: Type: SystemAddLanguageSpecificTextSet("LST22406E02_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single     The distance wanted from the point to the target.
  - *rightSide*: Type: SystemAddLanguageSpecificTextSet("LST22406E02_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean            Right side of the mob or left side of the mob.
  - **Returns:** ReferenceWoWMathHelper Class

- **`CalculatePointBehind Method`**
  ```csharp
  public static Vector3 CalculatePointBehind(
Vector3 target,
float targetFacingRadians,
float distanceToTarget
)
  ```
  Calculates a point behind a unit location with facing.
  - *target*: Type: System.NumericsAddLanguageSpecificTextSet("LSTB060305D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3             Target position.
  - *targetFacingRadians*: Type: SystemAddLanguageSpecificTextSet("LSTB060305D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleTarget facing in radians.
  - *distanceToTarget*: Type: SystemAddLanguageSpecificTextSet("LSTB060305D_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single   The yards behind the target the calculate point should be.
  - **Returns:** ReferenceWoWMathHelper Class

- **`CalculatePointFrom Method`**
  ```csharp
  public static Vector3 CalculatePointFrom(
Vector3 from,
Vector3 target,
float distance
)
  ```
  Calculates the point from.
  - *from*: Type: System.NumericsAddLanguageSpecificTextSet("LSTD4EE3C61_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3    .
  - *target*: Type: System.NumericsAddLanguageSpecificTextSet("LSTD4EE3C61_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3  .
  - *distance*: Type: SystemAddLanguageSpecificTextSet("LSTD4EE3C61_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single.
  - **Returns:** ReferenceWoWMathHelper Class

- **`CalculatePointInFront Method`**
  ```csharp
  public static Vector3 CalculatePointInFront(
Vector3 target,
float targetFacingRadians,
float distanceToTarget
)
  ```
  Calculates a point in front of a unit location with facing.
  - *target*: Type: System.NumericsAddLanguageSpecificTextSet("LST6FAC1423_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3             Target position.
  - *targetFacingRadians*: Type: SystemAddLanguageSpecificTextSet("LST6FAC1423_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleTarget facing in radians.
  - *distanceToTarget*: Type: SystemAddLanguageSpecificTextSet("LST6FAC1423_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Single   The yards in front of the target the calculate point should
            be.
  - **Returns:** ReferenceWoWMathHelper Class

- **`DegreesToRadians Method`**
  ```csharp
  public static float DegreesToRadians(
float degrees
)
  ```
  Degrees to radians.
  - *degrees*: Type: SystemAddLanguageSpecificTextSet("LST8F51DFED_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe degrees.
  - **Returns:** ReferenceWoWMathHelper Class

- **`GetClosestInPath Method`**
  ```csharp
  public static WoWUnit GetClosestInPath(
IEnumerable&lt;WoWUnit&gt; units,
Vector3 destination
)
  ```
  Gets a closest in path.
  - *units*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST3FD01625_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST3FD01625_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWUnitAddLanguageSpecificTextSet("LST3FD01625_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");      The units.
  - *destination*: Type: System.NumericsAddLanguageSpecificTextSet("LST3FD01625_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3Destination for the.
  - **Returns:** ReferenceWoWMathHelper Class

- **`GetNearestPointOnLineSegment Method`**
  ```csharp
  public static Vector3 GetNearestPointOnLineSegment(
Vector3 point,
Vector3 segmentStart,
Vector3 segmentEnd
)
  ```
  Gets the location on line segment that is nearest to point.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LSTF035F011_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The point.
  - *segmentStart*: Type: System.NumericsAddLanguageSpecificTextSet("LSTF035F011_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The segment start point.
  - *segmentEnd*: Type: System.NumericsAddLanguageSpecificTextSet("LSTF035F011_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The segment end point.
  - **Returns:** See Also

- **`GetPointAt Method`**
  ```csharp
  public static Vector3 GetPointAt(
Vector3 from,
float distance,
float rotationRadians,
float pitchRadians
)
  ```
  Gets a point at a specified location, given a distance, rotation, and pitch.
  - *from*: Type: System.NumericsAddLanguageSpecificTextSet("LSTC21F6DA3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3           The origin location.
  - *distance*: Type: SystemAddLanguageSpecificTextSet("LSTC21F6DA3_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single       How far away the point should be.
  - *rotationRadians*: Type: SystemAddLanguageSpecificTextSet("LSTC21F6DA3_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe rotation in radians.
  - *pitchRadians*: Type: SystemAddLanguageSpecificTextSet("LSTC21F6DA3_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Single   The pitch in radians.
  - **Returns:** ReferenceWoWMathHelper Class

- **`GetRandomPointInCircle Method`**
  ```csharp
  public static Vector3 GetRandomPointInCircle(
Vector3 location,
float maxRadius
)
  ```
  Gets a random point in a circle centered at location along the X/Y plane.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LSTBF5CFB78_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *maxRadius*: Type: SystemAddLanguageSpecificTextSet("LSTBF5CFB78_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe max radius.
  - **Returns:** See Also

- **`GetRandomPointInSphere Method`**
  ```csharp
  public static Vector3 GetRandomPointInSphere(
Vector3 location,
float maxRadius
)
  ```
  Gets a random point in a sphere centered at location.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LSTA7AFC23F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *maxRadius*: Type: SystemAddLanguageSpecificTextSet("LSTA7AFC23F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe max radius.
  - **Returns:** See Also

- **`IsBehind Method`**
  Returns a boolean indicating whether you are behind a target. A parameter indicates
            the wideness of the cone used to test.

- **`IsBehind Method (WoWObject, WoWObject)`**
  ```csharp
  public static bool IsBehind(
WoWObject target,
WoWObject caster
)
  ```
  Returns a boolean indicating whether you are behind a target. A parameter indicates
            the wideness of the cone used to test.
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST2C32425A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectPosition of target.
  - *caster*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST2C32425A_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe caster.
  - **Returns:** ReferenceWoWMathHelper Class

- **`IsBehind Method (WoWObject, Single, WoWObject)`**
  ```csharp
  public static bool IsBehind(
WoWObject target,
float arcAngle,
WoWObject caster
)
  ```
  Returns a boolean indicating whether you are behind a target. A parameter indicates
            the wideness of the cone used to test.
  - *target*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST4B504AC5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObject  Position of target.
  - *arcAngle*: Type: SystemAddLanguageSpecificTextSet("LST4B504AC5_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe arc angle.
  - *caster*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST4B504AC5_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObject  The caster.
  - **Returns:** ReferenceWoWMathHelper Class

- **`IsBehind Method (Vector3, Vector3, Single, Single)`**
  ```csharp
  public static bool IsBehind(
Vector3 me,
Vector3 target,
float targetFacingRadians,
float arcRadians = 3.141593f
)
  ```
  Returns a boolean indicating whether you are behind a target. A parameter indicates
            the wideness of the cone used to test.
  - *me*: Type: System.NumericsAddLanguageSpecificTextSet("LST3493BD92_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3                 Position of me.
  - *target*: Type: System.NumericsAddLanguageSpecificTextSet("LST3493BD92_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3             Position of target.
  - *targetFacingRadians*: Type: SystemAddLanguageSpecificTextSet("LST3493BD92_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleFacing of target in radians.
  - **Returns:** ReferenceWoWMathHelper Class

- **`IsFacing Method`**
  ```csharp
  public static bool IsFacing(
Vector3 me,
float myFacingRadians,
Vector3 target,
float arcRadians = 3.141593f
)
  ```
  Returns a boolean indicating whether you are facing a target. A parameter indicates
            the wideness of the cone used to test.
  - *me*: Type: System.NumericsAddLanguageSpecificTextSet("LST579AE180_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3             Position of me.
  - *myFacingRadians*: Type: SystemAddLanguageSpecificTextSet("LST579AE180_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleFacing of me in radians.
  - *target*: Type: System.NumericsAddLanguageSpecificTextSet("LST579AE180_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3         Position of target.
  - **Returns:** ReferenceWoWMathHelper Class

- **`IsInPath Method`**
  Query if 'unit' is in path.

- **`IsInPath Method (IEnumerable(WoWUnit), Vector3)`**
  ```csharp
  public static bool IsInPath(
IEnumerable&lt;WoWUnit&gt; units,
Vector3 destination
)
  ```
  Query if 'unit' is in path.
  - *units*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTD0E33596_3?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTD0E33596_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");WoWUnitAddLanguageSpecificTextSet("LSTD0E33596_5?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");      The units.
  - *destination*: Type: System.NumericsAddLanguageSpecificTextSet("LSTD0E33596_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3Destination for the.
  - **Returns:** ReferenceWoWMathHelper Class

- **`IsInPath Method (BoundingSphere, Vector3, Vector3)`**
  ```csharp
  public static bool IsInPath(
BoundingSphere sphere,
Vector3 start,
Vector3 destination
)
  ```
  Query if 'a bounding sphere' is in path.
  - *sphere*: Type: Styx.CommonAddLanguageSpecificTextSet("LSTA942D598_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BoundingSphere     The sphere.
  - *start*: Type: System.NumericsAddLanguageSpecificTextSet("LSTA942D598_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3      The start point.
  - *destination*: Type: System.NumericsAddLanguageSpecificTextSet("LSTA942D598_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3Destination for the.
  - **Returns:** raphus, 17/07/2013.

- **`IsInPath Method (WoWUnit, Vector3, Vector3)`**
  ```csharp
  public static bool IsInPath(
WoWUnit unit,
Vector3 start,
Vector3 destination
)
  ```
  Query if 'unit' is in path.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST67A36661_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnit       The unit.
  - *start*: Type: System.NumericsAddLanguageSpecificTextSet("LST67A36661_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3      The start point.
  - *destination*: Type: System.NumericsAddLanguageSpecificTextSet("LST67A36661_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3Destination for the.
  - **Returns:** ReferenceWoWMathHelper Class

- **`IsInPath Method (Vector3, Single, Vector3, List(Vector3))`**
  ```csharp
  public static bool IsInPath(
Vector3 location,
float aggroRange,
Vector3 myLocation,
List&lt;Vector3&gt; path
)
  ```
  Query if 'the radius around a location' is in path.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LSTBB8A67FB_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3  The location.
  - *aggroRange*: Type: SystemAddLanguageSpecificTextSet("LSTBB8A67FB_4?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe aggro range.
  - *myLocation*: Type: System.NumericsAddLanguageSpecificTextSet("LSTBB8A67FB_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3My location.
  - *path*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTBB8A67FB_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ListAddLanguageSpecificTextSet("LSTBB8A67FB_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTBB8A67FB_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");      Full pathname of the file.
  - **Returns:** raphus, 25/07/2013.

- **`IsInPath Method (Vector3, Single, Vector3, Vector3)`**
  ```csharp
  public static bool IsInPath(
Vector3 location,
float radius,
Vector3 start,
Vector3 destination
)
  ```
  Query if 'the radius around a location' is in path.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST22CAD4A8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3   The location.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST22CAD4A8_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single     The radius.
  - *start*: Type: System.NumericsAddLanguageSpecificTextSet("LST22CAD4A8_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3      The start point.
  - *destination*: Type: System.NumericsAddLanguageSpecificTextSet("LST22CAD4A8_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3Destination for the.
  - **Returns:** raphus, 17/07/2013.

- **`IsPointInPoly Method`**
  ```csharp
  public static bool IsPointInPoly(
Vector3 point,
Vector2[] polygon
)
  ```
  Query if point is inside polygon.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LST9F585FB8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The point.
  - *polygon*: Type: AddLanguageSpecificTextSet("LST9F585FB8_2?cpp=array&lt;");System.NumericsAddLanguageSpecificTextSet("LST9F585FB8_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector2AddLanguageSpecificTextSet("LST9F585FB8_4?cpp=&gt;|vb=()|nu=[]");The polygon.
  - **Returns:** ReferenceWoWMathHelper Class

- **`IsSafelyBehind Method`**
  ```csharp
  public static bool IsSafelyBehind(
Vector3 me,
Vector3 target,
float targetFacingRadians
)
  ```
  Returns a boolean indcating whether you are safely behind a target.
  - *me*: Type: System.NumericsAddLanguageSpecificTextSet("LSTBA747471_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3                 Position of me.
  - *target*: Type: System.NumericsAddLanguageSpecificTextSet("LSTBA747471_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3             Position of target.
  - *targetFacingRadians*: Type: SystemAddLanguageSpecificTextSet("LSTBA747471_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleFacing of target in radians.
  - **Returns:** ReferenceWoWMathHelper Class

- **`IsSafelyFacing Method`**
  ```csharp
  public static bool IsSafelyFacing(
Vector3 me,
float myFacingRadians,
Vector3 target
)
  ```
  Returns a boolean indicating whether you are facing a target safely.
  - *me*: Type: System.NumericsAddLanguageSpecificTextSet("LSTF013A7D1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3             Position of me.
  - *myFacingRadians*: Type: SystemAddLanguageSpecificTextSet("LSTF013A7D1_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleFacing of me in radians.
  - *target*: Type: System.NumericsAddLanguageSpecificTextSet("LSTF013A7D1_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3         Position of target.
  - **Returns:** ReferenceWoWMathHelper Class

- **`NormalizeRadian Method`**
  ```csharp
  public static float NormalizeRadian(
float radian
)
  ```
  Normalizes a radian so it's between 0 and pi * 2.
  - *radian*: Type: SystemAddLanguageSpecificTextSet("LST32667A27_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Single.
  - **Returns:** ReferenceWoWMathHelper Class

- **`RadiansToDegrees Method`**
  ```csharp
  public static float RadiansToDegrees(
float radians
)
  ```
  Radians to degrees.
  - *radians*: Type: SystemAddLanguageSpecificTextSet("LST7BC0DFB9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe radians.
  - **Returns:** ReferenceWoWMathHelper Class

---

### WoWSpecExtensions Class

**Inheritance:** System.Object → Styx.Helpers.WoWSpecExtensions

```csharp
public static class WoWSpecExtensions
```

WoWSpec extensions

#### WoWSpecExtensions Methods

- **`IsDps Method`**
  ```csharp
  public static bool IsDps(
this WoWSpec spec
)
  ```
  Determines whether the specified spec is DPS.
  - *spec*: Type: StyxAddLanguageSpecificTextSet("LSTDD09B450_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpec
  - **Returns:** ReferenceWoWSpecExtensions Class

- **`IsHealer Method`**
  ```csharp
  public static bool IsHealer(
this WoWSpec spec
)
  ```
  Determines whether the spec is used for healing.
  - *spec*: Type: StyxAddLanguageSpecificTextSet("LST46013AD0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpec
  - **Returns:** ReferenceWoWSpecExtensions Class

- **`IsMelee Method`**
  ```csharp
  public static bool IsMelee(
this WoWSpec spec
)
  ```
  Determines whether the specified spec is melee.
  - *spec*: Type: StyxAddLanguageSpecificTextSet("LST3C28422B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpec
  - **Returns:** ReferenceWoWSpecExtensions Class

- **`IsRange Method`**
  ```csharp
  public static bool IsRange(
this WoWSpec spec
)
  ```
  Determines whether the specified spec is range.
  - *spec*: Type: StyxAddLanguageSpecificTextSet("LSTE3A65D3C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpec
  - **Returns:** ReferenceWoWSpecExtensions Class

- **`IsTank Method`**
  ```csharp
  public static bool IsTank(
this WoWSpec spec
)
  ```
  Determines whether the spec is used for tanking.
  - *spec*: Type: StyxAddLanguageSpecificTextSet("LST529A115B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpec
  - **Returns:** ReferenceWoWSpecExtensions Class

---

### XmlExtensions Class

**Inheritance:** System.Object → Styx.Helpers.XmlExtensions

```csharp
public static class XmlExtensions
```

An XML extensions.

#### XmlExtensions Methods

- **`Error Method`**
  ```csharp
  public static void Error(
this XObject actualElement,
string errorMessage
)
  ```
  An XNode extension method that prints an error message, including line number [if
            viable] to the Debug logger.
  - *actualElement*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST9E967A47_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XObjectThe actualElement to act on.
  - *errorMessage*: Type: SystemAddLanguageSpecificTextSet("LST9E967A47_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String Message describing the error.

- **`GetError Method`**
  ```csharp
  public static string GetError(
this XObject actualElement,
string errorMessage
)
  ```
  An XObject extension method that gets an error.
  - *actualElement*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTCE0076BD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XObjectThe actualElement to act on.
  - *errorMessage*: Type: SystemAddLanguageSpecificTextSet("LSTCE0076BD_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String Message describing the error.
  - **Returns:** ReferenceXmlExtensions Class

---

### XmlUtils Class

**Inheritance:** System.Object → Styx.Helpers.XmlUtils

```csharp
public static class XmlUtils
```

An XML utilities.

#### XmlUtils Methods

- **`FindAttributeValue Method`**
  ```csharp
  public static string FindAttributeValue(
XElement element,
string name,
bool caseSensitive
)
  ```
  Searches for the first attribute value.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST30C87A92_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElement      The element.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LST30C87A92_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String         The attribute name to get the value from.
  - *caseSensitive*: Type: SystemAddLanguageSpecificTextSet("LST30C87A92_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Booleantrue to case sensitive.
  - **Returns:** ReferenceXmlUtils Class

- **`GetInt32Attribute Method`**
  ```csharp
  public static bool GetInt32Attribute(
XElement element,
string name,
int minValue,
int maxValue,
out int value
)
  ```
  Gets an integer value from the specified element, using the provided attribute name.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTEE724510_1?cs=.|vb=.|cpp=::|nu=.|fs=.");XElement The element.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LSTEE724510_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String    The attribute name to get the value from.
  - *minValue*: Type: SystemAddLanguageSpecificTextSet("LSTEE724510_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The minimum value.
  - *maxValue*: Type: SystemAddLanguageSpecificTextSet("LSTEE724510_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The maximum value.
  - *value*: Type: SystemAddLanguageSpecificTextSet("LSTEE724510_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LSTEE724510_6?cpp=%");   [out] The value of the attribute.
  - **Returns:** ReferenceXmlUtils Class

---
