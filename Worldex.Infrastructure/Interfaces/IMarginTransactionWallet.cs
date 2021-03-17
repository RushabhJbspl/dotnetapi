using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.Wallet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Interfaces
{
    public interface IMarginTransactionWallet
    {
        Task<WalletDrCrResponse> MarginGetWalletHoldNew(long requestUserID, string coinName, string timestamp, decimal amount, string accWalletID, long TrnRefNo, enServiceType serviceType, enMarginWalletTrnType trnType, enTrnType routeTrnType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, enWalletDeductionType enWalletDeductionType = enWalletDeductionType.Normal);
        Task<WalletDrCrResponse> MarginGetWalletCreditDrForHoldNewAsyncFinal(MarginPNL PNLObj, MarginCommonClassCrDr firstCurrObj, MarginCommonClassCrDr secondCurrObj, string timestamp, enServiceType serviceType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web);
        Task<WalletDrCrResponse> MarginGetReleaseHoldNew(string coinName, string timestamp, decimal amount, string accWalletID, long TrnRefNo, enServiceType serviceType, enMarginWalletTrnType trnType, enTrnType routeTrnType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, string Token = "");
        Task<string> GetDefaultAccWalletID(string SMSCode, long UserID);
        Task<string> GetAccWalletID(long WalletID);
        Task<long> GetWalletID(string AccWalletID);
        Task<bool> ReleaseMarginWalletforSettleLeverageBalance(long BatchNo);
        Task<BizResponseClass> SettleMarketOrderForCharge(long ChargeID);
        Task<TransactionWalletResponse> GetTransactionWalletByCoin(long userid, string coin);
    }
}
