using System;
using System.Collections.Generic;
using Worldex.Core.ApiModels;
using Worldex.Core.Enums;

namespace Worldex.Core.ViewModels.Transaction.MarketMaker
{
    /// <summary>
    /// store market maker buy prefences get from database join query 
    /// </summary>
    /// <remarks>-Sahil 12-10-2019 12:11 PM</remarks>
    public class MarketMakerBuyPreferencesViewModel
    {
        //comment market  maker validation is not in used currently -Sahil 15-10-2019 04:08 PM
        public long Id { get; set; }
        public long UserId { get; set; }
        public long PairId { get; set; }
        public string PairName { get; set; }
        public string ProviderName { get; set; }

        public long BuyLTPPrefProID { get; set; }

        public RangeType BuyLTPRangeType { get; set; }

        //change datatype double to decimal for avoid explicit conversion -Sahil 17-10-2019 05:52 PM
        //change datatype int to double  for Percentage -Sahil 11-10-2019 03:24 PM
        public decimal BuyUpPercentage { get; set; }
        public decimal BuyDownPercentage { get; set; }

        public decimal BuyThreshold { get; set; }

        //commented as defined in separate class -Sahil 17-10-2019 05:32 PM
        //public decimal HoldOrderRateChange { get; set; }
        //public string HoldOrderRateChange { get; set; }//rita 16-10-19 taken dynamic configuration for for multiple market maker hold txn
    }

    public class GetMarketMakerHoldOrderRateChange //rita 16-10-19 make new class for only one column
    {
        public string HoldOrderRateChange { get; set; }
    }

    public class MarketMakerBalancePerformanceResponse : BizResponseClass
    {
        public List<MarketMakerBalancePerformanceViewModel> Response { get; set; }
    }
    public class MarketMakerBalancePerformanceViewModel
    {
        public string WalletTypeName { get; set; }
        public Decimal OldBalance { get; set; }
        public Decimal NewBalance { get; set; }
    }

    public class MarketMakerTradePerformanceResponse : BizResponseClass
    {
        public List<MarketMakerTradePerformance> Response { get; set; }
    }

    public class MarketMakerTradePerformance
    {
        public string PairName { get; set; }
        public MarketMakerBuySellCount TradeCount { get; set; }
    }
    public class MarketMakerBuySellCount
    {
        public long BuyCount { get; set; }
        public decimal BuyAvgPrice { get; set; }
        public long SellCount { get; set; }
        public decimal SellAvgPrice { get; set; }
    }
    public class GetMarketMakerUser
    {
        public long UserID { get; set; }
    }
    public class MarketMakerTradePairList
    {
        public long PairID { get; set; }
        public string PairName { get; set; }
    }
    public class MarketMakerTradeCountListQryRes
    {
        public long Count { get; set; }
        public decimal AvgPrice { get; set; }
        public long PairID { get; set; }
    }

    public class MarketMakerSettleTrxnByTakeViewModel
    {
        public short TrnType { get; set; }
        public short OrderType { get; set; }
        public string SMSCode { get; set; }
        public string TransactionAccount { get; set; }
        public long PairID { get; set; }
        public long MemberId { get; set; }
        public long orderWalletID { get; set; }
        public long deliveryWalletID { get; set; }
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
    }
}
