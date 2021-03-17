using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Worldex.Core.ApiModels;

namespace Worldex.Core.ViewModels.Referral
{
   public class ReferralServiceTypeViewModel
    {
        [Required(ErrorMessage = "1,Please enter required service type name.,32069")]
        [StringLength(50,ErrorMessage = "1,Please enter valid service type name.,32070")]
        public string ServiceTypeName { get; set; }
    }

    public class ReferralServiceTypeUpdateViewModel
    {
        public long Id { get; set; }
        public string ServiceTypeName { get; set; }
    }

    public class ReferralServiceTypeListViewModel
    {
        public long Id { get; set; }
        public string ServiceTypeName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
    }


    public class ReferralServiceTypeDropDownViewModel
    {
        public long Id { get; set; }
        public string ServiceTypeName { get; set; }     
    }


    public class ReferralServiceTypeStatusViewModel
    {
        public long Id { get; set; }
    }
    
    public class ReferralServiceTypeResponse : BizResponseClass
    {
       
    }

    public class ReferralServiceTypeListResponse : BizResponseClass
    {
        public List<ReferralServiceTypeListViewModel> ReferralServiceTypeList { get; set; }        
    }
    
    public class ReferralServiceTypeDropDownResponse : BizResponseClass
    {        
        public List<ReferralServiceTypeDropDownViewModel> ReferralServiceTypeDropDownList { get; set; }
    }

    public class ReferralServiceTypeObjResponse : BizResponseClass
    {
        public ReferralServiceTypeUpdateViewModel ReferralServiceTypeObj { get; set; }
    }

    public class ReferralServiceTypeUpdateObjResponse : BizResponseClass
    {
        public ReferralServiceTypeUpdateViewModel ReferralServiceTypeUpdate { get; set; }
    }

    public class ReferralServiceTypeExistResponse : BizResponseClass
    {
        public bool ReferralServiceTypeExist { get; set; }
    }

    public class ReferralServiceTypeStatusResponse : BizResponseClass
    {
        public bool ReferralServiceTypeStatus { get; set; }
    }
}
