# ProfileGenerator - Plan de Développement

## Sources de Données Disponibles

### 1. Zygor Guides (Primary)
- **Fichier**: `ZygorLevelingHordeWOTLK.lua`
- **Contenu**: 86 guides avec ordre optimisé des quêtes
- **Format**: `accept QuestName##QuestID`, `turnin`, `kill N Mob##MobID`, `collect N Item##ItemID`
- **Coords**: `|goto ZoneName X,Y` (pourcentage de zone, pas coords monde)
- **Class filter**: `|only if BloodElf Paladin` etc.

### 2. Questie-335 Database
- **wotlkQuestDB.lua**: Quêtes avec objectives (mobs/items), giver/turnin NPCs
- **wotlkNpcDB.lua**: NPCs avec noms et spawns (coords zone %)
- **wotlkItemDB.lua**: Items avec noms

### 3. Database Serveur (À implémenter)
- **world.creature**: Coords monde exactes des NPCs  
- **world.gameobject**: Coords monde exactes des GameObjects
- **world.quest_template**: Données quêtes complètes
- Connexion via pymysql ou autre

## Architecture du Parser

```
Zygor Guide → Parser → Enrichissement Questie → Enrichissement DB → XML Profile
```

## Fonctionnalités Implémentées ✅

1. **Parsing Zygor**
   - ✅ accept/turnin/kill/collect
   - ✅ Extraction QuestID, MobID, ItemID, NpcID
   - ✅ Tracking quêtes actives (PickUp→TurnIn)

2. **Enrichissement Questie**
   - ✅ Index inversés mob→quests, item→quests
   - ✅ Résolution nom de quête pour objectives
   - ✅ Noms NPCs/Items

3. **Génération XML**
   - ✅ PickUp, TurnIn, Objective (KillMob, CollectItem)
   - ✅ QuestId, MobId, ItemId, NpcId, noms

## Fonctionnalités À Implémenter 🔲

### Phase 1: Class/Race Filtering
- 🔲 Parser `|only if Race Class` de Zygor
- 🔲 Générer `<If Condition="(Me.Class == WoWClass.X) && (Me.Race == WoWRace.Y)">`
- 🔲 Grouper les quêtes de classe ensemble

### Phase 2: Coordonnées Monde
- 🔲 Connexion à la DB serveur
- 🔲 Récupérer coords NPCs depuis `creature` table
- 🔲 Récupérer coords GameObjects depuis `gameobject` table
- 🔲 Ajouter `<Hotspots>` aux objectives
- 🔲 Convertir coords Zygor (zone %) → coords monde (si pas en DB)

### Phase 3: Quest Overrides
- 🔲 Générer section `<Quest>` pour objectives complexes
- 🔲 CollectFrom avec GameObject/Mob
- 🔲 Hotspots multiples par objectif

### Phase 4: Améliorations
- 🔲 Vendors/Trainers depuis DB
- 🔲 Mailboxes depuis DB
- 🔲 AvoidMobs/Blackspots
- 🔲 CustomBehaviors (UseItemOn, WaitTimer, etc.)

## Format XML Cible

```xml
<!-- Quête de classe -->
<If Condition="(Me.Class == WoWClass.Paladin) &amp;&amp; (Me.Race == WoWRace.Dwarf)">
    <PickUp QuestName="Consecrated Rune" QuestId="3107" GiverName="Jona Ironstock" GiverId="37087" />
    <TurnIn QuestName="Consecrated Rune" QuestId="3107" TurnInName="Bromos Grummner" TurnInId="926" />
</If>

<!-- Objective avec Hotspots -->
<Objective QuestName="Kill Stuff" QuestId="123" Type="KillMob" MobId="456" KillCount="10">
    <Hotspots>
        <Hotspot X="-656.79" Y="-4274.15" Z="38.22" />
    </Hotspots>
</Objective>

<!-- Quest Override pour collect depuis GameObject -->
<Quest Id="24474" Name="First Things First">
    <Objective Type="CollectItem" ItemId="49746" CollectCount="1">
        <CollectFrom>
            <GameObject Name="Keg of Gnomenbrau" Id="201611" />
        </CollectFrom>
        <Hotspots>
            <Hotspot X="-6141.967" Y="321.3142" Z="399.2259" />
        </Hotspots>
    </Objective>
</Quest>
```

## Mapping Classes/Races

### WoWClass
- Warrior, Paladin, Hunter, Rogue, Priest, Shaman, Mage, Warlock, Druid, DeathKnight, Monk

### WoWRace  
- Human, Dwarf, NightElf, Gnome, Draenei, Worgen (Alliance)
- Orc, Troll, Tauren, Undead, BloodElf, Goblin (Horde)

## Notes Techniques

### Conversion Coords Zygor → Monde
Les coords Zygor sont en % de la zone (0-100). Pour convertir:
1. Récupérer les bounds de la zone depuis AreaTable.dbc
2. Appliquer: `worldX = zoneMinX + (zygorX/100) * (zoneMaxX - zoneMinX)`
3. Ou mieux: utiliser les coords de la DB serveur directement

### Tables DB Serveur Utiles
- `creature`: guid, id, map, position_x/y/z
- `creature_template`: entry, name
- `gameobject`: guid, id, map, position_x/y/z  
- `gameobject_template`: entry, name
- `quest_template`: entry, Title, objectives, etc.
