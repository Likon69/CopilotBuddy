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
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Effects;

using CopilotBuddy.Buddy.Overlay;
using CopilotBuddy.Buddy.Overlay.Controls;
using BuddyControlPanel.Resources.Localization;
using CopilotBuddy.UI;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;

using Application = System.Windows.Application;
using Brushes = System.Windows.Media.Brushes;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantLambdaSignatureParentheses
#endregion


namespace BuddyControlPanel
{
    public class Overlay_ControlBar : OverlayUIComponent, IDisposable, IBotChangeListener, IBotStateChangeListener
    {
        #region Creation and Destruction
        private Overlay_ControlBar()
            : base(true /*isHitTestable, determines whether UI component can handle mouse clicks */)
        {
            StyxWoW.Overlay.AddUIComponent(this);

            // Register event handlers...
            BotEvents.OnBotChanged += BotEvents_OnBotChanged;
            BotEvents.OnBotPaused += BotEvents_OnBotPaused;
            BotEvents.OnBotResumed += BotEvents_OnBotResumed;
            BotEvents.OnBotStarted += BotEvents_OnBotStarted;
            BotEvents.OnBotStartRequested += BotEvents_OnBotStartRequested;
            BotEvents.OnBotStopped += BotEvents_OnBotStopped;
            BotEvents.OnBotStopRequested += BotEvents_OnBotStopRequested;
            BotEvents.Profile.OnNewOuterProfileLoaded += ProfileEvents_OnProfileChanged;
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

            // Remove event handlers...
            BotEvents.OnBotChanged -= BotEvents_OnBotChanged;
            BotEvents.OnBotPaused -= BotEvents_OnBotPaused;
            BotEvents.OnBotResumed -= BotEvents_OnBotResumed;
            BotEvents.OnBotStarted -= BotEvents_OnBotStarted;
            BotEvents.OnBotStartRequested -= BotEvents_OnBotStartRequested;
            BotEvents.OnBotStopped -= BotEvents_OnBotStopped;
            BotEvents.OnBotStopRequested -= BotEvents_OnBotStopRequested;
            BotEvents.Profile.OnNewOuterProfileLoaded -= ProfileEvents_OnProfileChanged;

            // Reclaim unmanaged resources (hooked elements) --
            // N.B.: Don't mess with _overlayControl here.  The Overlay will eliminate Control
            // when it is no longer needed.  Because Control is not IDisposable, we can't do
            // any activities that require cleaning up of unmanaged resources.  All such unmanaged
            // resource allocations and reclamation must be done in this (OverlayUIComponent) class,
            // instead.
            StyxWoW.Overlay.RemoveUIComponent(this);
            s_instance = null;
        }

        private static Overlay_ControlBar s_instance;
        public static Overlay_ControlBar Instance
        {
            get
            {
                return s_instance ?? (s_instance = new Overlay_ControlBar());
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
                    BuddyControlPanelSettings.ControlBarSettings settings = null;

                    Utility.InvokeOnSpecificDispatcher(overlayControl.Dispatcher, () =>
                    {
                        settings = s_instance.Configuration_Get();
                        settings.ControlLeft = overlayControl.X;
                        settings.ControlTop = overlayControl.Y;
                    });

                    if (settings != null)
                    {
                        BuddyControlPanelSettings.Instance.OverlayControlBarConfiguration = settings;
                        BuddyControlPanelSettings.Instance.Save();
                    }
                }

                s_instance.Dispose();
                s_instance = null;
            }
        }


        private OverlayControl_ControlBar _overlayControl;

        public override OverlayControl Control
        {
            get
            {
                if (_overlayControl == null)
                {
                    var config = BuddyControlPanelSettings.Instance.OverlayControlBarConfiguration
                                 ?? Configuration_GetDefaults();

                    _overlayControl = new OverlayControl_ControlBar(config);
                }
                return _overlayControl;
            }
        }
        #endregion


        public void NotifyBotChanged(BotBase newBot)
        {
            if (_overlayControl != null)
            {
                Utility.InvokeOnSpecificDispatcher(_overlayControl.Dispatcher, () =>
                    _overlayControl.NotifyBotChanged(newBot));
            }
        }


