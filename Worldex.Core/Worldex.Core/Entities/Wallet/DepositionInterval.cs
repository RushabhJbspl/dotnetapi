using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Wallet
{
    public class DepositionInterval : BizBase
    {
        public long DepositHistoryFetchListInterval { get; set; }
        public long DepositStatusCheckInterval { get; set; }
    }
}
