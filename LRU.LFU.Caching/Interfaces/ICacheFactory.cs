namespace LRU.LFU.Caching.Interfaces
{
    public interface ICacheFactory
    {
        ICache<TKey, TValue> CreateLruCache<TKey, TValue>(int? capacity = null) where TKey : notnull;
        ICache<TKey, TValue> CreateLfuCache<TKey, TValue>(int? capacity = null) where TKey : notnull;
    }
}
