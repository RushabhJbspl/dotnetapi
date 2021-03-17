using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.MarginEntitiesWallet
{
    public class MarginDepositHistory : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Key]
        [StringLength(100)]
        public string TrnID { get; set; }

        public string GUID { get; set; }

        [Required]
        public string SMSCode { get; set; }

        [Key]
        [Required]
        [StringLength(50)]
        public string Address { get; set; }

        [Required]
        public long Confirmations { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(100)]
        public string StatusMsg { get; set; }

        [Required]
        public string TimeEpoch { get; set; }

        [Required]
        public string ConfirmedTime { get; set; }


        public string EpochTimePure { get; set; } // time converted from epoch time 

        public long OrderID { get; set; }

        [DefaultValue(0)]
        public byte IsProcessing { get; set; }

        [Required]
        [StringLength(50)]
        public string FromAddress { get; set; }

        public string APITopUpRefNo { get; set; }

        public string SystemRemarks { get; set; }

        public string RouteTag { get; set; }

        public long SerProID { get; set; }

        public long UserId { get; set; }

        public short? IsInternalTrn { get; set; }       //Uday 22-01-2019 Check for internal transaction

        [DefaultValue(0)]
        public long WalletId { get; set; }
    }

}
