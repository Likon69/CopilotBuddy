using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Styx;
using Styx.WoWInternals.WoWObjects;

namespace MrItemRemover2
{
    public partial class MrItemRemover2Gui : Form
    {
        private readonly string _goldImangePathName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            string.Format(@"Plugins/MrItemRemover2/Gold2.bmp"));

        private readonly string _refreshImangePathName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            string.Format(@"Plugins/MrItemRemover2/ref.bmp"));

        public MrItemRemover2Gui(MrItemRemover2 controller)
        {
            Controller = controller;
            InitializeComponent();
        }

        public MrItemRemover2 Controller { get; private set; }

        public static void Slog(string format, params object[] args)
        {
            MrItemRemover2.Slog(format, args);
        }

        public static void Dlog(string format, params object[] args)
        {
            MrItemRemover2.Dlog(format, args);
        }

        private void MrItemRemover2GUI_Load(object sender, EventArgs e)
        {
            var refresh = new Bitmap(_refreshImangePathName);
            var goldImg = new Bitmap(_goldImangePathName);
            GoldBox.Image = goldImg;
            resf.Image = refresh;
            MrItemRemover2Settings.Instance.Load();
            SellDropDown.SelectedItem = MrItemRemover2Settings.Instance.EnableSell;
            RemoveDropDown.SelectedItem = MrItemRemover2Settings.Instance.EnableRemove;
            OpeningDropDown.SelectedItem = MrItemRemover2Settings.Instance.EnableOpen;
            SellGraysDropDown.SelectedItem = MrItemRemover2Settings.Instance.SellGray;
            SellWhitesDropDown.SelectedItem = MrItemRemover2Settings.Instance.SellWhite;
            SellGreensDropDown.SelectedItem = MrItemRemover2Settings.Instance.SellGreen;
            SellBluesDropDown.SelectedItem = MrItemRemover2Settings.Instance.SellBlue;
            SellFoodDropDown.SelectedItem = MrItemRemover2Settings.Instance.SellFood;
            SellDrinksDropDown.SelectedItem = MrItemRemover2Settings.Instance.SellDrinks;
            RemoveGraysDropDown.SelectedItem = MrItemRemover2Settings.Instance.DeleteAllGray;
            RemoveWhitesDropDown.SelectedItem = MrItemRemover2Settings.Instance.DeleteAllWhite;
            RemoveGreensDropDown.SelectedItem = MrItemRemover2Settings.Instance.DeleteAllGreen;
            RemoveBluesDropDown.SelectedItem = MrItemRemover2Settings.Instance.DeleteAllBlue;
            RemoveQuestStartersDropDown.SelectedItem = MrItemRemover2Settings.Instance.DeleteQuestItems;
            RemoveFoodDropDown.SelectedItem = MrItemRemover2Settings.Instance.RemoveFood;
            RemoveDrinksDropDown.SelectedItem = MrItemRemover2Settings.Instance.RemoveDrinks;
            SellSoulboundDropDown.SelectedItem = MrItemRemover2Settings.Instance.SellSoulbound;
            CheckAfterLootDropDown.SelectedItem = MrItemRemover2Settings.Instance.LootCheck;
            GoldGrays.Text = MrItemRemover2Settings.Instance.GoldGrays.ToString(CultureInfo.InvariantCulture);
            SilverGrays.Text = MrItemRemover2Settings.Instance.SilverGrays.ToString(CultureInfo.InvariantCulture);
            CopperGrays.Text = MrItemRemover2Settings.Instance.CopperGrays.ToString(CultureInfo.InvariantCulture);
            Time.Value = MrItemRemover2Settings.Instance.Time;
            ReqRefLvl.Value = MrItemRemover2Settings.Instance.ReqRefLvl;

            foreach (string itm in Controller.ItemName)
            {
                RemoveList.Items.Add(itm);
            }
            foreach (string itm in Controller.ItemNameSell)
            {
                SellList.Items.Add(itm);
            }
            foreach (string itm in Controller.KeepList)
            {
                ProtectedList.Items.Add(itm);
            }
            foreach (string itm in Controller.Combine3List)
            {
                Combine3List.Items.Add(itm);
            }
            foreach (string itm in Controller.Combine5List)
            {
                Combine5List.Items.Add(itm);
            }
            foreach (string itm in Controller.Combine10List)
            {
                Combine10List.Items.Add(itm);
            }
            foreach (string itm in Controller.FoodList)
            {
                FoodList.Items.Add(itm);
            }
            foreach (string itm in Controller.DrinkList)
            {
                DrinkList.Items.Add(itm);
            }

            foreach (WoWItem bagItem in StyxWoW.Me.BagItems)
            {
                if (bagItem.IsValid && bagItem.BagSlot != -1 && !MyBag.Items.Contains(bagItem.Name))
                {
                    MyBag.Items.Add(bagItem.Name);
                }
            }
        }

        private void AddToBagList_Click(object sender, EventArgs e)
        {
            if (InputAddToBagItem.Text != null)
            {
                MyBag.Items.Add(InputAddToBagItem.Text);
                Slog("Added {0} to Inventory List", InputAddToBagItem.Text);
            }
        }

        private void AddToSellList_Click(object sender, EventArgs e)
        {
            if (MyBag.SelectedItems[0] != null)
            {
                SellList.Items.Add(MyBag.SelectedItem);
                Controller.ItemNameSell.Add(MyBag.SelectedItem.ToString());
            }
        }

        private void AddToRemoveList_Click(object sender, EventArgs e)
        {
            if (MyBag.SelectedItems[0] != null)
            {
                RemoveList.Items.Add(MyBag.SelectedItem);
                Controller.ItemName.Add(MyBag.SelectedItem.ToString());
            }
        }

        private void AddToProtList_Click(object sender, EventArgs e)
        {
            if (MyBag.SelectedItems[0] != null)
            {
                ProtectedList.Items.Add(MyBag.SelectedItem);
                Controller.KeepList.Add(MyBag.SelectedItem.ToString());
            }
        }

        private void RemoveSellItem_Click(object sender, EventArgs e)
        {
            if (SellList.SelectedItem != null)
            {
                Slog("{0} Removed", SellList.SelectedItem.ToString());
                Controller.ItemNameSell.Remove(SellList.SelectedItem.ToString());
                SellList.Items.Remove(SellList.SelectedItem);
            }
        }

        private void RemoveRemoveItem_Click(object sender, EventArgs e)
        {
            if (RemoveList.SelectedItem != null)
            {
                Slog("{0} Removed", RemoveList.SelectedItem.ToString());
                Controller.ItemName.Remove(RemoveList.SelectedItem.ToString());
                RemoveList.Items.Remove(RemoveList.SelectedItem);
            }
        }

        private void RemoveProtectedItem_Click(object sender, EventArgs e)
        {
            if (ProtectedList.SelectedItem != null)
            {
                Slog("{0} Removed", ProtectedList.SelectedItem.ToString());
                Controller.KeepList.Remove(ProtectedList.SelectedItem.ToString());
                ProtectedList.Items.Remove(ProtectedList.SelectedItem);
            }
        }

        public void PrintSettings()
        {
            Dlog("Mr.ItemRemover2 Settings");
            Dlog("------------------------------------------");
            foreach (var setting in MrItemRemover2Settings.Instance.GetSettings())
            {
                string key = setting.Key;
                object value = setting.Value;
                Dlog(string.Format("{0} - {1}", key, value));
            }
            Dlog("------------------------------------------");
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Controller.MirSave();
            MrItemRemover2Settings.Instance.Save();
            PrintSettings();
            Close();
        }

        private void Time_ValueChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.Time = int.Parse(Time.Value.ToString(CultureInfo.InvariantCulture));
        }

        private void ReqRefLvl_ValueChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.ReqRefLvl = int.Parse(ReqRefLvl.Value.ToString(CultureInfo.InvariantCulture));
        }

        private void resf_Click(object sender, EventArgs e)
        {
            MyBag.Items.Clear();
            foreach (WoWItem bagItem in StyxWoW.Me.BagItems)
            {
                if (bagItem.BagSlot != -1 && !MyBag.Items.Contains(bagItem.Name))
                {
                    MyBag.Items.Add(bagItem.Name);
                }
            }
        }

        private void Run_Click(object sender, EventArgs e)
        {
            Controller.ManualCheckRequested = true;
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {
        }

        private void SellDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.EnableSell = SellDropDown.SelectedItem.ToString();
        }

        private void RemoveDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.EnableRemove = RemoveDropDown.SelectedItem.ToString();
        }

        private void OpeningDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.EnableOpen = OpeningDropDown.SelectedItem.ToString();
        }

        private void CheckAfterLootDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.LootCheck = CheckAfterLootDropDown.SelectedItem.ToString();
        }

        private void GoldGrays_TextChanged_1(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.GoldGrays = int.Parse(GoldGrays.Text);
        }

        private void SilverGrays_TextChanged_1(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SilverGrays = int.Parse(SilverGrays.Text);
        }

        private void CopperGrays_TextChanged_1(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.CopperGrays = int.Parse(CopperGrays.Text);
        }

        private void SellGraysDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SellGray = SellGraysDropDown.SelectedItem.ToString();
        }

        private void SellWhitesDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SellWhite = SellWhitesDropDown.SelectedItem.ToString();
        }

        private void SellGreensDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SellGreen = SellGreensDropDown.SelectedItem.ToString();
        }

        private void SellBluesDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SellBlue = SellBluesDropDown.SelectedItem.ToString();
        }

        private void SellSoulboundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SellSoulbound = SellSoulboundDropDown.SelectedItem.ToString();
        }

        private void SellFoodDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SellFood = SellFoodDropDown.SelectedItem.ToString();
        }

        private void SellDrinksDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.SellDrinks = SellDrinksDropDown.SelectedItem.ToString();
        }

        private void RemoveGraysDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.DeleteAllGray = RemoveGraysDropDown.SelectedItem.ToString();
        }

        private void RemoveWhitesDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.DeleteAllWhite = RemoveWhitesDropDown.SelectedItem.ToString();
        }

        private void RemoveGreensDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.DeleteAllGreen = RemoveGreensDropDown.SelectedItem.ToString();
        }

        private void RemoveBluesDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.DeleteAllBlue = RemoveBluesDropDown.SelectedItem.ToString();
        }

        private void RemoveQuestStartersDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.DeleteQuestItems = RemoveQuestStartersDropDown.SelectedItem.ToString();
        }

        private void RemoveFoodDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.RemoveFood = RemoveFoodDropDown.SelectedItem.ToString();
        }

        private void RemoveDrinksDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            MrItemRemover2Settings.Instance.RemoveDrinks = RemoveDrinksDropDown.SelectedItem.ToString();
        }
    }
}