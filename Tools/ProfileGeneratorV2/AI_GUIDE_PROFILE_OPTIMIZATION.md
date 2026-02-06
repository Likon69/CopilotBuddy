# Guide Expert pour l'Amélioration des Profiles HonorBuddy
## Document de Référence pour Modèle IA

---

## CONTEXTE DU PROJET

### Version du Jeu
- **World of Warcraft: Wrath of the Lich King Classic (WotLK 3.3.5a)**
- Extension: Wrath of the Lich King
- Niveau max: 80
- Particularité: Pas de classes Death Knight disponibles à la création (débloquées au niveau 55)
- Pas de Monks, Pandaren, ou classes/races MoP+

### Structure du Projet
```
Tools/ProfileGeneratorV2/
├── zygor_parser_v2.py       # Parser principal Zygor → HBProfile
├── generate_all_v2.py       # Générateur batch
├── transport_data.py        # 24 routes de transport
├── output_v2/
│   ├── Alliance/            # ~109 profiles
│   ├── Horde/               # ~117 profiles
│   └── Transport/           # 24 profiles de transport
```

---

## ACCÈS À LA BASE DE DONNÉES QUESTIE

### Emplacement
```
c:\Users\Texy\Desktop\Questie-335\Database\
```

### Structure de la Database
```
Database/
├── questDB.lua      # Données des quêtes
├── npcDB.lua        # Données des NPCs (vendors, quest givers, mobs)
├── itemDB.lua       # Données des items
├── objectDB.lua     # Données des objets interactibles (GameObject)
├── Zones/           # Informations par zone
├── Classic/         # Données spécifiques Classic (1-60)
├── TBC/             # Données spécifiques TBC (60-70)
├── Wotlk/           # Données spécifiques WotLK (70-80)
└── QuestXP/         # XP des quêtes
```

### Clés de la Table Quest (questDB.lua)
```lua
QuestieDB.questKeys = {
    ['name'] = 1,              -- Nom de la quête
    ['startedBy'] = 2,         -- {creatureStart, objectStart, itemStart}
    ['finishedBy'] = 3,        -- {creatureEnd, objectEnd}
    ['requiredLevel'] = 4,     -- Niveau requis
    ['questLevel'] = 5,        -- Niveau de la quête
    ['requiredRaces'] = 6,     -- Bitmask races
    ['requiredClasses'] = 7,   -- Bitmask classes
    ['objectivesText'] = 8,    -- Description des objectifs
    ['objectives'] = 10,       -- {creatureObjective, objectObjective, itemObjective, ...}
    ['preQuestGroup'] = 12,    -- Quêtes prérequises (toutes requises)
    ['preQuestSingle'] = 13,   -- Quêtes prérequises (une seule suffit)
    ['zoneOrSort'] = 17,       -- Zone ID
}
```

### Clés de la Table NPC (npcDB.lua)
```lua
QuestieDB.npcKeys = {
    ['name'] = 1,              -- Nom du NPC
    ['minLevelHealth'] = 2,    -- HP min
    ['maxLevelHealth'] = 3,    -- HP max
    ['minLevel'] = 4,          -- Niveau min
    ['maxLevel'] = 5,          -- Niveau max
    ['rank'] = 6,              -- Rang (0=normal, 1=elite, 2=boss)
    ['spawns'] = 7,            -- {[zoneID] = {{x, y}, ...}, ...}
    ['waypoints'] = 8,         -- Points de patrol
    ['zoneID'] = 9,            -- Zone principale
    ['factionID'] = 10,        -- Faction
    ['friendlyToFaction'] = 11,-- Amical pour quelle faction
    ['subName'] = 12,          -- Sous-titre (<Vendor>, <Quest>)
    ['npcFlags'] = 13,         -- Flags (vendor, repair, etc.)
}
```

### Comment Interroger la Database
Pour obtenir des coordonnées de vendeurs, mailboxes, ou NPCs, il faut:
1. Lire le fichier `npcDB.lua` ou `Wotlk/npcData.lua`
2. Parser les tables Lua
3. Extraire les `spawns` avec coordonnées (format: zoneID → [{x, y}, ...])

---

## ANALYSE COMPARATIVE: PROFILS PREMIUM vs GÉNÉRÉS

### Ce que font les Profils Premium (Cava)

