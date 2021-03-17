using Worldex.Core.ApiModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Complaint
{
    public class ComplainmasterViewModel
    {

        [Required(ErrorMessage = "1,Please Enter complaint Subject,4118")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "1,Please Enter complaint Description,4117")]
        public string Description { get; set; }
        [Required(ErrorMessage = "1,Please Enter complaint Type,4119")]
        public int TypeId { get; set; }
        public long ComplaintPriorityId { get; set; }
    }

    public class ComplainmasterReqViewModel
    {

        [Required(ErrorMessage = "1,Please Enter complaint Subject,4118")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "1,Please Enter complaint Description,4117")]
        public string Description { get; set; }
        [Required(ErrorMessage = "1,Please Enter complaint Type,4119")]
        public int TypeId { get; set; }
        public int UserID { get; set; }
        public long ComplaintPriorityId { get; set; }
    }

    public class ComplainmasterResonse : BizResponseClass
    {
        public List<CompainDetailResponse> compainDetailResponses { get; set; }
        public List<UserWiseCompaintDetailResponce> userWiseCompaintDetailResponces { get; set; }
    }
    public class ComplainmasterDataResonse : BizResponseClass
    {
        public ComplainChildParentViewmodel ComplainViewmodel { get; set; }
    }
}
