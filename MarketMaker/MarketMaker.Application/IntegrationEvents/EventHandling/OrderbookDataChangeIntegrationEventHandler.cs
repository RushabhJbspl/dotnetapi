using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventBusRabbitMQ.EventHandlers;
using FluentValidation.Results;
using LoggingNlog;
using MarketMaker.Application.IntegrationEvents.Events;
using MarketMaker.Application.Interfaces.Queries;
using MarketMaker.Application.ViewModels.Config;
using MarketMaker.Application.ViewModels.Queries;
using MarketMaker.Application.ViewModels.Response;
using MarketMaker.Domain.Enum;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MarketMaker.Application.IntegrationEvents.EventHandling
{
    public class OrderbookDataChangeIntegrationEventHandler : IIntegrationEventHandler<OrderbookDataChangeIntegrationEvent>
    {
        private readonly IMarketMakerQueries _iMarketMakerQueries;
        private readonly IMediator _iMediator;
        private readonly INLogger<OrderbookDataChangeIntegrationEventHandler> _logger;
        private readonly MarketMakerOrderBookTradeConfig _tradeConfig;
        private readonly IConfiguration _configuration;

        public OrderbookDataChangeIntegrationEventHandler(IMarketMakerQueries iMarketMakerQueries, IMediator iMediator,
            IOptions<MarketMakerOrderBookTradeConfig> iOptions, INLogger<OrderbookDataChangeIntegrationEventHandler> logger,
            IConfiguration configuration)
        {
            _iMarketMakerQueries = iMarketMakerQueries;
            _iMediator = iMediator;
            _logger = logger;
            _tradeConfig = iOptions.Value;
            _configuration = configuration;
        }

        public Task Handle(OrderbookDataChangeIntegrationEvent @event)
        {
            try
            {
                //_logger.WriteInfoLog("Handle", $"{@event._OrderType}Orderbook event occur, pair:{@event.Data} ");
                if (_configuration["StopMarketMakerBot"] == "True")
                    return Task.CompletedTask;

                var orderBookData = JsonConvert.DeserializeObject<List<BuySellBook>>(@event.Data);

                var queryResult = _iMarketMakerQueries.GetMarketMakerTradeCount(@event._PairName, @event._OrderType).Result;

                //_logger.WriteInfoLog("Handle", $"query result pairId:{queryResult.PairID}, trade count:{queryResult.TradeCount}");

                //check for if query return count for given pair -Sahil 14-11-2019 05:58 PM
                ValidationResult validationResult = new MarketMakerTradeCountViewModelValidator().Validate(queryResult);
                if (!validationResult.IsValid) return Task.CompletedTask;

                //get master configuration data
                var MasterConfig = _iMarketMakerQueries.GetMarketMakerMssetrConfiguration(queryResult.PairID).Result;
                //get length data 
                var PairDetailObj = _iMarketMakerQueries.GetPairDetailData(queryResult.PairID).Result;

                long maximumTradeCount = @event._OrderType == "BUY" ? MasterConfig.NoOfBuyOrder : MasterConfig.NoOfSellOrder;
                //int createTradeCount = _tradeConfig.maximumTradeCount - queryResult.TradeCount;
                long createTradeCount = maximumTradeCount - queryResult.TradeCount;
                //_logger.WriteInfoLog("Handle", $"createTradeCount: {createTradeCount}");

                //return if new trade count is 0 or less -Sahil 14-11-2019 05:11 PM
                if (createTradeCount <= 0) return Task.CompletedTask;
                if (createTradeCount < MasterConfig.OrderPerCall)
                    MasterConfig.OrderPerCall = (int)createTradeCount;

                var OrderBookRes = GenerateDifferentAmountListV2(MasterConfig.AvgQty, MasterConfig.Depth, MasterConfig.OrderPerCall, orderBookData, PairDetailObj);

                foreach (var obj in OrderBookRes)
                {
                    if (obj.Amount == 0 || obj.Price == 0)
                        continue;
                    TransactionType type = @event._OrderType == "BUY" ? TransactionType.Buy : TransactionType.Sell;
                    _iMediator.Publish(new UserBalanceCheckCompletedIntegrationEvent(
                        queryResult.PairID,
                        Math.Round(obj.Price, PairDetailObj.PriceLength),
                        Math.Round(obj.Amount, PairDetailObj.QtyLength),
                        (short)type
                    ));
                }
            }
            catch (Exception e)
            {
                _logger.WriteErrorLog("Handle", e);
            }

            return Task.CompletedTask;
        }

        private List<BuySellBook> GenerateDifferentAmountListV2(decimal AvgAmount, decimal percentage, long createTradeCount, List<BuySellBook> OrderBook, PairDetailDataViewModel pairDetail)
        {
            List<BuySellBook> res = new List<BuySellBook>();
            var cnt = 0;
            foreach (var obj in OrderBook)
            {
                if (obj.UpdatedDate < DateTime.UtcNow.AddMinutes(-1))
                    continue;
                cnt++;
                AvgAmount = AvgAmount + (AvgAmount * percentage) / 100;
                res.Add(new BuySellBook() { Amount = Math.Round(AvgAmount, pairDetail.QtyLength), Price = Math.Round(obj.Price, pairDetail.PriceLength) });
                if (cnt >= createTradeCount)
                    break;
            }
            return res;
        }
    }
}
