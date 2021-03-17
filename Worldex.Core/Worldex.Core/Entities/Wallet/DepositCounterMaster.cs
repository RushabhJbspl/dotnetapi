using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Wallet
{
    public class DepositCounterMaster: BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }
        public int RecordCount { get; set; }
        public long Limit { get; set; }
        public string LastTrnID { get; set; }
        public long MaxLimit { get; set; }
        [Key]
        public long WalletTypeID { get; set; }
        [Key]
        public long SerProId { get; set; }
        public string PreviousTrnID { get; set; }
        public string prevIterationID { get; set; }
        public int FlushAddressEnable { get; set; }
        public long TPSPickupStatus { get; set; }
        public int AppType { get; set; } //Uday 04-02-2019  In Deposit App Check Provider
        public double StartTime { get; set; }
        public double EndTime { get; set; }
    }
}
