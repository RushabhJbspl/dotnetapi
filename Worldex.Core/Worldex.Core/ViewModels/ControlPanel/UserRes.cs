using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.ControlPanel
{
    #region Count Methods

    public class CountRes : BizResponseClass
    {
        public long Count { get; set; }
    }
    public class TempCount
    {
        public Int32 TotalCount { get; set; }
    }
    public class TodaysCounts
    {
        public Int32 TotalCount { get; set; }
        public Int32 TodayCount { get; set; }
    }
    public class TodaysCount : BizResponseClass
    {
        public long TotalCount { get; set; }
        public long TodayCount { get; set; }
    }
    public class CommonCountClass
    {
        public int Key { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }
    public class StatusWiseRes : BizResponseClass
    {
        public List<CommonCountClass> Counter { get; set; }
    }
    public class UTypeWiseRes : BizResponseClass
    {
        public List<CommonCountClass> Counter { get; set; }
    }
    public class OrgWiseRes : BizResponseClass
    {
        public List<CommonCountClass> Counter { get; set; }
    }
    public class WalletTypeWiseRes : BizResponseClass
    {
        public List<CommonCountClass> Counter { get; set; }
    }
    public class WalletTrnTypeWiseRes : BizResponseClass
    {
        public List<CommonCountClass> Counter { get; set; }
    }
    public class RolewiseUserCount : BizResponseClass
    {
        public List<CommonCountClass> Counter { get; set; }
    }
    public class ChannelwiseTranCount
    {
        public long ChannelID { get; set; }
        public string ChannelName { get; set; }
        public int TotalCount { get; set; }
    }
    public class ListChannelwiseTrnCount : BizResponseClass
    {
        public List<ChannelwiseTranCount> Counter { get; set; }
    }


    public class OrgDetail
    {
        public int UserCount { get; set; }
        public int WalletCount { get; set; }
        public int WalletTypeCount { get; set; }
        public decimal ToatalBalance { get; set; }
    }
    public class ListOrgDetail : BizResponseClass
    {
        public OrgDetail Data { get; set; }
    }
    public class TypeWiseDetail
    {
        public long WalletTypeId { get; set; }
        public string WalletType { get; set; }
        public int UserCount { get; set; }
        public int WalletCount { get; set; }
        public int TransactionCount { get; set; }
        public decimal ToatalBalance { get; set; }
    }
    public class ListTypeWiseDetail : BizResponseClass
    {
        public List<TypeWiseDetail> WalletTypes { get; set; }
    }
    #endregion

    #region List Methods

    public class OrgMasterRes
    {
        public long OrgID { get; set; }
        public string OrgName { get; set; }
        public short IsDefault { get; set; }
        public short Status { get; set; }
        public Int32 TotalWallets { get; set; }
        public Int32 TotalUsers { get; set; }
        public decimal TotalBalance { get; set; }
    }
    public class OrgList : BizResponseClass
    {
        public List<OrgMasterRes> Organizations { get; set; }
    }

    public class ChannelMasterRes
    {
        public long ChannelId { get; set; }
        public string ChannelName { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
    }
    public class ListChannels : BizResponseClass
    {
        public List<ChannelMasterRes> Channels { get; set; }
        public long TotalCount { get; set; }
    }

    public class RoleMasterRes
    {
        public long ID { get; set; }
        public string RoleName { get; set; }
        public short Status { get; set; }
    }
    public class RoleList : BizResponseClass
    {
        public List<RoleMasterRes> Roles { get; set; }
    }


    public class CurrencyMasterReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4323")]
        public long ID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4322")]
        public string CurrencyTypeName { get; set; }
        public short? Status { get; set; }
    }
    public class CurrencyMasterRes
    {
        public long ID { get; set; }
        public string CurrencyName { get; set; }
        public short Status { get; set; }
    }
    public class CurrencyList : BizResponseClass
    {
        public List<CurrencyMasterRes> Currencies { get; set; }
    }

    public class WalletTypeMasterRes
    {
        public long ID { get; set; }
        public string TypeName { get; set; }
        public long CurrencyTypeID { get; set; }
        public string Description { get; set; }
        public short Status { get; set; }
    }
    public class WalletTypeList : BizResponseClass
    {
        public List<WalletTypeMasterRes> Types { get; set; }
    }

    public class WalletTypeMasterResp : BizResponseClass
    {
        public long ID { get; set; }
        public string TypeName { get; set; }
        public long CurrencyTypeID { get; set; }
        public string Description { get; set; }
        public short Status { get; set; }
    }

    public class ChargeTypeMasterRes
    {
        //public long ID { get; set; }
        public string TypeName { get; set; }
        public long ChargeTypeID { get; set; }
        public short Status { get; set; }
    }
    public class ChargeTypeList : BizResponseClass
    {
        public List<ChargeTypeMasterRes> Types { get; set; }
    }

    public class CommissionTypeMasterRes
    {
        //public long ID { get; set; }
        public string TypeName { get; set; }
        public long TypeID { get; set; }
        public short Status { get; set; }
    }
    public class CommisssionTypeList : BizResponseClass
    {
        public List<CommissionTypeMasterRes> Types { get; set; }
    }

    public class ProviderRes
    {
        public long ID { get; set; }
        public string ProviderName { get; set; }
        public int Status { get; set; }
        public string StrStatus { get; set; }
    }
    public class ListProviderRes : BizResponseClass
    {
        public List<ProviderRes> Providers { get; set; }
        public long TotalCount { get; set; }
    }

    public class ChargePolicyRes
    {
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public long Id { get; set; }
        public string PolicyName { get; set; }
        public long WalletTrnType { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public long WalletType { get; set; }
        public long ChargeType { get; set; }
        public long Type { get; set; }
        public decimal ChargeValue { get; set; }
    }
    public class ListChargePolicy : BizResponseClass
    {
        public List<ChargePolicyRes> ChargePolicyList { get; set; }
    }
    public class CommissionPolicyRes
    {
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public long Id { get; set; }
        public string PolicyName { get; set; }
        public long WalletTrnType { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public long WalletType { get; set; }
        public long CommissionType { get; set; }
        public long Type { get; set; }
        public decimal CommissionValue { get; set; }
    }
    public class ListCommissionPolicy : BizResponseClass
    {
        public List<CommissionPolicyRes> CommissionPolicyList { get; set; }
    }

    public class WalletusagePolicyRes
    {
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public long Id { get; set; }
        public string WalletType { get; set; }
        public string AllowedIP { get; set; }
        public string AllowedLocation { get; set; }
        public int AuthenticationType { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public long HourlyTrnCount { get; set; }
        public decimal HourlyTrnAmount { get; set; }
        public long DailyTrnCount { get; set; }
        public decimal DailyTrnAmount { get; set; }
        public long MonthlyTrnCount { get; set; }
        public string PolicyName { get; set; }
        public decimal MonthlyTrnAmount { get; set; }
        public long WeeklyTrnCount { get; set; }
        public decimal WeeklyTrnAmount { get; set; }
        public long YearlyTrnCount { get; set; }
        public decimal YearlyTrnAmount { get; set; }
        public long LifeTimeTrnCount { get; set; }
        public decimal LifeTimeTrnAmount { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
    }
    public class ListWalletusagePolicy : BizResponseClass
    {
        public List<WalletusagePolicyRes2> UsageList { get; set; }
    }

    public class WalletusagePolicyRes2
    {
        public long AutoNo { get; set; }
        public long Id { get; set; }
        public string PolicyName { get; set; }
        public string WalletType { get; set; }
        public long WalletTypeId { get; set; }
        public string AllowedIP { get; set; }
        public string AllowedLocation { get; set; }
        public int AuthenticationType { get; set; }
        public double StartTime { get; set; }
        public double EndTime { get; set; }
        public long HourlyTrnCount { get; set; }
        public decimal HourlyTrnAmount { get; set; }
        public long DailyTrnCount { get; set; }
        public decimal DailyTrnAmount { get; set; }
        public long MonthlyTrnCount { get; set; }
        public decimal MonthlyTrnAmount { get; set; }
        public long WeeklyTrnCount { get; set; }
        public decimal WeeklyTrnAmount { get; set; }
        public long YearlyTrnCount { get; set; }
        public decimal YearlyTrnAmount { get; set; }
        public long LifeTimeTrnCount { get; set; }
        public decimal LifeTimeTrnAmount { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public string DayNo { get; set; }
    }
    public class ListWalletusagePolicy2 : BizResponseClass
    {
        public List<WalletusagePolicyRes2> Details { get; set; }
    }


    public class UserRes
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
    }
    public class UserList : BizResponseClass
    {
        public long WalletUserCount { get; set; }
        public List<UserRes> UserLists { get; set; }
    }
    public class TypeRes
    {
        public long TypeId { get; set; }
        public string TypeName { get; set; }
        public short Status { get; set; }
    }
    public class ListTypeRes : BizResponseClass
    {
        public List<TypeRes> Data { get; set; }
    }

    public class AllowTrnTypeRoleWiseRes
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        public long TrnTypeId { get; set; }
        public string RoleName { get; set; }
        public string TrnTypeName { get; set; }
        public string StrStatus { get; set; }
        public short Status { get; set; }
    }

    public class ListAllowTrnTypeRoleWise : BizResponseClass
    {
        public List<AllowTrnTypeRoleWiseRes> Data { get; set; }
    }

    public class ListStopLossRes : BizResponseClass
    {
        public List<StopLossRes> Data { get; set; }
    }

    public class StopLossRes
    {
        public long Id { get; set; }
        public long WalletTypeId { get; set; }
        public string WalletTypeName { get; set; }
        public decimal StopLossPer { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
    }

    public class ListLeverageRes : BizResponseClass
    {
        public List<LeverageRes> Data { get; set; }
    }

    public class LeverageRes
    {
        public long Id { get; set; }
        public long WalletTypeId { get; set; }
        public string WalletTypeName { get; set; }
        public decimal LeveragePer { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public short IsAutoApprove { get; set; }
        public decimal SafetyMarginPer { get; set; }
        public decimal MarginChargePer { get; set; }
        public int LeverageChargeDeductionType { get; set; }
        public string LeverageChargeDeductionTypeName { get; set; }
        public decimal InstantChargePer { get; set; }
    }
    public class ListStakingPolicyRes : BizResponseClass
    {
        public List<StakingPolicyRes> Data { get; set; }
    }

    public class StakingPolicyRes
    {
        public long Id { get; set; }
        public long WalletTypeId { get; set; }
        public string WalletTypeName { get; set; }
        public short SlabType { get; set; }
        public string SlabTypeName { get; set; }
        public short StakingType { get; set; }
        public string StakingTypeName { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
    }

    public class AdminAssetsres
    {
        public long Id { get; set; }
        public string WalletName { get; set; }
        public string WalletTypeName { get; set; }
        public long WalletTypeId { get; set; }
        public decimal Balance { get; set; }
        public decimal InBoundBalance { get; set; }
        public decimal OutBoundBalance { get; set; }
        public short WalletUsageType { get; set; }
        public string StrWalletUsageType { get; set; }
        public string OrganizationName { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public string Email { get; set; }
        public long UserId { get; set; }
        public long OrgId { get; set; }
        public string AccWalletId { get; set; }
    }

    public class OrgWalletres
    {
        public long Id { get; set; }
        public string WalletName { get; set; }
        public string WalletTypeName { get; set; }
        public long WalletTypeId { get; set; }
        public short WalletUsageType { get; set; }
        public string StrWalletUsageType { get; set; }
        public string OrganizationName { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public string Email { get; set; }
        public long UserId { get; set; }
        public long OrgId { get; set; }
        public string AccWalletId { get; set; }
    }

    public class ListAdminAssetsres : BizResponseClass
    {
        public List<AdminAssetsres> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int PageNo { get; set; }
    }

    public class ListOrgLedger : BizResponseClass
    {
        public List<OrgWalletres> Data { get; set; }
        public int TotalCount { get; set; }
    }

    public class MarginWithdrawPreConfirmResponse : BizResponseClass
    {
        //public List<LeverageRes> Data { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ProfitAmount { get; set; }
        public decimal SafetyAmount { get; set; }
        public long LoanID { get; set; }
        public decimal ChargeAmount { get; set; }
    }




    #endregion

    #region CRUD Classes

    public class TransactionBlockedChannelReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4323")]
        public long ID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4318")]
        public long ChannelID { get; set; }

        //[Required(ErrorMessage = "1,Please Enter Required Parameter,4319")]
        //public string ChannelName { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4320")]
        [EnumDataType(typeof(enWalletTrnType), ErrorMessage = "1,Fail,4321")]
        public enWalletTrnType TrnType { get; set; }

        public short? Status { get; set; }
    }

    public class AllowedChannelReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4323")]
        public long ID { get; set; }

        //[Required(ErrorMessage = "1,Please Enter Required Parameter,4318")]
        //public long ChannelID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4319")]
        public string ChannelName { get; set; }

        public short? Status { get; set; }
    }

    public class TransactionBlockedChannelRes
    {
        public long AutoNo { get; set; }
        public long ID { get; set; }
        public long ChannelID { get; set; }
        //public string ChannelName { get; set; }
        public long TrnType { get; set; }
        public string TrnTypeName { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
    }
    public class ListTransactionBlockedChannelRes : BizResponseClass
    {
        public List<TransactionBlockedChannelRes> Details { get; set; }
    }

    public class BlockTrnTypewiseReport
    {
        public long AutoNo { get; set; }
        public long WalletID { get; set; }
        public string AccWalletID { get; set; }
        public string Walletname { get; set; }
        public long WalletTypeID { get; set; }
        public string Currency { get; set; }
        public long UserID { get; set; }
        public string UserName { get; set; }
        public decimal Balance { get; set; }
        public decimal InBoundBalance { get; set; }
        public decimal OutBoundBalance { get; set; }
        public string Remarks { get; set; }
        public short Status { get; set; }
    }
    public class ListBlockTrnTypewiseReport : BizResponseClass
    {
        public List<BlockTrnTypewiseReport> Details { get; set; }
        public long TotalPages { get; set; }
        public long PageSize { get; set; }
    }

    public class UserDetailRes
    {
        public long AutoNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string OrganizationName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CountryCode { get; set; }
        public string IsEnabled { get; set; }
        public string EmailConfirmed { get; set; }
        public decimal TotalBalance { get; set; }
        public string UserType { get; set; }
    }
    public class ListUserDetailRes : BizResponseClass
    {
        public List<UserDetailRes> Details { get; set; }
        public long TotalPages { get; set; }
        public long PageSize { get; set; }
    }

    public class UserTypeRes
    {
        public long UserTypeId { get; set; }
        public string UserType { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
    }
    public class ListUserTypeRes : BizResponseClass
    {
        public List<UserTypeRes> Details { get; set; }
    }

    public class WalletPolicyAllowedDayReq
    {
        public long ID { get; set; }

        [Range(1, 9999999, ErrorMessage = "1,Invalid Required Parameter,4325")]
        public short WalletPolicyID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4326")]
        [EnumDataType(typeof(EnWeekDays), ErrorMessage = "1,Fail,4327")]
        public EnWeekDays DayNo { get; set; }

        //  public short[] DayNo { get; set; }

        public short? Status { get; set; }
    }

    //public class AuthAppDetailRes : BizResponseClass
    //{
    //    public long AppID { get; set; }
    //    public string AppName { get; set; }
    //    public string SiteURL { get; set; }
    //    public string SecretKey { get; set; }
    //    public short Status { get; set; }
    //    public string StrStatus { get; set; }
    //}
    public class AuthAppRes
    {
        public long AutoNo { get; set; }
        public long AppID { get; set; }
        public string AppName { get; set; }
        public string SiteURL { get; set; }
        public string SecretKey { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
    }
    public class ListAuthAppRes : BizResponseClass
    {
        public List<AuthAppRes> Details { get; set; }
    }

    public class UserActivityLoging
    {
        public long AutoNo { get; set; }
        public Int16 ActivityType { get; set; }
        public long UserID { get; set; }
        public long WalletID { get; set; }
        public long TranNo { get; set; }
        public string Remarks { get; set; }
        public short WalletTrnType { get; set; }
        public string WalletTrnTypeName { get; set; }
    }
    public class ListUserActivityLoging : BizResponseClass
    {
        public List<UserActivityLoging> Details { get; set; }
    }
    public class ListWalletRes : BizResponseClass
    {
        public List<WalletRes> Data { get; set; }
        public long TotalWallet { get; set; }
        public long PageSize { get; set; }
        public long PageNo { get; set; }
    }
    public class ListWalletResV1 : BizResponseClass
    {
        public List<WalletResV1> Data { get; set; }
        public long TotalWallet { get; set; }
        public long PageSize { get; set; }
        public long PageNo { get; set; }
    }
    public class GetWalletRes : BizResponseClass
    {
        public List<WalletRes> Data { get; set; }
        public long TotalWallet { get; set; }
    }

    public class WalletPolicyAllowedDayRes
    {
        public long AutoNo { get; set; }
        public long ID { get; set; }
        public long WalletPolicyID { get; set; }
        public string PolicyName { get; set; }
        public long WalletType { get; set; }
        public short DayNo { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
    }
    public class ListWalletPolicyAllowedDayRes : BizResponseClass
    {
        public List<WalletPolicyAllowedDayRes> Details { get; set; }
    }


    public class WalletRes
    {
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public decimal Balance { get; set; }
        public string WalletTypeName { get; set; }
        public bool IsValid { get; set; }
        public string OrganizationName { get; set; }
        public string UserName { get; set; }
        public string Walletname { get; set; }
        public string AccWalletID { get; set; }
        public byte IsDefaultWallet { get; set; }
        public string PublicAddress { get; set; }
        public decimal InBoundBalance { get; set; }
        public decimal OutBoundBalance { get; set; }
    }
    public class WalletResV1
    {
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public decimal Balance { get; set; }
        public string WalletTypeName { get; set; }
        public bool IsValid { get; set; }
        public string OrganizationName { get; set; }
        public string UserName { get; set; }
        public string Walletname { get; set; }
        public string AccWalletID { get; set; }
        public byte IsDefaultWallet { get; set; }
        public string PublicAddress { get; set; }
        public decimal InBoundBalance { get; set; }
        public decimal OutBoundBalance { get; set; }
        public DateTime Date { get; set; }
    }
    public class WalletResv11 : BizResponseClass
    {
        public WalletResV1 Data { get; set; }
    }

    public class WalletRes1 : BizResponseClass
    {
        public WalletRes Data { get; set; }
    }

    public class AuthAppReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4323")]
        public long AppID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4315")]
        public string AppName { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4316")]
        public string SecreteKey { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4317")]
        public string SiteURL { get; set; }
        public short? Status { get; set; }
    }

    public class WalletTypeMasterReq
    {
        public long ID { get; set; }//For Insert/Update Decission Making

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4382")]
        public string WalletTypeName { get; set; }

        public string Description { get; set; }

        public short? Status { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4383")]
        [EnumDataType(typeof(EnCurrencyType), ErrorMessage = "1,Invalid Required Parameter,4384")]
        public EnCurrencyType CurrencyTypeID { get; set; }
    }

    public class MinMaxRanges
    {
        public decimal MinRange { get; set; }
        public decimal MaxRange { get; set; }
    }
    public class Counts
    {
        public int Count { get; set; }
    }
    public class StakingPolicyReq
    {
        //[Required(ErrorMessage = "1,Please Enter Required Parameter,4392")]
        //public long CurrencyTypeID { get; set; }

        //[Required(ErrorMessage = "1,Please Enter Required Parameter,4393")]
        //[EnumDataType(typeof(EnStakingType), ErrorMessage = "1,Fail,4394")]
        //public EnStakingType StakingType { get; set; }

        //[Required(ErrorMessage = "1,Please Enter Required Parameter,4395")]
        //[EnumDataType(typeof(EnStakingSlabType), ErrorMessage = "1,Fail,4396")]
        //public EnStakingSlabType Slab { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4439")]
        public long StakingPolicyID { get; set; }

        [Range(0, 4, ErrorMessage = "1,Invalid Parameter,4444")]
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4397")]
        public short DurationWeek { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4398")]
        public short DurationMonth { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4399")]
        public short AutoUnstakingEnable { get; set; }

        public EnInterestType? InterestType { get; set; }

        [Column(TypeName = "decimal(28, 18)")]
        public decimal? InterestValue { get; set; }

        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Invalid Parameter,4400")]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal? Amount { get; set; }

        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Invalid Parameter,4401")]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal? MinAmount { get; set; }

        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Invalid Parameter,4402")]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal? MaxAmount { get; set; }

        public short? RenewUnstakingEnable { get; set; }
        public short? RenewUnstakingPeriod { get; set; }

        public short? EnableStakingBeforeMaturity { get; set; }
        [Column(TypeName = "decimal(28, 18)")]
        public decimal? EnableStakingBeforeMaturityCharge { get; set; }

        public long? MaturityCurrency { get; set; }

        [Column(TypeName = "decimal(28, 18)")]
        public decimal? MakerCharges { get; set; }

        [Column(TypeName = "decimal(28, 18)")]
        public decimal? TakerCharges { get; set; }

        public short? Status { get; set; }

    }

    public class UpdateStakingDetailReq
    {

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4397")]
        public short DurationWeek { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4398")]
        public short DurationMonth { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4399")]
        public short AutoUnstakingEnable { get; set; }

        public EnInterestType? InterestType { get; set; }

        [Column(TypeName = "decimal(28, 18)")]
        public decimal? InterestValue { get; set; }

        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Invalid Parameter,4400")]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal? Amount { get; set; }

        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Invalid Parameter,4401")]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal? MinAmount { get; set; }

        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Invalid Parameter,4402")]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal? MaxAmount { get; set; }

        public short? RenewUnstakingEnable { get; set; }
        public short? RenewUnstakingPeriod { get; set; }

        public short? EnableStakingBeforeMaturity { get; set; }

        [Column(TypeName = "decimal(28, 18)")]
        public decimal? EnableStakingBeforeMaturityCharge { get; set; }

        public long? MaturityCurrency { get; set; }

        [Column(TypeName = "decimal(28, 18)")]
        public decimal? MakerCharges { get; set; }
        [Column(TypeName = "decimal(28, 18)")]
        public decimal? TakerCharges { get; set; }

        public short? Status { get; set; }
    }


    public class DepositeCounterRes
    {
        public long Id { get; set; }
        //   public long AppTypeID { get; set; }
        //  public string AppTypeName { get; set; }
        public DateTime UpdatedDate { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public int RecordCount { get; set; }
        public long Limit { get; set; }
        public long MaxLimit { get; set; }
        public long TPSPickupStatus { get; set; }
        public long WalletTypeID { get; set; }
        public string WalletTypeName { get; set; }
        public long SerProId { get; set; }
        public string SerProName { get; set; }
        public string LastTrnID { get; set; }
        public string PreviousTrnID { get; set; }
        public string PrevIterationID { get; set; }
    }

    public class ListDepositeCounterRes : BizResponseClass
    {
        public List<DepositeCounterRes> Data { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
    #endregion
}
