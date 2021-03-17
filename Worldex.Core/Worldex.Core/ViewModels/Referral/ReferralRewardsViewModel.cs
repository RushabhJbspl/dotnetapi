using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Referral
{
    public class ReferralRewardsViewModel
    {
        public long UserId { get; set; }
        public long ReferralServiceId { get; set; }
        public decimal ReferralPayRewards { get; set; }

        public long LifeTimeUserCount { get; set; }

        public long NewUserCount { get; set; }

        public long CommissionCroneID { get; set; }

        public long CommissionCurrecyId { get; set; }

        public long ReferralPayTypeId { get; set; }

        public decimal SumChargeAmount { get; set; }

        public long TransactionCurrencyId { get; set; }

        public long SumOfTransaction { get; set; }

        public long TrnUserId { get; set; }

        public long FromWalletId { get; set; }

        public long ToWalletId { get; set; }

        public long TrnRefNo { get; set; }

        public decimal CommissionAmount { get; set; }

        public decimal TransactionAmount { get; set; }

        public DateTime? TrnDate { get; set; }
    }

    public class ReferralRewardsListViewModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }

        public long ReferralServiceId { get; set; }
        public string ReferralServiceDescription { get; set; }

        public long ReferralPayTypeId { get; set; }
        public string ReferralPayTypeName { get; set; }

        public long CurrencyId { get; set; }
        public string CurrencyName { get; set; }

        public decimal ReferralPayRewards { get; set; }
        public DateTime CreatedDate { get; set; }

        public long LifeTimeUserCount { get; set; }

        public long NewUserCount { get; set; }

        public long CommissionCroneID { get; set; }
        public string CommissionCroneRemarks { get; set; }

        public long CommissionCurrecyId { get; set; }
        public string CommissionCurrecyName { get; set; }

        public decimal SumChargeAmount { get; set; }

        public long TransactionCurrencyId { get; set; }
        public string TransactionCurrecyName { get; set; }

        public long SumOfTransaction { get; set; }

        public long TrnUserId { get; set; }
        public string TrnUserName { get; set; }

        public long FromWalletId { get; set; }
        public string FromWalletName { get; set; }

        public long ToWalletId { get; set; }
        public string ToWalletName { get; set; }

        public long TrnRefNo { get; set; }

        public decimal CommissionAmount { get; set; }

        public decimal TransactionAmount { get; set; }

        public DateTime? TrnDate { get; set; }
    }

    public class ReferralRewardsResponse : BizResponseClass
    {

    }

    public class ReferralRewardsListResponse : BizResponseClass
    {
        public List<ReferralRewardsListViewModel> ReferralRewardsList { get; set; }
        public int TotalCount { get; set; }
    }
}
