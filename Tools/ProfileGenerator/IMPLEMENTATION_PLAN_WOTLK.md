# Plan d'Implémentation WotLK - 100% Compatibilité

## 📊 Résumé de l'Analyse HB

### CustomBehaviors les Plus Utilisés
| CustomBehavior | Count | Priorité | Description |
|----------------|-------|----------|-------------|
| FlyTo | 11,255 | 🟢 Déjà implémenté | Déplacement en vol |
| ForcedDismount | 9,834 | 🟡 Haute | Descendre de monture |
| WaitTimer | 4,436 | 🟡 Haute | Attendre X millisecondes |
| InteractWith | 2,729 | 🔴 Critique | Interagir avec NPC/objet |
| RunMacro | 1,628 | 🟡 Moyenne | Exécuter macro WoW |
| MyCTM | 1,288 | 🟢 Moyenne | Click-to-move |
| Misc\RunLua | 1,154 | 🟡 Haute | Exécuter code Lua |
| UserSettings | 1,017 | 🟢 Déjà implémenté | Paramètres temporaires |
| Message | 909 | 🟢 Basse | Afficher message |
| UserDialog | 492 | 🟢 Basse | Dialogue utilisateur |
| UseItemOn | 481 | 🔴 Critique | Utiliser item sur cible |
| ForceSetVendor | 201 | 🟡 Moyenne | Forcer vendeur |
| CastSpellOn | 180 | 🔴 Critique | Lancer sort sur cible |
| NoCombatMoveTo | 155 | 🟡 Moyenne | Se déplacer sans combat |
| CombatUseItemOn | 152 | 🔴 Haute | Item en combat |
| UseTransport | 135 | 🟡 Haute | Utiliser transport (bateau, zeppelin) |
| LoadProfile | 117 | 🟢 Déjà implémenté | Charger autre profil |

### Objective Types
| Type | Count | Statut |
|------|-------|--------|
| CollectItem | 3,449 | 🟢 Implémenté |
| KillMob | 2,898 | 🟢 Implémenté |
| Collect | 431 | 🟢 Implémenté |
| UseObject | 209 | 🟡 À implémenter |

### Conditions IF (Patterns)
```
HasQuest(ID) - Vérifier si quête acceptée
IsQuestCompleted(ID) - Quête complétée
IsObjectiveComplete(QuestID, ObjectiveIndex) - Objectif spécifique
HasItem(ItemID) - Possède item
CanFly() - Peut voler
Me.IsHorde / Me.IsAlliance - Faction
Me.Gold >= X - Or minimum
Me.Level >= X - Niveau minimum
Me.GetSkill(SkillLine.Riding).CurrentValue - Niveau monte
```

---

## 🎯 Phase 1: CRITIQUE (Semaine 1-2)

### 1.1 UseItemOn - 481 utilisations
**Mapping Zygor → HB:**
```lua
-- Zygor:
use Fishing Chair##33223 |q 11665
use Red Snapper Control Console##35581 |q 11625
```
```xml
<!-- HB: -->
<CustomBehavior File="UseItemOn" QuestId="11665" ItemId="33223" MobId="0" 
    NumOfTimes="1" CollectionDistance="30" WaitTime="1500" />
```

**Paramètres:**
- `QuestId` - ID de la quête
- `ItemId` - ID de l'item à utiliser
- `MobId` - Créature cible (0 si environnement)
- `NumOfTimes` - Nombre d'utilisations
- `Range` - Portée d'utilisation
- `CollectionDistance` - Distance de recherche
- `WaitTime` - Temps d'attente après utilisation

### 1.2 InteractWith - 2,729 utilisations
**Mapping Zygor → HB:**
```lua
-- Zygor:
click Webbed Crusader##28108 |q 12494
talk Lord Afrasastrasz##27575 |q 12437
```
```xml
<!-- HB: -->
<CustomBehavior File="InteractWith" QuestId="12494" MobId="28108"
    NumOfTimes="5" CollectionDistance="100" />
    
<CustomBehavior File="InteractWith" QuestId="12437" MobId="27575"
    GossipOptions="1" />
```

**Paramètres:**
- `QuestId` - ID de la quête
- `MobId` - Créature/objet cible
- `NumOfTimes` - Nombre d'interactions
- `GossipOptions` - Option de dialogue (1, 2, 3...)
- `CollectionDistance` - Distance de recherche
- `WaitTime` - Attente entre interactions

