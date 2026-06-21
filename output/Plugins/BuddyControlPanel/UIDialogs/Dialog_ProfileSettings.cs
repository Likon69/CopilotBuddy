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
using System.Windows.Media.Effects;

using BuddyControlPanel.Resources.Localization;
using Styx;
using Styx.CommonBot;
using Styx.Logic.Profiles;
using Styx.Helpers;

// ReSharper disable CheckNamespace
// ReSharper disable RedundantLambdaSignatureParentheses
// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    internal class Dialog_ProfileSettings : Assets.ThemedWindow, IDisposable
    {
        #region Creation and Destruction
        private Dialog_ProfileSettings()
        {
            MaxHeight = 500;
            MaxWidth = 700;
            ResizeMode = ResizeMode.NoResize;
            SizeToContent = SizeToContent.WidthAndHeight;
            Title = BCPGlobalization.Title_ProfileConfigurationSettings;

            _configurationCurrent = new ProfileOverrides();

            BuildDialog();

            Closing += (sender, evt) =>
            {
                Visibility = Visibility.Hidden;
                evt.Cancel = true;
            };

            BotEvents.Profile.OnNewOuterProfileLoaded += OnNewProfileLoaded;

            ReevaluateState();
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
            BotEvents.Profile.OnNewOuterProfileLoaded -= OnNewProfileLoaded;

            s_instance = null;
        }

        private static Dialog_ProfileSettings s_instance;
        public static Dialog_ProfileSettings Instance
        {
            get
            {
                return s_instance ?? (s_instance = new Dialog_ProfileSettings());
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


        #region Private data...
        private readonly ProfileOverrides _configurationCurrent;
        private TextBlock _textBlock_ProfileName;
        #endregion


        #region Events...
        private void OnNewProfileLoaded(BotEvents.Profile.NewProfileLoadedEventArgs evtArgs)
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
            {
                if ((ProfileManager.CurrentOuterProfile != null)
                    && !string.IsNullOrEmpty(ProfileManager.CurrentOuterProfile.Name))
                {
                    _textBlock_ProfileName.Text = ProfileManager.CurrentOuterProfile.Name.Trim();
                }

                _configurationCurrent.FireProfileOverridesChanged();
            });
        }

        public void ReevaluateState()
        {
            _configurationCurrent.CaptureFromCharacterSettings();
            _configurationCurrent.FireProfileOverridesChanged();
        }
        #endregion


        private void BuildDialog()
        {
            var grid = new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() {Width = new GridLength(1.0, GridUnitType.Star)},
                },
                RowDefinitions =
                {
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = new GridLength(1.0, GridUnitType.Star)},
                    new RowDefinition() {Height = GridLength.Auto},
                },
            };
            Content = grid;

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
            grid.Children.Add(buddyBotLogo);
            Grid.SetColumn(buddyBotLogo, 0);
            Grid.SetColumnSpan(buddyBotLogo, 1);
            Grid.SetRow(buddyBotLogo, 0);
            Grid.SetRowSpan(buddyBotLogo, 3);
            Panel.SetZIndex(buddyBotLogo, 500);

            // Row 1
            var stackPanel = new StackPanel()
            {
                Background = Assets.BrushTransparent,
                Margin = new Thickness(5, 5, 5, 5),
                Orientation = Orientation.Horizontal,
            };
            grid.Children.Add(stackPanel);
            Grid.SetColumn(stackPanel, 0);
            Grid.SetRow(stackPanel, 0);
            Panel.SetZIndex(stackPanel, 100);


            var toolTip_ProfileNameLocation =
                string.Format(BCPGlobalization.Item_ProfileLocation_ToolTipFormat, Utility.GetCurrentProfileName());

            var textBlock_ProfileNameLabel = new Assets.ThemedTextBlock()
            {
                Background = Assets.BrushTransparent,
                Effect = new DropShadowEffect(),
                FontWeight = FontWeights.Bold,
                Foreground = Assets.BrushText,
                Margin = new Thickness(0, 0, 5, 0),
                Text = string.Format(BCPGlobalization.Item_ProfileLocation_LabelFormat),
                ToolTip = toolTip_ProfileNameLocation,
            };
            stackPanel.Children.Add(textBlock_ProfileNameLabel);


            _textBlock_ProfileName = new Assets.ThemedTextBlock()
            {
                Background = Assets.BrushTransparent,
                Effect = new DropShadowEffect(),
                Foreground = Assets.BrushText,
                Margin = new Thickness(0, 0, 5, 0),
                TextWrapping = TextWrapping.Wrap,
                Text = Utility.GetCurrentProfileName(),
                ToolTip = toolTip_ProfileNameLocation,
            };
            stackPanel.Children.Add(_textBlock_ProfileName);


            // Row 2
            var borderForConfigurables = new Border()
            {
                Background = Assets.BrushTransparent,
                BorderBrush = Assets.BrushOpaqueBlack,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5, 5, 5, 5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            grid.Children.Add(borderForConfigurables);
            Grid.SetColumn(borderForConfigurables, 0);
            Grid.SetRow(borderForConfigurables, 1);
            Panel.SetZIndex(borderForConfigurables, 100);

            borderForConfigurables.Child = Build_Configurables();

            // Row 3
            var decisionButtons = Build_DecisionButtons();
            grid.Children.Add(decisionButtons);
            Grid.SetColumn(decisionButtons, 0);
            Grid.SetRow(decisionButtons, 2);
        }


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

            // Row 1
            var checkboxOverrideProfileSettings = new CheckBox_OverrideEnable(
                                BCPGlobalization.Button_OverrideProfileSettings_Label,
                                BCPGlobalization.Button_OverrideProfileSettings_ToolTip,
                                _configurationCurrent,
                                new LambdaProperty<ProfileOverrides, bool>(
                                    BCPGlobalization.Button_OverrideProfileSettings_Label,      // "Override Profile Settings"
                                    (profileOverrides) => profileOverrides.OverrideProfileSettings,
                                    (profileOverrides, value) => { profileOverrides.OverrideProfileSettings = value; }))
            {
                Margin = new Thickness(5, 5, 5, 5),
            };

            grid.Children.Add(checkboxOverrideProfileSettings);
            Grid.SetColumn(checkboxOverrideProfileSettings, 0);
            Grid.SetRow(checkboxOverrideProfileSettings, 0);
            Panel.SetZIndex(checkboxOverrideProfileSettings, 1000);

            // Row 2
            var groupOverlayConfiguration = BuildGroup_InventoryManagement();
            grid.Children.Add(groupOverlayConfiguration);
            Grid.SetColumn(groupOverlayConfiguration, 0);
            Grid.SetRow(groupOverlayConfiguration, 1);

            // Row 3
            var groupKeybindingsConfiguration = BuildGroup_QuestingBot();
            grid.Children.Add(groupKeybindingsConfiguration);
            Grid.SetColumn(groupKeybindingsConfiguration, 0);
            Grid.SetRow(groupKeybindingsConfiguration, 2);

            return grid;
        }


        private Panel Build_DecisionButtons()
        {
            var grid = new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() {Width = new GridLength(1.0, GridUnitType.Star)},
                    new ColumnDefinition() {Width = GridLength.Auto},
                    new ColumnDefinition() {Width = GridLength.Auto},
                },
                RowDefinitions =
                {
                    new RowDefinition() {Height = GridLength.Auto},
                },
            };

            var textBlock_HowItWorks_ProfileSettings = new TextBlock()
            {
                Background = Assets.BrushTransparent,
                Foreground = Assets.BrushText,
                FontStyle = FontStyles.Oblique,
                Margin = new Thickness(10, 10, 10, 5),
                Text = string.Format(BCPGlobalization.GeneralTextFormat_HowItWorks_ProfileSettings,
                                     Environment.NewLine),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            grid.Children.Add(textBlock_HowItWorks_ProfileSettings);
            Grid.SetColumn(textBlock_HowItWorks_ProfileSettings, 0);
            Grid.SetRow(textBlock_HowItWorks_ProfileSettings, 0);
            Panel.SetZIndex(textBlock_HowItWorks_ProfileSettings, 1000);

            var button_OK = new Button()
            {
                Content = BCPGlobalization.ButtonText_OK,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 5, 5, 5),
                VerticalAlignment = VerticalAlignment.Bottom,
                Width = 75,
            };
            grid.Children.Add(button_OK);
            Panel.SetZIndex(button_OK, 1000);

            Grid.SetColumn(button_OK, 1);
            Grid.SetRow(button_OK, 0);
            Panel.SetZIndex(button_OK, 100);

            button_OK.Click += (sender, evt) =>
            {
                _configurationCurrent.SaveToCharacterSettings();
                Close();
            };


            var button_Cancel = new Button()
            {
                Content = BCPGlobalization.ButtonText_Cancel,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 5, 5, 5),
                VerticalAlignment = VerticalAlignment.Bottom,
                Width = 75,
            };
            grid.Children.Add(button_Cancel);
            Grid.SetColumn(button_Cancel, 2);
            Grid.SetRow(button_Cancel, 0);
            Panel.SetZIndex(button_Cancel, 1000);

            button_Cancel.Click += (sender, evt) =>
            {
                Close();
            };

            return grid;
        }


        private GroupBox BuildGroup_InventoryManagement()
        {
            var groupBox = new Assets.ThemedGroupBox()
            {
                Background = Assets.BrushTransparent,
                Header = BCPGlobalization.Title_InventoryManagement,
                Margin = new Thickness(5, 5, 5, 5),
            };

            var grid = new Grid()
            {
                Background = Assets.BrushTransparent,
                ColumnDefinitions =
                {
                    new ColumnDefinition() {SharedSizeGroup = "LabelsColumn", Width = GridLength.Auto},
                    new ColumnDefinition() {Width = GridLength.Auto},
                    new ColumnDefinition() {Width = new GridLength(1.0, GridUnitType.Star)},
                },
                RowDefinitions =
                {
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = GridLength.Auto},
                },
            };
            groupBox.Content = grid;

            Build_InventoryQualityDisposition(_configurationCurrent, grid, 0,
                BCPGlobalization.Item_ItemQuality_Poor,         // "Poor"
                BCPGlobalization.Item_ItemQuality_PoorColor,    // "Grey"
                new LambdaProperty<ProfileOverrides, bool>(
                    "MailGrey",
                    (profileOverrides) => profileOverrides.MailGrey,
                    (profileOverrides, value) => { profileOverrides.MailGrey = value; }),
                () => (ProfileManager.CurrentOuterProfile == null) ? (bool?)null : ProfileManager.CurrentOuterProfile.MailGrey,
                new LambdaProperty<ProfileOverrides, bool>(
                    "SellGrey",
                    (profileOverrides) => profileOverrides.SellGrey,
                    (profileOverrides, value) => { profileOverrides.SellGrey = value; }),
                () => (ProfileManager.CurrentOuterProfile == null) ? (bool?)null : ProfileManager.CurrentOuterProfile.SellGrey);

            Build_InventoryQualityDisposition(_configurationCurrent, grid, 1,
                BCPGlobalization.Item_ItemQuality_Common,       // "Common"
                BCPGlobalization.Item_ItemQuality_CommonColor,  // "White"
                new LambdaProperty<ProfileOverrides, bool>(
                    "MailWhite",
                    (profileOverrides) => profileOverrides.MailWhite,
                    (profileOverrides, value) => { profileOverrides.MailWhite = value; }),
                () => (ProfileManager.CurrentOuterProfile == null) ? (bool?)null : ProfileManager.CurrentOuterProfile.MailWhite,
                new LambdaProperty<ProfileOverrides, bool>(
                    "SellWhite",
                    (profileOverrides) => profileOverrides.SellWhite,
                    (profileOverrides, value) => { profileOverrides.SellWhite = value; }),
                () => (ProfileManager.CurrentOuterProfile == null) ? (bool?)null : ProfileManager.CurrentOuterProfile.SellWhite);

            Build_InventoryQualityDisposition(_configurationCurrent, grid, 2,
                BCPGlobalization.Item_ItemQuality_Uncommon,         // "Uncommon"
                BCPGlobalization.Item_ItemQuality_UncommonColor,    // "Green"
                new LambdaProperty<ProfileOverrides, bool>(
                    "MailGreen",
                    (profileOverrides) => profileOverrides.MailGreen,
                    (profileOverrides, value) => { profileOverrides.MailGreen = value; }),
                () => (ProfileManager.CurrentOuterProfile == null) ? (bool?)null : ProfileManager.CurrentOuterProfile.MailGreen,
                new LambdaProperty<ProfileOverrides, bool>(
                    "SellGreen",
                    (profileOverrides) => profileOverrides.SellGreen,
                    (profileOverrides, value) => { profileOverrides.SellGreen = value; }),
                () => (ProfileManager.CurrentOuterProfile == null) ? (bool?)null : ProfileManager.CurrentOuterProfile.SellGreen);

            Build_InventoryQualityDisposition(_configurationCurrent, grid, 3,
                BCPGlobalization.Item_ItemQuality_Rare,         // "Rare"
                BCPGlobalization.Item_ItemQuality_RareColor,    // "Blue"
                new LambdaProperty<ProfileOverrides, bool>(
                    "MailBlue",
                    (profileOverrides) => profileOverrides.MailBlue,
                    (profileOverrides, value) => { profileOverrides.MailBlue = value; }),
                () => (ProfileManager.CurrentOuterProfile == null) ? (bool?)null : ProfileManager.CurrentOuterProfile.MailBlue,
                new LambdaProperty<ProfileOverrides, bool>(
                    "SellBlue",
                    (profileOverrides) => profileOverrides.SellBlue,
                    (profileOverrides, value) => { profileOverrides.SellBlue = value; }),
                () => (ProfileManager.CurrentOuterProfile == null) ? (bool?)null : ProfileManager.CurrentOuterProfile.SellBlue);

            Build_InventoryQualityDisposition(_configurationCurrent, grid, 4,
                BCPGlobalization.Item_ItemQuality_Epic,         // "Epic"
                BCPGlobalization.Item_ItemQuality_EpicColor,    // "Purple"
                new LambdaProperty<ProfileOverrides, bool>(
                    "MailPurple",
                    (profileOverrides) => profileOverrides.MailPurple,
                    (profileOverrides, value) => { profileOverrides.MailPurple = value; }),
                () => (ProfileManager.CurrentOuterProfile == null) ? (bool?)null : ProfileManager.CurrentOuterProfile.MailPurple,
                new LambdaProperty<ProfileOverrides, bool>(
                    "SellPurple",
                    (profileOverrides) => profileOverrides.SellPurple,
                    (profileOverrides, value) => { profileOverrides.SellPurple = value; }),
                () => (ProfileManager.CurrentOuterProfile == null) ? (bool?)null : ProfileManager.CurrentOuterProfile.SellPurple);

            var textBlock_HowItWorks_MailSell = new TextBlock()
            {
                Background = Assets.BrushTransparent,
                FontStyle = FontStyles.Oblique,
                Margin = new Thickness(10, 10, 10, 5),
                Text = BCPGlobalization.GeneralText_HowItWorks_MailSell,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            grid.Children.Add(textBlock_HowItWorks_MailSell);
            Grid.SetColumn(textBlock_HowItWorks_MailSell, 0);
            Grid.SetColumnSpan(textBlock_HowItWorks_MailSell, 3);
            Grid.SetRow(textBlock_HowItWorks_MailSell, 5);
            Panel.SetZIndex(textBlock_HowItWorks_MailSell, 1000);

            return groupBox;
        }


        private void Build_InventoryQualityDisposition(ProfileOverrides currentConfiguration,
            Grid grid,
            int row,
            string itemQuality,
            string itemQualityColorName,
            LambdaProperty<ProfileOverrides, bool> mailProperty,
            Func<bool?> mailOriginalValue,
            LambdaProperty<ProfileOverrides, bool> sellProperty,
            Func<bool?> sellOriginalValue)
        {
            Contract.Requires(!string.IsNullOrEmpty(itemQuality), () => "itemQuality may not be null or empty.");
            Contract.Requires(!string.IsNullOrEmpty(itemQualityColorName), () => "itemQualityColorName may not be null or empty.");
            Contract.Requires(mailProperty != null, () => "mailProperty may not be null.");
            Contract.Requires(sellProperty != null, () => "sellProperty may not be null.");

            var label_Disposition = new Label()
            {
                Content = itemQuality + ":",
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 2, 10, 2),
                ToolTip = string.Format(BCPGlobalization.GeneralTextFormat_ItemDisposition_ToolTip, itemQualityColorName),
                VerticalAlignment = VerticalAlignment.Center,
            };
            grid.Children.Add(label_Disposition);
            Grid.SetColumn(label_Disposition, 0);
            Grid.SetRow(label_Disposition, row);
            Panel.SetZIndex(label_Disposition, 1000);

            var mailDisposition = new CheckBox_Configurable(
                string.Format(BCPGlobalization.GeneralTextFormat_MailItem_Label, itemQualityColorName),
                string.Format(BCPGlobalization.GeneralTextFormat_MailItem_ToolTip, Environment.NewLine, itemQuality),
                currentConfiguration,
                mailProperty,
                mailOriginalValue)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 2, 5, 2),
                MinWidth = 100,
                VerticalAlignment = VerticalAlignment.Center,
            };
            grid.Children.Add(mailDisposition);
            Grid.SetColumn(mailDisposition, 1);
            Grid.SetRow(mailDisposition, row);
            Panel.SetZIndex(mailDisposition, 1000);

            var sellDisposition = new CheckBox_Configurable(
                string.Format(BCPGlobalization.GeneralTextFormat_SellItem_Label, itemQualityColorName),
                string.Format(BCPGlobalization.GeneralTextFormat_SellItem_ToolTip, Environment.NewLine, itemQuality),
                currentConfiguration,
                sellProperty,
                sellOriginalValue)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 2, 5, 2),
                VerticalAlignment = VerticalAlignment.Center,
            };
            grid.Children.Add(sellDisposition);
            Grid.SetColumn(sellDisposition, 2);
            Grid.SetRow(sellDisposition, row);
            Panel.SetZIndex(sellDisposition, 1000);
        }


        private GroupBox BuildGroup_QuestingBot()
        {
            var groupBox = new Assets.ThemedGroupBox()
            {
                Background = Assets.BrushTransparent,
                Header = BCPGlobalization.Title_QuestSpecificConfiguration,
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
                },
            };
            groupBox.Content = grid;

            var checkbox_IgnoreCheckpoints = new CheckBox_Configurable(
                BCPGlobalization.Item_IgnoreCheckpoints_Label,  // "Ignore Checkpoints"
                null,
                _configurationCurrent,
                new LambdaProperty<ProfileOverrides, bool>(
                    "IgnoreCheckpoints",
                        (profileOverrides) => profileOverrides.IgnoreCheckpoints,
                        (profileOverrides, value) => profileOverrides.IgnoreCheckpoints = value),
                () => Utility.IsQuestProfile(ProfileManager.CurrentOuterProfile)
                        && ProfileManager.CurrentOuterProfile.QuestOrder.IgnoreCheckpoints)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                IsChecked = (ProfileManager.CurrentOuterProfile != null)
                    && (ProfileManager.CurrentOuterProfile.QuestOrder != null)
                    && (ProfileManager.CurrentOuterProfile.QuestOrder.Count > 0),
                Margin = new Thickness(5, 2, 5, 2),
                VerticalAlignment = VerticalAlignment.Center,
            };
            grid.Children.Add(checkbox_IgnoreCheckpoints);
            Grid.SetColumn(checkbox_IgnoreCheckpoints, 0);
            Grid.SetColumnSpan(checkbox_IgnoreCheckpoints, 2);
            Grid.SetRow(checkbox_IgnoreCheckpoints, 0);
            Panel.SetZIndex(checkbox_IgnoreCheckpoints, 1000);
            ToolTipService.SetShowOnDisabled(checkbox_IgnoreCheckpoints, true);

            var textBlock_HowItWorks_IgnoreCheckpoints = new TextBlock()
            {
                Background = Assets.BrushTransparent,
                FontStyle = FontStyles.Oblique,
                Margin = new Thickness(10, 10, 10, 5),
                Text = string.Format(BCPGlobalization.GeneralTextFormat_HowItWorks_IgnoreCheckpoints,
                                    Environment.NewLine,
                                    Utility.BotName_QuestBot),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            grid.Children.Add(textBlock_HowItWorks_IgnoreCheckpoints);
            Grid.SetColumn(textBlock_HowItWorks_IgnoreCheckpoints, 0);
            Grid.SetColumnSpan(textBlock_HowItWorks_IgnoreCheckpoints, 2);
            Grid.SetRow(textBlock_HowItWorks_IgnoreCheckpoints, 1);
            Panel.SetZIndex(textBlock_HowItWorks_IgnoreCheckpoints, 1000);
            ToolTipService.SetShowOnDisabled(textBlock_HowItWorks_IgnoreCheckpoints, true);

            return groupBox;
        }


        private class CheckBox_Configurable : Assets.ThemedCheckBox
        {
            public CheckBox_Configurable(string checkBoxLabel,
                string defaultToolTip,
                ProfileOverrides profileOverrides,
                LambdaProperty<ProfileOverrides, bool> lambdaCurrentValue,
                Func<bool?> originalProfileValue)
            {
                _defaultToolTip = defaultToolTip;
                _profileOverrides = profileOverrides;
                _originalProfileValue = originalProfileValue;
                _lambdaCurrentValue = lambdaCurrentValue;

                Content = checkBoxLabel;
                IsThreeState = false;
                MinWidth = 100;
                ToolTip = _defaultToolTip;
                Panel.SetZIndex(this, 1000);
                ToolTipService.SetShowOnDisabled(this, true);

                _profileOverrides.OnProfileOverridesChanged += NotifyValueChanged;
                NotifyValueChanged();
            }

            private readonly string _defaultToolTip;
            private readonly ProfileOverrides _profileOverrides;
            private readonly LambdaProperty<ProfileOverrides, bool> _lambdaCurrentValue;
            private readonly Func<bool?> _originalProfileValue;


            protected override void OnChecked(RoutedEventArgs evt)
            {
                _lambdaCurrentValue.Setter(_profileOverrides, true);
                base.OnChecked(evt);
            }

            protected override void OnUnchecked(RoutedEventArgs evt)
            {
                _lambdaCurrentValue.Setter(_profileOverrides, false);
                base.OnUnchecked(evt);
            }

            private void NotifyValueChanged()
            {
                IsEnabled = _profileOverrides.OverrideProfileSettings;

                // If enabled, make checkbox setting reflect selected value...
                if (IsEnabled)
                {
                    ToolTip = _defaultToolTip;

                    var characterSettingsValue = _lambdaCurrentValue.Getter(_profileOverrides);
                    // Don't fire events needlessly...
                    if (IsChecked != characterSettingsValue)
                        IsChecked = characterSettingsValue;
                    return;
                }

                ToolTip = string.Format(BCPGlobalization.GeneralTextFormat_DefaultProfileSettingInUse,
                                        Environment.NewLine);

                // Setting is disabled, show the value Honorbuddy will use...
                bool? valueToShow = null;

                // If value from profile is available, show it...
                if (_originalProfileValue != null)
                    valueToShow = _originalProfileValue();

                // Otherwise, no profile loaded, so show value from CharacterSettings profile overrides...
                if (!valueToShow.HasValue)
                    valueToShow = _lambdaCurrentValue.Getter(_profileOverrides);

                // Don't fire events needlessly...
                if (IsChecked != valueToShow)
                    IsChecked = valueToShow;
            }
        }


        private class CheckBox_OverrideEnable : Assets.ThemedCheckBox
        {
            public CheckBox_OverrideEnable(string checkBoxLabel,
                string defaultToolTip,
                ProfileOverrides profileOverrides,
                LambdaProperty<ProfileOverrides, bool> lambdaCurrentValue)
            {
                _defaultToolTip = defaultToolTip;
                _profileOverrides = profileOverrides;
                _lambdaCurrentValue = lambdaCurrentValue;

                Content = checkBoxLabel;
                IsEnabled = true;
                IsThreeState = false;
                MinWidth = 100;
                Panel.SetZIndex(this, 1000);
                ToolTip = _defaultToolTip;
                ToolTipService.SetShowOnDisabled(this, true);

                _profileOverrides.OnProfileOverridesChanged += NotifyValueChanged;
                NotifyValueChanged();
            }

            private readonly string _defaultToolTip;
            private readonly ProfileOverrides _profileOverrides;
            private readonly LambdaProperty<ProfileOverrides, bool> _lambdaCurrentValue;

            protected override void OnChecked(RoutedEventArgs evt)
            {
                _profileOverrides.CaptureFromCharacterSettings();
                _lambdaCurrentValue.Setter(_profileOverrides, true);
                _profileOverrides.FireProfileOverridesChanged();
                base.OnChecked(evt);
            }

            protected override void OnUnchecked(RoutedEventArgs evt)
            {
                _lambdaCurrentValue.Setter(_profileOverrides, false);
                _profileOverrides.FireProfileOverridesChanged();
                base.OnUnchecked(evt);
            }

            private void NotifyValueChanged()
            {
                var currentValue = _profileOverrides.OverrideProfileSettings;
                // Don't fire events needlessly...
                if (IsChecked != currentValue)
                    IsChecked = currentValue;
            }
        }


        private class ProfileOverrides
        {
            public delegate void ProfileOverridesChangedDelegate();
            public event ProfileOverridesChangedDelegate OnProfileOverridesChanged;

            public void CaptureFromCharacterSettings()
            {
                var instance = CharacterSettings.Instance;

                OverrideProfileSettings = instance.OverrideProfileSettings;
                IgnoreCheckpoints = instance.IgnoreCheckpoints;

                MailGrey = instance.MailGrey;
                MailWhite = instance.MailWhite;
                MailGreen = instance.MailGreen;
                MailBlue = instance.MailBlue;
                MailPurple = instance.MailPurple;

                SellGrey = instance.SellGrey;
                SellWhite = instance.SellWhite;
                SellGreen = instance.SellGreen;
                SellBlue = instance.SellBlue;
                SellPurple = instance.SellPurple;
            }


            internal void FireProfileOverridesChanged()
            {
                if (OnProfileOverridesChanged == null)
                    return;

                foreach (var d in OnProfileOverridesChanged.GetInvocationList())
                {
                    try
                    {
                        d.DynamicInvoke();
                    }
                    catch (Exception ex)
                    {
                        PluginLog.Exception(ex);
                    }
                }
            }


            public void SaveToCharacterSettings()
            {
                var instance = CharacterSettings.Instance;

                instance.OverrideProfileSettings = OverrideProfileSettings;
                instance.IgnoreCheckpoints = IgnoreCheckpoints;

                instance.MailGrey = MailGrey;
                instance.MailWhite = MailWhite;
                instance.MailGreen = MailGreen;
                instance.MailBlue = MailBlue;
                instance.MailPurple = MailPurple;

                instance.SellGrey = SellGrey;
                instance.SellWhite = SellWhite;
                instance.SellGreen = SellGreen;
                instance.SellBlue = SellBlue;
                instance.SellPurple = SellPurple;

                instance.Save();
            }


            public bool OverrideProfileSettings { get; set; }
            public bool IgnoreCheckpoints { get; set; }

            public bool MailGrey { get; set; }
            public bool MailWhite { get; set; }
            public bool MailGreen { get; set; }
            public bool MailBlue { get; set; }
            public bool MailPurple { get; set; }

            public bool SellGrey { get; set; }
            public bool SellWhite { get; set; }
            public bool SellGreen { get; set; }
            public bool SellBlue { get; set; }
            public bool SellPurple { get; set; }
        }
    }
}
