using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Configuration.FeedConfiguration
{
    public class APIPlanMaster : BizBase
    {
        [Required]
        [StringLength(50)]
        public string PlanName { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Price { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Charge { get; set; }
        public string PlanDesc { get; set; }
        public int Priority { get; set; }
        public long MaxPerMinute { get; set; }
        public long MaxPerDay { get; set; }
        public long MaxPerMonth { get; set; }
        public long MaxOrderPerSec { get; set; }
        public long MaxRecPerRequest { get; set; }
        public long MaxReqSize { get; set; }
        public long MaxResSize { get; set; }
        public int WhitelistedEndPoints { get; set; }
        public int ConcurrentEndPoints { get; set; }
        public int HistoricalDataMonth { get; set; }
        public short IsPlanRecursive { get; set; }
        public int PlanValidity { get; set; }
        public int PlanValidityType { get; set; }
        public string CreatedIPAddress { get; set; }
        [StringLength(6)]
        public string Currency { get; set; }
        public long ServiceID { get; set; }
    }
    public class APIPlanConfigurationHistory : BizBase
    {
        public long PlanID { get; set; }
        [Required]
        [StringLength(50)]
        public string PlanName { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Price { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Charge { get; set; }
        public string PlanDesc { get; set; }
        public int Priority { get; set; }
        public long MaxPerMinute { get; set; }
        public long MaxPerDay { get; set; }
        public long MaxPerMonth { get; set; }
        public long MaxOrderPerSec { get; set; }
        public long MaxRecPerRequest { get; set; }
        public long MaxReqSize { get; set; }
        public long MaxResSize { get; set; }
        public int WhitelistedEndPoints { get; set; }
        public int ConcurrentEndPoints { get; set; }
        public int HistoricalDataMonth { get; set; }
        public short IsPlanRecursive { get; set; }
        public int PlanValidity { get; set; }
        public int PlanValidityType { get; set; }
        public string CreatedIPAddress { get; set; }
        public long LastModifyBy { get; set; }
        public DateTime LastModifyDate { get; set; }
        public string ModifyDetails { get; set; }
        public string Currency { get; set; }
        public long ServiceID { get; set; }
    }
}
