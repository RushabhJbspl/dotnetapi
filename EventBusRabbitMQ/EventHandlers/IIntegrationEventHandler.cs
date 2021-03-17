using EventBusRabbitMQ.Events;
using System.Threading.Tasks;

namespace EventBusRabbitMQ.EventHandlers
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
