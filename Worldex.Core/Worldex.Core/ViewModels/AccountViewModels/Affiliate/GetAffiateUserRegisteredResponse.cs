using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.AccountViewModels.Affiliate
{
    public class GetAffiateUserRegisteredResponse : BizResponseClass
    {
        public List<GetAffiateUserRegisteredData> Response { get; set; }
        public long TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public long TotalPages { get; set; }
    }

    public class GetAffiateUserRegisteredData
    {
        public long UserId { get; set; }
        public long AffiliateId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime JoinDate { get; set; }
        public short Status { get; set; }
        public string StatusMsg { get; set; }
        public long ParentId { get; set; }
        public string ParentUserName { get; set; }
        public string ParentEmail { get; set; }
        public string ParentMobile { get; set; }
        public string SchemeType { get; set; }
    }
}
