using Worldex.Core.ViewModels.Referral;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.Referral
{
    public interface IReferralChannelType
    {
        long AddReferralChannelType(ReferralChannelTypeViewModel model, long UserID);
        long UpdateReferralChannelType(ReferralChannelTypeUpdateViewModel model, long UserID);
        ReferralChannelTypeUpdateViewModel GetReferralChannelType(long Id);
        bool IsReferralChannelTypeExist(string ChannelTypeName);
        List<ReferralChannelTypeListViewModel> ListReferralChannelType();
        List<ReferralChannelTypeDropDownViewModel> DropDownReferralChannelType();
        bool DisableReferralChannelType(ReferralChannelTypeStatusViewModel model, long UserId);
        bool EnableReferralChannelType(ReferralChannelTypeStatusViewModel model, long UserId);
        bool IsReferralChannelTypeExistById(int ChannelTypeId);
    }
}
