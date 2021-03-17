using Worldex.Core.SharedKernel;
using System;

namespace Worldex.Core.Entities.Transaction
{
    public class TransactionMarketType : BizBase
    {
        public string MarketName { get; set; }
        public Boolean AllowForFollowers { get; set; }
    }
}
