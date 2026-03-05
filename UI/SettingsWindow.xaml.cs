using System;
using System.Windows;
using System.Windows.Controls;
using Styx.Helpers;

namespace CopilotBuddy.UI
{
    /// <summary>
    /// Settings window for CopilotBuddy, similar to HB WoD settings
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            
            // Set DataContext to enable bindings to CharacterSettings and StyxSettings
            DataContext = new SettingsDataContext
            {
                CharSettings = CharacterSettings.Instance,
                StyxSettings = StyxSettings.Instance
            };

            // Initialize log level ComboBox
            InitializeLogLevelComboBox();

            // Restore window position/size
            try
            {
                if (UISettings.Instance.SettingsWindowWidth > 0)
                    this.Width = UISettings.Instance.SettingsWindowWidth;
                if (UISettings.Instance.SettingsWindowHeight > 0)
                    this.Height = UISettings.Instance.SettingsWindowHeight;
                if (UISettings.Instance.SettingsWindowLocationX >= 0)
                    this.Left = UISettings.Instance.SettingsWindowLocationX;
                if (UISettings.Instance.SettingsWindowLocationY >= 0)
                    this.Top = UISettings.Instance.SettingsWindowLocationY;
                if (UISettings.Instance.SettingsWindowState != WindowState.Minimized)
                    this.WindowState = UISettings.Instance.SettingsWindowState;
            }
            catch { /* Ignore errors on first run */ }
        }

        private void InitializeLogLevelComboBox()
        {
            // Set selected item based on current LoggingLevel
            int logLevelIndex = (int)StyxSettings.Instance.LoggingLevel;
            if (logLevelIndex >= 0 && logLevelIndex < cmbLogLevel.Items.Count)
            {
                cmbLogLevel.SelectedIndex = logLevelIndex;
            }
        }

        private void cmbLogLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbLogLevel.SelectedItem is ComboBoxItem selectedItem)
            {
                if (int.TryParse(selectedItem.Tag?.ToString(), out int level))
                {
                    StyxSettings.Instance.LoggingLevel = (LogLevel)level;
                }
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Save window position/size
            try
            {
                if (this.WindowState == WindowState.Normal)
                {
                    UISettings.Instance.SettingsWindowWidth = (int)this.Width;
                    UISettings.Instance.SettingsWindowHeight = (int)this.Height;
                    UISettings.Instance.SettingsWindowLocationX = (int)this.Left;
                    UISettings.Instance.SettingsWindowLocationY = (int)this.Top;
                }
                UISettings.Instance.SettingsWindowState = this.WindowState;
                UISettings.Instance.Save();
            }
            catch { /* Ignore errors */ }
            
            base.OnClosing(e);
        }

        private void btnSaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            // Save both settings
            CharacterSettings.Instance.Save();
            StyxSettings.Instance.Save();
            
            // Close the window
            Close();
        }

        private void btnChangeMeshPath_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select the folder containing mmaps (Trinity mesh tiles)",
                ShowNewFolderButton = true
            };

            // Set initial directory if path exists
            if (!string.IsNullOrEmpty(StyxSettings.Instance.MeshesFolderPath) && 
                System.IO.Directory.Exists(StyxSettings.Instance.MeshesFolderPath))
            {
                folderDialog.SelectedPath = StyxSettings.Instance.MeshesFolderPath;
            }

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StyxSettings.Instance.MeshesFolderPath = folderDialog.SelectedPath;
            }
        }
    }

    /// <summary>
    /// Data context for binding both CharacterSettings and StyxSettings
    /// </summary>
    public class SettingsDataContext
    {
        public CharacterSettings? CharSettings { get; set; }
        public StyxSettings? StyxSettings { get; set; }
    }
}
