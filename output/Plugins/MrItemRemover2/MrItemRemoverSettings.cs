using System;
using System.IO;
using Styx;
using Styx.Helpers;

namespace MrItemRemover2
{
    public class MrItemRemover2Settings : Settings
    {
        public static readonly MrItemRemover2Settings Instance = new MrItemRemover2Settings();

        private MrItemRemover2Settings()
            : base(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Settings/MrItemRemover2_Settings_{0}.xml",
                    StyxWoW.Me.Name))
        {
        }

        [Setting, DefaultValue("False")]
        public string EnableSell { get; set; }

        [Setting, DefaultValue("False")]
        public string EnableRemove { get; set; }

        [Setting, DefaultValue("False")]
        public string CombineItems { get; set; }

        [Setting, DefaultValue("False")]
        public string EnableOpen { get; set; }

        [Setting, DefaultValue("False")]
        public string LootCheck { get; set; }

        [Setting, DefaultValue("False")]
        public string SellGray { get; set; }

        [Setting, DefaultValue("False")]
        public string SellWhite { get; set; }

        [Setting, DefaultValue("False")]
        public string SellGreen { get; set; }

        [Setting, DefaultValue("False")]
        public string SellBlue { get; set; }

        [Setting, DefaultValue("False")]
        public string SellSoulbound { get; set; }

        [Setting, DefaultValue("False")]
        public string SellFood { get; set; }

        [Setting, DefaultValue("False")]
        public string SellDrinks { get; set; }

        [Setting, DefaultValue("False")]
        public string DeleteAllGray { get; set; }

        [Setting, DefaultValue("False")]
        public string DeleteAllWhite { get; set; }

        [Setting, DefaultValue("False")]
        public string DeleteAllGreen { get; set; }

        [Setting, DefaultValue("False")]
        public string DeleteAllBlue { get; set; }

        [Setting, DefaultValue("False")]
        public string DeleteQuestItems { get; set; }

        [Setting, DefaultValue("False")]
        public string RemoveFood { get; set; }

        [Setting, DefaultValue("False")]
        public string RemoveDrinks { get; set; }

        [Setting, DefaultValue(1)]
        public int GoldGrays { get; set; }

        [Setting, DefaultValue(53)]
        public int SilverGrays { get; set; }

        [Setting, DefaultValue(41)]
        public int CopperGrays { get; set; }

        [Setting, DefaultValue(5)]
        public int Time { get; set; }

        [Setting, DefaultValue(0)]
        public int ReqRefLvl { get; set; }
    }
}