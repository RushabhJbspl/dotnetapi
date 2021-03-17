using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Transaction
{
    public class MarginTradingAllowToUser : BizBase
    {
        public long UserId { get; set; }
    }

    public class ArbitrageTradingAllowToUser : BizBase
    {
        public long UserId { get; set; }

        public short SmaartTradePriority { get; set; } = 0;//0-None , 1-BuyFirst ,2 - Sell First
    }
}
