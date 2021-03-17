using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.NewWallet
{
    public class CurrencyTypeMaster : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Required]
        [Key]
        public long CurrencyTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string CurrencyTypeName { get; set; }

        public void DisableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<CurrencyTypeMaster>(this));
        }

        public void EnableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<CurrencyTypeMaster>(this));
        }
    }
}
