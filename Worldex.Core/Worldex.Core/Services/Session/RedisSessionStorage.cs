using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Worldex.Core.Services.Session
{
    public class RedisSessionStorage
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public RedisSessionStorage(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public void SetObject(string key, object value, string EncyptedKey)
        {
            if (!string.IsNullOrEmpty(key) && value != null)
            {
                if (!_session.IsAvailable)
                    _session.LoadAsync();
                if (value != null)
                {
                    string Data = JsonConvert.SerializeObject(value);
                    if (!string.IsNullOrEmpty(EncyptedKey))
                        Data = SessionDataEncyptedDecrypted.Encrypt(Data, EncyptedKey);


                    _session.SetString(key, Data);
                    _session.CommitAsync();
                }
            }
        }

        public void RemoveSession(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _session.Remove(key);
            }
        }


        public void RemoveSessionAllData()
        {
            _session.Clear();
        }

        public T GetObjectFromJson<T>(string key, string EncyptedKey)
        {

            string data = _session.GetString(key);

            if (data != null)
            {
                if (!string.IsNullOrEmpty(EncyptedKey))
                    data = SessionDataEncyptedDecrypted.Decrypt(data, EncyptedKey);
            }

            return data == null ? default(T) : JsonConvert.DeserializeObject<T>(data);
        }
    }
}
