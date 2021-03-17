using MarketMaker.Domain.Entities.Aggregate;
using System;

namespace MarketMaker.Domain.Entities
{
    public sealed class MarketMakerRangeDetail : Entity
    {
        public long PreferenceId { get; private set; }

        public decimal RangeMin { get; private set; }
        public decimal RangeMax { get; private set; }

        public MarketMakerRangeDetail()
        {

        }
        public MarketMakerRangeDetail(long preferenceId, decimal rangeMin, decimal rangeMax)
        {
            GUID = Guid.NewGuid();
            PreferenceId = preferenceId;
            RangeMin = rangeMin;
            RangeMax = rangeMax;
        }

    }
}
