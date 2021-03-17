using Worldex.Core.ViewModels.Referral;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.Referral
{
    public interface IReferralPayType
    {
        long AddReferralPayType(ReferralPayTypeViewModel model, long UserID);
        long UpdateReferralPayType(ReferralPayTypeUpdateViewModel model, long UserID);
        ReferralPayTypeUpdateViewModel GetReferralPayType(long Id);
        bool IsReferralPayTypeExist(string PayTypeName);
        List<ReferralPayTypeListViewModel> ListReferralPayType();
        List<ReferralPayTypeDropDownViewModel> DropDownReferralPayType();
        bool DisableReferralPayType(ReferralPayTypeStatusViewModel model, long UserId);
        bool EnableReferralPayType(ReferralPayTypeStatusViewModel model, long UserId);
    }
}
