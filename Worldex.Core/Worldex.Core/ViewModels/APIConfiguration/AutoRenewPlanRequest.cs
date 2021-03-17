using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class AutoRenewPlanRequest
    {
        [Required]
        public long SubscribePlanID { get; set; }
        [Required]
        public long DaysBeforeExpiry { get; set; }
    }
}
