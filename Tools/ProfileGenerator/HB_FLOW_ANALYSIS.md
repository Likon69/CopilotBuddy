# Analyse Complète du Flux HB Profiles

## 🔍 Patterns Identifiés dans les Profils HB

### Structure Générale d'un Profil
```xml
<HBProfile>
    <Name>...</Name>
    <MinLevel>1</MinLevel>
    <MaxLevel>101</MaxLevel>
    
    <!-- Paramètres globaux -->
    <MinDurability>0.3</MinDurability>
    <MinFreeBagSlots>3</MinFreeBagSlots>
    <MailGrey>False</MailGrey>
    <SellGrey>True</SellGrey>
    
    <!-- Zones à éviter -->
    <Blackspots>...</Blackspots>
    
    <!-- Mobs à ignorer -->
    <AvoidMobs>...</AvoidMobs>
    
    <!-- Vendeurs optionnels -->
    <Vendors>...</Vendors>
    
    <!-- Overrides d'objectifs de quêtes -->
    <Quest Id="867" Name="...">
        <Objective Type="CollectItem" ItemId="5064" CollectCount="8">
            <CollectFrom>
                <Mob Name="..." Id="..."/>
            </CollectFrom>
            <Hotspots>...</Hotspots>
        </Objective>
    </Quest>
    
    <!-- Ordre des quêtes -->
    <QuestOrder>...</QuestOrder>
</HBProfile>
```

---

## 📋 Les 12 Patterns de Base

### 1. PickUp SIMPLE (sans condition)
```xml
<PickUp QuestName="Harpy Raiders" QuestId="867" GiverName="Darsok Swiftdagger" GiverId="3449" />
```
**Usage:** Quête de base, le bot vérifie automatiquement si pas déjà acceptée.
**Zygor équivalent:** `accept Harpy Raiders##867 |goto Darsok Swiftdagger##3449`

---

### 2. PickUp CONDITIONNEL (avec If)
```xml
<If Condition="((!HasQuest(869)) &amp;&amp; (!IsQuestCompleted(869)))" >
    <PickUp QuestName="To Track a Thief" QuestId="869" GiverName="Gazrog" GiverId="3464"/>
</If>
```
**Usage:** 
- Quête chaînée (vient après une autre)
- Pré-requis spécifique
- Éviter de spam si déjà complétée

**Zygor équivalent:** `accept To Track a Thief##869` (après une autre quête dans le guide)

**Détection:** Si dans Zygor la quête est après un `turnin` ou `|only if completedq(AUTRE_QUEST)`

---

### 3. Objective SIMPLE (sans condition)
```xml
<Objective QuestName="Harpy Raiders" QuestId="867" Type="CollectItem" ItemId="5064" CollectCount="8"/>
<Objective QuestName="..." QuestId="891" Type="KillMob" MobId="3278" KillCount="6"/>
```
**Usage:** Objectif standard - le bot vérifie automatiquement via QuestId.
**Zygor équivalent:** `kill Mob##ID |q 891/1` ou `collect Item##ID |q 891/2`

---

### 4. Actions Spéciales avec While (!IsQuestCompleted)
```xml
<While Condition="((HasQuest(14066)) &amp;&amp; (!IsQuestCompleted(14066)))" >
    <RunTo QuestId="14066" X="-772.7415" Y="-3193.571" Z="91.69006"/>
    <CustomBehavior QuestId="14066" File="WaitTimer" WaitTime="5000" GoalText="Waiting for bot {TimeRemaining}" />
</While>
```
**Usage:**
- Quêtes qui se complètent en allant à un endroit
- UseItem, UseObject
- InteractWith non-standard  
- Escort quest
- Actions répétables jusqu'à completion

**Zygor équivalent:** `goto X,Y,Z |q 14066` quand c'est une quête "go here"

---

### 5. CompleteLogQuest avec While (IsQuestCompleted)
```xml
<While Condition="((HasQuest(14066)) &amp;&amp; (IsQuestCompleted(14066)))" >
    <CustomBehavior File="CompleteLogQuest" QuestId="14066" /> <!-- Get 869 -->
    <CustomBehavior File="WaitTimer" WaitTime="5000" GoalText="Waiting for bot {TimeRemaining}" />
</While>
```
**Usage:** Quête qui se complète automatiquement dans le log (pas besoin de TurnIn à un NPC).
**Zygor équivalent:** Pas d'équivalent direct - détecter via Questie si `turnin` absent

---

