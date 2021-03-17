using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.AccountViewModels.Affiliate
{
    public class GetReferralLinkClickResponse : BizResponseClass
    {
        public List<GetReferralLinkClickData> Response { get; set; }
        public long TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public long TotalPages { get; set; }
    }

    public class GetReferralLinkClickData
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long AffiliateId { get; set; }
        public string IpAddress { get; set; }
        public DateTime ClickTime { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
