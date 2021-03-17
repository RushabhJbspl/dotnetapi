using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class StopAutoRenewRequest
    {
        [Required]
        public long AutoRenewID { get; set; }
        [Required]
        public long SubscribeID { get; set; }
    }
}