### 6. TurnIn SIMPLE
```xml
<TurnIn QuestName="Harpy Raiders" QuestId="867" TurnInName="Darsok Swiftdagger" TurnInId="3449"/>
```
**Usage:** TurnIn standard.
**Zygor équivalent:** `turnin Harpy Raiders##867 |goto Darsok Swiftdagger##3449`

---

### 7. TurnIn CONDITIONNEL
```xml
<If Condition="((HasQuest(29024)) &amp;&amp; (IsQuestCompleted(29024)))" >
    <TurnIn QuestName="Samophlange" QuestId="29024" TurnInName="Sputtervalve" TurnInId="3442" />
    <CustomBehavior File="DeleteItems" Ids="5088" />
</If>
```
**Usage:** TurnIn après une action spéciale qui peut échouer, ou avec cleanup (DeleteItems).

---

### 8. InteractWith pour dialogue/gossip
```xml
<If Condition="((HasQuest(14046)) &amp;&amp; (!IsQuestCompleted(14046)))" >
    <CustomBehavior File="InteractWith" QuestId="14046" MobId="3467" GossipOptions="2" WaitTime="3000" X="-1454.157" Y="-3815.583" Z="21.60774" />
</If>
```
**Usage:** Parler à NPC avec option de dialogue spécifique.
**Zygor équivalent:** `talk NPC##ID |q QUEST` (avec dialogue implicite)

---

### 9. InteractWith pour transport/téléportation
```xml
<If Condition="((HasQuest(29026)) &amp;&amp; (IsQuestCompleted(29026)))" >
    <CustomBehavior File="InteractWith" MobId="34674" GossipOptions="1" X="1160.245" Y="-3395.619" Z="91.66805" />
    <CustomBehavior File="WaitTimer" WaitTime="15000" GoalText="Waiting for port{TimeRemaining}" />
</If>
```
**Usage:** NPC qui téléporte après avoir complété la quête.
**Détection:** Via Questie - NPC avec flag transport ou gossip

---

### 10. UseItem dans While
```xml
<While Condition="((HasQuest(14038)) &amp;&amp; (!IsQuestCompleted(14038)))" >
    <RunTo X="-1468.916" Y="-3967.801" Z="-1.651532" />
    <CustomBehavior File="WaitTimer" WaitTime="2000" GoalText="Waiting for agro to clear {TimeRemaining}" />
    <UseItem ItemId="46829" QuestName="Love it or Limpet" QuestId="14038" X="-1469.204" Y="-3967.555" Z="-1.641989" />
    <CustomBehavior File="WaitTimer" WaitTime="10000" GoalText="Waiting for quest to complete {TimeRemaining}" />
</While>
```
**Usage:** Item à utiliser pour compléter une quête.
**Zygor équivalent:** `use Item##46829 |q 14038`

---

### 11. MountVehOnly pour véhicules
```xml
<If Condition="((HasQuest(26769)) &amp;&amp; (!IsQuestCompleted(26769)))" >
    <RunTo X="-702.7002" Y="-3986.347" Z="19.46557" />
    <CustomBehavior File="ForcedDismount" />
    <CustomBehavior File="MountVehOnly" VehicleMountId="44057" X="-702.8704" Y="-3985.98" Z="19.46014" />
    <CustomBehavior File="WaitTimer" WaitTime="122000" GoalText="Waiting for ride {TimeRemaining}" />
</If>
```
**Usage:** Quête avec véhicule/monture spéciale.
**Détection:** Questie - quête avec vehicleId ou dans zone spéciale

---

### 12. Escort quest avec fallback
```xml
<If Condition="((HasQuest(863)) &amp;&amp; (!IsQuestCompleted(863)))" >
    <CustomBehavior File="AbandonQuest" QuestId="863" Type="Failed" />
    <CustomBehavior File="WaitTimer" WaitTime="5000" />
</If>
<PickUp QuestName="The Escape" QuestId="863" GiverName="Wizzlecrank's Shredder" GiverId="3439" />
<While Condition="((HasQuest(863)) &amp;&amp; (!IsQuestCompleted(863)))" >
    <CustomBehavior File="UserSettings" PullDistance="1" />
    <CustomBehavior File="Escort" EscortUntil="DestinationReached" MobId="3439" X="..." EscortDestX="..." />
    <CustomBehavior File="WaitTimer" WaitTime="3000" />
    <CustomBehavior File="UserSettings" PullDistance="25" />
    <CustomBehavior File="WaitTimer" WaitTime="60000" />
    <If Condition="((HasQuest(863)) &amp;&amp; (!IsQuestCompleted(863)))" >
        <CustomBehavior File="AbandonQuest" QuestId="863" Type="Incomplete" />
        <CustomBehavior File="WaitTimer" WaitTime="5000" />
        <PickUp QuestName="The Escape" QuestId="863" ... /> <!-- Retry -->
    </If>
</While>
```
**Usage:** Escort qui peut échouer avec retry automatique.

