// Decompiled with JetBrains decompiler
// Type: Styx.Logic.Profiles.Quest.OrderNode
// Assembly: Honorbuddy, Version=2.0.0.5999, Culture=neutral, PublicKeyToken=50a565ab5c01ae50
// MVID: FB7FEB85-27C0-4D17-B8DE-615FDFDA7752
// Assembly location: C:\Users\Texy6\Desktop\Honorbuddy-cleaned.exe

using System;
using System.Collections.Generic;
using System.Xml.Linq;



#nullable disable
namespace Styx.Logic.Profiles.Quest;

public abstract class OrderNode : IXmlObject
{
    private static readonly Dictionary<OrderNodeType, Func<XElement, OrderNode>> _nodeFactories = new Dictionary<OrderNodeType, Func<XElement, OrderNode>>()
    {
        { OrderNodeType.Checkpoint, CheckpointNode.FromXml },
        { OrderNodeType.PickUp, PickUpNode.FromXml },
        { OrderNodeType.TurnIn, TurnInNode.FromXml },
        { OrderNodeType.Objective, ObjectiveNode.FromXml },
        { OrderNodeType.ClearGrindArea, ClearGrindAreaNode.FromXml },
        { OrderNodeType.ClearMailbox, ClearMailboxNode.FromXml },
        { OrderNodeType.ClearVendor, ClearVendorNode.FromXml },
        { OrderNodeType.SetGrindArea, SetGrindAreaNode.FromXml },
        { OrderNodeType.SetMailbox, SetMailboxNode.FromXml },
        { OrderNodeType.SetVendor, SetVendorNode.FromXml },
        { OrderNodeType.GrindTo, GrindToNode.FromXml },
        { OrderNodeType.If, IfNode.FromXml },
        { OrderNodeType.While, WhileNode.FromXml },
        { OrderNodeType.AbandonQuest, AbandonQuestNode.FromXml },
        { OrderNodeType.MoveTo, MoveToNode.FromXml },
        { OrderNodeType.UseItem, UseItemNode.FromXml },
        { OrderNodeType.Code, CodeNode.FromXml },
        { OrderNodeType.EnableRepair, EnableRepairNode.FromXml },
        { OrderNodeType.DisableRepair, DisableRepairNode.FromXml }
    };

    protected OrderNode(OrderNodeType type) => this.Type = type;

    protected OrderNode(OrderNodeType type, XElement fromElement)
        : this(type)
    {
        this.Element = fromElement;
    }

    public OrderNodeType Type { get; private set; }

    public XElement Element { get; protected set; }

    public static OrderNode FromXml(XElement element)
    {
        OrderNodeType? nullable = ParseNodeType(element.Name.ToString());
        if (!nullable.HasValue)
            throw new ProfileUnknownElementException(element);
        return _nodeFactories[nullable.Value](element);
    }

    private static OrderNodeType? ParseNodeType(string nodeTypeName)
    {
        switch (nodeTypeName.ToLowerInvariant())
        {
            case "checkpoint":
                return new OrderNodeType?(OrderNodeType.Checkpoint);
            case "pickup":
            case "pickupquest":
                return new OrderNodeType?(OrderNodeType.PickUp);
            case "turnin":
            case "turninquest":
            case "handin":
            case "handinquest":
                return new OrderNodeType?(OrderNodeType.TurnIn);
            case "objective":
                return new OrderNodeType?(OrderNodeType.Objective);
            case "cleargrindarea":
                return new OrderNodeType?(OrderNodeType.ClearGrindArea);
            case "clearmailbox":
                return new OrderNodeType?(OrderNodeType.ClearMailbox);
            case "clearvendor":
                return new OrderNodeType?(OrderNodeType.ClearVendor);
            case "setgrindarea":
                return new OrderNodeType?(OrderNodeType.SetGrindArea);
            case "setmailbox":
                return new OrderNodeType?(OrderNodeType.SetMailbox);
            case "setvendor":
                return new OrderNodeType?(OrderNodeType.SetVendor);
            case "grindto":
                return new OrderNodeType?(OrderNodeType.GrindTo);
            case "disablerepair":
                return new OrderNodeType?(OrderNodeType.DisableRepair);
            case "enablerepair":
                return new OrderNodeType?(OrderNodeType.EnableRepair);
            case "if":
                return new OrderNodeType?(OrderNodeType.If);
            case "while":
                return new OrderNodeType?(OrderNodeType.While);
            case "abandon":
            case "abandonquest":
                return new OrderNodeType?(OrderNodeType.AbandonQuest);
            case "walkto":
            case "strollto":
            case "rideto":
            case "moveto":
            case "navigateto":
            case "move":
            case "navigate":
            case "runto":
            case "run":
                return new OrderNodeType?(OrderNodeType.MoveTo);
            case "useitem":
                return new OrderNodeType?(OrderNodeType.UseItem);
            case "code":
            case "customobjective":
            case "custombehavior":
            case "questbehavior":
            case "script":
                return new OrderNodeType?(OrderNodeType.Code);
            default:
                return new OrderNodeType?();
        }
    }
}
