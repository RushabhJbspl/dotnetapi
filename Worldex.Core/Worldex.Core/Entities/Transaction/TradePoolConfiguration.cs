using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Transaction
{
    public class TradePoolConfiguration : BizBase
    {
        [Required]
        public long CountPerPrice { get; set; }
    }
}
