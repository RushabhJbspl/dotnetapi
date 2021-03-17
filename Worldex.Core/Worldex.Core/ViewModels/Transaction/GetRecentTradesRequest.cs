using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class GetRecentTradesRequest
    {
        [Required]
        public string pair_name { get; set; }
        public long timestamp { get; set; }
    }
}
