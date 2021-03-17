using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.WalletOperations
{
    public class WalletLimitConfigurationRes  
    {
        public string AccWalletID { get; set; }

        public int TrnType { get; set; }

        public decimal LimitPerHour { get; set; }

        public decimal LimitPerDay { get; set; }

        public decimal LimitPerTransaction { get; set; }

        public double? StartTime { get; set; }

        public double? EndTime { get; set; }

        public decimal? LifeTime { get; set; }
    }

    public class LimitResponse : BizResponseClass
    {
        public List<WalletLimitConfigurationRes> WalletLimitConfigurationRes { get; set; }
    }
}
