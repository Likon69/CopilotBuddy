using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Styx.Helpers;

#nullable disable
namespace Styx.Logic.Profiles.Quest
{
    public class Else : IXmlObject
    {
        public Else(IEnumerable<OrderNode> body, XElement element)
        {
            this.Body = body != null ? new OrderNodeCollection(body) : new OrderNodeCollection();
            this.Element = element;
        }

        public OrderNodeCollection Body { get; private set; }

        public XElement Element { get; private set; }

        public static Else FromXml(XElement element)
        {
            List<OrderNode> body = new List<OrderNode>();
            foreach (XElement child in element.Elements().Where(e => e.NodeType != XmlNodeType.Comment))
            {
                try
                {
                    body.Add(OrderNode.FromXml(child));
                }
                catch (ProfileException ex)
                {
                    if (StyxSettings.Instance.ProfileDebuggingMode)
                        Logging.WriteException(ex);
                    throw new ProfileException("Could not parse Else body node", ex);
                }
            }
            return new Else(body, element);
        }
    }
}
