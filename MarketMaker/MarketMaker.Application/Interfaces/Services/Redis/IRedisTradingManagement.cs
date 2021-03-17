using MarketMaker.Application.ViewModels.Services.Redis;
using System.Threading.Tasks;
using System.Transactions;

namespace MarketMaker.Application.Interfaces.Services.Redis
{

    /// <summary>
    /// interface provide methods to fetch/store ticker data into redis cache.
    /// <para>
    /// Code have reference from: 
    /// <seealso cref="CleanArchitecture.Infrastructure.Interfaces.IResdisTradingManagment"/>,
    /// <seealso cref="Transaction.ResdisTradingManagmentService"/>
    /// </para>
    /// <remarks>-Sahil 28-09-2019</remarks>
    /// </summary>
    public interface IRedisTradingManagement
    {
        /// <summary>
        /// Used for get LTP price update data from redis cache.
        /// </summary>
        /// <param name="lpName">Liquidity Provider name</param>
        /// <param name="pairName"> Currency pair</param>
        /// <returns>cached ticker data</returns>
        /// <remarks>-Sahil 28-09-2019</remarks>
        Task<TickerDataViewModel> GetTickerDataAsync(string lpName, string pairName);
    }
}
