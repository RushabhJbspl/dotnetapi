using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Transaction
{
    public  class TransactionStatus : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }
        [Key]
        [Required]
        public long TrnNo { get; set; }
        [Key]
        [Required]
        public long ServiceID { get; set; }
        [Key]
        [Required]
        public long SerProID { get; set; }
        public string StatusMsg { get; set; }
    }
}
