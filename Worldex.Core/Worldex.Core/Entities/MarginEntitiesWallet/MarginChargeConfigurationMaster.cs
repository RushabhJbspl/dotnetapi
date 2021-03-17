using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.MarginEntitiesWallet
{
    public class MarginChargeConfigurationMaster : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Key]
        [Required]
        public long WalletTypeID { get; set; }//fk

        [Key]
        [Required]
        public long TrnType { get; set; }//EnWalletTrnType

        [Key]
        [Required]
        public short KYCComplaint { get; set; }

        [Required]
        public short SlabType { get; set; } //EnStakingSlabType

        [Key]
        [Required]
        public long SpecialChargeConfigurationID { get; set; }//fk

        public string Remarks { get; set; }
    }

    public class ArbitrageChargeConfigurationMaster : BizBase
    {
        [Required]
        public long WalletTypeID { get; set; }//fk

        [Required]
        public long PairId { get; set; }//fk

        [Required]
        public long SerProId { get; set; }//fk

        [Required]
        public long TrnType { get; set; }//EnWalletTrnType

        [Required]
        public short KYCComplaint { get; set; }

        public string Remarks { get; set; }
    }
}
