using System.Collections.Generic;

namespace Worldex.Core.ViewModels.LiquidityProvider
{
    public class EXMOAPIReqRes
    {
        public class Symbol
        {
            public string buy_price { get; set; }
            public string sell_price { get; set; }
            public string last_trade { get; set; }
            public string high { get; set; }
            public string low { get; set; }
            public string avg { get; set; }
            public string vol { get; set; }
            public string vol_curr { get; set; }
            public int updated { get; set; }
        }

        public class EXMOLTPCheckResponse
        {
            public Symbol symbol { get; set; }
        }

        public class ExmoPlaceOrderResult
        {
            public bool result { get; set; }
            public string error { get; set; }
            public int order_id { get; set; }
        }

        public class ExmoCancelOrderResult
        {
            public bool result { get; set; }
            public string error { get; set; }
        }
    }
    public class PairData
    {
        public string ask_quantity { get; set; }
        public string ask_amount { get; set; }
        public string ask_top { get; set; }
        public string bid_quantity { get; set; }
        public string bid_amount { get; set; }
        public string bid_top { get; set; }
        public List<List<string>> ask { get; set; }
        public List<List<string>> bid { get; set; }
    }

    public class EXMOOrderbookResponse
    {
        public PairData Data { get; set; }
    }

    public class EXMOMarketData
    {
        public int trade_id { get; set; }
        public string type { get; set; }
        public string quantity { get; set; }
        public string price { get; set; }
        public string amount { get; set; }
        public double date { get; set; }
    }

    public class EXMOTradeHistoryResponse 
    {
        public List<EXMOMarketData> Data { get; set; }
    }


    public class Balances
    {
        public string Currency { get; set; }        
    }

    public class Reserved
    {
        public string Currency { get; set; }
    }

    public class EXMOBalanceResponse 
    {
        public int uid { get; set; }
        public int server_date { get; set; }
        public Balances balances { get; set; }
        public Reserved reserved { get; set; }
    }
    public class EXMOPlaceOrderResponse 
    {
        public bool result { get; set; }
        public string error { get; set; }
        public int order_id { get; set; }
    }

    public class EXMOOpenOrderUnit 
    {
        public string order_id { get; set; }
        public string created { get; set; }
        public string type { get; set; }
        public string pair { get; set; }
        public string price { get; set; }
        public string quantity { get; set; }
        public string amount { get; set; }
    }

    public class EXMOOpenOrderResp 
    {
        public List<EXMOOpenOrderUnit> Data { get; set; }
    }
    public class EXMOCancelOrderListResp 
    {
        public int date { get; set; }
        public int order_id { get; set; }
        public string order_type { get; set; }
        public string pair { get; set; }
        public int price { get; set; }
        public int quantity { get; set; }
        public int amount { get; set; }
    }

    public class EXMOTrade 
    {
        public int trade_id { get; set; }
        public int date { get; set; }
        public string type { get; set; }
        public string pair { get; set; }
        public int order_id { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
        public int amount { get; set; }
    }

    public class EXMOOrderTradeResponse 
    {
        public bool result { get; set; }
        public string error { get; set; }
        public string type { get; set; }
        public string in_currency { get; set; }
        public string in_amount { get; set; }
        public string out_currency { get; set; }
        public string out_amount { get; set; }
        public List<EXMOTrade> trades { get; set; }
    }

    public class EXMOCancelOrderListResponse
    {
        public int date { get; set; }
        public int order_id { get; set; }
        public string order_type { get; set; }
        public string pair { get; set; }
        public int price { get; set; }
        public int quantity { get; set; }
        public int amount { get; set; }
    }

}
