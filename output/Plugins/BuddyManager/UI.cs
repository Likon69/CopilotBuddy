using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Styx;
using Styx.Helpers;

namespace BuddyManager
{
    public partial class UI : Form
    {
        //BuddyManager._BotPath AutoAngler2 = new BuddyManager._BotPath("AutoAngler2");
        public List<string> NonProfileBB = new List<string> { "BG Bot [Beta]", "PvP", "ArchaeologyBuddy", "Instancebuddy"};
        public UI()
        {
            InitializeComponent(); 
        }
        private void DrBbConfig_Load(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.Load();

            foreach (KeyValuePair<string, Styx.BotBase> BB in BotManager.Instance.Bots)
            {
                if (!BB.Key.ToLower().Equals("pvp") && !BB.Key.ToLower().Equals("mixed mode") && !BB.Key.ToLower().Equals("combat bot") && !BB.Key.ToLower().Equals("lazyraider") && !BB.Key.ToLower().Equals("partybot") && !BB.Key.ToLower().Equals("questing"))
                {
                    if (!BBcb1.Items.Contains(BB.Key))
                    {
                        BBcb1.Items.Add(BB.Key);
                    }
                    if (!BBcb2.Items.Contains(BB.Key))
                    {
                        BBcb2.Items.Add(BB.Key);
                    }
                    if (!BBcb3.Items.Contains(BB.Key))
                    {
                        BBcb3.Items.Add(BB.Key);
                    }
                    if (!BBcb4.Items.Contains(BB.Key))
                    {
                        BBcb4.Items.Add(BB.Key);
                    }
                }
            }

            BBcb1.SelectedItem = BuddyManagerSettings.Instance.SelectedBB1;
            BBcb2.SelectedItem = BuddyManagerSettings.Instance.SelectedBB2;
            BBcb3.SelectedItem = BuddyManagerSettings.Instance.SelectedBB3;
            BBcb4.SelectedItem = BuddyManagerSettings.Instance.SelectedBB4;
            SelectedProfile1.Text = BuddyManagerSettings.Instance.SelectedProfile1;
            SelectedProfile2.Text = BuddyManagerSettings.Instance.SelectedProfile2;
            SelectedProfile3.Text = BuddyManagerSettings.Instance.SelectedProfile3;
            SelectedProfile4.Text = BuddyManagerSettings.Instance.SelectedProfile4;
            ThreeEnabled.Checked = BuddyManagerSettings.Instance.ThreeEnabled;
            FourEnabled.Checked = BuddyManagerSettings.Instance.FourEnabled;
            LogOut.Checked = BuddyManagerSettings.Instance.LogOutAfter;
            LoopAll.Checked = BuddyManagerSettings.Instance.LoopAll;
            LoopHours.Value = BuddyManagerSettings.Instance.LoopHours;
            LoopMins.Value = BuddyManagerSettings.Instance.LoopMins;
            P1Hours.Value = BuddyManagerSettings.Instance.P1Hours;
            P2Hours.Value = BuddyManagerSettings.Instance.P2Hours;
            P3Hours.Value = BuddyManagerSettings.Instance.P3Hours;
            P4Hours.Value = BuddyManagerSettings.Instance.P4Hours;
            P1Mins.Value = BuddyManagerSettings.Instance.P1Mins;
            P2Mins.Value = BuddyManagerSettings.Instance.P2Mins;
            P3Mins.Value = BuddyManagerSettings.Instance.P3Mins;
            P4Mins.Value = BuddyManagerSettings.Instance.P4Mins;
            Zone1.SelectedItem = BuddyManagerSettings.Instance.Zone1;
            Zone2.SelectedItem = BuddyManagerSettings.Instance.Zone2;
            Zone3.SelectedItem = BuddyManagerSettings.Instance.Zone3;
            Zone4.SelectedItem = BuddyManagerSettings.Instance.Zone4;
            repair.Checked = BuddyManagerSettings.Instance.Repair;
            sell.Checked = BuddyManagerSettings.Instance.Sell;
            mail.Checked = BuddyManagerSettings.Instance.Mail;
            hearthrms.Checked = BuddyManagerSettings.Instance.HearthRMS;


            

            if (Styx.StyxWoW.Me != null && Styx.StyxWoW.Me.IsAlliance) { hearthrms.Enabled = false; sell.Enabled = false; mail.Enabled = false; repair.Enabled = false; }
            else { hearthrms.Enabled = true; sell.Enabled = true; mail.Enabled = true; repair.Enabled = true; }

            if (ThreeEnabled.Checked)
            {
                SelectedProfile3.Enabled = true;
                BBcb3.Enabled = true;
                P3Hours.Enabled = true;
                P3Mins.Enabled = true;
                Zone3.Enabled = true;
            }
            else
            {
                SelectedProfile3.Enabled = false;
                BBcb3.Enabled = false;
                P3Hours.Enabled = false;
                P3Mins.Enabled = false;
                Zone3.Enabled = false;
            }
            if (FourEnabled.Checked)
            {
                SelectedProfile4.Enabled = true;
                BBcb4.Enabled = true;
                P4Hours.Enabled = true;
                P4Mins.Enabled = true;
                Zone4.Enabled = true;
            }
            else
            {
                SelectedProfile4.Enabled = false;
                BBcb4.Enabled = false;
                P4Hours.Enabled = false;
                P4Mins.Enabled = false;
                Zone4.Enabled = false;
            }
            if (BBcb1.SelectedItem != null && NonProfileBB.Contains(BBcb1.SelectedItem.ToString()))
            {
                SelectedProfile1.Enabled = false;
            }
            else { SelectedProfile1.Enabled = true; }
            if (BBcb2.SelectedItem != null && NonProfileBB.Contains(BBcb2.SelectedItem.ToString()))
            {
                SelectedProfile2.Enabled = false;
            }
            else { SelectedProfile2.Enabled = true; }
            if (BBcb3.SelectedItem != null && NonProfileBB.Contains(BBcb3.SelectedItem.ToString()))
            {
                SelectedProfile3.Enabled = false;
            }
            else { SelectedProfile3.Enabled = true; }
            if (BBcb4.SelectedItem != null && NonProfileBB.Contains(BBcb4.SelectedItem.ToString()))
            {
                SelectedProfile4.Enabled = false;
            }
            else { SelectedProfile4.Enabled = true; }

        }
        private void Save_Click(object sender, EventArgs e)
        {
            Logging.Write(System.Drawing.Color.Red, "[BuddyManager] You saved settings.  If you changed something please click \"Recompile All\" in the Plugins window.");
            BuddyManagerSettings.Instance.Save();
        }


