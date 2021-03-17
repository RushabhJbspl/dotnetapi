using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.EmailMaster
{
    public class EmailMasterViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Email Id,4007")]
        [StringLength(50, ErrorMessage = "1,Please Enter Valid Email Id,4008")]
        [RegularExpression(@"^[-a-zA-Z0-9~!$%^&*_=+}{\'?]+(\.[-a-zA-Z0-9~!$%^&*_=+}{\'?]+)*@([a-zA-Z0-9_][-a-zA-Z0-9_]*(\.[-a-zA-Z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-zA-Z]{2,3})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$", ErrorMessage = "1,Please enter a valid Email Address,4009")]

        public string Email { get; set; }
        public bool IsPrimary { get; set; }

    }
    public class EmailMasterReqViewModel
    {
        public int Userid { get; set; }
        [Required(ErrorMessage = "1,Please Enter Email Id,4007")]
        [StringLength(50, ErrorMessage = "1,Please Enter Valid Email Id,4008")]
        [RegularExpression(@"^[-a-zA-Z0-9~!$%^&*_=+}{\'?]+(\.[-a-zA-Z0-9~!$%^&*_=+}{\'?]+)*@([a-zA-Z0-9_][-a-zA-Z0-9_]*(\.[-a-zA-Z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-zA-Z]{2,3})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$", ErrorMessage = "1,Please enter a valid Email Address,4009")]

        public string Email { get; set; }
        public bool IsPrimary { get; set; }

    }
    public class EmailMasterUpdateViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Id,8002")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "1,Please Enter Email Id,4007")]
        [StringLength(50, ErrorMessage = "1,Please Enter Valid Email Id,4008")]
        [RegularExpression(@"^[-a-zA-Z0-9~!$%^&*_=+}{\'?]+(\.[-a-zA-Z0-9~!$%^&*_=+}{\'?]+)*@([a-zA-Z0-9_][-a-zA-Z0-9_]*(\.[-a-zA-Z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-zA-Z]{2,3})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$", ErrorMessage = "1,Please enter a valid Email Address,4009")]

        public string Email { get; set; }
        public bool IsPrimary { get; set; }

    }
    public class EmailMasterUpdateReqViewModel
    {
        public int Userid { get; set; }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public bool IsPrimary { get; set; }
    }
    public class EmailMasterDeleteViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Id,8002")]
        public Guid Id { get; set; }
    }
    public class EmailMasterListViewModel
    {
        [Required(ErrorMessage = "1,Please Enter UserID,8004")]
        public int Userid { get; set; }
    }
    public class EmailMasterResponse : BizResponseClass
    {

    }
    public class EmailListResponse : BizResponseClass
    {
        public List<EmailListViewModel> emailListViewModels { get; set; }
    }
    public class EmailListViewModel
    {
        public string Email { get; set; }

    }

}
