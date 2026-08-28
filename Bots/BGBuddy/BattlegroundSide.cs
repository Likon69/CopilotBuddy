// Note: This is the BGBuddy-specific side enum (includes Attack/Defend for SotA).
// Distinct from Styx.Logic.BattlegroundSide which is faction-only.

using System;

namespace Bots.BGBuddy
{
    /// <summary>
    /// Represents which side the player is on inside a battleground.
    /// For SotA (map 607) this becomes Attack/Defend instead of Horde/Alliance.
    /// </summary>
    public enum BattlegroundSide
    {
        Horde,
        Alliance,
        Attack,
        Defend
    }
}
