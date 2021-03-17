using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Infrastructure.LiquidityProvider.CEXIO
{
    public class CEXIOGlobalSetting
    {
        
        
            public static string API_Key;
            public static string Secret;
            public static string PassPhrase;

            public static byte[] Secret_Key
            {
                get
                {
                    return Convert.FromBase64String(Secret);
                }
            }
    }
    
 }
