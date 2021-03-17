using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction.BackOfficeCount
{
    public class LedgerCountInfo
    {
        public long LedgerCount { get; set; }
    }
    public class LedgerCountResponse : BizResponseClass
    {
        public LedgerCountInfo Response { get; set; }
    }
}
