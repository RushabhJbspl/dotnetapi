using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.IEOWallet;
using System;

namespace Worldex.Core.Interfaces
{
    public interface IIEOWalletSPRepositories
    {
        PreConfirmResponseV2 CallSP_PreConfirm(string PaidCurrencyWallet, decimal PaidQauntity, string PaidCurrency, string DeliveryCurrency, string RoundID, string Remarks, Int64 UserID);

        PreConfirmResponse CallSP_ConfirmTrn(string PaidCurrencyWallet, decimal PaidQauntity, string PaidCurrency, string DeliveryCurrency, string RoundID, string Remarks, Int64 UserID);

        BizResponseClass Callsp_IEOAdminWalletCreditBalance(IEOAdminWalletCreditReq request);
    }
}
