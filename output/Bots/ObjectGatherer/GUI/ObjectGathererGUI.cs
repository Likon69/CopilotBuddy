using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ObjectGatherer.GUI.OutlookGrid;
using ObjectGatherer.GUI.Tabs;
using ObjectGatherer.Helpers;
using Styx;
using Styx.Helpers;
using DataRowComparer = ObjectGatherer.Helpers.DataRowComparer;
using Settings = ObjectGatherer.Properties.Settings;

namespace ObjectGatherer.GUI {
    public partial class ObjectGathererGUI : Form {

        // ===========================================================
        // Constants
        // ===========================================================

        private const int FontSize = 12;

        // ===========================================================
        // Fields
        // ===========================================================

        private ArrayList _defaultArrayList;

        private readonly DataSet _dataSet = new DataSet("ObjectGatherer");
        private DataTable _objectsDataTable = new DataTable("Objects");

        private DataGridViewCheckBoxColumn _enabledColumn;
        private DataGridViewTextBoxColumn _categoryColumn;
        private DataGridViewTextBoxColumn _entryColumn;
        private DataGridViewTextBoxColumn _nameColumn;
        private DataGridViewButtonColumn _deleteColumn;

        private Color _foreColor;
        private Color _backColor;
        private readonly ToolTip _tooltip = new ToolTip();

        // ===========================================================
        // Constructors
        // ===========================================================

        public ObjectGathererGUI() {
            InitializeComponent();

            InitializeFormData();
        }

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        public static string SettingsFilePath {
            get {
                return Path.Combine(AppContext.BaseDirectory, string.Format(@"Settings\{0}\{1}-{2}\{3}.xml", "ObjectGatherer", StyxWoW.Me.Name, StyxWoW.Me.RealmName, "ObjectSettings"));
            }
        }

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        public void CreateFolder() {
            var folderPath = Path.GetDirectoryName(SettingsFilePath);

            if(folderPath != null && !Directory.Exists(folderPath)) {
                Directory.CreateDirectory(folderPath);
            }
        }

        #region Load/Save XML
        public void LoadXml() {
            try {
                if(File.Exists(SettingsFilePath)) {
                    CreateColumns();
                    _dataSet.ReadXml(SettingsFilePath);
                    searchGridView.BindData(_dataSet, "Objects");
                    GroupAndSort();
                } else {
                    searchGridView.BindData(null, null);
                    DefaultSettings();
                    CreateFolder();
                    CreateColumns();
                    CreateRows();
                    UnboundGroupAndSort();
                }
            } catch(Exception e) {
                MessageBox.Show(e.Message, @"Load");
            }
        }

        public void SaveXml() {
            try {
                if(File.Exists(SettingsFilePath)) {
                    var ds = new DataSet("ObjectGatherer");
                    _objectsDataTable = GetEntriesDataTableFromDataGridView(searchGridView);
                    ds.Tables.Add(_objectsDataTable);
                    ds.WriteXml(SettingsFilePath);
                    return;
                }

                _objectsDataTable = GetEntriesDataTableFromDataGridView(searchGridView);
                _dataSet.Tables.Add(_objectsDataTable);
                _dataSet.WriteXml(File.OpenWrite(SettingsFilePath));
            } catch(Exception e) {
                MessageBox.Show(e.Message, @"Save");
            }
        }
        #endregion

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

        private void ObjectGathererGUI_Load(object sender, EventArgs e) {
            WindowSettings.Restore(Settings.Default.CustomWindowSettings, this);

            SetUpToolTips();
            LoadXml();
        }

        private void ObjectGathererGUI_FormClosing(object sender, FormClosingEventArgs e) {
            Settings.Default.CustomWindowSettings = WindowSettings.Record(Settings.Default.CustomWindowSettings, this);
            Settings.Default.Save();
            SaveXml();
        }


        private void InitializeFormData() {
            checkboxLootCorpses.Checked = GeneralSettings.Instance.LootCorpses;
            checkboxGatherHerbs.Checked = GeneralSettings.Instance.GatherHerbs;
            checkboxGatherOre.Checked = GeneralSettings.Instance.GatherOre;

            UpdateSettings();
        }

