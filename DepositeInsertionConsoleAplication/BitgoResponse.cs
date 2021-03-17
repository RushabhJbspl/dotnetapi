using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepositConsoleApplication
{
   
    public class History
    {
        public DateTime date { get; set; }
        public string action { get; set; }
    }

    public class Entry
    {
        public string address { get; set; }
        public decimal value { get; set; }
        public string valueString { get; set; }
        public bool isChange { get; set; }
        public bool isPayGo { get; set; }
        public string wallet { get; set; }
    }

    public class Output
    {
        public string id { get; set; }
        public string address { get; set; }
        public object value { get; set; }
        public string valueString { get; set; }
        public string wallet { get; set; }
        public int chain { get; set; }
        public int index { get; set; }
        public string redeemScript { get; set; }
        public bool isSegwit { get; set; }
    }

    public class Input
    {
        public string id { get; set; }
        public string address { get; set; }
        public object value { get; set; }
        public string valueString { get; set; }
        public string redeemScript { get; set; }
        public bool isSegwit { get; set; }
        public string wallet { get; set; }
        public int? chain { get; set; }
        public int? index { get; set; }
    }

    public class Transfer
    {
        public string id { get; set; }
        public string coin { get; set; }
        public string wallet { get; set; }
        public string enterprise { get; set; }
        public string txid { get; set; }
        public int height { get; set; }
        public DateTime date { get; set; }
        public int confirmations { get; set; }
        public string type { get; set; }
        public decimal value { get; set; }
        public string valueString { get; set; }
        public string feeString { get; set; }
        public int payGoFee { get; set; }
        public string payGoFeeString { get; set; }
        public double usd { get; set; }
        public double usdRate { get; set; }
        public string state { get; set; }
        public bool instant { get; set; }
        public List<string> tags { get; set; }
        public List<History> history { get; set; }
        public List<Entry> entries { get; set; }
        public DateTime confirmedTime { get; set; }
        public DateTime unconfirmedTime { get; set; }
        public DateTime createdTime { get; set; }
        public string label { get; set; }
        public List<Output> outputs { get; set; }
        public List<Input> inputs { get; set; }
        public string normalizedTxHash { get; set; }
    }

    public class BitgoResponse
    {
        public string coin { get; set; }
        public List<Transfer> transfers { get; set; }
        public string nextBatchPrevId { get; set; }
    }


}
