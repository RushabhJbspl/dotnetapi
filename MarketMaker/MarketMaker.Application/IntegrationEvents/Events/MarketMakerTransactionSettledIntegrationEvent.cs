using EventBusRabbitMQ.Events;

namespace MarketMaker.Application.IntegrationEvents.Events
{
    public class MarketMakerTransactionSettledIntegrationEvent : IntegrationEvent
    {
        public long currencyPairId { get; set;}
        public long debitWalletId { get; set;} = 0000000000000000;
        public long creditWalletId { get; set;} = 0000000000000000;
        public int feePer { get; set;} = 0;
        public decimal fee { get; set;} = 0;
        public short trnMode { get; set;} = 11;
        public decimal price { get; set;}
        public decimal amount { get; set;}
        public decimal total { get; set;}
        public decimal stopPrice { get; set;} = 0;
        public short orderType { get; set;} = 1;
        public long nonce { get; set;}
        public short orderSide { get; set;}

        //use for testing purpose also some member assined default values -Sahil 07-10-2019 03:41 PM
        public MarketMakerTransactionSettledIntegrationEvent(long currencyPairId, decimal price, decimal amount, short orderSide)
        {
            this.currencyPairId = currencyPairId;
            this.price = price;
            this.amount = amount;
            this.orderSide = orderSide;

            total = price * amount;

            nonce = 55445454;
        }



        public MarketMakerTransactionSettledIntegrationEvent(long currencyPairId, long debitWalletId, long creditWalletId, int feePer, decimal fee, short trnMode, decimal price, decimal amount, decimal total, decimal stopPrice, short orderType, long nonce, short orderSide)
        {
            this.currencyPairId = currencyPairId;
            this.debitWalletId = debitWalletId;
            this.creditWalletId = creditWalletId;
            this.feePer = feePer;
            this.fee = fee;
            this.trnMode = trnMode;
            this.price = price;
            this.amount = amount;
            this.total = total;
            this.stopPrice = stopPrice;
            this.orderType = orderType;
            this.nonce = nonce;
            this.orderSide = orderSide;
        }
    }
}

