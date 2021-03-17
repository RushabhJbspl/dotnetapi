using Worldex.Core.SharedKernel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.MarginEntitiesWallet
{
    public class MarginTrnChargeLog : BizBase
    {
        public string BatchNo { get; set; }

        [Required]
        public long TrnNo { get; set; }

        [Required]
        public long TrnType { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal? MakerCharge { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal? TakerCharge { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal? Charge { get; set; }

        public long? StakingChargeMasterID { get; set; }

        public long? ChargeConfigurationDetailID { get; set; }

        public string TimeStamp { get; set; }

        [Required]
        public long DWalletID { get; set; }

        [Required]
        public long OWalletID { get; set; }//org

        [Required]
        public long DUserID { get; set; }

        [Required]
        public long OuserID { get; set; }

        public long WalletTypeID { get; set; }

        public short SlabType { get; set; }//EnStakingSlabType

        public string Remarks { get; set; }

        public long? ChargeConfigurationMasterID { get; set; }

        public short? IsMaker { get; set; }//25-1-2019,1= maker,0= taker,2=else

        public long? TrnRefNo { get; set; }

        [DefaultValue(0)]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal? OriginalAmount { get; set; }
    }

    public class ArbitrageTrnChargeLog : BizBase
    {
        public string BatchNo { get; set; }

        [Required]
        public short Type { get; set; }//1-user 2=provider

        [Required]
        public long TrnNo { get; set; }

        [Required]
        public long TrnType { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal? MakerCharge { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal? TakerCharge { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal? Charge { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MarkUpValue { get; set; }

        public long? ChargeConfigurationDetailID { get; set; }

        public string TimeStamp { get; set; }

        [Required]
        public long DWalletID { get; set; }

        [Required]
        public long OWalletID { get; set; }//org

        [Required]
        public long DUserID { get; set; }

        [Required]
        public long OuserID { get; set; }

        public long WalletTypeID { get; set; }

        public long PairId { get; set; }

        public string Remarks { get; set; }

        public long? ChargeConfigurationMasterID { get; set; }

        public short? IsMaker { get; set; }//25-1-2019,1= maker,0= taker,2=else

        public long? TrnRefNo { get; set; }
    }
}
