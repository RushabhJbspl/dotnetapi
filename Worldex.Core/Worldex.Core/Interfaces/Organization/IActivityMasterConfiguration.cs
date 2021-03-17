using Worldex.Core.Entities.Complaint;
using Worldex.Core.Entities.Organization;
using Worldex.Core.Entities.Profile_Management;
using Worldex.Core.Entities.User;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.Organization
{
    public interface IActivityMasterConfiguration
    {
        List<Typemaster> GetTypeMasterData();
        List<HostURLMaster> GetHostURLData();
        List<ActivityType_Master> GetActivityTypeData();
        List<ApplicationUser> GetAlluserData();
        List<ComplainStatusTypeMaster> GetComplainStatus();
        List<ApplicationMaster> GetMasterApplicationData();
        List<ProfileMaster> GetMasterProfileData();
        List<ProfileLevelMaster> GetMasterProfileLevelData();
        List<SubscriptionMaster> GetUserSubscription();
    }
}
