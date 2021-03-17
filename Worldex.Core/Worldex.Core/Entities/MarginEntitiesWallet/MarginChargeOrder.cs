using Worldex.Core.Enums;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.MarginEntitiesWallet
{
    public class MarginChargeOrder : BizBase
    {
        // order always sell order 
        public long BatchNo { get; set; }
        public long LoanID { get; set; }
        public long UserID { get; set; }
        public DateTime TrnDate { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }
        public long PairID { get; set; } 
        public MarginChargeCase MarginChargeCase { get; set; }
        public long TrnRefNo { get; set; } // TQ id fk 
        public string Guid { get; set; } // TQ Guid response from rita
        public string DebitAccountID { get; set; } // second currency 16 digit no
        public string CreditAccountID { get; set; }// base currency = leverage wallet 16 digit no
        public string SMSCode { get; set; }
        public string BaseCurrency { get; set; }
        [StringLength(250)]
        public string Remarks { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal LoanProfit { get; set; } //added for profit 
    }
}
