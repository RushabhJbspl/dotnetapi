using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Threading.Tasks;
using Binance.Net;

using Binance.Net.Objects;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Core.ViewModels.Transaction;
using CryptoExchange.Net.Objects;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using StructureMap;
using Huobi.Net.Objects;
using Huobi.Net;
using Newtonsoft.Json.Linq;
using System.Linq;
using Worldex.Core.Interfaces;
using System.Net.Http;

namespace Worldex.Infrastructure.LiquidityProvider
{
    public class HuobiLPService : IHuobiLPService
    {
        public IWebApiSendRequest _WebAPISendRequest { get; set; }
        public HuobiClient _client;
        public HuobiLPService(HuobiClient Client)
        {
            _client = Client;
        }

        public Task<WebCallResult<long>> CancelOrderAsync(long orderId)
        {
            try
            {
                WebCallResult<long> result = _client.CancelOrder(orderId);


                if (result != null)
                {
                    return Task.FromResult(result);

                }
                else
                {
                    return null;
                }

            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("cancalOrder", "HuboiAPI", e);
                return null;
            }
        }

        public Task<WebCallResult<List<HuobiBalance>>> GetBalancesAsync(long accountId)
        {
            try
            {
                var Result = _client.GetBalances(accountId);
                if (Result != null)
                {
                    try
                    {
                        var list = JsonConvert.DeserializeObject<WebCallResult<List<HuobiBalance>>>(Result.Data.ToString());
                        return Task.FromResult(list);


                    }
                    catch (Exception e)
                    {

                        return null;
                    }

                }
                else
                {
                    return null;
                }

            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("Getbalance", "HuboiAPI", e);
                return null;
            }
        }


        public Task<WebCallResult<List<HuobiSymbol>>> GetExchangeInfoAsync()
        {
            try
            {
                var Result = _client.GetSymbols();
                return Task.FromResult(Result);
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("GetExchangeInfo", "HuboiAPI", e);
                return null;
            }
        }

        public Task GetMarketSummaryAsync(string market)
        {
            try
            {
                var Result = _client.GetMarketDetails24H(market);
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("GetMarketSummary", "HuboiAPI", e);
                return null;
            }
        }

        public Task GetOpenOrdersAsync(string market = null, int? receiveWindow = null)
        {
            try
            {
                var Result = _client.GetOpenOrders();
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("GetopenOrder", "HuboiAPI", e);
                return null;
            }
        }

        public Task<WebCallResult<HuobiMarketDepth>> GetOrderBookAsync(string market, int limit)
        {
            try
            {

                WebCallResult<HuobiMarketDepth> Result = _client.GetMarketDepth(market.ToLower(), 0, limit);

                if (Result.Data != null)
                {
                    try
                    {
                        return Task.FromResult(Result);

                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }


            }

            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("GetOrderbook", "HuboiAPI", e);
                return null;
            }

        }


        public Task<WebCallResult<List<HuobiOrderTrade>>> GetOrderHistoryAsync(string symbol, IEnumerable<HuobiOrderType> types = null, DateTime? startTime = null, DateTime? endTime = null, long? fromId = null, HuobiFilterDirection? direction = null, int? limit = null)
        {
            try
            {
                var Result = _client.GetSymbolTrades(symbol);
                return Task.FromResult(Result);
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("GetOrderHistory", "HuboiAPI", e);
                return null;
            }
        }

        public Task<WebCallResult<HuobiOrder>> GetOrderInfoAsync(long orderId)
        {
            try
            {
                var Result = _client.GetOrderInfo(orderId);

                if (Result != null)
                {
                    try
                    {
                        return Task.FromResult(JsonConvert.DeserializeObject<WebCallResult<HuobiOrder>>(Result.Data.ToString()));

                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("GetOrderinfo", "HuboiAPI", e);
                return null;
            }
        }

        public async Task<WebCallResult<List<HuobiMarketTrade>>> GetTradeHistoryAsync(string market, int limit)
        {
            try
            {
                WebCallResult<List<HuobiMarketTrade>> Res = _client.GetMarketTradeHistory(market.ToLower(), limit);
                if (Res.Data != null)
                {


                    return await Task.FromResult(Res);
                }
                else
                {
                    return null;
                }


            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("GetTradeHistory", "HuboiAPI", e);
                return null;
            }
        }

        public Task<WebCallResult<long>> PlaceOrder(long accountId, string symbol, HuobiOrderType orderType, decimal amount, decimal? price = null)
        {
            try
            {
                WebCallResult<long> result = _client.PlaceOrder(accountId, symbol, orderType, amount, price);
                if (result != null)
                {
                    return Task.FromResult(result);

                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                HelperForLog.WriteErrorLog("placeorder", "HuboiAPI", e);
                return null;
            }


        }

    }
}