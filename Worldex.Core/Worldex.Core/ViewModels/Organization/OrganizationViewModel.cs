using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Organization
{
    public class OrganizationViewModel : TrackerViewModel
    {
        public int UserId { get; set; }
        public Guid Id { get; set; }
        [Required(ErrorMessage = "1,Please Enter Domain Name,9001")]
        [StringLength(250, ErrorMessage = "1,Please enter a valid Domain Name,9002")]
        public string DomainName { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
        public bool Status { get; set; }
        [StringLength(250)]
        public string AliasName { get; set; }
    }

    public class DomainViewModel : TrackerViewModel
    {
        [StringLength(250)]
        public string AliasName { get; set; }
        [Required(ErrorMessage = "1,Please Enter Domain Name,9001")]
        [StringLength(250, ErrorMessage = "1,Please enter a valid Domain Name,9002")]
        public string DomainName { get; set; }
    }

    public class OrganizationTotalDomainCount 
    {
        public long TotalDoamin { get; set; }
        public long TotalActiveDomain { get; set; }
        public long TotalDisActiveDomain { get; set; }
    }

    public class OrganizationTotalDomainResponse : BizResponseClass
    {
        public OrganizationTotalDomainCount TotalCountDomain { get; set; }
    }

    public class GetTotalOrganizationList 
    {
        public Guid Id { get; set; }
        public string DomainName { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedDate { get; set; }
        public bool Status { get; set; }
        public string UserName { get; set; }
        public string AliasName { get; set; }
    }

    public class GetTotalOrganizationResponse : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<GetTotalOrganizationList> GetTotalDomainList { get; set; }
    }

    public class GetTotalActiveOrganizationResponse : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<GetTotalOrganizationList> GetTotalActiveDomainList { get; set; }
    }

    public class GetTotalDisactiveOrganizationResponse : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<GetTotalOrganizationList> GetTotalDisactiveDomainList { get; set; }
    }

    public class OrganizationEnableDisable: TrackerViewModel
    {
        public Guid Id { get; set; }
    }

    public class OrganizationResponse : BizResponseClass
    {

    }

    public class GetUserWiseDomainData
    {
        public Guid Id { get; set; }
        public string DomainName { get; set; }
    }

    public class GetUserWiseDomainResponse : BizResponseClass
    {
        public List<GetUserWiseDomainData> GetUserWiseDomainData { get; set; }
    }
}
