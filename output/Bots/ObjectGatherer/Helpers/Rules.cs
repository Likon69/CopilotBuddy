using System;
using Styx;

namespace ObjectGatherer.Helpers {
    public class Rules {
        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        // ===========================================================
        // Constructors
        // ===========================================================

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        public static void Check() {
            if(Variables.LootObject != null) {
                if(Variables.LootObject.Entry == 214388 && !Variables.HasAncientGuoLaiCacheKey) {
                    CustomLog.Diagnostic("We don't have a Ancient Guo Lai Cache Key, blacklisting the Cache for 5 minutes.");
                    CustomBlacklist.Add(Variables.LootObject.Guid, TimeSpan.FromMinutes(5));
                    Variables.LootObject = null;
                }
            }

            // NOTE: Timber/Logging checks removed — SkillLine.Logging is WoD Garrisons content, not WotLK.
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

    }
}
