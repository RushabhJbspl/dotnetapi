using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class TrackerViewModel
    {
        [Required(ErrorMessage = "1,DeviceID Not Found,4015")]
        [StringLength(2000, ErrorMessage = "1,DeviceID Not Valid,4016")]
        public string DeviceId { get; set; }

        [Required(ErrorMessage = "1,Mode Not Found,4017")]
        [StringLength(10, ErrorMessage = "1,Mode Not Valid,4018")]
        public string Mode { get; set; }

        public string IPAddress { get; set; }

        [Required(ErrorMessage = "1,HostName Not Found,4021")]
        [StringLength(250, ErrorMessage = "1,Invalid HostName,4022")]
        public string HostName { get; set; }
    }
}
