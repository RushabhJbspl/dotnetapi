using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using Worldex.Core.ViewModels.Wallet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Interfaces
{
    public interface IWalletDeposition
    {
        WalletDrCrResponse DepositionWalletOperation(string timestamp, string address, string coinName, decimal amount, long TrnRefNo, enServiceType serviceType, enWalletTrnType trnType, enWalletTranxOrderType enWalletTranx, enWalletLimitType enWalletLimit, enTrnType routeTrnType, string Token = "", string RefGuid = "");
        Task<BizResponseClass> WithdrawalReconV1(WithdrawalReconRequest request, long UserId, string accessToken);
    }
}
