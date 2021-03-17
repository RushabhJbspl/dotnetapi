using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHStatusCheckApp
{
    public class BitGoResponses
    {
        public string id { get; set; }
        public string address { get; set; }
        public string coin { get; set; }
        public string wallet { get; set; }
        public string txid { get; set; }
        public string coinSpecific { get; set; }
    }

}
