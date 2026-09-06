#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.WoWInternals;

namespace Styx.Logic.Profiles
{
    /// <summary>
    /// Manages mailbox locations from profiles.
    /// </summary>
    public class MailboxManager
    {
        private List<Mailbox> _filteredMailboxes = new List<Mailbox>();

        /// <summary>
        /// Gets or sets forced mailboxes that override profile mailboxes.
        /// </summary>
        public List<Mailbox> ForcedMailboxes { get; set; }

        /// <summary>
        /// Gets all mailboxes in the profile.
        /// </summary>
        public List<Mailbox> AllMailboxes { get; private set; }

        /// <summary>
        /// Gets blacklisted mailboxes.
        /// </summary>
        public List<Mailbox> Blacklist { get; private set; }

        /// <summary>
        /// Gets mailboxes excluding blacklisted ones.
        /// </summary>
        public List<Mailbox> Mailboxes
        {
            get
            {
                RemoveBlacklisted();
                return _filteredMailboxes;
            }
        }

        /// <summary>
        /// Creates an empty mailbox manager.
        /// </summary>
        public MailboxManager()
        {
            AllMailboxes = new List<Mailbox>();
            Blacklist = new List<Mailbox>();
            ForcedMailboxes = new List<Mailbox>();
        }

        /// <summary>
        /// Creates a mailbox manager from XML.
        /// </summary>
        public MailboxManager(XElement xml)
        {
            AllMailboxes = new List<Mailbox>();
            Blacklist = new List<Mailbox>();
            ForcedMailboxes = new List<Mailbox>();

            foreach (var child in xml.Elements().ToList())
            {
                try
                {
                    if (child.Name.ToString().ToLowerInvariant() != "mailbox")
                        throw new ProfileUnknownElementException(child, "Mailbox");

                    AllMailboxes.Add(new Mailbox(child));
                }
                catch (ProfileException ex)
                {
                    Logging.WriteException(ex);
                }
            }
            _filteredMailboxes = AllMailboxes;
        }

        /// <summary>
        /// Gets the closest mailbox to the player.
        /// </summary>
        public Mailbox GetClosestMailbox()
        {
            return GetClosestMailbox(ObjectManager.Me.Location);
        }

        /// <summary>
        /// Gets the closest mailbox to a location.
        /// </summary>
        public Mailbox GetClosestMailbox(WoWPoint location)
        {
            Mailbox closest = null;
            float closestDist = float.MaxValue;

            // Use forced mailboxes if available, otherwise use profile mailboxes
            var mailboxList = (ForcedMailboxes != null && ForcedMailboxes.Count > 0)
                ? ForcedMailboxes
                : Mailboxes;

            foreach (var mailbox in mailboxList.Where(IsUsable))
            {
                float dist = location.DistanceSqr(mailbox.Location);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = mailbox;
                }
            }
            return closest;
        }

        /// <summary>
        /// A mailbox with no UsableWhen is always usable, otherwise its condition decides.
        /// HB 6.2.3 MailboxManager.Class1186.method_0, applied where HB applies it: at selection.
        /// </summary>
        private static bool IsUsable(Mailbox mailbox)
        {
            return mailbox.UsableWhen == null || mailbox.UsableWhen.CallableExpression();
        }

        /// <summary>
        /// Removes blacklisted mailboxes from the filtered list.
        /// </summary>
        private void RemoveBlacklisted()
        {
            if (_filteredMailboxes.Count == 0)
                return;
            _filteredMailboxes.RemoveAll(m => Blacklist.Contains(m));
        }
    }
}
