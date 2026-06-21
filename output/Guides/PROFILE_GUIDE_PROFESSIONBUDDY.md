# ProfessionBuddy Profile Guide — WotLK 3.3.5a

## What ProfessionBuddy is for

ProfessionBuddy (PB) automates **crafting and processing** professions: Alchemy, Blacksmithing, Cooking, Enchanting, Engineering, First Aid, Fishing, Inscription, Jewelcrafting, Leatherworking, Tailoring, and processing sub-skills (Smelting, etc.).

**Gathering professions (Mining nodes, Herbalism, Skinning) are handled by GatherBuddy**, not ProfessionBuddy. PB can call `SpellManager.Cast("Smelting")` because Smelting is an actual cast. It cannot "mine a node" — that belongs to GatherBuddy.

---

## Profile Structure

A profile is an XML file with a root `<Professionbuddy>` element. Actions execute top-to-bottom. The `ChildrenCount` attribute is **not needed** — the parser counts children from the XML structure automatically. Including it produces harmless debug warnings; omit it.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Professionbuddy>
  <!-- actions here -->
</Professionbuddy>
```

---

## Available Actions

### MoveToAction
Move to a world position or to the nearest smart target.
```xml
<!-- Move to explicit coordinates -->
<MoveToAction MoveType="Location" Pathing="Navigator" Entry="0"
    X="1234.5" Y="-567.8" Z="89.0" />

<!-- Move to nearest vendor (no coordinates needed) -->
<MoveToAction MoveType="NearestVendor" Pathing="Navigator" Entry="0" />

<!-- Move to a specific NPC by Entry ID -->
<MoveToAction MoveType="NpcByID" Pathing="Navigator" Entry="5493"
    X="-8802.0" Y="770.9" Z="96.3" />
```

`MoveType` values:

| Value | Description |
|-------|-------------|
| `Location` | Move to the given X/Y/Z coordinates |
| `NearestVendor` | Nearest general vendor |
| `NearestMailbox` | Nearest mailbox |
| `NearestFlight` | Nearest flight master |
| `NearestAH` | Nearest auction house |
| `NearestRepair` | Nearest repair NPC |
| `NearestReagentVendor` | Nearest reagent vendor |
| `NearestBanker` | Nearest bank |
| `NearestGB` | Nearest guild bank |
| `NpcByID` | A specific NPC by Entry ID (use with coordinates) |

`Pathing` values: `Navigator` (pathfinding), `ClickToMove` (direct line, no pathfinding).

For `NearestXxx` types, coordinates are optional — the bot finds the closest matching target automatically.

### CustomAction
Run arbitrary C# code inline. Has access to the full CodeDriver context (see below).
```xml
<CustomAction Code="Log(&quot;Hello&quot;);" />
```

### WaitAction
Wait for a fixed duration (ms), or until a condition becomes true.
```xml
<!-- Always wait 2 seconds -->
<WaitAction Condition="false" Timeout="2000" />
<!-- Wait until out of combat, up to 10s -->
<WaitAction Condition="!Me.IsInCombat" Timeout="10000" />
<!-- Wait until spell cast finishes, up to 15s -->
<WaitAction Condition="!Me.IsCasting" Timeout="15000" />
```

### If
Execute children only when Condition is true.
```xml
<If Condition="Cooking.Level &lt; 75">
  <CustomAction Code="SpellManager.Cast(&quot;Cook&quot;);" />
</If>
```

### While
Loop children while Condition is true.
```xml
<While Condition="Cooking.Level &lt; 300" IgnoreCanRun="True">
  <!-- loop body -->
</While>
```

### CallSubRoutine / SubRoutine
Define and call reusable blocks. SubRoutines must be at the **root level** of the profile — not nested inside other actions.
```xml
<CallSubRoutine SubRoutineName="VendorAndTrain" />

<SubRoutine SubRoutineName="VendorAndTrain">
  <TrainSkillAction NpcEntry="1234" X="0.0" Y="0.0" Z="0.0" />
  <CustomAction Code="Lua.DoString(&quot;CloseTrainer()&quot;);" />
  <WaitAction Condition="false" Timeout="1500" />
