using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.NewWallet
{
    public class TransactionPolicy : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }
        [Key]
        [Required]
        public long TrnType { get; set; }

        [Key]
        [Required]
        public long RoleId { get; set; }//2018-12-14

        [Required]
        public string AllowedIP { get; set; }
        [Required]
        public string AllowedLocation { get; set; }

        [Required]
        public int AuthenticationType { get; set; }
        public double? StartTime { get; set; } 
        public double? EndTime { get; set; }


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
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MinAmount { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MaxAmount { get; set; }


        [Required]
        public short AuthorityType { get; set; }
        [Required]
        public short AllowedUserType { get; set; }

        //2019-3-8 add bit for KYC
        [Key]
        [Required]
        [DefaultValue(0)]
        public short IsKYCEnable { get; set; }


        [Required]
        public long DailyVelocityAddressCount { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal DailyVelocityAddressAmount { get; set; }


        [Required]
        public long MonthlyVelocityAddressCount { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MonthlyVelocityAddressAmount { get; set; }


        [Required]
        public long WeeklyVelocityAddressCount { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal WeeklyVelocityAddressAmount { get; set; }


        [Required]
        public long YearlyVelocityAddressCount { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal YearlyVelocityAddressAmount { get; set; }

        public void DisableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<TransactionPolicy>(this));
        }

        public void EnableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<TransactionPolicy>(this));
        }
    }
}