---

## 📊 Tableau de Décision: Zygor → HB

| Type Zygor | Pattern HB | Condition |
|------------|------------|-----------|
| `accept Quest##ID` simple | `<PickUp ... />` | Aucune |
| `accept Quest##ID` chaîné | `<If !HasQuest && !IsQuestCompleted>` | Après autre quête |
| `turnin Quest##ID` simple | `<TurnIn ... />` | Aucune |
| `turnin Quest##ID` après action | `<If HasQuest && IsQuestCompleted>` | Après UseItem/InteractWith |
| `kill Mob##ID |q X/Y` | `<Objective Type="KillMob" ... />` | Aucune |
| `collect Item##ID |q X/Y` | `<Objective Type="CollectItem" ... />` | Aucune |
| `use Item##ID |q X` | `<While HasQuest && !IsQuestCompleted>` | Toujours |
| `click Object##ID |q X` | `<If/While HasQuest && !IsQuestCompleted>` | Toujours |
| `talk NPC##ID |q X` | `<If HasQuest && !IsQuestCompleted>` avec GossipOptions | Si dialogue |
| `goto X,Y,Z |q X` (autocomplete) | `<While>` + `<RunTo>` | Si quête se complète au lieu |
| Quête sans TurnIn | `<While IsQuestCompleted> <CompleteLogQuest>` | Si pas de turnin dans Questie |

---

## 🔑 Détection Automatique des Patterns

### Comment savoir si une quête est chaînée?
1. Dans Zygor: `|only if completedq(QUEST_ID)` ou position après `turnin`
2. Dans Questie: `preQuestSingle` ou `preQuestGroup`

### Comment savoir si UseItem est nécessaire?
1. Dans Zygor: `use Item##ID`
2. Dans Questie: `ObjectiveType.Item` avec `UseItem = true`

### Comment savoir si InteractWith avec GossipOptions?
1. Dans Zygor: `talk NPC##ID` sans `accept` ni `turnin`
2. Dans Questie: NPC avec flags dialogue/gossip

### Comment savoir si CompleteLogQuest?
1. Pas de `turnin` dans Zygor pour cette quête
2. Dans Questie: `Flags` contient QUEST_FLAG_AUTOCOMPLETE

### Comment savoir si quête véhicule?
1. Dans Questie: `vehicleId` présent
2. Zygor: Pas d'indicateur direct, mais coordonnées vers un véhicule

---

## 🛠️ Logique du Parser Intelligent

