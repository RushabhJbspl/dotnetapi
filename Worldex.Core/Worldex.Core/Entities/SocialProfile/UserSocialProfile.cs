using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.SocialProfile
{
    public class UserSocialProfile : BizBase
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(10)]
        public string ProfileRole { get; set; }

        public bool IsEnable { get; set; }

        public bool IsDeleted { get; set; }


        public void SetEnableStatus()
        {
            IsEnable = false;
            Events.Add(new ServiceStatusEvent<UserSocialProfile>(this));
        }
        public void SetDisableStatus()
        {
            IsEnable = true;
            Events.Add(new ServiceStatusEvent<UserSocialProfile>(this));
        }


        public void SetUnDeleteStatus()
        {
            IsDeleted = false;
            Events.Add(new ServiceStatusEvent<UserSocialProfile>(this));
        }
        public void SetDeleteStatus()
        {
            IsDeleted = true;
            Events.Add(new ServiceStatusEvent<UserSocialProfile>(this));
        }
    }
}