        public void NotifyProfileChanged()
        {
            if (_overlayControl != null)
            {
                Utility.InvokeOnSpecificDispatcher(_overlayControl.Dispatcher, () =>
                    _overlayControl.NotifyProfileChanged());
            }
        }


        public void NotifyStateChanged(CurrentBotStateNotification currentBotStateNotification)
        {
            if (_overlayControl != null)
            {
                Utility.InvokeOnSpecificDispatcher(_overlayControl.Dispatcher, () =>
                    _overlayControl.NotifyStateChanged(currentBotStateNotification));
            }
        }


        #region Configuration
        public enum ControlSizeEnum
        {
            Tiny = 16,
            VerySmall = 24,
            Small = 32,
            Normal = 48,
            Large = 64,
            ExtraLarge = 96,
        }


        public BuddyControlPanelSettings.ControlBarSettings Configuration_Get()
        {
            Contract.Requires(_overlayControl != null, () => "_overlayControl may not be null");

            BuddyControlPanelSettings.ControlBarSettings settings = null;

            Utility.InvokeOnSpecificDispatcher(_overlayControl.Dispatcher, () =>
            {
                settings = new BuddyControlPanelSettings.ControlBarSettings()
                {
                    ControlLeft = _overlayControl.X,
                    ControlSize = _overlayControl.ControlSize,
                    ControlTop = _overlayControl.Y,
                    DragLocked = !_overlayControl.AllowMoving,
                };
            });

            return settings;
        }


        // N.B. Configuration_GetDefaults() must always be declared static.
        // This method is used to initialize instances (or the singleton), and we can't require
        // a valid instance of the object in order to determine the default values.
        public static BuddyControlPanelSettings.ControlBarSettings Configuration_GetDefaults()
        {
            return new BuddyControlPanelSettings.ControlBarSettings()
            {
                ControlLeft = 90,
                ControlSize = ControlSizeEnum.Normal,
                ControlTop = 0,
                DragLocked = false,
            };
        }


        public void Configuration_Set(BuddyControlPanelSettings.ControlBarSettings configuration)
        {
            Contract.Requires(configuration != null, () => "configuration may not be null");
            Contract.Requires(_overlayControl != null, () => "_overlayControl may not be null");

            Utility.InvokeOnSpecificDispatcher(_overlayControl.Dispatcher, () =>
            {
                _overlayControl.ControlSize = configuration.ControlSize;
                _overlayControl.X = configuration.ControlLeft;
                _overlayControl.Y = configuration.ControlTop;
                _overlayControl.AllowMoving = !configuration.DragLocked;
            });
        }
        #endregion


        #region Events
        // NB: We sometimes tell OnBuddyBotStateChanged what the new state is, rather than having it ask TreeRoot.State.
        // This prevents boundary conditions such as TreeRoot.State being in the Stopping state (instead of
        // Stopped state) when OnBotStop is called.  It also allows us to discern a running state coming
        // from Stopped vs Resume (i.e., there is no TreeRootState.Resumed state).
        //
        // NB: Notification events may occur at odd times.  Thus, we must handle situations
        // where the _overlayControl may not be initialized.

        private void BotEvents_OnBotChanged(BotEvents.BotChangedEventArgs args)
        {
            if (_overlayControl != null)
                _overlayControl.NotifyBotChanged(BotManager.Current);
        }

        private void BotEvents_OnBotPaused(object obj, EventArgs args)
        {
            if (_overlayControl != null)
                _overlayControl.NotifyStateChanged(new CurrentBotStateNotification() { TreeRootState = TreeRoot.State });
            Utility.OverlayNotification(string.Format(BCPGlobalization.GeneralTextFormat_Event_Paused,
                                                        BCPGlobalization.BuddyBotName,
                                                        BotManager.Current.Name),
                                        Assets.ColorWarning);
        }