        #region DataGridView Setup, Sorting and Grouping
        private void CreateColumns() {
            _enabledColumn = new DataGridViewCheckBoxColumn {
                DataPropertyName = "Enabled",
                HeaderText = @"Enabled",
                Name = "Enabled",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.Programmatic
            };

            _categoryColumn = new DataGridViewTextBoxColumn {
                DataPropertyName = "Category",
                HeaderText = @"Category",
                Name = "Category",
                Visible = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.Programmatic
            };

            _entryColumn = new DataGridViewTextBoxColumn {
                DataPropertyName = "Entry",
                HeaderText = @"Entry",
                Name = "Entry",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.Programmatic
            };

            _nameColumn = new DataGridViewTextBoxColumn {
                DataPropertyName = "Name",
                HeaderText = @"Name",
                Name = "Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                SortMode = DataGridViewColumnSortMode.Programmatic
            };

            _deleteColumn = new DataGridViewButtonColumn {
                DataPropertyName = "Delete",
                Text = "Delete",
                HeaderText = @"Delete",
                Name = "Delete",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                UseColumnTextForButtonValue = true,
                SortMode = DataGridViewColumnSortMode.Programmatic
            };

            searchGridView.RowTemplate.Height = 23;
            searchGridView.AutoGenerateColumns = false;
            searchGridView.Columns.Add(_enabledColumn);
            searchGridView.Columns.Add(_categoryColumn);
            searchGridView.Columns.Add(_entryColumn);
            searchGridView.Columns.Add(_nameColumn);
            searchGridView.Columns.Add(_deleteColumn);
        }

        private void CreateRows() {
            // example of unbound items
            foreach(CustomAddRow obj in _defaultArrayList) {
                // notice that the outlookgrid only works with OutlookGridRow objects
                var row = new OutlookGridRow();
                row.CreateCells(searchGridView, true, obj.Category, obj.Entry, obj.Name, obj.Delete);
                searchGridView.Rows.Add(row);
            }
        }

        private void GroupAndSort() {
            searchGridView.GroupTemplate.Column = searchGridView.Columns[1];
            searchGridView.Sort(new DataRowComparer(1, ListSortDirection.Ascending));
            searchGridView.CollapseAll();
        }

        private void UnboundGroupAndSort() {
            var ds = new DataSet();
            _objectsDataTable = GetEntriesDataTableFromDataGridView(searchGridView);
            ds.Tables.Add(_objectsDataTable);
            searchGridView.BindData(ds, "Objects");

            searchGridView.GroupTemplate.Column = searchGridView.Columns[1];
            searchGridView.Sort(new DataRowComparer(1, ListSortDirection.Ascending));
            searchGridView.CollapseAll();
        }
        #endregion

