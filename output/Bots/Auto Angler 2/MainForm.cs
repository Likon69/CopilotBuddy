using System;
using System.Windows.Forms;
using Styx.Logic.Profiles;
using Styx.Logic.POI;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using System.IO;

namespace HighVoltz
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            propertyGrid.SelectedObject = AutoAngler.Instance.MySettings;
        }

        private void DonateButton_Click(object sender, EventArgs e)
        {
            // my debug button :)
            if (System.Environment.UserName == "highvoltz")
            {
                //LocalPlayer me = ObjectManager.Me;
                //var list = MoveToPoolAction.GetQuadSloopTraceLines(me.Location);
                //WoWPoint top  = me.Location;
                //top.Z +=6;
                //WoWPoint bot = top;
                //bot.Z -= 12;
                //WoWPoint meResult;
                //GameWorld.TraceLine(top, bot, GameWorld.CGWorldFrameHitFlags.HitTestGroundAndStructures, out meResult);
                //bool[] slopelinesRetVals;
                //WoWPoint[] slopeHits;
                //GameWorld.MassTraceLine(list.ToArray(), GameWorld.CGWorldFrameHitFlags.HitTestGroundAndStructures,
                //out slopelinesRetVals, out slopeHits);

                //Logging.Write("Me {0}, meResult {1}", me.Location, meResult);
                //var ret = MoveToPoolAction.ProcessSlopeResult(new List<WoWPoint> { meResult }, slopeHits);
                //foreach (var line in slopeHits)
                //{
                //    Logging.Write("{0}", line);
                //}
            }
            else
                System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=MMC4GPHR8GQFN&lc=US&item_name=Highvoltz%27s%20Development%20fund&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_LG%2egif%3aNonHosted");
        }

        private void RepButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.thebuddyforum.com/reputation.php?do=addreputation&p=343952");
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.Label == "Poolfishing")
            {
                if (!(bool)e.ChangedItem.Value)
                {
                    if (!string.IsNullOrEmpty(ProfileManager.XmlLocation))
                    {
                        AutoAnglerSettings.Instance.LastLoadedProfile = ProfileManager.XmlLocation;
                        AutoAnglerSettings.Instance.Save();
                    }
                    ProfileManager.LoadEmpty();
                }
                else if ((ProfileManager.CurrentProfile == null || ProfileManager.CurrentProfile.Name == "Empty Profile") &&
                    !string.IsNullOrEmpty(AutoAnglerSettings.Instance.LastLoadedProfile) &&
                    File.Exists(AutoAnglerSettings.Instance.LastLoadedProfile))
                {
                    ProfileManager.LoadNew(AutoAnglerSettings.Instance.LastLoadedProfile);
                }
            }
            AutoAngler.Instance.MySettings.Save();
        }

        private void MailButton_Click(object sender, EventArgs e)
        {
            var profile = ProfileManager.CurrentProfile;
            if (profile != null && profile.MailboxManager != null)
            {
                var mailbox = profile.MailboxManager.GetClosestMailbox();
                if (mailbox != null)
                {
                    if (!string.IsNullOrEmpty(CharacterSettings.Instance.MailRecipient))
                    {
                        BotPoi.Current = new BotPoi(mailbox);
                        AutoAngler.Instance.Log("Forced Mail run");
                        TreeRoot.StatusText = "Doing Mail Run";
                    }
                    else
                        AutoAngler.Instance.Log("No mail recipient set");
                }
                else
                {
                    AutoAngler.Instance.Log("Profile has no Mailbox");
                }

            }
        }

    }
}
