using Bots.Grind;
using CommonBehaviors.Actions;
using Levelbot.Actions.Death;
using Levelbot.Decorators.Death;
using Styx;
using Styx.Logic.Combat;
using TreeSharp;

namespace ObjectGatherer.Helpers {
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
                CombatRoutine(),
                FollowPath()
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

        private static Composite CombatRoutine() {
            return new Decorator(ctx => StyxWoW.Me.Combat,
                new PrioritySelector(
                    RoutineManager.Current.HealBehavior,
                    RoutineManager.Current.CombatBuffBehavior,
                    RoutineManager.Current.CombatBehavior
                )
            );
        }

        private static Composite FollowPath() {
            return new Decorator(ctx => !Variables.NeedToLoot && !StyxWoW.Me.Combat && !StyxWoW.Me.IsActuallyInCombat,
                new PrioritySelector(
                    new FollowPathAction()
                )
            );
        }
    }
}
