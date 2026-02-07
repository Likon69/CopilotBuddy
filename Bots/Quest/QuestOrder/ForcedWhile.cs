// Decompiled with JetBrains decompiler
// Type: Bots.Quest.QuestOrder.ForcedWhile
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
using System.Threading;
using TreeSharp;

#nullable disable
namespace Bots.Quest.QuestOrder;

public class ForcedWhile : ForcedBehavior
{
    private WhileComposite whileComposite;

    public ForcedWhile(WhileNode node)
    {
        this.WhileNode = node != null ? node : throw new ArgumentNullException(nameof(node));
    }

    public WhileNode WhileNode { get; private set; }

    protected override Composite CreateBehavior()
    {
        return (Composite)(this.whileComposite ?? (this.whileComposite = new WhileComposite(this.WhileNode)));
    }

    public override bool IsDone
    {
        get => (Composite)this.whileComposite != (Composite)null && this.whileComposite.IsDone;
    }

    private class WhileComposite : Composite
    {
        private readonly WhileNode whileNode;
        private ForcedBehaviorExecutor behaviorExecutor;
        private bool hasInitialized;

        public WhileComposite(WhileNode node) => this.whileNode = node;

        public bool IsDone { get; private set; }

        protected override IEnumerable<RunStatus> Execute(object context)
        {
            if (!this.hasInitialized)
            {
                bool flag;
                try
                {
                    flag = this.whileNode.Condition();
                }
                catch (Exception ex)
                {
                    if (ex is ThreadAbortException)
                        throw;
                    Logging.Write(Color.Red, "Unable to evaluate compile condition in If tag. Please check your profile.");
                    Logging.Write(Color.Red, "CopilotBuddy stopped!");
                    Logging.WriteException(ex);
                    TreeRoot.Stop();
                    yield break;
                }
                if (!flag)
                {
                    this.IsDone = true;
                    yield return RunStatus.Success;
                    yield break;
                }
                Logging.WriteDiagnostic("[While] Condition is true, executing While body");
                this.hasInitialized = true;
                QuestOrder order = new QuestOrder(new OrderNodeCollection((IEnumerable<OrderNode>)this.whileNode.Body))
                {
                    IgnoreCheckpoints = QuestState.Instance.Order.IgnoreCheckpoints
                };
                order.UpdateNodes();
                this.behaviorExecutor = new ForcedBehaviorExecutor(order);
            }
            if (this.behaviorExecutor.Order.Nodes.Count <= 0)
            {
                this.hasInitialized = false;
                yield return RunStatus.Success;
            }
            else
            {
                this.behaviorExecutor.Start(context);
                while (this.behaviorExecutor.Tick(context) == RunStatus.Running)
                    yield return RunStatus.Running;
                this.behaviorExecutor.Stop(context);
                yield return this.behaviorExecutor.LastStatus ?? RunStatus.Failure;
            }
        }
    }
}
