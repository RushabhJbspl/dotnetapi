using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class ActiveOrderDataResponse
    {
        public long TrnNo { get; set; }
        public DateTime  TrnDate { get; set; }
        public string Type { get; set; }
        public short ordertype { get; set; }
        public string PairName { get; set; }
        public long PairId { get; set; }
        public string Order_Currency { get; set; }
        public string Delivery_Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public short IsCancelled { get; set; }
        //public decimal SellQty { get; set; }
        //public decimal BuyQty { get; set; }
        //public decimal AskPrice { get; set; }
        //public decimal BidPrice { get; set; }
        public DateTime? SettledDate { get; set; }//Rita 12-3-19 added for needed at front side
        public decimal SettledQty { get; set; }
    }
    public class ActiveOrderDataResponseArbitrage
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
        //public decimal SellQty { get; set; }
        //public decimal BuyQty { get; set; }
        //public decimal AskPrice { get; set; }
        //public decimal BidPrice { get; set; }
        public DateTime? SettledDate { get; set; }//Rita 12-3-19 added for needed at front side
        public decimal SettledQty { get; set; }
        public string ExchangeName { get; set; }//komal 08-06-2019 add exchange name
    }
}
