using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Worldex.Core.ApiModels;

namespace Worldex.Core.ViewModels.Referral
{
    public class ReferralUserViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4901")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4901")]
        public int ReferUserId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4901")]
        public int ReferralServiceId { get; set; }

        public int ReferralChannelTypeId { get; set; }

        public short IsCommissionCredited{ get; set; }
    }

    public class GetReferralUserViewModel
    {
        public int UserId { get; set; }
        public int ReferUserId { get; set; }
        public int ReferralServiceId { get; set; }
        public int ReferralChannelTypeId { get; set; }
        public short IsCommissionCredited { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ReferralUserListViewModel
    {
        public long Id { get; set;}
        public int UserId { get; set; }

        public string UserName { get; set; }
        public string UserReferralCode { get; set; }
           
        public int ReferUserId { get; set; }
        public string ReferUserName { get; set; }
        public string ReferUserReferralCode { get; set; }

        public int ReferralServiceId { get; set; }
        public string ReferralServiceDescription { get; set; }

        public int ReferralChannelTypeId { get; set; }
        public string ReferralChanneTypeName { get; set; }

        public short IsCommissionCredited { get; set; }
        public string IsCommissionCreditedName { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ReferralCountViewModel
    {
        public int count { get; set; }
    }

    public class ReferralCodeViewModel
    {
        public string ReferralCode { get; set; }
    }

    public class ReferralUserResponse : BizResponseClass
    {

    }
    public class ReferralUserCountResponse : BizResponseClass
    {
        public int ReferralUserCount { get; set; }
    }
    public class ReferralAdminCountResponse : BizResponseClass
    {
        public int ReferralAdminCount { get; set; }
    }

    public class UserReferralCodeResponse : BizResponseClass
    {
        public string UserReferralCode { get; set; }
    }
    public class ReferralUserListResponse : BizResponseClass
    {
        public List<ReferralUserListViewModel> ReferralUserList { get; set; }
        public int TotalCount { get; set; }
    }
}
