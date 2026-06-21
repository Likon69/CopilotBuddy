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
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;

using BuddyControlPanel.Resources.Localization;

using Brushes = System.Windows.Media.Brushes;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using Label = System.Windows.Controls.Label;
using Orientation = System.Windows.Controls.Orientation;

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    public class UiControl_KeybindConfiguration : Assets.ThemedButton
    {
        public UiControl_KeybindConfiguration()
        {
            MinWidth = 150;
            KeyConfigured = new BuddyControlPanelSettings.WpfKeyWithModifiers() { Key = Key.None };
        }

        private BuddyControlPanelSettings.WpfKeyWithModifiers _keyConfigured;
        public BuddyControlPanelSettings.WpfKeyWithModifiers KeyConfigured
        {
            get { return _keyConfigured; }
            set
            {
                _keyConfigured = value;
                Content = _keyConfigured;
            }
        }

        protected override void OnClick()
        {
            var flyout = new KeybindConfigurationFlyout(this, KeyConfigured) { IsOpen = true };
            flyout.OnKeybindChanged += (sender, evt) =>
            {
                var keyProposed = evt.KeyWithModifiers;

                if (!keyProposed.IsViableKeybind())
                {
                    var mousePosition = Utility.GetStartLocation(this);

                    var invalidKeybindDialog = new Dialog_General(SystemIcons.Error,
                        BCPGlobalization.Title_InvalidKeybind,
                        string.Format(BCPGlobalization.Dialog_FormatText_IsNotAValidKeybind,
                            Environment.NewLine,
                            keyProposed.ToString()),
                        MessageBoxButton.OK)
                    {
                        WindowStartupLocation = WindowStartupLocation.Manual,
                        Left = mousePosition.X,
                        Top = mousePosition.Y
                    };

                    invalidKeybindDialog.ShowDialog();
                    return;
                }
                KeyConfigured = keyProposed;
                FireKeybindChangedEventHandler();
            };
            base.OnClick();
        }


        #region Events
        public class KeybindEventArgs : EventArgs
        {
            public BuddyControlPanelSettings.WpfKeyWithModifiers KeyWithModifiers { get; set; }
        }

        public delegate void KeybindChangedEventHandler(object sender, KeybindEventArgs e);
        public event KeybindChangedEventHandler OnKeybindChanged;

        private void FireKeybindChangedEventHandler()
        {
            if (OnKeybindChanged != null)
                OnKeybindChanged(this, new KeybindEventArgs() { KeyWithModifiers = _keyConfigured, });
        }
        #endregion
    }


    internal class KeybindConfigurationFlyout : Popup
    {
        #region Construction & Destruction
        public KeybindConfigurationFlyout(FrameworkElement parent, BuddyControlPanelSettings.WpfKeyWithModifiers keyConfigured)
        {
            var foregroundBrush = Brushes.White;
            _keyOriginal = keyConfigured;

            Placement = PlacementMode.Bottom;
            PlacementTarget = parent;
            StaysOpen = false;

            var border = new Border()
            {
                Background = Brushes.DarkGray,
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(2),
                MinHeight = parent.ActualHeight,
                MinWidth = parent.ActualWidth,
            };
            Child = border;

            var grid = new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() {Width = GridLength.Auto},
                    new ColumnDefinition() {Width = new GridLength(1.0, GridUnitType.Star)},
                },
                Margin = new Thickness(2, 2, 2, 2),
                RowDefinitions =
                {
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = new GridLength(1.0, GridUnitType.Star)},
                }
            };
            border.Child = grid;

            // Row 0
            var labelModifiers = new Label()
            {
                Content = BCPGlobalization.Item_KeybindModifiers_Label, // "Modifiers:"
                Margin = new Thickness(5, 5, 5, 5),
                Foreground = foregroundBrush,
            };
            grid.Children.Add(labelModifiers);
            Grid.SetColumn(labelModifiers, 0);
            Grid.SetRow(labelModifiers, 0);

            // Row 1
            var stackPanel_Modifiers = new StackPanel()
            {
                Margin = new Thickness(10, 0, 10, 2),
                Orientation = Orientation.Horizontal,
            };
            grid.Children.Add(stackPanel_Modifiers);
            Grid.SetColumn(stackPanel_Modifiers, 0);
            Grid.SetRow(stackPanel_Modifiers, 1);

            _checkBox_ModifierControl = new CheckBox()
            {
                Content = "Ctrl",
                Foreground = foregroundBrush,
                Margin = new Thickness(4, 0, 4, 0),
            };
            stackPanel_Modifiers.Children.Add(_checkBox_ModifierControl);
            _checkBox_ModifierControl.Click += (sender, evt) =>
            {
                if (!_checkBox_ModifierControl.IsChecked.HasValue)
                    return;

                _keyConfigured.ModifierControl = _checkBox_ModifierControl.IsChecked.Value;
                FireKeybindChangedEventHandler();
            };

            _checkBox_ModifierShift = new CheckBox()
            {
                Content = "Shift",
                Foreground = foregroundBrush,
                Margin = new Thickness(4, 0, 4, 0),
            };
            stackPanel_Modifiers.Children.Add(_checkBox_ModifierShift);
            _checkBox_ModifierShift.Click += (sender, evt) =>
            {
                if (!_checkBox_ModifierShift.IsChecked.HasValue)
                    return;

                _keyConfigured.ModifierShift = _checkBox_ModifierShift.IsChecked.Value;
                FireKeybindChangedEventHandler();
            };

            _checkBox_ModifierAlt = new CheckBox()
            {
                Content = "Alt",
                Foreground = foregroundBrush,
                Margin = new Thickness(4, 0, 4, 0),
            };
            stackPanel_Modifiers.Children.Add(_checkBox_ModifierAlt);
            _checkBox_ModifierAlt.Click += (sender, evt) =>
            {
                if (!_checkBox_ModifierAlt.IsChecked.HasValue)
                    return;

                _keyConfigured.ModifierAlt = _checkBox_ModifierAlt.IsChecked.Value;
                FireKeybindChangedEventHandler();
            };


            // Row 2
            var labelKey = new Label()
            {
                Content = BCPGlobalization.Item_KeybindKey_Label,       // "Key:"
                Margin = new Thickness(5, 5, 5, 5),
                Foreground = foregroundBrush,
            };
            grid.Children.Add(labelKey);
            Grid.SetColumn(labelKey, 0);
            Grid.SetRow(labelKey, 2);

            // Row 3
            var stackPanel_Key = new StackPanel()
            {
                Margin = new Thickness(10, 0, 10, 2),
                Orientation = Orientation.Horizontal,
            };
            grid.Children.Add(stackPanel_Key);
            Grid.SetColumn(stackPanel_Key, 0);
            Grid.SetRow(stackPanel_Key, 3);

            _comboBox_KeyChoices = new ComboBox()
            {
                Margin = new Thickness(10, 0, 10, 5),
                MinHeight = parent.ActualHeight,
                Width = 100,
            };
            stackPanel_Key.Children.Add(_comboBox_KeyChoices);
            _comboBox_KeyChoices.SelectionChanged += (sender, evt) =>
            {
                _keyConfigured.Key = (Key)_comboBox_KeyChoices.SelectedItem;
                FireKeybindChangedEventHandler();
            };
            PopulateKeyChoicesComboBox(_comboBox_KeyChoices);


            var buttonReset = new Button()
            {
                Content = BCPGlobalization.ButtonText_Reset,    // "Reset"
                Margin = new Thickness(10, 0, 10, 5),
            };
            stackPanel_Key.Children.Add(buttonReset);
            buttonReset.Click += (sender, evt) =>
            {
                SetConfigurationState(_keyOriginal);
            };

            SetConfigurationState(_keyOriginal);
        }
        #endregion

        private readonly ComboBox _comboBox_KeyChoices;
        private readonly CheckBox _checkBox_ModifierAlt;
        private readonly CheckBox _checkBox_ModifierControl;
        private readonly CheckBox _checkBox_ModifierShift;
        private BuddyControlPanelSettings.WpfKeyWithModifiers _keyConfigured;
        private readonly BuddyControlPanelSettings.WpfKeyWithModifiers _keyOriginal;


        #region Events
        public event UiControl_KeybindConfiguration.KeybindChangedEventHandler OnKeybindChanged;

        private void FireKeybindChangedEventHandler()
        {
            if (OnKeybindChanged != null)
                OnKeybindChanged(this, new UiControl_KeybindConfiguration.KeybindEventArgs() { KeyWithModifiers = _keyConfigured, });
        }
        #endregion


        private void PopulateKeyChoicesComboBox(ComboBox comboBox)
        {
            // Since Honorbuddy HotkeysManager is limited to the old Windows.Forms.Keys,
            // we must limit ourselves to that set of equivalent Windows.Input.Key.
            foreach (var formsKey in Utility.GetEnumValues<Keys>().OrderBy(k => k.ToString()))
            {
                var wpfKey = KeyInterop.KeyFromVirtualKey((int)formsKey);

                // If there is no Forms-to-WPF mapping for key, skip it...
                if ((formsKey != Keys.None) && (wpfKey == Key.None))
                    continue;

                comboBox.Items.Add(wpfKey);
            }
        }


        private void SetConfigurationState(BuddyControlPanelSettings.WpfKeyWithModifiers keyWithModifiers)
        {
            _keyConfigured = keyWithModifiers;
            _checkBox_ModifierAlt.IsChecked = _keyConfigured.ModifierAlt;
            _checkBox_ModifierControl.IsChecked = _keyConfigured.ModifierControl;
            _checkBox_ModifierShift.IsChecked = _keyConfigured.ModifierShift;
            _comboBox_KeyChoices.SelectedItem = _keyConfigured.Key;
        }
    }
}
