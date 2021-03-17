using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class OpenOrderQryResponse
    {
        public long TrnNo { get; set; }
        public DateTime TrnDate { get; set; }
        public string Type { get; set; }
        public short ordertype { get; set; }
        public string PairName { get; set; }
        public long PairId { get; set; }
        public string Order_Currency { get; set; }
        public string Delivery_Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public short IsCancelled { get; set; }
    }

    public class GetOpenOrderResponse : BizResponseClass
    {
        public List<OpenOrderInfo> Response { get; set; }
    }
    public class OpenOrderInfo
    {
        public long Id { get; set; }
        public DateTime TrnDate { get; set; }
        public string Type { get; set; }
        public string Order_Currency { get; set; }
        public string Delivery_Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public short IsCancelled { get; set; }
        public string PairName { get; set; }
        public long PairId { get; set; }
        public string OrderType { get; set; }
    }
    public class GetOpenOrderRequest
    {

        public string Pair { get; set; }

        public string OrderType { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public int Page { get; set; }

        public short IsMargin { get; set; } = 0;//Rita 21-2-19,   1-for Margin trading cancel txn
    }
}
