using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.MarginEntitiesWallet
{
    public class MarginCloseUserPositionWallet : BizBase
    {
        [Required]
        public long UserID { get; set; }

        [Required]
        public long WalletTypeID { get; set; }

        public string SMSCode { get; set; }

        public string TrnRefNo { get; set; }

        public long LoanID { get; set; }

        public long ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

    }

    public class DepositionRequired : BizBase
    {
        [Required]
        public long UserID { get; set; }

        [Required]
        public string SMSCode { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        public long LoanID { get; set; }

        public long WalletID { get; set; }

        public int TrnType { get; set; }

        public string TrnRefNo { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ReceivedDate { get; set; }

    }
}
