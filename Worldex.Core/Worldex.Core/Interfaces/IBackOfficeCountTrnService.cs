using Worldex.Core.ViewModels.Transaction.BackOfficeCount;

namespace Worldex.Core.Interfaces
{
    public interface IBackOfficeCountTrnService
    {
        ActiveTradeUserTime GetActiveTradeUserCount(short IsMargin = 0);//Rita 6-3-19 for Margin Trading
        ConfigurationCountResponse GetConfigurationCount(short IsMargin=0);//Rita 6-3-19 for Margin Trading
        LedgerCountResponse GetLedgerCount(short IsMargin = 0);//Rita 6-3-19 for Margin Trading
        OrderSummaryCount GetOrderSummaryCount(short IsMargin = 0);//Rita 6-3-19 for Margin Trading
        TradeSummaryCountResponse GetTradeSummaryCount(short IsMargin = 0);//Rita 6-3-19 for Margin Trading
        ProfitSummaryCount GetProfitSummaryCount(short IsMargin = 0);//Rita 6-3-19 for Margin Trading
        TradeSummaryCountResponseInfo GetTradeUserMarketTypeCount(string Type,short IsMargin= 0);//Rita 6-3-19 for Margin Trading
        TransactionReportCountResponse TransactionReportCount();

        ActiveTradeUserTime GetActiveTradeUserCountArbitrage(string Status);
        ConfigurationCountResponse GetConfigurationCountArbitrage();
        TradeSummaryCountResponse GetTradeSummaryCountArbitrage();
        TradeSummaryCountResponseInfo GetTradeUserMarketTypeCountArbitrage(string Type, string Status);
    }
}
