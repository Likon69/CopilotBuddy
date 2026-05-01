

UPDATE 5 — 2026-05-02
==========================

Custom / private server client support
--------------
  * Process attachment now works on custom clients with no version info (.rsrc-less EXE); build 0 accepted alongside 12340
  * Fixed crash on startup against LARGEADDRESSAWARE clients (Epoch/Ascension); memory reads with addresses > 0x7FFFFFFF no longer throw OverflowException

GatherBuddy — patrol efficiency
--------------
  * Patrol now adapts minimum flight height to the next waypoint; avoids the unnecessary descend/climb cycle between consecutive nodes at different altitudes

GatherBuddy — full rewrite (WoD port)
--------------
  * Rewrote gather loop against WoD reference; removed dead code and unused imports
  * Blacklist system cleaned up; public HashSet<WoWPoint> API preserved for plugin compatibility (DruidHarvestHelper etc.)
  * Approach point, gather attempt counter and combat suppression flag ported from WoD field names

Flightor / PolyNav
--------------
  * Ported PolyNav 2D path queue from WoD (smethod_10); replaces single straight-line CTM
  * Added ShouldWalk() — prefers ground nav when mount+fly time exceeds walk time
  * Added CanFly property (WotLK-adapted from 6.2.3)
  * Anti-stuck system ported from WoD smethod_14: checks position every 500 ms, tries ascend/strafe/back sequence before giving up
  * No-fly zone and elevator detection now redirect to ground nav instead of attempting to mount
  * Smart-Z on intermediate waypoints: stays at current altitude until within 200 m of destination
  * Added FlightorNavigation/ folder: PolyNav.cs (2D nav mesh), Areas.cs (zone area table), AerialBlackspotManager.cs (aerial obstacle blackspots) — ported from HB 6.2.3
  * Added FlightorAnnotation/IndoorEntrance.cs — annotated dismount points for indoor entrances (API port, not active at runtime for WotLK)

Navigator / Pathing
--------------
  * IsInNoFlyZone and IsRidingElevator checks added for Flightor redirect
  * Minor fixes to path smoothing and stuck detection

WoWObject / WoWUnit
--------------
  * Added IsOutdoors property to WoWObject — calls the 3.3.5a client IsOutdoors routine (0x71B7F0) via inline ASM; IsIndoors derived from it
  * Added IsFlying shortcut on WoWObject (delegates to MovementInfo)
  * WoWUnit: minor fixes

Tripper / AreaType
--------------
  * Added missing AreaType enum values (KnownBuilding, Misc4–Misc10) from HB 6.2.3 reference

Quest / RunCode
--------------
  * Added CompileExpressionAttribute, CompileStringAttribute, CustomBehaviorFileNameAttribute — used by quest behaviors with inline C# code blocks
  * Added IXmlObject interface — base contract for XML-driven quest objects
  * Added ProfileBatchManager — manages the per-profile Roslyn compile batch for RunCode/RunLua behaviors; mirrors HB WoD Class1208 flow (Reset on Start, Register per behavior, EnsureCompiled on first tick)
  * Added CompileBatch and CompileError — wraps Roslyn compilation unit for cross-node variable sharing
  * Added DelayCompiledExpression — deferred expression wrapper for expressions compiled after profile load
  * ForcedCodeBehavior: hooked into ProfileBatchManager lifecycle (Register + EnsureCompiled)
  * QuestBot: Reset call added on bot Start

MountHelper
--------------
  * MountUp logic hardened for WotLK; edge cases around Sea Legs and swimming fixed

WoWChat
--------------
  * Cleaned up and reduced to WotLK-relevant message types; removed MoP+ channel ids

Other
--------------
  * BotEvents: two missing event hookup fixes
  * LegacySpellManager: missing null guard on spell lookup
  * WoWMovementInfo: IsDescending / IsAscending flags corrected
  * BlackspotManager: removed dead method, fixed range comparison


Plugin DruidHarvestHelper
--------------
  * DruidHarvestHelper 2.0.1.cs
    - Fixed bot name check: "GATHERBUDDY2" / Left(..., 12) → "GATHERBUDDY" / Left(..., 11) — our bot is named "GatherBuddy"; the plugin never activated before; now activates automatically when playing a Druid under GatherBuddy


