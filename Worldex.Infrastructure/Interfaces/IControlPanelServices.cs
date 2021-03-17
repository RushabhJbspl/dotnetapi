using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.ViewModels.WalletConfiguration;
using Worldex.Core.ViewModels.WalletOperations;
using System;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Interfaces
{
    public interface IControlPanelServices
    {
        TodaysCount GetUserCount(long? OrgID, long? UserType, short? Status, long? RoleID);
        TodaysCount GetOrganizationCount(short? status);
        StatusWiseRes GetUCntStatusWise();
        UTypeWiseRes GetUCntTypeWise();
        OrgWiseRes GetUCntOrgWise();
        RolewiseUserCount GetUCntRoleWise();
        Task<OrgList> ListOrgDetail(short? Status, long? OrgID);
        CountRes GetWalletCount(long? walletTypeID, short? status, long? orgID, long? userID);
        WalletTypeWiseRes GetWCntTypeWise();
        WalletTypeWiseRes GetWCntStatusWise();
        WalletTypeWiseRes GetWCntOrgWise();
        WalletTypeWiseRes GetWCntUserWise();
        CountRes GetWalletAuthUserCount(short? status, long? orgID, long? userID, long? RoleID);
        WalletTypeWiseRes GetWAUCntStatusWise();
        WalletTypeWiseRes GetWAUCntOrgWise();
        CountRes GetUserRoleCount(short? status);
        RoleList ListRoleDetail(short? status);
        CountRes GetCurrencyCount(short? status);
        CurrencyList ListCurrencyDetail(short? status);
        CountRes GetWalletTypeCount(short? status, long? CurrencyTypeID);
        WalletTypeList ListWalletTypeDetail(short? status, long? ServiceProviderId, long? currencyTypeID, short isMargin);
        RolewiseUserCount GetWAUCntRoleWise();
        CountRes GetTodayOrganizationCount(short? status);
        CountRes GetTodayUserCount(short? status);
        ListWalletGraphRes GraphForOrgCount();
        ListTransactionTypewiseCount GraphForTrnTypewiseCount();
        Task<BizResponseClass> AddAuthorizedApps(AuthAppReq Request, long UserID); 
        ListAuthAppRes ListAuthorizedAppDetail(short? Status);
        Task<BizResponseClass> BlockTranForChannel(TransactionBlockedChannelReq request, long userID);
        ListAuthAppRes GetAuthorizedAppDetail(long AppId, short? Status);
        BizResponseClass UpdateTradingChargeType(UpdateTradingChargeTypeReq Request);
        //vsolanki 24-11-2018
        CountRes GetChargeTypeCount(short? status, long? ChargeTypeID);
        ChargeTypeList ListChargeTypeDetail(short? status, long? ChargeTypeID);

        //vsolanki 24-11-2018
        CountRes GetCommissionTypeCount(short? status, long? TypeID);
        CommisssionTypeList ListCommissionTypeDetail(short? status, long? TypeID);

        Task<LPServiceProBalanceResponse> GetLPProviderBalance(long SerProID, string SMSCode, int GenerateMismatch = 0);

        //vsolanki 24-11-2018
        CountRes GetChargePolicyRecCount(long? WalletTypeID, short? status, long? WalletTrnTypeID);
        WalletTypeWiseRes GetChargePolicyWalletTypeWiseCount();
        StatusWiseRes GetChargePolicyStatusWiseCount();
        WalletTrnTypeWiseRes GetChargePolicyWalletTrnTypeWiseCount();
        ListChargePolicy GetChargePolicyList(short? status, long? WalletType, long? WalletTrnType);
        ListChargePolicy ListChargePolicyLast5();

        //vsolanki 24-11-2018
        CountRes GetCommissionPolicyRecCount(long? WalletTypeID, short? status, long? WalletTrnTypeID);
        ListTransactionBlockedChannelRes GetBlockTranForChannel(long iD, short? status, long? ChannelID);
        WalletTypeWiseRes GetCommissionPolicyWalletTypeWiseCount();
        StatusWiseRes GetCommissionPolicyStatusWiseCount();
        WalletTrnTypeWiseRes GetCommissionPolicyWalletTrnTypeWiseCount();
        ListCommissionPolicy GetCommissionPolicyList(short? status, long? WalletType, long? WalletTrnType);
        ListCommissionPolicy ListCommissionPolicy();

        //vsoalnki 2018-11-27
        ListWalletGraphRes GraphForUserCount();

        ListWalletusagePolicy ListUsagePolicyLast5();
        UserList ListUserLast5();

        //vsoalnki 2018-11-28
        BizResponseClass UpdateWTrnTypeStatus(long TrnTypeId, short Status,long UserId);
        //vsoalnki 2018-11-28
        BizResponseClass InsertBlockWalletTrnType(BlockWalletTrnTypeReq Req,long UserId);

        ListTypeRes ListWalletType();
        ListTypeRes ListWalletTrnType();
        ListTypeRes GetBlockWTypewiseTrnTypeList(long WalletType);

        BizResponseClass InsertChargePolicy(ChargePolicyReq Req);
        BizResponseClass AddCommPolicy(CommPolicyReq request, long UserID);
        BizResponseClass UpdateCommPolicyDetail(long CommPolicyID, UpdateCommPolicyReq request, long UserID);
        ListTransactionBlockedChannelRes ListBlockTranForChannel(enWalletTrnType? trnType, long? channelID, short? status);
        BizResponseClass UpdateChargePolicy(long ID, UpdateChargePolicyReq Req,long UserId);
        BizResponseClass SetDefaultOrganization(long orgID, long userid);
        ListChannels ListChannels();
        ListProviderRes ListProviders();
        ListChannelwiseTrnCount ListChannelwiseTrnCnt();
        Task<BizResponseClass> AddNewCurrencyType(CurrencyMasterReq Request, long UserID);
        Task<ListOrgDetail> GetOrgAllDetail();
        Task<ListTypeWiseDetail> GetDetailTypeWise();

        ListTypeWiseDetail GetDetailTypeWiseV1();
       

        ListTransactionPolicyRes ListTransactionPolicy();
        BizResponseClass InsertTransactionPolicy(AddTransactionPolicyReq Req, long UserId);
        BizResponseClass UpdateTransactionPolicy(UpdateTransactionPolicyReq Req, long UserId, long TrnPolicyId);
        BizResponseClass UpdateTransactionPolicyStatus(short Status, long UserId, long TrnPolicyId);
        Task<BizResponseClass> AddWPolicyAllowedDay(WalletPolicyAllowedDayReq request, long id);
        ListUserDetailRes ListAllUserDetails(long? orgID, long? userType, short? status, int? pageNo, int? pageSize);

        ListUserWalletBlockTrnType ListUserWalletBlockTrnType(string WalletId, long? TrnTypeId);
        BizResponseClass InsertUpdateUserWalletBlockTrnType(InsertUpdateUserWalletBlockTrnTypeReq Req, long UserId);
        BizResponseClass ChangeUserWalletBlockTrnTypeStatus(long Id, short Status, long UserId);
        ListUserTypeRes ListAllUserType(short? status);

        ListWalletAuthorizeUserRes ListWalletAuthorizeUser(string WalletId);

        BizResponseClass InsertUpdateCommisssionType(CommisssionTypeReq Req, long UserId);
        BizResponseClass ChangeCommisssionTypeReqStatus(long Id, short Status, long UserId);

        BizResponseClass InsertUpdateWalletUsagePolicy(AddWalletUsagePolicyReq Req, long UserId);
        BizResponseClass ChangeWalletUsagePolicyStatus(long Id, short Status, long UserId);

        BizResponseClass InsertUpdateChargeType(ChargeTypeReq Req, long UserId);
        ListWalletPolicyAllowedDayRes GetWPolicyAllowedDay(long ID, EnWeekDays? DayNo, long? PolicyID, short? Status);
        ListWalletPolicyAllowedDayRes ListWPolicyAllowedDay(EnWeekDays? DayNo, long? PolicyID, short? Status);
        BizResponseClass ChangeChargeTypeStatus(long Id, short Status, long UserId);
        ListUserActivityLoging GetUserActivities(long? userID, DateTime? fromDate, DateTime? toDate);

        ListWalletResV1 ListAllWallet(DateTime? FromDate, DateTime? ToDate, short? Status, int PageSize, int Page, long? UserId, long? OrgId, string WalletType);
        WalletRes1 GetWalletIdWise(string AccWalletId);
        ListBlockTrnTypewiseReport GetBlockedTrnTypeWiseWalletDetail(enWalletTrnType type, int? PageNo, int? PageSize);
        ListWalletusagePolicy2 ListUsagePolicy(long? walletTypeID, short? status);
        Task<BizResponseClass> AddAllowedChannels(AllowedChannelReq Request, long UserID);
        ListChannels GetChannels(long channelID, short? status);

        Task<ListAllowTrnTypeRoleWise> ListAllowTrnTypeRoleWise(long? RoleId, long? TrnTypeId, short? Status);
        Task<BizResponseClass> ChangeAllowTrnTypeRoleStatus(short Status, long UserId, long Id);
        Task<BizResponseClass> InserUpdateAllowTrnTypeRole(InserUpdateAllowTrnTypeRoleReq Req, long UserId);
        Task<BizResponseClass> AddWalletTypeDetails(WalletTypeMasterReq Request, long UserID);
        WalletTypeMasterResp GetWalletTypeDetail(long typeID);
        Task<BizResponseClass> AddStakingPolicy(StakingPolicyReq Request, long UserID);
        Task<ListStakingPolicyDetailRes2> ListStakingPolicy(long StackingPolicyMasterId, EnStakingType? stakingType, EnStakingSlabType? slabType, short? status);
        Task<ListStakingPolicyDetailRes> GetStakingPolicy(long policyDetailID, short? status);
        Task<BizResponseClass> UpdateStakingPolicy(long PolicyDetailId, UpdateStakingDetailReq Request, long UserID);
        Task<ListStopLossRes> ListStopLoss(long? WalletTypeId, short? Status);
        Task<BizResponseClass> InserUpdateStopLoss(InserUpdateStopLossReq Request, long UserId);
        Task<BizResponseClass> ChangeStopLossStatus(short Status, long UserId, long Id);
        Task<BizResponseClass> ChangeStakingPolicyStatus(long PolicyDetailId, ServiceStatus Status, long UserID);

        Task<ListLeverageRes> ListLeverage(long? WalletTypeId, short? Status);
        Task<BizResponseClass> InserUpdateLeverage(InserUpdateLeverageReq Request, long UserId);
        Task<BizResponseClass> ChangeLeverageStatus(short Status, long UserId, long Id);
        Task<BizResponseClass> ExportAddressDetails(long? ServiceProviderID, long? UserID, long? WalletTypeID, long LoginUserID);

        Task<BizResponseClass> ExportWallet(string FileName, string Coin);
        Task<BizResponseClass> ImportAddressDetails(string WebRootPath, string FullPath);

        ListStakingPolicyRes ListStakingPolicyMaster(long? WalletTypeID, short? Status, short? enStakingSlabType, short? enStakingType);
        BizResponseClass ChangeStakingPolicyStatus(long Id, short Status, long UserId);
        BizResponseClass InsertUpdateStakingPolicy(InsertUpdateStakingPolicyReq Req, long UserId);

        ListAddressRes ListAddressDetails(long? ServiceProviderID, long? UserID, long? WalletTypeID, string Address, int PageNo, int PageSize);

        ListChargesTypeWise ListChargesTypeWise(string WalletTypeName, long? TrnTypeId);
        ListTrnChargeLogRes TrnChargeLogReport(int PageNo, int PageSize, short? Status, long? TrnTypeID, long? WalleTypeId, short? SlabType, DateTime? FromDate, DateTime? ToDate, long? TrnNo);

        ListTrnChargeLogResv2 TrnChargeLogReportv2(int PageNo, int PageSize, short? Status, long? TrnTypeID, long? WalleTypeId, short? SlabType, DateTime? FromDate, DateTime? ToDate,string TrnNo);

        ListDepositeCounterRes GetDepositCounter(long? WalletTypeID, long? SerProId, int PageNo, int PageSize);
        BizResponseClass InsertUpdateDepositCounter(InsertUpdateDepositCounterReq Request);
        BizResponseClass ChangeDepositCounterStatus(long Id, short Status);

        ListAdminAssetsres AdminAssets(long? WalletTypeId, EnWalletUsageType? WalletUsageType, long Userid, int PageNo, int PageSize);

        ListOrgLedger OrganizationLedger(long? WalletTypeId, EnWalletUsageType? WalletUsageType);
        Task<ListUnStakingHistory> ListUnStakingHistory(long? userid, short? status, EnUnstakeType unStakingType);
        Task<ListUnStakingHistoryv2> ListUnStakingHistoryv2(long? userid, short? status, EnUnstakeType unStakingType);
        Task<BizResponseClass> AdminUnstakeRequest(long adminReqID, ServiceStatus bit, long UserID, UserUnstakingReq Request);
        Task<BizResponseClass> AdminUnstakeRequestv2(string adminReqID, ServiceStatus bit, long UserID, UserUnstakingReqv2 Request);
        Task<BizResponseClass> AddNewChargeConfiguration(ChargeConfigurationMasterReq request, long id);
        Task<BizResponseClass> UpdateChargeConfiguration(long masterId, ChargeConfigurationMasterReq2 request, long id);
        Task<ChargeConfigurationMasterRes2> GetChargeConfiguration(long masterId);
        Task<ListChargeConfigurationMasterRes> ListChargeConfiguration(long? walletTypeId, long? trnType, short? slabType, short? status, int? PageSize, int? PageNo);
        Task<BizResponseClass> AddNewChargeConfigurationDetail(ChargeConfigurationDetailReq request, long id);
        Task<BizResponseClass> UpdateChargeConfigurationDetail(long detailId, ChargeConfigurationDetailReq request, long id);
        Task<ChargeConfigurationDetailRes2> GetChargeConfigurationDetail(long detailId);
        Task<ListChargeConfigurationDetailRes> ListChargeConfigurationDetail(long? masterId, long? chargeType, short? chargeValueType, short? chargeDistributionBasedOn, short? status);

        //ListLeaverageReportRes LeveragePendingReport(long? WalletTypeId, long? UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize);

        //ListLeaverageRes LeverageReport(long? WalletTypeId, long? UserId, DateTime FromDate, DateTime ToDate, int PageNo, int PageSize, short? Status);
        Task<DepositionHistoryRes> DepositionReport(string trnid, string address, short? isInternal, long? userID, string coinName, long? provider, int pageNo, int pageSize);
        Task<BizResponseClass> AddMasterLimitConfiguration(WalletTrnLimitConfigurationInsReq Request, long UserId);
        Task<BizResponseClass> UpdateMasterLimitConfiguration(WalletTrnLimitConfigurationUpdReq request, long UserId);
        Task<BizResponseClass> ChangeMasterLimitConfigStatus(ChangeServiceStatus request, long UserId);
        Task<ListWalletTrnLimitConfigResp> ListMasterLimitConfiguration(long? walletTypeId, long? trnType, EnIsKYCEnable? isKYCEnable, short? status);
        Task<GetWalletTrnLimitConfigResp> GetMasterLimitConfiguration(long id);

        ListTradingChargeTypeRes ListTradingChargeTypeMaster();
        ListMarketCurrencyRes ListChargeFreeMarketCurrencyMaster();

        BizResponseClass InsertTradingChargeType(InsertTradingChargeTypeReq Request);
        BizResponseClass InsertChargeFreeMarketCurrencyMaster(string Currency,long UserId, short Status);

        Task<ListDepositionReconRes> DepositionReconProcess(DepositionReconReq request, long id);
        
        long AddDepositionInterval(DepositionIntervalViewModel DepositionIntervalInsert, long UserID);
        ListDepositionIntervalResponse ListDepositionInterval();
        bool DisableDepositionInterval(DepositionIntervalStatusViewModel model, long UserId);
        bool EnableDepositionInterval(DepositionIntervalStatusViewModel model, long UserId);

        MultichainConnectionViewModel MultichainConnection(string chainName);
        Task<ServiceProviderBalanceResponse> GetSerProviderBalance(long ServiceProvider, enTrnType? TransactionType, string CurrencyName);
        Task<ListStakingHistoryRes> GetStackingHistoryData(DateTime? fromDate, DateTime? toDate, EnStakeUnStake? type, int pageSize, int pageNo, EnStakingSlabType? slab, EnStakingType? stakingType, long? userId);

        Task<ListStakingHistoryResv2> GetStackingHistoryDatav2(DateTime? fromDate, DateTime? toDate, EnStakeUnStake? type, int pageSize, int pageNo, EnStakingSlabType? slab, EnStakingType? stakingType, long? userId);

        Task<BizResponseClass> BlockUnblockUserAddress(BlockUserReqRes req, long Userid);
        Task<ListBlockUserRes> ListBlockUnblockUserAddress(long? userId, string address, DateTime? fromDate, DateTime? toDate, short? status);
        Task<BizResponseClass> DestroyBlackFund(DestroyBlackFundReq req, long Userid);
        Task<BizResponseClass> TokenTransfer(TokenTransferReq req, long Userid);
        Task<BizResponseClass> IncreaseTokenSupply(TokenSupplyReq req, long Userid);
        Task<BizResponseClass> DecreaseTokenSupply(TokenSupplyReq req, long Userid);

        Task<BizResponseClass> SetTransferFee(SetTransferFeeReq req, long Userid);
        Task<ListWithdrawalAdminRes> ListWithdrawalRequest(long? trnNo, DateTime? FromDate, DateTime? ToDate,short? status);
        Task<BizResponseClass> AdminWithdrawalRequest(long adminReqId, ApprovalStatus bit, long userid, string remarks);
        Task<ListTokenSupplyRes> IncreaseDecreaseTokenSupplyHistory(long? walletTypeId, short? actionType, DateTime? fromDate, DateTime? toDate);
        Task<ListDestroyBlackFundRes> DestroyedBlackFundHistory(string address, DateTime? fromDate, DateTime? toDate);
        Task<ListTokenTransferRes> TokenTransferHistory(DateTime? fromDate, DateTime? toDate);
        Task<ListSetTransferFeeRes> TransferFeeHistory(long? walletTypeId, DateTime? fromDate, DateTime? toDate);
        Task<BizResponseClass> GetToken();
        Task<LPServiceProBalanceResponse> GetLPProviderBalance(long SerProID, string SMSCode);

        ListLPWalletMismatchRes ListLPWalletMismatch(DateTime FromDate, DateTime ToDate, int Page, int PageSize, long WalletId, long SerProID, Int16 Status);

        BizResponseClass UpdatePayUCoinLimitConfiguration(ReqUpdatePayUCoinLimitConfiguration Req);
        Task<BizResponseClass> CreateTextFile(CreateTextFileReq textFileReq);   //add by akshay kothari
        //Task<BizResponseClass> CreateTextFile(string URL, string UserName, string Coin, string WalletTypeName,int AppType);

        //Rushabh 09-05-2020
        Task<UserWithdrawalResponse> GetTotalWithdrawalHistory(long UserId, string CurrencyName);
        Task<UserWithdrawalResponse> GetTotalDepositionHistory(long UserId, string CurrencyName);
        Task<UserAllWalletBalanceResp> GetAllWalletBalanceUserWise(long UserId, string CurrencyName);
        Task<UserTradingSummaryResp> GetUserTradeSummary(long UserId);
    }
}
