using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Worldex.Core.Helpers
{
    public static class Helpers
    {
        public static int PageSize = 20;
        public static string JsonSerialize(object obj)
        {
            return JsonConvert.SerializeObject(obj,
                        new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
        }
        public static decimal DoRoundForTrading(decimal Value,short FractionalLength) 
        {
            //return Math.Round(Value, FractionalLength, MidpointRounding.AwayFromZero);//235.415001286,8 =  235.41500129 
            decimal step = (decimal)Math.Pow(10, FractionalLength); //ex. 8 =>step=100000000
            return Math.Truncate(step * Value) / step;//235.415001286,8 =  235.41500128 
        }
        public static decimal TradingPriceToRedis(decimal Value)//for saving score in Redis
        {
            return Value * 1000000000;
        }
        public static decimal TradingPriceFromRedis(decimal Value)//for getting score in Redis
        {
            return Value / 1000000000;
        }
        public static DateTime UTC_To_IST()
        {
            try
            {
                //DateTime myUTC = DateTime.UtcNow;
                //DateTime istdate = TimeZoneInfo.ConvertTimeFromUtc(myUTC, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                //return istdate;

                //rita 29-8-19 taken dynamic timig
                DateTime myUTC = DateTime.UtcNow;
                DateTime istdate = TimeZoneInfo.ConvertTimeFromUtc(myUTC, TimeZoneInfo.FindSystemTimeZoneById("UTC"));
                return istdate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetTimeStamp()
        {
            return UTC_To_IST().ToString("ddMMyyyyHHmmssfff");//take milliseconds
        }

        //public static string GetUTCTime()
        //{
        //    try
        //    {
        //        TimeZone CurrentZone = TimeZone.CurrentTimeZone;
        //        DateTime CurrentDate = DateTime.Now;
        //        DateTime CurrentUTC = CurrentZone.ToUniversalTime(CurrentDate);
        //        string Data = Convert.ToInt64((CurrentUTC - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds * 1000).ToString();
        //        return Data;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public static string GetUTCTime()
        {
            try
            {
                TimeZone CurrentZone = TimeZone.CurrentTimeZone;
                DateTime CurrentDate = DateTime.Now;
                DateTime CurrentUTC = CurrentZone.ToUniversalTime(CurrentDate);
                string Data = Convert.ToInt64((CurrentUTC - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds * 1000).ToString();
                return Data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static long GenerateBatch()
        {
            string UniqNo = UTC_To_IST().ToString("yyyyMMddHHmmssf");
            return Convert.ToInt64(UniqNo);
        }

        public static List<KeyValuePair<string, int>> GetEnumList<T>()
        {
            var list = new List<KeyValuePair<string, int>>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                list.Add(new KeyValuePair<string, int>(e.ToString(), (int)e));
            }
            return list;
        }

        public static List<int> GetEnumValue<T>()
        {
            var list = new List<int>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                list.Add((int)e);
            }
            return list;
        }
        public static bool IsValidDecimal(string Value,ref decimal OutValue)//rita 25-7-19 added for finding number from string
        {
            var Result = decimal.TryParse(Value, out OutValue);
            return Result;
        }

    }

    public class CopyClass
    {
        public static void CopyObject<T>(object sourceObject, ref T destObject)
        {
            //  If either the source, or destination is null, return
            if (sourceObject == null || destObject == null)
                return;

            //  Get the type of each object
            Type sourceType = sourceObject.GetType();
            Type targetType = destObject.GetType();

            //  Loop through the source properties
            foreach (PropertyInfo p in sourceType.GetProperties())
            {
                //  Get the matching property in the destination object
                PropertyInfo targetObj = targetType.GetProperty(p.Name);
                //  If there is none, skip
                if (targetObj == null)
                    continue;

                //  Set the value in the destination
                targetObj.SetValue(destObject, p.GetValue(sourceObject, null), null);
            }
        }
        //https://www.codeproject.com/Articles/28952/%2FArticles%2F28952%2FShallow-Copy-vs-Deep-Copy-in-NET
        public static T DeepCopy<T>(T item)//rita 9-1-19 for same class object copy to other object , withour refernce assign ,evern internal struct type object's reference not assign
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, item);
            stream.Seek(0, SeekOrigin.Begin);
            T result = (T)formatter.Deserialize(stream);
            stream.Close();
            return result;
        }
    }
}
