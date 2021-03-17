using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class APIPlanMasterViewModel
    {
        public long? ID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11077")]
        [StringLength(50)]
        public string PlanName { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Price { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Charge { get; set; }
        public string PlanDesc { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11085")]
        public int Priority { get; set; }
        public long MaxPerMinute { get; set; }
        public long MaxPerDay { get; set; }
        public long MaxPerMonth { get; set; }
        public long MaxOrderPerSec { get; set; }
        public long MaxRecPerRequest { get; set; }
        public long MaxReqSize { get; set; }
        public long MaxResSize { get; set; }
        public int WhitelistedEndPoints { get; set; }
        public int ConcurrentEndPoints { get; set; }
        public int HistoricalDataMonth { get; set; }
        public short IsPlanRecursive { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11086")]
        public int PlanValidity { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11087")]
        public int PlanValidityType { get; set; }
        public List<long> ReadonlyAPI { get; set; }
        public List<long> FullAccessAPI { get; set; }
        public short Status { get; set; }
        public string CreatedIPAddress { get; set; }
        public long ServiceID { get; set; }
    }
    public class APIPlanMasterRequest : APIPlanMasterViewModel
    {

    }
    public class APIPlanMasterResponse : BizResponseClass
    {
        public List<APIPlanMasterResponseInfo> Response { get; set; }
    }
    public class APIPlanMasterResponseV2 : BizResponseClass
    {
        public List<APIPlanMasterResponseInfoV2> Response { get; set; }
    }
    public class APIPlanMasterResponseInfoV2
    {
        public long ID { get; set; }
        public string PlanName { get; set; }
        public short Status { get; set; }
    }
    public class APIPlanMasterResponseInfo
    {
        public long ID { get; set; }
        public string PlanName { get; set; }
        public string PlanDesc { get; set; }
        public decimal Price { get; set; }
        public decimal Charge { get; set; }
        public int PlanValidity { get; set; }
        public int Priority { get; set; }
        public short Status { get; set; }
        public short IsPlanRecursive { get; set; }
        public int ConcurrentEndPoints { get; set; }
        public int HistoricalDataMonth { get; set; }
        public int Whitelistendpoints { get; set; }
        public long MaxPerDay { get; set; }
        public long MaxOrderPerSec { get; set; }
        public long MaxPerMinute { get; set; }
        public long MaxPerMonth { get; set; }
        public long MaxRecPerRequest { get; set; }
        public long MaxReqSize { get; set; }
        public long MaxResSize { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int PlanValidityType { get; set; }
        public string CreatedIPAddress { get; set; }
        public long ServiceID { get; set; }
        public Dictionary<long,String> ReadOnlyAPI { get; set; }
        public Dictionary<long,String> FullAccessAPI { get; set; }
    }
    public class ViewAPIPlanDetailQryResponseV2
    {
        public long Id { get; set; }
        public string PlanName { get; set; }
        public int PlanValidity { get; set; }
        public decimal Price { get; set; }
        public decimal Charge { get; set; }
        public short PlanStatus { get; set; }
        public long PlanDetailID { get; set; }
        public int ConcurrentEndPoints { get; set; }
        public int HistoricalDataMonth { get; set; }
        public long MaxPerDay { get; set; }
        public long MaxOrderPerSec { get; set; }
        public long MaxPerMinute { get; set; }
        public long MaxPerMonth { get; set; }
        public long MaxRecPerRequest { get; set; }
        public long MaxReqSize { get; set; }
        public long MaxResSize { get; set; }
        public short PlanDetailStatus { get; set; }
        public int Priority { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        
    }
    
    public class APIPlanMasterRequestV1 : APIPlanMasterViewModelV1
    {

    }
    public class APIPlanMasterViewModelV1
    {
        public long? ID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11077")]
        [StringLength(50)]
        public string PlanName { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Price { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Charge { get; set; }
        public string PlanDesc { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11085")]
        public int Priority { get; set; }
        public long MaxPerMinute { get; set; }
        public long MaxPerDay { get; set; }
        public long MaxPerMonth { get; set; }
        public long MaxOrderPerSec { get; set; }
        public long MaxRecPerRequest { get; set; }
        public long MaxReqSize { get; set; }
        public long MaxResSize { get; set; }
        public int WhitelistedEndPoints { get; set; }
        public int ConcurrentEndPoints { get; set; }
        public int HistoricalDataMonth { get; set; }
        public short IsPlanRecursive { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11086")]
        public int PlanValidity { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11087")]
        public int PlanValidityType { get; set; }
        public List<long> ReadonlyAPI { get; set; }
        public List<long> FullAccessAPI { get; set; }
        public string CreatedIPAddress { get; set; }
        public long ServiceID { get; set; }
    }
}
