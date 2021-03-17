using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class ManualRenewAPIPlanRequest
    {
        [Required]
        public long SubscribePlanID { get; set; }
        [Required]
        public short ChannelID { get; set; }
    }
}
