using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class GetOrderHistoryResponse : BizResponseClass
    {
        public List<GetOrderHistoryInfo> response { get; set; }
    }
    public class GetOrderHistoryInfo
    {
        public string GUID { get; set; }
        public string TrnNo { get; set; }
        public DateTime DateTime { get; set; }
        public Decimal Price { get; set; }
        public Decimal SettlementPrice { get; set; }
        public Decimal Amount { get; set; }
        public Decimal SettledQty { get; set; }
        public Decimal Total { get; set; }
        public string Type { get; set; }
        public String PairName { get; set; }
        public short IsCancel { get; set; }
        public string OrderType { get; set; }
        public DateTime? SettledDate { get; set; }
    }

    public class GetTradeHistoryResponseV1 : BizResponseClass
    {
        public List<GetTradeHistoryInfoV1> response { get; set; }
    }
    public class GetTradeHistoryInfoV1
    {
        public string TrnNo { get; set; }
        public string GUID { get; set; }
        public DateTime DateTime { get; set; }
        public Decimal Price { get; set; }
        public Decimal? SettlementPrice { get; set; }
        public Decimal Amount { get; set; }
        public Decimal SettledQty { get; set; }
        public Decimal? Total { get; set; }
        public short Status { get; set; }
        public string Type { get; set; }
        public String PairName { get; set; }
        public string StatusText { get; set; }
        public Decimal? ChargeRs { get; set; }
        public short IsCancel { get; set; }
        public string OrderType { get; set; }
        public DateTime? SettledDate { get; set; }
        public string Chargecurrency { get; set; }

    }

    public class GetOrderHistoryResponseArbitrageV1 : BizResponseClass
    {
        public List<GetOrderHistoryInfoArbitrageV1> response { get; set; }
    }

    public class GetOrderHistoryInfoArbitrageV1
    {
        public string TrnNo { get; set; }
        public string GUID { get; set; }
        public DateTime DateTime { get; set; }
        public Decimal Price { get; set; }
        public Decimal SettlementPrice { get; set; }
        public Decimal Amount { get; set; }
        public Decimal SettledQty { get; set; }
        public Decimal Total { get; set; }
        public string Type { get; set; }
        public String PairName { get; set; }
        public short IsCancel { get; set; }
        public string OrderType { get; set; }
        public DateTime? SettledDate { get; set; }
        public string ExchangeName { get; set; }
    }
    public class GetTradeHistoryResponseArbitrageV1 : BizResponseClass
    {
        public List<GetTradeHistoryInfoArbitrageV1> response { get; set; }
    }
    public class GetTradeHistoryInfoArbitrageV1
    {
        public string TrnNo { get; set; }
        public string GUID { get; set; }
        public string Type { get; set; }
        public Decimal Price { get; set; }
        public Decimal? SettlementPrice { get; set; }//komal 08-06-2019 
        public Decimal Amount { get; set; }
        public Decimal Total { get; set; }
        public DateTime DateTime { get; set; }
        public short Status { get; set; }
        public string StatusText { get; set; }
        public String PairName { get; set; }
        public Decimal? ChargeRs { get; set; }
        public short IsCancel { get; set; }
        public string OrderType { get; set; }
        public DateTime? SettledDate { get; set; }
        public Decimal SettledQty { get; set; }
        public string Chargecurrency { get; set; }
        public string ExchangeName { get; set; }//komal 08-06-2019 add exchange name
    }
}
