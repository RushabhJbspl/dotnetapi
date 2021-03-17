using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class SiteTokenTypeResponse : BizResponseClass
    {
        public List<SiteTokenTypeInfo> Response { get; set; }
    }
    public class SiteTokenTypeInfo
    {
        public long Id { get; set; }
        public string SiteTokenType { get; set; }
        public short Status { get; set; }
    }
    public class SiteTokenMasterInfo
    {
        public long? ID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4520")]
        public long CurrencyID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,11036")]
        public long BaseCurrencyID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,11038")]
        public string CurrencySMSCode { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,11039")]
        public string BaseCurrencySMSCode { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,1137")]
        public short RateType { get; set; }

        public Decimal Rate { get; set; }
        public Decimal MinLimit { get; set; }
        public Decimal MaxLimit { get; set; }
        public Decimal DailyLimit { get; set; }
        public Decimal WeeklyLimit { get; set; }
        public Decimal MonthlyLimit { get; set; }
        public string Note { get; set; }
        public short Status { get; set; }
    }
    public class SiteTokenMasterRequest : SiteTokenMasterInfo
    {
        public short IsMargin { get; set; } = 0;//Rita 16-4-19,   1-for Margin trading -used for convert Qty Base to Second currency
    }
    public class SiteTokenMasterResponse : BizResponseClass
    {
        public List<SiteTokenMasterInfo> Response { get; set; }
    }
}
