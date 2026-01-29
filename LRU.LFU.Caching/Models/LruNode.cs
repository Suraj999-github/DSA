namespace LRU.LFU.Caching.Models
{
    internal class LruNode<TKey, TValue> where TKey : notnull
    {
        public TKey Key { get; set; } = default!;
        public TValue Value { get; set; } = default!;
        public LruNode<TKey, TValue>? Previous { get; set; }
        public LruNode<TKey, TValue>? Next { get; set; }

        public LruNode() { }

        public LruNode(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}
