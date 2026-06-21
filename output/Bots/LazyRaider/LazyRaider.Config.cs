using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

using Bobby53;

namespace Styx.Bot.CustomBots
{
    public partial class SelectTankForm : Form
    {
        public SelectTankForm()
        {
            InitializeComponent();
        }

        private void btnSetLeader_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Choose a Player before clicking Set Leader");
                listView.Focus();
            }
            else
            {
                WoWPlayer chosenPlayer = (WoWPlayer)listView.SelectedItems[0].Tag;
                try
                {
                    if (chosenPlayer != null)
                    {
                        RaFHelper.SetLeader(chosenPlayer);
                        LazyRaider.IamTheTank = false;
                        Logging.Write( Color.Blue, "[LazyRaider] Tank set to {0}, max health {1}", LazyRaider.Safe_UnitName( RaFHelper.Leader), RaFHelper.Leader.MaxHealth);
                    }
                    else // == null
                    {
                        RaFHelper.ClearLeader();
                        LazyRaider.IamTheTank = true;
                        Logging.Write(Color.Blue, "[LazyRaider] selected -ME-, leader cleared");
                    }
                }
                catch
                {
                    listView.SelectedItems[0].Remove();
                }

                this.Hide();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if ( chkRunWithoutTank.Checked )
            {
                if ( RaFHelper.Leader != null )
                    Logging.Write(Color.Blue, "[LazyRaider] selected -ME-, leader cleared");
                RaFHelper.ClearLeader();
            }

            this.Hide();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadListView();
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView_Click(object sender, EventArgs e)
        {

        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            if (btnSetLeader.Enabled)
            {
                btnSetLeader_Click(sender, e);
            }
        }


        private void LoadListView()
        {
            listView.Items.Clear();

            AddRow(null, "-ME-", LazyRaider.Me.Class.ToString(), LazyRaider.GetGroupRoleAssigned(LazyRaider.Me).ToString(), LazyRaider.Me.MaxHealth.ToString());

            if (ObjectManager.IsInGame && ObjectManager.Me != null)
            {
                ObjectManager.Update();

                Logging.WriteDebug(Color.Chocolate, "-- Group Count: {0}", LazyRaider.GroupMembers.Count);

                foreach (WoWPartyMember pm in LazyRaider.GroupMemberInfos)
                {
                    WoWPlayer p = pm.ToPlayer();
                    if ( pm == null || p == null || p.IsMe )
                        continue;
                    
                    string sRole = LazyRaider.GetGroupRoleAssigned( pm).ToString().ToUpper();
                    Logging.WriteDebug(Color.Chocolate, "-- Group Member: {0} hp={1} is {2}", p.Class.ToString(), p.MaxHealth, sRole);

                    AddRow(p, p.Name, p.Class.ToString(), sRole, p.MaxHealth.ToString());
                }
            }

            btnSetLeader.Enabled = true;
        }

        private void AddRow( WoWObject @tag, string @name, string @class, string @role, string @health  )
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = @name;
            lvi.Tag = tag;

            ulong tagGuid = @tag == null ? 0 : @tag.Guid;
            ulong leadGuid = RaFHelper.Leader == null ? 0 : RaFHelper.Leader.Guid;
            lvi.Selected = tagGuid == leadGuid;

            ListViewItem.ListViewSubItem lvs = new ListViewItem.ListViewSubItem();
            lvs.Text = @class.ToString();
            lvi.SubItems.Add(lvs);

            lvs = new ListViewItem.ListViewSubItem();
            lvs.Text = @role;
            lvi.SubItems.Add(lvs);

            lvs = new ListViewItem.ListViewSubItem();
            lvs.Text = @health.ToString();
            lvi.SubItems.Add(lvs);

            listView.Items.Add(lvi);
        }

        private void SelectTankForm_Shown(object sender, EventArgs e)
        {

        }

        private static bool isVisible = false;
        private void SelectTankForm_VisibleChanged(object sender, EventArgs e)
        {
            isVisible = !isVisible;

            if (isVisible)
            {
                chkAutoFollow.Checked = LazyRaiderSettings.Instance.FollowTank;
                numFollowDistance.Value = LazyRaiderSettings.Instance.FollowDistance;
                LoadListView();
                chkRunWithoutTank.Checked = LazyRaiderSettings.Instance.NoTank;
                chkDisableTank_CheckedChanged(sender, e);
                listView.Focus();
            }
            else
            {
                LazyRaiderSettings.Instance.NoTank = chkRunWithoutTank.Checked;
                LazyRaiderSettings.Instance.FollowTank = chkAutoFollow.Checked;
                LazyRaiderSettings.Instance.FollowDistance = (int)numFollowDistance.Value;
                LazyRaiderSettings.Instance.Save();
            }

            return;
        }

        private void SelectTankForm_Activated(object sender, EventArgs e)
        {
            return;
        }

        private void SelectTankForm_Deactivate(object sender, EventArgs e)
        {
        }

        private void SelectTankForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // hide instead of close when user clicks X
            this.Hide();
            e.Cancel = true;
        }

        private void chkDisableTank_CheckedChanged(object sender, EventArgs e)
        {
            chkAutoFollow.Enabled = !chkRunWithoutTank.Checked;
            lblFollowDistance.Enabled = !chkRunWithoutTank.Checked;
            numFollowDistance.Enabled = !chkRunWithoutTank.Checked;
            btnSetLeader.Enabled = !chkRunWithoutTank.Checked;
            listView.Enabled = !chkRunWithoutTank.Checked;
            btnRefresh.Enabled = !chkRunWithoutTank.Checked;
        }
    }
}
