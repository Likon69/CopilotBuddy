namespace Templar.GUI {
    partial class TemplarGUI {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tabcontrolBotConfig = new System.Windows.Forms.TabControl();
            this.tabpageGeneral = new System.Windows.Forms.TabPage();
            this.textboxBagSlotsRemaining = new System.Windows.Forms.TextBox();
            this.labelMinimumBagSlots = new System.Windows.Forms.Label();
            this.groupboxLooting = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textboxTimeBetweenLoots = new System.Windows.Forms.TextBox();
            this.labelLootingTimeBetweenLoots = new System.Windows.Forms.Label();
            this.checkboxLootMobs = new System.Windows.Forms.CheckBox();
            this.groupboxSkinning = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkboxSkinMobs = new System.Windows.Forms.CheckBox();
            this.textboxTimeBetweenSkins = new System.Windows.Forms.TextBox();
            this.labelSkinningTimeBetweenSkins = new System.Windows.Forms.Label();
            this.checkboxNinjaSkin = new System.Windows.Forms.CheckBox();
            this.textboxStartLocationMaxDist = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.checkboxIgnoreEliteMobs = new System.Windows.Forms.CheckBox();
            this.labelStartLocationMaxDistance = new System.Windows.Forms.Label();
            this.groupboxMultiPull = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.labelMultiPullHealthThreshold = new System.Windows.Forms.Label();
            this.textboxMultiPullHpThresh = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.labelMultiPullMobThreshold = new System.Windows.Forms.Label();
            this.textboxMultiPullMobThresh = new System.Windows.Forms.TextBox();
            this.checkboxMultiPull = new System.Windows.Forms.CheckBox();
            this.tabpageWhitelist = new System.Windows.Forms.TabPage();
            this.checkboxAttackOnlyWhitelist = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonWhitelistAddUnit = new System.Windows.Forms.Button();
            this.labelWhitelistID = new System.Windows.Forms.Label();
            this.labelWhitelistName = new System.Windows.Forms.Label();
            this.textboxWhitelistID = new System.Windows.Forms.TextBox();
            this.textboxWhitelistName = new System.Windows.Forms.TextBox();
            this.buttonWhitelistRemove = new System.Windows.Forms.Button();
            this.buttonWhitelistAddTarget = new System.Windows.Forms.Button();
            this.datagridviewWhitelist = new System.Windows.Forms.DataGridView();
            this.tabpageBlacklist = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonBlacklistAddUnit = new System.Windows.Forms.Button();
            this.labelBlacklistID = new System.Windows.Forms.Label();
            this.labelBlacklistName = new System.Windows.Forms.Label();
            this.textboxBlacklistID = new System.Windows.Forms.TextBox();
            this.textboxBlacklistName = new System.Windows.Forms.TextBox();
            this.buttonBlacklistRemove = new System.Windows.Forms.Button();
            this.buttonBlacklistAddTarget = new System.Windows.Forms.Button();
            this.datagridviewBlacklist = new System.Windows.Forms.DataGridView();
            this.tabpageProtectedItems = new System.Windows.Forms.TabPage();
            this.addItemGroupBox = new System.Windows.Forms.GroupBox();
            this.buttonProtectedItemAddItem = new System.Windows.Forms.Button();
            this.labelProtectedItemsID = new System.Windows.Forms.Label();
            this.labelProtectedItemsName = new System.Windows.Forms.Label();
            this.textboxProtectedItemID = new System.Windows.Forms.TextBox();
            this.textboxProtectedItemName = new System.Windows.Forms.TextBox();
            this.buttonProtectedItemRemove = new System.Windows.Forms.Button();
            this.datagridviewProtectedItems = new System.Windows.Forms.DataGridView();
            this.tabpageMail = new System.Windows.Forms.TabPage();
            this.textboxRecipient = new System.Windows.Forms.TextBox();
            this.labelMailRecipient = new System.Windows.Forms.Label();
            this.checkboxMailPurples = new System.Windows.Forms.CheckBox();
            this.checkboxMailBlues = new System.Windows.Forms.CheckBox();
            this.checkboxMailGreens = new System.Windows.Forms.CheckBox();
            this.checkboxMail = new System.Windows.Forms.CheckBox();
            this.checkboxMailWhites = new System.Windows.Forms.CheckBox();
            this.tabpageVendor = new System.Windows.Forms.TabPage();
            this.checkboxVendor = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.labelRepairAtPercent = new System.Windows.Forms.Label();
            this.textboxRepairDurability = new System.Windows.Forms.TextBox();
            this.checkboxSellPurples = new System.Windows.Forms.CheckBox();
            this.checkboxSellBlues = new System.Windows.Forms.CheckBox();
            this.checkboxSellGreens = new System.Windows.Forms.CheckBox();
            this.checkboxSellWhites = new System.Windows.Forms.CheckBox();
            this.checkboxSellGrays = new System.Windows.Forms.CheckBox();
            this.tabpageDisenchant = new System.Windows.Forms.TabPage();
            this.checkboxDisenchantOnlyNewItems = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textboxTimeBetweenDisenchants = new System.Windows.Forms.TextBox();
            this.labelTimeBetweenDisenchants = new System.Windows.Forms.Label();
            this.checkboxDisenchantPurples = new System.Windows.Forms.CheckBox();
            this.checkboxDisenchantBlues = new System.Windows.Forms.CheckBox();
            this.checkboxDisenchantGreens = new System.Windows.Forms.CheckBox();
            this.checkboxDisenchant = new System.Windows.Forms.CheckBox();
            this.checkboxDisenchantWeapons = new System.Windows.Forms.CheckBox();
            this.tabcontrolBotConfig.SuspendLayout();
            this.tabpageGeneral.SuspendLayout();
            this.groupboxLooting.SuspendLayout();
            this.groupboxSkinning.SuspendLayout();
            this.groupboxMultiPull.SuspendLayout();
            this.tabpageWhitelist.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.datagridviewWhitelist)).BeginInit();
            this.tabpageBlacklist.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.datagridviewBlacklist)).BeginInit();
            this.tabpageProtectedItems.SuspendLayout();
            this.addItemGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.datagridviewProtectedItems)).BeginInit();
            this.tabpageMail.SuspendLayout();
            this.tabpageVendor.SuspendLayout();
            this.tabpageDisenchant.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabcontrolBotConfig
            // 
            this.tabcontrolBotConfig.Controls.Add(this.tabpageGeneral);
            this.tabcontrolBotConfig.Controls.Add(this.tabpageWhitelist);
            this.tabcontrolBotConfig.Controls.Add(this.tabpageBlacklist);
            this.tabcontrolBotConfig.Controls.Add(this.tabpageProtectedItems);
            this.tabcontrolBotConfig.Controls.Add(this.tabpageMail);
            this.tabcontrolBotConfig.Controls.Add(this.tabpageVendor);
            this.tabcontrolBotConfig.Controls.Add(this.tabpageDisenchant);
            this.tabcontrolBotConfig.Location = new System.Drawing.Point(1, 1);
            this.tabcontrolBotConfig.Name = "tabcontrolBotConfig";
            this.tabcontrolBotConfig.SelectedIndex = 0;
            this.tabcontrolBotConfig.Size = new System.Drawing.Size(395, 402);
            this.tabcontrolBotConfig.TabIndex = 0;
            // 
            // tabpageGeneral
            // 
            this.tabpageGeneral.Controls.Add(this.textboxBagSlotsRemaining);
            this.tabpageGeneral.Controls.Add(this.labelMinimumBagSlots);
            this.tabpageGeneral.Controls.Add(this.groupboxLooting);
            this.tabpageGeneral.Controls.Add(this.groupboxSkinning);
            this.tabpageGeneral.Controls.Add(this.textboxStartLocationMaxDist);
            this.tabpageGeneral.Controls.Add(this.label11);
            this.tabpageGeneral.Controls.Add(this.checkboxIgnoreEliteMobs);
            this.tabpageGeneral.Controls.Add(this.labelStartLocationMaxDistance);
            this.tabpageGeneral.Controls.Add(this.groupboxMultiPull);
            this.tabpageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabpageGeneral.Name = "tabpageGeneral";
            this.tabpageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabpageGeneral.Size = new System.Drawing.Size(387, 376);
            this.tabpageGeneral.TabIndex = 0;
            this.tabpageGeneral.Text = "General";
            this.tabpageGeneral.UseVisualStyleBackColor = true;
            // 
            // textboxBagSlotsRemaining
            // 
            this.textboxBagSlotsRemaining.Location = new System.Drawing.Point(108, 275);
            this.textboxBagSlotsRemaining.Name = "textboxBagSlotsRemaining";
            this.textboxBagSlotsRemaining.Size = new System.Drawing.Size(41, 20);
            this.textboxBagSlotsRemaining.TabIndex = 31;
            this.textboxBagSlotsRemaining.TextChanged += new System.EventHandler(this.textboxBagSlotsRemaining_TextChanged);
            // 
            // labelMinimumBagSlots
            // 
            this.labelMinimumBagSlots.AutoSize = true;
            this.labelMinimumBagSlots.Location = new System.Drawing.Point(6, 278);
            this.labelMinimumBagSlots.Name = "labelMinimumBagSlots";
            this.labelMinimumBagSlots.Size = new System.Drawing.Size(99, 13);
            this.labelMinimumBagSlots.TabIndex = 32;
            this.labelMinimumBagSlots.Text = "Minimum Bag Slots:";
            // 
            // groupboxLooting
            // 
            this.groupboxLooting.Controls.Add(this.label9);
            this.groupboxLooting.Controls.Add(this.textboxTimeBetweenLoots);
            this.groupboxLooting.Controls.Add(this.labelLootingTimeBetweenLoots);
            this.groupboxLooting.Controls.Add(this.checkboxLootMobs);
            this.groupboxLooting.Location = new System.Drawing.Point(9, 29);
            this.groupboxLooting.Name = "groupboxLooting";
            this.groupboxLooting.Size = new System.Drawing.Size(211, 62);
            this.groupboxLooting.TabIndex = 22;
            this.groupboxLooting.TabStop = false;
            this.groupboxLooting.Text = "Looting";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(159, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 13);
            this.label9.TabIndex = 25;
            this.label9.Text = "seconds";
            // 
            // textboxTimeBetweenLoots
            // 
            this.textboxTimeBetweenLoots.Location = new System.Drawing.Point(116, 36);
            this.textboxTimeBetweenLoots.Name = "textboxTimeBetweenLoots";
            this.textboxTimeBetweenLoots.Size = new System.Drawing.Size(37, 20);
            this.textboxTimeBetweenLoots.TabIndex = 24;
            this.textboxTimeBetweenLoots.TextChanged += new System.EventHandler(this.textboxTimeBetweenLoots_TextChanged);
            // 
            // labelLootingTimeBetweenLoots
            // 
            this.labelLootingTimeBetweenLoots.AutoSize = true;
            this.labelLootingTimeBetweenLoots.Location = new System.Drawing.Point(6, 39);
            this.labelLootingTimeBetweenLoots.Name = "labelLootingTimeBetweenLoots";
            this.labelLootingTimeBetweenLoots.Size = new System.Drawing.Size(104, 13);
            this.labelLootingTimeBetweenLoots.TabIndex = 23;
            this.labelLootingTimeBetweenLoots.Text = "Time Between Loots";
            // 
            // checkboxLootMobs
            // 
            this.checkboxLootMobs.AutoSize = true;
            this.checkboxLootMobs.Location = new System.Drawing.Point(6, 19);
            this.checkboxLootMobs.Name = "checkboxLootMobs";
            this.checkboxLootMobs.Size = new System.Drawing.Size(76, 17);
            this.checkboxLootMobs.TabIndex = 18;
            this.checkboxLootMobs.Text = "Loot Mobs";
            this.checkboxLootMobs.UseVisualStyleBackColor = true;
            this.checkboxLootMobs.CheckedChanged += new System.EventHandler(this.checkboxLootMobs_CheckedChanged);
            // 
            // groupboxSkinning
            // 
            this.groupboxSkinning.Controls.Add(this.label4);
            this.groupboxSkinning.Controls.Add(this.checkboxSkinMobs);
            this.groupboxSkinning.Controls.Add(this.textboxTimeBetweenSkins);
            this.groupboxSkinning.Controls.Add(this.labelSkinningTimeBetweenSkins);
            this.groupboxSkinning.Controls.Add(this.checkboxNinjaSkin);
            this.groupboxSkinning.Location = new System.Drawing.Point(9, 97);
            this.groupboxSkinning.Name = "groupboxSkinning";
            this.groupboxSkinning.Size = new System.Drawing.Size(215, 62);
            this.groupboxSkinning.TabIndex = 21;
            this.groupboxSkinning.TabStop = false;
            this.groupboxSkinning.Text = "Skinning";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(159, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "seconds";
            // 
            // checkboxSkinMobs
            // 
            this.checkboxSkinMobs.AutoSize = true;
            this.checkboxSkinMobs.Location = new System.Drawing.Point(6, 19);
            this.checkboxSkinMobs.Name = "checkboxSkinMobs";
            this.checkboxSkinMobs.Size = new System.Drawing.Size(76, 17);
            this.checkboxSkinMobs.TabIndex = 19;
            this.checkboxSkinMobs.Text = "Skin Mobs";
            this.checkboxSkinMobs.UseVisualStyleBackColor = true;
            this.checkboxSkinMobs.CheckedChanged += new System.EventHandler(this.checkboxSkinMobs_CheckedChanged);
            // 
            // textboxTimeBetweenSkins
            // 
            this.textboxTimeBetweenSkins.Location = new System.Drawing.Point(116, 36);
            this.textboxTimeBetweenSkins.Name = "textboxTimeBetweenSkins";
            this.textboxTimeBetweenSkins.Size = new System.Drawing.Size(37, 20);
            this.textboxTimeBetweenSkins.TabIndex = 22;
            this.textboxTimeBetweenSkins.TextChanged += new System.EventHandler(this.textboxTimeBetweenSkins_TextChanged);
            // 
            // labelSkinningTimeBetweenSkins
            // 
            this.labelSkinningTimeBetweenSkins.AutoSize = true;
            this.labelSkinningTimeBetweenSkins.Location = new System.Drawing.Point(6, 39);
            this.labelSkinningTimeBetweenSkins.Name = "labelSkinningTimeBetweenSkins";
            this.labelSkinningTimeBetweenSkins.Size = new System.Drawing.Size(104, 13);
            this.labelSkinningTimeBetweenSkins.TabIndex = 21;
            this.labelSkinningTimeBetweenSkins.Text = "Time Between Skins";
            // 
            // checkboxNinjaSkin
            // 
            this.checkboxNinjaSkin.AutoSize = true;
            this.checkboxNinjaSkin.Location = new System.Drawing.Point(88, 19);
            this.checkboxNinjaSkin.Name = "checkboxNinjaSkin";
            this.checkboxNinjaSkin.Size = new System.Drawing.Size(74, 17);
            this.checkboxNinjaSkin.TabIndex = 20;
            this.checkboxNinjaSkin.Text = "Ninja Skin";
            this.checkboxNinjaSkin.UseVisualStyleBackColor = true;
            this.checkboxNinjaSkin.CheckedChanged += new System.EventHandler(this.checkboxNinjaSkin_CheckedChanged);
            // 
            // textboxStartLocationMaxDist
            // 
            this.textboxStartLocationMaxDist.Location = new System.Drawing.Point(130, 252);
            this.textboxStartLocationMaxDist.Name = "textboxStartLocationMaxDist";
            this.textboxStartLocationMaxDist.Size = new System.Drawing.Size(41, 20);
            this.textboxStartLocationMaxDist.TabIndex = 13;
            this.textboxStartLocationMaxDist.TextChanged += new System.EventHandler(this.textboxStartLocationMaxDist_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(177, 255);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 13);
            this.label11.TabIndex = 14;
            this.label11.Text = "yards";
            // 
            // checkboxIgnoreEliteMobs
            // 
            this.checkboxIgnoreEliteMobs.AutoSize = true;
            this.checkboxIgnoreEliteMobs.Location = new System.Drawing.Point(6, 6);
            this.checkboxIgnoreEliteMobs.Name = "checkboxIgnoreEliteMobs";
            this.checkboxIgnoreEliteMobs.Size = new System.Drawing.Size(108, 17);
            this.checkboxIgnoreEliteMobs.TabIndex = 17;
            this.checkboxIgnoreEliteMobs.Text = "Ignore Elite Mobs";
            this.checkboxIgnoreEliteMobs.UseVisualStyleBackColor = true;
            this.checkboxIgnoreEliteMobs.CheckedChanged += new System.EventHandler(this.checkboxIgnoreEliteMobs_CheckedChanged);
            // 
            // labelStartLocationMaxDistance
            // 
            this.labelStartLocationMaxDistance.AutoSize = true;
            this.labelStartLocationMaxDistance.Location = new System.Drawing.Point(6, 255);
            this.labelStartLocationMaxDistance.Name = "labelStartLocationMaxDistance";
            this.labelStartLocationMaxDistance.Size = new System.Drawing.Size(121, 13);
            this.labelStartLocationMaxDistance.TabIndex = 12;
            this.labelStartLocationMaxDistance.Text = "Start Loc Max Distance:";
            // 
            // groupboxMultiPull
            // 
            this.groupboxMultiPull.Controls.Add(this.label7);
            this.groupboxMultiPull.Controls.Add(this.labelMultiPullHealthThreshold);
            this.groupboxMultiPull.Controls.Add(this.textboxMultiPullHpThresh);
            this.groupboxMultiPull.Controls.Add(this.label6);
            this.groupboxMultiPull.Controls.Add(this.labelMultiPullMobThreshold);
            this.groupboxMultiPull.Controls.Add(this.textboxMultiPullMobThresh);
            this.groupboxMultiPull.Controls.Add(this.checkboxMultiPull);
            this.groupboxMultiPull.Location = new System.Drawing.Point(9, 165);
            this.groupboxMultiPull.Name = "groupboxMultiPull";
            this.groupboxMultiPull.Size = new System.Drawing.Size(200, 87);
            this.groupboxMultiPull.TabIndex = 3;
            this.groupboxMultiPull.TabStop = false;
            this.groupboxMultiPull.Text = "Multi Pull";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(150, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(15, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "%";
            // 
            // labelMultiPullHealthThreshold
            // 
            this.labelMultiPullHealthThreshold.AutoSize = true;
            this.labelMultiPullHealthThreshold.Location = new System.Drawing.Point(6, 65);
            this.labelMultiPullHealthThreshold.Name = "labelMultiPullHealthThreshold";
            this.labelMultiPullHealthThreshold.Size = new System.Drawing.Size(91, 13);
            this.labelMultiPullHealthThreshold.TabIndex = 7;
            this.labelMultiPullHealthThreshold.Text = "Health Threshold:";
            // 
            // textboxMultiPullHpThresh
            // 
            this.textboxMultiPullHpThresh.Location = new System.Drawing.Point(103, 62);
            this.textboxMultiPullHpThresh.Name = "textboxMultiPullHpThresh";
            this.textboxMultiPullHpThresh.Size = new System.Drawing.Size(41, 20);
            this.textboxMultiPullHpThresh.TabIndex = 6;
            this.textboxMultiPullHpThresh.TextChanged += new System.EventHandler(this.textboxMultiPullHpThresh_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(130, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "mobs";
            // 
            // labelMultiPullMobThreshold
            // 
            this.labelMultiPullMobThreshold.AutoSize = true;
            this.labelMultiPullMobThreshold.Location = new System.Drawing.Point(6, 39);
            this.labelMultiPullMobThreshold.Name = "labelMultiPullMobThreshold";
            this.labelMultiPullMobThreshold.Size = new System.Drawing.Size(81, 13);
            this.labelMultiPullMobThreshold.TabIndex = 4;
            this.labelMultiPullMobThreshold.Text = "Mob Threshold:";
            // 
            // textboxMultiPullMobThresh
            // 
            this.textboxMultiPullMobThresh.Location = new System.Drawing.Point(93, 36);
            this.textboxMultiPullMobThresh.Name = "textboxMultiPullMobThresh";
            this.textboxMultiPullMobThresh.Size = new System.Drawing.Size(31, 20);
            this.textboxMultiPullMobThresh.TabIndex = 3;
            this.textboxMultiPullMobThresh.TextChanged += new System.EventHandler(this.textboxMultiPullMobThreshold_TextChanged);
            // 
            // checkboxMultiPull
            // 
            this.checkboxMultiPull.AutoSize = true;
            this.checkboxMultiPull.Location = new System.Drawing.Point(6, 19);
            this.checkboxMultiPull.Name = "checkboxMultiPull";
            this.checkboxMultiPull.Size = new System.Drawing.Size(68, 17);
            this.checkboxMultiPull.TabIndex = 2;
            this.checkboxMultiPull.Text = "Multi Pull";
            this.checkboxMultiPull.UseVisualStyleBackColor = true;
            this.checkboxMultiPull.CheckedChanged += new System.EventHandler(this.checkboxMultiPull_CheckedChanged);
            // 
            // tabpageWhitelist
            // 
            this.tabpageWhitelist.Controls.Add(this.checkboxAttackOnlyWhitelist);
            this.tabpageWhitelist.Controls.Add(this.groupBox2);
            this.tabpageWhitelist.Controls.Add(this.buttonWhitelistRemove);
            this.tabpageWhitelist.Controls.Add(this.buttonWhitelistAddTarget);
            this.tabpageWhitelist.Controls.Add(this.datagridviewWhitelist);
            this.tabpageWhitelist.Location = new System.Drawing.Point(4, 22);
            this.tabpageWhitelist.Name = "tabpageWhitelist";
            this.tabpageWhitelist.Padding = new System.Windows.Forms.Padding(3);
            this.tabpageWhitelist.Size = new System.Drawing.Size(387, 376);
            this.tabpageWhitelist.TabIndex = 1;
            this.tabpageWhitelist.Text = "Whitelist";
            this.tabpageWhitelist.UseVisualStyleBackColor = true;
            // 
            // checkboxAttackOnlyWhitelist
            // 
            this.checkboxAttackOnlyWhitelist.AutoSize = true;
            this.checkboxAttackOnlyWhitelist.Location = new System.Drawing.Point(6, 317);
            this.checkboxAttackOnlyWhitelist.Name = "checkboxAttackOnlyWhitelist";
            this.checkboxAttackOnlyWhitelist.Size = new System.Drawing.Size(147, 17);
            this.checkboxAttackOnlyWhitelist.TabIndex = 24;
            this.checkboxAttackOnlyWhitelist.Text = "Attack only whitelist mobs";
            this.checkboxAttackOnlyWhitelist.UseVisualStyleBackColor = true;
            this.checkboxAttackOnlyWhitelist.CheckedChanged += new System.EventHandler(this.checkboxAttackOnlyWhitelist_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonWhitelistAddUnit);
            this.groupBox2.Controls.Add(this.labelWhitelistID);
            this.groupBox2.Controls.Add(this.labelWhitelistName);
            this.groupBox2.Controls.Add(this.textboxWhitelistID);
            this.groupBox2.Controls.Add(this.textboxWhitelistName);
            this.groupBox2.Location = new System.Drawing.Point(6, 233);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(299, 78);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Add NPC";
            // 
            // buttonWhitelistAddUnit
            // 
            this.buttonWhitelistAddUnit.Location = new System.Drawing.Point(194, 45);
            this.buttonWhitelistAddUnit.Name = "buttonWhitelistAddUnit";
            this.buttonWhitelistAddUnit.Size = new System.Drawing.Size(87, 23);
            this.buttonWhitelistAddUnit.TabIndex = 15;
            this.buttonWhitelistAddUnit.Text = "Add NPC";
            this.buttonWhitelistAddUnit.UseVisualStyleBackColor = true;
            this.buttonWhitelistAddUnit.Click += new System.EventHandler(this.buttonWhitelistAddUnit_Click);
            // 
            // labelWhitelistID
            // 
            this.labelWhitelistID.AutoSize = true;
            this.labelWhitelistID.Location = new System.Drawing.Point(6, 22);
            this.labelWhitelistID.Name = "labelWhitelistID";
            this.labelWhitelistID.Size = new System.Drawing.Size(21, 13);
            this.labelWhitelistID.TabIndex = 16;
            this.labelWhitelistID.Text = "ID:";
            // 
            // labelWhitelistName
            // 
            this.labelWhitelistName.AutoSize = true;
            this.labelWhitelistName.Location = new System.Drawing.Point(75, 22);
            this.labelWhitelistName.Name = "labelWhitelistName";
            this.labelWhitelistName.Size = new System.Drawing.Size(38, 13);
            this.labelWhitelistName.TabIndex = 17;
            this.labelWhitelistName.Text = "Name:";
            // 
            // textboxWhitelistID
            // 
            this.textboxWhitelistID.Location = new System.Drawing.Point(33, 19);
            this.textboxWhitelistID.Name = "textboxWhitelistID";
            this.textboxWhitelistID.Size = new System.Drawing.Size(36, 20);
            this.textboxWhitelistID.TabIndex = 14;
            // 
            // textboxWhitelistName
            // 
            this.textboxWhitelistName.Location = new System.Drawing.Point(113, 19);
            this.textboxWhitelistName.Name = "textboxWhitelistName";
            this.textboxWhitelistName.Size = new System.Drawing.Size(168, 20);
            this.textboxWhitelistName.TabIndex = 18;
            // 
            // buttonWhitelistRemove
            // 
            this.buttonWhitelistRemove.Location = new System.Drawing.Point(311, 262);
            this.buttonWhitelistRemove.Name = "buttonWhitelistRemove";
            this.buttonWhitelistRemove.Size = new System.Drawing.Size(70, 23);
            this.buttonWhitelistRemove.TabIndex = 3;
            this.buttonWhitelistRemove.Text = "Remove";
            this.buttonWhitelistRemove.UseVisualStyleBackColor = true;
            this.buttonWhitelistRemove.Click += new System.EventHandler(this.buttonWhitelistRemove_Click);
            // 
            // buttonWhitelistAddTarget
            // 
            this.buttonWhitelistAddTarget.Location = new System.Drawing.Point(311, 233);
            this.buttonWhitelistAddTarget.Name = "buttonWhitelistAddTarget";
            this.buttonWhitelistAddTarget.Size = new System.Drawing.Size(70, 23);
            this.buttonWhitelistAddTarget.TabIndex = 2;
            this.buttonWhitelistAddTarget.Text = "Add Target";
            this.buttonWhitelistAddTarget.UseVisualStyleBackColor = true;
            this.buttonWhitelistAddTarget.Click += new System.EventHandler(this.buttonWhitelistAddTarget_Click);
            // 
            // datagridviewWhitelist
            // 
            this.datagridviewWhitelist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridviewWhitelist.Location = new System.Drawing.Point(6, 6);
            this.datagridviewWhitelist.Name = "datagridviewWhitelist";
            this.datagridviewWhitelist.Size = new System.Drawing.Size(375, 221);
            this.datagridviewWhitelist.TabIndex = 0;
            // 
            // tabpageBlacklist
            // 
            this.tabpageBlacklist.Controls.Add(this.groupBox1);
            this.tabpageBlacklist.Controls.Add(this.buttonBlacklistRemove);
            this.tabpageBlacklist.Controls.Add(this.buttonBlacklistAddTarget);
            this.tabpageBlacklist.Controls.Add(this.datagridviewBlacklist);
            this.tabpageBlacklist.Location = new System.Drawing.Point(4, 22);
            this.tabpageBlacklist.Name = "tabpageBlacklist";
            this.tabpageBlacklist.Size = new System.Drawing.Size(387, 376);
            this.tabpageBlacklist.TabIndex = 2;
            this.tabpageBlacklist.Text = "Blacklist";
            this.tabpageBlacklist.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonBlacklistAddUnit);
            this.groupBox1.Controls.Add(this.labelBlacklistID);
            this.groupBox1.Controls.Add(this.labelBlacklistName);
            this.groupBox1.Controls.Add(this.textboxBlacklistID);
            this.groupBox1.Controls.Add(this.textboxBlacklistName);
            this.groupBox1.Location = new System.Drawing.Point(6, 233);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(299, 78);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add NPC";
            // 
            // buttonBlacklistAddUnit
            // 
            this.buttonBlacklistAddUnit.Location = new System.Drawing.Point(194, 45);
            this.buttonBlacklistAddUnit.Name = "buttonBlacklistAddUnit";
            this.buttonBlacklistAddUnit.Size = new System.Drawing.Size(87, 23);
            this.buttonBlacklistAddUnit.TabIndex = 15;
            this.buttonBlacklistAddUnit.Text = "Add NPC";
            this.buttonBlacklistAddUnit.UseVisualStyleBackColor = true;
            this.buttonBlacklistAddUnit.Click += new System.EventHandler(this.buttonBlacklistAddUnit_Click);
            // 
            // labelBlacklistID
            // 
            this.labelBlacklistID.AutoSize = true;
            this.labelBlacklistID.Location = new System.Drawing.Point(6, 22);
            this.labelBlacklistID.Name = "labelBlacklistID";
            this.labelBlacklistID.Size = new System.Drawing.Size(21, 13);
            this.labelBlacklistID.TabIndex = 16;
            this.labelBlacklistID.Text = "ID:";
            // 
            // labelBlacklistName
            // 
            this.labelBlacklistName.AutoSize = true;
            this.labelBlacklistName.Location = new System.Drawing.Point(75, 22);
            this.labelBlacklistName.Name = "labelBlacklistName";
            this.labelBlacklistName.Size = new System.Drawing.Size(38, 13);
            this.labelBlacklistName.TabIndex = 17;
            this.labelBlacklistName.Text = "Name:";
            // 
            // textboxBlacklistID
            // 
            this.textboxBlacklistID.Location = new System.Drawing.Point(33, 19);
            this.textboxBlacklistID.Name = "textboxBlacklistID";
            this.textboxBlacklistID.Size = new System.Drawing.Size(36, 20);
            this.textboxBlacklistID.TabIndex = 14;
            // 
            // textboxBlacklistName
            // 
            this.textboxBlacklistName.Location = new System.Drawing.Point(113, 19);
            this.textboxBlacklistName.Name = "textboxBlacklistName";
            this.textboxBlacklistName.Size = new System.Drawing.Size(168, 20);
            this.textboxBlacklistName.TabIndex = 18;
            // 
            // buttonBlacklistRemove
            // 
            this.buttonBlacklistRemove.Location = new System.Drawing.Point(311, 262);
            this.buttonBlacklistRemove.Name = "buttonBlacklistRemove";
            this.buttonBlacklistRemove.Size = new System.Drawing.Size(70, 23);
            this.buttonBlacklistRemove.TabIndex = 7;
            this.buttonBlacklistRemove.Text = "Remove";
            this.buttonBlacklistRemove.UseVisualStyleBackColor = true;
            this.buttonBlacklistRemove.Click += new System.EventHandler(this.buttonBlacklistRemove_Click);
            // 
            // buttonBlacklistAddTarget
            // 
            this.buttonBlacklistAddTarget.Location = new System.Drawing.Point(311, 233);
            this.buttonBlacklistAddTarget.Name = "buttonBlacklistAddTarget";
            this.buttonBlacklistAddTarget.Size = new System.Drawing.Size(70, 23);
            this.buttonBlacklistAddTarget.TabIndex = 6;
            this.buttonBlacklistAddTarget.Text = "Add Target";
            this.buttonBlacklistAddTarget.UseVisualStyleBackColor = true;
            this.buttonBlacklistAddTarget.Click += new System.EventHandler(this.buttonBlacklistAddTarget_Click);
            // 
            // datagridviewBlacklist
            // 
            this.datagridviewBlacklist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridviewBlacklist.Location = new System.Drawing.Point(6, 6);
            this.datagridviewBlacklist.Name = "datagridviewBlacklist";
            this.datagridviewBlacklist.Size = new System.Drawing.Size(375, 221);
            this.datagridviewBlacklist.TabIndex = 1;
            // 
            // tabpageProtectedItems
            // 
            this.tabpageProtectedItems.Controls.Add(this.addItemGroupBox);
            this.tabpageProtectedItems.Controls.Add(this.buttonProtectedItemRemove);
            this.tabpageProtectedItems.Controls.Add(this.datagridviewProtectedItems);
            this.tabpageProtectedItems.Location = new System.Drawing.Point(4, 22);
            this.tabpageProtectedItems.Name = "tabpageProtectedItems";
            this.tabpageProtectedItems.Size = new System.Drawing.Size(387, 376);
            this.tabpageProtectedItems.TabIndex = 3;
            this.tabpageProtectedItems.Text = "Protected Items";
            this.tabpageProtectedItems.UseVisualStyleBackColor = true;
            // 
            // addItemGroupBox
            // 
            this.addItemGroupBox.Controls.Add(this.buttonProtectedItemAddItem);
            this.addItemGroupBox.Controls.Add(this.labelProtectedItemsID);
            this.addItemGroupBox.Controls.Add(this.labelProtectedItemsName);
            this.addItemGroupBox.Controls.Add(this.textboxProtectedItemID);
            this.addItemGroupBox.Controls.Add(this.textboxProtectedItemName);
            this.addItemGroupBox.Location = new System.Drawing.Point(6, 233);
            this.addItemGroupBox.Name = "addItemGroupBox";
            this.addItemGroupBox.Size = new System.Drawing.Size(299, 78);
            this.addItemGroupBox.TabIndex = 21;
            this.addItemGroupBox.TabStop = false;
            this.addItemGroupBox.Text = "Add Item";
            // 
            // buttonProtectedItemAddItem
            // 
            this.buttonProtectedItemAddItem.Location = new System.Drawing.Point(194, 45);
            this.buttonProtectedItemAddItem.Name = "buttonProtectedItemAddItem";
            this.buttonProtectedItemAddItem.Size = new System.Drawing.Size(87, 23);
            this.buttonProtectedItemAddItem.TabIndex = 15;
            this.buttonProtectedItemAddItem.Text = "Add Item";
            this.buttonProtectedItemAddItem.UseVisualStyleBackColor = true;
            this.buttonProtectedItemAddItem.Click += new System.EventHandler(this.buttonProtectedItemAddItem_Click);
            // 
            // labelProtectedItemsID
            // 
            this.labelProtectedItemsID.AutoSize = true;
            this.labelProtectedItemsID.Location = new System.Drawing.Point(6, 22);
            this.labelProtectedItemsID.Name = "labelProtectedItemsID";
            this.labelProtectedItemsID.Size = new System.Drawing.Size(21, 13);
            this.labelProtectedItemsID.TabIndex = 16;
            this.labelProtectedItemsID.Text = "ID:";
            // 
            // labelProtectedItemsName
            // 
            this.labelProtectedItemsName.AutoSize = true;
            this.labelProtectedItemsName.Location = new System.Drawing.Point(75, 22);
            this.labelProtectedItemsName.Name = "labelProtectedItemsName";
            this.labelProtectedItemsName.Size = new System.Drawing.Size(38, 13);
            this.labelProtectedItemsName.TabIndex = 17;
            this.labelProtectedItemsName.Text = "Name:";
            // 
            // textboxProtectedItemID
            // 
            this.textboxProtectedItemID.Location = new System.Drawing.Point(33, 19);
            this.textboxProtectedItemID.Name = "textboxProtectedItemID";
            this.textboxProtectedItemID.Size = new System.Drawing.Size(36, 20);
            this.textboxProtectedItemID.TabIndex = 14;
            // 
            // textboxProtectedItemName
            // 
            this.textboxProtectedItemName.Location = new System.Drawing.Point(113, 19);
            this.textboxProtectedItemName.Name = "textboxProtectedItemName";
            this.textboxProtectedItemName.Size = new System.Drawing.Size(168, 20);
            this.textboxProtectedItemName.TabIndex = 18;
            // 
            // buttonProtectedItemRemove
            // 
            this.buttonProtectedItemRemove.Location = new System.Drawing.Point(311, 233);
            this.buttonProtectedItemRemove.Name = "buttonProtectedItemRemove";
            this.buttonProtectedItemRemove.Size = new System.Drawing.Size(70, 23);
            this.buttonProtectedItemRemove.TabIndex = 13;
            this.buttonProtectedItemRemove.Text = "Remove";
            this.buttonProtectedItemRemove.UseVisualStyleBackColor = true;
            this.buttonProtectedItemRemove.Click += new System.EventHandler(this.buttonProtectedItemRemove_Click);
            // 
            // datagridviewProtectedItems
            // 
            this.datagridviewProtectedItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridviewProtectedItems.Location = new System.Drawing.Point(6, 6);
            this.datagridviewProtectedItems.Name = "datagridviewProtectedItems";
            this.datagridviewProtectedItems.Size = new System.Drawing.Size(375, 221);
            this.datagridviewProtectedItems.TabIndex = 2;
            // 
            // tabpageMail
            // 
            this.tabpageMail.Controls.Add(this.textboxRecipient);
            this.tabpageMail.Controls.Add(this.labelMailRecipient);
            this.tabpageMail.Controls.Add(this.checkboxMailPurples);
            this.tabpageMail.Controls.Add(this.checkboxMailBlues);
            this.tabpageMail.Controls.Add(this.checkboxMailGreens);
            this.tabpageMail.Controls.Add(this.checkboxMail);
            this.tabpageMail.Controls.Add(this.checkboxMailWhites);
            this.tabpageMail.Location = new System.Drawing.Point(4, 22);
            this.tabpageMail.Name = "tabpageMail";
            this.tabpageMail.Size = new System.Drawing.Size(387, 376);
            this.tabpageMail.TabIndex = 4;
            this.tabpageMail.Text = "Mail";
            this.tabpageMail.UseVisualStyleBackColor = true;
            // 
            // textboxRecipient
            // 
            this.textboxRecipient.Location = new System.Drawing.Point(62, 112);
            this.textboxRecipient.Name = "textboxRecipient";
            this.textboxRecipient.Size = new System.Drawing.Size(130, 20);
            this.textboxRecipient.TabIndex = 28;
            this.textboxRecipient.TextChanged += new System.EventHandler(this.textboxRecipient_TextChanged);
            // 
            // labelMailRecipient
            // 
            this.labelMailRecipient.AutoSize = true;
            this.labelMailRecipient.Location = new System.Drawing.Point(4, 115);
            this.labelMailRecipient.Name = "labelMailRecipient";
            this.labelMailRecipient.Size = new System.Drawing.Size(55, 13);
            this.labelMailRecipient.TabIndex = 27;
            this.labelMailRecipient.Text = "Recipient:";
            // 
            // checkboxMailPurples
            // 
            this.checkboxMailPurples.AutoSize = true;
            this.checkboxMailPurples.Checked = true;
            this.checkboxMailPurples.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkboxMailPurples.Location = new System.Drawing.Point(7, 95);
            this.checkboxMailPurples.Name = "checkboxMailPurples";
            this.checkboxMailPurples.Size = new System.Drawing.Size(83, 17);
            this.checkboxMailPurples.TabIndex = 26;
            this.checkboxMailPurples.Text = "Mail Purples";
            this.checkboxMailPurples.UseVisualStyleBackColor = true;
            this.checkboxMailPurples.CheckedChanged += new System.EventHandler(this.checkboxMailPurples_CheckedChanged);
            // 
            // checkboxMailBlues
            // 
            this.checkboxMailBlues.AutoSize = true;
            this.checkboxMailBlues.Checked = true;
            this.checkboxMailBlues.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkboxMailBlues.Location = new System.Drawing.Point(7, 72);
            this.checkboxMailBlues.Name = "checkboxMailBlues";
            this.checkboxMailBlues.Size = new System.Drawing.Size(74, 17);
            this.checkboxMailBlues.TabIndex = 25;
            this.checkboxMailBlues.Text = "Mail Blues";
            this.checkboxMailBlues.UseVisualStyleBackColor = true;
            this.checkboxMailBlues.CheckedChanged += new System.EventHandler(this.checkboxMailBlues_CheckedChanged);
            // 
            // checkboxMailGreens
            // 
            this.checkboxMailGreens.AutoSize = true;
            this.checkboxMailGreens.Checked = true;
            this.checkboxMailGreens.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkboxMailGreens.Location = new System.Drawing.Point(7, 49);
            this.checkboxMailGreens.Name = "checkboxMailGreens";
            this.checkboxMailGreens.Size = new System.Drawing.Size(82, 17);
            this.checkboxMailGreens.TabIndex = 24;
            this.checkboxMailGreens.Text = "Mail Greens";
            this.checkboxMailGreens.UseVisualStyleBackColor = true;
            this.checkboxMailGreens.CheckedChanged += new System.EventHandler(this.checkboxMailGreens_CheckedChanged);
            // 
            // checkboxMail
            // 
            this.checkboxMail.AutoSize = true;
            this.checkboxMail.Checked = true;
            this.checkboxMail.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkboxMail.Location = new System.Drawing.Point(7, 3);
            this.checkboxMail.Name = "checkboxMail";
            this.checkboxMail.Size = new System.Drawing.Size(95, 17);
            this.checkboxMail.TabIndex = 21;
            this.checkboxMail.Text = "Enable Mailing";
            this.checkboxMail.UseVisualStyleBackColor = true;
            this.checkboxMail.CheckedChanged += new System.EventHandler(this.checkboxMail_CheckedChanged);
            // 
            // checkboxMailWhites
            // 
            this.checkboxMailWhites.AutoSize = true;
            this.checkboxMailWhites.Checked = true;
            this.checkboxMailWhites.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkboxMailWhites.Location = new System.Drawing.Point(7, 26);
            this.checkboxMailWhites.Name = "checkboxMailWhites";
            this.checkboxMailWhites.Size = new System.Drawing.Size(81, 17);
            this.checkboxMailWhites.TabIndex = 23;
            this.checkboxMailWhites.Text = "Mail Whites";
            this.checkboxMailWhites.UseVisualStyleBackColor = true;
            this.checkboxMailWhites.CheckedChanged += new System.EventHandler(this.checkboxMailWhites_CheckedChanged);
            // 
            // tabpageVendor
            // 
            this.tabpageVendor.Controls.Add(this.checkboxVendor);
            this.tabpageVendor.Controls.Add(this.label14);
            this.tabpageVendor.Controls.Add(this.labelRepairAtPercent);
            this.tabpageVendor.Controls.Add(this.textboxRepairDurability);
            this.tabpageVendor.Controls.Add(this.checkboxSellPurples);
            this.tabpageVendor.Controls.Add(this.checkboxSellBlues);
            this.tabpageVendor.Controls.Add(this.checkboxSellGreens);
            this.tabpageVendor.Controls.Add(this.checkboxSellWhites);
            this.tabpageVendor.Controls.Add(this.checkboxSellGrays);
            this.tabpageVendor.Location = new System.Drawing.Point(4, 22);
            this.tabpageVendor.Name = "tabpageVendor";
            this.tabpageVendor.Size = new System.Drawing.Size(387, 376);
            this.tabpageVendor.TabIndex = 5;
            this.tabpageVendor.Text = "Vendor";
            this.tabpageVendor.UseVisualStyleBackColor = true;
            // 
            // checkboxVendor
            // 
            this.checkboxVendor.AutoSize = true;
            this.checkboxVendor.Checked = true;
            this.checkboxVendor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkboxVendor.Location = new System.Drawing.Point(7, 3);
            this.checkboxVendor.Name = "checkboxVendor";
            this.checkboxVendor.Size = new System.Drawing.Size(184, 17);
            this.checkboxVendor.TabIndex = 32;
            this.checkboxVendor.Text = "Enable Vendoring (sell and repair)";
            this.checkboxVendor.UseVisualStyleBackColor = true;
            this.checkboxVendor.CheckedChanged += new System.EventHandler(this.checkboxVendor_CheckedChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(109, 138);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(15, 13);
            this.label14.TabIndex = 26;
            this.label14.Text = "%";
            // 
            // labelRepairAtPercent
            // 
            this.labelRepairAtPercent.AutoSize = true;
            this.labelRepairAtPercent.Location = new System.Drawing.Point(4, 138);
            this.labelRepairAtPercent.Name = "labelRepairAtPercent";
            this.labelRepairAtPercent.Size = new System.Drawing.Size(53, 13);
            this.labelRepairAtPercent.TabIndex = 25;
            this.labelRepairAtPercent.Text = "Repair at:";
            // 
            // textboxRepairDurability
            // 
            this.textboxRepairDurability.Location = new System.Drawing.Point(62, 135);
            this.textboxRepairDurability.Name = "textboxRepairDurability";
            this.textboxRepairDurability.Size = new System.Drawing.Size(41, 20);
            this.textboxRepairDurability.TabIndex = 24;
            this.textboxRepairDurability.TextChanged += new System.EventHandler(this.textboxRepairDurability_TextChanged);
            // 
            // checkboxSellPurples
            // 
            this.checkboxSellPurples.AutoSize = true;
            this.checkboxSellPurples.Location = new System.Drawing.Point(7, 118);
            this.checkboxSellPurples.Name = "checkboxSellPurples";
            this.checkboxSellPurples.Size = new System.Drawing.Size(81, 17);
            this.checkboxSellPurples.TabIndex = 31;
            this.checkboxSellPurples.Text = "Sell Purples";
            this.checkboxSellPurples.UseVisualStyleBackColor = true;
            this.checkboxSellPurples.CheckedChanged += new System.EventHandler(this.checkboxSellPurples_CheckedChanged);
            // 
            // checkboxSellBlues
            // 
            this.checkboxSellBlues.AutoSize = true;
            this.checkboxSellBlues.Location = new System.Drawing.Point(7, 95);
            this.checkboxSellBlues.Name = "checkboxSellBlues";
            this.checkboxSellBlues.Size = new System.Drawing.Size(72, 17);
            this.checkboxSellBlues.TabIndex = 30;
            this.checkboxSellBlues.Text = "Sell Blues";
            this.checkboxSellBlues.UseVisualStyleBackColor = true;
            this.checkboxSellBlues.CheckedChanged += new System.EventHandler(this.checkboxSellBlues_CheckedChanged);
            // 
            // checkboxSellGreens
            // 
            this.checkboxSellGreens.AutoSize = true;
            this.checkboxSellGreens.Location = new System.Drawing.Point(7, 72);
            this.checkboxSellGreens.Name = "checkboxSellGreens";
            this.checkboxSellGreens.Size = new System.Drawing.Size(80, 17);
            this.checkboxSellGreens.TabIndex = 29;
            this.checkboxSellGreens.Text = "Sell Greens";
            this.checkboxSellGreens.UseVisualStyleBackColor = true;
            this.checkboxSellGreens.CheckedChanged += new System.EventHandler(this.checkboxSellGreens_CheckedChanged);
            // 
            // checkboxSellWhites
            // 
            this.checkboxSellWhites.AutoSize = true;
            this.checkboxSellWhites.Location = new System.Drawing.Point(7, 49);
            this.checkboxSellWhites.Name = "checkboxSellWhites";
            this.checkboxSellWhites.Size = new System.Drawing.Size(79, 17);
            this.checkboxSellWhites.TabIndex = 28;
            this.checkboxSellWhites.Text = "Sell Whites";
            this.checkboxSellWhites.UseVisualStyleBackColor = true;
            this.checkboxSellWhites.CheckedChanged += new System.EventHandler(this.checkboxSellWhites_CheckedChanged);
            // 
            // checkboxSellGrays
            // 
            this.checkboxSellGrays.AutoSize = true;
            this.checkboxSellGrays.Checked = true;
            this.checkboxSellGrays.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkboxSellGrays.Location = new System.Drawing.Point(7, 26);
            this.checkboxSellGrays.Name = "checkboxSellGrays";
            this.checkboxSellGrays.Size = new System.Drawing.Size(73, 17);
            this.checkboxSellGrays.TabIndex = 27;
            this.checkboxSellGrays.Text = "Sell Grays";
            this.checkboxSellGrays.UseVisualStyleBackColor = true;
            this.checkboxSellGrays.CheckedChanged += new System.EventHandler(this.checkboxSellGrays_CheckedChanged);
            // 
            // tabpageDisenchant
            // 
            this.tabpageDisenchant.Controls.Add(this.checkboxDisenchantWeapons);
            this.tabpageDisenchant.Controls.Add(this.checkboxDisenchantOnlyNewItems);
            this.tabpageDisenchant.Controls.Add(this.label1);
            this.tabpageDisenchant.Controls.Add(this.textboxTimeBetweenDisenchants);
            this.tabpageDisenchant.Controls.Add(this.labelTimeBetweenDisenchants);
            this.tabpageDisenchant.Controls.Add(this.checkboxDisenchantPurples);
            this.tabpageDisenchant.Controls.Add(this.checkboxDisenchantBlues);
            this.tabpageDisenchant.Controls.Add(this.checkboxDisenchantGreens);
            this.tabpageDisenchant.Controls.Add(this.checkboxDisenchant);
            this.tabpageDisenchant.Location = new System.Drawing.Point(4, 22);
            this.tabpageDisenchant.Name = "tabpageDisenchant";
            this.tabpageDisenchant.Size = new System.Drawing.Size(387, 376);
            this.tabpageDisenchant.TabIndex = 6;
            this.tabpageDisenchant.Text = "Disenchant";
            this.tabpageDisenchant.UseVisualStyleBackColor = true;
            // 
            // checkboxDisenchantOnlyNewItems
            // 
            this.checkboxDisenchantOnlyNewItems.AutoSize = true;
            this.checkboxDisenchantOnlyNewItems.Location = new System.Drawing.Point(7, 26);
            this.checkboxDisenchantOnlyNewItems.Name = "checkboxDisenchantOnlyNewItems";
            this.checkboxDisenchantOnlyNewItems.Size = new System.Drawing.Size(157, 17);
            this.checkboxDisenchantOnlyNewItems.TabIndex = 26;
            this.checkboxDisenchantOnlyNewItems.Text = "Disenchant Only New Items";
            this.checkboxDisenchantOnlyNewItems.UseVisualStyleBackColor = true;
            this.checkboxDisenchantOnlyNewItems.CheckedChanged += new System.EventHandler(this.checkboxDisenchantOnlyNewItems_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(190, 138);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "seconds";
            // 
            // textboxTimeBetweenDisenchants
            // 
            this.textboxTimeBetweenDisenchants.Location = new System.Drawing.Point(147, 135);
            this.textboxTimeBetweenDisenchants.Name = "textboxTimeBetweenDisenchants";
            this.textboxTimeBetweenDisenchants.Size = new System.Drawing.Size(37, 20);
            this.textboxTimeBetweenDisenchants.TabIndex = 24;
            this.textboxTimeBetweenDisenchants.TextChanged += new System.EventHandler(this.textboxTimeBetweenDisenchants_TextChanged);
            // 
            // labelTimeBetweenDisenchants
            // 
            this.labelTimeBetweenDisenchants.AutoSize = true;
            this.labelTimeBetweenDisenchants.Location = new System.Drawing.Point(4, 138);
            this.labelTimeBetweenDisenchants.Name = "labelTimeBetweenDisenchants";
            this.labelTimeBetweenDisenchants.Size = new System.Drawing.Size(137, 13);
            this.labelTimeBetweenDisenchants.TabIndex = 23;
            this.labelTimeBetweenDisenchants.Text = "Time Between Disenchants";
            // 
            // checkboxDisenchantPurples
            // 
            this.checkboxDisenchantPurples.AutoSize = true;
            this.checkboxDisenchantPurples.Location = new System.Drawing.Point(7, 118);
            this.checkboxDisenchantPurples.Name = "checkboxDisenchantPurples";
            this.checkboxDisenchantPurples.Size = new System.Drawing.Size(118, 17);
            this.checkboxDisenchantPurples.TabIndex = 3;
            this.checkboxDisenchantPurples.Text = "Disenchant Purples";
            this.checkboxDisenchantPurples.UseVisualStyleBackColor = true;
            this.checkboxDisenchantPurples.CheckedChanged += new System.EventHandler(this.checkboxDisenchantPurples_CheckedChanged);
            // 
            // checkboxDisenchantBlues
            // 
            this.checkboxDisenchantBlues.AutoSize = true;
            this.checkboxDisenchantBlues.Location = new System.Drawing.Point(7, 95);
            this.checkboxDisenchantBlues.Name = "checkboxDisenchantBlues";
            this.checkboxDisenchantBlues.Size = new System.Drawing.Size(109, 17);
            this.checkboxDisenchantBlues.TabIndex = 2;
            this.checkboxDisenchantBlues.Text = "Disenchant Blues";
            this.checkboxDisenchantBlues.UseVisualStyleBackColor = true;
            this.checkboxDisenchantBlues.CheckedChanged += new System.EventHandler(this.checkboxDisenchantBlues_CheckedChanged);
            // 
            // checkboxDisenchantGreens
            // 
            this.checkboxDisenchantGreens.AutoSize = true;
            this.checkboxDisenchantGreens.Location = new System.Drawing.Point(7, 72);
            this.checkboxDisenchantGreens.Name = "checkboxDisenchantGreens";
            this.checkboxDisenchantGreens.Size = new System.Drawing.Size(117, 17);
            this.checkboxDisenchantGreens.TabIndex = 1;
            this.checkboxDisenchantGreens.Text = "Disenchant Greens";
            this.checkboxDisenchantGreens.UseVisualStyleBackColor = true;
            this.checkboxDisenchantGreens.CheckedChanged += new System.EventHandler(this.checkboxDisenchantGreens_CheckedChanged);
            // 
            // checkboxDisenchant
            // 
            this.checkboxDisenchant.AutoSize = true;
            this.checkboxDisenchant.Location = new System.Drawing.Point(7, 3);
            this.checkboxDisenchant.Name = "checkboxDisenchant";
            this.checkboxDisenchant.Size = new System.Drawing.Size(130, 17);
            this.checkboxDisenchant.TabIndex = 0;
            this.checkboxDisenchant.Text = "Enable Disenchanting";
            this.checkboxDisenchant.UseVisualStyleBackColor = true;
            this.checkboxDisenchant.CheckedChanged += new System.EventHandler(this.checkboxDisenchant_CheckedChanged);
            // 
            // checkboxDisenchantWeapons
            // 
            this.checkboxDisenchantWeapons.AutoSize = true;
            this.checkboxDisenchantWeapons.Location = new System.Drawing.Point(7, 49);
            this.checkboxDisenchantWeapons.Name = "checkboxDisenchantWeapons";
            this.checkboxDisenchantWeapons.Size = new System.Drawing.Size(129, 17);
            this.checkboxDisenchantWeapons.TabIndex = 28;
            this.checkboxDisenchantWeapons.Text = "Disenchant Weapons";
            this.checkboxDisenchantWeapons.UseVisualStyleBackColor = true;
            this.checkboxDisenchantWeapons.CheckedChanged += new System.EventHandler(this.checkboxDisenchantWeapons_CheckedChanged);
            // 
            // TemplarGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 402);
            this.Controls.Add(this.tabcontrolBotConfig);
            this.Name = "TemplarGUI";
            this.Text = "Templar by AknA and Wigglez";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TemplarGUI_FormClosing);
            this.Load += new System.EventHandler(this.TemplarGUI_Load);
            this.tabcontrolBotConfig.ResumeLayout(false);
            this.tabpageGeneral.ResumeLayout(false);
            this.tabpageGeneral.PerformLayout();
            this.groupboxLooting.ResumeLayout(false);
            this.groupboxLooting.PerformLayout();
            this.groupboxSkinning.ResumeLayout(false);
            this.groupboxSkinning.PerformLayout();
            this.groupboxMultiPull.ResumeLayout(false);
            this.groupboxMultiPull.PerformLayout();
            this.tabpageWhitelist.ResumeLayout(false);
            this.tabpageWhitelist.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.datagridviewWhitelist)).EndInit();
            this.tabpageBlacklist.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.datagridviewBlacklist)).EndInit();
            this.tabpageProtectedItems.ResumeLayout(false);
            this.addItemGroupBox.ResumeLayout(false);
            this.addItemGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.datagridviewProtectedItems)).EndInit();
            this.tabpageMail.ResumeLayout(false);
            this.tabpageMail.PerformLayout();
            this.tabpageVendor.ResumeLayout(false);
            this.tabpageVendor.PerformLayout();
            this.tabpageDisenchant.ResumeLayout(false);
            this.tabpageDisenchant.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabcontrolBotConfig;
        private System.Windows.Forms.TabPage tabpageGeneral;
        private System.Windows.Forms.TabPage tabpageWhitelist;
        private System.Windows.Forms.TabPage tabpageBlacklist;
        private System.Windows.Forms.DataGridView datagridviewWhitelist;
        private System.Windows.Forms.DataGridView datagridviewBlacklist;
        private System.Windows.Forms.Button buttonWhitelistRemove;
        private System.Windows.Forms.Button buttonWhitelistAddTarget;
        private System.Windows.Forms.Button buttonBlacklistRemove;
        private System.Windows.Forms.Button buttonBlacklistAddTarget;
        private System.Windows.Forms.GroupBox groupboxMultiPull;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelMultiPullMobThreshold;
        private System.Windows.Forms.TextBox textboxMultiPullMobThresh;
        private System.Windows.Forms.CheckBox checkboxMultiPull;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelMultiPullHealthThreshold;
        private System.Windows.Forms.TextBox textboxMultiPullHpThresh;
        private System.Windows.Forms.TextBox textboxStartLocationMaxDist;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label labelStartLocationMaxDistance;
        private System.Windows.Forms.TabPage tabpageProtectedItems;
        private System.Windows.Forms.TabPage tabpageMail;
        private System.Windows.Forms.CheckBox checkboxMailPurples;
        private System.Windows.Forms.CheckBox checkboxMailBlues;
        private System.Windows.Forms.CheckBox checkboxMailGreens;
        private System.Windows.Forms.CheckBox checkboxMailWhites;
        private System.Windows.Forms.CheckBox checkboxMail;
        private System.Windows.Forms.TextBox textboxRecipient;
        private System.Windows.Forms.Label labelMailRecipient;
        private System.Windows.Forms.CheckBox checkboxIgnoreEliteMobs;
        private System.Windows.Forms.TextBox textboxProtectedItemName;
        private System.Windows.Forms.Label labelProtectedItemsName;
        private System.Windows.Forms.Label labelProtectedItemsID;
        private System.Windows.Forms.Button buttonProtectedItemAddItem;
        private System.Windows.Forms.TextBox textboxProtectedItemID;
        private System.Windows.Forms.Button buttonProtectedItemRemove;
        private System.Windows.Forms.DataGridView datagridviewProtectedItems;
        private System.Windows.Forms.GroupBox addItemGroupBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonWhitelistAddUnit;
        private System.Windows.Forms.Label labelWhitelistID;
        private System.Windows.Forms.Label labelWhitelistName;
        private System.Windows.Forms.TextBox textboxWhitelistID;
        private System.Windows.Forms.TextBox textboxWhitelistName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonBlacklistAddUnit;
        private System.Windows.Forms.Label labelBlacklistID;
        private System.Windows.Forms.Label labelBlacklistName;
        private System.Windows.Forms.TextBox textboxBlacklistID;
        private System.Windows.Forms.TextBox textboxBlacklistName;
        private System.Windows.Forms.CheckBox checkboxLootMobs;
        private System.Windows.Forms.CheckBox checkboxAttackOnlyWhitelist;
        private System.Windows.Forms.CheckBox checkboxNinjaSkin;
        private System.Windows.Forms.CheckBox checkboxSkinMobs;
        private System.Windows.Forms.GroupBox groupboxLooting;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textboxTimeBetweenLoots;
        private System.Windows.Forms.Label labelLootingTimeBetweenLoots;
        private System.Windows.Forms.GroupBox groupboxSkinning;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textboxTimeBetweenSkins;
        private System.Windows.Forms.Label labelSkinningTimeBetweenSkins;
        private System.Windows.Forms.TabPage tabpageVendor;
        private System.Windows.Forms.CheckBox checkboxVendor;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label labelRepairAtPercent;
        private System.Windows.Forms.TextBox textboxRepairDurability;
        private System.Windows.Forms.CheckBox checkboxSellPurples;
        private System.Windows.Forms.CheckBox checkboxSellBlues;
        private System.Windows.Forms.CheckBox checkboxSellGreens;
        private System.Windows.Forms.CheckBox checkboxSellWhites;
        private System.Windows.Forms.CheckBox checkboxSellGrays;
        private System.Windows.Forms.TabPage tabpageDisenchant;
        private System.Windows.Forms.TextBox textboxBagSlotsRemaining;
        private System.Windows.Forms.Label labelMinimumBagSlots;
        private System.Windows.Forms.CheckBox checkboxDisenchant;
        private System.Windows.Forms.CheckBox checkboxDisenchantPurples;
        private System.Windows.Forms.CheckBox checkboxDisenchantBlues;
        private System.Windows.Forms.CheckBox checkboxDisenchantGreens;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textboxTimeBetweenDisenchants;
        private System.Windows.Forms.Label labelTimeBetweenDisenchants;
        private System.Windows.Forms.CheckBox checkboxDisenchantOnlyNewItems;
        private System.Windows.Forms.CheckBox checkboxDisenchantWeapons;
    }
}