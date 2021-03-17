using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction.BackOfficeCount
{
    public class OrderSummaryCount
    {
        public long TotalCount { get; set; }
        public long BuyCount { get; set; }
        public long SellCount { get; set; }
    }
    public class OrderSummaryCountResponse : BizResponseClass
    {
        public OrderSummaryCount Response { get; set; }
    }
}
