using Worldex.Core.SharedKernel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.NewWallet
{
    public class WalletTrnLimitConfiguration : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Key]
        [Required]
        public long TrnType { get; set; }

        [Key]
        [Required]
        public long WalletType { get; set; }

        [Required]
        [DefaultValue(0)]
        public double StartTime { get; set; }

        [Required]
        [DefaultValue(0)]
        public double EndTime { get; set; }

        [Required]
        public long HourlyTrnCount { get; set; }

        [Required]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal HourlyTrnAmount { get; set; }

        [Required]
        public long DailyTrnCount { get; set; }

        [Required]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal DailyTrnAmount { get; set; }

        [Required]
        public long MonthlyTrnCount { get; set; }

        [Required]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MonthlyTrnAmount { get; set; }

        [Required]
        public long WeeklyTrnCount { get; set; }

        [Required]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal WeeklyTrnAmount { get; set; }

        [Required]
        public long YearlyTrnCount { get; set; }

        [Required]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal YearlyTrnAmount { get; set; }

        [Required]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MinAmount { get; set; }

        [Required]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MaxAmount { get; set; }

        //2019-3-8 add bit for KYC
        [Key]
        [Required]
        [DefaultValue(0)]
        public short IsKYCEnable { get; set; }
    }
}
