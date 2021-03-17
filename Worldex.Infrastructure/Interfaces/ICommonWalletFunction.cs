using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.ViewModels.WalletOperations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Interfaces
{
    public interface ICommonWalletFunction
    {
        decimal GetLedgerLastPostBal(long walletId);

        enErrorCode CheckShadowLimit(long WalletID, decimal Amount, EnWalletUsageType enWalletUsageType = EnWalletUsageType.Trading_Wallet);

        ServiceLimitChargeValue GetServiceLimitChargeValue(enWalletTrnType TrnType, string CoinName);

        //vsolanki 2018-11-24
        Task<enErrorCode> CheckShadowLimitAsync(long WalletID, decimal Amount, EnWalletUsageType enWalletUsageType = EnWalletUsageType.Trading_Wallet);

        //vsolanki 2018-11-24
        Task<enErrorCode> InsertUpdateShadowAsync(long WalletID, decimal Amount, string Remarks, long WalleTypeId, EnWalletUsageType enWalletUsageType = EnWalletUsageType.Trading_Wallet);

        Task<enErrorCode> UpdateShadowAsync(long WalletID, decimal Amount, EnWalletUsageType enWalletUsageType = EnWalletUsageType.Trading_Wallet);

        Task<WalletTrnLimitResponse> CheckWalletLimitAsyncV1(enWalletLimitType TrnType, long WalletId, decimal Amount, long TrnNo = 0);

        WalletTransactionQueue InsertIntoWalletTransactionQueue(Guid Guid, enWalletTranxOrderType TrnType, decimal Amount, long TrnRefNo, DateTime TrnDate, DateTime? UpdatedDate, long WalletID, string WalletType, long MemberID, string TimeStamp, enTransactionStatus Status, string StatusMsg, enWalletTrnType enWalletTrnType, Int64 ErrorCode = 0, decimal holdChargeAmount = 0, decimal chargeAmount = 0);

        Task<BizResponseClass> CheckWithdrawBeneficiary(string address, long userID, string smscode);

        Task SMSSendAsyncV1(EnTemplateType templateType, string UserID, string WalletName = null, string SourcePrice = null, string DestinationPrice = null, string ONOFF = null, string Coin = null, string TrnType = null, string TrnNo = null);

        Task EmailSendAsyncV1(EnTemplateType templateType, string UserID, string Param1 = "", string Param2 = "", string Param3 = "", string Param4 = "", string Param5 = "", string Param6 = "", string Param7 = "", string Param8 = "", string Param9 = "");
    }
}
