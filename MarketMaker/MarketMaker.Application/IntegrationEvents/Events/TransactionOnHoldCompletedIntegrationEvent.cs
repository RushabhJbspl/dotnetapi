using EventBusRabbitMQ.Events;
using MarketMaker.Domain.Enum;

namespace MarketMaker.Application.IntegrationEvents.Events
{
    public sealed class TransactionOnHoldCompletedIntegrationEvent : IntegrationEvent
    {

        public long UserId { get; }
        public long TransactionId { get; }
        public TransactionType TransactionType { get; }
        public decimal Price { get; }
        public decimal Quantity { get; }
        public long Pair { get; }

        public TransactionOnHoldCompletedIntegrationEvent(long userId, long transactionId, TransactionType transactionType, decimal price, decimal quantity, long pair)
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
