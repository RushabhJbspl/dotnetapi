using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Wallet
{
    public class StackingHistoryRes
    {
        public string CoinName { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
    }
    public class ListStackingHistoryRes
    {
        public List<StackingHistoryRes> HistoryRes { get; set; }
        public BizResponseClass BizResponseObj { get; set; }
    }
}
