using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.WalletOperations
{
    public class StakingPolicyDetailRes
    {
        public long PolicyDetailID { get; set; }
        public short StakingType { get; set; }
        public string StakingTypeName { get; set; }
        public short SlabType { get; set; }
        public string SlabTypeName { get; set; }
        public long WalletTypeID { get; set; }
        public string StakingCurrency { get; set; }
        public short DurationWeek { get; set; }
        public short DurationMonth { get; set; }
        public short InterestType { get; set; }
        public string InterestTypeName { get; set; }
        public string AvailableAmount { get; set; }
        public decimal InterestValue { get; set; }
        public long MaturityCurrencyID { get; set; } // InterestWalletTypeID
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        //public long MaturityCurrencyId { get; set; }
        public string MaturityCurrencyName { get; set; }
        public decimal MakerCharges { get; set; }
        public decimal TakerCharges { get; set; }
        public short EnableAutoUnstaking { get; set; }
        public string StrEnableAutoUnstaking { get; set; }
        public short EnableStakingBeforeMaturity { get; set; }
        public string StrEnableStakingBeforeMaturity { get; set; }
        public decimal EnableStakingBeforeMaturityCharge { get; set; }
        public short RenewUnstakingEnable { get; set; }
        public short RenewUnstakingPeriod { get; set; }
        public string StrRenewUnstakingEnable { get; set; }
        public short Status { get; set; }
        //SPD.RenewUnstakingEnable,SPD.RenewUnstakingPeriod
    }

    public class StakingPolicyDetailResV2
    {
        public string PolicyDetailID { get; set; }
        //public long PolicyDetailID { get; set; }
        public short StakingType { get; set; }
        public string StakingTypeName { get; set; }
        public short SlabType { get; set; }
        public string SlabTypeName { get; set; }
        public long WalletTypeID { get; set; }
        public string StakingCurrency { get; set; }
        public short DurationWeek { get; set; }
        public short DurationMonth { get; set; }
        public short InterestType { get; set; }
        public string InterestTypeName { get; set; }
        public string AvailableAmount { get; set; }
        public decimal InterestValue { get; set; }
        public long MaturityCurrencyID { get; set; } // InterestWalletTypeID
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        //public long MaturityCurrencyId { get; set; }
        public string MaturityCurrencyName { get; set; }
        public decimal MakerCharges { get; set; }
        public decimal TakerCharges { get; set; }
        public short EnableAutoUnstaking { get; set; }
        public string StrEnableAutoUnstaking { get; set; }
        public short EnableStakingBeforeMaturity { get; set; }
        public string StrEnableStakingBeforeMaturity { get; set; }
        public decimal EnableStakingBeforeMaturityCharge { get; set; }
        public short RenewUnstakingEnable { get; set; }
        public short RenewUnstakingPeriod { get; set; }
        public string StrRenewUnstakingEnable { get; set; }
        public short Status { get; set; }
        //SPD.RenewUnstakingEnable,SPD.RenewUnstakingPeriod
    }
    public class StakingPolicyMasterRes
    {
        public long Id { get; set; }
        public long WalletTypeID { get; set; }
        public short SlabType { get; set; }
        public short StakingType { get; set; }
        public short Status { get; set; }
    }

    public class ListStakingPolicyDetailRes : BizResponseClass
    {
        public List<StakingPolicyDetailRes> Details { get; set; }
    }

    public class ListStakingPolicyDetailResV2 : BizResponseClass
    {
        public List<StakingPolicyDetailResV2> Details { get; set; }
    }

    public class ListStakingPolicyDetailRes2 : BizResponseClass
    {
        public List<StakingPolicyDetailRes> Details { get; set; }

        public List<StakingPolicyMasterRes> MasterDetail { get; set; }
    }

    public class PreStackingConfirmationReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4415")]
        public long PolicyDetailID { get; set; }

        [Range(0.000000000000000009, 9999999999.999999999999999999, ErrorMessage = "1,Invalid Parameter,4416")] //ntrivedi 23-07-2019 for BTC minimum amount 0.0000001
        public decimal Amount { get; set; }
    }

    public class PreStackingConfirmationRes
    {
        public long PolicyDetailID { get; set; }
        public short StakingType { get; set; }
        public string StakingTypeName { get; set; }
        public short SlabType { get; set; }
        public string SlabTypeName { get; set; }
        public long WalletTypeID { get; set; }
        public long InterestWalletTypeID { get; set; }//R
        public string MaturityCurrencyName { get; set; }
        public short InterestType { get; set; }
        public decimal InterestValue { get; set; }
        public short DurationWeek { get; set; }
        public short DurationMonth { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal MakerCharges { get; set; }
        public decimal TakerCharges { get; set; }
        public short EnableAutoUnstaking { get; set; }
        public string StrEnableAutoUnstaking { get; set; }
        public short EnableStakingBeforeMaturity { get; set; }
        public string StrEnableStakingBeforeMaturity { get; set; }
        public decimal EnableStakingBeforeMaturityCharge { get; set; }
    }
    public class PreStackingConfirmationRes1
    {
        public decimal Amount { get; set; }
        public decimal DeductionAmount { get; set; }
        public bool IsUpgrade { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal MaturityAmount { get; set; }
        public DateTime MaturityDate { get; set; }
    }
    public class ListPreStackingConfirmationRes : BizResponseClass
    {
        public PreStackingConfirmationRes StakingDetails { get; set; }
        public PreStackingConfirmationRes1 MaturityDetail { get; set; }
    }

    public class StakingHistoryReq
    {
        public long StakingPolicyDetailID { get; set; }
        public string AccWalletID { get; set; }
        public decimal InterestValue { get;set; }
        public long ChannelID { get; set; }
        public decimal DeductionAmount { get; set; }
        public decimal Amount { get; set; }
        public DateTime MaturityDate { get; set; }
        public decimal MaturityAmount { get; set; }
        public decimal MakerCharges { get; set; }
        public decimal TakerCharges { get; set; }
        //public long WalletTypeID { get; set; }
        public short EnableAutoUnstaking { get; set; }
    }
    
    public class UnstakingHistoryRes
    {
        //public long Id { get; set; }
        public long TokenStakingHistoryID { get; set; }
        public decimal AmountCredited { get; set; }
        public short UnstakeType { get; set; }
        public string StrUnstakeType { get; set; }
        public decimal InterestCreditedValue { get; set; }
        public decimal ChargeBeforeMaturity { get; set; }
        public long DegradeStackingHistoryRequestID { get; set; }
        public DateTime UnstakingDate { get; set; }
    }

    public class UnStakingHistoryData
    {
        public long StakingHistoryId { get; set; }
        public decimal StakingAmount { get; set; }
        public long PolicyDetailID { get; set; }
        public short Status { get; set; }
        public DateTime MaturityDate { get; set; }
        public decimal MaturityAmount { get; set; }
        //public decimal MinAmount { get; set; }
        //public decimal MaxAmount { get; set; }
        public string AvailableAmount { get; set; }
        public decimal MakerCharges { get; set; }
        public decimal TakerCharges { get; set; }

        public short EnableAutoUnstaking { get; set; }
        public string StrEnableAutoUnstaking { get; set; }
        public short EnableStakingBeforeMaturity { get; set; }
        public string StrEnableStakingBeforeMaturity { get; set; }
        public decimal EnableStakingBeforeMaturityCharge { get; set; }
        public short RenewUnstakingEnable { get; set; }
        public string StrRenewUnstakingEnable { get; set; }
        public short RenewUnstakingPeriod { get; set; }

        public short DurationWeek { get; set; }
        public short DurationMonth { get; set; }

        public long ChannelID { get; set; }
        public long WalletId { get; set; }
        public string WalletName { get; set; }
        public long WalletOwnerID { get; set; }
        public long WalletTypeID { get; set; }
        
        public short InterestType { get; set; }
        public string InterestTypeName { get; set; }
        public decimal InterestValue { get; set; }
        public long InterestWalletTypeID { get; set; }
        public string MaturityCurrency { get; set; }

        public short StakingType { get; set; }
        public short SlabType { get; set; }
        public string SlabTypeName { get; set; }        
        public string StakingCurrency { get; set; }
        public decimal DeductionAmount { get; set; }

    }

    public class UnStakingHistoryDatav2
    {
        public string StakingHistoryId { get; set; }
        public decimal StakingAmount { get; set; }
        public long PolicyDetailID { get; set; }
        public short Status { get; set; }
        public DateTime MaturityDate { get; set; }
        public decimal MaturityAmount { get; set; }
        //public decimal MinAmount { get; set; }
        //public decimal MaxAmount { get; set; }
        public string AvailableAmount { get; set; }
        public decimal MakerCharges { get; set; }
        public decimal TakerCharges { get; set; }

        public short EnableAutoUnstaking { get; set; }
        public string StrEnableAutoUnstaking { get; set; }
        public short EnableStakingBeforeMaturity { get; set; }
        public string StrEnableStakingBeforeMaturity { get; set; }
        public decimal EnableStakingBeforeMaturityCharge { get; set; }
        public short RenewUnstakingEnable { get; set; }
        public string StrRenewUnstakingEnable { get; set; }
        public short RenewUnstakingPeriod { get; set; }

        public short DurationWeek { get; set; }
        public short DurationMonth { get; set; }

        public long ChannelID { get; set; }
        public long WalletId { get; set; }
        public string WalletName { get; set; }
        public long WalletOwnerID { get; set; }
        public long WalletTypeID { get; set; }

        public short InterestType { get; set; }
        public string InterestTypeName { get; set; }
        public decimal InterestValue { get; set; }
        public long InterestWalletTypeID { get; set; }
        public string MaturityCurrency { get; set; }

        public short StakingType { get; set; }
        public short SlabType { get; set; }
        public string SlabTypeName { get; set; }
        public string StakingCurrency { get; set; }
        public decimal DeductionAmount { get; set; }

    }

    public class StakingHistoryRes
    {
        public long StakingHistoryId { get; set; }
        public long PolicyDetailID { get; set; }
        public short StakingType { get; set; }
        public string StakingTypeName { get; set; }
        public short SlabType { get; set; }
        public string SlabTypeName { get; set; }
        public long WalletTypeID { get; set; }
        public string StakingCurrency { get; set; }
        public short InterestType { get; set; }
        public string InterestTypeName { get; set; }
        public decimal InterestValue { get; set; }
        public short DurationWeek { get; set; }
        public short DurationMonth { get; set; }
        public long InterestWalletTypeID { get; set; }
        public string MaturityCurrency { get; set; }
        //public decimal MinAmount { get; set; }
        //public decimal MaxAmount { get; set; }
        public decimal MakerCharges { get; set; }
        public decimal TakerCharges { get; set; }

        public string AvailableAmount { get; set; }       

        public short EnableAutoUnstaking { get; set; }
        public string StrEnableAutoUnstaking { get; set; }
        public short EnableStakingBeforeMaturity { get; set; }
        public string StrEnableStakingBeforeMaturity { get; set; }
        public decimal EnableStakingBeforeMaturityCharge { get; set; }
        public short RenewUnstakingEnable { get; set; }
        public string StrRenewUnstakingEnable { get; set; }
        public short RenewUnstakingPeriod { get; set; }


        public decimal StakingAmount { get; set; }
        //public decimal Charge { get; set; }
        public DateTime MaturityDate { get; set; }
        public decimal MaturityAmount { get; set; }
        public long ChannelID { get; set; }
        public string ChannelName { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long WalletId { get; set; }
        public string WalletName { get; set; }
        public long WalletOwnerID { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public long TokenStakingHistoryID { get; set; }
        public decimal AmountCredited { get; set; }
        public short UnstakeType { get; set; }
        public string StrUnstakeType { get; set; }
        public decimal InterestCreditedValue { get; set; }
        public decimal ChargeBeforeMaturity { get; set; }
        public long DegradeStakingHistoryRequestID { get; set; }
        public DateTime UnstakingDate { get; set; }

        public DateTime StakingDate { get; set; }
    }
    public class StakingHistoryResv2
    {
        public string StakingHistoryId { get; set; }
        public long PolicyDetailID { get; set; }
        public short StakingType { get; set; }
        public string StakingTypeName { get; set; }
        public short SlabType { get; set; }
        public string SlabTypeName { get; set; }
        public long WalletTypeID { get; set; }
        public string StakingCurrency { get; set; }
        public short InterestType { get; set; }
        public string InterestTypeName { get; set; }
        public decimal InterestValue { get; set; }
        public short DurationWeek { get; set; }
        public short DurationMonth { get; set; }
        public long InterestWalletTypeID { get; set; }
        public string MaturityCurrency { get; set; }
        //public decimal MinAmount { get; set; }
        //public decimal MaxAmount { get; set; }
        public decimal MakerCharges { get; set; }
        public decimal TakerCharges { get; set; }

        public string AvailableAmount { get; set; }

        public short EnableAutoUnstaking { get; set; }
        public string StrEnableAutoUnstaking { get; set; }
        public short EnableStakingBeforeMaturity { get; set; }
        public string StrEnableStakingBeforeMaturity { get; set; }
        public decimal EnableStakingBeforeMaturityCharge { get; set; }
        public short RenewUnstakingEnable { get; set; }
        public string StrRenewUnstakingEnable { get; set; }
        public short RenewUnstakingPeriod { get; set; }


        public decimal StakingAmount { get; set; }
        //public decimal Charge { get; set; }
        public DateTime MaturityDate { get; set; }
        public decimal MaturityAmount { get; set; }
        public long ChannelID { get; set; }
        public string ChannelName { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long WalletId { get; set; }
        public string WalletName { get; set; }
        public long WalletOwnerID { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public long TokenStakingHistoryID { get; set; }
        public decimal AmountCredited { get; set; }
        public short UnstakeType { get; set; }
        public string StrUnstakeType { get; set; }
        public decimal InterestCreditedValue { get; set; }
        public decimal ChargeBeforeMaturity { get; set; }
        public long DegradeStakingHistoryRequestID { get; set; }

        public DateTime StakingDate { get; set; }
        public DateTime UnstakingDate { get; set; }
    }
    public class ListStakingHistoryRes : BizResponseClass
    {        
        public List<StakingHistoryRes> Stakings { get; set; }
        //public List<UnstakingHistoryRes> Unstakings { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public long PageNo { get; set; }
    }
    public class ListStakingHistoryResv2 : BizResponseClass
    {
        public List<StakingHistoryResv2> Stakings { get; set; }
        //public List<UnstakingHistoryRes> Unstakings { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public long PageNo { get; set; }
    }

    public class PreUnstackingConfirmationReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4421")]
        public long StakingHistoryId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4420")]
        public EnUnstakeType UnstakingType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4419")]
        public long DegradePolicyDetailID { get; set; } // 0 When Full Unstake

        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Invalid Parameter,4416")]
        public decimal Amount { get; set; } // 0 When Full Unstake

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4418")]
        public long ChannelId { get; set; }
    }

    public class PreUnstackingConfirmationReqv2
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4421")]
        public string StakingHistoryId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4420")]
        public EnUnstakeType UnstakingType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4419")]
        public long DegradePolicyDetailID { get; set; } // 0 When Full Unstake

        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Invalid Parameter,4416")]
        public decimal Amount { get; set; } // 0 When Full Unstake

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4418")]
        public long ChannelId { get; set; }
    }

    public class PreUnstackingConfirmationRes
    {
        public long StakingHistoryId { get; set; }
        public decimal StakingAmount { get; set; }
        public decimal BeforeMaturityChargeDeduction { get; set; }
        public decimal NewStakingAmountDeduction { get; set; }
        public decimal CreditAmount { get; set; }
    }
    public class PreUnstackingConfirmationResv2
    {
        public string StakingHistoryId { get; set; }
        public decimal StakingAmount { get; set; }
        public decimal BeforeMaturityChargeDeduction { get; set; }
        public decimal NewStakingAmountDeduction { get; set; }
        public decimal CreditAmount { get; set; }
    }
    public class UnstakingDetailRes : BizResponseClass
    {
        public PreUnstackingConfirmationRes UnstakingDetail { get; set; }
        public PreStackingConfirmationRes NewStakingDetails { get; set; }
        public PreStackingConfirmationRes1 NewMaturityDetail { get; set; }
    }

    public class UnstakingDetailResv2 : BizResponseClass
    {
        public PreUnstackingConfirmationResv2 UnstakingDetail { get; set; }
        public PreStackingConfirmationRes NewStakingDetails { get; set; }
        public PreStackingConfirmationRes1 NewMaturityDetail { get; set; }
    }

    public class UserUnstakingReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4421")]
        public long StakingHistoryId { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4420")]
        public EnUnstakeType Type { get; set; }
        public long StakingPolicyDetailId { get; set; }
        public decimal StakingAmount { get; set; }
        public long ChannelID { get; set; }
    }

    public class UserUnstakingReqv2
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4421")]
        public string StakingHistoryId { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4420")]
        public EnUnstakeType Type { get; set; }
        public long StakingPolicyDetailId { get; set; }
        public decimal StakingAmount { get; set; }
        public long ChannelID { get; set; }
    }

    public class UserUnstakingReq2
    {        
        public long Id { get; set; }
        public long UserID { get; set; }        
        public long StakingPolicyDetailId { get; set; }
        public long ChannelID { get; set; }
        public decimal StakingAmount { get; set; }
        public DateTime MaturityDate { get; set; }
        public short EnableAutoUnstaking { get; set; }
    }
    public class ListUserUnstakingReq2
    {
        public List<UserUnstakingReq2> Data { get; set; }
    }

    public class UnStakingHistory
    {
        public long AdminReqID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public long TokenStakingHistoryID { get; set; }
        public decimal StakingAmount { get; set; }
        public decimal AmountCredited { get; set; }
        public short UnstakeType { get; set; }
        public string UnstackTypeName { get; set; }
        public decimal InterestCreditedValue { get; set; }
        public decimal ChargeBeforeMaturity { get; set; }
        public long DegradeStakingHistoryRequestID { get; set; }
        public decimal DegradeStakingAmount { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public DateTime RequestDate { get; set; }
    }
    public class UnStakingHistoryv2
    {
        public string AdminReqID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public long TokenStakingHistoryID { get; set; }
        public decimal AmountCredited { get; set; }
        public short UnstakeType { get; set; }
        public string UnstackTypeName { get; set; }
        public decimal InterestCreditedValue { get; set; }
        public decimal ChargeBeforeMaturity { get; set; }
        public long DegradeStakingHistoryRequestID { get; set; }
        public decimal DegradeStakingAmount { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public DateTime RequestDate { get; set; }
    }
    public class ListUnStakingHistory : BizResponseClass
    {
        public List<UnStakingHistory> Unstakings { get; set; }
        public List<UnStakingHistoryData> StakingDetail { get; set; }
    }
    public class ListUnStakingHistoryv2 : BizResponseClass
    {
        public List<UnStakingHistoryv2> Unstakings { get; set; }
        public List<UnStakingHistoryDatav2> StakingDetail { get; set; }
    }

}
