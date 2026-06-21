using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Styx;
using Styx.WoWInternals.WoWObjects;
using Templar.GUI.Tabs;
using Templar.Helpers;
using Templar.Properties;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;

namespace Templar.GUI {
    public partial class TemplarGUI : Form {
        // ===========================================================
        // Constants
        // ===========================================================

        private const int FontSize = 18;

        // ===========================================================
        // Fields
        // ===========================================================

        private Color _foreColor;
        private Color _backColor;
        readonly ToolTip _tooltip = new ToolTip();

        // ===========================================================
        // Constructors
        // ===========================================================

        public TemplarGUI() {
            InitializeComponent();

            InitializeFormData();
        }

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

        private static bool IsViable(WoWObject o) {
            return (o != null) && o.IsValid;
        }

        private void TemplarGUI_Load(object sender, EventArgs e) {
            WindowSettings.Restore(Settings.Default.CustomWindowSettings, this);

            SetUpToolTips();

            #region General tab
            checkboxIgnoreEliteMobs.Checked = GeneralSettings.Instance.IgnoreElites;

            // Groupbox: Looting
            checkboxLootMobs.Checked = GeneralSettings.Instance.LootMobs;
            textboxTimeBetweenLoots.Text = GeneralSettings.Instance.TimeBetweenLoots.ToString(CultureInfo.InvariantCulture);

            // Groupbox: Skinning
            checkboxSkinMobs.Checked = GeneralSettings.Instance.SkinMobs;
            checkboxNinjaSkin.Checked = GeneralSettings.Instance.NinjaSkin;
            textboxTimeBetweenSkins.Text = GeneralSettings.Instance.TimeBetweenSkins.ToString(CultureInfo.InvariantCulture);

            // Groupbox: Multi Pull
            checkboxMultiPull.Checked = GeneralSettings.Instance.MultiPull;
            textboxMultiPullHpThresh.Text = GeneralSettings.Instance.HealthPercentThreshold.ToString(CultureInfo.InvariantCulture);
            textboxMultiPullMobThresh.Text = GeneralSettings.Instance.MobThreshold.ToString(CultureInfo.InvariantCulture);

            textboxStartLocationMaxDist.Text = GeneralSettings.Instance.StartingLocationMaxDistance.ToString(CultureInfo.InvariantCulture);

            textboxBagSlotsRemaining.Text = GeneralSettings.Instance.MinimumFreeBagSlots.ToString(CultureInfo.InvariantCulture);
            #endregion

            #region Whitelist tab
            datagridviewWhitelist.DataSource = WhitelistSettings.Instance.WhitelistedUnits;
            datagridviewWhitelist.Columns.Clear();

            var whitelistEntryColumn = new DataGridViewTextBoxColumn {
                DataPropertyName = "Entry",
                HeaderText = "Entry",
                Name = "Entry",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                //Width = 90, //Enable this if you change Fill to None
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            var whitelistNameColumn = new DataGridViewTextBoxColumn {
                DataPropertyName = "Name",
                HeaderText = "Name",
                Name = "Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                //Width = 375, //Enable this if you change Fill to None
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            datagridviewWhitelist.RowTemplate.Height = 20;
            datagridviewWhitelist.AutoGenerateColumns = false;
            datagridviewWhitelist.Columns.AddRange(new DataGridViewColumn[] { whitelistEntryColumn, whitelistNameColumn });

            checkboxAttackOnlyWhitelist.Checked = WhitelistSettings.Instance.AttackWhitelistOnly;
            #endregion

            #region Blacklist Tab
            datagridviewBlacklist.DataSource = BlacklistSettings.Instance.BlacklistedUnits;
            datagridviewBlacklist.Columns.Clear();

            var blacklistEntryColumn = new DataGridViewTextBoxColumn {
                DataPropertyName = "Entry",
                HeaderText = "Entry",
                Name = "Entry",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                //Width = 90, //Enable this if you change Fill to None
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            var blacklistNameColumn = new DataGridViewTextBoxColumn {
                DataPropertyName = "Name",
                HeaderText = "Name",
                Name = "Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                //Width = 375, //Enable this if you change Fill to None
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            datagridviewBlacklist.RowTemplate.Height = 20;
            datagridviewBlacklist.AutoGenerateColumns = false;
            datagridviewBlacklist.Columns.AddRange(new DataGridViewColumn[] { blacklistEntryColumn, blacklistNameColumn });
            #endregion

            #region Protected Items tab
            datagridviewProtectedItems.DataSource = ProtectedItemSettings.Instance.ProtectedItems;
            datagridviewProtectedItems.Columns.Clear();

            var protectedItemsEntryColumn = new DataGridViewTextBoxColumn {
                DataPropertyName = "Entry",
                HeaderText = "Entry",
                Name = "Entry",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                //Width = 90, //Enable this if you change Fill to None
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            var protectedItemsNameColumn = new DataGridViewTextBoxColumn {
                DataPropertyName = "Name",
                HeaderText = "Name",
                Name = "Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                //Width = 375, //Enable this if you change Fill to None
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            datagridviewProtectedItems.RowTemplate.Height = 20;
            datagridviewProtectedItems.AutoGenerateColumns = false;
            datagridviewProtectedItems.Columns.AddRange(new DataGridViewColumn[] { protectedItemsEntryColumn, protectedItemsNameColumn });
            #endregion

            #region Mail tab
            checkboxMail.Checked = MailSettings.Instance.Mail;

            checkboxMailWhites.Checked = MailSettings.Instance.MailWhites;
            checkboxMailGreens.Checked = MailSettings.Instance.MailGreens;
            checkboxMailBlues.Checked = MailSettings.Instance.MailBlues;
            checkboxMailPurples.Checked = MailSettings.Instance.MailPurples;

            textboxRecipient.Text = MailSettings.Instance.Recipient;
            #endregion

            #region Vendor tab
            checkboxVendor.Checked = VendorSettings.Instance.Vendor;

            checkboxSellGrays.Checked = VendorSettings.Instance.SellGrays;
            checkboxSellWhites.Checked = VendorSettings.Instance.SellWhites;
            checkboxSellGreens.Checked = VendorSettings.Instance.SellGreens;
            checkboxSellBlues.Checked = VendorSettings.Instance.SellBlues;
            checkboxSellPurples.Checked = VendorSettings.Instance.SellPurples;

            textboxRepairDurability.Text = VendorSettings.Instance.MinimumDurabilityForRepairs.ToString(CultureInfo.InvariantCulture);
            #endregion

            #region Disenchant tab
            checkboxDisenchant.Checked = DisenchantSettings.Instance.Disenchant;
            checkboxDisenchantOnlyNewItems.Checked = DisenchantSettings.Instance.DisenchantOnlyNewItems;
            checkboxDisenchantWeapons.Checked = DisenchantSettings.Instance.DisenchantWeapons;
            checkboxDisenchantGreens.Checked = DisenchantSettings.Instance.DisenchantGreens;
            checkboxDisenchantBlues.Checked = DisenchantSettings.Instance.DisenchantBlues;
            checkboxDisenchantPurples.Checked = DisenchantSettings.Instance.DisenchantPurples;
            textboxTimeBetweenDisenchants.Text = DisenchantSettings.Instance.TimeBetweenDisenchants.ToString(CultureInfo.InvariantCulture);
            #endregion
        }

        private void TemplarGUI_FormClosing(object sender, FormClosingEventArgs e) {
            Settings.Default.CustomWindowSettings = WindowSettings.Record(Settings.Default.CustomWindowSettings, this);

            Settings.Default.Save();
        }

        void InitializeFormData() {
            UpdateSettings();
        }

        void UpdateSettings() {
            Font = new Font("Microsoft Sans Serif", FontSize, FontStyle.Regular, GraphicsUnit.Pixel, 0);

            _foreColor = Color.DeepSkyBlue;
            ForeColor = _foreColor;

            _backColor = Color.Black;
            BackColor = _backColor;

            // Loop through all the tabpages for styling
            foreach(TabPage tabPage in tabcontrolBotConfig.TabPages) {
                foreach(var dataGridView in tabPage.Controls.OfType<DataGridView>()) {
                    dataGridView.AllowUserToAddRows = false;
                    dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.Black;
                    dataGridView.AlternatingRowsDefaultCellStyle.ForeColor = Color.DeepSkyBlue;
                    dataGridView.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
                    dataGridView.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

                    dataGridView.BackgroundColor = Color.FromArgb(25, 25, 25);

                    dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
                    dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.DeepSkyBlue;
                    dataGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
                    dataGridView.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
                    dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    dataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

                    dataGridView.DefaultCellStyle.BackColor = Color.Black;
                    dataGridView.DefaultCellStyle.ForeColor = Color.White;
                    dataGridView.DefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
                    dataGridView.DefaultCellStyle.SelectionForeColor = Color.White;
                    dataGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    dataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.False;

                    dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                    dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
                    dataGridView.EnableHeadersVisualStyles = false;
                    dataGridView.GridColor = Color.White;

                    dataGridView.RowHeadersDefaultCellStyle.BackColor = Color.Black;
                    dataGridView.RowHeadersDefaultCellStyle.ForeColor = Color.DeepSkyBlue;
                    dataGridView.RowHeadersDefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
                    dataGridView.RowHeadersDefaultCellStyle.SelectionForeColor = Color.White;
                    dataGridView.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    dataGridView.RowHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

                    dataGridView.RowHeadersVisible = false;

                    dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                }

                foreach(var tabControl in tabPage.Controls.OfType<TabControl>().SelectMany(tabpage => tabpage.Controls.OfType<TabPage>())) {
                    SetColor(tabControl);
                }

                // For every groupbox directly in a tabpage, stylize it
                foreach(var groupbox in tabPage.Controls.OfType<GroupBox>()) {
                    SetColor(groupbox);
                }

                // For every button that is directly in a tabpage, stylize it
                foreach(var button in tabPage.Controls.OfType<Button>()) {
                    SetColor(button, true);

                    button.FlatStyle = FlatStyle.Popup;
                }

                // For every button that is within a groupbox of the tabpage, stylize it
                foreach(var button in tabPage.Controls.OfType<GroupBox>().SelectMany(groupbox => groupbox.Controls.OfType<Button>())) {
                    SetColor(button, true);

                    button.FlatStyle = FlatStyle.Popup;
                }

                foreach(var checkbox in tabPage.Controls.OfType<GroupBox>().SelectMany(groupbox => groupbox.Controls.OfType<CheckBox>())) {
                    if(!checkbox.Enabled) {
                        checkbox.BackColor = Color.FromArgb(25, 25, 25);
                    }
                }

                // Otherwise, stylize whatever else is in the tabpage
                SetColor(tabPage);
            }
        }

        void SetColor(Control control, bool isButton = false) {
            if(!isButton) {
                control.ForeColor = _foreColor;
                control.BackColor = _backColor;
            } else {
                control.ForeColor = _backColor;
                control.BackColor = _foreColor;
            }
        }


        // Tooltip setup
        private void tooltip_Draw(object sender, DrawToolTipEventArgs e) {
            var font = new Font("Microsoft Sans Serif", 9f);

            _tooltip.BackColor = Color.Black;

            e.DrawBackground();
            e.DrawBorder();
            e.Graphics.DrawString(e.ToolTipText, font, Brushes.DeepSkyBlue, new PointF(0, 0));
        }

        private void SetUpToolTips() {
            _tooltip.OwnerDraw = true;
            _tooltip.Draw += tooltip_Draw;

            _tooltip.AutoPopDelay = 32000;
            _tooltip.InitialDelay = 1;
            _tooltip.ReshowDelay = 1;

            #region General tab
            _tooltip.SetToolTip(checkboxIgnoreEliteMobs, "Default: TRUE\n\nBy disabling this, you are allowing Templar to scan for elite mobs to kill.");

            // Groupbox: Looting
            _tooltip.SetToolTip(checkboxLootMobs, "Default: TRUE\n\nBy disabling this, you are not allowing Templar to scan for any potential corpses to loot. This is an excellent option for grinding reputation gained from mobs on kill for maximum efficiency.");
            _tooltip.SetToolTip(labelLootingTimeBetweenLoots, "Default: 3\n\nThe amount of time (in seconds) that Templar waits before attempting to loot the next mob.");

            // Groupbox: Skinning
            _tooltip.SetToolTip(checkboxSkinMobs, "Default: FALSE\n\nBy enabling this, you are allowing Templar to scan for mobs that are capable of being skinned that you have killed.");
            _tooltip.SetToolTip(checkboxNinjaSkin, "Default: FALSE\n\nBy enabling this, you are allowing Templar to skin mobs that you or other people have killed. This is a good option if someone in your farming area is leaving behind bodies that can be skinned.");
            _tooltip.SetToolTip(labelSkinningTimeBetweenSkins, "Default: 3\n\nThe amount of time (in seconds) that Templar waits before attempting to skin the next mob.");

            // Groupbox: Multi Pull
            _tooltip.SetToolTip(checkboxMultiPull, "Default: FALSE\n\nBy enabling this, you are allowing Templar to pull multiple mobs at once.");
            _tooltip.SetToolTip(labelMultiPullMobThreshold, "Default: 3\n\nThe amount of mobs that must be targetting you in order for Templar to stop pulling additional mobs.");
            _tooltip.SetToolTip(labelMultiPullHealthThreshold, "Default: 50\n\nThe percentage of health that your character must at least have in order to pull additional mobs.");

            _tooltip.SetToolTip(labelStartLocationMaxDistance, "Default: 1000\n\nThe maximum distance the character is allowed to be from the starting location before being forced to return back to it.");
            _tooltip.SetToolTip(labelMinimumBagSlots, "Default: 2\n\nThe minimum amount of slots in your inventory before Templar decides it's time to mail or vendor to provide space in the bags.");
            #endregion

            #region Whitelist tab
            // Groupbox: Add NPC
            _tooltip.SetToolTip(labelWhitelistID, "The ID (entry) of the NPC you wish to whitelist.");
            _tooltip.SetToolTip(labelWhitelistName, "The name of the NPC you wish to whitelist.");
            _tooltip.SetToolTip(buttonWhitelistAddUnit, "Adds the NPC that you have provided the ID and name for to the whitelist.");

            _tooltip.SetToolTip(buttonWhitelistAddTarget, "Adds your current target in WoW to the whitelist.");
            _tooltip.SetToolTip(buttonWhitelistRemove, "Removes the currently selected row from the whitelist.");

            _tooltip.SetToolTip(checkboxAttackOnlyWhitelist, "Default: FALSE\n\nBy enabling this, you are allowing Templar to only scan for the NPCs in your whitelist to kill.");
            #endregion

            #region Blacklist tab
            // Groupbox: Add NPC
            _tooltip.SetToolTip(labelBlacklistID, "The ID (entry) of the NPC you wish to blacklist.");
            _tooltip.SetToolTip(labelBlacklistName, "The name of the NPC you wish to blacklist.");
            _tooltip.SetToolTip(buttonBlacklistAddUnit, "Adds the NPC that you have provided the ID and name for to the blacklist.");

            _tooltip.SetToolTip(buttonBlacklistAddTarget, "Adds your current target in WoW to the blacklist.");
            _tooltip.SetToolTip(buttonBlacklistRemove, "Removes the currently selected row from the blacklist.");
            #endregion

            #region Protected Items tab
            // Groupbox: Add Item
            _tooltip.SetToolTip(labelProtectedItemsID, "The ID (entry) of the item you wish to protect from selling and mailing.");
            _tooltip.SetToolTip(labelProtectedItemsName, "The name of the item you wish to protect from selling and mailing.");
            _tooltip.SetToolTip(buttonProtectedItemAddItem, "Adds the item that you have provided the ID and name for to the protected items list");

            _tooltip.SetToolTip(buttonProtectedItemRemove, "Removes the currently selected row from the protected items list.");
            #endregion

            #region Mail tab
            _tooltip.SetToolTip(checkboxMail, "Default: FALSE\n\nBy enabling this, you are allowing Templar to mail items from your bags to a given recipient that isn't a protected item.");
            _tooltip.SetToolTip(checkboxMailWhites, "Default: TRUE\n\nBy disabling this, you are not allowing Templar to mail white items from your bags.");
            _tooltip.SetToolTip(checkboxMailGreens, "Default: TRUE\n\nBy disabling this, you are not allowing Templar to mail green items from your bags.");
            _tooltip.SetToolTip(checkboxMailBlues, "Default: TRUE\n\nBy disabling this, you are not allowing Templar to mail blue items from your bags.");
            _tooltip.SetToolTip(checkboxMailPurples, "Default: TRUE\n\nBy disabling this, you are not allowing Templar to mail purple items from your bags.");
            _tooltip.SetToolTip(textboxRecipient, "Default: empty\n\nBy entering a name, you are allowing Templar to mail to this recipient if you have enabled mailing. If your recipient is not on your realm, remember to include the realm as such: Gordonramsay-Mal'Ganis");
            #endregion

            #region Vendor tab
            _tooltip.SetToolTip(checkboxVendor, "Default: TRUE\n\nBy disabling this, you are not allowing Templar to sell items or repair at the vendor.");
            _tooltip.SetToolTip(checkboxSellGrays, "Default: TRUE\n\nBy disabling this, Templar will not sell your grays to the vendor.");
            _tooltip.SetToolTip(checkboxSellWhites, "Default: FALSE\n\nBy enabling this, Templar will sell all white items that aren't a protected item to the vendor.");
            _tooltip.SetToolTip(checkboxSellGreens, "Default: FALSE\n\nBy enabling this, Templar will sell all green items that aren't a protected item to the vendor.");
            _tooltip.SetToolTip(checkboxSellBlues, "Default: FALSE\n\nBy enabling this, Templar will sell all blue items that aren't a protected item to the vendor.");
            _tooltip.SetToolTip(checkboxSellPurples, "Default: FALSE\n\nBy enabling this, Templar will sell all purple items that aren't a protected item to the vendor.");
            _tooltip.SetToolTip(textboxRepairDurability, "Default: 25\n\nThe percentage of durability remaining on your equipment to be considered for a visit to a repair vendor.");
            #endregion

            #region Disenchant tab
            _tooltip.SetToolTip(checkboxDisenchant, "Default: FALSE\n\nBy enabling this, you are allowing Templar to disenchant items in your inventory (not your character's equipped items!).");
            _tooltip.SetToolTip(checkboxDisenchantOnlyNewItems, "Default: TRUE\n\nBy disabling this, you are allowing Templar to disenchant your entire inventory (not including equipped items). If enabled, Templar will only disenchant the new items that you pick up starting from this session.\n\nThis setting requires 'Enable Disenchanting' to be enabled to function.");
            _tooltip.SetToolTip(checkboxDisenchantWeapons, "Default: TRUE\n\nBy disabling this, you are only allowing Templar to disenchant items that aren't weapons.");
            _tooltip.SetToolTip(checkboxDisenchantGreens, "Default: FALSE\n\nBy enabling this, you are allowing Templar to disenchant green items.");
            _tooltip.SetToolTip(checkboxDisenchantBlues, "Default: FALSE\n\nBy enabling this, you are allowing Templar to disenchant blue items.");
            _tooltip.SetToolTip(checkboxDisenchantPurples, "Default: FALSE\n\nBy enabling this, you are allowing Templar to disenchant purple items.");
            _tooltip.SetToolTip(labelTimeBetweenDisenchants, "Default: 5\n\nThe amount of time (in seconds) that Templar waits before attempting to disenchant the next item.");
            #endregion
        }

        #region General tab
        private void checkboxIgnoreEliteMobs_CheckedChanged(object sender, EventArgs e) {
            GeneralSettings.Instance.IgnoreElites = checkboxIgnoreEliteMobs.Checked;
            GeneralSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Ignore Elites: {0}", GeneralSettings.Instance.IgnoreElites);
        }

        private void checkboxLootMobs_CheckedChanged(object sender, EventArgs e) {
            GeneralSettings.Instance.LootMobs = checkboxLootMobs.Checked;
            GeneralSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Loot Mobs: {0}", GeneralSettings.Instance.LootMobs);
        }

        private void textboxTimeBetweenLoots_TextChanged(object sender, EventArgs e) {
            double timeBetweenLoots;

            if(double.TryParse(textboxTimeBetweenLoots.Text, out timeBetweenLoots)) {
                GeneralSettings.Instance.TimeBetweenLoots = timeBetweenLoots;
                GeneralSettings.Save();
                CustomLog.Diagnostic("SETTING CHANGED! Time Between Loots: {0}", GeneralSettings.Instance.TimeBetweenLoots);
            }
        }

        private void checkboxSkinMobs_CheckedChanged(object sender, EventArgs e) {
            GeneralSettings.Instance.SkinMobs = checkboxSkinMobs.Checked;
            GeneralSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Skin Mobs: {0}", GeneralSettings.Instance.SkinMobs);
        }

        private void checkboxNinjaSkin_CheckedChanged(object sender, EventArgs e) {
            GeneralSettings.Instance.NinjaSkin = checkboxNinjaSkin.Checked;
            GeneralSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Ninja Skin: {0}", GeneralSettings.Instance.NinjaSkin);
        }

        private void textboxTimeBetweenSkins_TextChanged(object sender, EventArgs e) {
            double timeBetweenSkins;

            if(double.TryParse(textboxTimeBetweenLoots.Text, out timeBetweenSkins)) {
                GeneralSettings.Instance.TimeBetweenSkins = timeBetweenSkins;
                GeneralSettings.Save();
                CustomLog.Diagnostic("SETTING CHANGED! Time Between Skins: {0}", GeneralSettings.Instance.TimeBetweenSkins);
            }
        }

        private void checkboxMultiPull_CheckedChanged(object sender, EventArgs e) {
            GeneralSettings.Instance.MultiPull = checkboxMultiPull.Checked;
            GeneralSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Multi Pull: {0}", GeneralSettings.Instance.MultiPull);
        }

        private void textboxMultiPullMobThreshold_TextChanged(object sender, EventArgs e) {
            int threshold;

            if(int.TryParse(textboxMultiPullMobThresh.Text, out threshold)) {
                if(threshold > 0) {
                    GeneralSettings.Instance.MobThreshold = threshold;
                    GeneralSettings.Save();
                    CustomLog.Diagnostic("SETTING CHANGED! Multi Pull Mob Threshold: {0}", GeneralSettings.Instance.MobThreshold);
                } else {
                    MessageBox.Show("Mob threshold must be greater than 0.");
                }
            }
        }

        private void textboxMultiPullHpThresh_TextChanged(object sender, EventArgs e) {
            int threshold;

            if(int.TryParse(textboxMultiPullHpThresh.Text, out threshold)) {
                if(threshold >= 0 && threshold <= 100) {
                    GeneralSettings.Instance.HealthPercentThreshold = threshold;
                    GeneralSettings.Save();
                    CustomLog.Diagnostic("SETTING CHANGED! Multi Pull Health Threshold: {0}%", GeneralSettings.Instance.HealthPercentThreshold);
                } else {
                    MessageBox.Show("Health threshold percent must be between 0 and 100.");
                }
            }
        }

        private void textboxStartLocationMaxDist_TextChanged(object sender, EventArgs e) {
            int threshold;

            if(int.TryParse(textboxStartLocationMaxDist.Text, out threshold)) {
                if(threshold > 0 && threshold < 3000) {
                    GeneralSettings.Instance.StartingLocationMaxDistance = threshold;
                    GeneralSettings.Save();
                    CustomLog.Diagnostic("SETTING CHANGED! Start Location Max Distance: {0}", GeneralSettings.Instance.StartingLocationMaxDistance);
                } else {
                    MessageBox.Show("Max distance threshold must be greater than 0 and less than 3000 yards.");
                }
            }
        }

        private void textboxBagSlotsRemaining_TextChanged(object sender, EventArgs e) {
            int threshold;

            if(int.TryParse(textboxBagSlotsRemaining.Text, out threshold)) {
                if(threshold >= 0) {
                    GeneralSettings.Instance.MinimumFreeBagSlots = threshold;
                    GeneralSettings.Save();
                    CustomLog.Diagnostic("SETTING CHANGED! Minimum Bag Slots: {0}", GeneralSettings.Instance.MinimumFreeBagSlots);
                } else {
                    MessageBox.Show("Bag slots remaining threshold must be greater than or equal to 0.");
                }
            }
        }
        #endregion

        #region Whitelist tab
        private void buttonWhitelistAddTarget_Click(object sender, EventArgs e) {
            var target = StyxWoW.Me.CurrentTarget;
            if(IsViable(target) && !target.IsPlayer) {
                var whitelistInfo = new WhitelistEntry { Entry = target.Entry, Name = target.Name };

                var oldWhiteList = WhitelistSettings.Instance.WhitelistedUnits.FirstOrDefault(b => b.Entry == whitelistInfo.Entry);

                if(oldWhiteList != null) {
                    MessageBox.Show(this, "This unit is already whitelisted.", "Already listed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                WhitelistSettings.Instance.WhitelistedUnits.Add(whitelistInfo);
                WhitelistSettings.Save();
                CustomLog.Diagnostic("SETTING CHANGED! New whitelisted target! Name: {0}, Entry: {1}", whitelistInfo.Name, whitelistInfo.Entry);
            } else {
                MessageBox.Show(this, "You have no target, or the target you have selected isn't valid!", "No target", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonWhitelistRemove_Click(object sender, EventArgs e) {
            foreach(DataGridViewRow selectedRow in datagridviewWhitelist.SelectedRows) {
                WhitelistSettings.Instance.WhitelistedUnits.RemoveAt(selectedRow.Index);
            }

            WhitelistSettings.Save();
        }

        private void buttonWhitelistAddUnit_Click(object sender, EventArgs e) {
            if(string.IsNullOrEmpty(textboxWhitelistID.Text)) {
                MessageBox.Show(this, "You need to enter an ID", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var whitelistInfo = new WhitelistEntry { Entry = uint.Parse(textboxWhitelistID.Text), Name = textboxWhitelistName.Text };
            var oldWhiteList = WhitelistSettings.Instance.WhitelistedUnits.FirstOrDefault(b => b.Entry == whitelistInfo.Entry);
            if(oldWhiteList != null) {
                MessageBox.Show(this, "This unit is already whitelisted.", "Already listed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            textboxWhitelistID.Clear();
            textboxWhitelistName.Clear();
            WhitelistSettings.Instance.WhitelistedUnits.Add(whitelistInfo);
            WhitelistSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! New whitelisted unit! Name: {0}, Entry: {1}", whitelistInfo.Name, whitelistInfo.Entry);
        }

        private void checkboxAttackOnlyWhitelist_CheckedChanged(object sender, EventArgs e) {
            WhitelistSettings.Instance.AttackWhitelistOnly = checkboxAttackOnlyWhitelist.Checked;
            WhitelistSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Attack Only Whitelist: {0}", WhitelistSettings.Instance.AttackWhitelistOnly);
        }
        #endregion

        #region Blacklist tab
        private void buttonBlacklistAddTarget_Click(object sender, EventArgs e) {
            var target = StyxWoW.Me.CurrentTarget;
            if(IsViable(target) && !target.IsPlayer) {
                var blacklistInfo = new BlacklistEntry { Entry = target.Entry, Name = target.Name };

                var oldBlackList = BlacklistSettings.Instance.BlacklistedUnits.FirstOrDefault(b => b.Entry == blacklistInfo.Entry);

                if(oldBlackList != null) {
                    MessageBox.Show(this, "This unit is already blacklisted.", "Already listed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                BlacklistSettings.Instance.BlacklistedUnits.Add(blacklistInfo);
                BlacklistSettings.Save();
                CustomLog.Diagnostic("SETTING CHANGED! New blacklisted target! Name: {0}, Entry: {1}", blacklistInfo.Name, blacklistInfo.Entry);
            } else {
                MessageBox.Show(this, "You have no target, or the target you have selected isn't valid!",
                "No target", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void buttonBlacklistRemove_Click(object sender, EventArgs e) {
            foreach(DataGridViewRow selectedRow in datagridviewBlacklist.SelectedRows) {
                BlacklistSettings.Instance.BlacklistedUnits.RemoveAt(selectedRow.Index);
            }

            BlacklistSettings.Save();
        }

        private void buttonBlacklistAddUnit_Click(object sender, EventArgs e) {
            if(string.IsNullOrEmpty(textboxBlacklistID.Text)) {
                MessageBox.Show(this, "You need to enter an ID", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var blacklistInfo = new BlacklistEntry { Entry = uint.Parse(textboxBlacklistID.Text), Name = textboxBlacklistName.Text };
            var oldBlackList = BlacklistSettings.Instance.BlacklistedUnits.FirstOrDefault(b => b.Entry == blacklistInfo.Entry);
            if(oldBlackList != null) {
                MessageBox.Show(this, "This unit is already blacklisted.", "Already listed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            textboxBlacklistID.Clear();
            textboxBlacklistName.Clear();
            BlacklistSettings.Instance.BlacklistedUnits.Add(blacklistInfo);
            BlacklistSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! New blacklisted unit! Name: {0}, Entry: {1}", blacklistInfo.Name, blacklistInfo.Entry);
        }
        #endregion

        #region Protected Items tab
        private void buttonProtectedItemRemove_Click(object sender, EventArgs e) {
            foreach(DataGridViewRow selectedRow in datagridviewProtectedItems.SelectedRows) {
                ProtectedItemSettings.Instance.ProtectedItems.RemoveAt(selectedRow.Index);
            }

            ProtectedItemSettings.Save();
        }

        private void buttonProtectedItemAddItem_Click(object sender, EventArgs e) {
            if(string.IsNullOrEmpty(buttonProtectedItemAddItem.Text)) {
                MessageBox.Show(this, "You need to enter an ID", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var protectedItemInfo = new ProtectedItemEntry { Entry = uint.Parse(textboxProtectedItemID.Text), Name = textboxProtectedItemName.Text };
            var oldBlackList = ProtectedItemSettings.Instance.ProtectedItems.FirstOrDefault(b => b.Entry == protectedItemInfo.Entry);
            if(oldBlackList != null) {
                MessageBox.Show(this, "This item is already a protected item.", "Already listed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            textboxProtectedItemID.Clear();
            textboxProtectedItemName.Clear();
            ProtectedItemSettings.Instance.ProtectedItems.Add(protectedItemInfo);
            ProtectedItemSettings.Save();
        }
        #endregion

        #region Mail tab
        private void checkboxMail_CheckedChanged(object sender, EventArgs e) {
            MailSettings.Instance.Mail = checkboxMail.Checked;
            MailSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Mailing: {0}", MailSettings.Instance.Mail);
        }

        private void checkboxMailWhites_CheckedChanged(object sender, EventArgs e) {
            MailSettings.Instance.MailWhites = checkboxMailWhites.Checked;
            MailSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Mail Whites: {0}", MailSettings.Instance.MailWhites);
        }

        private void checkboxMailGreens_CheckedChanged(object sender, EventArgs e) {
            MailSettings.Instance.MailGreens = checkboxMailGreens.Checked;
            MailSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Mail Greens: {0}", MailSettings.Instance.MailGreens);
        }

        private void checkboxMailBlues_CheckedChanged(object sender, EventArgs e) {
            MailSettings.Instance.MailBlues = checkboxMailBlues.Checked;
            MailSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Mail Blues: {0}", MailSettings.Instance.MailBlues);
        }

        private void checkboxMailPurples_CheckedChanged(object sender, EventArgs e) {
            MailSettings.Instance.MailPurples = checkboxMailPurples.Checked;
            MailSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Mail Purples: {0}", MailSettings.Instance.MailPurples);
        }

        private void textboxRecipient_TextChanged(object sender, EventArgs e) {
            MailSettings.Instance.Recipient = textboxRecipient.Text.Trim();
            MailSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Recipient: {0}", MailSettings.Instance.Recipient == "" ? "empty" : "contains name");
        }
        #endregion

        #region Vendor tab
        private void checkboxVendor_CheckedChanged(object sender, EventArgs e) {
            VendorSettings.Instance.Vendor = checkboxVendor.Checked;
            VendorSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Vendoring: {0}", VendorSettings.Instance.Vendor);
        }

        private void checkboxSellGrays_CheckedChanged(object sender, EventArgs e) {
            VendorSettings.Instance.SellGrays = checkboxSellGrays.Checked;
            VendorSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Sell Grays: {0}", VendorSettings.Instance.SellGrays);
        }

        private void checkboxSellWhites_CheckedChanged(object sender, EventArgs e) {
            VendorSettings.Instance.SellWhites = checkboxSellWhites.Checked;
            VendorSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Sell Whites: {0}", VendorSettings.Instance.SellWhites);
        }

        private void checkboxSellGreens_CheckedChanged(object sender, EventArgs e) {
            VendorSettings.Instance.SellGreens = checkboxSellGreens.Checked;
            VendorSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Sell Greens: {0}", VendorSettings.Instance.SellGreens);
        }

        private void checkboxSellBlues_CheckedChanged(object sender, EventArgs e) {
            VendorSettings.Instance.SellBlues = checkboxSellBlues.Checked;
            VendorSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Sell Blues: {0}", VendorSettings.Instance.SellBlues);
        }

        private void checkboxSellPurples_CheckedChanged(object sender, EventArgs e) {
            VendorSettings.Instance.SellPurples = checkboxSellPurples.Checked;
            VendorSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Sell Purples: {0}", VendorSettings.Instance.SellPurples);
        }

        private void textboxRepairDurability_TextChanged(object sender, EventArgs e) {
            int threshold;

            if(int.TryParse(textboxRepairDurability.Text, out threshold)) {
                if(threshold >= 0 && threshold < 100) {
                    VendorSettings.Instance.MinimumDurabilityForRepairs = threshold;
                    VendorSettings.Save();
                    CustomLog.Diagnostic("SETTING CHANGED! Repair at: {0}%", VendorSettings.Instance.MinimumDurabilityForRepairs);
                } else {
                    MessageBox.Show("Repair durability % threshold must be greater than or equal to 0 and less than 100.");
                }
            }
        }
        #endregion

        #region Disenchant tab
        private void checkboxDisenchant_CheckedChanged(object sender, EventArgs e) {
            DisenchantSettings.Instance.Disenchant = checkboxDisenchant.Checked;
            DisenchantSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Disenchanting: {0}", DisenchantSettings.Instance.Disenchant);
        }

        private void checkboxDisenchantOnlyNewItems_CheckedChanged(object sender, EventArgs e) {
            DisenchantSettings.Instance.DisenchantOnlyNewItems = checkboxDisenchantOnlyNewItems.Checked;
            DisenchantSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Disenchant Only New Items: {0}", DisenchantSettings.Instance.DisenchantOnlyNewItems);
        }

        private void checkboxDisenchantWeapons_CheckedChanged(object sender, EventArgs e) {
            DisenchantSettings.Instance.DisenchantWeapons = checkboxDisenchantWeapons.Checked;
            DisenchantSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Disenchant Weapons: {0}", DisenchantSettings.Instance.DisenchantWeapons);
        }

        private void checkboxDisenchantGreens_CheckedChanged(object sender, EventArgs e) {
            DisenchantSettings.Instance.DisenchantGreens = checkboxDisenchantGreens.Checked;
            DisenchantSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Disenchant Greens: {0}", DisenchantSettings.Instance.DisenchantGreens);
        }

        private void checkboxDisenchantBlues_CheckedChanged(object sender, EventArgs e) {
            DisenchantSettings.Instance.DisenchantBlues = checkboxDisenchantBlues.Checked;
            DisenchantSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Disenchant Blues: {0}", DisenchantSettings.Instance.DisenchantBlues);
        }

        private void checkboxDisenchantPurples_CheckedChanged(object sender, EventArgs e) {
            DisenchantSettings.Instance.DisenchantPurples = checkboxDisenchantPurples.Checked;
            DisenchantSettings.Save();
            CustomLog.Diagnostic("SETTING CHANGED! Disenchant Purples: {0}", DisenchantSettings.Instance.DisenchantPurples);
        }

        private void textboxTimeBetweenDisenchants_TextChanged(object sender, EventArgs e) {
            double timeBetweenDisenchants;

            if(double.TryParse(textboxTimeBetweenDisenchants.Text, out timeBetweenDisenchants)) {
                DisenchantSettings.Instance.TimeBetweenDisenchants = timeBetweenDisenchants;
                DisenchantSettings.Save();
                CustomLog.Diagnostic("SETTING CHANGED! Time Between Disenchants: {0}", DisenchantSettings.Instance.TimeBetweenDisenchants);
            }
        }
        #endregion
    }
}
