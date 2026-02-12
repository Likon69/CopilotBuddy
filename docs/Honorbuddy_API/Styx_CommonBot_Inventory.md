# Styx.CommonBot.Inventory

Contains classes for inventory related classes.

## Contents

- [Consumable Class](#consumable-class)
- [EquipmentManager Class](#equipmentmanager-class)
- [EquipmentSet Class](#equipmentset-class)
- [InventoryManager Class](#inventorymanager-class)
- [InventorySlot Enumeration](#inventoryslot-enumeration)
- [LootRoll Class](#lootroll-class)
- [LootRoll.LootRollEventDelegate Delegate](#lootroll.lootrolleventdelegate-delegate)
- [LootRoll.LootRollItem Class](#lootroll.lootrollitem-class)
- [WoWPrice Class](#wowprice-class)

---

### Consumable Class

**Inheritance:** System.Object → Styx.CommonBot.Inventory.Consumable

```csharp
public static class Consumable
```

A small wrapper class around consumable items. [Food, Water, Potions, Bandages, etc].

#### Consumable Methods

- **`GetBestDrink Method`**
  ```csharp
  public static WoWItem GetBestDrink(
bool includeSpecialtyItems
)
  ```
  Returns the best drink in your inventory, optionally including specialty items.
  - *includeSpecialtyItems*: Type: SystemAddLanguageSpecificTextSet("LST3CA6E3B6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean.
  - **Returns:** ReferenceConsumable Class

- **`GetBestFood Method`**
  ```csharp
  public static WoWItem GetBestFood(
bool includeSpecialtyItems
)
  ```
  Returns the best food you currently have, optionally including specialty foods.
  - *includeSpecialtyItems*: Type: SystemAddLanguageSpecificTextSet("LSTD52EFC94_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean.
  - **Returns:** ReferenceConsumable Class

- **`GetDrinks Method`**
  ```csharp
  public static List&lt;WoWItem&gt; GetDrinks()
  ```
  Returns a list of all the drinks you currently have in your inventory.
  - **Returns:** See Also

- **`GetFood Method`**
  ```csharp
  public static List&lt;WoWItem&gt; GetFood()
  ```
  Returns a list of all the food you currently have in your inventory.
  - **Returns:** See Also

- **`IsDrink Method`**
  ```csharp
  public static bool IsDrink(
ItemInfo itemInfo
)
  ```
  Determines whether the specified item is drink.
  - *itemInfo*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST71A1CA9C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ItemInfoThe itemInfo.
  - **Returns:** See Also

- **`IsFood Method`**
  ```csharp
  public static bool IsFood(
ItemInfo itemInfo
)
  ```
  Determines whether the specified item is food.
  - *itemInfo*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTD35E8EE6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ItemInfoThe itemInfo.
  - **Returns:** See Also

---

### EquipmentManager Class

**Inheritance:** System.Object → Styx.CommonBot.Inventory.EquipmentManager

```csharp
public static class EquipmentManager
```

Manager for equipment sets.

#### EquipmentManager Properties

- **`ActiveSet Property`**
  ```csharp
  public static EquipmentSet ActiveSet { get; }
  ```
  Gets the active equipment set.

- **`IsEnabled Property`**
  ```csharp
  public static bool IsEnabled { get; }
  ```
  Gets a value indicating whether equipment sets is enabled in WoW.

#### EquipmentManager Methods

- **`EquipSet Method`**
  ```csharp
  public static bool EquipSet(
string name
)
  ```
  Equip a set.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LST62DA94B7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name.
  - **Returns:** raphus, 08/06/2013.

#### EquipmentManager Fields

- **`EquipmentSets Field`**
  ```csharp
  public static readonly List&lt;EquipmentSet&gt; EquipmentSets
  ```
  List of equipment sets.

---

### EquipmentSet Class

**Inheritance:** System.Object → Styx.CommonBot.Inventory.EquipmentSet

```csharp
public class EquipmentSet
```

Equipment set.

#### EquipmentSet Properties

- **`IsEquipped Property`**
  ```csharp
  public bool IsEquipped { get; }
  ```
  Gets a value indicating whether this object is equipped.

- **`Items Property`**
  ```csharp
  public IEnumerable&lt;WoWItem&gt; Items { get; }
  ```
  Gets the items.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`NumEquipped Property`**
  ```csharp
  public int NumEquipped { get; }
  ```
  Gets the number of equipped items.

- **`NumIgnored Property`**
  ```csharp
  public int NumIgnored { get; }
  ```
  Gets the number of ignored slots.

- **`NumInInventory Property`**
  ```csharp
  public int NumInInventory { get; }
  ```
  Gets the number of items in inventory.

- **`NumItems Property`**
  ```csharp
  public int NumItems { get; }
  ```
  Gets the number of items.

- **`NumMissing Property`**
  ```csharp
  public int NumMissing { get; }
  ```
  Gets the number of missing items.

#### EquipmentSet Methods

- **`DeleteEquipmentSet Method`**
  ```csharp
  public void DeleteEquipmentSet()
  ```
  Deletes the equipment set.

- **`SaveEquipmentSet Method`**
  ```csharp
  public void SaveEquipmentSet()
  ```
  Saves the equipment set.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a string that represents the current object.
  - **Returns:** raphus, 08/06/2013.

- **`UseEquipmentSet Method`**
  ```csharp
  public void UseEquipmentSet()
  ```
  Use equipment set.

---

### InventoryManager Class

**Inheritance:** System.Object → Styx.CommonBot.Inventory.InventoryManager

```csharp
public static class InventoryManager
```

Manager for inventories.

#### InventoryManager Properties

- **`HaveItemsToMail Property`**
  ```csharp
  public static bool HaveItemsToMail { get; }
  ```
  Gets a value indicating whether the have items to mail.

- **`MailQualities Property`**
  ```csharp
  public static List&lt;WoWItemQuality&gt; MailQualities { get; }
  ```
  Gets the mail qualities.

#### InventoryManager Methods

- **`GetInventorySlotsByEquipSlot Method`**
  ```csharp
  public static List&lt;InventorySlot&gt; GetInventorySlotsByEquipSlot(
InventoryType type
)
  ```
  Returns a list of inventory slots that an item with the specified equip slot would be
            able to go in.
  - *type*: Type: StyxAddLanguageSpecificTextSet("LST685DA0E1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");InventoryTypeThe type.
  - **Returns:** See Also

- **`GetItemsToMail Method`**
  ```csharp
  public static WoWItem[] GetItemsToMail()
  ```
  Gets items to mail.
  - **Returns:** See Also

---

### InventorySlot Enumeration

```csharp
public enum InventorySlot
```

Values that represent InventorySlot.

---

### LootRoll Class

**Inheritance:** System.Object → Styx.CommonBot.Inventory.LootRoll

```csharp
public class LootRoll
```

A loot roll.

#### LootRoll Events

- **`OnLootRoll Event`**
  ```csharp
  public static event LootRoll.LootRollEventDelegate OnLootRoll
  ```
  Event queue for all listeners interested in OnLootRoll events.

---

### LootRoll.LootRollEventDelegate Delegate

```csharp
public delegate void LootRollEventDelegate(
LootRollItemInfo info
)
```

Loot roll event delegate.

---

### LootRoll.LootRollItem Class

**Inheritance:** System.Object → Styx.CommonBot.Inventory.LootRoll.LootRollItem

```csharp
public class LootRollItem
```

A loot roll item.

#### LootRollItem Properties

- **`CanDisenchant Property`**
  ```csharp
  public bool CanDisenchant { get; }
  ```
  Gets a value indicating whether we can disenchant.

- **`CanGreed Property`**
  ```csharp
  public bool CanGreed { get; }
  ```
  Gets a value indicating whether we can greed.

- **`CanNeed Property`**
  ```csharp
  public bool CanNeed { get; }
  ```
  Gets a value indicating whether we can need.

- **`RollId Property`**
  ```csharp
  public uint RollId { get; }
  ```
  Gets the identifier of the roll.

---

### WoWPrice Class

**Inheritance:** System.Object → Styx.CommonBot.Inventory.WoWPrice

```csharp
public class WoWPrice
```

Class representing the price of something that can be bought in wow.

#### WoWPrice Properties

- **`Copper Property`**
  ```csharp
  public long Copper { get; }
  ```
  The amount of copper.

- **`Gold Property`**
  ```csharp
  public long Gold { get; }
  ```
  The amount of gold.

- **`Silver Property`**
  ```csharp
  public long Silver { get; }
  ```
  The amount of silver.

- **`TotalCoppers Property`**
  ```csharp
  public long TotalCoppers { get; }
  ```
  The total amount of coppers.

#### WoWPrice Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceWoWPrice Class

#### WoWPrice Operators

- **`Addition Operator`**
  Addition operator.

- **`Addition Operator (WoWPrice, WoWPrice)`**
  ```csharp
  public static WoWPrice operator +(
WoWPrice left,
WoWPrice right
)
  ```
  Addition operator.
  - *left*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LST48F79AC6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPrice The left.
  - *right*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LST48F79AC6_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPriceThe right.
  - **Returns:** ReferenceWoWPrice Class

- **`Addition Operator (WoWPrice, Int64)`**
  ```csharp
  public static WoWPrice operator +(
WoWPrice left,
long copper
)
  ```
  Addition operator.
  - *left*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LSTDACC0D62_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPrice  The left.
  - *copper*: Type: SystemAddLanguageSpecificTextSet("LSTDACC0D62_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int64The copper.
  - **Returns:** ReferenceWoWPrice Class

- **`Division Operator`**
  Division operator.

- **`Division Operator (WoWPrice, WoWPrice)`**
  ```csharp
  public static WoWPrice operator /(
WoWPrice left,
WoWPrice right
)
  ```
  Division operator.
  - *left*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LST4B4167D1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPrice The left.
  - *right*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LST4B4167D1_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPriceThe right.
  - **Returns:** ReferenceWoWPrice Class

- **`Division Operator (WoWPrice, Double)`**
  ```csharp
  public static WoWPrice operator /(
WoWPrice left,
double factor
)
  ```
  Division operator.
  - *left*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LST293DD37_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPrice  The left.
  - *factor*: Type: SystemAddLanguageSpecificTextSet("LST293DD37_2?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe factor.
  - **Returns:** ReferenceWoWPrice Class

- **`Division Operator (WoWPrice, Int64)`**
  ```csharp
  public static WoWPrice operator /(
WoWPrice left,
long copper
)
  ```
  Division operator.
  - *left*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LST11CFCB2F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPrice  The left.
  - *copper*: Type: SystemAddLanguageSpecificTextSet("LST11CFCB2F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int64The copper.
  - **Returns:** ReferenceWoWPrice Class

- **`GreaterThan Operator`**
  ```csharp
  public static bool operator &gt;(
WoWPrice left,
WoWPrice right
)
  ```
  Greater-than comparison operator.
  - *left*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LST134E1035_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPrice The left.
  - *right*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LST134E1035_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPriceThe right.
  - **Returns:** ReferenceWoWPrice Class

- **`GreaterThanOrEqual Operator`**
  ```csharp
  public static bool operator &gt;=(
WoWPrice left,
WoWPrice right
)
  ```
  Greater-than-or-equal comparison operator.
  - *left*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LST2C99A616_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPrice The left.
  - *right*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LST2C99A616_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPriceThe right.
  - **Returns:** ReferenceWoWPrice Class

- **`LessThan Operator`**
  ```csharp
  public static bool operator &lt;(
WoWPrice left,
WoWPrice right
)
  ```
  Less-than comparison operator.
  - *left*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LST319D7BA8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPrice The left.
  - *right*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LST319D7BA8_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPriceThe right.
  - **Returns:** ReferenceWoWPrice Class

- **`LessThanOrEqual Operator`**
  ```csharp
  public static bool operator &lt;=(
WoWPrice left,
WoWPrice right
)
  ```
  Less-than-or-equal comparison operator.
  - *left*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LSTAD6063A7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPrice The left.
  - *right*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LSTAD6063A7_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPriceThe right.
  - **Returns:** ReferenceWoWPrice Class

- **`Multiply Operator`**
  Multiplication operator.

- **`Multiply Operator (WoWPrice, WoWPrice)`**
  ```csharp
  public static WoWPrice operator *(
WoWPrice left,
WoWPrice right
)
  ```
  Multiplication operator.
  - *left*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LSTB2FCBAE2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPrice The left.
  - *right*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LSTB2FCBAE2_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPriceThe right.
  - **Returns:** ReferenceWoWPrice Class

- **`Multiply Operator (WoWPrice, Double)`**
  ```csharp
  public static WoWPrice operator *(
WoWPrice left,
double factor
)
  ```
  Multiplication operator.
  - *left*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LSTC5C3C6FC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPrice  The left.
  - *factor*: Type: SystemAddLanguageSpecificTextSet("LSTC5C3C6FC_2?cs=.|vb=.|cpp=::|nu=.|fs=.");DoubleThe factor.
  - **Returns:** ReferenceWoWPrice Class

- **`Multiply Operator (WoWPrice, Int64)`**
  ```csharp
  public static WoWPrice operator *(
WoWPrice left,
long copper
)
  ```
  Multiplication operator.
  - *left*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LST1CD0E852_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPrice  The left.
  - *copper*: Type: SystemAddLanguageSpecificTextSet("LST1CD0E852_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int64The copper.
  - **Returns:** ReferenceWoWPrice Class

- **`Subtraction Operator`**
  Subtraction operator.

- **`Subtraction Operator (WoWPrice, WoWPrice)`**
  ```csharp
  public static WoWPrice operator -(
WoWPrice left,
WoWPrice right
)
  ```
  Subtraction operator.
  - *left*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LSTCBC656E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPrice The left.
  - *right*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LSTCBC656E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPriceThe right.
  - **Returns:** ReferenceWoWPrice Class

- **`Subtraction Operator (WoWPrice, Int64)`**
  ```csharp
  public static WoWPrice operator -(
WoWPrice left,
long copper
)
  ```
  Subtraction operator.
  - *left*: Type: Styx.CommonBot.InventoryAddLanguageSpecificTextSet("LSTF33FE302_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPrice  The left.
  - *copper*: Type: SystemAddLanguageSpecificTextSet("LSTF33FE302_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int64The copper.
  - **Returns:** ReferenceWoWPrice Class

---
