using MarketMaker.Domain.Entities.Aggregate;
using MarketMaker.Domain.Enum;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketMaker.Domain.Entities
{
    public sealed class MarketMakerPreference : Entity
    {


        public long UserId { get; private set; }
        public long PairId { get; private set; }

        public long BuyLTPPrefProID { get; private set; }
        public long SellLTPPrefProID { get; private set; }

        public RangeType BuyLTPRangeType { get; private set; }
        public RangeType SellLTPRangeType { get; private set; }

        //change percentage datatype -Sahil 11-10-2019 03:20
        [Column(TypeName = "decimal(28,18)")]
        public double BuyUpPercentage { get; private set; }

        [Column(TypeName = "decimal(28,18)")]
        public double BuyDownPercentage { get; private set; }

        [Column(TypeName = "decimal(28,18)")]
        public double SellUpPercentage { get; private set; }

        [Column(TypeName = "decimal(28,18)")]
        public double SellDownPercentage { get; private set; }

        // Currently threshold value has been taken as int. datatype changes could be occur in future -Sahil 25-09-2019
        [Column(TypeName = "decimal(28,18)")]
        public decimal BuyThreshold { get; private set; }

        [Column(TypeName = "decimal(28,18)")]
        public decimal SellThreshold { get; private set; }

        //add for market maker settle order price difference calculation -Sahil 14-10-2019 12:31 PM
        //change data type form decimal to string to store price variation -Sahil 05:17 16-10-2019
        [Column(TypeName = "varchar(200)")]
        public string HoldOrderRateChange { get; private set; }



        public MarketMakerPreference()
        {

        }

        public MarketMakerPreference(long userId, int pairId, long buyLtpPrefProId, long sellLtpPrefProId, RangeType buyLtpRangeType, RangeType sellLtpRangeType, int buyUpPercentage, int buyDownPercentage, int sellUpPercentage, int sellDownPercentage, decimal buyThreshold, decimal sellThreshold, string holdOrderRateChange)
        {
            GUID = Guid.NewGuid();
            UserId = userId;
            PairId = pairId;
            BuyLTPPrefProID = buyLtpPrefProId;
            SellLTPPrefProID = sellLtpPrefProId;
            BuyLTPRangeType = buyLtpRangeType;
            SellLTPRangeType = sellLtpRangeType;
            BuyUpPercentage = buyUpPercentage;
            BuyDownPercentage = buyDownPercentage;
            SellUpPercentage = sellUpPercentage;
            SellDownPercentage = sellDownPercentage;
            BuyThreshold = buyThreshold;
            SellThreshold = sellThreshold;
            HoldOrderRateChange = holdOrderRateChange;
        }
    }
}
