using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public  class GetRecentTradeRequest
    {
        [Required]
        [StringLength(7,MinimumLength =6)]
        public string Pair { get; set; }

        public long Limit { get; set; }
    }
}
