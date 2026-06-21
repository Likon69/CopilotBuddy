using Styx.Common;
using Styx.CommonBot;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;

namespace Tyrael.Shared
{
    public partial class TyraelInterface : Form
    {
        #region Form Dragging API Support
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern bool ReleaseCapture();

        private void GuiDragDrop(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    ReleaseCapture();
                    SendMessage(Handle, 0xa1, 0x2, 0);
                    break;
            }
        }
        #endregion

        #region CBOItem
        public class CboItem
        {
            public readonly int E;
            private readonly string _s;

            public CboItem(int pe, string ps)
            {
                E = pe;
                _s = ps;
            }

            public override string ToString()
            {
                return _s;
            }
        }
        #endregion

        #region Combobox ENUM - Required for ENUM in comboboxes
        private static void SetComboBoxEnum(ComboBox cb, int e)
        {
            CboItem item;
            for (var i = 0; i < cb.Items.Count; i++)
            {
                item = (CboItem)cb.Items[i];
                if (item.E != e) continue;
                cb.SelectedIndex = i;
                return;
            }
            item = (CboItem)cb.Items[0];
            TyraelUtilities.DiagnosticLog("Dialog Error: Combobox {0} does not have enum({1}) in list, defaulting to enum({2})",
                          cb.Name, e, item.E);
            cb.SelectedIndex = 0;
        }
        private static int GetComboBoxEnum(ComboBox cb)
        {
            var item = (CboItem)cb.Items[cb.SelectedIndex];
            return item.E;
        }
        #endregion

