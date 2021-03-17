using System;
using Worldex.Core.ViewModels.Referral;

namespace Worldex.Core.Interfaces.Referral
{
    public interface IReferralRewards
    {
        long AddReferralRewards(ReferralRewardsViewModel model, long UserID);
        ReferralRewardsListResponse ListAdminReferralRewards(int PageIndex = 0, int Page_Size = 0,  long ReferralServiceId = 0, int UserId = 0,int TrnUserId=0, DateTime? FromDate = null, DateTime? ToDate = null);
        ReferralRewardsListResponse ListUserReferralRewards(int UserId, int PageIndex = 0, int Page_Size = 0,  long ReferralServiceId = 0, int TrnUserId = 0,DateTime? FromDate = null, DateTime? ToDate = null);
    }
}
