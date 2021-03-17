using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class GetMarketDepthChartResponse : BizResponseClass
    {
        public GetBuySellMarketBook Response { get; set; }
    }
    public class GetBuySellMarketBookData
    {
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public Decimal Amount { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public Decimal Price { get; set; }
    }
    public class GetBuySellMarketBook
    {
       public List<GetBuySellMarketBookData> Bid { get; set; }
       public List<GetBuySellMarketBookData> Ask { get; set; }
    }
}
