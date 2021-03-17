using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.SocialProfile;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces.Configuration
{
    public interface IProfileConfigurationService
    {
        List<ProfileConfigurationModel> GetProfileConfiguration(string ConfigType = null);
        void SetLeaderProfileConfiguration(LeaderAdminPolicyModel model, string ConfigType = null, int UserId = 0);
        void SetFolowerProfileConfiguration(FollowerAdminModel model, string ConfigType = null, int UserId = 0);
        Task<LeaderAdminPolicyGetModel> GetLeaderProfileConfiguration();
        Task<FollowerAdminPolicyGetModel> GetFollowerProfileConfiguration();
        GetLeaderFrontPolicyModel GetUserLeaderProfileFrontConfiguration(int UserId = 0);
        void SetLeaderFrontProfileConfiguration(LeaderFrontPolicyModel model, int UserId = 0);
        FollowerServiceFrontModel GetFollowerProfileFrontConfiguration(int UserId = 0, string Leaderid = null);
        void SetFollowerFrontProfileConfiguration(FollowerFrontModel model, int UserId = 0);
        LeaderListWithGroupModel GetFrontLeaderList(int pageIndex = 0, int pageSize = 0, int UserId=0);
        LeaderwiseFollower GetLeaderWiseFollowerCongfigList(long LeaderId = 0);
        void Unfollowleader(long FollowerId = 0, string LeaderID = null);
        LeaderListModel GetLeaderListByFollowerId(int pageIndex = 0, int pageSize = 0, int userId = 0);
        LeaderwiseFollowersList GetLeaderWiseFollowers(long LeaderId = 0, int pageIndex = 0, int pageSize = 0);
        void UnSubscribeLeaderFrontProfileConfiguration(int UserId = 0, long ProfileId = 0);
        bool AddWatch(WatchMasterViewModel model, int UserId = 0);
        bool UnWatch(WatchMasterViewModel model, int UserId = 0);
        WatcherWiseLeaderResponse GetWatcherWiseLeaderList(int pageIndex = 0, int pageSize = 0, int UserId = 0, int GroupId = 0);
        GetLeaderFollowerCountModel GetLeaderFollowNuber(int LeaderId = 0);
        GetLeaderFrontPolicyModel GetUserLeaderProfileConfiguration(int UserId = 0);
    }
}
