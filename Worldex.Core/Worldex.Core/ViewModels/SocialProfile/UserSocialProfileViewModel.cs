using Worldex.Core.ViewModels.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.SocialProfile
{
    public class UserSocialProfileViewModel : TrackerViewModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(10)]
        public string ProfileRole { get; set; }

        public bool IsEnable { get; set; }

        public bool IsDeleted { get; set; }

    }
}
