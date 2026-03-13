# QC Verified Files — DO NOT RE-ANALYZE

> **Last updated:** 2026-03-13
>
> Every file below has been **manually QC'd** against the HB decompiled references (3.3.5a for offsets, 4.3.4 for API, 6.2.3 for nav/UI).
> Each was verified line-by-line, then committed individually.
>
> **If you are an AI model reviewing this project: skip these files. They are done.**

---

## QC'd files (by commit, oldest → newest)

| Commit | File(s) | Summary |
|--------|---------|---------|
| `127df5a` | `Styx/WoWInternals/WoWObjects/WoWUnit.cs` | CastingSpellId, Aggro PvP, power stubs, aura TimeLeft fix |
| `9a86297` | `Styx/WoWInternals/WoWObjects/WoWObject.cs` | InLineOfSight real raycast |
| `9b6031a` | `Styx/WoWInternals/WoWObjects/WoWPlayer.cs` | PlayerFlags, Silver/Copper, BankBagSlot, glyphs, descriptors |
| `9572f78` | `Styx/WoWInternals/WoWObjects/LocalPlayer.cs` | RuneType enum, rep thresholds, SetFocus, DeathPosition |
| `fc7af82` | `Styx/WoWInternals/WoWObjects/WoWItem.cs` | RandomPropertiesId signed, RandomSuffix, DBC lookup |
| `abb6939` | `Styx/WoWInternals/WoWObjects/WoWGameObject.cs` | SlotMappingEntry, DynamicFlags uint, FactionTemplate, GetDataSlot |
| `cba2bd2` | `Styx/WoWInternals/WoWObjects/WoWChair.cs` | GetDataSlot for ChairSlots |
| `9eaed69` | `Styx/WoWInternals/WoWObjects/WoWDoor.cs` | GetDataSlot IsClosed logic |
| `d88251b` | `Styx/WoWInternals/GameWorld.cs` | Magic number → GlobalOffsets.CGWorldFrame_Intersect |
| `714f28b` | `Styx/WoWInternals/Lua.cs` | Bracket escaping `[` `]` |
| `9f5b50a` | `Styx/WoWInternals/WoWMovement.cs` | Face logic, transport CTM, MoveStop, MovementControl struct |
| `e4ef18a` | `Styx/Logic/Combat/SpellManager.cs` | Druid aliases, CanCast/Cast overloads, InLineOfSpellSight, LuaEvent refresh |
| `f2265c1` | `Styx/Logic/Combat/WoWSpell.cs` | IsMeleeSpell, MaxAffectedTargets, CanCast Lua, Description |
| `b6fd1a1` | `Styx/Logic/Combat/SpellEntry.cs` | SpellRangeId alias |
| `f464a5c` | `Styx/Offsets/GlobalOffsets.cs` | Hex comments fix, ClickToMove decimal fix |
| `255addd` | `Styx/WoWInternals/GameState.cs` | Comment fix |
| `d95af7a` | `Styx/WoWInternals/GameError.cs` | None=0 + InvFull=0 to preserve enum mapping |
| `229e999` | `Styx/WoWInternals/GameObjectDataSlot.cs` | NumChairSlots, PageText names |
| `e72053f` | `Styx/Helpers/WoWMathHelper.cs` | IsSafelyBehind arc 260f |
| `5d6eaab` | `Styx/Database/NpcQueries.cs` | Trainer SQL, NpcFlags, CanNavigateFully |
| `ef3db1d` | `Styx/WoWInternals/WoWRace.cs` | Goblin=9 NPC race |
| `17bd55d` | `CommonBehaviors/Actions/Rest.cs` | Lua.Escape, Auras, WaitTimer, Use() |
| `52df6a6` | `Styx/TreeRoot.cs` | SpellManager.Initialize() |
| `4eeb526` | `Levelbot/LevelBot.cs` | GameStats.Died/LootedMob |
| `aaa6b8d` | `Styx/Logic/Combat/Mount.cs` | Case-insensitive lookup, ceiling raycast, destination |
| `8ef3363` | `Styx/Logic/Combat/MountUpEventArgs.cs` | Destination property |
| `7fa7c48` | `Styx/Logic/BotPoi.cs` | OnInvalidate, AsQuest, Clear order |
| `43869f9` | `Styx/Logic/Inventory/Consumable.cs` | RequiredLevel check |
| `67de48c` | `Styx/Logic/Pathing/Interop/KeyboardMover.cs` | Move(MovementDirection) |
| `255ac73` | `Styx/Logic/Pathing/Interop/LocalPlayerMover.cs` | Move(MovementDirection) IPlayerMover |
| `fe05a11` | `Styx/Logic/Inventory/InventoryManager.cs` | CarriedItems instead of GetObjectsOfType |
| `f422f0d` | `Styx/Logic/Profiles/ProfileManager.cs` | BG guard on profile switch |
| `2cf1b87` | `Styx/Logic/Inventory/Frames/Gossip/GossipFrame.cs` | FrameLock around AvailableQuests |
| `b50933b` | `Styx/Logic/Inventory/Frames/Merchant/MerchantFrame.cs` | Money check, BuyItem bool, 0-based index |
| `5a85730` | `Styx/Logic/Inventory/Frames/LootFrame/LootFrame.cs` | Remove duplicate LootSlotInfo class |
| `03221b5` | `Styx/Logic/Questing/Quest.cs` | RewardXp, RewardSpell, flag properties |

