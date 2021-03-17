using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.MobileMaster
{
    public class PhoneMasterViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter mobilenumber,8005")]
        public string MobileNumber { get; set; }
        public bool IsPrimary { get; set; }
        [Display(Name = "CountryCode")]
        [StringLength(5, ErrorMessage = "1,Please enter a valid Contry Code,4131")]
        public string CountryCode { get; set; }
    }

    public class PhoneMasterReqViewModel
    {
        public int Userid { get; set; }
        public string MobileNumber { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class PhoneMasterResponse : BizResponseClass
    {

    }
    public class PhoneMasterUpdateViewModel  : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Id,8002")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "1,Please Enter mobilenumber,8005")]
        public string MobileNumber { get; set; }
        public bool IsPrimary { get; set; }
        [Display(Name = "CountryCode")]
        [StringLength(5, ErrorMessage = "1,Please enter a valid Contry Code,4131")]
        public string CountryCode { get; set; }
    }
    public class PhoneMasterUpdateReqViewModel
    {

        public Guid Id { get; set; }
        public int Userid { get; set; }
        public string MobileNumber { get; set; }
        public bool IsPrimary { get; set; }
    }


    public class MobileNumebrListViewModel
    {
        public string MobileNumber { get; set; }

    }

    public class PhoneMasterDeleteViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Id,8002")]
        public Guid Id { get; set; }
    }

  
    public class PhoneListResponse : BizResponseClass
    {
        public List<MobileNumebrListViewModel> PhonenumberListViewModels { get; set; }
    }
}
