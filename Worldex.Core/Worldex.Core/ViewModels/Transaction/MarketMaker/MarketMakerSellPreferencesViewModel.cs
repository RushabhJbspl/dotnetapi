using Worldex.Core.Enums;

namespace Worldex.Core.ViewModels.Transaction.MarketMaker
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

        //change datatype double to decimal for avoid explicit conversion -Sahil 17-10-2019 05:52 PM
        //change datatype int to double  for Percentage -Sahil 11-10-2019 03:24 PM
        public decimal SellUpPercentage { get; set; }
        public decimal SellDownPercentage { get; set; }

        public decimal SellThreshold { get; set; }

        //commented as defined in separate class -Sahil 17-10-2019 05:32 PM
        //public decimal HoldOrderRateChange { get; set; }
        //public string HoldOrderRateChange { get; set; }//rita 16-10-19 taken dynamic configuration for for multiple market maker hold txn
    }
}
