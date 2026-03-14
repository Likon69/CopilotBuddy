using System;
using System.IO;
using Styx.Logic;

namespace Styx.Helpers
{
    public class BGBotSettings : Settings
    {
        public static readonly BGBotSettings Instance = new BGBotSettings();

        public BGBotSettings()
            : base(Path.Combine(Logging.ApplicationPath, string.Format("Settings\\BGBotSettings_{0}.xml", (StyxWoW.Me != null) ? StyxWoW.Me.Name : "")))
        {
        }

        [Setting("BG1", "The first BG you would like to queue for.")]
        [DefaultValue(BattlegroundType.WSG)]
        public BattlegroundType BG1 { get; set; }

        [Setting("BG2", "The second BG you would like to queue for.")]
        [DefaultValue(BattlegroundType.None)]
        public BattlegroundType BG2 { get; set; }

        [Setting("IncludeMountedPlayers", "Set this to true if you wish to include mounted players in battlegrounds.")]
        [DefaultValue(false)]
        public bool IncludeMountedPlayers { get; set; }

        [Setting("IsHealer", "Set this to true if you wish to only heal people and not target any players.")]
        [DefaultValue(false)]
        public bool IsHealer { get; set; }
    }
}
