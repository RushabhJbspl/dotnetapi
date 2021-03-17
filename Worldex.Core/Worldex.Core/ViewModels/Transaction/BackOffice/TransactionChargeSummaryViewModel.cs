using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction.BackOffice
{
    public class TrnChargeSummaryViewModel
    {
        public long TrnNo { get; set; }
        public string TrnTypeName { get; set; }
        public DateTime TrnDate { get; set; }
        public string PairName { get; set; }
        public decimal Amount { get; set; }
        public decimal ChargePer { get; set; }
        public decimal ChargeRs { get; set; }
        public string ChargeType { get; set; }
    }
    public class TransactionChargeResponse : BizResponseClass
    {
        public List<TrnChargeSummaryViewModel> response { get; set; }
    }
    public class TransactionChargeRequest
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Trade { get; set; }
    }
}
