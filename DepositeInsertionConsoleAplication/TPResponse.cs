using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepositConsoleApplication
{

    public class Balance
    {
        public double amount { get; set; }
        public List<assetsClass> assets { get; set; }
    }
    public class assetsClass 
        {
        public string assetref { get; set; }
        public string name { get; set; }
        public decimal qty { get; set; }
    }

    public class Result
    {
        public Balance balance { get; set; }
        public List<string> myaddresses { get; set; }
        public List<string> addresses { get; set; }
        public List<object> permissions { get; set; }
        public List<object> items { get; set; }
        public List<object> data { get; set; }
        public int confirmations { get; set; }
        public string blockhash { get; set; }
        public int blockindex { get; set; }
        public int blocktime { get; set; }
        public string txid { get; set; }
        public bool valid { get; set; }
        public int time { get; set; }
        public int timereceived { get; set; }
    }

    public class TPResponse
    {
        public List<Result> result { get; set; }
        public object error { get; set; }
        public string id { get; set; }
    }
}