</SubRoutine>
```

### TrainSkillAction
Walk to a trainer NPC and train all available ranks.
```xml
<TrainSkillAction NpcEntry="5493" X="-8802.0" Y="770.9" Z="96.3" />
```
Always follow with `Lua.DoString("CloseTrainer()")` and a short `WaitAction`.

### BuyItemAction
Walk to a vendor NPC and buy a specific item.
```xml
<BuyItemAction NpcEntry="5494" X="-8803.2" Y="766.7" Z="96.3"
    ItemID="6256" Count="1" BuyItemType="SpecificItem" BuyAdditively="True" />
```

`BuyItemType` values: `SpecificItem` (by Item ID), `Material` (buy by material name for crafting needs).

Always follow with `Lua.DoString("CloseMerchant()")` and a short `WaitAction`.

### SellItemAction
Walk to a vendor NPC and sell items.
```xml
<!-- Sell specific item IDs (comma-separated) -->
<SellItemAction NpcEntry="5494" X="-8803.2" Y="766.7" Z="96.3"
    ItemID="858,6309" Count="0" SellItemType="Specific" />

<!-- Sell all grey quality items -->
<SellItemAction NpcEntry="5494" X="-8803.2" Y="766.7" Z="96.3"
    ItemID="0" Count="0" SellItemType="Greys" />

<!-- Sell all white quality items -->
<SellItemAction NpcEntry="5494" X="-8803.2" Y="766.7" Z="96.3"
    ItemID="0" Count="0" SellItemType="Whites" />

<!-- Sell all green quality items -->
<SellItemAction NpcEntry="5494" X="-8803.2" Y="766.7" Z="96.3"
    ItemID="0" Count="0" SellItemType="Greens" />
```

`SellItemType` values: `Specific`, `Greys`, `Whites`, `Greens`.

### Comment
A label. Does nothing at runtime, useful for readability.
```xml
<Comment Text="Move to crafting area" />
```

---

## XML Escaping Rules

Inside `Code=""` or `Condition=""` attributes you must escape these characters:

| Character | Write as |
|-----------|----------|
| `<` | `&lt;` |
| `>` | `&gt;` |
| `&` | `&amp;` |
| `"` (string literal inside code) | `&quot;` |

Example:
```xml
<!-- SpellManager.Cast("Smelting") -->
<CustomAction Code="SpellManager.Cast(&quot;Smelting&quot;);" />

<!-- Cooking.Level < 300 -->
<If Condition="Cooking.Level &lt; 300">
```

---

## CodeDriver Context

Everything available directly inside `Code=` or `Condition=`:

| Symbol | Type | Description |
|--------|------|-------------|
| `Me` | `LocalPlayer` | The player character |
| `ObjectManager` | static | Access all in-range objects |
| `SpellManager.Cast(name)` | method | Cast a spell by name |
| `Lua.DoString(lua)` | method | Fire-and-forget Lua execution |
| `Lua.GetReturnValues(lua)` | `List<string>` | Lua call with return values |
| `Lua.GetReturnVal<T>(lua, index)` | `T` | Single Lua return value |
| `Log(format, args)` | method | Write to the HB log window |
| `TreeRoot.Stop()` | method | Stop the bot |
| `var1`–`var9` | `object` (static) | Scratch variables shared across actions |
| `HasItem(id)` | `bool` | True if item `id` is in any bag |
| `InbagCount(id)` | `int` | Count of item `id` across all bags |
| `InBankCount(id)` | `int` | Count of item `id` in the bank |
| `DistanceTo(x, y, z)` | `float` | Distance from player to world point |
| `MoveTo(x, y, z)` | `void` | Move to a world point (uses navigator) |
| `CTM(x, y, z)` | `void` | Click-to-move (direct line, no pathfinding) |

`var1`–`var9` are static objects that persist between consecutive actions in the same profile tick. Use them to pass an object found in one `CustomAction` to the condition of the next `If`.

---

## Tradeskill Helpers

All primary and secondary professions expose `.Level` and `.MaxLevel`:

