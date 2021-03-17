using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Transaction;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Configuration
{
    public class GetMarketTickerPairData : BizResponseClass
    {
        public List<MarketTickerPairData> Response { get; set; }
    }

    public class MarketTickerPairData
    {
        public long PairId { get; set; }
        public string PairName { get; set; }
        public int IsMarketTicker { get; set; }
    } 

    public class UpdateMarketTickerPairData
    {
        public List<MarketTickerPairData> Request { get; set; }
        public short IsMargin { get; set; } = 0;//Rita 5-3-19 for margin trading
    }

    public class GetMarketTickerResponse : BizResponseClass
    {
        public List<VolumeDataRespose> Response { get; set; }
    }
}
