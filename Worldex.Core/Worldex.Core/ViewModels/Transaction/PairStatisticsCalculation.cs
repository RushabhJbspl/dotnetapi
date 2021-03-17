using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class PairStatisticsCalculation
    {
        public long PairId { get; set; }
        public decimal Volume { get; set; }
        public decimal ChangePer { get; set; }
        public decimal ChangeValue { get; set; }
    }
}
