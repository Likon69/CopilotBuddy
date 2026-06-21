namespace Styx.Bot.CustomBots
{
    partial class SelectTankForm
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
            this.colMaxHealth = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSetLeader = new System.Windows.Forms.Button();
            this.colRole = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colClass = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnRefresh = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numFollowDistance = new System.Windows.Forms.NumericUpDown();
            this.lblFollowDistance = new System.Windows.Forms.Label();
            this.chkRunWithoutTank = new System.Windows.Forms.CheckBox();
            this.chkAutoFollow = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFollowDistance)).BeginInit();
            this.SuspendLayout();
            // 
            // colMaxHealth
            // 
            this.colMaxHealth.Text = "Health";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(343, 47);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSetLeader
            // 
            this.btnSetLeader.Location = new System.Drawing.Point(342, 17);
            this.btnSetLeader.Name = "btnSetLeader";
            this.btnSetLeader.Size = new System.Drawing.Size(75, 23);
            this.btnSetLeader.TabIndex = 1;
            this.btnSetLeader.Text = "Set Tank";
            this.btnSetLeader.UseVisualStyleBackColor = true;
            this.btnSetLeader.Click += new System.EventHandler(this.btnSetLeader_Click);
            // 
            // colRole
            // 
            this.colRole.Text = "Role";
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colClass,
            this.colRole,
            this.colMaxHealth});
            this.listView.FullRowSelect = true;
            this.listView.HideSelection = false;
            this.listView.LabelWrap = false;
            this.listView.Location = new System.Drawing.Point(12, 101);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.ShowGroups = false;
            this.listView.Size = new System.Drawing.Size(314, 224);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            this.listView.Click += new System.EventHandler(this.listView_Click);
            this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 120;
            // 
            // colClass
            // 
            this.colClass.Text = "Class";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(342, 101);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numFollowDistance);
            this.groupBox1.Controls.Add(this.lblFollowDistance);
            this.groupBox1.Controls.Add(this.chkRunWithoutTank);
            this.groupBox1.Controls.Add(this.chkAutoFollow);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(314, 65);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // numFollowDistance
            // 
            this.numFollowDistance.Location = new System.Drawing.Point(254, 34);
            this.numFollowDistance.Name = "numFollowDistance";
            this.numFollowDistance.Size = new System.Drawing.Size(45, 20);
            this.numFollowDistance.TabIndex = 3;
            this.numFollowDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblFollowDistance
            // 
            this.lblFollowDistance.AutoSize = true;
            this.lblFollowDistance.Location = new System.Drawing.Point(171, 36);
            this.lblFollowDistance.Name = "lblFollowDistance";
            this.lblFollowDistance.Size = new System.Drawing.Size(82, 13);
            this.lblFollowDistance.TabIndex = 2;
            this.lblFollowDistance.Text = "Follow Distance";
            // 
            // chkRunWithoutTank
            // 
            this.chkRunWithoutTank.AutoSize = true;
            this.chkRunWithoutTank.Location = new System.Drawing.Point(6, 14);
            this.chkRunWithoutTank.Name = "chkRunWithoutTank";
            this.chkRunWithoutTank.Size = new System.Drawing.Size(176, 17);
            this.chkRunWithoutTank.TabIndex = 0;
            this.chkRunWithoutTank.Text = "Run Without a Tank (no leader)";
            this.chkRunWithoutTank.UseVisualStyleBackColor = true;
            this.chkRunWithoutTank.CheckedChanged += new System.EventHandler(this.chkDisableTank_CheckedChanged);
            // 
            // chkAutoFollow
            // 
            this.chkAutoFollow.AutoSize = true;
            this.chkAutoFollow.Location = new System.Drawing.Point(6, 35);
            this.chkAutoFollow.Name = "chkAutoFollow";
            this.chkAutoFollow.Size = new System.Drawing.Size(149, 17);
            this.chkAutoFollow.TabIndex = 1;
            this.chkAutoFollow.Text = "Automatically Follow Tank";
            this.chkAutoFollow.UseVisualStyleBackColor = true;
            // 
            // SelectTankForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 371);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSetLeader);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.btnRefresh);
            this.Name = "SelectTankForm";
            this.Text = "LazyRaider - Set the Tank";
            this.Activated += new System.EventHandler(this.SelectTankForm_Activated);
            this.Deactivate += new System.EventHandler(this.SelectTankForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectTankForm_FormClosing);
            this.Shown += new System.EventHandler(this.SelectTankForm_Shown);
            this.VisibleChanged += new System.EventHandler(this.SelectTankForm_VisibleChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFollowDistance)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader colMaxHealth;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSetLeader;
        private System.Windows.Forms.ColumnHeader colRole;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colClass;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numFollowDistance;
        private System.Windows.Forms.Label lblFollowDistance;
        private System.Windows.Forms.CheckBox chkAutoFollow;
        private System.Windows.Forms.CheckBox chkRunWithoutTank;

    }
}