using System;
using System.Collections.Generic;
using System.Text;
using EventBusRabbitMQ.Events;

namespace Worldex.Infrastructure.IntegrationEvents
{
    /// <summary>
    /// This event is publish with RabbitMQ when MarketMaker is on
    /// </summary>
    /// <remarks>-Sahil 16-10-2019 05:44 PM</remarks>
    public class TransactionOnHoldCompletedIntegrationEvent : IntegrationEvent
    {
        public long UserId { get; }
        public long TransactionId { get; }
        public short TransactionType { get; }
        public decimal Price { get; }
        public decimal Quantity { get; }
        public long Pair { get; }

        public TransactionOnHoldCompletedIntegrationEvent(long userId, long transactionId, short transactionType, decimal price, decimal quantity, long pair)
        {
            UserId = userId;
            TransactionId = transactionId;
            TransactionType = transactionType;
            Price = price;
            Quantity = quantity;
            Pair = pair;
        }

    }
}
