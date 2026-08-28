// Note: Original uses ResourceManager + .resx. Ported to hardcoded English strings
// since CopilotBuddy doesn't use .resx satellite assemblies.
// Twin Peaks and Battle for Gilneas entries kept for parity but are Cataclysm-only.

using System;
using System.Globalization;
using System.Resources;

namespace Bots.BGBuddy.Resources
{
    /// <summary>
    /// BGBuddy string resources. Hardcoded English values instead of .resx.
    /// </summary>
    internal static class BGBuddyResources
    {
        public static string AcceptingJoin => "Accepting battleground join...";
        public static string AlteracValley => "Alterac Valley";
        public static string ArathiBasin => "Arathi Basin";
        public static string Assault => "Assault";
        public static string BalindaIsDead => "Balinda is Dead";
        public static string BalindaStonehearth => "Balinda Stonehearth";
        public static string Battle => "Battle";
        public static string BattleForGilneas => "Battle For Gilneas"; // Cata only
        public static string Battleground_ClearTarget_Target_being_cleared_ => "Target being cleared.";
        public static string Battleground_CreateInteractFlagsBehavior_Moving_to_interact_with_flag => "Moving to interact with flag";
        public static string Battleground_CreateLootInsigniaBehavior_Moving_to_loot_the_player_corpse => "Moving to loot the player corpse";
        public static string Battleground_CreateTargetingSanityChecks_We_ve_gotten_to_far_away_from_the_original_target_spot__Blacklisting_target_for_5s_and_clearing_ => "We've gotten too far away from the original target spot. Blacklisting target for 5s and clearing.";
        public static string Battleground_SetHotspot_biggest_fight => "biggest fight";
        public static string Battleground_SetHotspot_Moving_to__0_ => "Moving to {0}";
        public static string Battleground_SetHotspot_Moving_to__0___Reason___1__ => "Moving to {0}. Reason: {1}";
        public static string BattlegroundEnded => "Battleground ended. We {0}!";
        public static string BgBotProfile_settings_ValidationEventHandler_Error_loading_BGBuddy_profile____0___ => "Error loading BGBuddy profile... {0}";
        public static string BGBuddySessionReport => "--- BGBuddy Session Report ---";
        public static string BiggestFight => "Biggest Fight";
        public static string BiggestFriendlyPack => "Biggest Friendly Pack";
        public static string BotName => "BGBuddy";
        public static string CanNotStartUnderLevel10 => "Cannot start BGBuddy under level 10";
        public static string CaptainGalvandar => "Captain Galvandar";
        public static string Conflicted => "Conflicted";
        public static string DebugLogPrefix => "[BGBuddy Debug] ";
        public static string DefendFriendlyFlagCarrier => "Defend Friendly Flag Carrier";
        public static string Died => "Died";
        public static string EnemyFlagCarrier => "Enemy Flag Carrier";
        public static string EnemyKeep => "Enemy Keep";
        public static string EyeOfTheStorm => "Eye of the Storm";
        public static string GeneralDrekThar => "General Drek'Thar";
        public static string GettingOfTheBoat => "Getting off the boat";
        public static string GUITitle => "BGBuddy Config";
        public static string IsleOfConquest => "Isle of Conquest";
        public static string LeavingBattleground => "Leaving battleground...";
        public static string LogPrefix => "[BGBuddy] ";
        public static string LootCorpses => "Loot Corpses";
        public static string Lost => "Lost";
        public static string Lost2 => "lost";
        public static string None => "None";
        public static string NothingElseToDo => "Nothing else to do";
        public static string NotSupported => "This battleground is not supported";
        public static string PullDistance => "Pull Distance";
        public static string Queue => "Queue";
        public static string Queue1 => "Queue #1";
        public static string Queue2 => "Queue #2";
        public static string QueueingUpFor => "Queueing up for {0}";
        public static string RandomBattleground => "Random Battleground";
        public static string Settings => "Settings";
        public static string Starting => "Starting ";
        public static string StartOfGame => "Start of Game";
        public static string StrandOfTheAncients => "Strand of the Ancients";
        public static string TwinPeaks => "Twin Peaks"; // Cata only
        public static string VanndarStormpike => "Vanndar Stormpike";
        public static string WaitingForBoat => "Waiting for boat";
        public static string WaitingForDeserter => "Waiting for deserter debuff to expire...";
        public static string WaitingGroupLeader => "Waiting for group leader to queue...";
        public static string WarsongGulch => "Warsong Gulch";
        public static string WinsLosses => "{0}: {1} wins, {2} losses";
        public static string Won => "Won";
        public static string Won2 => "won";
    }
}
