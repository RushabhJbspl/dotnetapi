using System;
using System.Collections.Generic;
using System.Text;

namespace MarketMaker.Application.ViewModels.Config
{
    public class MarketMakerOrderBookTradeConfig
    {
        public int maximumTradeCount { get; set; }
        public decimal amountRangePercentage { get; set; }

    }
}
