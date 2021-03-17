using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Charges
{
    public class MarketCapCounterMaster : BizBase
    {
        [Required]
        public long RecordCountLimit { get; set; }

        [Required]
        public long MaxLimit { get; set; }

        public long WalletTypeID { get; set; }

        public string CurrencyName { get; set; }

        [Required]
        public long TPSPickupStatus { get; set; }

        [Required]
        public long StartLimit { get; set; }

        [Required]
        public string Url { get; set; }
    }
}
