// Decompiled with JetBrains decompiler
// Type: Bots.Quest.QuestOrder.ForcedIf
// Assembly: Honorbuddy, Version=2.0.0.5999, Culture=neutral, PublicKeyToken=50a565ab5c01ae50
// MVID: FB7FEB85-27C0-4D17-B8DE-615FDFDA7752
// Assembly location: C:\Users\Texy6\Desktop\Honorbuddy-cleaned.exe

using Bots.Quest.Actions;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Profiles.Quest;
using System;
using System.Collections.Generic;
using System.Drawing;
using TreeSharp;

#nullable disable
namespace Bots.Quest.QuestOrder;

public class ForcedIf : ForcedBehavior
{
    private QuestOrder conditionalOrder;
    private Composite behaviorExecutor;

    public ForcedIf(IfNode node)
    {
        this.IfNode = node != null ? node : throw new ArgumentNullException(nameof(node));
    }

    public IfNode IfNode { get; private set; }

    protected override Composite CreateBehavior()
    {
        Composite behavior = this.behaviorExecutor;
        if ((object)behavior == null)
            behavior = this.behaviorExecutor = (Composite)new ForcedBehaviorExecutor(this.conditionalOrder);
        return behavior;
    }

    public override void OnStart()
    {
        try
        {
            if (this.IfNode.Condition())
            {
                Logging.WriteDiagnostic("[If] Condition is true, executing If body");
                this.conditionalOrder = new QuestOrder(new OrderNodeCollection((IEnumerable<OrderNode>)this.IfNode.Body));
            }
            else
            {
                OrderNodeCollection matchingElseIfBody;
                if (this.TryGetMatchingElseIf(out matchingElseIfBody))
                {
                    Logging.WriteDiagnostic("[ElseIf] Condition matched, executing ElseIf body");
                    this.conditionalOrder = new QuestOrder(new OrderNodeCollection((IEnumerable<OrderNode>)matchingElseIfBody));
                }
                else if (this.IfNode.Else != null)
                {
                    Logging.WriteDiagnostic("[Else] No conditions matched, executing Else body");
                    this.conditionalOrder = new QuestOrder(new OrderNodeCollection((IEnumerable<OrderNode>)this.IfNode.Else.Body));
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Write(Color.Red, "Unable to evaluate compile condition in If tag. Please check your profile.");
            Logging.Write(Color.Red, "CopilotBuddy stopped!");
            Logging.WriteException(ex);
            TreeRoot.Stop();
        }
        if (this.conditionalOrder == null)
            return;
        this.conditionalOrder.IgnoreCheckpoints = QuestState.Instance.Order.IgnoreCheckpoints;
        this.conditionalOrder.UpdateNodes();
    }

    private bool TryGetMatchingElseIf(out OrderNodeCollection matchingBody)
    {
        bool flag;
        using (List<ElseIf>.Enumerator enumerator = this.IfNode.ElseIfs.GetEnumerator())
        {
            ElseIf current;
            do
            {
                if (enumerator.MoveNext())
                    current = enumerator.Current;
                else
                    goto label_6;
            }
            while (!current.Condition());
            matchingBody = current.Body;
            flag = true;
            goto label_7;
        }
    label_6:
        matchingBody = (OrderNodeCollection)null;
        return false;
    label_7:
        return flag;
    }

    public override bool IsDone
    {
        get
        {
            return this.conditionalOrder == null || this.conditionalOrder.Nodes == null || this.conditionalOrder.Nodes.Count <= 0;
        }
    }
}
