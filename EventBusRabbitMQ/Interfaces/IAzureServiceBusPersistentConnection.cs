using System;
using Microsoft.Azure.ServiceBus;

namespace EventBusRabbitMQ.Interfaces
{
    public interface IAzureServiceBusPersistentConnection: IDisposable
    {
        ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder { get; }

        ITopicClient CreateModel();
    }
}
