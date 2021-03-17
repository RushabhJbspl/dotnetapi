using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Charges
{
    public class ChargeFreeMarketCurrencyMaster : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Required]
        [StringLength(7)]
        [Key]
        public string MarketCurrency { get; set; }
    }
}
