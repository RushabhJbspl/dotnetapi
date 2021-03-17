using System;
using System.Collections.Generic;
using System.Text;
using EventBusRabbitMQ.Events;

namespace MarketMaker.Application.IntegrationEvents.Events
{
    public class TickerDataChangeIntegrationEvent : IntegrationEvent
    {
        public decimal LTP { get; set; }
        public string Pair { get; set; }
        public short LPType { get; set; }
        public string LPName { get; set; }
        public decimal Volume { get; set; }
        public decimal Fees { get; set; }
        public decimal ChangePer { get; set; }
        public short UpDownBit { get; set; }
    }
}