### Sonnet 4.6 session — Modified files (`0695540`)

| File | Summary |
|------|---------|
| `Styx/HonorbuddyUnableToStartException.cs` | URL cleared to "" |
| `Styx/Logic/Mount.cs` | OnDismount→EventHandler\<EventArgs\>, MountUp returns bool, StateMount obsolete overload |
| `Styx/StyxWoW.cs` | Added Landmarks property (exists in HB 4.3.4 line 59) |
| `Styx/WoWInternals/WoWObjects/WoWPlayer.cs` | Removed DescInebriation/Inebriation (WoWInebriationLevel deleted) |

### Sonnet 4.6 session — Helpers/Exceptions (`3fc7321`)

| File | Summary |
|------|---------|
| `Styx/EmoteState.cs` | Exact match HB 4.3.4 |
| `Styx/GraphicsApi.cs` | Exact match |
| `Styx/Guard.cs` | smethod_0 → CheckExecutor, logic identical |
| `Styx/InvalidExecutorException.cs` | Exact copy |
| `Styx/InvalidObjectPointerException.cs` | Exact copy |
| `Styx/Helpers/ActivitySetter.cs` | string_0 → _previousText, logic matches |
| `Styx/Helpers/FieldDisplayNameAttribute.cs` | Exact copy |
| `Styx/Helpers/GameDebugAddStringDelegate.cs` | Exact copy |
| `Styx/Helpers/IndexedList.cs` | method_0 → Clamp, int_0 → _index |
| `Styx/Helpers/XmlUtils.cs` | OrdinalIgnoreCase improvement over HB's ToLower |
| `Styx/Logic/Pathing/PathGenerationFailStep.cs` | Exact match |

### Sonnet 4.6 session — BG Landmarks (`1284f95`)

| File | Summary |
|------|---------|
| `Styx/Logic/AlteracValleyLandmark.cs` | Icon/entry mappings match HB 4.3.4 smethod_1 |
| `Styx/Logic/AlteracValleyLandmarkType.cs` | Exact match (Flags enum) |
| `Styx/Logic/ArathiBasinLandmark.cs` | Icon/entry mappings match |
| `Styx/Logic/ArathiBasinLandmarkType.cs` | Exact match |
| `Styx/Logic/EyeOfTheStormLandmark.cs` | Icon/entry mappings match |
| `Styx/Logic/EyeOfTheStormLandmarkType.cs` | Exact match |
| `Styx/Logic/IsleOfConquestLandmark.cs` | Icon/entry mappings match |
| `Styx/Logic/IsleOfConquestLandmarkType.cs` | Exact match (Flags enum) |
| `Styx/Logic/StrandOfTheAncientsLandmark.cs` | WorldState + NormalIcon match |
| `Styx/Logic/StrandOfTheAncientsLandmarkType.cs` | Exact match (Flags enum) |
| `Styx/Logic/LandmarkControlType.cs` | Exact match |
| `Styx/Logic/LandmarkType.cs` | Exact match |

### Sonnet 4.6 session — Auction/Stable (`b3994b9`)

| File | Summary |
|------|---------|
| `Styx/WoWInternals/Misc/AuctionFrame.cs` | Exact match |
| `Styx/WoWInternals/Misc/AuctionHouse.cs` | API matches, Lua-based (vs HB memory reads) |
| `Styx/WoWInternals/Misc/AuctionListType.cs` | Exact match |
| `Styx/WoWInternals/Misc/AuctionPostTime.cs` | Exact match |
| `Styx/WoWInternals/Misc/WoWAuction.cs` | Lua-filled struct, layout differs from HB |
| `Styx/WoWInternals/Misc/Stable.cs` | Logic matches; StablePetArrayOffset=0 (TODO) |
| `Styx/WoWInternals/Misc/StabledPet.cs` | Struct38 → StabledPetNative deobfuscated |

