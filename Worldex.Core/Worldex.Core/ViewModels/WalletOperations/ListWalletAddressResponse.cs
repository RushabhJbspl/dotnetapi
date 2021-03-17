using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.WalletOperations
{
    public class ListWalletAddressResponse : BizResponseClass
    {
        public List<AddressMasterResponse> AddressList { get; set; }
    }
}