#### 1. Section Header Complète
```xml
<HBProfile>
  <Name>[H-Quest](01-05)Orc[Cava]</Name>
  <MinLevel>1</MinLevel>
  <MaxLevel>5</MaxLevel>
  <MinDurability>0.3</MinDurability>
  <MinFreeBagSlots>1</MinFreeBagSlots>
  
  <!-- BLACKSPOTS - Zones à éviter -->
  <Blackspots>
    <Blackspot X="-397.3811" Y="-4108.694" Z="50.1936" Radius="35" />
    <Blackspot X="-171.0536" Y="-4698.67" Z="11.5506" Radius="25" />
  </Blackspots>
  
  <!-- VENDORS - NPCs pour vendre/réparer -->
  <Vendors>
    <Vendor Name="Zlagk" Entry="3882" Type="Food" X="-628.4" Y="-4237.7" Z="38.1" />
    <Vendor Name="Huklah" Entry="3160" Type="Repair" X="-617.8" Y="-4210.1" Z="38.1" />
  </Vendors>
  
  <!-- MAILBOX -->
  <Mailboxes>
    <Mailbox X="-607.6" Y="-4253.3" Z="38.6" />
  </Mailboxes>
</HBProfile>
```

#### 2. Quest Overrides avec Hotspots Détaillés
Les profils premium ont des `Quest Overrides` avec des zones de chasse précises:
```xml
<Quest Id="25134" Name="Lazy Peons">
  <Objective Type="InteractWith" MobId="10556" NumOfTimes="4">
    <Hotspots>
      <Hotspot X="-621.914" Y="-4349.424" Z="41.0" />
      <Hotspot X="-647.916" Y="-4294.271" Z="40.7" />
      <Hotspot X="-507.414" Y="-4376.126" Z="46.2" />
    </Hotspots>
  </Objective>
</Quest>
```

#### 3. Branchement par Classe
```xml
<If Condition="Me.Class == WoWClass.Warrior">
  <PickUp QuestName="Simple Parchment" QuestId="2383" ... />
  <TurnIn QuestName="Simple Parchment" QuestId="2383" ... />
  <PickUp QuestName="Charge" QuestId="25147" ... />
  <While Condition="HasQuest(25147) &amp;&amp; !IsQuestCompleted(25147)">
    <CustomBehavior File="InteractWith" InteractByCastingSpellId="100" MobId="44820" />
  </While>
  <TurnIn QuestName="Charge" QuestId="25147" ... />
</If>
```

#### 4. Boucles While pour Fiabilité
```xml
<While Condition="HasQuest(25134) &amp;&amp; !IsQuestCompleted(25134)">
  <CustomBehavior File="InteractWith" QuestId="25134" MobId="10556" 
    NumOfTimes="1000" AuraIdOnMob="17743" InteractByUsingItemId="16114">
    <HuntingGrounds>
      <Hotspot X="-621.9144" Y="-4349.424" Z="41.01179" />
      <Hotspot X="-647.916" Y="-4294.271" Z="40.74682" />
    </HuntingGrounds>
  </CustomBehavior>
</While>
```

#### 5. Gestion des Sacs
```xml
<If Condition="((Lua.GetReturnVal&lt;int&gt;(&quot;return GetContainerNumSlots(1)&quot;, 0)) &lt; 6) &amp;&amp; Me.Silver >= 5">
  <CustomBehavior File="Message" Text="Buying bag..." LogColor="Aquamarine" />
  <RunTo X="-779.79" Y="-4938.55" Z="22.24" />
  <CustomBehavior File="InteractWith" MobId="3186" InteractByBuyingItemId="4496" WaitTime="4000" />
</If>
```

#### 6. Hébergement (Hearthstone)
```xml
<CustomBehavior File="Cava\GoSavedInn" BindPointAreaId="363" />
```

#### 7. Vérification de Race au Début
```xml
<If Condition="Me.Race != WoWRace.Orc &amp;&amp; Me.Race != WoWRace.Troll">
  <CustomBehavior File="LoadProfile" ProfileName="[Wrong Race Profile]" />
</If>
```

#### 8. Chaînage de Profiles
```xml
<CustomBehavior File="LoadProfile" ProfileName="[H-Quest](05-11)TirisfalGlades[Cava]" />
```

---

### Ce que NOS Profils Générés Manquent

| Élément | Premium | Nos Profils | Priorité |
|---------|---------|-------------|----------|
| Vendors | ✅ Avec coords | ❌ Absent | HAUTE |
| Mailboxes | ✅ Avec coords | ❌ Absent | HAUTE |
| Blackspots | ✅ Zones dangereuses | ❌ Absent | MOYENNE |
| While loops | ✅ Pour fiabilité | ❌ Absent | HAUTE |
| Class branching | ✅ Détaillé | ⚠️ Basique | MOYENNE |
| Bag management | ✅ Auto-achat | ❌ Absent | BASSE |
| Profile chaining | ✅ Automatique | ❌ Absent | HAUTE |
| Race verification | ✅ Au début | ❌ Absent | BASSE |
| HuntingGrounds | ✅ Dans InteractWith | ⚠️ Partiel | MOYENNE |
| SetHearthstone | ✅ Présent | ❌ Absent | BASSE |

