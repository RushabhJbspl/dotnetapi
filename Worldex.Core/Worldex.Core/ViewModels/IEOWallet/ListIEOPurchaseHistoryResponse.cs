using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.IEOWallet
{
    public class ListIEOPurchaseHistoryResponseBO : BizResponseClass
    {
        public List<IEOPurchaseHistoryResponseBO> PurchaseHistory { get; set; }
        public long TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class IEOPurchaseHistoryResponseBO
    {
        public Int64 Id { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal PaidQuantity { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal DeliveredQuantity { get; set; }
        public string PaidCurrency { get; set; }
        public string DeliveredCurrency { get; set; }
        public Int64 RoundID { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal InstantQuantity { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal InstantQuantityPer { get; set; }
        public DateTime TrnDate { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Rate { get; set; }
        public string Email { get; set; }
        public decimal BonusPercentage { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal MaxDeliveryQuantity { get; set; }
        public decimal MaxDeliveryQuantityWOBonus { get; set; }
        [NotMapped]
        public List<IEOCronMasterResponse> IEOCronMasterList { get; set; }
    }

    public class ListIEOPurchaseHistoryResponse : BizResponseClass
    {
        public List<IEOPurchaseHistoryResponse> PurchaseHistory { get; set; }
        public long TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class IEOPurchaseHistoryResponse
    {
        public Int64 Id { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal PaidQuantity { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal DeliveredQuantity { get; set; }
        public string PaidCurrency { get; set; }
        public string DeliveredCurrency { get; set; }
        public Int64 RoundID { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal InstantQuantity { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal InstantQuantityPer { get; set; }
        public DateTime TrnDate { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Rate { get; set; }
        public decimal BonusPercentage { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal MaxDeliveryQuantity { get; set; }
        public decimal MaxDeliveryQuantityWOBonus { get; set; }
        [NotMapped]
        public List<IEOCronMasterResponse> IEOCronMasterList { get; set; }
    }

    public class IEOCronMasterResponse
    {
        public DateTime MaturityDate { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal DeliveryQuantity { get; set; }
        public string DeliveryCurrency { get; set; }
        public string CMGUID { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal SlabPercentage { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
    }
}
