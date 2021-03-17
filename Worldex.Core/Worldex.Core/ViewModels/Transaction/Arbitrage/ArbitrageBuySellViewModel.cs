using Worldex.Core.ApiModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.Transaction.Arbitrage
{
    public class ArbitrageBuySellViewModel
    {
        public long LPType { get; set; }
       // public long RouteID { get; set; }
        public string ProviderName { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal LTP { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Fees { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MinNotional { get; set; } = 0;
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MaxNotional { get; set; } = 0;

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MinPrice { get; set; } = 0;
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MaxPrice { get; set; } = 0;

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MinQty { get; set; } = 0;
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MaxQty { get; set; } = 0;
    }
    public class ArbitrageBuySellResponse : BizResponseClass
    {
        public List<ArbitrageBuySellViewModel> Response { get; set; }
    }
}
