using System;

namespace Styx.Logic.Combat
{
    /// <summary>
    /// FEAT-17: Spell cooldown tracking information.
    /// Represents the cooldown state of a single spell at a point in time.
    /// </summary>
    public readonly struct SpellCooldownInfo
    {
        /// <summary>Spell ID.</summary>
        public int SpellId { get; }

        /// <summary>Total cooldown duration.</summary>
        public TimeSpan Duration { get; }

        /// <summary>Time remaining on cooldown.</summary>
        public TimeSpan TimeLeft { get; }

        /// <summary>When the cooldown started.</summary>
        public DateTime StartTime { get; }

        /// <summary>Whether the spell is currently on cooldown.</summary>
        public bool IsOnCooldown => TimeLeft > TimeSpan.Zero;

        public SpellCooldownInfo(int spellId, TimeSpan duration, TimeSpan timeLeft, DateTime startTime)
        {
            SpellId = spellId;
            Duration = duration;
            TimeLeft = timeLeft;
            StartTime = startTime;
        }

        public override string ToString() =>
            $"Spell {SpellId}: {(IsOnCooldown ? $"{TimeLeft.TotalSeconds:F1}s / {Duration.TotalSeconds:F1}s" : "Ready")}";
    }
}
