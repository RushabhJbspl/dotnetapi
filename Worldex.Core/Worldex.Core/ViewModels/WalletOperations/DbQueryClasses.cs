using Worldex.Core.Enums;
using System;

namespace Worldex.Core.ViewModels.WalletOperations
{
    public class CheckTrnRefNoRes
    {
        public Int32 TotalCount { get; set; }
    }

    public class CheckTransactionSuccessOrNotRes
    {
        public enTransactionStatus Status { get; set; }
    }  
    public class GetTransactionSettledQty
    {
        public decimal SettledAmt { get; set; }
    }
}
