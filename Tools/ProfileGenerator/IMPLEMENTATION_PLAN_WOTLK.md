# Plan d'Implémentation WotLK - 100% Compatibilité

## ⚠️ RÈGLE CRITIQUE: Conditions If/While

**CHAQUE CustomBehavior DOIT être dans un If ou While avec condition de quête!**
Sans condition, le bot exécute l'action à chaque reload du profil.

```xml
<!-- ❌ MAUVAIS - sera exécuté à chaque fois -->
<CustomBehavior File="UseItemOn" ItemId="35272" MobId="0" />

<!-- ✅ BON - exécuté seulement si quête active et pas complétée -->
<While Condition="((HasQuest(14066)) &amp;&amp; (!IsQuestCompleted(14066)))" >
    <CustomBehavior File="UseItemOn" ItemId="35272" MobId="0" />
</While>
```

---

## 📋 Les 12 Patterns de Base HB

### Pattern 1: PickUp SIMPLE
```xml
<PickUp QuestId="867" QuestName="Harpy Raiders" GiverId="3449" GiverName="Darsok" />
```
**Quand:** Quête de base sans pré-requis

### Pattern 2: PickUp CHAÎNÉ (avec If)
```xml
<If Condition="((!HasQuest(869)) &amp;&amp; (!IsQuestCompleted(869)))" >
    <PickUp QuestId="869" QuestName="To Track a Thief" GiverId="3464" />
</If>
```
**Quand:** Quête après une autre, ou avec pré-requis Questie

### Pattern 3: Objective SIMPLE
```xml
<Objective QuestId="867" Type="KillMob" MobId="3278" KillCount="6" />
<Objective QuestId="891" Type="CollectItem" ItemId="5078" CollectCount="10" />
```
**Quand:** Kill/Collect standard - QuestId suffit pour le skip

### Pattern 4: UseItem dans While
```xml
<While Condition="((HasQuest(14038)) &amp;&amp; (!IsQuestCompleted(14038)))" >
    <RunTo X="-1468" Y="-3967" Z="-1.65" />
    <UseItem ItemId="46829" QuestId="14038" X="-1469" Y="-3967" Z="-1.64" />
    <CustomBehavior File="WaitTimer" WaitTime="3000" />
</While>
```
**Quand:** Zygor `use Item##ID |q QUEST`

### Pattern 5: InteractWith dans If
```xml
<If Condition="((HasQuest(14046)) &amp;&amp; (!IsQuestCompleted(14046)))" >
    <CustomBehavior File="InteractWith" QuestId="14046" MobId="3467" 
        GossipOptions="2" WaitTime="3000" X="-1454" Y="-3815" Z="21.6" />
</If>
```
**Quand:** Zygor `click Object##ID` ou `talk NPC##ID`

### Pattern 6: CompleteLogQuest (quête auto-complete)
```xml
<While Condition="((HasQuest(14066)) &amp;&amp; (IsQuestCompleted(14066)))" >
    <CustomBehavior File="CompleteLogQuest" QuestId="14066" />
    <CustomBehavior File="WaitTimer" WaitTime="5000" />
</While>
```
**Quand:** Pas de TurnIn NPC dans Questie (quête se termine dans le log)

### Pattern 7: TurnIn SIMPLE
```xml
<TurnIn QuestId="867" QuestName="Harpy Raiders" TurnInId="3449" />
```

### Pattern 8: TurnIn CONDITIONNEL
```xml
<If Condition="((HasQuest(29024)) &amp;&amp; (IsQuestCompleted(29024)))" >
    <TurnIn QuestId="29024" TurnInId="3442" />
</If>
```
**Quand:** Après action spéciale, ou avec cleanup

### Pattern 9: Transport/Téléportation
```xml
<If Condition="((HasQuest(29026)) &amp;&amp; (IsQuestCompleted(29026)))" >
    <CustomBehavior File="InteractWith" MobId="34674" GossipOptions="1" />
    <CustomBehavior File="WaitTimer" WaitTime="15000" />
</If>
```

### Pattern 10: Véhicule
```xml
<If Condition="((HasQuest(26769)) &amp;&amp; (!IsQuestCompleted(26769)))" >
    <RunTo X="-702" Y="-3986" Z="19.4" />
    <CustomBehavior File="ForcedDismount" />
    <CustomBehavior File="MountVehOnly" VehicleMountId="44057" X="-702" Y="-3985" Z="19.4" />
    <CustomBehavior File="WaitTimer" WaitTime="122000" />
</If>
```

### Pattern 11: Escort avec retry
```xml
<While Condition="((HasQuest(863)) &amp;&amp; (!IsQuestCompleted(863)))" >
    <CustomBehavior File="UserSettings" PullDistance="1" />
    <CustomBehavior File="Escort" EscortUntil="DestinationReached" MobId="3439" ... />
    <CustomBehavior File="WaitTimer" WaitTime="60000" />
    <If Condition="((HasQuest(863)) &amp;&amp; (!IsQuestCompleted(863)))" >
        <CustomBehavior File="AbandonQuest" QuestId="863" Type="Incomplete" />
        <PickUp QuestId="863" ... /> <!-- Retry -->
    </If>
</While>
```

