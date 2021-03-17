using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.User
{
    public class CustomPassword : BizBase
    {
        [Required]
        public long UserId { get; set; }
        [Required]
        public string Password { get; set; }

        public bool EnableStatus { get; set; }

        public void SetAsPasswordStatus()
        {
            EnableStatus = true;
            Events.Add(new ServiceStatusEvent<CustomPassword>(this));
        }

        public void SetAsUpdateDate(long Id)
        {
            UpdatedDate = DateTime.UtcNow;
            UpdatedBy = Id;
            Events.Add(new ServiceStatusEvent<CustomPassword>(this));
        }
    }
}
