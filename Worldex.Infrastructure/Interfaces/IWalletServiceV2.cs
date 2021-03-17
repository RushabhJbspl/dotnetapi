using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.Wallet;
using System;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Interfaces
{

    public interface IWalletServiceV2
    {
        Task<WalletDrCrResponse> GetWalletDeductionNew(string coinName, string timestamp, enWalletTranxOrderType orderType, decimal amount, long userID, string accWalletID, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enTrnType routeTrnType, string Token = "", string Refguid = "");

        Task<WalletDrCrResponse> GetWalletHoldNew(string coinName, string timestamp, decimal amount, string accWalletID, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enTrnType routeTrnType, EnAllowedChannels allowedChannels, string Token = "", enWalletDeductionType enWalletDeductionType = enWalletDeductionType.Normal, string Refguid = "");

        Task<WalletDrCrResponse> GetWalletCreditDrForHoldNewAsyncFinal(CommonClassCrDr firstCurrObj, CommonClassCrDr secondCurrObj, string timestamp, enServiceType serviceType, EnAllowedChannels enAllowedChannels, enWalletDeductionType enWalletDeductionType = enWalletDeductionType.Normal);

        Task<WalletDrCrResponse> GetReleaseHoldNew(string coinName, string timestamp, decimal amount, string accWalletID, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enTrnType routeTrnType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, string Token = "", string Refguid = "");
    }
}