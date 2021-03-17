using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Worldex.Infrastructure.DTOClasses
{
    public class ThirdPartyAPIRequest
    {
        
        public string RequestURL { get; set; }
        public string RequestBody { get; set; }
        public WebHeaderCollection keyValuePairsHeader { get; set; }
        public byte DelayAddress { get; set; }
        public string walletID { get; set; }

    }

    public class ThirdPartyAPIRequestArbitrage : ThirdPartyAPIRequest
    {
        public string ContentType { get; set; }
        public string MethodType { get; set; }
    }
}
