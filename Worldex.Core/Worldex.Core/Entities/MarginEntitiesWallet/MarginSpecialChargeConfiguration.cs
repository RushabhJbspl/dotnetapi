using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.MarginEntitiesWallet
{
    public class MarginSpecialChargeConfiguration : BizBase
    {
        [Required]
        public DateTime TrnDate { get; set; }

        public string Remarks { get; set; }//(Diwali,Holi)
    }
}
