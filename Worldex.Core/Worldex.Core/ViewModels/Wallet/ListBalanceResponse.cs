using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Wallet
{
    public class ListBalanceResponse 
    {
        public List<BalanceResponse> Response { get; set; }
        public BizResponseClass BizResponseObj { get; set; }
    }
}
