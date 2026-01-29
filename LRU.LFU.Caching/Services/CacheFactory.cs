using LRU.LFU.Caching.Configuration;
using LRU.LFU.Caching.Implementations;
using LRU.LFU.Caching.Interfaces;

namespace LRU.LFU.Caching.Services
{
    public class CacheFactory : ICacheFactory
    {
        private readonly CacheSettings _cacheSettings;

        public CacheFactory(CacheSettings cacheSettings)
        {
            _cacheSettings = cacheSettings;
        }

        public ICache<TKey, TValue> CreateLruCache<TKey, TValue>(int? capacity = null)
            where TKey : notnull
        {
            return new LruCache<TKey, TValue>(capacity ?? _cacheSettings.LruCacheCapacity);
        }

        public ICache<TKey, TValue> CreateLfuCache<TKey, TValue>(int? capacity = null)
            where TKey : notnull
        {
            return new LfuCache<TKey, TValue>(capacity ?? _cacheSettings.LfuCacheCapacity);
        }
    }
}