        private void BotEvents_OnBotResumed(object obj, EventArgs args)
        {
            if (_overlayControl != null)
                _overlayControl.NotifyStateChanged(new CurrentBotStateNotification() { TreeRootState = TreeRoot.State });
            Utility.OverlayNotification(string.Format(BCPGlobalization.GeneralTextFormat_Event_Resumed,
                                                        BCPGlobalization.BuddyBotName,
                                                        BotManager.Current.Name),
                                        Assets.ColorConfirmation);
        }

        private void BotEvents_OnBotStarted(EventArgs args)
        {
            if (_overlayControl != null)
                _overlayControl.NotifyStateChanged(new CurrentBotStateNotification() { TreeRootState = TreeRoot.State });
            Utility.OverlayNotification(string.Format(BCPGlobalization.GeneralTextFormat_Event_Running,
                                                        BCPGlobalization.BuddyBotName,
                                                        BotManager.Current.Name),
                                        Assets.ColorConfirmation);
        }

        private void BotEvents_OnBotStartRequested(EventArgs args)
        {
            TreeRoot.GoalText = null;
        }

        private void BotEvents_OnBotStopped(EventArgs args)
        {
            TreeRoot.GoalText = null;
            if (_overlayControl != null)
                _overlayControl.NotifyStateChanged(new CurrentBotStateNotification() { TreeRootState = TreeRootState.Stopped });
            Utility.OverlayNotification(string.Format(BCPGlobalization.GeneralTextFormat_Event_Stopped,
                                                        BCPGlobalization.BuddyBotName),
                                        Assets.ColorProblem);
        }

        private void BotEvents_OnBotStopRequested(EventArgs args)
        {
            if (_overlayControl != null)
                _overlayControl.NotifyStateChanged(new CurrentBotStateNotification() { TreeRootState = TreeRootState.Stopping });
            Utility.OverlayNotification(string.Format(BCPGlobalization.GeneralTextFormat_Event_Stopping,
                                                        BCPGlobalization.BuddyBotName,
                                                        BotManager.Current.Name),
                                        Assets.ColorProblem);
        }

