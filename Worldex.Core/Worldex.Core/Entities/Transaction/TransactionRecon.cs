using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Transaction
{
    public class TransactionRecon : BizBase
    {
        [Required]
        public long TrnNo { get; set; }
        [Required]
        public long ServiceID { get; set; }
        [Required]
        public long SerProID { get; set; }
        [Required]
        public short OldStatus { get; set; }
        [Required]
        public short NewStatus { get; set; }
        [Required]
        public string Remarks { get; set; }
    }
}