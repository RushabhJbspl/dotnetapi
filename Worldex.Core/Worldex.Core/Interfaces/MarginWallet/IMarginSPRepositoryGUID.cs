using Worldex.Core.ApiModels;
using Worldex.Core.Entities.MarginEntitiesWallet;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.Wallet;
using System;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.MarginWallet
{
    public interface IMarginSPRepositoriesGUID
    {
        BizResponseClass Callsp_HoldWallet(MarginWalletMaster dWalletobj, string timestamp, enServiceType serviceType, decimal amount, string coin, EnAllowedChannels channelType, long WalletType, long TrnRefNo, long walletID, long UserID, enTrnType TrnType, enMarginWalletTrnType walletTrnType, ref long TrnNo, enWalletDeductionType enWalletDeductionType, string RefGuid = "");

        BizResponseClass Callsp_CrDrWalletForHold(MarginPNL PNLObj, MarginCommonClassCrDr firstCurrObj, MarginCommonClassCrDr secondCurrObj, string timestamp, enServiceType serviceType, long firstCurrWalletType, long secondCurrWalletType, long channelType);

        BizResponseClass Callsp_ReleaseHoldWallet(MarginWalletMaster dWalletobj, string timestamp, enServiceType serviceType, decimal amount, string coin, EnAllowedChannels channelType, long WalletType, long TrnRefNo, long walletID, long UserID, enTrnType TrnType, enMarginWalletTrnType walletTrnType, ref long trnNo);

        BizResponseClass Callsp_HoldWallet_MarketTrade(MarginWalletMaster dWalletobj, string timestamp, enServiceType serviceType, decimal amount, string coin, EnAllowedChannels channelType, long WalletType, long TrnRefNo, long walletID, long UserID, enTrnType TrnType, enMarginWalletTrnType walletTrnType, enWalletDeductionType enWalletDeductionType);

    
        BizResponseClass Callsp_IsValidWalletTransaction(long WalletID, long UserID, long WalletTypeID, long ChannelID, long WalletTrnType);

        MarginPreConfirmationRes CallSP_MarginFundTransferCalculation(long WalletTypeId, decimal Amount, long UserID, long WalletID, short LeverageChargeDeductionType, decimal Leverage);

        BizResponseClass CallSP_MarginProcess(long WalletTypeId, decimal Amount, long UserID, long WalletID, string TimeStamp, short LeverageChargeDeductionType, ref long RequestId, decimal Leverage);

        BizResponseClass CreateMarginWallet(long WalletTypeId, long UserId);

        BizResponseClass CreateAllMarginWallet(long UserId);

        GetMemberBalRes Callsp_MarginGetMemberBalance(long walletID, long UserID, long WalletMasterID, short BalanceType, decimal Amount, int WalletUsageType);

        BizResponseClass CallSP_AdminMarginChargeRequestApproval(short IsApproved, long ReuestId, string TimeStamp, string Remarks);
        BizResponseClass CallSP_AdminMarginChargeRequestApprovalv2(short IsApproved, string ReuestId, string TimeStamp, string Remarks);

        BizResponseClass CallSP_CreateMarginWalletForAllWalletType(long UserId);

        BizResponseClass Callsp_MarginChargeWalletCallBGTaskNew(ref long BatchNo); //ntrivedi 10-04-2019

        BizResponseClass Callsp_MarginProcessLeverageAccountEOD(long loanID, long BatchNo, short ActionType); //ntrivedi 10-04-2019

        MarginWithdrawPreConfirmResponse CallSP_MarginWithdrawCalc(long UserId, string currency); //ntrivedi 13-04-2019

        MarginWithdrawPreConfirmResponse CallSP_MarginWithdraw(long UserId, string currency);//ntrivedi 13-04-2019

        BizResponseClass CallSP_UpgradeLoan(long UserID, long LoanID, decimal LeverageX);//ntrivedi 15-04-2019
        List<WalletLedgerResponse> CallGetMarginWalletLedger(DateTime FromDate, DateTime ToDate, long WalletId, int page, int? PageSize, ref int TotalCount);
    }

    //public interface ILPSPRepositories
    //{
    //    GetMemberBalRes Callsp_LPGetMemberBalance(long walletID, long SerProID, long WalletMasterID, short BalanceType, decimal Amount, int WalletUsageType);

    //    BizResponseClass Callsp_HoldWallet(LPHoldDr lPHoldDr, LPWalletMaster dWalletobj);

    //    BizResponseClass Callsp_LPCrDrWalletForHold(ArbitrageCommonClassCrDr firstCurrObj, string timestamp, enServiceType serviceType, long firstCurrWalletType, long secondCurrWalletType, long channelType = (long)EnAllowedChannels.Web);
    //}

    //public interface IArbitrageSPRepositories
    //{
    //    List<WalletLedgerResponse> CallGetArbitrageWalletLedger(DateTime FromDate, DateTime ToDate, long WalletId, int page, int? PageSize, ref int TotalCount);

    //    BizResponseClass sp_CreateMarginWallet(string SMSCode, long UserId);

    //    BizResponseClass Callsp_ArbitrageHoldWallet(LPHoldDr lPHoldDr, LPArbitrageWalletMaster dWalletobj);

    //    BizResponseClass Callsp_HoldWallet(ArbitrageWalletMaster dWalletobj, string timestamp, enServiceType serviceType, decimal amount, string coin, EnAllowedChannels channelType, long WalletType, long TrnRefNo, long walletID, long UserID, enTrnType TrnType, enWalletTrnType walletTrnType, ref long trnNo, enWalletDeductionType enWalletDeductionType);

    //    BizResponseClass Callsp_ReleaseHoldWallet(ArbitrageWalletMaster dWalletobj, string timestamp, enServiceType serviceType, decimal amount, string coin, EnAllowedChannels channelType, long WalletType, long TrnRefNo, long walletID, long UserID, enTrnType TrnType, enWalletTrnType walletTrnType, ref long trnNo);

    //    BizResponseClass Callsp_ArbitrageCrDrWalletForHold(CommonClassCrDr firstCurrObj, CommonClassCrDr secondCurrObj, string timestamp, enServiceType serviceType, long firstCurrWalletType, long secondCurrWalletType, long channelType = (long)EnAllowedChannels.Web);

    //    FundTransferResponse Callsp_CreditArbitrageProviderInitialBalance(ProviderFundTransferRequest request);

    //    BizResponseClass Callsp_LPArbitrageCrDrWalletForHold(ArbitrageCommonClassCrDr firstCurrObj, string timestamp, enServiceType serviceType, long firstCurrWalletType, long secondCurrWalletType, long channelType = (long)EnAllowedChannels.Web);

    //    FundTransferResponse Call_sp_ArbitrageWalletFundTransfer(FundTransferRequest Request, long UserId);

    //    BizResponseClass Callsp_HoldWallet_MarketTrade(ArbitrageWalletMaster dWalletobj, string timestamp, enServiceType serviceType, decimal amount, string coin, EnAllowedChannels channelType, long WalletType, long TrnRefNo, long walletID, long UserID, enTrnType TrnType, enWalletTrnType walletTrnType, enWalletDeductionType enWalletDeductionType);

    //    FundTransferResponse Call_sp_ArbitrageToTradingWalletFundTransfer(FundTransferRequest Request, long UserId);

    //    List<WalletType> GetArbitrageChargeWalletType(long? id);

    //    List<ChargesTypeWise> ListArbitrageChargesTypeWise(long WalletTypeId, long? TrntypeId);

    //    BizResponseClass Call_sp_ArbitrageRecon(ReconRequest recon, long UserId);

    //    BizResponseClass Callsp_ReconSuccessAndDebitWalletWithChargeArbitrage(ArbitrageWalletMaster dWalletobj, string timestamp, enServiceType serviceType, decimal amount, string coin, EnAllowedChannels channelType, long WalletType, long TrnRefNo, long walletID, long UserID, enTrnType TrnType, enWalletTrnType walletTrnType, ref long trnNo, enWalletDeductionType enWalletDeductionType);

    //    List<WalletLedgerResponse> CallGetLpArbitrageWalletLedger(DateTime FromDate, DateTime ToDate, long WalletId, int page, int? PageSize, ref int TotalCount);
    //}
}