```
Alchemy.Level          Alchemy.MaxLevel
Blacksmithing.Level    Blacksmithing.MaxLevel
Cooking.Level          Cooking.MaxLevel
Enchanting.Level       Enchanting.MaxLevel
Engineering.Level      Engineering.MaxLevel
FirstAid.Level         FirstAid.MaxLevel
Fishing.Level          Fishing.MaxLevel
Herbalism.Level        Herbalism.MaxLevel
Inscription.Level      Inscription.MaxLevel
Jewelcrafting.Level    Jewelcrafting.MaxLevel
Leatherworking.Level   Leatherworking.MaxLevel
Mining.Level           Mining.MaxLevel
Tailoring.Level        Tailoring.MaxLevel
```

`MaxLevel` = the current trained cap. Returns `0` if the profession is not yet learned — use this to detect a fresh character that needs training.

Skill cap thresholds in WotLK:

| Rank | Cap |
|------|-----|
| Apprentice | 75 |
| Journeyman | 150 |
| Expert | 225 |
| Artisan | 300 |
| Master | 375 |
| Grand Master | 450 |

---

## Common Patterns

### Stop at target skill level and log out
```xml
<If Condition="Cooking.Level &gt;= 300">
  <CustomAction Code="Log(&quot;Skill 300 reached! Logging out.&quot;); Lua.DoString(&quot;Logout()&quot;); TreeRoot.Stop();" />
</If>
```

### Train when at skill cap (before next rank is available)
```xml
<If Condition="Cooking.Level == Cooking.MaxLevel &amp;&amp; Cooking.MaxLevel != 450">
  <CallSubRoutine SubRoutineName="TrainAndVendor" />
</If>
```

### Vendor when bags are full
```xml
<!-- BagsFull includes ammo pouch / quiver slots -->
<If Condition="Me.BagsFull">
  <CallSubRoutine SubRoutineName="TrainAndVendor" />
</If>

<!-- NormalBagsFull ignores ammo/quiver bags -->
<If Condition="Me.NormalBagsFull">
  <CallSubRoutine SubRoutineName="TrainAndVendor" />
</If>
```

### Check if a profession is learned
```xml
<If Condition="Cooking.MaxLevel == 0">
  <CallSubRoutine SubRoutineName="TrainCooking" />
</If>
```

### Check item count in bags
```xml
<!-- True if at least 1 copy of item 2604 is in bags -->
<If Condition="HasItem(2604)">
  <CustomAction Code="Log(&quot;Reagents available&quot;);" />
</If>

<!-- Log the exact count -->
<CustomAction Code="Log(&quot;Count: &quot; + InbagCount(2604));" />
```

### Find and use an item in bags
```xml
<CustomAction Code="var1 = Me.BagItems.FirstOrDefault(i =&gt; i.Entry == 2604);" />
<If Condition="var1 != null">
  <CustomAction Code="((WoWItem)var1).UseContainerItem();" />
</If>
```

### Find a game object by Entry ID (bobber, chest, etc.)
```xml
<CustomAction Code="var1 = ObjectManager.GetObjectsOfType&lt;WoWGameObject&gt;().OrderBy(o =&gt; o.Distance).FirstOrDefault(o =&gt; o.Entry == 1731);" />
<If Condition="var1 != null &amp;&amp; ((WoWGameObject)var1).IsValid">
  <CustomAction Code="((WoWGameObject)var1).Interact();" />
</If>
```
Use Wowhead (filter for WotLK 3.3.5a) to find the Entry ID for your object type.

### Cast a crafting spell and wait for it to finish
```xml
<CustomAction Code="SpellManager.Cast(&quot;Smelting&quot;);" />
<WaitAction Condition="!Me.IsCasting" Timeout="15000" />
```

> **Note:** Gathering professions (Mining, Herbalism, Skinning) are **passive** — you do not cast them. Use GatherBuddy for node gathering. ProfessionBuddy handles crafting, processing (Smelting, Cooking, Alchemy, etc.), and fishing.

### Check equipped weapon slot
```xml
<If Condition="Me.Inventory.Equipped.MainHand == null ||
    Me.Inventory.Equipped.MainHand.ItemInfo.WeaponClass != WoWItemWeaponClass.FishingPole">
  <!-- equip logic -->
</If>
```

### Enable auto-loot
```xml
<CustomAction Code="Lua.DoString(&quot;SetCVar(\&quot;AutoLootDefault\&quot;, 1)&quot;);" />
```

---

## Lua Event Listener Pattern

