using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Wallet
{
    public class TotalBalanceRes
    {
        public List<BalanceResponse> Response { get; set; }
        public decimal? TotalBalance { get; set; }
        public BizResponseClass BizResponseObj { get; set; }
    }
}