---

## PATTERNS CRITIQUES À IMPLÉMENTER

### 1. Section Vendors (PRIORITÉ HAUTE)
Chaque zone doit avoir ses vendors déclarés:
```xml
<Vendors>
  <Vendor Name="NomVendor" Entry="ID_NPC" Type="Food" X="..." Y="..." Z="..." />
  <Vendor Name="NomVendor" Entry="ID_NPC" Type="Repair" X="..." Y="..." Z="..." />
</Vendors>
```

Types de vendors:
- `Food` - Vendeur de nourriture
- `Repair` - Réparateur d'armure
- `General` - Vendeur général

### 2. Section Mailboxes (PRIORITÉ HAUTE)
```xml
<Mailboxes>
  <Mailbox X="..." Y="..." Z="..." />
</Mailboxes>
```

### 3. While Loop Pattern (PRIORITÉ HAUTE)
Toujours wrapper les objectifs complexes:
```xml
<While Condition="HasQuest(QUEST_ID) &amp;&amp; !IsQuestCompleted(QUEST_ID)">
  <!-- actions -->
</While>
```

### 4. Grind Areas avec Factions
```xml
<SetGrindArea>
  <GrindArea>
    <Name>Description de la zone</Name>
    <Factions>ID_FACTION_HOSTILE</Factions>
    <TargetMinLevel>5</TargetMinLevel>
    <TargetMaxLevel>8</TargetMaxLevel>
    <Hotspots>
      <Hotspot X="..." Y="..." Z="..." />
    </Hotspots>
  </GrindArea>
</SetGrindArea>
```

### 5. InteractWith avec HuntingGrounds
```xml
<CustomBehavior File="InteractWith" QuestId="ID" MobId="MOB_ID" NumOfTimes="10">
  <HuntingGrounds>
    <Hotspot X="..." Y="..." Z="..." />
    <Hotspot X="..." Y="..." Z="..." />
  </HuntingGrounds>
</CustomBehavior>
```

### 6. Profile Chaining
À la fin de chaque profile:
```xml
<CustomBehavior File="LoadProfile" ProfileName="Next_Zone_Profile.xml" />
```

---

## CUSTOM BEHAVIORS DISPONIBLES

### Fichiers de Comportement Standard
- `InteractWith` - Interaction avec NPCs/objets
- `Message` - Affichage de messages
- `LoadProfile` - Chargement d'un autre profile
- `UseTransport` - Utilisation de transports (bateaux, zeppelins)
- `TaxiRide` - Prise de vols
- `ForceSetVendor` - Force vente/réparation
- `SetHearthstone` - Définir pierre de foyer

### Fichiers Cava Custom (si disponibles)
- `Cava\CastSpellOn` - Cast spell sur cible
- `Cava\GoSavedInn` - Retour à l'auberge
- `Cava\BuyBags` - Achat automatique de sacs

---

## RÈGLES DE CONVERSION IMPORTANTES

### 1. QuestIDs
Les IDs de quêtes 3.3.5a sont DIFFÉRENTS des IDs Cataclysm+.
Nos IDs viennent de Zygor 3.3.5 donc sont corrects.

### 2. Coordonnées
Format HonorBuddy: `X="..." Y="..." Z="..."`
Attention: Certaines coordonnées Zygor peuvent avoir des erreurs de Z.

### 3. NPC IDs vs Object IDs
- NPCs: utiliser `MobId` ou `Entry`
- GameObjects: utiliser `ObjectId`
- Items: utiliser `ItemId`

### 4. Conditions Lua
```xml
<!-- Classes -->
Me.Class == WoWClass.Warrior
Me.Class == WoWClass.Paladin
Me.Class == WoWClass.Hunter
Me.Class == WoWClass.Rogue
Me.Class == WoWClass.Priest
Me.Class == WoWClass.Shaman
Me.Class == WoWClass.Mage
Me.Class == WoWClass.Warlock
Me.Class == WoWClass.Druid
Me.Class == WoWClass.DeathKnight

<!-- Races Horde -->
Me.Race == WoWRace.Orc
Me.Race == WoWRace.Undead
Me.Race == WoWRace.Tauren
Me.Race == WoWRace.Troll
Me.Race == WoWRace.BloodElf

<!-- Races Alliance -->
Me.Race == WoWRace.Human
Me.Race == WoWRace.Dwarf
Me.Race == WoWRace.NightElf
Me.Race == WoWRace.Gnome
Me.Race == WoWRace.Draenei

<!-- État des quêtes -->
HasQuest(ID)
IsQuestCompleted(ID)
!HasQuest(ID)
!IsQuestCompleted(ID)
```