### 1.3 CastSpellOn - 180 utilisations
**Mapping Zygor → HB:**
```lua
-- Zygor avec aura temporaire:
use Warsong Banner##24426 |q 11595
```
```xml
<!-- HB: -->
<CustomBehavior File="CastSpellOn" QuestId="11595" SpellId="SPELL_ID"
    MobId="TARGET_ID" Range="30" />
```

---

## 🎯 Phase 2: HAUTE PRIORITÉ (Semaine 3-4)

### 2.1 ForcedDismount - 9,834 utilisations
**Usage:** Avant d'interagir avec NPC, utiliser item, entrer en instance
```xml
<CustomBehavior File="ForcedDismount" />
```

### 2.2 WaitTimer - 4,436 utilisations
**Usage:** Attendre après action, RP, cinématiques
```xml
<CustomBehavior File="WaitTimer" WaitTime="3000" />
```

### 2.3 Misc\RunLua - 1,154 utilisations
**Usage:** Actions complexes via Lua
```xml
<CustomBehavior File="Misc\RunLua" Lua="UseItemByName('Item Name')" />
<CustomBehavior File="Misc\RunLua" Lua="SelectGossipOption(1)" />
```

### 2.4 RunMacro - 1,628 utilisations
**Usage:** Macros WoW complexes
```xml
<CustomBehavior File="RunMacro" Macro="/target Mob Name&#xA;/click QuestFrameAcceptButton" />
```

### 2.5 UseTransport - 135 utilisations
**Usage:** Bateaux, zeppelins, téléporteurs
```xml
<CustomBehavior File="UseTransport" 
    TransportId="20808"
    GetOnX="-3845.55" GetOnY="-575.25" GetOnZ="11.21"
    GetOffX="-3867.15" GetOffY="2678.78" GetOffZ="1.50" />
```

---

## 🎯 Phase 3: MOYENNE PRIORITÉ (Semaine 5-6)

### 3.1 MyCTM (Click-To-Move) - 1,288 utilisations
**Usage:** Mouvements précis, passages étroits
```xml
<CustomBehavior File="MyCTM" X="1234.5" Y="5678.9" Z="123.4" />
```

### 3.2 NoCombatMoveTo - 155 utilisations
**Usage:** Se déplacer en évitant le combat
```xml
<CustomBehavior File="NoCombatMoveTo" X="1234.5" Y="5678.9" Z="123.4" />
```

### 3.3 CombatUseItemOn - 152 utilisations
**Usage:** Utiliser item pendant le combat
```xml
<CustomBehavior File="CombatUseItemOn" QuestId="12345" ItemId="34567"
    MobId="22222" MobHpPercentLeft="25" />
```

### 3.4 ForceSetVendor - 201 utilisations
**Usage:** Définir vendeur spécifique
```xml
<CustomBehavior File="ForceSetVendor" VendorId="12345" X="100" Y="200" Z="50" />
```

---

## 🎯 Phase 4: BASSE PRIORITÉ (Semaine 7+)

### 4.1 Message / UserDialog
```xml
<CustomBehavior File="Message" Text="Information importante" />
<CustomBehavior File="UserDialog" Title="Attention" 
    Text="Faire action manuelle" />
```

### 4.2 Behaviors Spéciaux
- `CollectThings` - 121 utilisations
- `AscendInWater` - 104 utilisations
- `EjectVeh` - 85 utilisations
- `MountVehOnly` - 70 utilisations
- `Escort` - 65 utilisations
- `KillUntilComplete` - 65 utilisations

---

## 📝 Modifications Parser Zygor

### Patterns à Ajouter

```python
# Pattern: use Item##ID
USE_ITEM_PATTERN = r"use\s+(.+?)##(\d+)"
# → Génère: <CustomBehavior File="UseItemOn" ItemId="ID" />

# Pattern: click Object##ID  
CLICK_PATTERN = r"click\s+(.+?)##(\d+)"
# → Génère: <CustomBehavior File="InteractWith" MobId="ID" />

# Pattern: talk NPC##ID
TALK_PATTERN = r"talk\s+(.+?)##(\d+)"
# → Génère: <CustomBehavior File="InteractWith" MobId="ID" GossipOptions="1" />

# Pattern: fly X,Y,Z
FLY_PATTERN = r"fly\s+([\d.-]+),([\d.-]+),([\d.-]+)"
# → Génère: <CustomBehavior File="FlyTo" X="" Y="" Z="" />
```

