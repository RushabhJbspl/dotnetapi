using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Referral
{
    public class ReferralUserClickViewModel
    {
        public string ReferralCode { get; set; }
        public long ReferralServiceId { get; set; }
        public long ReferralChannelTypeId { get; set; }
        public string IPAddress { get; set; }
    }

    public class ReferralUserClickListViewModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }

        public string UserName { get; set; }
        public string UserReferralCode { get; set; }

        public long ReferralServiceId { get; set; }
        public string ReferralServiceDescription { get; set; }

        public long ReferralChannelTypeId { get; set; }
        public string ReferralChanneTypeName { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public class ReferralUserClickResponse : BizResponseClass
    {
        public string ReferralCode { get; set; }
        public long ReferralServiceId { get; set; }
        public long ReferralChannelTypeId { get; set; }
    }
    public class ReferralUserClickListResponse : BizResponseClass
    {
        public List<ReferralUserClickListViewModel> ReferralUserClickList { get; set; }
        public int TotalCount { get; set; }
    }
}
