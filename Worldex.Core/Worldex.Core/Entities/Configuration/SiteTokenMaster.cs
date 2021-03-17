using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Configuration
{
    public class SiteTokenMaster : BizBase
    {
        [Required]
        public long CurrencyID { get; set; }

        [Required]
        public long BaseCurrencyID { get; set; }

        [Required]
        public string CurrencySMSCode { get; set; }

        [Required]
        public string BaseCurrencySMSCode { get; set; }

        [Required]
        public short RateType { get; set; }

        public long PairID { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public Decimal Rate { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public Decimal MinLimit { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public Decimal MaxLimit { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public Decimal DailyLimit { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public Decimal WeeklyLimit { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public Decimal MonthlyLimit { get; set; }

        public string Note { get; set; }

        public void DisableAppType()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<SiteTokenMaster>(this));
        }
        public void EnableAppType()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<SiteTokenMaster>(this));
        }
    }

    public class SiteTokenMasterMargin : BizBase
    {
        [Required]
        public long CurrencyID { get; set; }

        [Required]
        public long BaseCurrencyID { get; set; }

        [Required]
        public string CurrencySMSCode { get; set; }

        [Required]
        public string BaseCurrencySMSCode { get; set; }

        [Required]
        public short RateType { get; set; }

        public long PairID { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public Decimal Rate { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public Decimal MinLimit { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public Decimal MaxLimit { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public Decimal DailyLimit { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public Decimal WeeklyLimit { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public Decimal MonthlyLimit { get; set; }

        public string Note { get; set; }

        public void DisableAppType()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<SiteTokenMasterMargin>(this));
        }
        public void EnableAppType()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<SiteTokenMasterMargin>(this));
        }
    }
}
