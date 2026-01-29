namespace LRU.LFU.Caching.Models
{
    internal class LfuNode<TKey, TValue> where TKey : notnull
    {
        public TKey Key { get; set; } = default!;
        public TValue Value { get; set; } = default!;
        public int Frequency { get; set; }
        public LfuNode<TKey, TValue>? Previous { get; set; }
        public LfuNode<TKey, TValue>? Next { get; set; }

        public LfuNode() { }

        public LfuNode(TKey key, TValue value, int frequency = 1)
        {
            Key = key;
            Value = value;
            Frequency = frequency;
        }
    }
}
