using System.Collections.Generic;

namespace Worldex.Core.Services.RadisDatabase
{
    public interface IRedisService<T>
    {
        T Get(string key);

        void Save(string key, T obj);

        void Delete(string key);

        // khushali 18-10-2018 For signalr scaleout with Redis        

        string GetSetData(string key);
        
        T GetData(string key);

        string GetHashData(string Key, string Field);

        void DeleteHash(string key);

        void DeleteTag(string key, string Tag);

        void Scan(string value, string SpecialText);

        IReadOnlyList<T> GetSetList(string Tag);

        string GetPairOrMarketData(string Value, string keySplitString,string Identifier);

        IEnumerable<T> GetConnectionIDForTest(string Token);

        IEnumerable<string> GetKey(string Tag);

        void SaveToSet(string key, T obj, string Tag);

        void SaveTagsToSetMember(string Key, string Value, string Tag);

        void RemoveSetMember(string Key, string Value);

        void SaveToHash(string key, T obj, string Tag1, string Tag2);

        void SaveToHash(string key, T obj, string Tag1);

        void SaveToHash(string key, T obj);

        void SaveToSortedSet(string key, T obj, string Tag);
    }
}
