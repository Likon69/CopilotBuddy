using System;
using System.Collections.Generic;
using System.Linq;
using Styx;
using TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using HighVoltz.Composites;
using Levelbot.Decorators.Death;
using Levelbot.Actions.Death;
using Styx.Logic.Combat;
using Action = TreeSharp.Action;
using Styx.Helpers;
using Styx.Logic.Profiles;
using Styx.Logic.POI;
using System.IO;

namespace HighVoltz
{
    public enum PathingType { Circle, Bounce }
    public class AutoAngler : BotBase
    {
        public static Dictionary<string, uint> FishCaught { get; private set; }
        public static readonly List<uint> PoolsToFish = new List<uint>();
        static DateTime _botStartTime = new DateTime();
        LocalPlayer me = ObjectManager.Me;
        public readonly Version Version = new Version(2, new Svn().Revision);
        public static AutoAngler Instance { get; private set; }
        public AutoAngler()
        {
            Instance = this;
        }

        AutoAnglerSettings _settings;
        public AutoAnglerSettings MySettings
        {
            get
            {
                return _settings ?? (_settings = new AutoAnglerSettings(
                    string.Format("{0}\\Settings\\AutoAngler\\AutoAngler-{1}",
                    Logging.ApplicationPath, me.Name)));
            }
        }

        #region overrides
        public override string Name { get { return "AutoAngler"; } }

        public override PulseFlags PulseFlags
        {
            get { return Styx.PulseFlags.All; }
        }

        PrioritySelector _root;
        public override TreeSharp.Composite Root
        {
            get
            {
                return _root ?? (_root = new PrioritySelector(
                    // Is bot dead? if so, release and run back to corpse
                    new Decorator(c => !me.IsAlive,
                      new PrioritySelector(
                          new DecoratorNeedToRelease(new ActionReleaseFromCorpse()),
                          new DecoratorNeedToMoveToCorpse(new ActionMoveToCorpse()),
                          new DecoratorNeedToTakeCorpse(new ActionRetrieveCorpse()),
                          new ActionSuceedIfDeadOrGhost()
                       )),
                    // If bot is in combat call the CC routine
                    new Decorator(c => me.IsActuallyInCombat && !me.IsFlying,
                        new CombatAction()),
                    // If bot needs to rest then call the CC rest behavior
                    new Decorator(c => RoutineManager.Current.NeedRest && !ObjectManager.Me.IsCasting && !me.IsFlying,
                        new PrioritySelector(
                    // if Rest Behavior exists use it..
                            new Decorator(c => RoutineManager.Current.RestBehavior != null,
                                new PrioritySelector(
                                    new Action(c =>
                                    {
                                        // reset the autoBlacklist timer since we're stoping to rest.
                                        if (BotPoi.Current != null && BotPoi.Current.Type == PoiType.Harvest)
                                        {
                                            MoveToPoolAction.MoveToPoolSW.Reset();
                                            MoveToPoolAction.MoveToPoolSW.Start();
                                        }
                                        return RunStatus.Failure;
                                    }),
                                    RoutineManager.Current.RestBehavior
                                    )),
                    // else call legacy Rest() method
                                new Action(c =>
                                {
                                    // reset the autoBlacklist timer since we're stoping to rest.
                                    if (BotPoi.Current != null && BotPoi.Current.Type == PoiType.Harvest)
                                    {
                                        MoveToPoolAction.MoveToPoolSW.Reset();
                                        MoveToPoolAction.MoveToPoolSW.Start();
                                    }
                                    RoutineManager.Current.Rest();
                                })
                            )),
                    // mail and repair if bags are full or items have low durability. logout if profile doesn't have mailbox and vendor.
                    new Decorator(c => me.BagsFull || me.DurabilityPercent <= 0.2 &&
                        (BotPoi.Current == null || BotPoi.Current.Type != PoiType.Mail ||
                        BotPoi.Current.Type != PoiType.Repair || BotPoi.Current.Type != PoiType.InnKeeper),
                        new Action(c =>
                        {
                            if (ProfileManager.CurrentOuterProfile != null &&
                                ProfileManager.CurrentOuterProfile.MailboxManager != null)
                            {
                                Mailbox mbox = ProfileManager.CurrentOuterProfile.MailboxManager.GetClosestMailbox();
                                if (mbox != null && !string.IsNullOrEmpty(CharacterSettings.Instance.MailRecipient))
                                    BotPoi.Current = new BotPoi(mbox);
                                else
                                {
                                    Vendor ven = ProfileManager.CurrentOuterProfile.VendorManager.GetClosestVendor();
                                    if (ven != null)
                                        BotPoi.Current = new BotPoi(ven, PoiType.Repair);
                                    else
                                        // we'll use this POI to hearth+Logout...
                                        BotPoi.Current = new BotPoi(PoiType.InnKeeper);
                                }
                            }
                            return RunStatus.Failure;
                        })),
                    new Decorator(c => BotPoi.Current != null && BotPoi.Current.Type == PoiType.Mail,
                        new MailAction()),
                    new Decorator(c => BotPoi.Current != null && BotPoi.Current.Type == PoiType.Repair,
                        new VendorAction()),
                    new Decorator(c => BotPoi.Current != null && BotPoi.Current.Type == PoiType.InnKeeper,
                        new LogoutAction()),
                    // loot Any dead lootable NPCs if setting is enabled.
                    new Decorator (c => AutoAnglerSettings.Instance.LootNPCs,
                        new LootNPCsAction()),
                    new AutoAnglerDecorator(
                        new PrioritySelector(
                            new HarvestPoolDecorator(
                                new PrioritySelector(
                                    new LootAction(),
                                    new WaterWalkingAction(),
                                    new MoveToPoolAction(),
                                    new EquipPoleAction(),
                                    new ApplyLureAction(),
                                    new FishAction()
                                ))
                        )),
                    // follow the path...
                    new FollowPathAction()

                ));
            }
        }

