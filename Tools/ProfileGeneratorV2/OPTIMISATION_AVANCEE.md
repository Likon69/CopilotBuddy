# Guide d'Optimisation Avancée des Profils HonorBuddy

Basé sur l'analyse des profils premium Cava et TBMC.

---

## 1. HEADER - Paramètres Globaux Manquants

### Ce que vous avez:
```xml
<MinDurability>0.2</MinDurability>
<MinFreeBagSlots>2</MinFreeBagSlots>
```

### Ce qu'il faut ajouter:
```xml
<!-- Gestion du courrier automatique -->
<MailGrey>False</MailGrey>
<MailWhite>False</MailWhite>
<MailGreen>True</MailGreen>
<MailBlue>True</MailBlue>
<MailPurple>True</MailPurple>

<!-- Gestion des ventes automatiques -->
<SellGrey>True</SellGrey>
<SellWhite>True</SellWhite>
<SellGreen>False</SellGreen>
<SellBlue>False</SellBlue>
<SellPurple>False</SellPurple>

<!-- Éviter les élites -->
<TargetElites>False</TargetElites>
```

---

## 2. ZONES DE SÉCURITÉ - Blackspots & AvoidMobs

### Blackspots (zones à éviter)
Ajouter des blackspots pour les zones dangereuses:
```xml
<Blackspots>
  <!-- Zone de boss, garde élite, etc -->
  <Blackspot X="-8908.086" Y="-130.6742" Z="80.82938" Radius="15" />
</Blackspots>
```

### AvoidMobs (mobs à éviter)
```xml
<AvoidMobs>
  <Mob Name="Guard Elites" Id="12345" />
</AvoidMobs>
```

---

## 3. INFRASTRUCTURE - Vendors & Mailboxes

### Ajouter des vendeurs par zone:
```xml
<Vendors>
  <Vendor Name="Innkeeper" Entry="6741" Type="Food" X="331.45" Y="-4731.91" Z="11.06" />
  <Vendor Name="Blacksmith" Entry="3165" Type="Repair" X="327.13" Y="-4724.91" Z="10.83" />
</Vendors>

<Mailboxes>
  <Mailbox X="328.45" Y="-4729.21" Z="10.93" />
</Mailboxes>
```

---

## 4. BEHAVIOURS MANQUANTS

### 4.1 Initialisation du profil
```xml
<QuestOrder>
  <!-- Vérification de race au démarrage -->
  <If Condition="Me.Race != WoWRace.Orc &amp;&amp; Me.Race != WoWRace.Troll">
    <CustomBehavior File="LoadProfile" ProfileName="Next[Profile]" />
  </If>
  
  <!-- Configuration utilisateur -->
  <CustomBehavior File="UserSettings" 
    GroundMountFarmingMode="False" 
    KillBetweenHotspots="False" 
    AutoEquip="true" />
  
  <!-- Activation plugins utiles -->
  <CustomBehavior File="EnablePlugin" Names="Anti Drown" />
  
  <!-- Auto-loot activé -->
  <CustomBehavior File="RunMacro" 
    Macro="/script SetCVar('AutoLootDefault', 1)" 
    NumOfTimes="1" 
    WaitTime="1000" />
  
  <!-- Message de démarrage -->
  <CustomBehavior File="Message" 
    Text="Démarrage du profil Durotar (1-12)" 
    LogColor="Aquamarine" />
</QuestOrder>
```

### 4.2 InteractWith Avancé (pour quêtes d'interaction)
Au lieu de simple `<Objective>`, utiliser:
```xml
<If Condition="HasQuest(1234) &amp;&amp; !IsQuestCompleted(1234)">
  <While Condition="!IsQuestCompleted(1234)">
    <CustomBehavior File="InteractWith" 
      QuestId="1234" 
      MobId="5678" 
      NumOfTimes="1000" 
      CollectionDistance="100" 
      ProactiveCombatStrategy="ClearMobsTargetingUs" 
      Range="5">
      <HuntingGrounds>
        <Hotspot X="100" Y="200" Z="50" />
        <Hotspot X="150" Y="250" Z="52" />
      </HuntingGrounds>
    </CustomBehavior>
  </While>
</If>
```

