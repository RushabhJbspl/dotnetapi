using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Communication
{
    public  class CommServiceTypeMaster : BizBase
    {
        [Required]
        public long CommServiceTypeID { get; set; }

        [Required]
        public long ServiceTypeID { get; set; }

        [Required]
        [StringLength(60)]
        public string CommServiceTypeName { get; set; }

        public void DisableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<CommServiceTypeMaster>(this));
        }

        public void EnableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<CommServiceTypeMaster>(this));
        }
    }
}