Use a named frame to register a Lua event listener. The named frame prevents duplicate registrations if the bot restarts while WoW is still running (Lua globals survive a bot restart within the same WoW session).

```xml
<!-- Reset the log table unconditionally; create the frame only once per WoW session -->
<CustomAction Code="Lua.DoString(&quot;_MyLog = {}; if not _MyLogFrame then _MyLogFrame = CreateFrame('Frame'); _MyLogFrame:RegisterEvent('LOOT_OPENED'); _MyLogFrame:SetScript('OnEvent', function() for i = 1, GetNumLootItems() do local _, name = GetLootSlotInfo(i); if name then table.insert(_MyLog, name) end end end) end&quot;);" />
```

Read and flush after an interaction:
```xml
<WaitAction Condition="false" Timeout="500" />
<CustomAction Code="var r = Lua.GetReturnValues(&quot;local out = table.concat(_MyLog, '|'); _MyLog = {}; return out&quot;); if (r != null &amp;&amp; r.Count &gt; 0 &amp;&amp; !string.IsNullOrEmpty(r[0])) { foreach (string name in r[0].Split('|')) { if (!string.IsNullOrEmpty(name)) Log(&quot;Looted: &quot; + name); } }" />
```

> `GetLootSlotInfo(i)` returns `(texture, name, quantity, quality, locked)`.
> Always use `local _, name = GetLootSlotInfo(i)` — the first return value is the icon texture path, not the name.

---

## VendorAndTrain Subroutine Template

Copy-paste starting point for any crafting profile:

```xml
<SubRoutine SubRoutineName="VendorAndTrain">
  <!-- Train all available skill ranks -->
  <CustomAction Code="Log(&quot;Training skill...&quot;);" />
  <TrainSkillAction NpcEntry="TRAINER_NPC_ID" X="0.0" Y="0.0" Z="0.0" />
  <CustomAction Code="Lua.DoString(&quot;CloseTrainer()&quot;);" />
  <WaitAction Condition="false" Timeout="1500" />
  <!-- Vendor grey items -->
  <SellItemAction NpcEntry="VENDOR_NPC_ID" X="0.0" Y="0.0" Z="0.0"
      ItemID="0" Count="0" SellItemType="Greys" />
  <CustomAction Code="Lua.DoString(&quot;CloseMerchant()&quot;);" />
  <WaitAction Condition="false" Timeout="1500" />
</SubRoutine>
```

Replace `TRAINER_NPC_ID` / `VENDOR_NPC_ID` and coordinates with the actual NPC IDs from Wowhead (filter for WotLK build 12340).

---

## Tips

- **No `ChildrenCount` attribute.** The parser reads children from XML structure. Adding it generates harmless `[D]` debug warnings; omit it.
- **Profile loads twice per session** — once on UI selection, once on `Start()`. Debug messages appear twice. This is normal.
- **`IgnoreCanRun="True"`** on `While`/`If` prevents the behavior tree from skipping a node when a sibling already succeeded. Use it on most nodes inside loops.
- **Always close vendor/trainer windows** after `BuyItemAction` / `TrainSkillAction` with a Lua call + short `WaitAction`, or the next navigation move will be blocked.
- **X/Y/Z coordinates** are a legacy format the profile loader converts to the internal `Location` string. Both formats work identically.
- **PERF warnings** (`[D] Tick took Xms`) on first start are normal — the navigator loads map tiles for the current zone.

---

## API Reference

### C# CodeDriver — Complete Symbol List

Verified from `Dynamic/DynamicCodeCompiler.cs` (Prefix + Postfix).

#### Scratch Variables

```csharp
object var1, var2, var3, var4, var5, var6, var7, var8, var9
```

Static fields — persist across ticks within the same profile run. Use for cross-tick state (e.g. storing a target NPC GUID, a craft loop counter, a cooldown timestamp).

---

#### Player & Settings

| Symbol | Type | Description |
|--------|------|-------------|
| `Me` | `LocalPlayer` | The bot's character. Same as `ObjectManager.Me`. |
| `Settings` | `PbProfileSettings` | Profile-level settings (advanced). |
| `SecondaryBot` | `BotBase` | Currently active secondary bot. |
| `HasNewMail` | `bool` | True if there is unread mail in the mailbox. |
| `MailCount` | `int` | Total mail count. |
| `HasDataStoreAddon` | `bool` | True if the DataStore addon is installed and active. |
| `DataStore` | `DataStore` | Raw DataStore instance (cross-character data). |

