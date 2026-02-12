# CopilotBuddy — Rapport Définitif de Couverture API

**Date:** 2026-02-12  
**Source référence:** `Honorbuddy.chm` (HB MoP 5.4.8 — Sandcastle API docs)  
**Source code:** HB 4.3.4 (Cata) pour l'architecture, HB 3.3.5a pour les offsets  
**Cible:** WoW 3.3.5a (WotLK, build 12340)

---

## Résumé Global

| Métrique | Valeur |
|----------|--------|
| **Namespaces CHM total** | 60 |
| **Namespaces pertinents (excl. MoP/WoD)** | 52 |
| **Types dans le CHM** | 942 classes/types, 6294 membres |
| **Types pertinents pour 3.3.5a** | ~720 |
| **Types présents dans CB** | ~510 |
| **Types manquants (pertinents)** | ~210 |
| **Couverture globale** | **~71%** |

### Namespaces à ignorer (MoP/WoD/Cata only)

Ces 8 namespaces sont post-3.3.5a et n'ont PAS besoin d'être portés :

- `Styx.WoWInternals.Garrison` (18 types — WoD)
- `Styx.WoWInternals.WoWObjects.AreaTriggerShapes` (5 types — MoP+)
- `Buddy.Coroutines` (7 types — MoP async)
- `Styx.CommonBot.Coroutines` (6 types — MoP async)
- `Styx.CommonBot.Events.Profile` (1 type)
- `Styx.WoWInternals.WoWObjects.SubObjects` (2 types)
- BG landmarks Cata+ (BattleForGilneas, DeepwindGorge, ResearchSite)

---

## Couverture par Namespace

### A. Namespaces 100% complets ✅

| Namespace | Types |
|-----------|-------|
| Styx.Plugins | 4/4 — HBPlugin, PluginContainer, PluginManager, PluginWrapper |
| Styx.CommonBot.AreaManagement | 10/10 — Area, GrindArea, Hotspot, QuestArea, etc. |
| CommonBehaviors.Decorators | 4/4 + 1 extra (DecoratorNeedToMoveToPoint) |
| Styx.CommonBot.POI | 3/3 — BotPoi, PoiType, PoiTypeExtensions |

### B. Namespaces haute couverture (>75%) ✅

| Namespace | Total | CB | Manquant | % | Notes |
|-----------|-------|-----|---------|---|-------|
| Styx.WoWInternals.WoWObjects | 40 | 36 | 4 | 90% | Manque: RaidTargetMarker, WoWArenaTeamInfo, WoWInebriationLevel, WoWPlayerCombatRating |
| CommonBehaviors.Actions | 14 | 12 | 2 | 86% | Manque: NavigationInfo, SleepForLagDuration. A 5 extras |
| Styx.TreeSharp | 26 | 21 | 5 | 81% | Manque: DynamicChildSelector, ProbabilitySelector, Sleep, WhileLoop |
| Styx.CommonBot.Frames | 24 | 19 | 3 | 79% | Manque: GuildBankFrame, FlightMapFrame |
| Styx.CommonBot.Inventory | 9 | 7 | 2 | 78% | Manque: EquipmentManager, EquipmentSet |

### C. Namespaces couverture moyenne (50-75%)

| Namespace | Total | CB | Manquant | % | Notes |
|-----------|-------|-----|---------|---|-------|
| Styx.WoWInternals | ~85 | ~65 | ~15 | 73% | Manque: WoWGuid, WoWVehicle, WoWPetControl, MoveFlags, GameInput, BG landmarks (AV, AB, EotS, IoC) |
| Styx.CommonBot.Routines | 3 | 2 | 1 | 67% | Manque: InvalidRoutineWrapper (mineur) |
| Styx (root) | 82 | 53 | 29 | 65% | Manque: SpellAttributes ×9, WoWSpec, NavType, EmoteState, WoWGameObjectState |
| Styx.CommonBot | ~65 | ~45 | ~10 | 60% | Manque: HealTargeting, Chat (shape HB), SpellFindResults |
| Styx.CommonBot.Profiles | 36 | 22 | 8 | 61% | Manque: CFB constraint types, IXmlObject |
| Styx.Helpers | 31 | 18 | 13 | 58% | Manque: GlobalSettings, CachedValue<T>, TreeHooks-related, BGBotSettings |

