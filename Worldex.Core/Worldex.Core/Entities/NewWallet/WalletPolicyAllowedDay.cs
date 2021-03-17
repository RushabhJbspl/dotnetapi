using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.NewWallet
{
    public class WalletPolicyAllowedDay : BizBase
    {
        [Required]
        public long WalletPolicyID { get; set; }
        [Required]
        public short DayNo { get; set; } //0-All, 1-Monday onwards

        public void DisableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<WalletPolicyAllowedDay>(this));
        }

        public void EnableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<WalletPolicyAllowedDay>(this));
        }
    }
}
