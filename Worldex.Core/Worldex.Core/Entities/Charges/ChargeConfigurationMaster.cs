using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Charges
{
    public class ChargeConfigurationMaster : BizBase
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
    public class ChargeConfigurationMasterArbitrage : BizBase
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
}
