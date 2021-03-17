using System;
using System.Collections.Generic;
using System.Text;
using Worldex.Core.ViewModels.FiatBankIntegration;
using Worldex.Core.ViewModels.Wallet;

namespace Worldex.Core.Interfaces
{
    /// <summary>
    /// vsolanki 2019-10-9 Added New Interface repositoty for Fiat COnfiguration
    /// </summary>
    public interface IFiatIntegrateRepository
    {
        string GetWithdrawTrnId(long TrnNo);

        List<GetLTP> GetFiatLTP(short? TransactionType);

        List<FiatBuyHistory> FiatBuyHistory(string FromCurrency, string ToCurrency, short? Status, string TrnId, string Email, DateTime? FromDate, DateTime? ToDate);

        List<FiatSellHistory> FiatSellHistory(string FromCurrency, string ToCurrency, short? Status, string TrnId, string Email, DateTime? FromDate, DateTime? ToDate);


        GetFiatTradeInfo GetFiatTradeInfo();

        List<GetFiatCurrencyInfo> GetFiatCurrencyInfo();
        List<GetFiatCurrencyInfo> GetFiatCurrencyInfoBO(short? Status);

        List<FiatSellWithdrawTraxn> GetSellWithdrawTrxn();

        List<LPTPairFiat> GetPairForBinnance();

        List<FiatSellWithdrawTraxn> GetSellWithdrawPendingTrxn();
    }
}
