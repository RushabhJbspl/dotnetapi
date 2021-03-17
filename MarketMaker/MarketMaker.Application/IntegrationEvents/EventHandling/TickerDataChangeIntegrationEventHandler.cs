using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EventBusRabbitMQ.EventHandlers;
using LoggingNlog;
using MarketMaker.Application.IntegrationEvents.Events;
using MarketMaker.Application.Interfaces.Queries;

namespace MarketMaker.Application.IntegrationEvents.EventHandling
{
    public class TickerDataChangeIntegrationEventHandler : IIntegrationEventHandler<TickerDataChangeIntegrationEvent>
    {
        private readonly IMarketMakerQueries _iMakerQueries;
        private readonly INLogger<TickerDataChangeIntegrationEventHandler> _logger;

        public TickerDataChangeIntegrationEventHandler(IMarketMakerQueries iMakerQueries, INLogger<TickerDataChangeIntegrationEventHandler> logger)
        {
            _iMakerQueries = iMakerQueries;
            _logger = logger;
        }
        public Task Handle(TickerDataChangeIntegrationEvent @event)
        {
            //condition check ticker event is for LP = binance -Sahil 15-11-2019 05:28 PM
            if (!@event.LPName.Equals("BINANCE")) return Task.CompletedTask;

            //condition check for pairName is exist in database -Sahil 15-11-2019 05:29 PM
            var queryResult = _iMakerQueries.GetFiatCoinPairList(@event.Pair, @event.LTP);
            //if (queryResult != null)
            //{
            //    if (queryResult != null)
            //    {
            //        _iMakerQueries.InsertFiatCoinPair(@event.Pair);
            //    }
            //    else
            //    {
            //        _iMakerQueries.UpdateFiatCoinPrice(@event.Pair, @event.LTP);
            //    }
            //}
            return Task.CompletedTask;
        }
    }
}
