using System.ComponentModel;
using System.IO;
using System.Windows;

namespace Styx.Helpers
{
    /// <summary>
    /// Settings for UI window positions and sizes.
    /// Global (not per-character) — stored in Settings/UISettings.xml.
    /// Pattern from HB 5.4.8.
    /// </summary>
    public class UISettings : Settings
    {
        public static readonly UISettings Instance = new UISettings();

        public UISettings()
            : base(Path.Combine(SettingsDirectory, "UISettings.xml"))
        {
        }

        #region Main Window

        [Setting]
        [DefaultValue(-1)]
        public int MainWindowLocationX { get; set; } = -1;

        [Setting]
        [DefaultValue(-1)]
        public int MainWindowLocationY { get; set; } = -1;

        [Setting]
        [DefaultValue(531)]
        public int MainWindowWidth { get; set; } = 531;

        [Setting]
        [DefaultValue(350)]
        public int MainWindowHeight { get; set; } = 350;

        [Setting]
        [DefaultValue(WindowState.Normal)]
        public WindowState MainWindowState { get; set; } = WindowState.Normal;

        #endregion

        #region Settings Window

        [Setting]
        [DefaultValue(20)]
        public int SettingsWindowLocationX { get; set; } = 20;

        [Setting]
        [DefaultValue(20)]
        public int SettingsWindowLocationY { get; set; } = 20;

        [Setting]
        [DefaultValue(508)]
        public int SettingsWindowWidth { get; set; } = 508;

        [Setting]
        [DefaultValue(408)]
        public int SettingsWindowHeight { get; set; } = 408;

        [Setting]
        [DefaultValue(WindowState.Normal)]
        public WindowState SettingsWindowState { get; set; } = WindowState.Normal;

        #endregion

        #region Enhanced Mode

        [Setting]
        [DefaultValue(false)]
        public bool EnhancedMode { get; set; } = false;

        #endregion
    }
}
