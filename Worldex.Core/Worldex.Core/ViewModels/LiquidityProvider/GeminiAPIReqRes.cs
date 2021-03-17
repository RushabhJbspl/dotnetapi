using System.Collections.Generic;

namespace Worldex.Core.ViewModels.LiquidityProvider
{
    public class GeminiOrderbookRes
    {
        public string result { get; set; }
        public string message { get; set; }
        public List<Bid> bids { get; set; }
        public List<Ask> asks { get; set; }
    }
    public class GeminiOrderbookResponse
    {
        public GeminiOrderbookRes Data { get; set; }
    }

    public class GeminiBalanceRes
    {
        public string type { get; set; }
        public string currency { get; set; }
        public string amount { get; set; }
        public string available { get; set; }
        public string availableForWithdrawal { get; set; }
    }
    public class GeminiBalanceResponse
    {
        public string result { get; set; }
        public string message { get; set; }
        public List<GeminiBalanceRes> Data { get; set; }
    }

    public class GeminiTradeHistoryRes
    {
        public int timestamp { get; set; }
        public object timestampms { get; set; }
        public long tid { get; set; }
        public string price { get; set; }
        public string amount { get; set; }
        public string exchange { get; set; }
        public string type { get; set; }
    }

    public class GeminiTradeHistoryResponse
    {
        public string result { get; set; }
        public string message { get; set; }
        public List<GeminiTradeHistoryRes> Data { get; set; }
    }

    public class GeminiPlaceOrderRes
    {
        public string order_id { get; set; }
        public string id { get; set; }
        public string symbol { get; set; }
        public string exchange { get; set; }
        public string avg_execution_price { get; set; }
        public string side { get; set; }
        public string type { get; set; }
        public string timestamp { get; set; }
        public long timestampms { get; set; }
        public bool is_live { get; set; }
        public bool is_cancelled { get; set; }
        public bool is_hidden { get; set; }
        public bool was_forced { get; set; }
        public string executed_amount { get; set; }
        public string remaining_amount { get; set; }
        public string client_order_id { get; set; }
        public List<object> options { get; set; }
        public string price { get; set; }
        public string original_amount { get; set; }
        public string result { get; set; }
        public string message { get; set; }
    }

    public class GeminiStatusCheckRes 
    {
        public string order_id { get; set; }
        public string id { get; set; }
        public string symbol { get; set; }
        public string exchange { get; set; }
        public string avg_execution_price { get; set; }
        public string side { get; set; }
        public string type { get; set; }
        public string timestamp { get; set; }
        public long timestampms { get; set; }
        public bool is_live { get; set; }
        public bool is_cancelled { get; set; }
        public bool is_hidden { get; set; }
        public bool was_forced { get; set; }
        public string executed_amount { get; set; }
        public string remaining_amount { get; set; }
        public List<object> options { get; set; }
        public string price { get; set; }
        public string original_amount { get; set; }
    }
    public class GeminiStatusCheckResponse
    {
        public string result { get; set; }
        public string message { get; set; }
        public GeminiStatusCheckRes Data { get; set; }
    }
}
