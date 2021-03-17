using System;
using System.Collections.Generic;
using Worldex.Core.ApiModels;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Referral
{
    public class ReferralServiceViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4901")]
        public int ReferralServiceTypeId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4901")]
        public int ReferralPayTypeId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4901")]
        public long CurrencyId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4901")]
        public string Description { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4901")]
        public int ReferMinCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4901")]
        public int ReferMaxCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4901")]
        public decimal RewardsPay { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4901")]
        [DataType(DataType.DateTime)]
        public DateTime ActiveDate { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4901")]
        [DataType(DataType.DateTime)]
        public DateTime ExpireDate { get; set; }
    }

    public class ReferralServiceResponse : BizResponseClass
    {

    }   

    public class ReferralServiceUpdateViewModel
    {
        public long Id { get; set; }
        public int ReferralServiceTypeId { get; set; }
        public int ReferralPayTypeId { get; set; }
        public long CurrencyId { get; set; }
        public string Description { get; set; }
        public int ReferMinCount { get; set; }
        public int ReferMaxCount { get; set; }
        public decimal RewardsPay { get; set; }
        public DateTime ActiveDate { get; set; }
        public DateTime ExpireDate { get; set; }      
    }

    public class ReferralServiceListViewModel
    {
        public long Id { get; set; }
        public int ReferralServiceTypeId { get; set; }
        public string ReferralServiceTypeName { get; set; }
        public int ReferralPayTypeId { get; set; }
        public string ReferralPayTypeName { get; set; }
        public long CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string Description { get; set; }
        public int ReferMinCount { get; set; }
        public int ReferMaxCount { get; set; }
        public decimal RewardsPay { get; set; }
        public DateTime ActiveDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
    }     

    public class ReferralServiceStatusViewModel
    {
        public long Id { get; set; }
    }
   
    public class ReferralServiceListResponse : BizResponseClass
    {
        public List<ReferralServiceListViewModel> ReferralServiceList { get; set; }
        public int TotalCount { get; set; }
    }

    public class ReferralServiceDropDownViewModel
    {
        public long Id { get; set; }
        public string ServiceSlab { get; set; }
    }

    public class ReferralServiceDropDownResponse : BizResponseClass
    {
        public List<ReferralServiceDropDownViewModel> ReferralServiceDropDownList { get; set; }
    }
    public class ReferralServiceObjResponse : BizResponseClass
    {
        public ReferralServiceUpdateViewModel ReferralServiceObj { get; set; }
    }

    public class ReferralServiceUpdateObjResponse : BizResponseClass
    {
        public ReferralServiceUpdateViewModel ReferralServiceUpdate { get; set; }
    }

    public class ReferralServiceExistResponse : BizResponseClass
    {
        public bool ReferralServiceExist { get; set; }
    }

    public class ReferralServiceStatusResponse : BizResponseClass
    {
        public bool ReferralServiceStatus { get; set; }
    }
}
