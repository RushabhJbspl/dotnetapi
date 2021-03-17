using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.AccountViewModels.Affiliate
{
    public class GetDetailAffiliateSchemeTypeResponse : BizResponseClass
    {
        public List<AffiliateSchemeAvailableSchemeData> Response { get; set; }
    }

    public class AffiliateSchemeAvailableSchemeData
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public List<AffiliateSchemeDetailData> AvailableScheme { get; set; }
    }

    public class AffiliateSchemeDetailData
    {
        public string SchemeName { get; set; }
        public List<String> SchemeDetail = new List<String>();
    }

    public class GetAffiliateSchemePlan
    {
        public long Id { get; set; }
        public string PlanType { get; set; }
        public string SchemeName { get; set; }
        public string SchemeDetailName { get; set; }
        public string PlanDetail { get; set; }
        public string ExtraDetail { get; set; }
    }
}
