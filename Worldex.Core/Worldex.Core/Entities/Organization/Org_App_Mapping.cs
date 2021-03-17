using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Organization
{
    public class Org_App_Mapping : BizBaseExtended
    {
        [Required]
        public Guid OrgId { get; set; }

        [Required]
        public Guid AppId { get; set; }

        [Required]
        [StringLength(250)]
        public string AppName { get; set; }

        [Required]
        public Guid ClientSecret { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(250)]
        public string ApplicationLogo { get; set; }

        [StringLength(500)]
        public string AllowedCallBackURLS { get; set; }

        [StringLength(500)]
        public string AllowedWebOrigins { get; set; }

        [StringLength(500)]
        public string AllowedLogoutURLS { get; set; }

        [StringLength(500)]
        public string AllowedOriginsCORS { get; set; }

        public long JWTExpiration { get; set; }
    }
}
