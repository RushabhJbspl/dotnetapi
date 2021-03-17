using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Infrastructure.LiquidityProvider.UpbitAPI
{
    public class UpBitGlobalSettings
    {
        public static string API_Key = "XYZ";
        public static string Secret = "XYZ";
        public static string PassPhrase= "XYZ" ;
        public static byte[] Secret_Key
        {
            get
            {
                return Convert.FromBase64String(Secret);
            }
        }
    }
}
