using System;
using System.Collections.Generic;
using Worldex.Core.ApiModels;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Referral
{
   public class ReferralPayTypeViewModel
    {
        [Required(ErrorMessage = "1,Please enter required pay type name ,32071")]
        [StringLength(50, ErrorMessage = "1,Please enter valid pay type name ,32072")]
        public string PayTypeName { get; set; }          
    }

    public class ReferralPayTypeUpdateViewModel
    {
        public long Id { get; set; }
        public string PayTypeName { get; set; }
    }

    public class ReferralPayTypeListViewModel
    {
        public long Id { get; set; }
        public string PayTypeName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
    }
    public class ReferralPayTypeDropDownViewModel
    {
        public long Id { get; set; }
        public string PayTypeName { get; set; }
       
    }
    public class ReferralPayTypeStatusViewModel
    {
        public long Id { get; set; }       
    }

    public class ReferralPayTypeResponse : BizResponseClass
    {

    }
    public class ReferralPayTypeListResponse : BizResponseClass
    {      
        public List<ReferralPayTypeListViewModel> ReferralPayTypeList { get; set; }        
    }

    public class ReferralPayTypeDropDownResponse : BizResponseClass
    {       
        public List<ReferralPayTypeDropDownViewModel> ReferralPayTypeDropDownList { get; set; }
    }

    public class ReferralPayTypeObjResponse : BizResponseClass
    {       
        public ReferralPayTypeUpdateViewModel ReferralPayTypeObj { get; set; }
    }

    public class ReferralPayTypeUpdateObjResponse : BizResponseClass
    {
        public ReferralPayTypeUpdateViewModel ReferralPayTypeUpdate { get; set; }
    }

    public class ReferralPayTypeExistResponse : BizResponseClass
    {
        public bool ReferralPayTypeExist { get; set; }
    }
    public class ReferralPayTypeStatusResponse : BizResponseClass
    {
        public bool ReferralPayTypeStatus { get; set; }
    }
   
}