        private void ProfileEvents_OnProfileChanged(EventArgs args)
        {
            if (_overlayControl != null)
                _overlayControl.NotifyProfileChanged();
        }
        #endregion		
    }


    public class OverlayControl_ControlBar : OverlayControl, IBotChangeListener, IBotStateChangeListener, IProfileChangeListener
    {
        #region Creation & Destruction
        // N.B.: Because OverlayControl is not IDisposable, we can't be designed to do
        // any activities that require unmanaged resources.  All such activities must be conducte
        // by the OverlayUIControl containing class.
        public OverlayControl_ControlBar(BuddyControlPanelSettings.ControlBarSettings configuration)
        {
            Contract.Requires(configuration != null, () => "configuration may not be null");

            // Build the main control...
            _grid = new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() {Width = GridLength.Auto},
                    new ColumnDefinition() {Width = GridLength.Auto},
                    new ColumnDefinition() {Width = GridLength.Auto},
                    new ColumnDefinition() {Width = GridLength.Auto},
                    new ColumnDefinition() {Width = GridLength.Auto},
                },
                RowDefinitions =
                {
                    new RowDefinition() {Height = new GridLength(1.0, GridUnitType.Star)},
                },
            };

            var textBlock_ProfileLoading = new Assets.ThemedTextBlock()
            {
                Effect = new DropShadowEffect(),
                FontSize = 16,
                FontStyle = FontStyles.Oblique,
                FontWeight = FontWeights.SemiBold,
                Foreground = Assets.BrushText,
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = string.Format(BCPGlobalization.GeneralTextFormat_ProfileLoadingPleaseWait, Environment.NewLine),
                TextWrapping = TextWrapping.NoWrap,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _viewBox_ProfileLoading = new Viewbox()
            {
                Child = textBlock_ProfileLoading,
                Effect = new DropShadowEffect(),
                HorizontalAlignment = HorizontalAlignment.Center,
                Stretch = Stretch.Fill,
                StretchDirection = StretchDirection.Both,
                Visibility = Visibility.Collapsed,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _grid.Children.Add(_viewBox_ProfileLoading);
            Grid.SetColumn(_viewBox_ProfileLoading, 2);
            Grid.SetColumnSpan(_viewBox_ProfileLoading, 3);
            Grid.SetRow(_viewBox_ProfileLoading, 0);
            Panel.SetZIndex(_viewBox_ProfileLoading, 200);

            _button_BuddyPopUpDown = new Button_BuddyPopUpDown(this, configuration.ControlSize)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(1, 0, 1, 0),
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            _grid.Children.Add(_button_BuddyPopUpDown);
            Grid.SetColumn(_button_BuddyPopUpDown, 0);
            Grid.SetRow(_button_BuddyPopUpDown, 0);
            Panel.SetZIndex(_button_BuddyPopUpDown, 100);

            var separator = new Assets.ThemedSeparator()
            {
                LayoutTransform = new RotateTransform(90.0),
                Margin = new Thickness(3, 7, 3, 7),
            };
            _grid.Children.Add(separator);
            Grid.SetColumn(separator, 1);
            Grid.SetRow(separator, 0);
            Panel.SetZIndex(separator, 100);

            _button_Stop = new Button_Stop(this, configuration.ControlSize)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(1, 0, 1, 0),
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            _grid.Children.Add(_button_Stop);
            Grid.SetColumn(_button_Stop, 2);
            Grid.SetRow(_button_Stop, 0);
            Panel.SetZIndex(_button_Stop, 100);

            _button_Start = new Button_Start(this, configuration.ControlSize)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(1, 0, 1, 0),
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            _grid.Children.Add(_button_Start);
            Grid.SetColumn(_button_Start, 3);
            Grid.SetRow(_button_Start, 0);
            Panel.SetZIndex(_button_Start, 100);

            _button_Pause = new Button_Pause(this, configuration.ControlSize)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(1, 0, 1, 0),
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            _grid.Children.Add(_button_Pause);
            Grid.SetColumn(_button_Pause, 4);
            Grid.SetRow(_button_Pause, 0);
            Panel.SetZIndex(_button_Pause, 100);

            // Context menu for the control...
            _contextMenu = new ContextMenu_Main();
            ContextMenuService.SetContextMenu(_grid, _contextMenu);
            ContextMenuService.SetPlacement(_grid, PlacementMode.Bottom);
            ContextMenuService.SetHasDropShadow(_grid, true);

            NotifyBotChanged(BotManager.Current);
            NotifyProfileChanged();
            NotifyStateChanged(new CurrentBotStateNotification() { TreeRootState = TreeRoot.State });

            AllowMoving = !configuration.DragLocked;
            AllowResizing = false;
            Background = Brushes.Transparent;
            ControlSize = configuration.ControlSize;
            Content = _grid;
            X = configuration.ControlLeft;
            Y = configuration.ControlTop;
        }
        #endregion

        private readonly ControlButtonBase _button_BuddyPopUpDown;
        private readonly ControlButtonBase _button_Pause;
        private readonly ControlButtonBase _button_Start;
        private readonly ControlButtonBase _button_Stop;
        private readonly ContextMenu_Main _contextMenu;
        private readonly Viewbox _viewBox_ProfileLoading;
        private readonly Grid _grid;


        public void NotifyBotChanged(BotBase newBot)
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
            {
                _button_BuddyPopUpDown.NotifyBotChanged(newBot);
                _button_Pause.NotifyBotChanged(newBot);
                _button_Start.NotifyBotChanged(newBot);
                _button_Stop.NotifyBotChanged(newBot);
                _contextMenu.NotifyBotChanged(newBot);
            });
        }


        public void NotifyProfileChanged()
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
            {
                _contextMenu.NotifyProfileChanged();
            });
        }

        public void NotifyStateChanged(CurrentBotStateNotification currentBotState)
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
            {
                _button_BuddyPopUpDown.NotifyStateChanged(currentBotState);
                _button_Pause.NotifyStateChanged(currentBotState);
                _button_Start.NotifyStateChanged(currentBotState);
                _button_Stop.NotifyStateChanged(currentBotState);
                _contextMenu.NotifyStateChanged(currentBotState);

                // Show panel info, if a profile is loading...
                if (currentBotState.IsLoadProfileInProgress.HasValue)
                {
                    _viewBox_ProfileLoading.Visibility =
                        currentBotState.IsLoadProfileInProgress.Value
                            ? Visibility.Visible
                            : Visibility.Collapsed;
                }
            });
        }


        public void DisableControlButtons()
        {
            // Provide immediate feedback to the user that the click was recognized...
            _button_Pause.IsEnabled = false;
            _button_Start.IsEnabled = false;
            _button_Stop.IsEnabled = false;
        }


        private Overlay_ControlBar.ControlSizeEnum _controlSize;
        public Overlay_ControlBar.ControlSizeEnum ControlSize
        {
            get { return _controlSize; }
            set
            {
                _controlSize = value;
                _button_BuddyPopUpDown.NotifyNewControlSize(_controlSize);
                _button_Pause.NotifyNewControlSize(_controlSize);
                _button_Start.NotifyNewControlSize(_controlSize);
                _button_Stop.NotifyNewControlSize(_controlSize);

                _grid.Height = (int)_controlSize;
            }
        }
    }


    internal abstract class ControlButtonBase : Assets.ThemedButton, IBotChangeListener, IBotStateChangeListener
    {
        protected ControlButtonBase(
            OverlayControl_ControlBar owner,
            Overlay_ControlBar.ControlSizeEnum controlSize,
            string iconName,
            string toolTip)
        {
            _controlSize = controlSize;
            _iconName = iconName;
            _owner = owner;

            Background = Assets.BrushTransparent;
            Content = new Image() { Source = Utility.ToImageSource(_iconName, (int)controlSize) };
            HorizontalAlignment = HorizontalAlignment.Center;
            Padding = new Thickness(2, 0, 2, 0);
            ToolTip = toolTip;
            VerticalAlignment = VerticalAlignment.Center;
        }

        private Overlay_ControlBar.ControlSizeEnum _controlSize;
        private readonly string _iconName;
        protected readonly OverlayControl_ControlBar _owner;

        public virtual void NotifyBotChanged(BotBase newBot) { /*empty*/ }
        public virtual void NotifyStateChanged(CurrentBotStateNotification currentBotStateNotification) { /*empty*/ }

        public void NotifyNewControlSize(Overlay_ControlBar.ControlSizeEnum controlSize)
        {
            _controlSize = controlSize;
            Content = new Image() { Source = Utility.ToImageSource(_iconName, (int)_controlSize) };
        }

        // Convenience methods for derived-classes...
        protected void SetToolTip(string message, BotBase newBot)
        {
            var botName = ((newBot != null) && !string.IsNullOrEmpty(newBot.Name))
                ? newBot.Name
                : BCPGlobalization.BuddyBotName;
            var messageContextMenu = BCPGlobalization.GeneralText_MouseRightForContextMenu;

            if (message == null)
                ToolTip = messageContextMenu;
            else
            {
                message = string.Format(message, botName);
                ToolTip = string.Format("{1}{0}{2}", Environment.NewLine, message, messageContextMenu);
            }
        }
    }


    internal class Button_BuddyPopUpDown : ControlButtonBase
    {
        public Button_BuddyPopUpDown(OverlayControl_ControlBar owner, Overlay_ControlBar.ControlSizeEnum controlSize)
            : base(owner, controlSize, "logo-honorbuddy.png",
                    string.Format(BCPGlobalization.Button_BuddyPopUpDown_ToolTipFormat, BCPGlobalization.BuddyBotName))
        {
            // N.B. Since the Notify*() methods are virtual, we can't (legally, reliably) call them here.
            // So, we'll leave it for the parent to do this and finish the initialization.
        }

        protected override void OnClick()
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                // Make certain any actions we take happen on the 'MainWindow thread' (HB API is not MT-safe)...
                Utility.InvokeOnSpecificDispatcher(Application.Current.Dispatcher, () =>
                {
                    var mainWindow = (MainWindow)Application.Current.MainWindow;

                    // If Window is open, then minimize it...
                    if (mainWindow.WindowState != WindowState.Minimized)
                    {
                        Utility.OverlayNotification(string.Format(BCPGlobalization.GeneralTextFormat_MinimizingBuddyBot,
                                                                    BCPGlobalization.BuddyBotName),
                                                        Assets.ColorWarning);
                        mainWindow.WindowState = WindowState.Minimized;
                    }

                    // Otherwise, window is minimized, so open it...
                    else
                    {
                        Utility.OverlayNotification(string.Format(BCPGlobalization.GeneralTextFormat_RestoringBuddyBot,
                                                                BCPGlobalization.BuddyBotName),
                                                    Assets.ColorConfirmation);
                        mainWindow.WindowState = WindowState.Normal;
                        mainWindow.ShowAtFront();
                    }
                });
            });

            base.OnClick();
        }

        public override void NotifyBotChanged(BotBase newBot)
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
                SetToolTip(BCPGlobalization.Button_BuddyPopUpDown_ToolTipFormat, null));
        }

        public override void NotifyStateChanged(CurrentBotStateNotification currentBotState)
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
            {
                Background =
                    (currentBotState.TreeRootState == TreeRootState.Paused) ? Assets.BrushWarning
                    : (currentBotState.TreeRootState == TreeRootState.Running) ? Assets.BrushConfirmation
                    : Brushes.Transparent;
            });
        }
    }


    internal class Button_Pause : ControlButtonBase
    {
        public Button_Pause(OverlayControl_ControlBar owner, Overlay_ControlBar.ControlSizeEnum controlSize)
            : base(owner, controlSize, "Actions-media-playback-pause-icon.png", BCPGlobalization.Button_Pause_Label)
        {
            // N.B. Since the Notify*() methods are virtual, we can't call (legally, reliably) them here.
            // So, we'll leave it for the parent to do this and finish the initialization.
        }

        protected override void OnClick()
        {
            // Provide immediate feedback to the user that the click was recognized...
            _owner.DisableControlButtons();

            // N.B.: Some bots are multi-threaded (e.g., ArchaeologyBuddy), and we must defer this work
            // to another thread; otherwise, deadlock can ensue.
            ThreadPool.QueueUserWorkItem(o =>
            {
                Utility.InvokeOnSpecificDispatcher(Application.Current.Dispatcher, () =>
                {
                    if (TreeRoot.IsPaused)
                        TreeRoot.Resume();
                    else if (TreeRoot.IsRunning)
                        TreeRoot.Pause();
                });
            });

            base.OnClick();
        }

        public override void NotifyBotChanged(BotBase newBot)
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
                SetToolTip(BCPGlobalization.Button_Pause_ToolTipFormat, newBot));
        }

        public override void NotifyStateChanged(CurrentBotStateNotification currentBotState)
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
            {
                // Disable button, if profile load in progress...
                if (currentBotState.IsLoadProfileInProgress.HasValue)
                {
                    IsEnabled = !currentBotState.IsLoadProfileInProgress.Value;
                    if (!IsEnabled)
                        return;
                }

                // Disable button, if we're not running...
                if (currentBotState.TreeRootState.HasValue)
                {
                    IsEnabled = (currentBotState.TreeRootState == TreeRootState.Running)
                                || (currentBotState.TreeRootState == TreeRootState.Paused);
                }
            });
        }
    }


    internal class Button_Start : ControlButtonBase
    {
        public Button_Start(OverlayControl_ControlBar owner, Overlay_ControlBar.ControlSizeEnum controlSize)
            : base(owner, controlSize, "Actions-media-playback-start-icon.png", BCPGlobalization.Button_Start_Label)
        {
            // N.B. Since the Notify*() methods are virtual, we can't (legally, reliably) call them here.
            // So, we'll leave it for the parent to do this and finish the initialization.
        }

        protected override void OnClick()
        {
            // Provide immediate feedback to the user that the click was recognized...
            _owner.DisableControlButtons();

            // N.B.: Some bots are multi-threaded (e.g., ArchaeologyBuddy), and we must defer this work
            // to another thread; otherwise, deadlock can ensue.
            ThreadPool.QueueUserWorkItem(o =>
            {
                Utility.InvokeOnSpecificDispatcher(Application.Current.Dispatcher, () =>
                {
                    if (!TreeRoot.IsRunning)
                    {
                        // Capture any messages that may result from a failed bot launch...
                        var finalPhrase = BCPGlobalization.GeneralText_FinalLogPhrase;
                        var logWatcher = new LogWatcher(finalPhrase);

                        TreeRoot.Start();

                        // If start was successful, we don't want to emit any log entries...
                        if (TreeRoot.IsRunning)
                            logWatcher.Dispose();

                        // On a failed start, tell the user what went' wrong...
                        else
                        {
                            NotifyStateChanged(new CurrentBotStateNotification() { TreeRootState = TreeRootState.Stopped });
                            // Use normal logging here...
                            // These messages will be reflected as 'toasts' on the game-client screen. Our PluginLog facilities
                            // will adorn the messages with the plugin name and version, and we don't want that for toast messages.
                            Logging.Write(Assets.ColorProblem,
                                BCPGlobalization.GeneralText_FailedToStartBot,
                                BCPGlobalization.BuddyBotName,
                                BotManager.Current.Name);
                            Logging.Write(Assets.ColorProblem, finalPhrase);
                        }
                    }
                });
            });

            base.OnClick();
        }

        public override void NotifyBotChanged(BotBase newBot)
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
                SetToolTip(BCPGlobalization.Button_Start_ToolTipFormat, newBot));
        }

        public override void NotifyStateChanged(CurrentBotStateNotification currentBotState)
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
            {
                // Disable button, if a profile is loading...
                if (currentBotState.IsLoadProfileInProgress.HasValue)
                {
                    IsEnabled = !currentBotState.IsLoadProfileInProgress.Value;
                    if (!IsEnabled)
                        return;
                }

                // Disable button, if bot is already running...
                IsEnabled = !TreeRoot.IsRunning;
            });
        }
    }


    internal class Button_Stop : ControlButtonBase
    {
        public Button_Stop(OverlayControl_ControlBar owner, Overlay_ControlBar.ControlSizeEnum controlSize)
            : base(owner, controlSize, "Actions-media-playback-stop-icon.png", BCPGlobalization.Button_Stop_Label)
        {
            // N.B. Since the Notify*() methods are virtual, we can't (legally, reliably) call them here.
            // So, we'll leave it for the parent to do this and finish the initialization.
        }

        protected override void OnClick()
        {
            // Provide immediate feedback to the user that the click was recognized...
            _owner.DisableControlButtons();

            // N.B.: Some bots are multi-threaded (e.g., ArchaeologyBuddy), and we must defer this work
            // to another thread; otherwise, deadlock can ensue.
            ThreadPool.QueueUserWorkItem(o =>
            {
                Utility.InvokeOnSpecificDispatcher(Application.Current.Dispatcher, () =>
                {
                    if (TreeRoot.IsRunning || TreeRoot.IsPaused)
                        TreeRoot.Stop();
                });
            });

            base.OnClick();
        }

        public override void NotifyBotChanged(BotBase newBot)
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
                SetToolTip(BCPGlobalization.Button_Stop_ToolTipFormat, newBot));
        }

        public override void NotifyStateChanged(CurrentBotStateNotification currentBotState)
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
            {
                // Disable button, if a profile is loading...
                if (currentBotState.IsLoadProfileInProgress.HasValue)
                {
                    IsEnabled = !currentBotState.IsLoadProfileInProgress.Value;
                    if (!IsEnabled)
                        return;
                }

                // Disable button, if bot is not running...
                if (currentBotState.TreeRootState.HasValue)
                {
                    IsEnabled = ((currentBotState.TreeRootState == TreeRootState.Running)
                                    || (currentBotState.TreeRootState == TreeRootState.Paused));
                }
            });
        }
    }
}