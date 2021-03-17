using Worldex.Core.Entities.Wallet;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Events
{
    class BalanceCheckEvent : BaseDomainEvent
    {
        public WalletMaster WalletObj { get; set; }
    }
}
