using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Charges
{
    public class CurrencyRateDetail : BizBase
    {
        [Required]
        public long CurrencyRateMasterId { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(33, 18)")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(33, 18)")]//here old is (25,10) so length is 15
        public decimal Volume_24h { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(33, 18)")]
        public decimal Market_cap { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(33, 18)")]
        public decimal Percent_change_1h { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(33, 18)")]
        public decimal Percent_change_24h { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(33, 18)")]
        public decimal Percent_change_7d { get; set; }

        [Required]
        public string Last_updated { get; set; }

        [Required]
        public string CoinName { get; set; }

        [Required]
        public string Symbol { get; set; }
                
        public DateTime? Last_updatedDateTime { get; set; }


    }
}
