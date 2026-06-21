using Bots.Grind;
using CommonBehaviors.Actions;
using Levelbot.Actions.Death;
using Levelbot.Decorators.Death;
using Styx;
using Styx.Logic.Combat;
using TreeSharp;

namespace Templar.Helpers {
    public class Composites {

        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        // ===========================================================
        // Constructors
        // ===========================================================

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        public static Composite CreateRoot() {
            return new PrioritySelector(
                DeathRoutine(),
                PreCombatRoutine(),
                PullRoutine(),
                CombatRoutine()
            );
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

        private static Composite DeathRoutine() {
            return new Decorator(ctx => StyxWoW.Me.IsDead || StyxWoW.Me.IsGhost,
                new PrioritySelector(
                    new DecoratorNeedToRelease(new ActionReleaseFromCorpse()),
                    new DecoratorNeedToMoveToCorpse(LevelBot.CreateDeathBehavior()),
                    new DecoratorNeedToTakeCorpse(LevelBot.CreateDeathBehavior()),
                    new ActionSuceedIfDeadOrGhost()
                )
            );
        }

        private static Composite PreCombatRoutine() {
            return new Decorator(ctx => !StyxWoW.Me.Combat && !StyxWoW.Me.IsActuallyInCombat,
                new PrioritySelector(
                    new Sequence(
                        RoutineManager.Current.RestBehavior,
                        new ActionAlwaysSucceed()
                    ),
                    new Sequence(
                        RoutineManager.Current.PreCombatBuffBehavior,
                        new ActionAlwaysSucceed()
                    )
                )
            );
        }

        public static Composite PullRoutine() {
            return new Decorator(ctx => !StyxWoW.Me.IsFlying,
                new Decorator(ctx => StyxWoW.Me.CurrentTarget != null && Variables.NextMob != null && StyxWoW.Me.CurrentTarget == Variables.NextMob && PriorityTreeState.TreeState == PriorityTreeState.State.Pulling && Variables.NeedToPull,
                    new Sequence(
                        RoutineManager.Current.PullBehavior,
                        new ActionAlwaysFail()
                    )
                )
            );
        }

        private static Composite CombatRoutine() {
            return new Decorator(ctx => StyxWoW.Me.Combat,
                new PrioritySelector(
                    new Sequence(
                        RoutineManager.Current.HealBehavior,
                        new ActionAlwaysFail()
                    ),
                    new Sequence(
                        RoutineManager.Current.CombatBuffBehavior,
                        new ActionAlwaysFail()
                    ),
                    new Sequence(
                        RoutineManager.Current.CombatBehavior,
                        new ActionAlwaysFail()
                    )
                )
            );
        }
    }
}
