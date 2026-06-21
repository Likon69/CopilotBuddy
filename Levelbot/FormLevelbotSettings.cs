using System;
using System.Windows.Forms;
using Styx;
using Styx.Helpers;
using Styx.Localization;

namespace Levelbot
{
    public class FormLevelbotSettings : Form
    {
        private readonly CheckBox _groundMountFarmingMode;
        private readonly Button _save;
        private readonly Button _saveAndClose;

        public FormLevelbotSettings()
        {
            Text = Globalization.Get("LvlTitle");
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            _groundMountFarmingMode = new CheckBox
            {
                Text = Globalization.Get("LvlGroundMountFarming"),
                AutoSize = true,
                Checked = false,
                Margin = new Padding(12, 12, 12, 6)
            };

            _save = new Button
            {
                Text = Globalization.Get("ButtonText_Save"),
                AutoSize = true,
                Anchor = AnchorStyles.Right
            };

            _saveAndClose = new Button
            {
                Text = Globalization.Get("ButtonText_SaveAndClose"),
                AutoSize = true,
                Anchor = AnchorStyles.Right
            };

            var buttons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(12, 6, 12, 12)
            };
            buttons.Controls.Add(_saveAndClose);
            buttons.Controls.Add(_save);

            var root = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 2,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            root.Controls.Add(_groundMountFarmingMode, 0, 0);
            root.Controls.Add(buttons, 0, 1);

            Controls.Add(root);

            Load += OnLoad;
            FormClosing += OnFormClosing;
            _save.Click += (_, __) => SaveAll();
            _saveAndClose.Click += (_, __) =>
            {
                SaveAll();
                Close();
            };
            _groundMountFarmingMode.CheckedChanged += (_, __) => LevelbotSettings.Instance.GroundMountFarmingMode = _groundMountFarmingMode.Checked;
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            StyxSettings.Instance.Load();
            LevelbotSettings.Instance.Load();
            _groundMountFarmingMode.Checked = LevelbotSettings.Instance.GroundMountFarmingMode;
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            SaveAll();
        }

        private static void SaveAll()
        {
            StyxSettings.Instance.Save();
            LevelbotSettings.Instance.Save();
        }
    }
}
