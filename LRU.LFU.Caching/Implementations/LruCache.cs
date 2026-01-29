using LRU.LFU.Caching.Interfaces;
using LRU.LFU.Caching.Models;
using System.Collections.Concurrent;

namespace LRU.LFU.Caching.Implementations
{
    public class LruCache<TKey, TValue> : ILruCache<TKey, TValue> where TKey : notnull
    {
        private readonly int _capacity;
        private readonly ConcurrentDictionary<TKey, LruNode<TKey, TValue>> _cache;
        private readonly ReaderWriterLockSlim _lock = new();

        private LruNode<TKey, TValue>? _head;
        private LruNode<TKey, TValue>? _tail;

        public event EventHandler<LruItemEvictedEventArgs<TKey, TValue>>? ItemEvicted;

        public int Capacity => _capacity;
        public int Count => _cache.Count;

        public LruCache(int capacity = 100)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be greater than 0", nameof(capacity));

            _capacity = capacity;
            _cache = new ConcurrentDictionary<TKey, LruNode<TKey, TValue>>();
        }

        public bool TryGet(TKey key, out TValue value)
        {
            value = default!;

            _lock.EnterUpgradeableReadLock();
            try
            {
                if (!_cache.TryGetValue(key, out var node))
                    return false;

                // Move accessed node to head (most recently used)
                _lock.EnterWriteLock();
                try
                {
                    MoveToHead(node);
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
                    // Update existing value and move to head
                    existingNode.Value = value;
                    MoveToHead(existingNode);
                    return;
                }

                // Create new node
                var newNode = new LruNode<TKey, TValue>(key, value);

                if (_cache.Count >= _capacity)
                {
                    // Remove least recently used item
                    EvictLru();
                }

                // Add new node to cache and list
                _cache[key] = newNode;
                AddToHead(newNode);
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

                RemoveNode(node);
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
                _head = _tail = null;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private void AddToHead(LruNode<TKey, TValue> node)
        {
            node.Previous = null;
            node.Next = _head;

            if (_head != null)
                _head.Previous = node;

            _head = node;
            _tail ??= node;
        }

        private void RemoveNode(LruNode<TKey, TValue> node)
        {
            if (node.Previous != null)
                node.Previous.Next = node.Next;
            else
                _head = node.Next;

            if (node.Next != null)
                node.Next.Previous = node.Previous;
            else
                _tail = node.Previous;
        }

        private void MoveToHead(LruNode<TKey, TValue> node)
        {
            if (node == _head) return;

            RemoveNode(node);
            AddToHead(node);
        }

        private void EvictLru()
        {
            if (_tail == null) return;

            var lruNode = _tail;
            RemoveNode(lruNode);

            if (_cache.TryRemove(lruNode.Key, out var removedNode))
            {
                OnItemEvicted(removedNode.Key, removedNode.Value);
            }
        }

        protected virtual void OnItemEvicted(TKey key, TValue value)
        {
            ItemEvicted?.Invoke(this, new LruItemEvictedEventArgs<TKey, TValue>(key, value));
        }
    }
}
