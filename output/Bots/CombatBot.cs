using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Levelbot.Actions.Combat;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.Logic.Profiles;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;
using Sequence = TreeSharp.Sequence;
using Styx.WoWInternals;

namespace Styx.Bot.CustomBots
{
    public class CombatBot : BotBase
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey); 

        #region Overrides of BotBase


        public override string Name
        {
            get { return "Combat Bot"; }
        }

        private Composite _root;
        public override Composite Root
        {
            get
            {
                return _root ?? (_root = 
                    new PrioritySelector(
                        CreateCombatBehavior()
                        //,
                        //CreateFollowBehavior()
                        )
                    );
            }
        }

        public override void Initialize()
        {
            BotEvents.Player.OnMapChanged += Player_OnMapChanged;
        }

        private void Player_OnMapChanged(BotEvents.Player.MapChangedEventArgs args)
        {
            _root = null;
        }

        public override void Pulse()
        {
            //if ((GetAsyncKeyState(Keys.NumPad0) & 1) != 0)
            //    BotPoi.Current = new BotPoi(StyxWoW.Me.CurrentTarget, PoiType.Kill);
        }

        public override PulseFlags PulseFlags
        {
            get { return PulseFlags.All; }
        }

        public override void Start()
        {
            Targeting.Instance.IncludeTargetsFilter += IncludeTargetsFilter;
            StyxSettings.Instance.LogoutForInactivity = false;

            // Load an empty profile
            if (ProfileManager.CurrentProfile == null)
                ProfileManager.LoadEmpty();
        }

        public override void Stop()
        {
            Targeting.Instance.IncludeTargetsFilter -= IncludeTargetsFilter;
            StyxSettings.Instance.LogoutForInactivity = true;
        }

        #endregion

        #region Targeting Filter

        private static void IncludeTargetsFilter(List<WoWObject> incomingUnits, HashSet<WoWObject> outgoingUnits)
        {
            if (StyxWoW.Me.GotTarget && StyxWoW.Me.CurrentTarget.Attackable)
            {
                outgoingUnits.Add(StyxWoW.Me.CurrentTarget);
            }
        }

        #endregion

        #region Behaviors

        #region Combat Behavior

        private static bool NeedPull(object context)
        {
            var target = StyxWoW.Me.CurrentTarget;

            if (target == null)
                return false;

            if (!target.InLineOfSight)
                return false;

            if (target.Distance > Targeting.PullDistance)
                return false;

            return true;
        }

        private static Composite CreateCombatBehavior()
        {
            return new PrioritySelector(

                new Decorator(ret => !StyxWoW.Me.Combat,
                    new PrioritySelector(

                        #region Rest

                        new PrioritySelector(
                        // Use the bt
                        new Decorator(ctx => RoutineManager.Current.RestBehavior != null,
                            RoutineManager.Current.RestBehavior),

                            // new ActionDebugString("[Combat] Rest -> Old Behavior"),
                            // don't use the bt
                            new Decorator(ctx => RoutineManager.Current.NeedRest,
                                new Sequence(
                                    new Action(ret => TreeRoot.StatusText = "Resting"),
                                    new Action(ret => RoutineManager.Current.Rest())))
                                    ),

                        #endregion

                        #region PreCombatBuffs

                        new PrioritySelector(
                            // new ActionDebugString("[Combat] Checking PCBBehavior"),
                            // Use the bt
                            new Decorator(ctx => RoutineManager.Current.PreCombatBuffBehavior != null,
                                RoutineManager.Current.PreCombatBuffBehavior),

                            // don't use the bt
                            // new ActionDebugString("[Combat] Checking PCBOld"),
                            new Decorator(
                                ctx => RoutineManager.Current.NeedPreCombatBuffs,
                                new Sequence(
                                    new Action(ret => TreeRoot.StatusText = "Applying pre-combat buffs"),
                                    new Action(ret => RoutineManager.Current.PreCombatBuff())
                                    ))),

                        #endregion

                        #region Pull

                        // new ActionDebugString("[Combat] Pull"),
                        // Don't pull, unless we've decided to pull already.
                        new Decorator(ret => BotPoi.Current.Type == PoiType.Kill,
                            new PrioritySelector(

                                // Make sure we have a valid target list.
                                new Decorator(ret => Targeting.Instance.TargetList.Count != 0,
                                    // Force the 'correct' POI to be our target.
                                    new Decorator(ret => BotPoi.Current.AsObject != Targeting.Instance.FirstUnit &&
                                        BotPoi.Current.Type == PoiType.Kill,
                                        new Sequence(

                                            new Action(ret => BotPoi.Current = new BotPoi(Targeting.Instance.FirstUnit, PoiType.Kill)),
                                            new Action(ret => BotPoi.Current.AsObject.ToUnit().Target())))),

                                        new Decorator(NeedPull,
                                            new PrioritySelector(
                                                new Decorator(ctx => RoutineManager.Current.PullBuffBehavior != null,
                                                    RoutineManager.Current.PullBuffBehavior),

                                                new Decorator(ctx => RoutineManager.Current.PullBehavior != null,
                                                    RoutineManager.Current.PullBehavior),

                                                    new ActionPull())))))),
                        #endregion

                new Decorator(ret => StyxWoW.Me.Combat,

                    new PrioritySelector(

                        #region Heal

                        new PrioritySelector(
                        // Use the Behavior
                            new Decorator(ctx => RoutineManager.Current.HealBehavior != null,
                                new Sequence(
                                    RoutineManager.Current.HealBehavior,
                                    new Action(delegate { return RunStatus.Success; })
                                    )),

                            // Don't use the Behavior
                            new Decorator(ctx => RoutineManager.Current.NeedHeal,
                                new Sequence(
                                    new Action(ret => TreeRoot.StatusText = "Healing"),
                                    new Action(ret => RoutineManager.Current.Heal())
                                    ))),

                        #endregion

                        #region Combat Buffs

                        new PrioritySelector(
                        // Use the Behavior
                            new Decorator(ctx => RoutineManager.Current.CombatBuffBehavior != null,
                                        new Sequence(
                                            RoutineManager.Current.CombatBuffBehavior,
                                            new Action(delegate { return RunStatus.Success; })
                                            )
                                ),

                            // Don't use the Behavior
                            new Decorator(ctx => RoutineManager.Current.NeedCombatBuffs,
                                        new Sequence(
                                            new Action(ret => TreeRoot.StatusText = "Applying Combat Buffs"),
                                            new Action(ret => RoutineManager.Current.CombatBuff())
                                            ))),

                        #endregion

                        #region Combat

                        new PrioritySelector(
                            // Use the Behavior
                            new Decorator(ctx => RoutineManager.Current.CombatBehavior != null,
                                new PrioritySelector(
                                    RoutineManager.Current.CombatBehavior,
                                    new Action(delegate { return RunStatus.Success; })
                                    )),

                            // Don't use the Behavior
                            new Sequence(
                                new Action(ret => TreeRoot.StatusText = "Combat"),
                                new Action(ret => RoutineManager.Current.Combat())))

                        #endregion

                        )));
        }

        #endregion

        #region Follow Behavior

        private static WoWUnit _followMe;
        private static bool _isInitialized;
        private static WoWUnit FollowMe
        {
            get
            {
                if(!_isInitialized && _followMe != null)
                    _followMe.OnInvalidate += new ObjectInvalidateDelegate(_followMe_OnInvalidate);

                if (_followMe == null || !_followMe.IsValid)
                {
                    if (StyxWoW.Me.IsInInstance)
                    {
                        for (int i = 1; i < 5; i++)
                        {
                            string role = Lua.GetReturnVal<string>(string.Format("return UnitGroupRolesAssigned('party{0}')", i), 0);
                            if (role == "TANK")
                                _followMe = ObjectManager.GetObjectByGuid<WoWPlayer>(StyxWoW.Me.GetPartyMemberGuid(i - 1));
                        }
                    }
                    else
                    {
                        _followMe = RaFHelper.Leader ?? StyxWoW.Me.PartyMembers.FirstOrDefault();
                    }
                    RaFHelper.SetLeader(_followMe.Guid);
                }

                if (_followMe.Guid != RaFHelper.Leader.Guid)
                    _followMe = RaFHelper.Leader;


                if (_followMe == null)
                    Logging.Write("Could not find suitable unit to follow!");

                return _followMe;
            }
        }

        static void _followMe_OnInvalidate()
        {
            _followMe = null;
        }

        private static Composite CreateFollowBehavior()
        {
            return new PrioritySelector(
                
                new Decorator(ret => StyxWoW.Me.IsInParty && (FollowMe != null && FollowMe.Distance > 10 || FollowMe != null && !FollowMe.InLineOfSight),
                    new Action(ret => Navigator.MoveTo(FollowMe.Location))
                    )
                );
        }

        #endregion

        #endregion
    }
}
