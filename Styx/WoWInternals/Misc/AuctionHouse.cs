using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using GreenMagic;
using Styx.WoWInternals.WoWObjects;

namespace Styx.WoWInternals.Misc
{
    public static class AuctionHouse
    {
        private static AuctionFrame? _frame;
        private static int _searchPage;

        public static AuctionFrame Frame => _frame ??= new AuctionFrame();

        public static bool CanPerformSearch =>
            Lua.GetReturnVal<bool>("return CanSendAuctionQuery(\"list\")", 0U);

        public static bool CanPerformGetAllSearch =>
            Lua.GetReturnVal<bool>("return CanSendAuctionQuery(\"all\")", 0U);

        public static int NumListAuctions
        {
            get { GetNumAuctionItems(AuctionListType.List, out int b, out _); return b; }
        }

        public static int NumBidderAuctions
        {
            get { GetNumAuctionItems(AuctionListType.Bidder, out int b, out _); return b; }
        }

        public static int NumOwnerAuctions
        {
            get { GetNumAuctionItems(AuctionListType.Owner, out int b, out _); return b; }
        }

        public static int FullNumBidderAuctions
        {
            get { GetNumAuctionItems(AuctionListType.Bidder, out _, out int t); return t; }
        }

        public static int FullNumListAuctions
        {
            get { GetNumAuctionItems(AuctionListType.List, out _, out int t); return t; }
        }

        public static int NumListAuctionPages
        {
            get
            {
                int full = FullNumListAuctions;
                return full <= 50 ? 1 : (int)Math.Ceiling(full / 50.0);
            }
        }

        public static ulong CalculateAuctionDeposit(int runTime, int stackSize, int numStacks) =>
            Lua.GetReturnVal<ulong>(string.Format("return CalculateAuctionDeposit({0},{1},{2})", runTime, stackSize, numStacks), 0U);

        public static bool CanCancelAuction(int index) =>
            Lua.GetReturnVal<bool>(string.Format("return CanCancelAuction({0})", index), 0U);

        public static void CancelSell() => Lua.DoString("CancelSell()");

        public static void CancelAuction(int index) =>
            Lua.DoString(string.Format("CancelAuction({0})", index));

        public static void ClickAuctionSellItemButton() =>
            Lua.DoString("ClickAuctionSellItemButton()");

        public static ulong GetAuctionHouseDepositRate() =>
            Lua.GetReturnVal<ulong>("return GetAuctionHouseDepositRate()", 0U);

        /// <summary>
        /// Returns the number of auctions currently loaded in the specified list.
        /// Uses the WoW Lua API: GetNumAuctionItems(type).
        /// </summary>
        public static void GetNumAuctionItems(AuctionListType type, out int batch, out int totalCount)
        {
            string typeStr = TypeToString(type);
            var results = Lua.GetReturnValues(string.Format("return GetNumAuctionItems(\"{0}\")", typeStr));

            batch = results.Count > 0 ? ParseInt(results[0]) : 0;
            // Owner type returns only one value (batch == total for owner auctions)
            totalCount = type == AuctionListType.Owner
                ? batch
                : (results.Count > 1 ? ParseInt(results[1]) : batch);
        }

        public static bool GetAuction(AuctionListType list, int index, out WoWAuction auction)
        {
            GetNumAuctionItems(list, out int batch, out _);
            if (index >= 0 && index < batch)
            {
                FillAuctionFromLua(list, index + 1, out auction);
                return true;
            }
            auction = default;
            return false;
        }

        public static WoWAuction[] GetAllBatchAuctions(AuctionListType list)
        {
            GetNumAuctionItems(list, out int batch, out _);
            if (batch <= 0) return Array.Empty<WoWAuction>();

            var result = new WoWAuction[batch];
            for (int i = 0; i < batch; i++)
                FillAuctionFromLua(list, i + 1, out result[i]);
            return result;
        }

        public static WoWAuction[] GetOwnedAuctions() => GetAllBatchAuctions(AuctionListType.Owner);

