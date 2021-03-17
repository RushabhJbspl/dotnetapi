using System;

namespace Worldex.Core.ViewModels.Transaction.BackOffice
{
    public class TradingSummaryViewModel
    {
        public String TrnNo { get; set; }
        public long MemberID { get; set; }
        public string Type { get; set; }
        public Decimal Price { get; set; }
        public Decimal Amount { get; set; }
        public Decimal Total { get; set; }
        public DateTime DateTime { get; set; }
        public string StatusText { get; set; }
        public long PairID { get; set; }
        public String PairName { get; set; }
        public Decimal ChargeRs { get; set; }
        public string OrderType { get; set; }
        public short StatusCode { get; set; }
        public short IsCancelled { get; set; }
        public decimal SettleQty { get; set; }
    }

    public class TradingSummaryLPViewModel
    {
        public string TrnNo { get; set; }
        public long MemberID { get; set; }
        public string Type { get; set; }
        public Decimal Price { get; set; }
        public Decimal Amount { get; set; }
        public Decimal Total { get; set; }
        public DateTime DateTime { get; set; }
        public string StatusText { get; set; }
        public long PairID { get; set; }
        public String PairName { get; set; }
        public Decimal ChargeRs { get; set; }
        public string OrderType { get; set; }
        public short StatusCode { get; set; }
        public short IsCancelled { get; set; }
        public decimal SettleQty { get; set; }
        public string ProviderName { get; set; }
    }


    public class TradingReconHistoryViewModel
    {
        public string TrnNo { get; set; }
        public long MemberID { get; set; }
        public string Type { get; set; }
        public Decimal Price { get; set; }
        public Decimal Amount { get; set; }
        public Decimal Total { get; set; }
        public DateTime DateTime { get; set; }
        public string StatusText { get; set; }
        public long PairID { get; set; }
        public String PairName { get; set; }
        public Decimal ChargeRs { get; set; }
        public string OrderType { get; set; }
        public short StatusCode { get; set; }
        public short IsCancelled { get; set; }
        public decimal SettleQty { get; set; }
        public string ProviderName { get; set; }
        public int IsProcessing { get; set; }
        public string ActionStage { get; set; }
        public short IsAPITrade { get; set; }
        public string UserName { get; set; }
    }
}