```python
def generate_quest_block(quest_id, quest_actions, questie_db):
    """Génère le bloc XML complet pour une quête"""
    
    quest_data = questie_db.get_quest(quest_id)
    is_chained = quest_data.has_prereq()
    has_turnin_npc = quest_data.turnin_npc_id is not None
    
    output = []
    
    # 1. PickUp
    if is_chained:
        output.append(f'<If Condition="((!HasQuest({quest_id})) &amp;&amp; (!IsQuestCompleted({quest_id})))" >')
        output.append(f'    <PickUp QuestId="{quest_id}" ... />')
        output.append('</If>')
    else:
        output.append(f'<PickUp QuestId="{quest_id}" ... />')
    
    # 2. Objectives et Actions
    for action in quest_actions:
        if action.type == 'kill':
            output.append(f'<Objective QuestId="{quest_id}" Type="KillMob" MobId="{action.mob_id}" KillCount="{action.count}" />')
        
        elif action.type == 'collect':
            output.append(f'<Objective QuestId="{quest_id}" Type="CollectItem" ItemId="{action.item_id}" CollectCount="{action.count}" />')
        
        elif action.type == 'use':
            output.append(f'<While Condition="((HasQuest({quest_id})) &amp;&amp; (!IsQuestCompleted({quest_id})))" >')
            output.append(f'    <RunTo X="{action.x}" Y="{action.y}" Z="{action.z}" />')
            output.append(f'    <UseItem ItemId="{action.item_id}" QuestId="{quest_id}" X="{action.x}" Y="{action.y}" Z="{action.z}" />')
            output.append(f'    <CustomBehavior File="WaitTimer" WaitTime="3000" />')
            output.append('</While>')
        
        elif action.type == 'click':
            output.append(f'<If Condition="((HasQuest({quest_id})) &amp;&amp; (!IsQuestCompleted({quest_id})))" >')
            output.append(f'    <CustomBehavior File="InteractWith" QuestId="{quest_id}" MobId="{action.object_id}" X="{action.x}" Y="{action.y}" Z="{action.z}" />')
            output.append('</If>')
        
        elif action.type == 'talk':
            output.append(f'<If Condition="((HasQuest({quest_id})) &amp;&amp; (!IsQuestCompleted({quest_id})))" >')
            output.append(f'    <CustomBehavior File="InteractWith" QuestId="{quest_id}" MobId="{action.npc_id}" GossipOptions="1" WaitTime="3000" X="{action.x}" Y="{action.y}" Z="{action.z}" />')
            output.append('</If>')
        
        elif action.type == 'goto' and quest_data.is_goto_complete():
            # Quête qui se complète en allant quelque part
            output.append(f'<While Condition="((HasQuest({quest_id})) &amp;&amp; (!IsQuestCompleted({quest_id})))" >')
            output.append(f'    <RunTo QuestId="{quest_id}" X="{action.x}" Y="{action.y}" Z="{action.z}" />')
            output.append(f'    <CustomBehavior File="WaitTimer" WaitTime="5000" />')
            output.append('</While>')
    
    # 3. CompleteLogQuest ou TurnIn
    if has_turnin_npc:
        output.append(f'<TurnIn QuestId="{quest_id}" TurnInId="{quest_data.turnin_npc_id}" ... />')
    else:
        # Quête auto-complete
        output.append(f'<While Condition="((HasQuest({quest_id})) &amp;&amp; (IsQuestCompleted({quest_id})))" >')
        output.append(f'    <CustomBehavior File="CompleteLogQuest" QuestId="{quest_id}" />')
        output.append(f'    <CustomBehavior File="WaitTimer" WaitTime="5000" />')
        output.append('</While>')
    
    return output
```

---

## ⚠️ Cas Spéciaux à Gérer

### 1. Quêtes avec plusieurs objectifs séquentiels
Certains objectifs doivent être faits dans l'ordre:
```xml
<While Condition="HasQuest(29022) &amp;&amp; !IsQuestCompleted(29022)">
    <Objective Type="UseObject" ObjectId="4072" UseCount="1" />
    <CustomBehavior File="WaitTimer" WaitTime="2000" />
    <Objective Type="UseObject" ObjectId="61936" UseCount="1" />
    <CustomBehavior File="WaitTimer" WaitTime="2000" />
    <Objective Type="UseObject" ObjectId="61935" UseCount="1" />
</While>
```

### 2. Quêtes Escort avec retry
Voir pattern #12

### 3. Quêtes véhicule avec timer
```xml
<CustomBehavior File="MountVehOnly" VehicleMountId="44057" ... />
<CustomBehavior File="WaitTimer" WaitTime="122000" />  <!-- ~2 min -->
```

### 4. UserSettings temporaires
```xml
<CustomBehavior File="UserSettings" PullDistance="1" />
<!-- action -->
<CustomBehavior File="UserSettings" PullDistance="25" />  <!-- reset -->
```

### 5. RunMacro pour boutons UI
```xml
<CustomBehavior File="RunMacro" QuestId="29111" Macro="/click BonusActionButton2" NumOfTimes="15" WaitTime="10000" />
```

---

## 📈 Estimation Complexité WotLK

| Type de Quête | % Estimé | Pattern HB |
|---------------|----------|------------|
| Kill/Collect simples | 60% | Objective sans condition |
| Accept/TurnIn chaînés | 15% | If avec condition |
| UseItem | 10% | While avec UseItem |
| InteractWith/Click | 8% | If/While avec InteractWith |
| Véhicule/Transport | 4% | MountVehOnly, UseTransport |
| Escort | 2% | Escort avec retry |
| Cas spéciaux | 1% | Manuel |

---

## 🚀 Prochaine Étape: Implémenter le Parser Intelligent

Le parser doit:
1. **Grouper les actions par quête** (quest_id)
2. **Détecter le type de chaque action** (kill, collect, use, click, talk, goto)
3. **Consulter Questie** pour:
   - Pré-requis (chaînage)
   - Type de TurnIn (NPC ou auto-complete)
   - Flags spéciaux (véhicule, escort)
4. **Générer les conditions appropriées** (If/While)
5. **Ajouter les WaitTimer** après actions spéciales
