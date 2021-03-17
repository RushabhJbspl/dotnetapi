using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.AccountViewModels.Affiliate
{
    public class GetAllAffiliateUserResponse : BizResponseClass
    {
        public List<GetAllAffiliateUserData> Response { get; set; }
    }

    public class GetAllAffiliateUserData
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime JoinDate { get; set; }
    }
}
