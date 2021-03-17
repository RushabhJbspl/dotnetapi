using Worldex.Core.Enums;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.NewWallet
{
    public class MarginLoanRequest : BizBase
    {
        [Required]
        public long WalletTypeID { get; set; }

        public  string Guid { get; set; }

        [Required]
        public long FromWalletID { get; set; }

        [Required]
        public long ToWalletID { get; set; }

        [Required]
        public long UserID { get; set; }

        [Required]
        public long LeverageID { get; set; }

        [Required]
        public short IsAutoApprove { get; set; }

        [Required]
        [StringLength(500)]
        public string RequestRemarks { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        public DateTime TrnDate { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal LeverageAmount { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ChargeAmount { get; set; } //instant charge

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal SafetyMarginAmount { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal CreditAmount { get; set; }
        
        public long? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }
                
        [StringLength(500)]
        public string ApprovedRemarks { get; set; }
                
        [StringLength(500)]
        public string SystemRemarks { get; set; } //Additional remarks if needed for internal purpose

        public new enMarginLoanStatus Status { get; set; }

        public short IsChargeDeducted { get; set; }//1=decuct , 0=not

        //ntrivedi 26-03-2019
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]        
        public decimal Leverage { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MaxLeverage { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal TotalSafetyCharge { get; set; } //every time need to be sum up //not in use this column

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal DailyChargePer { get; set; } //everyday charge

        public DateTime? LastChargeCalculated { get; set; }

        [Required]
        public long SafetyWalletID { get; set; }

        [Required]
        public long UserLoanMasterID { get; set; }

        [Required]
        public long? UpgradeLoanID { get; set; }//self kd

    }

    public class UserLoanMaster : BizBase
    {
        public string Guid { get; set; }

        [Required]
        public string SMSCode { get; set; }

        [Required]
        public long WalletTypeID { get; set; }

        [Required]
        public long ToWalletID { get; set; } //margintradingwallet

        [Required]
        public long UserID { get; set; }       

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; } // trading wallet deducted amount

        public DateTime TrnDate { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal LeverageAmount { get; set; } // 10 * 3X = 30 is LeverageAmount "Total"

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal SafetyMarginAmount { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal CreditAmount { get; set; }// "Total" credit to leveragewallet


        [StringLength(500)]
        public string SystemRemarks { get; set; } //Additional remarks if needed for internal purpose

        public new enMarginLoanStatus Status { get; set; }

        //ntrivedi 26-03-2019
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Leverage { get; set; } // "Average" leverage X

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MaxLeverage { get; set; } // "Average" maxleverage X

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal TotalSafetyCharge { get; set; } //every time need to be sum up 

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal DailyChargePer { get; set; } //everyday charge  "Average"

        public DateTime? LastChargeCalculated { get; set; }

        [Required]
        public long SafetyWalletID { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ProfitAmount { get; set; }

    }

    public class WithdrawLoanMaster : BizBase
    {
        public string Guid { get; set; }

        [Required]
        public string LoanID { get; set; } //UserLoanMasterID

        [Required]
        public string SMSCode { get; set; }

        [Required]
        public long WalletTypeID { get; set; }

        [Required]
        public long ToWalletID { get; set; } //non-margin wallet

        [Required]
        public long SafetyWalletID { get; set; }

        [Required]
        public long ProfitWalletID { get; set; }

        [Required]
        public long UserID { get; set; }

        public DateTime TrnDate { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal SafetyMarginAmount { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ProfitMarginAmount { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal CreditAmount { get; set; }// "Total" credit to leveragewallet

        [StringLength(500)]
        public string SystemRemarks { get; set; } //Additional remarks if needed for internal purpose

        [StringLength(100)]
        public string StatusMessage { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ChargeAmount { get; set; }

        public int? ErrorCode { get; set; }
    }
}
