using LRU.LFU.Caching.Interfaces;
using LRU.LFU.Caching.Models;
using System.Collections.Concurrent;

namespace LRU.LFU.Caching.Implementations
{
    public class LfuCache<TKey, TValue> : ILfuCache<TKey, TValue> where TKey : notnull
    {
        private readonly int _capacity;
        private readonly ConcurrentDictionary<TKey, LfuNode<TKey, TValue>> _cache;
        private readonly Dictionary<int, LinkedList<LfuNode<TKey, TValue>>> _frequencyMap;
        private readonly ReaderWriterLockSlim _lock = new();

        private int _minFrequency;
        private int _size;

        public event EventHandler<LfuItemEvictedEventArgs<TKey, TValue>>? ItemEvicted;

        public int Capacity => _capacity;
        public int Count => _cache.Count;

        public LfuCache(int capacity = 100)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be greater than 0", nameof(capacity));

            _capacity = capacity;
            _cache = new ConcurrentDictionary<TKey, LfuNode<TKey, TValue>>();
            _frequencyMap = new Dictionary<int, LinkedList<LfuNode<TKey, TValue>>>();
            _minFrequency = 0;
            _size = 0;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            value = default!;

            _lock.EnterUpgradeableReadLock();
            try
            {
                if (!_cache.TryGetValue(key, out var node))
                    return false;

                // Update frequency
                _lock.EnterWriteLock();
                try
                {
                    UpdateFrequency(node);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }

                value = node.Value;
                return true;
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        public void Put(TKey key, TValue value)
        {
            _lock.EnterWriteLock();
            try
            {
                if (_cache.TryGetValue(key, out var existingNode))
                {
                    // Update existing value and frequency
                    existingNode.Value = value;
                    UpdateFrequency(existingNode);
                    return;
                }

                if (_size >= _capacity)
                {
                    EvictLfu();
                }

                // Create new node with frequency 1
                var newNode = new LfuNode<TKey, TValue>(key, value, 1);
                _cache[key] = newNode;

                // Add to frequency map
                if (!_frequencyMap.ContainsKey(1))
                {
                    _frequencyMap[1] = new LinkedList<LfuNode<TKey, TValue>>();
                }
                _frequencyMap[1].AddLast(newNode);

                _minFrequency = 1;
                _size++;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool Remove(TKey key)
        {
            _lock.EnterWriteLock();
            try
            {
                if (!_cache.TryRemove(key, out var node))
                    return false;

                // Remove from frequency map
                var freqList = _frequencyMap[node.Frequency];
                freqList.Remove(node);

                if (freqList.Count == 0)
                {
                    _frequencyMap.Remove(node.Frequency);
                    if (node.Frequency == _minFrequency)
                    {
                        _minFrequency++;
                    }
                }

                _size--;
                return true;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _cache.Clear();
                _frequencyMap.Clear();
                _minFrequency = 0;
                _size = 0;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private void UpdateFrequency(LfuNode<TKey, TValue> node)
        {
            // Remove from current frequency list
            var oldFreq = node.Frequency;
            var oldList = _frequencyMap[oldFreq];
            oldList.Remove(node);

            // If this was the last node at this frequency, clean up
            if (oldList.Count == 0)
            {
                _frequencyMap.Remove(oldFreq);
                if (oldFreq == _minFrequency)
                {
                    _minFrequency++;
                }
            }

            // Increase frequency
            var newFreq = oldFreq + 1;
            node.Frequency = newFreq;

            // Add to new frequency list
            if (!_frequencyMap.ContainsKey(newFreq))
            {
                _frequencyMap[newFreq] = new LinkedList<LfuNode<TKey, TValue>>();
            }
            _frequencyMap[newFreq].AddLast(node);
        }

        private void EvictLfu()
        {
            var minFreqList = _frequencyMap[_minFrequency];
            var nodeToEvict = minFreqList.First!.Value;

            // Remove from cache and frequency list
            _cache.TryRemove(nodeToEvict.Key, out _);
            minFreqList.RemoveFirst();

            if (minFreqList.Count == 0)
            {
                _frequencyMap.Remove(_minFrequency);
            }

            _size--;
            OnItemEvicted(nodeToEvict.Key, nodeToEvict.Value, nodeToEvict.Frequency);
        }

        protected virtual void OnItemEvicted(TKey key, TValue value, int frequency)
        {
            ItemEvicted?.Invoke(this, new LfuItemEvictedEventArgs<TKey, TValue>(key, value, frequency));
        }
    }
}