### 4.3 Cast Spell on Target (quêtes de classe)
```xml
<CustomBehavior File="InteractWith" 
  InteractByCastingSpellId="1234"  <!-- ID du sort -->
  MobId="5678" 
  NumOfTimes="3" 
  X="100" Y="200" Z="50" />
```

---

## 5. GESTION DES BAGS AUTOMATIQUE

```xml
<!-- Vérifier si bag slot 1 a moins de 6 slots et acheter un sac -->
<If Condition="((Lua.GetReturnVal&lt;int&gt;(&quot;return GetContainerNumSlots(1)&quot;, 0)) &lt; 6) &amp;&amp; Me.Silver &gt;= 5">
  <CustomBehavior File="Message" 
    Text="Achat d'un sac automatique..." 
    LogColor="Aquamarine" />
  <CustomBehavior File="InteractWith" 
    MobId="152" 
    InteractByBuyingItemId="4496"  <!-- Small Brown Pouch -->
    WaitTime="4000" 
    X="-8901.59" Y="-112.716" Z="81.84904" />
</If>
```

---

## 6. GRIND INTELLIGENT

### Au lieu de simple GrindTo:
```xml
<If Condition="Me.LevelFraction &lt; 5.4">
  <SetGrindArea>
    <GrindArea>
      <Name>Grind Zone - Scorpids</Name>
      <MobIDs>3124,3125</MobIDs>  <!-- Liste de MobIds à cibler -->
      <Factions>189</Factions>
      <TargetMinLevel>4</TargetMinLevel>
      <TargetMaxLevel>6</TargetMaxLevel>
      <Hotspots>
        <Hotspot X="100" Y="200" Z="50" />
        <Hotspot X="150" Y="250" Z="52" />
      </Hotspots>
    </GrindArea>
  </SetGrindArea>
  <GrindTo Condition="Me.LevelFraction &gt;= 5.4" />
</If>
```

---

## 7. RÉPARER/VENDRE AVANT TRANSPORT

```xml
<If Condition="HasQuest(1234)">
  <!-- Forcer vente/réparation/mail -->
  <CustomBehavior File="ForceSetVendor" 
    DoMail="True" 
    DoSell="True" 
    DoRepair="True" />
  
  <!-- Aller au vendeur -->
  <RunTo X="100" Y="200" Z="50" />
  <CustomBehavior File="InteractWith" 
    MobId="3165" 
    WaitTime="1000" />
</If>
```

---

## 8. TRANSPORTS - UseTransport OPTIMISÉ

```xml
<!-- Attendre le zeppelin avec toutes les coordonnées -->
<CustomBehavior File="UseTransport" 
  TransportId="164871"
  WaitAtX="1677.41" WaitAtY="-4313.2" WaitAtZ="61.67"
  TransportStartX="1676.41" TransportStartY="-4318.65" TransportStartZ="67.56"
  TransportEndX="2057.48" TransportEndY="240.3" TransportEndZ="99.74"
  StandOnX="1674.84" StandOnY="-4314.87" StandOnZ="67.58"
  GetOffX="2061.66" GetOffY="245.65" GetOffZ="100.25"
  WaitTime="60000" />  <!-- Timeout -->
```

---

## 9. TAXI/FLIGHT PATH

```xml
<If Condition="Me.ZoneId == 14">  <!-- Durotar -->
  <CustomBehavior File="Message" 
    Text="Utilisation du taxi vers Orgrimmar" 
    LogColor="Aquamarine" />
  <RunTo X="276.68" Y="-4319.87" Z="25.11" />
  <CustomBehavior File="Cava\TaxiRide" 
    MobId="3615" 
    MobState="Alive" 
    WaitForNpcs="True" 
    TaxiNumber="1"  <!-- Numéro de la destination -->
    WaitTime="5000" />
</If>
```

---

## 10. DISABLE/ENABLE BEHAVIORS

Pour éviter de pull des mobs pendant les déplacements:
```xml
<If Condition="HasQuest(1234)">
  <DisableBehavior Name="Pull" />
  <RunTo X="100" Y="200" Z="50" />
  <TurnIn QuestName="Quest" QuestId="1234" ... />
  <EnableBehavior Name="Pull" />
</If>
```

