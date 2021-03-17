using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.ViewModels.WalletConfiguration;
using System;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Interfaces
{
    public interface ILPWalletTransaction
    {
        Task<WalletDrCrResponse> LPGetWalletHoldNew(LPHoldDr LPObj);
        Task<WalletDrCrResponse> GetLPWalletCreditDrForHoldNewAsyncFinal(ArbitrageCommonClassCrDr firstCurrObj, string timestamp, enServiceType serviceType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, enWalletDeductionType enWalletDeductionType = enWalletDeductionType.Normal);
        BizResponseClass ArbitrageRecon(ReconRequest Request, long UserId);
    }

    
}
