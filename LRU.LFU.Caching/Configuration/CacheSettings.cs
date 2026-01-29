namespace LRU.LFU.Caching.Configuration
{
    public class CacheSettings
    {
        public int LruCacheCapacity { get; set; } = 100;
        public int LfuCacheCapacity { get; set; } = 100;
        public string DefaultCacheType { get; set; } = "LRU";
    }
}
