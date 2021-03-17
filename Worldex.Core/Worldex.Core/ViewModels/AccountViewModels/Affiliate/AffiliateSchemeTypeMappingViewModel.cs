using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.AccountViewModels.Affiliate
{
   public class AffiliateSchemeTypeMappingViewModel
    {       
        [Required(ErrorMessage ="1,Please Enter Required Parameter,31028")]
        public long SchemeMasterId { get; set; }  // Reference From  AffiliateSchemeMaster

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31023")]
        public long SchemeTypeMasterId { get; set; }  // Reference From  AffiliateSchemeTypeMaster

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31029")]
        public decimal MinimumDepositionRequired { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31030")]
        public long DepositWalletTypeId { get; set; }  // Reference From  WalletTypeMasters

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31031")]
        [EnumDataType(typeof(EnAffiCommisionTypeInterval), ErrorMessage = "1,Invalid Required Parameter,31032")]
        public EnAffiCommisionTypeInterval CommissionTypeInterval { get; set; }  // EnAffiCommisionTypeInterval

        public string Description { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31033")]
        public long CommissionHour { get; set; } // Total Commission Hour 

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31026")]
        [EnumDataType(typeof(ServiceStatus), ErrorMessage = "1,Invalid Required Parameter,31034")]
        public ServiceStatus Status { get; set; }
    }
    public class AffiliateSchemeTypeMappingResponse : BizResponseClass
    {

    }
    public class AffiliateSchemeTypeMappingUpdateViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,31035")]
        public long MappingId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31029")]
        public decimal MinimumDepositionRequired { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31030")]
        public long DepositWalletTypeId { get; set; }  // Reference From  WalletTypeMasters

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31031")]
        [EnumDataType(typeof(EnAffiCommisionTypeInterval), ErrorMessage = "1,Invalid Required Parameter,31032")]
        public EnAffiCommisionTypeInterval CommissionTypeInterval { get; set; }  // EnAffiCommisionTypeInterval

        public string Description { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31033")]
        public long CommissionHour { get; set; } // Total Commission Hour 

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31026")]
        [EnumDataType(typeof(ServiceStatus), ErrorMessage = "1,Invalid Required Parameter,31034")]
        public ServiceStatus Status { get; set; }
    }
    public class AffiliateSchemeTypeMappingListViewModel
    {
        public long MappingId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public long SchemeMasterId { get; set; }
        public string SchemeName { get; set; }
        public long SchemeTypeMasterId { get; set; }
        public string SchemeTypeName { get; set; }
        public decimal MinimumDepositionRequired { get; set; }
        public long DepositWalletTypeId { get; set; }
        public string DepositWalletTypeName { get; set; }
        public int CommissionTypeInterval { get; set; }
        public string Description { get; set; }
        public long CommissionHour { get; set; }
        public int Status { get; set; }
    }
    public class AffiliateSchemeTypeMappingStatusViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,31035")]
        public long MappingId { get; set; }
    }
    public class GetByIdSchemeTypeMapping : BizResponseClass
    {
        public AffiliateSchemeTypeMappingListViewModel Data { get; set; }
    }
    public class AffiliateSchemeTypeMappingListResponse : BizResponseClass
    {
        public List<AffiliateSchemeTypeMappingListViewModel> AffiliateSchemeTypeMappingList { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class AffiliateSchemeTypeMappingObjResponse : BizResponseClass
    {
        public AffiliateSchemeTypeMappingUpdateViewModel AffiliateSchemeTypeMappingObj { get; set; }
    }
    public class SchemeTypeMappingChangeStatusViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,31035")]
        public long MappingId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31026")]
        [EnumDataType(typeof(ServiceStatus), ErrorMessage = "1,Invalid Required Parameter,31034")]
        public ServiceStatus Status { get; set; }
    }
}
