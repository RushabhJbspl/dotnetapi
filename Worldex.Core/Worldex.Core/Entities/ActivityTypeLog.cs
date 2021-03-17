using Worldex.Core.Enums;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities
{
    public class ActivityTypeLog : BizBase
    {
        [Required]
        public long UserID { get; set; }

        [Required]
        public enActivityType ActivityType { get; set; }//(0-ChangePAssword,1-ForgotPassword,2-DeviceChange)

        [Required]
        public DateTime ActivityDate { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }
    }

    public class ActivityTypeHour : BizBase
    {
        [Required]
        public int ActivityHour { get; set; }

        [Required]
        public int ActivityType { get; set; }
    }
}
