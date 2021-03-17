using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Infrastructure.DTOClasses
{
    
    public class History
    {
        public DateTime date { get; set; }
        public string action { get; set; }
    }

    public class Entry
    {
        public string address { get; set; }
        public int value { get; set; }
        public string wallet { get; set; }
    }

    public class Output
    {
        public string id { get; set; }
        public string address { get; set; }
        public int value { get; set; }
        public string valueString { get; set; }
        public string wallet { get; set; }
        public int chain { get; set; }
        public int index { get; set; }
    }

    public class Input
    {
        public string id { get; set; }
        public string address { get; set; }
        public int value { get; set; }
        public string valueString { get; set; }
        public string wallet { get; set; }
        public int chain { get; set; }
        public int index { get; set; }
    }

    public class Transfer
    {
        public string id { get; set; }
        public string coin { get; set; }
        public string wallet { get; set; }
        public string txid { get; set; }
        public string normalizedTxHash { get; set; }
        public int height { get; set; }
        public DateTime date { get; set; }
        public int confirmations { get; set; }
        public string type { get; set; }
        public int value { get; set; }
        public int bitgoFee { get; set; }
        public double usd { get; set; }
        public double usdRate { get; set; }
        public string state { get; set; }
        public int vSize { get; set; }
        public int nSegwitInputs { get; set; }
        public List<string> tags { get; set; }
        public string sequenceId { get; set; }
        public List<History> history { get; set; }
        public List<Entry> entries { get; set; }
        public List<Output> outputs { get; set; }
        public List<Input> inputs { get; set; }
        public DateTime confirmedTime { get; set; }
        public DateTime createdTime { get; set; }
    }

    public class BitGoListResponse
    {
        public string coin { get; set; }
        public List<Transfer> transfers { get; set; }
        public int count { get; set; }
    }
}
