using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.Wallet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Interfaces
{
    public interface IMarginTransactionWalletGUID
    {
        Task<WalletDrCrResponse> MarginGetWalletHoldNew(long requestUserID, string coinName, string timestamp, decimal amount, string accWalletID, long TrnRefNo, enServiceType serviceType, enMarginWalletTrnType trnType, enTrnType routeTrnType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, enWalletDeductionType enWalletDeductionType = enWalletDeductionType.Normal, string RefGuid = "");
        Task<WalletDrCrResponse> MarginGetWalletCreditDrForHoldNewAsyncFinal(MarginPNL PNLObj, MarginCommonClassCrDr firstCurrObj, MarginCommonClassCrDr secondCurrObj, string timestamp, enServiceType serviceType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, string RefGuid = "");
        Task<WalletDrCrResponse> MarginGetReleaseHoldNew(string coinName, string timestamp, decimal amount, string accWalletID, long TrnRefNo, enServiceType serviceType, enMarginWalletTrnType trnType, enTrnType routeTrnType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, string Token = "", string refGuid = "");
        Task<string> GetDefaultAccWalletID(string SMSCode, long UserID);
        Task<string> GetAccWalletID(long WalletID);
        Task<long> GetWalletID(string AccWalletID);
        Task<bool> ReleaseMarginWalletforSettleLeverageBalance(long BatchNo);
        Task<BizResponseClass> SettleMarketOrderForCharge(long ChargeID);
    }
}
