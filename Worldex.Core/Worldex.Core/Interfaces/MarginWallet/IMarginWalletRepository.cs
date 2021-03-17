using Worldex.Core.ApiModels;
using Worldex.Core.Entities.MarginEntitiesWallet;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.ViewModels.WalletOperations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces.MarginWallet
{
    public interface IMarginWalletRepository
    {
        List<MarginWalletMasterRes> ListMarginWalletMaster(long? WalletTypeId, EnWalletUsageType? WalletUsageType, short? Status, string AccWalletId, long? UserId);

        List<MarginWalletMasterRes2> ListMarginWallet(int PageNo, int PageSize, long? WalletTypeId, EnWalletUsageType? WalletUsageType, short? Status, string AccWalletId, long? UserId, ref int TotalCount);

        List<MarginWalletByUserIdRes> GetMarginWalletByUserId(long UserId);

        Task<bool> CheckTrnIDDrForHoldAsync(MarginCommonClassCrDr arryTrnID);

        Task<bool> CheckTrnIDDrForMarketAsync(MarginCommonClassCrDr arryTrnID);

        void ReloadEntity(MarginWalletMaster wm1, MarginWalletMaster wm2, MarginWalletMaster wm3, MarginWalletMaster wm4);

        List<LeaverageReport> LeverageRequestReport(long? WalletTypeId, long UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize, short? Status, ref int TotalCount);

        List<LeaverageReportv2> LeverageRequestReportv2(long? WalletTypeId, long UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize, short? Status, ref int TotalCount);

        List<LeaverageReportRes> LeveragePendingReport(long? WalletTypeId, long? UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize, ref int TotalCount);
        List<LeaverageReportResv2> LeveragePendingReportv2(long? WalletTypeId, long? UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize, ref int TotalCount);

        List<LeaverageReport> LeverageReport(long? WalletTypeId, long? UserId, DateTime FromDate, DateTime ToDate, int PageNo, int PageSize, short? Status, ref int TotalCount);

        List<LeaverageReportv2> LeverageReportv2(long? WalletTypeId, long? UserId, DateTime FromDate, DateTime ToDate, int PageNo, int PageSize, short? Status, ref int TotalCount);

        Task<List<LeverageRes>> ListLeverage(long? WalletTypeId, short? Status);

        List<ChargesTypeWise> ListMarginChargesTypeWise(long WalletTypeId, long? TrntypeId);

        List<WalletType> GetMarginChargeWalletType(long? WalletTypeId);

        List<TrnChargeLogRes> MarginTrnChargeLogReport(int PageNo, int PageSize, short? Status, long? TrnTypeID, long? WalleTypeId, short? SlabType, DateTime? FromDate, DateTime? ToDate, long? TrnNo, ref long TotalCount);

        decimal FindChargeValueHold(string Timestamp, long TrnRefNo);

        decimal FindChargeValueDeduct(string Timestamp, long TrnRefNo);

        decimal FindChargeValueRelease(string Timestamp, long TrnRefNo);

        long FindChargeValueWalletId(string Timestamp, long TrnRefNo);

        List<WalletLedgerRes> GetMarginWalletLedger(DateTime FromDate, DateTime ToDate, long WalletId, int page, int PageSize, ref int TotalCount);
        //ntrivedi 05-03-2019 added
        LeveragePairDetail GetPairLeverageDetail(long WalletID);

        Task<List<LeverageRes>> ListLeverageBaseCurrency(long? WalletTypeId, short? Status);

        PositionValue GetPositionDetailValue(long OpenPositionMasterID); //ntrivedi added 04-03-2019

        OpenPositionMaster GetPositionMasterValue(long PairID, long UserID); //ntrivedi added 04-03-2019
        List<PNLAccount> GetProfitNLossData(long? pairId, string currencyName, long id);
        List<OpenPosition> GetOpenPosition(long pairId, long userid);
        OpenPositionMaster GetPositionOpenInOtherPair(long PairID, long UserID);//Rita 26-4-18 for check open position on other pair , open by site token conversion 

        OpenPositionMaster GetPairPositionMasterValue(long UserID); //ntrivedi for settle 01-05-2019

        //khushali 11-04-2019 Process for Release Stuck Order - wallet side   
        enTransactionStatus CheckTransactionSuccessOrNot(long TrnRefNo);
        bool CheckSettlementProceed(long MakerTrnNo, long TakerTrnNo);
        List<WalletMasterResponsev2> GetTransactionWalletMasterResponseByCoin(long UserId, string coin);
    }

    public interface ILPWalletRepository //ntrivedi for Liquidity provider 28-05-2019
    {
        void ReloadEntitySingle(WalletMaster wm1, LPWalletMaster wm2, WalletMaster wm3);
    }

    

}
