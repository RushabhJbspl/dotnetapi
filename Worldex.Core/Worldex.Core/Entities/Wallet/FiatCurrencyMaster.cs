using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Wallet
{
    public class FiatCurrencyMaster : BizBase
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string CurrencyCode { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal USDRate { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal BuyFee { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public  decimal SellFee { get; set;}

        [Required]
        public short BuyFeeType { get; set;}//1 -fixed 2 -percentage

        [Required]
        public short SellFeeType { get; set; }


    }
}
