using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Transaction
{
    public class CloseOpenPostionMargin : BizBase
    {
        [Required]
        public long UserID { get; set; }

        [Required]
        public long PairID { get; set; }
       
        public string TrnRefNo { get; set; }
    }
}
