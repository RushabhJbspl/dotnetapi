using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Wallet
{
    public class TradingChartData : BizBase
    {
        public long PairId { get; set; }

        public long UserId { get; set; }

        public string Data { get; set; }
    }
}
