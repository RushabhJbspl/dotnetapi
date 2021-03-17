using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.ControlPanel
{
    public class ChargeConfigurationDetailReq
    {
        [Required(ErrorMessage = "1,Enter Required Parameters,4459")]
        public long ChargeConfigurationMasterID { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4460")]
        [EnumDataType(typeof(EnChargeDistributionType), ErrorMessage = "1,Fail,4321")]
        public EnChargeDistributionType ChargeDistributionBasedOn { get; set; }//2.(1- Regular,2 - Volume ,3 - Day end balance)

        [Required(ErrorMessage = "1,Enter Required Parameters,4462")]
        public EnConfigChargeType ChargeType { get; set; }//(1.Regular,2.Recurring , fk)

        [Required(ErrorMessage = "1,Enter Required Parameters,4461")]
        public long DeductionWalletTypeId { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4271")]
        public decimal ChargeValue { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4463")]
        public enChargeType ChargeValueType { get; set; }//(1.Fixed,2.Percentage)

        [Required(ErrorMessage = "1,Enter Required Parameters,4408")]
        public decimal MakerCharge { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4409")]
        public decimal TakerCharge { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4464")]
        public decimal MinAmount { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4465")]
        public decimal MaxAmount { get; set; }

        public string Remarks { get; set; }

        public short? Status { get; set; }
    }

    public class ChargeConfigurationDetailArbitrageReq
    {
        [Required(ErrorMessage = "1,Enter Required Parameters,4459")]
        public long ChargeConfigurationMasterID { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4460")]
        [EnumDataType(typeof(EnChargeDistributionType), ErrorMessage = "1,Fail,4321")]
        public EnChargeDistributionType ChargeDistributionBasedOn { get; set; }//2.(1- Regular,2 - Volume ,3 - Day end balance)

        [Required(ErrorMessage = "1,Enter Required Parameters,4462")]
        public EnConfigChargeType ChargeType { get; set; }//(1.Regular,2.Recurring , fk)

        [Required(ErrorMessage = "1,Enter Required Parameters,4461")]
        public long DeductionWalletTypeId { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4271")]
        public decimal ChargeValue { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4463")]
        public enChargeType ChargeValueType { get; set; }//(1.Fixed,2.Percentage)

        [Required(ErrorMessage = "1,Enter Required Parameters,4408")]
        public decimal MakerCharge { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4409")]
        public decimal TakerCharge { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4464")]
        public decimal MinAmount { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4465")]
        public decimal MaxAmount { get; set; }

        public string Remarks { get; set; }

        public short? Status { get; set; }
    }

    public class ChargeConfigurationMasterReq
    {
        [Required(ErrorMessage = "1,Enter Required Parameters,4248")]
        public long WalletTypeID { get; set; }//fk

        [Required(ErrorMessage = "1,Enter Required Parameters,4263")]
        [EnumDataType(typeof(enWalletTrnType), ErrorMessage = "1,Fail,4321")]
        public long TrnType { get; set; }//EnWalletTrnType

        [Required(ErrorMessage = "1,Enter Required Parameters,4458")]
        public short KYCComplaint { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4435")]
        [EnumDataType(typeof(EnStakingSlabType), ErrorMessage = "1,Fail,4396")]
        public short SlabType { get; set; } //EnStakingSlabType

        [Required(ErrorMessage = "1,Enter Required Parameters,4457")]
        public long SpecialChargeConfigurationID { get; set; }//fk

        public string Remarks { get; set; }

        public short? Status { get; set; }
    }

    public class ChargeConfigurationMasterReq2
    {
        [Required(ErrorMessage = "1,Enter Required Parameters,4435")]
        [EnumDataType(typeof(EnStakingSlabType), ErrorMessage = "1,Fail,4396")]
        public short SlabType { get; set; } //EnStakingSlabType

        public string Remarks { get; set; }

        public short? Status { get; set; }
    }

    public class ChargeConfigurationMasterRes
    {
        public long Id { get; set; }
        public long WalletTypeID { get; set; }//fk
        public string WalletTypeName { get; set; }

        public long TrnType { get; set; }//EnWalletTrnType
        public string TrnTypeName { get; set; }
        public short KYCComplaint { get; set; }
        public string StrKYCComplaint { get; set; }

        public short SlabType { get; set; } //EnStakingSlabType
        public string StrSlabType { get; set; }
        public long SpecialChargeConfigurationID { get; set; }//fk

        public string Remarks { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
    }

    public class ListChargeConfigurationMasterRes : BizResponseClass
    {
        public List<ChargeConfigurationMasterRes> Details { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public class ChargeConfigurationMasterRes2 : BizResponseClass
    {
        public ChargeConfigurationMasterRes Details { get; set; }
    }

    public class ChargeConfigurationDetailRes
    {
        public long ChargeConfigDetailId { get; set; }
        public long ChargeConfigurationMasterID { get; set; }
        public short ChargeDistributionBasedOn { get; set; }//2.(1- Regular,2 - Volume ,3 - Day end balance)
        public string StrChargeDistributionBasedOn { get; set; }
        public long ChargeType { get; set; }//(1.Regular,2.Recurring , fk)
        public string StrChargeType { get; set; }
        public long DeductionWalletTypeId { get; set; }
        public string WalletTypeName { get; set; }
        public decimal ChargeValue { get; set; }
        public short ChargeValueType { get; set; }//(1.Fixed,2.Percentage)
        public string StrChargeValueType { get; set; }
        public decimal MakerCharge { get; set; }
        public decimal TakerCharge { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public string Remarks { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
    }

    public class ChargeConfigurationDetailArbitrageRes
    {
        public long ChargeConfigDetailId { get; set; }
        public long ChargeConfigurationMasterID { get; set; }
        public short ChargeDistributionBasedOn { get; set; }//2.(1- Regular,2 - Volume ,3 - Day end balance)
        public string StrChargeDistributionBasedOn { get; set; }
        public long ChargeType { get; set; }//(1.Regular,2.Recurring , fk)
        public string StrChargeType { get; set; }
        public long DeductionWalletTypeId { get; set; }
        public string WalletTypeName { get; set; }
        public decimal ChargeValue { get; set; }
        public short ChargeValueType { get; set; }//(1.Fixed,2.Percentage)
        public string StrChargeValueType { get; set; }
        public decimal MakerCharge { get; set; }
        public decimal TakerCharge { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public string Remarks { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
    }

    public class ArbitrageChargeConfigurationDetailRes
    {
        public long ChargeConfigDetailId { get; set; }
        public long ChargeConfigurationMasterID { get; set; }
        public short ChargeDistributionBasedOn { get; set; }//2.(1- Regular,2 - Volume ,3 - Day end balance)
        public string StrChargeDistributionBasedOn { get; set; }
        public long ChargeType { get; set; }//(1.Regular,2.Recurring , fk)
        public string StrChargeType { get; set; }
        public long DeductionWalletTypeId { get; set; }
        public string WalletTypeName { get; set; }
        public decimal ChargeValue { get; set; }
        public short ChargeValueType { get; set; }//(1.Fixed,2.Percentage)
        public string StrChargeValueType { get; set; }
        public decimal MakerCharge { get; set; }
        public decimal TakerCharge { get; set; }
        public string Remarks { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public Int16 IsCurrencyConverted { get; set; }
    }

    public class ListChargeConfigurationDetailRes : BizResponseClass
    {
        public List<ChargeConfigurationDetailRes> Details { get; set; }
    }

    public class ListChargeConfigurationDetailArbitrageRes : BizResponseClass
    {
        public List<ChargeConfigurationDetailArbitrageRes> Details { get; set; }
        public int? PageNo { get; set; }
        public int? PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public class ListArbitrageChargeConfigurationDetailRes : BizResponseClass
    {
        public List<ArbitrageChargeConfigurationDetailRes> Details { get; set; }
        public int? PageNo { get; set; }
        public int? PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public class ChargeConfigurationDetailRes2 : BizResponseClass
    {
        public ChargeConfigurationDetailRes Details { get; set; }
    }

    public class ChargeConfigurationDetailArbitrageRes2 : BizResponseClass
    {
        public ChargeConfigurationDetailArbitrageRes Details { get; set; }
    }

    public class LeaverageReportRes
    {
        public long Id { get; set; }
        public long WalletTypeID { get; set; }
        public string WalletTypeName { get; set; }
        public long FromWalletID { get; set; }
        public string FromWalletName { get; set; }
        public long ToWalletID { get; set; }
        public string ToWalletName { get; set; }
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public long LeverageID { get; set; }
        public short IsAutoApprove { get; set; }
        public string RequestRemarks { get; set; }
        public decimal Amount { get; set; }
        public DateTime TrnDate { get; set; }
        public decimal LeverageAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public decimal SafetyMarginAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string SystemRemarks { get; set; }
        public int Status { get; set; }
        public string StrStatus { get; set; }
        public decimal LeveragePer { get; set; }
        public decimal MaxLeverage { get; set; }//15-05-2019
    }

    public class LeaverageReportResv2
    {
        public string Id { get; set; }
        public long WalletTypeID { get; set; }
        public string WalletTypeName { get; set; }
        public long FromWalletID { get; set; }
        public string FromWalletName { get; set; }
        public long ToWalletID { get; set; }
        public string ToWalletName { get; set; }
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public long LeverageID { get; set; }
        public short IsAutoApprove { get; set; }
        public string RequestRemarks { get; set; }
        public decimal Amount { get; set; }
        public DateTime TrnDate { get; set; }
        public decimal LeverageAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public decimal SafetyMarginAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string SystemRemarks { get; set; }
        public int Status { get; set; }
        public string StrStatus { get; set; }
        public decimal LeveragePer { get; set; }
        public decimal MaxLeverage { get; set; }//15-05-2019
    }

    public class ListLeaverageReportRes : BizResponseClass
    {
        public List<LeaverageReportRes> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
    public class ListLeaverageReportResv2 : BizResponseClass
    {
        public List<LeaverageReportResv2> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class LeaverageReport
    {
        public long Id { get; set; }
        public long WalletTypeID { get; set; }
        public string WalletTypeName { get; set; }
        public long FromWalletID { get; set; }
        public string FromWalletName { get; set; }
        public long ToWalletID { get; set; }
        public string ToWalletName { get; set; }
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public long LeverageID { get; set; }
        public short IsAutoApprove { get; set; }
        public string RequestRemarks { get; set; }
        public decimal Amount { get; set; }
        public DateTime TrnDate { get; set; }
        public decimal LeverageAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public decimal SafetyMarginAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string SystemRemarks { get; set; }
        public int Status { get; set; }
        public string StrStatus { get; set; }
        public decimal LeveragePer { get; set; }
        public long? ApprovedBy { get; set; }
        public string ApprovedByUserName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public decimal MaxLeverage { get; set; }
    }
    public class LeaverageReportv2
    {
        public string Id { get; set; }
        public long WalletTypeID { get; set; }
        public string WalletTypeName { get; set; }
        public long FromWalletID { get; set; }
        public string FromWalletName { get; set; }
        public long ToWalletID { get; set; }
        public string ToWalletName { get; set; }
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public long LeverageID { get; set; }
        public short IsAutoApprove { get; set; }
        public string RequestRemarks { get; set; }
        public decimal Amount { get; set; }
        public DateTime TrnDate { get; set; }
        public decimal LeverageAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public decimal SafetyMarginAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string SystemRemarks { get; set; }
        public int Status { get; set; }
        public string StrStatus { get; set; }
        public decimal LeveragePer { get; set; }
        public long? ApprovedBy { get; set; }
        public string ApprovedByUserName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public decimal MaxLeverage { get; set; }
    }

    public class ListLeaverageRes : BizResponseClass
    {
        public List<LeaverageReport> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class ListLeaverageResv2 : BizResponseClass
    {
        public List<LeaverageReportv2> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
    public class OpenPositionRes : BizResponseClass
    {
        public List<OpenPosition> Data { get; set; }
    }
    public class OpenPosition
    {
        public long MasterID { get; set; }
        public DateTime TrnDate { get; set; }
        public long PairID { get; set; }
        public string PairName { get; set; }
        public long UserID { get; set; }
        public string UserName { get; set; }
        [NotMapped]
        public List<OpenPositionDetailVM> DetailedData { get; set; }
    }
    public class PNLAccountRes : BizResponseClass
    {
        public List<PNLAccount> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
    public class PNLAccount
    {
        public int ID { get; set; }
        public long TrnNo { get; set; }
        public long OpenPositionMasterID { get; set; }
        public string UserName { get; set; }
        public decimal ProfitAmount { get; set; }
        public decimal AvgLandingBuy { get; set; }
        public decimal AvgLandingSell { get; set; }
        public decimal SettledQty { get; set; }
        public string ProfitCurrencyName { get; set; }
        public DateTime CreatedDate { get; set; }
        [NotMapped]
        public List<OpenPositionDetailVM> DetailedData { get; set; }
    }

    public class OpenPositionDetailVM
    {
        public long ID { get; set; }
        public DateTime TrnDate { get; set; }
        public long TrnNo { get; set; }
        public long PairID { get; set; }
        public string PairName { get; set; }
        public string OrderType { get; set; } //Buy or sell
        public decimal Qty { get; set; }
        public decimal BidPrice { get; set; }
        public decimal LandingPrice { get; set; }
        public string BaseCurrency { get; set; }
        public string SecondCurrency { get; set; }
    }

    public class PNLQueryData
    {
        public long ID { get; set; }
        public long TrnNo { get; set; }
        public long OpenPositionMasterID { get; set; }
        public string UserName { get; set; }
        public decimal ProfitAmount { get; set; }
        public decimal AvgLandingBuy { get; set; }
        public decimal AvgLandingSell { get; set; }
        public decimal SettledQty { get; set; }
        public string CurrencyName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
