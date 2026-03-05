// Decompiled with JetBrains decompiler
// Type: Bots.Quest.QuestOrder.ForcedGrindTo
// Assembly: Honorbuddy, Version=2.0.0.5999, Culture=neutral, PublicKeyToken=50a565ab5c01ae50
// MVID: FB7FEB85-27C0-4D17-B8DE-615FDFDA7752
// Assembly location: C:\Users\Texy6\Desktop\Honorbuddy-cleaned.exe

using Bots.Grind;
using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.AreaManagement;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Profiles.Quest;
using Styx.WoWInternals;
using System;
using TreeSharp;

#nullable disable
namespace Bots.Quest.QuestOrder;

public class ForcedGrindTo : ForcedBehavior
{
    public ForcedGrindTo(GrindToNode node)
    {
        this.Node = node != null ? node : throw new ArgumentNullException(nameof(node));
    }

    public GrindToNode Node { get; private set; }

    protected override Composite CreateBehavior() => (Composite)LevelBot.CreateRoamBehavior();

    public override bool IsDone
    {
        get
        {
            if (this.Node.Condition != null)
                return this.Node.Condition();
            float fraction = ObjectManager.Me.LevelFraction;
            float target = this.Node.Level;
            bool done = fraction >= target;
            if (done)
                Logging.WriteDiagnostic("[ForcedGrindTo] IsDone=true: LevelFraction={0:F2} >= Level={1:F1}", fraction, target);
            return done;
        }
    }

    public override void OnStart()
    {
        if ((Area)QuestState.Instance.CurrentGrindArea == (Area)null)
        {
            Logging.Write("Reached GrindTo (level: {0}) without a current grind area. Can't continue.", (object)this.Node.Level);
            TreeRoot.Stop();
        }
        else
        {
            StyxWoW.AreaManager.SetArea(QuestState.Instance.CurrentGrindArea);
            Targeting.Instance.IncludeTargetsFilter += new IncludeTargetsFilterDelegate(LevelBot.LevelBotIncludeTargetsFilter);
            string goalText = this.GetGoalText();
            Logging.Write("[GrindTo] {0}, Target Level: {1}", (object)goalText, (object)this.Node.Level);
            TreeRoot.GoalText = goalText;
        }
    }

    private string GetGoalText()
    {
        if (!string.IsNullOrEmpty(this.Node.GoalText))
            return this.Node.GoalText;
        return (double)this.Node.Level == -1.0 ? "Grinding" : string.Format("Grinding to level {0}", (object)this.Node.Level);
    }

    public override void Dispose()
    {
        Targeting.Instance.IncludeTargetsFilter -= new IncludeTargetsFilterDelegate(LevelBot.LevelBotIncludeTargetsFilter);
        StyxWoW.AreaManager.SetArea((GrindArea)null);
    }

    public override string ToString()
    {
        return string.Format("[ForcedGrindTo Level: {0}]", (object)this.Node.Level);
    }
}
