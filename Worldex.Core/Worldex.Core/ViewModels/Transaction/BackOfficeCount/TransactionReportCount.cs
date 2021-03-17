using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction.BackOfficeCount
{
    public class TransactionReportCountQryRes
    {
        public long SiteTokenConversionCount { get; set; }
        public long TradeRoutingCount { get; set; }
        public long TradeReconCount { get; set; }
        public long TotalCount
        {
            get { return SiteTokenConversionCount + TradeRoutingCount + TradeReconCount; }
            set { value= SiteTokenConversionCount + TradeRoutingCount + TradeReconCount; }
        }
    }
    public class TransactionReportCountResponse : BizResponseClass
    {
        public TransactionReportCountQryRes Response { get; set; }
    }
}
