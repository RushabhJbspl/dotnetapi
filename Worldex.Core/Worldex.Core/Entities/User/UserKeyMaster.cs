using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.User
{
    public class UserKeyMaster : BizBase
    {
        [Required]
        public long UserId { get; set; }
        [Required]
        public string uniqueKey { get; set; }

        public bool EnableStatus { get; set; }

        public void SetAsUniqueKeyStatus()
        {
            EnableStatus = true;
            Events.Add(new ServiceStatusEvent<UserKeyMaster>(this));
        }

        public void SetAsUpdateDate(long Id)
        {
            UpdatedDate = DateTime.UtcNow;
            UpdatedBy = Id;
            Events.Add(new ServiceStatusEvent<UserKeyMaster>(this));
        }
    }
}
