using Worldex.Core.ViewModels.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.SocialProfile
{
    public class UserProfileConfigViewModel : TrackerViewModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public long ProfileConfigId { get; set; }
        [Required]
        [StringLength(250)]
        public string ConfigValue { get; set; }

        public bool IsEnable { get; set; }

        public bool IsDeleted { get; set; }
    }
}
