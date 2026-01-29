namespace LRU.LFU.Caching.Interfaces
{
    public interface ILruCache<TKey, TValue> : ICache<TKey, TValue> where TKey : notnull
    {
        int Capacity { get; }
        event EventHandler<LruItemEvictedEventArgs<TKey, TValue>>? ItemEvicted;
    }
    public class LruItemEvictedEventArgs<TKey, TValue> : EventArgs
    {
        public TKey Key { get; }
        public TValue Value { get; }

        public LruItemEvictedEventArgs(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}
