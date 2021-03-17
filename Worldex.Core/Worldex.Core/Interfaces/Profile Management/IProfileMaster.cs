using Worldex.Core.ViewModels.Profile_Management;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.Profile_Management
{
    public interface IProfileMaster
    {
        List<ProfileMasterData> GetProfileData(int userid);
        List<SocialProfileModel> GetSocialProfileData(int userid = 0);
        bool GetSocialProfile(int ProfileId = 0);
        GetProfileDataResponse GetAllUserProfileData(int UserId =0,int PageIndex = 0, int Page_Size = 0);
    }
}
