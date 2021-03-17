using System.Collections.Generic;

namespace Worldex.Core.ViewModels.LiquidityProvider
{
    public class Bid
    {
        public string price { get; set; }
        public string amount { get; set; }
        public string timestamp { get; set; }
    }

    public class Ask
    {
        public string price { get; set; }
        public string amount { get; set; }
        public string timestamp { get; set; }
    }

    public class BitfinexOrderbookRes
    {
        public string message { get; set; }
        public List<Bid> bids { get; set; }
        public List<Ask> asks { get; set; }
    }
    public class BitfinexOrderBookResponse
    {        
        public BitfinexOrderbookRes Data { get; set; }
    }
}
