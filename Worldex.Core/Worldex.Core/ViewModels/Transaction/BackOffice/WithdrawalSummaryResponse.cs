using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction.BackOffice
{
    public class WithdrawalSummaryResponse : BizResponseClass
    {
        public List<WithdrawalSummaryViewModel> response { get; set; }
    }
    public class WithdrawalSummaryViewModel
    {
        public long TrnNo { get; set; }
        public long MemberID { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }
        public DateTime TrnDate { get; set; }
        public string StatusText { get; set; }
        public string ServiceName { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ChargeRs { get; set; }
        public string DestAddress { get; set; }
        public string DebitAccountId { get; set; }
    }
}
