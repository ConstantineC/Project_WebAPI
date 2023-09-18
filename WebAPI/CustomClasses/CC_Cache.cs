using System.Runtime.Caching;
using static WebAPI.CustomClasses.CC_Cache;

namespace WebAPI.CustomClasses
{
    public class CC_Cache
    {
        public class CacheObject
        {
            public string content="";
        }

        public class CacheHandler
        {
           
            public CacheItemPolicy GetCachePolicy(float expiration_amount)
            {
                var cacheItemPolicy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(expiration_amount)
                };
                return cacheItemPolicy;
            }

            public void AddContent(ObjectCache cache ,string cache_key,string addedContent,float expiration_amount)
            {
                //add to cache
                CC_Cache.CacheObject savedObject = new CC_Cache.CacheObject();
                savedObject.content = addedContent;

                CacheItemPolicy policy = GetCachePolicy(expiration_amount);

                try
                {
                    if (cache.Add(cache_key, savedObject, policy))
                        Console.WriteLine("Succesfully added");
                    else
                        Console.WriteLine("Did not add");
                }
                catch (Exception e) { 
                    Console.WriteLine(e.Message); 
                }
            }

        }
    }
}
