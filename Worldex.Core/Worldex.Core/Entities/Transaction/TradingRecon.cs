using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Transaction
{
    public class TradingRecon : BizBase
    {
        [Required]
        public long TrnNo { get; set; }
        [Required]
        public short OldStatus { get; set; }
        [Required]
        public short NewStatus { get; set; }
        [Required]
        public string Remarks { get; set; }
        [Required]
        public long StatusCode { get; set; }
        [Required]
        public string StatusMsg { get; set; }
    }
    public class TradingReconMargin : BizBase
    {
        [Required]
        public long TrnNo { get; set; }
        [Required]
        public short OldStatus { get; set; }
        [Required]
        public short NewStatus { get; set; }
        [Required]
        public string Remarks { get; set; }
        [Required]
        public long StatusCode { get; set; }
        [Required]
        public string StatusMsg { get; set; }
    }
    public class TradingReconArbitrage : BizBase
    {
        [Required]
        public long TrnNo { get; set; }
        [Required]
        public short OldStatus { get; set; }
        [Required]
        public short NewStatus { get; set; }
        [Required]
        public string Remarks { get; set; }
        [Required]
        public long StatusCode { get; set; }
        [Required]
        public string StatusMsg { get; set; }
    }
}