using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Referral
{
    public class ReferralRewards : BizBase
    {
        public long UserId { get; set; }

        public long ReferralServiceId { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ReferralPayRewards { get; set; }

        public long LifeTimeUserCount { get; set; }

        public long NewUserCount { get; set; }

        public long CommissionCroneID { get; set; }

        public long CommissionCurrecyId { get; set; }

        public long ReferralPayTypeId { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal SumChargeAmount { get; set; }

        public long TransactionCurrencyId { get; set; }

        public long SumOfTransaction { get; set; }

        public long TrnUserId { get; set; }

        public long FromWalletId { get; set; }

        public long ToWalletId { get; set; }

        public long TrnRefNo { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal CommissionAmount { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal TransactionAmount { get; set; }

        public DateTime? TrnDate { get; set; }
    }
}
