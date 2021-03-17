using MediatR;
using System;
using System.Numerics;

namespace MarketMaker.Application.IntegrationEvents.Events
{
    //TODO implement rabbitMQ library integration event -Sahil 07-10-2019 01:21 PM
    //for testing implemant mediatr inotification -Sahil 07-10-2019 03:06 PM
    public class UserBalanceCheckCompletedIntegrationEvent : INotification
    {
        public long currencyPairId { get; }
        public long debitWalletId { get; } = 0000000000000000;
        public long creditWalletId { get; } = 0000000000000000;
        public int feePer { get; } = 0;
        public decimal fee { get; } = 0;
        public short trnMode { get; } = 11;
        public decimal price { get; }
        public decimal amount { get; }
        public decimal total { get; }
        public decimal stopPrice { get; } = 0;
        public short orderType { get; } = 1;
        public long nonce { get; }
        public short orderSide { get; }

        //use for testing purpose also some member assined default values -Sahil 07-10-2019 03:41 PM
        public UserBalanceCheckCompletedIntegrationEvent(long currencyPairId, decimal price, decimal amount, short orderSide)
        {
            this.currencyPairId = currencyPairId;
            this.price = price;
            this.amount = amount;
            this.orderSide = orderSide;

            total = price * amount;

            nonce = 55445454;
        }



        public UserBalanceCheckCompletedIntegrationEvent(long currencyPairId, long debitWalletId, long creditWalletId, int feePer, decimal fee, short trnMode, decimal price, decimal amount, decimal total, decimal stopPrice, short orderType, long nonce, short orderSide)
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
