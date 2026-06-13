namespace Bots.DungeonBuddy.Enums
{
    /// <summary>
    /// Role to queue for in the Dungeon Finder.
    /// Ported from HB 5.4.8 / 6.2.3 DungeonBuddySettings.QueueRole.
    /// In 3.3.5a (WotLK), the LFG UI uses SetLFGRoles(GetLFGRoles(), tank, healer, dps)
    /// where each flag is true/false — only one role can be active at queue time.
    /// </summary>
    public enum QueueRole
    {
        /// <summary>
        /// Role is picked automatically based on the player's current talent spec
        /// (Tank spec → Tank, Healer spec → Healer, anything else → Damage).
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Queue as tank.
        /// </summary>
        Tank = 1,

        /// <summary>
        /// Queue as healer.
        /// </summary>
        Healer = 2,

        /// <summary>
        /// Queue as damage dealer.
        /// </summary>
        Damage = 3,
    }
}
