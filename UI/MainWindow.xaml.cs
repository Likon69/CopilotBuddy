using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using GreenMagic;
using Styx;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Profiles;
using Styx.WoWInternals;

using WpfFontFamily = System.Windows.Media.FontFamily;

namespace CopilotBuddy.UI
{
    /// <summary>
    /// Main window for CopilotBuddy - HB 5.4.8 style interface.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private Memory? _memory;
        private ExecutorRand? _executor;
        private Paragraph? _logParagraph;
        private WpfFontFamily? _logFont;
        private int _logCount;
        private const int MaxLogLines = 1000;
        private bool _isRunning;
        private DispatcherTimer? _infoTimer;

        /// <summary>
        /// Mutex that claims ownership of the attached WoW process.
        /// Prevents another CopilotBuddy from attaching to the same WoW.
        /// HB 4.3.4 pattern: held for entire session lifetime.
        /// </summary>
        private Mutex? _processMutex;

        /// <summary>
        /// Selected WoW process ID from the process selector.
        /// null = pending selection, 0 = user cancelled.
        /// HB 4.3.4 pattern.
        /// </summary>
        private int? _selectedWoWProc;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            // Setup logging
            InitializeLogging();

            // Setup info panel timer
            InitializeInfoTimer();

            // Log version (HB 4.3.4: "Honorbuddy v{0} started.")
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Logging.Write("CopilotBuddy v{0}.{1}.{2} started.", version.Major, version.Minor, version.Build);
        }

        #endregion

        #region Initialization

        private void InitializeLogging()
        {
            // Initialize RichTextBox
            rtbLog.Document.Blocks.Clear();
            _logParagraph = new Paragraph();
            rtbLog.Document.Blocks.Add(_logParagraph);
            _logFont = new WpfFontFamily("Consolas");
            _logCount = 0;

            // Setup log file path
            var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);
            var timestamp = DateTime.Now;
            Logging.LogFilePath = Path.Combine(logDir,
                $"{timestamp.Year}-{timestamp.Month:D2}-{timestamp.Day:D2}_{timestamp.Hour:D2}{timestamp.Minute:D2}_{Process.GetCurrentProcess().Id}.log");

            // Subscribe to logging events
            Logging.OnLogMessage += OnLogMessage;
        }

        private void InitializeInfoTimer()
        {
            _infoTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _infoTimer.Tick += (s, e) => UpdateInfoPanel();
            
            // Subscribe to StatusText changes — updates StatusBar like HB 4.3.4
            TreeRoot.OnStatusTextChanged += (sender, e) =>
            {
                Dispatcher.BeginInvoke(() => lblActivityText.Content = e.NewStatus ?? "Ready");
            };
            
            // Show default values immediately
            UpdateInfoPanel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load UI settings and restore window position/size
            try
            {
                UISettings.Instance.Load();
                
                if (UISettings.Instance.MainWindowWidth > 0)
                    this.Width = UISettings.Instance.MainWindowWidth;
                if (UISettings.Instance.MainWindowHeight > 0)
                    this.Height = UISettings.Instance.MainWindowHeight;
                if (UISettings.Instance.MainWindowLocationX >= 0)
                    this.Left = UISettings.Instance.MainWindowLocationX;
                if (UISettings.Instance.MainWindowLocationY >= 0)
                    this.Top = UISettings.Instance.MainWindowLocationY;
                if (UISettings.Instance.MainWindowState != WindowState.Minimized)
                    this.WindowState = UISettings.Instance.MainWindowState;
                    
                // Restore Enhanced Mode checkbox state
                if (UISettings.Instance.EnhancedMode)
                {
                    EnhancedMode.IsChecked = true;
                }
            }
            catch { /* Ignore errors on first run */ }

            SetStatus("Searching for WoW...");
            ToggleButtons(false);

            Task.Run(() =>
            {
                try
                {
                    if (!FindAndAttachToWoW())
                        return;

                    if (ObjectManager.IsInitialized)
                    {
                        Logging.Write("Attached to WoW with ID {0}", _selectedWoWProc!.Value);

                        // HB 6.2.3 smethod_3: Start minimize guard thread + event handlers
                        TreeRoot.Initialize();
                        BotEvents.OnBotStopped += BotEvents_OnBotStopped;

                        // Log character info (HB 4.3.4 pattern after attach)
                        if (ObjectManager.Me != null)
                        {
                            Logging.Write("Character is a level {0} {1} {2}",
                                ObjectManager.Me.Level,
                                ObjectManager.Me.Race,
                                ObjectManager.Me.Class);
                            // Honorbuddy 3.3.5a simply logged RealZoneText
                            Logging.Write("Current zone is {0}", ObjectManager.Me.RealZoneText);
                        }

                        // Reinitialize and load settings for this character (HB 4.3.4 pattern)
                        LoadSettings();

                        // Initialize Combat Routines (compile and load from Routines folder)
                        // This must be done AFTER attachment when ObjectManager.Me is available
                        try
                        {
                            Styx.Logic.Combat.RoutineManager.Init();
                        }
                        catch (Exception ex)
                        {
                            Logging.Write(Colors.Red, "Failed to initialize combat routines: {0}", ex.Message);
                            Logging.WriteException(ex);
                        }

                        // Load additional bots from Bots folder (external/custom bots)
                        try
                        {
                            string botsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bots");
                            if (Directory.Exists(botsPath))
                            {
                                BotManager.Instance.LoadBots(botsPath);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.WriteException(ex);
                        }

                        // Initialize plugins (compile and load from Plugins folder)
                        try
                        {
                            Logging.Write("Initializing Plugins...");
                            
                            // Pre-load System.Drawing.Common for plugin compilation
                            try
                            {
                                // Force load System.Drawing.Common into AppDomain
                                var _ = System.Drawing.Font.FromHdc(IntPtr.Zero);
                            }
                            catch { /* Ignore, just ensure assembly is loaded */ }
                            
                            // Load previously enabled plugins from CharacterSettings
                            var enabledPlugins = CharacterSettings.Instance.EnabledPlugins ?? new string[0];
                            Styx.Plugins.PluginManager.Initialize(enabledPlugins);
                        }
                        catch (Exception ex)
                        {
                            Logging.Write(Colors.Red, "Failed to initialize plugins: {0}", ex.Message);
                            Logging.WriteException(ex);
                        }

                        Dispatcher.Invoke(() =>
                        {
                            // Populate bot selector with all loaded bots (built-in + external)
                            cmbBotSelector.Items.Clear();
                            foreach (var bot in BotManager.Instance.Bots)
                            {
                                cmbBotSelector.Items.Add(bot.Key);
                            }
                            
                            // Restore last selected bot (HB 4.3.4 pattern)
                            if (cmbBotSelector.Items.Count > 0)
                            {
                                int selectedIndex = CharacterSettings.Instance.SelectedBotIndex;
                                if (selectedIndex < 0 || selectedIndex >= cmbBotSelector.Items.Count)
                                {
                                    // Invalid index, reset to 0
                                    CharacterSettings.Instance.SelectedBotIndex = 0;
                                    selectedIndex = 0;
                                }
                                cmbBotSelector.SelectedIndex = selectedIndex;
                            }

                            ToggleButtons(true);
                            SetStatus("CopilotBuddy Startup Complete");
                            _infoTimer.Start();
                        });
                    }
                    else
                    {
                        Logging.WriteQuiet(Colors.Red, "Failed to initialize ObjectManager");
                        Dispatcher.Invoke(() => SetStatus("Initialization failed"));
                    }
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    Dispatcher.Invoke(() =>
                    {
                        SetStatus("Error");
                        System.Windows.MessageBox.Show(
                            $"Injection failed!\n\nError: {ex.Message}",
                            "CopilotBuddy - Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    });
                }
            });
        }

        /// <summary>
        /// Finds available WoW processes, handles auto-attach or process selection.
        /// Ported from HB 4.3.4 MainWindow.FindAndAttachToWoW() with 6.2.3 improvements.
        /// 
        /// Logic:
        ///   1. Check /pid= command line arg for explicit process selection
        ///   2. Find all Wow.exe processes matching build 12340
        ///   3. Filter: logged in + not claimed by another CopilotBuddy (via mutex)
        ///   4. If 0 available: show error
        ///   5. If 1 available: auto-attach
        ///   6. If 2+: show ProcessSelectorWindow
        ///   7. Attach to selected process
        /// </summary>
        private bool FindAndAttachToWoW()
        {
            Logging.Write("Searching for WoW process...");

            // Step 1: Check for /pid= command line argument
            string? pidArg = Environment.GetCommandLineArgs()
                .FirstOrDefault(s => s.StartsWith("/pid=", StringComparison.OrdinalIgnoreCase));

            _selectedWoWProc = null;
            var candidates = new List<Process>();

            if (pidArg == null)
            {
                // Find ALL Wow processes
                var wowProcesses = Process.GetProcessesByName("Wow");
                if (wowProcesses.Length == 0)
                    wowProcesses = Process.GetProcessesByName("WoW");

                foreach (var proc in wowProcesses)
                {
                    try
                    {
                        if (proc.HasExited) continue;
                        int build = proc.MainModule?.FileVersionInfo.FilePrivatePart ?? 0;
                        // build == 0 means no version resource (custom/private server client)
                        if (build == ObjectManager.SupportedBuild || build == 0)
                            candidates.Add(proc);
                    }
                    catch { /* Access denied or process exited */ }
                }
            }
            else
            {
                // Explicit PID from command line
                string pidStr = pidArg.Substring("/pid=".Length).Trim();
                if (int.TryParse(pidStr, out int pid))
                {
                    try
                    {
                        var proc = Process.GetProcessById(pid);
                        if (!proc.HasExited)
                        {
                            int build = proc.MainModule?.FileVersionInfo.FilePrivatePart ?? 0;
                            if (build == ObjectManager.SupportedBuild)
                                candidates.Add(proc);
                        }
                    }
                    catch
                    {
                        Logging.Write(Colors.Red, "Process with PID {0} not found or inaccessible.", pid);
                    }
                }
            }

            if (candidates.Count == 0)
            {
                Logging.WriteQuiet(Colors.Red, "WoW.exe not found! Please launch WoW 3.3.5a first.");
                Dispatcher.Invoke(() => SetStatus("WoW not found"));
                return false;
            }

            // Step 2: Filter by logged-in state + mutex availability
            var available = new List<KeyValuePair<int, Mutex>>();
            foreach (var proc in candidates)
            {
                try
                {
                    using var tempMemory = new Memory(proc.Id);

                    // Check if player is logged in
                    bool isLoggedIn = tempMemory.Read<byte>(Styx.Offsets.GlobalOffsets.InGame) != 0;
                    if (!isLoggedIn)
                        continue;

                    // Try to claim the process via mutex
                    var mutex = ProcessMutex.Create(proc.Id, out bool createdNew);
                    if (!createdNew)
                    {
                        // Another CopilotBuddy already owns this process
                        mutex.Close();
                        continue;
                    }

                    available.Add(new KeyValuePair<int, Mutex>(proc.Id, mutex));
                }
                catch { /* Process exited or memory read failed */ }
            }

            if (available.Count == 0)
            {
                Logging.WriteQuiet(Colors.Red, "No available WoW processes. They may not be logged in or already attached by another CopilotBuddy.");
                Dispatcher.Invoke(() => SetStatus("No available WoW"));
                return false;
            }

            // Step 3: Auto-attach if only one, otherwise show selector
            if (available.Count == 1)
            {
                _selectedWoWProc = available[0].Key;
                _processMutex = available[0].Value;
            }
            else
            {
                // Release all mutexes — ProcessSelectorWindow will acquire its own
                foreach (var kvp in available)
                    kvp.Value.Close();

                // Show selector on UI thread (HB 4.3.4 pattern)
                _selectedWoWProc = null;
                ProcessSelectorWindow.ProcessEntry? selectedEntry = null;

                Dispatcher.Invoke(() =>
                {
                    var psw = new ProcessSelectorWindow { Owner = this };
                    if (psw.ShowDialog() == true && psw.Entry != null)
                    {
                        selectedEntry = psw.Entry;
                        _selectedWoWProc = psw.Entry.ProcessId;
                    }
                    else
                    {
                        _selectedWoWProc = 0; // Cancelled
                    }
                });

                if (_selectedWoWProc == null || _selectedWoWProc == 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        SetStatus("Cancelled");
                        Close();
                    });
                    return false;
                }

                // Use the mutex from the dialog directly — it's already acquired
                _processMutex = selectedEntry!.Mutex;
            }

            // Step 4: Attach to selected process
            int wowPid = _selectedWoWProc.Value;
            Logging.Write(Colors.Green, "Please wait a few seconds while CopilotBuddy initializes.");
            Logging.WriteDebug("Using WoW with process ID {0}", wowPid);
            Logging.WriteDebug("Platform: {0}", Environment.Is64BitProcess ? "x64" : "x86");
            Logging.WriteDebug("Executable Path: {0}", AppDomain.CurrentDomain.BaseDirectory);

            try
            {
                var memory = new Memory(wowPid);

                // Verify still logged in before committing
                if (memory.Read<byte>(Styx.Offsets.GlobalOffsets.InGame) == 0)
                {
                    memory.Dispose();
                    Logging.Write(Colors.Red, "Process ID {0} is not logged in game!", wowPid);
                    Dispatcher.Invoke(() => SetStatus("Not logged in"));
                    return false;
                }

                ObjectManager.Initialize(memory);

                _memory = memory;
                _executor = ObjectManager.Executor;
                return true;
            }
            catch (Exception ex)
            {
                Logging.Write(Colors.Red, "Error attaching to WoW! - {0}", ex.Message);
                Logging.WriteException(ex);
                Dispatcher.Invoke(() => SetStatus("Attach failed"));
                return false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Stop bot if running
            if (_isRunning)
            {
                StopBot();
            }

            // Save all settings (HB 4.3.4 pattern - save at close)
            SaveSettings();

            // Release process mutex so another CopilotBuddy can claim this WoW
            try
            {
                _processMutex?.Dispose();
                _processMutex = null;
            }
            catch { /* Mutex may already be released */ }

            // Save window position/size
            try
            {
                if (this.WindowState == WindowState.Normal)
                {
                    UISettings.Instance.MainWindowWidth = (int)this.Width;
                    UISettings.Instance.MainWindowHeight = (int)this.Height;
                    UISettings.Instance.MainWindowLocationX = (int)this.Left;
                    UISettings.Instance.MainWindowLocationY = (int)this.Top;
                }
                UISettings.Instance.MainWindowState = this.WindowState;
                UISettings.Instance.Save();
            }
            catch { /* Ignore errors */ }

            // Cleanup
            Logging.OnLogMessage -= OnLogMessage;
            BotEvents.OnBotStopped -= BotEvents_OnBotStopped;
            _infoTimer?.Stop();
        }

        #endregion

        #region Logging

        private void OnLogMessage(ReadOnlyCollection<LogMessage> messages)
        {
            if (Thread.CurrentThread != Dispatcher.Thread)
            {
                Dispatcher.BeginInvoke(new Logging.LogMessageDelegate(OnLogMessage), messages);
                return;
            }

            // Clear if too many lines
            if (_logCount >= MaxLogLines)
            {
                _logParagraph.Inlines.Clear();
                _logCount = 0;
            }

            foreach (var message in messages)
            {
                // Filter by log level - only show messages at or below current LoggingLevel
                if (message.Level > Logging.LoggingLevel)
                    continue;

                // Create colored run
                var brush = new SolidColorBrush(message.Color);
                brush.Freeze();

                var run = new Run(message.ToString() + Environment.NewLine)
                {
                    Foreground = brush,
                    FontFamily = _logFont
                };

                _logParagraph.Inlines.Add(run);
                _logCount++;
            }

            // Auto-scroll to end
            scrollLog.ScrollToEnd();
        }

        #endregion

        #region Info Panel

        private void UpdateInfoPanel()
        {
            // HB 4.3.4 pattern: always display current values; no IsMeasuring gate.
            // Values start at 0 and accumulate once the bot starts.
            var sb = new System.Text.StringBuilder();

            sb.AppendLine($"XP/HR: {GameStats.XPPerHour:F0}");
            sb.AppendLine($"Kills: {GameStats.MobsKilled} ({GameStats.MobsPerHour:F0}/hr)");
            sb.AppendLine($"Deaths: {GameStats.Deaths} ({GameStats.DeathsPerHour:F0}/hr)");
            sb.AppendLine($"Loots: {GameStats.Loots} ({GameStats.LootsPerHour:F0}/hr)");
            sb.AppendLine($"Honor Gained: {GameStats.HonorGained} ({GameStats.HonorPerHour:F0}/hr)");
            sb.AppendLine($"BGs Won: {GameStats.BGsWon} Lost: {GameStats.BGsLost} Total: {GameStats.BGsCompleted} ({GameStats.BGsPerHour:F0}/hr)");

            // TTL only meaningful below max level (80 in WotLK 3.3.5a)
            var me = ObjectManager.Me;
            if (me != null && me.Level < 80 && GameStats.TimeToLevel > TimeSpan.Zero)
            {
                var ttl = GameStats.TimeToLevel;
                sb.AppendLine($"Time to Level: {(int)ttl.TotalHours} hour(s) {ttl.Minutes:00} minute(s) {ttl.Seconds:00} second(s)");
            }

            // Goal text — polled every second like HB 4.3.4
            var goalText = TreeRoot.GoalText;
            if (!string.IsNullOrEmpty(goalText))
            {
                sb.AppendLine();
                sb.AppendLine($"Goal: {goalText}");
            }

            tbInfoBlock.Text = sb.ToString();
            tbInfoBlock.TextWrapping = System.Windows.TextWrapping.Wrap;
        }

        #endregion

        #region UI Helpers

        private void ToggleButtons(bool enabled)
        {
            btnLoadProfile.IsEnabled = enabled;
            btnStart.IsEnabled = enabled;
            btnSettings.IsEnabled = enabled;
            cmbBotSelector.IsEnabled = enabled;
            btnBotConfig.IsEnabled = enabled;
            btnClassConfig.IsEnabled = enabled;
            btnDevTools.IsEnabled = enabled;
            btnPlugins.IsEnabled = enabled;
            EnhancedMode.IsEnabled = enabled;
        }

        private void SetStatus(string status)
        {
            lblActivityText.Content = status;
        }

        #endregion

        #region Bot Control

        private void StartBot()
        {
            if (_isRunning) return;

            if (BotManager.Current == null)
            {
                Logging.Write(Colors.Red, "No bot selected!");
                return;
            }

            // Save settings before starting bot (HB 4.3.4 pattern)
            SaveSettings();

            _isRunning = true;
            btnStart.Visibility = Visibility.Hidden;
            btnStop.Visibility = Visibility.Visible;
            btnLoadProfile.IsEnabled = false;
            cmbBotSelector.IsEnabled = false;
            btnSettings.IsEnabled = false;

            try
            {
                Logging.Write("Starting the bot.");
                Logging.Write("Currently Using BotBase : {0}", BotManager.Current.Name);
                if (ObjectManager.Me != null)
                {
                    Logging.Write("Character is a level {0} {1} {2}",
                        ObjectManager.Me.Level,
                        ObjectManager.Me.Race,
                        ObjectManager.Me.Class);
                    Task.Run(() =>
                    {
                        Logging.Write("Current zone is {0}", ObjectManager.Me.RealZoneText);
                    });
                }
                TreeRoot.Start();
                SetStatus("Running...");
            }
            catch (HonorbuddyUnableToStartException ex)
            {
                Logging.Write(Colors.Red, ex.Message);
                Logging.WriteException(ex);
                StopBot();
            }
            catch (UserException ex)
            {
                Logging.Write(Colors.Red, ex.Message);
                Logging.WriteException(ex);
                StopBot();
            }
            catch (Exception ex)
            {
                Logging.Write(Colors.Red, ex.Message);
                Logging.WriteException(ex);
                StopBot();
            }
        }

        private void StopBot()
        {
            if (!_isRunning) return;

            _isRunning = false;
            btnStart.Visibility = Visibility.Visible;
            btnStop.Visibility = Visibility.Hidden;
            SetStatus("Stopping...");

            Logging.Write("Stopping the bot!");
            TreeRoot.Stop();
        }

        #endregion

        #region Button Click Handlers

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartBot();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            StopBot();
        }

        private void BotEvents_OnBotStopped(EventArgs args)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(new Action<EventArgs>(BotEvents_OnBotStopped), args);
                return;
            }

            _isRunning = false;
            btnStart.Visibility = Visibility.Visible;
            btnStop.Visibility = Visibility.Hidden;
            btnLoadProfile.IsEnabled = true;
            cmbBotSelector.IsEnabled = true;
            btnSettings.IsEnabled = true;
            SetStatus("CopilotBuddy Stopped");
        }

        private void cmbBotSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBotSelector.SelectedItem == null) return;

            string? botName = cmbBotSelector.SelectedItem.ToString();
            if (botName != null && BotManager.Instance.Bots.TryGetValue(botName, out BotBase? bot) && bot != null)
            {
                BotManager.Instance.SetCurrent(bot);
                
                // Show Bot Config button — always visible when a bot is selected.
                // The handler checks ConfigurationForm at click time.
                btnBotConfig.Visibility = Visibility.Visible;
                
                // Update selected bot index (no immediate save - HB 4.3.4 pattern)
                CharacterSettings.Instance.SelectedBotIndex = cmbBotSelector.SelectedIndex;
            }
        }

        private void btnLoadProfile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Profile files (*.xml)|*.xml|All files (*.*)|*.*",
                Title = "Load Profile",
                InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Default Profiles")
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    ProfileManager.LoadNew(openFileDialog.FileName);
                    Logging.Write($"Profile loaded: {Path.GetFileName(openFileDialog.FileName)}");
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    System.Windows.MessageBox.Show(
                        $"Failed to load profile!\n\nError: {ex.Message}",
                        "CopilotBuddy - Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow
            {
                Owner = this
            };
            settingsWindow.ShowDialog();
        }

        private void btnBotConfig_Click(object sender, RoutedEventArgs e)
        {
            if (BotManager.Current == null)
            {
                Logging.Write("No bot selected.");
                return;
            }

            var configForm = BotManager.Current.ConfigurationForm;
            if (configForm == null)
            {
                Logging.Write("[{0}] has no configuration window.", BotManager.Current.Name);
                return;
            }

            if (configForm is System.Windows.Window window)
            {
                window.Owner = this;
                window.ShowDialog();
            }
            else if (configForm is System.Windows.Forms.Form winForm)
            {
                // WinForms support — external bots (LazyRaider etc.) use WinForms config dialogs
                winForm.ShowDialog();
            }
            else
            {
                Logging.Write("[{0}] ConfigurationForm type not supported: {1}",
                    BotManager.Current.Name, configForm.GetType().Name);
            }
        }

        private void btnClassConfig_Click(object sender, RoutedEventArgs e)
        {
            // Open class/routine config
            var routine = Styx.Logic.Combat.RoutineManager.Current;
            if (routine != null && routine.WantButton)
            {
                try
                {
                    routine.OnButtonPress();
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    System.Windows.MessageBox.Show($"Error opening routine config: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("No combat routine loaded or routine has no configuration.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnPlugins_Click(object sender, RoutedEventArgs e)
        {
            var pluginsWindow = new PluginsWindow
            {
                Owner = this
            };
            pluginsWindow.ShowDialog();
        }

        private void btnDevTools_Click(object sender, RoutedEventArgs e)
        {
            var devTools = new DeveloperToolsWindow();
            devTools.Owner = this;
            devTools.Show();
        }

        private void EnhancedMode_Checked(object sender, RoutedEventArgs e)
        {
            // Show all hidden buttons
            btnBotConfig.Visibility = Visibility.Visible;
            btnClassConfig.Visibility = Visibility.Visible;
            btnPlugins.Visibility = Visibility.Visible;
            btnDevTools.Visibility = Visibility.Visible;
            
            // Save setting
            UISettings.Instance.EnhancedMode = true;
            UISettings.Instance.Save();
        }

        private void EnhancedMode_Unchecked(object sender, RoutedEventArgs e)
        {
            // Hide optional buttons
            btnBotConfig.Visibility = Visibility.Hidden;
            btnClassConfig.Visibility = Visibility.Hidden;
            btnPlugins.Visibility = Visibility.Hidden;
            btnDevTools.Visibility = Visibility.Hidden;
            
            // Save setting
            UISettings.Instance.EnhancedMode = false;
            UISettings.Instance.Save();
        }

        #endregion

        #region Settings Management (HB 5.4.8 Pattern)

        /// <summary>
        /// Loads all settings after game attachment.
        /// Creates CharacterSettings instance (per-realm/per-character folder).
        /// Pattern from HB 5.4.8.
        /// </summary>
        private static void LoadSettings()
        {
            if (ObjectManager.Wow == null || !StyxWoW.IsInGame || StyxWoW.Me == null)
                return;

            try
            {
                // Create CharacterSettings instance for the current character
                // Pattern from HB 5.4.8 smethod_0() — called after game attachment
                CharacterSettings.Initialize();

                // Load other per-character settings
                UISettings.Instance.Load();
                StyxSettings.Instance.Load();
                LevelbotSettings.Instance.Load();
                
                Logging.WriteDebug("[Settings] Loaded settings for character: {0}", StyxWoW.Me?.Name ?? "Unknown");
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
            }
        }

        /// <summary>
        /// Saves all settings. Called at app close and before bot start.
        /// Pattern from HB 4.3.4.
        /// </summary>
        private static void SaveSettings()
        {
            try
            {
                if (ObjectManager.Wow == null || !ObjectManager.IsInGame)
                    return;

                UISettings.Instance.Save();
                CharacterSettings.Instance.Save();
                StyxSettings.Instance.Save();
                LevelbotSettings.Instance.Save();
                
                Logging.WriteDebug("[Settings] Saved all settings");
            }
            catch (Exception ex)
            {
                Logging.WriteDebug("[Settings] Error saving settings");
                Logging.WriteException(ex);
            }
        }

        #endregion
    }
}
