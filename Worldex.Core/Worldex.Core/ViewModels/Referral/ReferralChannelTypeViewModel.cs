using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Worldex.Core.ApiModels;
namespace Worldex.Core.ViewModels.Referral
{
    public class ReferralChannelTypeViewModel
    {
        [Required(ErrorMessage = "1,Please enter required channel type name,32073")]
        [StringLength(50, ErrorMessage = "1,Please enter valid channel type name,32074")]
        public string ChannelTypeName { get; set; }
        public int HourlyLimit { get; set; }
        public int DailyLimit { get; set; }
        public int WeeklyLimit { get; set; }
        public int MonthlyLimit { get; set; }
    }
    public class ReferralLimitCount
    {
        public int HourlyCount { get; set; }
        public int DailyCount { get; set; }
        public int WeeklyCount { get; set; }
        public int MonthlyCount { get; set; }
    }
    public class ReferralChannelTypeUpdateViewModel
    {
        public long Id { get; set; }
        public string ChannelTypeName { get; set; }
        public int HourlyLimit { get; set; }
        public int DailyLimit { get; set; }
        public int WeeklyLimit { get; set; }
        public int MonthlyLimit { get; set; }
    }
    
    public class ReferralChannelTypeListViewModel
    {
        public long Id { get; set; }
        public string ChannelTypeName { get; set; }
        public int HourlyLimit { get; set; }
        public int DailyLimit { get; set; }
        public int WeeklyLimit { get; set; }
        public int MonthlyLimit { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
    }

    public class ReferralChannelTypeDropDownViewModel
    {
        public long Id { get; set; }
        public string ChannelTypeName { get; set; }       
    }

    public class ReferralChannelTypeStatusViewModel
    {
        public long Id { get; set; }        
    }

    public class ReferralChannelTypeResponse : BizResponseClass
    {
       
    }

    public class ReferralChannelTypeListResponse : BizResponseClass
    {
        public List<ReferralChannelTypeListViewModel> ReferralChannelTypeList { get; set; }      
    }
    public class ReferralChannelTypeDropDownResponse : BizResponseClass
    {        
        public List<ReferralChannelTypeDropDownViewModel> ReferralChannelTypeDropDownList { get; set; }
    }
    public class ReferralChannelTypeObjResponse : BizResponseClass
    {
        public ReferralChannelTypeUpdateViewModel ReferralChannelTypeObj { get; set; }
    }
    public class ReferralChannelTypeUpdateObjResponse : BizResponseClass
    {
        public ReferralChannelTypeUpdateViewModel ReferralChannelTypeUpdate { get; set; }
    }
    public class ReferralChannelTypeExistResponse : BizResponseClass
    {
        public bool ReferralChannelTypeExist { get; set; }
    }
    public class ReferralChannelTypeStatusResponse : BizResponseClass
    {
        public bool ReferralChannelTypeStatus { get; set; }
    }
}
