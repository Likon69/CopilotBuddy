namespace Styx.WoWInternals.WoWObjects
{
    /// <summary>
    /// FEAT-24: Structured power data for a unit's power type.
    /// Provides current/max values, percentages, regen rates, and cost modifiers.
    /// </summary>
    public readonly struct PowerInfo
    {
        public WoWPowerType Type { get; }
        public int Current { get; }
        public int Max { get; }
        public float Percent { get; }
        public float RegenFlat { get; }
        public float RegenInterrupted { get; }
        public int CostModifier { get; }
        public float CostMultiplier { get; }

        public PowerInfo(WoWPowerType type, int current, int max, float percent,
            float regenFlat, float regenInterrupted, int costModifier, float costMultiplier)
        {
            Type = type;
            Current = current;
            Max = max;
            Percent = percent;
            RegenFlat = regenFlat;
            RegenInterrupted = regenInterrupted;
            CostModifier = costModifier;
            CostMultiplier = costMultiplier;
        }

        public override string ToString() =>
            $"{Type}: {Current}/{Max} ({Percent:F1}%) Regen={RegenFlat:F1} RegenCombat={RegenInterrupted:F1}";
    }
}
