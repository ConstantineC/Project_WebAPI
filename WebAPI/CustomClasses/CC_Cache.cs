using System.Runtime.Caching;

namespace WebAPI.CustomClasses
{
    public class CC_Cache
    {
        public class CacheHandler
        {
            public MemoryCache InitializeCache(string cache_name)
            {
                MemoryCache cache = new MemoryCache(cache_name);
                return cache;
            }

            public CacheItemPolicy GetCachePolicy(float expiration_amount)
            {
                var cacheItemPolicy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(expiration_amount)
                };
                return cacheItemPolicy;
            }
            public bool AddContents(MemoryCache cache, string cache_key,string addvalue,CacheItemPolicy policy)
            {
                return cache.Add(cache_key, addvalue, policy);
            }

            public Object GetContents(MemoryCache cache, string cache_key)
            {
                var result=cache.Get(cache_key);
                return result;
            }

            public bool RemoveContents(MemoryCache cache,string cache_key)
            {
                if (cache.Remove(cache_key) == null)
                    return false;
                return true;
            }
        }
    }
}
