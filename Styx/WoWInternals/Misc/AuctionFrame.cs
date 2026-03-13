using Styx.Logic.Inventory.Frames;
using Styx.WoWInternals;

namespace Styx.WoWInternals.Misc
{
    public class AuctionFrame : Frame
    {
        public AuctionFrame()
            : base("AuctionFrame")
        {
        }

        public void Close()
        {
            Lua.DoString("CloseAuctionHouse()");
        }

        public override void Hide()
        {
            Close();
        }
    }
}
