using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.NewWallet
{
    public class WalletUsagePolicy : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PolicyName { get; set; }

        [Key]
        [Required]
        public long WalletType { get; set; }

        [Required]
        public string AllowedIP { get; set; }
        [Required]
        public string AllowedLocation { get; set; }
        [Required]
        public int AuthenticationType { get; set; }
        public double? StartTime { get; set; }
        public double? EndTime { get; set; }

        [Required]
        public long HourlyTrnCount { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal HourlyTrnAmount { get; set; }

        [Required]
        public long DailyTrnCount { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal DailyTrnAmount { get; set; }

        [Required]
        public long MonthlyTrnCount { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MonthlyTrnAmount { get; set; }

        [Required]
        public long WeeklyTrnCount { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal WeeklyTrnAmount { get; set; }

        [Required]
        public long YearlyTrnCount { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal YearlyTrnAmount { get; set; }

        [Required]
        public long LifeTimeTrnCount { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal LifeTimeTrnAmount { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MinAmount { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MaxAmount { get; set; }

        public void DisableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<WalletUsagePolicy>(this));
        }

        public void EnableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<WalletUsagePolicy>(this));
        }
    }
}