### Logique de Détection Auto

```python
def detect_behavior_type(action, quest_data, questie_db):
    """Détermine automatiquement le CustomBehavior approprié"""
    
    if "use" in action:
        item_id = extract_item_id(action)
        if requires_target(item_id, questie_db):
            return "UseItemOn", {"ItemId": item_id, "MobId": get_target_mob(quest_data)}
        else:
            return "UseItemOn", {"ItemId": item_id, "MobId": 0}
    
    if "click" in action:
        obj_id = extract_object_id(action)
        if is_gameobject(obj_id, questie_db):
            return "InteractWith", {"MobId": obj_id, "ObjectType": "GameObject"}
        else:
            return "InteractWith", {"MobId": obj_id}
    
    if "talk" in action:
        npc_id = extract_npc_id(action)
        gossip = detect_gossip_option(quest_data)
        return "InteractWith", {"MobId": npc_id, "GossipOptions": gossip}
```

---

## 📊 Données Nécessaires pour WotLK

### Sources de Données
1. **Zygor Guides WotLK** - Actions, coordonnées, séquence
2. **Questie-335 Database** - IDs quêtes, items, NPCs, objects
3. **WoWHead/Wowpedia** - Vérification manuelle si besoin

### Mapping Requis
```python
# Questie Database Integration
QUESTIE_DB = {
    "quests": {},      # Quest ID → Requirements, Objectives
    "npcs": {},        # NPC ID → Coordinates, Zone
    "items": {},       # Item ID → Quest Item?, Usable?
    "objects": {},     # Object ID → Interactable?, Quest?
}

# Zygor → HB Mapping
ZYGOR_TO_HB = {
    "accept": "PickUp",
    "turnin": "TurnIn", 
    "kill": "Objective Type='KillMob'",
    "collect": "Objective Type='CollectItem'",
    "use": "CustomBehavior File='UseItemOn'",
    "click": "CustomBehavior File='InteractWith'",
    "talk": "CustomBehavior File='InteractWith'",
    "goto": "RunTo",
    "fly": "FlyTo",
}
```

---

## ✅ Checklist d'Implémentation

### Phase 1 (Critique)
- [ ] Parser: Ajouter pattern `use Item##ID`
- [ ] Parser: Ajouter pattern `click Object##ID`
- [ ] Parser: Ajouter pattern `talk NPC##ID`
- [ ] Générer: UseItemOn avec paramètres corrects
- [ ] Générer: InteractWith avec MobId
- [ ] Intégrer: Questie DB pour valider IDs

### Phase 2 (Haute)
- [ ] Ajouter: ForcedDismount avant interactions
- [ ] Ajouter: WaitTimer après actions spéciales
- [ ] Parser: Détecter besoins RunLua
- [ ] Parser: Détecter transports (Zygor waypoints)

### Phase 3 (Moyenne)
- [ ] Parser: Coordonnées précises → MyCTM
- [ ] Parser: Zones dangereuses → NoCombatMoveTo
- [ ] Générer: Vendeurs par zone

### Phase 4 (Finition)
- [ ] Messages informatifs
- [ ] Dialogs pour actions manuelles
- [ ] Quêtes véhicules
- [ ] Quêtes escorte

---

## 📈 Estimation de Couverture

| Avec Current Parser | Avec Phase 1 | Avec Phase 2 | Avec Phase 3 | Complet |
|---------------------|--------------|--------------|--------------|---------|
| ~75% | ~90% | ~95% | ~98% | ~100% |

**Quêtes WotLK estimées:** ~3000 quêtes
- ~2250 fonctionnent déjà (accept/turnin/kill/collect)
- ~450 nécessitent UseItemOn/InteractWith (Phase 1)
- ~150 nécessitent behaviors spéciaux (Phase 2)
- ~100 nécessitent finition (Phase 3)
- ~50 cas edge (Phase 4)

---

## 🚀 Prochaines Étapes Immédiates

1. **Modifier `zygor_parser.py`** pour supporter `use`, `click`, `talk`
2. **Créer mapping Questie** - ItemID → RequiresTarget, ObjectID → IsGameObject
3. **Tester sur 10 profils** avec quêtes utilisant items
4. **Valider génération** des CustomBehaviors corrects
5. **Itérer** sur les cas problématiques
