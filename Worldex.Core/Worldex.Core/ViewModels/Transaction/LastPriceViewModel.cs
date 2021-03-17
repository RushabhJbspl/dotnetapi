using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class LastPriceViewModel
    {
        //for socket method
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal LastPrice { get; set; }
        public short UpDownBit { get; set; }
    }

    public class LastPriceViewModelArbitrage
    {
        //for socket method
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal LastPrice { get; set; }
        public short UpDownBit { get; set; }
        public string ExchangeName { get; set; }
        public short LPType { get; set; }
    }
}
