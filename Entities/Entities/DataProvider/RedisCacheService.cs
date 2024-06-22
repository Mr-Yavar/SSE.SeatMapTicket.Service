using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;


namespace Entities.DataProvider
{
    public class RedisCacheService
    {
        private readonly IDistributedCache? _cache;
        private readonly IDatabase? _redis;
       
        public RedisCacheService(IDistributedCache cache,IDatabase redis )
        {
            _cache = cache;
            _redis = redis;


        }

        public List<Guid> LockedTickets(string ticketPatterns)
        {

            return _redis.Multiplexer.GetServers()[0].Keys(database:0,pattern: ticketPatterns).Select(x => Guid.Parse(x.ToString().Split(".")[1])).ToList();

        }
        
        public bool IsExist(string key)
        {
            return _redis.KeyExists(key);
            
            
        }


        public bool AddKey<T>(string key,T data,TimeSpan cacheDuration) {
            var jsonData = JsonSerializer.Serialize(data);
           return  _redis.StringSet(key, jsonData, cacheDuration,true);
        }

        public bool Remove(string key)
        {
            return _redis.KeyDelete(key);
            
        }

        public T GetCachedData<T>(string key)
        {
            var jsonData = _cache.GetString(key);
            if (jsonData == null)
                return default(T);
            return JsonSerializer.Deserialize<T>(jsonData);
        }

        public void SetCachedData<T>(string key, T data, TimeSpan cacheDuration)
        {
            var jsonData = JsonSerializer.Serialize(data);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheDuration,
                
            };

            if (cacheDuration == TimeSpan.Zero)
            {
                _cache.SetString(key, jsonData);

            }
            else
            {
                _cache.SetString(key, jsonData, options);

            }

        }

       

    }
}
