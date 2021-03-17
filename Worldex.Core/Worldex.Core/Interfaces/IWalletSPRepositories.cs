using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.WalletOperations;
using Worldex.Core.ViewModels.ControlPanel;
using System;
using Worldex.Core.ViewModels.APIConfiguration;
using Worldex.Core.ViewModels.Wallet;
using System.Collections.Generic;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.ViewModels.IEOWallet;

namespace Worldex.Core.Interfaces
{
    public interface IWalletSPRepositories
    {
        BizResponseClass Callsp_HoldWallet(WalletMaster dWalletobj, string timestamp, enServiceType serviceType, decimal amount, string coin, EnAllowedChannels channelType, long WalletType, long TrnRefNo, long walletID, long UserID, enTrnType TrnType, enWalletTrnType walletTrnType, ref long TrnNo, enWalletDeductionType enWalletDeductionType, string RefGuid = "", string MarketCurrency = "");

        BizResponseClass Callsp_CrDrWalletForHold(CommonClassCrDr firstCurrObj, CommonClassCrDr secondCurrObj, string timestamp, enServiceType serviceType, long firstCurrWalletType, long secondCurrWalletType, long channelType);

        BizResponseClass Callsp_ReleaseHoldWallet(WalletMaster dWalletobj, string timestamp, enServiceType serviceType, decimal amount, string coin, EnAllowedChannels channelType, long WalletType, long TrnRefNo, long walletID, long UserID, enTrnType TrnType, enWalletTrnType walletTrnType, ref long trnNo);

        BizResponseClass Callsp_HoldWallet_MarketTrade(WalletMaster dWalletobj, string timestamp, enServiceType serviceType, decimal amount, string coin, EnAllowedChannels channelType, long WalletType, long TrnRefNo, long walletID, long UserID, enTrnType TrnType, enWalletTrnType walletTrnType, enWalletDeductionType enWalletDeductionType);

        BizResponseClass Callsp_DebitWallet(WalletMaster dWalletobj, string timestamp, enServiceType serviceType, decimal amount, string coin, EnAllowedChannels channelType, long WalletType, long TrnRefNo, long walletID, long UserID, enTrnType TrnType, enWalletTrnType walletTrnType, ref long trnNo, enWalletDeductionType enWalletDeductionType);

        BizResponseClass Callsp_CreditWallet(WalletMaster cWalletobj, string timestamp, enServiceType serviceType, decimal amount, string coin, EnAllowedChannels channelType, long WalletType, long TrnRefNo, long walletID, long UserID, enTrnType TrnType, enWalletTrnType walletTrnType, ref long trnNo, enWalletDeductionType enWalletDeductionType);

        BizResponseClass Callsp_StakingSchemeRequest(StakingHistoryReq Req, long UserID, long WalletID, long WalletTypeID);

        BizResponseClass Callsp_StakingSchemeRequestv2(StakingHistoryReq Req, long UserID, long WalletID, long WalletTypeID);

        BizResponseClass Callsp_IsValidWalletTransaction(long WalletID, long UserID, long WalletTypeID, long ChannelID, long WalletTrnType);

        BizResponseClass Callsp_UnstakingSchemeRequest(UserUnstakingReq request, long userID, int IsReqFromAdmin);

        BizResponseClass Callsp_UnstakingSchemeRequestv2(UserUnstakingReq request, long userID, int IsReqFromAdmin);

        sp_BalanceStatisticRes Callsp_GetWalletBalanceStatistics(long userID, int Month, int Year);

        BizResponseClass Callsp_DepositionProcess(ref long TrnNo, EnAllowedChannels channelType, string timeStamp, string WalletType, enWalletTrnType walletTrnType, decimal Amount, long TrnRefNo);

        WalletTrnLimitResponse Callsp_CheckWalletTranLimit(short TrnType, long WalletID, decimal Amount, long TrnNo = 0);

        BizResponseClass CallSP_InsertUpdateProfit(DateTime TrnDate, string CurrencyName = "USD");
        BizResponseClass CallSP_IEOCallSP(ref List<IEOCronMailData> IEOCronMailDataList);
      

        BizResponseClass CallSP_ConvertFundWalletOperation(string CrCurr, string DrCurr, decimal CrAmt, decimal DrAmt, long UserID, long TrnNo, int channelType, string timeStamp, int serviceType, int walletTrnType, int IsUseDefaultWallet = 1, string RefGuid = "");
        //rita 16-4-19 added for margin site token conversion
        BizResponseClass CallSP_MarginConvertFundWalletOperation(string CrCurr, string DrCurr, decimal CrAmt, decimal DrAmt, long UserID, long TrnNo, int channelType, string timeStamp, int serviceType, int walletTrnType, long PairID, decimal BidPrice, ref string CreditAccoutID, ref string DebitAccoutID, int IsUseDefaultWallet = 1, string RefGuid = "");

        BizResponseClass CallSP_DepositionRecon(long TrnNo, DepositionReconReq request, long UserId);

        sp_APIPlanDepositProcessResponse Callsp_APIPlanDepositProcess(long UserID, long ServiceID, short ChannelID, decimal Amount, long TrnRefNo, short TrnType);

        // khushali 23-03-2019 For Success and Debit Reocn Process

        BizResponseClass Callsp_ReconSuccessAndDebitWalletWithCharge(WalletMaster dWalletobj, string timestamp, enServiceType serviceType, decimal amount, string coin, EnAllowedChannels channelType, long WalletType, long TrnRefNo, long walletID, long UserID, enTrnType TrnType, enWalletTrnType walletTrnType, ref long trnNo, enWalletDeductionType enWalletDeductionType);

        BizResponseClass Callsp_ReferCommissionSignUp(long CronRefNo, DateTime FromDate, DateTime ToDate);

        GetMemberBalRes Callsp_GetMemberBalance(long walletID, long UserID, long WalletMasterID, short BalanceType, decimal Amount, int WalletUsageType);

        List<WalletLedgerResponse> CallGetWalletLedger(DateTime FromDate, DateTime ToDate, long WalletId, int page, int? PageSize, ref int TotalCount);

        BizResponseClass Callsp_NormalTradingFullSettlementReleaseProfitAmountCron(long CronRefNo);

        BizResponseClass Callsp_MarginTradingFullSettlementReleaseProfitAmountCron(long CronRefNo);

        BizResponseClass Callsp_LPHoldLPFailed_HoldAgain(long CronRefNo);
    }
}
