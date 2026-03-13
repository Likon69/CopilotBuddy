namespace Styx
{
    /// <summary>
    /// WotLK specialization IDs. In 3.3.5a these IDs map to the spec system
    /// introduced in Cata — present here for API compatibility with custom classes.
    /// Use SpecType for role-based detection (Tank/Healer/DPS) instead.
    /// </summary>
    public enum WoWSpec
    {
        None = 0,
        // Mage
        MageArcane        = 62,
        MageFire          = 63,
        MageFrost         = 64,
        // Paladin
        PaladinHoly       = 65,
        PaladinProtection = 66,
        PaladinRetribution = 70,
        // Warrior
        WarriorArms       = 71,
        WarriorFury       = 72,
        WarriorProtection = 73,
        // Hunter pets
        PetFerocity       = 74,
        PetCunning        = 79,
        PetTenacity       = 81,
        // Druid
        DruidBalance      = 102,
        DruidFeral        = 103,
        DruidRestoration  = 105,
        // Death Knight
        DeathKnightBlood  = 250,
        DeathKnightFrost  = 251,
        DeathKnightUnholy = 252,
        // Hunter
        HunterBeastMastery  = 253,
        HunterMarksmanship  = 254,
        HunterSurvival      = 255,
        // Priest
        PriestDiscipline  = 256,
        PriestHoly        = 257,
        PriestShadow      = 258,
        // Rogue
        RogueCombat       = 259,
        RogueAssassination = 260,
        RogueSubtlety     = 261,
        // Shaman
        ShamanElemental   = 262,
        ShamanEnhancement = 263,
        ShamanRestoration = 264,
        // Warlock
        WarlockAffliction  = 265,
        WarlockDemonology  = 266,
        WarlockDestruction = 267,
    }
}
