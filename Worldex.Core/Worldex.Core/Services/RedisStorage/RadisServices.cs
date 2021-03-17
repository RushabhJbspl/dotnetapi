using CachingFramework.Redis;
using CachingFramework.Redis.Contracts;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Worldex.Core.ViewModels.Chat;

namespace Worldex.Core.Services.RadisDatabase
{   
   public class RadisServices<T>  :BaseService<T>, IRedisService<T>
    {
        internal readonly IDatabase Db;
        protected readonly RedisConnectionFactory ConnectionFactory;
        internal readonly RedisContext Context;
        //internal readonly Mapper _mapper;

        public RadisServices(RedisConnectionFactory connectionFactory)
        {
            this.ConnectionFactory = connectionFactory;
            this.Db = this.ConnectionFactory.Connection().GetDatabase();
            this.Context = new RedisContext(this.ConnectionFactory.Connection());
        }       

        public T Get(string key)
        {
            try
            {
                //key = this.GenerateKey(key); //for currency rate update commented by rita , ntrivedi 04-11-2019 
                var hash = this.Db.HashGetAll(key);
                return this.MapFromHash(hash);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public T GetV2(string key)
        {
            try
            {
                //key = this.GenerateKey(key); //for currency rate update commented by rita , ntrivedi 04-11-2019 
                var hash = this.Db.HashGetAll(key);
                return this.MapFromHashV2(hash);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void Save(string key, T obj)
        {
            try
            {
                if (obj != null)
                {
                    var hash = this.GenerateHash(obj);
                    key = this.GenerateKey(key);

                    //if (this.Db.HashLength(key) == 0)
                    //{
                        this.Db.HashSet(key, hash);
                    //}
                    //else
                    //{
                    //    var props = this.Properties;
                    //    foreach (var item in props)
                    //    {
                    //        if (this.Db.HashExists(key, item.Name))
                    //        {
                    //            this.Db.HashIncrement(key, item.Name, Convert.ToInt64(item.GetValue(obj)));
                    //        }
                    //    }
                    //}

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public void SaveWithOrigionalKey(string key, T obj,string Tag)
        {
            try
            {
                if (obj != null)
                {
                    var hash = this.GenerateHash(obj);
                    this.Db.HashSet(key, hash);
                    //this.Context.Cache.AddTagsToHashField(key, "Token", new[] { Tag }); // khushali 18-03-2019 comment remove tag and add ttl to 1 hr
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void Delete(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key) || key.Contains(":")) throw new ArgumentException("invalid key");

                key = this.GenerateKey(key);
                this.Db.KeyDelete(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        // --khushali-- For signalr scaleout with Redis

        public string GetSetData(string key)
        {
            try
            {
                //RedisContext context = new RedisContext();
                var Messages = this.Db.SetMembers(key);
                string Data = "[" + string.Join(",", Messages) + "]"; // make json format
                return Data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public T GetData(string key)
        {
            try
            {
                return this.Context.Cache.GetObject<T>(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetHashData(string Key,string Field)
        {
            try
            {
                //string Data = this.Context.Cache.GetHashed<string,string>(Key, Field);
                string Data = this.Db.HashGet(Key,Field).ToString();
                return Data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteTag(string Key, string Tag)
        {
            try
            {
                this.Context.Cache.RemoveTagsFromKeyAsync(Key, new[] { Tag });
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public void DeleteHash(string key)
        {
            try
            {
                // if (string.IsNullOrWhiteSpace(key) || key.Contains(":")) throw new ArgumentException("invalid key");
                this.Db.KeyDelete(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public void Scan(string value,string SpecialText)
        {
            try
            {
                IEnumerable<TagMember> members = this.Context.Cache.GetMembersByTag(value);
                foreach (TagMember member in members)
                {
                    if (string.IsNullOrWhiteSpace(member.Key) || member.Key.Contains(SpecialText))
                    {
                        var key = member.Key;
                        var type = member.MemberType;
                        var user = member.GetMemberAs<RedisUserdata>();
                        DeleteHash(key);
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IReadOnlyList<T> GetSetList(string Tag)
        {
            try
            {
                //IEnumerable<TagMember> members = this.Context.Cache.GetMembersByTag(Tag);
                //foreach (TagMember member in members)
                //{
                //    if (string.IsNullOrWhiteSpace(member.Key) || member.Key.Contains(":"))
                //    {
                //        var key = member.Key;
                //        var type = member.MemberType;
                //        var user = member.GetMemberAs<string>();
                //        return user;
                //    }
                //}
                IReadOnlyList<T> Members = this.Context.Cache.GetObjectsByTag<T>(new[] { Tag }).ToList().AsReadOnly();
                return Members;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetPairOrMarketData(string Value,string keySplitString,string Identifier)
        {
            try
            {
                IEnumerable<TagMember> members = this.Context.Cache.GetMembersByTag(Value);
                foreach (TagMember member in members)
                {
                    if(member.Key.Contains(Identifier))
                    {
                        var Key = member.Key;
                        if (string.IsNullOrWhiteSpace(Key) || Key.Contains(keySplitString))
                        {
                            Key = Key.Split(keySplitString)[1];
                        }
                        return Key;
                    }                   
                }
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        

        public IEnumerable<T> GetConnectionIDForTest(string Token)
        {
            try
            {
                return this.Context.Cache.GetObjectsByTag<T>(Token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<string> GetKey(string Tag)
        {
            try
            {
                return this.Context.Cache.GetKeysByTag(new[] { Tag });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveToSet(string key, T obj, string Tag)
        {
            try
            {
                if (obj != null)
                {
                    var hash = this.GenerateHash(obj);
                    //RedisContext context = new RedisContext();
                    this.Context.Cache.AddToSet(key, obj, new[] { Tag});
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void SaveTagsToSetMember(string Key, string Value, string Tag)
        {
            try
            {
                if (Value != null)
                {                    
                    //this.Context.Cache.AddToSet(Key, Value, new[] { Tag , Value }); 
                    this.Context.Cache.AddToSet(Key, Value, ttl: TimeSpan.FromHours(1)); // khushali 18-03-2019 for remove tag mechanism and ttl to 1 hr
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RemoveSetMember(string Key, string Value)
        {
            try
            {
                if (Value != null)
                {
                    //var hash = this.GenerateHash(obj);
                    //RedisContext context = new RedisContext();
                    //this.Context.Cache.AddToSet(key, obj, new[] { Tag });
                    this.Context.Cache.RemoveFromSet(Key, Value);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveToHash(string key, T obj, string Tag1, string Tag2)
        {
            try
            {
                if (obj != null)
                {
                    var hash = this.GenerateHash(obj);
                    //RedisContext context = new RedisContext();
                    this.Context.Cache.SetObject(key, obj, new[] { Tag1,Tag2 });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveToHash(string key, T obj, string Tag1)
        {
            try
            {
                if (obj != null)
                {
                    var hash = this.GenerateHash(obj);
                    //RedisContext context = new RedisContext();
                    this.Context.Cache.SetObject(key, obj, new[] { Tag1 }, TimeSpan.FromHours(1));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveToHash(string key, T obj)
        {
            try
            {
                if (obj != null)
                {
                    var hash = this.GenerateHash(obj);
                    //RedisContext context = new RedisContext();
                    this.Context.Cache.SetObject(key, obj);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetSortedSetList(string key,int MaxListCount,string OrderType)
        {
            try
            {
                string Data = string.Empty;
                Order sortOrder = new Order();
                if (OrderType.Equals("BUY"))
                {
                    sortOrder = Order.Descending;
                }
                if (OrderType.Equals("SELL"))
                {
                    sortOrder = Order.Ascending;
                }

                var orderList = this.Db.SortedSetRangeByRankWithScores(key, order: sortOrder).ToList().Take(MaxListCount);
                if (orderList.Count() != 0)
                {
                    Data += "[";
                    foreach (var order in orderList)
                    {
                        var Amount = order.Element;
                        var Price = Helpers.Helpers.TradingPriceFromRedis((decimal)order.Score);
                        Data += "{" + "Amount:" + Amount + "," + "Price:" + Price + "},";
                    }
                    Data += "]";
                }                
                return Data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetSortedSet(string key)
        {
            try
            {
                
                long chatCount = Db.SortedSetLength(key);
                var Messages = this.Db.SortedSetRangeByScore(key, chatCount, chatCount-20, order: Order.Ascending);
                string Data = "[" + string.Join(",", Messages) + "]"; // make json format
                return Data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RedisValue[] GetSortedSet(string key,string Tag = "")
        {
            try
            {

                long chatCount = Db.SortedSetLength(key);
                var Messages = this.Db.SortedSetRangeByScore(key, chatCount, chatCount - 20, order: Order.Ascending);
                //string Data = "[" + string.Join(",", Messages) + "]"; // make json format
                return Messages;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveToSortedSet(string key, T obj, string Tag)
        {
            try
            {
                if (obj != null)
                {
                    double count = this.Db.SortedSetLength(key);
                    this.Db.SortedSetAdd(key, JsonConvert.SerializeObject(obj), count + 1);
                    //this.Context.Cache.AddToSortedSet(key ,count + 1, JsonConvert.SerializeObject(obj), new[] { Tag });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SortedSetEntry> SortedSetScan(string key)
        {
            try
            {
                int BatchSize = 20;
                var Messages = this.Db.SortedSetScan(key);
                if (Messages.Count() > 0)
                {
                    return Messages.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ChatHistoryViewModel GetSortedSetDataByusername(string key, UserChatViewModel Request)
        {
            try
            {
                //IEnumerable<SortedSetEntry> peopleList1 = this.Db.SortedSetScan(key);
                //var result = peopleList1.Where(s => s.Element.ToString().Contains(Username));
                //return result;
                int BatchSize = 20;
                ChatHistoryViewModel SortedChatHistory = new ChatHistoryViewModel();
                var Messages = this.Db.SortedSetScan(key);
                if (Messages.Count() > 0)
                {
                    string Data = "[" + string.Join("},", Messages) + "}]";
                    Data = Data.Replace("}:", ",\"Score\":");
                    //string Data = GetSortedSet(key);
                    //JObject list = JObject.Parse(Data);
                    IEnumerable<SortedChatHistory> ChatHistory = JsonConvert.DeserializeObject<IEnumerable<SortedChatHistory>>(Data);
                    //SortedChatHistory = ChatHistory.Where(e => e.Name == Username).SkipLast(100);
                    var SortedChatAllData = ChatHistory.Where(e => e.Name == Request.Username).OrderByDescending(x => x.Score);
                    int Page = SortedChatAllData.Count() / BatchSize + 1;

                    var chunk = GetChunks<SortedChatHistory>(SortedChatAllData, BatchSize);
                    if(chunk.Count() > Request.Page)
                    {
                        SortedChatHistory.Data = chunk.ToArray()[Request.Page];
                        SortedChatHistory.Page = Page;
                    }
                    else
                    {
                        SortedChatHistory.Data = null;
                        SortedChatHistory.Page = 0;
                    }                   
                    
                    SortedChatHistory.TotalRecord = SortedChatAllData.Count();
                }
                return SortedChatHistory;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<IEnumerable<SortedChatHistory>> GetChunks<SortedChatHistory>(IEnumerable<SortedChatHistory> source, int batchsize)
        {
            IList<IEnumerable<SortedChatHistory>> result = null;
            if (source != null && batchsize > 0)
            {
                var list = source as List<SortedChatHistory> ?? source.ToList();
                if (list.Count > 0)
                {
                    result = new List<IEnumerable<SortedChatHistory>>();
                    for (var index = 0; index < list.Count; index += batchsize)
                    {
                        var rangesize = Math.Min(batchsize, list.Count - index);
                        result.Add(list.GetRange(index, rangesize).ToArray());
                    }
                }
            }
            return result ?? Enumerable.Empty<IEnumerable<SortedChatHistory>>().ToList();
        }

        public void SaveToSortedSetByID(string key, T obj, long ID)
        {
            try
            {
                if (obj != null)
                {
                    long count = this.Db.SortedSetLength(key);
                    this.Db.SortedSetAdd(key, JsonConvert.SerializeObject(obj), ID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SaveToSortedSetByPrice(string key, string Value, decimal Price)
        {
            try
            {
                //double value = Convert.ToDouble(Price);
                //long count = this.Db.SortedSetLength(key);
                Db.SortedSetAdd(key, Value, Convert.ToDouble(Price));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public long SortedSetCombineAndStore(string key1, string key2)
        {
            try
            {

                var peopleList2 = this.Db.SortedSetScan(key1);
                var peopleList1 = this.Db.SortedSetScan(key2);
                var result = peopleList2.Where(p => !peopleList1.Any(p2 => p2.Score == p.Score));
                long count = result.Count();
                return count;
                //long count = this.Db.SortedSetCombineAndStore(SetOperation.Intersect, "intersectKey", key1, key2);
                //var xyz1 = this.Db.SortedSetScan("intersectKey");
                //long count1 = this.Db.SortedSetCombineAndStore(SetOperation.Difference, "intersectKey1", key1, key2);
                //var xyz =  this.Db.SortedSetScan("intersectKey1");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RemoveSortedSetByID(string key, T obj)
        {
            try
            {
                if (obj != null)
                {                    
                    this.Db.SortedSetRemove(key, JsonConvert.SerializeObject(obj));
                    long count = this.Db.SortedSetLength(key);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RemoveSortedSetByIDV1(string key, double score)
        {
            try
            {
                //if (obj != null)
                //{
                    this.Db.SortedSetRemoveRangeByScore(key, score, score);
                    long count = this.Db.SortedSetLength(key);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public long SortedSetLength(string key)
        {
            try
            {
                long Count = 0;                
                Count = this.Db.SortedSetLength(key);
                return Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RedisValue[] GetSortedSetDataByScore(string Key ,long Start,long Stop)
        {
            try
            {
                RedisValue[] Value = this.Db.SortedSetRangeByScore(Key, Start, Stop, Exclude.None, Order.Descending); ;
                return Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public long GetSetLength(string key)
        {
            try
            {

                long chatCount = this.Db.SetLengthAsync(key).Result;
                //long chatCount1 = this.Context.Cache.GetRedisList<T>(key).Count;
                return chatCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
