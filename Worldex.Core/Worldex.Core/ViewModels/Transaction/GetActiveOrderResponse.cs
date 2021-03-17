using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class GetActiveOrderRequest
    {
        public string Pair { get; set; }

        public string OrderType { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public int Page { get; set; }

        public short IsMargin { get; set; } = 0;//Rita 21-2-19,   1-for Margin trading cancel txn
    }

    public class GetActiveOrderResponseV1 : BizResponseClass
    {
        public List<ActiveOrderInfoV1> response { get; set; }
    }


    public class ActiveOrderInfoV1
    {
        public string Id { get; set; }
        public string GUID { get; set; }
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
        public DateTime? SettledDate { get; set; }//Rita 12-3-19 added for needed at front side
        public decimal SettledQty { get; set; }
        public Decimal? ChargeRs { get; set; }
        public string Chargecurrency { get; set; }
    }

    public class GetActiveOrderResponseArbitrageV1 : BizResponseClass
    {
        public List<ActiveOrderInfoArbitrageV1> response { get; set; }
    }
    public class ActiveOrderInfoArbitrageV1
    {
        public string Id { get; set; }
        public string GUID { get; set; }
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
        public DateTime? SettledDate { get; set; }//Rita 12-3-19 added for needed at front side
        public decimal SettledQty { get; set; }
        public string ExchangeName { get; set; }//komal 08-06-2019 add exchange name
        public Decimal? ChargeRs { get; set; }
        public string Chargecurrency { get; set; }
    }

    public class RemoveActiveOrder
    {
        public List<long> Data { get; set; }
    }
}