---

## TÂCHES D'AMÉLIORATION PAR PRIORITÉ

### HAUTE PRIORITÉ
1. **Ajouter Vendors** - Interroger npcDB pour trouver vendors par zone
2. **Ajouter Mailboxes** - Chercher dans objectDB (type mailbox)
3. **Wrapper avec While** - Tous les objectifs complexes
4. **Profile Chaining** - Lier les profiles par niveau/zone

### MOYENNE PRIORITÉ
5. **Blackspots** - Zones à éviter (camps mobs dangereux)
6. **HuntingGrounds améliorés** - Plus de hotspots par objectif
7. **Class quests** - Vérifier tous les branchements classe

### BASSE PRIORITÉ
8. **Bag management** - Auto-achat sacs
9. **SetHearthstone** - Définir foyer par zone
10. **Race verification** - Vérifier race au début

---

## EXEMPLE DE TRANSFORMATION

### AVANT (Notre Profil)
```xml
<PickUp QuestName="Cutting Teeth" QuestId="788" GiverName="Gornek" GiverId="3143" />
<Objective QuestName="Cutting Teeth" QuestId="788" Type="KillMob" MobId="3098" KillCount="10" />
<TurnIn QuestName="Cutting Teeth" QuestId="788" TurnInName="Gornek" TurnInId="3143" />
```

### APRÈS (Style Premium)
```xml
<!-- En-tête avec Vendors -->
<Vendors>
  <Vendor Name="Zlagk" Entry="3882" Type="Food" X="-628.4" Y="-4237.7" Z="38.1" />
</Vendors>

<!-- QuestOrder -->
<PickUp QuestName="Cutting Teeth" QuestId="788" GiverName="Gornek" GiverId="3143" 
        X="-597.74" Y="-4248.26" Z="38.96" />

<While Condition="HasQuest(788) &amp;&amp; !IsQuestCompleted(788)">
  <CustomBehavior File="InteractWith" QuestId="788" MobId="3098" NumOfTimes="10"
                  ProactiveCombatStrategy="ClearMobsTargetingUs">
    <HuntingGrounds>
      <Hotspot X="-580.0" Y="-4300.0" Z="38.0" />
      <Hotspot X="-560.0" Y="-4320.0" Z="39.0" />
      <Hotspot X="-540.0" Y="-4280.0" Z="38.0" />
    </HuntingGrounds>
  </CustomBehavior>
</While>

<TurnIn QuestName="Cutting Teeth" QuestId="788" TurnInName="Gornek" TurnInId="3143"
        X="-597.74" Y="-4248.26" Z="38.96" />
```

---

## NOTES TECHNIQUES IMPORTANTES

### Échappement XML
- `&` → `&amp;`
- `<` → `&lt;`
- `>` → `&gt;`
- `"` → `&quot;`
- `'` → `&apos;`

### Version HonorBuddy
Ces profiles sont pour HonorBuddy compatible WotLK 3.3.5a.
Les CustomBehaviors modernes peuvent différer des versions MoP.

### Test des Profiles
Toujours tester:
1. Le profile se charge sans erreur XML
2. Les quêtes sont ramassées correctement
3. Les objectifs se complètent
4. Les TurnIn fonctionnent
5. La transition vers le profile suivant fonctionne

---

## RÉSUMÉ POUR LE MODÈLE IA

Tu dois améliorer des profiles HonorBuddy XML pour WoW WotLK 3.3.5a.

**Ce que tu dois faire:**
1. Ajouter des sections `<Vendors>` et `<Mailboxes>` dans le header
2. Wrapper les objectifs complexes avec `<While>` loops
3. Ajouter des coordonnées X/Y/Z aux PickUp et TurnIn
4. Enrichir les HuntingGrounds avec plus de Hotspots
5. Ajouter le chaînage vers le profile suivant à la fin

**Base de données disponible:**
- `c:\Users\Texy\Desktop\Questie-335\Database\` contient toutes les infos NPCs, quêtes, items
- Utilise npcDB.lua pour les coordonnées de vendors
- Utilise questDB.lua pour vérifier les quêtes

**Format des coordonnées:**
- Toujours 3 valeurs: X, Y, Z
- Z est l'altitude (généralement entre 0 et 500)

**Classes disponibles 3.3.5:**
Warrior, Paladin, Hunter, Rogue, Priest, Shaman, Mage, Warlock, Druid, DeathKnight

**Races disponibles 3.3.5:**
- Horde: Orc, Undead, Tauren, Troll, BloodElf
- Alliance: Human, Dwarf, NightElf, Gnome, Draenei
