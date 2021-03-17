using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.MarginEntitiesWallet
{
    public class ArbitrageDepositFund : BizBase
    {
        public string GUID { get; set; }

        [StringLength(1000)]
        public string TrnID { get; set; }

        [Required]
        [StringLength(50)]
        public string SMSCode { get; set; }

        [Required]
        public long FromSerProId { get; set; }

        [Required]
        public long ToserProId { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        [Required]
        public short IsProcessing { get; set; }

        [Required]
        [StringLength(50)]
        public string ToAddress { get; set; }

        [Required]
        [StringLength(100)]
        public string SystemRemarks { get; set; }

        [Required]
        public long TrnNo { get; set; }

        [Required]
        public string RouteTag { get; set; }

        [Required]
        public long UserId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TrnDate { get; set; }

        [Required]
        [StringLength(50)]
        public string ProviderWalletID { get; set; }

        public long ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        [Required]
        public long Confirmations { get; set; }

        [Required]
        public decimal? Value { get; set; }
    }
}
