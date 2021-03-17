using System;
using System.ComponentModel.DataAnnotations;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Log
{
   public class IpMaster  : BizBase
    {
        //public Guid? GUID { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(15)]
        public string IpAddress { get; set; }
        [StringLength(150)]
        public string IpAliasName { get; set; }
        public bool IsEnable { get; set; }
        public bool IsDeleted { get; set; }
      

        public void SetAsIsEnabletatus()
        {
            IsEnable = true;
            Status = 1; //add status bit- mansi 09-10-2019
            Events.Add(new ServiceStatusEvent<IpMaster>(this));
        }

        public void SetAsIsDisabletatus()
        {
            IsEnable = false;
            Status = 0;//add status bit- mansi 09-10-2019
            Events.Add(new ServiceStatusEvent<IpMaster>(this));
        }

        public void SetAsIpDeletetatus()
        {
            IsDeleted = true;
            Status = 0;//add status bit- mansi 09-10-2019
            Events.Add(new ServiceStatusEvent<IpMaster>(this));
        }
    }
}
