using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.NewWallet
{
    public class StatasticsDetail : BizBase
    {
        [Required]
        public long StatasticsId { get; set; }

        [Required]
        [StringLength(50)]
        public string EntityName { get; set; }

        [Required]
        public string TrnNo { get; set; }

        [Required]
        public long Type { get; set; } //1-credit, 2-debit

        [Required]
        public decimal Amount { get; set; }

        public DateTime? USDLastUpdateDateTime { get; set; }

        public decimal? USDAmount { get; set; }
    }
}
