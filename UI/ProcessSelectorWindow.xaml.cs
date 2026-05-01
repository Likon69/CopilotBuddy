using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using GreenMagic;
using Styx;
using Styx.Offsets;
using Styx.WoWInternals;

namespace CopilotBuddy.UI
{
    /// <summary>
    /// Process selector dialog shown when multiple WoW instances are running.
    /// Ported from HB 4.3.4 ProcessSelectorWindow with 6.2.3 improvements.
    /// </summary>
    public partial class ProcessSelectorWindow : Window
    {
        #region Fields

        private readonly List<ProcessEntry> _processes = new List<ProcessEntry>();

        #endregion

        #region Constructor

        public ProcessSelectorWindow()
        {
            InitializeComponent();
            RefreshProcesses();
        }

        #endregion

        #region Properties

        /// <summary>The selected process entry, set when user clicks Select.</summary>
        public ProcessEntry? Entry { get; set; }

        #endregion

        #region Button Handlers

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshProcesses();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Release all mutexes on cancel
            foreach (var pe in _processes)
                pe.Mutex?.Close();
            _processes.Clear();
            DialogResult = false;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            Entry = cbProcesses.SelectedItem as ProcessEntry;
            // Release mutexes for non-selected entries before closing
            foreach (var pe in _processes)
            {
                if (pe != Entry)
                    pe.Mutex?.Close();
            }
            DialogResult = true;
        }

        #endregion

        #region Process Enumeration

        /// <summary>
        /// Enumerates all WoW.exe processes, reads character names from memory,
        /// filters out processes already claimed by another CopilotBuddy instance,
        /// and populates the combo box.
        /// Pattern from HB 4.3.4 ProcessSelectorWindow.RefreshProcesses().
        /// </summary>
        private void RefreshProcesses()
        {
            // Close previously held mutexes
            foreach (var pe in _processes)
                pe.Mutex?.Close();

            _processes.Clear();
            cbProcesses.Items.Clear();

            Process[] wowProcesses = Process.GetProcessesByName("Wow");
            if (wowProcesses.Length == 0)
                wowProcesses = Process.GetProcessesByName("WoW");

            for (int i = 0; i < wowProcesses.Length; i++)
            {
                try
                {
                    // HB 6.2.3: check HasExited before reading memory
                    if (wowProcesses[i].HasExited)
                        continue;

                    // Verify build matches 3.3.5a (12340)
                    int build = wowProcesses[i].MainModule?.FileVersionInfo.FilePrivatePart ?? 0;
                    // build == 0 means no version resource (custom/private server client)
                    if (build != ObjectManager.SupportedBuild && build != 0)
                        continue;

                    using var memory = new Memory(wowProcesses[i].Id);

                    // Check if player is logged in (InGame offset)
                    bool isLoggedIn = memory.Read<byte>(GlobalOffsets.InGame) != 0;
                    if (!isLoggedIn)
                        continue;

                    // Read character name
                    string characterName = memory.ReadString(GlobalOffsets.PlayerName);
                    if (string.IsNullOrEmpty(characterName))
                        characterName = "Unknown";

                    // Try to acquire the process mutex
                    Mutex mutex = ProcessMutex.Create(wowProcesses[i].Id, out bool createdNew);
                    if (!createdNew)
                    {
                        // Another CopilotBuddy already owns this process — skip
                        mutex.Close();
                        continue;
                    }

                    // Available! Add to list (keep mutex held so another instance can't grab it)
                    var entry = new ProcessEntry(characterName, wowProcesses[i].Id, mutex);
                    _processes.Add(entry);
                    cbProcesses.Items.Add(entry);
                }
                catch
                {
                    // Process may have exited or access denied — skip
                }
            }

            if (cbProcesses.Items.Count > 0)
                cbProcesses.SelectedIndex = 0;
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// Represents a WoW process available for attachment.
        /// Ported from HB 4.3.4 ProcessSelectorWindow.ProcessEntry.
        /// </summary>
        public class ProcessEntry
        {
            private readonly string _characterName;

            public ProcessEntry(string characterName, int processId, Mutex mutex)
            {
                _characterName = characterName;
                ProcessId = processId;
                Mutex = mutex;
            }

            /// <summary>The WoW process ID.</summary>
            public readonly int ProcessId;

            /// <summary>The mutex claiming ownership of this process.</summary>
            public readonly Mutex Mutex;

            public override string ToString()
            {
                return $"PID: {ProcessId} logged in on: {_characterName}";
            }
        }

        #endregion
    }
}
