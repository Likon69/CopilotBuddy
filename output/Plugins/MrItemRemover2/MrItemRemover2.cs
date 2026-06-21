/*
 * Mr.ItemRemover2 - Created by CodenameGamma - 1-31-11 - For WoW Version 4.0.3
 * 2.4 Update by Bambam922
 * www.thebuddyforum.com
 * This is a free plugin and should not be sold or repackaged.
 * Donations accepted.
 * Version 2.4 for WoW Version 6.0.3 +
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;
using Styx;
using Styx.Common;
using Styx.Helpers;
using Styx.Plugins;
using Styx.Plugins.PluginClass;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace MrItemRemover2
{
    public partial class MrItemRemover2 : HBPlugin
    {
        // ReSharper disable InconsistentNaming
        private const string _name = "MIR2";
        private const string _debug = "MIR2 DEBUG";

        private readonly WaitTimer _checkTimer =
        new WaitTimer(TimeSpan.FromMinutes(MrItemRemover2Settings.Instance.Time));

        private readonly string _bagListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            string.Format(@"Plugins/MrItemRemover2/Lists/ItemNameBagList.txt"));

        private readonly string _combineList10Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            string.Format(@"Plugins/MrItemRemover2/Lists/ItemNameCombineList10.txt"));

        private readonly string _combineList3Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            string.Format(@"Plugins/MrItemRemover2/Lists/ItemNameCombineList3.txt"));

        private readonly string _combineList5Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            string.Format(@"Plugins/MrItemRemover2/Lists/ItemNameCombineList5.txt"));

        private readonly string _drinkListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            string.Format(@"Plugins/MrItemRemover2/Lists/ItemNameDrinkList.txt"));

        private readonly string _foodListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            string.Format(@"Plugins/MrItemRemover2/Lists/ItemNameFoodList.txt"));

        private readonly string _keepListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            string.Format(@"Plugins/MrItemRemover2/Lists/ItemNameKeepList.txt"));

        private readonly string _opnListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            string.Format(@"Plugins/MrItemRemover2/Lists/ItemNameOpnList.txt"));

        private readonly string _removeListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            string.Format(@"Plugins/MrItemRemover2/Lists/ItemNameRemoveList.txt"));

        private readonly string _sellListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            string.Format(@"Plugins/MrItemRemover2/Lists/ItemNameSellList.txt"));

        public List<string> BagList = new List<string>();
        public List<string> Combine10List = new List<string>();
        public List<string> Combine3List = new List<string>();
        public List<string> Combine5List = new List<string>();
        public List<string> DrinkList = new List<string>();
        public List<string> FoodList = new List<string>();
        public List<string> InventoryList = new List<string>();
        public List<string> ItemName = new List<string>();
        //Specific items from the TXT Doc are loaded here.
        public List<string> ItemNameSell = new List<string>();
        public List<string> KeepList = new List<string>();
        public List<string> OpnList = new List<string>();

        // ReSharper restore InconsistentNaming

        //Normal Stuff.
        public override string Name
        {
            get { return _name; }
        }

        public override string Author
        {
            get { return "CnG & Bambam922"; }
        }

        public override Version Version
        {
            get { return new Version(2, 4); }
        }

        public override bool WantButton
        {
            get { return true; }
        }

        public override string ButtonText
        {
            get { return "MIR2 Settings"; }
        }

// ReSharper disable UnusedAutoPropertyAccessor.Local
        public MrItemRemover2 Controller { get; private set; }
// ReSharper restore UnusedAutoPropertyAccessor.Local

        public bool ManualCheckRequested { get; set; }
        private bool EnableCheck { get; set; }
        private bool IsInitialized { get; set; }

        private static LocalPlayer Me
        {
            get { return StyxWoW.Me; }
        }

        public override void OnButtonPress()
        {
            if (!IsInitialized)
            {
                Slog("Not finished initializing");
                return;
            }
            var form = new MrItemRemover2Gui(this);
            form.ShowDialog();
        }

        public static void Slog(string format, params object[] args)
        {
            Slog(Colors.SkyBlue, format, args);
        }

        public static void Slog(Color color, string format, params object[] args)
        {
            Logging.Write(color, "[" + _name + "]: " + format, args);
        }

        public static void Dlog(string format, params object[] args)
        {
            Dlog(Colors.Crimson, format, args);
        }

        public static void Dlog(Color color, string format, params object[] args)
        {
            Logging.WriteDiagnostic(color, "[" + _debug + "]: " + format, args);
        }

        public override void OnEnable()
        {
            Lua.Events.AttachEvent("DELETE_ITEM_CONFIRM", DeleteItemConfirmPopup);
            Lua.Events.AttachEvent("MERCHANT_SHOW", SellVenderItems);
            Lua.Events.AttachEvent("LOOT_CLOSED", LootEnded);
            Lua.DoString("SetCVar('AutoLootDefault','1')");

            InitialMirLoad();
            MrItemRemover2Settings.Instance.Load();
            PrintSettings();

            _checkTimer.Reset(); //should start the timer 

            IsInitialized = true;
        }

        public override void OnDisable()
        {
            Lua.Events.DetachEvent("DELETE_ITEM_CONFIRM", DeleteItemConfirmPopup);
            Lua.Events.DetachEvent("MERCHANT_SHOW", SellVenderItems);
            Lua.Events.DetachEvent("LOOT_CLOSED", LootEnded);

            IsInitialized = false;
            MirSave();

            Dlog("MIR is now disabled.");
        }

        public override void Pulse()
        {
            if (ManualCheckRequested)
            {
                EnableCheck = true;
                ManualCheckRequested = false;
                _checkTimer.Reset();

                Slog("Checking Bags Manually.");
                CheckForItems(); 
            }

            else if (MrItemRemover2Settings.Instance.LootCheck == "False")
            {
                if (_checkTimer.TimeLeft.Ticks <= 0)
                {
                    if (EnableCheck == false)
                    {
                        if (!Me.Mounted)
                        {
                            EnableCheck = true;
                            CheckForItems();
                            _checkTimer.Reset();

                            Slog("Enabling Check at {0}", GetTime(DateTime.Now));
                            Dlog(
                                "Checktimer has Finished its Total wait of {0} Minutes. Checking Items and Enabling Item Check for next Opportunity",
                                MrItemRemover2Settings.Instance.Time.ToString(CultureInfo.InvariantCulture));
                            Slog("Will Run Next Check At {0}", GetTime(_checkTimer.EndTime));
                        }
                    }
                }
            }

            if (!Me.Combat && !Me.IsCasting && !Me.IsDead && !Me.IsGhost && EnableCheck)
            {
                Slog("EnableCheck was Passed!");
                if (MrItemRemover2Settings.Instance.EnableRemove == "True")
                {
                    CheckForItems();
                }
                EnableCheck = false;
                Slog("Turning off Check Since Done!");
            }
        }

        private void LootEnded(object sender, LuaEventArgs args)
        {
            if (MrItemRemover2Settings.Instance.LootCheck == "True")
            {
                if (EnableCheck == false)
                {
                    EnableCheck = true;
                    Slog("Enabling Check because Loot Ended");
                }
            }
        }

        public void InitialMirLoad()
        {
            Slog("Initial Loading of Individual Item Lists.");
            LoadList(ItemName, _removeListPath);
            LoadList(ItemNameSell, _sellListPath);
            LoadList(KeepList, _keepListPath);
            LoadList(OpnList, _opnListPath);
            LoadList(BagList, _bagListPath);
            LoadList(Combine3List, _combineList3Path);
            LoadList(Combine5List, _combineList5Path);
            LoadList(Combine10List, _combineList10Path);
            LoadList(FoodList, _foodListPath);
            LoadList(DrinkList, _drinkListPath);
            Slog("Initial Loading Complete!");
        }

        public void LoadList(List<string> listToLoad, string filePath)
        {
            listToLoad.Clear();
            try
            {
                using (var read = new StreamReader(Convert.ToString(filePath)))
                {
                    while (read.Peek() >= 0)
                    {
                        listToLoad.Add(Convert.ToString(read.ReadLine()));
                    }
                    read.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex.Message));
            }
        }

        public void MirSave()
        {
            if (ItemName.Count != 0)
            {
                PrintSettings();
                Slog("Saving All Lists.");
                WriteList(ItemName, _removeListPath);
                WriteList(ItemNameSell, _sellListPath);
                WriteList(KeepList, _keepListPath);
                WriteList(OpnList, _opnListPath);
            }
        }

        public void WriteList(List<string> listName, string filePath)
        {
            if (ItemName.Count != 0)
            {
                try
                {
                    var write = new StreamWriter(filePath);
                    // ReSharper disable ForCanBeConvertedToForeach
                    for (int I = 0; I < listName.Count; I++)
                        // ReSharper restore ForCanBeConvertedToForeach
                    {
                        write.WriteLine(Convert.ToString(listName[I]));
                    }
                    write.Close(); //dakkon for this fix.
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Convert.ToString(ex.Message));
                }
            }
        }
    }
}