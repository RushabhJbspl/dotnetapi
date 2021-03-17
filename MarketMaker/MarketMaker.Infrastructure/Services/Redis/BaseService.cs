using StackExchange.Redis;
using System;
using System.Linq;
using System.Reflection;

namespace MarketMaker.Infrastructure.Services.Redis
{
    public abstract class BaseService<T>
    {
        protected Type Type => typeof(T);
        protected string Name => this.Type.Name;
        protected PropertyInfo[] Properties => this.Type.GetProperties();



        /// <summary>
        /// Generates a key for a Redis Entry  , follows the Redis Name Convention of inserting a colon : to identify values
        /// </summary>
        /// <param name="key">Redis identifier key</param>
        /// <returns>concatenates the key with the name of the type</returns>
        protected string GenerateKey(string key)
        {
            return $"TickerData:{key.ToUpper()}";
        }

        protected T MapFromHash(HashEntry[] hash)
        {
            var obj = (T)Activator.CreateInstance(this.Type); // new instance of T
            var props = this.Properties;

            for (var i = 0; i < props.Count(); i++)
            {
                for (var j = 0; j < hash.Count(); j++)
                {
                    if (props[i].Name == hash[j].Name)
                    {
                        var val = hash[j].Value;
                        var type = props[i].PropertyType;

                        if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                            if (string.IsNullOrEmpty(val))
                            {
                                props[i].SetValue(obj, null);
                            }
                        props[i].SetValue(obj, Convert.ChangeType(val, type));
                    }
                }
            }
            return obj;
        }
    }
}
