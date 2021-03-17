using Worldex.Core.SharedKernel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.NewWallet
{
    public class CurrencyRateMaster : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Required]
        [Key]
        public long WalletTypeId { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        [DefaultValue(0)]
        public decimal CurrentRate { get; set; }

        [DefaultValue("USD")]
        public string CurrencyName { get; set; }//2019-02-01
    }


    public class BalanceStatistics : BizBase
    {
        [Required]
        public long UserID { get; set; }

        [Required]
        public long WalletID { get; set; }

        [Required]
        public long WalletTypeID { get; set; }

        [Required]
        public short Year { get; set; }

        [Required]
        public short Month { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        [DefaultValue(0)]
        public decimal StartingBalance { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        [DefaultValue(0)]
        public decimal EndingBalance { get; set; }
    }
}