        #region FormStyling and Tooltips
        private void UpdateSettings() {
            Font = new Font("Microsoft Sans Serif", FontSize, FontStyle.Regular, GraphicsUnit.Pixel, 0);

            _foreColor = Color.DeepSkyBlue;
            ForeColor = _foreColor;

            _backColor = Color.Black;
            BackColor = _backColor;

            foreach(var outlookGrid in Controls.OfType<OutlookGrid.OutlookGrid>()) {
                outlookGrid.AllowUserToAddRows = false;
                outlookGrid.AllowUserToDeleteRows = false;
                outlookGrid.AllowUserToOrderColumns = false;
                outlookGrid.AllowUserToResizeColumns = false;
                outlookGrid.AllowUserToResizeRows = false;

                outlookGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.Black;
                outlookGrid.AlternatingRowsDefaultCellStyle.ForeColor = Color.DeepSkyBlue;
                outlookGrid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
                outlookGrid.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

                outlookGrid.BackgroundColor = Color.FromArgb(25, 25, 25);

                outlookGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
                outlookGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.DeepSkyBlue;
                outlookGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
                outlookGrid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
                outlookGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                outlookGrid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

                outlookGrid.DefaultCellStyle.BackColor = Color.Black;
                outlookGrid.DefaultCellStyle.ForeColor = Color.White;
                outlookGrid.DefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
                outlookGrid.DefaultCellStyle.SelectionForeColor = Color.White;
                outlookGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                outlookGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;

                outlookGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                outlookGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
                outlookGrid.EnableHeadersVisualStyles = false;
                outlookGrid.GridColor = Color.White;

                outlookGrid.RowHeadersDefaultCellStyle.BackColor = Color.Black;
                outlookGrid.RowHeadersDefaultCellStyle.ForeColor = Color.DeepSkyBlue;
                outlookGrid.RowHeadersDefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
                outlookGrid.RowHeadersDefaultCellStyle.SelectionForeColor = Color.White;
                outlookGrid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                outlookGrid.RowHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

                outlookGrid.RowHeadersVisible = false;

                outlookGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }

            foreach(var tabControl in Controls.OfType<TabControl>().SelectMany(tabpage => tabpage.Controls.OfType<TabPage>())) {
                SetColor(tabControl);
            }

            // For every groupbox directly in the form, stylize it
            foreach(var groupbox in Controls.OfType<GroupBox>()) {
                groupbox.BackColor = Color.Black;
                groupbox.ForeColor = Color.DeepSkyBlue;
                //SetColor(groupbox);
            }

            // For every button that is directly in the form, stylize it
            foreach(var button in Controls.OfType<Button>()) {
                SetColor(button, true);

                button.FlatStyle = FlatStyle.Popup;
            }

            // For every label that is directly in the form, stylize it
            foreach(var label in Controls.OfType<Label>()) {
                SetColor(label);

                label.ForeColor = Color.White;
            }

            foreach(var button in Controls.OfType<TabControl>().SelectMany(groupbox => groupbox.Controls.OfType<Button>())) {
                SetColor(button, true);

                button.FlatStyle = FlatStyle.Popup;
            }

            // For every label that is within a groupbox of the form, stylize it
            foreach(var label in Controls.OfType<GroupBox>().SelectMany(groupbox => groupbox.Controls.OfType<Label>())) {
                SetColor(label);

                label.ForeColor = Color.White;
            }

            // For every button that is within a groupbox of the tabpage, stylize it
            foreach(var button in Controls.OfType<GroupBox>().SelectMany(groupbox => groupbox.Controls.OfType<Button>())) {
                SetColor(button, true);

                button.FlatStyle = FlatStyle.Popup;
            }

            // For every checkbox that is within a groupbox and is disabled, stylize it
            foreach(var checkbox in Controls.OfType<GroupBox>().SelectMany(groupbox => groupbox.Controls.OfType<CheckBox>())
                ) {
                SetColor(checkbox);

                if(!checkbox.Enabled) {
                    checkbox.BackColor = Color.FromArgb(25, 25, 25);
                }
            }
        }

        private void SetColor(Control control, bool isButton = false) {
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

            //_tooltip.SetToolTip(runtimeSessionLabel, "The amount of time the bot has been running.");
        }
        #endregion

