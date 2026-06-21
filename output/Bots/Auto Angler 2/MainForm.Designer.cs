namespace HighVoltz
{
    partial class MainForm
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
            this.WpfHost = new System.Windows.Forms.Integration.ElementHost();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.DonateButton = new System.Windows.Forms.Button();
            this.RepButton = new System.Windows.Forms.Button();
            this.MailButton = new System.Windows.Forms.Button();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // WpfHost
            // 
            this.WpfHost.Location = new System.Drawing.Point(0, 0);
            this.WpfHost.Name = "WpfHost";
            this.WpfHost.Size = new System.Drawing.Size(200, 100);
            this.WpfHost.TabIndex = 2;
            this.WpfHost.Child = null;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.propertyGrid, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(344, 376);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Controls.Add(this.DonateButton, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.RepButton, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.MailButton, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 349);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(338, 24);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // DonateButton
            // 
            this.DonateButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DonateButton.Location = new System.Drawing.Point(3, 3);
            this.DonateButton.Name = "DonateButton";
            this.DonateButton.Size = new System.Drawing.Size(106, 20);
            this.DonateButton.TabIndex = 0;
            this.DonateButton.Text = "Donate";
            this.DonateButton.UseVisualStyleBackColor = true;
            this.DonateButton.Click += new System.EventHandler(this.DonateButton_Click);
            // 
            // RepButton
            // 
            this.RepButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RepButton.Location = new System.Drawing.Point(115, 3);
            this.RepButton.Name = "RepButton";
            this.RepButton.Size = new System.Drawing.Size(106, 20);
            this.RepButton.TabIndex = 0;
            this.RepButton.Text = "Give Reputation";
            this.RepButton.UseVisualStyleBackColor = true;
            this.RepButton.Click += new System.EventHandler(this.RepButton_Click);
            // 
            // MailButton
            // 
            this.MailButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MailButton.Location = new System.Drawing.Point(227, 3);
            this.MailButton.Name = "MailButton";
            this.MailButton.Size = new System.Drawing.Size(108, 20);
            this.MailButton.TabIndex = 1;
            this.MailButton.Text = "Force Mail";
            this.MailButton.UseVisualStyleBackColor = true;
            this.MailButton.Click += new System.EventHandler(this.MailButton_Click);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(338, 340);
            this.propertyGrid.TabIndex = 2;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 376);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.WpfHost);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainForm";
            this.Text = "AutoAngler2 Config";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost WpfHost;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button DonateButton;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Button RepButton;
        private System.Windows.Forms.Button MailButton;
    }
}