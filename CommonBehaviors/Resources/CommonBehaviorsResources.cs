// Target path: CommonBehaviors/Resources/CommonBehaviorsResources.cs
// Strings shared by the reusable behavior tree actions. Hardcoded English
// values (no .resx), matching the style of BGBuddyResources.

namespace CommonBehaviors.Resources
{
    /// <summary>
    /// String resources shared by CommonBehaviors and consumed by other bots
    /// (e.g. BGBuddy's AlteracValley boss-kill logging).
    /// </summary>
    internal static class CommonBehaviorsResources
    {
        public static string GalvandarIsDead => "Galvandar is dead";
        public static string NothingElseToDoWaitingTower => "Nothing else to do (waiting on tower)";
    }
}
