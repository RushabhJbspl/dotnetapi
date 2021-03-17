using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class GetRecentTradeResponceV1 : BizResponseClass
    {
        public List<RecentOrderInfoV1> response { get; set; }
    }
    public class RecentOrderInfoV1
    {
        public string GUID { get; set; }
        public string TrnNo { get; set; }
        public string Type { get; set; }
        public Decimal Price { get; set; }
        public Decimal? SettlementPrice { get; set; }//komal 02-07-2019 
        public Decimal Qty { get; set; }
        public Decimal SettledQty { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime? SettledDate { get; set; }
        public string Status { get; set; }
        public short StatusCode { get; set; }
        public string PairName { get; set; }
        public long PairId { get; set; }
        public string OrderType { get; set; }
        public short ISFollowersReq { get; set; }
        public short IsCancel { get; set; } = 0;//Rita 22-3-19 added for in cancellation process display with fail status in front ,as present in API response GetTradeHistoryInfo
        public Decimal? ChargeRs { get; set; }
        public string Chargecurrency { get; set; }
    }

    public class GetRecentTradeResponceArbitrageV1 : BizResponseClass
    {
        public List<RecentOrderInfoArbitrageV1> response { get; set; }
    }
    public class RecentOrderInfoArbitrageV1
    {
        public string TrnNo { get; set; }
        public string GUID { get; set; }
        public string Type { get; set; }
        public Decimal Price { get; set; }
        public Decimal? SettlementPrice { get; set; }//komal 02-07-2019 
        public Decimal Qty { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public string PairName { get; set; }
        public long PairId { get; set; }
        public string OrderType { get; set; }
        public short StatusCode { get; set; }
        public DateTime? SettledDate { get; set; }
        public Decimal SettledQty { get; set; }
        public short ISFollowersReq { get; set; }
        public short IsCancel { get; set; } = 0;//Rita 22-3-19 added for in cancellation process display with fail status in front ,as present in API response GetTradeHistoryInfo
        public string ExchangeName { get; set; }//komal 08-06-2019 add exchange name
    }
}
