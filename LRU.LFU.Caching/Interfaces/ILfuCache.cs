namespace LRU.LFU.Caching.Interfaces
{
    public interface ILfuCache<TKey, TValue> : ICache<TKey, TValue> where TKey : notnull
    {
        int Capacity { get; }
        event EventHandler<LfuItemEvictedEventArgs<TKey, TValue>>? ItemEvicted;
    }

    public class LfuItemEvictedEventArgs<TKey, TValue> : EventArgs
    {
        public TKey Key { get; }
        public TValue Value { get; }
        public int Frequency { get; }

        public LfuItemEvictedEventArgs(TKey key, TValue value, int frequency)
        {
            Key = key;
            Value = value;
            Frequency = frequency;
        }
    }
}
