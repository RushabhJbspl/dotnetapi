using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace MarketMaker.Application.ViewModels.Queries
{
    public class MarketMakerTradeCountViewModel
    {
        public long PairID { get; set; }
        public int TradeCount { get; set; }
    }

    public class MarketMakerTradeCountViewModelValidator : AbstractValidator<MarketMakerTradeCountViewModel>
    {
        public MarketMakerTradeCountViewModelValidator()
        {
            RuleFor(x => x.PairID).NotEqual(0);
        }
    }
    public class PairDetailDataViewModel
    {
        public int AmtLength { get; set; }
        public int PriceLength { get; set; }
        public int QtyLength { get; set; }
    }

    public class MarketMakerConfigurationViewModel
    {
        public long NoOfBuyOrder { get; set; }
        public long NoOfSellOrder { get; set; }
        public decimal Depth { get; set; }
        public decimal AvgQty { get; set; }
        public int OrderPerCall { get; set; }
    }
}
