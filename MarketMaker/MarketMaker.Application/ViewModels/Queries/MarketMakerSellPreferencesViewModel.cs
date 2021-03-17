using FluentValidation;
using MarketMaker.Domain.Enum;

namespace MarketMaker.Application.ViewModels.Queries
{
    public class MarketMakerSellPreferencesViewModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long PairId { get; set; }
        public string PairName { get; set; }
        public string ProviderName { get; set; }

        public long SellLTPPrefProID { get; set; }

        public RangeType SellLTPRangeType { get; set; }

        //change datatype int to double  for Percentage -Sahil 11-10-2019 03:24 PM
        public double SellUpPercentage { get; set; }
        public double SellDownPercentage { get; set; }

        public decimal SellThreshold { get; set; }

    }

    public class MarketMakerSellPreferencesValidator : AbstractValidator<MarketMakerSellPreferencesViewModel>
    {
        public MarketMakerSellPreferencesValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.PairId).NotEmpty();
            RuleFor(x => x.PairName).NotEmpty();
            RuleFor(x => x.ProviderName).NotEmpty();

            RuleFor(x => x.SellLTPPrefProID).NotEmpty();

            RuleFor(x => x.SellLTPRangeType).NotEmpty();

            RuleFor(x => x.SellUpPercentage).NotEmpty();
            RuleFor(x => x.SellDownPercentage).NotEmpty();

            RuleFor(x => x.SellThreshold).NotEmpty();

        }
    }
}
