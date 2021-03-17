using System;
using System.Security.Cryptography;

namespace Worldex.Core.StaticClasses
{
    public static class SecureRandomNumberGenerator
    {
        public static int HOTP(UInt64 C, int digits = 6)
        {
            byte[] K;

            RandomNumberGenerator rng = new RNGCryptoServiceProvider();

            /*    Keys SHOULD be of the length of the HMAC output to facilitate
                  interoperability.*/
            K = new byte[HMACSHA1.Create().HashSize / 8];
            rng.GetBytes(K);

            var hmac = HMACSHA1.Create();
            hmac.Key = K;
            hmac.ComputeHash(BitConverter.GetBytes(C));
            return Truncate(hmac.Hash, digits);
        }
        public static UInt64 CounterNow(int T1 = 30)
        {
            var secondsSinceEpoch = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            return (UInt64)Math.Floor(secondsSinceEpoch / T1);
        }
        private static int DT(byte[] hmac_result)
        {
            int offset = hmac_result[19] & 0xf;
            int bin_code = (hmac_result[offset] & 0x7f) << 24
               | (hmac_result[offset + 1] & 0xff) << 16
               | (hmac_result[offset + 2] & 0xff) << 8
               | (hmac_result[offset + 3] & 0xff);
            return bin_code;
        }

        private static int Truncate(byte[] hmac_result, int digits)
        {
            var Snum = DT(hmac_result);
            return Snum % (int)Math.Pow(10, digits);
        }
    }
}
