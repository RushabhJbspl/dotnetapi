using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class GetPairRatesResponse : BizResponseClass
    {
        public PairRatesResponse response { get; set; }
    }
    public class PairRatesResponse
    {
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal BuyPrice { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal SellPrice { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal BuyMaxPrice { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal BuyMinPrice { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal SellMaxPrice { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal SellMinPrice { get; set; }
    }
}