---

#### Tradeskill Helpers (13 total)

Available as properties on `CodeDriver`: `Alchemy`, `Blacksmithing`, `Cooking`, `Enchanting`, `Engineering`, `FirstAid`, `Fishing`, `Inscription`, `Herbalism`, `Jewelcrafting`, `Leatherworking`, `Mining`, `Tailoring`.

Each `TradeskillHelper` exposes:

| Property | Type | Description |
|----------|------|-------------|
| `.Level` | `int` | Current skill points. |
| `.MaxLevel` | `int` | Current skill cap. `0` = profession not yet learned. |
| `.Name` | `string` | Localized skill name. |
| `.RecipeCount` | `int` | Number of recipes visible in the tradeskill UI. |
| `.KnownRecipes` | `Dictionary<uint, Recipe>` | Learned recipes keyed by spell ID. |
| `.Recipes` | `Dictionary<uint, Recipe>` | All recipes including unlearned. |

---

#### Craft / Recipe Helpers

All take a **spell ID** (not item ID). Get it from Wowhead → the tradeskill spell, not the crafted item.

| Method | Return | Description |
|--------|--------|-------------|
| `CanRepeatNum(uint spellId)` | `uint` | Times you can craft with current inventory. |
| `CanCraft(uint spellId)` | `bool` | True if you have mats + tools. |
| `HasMats(uint spellId)` | `bool` | True if material requirements are met. |
| `HasTools(uint spellId)` | `bool` | True if tool requirements are met. |
| `HasRecipe(uint spellId)` | `bool` | True if the recipe is known. |
| `RecipeIsOnCD(int spellId)` | `bool` | True if the recipe is on cooldown (e.g. transmutes). |

```csharp
// Example: craft as long as possible
if (CanRepeatNum(2819) > 0)
    SpellManager.Cast(2819u);   // 2819 = Smelt Copper
```

---

#### Inventory / Items

| Method | Return | Description |
|--------|--------|-------------|
| `HasItem(uint itemId)` | `bool` | True if item is in bags (alias for `InbagCount > 0`). |
| `InbagCount(uint itemId)` | `int` | Item count across all bags. |
| `InBankCount(uint itemId)` | `int` | Item count in the bank. |
| `InGBankCount(uint itemId)` | `int` | Item count in your guild bank. |
| `InGBankCount(string char, uint itemId)` | `int` | Guild bank count for another character (requires DataStore). |
| `OnAhCount(uint itemId)` | `int` | Items you have currently listed on the AH. |
| `OnAhCount(string char, uint itemId)` | `int` | AH listings for another character (requires DataStore). |

---

#### Movement

| Method | Description |
|--------|-------------|
| `MoveTo(double x, double y, double z)` | Navigate to world coords via the pathfinder. |
| `MoveTo(WoWPoint p)` | Navigate to a `WoWPoint`. |
| `CTM(double x, double y, double z)` | Click-to-move (no pathfinding, direct line). |
| `CTM(WoWPoint p)` | Click-to-move to a `WoWPoint`. |
| `DistanceTo(double x, double y, double z)` | Returns `float` distance to coords. |
| `DistanceTo(WoWPoint p)` | Returns `float` distance to a `WoWPoint`. |

---

#### Logging

```csharp
Log("Crafted {0} items", count);
Log(Color.Cyan, "Fish caught: {0}", itemName);
Log(Color.Green, "Status", Color.White, "Bags full — heading to vendor");
```

---

#### Bot Control

| Method | Description |
|--------|-------------|
| `SwitchToBot(string botName)` | Changes the secondary bot. E.g. `"Questing"`, `"Combat Grinding"`. |
| `SwitchCharacter(string char, string server, string bot)` | Logs out and switches to another character. |
| `RefreshDataStore()` | Re-imports DataStore addon data from Lua. |

---

#### Auto-Imported Namespaces

Inside `<Code>` blocks you do **not** need `using` statements — these are already injected:

