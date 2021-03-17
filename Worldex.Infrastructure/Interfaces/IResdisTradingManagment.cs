using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Services.Transaction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Interfaces
{
    public interface IResdisTradingManagment
    {
        Task TransactionOrderCacheEntry(BizResponse _Resp, long TrnNo, long PairID, string PairName, decimal Price, decimal Qty, decimal RemainQty, short OrderType, string OrderSide, short IsAPITrade = 0);
        Task<BizResponse> MakeNewTransactionEntry(BizResponse _Resp);
        Task<BizResponse> StoreDataToRedis(BizResponse _Resp, List<string> LPList, List<string> PairName, RedisTickerData data);
        Task<BizResponse> StoreDataToRedisv2(BizResponse _Resp, RedisTickerData data);
        Task<BizResponse> StoreOrderBookToRedis(BizResponse _Resp, List<GetBuySellBook> OrderList, string LPName, string PairName, string OrderType);
        Task<List<GetBuySellBook>> GetTopOrderBookFromRedis(string LPName, string PairName, string OrderType,int MaxListCount);
        Task<BizResponse> GetOrderBookFromRedis(BizResponse _Resp, string PairName, string OrderType,string LPName);
        Task<TickerData> GetTickerDataAsync(string lpName, string pairName);
    }
}
