// Rewritten: WinForms → WPF (System.Windows.Window), dark theme XAML.

using System;
using System.Collections.Generic;
using System.Windows;
using Bots.BGBuddy.Resources;
using Styx.Logic;

namespace Bots.BGBuddy.Forms
{
    /// <summary>
    /// WPF configuration window for BGBuddy.
    /// Allows selecting battleground queues, pull distance, mount distance, and loot toggle.
    /// </summary>
    public partial class ConfigWindow : Window
    {
        // WotLK-only battleground entries (no Twin Peaks, no Battle for Gilneas)
        private static readonly List<string> _bgNames = new List<string>
        {
            BGBuddyResources.None,
            BGBuddyResources.RandomBattleground,
            BGBuddyResources.AlteracValley,
            BGBuddyResources.ArathiBasin,
            BGBuddyResources.EyeOfTheStorm,
            BGBuddyResources.IsleOfConquest,
            BGBuddyResources.StrandOfTheAncients,
            BGBuddyResources.WarsongGulch
        };

        public ConfigWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        /// <summary>
        /// Populates controls from current BGBuddySettings values.
        /// </summary>
        private void LoadSettings()
        {
            if (BGBuddySettings.Instance.FirstTime)
            {
                BGBuddySettings.Instance.Queue1 = BattlegroundType.RandomBattleground;
                BGBuddySettings.Instance.FirstTime = false;
                BGBuddySettings.Instance.Save();
            }

            foreach (string name in _bgNames)
            {
                cbQueue1.Items.Add(name);
                cbQueue2.Items.Add(name);
            }

            cbQueue1.SelectedItem = BgTypeToString(BGBuddySettings.Instance.Queue1);
            cbQueue2.SelectedItem = BgTypeToString(BGBuddySettings.Instance.Queue2);
            chkLootCorpses.IsChecked = BGBuddySettings.Instance.LootCorpses;
            txtPullDistance.Text = BGBuddySettings.Instance.PullDistance.ToString("F1");
            txtMountUpDistance.Text = BGBuddySettings.Instance.MountUpDistance.ToString("F1");
        }

        /// <summary>
        /// Saves current UI state back to BGBuddySettings.
        /// </summary>
        private void SaveSettings()
        {
            BGBuddySettings.Instance.Queue1 = BgTypeFromString(cbQueue1.SelectedItem?.ToString() ?? "");
            BGBuddySettings.Instance.Queue2 = BgTypeFromString(cbQueue2.SelectedItem?.ToString() ?? "");
            BGBuddySettings.Instance.LootCorpses = chkLootCorpses.IsChecked == true;

            if (double.TryParse(txtPullDistance.Text, out double pullDist))
                BGBuddySettings.Instance.PullDistance = pullDist;

            if (double.TryParse(txtMountUpDistance.Text, out double mountDist))
                BGBuddySettings.Instance.MountUpDistance = mountDist;

            BGBuddySettings.Instance.Save();
        }

        private void btnSaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Reload to discard any unsaved changes
            BGBuddySettings.Instance.Load();
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Live-save on any setting change so values are always persisted.
        /// </summary>
        private void OnSettingChanged(object sender, EventArgs e)
        {
            // Only save if selections are valid (window fully loaded)
            if (cbQueue1.SelectedItem != null && cbQueue2.SelectedItem != null)
                SaveSettings();
        }

        /// <summary>
        /// Converts a localized BG name string back to BattlegroundType enum.
        /// </summary>
        private static BattlegroundType BgTypeFromString(string name)
        {
            if (string.Equals(name, BGBuddyResources.None))
                return BattlegroundType.None;
            if (string.Equals(name, BGBuddyResources.RandomBattleground))
                return BattlegroundType.RandomBattleground;
            if (string.Equals(name, BGBuddyResources.AlteracValley))
                return BattlegroundType.AV;
            if (string.Equals(name, BGBuddyResources.ArathiBasin))
                return BattlegroundType.AB;
            if (string.Equals(name, BGBuddyResources.EyeOfTheStorm))
                return BattlegroundType.EotS;
            if (string.Equals(name, BGBuddyResources.IsleOfConquest))
                return BattlegroundType.IoC;
            if (string.Equals(name, BGBuddyResources.StrandOfTheAncients))
                return BattlegroundType.SotA;
            if (string.Equals(name, BGBuddyResources.WarsongGulch))
                return BattlegroundType.WSG;
            return BattlegroundType.None;
        }

        /// <summary>
        /// Converts a BattlegroundType enum to a localized display name.
        /// </summary>
        private static string BgTypeToString(BattlegroundType type)
        {
            return type switch
            {
                BattlegroundType.None => BGBuddyResources.None,
                BattlegroundType.AV => BGBuddyResources.AlteracValley,
                BattlegroundType.WSG => BGBuddyResources.WarsongGulch,
                BattlegroundType.AB => BGBuddyResources.ArathiBasin,
                BattlegroundType.EotS => BGBuddyResources.EyeOfTheStorm,
                BattlegroundType.SotA => BGBuddyResources.StrandOfTheAncients,
                BattlegroundType.IoC => BGBuddyResources.IsleOfConquest,
                BattlegroundType.RandomBattleground => BGBuddyResources.RandomBattleground,
                _ => BGBuddyResources.None
            };
        }
    }
}