```
System, System.Collections.Generic, System.Collections, System.Linq
System.Text, System.IO, System.Drawing, System.Reflection
System.Threading, System.Diagnostics, System.Windows.Forms
System.Xml.Linq
Styx, Styx.Helpers, Styx.Logic, Styx.Logic.Combat
Styx.WoWInternals, Styx.WoWInternals.WoWObjects
Styx.Logic.BehaviorTree, Styx.Logic.Pathing, Styx.Logic.Profiles
Styx.Logic.AreaManagement, Styx.Logic.Inventory.Frames.Gossip
Styx.Logic.Inventory.Frames.LootFrame, Styx.Logic.Inventory.Frames.MailBox
Styx.Logic.Inventory.Frames.Merchant, Styx.Plugins, Styx.Plugins.PluginClass
Styx.WoWInternals.World, Styx.Combat.CombatRoutine
HighVoltz, HighVoltz.Composites, HighVoltz.Dynamic
TreeSharp
```

---

### Lua — WotLK 3.3.5a Reference (Profile-Relevant Subset)

Full API at [wowwiki-archive.fandom.com/wiki/API](https://wowwiki-archive.fandom.com/wiki/World_of_Warcraft_API). Below are the functions most commonly needed in ProfessionBuddy profiles.

#### Items & Bags

```lua
-- How many of an item do you have (bags only)?
local count = GetItemCount(itemId)

-- Include bank? (only works when bank is open)
local count = GetItemCount(itemId, true)

-- Item quality / level info
local name, _, quality = GetItemInfo(itemId)
-- quality: 0=grey 1=white 2=green 3=blue 4=purple

-- Iterate bag slots
local numSlots = GetContainerNumSlots(bagIndex)  -- bagIndex: 0=backpack, 1-4=bags
local itemLink = GetContainerItemLink(bag, slot)
local texture, count, locked = GetContainerItemInfo(bag, slot)
local itemId = GetContainerItemID(bag, slot)
```

#### Tradeskills

```lua
-- Open the tradeskill window (required before GetNumTradeSkills)
CastSpellByName("Cooking")

-- Skill level
local name, header, rank, maxRank = GetTradeSkillLine()

-- Recipe list
local numRecipes = GetNumTradeSkills()
local recipeName, diffType, numAvailable = GetTradeSkillInfo(index)
-- diffType: "header" | "optimal" | "medium" | "easy" | "trivial" | "nodifficulty"

-- Craft one item (index from loop above)
DoTradeSkill(index)
DoTradeSkill(index, quantity)   -- craft N items
```

#### Loot

```lua
local numSlots = GetNumLootItems()
local texture, name, qty, quality, locked = GetLootSlotInfo(slotIndex)  -- 1-based
-- IMPORTANT: first return is icon path ("Interface\Icons\..."), NOT the name

LootSlot(slotIndex)
CloseLoot()
```

#### Merchant / Vendor

```lua
local numItems = GetMerchantNumItems()
local name, _, price, stackSize, numLeft = GetMerchantItemInfo(index)
local itemId = GetMerchantItemID(index)

BuyMerchantItem(index, quantity)
CloseMerchant()
```

#### Trainer

```lua
local count = GetNumTrainerServices()
local name, ttype, cost = GetTrainerServiceInfo(index)
-- ttype: "available" | "notavailable" | "used"

BuyTrainerService(index)
CloseTrainer()
```

#### Equipment

```lua
PickupInventoryItem(slotId)     -- pick item from equipped slot
-- Common slot IDs:
-- 16 = main hand, 17 = off hand, 22 = ranged/fishing pole/wand

EquipItemByName("item name")    -- equip item from bags by name
```

#### Spells & Casting

```lua
IsSpellKnown(spellId)           -- returns true/false
CastSpellByID(spellId)
CastSpellByName("Spell Name")

-- Check cooldown
local start, duration, enabled = GetSpellCooldown(spellId)
-- enabled=0 means GCD only; duration=0 means no cooldown
```

#### Player Info

```lua
local level = UnitLevel("player")
local hp, maxHp = UnitHealth("player"), UnitHealthMax("player")
local x, y = GetPlayerMapPosition("player")   -- 0.0-1.0 fractions
local now = GetTime()                          -- seconds since login (float)
```

---

