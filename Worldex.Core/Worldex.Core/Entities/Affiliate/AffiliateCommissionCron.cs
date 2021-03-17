using Worldex.Core.SharedKernel;
using System;

namespace Worldex.Core.Entities.Affiliate
{
    public class AffiliateCommissionCron : BizBase
    {
        public long SchemeMappingId { get; set; }
        public string Remarks { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