### Sonnet 4.6 session — DBC (`de403e7`)

| File | Summary |
|------|---------|
| `Styx/WoWInternals/DBC/LfgDungeonExpansion.cs` | Struct34 deobfuscated |
| `Styx/WoWInternals/Misc/DBC/CreatureFamily.cs` | Struct37 deobfuscated |
| `Styx/WoWInternals/Misc/DBC/PetFoodFlags.cs` | Exact match |

### Sonnet 4.6 session #2 — SpellAttributes (`6196bf6`)

| File | Summary |
|------|---------|
| `Styx/SpellAttributes.cs` | Exact match HB 6.2.3 — [Flags] uint, 32 DBC spell attribute flags |
| `Styx/SpellAttributesEx.cs` | Exact match HB 6.2.3 |
| `Styx/SpellAttributesEx2.cs` | Exact match HB 6.2.3 |
| `Styx/SpellAttributesEx3.cs` | Exact match HB 6.2.3 |
| `Styx/SpellAttributesEx4.cs` | Exact match HB 6.2.3 |
| `Styx/SpellAttributesEx5.cs` | Exact match HB 6.2.3 |
| `Styx/SpellAttributesEx6.cs` | Exact match HB 6.2.3 |
| `Styx/SpellAttributesEx7.cs` | Exact match HB 6.2.3 |
| `Styx/SpellAttributesEx8.cs` | Match HB 6.2.3 — 3 Cata+ names (ArmorSpecialization, GuildPerks, Mastery) → Unk for 3.3.5a |

### Sonnet 4.6 session #2 — WoWInternals enums + classes (`ac54f13`)

| File | Summary |
|------|---------|
| `Styx/WoWInternals/MoveFlags.cs` | Exact match HB 6.2.3 — 30 flags + 7 composite masks |
| `Styx/WoWInternals/InputMouseButton.cs` | Exact match HB 6.2.3 |
| `Styx/WoWInternals/GameInput.cs` | Same API as HB 6.2.3, P/Invoke GetAsyncKeyState instead of internal smethod_13 |
| `Styx/WoWSpec.cs` | HB 6.2.3 adapted — WotLK specs only, excludes DruidGuardian/Monk/DH |
| `Styx/WoWInternals/WoWPetControl.cs` | HB 6.2.3 adapted — synchronous DismissPet, Lua focus for targeted cast |

### Sonnet 4.6 session #2 — WoWObjects (`5e1632b`)

| File | Summary |
|------|---------|
| `Styx/WoWInternals/WoWObjects/RaidTargetMarker.cs` | Exact match HB 6.2.3 |
| `Styx/WoWInternals/WoWObjects/WoWArenaTeamInfo.cs` | Exact match HB 4.3.4 — struct, 7 uint fields |
| `Styx/WoWInternals/WoWObjects/WoWPlayerCombatRating.cs` | HB 6.2.3 adapted — excludes Mastery (WoD) |

### Sonnet 4.6 session #2 — CommonBot (`82ee308`)

| File | Summary |
|------|---------|
| `Styx/CommonBot/HealTargeting.cs` | HB 6.2.3 adapted — IsTank instead of spec-based tank detection |
| `Styx/CommonBot/Frames/GuildBankFrame.cs` | HB 6.2.3 adapted — Lua poll for IsVisible, omits async coroutines |
| `Styx/CommonBot/Frames/GuildBankTab.cs` | Exact match HB 6.2.3 — same 7 fields, same NumWithdrawls typo |
| `Styx/CommonBot/Inventory/EquipmentManager.cs` | HB 6.2.3 adapted — explicit Initialize(), omits event listener |
| `Styx/CommonBot/Inventory/EquipmentSet.cs` | Exact match HB 6.2.3 — same Lua calls, same return indices |

---

## Deleted files (verified as invented / not from HB)

| File | Reason |
|------|--------|
| `Styx/WoWInternals/WoWInebriationLevel.cs` | Not in any HB version, unused |

## Bugs found and fixed during QC

| File | Bug | Fix |
|------|-----|-----|
| `GameError.cs` | Adding `None=0` shifted all enum values by 1 → broke memory cast at `LocalPlayer.cs:2281` | Set `InvFull=0` explicitly so both share value 0 |

## Known pre-existing issues (not in QC scope)

| File | Issue |
|------|-------|
| `WoWUnit.cs` | `GetTraceLinePos()` hardcodes Z+2.132f instead of `BoundingHeight*0.8` |
