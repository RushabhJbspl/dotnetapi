using System;
using System.Collections.Generic;
using System.Text;

namespace DepositStatusCheckApp
{
    public class ERC20Response
    {
        public bool isError { get; set; }
        public long confirmations { get; set; }
        public string txnid { get; set; }
        public string msg { get; set; }
    }
}
