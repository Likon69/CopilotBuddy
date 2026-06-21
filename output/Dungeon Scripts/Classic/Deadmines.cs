using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

using CommonBehaviors.Actions;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.POI;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using TreeSharp;
using Styx;
using Styx.WoWInternals.WoWObjects;
using System.Collections.Generic;
using Action = TreeSharp.Action;

#if USE_DUNGEONBUDDY_DLL
using Bots.DungeonBuddyDll;
using Bots.DungeonBuddyDll.Attributes;
using Bots.DungeonBuddyDll.Profiles;
using Bots.DungeonBuddyDll.Helpers;
namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Classic
#else
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
    using Bots.DungeonBuddy.Profiles;
namespace Bots.DungeonBuddy.Dungeon_Scripts.Classic
#endif
{
    public class TheDeadmines : Dungeon
    {
        #region Overrides of Dungeon

        /// <summary> The mapid of this dungeon. </summary>
        /// <value>The map identifier.</value>
        public override uint DungeonId
        {
            get { return 6; }
        }

        public override WoWPoint Entrance { get { return new WoWPoint(-11208.21, 1680.011, 23.94507); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(-14.68, -388.10, 63.06); } }

        #endregion

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            base.IncludeTargetsFilter(incomingunits, outgoingunits);

            foreach (var unit in incomingunits.Select(obj => obj.ToUnit()))
            {
                if (!StyxWoW.Me.Combat && ScriptHelpers.Tank == StyxWoW.Me)
                {
                    //Force add bugged mobs to pull, such as the goblin and a few defies that return !IsHostile
                    if ((unit.DistanceSqr < 40 * 40 && !unit.IsTargetingMyPartyMember) && (unit.Entry == 48279 || unit.Entry == 48502 || unit.Entry == 48417 || unit.Entry == 48522 || unit.Entry == 47714))
                    {
                        outgoingunits.Add(unit);
                    }
                }

                if (StyxWoW.Me.Combat)
                {
                    //Force it to attack Helix
                    if ((unit.DistanceSqr < 40 * 40 && !unit.IsTargetingMyPartyMember) && (unit.Entry == 47296 || unit.Entry == 47739))
                    {
                        outgoingunits.Add(unit);
                    }
                }

            }
        }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var t in units)
            {
                var prioObject = t.Object;

                //Target good ones first
                if (prioObject.Entry == 48418 || prioObject.Entry == 11318 || prioObject.Entry == 48279)
                {
                    t.Score += 400;
                }
                //Vapors
                if (prioObject.Entry == 47714)
                {
                    t.Score += 400;
                }
                //Admiral
                if (prioObject.Entry == 47626)
                {
                    t.Score += 600;
                }
            }
        }

        public override void OnEnter()
        {
            LandedInWater = false;

            Lua.Events.AttachEvent("COMBAT_LOG_EVENT_UNFILTERED", HandleSpellCast);
            Lua.Events.AddFilter(
                "COMBAT_LOG_EVENT_UNFILTERED",
                "return args[2] == 'SPELL_CAST_START'");
        }

        public override void OnExit()
        {
            Lua.Events.DetachEvent("COMBAT_LOG_EVENT_UNFILTERED", HandleSpellCast);
        }

        #region Encounter Handlers

        readonly WoWPoint _firstCannonLoc = new WoWPoint(-89.25, -782.52, 17.17);
        readonly WoWPoint _firstCannonTargetLoc = new WoWPoint(-99.35, -704.08, 9.09);
        readonly WoWPoint _secondCannonLoc = new WoWPoint(-82.31, -775.5, 26.81);
        readonly WoWPoint _secondCannonTargetLoc = new WoWPoint(-89.69, -723.89, 8.57);
        readonly WoWPoint _thirdCannonLoc = new WoWPoint(-72.11, -786.89, 39.47);
        readonly WoWPoint _thirdCannonTargetLoc = new WoWPoint(-74.14581, -730.1188, 8.663315);
        readonly WoWPoint _fourthCannonLoc = new WoWPoint(-58.64, -787.13, 39.27);
        readonly WoWPoint _fourthCannonTargetLoc = new WoWPoint(-50.85767, -729.6558, 9.288933);
        readonly WoWPoint _fifthCannonLoc = new WoWPoint(-46.90, -783.15, 18.41);
        readonly WoWPoint _fifthCannonTargetLoc = new WoWPoint(-32.16543, -726.563, 8.508902);
        readonly WoWPoint _sixthCannonLoc = new WoWPoint(-40.00, -793.30, 39.39);
        readonly WoWPoint _sixthCannonTargetLoc = new WoWPoint(-12.92573, -738.6484, 8.905853);
        readonly WoWPoint _seventhCannonLoc = new WoWPoint(-30.26, -793.07, 19.13);
        readonly WoWPoint _seventhCannonTargetLoc = new WoWPoint(-0.362588, -766.2479, 9.636906);

        private readonly WaitTimer _firstCannonTargetTimer = new WaitTimer(TimeSpan.FromSeconds(5));
        private readonly WaitTimer _secondCannonTargetTimer = new WaitTimer(TimeSpan.FromSeconds(5));
        private readonly WaitTimer _thirdCannonTargetTimer = new WaitTimer(TimeSpan.FromSeconds(5));
        private readonly WaitTimer _fourthCannonTargetTimer = new WaitTimer(TimeSpan.FromSeconds(5));
        private readonly WaitTimer _fifthCannonTargetTimer = new WaitTimer(TimeSpan.FromSeconds(5));
        private readonly WaitTimer _sixthCannonTargetTimer = new WaitTimer(TimeSpan.FromSeconds(5));
        private readonly WaitTimer _seventhCannonTargetTimer = new WaitTimer(TimeSpan.FromSeconds(5));

        public bool LandedInWater;

        private void HandleSpellCast(object sender, LuaEventArgs args)
        {
            var spellId = (int)(double)args.Args[11];
            if ((string)args.Args[1] == "SPELL_CAST_START" && (spellId == 89757 || spellId == 91065))
            {
                var sourceGuid = ulong.Parse(args.Args[3].ToString().Replace("0x", string.Empty), NumberStyles.HexNumber);
                var cannon = ObjectManager.GetAnyObjectByGuid<WoWUnit>(sourceGuid);
                var waitTimer = GetWaitTimerForCannon(cannon);
                if (waitTimer != null)
                {
                    waitTimer.Reset();
                }
            }
        }

        WaitTimer GetWaitTimerForCannon(WoWUnit cannon)
        {
            if (cannon == null)
                return null;

            if (cannon.Location.Distance2DSqr(_firstCannonLoc) < 2 * 2)
                return _firstCannonTargetTimer;

            if (cannon.Location.Distance2DSqr(_secondCannonLoc) < 2 * 2)
                return _secondCannonTargetTimer;

            if (cannon.Location.Distance2DSqr(_thirdCannonLoc) < 2 * 2)
                return _thirdCannonTargetTimer;

            if (cannon.Location.Distance2DSqr(_fourthCannonLoc) < 2 * 2)
                return _fourthCannonTargetTimer;

            if (cannon.Location.Distance2DSqr(_fifthCannonLoc) < 2 * 2)
                return _fifthCannonTargetTimer;

            if (cannon.Location.Distance2DSqr(_sixthCannonLoc) < 2 * 2)
                return _sixthCannonTargetTimer;

            if (cannon.Location.Distance2DSqr(_seventhCannonLoc) < 2 * 2)
                return _seventhCannonTargetTimer;

            return null;
        }

        private WoWGameObject Cannon
        {
            get
            {
                return ObjectManager.
                    GetObjectsOfType<
                        WoWGameObject>().
                    FirstOrDefault(
                        x =>
                        x.
                            Entry == 16398);
            }
        }


        private WoWGameObject Door
        {
            get
            {
                return ObjectManager.
                    GetObjectsOfType<
                        WoWGameObject>().
                    FirstOrDefault(
                        x =>
                        x.
                            Entry == 16397);
            }
        }

        private WoWDynamicObject RageZone
        {
            get
            {
                return ObjectManager.
                    GetObjectsOfType<
                        WoWDynamicObject>().
                    FirstOrDefault(
                        x =>
                        x.
                            Entry == 90932);
            }
        }

        private static readonly Stopwatch CannonWatcher = new Stopwatch();
        
        [EncounterHandler(47403, Mode = CallBehaviorMode.Proximity)]
        public Composite DefiasReaper() { return ScriptHelpers.CreateClearArea(()=>new WoWPoint(-209.3573,-567.5178,20.97694),50,u=>u.Entry == 47403); }

        [EncounterHandler(0)]
        public Composite RootLogic()
        {
            WoWUnit bestTarget = null;

            return
                new PrioritySelector(

                    //Sticky Bombs
                    ScriptHelpers.CreateRunAwayFromBad(ctx => ScriptHelpers.FindBestTargetWithIdsRange(4, 47314) != null, 3f, u => u.Entry == 47314 && u.Distance <= 3),

                    //Tank moves out of RageZones and Ranged DPS move in
                    new Decorator(nat => RageZone != null && StyxWoW.Me.IsActuallyInCombat,
                           new PrioritySelector(
                                new Decorator(nat => StyxWoW.Me.IsDps() && StyxWoW.Me.IsRange() && RageZone.Location.Distance(ScriptHelpers.Tank.Location) < 15 && RageZone.Location.Distance(StyxWoW.Me.Location) > 5,
                                    new Sequence(
                                        new Action(ret => Navigator.MoveTo(RageZone.Location))
                                    )),
                                new Decorator(nat => StyxWoW.Me.IsTank() && RageZone.Location.Distance(StyxWoW.Me.Location) < 10,
                                        ScriptHelpers.CreateRunAwayFromBad(ctx => RageZone != null, 10f, u => u.Entry == 90932 && u.Distance <= 10))

                                    )),
                     // Water Handler for falling in

                     new Decorator(nat => StyxWoW.Me.IsSwimming,
                         new PrioritySelector(
                             new Decorator(nat => !LandedInWater,
                                 new Sequence(
                                     new Action(nat => LandedInWater = true),
                                     new Action(nat => Logging.Write("We landed in the water! Moving back to dock entrance.")))),

                              new Decorator(nat => StyxWoW.Me.Location.Distance(new WoWPoint(-81.09593, -695.2491, 0.0303379)) > 3,
                                     new Action(nat => WoWMovement.ClickToMove(new WoWPoint(-81.09593, -695.2491, 0.0303379)))))),

                     new Decorator(nat => !StyxWoW.Me.IsSwimming && LandedInWater && StyxWoW.Me.Location.Distance(new WoWPoint(-106.4652, -685.4393, 6.44209)) > 3,
                               new Action(nat => Navigator.MoveTo(new WoWPoint(-106.4652, -685.4393, 6.44209)))),

                     new Decorator(nat => !StyxWoW.Me.IsSwimming && LandedInWater && StyxWoW.Me.Location.Distance(new WoWPoint(-106.4652, -685.4393, 6.44209)) < 3,
                         new Sequence(
                               new Action(nat => LandedInWater = false),
                               new Action(nat => WoWMovement.ClickToMove(new WoWPoint(-102.3294, -684.3071, 7.425116))),
                               new Action(nat => Thread.Sleep(1000)))),

            //Safety Check for falling onto the ship side

                     new Decorator(nat => StyxWoW.Me.Location.Distance(new WoWPoint(-54.77596, -796.0432, 33.53532)) < 4,
                                     new Action(nat => WoWMovement.ClickToMove(new WoWPoint(-61.75788, -782.5929, 17.87629)))),


                     ScriptHelpers.CreateRunAwayFromLocation(ctx => !LandedInWater && StyxWoW.Me.Location.Distance(_firstCannonTargetLoc) < 5, 2.2f, () => _firstCannonTargetLoc),
                     ScriptHelpers.CreateRunAwayFromLocation(ctx => !_secondCannonTargetTimer.IsFinished && !LandedInWater && StyxWoW.Me.Location.Distance(_secondCannonTargetLoc) < 5, 2.2f, () => _secondCannonTargetLoc),
                     ScriptHelpers.CreateRunAwayFromLocation(ctx => !LandedInWater && StyxWoW.Me.Location.Distance(_thirdCannonTargetLoc) < 5, 2.2f, () => _thirdCannonTargetLoc),
                     ScriptHelpers.CreateRunAwayFromLocation(ctx => !LandedInWater && StyxWoW.Me.Location.Distance(_fourthCannonTargetLoc) < 5, 2.2f, () => _fourthCannonTargetLoc),
                     ScriptHelpers.CreateRunAwayFromLocation(ctx => !LandedInWater && StyxWoW.Me.Location.Distance(_fifthCannonTargetLoc) < 5, 2.2f, () => _fifthCannonTargetLoc),
                     ScriptHelpers.CreateRunAwayFromLocation(ctx => !LandedInWater && StyxWoW.Me.Location.Distance(_sixthCannonTargetLoc) < 5, 2.2f, () => _sixthCannonTargetLoc),
                     ScriptHelpers.CreateRunAwayFromLocation(ctx => !LandedInWater && StyxWoW.Me.Location.Distance(_seventhCannonTargetLoc) < 5, 2.2f, () => _seventhCannonTargetLoc)



                        
                    );
        }

        [EncounterHandler(47162, "Glubtok")]
        public Composite GlubtokFight()
        {

            return new PrioritySelector(
                    ScriptHelpers.CreateTankFaceAwayGroupUnit(10));
        }

        [ObjectHandler(16398, "Defias Cannon", ObjectRange = 30)]
        public Composite DefiasCannonHandler()
        {
            return new Decorator(r=>Cannon != null && Door != null && StyxWoW.Me.IsTank() && !StyxWoW.Me.Combat && Cannon.State == WoWGameObjectState.Ready && Door.State != WoWGameObjectState.ActiveAlternative,
                        new Action(z=>Cannon.Interact()));
        }


        [EncounterHandler(43778, "Foe Reaper 5000")]
        public Composite FoeReaperFight()
        {
            WoWUnit bestTarget = null;
            return new PrioritySelector(
                    ScriptHelpers.CreateRunAwayFromBad(ctx => ScriptHelpers.CurrentBoss != null && (ScriptHelpers.CurrentBoss.CastingSpellId == 88481 || ScriptHelpers.CurrentBoss.CastingSpellId == 88501) && StyxWoW.Me.IsFollower(), 18f, u => u.Entry == 43778 && u.Distance <= 18),
                    
                    ScriptHelpers.CreateTankFaceAwayGroupUnit(10));
        }
        [EncounterHandler(47626, "Admiral Ripsnarl", Mode = CallBehaviorMode.CurrentBoss)]
        public Composite AdmiralRipsnarlFight()
        {
            var deckCenter = new WoWPoint(-65.06589, -819.3928, 41.07232);
            WoWUnit boss = null;

            return new PrioritySelector(ctx => boss = ctx as WoWUnit,
                // handle vanish.
                                        new Decorator(
                                            ctx => boss == null && StyxWoW.Me.Location.DistanceSqr(deckCenter) < 30 * 30 && Targeting.Instance.FirstUnit == null,
                                            new Sequence(
                                                new WaitContinue(20, ctx => Targeting.Instance.FirstUnit != null,
                                                                 new Action(ret => Navigator.MoveTo(deckCenter))),
                                                new DecoratorContinue(
                                                    ctx =>
                                                    Targeting.Instance.FirstUnit == null &&
                                                    !ObjectManager.GetObjectsOfType<WoWUnit>().Any(u => u.Entry == 47626),
                                                    new Action(ctx => BossManager.CurrentBoss.MarkAsDead()))
                                                )),
                                      new Decorator(ctx => StyxWoW.Me.CurrentTarget != null && StyxWoW.Me.CurrentTarget == boss,
                                        ScriptHelpers.CreateTankFaceAwayGroupUnit(10))
                );
        }
        //good 48006 48296  // bad 48276 
        [EncounterHandler(47739, "\"Captain\" Cookie", Mode = CallBehaviorMode.CurrentBoss)]
        public Composite CaptainCookieFight()
        {
            WoWUnit bestTarget = null;

            bool IsMounted = ObjectManager.GetObjectsOfType<WoWPlayer>().Any(x => x.Distance2DSqr < 30*30 && x.IsAlive && !x.IsGhost && x.HealthPercent > 99 && !StyxWoW.Me.Mounted);

            WoWUnit allFood = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(x => x.Entry == 48006 || x.Entry == 48296 || x.Entry == 48276 || x.Entry == 48302 || x.Entry == 48299 || x.Name.Contains("Rotten"));
            WoWUnit badFood = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(x => x.Name.Contains("Rotten"));

            var deckCenter = new WoWPoint(-78.43713, -819.5272, 39.89661);
            WoWUnit boss = null;

            return new PrioritySelector(ctx => boss = ctx as WoWUnit,
                // Wait for boss to apear

                new Decorator(ctx => boss == null && Targeting.Instance.FirstUnit == null && !ProfileManager.CurrentProfile.BossEncounters.Any(b => !b.IsAlive && b.Entry == 47626),
                                new ActionAlwaysSucceed()),

                new Decorator(ctx => boss == null && Targeting.Instance.FirstUnit == null && StyxWoW.Me.IsTank(),
                                new Action(ret => Navigator.MoveTo(deckCenter))),

                new Decorator(
                    ctx => boss == null && StyxWoW.Me.Location.DistanceSqr(deckCenter) < 30 * 30 && Targeting.Instance.FirstUnit == null,
                    new Sequence(
                        new WaitContinue(20, ctx => Targeting.Instance.FirstUnit != null,
                                            new Action(ret => Navigator.MoveTo(deckCenter))),
                        new DecoratorContinue(
                            ctx =>
                            Targeting.Instance.FirstUnit == null &&
                            !ObjectManager.GetObjectsOfType<WoWUnit>().Any(u => u.Entry == 47739),
                            new Action(ctx => BossManager.CurrentBoss.MarkAsDead()))
                        )),
                                new Decorator(ret => ScriptHelpers.Tank == StyxWoW.Me && badFood != null,
                                    new PrioritySelector(
                                         new Decorator(ret =>  allFood != null && allFood.IsWithinMeleeRange,
                                                 new Action(ret => allFood.Interact())),
                                         new Decorator(ret => allFood != null && !allFood.IsWithinMeleeRange,
                                                 new Action(ret => Navigator.MoveTo(allFood.Location)))
                                             
                                             )),


                                new Decorator(ret => ScriptHelpers.Tank != StyxWoW.Me && badFood != null,
                                             ScriptHelpers.CreateRunAwayFromBad(ctx => badFood != null, 5f, u => u.Entry == 43778 && u.Distance <= 5)
                                             ),

                                    new Decorator(ctx => boss != null,
                                        new PrioritySelector())
                                    );
        }


        #endregion
    }
}
