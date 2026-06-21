using System.Drawing;
using System.Windows.Forms;

namespace ObjectGatherer.GUI {
    partial class ObjectGathererGUI {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.checkboxGatherOre = new System.Windows.Forms.CheckBox();
            this.checkboxGatherHerbs = new System.Windows.Forms.CheckBox();
            this.checkboxLootCorpses = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.searchGridView = new GUI.OutlookGrid.OutlookGrid();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.idTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.addEntryButton = new System.Windows.Forms.Button();
            this.categoryComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(1, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(519, 352);
            this.tabControl1.TabIndex = 7;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.checkboxGatherOre);
            this.tabPage1.Controls.Add(this.checkboxGatherHerbs);
            this.tabPage1.Controls.Add(this.checkboxLootCorpses);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(511, 326);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Settings";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkboxGatherOre
            // 
            this.checkboxGatherOre.AutoSize = true;
            this.checkboxGatherOre.Location = new System.Drawing.Point(8, 53);
            this.checkboxGatherOre.Name = "checkboxGatherOre";
            this.checkboxGatherOre.Size = new System.Drawing.Size(78, 17);
            this.checkboxGatherOre.TabIndex = 2;
            this.checkboxGatherOre.Text = "Gather Ore";
            this.checkboxGatherOre.UseVisualStyleBackColor = true;
            this.checkboxGatherOre.CheckedChanged += new System.EventHandler(this.checkboxGatherOre_CheckedChanged);
            // 
            // checkboxGatherHerbs
            // 
            this.checkboxGatherHerbs.AutoSize = true;
            this.checkboxGatherHerbs.Location = new System.Drawing.Point(8, 30);
            this.checkboxGatherHerbs.Name = "checkboxGatherHerbs";
            this.checkboxGatherHerbs.Size = new System.Drawing.Size(89, 17);
            this.checkboxGatherHerbs.TabIndex = 1;
            this.checkboxGatherHerbs.Text = "Gather Herbs";
            this.checkboxGatherHerbs.UseVisualStyleBackColor = true;
            this.checkboxGatherHerbs.CheckedChanged += new System.EventHandler(this.checkboxGatherHerbs_CheckedChanged);
            // 
            // checkboxLootCorpses
            // 
            this.checkboxLootCorpses.AutoSize = true;
            this.checkboxLootCorpses.Location = new System.Drawing.Point(8, 7);
            this.checkboxLootCorpses.Name = "checkboxLootCorpses";
            this.checkboxLootCorpses.Size = new System.Drawing.Size(88, 17);
            this.checkboxLootCorpses.TabIndex = 0;
            this.checkboxLootCorpses.Text = "Loot Corpses";
            this.checkboxLootCorpses.UseVisualStyleBackColor = true;
            this.checkboxLootCorpses.CheckedChanged += new System.EventHandler(this.checkboxLootCorpses_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.searchGridView);
            this.tabPage2.Controls.Add(this.nameTextBox);
            this.tabPage2.Controls.Add(this.idTextBox);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.addEntryButton);
            this.tabPage2.Controls.Add(this.categoryComboBox);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(511, 326);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Objects";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // searchGridView
            // 
            this.searchGridView.AllowUserToAddRows = false;
            this.searchGridView.AllowUserToDeleteRows = false;
            this.searchGridView.AllowUserToOrderColumns = false;
            this.searchGridView.AllowUserToResizeColumns = false;
            this.searchGridView.AllowUserToResizeRows = false;
            this.searchGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.Black;
            this.searchGridView.AlternatingRowsDefaultCellStyle.ForeColor = Color.DeepSkyBlue;
            this.searchGridView.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
            this.searchGridView.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;
            this.searchGridView.BackgroundColor = Color.FromArgb(25, 25, 25);
            this.searchGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
            this.searchGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.DeepSkyBlue;
            this.searchGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
            this.searchGridView.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            this.searchGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.searchGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.searchGridView.DefaultCellStyle.BackColor = Color.Black;
            this.searchGridView.DefaultCellStyle.ForeColor = Color.White;
            this.searchGridView.DefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
            this.searchGridView.DefaultCellStyle.SelectionForeColor = Color.White;
            this.searchGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.searchGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            this.searchGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.searchGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.searchGridView.EnableHeadersVisualStyles = false;
            this.searchGridView.GridColor = Color.White;
            this.searchGridView.RowHeadersDefaultCellStyle.BackColor = Color.Black;
            this.searchGridView.RowHeadersDefaultCellStyle.ForeColor = Color.DeepSkyBlue;
            this.searchGridView.RowHeadersDefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
            this.searchGridView.RowHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            this.searchGridView.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.searchGridView.RowHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.searchGridView.RowHeadersVisible = false;
            this.searchGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.searchGridView.BackgroundColor = System.Drawing.Color.Gray;
            this.searchGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.searchGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.searchGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.searchGridView.Location = new System.Drawing.Point(3, 3);
            this.searchGridView.Name = "searchGridView";
            this.searchGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.searchGridView.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.searchGridView.Size = new System.Drawing.Size(499, 214);
            this.searchGridView.TabIndex = 7;
            this.searchGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.searchGridView_CellContentClick);
            this.searchGridView.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.searchGridView_CellMouseUp);
            this.searchGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.searchGridView_DataError);
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(114, 246);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(151, 20);
            this.nameTextBox.TabIndex = 10;
            // 
            // idTextBox
            // 
            this.idTextBox.Location = new System.Drawing.Point(114, 220);
            this.idTextBox.Name = "idTextBox";
            this.idTextBox.Size = new System.Drawing.Size(151, 20);
            this.idTextBox.TabIndex = 9;
            this.idTextBox.TextChanged += new System.EventHandler(this.idTextBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 249);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Name:";
            // 
            // addEntryButton
            // 
            this.addEntryButton.ForeColor = Color.Black;
            this.addEntryButton.BackColor = Color.DeepSkyBlue;
            this.addEntryButton.FlatStyle = FlatStyle.Popup;
            this.addEntryButton.Location = new System.Drawing.Point(3, 299);
            this.addEntryButton.Name = "addEntryButton";
            this.addEntryButton.Size = new System.Drawing.Size(75, 23);
            this.addEntryButton.TabIndex = 13;
            this.addEntryButton.Text = "Add Entry";
            this.addEntryButton.UseVisualStyleBackColor = true;
            this.addEntryButton.Click += new System.EventHandler(this.addEntryButton_Click);
            // 
            // categoryComboBox
            // 
            this.categoryComboBox.FormattingEnabled = true;
            this.categoryComboBox.Location = new System.Drawing.Point(114, 272);
            this.categoryComboBox.Name = "categoryComboBox";
            this.categoryComboBox.Size = new System.Drawing.Size(151, 21);
            this.categoryComboBox.TabIndex = 12;
            this.categoryComboBox.DropDown += new System.EventHandler(this.categoryComboBox_DropDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 223);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "ID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 275);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Add/Select Category:";
            // 
            // ObjectGathererGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 355);
            this.Controls.Add(this.tabControl1);
            this.Name = "ObjectGathererGUI";
            this.Text = "ObjectGatherer by AknA and Wigglez";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ObjectGathererGUI_FormClosing);
            this.Load += new System.EventHandler(this.ObjectGathererGUI_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox checkboxGatherOre;
        private System.Windows.Forms.CheckBox checkboxGatherHerbs;
        private System.Windows.Forms.CheckBox checkboxLootCorpses;
        private System.Windows.Forms.TabPage tabPage2;
        private OutlookGrid.OutlookGrid searchGridView;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.TextBox idTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button addEntryButton;
        private System.Windows.Forms.ComboBox categoryComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;

    }
}