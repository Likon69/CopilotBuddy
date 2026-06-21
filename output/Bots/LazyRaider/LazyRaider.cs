/*
 * NOTE:    DO NOT POST ANY MODIFIED VERSIONS OF THIS TO THE FORUMS.
 * 
 *          DO NOT UTILIZE ANY PORTION OF THIS PLUGIN WITHOUT
 *          THE PRIOR PERMISSION OF AUTHOR.  PERMITTED USE MUST BE
 *          ACCOMPANIED BY CREDIT/ACKNOWLEDGEMENT TO ORIGINAL AUTHOR.
 * 
 * Author:  Bobby53
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

using Levelbot.Actions.Combat;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using CommonBehaviors.Actions;
using Action = TreeSharp.Action;
using Sequence = TreeSharp.Sequence;
using Styx.WoWInternals;

using Bobby53;

namespace Styx.Bot.CustomBots
{
    public class LazyRaider : BotBase
    {
        #region Overrides of BotBase

        private readonly Version _version = new Version(1, 1, 1);

        public override string Name
        {
            get { return "LazyRaider"; }
        }

        public static LocalPlayer Me { get { return StyxWoW.Me; } }
        public static bool IsInGroup { get { return Me.IsInRaid || Me.IsInParty; } }
        public static List<WoWPlayer> GroupMembers { get { return !Me.IsInRaid ? Me.PartyMembers : Me.RaidMembers; } }
        public static List<WoWPartyMember> GroupMemberInfos { get { return !Me.IsInRaid ? Me.PartyMemberInfos : Me.RaidMemberInfos; } }

        public static bool IamTheTank = false;

        public SelectTankForm _frm;
        public override System.Windows.Forms.Form ConfigurationForm
        {
            get
            {
                return _frm ?? (_frm = new SelectTankForm());
            }
        }

        private Composite _root;
        public override Composite Root
        {
            get
            {
                return _root ?? (_root =
                    new PrioritySelector(
                        CreateDetectTankBehavior(),
                        new Decorator(ret => !(Me.Mounted && CharacterSettings.Instance.UseMount && LazyRaiderSettings.Instance.DismountOnlyWithTankOrUser),
                            CreateCombatBehavior()),
                        CreateFollowBehavior()
                        )
                    );
            }
        }

        public override PulseFlags PulseFlags
        {
            get 
            { 
                PulseFlags flags = PulseFlags.Objects
                                 | PulseFlags.Plugins
                                 | PulseFlags.BotEvents
                                 | PulseFlags.InfoPanel
                                 | PulseFlags.Lua;

                if (LazyRaiderSettings.Instance.AutoTarget)
                    flags |= PulseFlags.Targeting;
                return flags;
            }
        }

        public override void Initialize()
        {
            Log(Color.Blue, "Version {0} initialized", _version);
            LazyRaiderSettings.Instance.Load();
            Logic.Profiles.ProfileManager.LoadEmpty();
            Log(Color.Blue, "Blank Profile loaded" );
            base.Initialize();
        }

        public override void Start()
        {
            Targeting.Instance.IncludeTargetsFilter += IncludeTargetsFilter;
            Lua.Events.AttachEvent("PARTY_MEMBERS_CHANGED", HandlePartyMembersChanged);
            StyxSettings.Instance.LogoutForInactivity = false;
            Log(Color.Blue, "Version {0} Started", _version);
        }

        public override void Stop()
        {
            Lua.Events.DetachEvent("PARTY_MEMBERS_CHANGED", HandlePartyMembersChanged);
            Targeting.Instance.IncludeTargetsFilter -= IncludeTargetsFilter;
            StyxSettings.Instance.LogoutForInactivity = true;
            Log(Color.Blue, "Version {0} Stopped", _version);
        }

        #endregion

        #region MISC

        public static bool IsGameStable()
        {
            return ObjectManager.IsInGame && Me != null && Me.IsValid;
        }

        private static uint lineCount = 0;
        private static string _Slogspam;

        public static void Log( string msg, params object[] args)
        {
            Log(Color.Blue, msg, args);
        }

        public static void Log(Color clr, string msg, params object[] args)
        {
            try
            {
                // following linecount hack is to stop dup line suppression of Log window
                Logging.Write(clr, "[LazyRaider] " + msg + (++lineCount % 2 == 0 ? "" : " "), args);
                _Slogspam = msg;
            }
            catch (ThreadAbortException) { throw; }
            catch (Exception e)
            {
                Log(Color.Red, "An Exception occured. Check debug log for details.");
                Logging.WriteDebug(">>> EXCEPTION: occurred logging msg: \n\t\"" + SafeLogException(msg) + "\"");
                Logging.WriteException(e);
            }
        }

        public static string SafeLogException(string msg)
        {
            msg = msg.Replace("{", "(");
            msg = msg.Replace("}", ")");
            return msg;
        }

        #endregion

        #region Targeting Filter

        private static void IncludeTargetsFilter(List<WoWObject> incomingUnits, HashSet<WoWObject> outgoingUnits)
        {
            if (LazyRaiderSettings.Instance.AutoTarget)
            {
                WoWUnit unit = GetBestTarget();
                outgoingUnits.Add(unit);
            }
        }

        private static WoWUnit GetBestTarget()
        {
            if (StyxWoW.Me.GotTarget && StyxWoW.Me.CurrentTarget.Attackable)
            {
                return StyxWoW.Me.CurrentTarget;
            }

            if (RaFHelper.Leader != null && RaFHelper.Leader.GotTarget && RaFHelper.Leader.CurrentTarget.Attackable)
            {
                return  RaFHelper.Leader.CurrentTarget;
            }

            if (Battlegrounds.IsInsideBattleground)
            {
                return (from o in ObjectManager.ObjectList
                        where o is WoWPlayer && o.Location.DistanceSqr(Me.Location) < Targeting.PullDistanceSqr
                        let p = o.ToUnit().ToPlayer()
                        where p.IsHorde != Me.IsHorde
                        orderby p.CurrentHealth
                        select p.ToUnit()).FirstOrDefault();
            }
            else
            {
                return (from o in ObjectManager.ObjectList
                        where o is WoWUnit && o.Location.DistanceSqr(Me.Location) < Targeting.PullDistanceSqr
                        let unit = o.ToUnit()
                        where unit.Attackable && (!unit.IsPlayer ? !unit.IsFriendly : unit.ToPlayer().IsHorde != Me.IsHorde)
                        orderby unit.Location.DistanceSqr(Me.Location)
                        select unit).FirstOrDefault();
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

        private static Composite CreateDetectTankBehavior()
        {
            return new PrioritySelector(
                new Decorator(ret => DoWeNeedToFindLeader(),
                    new Action(ret => DetectTheTank()))
                );
        }

        private static Composite CreateCombatBehavior()
        {
            return new PrioritySelector(
                new Decorator(ret => !StyxWoW.Me.Combat && StyxWoW.Me.IsAlive,
                    new PrioritySelector(
                        new PrioritySelector(
                            new Decorator(ctx => RoutineManager.Current.RestBehavior != null, RoutineManager.Current.RestBehavior),
                            new Decorator(ctx => RoutineManager.Current.NeedRest,
                                new Sequence(
                                    new Action(ret => TreeRoot.StatusText = "Resting"),
                                    new Action(ret => RoutineManager.Current.Rest())
                                    )
                                )
                            ),

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
                                    )
                                )
                            ),

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
                                            new Action(ret => BotPoi.Current.AsObject.ToUnit().Target())
                                            )
                                        )
                                    ),

                                new Decorator(NeedPull,
                                    new PrioritySelector(
                                        new Decorator(ctx => RoutineManager.Current.PullBuffBehavior != null,
                                            RoutineManager.Current.PullBuffBehavior),

                                        new Decorator(ctx => RoutineManager.Current.PullBehavior != null,
                                            RoutineManager.Current.PullBehavior),

                                        new ActionPull()
                                        )
                                    )
                                )
                            )
                        )
                    ),

                new Decorator(ret => StyxWoW.Me.Combat,

                    new PrioritySelector(

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
                    )));
        }

        #endregion

        #region Find Leader Behavior

        static bool haveWeChecked;

        private static bool DoWeNeedToFindLeader()
        {
            // check flag that helps avoid spamming check
            if (haveWeChecked)
                return false;

            if (RaFHelper.Leader != null && !RaFHelper.Leader.IsValid)
                RaFHelper.ClearLeader();

            return (RaFHelper.Leader == null && !Battlegrounds.IsInsideBattleground && IsInGroup );
        }

        static ObjectInvalidateDelegate invalidDelegate;

        private static RunStatus DetectTheTank()
        {
            IamTheTank = false;
            if (RaFHelper.Leader != null && invalidDelegate == null)
            {
                invalidDelegate = new ObjectInvalidateDelegate(Leader_OnInvalidate);
                RaFHelper.Leader.OnInvalidate += invalidDelegate;
            }

            if (RaFHelper.Leader != null && !RaFHelper.Leader.IsValid)
            {
                Log(Color.Blue, "Tank invalid, resetting");
                RaFHelper.ClearLeader();
            }

            if (IsInGroup && RaFHelper.Leader == null)
            {
                IamTheTank = ( GetGroupRoleAssigned( Me ) == WoWPartyMember.GroupRole.Tank );
                if ( IamTheTank || LazyRaiderSettings.Instance.NoTank)
                {
                    Log(Color.Blue, "Tank set to -ME-, max health {0}", Me.MaxHealth);
                    return RunStatus.Failure;
                }

                foreach (WoWPlayer p in GroupMembers)
                {
                    if ( GetGroupRoleAssigned( p ) == WoWPartyMember.GroupRole.Tank )
                    {
                        RaFHelper.SetLeader(p);
                        Log(Color.Blue, "Tank set to {0}, max health {1}", RaFHelper.Leader.Class, RaFHelper.Leader.MaxHealth);
                        return RunStatus.Failure;
                    }
                }

                WoWPlayer tank = (from p in GroupMembers orderby p.MaxHealth descending where !p.IsMe select p).FirstOrDefault();
                if (tank != null)
                {
                    RaFHelper.SetLeader(tank);
                    Log( Color.Blue, "Tank set to {0}, max health {1}", RaFHelper.Leader.Class, RaFHelper.Leader.MaxHealth);
                    return RunStatus.Failure;
                }
            }

            if (IsInGroup)
                Log( Color.Red, "Could not find suitable unit to Tank!");

            haveWeChecked = true;
            return RunStatus.Failure;
        }

        static void Leader_OnInvalidate()
        {
            RaFHelper.ClearLeader();
            invalidDelegate = null;
            haveWeChecked = false;
        }

        private void HandlePartyMembersChanged(object sender, LuaEventArgs args)
        {
            if (!IsInGroup)
            {
                Logging.WriteDebug("You left the group");
                haveWeChecked = false;
            }
            else if (IamTheTank || LazyRaiderSettings.Instance.NoTank)
            {
                Logging.WriteDebug("You are acting as Tank, no leader needed");
                haveWeChecked = true;
            }
            else if (RaFHelper.Leader == null)
            {
                Log("Joined party -- need to find tank");
                haveWeChecked = false;
            }
            else if (GroupMembers.Contains(RaFHelper.Leader))
            {
                Logging.WriteDebug("Party Members Changed - Tank still in group");
            }
            else
            {
                Log("Tank left group, no tank currently");
                RaFHelper.ClearLeader();
                haveWeChecked = false;
            }
        }

        public static string Safe_UnitName(WoWUnit unit)
        {
            if (unit == null)
                return "(null)";

            return unit.Class.ToString() + " (max health:" + unit.MaxHealth + ")";
        }

#if COMMENT
        public static string GetGroupRoleAssigned(WoWPlayer p)
        {
            string sRole = "NONE";
            if (ObjectManager.Me.IsInParty || ObjectManager.Me.IsInRaid)
            {
                try
                {                   
                    string luaCmd = "return UnitGroupRolesAssigned(\"" + p.Name + "\")";
                    sRole = Lua.GetReturnVal<string>(luaCmd, 0);
                }
                catch
                {
                    sRole = "NONE";
                }
            }

            return sRole;
        }
#else
        public static WoWPartyMember.GroupRole GetGroupRoleAssigned(WoWPartyMember pm)
        {
            WoWPartyMember.GroupRole role = WoWPartyMember.GroupRole.None;
            if (pm != null && IsInGroup )
            {
                const int ROLEMASK = (int)WoWPartyMember.GroupRole.None | (int)WoWPartyMember.GroupRole.Tank | (int)WoWPartyMember.GroupRole.Healer | (int)WoWPartyMember.GroupRole.Damage;
                role = (WoWPartyMember.GroupRole)((int)pm.Role & ROLEMASK );
            }

            return role;
        }

        public static WoWPartyMember.GroupRole GetGroupRoleAssigned(WoWPlayer p)
        {
            WoWPartyMember.GroupRole role = WoWPartyMember.GroupRole.None;
            if (p != null && IsInGroup)
            {
                // GroupMemberInfos.FirstOrDefault(t => t.Guid == p.Guid);
                WoWPartyMember pm = new WoWPartyMember( p.Guid, true );
                if (pm != null)
                    role = GetGroupRoleAssigned(pm);
            }

            return role;
        }
#endif

        #endregion

        #endregion

        #region Follow Behavior

        private static bool botMovement = false;

        private static Composite CreateFollowBehavior()
        {
            return new PrioritySelector(

                new Decorator(ret => !LazyRaiderSettings.Instance.FollowTank, 
                    new ActionAlwaysSucceed()),

                new Decorator(ret => !IsInGroup,
                    new ActionAlwaysSucceed()),

                new Decorator(ret => RaFHelper.Leader == null,
                    new ActionAlwaysSucceed()),

                new Decorator(ret => Me.CurrentHealth <= 1,     // if dead or ghost
                    new ActionAlwaysSucceed()),

                new Decorator(ret => RaFHelper.Leader.CurrentHealth <= 1,     // if dead or ghost
                    new ActionAlwaysSucceed()),

                new Decorator(ret => NeedToMount(),
                    new Action(delegate
                    {
                        WaitForMount();
                    })),

                new Decorator(ret => NeedToDismount(),
                    new Action(delegate
                    {
                        WaitForDismount();
                    })),

                new Decorator(ret => !RaFHelper.Leader.InLineOfSightOCD
                                    || RaFHelper.Leader.Distance > LazyRaiderSettings.Instance.FollowDistance, 
                    new Action( delegate
                        {
                            botMovement = true;

                            WoWPoint pt = RaFHelper.Leader.Location;
                            WoWPartyMember tankInfo = GroupMemberInfos.FirstOrDefault(t => t.Guid == RaFHelper.Leader.Guid);
                            if (tankInfo != null && Me.Location.Distance(tankInfo.Location3D) > 100)
                                pt = tankInfo.Location3D;

                            Navigator.MoveTo(RaFHelper.Leader.Location);
                            return RunStatus.Success;
                        })),

                new Decorator(ret => Me.IsMoving && botMovement,
                    new Action( delegate
                        {
                            botMovement = false;
                            while (IsGameStable() && Me.IsMoving)
                            {
                                WoWMovement.MoveStop();
                                if (Me.IsMoving)
                                {
                                    System.Threading.Thread.Sleep(25);
                                }
                            }

                            return RunStatus.Success;
                        }))

                );
        }

        #endregion

        public static bool NeedToDismount()
        {
            return Me.Mounted
                && CharacterSettings.Instance.UseMount
                && RaFHelper.Leader != null 
                && RaFHelper.Leader.Distance <= LazyRaiderSettings.Instance.FollowDistance
                && !RaFHelper.Leader.Mounted;
        }

        public static bool NeedToMount()
        {
            return !Me.Mounted 
                && CharacterSettings.Instance.UseMount 
                && RaFHelper.Leader != null
                && (RaFHelper.Leader.Distance > NeedToMountDistance || RaFHelper.Leader.Mounted)
                && Me.IsOutdoors 
                && Mount.CanMount();
        }

        public static int NeedToMountDistance
        {
            get
            {
                return Math.Max(CharacterSettings.Instance.MountDistance, LazyRaiderSettings.Instance.FollowDistance + 20);
            }
        }

        public static void WaitForDismount()
        {
            while (IsGameStable() && Me.CurrentHealth > 1 && Me.Mounted)
            {
                Lua.DoString("Dismount()");
                // Mount.Dismount();  // HB API forces Stop also, so use LUA to keep running and let Squire or CC stop if needed
                StyxWoW.SleepForLagDuration();
            }
        }

        public static void WaitForMount()
        {
            if (Me.Combat || Me.IsIndoors || !CharacterSettings.Instance.UseMount)
                return;

            WaitForStop();
            WoWPoint ptStop = Me.Location;

            var timeOut = new Stopwatch();
            timeOut.Start();

            if (!Mount.CanMount())
                return;

            Log("Attempting to mount via HB...");
            Mount.MountUp();
            StyxWoW.SleepForLagDuration();

            while (IsGameStable() && Me.CurrentHealth > 1 && Me.IsCasting)
            {
                Thread.Sleep(75);
            }

            if (!Me.Mounted)
            {
                Log("unable to mount after {0} ms", timeOut.ElapsedMilliseconds);
                if (ptStop.Distance(Me.Location) != 0)
                    Log("character was stopped but somehow moved {0:F3} yds while trying to mount", ptStop.Distance(Me.Location));
            }
            else
            {
                Log("Mounted");
            }
        }

        public static void WaitForStop()
        {
            // excessive attempt to make sure HB doesn't have any cached movement
            WoWMovement.MoveStop(WoWMovement.MovementDirection.AutoRun);
            WoWMovement.MoveStop(WoWMovement.MovementDirection.Backwards);
            WoWMovement.MoveStop(WoWMovement.MovementDirection.ClickToMove);
            WoWMovement.MoveStop(WoWMovement.MovementDirection.Descend);
            WoWMovement.MoveStop(WoWMovement.MovementDirection.Forward);
            WoWMovement.MoveStop(WoWMovement.MovementDirection.ForwardBackMovement);
            WoWMovement.MoveStop(WoWMovement.MovementDirection.JumpAscend);
            WoWMovement.MoveStop(WoWMovement.MovementDirection.PitchDown);
            WoWMovement.MoveStop(WoWMovement.MovementDirection.PitchUp);
            WoWMovement.MoveStop(WoWMovement.MovementDirection.StrafeLeft);
            WoWMovement.MoveStop(WoWMovement.MovementDirection.StrafeRight);
            WoWMovement.MoveStop(WoWMovement.MovementDirection.TurnLeft);
            WoWMovement.MoveStop(WoWMovement.MovementDirection.TurnRight);

            WoWMovement.MoveStop(WoWMovement.MovementDirection.All);
            WoWMovement.MoveStop();

            do
            {
                StyxWoW.SleepForLagDuration();
            } while (IsGameStable() && Me.CurrentHealth > 1 && Me.IsMoving);
        }

    }
}

