namespace LRU.LFU.Caching.Interfaces
{
    public interface ICache<TKey, TValue> where TKey : notnull
    {
        bool TryGet(TKey key, out TValue value);
        void Put(TKey key, TValue value);
        bool Remove(TKey key);
        void Clear();
        int Count { get; }
    }
}
