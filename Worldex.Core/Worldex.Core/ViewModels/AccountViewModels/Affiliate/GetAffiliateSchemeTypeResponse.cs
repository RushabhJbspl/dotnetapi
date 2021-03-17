using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.AccountViewModels.Affiliate
{
    public class GetAffiliateSchemeTypeResponse : BizResponseClass
    {
        public List<AffiliateSchemeTypeResponseData> Response { get; set; }
    }

    public class AffiliateSchemeTypeResponseData
    {
        public long Id { get; set; }
        public string Value { get; set; }
    }
}
