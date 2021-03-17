using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Charges
{
    public class BackgroundCallMaster : BizBase
    {
        [Required]
        public string BathNo { get; set; }

        [Required]
        public short BgTaskType { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime BgTaskCallDate { get; set; }

        [Required]
        public string Remarks { get; set; }
    }
}
