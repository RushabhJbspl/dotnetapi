using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.LiquidityProvider
{
    public class YobitAPIReqRes
    {
        public class YobitLTPResult
        {
            public decimal high { get; set; }
            public decimal low { get; set; }
            public decimal avg { get; set; }
            public decimal vol { get; set; }
            public decimal vol_cur { get; set; }
            public decimal last { get; set; }
            public decimal buy { get; set; }
            public decimal sell { get; set; }
            public int updated { get; set; }
        }

        public class YobitLTPCheckRespopnse
        {
            public YobitLTPResult result { get; set; }
        }

        public class GetInfoResponse
        {
            public Return @return { get; set; }
            public string error { get; set; }
        }

        public class Return
        {
           
            public Funds funds { get; set; }
            public Funds_Incl_Orders funds_incl_orders { get; set; }
            public Rights rights { get; set; }
            [Obsolete]
            public int transaction_count { get; set; }
            [Obsolete]
           public int open_orders { get; set; }
            public int server_time { get; set; }
        }

        public class Funds : Dictionary<string, float>
        {
        }

        public class Funds_Incl_Orders : Dictionary<string, float>
        {
        }

        public class Rights
        {
            public int info { get; set; }
            public int trade { get; set; }
            public int withdraw { get; set; }
        }


        public class TradeResponse
        {
            public TradeResult tradeResult { get; set; }
            public string error { get; set; }
        }

        public class TradeResult
        {   
            public float received { get; set; }
            
            public int remains { get; set; }
            
            public int order_id { get; set; }

            public Funds funds { get; set; }
        }

        public class ExchangeOrderResult
        {
            public string error { get; set; }
            public decimal Price { get; set; }
            public decimal Fees { get; set; }
            public bool IsBuy { get; set; }
            public string MarketSymbol { get; set; }
            public DateTime FillDate { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal AveragePrice { get; set; }
            public string TradeId { get; set; }
            public decimal AmountFilled { get; set; }
            public decimal Amount { get; set; }
            public string Message { get; set; }
            public ExchangeAPIOrderResult Result { get; set; }
            public string OrderId { get; set; }
            public string FeesCurrency { get; set; }
        }

        public enum ExchangeAPIOrderResult
        {
            Unknown = 0,
            Filled = 1,
            FilledPartially = 2,
            Pending = 3,
            Error = 4,
            Canceled = 5,
            FilledPartiallyAndCancelled = 6,
            PendingCancel = 7
        }

        public class YobitCancelOrderResult
        {
            public int success { get; set; }
            public string error { get; set; }
            public YobitCancelOrder result { get; set; }
        }

        public class YobitCancelOrder
        {
            public int order_id { get; set; }
            public Funds funds { get; set; }
        }
    }
}
