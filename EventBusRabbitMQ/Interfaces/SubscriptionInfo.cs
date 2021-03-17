using System;

namespace EventBusRabbitMQ.Interfaces
{
    public class SubscriptionInfo
    {
        public Type HandlerType { get; }
        public string QueueName { get; set; }
        public string BrokerName { get; }
        public string RoutingKey { get; }

        private SubscriptionInfo(Type handlerType, string queueName, string brokerName, string routingKey)
        {
            HandlerType = handlerType;
            QueueName = queueName;
            BrokerName = brokerName;
            RoutingKey = routingKey;
        }

        public static SubscriptionInfo Typed(Type handlerType, string queueName, string brokerName, string routingKey)
        {
            return new SubscriptionInfo(handlerType, queueName, brokerName, routingKey);
        }
    }
}
