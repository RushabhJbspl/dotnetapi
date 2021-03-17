using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Affiliate;
using Worldex.Core.Entities.Charges;
using Worldex.Core.Entities.Wallet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces
{
    public interface IMarketCap
    {
        BizResponseClass CallAPI();

        BizResponseClass ParseResponse(string response);

        BizResponseClass UpdateMarketCapCounter();

        string SendAPIRequest(MarketCapCounterMaster marketCap, string ContentType = "application/json", int Timeout = 180000, string MethodType = "GET");

        List<WalletTypeMaster> GetWalletTypeMaster();

        MarketCapCounterMaster GetMarketCounter();

        BizResponseClass CallSP_InsertUpdateProfit(DateTime TrnDate, string CurrencyName = "USD");
        BizResponseClass CallSP_IEOCallSP();

        AffiliateCommissionCron GetCronData();

        AffiliateCommissionCron InsertIntoCron(int Hour);

        Task<bool> ForceWithdrwLoan();
    }
}
