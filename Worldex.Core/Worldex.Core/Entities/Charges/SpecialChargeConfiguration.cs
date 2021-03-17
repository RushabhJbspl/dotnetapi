using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Charges
{
    public class SpecialChargeConfiguration:BizBase
    {
        [Required]
        public DateTime TrnDate { get; set; }

        public string Remarks { get; set; }//(Diwali,Holi)
    }
}
