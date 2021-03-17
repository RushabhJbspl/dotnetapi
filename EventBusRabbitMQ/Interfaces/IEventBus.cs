using EventBusRabbitMQ.EventHandlers;
using EventBusRabbitMQ.Events;

namespace EventBusRabbitMQ.Interfaces
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event, string brokerName, string routingKey, string typeOfExchange);

        void Subscribe<T, TH>(string queueName, string brokerName, string routingKey, string TypeOfExchange)
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;
    }
}
