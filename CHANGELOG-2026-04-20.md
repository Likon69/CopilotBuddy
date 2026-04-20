UPDATE 3
==========================

Bug fixes:
--------------
  * WoWObjects
    - WoWContainer: null-safe Bag delegates — Slots, UsedSlots, FreeSlots, ItemGuids, Items all guard against null Bag (HB 4.3.4 has same bug)
    - WoWGameObject: fixed WoWGameObjectState enum — Active=0, Ready=1 (was inverted); CanGather now checks State==Ready for herbs/minerals to skip already-harvested nodes; CanMine/CanHarvest no longer double-check State (moved into CanGather); RequiredSkill==0 treated as no requirement
    - LocalPlayer: removed hardcoded SkillsBasePtr (0xBE55C8) — GetSkill/GetAllSkills now use GetSkillByIndex() which reads from player descriptors (correct per-character data instead of static address)

  * Questing
    - QuestLog.GetQuestInfo: fixed offset formula — was (158 + index * 5 * 4), now ((158 + index * 5) * 4) to match ReadDescriptor pattern (HB 4.3.4 has same bug)
    - ActionSelectReward: ported HB 4.3.4 WeightSetEx.EvaluateItem(itemInfo, itemStats) scoring with ItemStats parsed from item link; vendor sell-price fallback for non-equippable items (replaces custom AutoEquipper.EvaluateRewards)

  * Character Management
    - AutoEquipper.EvaluateItem: dual-slot comparison for Finger1/Finger2 and Trinket1/Trinket2 via GetSecondaryItemSlot(); null guard on StyxWoW.Me.Inventory (HB 4.3.4 and 6.2.3 both check both slots)

  * Profile Parsing
    - HotspotManager: use InvariantCulture for float.TryParse on X/Y/Z hotspot coordinates — fixes comma-vs-dot locale issues on non-English systems

  * WoWDb
    - Added diagnostic logging on DBC table load (table count, first 5 keys, Lock DBC accessibility check)

  * GatherBuddy
    - GatherbuddyBot: added SleepForLag after Interact before WaitContinue(!IsCasting) — server needs time to start cast channel
    - GatherbuddyBot: HB 4.3.4 mounted combat — _combatSuppressed flag, keep moving when mounted+combat, only fight when dismounted
    - GatherbuddyBot: fixed node oscillation — _lockedNodeLocation cached on approach, gather gate accepts locked node, approach uses fallback location
    - GatherbuddyBot: fixed NRE on LootTargeting.Instance.FirstObject null check in branch[0]
    - GatherbuddyBot: path distance ratio filter — nodes >30y with pathDist >3.5× straight-line distance skipped in IncludeLootFilter (avoids cave nodes)
    - GatherbuddyBot: CycleToNearestWaypoint called in OnGatherComplete and combat lock release (prevents backtracking to start of route)
    - GatherbuddyBot: auto-vendor Data.bin fallback — syncs FindVendorsAutomatically to CharacterSettings at Start(); NeedsToSell/NeedsToRepair now check profile vendor → ObjectManager → NpcQueries (Data.bin); CreateNearbyVendorBehavior navigates to Data.bin vendor when none in render range
    - GatherbuddyBot: auto-find mailbox — NeedsToMail checks profile mailbox → ObjectManager scan for WoWGameObjectType.Mailbox; CreateMailBehavior uses same fallback chain
    - New helpers: FindVendorFromDatabase(UnitNPCFlags) wraps NpcQueries.GetNearestNpc; FindNearbyMailbox() scans ObjectManager for nearby mailbox GameObjects

