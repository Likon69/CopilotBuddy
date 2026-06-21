using Styx.CommonBot;

namespace Tyrael.Shared
{
    partial class TyraelInterface
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SettingsPanel = new System.Windows.Forms.Panel();
            this.TicksPerSecondTrackBar = new System.Windows.Forms.TrackBar();
            this.ClickToMove = new System.Windows.Forms.CheckBox();
            this.PauseKeyComboBox = new System.Windows.Forms.ComboBox();
            this.ModifierKeyComboBox = new System.Windows.Forms.ComboBox();
            this.PauseLabel = new System.Windows.Forms.Label();
            this.Plugins = new System.Windows.Forms.CheckBox();
            this.ContinuesHealingMode = new System.Windows.Forms.CheckBox();
            this.HardLock = new System.Windows.Forms.CheckBox();
            this.ChatOutput = new System.Windows.Forms.CheckBox();
            this.SaveandCloseButton = new System.Windows.Forms.Button();
            this.NamePanel = new System.Windows.Forms.Panel();
            this.RepLinkLabel = new System.Windows.Forms.LinkLabel();
            this.FAQLinkLabel = new System.Windows.Forms.LinkLabel();
            this.TyraelNameLabel = new System.Windows.Forms.Label();
            this.TpsLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AdditionalFeaturesLabel = new System.Windows.Forms.Label();
            this.SoftLock = new System.Windows.Forms.CheckBox();
            this.FrameLockFeaturesLabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.RaidWarningOutput = new System.Windows.Forms.CheckBox();
            this.LoggingLabel = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.QuickPerformanceButton = new System.Windows.Forms.Button();
            this.QuickConfigurationLabel = new System.Windows.Forms.Label();
            this.MaximumPerformanceButton = new System.Windows.Forms.Button();
            this.NormalPerformanceButton = new System.Windows.Forms.Button();
            this.CasualPerformanceButton = new System.Windows.Forms.Button();
            this.SettingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TicksPerSecondTrackBar)).BeginInit();
            this.NamePanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // SettingsPanel
            // 
            this.SettingsPanel.BackColor = System.Drawing.Color.White;
            this.SettingsPanel.Controls.Add(this.TicksPerSecondTrackBar);
            this.SettingsPanel.Location = new System.Drawing.Point(2, 45);
            this.SettingsPanel.Name = "SettingsPanel";
            this.SettingsPanel.Size = new System.Drawing.Size(761, 50);
            this.SettingsPanel.TabIndex = 0;
            // 
            // TicksPerSecondTrackBar
            // 
            this.TicksPerSecondTrackBar.BackColor = System.Drawing.Color.White;
            this.TicksPerSecondTrackBar.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.TicksPerSecondTrackBar.Location = new System.Drawing.Point(10, 2);
            this.TicksPerSecondTrackBar.Maximum = 100;
            this.TicksPerSecondTrackBar.Minimum = 10;
            this.TicksPerSecondTrackBar.Name = "TicksPerSecondTrackBar";
            this.TicksPerSecondTrackBar.Size = new System.Drawing.Size(742, 45);
            this.TicksPerSecondTrackBar.SmallChange = 10;
            this.TicksPerSecondTrackBar.TabIndex = 0;
            this.TicksPerSecondTrackBar.TickFrequency = 5;
            this.TicksPerSecondTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.TicksPerSecondTrackBar.Value = 30;
            this.TicksPerSecondTrackBar.Scroll += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            // 
            // ClickToMove
            // 
            this.ClickToMove.AutoSize = true;
            this.ClickToMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ClickToMove.ForeColor = System.Drawing.Color.DodgerBlue;
            this.ClickToMove.Location = new System.Drawing.Point(10, 105);
            this.ClickToMove.Name = "ClickToMove";
            this.ClickToMove.Size = new System.Drawing.Size(162, 21);
            this.ClickToMove.TabIndex = 8;
            this.ClickToMove.Text = "Enable Click to Move";
            this.ClickToMove.UseVisualStyleBackColor = false;
            this.ClickToMove.CheckedChanged += new System.EventHandler(this.ClickToMove_CheckedChanged);
            this.ClickToMove.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.ClickToMove.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ClickToMove_MouseMove);
            // 
            // PauseKeyComboBox
            // 
            this.PauseKeyComboBox.BackColor = System.Drawing.Color.White;
            this.PauseKeyComboBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PauseKeyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PauseKeyComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PauseKeyComboBox.ForeColor = System.Drawing.Color.DodgerBlue;
            this.PauseKeyComboBox.FormattingEnabled = true;
            this.PauseKeyComboBox.Location = new System.Drawing.Point(13, 59);
            this.PauseKeyComboBox.Name = "PauseKeyComboBox";
            this.PauseKeyComboBox.Size = new System.Drawing.Size(229, 25);
            this.PauseKeyComboBox.TabIndex = 7;
            this.PauseKeyComboBox.SelectedIndexChanged += new System.EventHandler(this.PauseKeyComboBox_SelectedIndexChanged);
            this.PauseKeyComboBox.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            // 
            // ModifierKeyComboBox
            // 
            this.ModifierKeyComboBox.BackColor = System.Drawing.Color.White;
            this.ModifierKeyComboBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ModifierKeyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ModifierKeyComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ModifierKeyComboBox.ForeColor = System.Drawing.Color.DodgerBlue;
            this.ModifierKeyComboBox.FormattingEnabled = true;
            this.ModifierKeyComboBox.Location = new System.Drawing.Point(13, 22);
            this.ModifierKeyComboBox.Name = "ModifierKeyComboBox";
            this.ModifierKeyComboBox.Size = new System.Drawing.Size(229, 25);
            this.ModifierKeyComboBox.TabIndex = 6;
            this.ModifierKeyComboBox.SelectedIndexChanged += new System.EventHandler(this.ModifierKeyComboBox_SelectedIndexChanged);
            this.ModifierKeyComboBox.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            // 
            // PauseLabel
            // 
            this.PauseLabel.AutoSize = true;
            this.PauseLabel.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PauseLabel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.PauseLabel.Location = new System.Drawing.Point(10, 3);
            this.PauseLabel.Name = "PauseLabel";
            this.PauseLabel.Size = new System.Drawing.Size(226, 16);
            this.PauseLabel.TabIndex = 5;
            this.PauseLabel.Text = "Select Pause Hotkey Combination";
            this.PauseLabel.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            // 
            // Plugins
            // 
            this.Plugins.AutoSize = true;
            this.Plugins.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Plugins.ForeColor = System.Drawing.Color.DodgerBlue;
            this.Plugins.Location = new System.Drawing.Point(10, 145);
            this.Plugins.Name = "Plugins";
            this.Plugins.Size = new System.Drawing.Size(217, 21);
            this.Plugins.TabIndex = 4;
            this.Plugins.Text = "Enable Plugins and AutoEquip";
            this.Plugins.UseVisualStyleBackColor = false;
            this.Plugins.CheckedChanged += new System.EventHandler(this.Plugins_CheckedChanged);
            this.Plugins.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.Plugins.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Plugins_MouseMove);
            // 
            // ContinuesHealingMode
            // 
            this.ContinuesHealingMode.AutoSize = true;
            this.ContinuesHealingMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ContinuesHealingMode.ForeColor = System.Drawing.Color.DodgerBlue;
            this.ContinuesHealingMode.Location = new System.Drawing.Point(10, 125);
            this.ContinuesHealingMode.Name = "ContinuesHealingMode";
            this.ContinuesHealingMode.Size = new System.Drawing.Size(240, 21);
            this.ContinuesHealingMode.TabIndex = 3;
            this.ContinuesHealingMode.Text = "Enable Continuous Healing Mode";
            this.ContinuesHealingMode.UseVisualStyleBackColor = false;
            this.ContinuesHealingMode.CheckedChanged += new System.EventHandler(this.ContinuesHealingMode_CheckedChanged);
            this.ContinuesHealingMode.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.ContinuesHealingMode.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ContinuesHealingMode_MouseMove);
            // 
            // HardLock
            // 
            this.HardLock.AutoSize = true;
            this.HardLock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.HardLock.ForeColor = System.Drawing.Color.DodgerBlue;
            this.HardLock.Location = new System.Drawing.Point(10, 22);
            this.HardLock.Name = "HardLock";
            this.HardLock.Size = new System.Drawing.Size(133, 21);
            this.HardLock.TabIndex = 2;
            this.HardLock.Text = "Enable HardLock";
            this.HardLock.UseVisualStyleBackColor = false;
            this.HardLock.CheckedChanged += new System.EventHandler(this.HardLock_CheckedChanged);
            this.HardLock.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.HardLock.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HardLock_MouseMove);
            // 
            // ChatOutput
            // 
            this.ChatOutput.AutoSize = true;
            this.ChatOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChatOutput.ForeColor = System.Drawing.Color.DodgerBlue;
            this.ChatOutput.Location = new System.Drawing.Point(13, 119);
            this.ChatOutput.Name = "ChatOutput";
            this.ChatOutput.Size = new System.Drawing.Size(155, 21);
            this.ChatOutput.TabIndex = 1;
            this.ChatOutput.Text = "Enable Chat Output";
            this.ChatOutput.UseVisualStyleBackColor = false;
            this.ChatOutput.CheckedChanged += new System.EventHandler(this.ChatOutput_CheckedChanged);
            this.ChatOutput.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.ChatOutput.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChatOutput_MouseMove);
            // 
            // SaveandCloseButton
            // 
            this.SaveandCloseButton.BackColor = System.Drawing.Color.DodgerBlue;
            this.SaveandCloseButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.SaveandCloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveandCloseButton.Font = new System.Drawing.Font("Century Gothic", 9.75F);
            this.SaveandCloseButton.Location = new System.Drawing.Point(620, 288);
            this.SaveandCloseButton.Name = "SaveandCloseButton";
            this.SaveandCloseButton.Size = new System.Drawing.Size(143, 23);
            this.SaveandCloseButton.TabIndex = 0;
            this.SaveandCloseButton.Text = "Save and Close";
            this.SaveandCloseButton.UseVisualStyleBackColor = true;
            this.SaveandCloseButton.Click += new System.EventHandler(this.SaveandCloseButton_Click);
            this.SaveandCloseButton.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.SaveandCloseButton.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SaveButton_MouseMove);
            // 
            // NamePanel
            // 
            this.NamePanel.BackColor = System.Drawing.Color.White;
            this.NamePanel.Controls.Add(this.RepLinkLabel);
            this.NamePanel.Controls.Add(this.FAQLinkLabel);
            this.NamePanel.Controls.Add(this.TyraelNameLabel);
            this.NamePanel.Location = new System.Drawing.Point(2, 2);
            this.NamePanel.Name = "NamePanel";
            this.NamePanel.Size = new System.Drawing.Size(761, 41);
            this.NamePanel.TabIndex = 0;
            this.NamePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GuiDragDrop);
            // 
            // RepLinkLabel
            // 
            this.RepLinkLabel.ActiveLinkColor = System.Drawing.Color.SteelBlue;
            this.RepLinkLabel.AutoSize = true;
            this.RepLinkLabel.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RepLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.RepLinkLabel.LinkColor = System.Drawing.Color.DodgerBlue;
            this.RepLinkLabel.Location = new System.Drawing.Point(617, 3);
            this.RepLinkLabel.Name = "RepLinkLabel";
            this.RepLinkLabel.Size = new System.Drawing.Size(136, 17);
            this.RepLinkLabel.TabIndex = 14;
            this.RepLinkLabel.TabStop = true;
            this.RepLinkLabel.Text = "Rep+ nomnomnom";
            this.RepLinkLabel.VisitedLinkColor = System.Drawing.Color.DodgerBlue;
            this.RepLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.RepLinkLabel_LinkClicked);
            // 
            // FAQLinkLabel
            // 
            this.FAQLinkLabel.ActiveLinkColor = System.Drawing.Color.SteelBlue;
            this.FAQLinkLabel.AutoSize = true;
            this.FAQLinkLabel.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FAQLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.FAQLinkLabel.LinkColor = System.Drawing.Color.DodgerBlue;
            this.FAQLinkLabel.Location = new System.Drawing.Point(680, 21);
            this.FAQLinkLabel.Name = "FAQLinkLabel";
            this.FAQLinkLabel.Size = new System.Drawing.Size(73, 17);
            this.FAQLinkLabel.TabIndex = 13;
            this.FAQLinkLabel.TabStop = true;
            this.FAQLinkLabel.Text = "Tyrael FAQ";
            this.FAQLinkLabel.VisitedLinkColor = System.Drawing.Color.DodgerBlue;
            this.FAQLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.FAQLinkLabel_LinkClicked);
            this.FAQLinkLabel.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.FAQLinkLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TyraelForumTopicLabel_MouseMove);
            // 
            // TyraelNameLabel
            // 
            this.TyraelNameLabel.AutoSize = true;
            this.TyraelNameLabel.Font = new System.Drawing.Font("Century Gothic", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TyraelNameLabel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.TyraelNameLabel.Location = new System.Drawing.Point(195, 1);
            this.TyraelNameLabel.Name = "TyraelNameLabel";
            this.TyraelNameLabel.Size = new System.Drawing.Size(385, 39);
            this.TyraelNameLabel.TabIndex = 0;
            this.TyraelNameLabel.Text = "Tyrael - Raiding BotBase";
            this.TyraelNameLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GuiDragDrop);
            // 
            // TpsLabel
            // 
            this.TpsLabel.AutoSize = true;
            this.TpsLabel.BackColor = System.Drawing.Color.Transparent;
            this.TpsLabel.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TpsLabel.ForeColor = System.Drawing.Color.White;
            this.TpsLabel.Location = new System.Drawing.Point(9, 291);
            this.TpsLabel.Name = "TpsLabel";
            this.TpsLabel.Size = new System.Drawing.Size(43, 17);
            this.TpsLabel.TabIndex = 1;
            this.TpsLabel.Text = "Tyrael";
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.AdditionalFeaturesLabel);
            this.panel1.Controls.Add(this.SoftLock);
            this.panel1.Controls.Add(this.FrameLockFeaturesLabel);
            this.panel1.Controls.Add(this.ClickToMove);
            this.panel1.Controls.Add(this.ContinuesHealingMode);
            this.panel1.Controls.Add(this.HardLock);
            this.panel1.Controls.Add(this.Plugins);
            this.panel1.Location = new System.Drawing.Point(2, 97);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(253, 188);
            this.panel1.TabIndex = 10;
            // 
            // AdditionalFeaturesLabel
            // 
            this.AdditionalFeaturesLabel.AutoSize = true;
            this.AdditionalFeaturesLabel.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AdditionalFeaturesLabel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.AdditionalFeaturesLabel.Location = new System.Drawing.Point(10, 66);
            this.AdditionalFeaturesLabel.Name = "AdditionalFeaturesLabel";
            this.AdditionalFeaturesLabel.Size = new System.Drawing.Size(135, 16);
            this.AdditionalFeaturesLabel.TabIndex = 11;
            this.AdditionalFeaturesLabel.Text = "Additional Features";
            // 
            // SoftLock
            // 
            this.SoftLock.AutoSize = true;
            this.SoftLock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SoftLock.ForeColor = System.Drawing.Color.DodgerBlue;
            this.SoftLock.Location = new System.Drawing.Point(10, 42);
            this.SoftLock.Name = "SoftLock";
            this.SoftLock.Size = new System.Drawing.Size(126, 21);
            this.SoftLock.TabIndex = 10;
            this.SoftLock.Text = "Enable SoftLock";
            this.SoftLock.UseVisualStyleBackColor = false;
            this.SoftLock.CheckedChanged += new System.EventHandler(this.SoftLock_CheckedChanged);
            this.SoftLock.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.SoftLock.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SoftLock_MouseMove);
            // 
            // FrameLockFeaturesLabel
            // 
            this.FrameLockFeaturesLabel.AutoSize = true;
            this.FrameLockFeaturesLabel.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FrameLockFeaturesLabel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.FrameLockFeaturesLabel.Location = new System.Drawing.Point(10, 3);
            this.FrameLockFeaturesLabel.Name = "FrameLockFeaturesLabel";
            this.FrameLockFeaturesLabel.Size = new System.Drawing.Size(134, 16);
            this.FrameLockFeaturesLabel.TabIndex = 8;
            this.FrameLockFeaturesLabel.Text = "Framelock Features";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.RaidWarningOutput);
            this.panel2.Controls.Add(this.LoggingLabel);
            this.panel2.Controls.Add(this.PauseKeyComboBox);
            this.panel2.Controls.Add(this.PauseLabel);
            this.panel2.Controls.Add(this.ModifierKeyComboBox);
            this.panel2.Controls.Add(this.ChatOutput);
            this.panel2.Location = new System.Drawing.Point(257, 97);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(252, 188);
            this.panel2.TabIndex = 11;
            // 
            // RaidWarningOutput
            // 
            this.RaidWarningOutput.AutoSize = true;
            this.RaidWarningOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RaidWarningOutput.ForeColor = System.Drawing.Color.DodgerBlue;
            this.RaidWarningOutput.Location = new System.Drawing.Point(13, 139);
            this.RaidWarningOutput.Name = "RaidWarningOutput";
            this.RaidWarningOutput.Size = new System.Drawing.Size(204, 21);
            this.RaidWarningOutput.TabIndex = 9;
            this.RaidWarningOutput.Text = "Enable Raidwarning Output";
            this.RaidWarningOutput.UseVisualStyleBackColor = false;
            this.RaidWarningOutput.CheckedChanged += new System.EventHandler(this.RaidWarningOutput_CheckedChanged);
            this.RaidWarningOutput.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.RaidWarningOutput.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RaidWarningOutput_MouseMove);
            // 
            // LoggingLabel
            // 
            this.LoggingLabel.AutoSize = true;
            this.LoggingLabel.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoggingLabel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.LoggingLabel.Location = new System.Drawing.Point(10, 97);
            this.LoggingLabel.Name = "LoggingLabel";
            this.LoggingLabel.Size = new System.Drawing.Size(119, 16);
            this.LoggingLabel.TabIndex = 8;
            this.LoggingLabel.Text = "Logging Features";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.QuickPerformanceButton);
            this.panel3.Controls.Add(this.QuickConfigurationLabel);
            this.panel3.Controls.Add(this.MaximumPerformanceButton);
            this.panel3.Controls.Add(this.NormalPerformanceButton);
            this.panel3.Controls.Add(this.CasualPerformanceButton);
            this.panel3.Location = new System.Drawing.Point(511, 97);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(252, 188);
            this.panel3.TabIndex = 12;
            // 
            // QuickPerformanceButton
            // 
            this.QuickPerformanceButton.BackColor = System.Drawing.Color.White;
            this.QuickPerformanceButton.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.QuickPerformanceButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.QuickPerformanceButton.Font = new System.Drawing.Font("Century Gothic", 9.75F);
            this.QuickPerformanceButton.ForeColor = System.Drawing.Color.DodgerBlue;
            this.QuickPerformanceButton.Location = new System.Drawing.Point(9, 87);
            this.QuickPerformanceButton.Name = "QuickPerformanceButton";
            this.QuickPerformanceButton.Size = new System.Drawing.Size(235, 26);
            this.QuickPerformanceButton.TabIndex = 16;
            this.QuickPerformanceButton.Text = "Quick Performance";
            this.QuickPerformanceButton.UseVisualStyleBackColor = false;
            this.QuickPerformanceButton.Click += new System.EventHandler(this.QuickPerformanceButton_Click);
            this.QuickPerformanceButton.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.QuickPerformanceButton.MouseMove += new System.Windows.Forms.MouseEventHandler(this.QuickPerformanceButton_MouseMove);
            // 
            // QuickConfigurationLabel
            // 
            this.QuickConfigurationLabel.AutoSize = true;
            this.QuickConfigurationLabel.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QuickConfigurationLabel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.QuickConfigurationLabel.Location = new System.Drawing.Point(10, 3);
            this.QuickConfigurationLabel.Name = "QuickConfigurationLabel";
            this.QuickConfigurationLabel.Size = new System.Drawing.Size(190, 16);
            this.QuickConfigurationLabel.TabIndex = 15;
            this.QuickConfigurationLabel.Text = "Quick Configuration Buttons";
            // 
            // MaximumPerformanceButton
            // 
            this.MaximumPerformanceButton.BackColor = System.Drawing.Color.White;
            this.MaximumPerformanceButton.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.MaximumPerformanceButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MaximumPerformanceButton.Font = new System.Drawing.Font("Century Gothic", 9.75F);
            this.MaximumPerformanceButton.ForeColor = System.Drawing.Color.DodgerBlue;
            this.MaximumPerformanceButton.Location = new System.Drawing.Point(9, 119);
            this.MaximumPerformanceButton.Name = "MaximumPerformanceButton";
            this.MaximumPerformanceButton.Size = new System.Drawing.Size(235, 26);
            this.MaximumPerformanceButton.TabIndex = 13;
            this.MaximumPerformanceButton.Text = "Maximum Performance";
            this.MaximumPerformanceButton.UseVisualStyleBackColor = false;
            this.MaximumPerformanceButton.Click += new System.EventHandler(this.MaximumPerformanceButton_Click);
            this.MaximumPerformanceButton.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.MaximumPerformanceButton.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MaximumPerformanceButton_MouseMove);
            // 
            // NormalPerformanceButton
            // 
            this.NormalPerformanceButton.BackColor = System.Drawing.Color.White;
            this.NormalPerformanceButton.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.NormalPerformanceButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.NormalPerformanceButton.Font = new System.Drawing.Font("Century Gothic", 9.75F);
            this.NormalPerformanceButton.ForeColor = System.Drawing.Color.DodgerBlue;
            this.NormalPerformanceButton.Location = new System.Drawing.Point(9, 55);
            this.NormalPerformanceButton.Name = "NormalPerformanceButton";
            this.NormalPerformanceButton.Size = new System.Drawing.Size(235, 26);
            this.NormalPerformanceButton.TabIndex = 14;
            this.NormalPerformanceButton.Text = "Normal Performance";
            this.NormalPerformanceButton.UseVisualStyleBackColor = false;
            this.NormalPerformanceButton.Click += new System.EventHandler(this.NormalPerformanceButton_Click);
            this.NormalPerformanceButton.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.NormalPerformanceButton.MouseMove += new System.Windows.Forms.MouseEventHandler(this.NormalPerformanceButton_MouseMove);
            // 
            // CasualPerformanceButton
            // 
            this.CasualPerformanceButton.BackColor = System.Drawing.Color.White;
            this.CasualPerformanceButton.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.CasualPerformanceButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CasualPerformanceButton.Font = new System.Drawing.Font("Century Gothic", 9.75F);
            this.CasualPerformanceButton.ForeColor = System.Drawing.Color.DodgerBlue;
            this.CasualPerformanceButton.Location = new System.Drawing.Point(9, 23);
            this.CasualPerformanceButton.Name = "CasualPerformanceButton";
            this.CasualPerformanceButton.Size = new System.Drawing.Size(235, 26);
            this.CasualPerformanceButton.TabIndex = 14;
            this.CasualPerformanceButton.Text = "Casual Performance";
            this.CasualPerformanceButton.UseVisualStyleBackColor = false;
            this.CasualPerformanceButton.Click += new System.EventHandler(this.CasualPerformanceButton_Click);
            this.CasualPerformanceButton.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.CasualPerformanceButton.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CasualPerformanceButton_MouseMove);
            // 
            // TyraelInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DodgerBlue;
            this.ClientSize = new System.Drawing.Size(765, 314);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.SaveandCloseButton);
            this.Controls.Add(this.TpsLabel);
            this.Controls.Add(this.NamePanel);
            this.Controls.Add(this.SettingsPanel);
            this.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TyraelInterface";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tyrael GUI";
            this.SettingsPanel.ResumeLayout(false);
            this.SettingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TicksPerSecondTrackBar)).EndInit();
            this.NamePanel.ResumeLayout(false);
            this.NamePanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel SettingsPanel;
        private System.Windows.Forms.Panel NamePanel;
        private System.Windows.Forms.Button SaveandCloseButton;
        private System.Windows.Forms.Label TpsLabel;
        private System.Windows.Forms.Label TyraelNameLabel;
        private System.Windows.Forms.TrackBar TicksPerSecondTrackBar;
        private System.Windows.Forms.CheckBox ChatOutput;
        private System.Windows.Forms.CheckBox Plugins;
        private System.Windows.Forms.CheckBox ContinuesHealingMode;
        private System.Windows.Forms.CheckBox HardLock;
        private System.Windows.Forms.Label PauseLabel;
        private System.Windows.Forms.ComboBox ModifierKeyComboBox;
        private System.Windows.Forms.ComboBox PauseKeyComboBox;
        private System.Windows.Forms.CheckBox ClickToMove;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button MaximumPerformanceButton;
        private System.Windows.Forms.Button CasualPerformanceButton;
        private System.Windows.Forms.Button NormalPerformanceButton;
        private System.Windows.Forms.Label QuickConfigurationLabel;
        private System.Windows.Forms.CheckBox SoftLock;
        private System.Windows.Forms.Label FrameLockFeaturesLabel;
        private System.Windows.Forms.Label AdditionalFeaturesLabel;
        private System.Windows.Forms.Button QuickPerformanceButton;
        private System.Windows.Forms.LinkLabel FAQLinkLabel;
        private System.Windows.Forms.LinkLabel RepLinkLabel;
        private System.Windows.Forms.Label LoggingLabel;
        private System.Windows.Forms.CheckBox RaidWarningOutput;

    }
}