using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction.BackOfficeCount
{
    public class ProfitSummaryCount
    {
        public long TotalCount
        {
            get => BuyCount + SellCount + DepositCount + WithdrawlCount;
        }
        public long BuyCount { get; set; }
        public long SellCount { get; set; }
        public long DepositCount { get; set; }
        public long WithdrawlCount { get; set; }
    }

    public class GetProfitSummaryCountResponse : BizResponseClass
    {
        public ProfitSummaryCount Response { get; set; }
    }
}