### D. Namespaces faible couverture (<50%)

| Namespace | Total | CB | Manquant | % | Notes |
|-----------|-------|-----|---------|---|-------|
| Styx.Pathing | 24 | 10 | 10 | 42% | Architecture: CB utilise Navigator statique, pas le pattern NavigationProvider |
| Styx.WoWInternals.DBC | 27 | 8 | 19 | 30% | Beaucoup de wrappers DBC absents |
| Styx.Common | 55 | 15 | 40 | 27% | TreeHooks, géométrie, HookExecutor, extensions |
| Styx.CommonBot.CharacterManagement | 12 | 3 | 9 | 25% | TalentSelector, ClassProfile, WeaponStyle |
| Styx.WoWInternals.DB | 20 | 2 | 18 | 10% | Types DB: Vehicle, ItemEntry, WoWDbRow/Table |

---

## Classification des Gaps

### Tier 1 — BLOQUANTS (empêchent des features cruciales) 🔴

| # | Type manquant | Namespace | Impact | Effort |
|---|---------------|-----------|--------|--------|
| 1 | **SpellManager overloads** (~20) | Styx.WoWInternals | Combat routines ne compilent pas si elles appellent `Cast(string, ...)`, `CanCast(WoWSpell, ...)` etc. | MOYEN — ajouter surcharges dans SpellManager.cs |
| 2 | **RoutineManager.Current setter** | Styx.CommonBot.Routines | `RoutineManager.Current = myRoutine;` échoue | FACILE — ajouter setter |
| 3 | **WoWSpell.Description** | Styx.WoWInternals | Certaines routines lisent la description du sort | FACILE — Lua call |

### Tier 2 — IMPORTANTS (features secondaires mais attendues) 🟡

| # | Type manquant | Namespace | Impact | Effort |
|---|---------------|-----------|--------|--------|
| 4 | **WoWVehicle** | Styx.WoWInternals | Véhicules WotLK (Ulduar, ICC, Wintergrasp) | MOYEN — ~150 lignes, offsets dans 3.3.5a |
| 5 | **HealTargeting** | Styx.CommonBot | Bots heal (Singular heal spec) | MOYEN — ~100 lignes, liste de cibles à heal |
| 6 | **WoWPetControl** | Styx.WoWInternals | Gestion pet (Hunter/Lock/DK) | MOYEN — ~100 lignes |
| 7 | **MoveFlags** enum | Styx.WoWInternals | Détection mouvement (swimming, flying, falling) | FACILE — enum, ~30 flags |
| 8 | **WoWGuid** struct | Styx.WoWInternals | Type-safe GUID wrapper (Player vs Unit vs Item) | MOYEN — touche beaucoup de code |
| 9 | **GameInput** | Styx.WoWInternals | Simulation clavier/souris | MOYEN — ~200 lignes |
| 10 | **SpellAttributes** ×9 enums | Styx | Filtrage avancé de sorts | FACILE — copier enums depuis HB 4.3.4 |
| 11 | **WoWSpec** enum | Styx | Identification spécialisation | FACILE — enum, ~30 specs WotLK |
| 12 | **GuildBankFrame** | Styx.CommonBot.Frames | Interaction banque de guilde | MOYEN — Lua calls |
| 13 | **EquipmentManager/EquipmentSet** | Styx.CommonBot.Inventory | Auto-équipement par set | MOYEN |
| 14 | **BG Landmarks** (AV, AB, EotS, IoC) | Styx.WoWInternals | Bots BG ont besoin des positions de flags/bases | MOYEN — ~400 lignes total |

### Tier 3 — UTILES (améliorations, pas bloquants) 🟢

