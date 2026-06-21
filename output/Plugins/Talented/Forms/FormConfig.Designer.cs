namespace Talented.Forms
{
    partial class FormConfig
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
            this.label1 = new System.Windows.Forms.Label();
            this.lbTalents = new System.Windows.Forms.ListBox();
            this.btnSaveAndClose = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblName = new System.Windows.Forms.Label();
            this.lblClass = new System.Windows.Forms.Label();
            this.lbTalentBuilds = new System.Windows.Forms.ListBox();
            this.btnDump = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(190, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Talents:";
            // 
            // lbTalents
            // 
            this.lbTalents.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbTalents.FormattingEnabled = true;
            this.lbTalents.Location = new System.Drawing.Point(193, 64);
            this.lbTalents.Name = "lbTalents";
            this.lbTalents.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbTalents.Size = new System.Drawing.Size(181, 173);
            this.lbTalents.TabIndex = 17;
            // 
            // btnSaveAndClose
            // 
            this.btnSaveAndClose.Location = new System.Drawing.Point(94, 243);
            this.btnSaveAndClose.Name = "btnSaveAndClose";
            this.btnSaveAndClose.Size = new System.Drawing.Size(90, 23);
            this.btnSaveAndClose.TabIndex = 15;
            this.btnSaveAndClose.Text = "Save and Close";
            this.btnSaveAndClose.UseVisualStyleBackColor = true;
            this.btnSaveAndClose.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(12, 243);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 14;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(190, 27);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(41, 13);
            this.lblName.TabIndex = 13;
            this.lblName.Text = "Name: ";
            // 
            // lblClass
            // 
            this.lblClass.AutoSize = true;
            this.lblClass.Location = new System.Drawing.Point(190, 12);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(38, 13);
            this.lblClass.TabIndex = 11;
            this.lblClass.Text = "Class: ";
            // 
            // lbTalentBuilds
            // 
            this.lbTalentBuilds.FormattingEnabled = true;
            this.lbTalentBuilds.Location = new System.Drawing.Point(12, 12);
            this.lbTalentBuilds.Name = "lbTalentBuilds";
            this.lbTalentBuilds.Size = new System.Drawing.Size(172, 225);
            this.lbTalentBuilds.TabIndex = 10;
            this.lbTalentBuilds.SelectedIndexChanged += new System.EventHandler(this.lbTalentBuilds_SelectedIndexChanged);
            // 
            // btnDump
            // 
            this.btnDump.Location = new System.Drawing.Point(193, 241);
            this.btnDump.Name = "btnDump";
            this.btnDump.Size = new System.Drawing.Size(181, 23);
            this.btnDump.TabIndex = 19;
            this.btnDump.Text = "Dump current build to clipboard";
            this.btnDump.UseVisualStyleBackColor = true;
            this.btnDump.Click += new System.EventHandler(this.btnDump_Click);
            // 
            // FormConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 276);
            this.Controls.Add(this.btnDump);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbTalents);
            this.Controls.Add(this.btnSaveAndClose);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblClass);
            this.Controls.Add(this.lbTalentBuilds);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfig";
            this.Text = "Select Talent Set";
            this.Load += new System.EventHandler(this.FormConfig_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbTalents;
        private System.Windows.Forms.Button btnSaveAndClose;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblClass;
        private System.Windows.Forms.ListBox lbTalentBuilds;
        private System.Windows.Forms.Button btnDump;
    }
}