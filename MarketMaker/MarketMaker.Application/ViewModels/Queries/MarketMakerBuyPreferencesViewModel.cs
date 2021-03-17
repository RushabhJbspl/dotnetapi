using FluentValidation;
using MarketMaker.Domain.Enum;

namespace MarketMaker.Application.ViewModels.Queries
{
    public class MarketMakerBuyPreferencesViewModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long PairId { get; set; }
        public string PairName { get; set; }
        public string ProviderName { get; set; }

        public long BuyLTPPrefProID { get; set; }

        public RangeType BuyLTPRangeType { get; set; }

        //change datatype int to double  for Percentage -Sahil 11-10-2019 03:24 PM
        public double BuyUpPercentage { get; set; }
        public double BuyDownPercentage { get; set; }

        public decimal BuyThreshold { get; set; }


    }

    public class MarketMakerBuyPreferencesValidator : AbstractValidator<MarketMakerBuyPreferencesViewModel>
    {
        public MarketMakerBuyPreferencesValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.PairId).NotEmpty();
            RuleFor(x => x.PairName).NotEmpty();
            RuleFor(x => x.ProviderName).NotEmpty();

            RuleFor(x => x.BuyLTPPrefProID).NotEmpty();

            RuleFor(x => x.BuyLTPRangeType).NotEmpty();

            RuleFor(x => x.BuyUpPercentage).NotEmpty();
            RuleFor(x => x.BuyDownPercentage).NotEmpty();

            RuleFor(x => x.BuyThreshold).NotEmpty();

        }
    }
}