---

## 11. CONDITIONS AVANCÉES

### Vérifications utiles:
```lua
-- Niveau et fraction
Me.Level >= 20
Me.LevelFraction < 10.5  -- Niveau 10, 50% XP

-- Or
Me.Gold >= 50
Me.Silver >= 5

-- Classe/Race
Me.Class == WoWClass.Warrior
Me.Race == WoWRace.Orc

-- Zone/Continent
Me.MapId == 1   -- Kalimdor
Me.MapId == 0   -- Eastern Kingdoms
Me.MapId == 530 -- Outland
Me.ZoneId == 14 -- Durotar

-- Skills
Me.GetSkill(Styx.SkillLine.Riding).CurrentValue >= 75

-- Montures
Mount.GroundMounts.Count() == 0
Mount.FlyingMounts.Count() == 0

-- Spells
HasSpell(90267)  -- Vérifier si le joueur a un spell

-- Bags
Lua.GetReturnVal<int>("return GetContainerNumSlots(1)", 0) < 6
```

---

## 12. CHAÎNAGE DE PROFILS

À la fin de chaque profil:
```xml
<!-- Navigation vers le prochain profil -->
<If Condition="Me.Level >= 12 &amp;&amp; Me.ZoneId == 14">
  <CustomBehavior File="Message" 
    Text="Zone terminée! Passage à Barrens..." 
    LogColor="Green" />
  <CustomBehavior File="LoadProfile" 
    ProfileName="Horde\Leveling_Guides__The_Barrens_(12-15).xml" />
</If>
```

---

## 13. LootMobs Dynamique

Pour les quêtes de collecte où le loot est important:
```xml
<If Condition="HasQuest(1234) &amp;&amp; !IsQuestCompleted(1234)">
  <LootMobs Value="true" />
  <While Condition="!IsQuestCompleted(1234)">
    <!-- ... Objectifs ... -->
  </While>
</If>
<If Condition="IsQuestCompleted(1234)">
  <LootMobs Value="null" />  <!-- Retour au défaut -->
</If>
```

---

## 14. ACHAT AUTOMATIQUE DE MONTURE

```xml
<If Condition="Me.Level >= 20 &amp;&amp; Me.GetSkill(Styx.SkillLine.Riding).CurrentValue == 0 &amp;&amp; Me.Gold >= 4">
  <CustomBehavior File="ForceTrainRiding" MobId="4752" />  <!-- Trainer -->
  
  <If Condition="Mount.GroundMounts.Count() == 0 &amp;&amp; Me.Gold >= 1">
    <CustomBehavior File="InteractWith" 
      MobId="4730" 
      InteractByBuyingItemId="8629"  <!-- Monture -->
      WaitTime="2000" />
    <CustomBehavior File="Misc\RunLua" 
      Lua="UseItemByName(8629)" 
      WaitTime="2000" />
  </If>
</If>
```

---

## RÉSUMÉ DES AMÉLIORATIONS PRIORITAIRES

| Priorité | Amélioration | Impact |
|----------|--------------|--------|
| **1** | Ajouter Vendors/Mailboxes | Autonomie du bot |
| **2** | Blackspots zones dangereuses | Éviter les morts |
| **3** | Vérification race au départ | Redirection correcte |
| **4** | ForceSetVendor avant transport | Pas de perte d'items |
| **5** | While + InteractWith avancé | Quêtes plus fiables |
| **6** | LoadProfile en fin de zone | Enchaînement fluide |
| **7** | Grind avec MobIDs spécifiques | XP optimisé |
| **8** | Disable Pull pendant déplacements | Moins de morts |

---

## FICHIERS DE RÉFÉRENCE

- **Cava**: `Honorbuddy-MoP-5.4.8-18414-main\Default Profiles\Cava\Scripts\`
- **TBMC**: `Honorbuddy-MoP-5.4.8-18414-main\Default Profiles\TBMC\Questing\`

Ces profils utilisent des CustomBehaviors spécifiques comme `Cava\TaxiRide` et `Cava\CastSpellOn` qui nécessitent le plugin Cava pour fonctionner.
