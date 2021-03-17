using System;
using System.ComponentModel.DataAnnotations;
using Worldex.Core.Enums.Modes;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.User
{
    public class TempUserRegister : BizBase
    {
        [Required]
        public int RegTypeId { get; set; }
        [StringLength(100)]
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStemp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        [StringLength(250)]
        public string FirstName { get; set; }
        [StringLength(250)]
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public bool RegisterStatus { get; set; }
        public bool IsDeleted { get; set; }
        [StringLength(5)]
        public string CountryCode { get; set; }

        public void SetAsStatus()
        {
            RegisterStatus = Convert.ToBoolean(ModeStatus.True);
            Events.Add(new ServiceStatusEvent<TempUserRegister>(this));
        }

        public void SetAsUpdateDate(long Id)
        {
            UpdatedDate = DateTime.UtcNow;
            UpdatedBy = Id;
            Events.Add(new ServiceStatusEvent<TempUserRegister>(this));
        }

    }
}
