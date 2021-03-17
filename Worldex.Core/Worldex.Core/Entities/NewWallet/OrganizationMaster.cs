using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.NewWallet
{
    public class OrganizationMaster : BizBase
    {
        [Required]
        [StringLength(50)]
        public string OrganizationName { get; set; }

        [Required]
        public short IsDefault { get; set; }

        public string Email { get; set; }

        public string ContactNo { get; set; }

        [StringLength(50)]
        public string Website { get; set; }

        public string Address { get; set; }

        public long CityID { get; set; }

        public short AuthenticationType { get; set; }  //1-2FA,2-OTP,3-Pin

        public short TnCAccepted { get; set; }

        public void DisableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<OrganizationMaster>(this));
        }

        public void EnableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<OrganizationMaster>(this));
        }
    }
}