        #region DefaultSettings (ArrayList)
        private void DefaultSettings() {
            try {
                _defaultArrayList = new ArrayList
                {
                    new CustomAddRow(_enabledColumn, "Ancient Guo-Lai Cache", 214388, "Ancient Guo-Lai Cache", _deleteColumn),

                    new CustomAddRow(_enabledColumn, "Dark Soil", 210565, "Dark Soil", _deleteColumn),

                    new CustomAddRow(_enabledColumn, "Gold Coins", 186633, "Gold Coins", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Gold Coins", 186634, "Gold Coins", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Gold Coins", 210894, "Gold Coins", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Gold Coins", 214458, "Gold Coins", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Gold Coins", 214985, "Gold Coins", _deleteColumn),

                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213363, "Wodin's Mantid Shanker", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213364, "Ancient Pandaren Mining Pick", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213366, "Ancient Pandaren Tea Pot (Grey trash worth 100G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213368, "Lucky Pandaren Coin (Grey trash worth 95G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213649, "Cache of Pilfered Goods", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213653, "Pandaren Fishing Spear", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213741, "Ancient Jinyu Staff", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213742, "Hammer of Ten Thunders", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213748, "Pandaren Ritual Stone (Grey trash worth 105G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213749, "Staff of the Hidden Master", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213750, "Saurok Stone Tablet (Grey trash worth 100G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213751, "Sprite's Cloth Chest", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213765, "Tablet of Ren Yun (Cooking Recipy)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213768, "Hozen Warrior Spear", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213771, "Statue of Xuen (Grey trash worth 100G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213782, "Terracotta Head (Grey trash worth 100G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213793, "Riktik's Tiny Chest (Grey trash worth 105G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213842, "Stash of Yaungol Weapons", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213844, "Amber Encased Moth (Grey trash worth 105G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213845, "The Hammer of Folly (Grey trash worth 100G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213956, "Fragment of Dread (Grey trash worth 90G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213959, "Hardened Sap of Kri'vess (Grey trash worth 110G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213960, "Yaungol Fire Carrier", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213962, "Wind-Reaver's Dagger of Quick Strikes", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213964, "Malik's Stalwart Spear", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213966, "Amber Encased Necklace", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213967, "Blade of the Prime", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213968, "Swarming Cleaver of Ka'roz", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213969, "Dissector's Staff of Mutilation", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213970, "Bloodsoaked Chitin Fragment", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 213972, "Blade of the Poisoned Mind", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 214340, "Boat-Building Instructions (Grey trash worth 10G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 214438, "Ancient Mogu Tablet (Grey trash worth 95G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Is Another Man's Treasure", 214439, "Barrel of Banana Infused Rum (Cooking Recipy and Rum)", _deleteColumn),

                    new CustomAddRow(_enabledColumn, "Eggs", 185915, "Netherwing Egg", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Eggs", 214945, "Onyx Egg", _deleteColumn),

                    new CustomAddRow(_enabledColumn, "Timber", 233604, "Small Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 233922, "Small Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234021, "Small Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234080, "Small Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234097, "Small Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234109, "Small Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234110, "Small Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234122, "Small Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234126, "Small Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234193, "Small Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234197, "Small Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 237727, "Small Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 233634, "Medium Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234000, "Medium Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234098, "Medium Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234111, "Medium Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234119, "Medium Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234123, "Medium Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234127, "Medium Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234194, "Medium Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234196, "Medium Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234198, "Medium Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234022, "Medium Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 233635, "Large Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234023, "Large Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234007, "Large Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234099, "Large Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234120, "Large Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234124, "Large Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234128, "Large Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234195, "Large Timber", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Timber", 234199, "Large Timber", _deleteColumn),

                    new CustomAddRow(_enabledColumn, "Treasure Chests", 176944, "Old Treasure Chest (Scholomance Instance)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 179697, "Arena Treasure Chest (STV Arena)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 203090, "Sunken Treaure Chest", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207472, "Silverbound Treasure Chest (Zone 1)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207473, "Silverbound Treasure Chest (Zone 2)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207474, "Silverbound Treasure Chest (Zone 3)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207475, "Silverbound Treasure Chest (Zone 4)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207476, "Silverbound Treasure Chest (Zone 5)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207477, "Silverbound Treasure Chest (Zone 6)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207478, "Silverbound Treasure Chest (Zone 7)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207479, "Silverbound Treasure Chest (Zone 8)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207480, "Silverbound Treasure Chest (Zone 9)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207484, "Sturdy Treasure Chest (Zone 1)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207485, "Sturdy Treasure Chest (Zone 2)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207486, "Sturdy Treasure Chest (Zone 3)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207487, "Sturdy Treasure Chest (Zone 4)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207488, "Sturdy Treasure Chest (Zone 5)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207489, "Sturdy Treasure Chest (Zone 6)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207492, "Sturdy Treasure Chest (Zone 7)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207493, "Sturdy Treasure Chest (Zone 8)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207494, "Sturdy Treasure Chest (Zone 9)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207495, "Sturdy Treasure Chest (Zone 10)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207496, "Dark Iron Treasure Chest (Zone 1)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207497, "Dark Iron Treasure Chest (Zone 2)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207498, "Dark Iron Treasure Chest (Zone 3)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207500, "Dark Iron Treasure Chest (Zone 4)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207507, "Dark Iron Treasure Chest (Zone 5)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207512, "Silken Treasure Chest (Zone 1)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207513, "Silken Treasure Chest (Zone 2)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207517, "Silken Treasure Chest (Zone 3)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207518, "Silken Treasure Chest (Zone 4)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207519, "Silken Treasure Chest (Zone 5)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207520, "Maplewood Treasure Chest (Zone 1)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207521, "Maplewood Treasure Chest (Zone 2)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207522, "Maplewood Treasure Chest (Zone 3)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207523, "Maplewood Treasure Chest (Zone 4)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207524, "Maplewood Treasure Chest (Zone 5)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207528, "Maplewood Treasure Chest (Zone 6)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207529, "Maplewood Treasure Chest (Zone 7)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207533, "Runestone Treasure Chest (Zone 1)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207534, "Runestone Treasure Chest (Zone 2)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207535, "Runestone Treasure Chest (Zone 3)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207540, "Runestone Treasure Chest (Zone 4)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 207542, "Runestone Treasure Chest (Zone 5)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 213362, "Ship's Locker (Contains ~ 96G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 213650, "Virmen Treasure Cache (Contains ~ 100G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 213769, "Hozen Treasure Cache (Contains ~ 100G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 213770, "Stolen Sprite Treasure (Contains ~ 105G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 213774, "Lost Adventurer's Belongings (Contains ~ 100G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 213961, "Abandoned Crate of Goods (Contains ~ 100G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 214325, "Forgotten Lockbox (Contains ~ 10G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 214407, "Mo-Mo's Treasure Chest (Contains ~ 9G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 214337, "Stash of Gems (few green uncut MoP gems and ~ 7G)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 214337, "Offering of Rememberance (Contains ~ 30G and debuff turns you grey)", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Treasure Chests", 218593, "Trove of the Thunder King", _deleteColumn),

                    new CustomAddRow(_enabledColumn, "Mysterious Camel Figurine", 50409, "Mysterious Camel Figurine", _deleteColumn),
                    new CustomAddRow(_enabledColumn, "Mysterious Camel Figurine", 50410, "Mysterious Camel Figurine", _deleteColumn),
                };
            } catch {
                MessageBox.Show(@"Error creating default settings.");
            }
        }
        #endregion

        private DataTable GetEntriesDataTableFromDataGridView(DataGridView dataGridView) {
            var dt = new DataTable("Objects");
            dt.Columns.Clear();
            dt.Columns.Add("Enabled");
            dt.Columns.Add("Category");
            dt.Columns.Add("Entry");
            dt.Columns.Add("Name");
            dt.Columns.Add("Button");

            var cellValues = new object[dataGridView.Columns.Count];
            foreach(DataGridViewRow row in dataGridView.Rows) {
                for(var i = 0; i < row.Cells.Count; i++) {
                    cellValues[i] = row.Cells[i].Value;
                }
                if(cellValues[2] != null) {
                    dt.Rows.Add(cellValues);
                }
            }

            return dt;
        }

        #region GUI Events
        private void addEntryButton_Click(object sender, EventArgs e) {
            if(string.IsNullOrEmpty(idTextBox.Text)) {
                MessageBox.Show(this, @"You need to enter an ID", @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if(string.IsNullOrEmpty(nameTextBox.Text)) {
                MessageBox.Show(this, @"You need to enter a Name", @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if(string.IsNullOrEmpty(categoryComboBox.Text)) {
                MessageBox.Show(this, @"You need to enter or select a Category", @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var cellValues = new object[searchGridView.Columns.Count];
            foreach(DataGridViewRow rows in searchGridView.Rows) {
                for(var i = 0; i < rows.Cells.Count; i++) {
                    cellValues[i] = rows.Cells[i].Value;
                }

                if(cellValues[2] == null || !Equals(cellValues[2], idTextBox.Text)) {
                    continue;
                }

                MessageBox.Show(this, @"This entry is already in the list.", @"Already listed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = new OutlookGridRow();
            row.CreateCells(searchGridView, true, categoryComboBox.Text, Convert.ToUInt32(idTextBox.Text), nameTextBox.Text, "Delete");
            searchGridView.Rows.Add(row);
            EntryList.Add(Convert.ToUInt32(idTextBox.Text));

            UnboundGroupAndSort();
            SaveXml();
        }

        private void categoryComboBox_DropDown(object sender, EventArgs e) {
            categoryComboBox.Items.Clear();
            var cellValues = new object[searchGridView.Columns.Count];
            foreach(DataGridViewRow row in searchGridView.Rows) {
                for(var i = 0; i < row.Cells.Count; i++) {
                    cellValues[i] = row.Cells[i].Value;
                }
                if(cellValues[1] != null && !categoryComboBox.Items.Contains(cellValues[1].ToString())) {
                    categoryComboBox.Items.Add(cellValues[1].ToString());
                }
            }
        }

        private void searchGridView_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            try {
                if(e.RowIndex == -1) {
                    return;
                }

                var senderGrid = (DataGridView)sender;

                if(senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0) {
                    var deleteConfirm = MessageBox.Show(this, @"Are you sure you want to delete selected Entry?",
                        @"Delete", MessageBoxButtons.YesNo);
                    if(deleteConfirm == DialogResult.Yes) {
                        var entry = searchGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
                        EntryList.Remove(Convert.ToUInt32(entry));
                        searchGridView.Rows.RemoveAt(e.RowIndex);
                        UnboundGroupAndSort();
                        SaveXml();
                    }
                }

                if(senderGrid.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn && e.RowIndex >= 0) {
                    var checkbox = (DataGridViewCheckBoxCell)searchGridView.CurrentCell;
                    var isChecked = (bool)checkbox.EditedFormattedValue;
                    searchGridView.Rows[e.RowIndex].Cells[0].Value = !isChecked;
                    searchGridView.EndEdit();
                    var entry = searchGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
                    if(isChecked) {
                        EntryList.Remove(Convert.ToUInt32(entry));
                    } else {
                        EntryList.Add(Convert.ToUInt32(entry));
                    }

                    SaveXml();
                }
                // ReSharper disable once EmptyGeneralCatchClause
            } catch(Exception) { }
        }

        private void idTextBox_TextChanged(object sender, EventArgs e) {
            int threshold;
            if(int.TryParse(idTextBox.Text, out threshold)) {
                if(threshold <= 0) {
                    MessageBox.Show(@"Entry must be greater than 0.");
                }
            } else {
                MessageBox.Show(@"Entry must be an integer.");
            }
        }

        private void searchGridView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e) {
            if(e.ColumnIndex == _enabledColumn.Index && e.RowIndex != -1) {
                searchGridView.EndEdit();
            }
        }

        private void searchGridView_DataError(object sender, DataGridViewDataErrorEventArgs e) {
            //Empty to hide error.
        }
        #endregion

        private void checkboxLootCorpses_CheckedChanged(object sender, EventArgs e) {
            GeneralSettings.Instance.LootCorpses = checkboxLootCorpses.Checked;
            GeneralSettings.Save();
        }

        private void checkboxGatherHerbs_CheckedChanged(object sender, EventArgs e) {
            GeneralSettings.Instance.GatherHerbs = checkboxGatherHerbs.Checked;
            GeneralSettings.Save();
        }

        private void checkboxGatherOre_CheckedChanged(object sender, EventArgs e) {
            GeneralSettings.Instance.GatherOre = checkboxGatherOre.Checked;
            GeneralSettings.Save();
        }
    }
}
