using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.BackOfficeAffiliate
{
    public class AffiliateSchemeMasterReqRes
    {
        public long? SchemeMasterId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31024")]
        [StringLength(100,ErrorMessage = "1,Invalid Length,31059")]
        public string SchemeName { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31025")]
        [StringLength(100, ErrorMessage = "1,Invalid Length,31060")]
        public string SchemeType { get; set; }

        public ServiceStatus? Status { get; set; }
    }

    public class ChangeAffiliateSchemeStatus
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,31028")]
        public long SchemeMasterId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31026")]
        public ServiceStatus Status { get; set; }
    }

    public class GetAffiliateSchemeMasterRes : BizResponseClass
    {
        public GetAllAffiliateSchemeMasterRes Data { get; set; }
    }

    public class GetAllAffiliateSchemeMasterRes
    {
        public long SchemeMasterId { get; set; }

        public string SchemeName { get; set; }

        public string SchemeType { get; set; }

        public short Status { get; set; }
    }

    public class ListAffiliateSchemeMasterRes : BizResponseClass
    {
        public List<GetAllAffiliateSchemeMasterRes> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class AffiliateSchemeTypeMasterReq
    {
        public long? SchemeTypeId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31027")]
        [StringLength(100,ErrorMessage ="1,Invalid Length,31058")]
        public string SchemeTypeName { get; set; }

        [StringLength(250, ErrorMessage = "1,Invalid Length,31057")]
        public string Description { get; set; }

        public ServiceStatus? Status { get; set; }
    }

    public class ChangeAffiliateSchemeTypeStatus
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,31023")]
        public long SchemeTypeId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31026")]
        public ServiceStatus Status { get; set; }
    }

    public class GetAffiliateSchemeTypeMasterRes : BizResponseClass
    {
        public GetAllAffiliateSchemeTypeMasterRes Data { get; set; }
    }

    public class GetAllAffiliateSchemeTypeMasterRes
    {
        public long SchemeTypeId { get; set; }

        public string SchemeTypeName { get; set; }

        public string Description { get; set; }

        public short Status { get; set; }
    }

    public class ListAffiliateSchemeTypeMasterRes : BizResponseClass
    {
        public List<GetAllAffiliateSchemeTypeMasterRes> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
       
    public class AffiliatePromotionMasterReq
    {
        public long? PromotionId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31037")]
        [StringLength(100, ErrorMessage = "1,Invalid Length,31061")]
        public string PromotionType { get; set; }

        public ServiceStatus? Status { get; set; }
    }

    public class ChangeAffiliatePromotionStatus
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,31038")]
        public long PromotionId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31039")]
        public ServiceStatus Status { get; set; }
    }

    public class GetAffiliatePromotionMasterRes : BizResponseClass
    {
        public GetAllAffiliatePromotionMasterRes Data { get; set; }
    }

    public class GetAllAffiliatePromotionMasterRes
    {
        public long PromotionId { get; set; }

        public string PromotionType { get; set; }

        public short Status { get; set; }
    }

    public class ListAllAffiliatePromotionMasterRes : BizResponseClass
    {
        public List<GetAllAffiliatePromotionMasterRes> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class GetAffiliateShemeDetailRes
    {
        public long DetailId { get; set; }
        public long SchemeMasterId { get; set; }
        public long SchemeMappingId { get; set; }  // Reference From  SchemeTypeMapping    
        public string SchemeMappingName { get; set; }  // Reference From  SchemeTypeMapping    
        public int Level { get; set; }
        public decimal MinimumValue { get; set; }
        public decimal MaximumValue { get; set; }
        public long CreditWalletTypeId { get; set; } // Reference From  WalletTypeMasters
        public string CreditWalletTypeName { get; set; } // Reference From  WalletTypeMasters
        public int CommissionType { get; set; } // EnAffiCommissionType
        public string CommissionTypeName { get; set; } // EnAffiCommissionType
        public decimal CommissionValue { get; set; }
        public int DistributionType { get; set; } // EnAffiDistributedType
        public string DistributionTypeName { get; set; } // EnAffiDistributedType
        public long TrnWalletTypeId { get; set; }
        public string TrnWalletTypeName { get; set; }
        public short Status { get; set; }
    }

    public class ListAffiliateShemeDetailRes : BizResponseClass
    {
        public List<GetAffiliateShemeDetailRes> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class GetAffiliateShemeDetailResById : BizResponseClass
    {
        public GetAffiliateShemeDetailRes Data { get; set; }
    }

    public class ChangeAffiliateSchemeDetailStatus
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,31041")]
        public long DetailId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31039")]
        public ServiceStatus Status { get; set; }
    }

    public class AffiliateShemeDetailReq
    {
        public long? DetailId { get; set; }

        [Required(ErrorMessage ="1,Please Enter Required Parameter,31043")]
        public long SchemeMappingId { get; set; }  // Reference From  SchemeTypeMapping

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31044")]
        public int Level { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31045")]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]        
        public decimal MinimumValue { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31046")]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal MaximumValue { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31047")]
        public long CreditWalletTypeId { get; set; } // Reference From  WalletTypeMasters

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31048")]
        public int CommissionType { get; set; } // EnAffiCommissionType

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31049")]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]        
        public decimal CommissionValue { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31050")]
        public int DistributionType { get; set; } // EnAffiDistributedType

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31051")]
        public long TrnWalletTypeId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,31039")]
        public ServiceStatus Status { get; set; }
    }

}
