using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Wallet
{
    public class FiatTradeConfigurationMaster : BizBase
    {
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal BuyFee { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SellFee { get; set; }

        [Required]
        public string TermsAndCondition { get; set; }

        [Required]
        public short IsBuyEnable { get; set; }//1 -enable 0 -disable

        [Required]
        public short IsSellEnable { get; set; }//1 -enable 0 -disable

        [Required]
        public short BuyFeeType { get; set; }//1-fixed 2 Percetage

        [Required]
        public short SellFeeType { get; set; }//1-fixed 2 Percetage

        [StringLength(250)]
        public string BuyNotifyURL { get; set; }

        [StringLength(250)]
        public string SellNotifyURL { get; set; }

        [StringLength(250)]
        public string CallBackURL { get; set; }

        [StringLength(50)]
        public string EncryptionKey { get; set; }

        [StringLength(250)]
        public string SellCallBackURL { get; set; }

        [StringLength(250)]
        public string WithdrawURL { get; set; }

        [StringLength(250)]
        public string Platform { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal FiatCurrencyRate { get; set; }

        [Required]
        [StringLength(50)]
        public string FiatCurrencyName { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinLimit { get; set; }//now used another table for validatiom

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MaxLimit { get; set; }//now used another table for validatiom
    }
}
