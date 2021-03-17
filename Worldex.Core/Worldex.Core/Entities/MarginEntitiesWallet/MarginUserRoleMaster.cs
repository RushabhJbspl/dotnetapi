using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.MarginEntitiesWallet
{
    public class MarginUserRoleMaster : BizBase
    {
        [Required]
        [StringLength(20)]
        public string RoleName { get; set; }

        [Required]
        [StringLength(20)]
        public string RoleType { get; set; }

        public void DisableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<MarginUserRoleMaster>(this));
        }

        public void EnableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<MarginUserRoleMaster>(this));
        }
    }

    
}
