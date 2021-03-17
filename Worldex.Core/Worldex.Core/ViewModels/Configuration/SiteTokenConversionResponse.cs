using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Configuration
{
    public class SiteTokenConvertInfo 
    {
        public long UserID { get; set; }
        public long SourceCurrencyID { get; set; }
        public string SourceCurrency { get; set; }
        public long TargerCurrencyID { get; set; }
        public string TargerCurrency { get; set; }
        public decimal SourceCurrencyQty { get; set; }
        public decimal TargetCurrencyQty { get; set; }
        public decimal SourceToBasePrice { get; set; }
        public decimal SourceToBaseQty { get; set; }
        public decimal TokenPrice { get; set; }
        public DateTime TrnDate { get; set; }
    }

    public class SiteTokenConversionQueryRes
    {
        public long UserID { get; set; }
        public long SourceCurrencyID { get; set; }
        public string SourceCurrency { get; set; }
        public long TargerCurrencyID { get; set; }
        public string TargerCurrency { get; set; }
        public decimal SourceCurrencyQty { get; set; }
        public decimal TargerCurrencyQty { get; set; }
        public decimal SourceToBasePrice { get; set; }
        public decimal SourceToBaseQty { get; set; }
        public decimal TokenPrice { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class SiteTokenConvertFundResponse : BizResponseClass
    {
        public List<SiteTokenConvertInfo> Response { get; set; }
    }

    public class SiteTokenConvertFundRequest
    {
        public long? UserID { get; set; }
        public string SourceCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
