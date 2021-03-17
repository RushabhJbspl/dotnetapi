using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.KYCConfiguration
{
    public class DocumentMasterViewMaster : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter document name,8020")]
        [StringLength(50, ErrorMessage = "1,Please Enter valid document,8021")]
        public string Name { get; set; }
    }
    public class DocumentmasterReqViewMode
    {
        public int UserId { get; set; }
        public string Name { get; set; }
    }

    public class DocumentMasterUpdateViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }

    }
    public class DocumentMasterUpdateReqViewModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public Guid Id { get; set; }
    }


    public class DocumentMasterListViewModel
    {
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public Guid Id { get; set; }
    }
    public class DocumentMasterListResponse : BizResponseClass
    {
       public List<DocumentMasterListViewModel> DocumentMasterListViewModels { get; set; }
    }

    public class DocumentMasterResponse : BizResponseClass
    {

    }

    public class KYCDocumentConfigurationListViewmodel
    {
        public Guid ID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }

        public long KYCLevel { get; set; }
        public string KYCDocument { get; set; }
        public bool Status { get; set; }
        public string DocumentMasterId { get; set; }

    }
    public class KYCDocumentKYCFormatListViewmodel
    {
        public string DocumentName { get; set; }
    }
    public class KYCDocumentConfigurationListDisplayViewmodel
    {
        public Guid ID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }

        public long KYCLevel { get; set; }
        public string KYCDocument { get; set; }
        public bool Status { get; set; }
        public string DocumentMasterId { get; set; }
        public string DocumentName { get; set; }

    }

    public class KYCDocumentConfigurationListDisplayResponseViewmodel : BizResponseClass
    {
      public  List<KYCDocumentConfigurationListDisplayViewmodel> KYCDocumentConfigurationListDisplayViewmodels { get; set; }
    }
}
