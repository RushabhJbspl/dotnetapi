using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.KYCConfiguration
{
    public class KYCIdentityMasterInsertViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please enter a User Configuration name,8015")]
        [Display(Name = "Username")]
        [StringLength(100, ErrorMessage = "1,Please enter a valid Configuration Name,8016")]

        public string Name { get; set; }
        public string DocumentMasterId { get; set; }
    }
    public class KYCIdentityMasterUpdateViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please enter a User Configuration name,8015")]
        [Display(Name = "Username")]
        [StringLength(100, ErrorMessage = "1,Please enter a valid Configuration Name,8016")]
        public string Name { get; set; }
        [Required(ErrorMessage = "1,Please Enter Id,8002")]
        public Guid Id { get; set; }
        public string DocumentMasterId { get; set; }
        public bool Stutas { get; set; }
    }
    public class KYCIdentityMasterUpdateReqViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Stutas { get; set; }
        public int UserId { get; set; }
        public string DocumentMasterId { get; set; }
    }

    public class UserKYCConfigurationMappingViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Id,8002")]
        public Guid KYCConfigurationMasterId { get; set; }
        public long LevelId { get; set; }
    }
    public class UserKYCConfigurationMappingReqViewModel : TrackerViewModel
    {
        public Guid KYCConfigurationMasterId { get; set; }
        public int UserId { get; set; }
        public long LevelId { get; set; }
    }
    public class KYCIdentityMasterInsertReqViewModel
    {
        public string Name { get; set; }
        public int UserId { get; set; }
        public string DocumentMasterId { get; set; }
    }

    public class UserKYCConfigurationMappingUpdateViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Id,8002")]
        public Guid Id { get; set; }
        public bool Status { get; set; }
        public long LevelId { get; set; }
    }

    public class UserKYCConfigurationMappingUpdateReqViewModel
    {
        public Guid Id { get; set; }
        public bool Status { get; set; }
        public Guid KYCConfigurationMasterId { get; set; }
        public int UserId { get; set; }
        public long LevelId { get; set; }
    }
    public class KYCIndentityConfigurationList : BizResponseClass
    {
        public List<KYCIndentityMappinglistViewModel> kYCIndentitylistViewModel { get; set; }
    }

    public class KYCIndentitylistViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DocumentMasterId { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// get the KYCIudentityList created By pankaj
    /// </summary>
    public class KYCIndentitylistDataResponseViewModel  : BizResponseClass
    {
        public List<KYCIndentitylistViewModel> KYCIndentitylistViewModels{ get; set; }
    }

    public class KYCInsertDocumentId
    {
        public Guid ID { get; set; }
        public string DocumentMasterId { get; set; }
    }


    public class KYCIndentitylistResponseViewModel : BizResponseClass
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class KYCIndentityMappinglistViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class KYCIdentityMasterInsertResponse : BizResponseClass
    { }
    public class UserKYCConfigurationMappingresponse : BizResponseClass
    { }

}
