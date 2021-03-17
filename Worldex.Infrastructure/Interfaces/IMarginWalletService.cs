using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.ViewModels.WalletConfiguration;
using System;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Interfaces
{
    public interface IMarginWalletService
    {
        MarginPreConfirmationRes GetMarginPreConfirmationData(long WalletTypeId, decimal Amount, string AccWalletID, long UserId, short LeverageChargeDeductionType, decimal Leverage);

        BizResponseClass InsertMarginRequest(long WalletTypeId, decimal Amount, string AccWalletID, long UserId, short LeverageChargeDeductionType, decimal Leverage);

        Task<ListMarginWalletByUserIdRes> GetMarginWalletByUserId(long UserId);

        BizResponseClass CreateMarginWallet(long WalletTypeId, long UserId);

        Task<ListMarginWallet> ListMarginWalletMaster(long? WalletTypeId, EnWalletUsageType? WalletUsageType, short? Status, string AccWalletId, long? UserId);

        Task<ListMarginWallet2> ListMarginWallet(int PageNo, int PageSize,long? WalletTypeId, EnWalletUsageType? WalletUsageType, short? Status, string AccWalletId, long? UserId);

        ListMarginWalletTypeMasterResponse ListAllWalletTypeMaster();

        BizResponseClass AdminMarginChargeRequestApproval(short IsApproved, long ReuestId,string Remarks);
        BizResponseClass AdminMarginChargeRequestApprovalv2(short IsApproved, string ReuestId,string Remarks);

        ListLeaverageRes LeverageRequestReport(long? WalletTypeId, long UserId,  DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize, short? Status);
        ListLeaverageResv2 LeverageRequestReportv2(long? WalletTypeId, long UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize, short? Status);

        ListLeaverageReportRes LeveragePendingReport(long? WalletTypeId, long? UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize);
        ListLeaverageReportResv2 LeveragePendingReportv2(long? WalletTypeId, long? UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize);

        ListLeaverageRes LeverageReport(long? WalletTypeId, long? UserId, DateTime FromDate, DateTime ToDate, int PageNo, int PageSize, short? Status);

        ListLeaverageResv2 LeverageReportv2(long? WalletTypeId, long? UserId, DateTime FromDate, DateTime ToDate, int PageNo, int PageSize, short? Status);

        Task<ListLeverageRes> ListLeverage(long? WalletTypeId, short? Status);

        Task<BizResponseClass> InserUpdateLeverage(InserUpdateLeverageReq Request, long UserId);

        Task<BizResponseClass> ChangeLeverageStatus(short Status, long UserId, long Id);

        ListChargesTypeWise ListMarginChargesTypeWise(string WalletTypeName, long? TrnTypeId);

        ListTrnChargeLogRes MarginTrnChargeLogReport(int PageNo, int PageSize, short? Status, long? TrnTypeID, long? WalleTypeId, short? SlabType, DateTime? FromDate, DateTime? ToDate, long? TrnNo);

        ListWalletLedgerResv1 GetMarginWalletLedger(DateTime FromDate, DateTime ToDate, string WalletId, int page, int PageSize);

        ListWalletLedgerResponse GetMarginWalletLedgerv2(DateTime FromDate, DateTime ToDate, string WalletId, int page, int PageSize);

        //ntrivedi 05-03-2019
        PairLeverageDetailRes GetPairLeverageDetail(string FirstCurr, string SecondCurrency, long userID);
        //ntrivedi 27-03-2019
        Task<ListLeverageRes> ListLeverageBaseCurrency(long? WalletTypeId, short? Status);

        BizResponseClass CreateAllMarginWallet(long UserId);

        Task<StopLimitOrderPrice> ReCalculateInternalOrderPrice(long UserID, long PairID, string baseCurrency);

        MarginWithdrawPreConfirmResponse MarginWithdrawPreConfirm(long UserId, string Currency);//ntrivedi 15-04-2019
        BizResponseClass UpgradeLoan(long UserID,long LoanID, decimal LeverageX);//ntrivedi 15-04-2019

        PNLAccountRes GetProfitNLossData(int pageNo, int? pageSize, long? pairId, string currencyName, long id);

        MarginWithdrawPreConfirmResponse MarginWithdraw(long UserId, string Currency);

        OpenPositionRes GetOpenPosition(long? pairId, long? Userid);

        ListMarginWalletTypeMasterResponse ListAllWalletTypeMasterV2();

        MarginWalletTypeMasterResponse AddWalletTypeMaster(WalletTypeMasterRequest addWalletTypeMasterRequest, long Userid);

        MarginWalletTypeMasterResponse UpdateWalletTypeMaster(WalletTypeMasterUpdateRequest updateWalletTypeMasterRequest, long Userid, long WalletTypeId);

        MarginWalletTypeMasterResponse GetWalletTypeMasterById(long WalletTypeId);

        BizResponseClass DisableWalletTypeMaster(long WalletTypeId);
        
        //khushali 05-11-2019 Process for Release Stuck Order - wallet side  Margin 
        enTransactionStatus CheckTransactionSuccessOrNot(long TrnRefNo);
        bool CheckSettlementProceed(long MakerTrnNo, long TakerTrnNo);
    }
}
