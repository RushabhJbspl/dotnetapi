using System;
using System.Collections.Generic;
using System.Text;
using EventBusRabbitMQ.Interfaces;
using LoggingNlog;
using Microsoft.Azure.ServiceBus;

namespace EventBusRabbitMQ.Services
{
    public class DefaultAzureServiceBusPersistentConnection : IAzureServiceBusPersistentConnection
    {
        private readonly INLogger<DefaultAzureServiceBusPersistentConnection> nLogger;
        private readonly ServiceBusConnectionStringBuilder serviceBusConnectionStringBuilder;
        private ITopicClient topicClient;
        private bool _disposed;

        public DefaultAzureServiceBusPersistentConnection(ServiceBusConnectionStringBuilder serviceBusConnectionStringBuilder, INLogger<DefaultAzureServiceBusPersistentConnection> nLogger)
        {
            this.serviceBusConnectionStringBuilder = serviceBusConnectionStringBuilder ??
                                                     throw new ArgumentNullException(nameof(serviceBusConnectionStringBuilder));
            this.nLogger = nLogger;
            topicClient = new TopicClient(this.serviceBusConnectionStringBuilder, RetryPolicy.Default);

        }

        public ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder => serviceBusConnectionStringBuilder;

        public ITopicClient CreateModel()
        {
            if (topicClient.IsClosedOrClosing)
            {
                topicClient = new TopicClient(this.serviceBusConnectionStringBuilder, RetryPolicy.Default);
            }

            return topicClient;
        }
        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
        }
    }
}
