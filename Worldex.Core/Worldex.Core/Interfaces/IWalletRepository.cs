using Worldex.Core.ApiModels;
using Worldex.Core.Entities.MarginEntitiesWallet;
using Worldex.Core.Entities.User;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.BackOfficeReports;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.ViewModels.WalletOperations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces
{
    public interface IWalletRepository
    {
        Balance GetAllBalancesV1(long userid, long walletid);

        TradeBitGoDelayAddresses GetUnassignedETH();

        bool WalletOperation(WalletLedger wl1, WalletLedger wl2, TransactionAccount ta1, TransactionAccount ta2, WalletMaster wm2, WalletMaster wm1);

        List<WalletMasterResponse> ListWalletMasterResponse(long UserId);

        BalanceLat GetAllBalancesV1Lat(long userid, long walletid);// Rushabh 13-12-2019

        List<AddressMasterResponse> ListAddressMasterResponse(string AccWaletID); //Rushabh 15-10-2018

        List<WalletMasterResponse> GetWalletMasterResponseByCoin(long UserId, string coin);

        List<WalletMasterResponse> GetWalletMasterResponseById(long UserId, string coin, string walletId);

        Task<CheckTrnRefNoRes> CheckTranRefNoAsync(long TrnRefNo, enWalletTranxOrderType TrnType, enWalletTrnType enWalletTrn);

        int CheckTrnRefNoForCredit(long TrnRefNo, enWalletTranxOrderType TrnType);

        WalletTransactionQueue AddIntoWalletTransactionQueue(WalletTransactionQueue wtq, byte AddorUpdate);

        WalletTransactionOrder AddIntoWalletTransactionOrder(WalletTransactionOrder wo, byte AddorUpdate);

        bool CheckarryTrnID(CreditWalletDrArryTrnID[] arryTrnID, string coinName);

        //vsolanki 16-10-2018 
        WithdrawHistoryResponse DepositHistoy(DateTime FromDate, DateTime ToDate, string Coin, string TrnNo, decimal? Amount, byte? Status, long Userid, int PageNo);

        WithdrawHistoryResponsev2 DepositHistoyv2(DateTime FromDate, DateTime ToDate, string Coin, string TrnNo, decimal? Amount, byte? Status, long Userid, int PageNo);

        //vsolanki 16-10-2018 
        WithdrawHistoryNewResponse WithdrawalHistoy(DateTime FromDate, DateTime ToDate, string Coin, decimal? Amount, byte? Status, long Userid, int PageNo, short? IsInternalTransfer);

        WithdrawHistoryNewResponsev2 WithdrawalHistoyv2(DateTime FromDate, DateTime ToDate, string Coin, decimal? Amount, byte? Status, long Userid, int PageNo, short? IsInternalTransfer);

        bool WalletCreditwithTQ(WalletLedger wl1, TransactionAccount ta1, WalletMaster wm2, WalletTransactionQueue wtq, CreditWalletDrArryTrnID[] arryTrnID, decimal amount);

        List<WalletLimitConfigurationRes> GetWalletLimitResponse(string AccWaletID);

        List<AddressMasterResponse> GetAddressMasterResponse(string AccWaletID); //Rushabh 23-10-2018

        //vsolanki 24-10-2018
        List<BalanceResponse> GetAvailableBalance(long userid, long walletId);
        List<BalanceResponse> GetAllAvailableBalance(long userid);
        List<BalanceResponse> GetUnSettledBalance(long userid, long walletId);
        List<BalanceResponse> GetAllUnSettledBalance(long userid);
        List<BalanceResponse> GetUnClearedBalance(long userid, long walletId);
        List<BalanceResponse> GetUnAllClearedBalance(long userid);
        List<StackingBalanceRes> GetStackingBalance(long userid, long walletId);
        List<StackingBalanceRes> GetAllStackingBalance(long userid);
        List<BalanceResponse> GetShadowBalance(long userid, long walletId);
        List<BalanceResponse> GetAllShadowBalance(long userid);
        Balance GetAllBalances(long userid, long walletid);

        List<BeneficiaryMasterRes1> GetAllWhitelistedBeneficiaries(long WalletTypeID, long UserId,short? IsInternalAddress,long WalletId);

        List<BeneficiaryMasterRes> GetAllBeneficiaries(long UserID);
        //vsolanki 25-10-2018
        List<BalanceResponseLimit> GetAvailbleBalTypeWise(long userid);

        decimal NewGetTotalAvailbleBal(long userid);

        BeneUpdate BeneficiaryBulkEdit(string id, short bit);

        //vsolanki 24-10-2018 
        decimal GetTodayAmountOfTQ(long userId, long WalletId);
        List<WalletLedgerRes> GetWalletLedger(DateTime FromDate, DateTime ToDate, long WalletId, int page);

        List<WalletLedgerRes> GetWalletLedgerV1(DateTime FromDate, DateTime ToDate, long WalletId, int page, int PageSize, ref int TotalCount);

        Task<int> CreateDefaulWallet(long UserId);

        int CreateWalletForAllUser_NewService(string WalletType);

        //vsolanki 2018-10-29
        int AddBizUserTypeMapping(BizUserTypeMapping bizUser);

        long GetTypeMappingObj(long userid);

        Task<long> GetTypeMappingObjAsync(long userid);
        //vsolanki 2018-10-29
        List<IncomingTrnRes> GetIncomingTransaction(long Userid, string Coin);
        List<IncomingTrnResv2> GetIncomingTransactionv2(long Userid, string Coin);

        long getOrgID();

        // ntrivedi 29102018
        WalletTransactionQueue GetTransactionQueue(long TrnNo);

        decimal GetLedgerLastPostBal(long walletId);

        List<OutgoingTrnRes> GetOutGoingTransaction(long Userid, string Coin);
        List<OutgoingTrnResv2> GetOutGoingTransactionv2(long Userid, string Coin);

        List<TransfersRes> GetTransferIn(string Coin, int Page, int PageSize, long? UserId, string Address, string TrnID, long? OrgId, ref int TotalCount);

        List<TransfersRes> TransferOutHistory(string CoinName, int Page, int PageSize, long? UserId, string Address, string TrnID, long? OrgId, ref int TotalCount);

        int CheckTrnRefNo(long TrnRefNo, enWalletTranxOrderType TrnType, enWalletTrnType walletTrnType);

        Task<bool> CheckTrnIDDrForHoldAsync(CommonClassCrDr arryTrnID);

        void ReloadEntity(WalletMaster wm1, WalletMaster wm2, WalletMaster wm3, WalletMaster wm4);

        Task<bool> CheckTrnIDDrForMarketAsync(CommonClassCrDr arryTrnID);

        bool CheckUserBalanceV1(long WalletId, enBalanceType enBalance = enBalanceType.AvailableBalance, EnWalletUsageType enWalletUsageType = EnWalletUsageType.Trading_Wallet);

        Task<ApplicationUser> GetUserById(long id);

        Task<AllSumAmount> GetSumForPolicy(long WalletType, long TrnType);

        BeneUpdate UpdateDefaultWallets(long WalletTypeID, long UserID);

        List<AddWalletRequestRes> ListAddUserWalletRequest(long UserId);

        Task<List<UserWalletWise>> ListUserWalletWise(long WalletId);

        List<StakingPolicyDetailRes> GetStakingPolicyData(short statkingTypeID, short currencyTypeID);
        List<StakingPolicyDetailResV2> GetStakingPolicyDataV2(short statkingTypeID, short currencyTypeID);
        PreStackingConfirmationRes GetPreStackingData(long PolicyDetailID);

        Task<List<WalletMasterRes>> ListWalletMasterResponseNew(long UserId, string Coin);
        Task<List<WalletMasterRes>> GetWalletMasterResponseByCoinNew(long UserId, string coin);
        Task<List<WalletMasterRes>> GetWalletMasterResponseByIdNew(long UserId, string walletId);
        Balance GetAllBalancesNew(long userid, long walletid);
        List<BalanceResponse> GetAvailableBalanceNew(long userid, long walletId);
        List<BalanceResponse> GetAllAvailableBalanceNew(long userid);
        decimal GetTotalAvailbleBalNew(long userid);
        List<BalanceResponseLimit> GetAvailbleBalTypeWiseNew(long userid);
        List<StakingHistoryRes> GetStackingHistoryData(DateTime? fromDate, DateTime? toDate, EnStakeUnStake? type, int pageSize, int pageNo, EnStakingSlabType? slab, EnStakingType? stakingType, long userID, ref int TotalCount);

        List<StakingHistoryResv2> GetStackingHistoryDatav2(DateTime? fromDate, DateTime? toDate, EnStakeUnStake? type, int pageSize, int pageNo, EnStakingSlabType? slab, EnStakingType? stakingType, long userID, ref int TotalCount);
        int IsSelfAddress(string address, long userID, string smscode);

        int IsInternalAddress(string address, long userID, string smscode);
        List<WalletTransactiondata> GetWalletStatisticsdata(long userID, short month, short year);
        List<TranDetails> GetYearlyWalletStatisticsdata(long userID, short year);
        bool AddAddressIntoDB(long userID, string Address, string TxnID, string Key, long SerProDetailId, short Islocal);

        List<LeaderBoardRes> LeaderBoard(int? UserCount,long[] LeaderId);
        List<LeaderBoardRes> LeaderBoardWeekWiseTopFive(long[] LeaderId, DateTime Date, short IsGainer,int Count);

        List<ViewModels.Transaction.HistoricalPerformanceTemp> GetHistoricalPerformanceYearWise(long UserId, int Year);
        long FindChargeValueReleaseWalletId(string Timestamp, long TrnRefNo);

        decimal FindChargeValueHold(string Timestamp, long TrnRefNo);

        decimal FindChargeValueDeduct(string Timestamp, long TrnRefNo);

        decimal FindChargeValueRelease(string Timestamp, long TrnRefNo);

        long FindChargeValueWalletId(string Timestamp, long TrnRefNo);

        string FindChargeCurrencyDeduct(long TrnRefNo);

        TransactionPolicyRes ListTransactionPolicy(long TrnType, long userId);
        List<WalletType> GetChargeWalletType(long? id);
        List<ChargesTypeWise> ListChargesTypeWise(long WalletTypeId, long? TrntypeId);

        //khushali 11-04-2019 Process for Release Stuck Order - wallet side   
        enTransactionStatus CheckTransactionSuccessOrNot(long TrnRefNo);
        bool CheckSettlementProceed(long MakerTrnNo, long TakerTrnNo);
        List<UserUnstakingReq2> GetStakingdataForChrone();
        Decimal GetTransactionSettledQty(long TrnRefNo);//komal 03-07-2019 get settled qty 

        ValidationWithdrawal CheckActivityLog(long UserId, int Type);

        List<WalletMasterResponsev2> GetTransactionWalletMasterResponseByCoin(long UserId, string coin);
    }

    public interface IWalletTQInsert
    {
        WalletTransactionQueue AddIntoWalletTransactionQueue(WalletTransactionQueue wtq, byte AddorUpdate);


    }
}