### Pattern 12: Objectifs séquentiels
```xml
<While Condition="HasQuest(29022) &amp;&amp; !IsQuestCompleted(29022)">
    <Objective Type="UseObject" ObjectId="4072" UseCount="1" />
    <CustomBehavior File="WaitTimer" WaitTime="2000" />
    <Objective Type="UseObject" ObjectId="61936" UseCount="1" />
    <CustomBehavior File="WaitTimer" WaitTime="2000" />
</While>
```

---

## 📊 Tableau de Décision: Zygor → HB

| Type Zygor | Pattern HB | Condition Requise |
|------------|------------|-------------------|
| `accept Quest##ID` simple | `<PickUp />` | ❌ Non |
| `accept Quest##ID` chaîné | `<If !HasQuest && !IsQuestCompleted>` | ✅ Oui |
| `turnin Quest##ID` simple | `<TurnIn />` | ❌ Non |
| `turnin Quest##ID` après action | `<If HasQuest && IsQuestCompleted>` | ✅ Oui |
| `kill Mob##ID \|q X/Y` | `<Objective Type="KillMob" />` | ❌ Non (QuestId) |
| `collect Item##ID \|q X/Y` | `<Objective Type="CollectItem" />` | ❌ Non (QuestId) |
| `use Item##ID \|q X` | `<While HasQuest && !IsQuestCompleted>` | ✅ **TOUJOURS** |
| `click Object##ID \|q X` | `<If/While HasQuest && !IsQuestCompleted>` | ✅ **TOUJOURS** |
| `talk NPC##ID \|q X` | `<If HasQuest && !IsQuestCompleted>` | ✅ **TOUJOURS** |
| `goto X,Y,Z \|q X` (autocomplete) | `<While> + <RunTo>` | ✅ Oui |
| Quête sans TurnIn | `<While IsQuestCompleted> <CompleteLogQuest>` | ✅ Oui |

---

## 📊 Résumé de l'Analyse HB

### CustomBehaviors les Plus Utilisés
| CustomBehavior | Count | Priorité | Description |
|----------------|-------|----------|-------------|
| FlyTo | 11,255 | 🟢 Déjà implémenté | Déplacement en vol (Outland 70+, Northrend 77+ seulement) |
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

## 🎯 Phase 1: CRITIQUE - Parser Intelligent avec Conditions

### 1.1 Modifier generate_quest_block()
Le parser doit:
1. **Grouper les actions par quest_id**
2. **Générer If/While appropriés** selon le type d'action
3. **Consulter Questie** pour pré-requis et type TurnIn

```python
def generate_quest_block(quest_id, actions, questie_db):
    """Génère un bloc complet pour une quête avec conditions"""
    
    quest_data = questie_db.get_quest(quest_id)
    is_chained = quest_data.has_prereq()
    has_turnin_npc = quest_data.turnin_npc_id is not None
    
    output = []
    
    # 1. PickUp
    if is_chained:
        output.append(f'<If Condition="((!HasQuest({quest_id})) &amp;&amp; (!IsQuestCompleted({quest_id})))">')
        output.append(f'    <PickUp QuestId="{quest_id}" ... />')
        output.append('</If>')
    else:
        output.append(f'<PickUp QuestId="{quest_id}" ... />')
    
    # 2. Actions (UseItem, InteractWith → besoin While/If)
    for action in actions:
        if action.type in ['use', 'click', 'talk']:
            output.append(f'<While Condition="((HasQuest({quest_id})) &amp;&amp; (!IsQuestCompleted({quest_id})))">')
            output.append(f'    {generate_action(action)}')
            output.append(f'    <CustomBehavior File="WaitTimer" WaitTime="3000" />')
            output.append('</While>')
        else:
            output.append(generate_action(action))
    
    # 3. TurnIn ou CompleteLogQuest
    if has_turnin_npc:
        output.append(f'<TurnIn QuestId="{quest_id}" ... />')
    else:
        output.append(f'<While Condition="((HasQuest({quest_id})) &amp;&amp; (IsQuestCompleted({quest_id})))">')
        output.append(f'    <CustomBehavior File="CompleteLogQuest" QuestId="{quest_id}" />')
        output.append(f'    <CustomBehavior File="WaitTimer" WaitTime="5000" />')
        output.append('</While>')
    
    return output
```

### 1.2 Ajouter parsing `use`, `click`, `talk`
```python
USE_ITEM_PATTERN = r"use\s+(.+?)##(\d+)"      # use Item##ID
CLICK_PATTERN = r"click\s+(.+?)##(\d+)"        # click Object##ID
TALK_PATTERN = r"talk\s+(.+?)##(\d+)"          # talk NPC##ID
```

### 1.3 Interroger Questie pour pré-requis
```python
def has_prereq(quest_id, questie_db):
    """Vérifie si la quête a des pré-requis"""
    quest = questie_db['quests'].get(quest_id, {})
    return quest.get('preQuestSingle') or quest.get('preQuestGroup')

def get_turnin_npc(quest_id, questie_db):
    """Retourne l'ID du NPC TurnIn ou None si auto-complete"""
    quest = questie_db['quests'].get(quest_id, {})
    return quest.get('finishedBy', [None])[0]
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
