using System;

namespace Styx.WoWInternals.WoWObjects
{
    /// <summary>
    /// Matches the Honorbuddy enumeration for player alcohol status.
    /// This is present in later WoW versions; in 3.3.5a it is unused, but
    /// we port it for API compatibility with HB 4.3.4.
    /// </summary>
    public enum WoWInebriationLevel
    {
        Sober,
        Tipsy,
        Drunk,
        Smashed
    }
}
