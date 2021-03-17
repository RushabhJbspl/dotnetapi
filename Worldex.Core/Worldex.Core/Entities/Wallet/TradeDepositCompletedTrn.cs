using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Wallet
{
    public class TradeDepositCompletedTrn
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Key]
        public string TrnID { get; set; }
        [Key]
        public string Address { get; set; }

        public int Status { get; set; }

        public DateTime CreatedTime { get; set; }
    }

}
