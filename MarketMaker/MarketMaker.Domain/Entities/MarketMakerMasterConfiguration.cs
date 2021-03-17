using MarketMaker.Domain.Entities.Aggregate;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MarketMaker.Domain.Entities
{
    public class MarketMakerMasterConfiguration : Entity
    {
        public long NoOfBuyOrder { get; private set; }
        public long NoOfSellOrder { get; private set; }
        public long MarketMakerPreferenceID { get; private set; } //referenece of MarketMakerPreference
        [Column(TypeName = "decimal(5,2)")]
        public decimal Depth { get; private set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal Width { get; private set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal SpreadGap { get; private set; }
        [Column(TypeName = "decimal(28,18)")]
        public decimal AvgQty { get; set; }
        public int OrderPerCall { get; set; }


        public MarketMakerMasterConfiguration()
        {

        }

        public MarketMakerMasterConfiguration(long noOfBuyOrder, long noOfSellOrder, long marketMakerPreferenceID, int depth, int width, decimal spreadGap)
        {
            NoOfBuyOrder = noOfBuyOrder;
            NoOfSellOrder = noOfSellOrder;
            MarketMakerPreferenceID = marketMakerPreferenceID;
            Depth = depth;
            Width = width;
            SpreadGap = spreadGap;

        }
    }
}
