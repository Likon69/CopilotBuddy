using Styx.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace Styx.Logic.Profiles.Quest;

public class WhileNode : OrderNode, INodeContainer
{
    public WhileNode(string conditionText, IEnumerable<OrderNode> body, XElement element)
        : base(OrderNodeType.While, element)
    {
        if (string.IsNullOrEmpty(conditionText))
            throw new ArgumentException("condition cannot be null or empty", nameof(conditionText));

        this.ConditionText = conditionText;
        this.Condition = DelayCompiledExpression.Condition(conditionText);
        this.Body = body != null ? new OrderNodeCollection(body) : new OrderNodeCollection();
    }

    public string ConditionText { get; private set; }

    [CompileExpression]
    public DelayCompiledExpression<Func<bool>> Condition { get; private set; }

    public OrderNodeCollection Body { get; private set; }

    public override string ToString() => "[WhileNode]";

    public IEnumerable<OrderNode> GetNodes()
    {
        return this.Body != null ? (IEnumerable<OrderNode>)this.Body : Enumerable.Empty<OrderNode>();
    }

    public new static OrderNode FromXml(XElement element)
    {
        var conditionAttr = element.Attribute("Condition") ?? element.Attribute("condition");
        if (conditionAttr == null)
            throw new ProfileMissingAttributeException("condition", element);

        List<OrderNode> body = new List<OrderNode>();
        foreach (XElement childElement in element.Elements().Where(e => e.NodeType != XmlNodeType.Comment))
        {
            try
            {
                body.Add(OrderNode.FromXml(childElement));
            }
            catch (ProfileException ex)
            {
                if (StyxSettings.Instance.ProfileDebuggingMode)
                    Logging.WriteException(ex);
                throw new ProfileException("Could not parse While body node", ex);
            }
        }
        return new WhileNode(conditionAttr.Value, body, element);
    }
}
