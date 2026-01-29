using LRU.LFU.Caching.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LRU.LFU.Caching.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheDemoController : ControllerBase
    {
        private readonly ICacheFactory _cacheFactory;
        private readonly ILruCache<string, object> _lruCache;
        private readonly ILfuCache<string, object> _lfuCache;

        public CacheDemoController(
            ICacheFactory cacheFactory,
            ILruCache<string, object> lruCache,
            ILfuCache<string, object> lfuCache)
        {
            _cacheFactory = cacheFactory;
            _lruCache = lruCache;
            _lfuCache = lfuCache;

            // Subscribe to eviction events
            _lruCache.ItemEvicted += OnLruItemEvicted;
            _lfuCache.ItemEvicted += OnLfuItemEvicted;
        }

        [HttpPost("lru")]
        public IActionResult AddToLru([FromQuery] string key, [FromBody] object value)
        {
            _lruCache.Put(key, value);
            return Ok(new { Key = key, Cache = "LRU" });
        }

        [HttpGet("lru/{key}")]
        public IActionResult GetFromLru(string key)
        {
            if (_lruCache.TryGet(key, out var value))
            {
                return Ok(value);
            }
            return NotFound();
        }

        [HttpPost("lfu")]
        public IActionResult AddToLfu([FromQuery] string key, [FromBody] object value)
        {
            _lfuCache.Put(key, value);
            return Ok(new { Key = key, Cache = "LFU" });
        }

        [HttpGet("lfu/{key}")]
        public IActionResult GetFromLfu(string key)
        {
            if (_lfuCache.TryGet(key, out var value))
            {
                return Ok(value);
            }
            return NotFound();
        }

        [HttpPost("factory/lru/{key}")]
        public IActionResult AddWithFactory(string key, [FromBody] string value)
        {
            var cache = _cacheFactory.CreateLruCache<string, string>();
            cache.Put(key, value);
            return Ok(new { CacheType = "LRU", Count = cache.Count });
        }

        private void OnLruItemEvicted(object? sender, LruItemEvictedEventArgs<string, object> e)
        {
            // Handle LRU eviction (e.g., logging, cleanup)
            Console.WriteLine($"LRU Item evicted: Key={e.Key}");
        }

        private void OnLfuItemEvicted(object? sender, LfuItemEvictedEventArgs<string, object> e)
        {
            // Handle LFU eviction (e.g., logging, cleanup)
            Console.WriteLine($"LFU Item evicted: Key={e.Key}, Frequency={e.Frequency}");
        }
    }
}
