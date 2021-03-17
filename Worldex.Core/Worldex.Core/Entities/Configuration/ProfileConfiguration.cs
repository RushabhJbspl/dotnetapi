using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration
{
   public class ProfileConfiguration : BizBase
    {
        [Required]
        [StringLength(200)]
        public string ConfigType { get; set; }

        [Required]
        [StringLength(250)]
        public string ConfigKey { get; set; }

        [Required]
        [StringLength(250)]
        public string ConfigValue { get; set; }

        public bool IsEnable { get; set; }

        public bool IsDeleted { get; set; }

        public void SetEnableStatus()
        {
            IsEnable = false;
            Events.Add(new ServiceStatusEvent<ProfileConfiguration>(this));
        }
        public void SetDisableStatus()
        {
            IsEnable = true;
            Events.Add(new ServiceStatusEvent<ProfileConfiguration>(this));
        }


        public void SetUnDeleteStatus()
        {
            IsDeleted = false;
            Events.Add(new ServiceStatusEvent<ProfileConfiguration>(this));
        }
        public void SetDeleteStatus()
        {
            IsDeleted = true;
            Events.Add(new ServiceStatusEvent<ProfileConfiguration>(this));
        }
    }
}
