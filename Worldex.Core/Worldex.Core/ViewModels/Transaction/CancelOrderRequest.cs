using Worldex.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class CancelOrderRequest
    {
        [Required]
        public short CancelAll { get; set; }//0-singal trx,1-All,2-Marketwise komal 28-01-2019
        //[Required]
        public long TranNo { get; set; }

        public enTransactionMarketType OrderType { get; set; }

        public short IsMargin { get; set; } = 0;//Rita 21-2-19,   1-for Margin trading cancel txn
    }
    //komal 07-06-2019 cancel arbitrage Trade
    public class CancelOrderArbitrageRequest
    {
        [Required]
        public short CancelAll { get; set; }//0-singal trx,1-All,2-Marketwise komal 28-01-2019
        //[Required]
        public long TranNo { get; set; }

        public enTransactionMarketType OrderType { get; set; }

        //public short IsMargin { get; set; } = 0;//Rita 21-2-19,   1-for Margin trading cancel txn
    }
}
