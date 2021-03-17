using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Infrastructure.LiquidityProvider.Bitfinex
{
    public class BitfinexGlobalSettings
    {
        public static string API_Key = "zbYe8IlS4MVfFK1uABbcUrPz80wAyaMCr2dyzi2Yult";
        public static string Secret = "AjKpagyCYFAewEbfJYmCEftgAyxKp6IpF4ius62e7Em";
        public static string PassPhrase;
        public static string Username;
        public static byte[] Secret_Key
        {
            get
            {
                return Convert.FromBase64String(Secret);
            }
        }
    }
}
