using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Wallet
{
    public class WithdrawHistory : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public new long Id { get; set; }

        public string GUID { get; set; }

        [StringLength(1000)]
        public string TrnID { get; set; }

        [Required]
        [StringLength(50)]
        public string SMSCode { get; set; }

        [Required]
        public long WalletId { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        public long Confirmations { get; set; }

        [Required]
        public decimal Value { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        [Required]
        public decimal Charge { get; set; }

        [Required]
        public short State { get; set; }

        [Required]
        [StringLength(50)]
        public string confirmedTime { get; set; }

        [Required]
        [StringLength(50)]
        public string unconfirmedTime { get; set; }

        [Required]
        [StringLength(50)]
        public string createdTime { get; set; }

        [Required]
        public short IsProcessing { get; set; }

        [Required]
        [StringLength(200)]
        public string ToAddress { get; set; }

        [Required]
        [StringLength(50)]
        public string APITopUpRefNo { get; set; }

        [Required]
        [StringLength(100)]
        public string SystemRemarks { get; set; }

        [Required]
        public long TrnNo { get; set; }

        [Required]
        public string RouteTag { get; set; }

        [Required]
        public long SerProID { get; set; }

        [Required]
        public long UserId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TrnDate { get; set; }

        [Required]
        [StringLength(50)]
        public string ProviderWalletID { get; set; }
    }
}