        public static WoWAuction[] GetBidderAuctions() => GetAllBatchAuctions(AuctionListType.Bidder);

        public static WoWAuction[] GetListAuctions() => GetAllBatchAuctions(AuctionListType.List);

        public static List<WoWAuction> PerformSearch(string itemName) =>
            PerformSearch(itemName, 0);

        public static List<WoWAuction> PerformSearch(string itemName, int page)
        {
            if (!Frame.IsVisible) return null;

            while (!CanPerformSearch)
                Thread.Sleep(250);

            var sw = Stopwatch.StartNew();
            int prevCount = FullNumListAuctions;
            Lua.DoString(string.Format("QueryAuctionItems(\"{0}\",0,0,0,0,0,{1},0,0,0)", Lua.Escape(itemName), page));

            while (FullNumListAuctions == prevCount && !CanPerformSearch && sw.ElapsedMilliseconds < 10000)
                Thread.Sleep(250);

            var results = GetListAuctions().ToList();
            int pages = NumListAuctionPages;
            _searchPage++;
            if (pages > 1 && _searchPage < pages)
                results.AddRange(PerformSearch(itemName, _searchPage));

            return results;
        }

        public static void PostAuction(long minBid, long buyout, AuctionPostTime postTime,
            WoWItem item, uint stackSize, uint numStacks)
        {
            if (!Frame.IsVisible) return;
            if (item == null) throw new ArgumentNullException("item");

            item.PickUp();
            Lua.DoString("ClickAuctionSellItemButton()");
            Lua.DoString("ClearCursor()");
            Thread.Sleep(1000);
            Lua.DoString(string.Format("StartAuction({0},{1},{2},{3},{4})",
                minBid, buyout, (int)postTime, stackSize, numStacks));
        }

        private static string TypeToString(AuctionListType type)
        {
            switch (type)
            {
                case AuctionListType.List:   return "list";
                case AuctionListType.Bidder: return "bidder";
                case AuctionListType.Owner:  return "owner";
                default: throw new ArgumentException("type");
            }
        }

        private static void FillAuctionFromLua(AuctionListType list, int index, out WoWAuction auction)
        {
            // GetAuctionItemInfo(type, index) returns:
            // name, texture, count, quality, canUse, level, minBid, minIncrement, buyoutPrice, bidAmount, highBidder, bidderFullName, owner, ownerFullName, saleStatus, itemId, hasAllInfo
            string typeStr = TypeToString(list);
            var info = Lua.GetReturnValues(
                string.Format("return GetAuctionItemInfo(\"{0}\",{1})", typeStr, index));

            if (info == null || info.Count < 16)
            {
                auction = default;
                return;
            }

            auction = BuildAuctionFromLua(info);
        }

        private static WoWAuction BuildAuctionFromLua(System.Collections.Generic.List<string> info)
        {
            return new WoWAuction
            {
                AuctionId    = 0,                                              // not exposed by GetAuctionItemInfo
                ItemId       = info.Count > 15 ? (uint)ParseInt(info[15]) : 0,
                StackCount   = info.Count > 2  ? ParseInt(info[2]) : 1,
                MinBid       = info.Count > 6  ? ParseUInt(info[6]) : 0,
                MinIncrement = info.Count > 7  ? ParseUInt(info[7]) : 0,
                BuyoutPrice  = info.Count > 8  ? ParseUInt(info[8]) : 0,
                CurrentBid   = info.Count > 9  ? ParseUInt(info[9]) : 0,
                SaleStatus   = info.Count > 14 ? (info[14] == "1" ? 1u : 0u) : 0,
                EndTime      = 0,                                              // TimeLeft will return Zero
            };
        }

        private static int ParseInt(string s)
        {
            if (string.IsNullOrEmpty(s) || s == "nil") return 0;
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v) ? v : 0;
        }

        private static uint ParseUInt(string s)
        {
            if (string.IsNullOrEmpty(s) || s == "nil") return 0;
            return uint.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint v) ? v : 0;
        }
    }
}
