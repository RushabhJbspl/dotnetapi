using LucidOcean.MultiChain;
using System;
using System.Configuration;

namespace Worldex.Infrastructure.Data
{
    public static class MultichainSetting
    {
        public static MultiChainConnection Connection = null;
        public static int PageSize = 10;
       
        static MultichainSetting()
        {  
            Connection = new MultiChainConnection()
            {
                //Hostname = "0.0.0.0",               
                //Port = 0000,
                //Username = "multichainrpc",
                //Password = "",
                //ChainName = "name"
                //BurnAddress = "1XXXXXXX",
                //RootNodeAddress = "1XXXXXX"                
            };
        }


    }
}
