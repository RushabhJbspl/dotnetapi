using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Organization
{
    public class ApplicationViewModel : TrackerViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "1,Please Enter Application Name,9003")]
        [StringLength(250)]
        public string ApplicationName { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public bool EnableStatus { get; set; }

        public long CreatedBy { get; set; }
    }

    public class ApplicationData : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Application Name,9003")]
        [StringLength(250)]
        public string ApplicationName { get; set; }

        [StringLength(250)]
        public string Description { get; set; }
    }


    public class ApplicationEnableDisable : TrackerViewModel
    {
        public Guid Id { get; set; }
    }

    public class ApplicationResponse : BizResponseClass
    {

    }

    public class ApplicationTotalCount
    {
        public long TotalApplication { get; set; }
        public long TotalActiveApplication { get; set; }
        public long TotalDisActiveApplication { get; set; }
    }

    public class ApplicationTotalResponse : BizResponseClass
    {
        public ApplicationTotalCount TotalCountApplication { get; set; }
    }


    public class GetTotalApplicationList
    {
        public Guid Id { get; set; }
        public string ApplicationName { get; set; }
        public string Description { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }       
        public bool Status { get; set; }
    }

    public class GetTotalApplicationListResponse : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<GetTotalApplicationList> GetTotalApplicationList { get; set; }
    }

    public class GetTotalActiveApplicationResponse : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<GetTotalApplicationList> GetTotalActiveApplicationList { get; set; }
    }

    public class GetTotalDisactiveApplicationResponse : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<GetTotalApplicationList> GetTotalDisactiveApplicationList { get; set; }
    }

    public class GetApplicationData
    {
        public Guid Id { get; set; }
        public string ApplicationName { get; set; }
    }

    public class GetApplicationDataResponse : BizResponseClass
    {
        public List<GetApplicationData> GetApplicationData { get; set; }
    }
}
