using System;
using System.Collections.Generic;
using System.Text;

namespace WithdrawConsoleApplication
{
    public class EtherScanConfirmResponse
    {
        public bool isError { get; set; }
        public int confirmations { get; set; }
        public string txnid { get; set; }
        public string msg { get; set; }
    }
}
