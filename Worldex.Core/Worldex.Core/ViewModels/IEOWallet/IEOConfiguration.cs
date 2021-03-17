using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.IEOWallet
{
    public class IEOConfiguration
    {
    }

    public class IEOBannerRequest
    {
        public long Id { get; set; }
        [Required]
        public string BannerPath { get; set; }
        [Required]
        [StringLength(50)]
        public string BannerName { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string TermsAndCondition { get; set; }
        [Required]
        public short IsKYCReuired { get; set; }//1-required,2-not
        [Required]
        public short Status { get; set; }
    }
    public class GetIEOBannerRes : ApiModels.BizResponseClass
    {
        public long Id { get; set; }
        public string GUID { get; set; }
        public string BannerPath { get; set; }
        public string BannerName { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public string TermsAndCondition { get; set; }
        public short IsKYCReuired { get; set; }//1-required,2-not
        public short Status { get; set; }
    }

    public class IEOAdminWalletRequest
    {
        public long Id { get; set; }
        [Required]
        public string WalletPath { get; set; }
        [Required]
        [StringLength(50)]
        public string ShortCode { get; set; }
        [Required]
        public string CoinType { get; set; }
        [Required]
        public string WalletName { get; set; }
        [Required]
        public short Status { get; set; }
        [Required]
        public short Rounds { get; set; }
        public decimal Rate { get; set; }
        public string Description { get; set; }
    }

    public class GetIEOAdminWalletRes
    {
        public long Id { get; set; }
        public string WalletPath { get; set; }
        public string ShortCode { get; set; }
        public string CoinType { get; set; }
        public string WalletName { get; set; }
        public string Description { get; set; }
        public short Status { get; set; }
        public short Rounds { get; set; }
        public string AdminWalletId { get; set; }
        public string Balance { get; set; }
        public decimal Rate { get; set; }
    }

    public class ListGetIEOAdminWalletRes : BizResponseClass
    {
        public List<GetIEOAdminWalletRes> Data { get; set; }
    }

    public class InsertRoundConfigurationReq
    {
        //public long Id { get; set; }
        //public string BGPath { get; set; }
        [Required(ErrorMessage = "1,Please Enter IEOCurrencyId,17220")]
        public long IEOCurrencyId { get; set; }
        public List<ExchangeCurrencyDetail> ExchangeCurrency { get; set; }
        [Required(ErrorMessage = "1,Please Enter MinLimit,17221")]
        public decimal MinLimit { get; set; }
        [Required(ErrorMessage = "1,Please Enter MaxLimit,17222")]
        public decimal MaxLimit { get; set; }
        [Required(ErrorMessage = "1,Please Enter MaxOccurence,17223")]
        public short MaxOccurence { get; set; }
        [Required(ErrorMessage = "1,Please Enter FromDate,17224")]
        public DateTime FromDate { get; set; }
        [Required(ErrorMessage = "1,Please Enter ToDate,17225")]
        public DateTime ToDate { get; set; }
        [Required(ErrorMessage = "1,Please Enter TotalSupply,17226")]
        public decimal TotalSupply { get; set; }
        public short Status { get; set; }
        [Required(ErrorMessage = "1,Please Enter Bonus,17228")]
        public decimal Bonus { get; set; }
        //public short IsUSD { get; set; }//0-off-convert using purchase rate,1-on convert using market capp
        public List<AllocationDetailUpdate> AllocationDetail { get; set; }
    }
    public class AllocationDetail
    {
        [Required(ErrorMessage = "1,Please Enter AllocationPercentage,17227")]
        public decimal AllocationPercentage { get; set; }
        [Required(ErrorMessage = "1,Please Enter Bonus,17228")]
        public decimal Bonus { get; set; }
        [Required(ErrorMessage = "1,Please Enter AllocationPeriodType,17229")]
        public short AllocationPeriodType { get; set; }
        [Required(ErrorMessage = "1,Please Enter AllocationNoofPeriod,17230")]
        public short AllocationNoofPeriod { get; set; }
    }
    public class ExchangeCurrencyDetail
    {
        [Required(ErrorMessage = "1,Please Enter PaidCurrencyId,17233")]
        public long PaidCurrencyId { get; set; }
        [Required(ErrorMessage = "1,Please Enter Rate,17234")]
        public decimal Rate { get; set; }
        public short IsUSD { get; set; }//0-off-convert using purchase rate,1-on convert using market capp
    }

    public class UpdateRoundConfigurationReq
    {
        [Required(ErrorMessage = "1,Please Enter RoundId,17219")]
        public string RoundId { get; set; }
        [Required(ErrorMessage = "1,Please Enter IEOCurrencyId,17220")]
        public long IEOCurrencyId { get; set; }
        public List<ExchangeCurrencyDetailUpdate> ExchangeCurrency { get; set; }
        [Required(ErrorMessage = "1,Please Enter MinLimit,17221")]
        public decimal MinLimit { get; set; }
        [Required(ErrorMessage = "1,Please Enter MaxLimit,17222")]
        public decimal MaxLimit { get; set; }
        [Required(ErrorMessage = "1,Please Enter MaxOccurence,17223")]
        public short MaxOccurence { get; set; }
        [Required(ErrorMessage = "1,Please Enter FromDate,17224")]
        public DateTime FromDate { get; set; }
        [Required(ErrorMessage = "1,Please Enter ToDate,17225")]
        public DateTime ToDate { get; set; }
        [Required(ErrorMessage = "1,Please Enter TotalSupply,17226")]
        public decimal TotalSupply { get; set; }
        [Required(ErrorMessage = "1,Please Enter Bonus,17228")]
        public decimal Bonus { get; set; }
        public short Status { get; set; }
        public List<AllocationDetailUpdate> AllocationDetail { get; set; }
    }
    public class AllocationDetailUpdate
    {
        [Required(ErrorMessage = "1,Please Enter DetailId,17231")]
        public string DetailId { get; set; }
        [Required(ErrorMessage = "1,Please Enter AllocationPercentage,17227")]
        public decimal AllocationPercentage { get; set; }
        [Required(ErrorMessage = "1,Please Enter Bonus,17228")]
        public decimal Bonus { get; set; }
        [Required(ErrorMessage = "1,Please Enter AllocationPeriodType,17229")]
        public short AllocationPeriodType { get; set; }
        [Required(ErrorMessage = "1,Please Enter AllocationNoofPeriod,17230")]
        public short AllocationNoofPeriod { get; set; }
    }
    public class ExchangeCurrencyDetailUpdate
    {
        [Required(ErrorMessage = "1,Please Enter ExchangeId,17232")]
        public string ExchangeId { get; set; }
        [Required(ErrorMessage = "1,Please Enter PaidCurrencyId,17233")]
        public long PaidCurrencyId { get; set; }
        [Required(ErrorMessage = "1,Please Enter Rate,17234")]
        public decimal Rate { get; set; }
        public short IsUSD { get; set; }//0-off-convert using purchase rate,1-on convert using market capp
    }

    public class ListRoundConfigurationResponse : BizResponseClass
    {
        public List<IEORoundResponse> RoundDetails { get; set; }
    }

    public class IEORoundResponse
    {
        public long CurrencyID { get; set; }
        public string CurrencyName { get; set; }
        public string RoundId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public Int16 Status { get; set; }
        public string StatusStr { get; set; }
        public decimal TotalSupply { get; set; }
        public decimal MinLimit { get; set; }
        public decimal MaxLimit { get; set; }
        public Int16 MaxOccurence { get; set; }
        public string BGPath { get; set; }
        public decimal Bonus { get; set; }
        [NotMapped]
        public List<AllocationBack> Allocation { get; set; }
        [NotMapped]
        public List<PurchaseWalletsBack> PurchaseWallets { get; set; }
    }

    public class PurchaseWalletsBack
    {
        public string ExchangeId { get; set; }
        public long PaidCurrencyId { get; set; }
        public string PaidWalletName { get; set; }
        public Int16 CurrencyConvertType { get; set; }
        public string CurrencyConvertTypeName { get; set; }
        public decimal Rate { get; set; }
    }

    public class AllocationBack
    {
        public string DetailId { get; set; }
        public decimal AllocationPercentage { get; set; }
        public decimal Bonus { get; set; }
        public Int64 AllocationNoofPeriod { get; set; }
        public Int16 AllocationPeriodType { get; set; }
        public string AllocationPeriodTypeStr { get; set; }
    }

    public class IEOAdminWalletCreditReq
    {
        [Required(ErrorMessage = "1,Please Enter WalletTypeName,17235")]
        public string WalletTypeName { get; set; }
        [Required(ErrorMessage = "1,Please Enter AdminWalletId,17236")]
        public string AdminWalletId { get; set; }
        [Required(ErrorMessage = "1,Please Enter Amount,17237")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Amount,17239"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "1,Please Enter Remarks,17238")]
        public string Remarks { get; set; }
    }

    public class TokenCountRes
    {
       public string PaidCurrency { get; set; }
       public string DeliveredCurrency { get; set; }
       public string QuantityTotal { get; set; }
    }

    public class ListTokenCountRes : BizResponseClass
    {
        public List<TokenCountRes> Data { get; set; }
    }

    public class AllocateTokenCountRes
    {
        public string DeliveredCurrency { get; set; }
        public string QuantityTotal { get; set; }
    }

    public class ListAllocateTokenCountRes : BizResponseClass
    {
        public List<AllocateTokenCountRes> Data { get; set; }
    }

    public class IEOTokenReportDataRes
    {
        public string TrnRefNo { get; set; }
        public string PaidCurrency { get; set; }
        public string DeliveredCurrency { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public string Email { get; set; }
        public string DeliveredQuantity { get; set; }
        public string PaidQuantity { get; set; }
        public string MaximumDeliveredQuantiy { get; set; }
        public string Rate { get; set; }
        public DateTime Date { get; set; }
        public string Remarks { get; set; }
    }

    public class ListIEOTokenReportDataRes : BizResponseClass
    {
        public List<IEOTokenReportDataRes> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class IEOAllocatedTokenReportDataRes
    {
        public string TrnRefNo { get; set; }
        public string PaidCurrency { get; set; }
        public string DeliveredCurrency { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public string Email { get; set; }
        public string DeliveredQuantity { get; set; }
        public string PaidQuantity { get; set; }
        public string MaximumDeliveredQuantiy { get; set; }
        public string Rate { get; set; }
        public DateTime Date { get; set; }
        public string Remarks { get; set; }
    }

    public class ListIEOAllocatedTokenReportDataRes : BizResponseClass
    {
        public List<IEOAllocatedTokenReportDataRes> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}
