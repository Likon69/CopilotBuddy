using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Styx;
using Styx.Helpers;
using Styx.WoWInternals;

namespace Talented.Forms
{
    public partial class FormConfig : Form
    {
        public FormConfig()
        {
            InitializeComponent();
        }
        
        private void FormConfig_Load(object sender, EventArgs e)
        {
            if (TalentedSettings.Instance.FirstUseAfterChange)
                TalentedSettings.Instance.ChoosenTalentBuildName = null;

            btnRefresh.Click += (s,args) => RefreshTalentBuilds();
            btnSaveAndClose.Click += (s, args) =>
                                         {
                                             TalentedSettings.Instance.Save();
                                             Close();
                                         };
            RefreshTalentBuilds();
        }

        private List<TalentTree> _talentBuilds = new List<TalentTree>();

        private void RefreshTalentBuilds()
        {
            lbTalentBuilds.Items.Clear();
            _talentBuilds.Clear();

            var talentBuildPath = Path.Combine(TalentedSettings.PluginFolderPath, "Talent Builds");
            string[] files = Directory.GetFiles(talentBuildPath, "*.xml", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                
                try
                {
                    TalentTree talentTree = TalentTree.FromXml(XElement.Load(file));

                    _talentBuilds.Add(talentTree);
                }
                catch (XmlException ex)
                {
                    Logging.Write("[Talented]: Could not load talent build {0}", ex.Message);
                }
            }

            _talentBuilds = _talentBuilds.OrderBy(t => t.BuildName).ToList();
            lbTalentBuilds.Items.AddRange(_talentBuilds.ToArray());

            if (!string.IsNullOrEmpty(TalentedSettings.Instance.ChoosenTalentBuildName))
            {
                var build =
                    _talentBuilds.FirstOrDefault(b => b.BuildName == TalentedSettings.Instance.ChoosenTalentBuildName);

                if (build != null)
                    lbTalentBuilds.SelectedIndex = _talentBuilds.IndexOf(build);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lbTalentBuilds.SelectedIndex < 0 || lbTalentBuilds.SelectedIndex >= _talentBuilds.Count)
            {
                Close();
                return;
            }

            TalentedSettings.Instance.ChoosenTalentBuildName = _talentBuilds[lbTalentBuilds.SelectedIndex].BuildName;
            TalentedSettings.Instance.FirstUseAfterChange = false;
            Close();
        }

        private void lbTalentBuilds_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbTalents.Items.Clear();

            var item = lbTalentBuilds.SelectedItem;
            if(item is TalentTree)
            {
                var talentTree = (TalentTree) item;
                lblClass.Text = "Class: " + talentTree.Class;
                lblName.Text = "Name: " + talentTree.BuildName;

                foreach(TalentPlacement tp in talentTree.TalentPlacements)
                    lbTalents.Items.Add(tp);
            }
        }

        private void btnDump_Click(object sender, EventArgs e)
        {
            var talents = BuildLearnedTalents();

            XElement rootElement = new XElement("TalentTree",
                                                new XAttribute("Name", "Name of this build"),
                                                new XAttribute("Specialization", Lua.GetReturnVal<int>("return GetActiveTalentGroup()", 0)),
                                                new XAttribute("Class", StyxWoW.Me.Class));

            foreach(TalentPlacement tp in talents)
                rootElement.Add(
                    new XElement("Talent",
                        new XAttribute("Tab", tp.Tab),
                        new XAttribute("Index", tp.Index),
                        new XAttribute("Count", tp.Count),
                        new XAttribute("Name", tp.Name))
                    );
            
            Clipboard.SetText(rootElement.ToString());
            MessageBox.Show(
                "Xml of current build dumped to clipboard!", 
                "Sucess!", 
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
                );
        }

        private static IEnumerable<TalentPlacement> BuildLearnedTalents()
        {
            var ret = new List<TalentPlacement>();

            using (new FrameLock())
            {
                for (int tabIndex = 1; tabIndex <= 3; tabIndex++)
                {
                    int numTalents = Lua.GetReturnVal<int>("return GetNumTalents(" + tabIndex + ", false, false)", 0);
                    for (int talentIndex = 1; talentIndex <= numTalents; talentIndex++)
                    {
                        var vals = Lua.GetReturnValues("return GetTalentInfo(" + tabIndex + ", " + talentIndex + ")");
                        var name = vals[0];
                        var rank = int.Parse(vals[4]);
                        if(rank != 0)
                            ret.Add(new TalentPlacement(tabIndex, talentIndex, rank, name));
                    }
                }
            }

            return ret;
        }
    }
}
