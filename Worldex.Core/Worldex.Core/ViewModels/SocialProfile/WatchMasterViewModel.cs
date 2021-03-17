using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.SocialProfile
{
    public class WatchMasterViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please enter group id,12043")]
        public string GroupId { get; set; }

        [Required(ErrorMessage = "1,Please enter leader id,12044")]
        public string LeaderId { get; set; }
    }

    public class WatchMasterResponse : BizResponseClass
    {

    }
}
