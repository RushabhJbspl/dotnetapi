using MarketMaker.Application.ViewModels.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketMaker.Application.Interfaces.Queries
{
    public interface IMarketMakerQueries
    {

        Task<short> GetMarketMakerStatusAsync();
        Task<short> GetMarketMakerUserRoleStatusAsync(long userId);
        Task<MarketMakerBuyPreferencesViewModel> GetMarketMakerUserBuyPreferencesAsync(long pairId);
        Task<MarketMakerSellPreferencesViewModel> GetMarketMakerUserSellPreferencesAsync(long pairId);
        Task<List<MarketMakerUserFixRangeDetail>> GetMarketMakerFixRangeDetailsAsync(long preferenceId);
        Task<decimal> GetMarketMakerHoldOrderRateChange(long pairId);

        /// <summary>
        /// get marketmaker order with status 4 
        /// </summary>
        /// <param name="pairName">currency pair name</param>
        /// <param name="orderType"></param>
        /// <returns>order count</returns>
        /// <remarks>-Sahil 14-11-2019 03:24 PM</remarks>
        Task<MarketMakerTradeCountViewModel> GetMarketMakerTradeCount(string pairName, string orderType);

        /// <summary>
        /// get valid coin list for fiat from database
        /// </summary>
        /// <param name="pairName">currency pair name</param>
        /// <returns>list of coin/currency pair name</returns>
        /// <remarks>-Sahil 15-11-2019 05:16 PM</remarks>
        Task GetFiatCoinPairList(string pairName, decimal ltp);

        /// <summary>
        /// update fiat coin rate into databases
        /// </summary>
        /// <param name="pairName">currency pair name</param>
        /// <param name="ltp">current price</param>
        /// <remarks>-Sahil 15-11-2019 06:57 PM</remarks>
        Task UpdateFiatCoinPrice(string pairName, decimal ltp);

        /// <summary>
        /// insert coin pair for fiat
        /// </summary>
        /// <param name="pairName">currency pair name</param>
        /// <remarks>-Sahil 15-11-2019 06:58 PM</remarks>
        Task InsertFiatCoinPair(string pairName);

        Task<MarketMakerConfigurationViewModel> GetMarketMakerMssetrConfiguration(long PairID);
        Task<PairDetailDataViewModel> GetPairDetailData(long PairID);
    }
}
