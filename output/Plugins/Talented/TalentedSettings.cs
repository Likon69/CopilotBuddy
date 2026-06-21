using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Styx;
using Styx.Helpers;

namespace Talented
{
    class TalentedSettings : Settings
    {
        public static TalentedSettings Instance = new TalentedSettings();

        public TalentedSettings() : base(Path.Combine(Path.Combine(Logging.ApplicationPath, "Settings"), 
            string.Format("TalentedSettings_{0}.xml", StyxWoW.Me.Name)))
        {
        }

        public static readonly string PluginFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Path.Combine("Plugins", "Talented"));

        [Setting(Explanation = "The name of the last used Talent Build."), DefaultValue("")]
        public string ChoosenTalentBuildName { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FirstUseAfterChange { get; set; }

        public TalentTree ChoosenTalentBuild
        {
            get
            {
                if (string.IsNullOrEmpty(ChoosenTalentBuildName))
                    return null;

                var talentBuildPath = Path.Combine(PluginFolderPath, "Talent Builds");
                string[] files = Directory.GetFiles(talentBuildPath, "*.xml", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];

                    try
                    {
                        TalentTree talentTree = TalentTree.FromXml(XElement.Load(file));
                        if (talentTree != null && talentTree.BuildName == ChoosenTalentBuildName)
                            return talentTree;

                    }
                    catch(Exception)
                    {
                    }
                }

                return null;
            }
        }
    }
}
