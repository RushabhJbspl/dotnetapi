using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction.BackOfficeCount
{
    public class ConfigurationCount
    {
        public long CoinCount { get; set; }
        public long PairCount { get; set; }
        public long DaemonCount { get; set; }
        public long ProviderCount { get; set; }
        public long MarketCount { get; set; }
        public long LiquidityCount { get; set; }
        public long TradeRouteCount { get; set; }
        public long APICount { get; set; }
        public long APIResponseCount { get; set; }
        public long MarketCapTickerCount { get; set; }
        public long SiteToken { get; set; }
        public long ServiceProviderCount { get; set; }
        public long ExchangeFeedConfigCount { get; set; }
        public long ExchangeFeedLimitsCount { get; set; }
        public long Count
        {
            get { return CoinCount + PairCount + DaemonCount + ProviderCount + MarketCount + LiquidityCount + TradeRouteCount+ APICount+ APIResponseCount+ MarketCapTickerCount+ SiteToken+ ServiceProviderCount+ ExchangeFeedConfigCount+ ExchangeFeedLimitsCount; }
            set { value = CoinCount + PairCount + DaemonCount + ProviderCount + MarketCount + LiquidityCount + TradeRouteCount+ APICount+ APIResponseCount+ MarketCapTickerCount + SiteToken + ServiceProviderCount + ExchangeFeedConfigCount + ExchangeFeedLimitsCount; }
        }
    }
    public class ConfigurationCountResponse : BizResponseClass
    {
        public ConfigurationCount Response { get; set; }
    }
}
