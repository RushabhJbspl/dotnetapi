using Worldex.Core.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.MarginEntitiesWallet
{
    public class MarginWalletTransactionQueue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long TrnNo { get; set; }

        [Required]
        [StringLength(50)]
        public Guid Guid { get; set; }

        public string RefGuid { get; set; }

        [Required]
        public enWalletTranxOrderType TrnType { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        [Required]
        public long TrnRefNo { get; set; }

        [Required]
        public DateTime TrnDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [Required]
        public long WalletID { get; set; }

        [Required]
        [StringLength(10)]
        public string WalletType { get; set; }

        [Required]
        public long MemberID { get; set; }

        [Required]
        [StringLength(50)]
        public string TimeStamp { get; set; }

        public enTransactionStatus Status { get; set; }

        [Required]
        [StringLength(100)]
        public string StatusMsg { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        [DefaultValue(0)]
        public decimal SettedAmt { get; set; }

        public long AllowedChannelID { get; set; }
        
        public enMarginWalletTrnType WalletTrnType { get; set; }

        public enWalletDeductionType WalletDeductionType { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        [DefaultValue(0)]
        public decimal HoldChargeAmount { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        [DefaultValue(0)]
        public decimal DeductedChargeAmount { get; set; }

        public long? ErrorCode { get; set; }

        public int IsProcessing { get; set; } //ntrivedi 20-05-2019

        public MarginWalletTransactionQueue() // ntrivedi 04-12-2018 assign default channel
        {
            AllowedChannelID = (long) EnAllowedChannels.Web;
            WalletDeductionType = enWalletDeductionType.Normal;
            DeductedChargeAmount = 0;
            HoldChargeAmount = 0;
            ErrorCode = 0;
        }
        public void ChangeStatus(enTransactionStatus status, string msg)
        {
            Status = status;
            StatusMsg = msg;
            UpdatedDate = Helpers.Helpers.UTC_To_IST();           
        }
    }

    public class ArbitrageWalletTransactionQueue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long TrnNo { get; set; }

        [Required]
        [StringLength(50)]
        public Guid Guid { get; set; }

        public string RefGuid { get; set; }

        [Required]
        public enWalletTranxOrderType TrnType { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        [Required]
        public long TrnRefNo { get; set; }

        [Required]
        public DateTime TrnDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [Required]
        public long WalletID { get; set; }

        [Required]
        [StringLength(10)]
        public string WalletType { get; set; }

        [Required]
        public long PairID { get; set; } //ntrivedi 11-06-2019 

        [Required]
        public long MemberID { get; set; }

        [Required]
        [StringLength(50)]
        public string TimeStamp { get; set; }

        public enTransactionStatus Status { get; set; }

        [Required]
        [StringLength(100)]
        public string StatusMsg { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        [DefaultValue(0)]
        public decimal SettedAmt { get; set; }

        public long AllowedChannelID { get; set; }

        public enWalletTrnType WalletTrnType { get; set; }

        public enWalletDeductionType WalletDeductionType { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        [DefaultValue(0)]
        public decimal HoldChargeAmount { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        [DefaultValue(0)]
        public decimal DeductedChargeAmount { get; set; }

        public long? ErrorCode { get; set; }

        public int IsProcessing { get; set; } //ntrivedi 20-05-2019

        public ArbitrageWalletTransactionQueue() // ntrivedi 04-12-2018 assign default channel
        {
            AllowedChannelID = (long)EnAllowedChannels.Web;
            WalletDeductionType = enWalletDeductionType.Normal;
            DeductedChargeAmount = 0;
            HoldChargeAmount = 0;
            ErrorCode = 0;
        }
        public void ChangeStatus(enTransactionStatus status, string msg)
        {
            Status = status;
            StatusMsg = msg;
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
        }

        public int LPType { get; set; } //ntrivedi 10-06-2019
    }
}
