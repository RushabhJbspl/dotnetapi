using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Organization
{
    public class ActivityRegister : BizBaseExtended
    {
        [Required]
        [StringLength(500)]
        public string Remark { get; set; }

        [StringLength(500)]
        public string Connection { get; set; }

        public Guid ApplicationId { get; set; }

        public long StatusCode { get; set; }

        [StringLength(500)]
        public string Channel { get; set; }

        [StringLength(2000)]
        public string DeviceId { get; set; }

        [StringLength(30)]
        public string IPAddress { get; set; }

        public long ReturnCode { get; set; }

        [StringLength(8000)]
        public string ReturnMsg { get; set; }

        public long ErrorCode { get; set; }

        public Guid ActivityTypeId { get; set; }

        [StringLength(4000)]
        public string Session { get; set; }

        [StringLength(4000)]
        public string AccessToken { get; set; }

        [StringLength(1000)]
        public string AliasName { get; set; }

        public long ModuleTypeId { get; set; }

        public Guid HostURLId { get; set; }
    }
}
