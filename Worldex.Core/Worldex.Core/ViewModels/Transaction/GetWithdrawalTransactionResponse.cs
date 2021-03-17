using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class GetWithdrawalTransactionResponse : BizResponseClass
    {
       public GetWithdrawalTransactionData Response { get; set; }
    }

    public class GetWithdrawalTransactionData
    {
        public long TrnNo { get; set; }
        public string TransactionAddress { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public DateTime TrnDate { get; set; }
        public decimal Status { get; set; }
        public short IsVerified { get; set; }
        public string StatusMsg { get; set; }
        public decimal FinalAmount { get; set; }
        public string CurrencyName { get; set; }
    }
}
