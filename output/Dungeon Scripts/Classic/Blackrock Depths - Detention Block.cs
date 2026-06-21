using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommonBehaviors.Actions;
using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Inventory.Frames.Merchant;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;

#if USE_DUNGEONBUDDY_DLL
using Bots.DungeonBuddyDll;
using Bots.DungeonBuddyDll.Attributes;
using Bots.DungeonBuddyDll.Helpers;
namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Classic
#else
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
namespace Bots.DungeonBuddy.Dungeon_Scripts.Classic
#endif
{
    public class BlackrockDepthsDetentionBlock : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 30; } }

        public override WoWPoint Entrance { get { return new WoWPoint(-7178.79, -925.1274, 166.8448); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(457.0491, 38.14, -68.74); } }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var entry in units)
            {
                var unit = entry.Object;
                switch (unit.Entry)
                {
                    case 8894: // Anvilrage Medic
                        entry.Score += 50;
                        break;

                    case 8929: // Princess Moira Bronzebeard 
                        entry.Score = 0;
                        break;
                }
            }
        }

        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            units.RemoveAll(
                u =>
                    {
                        WoWUnit unit = u.ToUnit(); // Princess Moira Bronzebeard 
                        if (unit.Entry == 8929)
                            return true;
                        if (unit.Entry == 8901 && !unit.Combat && unit.DistanceSqr > 20*20) // Anvilrage Reservist
                            return true;
                        return false;
                    });
        }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            foreach (WoWUnit unit in incomingunits)
            {
                // Add Shadowforge Flame Keeper if we need to farm tourches.
                if (unit.Entry == 9956 && ShadowForgeTorgeLooted.IsFinished && (StyxWoW.Me.Role & WoWPartyMember.GroupRole.Tank) > 0 && (!_northBrazierLighted || !_southBrazierLighted))
                {
                    outgoingunits.Add(unit);
                }
            }
        }

        public override void OnEnter()
        {
            Lua.Events.AttachEvent("CHAT_MSG_LOOT", ChatMsgLoot);
        }

        public override void OnExit()
        {
            Lua.Events.DetachEvent("CHAT_MSG_LOOT", ChatMsgLoot);
            _northBrazierLighted = _lootedChestOfTheSeven = _talkedToDoomrelt = _southBrazierLighted = false;
        }

        #endregion

        private static readonly Regex ItemIdRegex = new Regex(@"Hitem:(?<id>\d+)", RegexOptions.CultureInvariant);

        private static void ChatMsgLoot(object sender, LuaEventArgs args)
        {
            var match = ItemIdRegex.Match(args.Args[0].ToString());
            if (match.Success && match.Groups["id"].Success)
            {
                var itemId = int.Parse(match.Groups["id"].Value);
                if (itemId == 11885) // Shadowforge Torch
                {
                    Logger.Write("A party member looted Shadowforge torch");
                    ShadowForgeTorgeLooted.Reset();
                }
            }
        }

        private readonly WoWPoint _ringOfLawCenterLocation = new WoWPoint(595.9567, -188.4252, -54.14674);
        private bool _talkedToDoomrelt;

        [EncounterHandler(9018, "High Interrogator Gerstahn")]
        public Composite HighInterrogatorGerstahnEncounter() { return ScriptHelpers.CreateTankUnitAtLocation(() => new WoWPoint(310.2939, -147.5773, -70.38612), 7); }

        [ObjectHandler(161524, "Ring of the Law", 1000)]
        public Composite RingOfTheLawEncounter()
        {
            WoWGameObject eastGarrisonDoor = null;
            WoWGameObject wave1Door = null;
            WoWGameObject wave2Door = null;
            bool wave1Inc = false;
            bool wave2Inc = false;
            // WoWDoor.IsClosed doesn't seem to work for these doors so using WowGameObject.State. Active state means it's open, Ready state means it's closed.
            return new PrioritySelector(
                ctx =>
                    {
                        wave1Door = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(o => o.Entry == 161525);
                        wave2Door = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(o => o.Entry == 161522);
                        eastGarrisonDoor = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(o => o.Entry == 161524);
                        wave1Inc = wave1Door != null && wave1Door.State == WoWGameObjectState.Active && eastGarrisonDoor != null && eastGarrisonDoor.State == WoWGameObjectState.Ready;
                        wave2Inc = wave2Door != null && wave2Door.State == WoWGameObjectState.Active && eastGarrisonDoor != null && eastGarrisonDoor.State == WoWGameObjectState.Ready;
                        return ctx;
                    },
                new Decorator(
                    ctx =>
                    eastGarrisonDoor != null && eastGarrisonDoor.State == WoWGameObjectState.Ready &&
                    (!ScriptHelpers.IsBossAlive("Houndmaster Grebmar") || !ScriptHelpers.ShouldKillBoss("Houndmaster Grebmar")) &&
                    StyxWoW.Me == ScriptHelpers.Tank,
                    new PrioritySelector(
                        new Decorator(
                            ctx => !wave1Inc && !wave2Inc,
                            new PrioritySelector(
                                new ActionSetActivity("Moving to Ring of Law"),
                                new Decorator(
                                    ctx => StyxWoW.Me.Location.DistanceSqr(_ringOfLawCenterLocation) > 5*5,
                                    new Action(ctx => ScriptHelpers.MoveTankTo(_ringOfLawCenterLocation))),
                                new Decorator(
                                    ctx => StyxWoW.Me.Location.DistanceSqr(_ringOfLawCenterLocation) <= 5*5 && Targeting.Instance.FirstUnit == null,
                                    new ActionAlwaysSucceed()))), // dont move anywhere
                        new Decorator(
                            ctx => wave2Inc && Targeting.Instance.FirstUnit == null,
                            new PrioritySelector(
                                new ActionSetActivity("Picking up boss."),
                                new Decorator(
                                    ctx => StyxWoW.Me.Location.DistanceSqr(wave2Door.Location) > 5*5,
                                    new Action(ctx => ScriptHelpers.MoveTankTo(wave2Door.Location))),
                                new Decorator(
                                    ctx => StyxWoW.Me.Location.DistanceSqr(wave2Door.Location) <= 5*5 && Targeting.Instance.FirstUnit == null,
                                    new ActionAlwaysSucceed()))), // dont move anywhere
                        new Decorator(
                            ctx => wave1Inc && !wave2Inc && Targeting.Instance.FirstUnit == null,
                            new PrioritySelector(
                                new ActionSetActivity("Picking up the waves of NPCs."),
                                new Decorator(
                                    ctx => StyxWoW.Me.Location.DistanceSqr(wave1Door.Location) > 5*5,
                                    new Action(ctx => ScriptHelpers.MoveTankTo(wave1Door.Location))),
                                new Decorator(
                                    ctx => StyxWoW.Me.Location.DistanceSqr(wave1Door.Location) <= 5*5 && Targeting.Instance.FirstUnit == null,
                                    new ActionAlwaysSucceed()))) // dont move anywhere
                        ))
                );
        }

        readonly WoWPoint _baelGarLocation = new WoWPoint(701.8241, 185.5464, -72.06669);
        [EncounterHandler(9016, "Bael'Gar", BossRange = 100, Mode = CallBehaviorMode.Proximity)]
        public Composite BaelGarEncounter()
        {
            // clear room of argo.
            WoWUnit boss = null;
            return new PrioritySelector(ctx => boss = ctx as WoWUnit,
                ScriptHelpers.CreateClearArea(() =>_baelGarLocation, 100, u => u.Entry != boss.Entry));
        }

        private readonly WoWPoint _lordIncendiusTankSpot = new WoWPoint(861.7827, -241.1472, -71.76051);

        [EncounterHandler(9017, "Lord Incendius")]
        public Composite LordIncendiusEncounter()
        {
            return new PrioritySelector(
                new Decorator(
                    ctx => StyxWoW.Me.IsTank(),
                    ScriptHelpers.CreateTankUnitAtLocation(() =>_lordIncendiusTankSpot, 7)),
                new Decorator(
                    ctx => StyxWoW.Me.IsDps(), // wait until boss is over in tanking spot before opening up on him.
                    new PrioritySelector(
                        new Decorator(
                            ctx => ((WoWUnit) ctx).Location.DistanceSqr(_lordIncendiusTankSpot) > 12*12,
                            new ActionAlwaysSucceed())
                        ))
                );
        }


        [ObjectHandler(161460, "The Shadowforge Lock", ObjectRange = 60)]
        public Composite TheShadowforgeLockHandler()
        {
            WoWGameObject shadowForgeLock = null;
            return new PrioritySelector(
                ctx => shadowForgeLock = ctx as WoWGameObject,
                new Decorator(
                    ctx => shadowForgeLock.State == WoWGameObjectState.Ready && (!ScriptHelpers.IsBossAlive("Lord Incendius") || !ScriptHelpers.ShouldKillBoss("Lord Incendius")),
                    new PrioritySelector(
                        new Decorator(
                            ctx => shadowForgeLock.DistanceSqr > 5*5 && StyxWoW.Me == ScriptHelpers.Tank,
                            new Action(ctx => ScriptHelpers.MoveTankTo(shadowForgeLock.Location))),
                        new Decorator(
                            ctx => shadowForgeLock.DistanceSqr <= 5*5,
                            ScriptHelpers.CreateInteractWithObject(161460))))
                );
        }

        [ObjectHandler(164911, "Thunderbrew Lager Keg", 75)]
        public Composite HurleyBlackbreathEncounter()
        {
            WoWGameObject keg = null;
            return new PrioritySelector(
                ctx => keg = ctx as WoWGameObject,
                new Decorator(
                    ctx => keg != null && ScriptHelpers.ShouldKillBoss("Hurley Blackbreath") && ScriptHelpers.IsBossAlive("Hurley Blackbreath"),
                    ScriptHelpers.CreateInteractWithObject(164911)));
        }

        private readonly WoWPoint _ribblyScrewspigotDpsSpot = new WoWPoint(893.4753, -157.8955, -49.76056);

        private readonly WoWPoint _ribblyScrewspigotTankSpot = new WoWPoint(896.8156, -163.6791, -49.76056);

        [EncounterHandler(9543, "Ribbly Screwspigot", Mode = CallBehaviorMode.Proximity)]
        public Composite RibblyScrewspigotEncounter()
        {
            return new PrioritySelector(
                new Decorator(
                    ctx =>
                    !ScriptHelpers.IsBossAlive("Hurley Blackbreath") &&
                    // loot up before we start event
                    !ObjectManager.GetObjectsOfType<WoWUnit>().Any(u => u.IsValid && u.Dead && u.Lootable && u.DistanceSqr < CharacterSettings.Instance.LootRadius * CharacterSettings.Instance.LootRadius),
                    new PrioritySelector(
                        // I am tank
                        new Decorator(
                            ctx => StyxWoW.Me.IsTank(),
                            new PrioritySelector(
                                new ActionSetActivity("here 3"),
                                new Decorator(
                                    ctx => !StyxWoW.Me.IsActuallyInCombat,
                                    new Sequence(
                                        ScriptHelpers.CreateTalkToNpcContinue(9543),
                                        ScriptHelpers.CreateMoveToContinue(() => _ribblyScrewspigotTankSpot),
                                        new WaitContinue(15, ctx => StyxWoW.Me.IsActuallyInCombat, new ActionAlwaysSucceed()))),
                                ScriptHelpers.CreateTankUnitAtLocation(() => _ribblyScrewspigotTankSpot, 4))),
                        // I am dps/healer.
                        new Decorator(
                            ctx => StyxWoW.Me.IsFollower(),
                            new PrioritySelector(
                                new Decorator(
                                    ctx => StyxWoW.Me.Location.DistanceSqr(_ribblyScrewspigotDpsSpot) > 4*4,
                                    new Action(ctx => Navigator.GetRunStatusFromMoveResult(Navigator.MoveTo(_ribblyScrewspigotDpsSpot)))),
                                new Decorator(
                                    ctx => ScriptHelpers.MovementEnabled && StyxWoW.Me.Location.DistanceSqr(_ribblyScrewspigotDpsSpot) <= 4*4,
                                    new Action(ctx => ScriptHelpers.DisableMovement(() => ScriptHelpers.IsBossAlive("Ribbly Screwspigot") && StyxWoW.Me.IsAlive)))
                                )))));
        }

        private readonly WoWPoint _openBarDoorSafeMoveTo = new WoWPoint(881.53, -226.6577, -46.51212);
        private readonly WoWPoint _openBarDoorWaitAtPoint = new WoWPoint(869.8575, -227.0011, -43.75132);

        [EncounterHandler(9499, "Plugger Spazzring", Mode = CallBehaviorMode.Proximity, BossRange = 100)]
        public Composite OpenBarDoorEncounter()
        {
            WoWGameObject barDoor = null;
            WoWUnit pluggerSpazzring = null;
            int mugsInBag = 0;
            return new PrioritySelector(
                ctx =>
                    {
                        barDoor = BarDoor;
                        pluggerSpazzring = ctx as WoWUnit;
                        return ctx;
                    },
                new Decorator(
                    ctx => barDoor != null && barDoor.State == WoWGameObjectState.Ready &&  !ScriptHelpers.IsBossAlive("Ribbly Screwspigot") &&
                           // loot up before we start event
                           !ObjectManager.GetObjectsOfType<WoWUnit>().Any(u => u.IsValid && u.Dead && u.Lootable && u.DistanceSqr < CharacterSettings.Instance.LootRadius*CharacterSettings.Instance.LootRadius),
                    new PrioritySelector(
                        ctx => mugsInBag = (int) StyxWoW.Me.CarriedItems.Sum(i => i != null && i.IsValid && i.Entry == 11325 ? i.StackCount : 0),
                        new Sequence(
                            // do we need to buy Dark Iron Ale Mug?
                            new DecoratorContinue(
                                ctx => StyxWoW.Me.IsTank() && mugsInBag < 6,
                                new Sequence(
                                    ScriptHelpers.CreateMoveToContinue(9499),
                                    new Action(ctx => pluggerSpazzring.Interact()),
                                    new WaitContinue(2, ctx => MerchantFrame.Instance.IsVisible, new ActionAlwaysSucceed()),
                                    new Action(ctx => ScriptHelpers.BuyItem(11325, 6 - mugsInBag)),
                                    new WaitContinue(4, ctx => StyxWoW.Me.BagItems.Any(i => i.Entry == 11325), new ActionAlwaysSucceed()),
                                    new Action(ctx => MerchantFrame.Instance.Close()),
                                    new WaitContinue(3,ctx => false, new ActionAlwaysSucceed()))),// wait some for bags to update.
                            new Action(ctx => Logger.Write("{0} in bag",mugsInBag)),
                            new DecoratorContinue(
                                ctx => StyxWoW.Me.IsTank() && mugsInBag >= 6,
                                new Sequence(
                                    ScriptHelpers.CreateTurninQuest(9503, 0), // turn in the quest 3 times.
                                    ScriptHelpers.CreateTurninQuest(9503, 0),
                                    ScriptHelpers.CreateTurninQuest(9503, 0)
                                    )),
                            // wait for the bar door to open. or Until I'm in combat
                            ScriptHelpers.CreateMoveToContinue(() => _openBarDoorWaitAtPoint),
                            new ActionSetActivity("Waiting for bar door to open"),
                            new WaitContinue(TimeSpan.FromMinutes(2), ctx => BarDoor.State == WoWGameObjectState.ActiveAlternative || StyxWoW.Me.Combat, new ActionAlwaysSucceed()),
                            // move through the door so we don't aggro.
                            ScriptHelpers.CreateMoveToContinue(() => _openBarDoorSafeMoveTo)
                            ))));
        }

        private WoWGameObject BarDoor { get { return ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(o => o.IsValid && o.Entry == 170571); } }

        [EncounterHandler(9039, "Doom'relt", Mode = CallBehaviorMode.Proximity, BossRange = 100)]
        public Composite TheSevenEncounter()
        {
            return new Decorator(
                ctx => StyxWoW.Me == ScriptHelpers.Tank,
                new PrioritySelector(
                    new Decorator(
                        ctx => !_talkedToDoomrelt,
                        new Sequence(
                            ScriptHelpers.CreateTalkToNpcContinue(9039),
                            new Action(ctx => _talkedToDoomrelt = true))),
                    new Decorator(
                        // don't do anything while waiting for combat... such as running off to look for next boss.
                        ctx => Targeting.Instance.FirstUnit == null,
                        new ActionAlwaysSucceed())));
        }


        private bool _lootedChestOfTheSeven;

        [ObjectHandler(169243, "Chest of The Seven")]
        public Composite ChestOfTheSevenHandler()
        {
            return new Decorator(
                ctx => !_lootedChestOfTheSeven,
                new Sequence(
                    ScriptHelpers.CreateMoveToContinue(169243),
                    new WaitContinue(3, ctx => !StyxWoW.Me.IsMoving, new ActionAlwaysSucceed()),
                    new Action(ctx => ((WoWGameObject) ctx).Interact()),
                    new WaitContinue(10, ctx => false, new ActionAlwaysSucceed()),
                    new Action(ctx => _lootedChestOfTheSeven = true)));
        }

        private bool _northBrazierLighted;
        private bool _southBrazierLighted;

        private static readonly WaitTimer ShadowForgeTorgeLooted = new WaitTimer(TimeSpan.FromMinutes(5));

        private readonly WoWPoint _northBrazierLocation = new WoWPoint(1431.913, -523.971, -92.03755);
        private readonly WoWPoint _southBrazierLocation = new WoWPoint(1332.868, -508.6617, -88.86429);

        [ObjectHandler(170574, "Golem Room South", ObjectRange = 120)] // door that opens if the braziers are lit
        public Composite LightTheBraziersHandler()
        {
            return new Decorator(
                ctx => !ScriptHelpers.IsBossAlive("Ambassador Flamelash") && ((WoWGameObject) ctx).State == WoWGameObjectState.Ready, // door is close
                new PrioritySelector(
                    ctx =>
                        {
                            WoWGameObject southBrazier = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(o => o.Entry == 174744);
                            WoWGameObject northBrazier = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(o => o.Entry == 174745);
                            if (southBrazier != null && southBrazier.State == WoWGameObjectState.Active)
                                _southBrazierLighted = true;
                            if (northBrazier != null && northBrazier.State == WoWGameObjectState.Active)
                                _northBrazierLighted = true;
                            return ctx;
                        },
                    new Decorator(
                        // Go light the north brazier
                        ctx => !_northBrazierLighted && Targeting.Instance.FirstUnit == null,
                        new PrioritySelector(
                            new Decorator(
                                ctx => StyxWoW.Me.Location.DistanceSqr(_northBrazierLocation) > 30*30,
                                new PrioritySelector(
                                    new ActionSetActivity("Moving to north brazier"),
                                    new Action(ctx => ScriptHelpers.MoveTankTo(_northBrazierLocation)))),
                            new Decorator(
                                ctx => StyxWoW.Me.Location.DistanceSqr(_northBrazierLocation) <= 30*30 && StyxWoW.Me.BagItems.Any(i => i.Entry == 11885 && Targeting.Instance.FirstUnit == null),
                                new PrioritySelector(
                                    ScriptHelpers.CreateInteractWithObject(174745, 6),
                                    new Decorator(
                                        ctx => Targeting.Instance.FirstUnit == null,
                                        new ActionAlwaysSucceed())))
                            )),
                    new Decorator(
                        // Go light the south brazier
                        ctx => _northBrazierLighted && !_southBrazierLighted && Targeting.Instance.FirstUnit == null,
                        new PrioritySelector(
                            new Decorator(
                                ctx => StyxWoW.Me.Location.DistanceSqr(_southBrazierLocation) > 30*30,
                                new PrioritySelector(
                                    new ActionSetActivity("Moving to south brazier"),
                                    new Action(ctx => ScriptHelpers.MoveTankTo(_southBrazierLocation)))),
                            new Decorator(
                                ctx => StyxWoW.Me.Location.DistanceSqr(_southBrazierLocation) <= 30*30 && StyxWoW.Me.BagItems.Any(i => i.Entry == 11885 && Targeting.Instance.FirstUnit == null),
                                new PrioritySelector(
                                    ScriptHelpers.CreateInteractWithObject(174744, 6),
                                    new Decorator(
                                        ctx => Targeting.Instance.FirstUnit == null,
                                        new ActionAlwaysSucceed())))
                            ))));
        }

        private readonly WoWPoint _magmusPartyMoveToLocation = new WoWPoint(1374.643, -655.4331, -92.05432);
        private readonly WoWPoint _magmusTankMoveToLocation = new WoWPoint(1386.977, -655.6155, -92.05432);

        [EncounterHandler(9938, "Magmus")]
        public Composite MagmusEncounter()
        {
            return new PrioritySelector(
                // move tank in place.
                new Decorator(
                    ctx => StyxWoW.Me == ScriptHelpers.Tank,
                    ScriptHelpers.CreateTankUnitAtLocation(() => _magmusTankMoveToLocation, 5)),
                // Move party into place 
                new Decorator(
                    ctx => StyxWoW.Me != ScriptHelpers.Tank,
                    new PrioritySelector(
                        new Decorator(
                            ctx => StyxWoW.Me.Location.DistanceSqr(_magmusPartyMoveToLocation) > 5*5,
                            new Action(ctx => Navigator.GetRunStatusFromMoveResult(Navigator.MoveTo(_magmusPartyMoveToLocation)))))));
        }
    }
}