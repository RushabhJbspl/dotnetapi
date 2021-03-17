using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.Log
{
   public class HistoryViewModel
    {
        public long Id { get; set; }
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
        public string HostName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
