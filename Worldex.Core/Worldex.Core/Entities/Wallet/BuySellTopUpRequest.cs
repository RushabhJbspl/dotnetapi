using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Wallet
{
    public class BuySellTopUpRequest : BizBase
    {
        [Required]
        public string Guid { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal FromAmount { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal ToAmount { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal CoinRate { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal FiatConverationRate { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Fee { get; set; }
        [Required]
        public long UserId { get; set; }
        [Required]
        public string FromCurrency { get; set; }
        [Required]
        public string ToCurrency { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string TransactionHash { get; set; }
        [Required]
        public string NotifyUrl { get; set; }
        [Required]
        public string TransactionId { get; set; }
        [Required]
        public string TransactionCode { get; set; }
        [Required]
        public string UserGuid { get; set; }
        [Required]
        public string Platform { get; set; }
        [Required]
        public short Type { get; set; }//1-buy , 2-sell
        [Required]
        public long FromBankId { get; set; }
        [Required]
        public long ToBankId { get; set; }
        [Required]
        public string Code { get; set; }

        public string Remarks { get; set; }

        public string BankName { get; set; }
        public string CurrencyName { get; set; }//get from api response

        public string BankId { get; set; }
        public string CurrencyId { get; set; }//get from api response    
    }

    public class SellTopUpRequest : BizBase
    {
        [Required]
        public string Guid { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal FromAmount { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal ToAmount { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal CoinRate { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal FiatConverationRate { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Fee { get; set; }
        [Required]
        public long UserId { get; set; }
        [Required]
        public string FromCurrency { get; set; }
        [Required]
        public string ToCurrency { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string TransactionHash { get; set; }
        [Required]
        public string NotifyUrl { get; set; }
        [Required]
        public string TransactionId { get; set; }
        [Required]
        public string TransactionCode { get; set; }
        [Required]
        public string UserGuid { get; set; }
        [Required]
        public string Platform { get; set; }
        [Required]
        public short Type { get; set; }//1-buy , 2-sell
        [Required]
        public long FromBankId { get; set; }
        [Required]
        public long ToBankId { get; set; }
        [Required]
        public string Code { get; set; }

        public string Remarks { get; set; }

        public string BankName { get; set; }
        public string CurrencyName { get; set; }//get from api response

        public string BankId { get; set; }
        public string CurrencyId { get; set; }//get from api response   
        
        public string user_bank_name { get; set; }
        public string user_bank_account_number { get; set; }
        public string user_bank_acount_holder_name { get; set; }
        public string user_currency_code { get; set; }
        public string payus_transaction_id { get; set; }
        public decimal payus_amount_usd { get; set; }
        public decimal payus_amount_crypto { get; set; }
        public decimal payus_mining_fees { get; set; }
        public decimal payus_total_payable_amount { get; set; }
        public decimal payus_fees { get; set; }
        public decimal payus_total_fees { get; set; }
        public decimal payus_usd_rate { get; set; }
        public DateTime payus_expire_datetime { get; set; }
        public string payus_payment_tracking { get; set; }

        public long TrnNo { get; set; }
        public short APIStatus { get; set; }
    }
}
