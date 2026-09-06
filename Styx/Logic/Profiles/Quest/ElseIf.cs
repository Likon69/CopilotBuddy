using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Styx.Helpers;

#nullable disable
namespace Styx.Logic.Profiles.Quest
{
    public class ElseIf : IXmlObject
    {
        public ElseIf(string conditionText, IEnumerable<OrderNode> body, XElement element)
        {
            if (string.IsNullOrEmpty(conditionText))
                throw new ArgumentException("condition cannot be null or empty", nameof(conditionText));

            this.ConditionText = conditionText;
            this.Condition = DelayCompiledExpression.Condition(conditionText);
            this.Body = body != null ? new OrderNodeCollection(body) : new OrderNodeCollection();
            this.Element = element;
        }

        public string ConditionText { get; private set; }

        [CompileExpression]
        public DelayCompiledExpression<Func<bool>> Condition { get; private set; }

        public OrderNodeCollection Body { get; private set; }

        public XElement Element { get; private set; }

        public static ElseIf FromXml(XElement element)
        {
            var condAttr = element.Attribute("Condition") ?? element.Attribute("condition");
            if (condAttr == null)
                throw new ProfileMissingAttributeException("Condition", element);

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
                    throw new ProfileException($"Could not parse ElseIf body node: {ex.Message}", ex);
                }
            }
            return new ElseIf(condAttr.Value, body, element);
        }
    }
}
