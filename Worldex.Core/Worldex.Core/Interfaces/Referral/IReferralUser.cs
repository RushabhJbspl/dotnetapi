using System;
using System.Threading.Tasks;
using Worldex.Core.ViewModels.Referral;
using Microsoft.AspNetCore.Identity;

namespace Worldex.Core.Interfaces.Referral
{
   public interface IReferralUser
    {
        string GenerateRandomReferralCode(PasswordOptions opts = null);        
        bool FindDuplicateReferralCode(string ReferralCode);
        long AddReferralUser(ReferralUserViewModel model);
        Task<int> GetReferCodeUser(string ReferralCode);       
        Task<int> GetUserReferralCount(int UserId);
        Task<int> GetAdminReferralCount();
        Task<string> GetUserReferralCode(int UserId);
        ReferralUserListResponse ListAdminParticipateReferralUser(int PageIndex = 0, int Page_Size = 0, long ReferralServiceId = 0, string UserName = null, string ReferUserName = null, DateTime? FromDate = null, DateTime? ToDate = null,int ReferralChannelTypeId=0);
        ReferralUserListResponse ListUserParticipateReferralUser(int UserId, int PageIndex = 0, int Page_Size = 0, long ReferralServiceId = 0, string ReferUserName = null, DateTime? FromDate = null, DateTime? ToDate = null, int ReferralChannelTypeId = 0);
    }
}
