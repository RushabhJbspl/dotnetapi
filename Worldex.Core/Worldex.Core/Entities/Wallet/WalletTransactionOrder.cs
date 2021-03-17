using Worldex.Core.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Wallet
{
    public class WalletTransactionOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long OrderID { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [Required]
        public DateTime TrnDate { get; set; }

        [Required]
        public long OWalletID { get; set; }

        [Required]
        public long DWalletID { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(10)]
        public string WalletType { get; set; }

        [Required]
        public long OTrnNo { get; set; }

        [Required]
        public long DTrnNo { get; set; }

        public enTransactionStatus Status { get; set; }

        [Required]
        [StringLength(50)]
        public string StatusMsg { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        [DefaultValue(0)]
        public decimal ChargeAmount { get; set; }

        public WalletTransactionOrder()
        {
            ChargeAmount = 0;
        }

        public void ChangeStatus(enTransactionStatus status , string msg)
        {
            Status = status;
            StatusMsg = msg;
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
        }
    }
}
