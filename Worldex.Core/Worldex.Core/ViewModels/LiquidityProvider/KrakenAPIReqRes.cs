using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.LiquidityProvider
{
    public class KrakenAPIReqRes
    {
    }
    public class KrakenMarket
    {
        public List<List<object>> asks { get; set; }
        public List<List<object>> bids { get; set; }
    }

    public class Result
    {
        public KrakenMarket Data { get; set; }
    }

    public class KrakenOrderBookResponse
    {
        public List<object> error { get; set; }
        public Result result { get; set; }
    }

    public class KrakenOrderBookBuySell
    {
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal quantity { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal rate { get; set; }
    }
}
