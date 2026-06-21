// Originally contributed by Chinajade
//
// LICENSE:
// This work is licensed under the
//     Creative Commons Attribution-NonCommercial-ShareAlike 4.0 Unported License.
// also known as CC-BY-NC-SA.  To view a copy of this license, visit
//      http://creativecommons.org/licenses/by-nc-sa/4.0/
// or send a letter to
//      Creative Commons // 171 Second Street, Suite 300 // San Francisco, California, 94105, USA.
//

#region Usings

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using BuddyControlPanel.Resources.Localization;

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    public class Dialog_Preferences : Assets.ThemedWindow, IDisposable
    {
        #region Construction and Destruction
        private Dialog_Preferences()
        {
            // Capture original configuration...
            _originalConfiguration_ControlBar = Overlay_ControlBar.Instance.Configuration_Get();
            _originalConfiguration_Keybinds = KeybindManager.Instance.Configuration_Get();
            _originalConfiguration_StatusDisplay = Overlay_StatusDisplay.Instance.Configuration_Get();

            Height = 300;
            ResizeMode = ResizeMode.NoResize;
            Title = string.Format(BCPGlobalization.TitleFormat_Preferences, BCPGlobalization.PluginName);
            Width = 400;

            var gridForWindow = new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() {Width = new GridLength(1.0, GridUnitType.Star)},
                },
                RowDefinitions =
                {
                    new RowDefinition() {Height = new GridLength(1.0, GridUnitType.Star)},
                    new RowDefinition() {Height = GridLength.Auto},
                },
            };
            Content = gridForWindow;

            var buddyBotLogo = new Image()
            {
                IsHitTestVisible = false,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(25, 15, 25, 5),
                Opacity = 0.25,
                Source = Utility.ToImageSource("logo-honorbuddy.png", 128),
                Stretch = Stretch.Uniform,
                VerticalAlignment = VerticalAlignment.Top,
            };
            gridForWindow.Children.Add(buddyBotLogo);
            Grid.SetColumn(buddyBotLogo, 0);
            Grid.SetColumnSpan(buddyBotLogo, 1);
            Grid.SetRow(buddyBotLogo, 0);
            Grid.SetRowSpan(buddyBotLogo, 2);
            Panel.SetZIndex(buddyBotLogo, 500);

            var borderForConfigurables = new Border()
            {
                Background = Assets.BrushTransparent,
                BorderBrush = Assets.BrushOpaqueBlack,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5, 5, 5, 5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            gridForWindow.Children.Add(borderForConfigurables);
            Grid.SetColumn(borderForConfigurables, 0);
            Grid.SetRow(borderForConfigurables, 0);
            Panel.SetZIndex(borderForConfigurables, 100);

            borderForConfigurables.Child = Build_Configurables();

            var decisionButtons = Build_DecisionButtons();
            gridForWindow.Children.Add(decisionButtons);
            Grid.SetColumn(decisionButtons, 0);
            Grid.SetRow(decisionButtons, 1);

            Closing += (sender, evt) =>
            {
                Visibility = Visibility.Hidden;
                evt.Cancel = true;
            };
        }


        // Basic Dispose pattern (ref: https://msdn.microsoft.com/en-us/library/b1yfkh5e%28v=vs.110%29.aspx)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            // Reclaim unmanaged resources (hooked elements) --

            s_instance = null;
        }

        private static Dialog_Preferences s_instance;
        public static Dialog_Preferences Instance
        {
            get
            {
                return s_instance ?? (s_instance = new Dialog_Preferences());
            }
        }

        /// <summary>
        /// De-initializes the singleton instance.
        /// </summary>
        public static void ToJunkyard()
        {
            if (s_instance != null)
            {
                s_instance.Dispose();
                s_instance = null;
            }
        }
        #endregion

        private CheckBox _checkBox_LockControlPositions;
        private ComboBox _comboBox_ControlBarSize;
        private ComboBox _comboBox_StatusDisplayFontSize;
        private UiControl_KeybindConfiguration _uiControl_KeybindStopStart;
        private UiControl_KeybindConfiguration _uiControl_KeybindPauseResume;
        private readonly BuddyControlPanelSettings.ControlBarSettings _originalConfiguration_ControlBar;
        private readonly BuddyControlPanelSettings.KeybindSettings _originalConfiguration_Keybinds;
        private readonly BuddyControlPanelSettings.StatusDisplaySettings _originalConfiguration_StatusDisplay;


        private Panel Build_Configurables()
        {
            var grid = new Grid()
            {
                Background = Assets.BrushTransparent,
                ColumnDefinitions =
                {
                    new ColumnDefinition() {Width = new GridLength(1.0, GridUnitType.Star)},
                },
                RowDefinitions =
                {
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = new GridLength(1.0, GridUnitType.Star)},
                },
            };

            var groupOverlayConfiguration = BuildGroup_OverlayConfiguration();
            grid.Children.Add(groupOverlayConfiguration);
            Grid.SetColumn(groupOverlayConfiguration, 0);
            Grid.SetRow(groupOverlayConfiguration, 0);


            var groupKeybindingsConfiguration = BuildGroup_KeybindConfiguration();
            grid.Children.Add(groupKeybindingsConfiguration);
            Grid.SetColumn(groupKeybindingsConfiguration, 0);
            Grid.SetRow(groupKeybindingsConfiguration, 1);

            var button_ResetToDefaults = new Button()
            {
                Content = BCPGlobalization.ButtonText_ResetToDefaults,  // "Reset to Defaults"
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 5, 5, 5),
                MinWidth = 100,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            grid.Children.Add(button_ResetToDefaults);
            Grid.SetColumn(button_ResetToDefaults, 0);
            Grid.SetRow(button_ResetToDefaults, 2);
            Panel.SetZIndex(button_ResetToDefaults, 1000);

            button_ResetToDefaults.Click += (sender, evt) =>
            {
                var configControlBar = Overlay_ControlBar.Configuration_GetDefaults();
                var configKeybinds = KeybindManager.Configuration_GetDefaults();
                var configStatusDisplay = Overlay_StatusDisplay.Configuration_GetDefaults();

                // Reset to default settings...
                Overlay_ControlBar.Instance.Configuration_Set(configControlBar);
                KeybindManager.Instance.Configuration_Set(configKeybinds);
                Overlay_StatusDisplay.Instance.Configuration_Set(configStatusDisplay);

                // Notify the preferences elements that something has changed...
                _checkBox_LockControlPositions.IsChecked = configControlBar.DragLocked;
                _comboBox_ControlBarSize.SelectedItem = configControlBar.ControlSize;
                _comboBox_StatusDisplayFontSize.SelectedItem = configStatusDisplay.FontSizeChoice;
                _uiControl_KeybindPauseResume.KeyConfigured = configKeybinds.KeyPauseResume;
                _uiControl_KeybindStopStart.KeyConfigured = configKeybinds.KeyStartStop;
            };

            return grid;
        }


        private Panel Build_DecisionButtons()
        {
            var stackPanel = new StackPanel()
            {
                Background = Assets.BrushTransparent,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 5, 5, 5),
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Bottom,
            };


            var button_OK = new Button()
            {
                Content = BCPGlobalization.ButtonText_OK,   // "OK"
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 0, 5, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 75,
            };
            stackPanel.Children.Add(button_OK);
            Panel.SetZIndex(button_OK, 1000);

            button_OK.Click += (sender, evt) =>
            {
                // Capture the current Configuration, and save it...
                BuddyControlPanelSettings.Instance.OverlayControlBarConfiguration = Overlay_ControlBar.Instance.Configuration_Get();
                BuddyControlPanelSettings.Instance.KeybindConfiguration = KeybindManager.Instance.Configuration_Get();
                BuddyControlPanelSettings.Instance.OverlayStatusDisplayConfiguration = Overlay_StatusDisplay.Instance.Configuration_Get();
                BuddyControlPanelSettings.Instance.Save();
                Close();
            };


            var button_Cancel = new Button()
            {
                Content = BCPGlobalization.ButtonText_Cancel,   // "Cancel"
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 0, 5, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 75,
            };
            stackPanel.Children.Add(button_Cancel);
            Panel.SetZIndex(button_Cancel, 1000);

            button_Cancel.Click += (sender, evt) =>
            {
                // Restore the original configuration...
                Overlay_ControlBar.Instance.Configuration_Set(_originalConfiguration_ControlBar);
                KeybindManager.Instance.Configuration_Set(_originalConfiguration_Keybinds);
                Overlay_StatusDisplay.Instance.Configuration_Set(_originalConfiguration_StatusDisplay);
                Close();
            };

            return stackPanel;
        }


        private GroupBox BuildGroup_OverlayConfiguration()
        {
            var groupBox = new Assets.ThemedGroupBox()
            {
                Background = Assets.BrushTransparent,
                Header = BCPGlobalization.Title_OverlayConfiguration,   // "Overlay Configuration"
                Margin = new Thickness(5, 5, 5, 5),
            };

            var grid = new Grid()
            {
                Background = Assets.BrushTransparent,
                ColumnDefinitions =
                {
                    new ColumnDefinition() {SharedSizeGroup = "LabelsColumn", Width = GridLength.Auto},
                    new ColumnDefinition() {Width = new GridLength(1.0, GridUnitType.Star)},
                },
                RowDefinitions =
                {
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = new GridLength(1.0, GridUnitType.Star)},
                },
            };
            groupBox.Content = grid;

            // Row 1
            var label_ControlBarSize = new Label()
            {
                Content = BCPGlobalization.Item_ControlBarSize_Label,       // "Control Bar Size:"
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 2, 10, 2),
                ToolTip = BCPGlobalization.Item_ControlBarSize_ToolTip,
                VerticalAlignment = VerticalAlignment.Top,
            };
            grid.Children.Add(label_ControlBarSize);
            Grid.SetColumn(label_ControlBarSize, 0);
            Grid.SetRow(label_ControlBarSize, 0);
            Panel.SetZIndex(label_ControlBarSize, 1000);

            _comboBox_ControlBarSize = new ComboBox()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 2, 5, 2),
                MinWidth = 150,
                VerticalAlignment = VerticalAlignment.Top,
            };
            grid.Children.Add(_comboBox_ControlBarSize);
            Grid.SetColumn(_comboBox_ControlBarSize, 1);
            Grid.SetRow(_comboBox_ControlBarSize, 0);
            Panel.SetZIndex(_comboBox_ControlBarSize, 1000);

            foreach (var controlBarSize in Utility.GetEnumValues<Overlay_ControlBar.ControlSizeEnum>())
                _comboBox_ControlBarSize.Items.Add(controlBarSize);

            _comboBox_ControlBarSize.SelectedItem = _originalConfiguration_ControlBar.ControlSize;

            _comboBox_ControlBarSize.SelectionChanged += (sender, evt) =>
            {
                var configuration = Overlay_ControlBar.Instance.Configuration_Get();

                // Don't fire events needlessly...
                if ((_comboBox_ControlBarSize.SelectedItem != null)
                    && ((Overlay_ControlBar.ControlSizeEnum)_comboBox_ControlBarSize.SelectedItem == configuration.ControlSize))
                {
                    return;
                }

                configuration.ControlSize = (Overlay_ControlBar.ControlSizeEnum)_comboBox_ControlBarSize.SelectedItem;
                Overlay_ControlBar.Instance.Configuration_Set(configuration);
            };

            // Row 2
            var label_StatusDisplayFontSize = new Label()
            {
                Content = BCPGlobalization.Item_StatusDisplayFontSize_Label,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 2, 10, 2),
                ToolTip = BCPGlobalization.Item_StatusDisplayFontSize_ToolTip,
                VerticalAlignment = VerticalAlignment.Center,
            };
            grid.Children.Add(label_StatusDisplayFontSize);
            Grid.SetColumn(label_StatusDisplayFontSize, 0);
            Grid.SetRow(label_StatusDisplayFontSize, 1);
            Panel.SetZIndex(label_StatusDisplayFontSize, 1000);

            _comboBox_StatusDisplayFontSize = new ComboBox()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 2, 5, 2),
                MinWidth = 150,
                VerticalAlignment = VerticalAlignment.Center,
            };
            grid.Children.Add(_comboBox_StatusDisplayFontSize);
            Grid.SetColumn(_comboBox_StatusDisplayFontSize, 1);
            Grid.SetRow(_comboBox_StatusDisplayFontSize, 1);
            Panel.SetZIndex(_comboBox_StatusDisplayFontSize, 1000);

            foreach (var fontSize in Utility.GetEnumValues<Overlay_StatusDisplay.FontSizeChoiceEnum>())
                _comboBox_StatusDisplayFontSize.Items.Add(fontSize);

            _comboBox_StatusDisplayFontSize.SelectedItem = _originalConfiguration_StatusDisplay.FontSizeChoice;

            _comboBox_StatusDisplayFontSize.SelectionChanged += (sender, evt) =>
            {
                var configuration = Overlay_StatusDisplay.Instance.Configuration_Get();

                // Don't fire events needlessly...
                if ((_comboBox_StatusDisplayFontSize.SelectedItem != null)
                    && ((Overlay_StatusDisplay.FontSizeChoiceEnum)_comboBox_StatusDisplayFontSize.SelectedItem == configuration.FontSizeChoice))
                {
                    return;
                }

                configuration.FontSizeChoice = (Overlay_StatusDisplay.FontSizeChoiceEnum)_comboBox_StatusDisplayFontSize.SelectedItem;
                Overlay_StatusDisplay.Instance.Configuration_Set(configuration);
            };

            // Row 3
            _checkBox_LockControlPositions = new CheckBox()
            {
                Content = BCPGlobalization.Item_LockControlPositions_Label,
                HorizontalAlignment = HorizontalAlignment.Left,
                IsThreeState = false,
                Margin = new Thickness(5, 2, 5, 2),
                ToolTip = BCPGlobalization.Item_LockControlPositions_ToolTip,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            grid.Children.Add(_checkBox_LockControlPositions);
            Grid.SetColumn(_checkBox_LockControlPositions, 0);
            Grid.SetColumnSpan(_checkBox_LockControlPositions, 2);
            Grid.SetRow(_checkBox_LockControlPositions, 2);
            Panel.SetZIndex(_checkBox_LockControlPositions, 1000);

            _checkBox_LockControlPositions.IsChecked = _originalConfiguration_ControlBar.DragLocked;
            _checkBox_LockControlPositions.Checked += (sender, evt) =>
            {
                HandleCheckboxChange_LockControlPositions();
            };
            _checkBox_LockControlPositions.Unchecked += (sender, evt) =>
            {
                HandleCheckboxChange_LockControlPositions();
            };

            return groupBox;
        }


        private GroupBox BuildGroup_KeybindConfiguration()
        {
            var hotkeyDisclaimerTooltip =
                string.Format(BCPGlobalization.Item_KeybindDisclaimer_ToolTipFormat,
                                Environment.NewLine);

            var groupBox = new Assets.ThemedGroupBox()
            {
                Background = Assets.BrushTransparent,
                Header = BCPGlobalization.Title_KeyBindingConfiguration,
                Margin = new Thickness(5, 5, 5, 5),
            };
            Panel.SetZIndex(groupBox, 1000);

            var grid = new Grid()
            {
                Background = Assets.BrushTransparent,
                ColumnDefinitions =
                {
                    new ColumnDefinition() {SharedSizeGroup = "LabelsColumn", Width = GridLength.Auto},
                    new ColumnDefinition() {Width = new GridLength(1.0, GridUnitType.Star)},
                },
                RowDefinitions =
                {
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = new GridLength(1.0, GridUnitType.Star)},
                },
            };
            groupBox.Content = grid;

            // Row 1
            var label_KeyPauseResume = new Label()
            {
                Content = BCPGlobalization.Item_KeybindPauseResume_Label,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 2, 10, 5),
                ToolTip = BCPGlobalization.Item_KeybindPauseResume_ToolTip + Environment.NewLine + hotkeyDisclaimerTooltip,
                VerticalAlignment = VerticalAlignment.Center,
            };
            grid.Children.Add(label_KeyPauseResume);
            Grid.SetColumn(label_KeyPauseResume, 0);
            Grid.SetRow(label_KeyPauseResume, 1);
            Panel.SetZIndex(label_KeyPauseResume, 1000);

            _uiControl_KeybindPauseResume = new UiControl_KeybindConfiguration()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 5, 10, 2),
                ToolTip = hotkeyDisclaimerTooltip,
                VerticalAlignment = VerticalAlignment.Center,
            };
            grid.Children.Add(_uiControl_KeybindPauseResume);
            Grid.SetColumn(_uiControl_KeybindPauseResume, 1);
            Grid.SetRow(_uiControl_KeybindPauseResume, 1);
            Panel.SetZIndex(_uiControl_KeybindPauseResume, 1000);

            _uiControl_KeybindPauseResume.KeyConfigured = _originalConfiguration_Keybinds.KeyPauseResume;

            _uiControl_KeybindPauseResume.OnKeybindChanged += (sender, evt) =>
            {
                var configurationKeybinds = KeybindManager.Instance.Configuration_Get();
                // Don't fire events needlessly...
                if (_uiControl_KeybindPauseResume.KeyConfigured.Equals(configurationKeybinds.KeyPauseResume))
                    return;

                configurationKeybinds.KeyPauseResume = _uiControl_KeybindPauseResume.KeyConfigured;
                KeybindManager.Instance.Configuration_Set(configurationKeybinds);
            };

            // Row 2
            var label_KeyStartStop = new Label()
            {
                Content = BCPGlobalization.Item_KeybindStartStop_Label,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 5, 10, 2),
                ToolTip = BCPGlobalization.Item_KeybindStartStop_ToolTip + Environment.NewLine + hotkeyDisclaimerTooltip,
                VerticalAlignment = VerticalAlignment.Top,
            };
            grid.Children.Add(label_KeyStartStop);
            Grid.SetColumn(label_KeyStartStop, 0);
            Grid.SetRow(label_KeyStartStop, 0);
            Panel.SetZIndex(label_KeyStartStop, 1000);

            _uiControl_KeybindStopStart = new UiControl_KeybindConfiguration()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 5, 10, 2),
                ToolTip = hotkeyDisclaimerTooltip,
                VerticalAlignment = VerticalAlignment.Center,
            };
            grid.Children.Add(_uiControl_KeybindStopStart);
            Grid.SetColumn(_uiControl_KeybindStopStart, 1);
            Grid.SetRow(_uiControl_KeybindStopStart, 0);
            Panel.SetZIndex(_uiControl_KeybindStopStart, 1000);

            _uiControl_KeybindStopStart.KeyConfigured = _originalConfiguration_Keybinds.KeyStartStop;

            _uiControl_KeybindStopStart.OnKeybindChanged += (sender, evt) =>
            {
                var configurationKeybinds = KeybindManager.Instance.Configuration_Get();
                // Don't fire events needlessly...
                if (_uiControl_KeybindStopStart.KeyConfigured.Equals(configurationKeybinds.KeyStartStop))
                    return;

                configurationKeybinds.KeyStartStop = _uiControl_KeybindStopStart.KeyConfigured;
                KeybindManager.Instance.Configuration_Set(configurationKeybinds);
            };

            return groupBox;
        }


        private void HandleCheckboxChange_LockControlPositions()
        {
            if (!_checkBox_LockControlPositions.IsChecked.HasValue)
                return;

            var configurationControlBar = Overlay_ControlBar.Instance.Configuration_Get();
            var configurationStatusDisplay = Overlay_StatusDisplay.Instance.Configuration_Get();
            var selection = _checkBox_LockControlPositions.IsChecked.Value;

            // Don't fire events needlessly...
            if (selection == configurationControlBar.DragLocked)
                return;

            configurationControlBar.DragLocked = selection;
            Overlay_ControlBar.Instance.Configuration_Set(configurationControlBar);

            configurationStatusDisplay.DragLocked = selection;
            Overlay_StatusDisplay.Instance.Configuration_Set(configurationStatusDisplay);
        }
    }
}
