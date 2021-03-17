using System;

namespace MarketMaker.Application.ViewModels.Services.Redis
{
    public class TickerDataViewModel
    {
        public decimal LTP { get; set; }

        public string Pair { get; set; }

        public short LPType { get; set; }

        public string LPName { get; set; }

        public decimal Volume { get; set; }

        public decimal Fees { get; set; }

        public decimal ChangePer { get; set; }

        public short UpDownBit { get; set; }

        public string UpdateDate { get; set; }
    }
}
