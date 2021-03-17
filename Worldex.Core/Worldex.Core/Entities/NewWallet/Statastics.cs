using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.NewWallet
{
    public class Statastics : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Key]
        [Required]
        public long TrnType { get; set; }

        [Key]
        [Required]
        public long WalletType { get; set; }

        [Key]
        [Required]
        public long WalletId { get; set; }

        [Key]
        [Required]
        public long UserId { get; set; }

        [Key]
        [Required]
        public long Hour { get; set; }

        [Key]
        [Required]
        public long Day { get; set; }

        [Key]
        [Required]
        public long Week { get; set; }

        [Key]
        [Required]
        public long Month { get; set; }

        [Key]
        [Required]
        public long Year { get; set; }

        [Required]
        public long Count { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public decimal? USDAmount { get; set; }
    }
}
