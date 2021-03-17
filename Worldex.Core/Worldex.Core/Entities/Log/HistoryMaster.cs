using System.ComponentModel.DataAnnotations;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Log
{
    public class HistoryMaster : BizBase
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public long HistoryTypeId { get; set; }
        [Required]
        [StringLength(250)]
        public string ServiceUrl { get; set; }
        [Required]
        public long IpId { get; set; }
        [Required]
        public long DeviceId { get; set; }
        [Required]
        [StringLength(10)]
        public string Mode { get; set; }
        [Required]
        [StringLength(250)]
        public  string HostName { get; set; }
        public bool IsDeleted { get; set; }

        public void SetAsIpDeletetatus()
        {
            IsDeleted = true;
            Events.Add(new ServiceStatusEvent<HistoryMaster>(this));
        }
    }
}
