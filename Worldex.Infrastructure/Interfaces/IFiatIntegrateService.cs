using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Wallet;

namespace Worldex.Infrastructure.Interfaces
{
    /// <summary>
    /// vsolanki 2019-10-9 Added New interface service for Fiat COnfiguration
    /// </summary>
    public interface IFiatIntegrateService
    {
        BuyTopUpResponse FiatBuyTopUpRequest(BuyTopUpRequest Req, ApplicationUser user);

        ListGetLTP GetFiatLTP(short? TransactionType);

        BizResponseClass NotifyDeposit(NotifyDepositReq Req);

        ListFiatBuyHistory FiatBuyHistory(string FromCurrency, string ToCurrency, short? Status, string TrnId, string Email,DateTime? FromDate, DateTime? ToDate);

        ListFiatSellHistory FiatSellHistory(string FromCurrency, string ToCurrency, short? Status, string TrnId, string Email, DateTime? FromDate, DateTime? ToDate);

        ListGetFiatTradeInfo GetFiatTradeInfo();

        ListFiatCurrencyInfo GetFiatCurrencyInfo();
        ListFiatCurrencyInfo GetFiatCurrencyInfoBO(short? Status);

        BizResponseClass BuyCallBackUpdate(InputBuyCallBackUpdateReq req,long UserId);

        Task<BizResponseClass> SellCallBackUpdate(InputBuyCallBackUpdateReq request, int id);

        SellResponse FiatSellTopUpRequest(SellRequest Req, ApplicationUser user);

        BizResponseClass UpdateTransactionHash(string Guid, string TransactionHash, long UserId);

        Task FiatSellWithdraw();

        SellResponseV2 FiatSellTopUpRequestV1(SellRequest Req, ApplicationUser user);

        GetWithdrawalTransactionResponse FiatSellRequestConfirmation(FiatSellConfirmReq Req, ApplicationUser user);

        Task FiatBinnanceLTPUpate();
        void UpdateHashFiat();
    }
}
