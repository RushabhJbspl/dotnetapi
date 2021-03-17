using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.WalletOperations
{
    public class TokenStackingHistoryRes
    {
        public List<TokenStackRes> StackHistoryRes { get; set; }
        public BizResponseClass BizResponseObj { get; set; }
    }
    public class TokenStackRes
    {
        public long AutoNo { get; set; }

        public string Type { get; set; }

        public decimal Coin { get; set; }

        public decimal MakerCharges { get; set; }

        public decimal TakerCharges { get; set; }

        public DateTime Date { get; set; }

    }
}
