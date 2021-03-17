using Worldex.Core.SharedKernel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Wallet
{
    public class TokenUnStakingHistory : BizBase
    {
        [Required]
        public long TokenStakingHistoryID { get; set; }

        public string GUID { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal AmountCredited { get; set; }

        [Required]
        [DefaultValue(0)]
        public short UnstakeType { get; set; }// 1 - full, 2- partial(degrade)

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal InterestCreditedValue { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal ChargeBeforeMaturity { get; set; }

        [Required]
        public long DegradeStakingHistoryRequestID { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        [DefaultValue(0)]
        public decimal DegradeStakingAmount { get; set; }
    }
}
