using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Communication
{
    public  class CommServiceMaster : BizBase
    {
        [Required]
        public long RequestID { get; set; }

        [Required]
        public long CommSerproID { get; set; }

        [Required]
        [StringLength(60)]
        public string ServiceName { get; set; }

        public string ResponseSuccess { get; set; }

        public string ResponseFailure { get; set; }

        public long ParsingDataID { get; set; }

        public void DisableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<CommServiceMaster>(this));
        }

        public void EnableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<CommServiceMaster>(this));
        }
    }
}
