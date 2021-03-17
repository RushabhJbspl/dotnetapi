using Worldex.Core.ViewModels.Profile_Management;
using System;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.BackOffice.ProfileConfiguration
{
    public interface IProfileConfigurationData
    {
        long AddProfileConfiguration(ProfileConfigurationAddReqViewModel profileConfiguration);
        string CreateJson(string Jsonname, string JsonData);
        long UpdateProfileConfiguration(ProfileConfigurationUpdateReqViewModel profileConfiguration);
        long DeleteProfileConfiguration(ProfileConfigurationDeleteReqViewModel profileConfiguration);
        long IsProfilelevelExistConfiguration(long ProfileLevel);
        ProfileConfigurationGetResponseViewmodel GetProfileConfiguration(int PageIndex = 0, int Page_Size = 0, long Typeid = 0, bool IsRecursive = true, DateTime? FromDate = null, DateTime? ToDate = null);
        List<ProfilelevelCountViewmodel> GetProfilelevelmaster();
        ProfilewiseuserlistResponseVoewmodel Profilewiseuserlist(long ProfileId, int PageIndex = 0, int Page_Size = 0);
        GetProfilelevelmasterResponse GetProfilelevelmaster(int PageIndex = 0, int Page_Size = 0);
        List<GetProfilelevelmaster> GetProfilelevelmasterDropDownList();
        ProfileConfigurationGetResponseByIdViewmodel GetProfileConfigurationById(long id);
    }
}