        public override bool IsPrimaryType
        {
            get
            {
                return true;
            }
        }
        public override void Initialize()
        {
            try
            {
                BotEvents.OnBotStart += BotEvents_OnBotStart;
                BotEvents.OnBotStop += BotEvents_OnBotStop;
                WoWItem mainhand = MySettings.MainHand != 0 ? Util1.GetIteminBag(MySettings.MainHand) : null;
                if (mainhand == null)
                    mainhand = FindMainHand();
                WoWItem offhand = MySettings.OffHand != 0 ? Util1.GetIteminBag(MySettings.OffHand) : null;
                if (((mainhand != null && mainhand.ItemInfo.InventoryType != InventoryType.TwoHandWeapon) ||
                    mainhand == null) && offhand == null)
                {
                    offhand = FindOffhand();
                }
                if (mainhand != null)
                    Log("Using {0} for mainhand weapon", mainhand.Name);
                if (offhand != null)
                    Log("Using {0} for offhand weapon", offhand.Name);
                if (!MySettings.Poolfishing)
                {
                    if (!string.IsNullOrEmpty(ProfileManager.XmlLocation))
                    {
                        MySettings.LastLoadedProfile = ProfileManager.XmlLocation;
                        MySettings.Save();
                    }
                    ProfileManager.LoadEmpty();
                }
                else if (ProfileManager.CurrentProfile == null && !string.IsNullOrEmpty(MySettings.LastLoadedProfile) && File.Exists(MySettings.LastLoadedProfile))
                    ProfileManager.LoadNew(MySettings.LastLoadedProfile);
            }
            catch (Exception ex) { Logging.WriteException(ex); }
        }

        public override System.Windows.Forms.Form ConfigurationForm
        {
            get
            {
                return new MainForm();
            }
        }
        #endregion

        void BotEvents_OnBotStart(EventArgs args)
        {
            _botStartTime = DateTime.Now;
            FishCaught = new Dictionary<string, uint>();
        }

        void BotEvents_OnBotStop(EventArgs args)
        {
            Log("In {0} days, {1} hours and {2} minutes we have caught",
                (DateTime.Now - _botStartTime).Days,
                (DateTime.Now - _botStartTime).Hours,
                (DateTime.Now - _botStartTime).Minutes);
            foreach (var kv in FishCaught)
            {
                Log("{0} x{1}", kv.Key, kv.Value);
            }
        }

        WoWItem FindMainHand()
        {
            bool is2Hand = false;
            var mainHand = me.Inventory.Equipped.MainHand;
            if (mainHand == null || mainHand.ItemInfo.WeaponClass == WoWItemWeaponClass.FishingPole)
            {
                mainHand = me.CarriedItems.OrderByDescending(u => u.ItemInfo.Level).
                    FirstOrDefault(i => i.IsSoulbound && (i.ItemInfo.InventoryType == InventoryType.WeaponMainHand ||
                        i.ItemInfo.InventoryType == InventoryType.TwoHandWeapon) && me.CanEquipItem(i));
                if (mainHand != null)
                {
                    MySettings.MainHand = mainHand.Entry;
                    is2Hand = mainHand.ItemInfo.InventoryType == InventoryType.TwoHandWeapon;
                }
                else
                    Err("Unable to find a mainhand weapon to swap to when in combat");
            }
            else
                MySettings.MainHand = mainHand.Entry;
            MySettings.Save();
            return mainHand;
        }

        // scans bags for offhand weapon if mainhand isn't 2h and none are equipped and uses the highest ilvl one
        WoWItem FindOffhand()
        {
            var offHand = me.Inventory.Equipped.OffHand;
            if (offHand == null)
            {
                offHand = me.CarriedItems.OrderByDescending(u => u.ItemInfo.Level).
                    FirstOrDefault(i => i.IsSoulbound && (i.ItemInfo.InventoryType == InventoryType.WeaponOffHand ||
                        i.ItemInfo.InventoryType == InventoryType.Weapon ||
                        i.ItemInfo.InventoryType == InventoryType.Shield) && MySettings.MainHand != i.Entry &&
                        me.CanEquipItem(i));
                if (offHand != null)
                {
                    MySettings.OffHand = offHand.Entry;
                }
                else
                    Err("Unable to find an offhand weapon to swap to when in combat");
            }
            else
                MySettings.OffHand = offHand.Entry;
            MySettings.Save();
            return offHand;
        }

        public void Log(string format, params object[] args)
        {
            Logging.Write(System.Drawing.Color.DodgerBlue, string.Format("AutoAngler[{0}]: {1}", Version, format), args);
        }

        public void Err(string format, params object[] args)
        {
            Logging.Write(System.Drawing.Color.Red, string.Format("AutoAngler[{0}]: {1}", Version, format), args);
        }

        public void Debug(string format, params object[] args)
        {
            Logging.WriteDebug(System.Drawing.Color.DodgerBlue, string.Format("AutoAngler[{0}]: {1}", Version, format), args);
        }
    }
}
