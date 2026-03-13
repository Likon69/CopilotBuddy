using System;
using System.Runtime.InteropServices;
using GreenMagic;

namespace Styx.WoWInternals.Misc
{
    /// <summary>
    /// WoW 3.3.5a auction entry struct.
    /// Memory layout derived from HB 3.3.5a AuctionEntry struct.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WoWAuction
    {
        private readonly uint _unk00;

        public uint AuctionId;

        public uint ItemId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        private readonly WoWAucEnchantInfo[] _enchantInfo;

        public uint RandomPropertyId;

        public uint ItemSuffixFactor;

        public int StackCount;

        public uint SpellCharges;

        private readonly uint _unk70;

        private readonly uint _unk74;

        public ulong PosterGuid;

        public uint MinBid;

        public uint MinIncrement;

        public uint BuyoutPrice;

        public uint EndTime;

        private readonly uint _bidderGuidHigh;
        private readonly uint _bidderGuidLow;

        public uint CurrentBid;

        public uint SaleStatus;

        public bool IsSold => SaleStatus != 0;

        public TimeSpan TimeLeft
        {
            get
            {
                ulong now = StyxWoW.WoWClient.PerformanceCounter();
                ulong endMs = EndTime * 1000UL;
                if (endMs <= now) return TimeSpan.Zero;
                return TimeSpan.FromMilliseconds(endMs - now);
            }
        }

        public DateTime ExpireTime => DateTime.Now.Add(TimeLeft);

        public string ItemName
        {
            get
            {
                var name = Lua.GetReturnVal<string>(string.Format("return GetItemInfo({0})", ItemId), 0U);
                return string.IsNullOrEmpty(name) ? "Unknown" : name;
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}] AuctionId:{1} MinBid:{2} CurrentBid:{3} MinIncrement:{4} BuyoutPrice:{5} StackCount:{6} ItemId:{7} IsSold:{8} TimeLeft:{9}",
                ItemName, AuctionId, MinBid, CurrentBid, MinIncrement, BuyoutPrice, StackCount, ItemId, IsSold, TimeLeft);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WoWAucEnchantInfo
    {
        public uint EnchantId;
        public uint EnchantDuration;
        public uint EnchantCharges;
    }
}
