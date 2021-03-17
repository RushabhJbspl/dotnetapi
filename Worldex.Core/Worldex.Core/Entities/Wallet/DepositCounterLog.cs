using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Wallet
{
    public class DepositCounterLog : BizBase
    {
        public string NewTxnID { get; set; }
        public string PreviousTrnID { get; set; }
        public string LastTrnID { get; set; }
        public long LastLimit { get; set; }
        public string NextBatchPrvID { get; set; }
        public long DepositCounterMasterId { get; set; }
    }
}
