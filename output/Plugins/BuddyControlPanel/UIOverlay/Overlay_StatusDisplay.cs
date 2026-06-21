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

using CopilotBuddy.Buddy.Overlay;
using CopilotBuddy.Buddy.Overlay.Controls;
using BuddyControlPanel.Resources.Localization;
using Styx;
using Styx.CommonBot;
using Styx.Logic.BehaviorTree;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    public class Overlay_StatusDisplay : OverlayUIComponent
    {
        #region Construction and Destruction
        private Overlay_StatusDisplay()
            : base(true /*isHitTestable, determines whether UI component can handle mouse clicks */)
        {
            StyxWoW.Overlay.AddUIComponent(this);

            BotEvents.OnBotStopped += BotEvents_OnBotStopped;
            TreeRoot.OnGoalTextChanged += TreeRootEvents_OnGoalTextChanged;
            TreeRoot.OnStatusTextChanged += TreeRootEvents_OnStatusTextChanged;
        }


        // Basic Dispose pattern (ref: https://msdn.microsoft.com/en-us/library/b1yfkh5e%28v=vs.110%29.aspx)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                BotEvents.OnBotStopped -= BotEvents_OnBotStopped;
                TreeRoot.OnGoalTextChanged -= TreeRootEvents_OnGoalTextChanged;
                TreeRoot.OnStatusTextChanged -= TreeRootEvents_OnStatusTextChanged;

                // Reclaim unmanaged resources (hooked elements) --
                // N.B.: Don't mess with _overlayControl here.  The Overlay will eliminate Control
                // when it is no longer needed.  Because Control is not IDisposable, we can't do
                // any activities that require cleaning up of unmanaged resources.  All such unmanaged
                // resource allocations and reclamation must be done in this (OverlayUIComponent) class,
                // instead.
                StyxWoW.Overlay.RemoveUIComponent(this);
                s_instance = null;
            }
        }


        private static Overlay_StatusDisplay s_instance;
        public static Overlay_StatusDisplay Instance
        {
            get
            {
                return s_instance ?? (s_instance = new Overlay_StatusDisplay());
            }
        }

        /// <summary>
        /// De-initializes the singleton instance.
        /// </summary>
        public static void ToJunkyard()
        {
            if (s_instance != null)
            {
                // If Control is moveable, save existing Control size and position...
                var overlayControl = s_instance._overlayControl;
                if (overlayControl != null)
                {
                    // N.B.: This code structured to prevent self-deadlock...
                    // We must fetch the settings using the _overlayControl thread, then save
                    // the settings using the Application main thread.
                    BuddyControlPanelSettings.StatusDisplaySettings settings = null;

                    Utility.InvokeOnSpecificDispatcher(overlayControl.Dispatcher, () =>
                    {
                        settings = s_instance.Configuration_Get();
                        settings.ControlLeft = overlayControl.X;
                        settings.ControlTop = overlayControl.Y;
                        settings.ControlWidth = overlayControl.Width;
                    });

                    if (settings != null)
                    {
                        BuddyControlPanelSettings.Instance.OverlayStatusDisplayConfiguration = settings;
                        BuddyControlPanelSettings.Instance.Save();
                    }
                }

                s_instance.Dispose();
                s_instance = null;
            }
        }
        #endregion


        private OverlayControl_StatusDisplay _overlayControl;
        public override OverlayControl Control
        {
            get
            {
                if (_overlayControl == null)
                {
                    var config = BuddyControlPanelSettings.Instance.OverlayStatusDisplayConfiguration
                                 ?? Configuration_GetDefaults();

                    _overlayControl = new OverlayControl_StatusDisplay(config);
                    _overlayControl.NotifyGoalTextChange(TreeRoot.GoalText);
                    _overlayControl.NotifyStatusTextChange(TreeRoot.StatusText);
                }
                return _overlayControl;
            }
        }



        #region Event Handlers
        // NB: Notification events may occur at odd times.  Thus, we must handle situations
        // where the _overlayControl may not be initialized.
        private void BotEvents_OnBotStopped(EventArgs args)
        {
            TreeRoot.GoalText = null;
            TreeRoot.StatusText = null;
        }


        private void TreeRootEvents_OnGoalTextChanged(object sender, GoalTextChangedEventArgs evt)
        {
            if (_overlayControl != null)
                _overlayControl.NotifyGoalTextChange(TreeRoot.GoalText);
        }


        private void TreeRootEvents_OnStatusTextChanged(object sender, StatusTextChangedEventArgs evt)
        {
            if (_overlayControl != null)
                _overlayControl.NotifyStatusTextChange(TreeRoot.StatusText);
        }
        #endregion


        #region Configuration
        public enum FontSizeChoiceEnum
        {
            None = 1,
            Small = 12,
            Normal = 16,
            Large = 20,
            ExtraLarge = 30,
        }


        public BuddyControlPanelSettings.StatusDisplaySettings Configuration_Get()
        {
            Contract.Requires(_overlayControl != null, () => "_overlayControl may not be null.");

            BuddyControlPanelSettings.StatusDisplaySettings settings = null;

            Utility.InvokeOnSpecificDispatcher(_overlayControl.Dispatcher, () =>
            {
                settings = new BuddyControlPanelSettings.StatusDisplaySettings()
                {
                    FontSizeChoice = _overlayControl.FontSizeChoice,
                    ControlLeft = _overlayControl.X,
                    ControlTop = _overlayControl.Y,
                    ControlWidth = _overlayControl.Width,
                    DragLocked = !_overlayControl.AllowMoving,
                };
            });

            return settings;
        }


        // N.B. Configuration_GetDefaults() must always be declared static.
        // This method is used to initialize instances (or the singleton), and we can't require
        // a valid instance of the object in order to determine the default values.
        public static BuddyControlPanelSettings.StatusDisplaySettings Configuration_GetDefaults()
        {
            Contract.Requires(StyxWoW.Overlay != null, () => "StyxWoW.Overlay may not be null.");

            var controlWidth = (int)(StyxWoW.Overlay.UnscaledOverlayWidth * 0.40); // Use 40% of dispay width

            return new BuddyControlPanelSettings.StatusDisplaySettings()
            {
                ControlLeft = (StyxWoW.Overlay.UnscaledOverlayWidth - controlWidth) / 2,
                ControlTop = 0,
                ControlWidth = controlWidth,
                FontSizeChoice = FontSizeChoiceEnum.Normal,
                DragLocked = false,
            };
        }


        public void Configuration_Set(BuddyControlPanelSettings.StatusDisplaySettings configuration)
        {
            Contract.Requires(configuration != null, () => "configuration may not be null");
            Contract.Requires(_overlayControl != null, () => "_overlayControl may not be null.");

            Utility.InvokeOnSpecificDispatcher(_overlayControl.Dispatcher, () =>
            {
                _overlayControl.AllowMoving = !configuration.DragLocked;
                _overlayControl.AllowResizing = !configuration.DragLocked;
                _overlayControl.FontSizeChoice = configuration.FontSizeChoice;
                _overlayControl.Width = configuration.ControlWidth;
                _overlayControl.X = configuration.ControlLeft;
                _overlayControl.Y = configuration.ControlTop;
            });
        }
        #endregion
    }


    internal class OverlayControl_StatusDisplay : OverlayControl
    {
        #region Creation & Destruction
        // N.B.: Because OverlayControl is not IDisposable, we can't be designed to do
        // any activities that require unmanaged resources.  All such activities must be conducte
        // by the OverlayUIControl containing class.
        public OverlayControl_StatusDisplay(BuddyControlPanelSettings.StatusDisplaySettings configuration)
        {
            Contract.Requires(configuration != null, () => "configuration may not be null");

            var grid = new Grid()
            {
                Background = Assets.BrushSemiTransparent,
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = GridLength.Auto },
                    new ColumnDefinition() { Width = GridLength.Auto },
                },
                MinWidth = configuration.ControlWidth,
                RowDefinitions =
                {
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                },
                Width = configuration.ControlWidth,
            };

            // Goal...
            _textBlockGoalLabel = new TextBlock()
            {
                Background = Assets.BrushTransparent,
                Foreground = Brushes.White,
                FontStyle = FontStyles.Normal,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Right,
                Padding = new Thickness(7, 3, 7, 0),
                Text = BCPGlobalization.Item_Goal_Label,
            };
            Grid.SetRow(_textBlockGoalLabel, 0);
            Grid.SetColumn(_textBlockGoalLabel, 0);
            grid.Children.Add(_textBlockGoalLabel);

            _textBlockGoal = new TextBlock()
            {
                Background = Assets.BrushTransparent,
                Foreground = Brushes.White,
                FontStyle = FontStyles.Oblique,
                FontWeight = FontWeights.Normal,
                HorizontalAlignment = HorizontalAlignment.Left,
                Padding = new Thickness(7, 3, 7, 0),
                TextWrapping = TextWrapping.WrapWithOverflow,
            };
            Grid.SetRow(_textBlockGoal, 0);
            Grid.SetColumn(_textBlockGoal, 1);
            grid.Children.Add(_textBlockGoal);

            // Status...
            _textBlockStatusLabel = new TextBlock()
            {
                Background = Assets.BrushTransparent,
                Foreground = Brushes.LightBlue,
                FontStyle = FontStyles.Normal,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Right,
                Padding = new Thickness(7, 3, 7, 3),
                Text = BCPGlobalization.Item_Status_Label,
            };
            Grid.SetRow(_textBlockStatusLabel, 1);
            Grid.SetColumn(_textBlockStatusLabel, 0);
            grid.Children.Add(_textBlockStatusLabel);

            _textBlockStatus = new TextBlock()
            {
                Background = Assets.BrushTransparent,
                Foreground = Brushes.LightBlue,
                FontStyle = FontStyles.Oblique,
                FontWeight = FontWeights.Normal,
                HorizontalAlignment = HorizontalAlignment.Left,
                Padding = new Thickness(7, 3, 7, 3),
                TextWrapping = TextWrapping.Wrap,
            };
            Grid.SetRow(_textBlockStatus, 1);
            Grid.SetColumn(_textBlockStatus, 1);
            grid.Children.Add(_textBlockStatus);

            AllowMoving = !configuration.DragLocked;
            AllowResizing = !configuration.DragLocked;
            Background = Assets.BrushTransparent;
            Content = grid;
            FontSizeChoice = configuration.FontSizeChoice;
            Foreground = Brushes.White;
            X = configuration.ControlLeft;
            Y = configuration.ControlTop;
            Width = configuration.ControlWidth;

            FontSizeChoice = configuration.FontSizeChoice;
        }
        #endregion

        private readonly TextBlock _textBlockGoal;
        private readonly TextBlock _textBlockGoalLabel;
        private readonly TextBlock _textBlockStatus;
        private readonly TextBlock _textBlockStatusLabel;

        private Overlay_StatusDisplay.FontSizeChoiceEnum _fontSizeChoice;
        public Overlay_StatusDisplay.FontSizeChoiceEnum FontSizeChoice
        {
            get { return _fontSizeChoice; }
            set
            {
                _fontSizeChoice = value;
                if (_fontSizeChoice == Overlay_StatusDisplay.FontSizeChoiceEnum.None)
                    Opacity = 0.0;
                else
                {
                    Opacity = 1.0;

                    _textBlockGoal.FontSize = (double)_fontSizeChoice;
                    _textBlockGoalLabel.FontSize = (double)_fontSizeChoice;
                    _textBlockStatus.FontSize = (double)_fontSizeChoice;
                    _textBlockStatusLabel.FontSize = (double)_fontSizeChoice;
                }
            }
        }

        public void NotifyGoalTextChange(string newText)
        {
            Utility.InvokeOnSpecificDispatcher(_textBlockGoal.Dispatcher, () =>
                _textBlockGoal.Text = !string.IsNullOrEmpty(newText) ? newText : BCPGlobalization.GeneralText_None);
        }

        public void NotifyStatusTextChange(string newText)
        {
            Utility.InvokeOnSpecificDispatcher(_textBlockStatus.Dispatcher, () =>
                _textBlockStatus.Text = !string.IsNullOrEmpty(newText) ? newText : BCPGlobalization.GeneralText_None);
        }
    }
}