| # | Type manquant | Namespace | Impact | Effort |
|---|---------------|-----------|--------|--------|
| 15 | **TreeHooks** | Styx.Common | Système de hooks dans l'arbre comportemental | MOYEN — architecture plugin |
| 16 | **DBC wrappers** (19 types) | Styx.WoWInternals.DBC | CreatureFamily, SkillLineAbility, SpellItemEnchantment, etc. | GROS — chaque wrapper = offsets + struct |
| 17 | **DB types** (Vehicle, ItemEntry, WoWDbRow) | Styx.WoWInternals.DB | Accès DB typé aux données de jeu | GROS |
| 18 | **TreeSharp extras** (5 types) | Styx.TreeSharp | DynamicChildSelector, ProbabilitySelector, Sleep, WhileLoop | FACILE |
| 19 | **CharacterManagement** (9 types) | Styx.CommonBot | TalentSelector, ClassProfile, WeaponStyle | MOYEN |
| 20 | **NavigationProvider pattern** | Styx.Pathing | Architecture pluggable — Navigator fonctionne déjà | GROS — refactoring |
| 21 | **CFB constraint types** (8 types) | Styx.CommonBot.Profiles | Validation d'attributs pour QuestBehaviors | MOYEN |
| 22 | **Styx.Common helpers** (40 types) | Styx.Common | Géométrie, HookExecutor, extensions, etc. | GROS — beaucoup de types utilitaires |
| 23 | **WoWArenaTeamInfo** | WoWObjects | Info équipes d'arène 3.3.5a | FACILE — struct |
| 24 | **EmoteState** enum | Styx | États d'emote | FACILE — enum |
| 25 | **RaidTargetMarker** enum | WoWObjects | Marqueurs de raid (skull, cross, etc.) | FACILE — enum |
| 26 | **GlobalSettings** | Styx.Helpers | Settings globaux HB | FACILE — classe settings |
| 27 | **Styx.Common geometry** | Styx.Common | BoundingBox2/3, BoundingSphere, Ray, LineSegment | MOYEN — math 3D |

### Tier 4 — STUBS SUFFISANTS (Cata/MoP features — stub return neutral) 🔵

| Type | Raison |
|------|--------|
| WoWAreaTrigger | MoP+ |
| CapabilityFlags/Manager | MoP+ |
| SpellChargeInfo, SpellDetailedPowerCost | MoP+ |
| PetBattleState, BattlePet types | MoP+ (pet battles) |
| Garrison everything (18 types) | WoD+ |
| ResearchSite | Cata+ archaeology |
| BattleForGilneas, DeepwindGorge BG landmarks | Cata+/MoP+ BGs |
| ActionRunCoroutine | MoP+ async |
| ItemContext | WoD+ |
| Buddy.Coroutines, Styx.CommonBot.Coroutines | MoP+ async |

---

## Quoi faire avec ces informations ?

### Option A — Test d'abord, fix après (RECOMMANDÉ)

1. **Tester maintenant** avec les bots existants (GrindBot, QuestBot, GatherBuddy)
2. Les erreurs de compilation des CC/plugins/quest behaviors révéleront les gaps Tier 1 en pratique
3. Fixer au fur et à mesure — chaque gap Tier 1 est petit (~30min chacun)

**Avantage:** Pas de travail inutile. On fixe seulement ce qui casse réellement.

### Option B — Fix les Tier 1 d'abord

3 items Tier 1 = ~2h de travail :
1. SpellManager overloads (~30min)
2. RoutineManager.Current setter (~10min)
3. WoWSpell.Description (~20min)

Puis tester.

### Option C — Référence passive

Garder le rapport + les docs Markdown comme **référence** uniquement. Quand un CC ou plugin ne compile pas, consulter ce rapport pour savoir exactement quel type manque et quel tier il appartient.

---

## Stats finales

```
Namespaces 100% complets     :   4 / 21  (excl. MoP/WoD-only)
Namespaces > 75%             :   5 / 21
Namespaces 50-75%            :   6 / 21
Namespaces < 50%             :   6 / 21

Types pertinents dans CHM    : ~720
Types présents dans CB       : ~510 (71%)
Gaps Tier 1 (bloquants)      :   3
Gaps Tier 2 (importants)     :  11
Gaps Tier 3 (utiles)         :  13+
Stubs Cata/MoP (Tier 4)      : ~50
```

**Verdict:** CB est fonctionnel pour les bots de base (Grind, Quest, Gather). Les 3 gaps Tier 1 sont petits et rapides à fixer. Les gaps Tier 2 ne bloquent que des features spécifiques (heal bot, vehicles, BG). Le gros des gaps est dans Styx.Common (utilitaires) et les DBC wrappers — nice-to-have mais pas bloquant.
