using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.User;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.ViewModels.WalletOperations;
using Worldex.Core.ViewModels.WalletOpnAdvanced;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Interfaces
{

    public interface IWalletService
    {
        string RandomGenerateAccWalletId(long userID, byte isDefaultWallet);

        Task<CreateWalletAddressRes> GenerateAddress(string walletid, string coin, string Token, int GenaratePendingbit = 0, long userId = 0);
        //vsolanki 10-10-2018
        Task<CreateWalletResponse> InsertIntoWalletMaster(string Walletname, string CoinName, byte IsDefaultWallet, int[] AllowTrnType, long userId, string accessToken = null, int isBaseService = 0, long OrgId = 0, DateTime? ExpiryDate = null);

        ListWalletResponse ListWallet(long userid);

        ListWalletResponse GetWalletByCoin(long userid, string coin);

        ListWalletResponse GetWalletById(long userid, string coin, string walletId);

        //Rushabh 13-12-2019 Added New Method As Per Client New Requirement
        ListAllBalanceTypeWiseResLat GetAllBalancesTypeWiseLat(long userId, string WalletType);

        Task<WalletDrCrResponse> GetWalletDeductionNew(string coinName, string timestamp, enWalletTranxOrderType orderType, decimal amount, long userID, string accWalletID, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enTrnType routeTrnType, string Token = "");

        ListWalletAddressResponse ListAddress(string AccWalletID);

        WithdrawHistoryResponse DepositHistoy(DateTime FromDate, DateTime ToDate, string Coin, decimal? Amount, byte? Status, string TrnNo, long Userid, int PageNo);

        WithdrawHistoryResponsev2 DepositHistoyv2(DateTime FromDate, DateTime ToDate, string Coin, decimal? Amount, byte? Status,string TrnId, long Userid, int PageNo);

        //vsolanki 16-10-2018
        //RUSHABH 11-12-2018
        WithdrawHistoryNewResponse WithdrawalHistoy(DateTime FromDate, DateTime ToDate, string Coin, decimal? Amount, byte? Status, long Userid, int PageNo, short? IsInternalTransfer);

        WithdrawHistoryNewResponsev2 WithdrawalHistoyv2(DateTime FromDate, DateTime ToDate, string Coin, decimal? Amount, byte? Status, long Userid, int PageNo, short? IsInternalTransfer);

        //ntrivedi 16-10-2018
        //WalletDrCrResponse GetWalletCreditNew(string coinName, string timestamp, enWalletTrnType trnType, decimal TotalAmount, long userID, string crAccWalletID, CreditWalletDrArryTrnID[] arryTrnID, long TrnRefNo, short isFullSettled, enWalletTranxOrderType orderType, enServiceType serviceType, enTrnType routeTrnType, string Token = "");

        //Rushabh 16-10-2018
        Task<LimitResponse> SetWalletLimitConfig(string accWalletID, WalletLimitConfigurationReq request, long userID, string Token);

        //Rushabh 16-10-2018
        LimitResponse GetWalletLimitConfig(string accWalletID);
        ListWalletAddressResponse GetAddress(string AccWalletID);

        //vsolanki 24-10-2018
        ListBalanceResponse GetAvailableBalance(long userid, string walletId);
        TotalBalanceRes GetAllAvailableBalance(long userid);
        //vsolanki 24-10-2018
        ListBalanceResponse GetUnSettledBalance(long userid, string walletId);
        ListBalanceResponse GetAllUnSettledBalance(long userid);
        //vsolanki 24-10-2018
        ListBalanceResponse GetUnClearedBalance(long userid, string walletId);
        ListBalanceResponse GetAllUnClearedBalance(long userid);
        Task<BizResponseClass> AddConvertedAddress(string address, string convertedAddress, long id);

        //vsolanki 24-10-2018
        ListStackingBalanceRes GetStackingBalance(long userid, string walletId);
        ListStackingBalanceRes GetAllStackingBalance(long userid);
        //vsolanki 24-10-2018
        ListBalanceResponse GetShadowBalance(long userid, string walletId);
        ListBalanceResponse GetAllShadowBalance(long userid);
        //vsolanki 24-10-2018
        AllBalanceResponse GetAllBalances(long userid, string walletId);
        // vsolanki 25-10-2018
        BalanceResponseWithLimit GetAvailbleBalTypeWise(long userid);

        BeneficiaryResponse AddBeneficiary(string CoinName, short WhitelistingBit, string Name, string BeneficiaryAddress, long UserId, string Token);
        BeneficiaryResponse1 ListWhitelistedBeneficiary(string accWalletID, long id, short? IsInternalAddress);
        BeneficiaryResponse ListBeneficiary(long id);
        UserPreferencesRes SetPreferences(long Userid, int GlobalBit, string Token);
        UserPreferencesRes GetPreferences(long Userid);
        BeneficiaryResponse UpdateBulkBeneficiary(BulkBeneUpdateReq request, long id, string Token);
        BeneficiaryResponse UpdateBeneficiaryDetails(BeneficiaryUpdateReq request, string AccWalletID, long id, string Token);
        //vsolanki 25-10-2018
        ListAllBalanceTypeWiseRes GetAllBalancesTypeWise(long userId, string WalletType);
        ListWalletLedgerRes GetWalletLedger(DateTime FromDate, DateTime ToDate, string WalletId, int page);
        ListWalletLedgerResv1 GetWalletLedgerV1(DateTime FromDate, DateTime ToDate, string WalletId, int page, int PageSize);
        ListWalletLedgerResponse GetWalletLedgerv2(DateTime FromDate, DateTime ToDate, string WalletId, int page, int PageSize);

        Task<BizResponseClass> CreateDefaulWallet(long UserID, string accessToken = null);
        BizResponseClass CreateWalletForAllUser_NewService(string WalletType);

        //vsolanki 2018-10-29
        BizResponseClass AddBizUserTypeMapping(AddBizUserTypeMappingReq req);

        Task<long> GetWalletID(string AccWalletID);
        Task<string> GetDefaultAccWalletID(string SMSCode, long UserID);//rita 9-1-19 for 
        Task<enErrorCode> CheckWithdrawalBene(long WalletID, string Name, string DestinationAddress, enWhiteListingBit WhitelistingBit);

        WalletTransactionQueue InsertIntoWalletTransactionQueue(Guid Guid, enWalletTranxOrderType TrnType, decimal Amount, long TrnRefNo, DateTime TrnDate, DateTime? UpdatedDate,
           long WalletID, string WalletType, long MemberID, string TimeStamp, enTransactionStatus Status, string StatusMsg, enWalletTrnType enWalletTrnType);
        Task<BizResponseClass> UpdateWalletDetail(string AccWalletID, string walletName, short? status, byte? isDefaultWallet, long UserID);

        //vsolanki 2018-10-29
        ListIncomingTrnRes GetIncomingTransaction(long Userid, string Coin);
        ListIncomingTrnResv2 GetIncomingTransactionv2(long Userid, string Coin);

        //Uday 30-10-2018
        Task<ServiceLimitChargeValue> GetServiceLimitChargeValue(enWalletTrnType TrnType, string CoinName);

        //vsoalnki 2018-10-31
        Task<CreateWalletAddressRes> CreateETHAddress(string Coin, int AddressCount, long UserId, string accessToken);

        ListOutgoingTrnRes GetOutGoingTransaction(long Userid, string Coin);
        ListOutgoingTrnResv2 GetOutGoingTransactionv2(long Userid, string Coin);

        bool InsertIntoWithdrawHistory(WithdrawHistory req);

        Task<bool> CheckUserBalanceAsync(decimal amount, long WalletId, enBalanceType enBalance, EnWalletUsageType enWalletUsageType = EnWalletUsageType.Trading_Wallet);  //ntrivedi 13-02-2019 added so margin wallet do not use in other transaction

        Task<WalletDrCrResponse> GetWalletHoldNew(string coinName, string timestamp, decimal amount, string accWalletID, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enTrnType routeTrnType, EnAllowedChannels allowedChannels, string Token = "", enWalletDeductionType enWalletDeductionType = enWalletDeductionType.Normal, string MarketCurrency = "");

        Task<WalletDrCrResponse> GetWalletCreditDrForHoldNewAsyncFinal(CommonClassCrDr firstCurrObj, CommonClassCrDr secondCurrObj, string timestamp, enServiceType serviceType, EnAllowedChannels enAllowedChannels, enWalletDeductionType enWalletDeductionType = enWalletDeductionType.Normal);

        Task<WalletDrCrResponse> GetReleaseHoldNew(string coinName, string timestamp, decimal amount, string accWalletID, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enTrnType routeTrnType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, string Token = "");

        Task<TransactionWalletResponse> GetTransactionWalletByCoin(long userid, string coin);

        Task EmailSendAsyncV1(EnTemplateType templateType, string UserID, string Param1 = "", string Param2 = "", string Param3 = "", string Param4 = "", string Param5 = "", string Param6 = "", string Param7 = "", string Param8 = "", string Param9 = "");

        //2018-12-6
        Task SMSSendAsyncV1(EnTemplateType templateType, string UserID, string WalletName = null, string SourcePrice = null, string DestinationPrice = null, string ONOFF = null, string Coin = null, string TrnType = null, string TrnNo = null);

        //2018-12-20
        ListAddWalletRequest ListAddUserWalletRequest(long UserId);
        Task<BizResponseClass> UpdateUserWalletPendingRequest(short Status, long RequestId, long UserId);
        Task<BizResponseClass> InsertUserWalletPendingRequest(InsertWalletRequest request, long UserId);

        Task<ListUserWalletWise> ListUserWalletWise(string WalletId);

        Task<ListStakingPolicyDetailRes> GetStakingPolicy(short statkingTypeID, short currencyTypeID);
        Task<ListStakingPolicyDetailResV2> GetStakingPolicyV2(short statkingTypeID, short currencyTypeID);
        Task<BizResponseClass> UserStackingRequest(StakingHistoryReq StakingHistoryReq, long UserID);
        Task<BizResponseClass> UserStackingRequestv2(StakingHistoryReq StakingHistoryReq, long UserID);
        Task<ListPreStackingConfirmationRes> GetPreStackingData(PreStackingConfirmationReq request, long UserId);

        //listing method
        Task<ListWalletMasterRes> ListWalletMasterResponseNew(long UserId, string Coin);
        ListWalletResNew GetWalletByCoinNew(long userid, string coin);
        ListWalletResNew GetWalletByIdNew(long userid, string walletId);
        //balance API
        BalanceResponseWithLimit GetAvailbleBalTypeWiseNew(long userid);
        TotalBalanceRes GetAllAvailableBalanceNew(long userid);
        ListBalanceResponse GetAvailableBalanceNew(long userid, string walletId);
        AllBalanceResponse GetAllBalancesNew(long userid, string walletId);
        ListAllBalanceTypeWiseRes GetAllBalancesTypeWiseNew(long userId, string WalletType);
        Task<ListStakingHistoryRes> GetStackingHistoryData(DateTime? FromDate, DateTime? ToDate, EnStakeUnStake? Type, int PageSize, int PageNo, EnStakingSlabType? Slab, EnStakingType? StakingType, long UserID);
        Task<ListStakingHistoryResv2> GetStackingHistoryDatav2(DateTime? FromDate, DateTime? ToDate, EnStakeUnStake? Type, int PageSize, int PageNo, EnStakingSlabType? Slab, EnStakingType? StakingType, long UserID);
        Task<UnstakingDetailRes> GetPreUnstackingData(PreUnstackingConfirmationReq Request, long UserID);
        Task<UnstakingDetailResv2> GetPreUnstackingDatav2(PreUnstackingConfirmationReqv2 Request, long UserID);
        Task<BizResponseClass> UserUnstackingRequest(UserUnstakingReq request, long UserID);
        Task<BizResponseClass> UserUnstackingRequestv2(UserUnstakingReqv2 request, long UserID);

        StatisticsDetailData GetMonthwiseWalletStatistics(long UserID, short Month, short Year);
        StatisticsDetailData2 GetYearwiseWalletStatistics(long UserID, short Year);
        Task<BizResponseClass> ColdWallet(string Coin, InsertColdWalletRequest req, long UserId);
        Task<BizResponseClass> ValidateAddress(string TrnAccountNo, int Length, string StartsWith, string AccNoValidationRegex);
        CreateWalletAddressRes CreateERC20Address(long UserId, string Coin, string AccWalletId, short IsLocal = 0);

        ListLeaderBoardRes LeaderBoard(int? UserCount);
        ListLeaderBoardRes LeaderBoardWeekWiseTopFive(long[] LeaderId, DateTime Date, short IsGainer, int Count);
        Task<ServiceLimitChargeValue> GetServiceLimitChargeValueV2(enWalletTrnType trnType, string coinName, long userId);

        GetTransactionPolicyRes ListTransactionPolicy(long TrnType, long userId);

        // khushali 23-03-2019 For Success and Debit Reocn Process
        Task<WalletDrCrResponse> GetDebitWalletWithCharge(string coinName, string timestamp, decimal amount, string accWalletID, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enTrnType routeTrnType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, string Token = "", enWalletDeductionType enWalletDeductionType = enWalletDeductionType.Normal);
        ListChargesTypeWise ListChargesTypeWise(string WalletTypeName, long? TrnTypeId, long UserId);

        //khushali 11-04-2019 Process for Release Stuck Order - wallet side   
        enTransactionStatus CheckTransactionSuccessOrNot(long TrnRefNo);
        bool CheckSettlementProceed(long MakerTrnNo, long TakerTrnNo);
        ListUserUnstakingReq2 GetUnstackingCroneData();
        Decimal GetTransactionSettledQty(long TrnRefNo);//komal 03-07-2019 get settled qty

        int CheckActivityLog(long UserId, int Type);

        BizResponseClass AddTradingChartData(TradingChartDataReq req, long UserId);

        TradingChartDataRes GetTradingChartData(long PairId, long UserId);

        //2019-7-25 vsolanki added new mwthod to validate user
        ValidateUserForInternalTransferRes ValidateUserForInternalTransfer(string Email, string CoinName, ApplicationUser user);
    }
}