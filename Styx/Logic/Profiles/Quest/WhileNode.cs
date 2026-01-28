// Decompiled with JetBrains decompiler
// Type: Styx.Logic.Profiles.Quest.WhileNode
// Assembly: Honorbuddy, Version=2.0.0.5999, Culture=neutral, PublicKeyToken=50a565ab5c01ae50
// MVID: FB7FEB85-27C0-4D17-B8DE-615FDFDA7752
// Assembly location: C:\Users\Texy6\Desktop\Honorbuddy-cleaned.exe

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
    public WhileNode(Func<bool> condition, IEnumerable<OrderNode> body)
        : base(OrderNodeType.While)
    {
        this.Condition = condition != null ? condition : throw new ArgumentNullException(nameof(condition));
        this.Body = body != null ? new OrderNodeCollection(body) : new OrderNodeCollection();
    }

    public Func<bool> Condition { get; private set; }

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

        Func<bool> condition = ConditionHelper.CompileCondition(conditionAttr);
        if (condition == null)
            throw new ProfileException($"Could not parse 'While'. Condition code \"{conditionAttr.Value}\" could not be compiled into C# code.");

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
        return new WhileNode(condition, body);
    }
}
