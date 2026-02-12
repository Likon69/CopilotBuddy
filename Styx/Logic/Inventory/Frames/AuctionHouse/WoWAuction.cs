using System;
using Styx.WoWInternals;

namespace Styx.Logic.Inventory.Frames.AuctionHouse
{
    /// <summary>
    /// FEAT-34: Represents a single auction listing.
    /// Data comes from Lua GetAuctionItemInfo().
    /// Ported from HB 4.3.4 WoWAuction struct.
    /// </summary>
    public class WoWAuction
    {
        public string Name { get; set; } = string.Empty;
        public int Texture { get; set; }
        public int Count { get; set; }
        public int Quality { get; set; }
        public bool CanUse { get; set; }
        public int Level { get; set; }
        public int MinBid { get; set; }
        public int MinIncrement { get; set; }
        public int BuyoutPrice { get; set; }
        public int CurrentBid { get; set; }
        public bool IsHighBidder { get; set; }
        public string Owner { get; set; } = string.Empty;
        public int TimeLeft { get; set; }
        public int AuctionId { get; set; }
        public int ItemId { get; set; }

        /// <summary>
        /// Time left as a human-readable string.
        /// WoW timeLeft: 1=Short (<30m), 2=Medium (30m-2h), 3=Long (2h-12h), 4=Very Long (12h+)
        /// </summary>
        public string TimeLeftString => TimeLeft switch
        {
            1 => "Short",
            2 => "Medium",
            3 => "Long",
            4 => "Very Long",
            _ => "Unknown"
        };

        /// <summary>
        /// Whether this auction has a buyout price.
        /// </summary>
        public bool HasBuyout => BuyoutPrice > 0;

        /// <summary>
        /// Price per item (buyout / count).
        /// </summary>
        public int UnitBuyoutPrice => Count > 0 ? BuyoutPrice / Count : 0;

        public override string ToString()
        {
            return $"[Auction: {Name} x{Count} (Bid: {CurrentBid}, Buyout: {BuyoutPrice}, Owner: {Owner})]";
        }
    }
}
