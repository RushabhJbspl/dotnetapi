using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Organization
{
    public class OrgAppMappingViewModel : TrackerViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid OrgId { get; set; }

        [Required]
        public Guid AppId { get; set; }

        [Required(ErrorMessage = "1,Please Enter AppName,9004")]
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

        public string DomainName { get; set; }

        public string ApplicationType { get; set; }

    }


    public class CreateUserApplication : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter DomainId,9059")]
        public string DomainId { get; set; }
        [Required(ErrorMessage = "1,Please Enter Master ApplicationId,9060")]
        public string AppId { get; set; }

        [Required(ErrorMessage = "1,Please Enter AppName,9004")]
        [StringLength(250)]
        public string AppName { get; set; }
    }

    public class CreateUserWiseApplication : TrackerViewModel
    {
        public int UserId { get; set; }

        public Guid DomainId { get; set; }

        public Guid AppId { get; set; }

        public string AppName { get; set; }
    }

    public class GetUserWiseAppData
    {
        public Guid Id { get; set; }

        public Guid DomainId { get; set; }

        public string DomainName { get; set; }

        public Guid AppId { get; set; }

        public string ApplicationName { get; set; }

        public string AppName { get; set; }

        public Guid ClientID { get; set; }

        public Guid ClientSecret { get; set; }

        public string Description { get; set; }

        public string ApplicationLogo { get; set; }

        public string AllowedCallBackURLS { get; set; }

        public string AllowedWebOrigins { get; set; }

        public string AllowedLogoutURLS { get; set; }

        public string AllowedOriginsCORS { get; set; }

        public long JWTExpiration { get; set; }

    }

    public class GetUserWiseAppDataResponse : BizResponseClass
    {
        public GetUserWiseAppData UserWiseAppData { get; set; }
    }

    public class UpdateUserWiseAppData
    {

        public Guid Id { get; set; }

        [Required(ErrorMessage = "1,Please Enter AppName,9004")]
        [StringLength(250)]
        public string AppName { get; set; }

        public string DomainId { get; set; }

        public Guid ClientSecret { get; set; }

        public string Description { get; set; }

        public string ApplicationLogo { get; set; }

        public string AppId { get; set; }

        public string AllowedCallBackURLS { get; set; }

        public string AllowedWebOrigins { get; set; }

        public string AllowedLogoutURLS { get; set; }

        public string AllowedOriginsCORS { get; set; }

        public long JWTExpiration { get; set; }

    }

    public class UpdateUserGUIDWiseAppData
    {

        public Guid Id { get; set; }

        public string AppName { get; set; }

        public Guid DomainId { get; set; }

        public Guid ClientSecret { get; set; }

        public string Description { get; set; }

        public string ApplicationLogo { get; set; }

        public Guid AppId { get; set; }

        public string AllowedCallBackURLS { get; set; }

        public string AllowedWebOrigins { get; set; }

        public string AllowedLogoutURLS { get; set; }

        public string AllowedOriginsCORS { get; set; }

        public long JWTExpiration { get; set; }

    }

    public class OrgAppMappingResponse : BizResponseClass
    {

    }

    public class GetTotalUserApplicationList
    {
        public Guid Id { get; set; }
        public string ApplicationName { get; set; }
        public string Description { get; set; }
        public Guid ClientSecret { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
        public bool Status { get; set; }
        public string MasterApplicationName { get; set; }
    }

    public class GetTotalUserApplicationListResponse : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<GetTotalUserApplicationList> TotalUserApplicationList { get; set; }
    }

}
