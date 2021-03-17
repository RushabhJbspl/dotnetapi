using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Wallet
{
    public class DestroyFundRequest : BizBase
    {
        [Required]
        public string Address { get; set; }
        [Required]
        public string Remarks { get; set; }

        public string TrnHash { get; set; }
    }
}
