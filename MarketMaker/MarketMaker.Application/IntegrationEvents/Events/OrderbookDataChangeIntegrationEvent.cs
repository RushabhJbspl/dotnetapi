using EventBusRabbitMQ.Events;

namespace MarketMaker.Application.IntegrationEvents.Events
{
    public class OrderbookDataChangeIntegrationEvent : IntegrationEvent
    {
        public string Data { get; set; }
        public string _LpName { get; set; }
        public string _PairName { get; set; }
        public string _OrderType { get; set; }
    }
}