        private void FourEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (FourEnabled.Checked)
            {
                SelectedProfile4.Enabled = true;
                BBcb4.Enabled = true;
                P4Hours.Enabled = true;
                P4Mins.Enabled = true;
                Zone4.Enabled = true;
                BuddyManagerSettings.Instance.FourEnabled = true;
            }
            else
            {
                Zone4.SelectedItem = "None";
                SelectedProfile4.Text = "None";
                SelectedProfile4.Enabled = false;
                BBcb4.Enabled = false;
                P4Hours.Enabled = false;
                P4Mins.Enabled = false;
                Zone4.Enabled = false;
                BuddyManagerSettings.Instance.FourEnabled = false;
            }
        }

        private void ThreeEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (ThreeEnabled.Checked)
            {
                SelectedProfile3.Enabled = true;
                BBcb3.Enabled = true;
                P3Hours.Enabled = true;
                P3Mins.Enabled = true;
                Zone3.Enabled = true;
                BuddyManagerSettings.Instance.ThreeEnabled = true;
            }
            else
            {
                Zone3.SelectedItem = "None";
                SelectedProfile3.Text = "None";
                SelectedProfile3.Enabled = false;
                BBcb3.Enabled = false;
                P3Hours.Enabled = false;
                P3Mins.Enabled = false;
                Zone3.Enabled = false;
                BuddyManagerSettings.Instance.ThreeEnabled = false;
            }
        }

        private void SelectedProfile1_DoubleClick(object sender, EventArgs e)
        {
            var SP1 = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (SP1.ShowDialog() == DialogResult.OK)
            {
                string profile1 = SP1.FileName;
                if (!string.IsNullOrEmpty(profile1))
                {
                    SelectedProfile1.Text = profile1;
                    BuddyManagerSettings.Instance.SelectedProfile1 = SelectedProfile1.Text;
                }
            }
        }

        private void SelectedProfile2_DoubleClick(object sender, EventArgs e)
        {
            var SP2 = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (SP2.ShowDialog() == DialogResult.OK)
            {
                string profile2 = SP2.FileName;
                if (!string.IsNullOrEmpty(profile2))
                {
                    SelectedProfile2.Text = profile2;
                    BuddyManagerSettings.Instance.SelectedProfile2 = SelectedProfile2.Text;
                }
            }
        }

        private void SelectedProfile3_DoubleClick(object sender, EventArgs e)
        {
            var SP3 = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (SP3.ShowDialog() == DialogResult.OK)
            {
                string profile3 = SP3.FileName;
                if (!string.IsNullOrEmpty(profile3))
                {
                    SelectedProfile3.Text = profile3;
                    BuddyManagerSettings.Instance.SelectedProfile3 = SelectedProfile3.Text;
                }
            }
        }

        private void SelectedProfile4_DoubleClick(object sender, EventArgs e)
        {
            var SP4 = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (SP4.ShowDialog() == DialogResult.OK)
            {
                string profile4 = SP4.FileName;
                if (!string.IsNullOrEmpty(profile4))
                {
                    SelectedProfile4.Text = profile4;
                    BuddyManagerSettings.Instance.SelectedProfile4 = SelectedProfile4.Text;
                }
            }
        }

        private void BBcb1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(NonProfileBB.Contains(BBcb1.SelectedItem.ToString()))
            {
                Zone1.SelectedItem = "None";
                SelectedProfile1.Text = "None";
                SelectedProfile1.Enabled = false;
                Zone1.Enabled = false;
            }
            else { SelectedProfile1.Enabled = true; Zone1.Enabled = true; }
            BuddyManagerSettings.Instance.SelectedBB1 = BBcb1.SelectedItem.ToString();
        }

        private void BBcb2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NonProfileBB.Contains(BBcb2.SelectedItem.ToString()))
            {
                Zone2.SelectedItem = "None";
                SelectedProfile2.Text = "None";
                SelectedProfile2.Enabled = false;
                Zone2.Enabled = false;
            }
            else { SelectedProfile2.Enabled = true; Zone2.Enabled = true; }
            BuddyManagerSettings.Instance.SelectedBB2 = BBcb2.SelectedItem.ToString();
        }

        private void BBcb3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NonProfileBB.Contains(BBcb3.SelectedItem.ToString()))
            {
                Zone3.SelectedItem = "None";
                SelectedProfile3.Text = "None";
                SelectedProfile3.Enabled = false;
                Zone3.Enabled = false;
            }
            else { SelectedProfile3.Enabled = true; Zone3.Enabled = true; }
            BuddyManagerSettings.Instance.SelectedBB3 = BBcb3.SelectedItem.ToString();
        }

        private void BBcb4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NonProfileBB.Contains(BBcb4.SelectedItem.ToString()))
            {
                Zone4.SelectedItem = "None";
                SelectedProfile4.Text = "None";
                SelectedProfile4.Enabled = false;
                Zone4.Enabled = false;
            }
            else { SelectedProfile4.Enabled = true; Zone4.Enabled = true; }
            BuddyManagerSettings.Instance.SelectedBB4 = BBcb4.SelectedItem.ToString();
        }

        private void P1Hours_ValueChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.P1Hours = Convert.ToInt32(P1Hours.Value);
        }

        private void P2Hours_ValueChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.P2Hours = Convert.ToInt32(P2Hours.Value);
        }

        private void P3Hours_ValueChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.P3Hours = Convert.ToInt32(P3Hours.Value);
        }

        private void P4Hours_ValueChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.P4Hours = Convert.ToInt32(P4Hours.Value);
        }

        private void P1Mins_ValueChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.P1Mins = Convert.ToInt32(P1Mins.Value);
        }

        private void P2Mins_ValueChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.P2Mins = Convert.ToInt32(P2Mins.Value);
        }

        private void P3Mins_ValueChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.P3Mins = Convert.ToInt32(P3Mins.Value);
        }

        private void P4Mins_ValueChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.P4Mins = Convert.ToInt32(P4Mins.Value);
        }

        private void LogOut_CheckedChanged_1(object sender, EventArgs e)
        {
            if (LogOut.Checked)
            {
                LoopAll.Enabled = false;
                LoopHours.Enabled = false;
                LoopMins.Enabled = false;
                BuddyManagerSettings.Instance.LogOutAfter = true;
                BuddyManagerSettings.Instance.LoopAll = false;
            }
            else
            {
                LoopAll.Enabled = true;
                LoopHours.Enabled = true;
                LoopMins.Enabled = true;
                BuddyManagerSettings.Instance.LogOutAfter = false;
                BuddyManagerSettings.Instance.LoopAll = true;
            }
        }

        private void LoopAll_CheckedChanged_1(object sender, EventArgs e)
        {
            if (LoopAll.Checked)
            {
                LogOut.Enabled = false;
                BuddyManagerSettings.Instance.LoopAll = true;
                BuddyManagerSettings.Instance.LogOutAfter = false;
            }
            else
            {
                LogOut.Enabled = true;
                BuddyManagerSettings.Instance.LoopAll = false;
                BuddyManagerSettings.Instance.LogOutAfter = true;
            }
        }

        private void LoopHours_ValueChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.LoopHours = Convert.ToInt32(LoopHours.Value);
        }

        private void LoopMins_ValueChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.LoopMins = Convert.ToInt32(LoopMins.Value);
        }

        private void Zone1_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.Zone1 = Zone1.SelectedItem.ToString();
        }

        private void Zone2_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.Zone2 = Zone2.SelectedItem.ToString();
        }

        private void Zone3_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.Zone3 = Zone3.SelectedItem.ToString();
        }

        private void Zone4_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.Zone4 = Zone4.SelectedItem.ToString();
        }

        private void SaveExit_Click(object sender, EventArgs e)
        {
            Logging.Write(System.Drawing.Color.Red, "[BuddyManager] You saved settings.  If you changed something please click \"Recompile All\" in the Plugins window.");
            BuddyManagerSettings.Instance.Save();
            UI.ActiveForm.Close();
        }

        private void repair_CheckedChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.Repair = repair.Checked;
        }

        private void mail_CheckedChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.Mail = mail.Checked;
        }

        private void sell_CheckedChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.Sell = sell.Checked;
        }

        private void hearthrms_CheckedChanged(object sender, EventArgs e)
        {
            BuddyManagerSettings.Instance.HearthRMS = hearthrms.Checked;
        }


        private Form _Exampletest;
        private void add_test_Click(object sender, EventArgs e)
        {
            if (_Exampletest == null || _Exampletest.IsDisposed || _Exampletest.Disposing)
                _Exampletest = new Example();

            _Exampletest.ShowDialog();
        }


        ///////////////////////////////////////////////////////////////////////////////////////
        public partial class Example : Form
        {
            public Example()
            {
                InitializeComponent();
            }

            private void add_Click(object sender, EventArgs e)
            {
                item n = new item();
                //copy whatever 
                n.BotBase = this.BBcb1.Text;
                n.Profile = this.SelectedProfile1.Text;
                n.Zone = this.Zone1.Text;
                n.Hours = this.P1Hours.Value.ToString();
                n.Minutes = this.P1Mins.Value.ToString();

                //save it in the list 
                routes.Items.Add(new ListViewItem(n.columns));

                //reset the screen 
            }

            [STAThread]
            static void Main()
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Example());

            }

            private void BBcb1_SelectedIndexChanged(object sender, EventArgs e)
            {

            }

            private void rem_Click(object sender, EventArgs e)
            {
                if (routes.SelectedItems.Count > 0)
                {
                    for (int i = 0; i < routes.SelectedItems.Count; i++)
                    {
                        routes.Items.Remove(routes.SelectedItems[i]);
                    }
                }

            }
        }

        partial class Example
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
                this.groupBox1 = new System.Windows.Forms.GroupBox();
                this.Zone1 = new System.Windows.Forms.ComboBox();
                this.label12 = new System.Windows.Forms.Label();
                this.label11 = new System.Windows.Forms.Label();
                this.P1Mins = new System.Windows.Forms.NumericUpDown();
                this.P1Hours = new System.Windows.Forms.NumericUpDown();
                this.label10 = new System.Windows.Forms.Label();
                this.label4 = new System.Windows.Forms.Label();
                this.SelectedProfile1 = new System.Windows.Forms.TextBox();
                this.BBcb1 = new System.Windows.Forms.ComboBox();
                this.routes = new System.Windows.Forms.ListView();
                this.botBase = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
                this.profile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
                this.zone = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
                this.runTimeH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
                this.runTimeM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
                this.add = new System.Windows.Forms.Button();
                this.rem = new System.Windows.Forms.Button();
                this.groupBox1.SuspendLayout();
                ((System.ComponentModel.ISupportInitialize)(this.P1Mins)).BeginInit();
                ((System.ComponentModel.ISupportInitialize)(this.P1Hours)).BeginInit();
                this.SuspendLayout();
                //  
                // groupBox1 
                //  
                this.groupBox1.BackColor = System.Drawing.Color.LightGray;
                this.groupBox1.Controls.Add(this.Zone1);
                this.groupBox1.Controls.Add(this.label12);
                this.groupBox1.Controls.Add(this.label11);
                this.groupBox1.Controls.Add(this.P1Mins);
                this.groupBox1.Controls.Add(this.P1Hours);
                this.groupBox1.Controls.Add(this.label10);
                this.groupBox1.Controls.Add(this.label4);
                this.groupBox1.Controls.Add(this.SelectedProfile1);
                this.groupBox1.Controls.Add(this.BBcb1);
                this.groupBox1.Location = new System.Drawing.Point(12, 3);
                this.groupBox1.Name = "groupBox1";
                this.groupBox1.Size = new System.Drawing.Size(554, 74);
                this.groupBox1.TabIndex = 4;
                this.groupBox1.TabStop = false;
                this.groupBox1.Text = "New ";
                //  
                // Zone1 
                //  
                this.Zone1.FormattingEnabled = true;
                this.Zone1.Items.AddRange(new object[] { 
            "Hyjal", 
            "Uldum", 
            "Deepholm", 
            "Twilight Highlands", 
            "Tol\'Barad Peninsula", 
            "None"});
                this.Zone1.Location = new System.Drawing.Point(6, 45);
                this.Zone1.Name = "Zone1";
                this.Zone1.Size = new System.Drawing.Size(121, 21);
                this.Zone1.TabIndex = 8;
                this.Zone1.Text = "Zone";
                //  
                // label12 
                //  
                this.label12.AutoSize = true;
                this.label12.Location = new System.Drawing.Point(402, 47);
                this.label12.Name = "label12";
                this.label12.Size = new System.Drawing.Size(47, 13);
                this.label12.TabIndex = 7;
                this.label12.Text = "Minutes:";
                //  
                // label11 
                //  
                this.label11.AutoSize = true;
                this.label11.Location = new System.Drawing.Point(219, 48);
                this.label11.Name = "label11";
                this.label11.Size = new System.Drawing.Size(38, 13);
                this.label11.TabIndex = 6;
                this.label11.Text = "Hours:";
                //  
                // P1Mins 
                //  
                this.P1Mins.Location = new System.Drawing.Point(455, 45);
                this.P1Mins.Name = "P1Mins";
                this.P1Mins.Size = new System.Drawing.Size(70, 20);
                this.P1Mins.TabIndex = 5;
                //  
                // P1Hours 
                //  
                this.P1Hours.Location = new System.Drawing.Point(263, 45);
                this.P1Hours.Name = "P1Hours";
                this.P1Hours.Size = new System.Drawing.Size(70, 20);
                this.P1Hours.TabIndex = 4;
                //  
                // label10 
                //  
                this.label10.AutoSize = true;
                this.label10.BackColor = System.Drawing.SystemColors.AppWorkspace;
                this.label10.Location = new System.Drawing.Point(145, 47);
                this.label10.Name = "label10";
                this.label10.Size = new System.Drawing.Size(71, 13);
                this.label10.TabIndex = 3;
                this.label10.Text = "Run for Time:";
                //  
                // label4 
                //  
                this.label4.AutoSize = true;
                this.label4.BackColor = System.Drawing.SystemColors.ControlLight;
                this.label4.Location = new System.Drawing.Point(140, 23);
                this.label4.Name = "label4";
                this.label4.Size = new System.Drawing.Size(76, 13);
                this.label4.TabIndex = 2;
                this.label4.Text = "Profile Filepath";
                //  
                // SelectedProfile1 
                //  
                this.SelectedProfile1.Location = new System.Drawing.Point(222, 20);
                this.SelectedProfile1.Name = "SelectedProfile1";
                this.SelectedProfile1.Size = new System.Drawing.Size(306, 20);
                this.SelectedProfile1.TabIndex = 1;
                this.SelectedProfile1.Text = "None";
                //  
                // BBcb1 
                //  
                this.BBcb1.FormattingEnabled = true;
                this.BBcb1.Location = new System.Drawing.Point(6, 19);
                this.BBcb1.Name = "BBcb1";
                this.BBcb1.Size = new System.Drawing.Size(121, 21);
                this.BBcb1.TabIndex = 0;
                this.BBcb1.Text = "BOT Base";
                this.BBcb1.SelectedIndexChanged += new System.EventHandler(this.BBcb1_SelectedIndexChanged);
                //  
                // routes 
                //  
                this.routes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                this.routes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { 
            this.botBase, 
            this.profile, 
            this.zone, 
            this.runTimeH, 
            this.runTimeM});
                this.routes.GridLines = true;
                this.routes.Location = new System.Drawing.Point(12, 83);
                this.routes.Name = "routes";
                this.routes.Size = new System.Drawing.Size(554, 97);
                this.routes.TabIndex = 5;
                this.routes.UseCompatibleStateImageBehavior = false;
                this.routes.View = System.Windows.Forms.View.Details;
                //  
                // botBase 
                //  
                this.botBase.Text = "BOT Base";
                this.botBase.Width = 84;
                //  
                // profile 
                //  
                this.profile.Text = "Profile";
                this.profile.Width = 80;
                //  
                // zone 
                //  
                this.zone.Text = "Zone";
                //  
                // runTimeH 
                //  
                this.runTimeH.Text = "Run Time Hours";
                this.runTimeH.Width = 128;
                //  
                // runTimeM 
                //  
                this.runTimeM.Text = "Run Time Minutes";
                this.runTimeM.Width = 139;
                //  
                // add 
                //  
                this.add.Location = new System.Drawing.Point(572, 3);
                this.add.Name = "add";
                this.add.Size = new System.Drawing.Size(28, 23);
                this.add.TabIndex = 6;
                this.add.Text = "+";
                this.add.UseVisualStyleBackColor = true;
                this.add.Click += new System.EventHandler(this.add_Click);
                //  
                // rem 
                //  
                this.rem.Location = new System.Drawing.Point(572, 83);
                this.rem.Name = "rem";
                this.rem.Size = new System.Drawing.Size(28, 23);
                this.rem.TabIndex = 7;
                this.rem.Text = "-";
                this.rem.UseVisualStyleBackColor = true;
                this.rem.Click += new System.EventHandler(this.rem_Click);
                //  
                // Example 
                //  
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(609, 188);
                this.Controls.Add(this.rem);
                this.Controls.Add(this.add);
                this.Controls.Add(this.routes);
                this.Controls.Add(this.groupBox1);
                this.Name = "Example";
                this.Text = "Example";
                this.groupBox1.ResumeLayout(false);
                this.groupBox1.PerformLayout();
                ((System.ComponentModel.ISupportInitialize)(this.P1Mins)).EndInit();
                ((System.ComponentModel.ISupportInitialize)(this.P1Hours)).EndInit();
                this.ResumeLayout(false);

            }

            #endregion

            private System.Windows.Forms.GroupBox groupBox1;
            private System.Windows.Forms.ComboBox Zone1;
            private System.Windows.Forms.Label label12;
            private System.Windows.Forms.Label label11;
            private System.Windows.Forms.NumericUpDown P1Mins;
            private System.Windows.Forms.NumericUpDown P1Hours;
            private System.Windows.Forms.Label label10;
            private System.Windows.Forms.Label label4;
            private System.Windows.Forms.TextBox SelectedProfile1;
            private System.Windows.Forms.ComboBox BBcb1;
            private System.Windows.Forms.ListView routes;
            private System.Windows.Forms.ColumnHeader botBase;
            private System.Windows.Forms.ColumnHeader profile;
            private System.Windows.Forms.ColumnHeader zone;
            private System.Windows.Forms.ColumnHeader runTimeH;
            private System.Windows.Forms.ColumnHeader runTimeM;
            private System.Windows.Forms.Button add;
            private System.Windows.Forms.Button rem;
        }

        class item
        {
            public string BotBase;
            public string Profile;
            public string Zone;
            public string Hours;
            public string Minutes;
            public string[] columns { get { return new string[] { BotBase, Profile, Zone, Hours, Minutes }; } }
        }

    }
}
