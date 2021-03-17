using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.SocialProfile
{
    public class GroupMasterViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please enter group name,12040")]
        [StringLength(200, ErrorMessage = "1,Please enter valid group name,12040")]        
        public string GroupName { get; set; }
    }

    public class GroupMasterAddModel
    {
        public int UserId { get; set; }
        [StringLength(200)]
        public string GroupName { get; set; }
    }
    public class GroupMasterModel 
    {
        public long Id { get; set; }
        public string GroupName { get; set; }
    }

    public class GroupMasterResponse : BizResponseClass
    {
    }

    public class GroupListResponse : BizResponseClass
    {
        public List<GroupMasterModel> GroupList { get; set; }
    }
}
