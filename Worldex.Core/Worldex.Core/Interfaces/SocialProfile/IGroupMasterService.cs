using Worldex.Core.ViewModels.SocialProfile;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.SocialProfile
{
    public interface IGroupMasterService
    {
        bool AddGroup(GroupMasterViewModel model, int UserId = 0);
        GroupMasterModel GetGroupbyUserId(long UserId = 0);
        GroupMasterModel GetGroupbyName(string GroupName = null);
        List<GroupMasterModel> GetGroupListByUserId(long UserId = 0);
    }
}
