#nullable disable
using System;
using System.Xml.Linq;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles.Quest;

namespace Styx.Logic.Profiles
{
    /// <summary>
    /// Represents a mailbox location in a profile.
    /// </summary>
    public class Mailbox : IEquatable<Mailbox>, IXmlObject
    {
        /// <summary>
        /// Gets the location of the mailbox.
        /// </summary>
        public WoWPoint Location { get; private set; }

        /// <summary>
        /// Creates a new mailbox from an XML element.
        /// </summary>
        /// <param name="element">The XML element containing mailbox data.</param>
        public Mailbox(XElement element)
        {
            Element = element;
            Location = ProfileHelper.ParseLocation(element);

            XAttribute usableWhen = element.Attribute("UsableWhen") ?? element.Attribute("usablewhen");
            if (usableWhen != null && !string.IsNullOrWhiteSpace(usableWhen.Value))
                UsableWhen = DelayCompiledExpression.Condition(usableWhen.Value);
        }

        /// <summary>
        /// The XML element this mailbox was parsed from, null when it was built from a location.
        /// </summary>
        public XElement Element { get; private set; }

        /// <summary>
        /// Optional condition guarding the mailbox, compiled with the rest of the profile.
        /// HB 6.2.3 Mailbox.UsableWhen; MailboxManager skips a mailbox whose condition is false.
        /// </summary>
        [CompileExpression]
        public DelayCompiledExpression<Func<bool>> UsableWhen { get; private set; }

        /// <summary>
        /// Creates a new mailbox at the specified location.
        /// </summary>
        /// <param name="location">The mailbox location.</param>
        public Mailbox(WoWPoint location)
        {
            Location = location;
        }

        /// <summary>
        /// Determines whether this mailbox equals another.
        /// </summary>
        public bool Equals(Mailbox other)
        {
            if (other == null) return false;
            return Location == other.Location;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Mailbox);
        }

        public override int GetHashCode()
        {
            return Location.GetHashCode();
        }

        public override string ToString()
        {
            return $"[Mailbox Location: {Location}]";
        }
    }
}
