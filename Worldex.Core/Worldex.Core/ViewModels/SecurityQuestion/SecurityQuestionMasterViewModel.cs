using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.SecurityQuestion
{
    public class SecurityQuestionMasterViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter securityquestion,8010")]
        public string SecurityQuestion { get; set; }
        [Required(ErrorMessage = "1,Please Enter Answer,8011")]
        public string Answer { get; set; }

    }
    public class SecurityQuestionMasterReqViewModel
    {

        public int Userid { get; set; }

        public string SecurityQuestion { get; set; }

        public string Answer { get; set; }

    }
    public class SecurityQuestionMasterResponse : BizResponseClass
    {

    }
}
