using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Organization
{
    public class Organization_Master : BizBaseExtended
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(250)]
        public string DomainName { get; set; }

        
        [StringLength(250)]
        public string AliasName { get; set; }

        public void SetEnableOrganizationStatus(int UserId)
        {
            Status = true;
            UpdatedBy = UserId;
            UpdatedDate = DateTime.UtcNow;
            Events.Add(new ServiceStatusEvent<Organization_Master>(this));
        }
        public void SetDisableOrganizationStatus(int UserId)
        {
            Status = false;
            UpdatedBy = UserId;
            UpdatedDate = DateTime.UtcNow;
            Events.Add(new ServiceStatusEvent<Organization_Master>(this));
        }
    }
}
