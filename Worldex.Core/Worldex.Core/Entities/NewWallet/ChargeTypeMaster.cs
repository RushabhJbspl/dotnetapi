using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.NewWallet
{
    public class ChargeTypeMaster : BizBase
    {
        [Key]
        [Required]
        public new long Id { get; set; }

        [Required]
        public string ChargeName { get; set; }      

        public void DisableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<ChargeTypeMaster>(this));
        }

        public void EnableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<ChargeTypeMaster>(this));
        }
    }
}
