using Styx.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

#nullable disable
namespace Styx.Logic.Profiles.Quest
{
    /// <summary>
    /// Gathers every [CompileString] and [CompileExpression] member reachable from a profile into a
    /// single CompileBatch, so a profile costs one Roslyn assembly instead of one per condition.
    /// Ported from HB 6.2.3 ns88.Class1208 (the walk) and ns88.Class1216 (the per-type member cache).
    /// </summary>
    public class CodeComposition
    {
        private readonly Dictionary<XElement, List<Registration>> _byElement =
            new Dictionary<XElement, List<Registration>>();
        private readonly Dictionary<Type, Members> _byType = new Dictionary<Type, Members>();

        public CompileBatch Batch { get; } = new CompileBatch();

        /// <summary>
        /// Registers the whole quest order of a profile, sub-profiles included.
        /// HB raises OnCodeComposition here and QuestBot walks the order; the port has a single
        /// consumer, so the walk lives with the composition.
        /// </summary>
        public void AddProfile(Profile profile)
        {
            if (profile == null)
                return;

            OrderNodeCollection order = profile.QuestOrder;
            if (order != null)
                AddNodes(order);

            // HB 6.2.3 ProfileManager.smethod_1 contributes the mailboxes and the vendors, whose
            // UsableWhen is a [CompileExpression] like any node condition.
            if (profile.MailboxManager != null)
            {
                foreach (Mailbox mailbox in profile.MailboxManager.AllMailboxes)
                    Add(mailbox);
            }
            if (profile.VendorManager != null)
            {
                foreach (Vendor vendor in profile.VendorManager.AllVendors)
                    Add(vendor);
            }

            if (profile.SubProfiles != null)
            {
                foreach (Profile sub in profile.SubProfiles)
                    AddProfile(sub);
            }
        }

        private void AddNodes(INodeContainer container)
        {
            foreach (OrderNode node in container.GetNodes())
            {
                Add(node);
                if (node is INodeContainer nested)
                    AddNodes(nested);
            }
        }

        /// <summary>
        /// HB 6.2.3 Class1208.method_0.
        /// </summary>
        public void Add(IXmlObject xmlObject)
        {
            if (xmlObject == null)
                return;

            Members members = MembersOf(xmlObject.GetType());
            if (members == null)
                return;

            XElement element = xmlObject.Element;
            if (element == null)
            {
                Logging.Write(Color.Red,
                    "[CodeComposition] {0} carries no XML element, its expressions cannot be compiled.",
                    xmlObject.GetType().Name);
                return;
            }

            if (_byElement.ContainsKey(element))
                return;

            foreach (PropertyInfo property in members.CompileStrings)
            {
                string code = property.GetValue(xmlObject) as string;
                if (!string.IsNullOrEmpty(code))
                    Batch.Add(code, element);
            }

            var registrations = new List<Registration>();
            foreach (PropertyInfo property in members.CompileExpressions)
            {
                if (property.GetValue(xmlObject) is DelayCompiledExpression expression)
                {
                    Batch.AddExpression(expression, element);
                    registrations.Add(new Registration(expression, property));
                }
            }
            _byElement[element] = registrations;

            Walk(xmlObject, members, Add);
        }

        /// <summary>
        /// Copies the delegates compiled for an element onto another instance parsed from that same
        /// element. HB 6.2.3 Class1208.method_1, used when a node is rebuilt after the batch compiled.
        /// </summary>
        public bool ApplyExpressions(IXmlObject xmlObject)
        {
            if (xmlObject == null)
                throw new ArgumentNullException(nameof(xmlObject));

            XElement element = xmlObject.Element;
            if (element == null)
                return false;

            Members members = MembersOf(xmlObject.GetType());
            if (members == null)
                return false;

            if (!_byElement.TryGetValue(element, out List<Registration> registrations))
                return false;

            foreach (Registration registration in registrations)
            {
                if (registration.Property.GetValue(xmlObject) is DelayCompiledExpression target)
                    target.CompiledExpression = registration.Expression.CompiledExpression;
            }

            Walk(xmlObject, members, o => ApplyExpressions(o));
            return true;
        }

        /// <summary>
        /// HB 6.2.3 Class1208.method_2: follow the IXmlObject and IEnumerable of IXmlObject
        /// properties, which is how Else, the ElseIfs and every body reach the batch.
        /// </summary>
        private static void Walk(IXmlObject xmlObject, Members members, Action<IXmlObject> action)
        {
            foreach (PropertyInfo property in members.XmlObjects)
            {
                if (property.GetValue(xmlObject) is IXmlObject child && child.Element != null)
                    action(child);
            }

            foreach (PropertyInfo property in members.XmlCollections)
            {
                if (property.GetValue(xmlObject) is IEnumerable<IXmlObject> children)
                {
                    foreach (IXmlObject child in children)
                    {
                        if (child != null && child.Element != null)
                            action(child);
                    }
                }
            }
        }

        private Members MembersOf(Type type)
        {
            if (_byType.TryGetValue(type, out Members cached))
                return cached;

            PropertyInfo[] properties = type.GetProperties();
            var compileStrings = properties.Where(p => p.GetCustomAttribute<CompileStringAttribute>() != null).ToList();
            var compileExpressions = properties.Where(p => p.GetCustomAttribute<CompileExpressionAttribute>() != null).ToList();
            var xmlObjects = properties.Where(p => typeof(IXmlObject).IsAssignableFrom(p.PropertyType)).ToList();
            var xmlCollections = properties.Where(p => typeof(IEnumerable<IXmlObject>).IsAssignableFrom(p.PropertyType)).ToList();

            Members members = null;
            if (compileStrings.Count > 0 || compileExpressions.Count > 0 ||
                xmlObjects.Count > 0 || xmlCollections.Count > 0)
            {
                members = new Members(compileStrings, compileExpressions, xmlObjects, xmlCollections);
            }
            _byType[type] = members;
            return members;
        }

        private sealed class Members
        {
            public Members(List<PropertyInfo> compileStrings, List<PropertyInfo> compileExpressions,
                List<PropertyInfo> xmlObjects, List<PropertyInfo> xmlCollections)
            {
                CompileStrings = compileStrings;
                CompileExpressions = compileExpressions;
                XmlObjects = xmlObjects;
                XmlCollections = xmlCollections;
            }

            public List<PropertyInfo> CompileStrings { get; }
            public List<PropertyInfo> CompileExpressions { get; }
            public List<PropertyInfo> XmlObjects { get; }
            public List<PropertyInfo> XmlCollections { get; }
        }

        private readonly struct Registration
        {
            public Registration(DelayCompiledExpression expression, PropertyInfo property)
            {
                Expression = expression;
                Property = property;
            }

            public DelayCompiledExpression Expression { get; }
            public PropertyInfo Property { get; }
        }
    }
}
