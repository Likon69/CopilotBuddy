# Styx.WoWInternals.WoWObjects

Contains WoW object related classes.

## Contents

- [BagType Enumeration](#bagtype-enumeration)
- [CorpseType Enumeration](#corpsetype-enumeration)
- [FactionStanding Structure](#factionstanding-structure)
- [GameObjectInfo Class](#gameobjectinfo-class)
- [ILootableObject Interface](#ilootableobject-interface)
- [ItemInfo Class](#iteminfo-class)
- [ItemStats Class](#itemstats-class)
- [LocalPlayer Class](#localplayer-class)
- [MirrorTimerInfo Structure](#mirrortimerinfo-structure)
- [ObjectInvalidateDelegate Delegate](#objectinvalidatedelegate-delegate)
- [RaidTargetMarker Enumeration](#raidtargetmarker-enumeration)
- [ReputationFlags Enumeration](#reputationflags-enumeration)
- [SpecType Enumeration](#spectype-enumeration)
- [UnitThreatInfo Class](#unitthreatinfo-class)
- [WoWAnimatedSubObject Class](#wowanimatedsubobject-class)
- [WoWAreaTrigger Class](#wowareatrigger-class)
- [WoWArenaTeamInfo Structure](#wowarenateaminfo-structure)
- [WoWChair Class](#wowchair-class)
- [WoWContainer Class](#wowcontainer-class)
- [WoWCorpse Class](#wowcorpse-class)
- [WoWDoor Class](#wowdoor-class)
- [WoWDynamicObject Class](#wowdynamicobject-class)
- [WoWFishingBobber Class](#wowfishingbobber-class)
- [WoWGameObject Class](#wowgameobject-class)
- [WoWGameObject.LockEntry Structure](#wowgameobject.lockentry-structure)
- [WoWGameObject.LockTypeEntry Structure](#wowgameobject.locktypeentry-structure)
- [WoWGlyphInfo Structure](#wowglyphinfo-structure)
- [WoWInebriationLevel Enumeration](#wowinebriationlevel-enumeration)
- [WoWItem Class](#wowitem-class)
- [WoWItem.WoWItemEnchantment Class](#wowitem.wowitemenchantment-class)
- [WoWItem.WoWItemStat Class](#wowitem.wowitemstat-class)
- [WoWLockType Enumeration](#wowlocktype-enumeration)
- [WoWObject Class](#wowobject-class)
- [WoWPartyMember Class](#wowpartymember-class)
- [WoWPartyMember.GroupRole Enumeration](#wowpartymember.grouprole-enumeration)
- [WoWPlayer Class](#wowplayer-class)
- [WoWPlayerCombatRating Enumeration](#wowplayercombatrating-enumeration)
- [WoWSubObject Class](#wowsubobject-class)
- [WoWUnit Class](#wowunit-class)
- [WoWUnit.PowerInfo Structure](#wowunit.powerinfo-structure)

---

### BagType Enumeration

```csharp
public enum BagType
```

Values that represent BagType.

---

### CorpseType Enumeration

```csharp
[FlagsAttribute]
public enum CorpseType
```

The corpse type.

---

### FactionStanding Structure

```csharp
public struct FactionStanding
```

The faction standing.

#### FactionStanding Properties

- **`HasBonusRepGain Property`**
  ```csharp
  public bool HasBonusRepGain { get; }
  ```
  Gets a value indicating whether this object has bonus rep gain.

- **`TotalReputation Property`**
  ```csharp
  public int TotalReputation { get; }
  ```
  Gets the total reputation.

#### FactionStanding Fields

- **`FactionId Field`**
  ```csharp
  public uint FactionId
  ```
  Identifier for the faction.

- **`Reputation Field`**
  ```csharp
  public int Reputation
  ```
  The reputation.

- **`ReputationBonus Field`**
  ```csharp
  public int ReputationBonus
  ```
  The reputation bonus.

---

### GameObjectInfo Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.GameObjectInfo

```csharp
public class GameObjectInfo
```

Information about the game object.

#### GameObjectInfo Properties

- **`BaseAddress Property`**
  ```csharp
  [ObsoleteAttribute("Always returns IntPtr.Zero. Use StyxWoW.Cache[CacheDb.GameObject] to get correct base addresses for cache entries.")]
public IntPtr BaseAddress { get; }
  ```
  For debugging purposes only.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Gets the identifier.

- **`InternalInfo Property`**
  ```csharp
  public WoWCache.GameObjectCacheEntry InternalInfo { get; }
  ```
  The internal info of this quest.

- **`IsCached Property`**
  ```csharp
  [ObsoleteAttribute("Always returns true")]
public bool IsCached { get; }
  ```
  Returns true if this quest is cached in WoW's cache. If it's true, you will be able
            to retrieve information about this quest.

- **`QuestItems Property`**
  ```csharp
  public List&lt;ItemInfo&gt; QuestItems { get; }
  ```
  Gets the question items.

#### GameObjectInfo Methods

- **`FromId Method`**
  ```csharp
  public static GameObjectInfo FromId(
uint gameObjectId
)
  ```
  Constructs a GameObjectInfo instance from a game object ID. Returns
            null if there is no cached information for this gameobject available.
  - *gameObjectId*: Type: SystemAddLanguageSpecificTextSet("LSTD8307788_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32.
  - **Returns:** ReferenceGameObjectInfo Class

- **`GetGameObjectInfo Method`**
  ```csharp
  [ObsoleteAttribute("Use InternalInfo instead")]
public WoWCache.GameObjectCacheEntry GetGameObjectInfo()
  ```
  Gets game object information.
  - **Returns:** See Also

- **`GetQuestItems Method`**
  ```csharp
  public List&lt;ItemInfo&gt; GetQuestItems()
  ```
  Gets quest items.
  - **Returns:** See Also

---

### ILootableObject Interface

```csharp
public interface ILootableObject
```

Interface provided for lootable objets.

#### ILootableObject Properties

- **`CanLoot Property`**
  ```csharp
  bool CanLoot { get; }
  ```
  Gets a value indicating whether we can loot.

---

### ItemInfo Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.ItemInfo

```csharp
public class ItemInfo
```

Information about the item.

#### ItemInfo Properties

- **`AllowedClasses Property`**
  ```csharp
  public int AllowedClasses { get; }
  ```
  Gets the allowed classes.

- **`AllowedRaces Property`**
  ```csharp
  public int AllowedRaces { get; }
  ```
  Gets the allowed races.

- **`ArmorClass Property`**
  ```csharp
  public WoWItemArmorClass ArmorClass { get; }
  ```
  The ArmorClass of this item. eg; 'Cloth', 'Leahter', 'Mail'.

- **`ArmorDamageModifier Property`**
  ```csharp
  public float ArmorDamageModifier { get; }
  ```
  Gets the armor damage modifier.

- **`BagFamily Property`**
  ```csharp
  public int BagFamily { get; }
  ```
  Gets the bag family.

- **`BagSlots Property`**
  ```csharp
  public int BagSlots { get; }
  ```
  The number of bagslots if this item is a bag, otherwise 0.

- **`BattlePetsClass Property`**
  ```csharp
  public WoWItemBattlePetsClass BattlePetsClass { get; }
  ```
  The BattlePet class. e.g. 'Humanoid', 'Dragonkin', 'Elemental'

- **`BeginQuestId Property`**
  ```csharp
  public int BeginQuestId { get; }
  ```
  The id of the quest this item starts, if any.

- **`Bond Property`**
  ```csharp
  public WoWItemBondType Bond { get; }
  ```
  The bond type of this item. eg; 'BindOnEquip', 'BindOnPickup'.

- **`BookTextId Property`**
  ```csharp
  public int BookTextId { get; }
  ```
  The book text id of this item, used for some dbc crap.

- **`BuyPrice Property`**
  ```csharp
  public int BuyPrice { get; }
  ```
  The total amount of copper this item costs from a vendor.

- **`CanExpire Property`**
  ```csharp
  public bool CanExpire { get; }
  ```
  Returns true if this ItemInfo object can expire.

- **`CanObliterate Property`**
  ```csharp
  public bool CanObliterate { get; }
  ```
  Gets a bool that indicates whether this item can be obliterated.

- **`CanProspect Property`**
  ```csharp
  public bool CanProspect { get; }
  ```
  Returns true if this ItemInfo object can be prospected.

- **`ConsumableClass Property`**
  ```csharp
  public WoWItemConsumableClass ConsumableClass { get; }
  ```
  The consumable class of this item.

- **`ContainerClass Property`**
  ```csharp
  public WoWItemContainerClass ContainerClass { get; }
  ```
  The ContainerClass of this item. eg; 'Bandage', 'Scroll', 'Flask'.

- **`DamageType Property`**
  ```csharp
  public int DamageType { get; }
  ```
  Gets the type of the damage.

- **`Description Property`**
  ```csharp
  [ObsoleteAttribute("Always returns an empty string")]
public string Description { get; }
  ```
  Gets the description.

- **`DisplayInfoId Property`**
  ```csharp
  public int DisplayInfoId { get; }
  ```
  The DisplayInfoId of this item.

- **`DPS Property`**
  ```csharp
  public float DPS { get; }
  ```
  The DPS of this item, if it's a weapon. Else 0.

- **`Effects Property`**
  ```csharp
  public ItemEffectList Effects { get; }
  ```

- **`EnhancementClass Property`**
  ```csharp
  public WoWItemEnhancementClass EnhancementClass { get; }
  ```
  The Enhancement class. e.g. 'Finger', 'Cloak', 'Shoulder'

- **`EquipSlot Property`**
  ```csharp
  public InventoryType EquipSlot { get; }
  ```
  The EquipSlot of this item. eg; 'Head', 'Shoulders', 'Back'.

- **`GemClass Property`**
  ```csharp
  public WoWItemGemClass GemClass { get; }
  ```
  The GemClass of this item. eg; 'Red', 'Blue', 'Yellow'.

- **`GlyphClass Property`**
  ```csharp
  [ObsoleteAttribute("No longer used; will be removed in the future.")]
public WoWItemGlyphClass GlyphClass { get; }
  ```
  The GlyphClass of this item. eg; 'Warrior', 'Paladin', 'Hunter'.

- **`HasEquipCooldown Property`**
  ```csharp
  public bool HasEquipCooldown { get; }
  ```
  Returns true if this ItemInfo object has equip cooldown.

- **`Icon Property`**
  ```csharp
  public string Icon { get; }
  ```

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Gets the id.

- **`InternalInfo Property`**
  ```csharp
  public ItemSparseEntry InternalInfo { get; }
  ```
  Gets the internal info.

- **`InventoryType Property`**
  ```csharp
  public InventoryType InventoryType { get; }
  ```
  The InventoryType (Slot) of this item. eg; 'Head', 'Neck', 'Shoulder'.

- **`IsAccountBound Property`**
  ```csharp
  public bool IsAccountBound { get; }
  ```
  Gets a bool that indicates whether this item is account bound or Battle.NET account bound.

- **`IsBattleNetAccountBound Property`**
  ```csharp
  public bool IsBattleNetAccountBound { get; }
  ```
  Gets a bool that indicates whether this item is Battle.NET account bound.

- **`IsCharter Property`**
  ```csharp
  public bool IsCharter { get; }
  ```
  Returns true if this ItemInfo object is a guild charter.

- **`IsConjured Property`**
  ```csharp
  public bool IsConjured { get; }
  ```
  Returns true if this ItemInfo object is a conjured item.

- **`IsCraftingReagent Property`**
  ```csharp
  public bool IsCraftingReagent { get; }
  ```
  Whether or not this item is considered a crafting reagent. (Eg; it can be put in your Reagent bank)

- **`IsDisenchantable Property`**
  ```csharp
  public bool IsDisenchantable { get; }
  ```
  Gets a value indicating whether this ItemInfo object is disenchantable.

- **`IsEnchantScroll Property`**
  ```csharp
  public bool IsEnchantScroll { get; }
  ```
  Returns true if this ItemInfo object is an 'unique-equipped' item.

- **`IsGarroshHeirloomWeapon Property`**
  ```csharp
  public bool IsGarroshHeirloomWeapon { get; }
  ```
  Gets a bool that indicates whether this item is one of the heirloom 90-100 weapons from Garrosh Hellscream.

- **`IsMillable Property`**
  ```csharp
  public bool IsMillable { get; }
  ```
  Returns true if this ItemInfo object is millable.

- **`IsOpenable Property`**
  ```csharp
  public bool IsOpenable { get; }
  ```
  Gets a bool that indicates whether this item is openable. Note that this does not take locks into account; for that, see IsOpenable.

- **`IsPvPItem Property`**
  ```csharp
  public bool IsPvPItem { get; }
  ```
  Returns true if this ItemInfo object is a pvp item.

- **`IsThrownWeapon Property`**
  ```csharp
  public bool IsThrownWeapon { get; }
  ```
  Returns true if this ItemInfo object is an thrown weapon.

- **`IsTotem Property`**
  ```csharp
  public bool IsTotem { get; }
  ```
  Returns true if this ItemInfo object is a totem.

- **`IsTournamentGear Property`**
  ```csharp
  public bool IsTournamentGear { get; }
  ```
  Gets a bool that indicates whether this item is a tournament gear item.

- **`IsUniqueEquipped Property`**
  ```csharp
  public bool IsUniqueEquipped { get; }
  ```
  Gets a bool that indicates whether this item is unique equipped.

- **`IsWand Property`**
  ```csharp
  public bool IsWand { get; }
  ```
  Returns true if this ItemInfo object is a wand.

- **`IsWeapon Property`**
  ```csharp
  public bool IsWeapon { get; }
  ```
  Gets a value indicating whether this instance is a weapon.

- **`IsWrappingPaper Property`**
  ```csharp
  public bool IsWrappingPaper { get; }
  ```
  Returns true if this ItemInfo object is wrapping paper.

- **`ItemAreaId Property`**
  ```csharp
  public int ItemAreaId { get; }
  ```
  Gets the item area id.

- **`ItemClass Property`**
  ```csharp
  public WoWItemClass ItemClass { get; }
  ```
  The itemclass of this item. eg; 'Weapon', 'Gem', 'Armor', 'Consumable'.

- **`ItemLink Property`**
  ```csharp
  public string ItemLink { get; }
  ```
  Gets an item link for this item info.

- **`ItemMapId Property`**
  ```csharp
  public int ItemMapId { get; }
  ```
  Gets the item map id.

- **`ItemSetId Property`**
  ```csharp
  public int ItemSetId { get; }
  ```
  Gets the item set id.

- **`KeyClass Property`**
  ```csharp
  public WoWItemKeyClass KeyClass { get; }
  ```
  The KeyClass of this item. None, Key or Lockpick.

- **`Level Property`**
  ```csharp
  public int Level { get; }
  ```
  The item level of this item.

- **`LockID Property`**
  ```csharp
  public int LockID { get; }
  ```
  Gets the lock ID for this item.

- **`MaxStackSize Property`**
  ```csharp
  public int MaxStackSize { get; }
  ```
  The maximum number of items a stack can contain.

- **`MiscClass Property`**
  ```csharp
  public WoWItemMiscClass MiscClass { get; }
  ```
  The MiscClass of this item. eg; 'Junk', 'Reagent', 'Pet'.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  The full name of this item.

- **`ProjectileClass Property`**
  ```csharp
  public WoWItemProjectileClass ProjectileClass { get; }
  ```
  The ProjectileClass of this item. eg; 'Wand', 'Bolt', 'Arrow'.

- **`Quality Property`**
  ```csharp
  public WoWItemQuality Quality { get; }
  ```
  The Quality of this item. eg; 'Legendary', 'Epic'.

- **`QuiverClass Property`**
  ```csharp
  public WoWItemQuiverClass QuiverClass { get; }
  ```
  The QuiverClass of this item. eg; 'AmmoPouch'.

- **`RandomSuffixId Property`**
  ```csharp
  public int RandomSuffixId { get; }
  ```
  Gets the random suffix id.

- **`ReagentClass Property`**
  ```csharp
  public WoWItemReagentClass ReagentClass { get; }
  ```
  The Reagent class. e.g. 'Reagent', 'Keystone'

- **`RecipeClass Property`**
  ```csharp
  public WoWItemRecipeClass RecipeClass { get; }
  ```
  The RecipeClass of this item. eg; 'Cooking', 'Alchemy', 'First Aid'.

- **`RequiredEnchantingLevelToDisenchant Property`**
  ```csharp
  public Nullable&lt;int&gt; RequiredEnchantingLevelToDisenchant { get; }
  ```
  Returns the required Enchanting level needed to disenchant item or null if not [IsDisenchantable].

- **`RequiredHonorRank Property`**
  ```csharp
  public int RequiredHonorRank { get; }
  ```
  The required honor rank to use this item.

- **`RequiredInscriptionLevelToMill Property`**
  ```csharp
  public Nullable&lt;int&gt; RequiredInscriptionLevelToMill { get; }
  ```
  Returns the required Inscription level needed to mill item or null if item can't be milled.

- **`RequiredJewelcraftingLevelToProspect Property`**
  ```csharp
  public Nullable&lt;int&gt; RequiredJewelcraftingLevelToProspect { get; }
  ```
  Returns the required Jewelcrafting level needed to prospect item or null if item can't be prospected.

- **`RequiredLevel Property`**
  ```csharp
  public int RequiredLevel { get; }
  ```
  The required level to use this item.

- **`RequiredReputationFactionId Property`**
  ```csharp
  public int RequiredReputationFactionId { get; }
  ```
  The required reputation faction id to use this item.

- **`RequiredReputationRank Property`**
  ```csharp
  public int RequiredReputationRank { get; }
  ```
  The requred reputation level to use this item.

- **`RequiredSkillId Property`**
  ```csharp
  public int RequiredSkillId { get; }
  ```
  The required skill id to use this item if any, 0 otherwise.

- **`RequiredSkillLevel Property`**
  ```csharp
  public int RequiredSkillLevel { get; }
  ```
  The required skill level to use this item.

- **`RequiredSpellId Property`**
  ```csharp
  public int RequiredSpellId { get; }
  ```
  The required spellid to use this item.

- **`ScalingStatDistribution Property`**
  ```csharp
  public ScalingStatDistribution ScalingStatDistribution { get; }
  ```
  Gets the scaling stat distribution entry, or null if invalid.

- **`ScalingStatDistributionID Property`**
  ```csharp
  public int ScalingStatDistributionID { get; }
  ```
  Gets the scaling stat distribution ID.

- **`SellPrice Property`**
  ```csharp
  public int SellPrice { get; }
  ```
  The total amount of copper retrieved by selling this item to a vendor.

- **`Stats Property`**
  ```csharp
  public ItemStats Stats { get; }
  ```
  Gets the stats for this item, including any suffixes/bonuses, but excluding gems/enchants.

- **`SubClassId Property`**
  ```csharp
  public int SubClassId { get; }
  ```
  Gets the sub class id.

- **`TotemCategory Property`**
  ```csharp
  public TotemCategory TotemCategory { get; }
  ```
  Gets the totem category.

- **`TotemCategoryId Property`**
  ```csharp
  public uint TotemCategoryId { get; }
  ```
  Gets the totem category ID.

- **`TradeGoodsClass Property`**
  ```csharp
  public WoWItemTradeGoodsClass TradeGoodsClass { get; }
  ```
  The TradeGoodsClass of this item. eg; 'Meat', 'Herb', 'Explosives'.

- **`TriggersSpell Property`**
  ```csharp
  public bool TriggersSpell { get; }
  ```
  Returns true if this ItemInfo triggers a spell.

- **`TypeFlags Property`**
  ```csharp
  public int TypeFlags { get; }
  ```
  Gets the type flags.

- **`UniqueCount Property`**
  ```csharp
  public int UniqueCount { get; }
  ```
  The maximum number of.

- **`WeaponClass Property`**
  ```csharp
  public WoWItemWeaponClass WeaponClass { get; }
  ```
  The WeaponClass of this item. eg; 'Bow', 'Gun', 'Mace' etc.

- **`WeaponDelay Property`**
  ```csharp
  public float WeaponDelay { get; }
  ```
  The Weapon speed in seconds of this weapon.

- **`WeaponSpeed Property`**
  ```csharp
  public int WeaponSpeed { get; }
  ```
  The WeaponDelay in milliseconds of this weapon. eg; '3000' for a staff.

#### ItemInfo Methods

- **`FromId Method`**
  ```csharp
  public static ItemInfo FromId(
uint itemId
)
  ```
  Constructs an ItemInfo instance from an item ID. Returns null if there
            is no cached information for that item.
  - *itemId*: Type: SystemAddLanguageSpecificTextSet("LST1FEF437E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32.
  - **Returns:** ReferenceItemInfo Class

- **`FromLink Method`**
  ```csharp
  public static ItemInfo FromLink(
string link
)
  ```
  Constructs an ItemInfo instance from an item link. Returns null if there is no information for that item.
  - *link*: Type: SystemAddLanguageSpecificTextSet("LSTF9B03629_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe item link.
  - **Returns:** ExceptionConditionArgumentNullExceptionThrown if link is null.ArgumentExceptionThrown if link is malformed.

- **`GetShownArmorClass Method`**
  ```csharp
  public WoWItemArmorClass GetShownArmorClass()
  ```
  Gets the shown armor class of this item.
  - **Returns:** This can depend on the level and class of the player, as some items (heirlooms) change armor class at level 40.For some items, this is 'None' even if ArmorClass is not None, for example on cloaks.

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceItemInfo Class

---

### ItemStats Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.ItemStats

```csharp
public class ItemStats
```

An item stats.

#### ItemStats Properties

- **`DPS Property`**
  ```csharp
  public float DPS { get; }
  ```
  Gets the DPS of this item. This is the same as querying the Stats dictionary for the DamagePerSecond key.

- **`Stats Property`**
  ```csharp
  public Dictionary&lt;StatType, float&gt; Stats { get; }
  ```
  Gets a dictionary of the stats for this item.

#### ItemStats Methods

- **`FromLink Method`**
  ```csharp
  public static ItemStats FromLink(
string itemLink
)
  ```
  Gets an instance of ItemStats for a specified item with a specified item link.
  - *itemLink*: Type: SystemAddLanguageSpecificTextSet("LST8314B3B6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe item link.
  - **Returns:** Remarks

---

### LocalPlayer Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWObject → Styx.WoWInternals.WoWObjects.WoWUnit → Styx.WoWInternals.WoWObjects.WoWPlayer → Styx.WoWInternals.WoWObjects.LocalPlayer

```csharp
public class LocalPlayer : WoWPlayer
```

The main local player (me)

#### LocalPlayer Properties

- **`AreaId Property`**
  ```csharp
  public uint AreaId { get; }
  ```
  Returns the current area ID.

- **`AutoRepeatingSpellId Property`**
  ```csharp
  [ObsoleteAttribute("Always returns 0")]
public int AutoRepeatingSpellId { get; }
  ```
  Gets the spellid of the current auto-repeat spell.

- **`BagItemGuids Property`**
  ```csharp
  public List&lt;WoWGuid&gt; BagItemGuids { get; }
  ```
  Gets all item GUIDs in the players 5 bags.

- **`BagItems Property`**
  ```csharp
  public List&lt;WoWItem&gt; BagItems { get; }
  ```
  Gets all items in the players 5 bags.

- **`BagsFull Property`**
  ```csharp
  public bool BagsFull { get; }
  ```
  Gets a value determining if all slots in your bags are filled with items.

- **`BankItemGuids Property`**
  ```csharp
  public IEnumerable&lt;WoWGuid&gt; BankItemGuids { get; }
  ```
  Gets all item GUIDs in the players bank.

- **`BankItems Property`**
  ```csharp
  public IEnumerable&lt;WoWItem&gt; BankItems { get; }
  ```
  Gets all items in the players banks.

- **`BattlefieldInstanceType Property`**
  ```csharp
  public int BattlefieldInstanceType { get; }
  ```
  Gets the current battlefield instance type.

- **`CanSkinLevel Property`**
  ```csharp
  public int CanSkinLevel { get; }
  ```
  Returns the maximum level of units you can skin.

- **`CarriedItemGuids Property`**
  ```csharp
  public List&lt;WoWGuid&gt; CarriedItemGuids { get; }
  ```
  Gets all carried item GUIDs; which is the players 5 bags and equipped items.

- **`CarriedItems Property`**
  ```csharp
  public List&lt;WoWItem&gt; CarriedItems { get; }
  ```
  Gets all carried items as WoWItem objects; which is the players 5 bags, equipped
            items, currency and key ring.

- **`ComboPoints Property`**
  ```csharp
  public int ComboPoints { get; }
  ```
  Returns the number of active combo points on the current target.

- **`CorpseMapId Property`**
  ```csharp
  public int CorpseMapId { get; }
  ```
  Gets the ID of the map that player's corpse is on or -1 if there is no corpse

- **`CorpsePoint Property`**
  ```csharp
  public Vector3 CorpsePoint { get; }
  ```
  Gets the location of the local player corpse.

- **`CurrentMap Property`**
  ```csharp
  public Map CurrentMap { get; }
  ```
  Returns information about the current map.

- **`CurrentPendingCursorSpell Property`**
  ```csharp
  public WoWSpell CurrentPendingCursorSpell { get; }
  ```
  Returns the current pending spell active in the cursor.

- **`CurrentRunes Property`**
  ```csharp
  public int CurrentRunes { get; }
  ```
  Gets the number of runes that are currently off cooldown.

- **`CurrentScenarioId Property`**
  ```csharp
  public uint CurrentScenarioId { get; }
  ```
  Gets the current scenario identifier.

- **`CurrentTargetGuid Property`**
  ```csharp
  public override WoWGuid CurrentTargetGuid { get; }
  ```
  Gets the current target guid.

- **`Dueler Property`**
  ```csharp
  public WoWPlayer Dueler { get; }
  ```
  Get the dueler, the player this player currently is participating in a duel with.

- **`DuelerGuid Property`**
  ```csharp
  public WoWGuid DuelerGuid { get; }
  ```
  Gets the GUID of the player this player currently is participating in a duel with.

- **`Durability Property`**
  ```csharp
  public uint Durability { get; }
  ```
  Gets the durability of all equipped items summed up together.

- **`DurabilityPercent Property`**
  ```csharp
  public double DurabilityPercent { get; }
  ```
  Gets the durability percentage as a decimal for all equipped items, where 0 means 0% and 1.0 means 100%.

- **`FocusedUnit Property`**
  ```csharp
  public WoWUnit FocusedUnit { get; }
  ```
  Gets the /focus'd unit.

- **`FocusedUnitGuid Property`**
  ```csharp
  public WoWGuid FocusedUnitGuid { get; }
  ```
  Gets a unique identifier of the currently /focus'd unit.

- **`FreeBagSlots Property`**
  ```csharp
  public uint FreeBagSlots { get; }
  ```
  Gets the number free slots in your bags.

- **`FreeNormalBagSlots Property`**
  ```csharp
  public uint FreeNormalBagSlots { get; }
  ```
  Gets the number of free normal bag slots. This does not include quivers etc.

- **`GroupInfo Property`**
  ```csharp
  public WoWGroupInfo GroupInfo { get; }
  ```
  Returns all information about the current raid or party.

- **`HearthstoneAreaId Property`**
  ```csharp
  public uint HearthstoneAreaId { get; }
  ```
  Gets the area ID for where your hearthstone is currently set.

- **`InPvpCombat Property`**
  ```csharp
  public bool InPvpCombat { get; }
  ```
  Gets a value indicating whether player is in PvP combat.

- **`InstanceCorpseLocation Property`**
  ```csharp
  public Vector3 InstanceCorpseLocation { get; }
  ```
  Gets the instance corpse location.

- **`InVehicle Property`**
  ```csharp
  public bool InVehicle { get; }
  ```
  The get reputation level with.

- **`Inventory Property`**
  ```csharp
  public WoWPlayerInventory Inventory { get; }
  ```
  Gets the players inventory.

- **`IsActuallyInCombat Property`**
  ```csharp
  public bool IsActuallyInCombat { get; }
  ```
  Returns if we are actually in combat + have a valid target. 
            This checks if player or any of player's minions are in combat and if there are 
            any mobs that are not blacklisted for combat and have player/minions in their threat table.

- **`IsAutoRepeatingSpell Property`**
  ```csharp
  [ObsoleteAttribute("Always returns false")]
public bool IsAutoRepeatingSpell { get; }
  ```
  Gets a value indicating whether the player is auto repeating a spell.

- **`IsBeingAttacked Property`**
  ```csharp
  public bool IsBeingAttacked { get; }
  ```
  Returns if player or any of player's minions are being attacked by mobs that are not blacklisted for combat.

- **`IsInArena Property`**
  ```csharp
  public bool IsInArena { get; }
  ```
  Gets a bool that indicates whether the local player is currently in an arena.

- **`IsIndoors Property`**
  ```csharp
  public override bool IsIndoors { get; }
  ```
  Returns true if the player is indoors.

- **`IsInGarrison Property`**
  ```csharp
  public bool IsInGarrison { get; }
  ```
  Gets a bool that indicates whether the local player is currently in a garrison.

- **`IsInInstance Property`**
  ```csharp
  public bool IsInInstance { get; }
  ```
  Returns true if the player is inside a dungeon.

- **`IsOutdoors Property`**
  ```csharp
  public override bool IsOutdoors { get; }
  ```
  Returns true if the player is outdoors.

- **`LowestDurabilityPercent Property`**
  ```csharp
  public double LowestDurabilityPercent { get; }
  ```
  Returns the lowest durability for all of your equipped items.

- **`MapId Property`**
  ```csharp
  public uint MapId { get; }
  ```
  Returns the current map id.

- **`MapName Property`**
  ```csharp
  public string MapName { get; }
  ```
  Gets the current map name. This is not a friendly name.

- **`MaxDurability Property`**
  ```csharp
  public uint MaxDurability { get; }
  ```
  Returns the maxdurability of all equipped items summed up together.

- **`MaxRunes Property`**
  ```csharp
  public int MaxRunes { get; }
  ```
  Gets the max amount of runes available for the player.

- **`MinimapZoneText Property`**
  ```csharp
  public string MinimapZoneText { get; }
  ```
  Returns the name of the current area (as displayed in the Minimap)

- **`Mounted Property`**
  ```csharp
  public override bool Mounted { get; }
  ```
  Returns true if the local player is mounted.

- **`Name Property`**
  ```csharp
  public override string Name { get; }
  ```

- **`NormalBagsFull Property`**
  ```csharp
  public bool NormalBagsFull { get; }
  ```
  Returns true if all normal bags are filled with items.

- **`PartyMembers Property`**
  ```csharp
  public List&lt;WoWPlayer&gt; PartyMembers { get; }
  ```
  Gets the party members.

- **`PetSpells Property`**
  ```csharp
  public List&lt;WoWPetSpell&gt; PetSpells { get; }
  ```
  Gets a list of the spells your pet or a vehicle you are in control of can use.

- **`QuestLog Property`**
  ```csharp
  public QuestLog QuestLog { get; }
  ```
  The quest log.

- **`RaidMembers Property`**
  ```csharp
  public List&lt;WoWPlayer&gt; RaidMembers { get; }
  ```
  Gets the RAID members.

- **`ReagentBankItemGuids Property`**
  ```csharp
  public IEnumerable&lt;WoWGuid&gt; ReagentBankItemGuids { get; }
  ```
  Gets the reagent bank item guids.

- **`ReagentBankItems Property`**
  ```csharp
  public IEnumerable&lt;WoWItem&gt; ReagentBankItems { get; }
  ```
  Gets the reagent bank items.

- **`RealmName Property`**
  ```csharp
  public string RealmName { get; }
  ```
  Gets the name of the realm.

- **`RealZoneText Property`**
  ```csharp
  public string RealZoneText { get; }
  ```
  Returns the "official" name of the zone or instance in which the player is located.

- **`ResearchSiteIds Property`**
  ```csharp
  public List&lt;uint&gt; ResearchSiteIds { get; }
  ```
  Gets a list of IDs of the research sites this character is working on. (These are
            entries into ResearchSite.dbc).

- **`Role Property`**
  ```csharp
  public WoWPartyMember.GroupRole Role { get; }
  ```
  The role of the local player when in a group. Tank, Healer, Damage etc.

- **`Stable Property`**
  ```csharp
  public Stable Stable { get; }
  ```
  Gets Stable.

- **`SubZoneId Property`**
  ```csharp
  public uint SubZoneId { get; }
  ```
  Gets the current sub zone ID.

- **`SubZoneText Property`**
  ```csharp
  public string SubZoneText { get; }
  ```
  Returns the name of the minor area in which the player is located.

- **`Totems Property`**
  ```csharp
  public List&lt;WoWTotemInfo&gt; Totems { get; }
  ```
  Gets the totems for the local player. This will return 5 entries in total, regardless
            if you have 5 totems actually out. This can be indexed using WoWTotemType - 1 to obtain info for specific type.

- **`ZoneId Property`**
  ```csharp
  public uint ZoneId { get; }
  ```
  returns the local players ZoneId.

- **`ZoneText Property`**
  ```csharp
  public string ZoneText { get; }
  ```
  Returns the name of the zone in which the player is located.

#### LocalPlayer Methods

- **`CanDualWield Method`**
  ```csharp
  public bool CanDualWield()
  ```
  Figures out whether the local player can dual wield or not.
  - **Returns:** ReferenceLocalPlayer Class

- **`CanEquipItem Method`**
  Finds out if an item can be used or equipped.

- **`CanEquipItem Method (ItemInfo)`**
  ```csharp
  public bool CanEquipItem(
ItemInfo itemInfo
)
  ```
  Finds out if an item can be used or equipped.
  - *itemInfo*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTA2AD1FC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ItemInfo.
  - **Returns:** ReferenceLocalPlayer Class

- **`CanEquipItem Method (WoWItem)`**
  ```csharp
  public bool CanEquipItem(
WoWItem item
)
  ```
  Finds out if an item can be used or equipped.
  - *item*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST12F395ED_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWItem.
  - **Returns:** ReferenceLocalPlayer Class

- **`CanEquipItem Method (ItemInfo, GameError)`**
  ```csharp
  public bool CanEquipItem(
ItemInfo itemInfo,
out GameError reason
)
  ```
  Finds out if an item can be used or equipped.
  - *itemInfo*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST8B5C852C_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ItemInfo.
  - *reason*: Type: StyxAddLanguageSpecificTextSet("LST8B5C852C_3?cs=.|vb=.|cpp=::|nu=.|fs=.");GameErrorAddLanguageSpecificTextSet("LST8B5C852C_4?cpp=%");  [out].
  - **Returns:** ReferenceLocalPlayer Class

- **`CanEquipItem Method (WoWItem, GameError)`**
  ```csharp
  public bool CanEquipItem(
WoWItem item,
out GameError reason
)
  ```
  Finds out if an item can be used or equipped.
  - *item*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST7B5A95C7_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWItem  .
  - *reason*: Type: StyxAddLanguageSpecificTextSet("LST7B5A95C7_3?cs=.|vb=.|cpp=::|nu=.|fs=.");GameErrorAddLanguageSpecificTextSet("LST7B5A95C7_4?cpp=%");[out].
  - **Returns:** ExceptionConditionArgumentNullExceptionThrown if item is null.

- **`CanUseItem Method`**
  Checks if an item can be used.

- **`CanUseItem Method (UInt32, GameError)`**
  ```csharp
  public bool CanUseItem(
uint itemId,
out GameError reason
)
  ```
  Checks if an item can be used.
  - *itemId*: Type: SystemAddLanguageSpecificTextSet("LST1928021C_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The item id.
  - *reason*: Type: StyxAddLanguageSpecificTextSet("LST1928021C_3?cs=.|vb=.|cpp=::|nu=.|fs=.");GameErrorAddLanguageSpecificTextSet("LST1928021C_4?cpp=%");[out] The reason.
  - **Returns:** ExceptionConditionInvalidOperationExceptionThrown if this LocalPlayer instance is
            invalid.

- **`CanUseItem Method (ItemInfo, GameError)`**
  ```csharp
  public bool CanUseItem(
ItemInfo itemInfo,
out GameError reason
)
  ```
  Checks if an item can be used.
  - *itemInfo*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST9FB9E25_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ItemInfoThe item info.
  - *reason*: Type: StyxAddLanguageSpecificTextSet("LST9FB9E25_3?cs=.|vb=.|cpp=::|nu=.|fs=.");GameErrorAddLanguageSpecificTextSet("LST9FB9E25_4?cpp=%");The reason that it cannot be used, if it cannot be used.
  - **Returns:** See Also

- **`ClearTarget Method`**
  ```csharp
  public void ClearTarget()
  ```
  Clear's the current target.

- **`GetBag Method`**
  ```csharp
  public WoWContainer GetBag(
WoWBagSlot slot
)
  ```
  Returns a WoWContainer object representing the requested bag, or null
            if there is no bag in the slot.
  - *slot*: Type: StyxAddLanguageSpecificTextSet("LST87C47DE9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWBagSlot.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetBagAtIndex Method`**
  ```csharp
  public WoWContainer GetBagAtIndex(
uint index
)
  ```
  Returns the bag at the specified index or null if there is no bag at the index.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST95A54EA9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32.
  - **Returns:** ExceptionConditionArgumentOutOfRangeExceptionThrown when one or more arguments are outside the
            required range.

- **`GetBagGuidAtIndex Method`**
  ```csharp
  public WoWGuid GetBagGuidAtIndex(
uint index
)
  ```
  Gets the bag GUID at the specified index.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST45BDEC2E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32.
  - **Returns:** ExceptionConditionArgumentOutOfRangeExceptionThrown when one or more arguments are outside the
            required range.

- **`GetBankItemCount Method`**
  ```csharp
  public int GetBankItemCount(
uint itemId
)
  ```
  Returns number items with a matching id that player has in personal bank
  - *itemId*: Type: SystemAddLanguageSpecificTextSet("LST67B143DE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceLocalPlayer Class

- **`GetCarriedItemCount Method`**
  ```csharp
  public int GetCarriedItemCount(
uint itemId
)
  ```
  Gets amount of items carried with a specific item ID.
  - *itemId*: Type: SystemAddLanguageSpecificTextSet("LST47A5A9F8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The item ID to check.
  - **Returns:** This function also includes quest items that are not shown in the bags.

- **`GetCurrentArea Method`**
  ```csharp
  public AreaTable GetCurrentArea()
  ```
  Gets the current area.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetEstimatedRepairCost Method`**
  ```csharp
  public WoWPrice GetEstimatedRepairCost()
  ```
  Returns the average repair cost for all carried items.
            Use with care as this does a lot of lookups.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetFactionStanding Method`**
  The get faction standing.

- **`GetFactionStanding Method (UInt32, FactionStanding)`**
  ```csharp
  public bool GetFactionStanding(
uint factionId,
out FactionStanding standing
)
  ```
  The get faction standing.
  - *factionId*: Type: SystemAddLanguageSpecificTextSet("LST3FDBA806_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The faction id.
  - *standing*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST3FDBA806_3?cs=.|vb=.|cpp=::|nu=.|fs=.");FactionStandingAddLanguageSpecificTextSet("LST3FDBA806_4?cpp=%"); [out] The standing.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetFactionStanding Method (Faction, FactionStanding)`**
  ```csharp
  public bool GetFactionStanding(
Faction faction,
out FactionStanding standing
)
  ```
  The get faction standing.
  - *faction*: Type: Styx.WoWInternals.DBCAddLanguageSpecificTextSet("LSTF92B6FA_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Faction The faction.
  - *standing*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTF92B6FA_3?cs=.|vb=.|cpp=::|nu=.|fs=.");FactionStandingAddLanguageSpecificTextSet("LSTF92B6FA_4?cpp=%");[out] The standing.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetFactionStanding Method (FactionId, FactionStanding)`**
  ```csharp
  public bool GetFactionStanding(
FactionId factionId,
out FactionStanding standing
)
  ```
  The get faction standing.
  - *factionId*: Type: StyxAddLanguageSpecificTextSet("LST15A3EF81_2?cs=.|vb=.|cpp=::|nu=.|fs=.");FactionIdThe faction id.
  - *standing*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST15A3EF81_3?cs=.|vb=.|cpp=::|nu=.|fs=.");FactionStandingAddLanguageSpecificTextSet("LST15A3EF81_4?cpp=%"); [out] The standing.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetInBagItemCount Method`**
  ```csharp
  public int GetInBagItemCount(
uint itemId
)
  ```
  Returns number items with a matching id that player is carrying in bags, not including the quest log item bag
  - *itemId*: Type: SystemAddLanguageSpecificTextSet("LST5E27863B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceLocalPlayer Class

- **`GetIngredientCount Method`**
  ```csharp
  public int GetIngredientCount(
uint itemId
)
  ```
  Gets the number of ingredients that the player has in his/her possession, items in bags and in personal and reagent
                banks
  - *itemId*: Type: SystemAddLanguageSpecificTextSet("LST43D291B0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The item identifier.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetLearnedTalent Method`**
  ```csharp
  public TalentPlacement GetLearnedTalent(
int tier
)
  ```
  Gets the talent learned in the specified 0-based tier.
  - *tier*: Type: SystemAddLanguageSpecificTextSet("LSTC3E515DC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The tier. 0 means level 15 talent, 1 means level 30 talent and so on. Range: [0; NumTalents - 1]
  - **Returns:** Exceptions

- **`GetLearnedTalents Method`**
  ```csharp
  public TalentPlacementSet GetLearnedTalents()
  ```
  Gets a TalentPlacementSet with the currently learned talents.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetMirrorTimerInfo Method`**
  ```csharp
  public MirrorTimerInfo GetMirrorTimerInfo(
MirrorTimerType type
)
  ```
  Returns detailed information on a mirror timer, eg; 'Feign Death', 'Breath' etc.
  - *type*: Type: StyxAddLanguageSpecificTextSet("LST72C8BB4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");MirrorTimerTypeThe type.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetPartyMemberGuid Method`**
  ```csharp
  public WoWGuid GetPartyMemberGuid(
int index
)
  ```
  Gets the Guid of a party member. Index must be 0, 1, 2 or 3.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST3D046712_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetRaidMember Method`**
  ```csharp
  public WoWPlayer GetRaidMember(
int index
)
  ```
  Gets a raid member by index.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST4BC9DF19_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32the index.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetRaidMemberGuid Method`**
  ```csharp
  public WoWGuid GetRaidMemberGuid(
int index
)
  ```
  Gets the Guid of a raid member. Index must be from 0 to and including 39.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST1CB71228_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32the index.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetReagentBankItemCount Method`**
  ```csharp
  public int GetReagentBankItemCount(
uint itemId
)
  ```
  Returns number items with a matching id that player has in personal bank
  - *itemId*: Type: SystemAddLanguageSpecificTextSet("LSTAB43DF2A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceLocalPlayer Class

- **`GetReputationLevelWith Method`**
  ```csharp
  public WoWUnitReaction GetReputationLevelWith(
uint factionId
)
  ```
  The get reputation level with.
  - *factionId*: Type: SystemAddLanguageSpecificTextSet("LSTA277DA5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The faction id.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetReputationWith Method`**
  ```csharp
  public int GetReputationWith(
uint toFactionId
)
  ```
  The get reputation with.
  - *toFactionId*: Type: SystemAddLanguageSpecificTextSet("LSTD3478167_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The to faction id.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetRuneCooldownTimeLeft Method`**
  ```csharp
  public TimeSpan GetRuneCooldownTimeLeft(
int index
)
  ```
  Gets the cooldown time left on the specified rune.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTA6A21972_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The index.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetSkill Method`**
  Returns detailed information on a skill, eg; 'Herbalism', 'Mining' etc.

- **`GetSkill Method (Int32)`**
  ```csharp
  public WoWSkill GetSkill(
int id
)
  ```
  Returns detailed information on a skill, eg; 'Herbalism', 'Mining' etc.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTACCB3D98_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The id.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetSkill Method (String)`**
  ```csharp
  public WoWSkill GetSkill(
string name
)
  ```
  Returns detailed information on a skill, eg; 'Herbalism', 'Mining' etc.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LST84A7AC05_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetSkill Method (UInt32)`**
  ```csharp
  public WoWSkill GetSkill(
uint index
)
  ```
  Returns detailed information on a skill, eg; 'Herbalism', 'Mining' etc.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTDE911553_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The index.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetSkill Method (SkillLine)`**
  ```csharp
  public WoWSkill GetSkill(
SkillLine skill
)
  ```
  Returns detailed information on a skill, eg; 'Herbalism', 'Mining' etc.
  - *skill*: Type: StyxAddLanguageSpecificTextSet("LSTF99152C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SkillLineThe skill.
  - **Returns:** ReferenceLocalPlayer Class

- **`GetTotemBarSpells Method`**
  ```csharp
  public List&lt;WoWSpell&gt; GetTotemBarSpells(
int totemBarSlot
)
  ```
  Gets the spells in a specified slot of the totem bar. (Valid values are 0-3)
  - *totemBarSlot*: Type: SystemAddLanguageSpecificTextSet("LST56CE1882_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The totem bar slot.
  - **Returns:** Exceptions

- **`HasPendingSpell Method`**
  Checks if this spell is pending in the cursor, eg; 'Blizzard', 'Flamestrike',
            'Skinning' etc.

- **`HasPendingSpell Method (Int32)`**
  ```csharp
  public bool HasPendingSpell(
int spellId
)
  ```
  Checks if this spell is pending in the cursor, eg; 'Blizzard', 'Flamestrike',
            'Skinning' etc.
  - *spellId*: Type: SystemAddLanguageSpecificTextSet("LST1204C224_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The spellid.
  - **Returns:** ReferenceLocalPlayer Class

- **`HasPendingSpell Method (String)`**
  ```csharp
  public bool HasPendingSpell(
string name
)
  ```
  Checks if this spell is pending in the cursor, eg; 'Blizzard', 'Flamestrike',
            'Skinning' etc.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LST6146CD3D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe spell name.
  - **Returns:** ReferenceLocalPlayer Class

- **`HasPendingSpell Method (WoWSpell)`**
  ```csharp
  public bool HasPendingSpell(
WoWSpell spell
)
  ```
  Checks if this spell is pending in the cursor, eg; 'Blizzard', 'Flamestrike',
            'Skinning' etc.
  - *spell*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST3609E8F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWSpellThe WoWSpell object.
  - **Returns:** ReferenceLocalPlayer Class

- **`IsBehind Method`**
  Returns a boolean indicating whether this object is behind an other object. This
            should not be used unless you know what you are doing. Use IsSafelyBehind instead.

- **`IsBehind Method (WoWUnit)`**
  ```csharp
  public bool IsBehind(
WoWUnit unit
)
  ```
  Returns true if you are behind a unit.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST6FDBDD1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnit.
  - **Returns:** ReferenceLocalPlayer Class

- **`LfgMode Method`**
  ```csharp
  public LfgState LfgMode(
LfgCategory category,
Nullable&lt;int&gt; lfgId = null
)
  ```
  Current looking for dungeon state
  - *category*: Type: StyxAddLanguageSpecificTextSet("LST26366A84_1?cs=.|vb=.|cpp=::|nu=.|fs=.");LfgCategoryThe category
  - **Returns:** ReferenceLocalPlayer Class

- **`MatchesCondition Method`**
  ```csharp
  public bool MatchesCondition(
uint playerConditionId
)
  ```
  Various game components have extra conditions attached to them (such as some WoWGameObjects)and this function determines whether the condition whose ID is equal to playerConditionId has been met.playerConditionId is a row ID into the PlayerCondition.dbc.
  - *playerConditionId*: Type: SystemAddLanguageSpecificTextSet("LST4492E68C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The player condition identifier.
  - **Returns:** ReferenceLocalPlayer Class

- **`SetFacing Method`**
  Sets your facing.

- **`SetFacing Method (Vector3)`**
  ```csharp
  public void SetFacing(
Vector3 otherLoc
)
  ```
  Sets your facing.
  - *otherLoc*: Type: System.NumericsAddLanguageSpecificTextSet("LSTD39142F9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The other Loc.

- **`SetFacing Method (Single)`**
  ```csharp
  public void SetFacing(
float facingInRadians
)
  ```
  Sets your facing.
  - *facingInRadians*: Type: SystemAddLanguageSpecificTextSet("LSTD0A1AB73_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe facing In Radians.

- **`SetFacing Method (WoWGameObject)`**
  ```csharp
  public void SetFacing(
WoWGameObject obj
)
  ```
  Sets your facing.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST3D65DBA6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGameObjectThe obj.

- **`SetFacing Method (WoWUnit)`**
  ```csharp
  public void SetFacing(
WoWUnit obj
)
  ```
  Sets your facing.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST43FC8C59_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe obj.

- **`SetFocus Method`**
  Sets the local player's focus to a specific GUID.

- **`SetFocus Method (WoWGuid)`**
  ```csharp
  public bool SetFocus(
WoWGuid guid
)
  ```
  Sets the local player's focus to a specific GUID.
  - *guid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST279BE14F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidThe guid of the object to set focus to.
  - **Returns:** ExceptionConditionArgumentNullException.

- **`SetFocus Method (WoWObject)`**
  ```csharp
  public bool SetFocus(
WoWObject obj
)
  ```
  The set focus.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST4655B540_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe obj.
  - **Returns:** ExceptionConditionArgumentNullException.

- **`TargetLastTarget Method`**
  ```csharp
  public void TargetLastTarget()
  ```
  Target's the last target.

- **`ToggleAttack Method`**
  ```csharp
  public void ToggleAttack()
  ```
  Toggle's attack.

---

### MirrorTimerInfo Structure

```csharp
public struct MirrorTimerInfo
```

The mirror timer info.

#### MirrorTimerInfo Properties

- **`CurrentTime Property`**
  ```csharp
  public uint CurrentTime { get; }
  ```
  Gets the current value of this timer.

- **`IsVisible Property`**
  ```csharp
  public bool IsVisible { get; }
  ```
  Gets a value indicating whether IsVisible.

#### MirrorTimerInfo Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  The to string.
  - **Returns:** ReferenceMirrorTimerInfo Structure

#### MirrorTimerInfo Fields

- **`ChangePerMillisecond Field`**
  ```csharp
  public int ChangePerMillisecond
  ```
  The change of the value of the timer per millisecond.

- **`InitialValue Field`**
  ```csharp
  public int InitialValue
  ```
  The initial value of the timer.

- **`MaxValue Field`**
  ```csharp
  public int MaxValue
  ```
  The max value of the timer.

- **`SpellID Field`**
  ```csharp
  public uint SpellID
  ```
  The spell id.

- **`StartTime Field`**
  ```csharp
  public uint StartTime
  ```
  The start time.

- **`Type Field`**
  ```csharp
  public MirrorTimerType Type
  ```
  The type.

---

### ObjectInvalidateDelegate Delegate

```csharp
public delegate void ObjectInvalidateDelegate()
```

Delegate used when invalidating objects.

---

### RaidTargetMarker Enumeration

```csharp
public enum RaidTargetMarker
```

Values that represent RaidTargetMarker.

---

### ReputationFlags Enumeration

```csharp
[FlagsAttribute]
public enum ReputationFlags
```

The reputation flags.

---

### SpecType Enumeration

```csharp
public enum SpecType
```

The spec type.

---

### UnitThreatInfo Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.UnitThreatInfo

```csharp
public class UnitThreatInfo
```

Information about the unit threat.

#### UnitThreatInfo Properties

- **`RawPercent Property`**
  ```csharp
  public byte RawPercent { get; }
  ```
  The percent of threat. The unit takes aggro when this value reaches 110 or 130, depending on the distance.

- **`ScaledPercent Property`**
  ```csharp
  public float ScaledPercent { get; }
  ```
  A percentage representing the unit's threat against the mob, scaled so that 100.0f is the point where the unit takes aggro.

- **`TargetGuid Property`**
  ```csharp
  public WoWGuid TargetGuid { get; }
  ```
  The GUID of this units primary target (the one that currently has aggro).

- **`ThreatStatus Property`**
  ```csharp
  public ThreatStatus ThreatStatus { get; }
  ```
  Gets the threat status.

- **`ThreatValue Property`**
  ```csharp
  public uint ThreatValue { get; }
  ```
  Gets the threat value.

#### UnitThreatInfo Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents this instance.
  - **Returns:** ReferenceUnitThreatInfo Class

---

### WoWAnimatedSubObject Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWSubObject → Styx.WoWInternals.WoWObjects.WoWAnimatedSubObject → Styx.WoWInternals.WoWObjects.SubObjects.WoWGarrisonShipment → Styx.WoWInternals.WoWObjects.WoWDoor → Styx.WoWInternals.WoWObjects.WoWFishingBobber

```csharp
public class WoWAnimatedSubObject : WoWSubObject
```

A WoW animated sub object.

#### WoWAnimatedSubObject Properties

- **`AnimationState Property`**
  ```csharp
  public byte AnimationState { get; }
  ```
  Gets the state of the animation.

---

### WoWAreaTrigger Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWObject → Styx.WoWInternals.WoWObjects.WoWAreaTrigger

```csharp
public class WoWAreaTrigger : WoWObject
```

A WoW area trigger.

#### WoWAreaTrigger Properties

- **`AreaTriggerFlags Property`**
  ```csharp
  public AreaTriggerFlags AreaTriggerFlags { get; }
  ```
  Gets the area trigger flags.

- **`Caster Property`**
  ```csharp
  public WoWObject Caster { get; }
  ```
  The caster of this trigger.

- **`CasterGuid Property`**
  ```csharp
  public WoWGuid CasterGuid { get; }
  ```
  The GUID of the object or unit that casted this trigger.

- **`Duration Property`**
  ```csharp
  public TimeSpan Duration { get; }
  ```
  The length of time in which this trigger is considered valid.

- **`Location Property`**
  ```csharp
  public override Vector3 Location { get; }
  ```

- **`MovementInfo Property`**
  ```csharp
  public WoWSimpleMovementInfo MovementInfo { get; }
  ```
  Gets the movement info for this area trigger.

- **`Name Property`**
  ```csharp
  public override string Name { get; }
  ```
  Gets the name.

- **`Rotation Property`**
  ```csharp
  public override float Rotation { get; }
  ```
  The rotation of this area trigger.

- **`Shape Property`**
  ```csharp
  public AreaTriggerShape Shape { get; }
  ```
  Gets the shape.

- **`ShapeType Property`**
  ```csharp
  public AreaTriggerShapeType ShapeType { get; }
  ```
  Gets the type of the shape.

- **`Spell Property`**
  ```csharp
  public WoWSpell Spell { get; }
  ```
  The spell that represents this trigger.

- **`SpellId Property`**
  ```csharp
  public int SpellId { get; }
  ```
  The spell ID that represents this trigger.

- **`TimeLeft Property`**
  ```csharp
  public TimeSpan TimeLeft { get; }
  ```
  The time left for this trigger to expire.

---

### WoWArenaTeamInfo Structure

```csharp
public struct WoWArenaTeamInfo
```

Information about the WoW arena team.

#### WoWArenaTeamInfo Fields

- **`GamesThisSeason Field`**
  ```csharp
  public uint GamesThisSeason
  ```
  The games this season.

- **`GamesThisWeek Field`**
  ```csharp
  public uint GamesThisWeek
  ```
  The games this week.

- **`Id Field`**
  ```csharp
  public uint Id
  ```
  The identifier.

- **`Member Field`**
  ```csharp
  public uint Member
  ```
  The member.

- **`PersonalRating Field`**
  ```csharp
  public uint PersonalRating
  ```
  The personal rating.

- **`SeasonWins Field`**
  ```csharp
  public uint SeasonWins
  ```
  The season wins.

- **`Type Field`**
  ```csharp
  public uint Type
  ```
  The type.

---

### WoWChair Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWSubObject → Styx.WoWInternals.WoWObjects.WoWChair

```csharp
public class WoWChair : WoWSubObject
```

A WoW chair.

#### WoWChair Properties

- **`ChairSlots Property`**
  ```csharp
  public int ChairSlots { get; }
  ```
  The number of slots that can be sit in.

- **`SlotPositions Property`**
  ```csharp
  public Vector3[] SlotPositions { get; }
  ```
  Gets the positions where a unit can sit in this chair.

---

### WoWContainer Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWObject → Styx.WoWInternals.WoWObjects.WoWItem → Styx.WoWInternals.WoWObjects.WoWContainer

```csharp
public class WoWContainer : WoWItem
```

A WoW container.

#### WoWContainer Properties

- **`Bag Property`**
  ```csharp
  public WoWBag Bag { get; }
  ```
  Gets the native bag this container uses.

- **`BagIndex Property`**
  ```csharp
  public int BagIndex { get; }
  ```
  Gets the index of this bag.

- **`BagType Property`**
  ```csharp
  public BagType BagType { get; }
  ```
  Return the type of the bag. (i.e Mining Bag, Herb Bag etc.)

- **`FreeSlots Property`**
  ```csharp
  public uint FreeSlots { get; }
  ```
  Gets the number of free slots in this bag.

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
  Gets the number of slots this bag has.

- **`SlotsFromDescriptor Property`**
  ```csharp
  public uint SlotsFromDescriptor { get; }
  ```
  Gets the number of slots.

- **`UsedSlots Property`**
  ```csharp
  public uint UsedSlots { get; }
  ```
  Gets the number of used slots in this bag.

#### WoWContainer Methods

- **`GetItemBySlot Method`**
  ```csharp
  public WoWItem GetItemBySlot(
uint slot
)
  ```
  Return an item by slot. The max slot is Slots - 1. If the returned value is null, the
            slot is empty.
  - *slot*: Type: SystemAddLanguageSpecificTextSet("LST4557BA20_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32.
  - **Returns:** ReferenceWoWContainer Class

- **`GetItemGuidBySlot Method`**
  ```csharp
  public WoWGuid GetItemGuidBySlot(
uint slot
)
  ```
  Returns an items GUID by slot. The max slot is Slots - 1. If the returned value is 0,
            the slot is empty.
  - *slot*: Type: SystemAddLanguageSpecificTextSet("LSTD6F87265_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32.
  - **Returns:** ReferenceWoWContainer Class

#### WoWContainer Type Conversions

- **`Implicit Conversion (WoWContainer to WoWBag)`**
  ```csharp
  public static implicit operator WoWBag (
WoWContainer container
)
  ```
  WoWBag casting operator.
  - *container*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST55B40437_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWContainerThe container.
  - **Returns:** ReferenceWoWContainer Class

---

### WoWCorpse Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWObject → Styx.WoWInternals.WoWObjects.WoWCorpse

```csharp
public class WoWCorpse : WoWObject
```

Resprents an ingame corpse, bones only etc. A player becomes a corpse after he/she
            releases from the body.

#### WoWCorpse Properties

- **`DisplayId Property`**
  ```csharp
  public uint DisplayId { get; }
  ```
  Gets the display id.

- **`DynamicFlags Property`**
  ```csharp
  public uint DynamicFlags { get; }
  ```
  Gets the dynamic flags.

- **`IsOnlyBones Property`**
  ```csharp
  public bool IsOnlyBones { get; }
  ```
  Gets a value indicating whether this instance is only bones.

- **`Lootable Property`**
  ```csharp
  public bool Lootable { get; }
  ```
  Gets a value indicating whether this WoWCorpse is lootable.

- **`Owner Property`**
  ```csharp
  public ulong Owner { get; }
  ```
  Gets the owner.

- **`Party Property`**
  ```csharp
  public ulong Party { get; }
  ```
  Gets the party.

---

### WoWDoor Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWSubObject → Styx.WoWInternals.WoWObjects.WoWAnimatedSubObject → Styx.WoWInternals.WoWObjects.WoWDoor

```csharp
public class WoWDoor : WoWAnimatedSubObject
```

A WoW door.

#### WoWDoor Properties

- **`IsClosed Property`**
  ```csharp
  public bool IsClosed { get; }
  ```
  Gets a value indicating whether this WoWDoor is closed.

- **`IsOpen Property`**
  ```csharp
  public bool IsOpen { get; }
  ```
  Gets a value indicating whether this WoWDoor is open.

#### WoWDoor Methods

- **`CanOpenNow Method`**
  Determines whether this WoWDoor [can be opened now].

- **`CanOpenNow Method`**
  ```csharp
  public bool CanOpenNow()
  ```
  Determines whether this WoWDoor [can be opened now].
  - **Returns:** Exceptions

- **`CanOpenNow Method (UInt32)`**
  ```csharp
  public bool CanOpenNow(
out uint reason
)
  ```
  Determines whether this WoWDoor [can open now] the specified reason.
  - *reason*: Type: SystemAddLanguageSpecificTextSet("LST9D06CB41_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32AddLanguageSpecificTextSet("LST9D06CB41_3?cpp=%");[out] The reason.
  - **Returns:** Exceptions

---

### WoWDynamicObject Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWObject → Styx.WoWInternals.WoWObjects.WoWDynamicObject

```csharp
public class WoWDynamicObject : WoWObject
```

A dynamic object within WoW. Usually spells placed on the ground, and similar effects.

#### WoWDynamicObject Properties

- **`Caster Property`**
  ```csharp
  public WoWUnit Caster { get; }
  ```
  Gets the caster who created this dynamic object. (The unit who cast the spell)

- **`CasterGuid Property`**
  ```csharp
  public WoWGuid CasterGuid { get; }
  ```
  Gets a unique identifier of the caster who created this dynamic object.

- **`CastTime Property`**
  ```csharp
  public uint CastTime { get; }
  ```
  Gets the time of the cast. This is a server time. (TimeNow - ServerStartTime) Use at
            your own risk, as determining server uptime is quite difficult!

- **`Radius Property`**
  ```csharp
  public float Radius { get; }
  ```
  Gets the radius of this dynamic object. (How far from Location this object extends
            to.)

- **`Spell Property`**
  ```csharp
  public WoWSpell Spell { get; }
  ```
  Gets the spell that 'created' this dynamic object.

- **`SpellId Property`**
  ```csharp
  public int SpellId { get; }
  ```
  Gets the spell ID that 'created' this dynamic object.

---

### WoWFishingBobber Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWSubObject → Styx.WoWInternals.WoWObjects.WoWAnimatedSubObject → Styx.WoWInternals.WoWObjects.WoWFishingBobber

```csharp
public class WoWFishingBobber : WoWAnimatedSubObject
```

A WoW fishing bobber.

#### WoWFishingBobber Properties

- **`IsBobbing Property`**
  ```csharp
  public bool IsBobbing { get; }
  ```
  8; } }

---

### WoWGameObject Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWObject → Styx.WoWInternals.WoWObjects.WoWGameObject

```csharp
public class WoWGameObject : WoWObject,
IComparable&lt;WoWUnit&gt;, IComparable&lt;WoWGameObject&gt;, IComparer&lt;WoWUnit&gt;,
IComparer&lt;WoWGameObject&gt;, ILootableObject
```

A WoW game object.

#### WoWGameObject Properties

- **`AnimationProgress Property`**
  ```csharp
  public byte AnimationProgress { get; }
  ```
  Gets the animation progress.

- **`ArtKit Property`**
  ```csharp
  public byte ArtKit { get; }
  ```
  Gets the art kit.

- **`Bytes1 Property`**
  ```csharp
  public byte[] Bytes1 { get; }
  ```
  Gets the bytes 1.

- **`CanLoot Property`**
  ```csharp
  public bool CanLoot { get; }
  ```
  Returns true if you can loot this gameobject, works for herbs minerals and chests.

- **`CreatedBy Property`**
  ```csharp
  public WoWUnit CreatedBy { get; }
  ```
  Gets the amount to created by.

- **`CreatedByGuid Property`**
  ```csharp
  public WoWGuid CreatedByGuid { get; }
  ```
  Gets a unique identifier of the created by.

- **`DisplayId Property`**
  ```csharp
  public uint DisplayId { get; }
  ```
  Gets the identifier of the display.

- **`FactionTemplate Property`**
  ```csharp
  public FactionTemplate FactionTemplate { get; }
  ```
  Gets the faction template.

- **`FactionTemplateId Property`**
  ```csharp
  public uint FactionTemplateId { get; }
  ```
  Gets the identifier of the faction template.

- **`FlagsUint Property`**
  ```csharp
  public uint FlagsUint { get; }
  ```
  Gets the flags uint.

- **`InteractRange Property`**
  ```csharp
  public override float InteractRange { get; }
  ```
  The InteractRange of this object.

- **`InUse Property`**
  ```csharp
  public bool InUse { get; }
  ```
  Gets a value indicating whether the in use.

- **`IsChest Property`**
  ```csharp
  public bool IsChest { get; }
  ```
  Gets a value indicating whether this object is chest.

- **`IsHerb Property`**
  ```csharp
  public bool IsHerb { get; }
  ```
  Gets a value indicating whether this object is herb.

- **`IsMineral Property`**
  ```csharp
  public bool IsMineral { get; }
  ```
  Gets a value indicating whether this object is mineral.

- **`Level Property`**
  ```csharp
  public int Level { get; }
  ```
  Gets the level.

- **`Location Property`**
  ```csharp
  public override Vector3 Location { get; }
  ```

- **`Locked Property`**
  ```csharp
  public bool Locked { get; }
  ```
  Gets a value indicating whether this object is locked.

- **`LockType Property`**
  ```csharp
  public WoWLockType LockType { get; }
  ```
  Gets the type of the lock.

- **`ModelFileId Property`**
  ```csharp
  public int ModelFileId { get; }
  ```

- **`MovementInfo Property`**
  ```csharp
  public WoWSimpleMovementInfo MovementInfo { get; }
  ```
  Gets the movement info for this game object.

- **`ParentRotation Property`**
  ```csharp
  public float ParentRotation { get; }
  ```
  Gets the parent rotation.

- **`PlayerConditionId Property`**
  ```csharp
  public uint PlayerConditionId { get; }
  ```
  Gets the player condition identifier; MatchesCondition(UInt32).

- **`RelativeLocation Property`**
  ```csharp
  public override Vector3 RelativeLocation { get; }
  ```
  Gets the relative location.

- **`RequiredSkill Property`**
  ```csharp
  public Nullable&lt;uint&gt; RequiredSkill { get; }
  ```
  Returns the required skill level to interact with this gameobject.

- **`Rotation Property`**
  ```csharp
  public override float Rotation { get; }
  ```
  Gets the rotation of this game object.

- **`SpellFocus Property`**
  ```csharp
  public WoWSpellFocus SpellFocus { get; }
  ```
  The type of spell focus this gameobject is.

- **`State Property`**
  ```csharp
  public WoWGameObjectState State { get; }
  ```
  Gets the state.

- **`SubObj Property`**
  ```csharp
  public WoWSubObject SubObj { get; }
  ```
  Gets the sub object.

- **`SubType Property`**
  ```csharp
  public WoWGameObjectType SubType { get; }
  ```
  Gets the type of the sub.

- **`Transport Property`**
  ```csharp
  public bool Transport { get; }
  ```
  Gets a value indicating whether the transport.

- **`Triggered Property`**
  ```csharp
  public bool Triggered { get; }
  ```
  Gets a value indicating whether the triggered.

- **`WithinInteractRange Property`**
  ```csharp
  public override bool WithinInteractRange { get; }
  ```
  Returns true if you are within interact range of this object. For game objects, this
            is not necessarily the same as being within InteractRange, because this takes into account
            the model of game objects.

#### WoWGameObject Methods

- **`CanUse Method`**
  ```csharp
  public bool CanUse()
  ```
  Returns a boolean indicating whether this game object can be used. This checks for
            stuff like object reaction.
  - **Returns:** ReferenceWoWGameObject Class

- **`CanUseNow Method`**
  Returns a boolean indicating whether this game object can be used right now. This
            checks for stuff like distance, too low level for herbalism/mining etc.

- **`CanUseNow Method`**
  ```csharp
  public bool CanUseNow()
  ```
  Returns a boolean indicating whether this game object can be used right now. This
            checks for stuff like distance, too low level for herbalism/mining etc.
  - **Returns:** ReferenceWoWGameObject Class

- **`CanUseNow Method (GameError)`**
  ```csharp
  public bool CanUseNow(
out GameError reason
)
  ```
  Returns a boolean indicating whether this game object can be used right now. If the
            return value is false, a out parameter gives the reason.
  - *reason*: Type: StyxAddLanguageSpecificTextSet("LST19C0DAC1_2?cs=.|vb=.|cpp=::|nu=.|fs=.");GameErrorAddLanguageSpecificTextSet("LST19C0DAC1_3?cpp=%");[out].
  - **Returns:** ReferenceWoWGameObject Class

- **`Compare Method`**
  Compares two objects and returns a value indicating whether one is less than, equal
            to, or greater than the other.

- **`Compare Method (WoWGameObject, WoWGameObject)`**
  ```csharp
  public int Compare(
WoWGameObject x,
WoWGameObject y
)
  ```
  Compares two objects and returns a value indicating whether one is less than, equal
            to, or greater than the other.
  - *x*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTAD9A67E1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGameObjectThe first object to compare.
  - *y*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTAD9A67E1_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGameObjectWow game object to be compared.
  - **Returns:** See Also

- **`Compare Method (WoWUnit, WoWUnit)`**
  ```csharp
  public int Compare(
WoWUnit x,
WoWUnit y
)
  ```
  Compares two objects and returns a value indicating whether one is less than, equal
            to, or greater than the other.
  - *x*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST851DE64D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe first object to compare.
  - *y*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST851DE64D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitWow unit to be compared.
  - **Returns:** See Also

- **`CompareTo Method`**
  Compares this WoWGameObject object to another to determine their relative
            ordering.

- **`CompareTo Method (WoWGameObject)`**
  ```csharp
  public int CompareTo(
WoWGameObject other
)
  ```
  Compares this WoWGameObject object to another to determine their relative
            ordering.
  - *other*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST8BBD0A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGameObjectAnother instance to compare.
  - **Returns:** See Also

- **`CompareTo Method (WoWUnit)`**
  ```csharp
  public int CompareTo(
WoWUnit other
)
  ```
  Compares this WoWUnit object to another to determine their relative ordering.
  - *other*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST87550255_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitAnother instance to compare.
  - **Returns:** See Also

- **`GetCachedInfo Method`**
  ```csharp
  public bool GetCachedInfo(
out WoWCache.GameObjectCacheEntry inf
)
  ```
  Gets cached information.
  - *inf*: Type: Styx.WoWInternals.WoWCacheAddLanguageSpecificTextSet("LST3C05E778_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWCacheAddLanguageSpecificTextSet("LST3C05E778_3?cs=.|vb=.|cpp=::|nu=.|fs=.");GameObjectCacheEntryAddLanguageSpecificTextSet("LST3C05E778_4?cpp=%");[out] The inf.
  - **Returns:** ReferenceWoWGameObject Class

- **`GetCursor Method`**
  ```csharp
  public WoWCursorType GetCursor()
  ```
  Gets the cursor that would show when mousing over this object ingame.
  - **Returns:** This can be overridden by GetCursorOverrideAddLanguageSpecificTextSet("LST15B2E00B_1?cs=()|vb=|cpp=()|nu=()|fs=()");.

- **`GetCursorOverride Method`**
  ```csharp
  public string GetCursorOverride()
  ```
  Gets the name of the cursor override for this unit.
  - **Returns:** See Also

- **`GetDataSlot Method`**
  Gets data slot.

- **`GetDataSlot Method (UInt32, Int32)`**
  ```csharp
  public bool GetDataSlot(
uint dataSlot,
out int value
)
  ```
  Gets data slot.
  - *dataSlot*: Type: SystemAddLanguageSpecificTextSet("LST73AE250D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The data slot.
  - *value*: Type: SystemAddLanguageSpecificTextSet("LST73AE250D_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LST73AE250D_4?cpp=%");   [out] The value.
  - **Returns:** ReferenceWoWGameObject Class

- **`GetDataSlot Method (GameObjectDataSlot, Boolean)`**
  ```csharp
  public bool GetDataSlot(
GameObjectDataSlot dataSlot,
out bool value
)
  ```
  Gets data slot.
  - *dataSlot*: Type: StyxAddLanguageSpecificTextSet("LSTAA34F7F0_2?cs=.|vb=.|cpp=::|nu=.|fs=.");GameObjectDataSlotThe data slot.
  - *value*: Type: SystemAddLanguageSpecificTextSet("LSTAA34F7F0_3?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanAddLanguageSpecificTextSet("LSTAA34F7F0_4?cpp=%");   [out] The value.
  - **Returns:** ReferenceWoWGameObject Class

- **`GetDataSlot Method (GameObjectDataSlot, Int32)`**
  ```csharp
  public bool GetDataSlot(
GameObjectDataSlot dataSlot,
out int value
)
  ```
  Gets data slot.
  - *dataSlot*: Type: StyxAddLanguageSpecificTextSet("LST99245114_2?cs=.|vb=.|cpp=::|nu=.|fs=.");GameObjectDataSlotThe data slot.
  - *value*: Type: SystemAddLanguageSpecificTextSet("LST99245114_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LST99245114_4?cpp=%");   [out] The value.
  - **Returns:** ReferenceWoWGameObject Class

- **`GetReactionTowards Method`**
  ```csharp
  public WoWUnitReaction GetReactionTowards(
WoWUnit unit
)
  ```
  Gets reaction towards.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTE715C33A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe unit.
  - **Returns:** ReferenceWoWGameObject Class

- **`GetWorldMatrix Method`**
  ```csharp
  public override Matrix4x4 GetWorldMatrix()
  ```
  Returns the WorldMatrix of this WoWObject
  - **Returns:** ReferenceWoWGameObject Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents this instance.
  - **Returns:** ReferenceWoWGameObject Class

---

### WoWGameObject.LockEntry Structure

```csharp
public struct LockEntry
```

A lock entry.

#### LockEntry Fields

- **`Action Field`**
  ```csharp
  public byte[] Action
  ```
  The action.

- **`LockProperties Field`**
  ```csharp
  public uint[] LockProperties
  ```
  The lock properties.

- **`RequiredSkill Field`**
  ```csharp
  public ushort[] RequiredSkill
  ```
  The required skill.

- **`Type Field`**
  ```csharp
  public byte[] Type
  ```
  The type.

---

### WoWGameObject.LockTypeEntry Structure

```csharp
public struct LockTypeEntry
```

A lock type entry.

#### LockTypeEntry Properties

- **`InternalName Property`**
  ```csharp
  public string InternalName { get; }
  ```
  Gets the name of the internal.

- **`ItemStateName Property`**
  ```csharp
  public string ItemStateName { get; }
  ```
  Gets the name of the item state.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`ProcessName Property`**
  ```csharp
  public string ProcessName { get; }
  ```
  Gets the name of the process.

#### LockTypeEntry Fields

- **`_internalNamePtr Field`**
  ```csharp
  public IntPtr _internalNamePtr
  ```
  The internal name pointer.

- **`_itemStateNamePtr Field`**
  ```csharp
  public IntPtr _itemStateNamePtr
  ```
  The item state name pointer.

- **`_namePtr Field`**
  ```csharp
  public IntPtr _namePtr
  ```
  The name pointer.

- **`_processNamePtr Field`**
  ```csharp
  public IntPtr _processNamePtr
  ```
  The process name pointer.

- **`Id Field`**
  ```csharp
  public int Id
  ```
  The identifier.

---

### WoWGlyphInfo Structure

```csharp
[ObsoleteAttribute("This structure is only returned by obsoleted API and will be removed soon.")]
public struct WoWGlyphInfo
```

Information about the WoW glyph.

#### WoWGlyphInfo Fields

- **`Enabled Field`**
  ```csharp
  public bool Enabled
  ```
  true if this glyph

- **`ItemId Field`**
  ```csharp
  public uint ItemId
  ```
  Identifier for the item.

- **`Slot Field`**
  ```csharp
  public int Slot
  ```
  The slot.

---

### WoWInebriationLevel Enumeration

```csharp
public enum WoWInebriationLevel
```

Values that represent WoWInebriationLevel.

---

### WoWItem Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWObject → Styx.WoWInternals.WoWObjects.WoWItem → Styx.WoWInternals.WoWObjects.WoWContainer

```csharp
public class WoWItem : WoWObject
```

A WoW item.

#### WoWItem Properties

- **`BagIndex Property`**
  ```csharp
  public int BagIndex { get; }
  ```
  Gets the index of the bag this item is found in.

- **`BagSlot Property`**
  ```csharp
  public int BagSlot { get; }
  ```
  Gets the bag slot.

- **`CanExpire Property`**
  ```csharp
  public bool CanExpire { get; }
  ```
  Returns true if this WoWItem object can expire.

- **`CanProspect Property`**
  ```csharp
  public bool CanProspect { get; }
  ```
  Returns true if this WoWItem object can be prospected.

- **`ContainerGuid Property`**
  ```csharp
  public WoWGuid ContainerGuid { get; }
  ```
  Gets the container GUID.

- **`Context Property`**
  ```csharp
  public ItemContext Context { get; }
  ```
  Gets the context of this item.

- **`Cooldown Property`**
  ```csharp
  public float Cooldown { get; }
  ```
  Returns the cooldown left on this WoWItem object as a float

- **`CooldownTimeLeft Property`**
  ```csharp
  public TimeSpan CooldownTimeLeft { get; }
  ```
  Gets the cooldown time left.

- **`CreatorGuid Property`**
  ```csharp
  public WoWGuid CreatorGuid { get; }
  ```
  Gets the creator GUID.

- **`Distance Property`**
  ```csharp
  public override double Distance { get; }
  ```
  The distance from the local player to this object.

- **`Distance2D Property`**
  ```csharp
  public override double Distance2D { get; }
  ```
  The 2d distance from the local player to this object.

- **`Distance2DSqr Property`**
  ```csharp
  public override double Distance2DSqr { get; }
  ```
  The squared 2d distance from the local player to this object.

- **`DistanceSqr Property`**
  ```csharp
  public override double DistanceSqr { get; }
  ```
  The squared distance from the local player to this object.

- **`Durability Property`**
  ```csharp
  public uint Durability { get; }
  ```
  Gets the durability.

- **`DurabilityPercent Property`**
  ```csharp
  public double DurabilityPercent { get; }
  ```
  Gets the durability percent.

- **`Effects Property`**
  ```csharp
  public ItemEffectList Effects { get; }
  ```
  Gets the effects for this item. Returns null if ItemInfo is null.

- **`Flags Property`**
  ```csharp
  public uint Flags { get; }
  ```
  Gets the flags.

- **`GiftCreatorGuid Property`**
  ```csharp
  public WoWGuid GiftCreatorGuid { get; }
  ```
  Gets the gift creator GUID.

- **`HasEquipCooldown Property`**
  ```csharp
  public bool HasEquipCooldown { get; }
  ```
  Returns true if this WoWItem object has equip cooldown.

- **`IsAccountBound Property`**
  ```csharp
  public bool IsAccountBound { get; }
  ```
  Returns true if this WoWItem object is an account bound item.

- **`IsCharter Property`**
  ```csharp
  public bool IsCharter { get; }
  ```
  Returns true if this WoWItem object is a guild charter.

- **`IsConjured Property`**
  ```csharp
  public bool IsConjured { get; }
  ```
  Returns true if this WoWItem object is a conjured item.

- **`IsEnchantScroll Property`**
  ```csharp
  public bool IsEnchantScroll { get; }
  ```
  Returns true if this WoWItem object is an 'unique-equipped' item.

- **`IsEquippedUnusable Property`**
  ```csharp
  public bool IsEquippedUnusable { get; }
  ```
  Returns true if this WoWItem object is not usable while equipped.

- **`IsGiftWrapped Property`**
  ```csharp
  public bool IsGiftWrapped { get; }
  ```
  Returns true if this WoWItem object is a gift wrapped item.

- **`IsMillable Property`**
  ```csharp
  public bool IsMillable { get; }
  ```
  Returns true if this WoWItem object is millable.

- **`IsOpenable Property`**
  ```csharp
  public bool IsOpenable { get; }
  ```
  Returns true if this WoWItem object is openable.

- **`IsPvPItem Property`**
  ```csharp
  public bool IsPvPItem { get; }
  ```
  Returns true if this WoWItem object is a pvp item.

- **`IsReadable Property`**
  ```csharp
  public bool IsReadable { get; }
  ```
  Returns true if this WoWItem object is a readable item.

- **`IsSoulbound Property`**
  ```csharp
  public bool IsSoulbound { get; }
  ```
  Returns true if this WoWItem object is soulbound.

- **`IsThrownWeapon Property`**
  ```csharp
  public bool IsThrownWeapon { get; }
  ```
  Returns true if this WoWItem object is an thrown weapon.

- **`IsTotem Property`**
  ```csharp
  public bool IsTotem { get; }
  ```
  Returns true if this WoWItem object is a totem.

- **`IsUniqueEquipped Property`**
  ```csharp
  public bool IsUniqueEquipped { get; }
  ```
  Returns true if this WoWItem object is a 'unique-equipped' item.

- **`IsWand Property`**
  ```csharp
  public bool IsWand { get; }
  ```
  Returns true if this WoWItem object is a wand.

- **`IsWrappingPaper Property`**
  ```csharp
  public bool IsWrappingPaper { get; }
  ```
  Returns true if this WoWItem object is wrapping paper.

- **`ItemInfo Property`**
  ```csharp
  public ItemInfo ItemInfo { get; }
  ```
  Gets the item info.

- **`ItemLink Property`**
  ```csharp
  public string ItemLink { get; }
  ```
  Gets the item link.

- **`ItemStats Property`**
  ```csharp
  public ItemStats ItemStats { get; }
  ```
  Retruns the stats of this item as a ItemStats object.

- **`Location Property`**
  ```csharp
  public override Vector3 Location { get; }
  ```
  Returns the location as a Vector3.

- **`MaxDurability Property`**
  ```csharp
  public uint MaxDurability { get; }
  ```
  Gets the max durability.

- **`Name Property`**
  ```csharp
  public override string Name { get; }
  ```
  The name of this object.

- **`OwnerGuid Property`**
  ```csharp
  public WoWGuid OwnerGuid { get; }
  ```
  Gets the owner GUID.

- **`PropertySeed Property`**
  ```csharp
  public int PropertySeed { get; }
  ```
  Gets the property seed.

- **`Quality Property`**
  ```csharp
  public WoWItemQuality Quality { get; }
  ```
  The rarity of this item. 'Epic', 'Common', 'Legendary' etc.

- **`RandomPropertiesId Property`**
  ```csharp
  public int RandomPropertiesId { get; }
  ```
  Gets the random properties id.

- **`RandomSuffix Property`**
  ```csharp
  public ItemRandomSuffix RandomSuffix { get; }
  ```
  Returns the random suffix of this WoWItem as a
            ItemRandomSuffix object.

- **`RequiredEnchantingLevelToDisenchant Property`**
  ```csharp
  public Nullable&lt;int&gt; RequiredEnchantingLevelToDisenchant { get; }
  ```
  Returns the required Enchanting level needed to disenchant item or null if item can't be disenchanted.

- **`RequiredInscriptionLevelToMill Property`**
  ```csharp
  public Nullable&lt;int&gt; RequiredInscriptionLevelToMill { get; }
  ```
  Returns the required Inscription level needed to mill item or null if item can't be milled.

- **`RequiredJewelcraftingLevelToProspect Property`**
  ```csharp
  public Nullable&lt;int&gt; RequiredJewelcraftingLevelToProspect { get; }
  ```
  Returns the required Jewelcrafting level needed to prospect item or null if item can't be prospected.

- **`SpellCharges Property`**
  ```csharp
  public uint SpellCharges { get; }
  ```
  Gets the spell charges.

- **`StackCount Property`**
  ```csharp
  public uint StackCount { get; }
  ```
  Gets the stack count.

- **`TemporaryEnchantment Property`**
  ```csharp
  public WoWItem.WoWItemEnchantment TemporaryEnchantment { get; }
  ```
  Gets the temporary enchantment.

- **`TriggersSpell Property`**
  ```csharp
  public bool TriggersSpell { get; }
  ```
  Returns true if this WoWItem triggers a spell.

- **`Usable Property`**
  ```csharp
  public bool Usable { get; }
  ```
  Returns true if this WoWItem object is usable,  Notethis property uses
            lua.

#### WoWItem Methods

- **`GetEffect Method`**
  ```csharp
  public ItemEffect GetEffect(
int index
)
  ```
  Gets the spell.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST898F53C8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The index.
  - **Returns:** Exceptions

- **`GetEnchantment Method`**
  Returns an item enchantment class containing info about the enchantment. Note: this
            does an equality based on a name.Contains. So you can pass Rockbiter and it will match
            Rockbiter I.

- **`GetEnchantment Method (String)`**
  ```csharp
  public WoWItem.WoWItemEnchantment GetEnchantment(
string name
)
  ```
  Returns an item enchantment class containing info about the enchantment. Note: this
            does an equality based on a name.Contains. So you can pass Rockbiter and it will match
            Rockbiter I.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LST2D4134BD_2?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name.
  - **Returns:** See Also

- **`GetEnchantment Method (UInt32)`**
  ```csharp
  public WoWItem.WoWItemEnchantment GetEnchantment(
uint index
)
  ```
  Returns an item enchantment class containing info about the enchantment.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTFF798F8B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The index.
  - **Returns:** See Also

- **`GetEnchantmentById Method`**
  ```csharp
  public WoWItem.WoWItemEnchantment GetEnchantmentById(
uint id
)
  ```
  Returns an item enchantment class containing info about the enchantment.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTEDF05951_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The id.
  - **Returns:** See Also

- **`GetItemName Method`**
  ```csharp
  public string GetItemName()
  ```
  Gets the name of this WoWItem.
  - **Returns:** ReferenceWoWItem Class

- **`GetLuaContainerPosition Method`**
  ```csharp
  public bool GetLuaContainerPosition(
out int bag,
out int slot
)
  ```
  Gets the lua container ID and slot index that this item is located in.
  - *bag*: Type: SystemAddLanguageSpecificTextSet("LST6255DC44_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LST6255DC44_2?cpp=%");The container ID. -3 for the reagent bank, -1 for the bank, 0 for the backpack or 1 to 11 for the player's bags/bank bags.
  - *slot*: Type: SystemAddLanguageSpecificTextSet("LST6255DC44_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LST6255DC44_4?cpp=%");The (1-based) slot of the item in the container represented by bag.
  - **Returns:** See Also

- **`Interact Method`**
  Simulates a right click with this WoWItem.

- **`Interact Method`**
  ```csharp
  public override void Interact()
  ```
  Simulates a right click with this WoWItem.

- **`PickUp Method`**
  ```csharp
  public void PickUp()
  ```
  Picks up this WoWItem from it's corresponding container.

- **`Use Method`**
  Simulates a right click on this item.

- **`Use Method`**
  ```csharp
  public bool Use()
  ```
  Simulates a right click on this item.
  - **Returns:** See Also

- **`Use Method (Boolean)`**
  ```csharp
  public bool Use(
bool forceUse
)
  ```
  Simulates a right click on this item.
  - *forceUse*: Type: SystemAddLanguageSpecificTextSet("LSTD31DFCD6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanA boolean indicating whether to force the usage; if true is passed,
            you will not be asked with a 'using this item binds it to you' popup, if the item is bound on
            usage.
  - **Returns:** ReferenceWoWItem Class

- **`Use Method (WoWGuid)`**
  ```csharp
  public bool Use(
WoWGuid targetGuid
)
  ```
  Simulates a right click on this item.
  - *targetGuid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LSTFA9D5D36_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidThe target on which to use the item.
  - **Returns:** ReferenceWoWItem Class

- **`Use Method (WoWGuid, Boolean)`**
  ```csharp
  public bool Use(
WoWGuid targetGuid,
bool forceUse
)
  ```
  Simulates a right click on this item.
  - *targetGuid*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST74323215_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWGuidThe target on which to use the item.
  - *forceUse*: Type: SystemAddLanguageSpecificTextSet("LST74323215_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean  A boolean indicating whether to force the usage; if true is passed,
            you will not be asked with a 'using this item binds it to you' popup, if the item is bound on
            usage.
  - **Returns:** ReferenceWoWItem Class

- **`UseContainerItem Method`**
  ```csharp
  public void UseContainerItem()
  ```
  Use this container item.

---

### WoWItem.WoWItemEnchantment Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWItem.WoWItemEnchantment

```csharp
public class WoWItemEnchantment
```

A WoW item enchantment.

#### WoWItemEnchantment Properties

- **`ChargesLeft Property`**
  ```csharp
  public int ChargesLeft { get; }
  ```
  Returns the number of charges left.

- **`ExpirationTimestamp Property`**
  ```csharp
  public int ExpirationTimestamp { get; }
  ```
  /Returns the timestamp when this enchant expiress.

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Gets the identifier.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

#### WoWItemEnchantment Methods

- **`GetStat Method`**
  ```csharp
  public WoWItem.WoWItemStat GetStat(
int index
)
  ```
  Gets a stat.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST4B4AA36A_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32Zero-based index of the.
  - **Returns:** See Also

---

### WoWItem.WoWItemStat Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWItem.WoWItemStat

```csharp
public class WoWItemStat
```

A WoW item stat.

#### WoWItemStat Properties

- **`Type Property`**
  ```csharp
  public WoWItemStatType Type { get; }
  ```
  Gets the type.

- **`Value Property`**
  ```csharp
  public int Value { get; set; }
  ```
  Gets or sets the value.

#### WoWItemStat Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents this instance.
  - **Returns:** ReferenceWoWItemAddLanguageSpecificTextSet("LSTB0BF8F4D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWItemStat Class

---

### WoWLockType Enumeration

```csharp
public enum WoWLockType
```

Values that represent WoWLockType.

---

### WoWObject Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWObject → More.

```csharp
public class WoWObject : IEquatable&lt;WoWObject&gt;,
IComparable&lt;WoWObject&gt;
```

A WoW object.

#### WoWObject Properties

- **`BaseAddress Property`**
  ```csharp
  public IntPtr BaseAddress { get; }
  ```
  The BaseAddress of this object in WoW.

- **`DescriptorGuid Property`**
  ```csharp
  [ObsoleteAttribute("Use Guid instead")]
public virtual WoWGuid DescriptorGuid { get; }
  ```
  The GUID of this object according to the descriptors.

- **`Distance Property`**
  ```csharp
  public virtual double Distance { get; }
  ```
  The distance from the local player to this object.

- **`Distance2D Property`**
  ```csharp
  public virtual double Distance2D { get; }
  ```
  The 2d distance from the local player to this object.

- **`Distance2DSqr Property`**
  ```csharp
  public virtual double Distance2DSqr { get; }
  ```
  The squared 2d distance from the local player to this object.

- **`DistanceSqr Property`**
  ```csharp
  public virtual double DistanceSqr { get; }
  ```
  The squared distance from the local player to this object.

- **`DynamicFlags Property`**
  ```csharp
  public int DynamicFlags { get; }
  ```
  Gets the dynamic flags.

- **`Entry Property`**
  ```csharp
  public uint Entry { get; }
  ```
  The Entry of this object.

- **`Guid Property`**
  ```csharp
  public WoWGuid Guid { get; }
  ```
  The GUID of this object.

- **`InLineOfSight Property`**
  ```csharp
  public virtual bool InLineOfSight { get; }
  ```
  Returns true if this object is in line of sight.

- **`InteractRange Property`**
  ```csharp
  public virtual float InteractRange { get; }
  ```
  The InteractRange of this object.

- **`InteractRangeSqr Property`**
  ```csharp
  public float InteractRangeSqr { get; }
  ```
  The Squared InteractRange of this object.

- **`InteractType Property`**
  ```csharp
  public WoWInteractType InteractType { get; }
  ```
  The InteractType of thisi object.

- **`IsDisabled Property`**
  ```csharp
  [ObsoleteAttribute("Always returns false")]
public bool IsDisabled { get; }
  ```
  Returns whether the current object is disabled. (Fading out)

- **`IsIndoors Property`**
  ```csharp
  public virtual bool IsIndoors { get; }
  ```
  Returns true if this WoWObject is indoors.

- **`IsMe Property`**
  ```csharp
  public bool IsMe { get; }
  ```
  Returns true if this is me.

- **`IsOutdoors Property`**
  ```csharp
  public virtual bool IsOutdoors { get; }
  ```
  Returns true if this WoWObject is outdoors.

- **`IsValid Property`**
  ```csharp
  public bool IsValid { get; }
  ```
  Always returns true if your toon is logged in and the object exists.

- **`Location Property`**
  ```csharp
  public virtual Vector3 Location { get; }
  ```
  Returns the location as a Vector3 object.

- **`MeIsBehind Property`**
  ```csharp
  public virtual bool MeIsBehind { get; }
  ```
  Returns a boolean indicating whether the local player is behind this unit. This
            should not be used unless you know what you are doing. Use MeIsSafelyBehind
            instead.

- **`MeIsSafelyBehind Property`**
  ```csharp
  public virtual bool MeIsSafelyBehind { get; }
  ```
  Returns a boolean indicating whether the local player is safely behind this unit.

- **`ModelAnimFlags Property`**
  ```csharp
  public uint ModelAnimFlags { get; }
  ```
  Gets the model animation flags.

- **`Name Property`**
  ```csharp
  public virtual string Name { get; }
  ```
  The name of this object.

- **`QuestGiverStatus Property`**
  ```csharp
  public QuestGiverStatus QuestGiverStatus { get; }
  ```
  The QuestGiverStatus of this object, 'TurnIn', 'Avaible' etc.

- **`RelativeLocation Property`**
  ```csharp
  public virtual Vector3 RelativeLocation { get; }
  ```
  Gets the relative location.

- **`Rotation Property`**
  ```csharp
  public virtual float Rotation { get; }
  ```
  The rotation for this object.

- **`RotationDegrees Property`**
  ```csharp
  public float RotationDegrees { get; }
  ```
  The rotation in degrees for this object.

- **`SafeName Property`**
  ```csharp
  public virtual string SafeName { get; }
  ```
  Returns a safe identifier that can be used for logging. For anything but players and
            pets, this is the name of the object.

- **`Scale Property`**
  ```csharp
  public float Scale { get; }
  ```
  Gets the scale of this object.

- **`Type Property`**
  ```csharp
  public virtual WoWObjectType Type { get; }
  ```
  This object's type.

- **`TypeFlags Property`**
  ```csharp
  public WoWObjectTypeFlag TypeFlags { get; }
  ```
  The TypeFlags of this object.

- **`WithinInteractRange Property`**
  ```csharp
  public virtual bool WithinInteractRange { get; }
  ```
  Returns true if you are within interact range of this object. For game objects, this
            is not necessarily the same as being within InteractRange, because this takes into account
            the model of game objects.

- **`WorldLocation Property`**
  ```csharp
  public Vector3 WorldLocation { get; }
  ```
  Returns the WorldLocation of this object as a Vector3

- **`X Property`**
  ```csharp
  public float X { get; }
  ```
  The X coordinate for this object.

- **`Y Property`**
  ```csharp
  public float Y { get; }
  ```
  The Y coordinate for this object.

- **`Z Property`**
  ```csharp
  public float Z { get; }
  ```
  The Z coordinate for this object.

- **`ZDiff Property`**
  ```csharp
  public float ZDiff { get; }
  ```
  Gets the z axis difference between this unit and the local player.

#### WoWObject Methods

- **`CompareTo Method`**
  ```csharp
  public int CompareTo(
WoWObject other
)
  ```
  Compares to.
  - *other*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTEE8C4E96_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe other.
  - **Returns:** See Also

- **`EnsureValid Method`**
  ```csharp
  protected void EnsureValid(
string funcName
)
  ```
  - *funcName*: Type: SystemAddLanguageSpecificTextSet("LSTF347376A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String

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
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LSTE073E57D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe Object to compare with this instance.
  - **Returns:** Exceptions

- **`Equals Method (WoWObject)`**
  ```csharp
  public bool Equals(
WoWObject other
)
  ```
  Equalses the specified other.
  - *other*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST4282BF43_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe other.
  - **Returns:** See Also

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  Returns a hash code for this instance.
  - **Returns:** ReferenceWoWObject Class

- **`GetObjectName Method`**
  ```csharp
  [ObsoleteAttribute("Use the Name property instead")]
public string GetObjectName()
  ```
  Returns the name of a WoWObject
  - **Returns:** ExceptionConditionInvalidOperationExceptionThrown if this object is invalid.

- **`GetPosition Method`**
  ```csharp
  [ObsoleteAttribute("Use the Location property instead")]
public Vector3 GetPosition()
  ```
  Gets the position of the object using the virtual method table. This is slower than
            using Location. Use Location whenever possible.
  - **Returns:** ExceptionConditionInvalidOperationExceptionCannot begin injected subroutine on an object
            with BaseAddress == 0.

- **`GetWorldMatrix Method`**
  ```csharp
  public virtual Matrix4x4 GetWorldMatrix()
  ```
  Returns the WorldMatrix of this WoWObject
  - **Returns:** ReferenceWoWObject Class

- **`HasFlag Method`**
  ```csharp
  protected static bool HasFlag(
uint flag,
uint value
)
  ```
  - *flag*: Type: SystemAddLanguageSpecificTextSet("LSTB566F4F0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - *value*: Type: SystemAddLanguageSpecificTextSet("LSTB566F4F0_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32
  - **Returns:** ReferenceWoWObject Class

- **`Interact Method`**
  Performs a simulated right click on this object, very useful for looting npcs etc.

- **`Interact Method`**
  ```csharp
  public virtual void Interact()
  ```
  Performs a simulated right click on this object, very useful for looting npcs etc.

- **`Interact Method (Boolean)`**
  ```csharp
  public void Interact(
bool ignoreTimer
)
  ```
  Performs a simulated right click on this object, very useful for looting npcs etc.
  - *ignoreTimer*: Type: SystemAddLanguageSpecificTextSet("LSTD85F57D5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Booleanif set to true [ignore timer].

- **`IsBehind Method`**
  ```csharp
  public bool IsBehind(
WoWObject obj
)
  ```
  Returns a boolean indicating whether this object is behind an other object. This
            should not be used unless you know what you are doing. Use IsSafelyBehind instead.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST8EF53716_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe other object.
  - **Returns:** Exceptions

- **`IsFacing Method`**
  Returns a boolean indicating wheter this object is facing a point.

- **`IsFacing Method (Vector3)`**
  ```csharp
  public bool IsFacing(
Vector3 location
)
  ```
  Returns a boolean indicating wheter this object is facing a point.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST453F44D5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location to check against.
  - **Returns:** See Also

- **`IsFacing Method (WoWObject)`**
  ```csharp
  public bool IsFacing(
WoWObject obj
)
  ```
  Returns a boolean indicating whether this object is facing another object. This
            should not be used unless you know what yo uare doing. Use IsSafelyFacing instead.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST66A708BC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe other object.
  - **Returns:** Exceptions

- **`IsSafelyBehind Method`**
  ```csharp
  public bool IsSafelyBehind(
WoWObject obj
)
  ```
  Returns a boolean indicating whether this object is safely behind an other object.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST4DC49398_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe other object.
  - **Returns:** Exceptions

- **`IsSafelyFacing Method`**
  Returns a boolean indicating whether this object is facing a point.

- **`IsSafelyFacing Method (Vector3)`**
  ```csharp
  public bool IsSafelyFacing(
Vector3 location
)
  ```
  Returns a boolean indicating whether this object is facing a point.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LSTB0503A25_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location to check against.
  - **Returns:** See Also

- **`IsSafelyFacing Method (WoWObject)`**
  ```csharp
  public bool IsSafelyFacing(
WoWObject obj
)
  ```
  Returns a boolean indicating whether this object is safely facing another object.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST628070A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe other object.
  - **Returns:** Exceptions

- **`IsSafelyFacing Method (Vector3, Single)`**
  ```csharp
  public bool IsSafelyFacing(
Vector3 location,
float viewDegrees
)
  ```
  Returns a bool indicating whether this object is safely facing a location.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST3C2391C6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *viewDegrees*: Type: SystemAddLanguageSpecificTextSet("LST3C2391C6_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe view in degrees.
  - **Returns:** See Also

- **`IsSafelyFacing Method (WoWObject, Single)`**
  ```csharp
  public bool IsSafelyFacing(
WoWObject obj,
float viewDegrees
)
  ```
  Returns a boolean indicating whether this object is safely facing another object.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST1C71ECF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObject        The other object.
  - *viewDegrees*: Type: SystemAddLanguageSpecificTextSet("LST1C71ECF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Single(Optional) The view in degrees.
  - **Returns:** ReferenceWoWObject Class

- **`ToContainer Method`**
  ```csharp
  public WoWContainer ToContainer()
  ```
  Casts this WoWObject as a WoWContainer
  - **Returns:** ReferenceWoWObject Class

- **`ToDynamicObject Method`**
  ```csharp
  public WoWDynamicObject ToDynamicObject()
  ```
  Casts this WoWObject as a WoWDynamicObject
  - **Returns:** ReferenceWoWObject Class

- **`ToGameObject Method`**
  ```csharp
  public WoWGameObject ToGameObject()
  ```
  Casts this WoWObject as a WoWGameObject
  - **Returns:** ReferenceWoWObject Class

- **`ToItem Method`**
  ```csharp
  public WoWItem ToItem()
  ```
  Casts this WoWObject as a WoWItem
  - **Returns:** ReferenceWoWObject Class

- **`ToPlayer Method`**
  ```csharp
  public WoWPlayer ToPlayer()
  ```
  Casts this WoWObject as a WoWPlayer
  - **Returns:** ReferenceWoWObject Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents this instance.
  - **Returns:** ReferenceWoWObject Class

- **`ToUnit Method`**
  ```csharp
  public WoWUnit ToUnit()
  ```
  Casts this WoWObject as a WoWUnit
  - **Returns:** ReferenceWoWObject Class

#### WoWObject Events

- **`OnInvalidate Event`**
  ```csharp
  [ObsoleteAttribute("Use IsValid instead")]
public event ObjectInvalidateDelegate OnInvalidate
  ```
  This event is fired when this object becomes invalid, when it is removed from the
            object list.

#### WoWObject Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
WoWObject left,
WoWObject right
)
  ```
  Implements the operator ==.
  - *left*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST5528B962_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObject The left.
  - *right*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST5528B962_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe right.
  - **Returns:** ReferenceWoWObject Class

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
WoWObject left,
WoWObject right
)
  ```
  Implements the operator !=.
  - *left*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTC22E2BF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObject The left.
  - *right*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTC22E2BF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe right.
  - **Returns:** ReferenceWoWObject Class

---

### WoWPartyMember Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWPartyMember

```csharp
public class WoWPartyMember : IEquatable&lt;WoWPartyMember&gt;
```

A WoW party member.

#### WoWPartyMember Properties

- **`Area Property`**
  ```csharp
  public AreaTable Area { get; }
  ```
  Gets the area.

- **`AreaTableId Property`**
  ```csharp
  public int AreaTableId { get; }
  ```
  Gets the identifier of the area table.

- **`ContinentId Property`**
  ```csharp
  [ObsoleteAttribute("Use MapId instead.")]
public int ContinentId { get; }
  ```
  Gets the continent id.

- **`Dead Property`**
  ```csharp
  public bool Dead { get; }
  ```
  Gets a value indicating whether the dead.

- **`DNDFlagged Property`**
  ```csharp
  public bool DNDFlagged { get; }
  ```
  Gets a value indicating whether the dnd flagged.

- **`FFAPvpFlagged Property`**
  ```csharp
  public bool FFAPvpFlagged { get; }
  ```
  Gets a value indicating whether the ffa pvp flagged.

- **`Ghost Property`**
  ```csharp
  public bool Ghost { get; }
  ```
  Gets a value indicating whether the ghost.

- **`GroupLeader Property`**
  ```csharp
  public bool GroupLeader { get; }
  ```
  Gets a value indicating whether the group leader.

- **`GroupNumber Property`**
  ```csharp
  public uint GroupNumber { get; }
  ```
  Gets the group number. [Only valid in raids].

- **`Guid Property`**
  ```csharp
  public WoWGuid Guid { get; }
  ```
  Gets a unique identifier.

- **`Health Property`**
  ```csharp
  public uint Health { get; }
  ```
  Gets the health.

- **`HealthMax Property`**
  ```csharp
  public uint HealthMax { get; }
  ```
  Gets the health maximum.

- **`InVehicle Property`**
  ```csharp
  public bool InVehicle { get; }
  ```
  Gets a value indicating whether [in vehicle].

- **`IsInSamePhase Property`**
  ```csharp
  public bool IsInSamePhase { get; }
  ```
  Returns true if WoWPartyMember is in a phase as that of Me.

- **`IsMainAssist Property`**
  ```csharp
  public bool IsMainAssist { get; }
  ```
  Gets a value indicating whether this object is main assist. [Only valid in raids].

- **`IsMainTank Property`**
  ```csharp
  public bool IsMainTank { get; }
  ```
  Gets a value indicating whether this object is main tank. [Only valid in raids].

- **`IsOnline Property`**
  ```csharp
  public bool IsOnline { get; }
  ```
  Gets a value indicating whether this object is online.

- **`Level Property`**
  ```csharp
  public int Level { get; }
  ```
  Gets the level.

- **`Location Property`**
  ```csharp
  public Vector2 Location { get; }
  ```
  Gets the location. This location is only the X and Y. WoW does not store any more. If
            you need a Z, please use the Location3D API.

- **`Location3D Property`**
  ```csharp
  public Vector3 Location3D { get; }
  ```
  Gets the location.

- **`Map Property`**
  ```csharp
  public Map Map { get; }
  ```
  Gets the map.

- **`MapId Property`**
  ```csharp
  public int MapId { get; }
  ```
  Gets the map id.

- **`Power Property`**
  ```csharp
  public uint Power { get; }
  ```
  Gets the power.

- **`PowerMax Property`**
  ```csharp
  public uint PowerMax { get; }
  ```
  Gets the power maximum.

- **`PowerType Property`**
  ```csharp
  public WoWPowerType PowerType { get; }
  ```
  Gets the type of the power.

- **`PvpFlagged Property`**
  ```csharp
  public bool PvpFlagged { get; }
  ```
  Gets a value indicating whether the pvp flagged.

- **`RAFLinked Property`**
  ```csharp
  public bool RAFLinked { get; }
  ```
  Gets a value indicating whether the raf linked.

- **`RaidRank Property`**
  ```csharp
  public uint RaidRank { get; }
  ```
  Gets the raid rank. [Only valid in raids].

- **`Role Property`**
  ```csharp
  public WoWPartyMember.GroupRole Role { get; }
  ```
  Gets the role of this member. [Only valid in raids].

- **`Specialization Property`**
  ```csharp
  public WoWSpec Specialization { get; }
  ```
  The specialization for this party or raid member.

#### WoWPartyMember Methods

- **`Equals Method`**
  Determines whether the specified Object is equal to the
            current Object.

- **`Equals Method (Object)`**
  ```csharp
  public override bool Equals(
Object obj
)
  ```
  Determines whether the specified Object is equal to the
            current Object.
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LST419770_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe Object to compare with the current
            Object.
  - **Returns:** ReferenceWoWPartyMember Class

- **`Equals Method (WoWPartyMember)`**
  ```csharp
  public bool Equals(
WoWPartyMember other
)
  ```
  Tests if this WoWPartyMember is considered equal to another.
  - *other*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTE4433F9B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPartyMemberThe WoW party member to compare to this object.
  - **Returns:** See Also

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  Serves as a hash function for a particular type.
  - **Returns:** ReferenceWoWPartyMember Class

- **`HasRole Method`**
  ```csharp
  public bool HasRole(
WoWPartyMember.GroupRole role
)
  ```
  Whether or not the party member has the role.
  - *role*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTD2455063_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPartyMemberAddLanguageSpecificTextSet("LSTD2455063_3?cs=.|vb=.|cpp=::|nu=.|fs=.");GroupRoleThe role.
  - **Returns:** ReferenceWoWPartyMember Class

- **`ToPlayer Method`**
  ```csharp
  public WoWPlayer ToPlayer()
  ```
  Converts this object to a player. This is only valid if the member is within range of
            the object manager. (Roughly 100yds)
  - **Returns:** ReferenceWoWPartyMember Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceWoWPartyMember Class

#### WoWPartyMember Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
WoWPartyMember left,
WoWPartyMember right
)
  ```
  Equality operator.
  - *left*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST7F41782B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPartyMember The left.
  - *right*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST7F41782B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPartyMemberThe right.
  - **Returns:** ReferenceWoWPartyMember Class

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
WoWPartyMember left,
WoWPartyMember right
)
  ```
  Inequality operator.
  - *left*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTB252A98_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPartyMember The left.
  - *right*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTB252A98_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPartyMemberThe right.
  - **Returns:** ReferenceWoWPartyMember Class

---

### WoWPartyMember.GroupRole Enumeration

```csharp
[FlagsAttribute]
public enum GroupRole
```

Values that represent a role in our current group.

---

### WoWPlayer Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWObject → Styx.WoWInternals.WoWObjects.WoWUnit → Styx.WoWInternals.WoWObjects.WoWPlayer → Styx.WoWInternals.WoWObjects.LocalPlayer

```csharp
public class WoWPlayer : WoWUnit
```

A WoW player.

#### WoWPlayer Properties

- **`ArcaneBonusNegative Property`**
  ```csharp
  public uint ArcaneBonusNegative { get; }
  ```
  Gets the arcane bonus negative.

- **`ArcaneBonusPositive Property`**
  ```csharp
  public uint ArcaneBonusPositive { get; }
  ```
  Gets the arcane bonus positive.

- **`ArcaneCritPercent Property`**
  ```csharp
  public float ArcaneCritPercent { get; }
  ```
  Gets the arcane crit percent.

- **`AverageItemLevel Property`**
  ```csharp
  public float AverageItemLevel { get; }
  ```
  Gets the average item level.

- **`AverageItemLevelEquipped Property`**
  ```csharp
  public float AverageItemLevelEquipped { get; }
  ```
  Gets the average equipped item level.

- **`AverageItemLevelPvP Property`**
  ```csharp
  public float AverageItemLevelPvP { get; }
  ```
  Gets the average PVP item level.

- **`BlockPercent Property`**
  ```csharp
  public float BlockPercent { get; }
  ```
  Gets the block percent.

- **`CharacterPoints Property`**
  ```csharp
  public uint CharacterPoints { get; }
  ```
  Gets the character points.

- **`ChosenTitle Property`**
  ```csharp
  public uint ChosenTitle { get; }
  ```
  Gets the chosen title.

- **`ContestedPvPFlagged Property`**
  ```csharp
  public bool ContestedPvPFlagged { get; }
  ```
  Returns wheter the player has been involved in a PvP combat and will be attacked by
            contested guards.

- **`Copper Property`**
  ```csharp
  public ulong Copper { get; }
  ```
  Returns the amount of money you got in copper.

- **`CritPercent Property`**
  ```csharp
  public float CritPercent { get; }
  ```
  Gets the crit percent.

- **`DodgePercent Property`**
  ```csharp
  public float DodgePercent { get; }
  ```
  Gets the dodge percent.

- **`DuelArbiterGuid Property`**
  ```csharp
  public ulong DuelArbiterGuid { get; }
  ```
  Gets the unique identifier of the duel arbiter.

- **`DuelTeamId Property`**
  ```csharp
  public uint DuelTeamId { get; }
  ```
  Gets the identifier of the duel team.

- **`Experience Property`**
  ```csharp
  public uint Experience { get; }
  ```
  Gets the experience.

- **`Expertise Property`**
  ```csharp
  public uint Expertise { get; }
  ```
  Gets the expertise.

- **`ExpertiseOffHand Property`**
  ```csharp
  public uint ExpertiseOffHand { get; }
  ```
  Gets the expertise off hand.

- **`FactionGroup Property`**
  ```csharp
  public WoWFactionGroup FactionGroup { get; }
  ```
  Gets the group the faction belongs to.

- **`FireBonusNegative Property`**
  ```csharp
  public uint FireBonusNegative { get; }
  ```
  Gets the fire bonus negative.

- **`FireBonusPositive Property`**
  ```csharp
  public uint FireBonusPositive { get; }
  ```
  Gets the fire bonus positive.

- **`FireCritPercent Property`**
  ```csharp
  public float FireCritPercent { get; }
  ```
  Gets the fire crit percent.

- **`FrostBonusNegative Property`**
  ```csharp
  public uint FrostBonusNegative { get; }
  ```
  Gets the frost bonus negative.

- **`FrostBonusPositive Property`**
  ```csharp
  public uint FrostBonusPositive { get; }
  ```
  Gets the frost bonus positive.

- **`FrostCritPercent Property`**
  ```csharp
  public float FrostCritPercent { get; }
  ```
  Gets the frost crit percent.

- **`Gold Property`**
  ```csharp
  public ulong Gold { get; }
  ```
  Returns the amount of money you got in gold.

- **`GuildDeleteDate Property`**
  ```csharp
  public uint GuildDeleteDate { get; }
  ```
  Gets the guild delete date.

- **`GuildLevel Property`**
  ```csharp
  public uint GuildLevel { get; }
  ```
  Gets the guild level.

- **`GuildRank Property`**
  ```csharp
  public uint GuildRank { get; }
  ```
  Gets the guild rank.

- **`GuildTimestamp Property`**
  ```csharp
  public uint GuildTimestamp { get; }
  ```
  Gets the guild timestamp.

- **`HealingBonusPercent Property`**
  ```csharp
  public float HealingBonusPercent { get; }
  ```
  Gets the healing bonus percent.

- **`HealingBonusPositive Property`**
  ```csharp
  public uint HealingBonusPositive { get; }
  ```
  Gets the healing bonus positive.

- **`HealingModifierPercent Property`**
  ```csharp
  public float HealingModifierPercent { get; }
  ```
  Gets the healing modifier percent.

- **`HolyBonusNegative Property`**
  ```csharp
  public uint HolyBonusNegative { get; }
  ```
  Gets the holy bonus negative.

- **`HolyBonusPositive Property`**
  ```csharp
  public uint HolyBonusPositive { get; }
  ```
  Gets the holy bonus positive.

- **`HolyCritPercent Property`**
  ```csharp
  public float HolyCritPercent { get; }
  ```
  Gets the holy crit percent.

- **`HonorableKills Property`**
  ```csharp
  public uint HonorableKills { get; }
  ```
  Gets the honorable kills.

- **`Inebriation Property`**
  ```csharp
  public WoWInebriationLevel Inebriation { get; }
  ```
  Gets the inebriation.

- **`IsAFKFlagged Property`**
  ```csharp
  public bool IsAFKFlagged { get; }
  ```
  Returns wheter the player is tagged AFK.

- **`IsAlive Property`**
  ```csharp
  public override bool IsAlive { get; }
  ```
  Returns true if this player is alive.

- **`IsAlliance Property`**
  ```csharp
  public bool IsAlliance { get; }
  ```
  Returns true if this player is a alliance player.

- **`IsAttackedByMyGroup Property`**
  ```csharp
  public bool IsAttackedByMyGroup { get; }
  ```
  Gets a value indicating whether any of my group members are attacking this player.

- **`IsAttackingMe Property`**
  ```csharp
  public bool IsAttackingMe { get; }
  ```
  Gets a value indicating whether this player is attacking me or my pet.

- **`IsAttackingMyGroup Property`**
  ```csharp
  public bool IsAttackingMyGroup { get; }
  ```
  Gets a value indicating whether this player is attacking me or my pet.

- **`IsAutoAttacking Property`**
  ```csharp
  public override bool IsAutoAttacking { get; }
  ```
  Returns true if this player is auto attacking (either with auto shot or melee auto attack).

- **`IsDNDFlagged Property`**
  ```csharp
  public bool IsDNDFlagged { get; }
  ```
  Returns wheter the player is tagged DND.

- **`IsDps Property`**
  ```csharp
  public bool IsDps { get; }
  ```
  Gets a value indicating whether this instance is a DPS.

- **`IsFFAPvPFlagged Property`**
  ```csharp
  public bool IsFFAPvPFlagged { get; }
  ```
  Returs wheter a player is in a FFA PvP zone.

- **`IsGhost Property`**
  ```csharp
  public bool IsGhost { get; }
  ```
  Returns a bool indicating whether this player is a ghost.

- **`IsGM Property`**
  ```csharp
  public bool IsGM { get; }
  ```
  Returns wheter if this player is a GM.

- **`IsGroupLeader Property`**
  ```csharp
  public bool IsGroupLeader { get; }
  ```
  Returs wheter the player is group leader.

- **`IsHealer Property`**
  ```csharp
  public bool IsHealer { get; }
  ```
  Gets a value indicating whether this object is healer.

- **`IsHidingCloak Property`**
  ```csharp
  public bool IsHidingCloak { get; }
  ```
  Returns wheter the players cloak is hidden.

- **`IsHidingHelm Property`**
  ```csharp
  public bool IsHidingHelm { get; }
  ```
  Returns wheter the players helm is hidden.

- **`IsHorde Property`**
  ```csharp
  public bool IsHorde { get; }
  ```
  Returns true if this player is a horde player.

- **`IsInMyParty Property`**
  ```csharp
  public bool IsInMyParty { get; }
  ```
  Returns true if this player is in the local players party.

- **`IsInMyPartyOrRaid Property`**
  ```csharp
  public bool IsInMyPartyOrRaid { get; }
  ```
  Returns a boolean indicating whether this player is in the local players party or
            raid.

- **`IsInMyRaid Property`**
  ```csharp
  public bool IsInMyRaid { get; }
  ```
  Returns true if this player is in the local players party.

- **`IsInsideSanctuary Property`**
  ```csharp
  public bool IsInsideSanctuary { get; }
  ```
  Returns wheter the player is in a sanctuary soon.

- **`IsMelee Property`**
  ```csharp
  public bool IsMelee { get; }
  ```
  Gets a value indicating whether this instance is a melee.

- **`IsMeleeDps Property`**
  ```csharp
  public bool IsMeleeDps { get; }
  ```
  Gets a value indicating whether this instance is a melee DPS.

- **`IsNeutralPandaren Property`**
  ```csharp
  public bool IsNeutralPandaren { get; }
  ```
  Returns true if this layer is a low level, neutral, pandaren player.

- **`IsOutOfBounds Property`**
  ```csharp
  public bool IsOutOfBounds { get; }
  ```
  returns true if the local player is out of bounds 1 if the player is currently
            outside the bounds of the world; otherwise nil (1nil)

- **`IsPvPFlagged Property`**
  ```csharp
  public bool IsPvPFlagged { get; }
  ```
  Returns wheter the player is in PvP combat.

- **`IsPvPTimerActive Property`**
  ```csharp
  public bool IsPvPTimerActive { get; }
  ```
  Returns wheter the pvp timer is active, only occurs after pvp is toggeled manually.

- **`IsRanged Property`**
  ```csharp
  public bool IsRanged { get; }
  ```
  Gets a value indicating whether this instance is a ranged.

- **`IsRangedDps Property`**
  ```csharp
  public bool IsRangedDps { get; }
  ```
  Gets a value indicating whether this instance is a ranged DPS.

- **`IsResting Property`**
  ```csharp
  public bool IsResting { get; }
  ```
  Returns wheter the player is resting, eg; inside a major city or inn.

- **`IsTank Property`**
  ```csharp
  public bool IsTank { get; }
  ```
  Gets a value indicating whether this instance is tank.

- **`LevelFraction Property`**
  ```csharp
  public float LevelFraction { get; }
  ```
  Gets the level fraction of this unit; this is the level, with a fraction representing
            the XP the player has into the next level.

- **`LifetimeHonorableKills Property`**
  ```csharp
  public uint LifetimeHonorableKills { get; }
  ```
  Gets the lifetime honorable kills.

- **`Mastery Property`**
  ```csharp
  public float Mastery { get; }
  ```
  Gets the mastery.

- **`MaxLevel Property`**
  ```csharp
  public uint MaxLevel { get; }
  ```
  Gets the maximum level.

- **`Minions Property`**
  ```csharp
  public List&lt;WoWUnit&gt; Minions { get; }
  ```
  Returns a list of minion belonging to this player (excluding non combat pets),
            pet's/totems/various guardian characters.

- **`Mounted Property`**
  ```csharp
  public override bool Mounted { get; }
  ```
  Returns true if this player is mounted.

- **`NatureBonusNegative Property`**
  ```csharp
  public uint NatureBonusNegative { get; }
  ```
  Gets the nature bonus negative.

- **`NatureBonusPositive Property`**
  ```csharp
  public uint NatureBonusPositive { get; }
  ```
  Gets the nature bonus positive.

- **`NatureCritPercent Property`**
  ```csharp
  public float NatureCritPercent { get; }
  ```
  Gets the nature crit percent.

- **`NextLevelExperience Property`**
  ```csharp
  public uint NextLevelExperience { get; }
  ```
  Gets the next level experience.

- **`OffHandCritPercent Property`**
  ```csharp
  public float OffHandCritPercent { get; }
  ```
  Gets the off hand crit percent.

- **`ParryPercent Property`**
  ```csharp
  public float ParryPercent { get; }
  ```
  Gets the parry percent.

- **`PetSpellPower Property`**
  ```csharp
  public uint PetSpellPower { get; }
  ```
  Gets the pet spell power.

- **`PhysicalBonusNegative Property`**
  ```csharp
  public uint PhysicalBonusNegative { get; }
  ```
  Gets the physical bonus negative.

- **`PhysicalBonusPositive Property`**
  ```csharp
  public uint PhysicalBonusPositive { get; }
  ```
  Gets the physical bonus positive.

- **`PhysicalCritPercent Property`**
  ```csharp
  public float PhysicalCritPercent { get; }
  ```
  public byte FacialHair { get { return PlayerBytes2[0]; } }
            public bool HasRestedXp { get { return PlayerBytes2[3] == 1; } }
            public byte BattlefieldArenaFaction { get { return PlayerBytes3[3]; } }

- **`PvpMedalCount Property`**
  ```csharp
  public uint PvpMedalCount { get; }
  ```
  Gets the number of pvp medals.

- **`RangedCritPercent Property`**
  ```csharp
  public float RangedCritPercent { get; }
  ```
  Gets the ranged crit percent.

- **`RangedExpertise Property`**
  ```csharp
  public uint RangedExpertise { get; }
  ```
  Gets the ranged expertise.

- **`RestedExperience Property`**
  ```csharp
  public uint RestedExperience { get; }
  ```
  Gets the rested experience.

- **`SafeName Property`**
  ```csharp
  public override string SafeName { get; }
  ```
  Returns a safe identifier that can be used for logging. This is not the name of the
            player.

- **`SelfResurrectSpellId Property`**
  ```csharp
  public uint SelfResurrectSpellId { get; }
  ```
  Gets the identifier of the self resurrect spell.

- **`ShadowBonusNegative Property`**
  ```csharp
  public uint ShadowBonusNegative { get; }
  ```
  Gets the shadow bonus negative.

- **`ShadowBonusPositive Property`**
  ```csharp
  public uint ShadowBonusPositive { get; }
  ```
  Gets the shadow bonus positive.

- **`ShadowCritPercent Property`**
  ```csharp
  public float ShadowCritPercent { get; }
  ```
  Gets the shadow crit percent.

- **`ShieldBlock Property`**
  ```csharp
  public uint ShieldBlock { get; }
  ```
  Gets the shield block.

- **`ShieldBlockCritPercent Property`**
  ```csharp
  public float ShieldBlockCritPercent { get; }
  ```
  Gets the shield block crit percent.

- **`Silver Property`**
  ```csharp
  public ulong Silver { get; }
  ```
  Returns the amount of money you got in silver.

- **`Specialization Property`**
  ```csharp
  public WoWSpec Specialization { get; }
  ```
  Gets the current talent specialization for this player.

- **`SpecType Property`**
  ```csharp
  public SpecType SpecType { get; }
  ```
  Gets the type of the specifier.

- **`SpellPowerModifierPercent Property`**
  ```csharp
  public float SpellPowerModifierPercent { get; }
  ```
  Gets the spell power modifier percent.

- **`TargetArmorModifier Property`**
  ```csharp
  public uint TargetArmorModifier { get; }
  ```
  Gets target armor modifier.

- **`TargetResistanceModifier Property`**
  ```csharp
  public uint TargetResistanceModifier { get; }
  ```
  Gets target resistance modifier.

- **`WatchedFactionIndex Property`**
  ```csharp
  public uint WatchedFactionIndex { get; }
  ```
  Gets the zero-based index of the watched faction.

#### WoWPlayer Methods

- **`GetCombatRating Method`**
  ```csharp
  public int GetCombatRating(
WoWPlayerCombatRating rating
)
  ```
  Gets a secondary stat rating (combat rating) of the player.
  - *rating*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST89FC116_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPlayerCombatRating.
  - **Returns:** ReferenceWoWPlayer Class

---

### WoWPlayerCombatRating Enumeration

```csharp
public enum WoWPlayerCombatRating
```

Values that represent WoWPlayerCombatRating.

---

### WoWSubObject Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWSubObject → Styx.WoWInternals.WoWObjects.WoWAnimatedSubObject → Styx.WoWInternals.WoWObjects.WoWChair

```csharp
public class WoWSubObject
```

A WoW sub object.

#### WoWSubObject Properties

- **`BaseAddress Property`**
  ```csharp
  public IntPtr BaseAddress { get; }
  ```
  Gets the base address.

- **`InteractDistance Property`**
  ```csharp
  public float InteractDistance { get; }
  ```
  Gets the interact distance.

- **`OwnerObject Property`**
  ```csharp
  public WoWGameObject OwnerObject { get; }
  ```
  Gets the owner object.

#### WoWSubObject Methods

- **`CanUse Method`**
  ```csharp
  public bool CanUse()
  ```
  Returns a boolean indicating whether this game object can be used. This checks for
            stuff like keys, too low level for herbalism/mining etc.
  - **Returns:** Exceptions

- **`CanUseNow Method`**
  Returns a boolean indicating whether this game object can be used right now. This
            checks for stuff like distance.

- **`CanUseNow Method`**
  ```csharp
  public bool CanUseNow()
  ```
  Returns a boolean indicating whether this game object can be used right now. This
            checks for stuff like distance.
  - **Returns:** Exceptions

- **`CanUseNow Method (GameError)`**
  ```csharp
  public bool CanUseNow(
out GameError reason
)
  ```
  Returns a boolean indicating whether this game object can be used right now. If the
            return value is false, an out parameter gives the reason.
  - *reason*: Type: StyxAddLanguageSpecificTextSet("LST6581F6DF_2?cs=.|vb=.|cpp=::|nu=.|fs=.");GameErrorAddLanguageSpecificTextSet("LST6581F6DF_3?cpp=%");[out] The reason.
  - **Returns:** Exceptions

- **`CheckRange Method`**
  ```csharp
  public bool CheckRange()
  ```
  Determines whether the player is in range to use this sub object.
  - **Returns:** Exceptions

- **`GetCursor Method`**
  ```csharp
  public WoWCursorType GetCursor()
  ```
  Gets the cursor that would show when mousing over this object ingame.
  - **Returns:** ReferenceWoWSubObject Class

- **`Use Method`**
  ```csharp
  public void Use()
  ```
  Uses this WoWSubObject.

---

### WoWUnit Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWObject → Styx.WoWInternals.WoWObjects.WoWUnit → Styx.WoWInternals.WoWObjects.WoWPlayer

```csharp
public class WoWUnit : WoWObject, ILootableObject
```

A WoW unit.

#### WoWUnit Properties

- **`ActiveAuras Property`**
  ```csharp
  public Dictionary&lt;string, WoWAura&gt; ActiveAuras { get; }
  ```
  Returns a dictionary of all the active auras on a unit, with the English name of the aura as the key.

- **`Aggro Property`**
  ```csharp
  public bool Aggro { get; }
  ```
  Returns a boolean indicating whether this unit is attacking me.

- **`Agility Property`**
  ```csharp
  public uint Agility { get; }
  ```
  Gets the agility.

- **`AgilityNegativeModifier Property`**
  ```csharp
  public uint AgilityNegativeModifier { get; }
  ```
  Gets the agility negative modifier.

- **`AgilityPositiveModifier Property`**
  ```csharp
  public uint AgilityPositiveModifier { get; }
  ```
  Gets the agility positive modifier.

- **`Armor Property`**
  ```csharp
  public uint Armor { get; }
  ```
  Returns the amount of armor this unit has.

- **`AstralPowerInfo Property`**
  ```csharp
  public WoWUnit.PowerInfo AstralPowerInfo { get; }
  ```
  Gets information describing astral power.

- **`AstralPowerPercent Property`**
  ```csharp
  public double AstralPowerPercent { get; }
  ```
  Gets the astral power percent.

- **`Attackable Property`**
  ```csharp
  public bool Attackable { get; }
  ```
  Gets a value indicating whether this instance is attackable. Value is cached for 500ms

- **`AttackPower Property`**
  ```csharp
  public uint AttackPower { get; }
  ```
  Gets the attack power.

- **`AttackPowerMultiplier Property`**
  ```csharp
  public float AttackPowerMultiplier { get; }
  ```
  Gets the attack power multiplier.

- **`Auras Property`**
  ```csharp
  public Dictionary&lt;string, WoWAura&gt; Auras { get; }
  ```
  Returns a dictionary of all auras on a unit, with the English name of the aura as the key.

- **`AuraState Property`**
  ```csharp
  public uint AuraState { get; }
  ```
  Gets the state of the aura.

- **`BaseAttackTime Property`**
  ```csharp
  public uint BaseAttackTime { get; }
  ```
  Gets the time of the base attack.

- **`BaseHealth Property`**
  ```csharp
  public uint BaseHealth { get; }
  ```
  Gets the base health.

- **`BaseMana Property`**
  ```csharp
  public uint BaseMana { get; }
  ```
  Gets the base mana.

- **`BaseOffHandAttackTime Property`**
  ```csharp
  public uint BaseOffHandAttackTime { get; }
  ```
  Gets the time of the base off hand attack.

- **`BaseRangedAttackTime Property`**
  ```csharp
  public uint BaseRangedAttackTime { get; }
  ```
  Gets the time of the base ranged attack.

- **`BehindTarget Property`**
  ```csharp
  public bool BehindTarget { get; }
  ```
  Returns true if you are behind this unit.

- **`BoundingHeight Property`**
  ```csharp
  public float BoundingHeight { get; }
  ```
  Returns the height of the bounding box for this unit.

- **`BoundingRadius Property`**
  ```csharp
  public float BoundingRadius { get; }
  ```
  Gets the bounding radius.

- **`Buffs Property`**
  ```csharp
  public Dictionary&lt;string, WoWAura&gt; Buffs { get; }
  ```
  Returns a dictionary of all buffs on a unit, with the English name of the auras as the key.

- **`CanGossip Property`**
  ```csharp
  public bool CanGossip { get; }
  ```
  Gets a value indicating whether we can gossip.

- **`CanInteract Property`**
  ```csharp
  public bool CanInteract { get; }
  ```
  Gets a value indicating whether this WoWunit can be interacted with.

- **`CanInteractNow Property`**
  ```csharp
  public bool CanInteractNow { get; }
  ```
  Gets a value indicating whether this WoWunit can be interacted with now.
            For example this would return false on vehicles with no free seats available

- **`CanInterruptCurrentSpellCast Property`**
  ```csharp
  public bool CanInterruptCurrentSpellCast { get; }
  ```
  Returns whether the current spell being cast by this unit, can actually be
            interrupted. If no spell is being cast, this should return false.

- **`CanLoot Property`**
  ```csharp
  public bool CanLoot { get; }
  ```
  Gets a value indicating whether we can loot.

- **`CanSelect Property`**
  ```csharp
  public bool CanSelect { get; }
  ```
  Gets a value indicating whether we can select.

- **`CanSkin Property`**
  ```csharp
  public bool CanSkin { get; }
  ```
  Returns a boolean indicating whether this unit can be skinned by the local player.

- **`CastingSpell Property`**
  ```csharp
  public WoWSpell CastingSpell { get; }
  ```
  Returns the current spell being cast.

- **`CastingSpellId Property`**
  ```csharp
  public int CastingSpellId { get; }
  ```
  Return's the current spell ID being cast.

- **`CastSpeedModifier Property`**
  ```csharp
  public float CastSpeedModifier { get; }
  ```
  Gets the cast speed modifier.

- **`ChanneledCastingSpellId Property`**
  ```csharp
  public int ChanneledCastingSpellId { get; }
  ```
  Returns the spellid of the current channeled spell being cast.

- **`ChanneledSpell Property`**
  ```csharp
  public WoWSpell ChanneledSpell { get; }
  ```
  Returns the current channeled spell being cast.

- **`ChannelObject Property`**
  ```csharp
  public WoWObject ChannelObject { get; }
  ```
  Returns a WoWObject with the current channeled object, eg; 'Fishing
            Bobber'.

- **`ChannelObjectGuid Property`**
  ```csharp
  public WoWGuid ChannelObjectGuid { get; }
  ```
  Returns the guid of the current channeled object, eg; 'Fishing Bobber'.

- **`CharmedByUnit Property`**
  ```csharp
  public WoWUnit CharmedByUnit { get; }
  ```
  Gets the charmed by unit.

- **`CharmedByUnitGuid Property`**
  ```csharp
  public WoWGuid CharmedByUnitGuid { get; }
  ```
  Gets the charmed by unit guid.

- **`CharmedUnit Property`**
  ```csharp
  public WoWUnit CharmedUnit { get; }
  ```
  Gets the charmed unit.

- **`CharmedUnitGuid Property`**
  ```csharp
  public WoWGuid CharmedUnitGuid { get; }
  ```
  Gets the guid of the charmed unit.

- **`ChiInfo Property`**
  ```csharp
  public WoWUnit.PowerInfo ChiInfo { get; }
  ```
  Gets information describing the chi.

- **`Class Property`**
  ```csharp
  public WoWClass Class { get; }
  ```
  Gets the class.

- **`Classification Property`**
  ```csharp
  public WoWUnitClassificationType Classification { get; }
  ```
  Gets the classification of this WoWUnit; 'Normal','Elite','Rare' etc.

- **`Combat Property`**
  ```csharp
  public bool Combat { get; }
  ```
  Gets a value indicating whether the combat.

- **`CombatReach Property`**
  ```csharp
  public float CombatReach { get; }
  ```
  Gets the combat reach.

- **`ControllingPlayer Property`**
  ```csharp
  public WoWPlayer ControllingPlayer { get; }
  ```
  Gets the player controlling this unit, or null if this unit is not controlled.

- **`CreatedBySpellId Property`**
  ```csharp
  public uint CreatedBySpellId { get; }
  ```
  Gets the identifier of the created by spell.

- **`CreatedByUnit Property`**
  ```csharp
  public WoWUnit CreatedByUnit { get; }
  ```
  Gets the created by unit.

- **`CreatedByUnitGuid Property`**
  ```csharp
  public WoWGuid CreatedByUnitGuid { get; }
  ```
  Gets the created by unit guid.

- **`CreatureFamilyInfo Property`**
  ```csharp
  public CreatureFamily CreatureFamilyInfo { get; }
  ```
  Returns detailed information about this CreatureFamily (If any) null otherwise.

- **`CreatureRank Property`**
  ```csharp
  [ObsoleteAttribute("Use WoWUnit.Classification")]
public WoWUnitClassificationType CreatureRank { get; }
  ```
  Returns the rank of this unit; 'Normal','Elite','Rare' etc.

- **`CreatureType Property`**
  ```csharp
  public WoWCreatureType CreatureType { get; }
  ```
  Returns the WoWCreatureType of this unit.

- **`CurrentAstralPower Property`**
  ```csharp
  public int CurrentAstralPower { get; }
  ```
  Gets the current astral power.

- **`CurrentCastEndTime Property`**
  ```csharp
  public DateTime CurrentCastEndTime { get; }
  ```
  Gets the current cast end time.

- **`CurrentCastStartTime Property`**
  ```csharp
  public DateTime CurrentCastStartTime { get; }
  ```
  Gets the current cast start time.

- **`CurrentCastTimeLeft Property`**
  ```csharp
  public TimeSpan CurrentCastTimeLeft { get; }
  ```
  Gets the current cast time left.

- **`CurrentChannelEndTime Property`**
  ```csharp
  public DateTime CurrentChannelEndTime { get; }
  ```
  Gets the current channel end time.

- **`CurrentChannelStartTime Property`**
  ```csharp
  public DateTime CurrentChannelStartTime { get; }
  ```
  Gets the current channel start time.

- **`CurrentChannelTimeLeft Property`**
  ```csharp
  public TimeSpan CurrentChannelTimeLeft { get; }
  ```
  Gets the current channel time left.

- **`CurrentChi Property`**
  ```csharp
  public uint CurrentChi { get; }
  ```
  Gets the current chi.

- **`CurrentEnergy Property`**
  ```csharp
  public uint CurrentEnergy { get; }
  ```
  Returns the current energy of this unit.

- **`CurrentFocus Property`**
  ```csharp
  public uint CurrentFocus { get; }
  ```
  Returns the current focus of this unit.

- **`CurrentFury Property`**
  ```csharp
  public uint CurrentFury { get; }
  ```
  Gets the current fury.

- **`CurrentHealth Property`**
  ```csharp
  public uint CurrentHealth { get; }
  ```
  Returns the current health of this unit.

- **`CurrentHolyPower Property`**
  ```csharp
  public uint CurrentHolyPower { get; }
  ```
  Gets the current holy power.

- **`CurrentMaelstrom Property`**
  ```csharp
  public uint CurrentMaelstrom { get; }
  ```
  Gets the current maelstrom.

- **`CurrentMana Property`**
  ```csharp
  public uint CurrentMana { get; }
  ```
  Returns the current mana of this unit.

- **`CurrentPain Property`**
  ```csharp
  public uint CurrentPain { get; }
  ```
  Gets the current pain.

- **`CurrentPower Property`**
  ```csharp
  public uint CurrentPower { get; }
  ```
  Returns the current power of this unit.

- **`CurrentPowerInfo Property`**
  ```csharp
  public WoWUnit.PowerInfo CurrentPowerInfo { get; }
  ```
  Gets the information describing the current power.

- **`CurrentRage Property`**
  ```csharp
  public uint CurrentRage { get; }
  ```
  Returns the current rage of this unit.

- **`CurrentRunicPower Property`**
  ```csharp
  public uint CurrentRunicPower { get; }
  ```
  Returns the current runicpower of this unit.

- **`CurrentSoulShards Property`**
  ```csharp
  public uint CurrentSoulShards { get; }
  ```
  Gets the current soul shards.

- **`CurrentTarget Property`**
  ```csharp
  public WoWUnit CurrentTarget { get; }
  ```
  Gets the current target.

- **`CurrentTargetGuid Property`**
  ```csharp
  public virtual WoWGuid CurrentTargetGuid { get; }
  ```
  Gets the current target guid.

- **`Dazed Property`**
  ```csharp
  public bool Dazed { get; }
  ```
  Gets a value indicating whether the dazed.

- **`Debuffs Property`**
  ```csharp
  public Dictionary&lt;string, WoWAura&gt; Debuffs { get; }
  ```
  Returns a dictionary of all debuffs on a unit, with the English name of the auras as the key.

- **`Difficulty Property`**
  ```csharp
  public DifficultyColor Difficulty { get; }
  ```
  Gets the difficulty indicator color.

- **`Disarmed Property`**
  ```csharp
  public bool Disarmed { get; }
  ```
  Gets a value indicating whether the disarmed.

- **`DisplayId Property`**
  ```csharp
  public uint DisplayId { get; }
  ```
  Gets the identifier of the display.

- **`Elite Property`**
  ```csharp
  public bool Elite { get; }
  ```
  Gets a value indicating whether the elite.

- **`EnergyInfo Property`**
  ```csharp
  public WoWUnit.PowerInfo EnergyInfo { get; }
  ```
  Gets the information describing the energy.

- **`EnergyPercent Property`**
  ```csharp
  public double EnergyPercent { get; }
  ```
  Gets the energy in percent.

- **`Faction Property`**
  ```csharp
  public Faction Faction { get; }
  ```
  Gets the faction.

- **`FactionId Property`**
  ```csharp
  public uint FactionId { get; }
  ```
  Gets the faction template id.

- **`FactionTemplate Property`**
  ```csharp
  public FactionTemplate FactionTemplate { get; }
  ```
  Returns the faction template object for this unit. This class wraps common Faction-
            based information.

- **`Flags Property`**
  ```csharp
  public uint Flags { get; }
  ```
  Gets the flags.

- **`Flags2 Property`**
  ```csharp
  public uint Flags2 { get; }
  ```
  Gets the flags 2.

- **`Flags3 Property`**
  ```csharp
  public uint Flags3 { get; }
  ```
  Gets the flags3.

- **`Fleeing Property`**
  ```csharp
  public bool Fleeing { get; }
  ```
  Gets a value indicating whether the fleeing.

- **`FocusInfo Property`**
  ```csharp
  public WoWUnit.PowerInfo FocusInfo { get; }
  ```
  Gets the information describing the focus.

- **`FocusPercent Property`**
  ```csharp
  public double FocusPercent { get; }
  ```
  Gets the focus in percent.

- **`FuryInfo Property`**
  ```csharp
  public WoWUnit.PowerInfo FuryInfo { get; }
  ```
  Gets information describing fury.

- **`FuryPercent Property`**
  ```csharp
  public double FuryPercent { get; }
  ```
  Gets the fury in percent.

- **`Gender Property`**
  ```csharp
  public WoWGender Gender { get; }
  ```
  Gets the gender.

- **`GotAlivePet Property`**
  ```csharp
  public bool GotAlivePet { get; }
  ```
  Returns a boolean indicating if you got an alive pet.

- **`GotTarget Property`**
  ```csharp
  public bool GotTarget { get; }
  ```
  Returns a boolean indicating if you got a target.

- **`HasQuestCursor Property`**
  ```csharp
  public bool HasQuestCursor { get; }
  ```
  Return true if the unit has a quest cursor.

- **`HasteModifier Property`**
  ```csharp
  public float HasteModifier { get; }
  ```
  Gets the haste modifier.

- **`HasteRegenModifier Property`**
  ```csharp
  public float HasteRegenModifier { get; }
  ```
  Gets the haste regen modifier.

- **`HealthPercent Property`**
  ```csharp
  public double HealthPercent { get; }
  ```
  Gets the health in percent.

- **`HolyPowerInfo Property`**
  ```csharp
  public WoWUnit.PowerInfo HolyPowerInfo { get; }
  ```
  Gets the information describing the holy power.

- **`HolyPowerPercent Property`**
  ```csharp
  public double HolyPowerPercent { get; }
  ```
  Gets the holy power in percent.

- **`HoverHeight Property`**
  ```csharp
  public float HoverHeight { get; }
  ```
  Gets the height of the hover.

- **`InLineOfSight Property`**
  ```csharp
  public override bool InLineOfSight { get; }
  ```
  Returns true if this object is in line of sight. Used for visual sight.

- **`InLineOfSpellSight Property`**
  ```csharp
  public bool InLineOfSpellSight { get; }
  ```
  Returns true if this object is in line of sight to cast heal/damage spells on. Be
            warned! This may return true while there is no visual sight to the unit.

- **`Intellect Property`**
  ```csharp
  public uint Intellect { get; }
  ```
  Gets the intellect.

- **`IntellectNegativeModifier Property`**
  ```csharp
  public uint IntellectNegativeModifier { get; }
  ```
  Gets the intellect negative modifier.

- **`IntellectPositiveModifier Property`**
  ```csharp
  public uint IntellectPositiveModifier { get; }
  ```
  Gets the intellect positive modifier.

- **`InteractRange Property`**
  ```csharp
  public override float InteractRange { get; }
  ```
  Gets the maximum distance that LocalPlayer needs to be from this unit inorder to interact.

- **`InteractSpellId Property`**
  ```csharp
  public uint InteractSpellId { get; }
  ```

- **`IsAlive Property`**
  ```csharp
  public virtual bool IsAlive { get; }
  ```
  Returns a boolean indicating if this unit is alive.

- **`IsAmmoVendor Property`**
  ```csharp
  public bool IsAmmoVendor { get; }
  ```
  Gets a value indicating whether this unit is ammo vendor.

- **`IsAnyTrainer Property`**
  ```csharp
  public bool IsAnyTrainer { get; }
  ```
  Gets a value indicating whether this unit is any trainer.

- **`IsAnyVendor Property`**
  ```csharp
  public bool IsAnyVendor { get; }
  ```
  Gets a value indicating whether this unit is any vendor.

- **`IsAuctioneer Property`**
  ```csharp
  public bool IsAuctioneer { get; }
  ```
  Gets a value indicating whether this unit is auctioneer.

- **`IsAutoAttacking Property`**
  ```csharp
  public virtual bool IsAutoAttacking { get; }
  ```
  Returns a boolean indicating if this unit is auto attacking.

- **`IsBanker Property`**
  ```csharp
  public bool IsBanker { get; }
  ```
  Gets a value indicating whether this unit is banker.

- **`IsBattleMaster Property`**
  ```csharp
  public bool IsBattleMaster { get; }
  ```
  Gets a value indicating whether this unit is battle master.

- **`IsBeast Property`**
  ```csharp
  public bool IsBeast { get; }
  ```
  Returns true if this is a beast.

- **`IsBlackMarketAuctioneer Property`**
  ```csharp
  public bool IsBlackMarketAuctioneer { get; }
  ```
  Gets a value indicating whether this unit is a black market auctioneer.

- **`IsBoss Property`**
  ```csharp
  public bool IsBoss { get; }
  ```
  Gets a value indicating whether this WoWUnit is a boss.

- **`IsCasting Property`**
  ```csharp
  public bool IsCasting { get; }
  ```
  Returns true if this unit is casting a spell.

- **`IsCastingHealingSpell Property`**
  ```csharp
  public bool IsCastingHealingSpell { get; }
  ```
  Gets a value indicating whether this object is casting healing spell.

- **`IsChanneling Property`**
  ```csharp
  public bool IsChanneling { get; }
  ```
  Returns true if this unit is channeling a spell.

- **`IsClassTrainer Property`**
  ```csharp
  public bool IsClassTrainer { get; }
  ```
  Gets a value indicating whether this unit is class trainer.

- **`IsCritter Property`**
  ```csharp
  public bool IsCritter { get; }
  ```
  Returns true if this is a Critter.

- **`IsDead Property`**
  ```csharp
  public bool IsDead { get; }
  ```
  Gets a value indicating whether this unit is dead.

- **`IsDemon Property`**
  ```csharp
  public bool IsDemon { get; }
  ```
  Returns true if this is a demon.

- **`IsDragon Property`**
  ```csharp
  public bool IsDragon { get; }
  ```
  Returns true if this is a dragon.

- **`IsElemental Property`**
  ```csharp
  public bool IsElemental { get; }
  ```
  Returns true if this is a elemental.

- **`IsEvadeRunningBack Property`**
  ```csharp
  public bool IsEvadeRunningBack { get; }
  ```
  Gets a value indicating whether this WoWUnit is evade running back. Does not include bugged mobs
                that are evading while stationary

- **`IsExotic Property`**
  ```csharp
  public bool IsExotic { get; }
  ```
  Gets a value indicating whether this creature is an exotic creature.

- **`IsFalling Property`**
  ```csharp
  public bool IsFalling { get; }
  ```
  Returns a boolean indicating whether this unit is falling.

- **`IsFlightMaster Property`**
  ```csharp
  public bool IsFlightMaster { get; }
  ```
  Gets a value indicating whether this unit is flight master.

- **`IsFlying Property`**
  ```csharp
  public bool IsFlying { get; }
  ```
  Returns a boolean indicating whether this unit is flying.

- **`IsFoodVendor Property`**
  ```csharp
  public bool IsFoodVendor { get; }
  ```
  Gets a value indicating whether this unit is food vendor.

- **`IsFriendly Property`**
  ```csharp
  public bool IsFriendly { get; }
  ```
  Returns a boolean indicating if this unit is friendly to the local player; meaning if
            this unit is 'green' for the local player.

- **`IsGasCloud Property`**
  ```csharp
  public bool IsGasCloud { get; }
  ```
  Returns true if this is a gascloud.

- **`IsGhostVisible Property`**
  ```csharp
  public bool IsGhostVisible { get; }
  ```
  Returns a value indicating whether this creature is visible to a dead player like a
            sprit healer.

- **`IsGiant Property`**
  ```csharp
  public bool IsGiant { get; }
  ```
  Returns true if this is a giant.

- **`IsGuildBanker Property`**
  ```csharp
  public bool IsGuildBanker { get; }
  ```
  Gets a value indicating whether this unit is guild banker.

- **`IsHostile Property`**
  ```csharp
  public bool IsHostile { get; }
  ```
  Returns a boolean indicating if this unit is hostile to the local player; meaning if
            this unit is 'red' for the local player.

- **`IsHumanoid Property`**
  ```csharp
  public bool IsHumanoid { get; }
  ```
  Returns true if this is a humanoid.

- **`IsInnkeeper Property`**
  ```csharp
  public bool IsInnkeeper { get; }
  ```
  Gets a value indicating whether this unit is innkeeper.

- **`IsInSamePhase Property`**
  ```csharp
  public bool IsInSamePhase { get; }
  ```
  Returns true if WoWUnit is in a phase as that of Me.

- **`IsMailCourier Property`**
  ```csharp
  public bool IsMailCourier { get; }
  ```
  Gets a value indicating whether this unit is a mail courier.

- **`IsMechanical Property`**
  ```csharp
  public bool IsMechanical { get; }
  ```
  Returns true if this is a mechanical unit.

- **`IsMoving Property`**
  ```csharp
  public bool IsMoving { get; }
  ```
  Returns a boolean indicating if this unit is moving.

- **`IsNeutral Property`**
  ```csharp
  public bool IsNeutral { get; }
  ```
  Returns a boolean indicating if this unit is neutral to the local player; meaning if
            this unit is 'yellow' for the local player.

- **`IsNonCombatPet Property`**
  ```csharp
  public bool IsNonCombatPet { get; }
  ```
  Returns true if this is a non combat pet.

- **`IsOnTransport Property`**
  ```csharp
  public bool IsOnTransport { get; }
  ```
  Returns a boolean indicating if you are on a transport.

- **`IsPet Property`**
  ```csharp
  public bool IsPet { get; }
  ```
  Returns a boolean indicating if this unit is a pet.

- **`IsPetBattleCritter Property`**
  ```csharp
  public bool IsPetBattleCritter { get; }
  ```
  Gets a value indicating whether this unit is a valid pet battle critter.

- **`IsPetitioner Property`**
  ```csharp
  public bool IsPetitioner { get; }
  ```
  Gets a value indicating whether this unit is petitioner.

- **`IsPlayer Property`**
  ```csharp
  public bool IsPlayer { get; }
  ```
  Returns true if this unit is a player.

- **`IsPlayerBehind Property`**
  ```csharp
  public bool IsPlayerBehind { get; }
  ```
  Returns a boolean indicating if you are behind this unit.

- **`IsPoisonVendor Property`**
  ```csharp
  public bool IsPoisonVendor { get; }
  ```
  Gets a value indicating whether this unit is poison vendor.

- **`IsProfessionTrainer Property`**
  ```csharp
  public bool IsProfessionTrainer { get; }
  ```
  Gets a value indicating whether this unit is profession trainer.

- **`IsQuestGiver Property`**
  ```csharp
  public bool IsQuestGiver { get; }
  ```
  Gets a value indicating whether this unit is question giver.

- **`IsReagentVendor Property`**
  ```csharp
  public bool IsReagentVendor { get; }
  ```
  Gets a value indicating whether this unit is reagent vendor.

- **`IsReforger Property`**
  ```csharp
  public bool IsReforger { get; }
  ```
  Gets a value indicating whether this unit is a reforger.

- **`IsRepairMerchant Property`**
  ```csharp
  public bool IsRepairMerchant { get; }
  ```
  Gets a value indicating whether this unit is repair merchant.

- **`IsSpiritGuide Property`**
  ```csharp
  public bool IsSpiritGuide { get; }
  ```
  Gets a value indicating whether this unit is spirit guide.

- **`IsSpiritHealer Property`**
  ```csharp
  public bool IsSpiritHealer { get; }
  ```
  Gets a value indicating whether this unit is spirit healer.

- **`IsStableMaster Property`**
  ```csharp
  public bool IsStableMaster { get; }
  ```
  Gets a value indicating whether this unit is stable master.

- **`IsStealthed Property`**
  ```csharp
  public bool IsStealthed { get; }
  ```
  Returns true if this unit is stealthed.

- **`IsSubmerged Property`**
  ```csharp
  public bool IsSubmerged { get; }
  ```
  Gets a bool that indicates whether this unit is submerged.

- **`IsSwimming Property`**
  ```csharp
  public bool IsSwimming { get; }
  ```
  Returns a boolean indicating whether this unit is swimming.

- **`IsTabardDesigner Property`**
  ```csharp
  public bool IsTabardDesigner { get; }
  ```
  Gets a value indicating whether this unit is tabard designer.

- **`IsTagged Property`**
  ```csharp
  public bool IsTagged { get; }
  ```
  Gets a value indicating whether this object is tagged.

- **`IsTameable Property`**
  ```csharp
  public bool IsTameable { get; }
  ```
  Gets a value indicating whether this creature can be tamed by a hunter.

- **`IsTargetingAnyMinion Property`**
  ```csharp
  public bool IsTargetingAnyMinion { get; }
  ```
  Returns a boolean indicating wheter this mob is targeting any of the local players
            minions.

- **`IsTargetingMeOrPet Property`**
  ```csharp
  public bool IsTargetingMeOrPet { get; }
  ```
  Returns a boolean indicating whether this mob is targeting the local player or the
            local players pet.

- **`IsTargetingMyPartyMember Property`**
  ```csharp
  public bool IsTargetingMyPartyMember { get; }
  ```
  Returns true if this unit is targeting anyone in the local player's party.

- **`IsTargetingMyRaidMember Property`**
  ```csharp
  public bool IsTargetingMyRaidMember { get; }
  ```
  Returns true if this unit is targeting anyone in the local player's raid.

- **`IsTargetingPet Property`**
  ```csharp
  public bool IsTargetingPet { get; }
  ```
  Returns a boolean indicating if this unit is targeting your pet, if you have any.

- **`IsTotem Property`**
  ```csharp
  public bool IsTotem { get; }
  ```
  Returns true if this is a totem.

- **`IsTrainer Property`**
  ```csharp
  public bool IsTrainer { get; }
  ```
  Gets a value indicating whether this unit is trainer.

- **`IsTransmogrifier Property`**
  ```csharp
  public bool IsTransmogrifier { get; }
  ```
  Gets a value indicating whether this unit is a transmogrifier.

- **`IsUndead Property`**
  ```csharp
  public bool IsUndead { get; }
  ```
  Returns true if this is an undead.

- **`IsUnit Property`**
  ```csharp
  public bool IsUnit { get; }
  ```
  Returns true if this unit is a unit.

- **`IsVaultKeeper Property`**
  ```csharp
  public bool IsVaultKeeper { get; }
  ```
  Gets a value indicating whether this unit is a vault keeper, thus providing void storage.

- **`IsVendor Property`**
  ```csharp
  public bool IsVendor { get; }
  ```
  Gets a value indicating whether this unit is vendor.

- **`IsWithinMeleeRange Property`**
  ```csharp
  public bool IsWithinMeleeRange { get; }
  ```
  Gets a bool that indicates whether the local player can melee attack this unit.

- **`IsWorldBoss Property`**
  ```csharp
  public bool IsWorldBoss { get; }
  ```
  Gets a value indicating whether this unit is world boss.

- **`KilledByMe Property`**
  ```csharp
  public bool KilledByMe { get; }
  ```
  Returns a boolean indicating whether this unit is tagged by me and is dead.

- **`Level Property`**
  ```csharp
  public int Level { get; }
  ```
  Gets the level.

- **`Location Property`**
  ```csharp
  public override Vector3 Location { get; }
  ```
  Returns the Location of this unit.

- **`Lootable Property`**
  ```csharp
  public bool Lootable { get; }
  ```
  Gets a value indicating whether we can loot.

- **`Looting Property`**
  ```csharp
  public bool Looting { get; }
  ```
  Gets a value indicating whether the looting.

- **`MaelstromInfo Property`**
  ```csharp
  public WoWUnit.PowerInfo MaelstromInfo { get; }
  ```
  Gets information describing maelstrom.

- **`ManaInfo Property`**
  ```csharp
  public WoWUnit.PowerInfo ManaInfo { get; }
  ```
  Gets the information describing the mana.

- **`ManaPercent Property`**
  ```csharp
  public double ManaPercent { get; }
  ```
  Gets the mana in percent.

- **`MaxAstralPower Property`**
  ```csharp
  public uint MaxAstralPower { get; }
  ```
  Gets the max astral power.

- **`MaxChi Property`**
  ```csharp
  public uint MaxChi { get; }
  ```
  Gets the maximum chi.

- **`MaxDamage Property`**
  ```csharp
  public float MaxDamage { get; }
  ```
  Gets the maximum damage.

- **`MaxEnergy Property`**
  ```csharp
  public uint MaxEnergy { get; }
  ```
  Returns the maxmimum energy of this unit.

- **`MaxFocus Property`**
  ```csharp
  public uint MaxFocus { get; }
  ```
  Returns the maximum focus of this unit.

- **`MaxFury Property`**
  ```csharp
  public uint MaxFury { get; }
  ```
  Gets the max fury.

- **`MaxHealth Property`**
  ```csharp
  public uint MaxHealth { get; }
  ```
  Returns the maximum health of this unit.

- **`MaxHealthModifier Property`**
  ```csharp
  public float MaxHealthModifier { get; }
  ```
  Gets the maximum health modifier.

- **`MaxHolyPower Property`**
  ```csharp
  public uint MaxHolyPower { get; }
  ```
  Gets the maximum holy power.

- **`MaxItemLevel Property`**
  ```csharp
  public int MaxItemLevel { get; }
  ```
  Gets the maximum item level.

- **`MaxMaelstrom Property`**
  ```csharp
  public uint MaxMaelstrom { get; }
  ```
  Gets the max maelstrom.

- **`MaxMana Property`**
  ```csharp
  public uint MaxMana { get; }
  ```
  Returns the maximum mana of this unit.

- **`MaxOffHandDamage Property`**
  ```csharp
  public float MaxOffHandDamage { get; }
  ```
  Gets the maximum off hand damage.

- **`MaxPain Property`**
  ```csharp
  public uint MaxPain { get; }
  ```
  Gets the max pain.

- **`MaxPower Property`**
  ```csharp
  public uint MaxPower { get; }
  ```
  Gets the maximum power.

- **`MaxRage Property`**
  ```csharp
  public uint MaxRage { get; }
  ```
  Returns the maximum rage of this unit.

- **`MaxRangedDamage Property`**
  ```csharp
  public float MaxRangedDamage { get; }
  ```
  Gets the maximum ranged damage.

- **`MaxRunicPower Property`**
  ```csharp
  public uint MaxRunicPower { get; }
  ```
  Returns the maximum runicpower of this unit.

- **`MaxSoulShards Property`**
  ```csharp
  public uint MaxSoulShards { get; }
  ```
  Gets the maximum soul shards.

- **`MeIsBehind Property`**
  ```csharp
  public override bool MeIsBehind { get; }
  ```
  Returns a boolean indicating whether the local player is behind this unit. This
            should not be used unless you know what you are doing. Use MeIsSafelyBehind
            instead.

- **`MeIsSafelyBehind Property`**
  ```csharp
  public override bool MeIsSafelyBehind { get; }
  ```
  Returns a boolean indicating whether the local player is safely behind this unit.

- **`MeleeRange Property`**
  ```csharp
  public float MeleeRange { get; }
  ```
  Gets the melee range of this unit; that is, the range at which this unit can attack the local player.

- **`MinDamage Property`**
  ```csharp
  public uint MinDamage { get; }
  ```
  Gets the minimum damage.

- **`MinOffHandDamage Property`**
  ```csharp
  public float MinOffHandDamage { get; }
  ```
  Gets the minimum off hand damage.

- **`MinRangedDamage Property`**
  ```csharp
  public float MinRangedDamage { get; }
  ```
  Gets the minimum ranged damage.

- **`MountDisplayId Property`**
  ```csharp
  public uint MountDisplayId { get; }
  ```
  Gets the identifier of the mount display.

- **`Mounted Property`**
  ```csharp
  public virtual bool Mounted { get; }
  ```
  Gets a value indicating whether the mounted.

- **`MovementInfo Property`**
  ```csharp
  public WoWMovementInfo MovementInfo { get; }
  ```
  Returns a WoWMovementInfo struct for this unit.

- **`MyAggroRange Property`**
  ```csharp
  public float MyAggroRange { get; }
  ```
  Returns the approximate aggro radius towards the local player.

- **`MyReaction Property`**
  ```csharp
  public WoWUnitReaction MyReaction { get; }
  ```
  Returns the WoWUnitReaction of the local player towards this unit.

- **`MyStealthDetectionRange Property`**
  ```csharp
  public float MyStealthDetectionRange { get; }
  ```
  Returns the approximate range at which the local player will become detected by this
            unit.

- **`NativeDisplayId Property`**
  ```csharp
  public uint NativeDisplayId { get; }
  ```
  Gets the identifier of the native display.

- **`NonChanneledCastingSpellId Property`**
  ```csharp
  public int NonChanneledCastingSpellId { get; }
  ```
  Returns the spell Id of the current non channeled spell being cast.

- **`NpcFlags Property`**
  ```csharp
  public uint NpcFlags { get; }
  ```
  Gets the npc flags.

- **`NpcFlags2 Property`**
  ```csharp
  public uint NpcFlags2 { get; }
  ```
  Gets the secondary NPC flags.

- **`OnTaxi Property`**
  ```csharp
  public bool OnTaxi { get; }
  ```
  Gets a value indicating whether the on taxi.

- **`OwnedByRoot Property`**
  ```csharp
  public WoWUnit OwnedByRoot { get; }
  ```
  Returns the root unit that owns this unit.

- **`OwnedByUnit Property`**
  ```csharp
  public virtual WoWUnit OwnedByUnit { get; }
  ```
  If this unit is a pet, this returns the Unit who 'owns' this pet. (Summoned, charmed
            or created by)

- **`Pacified Property`**
  ```csharp
  public bool Pacified { get; }
  ```
  Gets a value indicating whether the pacified.

- **`PainInfo Property`**
  ```csharp
  public WoWUnit.PowerInfo PainInfo { get; }
  ```
  Gets information describing pain.

- **`PainPercent Property`**
  ```csharp
  public double PainPercent { get; }
  ```
  Gets the pain in percent.

- **`PassiveAuras Property`**
  ```csharp
  public Dictionary&lt;string, WoWAura&gt; PassiveAuras { get; }
  ```
  Returns a dictionary of all the passive auras on a unit, with the English name of the aura as the key.

- **`Pet Property`**
  ```csharp
  public virtual WoWUnit Pet { get; }
  ```
  Returns the pet of this unit.

- **`PetAggro Property`**
  ```csharp
  public bool PetAggro { get; }
  ```
  Returns a boolean indicating whether this unit is attacking my pet.

- **`PetExperience Property`**
  ```csharp
  public uint PetExperience { get; }
  ```
  Gets the pet experience.

- **`PetInCombat Property`**
  ```csharp
  public bool PetInCombat { get; }
  ```
  Gets a value indicating whether the pet in combat.

- **`PetNameTimestamp Property`**
  ```csharp
  public uint PetNameTimestamp { get; }
  ```
  Gets the pet name timestamp.

- **`PetNextLevelExperience Property`**
  ```csharp
  public uint PetNextLevelExperience { get; }
  ```
  Gets the pet next level experience.

- **`PetNumber Property`**
  ```csharp
  public uint PetNumber { get; }
  ```
  Gets the pet number.

- **`PlayerControlled Property`**
  ```csharp
  public bool PlayerControlled { get; }
  ```
  Gets a value indicating whether the player controlled.

- **`Possessed Property`**
  ```csharp
  public bool Possessed { get; }
  ```
  Gets a value indicating whether the possessed.

- **`PowerPercent Property`**
  ```csharp
  public float PowerPercent { get; }
  ```
  Gets the power percent.

- **`PowerType Property`**
  ```csharp
  public WoWPowerType PowerType { get; }
  ```
  Gets the type of the power.

- **`PvpFlagged Property`**
  ```csharp
  public bool PvpFlagged { get; }
  ```
  Gets a value indicating whether the pvp flagged.

- **`Race Property`**
  ```csharp
  public WoWRace Race { get; }
  ```
  Gets the race.

- **`RafLinked Property`**
  ```csharp
  public bool RafLinked { get; }
  ```
  Gets a value indicating whether the raf linked.

- **`RageInfo Property`**
  ```csharp
  public WoWUnit.PowerInfo RageInfo { get; }
  ```
  Gets the information describing the rage.

- **`RagePercent Property`**
  ```csharp
  public double RagePercent { get; }
  ```
  Gets the rage in percent.

- **`RangedAttackPower Property`**
  ```csharp
  public uint RangedAttackPower { get; }
  ```
  Gets the ranged attack power.

- **`RangedAttackPowerMultiplier Property`**
  ```csharp
  public float RangedAttackPowerMultiplier { get; }
  ```
  Gets the ranged attack power multiplier.

- **`RelativeLocation Property`**
  ```csharp
  public override Vector3 RelativeLocation { get; }
  ```
  Returns the relative location of this unit.

- **`RenderFacing Property`**
  ```csharp
  public float RenderFacing { get; }
  ```
  Gets the render facing for this WoWUnit.

- **`ResistArcane Property`**
  ```csharp
  public uint ResistArcane { get; }
  ```
  Returns the amount of arcane resistance this unit has.

- **`ResistFire Property`**
  ```csharp
  public uint ResistFire { get; }
  ```
  Returns the amount of fire resistance this unit has.

- **`ResistFrost Property`**
  ```csharp
  public uint ResistFrost { get; }
  ```
  Returns the amount of frost resistance this unit has.

- **`ResistHoly Property`**
  ```csharp
  public uint ResistHoly { get; }
  ```
  Returns the amount of holy resistance this unit has.

- **`ResistNature Property`**
  ```csharp
  public uint ResistNature { get; }
  ```
  Returns the amount of nature resistance this unit has.

- **`ResistShadow Property`**
  ```csharp
  public uint ResistShadow { get; }
  ```
  Returns the amount of shadow resistance this unit has.

- **`Rooted Property`**
  ```csharp
  public bool Rooted { get; }
  ```
  Gets a value indicating whether the rooted.

- **`Rotation Property`**
  ```csharp
  public override float Rotation { get; }
  ```
  Returns the rotation/facing of this unit in radians.

- **`RunesPercent Property`**
  ```csharp
  public double RunesPercent { get; }
  ```
  Gets the runes in percent.

- **`RunesPowerInfo Property`**
  ```csharp
  public WoWUnit.PowerInfo RunesPowerInfo { get; }
  ```
  Gets the information describing the runes power.

- **`RunicPowerInfo Property`**
  ```csharp
  public WoWUnit.PowerInfo RunicPowerInfo { get; }
  ```
  Gets the information describing the runic power.

- **`RunicPowerPercent Property`**
  ```csharp
  public double RunicPowerPercent { get; }
  ```
  Gets the runic power in percent.

- **`SafeName Property`**
  ```csharp
  public override string SafeName { get; }
  ```
  Returns a safe identifier that can be used for logging. For anything but players and
            pets, this is the name of the unit.

- **`Shapeshift Property`**
  ```csharp
  public ShapeshiftForm Shapeshift { get; }
  ```
  Gets the shapeshift.

- **`Silenced Property`**
  ```csharp
  public bool Silenced { get; }
  ```
  Gets a value indicating whether the silenced.

- **`Skinnable Property`**
  ```csharp
  public bool Skinnable { get; }
  ```
  Gets a value indicating whether the unit is skinnable.
            This includes all skin types, such as leather, herbs and others.

- **`SkinType Property`**
  ```csharp
  public WoWCreatureSkinType SkinType { get; }
  ```
  Gets a value indicating the skin type of this unit.

- **`SoulShardsInfo Property`**
  ```csharp
  public WoWUnit.PowerInfo SoulShardsInfo { get; }
  ```
  Gets the information describing the soul shards.

- **`SoulShardsPercent Property`**
  ```csharp
  public double SoulShardsPercent { get; }
  ```
  Gets the soul shards in percent.

- **`SpellHasteModifier Property`**
  ```csharp
  public float SpellHasteModifier { get; }
  ```
  Gets the spell haste modifier.

- **`Spirit Property`**
  ```csharp
  public uint Spirit { get; }
  ```
  Gets the spirit.

- **`SpiritNegativeModifier Property`**
  ```csharp
  public uint SpiritNegativeModifier { get; }
  ```
  Gets the spirit negative modifier.

- **`SpiritPositiveModifier Property`**
  ```csharp
  public uint SpiritPositiveModifier { get; }
  ```
  Gets the spirit positive modifier.

- **`Stamina Property`**
  ```csharp
  public uint Stamina { get; }
  ```
  Gets the stamina.

- **`StaminaNegativeModifier Property`**
  ```csharp
  public uint StaminaNegativeModifier { get; }
  ```
  Gets the stamina negative modifier.

- **`StaminaPositiveModifier Property`**
  ```csharp
  public uint StaminaPositiveModifier { get; }
  ```
  Gets the stamina positive modifier.

- **`Strength Property`**
  ```csharp
  public uint Strength { get; }
  ```
  Gets the strength.

- **`StrengthNegativeModifier Property`**
  ```csharp
  public uint StrengthNegativeModifier { get; }
  ```
  This unit's current negative strength modifier. (Negative strength. Usually from
            debuffs.)

- **`StrengthPositiveModifier Property`**
  ```csharp
  public uint StrengthPositiveModifier { get; }
  ```
  This unit's current positive strength modifier. (Bonus strength. Usually from buffs.)

- **`Stunned Property`**
  ```csharp
  public bool Stunned { get; }
  ```
  Gets a value indicating whether the stunned.

- **`SubName Property`**
  ```csharp
  public string SubName { get; }
  ```
  Returns the subname of this unit.

- **`SummonedByUnit Property`**
  ```csharp
  public WoWUnit SummonedByUnit { get; }
  ```
  Gets the summoned by unit.

- **`SummonedByUnitGuid Property`**
  ```csharp
  public WoWGuid SummonedByUnitGuid { get; }
  ```
  Gets the summoned by unit guid.

- **`SummonedUnit Property`**
  ```csharp
  public WoWUnit SummonedUnit { get; }
  ```
  Gets the summoned unit.

- **`SummonedUnitGuid Property`**
  ```csharp
  public WoWGuid SummonedUnitGuid { get; }
  ```
  Gets the summoned unit guid.

- **`TaggedByMe Property`**
  ```csharp
  public bool TaggedByMe { get; }
  ```
  Gets a value indicating whether the tagged by me.

- **`TaggedByOther Property`**
  ```csharp
  public bool TaggedByOther { get; }
  ```
  Gets a value indicating whether the tagged by other.

- **`ThreatInfo Property`**
  ```csharp
  public UnitThreatInfo ThreatInfo { get; }
  ```
  Returns detailed threat information for this unit towards the local player.

- **`TotalAbsorbs Property`**
  ```csharp
  public int TotalAbsorbs { get; }
  ```
  Gets the total amount of damage this unit can absorb before losing health.

- **`Tracked Property`**
  ```csharp
  public bool Tracked { get; }
  ```
  Gets a value indicating whether the tracked.

- **`Transport Property`**
  ```csharp
  public WoWObject Transport { get; }
  ```
  Returns a WoWObject of the current transport or null if not on a
            transport.

- **`TransportGuid Property`**
  ```csharp
  public WoWGuid TransportGuid { get; }
  ```
  Returns the transport guid of this unit.

- **`VanityPet Property`**
  ```csharp
  public WoWUnit VanityPet { get; }
  ```
  Gets the vanity pet.

- **`VanityPetGuid Property`**
  ```csharp
  public WoWGuid VanityPetGuid { get; }
  ```
  Gets the vanity pet guid.

- **`VehicleInfo Property`**
  ```csharp
  public WoWVehicle VehicleInfo { get; }
  ```
  Gets a wrapper object containing additional vehicle information on this unit.

#### WoWUnit Methods

- **`Behind Method`**
  ```csharp
  public bool Behind(
WoWUnit obj
)
  ```
  Returns true if passed in object is behind this unit.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST769E3246_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe obj.
  - **Returns:** ReferenceWoWUnit Class

- **`CanAttackNow Method`**
  ```csharp
  public bool CanAttackNow(
WoWUnit other
)
  ```
  Determines whether this instance can attack the specified unit right now.
  - *other*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST74867818_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe other unit.
  - **Returns:** ReferenceWoWUnit Class

- **`Face Method`**
  ```csharp
  public void Face()
  ```
  Face a target.

- **`GetAggroRange Method`**
  ```csharp
  public float GetAggroRange(
WoWUnit to
)
  ```
  Gets the approximate aggro range of this unit towards another unit.
  - *to*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST551F1259_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe "To" unit.
  - **Returns:** ReferenceWoWUnit Class

- **`GetAllAuras Method`**
  ```csharp
  public WoWAuraCollection GetAllAuras()
  ```
  Gets all auras as a collection.
  - **Returns:** It is generally more useful to use the Auras dictionary instead, if multiple lookups by name have to be performed.This collection is cached on a per-frame basis and used internally when aura dictionaries are queried.

- **`GetAuraById Method`**
  ```csharp
  public WoWAura GetAuraById(
int id
)
  ```
  Gets an aura by id. If the return value is null, this unit does not have the aura.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST7032ADF9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The ID of the aura.
  - **Returns:** ReferenceWoWUnit Class

- **`GetAuraByName Method`**
  ```csharp
  public WoWAura GetAuraByName(
string name
)
  ```
  Gets an aura by name. If the return value is null, this unit does not have the aura.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LSTF35A1602_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe name of the aura.
  - **Returns:** Using this function avoids constructing and caching a dictionary which would be done when using the Auras dictionary.

- **`GetCachedInfo Method`**
  ```csharp
  public bool GetCachedInfo(
out WoWCache.CreatureCacheEntry info
)
  ```
  Gets the cached info for this unit. A return value indicates whether this unit has
            cached info.
  - *info*: Type: Styx.WoWInternals.WoWCacheAddLanguageSpecificTextSet("LST4F4CC4E1_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWCacheAddLanguageSpecificTextSet("LST4F4CC4E1_3?cs=.|vb=.|cpp=::|nu=.|fs=.");CreatureCacheEntryAddLanguageSpecificTextSet("LST4F4CC4E1_4?cpp=%");[out].
  - **Returns:** ReferenceWoWUnit Class

- **`GetCurrentPower Method`**
  ```csharp
  public uint GetCurrentPower(
WoWPowerType type
)
  ```
  Gets a current power.
  - *type*: Type: StyxAddLanguageSpecificTextSet("LST4FF6CF4F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPowerTypeThe type.
  - **Returns:** ReferenceWoWUnit Class

- **`GetCurrentPowerDecimal Method`**
  ```csharp
  public double GetCurrentPowerDecimal(
WoWPowerType type
)
  ```
  Gets a current power.
  - *type*: Type: StyxAddLanguageSpecificTextSet("LSTE7012FDE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPowerTypeThe power type.
  - **Returns:** Certain powers can be fractional; for example it is possible to have 2.5 burning embers, or 50.5 rage.

- **`GetCursorOverride Method`**
  ```csharp
  public string GetCursorOverride()
  ```
  Gets the cursor override for this unit.
  - **Returns:** See Also

- **`GetMark Method`**
  ```csharp
  public RaidTargetMarker GetMark()
  ```
  Gets the mark.
  - **Returns:** ReferenceWoWUnit Class

- **`GetMaxPower Method`**
  ```csharp
  public uint GetMaxPower(
WoWPowerType type
)
  ```
  Gets a maximum power.
  - *type*: Type: StyxAddLanguageSpecificTextSet("LST9E3ABC7C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPowerTypeThe type.
  - **Returns:** ReferenceWoWUnit Class

- **`GetMaxPowerDecimal Method`**
  ```csharp
  public double GetMaxPowerDecimal(
WoWPowerType type
)
  ```
  Gets a maximum power.
  - *type*: Type: StyxAddLanguageSpecificTextSet("LSTF8803971_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPowerType
  - **Returns:** ReferenceWoWUnit Class

- **`GetMeleeRangeOf Method`**
  ```csharp
  public float GetMeleeRangeOf(
WoWUnit otherUnit
)
  ```
  Gets the melee range of otherUnit as this unit;
            that is, the range otherUnit needs to be in to melee attack this unit.
  - *otherUnit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST452EC882_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe other unit.
  - **Returns:** Exceptions

- **`GetPowerCostModifier Method`**
  ```csharp
  public uint GetPowerCostModifier(
WoWPowerType type
)
  ```
  Gets a power cost modifier.
  - *type*: Type: StyxAddLanguageSpecificTextSet("LSTDD0D2CCE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPowerTypeThe type.
  - **Returns:** ReferenceWoWUnit Class

- **`GetPowerCostMultiplier Method`**
  ```csharp
  public float GetPowerCostMultiplier(
WoWPowerType type
)
  ```
  Gets a power cost multiplier.
  - *type*: Type: StyxAddLanguageSpecificTextSet("LSTB58ADF18_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPowerTypeThe type.
  - **Returns:** ReferenceWoWUnit Class

- **`GetPowerInfo Method`**
  ```csharp
  public WoWUnit.PowerInfo GetPowerInfo(
WoWPowerType type
)
  ```
  Gets a power information.
  - *type*: Type: StyxAddLanguageSpecificTextSet("LST3F1E7912_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPowerTypeThe type.
  - **Returns:** See Also

- **`GetPowerPercent Method`**
  ```csharp
  public float GetPowerPercent(
WoWPowerType type
)
  ```
  Gets a power percent.
  - *type*: Type: StyxAddLanguageSpecificTextSet("LST21CCE211_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPowerTypeThe type.
  - **Returns:** ReferenceWoWUnit Class

- **`GetPowerRegenFlat Method`**
  ```csharp
  public float GetPowerRegenFlat(
WoWPowerType type
)
  ```
  Gets a power regen flat.
  - *type*: Type: StyxAddLanguageSpecificTextSet("LST8FC86C6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPowerTypeThe type.
  - **Returns:** ReferenceWoWUnit Class

- **`GetPowerRegenInterrupted Method`**
  ```csharp
  public float GetPowerRegenInterrupted(
WoWPowerType type
)
  ```
  Gets a power regen interrupted.
  - *type*: Type: StyxAddLanguageSpecificTextSet("LSTE6ACB03_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWPowerTypeThe type.
  - **Returns:** ReferenceWoWUnit Class

- **`GetPredictedHealth Method`**
  ```csharp
  public uint GetPredictedHealth(
bool includeMyHeals = false
)
  ```
  Returns the health the unit is predicted to be at after heals are finished.
  - **Returns:** ReferenceWoWUnit Class

- **`GetPredictedHealthPercent Method`**
  ```csharp
  public float GetPredictedHealthPercent(
bool includeMyHeals = false
)
  ```
  Returns the health percent the unit is predicted to be at after heals are finished.
  - **Returns:** ReferenceWoWUnit Class

- **`GetReactionTowards Method`**
  ```csharp
  public WoWUnitReaction GetReactionTowards(
WoWUnit otherUnit
)
  ```
  Gets the reaction of this unit towards another unit.
  - *otherUnit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST73BA2F2B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe other unit.
  - **Returns:** ExceptionConditionArgumentNullException    Thrown if otherUnit is null.InvalidOperationExceptionThrown if this unit is invalid or if
            otherUnit is invalid.

- **`GetStealthDetectionRange Method`**
  ```csharp
  public float GetStealthDetectionRange(
WoWUnit to
)
  ```
  Gets the approximate range at which this unit will be detected by the passed in unit.
  - *to*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST3F61F36D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe "To" unit.
  - **Returns:** ReferenceWoWUnit Class

- **`GetThreatInfoFor Method`**
  ```csharp
  public UnitThreatInfo GetThreatInfoFor(
WoWUnit otherUnit
)
  ```
  Returns detailed threat information for this unit towards the passed in unit.
  - *otherUnit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST282643F5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe other unit.
  - **Returns:** ExceptionConditionArgumentNullExceptionThrown if otherUnit is null.

- **`GetTraceLinePos Method`**
  ```csharp
  public Vector3 GetTraceLinePos()
  ```
  Returns a point more suitable for usage with TraceLine.
  - **Returns:** ReferenceWoWUnit Class

- **`HasAura Method`**
  Returns a boolean indicating whether this unit has an aura.

- **`HasAura Method (Int32)`**
  ```csharp
  public bool HasAura(
int id
)
  ```
  Returns a boolean indicating whether this unit has an aura.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LSTDDF7F49F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The id of the aura.
  - **Returns:** See Also

- **`HasAura Method (String)`**
  ```csharp
  public bool HasAura(
string name
)
  ```
  Returns a boolean indicating whether this unit has an aura.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LST6D6EA8F4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe English name of the aura.
  - **Returns:** See Also

- **`IncomingHealsArray Method`**
  ```csharp
  public IncomingHeal[] IncomingHealsArray()
  ```
  Returns the Array of Heals this unit is about to receive.
  - **Returns:** See Also

- **`IncomingHealsCount Method`**
  ```csharp
  public int IncomingHealsCount()
  ```
  Returns the number of heals this unit is about to receive.
  - **Returns:** ReferenceWoWUnit Class

- **`IsWithinMeleeRangeOf Method`**
  ```csharp
  public bool IsWithinMeleeRangeOf(
WoWUnit otherUnit
)
  ```
  Gets a bool that indicates whether this unit is within melee range of another unit;
            that is, if this unit is within the melee range of otherUnit.
  - *otherUnit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST5ADA0389_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWUnitThe other unit.
  - **Returns:** See Also

- **`KnowsSpell Method`**
  ```csharp
  [ObsoleteAttribute("Use SpellManager.HasSpell instead")]
public bool KnowsSpell(
int id
)
  ```
  Checks if this unit knows a spell specified by an id. This will only be valid for the local player, and the local player's pet.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST83AAFE1A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The ID of the spell.
  - **Returns:** ReferenceWoWUnit Class

- **`Mark Method`**
  ```csharp
  public void Mark(
RaidTargetMarker mark
)
  ```
  Marks the given mark.
  - *mark*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTB60720AD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");RaidTargetMarkerThe mark.

- **`Target Method`**
  ```csharp
  public void Target()
  ```
  Target's this unit.

---

### WoWUnit.PowerInfo Structure

```csharp
public struct PowerInfo
```

Information about the power.

#### PowerInfo Properties

- **`CostModifier Property`**
  ```csharp
  public uint CostModifier { get; }
  ```
  Returns the cost modifier.

- **`CostMultiplier Property`**
  ```csharp
  public float CostMultiplier { get; }
  ```
  Returns the cost multiplier.

- **`Current Property`**
  ```csharp
  public uint Current { get; }
  ```
  Gets the current.

- **`CurrentDecimal Property`**
  ```csharp
  public double CurrentDecimal { get; }
  ```
  Gets the current power as a decimal value.

- **`CurrentI Property`**
  ```csharp
  public int CurrentI { get; }
  ```
  Gets the current value, as an integer.

- **`Max Property`**
  ```csharp
  public uint Max { get; }
  ```
  Returns the max power.

- **`MaxDecimal Property`**
  ```csharp
  public double MaxDecimal { get; }
  ```
  Gets the max power as a decimal value.

- **`Percent Property`**
  ```csharp
  public float Percent { get; }
  ```
  Returns the current power percent.

- **`RegenFlatModifier Property`**
  ```csharp
  public float RegenFlatModifier { get; }
  ```
  Returns the flat modifier.

- **`RegenInterruptedFlatModifier Property`**
  ```csharp
  public float RegenInterruptedFlatModifier { get; }
  ```
  Returns the regen interrupted flat modifier.

- **`Type Property`**
  ```csharp
  public WoWPowerType Type { get; }
  ```
  Gets the type.

#### PowerInfo Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceWoWUnitAddLanguageSpecificTextSet("LST6CFD10A7_2?cs=.|vb=.|cpp=::|nu=.|fs=.");PowerInfo Structure

---