        public TyraelInterface()
        {
            if (TreeRoot.IsRunning)
            {
                TyraelUtilities.PrintLog("We strongly advise to stop Honorbuddy before changing FrameLock settings (Hardlock and Softlock).");
            }

            InitializeComponent();
            StyxSettings.Instance.Load();
            TyraelSettings.Instance.Load();

            ModifierKeyComboBox.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Alt, "Alt - Mod"));
            ModifierKeyComboBox.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Control, "Ctrl - Mod"));
            ModifierKeyComboBox.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Shift, "Shift - Mod"));
            ModifierKeyComboBox.Items.Add(new CboItem((int)ModifierKeys, "Disable HK"));
            SetComboBoxEnum(ModifierKeyComboBox, (int)TyraelSettings.Instance.ModKeyChoice);

            PauseKeyComboBox.Items.Add(new CboItem((int)Keys.None, "Modifier Only"));
            PauseKeyComboBox.Items.Add(new CboItem((int)Keys.XButton1, "Mouse button 4"));
            PauseKeyComboBox.Items.Add(new CboItem((int)Keys.XButton2, "Mouse button 5"));
            PauseKeyComboBox.Items.Add(new CboItem((int)Keys.D1, "1 (no numpad)"));
            PauseKeyComboBox.Items.Add(new CboItem((int)Keys.D2, "2 (no numpad)"));
            PauseKeyComboBox.Items.Add(new CboItem((int)Keys.D3, "3 (no numpad)"));
            PauseKeyComboBox.Items.Add(new CboItem((int)Keys.Q, "Q"));
            PauseKeyComboBox.Items.Add(new CboItem((int)Keys.E, "E"));
            PauseKeyComboBox.Items.Add(new CboItem((int)Keys.R, "R"));
            PauseKeyComboBox.Items.Add(new CboItem((int)Keys.Z, "Z"));
            PauseKeyComboBox.Items.Add(new CboItem((int)Keys.X, "X"));
            PauseKeyComboBox.Items.Add(new CboItem((int)Keys.C, "C"));
            SetComboBoxEnum(PauseKeyComboBox, (int)TyraelSettings.Instance.PauseKeyChoice);

            ChatOutput.Checked = TyraelSettings.Instance.ChatOutput;
            ClickToMove.Checked = TyraelSettings.Instance.ClickToMove;
            HardLock.Checked = StyxSettings.Instance.UseFrameLock;
            ContinuesHealingMode.Checked = TyraelSettings.Instance.ContinuesHealingMode;
            Plugins.Checked = TyraelSettings.Instance.PluginPulsing;
            RaidWarningOutput.Checked = TyraelSettings.Instance.RaidWarningOutput;
            SoftLock.Checked = TyraelSettings.Instance.UseSoftLock;

            HonorbuddyTps = CharacterSettings.Instance.TicksPerSecond;
            TicksPerSecondTrackBar.Value = CharacterSettings.Instance.TicksPerSecond;
        }

        private void FAQLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.thebuddyforum.com/honorbuddy-forum/botbases/102004-bot-tyrael-raiding-botbase.html");
        }

        private void RepLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.thebuddyforum.com/reputation.php?do=addreputation&p=1002699");
        }

        private void ChatOutput_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables local Chat output in WoW.");
        }

        private void RaidWarningOutput_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables local RaidWarning output in WoW.");
        }

        private void ClickToMove_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables click to move in WoW.");
        }

        private void HardLock_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables HonorBuddy's HardLock (Framelock)");
        }

        private void SoftLock_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables Tyrael's SoftLock (Framelock)");
        }

        private void ContinuesHealingMode_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables healing mode - Casts out of combat!");
        }

        private void Plugins_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables plugins in Tyrael.");
        }

        private void CasualPerformanceButton_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("15 TPS - Hardlock disabled - Softlock disabled.");
        }

        private void NormalPerformanceButton_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("30 TPS - Hardlock disabled - Softlock enabled.");
        }

        private void QuickPerformanceButton_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("60 TPS - Hardlock disabled - Softlock enabled.");
        }

        private void MaximumPerformanceButton_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("90 TPS - Hardlock enabled - Softlock disabled.");
        }

        private void SaveButton_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Save the current settings and close the interface.");
        }

        private void TyraelForumTopicLabel_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Opens the Tyrael Topic in your default browser.");
        }

        private readonly int _var = CharacterSettings.Instance.TicksPerSecond;

        private int HonorbuddyTps
        {
            get
            {
                return (byte)TicksPerSecondTrackBar.Value;
            }
            set
            {
                Text = string.Format("Tyrael now ticks with {0} Ticks per Second.", (byte)value);
                TpsLabel.Text = Text = string.Format("{0} Ticks per Second.", (byte)value);
                TicksPerSecondTrackBar.Value = (byte)value;
            }
        }

        private bool TicksPerSecondChanges
        {
            get { return HonorbuddyTps != _var; }
        }

        private void TicksPerSecondTrackBar_Scroll(object sender, EventArgs e)
        {
            CharacterSettings.Instance.TicksPerSecond = (byte)TicksPerSecondTrackBar.Value;
            HonorbuddyTps = (byte)TicksPerSecondTrackBar.Value;
        }

        private void HardLock_CheckedChanged(object sender, EventArgs e)
        {
            StyxSettings.Instance.UseFrameLock = HardLock.Checked;
        }

        private void SoftLock_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.UseSoftLock = SoftLock.Checked;   
        }

        private void ChatOutput_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.ChatOutput = ChatOutput.Checked;
        }

        private void ClickToMove_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.ClickToMove = ClickToMove.Checked;
        }

        private void ContinuesHealingMode_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.ContinuesHealingMode = ContinuesHealingMode.Checked;
        }

        private void Plugins_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.PluginPulsing = Plugins.Checked;
        }

        private void RaidWarningOutput_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.RaidWarningOutput = RaidWarningOutput.Checked;
        }

        private void ModifierKeyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.ModKeyChoice = (ModifierKeys)GetComboBoxEnum(ModifierKeyComboBox);
        }

        private void PauseKeyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.PauseKeyChoice = (Keys)GetComboBoxEnum(PauseKeyComboBox);
        }

        private void CloseFormLogging()
        {
            TyraelUtilities.PrintLog("------------------------------------------");
            TyraelUtilities.PrintLog(TyraelSettings.Instance.ChatOutput
                ? "ChatOutput enabled!"
                : "ChatOutput disabled!");
            TyraelUtilities.PrintLog(TyraelSettings.Instance.ClickToMove
                ? "Click to Move enabled!"
                : "Click to Move disabled!");
            TyraelUtilities.PrintLog(StyxSettings.Instance.UseFrameLock
                ? "HardLock (Framelock) enabled!"
                : "HardLock (Framelock) disabled!");
            TyraelUtilities.PrintLog(TyraelSettings.Instance.UseSoftLock
                ? "SoftLock (Framelock) enabled!"
                : "SoftLock (Framelock) disabled!");
            TyraelUtilities.PrintLog(TyraelSettings.Instance.ContinuesHealingMode
                ? "Continues Healing mode enabled!"
                : "Continues Healing mode disabled!");
            TyraelUtilities.PrintLog(TyraelSettings.Instance.PluginPulsing
                ? "Plugins are enabled!"
                : "Plugins are disabled!");

            TyraelUtilities.PrintLog("{0} is the pause key, with {1} as modifier key.", TyraelSettings.Instance.PauseKeyChoice, TyraelSettings.Instance.ModKeyChoice);

            if (TicksPerSecondChanges)
            {
                TyraelUtilities.PrintLog("New TPS at {0} saved!", HonorbuddyTps);
            }

            TyraelUtilities.PrintLog("Interface saved and closed!");
            TyraelUtilities.PrintLog("------------------------------------------");
        }

        private void SaveSettings()
        {
            StyxSettings.Instance.Save();
            TyraelSettings.Instance.Save();

            TyraelUtilities.ClickToMove(1000);
            TyraelUtilities.ReRegisterHotkeys();
            TyraelUtilities.PrintInformation();
            Tyrael.InitializePlugins();

            TreeRoot.TicksPerSecond = CharacterSettings.Instance.TicksPerSecond;

            CloseFormLogging();

            Close();
        }

        private void SaveandCloseButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void CasualPerformanceButton_Click(object sender, EventArgs e)
        {
            TicksPerSecondTrackBar.Value = 15;

            CharacterSettings.Instance.TicksPerSecond = (byte)TicksPerSecondTrackBar.Value;
            StyxSettings.Instance.UseFrameLock = false;
            TyraelSettings.Instance.UseSoftLock = false;

            SaveSettings();
        }

        private void NormalPerformanceButton_Click(object sender, EventArgs e)
        {
            TicksPerSecondTrackBar.Value = 30;

            CharacterSettings.Instance.TicksPerSecond = (byte)TicksPerSecondTrackBar.Value;
            StyxSettings.Instance.UseFrameLock = false;
            TyraelSettings.Instance.UseSoftLock = true;

            SaveSettings();
        }

        private void QuickPerformanceButton_Click(object sender, EventArgs e)
        {
            TicksPerSecondTrackBar.Value = 60;

            CharacterSettings.Instance.TicksPerSecond = (byte)TicksPerSecondTrackBar.Value;
            StyxSettings.Instance.UseFrameLock = false;
            TyraelSettings.Instance.UseSoftLock = true;

            SaveSettings();
        }

        private void MaximumPerformanceButton_Click(object sender, EventArgs e)
        {
            TicksPerSecondTrackBar.Value = 90;

            CharacterSettings.Instance.TicksPerSecond = (byte)TicksPerSecondTrackBar.Value;
            StyxSettings.Instance.UseFrameLock = true;
            TyraelSettings.Instance.UseSoftLock = false;

            SaveSettings();
        }
    }
}
