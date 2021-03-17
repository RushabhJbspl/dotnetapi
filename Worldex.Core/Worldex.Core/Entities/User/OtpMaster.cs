using System;
using System.ComponentModel.DataAnnotations;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.User
{
    public class OtpMaster : BizBase
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int RegTypeId { get; set; }
        [Required]
        [StringLength(6, MinimumLength = 6)]
        [Range(6, Int64.MaxValue)]
        public string OTP { get; set; }        
        [DataType(DataType.DateTime)]
        public DateTime ExpirTime { get; set; }
        public string Remarks { get; set; }
        
        public void SetAsUpdateDate(long Id, string remarks, short status)
        {
            UpdatedDate = DateTime.UtcNow;
            UpdatedBy = Id;
            Remarks = remarks;
            Status = status;
            Events.Add(new ServiceStatusEvent<OtpMaster>(this));
        }
    }
}
