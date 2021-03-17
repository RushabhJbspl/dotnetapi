using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Infrastructure.LiquidityProvider.KrakenAPI
{
    public class KrakenGlobalSettings
    {
        public static string API_Key = "51Xu6NBk3TqwfyMOE19OQNzNobha9Gq+zPHAGGj5qbjBCG5lqR+6uypZ";
        public static string Secret = "el0tV5N9VzBslljAry9PsnJFyBPyItIPe2//Jp9Mmy/cvbuK9f+JaKx7LZdwaXVaO8hMJ4yLBgnPrUWx6dPv+A==";
        public static string PassPhrase = "Jbspl@123$";
        public static string Username = "jbspl_test";
        public static byte[] Secret_Key
        {
            get
            {
                return Convert.FromBase64String(Secret);
            }
        }
    }
}
