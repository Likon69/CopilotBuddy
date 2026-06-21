using Styx;
using Styx.Common;
using Styx.Helpers;
using System.IO;
using System.Windows.Forms;

namespace Tyrael.Shared
{
    internal sealed class TyraelSettings : Settings
    {
        private static TyraelSettings _instance;

        internal static TyraelSettings Instance
        {
            get { return _instance ?? (_instance = new TyraelSettings()); }
        }

        internal TyraelSettings()
            : base(
                Path.Combine(Utilities.AssemblyDirectory,
                             string.Format(@"Settings/Tyrael/Tyrael-Settings-{0}-Rev{1}.xml", StyxWoW.Me.Name, Tyrael.Revision)))
        {
        }

        #region UI Settings
        [Setting, DefaultValue(true)]
        internal bool ChatOutput { get; set; }

        [Setting, DefaultValue(true)]
        internal bool ClickToMove { get; set; }

        [Setting, DefaultValue(false)]
        internal bool ContinuesHealingMode { get; set; }

        [Setting, DefaultValue(false)]
        internal bool PluginPulsing { get; set; }

        [Setting, DefaultValue(false)]
        internal bool RaidWarningOutput { get; set; }

        [Setting, DefaultValue(false)]
        internal bool UseSoftLock { get; set; }

        [Setting, DefaultValue(Keys.X)]
        internal Keys PauseKeyChoice { get; set; }

        [Setting, DefaultValue(ModifierKeys.Alt)]
        internal ModifierKeys ModKeyChoice { get; set; }
        #endregion

        #region Manual Settings
        #endregion
    }
}
